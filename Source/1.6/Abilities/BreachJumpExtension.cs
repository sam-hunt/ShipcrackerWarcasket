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

    // Landing explosion damage and armor penetration against pawns (buildings get the def's
    // factors and the wearer's SCWC_BreachPower on top, in DamageWorker_Breach), and the blast
    // radius, which Ability_BreachJump also previews at the hovered cell while targeting.
    public int breachDamage = 20;
    public float breachArmorPenetration = 0.15f;
    public float breachRadius = 2f;

    // Radius of the roof punched through at takeoff and landing; kept one below breachRadius so
    // the hole in the ceiling is tighter than the hole in the wall. 0 is the wearer's cell alone.
    public float roofPunchRadius = 1f;

    // Space flight speed as a multiple of the wearer's VFEP_FlightSpeed (base 12 cells/s), and
    // a ceiling on flight time: the jump has no range limit in space, so past the cap the flyer
    // speeds up again rather than hang for a map-length hop. Read by PawnFlyer_BreachJump.
    public float spaceFlightSpeedFactor = 3f;
    public float spaceFlightMaxSeconds = 4f;

    // Effecters the flyer plays for the whole flight: flightEffecter on a planet,
    // spaceFlightEffecter on a space map (see PawnFlyer_BreachJump for why they differ). The
    // main tree names our orange copies of VFEP's Aerial and Shock exhausts; the Vanilla
    // Gravship Expanded root swaps in the purple astroflame pair beside its astrofuel patch,
    // since that is the fuel the tank burns there. Null falls back to the orange pair.
    public EffecterDef flightEffecter;
    public EffecterDef spaceFlightEffecter;

    // Gizmo icon shown while the wearer stands on a space map; null keeps the def's iconPath.
    [NoTranslate] public string spaceIconPath;

    // Sentence appended to the def's description in the gizmo tooltip, but only when a loaded
    // def can produce a space map (see Ability_BreachJump.SpaceMapsPossible), so a game without
    // Odyssey never describes a mode it cannot reach. An extension field rather than an XML
    // patch from a compat root: the DefInjected walker reaches extension strings, so this
    // translates as one more entry in the main tree instead of a gated file that would collide
    // with the main-tree entry for the same field. Null or empty appends nothing.
    [MustTranslate] public string vacuumDescription;

    // Gizmo label and tooltip title while the wearer stands on a space map, lowercase like a
    // def label (the ability class capitalises it). Same one-def, two-state pattern as VFEP's
    // grappling hook and its labelUnloaded. Null or empty keeps the def's label everywhere.
    [MustTranslate] public string vacuumLabel;

    // Cached per def instance rather than in a static field: the game warns about static
    // Texture2D fields on types without [StaticConstructorOnStartup]. Loaded lazily from the
    // gizmo path, which is always the main thread.
    [Unsaved(false)] private Texture2D spaceIcon;

    public Texture2D SpaceIcon =>
        spaceIconPath.NullOrEmpty() ? null : spaceIcon ??= ContentFinder<Texture2D>.Get(spaceIconPath);
}
