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

A scenario is `Passed` only after its result is executed in Unity and recorded with the package revision, FIRSTGAME revision and relevant diagnostic evidence.

## Evidence record template

```text
Scenario:
Date:
Unity version:
FIRSTGAME commit:
Framework commit/package version:
Starting scene:
Steps executed:
Expected result:
Observed result:
Diagnostics inspected:
Status:
Notes / UX friction:
```

---

## TS-01 — Application boot and menu Routes

### Goal

Prove that Player-independent navigation remains usable before any Player is required.

### Preconditions

```text
FG_UIGlobal and FG_Menu enabled in Build Settings
FG_GameApplication assigned to bootstrap
startup Route configured
```

### Steps

1. Start Play Mode from the supported application entry.
2. Confirm the menu becomes visible.
3. Select `Start Game`.
4. Confirm the general gameplay Route enters.
5. Return to the menu through the authored framework path.
6. Select `Player Local Test`.
7. Confirm `FG_PlayerSceneProvider` loads `SceneProvidedGameplay`.
8. Return to the menu again.

### Expected evidence

```text
no Player is required to navigate Routes
route request result is explicit
primary scenes load and release correctly
menu is restored after exit
second navigation does not retain previous Route scope
```

### Current status

`Not Run` in this document.

---

## TS-02 — Persistent Content and Camera Output

### Goal

Prove that the physical Camera Output remains Session-owned while contextual Camera requests change.

### Preconditions

```text
Conteiner Scene configured as Persistent Content
Camera Output authoring valid
one general or Player Camera request available
```

### Steps

1. Enter Play Mode and inspect the persistent Camera Output.
2. Enter the general gameplay Route.
3. Enter the Scene-Provided Player Route.
4. Confirm the same physical Camera Output remains authoritative.
5. Confirm the active contextual request changes when expected.
6. Exit to the menu.
7. Confirm the prior/default Camera state is restored.
8. Repeat the transition.

### Expected evidence

```text
one physical Camera Output
no Camera.main lookup
no duplicate Unity Camera or AudioListener
scoped request selected while its scope is active
previous/default state restored on release
second entry does not duplicate request identity
```

### Current status

`Not Run` in this document.

---

## TS-03 — Application-only Pause

### Goal

Prove that authored Pause controls can change logical Pause without an admitted Player binding.

### Preconditions

```text
PauseRequestTrigger composed in a valid lifecycle scope
application-only execution available
no Player required for the selected test
```

### Steps

1. Enter a context with no admitted Player requirement.
2. Invoke the authored Pause control.
3. Confirm logical Pause and presentation change.
4. Confirm `Time.timeScale` follows the configured policy.
5. Resume.
6. Exit and re-enter the context.

### Expected evidence

```text
Pause request port is bound
LastExecutionMode reports ApplicationOnly
no Player action map mutation is claimed
resume restores the prior state
scope exit releases the request binding
```

### Current status

`Not Run` in this document.

---

## TS-04 — Player-bound Pause and input gate

### Goal

Prove that physical Pause input belongs to the admitted official Player and that gameplay input is gated/restored correctly.

### Preconditions

```text
Player_SceneProvided_With_Pause or equivalent composed variant
PausePlayerInputBinding references the PlayerInput
UnityPlayerInputGateAdapter references the gameplay action map
Scene-Provided Player is admitted
```

### Steps

1. Enter the focused Player Route.
2. Confirm movement input is initially accepted.
3. Invoke Pause through the Player's authored Pause action.
4. Confirm logical Pause activates.
5. Confirm gameplay input is blocked according to policy.
6. Confirm the Pause/global action remains available.
7. Resume.
8. Confirm gameplay input is restored.
9. Exit the Route and verify bindings release.

### Expected evidence

```text
one eligible Player binding
PausePlayerInputBinding reports bound
UnityPlayerInputGateAdapter changes only the intended action map
no second global Player exists
previous input state is restored
```

### Current status

`Not Run` in this document.

---

## TS-05 — Scene-Provided Player admission

