# CLAUDE.md

Guidance for Claude Code (claude.ai/code) when working in this repository.

## Project Overview

**Shipcracker Warcasket** is a RimWorld 1.6 mod adding a single new warcasket apparel set
(armor, shoulder pads, helmet) for Vanilla Factions Expanded - Pirates (VFE Pirates), tuned for
the Odyssey DLC's end-game content. VFE Pirates is a hard dependency; Odyssey is optional and
Odyssey-only content must load only when the DLC is active. Requires Harmony (kept as a
dependency for now; drop it before release if no patch ever lands).

**Key technologies:** C# (.NET Framework 4.7.2), Harmony, RimWorld modding API, XML defs.

**Def prefix:** `SCWC_`. Mod-specific feature work has not started; `TODOs.md` holds the
scoping notes for it.

### Where documentation lives

**This file holds only cross-cutting rules and rationale.** Per-item values, tuning numbers and
decompile-verified call paths live in the header comment of the file they describe. When adding
or changing something, put the *why* there and only add a line here if it constrains work in
other files. Do not restate def values or call paths here; they drift.

## Build Commands

```bash
# Build (outputs to 1.6/Assemblies/ AND atomically redeploys to the RimWorld Mods folder)
dotnet build ShipcrackerWarcasket.sln -c Release

# Stage the mod into an arbitrary folder (used by CI; same manifest as the local deploy)
dotnet build Source/1.6/ShipcrackerWarcasket.csproj -c Release \
  -t:StageMod -p:StageDir=/path/to/output/ShipcrackerWarcasket

# Override RimWorld install path
RIMWORLD_PATH="/path/to/RimWorld" dotnet build ShipcrackerWarcasket.sln -c Release
# Or: dotnet build -p:RimWorldPath="/path/to/RimWorld"
```

The build auto-detects the RimWorld install (Windows/Linux/Mac, including WSL targeting a Windows
install), falling back to the `Krafs.Rimworld.Ref` NuGet package in CI. Debug builds go to the
default `bin/` and never deploy; only Release builds touch `1.6/Assemblies/` and the Mods folder.

**WSL setup:** `RIMWORLD_PATH` in `~/.bashrc` pointing at the Windows install, e.g.
`/mnt/c/Program Files (x86)/Steam/steamapps/common/RimWorld`.

### Dependency-mod assemblies

`VFEPirates.dll` and `VEF.dll` are referenced compile-only (`Private="false"`), resolved by the
csproj in this order: explicit override (`-p:VfePiratesDir=` / `-p:VefDir=` or the `VFEP_PATH` /
`VEF_PATH` env vars), the Steam Workshop copy beside the install (ids 2723801948 and 2023507013),
the sibling source checkouts under `../VanillaExpanded/`, then the RimWorld `Mods/` folder. The
Workshop copy is preferred because it is what players run against. Both references are skipped
silently when the DLL is absent, so a build still succeeds without them until code uses their
types. **CI has neither DLL**; the first C# use of a VFEP or VEF type must come with a CI source
for them (see `TODOs.md`).

Reference source checkouts: `../VanillaExpanded/VanillaFactionsExpanded-Pirates/` (its
`CLAUDE.md` maps the warcasket subsystem: `WarcasketDef`, `Apparel_Warcasket`,
`Building_WarcasketFoundry`, the entombing jobs) and `../VanillaExpanded/VanillaExpandedFramework/`.
Both are upstream repos, not ours: read from them, never commit into them from here.

### Deployment

The repo lives outside the Mods folder; every local Release build redeploys automatically and
atomically. The deploy folder name follows the project name (`Mods/ShipcrackerWarcasket`).

- **One manifest, one place:** the `_ModFiles` ItemGroup in the `StageMod` target of
  `Source/1.6/ShipcrackerWarcasket.csproj`; see that target's comments for how it globs and what
  it excludes. It is generic over folders, so a new `1.7/` or `Sounds/` needs no build change; only
  a brand-new *file type* does. Local deploy and CI release both call it, so they can't drift.
- **Stop hook (`.claude/hooks/sync-mod.sh`):** rebuilds+redeploys after a turn only when
  mod-relevant files changed, logs to `$TMPDIR/scwc-build.log`, warns on failure. Wired via a
  `Stop` hook in `.claude/settings.local.json`. It is local-only (see below); if it is ever
  promoted to committed config, move the helper somewhere version-controlled.

**`.claude/` is only partly gitignored.** `.gitignore` carries `.claude/*` followed by
`!.claude/skills/`, so the skills are tracked and shared while hooks and settings are local
per-machine. Editing a skill is a committed, team-visible change and must keep in step with
whatever it automates (e.g. `/release` encodes the CHANGELOG layout).

## Project Structure

```
About/           - Mod metadata (About.xml; Preview.png and PublishedFileId.txt once published)
Common/          - Version-independent assets (Languages, Textures)
1.6/             - RimWorld 1.6 specific content
  Assemblies/    - Compiled DLLs (build output, gitignored)
  Defs/          - XML definitions (ThingDefs, etc.)
  Patches/       - XML patches to modify base game/other mods
  Mods/<Name>/   - Optional-mod/DLC compat roots (version-specific), gated in LoadFolders.xml
Mods/<Name>/     - Optional-mod/DLC compat roots (version-independent art)
Source/1.6/      - C# source code targeting net472
LoadFolders.xml  - Tells RimWorld which folders to load per game version
CHANGELOG.md     - Keep a Changelog format; load-bearing for releases (see below)
TODOs.md         - Scoping notes for the feature work that has not landed yet
```

## RimWorld Modding Context

