# Shipcracker Warcasket

> A RimWorld mod adding a warcasket apparel set for Vanilla Factions Expanded - Pirates

[![RimWorld](https://img.shields.io/badge/RimWorld-1.6-blue.svg)](https://rimworldgame.com/)
[![Requires VFE Pirates](https://img.shields.io/badge/Requires-VFE%20Pirates-orange.svg)](https://steamcommunity.com/sharedfiles/filedetails/?id=2723801948)

## About

This mod is in early development. Nothing is implemented yet, and the notes below describe the
planned scope, not shipped content.

Shipcracker Warcasket will add a single new warcasket apparel set, armor, shoulder pads and
helmet, built for Vanilla Factions Expanded - Pirates (VFE Pirates). It is tuned for the
end-game content unlocked by the Odyssey DLC, though Odyssey itself is optional: any
Odyssey-specific content is gated behind the DLC and simply does not load without it.

## Features (planned)

### A New Warcasket Set

- **Armor, shoulder pads and helmet** as a single matched warcasket set
- Built on top of VFE Pirates' warcasket systems, rather than replacing them
- Tuned for the end-game power level reached with the Odyssey DLC installed

### Optional Odyssey Content

- Odyssey-specific tuning and content ships from a DLC-gated load folder, so it never loads without
  the DLC
- Without Odyssey, the mod still functions using only VFE Pirates as a base

No further mechanics, stats, or names are finalized yet.

## Requirements

- **RimWorld 1.6** or later
- **Vanilla Factions Expanded - Pirates** (required; Steam Workshop id
  [2723801948](https://steamcommunity.com/sharedfiles/filedetails/?id=2723801948)), which itself
  requires **Vanilla Expanded Framework**
- **Harmony** (auto-download from Steam Workshop if you don't have it)

**Odyssey DLC** is optional. Odyssey-specific content is gated and does not load without it.

## Installation

### Steam Workshop

Not yet published. A Workshop page will be linked here once the mod is released.

### Manual Installation

1. Download the latest release from the [Releases](https://github.com/sam-hunt/ShipcrackerWarcasket/releases) page
2. Extract the `ShipcrackerWarcasket` folder to your RimWorld `Mods` directory:
   - **Windows**: `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\`
   - **Mac**: `~/Library/Application Support/Steam/steamapps/common/RimWorld/RimWorldMac.app/Mods/`
   - **Linux**: `~/.steam/steam/steamapps/common/RimWorld/Mods/`
3. Enable the mod in RimWorld's mod menu, after VFE Pirates and its dependencies
4. Restart RimWorld

## Compatibility

- Early development. Not yet tested against existing saves.
- Not tested with Combat Extended.

## Contributing

Bug reports and feature requests welcome on
[GitHub Issues](https://github.com/sam-hunt/ShipcrackerWarcasket/issues).
Please attach any relevant hugslib logs/stack traces/mod lists etc.

For development setup, see [CONTRIBUTING.md](CONTRIBUTING.md). The mod builds with
`dotnet build ShipcrackerWarcasket.sln -c Release`; Release builds auto-deploy into the local
RimWorld Mods folder.

## Credits

**Author**: Sam Hunt ([@sam-hunt](https://github.com/sam-hunt))

**Built With**:

- [Harmony](https://github.com/pardeike/Harmony) by Andreas Pardeike - Runtime patching library
- RimWorld modding API, community examples

**Special Thanks**:

- [Vanilla Expanded team](https://steamcommunity.com/workshop/filedetails/?id=2723801948) for
  Vanilla Factions Expanded - Pirates
- [Ludeon Studios](https://ludeon.com) for RimWorld and modding API
