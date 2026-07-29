# FIRSTGAME — Current State

Status: Current  
Last updated: 2026-07-28  
Repository: `ImmersiveGames/planet-devourer`

## 1. Inspected and validated baseline

```text
planet-devourer
  116225d50a3c6af976355715d3216c0cb80852eb

com.immersive.framework
  bdb76a06a3b75adc9ac7fa5d3e63fbe457ed5ae2

QAFramework
  64f900a5c26ab07ad37f2e7d6e578e8efcfb72a4
```

Repository-visible state and manual Unity proof are recorded separately. The Scene-Provided Player baseline now has both.

## 2. Application and scene topology

`FG_GameApplication.asset` provides:

```text
startup Route
four ordered local Player Slot Profiles
Actor duplicate-selection policy
Persistent Content scene
```

Enabled scenes include:

| Scene | Role |
|---|---|
| `FG_UIGlobal` | bootstrap/global UI composition |
| `FG_Menu` | development entry menu |
| `FG_Gameplay` | general gameplay Route |
| `Conteiner Scene` | Persistent Content and physical Camera Output |
| `SceneProvidedGameplay` | focused Scene-Provided Player fixture |

## 3. Focused Route

```text
Route asset
  Assets/_Project/ScriptableObjects/ImmersiveFramework/Routes/
    FG_PlayerSceneProvider.asset

Route name
  FG Player Scene Provider

Primary scene
  Assets/_Project/Scenes/Gameplay/SceneProvidedGameplay.unity

Startup Activity
  Activity Player Local Provider
```

Menu entry:

```text
Player Local Test
  reason = firstgame.playerlocal.test
```

## 4. Persistent Content

`Conteiner Scene` owns the Session presentation composition:

- persistent physical Camera Output;
- transition surface;
- loading surface;
- Pause presentation;
- persistent `FrameworkRuntimeHost`.

The host now exposes `Scene-Provided Admissions` under `Advanced / Debug`.

## 5. Scene-Provided Player composition

Canonical shape:

```text
Actor_PlayerSceneProvided
  PlayerActorDeclaration
  Actor gameplay components
  movement
  Camera targets
  Visual

Player_SceneProvided
  PlayerInput
  LocalPlayerHostAuthoring
  SceneLocalPlayerAdmissionAuthoring
  Actor Mount
    Actor_PlayerSceneProvided
```

Focused prefab instance:

```text
Player_SceneProvided_With_Camera
```

The fixture also includes:

- `PlayerGameplayCameraAuthoring`;
- `CameraRigComposer`;
- local Cinemachine Camera;
- `UnityPlayerInputGateAdapter`;
- `PausePlayerInputBinding`;
- Reset Subject/Participant authoring.

## 6. Approved runtime proof

The Scene-Provided baseline is approved for:

```text
boot
Menu → Gameplay
one admitted Logical Player
PlayerSlot:player.1
scene-owned physical Host
adopted Logical Actor
Activity Ready
movement
Player gameplay Camera
Pause / Resume
Object Reset
Group Reset
Activity Restart
Gameplay → Menu
release
same-session reentry
Stop Play Mode
```

Active persistent diagnostic:

```text
Active Count = 1
Occupied Slot Count = 1
Last Status = SucceededAdmitted
Host Evidence Present = Yes
```

Released persistent diagnostic:

```text
Active Count = 0
Occupied Slot Count = 0
Last Status = SucceededReleased
Release Succeeded = Yes
Host Evidence Present = No
```

The previous `Framework identity value must be valid` teardown exception was not reproduced after Activity Restart.

See `FIRSTGAME-SCENE-PROVIDED-PLAYER-VALIDATION-2026-07-28.md`.

## 7. Player source coverage

| Source | Package status | FIRSTGAME status |
|---|---|---|
| Scene-Provided | Implemented | **Comparison baseline approved** |
| Manager-Provisioned | Implemented | Next consumer assembly |
| Session-Persistent | Not implemented | Blocked by package |
| second Player / multiplayer | Future scope | Not tested |

All sources must converge into the same Session `PlayerParticipationRuntimeContext` and typed Slot authority.

## 8. Current documentation state

The current guides now state that:

- Route Primary Scene admission is implemented;
- the Scene-Provided consumer path is manually approved;
- persistent release diagnostics are available;
- the persistent snapshot is diagnostic only;
- `Last Actor` is the stable authored Actor identity;
- post-operation counters and Host-evidence state are explicit;
- the QA formatting smoke is an Editor menu smoke.

## 9. Open finding

During scene unload, Reset Subjects may briefly register again through `update-retry` after SceneReleasing has unregistered them, then unregister during `on-disable`.

This is non-blocking for the approved Player baseline and must be handled in a separate Reset cut.

## 10. Immediate next work

Create a dedicated Manager-Provisioned comparison fixture:

```text
new Route and scene
Persistent Content provisioning registration
manual-join PlayerInputManager
Player prefab with PlayerInput + LocalPlayerHostAuthoring
empty Actor Mount
authorized join command
ordered Slot reservation
explicit rollback proof
```

Preserve the current movement, Camera, Pause and reset behavior so the comparison measures the Player source UX rather than unrelated gameplay changes.

Do not build a Session-Persistent workaround in FIRSTGAME.
