# FG-ADR-001 — Immersive Framework Sample and Demonstration Strategy

Status: **FROZEN BASELINE — REVISION 11 / PLAYER SCOPE DELEGATED TO FG-ADR-002**  
Baseline frozen on: **2026-08-16**  
Revision 2 consolidated on: **2026-08-16**  
Revision 3 consolidated on: **2026-08-16**  
Revision 4 consolidated on: **2026-08-16**  
Revision 5 consolidated on: **2026-08-16**  
Revision 6 consolidated on: **2026-08-16**  
Revision 7 finalized on: **2026-08-16**  
Revision 8 finalized on: **2026-08-16**  
Revision 9 finalized on: **2026-08-16**  
Revision 10 updated on: **2026-08-16**  
Revision 11 updated on: **2026-08-22**  
Current document revision: **11**  
Canonical filename: **`FG-ADR-001-Immersive-Framework-Sample-and-Demonstration-Strategy.md`**  
Scope: **Samples authoring + `Samples~` distribution strategy / product UX exploration / consumer demonstration**  
Source of truth for framework architecture: **`com.immersive.framework` ADRs 001–022 and current package implementation**  
Normative status: **Frozen product/sample-program baseline. Non-normative for Framework runtime architecture; the official IF-ADRs remain authoritative.**

---

## 1. Purpose

This document preserves the current strategy for rebuilding the Immersive Framework sample/demo surface from a clean product perspective.

The goal is not to freeze the final number of demonstrations, final names, visual style or exact scene topology. The goal is to preserve a mature starting point so further discussion can improve the divisions without losing the discoveries already made.

The previous sample/package attempt must not be treated as the structural baseline for the new sample program. Existing graphical assets, materials, models, textures, animations and other presentation-only resources may be reused where useful, but previous scene composition, sample taxonomy and technical setup do not define the new architecture.

Revision 11 preserves the general sample-program grammar while moving the **Player-specific sample scope, sequencing and blockers** into `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`. This removes the obsolete assumption that a fixed Player Demonstration Application catalog is frozen up front.

During active development, samples are currently authored under the visible `Assets/_Sample/` workspace in `planet-devourer:main`, so Unity imports them normally and they can be created, inspected and edited through the Project Browser. The official final distribution remains package-owned and uses the UPM `Samples~/` convention plus `package.json` `samples` metadata.

Revision 8 also corrects the physical meaning of global Shared content: a UPM sample import copies only the selected sample subfolder into `Assets`, so one importable sample group must not depend on a sibling top-level `Shared` folder.

Camera and Audio remain transversal across the sample program. Persistence remains deliberately more technical and initially proves only **Preferences** and **Progression Save** consumer usage.

The structure and workflow are frozen enough to implement. Scenario details, art choices and exact Unity asset topology remain evolutionary where explicitly marked.

Revision 2 introduced:

```text
Getting Started / Foundation
Camera as transversal coverage
Player demonstration applications
GameApplication / Session boundaries
character-selection progression
UPM grouping
multi-level Shared content
Audio as optional ecosystem integration
```

---

## 1.1 Decision-status vocabulary

This document distinguishes structural rules from implementation ideas.

```text
FROZEN / CONVENTION
  structural or ownership rule the sample program should follow

GUIDELINE
  preferred direction that may be overridden by a justified case

CANDIDATE
  current idea for a scenario, visual treatment, content or implementation

OPEN
  intentionally undecided
```

The sample architecture should become stable before all sample content is known.

Canonical rule:

> **Sample structure should be stable; sample content should remain evolutionary.**

New ideas discovered while building the samples are expected and should not require reopening frozen structural decisions unless they reveal a genuine contradiction.

## 1.2 Canonical document maintenance

Status: **FROZEN CONVENTION — REVISION 11**

This ADR uses one stable repository path.

Canonical file:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-001-Immersive-Framework-Sample-and-Demonstration-Strategy.md
```

The revision number belongs **inside the document**, not in the filename.

Do not create new files such as:

```text
...-REV10.md
...-REV11.md
...-REV12.md
```

for ordinary evolution of this ADR.

Instead:

```text
same canonical file
  -> update Current document revision
  -> update revision/date notes
  -> update content
  -> Git history preserves previous states
```

Create a separate ADR only when the decision itself becomes a genuinely separate architectural/product decision rather than a revision of this strategy.

## 1.3 Current operational authoring baseline

Status: **CANONICAL PROVISIONAL BASELINE — 2026-08-22**

For ongoing sample construction, the current `main` Git state is the operational truth:

```text
Repository
  ImmersiveGames/planet-devourer

Branch
  main

Observed baseline before Revision 11
  d5bba00a6c924a5c461d54d2856a6ae269a7a926
  "Player Clear"
```

Current authoring locations:

```text
Assets/_Sample/
  visible Unity authoring workspace for the sample program

Assets/_Project/
  future FIRSTGAME real-game ownership structure

Assets/Documentation~/
  architecture / ADR / plan documentation
```

Current sample group shape at the strategy level:

```text
Assets/_Sample/
├── GettingStarted/
│   └── MinimalGame/              # canonical Scene-Provided coverage
├── GameFlow/
│   └── GameFlowShowcase/
├── Player/
│   ├── ManagerProvisioned/       # next Player Demonstration Application
│   ├── CharacterSelection/       # planned; public Actor-selection blocker
│   └── LocalMultiplayer/         # planned; public Slot/device/input blocker
├── AdvancedContext/
│   └── Showcase/
├── Persistence/
│   └── PersistenceShowcase/
└── Shared/
```

The exact Player sample scope is **not frozen by this tree**. `FG-ADR-002` is authoritative for Player-specific Demonstration Applications, sequencing, blockers and the rule for materializing `Player/Shared` only after concrete reuse exists.

The branch state is **canonical provisionally**, not immutable.

Rule:

```text
before a new implementation cut
  -> inspect planet-devourer:main
  -> use the current Git tree as operational truth

if implementation reveals a better ownership shape
  -> adjust deliberately
  -> update this ADR when the change affects a frozen/conventional rule
```

Do not force the repository back to an older scaffold merely because a previous ZIP or revision differs.

### 1.3.1 Final ownership remains package-side

Using `planet-devourer:main` as the current visible authoring workspace does **not** transfer official sample ownership to FIRSTGAME.

The final relationship remains:

```text
planet-devourer:main / Assets/_Sample/
  current visible construction/proving workspace

com.immersive.framework / Samples~/
  final official UPM sample distribution
```

When a sample group is mature enough for package finalization, materialize/promote the required sample content into the package's `Samples~/` distribution tree and validate it there as a consumer import.

FIRSTGAME's future game structure under `Assets/_Project/` remains separate from the sample taxonomy.

## 2. Problem

The framework has a broad set of accepted and implemented technical contracts, but a consumer still needs an effective way to answer questions such as:

```text
How does this feature behave?
How do these concepts interact?
What does a valid configured composition look like?
Which surface do I inspect to understand it?
How does a simple use case evolve into an advanced one?
How does the Framework compose with other Immersive packages?
```

A developer should be able to import a sample group, choose a configured demonstration, enter Play Mode, observe the feature behaving, and then inspect the resulting assets/components/settings to understand the canonical usage shape.

The sample system therefore becomes both:

```text
consumer-facing executable documentation
+
product/UX investigation surface
```

---

## 3. What samples are not

Samples are not:

```text
step-by-step tutorials
"fill this field" training scenes
QA technical smokes
FIRSTGAME replacements
one sample per ADR
one sample per component
one sample per API
one giant scene containing every system
```

A sample should not primarily teach the user to mechanically reproduce a configuration field by field.

Its primary job is to show a working configured behavior and make the canonical composition inspectable.

---

## 4. Roles of the three proof surfaces

```text
QAFramework
  proves contracts technically
  proves negative cases and regression behavior

Samples~
  demonstrate configured framework behavior
  expose canonical composition patterns
  reveal product-surface and UX friction
  show how the Framework composes with optional ecosystem packages

FIRSTGAME
  proves the framework inside a real game
  proves real composition, content and gameplay integration
```

Canonical distinction:

```text
QAFramework
  "Does the contract work?"

Samples~
  "This is how the configured feature behaves."

FIRSTGAME
  "Can I build a real game with this?"
```

### 4.1 Gameplay boundary

Samples do **not** need gameplay merely to look like small games.

Use gameplay only when it is necessary to expose the framework concept being demonstrated.

```text
If a gameplay mechanic helps the consumer understand the Framework concept
  -> it may belong in the sample.

If a gameplay mechanic only makes the sample feel more like a game
  -> leave it out.

If the goal is to prove real gameplay and real integrated composition
  -> that belongs to FIRSTGAME.
```

Examples:

```text
Getting Started
  navigation only

Character Selection
  game-owned selection UI is useful because it demonstrates Player commands/observation

Pause
  simple movement is enough to make Pause observable

Initial Placement
  locomotion is enough to expose the spatial behavior

Game Flow
  a simple trigger/button may exist only to cause the transition being demonstrated
```

### 4.2 FIRSTGAME is not an "everything sample"

FIRSTGAME does not need to contain every Framework feature.

It is a real, short game that uses the Framework according to the requirements of that game.

The Framework should not distort the game merely to increase feature coverage.

```text
Samples optimize for understanding.

FIRSTGAME optimizes for applicability.
```

A developer may therefore use the sample progression as a careful learning path, or inspect FIRSTGAME directly as a more advanced integrated reference.

---

## 5. Core principle

### Progressive in concept, independent in execution

The demonstrations should form an intentional progression:

```text
Basic
  -> Intermediate
      -> Advanced
