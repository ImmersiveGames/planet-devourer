# Minimal Game

Status: **CAMERA-032-D/E MIGRATED LOCALLY — Unity revalidation pending**
UPM promotion: **PENDING package finalization/import proof**

## Purpose

Minimal Game is the Getting Started demonstration application for the minimum coherent Immersive Framework game.

It proves **navigation, not gameplay**.

## Canonical Scene Player reference

Minimal Game is the canonical executable Scene-Provided Player reference.

```text
PlayerSessionProfile
  HostProvisioning = SceneProvided
```

The product-facing composition is a Scene Player: a Local Player Host already authored in the gameplay Scene.

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
one Actor occurrence with an explicit Camera Subject

one Session Camera Output
one Output-owned Fixed Default Camera Rig
one Activity-owned Player Camera Presentation
Player Slot -> Output binding
Player Slot -> Presentation binding
Mounted / First Person Camera Rig
ExplicitSelection Subject policy

minimal movement/look Input
optional persistent Audio runtime
Route-owned ambient BGM
```

Application-specific assets:

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
  Prefabs/
    FG_MinimalGame_FirstPersonActor.prefab
    FG_MinimalGame_SceneProvisioned.prefab

  Scripts/
    MinimalFirstPersonLocomotion.cs
```

Reused canonical assets:

```text
Assets/_Sample/PlayerSamples/
  Shared/Prefabs/
    FG_Player.prefab
    FG_PlayerActor.prefab
  Player/Provisioned/
    FG_SceneProvisioned.prefab
Assets/_Sample/Shared/
  Prefabs/Cameras/
    PF_CameraOutput_Main.prefab
  Camera/Definitions/
    CameraOutput_Main.asset
  Camera/Behaviors/
    CameraBehavior_Fixed.asset
    CameraBehavior_MountedFirstPerson.asset
```

## Scene-Provided Player composition

```text
FG_FirstPersonActorProfile
  VisualContentPrefab = None (the Actor has no concrete visual content)

FG_MinimalGame_SceneProvisioned
  SceneProvidedLocalPlayerAuthoring
  FG_Player
    PlayerInput
    LocalPlayerHostAuthoring
      ActorMount
      PlayerActorRuntimeHostPrefab = FG_PlayerActor.prefab (shared host default)
    UnityPlayerInputGateAdapter
    ActorMount
      FG_MinimalGame_FirstPersonActor
        PlayerActorDeclaration
        CharacterController
        PlayerGameplayInputReader
        MinimalFirstPersonLocomotion
        ActorCameraSubjectAuthoring
          ObservationTransform = CameraMount
        CameraMount
        PlayerActorRuntimeHost
```

The Actor occurrence owns movement, physical state, and explicit Camera Subject evidence. The MinimalGame Actor has no configured visual content. The Player side does not own a physical Camera Output or a Camera Rig.

## Camera composition

The physical Camera is Session capacity owned by the GameApplication:

```text
GameApplication_MinimalGame
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings
      PlayerSlotProfile_Player1_MinimalGame
        -> CameraOutput_Main

    Player Presentation Bindings
      PlayerSlotProfile_Player1_MinimalGame
        -> CameraPresentation_MinimalGame_Player
```

The shared Output provides the standard/default camera:

```text
PF_CameraOutput_Main
  DefaultOutput
    Unity Camera
    CinemachineBrain
    CameraOutputAuthoring
      Output Definition = CameraOutput_Main
      Default Camera Rig = DefaultRig

  DefaultRig
    CameraRigComposer
      Behavior Definition = CameraBehavior_Fixed
      Cinemachine Camera
```

The gameplay Activity owns the Player Camera Presentation:

