# FG-ADR-002 — Player Sample Scope and Demonstration Architecture

Status: **ACCEPTED — CANONICAL PLAYER SAMPLE SCOPE / REVISION 10**  
Accepted on: **2026-08-22**  
Revision 2 updated on: **2026-08-24**  
Revision 3 updated on: **2026-08-26**  
Revision 4 updated on: **2026-08-28**  
Revision 5 updated on: **2026-09-05**  
Revision 6 updated on: **2026-09-07**  
Revision 7 updated on: **2026-09-19**  
Revision 8 updated on: **2026-09-20**  
Revision 9 updated on: **2026-09-20**  
Revision 10 updated on: **2026-09-22**  
Current document revision: **10**  
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

Revision 5 recorded the **then-current composition closure** after the Player prefab rebuild: Actor profiles resolve concrete `PresentationPrefab` assets through the Player Actor Runtime Host / Presentation boundary, the shared technical prefab baseline is concrete reuse, and Character Selection was reproven in consumer Play Mode on 2026-09-05. Revision 5 moved the historical Local Multiplayer blocker to a current public-contract re-audit before construction.

Revision 6 records the result of that re-audit and the first Local Multiplayer consumer proof. The current Framework public surface is sufficient for the implemented two-Slot Manager-Provisioned Join/Leave/Rejoin path without sample-owned Slot, device or input authority. Local Multiplayer is now materialized and its first lifecycle slice was proven in consumer Play Mode on 2026-09-07, including occupancy-driven UI, fresh-occurrence Rejoin and Activity placement reapplication. The application remains **in progress** until the remaining P2/input/full-Slot/Joining-control behaviors are proven.

Revision 7 records the Character Selection consumer migration to `IF-ADR-029`. Farmer/Cow presentations now contribute explicit `ActorCameraSubjectAuthoring` evidence through `CameraMount`; Player-owned Camera rig/Cinemachine materialization and the old Camera-relative movement dependency are removed. Character Selection reuses the Manager-Provisioned persistent `CameraSharedComposition` and its Third Person Gameplay Rig. The prior Player lifecycle proof remains historical evidence; Unity/consumer revalidation of the migrated Camera path is pending.

Revision 8 records the Local Multiplayer Camera topology decision and implementation. The application now uses one application-owned `CameraSharedComposition` with `AllAvailableSubjects` and a `GroupCameraRigBehaviorDefinition`. Prepared Player Actors contribute Camera Subjects only; Group membership follows Subject availability rather than Slot/device state. One Subject remains a valid one-member Group, multiple Subjects are framed together, and zero Subjects release the normal request so the Output presents its Fixed Default. Split-screen remains outside this Demonstration Application.

Revision 9 records the functional consumer reconciliation after `IF-ADR-030`. Local Multiplayer now owns dedicated Farmer/Cow Group ActorProfiles and Presentations, a camera-independent `MinimalLocalMultiplayerMovement`, presentation-owned Group framing anchors and per-Subject framing radii. Manual Unity Play Mode on 2026-09-20 confirms the current Group Camera consumer path is functional. Remaining Camera work is visual tuning of the application-owned Group behavior asset; QAFramework/technical certification remains separate.

Revision 10 reconciles Player samples with the normative `IF-ADR-032` Camera model. Character Selection now uses GameApplication-owned physical Output capacity, Route/Activity `CameraPresentationDefinition` ownership and Player `ExplicitSelection`. Local Multiplayer now uses one GameApplication-owned shared Output, a Route default Presentation, one Activity-owned Group Presentation with `AllAvailableSubjects`, and Actor-authored Framing Radius evidence. Manual Unity Play Mode on 2026-09-22 proves Local Multiplayer Route/Activity Presentation participation, one-Output Session materialization and shared Group framing. CAMERA-032-E is intentionally not claimed by Local Multiplayer because it has no Player-specific Camera Presentation binding.

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

Status: **CAMERA-032-D/E UNITY VALIDATED — current Player-specific Camera selection proof recorded 2026-09-22**

Character Selection remains a distinct Player Demonstration Application because its Session Actor-resolution intent is:

~~~text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
~~~

A Player may therefore be Joined while no Actor and no Camera Subject exists.

### 7.1 Player lifecycle

~~~text
Open Joining
  -> Join
  -> Slot Joined
  -> WaitingForActorSelection

Farmer / Cow choice
  -> PlayerSessionSelectActorCommandTrigger
  -> Actor selection committed
  -> Actor prepared/materialized
  -> GameplayReady

Leave
  -> current Player / Actor occurrence released

Rejoin
  -> fresh Player occurrence
  -> WaitingForActorSelection again
  -> fresh explicit Actor choice required
~~~

### 7.2 Current Camera-032 composition

Physical Camera capacity belongs to the GameApplication:

~~~text
GameApplication_CharacterSelection
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings
      Player Slot -> CameraOutput_Main

    Player Presentation Bindings
      Player Slot -> CameraPresentation_ThirdPerson
~~~

