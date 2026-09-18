using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;
using VEF.Abilities;
using Ability = VEF.Abilities.Ability;

namespace ShipcrackerWarcasket;

// The Shipcracker armor's jump. Modelled on VFEP's Ability_PowerJump (Aerial set) but with
// its own rules:
//  - On a planet: any walkable cell within the wearer's SCWC_BreachJumpRange, no sight test
//    (Aerial parity; every worn piece can add range through equippedStatOffsets).
//  - In space (Odyssey's vacuum biome or the Orbit planet layer): no range limit, but the
//    target must be in line of sight. Both checks are vanilla fields, so no DLC gate.
//  - No takeoff blast. The landing fires SCWC_Breach through PawnFlyer_BreachJump, scaled by
//    the wearer's SCWC_BreachPower, so the shoulders can make it hit harder.
//  - Takeoff and landing both punch through any non-thick roof within roofPunchRadius
//    (constructed, thin rock, SOS2 hull; never overhead mountain), unless the jump begins and
//    ends in the same indoor room, which reads as a hop across the floor, not the ceiling.
// Fuel comes from a CompApparelReloadable on the same apparel as the ability comp.
//
// Targeting previews: the vanilla Targeter hands DrawHighlight an invalid target whenever the
// hovered cell fails CanHitTarget, so VEF's base draws the target highlight and a
// radiusRingColor ring of GetRadiusForPawn() only over cells the jump can actually reach; we
// return the breach radius from that so the ring is the landing blast footprint. On a planet
// VEF's range ring stays as-is (Aerial parity). In space the range ring is meaningless, so we
// outline every reachable cell in view instead, the way vanilla's Verb_Jump outlines its valid
// cells: walkable and in sight, tested with the same predicate as CanHitTarget. That sweep is
// budgeted, cached per cell and re-run only when the map reports a change (see
// DrawSpaceValidCells): under Vanilla Gravship Expanded every space cell is walkable, so the
// view can hold tens of thousands of candidates, each needing a line-of-sight walk from the
// wearer, and a full sweep per camera move stalled the frame.
public class Ability_BreachJump : Ability
{
    // "Unlimited" as a finite number so VEF's range arithmetic and ring drawing stay sane
    // (DrawHighlight already skips the ring above GenRadial.MaxRadialPatternRadius).
    private const float SpaceRange = 10000f;

    // Main-thread time spent testing preview cells per frame while a sweep is in progress. A
    // zoomed-in view (a couple of thousand cells) completes within a frame or two; a whole orbit
    // map fills in over a second or so instead of freezing the game for that long. Once the view
    // is swept and nothing has changed, no cell is tested at all.
    private const double SpacePreviewBudgetMs = 2.0;

    private readonly List<IntVec3> leanScratch = new();
    private readonly List<IntVec3> roofScratch = new();
    private readonly List<IntVec3> spacePreviewCells = new();
    private readonly HashSet<IntVec3> spacePreviewUntested = new();
    private static readonly Stopwatch spacePreviewWatch = new();

    // Per-cell preview cache for spacePreviewMap, indexed by CellIndices: 0 = never tested,
    // otherwise the generation it was tested in shifted over two flag bits, walkable and
    // landable (see PreviewState). Walkable is kept separately so the path-cost event handler
    // can tell a real walkability flip from filth or a chunk landing in a cell that was already
    // out of sight. Cells of the current generation are fresh; older ones keep drawing as they
    // were but are re-tested by the sweep. The generation bumps only between sweeps, and only
    // when the wearer has changed cell or a map event has set spacePreviewDirty, so a static
    // scene costs no sight tests once the view is swept. It is never cleared for a wearer move,
    // which used to wipe the outline and regrow it from the bottom of the view every step.
    private int[] spacePreviewState;
    private Map spacePreviewMap;
    private IntVec3 spacePreviewOrigin;
    private int spacePreviewGeneration;
    private bool spacePreviewDirty;

    // The sweep: a cursor into the row-major enumeration of spacePreviewRect, carried across
    // frames so a re-test covers the whole view before another begins. Restarting from the
    // bottom row on every refresh starved the top of the view of re-tests entirely.
    private CellRect spacePreviewRect;
    private int spacePreviewCursor;

