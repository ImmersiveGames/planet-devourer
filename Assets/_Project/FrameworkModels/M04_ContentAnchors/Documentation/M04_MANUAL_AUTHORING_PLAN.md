# M04 Content Anchors — Manual Authoring Plan

## Cut Type

```text
UX/product + real integration
```

## Objective

Demonstrate passive Route and Activity Content Anchor declarations without automating framework authoring.

## Product Boundary

The public manual components are:

```text
RouteContentAnchor
ActivityContentAnchor
```

They declare intent only.

They do not:

- instantiate content;
- move objects;
- create runtime roots;
- perform logical binding;
- load or unload scenes;
- block lifecycle.

## Automated Preparation

The visual scaffold helper creates only:

```text
M04_Route
└── M04 Manual Authoring Visuals
    ├── Route Root Candidate
    └── Navigation Candidates

M04_ActivityA_Add
└── M04 Manual Authoring Visuals
    ├── Activity Root Candidate
    ├── Activity Slot Candidate
    └── Activity Point Candidate

M04_ActivityB_Add
└── M04 Manual Authoring Visuals
    ├── Activity Root Candidate
    ├── Activity Slot Candidate
    └── Activity Point Candidate
```

Each candidate contains:

```text
Visual
Label
Framework Component Mount (Add Manually)
Bindings Mount (Configure Manually)
```

No framework component is added.

## Manual Asset Configuration

Configure manually in the normal Inspectors:

```text
GA_M04_ContentAnchors
  Startup Route:
    Route_M04_ContentAnchors
```

```text
Route_M04_ContentAnchors
  Primary Scene:
    M04_Route

  Startup Activity:
    Activity_M04_A
```

```text
Activity_M04_A
  Activity Content Profile:
    ActivityContent_M04_A
```

```text
ActivityContent_M04_A
  Scene:
    M04_ActivityA_Add
```

Repeat for Activity B.

Do not use an Editor helper to assign these references.

## Manual Route Anchor

Select:

```text
M04_Route
→ M04 Manual Authoring Visuals
→ Route Root Candidate
→ Framework Component Mount (Add Manually)
```

Add manually:

```text
Immersive Framework
→ Route Content Anchor
```

Configure manually:

```text
Route:
  Route_M04_ContentAnchors

Anchor Id:
  m04.route.root

Kind:
  Root

Requiredness:
  Required

Display Name:
  M04 Route Root

Description:
  Route-owned passive root declaration for the M04 demonstration.
```

`Required` is authoring validation intent. It does not currently block Route lifecycle.

## Manual Activity A Anchors

Add `Activity Content Anchor` manually to each candidate mount.

### Root

```text
Activity:
  Activity_M04_A

Anchor Id:
  m04.activity-a.root

Kind:
  Root
```

### Slot

```text
Activity:
  Activity_M04_A

Anchor Id:
  m04.activity-a.slot

Kind:
  Slot
```

### Point

```text
Activity:
  Activity_M04_A

Anchor Id:
  m04.activity-a.point

Kind:
  Point
```

Choose Requiredness manually and record whether the Inspector explanation is sufficient.

## Manual Activity B Anchors

Repeat with:

```text
Activity:
  Activity_M04_B

Anchor Id:
  m04.activity-b.root
  m04.activity-b.slot
  m04.activity-b.point
```

## Local Scope

Do not configure a Local anchor in this cut.

The domain enum includes `Local`, but the verified scene-facing public components are Route- and Activity-specific. The FIRSTGAME must not invent a private-field workaround or consumer substitute.

Record this as a package authoring gap until an official public Local surface is confirmed.

## Manual Navigation

The scaffold creates physical navigation candidates only.

The user decides and configures the actual navigation surface manually using the official trigger component.

Required flow:

```text
Activity A
→ Activity B
→ Activity A
```

A later Route-exit path may be added manually when Route cleanup proof is selected.

## Static Authoring Review

For each anchor, inspect the official Inspector and record:

- owner field clarity;
- Anchor Id clarity;
- Kind explanation;
- Requiredness explanation;
- validation message;
- diagnostic identity;
- number of repeated fields;
- whether the component is understandable without source code.

## Play Mode Proof

Expected discovery:

```text
Route:
  candidates=1
  accepted=1

Activity A:
  candidates=3
  accepted=3

Activity B:
  candidates=3
  accepted=3
```

Expected lifecycle:

```text
Route Root remains while Activity changes.

Activity A scene unloads when B enters.
Activity B scene unloads when A re-enters.
Each new Activity occurrence is discovered without duplicate identity.
```

## Out of Scope

- prefab materialization;
- runtime content registration;
- logical Content Anchor binding;
- Local scope workaround;
- owner mismatch;
- duplicate IDs;
- invalid Kind;
- cleanup fault injection;
- QA synthetic fixtures.

## Acceptance

The cut passes only after manual authoring and actual Play Mode evidence.

The visual scaffold helper passing does not close the M04.
