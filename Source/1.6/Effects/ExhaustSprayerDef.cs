using Verse;

namespace ShipcrackerWarcasket;

// Sub-effecter entry for the Breach Jump's exhaust sprayers (the flame, glow and smoke children
// of the four flight effecters), used through <li Class="ShipcrackerWarcasket.ExhaustSprayerDef">
// with subEffecterClass SubEffecter_ExhaustSprayer. Adds one field to the vanilla def: the fleck
// to spray instead of fleckDef while the flying wearer faces north. See the sub-effecter for why.
public class ExhaustSprayerDef : SubEffecterDef
{
    // A copy of fleckDef raised to the Pawn altitude layer (the SCWC_*North fleck defs), so the
    // trail draws over the wearer's back when the thruster outlets face the camera. Null keeps
    // fleckDef for every facing.
    public FleckDef fleckDefNorth;
}
