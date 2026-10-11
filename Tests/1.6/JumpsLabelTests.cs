using Xunit;

namespace ShipcrackerWarcasket.Tests;

// The Breach Jump gizmo's "jumps / max" count: whole jumps the tank holds, floored, from the
// charges a jump burns (BreachJumpExtension.ChargesPerJump).
public class JumpsLabelTests
{
    [Fact]
    public void FullTank_IsFiveOfFive()
    {
        Assert.Equal("5 / 5", BreachJumpExtension.JumpsLabel(100, 100, 20));
    }

    [Fact]
    public void PartialRefuel_Floors()
    {
        Assert.Equal("1 / 5", BreachJumpExtension.JumpsLabel(37, 100, 20));
    }

    [Fact]
    public void Astrofuel_HalvesTheCost()
    {
        Assert.Equal("10 / 10", BreachJumpExtension.JumpsLabel(100, 100, 10));
    }

    [Fact]
    public void Empty_IsZeroOfMax()
    {
        Assert.Equal("0 / 5", BreachJumpExtension.JumpsLabel(0, 100, 20));
    }

    [Fact]
    public void NothingToCount_IsNull()
    {
        Assert.Null(BreachJumpExtension.JumpsLabel(100, 100, 0));
        Assert.Null(BreachJumpExtension.JumpsLabel(0, 0, 20));
    }
}
