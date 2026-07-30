# M02 — Lifecycle Events

Status: Pending — scaffold available  
Roadmap order: 2

## Purpose

Demonstrar objetos reagindo ao lifecycle de Scene, Route e Activity sem implementar um lifecycle paralelo.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M02_Lifecycle.asset` (GameApplication)
- `Routes/Route_M02_A.asset` (Route)
- `Routes/Route_M02_B.asset` (Route)
- `Activities/Activity_M02_A.asset` (Activity)
- `Activities/Activity_M02_B.asset` (Activity)
- `Profiles/ActivityContent_M02_A.asset` (ActivityContent)
- `Profiles/ActivityContent_M02_B.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M02_Boot.unity`
- `Scenes/M02_RouteA.unity`
- `Scenes/M02_RouteB.unity`
- `Scenes/M02_ActivityA_Add.unity`
- `Scenes/M02_ActivityB_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M02_SceneLifecycleObject.prefab`
- `Prefabs/PF_M02_RouteLifecycleObject.prefab`
- `Prefabs/PF_M02_ActivityLifecycleObject.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar Application, duas Routes e duas Activities próprias.
2. Associar cenas de Activity através dos Profiles auxiliares.
3. Adicionar os participants oficiais de Scene, Route e Activity lifecycle.
4. Conectar callbacks somente a apresentação local.

## Play Mode flow

```text
Route A / Activity A → Activity B → Route B → Route A / startup Activity → Menu
```

## UX review

Record:

- number of configuration steps;
- technical fields shown before designer intent;
- repeated assignments;
- hidden dependencies;
- missing Composer, Recipe, Profile or template opportunities;
- whether normal diagnostics are sufficient without opening code.

## QA Follow-ups

- ordem exata de callbacks
- idempotência
- participant obrigatório ou opcional com falha
- exceção durante Enter ou Exit
- reentrada repetida

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
