using System.Linq;
using VEF.Abilities;
using Verse;

namespace ShipcrackerWarcasket;

// The armor's thruster-outlet glow: a second render node from the armor def's
// apparel.renderNodeProperties, drawn over the worn texture with an alpha that follows the
// Breach Jump's cast and holds through the flight.
//
// A node rather than a draw hook because DynamicPawnRenderNodeSetup_Apparel adds every
// renderNodeProperties entry beside the default worn-graphic node with node.apparel set on
// both, so the game parents and matrices this node exactly as the armor's, and the base
// PawnRenderNode loads props.texPath as a Graphic_Multi, so the _north/_east/_south files are
// picked and west-flipped by the same code. The worker delegates transform and layer to the
// armor's node and writes the alpha into the property block.
//
// Holds only the per-node caches the worker needs; workers are shared singletons.
public class PawnRenderNode_ThrusterGlow : PawnRenderNode
{
    private PawnRenderNode armorNode;
    private Ability_BreachJump jump;
    private bool jumpResolved;

    public PawnRenderNode_ThrusterGlow(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
        : base(pawn, props, tree)
    {
    }

    // The default worn-graphic node of the same apparel: a sibling under the same parent (both
    // resolve to the ApparelBody tag node), found lazily because siblings are attached after
    // this node is constructed. Null until the tree has finished building, or if the armor
    // has no worn graphic, in which case the worker falls back to this node's own props.
    public PawnRenderNode ArmorNode
    {
        get
        {
            if (armorNode == null && parent?.children != null)
                armorNode = parent.children.FirstOrDefault(n => n is PawnRenderNode_Apparel && n != this && n.apparel == apparel);
            return armorNode;
        }
    }

    // The Breach Jump granted by this apparel's CompAbilitiesApparel; it owns the cast state.
    public Ability_BreachJump Jump
    {
        get
        {
            if (!jumpResolved)
            {
                jump = apparel?.GetComp<CompAbilitiesApparel>()?.GivenAbilities.OfType<Ability_BreachJump>().FirstOrDefault();
                jumpResolved = jump != null;
            }
            return jump;
        }
    }

    // 0 when idle, the cast curve while warming up, 1 for the whole flight.
    public float Alpha => tree?.pawn is Pawn wearer ? Ability_BreachJump.ThrusterGlowAlpha(wearer, Jump) : 0f;

    // Casting or flying, the on/off state the pawn cache bakes.
    public bool Lit => tree?.pawn is Pawn wearer && Ability_BreachJump.ThrusterGlowLit(wearer, Jump);

    // Moves every glow node among parent's children behind the worn-graphic node of the same
    // apparel. DynamicPawnRenderNodeSetup_Apparel yields a def's renderNodeProperties nodes
    // before the apparel's own node, and the pawn cache bakes the tree with DrawMeshNow in
    // request order: a transparent overlay drawn before the opaque sprite it sits on is simply
    // painted over, because it writes no depth for the sprite to fail against. The live path is
    // indifferent, Unity queues transparent materials after cutout ones. Called from
    // PawnRenderNode_AddChildren_Patch right after the tree attaches the children.
    public static void OrderAfterArmor(PawnRenderNode parent)
    {
        var children = parent.children;
        if (children == null)
            return;

        for (var g = 0; g < children.Length; g++)
        {
            if (children[g] is not PawnRenderNode_ThrusterGlow glow)
                continue;
            for (var a = g + 1; a < children.Length; a++)
            {
                if (children[a] is PawnRenderNode_Apparel armor && armor.apparel == glow.apparel)
                {
                    children[g] = armor;
                    children[a] = glow;
                    break;
                }
            }
        }
    }
}
