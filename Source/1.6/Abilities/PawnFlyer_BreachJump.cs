using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using VEF.Abilities;

namespace ShipcrackerWarcasket;

// Flight leg of the Breach Jump. Mirrors VFEP's PawnFlyer_PowerJump: flight time comes from
// the wearer's VFEP_FlightSpeed instead of the def's fixed flightSpeed, and the Aerial
// jump-flame effecter plays during flight. Differences: space hops are capped in duration,
// and the landing detonation is the ability's breach rather than a Bomb.
public class PawnFlyer_BreachJump : AbilityPawnFlyer
{
    // PawnFlyer keeps the takeoff-to-landing distance private; vanilla's own SpawnSetup uses
    // it the same way we do here.
    private static readonly AccessTools.FieldRef<PawnFlyer, float> FlightDistance =
        AccessTools.FieldRefAccess<PawnFlyer, float>("flightDistance");

    private Effecter flightEffecter;

    // ability is assigned between MakeFlyer and GenSpawn.Spawn, so it is set by the time this runs.
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (respawningAfterLoad)
            return;

        var seconds = Mathf.Max(FlightDistance(this), 1f) / FlyingPawn.GetStatValue(SCWC_DefOf.VFEP_FlightSpeed);
        if (ability is Ability_BreachJump jump && Ability_BreachJump.IsSpaceMap(map))
            seconds = Mathf.Min(seconds, jump.Ext.spaceFlightMaxSeconds);
        seconds = Mathf.Max(seconds, def.pawnFlyer.flightDurationMin);

        ticksFlightTime = seconds.SecondsToTicks();
        ticksFlying = 0;
    }

    protected override void Tick()
    {
        if (flightEffecter == null)
        {
            flightEffecter = SCWC_DefOf.VFEP_PowerJumpPawnEffect.Spawn();
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
            jump.DoBreach(cell, map, wearer);
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        flightEffecter?.Cleanup();
        flightEffecter = null;
        base.Destroy(mode);
    }
}