```

Progression must not mean runtime dependency between demonstrations.

A later demonstration may reuse the same visual language, characters, UI vocabulary and shared assets, but each Demonstration Application must remain a coherent runnable application when its required `GameApplication` is active.

The UPM import unit and the runnable demonstration unit are not necessarily the same thing.

---

## 6. Three structural units

The current working model distinguishes three different units that were previously conflated.

### 6.1 UPM Sample Group

A broad conceptual area imported through Package Manager.

Examples:

```text
Getting Started
Game Flow
Player
Audio & Ecosystem
Persistence [future]
```

A UPM Sample Group may contain more than one runnable Demonstration Application.

### 6.2 Demonstration Application

A coherent example with its own `GameApplicationAsset` and, when applicable, its own application/session-scoped configuration.

A Demonstration Application exists when the example needs a meaningfully different application/session archetype.

Player may contain more than one Demonstration Application when application/session intent genuinely differs, but the concrete Player catalog is not defined here.

`FG-ADR-002` is authoritative for which Player applications currently exist, which are next, and which remain blocked.

Each materialized Demonstration Application folder should make its `GameApplication` obvious.

### 6.3 Scenario

A Route, Activity, scene or sequence that demonstrates behavior inside one already coherent Demonstration Application.

Examples:

```text
basic join
Activity participation
Player lifetime across Activities
initial placement
Pause / Resume
Leave / Rejoin
```

A Scenario does not require another `GameApplication` merely because it demonstrates another capability.

### 6.4 Sample HUB / Menu

Status: **FROZEN CONVENTION**

When one Demonstration Application contains several independent sample topics, it may start in a sample-only HUB/Menu that lets the consumer select which Scenario to inspect.

Canonical relation:

```text
multiple topics
+
same Demonstration Application
=
optional Sample HUB
```

A HUB is not mandatory.

```text
one topic
or
natural linear demonstration
=
no HUB required
```

The HUB:

```text
selects sample topics
uses public/official Framework surfaces
does not become runtime authority
does not replace Route/Activity/Application authority
does not hide invalid configuration
does not create gameplay progression requirements
```

A consumer should be able to reach a topic quickly. Sample topics must not be locked behind quests, collectibles, completion requirements or unnecessary traversal.

The HUB is sample navigation, not gameplay.

---

## 7. Split rule

Do not create another UPM group, Demonstration Application or Scenario merely because another feature, component or ADR exists.

Choose the smallest structural level that matches the actual conceptual difference.

### New UPM Sample Group

Create only when the broad user problem changes substantially.

### New Demonstration Application

Create when the example requires a materially different application/session configuration or application-scoped authority.

Strong signals include:

```text
Session topology
Host Provisioning origin
Supported Slot universe
initial Joining intent
initial Actor resolution
application-level Player selection policy
other mutually incompatible application-scoped configuration
```

### New Scenario

Create when the application/session archetype remains coherent but runtime behavior, Route/Activity composition or presentation changes enough to deserve a separate executable example.

---

## 8. Primary, Supporting and Ambient features

Every demonstration may contain systems that are not the subject being demonstrated.

```text
Primary
  the concept the demonstration exists to show

Supporting
  required for the demonstration to function coherently

Ambient
  improves readability, presentation or game feel
```

Example:

```text
Player Initial Placement scenario

Primary
  Initial Placement

Supporting
  Game Flow
  Camera
  Input

Ambient
  BGM
  presentation polish
```

Supporting or ambient systems should use the simplest canonical configuration available and must not unnecessarily expand the conceptual burden.

### 8.1 Transversal Camera and Audio — frozen convention

Camera and Audio should not be perceived as features that only exist inside isolated specialist samples.

When natural to the context:

```text
Camera
  may be Supporting or Ambient in Getting Started, Game Flow and Player

Audio
  may be Supporting or Ambient in Game Flow, Player and other compatible contexts
```

Their presence should help communicate that these concerns cross application domains.

However:

```text
Camera
  is part of normal Framework composition

Audio
  remains an optional ecosystem integration
```

Therefore Audio must never become an accidental mandatory dependency of unrelated core samples merely to demonstrate transversality.

The sample design should prefer:

```text
natural contextual use
over
artificial feature isolation
```

while preserving dependency boundaries.

---

## 9. Sample code boundary

Sample-specific scripts are acceptable for demonstration concerns such as:

```text
buttons
triggers
character-selection UI
simple HUD
state visualization
sample-only interaction prompts
```

Sample scripts must consume public/product APIs and must not hide framework product problems by implementing missing framework responsibilities.

They should not:

```text
resolve internal bindings the framework should expose
perform private framework discovery
repair invalid framework composition
materialize contracts that the package should own
create parallel lifecycle/runtime authority
silently find Players/Cameras/Activities by name or hierarchy
silently switch invalid configuration into a working state
```

If a demonstration requires such a workaround to remain understandable, that is product evidence and should be investigated as package UX/architecture friction.

---

## 10. Samples as UX/product audit

Building each demonstration should intentionally answer:

```text
Can a consumer discover the official surface?
Is the intent clear in the Inspector?
How many conceptual decisions are required?
How many technical components are visible?
Are references duplicated unnecessarily?
Does normal setup require private/internal knowledge?
Are invalid states explained clearly?
Is remediation explicit and safe?
Does Advanced / Debug expose useful evidence?
Can normal game-owned UI consume the public APIs cleanly?
```

Friction classification:

```text
Legitimate product intent
  a real designer/developer decision

Repetitive authoring friction
  valid intent entered repeatedly or unnecessarily

Technical leakage
  consumer configures implementation details that should be product-owned

Non-discoverable requirement
  setup requires undocumented internal knowledge
```

---

# 11. Revised mental-model catalog

```text
IMMERSIVE FRAMEWORK SAMPLES
│
├── GETTING STARTED / FOUNDATION
│   └── one minimal application
│
├── GAME FLOW
│   ├── Route + Activity
│   ├── Activity Content / Visibility
│   ├── Transition
│   ├── Loading
│   ├── Readiness
│   └── Restart / Recovery
│
├── PLAYER
│   ├── Session Configuration
│   ├── Slots / Joining
│   ├── Actor Selection
│   ├── Scene-Provided
│   ├── Manager-Provisioned
│   ├── Activity Participation
│   ├── Physical Lifetime
│   ├── Initial Placement
│   ├── Input / Pause
│   └── Leave / Rejoin
│
├── ADVANCED CONTEXT
│   ├── Camera switching inside one Activity
│   ├── advanced Audio / ecosystem behavior
│   └── other transversal gaps only when justified
│
└── PERSISTENCE
    ├── Preferences
    ├── Snapshot
    └── Progression Save
```

Camera is no longer assumed to require its own top-level sample group.

Transversal concerns:

```text
CAMERA COVERAGE
AUTHORING / UX
IDENTITY / OWNERSHIP / DIAGNOSTICS
```

The Player entries in this mental-model catalog describe **capability space**, not a frozen Demonstration Application catalog. Concrete Player sample structure is governed by FG-ADR-002.

---

# 12. ADR-to-intention catalog

| ADR | Consumer-facing intention | Current sample placement | Current role |
|---|---|---|---|
| IF-ADR-001 | Application/Session/Route/Activity ownership and scoped runtime authority | Getting Started / Flow / Player | Foundation / transversal |
| IF-ADR-002 | Official product authoring through inspectable configured examples | All | Transversal UX |
| IF-ADR-003 | Join Players, assign Slots/Actors, project Players into Activities | Player | Core |
| IF-ADR-004 | Scoped Camera requests and one Session output | Getting Started / Game Flow / Player / Advanced Context | Transversal Camera |
| IF-ADR-005 | Input, Pause, Gate, Reset and Activity Restart | Player + Game Flow + possible utility scenario | Distributed |
| IF-ADR-006 | Cover, load, readiness wait, reveal and recovery | Game Flow | Advanced |
| IF-ADR-007 | Decide when an Activity is safe to reveal/play | Game Flow + Player | Advanced |
| IF-ADR-008 | Persistent application content composition | Getting Started and all applications | Core |
| IF-ADR-009 | Activity-local visibility | Game Flow | Intermediate |
| IF-ADR-010 | Discoverable, intent-first, diagnostic product surfaces | All | Transversal UX |
| IF-ADR-011 | Truthful readiness-aware loading progress | Game Flow | Advanced |
| IF-ADR-012 | Activity Player inclusion/exclusion and readiness compatibility | Player | Intermediate / advanced |
| IF-ADR-013 | Optional Route/Activity BGM integration | Primarily Game Flow + Advanced Context + optional ambient use elsewhere | Transversal optional Audio |
| IF-ADR-014 | Definition identity, stable IDs and runtime occurrence evidence | All | Transversal technical |
| IF-ADR-015 | Bounded Player commands and consumer observation | Player | Intermediate / advanced |
| IF-ADR-016 | Slots, Joining, Host Provisioning and initial Actor resolution | Player Demonstration Applications | Core |
| IF-ADR-017 | Project-level frame pacing | Getting Started / documentation | Project configuration |
| IF-ADR-018 | Preferences, Snapshot and durable Progression Save boundaries | Persistence | Partially experimental |
| IF-ADR-019 | Physical Player lifetime vs Activity representation | Player | Core / advanced |
| IF-ADR-020 | Leave, resource release and later Rejoin | Player | Advanced |
| IF-ADR-021 | Explicit initial spatial placement | Player | Intermediate / advanced |
| IF-ADR-022 | Fixed, Follow, Mounted and Third Person presentation models | Getting Started / Game Flow / Player / Advanced Context | Transversal Camera |

---

# 13. Getting Started / Minimal Game

Status of this section: **FROZEN DECISION — 2026-08-16**

This is the first sample area considered sufficiently defined to use as the baseline for later implementation planning.

## 13.1 Purpose

Getting Started answers one question:

> What is the minimum coherent game application I need to start using the Immersive Framework?

It is not a Player tutorial, Camera tutorial, Game Flow tutorial or mini-game.

Its job is to move the consumer from:

```text
Framework installed
```

to:

```text
a coherent Framework application is running
the Player can navigate
the consumer can inspect how the game starts
```

## 13.2 One sample group, one Demonstration Application

The group contains one minimal application:

```text
GETTING STARTED
└── Minimal Game
```

Expected conceptual content:

```text
one GameApplication
one PlayerSessionProfile
Persistent Content
one Route
one Activity
one gameplay scene
one Scene-Provided Player
one Mounted / First Person Camera presentation
minimal movement/look Input
```

No alternative application archetypes are needed inside Getting Started.

## 13.3 Runtime experience

The complete intended runtime experience is intentionally small:

```text
Play
  -> Framework starts the application
  -> startup Route enters
  -> startup Activity enters
  -> Scene-Provided Player is admitted
  -> Mounted Camera presents first-person view
  -> user navigates through the environment
