# Minimal Game

## Purpose

The smallest navigable Framework consumer sample: one Scene-Provided Player, one Session, a Route with an Activity, gameplay input, and an explicit Session Camera Assignment.

## Required authored assets

```text
GameApplication_MinimalGame.asset
PlayerProfiles/PlayerSessionProfile_MinimalGame.asset
PlayerProfiles/PlayerSlotProfile_Player1_MinimalGame.asset
PlayerProfiles/FG_FirstPersonActorProfile.asset
Routes/Route_MinimalGame.asset
Activities/Activity_MinimalGame.asset
Camera/Assignments/CameraAssignment_MinimalGame_FirstPerson.asset
Camera/Prefabs/MinimalGame_CameraRig_FirstPerson.prefab
Camera/Outputs/MinimalGame_CameraOutput_Main.prefab
Scenes/MinimalGame_Persistent.unity
Scenes/MinimalGame_Gameplay.unity
Prefabs/MinimalGame_Player_SceneProvided.prefab
Prefabs/FG_MinimalGame_FirstPersonActor.prefab
Scripts/MinimalFirstPersonLocomotion.cs
```

`PlayerSessionProfile_MinimalGame` configures Scene-Provided provisioning and the supported Player Slot. The Scene Player prefab supplies the exact `LocalPlayerHostAuthoring` / `ActorMount` composition. The Actor prefab contains `PlayerActorRuntimeHost`, `PlayerActorDeclaration`, `PlayerGameplayInputReader`, `MinimalFirstPersonLocomotion` and `ActorCameraSubjectAuthoring` bound to `CameraMount`.

## Camera ownership

`GameApplication_MinimalGame` owns the startup `CameraAssignment_MinimalGame_FirstPerson` and the physical Output configuration. The Assignment uses `IndividualPerPlayer`, explicitly includes the Player 1 Slot, maps it to `CameraOutput_Main` and references `MinimalGame_CameraRig_FirstPerson`. The Output prefab owns its Unity Camera, Cinemachine Brain and explicit fallback rig through `CameraOutputAuthoring`.

The Session Assignment owns selection and Slot-to-Output mapping. Route and Activity do not select Cameras. The current Actor occurrence owns the Subject evidence; its `CameraMount` is the observation transform. The Scene Player does not own an Output or camera rig.

## Run

1. Set `GameApplication_MinimalGame` as the active Game Application using its Inspector action.
2. Enter Play Mode from the sample project and use the configured Move and Look input.
3. Inspect `Scenes/MinimalGame_Gameplay.unity` for the Scene-Provided Player; `GameApplication_MinimalGame` references `Scenes/MinimalGame_Persistent.unity` as Persistent Content.

Expected behavior is a Scene-Provided Player admitted into the current Activity, with movement on the Actor occurrence and the Assignment rig following its explicit Subject. The second Route/Activity assets are navigation fixtures, not Camera owners.

## Inspect

Inspect the Game Application's Session Profile, Startup Route and Startup Camera Assignments; then inspect the Route, Activity, Slot Profile, Assignment, Output prefab, rig prefab and Scene Player/Actor hierarchy listed above.

## Validation

This README records the current authored composition. The previous Play Mode proof predates this Assignment migration; the repository tracker does not claim that the migrated Minimal Game was revalidated. Unity import/compile and consumer Play Mode validation remain to be confirmed.

## Related samples

- [Reset consumer usage](RESET-USAGE.md) — distinct Reset scenarios in this same sample.
- [Getting Started index](../README.md) — sample family entry point.
