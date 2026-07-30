# M03 Play Mode Validation Guide

## Purpose

Prove the complete happy path:

```text
enter
→ Waiting
→ Ready
→ exit
→ Intermission
→ re-enter
→ Waiting
→ Ready
```

## Before Play Mode

- [ ] `GA_M03_Readiness` is the active Game Application.
- [ ] `Validate M03` passes.
- [ ] `Validate Re-entry` passes.
- [ ] `M03_Boot` is open.
- [ ] Console is clear.

## Phase 1 — Initial Entry

Enter Play Mode and observe:

```text
M03_Boot
→ M03_Route
→ Activity_M03_Preparation
→ M03_Activity_Add
```

Expected logs:

```text
[FIRSTGAME_M03_ACTIVITY_READINESS] preparation='started'
[FIRSTGAME_M03_ACTIVITY_READINESS] presentation='Preparing'
```

Expected runtime state:

```text
participant = Preparing
aggregate Activity readiness = NotReady
```

Expected visual state:

```text
Waiting label visible
Waiting visual active
Ready visual inactive
prepared content unavailable
preparation visual moving
```

## Phase 2 — Completion

Wait for the visual sequence to finish.

Expected logs:

```text
[FIRSTGAME_M03_ACTIVITY_READINESS] preparation='completed'
[FIRSTGAME_M03_ACTIVITY_READINESS] presentation='Ready'
```

Expected runtime state:

```text
participant = Completed
aggregate Activity readiness = Ready
```

Expected visual state:

```text
Ready label visible
Waiting visual inactive
Ready visual active
prepared content available
```

## Phase 3 — Exit

Click:

```text
Leave Preparation
```

Expected logs include:

```text
reason='m03.leave-preparation'
preparation='released'
previousActivity='Activity M03 Preparation'
targetActivity='Activity M03 Intermission'
activitySceneRelease='Succeeded'
activityScenesReleased='1'
```

Expected scene state:

```text
M03_Route remains loaded
M03_Activity_Add is unloaded
M03_Intermission_Add is loaded
navigation remains available
```

## Phase 4 — Re-entry

Click:

```text
Return to Preparation
```

Expected logs include:

```text
reason='m03.return-to-preparation'
previousActivity='Activity M03 Intermission'
targetActivity='Activity M03 Preparation'
activitySceneComposition='Succeeded'
activitySceneRelease='Succeeded'
```

The new occurrence must then produce:

```text
preparation='started'
presentation='Preparing'
preparation='completed'
presentation='Ready'
```

Expected scene state:

```text
M03_Intermission_Add is unloaded
M03_Activity_Add is loaded again
```

## Pass Criteria

The model passes when the observed order is:

```text
Occurrence 1:
  started
  Preparing
  completed
  Ready

Exit:
  released
  Intermission active

Occurrence 2:
  started
  Preparing
  completed
  Ready
```

Also required:

- [ ] no unhandled exception;
- [ ] no silent fallback;
- [ ] both Activity requests succeed;
- [ ] one Activity-owned scene is released on each switch;
- [ ] the Route remains active;
- [ ] the second preparation is new;
- [ ] the presenter receives official readiness events;
- [ ] no polling or global lookup is introduced.

## Expected NotReady Blocking Issue

During preparation, a request or bootstrap summary can report:

```text
activityReadiness='NotReady'
blockingIssues='1'
```

For this happy path, the blocker is the required authorable participant in `Preparing`.

Verify that technical subsystems remain clear:

```text
activityContentExecutionBlockingIssues='0'
activitySceneCompositionBlockingIssues='0'
```

The readiness blocker is intentional and temporary.

## Proven Result

The validated FIRSTGAME run demonstrated:

```text
initial Waiting → Ready
release on exit
successful switch to Intermission
successful return request
new CommittedNotReady occurrence
second Waiting → Ready
```
