# M01 — Route and Activity

Status: Authoring  
Started: 2026-07-29  
Current checkpoint: Game Application foundation and local validation scope

## Purpose

Demonstrar a composição mínima de `GameApplication`, `Route` e `Activity` sem Player, gameplay Camera, Reset ou Pause.

A criação física dos assets não faz parte da avaliação deste modelo. O teste de UX começa quando um desenvolvedor consumidor precisa entender, montar, configurar, validar e executar o grafo autoral.

## Scaffold policy

O corte inclui uma ferramenta Editor explícita:

```text
Tools
└── Immersive Framework
    └── FIRSTGAME
        └── M01
            ├── Create Missing Scaffold
            └── Resolve Application Foundation
```

A ferramenta:

```text
cria somente arquivos ausentes;
preserva o GA_M01_RouteActivity já criado;
não substitui cenas, prefabs ou assets existentes;
não preenche referências entre assets;
não altera ProjectSettings ou Build Profiles;
não instala bootstrap;
não adiciona triggers do framework;
não executa gameplay;
cria a cena persistente do M01 a partir da fonte oficial do package;
remove Camera e EventSystem gerados nas cenas de Route/Boot para preservar uma única autoridade persistente.
```

Stable IDs de Route e Activity são gerados como metadado técnico de criação. A configuração de produto permanece aberta.

## What This Model Demonstrates

```text
Game Application
→ startup Menu Route
→ Gameplay Route
→ startup Activity A
→ Activity B
→ Activity A
→ Menu Route
```

## Required Package Features

```text
GameApplicationAsset
RouteAsset
ActivityAsset
ActivityContentProfileAsset
RouteRequestTrigger
ActivityRequestTrigger
Framework settings / application selection
Inspector validation for these authoring surfaces
```

## Generated authoring assets

```text
Application/GA_M01_RouteActivity.asset
Routes/Route_M01_Menu.asset
Routes/Route_M01_Gameplay.asset
Activities/Activity_M01_A.asset
Activities/Activity_M01_B.asset
Profiles/ActivityContent_M01_A.asset
Profiles/ActivityContent_M01_B.asset
```

The Game Application contains only its display name when created by the scaffold. Routes and Activities contain only their display name, description and stable creation identity. Profiles contain only their description.

## Generated scenes

```text
Scenes/M01_PersistentContent.unity
Scenes/M01_Boot.unity
Scenes/M01_Menu.unity
Scenes/M01_Gameplay.unity
Scenes/M01_ActivityA_Add.unity
Scenes/M01_ActivityB_Add.unity
```

`M01_PersistentContent` is copied from the official package source scene and owns the physical Camera, EventSystem, loading and transition presentation required by the current application contract. Route and Boot scenes keep visual content and UI mounts, but do not own a second Camera or EventSystem. No scene contains request destinations or authored Route/Activity references.

## Generated prefabs

```text
Prefabs/PF_M01_RouteNavigation.prefab
Prefabs/PF_M01_ActivityNavigation.prefab
Prefabs/PF_M01_CurrentContextDisplay.prefab
```

The navigation prefabs contain visual Unity UI buttons with empty `On Click` events. The context display contains presentation-only placeholder labels. Framework components and bindings must be added manually.

## Setup — current block

### 1. Materialize the scaffold

1. Apply the ZIP to `planet-devourer`.
2. Let Unity compile.
3. Run `Tools > Immersive Framework > FIRSTGAME > M01 > Create Missing Scaffold`.
4. Run `Tools > Immersive Framework > FIRSTGAME > M01 > Resolve Application Foundation`.
5. Confirm `M01_PersistentContent` exists and that `M01_Boot`, `M01_Menu` and `M01_Gameplay` no longer contain a generated `Main Camera` or `EventSystem`.
6. Both commands are idempotent and preserve existing assets.

### 2. Configure Activity Content Profiles

Open `ActivityContent_M01_A`:

