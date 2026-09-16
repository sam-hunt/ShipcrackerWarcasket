using RimWorld;
using Verse;

namespace ShipcrackerWarcasket;

[DefOf]
public static class SCWC_DefOf
{
    public static StatDef SCWC_BreachJumpRange;
    public static StatDef SCWC_BreachPower;
    public static DamageDef SCWC_Breach;
    public static ThingDef SCWC_BreachJumpFlyer;
    public static EffecterDef SCWC_BreachJumpBlastOff;

    // VFE Pirates defs reused by name only (VFEP is a hard dependency, so they always exist):
    // the Aerial set's jump flight-speed stat, its jump-flame effecter (planet flights) and the
    // Shock set's Blast Off exhaust (space flights; see PawnFlyer_BreachJump for why). Reusing
    // the stat means Aerial pieces' x1.5 flight-speed factors also speed up a Breach Jump;
    // harmless.
    public static StatDef VFEP_FlightSpeed;
    public static EffecterDef VFEP_PowerJumpPawnEffect;
    public static EffecterDef VFEP_BlastOffEffect;

    static SCWC_DefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SCWC_DefOf));
}
