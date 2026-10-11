using UnityEngine;
using Verse;

namespace ShipcrackerWarcasket;

// Tuning knobs for SCWC_BreachJump, read by Ability_BreachJump and PawnFlyer_BreachJump.
// They live on the AbilityDef rather than the apparel so a compat root can retune a single
// node with a patch (the Vanilla Gravship Expanded root sets chemfuelPerAmmo when it switches
// the tank to astrofuel, which refines 2:1 from chemfuel). The mod settings overwrite the
// numeric fields at startup and on every settings write (ShipcrackerWarcasketSettings).
public class BreachJumpExtension : DefModExtension
{
    // What one jump costs, in chemfuel whatever the tank takes, so the setting means the same
    // with or without the VGE root. 0 makes jumps free.
    public int fuelPerJump = 20;

    // Chemfuel one unit of the tank's ammoDef is worth: 1 for chemfuel itself, 2 for astrofuel.
    public float chemfuelPerAmmo = 1f;

    // Landing explosion damage and armor penetration against pawns (buildings get the def's
    // factors and the wearer's SCWC_BreachPower on top, in DamageWorker_Breach), and the blast
    // radius, which Ability_BreachJump also previews at the hovered cell while targeting. A
    // radius of 0 turns the landing blast off.
    public int breachDamage = 20;
    public float breachArmorPenetration = 0.15f;
    public float breachRadius = 2f;

    // Radius of the roof punched through at takeoff and landing, set apart from breachRadius so
    // the hole in the ceiling can be tighter than the hole in the wall. 0 punches nothing; any
    // radius under 1 is the wearer's cell alone.
    public float roofPunchRadius = 1f;

    // Space flight speed as a multiple of the wearer's VFEP_FlightSpeed (base 12 cells/s), and
    // a ceiling on flight time: the jump has no range limit in space, so past the cap the flyer
    // speeds up again rather than hang for a map-length hop. Read by PawnFlyer_BreachJump.
    public float spaceFlightSpeedFactor = 3f;
    public float spaceFlightMaxSeconds = 4f;

    // Tank charges one jump burns: fuelPerJump chemfuel over what a charge is worth in chemfuel
    // (its ammoCountPerCharge units of ammo at chemfuelPerAmmo each), rounded up so a jump never
    // costs less than the setting says. 0 only for a free jump.
    public static int ChargesPerJump(int fuelPerJump, float chemfuelPerAmmo, int ammoPerCharge)
    {
        if (fuelPerJump <= 0)
            return 0;
        var chemfuelPerCharge = Mathf.Max(chemfuelPerAmmo, 0.01f) * Mathf.Max(ammoPerCharge, 1);
        return Mathf.Max(1, Mathf.CeilToInt(fuelPerJump / chemfuelPerCharge - 0.0001f));
    }

    // The gizmo's top-right count, "jumps / max" in whole jumps the tank holds, in the "N / M"
    // shape vanilla's charged commands use. A partial refuel floors: a tank at 37 of 100 is
    // "1 / 5", matching where the reload reason kicks in. Null when there is nothing to count.
    public static string JumpsLabel(int remainingCharges, int maxCharges, int chargesPerJump)
    {
        if (chargesPerJump <= 0 || maxCharges <= 0)
            return null;
        return $"{remainingCharges / chargesPerJump} / {maxCharges / chargesPerJump}";
    }

    // Flight time for a jump of distance cells at flightSpeed cells/s. spaceBurn is the jump's
    // extension for the breach burn and null for a jump; minSeconds (the flyer def's
    // flightDurationMin) floors both, so it outranks the space cap.
    public static float FlightSeconds(float distance, float flightSpeed, BreachJumpExtension spaceBurn, float minSeconds)
    {
        var seconds = Mathf.Max(distance, 1f) / flightSpeed;
        if (spaceBurn != null)
        {
            seconds /= Mathf.Max(spaceBurn.spaceFlightSpeedFactor, 0.01f);
            seconds = Mathf.Min(seconds, spaceBurn.spaceFlightMaxSeconds);
        }
        return Mathf.Max(seconds, minSeconds);
    }

    // Effecters the flyer plays for the whole flight: flightEffecter for a jump,
    // spaceFlightEffecter for the burn (see PawnFlyer_BreachJump for why they differ). The
    // main tree names our orange copies of VFEP's Aerial and Shock exhausts; the Vanilla
    // Gravship Expanded root swaps in the purple astroflame pair beside its astrofuel patch,
    // since that is the fuel the tank burns there. Null falls back to the orange pair.
    public EffecterDef flightEffecter;
    public EffecterDef spaceFlightEffecter;

    // Opacity of the armor's thruster-outlet glow (PawnRenderNode_ThrusterGlow) over the cast,
    // as a curve of cast progress 0..1 to alpha 0..1; the flight then holds alpha 1 and there
    // is no fade-out. Null uses progress directly. Set in XML so it can be retuned without a
    // rebuild; the tint lives on the render node entry in the armor def, where a compat root
    // can recolor it.
    public SimpleCurve thrusterGlowCurve;

    // Gizmo icon shown while the jump is the breach burn; null keeps the def's iconPath.
    [NoTranslate] public string spaceIconPath;

    // Sentence appended to the def's description in the gizmo tooltip, but only when a loaded
    // def can produce a space map (see Ability_BreachJump.SpaceMapsPossible) and the burn is on
    // in the settings, so the tooltip never describes a mode the game cannot reach. An extension field rather than an XML
    // patch from a compat root: the DefInjected walker reaches extension strings, so this
    // translates as one more entry in the main tree instead of a gated file that would collide
    // with the main-tree entry for the same field. Null or empty appends nothing.
    [MustTranslate] public string vacuumDescription;

    // Gizmo label and tooltip title while the jump is the breach burn, lowercase like a
    // def label (the ability class capitalizes it). Same one-def, two-state pattern as VFEP's
    // grappling hook and its labelUnloaded. Null or empty keeps the def's label everywhere.
    [MustTranslate] public string vacuumLabel;

    // Cached per def instance rather than in a static field: the game warns about static
    // Texture2D fields on types without [StaticConstructorOnStartup]. Loaded lazily from the
    // gizmo path, which is always the main thread.
    [Unsaved(false)] private Texture2D spaceIcon;

    public Texture2D SpaceIcon
    {
        get
        {
            if (spaceIconPath.NullOrEmpty())
                return null;
            // Unity-overloaded ==, deliberately not ??= (UNT0023).
            if (spaceIcon == null)
                spaceIcon = ContentFinder<Texture2D>.Get(spaceIconPath);
            return spaceIcon;
        }
    }
}
