# M07 — Manager-Provisioned Player

Status: Pending — scaffold available  
Roadmap order: 7

## Purpose

Demonstrar criação autorizada de Player via PlayerInputManager.

The creation scaffold is preparation only. This model becomes `Authoring` when its configuration starts in roadmap order.

## Materialize scaffold

Run:

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02-M16
```

The command is idempotent and preserves existing files.

## Planned authoring assets

- `Application/GA_M07_ProvisionedPlayer.asset` (GameApplication)
- `Routes/Route_M07_ProvisionedPlayer.asset` (Route)
- `Activities/Activity_M07_ProvisionedPlayer.asset` (Activity)
- `Profiles/ActivityContent_M07_ProvisionedPlayer.asset` (ActivityContent)
- `Profiles/PlayerSlot_M07_Player1.asset` (PlayerSlot — optional type resolution)
- `Profiles/Actor_M07_Default.asset` (Actor — optional type resolution)

Common assets receive only an asset name, designer-facing name, description and required creation identity when applicable. No reference graph is assigned. Optional types are created only when their current ScriptableObject class exists.

## Generated scenes

- `Scenes/M07_Boot.unity`
- `Scenes/M07_Route.unity`
- `Scenes/M07_Activity_Add.unity`

Route/Boot scenes contain a camera, light, ground, visual markers and explicit manual mount roots. Additive scenes contain only owned visual content and manual mount roots. No framework bootstrap or lifecycle component is installed.

## Generated prefabs

- `Prefabs/PF_M07_PlayerInputManagerHost.prefab`
- `Prefabs/PF_M07_RuntimePlayer.prefab`
- `Prefabs/PF_M07_PlayerActor.prefab`
- `Prefabs/PF_M07_JoinControl.prefab`
- `Prefabs/PF_M07_PlayerStatusDisplay.prefab`

Each prefab is a visible placeholder with empty `Framework Components` and `Bindings` mount points. Add the official feature components manually.

## Configuration focus

1. Configurar Slot, Actor e prefab de Player.
2. Montar PlayerInputManager e provisioning authoring.
3. Configurar authorized join e Activity participation.
4. Expor Waiting for Join e admissão concluída.

## Play Mode flow

```text
Activity entra → Waiting for Join → Join autorizado → Player criado → saída libera Player
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

- join duplicado
- Slot ocupado
- Host validation failure
- rollback
- timeout
- release failure

Do not implement these cases in FIRSTGAME. Transfer confirmed technical scenarios to QAFramework.
