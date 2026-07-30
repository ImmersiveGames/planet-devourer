# M15 — Camera Overrides

Status: Pending — scaffold available  
Roadmap order: 15

## Purpose

Demonstrar override de Activity sobre Player Camera e restauração normal.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M15_CameraOverrides.asset` (GameApplication)
- `Routes/Route_M15_CameraOverrides.asset` (Route)
- `Activities/Activity_M15_PlayerCamera.asset` (Activity)
- `Activities/Activity_M15_Cinematic.asset` (Activity)
- `Profiles/ActivityContent_M15_PlayerCamera.asset` (ActivityContent)
- `Profiles/ActivityContent_M15_Cinematic.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M15_Boot.unity`
- `Scenes/M15_Route.unity`
- `Scenes/M15_Player_Add.unity`
- `Scenes/M15_Cinematic_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M15_PlayerCamera.prefab`
- `Prefabs/PF_M15_ActivityCameraOverride.prefab`
- `Prefabs/PF_M15_CameraStatus.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Reutilizar apenas a ideia de M10, sem depender dos assets dele.
2. Configurar Player Camera e Activity override com prioridade explícita.
3. Garantir release e restauração normal.
4. Session e Route overrides ficam fora do primeiro corte.

## Play Mode flow

```text
Player Camera → Cinematic Activity → override vence → voltar → Player Camera restaurada
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

- priority tie
- multiple overrides
- stale override
- release order
- missing output

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
