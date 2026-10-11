using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using VEF.Abilities;

namespace ShipcrackerWarcasket;

// Flight leg of the Breach Jump, after VFEP's PawnFlyer_PowerJump: flight time comes from the
// wearer's VFEP_FlightSpeed rather than the def's flightSpeed, an exhaust effecter plays for
// the flight, and the landing is the ability's breach rather than a Bomb.
//
// The breach burn (Ability_BreachJump.UsesBurn) flies differently:
//  - A straight line at constant speed through VEF's CustomRecomputePosition hook. Vanilla's
//    RecomputePosition front-loads progress (PawnFlyerBase's progressCurve) and lifts the pawn
//    along an inverse parabola scaled by heightFactor, which reads as a hop under gravity; the
//    def's curve and heightFactor still apply on a planet.
//  - Faster: VFEP_FlightSpeed times the extension's spaceFlightSpeedFactor, capped at
//    spaceFlightMaxSeconds so a map-length hop does not drag.
//  - The extension's spaceFlightEffecter instead of flightEffecter. The two differ only in the
//    sprayers' maxMoteCount: the jump exhaust cuts out early to suit a short hop, the burn runs
//    to the landing. Both live on the extension so a compat root can recolor them by patch;
//    the DefOf entries are the fallback for a flyer whose ability reference did not survive a
//    reload.
public class PawnFlyer_BreachJump : AbilityPawnFlyer
{
    // PawnFlyer keeps the takeoff-to-landing distance private; vanilla's own SpawnSetup uses
    // it the same way we do here.
    private static readonly AccessTools.FieldRef<PawnFlyer, float> FlightDistance =
        AccessTools.FieldRefAccess<PawnFlyer, float>("flightDistance");

    private Effecter flightEffecter;

    // Whether this flight is the breach burn, set in SpawnSetup on both the fresh spawn and the
    // reload path, so it is never scribed: the map cannot change mid-flight. A save reloaded
    // after the vacuum burn setting flipped finishes its flight on the other path, which is
    // cosmetic only.
    private bool burn;

    // Whether the landing punches through the roof, decided by the ability at launch (false for
    // a jump that starts and ends in the same indoor room; the takeoff punch already happened
    // under the same decision). Scribed: the rooms may have changed by the time a reloaded
    // flight lands.
    public bool punchRoof = true;

    // ability is assigned between MakeFlyer and GenSpawn.Spawn, so it is set by the time this runs.
    // The wearer is lit for the whole flight (Ability_BreachJump.ThrusterGlowLit), so the takeoff
    // and the landing in RespawnPawn are the glow's two flight transitions. A flyer respawned by
    // a gravship landing keeps its timing, as vanilla's does: Notify_TransportedOnGravship has
    // already shifted the flight onto the new map's cells, so it carries on mid-air.
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        burn = Ability_BreachJump.UsesBurn(map);
        Ability_BreachJump.NotifyThrusterGlowChanged(FlyingPawn);
        if (respawningAfterLoad || BeingTransportedOnGravship)
            return;

        var seconds = BreachJumpExtension.FlightSeconds(FlightDistance(this),
            FlyingPawn.GetStatValue(SCWC_DefOf.VFEP_FlightSpeed),
            burn ? (ability as Ability_BreachJump)?.Ext : null,
            def.pawnFlyer.flightDurationMin);

        ticksFlightTime = seconds.SecondsToTicks();
        ticksFlying = 0;
    }

    // Replaces vanilla's RecomputePosition for the burn (VEF's prefix skips it when this
    // returns true). Vanilla caches per tick behind a private field we cannot reach; the lerp
    // is cheap enough to redo per call, and Thing.Position's setter is a no-op when unchanged.
    // Height stays 0 (full-size shadow, no forward lift), but the draw position sits one
    // altitude increment up, which is where vanilla's arc peaks, so the flyer is drawn over
    // pawns it passes rather than z-fighting them at resting altitude.
    protected override bool CustomRecomputePosition()
    {
        if (!burn)
            return false;

        var t = Mathf.Clamp01((float)ticksFlying / ticksFlightTime);
        GroundPos = Vector3.Lerp(startVec, DestinationPos, t);
        EffectiveHeight = 0f;
        EffectivePos = GroundPos + Altitudes.AltIncVect;
        Position = GroundPos.ToIntVec3();
        return true;
    }

    // The exhaust is ticked after base.TickInterval, which advances ticksFlying and so DrawPos,
    // where the sprayers spawn their flecks (PawnFlyer moves in TickInterval, every tick, and
    // Thing.DoTick runs Tick before it). Ticked before, each fleck would spawn a whole tick of
    // travel behind where the wearer is drawn this frame (0.6 cells at the space burn's speed)
    // and the trail would never overlap the body, which the north-facing raised flecks rely on.
    // base.TickInterval destroys the flyer on landing, and Destroy has already cleaned the
    // effecter up; recreating it here would replay the launch.
    protected override void TickInterval(int delta)
    {
        base.TickInterval(delta);
        if (Destroyed)
            return;

        if (flightEffecter == null)
        {
            var ext = (ability as Ability_BreachJump)?.Ext;
            var effecterDef = burn
                ? ext?.spaceFlightEffecter ?? SCWC_DefOf.SCWC_BreachBurnFlame
                : ext?.flightEffecter ?? SCWC_DefOf.SCWC_BreachJumpFlame;
            flightEffecter = effecterDef.Spawn();
            flightEffecter.Trigger(this, TargetInfo.Invalid);
        }
        else
        {
            flightEffecter.EffectTick(this, TargetInfo.Invalid);
        }
    }

    protected override void RespawnPawn()
    {
        var wearer = FlyingPawn;
        var cell = Position;
        var map = Map;

        base.RespawnPawn();

        Ability_BreachJump.NotifyThrusterGlowChanged(wearer);
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
