# Brazilian Portuguese glossary: Shipcracker Warcasket

Initial generation, 2026-09-26 (machine-assisted, pending native review). Only
mod-specific coinages and grounding decisions; family-wide mechanics and style
live in `l10n/languages/PortugueseBrazilian.md`.

Community VFEP translation grounded against: RafaelNaymaier's
"Tradução Vanilla Expanded PT-BR" (GitHub `RafaelNaymaier/Traducao-Vanilla-Expanded-PT-BR`,
Workshop 3643091941), confirmed from its ThingDef and Keyed files.

| English | Brazilian Portuguese | Grounding |
|---|---|---|
| warcasket | carcaça de guerra | Community VFEP pack throughout (`VFEP_Warcasket_*.label`, `VFEP.RequiresWarcasket`). "shell" in the English first line folds into the same noun, as the pack does. |
| shipcracker (set name) | quebra-naves | **Coined.** Royalty's `Shipcracker37.title` is "Rastreador de nave" ("ship tracker", a mistranslation), unusable as a set qualifier. Built on the pack's `quebra-cerco` (siegebreaker) so the set reads like one of VFEP's own. Invariable compound, so it needs no gender agreement with carcaça (f), ombreiras (f pl) or capacete (m). Also serves as the agent noun in lore ("Um quebra-naves se lança..."). |
| shipcracker warcasket / shoulders / helmet | carcaça de guerra quebra-naves / ombreiras de carcaça de guerra quebra-naves / capacete de carcaça de guerra quebra-naves | Pack label pattern (`carcaça de guerra quebra-cerco`, `ombreiras de carcaça de guerra quebra-cerco`, `capacete de carcaça de guerra quebra-cerco`). |
| pauldrons / shoulders | ombreiras | Pack uses `ombreiras` for both the label and the "pauldrons" in descriptions. |
| Specialized spacer-tech warcasket ... with improved impact dispersion, fitted/equipped with ... | Carcaça de guerra de tecnologia espacial especializada com dispersão de impacto aprimorada, equipada com ... | Verbatim opening of the pack's Brute/Guardian descriptions, with the agreement switched per part (especializadas/equipadas for ombreiras, especializado/equipado for capacete). |
| melee assistor modules | módulos de assistência corpo a corpo | Pack's `VFEP_WarcasketShoulders_Brute.description`. |
| warcasket foundry | fundição de carcaças de guerra | Pack's `VFEP_WarcasketFoundry` label. |
| entomb / welded into (Workshop) | integrar (em carcaça de guerra) | Pack's `VFEPirates.EntombInWarcasket` = "Integrar em carcaça de guerra". "welded" on the Workshop page uses the same verb, since "soldado" reads as "soldier". |
| siegebreaker / brute / guardian (set names, Workshop) | Quebra-cerco / Bruto / Guardião | Pack's lore paragraphs ("A série Quebra-cerco", "A série Bruto", "A série Guardião"). |
| 10th-generation | de 10ª geração | Pack's shared 10th-gen lore paragraph. |
| breach (damage label, and qualifier in all breach terms) | irrupção | Ideology's `MeleeWeapon_BreachAxe.label` = "machado de irrupção", the only vanilla noun for "breach". Used as the fixed qualifier in every label; prose uses ordinary verbs (romper, atravessar, abrir caminho). |
| breach jump (ability, stat stem) | salto de irrupção | Royalty `JumpRange.label` = "Alcance do salto" supplies "salto"; qualifier as above. |
| breach burn (vacuumLabel) / the burn (prose) | impulso de irrupção / o impulso | **Coined.** "queima" is the aerospace calque but reads as "burning" to a player; "impulso" carries the thrust sense and is used everywhere "burn" appears (lore, vacuumDescription, Workshop). |
| breach jump range | alcance do salto de irrupção | Royalty `JumpRange.label`. Description mirrors `JumpRange.description` ("O máximo alcance do salto") in register. |
| breach power | poder de irrupção | Coined; description mirrors Core `MeleeDamageFactor.description` ("Um multiplicador na quantidade de dano..."). |
| breaching impact (deathMessage) | {0} morreu esmagad{PAWN_gender ? o : a} por um impacto de irrupção. | Mirrors Core `Crush.deathMessage`. `PAWN` is a named argument at the call site (`deathMessage.Formatted(pawn.LabelShortCap, pawn.Named("PAWN"))`, decompiled), so the resolver split is safe. |
| drop thrusters | propulsores de descida | Coined; pack uses "propulsores" (`propulsores de foguete`, `propulsores de ombro`), "de descida" for "drop". |
| breaching arms | braços de irrupção | Coined from the breach qualifier. |
| ranged shield / shield bubble | escudo contra ataques à distância / bolha de escudo | Pack Brute lore ("ataques à distância"); "bolha de escudo" on the Workshop page only. |
| respirator / aiming assist | respirador de bordo / assistência de mira | Pack's `telemetria de bordo` for "onboard". Core `AimingDelayFactor.label` = "tempo de pontaria"; "mira" follows the pack's `computadores de mira`. |
| wearer | portador | Core `Apparel_ShieldBelt.description`. |
| plasteel / uranium / chemfuel | plastiaço / urânio / combustível químico | Core material labels. The pack writes "plastoaço" in lore; vanilla's label wins because it is what the cost list shows. |
| Imperial | imperial (lowercase adjective) | Royalty `Empire.pawnSingular` and in-text "flotilha imperial", "catafratas imperiais". |
| vacuum / hull / line of sight / gravlite panel / orbit | vácuo / casco / linha de visão / painel de gravilita / órbita | Odyssey (`GravshipHull` = "Casco de Gravinave", `GravlitePanel.label`, `Orbit.label`); Core `AbilityRequiresLOS` = "Linha de visão necessária". |
| wall / roof | parede / telhado | Core `Wall.label`, `RoofConstructed.label`. Workshop prose uses "muros" where the English means clearing obstacles, as Core's breaching raid text does ("quebrar seus muros"). |
| backstory (Workshop) | passado | Core Keyed `Backstory` = "Passado". |
| astrofuel (VGE, Workshop) | astrocombustível | Coined; VGE's own pt-BR rendering not checked. |
| Workshop title | Carcaça de Guerra Quebra-Naves | Contains the pack's "carcaça de guerra". Title Case like the sibling mod's pt-BR title. The mod has no settings key to couple. |

