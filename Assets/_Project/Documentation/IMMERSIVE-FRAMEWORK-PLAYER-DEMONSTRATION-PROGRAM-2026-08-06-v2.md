# Immersive Framework — Player Demonstration Program and Product Evolution Plan

**Date:** 2026-08-06  
**Status:** Reorganized — Demo 02 implementation closed; Demo 03 is the next active demonstration  
**Scope:** Player / Local Multiplayer / Participation Scenarios / Logical Actor / Activity Readiness  
**Primary consumer:** `planet-devourer` — FIRSTGAME / Demo 02, Demo 03 and Demo 04  
**Technical validation:** `QAFramework`  
**Official implementation:** `com.immersive.framework`

---

## 1. Purpose

This plan converts the current Player architecture and implementation audit into an ordered program with two priorities:

```text
1. Create demonstrations and real-use tests of the framework.
2. Evolve the framework when those demonstrations reveal missing product surfaces or contracts.
```

The program must avoid turning FIRSTGAME into a permanent laboratory for framework internals. Demonstration identifiers are organized by the FIRSTGAME demo that owns the user-facing proof, rather than by one global `PLAYER-Dxx` sequence.

The intended responsibility split is:

```text
com.immersive.framework
  Official authoring, runtime, contracts, diagnostics and reusable product APIs.

QAFramework
  Technical proofs, negative cases, policy matrices, rollback and regressions.

FIRSTGAME / planet-devourer
  Real-use demonstrations, manual assembly, usability evidence and gameplay integration.
```

---

## 2. Audited source baseline

The analysis used the following repository states:

```text
com.immersive.framework
  95f2626caf0f9e387cc3efd46deff4b3d0831ee2
  commit: IF-M07-12B-7

planet-devourer
  f695d3d180b64f054a5669f8b649d51eb0ec71ae
  commit: Demo 02

QAFramework
  M07-12B close-gate line current on 2026-08-06
```

Relevant package documents and code inspected:

```text
Documentation~/Architecture/ADRs/
  IF-ADR-003-Player-Participation-and-Actor-Lifecycle.md
  IF-ADR-011-Participant-Aware-Activity-Readiness-Loading-Progress.md

Documentation~/Guides/
  Player-Usage.md

Runtime/PlayerParticipation/
  Authoring/LocalPlayerProvisioningAuthoring.cs
  Authoring/LocalPlayerActorSelectionRequestAuthoring.cs
  Contracts/ManagerProvisionedPlayerLifecycleSnapshot.cs
  Contracts/ManagerProvisionedPlayerLifecycleSlotSnapshot.cs
  Runtime/LocalPlayerProvisioningRuntimeHostModule.cs
  Runtime/LocalPlayerProvisioningBridge.cs
  Runtime/PlayerParticipationRuntimeContext.cs

Editor/PlayerParticipation/
  LocalPlayerProvisioningAuthoringEditor.cs
```

Relevant FIRSTGAME code inspected:

```text
Assets/_Project/Demo02/
  README.md
  Scripts/ManagerProvisionedPlayer/Commands/
    ManagerProvisionedPlayerCommandReceiver.cs
```

---

## 3. Canonical Player model

The Player domain must preserve the following separations:

```text
Player Slot
  Stable Session participation seat.

Logical Player
  Session participant associated with one PlayerSlotId.

Local Player Host
  Physical Unity host, normally containing PlayerInput.

Actor selection
  Selected ActorProfile associated with a joined Slot.

Logical Actor
  Runtime Actor identity correlated with the Logical Player.

Actor materialization
  Physical gameplay representation of the Logical Actor.

Activity participation
  Projection and readiness requirements applied by an Activity.
```

The framework recognizes three Logical Player sources:

```text
Manager-Provisioned Logical Player
  Implemented.

Scene-Provided Logical Player
  Implemented.

Session-Persistent Logical Player
  Accepted architecture; not implemented.
```

Rules:

```text
PlayerInput.playerIndex is never PlayerSlotId.

Join does not inherently mean Actor selection.

Actor selection does not inherently mean Logical Actor preparation.

Logical Actor preparation does not inherently mean gameplay readiness.

Activity may prepare and release contextual Actor/gameplay state,
but it does not own the Session identity of a Manager-Provisioned Player.
```

---

## 4. Current implementation assessment

### 4.1 Implemented and demonstrable

```text
Scene-Provided Player from Route Primary Scene.
Scene-Provided Player from Activity content scenes.
Manager-Provisioned Player through manual PlayerInputManager join.
Ordered Slot allocation.
Dynamic capacity.
Opening and closing the joining window.
Default Actor selection policy.
Logical Actor preparation and physical materialization.
Activity participation projections.
Progressive Player readiness requirements.
Activity entry readiness gating.
Contextual release and Activity reentry.
Manager-Provisioned lifecycle snapshots.
Scene-Provided persistent release diagnostics.
```

