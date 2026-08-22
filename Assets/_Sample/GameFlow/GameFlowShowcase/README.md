# Game Flow Showcase

## Demonstrates

One coherent `GameApplication` containing a sample HUB plus compatible Game Flow scenarios.

The current sample has two proven Route-level scenarios:

```text
Game Flow HUB
  ├─ Basic Flow
  │    A / B / C
  │
  └─ Readiness Showcase
       C = neutral baseline
       C -> D -> C = Wait Visible
       C -> E -> C = Wait Covered
```

The HUB is sample navigation. It is not Framework authority or gameplay progression.

## Run

```text
Application
  GameApplication_GameFlow.asset

Entry scene
  Scenes/SCN_GameFlow_Hub.unity
```

Set `GameApplication_GameFlow.asset` Active through the official Framework Inspector surface, then enter Play Mode.

## Persistent presentation

`SCN_GameFlow_Persistence.unity` explicitly composes the presentation adapters used by both scenarios:

```text
Transition
  UnityFadeCurtainEffectAdapter

Loading
  UnityLoadingSurfaceAdapter
```

The runtime resolves these adapters from Persistent Content. Route and Activity content do not create or own persistent presentation surfaces.

---

# Basic Flow

## Topology

```text
Route_Hub -> Route_BasicFlow
  Route transition = Fade cover/reveal
  Route loading presentation = enabled

Route_BasicFlow
  Primary Scene = SCN_GameFlow_Basic
  Startup Activity = Activity_Basic_A

Activity_Basic_A <-> Activity_Basic_B
  target policy = Seamless

Activity_Basic_A / B -> Activity_Basic_C
  target policy = Fade

Activity_Basic_C -> Activity_Basic_A / B
  target policy = Seamless
```

`SCN_GameFlow_Basic.unity` remains Route-owned while the Activity changes.

The scenario deliberately combines:

```text
Route-owned content
  environment / walls

Activity-local content
  Visitors A -> ActivityContentBinding -> Activity_Basic_A
  Visitors B -> ActivityContentBinding -> Activity_Basic_B

Activity-owned scene content
  Activity A -> SCN_GameFlow_Basic_A
  Activity B -> SCN_GameFlow_Basic_B

Content-less Activity
  Activity C -> no ActivityContentProfile / no Activity-owned scene
```

Activity-local visibility is not Activity scene materialization. Activity C proves that an Activity may be valid without owned content.

## Basic presentation proof

```text
HUB -> Basic Flow
  Fade cover/reveal
  Loading presentation

A <-> B
  Seamless
  Activity scene composition/release still occurs
  visual Transition and Loading skipped by target policy

A/B -> C
  Fade cover/reveal
  canonical Loading presentation skipped by Fade policy
  previous Activity-owned scene released

C -> A/B
  Seamless
  target Activity-owned scene materialized
```

The proven paths complete with `blockingIssues=0`.

## BGM proof

Explicit Play/Silence:

```text
Route_Hub Silence
  -> Activity_Basic_A BGM_Floresta
  -> Activity_Basic_B BGM_Gelo
  -> Route_Hub Silence
```

No-request preservation:

```text
Activity_Basic_A BGM
  -> Activity_Basic_C publishes no new BGM intent
  -> Activity A BGM remains confirmed

Activity_Basic_B BGM
  -> Activity_Basic_C publishes no new BGM intent
  -> Activity B BGM remains confirmed
```

Owner exit preserves the confirmed BGM presentation. Only an explicit destination Play or Silence intent changes it.

---

# Readiness Showcase

## Purpose

`Route_ReadinessShowcase` isolates the successful Activity Entry Readiness behaviors from the Basic Flow A/B/C demonstration.

The comparison uses the same preparation content for both waiting policies. The deliberate variable is the Activity Entry Readiness policy:

```text
D = Wait Visible
E = Wait Covered
```

## Topology

```text
Route_ReadinessShowcase
  Primary Scene = SCN_GameFlow_Basic_Readiness
  Startup Activity = Activity_Basic_C

Activity_Basic_C
  ActivityContentProfile = None
  Activity-owned scene = None
  Entry Readiness = Observe Only
  role in this Route = neutral test baseline

Activity_Basic_D
  ActivityContentProfile = ActivityContentReadiness
  Entry Readiness = Wait Visible
  Visual Transition = Fade With Loading
  Gate = Input Interaction And Gameplay

Activity_Basic_E
  ActivityContentProfile = ActivityContentReadiness
  Entry Readiness = Wait Covered
  Visual Transition = Fade With Loading
  Gate = Input Interaction And Gameplay

ActivityContentReadiness
  -> SCN_GameFlow_Content_Readiness
  -> released on Activity change
```

`SCN_GameFlow_Content_Readiness.unity` contains the sample-owned preparation plus the official readiness participant. The preparation controller moves the visitors; completion calls the public `ActivityReadinessParticipant.CompletePreparation()` surface. The sample does not create occurrences or own Framework readiness state.

## Neutral baseline and repeatability

The Readiness menu deliberately does not allow direct `D -> E` or `E -> D` requests.

