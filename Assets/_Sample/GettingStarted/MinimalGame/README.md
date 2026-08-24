# Minimal Game

Status: **AUTHORING COMPLETE / PLAY MODE PROVEN — 2026-08-22**  
UPM promotion: **PENDING package finalization/import proof**

## Purpose

Minimal Game is the Getting Started demonstration application for the minimum coherent Immersive Framework game.

It proves **navigation, not gameplay**.

## Canonical Scene Player reference

Minimal Game is the **canonical executable Scene Player reference** for the sample program.

```text
Assets/_Sample/GettingStarted/MinimalGame/
  -> canonical Scene Player coverage
```

Runtime policy:

```text
PlayerSessionProfile
  HostProvisioning = SceneProvided
```

`SceneProvided` is the Host Provisioning mode. The product-facing composition demonstrated here is a **Scene Player**: a Local Player Host already authored in the Scene.

The Player sample family must not duplicate this baseline as a dedicated Scene Player Demonstration Application under Player unless future implementation evidence reveals a distinct Scene Player consumer contract that cannot be demonstrated here.

Player-specific sample sequencing, blockers and terminology are governed by:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
```

## Implemented composition

```text
one GameApplication
one PlayerSessionProfile
one supported Player Slot
Persistent Content
one Route
one Activity
one gameplay scene
one Scene-authored Local Player Host
one Scene Logical Player
Mounted / First Person Camera
minimal movement/look Input
```

The current materialized application uses:

```text
GameApplication_MinimalGame.asset
PlayerProfiles/PlayerSessionProfile_MinimalGame.asset
Routes/Route_MinimalGame.asset
Activities/Activity_MinimalGame.asset
Scenes/MinimalGame_Gameplay.unity
Scenes/MinimalGame_Persistent.unity
Shared/Prefabs/Scene-Provided Local Player.prefab
Shared/Prefabs/Scene-Provided Logical Player.prefab
Scripts/MinimalFirstPersonLocomotion.cs
```

The existing prefab/file names above describe the currently materialized Unity assets. They should be renamed only through an asset-safe Unity move/rename that preserves `.meta` identity; documentation terminology does not silently rename serialized assets.

## Runtime contract proven

The accepted runtime proof for this authoring cut reached:

```text
Framework boot
  -> Succeeded

startup Route
  -> entered

startup Activity
  -> Ready
  -> blockingIssues = 0

Scene Player
  -> gameplay admission completed

PlayerGameplayInputConsumerBinding
  -> current gameplay binding available
  -> GameplayReady = true

Camera Output
  -> initialized
  -> explicit Default Camera Rig = Session Camera Rig

Player Camera
  -> Mounted / First Person

MinimalFirstPersonLocomotion
  -> READY
  -> Move input received
  -> Look input received
```

The Player-owned first-person rig remains a normal eligible Local Player Camera request. The persistent `Session Camera Rig` is the explicit output Default and is not a fake Session Camera request.

## Run

1. Select `GameApplication_MinimalGame.asset` and make it the Active Game Application through the official Framework authoring surface when required.
2. Open the Minimal Game entry/gameplay context in Unity.
3. Enter Play Mode.
4. Use Move and Look to navigate the environment.

## Observe

The intended experience is intentionally small:

```text
Play
  -> application starts
  -> Route enters
  -> Activity enters
  -> Scene Player becomes gameplay-ready
  -> Mounted Camera presents first-person view
  -> user navigates
```

No objectives, collectibles, combat, mission flow, Route switching, Activity switching or unrelated gameplay belong to this sample.

## Inspect

The canonical inspection path is:

```text
GameApplication
  -> PlayerSessionProfile (HostProvisioning = SceneProvided)
  -> Route
  -> Activity
  -> Scene Player
      -> Local Player Host
      -> Scene Local Player Admission
      -> Scene Logical Player
  -> PlayerGameplayInputConsumerBinding
  -> PlayerGameplayCameraAuthoring
  -> First Person Camera Rig / CameraRigComposer
  -> Persistent Content / CameraOutputSessionBinding
```

## Completion boundary

For the current visible authoring/proving phase:

```text
Getting Started / Minimal Game
  COMPLETE
```

This means the configured sample behavior and canonical composition are materialized and proven in Play Mode.

It does **not** mean final UPM distribution is complete. Release promotion remains a separate step:

```text
planet-devourer/Assets/_Sample/GettingStarted
  -> promote/materialize into
com.immersive.framework/Samples~/GettingStarted
  -> declare package samples metadata
  -> import through Package Manager in a clean consumer project
  -> validate references and Play Mode from the imported copy
```

That packaging/import gate does not block beginning the next sample implementation cut.
