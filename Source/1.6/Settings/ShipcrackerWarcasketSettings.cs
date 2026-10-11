using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace ShipcrackerWarcasket;

// The set's mod settings. Each Tunable overrides a def field, written onto the def by
// ApplyToDefs (at startup, then whenever the window closes), so every reader of those fields,
// vanilla's and VEF's included, sees the setting with no code of its own. The defaults, and the
// rationale for them, are the defs'. Nothing here is saved into a game: a save loads the same
// with any settings, and with none.
//
// Units are chosen so a saved setting means the same thing with or without the Vanilla
// Gravship Expanded root, which switches the tank to astrofuel: the jump's cost is stored in
// chemfuel (BreachJumpExtension.fuelPerJump, converted through chemfuelPerAmmo), the tank in
// units of whatever it takes. The page itself speaks one fuel throughout, the tank's: the cost
// row shows through Tunable.DisplayScale, the summary counts in it, and the section icons are
// the gizmos' own, which the VGE art root already recolors by path. The damage rows read in
// the numbers a landing deals, converted to the SCWC_Breach def's factors by BreachTuning. The
// jump and landing sections each end on a line of what their sliders amount to.
//
// What a write leaves on live ability instances (a running cooldown, a tooltip cache, fuel
// above a lowered capacity) is caught up by each instance on its next gizmo request, keyed
// on Generation (Ability_BreachJump.ReconcileSettings).
public class ShipcrackerWarcasketSettings : ModSettings
{
    private const string JumpDefName = "SCWC_BreachJump";
    private const string ArmorDefName = "SCWC_Warcasket_Shipcracker";
    private const string ShouldersDefName = "SCWC_WarcasketShoulders_Shipcracker";

    // The whitespace above every section header but the first, and below the last row.
    private const float SectionGap = 28f;

    // Bumped by every ApplyToDefs, including the one at startup.
    public static int Generation { get; private set; }

    // The jump: BreachJumpExtension.fuelPerJump in chemfuel, the tank's maxCharges in its own
    // fuel, the armor's SCWC_BreachJumpRange offset and the AbilityDef's cooldownTime in hours.
    // The range tops out where VEF stops drawing the range ring with the aerial pieces' offsets
    // on top (GenRadial.MaxRadialPatternRadius).
    public readonly Tunable fuelPerJump = new("FuelPerJump", 0f, 50f, 2f, Whole);
    public readonly Tunable tankCapacity = new("TankCapacity", 10f, 300f, 10f, Whole);
    public readonly Tunable jumpRange = new("JumpRange", 5f, 40f, 1f, Whole);
    public readonly Tunable cooldown = new("Cooldown", 0f, 24f, 0.5f, v => v.ToString("0.#"));

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

    // Whether a jump on a space map is the breach burn (unlimited range, line of sight) or
    // follows the planet rules (Ability_BreachJump.UsesBurn). Read live, so it needs no def.
    public bool vacuumBurn = true;

    private bool defaultsLoaded;
    private Vector2 scrollPosition;
    private float contentHeight;

    // Read with the defaults: the tank's fuel and how a jump's chemfuel converts into its
    // charges, for the summary and the fuel rows' labels.
    private string ammoLabel;
    private float chemfuelPerAmmo = 1f;
    private int ammoPerCharge = 1;

    // The jump def and its extension, for the gizmo icons beside the jump and burn headers
    // (the landing has no gizmo of its own). Read at draw time, since VEF loads the def's icon
    // on the long-event queue rather than with the def.
    private AbilityDef jumpDef;
    private BreachJumpExtension jumpExt;

    private IEnumerable<Tunable> All =>
        [fuelPerJump, tankCapacity, jumpRange, cooldown, blastRadius, roofRadius, creatureDamage, armorPenetration,
            wallDamage, structureDamage, shouldersBonus];

    private static string Whole(float v) => Mathf.RoundToInt(v).ToString();

    // A vanilla wall's hit points in the given stuff, as its info card shows them, for the wall
    // damage tooltip's yardsticks.
    private static string WallHitPoints(ThingDef stuff) =>
        Whole(ThingDefOf.Wall.GetStatValueAbstract(StatDefOf.MaxHitPoints, stuff));

    private static string OffOrRadius(float v) => v <= 0f ? "SCWC_Off".Translate().ToString() : v.ToString("0.#");

    public override void ExposeData()
    {
        base.ExposeData();
        foreach (Tunable tunable in All)
            tunable.ExposeData();
        Scribe_Values.Look(ref vacuumBurn, "VacuumBurn", true);
    }

