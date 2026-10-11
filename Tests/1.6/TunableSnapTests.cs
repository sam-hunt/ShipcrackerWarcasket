using Xunit;

namespace ShipcrackerWarcasket.Tests;

// A settings slider's snap (Tunable.Snap): the stored value a release lands on, in the row's
// stored units, when the row shows those units as they are and when it shows them scaled to
// the tank's fuel (the fuel-per-jump row under VGE stores chemfuel and shows astrofuel).
public class TunableSnapTests
{
    [Fact]
    public void Unscaled_SnapsToTheStep()
    {
        Assert.Equal(22f, Tunable.Snap(22.9f, 0f, 50f, 2f, 1f));
        Assert.Equal(24f, Tunable.Snap(23.1f, 0f, 50f, 2f, 1f));
        Assert.Equal(2.5f, Tunable.Snap(2.6f, 0f, 6f, 0.5f, 1f));
    }

    [Fact]
    public void Unscaled_ClampsToTheRange()
    {
        Assert.Equal(50f, Tunable.Snap(80f, 0f, 50f, 2f, 1f));
        Assert.Equal(10f, Tunable.Snap(3f, 10f, 300f, 10f, 1f));
    }

    // Shown in astrofuel at two chemfuel each: a release at 10 stores 20, and the 2-chemfuel
    // step is one whole astrofuel, so 10.4 and 10.6 land on 20 and 22.
    [Fact]
    public void Astrofuel_StoresChemfuel()
    {
        Assert.Equal(20f, Tunable.Snap(10f, 0f, 50f, 2f, 2f));
        Assert.Equal(20f, Tunable.Snap(10.4f, 0f, 50f, 2f, 2f));
        Assert.Equal(22f, Tunable.Snap(10.6f, 0f, 50f, 2f, 2f));
        Assert.Equal(50f, Tunable.Snap(25f, 0f, 50f, 2f, 2f));
    }

    // A rate that does not divide the step evenly rounds the step to whole shown units, so
    // every stop shows as a whole number.
    [Fact]
    public void OddRate_StepsInWholeShownUnits()
    {
        Assert.Equal(3f, Tunable.StoredStep(2f, 3f));
        Assert.Equal(9f, Tunable.Snap(3.2f, 0f, 50f, 2f, 3f));
        Assert.Equal(2f, Tunable.StoredStep(2f, 0.5f));
        Assert.Equal(2f, Tunable.Snap(4f, 0f, 50f, 2f, 0.5f));
    }
}
