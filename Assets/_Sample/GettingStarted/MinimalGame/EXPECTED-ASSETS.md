# Minimal Game — Materialization Checklist

Status: **MATERIALIZED / PLAY MODE PROVEN — CAMERA COMPOSITION NORMALIZED 2026-09-17**

The Minimal Game materialization target is present and aligned with the current Scene-Provided Player, Actor Camera Subject and viewport-free Camera composition contracts.

## Materialized application assets

```text
Assets/_Sample/GettingStarted/MinimalGame/
  GameApplication_MinimalGame.asset

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

## Reused canonical sample assets

```text
Assets/_Sample/PlayerSamples/
  Shared/Prefabs/
    FG_Player.prefab
    FG_PlayerActor.prefab

  Player/Provisioned/
    FG_SceneProvisioned.prefab

  Player/Players/
    FG_FirstPersonPresentation.prefab

Assets/_Sample/Shared/Camera/
  FG_DefaultCamera.prefab
  CameraView_Gameplay.asset
  CameraOutput_Main.asset
  CameraRigBehavior_MountedFirstPerson.asset
```

## Required Player composition verified

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
      ActorMount
      PlayerActorRuntimeHostPrefab = FG_PlayerActor.prefab
    UnityPlayerInputGateAdapter
    ActorMount
      FG_PlayerActor
        PlayerActorDeclaration
        PlayerActorRuntimeHost
          PresentationMount
            FG_FirstPersonPresentation
              PlayerGameplayInputReader
              CharacterController
              MinimalFirstPersonLocomotion
              ActorCameraSubjectAuthoring
                Observation Transform = CameraMount
              CameraMount

Activity_MinimalGame
  Player participation requirement = GameplayReady
```

The Player side provides Camera Subject evidence. It does not own a normal Camera request, Camera View, Camera Rig or Camera Output.

## Required Camera composition verified

```text
MinimalGame_Persistent
  FG_DefaultCamera
    Camera Output
      Unity Camera
      CinemachineBrain
      CameraOutputAuthoring
        Output Definition = CameraOutput_Main
        Default Camera Rig = Default Camera Rig

    Shared Camera Composition
      CameraSharedComposition
        View Definition = CameraView_Gameplay
        Output Definition = CameraOutput_Main

    Default Camera Rig
      CameraRigComposer
        Behavior Definition = CameraRigBehavior_MountedFirstPerson
        Cinemachine Camera
```

Camera ownership constraints:

```text
CameraSharedComposition carries no viewport
Framework does not write Camera.rect
Unity Camera remains authored full-screen
PlayerInputManager is not required for this single-player SceneProvided sample
```

## Runtime evidence

The accepted Play Mode proof reaches:

```text
Scene-Provided authoring validation = valid
CameraOutputAuthoring = Initialized
Framework boot = Succeeded
Activity = Ready
blockingIssues = 0
Scene Player contextual projection = established
Mounted / First Person presentation = operational
Move / Look navigation = operational
```

`Local Player provisioning is not configured` is expected here because the sample uses `HostProvisioning = SceneProvided`.

No additional Unity asset is required to close the **current authoring/proving phase** of Getting Started / Minimal Game.

Final UPM promotion/import validation remains a later package-finalization gate and is not represented as complete by this checklist.
