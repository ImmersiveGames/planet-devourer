# FIRSTGAME — Test Scenarios

Status: Current validation protocol  
Last updated: 2026-07-28

## Status values

```text
Not Run
Running
Passed
Failed
Blocked
```

A scenario is `Passed` only after execution in Unity and recorded evidence.

## Evidence template

```text
Scenario:
Date:
Unity version:
FIRSTGAME SHA:
Framework SHA:
Starting scene:
Steps:
Observed result:
Diagnostics:
Status:
UX notes:
```

Current validated references:

```text
FIRSTGAME
  116225d50a3c6af976355715d3216c0cb80852eb

Framework
  bdb76a06a3b75adc9ac7fa5d3e63fbe457ed5ae2

QA
  64f900a5c26ab07ad37f2e7d6e578e8efcfb72a4
```

---

## TS-01 — Application boot and focused Route navigation

### Goal

Prove that Player-independent menu navigation can enter and leave the focused Scene-Provided fixture.

### Steps

```text
Menu
→ Player Local Test
→ SceneProvidedGameplay
→ Menu
```

### Expected evidence

- boot succeeds;
- Route request succeeds;
- primary scene changes;
- Activity becomes Ready;
- return to Menu succeeds;
- no blocking issue.

### Status

`Passed` for the focused `Player Local Test` Route.

The general `Start Game` branch is not reclassified by this specific freeze.

---

## TS-02 — Persistent Content and physical Camera Output

### Goal

Prove that one Session-owned physical Camera Output remains authoritative while the Player publishes contextual gameplay Camera evidence.

### Expected evidence

- one physical Camera Output;
- no duplicate Unity Camera or AudioListener;
- Player Camera request active during Gameplay;
- request released on exit;
- Menu output restored;
- reentry does not duplicate the request.

### Status

`Passed` in the Scene-Provided baseline.

---

## TS-03 — Application-only Pause

### Goal

Prove authored Pause without a Player binding.

### Status

`Not Run` by the `PLAYER-DIAG-1` freeze.

This scenario remains separate from Player-bound Pause.

---

## TS-04 — Player-bound Pause and input Gate

### Goal

Prove that physical Pause input belongs to the admitted Player and gameplay input restores correctly.

### Expected evidence

- one eligible Player binding;
- Pause toggles logical state;
- gameplay action map is gated;
- Pause/global action remains available;
- Resume restores gameplay input;
- exit releases binding.

### Status

`Passed` in the Scene-Provided comparison baseline.

---

## TS-05 — Scene-Provided Player admission

### Goal

Prove the Route Primary Scene Player composition.

### Steps

1. Enter through `Player Local Test`.
2. Confirm one active admission.
3. Confirm `PlayerSlot:player.1`.
4. Confirm the scene-existing Host is used.
5. Confirm the nested Actor is adopted.
6. Confirm Activity readiness.
7. Exit to Menu.
8. Confirm release.
9. Reenter.
10. Confirm no residual admission or duplicate Player.

### Recorded evidence

During Gameplay:

```text
Active Count = 1
Occupied Slot Count = 1
Last Operation = AdmitSceneLocalPlayer
Last Status = SucceededAdmitted
Last Slot = PlayerSlot:player.1
Host Evidence Present = Yes
```

After release:

```text
Active Count = 0
Occupied Slot Count = 0
Last Operation = ReleaseSceneLocalPlayer
Last Status = SucceededReleased
Release Succeeded = Yes
Host Evidence Present = No
```

### Status

`Passed`.

---

## TS-05R — Scene-Provided Activity Restart and teardown

### Goal

Prove Activity Restart, readmission/reentry, release and runtime teardown without invalid identity formatting.

### Steps

```text
Menu
→ Gameplay
→ move Player
→ Activity Restart
→ verify Player remains valid
→ Menu
→ inspect release snapshot
→ Stop Play Mode
```

### Recorded evidence

```text
Activity Restart status = Succeeded
resetStatus = Succeeded
resetSubjects = 2
resetParticipants = 2
clearStatus = Succeeded
reentryStatus = Succeeded
blockingIssues = 0
```

The tester manually confirmed valid readmission after Restart.

Not reproduced:

```text
ArgumentException: Framework identity value must be valid
```

### Status

`Passed`.

---

## TS-06 — Scene-Provided gameplay Camera

### Goal

Prove the Player gameplay Camera through the persistent physical output.

### Expected evidence

- explicit Follow/LookAt targets;
- one local Camera rig request;
- one persistent physical output;
- Camera follows the Player;
- Pause/Resume does not corrupt the output;
- exit releases the request;
- reentry remains stable.

### Status

`Passed`.

---

## TS-07 — Manager-Provisioned Player

### Goal

Prove runtime-created local Player provisioning through the official path.

### Preconditions

```text
dedicated Route and scene
manual-join PlayerInputManager
LocalPlayerProvisioningAuthoring
LocalPlayerProvisioningHostRegistration
Player prefab with empty Actor Mount
authorized join command
```

### Required proof

- Slot reserved before provisioning;
- exactly one Host created;
- Host validates and commits;
- Actor selected/prepared;
- failed join rolls back Slot and Host evidence;
- exit permits Slot reuse;
- no `playerIndex` identity authority.

### Status

`Not Run` — next consumer integration scenario.

---

## TS-08 — Session-Persistent Player

### Goal

Reserved future proof for a Logical Player whose Session identity survives Route and Activity changes.

### Status

`Blocked` by Framework.

Do not create a FIRSTGAME workaround. The package must first provide official authoring, admission, lifetime and reconciliation contracts.

---

## Open technical finding

Reset unload sequence may include:

```text
SceneReleasing unregister
→ update-retry register
→ on-disable unregister
```

This finding is outside Player scenario acceptance and requires a separate Reset lifecycle test.
