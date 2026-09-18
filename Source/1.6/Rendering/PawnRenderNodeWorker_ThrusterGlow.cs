using UnityEngine;
using Verse;

namespace ShipcrackerWarcasket;

// Worker for PawnRenderNode_ThrusterGlow. Inherits the body-apparel worker for its posture,
// NoBody and Clothes gates, then:
//  - takes offset, rotation, scale and layer from the armor's own node (same parent, so the
//    ancestor chain is otherwise identical), plus half a layer so the glow sits above the armor
//    and below the next apparel item, which the setup stacks a whole layer up. That includes
//    the Shell rule drawing body apparel above the head when facing north.
//  - multiplies the node's alpha into the _Color the base worker writes per draw; the tree
//    rebuilds the property block each frame, so no material is created for an alpha step.
//  - never draws into portraits, and draws into the zoomed-out pawn cache only while lit: both
//    bake with DrawMeshNow, which ignores property blocks, so the cache gets the overlay at full
//    opacity or not at all, and Ability_BreachJump.NotifyThrusterGlowChanged rebakes it at each
//    transition. In the live tree the node stays in the cached request list at alpha 0 while
//    idle rather than toggling CanDrawNow, which would rebuild the requests on every cast.
public class PawnRenderNodeWorker_ThrusterGlow : PawnRenderNodeWorker_Apparel_Body
{
    private const float LayerAboveArmor = 0.5f;

    private static PawnRenderNode Armor(PawnRenderNode node) => (node as PawnRenderNode_ThrusterGlow)?.ArmorNode;

    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms) =>
        !parms.Portrait
        && (!parms.Cache || (node as PawnRenderNode_ThrusterGlow)?.Lit == true)
        && base.CanDrawNow(node, parms);

    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        var armor = Armor(node);
        return armor != null ? armor.Worker.OffsetFor(armor, parms, out pivot) : base.OffsetFor(node, parms, out pivot);
    }

    public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
    {
        var armor = Armor(node);
        return armor != null ? armor.Worker.RotationFor(armor, parms) : base.RotationFor(node, parms);
    }

    public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
    {
        var armor = Armor(node);
        return armor != null ? armor.Worker.ScaleFor(armor, parms) : base.ScaleFor(node, parms);
    }

    public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
    {
        var armor = Armor(node);
        return (armor != null ? armor.Worker.LayerFor(armor, parms) : base.LayerFor(node, parms)) + LayerAboveArmor;
    }

    public override MaterialPropertyBlock GetMaterialPropertyBlock(PawnRenderNode node, Material material, PawnDrawParms parms)
    {
        var block = base.GetMaterialPropertyBlock(node, material, parms);
        if (block == null)
            return null;

        var color = parms.tint * material.color;
        color.a *= node is PawnRenderNode_ThrusterGlow glow ? glow.Alpha : 0f;
        block.SetColor(ShaderPropertyIDs.Color, color);
        return block;
    }
}
