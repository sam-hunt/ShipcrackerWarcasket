# CLAUDE.md

Guidance for Claude Code (claude.ai/code) when working in this repository.

## Project Overview

**Shipcracker Warcasket** is a RimWorld 1.6 mod adding a single new warcasket apparel set
(armor, shoulder pads, helmet) for Vanilla Factions Expanded - Pirates (VFE Pirates), tuned for
the Odyssey DLC's end-game content. VFE Pirates is a hard dependency; Odyssey is optional and
Odyssey-only content must load only when the DLC is active. Requires Harmony (bootstrapped in
`ModInit.cs`; the patch classes live in `Source/1.6/Patches/`).

**Key technologies:** C# (.NET Framework 4.7.2), Harmony, RimWorld modding API, XML defs.

**Def prefix:** `SCWC_`. Stat/cost tuning has started (each def's header carries its
rationale); `TODOs.md` holds the scoping notes for what has not landed.

### Where documentation lives

**This file holds only cross-cutting rules and rationale.** Per-item values, tuning numbers and
decompile-verified call paths live in the header comment of the file they describe. When adding
or changing something, put the *why* there and only add a line here if it constrains work in
other files. Do not restate def values or call paths here; they drift.

**Comments describe the present, git describes the past.** A header or code comment explains the
current state where the code does not make it obvious: engine facts it relies on, what a number is
balanced against, cross-file coupling. It carries no dates, no previous values, no record of what
was tried and dropped, and no "seen in game" notes; that history belongs in the commit message
and diff. Def comments ship in the bundle, so keep them lean.

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
types. CI fetches both DLLs from the Workshop with SteamCMD in the release workflow and injects
them via the `VEF_PATH` / `VFEP_PATH` environment variables.

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
Textures/        - Art (version-independent, loaded via the "/" root; no Common/ root)
1.6/             - RimWorld 1.6 specific content
  Assemblies/    - Compiled DLLs (build output, gitignored)
  Defs/          - XML definitions (ThingDefs, etc.)
  Patches/       - XML patches to modify base game/other mods
  Mods/<Name>/   - Optional-mod/DLC compat roots (version-specific), gated in LoadFolders.xml
