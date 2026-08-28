# FG-ADR-002 — Player Sample Scope and Demonstration Architecture

Status: **ACCEPTED — CANONICAL PLAYER SAMPLE SCOPE / REVISION 4**  
Accepted on: **2026-08-22**  
Revision 2 updated on: **2026-08-24**  
Revision 3 updated on: **2026-08-26**  
Revision 4 updated on: **2026-08-28**  
Current document revision: **4**  
Canonical filename: **`FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md`**  
Scope: **Player sample coverage, Demonstration Application boundaries, implementation sequence, public-surface blockers, Player-specific sharing and product-facing terminology**  
Related strategy: **FG-ADR-001 — Immersive Framework Sample and Demonstration Strategy**  
Framework authority: **official IF-ADRs and current `com.immersive.framework` implementation remain authoritative for runtime architecture**

---

## 1. Purpose

This ADR defines the **Player-specific demonstration architecture** for the Immersive Framework sample program.

FG-ADR-001 remains authoritative for the general sample grammar:

```text
UPM Sample Group
Demonstration Application
Scenario
Sample HUB / Menu
authoring under Assets/_Sample/
final UPM promotion to Samples~/
public/product API consumption
no hidden sample runtime authority
```

FG-ADR-002 owns the Player-specific decisions that should no longer be duplicated or frozen inside FG-ADR-001:

```text
where Scene Player is demonstrated canonically
which Player Demonstration Applications are materialized / proven
which later Player demonstrations remain blocked by missing public contracts
when Player/Shared is allowed to exist as real shared ownership
how Player Host and Player Provisioning are named in product-facing surfaces
```

Canonical relationship:

```text
FG-ADR-001
  general sample-program strategy

FG-ADR-002
  Player sample scope and demonstration architecture
```

Revision 4 records the closure of Character Selection authoring/proving, the corrected `LeaveUnresolved` runtime behavior, the public `PlayerSessionObserver` composition used by the sample, ActorProfile-driven button presentation, and the Full Player `30/30` certification evidence.

---

## 2. Product-facing terminology

Status: **CANONICAL**

The Player model contains one common technical host and more than one way for that host to enter the Session.

```text
Local Player Host
  technical host for one local Player
  owns PlayerInput evidence, Actor Mount and Slot-admission evidence
  common to both provisioning paths

Scene Player
  product/sample/editor name for a Local Player Host already authored in a Scene
  uses HostProvisioning = SceneProvided

Player Provisioning
  product/sample/editor name for the Session/UIGlobal authority that can create Local Player Hosts
  uses HostProvisioning = ManagerProvisioned
```

Canonical distinction:

```text
SceneProvided
ManagerProvisioned
  = parallel Host Provisioning modes

Scene Player
Player Provisioning
  = different product compositions
  = not parallel Player Host types
```

`SceneProvided`, `ManagerProvisioned`, `PlayerHostProvisioningMode` and existing runtime/API names remain valid runtime terminology.

Product-facing surfaces should prefer short contextual grouping rather than presenting Scene Player and Player Provisioning as equivalent objects.

---

## 3. Current product evidence

The Player sample architecture tracks real implementation status rather than a fixed historical catalog.

Current evidence is:

```text
Getting Started / Minimal Game
  canonical Scene Player reference
  HostProvisioning = SceneProvided
  PROVEN

Player Provisioning
  HostProvisioning = ManagerProvisioned
  configured Default Actor resolution
  MATERIALIZED / PLAY MODE PROVEN 2026-08-24

Character Selection
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
  AUTHORING COMPLETE / PLAY MODE PROVEN 2026-08-28

Local Multiplayer
  PLANNED / BLOCKED
  public Slot/device/input ownership/observation contract still missing
```

Character Selection is no longer merely “unblocked”. Revision 4 records successful materialization and consumer Play Mode proof.

---

## 4. Decision

The canonical Player sample scope is:

```text
GETTING STARTED / MINIMAL GAME
  canonical Scene Player coverage
  HostProvisioning = SceneProvided
  PROVEN

PLAYER
  Player Provisioning
    HostProvisioning = ManagerProvisioned
    ResolveConfiguredDefault
    MATERIALIZED / PLAY MODE PROVEN

  Character Selection
    HostProvisioning = ManagerProvisioned
    ActorResolution = LeaveUnresolved
    AUTHORING COMPLETE / PLAY MODE PROVEN

  Local Multiplayer
    PLANNED / BLOCKED
    wait for public Slot/device/input ownership/observation contract
```

This is the current implementation sequence, not a permanent closed catalog.

A new Player Demonstration Application is added only when a materially distinct application/session contract requires one and the necessary public Framework surface is sufficient to demonstrate it without sample-owned architectural workarounds.

---

## 5. Scene Player coverage

