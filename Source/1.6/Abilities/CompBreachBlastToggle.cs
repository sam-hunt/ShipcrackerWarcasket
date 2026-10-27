using System.Collections.Generic;
using Verse;
using VEF.Abilities;

namespace ShipcrackerWarcasket;

// Puts the landing blast toggle (Ability_BreachJump.GetBlastToggleGizmo) on the wearer's gizmo
// bar beside the jump button. VEF's CompAbilitiesApparel yields exactly one gizmo per ability,
// so a second one needs another comp on the armor: this one is declared by compClass directly
// after the abilities comp in the armor def, since worn gizmos come out in comp order and the
// grid's sort is stable (the two share an Order). It also relies on that comp having already
// bound the ability to the current wearer, which it does while yielding the jump button. Holds
// no state of its own; the toggle's value is saved with the ability.
public class CompBreachBlastToggle : ThingComp
{
    private CompAbilitiesApparel abilities;

    public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
    {
        abilities ??= parent.TryGetComp<CompAbilitiesApparel>();
        var given = abilities?.GivenAbilities;
        if (given == null)
            yield break;

        for (var i = 0; i < given.Count; i++)
        {
            if (given[i] is Ability_BreachJump jump && jump.ShowBlastToggle)
                yield return jump.GetBlastToggleGizmo();
        }
    }
}
