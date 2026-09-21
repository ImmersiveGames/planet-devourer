# Local Multiplayer Assets

Status: **CURRENT LOCAL MULTIPLAYER COMPOSITION MATERIALIZED — GROUP CAMERA CONSUMER UNITY PASS / QA PENDING**

The historical public Slot/device/input ownership blocker is closed for the current Local Multiplayer implementation path. This file records the materialized application rather than a blocked future asset list.

## Current application-owned assets

```text
GameApplication_LocalMultiplayer.asset

Player/
  PlayerSessionProfile_LocalMultiplayer.asset
  PlayerSlotProfile_LocalMultiplayer_P1.asset
  PlayerSlotProfile_LocalMultiplayer_P2.asset
  ActorProfile_FarmerGroup.asset
  ActorProfile_CowGroup.asset
  FG_FarmerPresentationGroup.prefab
  FG_CowPresentationGroup.prefab

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
  MinimalLocalMultiplayerMovement.cs
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
ActorProfile_FarmerGroup / ActorProfile_CowGroup
  -> dedicated Local Multiplayer Group PresentationPrefab
  -> MinimalLocalMultiplayerMovement
  -> ActorCameraSubjectAuthoring
     Observation = presentation-owned Group framing anchor
     Framing Radius = per-Presentation Subject extent

Local Multiplayer Camera
  -> Camera Output / CameraOutput_Main
     Default Camera Rig = Fixed
  -> CameraSharedComposition
     SubjectPolicy = AllAvailableSubjects
     Composition Rig = Group Camera Rig
     request precedence = 50

Group Camera Rig
  -> CameraRigBehavior_LocalMultiplayerGroup
     member weight + fallback member radius
     framing / damping / FOV / dolly / ortho tuning
  -> CinemachineFollow
  -> CinemachineHardLookAt
  -> CinemachineTargetGroup
     per-member Radius = Subject Framing Radius or Group fallback
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
dedicated P1/P2 Group Presentations materialized
MinimalLocalMultiplayerMovement functional
Group Camera consumer functional in manual Unity Play Mode
per-Subject framing evidence consumed by Group Camera
```

The validated happy path produced no Framework duplicate-device rejection and no transient `RegisteredHost.NotRegistered` ownership diagnostic.

## Remaining proof before closure

Join/Open/Close/Reopen Joining behavior is already owned by the existing Player Join/public
Session proof and is not repeated as a Local Multiplayer-specific certification gate.

The remaining technical proof is Camera:

```text
CAMERA-029 current Group lifecycle -> current QAFramework Camera certification
CAMERA-030 per-Subject framing    -> focused QA evidence
```

Do not introduce sample-owned Slot, device, input or Camera membership authority to complete
Camera certification.

For the current Camera consumer slice:

```text
Implemented = YES
Unity consumer tested = YES
Integrated = YES
QA Framework tested = NO
Validated = NO
Certified = NO
```

Remaining Camera work is application-owned visual tuning of the Group behavior asset.