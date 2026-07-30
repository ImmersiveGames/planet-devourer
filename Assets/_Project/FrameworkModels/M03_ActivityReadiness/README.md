# M03 Activity Readiness

## Purpose

This model demonstrates an Activity that waits for a valid visual preparation before becoming Ready.

## What This Model Demonstrates

- Required `ActivityReadinessParticipant`;
- preparation started by the runtime;
- official presentation through `ActivityReadinessEvents`;
- aggregate Waiting/NotReady;
- `CompletePreparation()` when the visual sequence finishes;
- Ready, cleanup, and re-entry.

## Required Package Features

- `ActivityReadinessParticipant`;
- `ActivityReadinessEvents`;
- `GameApplicationAsset`;
- `RouteAsset`;
- `ActivityAsset`;
- official Route and Activity navigation triggers used by the selected composition.

## Assets To Create Manually

- `Application/GA_M03_Readiness.asset`
- `Routes/Route_M03_Readiness.asset`
- `Activities/Activity_M03_Preparation.asset`

## Scenes To Create Manually

- `Scenes/M03_Boot.unity`
- `Scenes/M03_Route.unity`
- `Scenes/M03_Activity_Add.unity`

## Prefabs To Assemble Manually

- `Prefabs/PF_M03_PreparationParticipant.prefab`
- `Prefabs/PF_M03_ReadinessDisplay.prefab`
- `Prefabs/PF_M03_PreparedContent.prefab`

## Recommended Prefab Composition

`PF_M03_PreparationParticipant`

```text
Prefab Root
├── Preparation Visual
├── ActivityReadinessParticipant
└── M03PreparationSequence
```

Wire `ActivityReadinessParticipant.PreparationStarted` to `M03PreparationSequence.BeginPreparation` and `PreparationReleased` to `M03PreparationSequence.ReleasePreparation`. The sequence must reference that same participant explicitly.

`PF_M03_ReadinessDisplay`

```text
Prefab Root
├── Waiting Visual
├── Ready Visual
├── Status Label
├── ActivityReadinessEvents
└── M03ReadinessPresenter
```

Wire the official UnityEvents manually:

- `ActivityReadinessEvents.preparing` → `M03ReadinessPresenter.ShowPreparing`
- `ActivityReadinessEvents.ready` → `M03ReadinessPresenter.ShowReady`
- `ActivityReadinessEvents.notReady` → `M03ReadinessPresenter.ShowNotReady`
- `ActivityReadinessParticipant.PreparationReleased` → `M03ReadinessPresenter.ResetPresentation`

`PF_M03_PreparedContent` is simple visual or interactive content, initially disabled and enabled only by `ShowReady`.

## Setup

1. Create the Game Application.
2. Create the Route.
3. Create the Activity.
4. Associate the scenes.
5. Place the three prefabs in the Activity additive scene.
6. Set a Participant Id.
7. Set Requiredness to Required.
8. Wire `PreparationStarted` and `PreparationReleased`.
9. Wire `ActivityReadinessEvents` to the presenter.
10. Validate every Inspector reference.
11. Save the assets and scenes.
12. Enter Play Mode.

## Inspector Checklist

- Participant Id is filled.
- Requiredness is Required.
- `M03PreparationSequence` points to the same participant.
- The preparation visual is configured.
- `ActivityReadinessEvents` has its callbacks.
- The presenter has labels and visual roots.
- Prepared content starts unavailable.
- No reference belongs to another model.

## Play Mode Flow

```text
Boot → Route → Activity enters → Waiting → visual preparation → Ready
→ prepared content available → leave Route or Activity → re-enter → preparation repeats
```

## Expected Result

- Waiting appears immediately after Activity entry.
- Prepared content remains unavailable.
- The visual sequence finishes.
- Ready appears and prepared content becomes available.
- No new Activity request is executed.
- Exit resets the visual.
- Re-entry repeats the flow.

## Reusable Pieces

- `ActivityReadinessParticipant` is framework functionality.
- `ActivityReadinessEvents` is framework functionality.
- `M03PreparationSequence` is a consumer example.
- `M03ReadinessPresenter` is consumer presentation.
- Visuals can change without changing the contract.

## UX Findings

| Area | Observation | Impact | Destination |
|---|---|---|---|
| Creation |  |  |  |
| Inspector |  |  |  |
| Composition |  |  |  |
| Runtime |  |  |  |
| Diagnostics |  |  |  |
| Reuse |  |  |  |

- Is the participant → readiness relation clear?
- Is Requiredness easy to find?
- Are the required UnityEvents obvious?
- Is it clear that the presenter is not the authority?
- Is the Waiting reason visible without opening code?
- Does re-entry require additional configuration?
- Would a Composer reduce errors without hiding contracts?

## QA Follow-ups

Record, without implementing, the following scenarios in `C:\Projetos\QAFramework`:

- required participant failure;
- optional participant failure;
- participant missing;
- duplicate completion;
- late completion;
- stale occurrence;
- replacement during preparation.
