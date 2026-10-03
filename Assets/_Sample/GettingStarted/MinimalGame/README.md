# Minimal Game

Status: **Session Camera Assignment asset migrated locally — Unity revalidation pending**
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
one Session Camera Assignment asset (`CameraAssignment_MinimalGame_FirstPerson`)
Player Slot -> Output mapping owned by the Assignment
Mounted / First Person Rig Prefab

minimal movement/look Input
optional persistent Audio runtime
Route-owned ambient BGM
```

Application-specific assets:

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

The Game Application owns one physical Output prefab and references `CameraAssignment_MinimalGame_FirstPerson.asset` in `StartupCameraAssignments`. The Assignment asset directly references `MinimalGame_CameraRig_FirstPerson`, declares the Player Slot and maps it to `CameraOutput_Main`. The Framework derives `PlayerInput.camera` from this Assignment and the current Player Host evidence.

The camera Rig follows the Actor's explicit `CameraMount` Subject. The Output owns its Unity Camera, Cinemachine Brain and Fallback rig. Route and Activity assets do not select cameras, and Framework camera code does not write `Camera.rect` or `Camera.pixelRect`.

## Unity validation target

The previous Play Mode proof predates the Assignment asset migration. The migrated composition must prove:

```text
Framework boot = Succeeded
Camera Output materialized = CameraOutput_Main
Session Camera Assignment materialized = CameraAssignment_MinimalGame_FirstPerson
Scene-Provided Player admitted with a current Actor occurrence
PlayerInput.camera references CameraOutput_Main Unity Camera
Mounted / First Person rig follows the Actor Camera Subject
Move / Look navigation operational
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
      -> Startup Camera Assignment = CameraAssignment_MinimalGame_FirstPerson
      -> P1 PlayerInput.camera = CameraOutput_Main Unity Camera
  -> Route_MinimalGame
      -> Activity_MinimalGame
          -> CameraAssignment_MinimalGame_FirstPerson

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

Reset proof status is tracked independently from the Camera migration status at the top of this README. The Reset consumer slice is CLOSED for authoring/proving; see RESET-USAGE.md for the eight validated scenarios and closure evidence.
