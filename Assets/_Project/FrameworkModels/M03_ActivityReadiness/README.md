# M03 — Activity Readiness

Status: Pending — scaffold available  
Roadmap order: 3

## Purpose

Demonstrar uma Activity aguardando uma preparação válida antes de ficar Ready.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M03_Readiness.asset` (GameApplication)
- `Routes/Route_M03_Readiness.asset` (Route)
- `Activities/Activity_M03_Preparation.asset` (Activity)
- `Profiles/ActivityContent_M03_Preparation.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M03_Boot.unity`
- `Scenes/M03_Route.unity`
- `Scenes/M03_Activity_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M03_PreparationParticipant.prefab`
- `Prefabs/PF_M03_ReadinessDisplay.prefab`
- `Prefabs/PF_M03_PreparedContent.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar o grafo Application → Route → Activity.
2. Associar M03_Activity_Add ao Profile.
3. Adicionar o participant oficial de preparação.
4. Expor Preparing e Ready sem timer controlador de demo.

## Play Mode flow

```text
entrar na Activity → observar preparação → observar Ready → usar conteúdo → sair
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

- required participant failure
- optional failure
- timeout
- participant ausente
- late completion

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
