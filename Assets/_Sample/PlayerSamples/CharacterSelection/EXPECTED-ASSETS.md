# Character Selection — Expected Unity Assets

Status: **CAMERA-032-D/E MATERIALIZED AND LOCALLY VALIDATED — Framework application-quit Camera teardown issue pending**

## Application intent

```text
GameApplication_CharacterSelection.asset

PlayerSessionProfile_CharacterSelection.asset
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
  initialJoiningOpen = false
  supportedSlots = PlayerSlotProfile_ManagerProvisioned
```

Character Selection is distinct from default-resolving Player Provisioning because the Player may be Joined while Actor resolution is still pending.

## Sample-owned assets

```text
CharacterSelection/
  GameApplication_CharacterSelection.asset

  Player/
    PlayerSessionProfile_CharacterSelection.asset
    ActorProfile_Farmer.asset
    ActorProfile_Cow.asset

  Routes/
    Route_Character Selection.asset
    Route Content Character Selection.asset

  Activities/
    Activity_Character Selection.asset

  Scenes/
    CharacterSelection_UI.unity

  Scripts/
    CharacterSelectionActorButtonPresenter.cs
```

## Reused Player assets

```text
PlayerSlotProfile_ManagerProvisioned

FG_FarmerPresentation
FG_CowPresentation
```

Both Actor presentations must expose:

```text
gameplay input
minimal Player movement
minimal Third Person look
ActorCameraSubjectAuthoring
  Observation Transform = Third Person tracking pivot
```

They must not contain:

```text
CameraOutputAuthoring
CameraRigComposer
CinemachineCamera
gameplay CameraRequest ownership
```

## Reused Camera assets

No CharacterSelection-specific Camera asset is required.

```text
Assets/_Sample/Shared/Prefabs/Cameras/
  PF_CameraOutput_Main.prefab

Assets/_Sample/Shared/Camera/Definitions/
  CameraOutput_Main.asset

Assets/_Sample/PlayerSamples/Shared/Camera/
  CameraPresentation_Default_Route.asset
  CameraPresentation_ThirdPerson.asset

  Presentation/
    PF_Default_Route_Presentation.prefab
    PF_Player_ThirdPerson_Presentation.prefab
```

## Required GameApplication Camera configuration

```text
GameApplication_CharacterSelection
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

## Required Route Camera configuration

```text
Route_Character Selection
  Camera Presentations
    CameraPresentation_Default_Route

CameraPresentation_Default_Route
  Output = CameraOutput_Main
  Subject Policy = AllAvailableSubjects
  Transition = Cut
  Request Precedence = 200
  Rig = PF_Default_Route_Presentation
```

## Required Activity Camera configuration

```text
Activity_Character Selection
  Camera Presentations
    CameraPresentation_ThirdPerson

CameraPresentation_ThirdPerson
  Output = CameraOutput_Main
  Subject Policy = ExplicitSelection
  Transition = Blend
  Request Precedence = 300
  Rig = PF_Player_ThirdPerson_Presentation
```

The Activity declaration is required. A Player Presentation binding in the GameApplication only maps a Slot to an already-live Presentation definition; it does not materialize the Presentation occurrence.

## Route / UI boundary

```text
Route_Character Selection
  Primary Scene = ManagerProvisioned.unity
  Route Content = CharacterSelection_UI.unity

CharacterSelection_UI
  PlayerSessionObserver
    scope = Route

  Farmer selection
    PlayerSessionSelectActorCommandTrigger
    Player Slot = PlayerSlotProfile_ManagerProvisioned
    Actor = ActorProfile_Farmer

  Cow selection
    PlayerSessionSelectActorCommandTrigger
    Player Slot = PlayerSlotProfile_ManagerProvisioned
    Actor = ActorProfile_Cow
```

UI code remains presentation-only and does not own Player or Camera lifecycle.

## Expected runtime path

```text
Boot
  -> Route Camera [200]

Open Joining
Join
  -> SucceededJoined
  -> WaitingForActorSelection
  -> no Actor Camera Subject
  -> Route Camera remains

Select Farmer / Cow
  -> Actor selection commit
  -> Actor preparation/materialization
  -> ActorCameraSubjectAuthoring publishes exact current Subject
  -> ExplicitSelection resolves that Subject
  -> Third Person [300]
  -> GameplayReady

Leave
  -> Actor / Subject occurrence ends
  -> Third Person loses eligibility
  -> Route Camera [200]

Rejoin
  -> WaitingForActorSelection
  -> Route Camera remains
  -> fresh explicit Actor selection required
```

## Validated runtime behavior

```text
Camera Session Outputs materialized = 1
Route Presentation = CameraPresentation_Default_Route / Cut / 200
Activity Presentation = CameraPresentation_ThirdPerson / Blend / 300
Player Camera Presentation selection = attached

Join after Open = SucceededJoined
pre-selection state = WaitingForActorSelection

Farmer selection = SucceededSelected
Farmer Actor = prepared/materialized
GameplayReady = true

Leave = succeeded
Route Camera returns

Rejoin = fresh Player occurrence
pre-selection state = WaitingForActorSelection

Cow selection = succeeded
Cow Actor = fresh prepared/materialized occurrence
GameplayReady = true
Third Person = operational
```

## Camera ownership constraints

```text
GameApplication owns physical Camera Session capacity
Route owns neutral waiting / selection framing
Activity owns Player Third Person Presentation occurrence
Player binding supplies exact Subject selection
Actor supplies Camera Subject evidence only

Persistent Content contains no Camera authority
PlayerInputManager remains provisioning / physical split-screen authority
Framework Camera does not own Camera.rect
```

## Known Framework issue

Application quit can currently produce Route / Activity Camera Presentation release warnings because normal request arbitration is attempted after Unity has invalidated the materialized Cinemachine Camera Scene.

This is Framework teardown-order debt and is not missing CharacterSelection authoring.