### 4.2 Implemented technically but not yet presented well in FIRSTGAME

```text
Manager-Provisioned lifecycle state projection.
Dynamic Capacity.
Multiple configured Slots.
Multiple Local Player Hosts.
Device/control-scheme hints on join request.
Participation projection policies.
Zero-participant policies.
Included/excluded Slot behavior.
Projection freeze for the active Activity occurrence.
Shared versus unique Actor selection policy.
```

### 4.3 Official product gaps

```text
Explicit public selection of an arbitrary ActorProfile.
Deferred Actor preparation while awaiting player choice.
Public Session Leave.
Session-Persistent Logical Player.
Actor replacement after preparation.
Disconnect/reconnect semantics.
Multiplayer Pause policy.
Multiple Camera outputs / split-screen.
Canonical Manager-Provisioned Recipe/Composer.
```

---

## 5. Program strategy

The work is divided into two tracks.

### Track A — Demonstrate existing capabilities

Use the current package without creating alternate framework logic in FIRSTGAME.

```text
FIRSTGAME creates understandable gameplay scenarios.
QA retains technical policy matrices and negative proof.
Package remains the source of truth for runtime behavior.
```

### Track B — Develop missing product capabilities

A package cut is created only when the demonstration reveals one of these conditions:

```text
A required public contract does not exist.
The same manual composition repeats across several demonstrations.
The framework exposes technical pieces but no usable product workflow.
The consumer would need global lookup, reflection or internal APIs.
A failure cannot be diagnosed through official evidence.
A valid gameplay flow requires a new runtime authority or lifecycle transaction.
```

---

# 6. Canonical demonstration organization

## 6.1 Naming rule

The old global sequence:

```text
PLAYER-D01
PLAYER-D02
PLAYER-D03
...
```

is retired for new work because it does not identify which FIRSTGAME demo owns the feature.

The canonical identifiers are now:

```text
DEMO02-MODEL-xx
  Player source and physical lifetime.

DEMO03-MULTI-xx
  Local multiplayer admission, capacity, devices and multiple Players.

DEMO04-SCENARIO-xx
  Playable participation and Actor-policy scenarios.

IF-PLAYER-Pxx
  Package/product cuts that create official framework capabilities.
```

Legacy identifiers remain only as migration references. New scenes, assets, documentation headings and commit messages should use the canonical identifiers.

## 6.2 Migration map

| Previous identifier | Canonical identifier | Destination |
|---|---|---|
| Existing M06 | `DEMO02-MODEL-01` | Scene-Provided Player — Route-Owned |
| `PLAYER-D01` / M07 | `DEMO02-MODEL-02` | Manager-Provisioned Player — Single Local Player |
| `PLAYER-D02` | `DEMO02-MODEL-03` | Scene-Provided Player — Activity-Owned |
| `PLAYER-D07` | `DEMO02-MODEL-04` | Source and lifetime comparison |
| `PLAYER-D03` | `DEMO03-MULTI-01` | Dynamic Capacity and Late Join |
| `PLAYER-D04` | `DEMO03-MULTI-02` | Two Local Players with Explicit Devices |
| `PLAYER-D05` | `DEMO04-SCENARIO-01` | Activity participation scenarios |
| `PLAYER-D06` | `DEMO04-SCENARIO-02` | Shared and unique Actor defaults |
| `PLAYER-P01..P07` | `IF-PLAYER-P01..P07` | Official package/product evolution |

## 6.3 Current status

| Demo | Purpose | Status |
|---|---|---|
| Demo 02 | Compare Player source and physical lifetime | Implementations closed; comparison documentation pending refresh |
| Demo 03 | Prove local multiplayer admission and multiple physical Players | Next active demo |
| Demo 04 | Present gameplay-oriented participation and Actor-policy scenarios | Planned after Demo 03 foundation |

## 6.4 Demo boundaries

```text
Demo 02
  Where does the Player come from?
  Which context owns the Host and Actor?
  What survives Route or Activity changes?

Demo 03
  How do multiple local Players enter?
  How do capacity, joining, Slots and devices interact?
  Can multiple Hosts and Actors operate independently?

Demo 04
  How are those Players used in real Activities?
  Which Players participate?
  Which readiness condition applies?
  Can Actor defaults be shared or unique?
```

Demo 04 must consume the multiplayer foundation proven in Demo 03. It must not recreate a parallel admission or provisioning implementation.

---

# 7. Demonstration roadmap

## Demo 02 — Player source and physical lifetime

### Purpose

Demonstrate the implemented Player origins and make their physical lifetime visible to a framework user.

### Closed baseline: DEMO02-MODEL-01 — Scene-Provided Player — Route-Owned

