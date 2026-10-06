# Local Multiplayer Assets

Status: **IF-ADR-038 / IF-ADR-042 SharedGroup consumer manual validation PASS — 2026-10-06**

This file records the current two-Player sample composition and its startup Session Camera Assignment. Session Camera is the single writer for Assignment activation, occurrence membership/projection, Output routing, and fallback coverage.

## Application-owned assets

```text
GameApplication_LocalMultiplayer.asset

Player/
  PlayerSessionProfile_LocalMultiplayer.asset
  PlayerSlotProfile_LocalMultiplayer_P1.asset
  PlayerSlotProfile_LocalMultiplayer_P2.asset
  ActorProfile_FarmerGroup.asset
  ActorProfile_CowGroup.asset
  FG_Player_LocalMultiplayer.prefab
  FG_PlayerActor_LocalMultiplayer.prefab
  FG_FarmerPresentationGroup.prefab
  FG_CowPresentationGroup.prefab

Camera/Assignments/
  CameraAssignment_LocalMultiplayer_Group.asset

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
```

## Player Actor composition

The Local Multiplayer host references `FG_Player_LocalMultiplayer`, which uses `FG_PlayerActor_LocalMultiplayer`. The Actor variant retains `PlayerGameplayInputReader` and `CharacterController`, and owns movement/rotation through `MinimalLocalMultiplayerMovement`. `MinimalPlayerMovement` and `MinimalThirdPersonLook` are absent from this variant. Presentation prefabs do not own gameplay input or movement.

## Reused Camera assets

```text
Assets/_Sample/Shared/
  Camera/Definitions/CameraOutput_Main.asset
  Camera/Behaviors/CameraBehavior_Group.asset
  Prefabs/Cameras/PF_CameraOutput_Main.prefab

Assets/_Sample/PlayerSamples/LocalMultiplayer/
  Camera/Assignments/CameraAssignment_LocalMultiplayer_Group.asset
  Presentation/PF_LocalMultiplayer_Activity_Presentation.prefab
```

## Startup Session Camera Assignment

`GameApplication_LocalMultiplayer.StartupCameraAssignments` contains `CameraAssignment_LocalMultiplayer_Group`. It is activated at startup; no trigger is used.

```text
OccurrenceMode   = SharedGroup
MembershipPolicy = ExplicitPlayerSlots (P1, P2)
TargetPolicy     = MemberActorTargets
Output           = CameraOutput_Main
Rig              = PF_LocalMultiplayer_Activity_Presentation
```

The Assignment creates one occurrence per Assignment/Output. That occurrence persists while membership changes. Session Camera projects all current eligible Actor Subjects into the Framework-owned `CinemachineTargetGroup`, replacing the member set on reconciliation so no duplicate targets accumulate.

```text
Target.Object = Subject.Observation
Target.Radius = Subject.FramingRadius > 0
              ? Subject.FramingRadius
              : CameraBehavior_Group.GroupMemberRadius
Target.Weight = CameraBehavior_Group.GroupMemberWeight
```

The rig contains the materialized Group `CinemachineTargetGroup` and enabled `CinemachineGroupFraming`, configured from `CameraBehavior_Group` (`FramingSize`, `Damping`, `FovRange`, `DollyRange`, and `OrthoSizeRange`).

```text
0 eligible Subjects -> empty TargetGroup; Output fallback active
1 eligible Subject  -> one TargetGroup member; fallback released
N eligible Subjects -> N current members; fallback released
last member leaves  -> empty TargetGroup; fallback active
new member eligible -> fallback released; same occurrence retained
```

Join, Leave, Rejoin and Actor replacement reconcile current membership on the same occurrence. `FovRange.x` must follow the base lens FOV.

## Actor Camera Subject

The Actor root owns `ActorCameraSubjectAuthoring`. Farmer and Cow use a neutral Actor-owned Observation anchor at local `X=0`, `Z=0`, approximately `Y=1`, with `FramingRadius=1`.

The Observation is not `CameraMount` ThirdPerson. That mount is offset and orbits with Actor rotation, which would shift the observed point when the Actor turns. Preserve the shared ThirdPerson `CameraMount` unchanged.

## Consumer validation

Manual Unity validation: **PASS — 2026-10-06**.

```text
1 Player follows translation without orbiting around the Actor
2 Players are both framed
separating Players causes dolly/FOV adjustment while keeping both visible
rotating an Actor in place does not move its Subject
```

This is consumer evidence for SharedGroup behavior. Framework Camera Editor/QA suites and any other unexecuted certification remain pending; see IF-TRACK.

## Explicit boundaries

- One shared physical Output: `CameraOutput_Main`.
- No Route or Activity Camera ownership.
- No parallel Camera arbitration authority.
- No individual Output topology, viewport, or command-boundary changes.
- Session Camera remains the sole writer of Assignment activation and active Camera Output state.
