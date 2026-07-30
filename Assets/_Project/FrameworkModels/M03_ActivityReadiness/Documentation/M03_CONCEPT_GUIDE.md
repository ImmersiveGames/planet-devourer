# M03 Concept Guide

## The Core Idea

The M03 answers one question:

> How can an Activity be active but not ready yet?

A game often loads a scene before every gameplay dependency is prepared. For example:

- a room is still being assembled;
- enemies are still being spawned;
- a terminal is still booting;
- local data is still being prepared;
- an entry animation must finish;
- a required gameplay service is not ready yet.

The framework therefore separates:

```text
Activity exists and its scene is loaded
```

from:

```text
Activity is Ready for use
```

## The M03 Example

The model uses a moving object as a visible preparation.

```text
The object starts moving
→ preparation is still running
→ Activity remains NotReady

The object finishes moving
→ the coroutine calls CompletePreparation()
→ the framework marks the participant Completed
→ the Activity can become Ready
```

The movement is only a demonstration. A real game can replace it with any asynchronous or staged preparation.

## What Makes the Activity Wait

`ActivityReadinessParticipant` contributes to the aggregate readiness of the active Activity.

The M03 participant is configured as:

```text
Participant Id: m03.preparation
Requiredness: Required
Order: 0
```

`Required` means that this participant must complete before the aggregate Activity can be `Ready`.

The runtime begins the participant when the Activity is admitted. The participant changes to:

```text
Preparing
```

At that point the Activity aggregate becomes:

```text
NotReady
```

## What the Coroutine Does

`M03PreparationSequence.BeginPreparation()` starts a coroutine.

The coroutine:

1. records the starting visual position;
2. moves the visual toward the prepared position;
3. waits until the configured duration is complete;
4. logs `preparation='completed'`;
5. calls `readinessParticipant.CompletePreparation()`.

The important call is:

```csharp
readinessParticipant.CompletePreparation();
```

Without this call, the participant remains `Preparing`, even if the object visually reached its destination.

## Who Decides Ready

The coroutine does not decide the aggregate Activity state.

It only reports that one participant completed.

The framework then evaluates:

```text
technical baseline
+ required participants
+ optional participants
= aggregate Activity readiness
```

For the M03 happy path:

```text
BaselineReady
+ required participant Preparing
= NotReady
```

Then:

```text
BaselineReady
+ required participant Completed
= Ready
```

## Who Displays the State

`ActivityReadinessEvents` receives the official readiness snapshot published by the framework.

It raises one of these UnityEvents:

```text
Preparing
Ready
NotReady
```

`M03ReadinessPresenter` reacts to those events.

The presenter can:

- change labels;
- switch Waiting and Ready visuals;
- enable prepared content.

It must not calculate readiness and must not poll the participant.

## What Happens on Exit

When the Activity is replaced, the framework releases the tracked participant.

The participant invokes:

```text
PreparationReleased
```

`M03PreparationSequence.ReleasePreparation()` then:

- stops the coroutine if it is still running;
- restores the visual position;
- logs `preparation='released'`.

The Activity-owned scene is then unloaded.

## What Happens on Re-entry

When `Activity_M03_Preparation` is requested again:

- its additive scene is loaded again;
- a new participant instance is discovered;
- a new readiness occurrence begins;
- the visual preparation runs again;
- the Activity returns to `NotReady`;
- completion returns it to `Ready`.

The previous participant state is not reused.

## Responsibility Summary

| Element | Responsibility |
|---|---|
| Activity asset | Declares the Activity and its content profile |
| Content profile | Declares the additive scene |
| Readiness participant | Declares a readiness contribution |
| Preparation sequence | Performs the consumer-specific work |
| Framework runtime | Owns aggregate readiness |
| Readiness events | Publishes official presentation events |
| Presenter | Displays the official state |
| Prepared content | Represents content available after Ready |
| Intermission Activity | Proves real exit and re-entry |

## The Rule to Remember

```text
The object movement does not make the Activity Ready by itself.

The coroutine finishes the work and calls CompletePreparation().
The framework then decides whether the Activity is Ready.
```
