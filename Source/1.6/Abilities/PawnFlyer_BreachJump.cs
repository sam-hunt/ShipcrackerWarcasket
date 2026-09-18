using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using VEF.Abilities;

namespace ShipcrackerWarcasket;

// Flight leg of the Breach Jump. Mirrors VFEP's PawnFlyer_PowerJump on a planet: flight time
// comes from the wearer's VFEP_FlightSpeed instead of the def's fixed flightSpeed, and a
// jump-flame effecter plays during flight. The landing detonation is the ability's
// breach rather than a Bomb.
//
// Space maps get a different flight (2026-09-17):
//  - Straight line at constant speed, no arc. Vanilla's RecomputePosition front-loads the trip
//    (15% of the distance in the first 10% of the time, PawnFlyerBase's progressCurve) and
//    lifts the pawn along an inverse parabola scaled by heightFactor; both read as a hop under
//    gravity. In zero g the thrusters push one way the whole trip, so progress is linear and
//    the pawn stays on the line. Done through VEF's CustomRecomputePosition hook, so the def's
//    curve and heightFactor still apply on a planet.
//  - Faster: the wearer's VFEP_FlightSpeed times the extension's spaceFlightSpeedFactor, still
//    capped at spaceFlightMaxSeconds so a map-length hop does not drag.
//  - SCWC_BreachBurnFlame instead of SCWC_BreachJumpFlame (our copies of VFEP's Shock Blast
//    Off and Aerial Power Jump effects). The two are identical except for the flame sprayers'
//    maxMoteCount: the jump exhaust cuts out early, which suits a short hop but leaves a
//    multi-second space run coasting silently; the burn keeps going to the landing. Both come
//    from the ability extension (flightEffecter / spaceFlightEffecter) so a compat root can
//    recolour the exhaust by patch; the DefOf entries are the fallback for a flyer whose
//    ability reference did not survive a reload. Their flecks are our copies raised to the
//    Pawn layer, so the trail draws over the flying wearer rather than under (see the fleck
//    defs' headers).
public class PawnFlyer_BreachJump : AbilityPawnFlyer
{
    // PawnFlyer keeps the takeoff-to-landing distance private; vanilla's own SpawnSetup uses
    // it the same way we do here.
    private static readonly AccessTools.FieldRef<PawnFlyer, float> FlightDistance =
        AccessTools.FieldRefAccess<PawnFlyer, float>("flightDistance");

    private Effecter flightEffecter;

    // Set from the map in SpawnSetup on both the fresh spawn and the reload path, so it is never
    // scribed; the map cannot change mid-flight.
    private bool inSpace;

    // Whether the landing punches through the roof, decided by the ability at launch (false for
    // a jump that starts and ends in the same indoor room; the takeoff punch already happened
    // under the same decision). Scribed: the rooms may have changed by the time a reloaded
    // flight lands.
    public bool punchRoof = true;

    // ability is assigned between MakeFlyer and GenSpawn.Spawn, so it is set by the time this runs.
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        inSpace = Ability_BreachJump.IsSpaceMap(map);
        if (respawningAfterLoad)
            return;

        var seconds = Mathf.Max(FlightDistance(this), 1f) / FlyingPawn.GetStatValue(SCWC_DefOf.VFEP_FlightSpeed);
        if (inSpace && ability is Ability_BreachJump jump)
        {
            seconds /= Mathf.Max(jump.Ext.spaceFlightSpeedFactor, 0.01f);
            seconds = Mathf.Min(seconds, jump.Ext.spaceFlightMaxSeconds);
        }
        seconds = Mathf.Max(seconds, def.pawnFlyer.flightDurationMin);

        ticksFlightTime = seconds.SecondsToTicks();
        ticksFlying = 0;
    }

    // Replaces vanilla's RecomputePosition on space maps (VEF's prefix skips it when this
    // returns true). Vanilla caches per tick behind a private field we cannot reach; the lerp
    // is cheap enough to redo per call, and Thing.Position's setter is a no-op when unchanged.
    // Height stays 0 (full-size shadow, no forward lift), but the draw position sits one
    // altitude increment up, which is where vanilla's arc peaks, so the flyer is drawn over
    // pawns it passes rather than z-fighting them at resting altitude.
    protected override bool CustomRecomputePosition()
    {
        if (!inSpace)
            return false;

        var t = Mathf.Clamp01((float)ticksFlying / ticksFlightTime);
        GroundPos = Vector3.Lerp(startVec, DestinationPos, t);
        EffectiveHeight = 0f;
        EffectivePos = GroundPos + Altitudes.AltIncVect;
        Position = GroundPos.ToIntVec3();
        return true;
    }

    protected override void Tick()
    {
        if (flightEffecter == null)
        {
            var ext = (ability as Ability_BreachJump)?.Ext;
            var effecterDef = inSpace
                ? ext?.spaceFlightEffecter ?? SCWC_DefOf.SCWC_BreachBurnFlame
                : ext?.flightEffecter ?? SCWC_DefOf.SCWC_BreachJumpFlame;
            flightEffecter = effecterDef.Spawn();
            flightEffecter.Trigger(this, TargetInfo.Invalid);
        }
        else
        {
            flightEffecter.EffectTick(this, TargetInfo.Invalid);
        }

        base.Tick();
    }

    protected override void RespawnPawn()
    {
        var wearer = FlyingPawn;
        var cell = Position;
        var map = Map;

        base.RespawnPawn();

        if (wearer != null && ability is Ability_BreachJump jump)
            jump.DoBreach(cell, map, wearer, punchRoof);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref punchRoof, "punchRoof", true);
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        flightEffecter?.Cleanup();
        flightEffecter = null;
        base.Destroy(mode);
    }
}
