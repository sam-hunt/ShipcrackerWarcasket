using System.Collections.Generic;
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
// cells: walkable and in sight, tested with the same predicate as CanHitTarget.
public class Ability_BreachJump : Ability
{
    // "Unlimited" as a finite number so VEF's range arithmetic and ring drawing stay sane
    // (DrawHighlight already skips the ring above GenRadial.MaxRadialPatternRadius).
    private const float SpaceRange = 10000f;

    // The space preview recomputes when the camera, map or wearer moves, and at most this often
    // otherwise, so a door opening or a wall dropping shows up without a per-frame LoS sweep.
    private const int SpacePreviewRefreshTicks = 30;

    private readonly List<IntVec3> leanScratch = new();
    private readonly List<IntVec3> roofScratch = new();
    private readonly List<IntVec3> spacePreviewCells = new();
    private Map spacePreviewMap;
    private IntVec3 spacePreviewOrigin;
    private CellRect spacePreviewRect;
    private int spacePreviewTick = int.MinValue;

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
    private bool CanLandOn(IntVec3 cell, Map map)
    {
        if (!cell.WalkableBy(map, pawn))
            return false;

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

    // Outlines the reachable cells inside the camera view in the range-ring colour. Bounding
    // the sweep to the view keeps the per-cell line-of-sight walk affordable on an orbit map,
    // and the result is cached between camera moves.
    private void DrawSpaceValidCells()
    {
        var map = pawn.Map;
        var rect = Find.CameraDriver.CurrentViewRect.ExpandedBy(1).ClipInsideMap(map);
        var tick = Find.TickManager.TicksGame;
        if (map != spacePreviewMap || pawn.Position != spacePreviewOrigin || rect != spacePreviewRect
            || tick - spacePreviewTick >= SpacePreviewRefreshTicks)
        {
            spacePreviewMap = map;
            spacePreviewOrigin = pawn.Position;
            spacePreviewRect = rect;
            spacePreviewTick = tick;
            spacePreviewCells.Clear();
            foreach (var cell in rect)
                if (CanLandOn(cell, map))
                    spacePreviewCells.Add(cell);
        }

        if (spacePreviewCells.Count > 0)
            GenDraw.DrawFieldEdges(spacePreviewCells, def.rangeRingColor);
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
            PunchRoof(pawn.Position, map, pawn);

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
            PunchRoof(center, map, wearer);

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
    private void PunchRoof(IntVec3 center, Map map, Pawn wearer)
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
            if (cell == wearer.Position)
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
