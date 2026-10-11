#!/usr/bin/env python3
# Shipcracker Warcasket's config shim over the shared translation checker
# (l10n/checker/check_translations.py, the rimworld-l10n submodule). The
# engine holds all logic; this file holds only this repo's config and the
# rationale behind it. Usage is unchanged:
#   python3 Scripts/check-translations.py [--strict] [--root PATH]
# If l10n/ is empty, run: git submodule update --init

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "checker"))
import check_translations as engine  # noqa: E402  (import after sys.path edit)

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

# No [TranslationCanChangeCount]-style matching-token fields in this repo:
# the surface is three apparel defs' label/description text.
engine.PARITY_EXEMPT_FIELDS = set()

# RATIONALE: no DLC is a hard dependency (About.xml's modDependencies are
# Harmony and VFE Pirates), but the sidecar is multi-world (see the refresh
# shim's WORLDS) and Odyssey must be active in at least one of them: the
# MayRequire stat and cost leaves gate on it, and Vanilla Gravship Expanded
# requires it. Ideology stays out: its MayRequire stat leaf drops a number,
# never a key, and nothing here will ever gate translatable content on it.
engine.REQUIRED_DLCS = {"Odyssey"}

# Every def here is a VFEPirates.WarcasketDef, a ThingDef subclass with no
# database of its own (it adds shortDescription and three role flags). The
# game rolls it into DefDatabase<ThingDef>, so the probe's walker dumps
# these defs under ThingDef and DefInjected translations legally target
# 1.6/Languages/<Language>/DefInjected/ThingDef/. The checker needs the
# XML element tag mapped to that folder to match Defs/ against the sidecar.
engine.DEF_TYPE_ALIASES = {
    "VFEPirates.WarcasketDef": "ThingDef",
}

# The mod settings window is a real Keyed surface, so a missing Languages/
# tree is a hard config error, not a legal state.
engine.ALLOW_NO_KEYED_SURFACE = False

# The localized Steam Workshop title lives in this Keyed key (the
# settings-window header); the checker enforces the title-coupling rule
# against each .steamworkshop/Description/<Language>.txt title line.
engine.WORKSHOP_TITLE_KEY = "SCWC_SettingsCategory"

raise SystemExit(engine.main())
