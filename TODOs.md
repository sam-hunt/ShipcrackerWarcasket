# TODOs

## Open questions

- Salvagers: watch what VE does with Odyssey's salvager faction, then revisit whether they should field the set.
- Mod setting for fuel type when VGE is active?
- Mod setting for breach burn range?
- Render inner targeting ring for roof punch radius?

## Settings window

Findings against the family's settings window rules (`~/dev/RimworldModTemplate`, `Source/<version>/Settings/CLAUDE.md`).

- **"(requires Odyssey)" suffix uncoloured** (`ShipcrackerWarcasket.xml:54`): pick a colour. The label is built as `string += TaggedString` (`Settings_Burn.cs` `DoVacuumBurnRow`), which strips tags, so colour the resolved suffix with `Colorize`.