```text
Status: Closed
Legacy reference: M06

Route Primary Scene owns:
  Local Player Host
  Actor Mount
  Scene-provided Actor

Expected lifetime:
  physical Player remains while the Route remains active
  physical state can remain across Activity changes inside that Route
```

### DEMO02-MODEL-02 — Manager-Provisioned Player — Single Local Player

**Legacy reference:** `PLAYER-D01 / M07`  
**Status:** Closed

#### Objective

Turn the existing M07 into a clear reference demonstration rather than a collection of low-level commands.

#### Type

```text
UX/product
Documentation
Consumer integration
```

#### Scope

```text
Update Demo02 documentation.
Document the actual Manager-Provisioned flow.
Present lifecycle state in readable form.
Remove redundant actions from the primary UI.
Preserve low-level evidence under Advanced / Debug.
```

#### Out of scope

```text
Explicit Actor choice.
Session Leave.
Two-player gameplay.
Split-screen.
Package runtime changes.
```

#### Issue resolved

The current FIRSTGAME receiver exposes:

```text
Open Joining
Close Joining
Request Join
Select Default Actor
Request Join And Select Default Actor
```

In the current configuration, `RequestJoin` is followed by the framework lifecycle preparing the configured default Actor. Therefore:

```text
Select Default Actor
Request Join And Select Default Actor
```

are not valid primary user flows for this demonstration.

#### Expected product flow

```text
Enter Manager-Provisioned Activity
→ Open Joining
→ Join Player
→ Host appears
→ default Actor becomes selected/prepared/materialized
→ gameplay becomes available
→ close joining when appropriate
```

#### Product surface

Primary controls:

```text
Open Joining
Join Player
Close Joining
Restart Activity
Back To Menu
```

Advanced / Debug:

```text
Joining Open
Dynamic Capacity
Host Count
Slot State
Selected Actor
Logical Actor Prepared
Physical Actor Materialized
Gameplay Admitted
Activity Readiness
Last command result
Last framework diagnostic
```

#### Files expected to change

FIRSTGAME only:

```text
Assets/_Project/Demo02/README.md
Assets/_Project/Demo02/Scripts/ManagerProvisionedPlayer/Commands/
  ManagerProvisionedPlayerCommandReceiver.cs
Manager-Provisioned UI prefab/scene assets
Optional consumer lifecycle presentation component
```

#### Technical smoke

```text
Open Joining succeeds.
Join succeeds.
Exactly one Slot becomes Joined.
Exactly one Host exists.
Default Actor is selected.
Logical Actor becomes prepared.
Physical Actor becomes materialized.
Gameplay becomes admitted.
Restart does not duplicate Host or Actor.
```

#### Technical acceptance

```text
Compiles.
No hidden fallback.
No use of internal package APIs.
No first-found Player or Actor lookup.
No duplicate Host or Actor after restart.
Diagnostics identify Slot, Host, Actor and readiness state.
```

#### Product acceptance

```text
A user understands that Join creates the Player Host.
A user does not need to manually request the configured default Actor.
Primary UI does not expose redundant commands.
Advanced state remains inspectable.
README describes the exact current flow.
```

#### Architectural gain

```text
Separates product actions from low-level diagnostic actions.
Prevents FIRSTGAME from teaching an incorrect Actor selection sequence.
```

#### Usability gain

```text
The M07 becomes a usable reference instead of a technical command console.
```

#### Suggested commit

```text
DEMO02-MODEL-02 — Close Manager-Provisioned Player demonstration UX and documentation
```

---

### DEMO02-MODEL-03 — Scene-Provided Player — Activity-Owned

**Legacy reference:** `PLAYER-D02`  
**Status:** Closed

#### Objective

Demonstrate a Scene-Provided Player physically owned by an Activity content scene.

#### Type

```text
Real integration
Lifecycle demonstration
UX/product
```

#### Scope

```text
Create one Activity whose content scene owns the Player Host and Actor.
Admit the existing scene composition.
Release it on Activity exit.
Recreate and readmit it on reentry.
```

#### Out of scope

```text
Manager-Provisioned flow.
Camera.
Pause.
Multiple Players.
New package contracts.
```

#### Expected product flow

```text
Enter Route
→ enter Player Activity
→ Activity scene loads Host and Actor
→ framework admits existing Player
→ movement works
→ exit to Intermission
→ Activity scene unloads
→ Player admission releases
→ Slot becomes available
→ reenter Activity
→ new scene-owned Player is admitted
```

#### Product surface affected

```text
Scene-Provided Player Composer
Activity participation configuration
Scene-owned Player prefab
Persistent runtime diagnostics
```

#### Files expected to be created or changed

FIRSTGAME:

```text
New Activity asset
New Activity content scene
Optional new Route or Activity navigation entry
Scene-Provided Player prefab variant
README section for Activity-owned source
```

#### Technical smoke

