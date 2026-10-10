# Russian glossary, Shipcracker Warcasket

Initial generation, 2026-09-26 (machine-assisted, pending native review). Only
mod-specific coinages and grounding decisions; family-wide mechanics and style
live in `l10n/languages/Russian.md`.

VFEP grounding source: the community Russian translation of VFE Pirates,
OneCodeUnit/VanillaRussianExpanded on GitHub (Vanilla Russian Expanded). Its
set pattern is an adjective on `броня мертвеца`; a named set's shoulders and
helmet drop `мертвеца` (`наплечники прорывной брони`, `шлем прорывной брони`).
This mod follows that pattern so the set reads like one of VFEP's own.

| English | Russian | Grounding |
|---|---|---|
| warcasket | броня мертвеца | Community VFEP pack (`VFEP_Warcasket_Warcasket.label`). No separate word for "shell"; it is dropped in the part descriptions, as the pack does. |
| warcasket foundry | цех мертвеца | Community VFEP pack (`VFEP_WarcasketFoundry.label`). Foundries in general (lore paragraph 2) are `цеха`. |
| entomb / weld into (the set) | заковать в броню мертвеца | Community VFEP pack (`VFEPirates.EntombInWarcasket`). Used in the Workshop page for "weld it on" and "welded into the set". |
| shipcracker (set name) | абордажная (броня мертвеца) | **Coined.** Royalty's `Shipcracker37.title` is `штурмовик`, but its adjective `штурмовая` is already the community pack's Shock set (`штурмовая броня мертвеца`), so reusing it would collide. `абордажная` (boarding) follows the pack's adjective pattern and names the same role the backstory describes. |
| shipcracker warcasket / shoulders / helmet | абордажная броня мертвеца / наплечники абордажной брони / шлем абордажной брони | Pack pattern for named sets (Siegebreaker: `прорывная броня мертвеца`, `наплечники прорывной брони`, `шлем прорывной брони`). |
| a shipcracker (the pilot, lore paragraph 3) | штурмовик | Royalty `Shipcracker37.title`, verbatim. Used only as a person noun ("Штурмовик в такой броне"), never as the set adjective. |
| Shipcracker backstory (Workshop page) | предыстория «Штурмовик» | Royalty `Shipcracker37.title`, capitalized and in guillemets as a cited name. |
| launch through the vacuum ... room-to-room combat | летит сквозь пустоту космоса к вражескому кораблю ... в тесном бою | Mirrors Royalty `Shipcracker37.description` ("полёт ... сквозь пустоту космоса в сторону вражеского корабля ... захват в тесном бою"), recast into finite verbs. |
| Imperial | имперский | Royalty (`имперский дезертир`, `имперский торговец`). |
| spacer-tech (warcasket) | космического уровня | Core `TechLevel_Spacer` = `космический`; the pack's spacer sets say `сделанная по технологиям космического уровня`, shortened here. |
| improved impact dispersion | улучшенное рассеивание энергии ударов | Community pack, Guardian (`с продвинутым рассеиванием энергии ударов`), with `улучшенным` for "improved". |
| 10th-generation chassis | каркас десятого поколения | Pack lore (`Броня мертвеца десятого поколения`); `каркас` is the pack's word for the warcasket frame. |
| drop thrusters | десантные двигатели | **Coined.** `десант` carries the drop-troop sense; the pack uses `реактивные двигатели` for Aerial/Shock. Odyssey's `Thruster` = `ускоритель` is the gravship building, so it is not reused. |
| breach jump (ability) | пробивной прыжок | **Coined.** Not `прорыв-`: `прорывная` is the pack's Siegebreaker. Parallels the pack's `силовой прыжок` for VFEP's own jump. |
| breach burn (vacuum label) | пробивной рывок | **Coined**, same root; `рывок` for the long thruster burn. |
| breach (DamageDef) / breach power | пробой / сила пробоя | **Coined**, same root. |
| breach jump range | дальность пробивного прыжка | Royalty `JumpRange` = `Дальность прыжка`; description mirrors its `Максимальная дистанция прыжка.` |
| breaching arms (shoulders) | пробивные манипуляторы | **Coined.** |
| melee assistor modules | модули поддержки в ближнем бою | Coined; the pack's Brute shoulders use `компонентами, усиливающими оператора в ближнем бою`. |
| aiming assist | система помощи в прицеливании | Coined; Core `AimingDelayFactor` = `время прицеливания`. |
| ranged shield / shield bubble | силовой щит (от дальних атак) | Pack (`силовой щит`); Core's shield-belt tutor text frames it as stopping ranged attacks (`дальние атаки`). |
| hull / hull plating | корпус / обшивка корпуса | Odyssey `GravshipHull` = `корпус гравикорабля`. |
| vacuum / line of sight | вакуум / прямая видимость | Odyssey (`в вакууме`); Core `AbilityRequiresLOS` = `Требуется прямая видимость`. |
| structures (stat description) | постройки | Core `Structure.label` = `постройка`. |
| plasteel / uranium / gravlite panel | пласталь / уран / панель из гравлита | Core; Odyssey `GravlitePanel`. |
| chemfuel / astrofuel | химтопливо / астротопливо | Short form as in Odyssey and the pack (`химтоплива`); astrofuel is coined (VGE's Russian label not checked). |
| deathMessage | `{0} {PAWN_gender ? раздавлен : раздавлена} пробивным ударом.` | Mirrors Core `Crush.deathMessage` (`раздавлен : раздавлена`). Keeps the English `{0}`; `PAWN` is supplied by the call site (`deathMessage.Formatted(pawn.LabelShortCap, pawn.Named("PAWN"))`, decompile-verified). |
| Siegebreaker / Brute / Guardian (Workshop page) | прорывная / беспощадная / охранная броня | Community pack set adjectives. |
| spacer warcaskets (research, Workshop FAQ) | «Космическая броня мертвеца» | Coined; the pack's research label was not available to check. Quoted as a project label because the English FAQ names the project. |
| Workshop title | Абордажная броня мертвеца | Contains `броня мертвеца`; sentence case. |
| fuel unit (`chargeNoun`, the tank's charge word) | единица топлива | Generic `топливо`, covering химтопливо and астротопливо. The ending fallback would mangle the last character, so `1.6/Languages/Russian/WordInfo/plural.txt` maps it to `единицы топлива` and `WordInfo/case.txt` carries both numbers' six cases for the genitive lookups (`Осталось единиц топлива`, `Стоимость 1 единицы топлива`). |

## Pending native review

- `абордажная` as the set adjective, instead of Royalty's `штурмовик`/`штурмовая`
  (taken by the pack's Shock set). Is the person noun `штурмовик` in lore
  paragraph 3 confusing next to the Shock set?
- The coined breach family (`пробивной прыжок`, `пробивной рывок`, `пробой`,
  `сила пробоя`, `пробивным ударом`) and `десантные двигатели`.
- `пробивные манипуляторы` for "breaching arms": whether "arms" reads better as
  mechanical manipulators or as armored forearms.
- The research name in the Workshop FAQ: replace with the pack's actual label
  for VFEP's "spacer warcaskets" research if it differs.
- Lore paragraphs 2 and 3 and the whole Workshop page are first-pass free prose.