```text
Activity_MinimalGame
  Camera Presentations
    CameraPresentation_MinimalGame_Player
      Output = CameraOutput_Main
      Subject Policy = ExplicitSelection
      Transition = Cut
      Request Precedence = 300
      Rig Prefab = PF_MinimalGame_Player_FirstPerson_Presentation

PF_MinimalGame_Player_FirstPerson_Presentation
  CameraRigComposer
    Behavior = CameraBehavior_MountedFirstPerson
    Cinemachine Camera
      CinemachineHardLockToTarget
      CinemachineRotateWithFollowTarget
```

Runtime ownership:

```text
Player 1 Actor occurrence
  -> fresh Camera Subject evidence

Player 1 -> Presentation binding
  -> selects that exact Camera Subject
  -> attaches it to the live Activity Presentation occurrence

Activity Presentation
  -> publishes normal CameraRequest when its selected Subject is available

CameraOutput_Main
  -> Activity request wins while eligible
  -> Default Rig remains persistent fallback
```

Persistent Content intentionally has no Camera authority:

```text
MinimalGame_Persistent
  EventSystem
  Audio Runtime

  no CameraOutputAuthoring
  no PlayerCameraOutputPolicyAuthoring
  no PlayerCameraCompositionPolicyAuthoring
  no CameraSharedComposition
```

The Framework does not write `Camera.rect`.

## Unity validation target

The previous Play Mode proof predates this CAMERA-032-D/E migration. The migrated composition must now prove:

```text
Framework boot
  -> Succeeded

Camera Session
  -> Camera Session Outputs materialized
  -> outputCount = 1
  -> CameraOutput_Main

startup Activity
  -> Ready
  -> blockingIssues = 0
  -> CameraPresentation_MinimalGame_Player materialized

Scene Player
  -> Scene-Provided admission completed
  -> current Player 1 Actor occurrence established

Player Camera Subject
  -> Actor occurrence provides ActorCameraSubjectAuthoring
  -> ObservationTransform = CameraMount

Player Camera Presentation
  -> Player Camera Presentation selection attached
  -> ExplicitSelection resolves Player 1 Subject
  -> Mounted / First Person CameraRequest wins CameraOutput_Main

MinimalFirstPersonLocomotion
  -> Move / Look navigation operational
```

## Run

1. Select `GameApplication_MinimalGame.asset` as the Active Game Application.
2. Open the Minimal Game gameplay context.
3. Enter Play Mode.
4. Use Move and Look.

## Inspect

```text
GameApplication_MinimalGame
  -> PlayerSessionProfile_MinimalGame
  -> Camera Session
      -> PF_CameraOutput_Main
      -> P1 -> CameraOutput_Main
      -> P1 -> CameraPresentation_MinimalGame_Player
  -> Route_MinimalGame
      -> Activity_MinimalGame
          -> CameraPresentation_MinimalGame_Player

MinimalGame_Gameplay
  -> FG_MinimalGame_SceneProvisioned
      -> FG_Player
      -> FG_MinimalGame_FirstPersonActor
          -> ActorCameraSubjectAuthoring
              -> ObservationTransform = CameraMount
          -> CameraMount
          -> PlayerActorRuntimeHost (no visual content configured)
          -> MinimalFirstPersonLocomotion on Actor root: Move and yaw
             -> ActorCameraSubjectAuthoring ObservationTransform: pitch

MinimalGame_Persistent
  -> EventSystem
  -> Audio Runtime
```

## Completion boundary

```text
Getting Started / Minimal Game
  CAMERA-032-D/E authoring migration complete
  Unity Play Mode revalidation pending
```

Final UPM promotion/import validation remains a later package-finalization gate.


## Reset consumer proof

Minimal Game is also the current consumer proving context for the IF-ADR-035 Reset authoring model. The Reset slice is documented separately so this README can remain focused on the minimum application/player/camera composition.

See [RESET-USAGE.md](RESET-USAGE.md) for Object Direct/Stable, Composition Direct/Stable, CurrentActivity, CurrentRoute, Activity Restart and Multiple Participants.

Reset proof status is tracked independently from the Camera migration status at the top of this README.