```text
One active admission during the Activity.
One occupied Slot during the Activity.
Zero active admissions after exit.
Zero occupied Slots after exit.
Valid reentry.
No duplicate Actor or Host.
```

#### Technical acceptance

```text
Host and Actor are physically scene-owned.
Framework does not instantiate a duplicate Actor.
Release uses official Activity lifecycle.
No scene-name or hierarchy lookup is used as authority.
```

#### Product acceptance

```text
The user can distinguish Route-owned and Activity-owned Scene-Provided Players.
The scene composition is understandable in the Hierarchy and Inspector.
The release can be proven after the scene object disappears.
```

#### Architectural gain

```text
Proves that physical ownership and participation authority are separate.
Covers the second official Scene-Provided origin.
```

#### Usability gain

```text
Shows when a Player composition should live inside an Activity scene.
```

#### Suggested commit

```text
DEMO02-MODEL-03 — Add Activity-owned Scene-Provided Player demonstration
```

---

### DEMO02-MODEL-04 — Player Source and Lifetime Comparison

**Legacy reference:** `PLAYER-D07`  
**Status:** Documentation close pending

#### Objective

Create a final Demo02 comparison surface for all implemented Player origins.

#### Type

```text
Documentation
Navigation
Product overview
```

#### Scope

Present:

| Source | Host provider | Actor provider | Physical lifetime |
|---|---|---|---|
| Scene-Provided / Route-owned | Route scene | Route scene | Route |
| Scene-Provided / Activity-owned | Activity scene | Activity scene | Activity |
| Manager-Provisioned | `PlayerInputManager` | framework from ActorProfile | Host in Session; Actor contextual |
| Session-Persistent | unavailable | future contract | Session |

#### Out of scope

```text
Implementing Session-Persistent Player.
New runtime contracts.
```

#### Product acceptance

```text
The Demo02 menu identifies each model by source and lifetime.
Each entry links to a concise explanation.
The comparison does not rely on internal class names alone.
```

#### Suggested commit

```text
DEMO02-MODEL-04 — Consolidate Player source and lifetime comparison
```

---


---

## Demo 03 — Local multiplayer foundations

### Purpose

Prove admission control and multiple local Player composition before introducing gameplay scenario matrices.

### Product question

```text
How do multiple local Players enter, occupy ordered Slots,
receive technical Hosts and operate with explicit devices?
```

### DEMO03-MULTI-01 — Dynamic Capacity and Late Join

**Legacy reference:** `PLAYER-D03`  
**Status:** Next active cut

#### Objective

Demonstrate the distinction between configured Slot count, dynamic capacity and joining state.

#### Type

```text
Player participation demonstration
Consumer integration
```

#### Scope

```text
Configure two Slots.
Start with Dynamic Capacity = 1.
Join Player 1.
Prove Player 2 is blocked by capacity.
Increase capacity to 2.
Join Player 2.
Reduce capacity without evicting existing Players.
```

#### Out of scope

```text
Session Leave.
Disconnect/reconnect.
Split-screen.
Networked participation.
```

#### Expected product flow

```text
Configured Slots = 2
Dynamic Capacity = 1
Joining = Open

Join Player 1
→ success

Join Player 2
→ rejected because capacity is reached

Set Capacity = 2
→ Join Player 2 succeeds

Set Capacity = 1
→ existing Players remain
→ no additional join is allowed
```

#### Product surface affected

FIRSTGAME controls:

```text
Set Capacity 0
Set Capacity 1
Set Capacity 2
Open Joining
Close Joining
Join Player
```

Runtime presentation:

```text
Configured Slots
Dynamic Capacity
Joining Open
Joined Count
Available Count
Host Count
```

#### Files expected to change

FIRSTGAME:

```text
Second PlayerSlotProfile
GameApplication Slot configuration
Manager-Provisioned command enum/channel/receiver
UI controls and presentation
README
```

#### Technical smoke

```text
Capacity rejection is typed and diagnostic.
Increasing capacity permits the next ordered Slot.
Reducing capacity does not evict an existing Player.
Closing joining blocks reservation even when capacity is available.
```

#### Technical acceptance

```text
No direct Slot mutation from FIRSTGAME.
No fallback to another participation lane.
Allocation remains first available by configured order.
```

#### Product acceptance

```text
A user can explain:
  configured capacity
  dynamic capacity
  joining window
  current participation
```

#### Architectural gain

```text
Exercises the Session participation authority without introducing new runtime code.
```

#### Usability gain

```text
Turns abstract capacity rules into visible gameplay behavior.
```

#### Suggested commit

```text
DEMO03-MULTI-01 — Demonstrate dynamic capacity and late join
```

---

### DEMO03-MULTI-02 — Two Local Players with Explicit Devices

**Legacy reference:** `PLAYER-D04`  
**Status:** Planned after DEMO03-MULTI-01

#### Objective

Prove two Manager-Provisioned Players using explicit input-device intent.