```

Navigation is sufficient.

There is no requirement for:

```text
interaction
objectives
collectables
enemies
combat
doors
gameplay triggers
mission HUD
score
gameplay tutorial
```

The consumer should spend time understanding **how the game starts**, not playing a custom sample game.

## 13.4 Player model — frozen

Getting Started uses:

```text
Scene-Provided Player
```

Reason:

- it is the simplest current Player origin to inspect;
- this Minimal Game is the **canonical executable Scene-Provided reference** for the sample program;
- the Player sample family must not duplicate a dedicated Scene-Provided Demonstration Application under Player merely to restate the same contract;
- Manager-Provisioned and later Player demonstrations exist only when they add a distinct Player contract;
- the first-person presentation means the physical Player does not need to be visually emphasized.

The Player-specific demonstration sequence and blockers are governed by `FG-ADR-002`.

The Player may be a deliberately simple object whose only visible purpose is to provide:

```text
Framework Player composition
movement
look
Camera mount
```

It does not need a complete character mesh or character-gameplay presentation.

## 13.5 Player Session intent

The application should use the smallest coherent Session configuration compatible with the official Scene-Provided path.

Target conceptual shape:

```text
Supported Slots
  one Slot

Host Provisioning
  Scene-Provided

Actor Resolution
  configured/default direct path

Joining / admission
  simplest explicit valid configuration for immediate gameplay
```

Exact serialized values must be verified against the implementation when the sample is built; this draft freezes the product intention, not unverified field values.

## 13.6 Camera model — frozen at product level

Getting Started uses a first-person presentation based on the ADR-022 `Mounted` model.

Purpose:

```text
support navigation
make the sample immediately feel like a running game
cover one Camera presentation model naturally
avoid turning Camera into the lesson
```

The exact rig/prefab details are implementation work, but the presentation intention is frozen:

```text
Mounted
First Person
single Session output
no Camera switching lesson
no arbitration lesson
```

## 13.7 Game Flow shape

Getting Started contains:

```text
one Route
one Activity
```

There is no Route change and no Activity change.

The consumer should be able to inspect that these concepts exist without having to learn transition behavior yet.

Conceptually:

```text
GameApplication
  -> Startup Route
      -> Gameplay Activity
          -> Gameplay Scene
```

Game Flow behavior itself belongs to the `Game Flow` sample group.

## 13.8 Environment

The gameplay scene should be visually credible enough to read as a small game environment rather than a technical smoke scene.

Existing presentation/game props may be reused.

Desired content:

```text
floor / terrain
architecture
lighting
props
visual landmarks
enough spatial variation to make navigation pleasant
```

The environment does not need gameplay.

The sample should communicate:

> This could be the starting point of a game.

It should not attempt to communicate:

> This is a complete mini-game.

## 13.9 UI and diagnostics

Primary runtime UI should be absent or minimal.

A small control hint is acceptable when useful:

```text
Move
Look
```

Do not add a gameplay HUD merely for presentation.

Framework-owned Advanced/Debug surfaces may be inspected when available, but Getting Started should not require a custom technical HUD to explain itself.

## 13.10 Persistent Content

Persistent Content remains visible in the first sample because it is part of the real application composition.

Only infrastructure actually required by the minimal application should be present.

Do not pre-populate future systems merely because later samples may use them.

The exact hierarchy should be designed during implementation, with the goal that a consumer can clearly distinguish:

```text
persistent application composition
vs
Activity-local gameplay content
```

## 13.11 Active Game Application documentation

Getting Started establishes the normal project rule:

```text
A normal game project uses one Active Game Application.
```

It should also introduce the sample-specific convention:

```text
Other sample groups may contain multiple Demonstration Applications
because the package demonstrates several mutually incompatible application configurations
inside one imported Unity project.
```

Those groups use the official explicit `Set Active` workflow.

No hidden sample runtime should switch the application automatically.

## 13.12 Primary / Supporting / Ambient classification

```text
Primary
  minimal Framework application composition

Supporting
  Scene-Provided Player
  Mounted / First Person Camera
  movement/look Input
  Persistent Content

Ambient
  compact environment
  lighting
  existing presentation props
```

Camera is intentionally visible here as a transversal Framework concern:

```text
Mounted / First Person
```

Audio is **not required** in Getting Started. If later implementation proves that a tiny ambient Audio use can be included without adding optional-package dependency friction, it may appear as Ambient content. The frozen rule is that Getting Started must remain independently understandable and must not require Audio merely for showcase value.

## 13.13 Explicitly out of scope

```text
gameplay interaction
Player provisioning alternatives
character selection
multiple Players / Slots as a demonstrated concept
Route transitions
Activity transitions
loading presentation
readiness waiting
Pause
Reset
Audio ecosystem
Persistence
Pooling
Camera switching
Camera arbitration
game objectives
```

If some technical supporting element is mandatory for the official runtime path, it may exist without becoming part of the lesson.

## 13.14 Success criteria

The sample succeeds when:

```text
consumer imports the sample
consumer activates its GameApplication
consumer enters Play Mode
consumer can navigate a reasonably populated environment
consumer can inspect Application -> Route -> Activity -> Player -> Camera composition
```

The consumer should not need advanced ADR knowledge to understand the basic shape.

The sample also acts as a UX test:

> If producing this minimal navigable application requires exposing excessive internal infrastructure or undocumented knowledge, that friction is product evidence.

## 13.15 Frozen rule

```text
Getting Started proves navigation, not gameplay.
```

Do not expand this sample with gameplay mechanics merely to make it more entertaining.

Gameplay belongs in a sample only when it is necessary to demonstrate that sample's Framework concept.

Real gameplay composition is primarily the role of FIRSTGAME.

---

# 14. Game Flow

Status of this section: **FROZEN STRUCTURE / EVOLUTIONARY CONTENT — 2026-08-16**

Game Flow remains the broad context for:

```text
Where is the game now?
What Route is current?
What Activity is current?
How does the game move to another state/content composition?
When is the destination safe to reveal?
```

The known capability space includes:

```text
Route
Activity
Activity-local content
Activity-local visibility
Activity transition
Route transition
Transition cover/reveal
Loading
Readiness
Readiness-aware loading progress
Activity Restart
recovery / committed-not-ready cases
```

These capabilities establish the domain, but they do **not** freeze a one-to-one scenario catalog.

## 14.1 UPM and Application shape — frozen

Game Flow is one UPM Sample Group.

The initial structural baseline is:

```text
GAME FLOW
└── one Demonstration Application
    ├── one GameApplication
    ├── Sample HUB / Menu
    └── multiple Scenarios as needed
```

Reason:

> In Game Flow, changing Route/Activity/state is the subject being demonstrated. Compatible Flow concepts should therefore remain inside the same application unless a genuine application-scoped incompatibility is discovered.

Do not create another `GameApplication` merely to isolate another Flow topic.

A second Demonstration Application is justified only if implementation evidence reveals incompatible application/session-scoped configuration.

## 14.2 Entry surface — frozen

The Game Flow application starts in a **Sample HUB / Menu**.

Purpose:

```text
provide a fast entry point
list the available Flow topics
enter the selected Scenario
return to a known sample-navigation surface when appropriate
```

The HUB is sample-only navigation.

It must not:

```text
become Framework authority
invent another lifecycle
replace Route/Activity semantics
silently repair configuration
require gameplay progression to unlock topics
```

The exact visual form is not frozen. It may be a traditional menu or a compact physical HUB, whichever is smaller and clearer.

## 14.3 Scenario catalog — intentionally evolutionary

The following remain **CANDIDATES**, not commitments:

```text
Basic Flow
  Route + Activity

Composition
  Activity content / visibility

Transition
  cover / reveal

Loading & Readiness
  technical load + readiness wait + truthful progress

Restart / Recovery
  advanced lifecycle behavior
```

During implementation, topics may be:

```text
merged
split
removed
renamed
reordered
or supplemented
```

when that produces clearer executable documentation.

The structural rule remains unchanged:

```text
same coherent GameApplication
  -> add/adjust Scenarios

incompatible application-scoped intent
  -> consider another Demonstration Application
```

## 14.4 Asset strategy — frozen rule, open content

Game Flow should aggressively reuse the shared sample asset library.

Do not create unique art merely because a technical concept has another Scenario.

Preferred direction:

```text
small reusable environment vocabulary
recombined across Scenarios
```

Current **CANDIDATE** visual source:

```text
presentation assets primarily derived from the existing farm-game content
with selected assets from other owned games when they solve a real presentation need
```

The farm theme is not normative.

The normative rule is:

> Samples should minimize unique presentation content and reuse shared assets whenever reuse does not obscure the concept being demonstrated.

## 14.5 Gameplay boundary — frozen

Game Flow does not need gameplay for entertainment.

Gameplay/input exists only when needed to cause or observe a Flow behavior.

Examples of acceptable sample mechanics:

```text
simple menu selection
simple trigger
simple button
simple navigation
```

when they directly invoke or expose the Flow concept.

Do not add combat, quests, puzzles or objectives merely to make a Scenario feel more game-like.

## 14.6 Player / Camera / other systems

Supporting systems should remain as simple as the Scenario allows.

The exact Player and Camera presentation used across Game Flow remains **OPEN/CANDIDATE** until implementation makes the clearest choice evident.

Game Flow must not expand into a Player or Camera tutorial merely because those systems are needed to make a Scenario runnable.

## 14.7 Camera and Audio inside Game Flow — frozen strategy

Game Flow is the primary natural context for basic contextual Camera and Audio behavior.

As Routes and Activities change, the sample may naturally expose:

```text
Camera
  Route-scoped presentation/request
  Activity-scoped presentation/request
  contextual change of presentation

Audio
  Route BGM intent
  Activity BGM intent
  inheritance
  override
  silence
  contextual restoration
