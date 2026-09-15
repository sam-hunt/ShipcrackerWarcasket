# TODOs

Scoping notes for the feature work. Infrastructure, the three defs, the art, the first stat
and cost tuning pass (2026-09-13), the SOS2/Universum parity patch and the l10n toolchain have
landed; everything below is still open.

- Check whether undersuit is rendered unconditionally in game and whether it's necessary in the mod icon or not since our artist delivered both with/without versions
- Warcasket abilities (what the Shipcracker does beyond stats); next session
- Real descriptions and `shortDescription` lore text; translation passes wait for the release
  gate and for this text to be final
- Investigate VGE2 astrorig functionality, we want something similar that's built-in to our torso piece.
- Investigate whether we can use either of VFEP's aerial/shock abilities with huge demolish damage on power-jump landing
- Investigate whether we can switch jump ability while in space/orbit (requires odyssey) to a new one which has infinite range (no gravity), requires LoS
- Investigate whether we can switch the jump ability to use VGE2's astrofuel instead of chemfuel if the mod is active
- Ensure comments in shipped XML are lean to reduce bundle bloat

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
- Textures: the commissioned art landed 2026-09-10 under
  `Textures/Things/Pawn/Warcasketlike/WarcasketShipcracker/` (root `Textures/`, like every mod in
  the family; there is no `Common/` root) and all three defs point at it. Still open: whether the
  set should be colourable (`CompColorable` mask conventions), and whether any part ends up
  Odyssey-gated, in which case its art moves to `Mods/Odyssey/Textures/`. Verify in-game that
  the worn graphics line up with VFEP's pawn offsets.
- Does anything actually need C#? A pure-XML set may need no DLL at all. If no Harmony patch
  lands, remove the Harmony dependency from `About.xml` and the csproj before first release.

## Infrastructure follow-ups

- **CI cannot compile against VFEP/VEF yet.** The csproj references `VFEPirates.dll` and
  `VEF.dll` compile-only and skips them when absent; the release workflow runs on a bare Ubuntu
  runner. The first C# use of a VFEP/VEF type needs a source in CI: either commit the two DLLs
  under `Source/refs/` (common practice, but pins a version) or fetch the Workshop copies in the
  workflow (SteamCMD, anonymous login works for Workshop content).
- **Vanilla Gravship Expanded is not installed locally** (neither Workshop nor `Mods/`), so the
  `1.6/Mods/VanillaGravshipExpanded` compat root (vacuum-resistance parity patch) is not on the
  smoke list and has never been booted. Install VGE1 (and VGE2 for the astrofuel idea) and add
  them to `Scripts/integration-smoke-test.py` before release.
- Preview image (`About/Preview.png`) and mod icon (`About/ModIcon.png`) before publishing;
  `About/PublishedFileId.txt` is written by the Workshop uploader on first publish.
- Consider `.steamworkshop/Description/English.txt` once there is a Workshop page; the
  release skill's Workshop-description step is still to be ported from a sibling then.
