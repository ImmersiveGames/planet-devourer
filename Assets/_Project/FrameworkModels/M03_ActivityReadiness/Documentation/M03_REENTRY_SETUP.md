# M03 Re-entry Setup and Validation

## Why Re-entry Exists

Requesting the already active Activity does not prove reset or a new readiness occurrence.

A real re-entry requires:

```text
leave Activity_M03_Preparation
→ enter another Activity
→ release the previous participant and scene
→ request Activity_M03_Preparation again
```

The M03 uses a neutral Activity:

```text
Activity_M03_Intermission
```

Its purpose is only to create a valid exit and return path.

## Re-entry Assets

```text
Activities/
  Activity_M03_Intermission.asset

Profiles/
  ActivityContent_M03_Intermission.asset

Scenes/
  M03_Intermission_Add.unity

Prefabs/
  PF_M03_ActivityNavigation.prefab
```

## Navigation Ownership

The navigation prefab is instantiated in:

```text
M03_Route
```

The Route remains active while Activity-owned scenes are replaced.

This prevents the buttons from disappearing during the switch.

## Concrete Bindings

On the `M03_Route` scene instance:

```text
Leave Preparation
  ActivityRequestTrigger.targetActivity:
    Activity_M03_Intermission

  reason:
    m03.leave-preparation
```

```text
Return to Preparation
  ActivityRequestTrigger.targetActivity:
    Activity_M03_Preparation

  reason:
    m03.return-to-preparation
```

The buttons invoke:

```text
ActivityRequestTrigger.RequestActivity()
```

## Expected Exit Order

```text
Intermission scene is composed
→ current authorable readiness is invalidated
→ participant is Released
→ PreparationReleased event runs
→ preparation visual is restored
→ M03_Activity_Add is released
→ Activity_M03_Intermission becomes current
```

The exact internal scheduling can interleave scene lifecycle messages, but the committed result must show:

```text
previousActivity = Activity M03 Preparation
targetActivity = Activity M03 Intermission
activitySceneComposition = Succeeded
activitySceneRelease = Succeeded
```

## Expected Return Order

```text
M03_Activity_Add is composed again
→ new participant instance is discovered
→ new readiness occurrence starts
→ participant becomes Preparing
→ aggregate becomes NotReady
→ coroutine completes
→ participant becomes Completed
→ aggregate becomes Ready
```

## Acceptance

```text
Preparation occurrence 1
→ Waiting
→ Ready
→ release
→ Intermission
→ Preparation occurrence 2
→ Waiting
→ Ready
```

The model fails re-entry validation when:

- the previous participant is not released;
- the old preparation continues after exit;
- the previous scene remains loaded unexpectedly;
- the return request is ignored as already active;
- the second occurrence starts as Ready without preparing;
- stale completion from the previous occurrence changes the new state.
