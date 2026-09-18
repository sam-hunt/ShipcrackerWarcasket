# TODOs

- new modicon with accent
- tune the VGE1 purple flight exhaust in game (landed 2026-09-18 on VGE's Astrospark texture:
  `1.6/Mods/VanillaGravshipExpanded/Defs/`; knobs are the flecks' drawSize 3 and the glow/flash
  tints; Core's grey `BlastExtinguisher` tinted purple is the reserve if the sticker edge shows)
- gizmo swap to purple flame variant with VGE1
- check whether aerial/shock warcasket abilities complement shipcracker
- apparel descriptions etc
- test with vge1/2
- warming-up thruster overlay during cast with orange->purple color switch with VGE1
- **In-game test of the Breach Jump** (fuel gizmo, save/load mid-flight; the landing shockwave ring is checked).
- Replace the placeholder ability icons (VFEP's Power Jump on a planet, Blast Off in space) with
  the artist's textures once the abilities are tested and locked in; both paths are in
  `1.6/Defs/AbilityDefs/BreachJump.xml`.
- Real descriptions and `shortDescription` lore text; translation passes wait for the release
  gate and for this text to be final. The Royalty "shipcracker" backstory is the brief (see
  `WARCASKET_ABILITIES_RESEARCH.md`).
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

## Infrastructure follow-ups

- **Confirm the CI Workshop fetch actually works.** The release workflow now fetches VEF and
  VFEP from the Workshop with SteamCMD (anonymous login) and injects them via `VEF_PATH` /
  `VFEP_PATH`, but anonymous download of these two items has never been exercised in CI; the
  first tagged release should confirm it succeeds.
- Preview image (`About/Preview.png`) and mod icon (`About/ModIcon.png`) before publishing;
  `About/PublishedFileId.txt` is written by the Workshop uploader on first publish.
- Consider `.steamworkshop/Description/English.txt` once there is a Workshop page; the
  release skill's Workshop-description step is still to be ported from a sibling then.
