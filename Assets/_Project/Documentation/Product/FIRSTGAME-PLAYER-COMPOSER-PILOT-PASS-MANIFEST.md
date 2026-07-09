# FIRSTGAME PlayerComposer Pilot PASS Manifest

Status: Documentation cut
Date: 2026-07-09
Repository: `planet-devourer`
Scope: FIRSTGAME

## Cut

P1D closure for the FIRSTGAME `PlayerComposer` pilot.

## Objective

Record that FIRSTGAME successfully validated the official package `PlayerComposer` as the main player authoring surface for the MVP pilot.

## Files created

```text
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-COMPOSER-PILOT-PASS.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-COMPOSER-PILOT-PASS-MANIFEST.md
```

## Files changed

```text
none
```

## Files removed

```text
none
```

## Out of scope

```text
C# changes
asmdef changes
package changes
QAFramework changes
runtime changes
editor tooling changes
PlayerRecipe
PlayerRuntimeContext
smoke
validator
movement
spawn
save/progression
menu removal
facade deletion
```

## Evidence recorded

```text
Configured official PlayerComposer intent.
Validation succeeded.
First Apply/Rebuild created/repaired materialization with blocked='0'.
Second Apply/Rebuild was idempotent with created='0' and repaired='0'.
FIRSTGAME pilot validation succeeded.
resetEnabled='False'.
resetParticipantPolicy='None'.
```

## Decisions recorded

- P1D is accepted as PASS.
- FIRSTGAME helper is a consumer setup aid, not official product API.
- Official materialization belongs to the package `PlayerComposer` Inspector.
- Old FIRSTGAME canonical binding menus may remain temporarily for debug/comparison.
- Next cut should demote old FIRSTGAME binding menus from primary UX.

## Acceptance criteria

This cut is PASS if:

- the two documentation files are created;
- no C# file is changed;
- no asmdef is changed;
- no package file is changed;
- no QAFramework file is changed;
- the PASS evidence is recorded;
- the next cut is identified as menu demotion, not deletion.

## Suggested commit message

```text
Docs: close FIRSTGAME PlayerComposer pilot
```
