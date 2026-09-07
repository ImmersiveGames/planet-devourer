# Samples Authoring Guide and Status

Player status reconciled: **2026-09-07**  
Previous Player snapshot: **2026-09-05**  
Previous general construction snapshot: **2026-08-21**

Canonical strategy:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-001-Immersive-Framework-Sample-and-Demonstration-Strategy.md
```

Player-specific authority:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
  Revision 6
```

This file is the **operational guide/status surface** for the active sample-construction program. The ADRs define strategy and boundaries; this guide records where current work happens, what is already proven in authoring/Play Mode, and what remains before final UPM release.

## Current operational baseline

```text
Repository
  ImmersiveGames/planet-devourer

Branch
  main

Authoring workspace
  Assets/_Sample/
```

The older `FirstGame` branch references are historical context. Current implementation work and operational truth are on `main`.

## Current sample progress

| Order | Group / Demonstration | Authoring / Play Mode | UPM release |
|---|---|---|---|
| 00 | Getting Started / Minimal Game | **COMPLETE / PROVEN** | Pending promotion + Package Manager import proof |
| 01 | Game Flow / GameFlowShowcase | **MATERIALIZED / core flow proven in dated snapshot** | Pending |
| 02 | Player | **IN PROGRESS — Scene Player PROVEN; Player Provisioning PROVEN; Character Selection CLOSED/REPROVEN; Local Multiplayer bidirectional Leave/Rejoin + device ownership PROVEN** | Pending |
| 03 | Advanced Context | Planned | Pending |
| 04 | Persistence | Planned | Pending |

## Sample 00 — Getting Started closure

Getting Started / Minimal Game satisfies the current authoring-phase goal:

```text
one GameApplication
one PlayerSessionProfile
Persistent Content
one Route
one Activity
one gameplay scene
Scene-Provided Player
GameplayReady participation
Mounted / First Person Camera
minimal movement/look Input
```

Sample 00 implementation work remains **CLOSED**. UPM promotion/import proof remains **PENDING**.

## Sample 01 — Game Flow dated state

Game Flow remains materialized under:

```text
Assets/_Sample/GameFlow/GameFlowShowcase/
```

The previously recorded core proof remains valid. This Player reconciliation does not redefine the Game Flow architecture.

## Sample 02 — Player current state

Player status is governed by FG-ADR-002 Revision 6. The current proof progression below is operational status and does not change that ADR's architecture decision.

Current sequence:

```text
Getting Started / Minimal Game
  Scene Player
  HostProvisioning = SceneProvided
  CANONICAL / PROVEN

Player Provisioning
  HostProvisioning = ManagerProvisioned
  MATERIALIZED / PLAY MODE PROVEN

Character Selection
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
  CLOSED / PLAY MODE REPROVEN 2026-09-05

Local Multiplayer
  HostProvisioning = ManagerProvisioned
  two configured local Player Slots
  MATERIALIZED
  BIDIRECTIONAL LEAVE / REJOIN + DEVICE OWNERSHIP PLAY MODE PROVEN 2026-09-07
  ACTIVE CONSTRUCTION CONTINUES
```

### Current shared Player prefab baseline

The Player prefab rebuild established concrete reusable technical composition under:

```text
Assets/_Sample/PlayerSamples/Shared/Prefabs/
  FG_Player.prefab
  FG_PlayerActor.prefab
  FG_Presentation.prefab
```

This shared layer contains reusable technical/presentation composition only. Application/session authority remains local to each Demonstration Application.

### Character Selection historical lifecycle proof

The original consumer path, proven on 2026-08-28, remains the canonical lifecycle:

```text
Open Joining
  -> Join
  -> Joined Slot + unresolved Actor
  -> Preparing / WaitingForActorSelection
        ↓
PlayerSessionObserver.OnPlayerJoined
  -> show Character Selection Controls
        ↓
Farmer / Cow ActorProfile choices
  -> PlayerSessionSelectActorCommandTrigger
        ↓
Framework selection commit
  -> Actor preparation
  -> physical materialization
  -> Activity participation / GameplayReady
        ↓
PlayerSessionObserver.OnActorSelected
  -> hide Character Selection Controls
```

Leave/Rejoin returns to `WaitingForActorSelection` and supports another explicit Actor choice.

The selection buttons project presentation from the same `ActorProfile` that the command will select:

```text
PlayerSessionSelectActorCommandTrigger.ActorProfile
  ├── DisplayName -> label
  └── Icon        -> image
```

### Character Selection current physical composition

The sample was rebuilt after the current Player Actor / Presentation architecture replaced the previous logical-host composition.

Current Actor mapping:

```text
ActorProfile_Farmer
  -> PresentationPrefab = FG_FarmerPresentation

ActorProfile_Cow
  -> PresentationPrefab = FG_CowPresentation
```

Concrete presentations:

