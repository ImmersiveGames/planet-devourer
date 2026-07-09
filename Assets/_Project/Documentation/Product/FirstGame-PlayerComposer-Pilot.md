# FIRSTGAME PlayerComposer Pilot

Status: FIRSTGAME integration pilot
Date: 2026-07-09
Scope: `planet-devourer` / FIRSTGAME consumer project

## Objective

Use the official `Immersive.Framework.PlayerAuthoring.PlayerComposer` as the main Player authoring surface in FIRSTGAME.

This pilot does not turn FIRSTGAME scripts into package API. It only prepares a real consumer scene/prefab to use the official package component.

## Product rule

The official Player flow is:

```text
PlayerComposer Inspector
  -> Validate
  -> Apply/Rebuild
  -> _Framework/_Bindings materialization
```

FIRSTGAME helper menus may preconfigure the selected player, but they must not replace the official package `PlayerComposer` Inspector as the product surface.

## Added menu

```text
FIRSTGAME > Immersive Framework > Player Composer Pilot > Configure Selected Player Composer
FIRSTGAME > Immersive Framework > Player Composer Pilot > Validate Selected Player Composer
```

## Configure Selected Player Composer

This command expects a selected Player GameObject.

It will:

- add `PlayerComposer` if missing;
- assign the local `PlayerInput` if present;
- set `actorId` to `player.actor`;
- set `playerSlotId` to `player.1`;
- resolve a gameplay action map from `Player`, `Gameplay`, `PlayerInput.defaultActionMap`, or the first action map;
- create `Anchors/CameraTarget` and `Anchors/LookAtTarget` if missing;
- assign camera target and look-at target;
- keep reset disabled by default;
- keep reset participant policy as `None`;
- leave technical binding materialization to the official package Inspector.

## Validate Selected Player Composer

This command calls the official `PlayerComposer.TryValidateForApply()` and logs a FIRSTGAME pilot result.

It does not run Apply/Rebuild.

## Expected manual validation

1. Select the FIRSTGAME player GameObject.
2. Run `Configure Selected Player Composer`.
3. In the Inspector, use the official `PlayerComposer` Validate button.
4. In the Inspector, use the official `PlayerComposer` Apply/Rebuild button.
5. Run Apply/Rebuild a second time and confirm idempotency:

```text
created='0'
repaired='0'
alreadyValid='high'
blocked='0'
```

6. Enter Play Mode and confirm FIRSTGAME still has PlayerInput, camera target, player identity and reset behavior as expected.

## Out of scope

- No package implementation.
- No QAFramework changes.
- No smokes.
- No validator replacement.
- No PlayerRecipe.
- No PlayerRuntimeContext.
- No movement ownership.
- No spawn/join/save.
- No removal of previous FIRSTGAME facade scripts in this cut.

## Acceptance criteria

PASS if:

- FIRSTGAME compiles;
- selected Player receives official `PlayerComposer`;
- the official package Apply/Rebuild materializes bindings;
- helper menu is not required after configuration;
- reset participant is not created by default;
- old FIRSTGAME facade/repair menu is no longer the main Player authoring path.

## Suggested commit message

```text
FIRSTGAME: pilot official PlayerComposer
```
