using HarmonyLib;
using Verse;

namespace ShipcrackerWarcasket.Patches;

// Puts each thruster glow node after its armor's worn-graphic node among the ApparelBody tag
// node's children (PawnRenderNode_ThrusterGlow.OrderAfterArmor explains why the order matters).
// AddChildren is how PawnRenderTree attaches dynamic nodes, so this runs only when a pawn's
// render tree is built or rebuilt, never per frame.
[HarmonyPatch(typeof(PawnRenderNode), nameof(PawnRenderNode.AddChildren))]
public static class PawnRenderNode_AddChildren_Patch
{
    public static void Postfix(PawnRenderNode __instance) => PawnRenderNode_ThrusterGlow.OrderAfterArmor(__instance);
}
