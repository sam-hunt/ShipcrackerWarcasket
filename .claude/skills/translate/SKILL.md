---
name: translate
description: Generate, update, or audit mod localization (DefInjected only) for a target language, grounded in Vanilla Factions Expanded - Pirates warcasket terminology plus vanilla Core/Odyssey apparel terminology for Shipcracker Warcasket's single warcasket set. Use when asked to add a language, update translations, or check translation freshness.
argument-hint: "[language, e.g. German | update | check]"
---

# Translate

Produce or refresh localization files for Shipcracker Warcasket. English is
the source of truth; every other language derives from it.

**The family-wide process lives in the `l10n/` submodule, load these first,
and only these** (progressive disclosure; if `l10n/` is empty, run
`git submodule update --init`):

- `l10n/process.md`, non-negotiables, file/format conventions, terminology
  grounding method, and the generation / update / audit workflows. This is
  the workflow authority; follow it step by step.
- `l10n/languages/<Language>.md`, the target language's engine mechanics,
  style rules, and vanilla-grounded common vocabulary. Read ONLY the target
  language's file.
- `glossary/<Language>.md` (beside this file), this mod's own coined-term
  table for the target language. Read it in the same pass.
- `l10n/lessons.md`, cross-language lessons; read when generating a new
  language, skim otherwise. Its free-prose register lesson applies with
  full force here: the three descriptions are lore paragraphs with no
  vanilla sentence to mirror, so a separate register read of every
  description and Workshop sentence is part of every pass.
- `l10n/workshop.md`, the `.steamworkshop/` conventions, whenever the pass
  touches the Workshop description (every initial generation does).

