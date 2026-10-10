# Traditional Chinese glossary, Shipcracker Warcasket

Machine-assisted first pass (2026-09), no native review yet. Mod-specific terms
and grounding decisions only; family-wide mechanics, punctuation rules and the
zh-Hant/zh-Hans inversion table live in `l10n/languages/ChineseTraditional.md`.

**Community VFEP source.** The Workshop pack "Vanilla Factions Expanded - Pirates - 簡&繁中文漢化包"
(id 2727638702) attests the noun 戰棺 on its page, but its Traditional files could not be obtained. The file-confirmed lineage is the
zh-Hans pack (GitHub q847633684/Vanilla-Expanded-ZH): 战棺, `<set>型战棺装甲` /
`<set>型战棺护肩` / `<set>型战棺头盔`, 战棺铸造厂, 破袭型 / 圣盾型 / 狂怒型. The
terms below marked "derived" are Traditional forms of that pack's terms, each
checked character by character against the zh-Hant Core/Royalty/Odyssey tars
(not a script conversion). The page's 無畏戰棺 prefix is deliberately not used.

| English | Traditional Chinese | Grounding / notes |
|---|---|---|
| warcasket | 戰棺 | Attested on the community pack's page; zh-Hans files use 战棺. Workshop title anchor. |
| shipcracker (set name) | 破艦型 | Royalty `Shipcracker37.title` is 破船員, a person noun (員), which does not fit the `<set>型` slot, and 破船 alone reads as "shabby boat". Coined 破艦型 keeps the backstory's 破 and follows the verb-object shape of the VFEP set name 破襲型. |
| shipcracker (the pilot, lore paragraph 3; Workshop reference to the backstory) | 破船員 | Royalty `Shipcracker37.title` verbatim; paragraph 3 mirrors that backstory's description (穿過真空…降落在敵艦的船體上，打穿障礙進入內部). |
| shipcracker warcasket / shoulders / helmet | 破艦型戰棺裝甲 / 破艦型戰棺護肩 / 破艦型戰棺頭盔 | Derived from the zh-Hans pack's `<set>型战棺装甲/护肩/头盔`. 頭盔 and 裝甲 match vanilla (Core `Apparel_PowerArmorHelmet` 海陸頭盔, 海陸裝甲). |
| pauldrons / shoulder pads | 護肩 | Derived (zh-Hans 护肩, also the foundry UI's `VFEP.Shoulderpads`). No vanilla hit either way. |
| torso piece (Workshop) | 裝甲主體 | Derived from the foundry UI's `VFEP.ArmorFrame` (装甲主体). |
| shell (warcasket torso) | 外殼 | Core `Shell.label`, Royalty cataphract helmet 防護外殼. |
| warcasket foundry | 戰棺鑄造廠 | Derived (zh-Hans 战棺铸造厂); 鑄造廠 is Core `FoundryApprentice76`. |
| Siegebreaker / Brute / Guardian (Workshop) | 破襲型 / 狂怒型 / 聖盾型 | Derived from the zh-Hans pack. |
| entombed / welded into the set (Workshop) | 封在戰棺內; assembling at the foundry 組裝 | zh-Hans pack's `VFEP.WarcasketText` (封入, 组装) rather than a literal "weld". |
| specialized spacer-tech | 特化型太空科技 | 特化型 from the zh-Hans pack's descriptions; 太空科技 is Core `Synthread.description` (zh-Hans pack writes 太空时代级别). |
| improved impact dispersion | 強化了衝擊分散性能 | Coined, close to the English. The zh-Hans pack renders the same English loosely (在不增加机体重量的同时超大幅提升了防护性); not adopted because it invents a weight claim. |
| breach jump | 破壁跳躍 | Coined. 跳躍 is Royalty's jump vocabulary (`JumpRange` 跳躍距離, jump pack 跳躍背包); 破壁 (breach a wall) chosen over Core's 破城 (breach axe 破城斧), which implies siege of a city. |
| breach burn (vacuum label) / the burn | 破壁推進 / 推進力 | Coined; 推進 from Odyssey `Thruster` 推進器. |
| breach jump range | 破壁跳躍距離 | Mirrors Royalty `JumpRange.label`; description mirrors `JumpRange.description` 最大跳躍距離。 |
| breach power | 破壁威力 | Coined. Description mirrors Core `MeleeDamageFactor` (…傷害的倍率。). |
| breach (DamageDef label) | 破壁衝擊 | Coined; vanilla damage labels are injury nouns (Crush 壓傷, Bomb 炸傷) but none fits, so the label names the impact. |
| deathMessage | {0}被破壁衝擊壓死了。 | Mirrors Core `Crush.deathMessage` {0}被壓死了。 |
| drop thrusters / drop engine | 空降推進器 / 空降引擎 | Coined; 推進器 as above. |
| breaching arms | 破壁臂 (hardened: 硬化破壁臂) | Coined, consistent with 破壁. |
| melee assistor modules | 近戰輔助模組 | zh-Hans pack 近战辅助模块; 模組 is Taiwan usage for a hardware module too (Core `Ship_SensorCluster` 感測器模組). |
| aiming assist | 輔助瞄準裝置 | Odyssey `AimAssistance.label` 輔助瞄準. |
| respirator | 呼吸器 | Odyssey rulestrings (組裝呼吸器). |
| Imperial | 帝國 | Royalty cataphract descriptions (帝國的甲冑騎士); faction is 破碎帝國. |
| boarding corps | 帝國登船部隊 | Core `PirateTrooper73` 登船隊員 / 登船隊. |
| salvage / desertion | 打撈 / 叛逃 | Odyssey `Salvagers` 打撈者; Core 叛逃. |
| hull / hull plating | 船體 / 船體裝甲 | Royalty `Shipcracker37` 敵艦的船體, Odyssey `GravshipHull` 重力船船體. |
| dock | 船塢 | Odyssey rulestrings 太空船塢. |
| gravlite panel / chemfuel / vacuum / line of sight | 重力板 / 化合燃料 / 真空 / 視線 | Odyssey and Core labels, `AbilityRequiresLOS` 所需視線. |
| astrofuel (VGE, Workshop only) | 太空燃料 | Coined; the VGE zh-Hant term was not checked. |
| Workshop title | 破艦型戰棺 | Contains 戰棺. No settings, so no Keyed title coupling. |
| fuel unit (`chargeNoun`, the tank's charge word) | 燃料 | Plain mass noun (燃料剩餘, 沒有燃料。), matching zh-Hans; generic, covering 化合燃料 and 太空燃料. No plural change, no lookup file. |

## Pending native review

- 破艦型 as the set name, versus reusing Royalty's 破船員 more directly (for
  example 破船員型): the pass judged the 員 suffix wrong for a 型 slot.
- Every "derived" term above (護肩, 裝甲主體, 戰棺鑄造廠, 破襲型/狂怒型/聖盾型,
  特化型): converted from the zh-Hans pack and character-checked against the
  zh-Hant tars, but the actual Traditional pack could not be compared.
- 強化了衝擊分散性能 deviates from the zh-Hans pack's phrasing for the same
  English; a reviewer may prefer consistency with how the community pack
  renders the Brute/Guardian pieces.
- The lore paragraphs' voice (這是帝國的設計，不是海盜的。 / 護盾擋得住子彈，擋不住刀刃，
  所以駕駛員專挑愛打近身戰的人。) and 逐間掃蕩 for "room by room".
- 破壁衝擊 as a damage label, and 太空燃料 for VGE's astrofuel.
