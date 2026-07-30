# M10 — Player Camera

Status: Pending — scaffold available  
Roadmap order: 10

## Purpose

Demonstrar uma câmera do Player publicando request para um único output físico.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M10_PlayerCamera.asset` (GameApplication)
- `Routes/Route_M10_PlayerCamera.asset` (Route)
- `Activities/Activity_M10_PlayerCamera.asset` (Activity)
- `Profiles/ActivityContent_M10_PlayerCamera.asset` (ActivityContent)
- `Recipes/CameraRig_M10_Player.asset` (CameraRig — optional type resolution)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M10_Boot.unity`
- `Scenes/M10_Route.unity`
- `Scenes/M10_Activity_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M10_PersistentCameraOutput.prefab`
- `Prefabs/PF_M10_Player.prefab`
- `Prefabs/PF_M10_PlayerCameraRig.prefab`
- `Prefabs/PF_M10_CameraStatus.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar um único Camera Output físico.
2. Configurar Recipe/Composer de rig quando a classe estiver disponível.
3. Montar follow/look targets e PlayerGameplayCameraAuthoring.
4. Expor request ativo e vencedor do output.

## Play Mode flow

```text
Activity entra → Player admitido → request publicado → output segue Player → saída libera request
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

- dois requests
- prioridade empatada
- output ausente
- release failure
- request stale

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
