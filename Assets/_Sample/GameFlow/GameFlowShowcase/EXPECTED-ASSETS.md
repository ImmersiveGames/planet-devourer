# Expected Unity Assets

This file tracks the Game Flow Showcase materialization target. Unity assets must be created through Unity; this document does not replace serialized assets.

## Materialized in the current authoring tree

```text
GameApplication_GameFlow.asset
Routes/Route_Hub.asset
Routes/Route_BasicFlow.asset
Activities/Activity_Basic_A.asset
Activities/Activity_Basic_B.asset
Activities/Activity_Basic_C.asset
Activities/ActivityContent_Basic_A.asset
Activities/ActivityContent_Basic_B.asset
Scenes/SCN_GameFlow_Persistence.unity
Scenes/SCN_GameFlow_Hub.unity
Scenes/SCN_GameFlow_Basic.unity
Scenes/SCN_GameFlow_Basic_A.unity
Scenes/SCN_GameFlow_Basic_B.unity
```

The current tree also contains supporting sample UI/content, persistent Transition/Loading presentation and contextual BGM authoring used by the proven Basic Flow vertical.

There is intentionally no:

```text
Activities/ActivityContent_Basic_C.asset
Scenes/SCN_GameFlow_Basic_C.unity
```

`Activity_Basic_C` is deliberately content-less so the sample proves that an Activity does not require an Activity Content Profile and that A/B Activity-owned and Activity-local content does not leak into C.

## Current content roles

```text
Scenes/SCN_GameFlow_Basic.unity
  Route-owned environment / walls

  Activity-local content
    Visitors A -> Activity_Basic_A
    Visitors B -> Activity_Basic_B

  Route-owned navigation
    Go to Activity C
      no ActivityContentBinding required
      available independently of A/B visibility

Scenes/SCN_GameFlow_Basic_A.unity
  Activity-owned scene content for Activity_Basic_A

Scenes/SCN_GameFlow_Basic_B.unity
  Activity-owned scene content for Activity_Basic_B

Activity_Basic_C.asset
  no ActivityContentProfile
  no Activity-owned scene
  no C-specific Activity-local content
```

The `Visitors A` and `Visitors B` objects remain authored in the Route Primary Scene and use `ActivityContentBinding` for Activity-driven visibility. They are not Activity scene materialization.

`SCN_GameFlow_Basic_A.unity` and `SCN_GameFlow_Basic_B.unity` remain separate Activity-owned scenes so the same Basic Flow scenario demonstrates Activity scene composition and release.

Entering `Activity_Basic_C` proves the negative case: the previous Activity-owned scene is released and A/B local content is hidden while C remains a valid active Activity with no owned content profile.

## Persistent presentation roles

`SCN_GameFlow_Persistence.unity` explicitly composes the current sample presentation surfaces:

```text
Transition
  UnityFadeCurtainEffectAdapter

Loading
  UnityLoadingSurfaceAdapter
```

The proven presentation paths are:

```text
Route_Hub -> Route_BasicFlow
  Fade cover/reveal
  Loading presentation

Route_BasicFlow -> Route_Hub
  Fade cover/reveal
  Loading presentation

Activity A <-> B
  Seamless

Activity A -> C
Activity B -> C
  Fade
  no Loading presentation

Activity C -> A / B
  Seamless
```

`FadeWithLoading`, readiness-governed waits and participant-aware progress are not claimed by this materialization yet.

## Current BGM proof

The explicit Play/Silence path remains proven:

```text
Route_Hub Silence
  -> Activity_Basic_A BGM
  -> Activity_Basic_B BGM
  -> Route_Hub Silence
```

The content-less Activity C adds a no-request preservation proof:

```text
Activity_Basic_A BGM
  -> Activity_Basic_C has no new BGM intent
  -> Activity A BGM remains confirmed

Activity_Basic_B BGM
  -> Activity_Basic_C has no new BGM intent
  -> Activity B BGM remains confirmed
```

Therefore the sample now demonstrates all three normal intent outcomes needed by the baseline BGM contract: explicit Play, no-request Preserve, and explicit Silence.

## Remaining evolutionary materialization

Create only as justified by the scenario catalog:

```text
additional Scenario Route assets only when a distinct contract requires them
additional Scenario Activity assets only when a distinct contract requires them
Loading Readiness / participant-aware progress coverage
Restart / Recovery coverage
contextual Camera presentation/requests where natural
additional Audio coverage only where it teaches a new contract
supporting Player configuration only if a scenario actually requires Player
```

Composition / Visibility does not require a separate scenario because its basic contract is already demonstrated inside Basic Flow.

Basic Transition presentation does not require a separate scenario because Route Fade + Loading, Activity Seamless and Activity Fade are already demonstrated inside the same Basic Flow cycle.

Exact future scenario allocation remains evolutionary. Do not create assets merely to mirror the ADR inventory.
