# Player Provisioning — Joining Control

Status: **Camera Assignment migration authored; Unity import and Play Mode validation pending.**

This sample demonstrates Manager-Provisioned Player Join while keeping Session admission closed until the user opens it.

## Session policy

```text
PlayerSessionProfile_JoiningControl
  HostProvisioning = ManagerProvisioned
  ActorResolution = ResolveConfiguredDefault
  Initial Joining Open = false
  Supported Slot = PlayerSlotProfile_ManagerProvisioned
```

Open and Close Join controls remain the focus. They change Session admission state; closing Join does not remove an already Joined Player.

## Player composition

```text
GameApplication_JoiningControl
  Default Profile = PlayerSessionProfile_JoiningControl

Local_Player_Provisioning prefab
  LocalPlayerProvisioningAuthoring
    Local Player Host prefab = configured Manager-Provisioned Host

PlayerSlotProfile_ManagerProvisioned
  Default Actor = ActorProfile_ManagerProvisionedPlayer

FG_PlayerActor (canonical Actor occurrence)
  PlayerActorDeclaration
  CharacterController
  MinimalPlayerMovement
  MinimalThirdPersonLook
    Tracking Pivot = occurrence Camera pivot
  ActorCameraSubjectAuthoring
    Observation Transform = occurrence Camera pivot
```

Join provisions the Host and Actor occurrence and admits that Player to the sample Activity. Leave releases the exact Player and Actor occurrence. Rejoin creates a new Actor and Subject occurrence.

## Camera composition

```text
GameApplication_JoiningControl
  Camera Session
    Output Prefab = PF_CameraFallback
  Startup Camera Assignments
    CameraAssignment_JoiningControl_ThirdPerson

CameraAssignment_JoiningControl_ThirdPerson
  Occurrence = IndividualPerPlayer
  Membership = Explicit Player Slots
  Member = PlayerSlotProfile_ManagerProvisioned
  Target = MemberActorTargets
  Output = CameraOutput_Main
  Slot mapping:
    PlayerSlotProfile_ManagerProvisioned -> CameraOutput_Main
  Rig = PF_Player_ThirdPerson_Presentation
    CinemachineThirdPersonFollow
```

The Assignment remains configured for the Session even when no Player is Joined. `PF_CameraFallback` owns the physical Output, Unity Camera, Cinemachine Brain and fallback rig. The Actor occurrence provides the Subject through its `ActorCameraSubjectAuthoring`.

Route and Activity changes do not select a different Camera. Camera eligibility follows the Player and Actor occurrence through the application Assignment.

## Expected behavior

```text
Boot with Join closed
  -> CameraOutput_Main shows fallback

Join while closed
  -> rejected; fallback remains

Open Join, then Join
  -> Player becomes GameplayReady
  -> Third Person follows the current Actor Subject

Close Join
  -> current Player remains Joined; Third Person remains active

Leave
  -> Player/Actor occurrence released; Output returns to fallback

Reopen Join, then Rejoin
  -> fresh Actor/Subject occurrence; Third Person follows it
```

## Persistent Content boundary

`ManagerProvisioned_Persistent.unity` is shared infrastructure. It contains Player provisioning and shared services; it must not define Camera Session policy or sample gameplay Camera authority. `PlayerInputManager` remains physical Player provisioning authority, while the Framework Camera Assignment maps the Slot to its Output.

## Validate in Unity

1. Import the project and resolve any authoring validation errors.
2. Enter Play Mode with Join closed; confirm the Output shows fallback.
3. Attempt Join while closed; confirm rejection and unchanged fallback.
4. Open Join and Join; confirm GameplayReady and Third Person tracking.
5. Close Join; confirm the Player and Third Person view remain.
6. Leave; confirm the Slot is Available and the Output returns to fallback.
7. Reopen Join and Rejoin; confirm a fresh Actor/Subject occurrence is followed.
8. Change Route or Activity; confirm no other Camera is selected.
