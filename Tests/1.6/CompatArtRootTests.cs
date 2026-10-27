using System;
using System.IO;
using Xunit;

namespace ShipcrackerWarcasket.Tests;

// A compat art root under Mods/<Mod>/Textures/ works by shadowing a main-tree texture at the same
// relative path: LoadFolders.xml lists the gated root after "/", so the later root wins a shared
// path. A file with no twin at Textures/<same relative path> is dead art, never shown without
// that mod and a differently-drawn mod with it, and nothing at run time reports it.
public class CompatArtRootTests
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

    public static TheoryData<string> CompatArtFiles()
    {
        var data = new TheoryData<string>();
        foreach (var modDir in Directory.EnumerateDirectories(Path.Combine(RepoRoot, "Mods")))
        {
            var textures = Path.Combine(modDir, "Textures");
            if (!Directory.Exists(textures))
                continue;
            foreach (var file in Directory.EnumerateFiles(textures, "*", SearchOption.AllDirectories))
                data.Add(file.Substring(RepoRoot.Length + 1).Replace('\\', '/'));
        }
        return data;
    }

    [Fact]
    public void CompatArtRoots_HoldAtLeastOneFile()
    {
        Assert.NotEmpty(CompatArtFiles());
    }

    [Theory]
    [MemberData(nameof(CompatArtFiles))]
    public void EveryCompatTexture_ShadowsAMainTreeTexture(string relativePath)
    {
        // Mods/<Mod>/Textures/<rest> maps to Textures/<rest>.
        var parts = relativePath.Split(new[] { '/' }, 4);
        var twin = "Textures/" + parts[3];
        Assert.True(File.Exists(Path.Combine(RepoRoot, twin)), $"{relativePath} has no main-tree twin at {twin}");
    }
}