```

These behaviors should be demonstrated as part of Flow when they arise naturally, not copied into separate basic Camera/Audio samples.

Camera/Audio may be Supporting or Ambient depending on the Scenario.

The exact mapping remains evolutionary.

For Audio, implementation must preserve the optional package boundary. A Game Flow scenario must not silently become dependent on `com.immersive.audio`; any actual dependency requirement must be explicit in the sample packaging/documentation strategy.

## 14.8 Frozen Game Flow rule

```text
Freeze the navigation and ownership structure.
Let the Scenario ideas evolve during construction.
Use Camera and Audio naturally when they clarify contextual Flow.
```

This is the intended balance for the Game Flow group.

---

# 15. Player

Status of this section: **GENERAL STRATEGY ONLY — PLAYER-SPECIFIC AUTHORITY DELEGATED TO FG-ADR-002 — 2026-08-22**

Player remains a major sample domain, but Revision 11 deliberately stops freezing a concrete Player application catalog inside FG-ADR-001.

The general sample-program grammar remains authoritative here:

```text
UPM Sample Group
  broad consumer problem

Demonstration Application
  materially distinct GameApplication / Session initialization intent

Scenario
  compatible runtime behavior inside one Demonstration Application
```

The concrete Player sample model is owned by:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
```

FG-ADR-002 is authoritative for:

```text
canonical Scene-Provided coverage
current Player Demonstration Applications
implementation sequence
public-surface blockers
Player/Shared materialization rules
```

## 15.1 Canonical Scene-Provided coverage

Getting Started / Minimal Game is the canonical executable reference for the Scene-Provided Player path.

```text
Assets/_Sample/GettingStarted/MinimalGame/
  -> canonical Scene-Provided coverage
```

Do not create a dedicated Scene-Provided application under Player unless future implementation evidence proves that a distinct Scene-Provided-specific consumer contract cannot be demonstrated by the existing canonical reference.

## 15.2 Current Player application direction

FG-ADR-001 records only the current high-level direction:

```text
Manager-Provisioned
  next Player Demonstration Application

Character Selection
  planned
  blocked until a public arbitrary Actor-selection surface is sufficient
  for game-owned selection UI without private/internal workarounds

Local Multiplayer
  planned
  blocked until the public Slot/device/input contract is sufficient
  for a canonical consumer sample without parallel sample-owned authority
```

These statuses are not a new permanent closed catalog. FG-ADR-002 owns their exact scope and exit criteria.

## 15.3 Application/Scenario rule

The Player domain follows the same split rule as the rest of the sample program:

```text
materially incompatible initial Player Session intent
  -> separate Demonstration Application

compatible runtime behavior
  -> Scenario inside that Demonstration Application
```

Player capabilities such as:

```text
Joining
Slot occupancy
Actor Selection
Activity Participation
Activity Representation
Physical Player Lifetime
Initial Placement
Input / Pause
Leave / Rejoin
```

do not automatically create new Demonstration Applications.

## 15.4 Public-surface rule

A Player sample must consume public/product Framework surfaces.

Do not use sample code to bypass a missing product contract for:

```text
Actor selection
Slot ownership
device association
input routing
joining/admission
runtime observation
```

When the required public surface does not exist or is not sufficient, the demonstration remains **planned / blocked**. The missing surface is product evidence, not permission to create a parallel sample architecture.

## 15.5 No combinatorial matrix

Do not build an exhaustive Player sample matrix across provisioning, participation, lifetime, placement, Pause, Leave/Rejoin or other capabilities.

Representative canonical consumer use belongs in Samples.

Broad contract combinations and regressions belong in QAFramework.

## 15.6 Player Shared rule

`Player/Shared` is **not** a required scaffold or preallocated architecture layer.

```text
no concrete reuse
  -> keep content local
  -> do not promote to Player/Shared

two or more concrete Player Demonstration Applications genuinely reuse content
  -> Player/Shared may be introduced/promoted
```

Application/session authority remains local to the owning Demonstration Application.

Do not move authoritative assets such as `GameApplicationAsset`, `PlayerSessionProfile`, Route/Activity definitions or application-specific policies into `Player/Shared` merely to deduplicate files.

## 15.7 Player navigation

When more than one Player Demonstration Application is materialized:

```text
Player README
  -> explains the available applications and status

consumer chooses one application
  -> selects its GameApplication
  -> Set Active

optional per-application HUB
  -> selects compatible Scenarios
```

A global Player runtime HUB must not silently switch Active GameApplications.

## 15.8 Player / Camera / Audio

Player demonstrations may use Camera and optional Audio as Supporting/Ambient concerns when that helps expose the Player contract.

They must not distort the Player sample scope or introduce optional-package dependencies merely for decoration.

## 15.9 Governing rule

> **FG-ADR-001 owns the general sample grammar. FG-ADR-002 owns the concrete Player sample scope, sequence, blockers and Player-specific sharing decisions.**

---

# 16. GameApplication switching in samples

## 16.1 Normal Framework rule

```text
Unity Project
  -> ImmersiveFrameworkSettings
      -> one Active Game Application
```

A real game normally fixes its application configuration rather than switching among unrelated example applications.

## 16.2 Why samples are different

The package intentionally places multiple example applications in one consumer project so different application/session archetypes can be inspected without creating separate Unity projects.

This is a demonstration concern, not a runtime architecture defect.

## 16.3 Official switching path

The current product surface already supports explicit selection:

```text
select GameApplicationAsset
  -> Inspector
      -> Set Active
```

Project Settings also exposes the current Active Game Application.

No sample-specific runtime manager or automatic hidden switch is required.

## 16.4 Documentation rule

Getting Started explains the concept once.

Every group containing multiple Demonstration Applications should also explain:

```text
1. Each demonstration folder owns its own GameApplication.
2. A normal game normally keeps one application active.
3. Multiple applications exist here only for demonstration.
4. Select the intended GameApplication and use Set Active.
5. Open the indicated entry scene and enter Play Mode.
```

This is operational orientation, not a field-by-field setup tutorial.

---

# 17. Camera + Audio transversal strategy

Status of this section: **FROZEN STRATEGY / EVOLUTIONARY PLACEMENT — 2026-08-16**

Camera and Audio are transversal concerns.

They should appear naturally inside the sample contexts that already exercise the relevant lifecycle rather than being treated as isolated product silos.

Canonical principle:

```text
natural contextual coverage
+
focused advanced gap coverage
```

not:

```text
one mandatory sample family per feature
```

## 17.1 Camera — transversal coverage

Camera presentation and request authority are different dimensions.

Presentation models to cover across the sample program:

```text
Fixed
Follow
Mounted
Third Person
```

Request/authority scopes to make observable where relevant:

```text
Session
Route
Activity
eligible Local Player
```

The samples do not need every model/scope combination.

Examples of likely natural placement:

```text
Getting Started
  Mounted / First Person [FROZEN]

Game Flow
  Route/Activity contextual Camera
  presentation changes caused by Flow

Player
  Follow / Third Person
  eligible Local Player request
  Fixed where natural, such as character presentation/selection
```

The exact distribution is **CANDIDATE/EVOLUTIONARY**.

## 17.2 Focused Camera Switching Scenario — frozen need

A specific Camera gap has already been identified:

> Changing Camera presentation/request while remaining inside the same Route/Activity context.

This deserves one focused Scenario because Route/Activity changes would otherwise mask the concept.

Conceptual shape:

```text
same GameApplication
same Route
same Activity
same Player
same physical Camera output

UI selects another Camera presentation/request

Fixed
Follow
Mounted
Third Person
  -> as useful for the final implementation
```

The UI must not teach:

```text
enable Camera A
disable Camera B
```

It should communicate the actual Framework model:

```text
select/request Camera presentation
  -> framework resolves
      -> same physical Session Camera output
```

The exact number of rigs and final UI are evolutionary.

## 17.3 Audio — transversal contextual coverage

Basic Framework-owned BGM behavior should appear naturally in Game Flow where Route/Activity context already changes.

Candidate contextual coverage:

```text
Route BGM
Activity BGM
inherit
override
silence
restore according to context
```

This communicates that Audio intent participates in game lifecycle rather than existing only in a specialist scene.

Player or other samples may also use Audio naturally as Supporting/Ambient content when that does not distort dependency boundaries.

## 17.4 Audio ownership boundary — frozen

The Framework owns only its explicit integration semantics.

Broader provider behavior remains owned by the optional ecosystem packages.

```text
Framework
  Route / Activity lifecycle intent
  optional BGM integration boundary

com.immersive.audio
  Audio services and authoring
  SFX
  spatial SFX
  provider behavior

com.immersive.pooling
  pooling mechanics used by Audio where applicable
```

Do not present provider-owned behavior as Framework-owned functionality.

## 17.5 Audio dependency boundary — frozen

`com.immersive.audio` remains optional.

Do not make it a mandatory Framework dependency merely to simplify the sample catalog.

Do not silently install it, silently fall back, or hide a missing dependency.

If a sample or advanced context requires Audio:

```text
dependency requirement
  must be explicit

missing dependency
  must be explicit/diagnostic
```

Game Flow/Player should not accidentally become unusable without Audio solely because Audio was added for presentation.

The final packaging strategy for optional Audio usage remains **OPEN** and must preserve this rule.

## 17.6 Advanced Context — frozen structural role

Camera and Audio do not require large independent UPM families by default.

Instead, the sample program reserves one compact **Advanced Context** for transversal behaviors that do not arise naturally in Getting Started, Game Flow or Player.

Initial shape:

```text
ADVANCED CONTEXT
└── one Demonstration Application
    ├── HUB / Menu
    ├── focused Camera Switching Scenario
    └── Audio / Ecosystem advanced Scenarios as useful
```

The exact display name is **OPEN**.

The exact Activity/Scenario count is **EVOLUTIONARY**.

## 17.7 Advanced Audio candidates

Possible advanced/provider-focused behaviors include:

```text
direct SFX
spatial SFX
pooled SFX
provider-specific BGM operations
other Audio-package capabilities useful for ecosystem demonstration
```

These are **CANDIDATES**, not commitments.

They should reuse the same small shared environments and require only the interaction necessary to hear/observe the behavior.

## 17.8 Shared strategy with Camera and Audio

Camera and Audio follow the same sample-design method:

```text
TRANSVERSAL COVERAGE
  demonstrate naturally in other sample contexts

FOCUSED ADVANCED SCENARIO
  isolate behavior unlikely to appear naturally

NO COMBINATORIAL MATRIX
  do not test every model/scope/provider combination

SHARED CONTENT
  reuse environments and presentation assets

MINIMAL GAMEPLAY
  only enough input/UI to observe or request the behavior

EVOLUTIONARY SCENARIO LIST
  exact examples emerge during construction
```

