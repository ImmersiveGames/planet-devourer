# Player Provisioning (Manager-Provisioned runtime mode)

Status: **CAMERA-032-D/E MIGRATED — Join / Leave / Rejoin validated; Framework application-quit Camera teardown issue pending**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

`Player Provisioning` is the product-facing Demonstration Application for:

```text
HostProvisioning = ManagerProvisioned
```

The existing folder/asset names may still use `ManagerProvisioned`; runtime terminology remains valid. The product composition demonstrated here is **Player Provisioning**: Session-authorized authority creates a Local Player Host when Join is requested.

## Purpose

Demonstrate the smallest coherent public consumer path for:

```text
Player Provisioning authority
  -> explicit Join request
  -> Local Player Host creation
  -> default Actor selection/preparation
  -> physical Actor materialization
  -> Activity participation / readiness
  -> gameplay input
  -> Actor Camera Subject
  -> explicit Player Camera Presentation selection
  -> Activity Third Person Camera request
  -> Leave / Rejoin with a fresh Actor / Subject occurrence
```

Application count represents materially different application/session intent, not feature count. Compatible Player capabilities remain Scenarios by default.

## Current composition

```text
one GameApplication
one PlayerSessionProfile
HostProvisioning = ManagerProvisioned
one supported Player Slot
initial Joining open
Persistent Content
one Route
one Startup Activity
Local Player Provisioning authority
Local Player Host prefab
Logical Player Actor
initial placement
minimal movement / look

one Session Camera Output
one Output-owned Fixed Default Camera Rig
one Route-owned Fixed Camera Presentation
one Activity-owned Player Third Person Camera Presentation
Player Slot -> Output binding
Player Slot -> Presentation binding
ExplicitSelection Player Subject policy

Activity-owned ambient BGM
```

## Player runtime evidence

Validated Play Mode behavior:

```text
Player Session
  -> ManagerProvisioned
  -> one supported Slot
  -> Joining open

Provisioning runtime
  -> Ready

Before Join
  -> WaitingForJoin
  -> hostCount = 0
  -> Slot available

Join
  -> SucceededJoined
  -> Local Player Host created
  -> Actor selected/prepared/materialized
  -> gameplay admitted
  -> GameplayReady

Movement / Look
  -> minimal WASD movement
  -> horizontal Look rotates Player yaw
  -> vertical Look controls camera pitch

Leave
  -> SucceededLeft
  -> current Player occurrence released
  -> Slot returns to Available
  -> Actor / Camera Subject occurrence released

Rejoin
  -> second Join succeeds
  -> a new Local Player Host occurrence is created
  -> a new Actor occurrence is materialized
  -> a new Camera Subject occurrence becomes the explicit Player selection
```

## Camera composition

Physical Camera capacity belongs to the GameApplication Camera Session:

```text
GameApplication_ManagerProvisioned
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings
      PlayerSlotProfile_ManagerProvisioned
        -> CameraOutput_Main

    Player Presentation Bindings
      PlayerSlotProfile_ManagerProvisioned
        -> CameraPresentation_ManagerProvisioned_Player

    Session Camera Presentations
      none
```

The shared Output owns the persistent physical Camera, CinemachineBrain and Default Rig:

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
      Behavior = CameraBehavior_Fixed
```

The Output Default is infrastructure fallback for transition/menu continuity. Sample-specific framing belongs to Route or Activity Camera Presentations, not to Output variants unless the physical Output itself is materially different.

### Route Presentation

The Route owns the fixed presentation used before a Player joins:

```text
Route_ManagerProvisioned
  Camera Presentations
    CameraPresentation_ManagerProvisioned_Route

CameraPresentation_ManagerProvisioned_Route
  Output = CameraOutput_Main
  Rig = PF_ManagerProvisioned_Route_Presentation
  Subject Policy = AllAvailableSubjects
  Transition = Cut
  Request Precedence = 200

PF_ManagerProvisioned_Route_Presentation
  CameraRigComposer
    Behavior = CameraBehavior_Fixed
    Cinemachine Camera
```

Because the rig is Fixed, it does not require Follow or LookAt targets. `AllAvailableSubjects` does not make this rig follow a Player.

### Player Activity Presentation

The Activity owns the contextual Third Person presentation:

```text
Activity_ManagerProvisioned
  Camera Presentations
    CameraPresentation_ManagerProvisioned_Player

CameraPresentation_ManagerProvisioned_Player
  Output = CameraOutput_Main
  Rig = PF_ManagerProvisioned_Player_ThirdPerson_Presentation
  Subject Policy = ExplicitSelection
  Transition = Blend
  Request Precedence = 300

