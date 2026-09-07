# FG-ADR-002 — Player Sample Scope and Demonstration Architecture

Status: **ACCEPTED — CANONICAL PLAYER SAMPLE SCOPE / REVISION 6**  
Accepted on: **2026-08-22**  
Revision 2 updated on: **2026-08-24**  
Revision 3 updated on: **2026-08-26**  
Revision 4 updated on: **2026-08-28**  
Revision 5 updated on: **2026-09-05**  
Revision 6 updated on: **2026-09-07**  
Current document revision: **6**  
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

Revision 4 recorded the original Character Selection closure on 2026-08-28: corrected `LeaveUnresolved` behavior, public `PlayerSessionObserver` composition, ActorProfile-driven button presentation and Full Player `30/30` evidence.

Revision 5 records the **current-composition closure** after the Player prefab rebuild: Actor profiles now resolve concrete `PresentationPrefab` assets through the current Player Actor Runtime Host / Presentation boundary, the shared technical prefab baseline is concrete reuse, and Character Selection was reproven in consumer Play Mode on 2026-09-05. Revision 5 moved the historical Local Multiplayer blocker to a current public-contract re-audit before construction.

Revision 6 records the result of that re-audit and the first Local Multiplayer consumer proof. The current Framework public surface is sufficient for the implemented two-Slot Manager-Provisioned Join/Leave/Rejoin path without sample-owned Slot, device or input authority. Local Multiplayer is now materialized and its first lifecycle slice was proven in consumer Play Mode on 2026-09-07, including occupancy-driven UI, fresh-occurrence Rejoin and Activity placement reapplication. The application remains **in progress** until the remaining P2/input/full-Slot/Joining-control behaviors are proven.

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
  MATERIALIZED / PLAY MODE PROVEN

Character Selection
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
  CLOSED / PLAY MODE REPROVEN 2026-09-05
  current ActorProfile.PresentationPrefab composition

Local Multiplayer
  HostProvisioning = ManagerProvisioned
  two configured local Player Slots
  historical public Slot/device/input blocker closed for implemented path
  MATERIALIZED
  FIRST LIFECYCLE SLICE PLAY MODE PROVEN 2026-09-07
  ACTIVE CONSTRUCTION CONTINUES
```

The 2026-08-28 Character Selection lifecycle proof remains historical evidence. The 2026-09-05 run is the current Character Selection physical-composition proof after the Player Actor / Presentation rebuild. The 2026-09-07 Local Multiplayer run is the current consumer proof for the first multiplayer lifecycle slice.

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
    CLOSED / PLAY MODE REPROVEN

  Local Multiplayer
    HostProvisioning = ManagerProvisioned
    two configured local Player Slots
    public Slot/device/input ownership boundary sufficient for current path
    MATERIALIZED / FIRST LIFECYCLE SLICE PLAY MODE PROVEN
    continue remaining multiplayer proofs before closure
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

Therefore a second dedicated Scene Player application under Player is not required merely for symmetry.

---

## 6. Player Provisioning

Status: **MATERIALIZED / PLAY MODE PROVEN**

`Player Provisioning` is the product-facing name for the Manager-Provisioned application whose Session configuration resolves a configured Default Actor.

Core mental model:

```text
Player Provisioning authority
  -> uses authored Local Player Host Prefab
  -> creates Local Player Host when Join is explicitly requested
  -> Session Slot is joined
  -> configured Default Actor is selected/prepared
  -> Player Actor Runtime Host
  -> configured Actor Presentation
  -> Activity participation/readiness completes
```

The provisioning setup is **not itself a Player Host**.

This application remains the canonical demonstration of Session-authorized Local Player Host creation with default Actor resolution.

---

## 7. Character Selection

Status: **CLOSED / PLAY MODE REPROVEN — 2026-09-05**

Character Selection is a distinct Player Demonstration Application because its Session creation-time Actor-resolution intent differs from Player Provisioning:

```text
Player Provisioning
  ActorResolution = ResolveConfiguredDefault

Character Selection
  ActorResolution = LeaveUnresolved
```

### 7.1 Public consumer model

The canonical flow is:

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
  -> Player Actor Runtime Host
  -> ActorProfile.PresentationPrefab materialization
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

```text
Joined
  -> selected Actor = none
  -> Preparing
  -> WaitingForActorSelection
