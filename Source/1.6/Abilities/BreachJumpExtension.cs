using UnityEngine;
using Verse;

namespace ShipcrackerWarcasket;

// Tuning knobs for SCWC_BreachJump, read by Ability_BreachJump and PawnFlyer_BreachJump.
// They live on the AbilityDef rather than the apparel so a compat root can retune a single
// node with a patch (the Vanilla Gravship Expanded root halves fuelPerJump when the tank is
// switched to astrofuel, which refines 2:1 from chemfuel).
public class BreachJumpExtension : DefModExtension
{
    // Charges of the holder's CompApparelReloadable that one jump burns.
    public int fuelPerJump = 20;

    // Landing explosion damage before the wearer's SCWC_BreachPower multiplier.
    public int breachDamage = 30;
    public float breachRadius = 3f;

    // Ceiling on flight time on space maps, where the jump has no range limit; the flyer
    // speeds up rather than hang in the air for a long hop.
    public float spaceFlightMaxSeconds = 4f;

    // Gizmo icon shown while the wearer stands on a space map; null keeps the def's iconPath.
    [NoTranslate] public string spaceIconPath;

    // Cached per def instance rather than in a static field: the game warns about static
    // Texture2D fields on types without [StaticConstructorOnStartup]. Loaded lazily from the
    // gizmo path, which is always the main thread.
    [Unsaved(false)] private Texture2D spaceIcon;

    public Texture2D SpaceIcon =>
        spaceIconPath.NullOrEmpty() ? null : spaceIcon ??= ContentFinder<Texture2D>.Get(spaceIconPath);
}
