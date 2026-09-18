using UnityEngine;
using Verse;

namespace ShipcrackerWarcasket;

// Worker for PawnRenderNode_ThrusterGlow. Inherits the body-apparel worker so the node obeys
// the same posture, NoBody and Clothes gates as the armor, then:
//  - takes offset, rotation, scale and layer from the armor's own node (same parent, so the
//    ancestor chain PawnRenderTree.TryGetMatrix multiplies is otherwise identical), plus half a
//    layer so the glow sits just above the armor and below the next apparel item, which the
//    setup stacks a whole layer up. This also inherits the Shell rule that puts the body
//    apparel above the head when facing north (drawData layer 88), which the glow must follow.
//  - multiplies the node's alpha into the _Color the base worker writes each frame. The tree
//    rebuilds the property block per draw, so no material is ever created for an alpha step.
//  - never draws into portraits or the zoomed-out pawn cache: both bake with DrawMeshNow,
//    which ignores property blocks, so the overlay would be baked at full opacity. The live
//    view at cache zoom is handled by PawnRenderer_Patches, which keeps a glowing wearer out
//    of the cache. Draw requests are cached between frames until the parms change, so the node
//    stays in the request list with alpha 0 while idle rather than toggling CanDrawNow; one
//    fully transparent quad per wearer is cheaper than forcing a request rebuild on every
//    cast and landing.
public class PawnRenderNodeWorker_ThrusterGlow : PawnRenderNodeWorker_Apparel_Body
{
    private const float LayerAboveArmor = 0.5f;

    private static PawnRenderNode Armor(PawnRenderNode node) => (node as PawnRenderNode_ThrusterGlow)?.ArmorNode;

    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms) =>
        !parms.Portrait && !parms.Cache && base.CanDrawNow(node, parms);

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
