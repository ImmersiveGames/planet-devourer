# FIRSTGAME Demonstration Authoring Procedure

Status: Frozen for current FIRSTGAME demonstration work  
Applies to: all Framework Models in `Assets/_Project/FrameworkModels`

## Purpose

FIRSTGAME must prove that a real framework consumer can understand and assemble a feature.

The demonstration must not hide the real authoring experience by automatically installing or configuring the framework contracts that are being evaluated.

## Frozen Rule

```text
Automation may prepare physical and visual scaffolding.

The user must manually apply framework components,
configure framework assets,
assign owners and references,
and wire authoring events.
```

This rule applies to every FIRSTGAME demonstration unless a later cut explicitly records an approved exception.

## Automation Allowed

A demonstration helper may:

- create missing placeholder scenes;
- create neutral GameObjects and hierarchy roots;
- create visible physical representations;
- create labels, primitive meshes, lights and cameras;
- create clearly named manual mount points;
- position visual objects;
- preserve and reuse existing placeholder content;
- create documentation and checklists;
- validate the physical scaffold without mutating framework configuration.

Allowed examples:

```text
Activity Root Candidate
Activity Slot Candidate
Activity Point Candidate
Framework Component Mount (Add Manually)
Navigation Candidate (Configure Manually)
```

## Automation Forbidden

A demonstration helper must not:

- add framework `MonoBehaviour` components;
- add consumer gameplay scripts that complete the feature automatically;
- configure `GameApplicationAsset`;
- configure `RouteAsset`;
- configure `ActivityAsset`;
- configure content profiles;
- assign Startup Route or Startup Activity;
- assign scene references to framework assets;
- set the active Game Application;
- configure Build Profiles or `EditorBuildSettings`;
- assign owner assets to authoring components;
- generate stable feature IDs on behalf of the user;
- configure enum intent such as Scope, Kind or Requiredness;
- wire UnityEvents or trigger callbacks;
- create hidden bootstrap objects;
- repair invalid authoring silently;
- apply a Composer or materialization operation unless that Composer itself is the feature under evaluation and the user explicitly invokes it.

## Why

FIRSTGAME evaluates product usability.

Automatic framework configuration would prove only that an Editor script knows the internal contract. It would not prove that a user can:

```text
find the feature;
understand the Inspector;
choose the correct owner;
understand required fields;
configure references;
recognize invalid authoring;
validate the result;
use it in Play Mode.
```

## Required Workflow for Every Model

### 1. Reconcile the real package surface

Before authoring:

- inspect current package source;
- identify public components and assets;
- identify runtime authority;
- identify explicit discovery scope;
- identify validation and diagnostics;
- identify missing public surfaces;
- do not invent a consumer substitute for a package gap.

### 2. Prepare only the physical scaffold

A scaffold helper may create visible placeholders and empty manual mounts.

The helper must be:

- idempotent;
- non-destructive;
- Undo-aware when practical;
- explicit about every scene it modifies;
- free of framework runtime configuration;
- safe to run more than once.

### 3. Perform framework authoring manually

The user applies and configures the official components through the normal Unity UI.

Record:

- number of steps;
- Inspector labels;
- unclear fields;
- repeated assignments;
- hidden dependencies;
- validation usefulness;
- missing Recipe, Profile, Composer or Template opportunities.

### 4. Validate authoring separately

Validation may inspect configuration but must not repair it.

Static validation proves authoring only. It does not prove runtime behavior.

### 5. Prove Play Mode behavior

Record actual evidence for:

- creation and discovery;
- runtime authority;
- state transitions;
- cleanup;
- re-entry;
- presentation;
- diagnostics;
- absence of silent fallback.

### 6. Separate FIRSTGAME and QA responsibilities

```text
FIRSTGAME
  real consumer assembly;
  happy-path gameplay use;
  UX findings;
  integration and re-entry.

QAFramework
  synthetic contracts;
  negative cases;
  invalid identity;
  mismatch;
  duplication;
  stale operations;
  regression matrices.
```

## Tool Boundary

A visual scaffold tool should preferably depend only on:

```text
UnityEditor
UnityEngine
UnityEngine.SceneManagement
```

It should not require `Immersive.Framework.*` namespaces.

This makes the boundary mechanically visible: the tool cannot configure a framework feature it does not reference.

## Naming Standard

Generated placeholders must make manual responsibility visible:

```text
Manual Authoring Visuals
Framework Component Mount (Add Manually)
Bindings Mount (Configure Manually)
Navigation Candidate (Configure Manually)
```

Avoid names that imply runtime success such as:

```text
Bound
Registered
Ready
Materialized
Configured
```

before the user has actually authored and proven those states.

## Exception Process

An exception requires an explicit cut stating:

```text
reason;
feature being evaluated;
automation introduced;
why it does not hide the evaluated UX;
files affected;
rollback path.
```

Without that record, the frozen rule remains in force.

## Acceptance for a Demonstration

A FIRSTGAME demonstration is not complete merely because a helper runs.

It must show that the user can manually:

- create or locate the feature surface;
- configure the intended contract;
- understand validation feedback;
- enter Play Mode;
- observe the expected behavior;
- diagnose failure or cleanup;
- repeat the flow after re-entry when applicable.
