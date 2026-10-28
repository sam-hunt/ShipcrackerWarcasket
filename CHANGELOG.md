# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- Entries are short one-liners ready to paste as Steam Workshop change notes: what changed for the player, no rationale, no def names, no sub-bullets. -->

<!-- Convention: unreleased changes accumulate under the [Unreleased] heading at the top,
     directly below this intro, so a merged contribution is recorded where the release
     flow finds it. Each release renames that heading to its version and date and adds a
     link reference at the bottom of the file. The release workflow lifts the tagged
     version's section into the GitHub release body verbatim and FAILS a stable release
     if the section is missing, so write the section before tagging. Release candidates
     (X.Y.Z-rc.N) get no section: their changes land in the stable version's section at
     promotion. The /release skill walks through all of this. -->

## [Unreleased]

## [1.4.1] - 2026-10-28

### Changed

- Smaller download: developer comments stripped and images losslessly recompressed.

### Fixed

- Without Odyssey, the vacuum burn setting now shows unticked and greyed out.
- The "(requires Odyssey)" settings note is now Odyssey purple.

## [1.4.0] - 2026-10-27

### Added

- A toggle beside the Breach Jump button switches the landing blast off and on.

## [1.3.0] - 2026-10-27

### Added

- Combat Extended support: armor values, bulk and carry capacity matching other spacer-tier warcaskets, and the helmet blocks CE smoke.

### Fixed

- Mod settings keep applying after a mid-game language switch instead of falling back to the defaults until a restart.

## [1.2.1] - 2026-10-17

### Changed

- The mod preview image now shows RimWorld 1.7.

## [1.2.0] - 2026-10-15

### Added

- RimWorld 1.7 support. Save Our Ship 2 itself does not start cleanly on 1.7 yet, so its compatibility is only confirmed on 1.6 until SOS2 updates.

### Fixed

- German and Spanish reload stat descriptions now use the feminine article for the armor's fuel unit.

## [1.1.1] - 2026-10-11

### Changed

- Mod description now lists Combat Extended as reported working.

### Fixed

- The helmet's chosen color now tints only its painted plating instead of the whole helmet.

## [1.1.0] - 2026-10-11

### Added

- The Breach Jump button shows jumps left, like the jump pack.
- Mod settings for the jump: fuel cost, tank size, range and cooldown.
- Mod settings for the landing: blast and roof punch radius, damage, armor penetration and the shoulders' bonus.
- Mod setting to make the Breach Burn follow normal jump range in space.
- Mod settings translated into all 9 languages.

### Changed

- With Vanilla Gravship Expanded the tank holds 50 astrofuel instead of 100, the same 5 jumps as chemfuel.

## [1.0.2] - 2026-10-11

### Changed

- The armor's fuel tank now counts fuel units instead of "jumps", so the number shown matches what a Breach Jump burns (thanks [@Joedox](https://github.com/Joedox), [#2](https://github.com/sam-hunt/ShipcrackerWarcasket/pull/2)).
- French translation revised by a native speaker: the set keeps VFE Pirates' "warcasket" name instead of "sarcophage" (thanks [@Joedox](https://github.com/Joedox), [#1](https://github.com/sam-hunt/ShipcrackerWarcasket/pull/1)).

## [1.0.1] - 2026-10-06

### Changed

- Updated mod icon.
- Mod description split into paragraphs.

## [1.0.0] - 2026-10-06

First release.

### Features

- Shipcracker warcasket set: armor, shoulder pads, helmet.
- Breach Jump: leap walls, land with a wall-breaking shockwave.
- Breach Burn (Odyssey): fast line-of-sight flight in space.
- Vacuum rated with Odyssey.
- Aerial shoulders and helmet extend the jump's range.
- Worn by veteran raiders.
- Save Our Ship 2 and Vanilla Gravship Expanded support.
- Translated into 9 languages.

[Unreleased]: https://github.com/sam-hunt/ShipcrackerWarcasket/compare/v1.4.1...HEAD
[1.4.1]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.4.1
[1.4.0]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.4.0
[1.3.0]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.3.0
[1.2.1]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.2.1
[1.2.0]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.2.0
[1.1.1]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.1.1
[1.1.0]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.1.0
[1.0.2]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.0.2
[1.0.1]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.0.1
[1.0.0]: https://github.com/sam-hunt/ShipcrackerWarcasket/releases/tag/v1.0.0