## 17.9 Camera/Audio coverage ledger — frozen convention

Maintain an internal coverage ledger while implementing samples.

Example dimensions:

```text
Camera presentation:
  Fixed
  Follow
  Mounted
  Third Person

Camera request scope:
  Session
  Route
  Activity
  eligible Local Player

Audio:
  Route BGM
  Activity BGM
  inherit
  override
  silence
  optional provider/ecosystem behavior
```

The ledger is an implementation/planning aid, not a consumer-facing sample requirement.

Before creating another dedicated Camera/Audio Scenario:

```text
1. identify the uncovered concept
2. ask whether it fits naturally in an existing sample
3. add a focused Scenario only if the concept remains unclear
```

## 17.10 Frozen Camera + Audio rule

> **Camera and Audio should be visible as transversal concerns across the sample program. Contextual/basic behavior belongs where the lifecycle naturally demonstrates it; only behaviors that remain unclear deserve focused advanced Scenarios.**

---

# 18. Persistence

Status of this section: **FROZEN INITIAL BASELINE / TECHNICAL PRESENTATION — 2026-08-16**

Persistence is intentionally more technical than Getting Started, Game Flow or Player.

The sample does not need to disguise Preferences or Save operations as gameplay. UI is the correct consumer surface when UI is how a real project would configure or observe the behavior.

Initial scope:

```text
PERSISTENCE
└── one Demonstration Application
    ├── HUB / Menu
    ├── Preferences
    └── Progression Save
```

## 18.1 One Demonstration Application — frozen

Preferences and Progression Save initially share one coherent Demonstration Application.

Create another application only if implementation later reveals a real application-scoped incompatibility.

## 18.2 Preferences — frozen intent

Preferences is a UI-first demonstration.

Canonical consumer flow:

```text
open Preferences
  -> change values
  -> apply/persist
  -> leave/restart
  -> return
  -> persisted values are restored
```

No gameplay is required.

The exact preferences are evolutionary. Choose a small number of values that are cheap, visible and do not introduce unnecessary package dependencies.

Do not make an Audio-specific preference mandatory merely to showcase optional Audio integration.

## 18.3 Progression Save — frozen intent

Progression Save needs a small amount of meaningful state so the consumer can understand what is being persisted.

Canonical flow:

```text
initial progression state
  -> modify progression
  -> save
  -> restart/re-enter
  -> load/restore
  -> observe the same progression state
```

A minimal playful or visual context is encouraged only to give semantic meaning to the saved data.

Example candidates:

```text
progress level
unlocked area
small resource count
simple world-state marker
```

The sample is still fundamentally technical.

## 18.4 Progression UI — frozen boundary

Explicit controls such as these are acceptable:

```text
Advance Progress
Save
Load
Reset
```

The final labels and layout are evolutionary.

The purpose is to make persistence operations observable and inspectable, not to create artificial gameplay.

## 18.5 Snapshot — outside the initial baseline

Snapshot remains an architectural persistence domain but is not required in the first sample catalog.

```text
Snapshot
  not required for initial Persistence sample
```

It may be added later only if a distinct consumer use case justifies a canonical demonstration.

## 18.6 Backend independence — frozen boundary

Backend independence is an architectural capability, not a sample matrix.

The sample must **not** attempt to prove:

```text
third-party backend swapping
multiple backend implementations
backend comparison UI
cloud provider matrices
platform provider matrices
```

The Persistence sample proves normal project usage of Preferences and Progression Save.

Backend substitution remains documented architecture and may be technically validated elsewhere when needed.

## 18.7 Explicitly out of initial sample scope

```text
third-party save backends
backend switching showcase
cloud save
platform-specific save provider comparisons
migration matrices
corruption/recovery stress demonstrations
serialization stress demonstrations
Snapshot showcase
```

These exclusions do not remove architectural extensibility.

## 18.8 Diagnostics — frozen strategy

Persistence may expose more technical evidence than other samples.

Useful consumer-facing/debug evidence may include:

```text
current in-memory state
last persistence operation
operation result
persisted/restored state
relevant profile or identity
```

Only official/public diagnostic surfaces should be used.

Do not reach into runtime internals merely to make the sample look informative.

## 18.9 Presentation/content strategy

Reuse shared content.

A small farm-themed progression context remains a useful candidate because existing visual assets can give saved state meaning without requiring new gameplay systems.

Theme remains non-normative.

## 18.10 Frozen Persistence rule

> **Persistence samples prove how a consumer uses Preferences and Progression Save in a project. They do not attempt to prove every backend or persistence architecture possibility.**

---

# 19. Shared asset hierarchy

The sample program should use explicit sharing scopes rather than one generic `Shared` bucket.

```text
Samples~/
  Shared/                         global sample content

  <Group>/
    Shared/                       content shared inside one sample family

    <DemonstrationApplication>/
      Shared/                     content shared inside one GameApplication

      GameApplication.asset
      application-specific configuration
      Routes/
      Activities/
      Scenes/
```

## 19.1 `Samples~/Shared`

Shared by multiple top-level groups.

Typical candidates:

```text
generic meshes
textures
materials
animations
generic environment pieces
fonts
icons
generic UI visual primitives
generic VFX
truly generic sample-only helpers
```

Rule:

> Nothing here should assume which `GameApplication` is active.

## 19.2 `<Group>/Shared`

Shared by multiple Demonstration Applications inside one group **only after concrete reuse exists**.

This is an available promotion scope, not a folder that every group must pre-create.

For Player specifically, `FG-ADR-002` governs the stricter rule:

```text
no concrete reuse
  -> no Player/Shared ownership is established

concrete reuse across two or more Player Demonstration Applications
  -> promote the reusable content to Player/Shared
```

Reusable content must not define one application's authority.

## 19.3 `<DemonstrationApplication>/Shared`

Shared by multiple Routes/Activities/Scenes inside one Demonstration Application.

Example `Player/ManagerProvisioned/Shared`:

```text
Local Player Host prefab
provisioning UI
HUD used by several Activities
application-specific presentation prefabs
```

## 19.4 Promotion rule

Conceptual authoring rule:

```text
used by one scene/feature
  -> keep local

used by multiple scenes in one Demonstration Application
  -> DemonstrationApplication/Shared

used by multiple Demonstration Applications in one group
  -> Group/Shared

used by multiple sample groups during authoring
  -> _Sample/Shared
```

Promote only after reuse is real.

### 19.4.1 UPM import boundary correction — frozen in Revision 8

`_Sample/Shared` is an **authoring-time global source pool**, not a permitted hidden runtime dependency between final UPM samples.

The final Package Manager import unit is each top-level sample group declared in `package.json`.

Therefore:

```text
GettingStarted
GameFlow
Player
AdvancedContext
Persistence
```

must each be independently importable from its own final `Samples~/<Group>` subtree.

A final sample group must not reference:

```text
Samples~/Shared/
../Shared/
a sibling sample group
```

as required content.

If content is genuinely reused by multiple final UPM groups, choose explicitly:

```text
small presentation-only asset
  -> duplicate/materialize inside each required sample import root

official reusable product content
  -> promote to an appropriate package Runtime/Editor product surface

authoring-only convenience
  -> keep in _Sample/Shared while developing,
     but remove cross-group dependency before UPM finalization
```

Do not create a hidden sixth “Shared dependency sample” merely to avoid duplication.

Pedagogical/import isolation is more important than eliminating every duplicated presentation asset.

---

# 20. Authority must not hide in Shared

`Shared` means reusable content, not hidden application authority.

As a default, do not place authoritative configuration of one demonstration in a higher Shared scope merely because two examples look similar.

Typical application-owned assets include:

```text
GameApplicationAsset
PlayerSessionProfile
application-specific ProgressionSaveProfile
RouteAsset
ActivityAsset
application-specific Persistent Content references
application-specific policy/profile assets
bindings that only make sense for that demonstration
```

Some Profile assets may legitimately be shared when their semantic intent is genuinely reusable across applications, but this should be intentional rather than deduplication for its own sake.

Pedagogical clarity is more important than eliminating every duplicated ScriptableObject.

Rule of thumb:

```text
Shared = reusable content / reusable intent
Application folder = obvious ownership / application composition
```

---

# 21. Canonical implementation scaffold

Status: **FROZEN STRUCTURAL + AUTHORING BASELINE — REVISION 8 / 2026-08-16**

The package sample program should initially use five UPM Sample Groups:

```text
Getting Started
Game Flow
Player
Advanced Context
Persistence
```

## 21.0 Authoring tree — frozen

During active construction, the canonical visible root is:

```text
_Sample/
├── Shared/
│   ├── Art/
│   ├── Materials/
│   ├── UI/
│   ├── Input/
│   ├── VFX/
│   └── CommonSampleScripts/
│
├── GettingStarted/
│   ├── README.md
│   └── MinimalGame/
│       ├── README.md
│       ├── EXPECTED-ASSETS.md
│       ├── Shared/
│       ├── Routes/
│       ├── Activities/
│       └── Scenes/
│
├── GameFlow/
│   ├── README.md
│   ├── Shared/
│   └── GameFlowShowcase/
│       ├── README.md
│       ├── EXPECTED-ASSETS.md
│       ├── Shared/
│       ├── Routes/
│       ├── Activities/
│       └── Scenes/
│
├── Player/
│   ├── README.md
│   ├── ManagerProvisioned/       # next Player application
│   ├── CharacterSelection/       # planned / blocked
│   └── LocalMultiplayer/         # planned / blocked
│
├── AdvancedContext/
│   ├── README.md
│   ├── Shared/
│   └── Showcase/
│       ├── README.md
│       ├── EXPECTED-ASSETS.md
│       ├── Shared/
│       ├── Routes/
│       ├── Activities/
│       └── Scenes/
│
└── Persistence/
    ├── README.md
    ├── Shared/
    │   ├── UI/
    │   └── Presentation/
    └── PersistenceShowcase/
        ├── README.md
        ├── EXPECTED-ASSETS.md
        ├── Shared/
        ├── Routes/
        ├── Activities/
        └── Scenes/
```

