---
name: translate
description: Generate, update, or audit mod localization (DefInjected only) for a target language, grounded in Vanilla Factions Expanded - Pirates warcasket terminology plus vanilla Core/Odyssey apparel terminology for Shipcracker Warcasket's single warcasket set. Use when asked to add a language, update translations, or check translation freshness.
argument-hint: "[language, e.g. German | update | check]"
---

# Translate

Produce or refresh localization files for Shipcracker Warcasket. English is
the source of truth; every other language derives from it.

**Do not start a translation pass until the English text is final.** The
def XML today carries placeholder text ("Placeholder description for the
shipcracker warcasket ..."); real lore and descriptions have not been
written. The release gate is the earliest this skill should actually
translate anything, not before.

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
  language, skim otherwise.

There is no `l10n/workshop.md` reading needed yet: this mod has no
`.steamworkshop/` folder or Workshop title key today (see below).

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

The whole surface is 9 translatable DefInjected strings (10 sidecar
entries; the tenth is a texture path, see below), so a pass is a small,
bounded task.

- **No Keyed strings and no English Languages tree at all.** English is
  served entirely by the def XML's own `label`/`description`/
  `shortDescription`; there is nothing under `1.6/Languages/English/`. The
  translation surface is DefInjected only.
- **Enumerate the key set from `Scripts/expected-injections.json`, never
  from a Languages folder.** The three defs are `VFEPirates.WarcasketDef`, a
  `ThingDef` subclass with no def database of its own, so the game rolls
  them into `ThingDef`: DefInjected files go under
  `DefInjected/ThingDef/`, not a `WarcasketDef` folder.
- Today's 10 sidecar entries, 3 per def (`SCWC_Warcasket_Shipcracker`,
  `SCWC_WarcasketShoulders_Shipcracker`, `SCWC_WarcasketHelmet_Shipcracker`):
  `label`, `description`, and `shortDescription` (a VFEP-specific field on
  `WarcasketDef`, shown in the warcasket foundry's part-picker UI; it is
  required, translate it like any other field), plus one entry that is
  NOT required and must NEVER be translated or placed in any language
  file: `SCWC_Warcasket_Shipcracker.comps.CompShieldBubble.shieldTexPath`
  is a texture path, not player-facing text.
- **No gated compat load root is active yet.** An Odyssey compat root
  (`1.6/Mods/Odyssey/`, `Mods/Odyssey/`) is drafted in `LoadFolders.xml` but
  commented out; nothing is Odyssey-gated today, so every translation goes
  in the main `1.6/Languages/<Language>/` tree. When that root goes live,
  its defs' DefInjected must move into
  `1.6/Mods/Odyssey/Languages/<Language>/DefInjected/ThingDef/` with an
  `_Odyssey` filename suffix (never the main tree, see CLAUDE.md's
  Localization and Optional-Content Gating section for why), and any
  `MayRequire` on those defs becomes redundant and should be dropped at the
  same time.
- **No Workshop description convention yet.** There is no `.steamworkshop/`
  folder and no localized Workshop title key (this mod has no Keyed surface
  to hold one). Treat that convention as future work only; do not invent a
  `.steamworkshop/` folder as part of a translation pass.

## This mod's grounding domain

Domain mod: **Vanilla Factions Expanded - Pirates (VFEP)**, plus vanilla
Core and Odyssey. **VFEP ships English only** (checked: its `Languages/`
has only an `English` folder), so its warcasket vocabulary is not available
from VFEP itself for any other language. For each target language, check
first whether a community "Vanilla Expanded" translation mod covers VFEP
in that language and ground terms against it; where none exists, coin the
term and record it in `glossary/<Language>.md` rather than inventing
silently at translation time.

Terms that MUST be grounded before use:

- from VFEP's own vocabulary: "warcasket" itself, the VFEP set names our
  text references (siegebreaker, guardian, controller, sarcophagus, brute),
  "warcasket foundry", "shoulder pads" / "pauldrons", "helmet", "shell",
  and "entomb" / "entombing" (VFEP's term for installing a pawn into a
  warcasket);
- from vanilla Core (ground against the Core tar per `l10n/process.md`):
  apparel terms (armor, helmet, shoulder pads), plasteel, spacer
  components, uranium, energy shield / shield bubble, insulation, toxic
  resistance, psychic sensitivity;
- from vanilla Odyssey (ground against the Odyssey tar): vacuum resistance.

Grep the tars for just this handful of terms; never extract or read a
whole tar. The vanilla-grounded answers for common words live in
`l10n/languages/<Language>.md`; this mod's own coined terms and VFEP-term
decisions live in `glossary/<Language>.md`. A new language starts from
nothing and gets its terms grounded and recorded per `l10n/process.md`.

## Workflows

Follow `l10n/process.md`'s Initial generation / Update pass / Audit-only
workflows verbatim. This mod's specifics on top:

- The checker: `python3 Scripts/check-translations.py` (`--strict` for new
  languages). Sidecar regen: `python3
  Scripts/refresh-translation-expectations.py` (game must be closed; drives
  the deployed L10nProbe, which must have this mod ticked in its settings).
- There is no compat-root routing to do today (see above); everything
  lands in the main tree. Revisit this section once the Odyssey root goes
  live.
- The public roster is CONTRIBUTING.md's localization table, update it in
  the same commit as any language addition or native review.
