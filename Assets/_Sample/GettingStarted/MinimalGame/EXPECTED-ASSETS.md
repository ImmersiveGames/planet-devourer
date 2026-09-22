# Minimal Game — Materialization Checklist

Status: **CAMERA-032-D/E MATERIALIZED LOCALLY — Unity import and Play Mode revalidation pending**

## Materialized application assets

```text
Assets/_Sample/GettingStarted/MinimalGame/
  GameApplication_MinimalGame.asset

  Camera/
    Presentations/
      CameraPresentation_MinimalGame_Player.asset
    Prefabs/
      PF_MinimalGame_Player_FirstPerson_Presentation.prefab

  PlayerProfiles/
    PlayerSessionProfile_MinimalGame.asset
    PlayerSlotProfile_Player1_MinimalGame.asset
    FG_FirstPersonActorProfile.asset

  Routes/
    Route_MinimalGame.asset

  Activities/
    Activity_MinimalGame.asset

  Scenes/
    MinimalGame_Gameplay.unity
    MinimalGame_Persistent.unity

  Scripts/
    MinimalFirstPersonLocomotion.cs
```

## Reused canonical assets

```text
Assets/_Sample/PlayerSamples/
  Shared/Prefabs/
    FG_Player.prefab
    FG_PlayerActor.prefab
  Player/Provisioned/
    FG_SceneProvisioned.prefab
  Player/Players/
    FG_FirstPersonPresentation.prefab

Assets/_Sample/Shared/
  Prefabs/Cameras/
    PF_CameraOutput_Main.prefab
  Camera/Definitions/
    CameraOutput_Main.asset
  Camera/Behaviors/
    CameraBehavior_Fixed.asset
    CameraBehavior_MountedFirstPerson.asset
```

## Required Player composition

```text
PlayerSessionProfile_MinimalGame
  Host Provisioning = SceneProvided
  Supported Slot = PlayerSlotProfile_Player1_MinimalGame

FG_FirstPersonActorProfile
  PresentationPrefab = FG_FirstPersonPresentation.prefab

FG_SceneProvisioned
  SceneProvidedLocalPlayerAuthoring
  FG_Player
    PlayerInput
    LocalPlayerHostAuthoring
    FG_PlayerActor
      PlayerActorRuntimeHost
        FG_FirstPersonPresentation
          MinimalFirstPersonLocomotion
          ActorCameraSubjectAuthoring
            Observation Transform = CameraMount

Activity_MinimalGame
  Player participation requirement = GameplayReady
```

## Required Camera composition

```text
GameApplication_MinimalGame
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings
      Player 1 -> CameraOutput_Main

    Player Presentation Bindings
      Player 1 -> CameraPresentation_MinimalGame_Player

Activity_MinimalGame
  Camera Presentations
    CameraPresentation_MinimalGame_Player
      Output = CameraOutput_Main
      Subject Policy = ExplicitSelection
      Transition = Cut
      Request Precedence = 300
      Rig = PF_MinimalGame_Player_FirstPerson_Presentation

PF_MinimalGame_Player_FirstPerson_Presentation
  CameraRigComposer
    Behavior = CameraBehavior_MountedFirstPerson
    CinemachineCamera
      HardLockToTarget
      RotateWithFollowTarget

PF_CameraOutput_Main
  CameraOutputAuthoring
    Output = CameraOutput_Main
    Default Rig = Fixed
```

Camera ownership constraints:

```text
Persistent Content contains no Camera Output
Persistent Content contains no Player Camera policy
Persistent Content contains no CameraSharedComposition

Player owns Camera Subject evidence only
Activity owns the live Player Camera Presentation occurrence
GameApplication owns physical Session Camera capacity
Output Default Rig is the persistent fallback
Framework does not write Camera.rect
```

## Unity validation target

```text
GameApplication Camera Session validation = valid
Camera Session Outputs materialized = 1
CameraOutputAuthoring = Initialized
Framework boot = Succeeded
Activity = Ready
blockingIssues = 0
Scene Player contextual projection = established
Player Camera Presentation selection = attached
Mounted / First Person presentation = operational
Move / Look navigation = operational
```

Final UPM promotion/import validation remains a later package-finalization gate.
