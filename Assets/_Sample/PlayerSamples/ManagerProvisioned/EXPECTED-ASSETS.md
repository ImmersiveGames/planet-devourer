# Player Provisioning / ManagerProvisioned — Materialization Checklist

Status: **CAMERA-032-D/E MATERIALIZED AND RUNTIME-VALIDATED — Framework application-quit Camera teardown issue pending**

## Application assets

```text
Assets/_Sample/PlayerSamples/ManagerProvisioned/
  GameApplication_ManagerProvisioned.asset

  Shared/
    PlayerSessionProfile_ManagerProvisioned.asset
    ActorProfile_ManagerProvisionedPlayer.asset

  Routes/
    Route_ManagerProvisioned.asset

  Activities/
    Activity_ManagerProvisioned.asset

  Presentation/
    CameraPresentation_ManagerProvisioned_Route.asset
    CameraPresentation_ManagerProvisioned_Player.asset

  Prefabs/
    Local Player Provisioning.prefab
    Manager Provisioned Actor Presentation.prefab

    Camera/
      PF_ManagerProvisioned_Route_Presentation.prefab
      PF_ManagerProvisioned_Player_ThirdPerson_Presentation.prefab

  Scenes/
    ManagerProvisioned.unity
    ManagerProvisioned_Persistent.unity
```

## Reused canonical Camera assets

```text
Assets/_Sample/Shared/
  Prefabs/Cameras/
    PF_CameraOutput_Main.prefab

  Camera/Definitions/
    CameraOutput_Main.asset

  Camera/Behaviors/
    CameraBehavior_Fixed.asset
    CameraBehavior_ThirdPerson.asset
```

## Required Player composition

```text
PlayerSessionProfile_ManagerProvisioned
  Host Provisioning = ManagerProvisioned
  Supported Slot = PlayerSlotProfile_ManagerProvisioned
  Initial Joining Open = true

PlayerSlotProfile_ManagerProvisioned
  Default Actor = ActorProfile_ManagerProvisionedPlayer

ActorProfile_ManagerProvisionedPlayer
  Presentation Prefab = Manager Provisioned Actor Presentation

Manager Provisioned Actor Presentation
  movement / look
  ActorCameraSubjectAuthoring
    Observation Transform = Third Person tracking pivot

Activity_ManagerProvisioned
  Player participation = explicit Player 1
  Requirement = GameplayReady
```

## Required Camera Session composition

```text
GameApplication_ManagerProvisioned
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings
      PlayerSlotProfile_ManagerProvisioned
        -> CameraOutput_Main

    Player Presentation Bindings
      PlayerSlotProfile_ManagerProvisioned
        -> CameraPresentation_ManagerProvisioned_Player

  Session Camera Presentations
    none
```

## Required Route Camera composition

```text
Route_ManagerProvisioned
  Camera Presentations
    CameraPresentation_ManagerProvisioned_Route

CameraPresentation_ManagerProvisioned_Route
  Output = CameraOutput_Main
  Subject Policy = AllAvailableSubjects
  Transition = Cut
  Request Precedence = 200
  Rig = PF_ManagerProvisioned_Route_Presentation

PF_ManagerProvisioned_Route_Presentation
  CameraRigComposer
    Behavior = CameraBehavior_Fixed
    CinemachineCamera
```

## Required Activity Player Camera composition

```text
Activity_ManagerProvisioned
  Camera Presentations
    CameraPresentation_ManagerProvisioned_Player

CameraPresentation_ManagerProvisioned_Player
  Output = CameraOutput_Main
  Subject Policy = ExplicitSelection
  Transition = Blend
  Request Precedence = 300
  Rig = PF_ManagerProvisioned_Player_ThirdPerson_Presentation

PF_ManagerProvisioned_Player_ThirdPerson_Presentation
  CameraRigComposer
    Behavior = CameraBehavior_ThirdPerson
    CinemachineCamera
      CinemachineThirdPersonFollow
```

## Camera ownership constraints

```text
GameApplication owns physical Session Camera capacity
PF_CameraOutput_Main owns the persistent Default Rig fallback

Route owns the fixed waiting-screen Presentation
Activity owns the Player Third Person Presentation
Player owns Camera Subject evidence only

Player Slot -> Output is explicit
Player Slot -> Presentation is explicit
Player Presentation Subject Policy = ExplicitSelection

Persistent Content contains no Camera Output
Persistent Content contains no CameraSharedComposition
Persistent Content contains no legacy Player Camera policy authoring

PlayerInputManager remains provisioning / physical split-screen authority
Framework Camera does not own Camera.rect
```

## Validated runtime behavior

```text
Camera Session Outputs materialized = 1
Route Camera Presentation materialized
  transition = Cut
  precedence = 200
  scope = Route

Activity Player Camera Presentation materialized
  transition = Blend
  precedence = 300
  scope = Activity

Before Join
  Player lifecycle = WaitingForJoin
  Route Camera is the contextual presentation

Join
  SucceededJoined
  Actor prepared / materialized
  gameplay admitted
  Third Person Player Presentation becomes effective

Leave
  SucceededLeft
  Actor / Subject occurrence released
  Slot returns to Available
  Route Camera becomes effective

Rejoin
  second Join succeeds
  new Host / Actor occurrence
  new Camera Subject occurrence
  Third Person Player Presentation becomes effective again
```

## Known Framework issue

When exiting Play Mode, Framework application-quit teardown can attempt normal Camera request arbitration after Unity has already invalidated a materialized Cinemachine Camera Scene. The resulting Route / Activity Presentation release warnings are Framework teardown-order debt, not missing ManagerProvisioned authoring.
