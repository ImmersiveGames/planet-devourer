# M11 — Object Reset

Status: Pending — scaffold available  
Roadmap order: 11

## Purpose

Demonstrar restauração de Transform e estado de script por Reset oficial.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M11_Reset.asset` (GameApplication)
- `Routes/Route_M11_Reset.asset` (Route)
- `Activities/Activity_M11_Reset.asset` (Activity)
- `Profiles/ActivityContent_M11_Reset.asset` (ActivityContent)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M11_Boot.unity`
- `Scenes/M11_Route.unity`
- `Scenes/M11_Activity_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M11_TransformResettable.prefab`
- `Prefabs/PF_M11_StateResettable.prefab`
- `Prefabs/PF_M11_RuntimeSpawnedObject.prefab`
- `Prefabs/PF_M11_ResetControls.prefab`
- `Prefabs/PF_M11_ResetStatus.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Montar um Subject de Transform e outro de estado de script.
2. Configurar ObjectResetTrigger e ObjectResetGroupTrigger.
3. Manter Activity Restart fora deste modelo.
4. Runtime spawner permanece opcional.

## Play Mode flow

```text
alterar objetos → Reset Object → alterar novamente → Reset Group
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

- duplicate Subject identity
- participant failure
- group partial failure
- runtime registration failure

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
