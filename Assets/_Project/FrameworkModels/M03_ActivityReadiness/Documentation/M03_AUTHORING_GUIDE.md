# M03 Authoring Guide

## Objective

Configure an Activity that remains `NotReady` while one required authorable participant is preparing.

This guide documents the actual M03 composition. It does not ask the user to recreate assets that already exist.

## Existing Chain

```text
GA_M03_Readiness
→ Route_M03_Readiness
→ Activity_M03_Preparation
→ ActivityContent_M03_Preparation
→ M03_Activity_Add
```

The re-entry flow adds:

```text
Activity_M03_Intermission
→ ActivityContent_M03_Intermission
→ M03_Intermission_Add
```

## Setup Helpers

The FIRSTGAME model includes local Editor tools under:

```text
Tools
→ Immersive Framework
→ FIRSTGAME
→ M03 Activity Readiness
```

Available operations:

```text
Create or Configure
Configure Existing Prefabs
Compose Existing Activity Scene
Set Active Game Application
Validate M03
Create or Configure Re-entry
Validate Re-entry
```

These tools are explicit, idempotent setup helpers for the demonstration. They do not replace the framework contracts and are not runtime authority.

## Manual Composition

### 1. Preparation prefab

Open:

```text
Prefabs/PF_M03_PreparationParticipant.prefab
```

Required components:

```text
Framework Components
└── ActivityReadinessParticipant

Bindings
└── M03PreparationSequence
```

Configure `ActivityReadinessParticipant`:

```text
Participant Id: m03.preparation
Requiredness: Required
Order: 0
```

Configure `M03PreparationSequence`:

```text
Readiness Participant:
  the ActivityReadinessParticipant from this prefab

Preparation Visual:
  Visual Placeholder

Preparation Duration:
  a short positive duration

Prepared Local Position:
  the final local position of the visual
```

Bind UnityEvents:

```text
Preparation Started
→ M03PreparationSequence.BeginPreparation

Preparation Released
→ M03PreparationSequence.ReleasePreparation
```

### 2. Readiness display prefab

Open:

```text
Prefabs/PF_M03_ReadinessDisplay.prefab
```

Required components:

```text
Framework Components
└── ActivityReadinessEvents

Bindings
└── M03ReadinessPresenter
```

Configure the presenter references:

```text
Status Label
Detail Label
Waiting Visual
Ready Visual
```

`Prepared Content` is a concrete scene-instance reference. Assign it after the prefab is placed in `M03_Activity_Add`.

Bind UnityEvents:

```text
Preparing
→ M03ReadinessPresenter.ShowPreparing

Ready
→ M03ReadinessPresenter.ShowReady

Not Ready
→ M03ReadinessPresenter.ShowNotReady
```

### 3. Prepared content prefab

`PF_M03_PreparedContent` is a consumer visual.

It should not contain readiness authority. It is enabled or disabled by the presenter.

### 4. Activity scene

Open:

```text
Scenes/M03_Activity_Add.unity
```

Expected composition:

```text
M03_Activity_Add_Root
├── Authored Visual Content
│   ├── PF_M03_PreparationParticipant
│   └── PF_M03_PreparedContent
├── Framework Mount
└── UI Mount
    └── PF_M03_ReadinessDisplay
```

On the scene instance of `M03ReadinessPresenter`, assign:

```text
Prepared Content
→ PF_M03_PreparedContent scene instance
```

Bind the concrete participant release to presentation reset:

```text
ActivityReadinessParticipant.PreparationReleased
→ M03ReadinessPresenter.ResetPresentation
```

### 5. Re-entry navigation

`PF_M03_ActivityNavigation` lives in `M03_Route`, not in an Activity-owned scene.

This preserves navigation while Activities are switched.

Concrete Route-scene bindings:

```text
Leave Preparation
  Target Activity: Activity_M03_Intermission
  Reason: m03.leave-preparation

Return to Preparation
  Target Activity: Activity_M03_Preparation
  Reason: m03.return-to-preparation
```

Each button invokes:

```text
ActivityRequestTrigger.RequestActivity
```

## Inspector Checklist

### Participant

- [ ] Participant Id is not empty.
- [ ] Requiredness is `Required`.
- [ ] The preparation sequence references the same participant.
- [ ] The preparation visual reference is assigned.
- [ ] Preparation duration is positive.
- [ ] `PreparationStarted` calls `BeginPreparation`.
- [ ] `PreparationReleased` calls `ReleasePreparation`.

### Display

- [ ] `ActivityReadinessEvents` exists.
- [ ] `M03ReadinessPresenter` exists.
- [ ] Status and detail labels are assigned.
- [ ] Waiting and Ready visuals are assigned.
- [ ] Preparing, Ready and NotReady events are wired.
- [ ] The scene presenter references the prepared-content scene instance.
- [ ] Release resets presentation.

### Route navigation

- [ ] Navigation is in `M03_Route`.
- [ ] Leave targets the Intermission Activity.
- [ ] Return targets the Preparation Activity.
- [ ] Both buttons call `RequestActivity`.
- [ ] Both reasons are explicit.

## Why the References Are Split

Reusable prefab references belong in the prefab.

Concrete cross-instance references belong in the scene composition.

For example:

```text
M03PreparationSequence → participant in same prefab
```

is reusable.

But:

```text
M03ReadinessPresenter → prepared content scene instance
```

depends on the concrete composition and therefore belongs to the scene instance.

## Validation

Run:

```text
Validate M03
Validate Re-entry
```

Expected static result:

```text
status='Passed'
```

Static validation proves configuration. It does not replace the Play Mode flow.
