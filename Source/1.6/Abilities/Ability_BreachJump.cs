using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
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
    private readonly List<IntVec3> spacePreviewCells = new();
    private Map spacePreviewMap;
    private IntVec3 spacePreviewOrigin;
    private CellRect spacePreviewRect;
    private int spacePreviewTick = int.MinValue;

    public BreachJumpExtension Ext => def.GetModExtension<BreachJumpExtension>();

    public CompApparelReloadable Tank => holder?.TryGetComp<CompApparelReloadable>();

    public static bool IsSpaceMap(Map map) =>
        map != null && (map.Biome?.inVacuum == true || map.Tile.LayerDef?.isSpace == true);

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

    public override Gizmo GetGizmo()
    {
        var gizmo = base.GetGizmo();
        if (InSpace && gizmo is Command command && Ext.SpaceIcon is Texture2D icon)
            command.icon = icon;
        return gizmo;
    }

    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);

        var map = pawn.Map;
        var tank = Tank;
        for (var i = 0; i < Ext.fuelPerJump; i++)
            tank?.UsedOnce();

        var destination = targets[0].Cell;
        var flyer = (PawnFlyer_BreachJump)PawnFlyer.MakeFlyer(SCWC_DefOf.SCWC_BreachJumpFlyer, pawn, destination, null, null, true);
        flyer.ability = this;
        flyer.DestinationCell = destination;
        GenSpawn.Spawn(flyer, destination, map);
    }

    // Called by the flyer once the wearer is back on the map.
    public void DoBreach(IntVec3 center, Map map, Pawn wearer)
    {
        var damage = Mathf.RoundToInt(Ext.breachDamage * wearer.GetStatValue(SCWC_DefOf.SCWC_BreachPower));
        GenExplosion.DoExplosion(center, map, Ext.breachRadius, SCWC_DefOf.SCWC_Breach, wearer, damage,
            ignoredThings: new List<Thing> { wearer });
    }
}
