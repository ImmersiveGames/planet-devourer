# Player Samples

Status: **PLAYER SCOPE GOVERNED BY FG-ADR-002 REVISION 5 — CHARACTER SELECTION CLOSED / REPROVEN 2026-09-05**

Canonical Player sample authority:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
```

General sample-program strategy remains in FG-ADR-001.

## Naming model

The Player samples use short product-facing names while preserving the runtime provisioning terms.

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

It demonstrates the current Scene-Provided Player composition, gameplay readiness, gameplay input binding, camera and minimal Move / Look behavior.

These contracts are intentionally **not duplicated** as a dedicated Scene Player Demonstration Application under Player.

## Current Player demonstrations

| Demonstration | Runtime / initial policy | Status | Meaning |
|---|---|---|---|
| Getting Started / Minimal Game | `SceneProvided` | **CANONICAL / PROVEN** | Scene Player reference |
| Player Provisioning | `ManagerProvisioned` + configured Default Actor | **MATERIALIZED / PLAY MODE PROVEN** | Session-authorized Local Player Host creation/provisioning |
| Character Selection | `ManagerProvisioned` + `ActorResolution = LeaveUnresolved` | **CLOSED / PLAY MODE REPROVEN — 2026-09-05** | Explicit Actor choice on the current Player Actor / Presentation composition |
| Local Multiplayer | public Slot/device/input contract | **NEXT / PRE-IMPLEMENTATION RE-AUDIT** | Re-audit the current Framework contract before sample construction |

This is the current implementation sequence, not a permanent closed catalog.

## Current shared Player prefab baseline

The Player samples now have concrete cross-application reuse, so the shared technical prefab baseline is materialized under:

```text
Assets/_Sample/PlayerSamples/Shared/Prefabs/
  FG_Player.prefab
  FG_PlayerActor.prefab
  FG_Presentation.prefab
```

These prefabs capture reusable technical composition. They do **not** move `GameApplication`, `PlayerSessionProfile`, Route, Activity or application-specific authority into Shared.

Concrete Actor presentation variants are authored separately and may derive from the shared `FG_Presentation` baseline.

## Player Provisioning

Player Provisioning demonstrates the smallest coherent public consumer path for Session-authorized Local Player Host provisioning:

```text
Player Provisioning authority
  -> Local Player Host Prefab
  -> explicit Join request
  -> Local Player Host instance
  -> configured Default Actor selection / preparation
  -> Player Actor Runtime Host
  -> configured Actor Presentation
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

The documented public command family contains:

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

## Character Selection — closed on current Player architecture

Character Selection keeps the same canonical Session intent:

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

The lifecycle originally proven on 2026-08-28 remains:

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

### Current Actor presentation chain

The physical composition was rebuilt after the Player Actor / Presentation architecture changes.

Current Character Selection Actor assets are:

```text
ActorProfile_Farmer
  -> PresentationPrefab = FG_FarmerPresentation

ActorProfile_Cow
  -> PresentationPrefab = FG_CowPresentation
```

The concrete prefabs are authored under:

```text
Assets/_Sample/PlayerSamples/Player/Players/
  FG_FarmerPresentation.prefab
  FG_CowPresentation.prefab
```

Both use the shared `FG_Presentation` technical baseline. The selected `ActorProfile` therefore determines the presentation materialized by the current Player Actor runtime composition.

The old `LogicalActorHostPrefab` composition is not part of the current sample contract.

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

### Character Selection proof history

Historical lifecycle proof — **2026-08-28**:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
sessionChangeObservation = PASS
designerEventProjection = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

Current physical-composition reproof — **2026-09-05**:

```text
Join
-> WaitingForActorSelection
-> Farmer / Cow explicit selection
-> correct PresentationPrefab materialized
-> Follow camera functional
-> gameplay movement/input functional
-> GameplayReady
-> Leave / Rejoin
-> fresh explicit selection functional
```

Character Selection is therefore **closed for authoring/proving under `Assets/_Sample/` on the current Player architecture**. Final UPM promotion/import proof remains pending at the Player group level.

## Scoped binding and command availability

Keep authoring configuration and runtime availability distinct:

```text
valid Route / Activity authoring
  !=
current scoped access Bound
```

A valid command can temporarily be runtime-unbound and must reject without global lookup, alternate Session authority or direct mutation.

Presentation gating must use public scoped observation/binding evidence rather than a fallback authority.

## Local Multiplayer — next construction target

Local Multiplayer is now the next Player sample work item.

The last documented product blocker, confirmed in August, was the lack of a sufficient public boundary for:

```text
local participant / device intent
  -> Slot association
  -> Player admission
  -> correct input ownership/routing
  -> observable Slot / device / control-scheme state
  -> release/reuse when applicable
```

That blocker predates the most recent Player framework cuts. Therefore the **next step is a current Framework public-contract re-audit before prefab/sample construction**.

Until that audit closes the question, do not invent sample-owned Slot, device or input authority and do not assume that the August blocker is either still valid or already solved.

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

`Player/Shared` is created by **concrete reuse**, not by taxonomy alone.

The current shared technical prefab baseline is justified because `FG_Player`, `FG_PlayerActor` and `FG_Presentation` are reused across Player demonstrations.

Canonical rule remains:

```text
used by one Player application
  -> keep local

concretely reused by two or more Player applications
  -> promote reusable technical/presentation content to Player/Shared
```

Application/session authority always remains local to the owning Demonstration Application.

## Public-surface gate

Player samples consume public/product Framework APIs.

If a required public Player contract is missing, the demonstration remains blocked at that boundary. Sample code must not hide a product gap with internal discovery, reflection, direct runtime mutation, parallel registries or silent fallbacks.

Character Selection satisfies this gate and is closed on the current composition. Local Multiplayer proceeds next through contract re-audit, then implementation only if the public surface is sufficient.