#### Type

```text
Integration spike
Real gameplay proof
```

#### Scope

```text
Two configured Slots.
Two manual join requests.
Keyboard assigned to one request.
Gamepad assigned to another request.
Two Hosts, Actors and independent input bindings.
```

#### Out of scope

```text
Split-screen.
Multiplayer Pause.
Networking.
Reconnect.
Teams.
Role quotas.
```

#### Expected product flow

```text
Open Joining
→ Join Keyboard Player
→ PlayerSlot:player.1

Join Gamepad Player
→ PlayerSlot:player.2

Both Players move independently.
```

#### Product surface affected

```text
Join request UI
Device availability presentation
Two-player Actor spawn layout
Two-player movement test
Lifecycle diagnostics
```

#### Possible package findings

The spike may expose missing official support for:

```text
Spawn placement per Slot.
Device pairing diagnostics.
Control-scheme diagnostics.
Multiple simultaneous Actor materialization.
Player-specific Camera requests.
```

Any permanent solution for these findings must move to the package.

#### Technical smoke

```text
Two joined Slots.
Two technical Hosts.
Two selected Actors.
Two Logical Actors prepared.
Two physical Actors materialized.
Independent input.
No cross-control.
No duplication after restart.
```

#### Technical acceptance

```text
No playerIndex-to-Slot conversion.
No name-based Host resolution.
No first-found PlayerInput authority.
Device hints are explicit request inputs.
```

#### Product acceptance

```text
Each Player's device and Slot are understandable.
The demonstration remains playable without split-screen.
Failure to pair a device is explicit.
```

#### Architectural gain

```text
Proves that multiple local physical Hosts converge on the same Session participation authority.
```

#### Usability gain

```text
Reveals whether current Manager-Provisioned authoring is practical beyond one Player.
```

#### Suggested commit

```text
DEMO03-MULTI-02 — Add two-device local multiplayer integration proof
```

---


---

## Demo 04 — Participation and Actor-policy scenarios

### Purpose

Translate the multiplayer foundation into understandable game scenarios rather than another technical command console.

### Product question

```text
How does a game designer decide which Players participate,
what readiness is required and how Actor defaults are applied?
```

### DEMO04-SCENARIO-01 — Activity Participation Policies as Gameplay Scenarios

**Legacy reference:** `PLAYER-D05`  
**Status:** Planned

#### Objective

Translate the package/QA policy matrix into a small set of understandable game Activities.

#### Type

```text
UX/product
Integration
Policy demonstration
```

#### Scope

Create Activities representing:

| Activity | Projection | Requirement |
|---|---|---|
| Intermission | `NoSlots` | `None` |
| Lobby | `AllJoinedSlots` | `JoinedSlots` |
| Actor Staging | `AllJoinedSlots` | `LogicalActorsPrepared` |
| Arena | `AllJoinedSlots` | `GameplayReady` |
| Player 1 Tutorial | `ExplicitSlots` | selected Slot requirement |

#### Out of scope

```text
Reimplementing the full QA matrix in FIRSTGAME.
Synthetic failure injection as a primary user flow.
Networking.
Teams.
```

#### Expected product flow

```text
Join one or two Players
→ navigate between Activities
→ observe which Slots are projected
→ observe which readiness requirement blocks entry
→ observe which state remains Session-scoped
→ observe which Actor/gameplay evidence is released contextually
```

#### QA boundary

QA remains responsible for:

```text
Full requirement policy matrix.
Zero-participant policy matrix.
Active projection freeze.
Included/excluded failure scope.
Included/excluded release scope.
Stale occurrence handling.
```

FIRSTGAME proves only selected real-use scenarios.

#### Technical smoke

```text
NoSlots does not require a Player.
JoinedSlots waits for joined participation only.
LogicalActorsPrepared waits for Actor preparation.
GameplayReady waits for gameplay eligibility.
ExplicitSlots includes only the configured Slot.
```

#### Technical acceptance

```text
Activity configuration is the authority.
No runtime policy is duplicated by consumer scripts.
Included/excluded Slots match diagnostics.
```

#### Product acceptance

```text
The user can explain why each Activity is or is not ready.
The Inspector configuration corresponds to visible behavior.
```

#### Architectural gain

```text
Connects Game Flow, Player participation and readiness through official contracts.
```

#### Usability gain

```text
Makes Activity participation policy teachable without reading a technical matrix.
```

#### Suggested commit

```text
DEMO04-SCENARIO-01 — Demonstrate Activity participation scenarios
```

---

### DEMO04-SCENARIO-02 — Shared and Unique Actor Defaults

**Legacy reference:** `PLAYER-D06`  
**Status:** Planned

#### Objective

Demonstrate Actor duplicate-selection policies using authored defaults.

#### Type

```text
Policy demonstration
Consumer integration
```

#### Scope

