# Local Multiplayer Assets

Status: **CAMERA-032 SHARED-GROUP COMPOSITION MATERIALIZED — UNITY CONSUMER PASS 2026-09-22**

This file records the current Local Multiplayer application and the exact Camera ownership expected after migration to IF-ADR-032.

## Application-owned assets

~~~text
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

## Reused Camera assets

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

## Required GameApplication Camera configuration

~~~text
GameApplication_LocalMultiplayer
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings
      []

    Player Presentation Bindings
      []

  Session Camera Presentations
    []
~~~

The empty Player Camera bindings are intentional. This sample uses one shared physical Output and one shared Activity Group Presentation.

## Required Route Camera configuration

~~~text
Route_LocalMultiplayer
  Camera Presentations
    CameraPresentation_Default_Route

CameraPresentation_Default_Route
  Output = CameraOutput_Main
  Subject Policy = AllAvailableSubjects
  Transition = Cut
  Request Precedence = 200
  Rig = PF_Default_Route_Presentation
~~~

## Required Activity Camera configuration

~~~text
Activity_LocalMultiplayer
  Camera Presentations
    CameraPresentation_LocalMultiplayer

CameraPresentation_LocalMultiplayer
  Output = CameraOutput_Main
  Subject Policy = AllAvailableSubjects
  Transition = Blend
  Request Precedence = 300
  Rig = PF_LocalMultiplayer_Activity_Presentation
~~~

## Required Group Presentation prefab

~~~text
PF_LocalMultiplayer_Activity_Presentation
  CameraRigComposer
    Behavior Definition = CameraBehavior_Group
    Presentation Intent = Group
    materialized CinemachineCamera
    framework-owned CinemachineTargetGroup
    framework-owned CinemachineGroupFraming
~~~

Do not put `CameraOutputAuthoring`, a physical Unity Camera, CinemachineBrain or `CameraSharedComposition` in this Presentation prefab.

## Required Actor Camera evidence

Both Group Actor Presentations require explicit Camera Subject authoring:

~~~text
FG_FarmerPresentationGroup
  ActorCameraSubjectAuthoring
    Observation Transform = Farmer Group framing anchor
    Framing Radius > 0

FG_CowPresentationGroup
  ActorCameraSubjectAuthoring
    Observation Transform = Cow Group framing anchor
    Framing Radius > 0
~~~

The Observation Transform may be an empty GameObject.

For Group framing:

~~~text
Target.Object = Subject.Observation

Target.Radius =
  Subject.FramingRadius        when authored
  CameraBehavior_Group.MemberRadius otherwise
~~~

The behavior-level member radius is fallback only; character-specific extent belongs on the Actor Presentation.

## Current public Player composition

~~~text
Open Joining command
Join command from InputDevice
Leave command per configured Slot

IPlayerSessionScopedAccess.Changed
IPlayerSessionScopedAccess.TryGetObservation(...)

PlayerSessionScopedSlotObservation.IsJoined
PlayerSessionScopedSlotObservation.InputOwnership
~~~

The keyboard/gamepad simulator is test input only. It does not own Slot assignment or Session state.

## Proven current consumer slice

~~~text
GameApplication Camera Session materializes one explicit Output

Route
  -> CameraPresentation_Default_Route

Activity
  -> CameraPresentation_LocalMultiplayer

P1
  -> Farmer Group Actor Presentation
  -> Camera Subject
  -> one-member Group

P2
  -> Cow Group Actor Presentation
  -> Camera Subject
  -> two-member Group

Framing Radius
  -> per-Subject Target Group extent
  -> functional visual correction for dimensionless observation target

P1/P2 Leave/Rejoin
  -> Player lifecycle remains occurrence-safe
  -> Camera Subject membership follows current Actor occurrences
~~~

## Explicitly absent legacy Camera authority

The current sample must not contain or require:

~~~text
Local Multiplayer Camera.prefab
CameraSharedComposition
PlayerCameraOutputPolicyAuthoring
PlayerCameraCompositionPolicyAuthoring
physical Camera Output in Persistent Content
Player-owned Camera request/rig/output
~~~

## Validation boundary

~~~text
Local Multiplayer IF-ADR-032 migration      PASS
one-Output Session consumer proof           PASS
Route Presentation consumer proof           PASS
Activity Group Presentation consumer proof  PASS
AllAvailableSubjects Group consumer         PASS
per-Subject Framing Radius consumer         PASS

CAMERA-032-E ExplicitSelection              NOT EXERCISED BY THIS SAMPLE
two-Output / split-screen Camera             NOT THIS SAMPLE
full IF-ADR-032 certification                PENDING IN FRAMEWORK/QA
~~~

Visual tuning of `CameraBehavior_Group` remains application-owned.