Status: **CANONICAL / ALREADY PROVEN**

The canonical Scene Player demonstration is:

```text
Assets/_Sample/GettingStarted/MinimalGame/
```

It demonstrates:

```text
Scene-authored Local Player Host
  -> SceneProvided admission
  -> Session ownership after admission
  -> Activity participation / representation
  -> gameplay readiness
  -> gameplay input binding
  -> Mounted / First Person Camera
  -> minimal Move / Look navigation
```

Therefore a second dedicated Scene Player application under `Player/` is not required merely for symmetry.

A new Scene Player application would require evidence of a distinct consumer contract not already demonstrated by Getting Started / Minimal Game.

---

## 6. Player Provisioning

Status: **MATERIALIZED / PLAY MODE PROVEN 2026-08-24**

`Player Provisioning` is the product-facing name for the Manager-Provisioned application whose Session configuration resolves a configured Default Actor.

Core mental model:

```text
Player Provisioning authority
  -> uses authored Local Player Host Prefab
  -> creates Local Player Host when Join is explicitly requested
  -> Session Slot is joined
  -> configured Default Actor is selected/prepared
  -> physical Actor is materialized
  -> Activity participation/readiness completes
```

The provisioning setup is **not itself a Player Host**.

This application remains the canonical demonstration of Session-authorized Local Player Host creation with default Actor resolution.

---

## 7. Character Selection

Status: **AUTHORING COMPLETE / PLAY MODE PROVEN — 2026-08-28**

Character Selection is a distinct Player Demonstration Application because its Session creation-time Actor-resolution intent differs from Player Provisioning:

```text
Player Provisioning
  ActorResolution = ResolveConfiguredDefault

Character Selection
  ActorResolution = LeaveUnresolved
```

### 7.1 Public consumer model

The proven consumer flow is:

```text
PlayerSessionProfile
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
        ↓
Open Joining
  -> Join
  -> Slot Joined
  -> Actor unresolved
  -> Preparing / WaitingForActorSelection
        ↓
PlayerSessionObserver.OnPlayerJoined
  -> show game-owned Character Selection UI
        ↓
Farmer / Cow ActorProfile choices
        ↓
PlayerSessionSelectActorCommandTrigger.Invoke()
        ↓
Framework validates and commits selected Actor
        ↓
Actor preparation
  -> Manager-Provisioned materialization
  -> Activity participation / GameplayReady
        ↓
PlayerSessionObserver.OnActorSelected
  -> hide selection UI
```

Leave/Rejoin returns to:

```text
WaitingForJoin
  -> Join
  -> WaitingForActorSelection
  -> new explicit Actor choice
```

without an intermediate failed readiness state.

### 7.2 LeaveUnresolved semantics

`LeaveUnresolved` is an intentional pending state, not a failed/default-resolution state.

For a Joined Slot with no selected Actor:

```text
Joined
  -> selected Actor = none
  -> Preparing
  -> WaitingForActorSelection
```

The Framework must **not** invoke Default Actor resolution in this branch.

Revision 4 records the runtime correction that made the reconcile consult the immutable Session Actor-resolution policy before attempting `TrySelectDefaultActor`.

Canonical behavior:

```text
ResolveConfiguredDefault
  -> may resolve configured Default Actor

LeaveUnresolved
  -> never attempts default selection
  -> waits for explicit selection
```

Do not reinterpret `RejectedDefaultResolutionDisabled` as pseudo-success after issuing a forbidden default request. The request must not occur in the `LeaveUnresolved` branch.

### 7.3 PlayerSessionObserver presentation boundary

`PlayerSessionObserver` is read-only. It may project committed Session lifecycle into designer-facing presentation events without becoming a Player authority.

Character Selection uses:

```text
On Player Joined
  -> show Character Selection Controls

On Actor Selected
  -> hide Character Selection Controls

On Player Left
  -> hide Character Selection Controls
```

`On Actor Cleared` is intentionally not used to show the panel because Actor clear occurs during Leave before terminal Player Left.

The Observer remains outside the panel it activates/deactivates so its scoped observation lifetime is not disabled with the UI.

### 7.4 ActorProfile-driven button presentation

Each selection button keeps `PlayerSessionSelectActorCommandTrigger` as the authority for which `ActorProfile` will be selected.

The sample-owned `CharacterSelectionActorButtonPresenter` reads that same command's `ActorProfile` and projects presentation only:

```text
PlayerSessionSelectActorCommandTrigger.ActorProfile
  ├── DisplayName -> button label
  └── Icon        -> button image
```

This avoids authoring the same ActorProfile twice and keeps command semantics separate from presentation.

The presenter does not:

```text
select Actors
mutate Session state
own Player lifecycle
perform Player discovery
register another ActorProfile authority
silently wire the Button command
```

### 7.5 Sample ownership boundary