Two variants:

```text
Shared
  Player 1 default → Explorer
  Player 2 default → Explorer

Unique
  Player 1 default → Explorer
  Player 2 default → Heavy
```

#### Out of scope

```text
Interactive character selection.
Actor replacement after preparation.
Role quotas.
Teams.
```

#### Expected product flow

```text
Start Shared configuration
→ join two Players
→ both use the same ActorProfile
→ physical Actor occurrences remain distinct

Start Unique configuration
→ join two Players
→ each Slot uses a distinct default ActorProfile
```

#### Technical smoke

```text
Policy is explicit in GameApplication configuration.
Selections are correlated to typed Slots.
Physical Actors remain separate occurrences.
Unique configuration produces no duplicate-profile violation.
```

#### Product acceptance

```text
The user understands:
  ActorProfile identity
  Actor occurrence identity
  duplicate-selection policy
```

#### Architectural gain

```text
Demonstrates policy without inventing a character-selection system.
```

#### Usability gain

```text
Clarifies the difference between sharing a Profile and sharing one physical Actor.
```

#### Suggested commit

```text
DEMO04-SCENARIO-02 — Demonstrate shared and unique Actor defaults
```

---


---

# 8. Package product evolution

## IF-PLAYER-P01 — Explicit Actor Selection

### Objective

Provide a real product workflow for choosing one ActorProfile from an authored set.

### Type

```text
Package authoring
Public runtime contract
QA
FIRSTGAME integration
```

### Required package surface

```text
Actor Selection Profile or Actor Catalog
  Authored list of allowed ActorProfiles.

Public requests
  Select Actor
  Replace Selection
  Clear Selection

Selection policy
  Prepare Default Automatically
  Wait For Explicit Selection

Typed diagnostics
  expected revision
  previous selection
  selected Actor
  duplicate-policy result
  preparation state
```

### Expected flow

```text
Join
→ Slot is Joined without prepared Actor
→ selection UI lists allowed Actors
→ user selects Actor
→ selection commits
→ Logical Actor prepares
→ physical Actor materializes
→ gameplay becomes ready
```

### Out of scope

```text
Actor replacement while already prepared.
Cosmetic loadouts.
Networking.
```

### QA smoke

```text
Valid explicit selection.
Invalid Actor outside catalog.
Revision mismatch.
Duplicate policy rejection.
Clear selection.
Deferred preparation.
No silent default fallback.
```

### Product acceptance

```text
A designer authors available Actors without scripting internal contracts.
A user can choose an Actor before gameplay.
The framework, not FIRSTGAME, owns the selection transaction.
```

### Suggested commit sequence

```text
IF-PLAYER-SELECTION-01 — Add authored Actor selection catalog and request contracts
IF-PLAYER-SELECTION-02 — Add deferred preparation policy and runtime integration
IF-PLAYER-SELECTION-03 — Add designer-first Actor selection diagnostics
QA-PLAYER-SELECTION — Add explicit selection regression matrix
D2-PLAYER-SELECTION — Demonstrate interactive Actor selection
```

---

## IF-PLAYER-P02 — Public Session Leave

### Objective

Allow an admitted Logical Player to leave the Session through an official lifecycle transaction.

### Required package transaction

```text
RequestLeave(PlayerSlotId)
→ revoke contextual gameplay
→ release Camera/input eligibility
→ release Actor materialization
→ release Logical Actor preparation
→ release Host evidence
→ release current Slot assignment
→ mark Slot Available
→ destroy framework-owned Host
→ apply explicit selection preservation policy
```

### Required decisions

```text
Preserve or clear selected Actor?
What happens during Activity transition?
Can leave be retried?
What result represents already left?
How are partial failures recovered?
```

### QA smoke

```text
Normal leave.
Repeated leave.
Leave during Activity.
Leave with Actor prepared.
Leave with gameplay admitted.
Leave failure and retry.
Rejoin same Slot.
No residual Host/Actor evidence.
```

### FIRSTGAME demonstration

```text
Join
→ play
→ Leave Session
→ Host disappears
→ Slot becomes Available
→ join again
```

### Suggested commit sequence

```text
IF-PLAYER-LEAVE-01 — Add Session Player leave contracts
IF-PLAYER-LEAVE-02 — Add reverse-order release transaction
IF-PLAYER-LEAVE-03 — Add authoring and diagnostics
QA-PLAYER-LEAVE — Add Session leave regression suite
D2-PLAYER-LEAVE — Demonstrate leave and rejoin
```

---

## IF-PLAYER-P03 — Session-Persistent Logical Player

### Objective

Implement the third accepted Logical Player source.

### Required package surface

```text
SessionPersistentPlayerRecipe or Profile
SessionPersistentPlayerComposer
Explicit admission operation
Physical ownership declaration
Host and Actor adoption rules
Activity projection integration
Materialization reconciliation
Session teardown
```

