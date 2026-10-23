namespace ShipcrackerWarcasket;

// Everything this mod reads off or writes onto the loaded defs at startup: the settings'
// defaults and overrides, and the def instances the settings window draws from. Idempotent, and
// run on every play-data load, not only the first: ModInit's static constructor runs it once per
// process, and the postfix on StaticConstructorOnStartupUtility.CallAll
// (Patches/StaticConstructorOnStartupUtility_CallAll) again after every in-process reload, which
// replaces every def instance and would otherwise leave the settings unapplied on the fresh
// ones. Every run sees defs fresh from XML (a reload clears the database before it rebuilds),
// so the defaults it reads are never values an earlier run wrote.
public static class Startup
{
    public static void Run()
    {
        ShipcrackerWarcasketMod.Settings.LoadDefaultsFromDefs();
        ShipcrackerWarcasketMod.Settings.ApplyToDefs();
    }
}
