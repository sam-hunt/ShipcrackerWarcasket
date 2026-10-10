# Korean glossary — Shipcracker Warcasket

Initial generation, 2026-09-26 (machine-assisted, pending native review). Only
mod-specific coinages and grounding decisions; family-wide mechanics, josa
markers and style live in `l10n/languages/Korean.md`.

Community VFEP translation grounded against: the RimWorldKorea `RMK` pack
(GitHub RimWorldKorea/RMK), VFE Pirates' ThingDef, WarcasketDef and Keyed
files. Its warcasket set labels follow `<bare set prefix> 워캐스킷 [어깨|헬멧]`
and every set description opens with `우주시대 기술로 특화된 워캐스킷 ...입니다.`;
this mod copies both patterns so the set reads like one of VFEP's own.

| English | Korean | Grounding |
|---|---|---|
| warcasket | 워캐스킷 | RMK VFEP, every warcasket label. No separate word for "shell": RMK drops it (`...워캐스킷입니다.`), and so does this mod. |
| warcasket shoulders / pauldrons | 워캐스킷 어깨 | RMK `VFEP_WarcasketShoulders_*.label`; "pauldrons" in the description is rendered the same way (`워캐스킷 어깨입니다.`). |
| warcasket helmet | 워캐스킷 헬멧 | RMK `VFEP_WarcasketHelmet_*.label`. |
| warcasket foundry | 워캐스킷 주조대 | RMK `VFEP_WarcasketFoundry.label`; bare "foundries" in lore is `주조대`. |
| entomb / weld on | 안치 (워캐스킷에 안치) | RMK `VFEPirates.EntombInWarcasket`; the Workshop's "welded into" / "weld it on" both use 안치. |
| siegebreaker / brute / guardian | 돌격 / 야수 / 수호 | RMK set prefixes (Workshop "pitched between" line). |
| shipcracker (set name) | 함선 침투 | Royalty `Shipcracker37.title` = `함선 침투원`. The set prefix drops the person suffix `원` so it names the role like RMK's action-noun prefixes (돌격, 강습, 수호): `함선 침투 워캐스킷`. Deviation from verbatim reuse, flagged below. |
| a shipcracker (the pilot, lore paragraph 3) | 함선 침투원 | Royalty `Shipcracker37.title`, verbatim. The rest of that sentence mirrors `Shipcracker37.description` (`...적 함선의 선체에 착륙, 구멍을 뚫고 침투해 선실을 하나씩 제압...`). |
| Imperial | 제국 (bare prefix) | Royalty backstories: `제국 부대`, `제국 사제`, `제국군`. |
| spacer-tech / spacer | 우주시대 기술 / 우주시대 | RMK VFEP descriptions (`우주시대 기술로 특화된`); Core `TechLevel_Spacer` = `우주`. |
| improved impact dispersion | 개선된 충격 분산 능력 | RMK Guardian/Brute descriptions, verbatim. |
| 10th-generation (chassis) | 10세대 (우주시대 프레임) | RMK `10세대 워캐스킷`; "chassis" follows RMK's foundry text `워캐스킷 프레임`. |
| pilot | 조종사 | RMK `VFEP_WarcasketHelmet_Shock` (`조종사의 반응 시간`). |
| breach (damage label, "breaching") | 돌파 | Core `Tribal_Breacher` = `돌파자`, `Gun_ThumpCannon` `돌파용 폭탄`. `DamageDef` label shape follows Core `Bomb` = `폭발`. |
| jump | 도약 | Royalty `Apparel_ArmorLocust.verbs.jump` = `도약`; RMK Aerial `충격 도약`. |
| breach jump | 돌파 도약 | Composed from the two rows above, same shape as RMK `충격 도약`. |
| breach burn (vacuum label) / burn | 돌파 분사 / 분사 | Coined. `분사` (jet firing) for an engine burn; kept distinct from `도약` so the vacuum relabel reads as a different mode. |
| breach jump range | 돌파 도약 거리 | Core `JumpRange.label` = `도약 거리`. |
| breach power | 돌파력 | Coined; ordinary `-력` stat noun. Description register mirrors Odyssey `...에 적용되는 배율입니다`. |
| drop thrusters | 강하 추진기 | Odyssey `Thruster.label` = `추진기`; `강하` (airborne drop) coined as the qualifier. |
| breaching arms | 돌파용 강화 팔 | Coined. |
| melee assistor modules | 근접 전투 보조 모듈 | Coined after RMK Shock shoulders (`근접 전투에서 공격과 회피를 보조하는`). |
| onboard respirator / aiming assist | 내장 호흡 장치 / 조준 보조 장치 | Coined; Core `Breathing.label` = `호흡`, `RangedWarmupTime` = `조준 시간`. |
| sealed (helmet) | 밀폐형 | Core `AncientSealedContainer` family uses `밀봉`; `밀폐` is the usual word for an airtight suit. |
| shield | 보호막 | Core `Apparel_ShieldBelt` = `보호막 벨트`; RMK uses 보호막 throughout. |
| hull / hull plating | 선체 / 선체 외벽 | Royalty `Shipcracker37.description` `선체`; Odyssey `GravshipHull` = `중력부양선 외벽`. |
| line of sight / clear line | 시야 (시야가 확보되어야 합니다) | Core `AbilityRequiresLOS` = `시야 필요`; Core psychic lance descriptions `시야가 확보되어야 합니다`. |
| vacuum / orbit / gravlite panel | 진공 / 궤도 / 중력감응판 | Odyssey `Vacuum`, `Orbit.label`, gravship help texts. |
| plasteel / uranium / chemfuel | 플라스틸 / 우라늄 / 화학연료 | Core / Odyssey labels. |
| spacewalk / EVA | 선외활동 | Biotech `Starjack.description`. |
| salvage | 인양 | Odyssey `Salvagers` = `인양단`. |
| boarding corps | 승선 전투 부대 | Coined; avoids 강습 and 돌격, which RMK spends on set names. |
| backstory (Workshop) | 성장 환경 | Core Keyed `Backstory`. |
| astrofuel (VGE, Workshop only) | 아스트로연료 | Coined transliteration; no VGE Korean source checked. |
| deathMessage | `{0}(이)가 돌파 충격에 압사했습니다.` | Mirrors Core `Crush.deathMessage` = `{0}(이)가 압사했습니다.`; bare `{0}` with the `(이)가` marker. |
| Workshop title | 함선 침투 워캐스킷 | The armor label; contains `워캐스킷`. |
| fuel unit (`chargeNoun`, the tank's charge word) | 연료 | Plain mass noun reads naturally in every slot (잔여 연료, 연료당 재장전 비용); generic, covering 화학연료 and 아스트로연료. No plural change, no lookup file. |

## Pending native review

- `함선 침투` as the set prefix (Royalty's `함선 침투원` minus `원`): a
  native reader may prefer the verbatim title (`함선 침투원 워캐스킷`) or a
  shorter prefix.
- `돌파 분사` for the vacuum-mode label, and `분사` for "burn" in lore.
- `강하 추진기`, `돌파용 강화 팔`, `승선 전투 부대`, `아스트로연료`: coined
  with no vanilla or community precedent.
- Lore paragraph 2 (`...주조대에까지 흘러들었습니다`) and the terse two-sentence
  split of "The shield stops bullets, not blades: ...".
- Whole Workshop description (`.steamworkshop/Description/Korean.txt`) is a
  first machine-assisted pass.
