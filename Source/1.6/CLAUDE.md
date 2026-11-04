# C# guidance (Source/1.6)

Root namespace `ShipcrackerWarcasket`; Harmony patch classes live in `Patches/` under the
`.Patches` namespace suffix to avoid RimWorld type-name clashes. Log with the
`[Shipcracker Warcasket]` prefix. Every class header carries its own rationale; the rules below
are the ones that decide where new code goes.

- **Patch timing is the load-bearing hazard of this mod.** `PatchAll` runs from the
  `[StaticConstructorOnStartup]` in `ModInit.cs`, never from the `Mod` subclass constructor:
  Mod constructors run before defs load, and applying a patch runs the target type's static
  ctor, which permanently breaks a VFEP type whose cctor resolves defs (the BetterTradersGuild
  v1.1.0 CWTL incident). Keep it there; the header has the full account.
- **Startup def-derived state must survive an in-process play-data reload.** A mid-session
  language switch rebuilds every def, but a type initializer runs once. Anything that reads or
  writes defs at startup, or caches a def instance, goes in the idempotent `Startup.Run`,
  never a bare static ctor (its header has the load ordering). A mod-shipped texture dies on
  the same reload: hold one on the def instance or behind a getter that re-resolves on a
  Unity-null `== null` check (never `??`), never in a static field.
- **Worn graphics go through the pawn render tree, not draw hooks.** Extra graphics are
  `apparel.renderNodeProperties` entries on the def; the thruster glow in `Rendering/` is the
  model. The zoomed-out pawn cache bakes the tree once with DrawMeshNow, which ignores property
  blocks: anything per-frame must skip the bake or bake a fixed state and dirty the wearer's
  frames at each transition (`GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty`). The bake
  draws in request order with no sorting, so a transparent node must sit after the opaque node
  it overlays (the `AddChildren` postfix reorders the glow). Never force a live render with a
  `disableCache` prefix on `PawnRenderer`: it runs for every humanlike pawn, every frame.
- **Mod settings override def fields in place** (`Settings/`). Defaults are read from the
  defs after every patch has applied, so a def's header stays the record of the shipped tuning
  and a compat root's patch moves the default too. Settings never reach a save; stale
  per-instance state reconciles lazily against the settings' `Generation`. A setting must mean
  the same with any optional root loaded or not (the jump costs chemfuel; the VGE root sets an
  exchange rate, not a cost). Where a root changes a field's unit outright (Combat Extended's
  penetration in megapascals), the row becomes a separate setting under its own key and Keyed
  strings, never a rescale. `Settings/Tunable.cs` is shared across the mod family: keep it
  byte-identical but for the namespace and key prefix.
- **Dependency DLLs** (`VFEPirates.dll`, `VEF.dll`) are compile-only and resolved by the
  csproj (explicit override, Workshop copy, sibling checkout, Mods folder, in that order). A
  missing DLL is skipped silently, so a build passing without them proves nothing about code
  that uses their types. Logic the tests cover must stay free of VEF and VFEP types
  (`Tests/1.6/CLAUDE.md`).
- Plain `//` comments only, no `///` doc comments. Never bulk-apply RCS1146 to Unity-typed
  receivers; UNT0007/UNT0008/UNT0023 are the mechanical guard (`.editorconfig`).
