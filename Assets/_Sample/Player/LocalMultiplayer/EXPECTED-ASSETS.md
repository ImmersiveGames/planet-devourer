# Expected Unity Assets

Status: **PLANNED / BLOCKED — DO NOT MATERIALIZE YET**

Local Multiplayer remains blocked by the public Slot/device/input contract defined in FG-ADR-002.

When that blocker is resolved, the expected materialization may include:

```text
GameApplication_LocalMultiplayer.asset
PlayerSessionProfile_LocalMultiplayer.asset
multiple Supported Slots configuration
public device / Slot / input ownership composition
multi-participant Joining / Actor Selection presentation
supporting Route / Activity / Scene assets
optional application-local HUB when multiple compatible Scenarios exist
```

Do not create parallel sample-owned Slot, device or input authority to satisfy this list.
