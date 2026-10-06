using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Kept explicit (GenerateAssemblyInfo=false in the csproj) so the release flow can bump
// AssemblyVersion/AssemblyFileVersion in lockstep with About.xml's <modVersion> and
// CHANGELOG.md. Four-part X.Y.Z.0, where X.Y.Z is the mod's semantic version.
[assembly: AssemblyTitle("ShipcrackerWarcasket")]
[assembly: AssemblyDescription("A late-game warcasket set for Vanilla Factions Expanded - Pirates")]
[assembly: AssemblyProduct("ShipcrackerWarcasket")]
[assembly: AssemblyCopyright("Copyright © 2026")]
[assembly: ComVisible(false)]
[assembly: Guid("8de5c313-3fef-40fe-8dc1-095d73916ae6")]
[assembly: AssemblyVersion("1.0.1.0")]
[assembly: AssemblyFileVersion("1.0.1.0")]
// Mirrors About.xml <modVersion> verbatim, including any SemVer prerelease suffix
// (1.4.0-rc.1); the two numeric attributes above can't hold one and stay X.Y.Z.0.
[assembly: AssemblyInformationalVersion("1.0.1")]

// Test access to the pure helpers the Tests/1.6 suite covers (SpacePreviewCache).
[assembly: InternalsVisibleTo("ShipcrackerWarcasket.Tests")]
