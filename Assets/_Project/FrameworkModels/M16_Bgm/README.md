# M16 — BGM

Status: Pending — scaffold available  
Roadmap order: 16

## Purpose

Demonstrar o adapter opcional de BGM sem torná-lo dependência dos demais modelos.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M16_Bgm.asset` (GameApplication)
- `Routes/Route_M16_Bgm.asset` (Route)
- `Activities/Activity_M16_OwnMusic.asset` (Activity)
- `Activities/Activity_M16_UseRoute.asset` (Activity)
- `Activities/Activity_M16_Silence.asset` (Activity)
- `Profiles/ActivityContent_M16_OwnMusic.asset` (ActivityContent)
- `Profiles/ActivityContent_M16_UseRoute.asset` (ActivityContent)
- `Profiles/ActivityContent_M16_Silence.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M16_Boot.unity`
- `Scenes/M16_Route.unity`
- `Scenes/M16_OwnMusic_Add.unity`
- `Scenes/M16_UseRoute_Add.unity`
- `Scenes/M16_Silence_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M16_BgmDirector.prefab`
- `Prefabs/PF_M16_RouteBgmBinding.prefab`
- `Prefabs/PF_M16_ActivityBgmBinding.prefab`
- `Prefabs/PF_M16_BgmStatus.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Manter status Experimental visível.
2. Configurar Route BGM e policies das três Activities.
3. Montar Director e bindings opcionais atuais.
4. Não tornar BGM dependência de outros modelos.

## Play Mode flow

```text
Route BGM → Activity own BGM → use Route → silence → Route BGM restaurada
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

- clip ausente
- binding duplicado
- release failure
- policy inválida
- director ausente

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
