using RimWorld;
using Verse;

namespace ShipcrackerWarcasket;

// Landing section: BreachJumpExtension's breachRadius, roofPunchRadius, breachDamage and
// breachArmorPenetration, the SCWC_Breach def's structure factors (through BreachTuning) and
// the shoulders' SCWC_BreachPower offset. The landing has no gizmo, so its header has no icon.
public partial class ShipcrackerWarcasketSettings
{
    // The landing: the extension's breachRadius and roofPunchRadius (0 turns either off),
    // breachDamage and breachArmorPenetration (as a percentage), the structure damage the
    // SCWC_Breach def's factors make of it, and the shoulders' SCWC_BreachPower offset as a
    // percentage.
    public readonly Tunable blastRadius = new("BlastRadius", 0f, 6f, 0.5f, OffOrRadius);
    public readonly Tunable roofRadius = new("RoofRadius", 0f, 6f, 0.5f, OffOrRadius);
    public readonly Tunable creatureDamage = new("CreatureDamage", 1f, 60f, 1f, Whole);
    public readonly Tunable armorPenetration = new("ArmorPenetration", 0f, 100f, 5f, Whole);
    public readonly Tunable wallDamage = new("WallDamage", 50f, 2000f, 50f, Whole);
    public readonly Tunable structureDamage = new("StructureDamage", 25f, 1000f, 25f, Whole);
    public readonly Tunable shouldersBonus = new("ShouldersBonus", 0f, 200f, 10f, Whole);

    // A vanilla wall's hit points in the given stuff, as its info card shows them, for the wall
    // damage tooltip's yardsticks.
    private static string WallHitPoints(ThingDef stuff) =>
        Whole(ThingDefOf.Wall.GetStatValueAbstract(StatDefOf.MaxHitPoints, stuff));

    private static string OffOrRadius(float v) => v <= 0f ? "SCWC_Off".Translate().ToString() : v.ToString("0.#");

    private void ExposeLandingSettings()
    {
        blastRadius.ExposeData();
        roofRadius.ExposeData();
        creatureDamage.ExposeData();
        armorPenetration.ExposeData();
        wallDamage.ExposeData();
        structureDamage.ExposeData();
        shouldersBonus.ExposeData();
    }

    private void LoadLandingDefaults(Defs defs)
    {
        wallDamage.DescArgs = [WallHitPoints(ThingDefOf.Steel), WallHitPoints(ThingDefOf.Plasteel)];

        blastRadius.DefaultValue = defs.ext.breachRadius;
        roofRadius.DefaultValue = defs.ext.roofPunchRadius;
        creatureDamage.DefaultValue = defs.ext.breachDamage;
        armorPenetration.DefaultValue = defs.ext.breachArmorPenetration * 100f;
        wallDamage.DefaultValue = BreachTuning.StructureDamage(defs.ext.breachDamage,
            defs.breach.buildingDamageFactor, defs.breach.buildingDamageFactorImpassable);
        structureDamage.DefaultValue = BreachTuning.StructureDamage(defs.ext.breachDamage,
            defs.breach.buildingDamageFactor, defs.breach.buildingDamageFactorPassable);
        shouldersBonus.DefaultValue = defs.power.value * 100f;
    }

    private void ApplyLandingToDefs(Defs defs)
    {
        defs.ext.breachRadius = blastRadius.Value;
        defs.ext.roofPunchRadius = roofRadius.Value;
        defs.ext.breachDamage = creatureDamage.IntValue;
        defs.ext.breachArmorPenetration = armorPenetration.Value / 100f;
        if (defs.breach.buildingDamageFactor <= 0f)
            defs.breach.buildingDamageFactor = 1f;
        defs.breach.buildingDamageFactorImpassable = BreachTuning.StructureFactor(wallDamage.Value,
            creatureDamage.IntValue, defs.breach.buildingDamageFactor);
        defs.breach.buildingDamageFactorPassable = BreachTuning.StructureFactor(structureDamage.Value,
            creatureDamage.IntValue, defs.breach.buildingDamageFactor);
        defs.power.value = shouldersBonus.Value / 100f;
    }

    private void ResetLandingSettings()
    {
        blastRadius.Reset();
        roofRadius.Reset();
        creatureDamage.Reset();
        armorPenetration.Reset();
        wallDamage.Reset();
        structureDamage.Reset();
        shouldersBonus.Reset();
    }

    private void DrawLandingSection(Listing_Standard listing)
    {
        SectionHeader(listing, "SCWC_SectionLanding", null);
        blastRadius.DoRow(listing);
        roofRadius.DoRow(listing);
        creatureDamage.DoRow(listing);
        armorPenetration.DoRow(listing);
        wallDamage.DoRow(listing);
        structureDamage.DoRow(listing);
        shouldersBonus.DoRow(listing);
        SummaryRow(listing, LandingSummary);
    }

    // What a landing deals to structures without and with the shoulders, whose bonus adds to
    // SCWC_BreachPower's base of 1, and how many cells the blast covers.
    private string LandingSummary()
    {
        if (blastRadius.Value <= 0f)
            return "SCWC_LandingSummaryOff".Translate();
        float power = 1f + shouldersBonus.Value / 100f;
        return "SCWC_LandingSummary".Translate(Whole(wallDamage.Value), Whole(structureDamage.Value),
            GenRadial.NumCellsInRadius(blastRadius.Value), Whole(wallDamage.Value * power),
            Whole(structureDamage.Value * power));
    }
}
