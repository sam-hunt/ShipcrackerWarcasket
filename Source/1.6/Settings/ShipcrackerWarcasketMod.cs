using UnityEngine;
using Verse;

namespace ShipcrackerWarcasket;

// Owns the mod settings only. The constructor runs before defs load, so it neither patches
// (ModInit's comment) nor touches the defs the settings override: ModInit reads their defaults
// and applies the settings once defs exist, and WriteSettings re-applies them whenever the
// settings window closes.
public class ShipcrackerWarcasketMod : Mod
{
    public ShipcrackerWarcasketMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<ShipcrackerWarcasketSettings>();
    }

    public static ShipcrackerWarcasketSettings Settings { get; private set; }

    public override string SettingsCategory() => "SCWC_SettingsCategory".Translate();

    public override void DoSettingsWindowContents(Rect inRect) => Settings.DoWindowContents(inRect);

    public override void WriteSettings()
    {
        base.WriteSettings();
        Settings.ApplyToDefs();
    }
}