PF_ManagerProvisioned_Player_ThirdPerson_Presentation
  CameraRigComposer
    Behavior = CameraBehavior_ThirdPerson
    Cinemachine Camera
      CinemachineThirdPersonFollow
```

The Actor Presentation supplies Camera Subject evidence only:

```text
Manager Provisioned Actor Presentation
  MinimalThirdPersonLook
    Tracking Pivot
      -> same Transform used by Camera Subject

  ActorCameraSubjectAuthoring
    Observation Transform
      -> Tracking Pivot
```

The Player does not own a physical Camera Output or Camera Rig.

### Camera arbitration

```text
Boot / before Join
  -> Route Presentation [200] is effective

Join
  -> Actor occurrence materializes
  -> ActorCameraSubjectAuthoring publishes the Camera Subject
  -> Player Slot selects that exact current Subject
  -> Activity Player Presentation [300] becomes eligible
  -> Third Person overrides the Route Presentation

Leave
  -> current Actor / Subject occurrence is released
  -> Player Presentation loses eligibility
  -> Route Presentation [200] becomes effective again

Rejoin
  -> new Actor occurrence
  -> new Camera Subject occurrence
  -> same live Player Presentation receives the new exact Subject
  -> Third Person becomes effective again
```

Persistent Content intentionally contains no Camera authority:

```text
ManagerProvisioned_Persistent
  Local Player Provisioning
  Audio Runtime
  EventSystem
  supporting UI

  no CameraOutputAuthoring
  no CameraSharedComposition
  no legacy Player Camera policy authoring
```

The `PlayerInputManager` remains part of Local Player provisioning / physical split-screen authority. It is not Camera composition authority.

## Audio as supporting/ambient composition

Audio is not the primary subject of this Player demonstration. It is used naturally as a transversal supporting/ambient feature.

Current BGM composition:

```text
Route
  no FrameworkRouteBgmBinding

Activity: Manager Provisioned
  FrameworkActivityBgmBinding
    BGM = BGM_Antiguidade
```

Observed runtime result:

```text
Activity Enter
  -> FrameworkBgmDirector
  -> operation = Apply
  -> outcome = Applied
  -> requestedBgm = BGM_Antiguidade
  -> confirmedBgm = BGM_Antiguidade
```

`FrameworkActivityBgmBinding` does not require a `FrameworkRouteBgmBinding`. Route and Activity BGM authoring are independent.

## Run / observe

```text
Play
  -> application boots
  -> Route Fixed Camera Presentation becomes effective
  -> Startup Activity enters
  -> Activity BGM applies
  -> provisioning waits for Join

Join
  -> Player Host and Actor materialize
  -> Player Camera Subject is explicitly selected
  -> readiness completes
  -> Third Person Activity Presentation becomes effective
  -> movement/look are available

Leave
  -> current Player / Actor / Camera Subject occurrence is released
  -> Route Fixed Camera Presentation returns

Join again
  -> new Player / Actor / Camera Subject occurrence is provisioned
  -> Third Person Activity Presentation becomes effective again
```

## Inspect

```text
GameApplication_ManagerProvisioned
  -> PlayerSessionProfile_ManagerProvisioned
      HostProvisioning = ManagerProvisioned

  -> Camera Session
      -> PF_CameraOutput_Main
      -> P1 -> CameraOutput_Main
      -> P1 -> CameraPresentation_ManagerProvisioned_Player

  -> Persistent Content
      -> Local Player Provisioning
      -> Audio Runtime
      -> EventSystem
      -> no Camera authority

Route_ManagerProvisioned
  -> CameraPresentation_ManagerProvisioned_Route

Activity_ManagerProvisioned
  -> CameraPresentation_ManagerProvisioned_Player
  -> Activity BGM Binding / BGM_Antiguidade

Logical Player Actor
  -> gameplay input
  -> movement / look
  -> ActorCameraSubjectAuthoring / Tracking Pivot
```

## Validation boundary

The CAMERA-032-D/E consumer behavior is validated for:

```text
Boot
Route Camera before Join
Join -> Third Person
Leave -> Route Camera
Rejoin -> Third Person with a fresh Actor / Subject occurrence
```

A separate Framework shutdown issue remains when leaving Play Mode: Route and Activity Camera Presentation teardown can attempt to reapply a Cinemachine camera after Unity has already invalidated its loaded Scene. That issue belongs to Framework application-quit teardown ordering and is not sample authoring debt.

## Boundary

Do not add sample-owned Player discovery, Slot registries, device authority, hidden Actor mutation, parallel Camera authority or Audio fallback logic.

The sample consumes public Framework surfaces. If a later Player Scenario requires a missing public contract, that is product evidence and must not be hidden inside sample code.
