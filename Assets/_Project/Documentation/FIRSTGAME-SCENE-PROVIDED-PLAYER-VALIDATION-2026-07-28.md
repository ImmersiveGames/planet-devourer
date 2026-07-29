# FIRSTGAME — Scene-Provided Player Validation

Status: **Approved**  
Date: 2026-07-28  
Cut: `PLAYER-DIAG-1`

## 1. Source baseline

```text
planet-devourer
  branch: main
  SHA: 116225d50a3c6af976355715d3216c0cb80852eb

com.immersive.framework
  branch: master
  SHA: bdb76a06a3b75adc9ac7fa5d3e63fbe457ed5ae2

QAFramework
  branch: main
  SHA: 64f900a5c26ab07ad37f2e7d6e578e8efcfb72a4
```

Unity project version:

```text
6000.5.0f1
```

The manual tester did not report a Unity-version change during this validation.

## 2. Focused fixture

```text
Route
  FG Player Scene Provider

Activity
  Activity Player Local Provider

Scene
  SceneProvidedGameplay

Player prefab instance
  Player_SceneProvided_With_Camera

Configured Slot
  PlayerSlot:player.1
```

## 3. Authoring surfaces observed

- `LocalPlayerHostAuthoring`;
- `SceneLocalPlayerAdmissionAuthoring` displayed as **Scene-Provided Player Composer**;
- `PlayerActorDeclaration`;
- `PlayerGameplayCameraAuthoring`;
- `PausePlayerInputBinding`;
- `UnityPlayerInputGateAdapter`;
- Reset Subject Adapter;
- Transform Reset Participant.

## 4. Active admission evidence

The persistent `FrameworkRuntimeHost > Advanced / Debug` projection showed:

```text
Active Count = 1
Occupied Slot Count = 1
Last Operation = AdmitSceneLocalPlayer
Last Status = SucceededAdmitted
Last Slot = PlayerSlot:player.1
Release Succeeded = No
Already Released = No
Host Evidence Present = Yes
```

The Scene-Provided Player Composer showed:

```text
Admission = Admitted
Runtime = Ready
Host Joined = true
Runtime Ready = true
Active Admission = true
Actor Ownership = ExternalSceneOwned
Adoption Status = SucceededAdopted
```

## 5. Released state evidence

After returning to Menu, the persistent projection remained available and showed:

```text
Active Count = 0
Occupied Slot Count = 0
Last Operation = ReleaseSceneLocalPlayer
Last Status = SucceededReleased
Last Slot = PlayerSlot:player.1
Release Succeeded = Yes
Already Released = No
Host Evidence Present = No
```

This is direct evidence of release. A second admission is no longer required merely to infer that the Slot became reusable.

## 6. Activity Restart regression

Executed flow:

```text
Menu
→ Gameplay
→ Activity Restart
→ Menu
→ Stop Play Mode
```

Observed result:

```text
Activity Restart status = Succeeded
resetStatus = Succeeded
resetSubjects = 2
resetParticipants = 2
clearStatus = Succeeded
reentryStatus = Succeeded
blockingIssues = 0
```

The Player Transform returned to its baseline and the Player remained visually valid after reentry.

The tester manually confirmed valid readmission/reentry after Activity Restart.

The prior failure was not reproduced:

```text
ArgumentException: Framework identity value must be valid
PlayerSlotId.StableText
SceneLocalPlayerAdmissionRuntimeHostModule.OnDestroy
```

## 7. Validation matrix

| Check | Result |
|---|---|
| Framework/QA/FIRSTGAME import and initialization | Approved |
| Boot and Menu | Approved |
| Menu → Gameplay | Approved |
| Scene-Provided admission | Approved |
| Slot `player.1` | Approved |
| Host joined | Approved |
| Logical Actor adopted/materialized | Approved |
| Activity Ready | Approved |
| movement | Approved |
| gameplay Camera | Approved |
| Player-bound Pause/Resume | Approved |
| Object Reset | Approved |
| Group Reset | Approved |
| Activity Restart operation | Approved |
| Gameplay → Menu | Approved |
| persistent released snapshot | Approved |
| same-session reentry | Approved |
| Menu → Gameplay → Menu → Stop | Approved |
| Menu → Gameplay → Menu → Gameplay → Menu → Stop | Approved |
| Activity Restart → Menu → Stop | Approved |
| teardown identity regression | Approved / not reproduced |
| Manager-Provisioned Player | Not tested |
| second Player / multiplayer | Outside current scope |
| Session-Persistent Player | Blocked by package gap |

## 8. Product conclusions

The Scene-Provided path is now a stable FIRSTGAME comparison baseline.

A consumer can:

- create or reuse a scene-owned Player prefab;
- understand the Host/Actor/Slot composition;
- Apply/Rebuild and validate authoring;
- observe active admission in the Composer;
- observe release after scene unload in the persistent host;
- use Activity Restart without the previous teardown exception;
- reenter without residual Slot occupation.

The diagnostic projection remains a read-only view. It is not a second runtime authority.

## 9. Field interpretation

`Last Actor` represents the stable authored `PlayerActorDeclaration.ActorId` used by the Scene-Provided composition. Detailed runtime logs may additionally contain an Activity-scoped Actor occurrence identity.

`Host Evidence Present` and the counters describe the state after the recorded operation:

```text
after admit
  Host Evidence Present = Yes

after release
  Host Evidence Present = No
```

## 10. Open finding outside PLAYER-DIAG-1

Reset Subjects may:

```text
unregister during SceneReleasing
→ register again with reason='update-retry'
→ unregister during on-disable
```

This did not block Player behavior or leave a final registration. It remains a separate Reset lifecycle finding.

## 11. Limits

This validation does not prove:

- Manager-Provisioned consumer UX;
- Session-Persistent Player;
- two local Players;
- split-screen;
- multiplayer/reconnect;
- a complete final gameplay loop.

## 12. Next recommended work

Build a separate Manager-Provisioned Route and scene while preserving the same:

```text
movement
Camera
Pause
reset behavior
Activity requirement
```

The comparison should isolate provisioning, Slot reservation, Host creation, transaction rollback and teardown UX.
