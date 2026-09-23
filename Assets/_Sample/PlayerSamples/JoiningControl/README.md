# Player Provisioning — Joining Control

Status: **CAMERA-032-D/E VALIDATED — Join / Leave / Rejoin and Camera arbitration proven in Unity Play Mode**

This sample reuses the same Player, Primary Scene, Persistent Content and neutral Camera assets as ManagerProvisioned. Its product difference is explicit control over whether new Players may join.

## Session policy

```text
PlayerSessionProfile_JoiningControl
  HostProvisioning = ManagerProvisioned
  ActorResolution = ResolveConfiguredDefault
  initialJoiningOpen = false
  supportedSlots = 1
```

Open / Close Join controls belong to the Joining Control Activity. They change Session admission state; they do not own Camera state.

## Camera composition

No sample-specific Camera assets are required. JoiningControl reuses the neutral Player sample Camera assets:

```text
Assets/_Sample/Shared/
  Prefabs/Cameras/
    PF_CameraOutput_Main.prefab

Assets/_Sample/PlayerSamples/Shared/Camera/
  CameraPresentation_Default_Route.asset
  CameraPresentation_ThirdPerson.asset

  Presentation/
    PF_Default_Route_Presentation.prefab
    PF_Player_ThirdPerson_Presentation.prefab
```

The GameApplication owns physical Camera capacity and the explicit Player bindings:

```text
GameApplication_JoiningControl
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings
      PlayerSlotProfile_ManagerProvisioned
        -> CameraOutput_Main

    Player Presentation Bindings
      PlayerSlotProfile_ManagerProvisioned
        -> CameraPresentation_ThirdPerson

  Session Camera Presentations
    none
```

The Route owns the waiting / neutral framing:

```text
Route_Joining Control
  Camera Presentations
    CameraPresentation_Default_Route

CameraPresentation_Default_Route
  Output = CameraOutput_Main
  Rig = PF_Default_Route_Presentation
  Subject Policy = AllAvailableSubjects
  Transition = Cut
  Request Precedence = 200
```

The Activity owns the Player gameplay presentation:

```text
Activity_Joining Control
  Camera Presentations
    CameraPresentation_ThirdPerson

CameraPresentation_ThirdPerson
  Output = CameraOutput_Main
  Rig = PF_Player_ThirdPerson_Presentation
  Subject Policy = ExplicitSelection
  Transition = Blend
  Request Precedence = 300
```

The Player Actor contributes Camera Subject evidence only:

```text
Actor Presentation
  MinimalThirdPersonLook
    Tracking Pivot

  ActorCameraSubjectAuthoring
    Observation Transform
      -> Tracking Pivot
```

The Player does not own a Camera Output or Camera Rig.

## Runtime arbitration

```text
Boot / Joining closed
  -> Route Presentation [200]

Join while closed
  -> rejected
  -> Route Presentation remains

Open Join
Join
  -> Player Host created
  -> configured default Actor selected/prepared/materialized
  -> Actor Camera Subject becomes available
  -> Player Slot selects that exact Subject
  -> Activity Third Person Presentation [300] becomes effective

Close Join
  -> current Player remains
  -> Third Person remains

Leave
  -> current Actor / Subject occurrence is released
  -> Third Person loses eligibility
  -> Route Presentation [200] becomes effective again

Reopen Join
Rejoin
  -> fresh Player / Actor / Subject occurrence
  -> Third Person [300] becomes effective again
```

## Validated runtime evidence

Unity Play Mode validation confirmed:

```text
Camera Session Outputs materialized = 1

Route Camera Presentation materialized
  CameraPresentation_Default_Route
  Cut
  precedence = 200
  scope = Route

Player Camera Presentation selection attached
  PlayerSlot:player.slot.1
  CameraPresentation_ThirdPerson

Activity Camera Presentation materialized
  CameraPresentation_ThirdPerson
  Blend
  precedence = 300
  scope = Activity

OpenJoining = Succeeded
Join = SucceededJoined
Player = GameplayReady
Leave = SucceededLeft
Rejoin = SucceededJoined
fresh Actor occurrence = confirmed
CloseJoining = Succeeded
```

## Persistent Content boundary

`ManagerProvisioned_Persistent.unity` is reused as infrastructure only.

It intentionally contains:

```text
Local Player Provisioning
Audio / UI support
EventSystem
```

It must not contain:

```text
CameraOutputAuthoring
CameraSharedComposition
legacy Player Camera policy authoring
sample-specific gameplay Camera authority
```

`PlayerInputManager` remains provisioning / physical split-screen authority. The Framework Camera layer does not own `Camera.rect`.

## Known Framework issue

When leaving Play Mode, Framework application-quit teardown can attempt normal Camera request arbitration after Unity has already invalidated a materialized Cinemachine Camera Scene.

The resulting Route / Activity `Lifecycle Camera Presentation release failed` warnings are Framework shutdown-order debt, not JoiningControl authoring debt.
