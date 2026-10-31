using Xunit;

namespace ShipcrackerWarcasket.Tests;

// The settings sliders' pure arithmetic (Tunable): the stored value a release lands on (Snap,
// StoredStep), in the row's stored units, when the row shows those units as they are and when
// it shows them scaled to the tank's fuel (the fuel-per-jump row under VGE stores chemfuel and
// shows astrofuel); and a banded row's stops (Stops) and which one a stored value sits on
// (Nearest). This mod has no banded row, so the cases run the shared file's rule on bands of
// their own.
public class TunableTests
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

    // Banded steps (Tunable.StepBands): quarter steps up to 2, half steps from there to 5. The
    // stops are the minimum, then each band's step multiples within it, a boundary once.
    [Fact]
    public void Stops_FollowTheBands()
    {
        float[] stops = Tunable.Stops(0f, 5f, [(2f, 0.25f), (5f, 0.5f)]);
        Assert.Equal(0f, stops[0]);
        Assert.Equal(0.25f, stops[1]);
        Assert.Contains(2f, stops);
        Assert.Contains(2.5f, stops);
        Assert.DoesNotContain(2.25f, stops);
        Assert.Equal(5f, stops[^1]);
        Assert.Equal(1 + 8 + 6, stops.Length);
    }

    // A minimum off its band's grid is the first stop: five, then the tens.
    [Fact]
    public void Stops_KeepAnOffGridMinimum()
    {
        float[] stops = Tunable.Stops(5f, 60f, [(60f, 10f)]);
        Assert.Equal(5f, stops[0]);
        Assert.Equal(10f, stops[1]);
        Assert.Equal(60f, stops[^1]);
        Assert.Equal(7, stops.Length);
    }

    // A maximum short of the last band's end cuts the stops there.
    [Fact]
    public void Stops_EndAtTheMaximum()
    {
        float[] stops = Tunable.Stops(0f, 4f, [(2f, 1f), (10f, 2f)]);
        Assert.Equal([0f, 1f, 2f, 4f], stops);
    }

    [Fact]
    public void Nearest_PicksTheClosestStopLowerOnATie()
    {
        float[] stops = [0f, 1f, 2f, 4f];
        Assert.Equal(0, Tunable.Nearest(stops, -3f));
        Assert.Equal(2, Tunable.Nearest(stops, 2.4f));
        Assert.Equal(2, Tunable.Nearest(stops, 3f));
        Assert.Equal(3, Tunable.Nearest(stops, 9f));
    }
}
