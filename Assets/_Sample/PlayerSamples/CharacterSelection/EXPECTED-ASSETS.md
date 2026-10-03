# Character Selection — Expected Unity Assets

Status: **Session Camera Assignment asset migrated locally — Unity validation pending**

## Application intent

```text
GameApplication_CharacterSelection.asset

PlayerSessionProfile_CharacterSelection.asset
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
  initialJoiningOpen = false
  supportedSlots = PlayerSlotProfile_ManagerProvisioned
```

Character Selection is distinct from default-resolving Player Provisioning because the Player may be Joined while Actor resolution is still pending.

## Sample-owned assets

```text
CharacterSelection/
  GameApplication_CharacterSelection.asset

  Player/
    PlayerSessionProfile_CharacterSelection.asset
    ActorProfile_Farmer.asset
    ActorProfile_Cow.asset

  Routes/
    Route_Character Selection.asset
    Route Content Character Selection.asset

  Activities/
    Activity_Character Selection.asset

  Scenes/
    CharacterSelection_UI.unity

  Scripts/
    CharacterSelectionActorButtonPresenter.cs
```

## Reused Player assets

```text
PlayerSlotProfile_ManagerProvisioned

FG_FarmerPresentation
FG_CowPresentation
```

Both Actor presentations must expose:

```text
gameplay input
minimal Player movement
minimal Third Person look
ActorCameraSubjectAuthoring
  Observation Transform = Third Person tracking pivot
```

They must not contain:

```text
CameraOutputAuthoring
CameraRigComposer
CinemachineCamera
Framework Camera Output or rig ownership
```

## Reused Camera assets

```text
Assets/_Sample/Shared/Prefabs/Cameras/PF_CameraOutput_Main.prefab
Assets/_Sample/Shared/Camera/Definitions/CameraOutput_Main.asset
Assets/_Sample/PlayerSamples/Shared/Camera/Presentation/PF_Player_ThirdPerson_Presentation.prefab
```

## Required GameApplication Camera configuration

```text
GameApplication_CharacterSelection
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main
  Startup Camera Assignments
    CameraAssignment_CharacterSelection
      Occurrence = IndividualPerPlayer
      Member Slot = PlayerSlotProfile_ManagerProvisioned
      Target Policy = MemberActorTargets
      Rig Prefab = PF_Player_ThirdPerson_Presentation
      Output = CameraOutput_Main
      Slot -> Output = PlayerSlotProfile_ManagerProvisioned -> CameraOutput_Main
```

Route and Activity assets contain no Camera selection fields. Before Actor selection, the Output's own Fallback covers the target-required Assignment; after Actor selection, the current Actor Subject becomes eligible on the same Assignment occurrence.
## Route / UI boundary

```text
Route_Character Selection
  Primary Scene = ManagerProvisioned.unity
  Route Content = CharacterSelection_UI.unity

CharacterSelection_UI
  PlayerSessionObserver
    scope = Route

  Farmer selection
    PlayerSessionSelectActorCommandTrigger
    Player Slot = PlayerSlotProfile_ManagerProvisioned
    Actor = ActorProfile_Farmer

  Cow selection
    PlayerSessionSelectActorCommandTrigger
    Player Slot = PlayerSlotProfile_ManagerProvisioned
    Actor = ActorProfile_Cow
```

UI code remains presentation-only and does not own Player or Camera lifecycle.

## Expected runtime path

```text
Boot -> Session Assignment materialized; Output Fallback covers unavailable target
Join -> WaitingForActorSelection; no Actor Subject or Player camera association
Select Farmer / Cow -> current Subject resolves; PlayerInput.camera maps to CameraOutput_Main
Leave -> exact Player camera association clears; Fallback covers target absence
Rejoin -> a fresh Player occurrence waits for a fresh explicit Actor selection
Actor replacement -> Assignment occurrence remains; current Subject is refreshed
Shutdown -> PlayerInput.camera association and Session Output occurrences are released
```
## Verification status

The Assignment-owned Camera authoring is present in the current tree. Import, compile, Player Join/Leave/Rejoin, Actor selection/replacement and shutdown behavior still require Unity validation. The prior Presentation-era certification does not validate this revision.