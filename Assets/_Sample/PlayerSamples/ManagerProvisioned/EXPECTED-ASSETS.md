# Player Provisioning / Manager-Provisioned — Expected Assets

## Application assets

```text
Assets/_Sample/PlayerSamples/ManagerProvisioned/
  GameApplication_ManagerProvisioned.asset
  Camera/Assignments/
    CameraAssignment_ManagerProvisioned_ThirdPerson.asset
  Shared/
    PlayerSessionProfile_ManagerProvisioned.asset
    ActorProfile_ManagerProvisionedPlayer.asset
  Routes/
    Route_ManagerProvisioned.asset
    Route_ManagerProvisionedOther.asset
  Activities/
    Activity_ManagerProvisioned.asset
    Activity_ManagerProvisionedOIther.asset
  Prefabs/
    Local_Player_Provisioning.prefab
    Manager_Provisioned_Actor_Presentation.prefab
  Scenes/
    ManagerProvisioned.unity
    ManagerProvisioned_Persistent.unity
    ManagerProvisionedOtherRopute.unity
```

## Reused shared assets

```text
Assets/_Sample/PlayerSamples/Shared/Characters/Players/
  PlayerSlotProfile_ManagerProvisioned.asset

Assets/_Sample/PlayerSamples/Shared/Prefabs/
  FG_Player.prefab (Manager-Provisioned Local Player Host)

Assets/_Sample/PlayerSamples/Shared/Camera/Presentation/
  PF_Player_ThirdPerson_Presentation.prefab

Assets/_Sample/Shared/Camera/
  Definitions/CameraOutput_Main.asset
  Behaviors/CameraBehavior_ThirdPerson.asset

Assets/_Sample/Shared/Prefabs/Cameras/
  PF_CameraFallback.prefab (Camera Output with fallback rig)
```

## Required Player configuration

```text
GameApplication_ManagerProvisioned
  Player Session enabled = true
  Default Profile = PlayerSessionProfile_ManagerProvisioned

PlayerSessionProfile_ManagerProvisioned
  Host Provisioning = ManagerProvisioned
  Supported Slot = PlayerSlotProfile_ManagerProvisioned
  Initial Joining Open = true

Local_Player_Provisioning
  PlayerInputManager = manual local Join, maximum one Player
  Local Player Host Prefab = FG_Player

PlayerSlotProfile_ManagerProvisioned
  Default Actor = ActorProfile_ManagerProvisionedPlayer

ActorProfile_ManagerProvisionedPlayer
  Presentation Prefab = Manager_Provisioned_Actor_Presentation

FG_PlayerActor (canonical Actor occurrence)
  PlayerActorDeclaration
  PlayerGameplayInputReader
  CharacterController
  MinimalPlayerMovement
  MinimalThirdPersonLook
    Tracking Pivot = FG_PlayerActor/CameraMount
  ActorCameraSubjectAuthoring
    Observation Transform = the same FG_PlayerActor/CameraMount

Manager_Provisioned_Actor_Presentation
  visual content only; no movement, CharacterController, input reader or Actor Camera Subject

Activity_ManagerProvisioned
  Participation = explicit PlayerSlotProfile_ManagerProvisioned
  Requirement = GameplayReady
  Relocation = explicit Activity anchor
```

## Required Camera configuration

```text
GameApplication_ManagerProvisioned
  Camera Session Output Prefab = PF_CameraFallback
  Startup Camera Assignments
    CameraAssignment_ManagerProvisioned_ThirdPerson

CameraAssignment_ManagerProvisioned_ThirdPerson
  Occurrence Mode = IndividualPerPlayer
  Membership Policy = ExplicitPlayerSlots
  Member Slot = PlayerSlotProfile_ManagerProvisioned
  Target Policy = MemberActorTargets
  Output Definition = CameraOutput_Main
  Individual Slot-to-Output mapping:
    PlayerSlotProfile_ManagerProvisioned -> CameraOutput_Main
  Rig Prefab = PF_Player_ThirdPerson_Presentation

PF_Player_ThirdPerson_Presentation
  CameraRigComposer
    Behavior Definition = CameraBehavior_ThirdPerson
    Materialized position control = CinemachineThirdPersonFollow

PF_CameraFallback
  CameraOutputAuthoring
    Output Definition = CameraOutput_Main
    Unity Camera + CinemachineBrain
    persistent fallback camera rig
```

The Third Person camera Subject comes from the current Actor occurrence's `ActorCameraSubjectAuthoring`; the Assignment does not serialize a specific Actor occurrence.

## Expected behavior

```text
Boot, no Player
  Assignment remains configured
  Output presents fallback

Join
  Manager-Provisioned Player and Actor occurrence become eligible
  Third Person follows that occurrence's Subject

Leave
  Player/Actor/Subject occurrence is released
  Output presents fallback

Rejoin
  New Actor/Subject occurrence is followed by Third Person

Route or Activity change
  No Camera selection or ownership change
```

Route and Activity assets do not configure Camera selection. Persistent Content contains no Camera Output; the application Camera Session owns Output capacity.

## Unity validation checklist

- Import the sample and confirm all asset references resolve.
- Validate `GameApplication_ManagerProvisioned` and the Assignment in the Framework authoring validators.
- Confirm the rig materialization is valid and includes `CinemachineThirdPersonFollow`.
- In Play Mode, verify Boot fallback, Join Third Person, Leave fallback, and Rejoin with the new Actor/Subject.
- Change Route/Activity and confirm the configured Assignment remains authoritative.