Lifecycle-owned Presentation intent:

~~~text
Route_Character Selection
  -> CameraPresentation_Default_Route
     precedence = 200
     transition = Cut

Activity_Character Selection
  -> CameraPresentation_ThirdPerson
     Subject Policy = ExplicitSelection
     precedence = 300
     transition = Blend
~~~

The selected Actor Presentation supplies Camera Subject evidence only. It does not own a physical Camera Output, Camera Rig or CameraRequest authority.

The Player Presentation binding does not materialize a Presentation. The Activity owns the live Presentation occurrence; the binding attaches the exact current Player Camera Subject to that occurrence.

### 7.3 Validated consumer behavior

Unity Play Mode on 2026-09-22 confirmed:

~~~text
one Session Camera Output materialized
Route Presentation materialized
Activity Third Person Presentation materialized
Player Presentation selection attached

Join
  -> WaitingForActorSelection
  -> Route Camera remains

Select Farmer / Cow
  -> Actor/Subject occurrence materializes
  -> ExplicitSelection resolves exact Subject
  -> Third Person [300] becomes effective

Leave
  -> Actor/Subject ends
  -> Route [200] becomes effective

Rejoin
  -> fresh Player occurrence
  -> fresh explicit Actor selection
  -> fresh Camera Subject occurrence
~~~

Historical Player lifecycle proofs from 2026-08-28 and 2026-09-05 remain supporting evidence for the Player contract, but IF-ADR-032 is the current Camera authority.

### 7.4 Ownership boundary

The sample owns:

~~~text
ActorProfile choices
selection UI/presentation
concrete Farmer/Cow Actor Presentation authoring
minimal gameplay movement/look
exact Actor Camera observation Transform
Route/Activity Camera Presentation declarations
GameApplication Player Camera bindings
~~~

The Framework owns:

~~~text
Session/Slot lifecycle
Actor selection/preparation
Actor occurrence lifetime
Camera Subject publication lifetime
Player explicit Camera Subject selection
Camera Presentation arbitration/lifecycle
physical Camera Output lifecycle
Default fallback continuity
~~~

Do not restore `CameraSharedComposition`, Player-owned Camera rigs/Outputs or private Player discovery.


## 8. Local Multiplayer

Status: **CAMERA-032 SHARED-GROUP CONSUMER UNITY PASS — one-Output Session + Route/Activity Presentation proof 2026-09-22**

Local Multiplayer demonstrates a two-Slot Manager-Provisioned Player flow with one shared Camera view.

### 8.1 Player boundary

The public request/observation boundary remains:

~~~text
PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionLeaveCommandTrigger

IPlayerSessionScopedAccess
  -> Changed
  -> TryGetObservation(...)
  -> current Slot occupancy
  -> current InputOwnership
~~~

The Framework owns Slot allocation, Host admission, Actor preparation, input ownership and gameplay routing. The sample does not implement substitute authority.

### 8.2 Current application

~~~text
GameApplication_LocalMultiplayer
PlayerSessionProfile_LocalMultiplayer
PlayerSlotProfile_LocalMultiplayer_P1
PlayerSlotProfile_LocalMultiplayer_P2

ActorProfile_FarmerGroup
ActorProfile_CowGroup
FG_FarmerPresentationGroup
FG_CowPresentationGroup

Route_LocalMultiplayer
Activity_LocalMultiplayer

PF_LocalMultiplayer_Activity_Presentation
~~~

### 8.3 Camera-032 shared-screen topology

The GameApplication owns one explicit physical Output:

~~~text
GameApplication_LocalMultiplayer
  Camera Session
    Output Prefabs
      PF_CameraOutput_Main

    Player Output Bindings = []
    Player Presentation Bindings = []
~~~

Those empty Player bindings are intentional. This is not a split-screen or Player-specific camera topology.

The Route owns its neutral entry Presentation:

~~~text
Route_LocalMultiplayer
  -> CameraPresentation_Default_Route
     Output = CameraOutput_Main
     Transition = Cut
     Request Precedence = 200
~~~

The Activity owns one shared Group Presentation:

~~~text
Activity_LocalMultiplayer
  -> CameraPresentation_LocalMultiplayer
     Output = CameraOutput_Main
     Rig = PF_LocalMultiplayer_Activity_Presentation
     Subject Policy = AllAvailableSubjects
     Transition = Blend
     Request Precedence = 300
~~~

The Presentation prefab materializes the Group rig:

~~~text
PF_LocalMultiplayer_Activity_Presentation
  CameraRigComposer
    Behavior = CameraBehavior_Group
    CinemachineTargetGroup
    CinemachineGroupFraming
~~~

### 8.4 Actor Camera Subject evidence

Prepared Player Actors publish Camera Subjects only:

~~~text
FG_FarmerPresentationGroup
  ActorCameraSubjectAuthoring
    Observation Transform
    Framing Radius

FG_CowPresentationGroup
  ActorCameraSubjectAuthoring
    Observation Transform
    Framing Radius
~~~

