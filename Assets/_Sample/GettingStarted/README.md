# Getting Started

Status: **Current Session Camera Assignment composition is authored in the checkout.**

## Purpose

`MinimalGame` is the first runnable consumer composition for a small navigable game. It demonstrates a Scene-Provided Player, one Route, an Activity, gameplay input and a Session-owned Camera Assignment. The sample establishes application setup; it is not a full gameplay template.

## Minimum composition

```text
GameApplication_MinimalGame
  PlayerSessionProfile_MinimalGame
  Startup Route = Route_MinimalGame
  Startup Camera Assignment = CameraAssignment_MinimalGame_FirstPerson

Route_MinimalGame
  Activity_MinimalGame
  Gameplay scene = MinimalGame_Gameplay

MinimalGame_Gameplay
  MinimalGame_Player_SceneProvided
    PlayerInput + LocalPlayerHostAuthoring + ActorMount
    SceneProvidedLocalPlayerAuthoring
    FG_MinimalGame_FirstPersonActor
      PlayerActorRuntimeHost + PlayerActorDeclaration
      PlayerGameplayInputReader + MinimalFirstPersonLocomotion
      ActorCameraSubjectAuthoring -> CameraMount
```

The `PlayerSessionProfile` lists `PlayerSlotProfile_Player1_MinimalGame` as a supported Slot and uses Scene-Provided provisioning. The physical Player Host is authored in the gameplay scene. The Actor occurrence owns its body, movement and explicit Camera Subject. The camera output and rig are not owned by the Player or Activity.

## Camera Assignment

`GameApplication_MinimalGame` references `CameraAssignment_MinimalGame_FirstPerson`. The Assignment directly references `MinimalGame_CameraRig_FirstPerson`, uses `IndividualPerPlayer`, explicitly includes the Player 1 Slot and maps it to `CameraOutput_Main`. `MinimalGame_CameraOutput_Main` owns the Unity Camera, Cinemachine Brain and Fallback rig through `CameraOutputAuthoring`. The Assignment selects the normal rig; the Output supplies fallback coverage.

Route and Activity changes do not select or replace the Assignment. The Actor occurrence provides its exact `CameraMount` observation transform. Framework Camera code does not write viewport rectangles.

## Run and inspect

1. In Unity, open `Assets/_Sample/GettingStarted/MinimalGame/Scenes/MinimalGame_Persistent.unity` and `MinimalGame_Gameplay.unity` as the sample's authored scene setup.
2. Set `GameApplication_MinimalGame` as the active Game Application using its Inspector action.
3. Enter Play Mode and use the configured Move and Look actions.
4. Inspect the application asset, Session Profile, Slot Profile, Route, Activity, Camera Assignment and Output prefab listed above.

Expected: the Scene-Provided Player is admitted by the current Activity, movement input reaches the Actor, and the Session Assignment presents the Actor's explicit Camera Subject. See [Minimal Game](MinimalGame/README.md) for asset-by-asset details and [Reset consumer usage](MinimalGame/RESET-USAGE.md) for the separate Reset scenarios.

## Validation status

The repository has historical Scene-Provided readiness evidence, while the current Assignment migration is authored in this checkout. Do not infer current package import/compile or Play Mode validation from the authored assets. Check the Framework package's `Documentation~/Architecture/Tracking/IF-TRACK-Framework.md` for package gates and this sample's evidence before claiming validation.
