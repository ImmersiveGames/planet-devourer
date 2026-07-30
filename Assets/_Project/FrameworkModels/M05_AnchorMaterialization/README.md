# M05 — Anchor Materialization

Status: Pending — scaffold available  
Roadmap order: 5

## Purpose

Demonstrar materialização explícita de prefab em anchor e release com o scope.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M05_Materialization.asset` (GameApplication)
- `Routes/Route_M05_Materialization.asset` (Route)
- `Activities/Activity_M05_Materialization.asset` (Activity)
- `Profiles/ActivityContent_M05_Materialization.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M05_Boot.unity`
- `Scenes/M05_Route.unity`
- `Scenes/M05_Activity_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M05_Anchor.prefab`
- `Prefabs/PF_M05_MaterializedContent.prefab`
- `Prefabs/PF_M05_MaterializationBridge.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Modelo opcional: configurar somente quando houver superfície oficial suficiente.
2. Associar prefab, anchor, scope, owner e release policy explicitamente.
3. Não criar materialização automática fora do bridge oficial.

## Play Mode flow

```text
Activity entra → conteúdo materializa no anchor → Activity sai → conteúdo é liberado
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

- missing prefab
- missing anchor
- duplicate materialization
- invalid owner
- failed release

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
