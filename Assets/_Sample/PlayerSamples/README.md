# Player Samples

Status: **PLAYER SCOPE GOVERNED BY FG-ADR-002 REVISION 4 — CHARACTER SELECTION AUTHORING PROVEN 2026-08-28**

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
| Character Selection | `ManagerProvisioned` + `ActorResolution = LeaveUnresolved` | **AUTHORING COMPLETE / PLAY MODE PROVEN** | Explicit game-owned Actor choice through public Player observation/commands |
| Local Multiplayer | final application model pending public contract | **PLANNED / BLOCKED** | Requires sufficient public Slot/device/input ownership/observation contracts |

This is the current implementation sequence, not a permanent closed catalog.

## Player Provisioning

Player Provisioning demonstrates the smallest coherent public consumer path for Session-authorized Local Player Host provisioning:

```text
Player Provisioning authority
  -> Local Player Host Prefab
  -> explicit Join request
  -> Local Player Host instance
  -> configured Default Actor selection / preparation
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
  = read / presentation observation

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

`PlayerSessionObserver` remains read-only. It may be composed where Session / Slot / Actor state presentation is needed and does not become another Player authority.

## Character Selection — authoring complete

Character Selection is materialized and consumer Play Mode proven.

Its canonical initial Session intent is:

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

The proven flow is:

```text
Open Joining
  -> Join
  -> Slot Joined
  -> Actor unresolved
  -> Preparing / WaitingForActorSelection
        ↓
PlayerSessionObserver.OnPlayerJoined
  -> show Character Selection UI
        ↓
application-owned ActorProfile choices
  -> Farmer / Cow
        ↓
PlayerSessionSelectActorCommandTrigger.Invoke()
        ↓
Framework-owned selection commit
  -> Actor preparation
  -> physical materialization
  -> Activity participation / GameplayReady
        ↓
PlayerSessionObserver.OnActorSelected
  -> hide Character Selection UI
```

Leave/Rejoin returns to `WaitingForActorSelection` and supports another explicit choice without passing through a failed readiness state.

### Character Selection presentation boundary

The sample owns presentation and choice catalog only.

`CharacterSelection_UI.unity` uses a Route-scoped `PlayerSessionObserver` to show/hide the selection controls:

```text
On Player Joined  -> show
On Actor Selected -> hide
On Player Left    -> hide
```

Each button keeps `PlayerSessionSelectActorCommandTrigger` as the authority for the Actor choice.

The sample-owned `CharacterSelectionActorButtonPresenter` reads that command's `ActorProfile` and projects:

```text
ActorProfile.DisplayName -> label
ActorProfile.Icon        -> image
```

No second ActorProfile reference is authored in the presenter.

### LeaveUnresolved semantics

For Character Selection:

```text
Joined + no selected Actor
  = valid pending state
  = Preparing / WaitingForActorSelection
```

The Framework does not invoke Default Actor resolution in this state. Explicit selection advances the lifecycle.

Normal Character Selection demonstrates **initial explicit Select** only. Do not expose Replace/Clear merely because those APIs exist, and do not use Default selection as a hidden fallback.

`Replace Actor Selection` is not physical hot swap. After Actor preparation, the canonical preparation barrier prevents logical replacement that would imply replacing the prepared physical Actor.

### Character Selection proof

Consumer Play Mode proved both Actor choices and Leave/Rejoin.

Framework Full Player certification also reports:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
sessionChangeObservation = PASS
designerEventProjection = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

Character Selection is therefore closed for authoring/proving under `Assets/_Sample/`. Final UPM promotion/import proof remains pending at the Player group level.

## Scoped binding and command availability

Keep authoring configuration and runtime availability distinct:

```text
valid Route / Activity authoring
  !=
current scoped access Bound
```

A valid command can temporarily be runtime-unbound and must reject without global lookup, alternate Session authority or direct mutation.

Presentation gating must use public scoped observation/binding evidence rather than a fallback authority.

## Local Multiplayer

Local Multiplayer is the next planned Player demonstration, but it remains blocked by a different product contract.

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
  -> consider promotion of reusable presentation/content to Player/Shared
```

Application/session authority always remains local to the owning Demonstration Application.

Current authoring may reuse compatible content between Player demonstrations. Final UPM organization may promote genuinely reusable presentation/content when that improves clarity, but reuse does not authorize shared application/session authority.

## Public-surface gate

Player samples consume public/product Framework APIs.

If a required public Player contract is missing, the demonstration remains **PLANNED / BLOCKED**. Sample code must not hide a product gap with internal discovery, reflection, direct runtime mutation, parallel registries or silent fallbacks.

The Character Selection gate is satisfied and its authoring proof is complete. The Local Multiplayer gate is not.