`ActivityContentBinding` controls only which navigation controls are visible:

```text
while C is active
  Wait Visible button = visible
  Wait Covered button = visible
  Return/Reset button = hidden

while D or E is active
  Wait Visible button = hidden
  Wait Covered button = hidden
  Return/Reset button = visible
```

The Return/Reset control is a normal `ActivityRequestTrigger` targeting C. It is not Framework Reset/Restart.

The canonical sample cycles are:

```text
C -> D -> C
C -> E -> C

repeatable:
C -> D -> C -> D
C -> E -> C -> E
```

Returning to C releases `SCN_GameFlow_Content_Readiness`. The next readiness test therefore materializes the Activity-owned scene again and starts a fresh readiness occurrence.

This is intentional because D and E share the same Activity content. Returning through a content-less baseline prevents a previous test from turning the next test into an `AlreadyLoaded` path.

## Wait Visible — Activity D

Expected and proven behavior:

```text
C
  -> request D
  -> Fade cover
  -> SCN_GameFlow_Content_Readiness materializes
  -> Required readiness participant begins Preparing
  -> reveal occurs while preparation may still be running
  -> preparation remains observable
  -> participant completes
  -> Activity readiness becomes Ready
  -> capability gate releases
  -> request succeeds
```

Observed consumer evidence includes:

```text
activitySceneComposition = Succeeded
activitySceneCompositionLoaded = 1
activitySceneCompositionAlreadyLoaded = 0
activityReadiness = Ready
loading = SucceededWithUnitySurface
blockingIssues = 0
```

For this policy the final Loading progress phase is the Activity transition rather than a readiness-held covered phase, which is expected for `Wait Visible`.

## Wait Covered — Activity E

Expected and proven behavior:

```text
C
  -> request E
  -> Fade cover
  -> Loading shown
  -> SCN_GameFlow_Content_Readiness materializes
  -> Required readiness participant begins Preparing
  -> visual cover remains while readiness is not Ready
  -> participant completes
  -> Loading reaches terminal readiness progress
  -> Loading hides
  -> reveal occurs
  -> capability gate releases
  -> request succeeds
```

Observed consumer evidence includes:

```text
activitySceneComposition = Succeeded
activitySceneCompositionLoaded = 1
activitySceneCompositionAlreadyLoaded = 0
activityReadiness = Ready
loading = SucceededWithUnitySurface
loadingProgressMode = Determinate
loadingProgressPhase = ActivityReadiness
Required completed = 1
Required total = 1
Required pending = 0
blockingIssues = 0
```

This proves the participant-aware Loading path with one Required readiness contribution.

## Release / reentry proof

Returning from D or E to C releases the shared Activity-owned readiness scene:

```text
SceneReleasing
  scene = SCN_GameFlow_Content_Readiness
  reason = scene-unload

activitySceneRelease = Succeeded
activityScenesReleased = 1
```

Reentry then loads the scene again instead of reusing an already-loaded copy. `E -> C -> E` and the equivalent D cycle are repeatable.

## What this Readiness scenario closes

The successful consumer path now proves:

```text
Wait Visible authoring and reveal behavior
Wait Covered authoring and covered waiting
Fade With Loading under readiness
Required ActivityReadinessParticipant contribution
participant-aware determinate Loading progress
fresh occurrence on reentry
Activity-owned readiness scene release/reload
ActivityContentBinding-controlled sample navigation
blockingIssues = 0 on the successful paths
```

It does **not** claim terminal readiness failure/recovery. Failure, cancellation and recovery remain a separate later scenario.

---

# Inspect

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
```

There is intentionally no `ActivityContent_Basic_C.asset` and no `SCN_GameFlow_Basic_C.unity`.

The `GameApplication` keeps Player Session disabled because these Game Flow and Activity Readiness demonstrations do not require a Player.

# Framework concepts currently consumer-proven

```text
GameApplication ownership
Persistent Content
persistent Transition presentation
persistent Loading presentation
Route ownership
Route with no Startup Activity
Route with Startup Activity
mandatory covered Route switch
Activity-local content / visibility
Activity-owned scene composition
valid content-less Activity
explicit Route request
explicit Activity request
Activity Seamless presentation
Activity Fade presentation
Activity Fade With Loading presentation
Activity Entry Readiness Observe Only
Activity Entry Readiness Wait Visible
Activity Entry Readiness Wait Covered
Required readiness participant contribution
participant-aware determinate Loading progress
readiness occurrence reentry
Activity-owned content release/reload between readiness tests
contextual Route / Activity BGM intent
BGM no-request preservation
teardown and return to Activity None
```

# Evolutionary scenarios

Composition / Visibility, baseline Transition presentation and successful Readiness waiting/progress are no longer separate pending scenarios; they are demonstrated in the current coherent Game Flow Showcase.

Remaining candidates include:

```text
terminal Readiness failure / recovery
Restart / Recovery
contextual Camera coverage where natural
additional Audio coverage only where it teaches a new contract
supporting Player configuration only when a scenario actually requires Player
```

Do not add new scenarios merely to mirror the ADR inventory. Each new scenario must teach a distinct consumer contract.
