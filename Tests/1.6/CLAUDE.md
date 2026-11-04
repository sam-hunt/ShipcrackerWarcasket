# Test suite guidance (Tests/1.6)

Headless xUnit (net472), never shipped; vstest hosts it through mono on Linux/WSL.

- Tests may only load types free of VEF and VFEP, whose DLLs never reach the test output.
  Logic worth testing is pulled out of the ability, flyer and settings classes into plain
  ones (`SpacePreviewCache`, `BreachTuning`, `BreachJumpExtension.FlightSeconds`). The XML
  tests read the repo's defs and patches from disk, never the stripped deploy.
- The build copies the game's real 1.6 runtime DLLs into the output (Krafs's reference stubs
  cannot load at run time), resolved in the order the csproj documents: `RIMWORLD_MANAGED_1_6`,
  a 1.6 install's `Managed/`, then the frozen copy at `~/.local/share/RimWorld-1.6/Managed`,
  the only source on a 1.7 install. With none found the build stays green but every test
  fails with `TypeLoadException`, and that run overwrites real DLLs an earlier build copied in.
- CI builds the suite through the sln but cannot run it; the release skill runs it locally as
  its first gate, before anything that boots the game or commits.
