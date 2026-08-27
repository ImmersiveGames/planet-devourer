# Player Samples

Status: **PLAYER SCOPE GOVERNED BY FG-ADR-002 REVISION 3 — PUBLIC ACTOR SELECTION UNBLOCKED 2026-08-26**

Canonical Player sample authority:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
```

General sample-program strategy remains in FG-ADR-001.

## Naming model

The Player samples use short product-facing names while preserving the existing runtime provisioning terms.

```text
Local Player Host
  common technical host for one local Player

Scene Player
  Local Player Host already authored in a Scene
  HostProvisioning = SceneProvided

Player Provisioning
  Session/UIGlobal authority that can create Local Player Hosts
  HostProvisioning = ManagerProvisioned
```

`SceneProvided` and `ManagerProvisioned` remain valid runtime modes. They are not presented as two peer Player Host object types.

## Canonical Scene Player coverage

`Assets/_Sample/GettingStarted/MinimalGame/` is the canonical executable reference for the Scene Player flow.

It already demonstrates:

- a Scene-authored Local Player Host;
- `HostProvisioning = SceneProvided`;
- Scene Logical Player authoring;
- admission on Activity entry;
- logical Actor preparation;
- gameplay readiness;
- gameplay input binding;
- Mounted / First Person Camera;
- minimal Move / Look gameplay input.

These contracts are intentionally **not duplicated** as a dedicated Scene Player Demonstration Application under Player.

## Current Player demonstrations

| Demonstration | Runtime / initial policy | Status | Meaning |
|---|---|---|---|
| Getting Started / Minimal Game | `SceneProvided` | **CANONICAL / PROVEN** | Scene Player reference |
| Player Provisioning | `ManagerProvisioned` + configured Default Actor | **MATERIALIZED / PLAY MODE PROVEN** | Session-authorized Local Player Host creation/provisioning |
| Character Selection | `ManagerProvisioned` + `ActorResolution = LeaveUnresolved` | **NEXT / PUBLIC SURFACE UNBLOCKED** | Explicit game-owned Actor choice through public Player commands |
| Local Multiplayer | final application model pending public contract | **PLANNED / BLOCKED** | Requires a sufficient public Slot/device/input ownership/observation contract |

This is the current implementation sequence, not a permanent closed catalog.

## Player Provisioning

Player Provisioning demonstrates the smallest coherent public consumer path for Session-authorized Local Player Host provisioning:

```text
Player Provisioning authority
  -> Local Player Host Prefab
  -> explicit Join request
  -> Local Player Host instance
  -> default Actor selection / preparation
  -> admission
  -> Session ownership
  -> GameplayReady
```

The provisioning object is **not itself a Player Host**.

The application is Play Mode proven, including Join, gameplay, Camera and Leave/Rejoin behavior. See its local README for the exact evidence and composition.

## Current public Player Session surface

Canonical distinction:

```text
PlayerSessionObserver
  = read

explicit Player Session Command Trigger
  = request/change
```

The current public command family contains eight explicit components:

```text
PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionSelectActorCommandTrigger
PlayerSessionDefaultActorSelectionCommandTrigger
PlayerSessionReplaceActorSelectionCommandTrigger
PlayerSessionClearActorSelectionCommandTrigger
PlayerSessionLeaveCommandTrigger
```

Each command component represents one request and owns only its own typed result evidence.

`PlayerSessionObserver` remains read-only and may be composed where Session / Slot / Actor state presentation is needed. It is not required for commands to function and does not route or aggregate command results.

## Character Selection — next Player cut

Character Selection is no longer blocked by missing public arbitrary Actor selection.

Its canonical initial Session intent is:

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

Expected flow:

```text
Join
  -> Slot Joined
  -> Actor unresolved
        ↓
game-owned Character Selection UI
  -> presents application-owned ActorProfile choices
        ↓
PlayerSessionSelectActorCommandTrigger.Invoke()
        ↓
PlayerActorSelectionResult
        ↓
Framework-owned selection commit
        ↓
existing Actor preparation / materialization
        ↓
Activity participation / GameplayReady
```

The sample owns presentation and the choice catalog. The Framework owns Slot validity, selection revision/commit, duplicate policy, preparation, physical materialization and Activity admission.

Normal Character Selection should demonstrate the **initial explicit Select** operation. Do not expose Replace/Clear merely because those APIs exist, and do not invoke Default selection in the normal `LeaveUnresolved` path.

`Replace Actor Selection` is not physical hot swap. After Actor preparation, the canonical preparation barrier rejects logical selection changes that would imply replacing the prepared physical Actor.

## Scoped binding and command availability

Keep authoring configuration and runtime availability distinct:

```text
valid Route / Activity authoring
  !=
current scoped access Bound
```

A valid command can temporarily be runtime-unbound and must reject without global lookup, alternate Session authority or mutation.

This remains tracked as:

```text
PLAYER-COMMAND-SURFACE-READINESS / DEFERRED
```

Character Selection must not hide this debt with a fallback. If UI gating is required, use only public readiness/binding evidence available to the consumer.

## Local Multiplayer

Local Multiplayer remains planned but blocked by a different product contract.

The missing public boundary is sufficient ownership/observation for:

```text
local participant / device intent
  -> Slot association
  -> Player admission
  -> correct input ownership/routing
  -> observable Slot / device / control-scheme state
  -> release/reuse when applicable
```

The current ordinary Join command does not provide exact-Slot public Join and does not by itself provide a complete durable Slot-to-device/InputUser contract.

Do not invent sample-owned Slot, device or input authority.

## Application / Scenario rule

```text
materially incompatible initial Player Session intent
  -> separate Demonstration Application

compatible runtime behavior
  -> Scenario inside that application
```

Character Selection is a separate application because its creation-time Actor Resolution intent is intentionally different:

```text
Player Provisioning
  ResolveConfiguredDefault

Character Selection
  LeaveUnresolved
```

Participation, physical lifetime, placement, Input/Pause and Leave/Rejoin do not automatically create new GameApplications.

Samples demonstrate representative canonical usage; exhaustive combinations belong in QAFramework.

## Player/Shared

`Player/Shared` is **not** a required layer and must not be populated preemptively.

```text
used by one Player application
  -> keep local

concretely reused by two or more Player applications
  -> promote the reusable content to Player/Shared
```

Application/session authority always remains local to the owning Demonstration Application.

An existing empty or placeholder `Shared/` scaffold does not establish shared ownership.

## Public-surface gate

Player samples consume public/product Framework APIs.

If a required public Player contract is missing, the demonstration remains **PLANNED / BLOCKED**. Sample code must not hide a product gap with internal discovery, reflection, direct runtime mutation, parallel registries or silent fallbacks.

The Character Selection gate is now satisfied. The Local Multiplayer gate is not.
