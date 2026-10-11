using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ShipcrackerWarcasket.Tests;

// The conversions between the mod settings' units and the defs' fields: a jump's chemfuel cost
// into tank charges (BreachJumpExtension.ChargesPerJump) and the landing's damage numbers into
// the SCWC_Breach factors (BreachTuning). The shipped defs must round-trip unchanged, so a
// player who never opens the settings plays the XML's numbers.
public class SettingsConversionTests
{
    [Fact]
    public void Chemfuel_OneChargePerChemfuel()
    {
        Assert.Equal(20, BreachJumpExtension.ChargesPerJump(20, 1f, 1));
    }

    [Fact]
    public void Astrofuel_HalvesTheCharges()
    {
        Assert.Equal(10, BreachJumpExtension.ChargesPerJump(20, 2f, 1));
    }

    [Fact]
    public void OddCost_RoundsUp()
    {
        Assert.Equal(11, BreachJumpExtension.ChargesPerJump(21, 2f, 1));
        Assert.Equal(1, BreachJumpExtension.ChargesPerJump(1, 2f, 1));
    }

    [Fact]
    public void AmmoPerCharge_DividesTheCharges()
    {
        Assert.Equal(4, BreachJumpExtension.ChargesPerJump(20, 1f, 5));
    }

    [Fact]
    public void ZeroCost_IsFree()
    {
        Assert.Equal(0, BreachJumpExtension.ChargesPerJump(0, 1f, 1));
        Assert.Null(BreachJumpExtension.JumpsLabel(100, 100, BreachJumpExtension.ChargesPerJump(0, 2f, 1)));
    }

    [Fact]
    public void ShippedFactors_RoundTrip()
    {
        // 20 damage x30: 600 to a wall (impassable 1), 150 to other structures (passable 0.25).
        Assert.Equal(600f, BreachTuning.StructureDamage(20, 30f, 1f), 3);
        Assert.Equal(150f, BreachTuning.StructureDamage(20, 30f, 0.25f), 3);
        Assert.Equal(1f, BreachTuning.StructureFactor(600f, 20, 30f), 5);
        Assert.Equal(0.25f, BreachTuning.StructureFactor(150f, 20, 30f), 5);
    }

    [Fact]
    public void StructureDamage_HoldsWhenCreatureDamageMoves()
    {
        var factor = BreachTuning.StructureFactor(600f, 40, 30f);
        Assert.Equal(600f, BreachTuning.StructureDamage(40, 30f, factor), 3);
    }

    [Fact]
    public void ZeroBuildingFactor_StaysFinite()
    {
        Assert.Equal(30f, BreachTuning.StructureFactor(600f, 20, 0f), 5);
    }

    // The VGE root states astrofuel's chemfuel value rather than a pre-halved cost, so the main
    // def's cost and the root's rate must still come to the 10 astrofuel a jump it was tuned to,
    // and its smaller tank to the same 5 jumps as the main tree's.
    [Fact]
    public void VgeRoot_KeepsTenAstrofuelPerJumpAndFiveJumps()
    {
        var root = RepoRoot();
        var ext = XDocument.Load(Path.Combine(root, "1.6/Defs/AbilityDefs/BreachJump.xml")).Descendants("li")
            .Single(e => (string)e.Attribute("Class") == "ShipcrackerWarcasket.BreachJumpExtension");
        var fuelPerJump = int.Parse(ext.Element("fuelPerJump")!.Value, CultureInfo.InvariantCulture);

        var patch = XDocument.Load(Path.Combine(root, "1.6/Mods/VanillaGravshipExpanded/Patches/BreachJumpAstrofuel.xml"));
        Assert.DoesNotContain(patch.Descendants("xpath"), x => x.Value.EndsWith("/fuelPerJump", StringComparison.Ordinal));
        var rate = float.Parse(patch.Descendants("chemfuelPerAmmo").Single().Value, CultureInfo.InvariantCulture);

        var chargesPerJump = BreachJumpExtension.ChargesPerJump(fuelPerJump, rate, 1);
        Assert.Equal(10, chargesPerJump);

        var maxCharges = int.Parse(patch.Descendants("maxCharges").Single().Value, CultureInfo.InvariantCulture);
        var armor = XDocument.Load(Path.Combine(root, "1.6/Defs/ThingDefs_Misc/Warcasket_Shipcracker.xml"));
        var mainMaxCharges = int.Parse(armor.Descendants("maxCharges").Single().Value, CultureInfo.InvariantCulture);
        Assert.Equal(mainMaxCharges / fuelPerJump, maxCharges / chargesPerJump);
    }

    private static string RepoRoot()
    {
        for (var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); dir != null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "ShipcrackerWarcasket.sln")))
                return dir.FullName;
        }
        throw new InvalidOperationException("ShipcrackerWarcasket.sln not found above the test output folder");
    }
}
