# FIRSTGAME — Current State

Status: Current  
Last updated: 2026-07-28  
Repository: `ImmersiveGames/planet-devourer`

## Inspected Git baseline

```text
planet-devourer head
  ef66f6230cdde576d5ad88ec9ab09bb5466fa963

com.immersive.framework head used for documentation comparison
  91cdb98b1bbf33cc6a48aa08614dafc4713e4344

QAFramework head inspected for context
  4c8fea40949143b2f663de8a6361a7f13ab51a43
```

This document records repository-visible state. A manual Unity result is not considered passed merely because the required assets exist.

## 1. Application and scene topology

`FG_GameApplication.asset` currently provides:

```text
startup Route
four ordered local Player Slot Profiles
Actor duplicate-selection policy
Persistent Content container scene
```

The enabled scenes are:

| Order | Scene | Current role |
|---:|---|---|
| 0 | `FG_UIGlobal` | bootstrap/global UI composition |
| 1 | `FG_Menu` | development entry menu |
| 2 | `FG_Gameplay` | general gameplay route scene |
| 3 | `Conteiner Scene` | Persistent Content and persistent output composition |
| 4 | `SceneProvidedGameplay` | focused Scene-Provided Player test scene |

## 2. Menu and Routes

The menu contains two current route requests:

| Menu entry | Request reason | Target intent |
|---|---|---|
| `Start Game` | `firstgame.start.game` | general gameplay path |
| `Player Local Test` | `firstgame.playerlocal.test` | focused local Player path |

The focused local Player Route is:

```text
Asset
  Assets/_Project/ScriptableObjects/ImmersiveFramework/Routes/FG_PlayerSceneProvider.asset

Route name
  FG Player Scene Provider

Primary scene
  Assets/_Project/Scenes/Gameplay/SceneProvidedGameplay.unity

Startup Activity
  Activity_PlayerLocalProvider
```

## 3. Persistent Content and Camera Output

The application references:

```text
Assets/_Project/Scenes/System/Conteiner Scene.unity
```

as Persistent Content.

The scene contains the persistent presentation hierarchy and the physical Camera Output. Recent Git history also contains scoped/session Camera override work and persistent-output authoring changes.

Current classification:

| Evidence | State |
|---|---|
| Serialized Persistent Content scene | Present in Git |
| Persistent physical Camera Output | Present in Git |
| Session/scoped override implementation | Present in Git |
| Full manual transition result | Record in `TEST-SCENARIOS.md` |

## 4. Pause composition

The repository contains separate Pause usage shapes:

```text
application-only authored Pause request
Player-bound physical Pause input
Player input gate behavior
```

The current Player variant includes:

```text
PausePlayerInputBinding
UnityPlayerInputGateAdapter
```

`Global` remains an action map on the official Player `PlayerInput`; it is not a second global Player.

Current classification:

| Pause path | State |
|---|---|
| Application-only Pause runtime/product path | Runtime Implemented |
| External authored Pause control | Present in Git |
| Player Pause prefab variant | Present in Git |
| Player input gate adapter | Present in Git |
| Manual proof across Route enter/exit | Must be recorded |

## 5. Scene-Provided Player composition

The canonical prefab boundaries currently exist:

```text
Actor_PlayerSceneProvided
  PlayerActorDeclaration
  Actor-owned gameplay components
  movement
  Camera anchors
  Visual

Player_SceneProvided
  PlayerInput
  LocalPlayerHostAuthoring
  SceneLocalPlayerAdmissionAuthoring
  Actor Mount
    Actor_PlayerSceneProvided
```

The outer prefab has serialized evidence for:

```text
Player Slot Profile
Actor Profile
scene Logical Player Actor
ActorProfile-compatible nested prefab source
valid authoring status
```

The focused scene instantiates:

```text
Player_SceneProvided_With_Camera
```

Current classification:

| Evidence | State |
|---|---|
| Base Actor prefab | Present in Git |
| Base Scene-Provided Player prefab | Authoring Ready |
| Route Primary Scene integration | Present in Git |
| Package runtime Route Primary Scene admission | Runtime Implemented |
| Focused Play Mode admission/release pass | Current validation focus |

## 6. Player Camera composition

`Player_SceneProvided_With_Camera.prefab` currently adds:

```text
CameraRigComposer
local CinemachineCamera
CinemachineFollow
PlayerGameplayCameraAuthoring
UnityPlayerInputGateAdapter
```

The serialized Composer contains explicit Follow/LookAt targets, the materialized Cinemachine Camera reference and a successful Apply/Rebuild result.

The Player Actor contains `PlayerGameplayCameraAuthoring` referencing the same rig.

Current classification:

| Evidence | State |
|---|---|
| Camera rig authoring | Authoring Ready |
| Cinemachine materialization | Present in Git |
| Player Camera eligibility authoring | Present in Git |
| Persistent physical output | Present in Git |
| Runtime arbitration/restoration manual proof | Current validation focus |

## 7. Player source coverage

| Source | Framework architecture | Package runtime | FIRSTGAME consumer proof |
|---|---|---|---|
| Scene-Provided | Accepted | Implemented | Current focused scenario |
| Manager-Provisioned | Accepted | Implemented | Not yet rebuilt on current package surface |
| Session-Persistent | Accepted | Not implemented | Blocked by Framework |

All sources must converge into the same Session-scoped `PlayerParticipationRuntimeContext` and typed `PlayerSlotId` authority.

## 8. Known documentation discrepancy closed by this package

Before this update, repository documentation contained two misleading statements:

```text
FIRSTGAME README
  claimed canonical Player/Camera/Pause integration was not authored

Framework Player documentation
  claimed Route Primary Scene admission was not runtime-complete
```

Current Git contradicts both statements:

```text
FIRSTGAME contains the Scene-Provided Player, Pause and Camera compositions
Framework runtime contains Route Primary Scene admission support
```

Manual Play Mode proof remains a separate state and is not inferred from source presence.

## 9. Immediate next work

### Close current scenario

Record the Scene-Provided Player with Camera scenario:

```text
enter Route
admit one Logical Player
bind the configured Slot
adopt the existing Logical Actor
enable movement/input
publish and select Player Camera request
pause and resume
exit Route
release contextual evidence
re-enter without duplicates
```

### Then assemble Manager-Provisioned

Create a separate Route and scene using:

```text
PlayerInputManager
LocalPlayerProvisioningAuthoring
LocalPlayerProvisioningHostRegistration
Player prefab with PlayerInput + LocalPlayerHostAuthoring + empty Actor Mount
explicit framework-authorized join
ordered Slot reservation
```

### Do not assemble Session-Persistent locally

Wait for an official package authoring/runtime cut. FIRSTGAME must not invent the missing Session authority or admission contract.
