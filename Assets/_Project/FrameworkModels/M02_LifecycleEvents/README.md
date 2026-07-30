# M02 — Lifecycle Events

Status: Authoring  
Roadmap order: 2  
Started: 2026-07-30  
Current checkpoint: complete independent authoring foundation

## Purpose

Demonstrar objetos reagindo ao lifecycle oficial de Scene, Route e Activity sem implementar uma autoridade
paralela no jogo consumidor.

## Startup semantics

O M02 possui três conceitos diferentes que não devem ser confundidos:

```text
Unity entry scene / Build Profile index 0
  M02_Boot

Game Application startup
  Startup Route = Route_M02_A

Startup Route scene
  Route_M02_A.Primary Scene = M02_RouteA

Application persistent content
  Content Scene = M02_PersistentContent
```

`GameApplicationAsset` não possui um campo "Startup Scene". Ele inicia uma `Startup Route`; a Route resolve
sua própria `Primary Scene`.

## Independence rule

```text
M02 owns its Game Application, Routes, Activities, Profiles, scenes and Persistent Content.
M02 does not reuse M01_PersistentContent or any M01 Route/Activity asset.
M02 has zero Local Player Slots.
Consumer scripts present lifecycle evidence but do not own or trigger lifecycle.
```

Mesmo que `M01_PersistentContent` carregue tecnicamente, reutilizá-la acopla os modelos e invalida a prova de
isolamento. A cena persistente deve ser uma cópia própria criada a partir da fonte oficial do package.

## Materialize the complete foundation

Apply the current M02 correction ZIP, let Unity compile, then run:

```text
Tools
→ Immersive Framework
→ FIRSTGAME
→ M02
→ Resolve Application Foundation
```

Esse comando agora é autocontido. Ele:

```text
creates every missing M02 authoring asset;
creates the five missing non-persistent scenes;
creates the three lifecycle placeholder prefabs;
creates M02_PersistentContent from the official package source scene;
removes only generated Main Camera/EventSystem objects from recognized M02 Boot/Route scenes;
preserves all existing assets, scenes and prefabs;
does not assign cross-asset references;
does not add lifecycle participants;
does not install bootstrap objects;
does not change Build Profiles or ProjectSettings.
```

O comando separado `Create Missing Scaffold` permanece disponível para materializar apenas assets, cenas e
prefabs sem resolver Persistent Content.

## Expected inventory

```text
M02_LifecycleEvents/
├── Application/
│   └── GA_M02_Lifecycle.asset
├── Routes/
│   ├── Route_M02_A.asset
│   └── Route_M02_B.asset
├── Activities/
│   ├── Activity_M02_A.asset
│   └── Activity_M02_B.asset
├── Profiles/
│   ├── ActivityContent_M02_A.asset
│   └── ActivityContent_M02_B.asset
├── Scenes/
│   ├── M02_PersistentContent.unity
│   ├── M02_Boot.unity
│   ├── M02_RouteA.unity
│   ├── M02_RouteB.unity
│   ├── M02_ActivityA_Add.unity
│   └── M02_ActivityB_Add.unity
└── Prefabs/
    ├── PF_M02_SceneLifecycleObject.prefab
    ├── PF_M02_RouteLifecycleObject.prefab
    └── PF_M02_ActivityLifecycleObject.prefab
```

## Authoring graph

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

## Build Profile

```text
0 — M02_Boot
1 — M02_PersistentContent
2 — M02_RouteA
3 — M02_RouteB
4 — M02_ActivityA_Add
5 — M02_ActivityB_Add
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

## Current acceptance gate

Do not add lifecycle participants until:

- [ ] The complete inventory above exists.
- [ ] `M02_PersistentContent` is assigned to the Game Application.
- [ ] Both Profiles validate.
- [ ] Both Activities validate with `No Slots`.
- [ ] Route A starts Activity A.
- [ ] Route B starts Activity B.
- [ ] `GA_M02_Lifecycle` validates with zero Slots.
- [ ] The six M02 scenes are enabled in the active Build Profile.

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

## Lifecycle callback bindings

The M02 prefabs use the official Inspector surfaces and a local visual-only
presenter (`FirstGame.FrameworkModels.M02.M02LifecycleVisualPresenter`):

```text
PF_M02_SceneLifecycleObject
  SceneLifecycleEvents.Available / Releasing
  -> presenter.OnAvailable / OnReleasing

PF_M02_RouteLifecycleObject
  RouteContentBinding (Route assigned per scene instance)
  RouteContentLifecycleEvents.Entered / Exited
  -> presenter.OnEntered / OnExited

PF_M02_ActivityLifecycleObject
  ActivityLocalVisibilityAdapter (Activity assigned per scene instance)
  ActivityContentLifecycleEvents.Entered / Exited
  -> presenter.OnEntered / OnExited
```

The presenter has explicit references to `Visual Placeholder` and `Label`; it
only displays the received event and does not resolve or control lifecycle.
The Advanced / Debug fields on the official components expose counters and the
last received event.

`Framework Components (Configure Manually)` and `Bindings (Configure Manually)`
are organizational children of the prefab root. Framework discovery begins at
the explicit loaded scene roots and traverses descendants, including inactive
objects; it is not a global search. Route and Activity assets plus
`localContentId` remain explicit and prevent callbacks crossing scope
boundaries.

## Build Profile note

The M02 Build Profile remains unchanged. The lifecycle components react only
to scenes already loaded by the official Game Application flow; they do not add
scenes or observe `SceneManager` directly.
