using RimWorld;
using Verse;

namespace ShipcrackerWarcasket;

// Vanilla's continuous sprayer, spraying ExhaustSprayerDef.fleckDefNorth instead of fleckDef
// while the effecter's source faces north (2026-09-18).
//
// The exhaust flecks sit on the Projectile layer, one full altitude layer under Pawn, so the
// trail always drew under the flying wearer. Facing north the wearer's back, and so the thruster
// outlets the flecks leave from, faces the camera, and the flame should come out over the body.
// A fleck's altitude is read from its FleckDef every draw (FleckStatic.Draw), so the only way
// to move some flecks up is a second def, and the choice between the two has to be made where
// the flecks are created. Vanilla's MakeMote reads def.fleckDef and cannot be overridden, so
// this class keeps two defs per instance: the entry as written and a shallow copy with fleckDef
// swapped for fleckDefNorth, and points the inherited def field at whichever applies before
// each tick. Both are private to this instance; the shared def is never mutated.
//
// The facing read is the flying pawn's own Rotation, which is what the render tree draws the
// pawn with: PawnFlyer.MakeFlyer copies it from the caster, whom the cast job's wait toil has
// turned to face the target cell, so for a jump it is the flight direction rounded to a
// cardinal. It does not change in flight. Other facings keep the Projectile flecks: south
// hides the outlets behind the body, and side-on either order reads fine.
public class SubEffecter_ExhaustSprayer : SubEffecter_SprayerContinuous
{
    private readonly SubEffecterDef sideDef;
    private readonly SubEffecterDef northDef;

    public SubEffecter_ExhaustSprayer(SubEffecterDef def, Effecter parent) : base(def, parent)
    {
        sideDef = def;
        if (def is ExhaustSprayerDef { fleckDefNorth: not null } exhaust)
        {
            northDef = Gen.MemberwiseClone(def);
            northDef.fleckDef = exhaust.fleckDefNorth;
        }
    }

    public override void SubEffectTick(TargetInfo A, TargetInfo B)
    {
        def = northDef != null && FacesNorth(A) ? northDef : sideDef;
        base.SubEffectTick(A, B);
    }

    // The source is the flyer while the wearer is in flight; the pawn's rotation is the one
    // drawn. Falls back to the source thing's own rotation for an effecter played on a pawn.
    private static bool FacesNorth(TargetInfo source)
    {
        if (!source.HasThing)
            return false;
        var thing = source.Thing is PawnFlyer flyer ? flyer.FlyingPawn ?? (Thing)flyer : source.Thing;
        return thing.Rotation == Rot4.North;
    }
}
