# M06 — Scene-Provided Player

Status: Pending — scaffold available  
Roadmap order: 6

## Purpose

Demonstrar adoção de um Player já authorado na cena.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M06_ScenePlayer.asset` (GameApplication)
- `Routes/Route_M06_ScenePlayer.asset` (Route)
- `Activities/Activity_M06_ScenePlayer.asset` (Activity)
- `Profiles/ActivityContent_M06_ScenePlayer.asset` (ActivityContent)
- `Profiles/PlayerSlot_M06_Player1.asset` (PlayerSlot — optional type resolution)
- `Profiles/Actor_M06_Default.asset` (Actor — optional type resolution)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M06_Boot.unity`
- `Scenes/M06_Route.unity`
- `Scenes/M06_Activity_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M06_SceneProvidedPlayer.prefab`
- `Prefabs/PF_M06_PlayerActor.prefab`
- `Prefabs/PF_M06_PlayerStatusDisplay.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar Slot e Actor próprios.
2. Montar PlayerInput, Host, admission authoring e Actor declaration.
3. Configurar Activity participation e requirement mínimo.
4. Movimento permanece código do jogo.

## Play Mode flow

```text
entrar → Player admitido → mover → sair → reentrar sem duplicação
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

- Slot ocupado
- Host inválido
- Actor ausente
- duplicate Actor
- cleanup failure

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