The game/sample owns:

```text
which ActorProfile choices are presented
character labels/icons/layout
UI visibility wiring
which explicit selection command the user invokes
```

The Framework owns:

```text
Joined Slot validity
Actor-resolution policy
selection revision
selection commit
Session duplicate-selection policy
Actor preparation barrier
physical Actor materialization
Activity participation/admission/readiness
```

The sample must not use:

```text
private/internal runtime access
reflection
sample-specific Session discovery
direct mutation of internal Session state
parallel Actor-selection authority
hidden fallback Actor
sample-owned Actor preparation/materialization
```

### 7.6 Initial selection, not hot swap

Character Selection demonstrates **initial explicit Actor selection** after Join.

Do not add Replace/Clear UI merely because those public APIs exist.

`Replace Actor Selection` is not a physical hot-swap command. Once the Logical Actor is prepared, the canonical preparation barrier prevents logical selection changes that would imply replacing the prepared physical Actor.

### 7.7 Character Selection validation evidence

Consumer Play Mode proved:

```text
Open Joining -> Succeeded
Join -> SucceededJoined
Joined + unresolved Actor -> WaitingForActorSelection
gate held while selection is pending
Farmer -> SucceededSelected -> Prepared -> Materialized -> GameplayReady
Leave -> WaitingForJoin
Rejoin -> WaitingForActorSelection
Cow -> SucceededSelected -> Prepared -> Materialized -> GameplayReady
```

The corresponding Framework Full Player aggregate passed:

```text
historicalFullPlayer = 25/25
serialization = PASS
session = PASS
sceneProvided = PASS
managerProvisioned = PASS
leaveUnresolved = PASS
actor = PASS
publicSurface = PASS
sessionChangeObservation = PASS
designerEventProjection = PASS
leave = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

This closes Character Selection authoring/proving. Final UPM promotion/import proof remains a later Player sample-group release gate.

---

## 8. Local Multiplayer

Status: **NEXT PLANNED PLAYER DEMONSTRATION / PLANNED / BLOCKED**

Local Multiplayer requires more than multiple Player objects in one scene.

The intended sample must communicate a canonical relationship among:

```text
local participant
device
Slot
input ownership/routing
Player admission
Actor selection when applicable
leave / rejoin when applicable
```

The sample remains blocked until the Framework exposes a sufficient public **Slot/device/input ownership and observation contract** for a normal consumer.

The current ordinary Join command does not provide exact-Slot public Join and does not expose a complete durable Slot-to-device/InputUser/control-scheme observation contract.

The required contract must not depend on the sample inventing authority for:

```text
which device owns which Slot
which input stream belongs to which local Player
how a joining participant is associated with a Slot
how that association is observed
how ownership is released/reused
```

### 8.1 Exit criterion

Local Multiplayer may move to implementation when the public product surface supports a canonical consumer flow such as:

```text
device/participant intent
  -> exact/deterministic Slot association
  -> Player Join/admission
  -> correct input ownership
  -> observable Slot/device/control-scheme state
  -> release/reuse when required
```

without parallel sample-owned Slot/device/input authority.

Split-screen is not implied by this ADR.

---

## 9. Demonstration Application vs Scenario

Create another Demonstration Application only when the initial application/session intent is materially incompatible with an existing one.

Strong signals include:

```text
Host Provisioning mode
Supported Slot universe
initial Joining intent
initial Actor-resolution intent
application-level duplicate-selection policy
other application/session creation-time authority
```

Character Selection remains a separate Demonstration Application because `LeaveUnresolved` is creation-time Session intent and materially differs from Player Provisioning's `ResolveConfiguredDefault` behavior.

Use a Scenario when the same application/session archetype remains coherent and only runtime behavior changes.

Examples that remain Scenarios by default:

```text
Activity Participation
Activity Representation
Physical Player Lifetime
Initial Placement
Input / Pause
Leave / Rejoin
```

This prevents Player samples from becoming a combinatorial matrix.

---

## 10. Public-surface gate

A sample is executable documentation of the product surface.

Therefore:

> **A missing public consumer contract blocks the sample; it does not authorize the sample to implement a substitute framework.**

Player sample code may provide game-owned presentation and interaction such as:

```text
join prompts
character-selection UI
ActorProfile button presenters
simple HUD
minimal locomotion
sample navigation
```

It must not provide hidden Framework responsibilities such as:

```text
internal Player discovery
private Actor mutation
parallel Slot registry
parallel device ownership
parallel input routing authority
reflection-based binding
silent fallback that makes invalid configuration appear valid
```

Character Selection satisfies this gate and is proven. Local Multiplayer remains blocked by its own missing public contract.

---

## 11. Player/Shared

Status: **CONDITIONAL / CREATED BY REUSE, NOT BY PLAN**

`Player/Shared` is not a required Player architecture layer.

Canonical rule:

```text
used by one Player Demonstration Application
  -> keep local

