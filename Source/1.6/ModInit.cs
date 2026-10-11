using HarmonyLib;
using Verse;

namespace ShipcrackerWarcasket;

// Mod entry point. Applies every [HarmonyPatch] class in this assembly, then reads the setting
// defaults from the loaded defs and writes the player's settings onto them
// (ShipcrackerWarcasketSettings).
//
// Deliberately a [StaticConstructorOnStartup] rather than a Mod-subclass constructor:
// Mod constructors run BEFORE any defs are loaded, and applying a Harmony patch
// JIT-compiles the target and runs its declaring type's static constructor. This mod
// exists to extend another mod (VFE Pirates), so its patch targets will often live in
// VFEPirates.dll, and a VFEP type whose static ctor resolves defs would be permanently
// broken by an early patch. Static constructors on startup run after all defs have
// loaded, which makes foreign-target patches safe here without any deferral machinery,
// and is the first point the settings can reach the defs they override. Keep PatchAll here
// rather than in ShipcrackerWarcasketMod's constructor.
[StaticConstructorOnStartup]
public static class ModInit
{
    static ModInit()
    {
        // The Harmony id only needs to be unique; convention is the mod's packageId.
        var harmony = new Harmony("shunter.shipcrackerwarcasket");
        harmony.PatchAll();

        ShipcrackerWarcasketMod.Settings.LoadDefaultsFromDefs();
        ShipcrackerWarcasketMod.Settings.ApplyToDefs();
    }
}
