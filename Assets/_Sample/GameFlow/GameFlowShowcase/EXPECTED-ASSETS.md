# Expected Unity Assets

This file tracks the Game Flow Showcase materialization target and its serialized scene references. The IF-ADR-041 Route-scoped Camera observer path was validated in Unity on 2026-10-04; unrelated future materialization changes still require their own validation.

## Materialized in the current authoring tree

```text
GameApplication_GameFlow.asset

Routes/Route_Hub.asset
Routes/Route_BasicFlow.asset
Routes/Route_ReadinessShowcase.asset

Activities/Activity_Basic_A.asset
Activities/Activity_Basic_B.asset
Activities/Activity_Basic_C.asset
Activities/Activity_Basic_D.asset
Activities/Activity_Basic_E.asset

Activities/ActivityContent_Basic_A.asset
Activities/ActivityContent_Basic_B.asset
Activities/ActivityContentReadiness.asset

Scenes/SCN_GameFlow_Persistence.unity
Scenes/SCN_GameFlow_Hub.unity
Scenes/SCN_GameFlow_Basic.unity
Scenes/SCN_GameFlow_Basic_A.unity
Scenes/SCN_GameFlow_Basic_B.unity
Scenes/SCN_GameFlow_Basic_Readiness.unity
Scenes/SCN_GameFlow_Content_Readiness.unity

Scripts/GameFlowVisitorPreparation.cs
Scripts/GameFlowCameraAssignmentAdapter.cs

Camera/Assignments/CameraAssignment_GameFlow_A.asset
Camera/Assignments/CameraAssignment_GameFlow_B.asset
Camera/Assignments/CameraAssignment_GameFlow_Readiness.asset
Camera/Prefabs/PF_GameFlow_CameraRig_Readiness.prefab
```

Each Route Assignment references a fixed Rig Prefab and the shared physical Output. `SCN_GameFlow_Basic` and `SCN_GameFlow_Basic_Readiness` each serialize one `GameFlowCameraAssignmentAdapter` under a `RouteContentContribution`. The adapter's serialized Activity-to-Assignment map is Route policy: Basic Flow maps A/B to their Assignments and C to none; Readiness maps C/D/E to the shared Readiness Assignment. It observes committed Activity transition context and emits explicit Session Camera commands:

| Transition | Observer context | Command |
|---|---|---|
| Hub / boot -> Basic Flow A | Previous = none, Current = A | Activate A |
| A -> B | Previous = A, Current = B | Replace A -> B |
| B -> A | Previous = B, Current = A | Replace B -> A |
| A/B -> C | Previous = A/B, Current = C | Clear A/B |
| C -> A/B | Previous = C, Current = A/B | Activate A/B |
| A/B -> none / Route exit | Previous = A/B, Current = none | Clear A/B |
| C -> none / Route exit | Neither Activity maps to Assignment | No command |

Readiness camera transitions:

| Transition | Observer context | Command |
|---|---|---|
| Hub / boot -> Readiness C | Previous = none, Current = C | Activate Readiness |
| C -> D / E | Previous and Current map to the same Assignment | No-op |
| D / E -> C | Previous and Current map to the same Assignment | No-op |
| C/D/E -> none / Route exit | Previous maps to Readiness, Current = none | Clear Readiness |

Hub does not issue a boot Clear. Activity C remains content-less; Basic Flow maps it to no Assignment, while Readiness maps it to the same stable Assignment as D/E. There is no CameraRequest or Route/Activity Camera ownership.

Current validation evidence:

```text
Framework EditMode        169/169 PASS
RouteLifecycle observer     6/6 PASS
Camera Editor              74/74 PASS
GameFlow Play Mode         Hub -> A -> B -> A -> C -> B -> Hub
blockingIssues             0 on every executed request
```

Observed Session Camera state:
- Hub -> A: Assignment A active after transition coverage release;
- A -> C: no active normal Assignment;
- C -> B: Assignment B active;
- B -> Hub: no active normal Assignment.

The single Route-scoped observer therefore remains alive across A/B/C and is released with the Route as intended.


The current tree contains two proven topic Routes selected from the HUB:

```text
Basic Flow
Readiness Showcase
```

There is intentionally no:

```text
Activities/ActivityContent_Basic_C.asset
Scenes/SCN_GameFlow_Basic_C.unity
```

`Activity_Basic_C` is deliberately content-less. In Basic Flow it proves a valid Activity without Activity-owned content. In Readiness Showcase it is reused as a neutral baseline between repeatable readiness tests.

