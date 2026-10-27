# TODOs

- Add gizmo to toggle whether breach jump/burn fires the breach charge on landing or not... so players can have pawns jump back to their own ship afterwards without blowing a hole in its wall.

## Open questions

- Salvagers: watch what Vanilla Expanded does with Odyssey's salvager faction, then revisit whether they should field the set.
- Mod setting for fuel type when VGE is active?
- Mod setting for breach burn range?
- Render inner targeting ring for roof punch radius?

## Settings window

Findings against the family's settings window rules (`~/dev/RimworldModTemplate`, `Source/<version>/Settings/CLAUDE.md`).

- **Vacuum burn row shows its saved value without Odyssey** (`ShipcrackerWarcasketSettings.cs:286`); it defaults on, so it reads ticked. Show it forced off, keep the saved value, grey the label (Toxin's `ToggleRow(..., forced)` is the model).
- **"(requires Odyssey)" suffix uncoloured** (`ShipcrackerWarcasket.xml:52`): pick a colour. The label is built as `string += TaggedString` (`:283-285`), which strips tags, so colour the resolved suffix with `Colorize`.
