# FG-ADR-002 — Player Sample Scope and Demonstration Architecture

Status: **ACCEPTED — CANONICAL PLAYER SAMPLE SCOPE / REVISION 3**  
Accepted on: **2026-08-22**  
Revision 2 updated on: **2026-08-24**  
Revision 3 updated on: **2026-08-26**  
Current document revision: **3**  
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
where the Scene Player path is demonstrated canonically
which Player Demonstration Applications are materialized / proven
which Player Demonstration Application is next
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

### 1.1 Product-facing terminology

Status: **CANONICAL — REVISION 3**

The Player model contains one common technical host and more than one way for that host to enter the Session.

```text
Local Player Host
  the technical host for one local Player
  owns PlayerInput evidence, Actor Mount and Slot-admission evidence
  is common to both provisioning paths

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

`SceneProvided`, `ManagerProvisioned`, `PlayerHostProvisioningMode` and existing runtime/API type names remain valid runtime terminology. Revision 3 does **not** rename those contracts merely to change presentation vocabulary.

Product-facing surfaces should prefer short contextual grouping over repeating runtime-mode names as if they described equivalent objects.

Preferred grouping:

```text
Player
├── Scene
│   └── Local Player
└── Provisioning
    └── Setup / Authority
```

---

## 2. Current product evidence

The Player sample architecture must track real implementation status rather than preserve an old fixed catalog.

Current evidence is:

```text
Getting Started / Minimal Game
  canonical Scene Player reference
  SceneProvided
  proven

Player Provisioning
  ManagerProvisioned
  materialized
  Play Mode proven 2026-08-24

Character Selection
  public arbitrary Actor-selection blocker closed 2026-08-26
  next Player implementation cut

Local Multiplayer
  still blocked by public Slot/device/input ownership/observation contract
```

The public Actor-selection closure changes the Character Selection status from the older Revision 2 `PLANNED / BLOCKED` state. It does not change the Local Multiplayer blocker.

---

## 3. Decision

The canonical Player sample scope is now:

```text
GETTING STARTED / MINIMAL GAME
  canonical Scene Player coverage
  HostProvisioning = SceneProvided
  PROVEN

PLAYER
  Provisioning
    HostProvisioning = ManagerProvisioned
    MATERIALIZED / PLAY MODE PROVEN

  Character Selection
    NEXT / PUBLIC SURFACE UNBLOCKED
    use public explicit Actor-selection commands

  Local Multiplayer
    PLANNED / BLOCKED
    wait for public Slot/device/input ownership/observation contract
```

This is the current implementation sequence, not a permanent closed catalog.

A new Player Demonstration Application is added only when a materially distinct application/session contract requires one and the necessary public Framework surface is sufficient to demonstrate it without sample-owned architectural workarounds.

---

## 4. Scene Player coverage

Status: **CANONICAL / ALREADY PROVEN**

The canonical Scene Player demonstration is:

```text
Assets/_Sample/GettingStarted/MinimalGame/
```

It already demonstrates the coherent consumer path:

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

Therefore:

```text
a dedicated Scene Player application under Player
  is not required
```

Do not create a duplicate Scene Player application merely to make the Player folder appear symmetrical with Player Provisioning.

A dedicated Player Scene application becomes justified only if future implementation evidence reveals a **distinct consumer contract** that cannot be clearly demonstrated by Getting Started / Minimal Game.

The burden of proof is the new contract, not the old scaffold.

---

## 5. Player Provisioning

Status: **MATERIALIZED / PLAY MODE PROVEN 2026-08-24**

`Player Provisioning` is the product-facing name for the Manager-Provisioned Player application. Its underlying Session configuration uses:

```text
HostProvisioning = ManagerProvisioned
```

The application is distinct because it introduces Session-authorized authority that can create Local Player Hosts from the authored Host prefab.

Core mental model:

```text
Player Provisioning authority
  -> uses the authored Local Player Host Prefab
  -> creates a Local Player Host when join is explicitly requested
  -> admission succeeds
  -> Session owns the admitted physical Player
```

The provisioning setup is **not itself a Player Host**.

The current sample proves Join, Local Player Host creation, default Actor selection/preparation, physical materialization, Activity participation/readiness, gameplay input, Player Camera, Leave and Rejoin through the public product surface.

Compatible additional Player behaviors remain Scenarios by default when they do not require a different initial application/session contract.

---

## 6. Character Selection

Status: **NEXT PLAYER CUT / PUBLIC SURFACE UNBLOCKED — 2026-08-26**

Character Selection remains a distinct valid Player Demonstration Application and its previous public-surface blocker is now closed.

The Framework now exposes the required explicit public Actor-selection surface:

```text
PlayerSessionObserver

