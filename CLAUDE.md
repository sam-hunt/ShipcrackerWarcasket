# CLAUDE.md

Guidance for Claude Code when working in this repository. This file holds only what cuts
across the whole repo. Each domain folder carries its own `CLAUDE.md`, loaded when you work
there, and every def, patch, script and class carries its rationale in its own header.

## Project

**Shipcracker Warcasket** is a RimWorld 1.6/1.7 mod adding one warcasket apparel set (armor,
shoulder pads, helmet) with a thruster-driven Breach Jump to Vanilla Factions Expanded -
Pirates (VFEP, hard dependency). Odyssey is optional and its content loads only when the DLC
is active. C# (net472) with Harmony, XML defs under the `SCWC_` prefix. `TODOs.md` scopes the
work that has not landed; `Docs/Research/` holds the VFEP stat and ability surveys.

Reference checkouts of the upstream repos sit beside this one under `../VanillaExpanded/`
(VFEP's `CLAUDE.md` maps its warcasket subsystem). Read from them, never commit into them.

## Documentation rules

- **Rationale lives in the file it explains.** Per-item values, tuning and decompile-verified
  call paths go in that def's or class's header comment; a `CLAUDE.md` gets a line only when
  the rule constrains work in other files. Never restate def values here; they drift.
- **Comments describe the present, git describes the past.** No dates, previous values,
  dropped attempts or "seen in game" notes in any comment; that history is the commit message.
  Shipped XML is comment-stripped at deploy (`StageMod`), so a def header costs nothing.
- **No em dashes in player-facing text** (labels, descriptions, `Keyed/`, `About.xml`,
  `.steamworkshop/`); reflow the sentence. Code, comments and this file are unaffected.

## Commands

```bash
dotnet build ShipcrackerWarcasket.sln -c Release     # build + atomic redeploy into the Mods folder
dotnet test Tests/1.6/ShipcrackerWarcasket.Tests.csproj
python3 Scripts/check-translations.py                # l10n gate (also CI)
python3 Scripts/integration-smoke-test.py            # boots the game; game must be closed
```

Release builds compile against the Krafs 1.6 reference package and deploy to the RimWorld
install the csproj auto-detects (`RIMWORLD_PATH` or `-p:RimWorldPath=` overrides it; on WSL
point it at the Windows install). Debug builds never deploy. The 1.7 runtime loads the one
`1.6/` tree unchanged. How the dependency DLLs (VFEPirates, VEF) and the test suite's runtime
assemblies are resolved is documented in the two csproj files.

**Warnings are build errors** in both csproj files, locally, in the Stop hook and in CI;
nothing else surfaces a warning to an agent. Fix the code, not the severity (`.editorconfig`
pins the severities and explains the RCS1146 exception).

**Releases:** `/release`. `CHANGELOG.md` is Keep a Changelog; changes are recorded under
`## [Unreleased]` as they land, and the release workflow fails without the tag's section.
Candidates are `X.Y.Z-rc.N` tags: prerelease, no CHANGELOG section, no Workshop upload.

**Debugging:** the `rimworld-logs` skill (log locations, 1.6 vs 1.7 log formats, `monodis` and
`ilspycmd` against the local install). Our log lines carry the `[Shipcracker Warcasket]` prefix.

## Layout

```
About/            Mod metadata
Textures/         Art, loaded via the "/" root
1.6/              Defs, Patches, Languages, Mods/<Name>/ compat roots   -> 1.6/CLAUDE.md
Mods/<Name>/      Version-independent compat art                       -> Mods/CLAUDE.md
Source/1.6/       C# (net472)                                           -> Source/1.6/CLAUDE.md
Tests/1.6/        Headless xUnit suite, not shipped                     -> Tests/1.6/CLAUDE.md
Scripts/          l10n checker/refresh/smoke shims + the sidecar        -> Scripts/CLAUDE.md
l10n/             rimworld-l10n toolkit, submodule pinned to a release tag
.steamworkshop/   Workshop title and description per language (its README.md has the rules)
LoadFolders.xml   Load roots per game version; the compat gates live here
```

## Repo-wide conventions

- Defs use the `SCWC_` prefix, **one def per file**, named after the def minus the prefix.
  Defs load recursively and the deploy manifest globs, so new files and folders need no build
  change; only a new *file type* does. The `StageMod` target in the main csproj is the one
  manifest, shared by local deploy and CI, so they cannot drift.
- **`.claude/` is partly tracked:** `skills/`, `hooks/` and `settings.json` are committed and
  shared; `settings.local.json` is personal. Editing a skill is a team-visible change that must
  keep in step with what it automates (`/release` encodes the CHANGELOG layout and the
  version scheme; `release.yml` matches it).
- The Stop hook (`.claude/hooks/sync-mod.sh`) rebuilds and redeploys after any turn that
  changed mod files and feeds build errors back to the agent. It is byte-identical across the
  mod family: change the template and copy it verbatim, never edit it per repo.
- `l10n/` is upstream: never edit it in place. Mod-independent l10n learnings go to the
  canonical `~/dev/rimworld-l10n` checkout; mod-specific ones to the translate skill's
  glossary. The pin moves only at release, at the start of a translation pass or on a new
  upstream major (`l10n/tools/bump-consumer.sh`), never per upstream commit.
- English is the localization source of truth (the def XML's fields plus the one English
  Keyed file). Other languages come from `/translate` and are validated by the checker.
  Translation passes wait for the English to be final and run at the release gate.
