using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace ShipcrackerWarcasket;

// The set's mod settings: the window frame, the ExposeData / LoadDefaultsFromDefs / ApplyToDefs /
// ResetToDefaults fan-out and the shared row helpers. The class is split across
// Settings_<Section>.cs partials, one per window section, each owning its fields, scribe
// entries, def-backed defaults, apply and draw, so a setting is a one-file edit. Each Tunable
// overrides a def field, written onto the def by ApplyToDefs (on every play-data load, then
// whenever the window closes), so every reader of those fields, vanilla's and VEF's included,
// sees the setting with no code of its own. The defaults, and the rationale for them, are the
// defs'. Nothing here is saved into a game: a save loads the same with any settings, and with
// none.
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
public partial class ShipcrackerWarcasketSettings : ModSettings
{
    private const string JumpDefName = "SCWC_BreachJump";
    private const string ArmorDefName = "SCWC_Warcasket_Shipcracker";
    private const string ShouldersDefName = "SCWC_WarcasketShoulders_Shipcracker";

    // The whitespace above every section header but the first, and below the last row.
    private const float SectionGap = 28f;

    // Bumped by every ApplyToDefs, including the one on each play-data load.
    public static int Generation { get; private set; }

    private bool defaultsLoaded;
    private Vector2 scrollPosition;
    private float contentHeight;

    private static string Whole(float v) => Mathf.RoundToInt(v).ToString();

    public override void ExposeData()
    {
        base.ExposeData();
        ExposeJumpSettings();
        ExposeBurnSettings();
        ExposeLandingSettings();
    }

    // Reads the def-backed defaults before ApplyToDefs overwrites them; defs must be loaded, so
    // Startup.Run calls this once per load, never the Mod constructor.
    public void LoadDefaultsFromDefs()
    {
        if (!TryGetDefs(out Defs defs))
            return;
        LoadJumpDefaults(defs);
        LoadBurnDefaults(defs);
        LoadLandingDefaults(defs);
        defaultsLoaded = true;
    }

    public void ApplyToDefs()
    {
        if (!defaultsLoaded || !TryGetDefs(out Defs defs))
            return;
        ApplyJumpToDefs(defs);
        ApplyLandingToDefs(defs);
        Generation++;
        TrimWornTanks();
    }

    public void ResetToDefaults()
    {
        ResetJumpSettings();
        ResetBurnSettings();
        ResetLandingSettings();
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
        DrawJumpSection(listing);
        DrawBurnSection(listing);
        DrawLandingSection(listing);
        listing.Gap(SectionGap);
        contentHeight = listing.CurHeight;
        listing.End();
        Widgets.EndScrollView();

        if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - buttonHeight, buttonWidth, buttonHeight),
                "RestoreToDefaultSettings".Translate()))
        {
            ResetToDefaults();
        }
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
