using HarmonyLib;
using Verse;

namespace ShipcrackerWarcasket.Patches;

// Keeps a wearer whose thruster glow is lit out of the zoomed-out pawn cache.
//
// Past CameraDriver.ZoomRootSize 18 (the Middle zoom band and beyond) PawnRenderer draws
// humanlike pawns from a baked atlas frame instead of running the render tree, and only
// re-bakes when the pawn's appearance is marked dirty. The glow node changes every frame and
// is excluded from the bake (see PawnRenderNodeWorker_ThrusterGlow), so at that zoom a cast or
// flight would show no glow at all. ParallelGetPreRenderResults already takes a disableCache
// flag for callers that need a live render; this prefix raises it for the frames the glow is
// visible, which is the one-second cast plus the flight, for one pawn at a time. The method
// runs from the parallel pre-draw pass, so the check reads state only.
[HarmonyPatch(typeof(PawnRenderer), "ParallelGetPreRenderResults")]
public static class PawnRenderer_ParallelGetPreRenderResults_Patch
{
    public static void Prefix(Pawn ___pawn, ref bool disableCache)
    {
        if (!disableCache && Ability_BreachJump.ThrusterGlowLit(___pawn))
            disableCache = true;
    }
}