    private const int PreviewWalkable = 1;
    private const int PreviewLandable = 2;

    private static int PreviewState(int generation, bool walkable, bool landable) =>
        (generation << 2) | (walkable ? PreviewWalkable : 0) | (landable ? PreviewLandable : 0);

    public BreachJumpExtension Ext => def.GetModExtension<BreachJumpExtension>();

    public CompApparelReloadable Tank => holder?.TryGetComp<CompApparelReloadable>();

    public static bool IsSpaceMap(Map map) =>
        map != null && (map.Biome?.inVacuum == true || map.Tile.LayerDef?.isSpace == true);

    // Whether any loaded def can produce a map IsSpaceMap would accept: Odyssey's vacuum biome
    // and Orbit layer, or another mod's equivalent. Same two fields as IsSpaceMap, so the
    // tooltip's vacuum sentence appears exactly when the space rules can ever apply. Defs are
    // immutable after load, so this is computed once.
    private static bool? spaceMapsPossible;

    public static bool SpaceMapsPossible => spaceMapsPossible ??=
        DefDatabase<BiomeDef>.AllDefsListForReading.Any(b => b.inVacuum)
        || DefDatabase<PlanetLayerDef>.AllDefsListForReading.Any(l => l.isSpace);

    public bool InSpace => IsSpaceMap(pawn?.Map);

    public float PlanetRange => pawn.GetStatValue(SCWC_DefOf.SCWC_BreachJumpRange);

    public override float GetRangeForPawn() => InSpace ? SpaceRange : PlanetRange;

    // The landing blast footprint, so VEF's DrawHighlight previews it at the hovered cell.
    public override float GetRadiusForPawn() => Ext.breachRadius;

    // sightCheck (def.requireLineOfSight) is deliberately ignored: the sight rule follows the map.
    public override bool CanHitTarget(LocalTargetInfo target, bool sightCheck)
    {
        var map = pawn?.Map;
        if (map == null || !target.IsValid)
            return false;

        var cell = target.Cell;
        return cell.InBounds(map) && CanLandOn(cell, map);
    }

    // Shared by the hit test and the space preview so the outline can never disagree with
    // what a click accepts. Caller guarantees the cell is in bounds.
    private bool CanLandOn(IntVec3 cell, Map map) => cell.WalkableBy(map, pawn) && CanReach(cell, map);

    // The range or sight half of CanLandOn, for a cell already known to be walkable.
    private bool CanReach(IntVec3 cell, Map map)
    {
        if (!IsSpaceMap(map))
            return cell.DistanceTo(pawn.Position) <= PlanetRange;

        if (GenSight.LineOfSight(pawn.Position, cell, map))
            return true;

        // Same lean-out allowance VEF's base sight check grants.
        leanScratch.Clear();
        ShootLeanUtility.LeanShootingSourcesFromTo(pawn.Position, cell, map, leanScratch);
        foreach (var source in leanScratch)
            if (GenSight.LineOfSight(source, cell, map))
                return true;

        return false;
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        base.DrawHighlight(target);
        // DrawFieldEdges sizes its grid from Find.CurrentMap, so only outline the map on screen.
        if (InSpace && pawn.Map == Find.CurrentMap)
            DrawSpaceValidCells();
    }

