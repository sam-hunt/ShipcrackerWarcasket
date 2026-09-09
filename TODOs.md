# TODOs

Scoping notes for the feature work. This session set up infrastructure only; nothing below has
landed.

## Scope

- One warcasket set for VFE Pirates: armor + shoulder pads + helmet, as three
  `VFEPirates.WarcasketDef`s parented on `VFEP_WarcasketArmorBase`,
  `VFEP_WarcasketShoulderPadBase`, `VFEP_WarcasketHelmetBase` (see VFEP
  `1.6/Defs/ThingDefs_Misc/Apparel_Various.xml` and `Apparel_Headgear.xml`). `WarcasketDef` adds
  only `shortDescription`, `isArmor`, `isShoulderPads`, `isHelmet` over `ThingDef`.
- Tuned for Odyssey's end-game threats, but Odyssey must stay optional. Decide what, if
  anything, is Odyssey-only (e.g. a pawnkind/apparel-tag hook into VFEP's `VFEP_Salvager_*`
  Odyssey pawnkinds, which VFEP adds via `1.6/Patches/Odyssey.xml`), and ship that from the
  `Mods/Odyssey` + `1.6/Mods/Odyssey` compat roots drafted in `LoadFolders.xml`.
- Odyssey's `PatchOperationFindMod` in VFEP matches by display name; our gate uses the package
  id `ludeon.rimworld.odyssey` via `IfModActive`.

## Open questions

- Acquisition: foundry recipe/research only, or also raid/trader presence? VFEP tags its own
  parts `WarcasketVeteran` for pawnkind generation; check `PawnKinds_Junkers.xml` before reusing
  the tag, since reusing it puts our set on every Junker veteran.
- Research gating: which VFEP research project(s) to parent on (`ResearchProjects_Various.xml`).
- Textures: match VFEP's warcasket texture layout (`Things/Pawn/Warcasketlike/<Set>/...`, north/
  east/south variants, `CompColorable` mask conventions). Art lives under `Common/Textures/`
  (version-independent), or `Mods/Odyssey/Textures/` if the part itself is Odyssey-gated.
- Does anything actually need C#? A pure-XML set may need no DLL at all. If no Harmony patch
  lands, remove the Harmony dependency from `About.xml` and the csproj before first release.

## Infrastructure follow-ups

- **CI cannot compile against VFEP/VEF yet.** The csproj references `VFEPirates.dll` and
  `VEF.dll` compile-only and skips them when absent; the release workflow runs on a bare Ubuntu
  runner. The first C# use of a VFEP/VEF type needs a source in CI: either commit the two DLLs
  under `Source/refs/` (common practice, but pins a version) or fetch the Workshop copies in the
  workflow (SteamCMD, anonymous login works for Workshop content).
- Preview image (`About/Preview.png`) and mod icon (`About/ModIcon.png`) before publishing;
  `About/PublishedFileId.txt` is written by the Workshop uploader on first publish.
- Adopt the `l10n/` submodule + `Scripts/` shims + `translate` skill when player-facing strings
  stabilise (CLAUDE.md > Localization).
- Consider `.steamworkshop/Description/English.txt` once there is a Workshop page.
- Startup smoke test (`Scripts/integration-smoke-test.py` shim) once the l10n toolkit is in.
