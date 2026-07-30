# M14 — Transition and Loading

Status: Pending — scaffold available  
Roadmap order: 14

## Purpose

Demonstrar superfícies e políticas de transição e loading de forma isolada.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M14_TransitionLoading.asset` (GameApplication)
- `Routes/Route_M14_Menu.asset` (Route)
- `Routes/Route_M14_Destination.asset` (Route)
- `Activities/Activity_M14_Light.asset` (Activity)
- `Activities/Activity_M14_Loaded.asset` (Activity)
- `Profiles/ActivityContent_M14_Light.asset` (ActivityContent)
- `Profiles/ActivityContent_M14_Loaded.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M14_Boot.unity`
- `Scenes/M14_Menu.unity`
- `Scenes/M14_Destination.unity`
- `Scenes/M14_Light_Add.unity`
- `Scenes/M14_Loaded_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M14_TransitionSurface.prefab`
- `Prefabs/PF_M14_LoadingSurface.prefab`
- `Prefabs/PF_M14_Navigation.prefab`
- `Prefabs/PF_M14_TransitionStatus.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar Transition Surface e Loading Surface.
2. Associar adapters oficiais.
3. Configurar uma Route com loading e uma Activity sem loading visual.
4. Expor Covering, Loading, Revealing e Ready.

## Play Mode flow

```text
Menu → Destination com loading → Light Activity → Loaded Activity → Menu
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

- load failure
- transition adapter failure
- Gate release failure
- operation cancellation
- surface ausente

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