    // Outlines the reachable cells inside the camera view in the range-ring colour. Only cells
    // in view are ever tested and results persist across frames, camera moves and wearer moves.
    // Each frame advances the sweep cursor through the view, testing cells that are not of the
    // current generation until SpacePreviewBudgetMs is spent; fresh cells are skipped for the
    // cost of an array read. Whatever is known landable is drawn, and edges facing never-tested
    // cells are suppressed, so the fill carves shadows into view rather than sweeping a visible
    // frontier line up the map.
    private void DrawSpaceValidCells()
    {
        var map = pawn.Map;
        var origin = pawn.Position;
        var cellCount = map.cellIndices.NumGridCells;
        if (map != spacePreviewMap || spacePreviewState == null)
        {
            if (spacePreviewState == null || spacePreviewState.Length != cellCount)
                spacePreviewState = new int[cellCount];
            else
                System.Array.Clear(spacePreviewState, 0, cellCount);
            SetPreviewEventMap(map);
            spacePreviewOrigin = origin;
            spacePreviewGeneration = 1;
            spacePreviewDirty = false;
            spacePreviewRect = default;
            spacePreviewCursor = 0;
        }

        var rect = Find.CameraDriver.CurrentViewRect.ExpandedBy(1).ClipInsideMap(map);
        if (rect != spacePreviewRect)
        {
            spacePreviewRect = rect;
            spacePreviewCursor = 0;
        }
        var area = rect.Area;

        // A new generation starts only once the previous sweep has covered the whole view, so a
        // burst of map events coalesces into one re-test and no part of the view is starved.
        // CanLandOn reads the wearer's live cell, so cells tested after a mid-sweep step already
        // use the new origin; the mismatch recorded here just schedules the pass that makes the
        // rest agree.
        if (spacePreviewCursor >= area && (spacePreviewDirty || origin != spacePreviewOrigin))
        {
            spacePreviewGeneration++;
            spacePreviewOrigin = origin;
            spacePreviewDirty = false;
            spacePreviewCursor = 0;
        }

        var indices = map.cellIndices;
        var generation = spacePreviewGeneration;
        var width = rect.Width;
        spacePreviewWatch.Restart();
        for (; spacePreviewCursor < area; spacePreviewCursor++)
        {
            var cell = new IntVec3(rect.minX + spacePreviewCursor % width, 0, rect.minZ + spacePreviewCursor / width);
            var i = indices.CellToIndex(cell);
            if (spacePreviewState[i] >> 2 == generation)
                continue;
            var walkable = cell.WalkableBy(map, pawn);
            spacePreviewState[i] = PreviewState(generation, walkable, walkable && CanReach(cell, map));
            if (spacePreviewWatch.Elapsed.TotalMilliseconds >= SpacePreviewBudgetMs)
            {
                spacePreviewCursor++;
                break;
            }
        }

        // Collect everything known to be landable, at whatever age, plus the never-tested cells
        // next to them: DrawFieldEdges skips edges that face a cell in ignoreBorderCells.
        spacePreviewCells.Clear();
        spacePreviewUntested.Clear();
        var size = map.Size;
        foreach (var cell in rect)
        {
            if ((spacePreviewState[indices.CellToIndex(cell)] & PreviewLandable) == 0)
                continue;
            spacePreviewCells.Add(cell);
            for (var d = 0; d < 4; d++)
            {
                var n = cell + GenAdj.CardinalDirections[d];
                if (n.x >= 0 && n.z >= 0 && n.x < size.x && n.z < size.z && spacePreviewState[indices.CellToIndex(n)] == 0)
                    spacePreviewUntested.Add(n);
            }
        }

        if (spacePreviewCells.Count > 0)
            GenDraw.DrawFieldEdges(spacePreviewCells, def.rangeRingColor,
                ignoreBorderCells: spacePreviewUntested.Count > 0 ? spacePreviewUntested : null);
    }

    // The preview cache is invalidated by the map's own change events rather than on a timer.
    // CanLandOn reads two things: walkability, which is the path grid (PathCostRecalculate fires
    // from its single recalculation point), and line of sight, which is edifices and door state
    // (BuildingSpawned/Despawned, DoorOpened/Closed). ShootLeanUtility reads the same inputs.
    // Path-cost events fire for anything with a path cost (filth, chunks, plants, buildings), so
    // they only mark the cache dirty when the cell's walkability actually differs from what was
    // cached; buildings and doors change sight for every cell behind them, so they always do.
    // An event that arrives while the wearer is off this map is taken at face value, since the
    // cache cannot be checked against a pawn that is not there. The subscription
    // holds this ability for the map's lifetime; the map object is discarded on load and on
    // destruction, which releases it, and the handlers are a few field reads each.
    private void SetPreviewEventMap(Map map)
    {
        var old = spacePreviewMap?.events;
        if (old != null)
        {
            old.BuildingSpawned -= OnPreviewBuildingChanged;
            old.BuildingDespawned -= OnPreviewBuildingChanged;
            old.DoorOpened -= OnPreviewDoorChanged;
            old.DoorClosed -= OnPreviewDoorChanged;
            old.PathCostRecalculate -= OnPreviewPathCostRecalculated;
        }
        spacePreviewMap = map;
        var events = map?.events;
        if (events != null)
        {
            events.BuildingSpawned += OnPreviewBuildingChanged;
            events.BuildingDespawned += OnPreviewBuildingChanged;
            events.DoorOpened += OnPreviewDoorChanged;
            events.DoorClosed += OnPreviewDoorChanged;
            events.PathCostRecalculate += OnPreviewPathCostRecalculated;
        }
    }

