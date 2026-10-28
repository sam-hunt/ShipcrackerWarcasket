using Verse;

namespace ShipcrackerWarcasket;

// Burn section: the one toggle for a jump on a space map. It overrides no def field, so the
// section has no apply step; the header carries the burn gizmo's icon
// (BreachJumpExtension.SpaceIcon).
public partial class ShipcrackerWarcasketSettings
{
    // Whether a jump on a space map is the breach burn (unlimited range, line of sight) or
    // follows the planet rules (Ability_BreachJump.UsesBurn). Read live, so it needs no def.
    public bool vacuumBurn = true;

    // The jump's extension, for the burn gizmo's icon beside the header. Re-read with the
    // defaults on every load, which replaces it; the icon is read at draw time.
    private BreachJumpExtension jumpExt;

    private void ExposeBurnSettings()
    {
        Scribe_Values.Look(ref vacuumBurn, "VacuumBurn", true);
    }

    private void LoadBurnDefaults(Defs defs)
    {
        jumpExt = defs.ext;
    }

    private void ResetBurnSettings()
    {
        vacuumBurn = true;
    }

    private void DrawBurnSection(Listing_Standard listing)
    {
        SectionHeader(listing, "SCWC_SectionBurn", jumpExt.SpaceIcon);
        DoVacuumBurnRow(listing);
    }

    // Shown off and disabled, and labelled as needing Odyssey, when no loaded def can make a
    // space map (Ability_BreachJump.SpaceMapsPossible); the setting itself is read live, so a
    // change needs no restart.
    private void DoVacuumBurnRow(Listing_Standard listing)
    {
        bool spaceMaps = Ability_BreachJump.SpaceMapsPossible;
        string label = "SCWC_VacuumBurn".Translate();
        if (!spaceMaps)
            label += "SCWC_RequiresOdysseySuffix".Translate();
        ToggleRow(listing, label, "SCWC_VacuumBurnDesc".Translate(), ref vacuumBurn, spaceMaps ? null : false);
    }
}