- Target framework: .NET Framework 4.7.2
- References RimWorld assemblies via cross-platform paths in .csproj
- Uses `Verse` namespace for core modding APIs
- XML Defs define game objects; Patches modify existing Defs via XPath
- Harmony is referenced (`Lib.Harmony`, compile-only) and bootstrapped in `ModInit.cs`; the
  runtime DLL comes from the `brrainz.harmony` mod dependency declared in About.xml.

### Conventions

- All defs use the `SCWC_` prefix. **One def per file** in every `Defs/` `.xml`, named after the
  def with the prefix stripped. Defs load recursively and the deploy manifest globs, so new
  files/subfolders need no build change.
- Warcasket parts are `VFEPirates.WarcasketDef`s parented on VFEP's abstract bases
  (`VFEP_WarcasketArmorBase`, `VFEP_WarcasketShoulderPadBase`, `VFEP_WarcasketHelmetBase`), so
  the foundry, entombing flow and removal surgery pick them up without C#. Check VFEP's XML
  before reimplementing anything a base already provides.
- **C#:** root namespace `ShipcrackerWarcasket`; patch classes use a `.Patches` suffix to avoid
  RimWorld type-name conflicts. Log with the `[Shipcracker Warcasket]` prefix.
- **Patch timing is the load-bearing hazard of this mod.** `PatchAll()` runs from a
  `[StaticConstructorOnStartup]` (`ModInit.cs`), *not* a `Mod` subclass constructor, on purpose:
  Mod constructors run before defs load, and applying a detour JIT-compiles the target and runs
  its declaring type's static ctor. Our targets will often be VFEP's own methods, and a VFEP type
  whose cctor resolves defs would be permanently broken by an early patch (the BetterTradersGuild
  v1.1.0 CWTL incident). Static ctors on startup run after defs load, so foreign targets are safe
  there. If a `Mod` subclass is added for settings, leave `PatchAll` where it is.
- **No em dashes in player-facing text** (def labels/descriptions, `Keyed/`, `About.xml`);
  reflow the sentence instead. This file, code comments and def comments are unaffected.

## Localization and Optional-Content Gating

- `MayRequire`/`MayRequireAnyOf` work on def root nodes and list items, but the DefInjected loader
  ignores XML attributes entirely. A DefInjected entry for a gated def placed in the main tree
  loads unconditionally and logs a "found no def named ..." startup error whenever the gating
  mod/DLC is absent.
- The fix is a folder gate: ship the gated content from a compat load root, loaded via an
  `IfModActive` entry in `LoadFolders.xml` (the Odyssey pair is drafted there, commented out).
  Two flavours, mirroring the ungated roots: `1.6/Mods/<Mod Name>/` for version-specific content
  (Defs, and the DefInjected targeting them) and root-level `Mods/<Mod Name>/` for
  version-independent content (art). The def's `MayRequire` becomes redundant and should be
  dropped when it moves. Gate on the package id (`ludeon.rimworld.odyssey`), never
  `PatchOperationFindMod` (matches by display name).
- Compat roots must sit BESIDE the well-known folders, never inside them: anything under
  `1.6/Defs/**` or `1.6/Languages/**` loads unconditionally at any depth.
- The game gates a DefInjected entry by the load root that CONTAINS it, never by the def it
  targets. The reverse mistake also bites: a main-tree def's translation placed in a compat root
  silently vanishes when the gate is closed.
- A compat root's language files must never reuse a main-tree file's language-relative path
  (`DefInjected/<Type>/<File>.xml`, `Keyed/<File>.xml`): the game dedups language files per mod by
  that path and silently skips one whole file, in an enumeration order that is not LoadFolders
  order. Suffix compat-root filenames with the gate's name (`Apparel_Odyssey.xml`).
- If the mod grows translations, adopt the ecosystem tooling: the shared `rimworld-l10n` toolkit
  is consumed as a git submodule at `l10n/` (`git submodule add ../rimworld-l10n.git l10n`, pinned
  to a release tag), with thin per-repo config shims in `Scripts/` (`check-translations.py`,
  `refresh-translation-expectations.py`, `integration-smoke-test.py`; copy the `SHIM_TEMPLATE.py`
  files from the toolkit's `checker/`, `refresh/`, `smoke/` dirs), an `expected-injections.json`
  sidecar generated by the toolkit's L10nProbe dev mod, and the `translate` skill (copy the shape
  from UniqueMeleeWeapons). The `.steamworkshop/Description/<Language>.txt` convention (Workshop
  title + BBCode description per language) also comes with it.

**Releases:** run the `/release` skill, or by hand: add the version's `## [X.Y.Z]` section to
`CHANGELOG.md`, bump `About/About.xml` `<modVersion>` and `Source/1.6/Properties/AssemblyInfo.cs`,
then push a `v*.*.*` tag. The GitHub Actions workflow (`.github/workflows/release.yml`) builds,
stages via `StageMod`, lifts the tag's CHANGELOG section into the release body, and **fails the
release if that section is missing**.

## Debugging

1. **Dev Mode:** Settings > Dev Mode > Logging.
2. **Log:** `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`
   (WSL: `/mnt/c/Users/*/AppData/LocalLow/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`;
   Linux: `~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`).
3. **Logging convention:** `Log.Message("[Shipcracker Warcasket] ...")`; grep the prefix to
   isolate our output. VFEP's own messages carry no prefix; search by class name.
4. **Inspect the API:** `monodis` for signatures, `ilspycmd -t "Namespace.ClassName"` for method
   bodies, against the local install's `Assembly-CSharp.dll` (source of truth over the Krafs ref
   package) or the resolved `VFEPirates.dll` / `VEF.dll`. The `rimworld-logs` skill covers both.
5. **In-game test list:** VFE Pirates (Workshop 2723801948) and Vanilla Expanded Framework must
   both be active and load before this mod; the local deploy shows up as `ShipcrackerWarcasket`
   in the mod list.