This is the **authoring tree**. The `_Sample` name is intentional while samples are being built: it stays visible to Unity and is visually distinct from the final UPM `Samples~` root.

For Player, the tree is a current planning/materialization snapshot rather than a predetermined Player-application structural freeze. Getting Started owns canonical Scene-Provided coverage; `Player/Shared` is omitted from the required scaffold and may appear only after concrete reuse, as defined by FG-ADR-002.

## 21.0.1 Final UPM distribution tree — frozen

Before package release, the authoring root is converted to the official UPM distribution root:

```text
_Sample/
  -> Samples~/
```

The final importable roots are:

```text
Samples~/
├── GettingStarted/
├── GameFlow/
├── Player/
├── AdvancedContext/
└── Persistence/
```

The authoring-only top-level `_Sample/Shared` is **not** a final cross-sample dependency root.

Before finalization, every asset referenced by an importable group must either:

```text
live inside that group's own subtree
or
be a legitimate package-owned Runtime/Editor dependency
```

## 21.1 Why `EXPECTED-ASSETS.md` exists in the scaffold

The planning scaffold must not ship fake Unity `.asset`, `.prefab` or `.unity` files.

Those assets must be created through Unity so GUIDs, YAML references and serialized data are valid.

`EXPECTED-ASSETS.md` records the materialization target for each Demonstration Application.

Typical expected files include:

```text
GameApplication_<Application>.asset
PlayerSessionProfile_<Application>.asset        when the application uses Player Session
Route_<Name>.asset
Activity_<Name>.asset
SCN_<Name>.unity
application-local prefabs/profiles as required
```

Exact scenario asset names remain evolutionary.

## 21.2 Application root ownership

The `GameApplication` and application-defining profiles should remain obvious at the Demonstration Application level.

Do not bury them inside generic Shared folders.

A practical materialized application may therefore evolve toward:

```text
<DemonstrationApplication>/
  GameApplication_<Name>.asset
  PlayerSessionProfile_<Name>.asset   [when applicable]
  README.md
  Shared/
  Routes/
  Activities/
  Scenes/
```

## 21.3 UPM metadata baseline

The final UPM package should expose the five top-level groups through `package.json` `samples` metadata after the `_Sample/` -> `Samples~/` finalization transition.

The scaffold includes a proposed `package.samples.fragment.json` for manual integration.

Do not treat that fragment as an automatic package edit.

## 21.4 Structure is stable; Unity contents are evolutionary

Frozen:

```text
top-level sample groups
Demonstration Application ownership shape
Shared hierarchy
README convention
explicit GameApplication ownership
```

Evolutionary:

```text
exact Scene count
exact Route/Activity count
final scenario names
art/theme placement
exact prefabs
exact optional Audio composition
```

## 21.5 Canonical authoring workflow — frozen in Revision 8

The sample program uses two physical phases.

### Phase A — visible authoring

```text
planet-devourer / main
└── Assets/_Sample/
```

Purpose:

```text
visible in Unity Project Browser
normal Asset Database participation
normal Inspector editing
normal scene/prefab/ScriptableObject creation
normal authoring validation
normal Play Mode iteration
```

During this phase:

```text
package.json does not need to expose final Samples~ entries yet
```

A separate metadata fragment/document may be maintained until distribution finalization.

### Phase B — UPM distribution

At release/finalization:

```text
planet-devourer:main / Assets/_Sample/
  -> materialize/promote
com.immersive.framework / Samples~/
```

Then the package manifest declares each final importable group:

```json
"samples": [
  {
    "displayName": "...",
    "description": "...",
    "path": "Samples~/<Group>"
  }
]
```

This transition from the visible `Assets/_Sample/` authoring workspace to package-owned `Samples~/` is a **packaging/promotion transition**, not a reason to redesign sample behavior.

No scenario should be rewritten merely because it moves from the visible authoring workspace into the package `Samples~/` distribution root.

## 21.6 `.meta` and GUID rule — frozen validation boundary

During visible `_Sample/` authoring, Unity treats the content as normal Asset Database content and authoring metadata exists normally.

For final `Samples~` distribution, do **not** treat authoring-time `.meta` behavior as a Framework contract.

Unity documents tilde sample folders as ignored by the Asset Database and not tracked through `.meta` in the normal way.

Therefore the release rule is:

```text
do not assume authoring GUID behavior is sufficient proof
```

Final correctness is established by real consumer import.

The package finalization process must validate:

```text
Package Manager shows every declared sample
Import copies the selected group into Assets
imported scenes open
prefab references resolve
ScriptableObject references resolve
materials/assets resolve
no reference points to authoring-only _Sample/Shared
no reference points to a sibling sample group
Play Mode works from the imported copy
```

If metadata normalization is required by the Unity version/package workflow, handle it during finalization and validate the imported result.

The ADR intentionally does not freeze a manual “keep all `.meta`” or “delete all `.meta`” rule without import evidence.

## 21.7 Package Manager import is the release acceptance path

Editing successfully under `_Sample/` is not sufficient release proof.

Every top-level sample group must be tested as a consumer would use it:

```text
final package with Samples~/
  -> Package Manager
      -> Import <Group>
          -> copied under Assets/Samples/...
              -> inspect
              -> open
              -> Play
```

A sample is not release-ready if it only works while directly visible under package `_Sample/`.

## 21.8 No cross-import hidden dependencies

Because Package Manager imports the whole selected sample subfolder, each declared group must be self-contained relative to sample content.

Allowed:

```text
<Group>/Shared
  used by two or more concrete Demonstration Applications
  inside the same final import root
  only after reuse is real
```

For Player, `Player/Shared` is permitted only under the concrete-reuse rule in FG-ADR-002.

Allowed:

```text
GameFlow/Shared
  used by GameFlowShowcase
```

Not allowed:

```text
GettingStarted -> Samples~/Shared
GameFlow -> Samples~/Shared
Player -> GettingStarted/Shared
Persistence -> GameFlow/Shared
```

Cross-group reuse must be materialized explicitly before release.

## 21.9 Authoring workflow rule

> **Author visibly in `planet-devourer:main/Assets/_Sample/`; promote the mature official sample to `com.immersive.framework/Samples~/`; validate the final result through real Package Manager import.**

---

# 22. README convention

README files are short operational legends, not long tutorials.

Each top-level group README should contain:

```text
Purpose
What this group demonstrates
How Demonstration Applications are organized
How to activate a GameApplication when the group has more than one
Shared folder conventions
Which example to open first
```

Each Demonstration Application may have a very short local README or equivalent description:

```text
Demonstrates
Run
Observe
Inspect
Framework concepts
```

Example:

```text
Manager-Provisioned

Demonstrates
  Session-authorized Player provisioning.

Application
  GameApplication_ManagerProvisioned.asset

Run
  Set this GameApplication Active.
  Open the entry scene.
  Play.

Observe
  The Session provisions/adopts the Player through the official Manager-Provisioned path.

Inspect
  GameApplication
  PlayerSessionProfile
  provisioning authoring composition
  public Player request/observation surfaces
```

---

# 23. Visual continuity and reuse

Samples do not need to be visually unrelated.

Game Flow can reuse a compact environment vocabulary:

```text
Basic
  Route A / Route B

Intermediate
  same vocabulary + transition presentation

Advanced
  same vocabulary + loading/readiness/diagnostics
```

Player can progressively evolve game-language:

```text
direct entry
  -> character selection
      -> multiple joining Slots
          -> Activity participation
              -> lifetime / placement
                  -> Pause / Leave / Rejoin
```

The character-selection screen can become a reusable game-language element in later Player demonstrations rather than being rebuilt as diagnostic UI every time.

Existing graphical assets from the previous attempt may be reused, provided previous technical setup is not inherited as architectural authority.

---

# 24. Runtime self-explanation

A demonstration should be understandable in Play Mode without requiring the README to explain the basic observable behavior.

Small sample-only HUDs may expose relevant state such as:

```text
Current Route
Current Activity
Current Player provisioning origin
Current Player Slot
Physical Player identity
Activity representation state
Current Camera presentation
Current Camera request winner
Transition phase
Loading / readiness state
Current BGM policy / confirmed state
```

Such visualization is demonstrative, not runtime authority.

Game-like presentation is preferred over purely technical buttons when a familiar game interaction can represent the concept naturally.

Example:

```text
character-selection screen
```

is preferable to:

```text
[Request Actor Selection]
```

when the intent is to demonstrate how a real game consumes the Player API.

Advanced/Debug evidence can still expose the technical facts underneath.

---

# 25. Transversal coverage ledger

Camera and Audio remain transversal even without dedicated basic groups.

Maintain a lightweight implementation ledger rather than a combinatorial test matrix.

Initial Camera dimensions:

```text
Presentation
  Fixed
  Follow
  Mounted
  Third Person

Request scope
  Session
  Route
  Activity
  eligible Local Player

Special gap
  same-Activity runtime presentation switching
```

Initial Audio dimensions:

```text
Route BGM
Activity BGM
inherit
override
silence
restore
optional provider/ecosystem behavior
```

Expected natural homes:

```text
Getting Started
  Mounted / First Person

Game Flow
  contextual Route/Activity Camera
  contextual Route/Activity BGM

Player
  Player-natural Camera models/scopes
  optional supporting Audio when dependency-safe

Advanced Context
  same-Activity Camera switching
  advanced optional Audio/provider cases
```

The ledger is a planning aid. QAFramework remains responsible for technical contract coverage.

---

# 26. Frozen baseline catalog — Revision 11

This is the initial implementation catalog.

```text
GETTING STARTED
  Minimal Game
    Scene-Provided Player
    Mounted / First Person
    one Route
    one Activity
    navigation only

GAME FLOW
  one Demonstration Application
  Sample HUB / Menu
  evolutionary Scenarios:
    Route / Activity
    Content / Visibility
    Transition
    Loading / Readiness
    Restart / Recovery
  natural contextual Camera + BGM coverage

PLAYER
  specific scope delegated to FG-ADR-002

  canonical Scene-Provided coverage:
    Getting Started / Minimal Game

  current Player direction:
    Manager-Provisioned
      next Player Demonstration Application

    Character Selection
      planned / blocked by public arbitrary Actor-selection surface

    Local Multiplayer
      planned / blocked by public Slot/device/input contract

  Player/Shared
    only after concrete reuse

ADVANCED CONTEXT
  one Demonstration Application
  Sample HUB / Menu
  focused same-Activity Camera Switching
  advanced optional Audio/ecosystem behavior as useful

PERSISTENCE
  one Demonstration Application
  technical HUB / Menu
  Preferences
  Progression Save
  Snapshot not required initially
  backend swapping not demonstrated
```

