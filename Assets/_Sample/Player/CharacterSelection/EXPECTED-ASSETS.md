# Expected Unity Assets

Status: **PLANNED / BLOCKED — DO NOT MATERIALIZE YET**

Character Selection remains blocked by the public arbitrary Actor-selection surface defined in FG-ADR-002.

When that blocker is resolved, the expected materialization may include:

```text
GameApplication_CharacterSelection.asset
PlayerSessionProfile_CharacterSelection.asset
game-owned character-selection UI
public Framework Join / Actor Selection command consumption
supporting Route / Activity / Scene assets
optional application-local HUB when multiple compatible Scenarios exist
```

Do not create sample-owned internal Actor-selection infrastructure to satisfy this list.
