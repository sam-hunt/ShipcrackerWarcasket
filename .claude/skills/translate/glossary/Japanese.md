# Japanese glossary, Shipcracker Warcasket

Initial generation, 2026-09-26 (machine-assisted, pending native review). Only
mod-specific coinages and grounding decisions; family-wide mechanics and style
(ASCII `,` `.`, no LanguageWorker, polite register in descriptions, 「」 vs
ASCII quotes per slot) live in `l10n/languages/Japanese.md`.

**Community VFEP translation:** only the noun is attested. The Japanese mod
database rimworld.2game.info tags VFE Pirates content as ウォーキャスケット and
marks VFEP as 日本語化対応, but no translation files were found, so every other
VFEP term below (foundry, set names, part labels, entomb) is coined. A
different mod (Vanilla Armour Expanded) renders "siegebreaker" as
シージブレーカー, which is the basis for keeping set names in katakana.

| English | Japanese | Grounding |
|---|---|---|
| warcasket | ウォーキャスケット | Community VFEP noun (rimworld.2game.info tagging). Required in the Workshop title. |
| shipcracker (set name) | シップクラッカー | Coined katakana. Royalty's `Shipcracker37.title` is 船の破壊工作員 ("ship saboteur", `titleShort` 工作員), which does not read as an equipment set name in a label (船の破壊工作員ウォーキャスケット). Katakana follows the community set-name habit (シージブレーカー). |
| shipcracker (the role, lore paragraph 3) | 船の破壊工作員 | Royalty `Shipcracker37.title`, verbatim. Paragraph 3 mirrors that backstory's description vocabulary: 真空, 船体に着陸し, 内部へ穴を開け, 部屋から部屋へ. The Workshop page cites it as 経歴「船の破壊工作員」(Shipcracker) to tie the katakana set name to it. |
| X warcasket / X warcasket helmet | シップクラッカー・ウォーキャスケット / シップクラッカー・ウォーキャスケットヘルメット | Core/Royalty attach ヘルメット and アーマー directly (カタフラクトヘルメット, レコンヘルメット); Biotech/Odyssey separate a katakana modifier from a katakana compound with ・ (メカノイド・ドロップポッド, サブコア・エンコーダー). |
| shoulders / pauldrons / shoulder pads | ショルダーアーマー | Coined; no vanilla shoulder piece exists. Chosen over bare ショルダー (reads as the body part or a bag) and ショルダーパッド (evokes suit or sports padding). Used for the label, description and Workshop "Shoulder pads". |
| warcasket shell | ウォーキャスケット外殻 | Coined; plain word for "shell". |
| spacer-tech | 宇宙技術を用いた | Royalty `Apparel_PackJump.description` renders "spacer militaries" as 宇宙技術を用いる一部の軍隊. |
| spacer (tech tier, Workshop) | 宇宙世紀 | Core Keyed `TechLevel_Spacer`. |
| Imperial | 帝国 / 帝国の | Royalty (`Empire.pawnSingular` 帝国; backstories 帝国教会, 帝国の牧師; `Apparel_ArmorCataphract` 帝国のカタフラクト部隊). |
| boarding corps | 移乗攻撃部隊 | Coined from the naval term 移乗攻撃; Core renders boarding parties loosely (奇襲部隊, 乗り込んで来る宙賊), no fixed term. |
| 10th-generation | 第10世代 | Plain; VFEP's generation numbering. |
| breach jump | ブリーチジャンプ | Composed from two vanilla words: Core ブリーチアックス / ブリーチング用 (`Gun_ThumpCannon`) and Royalty ジャンプ / ジャンプパック. |
| breach jump range | ブリーチジャンプ範囲 | Mirrors Royalty `JumpRange.label` ジャンプ範囲 and its description 最大ジャンプ距離です. |
| breach burn (vacuum label) | ブリーチ噴射 | Coined. 噴射 is the ordinary word for a rocket burn; also used for "the burn" in descriptions. |
| breach power | ブリーチ威力 | Coined; description mirrors Core `MeleeDamageFactor` (…の掛け率です). |
| breach (DamageDef label) | 突破 | Coined noun, alongside Core damage labels such as 爆発. |
| deathMessage | {0}は 突破時の衝撃で押し潰されて死亡しました. | Mirrors Core `Crush.deathMessage` ({0}は 押し潰されて死亡しました.), keeping vanilla's space after the name; `{0}` bare. |
| breaching arms | ブリーチングアーム | Coined from Core's ブリーチング. |
| drop thrusters | ドロップスラスター | Odyssey `Thruster.label` スラスター. |
| hull / hull plating | 船体 / 船体装甲 | 船体 from Royalty `Shipcracker37` and Core `Ship_Beam` (船体骨格); 船体装甲 coined. Not Odyssey's `GravshipHull` (グラヴシップ壁), which is a specific wall building. |
| sealed | 気密 | Odyssey `GravshipHull.description` (気密壁). |
| respirator | 呼吸装置 | Coined plain term; no vanilla respirator item. |
| breathing (capacity) | 肺機能 | Core PawnCapacityDef `Breathing.label`. |
| melee (assistor, capabilities) | 格闘 | Core SkillDef `Melee.label`. |
| aiming time / aiming speed | 照準 | Core `AimingDelayFactor.label` 照準時間. |
| standard ranged shield | 標準的な対射撃シールド | Coined; Core `Apparel_ShieldBelt` describes the same effect (弾丸やエネルギー攻撃を食い止め). |
| line of sight | 視線が通る | Core Keyed `AbilityRequiresLOS` 視線が通ることが必要, `AbilityCannotHitTarget`. |
| vacuum / orbit / gravlite panel | 真空 / 軌道 / 重力軽量パネル | Odyssey. |
| wall / roof / structures | 壁 / 屋根 / 建造物 | Core `Wall.label`, `RoofConstructed.label`, `Gun_ThumpCannon.description` (壁や建造物). |
| plasteel / uranium / chemfuel | プラスチール / ウラン / バイオ液化燃料 | Core. |
| ability | 能力 | Royalty Keyed (この能力を獲得しますか). |
| pilot (warcasket wearer) | パイロット | Odyssey `PilotAssistant` (パイロットアシスタント). |
| warcasket foundry | ウォーキャスケット鋳造台 | Coined (Workshop only; also "foundries" in lore paragraph 2 as 鋳造台). Korean community uses 주조대, the same "casting bench" idea. |
| entomb | 封入 | Proposed coinage; not used in any shipped string of this mod. |
| siegebreaker / brute / guardian | シージブレーカー / ブルート / ガーディアン | Katakana; シージブレーカー attested in Vanilla Armour Expanded's Japanese, the other two coined to match. |
| spacewalking | 船外活動 | Plain standard term. |
| astrofuel (VGE) | アストロ燃料 | Coined; VGE has no Japanese translation found. |
| Workshop title | シップクラッカー・ウォーキャスケット | Equals the armor label. |
| fuel unit (`chargeNoun`, the tank's charge word) | 燃料 | Plain mass noun reads naturally in every slot (燃料残数, 燃料回復コスト); 燃料ユニット would be a calque. No plural change, no lookup file. |

## Pending native review

- シップクラッカー as the set name versus a Japanese rendering. The set name
  and the role (船の破壊工作員) now differ in-game, so the pun linking the
  set to the Royalty backstory only survives on the Workshop page.
- ショルダーアーマー for the shoulder piece, and the resulting label length
  (シップクラッカー・ウォーキャスケットショルダーアーマー).
- ブリーチ噴射 for "breach burn"; ブリーチバーン was the alternative.
- 突破 as the damage label, and 移乗攻撃部隊 for "boarding corps".
- ウォーキャスケット鋳造台 (foundry) and 封入 (entomb), both unattested. If
  the community VFEP translation surfaces, align these and the set names to it.
- Lore paragraphs 2 and 3 and the whole Workshop page: first machine-assisted
  prose pass, checked for natural clause order but not by a native reader.
