# Expected Unity Assets

Status: **PLANNED / BLOCKED — DO NOT MATERIALIZE YET — RECONFIRMED 2026-08-26**

Local Multiplayer remains blocked by the public Slot/device/input ownership and observation contract defined in FG-ADR-002 Revision 3.

The public arbitrary Actor-selection surface is now available, but that does not provide the missing Local Multiplayer participant/device/Slot authority.

When the remaining blocker is resolved, expected materialization may include:

```text
GameApplication_LocalMultiplayer.asset
PlayerSessionProfile_LocalMultiplayer.asset
multiple Supported Slots configuration
public participant / device / Slot ownership composition
public observable input ownership / effective control-scheme evidence
multi-participant Joining presentation
Actor Selection presentation when applicable
supporting Route / Activity / Scene assets
optional application-local HUB when multiple compatible Scenarios exist
```

Do not create parallel sample-owned Slot, device or input authority, hidden PlayerInput discovery or a second Session registry to satisfy this list.

This file remains intentionally non-materializable until the public blocker is closed.
