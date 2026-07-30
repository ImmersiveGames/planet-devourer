# M13 — Pause

Status: Pending — scaffold available  
Roadmap order: 13

## Purpose

Demonstrar Pause sem Player e Pause com PlayerInput como variantes independentes.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M13_Pause.asset` (GameApplication)
- `Routes/Route_M13_Pause.asset` (Route)
- `Activities/Activity_M13_ApplicationPause.asset` (Activity)
- `Activities/Activity_M13_PlayerPause.asset` (Activity)
- `Profiles/ActivityContent_M13_ApplicationPause.asset` (ActivityContent)
- `Profiles/ActivityContent_M13_PlayerPause.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M13_Boot.unity`
- `Scenes/M13_Route.unity`
- `Scenes/M13_ApplicationPause_Add.unity`
- `Scenes/M13_PlayerPause_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M13_PauseSurface.prefab`
- `Prefabs/PF_M13_PauseControls.prefab`
- `Prefabs/PF_M13_Player.prefab`
- `Prefabs/PF_M13_PausePlayerBinding.prefab`
- `Prefabs/PF_M13_PauseStatus.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar Pause Surface e Pause/Resume triggers.
2. Montar variante sem Player.
3. Montar variante com PausePlayerInputBinding.
4. Expor Paused, Time Scale e Input Mode.

## Play Mode flow

```text
Variant A Pause/Resume → Variant B mover/Pause/Resume → Menu
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

- Pause duplicado
- Resume sem Pause
- stale binding
- exit cleanup failure
- Gate imbalance

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
