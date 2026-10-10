using Verse;
using VEF.Abilities;

namespace ShipcrackerWarcasket;

// The Breach Jump's gizmo: VEF's ability Command plus the top-right "jumps / max" count that
// vanilla draws on the jump pack's verb command and on its own charged abilities. VEF's base
// has no charge slot, so this is named as the def's gizmoClass. The count is read live from the
// ability on every draw, so the cached Command in Ability_BreachJump.GetGizmo needs no refresh.
public class Command_BreachJump : Command_Ability
{
    public Command_BreachJump(Pawn pawn, Ability ability) : base(pawn, ability)
    {
    }

    public override string TopRightLabel => (ability as Ability_BreachJump)?.JumpsLabel;
}
