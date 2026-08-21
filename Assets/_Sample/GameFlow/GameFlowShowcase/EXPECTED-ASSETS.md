# Expected Unity Assets

This file tracks the Game Flow Showcase materialization target. Unity assets must be created through Unity; this document does not replace serialized assets.

## Materialized in the current authoring tree

```text
GameApplication_GameFlow.asset
Routes/Route_Hub.asset
Routes/Route_BasicFlow.asset
Activities/Activity_Basic_A.asset
Activities/Activity_Basic_B.asset
Activities/ActivityContent_Basic_A.asset
Activities/ActivityContent_Basic_B.asset
Scenes/SCN_GameFlow_Persistence.unity
Scenes/SCN_GameFlow_Hub.unity
Scenes/SCN_GameFlow_Basic.unity
Scenes/SCN_GameFlow_Basic_A.unity
Scenes/SCN_GameFlow_Basic_B.unity
```

The current tree also contains supporting sample UI/content and Activity-scoped BGM authoring for the Basic Flow Activities.

## Remaining evolutionary materialization

Create only as justified by the scenario catalog:

```text
additional Scenario Route assets
additional Scenario Activity assets
additional Scenario scenes
contextual Camera presentation/requests where natural
contextual Route/Activity BGM intent where dependency-safe
supporting Player configuration only if a scenario actually requires Player
```

Exact scenario allocation remains evolutionary. Do not create assets merely to mirror the ADR inventory.