    // Reads the def-backed defaults before the first ApplyToDefs overwrites them; defs must be
    // loaded, so ModInit's static constructor calls this, never the Mod constructor.
    public void LoadDefaultsFromDefs()
    {
        if (!TryGetDefs(out Defs defs))
            return;
        ammoLabel = defs.tank.ammoDef?.label ?? ThingDefOf.Chemfuel.label;
        chemfuelPerAmmo = defs.ext.chemfuelPerAmmo;
        ammoPerCharge = Mathf.Max(1, defs.tank.ammoCountPerCharge);
        fuelPerJump.LabelArg = ammoLabel;
        fuelPerJump.DisplayScale = chemfuelPerAmmo;
        tankCapacity.LabelArg = ammoLabel;
        wallDamage.DescArgs = [WallHitPoints(ThingDefOf.Steel), WallHitPoints(ThingDefOf.Plasteel)];
        jumpDef = defs.jump;
        jumpExt = defs.ext;

        fuelPerJump.DefaultValue = defs.ext.fuelPerJump;
        tankCapacity.DefaultValue = defs.tank.maxCharges * ammoPerCharge;
        jumpRange.DefaultValue = defs.range.value;
        cooldown.DefaultValue = (float)defs.jump.cooldownTime / GenDate.TicksPerHour;
        blastRadius.DefaultValue = defs.ext.breachRadius;
        roofRadius.DefaultValue = defs.ext.roofPunchRadius;
        creatureDamage.DefaultValue = defs.ext.breachDamage;
        armorPenetration.DefaultValue = defs.ext.breachArmorPenetration * 100f;
        wallDamage.DefaultValue = BreachTuning.StructureDamage(defs.ext.breachDamage,
            defs.breach.buildingDamageFactor, defs.breach.buildingDamageFactorImpassable);
        structureDamage.DefaultValue = BreachTuning.StructureDamage(defs.ext.breachDamage,
            defs.breach.buildingDamageFactor, defs.breach.buildingDamageFactorPassable);
        shouldersBonus.DefaultValue = defs.power.value * 100f;
        defaultsLoaded = true;
    }

