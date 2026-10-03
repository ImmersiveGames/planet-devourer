# Minimal Game — Materialization Checklist

Status: **Session Camera Assignment asset migrated locally — Unity import and Play Mode revalidation pending**

## Materialized application assets

```text
Assets/_Sample/GettingStarted/MinimalGame/
  GameApplication_MinimalGame.asset

  Camera/
    Assignments/
      CameraAssignment_MinimalGame_FirstPerson.asset

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

  Prefabs/
    FG_MinimalGame_FirstPersonActor.prefab
    FG_MinimalGame_SceneProvisioned.prefab

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
    FG_SceneProvisioned.prefab (shared baseline)
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
  VisualContentPrefab = None (the Actor has no concrete visual content)

FG_MinimalGame_SceneProvisioned
  SceneProvidedLocalPlayerAuthoring
  FG_Player
    PlayerInput
    LocalPlayerHostAuthoring
    FG_MinimalGame_FirstPersonActor
      PlayerActorDeclaration
      CharacterController
      PlayerGameplayInputReader
      MinimalFirstPersonLocomotion
      ActorCameraSubjectAuthoring
        ObservationTransform = CameraMount
      CameraMount
      PlayerActorRuntimeHost

Activity_MinimalGame
  Player participation requirement = GameplayReady
```

## Required Camera composition

```text
GameApplication_MinimalGame
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main
  Startup Camera Assignments
    CameraAssignment_MinimalGame_FirstPerson
      Rig Prefab = MinimalGame_CameraRig_FirstPerson
      Member Slot = PlayerSlotProfile_Player1_MinimalGame
      Output = CameraOutput_Main

MinimalGame_CameraRig_FirstPerson
  CameraRigComposer
    Behavior = CameraBehavior_MountedFirstPerson

PF_CameraOutput_Main
  CameraOutputAuthoring
    Output = CameraOutput_Main
    Fallback Camera Rig = Fixed
```

The Assignment is the sole Player Slot → Output authority. Session Output configuration contains physical Output capacity only. The Framework derives `PlayerInput.camera`; `PlayerInputManager` owns viewport geometry and Framework code does not write `Camera.rect` / `Camera.pixelRect`.
## Unity validation target

```text
GameApplication Camera Session validation = valid
Camera Session Outputs materialized = 1
CameraOutputAuthoring = Initialized
Framework boot = Succeeded
Activity = Ready
blockingIssues = 0
Scene Player contextual projection = established
PlayerInput.camera = CameraOutput_Main Unity Camera
Mounted / First Person Assignment = operational
Move / Look navigation = operational
```

Final UPM promotion/import validation remains a later package-finalization gate.
