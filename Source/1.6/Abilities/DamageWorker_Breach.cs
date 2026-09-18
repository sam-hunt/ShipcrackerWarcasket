using RimWorld;
using Verse;

namespace ShipcrackerWarcasket;

// Worker for the SCWC_Breach DamageDef. Pawns take the plain injury path; buildings are scaled
// by the instigator's SCWC_BreachPower before the def's building factors apply, so the
// shoulders' bonus makes a landing hit structures harder without touching pawn damage or
// armor penetration. Scaling the explosion's damage amount instead would do both, since
// GenExplosion derives armor penetration from the amount it is handed (decompile-verified).
public class DamageWorker_Breach : DamageWorker_AddInjury
{
    public override DamageResult Apply(DamageInfo dinfo, Thing thing)
    {
        if (thing.def.category == ThingCategory.Building && dinfo.Instigator is Pawn breacher)
        {
            var power = breacher.GetStatValue(SCWC_DefOf.SCWC_BreachPower);
            if (power != 1f)
                dinfo.SetAmount(dinfo.Amount * power);
        }

        return base.Apply(dinfo, thing);
    }
}