### Goal

Prove the current Route Primary Scene Player composition.

### Preconditions

```text
FG_PlayerSceneProvider primary scene is SceneProvidedGameplay
Activity_PlayerLocalProvider projects the configured Slot
Player_SceneProvided authoring validates
nested Actor prefab matches ActorProfile.LogicalActorHostPrefab
```

### Steps

1. Enter through `Player Local Test`.
2. Inspect Slot reservation/admission evidence.
3. Confirm exactly one Logical Player is joined.
4. Confirm the configured `PlayerSlotId` is used.
5. Confirm the scene-existing Local Player Host is admitted, not provisioned again.
6. Confirm the nested Logical Actor is adopted, not duplicated.
7. Confirm Activity readiness reaches the configured requirement.
8. Exit the Route.
9. Confirm contextual Actor, host and Slot evidence release in order.
10. Enter again and verify no duplicate admission.

### Expected evidence

```text
Scene-Provided source recorded
external scene ownership preserved
one Slot assignment
one Host identity
one current Actor correlation
no PlayerInputManager provisioning call
no object-name or playerIndex identity inference
```

### Current status

`Running` — this is the current focused integration scenario.

---

## TS-06 — Scene-Provided Player gameplay Camera

### Goal

Prove the Player Camera request through the persistent output authority.

### Preconditions

```text
TS-05 admission succeeds
Player_SceneProvided_With_Camera is instantiated
CameraRigComposer validates
PlayerGameplayCameraAuthoring references that rig
persistent Camera Output is valid
```

### Steps

1. Enter the focused Player Route.
2. Confirm CameraRigComposer targets resolve to the Player Actor hierarchy.
3. Confirm the local Cinemachine Camera exists and is not the physical output.
4. Confirm Player Camera eligibility becomes available only after required Player/Actor evidence.
5. Move the Player and verify Follow behavior.
6. Pause and resume.
7. Exit to the menu.
8. Confirm the Player Camera request releases and the previous output state returns.
9. Re-enter and verify identity/request duplication does not occur.

### Expected evidence

```text
explicit Follow/LookAt targets
one CameraRigComposer authority for the rig
one PlayerGameplayCameraAuthoring participation declaration
one persistent Camera Output
request precedence applied explicitly
release restores previous/default request
```

### Current status

`Running` — current focused integration scenario.

---

## TS-07 — Manager-Provisioned Player

### Goal

Prove runtime-created local Player provisioning through the official path.

### Preconditions

```text
new dedicated Route and scene
Persistent Content provisioning composition
manual-join PlayerInputManager
LocalPlayerProvisioningAuthoring
LocalPlayerProvisioningHostRegistration
Player prefab with empty Actor Mount
explicit authorized join command
```

### Steps

1. Enter the dedicated Manager-Provisioned test Route.
2. Issue one explicit authorized join.
3. Confirm the first configured free Slot is reserved.
4. Confirm `PlayerInputManager` creates exactly one Host.
5. Confirm the Host validates and is admitted.
6. Confirm the typed Slot assignment commits.
7. Confirm the Actor is selected/prepared according to policy.
8. Exit and confirm physical/contextual teardown.
9. Re-enter and confirm Slot reuse.
10. Run a negative provisioning case and confirm explicit rollback.

### Expected evidence

```text
framework reserves Slot before provisioning
PlayerInputManager is only the technical Host provisioner
playerIndex is diagnostic only
failed join releases reservation and Host evidence
successful exit permits Slot reuse
```

### Current status

`Blocked` by missing FIRSTGAME assembly, not by package runtime.

---

## TS-08 — Session-Persistent Player

### Goal

Reserved future proof for a Logical Player whose Session identity survives Route and Activity changes.

### Current status

`Blocked` by Framework.

Do not create a local workaround. The package must first provide:

```text
official authoring surface
admission request/result contract
physical ownership declaration
Slot assignment policy
Actor/materialization reconciliation
Session versus contextual release diagnostics
QA proof
```
