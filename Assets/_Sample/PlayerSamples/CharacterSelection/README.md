# Character Selection

Status: **CAMERA-032-D/E VALIDATED LOCALLY — explicit Actor selection and Player Camera handoff proven in Unity Play Mode**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

Character Selection demonstrates an explicit initial Actor choice after a Player Host has joined:

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

The key distinction from JoiningControl is that a joined Player exists before any Actor / Camera Subject exists.

## Player runtime flow

```text
Open Joining
  -> Join
  -> Slot Joined
  -> Actor unresolved
  -> WaitingForActorSelection

Farmer / Cow choice
  -> PlayerSessionSelectActorCommandTrigger
  -> Actor selection committed
  -> Actor prepared/materialized
  -> GameplayReady

Leave
  -> current Player / Actor occurrence released

Rejoin
  -> new Player Host
  -> WaitingForActorSelection again
  -> fresh explicit Actor choice required
```

The Route-additive `CharacterSelection_UI.unity` remains presentation-only. Player Session command components consume scoped Framework access and do not own Session authority.

## Actor presentation boundary

Character Selection owns two ActorProfile choices:

```text
ActorProfile_Farmer
  -> FG_FarmerPresentation

ActorProfile_Cow
  -> FG_CowPresentation
```

Both concrete presentations expose the same Camera-facing boundary:

```text
Actor Presentation
  gameplay input
  minimal Player movement
  minimal Third Person look

  ActorCameraSubjectAuthoring
    Observation Transform
      -> same tracking pivot used by Third Person look
```

They do not contain:

```text
CameraOutputAuthoring
CameraRigComposer
CinemachineCamera
gameplay CameraRequest ownership
```

## Camera composition

CharacterSelection reuses the same neutral Camera assets as ManagerProvisioned and JoiningControl.

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

The GameApplication owns Session Camera capacity and explicit Player bindings:

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

The Route owns the neutral / selection-screen framing:

```text
Route_Character Selection
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
Activity_Character Selection
  Camera Presentations
    CameraPresentation_ThirdPerson

CameraPresentation_ThirdPerson
  Output = CameraOutput_Main
  Rig = PF_Player_ThirdPerson_Presentation
  Subject Policy = ExplicitSelection
  Transition = Blend
  Request Precedence = 300
```

The GameApplication Player binding does not materialize the Presentation. The Activity owns the live Presentation occurrence; the Player binding attaches the exact current Player Camera Subject to that occurrence.

## Camera lifecycle

Before Actor selection, the Third Person Presentation exists but has no selected Subject. Therefore it cannot satisfy its target requirements and does not replace the Route Camera.

```text
Boot
  -> Route Presentation [200]

Join
  -> Player Host exists
  -> selected Actor = none
  -> Camera Subject = none
  -> WaitingForActorSelection
  -> Route Presentation remains effective

Select Farmer
  -> Farmer Actor occurrence materializes
  -> Farmer Camera Subject becomes available
  -> ExplicitSelection resolves that exact Subject
  -> Third Person [300] becomes effective

Leave
  -> Actor / Subject occurrence ends
  -> Third Person loses eligibility
  -> Route [200] becomes effective again

Rejoin
  -> fresh Player Host
  -> no Actor / Subject yet
  -> Route remains effective

Select Cow
  -> fresh Cow Actor / Subject occurrence
  -> Third Person [300] becomes effective again
```

This sample therefore proves that:

```text
Player Slot
  !=
current Camera Subject

Joined Player
  !=
camera-ready Player Presentation
```

## Route / UI composition

```text
Route_Character Selection
├── Primary Scene: ManagerProvisioned.unity
└── Route Content: CharacterSelection_UI.unity
    ├── PlayerSessionObserver
    └── Character Selection Controls
        ├── Farmer -> ActorProfile_Farmer
        └── Cow -> ActorProfile_Cow
```

The Actor selection controls are Route-scoped because the selection UI is Route Content. Joining / Leave controls in the primary gameplay scene remain Activity-scoped.

## Validated runtime evidence

Unity Play Mode validation confirmed the 032-D/E path:

```text
Camera Session Outputs materialized = 1

Route Camera Presentation materialized
  CameraPresentation_Default_Route
  Cut
  precedence = 200

Player Camera Presentation selection attached
  PlayerSlot:player.slot.1
  CameraPresentation_ThirdPerson

Activity Camera Presentation materialized
  CameraPresentation_ThirdPerson
  Blend
  precedence = 300

Join after Opening
  -> SucceededJoined

Joined unresolved Player
  -> WaitingForActorSelection
  -> selectedActor = empty
  -> logicalActorPrepared = false
  -> physicallyMaterialized = false
  -> gameplayAdmitted = false

Select Farmer
  -> SucceededSelected
  -> ActorProfile_Farmer
  -> GameplayReady
  -> Third Person operational

Leave
  -> current occurrence released
  -> Route Camera returns

Rejoin
  -> fresh Player occurrence
  -> WaitingForActorSelection

Select Cow
  -> ActorProfile_Cow
  -> fresh Actor / Camera Subject occurrence
  -> GameplayReady
  -> Third Person operational
```

A Join attempt while Joining is closed is correctly rejected as `RejectedJoiningClosed`; this is expected Session policy behavior.

## Ownership boundary

The sample/game owns:

```text
which ActorProfile choices are presented
button layout and visual presentation
Farmer/Cow concrete Actor presentation authoring
minimal gameplay movement/look
exact Actor Camera observation Transform
Route and Activity Camera Presentation declarations
GameApplication Player -> Camera bindings
```

The Framework owns:

```text
Session Player lifecycle
Actor-selection commit and preparation
Actor occurrence lifetime
Camera Subject publication lifetime
Player explicit Camera Subject selection
Camera Presentation arbitration
Camera Output lifecycle
Default fallback continuity
```

Do not add private Player discovery, direct Session mutation, parallel Actor-selection authority, Player-owned Camera Output / Rig, or a CharacterSelection-specific duplicate of the shared Third Person Camera assets.

## Application-quit teardown status

Earlier Play Mode evidence showed Framework application-quit teardown attempting normal Camera request arbitration after Unity had already invalidated materialized Cinemachine Camera Scenes.

The Framework now contains an application-quit-only terminal Camera Presentation cleanup path so shutdown does not require normal winner/Default re-application after physical Camera scene validity is gone. This does not change normal Route / Activity / Session release semantics.

The updated Framework was rerun in the Character Selection Multiplayer Split-Screen consumer on 2026-09-23. Terminal Activity/Route Presentation release and Session Output teardown completed cleanly, with no recurrence of the invalid Default CinemachineCamera warning.