```text
Assets/_Sample/PlayerSamples/Player/Players/
  FG_FarmerPresentation.prefab
  FG_CowPresentation.prefab
```

Both concrete variants derive from the shared `FG_Presentation` baseline.

The current teaching chain is therefore:

```text
Join
  -> explicit Actor selection
  -> Actor preparation
  -> Player Actor Runtime Host
  -> Presentation Mount
  -> ActorProfile.PresentationPrefab
  -> selected concrete Presentation
  -> GameplayReady
```

### Character Selection current reproof — 2026-09-05

After the prefab rebuild and `PresentationPrefab` migration, consumer Play Mode was rerun successfully.

Observed closure covers:

```text
Join
-> WaitingForActorSelection
-> select Farmer / Cow
-> correct PresentationPrefab materialized
-> Follow camera functional
-> gameplay movement/input functional
-> GameplayReady
-> Leave
-> Rejoin
-> fresh explicit Actor selection functional
```

The 2026-08-28 Full Player certification remains historical technical evidence:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
sessionChangeObservation = PASS
designerEventProjection = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

Character Selection is therefore **closed for authoring/proving under `Assets/_Sample/` on the current Player composition**.

### Current public Player Session surface

```text
PlayerSessionObserver
  read-only observation / designer-facing lifecycle projection

IPlayerSessionScopedAccess
  scoped Session snapshot/observation + current product request surface
  Changed
  TryGetObservation(...)

PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionSelectActorCommandTrigger
PlayerSessionDefaultActorSelectionCommandTrigger
PlayerSessionReplaceActorSelectionCommandTrigger
PlayerSessionClearActorSelectionCommandTrigger
PlayerSessionLeaveCommandTrigger
```

The sample program must not introduce internal Player discovery, direct Session mutation, parallel Actor selection, parallel Slot/device ownership, hidden fallback or sample-owned Player runtime authority.

### Local Multiplayer historical blocker — closed for current path

The previous blocker, recorded in August, was insufficient public Slot/device/input ownership and observation for a normal consumer.

That boundary was re-audited against the current Player Framework surface before construction. The implemented sample now has a canonical public path equivalent to:

```text
InputDevice intent
  -> PlayerSessionJoinCommandTrigger
  -> Framework-controlled available Slot allocation
  -> Local Player Host / PlayerInput admission
  -> Framework input ownership
  -> configured Actor preparation
  -> Activity placement / GameplayReady

current Session state
  -> IPlayerSessionScopedAccess.Changed
  -> deferred/coalesced refresh
  -> TryGetObservation(...)
  -> typed per-Slot IsJoined + InputOwnership evidence
  -> tutorial presentation / local device guard
```

The sample does not contain a parallel Slot registry, device ownership authority or gameplay input router.

### Local Multiplayer materialized composition

Current application root:

```text
Assets/_Sample/PlayerSamples/LocalMultiplayer/
```

Current application-owned core assets include:

```text
GameApplication_LocalMultiplayer.asset

Player/
  PlayerSessionProfile_LocalMultiplayer.asset
  PlayerSlotProfile_LocalMultiplayer_P1.asset
  PlayerSlotProfile_LocalMultiplayer_P2.asset

Routes/
  Route_LocalMultiplayer.asset
  MultiplayerRouteContentProfile.asset

Activities/
  Activity_LocalMultiplayer.asset

Scenes/
  LocalMultiplayer_Persistent.unity
  LocalMultiplayer.unity
  LocalMUltiplayerUI.unity

Scripts/
  LocalMultiplayerJoinInputSource.cs
  LocalMultiplayerJoinTutorialController.cs
  LocalMultiplayerKeyboardGamepadSimulator.cs
```

The keyboard/gamepad simulator provides deterministic sample InputDevices only; Slot allocation and ownership remain Framework-owned.

### Local Multiplayer Play Mode proof history — 2026-09-07

The first consumer run proved:

```text
Open Joining
-> Join P1
-> P1 prepared / materialized / GameplayReady
-> UI waits for P2

Leave P1
-> exact occurrence released
-> player.1 Available
-> UI returns to Waiting for Player 1

Rejoin P1
-> fresh player.1 occurrence
-> new host / Actor materialization
-> authored placement reapplied
-> UI remains waiting for P2

Join P2
-> player.2 joined
-> both Players active
-> Activity readiness completed
-> UI Completed

Leave P1 while P2 remains Joined
-> P2 remains active
-> UI = Player 2 joined; waiting for Player 1

Rejoin P1
-> fresh occurrence
-> placement reapplied
-> Activity readiness completes again
-> UI Completed
```

A later same-day consumer run proved the inverse direction and the tutorial ownership boundary:

