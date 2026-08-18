# Minimal Game — Materialization Checklist

Status: **MATERIALIZED / PLAY MODE PROVEN — 2026-08-17**

The planning scaffold originally used this file to record Unity assets that still had to be created. The Minimal Game materialization target is now present.

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
  Scene-Provided Logical Player.prefab

Scripts/
  MinimalFirstPersonLocomotion.cs
```

## Required composition verified

```text
Activity_MinimalGame
  Player participation requirement = GameplayReady

Scene-Provided Logical Player
  PlayerGameplayInputConsumerBinding
  PlayerGameplayCameraAuthoring
  First Person Camera Rig
  CameraRigComposer presentation = Mounted
  CameraMount
  CharacterController
  MinimalFirstPersonLocomotion

MinimalGame_Persistent
  CameraOutputSessionBinding
  Unity Camera
  CinemachineBrain
  explicit Default Camera Rig = Session Camera Rig
```

## Runtime evidence

The accepted Play Mode proof reached:

```text
Camera Output initialized
Activity Ready
blockingIssues = 0
Player gameplay binding READY
Move input received
Look input received
```

No additional Unity asset is required to close the **current authoring/proving phase** of Getting Started / Minimal Game.

Final UPM promotion/import validation remains a later package-finalization gate and is not represented as complete by this checklist.