```text
add one scene entry;
Scene = M01_ActivityA_Add;
Content ID = m01.activity-a.scene;
Requiredness = Required;
Load Mode = Additive;
Release Policy = Release On Activity Change.
```

Open `ActivityContent_M01_B`:

```text
add one scene entry;
Scene = M01_ActivityB_Add;
Content ID = m01.activity-b.scene;
Requiredness = Required;
Load Mode = Additive;
Release Policy = Release On Activity Change.
```

Use the labels exposed by the current Inspector. Record any wording or ordering difference as a UX finding.

### 3. Configure Activities

`Activity_M01_A`:

```text
Activity Content Profile = ActivityContent_M01_A
Projection = No Slots
Zero Participants = Allowed
Requirement Level = None
```

`Activity_M01_B`:

```text
Activity Content Profile = ActivityContent_M01_B
Projection = No Slots
Zero Participants = Allowed
Requirement Level = None
```

Keep transition presentation at the simplest valid option for this model.

### 4. Configure Routes

`Route_M01_Menu`:

```text
Primary Scene = M01_Menu
First Activity = None
Additional Content = None
```

`Route_M01_Gameplay`:

```text
Primary Scene = M01_Gameplay
First Activity = Activity_M01_A
Additional Content = None
```

### 5. Complete the Game Application

`GA_M01_RouteActivity`:

```text
Startup Route = Route_M01_Menu
Local Player Slots = empty
Persistent Content / Content Scene = M01_PersistentContent
Validation Mode = Standard
```

Run the validation available on both Activities, both Profiles, both Routes and the Game Application. Zero Local Player Slots is valid because both Activities use `Projection = No Slots`. Unrelated Actor Profiles from M06/M07 must not appear in local Game Application validation; they remain visible only in explicit project audits. Do not add Player, Route-local Camera, Reset or Pause to silence unrelated findings.

## Next block

After the authoring graph validates:

```text
mount PF_M01_RouteNavigation in M01_Menu and M01_Gameplay;
mount PF_M01_ActivityNavigation in M01_Gameplay;
mount PF_M01_CurrentContextDisplay where it remains readable;
add RouteRequestTrigger and ActivityRequestTrigger;
configure explicit destinations and request reasons;
configure M01_Boot and framework application selection;
add the five scenes to the active Build Profile;
run the complete Play Mode flow.
```

Do not begin this block until the asset graph above has been inspected and validated.

## Play Mode Flow

```text
Boot
→ Menu
→ Gameplay Route
→ startup Activity A
→ Activity B
→ Activity A
→ Menu
```

## Expected Result

```text
Menu exists only in the Menu Route;
Gameplay Route environment remains while Activities change;
Activity A content exists only in Activity A;
Activity B content exists only in Activity B;
returning to Menu releases Gameplay and Activity content.
```

## Reusable Pieces

The three prefabs are candidates for reuse. They are not considered official templates until the same pieces can be mounted in a second model without hidden assumptions.

## UX Findings

| Area | Observation | Impact | Destination |
|---|---|---|---|
| Creation | Asset creation is scaffolded because creation coverage is already accepted in QA. | Removes repetitive setup from the consumer test. | FIRSTGAME workflow |
| Activity composition | Current Activity scene declaration requires an Activity Content Profile. | Supporting assets must be visible in the model structure. | Package/docs |
| Inspector | To record during configuration. | — | Package |
| Validation | Local Game Application validation previously mixed project-wide Actor Profile findings and treated zero Slots as an error. | Blocked isolated models. | Fixed in package and covered by QA smoke |
| Runtime | Not started. | — | Package/QA |

Questions to record:

```text
Can a developer understand why Activity scenes live in a Profile?
Does the Profile Inspector make Additive and Release policy obvious?
Does the Route Inspector distinguish Primary Scene from First Activity?
Do validation messages identify the exact missing reference and asset?
Can the graph be configured without opening package source?
```

## QA Follow-ups

Register only technical cases discovered during configuration or Play Mode. Do not implement invalid fixtures inside FIRSTGAME.
