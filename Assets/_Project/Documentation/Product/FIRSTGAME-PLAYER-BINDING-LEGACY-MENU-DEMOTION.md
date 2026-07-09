# FIRSTGAME Player Binding Legacy Menu Removal Supersedes Demotion

Status: FIRSTGAME product-flow cleanup
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Record that the previous demotion of local player-binding menus was superseded by removal after the official package `PlayerComposer` was validated in FIRSTGAME.

The old local path is no longer available for comparison or debugging. The primary and only player authoring flow is the official `PlayerComposer`.

## New primary flow

```text
1. Select PlayerPrototype.
2. Use FIRSTGAME > Immersive Framework > Player Composer Pilot > Configure Selected Player Composer.
3. Use the official PlayerComposer inspector.
4. Run Validate.
5. Run Apply/Rebuild.
6. Run Apply/Rebuild again to confirm idempotence when needed.
```

The authoritative authoring surface is now the package `PlayerComposer`.

## Superseded decision

The temporary decision to keep a legacy/debug route is obsolete. Keeping that route would allow regressions back to local FIRSTGAME tooling after the official Composer flow had already passed.

## What must not happen

The removed local route must not return as the primary player creation or repair path.

It must not become package API.

It must not be copied into `com.immersive.framework`.

It must not replace `PlayerComposer` Apply/Rebuild.

## Acceptance

This cleanup is PASS if:

- the old local menus are absent, including from `Legacy Debug`;
- the official PlayerComposer flow remains the documented primary flow;
- no runtime behavior is changed;
- no package files are changed;
- no QAFramework files are changed.
