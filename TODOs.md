# TODOs

- test new modicon with accent
- check whether aerial/shock warcasket abilities complement shipcracker
- apparel descriptions etc
- check the thruster glow in game (landed 2026-09-18: opacity curve in `1.6/Defs/AbilityDefs/BreachJump.xml`, tint and shader on the render node in the armor def, purple tint in the VGE root's astrofuel patch; MoteGlow is the additive alternative if it should brighten rather than overlay), at both close and Middle zoom and in the flight
- check the breach jump / breach burn gizmo icons in game, with and without VGE (purple variants shadow the orange ones from `Mods/VanillaGravshipExpanded/Textures/`)
- Real descriptions and `shortDescription` lore text; translation passes wait for the release gate and for this text to be final. The Royalty "shipcracker" backstory is the brief (see `Docs/Research/WARCASKET_ABILITIES.md`).
- profile the postfix enabling zoomed-out thruster-glow recoloring on our old heavily modded save

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

## Infrastructure follow-ups

- **Confirm the CI Workshop fetch actually works.** The release workflow now fetches VEF and
  VFEP from the Workshop with SteamCMD (anonymous login) and injects them via `VEF_PATH` /
  `VFEP_PATH`, but anonymous download of these two items has never been exercised in CI; the
  first tagged release should confirm it succeeds.
- Preview image (`About/Preview.png`) and mod icon (`About/ModIcon.png`) before publishing;
  `About/PublishedFileId.txt` is written by the Workshop uploader on first publish.
- Consider `.steamworkshop/Description/English.txt` once there is a Workshop page; the
  release skill's Workshop-description step is still to be ported from a sibling then.
