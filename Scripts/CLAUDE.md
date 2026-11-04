# Localization toolchain guidance (Scripts/)

Thin per-repo config shims over the `l10n/` submodule's engines; each shim's comments carry
this repo's rationale (the dependency chain, the worlds, the smoke list). Never edit `l10n/`
from here; if it is empty, run `git submodule update --init`.

- `check-translations.py` validates every language deterministically (also a CI release gate)
  against `expected-injections.json`, a dump of every injection point the live game sees for
  this mod. The sidecar is multi-world (base, odyssey, vge: one boot per optional-package
  combination), so a gated root is asked only for the keys behind its gate. The checker refuses
  stale expectations, so new translatable content forces a regen. It also enforces the
  Workshop title coupling: each `.steamworkshop/Description/<Language>.txt` title must equal
  that language's settings-category Keyed string.
- `refresh-translation-expectations.py` regenerates the sidecar by driving the L10nProbe dev
  mod (source at `l10n/probe/`, built and deployed only from `~/dev/rimworld-l10n`; this mod is
  ticked in its settings) through the game's own walker. Game closed; the release skill runs
  it every release.
- `integration-smoke-test.py` boots the deployed, comment-stripped mod on a pinned list that
  opens every supported integration (Vanilla Gravship Expanded, Combat Extended) and fails on
  any error attributed to this mod or a VFEP/VEF/VGE/CE seam. Save Our Ship 2 is deliberately
  off the list: its patch is best-effort. Sibling-mod "dump WILL fail" warnings at launch are
  expected.
