# Content tree guidance (1.6/)

Defs, patches, translations and the gated compat roots. Each def's and patch's header carries
its own numbers and rationale (stripped at deploy, so be generous there); the rules below are
the cross-file ones.

## Defs and patches

- Warcasket parts are `VFEPirates.WarcasketDef`s parented on VFEP's abstract bases
  (`VFEP_WarcasketArmorBase`, `VFEP_WarcasketShoulderPadBase`, `VFEP_WarcasketHelmetBase`),
  so the foundry, entombing flow and removal surgery pick them up without C#. Read VFEP's XML
  before reimplementing anything a base already provides.
- Every VFEP base already carries a `VEF.Apparels.ApparelExtension`, and VEF merges duplicate
  extensions at resolve time, keeping the highest `priority` and dropping fields its `Merge`
  does not copy. Our entries carry `priority` 1; any new `ApparelExtension` field, from any
  load root, goes on that entry (rationale in the armor def's header).

## Optional content gating

- `MayRequire` works on def roots and list items, but the DefInjected loader ignores
  attributes entirely: a translation for a gated def placed in the main tree loads
  unconditionally and logs a "found no def named" startup error whenever the gating mod is
  absent. Gated content therefore ships from a compat load root behind an `IfModActive` entry
  in `LoadFolders.xml`: `1.6/Mods/<Name>/` for version-specific content (Defs, Patches and
  the DefInjected targeting them), root-level `Mods/<Name>/` for art. Gate on the package id,
  never `PatchOperationFindMod` (display names), unless the patch mirrors an upstream patch and
  copies its gate so both fire together. A def that moves into a root drops its `MayRequire`.
- Compat roots sit BESIDE the well-known folders, never inside them: anything under
  `1.6/Defs/**` or `1.6/Languages/**` loads unconditionally at any depth.
- A compat art variant goes at the main tree's exact texture path inside the gated root, not
  under a new name behind a patch: a later load folder wins a shared relative path, so gated
  roots stay below `/` in `LoadFolders.xml`.

## Languages

- English is served by the def XML and the single `Languages/English/Keyed/` file; every other
  language derives from it via `/translate`, with the English source quoted in an `EN:` comment
  above each entry. The expected key set is `Scripts/expected-injections.json`.
- `WarcasketDef` has no database of its own, so its DefInjected folder is `ThingDef/`; a
  `WarcasketDef/` folder would never load.
- The game gates a DefInjected entry by the load root that CONTAINS it, never by the def it
  targets. A key a compat root's patch adds to a main-tree def (the VGE helmet's oxygen
  `chargeNoun`) exists only behind that gate, so its translation lives in the gate's root; a
  main-tree key placed in a compat root silently vanishes when the gate is closed.
- A compat root must never reuse a main-tree file's language-relative path
  (`DefInjected/<Type>/<File>.xml`, `Keyed/<File>.xml`): the game dedups language files per
  mod by that path and silently drops one whole file. Suffix compat-root filenames with the
  gate's name (`Apparel_VanillaGravshipExpanded.xml`).
