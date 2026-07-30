# M12 — Activity Restart

Status: Pending — scaffold available  
Roadmap order: 12

## Purpose

Demonstrar a diferença entre Reset de objetos e Restart completo da Activity.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M12_ActivityRestart.asset` (GameApplication)
- `Routes/Route_M12_ActivityRestart.asset` (Route)
- `Activities/Activity_M12_Gameplay.asset` (Activity)
- `Profiles/ActivityContent_M12_Gameplay.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M12_Boot.unity`
- `Scenes/M12_Route.unity`
- `Scenes/M12_Activity_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M12_RestartableObjective.prefab`
- `Prefabs/PF_M12_RestartableWorld.prefab`
- `Prefabs/PF_M12_ActivityRestartControl.prefab`
- `Prefabs/PF_M12_RestartStatus.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar estado inicial e objetivo simples.
2. Adicionar Subjects necessários e ActivityRestartTrigger.
3. Expor Reset, Exit, Enter e Ready resumidamente.
4. Não chamar SceneManager para simular Restart.

## Play Mode flow

```text
alterar estado → completar objetivo → Restart Activity → observar reentrada → repetir
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

- Reset failure
- clear failure
- re-entry failure
- restart repetido
- restart durante transition

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
