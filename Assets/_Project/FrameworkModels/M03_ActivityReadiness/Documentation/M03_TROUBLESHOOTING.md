# M03 Troubleshooting

## Activity Never Shows Waiting

Check:

```text
ActivityReadinessParticipant exists
Participant Id is not empty
Requiredness is not Unknown
PreparationStarted is wired to BeginPreparation
Activity scene is included by the content profile
```

Expected first consumer log:

```text
preparation='started'
```

If the participant is not discovered, inspect the Activity scene composition and the explicit roots used by the framework.

## Visual Moves but Activity Never Becomes Ready

The movement alone does not complete readiness.

Check that the coroutine reaches:

```csharp
readinessParticipant.CompletePreparation();
```

Expected logs:

```text
preparation='completed'
presentation='Ready'
```

Also confirm that `M03PreparationSequence` references the same participant that the runtime discovered.

## Activity Becomes Ready but the Display Does Not Change

Check `PF_M03_ReadinessDisplay`:

```text
ActivityReadinessEvents exists
Preparing → ShowPreparing
Ready → ShowReady
NotReady → ShowNotReady
```

The presenter must be in a loaded explicit scene root visible to the official presentation bridge.

## Ready Appears but Prepared Content Is Missing

The `preparedContent` reference belongs to the scene instance of `M03ReadinessPresenter`.

Open `M03_Activity_Add` and assign:

```text
M03ReadinessPresenter.Prepared Content
→ PF_M03_PreparedContent scene instance
```

Do not assign a prefab asset when the intended object is the concrete scene instance.

## Preparation Does Not Reset on Exit

Check:

```text
PreparationReleased
→ M03PreparationSequence.ReleasePreparation
```

The release method stops the active coroutine and restores the original visual position.

Also check the scene-level binding:

```text
PreparationReleased
→ M03ReadinessPresenter.ResetPresentation
```

## Leave Works but Return Does Nothing

Open `M03_Route` and inspect the concrete navigation instance.

The Return button must contain:

```text
ActivityRequestTrigger
targetActivity = Activity_M03_Preparation
reason = m03.return-to-preparation
```

The Button must invoke:

```text
ActivityRequestTrigger.RequestActivity
```

Run `Validate Re-entry`.

## Target Activity Reference Disappears

The operational targets are concrete overrides on the navigation instance in `M03_Route`.

Do not depend only on the prefab template to carry cross-asset Activity references.

Re-run:

```text
Create or Configure Re-entry
Validate Re-entry
```

Then inspect the Route-scene instance.

## Logs Show BaselineReady and NotReady Together

These are different layers:

```text
BaselineReady
  technical baseline is valid

NotReady / Preparing
  aggregate readiness is blocked by an authorable required participant
```

This is valid behavior, but the current diagnostics can make the distinction difficult to read.

## Successful Request Shows a Failed Lifecycle Stage

During return to the Preparation Activity, diagnostics can show:

```text
kind='Succeeded'
phase='CommittedNotReady'
Readiness:NotReady
lifecycleOperationFailedStages='1'
```

The Activity request succeeded. `NotReady` is the expected temporary readiness state while the participant prepares.

This is a package diagnostics finding: an expected authorable waiting state should not look like a technical lifecycle failure.

## Player Participation Warnings

The M03 intentionally has no Local Player configuration.

Messages such as:

```text
Player participation is not configured
Local Player provisioning is not configured
```

are expected for this isolated model and do not block Activity Readiness.

## When to Stop Debugging FIRSTGAME

Move the issue to package or QA investigation when:

- the participant reports `Completed` but aggregate readiness remains stale;
- a released occurrence can still publish completion;
- official presentation receives duplicate identical revisions;
- a successful readiness wait is classified as a technical failure;
- re-entry reuses stale participant state;
- required and optional semantics differ from the official contract.