    public void ApplyToDefs()
    {
        if (!defaultsLoaded || !TryGetDefs(out Defs defs))
            return;
        defs.ext.fuelPerJump = fuelPerJump.IntValue;
        defs.tank.maxCharges = Mathf.Max(1, tankCapacity.IntValue / ammoPerCharge);
        defs.range.value = jumpRange.Value;
        defs.jump.cooldownTime = Mathf.RoundToInt(cooldown.Value * GenDate.TicksPerHour);
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
        Generation++;
        TrimWornTanks();
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

    // One row per tunable under a header per section: the jump, the burn's toggle, then the
    // landing, so the two sections headed by a gizmo icon sit together; a reset button below.
    // Sections are set apart by whitespace and a medium-font header rather than rules, which
    // read as more sliders, and the rows sit indented under their header; the last section
    // closes on the same gap so the list does not end flush on its last row. The sections
    // outgrow the window's height, so they scroll, sized from the previous frame's listing
    // height; the scrollbar only takes its width once it is needed. maxOneColumn keeps the
    // listing growing downward: past its rect's height it would start a new column off to the
    // right, outside the window, and never report the height the scroll view needs.
    // While the defs were not found there is nothing to tune, so the page says so instead of
    // drawing rows around the -1 the defaults never got.
    public void DoWindowContents(Rect inRect)
    {
        if (!defaultsLoaded)
        {
            Widgets.Label(inRect, "SCWC_SettingsUnavailable".Translate());
            return;
        }

        const float buttonHeight = 30f;
        const float buttonWidth = 200f;
        const float scrollbarWidth = 16f;
        var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - buttonHeight - Listing.DefaultGap);
        float viewWidth = contentHeight > outRect.height ? outRect.width - scrollbarWidth : outRect.width;
        var viewRect = new Rect(0f, 0f, viewWidth, Mathf.Max(contentHeight, outRect.height));
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        var listing = new Listing_Standard { maxOneColumn = true };
        listing.Begin(viewRect);
        SectionHeader(listing, "SCWC_SectionJump", jumpDef.icon, first: true);
        fuelPerJump.DoRow(listing);
        tankCapacity.DoRow(listing);
        jumpRange.DoRow(listing);
        cooldown.DoRow(listing);
        SummaryRow(listing, JumpSummary);
        SectionHeader(listing, "SCWC_SectionBurn", jumpExt.SpaceIcon);
        DoVacuumBurnRow(listing);
        SectionHeader(listing, "SCWC_SectionLanding", null);
        blastRadius.DoRow(listing);
        roofRadius.DoRow(listing);
        creatureDamage.DoRow(listing);
        armorPenetration.DoRow(listing);
        wallDamage.DoRow(listing);
        structureDamage.DoRow(listing);
        shouldersBonus.DoRow(listing);
        SummaryRow(listing, LandingSummary);
        listing.Gap(SectionGap);
        contentHeight = listing.CurHeight;
        listing.End();
        Widgets.EndScrollView();

        if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - buttonHeight, buttonWidth, buttonHeight),
                "RestoreToDefaultSettings".Translate()))
        {
            foreach (Tunable tunable in All)
                tunable.Reset();
            vacuumBurn = true;
        }
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

    // A muted line under a section's rows, indented and padded as they are, as tall as its text.
    private static void SummaryRow(Listing_Standard listing, Func<string> summary)
    {
        string text = summary();
        float width = listing.ColumnWidth - Tunable.RowIndent - Tunable.RowEndPad;
        Rect rect = listing.GetRect(Text.CalcHeight(text, width));
        rect.xMin += Tunable.RowIndent;
        rect.xMax -= Tunable.RowEndPad;
        GUI.color = ColoredText.SubtleGrayColor;
        Widgets.Label(rect, text);
        GUI.color = Color.white;
    }

    // A checkbox row laid out as a Tunable row is, the box where a slider row shows its value.
    // Disabled, and labelled as needing Odyssey, when no loaded def can make a space map
    // (Ability_BreachJump.SpaceMapsPossible); the setting itself is read live, so a change
    // needs no restart.
    private void DoVacuumBurnRow(Listing_Standard listing)
    {
        Rect row = listing.GetRect(Tunable.RowHeight);
        row.xMin += Tunable.RowIndent;
        row.xMax -= Tunable.RowEndPad;
        Widgets.DrawHighlightIfMouseover(row);
        TooltipHandler.TipRegion(row, "SCWC_VacuumBurnDesc".Translate());

        bool spaceMaps = Ability_BreachJump.SpaceMapsPossible;
        string label = "SCWC_VacuumBurn".Translate();
        if (!spaceMaps)
            label += "SCWC_RequiresOdysseySuffix".Translate();
        Widgets.CheckboxLabeled(row, label, ref vacuumBurn, disabled: !spaceMaps);
        listing.Gap(Tunable.RowGap);
    }

    // The header's text in the medium font after its icon, a square as tall as the header; a
    // header without one (the landing's, or one whose texture is missing) starts its text at
    // the left edge rather than after an empty slot.
    private static void SectionHeader(Listing_Standard listing, string key, Texture2D icon, bool first = false)
    {
        const float headerGap = 4f;
        const float headerHeight = 32f;
        const float iconPad = 8f;
        if (!first)
            listing.Gap(SectionGap);
        Rect rect = listing.GetRect(headerHeight);
        if (icon != null && icon != BaseContent.BadTex)
        {
            GUI.DrawTexture(rect.LeftPartPixels(headerHeight), icon);
            rect.xMin += headerHeight + iconPad;
        }
        GameFont font = Text.Font;
        TextAnchor anchor = Text.Anchor;
        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(rect, key.Translate());
        Text.Anchor = anchor;
        Text.Font = font;
        listing.Gap(headerGap);
    }

    private struct Defs
    {
        public AbilityDef jump;
        public BreachJumpExtension ext;
        public CompProperties_ApparelReloadable tank;
        public StatModifier range;
        public StatModifier power;
        public DamageDef breach;
    }

    // All or nothing: with any target missing (another mod patched it away) the settings
    // leave every def at its XML values and say so once, as a warning, since the set itself
    // still works.
    private static bool warned;

    private static bool TryGetDefs(out Defs defs)
    {
        AbilityDef jump = DefDatabase<AbilityDef>.GetNamedSilentFail(JumpDefName);
        ThingDef armor = DefDatabase<ThingDef>.GetNamedSilentFail(ArmorDefName);
        ThingDef shoulders = DefDatabase<ThingDef>.GetNamedSilentFail(ShouldersDefName);
        defs = new Defs
        {
            jump = jump,
            ext = jump?.GetModExtension<BreachJumpExtension>(),
            tank = armor?.GetCompProperties<CompProperties_ApparelReloadable>(),
            range = armor?.equippedStatOffsets?.FirstOrDefault(m => m.stat == SCWC_DefOf.SCWC_BreachJumpRange),
            power = shoulders?.equippedStatOffsets?.FirstOrDefault(m => m.stat == SCWC_DefOf.SCWC_BreachPower),
            breach = SCWC_DefOf.SCWC_Breach,
        };
        if (defs.ext != null && defs.tank != null && defs.range != null && defs.power != null && defs.breach != null)
            return true;
        if (!warned)
            Log.Warning("[Shipcracker Warcasket] Settings could not find the breach jump, its tank, the armor's jump range or the shoulders' breach power; the set keeps its XML values.");
        warned = true;
        return false;
    }
}