```

The Framework must **not** invoke Default Actor resolution in this branch.

Canonical behavior:

```text
ResolveConfiguredDefault
  -> may resolve configured Default Actor

LeaveUnresolved
  -> never attempts default selection
  -> waits for explicit selection
```

Revision 4 recorded the runtime correction that established this behavior.

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

The Observer remains outside the panel it activates/deactivates so its scoped observation lifetime is not disabled with the UI.

### 7.4 ActorProfile-driven button presentation

Each selection button keeps `PlayerSessionSelectActorCommandTrigger` as the authority for which `ActorProfile` will be selected.

The sample-owned `CharacterSelectionActorButtonPresenter` reads that same command's `ActorProfile` and projects presentation only:

```text
PlayerSessionSelectActorCommandTrigger.ActorProfile
  ├── DisplayName -> button label
  └── Icon        -> button image
```

The presenter does not select Actors, mutate Session state, own Player lifecycle, perform Player discovery, register another ActorProfile authority or silently wire the Button command.

### 7.5 Current Player Actor / Presentation composition

Revision 5 records the current physical composition used by Character Selection.

The reusable technical prefab baseline is:

```text
Assets/_Sample/PlayerSamples/Shared/Prefabs/
  FG_Player.prefab
  FG_PlayerActor.prefab
  FG_Presentation.prefab
```

Character Selection Actor profiles now use the current presentation contract:

```text
ActorProfile_Farmer
  -> PresentationPrefab = FG_FarmerPresentation

ActorProfile_Cow
  -> PresentationPrefab = FG_CowPresentation
```

Concrete presentation prefabs:

```text
Assets/_Sample/PlayerSamples/Player/Players/
  FG_FarmerPresentation.prefab
  FG_CowPresentation.prefab
```

Both concrete variants derive from the shared `FG_Presentation` baseline.

Canonical physical teaching chain:

```text
selected ActorProfile
  -> Actor preparation
  -> Player Actor Runtime Host
  -> Presentation Mount
  -> ActorProfile.PresentationPrefab
  -> selected concrete Presentation
```

The old `LogicalActorHostPrefab` composition is not part of the current Character Selection sample contract.

The concrete presentations provide the sample-facing gameplay/presentation behavior required by the demonstration, including the selected character presentation, Player gameplay input consumption and Follow camera composition.

### 7.6 Sample ownership boundary

The game/sample owns:

```text
which ActorProfile choices are presented
character labels/icons/layout
UI visibility wiring
which explicit selection command the user invokes
concrete Farmer/Cow Presentation authoring
```

The Framework owns:

```text
Joined Slot validity
Actor-resolution policy
selection revision
selection commit
Session duplicate-selection policy
Actor preparation barrier
Player Actor Runtime Host lifecycle
PresentationPrefab materialization
Activity participation/admission/readiness
```

The sample must not use private/internal runtime access, reflection, sample-specific Session discovery, direct mutation of internal Session state, parallel Actor-selection authority, hidden fallback Actor or sample-owned Actor preparation/materialization.

### 7.7 Initial selection, not hot swap

Character Selection demonstrates **initial explicit Actor selection** after Join.

Do not add Replace/Clear UI merely because those public APIs exist.

`Replace Actor Selection` is not a physical hot-swap command. Once the Actor is prepared, the preparation barrier governs later replacement semantics.

### 7.8 Validation evidence

Historical consumer proof — **2026-08-28**:

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

Historical Framework Full Player aggregate:

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

Current-composition consumer reproof — **2026-09-05**:

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

This closes Character Selection authoring/proving on the current Player architecture. Final UPM promotion/import proof remains a later Player sample-group release gate.

---

## 8. Local Multiplayer

Status: **MATERIALIZED / FIRST LIFECYCLE SLICE PLAY MODE PROVEN — 2026-09-07 / ACTIVE CONSTRUCTION**

Local Multiplayer requires more than multiple Player objects in one scene.

The demonstration communicates a canonical relationship among:

```text
local participant / InputDevice intent
Slot allocation
Player admission
input ownership/routing
Actor resolution/preparation
Activity placement/readiness
current Slot occupancy observation
leave / rejoin lifecycle
```

### 8.1 Historical blocker

The blocker recorded in August 2026 was insufficient public **Slot/device/input ownership and observation** for a normal consumer.

At that time the ordinary Join surface did not provide enough evidence to approve sample construction without risking a parallel sample-owned Slot/device/input authority.

Revision 5 therefore required a fresh public-contract audit before materialization.

### 8.2 Revision 6 blocker closure

The re-audit against the current Framework found a sufficient public path for the implemented Local Multiplayer scenario.

Canonical request side:

```text
PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionLeaveCommandTrigger
```

Canonical scoped observation side:

```text
IPlayerSessionScopedAccess
  -> Changed
  -> TryGetObservation(...)
  -> PlayerSessionScopedObservationSnapshot
  -> Slots
  -> PlayerSessionScopedSlotObservation.IsJoined