PlayerSessionSelectActorCommandTrigger
PlayerSessionDefaultActorSelectionCommandTrigger
PlayerSessionReplaceActorSelectionCommandTrigger
PlayerSessionClearActorSelectionCommandTrigger

PlayerActorSelectionResult
```

The intended consumer model is game-owned selection UI using official public Framework surfaces:

```text
PlayerSessionProfile
  ActorResolution = LeaveUnresolved
        ↓
Player joins
  -> Slot Joined
  -> Actor unresolved
        ↓
game-owned UI presents eligible ActorProfile choices
        ↓
consumer invokes PlayerSessionSelectActorCommandTrigger
        ↓
Framework validates and commits selected Actor
        ↓
existing Actor preparation / Manager-Provisioned materialization
        ↓
Activity participation / GameplayReady
```

### 6.1 Sample ownership boundary

The game/sample owns:

```text
which ActorProfile choices are presented
character labels / portraits / visual presentation
which explicit selection command the user invokes
```

The Framework owns:

```text
Joined Slot validity
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

### 6.2 Initial selection, not hot swap

The Character Selection sample demonstrates **initial explicit Actor selection** after Join.

Do not add Replace/Clear UI merely because the public API exists. Those operations are useful public lifecycle contracts but are not required to teach the initial Character Selection flow.

`Replace Actor Selection` is not a physical hot-swap command. Once the Logical Actor is prepared, the canonical preparation barrier rejects logical selection changes that would imply physical replacement.

### 6.3 Default Actor behavior

Normal Character Selection flow must not invoke Default Actor selection.

The application uses:

```text
ActorResolution = LeaveUnresolved
```

so `PlayerSessionDefaultActorSelectionCommandTrigger` correctly rejects instead of selecting a hidden fallback.

### 6.4 Exit criterion — SATISFIED

Revision 2 required that a normal game-owned consumer be able to:

```text
observe relevant Player/Actor state
identify/select an eligible Actor through supported public data
request the Actor selection through a supported public command
observe the confirmed result
```

The 2026-08-26 Framework public-surface closure and integrated Player QA satisfy this gate.

Character Selection can therefore proceed to materialization and consumer Play Mode proof.

---

## 7. Local Multiplayer

Status: **PLANNED / BLOCKED**

Local Multiplayer remains a valid Player sample goal, but it requires more than multiple Player objects in one scene.

The intended sample must be able to communicate a canonical relationship among:

```text
local participant
Slot
device
input ownership/routing
Player admission
Actor selection when applicable
leave / rejoin when applicable
```

The sample remains blocked until the Framework exposes a sufficient public **Slot/device/input ownership and observation contract** for a normal consumer.

The current public Join command is not an exact-Slot Join surface and does not expose a complete durable Slot-to-device/InputUser/control-scheme observation contract.

The required contract must not depend on the sample inventing its own authority for:

```text
which device owns which Slot
which input stream belongs to which local Player
how a joining participant is associated with a Slot
how that association is observed
how ownership is released/reused
```

### 7.1 Exit criterion

Local Multiplayer may move from **PLANNED / BLOCKED** to implementation when the public product surface is sufficient to author and observe a canonical local flow such as:

```text
device/participant intent
  -> Slot association
  -> Player join/admission
  -> correct input ownership
  -> observable Slot occupancy
  -> release/reuse when required
```

without parallel sample-owned Slot/device/input authority.

Split-screen is not implied by this ADR.

---

## 8. Demonstration Application vs Scenario

The Player group follows the general FG-ADR-001 split rule.

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

Character Selection justifies a separate Demonstration Application because its initial Actor-resolution intent deliberately differs from the default-resolving Player Provisioning sample:

```text
Player Provisioning
  ResolveConfiguredDefault

Character Selection
  LeaveUnresolved
```

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

## 9. Public-surface gate

A sample is executable documentation of the product surface.

Therefore:

> **A missing public consumer contract blocks the sample; it does not authorize the sample to implement a substitute framework.**

Player sample code may provide game-owned presentation and interaction such as:

```text
join prompts
character-selection UI
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

The Character Selection blocker is now closed because the Framework surface exists. The same rule continues to block Local Multiplayer until its own missing public contract is delivered.

---

## 10. Player/Shared

Status: **CONDITIONAL / CREATED BY REUSE, NOT BY PLAN**

`Player/Shared` is not part of the required Player architecture.

Canonical rule:

```text
used by one Player Demonstration Application
  -> keep local

used concretely by two or more Player Demonstration Applications
  -> consider promotion to Player/Shared

