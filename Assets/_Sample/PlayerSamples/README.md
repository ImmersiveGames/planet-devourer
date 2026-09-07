# Player Samples

Status: **PLAYER SCOPE GOVERNED BY FG-ADR-002 REVISION 6 — LOCAL MULTIPLAYER BIDIRECTIONAL LEAVE-REJOIN + DEVICE OWNERSHIP PROVEN 2026-09-07**

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
| Local Multiplayer | `ManagerProvisioned` + two configured Slots | **MATERIALIZED / BIDIRECTIONAL LEAVE-REJOIN + DEVICE OWNERSHIP PROVEN — 2026-09-07** | Two-Player Join/Leave/Rejoin, canonical occupancy/ownership-driven UI and placement lifecycle |

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

IPlayerSessionScopedAccess
  = provider-neutral scoped Session observation + request surface

explicit Player Session Command Trigger
  = authored request/change component
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

`PlayerSessionObserver` remains read-only. `IPlayerSessionScopedAccess` provides the current scoped Session snapshot/observation surface, including `Changed` and `TryGetObservation(...)`, without requiring a global Session authority.

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

## Local Multiplayer — active construction target

Local Multiplayer is materialized under:

```text
Assets/_Sample/PlayerSamples/LocalMultiplayer/
```

The historical August blocker for public Slot/device/input ownership and observation was re-audited against the current Framework and is **closed for the implemented path**.

The current consumer flow is:

```text
Open Joining
  -> InputDevice intent
  -> PlayerSessionJoinCommandTrigger
  -> Framework allocates next available configured Slot
  -> Local Player Host admitted
  -> configured Default Actor selected/prepared
  -> Presentation materialized
  -> Activity placement
  -> GameplayReady
```

The sample has two configured Slots and uses current scoped observation as its UI and device-ownership source of truth:

```text
IPlayerSessionScopedAccess.Changed
  -> deferred/coalesced refresh
IPlayerSessionScopedAccess.TryGetObservation(...)
PlayerSessionScopedObservationSnapshot.Slots
PlayerSessionScopedSlotObservation.IsJoined
PlayerSessionScopedSlotObservation.InputOwnership
```

Therefore tutorial state is derived from current occupancy, not historical Join count:

```text
P1 Available + P2 Available
  -> Waiting for Player 1

P1 Joined + P2 Available
  -> Waiting for Player 2

P1 Available + P2 Joined
  -> Waiting for Player 1

P1 Joined + P2 Joined
  -> Completed
```

An already-owned device is filtered by the tutorial before the Join command is invoked. Framework ownership validation remains authoritative.

### Local Multiplayer Play Mode proof history — 2026-09-07

The first consumer validation proved:

```text
Open Joining
-> Join P1
-> P1 Leave
-> P1 returns to Available
-> UI returns to Waiting for Player 1
-> P1 Rejoin creates a fresh occurrence
-> authored placement is reapplied
-> UI still waits for P2
-> Join P2
-> both Players active / Activity readiness completed
-> Leave P1 while P2 remains active
-> UI = Player 2 joined; waiting for Player 1
-> Rejoin P1
-> placement reapplied
-> Activity returns to completed readiness
-> UI returns to both Players joined
```

A later same-day run extended that proof to the inverse direction and input ownership:

```text
P2 Leave while P1 remains active
-> P1 ownership preserved
-> Device 1 remains P1-owned
-> Device 1 is blocked by tutorial for P2 Join
-> Device 2 rejoins P2

P1 Leave while P2 remains active
-> P2 ownership preserved
-> Device 2 remains P2-owned
-> Device 2 is blocked by tutorial for P1 Join
-> Device 1 rejoins P1

repeated P1/P2 Leave/Rejoin cycles
-> six successful Joins total
-> six successful Leaves total
-> current occupancy remains correct after every cycle
```

The validated happy path contained no `RejectedDeviceAlreadyOwned`, no transient `RegisteredHost.NotRegistered`, no failed ownership diagnostic, no warning and no error.

Current Full Player QA is also certified `17/17`, including the ownership-preservation, Leave/Rejoin reconciliation and relocation contracts exercised by this consumer flow.

### Local Multiplayer remaining proof

The application is not closed yet. Remaining consumer work is:

```text
actual gameplay input no-cross-control between P1 and P2
explicit extra-Join behavior while both configured Slots are occupied
Close Joining behavior while current Players remain preserved
Reopen Joining behavior when applicable
```

P2 Leave/Rejoin and distinct P1/P2 current device ownership are now proven and are no longer pending items.

Do not invent sample-owned Slot, device or input authority for the remaining cases.

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

Character Selection satisfies this gate and is closed on the current composition. Local Multiplayer also satisfies the public-surface gate for its current Join/Leave/Rejoin/occupancy/device-ownership slice and remains in active construction for the remaining multiplayer behavior.