using UnityEngine;

namespace ShipcrackerWarcasket;

// The conversions between the settings window's damage numbers and the SCWC_Breach DamageDef's
// building factors, kept free of VEF and VFEP types so the tests can load them. Vanilla's
// DamageWorker.Apply multiplies a building's damage by buildingDamageFactor and then by the
// impassable or passable factor, so a structure takes the blast's amount (the creature damage)
// times both; the settings keep buildingDamageFactor as the def has it and solve for the second.
public static class BreachTuning
{
    // Damage a structure takes from a blast of creatureDamage, before SCWC_BreachPower.
    public static float StructureDamage(int creatureDamage, float buildingDamageFactor, float structureFactor) =>
        creatureDamage * buildingDamageFactor * structureFactor;

    // The impassable or passable factor that makes a blast of creatureDamage deal structureDamage.
    // A non-positive buildingDamageFactor (patched to nothing) is read as 1 so the answer stays
    // finite; the factors then carry the whole multiplier.
    public static float StructureFactor(float structureDamage, int creatureDamage, float buildingDamageFactor) =>
        structureDamage / (Mathf.Max(creatureDamage, 1) * (buildingDamageFactor > 0f ? buildingDamageFactor : 1f));
}
