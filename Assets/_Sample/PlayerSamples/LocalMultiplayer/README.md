# Local Multiplayer

Status: **CAMERA-032-C/D CONSUMER UNITY PASS — shared Group Presentation and per-Subject framing proven 2026-09-22**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`, Revision 10.

Canonical Camera authority: `IF-ADR-032 — Camera Unified Authority, Session Outputs, Presentations, Subjects and Lifecycle`.

Local Multiplayer is an active **Player Demonstration Application** under:

~~~text
Assets/_Sample/PlayerSamples/LocalMultiplayer/
~~~

The sample demonstrates a canonical local two-Player Manager-Provisioned flow while using one shared physical Camera Output and one Activity-owned Group Camera Presentation.

## Current purpose

~~~text
Open Joining
  -> request Join from an InputDevice
  -> Framework allocates the next available configured Slot
  -> Local Player Host is created/admitted
  -> configured Default Actor is selected/prepared
  -> Player Actor / Presentation is materialized
  -> Activity placement is applied
  -> GameplayReady
~~~

Configured Slots:

~~~text
player.1
player.2
~~~

The tutorial/UI observes authoritative current Session state rather than counting historical Join operations.

## Public Player consumer boundary

Commands:

~~~text
PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionLeaveCommandTrigger
~~~

Observation:

~~~text
IPlayerSessionScopedAccess
  -> Changed
  -> deferred/coalesced refresh
  -> TryGetObservation(...)
  -> PlayerSessionScopedObservationSnapshot
  -> Slots
  -> IsJoined
  -> InputOwnership
~~~

`PlayerSessionChange` is invalidation only. The tutorial refreshes the canonical snapshot later from `Update`.

Expected UI state:

~~~text
P1 Available + P2 Available
  -> Waiting for Player 1

P1 Joined + P2 Available
  -> Player 1 joined; waiting for Player 2

P1 Available + P2 Joined
  -> Player 2 joined; waiting for Player 1

P1 Joined + P2 Joined
  -> Completed / both Players joined
~~~

The sample-owned keyboard/gamepad simulator only creates deterministic test InputDevices. It does not own Slot assignment, Player input routing or Session state.

## Current materialized assets

~~~text
LocalMultiplayer/
  GameApplication_LocalMultiplayer.asset

  Player/
    PlayerSessionProfile_LocalMultiplayer.asset
    PlayerSlotProfile_LocalMultiplayer_P1.asset
    PlayerSlotProfile_LocalMultiplayer_P2.asset
    ActorProfile_FarmerGroup.asset
    ActorProfile_CowGroup.asset
    FG_FarmerPresentationGroup.prefab
    FG_CowPresentationGroup.prefab

  Routes/
    Route_LocalMultiplayer.asset
    MultiplayerRouteContentProfile.asset

  Activities/
    Activity_LocalMultiplayer.asset

  Presentation/
    PF_LocalMultiplayer_Activity_Presentation.prefab

  Scenes/
    LocalMultiplayer_Persistent.unity
    LocalMultiplayer.unity
    LocalMUltiplayerUI.unity

  Scripts/
    LocalMultiplayerJoinInputSource.cs
    LocalMultiplayerJoinTutorialController.cs
    LocalMultiplayerKeyboardGamepadSimulator.cs
    MinimalLocalMultiplayerMovement.cs
~~~

Reused Camera assets:

~~~text
Assets/_Sample/Shared/
  Camera/Definitions/
    CameraOutput_Main.asset

  Camera/Behaviors/
    CameraBehavior_Group.asset

  Prefabs/Cameras/
    PF_CameraOutput_Main.prefab

Assets/_Sample/PlayerSamples/Shared/Camera/
  CameraPresentation_Default_Route.asset
  CameraPresentation_LocalMultiplayer.asset

  Presentation/
    PF_Default_Route_Presentation.prefab
~~~

## Camera composition — IF-ADR-032

### Session physical Output

The active GameApplication owns physical Camera capacity:

~~~text
GameApplication_LocalMultiplayer
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings
      none

    Player Presentation Bindings
      none

  Session Camera Presentations
    none
~~~

This is a **shared-screen** sample. Two local Players do not imply two physical Outputs.

`PF_CameraOutput_Main` owns the physical Unity Camera, CinemachineBrain and persistent Output Default Rig. Persistent Content no longer owns Camera Output or gameplay Camera composition.

### Route Presentation

~~~text
Route_LocalMultiplayer
  Camera Presentations
    CameraPresentation_Default_Route

CameraPresentation_Default_Route
  Output = CameraOutput_Main
  Rig = PF_Default_Route_Presentation
  Subject Policy = AllAvailableSubjects
  Transition = Cut
  Request Precedence = 200
~~~

The Route Presentation provides the neutral entry view.

### Activity shared Group Presentation

~~~text
Activity_LocalMultiplayer
  Camera Presentations
    CameraPresentation_LocalMultiplayer

CameraPresentation_LocalMultiplayer
  Output = CameraOutput_Main
  Rig = PF_LocalMultiplayer_Activity_Presentation
  Subject Policy = AllAvailableSubjects
  Transition = Blend
  Request Precedence = 300

PF_LocalMultiplayer_Activity_Presentation
  CameraRigComposer
    Behavior = CameraBehavior_Group
    Presentation Intent = Group
    CinemachineTargetGroup
    CinemachineGroupFraming
~~~

There is only one Activity Camera Presentation. Players do not own competing Camera Presentations.

### Actor Camera Subjects and framing radius

Each prepared Actor Presentation publishes Camera evidence:

~~~text
FG_FarmerPresentationGroup
  ActorCameraSubjectAuthoring
    Observation Transform = presentation-owned Group framing anchor
    Framing Radius = authored Farmer extent

FG_CowPresentationGroup
  ActorCameraSubjectAuthoring
    Observation Transform = presentation-owned Group framing anchor
    Framing Radius = authored Cow extent
~~~

The observation target may be an empty GameObject. Its transform supplies the observation position; it does not need Renderer, Collider or scale-derived dimensions.

Group projection is:

~~~text
CinemachineTargetGroup.Target.Object
  = CameraSubject.Observation

CinemachineTargetGroup.Target.Radius
  = CameraSubject.FramingRadius        when > 0
  = CameraBehavior_Group.memberRadius  otherwise
~~~

Therefore it is expected that the materialized Cinemachine Camera follows the framework-owned Target Group rather than an individual Actor transform. The Target Group GameObject itself does not need authored dimensions.

`CameraBehavior_Group.memberRadius` is a fallback. Actor-specific extent belongs in `ActorCameraSubjectAuthoring.FramingRadius`.

## Shared Group lifecycle

~~~text
0 available Subjects
  -> Group Presentation is not eligible
  -> surviving Route/Session request or Output Default remains visible

1 available Subject
  -> same Activity Group Presentation
  -> one Target Group member

2 available Subjects
  -> same Activity Group Presentation
  -> two Target Group members
  -> shared view reframes both

one Player leaves
  -> exact Actor Subject is removed
  -> same Group Presentation reframes remaining members

last Player leaves
  -> no eligible Group request

Rejoin
  -> fresh Actor occurrence
  -> fresh Camera Subject occurrence
~~~

Camera membership derives from Camera Subject availability, not Slot index, Player index, device identity or tutorial state.

## Camera consumer proof — 2026-09-22

Manual Unity Play Mode on the migrated IF-ADR-032 topology confirmed:

~~~text
Camera Session Outputs materialized
  outputCount = 1

Route Camera Presentation materialized
  CameraPresentation_Default_Route
  scope = Route
  transition = Cut
  precedence = 200

Activity Camera Presentation materialized
  CameraPresentation_LocalMultiplayer
  scope = Activity
  transition = Blend
  precedence = 300

P1 joins
  -> Farmer Group Actor prepared/materialized
  -> Camera Subject becomes available
  -> shared Group presentation functional

P2 joins
  -> Cow Group Actor prepared/materialized
  -> second Camera Subject becomes available
  -> same Group presentation frames both

ActorCameraSubjectAuthoring.FramingRadius authored
  -> per-member TargetGroup radius is used
  -> undesired close framing from a dimensionless observation target is corrected
~~~

This proof contributes consumer evidence to IF-ADR-032 CAMERA-032-C and CAMERA-032-D, but it does not certify:

~~~text
CAMERA-032-E Player ExplicitSelection
two physical Outputs
split-screen Camera layout
force-default transition continuity
full IF-ADR-032 QA/certification
~~~

## Player lifecycle proof history

The earlier Player proof remains valid:

~~~text
P1 Join / Leave / Rejoin
P2 Join / Leave / Rejoin
fresh Session Player occurrences
Activity placement reapplied
current occupancy UI
distinct current device ownership
already-owned device blocked before Join
~~~

Current Framework Player QA separately supplies the exhaustive Player runtime proof surface.

## Remaining sample work

Camera architecture migration for this sample is complete for the current shared-screen Group topology.

Remaining sample-level work is limited to concrete demonstration behavior or visual tuning, for example:

~~~text
actual gameplay no-cross-control proof for P1/P2
extra-Join behavior when all configured Slots are occupied
Close/Reopen Joining behavior where still desired
Group framing / offset / damping / FOV tuning
~~~

Full IF-ADR-032 technical closure remains a Framework/QA concern, not a reason to reintroduce sample-owned Camera authority.

## Non-goals

Do not add:

~~~text
sample-owned Slot registry
sample-owned device ownership
sample-owned input routing authority
sample-owned Camera Subject registry
Player-owned Camera Output
Player-owned Camera Presentation for this shared-screen topology
CameraSharedComposition
legacy Player Camera policy authoring
Camera.main authority
reflection
private/internal Player runtime access
a second Session authority
silent fallback
~~~

The Framework remains authoritative for Player Session, Slot allocation, Join/Leave lifecycle, Actor preparation, placement, gameplay admission, input ownership and Camera runtime contracts. The sample owns demonstration interaction, presentation, Actor framing evidence and visual tuning.