## Basic Flow content roles

```text
Scenes/SCN_GameFlow_Basic.unity
  Route-owned environment / walls

  Activity-local content
    Visitors A -> Activity_Basic_A
    Visitors B -> Activity_Basic_B

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

Entering `Activity_Basic_C` proves the negative case: the previous Activity-owned scene is released and A/B local content is hidden while C remains a valid active Activity with no owned content profile.

## Readiness Showcase content roles

```text
Route_ReadinessShowcase
  Primary Scene -> SCN_GameFlow_Basic_Readiness
  Startup Activity -> Activity_Basic_C

Activity_Basic_C
  neutral baseline
  Observe Only
  no ActivityContentProfile
  shares CameraAssignment_GameFlow_Readiness with D/E in Readiness Showcase

Activity_Basic_D
  Wait Visible
  Fade With Loading
  ActivityContentReadiness

Activity_Basic_E
  Wait Covered
  Fade With Loading
  ActivityContentReadiness

ActivityContentReadiness
  -> SCN_GameFlow_Content_Readiness
  -> release on Activity change
```

`SCN_GameFlow_Content_Readiness.unity` owns the sample preparation content and the official `ActivityReadinessParticipant`. D and E intentionally use the same physical preparation so the product difference is the authored readiness policy rather than a different mechanic.

The Readiness Route menu uses `ActivityContentBinding` to constrain the demonstration to:

```text
C -> D -> C
C -> E -> C
```

while C is active, D/E controls are visible. While D or E is active, only the return-to-C control is visible. The controls still use normal `ActivityRequestTrigger` requests; visibility is not request authority.

One Route-scoped Camera adapter in `SCN_GameFlow_Basic_Readiness` maps C, D, and E to `CameraAssignment_GameFlow_Readiness`. Entering the Route activates the fixed Assignment; transitions among C/D/E are no-ops; Route exit clears it. Camera is not part of the readiness participant or its result.

Returning to C unloads `SCN_GameFlow_Content_Readiness`, so the next D or E request materializes fresh Activity-owned content and creates a fresh readiness occurrence. `D -> C -> D` and `E -> C -> E` are repeatable.

## Persistent presentation roles

`SCN_GameFlow_Persistence.unity` explicitly composes:

```text
Transition
  UnityFadeCurtainEffectAdapter

Loading
  UnityLoadingSurfaceAdapter
```

The proven presentation paths are:

```text
Route_Hub -> Route_BasicFlow
Route_Hub -> Route_ReadinessShowcase
  Fade cover/reveal
  Loading presentation

Activity A <-> B
  Seamless

Activity A/B -> C
  Fade
  no Loading presentation

Readiness C -> D
  Fade With Loading
  Wait Visible
  preparation visible after reveal

Readiness C -> E
  Fade With Loading
  Wait Covered
  Loading remains governed by readiness until Ready
  determinate readiness progress
```

The successful Readiness proof uses one Required participant. For Wait Covered the runtime reaches terminal Loading through `ActivityReadiness`, with Required `1/1` completed and `blockingIssues=0`.

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

Therefore the sample demonstrates explicit Play, no-request Preserve and explicit Silence under normal Route/Activity lifetime.

## Current closure

Consumer-proven in this materialization:

```text
Basic Route / Activity flow
Activity-local visibility
Activity-owned scene load/release
content-less Activity
Route Fade + Loading
Activity Seamless
Activity Fade
Activity Fade With Loading
Observe Only
Wait Visible
Wait Covered
Required readiness participant
participant-aware determinate Loading progress
readiness scene release/reentry
contextual BGM Play / Preserve / Silence
```

The intended Game Flow consumer proof is closed by these valid paths. Negative/invalid/interrupted/terminal failure cases remain technical QA responsibility and are not materialization targets for this Sample.

## Evolutionary materialization

Create additional assets only when a distinct positive consumer contract justifies them. Possible later feature-owned demonstrations include:

```text
Activity Restart when it teaches a real gameplay restart flow
additional Session Camera Assignment command coverage only where it teaches a new contract
additional Audio coverage only where it teaches a new contract
supporting Player configuration only if a scenario actually requires Player
```

Composition / Visibility, baseline Transition presentation and successful readiness waiting/progress do not require new scenario allocation; they are already demonstrated by the current Game Flow Showcase.

Exact future scenario allocation remains evolutionary. Do not create assets merely to mirror the ADR inventory or to duplicate negative-path QA.
