# FIRSTGAME PlayerComposer Pilot PASS

Status: PASS
Date: 2026-07-09
Scope: FIRSTGAME consumer validation
Package surface validated: `Immersive.Framework.PlayerAuthoring.PlayerComposer`

## 1. Objective

Close the FIRSTGAME pilot for the official `PlayerComposer` surface.

This document records that FIRSTGAME can use the official package `PlayerComposer` as the main player authoring surface for the current MVP pilot.

This is not a QA smoke and it is not a new FIRSTGAME facade. It is a consumer validation of the official package surface.

## 2. Validated flow

The validated flow was:

```text
1. Select PlayerPrototype.
2. Run FIRSTGAME PlayerComposer pilot configuration.
3. Use the official PlayerComposer Inspector.
4. Validate PlayerComposer.
5. Apply/Rebuild PlayerComposer.
6. Apply/Rebuild again to prove idempotence.
7. Run FIRSTGAME pilot validation.
```

The FIRSTGAME helper only configured intent. Technical materialization remained owned by the official `PlayerComposer` Inspector.

## 3. Evidence

### FIRSTGAME intent configuration

```text
[FIRSTGAME][PlayerComposerPilot] Configured official PlayerComposer intent.
player='PlayerPrototype'
actorId='player.actor'
playerSlotId='player.1'
actionMap='Player'
hasPlayerInput='True'
Use the PlayerComposer Inspector Apply/Rebuild button to materialize framework bindings.
```

This confirms that FIRSTGAME did not materialize bindings directly. It configured the official composer surface and redirected the user to the package Inspector.

### Package validation

```text
[Immersive.Framework][PlayerComposer] Validation succeeded.
player='PlayerPrototype'
actorId='player.actor'
playerSlotId='player.1'
```

This confirms that the official package surface accepted the FIRSTGAME player intent.

### First Apply/Rebuild

```text
[Immersive.Framework][PlayerComposer] Apply/Rebuild completed.
player='PlayerPrototype'
actorId='player.actor'
playerSlotId='player.1'
created='7'
repaired='2'
alreadyValid='5'
skippedByPolicy='2'
blocked='0'
resetEnabled='False'
resetParticipantPolicy='None'
```

This confirms that the composer materialized missing technical bindings and repaired existing drift without blocking.

### Second Apply/Rebuild

```text
[Immersive.Framework][PlayerComposer] Apply/Rebuild completed.
player='PlayerPrototype'
actorId='player.actor'
playerSlotId='player.1'
created='0'
repaired='0'
alreadyValid='13'
skippedByPolicy='2'
blocked='0'
resetEnabled='False'
resetParticipantPolicy='None'
```

This confirms idempotence in the real consumer project.

### FIRSTGAME pilot validation

```text
[FIRSTGAME][PlayerComposerPilot] PlayerComposer validation succeeded.
player='PlayerPrototype'
actorId='player.actor'
playerSlotId='player.1'
playerInput='PlayerPrototype'
actionMap='Player'
actionMapFound='True'
resetEnabled='False'
```

This confirms that FIRSTGAME can read the configured official composer state and that the selected action map exists.

## 4. Product interpretation

The pilot proves that the player surface can move from:

```text
FIRSTGAME local facade + validate/apply/repair menus
```

to:

```text
official PlayerComposer + Inspector Validate + Inspector Apply/Rebuild
```

The FIRSTGAME helper remains only a consumer-side setup aid. It is not the official product API and it does not replace the package Inspector.

## 5. What this replaces as main UX

The official flow should become the primary player authoring path:

```text
PlayerComposer
  Validate
  Apply/Rebuild
  Advanced/Debug diagnostics
```

The older FIRSTGAME canonical binding menus may remain temporarily for comparison or emergency debug, but they should no longer be the main UX path once the Composer pilot is accepted.

## 6. What remains temporary

Temporary items:

- FIRSTGAME pilot helper menu.
- Existing FIRSTGAME canonical player binding facade menus.
- Hardcoded pilot defaults:
  - actorId = `player.actor`
  - playerSlotId = `player.1`
  - preferred action map = `Player`
- No `PlayerRecipe` yet.
- No `PlayerRuntimeContext` yet.
- No QAFramework smoke for PlayerComposer yet.

## 7. Out of scope

This closure did not implement:

- new package runtime;
- `PlayerRecipe`;
- `PlayerRuntimeContext`;
- QAFramework smoke;
- movement ownership;
- spawn;
- multiplayer join;
- save/progression;
- removal of old FIRSTGAME menus;
- deletion of local FIRSTGAME facades.

## 8. Acceptance

P1D is PASS because:

- FIRSTGAME configured the official `PlayerComposer`;
- package validation succeeded;
- first Apply/Rebuild completed with `blocked='0'`;
- second Apply/Rebuild was idempotent with `created='0'` and `repaired='0'`;
- reset participant remained optional by default;
- FIRSTGAME pilot validation succeeded;
- materialization stayed in the official package Inspector.

## 9. Next recommended cut

Next cut:

```text
P1E - FIRSTGAME Player Binding Menu Demotion
```

Goal:

```text
Reclassify old FIRSTGAME canonical player binding facade menus as legacy/debug/comparison tools,
without deleting them yet.
```

This should reduce the chance that future work keeps using local FIRSTGAME repair menus as the main product flow.

## 10. Suggested commit message

```text
Docs: close FIRSTGAME PlayerComposer pilot
```
