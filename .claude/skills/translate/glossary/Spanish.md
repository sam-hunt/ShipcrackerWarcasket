# Spanish (Castellano) glossary — Shipcracker Warcasket

Initial generation, 2026-09-26 (machine-assisted, pending native review). Only
mod-specific coinages and grounding decisions; family-wide mechanics/style
live in `l10n/languages/Spanish.md`. Folder `Spanish`, grounded against the
Castilian tars only (never `SpanishLatin`, whose VFEP lineage uses
"sarcotraje").

VFEP grounding: the community Castilian-leaning VFE Pirates translation by
Cito2310 (GitHub `Cito2310/translate-project-rimworld`). Its label pattern is
`ataúd de guerra <set>` for the torso, `hombreras <set> de ataúd de guerra`
and `casco <set> de ataúd de guerra` for the other parts; this mod follows it.

| English | Spanish | Grounding |
|---|---|---|
| warcasket | ataúd de guerra (pl. ataúdes de guerra) | Community VFEP (`VFEP_Warcasket_Warcasket.label`). Also the Workshop title anchor. |
| shipcracker (set name, also the trooper in lore paragraph 3) | rompenaves | **Coined.** Royalty's `Shipcracker37.title` in the Castilian tar is `navegante` ("navigator"), which drops the ship-breaking sense entirely and would read as a navigation set. `rompenaves` follows the productive verb+noun pattern of `rompehielos`/`rompeolas`; it is invariable in gender and number, so it sits after `hombreras` and `casco` without agreement and doubles as a person noun ("Un rompenaves se lanza..."). The Workshop page still cites the backstory by its in-game title, `"navegante"`. |
| shipcracker warcasket / shoulders / helmet | ataúd de guerra rompenaves / hombreras rompenaves de ataúd de guerra / casco rompenaves de ataúd de guerra | Community VFEP label pattern (`ataúd de guerra bruto`, `hombreras bruto de ataúd de guerra`, `casco bruto de ataúd de guerra`). |
| Workshop title | Ataúd de guerra rompenaves | Sentence case, contains the VFEP term. The mod has no settings key to match. |
| warcasket shell / pauldrons (shoulders) / helmet | armazón / hombreras / casco | Community VFEP (`Un armazón de ataúd de guerra...`, `VFEP.Shoulderpads`, `VFEP.Helmet`). |
| warcasket foundry | forja de ataúd de guerra | Community VFEP `VFEP_WarcasketFoundry.label`. |
| entomb / welded into | sepultar / sepultado en | Community VFEP `VFEPirates.EntombingWarning` ("Sepultar colonos en un ataúd de guerra"). |
| specialized spacer-tech | especializado de tecnología espacial | Community VFEP 10th-gen descriptions (`Un armazón especializado de ataúd de guerra de tecnología espacial`). |
| improved impact dispersion / melee assistor modules | dispersión de impacto mejorada / módulos de asistencia cuerpo a cuerpo | Community VFEP Brute torso and shoulders, verbatim. |
| VFEP set names in prose | Siegebreaker, Bruto, Guardián; "series" for set | Community VFEP (`La serie Bruto`); it leaves Siegebreaker in English and is inconsistent on `guardian`/`guardián`, so the accented form is used. |
| spacer (tech level) | era espacial | Core `TechLevel_Spacer`. |
| breach jump | salto de brecha | Coined on vanilla's verb rendering: Core `ImmediateAttackBreaching` renders "breach your walls" as `abrir una brecha en tus muros`; Royalty grenadier text uses `apertura de brechas`. Ability description mirrors Biotech `Longjump.description` register (third-person present: `Salta a un lugar distante...`). |
| breach burn (vacuum label) | impulso de brecha | Coined. "burn" is rendered `impulso` everywhere (lore paragraph 3, `vacuumDescription`, Workshop) so the vacuum-state label and its description read as one word family. |
| breaching arms (shoulders) | brazos zapadores | Vanilla's attributive "breach" is sapper vocabulary: Core `MeleeWeapon_BreachAxe` = `hacha zapeadora`, `Tribal_Breacher` = `zapador tribal`. |
| breach jump range / breach power (stats) | alcance del salto de brecha / potencia de brecha | Descriptions mirror Core `MeleeDamageFactor.description` ("Multiplicador de la cantidad de daño...") register. |
| breach (DamageDef label) | brecha | Core DamageDef labels are action nouns (`aplastamiento`, `explosión`). |
| deathMessage | El impacto de un salto de brecha ha aplastado a {0}. | Core `Crush.deathMessage` inflects `aplastado/aplastada` on `{0_gender}`, which resolves from the word tables on a plain name string and defaults masculine. Restructured to the active voice so nothing agrees with the victim; `{0}` stays bare. |
| drop thrusters | propulsores de descenso | `propulsor` is community VFEP's word (Aerial `propulsores de cohete`, Shock `gran propulsor`). |
| Imperial | imperial (lowercase adjective) | Royalty (`batallón imperial`, `desertor imperial`). |
| backstory | trasfondo | Core Keyed (`RestartAfterImportingLegacyBackstoryTanslations`). |
| lore paragraph 3 (launch... room by room) | se lanza a través del vacío, aterriza en el casco de la nave enemiga, lo perfora hasta el interior y conquista la nave de habitación en habitación | Mirrors Royalty `Shipcracker37.description` (`lanzarse a través del vacío del espacio, aterrizar en el casco de la nave enemiga, perforar... conquistarla en un combate de habitación en habitación`). |
| hull / hull plating | casco (de nave) / cascos de nave | Royalty `Shipcracker37.description`, Odyssey `GravshipHull` = `casco de gravinave`. Context separates it from `casco` (helmet). |
| sealed | hermético | Odyssey (`paredes y puertas herméticas`). |
| respirator | respirador (integrado / propio) | No vanilla Castilian item; plain word. |
| wearer | portador | Core (`Apparel_ArmorMarineHelmetPrestige.description`). |
| line of sight | línea de visión | Core smoke description (`oscureciendo la línea de visión`). |
| plasteel / uranium / chemfuel / gravlite panel / vacuum / orbit | plastiacero / uranio / biocombustible / panel de gravilita / vacío / órbita | Core and Odyssey labels. |
| astrofuel (VGE) | astrocombustible | Coined, parallel to Core `biocombustible`; VGE's own Spanish, if any, was not checked. |
| purple | púrpura | Per `l10n/languages/Spanish.md` (Odyssey over Core `morado`). |
| fuel unit (`chargeNoun`, the tank's charge word) | unidad de combustible | Generic `combustible`, not `biocombustible`, since VGE swaps in astrofuel. The fallback pluralizes only the last word, so `1.6/Languages/Spanish/WordInfo/plural.txt` maps it to `unidades de combustible`. |

## Pending native review

- `rompenaves` as the set name, and the decision not to reuse Royalty's
  `navegante`.
- `salto de brecha` / `impulso de brecha` / `potencia de brecha`: whether
  `brecha` reads naturally as a modifier, or whether a player expects
  something like `salto de asalto`.
- `brazos zapadores` for "breaching arms".
- The deathMessage restructure (active voice, victim as object).
- Lore paragraphs 2 and 3, in particular "entre chatarreros y desertores" for
  "Salvage and desertion" and "El escudo detiene balas, no filos".
- Whole Workshop description (`.steamworkshop/Description/Spanish.txt`),
  especially "una opción de peso" and "apto para EVA".
