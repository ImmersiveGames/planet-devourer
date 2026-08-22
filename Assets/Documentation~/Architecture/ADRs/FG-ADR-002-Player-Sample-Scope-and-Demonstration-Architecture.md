# FG-ADR-002 — Player Sample Scope and Demonstration Architecture

Status: **ACCEPTED — CANONICAL PLAYER SAMPLE SCOPE / REVISION 1**  
Accepted on: **2026-08-22**  
Current document revision: **1**  
Canonical filename: **`FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md`**  
Scope: **Player sample coverage, Demonstration Application boundaries, implementation sequence, public-surface blockers and Player-specific sharing**  
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
where Scene-Provided is demonstrated canonically
which Player Demonstration Application is next
which later Player demonstrations are planned
which public product surfaces currently block those demonstrations
when Player/Shared is allowed to exist as real shared ownership
```

Canonical relationship:

```text
FG-ADR-001
  general sample-program strategy

FG-ADR-002
  Player sample scope and demonstration architecture
```

---

## 2. Problem

The earlier Player sample baseline treated a fixed application catalog as if all of its entries were already frozen and equally ready for implementation.

Implementation evidence changed that conclusion.

Getting Started / Minimal Game now provides a complete executable Scene-Provided reference. Creating another dedicated Scene-Provided application under Player would duplicate a contract already demonstrated canonically.

At the same time, later Player demonstrations do not all have the same implementation readiness:

```text
Manager-Provisioned
  can be the next distinct Player application

Character Selection
  depends on a sufficient public arbitrary Actor-selection surface

Local Multiplayer
  depends on a sufficient public Slot/device/input contract
```

The sample architecture must therefore distinguish:

```text
canonical coverage that already exists
next materializable application
planned but product-blocked demonstrations
future applications justified only by new evidence
```

It must not preserve empty structural promises merely because an older scaffold contained folders for them.

---

## 3. Decision

The canonical Player sample scope is:

```text
GETTING STARTED / MINIMAL GAME
  canonical Scene-Provided coverage

PLAYER
  Manager-Provisioned
    next Player Demonstration Application

  Character Selection
    planned
    blocked by public arbitrary Actor-selection surface

  Local Multiplayer
    planned
    blocked by public Slot/device/input contract
```

This is the current implementation sequence, not a permanent closed catalog.

A new Player Demonstration Application is added only when a materially distinct application/session contract requires one and the necessary public Framework surface is sufficient to demonstrate it without sample-owned architectural workarounds.

---

## 4. Scene-Provided coverage

Status: **CANONICAL / ALREADY PROVEN**

The canonical Scene-Provided demonstration is:

```text
Assets/_Sample/GettingStarted/MinimalGame/
```

It already demonstrates the coherent consumer path:

```text
Scene-authored Player candidate
  -> Scene-Provided admission
  -> Session ownership after admission
  -> Activity participation / representation
  -> gameplay readiness
  -> gameplay input binding
  -> Mounted / First Person Camera
  -> minimal Move / Look navigation
```

Therefore:

```text
a dedicated Scene-Provided application under Player
  is not required
```

Do not create a duplicate Scene-Provided Player application merely to make the Player folder appear symmetrical.

A dedicated Player Scene-Provided application becomes justified only if future implementation evidence reveals a **distinct consumer contract** that cannot be clearly demonstrated by Getting Started / Minimal Game.

The burden of proof is the new contract, not the old scaffold.

---

## 5. Manager-Provisioned

Status: **NEXT PLAYER DEMONSTRATION APPLICATION**

Manager-Provisioned is the next Player application to materialize because it introduces a genuinely distinct provisioning origin and authoring path.

Core mental model:

```text
Session-authorized provisioning
  -> Player candidate is created/provided
  -> admission succeeds
  -> Session owns the admitted physical Player
