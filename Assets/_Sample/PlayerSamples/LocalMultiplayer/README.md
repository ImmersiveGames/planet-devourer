# Local Multiplayer

Status: **SharedGroup consumer validation PASS — 2026-10-06**

Local Multiplayer demonstrates two local Players (`P1`, `P2`) joining, reaching `GameplayReady`, and sharing one physical Camera Output. Camera assignment and occurrence behavior follow **IF-ADR-038 / IF-ADR-042**. Session Camera is the sole writer of Assignment activation, membership projection, Output routing, and fallback coverage.

## Player flow

```text
Open Joining
  -> Join from an InputDevice
  -> Framework allocates the next configured Slot
  -> Local Player Host is created/admitted
  -> configured Actor is prepared
  -> Activity placement is applied
  -> GameplayReady
```

Configured Slots are `P1` and `P2` (`player.1`, `player.2`). The keyboard/gamepad simulator supplies deterministic test devices; it does not own Slot assignment, input routing, or Session state.

The sample uses `FG_Player_LocalMultiplayer` and `FG_PlayerActor_LocalMultiplayer`. The Actor variant keeps `PlayerGameplayInputReader` and `CharacterController` as the gameplay input/movement owners and uses `MinimalLocalMultiplayerMovement` for Actor movement and rotation. It does not use `MinimalThirdPersonLook`; turning an Actor must not steer the shared Camera.

## Camera setup — IF-ADR-038 / IF-ADR-042

`GameApplication_LocalMultiplayer.StartupCameraAssignments` activates `CameraAssignment_LocalMultiplayer_Group` at startup. The assignment uses:

```text
OccurrenceMode   = SharedGroup
MembershipPolicy = ExplicitPlayerSlots (P1, P2)
TargetPolicy     = MemberActorTargets
Output           = CameraOutput_Main
Rig              = PF_LocalMultiplayer_Activity_Presentation
```

There is one Assignment occurrence per Assignment/Output, retained through Join, Leave, Rejoin, and Actor replacement. Session Camera reconciles the full eligible Subject set into its Framework-owned `CinemachineTargetGroup`; it replaces the member set on reconciliation and never accumulates duplicate targets.

```text
Target.Object = Subject.Observation
Target.Radius = Subject.FramingRadius > 0
              ? Subject.FramingRadius
              : CameraBehavior_Group.GroupMemberRadius
Target.Weight = CameraBehavior_Group.GroupMemberWeight
```

The Group rig has a materialized, enabled `CinemachineGroupFraming`, configured from `CameraBehavior_Group` (`FramingSize`, `Damping`, `FovRange`, `DollyRange`, and `OrthoSizeRange`). An empty group leaves the TargetGroup empty and the Output uses its fallback. One or more eligible Subjects releases fallback and the same occurrence frames the current group. A newly eligible member exits fallback without recreating the occurrence.

### Actor Camera Subject

The Actor-root `ActorCameraSubjectAuthoring` uses a neutral Actor-owned Observation anchor: local `X=0`, `Z=0`, approximately `Y=1`, with `FramingRadius=1`. Farmer and Cow use the same rule. The Observation must not reference `CameraMount` ThirdPerson: that mount has a positional offset and orbits when the Actor rotates, which would move the observed Subject without Actor translation. The shared ThirdPerson `CameraMount` remains untouched.

## SharedGroup manual validation

Manual Unity validation: **PASS** (2026-10-06).

```text
1 Player: Camera follows translation without orbiting around the Actor
2 Players: both Subjects are framed
Players move apart: GroupFraming uses dolly/FOV to keep both visible
Actor rotation in place: Subject position does not move
```

`CameraBehavior_Group.FovRange.x` must track the base lens FOV. This is a tuning constraint for the sample rig.

This consumer result does not close Framework Camera Editor/QA suites or other IF-ADR-038 validation gates; those remain tracked separately.

## Boundaries

- No Route or Activity Camera ownership is introduced.
- No parallel Camera arbitration authority is introduced.
- The sample keeps one physical Output; no individual Output topology, viewport, or command boundary changes are part of this sample.
- Session Camera remains the only writer of Assignment activation, occurrence membership, Subject projection, Output routing, and fallback coverage.