This catalog is intentionally smaller than the ADR inventory.

---

# 27. Consolidated decisions

1. **Getting Started / Minimal Game is frozen.** It uses one Scene-Provided Player, first-person `Mounted` Camera presentation, one Route, one Activity, minimal movement/look Input and a reasonably populated environment. Navigation is the complete gameplay requirement.

2. **Camera is transversal by default.** Different demonstrations should use different Camera models naturally; a dedicated Camera group is optional and only justified by uncovered concepts.

3. **Player needs Demonstration Applications, not merely one scene per concept.** Incompatible Session/application initialization intent belongs in separate application folders.

4. **A UPM Sample Group may contain multiple Demonstration Applications.** UPM import unit, `GameApplication`, and scene are different structural concepts.

5. **Multiple GameApplications in samples are a presentation need, not a Framework architecture defect.** Real games normally choose one Active Game Application.

6. **GameApplication switching should remain explicit.** Use the official `Set Active` path and explain it in Getting Started/group README rather than hiding it with sample magic.

7. **Character Selection remains a valuable planned game-owned Player demonstration surface, but it is currently blocked.** The sample must wait for a sufficient public arbitrary Actor-selection surface rather than bypass the Framework with sample-owned internals.

8. **Three Shared scopes are useful:** global sample, group, and Demonstration Application.

9. **Authority should remain visually local to its owning demonstration.** Do not hide application-defining assets in generic Shared folders for deduplication alone.

10. **Audio should also demonstrate ecosystem composition.** Framework BGM integration remains Framework-owned; broader Audio and Pooling capabilities remain owned by their respective packages.

11. **Samples do not need gameplay by default.** Gameplay exists only when necessary to make the Framework concept observable or to demonstrate realistic consumption of the public API.

12. **FIRSTGAME is not an “everything sample”.** It is a real game proving Framework applicability. It should use only the Framework systems that the game actually needs, without distorting gameplay for feature coverage.

13. **Consumers may enter at different depths.** Less experienced or more careful developers can follow focused samples progressively; experienced developers may inspect FIRSTGAME directly as an integrated real-game reference.

14. **Decision status is explicit.** `FROZEN / CONVENTION` governs structure; `GUIDELINE` expresses preference; `CANDIDATE` records an implementation idea; `OPEN` preserves intentional uncertainty.

15. **Structure is frozen before content.** New scenario ideas are expected during implementation and should not reopen structural conventions unless they reveal a genuine contradiction.

16. **Sample HUB/Menu is an official sample convention.** Multiple topics inside one Demonstration Application may use a HUB for fast selection. The HUB is navigation only and never runtime authority or gameplay progression.

17. **Game Flow structure is frozen around one initial Demonstration Application.** Compatible Flow topics are Scenarios selected from a HUB. More GameApplications are added only if real application-scoped incompatibility is discovered.

18. **Presentation content should remain compact.** Samples should reuse shared assets aggressively; creating unique art for each technical concept is discouraged.

19. **Farm-game assets are a current content candidate, not a structural requirement.** The visual library may incorporate other owned-game content when useful without changing the sample architecture.

20. **Player-specific sample scope is delegated to FG-ADR-002.** FG-ADR-001 no longer freezes a fixed Player Demonstration Application catalog.

21. **Getting Started / Minimal Game is the canonical Scene-Provided Player reference.** Do not duplicate it as a dedicated Scene-Provided application under Player without new consumer-contract evidence.

22. **Manager-Provisioned is the next Player Demonstration Application.** It is the next distinct provisioning/application path to materialize after the canonical Scene-Provided coverage.

23. **Character Selection is planned but blocked by the public arbitrary Actor-selection surface.** The sample must not invent a private workaround.

24. **Local Multiplayer is planned but blocked by the public Slot/device/input contract.** The sample must not create parallel sample-owned Slot/device/input authority.

25. **Player capabilities are Scenarios by default and the combinatorial matrix remains out of scope.** Representative canonical uses belong in Samples; exhaustive combinations belong in QAFramework.

26. **`Player/Shared` is conditional, not preallocated.** Introduce/promote it only after concrete reuse across Player Demonstration Applications; application authority stays local.

27. **Camera and Audio are transversal concerns.** They should be visible naturally across Getting Started, Game Flow, Player and Advanced Context rather than appearing to belong only to specialist demonstrations.

28. **Game Flow is the primary home for contextual Camera and BGM behavior.** Route/Activity-scoped Camera and Route/Activity BGM should be demonstrated there when natural.

29. **Camera requires one focused same-context switching Scenario.** It exists to demonstrate changing Camera presentation/request without changing Route or Activity while preserving one physical Session output.

30. **Advanced Context is a compact gap-filling application, not a feature warehouse.** It hosts focused transversal behaviors that do not fit naturally elsewhere.

31. **Audio remains optional.** Transversal Audio use must not silently turn unrelated samples or the Framework core into mandatory Audio consumers.

32. **Provider-owned Audio behavior stays provider-owned.** SFX, spatial SFX and pooling integration may be demonstrated, but must be labeled as Audio/Pooling capabilities rather than Framework lifecycle authority.

33. **Maintain a Camera/Audio coverage ledger.** Add focused Scenarios only after checking whether an uncovered concept can be demonstrated naturally in an existing sample.

34. **Persistence starts with one Demonstration Application.** Preferences and Progression Save share the initial Persistence application.

35. **Preferences is UI-first.** No gameplay wrapper is required to prove persistent user/project preferences.

36. **Progression Save proves normal consumer usage.** A small meaningful state and minimal visual context are sufficient.

37. **Snapshot is not required in the initial sample baseline.**

38. **Backend independence is not a backend-swapping sample.** Third-party/cloud/platform backend matrices remain outside initial sample scope.

39. **Persistence may be more technically self-explanatory.** Current state, last operation and restoration evidence are appropriate when exposed through official surfaces.

40. **The initial UPM sample program has five groups:** Getting Started, Game Flow, Player, Advanced Context and Persistence.

41. **The canonical scaffold must not contain fake Unity serialized assets.** Unity creates `.asset`, `.prefab` and `.unity` files; the planning scaffold records expected materialization instead.

42. **FIRSTGAME must not copy the Samples taxonomy.** It may reuse ownership, Shared and explicit-authority conventions while remaining organized as a real game.

43. **Samples are currently authored visibly under `planet-devourer:main/Assets/_Sample/`.** This is the canonical provisional development workspace so scenes, prefabs, ScriptableObjects and authoring components can be edited normally in Unity while remaining distinct from final UPM packaging.

44. **`com.immersive.framework/Samples~/` is the final UPM distribution shape.** Promotion from `planet-devourer:main/Assets/_Sample/` happens as an explicit package-finalization step; `_Sample/` must not be shipped as the final UPM sample root.

45. **Package Manager import is mandatory release validation.** A sample is not considered complete merely because it works while directly authored under `_Sample/`.

46. **Top-level global Shared is authoring-only unless independently packaged.** Final importable groups must not depend on a sibling `Samples~/Shared` tree.

47. **Cross-group presentation reuse may be duplicated deliberately.** Sample import isolation and pedagogical clarity take precedence over eliminating small duplicated assets.

48. **Shared product functionality should move to the package, not to a hidden sample dependency.** Only genuinely official reusable content belongs in Runtime/Editor package surfaces.

49. **Do not freeze an unsupported `.meta` migration assumption.** Authoring metadata exists under visible `_Sample/`, while final correctness is proven from the imported UPM sample under `Assets`.

50. **The final `package.json` sample entries point to `Samples~/<Group>`.** During authoring, a fragment may be maintained until the package finalization is performed.

51. **This ADR has one stable canonical filename.** Revision identifiers live in the document header; Git history preserves prior revisions.

52. **The current `planet-devourer:main` tree is the provisional operational baseline.** Older ZIP scaffolds do not override the current Git structure.

53. **`Assets/_Project/` and `Assets/_Sample/` serve different purposes.** `_Project` is the future real-game structure; `_Sample` is the current sample authoring workspace.

54. **Current authoring location does not change final ownership.** Mature official samples are promoted to `com.immersive.framework/Samples~/`.

---

# 28. Frozen ADR-review discoveries that remain valid

1. **Game Flow is broader than Route and Activity alone.** Transition, Loading, Readiness, visibility and restart/recovery are advanced parts of the same user problem.

2. **Player is the largest current sample domain.** The ADRs distinguish Session configuration, Slots, Joining, Actor selection, provisioning origin, Activity participation, physical lifetime, Initial Placement, public commands/observation, Input/Pause and Leave.

3. **Scene-Provided and Manager-Provisioned are peer origins before admission.** After successful admission both converge on Session ownership of the physical Player.

4. **Activity representation and physical Player lifetime are separate.** Activity transitions should demonstrate contextual replacement while preserving the same physical Player instance when appropriate.

5. **Initial Placement is not generic spawn.** Ordinary Activity changes preserve current Player pose unless explicit placement intent establishes a new spatial starting decision.

6. **IF-ADR-005 is deliberately cross-cutting.** Input/Pause fits Player; Activity Restart and Transition Gate fit Game Flow; Object/Group Reset is a general gameplay utility.

7. **Camera Presentation and Camera output authority remain distinct mental models even if demonstrated transversally.**

8. **Framework Audio at the ADR boundary remains optional BGM integration.** Provider-specific features must be labeled as provider-owned.

9. **Persistence is a real product family whose initial sample baseline is deliberately narrow: Preferences + Progression Save.**

10. **Authoring and Inspector standards are transversal.** Samples are one of the best ways to evaluate IF-ADR-002 / IF-ADR-010 product quality.

11. **Identity is transversal technical truth.** Stable IDs, authored definitions and runtime occurrences should be correctly exposed/diagnosed without becoming artificial identity demos by default.