```

The sample should demonstrate the smallest coherent Manager-Provisioned consumer composition.

It should not attempt to absorb every Player capability at once.

Compatible behaviors remain Scenarios by default, for example when useful:

```text
joining
Activity participation
physical lifetime
initial placement
input / Pause
leave / rejoin
```

Only the behaviors necessary to explain the Manager-Provisioned contract should be included in the first cut.

---

## 6. Character Selection

Status: **PLANNED / BLOCKED**

Character Selection remains a valid Player sample goal.

The intended consumer model is game-owned selection UI using official public Framework surfaces:

```text
Player joins
  -> no final Actor selected yet
  -> game-owned UI presents eligible Actors
  -> consumer requests selection through public API
  -> Framework confirms effective Actor
  -> gameplay continues
```

The sample is currently blocked until the Framework exposes a sufficient **public arbitrary Actor-selection surface** for this flow.

The required product surface must allow a normal game-owned consumer to request/select the intended Actor without:

```text
private/internal runtime access
reflection
sample-specific service discovery
direct mutation of internal Session state
parallel Actor-selection authority
hidden fallback
```

### 6.1 Exit criterion

Character Selection may move from **PLANNED / BLOCKED** to implementation when a consumer can:

```text
observe the relevant Player/Actor state
identify/select an eligible Actor through supported public data
request the Actor change/selection through a supported public command
observe the confirmed result
```

using only official product surfaces.

The sample must not be used to conceal the absence of that contract.

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

The sample is currently blocked until the Framework exposes a sufficient public **Slot/device/input contract** for a normal consumer.

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
Host Provisioning origin
Supported Slot universe
initial Joining intent
initial Actor-resolution intent
application-level duplicate-selection policy
other application/session creation-time authority
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

When a sample cannot be built cleanly through public surfaces, that is product evidence for `com.immersive.framework`.

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

The Player sequence is:

```text
0. Scene-Provided
   already covered canonically by Getting Started / Minimal Game
   do not duplicate

1. Manager-Provisioned
   next Player Demonstration Application

2. Character Selection
   planned / blocked
   wait for public arbitrary Actor-selection surface

3. Local Multiplayer
   planned / blocked
   wait for public Slot/device/input contract
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

Its Scene-Provided Player is not incidental sample plumbing anymore; it is the **canonical Scene-Provided consumer reference**.

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

Do not duplicate every Player permutation in Samples.

Do not use FIRSTGAME as justification for bypassing a missing public surface in Samples.

---

## 15. Superseded Player assumptions from FG-ADR-001 Revision 10

FG-ADR-002 supersedes the following earlier assumptions:

```text
Player requires a fixed initial Demonstration Application catalog

Scene-Provided requires a second dedicated application under Player

Character Selection is ready merely because it is a valid conceptual archetype

Local Multiplayer is ready merely because multiple Slots are conceptually supported

Player/Shared is a pre-created required group-level structure
```

The remaining general FG-ADR-001 rules continue to apply.

---

## 16. Normative summary

```text
Scene-Provided
  canonical coverage = Getting Started / Minimal Game
  no duplicate dedicated Player application by default

Manager-Provisioned
  next Player Demonstration Application

Character Selection
  planned / blocked
  requires sufficient public arbitrary Actor-selection surface

Local Multiplayer
  planned / blocked
  requires sufficient public Slot/device/input contract

Player/Shared
  not preallocated
  introduce/promote only after concrete reuse

Player application count
  evidence-driven
  not frozen to a predetermined count

Player capabilities
  Scenarios by default

Missing public surface
  blocks the sample
  never justifies parallel sample-owned Framework authority
```

---

## 17. Closure

This ADR is the canonical Player sample-scope authority.

FG-ADR-001 continues to own the general sample-program architecture and distribution strategy. Any future change to the Player demonstration catalog, sequencing, blocker status or Player-specific sharing rule should be reconciled here first and reflected into operational READMEs without reintroducing a competing Player catalog in FG-ADR-001.