```

The implemented Join request consumes an `InputDevice`; the Framework owns available-Slot reservation/allocation, Local Player Host admission, PlayerInput ownership, Actor lifecycle and gameplay routing. The sample does not author exact Slot assignment as a parallel authority.

The sample-owned `LocalMultiplayerKeyboardGamepadSimulator` creates deterministic test InputDevices only. It does not own Slot association or gameplay input routing.

The historical blocker is therefore **closed for this consumer path**.

This closure does not imply that every conceivable local-multiplayer topology or explicit-Slot targeting mode is now part of the sample contract.

### 8.3 Materialized application

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

The Session uses two configured local Player Slots and Manager-Provisioned Host creation.

### 8.4 Occupancy-driven presentation rule

Tutorial/UI state must represent **current authoritative Slot occupancy**, not historical successful Join count.

Canonical state table:

```text
P1 Available / P2 Available
  -> Waiting for Player 1

P1 Joined / P2 Available
  -> Player 1 joined; waiting for Player 2

P1 Available / P2 Joined
  -> Player 2 joined; waiting for Player 1

P1 Joined / P2 Joined
  -> Completed / both Players joined
```

This rule is important for Leave/Rejoin: a second successful Join by the same logical Slot does not imply that another Slot is occupied.

### 8.5 First consumer proof — 2026-09-07

The first lifecycle slice is proven in FIRSTGAME consumer Play Mode.

Observed sequence:

```text
Open Joining
-> Join P1
-> P1 prepared / physically materialized / GameplayReady
-> UI waits for P2

Leave P1
-> exact Session Player occurrence terminates
-> player.1 returns to Available
-> Activity representation / Actor / provisioning resources release
-> UI returns to Waiting for Player 1

Rejoin P1
-> same logical Slot player.1
-> fresh Session Player occurrence
-> new host / assignment / Actor materialization
-> Activity placement reapplied
-> UI still waits for P2

Join P2
-> player.2 joins
-> configured Actor prepared/materialized
-> both Players GameplayReady
-> Activity readiness completes
-> UI Completed

Leave P1 while P2 remains Joined
-> P1 resources release
-> P2 remains active
-> UI = Player 2 joined; waiting for Player 1

Rejoin P1 after the Activity had completed
-> fresh P1 occurrence reconciles
-> placement reapplied
-> Activity readiness completes again
-> UI returns to both Players joined
```

Manual visual validation confirmed both the status UI and the authored placement behavior.

### 8.6 Technical supporting evidence

The current Framework Full Player QA aggregate is:

```text
PLAYER QA CERTIFIED
17/17
```

The aggregate includes the Leave/Rejoin lifecycle reconciliation and Activity relocation contracts exercised by the Local Multiplayer consumer flow.

FIRSTGAME remains the consumer/integration proof; QAFramework remains the exhaustive technical proof surface.

### 8.7 Remaining Local Multiplayer proof

Revision 6 does **not** close the entire Local Multiplayer Demonstration Application.

Next consumer proof cut should cover:

```text
P2 Leave/Rejoin while P1 remains active
P1 and P2 independent gameplay input/device ownership
no cross-control between local Players
behavior when both configured Slots are occupied
Close Joining while existing Players remain joined
Reopen Joining behavior when applicable
```

Split-screen remains not implied by this ADR. Camera/output topology should be added only if the final demonstration scope requires it.

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

> **A missing public consumer contract blocks the sample; it does not authorize the sample to implement a substitute framework.**

Player sample code may provide game-owned presentation and interaction such as join prompts, character-selection UI, ActorProfile button presenters, simple HUD, minimal locomotion, test-device simulation and sample navigation.

It must not provide hidden Framework responsibilities such as internal Player discovery, private Actor mutation, parallel Slot registry, parallel device ownership, parallel input routing authority, reflection-based binding or silent fallback.

Character Selection satisfies this gate and is closed. Local Multiplayer also satisfies the gate for the currently implemented Join/Leave/Rejoin/occupancy path and remains in active construction for the remaining multiplayer proofs.

---

## 11. Player/Shared

Status: **CONDITIONAL / CREATED BY CONCRETE REUSE**

Canonical rule:

```text
used by one Player Demonstration Application
  -> keep local

