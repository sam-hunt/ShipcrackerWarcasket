using System;
using UnityEngine;
using Verse;

namespace ShipcrackerWarcasket;

// One player-tunable number, stored in the units of the def field it overrides. Until the
// player moves its slider it is DefaultValue, read from that field when defs load
// (ShipcrackerWarcasketSettings.LoadDefaultsFromDefs), so the def and its header stay the record
// of the shipped tuning and another mod's XML patch moves the default too. Only an override is
// saved; a slider landing within half a step of the default clears it, and a saved override
// outside the slider's range (saved before the range moved) reads as its nearest end.
public sealed class Tunable
{
    private const float Unset = -1f;

    // A row is the label, the slider and the value side by side, as a vanilla SliderLabeled row
    // is, with a gap below it so neighbouring rows read apart, indented so it sits under its
    // section header (Listing.Indent only moves the cursor and would push the row off the right)
    // and padded on the right so it does not run up against the scrollbar.
    // The value column is sized to the widest value a row can show, so the slider runs out to
    // it and the row ends near the window's edge as the checkbox rows do; every format here is
    // at most four characters, so "00.00" with the default suffix (as translated) bounds it.
    // The settings' checkbox rows share the height, gap, indent and pad.
    internal const float RowHeight = 30f;
    internal const float RowGap = 6f;
    internal const float RowIndent = Listing.DefaultIndent;
    internal const float RowEndPad = Listing.DefaultIndent;
    private const float LabelShare = 0.4f;
    private const string WidestValue = "00.00";
    private const float SliderPad = 10f;

    private readonly string key;
    private readonly float min;
    private readonly float max;
    private readonly float step;
    private readonly Func<float, string> format;
    private float value = Unset;

    public Tunable(string key, float min, float max, float step, Func<float, string> format)
    {
        this.key = key;
        this.min = min;
        this.max = max;
        this.step = step;
        this.format = format;
        DefaultValue = Unset;
    }

    public float DefaultValue { get; set; }

    // Stored units per shown unit. The fuel-per-jump row stores chemfuel, so the saved number
    // means the same with any tank, but shows the tank's own fuel beside the rows counted in
    // it: under VGE it reads 10 astrofuel where it stores 20 chemfuel. The slider, the value,
    // the default and the tooltip all show scaled; Value and IntValue stay stored.
    public float DisplayScale { get; set; } = 1f;

    // Filled into the label as {0} when set: the name of the fuel the tank currently takes, for
    // the rows counted in it.
    public string LabelArg { get; set; }

    // Filled into the tooltip as {1} onward when set, after the default's {0}: figures the
    // tooltip quotes from other defs, read when the defaults are, so they follow whatever the
    // loaded mods make of those defs.
    public string[] DescArgs { get; set; }

    public float Value => value < 0f ? DefaultValue : Mathf.Clamp(value, min, max);

    public int IntValue => Mathf.RoundToInt(Value);

    public void Reset() => value = Unset;

    public void ExposeData() => Scribe_Values.Look(ref value, key, Unset);

    // The stored value a slider released at shown lands on: the nearest step up from the
    // minimum, then clamped to the range. The step is the row's in stored units; a scale that
    // would make it a fraction of a shown unit rounds it to whole shown units instead, so a
    // scaled row never shows a value its format rounds away. Pure, for the tests.
    public static float Snap(float shown, float min, float max, float step, float scale)
    {
        float stored = Mathf.Clamp(shown * scale, min, max);
        float storedStep = StoredStep(step, scale);
        return Mathf.Clamp(min + Mathf.Round((stored - min) / storedStep) * storedStep, min, max);
    }

    public static float StoredStep(float step, float scale) =>
        scale == 1f ? step : Mathf.Max(1f, Mathf.Round(step / scale)) * scale;

    // Label "SCWC_<key>" (the unit is part of it, or LabelArg) on the left, the slider in the
    // middle, the current value on the right with "(default)" while unset, and "SCWC_<key>Desc"
    // with the default and any DescArgs as the whole row's tooltip.
    public void DoRow(Listing_Standard listing)
    {
        Rect row = listing.GetRect(RowHeight);
        row.xMin += RowIndent;
        row.xMax -= RowEndPad;
        Rect labelRect = row.LeftPart(LabelShare);
        float valueWidth = Text.CalcSize(WidestValue + "SCWC_DefaultSuffix".Translate()).x;
        var valueRect = new Rect(row.xMax - valueWidth, row.y, valueWidth, row.height);
        var sliderRect = new Rect(labelRect.xMax + SliderPad, row.y,
            valueRect.x - labelRect.xMax - 2f * SliderPad, row.height);

        Widgets.DrawHighlightIfMouseover(row);
        TooltipHandler.TipRegion(row, Tooltip());

        string shown = Format(Value);
        if (value < 0f)
            shown += "SCWC_DefaultSuffix".Translate();
        TextAnchor anchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(labelRect, LabelArg == null ? ("SCWC_" + key).Translate() : ("SCWC_" + key).Translate(LabelArg));
        Widgets.Label(valueRect, shown);
        Text.Anchor = anchor;

        float current = Value / DisplayScale;
        float raw = Widgets.HorizontalSlider(sliderRect, current, min / DisplayScale, max / DisplayScale, middleAlignment: true);
        listing.Gap(RowGap);
        if (raw == current)
            return;
        float snapped = Snap(raw, min, max, step, DisplayScale);
        value = Mathf.Abs(snapped - DefaultValue) < StoredStep(step, DisplayScale) * 0.5f ? Unset : snapped;
    }

    private string Format(float stored) => format(stored / DisplayScale);

    private string Tooltip()
    {
        string descKey = "SCWC_" + key + "Desc";
        if (DescArgs == null)
            return descKey.Translate(Format(DefaultValue));
        var args = new NamedArgument[DescArgs.Length + 1];
        args[0] = Format(DefaultValue);
        for (int i = 0; i < DescArgs.Length; i++)
            args[i + 1] = DescArgs[i];
        return descKey.Translate(args);
    }
}
