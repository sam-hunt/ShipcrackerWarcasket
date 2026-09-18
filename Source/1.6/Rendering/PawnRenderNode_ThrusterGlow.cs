using System.Linq;
using VEF.Abilities;
using Verse;

namespace ShipcrackerWarcasket;

// The armor's thruster-outlet glow: a second render node declared in the armor def's
// apparel.renderNodeProperties (see the def header), drawn over the worn armor texture with an
// alpha that follows the Breach Jump's cast progress and holds at full through the flight.
//
// A node, not a Harmony draw hook: DynamicPawnRenderNodeSetup_Apparel adds every
// renderNodeProperties entry AND the default worn-graphic node for the same apparel, with
// node.apparel set on both, so the game builds, parents and matrices this node exactly as it
// does the armor's. The graphic comes from the base PawnRenderNode (props.texPath as a
// Graphic_Multi through props.shaderTypeDef and props.color), so the overlay's _north/_east/
// _south files are picked and west-flipped by the same code as the armor's, and the mesh is the
// same humanlike body set. What differs is in the worker: transform and layer are delegated
// to the armor's own node so the two can never drift apart, and the alpha is written into the
// material property block every frame.
//
// This class only holds the per-node caches the worker needs; workers are shared singletons.
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
}