### Rejected shortcut

```text
Do not treat an arbitrary prefab under Persistent Content
as a Session-Persistent Logical Player.
```

### Expected flow

```text
Application starts
→ Session-Persistent Logical Player is admitted
→ Route changes preserve Logical Player and Slot
→ Activities prepare/adopt contextual Actor state
→ Session Leave or teardown ends the identity
```

### QA smoke

```text
Application startup admission.
Route transition.
Activity projection.
Actor contextual release.
No duplicate admission.
Session teardown.
```

### Suggested commit sequence

```text
IF-PLAYER-PERSISTENT-01 — Define Session-Persistent Player ADR and contracts
IF-PLAYER-PERSISTENT-02 — Add Composer and Session admission runtime
IF-PLAYER-PERSISTENT-03 — Add Actor/materialization reconciliation
QA-PLAYER-PERSISTENT — Add lifetime and transition regressions
D2-PLAYER-PERSISTENT — Demonstrate Session-Persistent Player
```

---

## IF-PLAYER-P04 — Actor Replacement After Preparation

### Objective

Provide a safe transaction for changing the selected Actor after an Actor is already prepared or materialized.

### Expected transaction

```text
Actor A prepared
→ request replacement
→ validate selection revision and policy
→ revoke gameplay eligibility
→ release Actor A materialization
→ release Actor A preparation
→ commit Actor B selection
→ prepare Actor B
→ materialize Actor B
→ restore gameplay
```

### QA smoke

```text
Successful replacement.
Replacement rejection.
Failure during A release.
Failure during B preparation.
Rollback/recovery.
Revision mismatch.
Duplicate-policy conflict.
```

### Suggested commit

```text
IF-PLAYER-ACTOR-REPLACE — Add prepared Actor replacement transaction
```

---

## IF-PLAYER-P05 — Disconnect and Reconnect

### Objective

Define behavior when a local input device or PlayerInput Host disconnects.

### Required decisions

```text
Does the Logical Player remain Joined?
Does the Slot enter Suspended state?
Does the Actor remain materialized?
Is gameplay gated?
Is there a timeout?
How is a replacement Host correlated?
Does reconnect preserve Actor selection?
```

### Required package work

```text
Disconnect policy asset/profile.
Suspended participation state.
Reconnect request/result.
Host replacement correlation.
Diagnostics and timeout/recovery policy.
```

### Suggested commit

```text
IF-PLAYER-RECONNECT — Add disconnect and reconnect lifecycle
```

---

## IF-PLAYER-P06 — Multiplayer Camera and Pause

### Objective

Extend current single-player Camera/Pause assumptions to multiple eligible Players.

### Required ADRs

```text
Camera output multiplicity.
Split-screen layout ownership.
Camera request arbitration per Slot.
Pause authorization policy.
Input gate behavior per Player.
Application-wide versus Player-owned Pause.
```

### Out of scope until ADR approval

```text
FIRSTGAME must not implement a parallel split-screen manager.
FIRSTGAME must not choose a multiplayer Pause policy locally.
```

---

## IF-PLAYER-P07 — Manager-Provisioned Player Recipe/Composer

### Objective

Convert repeated manual Manager-Provisioned assembly into an official authoring workflow.

### Timing

Do this after completing at least:

```text
DEMO02-MODEL-02
DEMO03-MULTI-01
DEMO03-MULTI-02
DEMO04-SCENARIO-01
```

These demonstrations will reveal the repeated configuration that is actually worth productizing.

### Proposed authoring model

```text
ManagerProvisionedPlayerRecipe
  Slot configuration reference
  duplicate selection policy
  initial dynamic capacity
  initial joining state
  Local Player Host prefab
  Actor preparation mode

ManagerProvisionedPlayerComposer
  Recipe
  PlayerInputManager
  Apply / Rebuild
  Validate
  Runtime Status
  Advanced / Debug
```

### Apply / Rebuild responsibilities

```text
Resolve or create explicit technical bindings.
Materialize the authored Host prefab on PlayerInputManager.
Validate manual join behavior.
Validate C# notifications.
Validate technical max Player count.
Validate Host prefab and empty Actor Mount.
Create explicit registration/binding components.
Remain idempotent and non-destructive.
```

### It must not

```text
Start gameplay.
Join a Player.
Mutate Profile assets at runtime.
Hide technical evidence without Advanced / Debug.
Use PlayerInputManager.instance.
```

### QA smoke

```text
Fresh apply.
Repeated apply.
Changed Recipe rebuild.
Divergent manager prefab.
Missing Host prefab.
Invalid Host.
Capacity mismatch.
No destructive replacement.
```

### Suggested commit sequence

