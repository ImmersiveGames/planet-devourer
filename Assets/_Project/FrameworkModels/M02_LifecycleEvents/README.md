# M02 — Lifecycle Events

Status: Authoring  
Roadmap order: 2  
Started: 2026-07-30  
Current checkpoint: independent application foundation and authoring graph

## Purpose

Demonstrar objetos reagindo ao lifecycle oficial de Scene, Route e Activity sem implementar um lifecycle
paralelo no jogo consumidor.

## Independence rule

```text
M02 owns its Game Application, Routes, Activities, Profiles and scenes.
M02 does not reuse the M01 Menu, Routes, Activities or Persistent Content.
M02 has zero Local Player Slots.
Consumer scripts present lifecycle evidence but do not own or trigger lifecycle.
```

## Materialize scaffold

Run once, if the unified scaffold has not already been materialized:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

Then run:

```text
Tools > Immersive Framework > FIRSTGAME > M02 > Resolve Application Foundation
```

The M02 resolver:

```text
creates M02_PersistentContent from the official package source scene;
removes only generated Main Camera/EventSystem objects from known M02 Boot/Route scenes;
preserves existing files;
does not assign Game Application references;
does not add lifecycle components;
does not change Build Profiles or ProjectSettings.
```

## Planned authoring assets

```text
Application/GA_M02_Lifecycle.asset
Routes/Route_M02_A.asset
Routes/Route_M02_B.asset
Activities/Activity_M02_A.asset
Activities/Activity_M02_B.asset
Profiles/ActivityContent_M02_A.asset
Profiles/ActivityContent_M02_B.asset
```

## Planned scenes

```text
Scenes/M02_PersistentContent.unity
Scenes/M02_Boot.unity
Scenes/M02_RouteA.unity
Scenes/M02_RouteB.unity
Scenes/M02_ActivityA_Add.unity
Scenes/M02_ActivityB_Add.unity
```

## Planned prefabs

```text
Prefabs/PF_M02_SceneLifecycleObject.prefab
Prefabs/PF_M02_RouteLifecycleObject.prefab
Prefabs/PF_M02_ActivityLifecycleObject.prefab
```

The prefabs are visible placeholders with manual framework/binding mount points. No lifecycle participant is
installed by the scaffold.

## Authoring graph — first block

### Activity Content A

```text
Content ID: m02.activity-a.scene
Scene: M02_ActivityA_Add
Requiredness: Required
Load Mode: Additive
Release Policy: Release On Activity Change
```

### Activity Content B

```text
Content ID: m02.activity-b.scene
Scene: M02_ActivityB_Add
Requiredness: Required
Load Mode: Additive
Release Policy: Release On Activity Change
```

### Activity A

```text
Activity Content Profile: ActivityContent_M02_A
Projection: No Slots
Zero Participants: Allowed
Requirement Level: None
Visual Transition: Seamless
Block During Transition: Lifecycle Requests Only
```

### Activity B

```text
Activity Content Profile: ActivityContent_M02_B
Projection: No Slots
Zero Participants: Allowed
Requirement Level: None
Visual Transition: Seamless
Block During Transition: Lifecycle Requests Only
```

### Route A

```text
Primary Scene: M02_RouteA
First Activity: Activity_M02_A
Additional Content: None
```

### Route B

```text
Primary Scene: M02_RouteB
First Activity: Activity_M02_B
Additional Content: None
```

### Game Application

```text
Application Name: M02 Lifecycle Events
Content Scene: M02_PersistentContent
Startup Route: Route_M02_A
Local Player Slots: Empty
Validation Mode: Standard
```

## Planned Play Mode flow

```text
Boot
→ Route A + Activity A
→ Activity B
→ Route B + Activity B
→ Route A + Activity A
```

There is no Menu step in this isolated model.

## Product behavior target

```text
Scene lifecycle
  visible response when a scene becomes available and before it releases.

Route lifecycle
  visible response when Route A/B enters and exits.

Activity lifecycle
  visible response when Activity A/B enters and exits.
```

## Consumer code boundary

Allowed:

```text
update label, material, light, animation or local counter;
store the last event for presentation;
receive callbacks from an official participant/binding.
```

Not allowed:

```text
invoke Enter/Exit manually;
resolve the active Route/Activity through global lookup;
replace framework lifecycle authority;
create a singleton or service locator;
use a QA probe as the presentation layer.
```

## Current acceptance gate

Do not add lifecycle participants until:

- [ ] `M02_PersistentContent` exists.
- [ ] Both Profiles validate.
- [ ] Both Activities validate with `No Slots`.
- [ ] Route A starts Activity A.
- [ ] Route B starts Activity B.
- [ ] `GA_M02_Lifecycle` validates with zero Slots.
- [ ] The six M02 scenes are enabled in the active Build Profile.

## UX review

Record:

```text
how the designer finds the correct participant;
whether Enter and Exit are clear in the Inspector;
whether callbacks are UnityEvents or typed adapters;
where requiredness is configured;
repeated assignments;
hidden dependencies;
Composer/Recipe/Template opportunities.
```

## QA follow-ups

```text
exact callback order;
idempotency;
required/optional participant failure;
exception during Enter/Exit;
reentry repetition.
```

Do not implement negative cases inside FIRSTGAME.
