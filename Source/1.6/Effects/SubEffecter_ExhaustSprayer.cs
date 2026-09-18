using RimWorld;
using Verse;

namespace ShipcrackerWarcasket;

// Vanilla's continuous sprayer, spraying ExhaustSprayerDef.fleckDefNorth instead of fleckDef
// while the effecter's source faces north.
//
// The exhaust flecks sit on Projectile, a layer under Pawn, so the trail draws under the flying
// wearer. Facing north the thruster outlets on the wearer's back face the camera and the flame
// should come out over the body. A fleck's altitude is read from its def every draw
// (FleckStatic.Draw), so raising some flecks means a second def chosen where the flecks are
// created. The raised copies sit on PawnState, the layer vanilla uses for motes held over a
// pawn; a fraction of a layer above Pawn still renders under the wearer, transparent draw order
// that close together is not plain height.
//
// MakeMote reads def.fleckDef and is not virtual, so each instance keeps the def as written and
// a shallow copy with fleckDef swapped, and points the inherited def field at one or the other
// before each tick. The shared def is never mutated.
//
// The facing read is the flying pawn's Rotation, which is what the render tree draws with:
// PawnFlyer.MakeFlyer copies it from the caster, whom the cast job's wait toil has turned to
// face the target, so it is the flight direction rounded to a cardinal and fixed for the flight.
// Other facings keep the low flecks: south hides the outlets behind the body, and side-on either
// order reads fine. PawnFlyer_BreachJump ticks the effecter after the position advances so this
// frame's flecks sit on the outlets at all (see its Tick).
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
        var thing = source.Thing is PawnFlyer flyer
            ? flyer.FlyingPawn ?? (Thing)flyer
            : source.Thing;
        return thing.Rotation == Rot4.North;
    }
}
