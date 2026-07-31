# M04 — Content Anchors

Status: **Passed — FIRSTGAME happy path validated**  
Roadmap order: 4  
Type: **UX/product + real integration**

## Objective

Demonstrar que objetos existentes em cenas reais podem declarar localizações semânticas passivas com ownership explícito de `Route` e `Activity`, usando as superfícies oficiais do framework.

O modelo prova:

```text
manual authoring
→ explicit ownership
→ passive discovery
→ Activity scene replacement
→ re-entry without duplicate accumulation
```

## Scope validated

```text
RouteContentAnchor:
  Root / Required

ActivityContentAnchor — Activity A:
  Root / Required
  Slot / Required
  Point / Optional

ActivityContentAnchor — Activity B:
  Root / Required
  Slot / Required
  Point / Optional

Runtime flow:
  Boot → Activity A → Activity B → Activity A
```

## Out of scope

```text
Local Content Anchor authoring
Runtime Content binding
anchor materialization
object instantiation or movement
required-anchor lifecycle blocking
negative mismatch or duplicate cases
automatic component installation
automatic asset or UnityEvent configuration
```

`ContentAnchorScope.Local` exists in the contract, but no public scene-authoring component for Local ownership was available during this model. The M04 therefore does not invent or simulate a Local authoring path.

## Assets

```text
Application/
  GA_M04_ContentAnchors.asset

Routes/
  Route_M04_ContentAnchors.asset

Activities/
  Activity_M04_A.asset
  Activity_M04_B.asset

Profiles/
  ActivityContent_M04_A.asset
  ActivityContent_M04_B.asset
```

Final reference graph:

```text
GA_M04_ContentAnchors
├── Startup Route → Route_M04_ContentAnchors
└── Persistent Content → Shared/Scenes/Shared_PersistentContent

Route_M04_ContentAnchors
├── Primary Scene → M04_Route
└── First Activity → Activity_M04_A

Activity_M04_A
└── Content Profile → ActivityContent_M04_A
    └── Required Additive Scene → M04_ActivityA_Add

Activity_M04_B
└── Content Profile → ActivityContent_M04_B
    └── Required Additive Scene → M04_ActivityB_Add
```

## Scenes

```text
Scenes/M04_Boot.unity
Scenes/M04_Route.unity
Scenes/M04_ActivityA_Add.unity
Scenes/M04_ActivityB_Add.unity
```

Shared infrastructure:

```text
../Shared/Scenes/Shared_PersistentContent.unity
```

The shared Persistent Content scene provides the camera output, EventSystem, loading surface, transition surface and pause surface. M04 does not duplicate these services.

## Manual authoring procedure

The visual scaffold may create placeholder geometry and neutral mount points only.

Framework components, asset references, IDs and UnityEvents are configured manually in the Inspector. This preserves the FIRSTGAME authoring test: the model must expose the real setup experience rather than install the contract automatically.

### Route anchor

Scene:

```text
M04_Route
└── M04 Manual Authoring Visuals
    └── Route Root Candidate
        └── Framework Component Mount (Add Manually)
```

Configuration:

```text
Component:
  RouteContentAnchor

Owner Route:
  Route_M04_ContentAnchors

Anchor ID:
  m04.route.root

Kind:
  Root

Requiredness:
  Required
```

### Activity A anchors

```text
m04.activity-a.root   Root   Required
m04.activity-a.slot   Slot   Required
m04.activity-a.point  Point  Optional
```

All three declarations are owned by `Activity_M04_A`.

### Activity B anchors

```text
m04.activity-b.root   Root   Required
m04.activity-b.slot   Slot   Required
m04.activity-b.point  Point  Optional
```

All three declarations are owned by `Activity_M04_B`.

## Navigation surface

The model reuses the shared `PF_ActivityNavigation` menu so the two Activity scenes use the same UI composition.

Activity A override:

```text
Target Activity:
  Activity_M04_B

Reason:
  m04.activity-a-to-b

Button:
  ActivityRequestTrigger.RequestActivity()
```

Activity B override:

```text
Target Activity:
  Activity_M04_A

Reason:
  m04.activity-b-to-a

Button:
  ActivityRequestTrigger.RequestActivity()
```

The triggers belong to the Activity-owned additive scenes. They are not placed in the Route primary scene because runtime binding scans the materialized Activity scene roots.

## Expected Play Mode flow

```text
M04_Boot
→ replaced by M04_Route
→ Shared_PersistentContent loaded
→ M04_ActivityA_Add loaded
→ Route anchor discovered
→ Activity A anchors discovered

Activity A → Activity B
→ M04_ActivityB_Add loaded
→ M04_ActivityA_Add released
→ Activity B anchors discovered

Activity B → Activity A
→ M04_ActivityA_Add loaded again
→ M04_ActivityB_Add released
→ Activity A anchors rediscovered without accumulation
```

## Runtime evidence

### Boot