no concrete cross-application reuse
  -> do not create/promote Player/Shared content
```

This applies to presentation/content such as:

```text
character visuals
portraits
UI visual pieces
input assets
presentation prefabs
sample-only visual helpers
```

when real reuse exists.

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

Pedagogical ownership clarity is more important than deduplication.

An existing empty or placeholder Shared scaffold does not establish architectural authority and should not be cited as evidence that Player requires a Shared layer.

---

## 11. Current implementation order

The Player sequence is now:

```text
0. Scene Player
   HostProvisioning = SceneProvided
   canonical Getting Started / Minimal Game
   PROVEN

1. Player Provisioning
   HostProvisioning = ManagerProvisioned
   MATERIALIZED / PLAY MODE PROVEN

2. Character Selection
   ActorResolution = LeaveUnresolved
   NEXT / PUBLIC SURFACE UNBLOCKED

3. Local Multiplayer
   PLANNED / BLOCKED
   wait for public Slot/device/input ownership/observation contract
```

This order may change only from concrete implementation/product evidence.

It must not be interpreted as a promise that every planned demonstration will be implemented before its public contract exists.

---

## 12. Consumer navigation

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

## 13. Relationship to Getting Started

Getting Started remains intentionally minimal.

Its Scene Player is not incidental sample plumbing; it is the **canonical Scene Player consumer reference** and proves the `SceneProvided` Host Provisioning mode.

This does not turn Getting Started into a full Player tutorial.

Its lesson remains:

```text
minimum coherent Framework application
+
navigation
```

The Player group begins where a distinct Player contract requires more than that baseline.

---

## 14. Relationship to QAFramework and FIRSTGAME

```text
Getting Started / Samples
  representative canonical consumer usage

QAFramework
  exhaustive technical contracts
  negative cases
  regression combinations

FIRSTGAME
  real-game integration and ergonomics
```

The 2026-08-26 Full Player aggregate completed `27/27` mandatory contracts with Actor Lifecycle and Public Surface both PASS. This is technical certification evidence for the public surface, not a substitute for the Character Selection sample's own consumer Play Mode proof.

Do not duplicate every Player permutation in Samples.

Do not use FIRSTGAME as justification for bypassing a missing public surface in Samples.

---

## 15. Superseded Player assumptions

FG-ADR-002 supersedes the following earlier assumptions:

```text
Player requires a fixed initial Demonstration Application catalog

SceneProvided requires a second dedicated application under Player

SceneProvided and ManagerProvisioned should be exposed as peer object/composition names

Character Selection is blocked by missing public arbitrary Actor-selection surface
  -> superseded 2026-08-26; public surface now delivered

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
  product/sample/editor name for the Scene-authored Local Player Host path
  runtime HostProvisioning = SceneProvided
  canonical coverage = Getting Started / Minimal Game
  PROVEN

Player Provisioning
  product/sample/editor name for Session-authorized Host creation authority
  runtime HostProvisioning = ManagerProvisioned
  provisioning setup is not itself a Player Host
  MATERIALIZED / PLAY MODE PROVEN

SceneProvided / ManagerProvisioned
  remain valid runtime provisioning-mode terminology
  are parallel modes, not peer object types

Character Selection
  NEXT / PUBLIC SURFACE UNBLOCKED
  PlayerSessionProfile.ActorResolution = LeaveUnresolved
  game-owned Actor catalog/UI
  public explicit Select Actor command
  no sample-owned Session or Actor preparation authority
  no physical hot swap

Local Multiplayer
  PLANNED / BLOCKED
  requires sufficient public Slot/device/input ownership/observation contract

Player/Shared
  not preallocated
  introduce/promote only after concrete reuse

Player application count
  evidence-driven
  not frozen to a predetermined count

Player capabilities
  Scenarios by default unless initial Session intent is materially incompatible

Missing public surface
  blocks the sample
  never justifies parallel sample-owned Framework authority
```

---

## 17. Closure

This ADR is the canonical Player sample-scope and product-facing terminology authority.

Revision 3 records that Player Provisioning is Play Mode proven and that the public arbitrary Actor-selection blocker has been closed by the Framework's explicit scoped command surface. Character Selection is therefore the next Player implementation cut. Local Multiplayer remains blocked by a different public contract and must not be pulled into the Character Selection work.

FG-ADR-001 continues to own the general sample-program architecture and distribution strategy. Any future change to the Player demonstration catalog, sequencing, blocker status, Player-specific sharing rule or product-facing naming should be reconciled here first and reflected into operational READMEs without reintroducing a competing Player catalog or false Player Host symmetry in FG-ADR-001.