The observation target may be an empty GameObject.

Group radius projection is:

~~~text
Target.Radius =
  Subject.FramingRadius        when specified
  CameraBehavior_Group.MemberRadius otherwise
~~~

Actor-specific extent belongs to the Actor Presentation. The Group behavior radius remains a fallback.

### 8.5 Canonical Camera membership lifecycle

~~~text
0 Subjects
  -> Group Presentation cannot satisfy required Group target
  -> lower-precedence Route/Session request or Output Default

1 Subject
  -> one-member Group

2 Subjects
  -> two-member Group
  -> one shared Camera frames both

Player leaves
  -> exact Actor Subject removed
  -> same Group Presentation reconciles remaining membership

Rejoin
  -> fresh Actor occurrence
  -> fresh Camera Subject occurrence
~~~

Membership follows Camera Subject availability, never Slot index, Player index, device identity or tutorial state.

### 8.6 Consumer proof — 2026-09-22

Manual Unity Play Mode confirms:

~~~text
Camera Session Outputs materialized = 1

Route Presentation
  CameraPresentation_Default_Route
  scope = Route
  Cut / 200

Activity Presentation
  CameraPresentation_LocalMultiplayer
  scope = Activity
  Blend / 300

P1
  -> Farmer Group Actor / Subject
  -> Group Presentation functional

P2
  -> Cow Group Actor / Subject
  -> same Group Presentation frames both

Actor Framing Radius
  -> Target Group receives presentation-space extent
  -> close framing from dimensionless observation target corrected
~~~

This is current consumer evidence for IF-ADR-032 CAMERA-032-C Route/Activity participation and CAMERA-032-D one-Output Session materialization.

It does **not** validate CAMERA-032-E because this sample uses `AllAvailableSubjects`, not Player `ExplicitSelection`.

It also does not claim two physical Outputs, split-screen layout or full IF-ADR-032 certification.

### 8.7 Player proof history

The 2026-09-07 Player proof remains valid for:

~~~text
P1/P2 Join
P1/P2 Leave/Rejoin
fresh Session Player occurrences
Activity placement reapplication
current occupancy UI
distinct device ownership
~~~

The Camera-032 migration does not replace that Player evidence.

### 8.8 Remaining sample-level work

~~~text
actual gameplay no-cross-control proof for P1/P2
extra-Join behavior while both configured Slots are occupied
Close/Reopen Joining behavior where still desired
visual Group Camera tuning
~~~

The remaining IF-ADR-032 acceptance/QA matrix belongs to Framework technical validation, not to sample-owned Camera work.


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

Player sample code may provide game-owned presentation and interaction such as join prompts, character-selection UI, ActorProfile button presenters, simple HUD, minimal locomotion, test-device simulation, sample navigation and application-specific Camera composition authoring through the public Camera product surface.

It must not provide hidden Framework responsibilities such as internal Player discovery, private Actor mutation, parallel Slot registry, parallel device ownership, parallel input routing authority, sample-owned Camera Subject/Slot registries, Player-owned Camera request/rig/Output authority, reflection-based binding or silent fallback.

Character Selection and Local Multiplayer both satisfy the Player public-surface gate. Their current Camera composition is migrated to IF-ADR-032; remaining technical Camera certification belongs to Framework/QA rather than to missing Player public contracts.

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
   LIFECYCLE / DEVICE PROOF 2026-09-07
   CAMERA-032 SHARED GROUP CONSUMER UNITY PASS 2026-09-22
   remaining sample work = multiplayer edge behavior / visual tuning
   remaining Camera certification = Framework / QA
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

The historical Full Player `30/30` result remains technical evidence for the older Player runtime surface. Character Selection's 2026-09-05 run remains historical Player/materialization evidence; the current CAMERA-032-D/E camera path was consumer-validated on 2026-09-22.

The current Full Player aggregate on 2026-09-07 is `PLAYER QA CERTIFIED 17/17` on the current suite composition. Local Multiplayer's 2026-09-07 run proves the Player lifecycle/integration path, while the 2026-09-22 run independently proves the migrated IF-ADR-032 shared Group Camera consumer path.

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
  current state observed through IPlayerSessionScopedAccess typed Slot evidence
  UI derives from current Slot occupancy, never historical Join count
  Leave -> Rejoin creates a fresh Session Player occurrence
  Activity placement is reapplied to the fresh occurrence
  GameApplication owns one explicit shared Camera Output
  Route owns CameraPresentation_Default_Route
  Activity owns CameraPresentation_LocalMultiplayer
  Subject Policy = AllAvailableSubjects
  Farmer/Cow Actor Presentations own Observation Transform + Framing Radius evidence
  shared Group Camera IF-ADR-032 consumer proven in Unity on 2026-09-22
  no Player Output/Presentation bindings are required for this shared-screen topology
  Camera visual tuning remains application-owned
  full Camera certification remains Framework / QA work

Public-surface rule
  missing product contract blocks the sample
  never replace it with sample-owned framework authority
```