concretely reused by two or more Player Demonstration Applications
  -> consider promotion of reusable presentation/content to Player/Shared

no concrete cross-application reuse
  -> do not create/promote Player/Shared content
```

This may apply to:

```text
character visuals
portraits/icons
UI visual pieces
input assets
presentation prefabs
sample-only visual helpers
```

It does **not** justify moving application authority upward.

Keep authoritative application/session configuration local, including as applicable:

```text
GameApplicationAsset
PlayerSessionProfile
RouteAsset
ActivityAsset
application-specific policies/profiles
application-specific Persistent Content composition
application-specific bindings
```

Current authoring may reuse compatible content between Player demonstrations. Final UPM organization may promote genuinely reusable presentation/content when that improves clarity, but reuse never creates shared Session authority.

---

## 12. Current implementation order

```text
0. Scene Player
   HostProvisioning = SceneProvided
   canonical Getting Started / Minimal Game
   PROVEN

1. Player Provisioning
   HostProvisioning = ManagerProvisioned
   ResolveConfiguredDefault
   MATERIALIZED / PLAY MODE PROVEN

2. Character Selection
   HostProvisioning = ManagerProvisioned
   ActorResolution = LeaveUnresolved
   AUTHORING COMPLETE / PLAY MODE PROVEN

3. Local Multiplayer
   NEXT PLANNED PLAYER DEMONSTRATION
   PLANNED / BLOCKED
   wait for public Slot/device/input ownership/observation contract
```

This order may change only from concrete implementation/product evidence.

It must not be interpreted as permission to implement a blocked demonstration with sample-owned replacement infrastructure.

---

## 13. Consumer navigation

The Player group README is the entry surface for choosing Player demonstrations.

When more than one runnable Player Demonstration Application exists:

```text
Player README
  -> identifies each application's purpose and status

consumer selects the intended GameApplication
  -> Set Active through the official Framework surface

Play
  -> optional application-local HUB may select compatible Scenarios
```

Do not create a global runtime Player HUB that silently switches Active GameApplications.

---

## 14. Relationship to Getting Started, QAFramework and FIRSTGAME

Getting Started remains intentionally minimal. Its Scene Player is the **canonical Scene Player consumer reference** and proves `SceneProvided` Host Provisioning.

Proof surfaces remain distinct:

```text
Samples / Getting Started
  representative canonical consumer usage

QAFramework
  exhaustive technical contracts
  negative cases
  regression combinations

FIRSTGAME / authoring workspace
  real integration, ergonomics and consumer composition proof
```

The Full Player `30/30` result is technical certification for the current Player runtime surface. Character Selection's own Play Mode run proves that a normal consumer composition can use that surface without private/internal access.

Do not duplicate every Player permutation in Samples.

Do not use FIRSTGAME as justification for bypassing a missing public surface.

---

## 15. Superseded Player assumptions

FG-ADR-002 supersedes the following earlier assumptions:

```text
Player requires a fixed initial Demonstration Application catalog

SceneProvided requires a second dedicated application under Player

SceneProvided and ManagerProvisioned should be exposed as peer object/composition names

Character Selection is blocked by missing public arbitrary Actor-selection surface
  -> superseded 2026-08-26

Character Selection is only “next/unblocked” and not yet proven
  -> superseded 2026-08-28

LeaveUnresolved may attempt Default Actor resolution during Activity reconcile
  -> superseded; LeaveUnresolved waits for explicit Actor selection

Local Multiplayer is ready merely because multiple Slots are conceptually supported

Player/Shared is a pre-created required group-level structure
```

The remaining general FG-ADR-001 rules continue to apply.

---

## 16. Normative summary

```text
Local Player Host
  common technical Player Host
  may already exist in the Scene or be created by provisioning

Scene Player
  product-facing SceneProvided composition
  canonical reference = Getting Started / Minimal Game

Player Provisioning
  product-facing ManagerProvisioned authority/application
  default Actor resolution case proven

Character Selection
  separate ManagerProvisioned application
  ActorResolution = LeaveUnresolved
  Joined + no Actor = WaitingForActorSelection
  PlayerSessionObserver controls presentation only
  PlayerSessionSelectActorCommandTrigger owns explicit choice request
  ActorProfile DisplayName/Icon may drive sample presentation
  Framework owns selection/preparation/materialization/readiness
  AUTHORING COMPLETE / PLAY MODE PROVEN

Local Multiplayer
  planned but blocked until public Slot/device/input ownership/observation is sufficient

Player/Shared
  created by concrete reuse only
  presentation/content may be shared
  application/session authority remains local

Public-surface rule
  missing product contract blocks the sample
  never replace it with sample-owned framework authority
```
