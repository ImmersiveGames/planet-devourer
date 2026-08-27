# Samples Authoring Guide and Status

Player status reconciled: **2026-08-26**  
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
  Revision 3
```

This file is the **operational guide/status surface** for the active sample-construction program. The ADRs define the strategy; this guide records where current work happens, what is already proven in authoring/Play Mode, and what remains before final UPM release.

The 2026-08-26 edit reconciles the Player row and Player section after the Framework arbitrary Actor-selection public-surface closure. Unrelated Game Flow construction details below retain the previously recorded snapshot unless explicitly noted.

## Current operational baseline

```text
Repository
  ImmersiveGames/planet-devourer

Branch
  main

Authoring workspace
  Assets/_Sample/
```

The older `FirstGame` branch references are historical Revision 10 context. Current implementation work and operational truth are on `main`.

## Current sample progress

| Order | Group / Demonstration | Authoring / Play Mode | UPM release |
|---|---|---|---|
| 00 | Getting Started / Minimal Game | **COMPLETE / PROVEN** | Pending promotion + Package Manager import proof |
| 01 | Game Flow / GameFlowShowcase | **See dated Game Flow section below** | Pending |
| 02 | Player | **IN PROGRESS — Scene Player PROVEN; Player Provisioning PROVEN; Character Selection NEXT / UNBLOCKED** | Pending |
| 03 | Advanced Context | Planned | Pending |
| 04 | Persistence | Planned | Pending |

### Sample 00 closure

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

Observed Play Mode terminal evidence:

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

### Sample 01 dated Game Flow state

The following Game Flow section preserves the previously recorded construction snapshot and is not the subject of the 2026-08-26 Player reconciliation.

Game Flow is materialized under:

```text
Assets/_Sample/GameFlow/GameFlowShowcase/
```

Recorded application shape:

```text
GameApplication_GameFlow.asset
  Player Session disabled
  Persistent Content -> SCN_GameFlow_Persistence
  Startup Route -> Route_Hub

SCN_GameFlow_Persistence
  persistent Camera / EventSystem / Audio baseline
  Transition adapter -> UnityFadeCurtainEffectAdapter
  Loading adapter -> UnityLoadingSurfaceAdapter

Route_Hub
  primary scene -> SCN_GameFlow_Hub
  no Startup Activity
  BGM intent -> Silence

Route_BasicFlow
  primary scene -> SCN_GameFlow_Basic
  Startup Activity -> Activity_Basic_A

Activities
  Activity_Basic_A
    Activity-owned scene -> SCN_GameFlow_Basic_A
    Visual Transition -> Seamless

  Activity_Basic_B
    Activity-owned scene -> SCN_GameFlow_Basic_B
    Visual Transition -> Seamless

  Activity_Basic_C
    Activity Content Profile -> None
    Activity-owned scene -> None
    Visual Transition -> Fade
```

Recorded authoring/Play Mode evidence for the Basic Flow vertical:

```text
Framework boots into Game Flow HUB
SCN_GameFlow_Persistence is loaded as Persistent Content
Transition adapter count = 1
Loading adapter count = 1
Route_Hub is valid with no Startup Activity
HUB settles with Activity = None
Route_Hub explicit BGM Silence is applied

Route_Hub -> Route_BasicFlow
  Transition = SucceededWithUnitySurface
  Transition effect = Fade
  Loading = SucceededWithUnitySurface
  Route gate applies/releases cleanly
  Activity_Basic_A becomes Ready
  blockingIssues = 0

SCN_GameFlow_Basic remains the Route Primary Scene while Activities switch

Activity-local visibility is proven inside SCN_GameFlow_Basic
  Visitors A -> Activity_Basic_A
  Visitors B -> Activity_Basic_B
  ActivityContentBinding activates/deactivates the correct local content

Activity-owned scene composition is proven
  Activity_Basic_A -> SCN_GameFlow_Basic_A
  Activity_Basic_B -> SCN_GameFlow_Basic_B
  A <-> B loads the target Activity scene and releases the previous Activity scene

Activity A <-> B presentation is proven Seamless
  Transition skipped by authored Activity policy
  Loading presentation skipped by authored Activity policy

Activity C proves a content-less Activity
  Activity Content Profile -> None
  no Activity-owned scene is materialized
  previous A/B Activity scene is released
  Visitors A and Visitors B are hidden
  Activity remains Active + Ready
  blockingIssues = 0

Activity A -> C and B -> C prove Activity Fade
  Transition = SucceededWithUnitySurface
  Transition effect = Fade
  canonical Loading presentation remains skipped by Fade policy

contextual BGM is proven
  Route_Hub Silence
    -> Activity_Basic_A BGM
    -> Activity_Basic_B BGM
    -> Route_Hub Silence

BGM no-request preservation is proven through Activity C
  Activity A -> C preserves Activity A confirmed BGM
  Activity B -> C preserves Activity B confirmed BGM
  owner exit does not mutate the confirmed BGM

return to HUB
  uses the Route transition/loading envelope
  restores Activity = None
  destination Route explicit Silence determines final BGM presentation

cycles are repeatable
blockingIssues = 0 in the proven flow
```

### Sample 02 — Player current state

Player sample status is governed by FG-ADR-002 Revision 3.

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
  NEXT / PUBLIC SURFACE UNBLOCKED 2026-08-26

Local Multiplayer
  PLANNED / BLOCKED
  requires public Slot/device/input ownership/observation contract
```

The Framework public arbitrary Actor-selection gate is closed. The current explicit Player Session command surface includes:

```text
Open Joining
Close Joining
Join
Select Actor
Select Default Actor
Replace Actor Selection
Clear Actor Selection
Leave
```

The Character Selection implementation target is:

```text
Join
  -> Joined Slot + unresolved Actor
  -> game-owned ActorProfile choices
  -> explicit Select Actor command
  -> Framework selection commit
  -> existing Actor preparation / Manager-Provisioned materialization
  -> Activity participation / GameplayReady
```

The sample must not introduce internal Player discovery, direct Session mutation, parallel Actor selection, hidden fallback, physical hot swap or Local Multiplayer device/input architecture.

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
  resolve cross-group Assets/_Sample/Shared dependencies
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
- final groups must not depend on sibling sample groups;
- authoring-only `Assets/_Sample/Shared` cross-group dependencies must be resolved before package finalization;
- final consumer behavior is proven from the Package Manager imported copy;
- finishing a sample's authoring phase does not silently claim UPM release validation;
- a committed/materialized configuration is not marked **PROVEN** until the corresponding observable runtime behavior has been verified;
- the canonical ADR filenames are stable; structural strategy changes belong in the ADRs, while ordinary construction progress belongs here and in sample-local README files.

## Current Player implementation cut

```text
Sample 02
  Player

Closed
  canonical Scene Player coverage in Getting Started
  Player Provisioning / ManagerProvisioned Play Mode proof
  Framework public arbitrary Actor-selection blocker

Next
  Character Selection materialization
  consumer Play Mode proof of LeaveUnresolved -> explicit Select -> preparation -> GameplayReady

Still blocked
  Local Multiplayer public Slot/device/input ownership/observation contract
```