**Where learnings land:** mod-independent findings (engine mechanics, a
language's grammar rule, corpus style facts) go in the `l10n/` submodule,
edit the canonical checkout at `~/dev/rimworld-l10n`, commit and tag there.
Mod-specific findings (coined terms, phrasing decisions) go in
`glossary/<Language>.md`.

**Before any pass, bump the pin:** run `l10n/tools/bump-consumer.sh` (fetches
upstream's release tags, checks out the latest, commits the pointer as `chore:
Bump l10n submodule vOLD -> vNEW`; no-op when already current). This is one of
the three moments a pin moves (release, pass start, new upstream major), never
per upstream commit. If it reports a MAJOR bump, read the upstream release
notes for the shim or flow edit this repo owes before continuing.

## This mod's translation surface

- **No Keyed strings and no English Languages tree at all.** English is
  served entirely by the def XML's own fields; there is nothing under
  `1.6/Languages/English/`. The translation surface is DefInjected only,
  plus the Workshop page under `.steamworkshop/`.
- **Enumerate the key set from `Scripts/expected-injections.json`, never
  from a Languages folder or by scanning `1.6/Defs/`.** The sidecar is a
  dump of what the live game walks; regenerate it (game closed) with
  `python3 Scripts/refresh-translation-expectations.py` whenever the
  checker reports it stale. Take the English source text for each
  `<!-- EN: -->` comment from the sidecar's `english` field.
- **Def type folders** (the game rolls a def type without its own database
  into its base; the checker maps these via `DEF_TYPE_ALIASES` in
  `Scripts/check-translations.py`):
  - `DefInjected/ThingDef/` for the three `VFEPirates.WarcasketDef`s
    (`SCWC_Warcasket_Shipcracker`, `SCWC_WarcasketShoulders_Shipcracker`,
    `SCWC_WarcasketHelmet_Shipcracker`): `label`, `description`, and
    `shortDescription` (a VFEP field shown in the foundry's part picker;
    translate it like any other). A `WarcasketDef` folder would never load.
  - `DefInjected/VEF.Abilities.AbilityDef/` for `SCWC_BreachJump`: `label`,
    `description`, and the two `BreachJumpExtension` strings
    (`vacuumLabel`, `vacuumDescription`) the DefInjected walker reaches
    through `modExtensions`. VEF's ability def is not vanilla's
    `AbilityDef`, so the folder is namespace-qualified.
  - `DefInjected/StatDef/` for `SCWC_BreachJumpRange` and
    `SCWC_BreachPower`; `DefInjected/DamageDef/` for `SCWC_Breach`
    (`label` and the `{0}` `deathMessage`).
  - Never translate or place a non-`required` sidecar entry (texture paths
    such as `shieldTexPath`) in any language file.
- **The three descriptions share their second and third paragraphs
  verbatim** (the Imperial-pattern lore and the boarding paragraph); only
  the first paragraph differs per part. Keep the shared paragraphs
  byte-identical across the three defs in every language, and keep each
  def's `shortDescription` identical to its description's first paragraph,
  as the English does. Paragraph breaks are the literal two-character
  `\n` sequences the def XML uses.
- **The armor's `chargeNoun` ("fuel unit") is shown only through vanilla's
  `{CHARGENOUN_plural}` and `{lookup: ...}` slots.** A rendering the language
  worker's fallback would mangle (a multi-word noun, or German's appended
  `s`) gets its forms from `1.6/Languages/<Language>/WordInfo/plural.txt`
  (`singular;plural`, and `case.txt` for Russian's genitive lookups): the
  game assembles each lookup table from every language folder, mods
  included, and resolves the file name lowercased, so keep the file names
  lowercase. French's worker never reads the table, so French takes a
  single-word noun instead. The checker ignores these files; the glossary
  row for the noun names the one it relies on.
- **One compat root carries strings: Vanilla Gravship Expanded.** Its
  `HelmetOxygen.xml` patch adds an oxygen comp to the main-tree helmet, and
  the comp's `chargeNoun` ("oxygen u³") is a key that exists only while VGE
  is active. The sidecar marks it with `"worlds": ["vge"]`; its translation
  lives ONLY under `1.6/Mods/VanillaGravshipExpanded/Languages/<Language>/
  DefInjected/ThingDef/Apparel_VanillaGravshipExpanded.xml` (gate-suffixed
  filename, see CLAUDE.md's Localization and Optional-Content Gating
  section), never the main tree, where it would be a startup error for
  players without VGE. Every other key is in all worlds and lands in the
  main `1.6/Languages/<Language>/` tree. Ground "oxygen" against Core/Odyssey.
- **Workshop page:** `.steamworkshop/Description/<Language>.txt`, per
  `l10n/workshop.md` and the folder's own `README.md`. The title's anchor
  term is "warcasket"; every localized title must contain the rendering of
  "warcasket" recorded in that language's glossary. There is no Keyed
  title key to keep in step with (`WORKSHOP_TITLE_KEY` is `None`).

## This mod's grounding domain

Domain mod: **Vanilla Factions Expanded - Pirates (VFEP)**, plus vanilla
Core and Odyssey. **VFEP ships English only**, so its warcasket vocabulary
is not available from VFEP itself for any other language. For each target
language, check first whether a community "Vanilla Expanded" translation
covers VFEP in that language and ground terms against it; where none does,
coin the term and record it in `glossary/<Language>.md` rather than
inventing silently at translation time. Each glossary names the source it
was grounded against.

Terms that MUST be grounded before use:

- from VFEP's own vocabulary: "warcasket" itself, the VFEP set names our
  text or Workshop page references (siegebreaker, guardian, brute),
  "warcasket foundry", "shoulders" / "pauldrons", "helmet", "shell";
- from vanilla Core (ground against the Core tar per `l10n/process.md`):
  apparel terms (armor, helmet, shoulder pads), plasteel, uranium, spacer
  tech level, energy shield / shield bubble, respirator, breathing, aiming
  time, melee, "Imperial" (Royalty's Empire, as a plain adjective);
- from vanilla Odyssey (ground against the Odyssey tar): vacuum, hull,
  hull plating, gravlite panel, gravship, orbit.

Grep the tars for just this handful of terms; never extract or read a
whole tar. The vanilla-grounded answers for common words live in
`l10n/languages/<Language>.md`; this mod's own coined terms and VFEP-term
decisions live in `glossary/<Language>.md`.

## Workflows

Follow `l10n/process.md`'s Initial generation / Update pass / Audit-only
workflows verbatim. This mod's specifics on top:

- The checker: `python3 Scripts/check-translations.py` (`--strict` for new
  languages). Sidecar regen: `python3
  Scripts/refresh-translation-expectations.py` (game must be closed; drives
  the deployed L10nProbe, which must have this mod ticked in its settings).
- Routing: a key whose sidecar entry lists `worlds` goes to the root gated
  on those worlds' extra package (today only the VGE charge noun, above);
  the checker simulates every world and reports the right root when a key
  is misplaced.
- The public roster is CONTRIBUTING.md's localization table, update it in
  the same commit as any language addition or native review.
- Machine-assisted passes are run as one Opus subagent per language with a
  bounded brief (the key list, the grounded VFEP terms, the language file
  and glossary, the register gate); the lead reviews every diff and owns
  the commit.
