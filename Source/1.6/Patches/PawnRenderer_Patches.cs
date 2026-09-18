using HarmonyLib;
using Verse;

namespace ShipcrackerWarcasket.Patches;

// Keeps a wearer whose thruster glow is lit out of the zoomed-out pawn cache.
//
// Past CameraDriver.ZoomRootSize 18 PawnRenderer draws humanlike pawns from a baked atlas frame
// and re-bakes only when the pawn is marked dirty. The glow node changes every frame and is
// excluded from the bake (PawnRenderNodeWorker_ThrusterGlow), so at that zoom it would never
// show. ParallelGetPreRenderResults takes a disableCache flag for callers needing a live
// render; this prefix raises it while the glow is lit, one pawn at a time. It runs in the
// parallel pre-draw pass, so the check reads state only.
[HarmonyPatch(typeof(PawnRenderer), "ParallelGetPreRenderResults")]
public static class PawnRenderer_ParallelGetPreRenderResults_Patch
{
    public static void Prefix(Pawn ___pawn, ref bool disableCache)
    {
        if (!disableCache && Ability_BreachJump.ThrusterGlowLit(___pawn))
            disableCache = true;
    }
}
