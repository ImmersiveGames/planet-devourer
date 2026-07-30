# M09 — Input Gate

Status: Pending — scaffold available  
Roadmap order: 9

## Purpose

Demonstrar elegibilidade de input e bloqueio temporário.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M09_InputGate.asset` (GameApplication)
- `Routes/Route_M09_InputGate.asset` (Route)
- `Activities/Activity_M09_InputGate.asset` (Activity)
- `Profiles/ActivityContent_M09_InputGate.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M09_Boot.unity`
- `Scenes/M09_Route.unity`
- `Scenes/M09_Activity_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M09_Player.prefab`
- `Prefabs/PF_M09_InteractionTarget.prefab`
- `Prefabs/PF_M09_GateControl.prefab`
- `Prefabs/PF_M09_InputStatus.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Montar base mínima de Player própria.
2. Adicionar adapter oficial de Input Gate.
3. Configurar Acquire e Release Gate.
4. Expor elegibilidade de Input, Interaction e Gameplay.

## Play Mode flow

```text
mover/interagir → Acquire Gate → ações param → Release Gate → ações retornam
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

- double acquire
- release sem acquire
- Gate stale
- Activity exit com Gate
- binding ausente

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