12. **A sample must not hide product friction with helper code.** Difficulty in building a clean demonstration is evidence for product review.

13. **The sample program is itself a product-quality investigation.** Mature product improvements belong back in `com.immersive.framework`; technical contracts remain validated by QAFramework; real-game usability remains proven by FIRSTGAME.

---

# 29. Implementation evidence behind Revision 2/3/4/5/6/7

The implementation-backed structural decisions introduced in Revision 2 and preserved through Revision 7 were checked against package implementation and the framework repository state used to close this baseline.

Relevant surfaces:

```text
Runtime/Authoring/GameApplicationAsset.cs
  PlayerSessionEnabled
  DefaultPlayerSessionProfile
  PlayerActorSelectionDuplicatePolicy
  PersistentContent

Runtime/PlayerParticipation/Authoring/PlayerSessionProfile.cs
  SupportedSlots
  InitialJoiningOpen
  HostProvisioning
  ActorResolutionPolicy

Runtime/Authoring/ImmersiveFrameworkSettingsAsset.cs
  ActiveGameApplication

Editor/Settings/ImmersiveFrameworkSettingsProvider.cs
  Active Game Application project surface

Editor/Authoring/GameApplicationAssetEditor.cs
  Active / Inactive status
  Set Active

Runtime/Bootstrap/ImmersiveFrameworkBootstrap.cs
  resolves project settings and boots the active GameApplication
```

Repository baseline verified for closure: `919722cfb66434abace1661822aea9b85a138e9b` (`ADR22-Complete`, 2026-08-15).

Current dependency shape checked during this revision:

```text
com.immersive.framework 1.0.0-preview.17
  com.immersive.foundation 0.2.0
  com.immersive.logging 0.2.1
  com.unity.cinemachine 3.1.0
  com.unity.inputsystem 1.19.0

com.immersive.audio 0.2.0
  com.immersive.pooling 0.2.0
```

The official ADRs and repository state remain the source of truth if implementation later changes.

---

# 29.1 Frozen sample-structure summary

As of Revision 11:

```text
UPM Sample Group
  broad consumer problem

Demonstration Application
  coherent GameApplication/application-session archetype

Scenario
  executable concept inside that application

Sample HUB / Menu
  optional navigation when several Scenarios share one application

Player
  concrete scope delegated to FG-ADR-002
  Getting Started / Minimal Game = canonical Scene-Provided coverage
  Manager-Provisioned = next Player application
  Character Selection = planned / blocked on public Actor selection
  Local Multiplayer = planned / blocked on public Slot/device/input
  Player/Shared only after concrete reuse
  no silent global GameApplication switching

Camera + Audio
  transversal across existing contexts
  Game Flow carries natural contextual Camera/BGM cases
  Advanced Context fills remaining gaps
  focused same-Activity Camera switching is required
  Audio remains optional and provider boundaries remain explicit

Persistence
  one Demonstration Application
  Preferences + Progression Save
  technical/UI-first presentation is acceptable
  Snapshot deferred
  backend swapping outside sample scope

Canonical scaffold
  five initial UPM sample groups
  explicit Demonstration Application ownership
  no fake Unity serialized assets in planning scaffold

Shared
  Global
  Group
  Demonstration Application

Gameplay
  only when needed to expose the Framework concept

Presentation assets
  reuse aggressively
  content/theme remains evolutionary

FIRSTGAME
  proves a real game
  not feature-complete by requirement
```

The catalog may evolve without changing this grammar.

# 30. Implementation decisions intentionally evolutionary

The strategy is closed even though implementation details remain intentionally open.

```text
final UPM display names/descriptions
exact metadata normalization mechanics proven during final Package Manager import
final Game Flow Scenario catalog and naming
final Scenario allocation inside materialized Player Demonstration Applications
future Player application additions only when FG-ADR-002 criteria and public surfaces justify them
final Camera presentation/request distribution
final visual form of HUB/Menu surfaces
final display name and exact Scenario catalog for Advanced Context
final optional com.immersive.audio packaging/install experience
exact Preferences demonstrated
exact Progression state demonstrated
whether Snapshot later earns a canonical sample
exact Scene/Route/Activity counts
final visual/art direction
which existing graphical assets are retained
```

These decisions may evolve without reopening the frozen structural grammar.

---

# 31. Implementation handoff

The planning phase is complete enough to begin implementation and repository reorganization work.

Recommended package implementation order:

```text
1. Create/use the visible Assets/_Sample/ authoring scaffold.
2. Materialize Getting Started / Minimal Game.
3. Build Game Flow around one Demonstration Application + HUB.
4. Follow FG-ADR-002 for Player: build Manager-Provisioned next; do not implement Character Selection or Local Multiplayer until their public-surface blockers are resolved.
5. Add Advanced Context only for transversal gaps.
6. Build Persistence / Preferences + Progression Save.
7. Audit Camera/Audio coverage.
8. Resolve every cross-group _Sample/Shared dependency.
9. Convert _Sample/ -> Samples~/ for UPM distribution.
10. Add/adjust package.json samples metadata.
11. Import each group through Package Manager into a clean consumer project.
12. Validate references, Play Mode and independence from sibling sample groups.
13. Use QAFramework for technical contract validation where sample implementation reveals contract risk.
```

Every sample implementation cut should still record:

```text
objective
scope
out of scope
type
files created/changed/removed
product surface affected
expected user flow
technical smoke
technical acceptance
product acceptance
architectural gain
usability gain
suggested commit message
```

---

# 32. FIRSTGAME reorganization boundary

FIRSTGAME is a real consumer and must not become a mirror of `Samples~`.

Do **not** reorganize FIRSTGAME as:

```text
GettingStarted/
GameFlow/
Player/
AdvancedContext/
Persistence/
```

Those folders classify documentation/demonstration problems, not game-domain ownership.

FIRSTGAME may safely adopt the structural lessons:

```text
one obvious active GameApplication
explicit Player Session configuration
clear Route / Activity ownership
authoritative assets close to the composition that owns them
Shared only after real reuse
technical materialization remains inspectable
Camera/Audio treated as transversal game composition
Persistence integrated where the game actually needs it
```

Canonical rule:

> **Samples organize concepts for learning; FIRSTGAME organizes a real game while consuming those concepts correctly.**

A concrete FIRSTGAME folder migration should be designed from the current `planet-devourer` repository state, not inferred from the Samples tree.

---

## Revision 9 naming decision — historical basis

Revision 9 changes only the **authoring workspace name**.

Frozen rule:

```text
Assets/_Sample/
  current visible authoring workspace
  normal Unity Asset Database participation
  normal Project Browser / Inspector workflow
  not the final UPM distribution root

Samples~/
  final UPM package sample root
  declared by package.json samples metadata
  validated through Package Manager import
```

Why `_Sample` instead of `Samples`:

```text
clear visual distinction from final UPM Samples~
easy to identify as temporary authoring workspace
avoids treating visible authoring layout as the shipped package layout
```

This is an Immersive Framework repository convention, not a Unity UPM requirement.

The structural sample taxonomy is unchanged.

---

## Revision 10 repository and filename decision

Revision 10 adds two maintenance decisions:

```text
1. ADR filename is stable.
   Revision lives inside the file.

2. Current visible authoring truth is the FirstGame branch:
   planet-devourer/Assets/_Sample/
```

This supersedes any wording that implies `_Sample/` must physically live at the root of `com.immersive.framework` during active authoring.

Final official UPM ownership remains:

```text
com.immersive.framework/Samples~/
```

---

## Revision 11 Player-scope delegation decision

Revision 11 resolves the Player sample contradiction introduced by the earlier predetermined Player-application baseline.

Canonical relationship:

```text
FG-ADR-001
  general sample-program grammar
  UPM grouping / authoring / distribution
  Demonstration Application vs Scenario rules

FG-ADR-002
  Player-specific sample scope
  canonical Scene-Provided coverage
  Player application sequence
  public-surface blockers
  Player/Shared promotion rule
```

The previous fixed Player-application freeze is superseded.

Current canonical Player direction is:

```text
Getting Started / Minimal Game
  canonical Scene-Provided reference

Player / Manager-Provisioned
  next Player Demonstration Application

Character Selection
  planned / blocked by public arbitrary Actor selection

Local Multiplayer
  planned / blocked by public Slot/device/input

Player/Shared
  only after concrete reuse
```

## Unity packaging evidence used by Revision 8

Revision 8 uses the Unity package-sample rules documented by the Unity Manual:

```text
Samples~ is the official package sample root.
Each subfolder of Samples~ represents a sample.
The tilde tells Unity to ignore that folder in normal Asset Database tracking.
package.json samples entries use paths starting with Samples~/...
Package Manager Import copies the selected sample subfolder structure into the consumer project's Assets folder.
```

Revision 8 adds the visible `_Sample/` phase as an **Immersive Framework authoring convention** so the team can create and inspect Unity assets normally before UPM finalization.

The authoring convention does not replace the official final `Samples~/` package layout.

# 33. Closure

Revision 11 keeps the general sample-program baseline frozen, records `planet-devourer:main/Assets/_Sample/` as the current operational authoring baseline, and delegates the concrete Player sample architecture to FG-ADR-002.

Frozen baseline:

```text
5 UPM Sample Groups
  Getting Started
  Game Flow
  Player
  Advanced Context
  Persistence

Getting Started
  one Minimal Game

Game Flow
  one Demonstration Application + HUB

Player
  concrete scope delegated to FG-ADR-002
  Getting Started / Minimal Game = canonical Scene-Provided coverage
  Manager-Provisioned = next Player application
  Character Selection = planned / blocked
  Local Multiplayer = planned / blocked

Camera + Audio
  transversal
  Advanced Context only for uncovered/advanced behavior

Persistence
  one Demonstration Application
  Preferences + Progression Save

Shared
  global / group / application conceptual scopes
  top-level global Shared is authoring-only as a cross-group pool
  final UPM groups are independently importable

Authoring / distribution
  Assets/_Sample/ while building
  Samples~/ for final UPM packaging
  Package Manager import is release acceptance

FIRSTGAME
  real-game proof
  not a copy of the Samples taxonomy
```

Future implementation discoveries may evolve scenarios and content. Structural changes require concrete evidence that the frozen baseline is insufficient or contradictory.