Mods/<Name>/     - Optional-mod/DLC compat roots (version-independent art)
Source/1.6/      - C# source code targeting net472
Scripts/         - l10n config shims + the expected-injections.json sidecar (not shipped)
l10n/            - rimworld-l10n toolkit, git submodule pinned to a release tag (not shipped)
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
- Every VFEP warcasket base already carries a `VEF.Apparels.ApparelExtension`, and VEF merges
  duplicate extensions at resolve time, keeping the highest `priority` and dropping fields its
  `Merge` does not copy. Our extension entries carry `priority` 1 so ours survive; any new
  `ApparelExtension` field, from any load root, must go on that entry (rationale in the armor
  def's header).
- **C#:** root namespace `ShipcrackerWarcasket`; patch classes live in `Source/1.6/Patches/`
  under the `.Patches` namespace suffix to avoid RimWorld type-name conflicts. Log with the
  `[Shipcracker Warcasket]` prefix.
- **Drawing on the wearer goes through the pawn render tree, not draw hooks.** Extra worn
  graphics are `apparel.renderNodeProperties` entries on the def (additive to the default
  worn-graphic node; the armor's thruster glow is the model). The zoomed-out pawn cache bakes
  the tree once with DrawMeshNow, which ignores property blocks: anything that changes per frame
  must skip the cache bake or bake a fixed state and mark the wearer's frames dirty at each
  transition (`GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty`); the glow's on/off bake is
  the precedent. That bake also draws in request order with no render-queue sorting, so a
  transparent node must sit after the opaque node it overlays in the tree (the glow's
  `AddChildren` postfix reorders it). Do not force a live render instead (a `disableCache`
  prefix on `PawnRenderer`): that check runs for every humanlike pawn drawn, every frame.
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
  `PatchOperationFindMod` (matches by display name). The one deliberate exception is a patch
  that mirrors a VFEP patch: `1.6/Patches/SOS2Patch.xml` copies VFEP's own display-name gate so
  the Shipcracker and VFEP's sets flip to EVA-rated under exactly the same condition.
- Compat roots must sit BESIDE the well-known folders, never inside them: anything under
  `1.6/Defs/**` or `1.6/Languages/**` loads unconditionally at any depth.
- The game gates a DefInjected entry by the load root that CONTAINS it, never by the def it
  targets. The reverse mistake also bites: a main-tree def's translation placed in a compat root
  silently vanishes when the gate is closed.
- A compat root's language files must never reuse a main-tree file's language-relative path
  (`DefInjected/<Type>/<File>.xml`, `Keyed/<File>.xml`): the game dedups language files per mod by
  that path and silently skips one whole file, in an enumeration order that is not LoadFolders
  order. Suffix compat-root filenames with the gate's name (`Apparel_Odyssey.xml`).

## Localization Toolchain

English (the def XML's `label`, `description` and VFEP's `shortDescription`) is the source of
truth; there is no Keyed surface and no English `Languages/` tree. Other languages derive from it
via the `/translate` skill (`.claude/skills/translate/SKILL.md`: this mod's surface, grounding
domain and per-language glossary; the family-wide process lives in the `l10n/` submodule) and
are validated deterministically by `python3 Scripts/check-translations.py` (also a CI release
gate). The DefInjected expected set is the checked-in sidecar `Scripts/expected-injections.json`:
a dump of every injection point the *live* game sees for this mod, produced by
`Scripts/refresh-translation-expectations.py` driving the L10nProbe dev mod (source at
`l10n/probe/`; build/deploy it only from the canonical `~/dev/rimworld-l10n` checkout; this mod
is ticked in the probe's settings) through the game's own walker. The checker refuses to run
against stale expectations, so new content forces a regen; the release skill regenerates every
release. Translation passes are deferred to the release gate and must wait for the English text
to be final. The public language roster lives in CONTRIBUTING.md.

- **Shared l10n toolkit (`l10n/` submodule):** the checker/refresh/smoke engines, per-language
  mechanics references, cross-language lessons and Workshop conventions come from the
  `rimworld-l10n` repo, consumed as a git submodule pinned to a semver release tag (`git submodule
  status` names it; if `l10n/` is empty, run `git submodule update --init`). `Scripts/*.py` are
  thin per-repo config shims over its engines; each shim's comments carry this repo's rationale
  (the VFEP dependency chain, why Odyssey is the only DLC pinned, why SOS2 and VGE1 are on the
  smoke list).
  Never edit `l10n/` in place here: mod-independent learnings go upstream in the canonical
  checkout; mod-specific ones go in the skill's glossary. The pin moves only at release, at the
  start of a translation pass, or when a new major lands (`l10n/tools/bump-consumer.sh`), never
  per upstream commit.
- **Def type folder:** the defs are `VFEPirates.WarcasketDef`, a `ThingDef` subclass without its
  own database, so the game dumps and injects them under `ThingDef`; the checker maps the element
  tag via `DEF_TYPE_ALIASES`. A DefInjected folder named `WarcasketDef` would never load.
- **Startup smoke test (pre-release):** `python3 Scripts/integration-smoke-test.py` (game closed)
  boots the deployed mod on a pinned list (VFEP chain + Odyssey + the two optional integrations:
  Save Our Ship 2 with Vehicle Framework so `SOS2Patch.xml` actually runs, and Vanilla Gravship
  Expanded so the `1.6/Mods/VanillaGravshipExpanded` root opens), then classifies logged errors
  by origin and fails on anything attributed to this mod or a VFEP/VEF/SOS2/VGE seam.
  Wired into the release skill. Sibling-mod "dump WILL fail" warnings at launch are expected: the
  probe is ticked for every family mod but each boot loads only its own list.
- **`.steamworkshop/`** does not exist yet; adopt the toolkit's `Description/<Language>.txt`
  convention when there is a Workshop page.

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
