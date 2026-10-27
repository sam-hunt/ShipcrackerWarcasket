using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ShipcrackerWarcasket.Tests;

// Worn gizmos are yielded in comp order and the gizmo grid sorts them stably by Order, so the
// blast toggle sits beside the Breach Jump button only while its comp directly follows the
// abilities comp. Nothing at run time complains if a reorder splits them.
public class ArmorCompOrderTests
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

    private static string Describe(XElement li)
    {
        var cls = (string)li.Attribute("Class");
        var compClass = (string)li.Element("compClass");
        return $"<li Class=\"{cls}\"> compClass={compClass}";
    }

    [Fact]
    public void BlastToggleComp_FollowsTheAbilitiesCompDirectly()
    {
        var doc = XDocument.Load(Path.Combine(RepoRoot, "1.6/Defs/ThingDefs_Misc/Warcasket_Shipcracker.xml"));
        var comps = doc.Descendants("comps").First().Elements("li").ToList();
        var abilities = comps.FindIndex(li => (string)li.Attribute("Class") == "VEF.Abilities.CompProperties_AbilitiesApparel");
        Assert.True(abilities >= 0, "the armor's comps list has no VEF.Abilities.CompProperties_AbilitiesApparel entry");
        Assert.True(abilities + 1 < comps.Count, "the abilities comp is the last entry; CompBreachBlastToggle must follow it");

        var next = comps[abilities + 1];
        var isToggle = (string)next.Attribute("Class") == null
                       && (string)next.Element("compClass") == "ShipcrackerWarcasket.CompBreachBlastToggle";
        Assert.True(isToggle, "the entry after the abilities comp must be a comp with compClass ShipcrackerWarcasket.CompBreachBlastToggle, but it is " + Describe(next));
    }
}