concretely reused by two or more Player Demonstration Applications
  -> promote reusable technical/presentation content to Player/Shared

no concrete cross-application reuse
  -> do not create/promote Player/Shared content
```

Revision 5 records concrete shared Player prefab reuse:

```text
Player/Shared/Prefabs/
  FG_Player
  FG_PlayerActor
  FG_Presentation
```

This does **not** justify moving application authority upward.

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

Reusable presentation/content may be shared; application/session authority remains local.

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
   CLOSED / PLAY MODE REPROVEN 2026-09-05

3. Local Multiplayer
   HostProvisioning = ManagerProvisioned
   two configured local Player Slots
   MATERIALIZED
   FIRST LIFECYCLE SLICE PLAY MODE PROVEN 2026-09-07
   CURRENT ACTIVE PLAYER WORK ITEM
```

This order may change only from concrete implementation/product evidence.

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

The historical Full Player `30/30` result remains technical evidence for the older Player runtime surface. Character Selection's 2026-09-05 Play Mode rerun proves the rebuilt consumer composition on the current Player Actor / Presentation chain.

The current Full Player aggregate on 2026-09-07 is `PLAYER QA CERTIFIED 17/17` on the current suite composition. Local Multiplayer's 2026-09-07 FIRSTGAME run independently proves the implemented two-Player Join/Leave/Rejoin/placement/UI integration path as a real consumer.

Do not duplicate every Player permutation in Samples and do not use FIRSTGAME as justification for bypassing a missing public surface.

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

Character Selection proof still represents the current physical Player composition
  -> superseded 2026-09-05 by PresentationPrefab/current prefab-chain reproof

LeaveUnresolved may attempt Default Actor resolution during Activity reconcile
  -> superseded; LeaveUnresolved waits for explicit Actor selection

Local Multiplayer is ready merely because multiple Slots are conceptually supported

The August Local Multiplayer blocker may be copied forward forever without re-auditing later Player cuts
  -> superseded by Revision 5 re-audit requirement

Local Multiplayer remains blocked by insufficient public Slot/device/input ownership and observation
  -> superseded 2026-09-07 for the implemented Manager-Provisioned two-Slot path
  -> current public commands + scoped Session observation are sufficient without parallel sample authority

A successful Join counter may represent current multiplayer occupancy
  -> superseded; current Slot occupancy is authoritative

Player/Shared is a pre-created required group-level structure
  -> superseded; Shared exists only where concrete reuse justifies it
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
  ActorProfile DisplayName/Icon may drive sample UI presentation
  ActorProfile.PresentationPrefab owns the selected concrete Actor presentation reference
  current concrete variants = FG_FarmerPresentation / FG_CowPresentation
  Framework owns selection/preparation/Player Actor materialization/readiness
  CLOSED / PLAY MODE REPROVEN 2026-09-05

Player/Shared
  created by concrete reuse only
  current reusable prefab baseline = FG_Player / FG_PlayerActor / FG_Presentation
  application/session authority remains local

Local Multiplayer
  ManagerProvisioned two-Slot Demonstration Application
  historical public ownership/observation blocker closed for implemented path
  current state observed through IPlayerSessionScopedAccess typed Slot evidence
  UI derives from current Slot occupancy, never historical Join count
  Leave -> Rejoin creates a fresh Session Player occurrence
  Activity placement is reapplied to the fresh occurrence
  P1 may leave while P2 remains active
  MATERIALIZED / FIRST LIFECYCLE SLICE PLAY MODE PROVEN 2026-09-07
  remaining P2/input/full-Slot/Joining-control proofs still required before closure

Public-surface rule
  missing product contract blocks the sample
  never replace it with sample-owned framework authority
```