```text
Boot:
  Succeeded
  blockingIssues = 0

Route discovery:
  candidates = 1
  accepted = 1
  anchors = 1
  issues = 0
  invalidAuthoring = 0
  routeMismatch = 0
  duplicateIdentity = 0
  duplicateAnchorId = 0

Activity A discovery:
  candidates = 3
  anchors = 3
  discoverySceneRoots = 1
  issues = 0
  invalidAuthoring = 0
  activityMismatch = 0

Activity readiness:
  Ready
```

### Activity A → B

```text
Activity Request:
  Succeeded

Scene composition:
  loaded = 1
  failed = 0
  blockingIssues = 0

Previous scene release:
  released = 1
  failed = 0
  blockingIssues = 0

Activity B discovery:
  candidates = 3
  anchors = 3
  issues = 0
  invalidAuthoring = 0
  activityMismatch = 0

Transition terminal:
  CommittedReady
```

### Activity B → A

```text
Activity Request:
  Succeeded

Scene composition:
  loaded = 1
  failed = 0
  blockingIssues = 0

Previous scene release:
  released = 1
  failed = 0
  blockingIssues = 0

Activity A re-entry:
  candidates = 3
  anchors = 3
  issues = 0
  invalidAuthoring = 0
  activityMismatch = 0

Scene ledger:
  loaded = 1
  released = 1
  stale = 0

Transition terminal:
  CommittedReady
```

`contentAnchorBindings = 0` and `activityContentHandles = 0` are expected. This model proves passive declarations and discovery, not binding or materialization.

## Acceptance

### Technical

- [x] Game Application boot succeeded.
- [x] Route primary scene and shared Persistent Content loaded.
- [x] Route anchor discovered as `1/1`.
- [x] Activity A anchors discovered as `3/3`.
- [x] Activity B anchors discovered as `3/3`.
- [x] Activity A scene released during A → B.
- [x] Activity B scene released during B → A.
- [x] Activity A re-entered with exactly three anchors.
- [x] No owner mismatch.
- [x] No invalid authoring.
- [x] No duplicate identity or duplicate Anchor ID.
- [x] No stale Activity scene ledger entry.
- [x] Final Activity readiness remained `Ready`.
- [x] No blocking issue.

### Product

- [x] User can configure the complete asset graph through official Inspectors.
- [x] User can add and configure Route and Activity anchors manually.
- [x] Anchor ownership, Kind and Requiredness are explicit.
- [x] Content Anchor Inspectors follow the framework authoring organization.
- [x] Validation is explicit and non-mutating.
- [x] Advanced diagnostics expose canonical declaration evidence.
- [x] Shared menu composition avoids divergent one-off navigation UI.
- [x] The model can be understood and exercised without opening framework code.

## Findings

### Resolved during M04

1. **Content Anchor Inspectors were below the frozen framework authoring standard.**  
   The package received product header, intent summary, organized authoring, explicit identity suggestion, configuration status, explicit validation and Advanced / Debug. QA authoring smoke passed all ten cases.

2. **The first visual scaffold duplicated older authored visual content.**  
   The obsolete `Authored Visual Content` branches were removed. Each scene now uses one `M04 Manual Authoring Visuals` representation.

3. **The Route anchor was initially installed on an older technical mount.**  
   It was moved to the mount associated with the visible Route Root candidate.

4. **Navigation placeholders were initially Route-scoped.**  
   They were removed. `ActivityRequestTrigger` is now authored in each Activity-owned scene, matching the runtime binding scope.

5. **M04 initially planned a dedicated Persistent Content scene.**  
   The model now reuses `Shared_PersistentContent`, avoiding duplicate camera, EventSystem and presentation adapters.

### Remaining product follow-ups

1. **Suggested Anchor IDs are deterministic but too dependent on technical hierarchy names.**  
   M04 uses short explicit IDs such as `m04.activity-a.root`. The package suggestion policy should evolve toward designer-facing semantic identity.

2. **Local ownership lacks a public scene-authoring component.**  
   A future package cut must define the intended Local authoring surface before FIRSTGAME or QA can validate it.

3. **Required Content Anchors are diagnostic intent only.**  
   They do not currently block Route or Activity lifecycle. Inspector copy must continue to state this explicitly.

## QA follow-ups

Negative technical scenarios belong to `QAFramework`, not this FIRSTGAME model:

```text
Route owner mismatch
Activity owner mismatch
duplicate canonical identity
duplicate Anchor ID
invalid declaration
invalid Kind
binding rejection
cleanup regression
```

## Architectural gain

```text
scene object
→ explicit owner
→ explicit semantic identity
→ passive canonical declaration
→ scoped discovery evidence
```

The model proves that Content Anchors can participate in a real Activity lifecycle without becoming hidden runtime authority, a global registry or an implicit materialization system.

## Usability gain

The authoring path is now visible and repeatable:

```text
create physical candidate
→ add official anchor component
→ assign owner
→ choose intent
→ set explicit ID
→ validate
→ inspect advanced evidence
→ exercise scene lifecycle
```
