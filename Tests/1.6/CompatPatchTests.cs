using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;

namespace ShipcrackerWarcasket.Tests;

// The gated compat roots under 1.6/Mods/<Mod>/Patches/ only ever run with their mod active, and
// 1.6/Patches/SOS2Patch.xml sits behind a PatchOperationFindMod gate of its own, so a renamed node
// in a main-tree def would break any of them silently for every player who has that mod and
// nobody else. Every operation's xpath must resolve in the main tree's defs, merged as the game
// merges them, to one node per def it names: the SOS2 patch names its two defs in one xpath, as
// VFEP's does. A PatchOperationConditional's own xpath is the exception, a probe for a node that
// should be absent. The other main-tree patch, AerialJumpRange.xml, targets VFEP's defs and is
// out of reach here.
public class CompatPatchTests
{
    private static readonly string RepoRoot = FindRepoRoot();

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); dir != null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "ShipcrackerWarcasket.sln")))
                return dir.FullName;
        }
        throw new InvalidOperationException("ShipcrackerWarcasket.sln not found above the test output folder");
    }

    private static XDocument MergedDefs()
    {
        var defs = new XElement("Defs");
        foreach (var file in Directory.EnumerateFiles(Path.Combine(RepoRoot, "1.6/Defs"), "*.xml", SearchOption.AllDirectories))
            defs.Add(XDocument.Load(file).Root!.Elements());
        return new XDocument(defs);
    }

    public static TheoryData<string> CompatPatchFiles()
    {
        var data = new TheoryData<string>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(RepoRoot, "1.6/Mods"), "*.xml", SearchOption.AllDirectories)
                     .Where(f => f.Replace('\\', '/').Contains("/Patches/"))
                     .OrderBy(f => f, StringComparer.Ordinal))
            data.Add(file.Substring(RepoRoot.Length + 1));
        data.Add("1.6/Patches/SOS2Patch.xml");
        return data;
    }

    [Theory]
    [MemberData(nameof(CompatPatchFiles))]
    public void EveryXPath_ResolvesToOneMainTreeNodePerNamedDef(string relativePath)
    {
        var defs = MergedDefs();
        var patch = XDocument.Load(Path.Combine(RepoRoot, relativePath));
        var operations = patch.Descendants().Where(e => e.Attribute("Class") != null && e.Element("xpath") != null);
        Assert.NotEmpty(operations);
        foreach (var op in operations)
        {
            var xpath = op.Element("xpath")!.Value;
            var hits = defs.XPathSelectElements(xpath).Count();
            var expected = (string)op.Attribute("Class") == "PatchOperationConditional"
                ? 0
                : Regex.Matches(xpath, "defName\\s*=\\s*\"").Count;
            Assert.True(hits == expected, $"{relativePath}: {xpath} resolved {hits} node(s), expected {expected}");
        }
    }

    // The ability passes the extension's penetration and GenExplosion ignores the DamageDef's
    // default when handed an amount, so the two are the same number by convention in every root
    // that sets either; a reader of the DamageDef must not be misled.
    [Fact]
    public void CombatExtendedRoot_KeepsBlastPenetrationAndDamageDefDefaultEqual()
    {
        var patch = XDocument.Load(Path.Combine(RepoRoot, "1.6/Mods/CombatExtended/Patches/BreachPenetration.xml"));
        var blast = float.Parse(patch.Descendants("breachArmorPenetration").Single().Value, CultureInfo.InvariantCulture);
        var damageDef = float.Parse(patch.Descendants("defaultArmorPenetration").Single().Value, CultureInfo.InvariantCulture);
        Assert.Equal(blast, damageDef);
    }
}
