# French glossary — Shipcracker Warcasket

Machine-assisted generation, revised by a native speaker (Joedox, PR #1). Only
mod-specific coinages and grounding decisions; family-wide mechanics/style
live in `l10n/languages/French.md`. Folder `French`, grounded against the
Core, Royalty, Biotech and Odyssey `French (Français).tar`s.

VFEP grounding: VFEP ships English only and no community French translation
survives (the "Vanilla Factions Expanded - Pirates [Fr]" Workshop item,
id 2725064005, has been removed), so a French player sees "brute warcasket",
"warcasket foundry" and "spacer warcaskets" in English in-game. "warcasket"
is therefore kept untranslated, as vanilla keeps "chemfuel": a native-speaker
PR (#1) replaced the earlier "sarcophage", which had come from that removed
item's page and matched nothing the player sees. Every other VFEP term below
(foundry, set names, parts, research) is coined.

**Contractions are written by hand.** DefInjected values are never passed
through `LanguageWorker_French.PostProcessed`: `DefInjectionPackage` only
unescapes `\n`, and our C# (`Ability_BreachJump`) concatenates
`description + " " + vacuumDescription` raw. Vanilla agrees: 0 uncontracted
`de/le/que/ne/se/la` + vowel sequences against 918 hand-elided ones across
Core and Odyssey ThingDef descriptions. Hand-contracted text is also inert
under the worker (its regexes match only the uncontracted forms; a simulation
over these files yields zero rewrites), so it is correct either way. The
Workshop page is never post-processed at all.

| English | French | Grounding |
|---|---|---|
| warcasket | warcasket (m., pl. warcaskets; invariable as a modifier: `coque warcasket`) | Untranslated, matching VFEP's English labels in a French game (see above); native PR #1. Also the Workshop title anchor. |
| shipcracker (set name, also the trooper in lore paragraph 3) | brise-coque | **Coined.** Royalty's `Shipcracker37.title`/`titleShort` in the French tar is `pirate`, unusable here: VFEP is the pirates mod and lore paragraph 2 opens "un modèle impérial, et non pirate". `brise-coque` follows the `brise-glace` compound pattern, is invariable after a noun, doubles as a person noun ("Un brise-coque se lance...") and pairs with the coined `brise-siège`. `coque` is Odyssey's hull word (`GravshipHull` = `coque de vaisseau`). The Workshop page cites the backstory by its in-game title, `"pirate"`, with "(Shipcracker en anglais)". |
| shipcracker warcasket / shoulders / helmet | warcasket brise-coque / épaulières de warcasket brise-coque / casque de warcasket brise-coque | Core/Royalty apparel pattern `casque de cataphracte`, `armure de commando` (part `de` material/class noun); `brise-coque` stays invariable after the noun. |
| Workshop title | Warcasket brise-coque | Sentence case, contains the VFEP term. No settings key to match. |
| warcasket shell (torso) | coque warcasket | Native PR #1 (replaced the coined `carapace`); the `warcasket` modifier keeps it apart from the hull `coque` used two sentences later. |
| pauldrons / shoulders | épaulières | **Coined** (no vanilla shoulder apparel; Core only has `épaule` body parts). Ordinary French armor term. Plural label: `[X_definite]` agreement may default masculine singular in vanilla keyed strings. |
| helmet / sealed helm (Workshop) | casque / heaume étanche | Core `casque de commando`; `heaume` has an aspirated h (`le heaume`). |
| spacer / spacer-tech / spacer warcasket | spatial / de technologie spatiale / warcasket spatial | Core `TechLevel_Spacer` = `spatial`; `de technologie spatiale` for the attributive "spacer-tech" (native PR #1). |
| warcasket foundry | fonderie de warcaskets | **Coined.** |
| spacer warcaskets (research) | « spacer warcaskets » (warcaskets spatiaux) | The English label in guillemets, because that is the clickable project every French player sees (VFEP is untranslated), with the French gloss in parentheses. |
| weld into / welded (entomb) | souder / soudé dans | Community VFEP Workshop page ("se soudent à l'intérieur"). |
| VFEP set names in prose | Brise-siège, Brute, Gardien; "ensemble" for set | **Coined.** Capitalized as proper names on the Workshop page only. |
| improved impact dispersion | qui disperse(nt) mieux les impacts | **Coined**; a finite relative clause rather than a noun chain. |
| melee assistor modules | modules d'assistance en mêlée | Core `Melee.label` = `mêlée`. |
| aiming assist / aiming speed | assistance à la visée / visée | Odyssey weapon trait `AimAssistance.label`; Core `AimingDelayFactor` = `temps de visée`. |
| onboard respirator | respirateur intégré | **Coined**, plain word (no vanilla French respirator item; Biotech has only `masque à gaz`). |
| sealed | étanche | Odyssey `GravshipHull.description` (`paroi étanche`). |
| drop thrusters | propulseurs de largage | Odyssey `LargeThruster` = `propulseur`; Core `capsule de largage` for drop. |
| breach (attributive) | de brèche | Core `MeleeWeapon_BreachAxe` = `hache de brèche`; `ImmediateAttackBreaching` = `faire une brèche dans vos murs`. |
| breach jump | saut de brèche | As above; description opens in third-person present (`Allume les propulseurs...`), the majority vanilla form (Royalty 23 third-person vs 8 infinitive; Biotech's infinitive `Longjump` is the outlier). Native PR #1. |
| breach burn (vacuum label) / burn | poussée de brèche / poussée | **Coined.** `poussée` is used for every "burn" (lore paragraph 3, `vacuumDescription`, Workshop) so the label and its sentence read as one family. |
| breaching arms | bras de brèche | **Coined**, same attributive pattern. |
| breach jump range / breach power | portée du saut de brèche / puissance de brèche | Royalty `JumpRange` = `portée de saut`; BreachPower description mirrors Core `MeleeDamageFactor.description` (`Un multiplicateur sur la quantité de dégâts...`). |
| breach (DamageDef label) | impact de brèche | Core DamageDef labels are action nouns (`écrasement`, `explosion`); bare `brèche` names the gap, not the blow. |
| jobReportString | utilise la capacité : {0}. | Third-person present like vanilla `JobDef.reportString`s, French-spaced colon, trailing period kept. |
| deathMessage | {0} s'est fait écraser par un impact de brèche. | Core `Crush.deathMessage` = `{0} s'est fait écraser.` verbatim plus the agent; the construction needs no gender agreement, `{0}` stays bare. |
| Imperial | impérial | Royalty `Empire.pawnSingular`. |
| backstory | histoire | Core Keyed `Backstory` = `Histoire :`. |
| lore paragraph 3 (launch... room by room) | se lance à travers le vide spatial, atterrit sur la coque du vaisseau ennemi, y perce un passage jusqu'à l'intérieur et conquiert le vaisseau pièce après pièce | Mirrors Royalty `Shipcracker37.description` (`se lancer à travers le vide spatial, d'atterrir sur la coque d'un navire ennemi, de percer des trous dedans et de le conquérir ... pièce après pièce`), with Odyssey's `vaisseau` for ship. |
| hull / hull plating | coque / coques de vaisseau | Odyssey `GravshipHull` = `coque de vaisseau`. |
| wall / roof | mur / toit | Core `Wall` = `mur`. Vanilla's RoofDef labels say `plafond`, but its prose says `sous un toit` (43 vs 13); jumping over a building reads as `toit`. |
| line of sight | ligne de vue | Core Keyed `AbilityRequiresLOS` = `Ligne de vue requise`. |
| energy shield / ranged shield | bouclier d'énergie / bouclier contre les tirs à distance | Core `Apparel_ShieldBelt` = `bouclier d'énergie`. |
| plasteel / uranium / chemfuel / gravlite panel / vacuum / orbit | plastacier / uranium / chemfuel / panneau de gravlite / vide / orbite | Core and Odyssey labels. |
| astrofuel (VGE) | astrofuel | Kept as-is, parallel to vanilla keeping `chemfuel`; VGE's own French, if any, was not checked. |
| EVA rated (SOS2) | apte aux sorties extravéhiculaires | Plain French. |

## Pending native review

Reviewed by a native speaker in PR #1: the warcasket noun, the three part
labels and descriptions, and the Breach Jump ability text. Still open:

- The coined VFEP terms: `fonderie de warcaskets`, `épaulières`, and the set
  names `Brise-siège`, `Brute`, `Gardien`.
- `saut de brèche` / `poussée de brèche` / `puissance de brèche` /
  `impact de brèche` / `bras de brèche`: whether `de brèche` reads naturally
  as a modifier this often.
- Lore paragraphs 2 and 3, in particular "épaves récupérées et désertions l'ont
  répandu" for "Salvage and desertion have since put it" and "leur goût du corps
  à corps" for "a taste for close work".
- Whole Workshop description (`.steamworkshop/Description/French.txt`),
  especially "un poids lourd du corps à corps" and the "de l'espace pour voler"
  pun in the Odyssey FAQ answer.
