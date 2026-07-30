# M04 — Content Anchors

Status: Pending — scaffold available  
Roadmap order: 4

## Purpose

Demonstrar objetos de cena declarando ownership Route, Activity e Local por anchors explícitos.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M04_ContentAnchors.asset` (GameApplication)
- `Routes/Route_M04_ContentAnchors.asset` (Route)
- `Activities/Activity_M04_A.asset` (Activity)
- `Activities/Activity_M04_B.asset` (Activity)
- `Profiles/ActivityContent_M04_A.asset` (ActivityContent)
- `Profiles/ActivityContent_M04_B.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M04_Boot.unity`
- `Scenes/M04_Route.unity`
- `Scenes/M04_ActivityA_Add.unity`
- `Scenes/M04_ActivityB_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M04_RouteRootAnchor.prefab`
- `Prefabs/PF_M04_ActivityRootAnchor.prefab`
- `Prefabs/PF_M04_ActivitySlotAnchor.prefab`
- `Prefabs/PF_M04_LocalPointAnchor.prefab`
- `Prefabs/PF_M04_AnchorStatusDisplay.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar Route e duas Activities.
2. Montar anchors Route Root, Activity Root/Slot/Point e Local.
3. Manter scope, kind e owner explícitos.
4. Mostrar binding status somente como diagnóstico.

## Play Mode flow

```text
Route / Activity A → Activity B → Menu, com cleanup visível dos anchors
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

- Route mismatch
- Activity mismatch
- duplicate anchor identity
- invalid scope ou kind
- cleanup failure

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
