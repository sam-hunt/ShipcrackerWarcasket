# Simplified Chinese glossary — Shipcracker Warcasket

Initial generation, 2026-09-26 (machine-assisted, pending native review). Only
mod-specific coinages and grounding decisions; family-wide mechanics/style
live in `l10n/languages/ChineseSimplified.md`.

VFEP terms are grounded against the community Vanilla Expanded Simplified
Chinese pack (GitHub `q847633684/Vanilla-Expanded-ZH`, the
`Vanilla Factions Expanded - Pirates` folder: its WarcasketDef ThingDef
injections, Keyed UI and ResearchProjectDef files). Vanilla terms are
grounded against the Core, Royalty and Odyssey `ChineseSimplified (简体中文).tar`.

| English | Simplified Chinese | Grounding |
|---|---|---|
| warcasket | 战棺 | Community VFEP pack throughout (`VFEP.RequiresWarcasket` = 需要战棺, every set label). |
| shipcracker (set name) | 飞船突袭型 | Royalty `Shipcracker37.title`/`titleShort` = 飞船突袭者, cut to the community pack's `<set>型` set-name pattern (破袭型 / 圣盾型 / 狂怒型 / 突击型). |
| shipcracker (the soldier, lore paragraph 3) | 飞船突袭者 | Royalty `Shipcracker37.title` verbatim, so the lore sentence names the backstory it paraphrases. |
| shipcracker warcasket / shoulders / helmet | 飞船突袭型战棺装甲 / 飞船突袭型战棺护肩 / 飞船突袭型战棺头盔 | Community pattern `<set>型战棺装甲/护肩/头盔` (`VFEP_Warcasket_Siegebreaker.label` = 破袭型战棺装甲 and siblings). The body piece takes 装甲 although the English label has no noun, exactly as the pack does. |
| siegebreaker / guardian / brute (Workshop) | 破袭型 / 圣盾型 / 狂怒型 | Community pack labels. The third-party 无畏 prefix (WarCasket Expanded) is not used; the VFEP pack has none. |
| Specialized spacer-tech warcasket shell/pauldrons/helmet | 特化型太空时代级别战棺装甲/护肩/头盔 | Community pack's opening phrase for every 10th-generation part, verbatim. |
| improved impact dispersion | 改进型冲击力偏转模块 | Community `VFEP_WarcasketHelmet_Guardian.description` (配备有改进型的冲击力偏转模块), trimmed of 的. |
| melee assistor modules | 近战辅助模块 | Community `VFEP_WarcasketShoulders_Brute.description`. |
| standard ranged shield | 标准型远程能量护盾 | Community Siegebreaker lore uses 远程能量护盾; 标准型 mirrors the pack's 标准型/改进型 register. |
| warcasket foundry | 战棺铸造厂 | Community `VFEP_WarcasketFoundry.label`. |
| weld on / entomb (Workshop) | 在战棺铸造厂完成改造; 封在战棺里 | Community renders entomb loosely (进入铸造厂, 战棺改造 in `VFEPirates.EntombingWarning`); the Workshop follows that looseness. |
| spacer warcaskets (research) | 「太空时代战棺」 | Community `VFEP_SpacerWarcaskets.label`; corner brackets per vanilla research-name citation (研究「基础逆重科技」). |
| pilot | 驾驶员 | Community Keyed/lore (成为驾驶员的殖民者). |
| breach (the mod's concept) | 破墙 | Core `MeleeWeapon_BreachAxe.label` = 破墙斧; Core breach raids "intend to breach your walls" = 攻破你的墙体. Used as the fixed prefix for every breach term below. |
| breach jump | 破墙跳跃 | 破墙 + Royalty `JumpRange.label` = 跳跃距离's 跳跃. |
| breach burn (vacuumLabel) | 破墙推进 | Coined. 推进 is the Odyssey thruster vocabulary (`Thruster.label` = 推进器) and renders "burn" in lore paragraph 3 too. |
| breach jump range | 破墙跳跃距离 | Royalty `JumpRange.label` = 跳跃距离; description mirrors `JumpRange.description` = 最大跳跃距离。 |
| breach power | 破墙威力 | Coined. Description mirrors Core `MeleeDamageFactor.description` (…伤害乘数。). |
| breach (DamageDef label) | 破墙冲击 | Coined. Vanilla damage labels are mostly X伤, but the breach term needed to stay recognisable; 冲击 carries "breaching impact". |
| deathMessage | {0}被破墙冲击压死了。 | Mirrors Core `Crush.deathMessage` = {0}被压死了。 (the def's hediff is Crush). |
| drop thrusters | 空降推进器 | Coined from community `VFEP_ActiveDropPawn.label` = 空投舱 (drop) and Odyssey 推进器 (thruster). |
| breaching arms | 破墙臂 | Coined on the 破墙 prefix. |
| respirator | 呼吸器 | Coined (no vanilla respirator item or gene in the zh tars); plain everyday word. |
| aiming assist / aiming time | 辅助瞄准装置 / 瞄准速度 | Core `AimingDelayFactor.label` = 瞄准时间; the Workshop's "aiming speed" stays colloquial. |
| breathing | 呼吸能力 | Core `Breathing.label`. |
| Imperial | 帝国 | Royalty adjective use (帝国逃兵, 帝国穿梭机, 帝国的甲胄骑士). |
| desertion | 逃兵 | Royalty `GiveQuest_Intro_Deserter.label` = 帝国逃兵. |
| boarding corps | 帝国登舰部队 | Plain military term; no vanilla analog. |
| hull (lore) / hull plating (ability) | 船体 / 舱壁 | Lore follows Royalty `Shipcracker37.description` (降落在敌舰的船体上); the ability names the buildable wall, Odyssey `GravshipHull.label` = 逆重飞船舱壁. |
| line of sight | 视线 | Odyssey psychic lance descriptions (需要目标在视线内). |
| vacuum / orbit / gravlite panel | 真空 / 轨道 / 逆重板 | Odyssey `Vacuum`, `Orbit.label`, `GravlitePanel.label`. |
| plasteel / uranium / wall / roof | 玻璃钢 / 铀 / 墙 / 屋顶 | Core labels. |
| energy shield / shield bubble | 能量护盾 | Core `Apparel_ShieldBelt.description` (单人用的能量护盾装置). |
| Royalty / Odyssey | 皇权 / 奥德赛 | Core `ExpansionDefs`. |
| EVA rated (SOS2) | 具备舱外活动（EVA）能力 | Plain term, EVA kept for SOS2 players. |
| astrofuel (VGE) | 天体燃料 | Coined: VGE ships English only and the community pack has no VGE translation. |
| Workshop title | 飞船突袭型战棺 | Set name + 战棺 (the README's anchor term). |
| fuel unit (`chargeNoun`, the tank's charge word) | 燃料 | Plain mass noun (燃料剩余, 没有燃料。), matching zh-Hant; generic, covering 化合燃料 and 天体燃料. Not 燃料罐, which names a container rather than one unit. No plural change, no lookup file. |

Lore paragraph 3's first sentence mirrors Royalty's `Shipcracker37.description`
(降落在敌舰的船体上，打孔到舰内) but renders "room by room" as 逐个舱室, not the
official 一对一的战斗 (a mistranslation of room-to-room).

## Pending native review

- 飞船突袭型 as a set name: five characters before 战棺装甲 makes the body label
  long (飞船突袭型战棺装甲); the Royalty tie was preferred over a shorter coinage.
- The whole 破墙 family (破墙跳跃, 破墙推进, 破墙威力, 破墙冲击, 破墙臂): 破墙
  is vanilla's breach word but reads wall-specific when the jump opens a ship
  hull in space. 破壁 was the alternative considered.
- 破墙推进 for "breach burn" and 破墙冲击 as a damage label (vanilla's are X伤).
- 空降推进器, 呼吸器, 天体燃料 (coined, no vanilla or community source).
- The lore paragraphs and the Workshop page prose, a first machine-assisted
  pass rewritten once for register.
