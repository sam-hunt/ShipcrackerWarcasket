using RimWorld;
using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace ShipcrackerWarcasket;

// Jump section: BreachJumpExtension.fuelPerJump, the armor's tank (CompProperties_ApparelReloadable
// maxCharges), its SCWC_BreachJumpRange offset and the AbilityDef's cooldownTime. The header
// carries the jump gizmo's icon; the tank's fuel and its conversion are read here for the fuel
// rows' labels and the summary.
public partial class ShipcrackerWarcasketSettings
{
    // The jump: BreachJumpExtension.fuelPerJump in chemfuel, the tank's maxCharges in its own
    // fuel, the armor's SCWC_BreachJumpRange offset and the AbilityDef's cooldownTime in hours.
    // The range tops out where VEF stops drawing the range ring with the aerial pieces' offsets
    // on top (GenRadial.MaxRadialPatternRadius).
    public readonly Tunable fuelPerJump = new("FuelPerJump", 0f, 50f, 2f, Whole);
    public readonly Tunable tankCapacity = new("TankCapacity", 10f, 300f, 10f, Whole);
    public readonly Tunable jumpRange = new("JumpRange", 5f, 40f, 1f, Whole);
    public readonly Tunable cooldown = new("Cooldown", 0f, 24f, 0.5f, v => v.ToString("0.#"));

    // Read with the defaults: the tank's fuel and how a jump's chemfuel converts into its
    // charges, for the summary and the fuel rows' labels.
    private string ammoLabel;
    private float chemfuelPerAmmo = 1f;
    private int ammoPerCharge = 1;

    // The jump def, for the gizmo icon beside the header. Re-read with the defaults on every
    // load, which replaces it. Its icon is read at draw time, since VEF loads the def's icon on
    // the long-event queue rather than with the def.
    private AbilityDef jumpDef;

    private void ExposeJumpSettings()
    {
        fuelPerJump.ExposeData();
        tankCapacity.ExposeData();
        jumpRange.ExposeData();
        cooldown.ExposeData();
    }

    private void LoadJumpDefaults(Defs defs)
    {
        ammoLabel = defs.tank.ammoDef?.label ?? ThingDefOf.Chemfuel.label;
        chemfuelPerAmmo = defs.ext.chemfuelPerAmmo;
        ammoPerCharge = Mathf.Max(1, defs.tank.ammoCountPerCharge);
        fuelPerJump.LabelArg = ammoLabel;
        fuelPerJump.DisplayScale = chemfuelPerAmmo;
        tankCapacity.LabelArg = ammoLabel;
        jumpDef = defs.jump;

        fuelPerJump.DefaultValue = defs.ext.fuelPerJump;
        tankCapacity.DefaultValue = defs.tank.maxCharges * ammoPerCharge;
        jumpRange.DefaultValue = defs.range.value;
        cooldown.DefaultValue = (float)defs.jump.cooldownTime / GenDate.TicksPerHour;
    }

    private void ApplyJumpToDefs(Defs defs)
    {
        defs.ext.fuelPerJump = fuelPerJump.IntValue;
        defs.tank.maxCharges = Mathf.Max(1, tankCapacity.IntValue / ammoPerCharge);
        defs.range.value = jumpRange.Value;
        defs.jump.cooldownTime = Mathf.RoundToInt(cooldown.Value * GenDate.TicksPerHour);
    }

    private void ResetJumpSettings()
    {
        fuelPerJump.Reset();
        tankCapacity.Reset();
        jumpRange.Reset();
        cooldown.Reset();
    }

    private void DrawJumpSection(Listing_Standard listing)
    {
        SectionHeader(listing, "SCWC_SectionJump", jumpDef.icon, first: true);
        fuelPerJump.DoRow(listing);
        tankCapacity.DoRow(listing);
        jumpRange.DoRow(listing);
        cooldown.DoRow(listing);
        SummaryRow(listing, JumpSummary);
    }

    // What the jump's sliders amount to, in the tank's own fuel: its size, the jumps it holds
    // and what each burns, through the same conversion the ability uses.
    private string JumpSummary()
    {
        int charges = BreachJumpExtension.ChargesPerJump(fuelPerJump.IntValue, chemfuelPerAmmo, ammoPerCharge);
        if (charges <= 0)
            return "SCWC_JumpSummaryFree".Translate();
        int maxCharges = Mathf.Max(1, tankCapacity.IntValue / ammoPerCharge);
        return "SCWC_JumpSummary".Translate(maxCharges * ammoPerCharge, ammoLabel, maxCharges / charges, charges * ammoPerCharge);
    }

    // A lowered tank size leaves worn tanks above it, which vanilla's reload accounting cannot
    // take (CompBreachTank); a stored armor is trimmed when equipped, and a tank saved above
    // the size on load. Every live pawn, since a caravan or transporter pawn spawns with no
    // hook of its own.
    private static void TrimWornTanks()
    {
        if (Current.ProgramState != ProgramState.Playing)
            return;
        foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive)
        {
            if (pawn.apparel == null)
                continue;
            foreach (Apparel apparel in pawn.apparel.WornApparel)
                apparel.TryGetComp<CompBreachTank>()?.Trim();
        }
    }
}
