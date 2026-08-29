# Minimal Game — Materialization Checklist

Status: **MATERIALIZED / PLAY MODE PROVEN — PLAYER COMPOSITION ALIGNED 2026-08-29**

The planning scaffold originally used this file to record Unity assets that still had to be created. The Minimal Game materialization target is now present and aligned with the current Player Actor Runtime Host + Presentation contract.

## Materialized application assets

```text
GameApplication_MinimalGame.asset

PlayerProfiles/
  PlayerSessionProfile_MinimalGame.asset
  PlayerSlotProfile_Player1_MinimalGame.asset
  ActorProfile_MinimalPlayer.asset

Routes/
  Route_MinimalGame.asset

Activities/
  Activity_MinimalGame.asset

Scenes/
  MinimalGame_Gameplay.unity
  MinimalGame_Persistent.unity

Shared/Prefabs/
  Scene-Provided Local Player.prefab
  Player Actor Runtime Host.prefab
  Presentation.prefab

Scripts/
  MinimalFirstPersonLocomotion.cs
```

## Required Player composition verified

```text
PlayerSessionProfile_MinimalGame
  Host Provisioning = SceneProvided

ActorProfile_MinimalPlayer
  PresentationPrefab = Presentation.prefab

Scene-Provided Local Player
  PlayerInput
  LocalPlayerHostAuthoring
    ActorMount = ActorMount
    PlayerActorRuntimeHostPrefab = Player Actor Runtime Host.prefab
  SceneLocalPlayerAdmissionAuthoring
  UnityPlayerInputGateAdapter
  ActorMount
    Player Actor Runtime Host
      PlayerActorDeclaration
      PlayerActorRuntimeHost
        PresentationMount = PresentationMount
      CharacterController
      MinimalFirstPersonLocomotion
      PlayerGameplayInputConsumerBinding
      PlayerGameplayCameraAuthoring
      CameraMount
      First Person Camera Rig
        CameraRigComposer presentation = Mounted
        Cinemachine Camera
      PresentationMount
        Presentation
          ScenePlayerActorPresentationEvidence

Activity_MinimalGame
  Player participation requirement = GameplayReady

MinimalGame_Persistent
  CameraOutputSessionBinding
  Unity Camera
  CinemachineBrain
  explicit Default Camera Rig = Session Camera Rig
```

The Runtime Host owns the sample-specific gameplay composition. The Actor-specific Presentation is materialized separately under the exact `PresentationMount`; it does not own the CharacterController, locomotion, gameplay input binding or gameplay camera authoring.

For the Scene-Provided path, `SceneLocalPlayerAdmissionAuthoring` adopts the authored Runtime Host and Presentation. **Apply / Rebuild** materializes or repairs the current Profile + Runtime Host + Presentation composition, and **Validate** verifies the resulting evidence.

## Runtime evidence

The accepted Play Mode proof reached:

```text
Scene-Provided authoring Validate = Valid
Framework boot succeeded
Activity Ready
blockingIssues = 0
Player gameplay binding READY
Mounted / First Person Camera active
Move input received
Look input received
```

No additional Unity asset is required to close the **current authoring/proving phase** of Getting Started / Minimal Game.

Final UPM promotion/import validation remains a later package-finalization gate and is not represented as complete by this checklist.