Lore paragraph 3 mirrors Royalty's `Shipcracker37.description` ("lançar-se através do vácuo do espaço, aterrissar no casco da nave inimiga ... e conquistá-la em combate de sala em sala") in verb choice and order, but not its "fazer furos no interior", which misreads "punch holes to the interior".

| fuel unit (`chargeNoun`, the tank's charge word) | unidade de combustível | Generic `combustível`, not `combustível químico`, since VGE swaps in astrofuel. The fallback pluralizes only the last word, so `1.6/Languages/PortugueseBrazilian/WordInfo/plural.txt` maps it to `unidades de combustível`. |
## Pending native review

- "quebra-naves" as the set name and agent noun, versus reusing the official "Rastreador de nave".
- "irrupção" as the breach qualifier: vanilla-attested but rare in speech. "arrombamento" (salto de arrombamento, impacto de arrombamento) is the more colloquial alternative if a native reader finds "irrupção" stiff.
- "impulso de irrupção" for "breach burn".
- "propulsores de descida" for "drop thrusters".
- The lore voice in paragraphs 2 and 3, especially "destroços recuperados e desertores o levaram a fundições" and "os pilotos são escolhidos entre quem gosta de lutar de perto".
- Workshop prose, particularly "concorrente pesada para o combate corpo a corpo em solo" and the VGE answer's tone.