    private void OnPreviewBuildingChanged(Building _) => spacePreviewDirty = true;

    private void OnPreviewDoorChanged(Building_Door _) => spacePreviewDirty = true;

    private void OnPreviewPathCostRecalculated(IntVec3 cell)
    {
        var map = spacePreviewMap;
        if (spacePreviewDirty || map == null || spacePreviewState == null || !cell.InBounds(map))
            return;
        if (pawn?.Map != map)
        {
            spacePreviewDirty = true;
            return;
        }
        var state = spacePreviewState[map.cellIndices.CellToIndex(cell)];
        // Only a change in walkability can alter this cell's answer through the path grid;
        // sight changes arrive through the building and door events.
        if (state != 0 && ((state & PreviewWalkable) != 0) != cell.WalkableBy(map, pawn))
            spacePreviewDirty = true;
    }

    public override bool IsEnabledForPawn(out string reason)
    {
        if (!base.IsEnabledForPawn(out reason))
            return false;

        var tank = Tank;
        if (tank == null)
            return false;

        var shortfall = Ext.fuelPerJump - tank.RemainingCharges;
        if (shortfall > 0)
        {
            // Vanilla's own "needs to be reloaded with N <ammo>" text, so it names whatever
            // fuel the tank currently takes (chemfuel, or astrofuel under VGE1) with no Keyed
            // string of our own.
            var ammoNeeded = shortfall * tank.Props.ammoCountPerCharge;
            reason = tank.DisabledReason(ammoNeeded, ammoNeeded);
            return false;
        }

        return true;
    }

    // The label the gizmo and tooltip show right now: the def's on a planet, vacuumLabel in space.
    private string CurrentLabelCap =>
        InSpace && !Ext.vacuumLabel.NullOrEmpty() ? Ext.vacuumLabel.CapitalizeFirst() : def.LabelCap.ToString();

    // VEF's tooltip is "LabelCap\n\ndescription\n\n" followed by generated stat lines. The head
    // is rebuilt with the current label and, when a space map can exist in this game, the
    // vacuum sentence after the description; the generated tail is kept as VEF wrote it.
    public override string GetDescriptionForPawn()
    {
        var text = base.GetDescriptionForPawn();
        string head = def.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + def.description;
        if (!text.StartsWith(head, System.StringComparison.Ordinal))
            return text;

        var description = def.description;
        if (SpaceMapsPossible && !Ext.vacuumDescription.NullOrEmpty())
            description += " " + Ext.vacuumDescription;

        return CurrentLabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + description + text.Substring(head.Length);
    }

