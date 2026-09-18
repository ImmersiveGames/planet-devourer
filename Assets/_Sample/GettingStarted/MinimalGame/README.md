# Minimal Game

Status: **AUTHORING COMPLETE / PLAY MODE PROVEN — CAMERA COMPOSITION NORMALIZED 2026-09-17**  
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
one Scene-Provided Local Player
one Player Actor Runtime Host
one first-person Actor Presentation
one logical Camera View
one physical Camera Output
one Output-owned Default Camera Rig
Mounted / First Person presentation
minimal movement/look Input
optional persistent Audio runtime
Route-owned ambient BGM
```

The application-specific assets are:

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

The sample intentionally reuses the canonical shared Player and Camera authoring assets:

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

Asset names are changed only through asset-safe Unity move/rename operations that preserve `.meta` identity; documentation terminology does not silently rename serialized assets.

## Scene-Provided Player composition

The current Player authoring chain is:

```text
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
```

The Player side exposes **Camera Subject evidence** only. Ordinary Player gameplay does not own a normal Camera request, Camera View, Camera Rig or Camera Output.

The first-person presentation supplies the explicit observation Transform through `ActorCameraSubjectAuthoring`. Camera presentation is resolved by the Camera composition described below.

## Camera composition

Persistent Content instantiates the shared camera prefab:

```text
MinimalGame_Persistent
  -> FG_DefaultCamera
```

The current Camera chain is:

```text
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

Ownership is intentionally separated:

```text
Player / Actor Presentation
  -> Camera Subject evidence

CameraSharedComposition
  -> logical View -> Output association

CameraOutputAuthoring
  -> physical Camera Output
  -> Output-owned Default Camera Rig

Unity Camera
  -> physical full-screen rect
```

`CameraSharedComposition` carries no viewport/layout data. The Framework does not write `Camera.rect`.

This sample is single-player and `SceneProvided`, so it does not require `PlayerInputManager` as a split-screen layout authority. Samples that demonstrate manager-provisioned local multiplayer own that separate physical layout concern.

## Runtime contract proven

The accepted Play Mode proof reaches:

```text
Framework boot
  -> Succeeded

startup Route
  -> entered

startup Activity
  -> Ready
  -> blockingIssues = 0

Scene Player
  -> Scene-Provided admission completed
  -> current contextual Player projection established

Camera Output
  -> CameraOutputAuthoring initialized
  -> output = CameraOutput_Main
  -> physical Unity Camera resolved
  -> CinemachineBrain resolved
  -> Default Camera Rig resolved

Player Camera Subject
  -> ActorCameraSubjectAuthoring exposes CameraMount

Camera presentation
  -> CameraView_Gameplay associated with CameraOutput_Main
  -> Mounted / First Person presentation

MinimalFirstPersonLocomotion
  -> Move / Look navigation operational

Route BGM
  -> FrameworkRouteBgmBinding = PlayOwn / BGM_Floresta
  -> BGM_Floresta applied after Startup Activity entry
```

Local Player provisioning being reported as `NotConfigured` is expected in this sample because Host Provisioning is `SceneProvided`; it is not a manager-provisioned Player sample.

Audio is **Ambient/Supporting**, not a primary Getting Started lesson.

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
  -> Camera Subject becomes available
  -> Mounted Camera presents first-person view
  -> user navigates
```

No objectives, collectibles, combat, mission flow, Route switching, Activity switching or unrelated gameplay belong to this sample.

## Inspect

The canonical inspection path is:

```text
GameApplication_MinimalGame
  -> PlayerSessionProfile_MinimalGame (HostProvisioning = SceneProvided)
  -> Route_MinimalGame
  -> Activity_MinimalGame

MinimalGame_Gameplay
  -> FG_SceneProvisioned
      -> SceneProvidedLocalPlayerAuthoring
      -> FG_Player / PlayerInput / LocalPlayerHostAuthoring
      -> FG_PlayerActor
          -> PlayerActorRuntimeHost
          -> PresentationMount
              -> FG_FirstPersonPresentation
                  -> MinimalFirstPersonLocomotion
                  -> ActorCameraSubjectAuthoring
                     -> CameraMount

MinimalGame_Persistent
  -> FG_DefaultCamera
      -> CameraOutputAuthoring / CameraOutput_Main
      -> CameraSharedComposition
         -> CameraView_Gameplay
         -> CameraOutput_Main
      -> Default Camera Rig / CameraRigComposer
         -> CameraRigBehavior_MountedFirstPerson
  -> AudioRuntimeHost + FrameworkBgmDirector

Route_MinimalGame
  -> FrameworkRouteBgmBinding (PlayOwn / BGM_Floresta)
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
