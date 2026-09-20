# Local Multiplayer Assets

Status: **GROUP CAMERA MIGRATED LOCALLY — UNITY / QA REVALIDATION PENDING**

The historical public Slot/device/input ownership blocker is closed for the current Local Multiplayer implementation path. This file records the materialized application rather than a blocked future asset list.

## Current application-owned assets

```text
GameApplication_LocalMultiplayer.asset

Player/
  PlayerSessionProfile_LocalMultiplayer.asset
  PlayerSlotProfile_LocalMultiplayer_P1.asset
  PlayerSlotProfile_LocalMultiplayer_P2.asset

Routes/
  Route_LocalMultiplayer.asset
  MultiplayerRouteContentProfile.asset

Activities/
  Activity_LocalMultiplayer.asset

Scenes/
  LocalMultiplayer_Persistent.unity
  LocalMultiplayer.unity
  LocalMUltiplayerUI.unity

Camera/
  CameraRigBehavior_LocalMultiplayerGroup.asset
  Local Multiplayer Camera.prefab

Scripts/
  LocalMultiplayerJoinInputSource.cs
  LocalMultiplayerJoinTutorialController.cs
  LocalMultiplayerKeyboardGamepadSimulator.cs
```

The application reuses the current Player technical/presentation baseline where configured. Application/session authority remains local to Local Multiplayer.

## Current public composition

The sample uses public Framework Player Session commands and scoped observation rather than parallel sample-owned authority:

```text
Open Joining command
Join command from InputDevice
Leave command per configured Slot
IPlayerSessionScopedAccess.Changed
IPlayerSessionScopedAccess.TryGetObservation(...)
PlayerSessionScopedSlotObservation.IsJoined
PlayerSessionScopedSlotObservation.InputOwnership
```

Session changes invalidate the tutorial cache. Canonical observation is read later through a deferred/coalesced refresh instead of synchronously inside `PlayerSessionChange` publication.

The keyboard/gamepad simulator exists only to provide deterministic test InputDevices in the sample. It does not own Slot assignment, Player input routing or Session state.

## Camera composition

```text
ActorProfile_Farmer / ActorProfile_Cow
  -> migrated PresentationPrefab
  -> ActorCameraSubjectAuthoring / CameraMount

Local Multiplayer Camera
  -> Camera Output / CameraOutput_Main
     Default Camera Rig = Fixed
  -> CameraSharedComposition
     SubjectPolicy = AllAvailableSubjects
     Composition Rig = Group Camera Rig
     request precedence = 50

Group Camera Rig
  -> CameraRigBehavior_LocalMultiplayerGroup
  -> CinemachineFollow
  -> CinemachineHardLookAt
  -> CinemachineTargetGroup
  -> CinemachineGroupFraming
```

Expected semantics are independent of Slot identity:

```text
0 available Subjects -> no Group request -> Fixed Default
1 available Subject  -> one-member Group request
2 available Subjects -> two-member Group request
Subject leave         -> deterministic membership removal
fresh Actor occurrence -> fresh Subject membership
```

The sample does not own a Slot-to-camera registry and does not derive Camera membership from `PlayerInputManager`.


## Proven lifecycle and ownership slice

```text
P1 Join
P1 Leave
P1 Rejoin with placement reapplied
P2 Join
both Players active / Activity ready
P1 Leave while P2 remains active
P1 Rejoin / Activity completes again
P2 Leave while P1 remains active
P2 Rejoin
repeated P1/P2 Leave/Rejoin cycles
UI derived from current Slot occupancy
P1 and P2 retain distinct current device ownership
already-owned device blocked by tutorial before Join
```

The validated happy path produced no Framework duplicate-device rejection and no transient `RegisteredHost.NotRegistered` ownership diagnostic.

## Remaining proof before closure

```text
actual gameplay no-cross-control between P1 and P2
explicit extra-Join behavior while both configured Slots are occupied
Close Joining behavior
Reopen Joining behavior when applicable

Group Camera:
  Fixed Default with zero Subjects
  one-member Group with one Player
  two-member Group with both Players
  membership shrink on either Leave
  Fixed Default after last Leave
  fresh membership after Rejoin
```

Do not introduce sample-owned Slot, device, input or Camera membership authority to complete those remaining proofs.

For this Camera migration:

```text
Implemented = YES
Static repository verification = PASS
Unity tested = NO
QA Framework tested = NO
Integrated = NO
Validated = NO
```