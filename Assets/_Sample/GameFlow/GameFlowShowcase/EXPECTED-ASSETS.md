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

The current tree also contains supporting sample UI/content and contextual BGM authoring used by the proven Basic Flow vertical.

## Current content roles

```text
Scenes/SCN_GameFlow_Basic.unity
  Route-owned environment / walls
  Activity-local content
    Visitors A -> Activity_Basic_A
    Visitors B -> Activity_Basic_B
  Activity navigation UI

Scenes/SCN_GameFlow_Basic_A.unity
  Activity-owned scene content for Activity_Basic_A

Scenes/SCN_GameFlow_Basic_B.unity
  Activity-owned scene content for Activity_Basic_B
```

The `Visitors A` and `Visitors B` objects remain authored in the Route Primary Scene and use `ActivityContentBinding` for Activity-driven visibility. They are not Activity scene materialization.

`SCN_GameFlow_Basic_A.unity` and `SCN_GameFlow_Basic_B.unity` remain separate Activity-owned scenes so the same Basic Flow scenario also demonstrates Activity scene composition and release.

The current BGM path is proven for the Basic Flow cycle:

```text
Route_Hub Silence
  -> Activity_Basic_A BGM
  -> Activity_Basic_B BGM
  -> Route_Hub Silence
```

## Remaining evolutionary materialization

Create only as justified by the scenario catalog:

```text
additional Scenario Route assets
additional Scenario Activity assets
additional Scenario scenes
Transition coverage
Loading & Readiness coverage
Restart / Recovery coverage
contextual Camera presentation/requests where natural
additional Audio coverage only where it teaches a new contract
supporting Player configuration only if a scenario actually requires Player
```

Composition / Visibility does not require a separate scenario at this point because its basic contract is already demonstrated inside Basic Flow.

Exact future scenario allocation remains evolutionary. Do not create assets merely to mirror the ADR inventory.
