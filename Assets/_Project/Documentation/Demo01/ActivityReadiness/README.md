# Demo 01 — M03 Activity Readiness

**Status:** FIRSTGAME product proof completed  
**Validation date:** 2026-08-04  
**Repository:** `ImmersiveGames/planet-devourer`  
**Documented Git baseline:** `a32e2ea8c620c22ae8072763490936eba4be952d`  
**Feature cut:** `FIRSTGAME-M03-READY-PROGRESS-01`

## Purpose

This demonstration compares the three Activity entry-readiness policies in one Route:

| Policy | Presentation | Expected behavior |
|---|---|---|
| `ObserveOnly` | Existing Activity presentation | Readiness is observed but does not retain entry cover or gate release. |
| `WaitVisible` | Activity content remains visible | Preparation is visible while the configured capabilities remain gated. |
| `WaitCovered` | `FadeWithLoading` | The target Activity remains covered until all Required readiness participants complete. |

The `WaitCovered` case is the production-like proof. It demonstrates that Loading progress is driven by framework readiness participants rather than by gameplay objects or FIRSTGAME scripts.

## Product flow

```text
Demo01StartScene
→ Activity Readiness Route
→ Observe Only
→ Intermission
→ Wait Visible or Wait Covered
→ Intermission
→ reenter another policy or repeat Wait Covered
```

For `WaitCovered`:

```text
Activity request
→ FadeWithLoading begins
→ technical Loading range completes below 100%
→ four Required participants complete independently
→ Optional participant remains pending
→ aggregate readiness becomes Ready
→ Loading reaches 100%
→ Loading hides
→ cover reveals the completed Activity
→ Intermission releases the occurrence and owned scene
```

## Primary assets

### Route and Activities

```text
Assets/_Project/Demo 01 - Routes and Activities/
  Data/Activity Readiness/
    Routes/RouteReadiness.asset
    Activities/Activity_Preparation.asset
    Activities/ActivityReadiness_WaitVisible.asset
    Activities/ActivityReadiness_WaitCovered.asset
    Activities/Profiles/ActivityContent_ReadinessWaitCovered.asset
```

### Scenes

```text
Assets/_Project/Demo 01 - Routes and Activities/
  Scenes/Activity Readiness/
    Activity_Readiness.unity
    Activity_Readiness_Intermission.unity
    ActivitiesContent/
      Activity_Readiness_Add.unity
      Activity_Readiness_WaitVisible.unity
      Activity_Readiness_WaitCovered.unity
```

### Reusable scenario and navigation

```text
Assets/_Project/Demo 01 - Routes and Activities/
  Prefabs/Activity Readiness/
    Activity Readiness Scenario.prefab
    Activity Readiness Scenario - Wait Covered.prefab
    Ui/Canvas_ActivityReadinessNavigation.prefab
```

### Persistent presentation

```text
Assets/_Project/
  Scenes/Shared/Shared_PersistentContent.unity
  Prefabs/Persistents/Persistent Presentation.prefab
```

## Wait Covered Activity configuration

```text
Activity Entry Readiness Policy = WaitCovered
Visual Transition Mode          = FadeWithLoading
Transition Gate Mode            = InputInteractionAndGameplay
Content Profile                 = ActivityContent_ReadinessWaitCovered
```

The Activity-owned scene contains one instance of:

```text
Activity Readiness Scenario - Wait Covered.prefab
```

The scene should not contain a second Loading Canvas, a `FrameworkRuntimeHost`, a bootstrap object, or a local Loading authority.

## Participant composition

The scenario uses four independent Required participants and one Optional participant.

| Order | Participant Id | Requiredness | Completion source |
|---:|---|---|---|
| 10 | `m03.wait-covered.chicken-01` | Required | Chicken 01 reaches its target |
| 20 | `m03.wait-covered.chicken-02` | Required | Chicken 02 reaches its target |
| 30 | `m03.wait-covered.chicken-03` | Required | Chicken 03 reaches its target |
| 40 | `m03.wait-covered.chicken-04` | Required | Chicken 04 reaches its target |
| 50 | `m03.wait-covered.optional` | Optional | Intentionally left pending |

Each Required participant owns one `ReadinessPreparationArea` configured with:

```text
Participant        = ActivityReadinessParticipant on the same GameObject
Preparation Volume = shared BoxCollider
Position Tolerance = 0.05
Subjects           = exactly one assigned Chicken
```

The framework counts participants, not Chickens. The Chicken-to-target sequence is consumer presentation and completion evidence only.

### Event wiring

Chicken 01 coordinates the shared visual sequence:

```text
Preparation Started
1. ReadinessPreparationSequence.BeginPreparation
2. its own ReadinessPreparationArea.BeginObservation

Preparation Released
1. its own ReadinessPreparationArea.ReleaseObservation
2. ReadinessPreparationSequence.ReleasePreparation
3. ReadinessProgressPresenter.ResetPresentation
```

Chicken 02, Chicken 03 and Chicken 04 each use only:

```text
Preparation Started
→ their own ReadinessPreparationArea.BeginObservation

Preparation Released
→ their own ReadinessPreparationArea.ReleaseObservation
```

The Optional participant has no preparation callbacks.

## Persistent Loading configuration

The persistent presentation uses one official:

```text
Immersive.Framework.Loading.UnityLoadingSurfaceAdapter
```

Expected configuration:

```text
progress-capable surface
determinate progress enabled
initial progress = 0
progress visible while Loading is visible
progress hidden and reset when Loading hides
hidden state applied on Awake
```

FIRSTGAME scripts do not resolve or update this adapter. The framework owns the Loading envelope and maps Required participant completion into the reserved readiness range.

## Running the demonstration

1. Open `Demo01StartScene.unity`.
2. Enter Play Mode with a clear Console.
3. Open the Activity Readiness Route.
4. Move to Intermission.
5. Select `Wait Covered`.
6. Observe four monotonic progress increments after the technical range.
7. Confirm the Activity is revealed only after the fourth Required completes.
8. Select Intermission.
9. Select `Wait Covered` again to validate a new occurrence.

## Expected diagnostics

A clean `WaitCovered` occurrence produces:

```text
preparation='started' steps='4'                     exactly once
area-observation='started' subjects='1'             four times
completion='submitted' subjects='1' occurrence='N'  four times
```

The terminal request diagnostics should include:

```text
kind='Succeeded'
activityReadiness='Ready'
loadingPresentation='SucceededWithUnitySurface'
blockingIssues='0'

loadingProgressSupported='True'
loadingProgressMode='Determinate'
loadingProgressValue='1.00'
loadingProgressPercent='100'
loadingProgressPhase='ActivityReadiness'

Required completed='4'
Required total='4'
Required pending='0'

Optional completed='0'
Optional total='1'
Optional pending='1'
```

The aggregate `ObserveOnly` scenario still legitimately logs `subjects='4'`. That log belongs to the original aggregate demonstration and must not be confused with the four independent `WaitCovered` areas.

## Reentry acceptance

A successful exit and reentry must show:

```text
first WaitCovered occurrence completes
→ Intermission becomes available
→ four observations release
→ shared preparation sequence releases once
→ WaitCovered scene unloads
→ second WaitCovered request creates a different occurrence
→ four Required participants complete again
→ Loading reaches 100%
→ request succeeds with no blocking issue
```

Play Mode evidence supplied for this closure showed successful `WaitCovered` occurrences `4` and `6`.

## Ownership boundaries

### FIRSTGAME owns

```text
Activity assets and Content Profile
scenario visuals
Chicken-to-target movement
thin participant completion bridge
navigation
persistent presentation configuration
short usage documentation
UX findings
```

### The framework owns

```text
participant discovery and occurrence capture
Required/Optional aggregation
readiness denominator
technical/readiness Loading envelope
100% publication
Loading hide ordering
cover/reveal ordering
capability gate retention and release
terminal diagnostics
```

FIRSTGAME must not:

```text
calculate authoritative Loading progress
write directly to the Loading surface
resolve a runtime host
perform global lookup
replace readiness authority
treat Optional as part of the successful denominator
```

## Scope limitations

This is a happy-path consumer demonstration.

Failure, invalidation, cancellation, late completion and duplicate terminal behavior belong to `QAFramework`. The initial contract is participant-granular: it does not provide weights or continuous progress inside one participant.

The local `ReadinessProgressPresenter` is explanatory presentation. It is not the persistent Loading authority.

## Reuse checklist

```text
[ ] Create an Activity and Activity Content Profile.
[ ] Select the required entry-readiness policy.
[ ] Use FadeWithLoading for covered determinate progress.
[ ] Use InputInteractionAndGameplay when normal gameplay must remain gated.
[ ] Author independent Required participants for independent progress increments.
[ ] Keep Participant Id values unique.
[ ] Decide whether Optional participants are diagnostic only.
[ ] Connect each completion bridge to the participant on the same GameObject.
[ ] Configure one persistent progress-capable Loading adapter.
[ ] Add navigation reachability for entry and exit.
[ ] Test exit and reentry as separate occurrences.
[ ] Confirm 100% precedes Loading Hide and reveal.
```