```text
IF-PLAYER-COMPOSER-01 — Add Manager-Provisioned Player Recipe
IF-PLAYER-COMPOSER-02 — Add Composer Apply/Rebuild and validation
IF-PLAYER-COMPOSER-03 — Add runtime and Advanced/Debug presentation
QA-PLAYER-COMPOSER — Add idempotence and negative authoring regressions
```

---

# 9. Readiness and Loading caution

The Player program must not use `WaitVisible` or `WaitCovered` behavior as a canonical demonstration until the configured asset and observed presentation are verified together.

Expected semantics:

```text
WaitVisible
  Activity may become visible while gameplay remains gated.

WaitCovered + FadeWithLoading
  Loading remains covered.
  Required readiness participants contribute to progress.
  100% is terminal only after readiness succeeds.
```

If a correctly configured asset behaves differently from the accepted ADR:

```text
classify as package bug
→ reproduce in QA
→ correct package
→ validate FIRSTGAME again
```

Do not work around it by changing Player contracts or adding consumer-controlled Loading updates.

---

# 10. Recommended execution order

## Phase 1 — Close Demo 02

```text
1. DEMO02-MODEL-01 — Scene-Provided Player — Route-Owned. Closed.
2. DEMO02-MODEL-02 — Manager-Provisioned Player — Single Local Player. Closed.
3. DEMO02-MODEL-03 — Scene-Provided Player — Activity-Owned. Closed.
4. DEMO02-MODEL-04 — Refresh source/lifetime comparison in menu and README.
```

Demo 02 is not extended with multiplayer controls. Its responsibility ends at Player source and lifetime.

## Phase 2 — Build Demo 03 local multiplayer foundations

```text
5. DEMO03-MULTI-01 — Dynamic Capacity and Late Join.
6. DEMO03-MULTI-02 — Two Local Players with Explicit Devices.
```

Demo 03 owns:

```text
multiple configured Slots
joining window
capacity changes
late join
multiple Local Player Hosts
explicit device intent
multiple physical Actors
```

Camera split-screen and multiplayer Pause remain outside the demo until their package policy is defined.

## Phase 3 — Build Demo 04 scenario catalog

```text
7. DEMO04-SCENARIO-01 — Activity Participation Policies.
8. DEMO04-SCENARIO-02 — Shared and Unique Actor Defaults.
```

Demo 04 reuses the admission and multi-Player foundation from Demo 03. Its primary surface is gameplay scenarios, not low-level controls.

## Phase 4 — Consolidate findings

For each finding, classify:

| Finding | Destination |
|---|---|
| Presentation specific to one demo | FIRSTGAME |
| Reusable authoring friction | package Recipe/Composer |
| Missing public operation | package runtime/contracts |
| Policy matrix or negative case | QAFramework |
| Ambiguous architecture | ADR before implementation |
| Runtime defect | package fix, QA regression, FIRSTGAME validation |

## Phase 5 — Product evolution

Recommended priority:

```text
9. IF-PLAYER-P01 — Explicit Actor Selection.
10. IF-PLAYER-P07 — Manager-Provisioned Recipe/Composer.
11. IF-PLAYER-P02 — Session Leave.
12. IF-PLAYER-P03 — Session-Persistent Logical Player.
13. IF-PLAYER-P04 — Actor Replacement.
14. IF-PLAYER-P05 — Disconnect/Reconnect.
15. IF-PLAYER-P06 — Multiplayer Camera/Pause.
```

---

# 11. Global acceptance criteria

## Technical

```text
Compiles in Unity 6.5.
Uses canonical Immersive.Framework.* APIs.
No runtime dependency on Editor.
No silent fallback.
No static/global Player authority.
No service locator.
No PlayerInput.playerIndex as PlayerSlotId.
No first-found Host, Actor or PlayerInput.
Failures are typed and diagnostic.
QA covers technical and negative behavior.
FIRSTGAME does not call internal package APIs.
```

## Product

```text
The user can create the demonstration.
The user can understand the intended Player source.
The Inspector exposes principal intent.
Apply / Rebuild exists when repeated technical materialization justifies it.
Advanced / Debug exposes technical evidence.
Each demonstration has a short usage guide.
The gameplay flow proves the intended capability.
FIRSTGAME remains a consumer, not the official implementation.
```

---

# 12. Immediate next cut

```text
DEMO03-MULTI-01 — Dynamic Capacity and Late Join
```

This cut starts a new FIRSTGAME demo dedicated to local multiplayer foundations.

It must begin with:

```text
2 configured Player Slots
Dynamic Capacity = 1
Joining controls
ordered join requests
visible admission state
```

It must prove:

```text
Player 1 can join.
Player 2 is rejected while capacity is 1.
Increasing capacity to 2 permits Player 2.
Reducing capacity does not evict joined Players.
Closing joining blocks new reservations without removing existing Players.
```

The detailed authoring walkthrough starts only after the Demo 03 folder, menu, Route and reusable asset boundaries are agreed.
