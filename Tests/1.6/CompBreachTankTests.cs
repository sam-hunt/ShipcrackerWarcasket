using System.Reflection;
using RimWorld;
using Xunit;

namespace ShipcrackerWarcasket.Tests;

// The tank's trim (CompBreachTank.Trim): a count above the maximum drops to it, anything at or
// below is left alone. The count is vanilla's protected field, reached by reflection here only.
public class CompBreachTankTests
{
    private static readonly FieldInfo RemainingCharges =
        typeof(CompApparelVerbOwner_Charged).GetField("remainingCharges", BindingFlags.Instance | BindingFlags.NonPublic);

    private static CompBreachTank Tank(int maxCharges, int remaining)
    {
        var tank = new CompBreachTank { props = new CompProperties_ApparelReloadable { maxCharges = maxCharges } };
        RemainingCharges.SetValue(tank, remaining);
        return tank;
    }

    [Fact]
    public void AboveMaximum_DropsToIt()
    {
        var tank = Tank(50, 80);
        tank.Trim();
        Assert.Equal(50, tank.RemainingCharges);
    }

    [Fact]
    public void AtOrBelowMaximum_Unchanged()
    {
        var full = Tank(50, 50);
        full.Trim();
        Assert.Equal(50, full.RemainingCharges);

        var partial = Tank(100, 37);
        partial.Trim();
        Assert.Equal(37, partial.RemainingCharges);
    }
}