    // In space the button takes the space icon and label; VEF's Command_Ability copies both from
    // the def in its constructor, so they are overwritten afterwards, as VFEP's Command_Grapple
    // does for the hook's reload state. Its shrunk-mode tooltip still prefixes def.LabelCap.
    public override Gizmo GetGizmo()
    {
        var gizmo = base.GetGizmo();
        if (InSpace && gizmo is Command command)
        {
            if (Ext.SpaceIcon is Texture2D icon)
                command.icon = icon;
            command.defaultLabel = CurrentLabelCap;
        }
        return gizmo;
    }

    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);

        var map = pawn.Map;
        var tank = Tank;
        for (var i = 0; i < Ext.fuelPerJump; i++)
            tank?.UsedOnce();

        // Space takeoff: a one-shot distortion ring at the origin (the flyer's own effecter
        // carries the launch sound and flash). Fired before MakeFlyer despawns the wearer,
        // while pawn.Position is still the launch cell. No planet equivalent: there the jump
        // has no takeoff effect beyond the flyer's flame, as on Aerial.
        if (IsSpaceMap(map))
            SCWC_DefOf.SCWC_BreachJumpBlastOff.Spawn(pawn.Position, map).Cleanup();

        var destination = targets[0].Cell;
        // Decided once at launch, while the wearer still stands at the origin, and carried by the
        // flyer: Room objects are rebuilt whenever regions change, so the landing cannot re-ask.
        var punchRoof = ShouldPunchRoof(pawn.Position, destination, map);
        if (punchRoof)
            PunchRoof(pawn.Position, map, pawn, crush: false);

        var flyer = (PawnFlyer_BreachJump)PawnFlyer.MakeFlyer(SCWC_DefOf.SCWC_BreachJumpFlyer, pawn, destination, null, null, true);
        flyer.ability = this;
        flyer.DestinationCell = destination;
        flyer.punchRoof = punchRoof;
        GenSpawn.Spawn(flyer, destination, map);
    }

    // Vanilla merges neighbouring districts into one Room except across doors
    // (RegionAndRoomUpdater.ShouldBeInTheSameRoom), so a Room is the space bounded by walls and
    // doors, and all connected outdoors is one Room. A jump inside one indoor Room never crosses
    // a wall, so there is no ceiling to come through. The outdoor Room (any Room touching the
    // map edge, vanilla's own outdoors test) is exempt: a hop from open ground to a cell under a
    // thin rock overhang, or from under one back out, still smashes the overhang.
    private static bool ShouldPunchRoof(IntVec3 origin, IntVec3 destination, Map map)
    {
        var room = RegionAndRoomQuery.RoomAt(origin, map);
        return room == null || room != RegionAndRoomQuery.RoomAt(destination, map) || room.TouchesMapEdge;
    }

    // Called by the flyer once the wearer is back on the map. Armor penetration is passed
    // explicitly: GenExplosion reads the def's default only when no damage amount is handed
    // in, and otherwise derives it as damage x 0.015. The wearer's SCWC_BreachPower is applied
    // to buildings alone by DamageWorker_Breach, so it is deliberately absent here.
    public void DoBreach(IntVec3 center, Map map, Pawn wearer, bool punchRoof)
    {
        if (punchRoof)
            PunchRoof(center, map, wearer, crush: true);

        GenExplosion.DoExplosion(center, map, Ext.breachRadius, SCWC_DefOf.SCWC_Breach, wearer, Ext.breachDamage,
            Ext.breachArmorPenetration, ignoredThings: new List<Thing> { wearer });
    }

    // Vanilla's roof punch is Skyfaller.HitRoof: play the roof's punch-through sound, then drop
    // the roof through RoofCollapserImmediate, which clears it, spawns its rubble filth and
    // crushes whatever stands in the cell. The thickness gate is DropCellFinder's: thick roofs
    // (overhead mountain) are never punched. Explosions never touch the roof grid, so this is
    // the only way the jump reaches a ceiling. The wearer's own cell loses its roof silently
    // through the grid instead, so they go through the ceiling without being crushed by it; a
    // drop pod likewise shields its occupants, who leave the pod after the roof is already gone.
    // Called at takeoff with the wearer at the origin and at landing with them at the center.
    // With crush false (takeoff) every cell goes the silent way: the sound plays and the roof
    // opens, but nothing under it is hurt and no rubble falls, so squadmates standing beside
    // the wearer at launch are spared. The landing keeps the full collapse.
    private void PunchRoof(IntVec3 center, Map map, Pawn wearer, bool crush)
    {
        roofScratch.Clear();
        RoofDef punched = null;
        foreach (var cell in GenRadial.RadialCellsAround(center, Ext.roofPunchRadius, true))
        {
            if (!cell.InBounds(map))
                continue;
            var roof = cell.GetRoof(map);
            if (roof == null || roof.isThickRoof)
                continue;

            punched ??= roof;
            if (!crush || cell == wearer.Position)
                map.roofGrid.SetRoof(cell, null);
            else
                roofScratch.Add(cell);
        }

        if (punched == null)
            return;
        if (!punched.soundPunchThrough.NullOrUndefined())
            punched.soundPunchThrough.PlayOneShot(new TargetInfo(center, map));
        if (roofScratch.Count > 0)
            RoofCollapserImmediate.DropRoofInCells(roofScratch, map);
    }
}
