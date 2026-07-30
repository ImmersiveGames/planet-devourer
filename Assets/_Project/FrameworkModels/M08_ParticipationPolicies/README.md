# M08 — Participation Policies

Status: Pending — scaffold available  
Roadmap order: 8

## Purpose

Demonstrar projection e requirement através de cinco Activities pequenas.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M08_Participation.asset` (GameApplication)
- `Routes/Route_M08_Participation.asset` (Route)
- `Activities/Activity_M08_NoSlots.asset` (Activity)
- `Activities/Activity_M08_JoinedSlots.asset` (Activity)
- `Activities/Activity_M08_SelectedActors.asset` (Activity)
- `Activities/Activity_M08_LogicalPrepared.asset` (Activity)
- `Activities/Activity_M08_GameplayReady.asset` (Activity)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M08_Boot.unity`
- `Scenes/M08_Route.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M08_ParticipationPlayer.prefab`
- `Prefabs/PF_M08_ParticipationStatus.prefab`
- `Prefabs/PF_M08_ActivitySelector.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Inferred scaffold support

M08_Boot e M08_Route são cenas de suporte inferidas; o roadmap não fixa cenas específicas para este modelo.

## Configuration focus

1. Usar uma única composição base de Player.
2. Alterar somente projection/requirement em cada Activity.
3. Expor Required Level, Observed Level e Ready.
4. Não usar scripts diferentes por Activity.

## Play Mode flow

```text
selecionar Activity → observar requirement → observar estado alcançado → trocar Activity
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

- missing evidence
- forced premature ready
- inconsistent projection
- late Actor preparation
- release between levels

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
