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
    // Fallback flight exhausts for a flyer whose ability reference cannot be read; the ability
    // extension's flightEffecter / spaceFlightEffecter normally name these same two.
    public static EffecterDef SCWC_BreachJumpFlame;
    public static EffecterDef SCWC_BreachBurnFlame;

    // VFE Pirates def reused by name only (VFEP is a hard dependency, so it always exists): the
    // Aerial set's jump flight-speed stat. Reusing it means Aerial pieces' x1.5 flight-speed
    // factors also speed up a Breach Jump; harmless.
    public static StatDef VFEP_FlightSpeed;

    static SCWC_DefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SCWC_DefOf));
}
