# POST-RESET-H2 — FIRSTGAME Camera Route/Activity Adapter

Status: migrated to framework camera components, pending Unity compile and FIRSTGAME smoke.

## Scope

This cut uses the official Immersive Framework camera components in FIRSTGAME. It does not change `com.immersive.framework`, `com.immersive.audio`, or the QA harness.

Editor setup path:

```text
Assets/_Project/Scripts/Editor/GameCamera/
```

## Runtime model

Camera priority:

```text
Activity Camera
> Retained Activity Camera for current Route
> Route Camera
> Default Camera
```

The real Unity Camera and `CinemachineBrain` are operational-scene owned. Route and Activity content provide Cinemachine rigs only.

Camera rigs must not contain:

```text
UnityEngine.Camera
CinemachineBrain
AudioListener
```

Audio remains owned by `com.immersive.audio`.

## Activity policies

```text
UseOwnOrRoute
UseOwnOrKeepPreviousActivity
UseRoute
```

## Editor setup

Run:

```text
Tools > FIRSTGAME > Camera > Configure Route-Activity Camera
```

The setup is editor-only and idempotent. It configures:

```text
FG_Menu
FG_Gameplay
Main Camera + CinemachineBrain
FirstGameCameraRoot
FirstGameCameraAnchors
MenuRoute_CameraRig
GameplayRoute_CameraRig
ActivityA_CameraRig
FrameworkCameraDirector
FrameworkRouteCameraBinding
FrameworkActivityCameraBinding
FrameworkCameraAnchorHost
FrameworkCinemachineRigApplier
```

## Expected smoke

Start in Menu, go to Gameplay, then use existing Activity buttons.

Expected behavior:

```text
Menu Route -> Menu Route camera
Gameplay Route -> Gameplay Route camera
Startup Activity A -> ActivityA camera
Activity B -> retained ActivityA camera
Activity C RouteFallback -> Gameplay Route camera
Activity D StopBgm -> Gameplay Route camera
Clear Activity -> Gameplay Route camera
```

Expected log prefix:

```text
[FRAMEWORK_CAMERA]
```
