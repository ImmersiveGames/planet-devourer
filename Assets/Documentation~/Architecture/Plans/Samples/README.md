# Samples Authoring Guide and Status

Player status reconciled: **2026-08-28**  
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
  Revision 4
```

This file is the **operational guide/status surface** for the active sample-construction program. The ADRs define the strategy; this guide records where current work happens, what is already proven in authoring/Play Mode, and what remains before final UPM release.

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
| 02 | Player | **IN PROGRESS — Scene Player PROVEN; Player Provisioning PROVEN; Character Selection PROVEN; Local Multiplayer BLOCKED** | Pending |
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

Observed Play Mode terminal evidence includes:

```text
Framework boot succeeded
Activity readiness = Ready
blockingIssues = 0
Camera Output initialized
Default Camera Rig = Session Camera Rig
Player gameplay binding READY
Move input received
Look input received
```

Therefore:

```text
Sample 00 implementation work
  CLOSED

UPM promotion/import proof
  PENDING
```

## Sample 01 — Game Flow dated state

Game Flow remains materialized under:

```text
Assets/_Sample/GameFlow/GameFlowShowcase/
```

The previously recorded core proof remains:

```text
Framework boots into the Game Flow HUB
Persistent Content loads correctly
Route_Hub is valid with no Startup Activity
Route_Hub -> Route_BasicFlow succeeds through Transition/Loading envelope
Activity_Basic_A / B switching is proven
Activity-owned scene composition is proven
Activity-local visibility through ActivityContentBinding is proven
content-less Activity_Basic_C is proven
Seamless and Fade Activity presentation policies are proven
contextual Route/Activity BGM behavior is proven
return to HUB restores Activity = None
cycles are repeatable
blockingIssues = 0 in the proven flow
```

This 2026-08-28 Player cleanup does not redefine the Game Flow architecture. Re-open Game Flow only when that sample itself is the active construction cut.

## Sample 02 — Player current state

Player status is governed by FG-ADR-002 Revision 4.

Current sequence:

```text
Getting Started / Minimal Game
  Scene Player
  HostProvisioning = SceneProvided
  CANONICAL / PROVEN

Player Provisioning
  HostProvisioning = ManagerProvisioned
  MATERIALIZED / PLAY MODE PROVEN 2026-08-24

Character Selection
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
  AUTHORING COMPLETE / PLAY MODE PROVEN 2026-08-28

Local Multiplayer
  NEXT PLANNED PLAYER DEMONSTRATION
  PLANNED / BLOCKED
  requires public Slot/device/input ownership/observation contract
```

### Character Selection proven composition

The Character Selection consumer path is now proven:

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
  -> Manager-Provisioned materialization
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

This projection is implemented by the sample-owned `CharacterSelectionActorButtonPresenter`; it does not own Session or Actor selection state.

### Character Selection framework evidence

The `LeaveUnresolved` reconcile defect found during consumer proof was corrected in the Framework. A Joined Slot with no Actor now remains a legitimate pending state instead of attempting Default Actor resolution.

Full Player certification after the fix:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
sessionChangeObservation = PASS
designerEventProjection = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

Character Selection therefore closes authoring/proving work. No additional sample-owned runtime authority is required.

### Current public Player Session surface

```text
PlayerSessionObserver
  read-only observation / designer-facing lifecycle projection

PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionSelectActorCommandTrigger
PlayerSessionDefaultActorSelectionCommandTrigger
PlayerSessionReplaceActorSelectionCommandTrigger
PlayerSessionClearActorSelectionCommandTrigger
PlayerSessionLeaveCommandTrigger
```

The sample program must not introduce internal Player discovery, direct Session mutation, parallel Actor selection, hidden fallback, physical hot swap or Local Multiplayer device/input architecture.

### Next Player gate

Local Multiplayer cannot begin as a normal sample until the Framework exposes a sufficient public contract for:

```text
local participant / device intent
  -> Slot association
  -> Player admission
  -> correct input ownership/routing
  -> observable Slot / device / control-scheme state
  -> release/reuse when applicable
```

The current ordinary Join surface does not provide exact-Slot public Join and does not establish a complete durable Slot-to-device/InputUser contract.

Do not hide this blocker with sample-owned Slot/device/input authority.

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
  Player Provisioning / ManagerProvisioned Play Mode proof
  Character Selection / LeaveUnresolved explicit Actor selection Play Mode proof
  Character Selection observer-driven UI presentation
  Character Selection ActorProfile DisplayName/Icon projection

Next planned
  Local Multiplayer

Blocked before implementation
  public exact-Slot/device/input ownership and observation contract
```
