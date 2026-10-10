# German glossary — Shipcracker Warcasket

Initial generation, 2026-09-26 (machine-assisted, pending native review). Only
mod-specific coinages and grounding decisions; family-wide mechanics/style
live in `l10n/languages/German.md`.

**No community VFEP German translation could be grounded against.** A Workshop
pack "Vanilla Factions Expanded - Pirates Deutsch" (id 2810180496) exists, but
its page shows none of its terms and its files were not available. Every VFEP
term below (warcasket, foundry, entomb, set names, research) is therefore
coined here and listed under pending native review. If that pack's files turn
up, re-ground against them and prefer its renderings.

| English | German | Grounding |
|---|---|---|
| warcasket (coined) | Kriegssarg (m.), pl. Kriegssärge | Literal war + coffin, mirroring the English compound and vanilla's war- → Kriegs- pattern (`MeleeWeapon_Zeushammer` = Kriegshammer). Does not collide with Core's coffin furniture, whose label is `Sarcophagus` = Sarkophag ("Sarg" appears only inside that def's description, never as a label in Core, Royalty or Odyssey). Masculine, like every label head in this set, so the Gender-table miss default (masculine) is correct. |
| compound rule for Kriegssarg | always hyphenated to a set name or part: Schiffsknacker-Kriegssarg, Kriegssarg-Helm, Kriegssarg-Hülle, Kriegssarg-Schulterpanzer, Kriegssarg-Gießerei | Durchkopplung once a set prefix is present; kept uniform in descriptions too so labels and prose match. |
| shipcracker (set name) | Schiffsknacker | Royalty `Shipcracker37.title` = Schiffsknacker (verbatim); Royalty `Apparel_ArmorLocust.description` also renders "shipcracking operations" as "das Knacken von Schiffen". |
| shipcracker warcasket / shoulders / helmet (labels) | Schiffsknacker-Kriegssarg / Schiffsknacker-Kriegssarg-Schulterpanzer / Schiffsknacker-Kriegssarg-Helm | VFEP's `<set> warcasket <part>` label pattern. |
| pauldrons / shoulders / shoulder pads | Schulterpanzer | Ordinary German armor term; "Schultern" alone reads as the body part. Used for VFEP's "warcasket shoulders" pattern and the Workshop "shoulder pads". |
| warcasket foundry (coined) | Kriegssarg-Gießerei | foundry = Gießerei per Core `FoundryApprentice76.title` = Gießereilehrling. |
| entomb (coined, not used in this mod's strings) | einsargen | Real German verb (to put into a coffin), derived directly from Kriegssarg. The Workshop page's "weld" stays einschweißen, as in English. |
| spacer warcaskets (VFEP research, cited in FAQ) | 'Raumfahrt-Kriegssärge' | Core `TechLevel_Spacer` = Raumfahrt; single-quoted as a cited label per the language file. |
| VFEP set names cited on the Workshop page (coined) | Belagerungsbrecher (siegebreaker), Rohling (brute), Wächter (guardian) | Plain renderings; also for later passes: controller = Kontrolleur, sarcophagus = Sarkophag (Core `Sarcophagus` label). |
| spacer-tech | aus der Raumfahrttechnik | Core `Plasteel.description` / `Synthread.description` render "spacer-tech" exactly so. |
| Imperial | imperial (imperiale, imperialen ...) | Royalty `PawnKinds_Empire` (imperialer Grenadier) and `Apparel_ArmorRecon.description` (imperialen Spähtrupps). |
| boarding corps | Enterkommandos | Royalty `ResearchProjects_Apparel` story line "Kataphraktrüstungen für imperiale Enterkommandos"; Core `PirateTrooper73` "Enterkommandos". |
| lore paragraph 3, first sentence | lässt sich durch das Vakuum schießen, landet auf dem feindlichen Rumpf, reißt ein Loch ins Innere und erobert das Schiff Raum für Raum | Mirrors Royalty `Shipcracker37.description` ("sich in die Leere des Weltraums schießen zu lassen, auf dem Rumpf eines feindlichen Schiffes zu landen, Löcher in das Innere zu reißen und das Schiff Raum für Raum zu erobern"), with Odyssey's Vakuum for "vacuum". |
| hull / hull plating | Rumpf / Schiffshüllen | Rumpf per `Shipcracker37.description`; hull plating after Odyssey `GravshipHull` = Gravschiffhülle. |
| breach (damage label) and the breach- prefix | Durchbruch, combining form Durchbruchs- | Core `MeleeWeapon_BreachAxe` = Durchbruchsaxt, `Tribal_Breacher` = Durchbrecher, breach raids "Mauern durchbrechen". |
| breach jump | Durchbruchssprung | Durchbruchs- + Sprung (Royalty jump pack verb springen). |
| breach burn (vacuum label) | Durchbruchsschub | Schub (thrust) for an engine burn; Odyssey thruster = Schubdüse. |
| breach jump range | Durchbruchssprungweite | Royalty `JumpRange` = Sprungweite; long compound in the style of vanilla stat labels (Schildenergiekapazität, Nahkampftrefferchance). Description mirrors Core `Ability_Range`. |
| breach power | Durchbruchskraft | Description mirrors Core `MeleeDamageFactor` ("Ein Multiplikator auf ..."). |
| deathMessage | {0} wurde von einem Durchbruchseinschlag zu Tode gequetscht. | Mirrors Core `Crush.deathMessage` = "{0} wurde zu Tode gequetscht."; `{0}` bare. Einschlag after Core `MeteoriteImpact` = Meteoriteneinschlag. |
| drop thrusters / drop engine | Landeschubdüsen / Landetriebwerk | Core drop pod = Landekapsel, Odyssey thruster = Schubdüse. |
| fuel-burning | treibstoffbetrieben | Not "spritbetrieben": with VGE the jump burns astrofuel. Odyssey `ChemfuelTank.label` = kleiner Treibstofftank. |
| chemfuel / astrofuel (Workshop) | Sprit / Astrosprit | Sprit is Core's chemfuel; Astrosprit coined by analogy (VGE's own German term unknown). |
| impact dispersion | Stoßverteilung | Plain technical coinage. |
| melee assistor modules | Nahkampf-Assistenzmodule | Royalty `Apparel_ArmorCataphract.description` "Neuromemetische Assistenzsysteme". |
| respirator / breathing | Atemgerät / Atmen, Atemleistung | Odyssey research story "Atemgeräten"; Core `Breathing` = Atmen. |
| sealed | luftdicht versiegelt | Odyssey `GravshipHull.description` "luftdichte Wand". |
| standard ranged shield / shield bubble | Standardschild gegen Fernkampfangriffe / Schutzschild | Core `Apparel_ShieldBelt.description` "persönlichen Schutzschild". |
| line of sight | Sichtlinie | No vanilla attestation found; ordinary German gaming term. |
| wall / roof | Wand / Dach | Core `Wall.label`, `Wall.description`. |
| backstory (Workshop) | Vorgeschichte | Core Keyed `Backstory`. |
| Workshop title | Schiffsknacker-Kriegssarg | Byte-identical to the body label; contains Kriegssarg. The mod has no settings key to match. |
| fuel unit (`chargeNoun`, the tank's charge word) | Treibstoffeinheit | Generic Treibstoff, matching the `treibstoffbetrieben` row (chemfuel or astrofuel). Core's `WordInfo/plural.txt` has no entry for it and the fallback would append `s`, so `1.6/Languages/German/WordInfo/plural.txt` maps it to `Treibstoffeinheiten`; `plural_decline` misses and returns that plural unchanged, giving `Keine Treibstoffeinheiten mehr.` |

## Pending native review

- Kriegssarg as the German for "warcasket" (the head of every label, the
  foundry and the Workshop title). Rejected alternatives:
  - Kriegssarkophag: collides with Core's Sarkophag furniture and would double up in VFEP's "Sarkophag-Kriegssarkophag" set.
  - Kriegskapsel: matches vanilla casket = Kapsel (Kryptoschlafkapsel) but loses the coffin sense and reads as a drop pod.
  - Panzersarg: strong sense, but Panzer invites a tank reading and clashes with Schulterpanzer.
  - Kampfsarg: acceptable, but Kriegs- mirrors the English "war-" and vanilla's Kriegshammer.
  - Kriegsrüstung / Kriegshülle: lose the permanent sealed-coffin sense; Rüstung is vanilla's generic armor.
  - Warcasket (English loan): vanilla de translates its coined nouns (Plastahl, Sprit), so a loan would stick out.
- Kriegssarg-Gießerei (foundry). Rejected: Kriegssargschmiede (Schmiede is vanilla's smithy); Kriegssarggießerei unhyphenated (hard to read).
- einsargen (entomb). Rejected: einschließen (too weak), einmauern (walling in, wrong image), bestatten (burial).
- Set names Belagerungsbrecher / Rohling / Wächter / Kontrolleur / Sarkophag, and 'Raumfahrt-Kriegssärge' for the research project: all may differ from the community pack.
- Schulterpanzer for "shoulders". Rejected: Schultern (body part), Schulterstücke (reads as uniform epaulettes).
- Durchbruchs- with the linking s (Durchbruchssprung, Durchbruchsschub, Durchbruchssprungweite) follows vanilla Durchbruchsaxt; a reviewer may prefer Durchbruchsprung.
- Durchbruchsschub for "breach burn". Rejected: Durchbruchszündung (sounds like a single ignition), Durchbruchsflug (loses the engine sense).
- Landeschubdüsen for "drop thrusters". Rejected: Sprungdüsen (loses the orbital-drop sense), Absetzdüsen (unidiomatic).
- Lore voice: "Imperiale Bauart, kein Piratenwerk" and "Als Piloten wählt man Leute, die gern aus nächster Nähe kämpfen" are free renderings of the English rhythm, not literal ones.
- Astrosprit (VGE astrofuel) and the whole Workshop page prose.