```text
P2 Leave while P1 remains Joined
-> P1 remains current
-> P1 retains Device 1
-> tutorial rejects Device 1 locally for P2
-> no duplicate-device Join request reaches Framework
-> Device 2 rejoins P2

P1 Leave while P2 remains Joined
-> P2 remains current
-> P2 retains Device 2
-> tutorial rejects Device 2 locally for P1
-> no duplicate-device Join request reaches Framework
-> Device 1 rejoins P1

repeated cycles
-> six successful Joins
-> six successful Leaves
-> occupancy remains canonical after each cycle
```

The observation timing fix is also consumer-proven:

```text
PlayerSessionChange
-> invalidate / mark refresh pending
-> Update performs deferred/coalesced canonical observation
-> Joined Slot ownership read only from stable scoped snapshot
```

The happy-path run contains no `RejectedDeviceAlreadyOwned`, no `RegisteredHost.NotRegistered`, no failed ownership diagnostic, no warning and no error.

Current Framework Full Player certification is:

```text
PLAYER QA CERTIFIED
17/17
```

The QA aggregate includes ownership preservation, Leave/Rejoin reconciliation and Activity relocation contracts used by this sample.

### Local Multiplayer remaining work

Local Multiplayer is not closed yet.

Next proof cut:

```text
actual gameplay no-cross-control between P1 and P2
explicit attempt to Join when both configured Slots are already occupied
Close Joining while current Players remain joined
Reopen Joining behavior when applicable
```

P2 Leave/Rejoin preservation and distinct P1/P2 current device ownership are now proven and no longer belong in the remaining-work list.

Additional camera/output topology is not implied by the current proof.

## Completion vocabulary

Do not collapse authoring completion and package release into one status.

```text
AUTHORING COMPLETE / PLAY MODE PROVEN
  sample content is materialized under Assets/_Sample/
  intended runtime behavior works in the authoring/proving project
  canonical composition is inspectable

UPM RELEASE COMPLETE
  mature group is promoted to com.immersive.framework/Samples~/
  package.json samples metadata is active
  selected group is imported through Package Manager into a clean consumer project
  imported references/scenes/prefabs/assets resolve
  Play Mode works from the imported copy
```

A sample group may be closed for construction and allow work to advance while its later UPM promotion gate remains pending.

## Authoring workflow

```text
AUTHORING / PROVING
  planet-devourer:main
  Assets/_Sample/
  visible in Unity
  normal Project Browser / Inspector / Play Mode work

GROUP CLOSURE
  verify intended composition
  verify intended observable behavior
  record runtime evidence
  update local/group README and this status guide

FINALIZATION
  resolve cross-demonstration/sample-group reuse deliberately
  materialize/promote mature sample groups into com.immersive.framework/Samples~/
  activate/update package.json samples metadata

RELEASE VALIDATION
  install/use final framework package
  Package Manager -> Import each group
  validate references, scenes, prefabs, ScriptableObjects and Play Mode
```

## Rules

- the current `main` tree is the operational truth for active sample construction;
- `_Sample/` is authoring/proving infrastructure, not the shipped UPM sample root;
- `Assets/_Project/` is the real FIRSTGAME/game structure and is not the sample taxonomy;
- final UPM sample groups must not depend on sibling top-level sample groups;
- reuse inside one Player group may be reorganized during finalization when concrete reuse justifies it;
- application/session authority must not be promoted into `Player/Shared` merely for deduplication;
- final consumer behavior is proven from the Package Manager imported copy;
- finishing a sample's authoring phase does not silently claim UPM release validation;
- a committed/materialized configuration is not marked **PROVEN** until corresponding observable runtime behavior has been verified;
- canonical ADR filenames are stable; structural strategy changes belong in ADRs, ordinary construction progress belongs here and in sample-local READMEs.

## Current Player implementation cut

```text
Sample 02
  Player

Closed
  canonical Scene Player coverage in Getting Started
  Player Provisioning / ManagerProvisioned proof
  Character Selection / LeaveUnresolved lifecycle proof
  Character Selection migration to ActorProfile.PresentationPrefab
  FG_FarmerPresentation / FG_CowPresentation current composition
  Character Selection Follow camera + gameplay input/movement reproof
  Character Selection Leave/Rejoin fresh-selection reproof

Local Multiplayer — proven current slice
  public Slot/device/input blocker re-audited and closed for implemented path
  application / Route / Activity / Session / two Slots materialized
  P1 Join / Leave / Rejoin
  current Slot occupancy-driven UI
  Activity placement reapplied on fresh P1 occurrence
  P2 Join
  both-Player readiness completion
  P1 Leave while P2 remains active
  P1 Rejoin after Activity had completed
  P2 Leave while P1 remains active
  P2 Rejoin
  distinct P1/P2 device ownership from scoped observation
  tutorial blocks already-owned device before Join
  deferred/coalesced stable observation after Session changes
  repeated bidirectional Leave/Rejoin cycles

Next
  actual P1/P2 gameplay no-cross-control proof
  explicit full-Slot extra-Join behavior
  Close/Reopen Joining behavior
```
