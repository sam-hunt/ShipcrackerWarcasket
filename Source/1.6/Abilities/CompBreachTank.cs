using RimWorld;
using Verse;

namespace ShipcrackerWarcasket;

// The armor's fuel tank: vanilla's CompApparelReloadable plus a trim of a tank holding more
// than its maximum, which vanilla never does. The maximum can drop under a saved count: the
// tank capacity setting lowers it in place, and the VGE root patches it. Vanilla's reload
// accounting breaks on the overfill (NeedsReload is "not equal", MaxAmmoNeeded goes negative,
// and the reload job ends in SplitOff throwing), so the excess is dropped before any reload
// can see it: on load, on equip (a stored armor's saved count) and on a settings write
// (ShipcrackerWarcasketSettings.TrimWornTanks). remainingCharges is protected on
// CompApparelVerbOwner_Charged, which is why this is a subclass rather than a sweep from
// outside. Declared by compClass on the armor's CompProperties_ApparelReloadable entry; the
// DefInjected handle of that entry's text fields is this class's name.
public class CompBreachTank : CompApparelReloadable
{
    public void Trim()
    {
        if (remainingCharges > MaxCharges)
            remainingCharges = MaxCharges;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
            Trim();
    }

    public override void Notify_Equipped(Pawn pawn)
    {
        base.Notify_Equipped(pawn);
        Trim();
    }
}
