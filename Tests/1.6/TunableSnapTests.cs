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

    // Banded steps (Tunable.StepBands): quarter steps up to 2, half steps from there to 5. A
    // value snaps to the grid of its own band, counted from that band's start; a release on the
    // boundary belongs to the band below.
    private static readonly (float upTo, float step)[] Bands = [(2f, 0.25f), (5f, 0.5f)];

    [Fact]
    public void Banded_SnapsToItsBandsStep()
    {
        Assert.Equal(0.75f, Tunable.Snap(0.8f, 0f, 5f, Bands));
        Assert.Equal(1f, Tunable.Snap(0.9f, 0f, 5f, Bands));
        Assert.Equal(3.5f, Tunable.Snap(3.3f, 0f, 5f, Bands));
        Assert.Equal(3f, Tunable.Snap(3.2f, 0f, 5f, Bands));
    }

    [Fact]
    public void Banded_ClampsToTheRange()
    {
        Assert.Equal(0f, Tunable.Snap(-1f, 0f, 5f, Bands));
        Assert.Equal(5f, Tunable.Snap(9f, 0f, 5f, Bands));
    }

    // Just over a boundary snaps to the band above's first stop or back to the boundary, never
    // to a stop the lower band's finer grid would offer.
    [Fact]
    public void Banded_BoundaryBelongsToTheBandBelow()
    {
        Assert.Equal(2f, Tunable.Snap(2f, 0f, 5f, Bands));
        Assert.Equal(2f, Tunable.Snap(2.2f, 0f, 5f, Bands));
        Assert.Equal(2.5f, Tunable.Snap(2.3f, 0f, 5f, Bands));
        Assert.Equal(0.25f, Tunable.BandStep(2f, 5f, Bands));
        Assert.Equal(0.5f, Tunable.BandStep(2.01f, 5f, Bands));
    }

    // The default-clearing threshold is half the step of the band the default sits in.
    [Fact]
    public void Banded_StepOfTheDefaultsBand()
    {
        Assert.Equal(0.25f, Tunable.BandStep(1f, 5f, Bands));
        Assert.Equal(0.5f, Tunable.BandStep(4f, 5f, Bands));
    }

    // No bands at all falls back to the clamped value as is.
    [Fact]
    public void Banded_NoBandsClampsOnly()
    {
        Assert.Equal(1.3f, Tunable.Snap(1.3f, 0f, 5f, null));
        Assert.Equal(5f, Tunable.Snap(7f, 0f, 5f, []));
    }
}
