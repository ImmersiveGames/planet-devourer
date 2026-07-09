# FIRSTGAME Player Binding Legacy Menu Removal Supersedes Demotion Manifest

Status: FIRSTGAME cleanup cut
Date: 2026-07-09

## Cut

P1E - FIRSTGAME Player Binding Menu Demotion, superseded by removal.

## Objective

Record that the previous `Legacy Debug` demotion was superseded by removal after validating the official package `PlayerComposer` flow in FIRSTGAME.

## Files changed

```text
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-LEGACY-MENU-DEMOTION.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-LEGACY-MENU-DEMOTION-MANIFEST.md
```

## Files created

```text
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-LEGACY-MENU-DEMOTION.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-LEGACY-MENU-DEMOTION-MANIFEST.md
```

## Files removed

```text
none
```

## Out of scope

```text
package changes
QAFramework changes
runtime behavior changes
PlayerComposer changes
removing legacy local scripts was done by the superseding removal cut
new smoke
new validator
```

## Decisions

- The official package `PlayerComposer` is the primary player authoring surface.
- The old FIRSTGAME local player-binding path is not kept as legacy/debug/comparison tooling.
- The old repair proof is not kept as legacy/debug tooling.

## Acceptance

This cut is PASS if:

- Unity compiles;
- old local menus are absent, including from `Legacy Debug`;
- PlayerComposer pilot menus remain unchanged;
- no package files are modified;
- no QAFramework files are modified.

## Suggested commit message

```text
FIRSTGAME: demote legacy player binding menus
```
