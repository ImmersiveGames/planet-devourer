# FIRSTGAME PlayerComposer Pilot Manifest

Status: FIRSTGAME integration cut
Date: 2026-07-09

## Cut

P1D - FIRSTGAME pilot for the official PlayerComposer product surface.

## Objective

Introduce a FIRSTGAME-only pilot helper and documentation so the real consumer project can configure a Player with the official package `PlayerComposer` instead of using local binding facades as the main authoring flow.

## Files created

```text
Assets/_Project/Scripts/Editor/ImmersiveFramework/FirstGamePlayerComposerPilot.cs
Assets/_Project/Scripts/Editor/ImmersiveFramework/FirstGamePlayerComposerPilot.cs.meta
Assets/_Project/Scripts/Editor/ImmersiveFramework.meta
Assets/_Project/Documentation/Product/FirstGame-PlayerComposer-Pilot.md
Assets/_Project/Documentation/Product/FirstGame-PlayerComposer-Pilot.md.meta
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-COMPOSER-PILOT-MANIFEST.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-COMPOSER-PILOT-MANIFEST.md.meta
```

## Files changed

```text
none
```

## Files removed

```text
none
```

## Decisions

- The official package `PlayerComposer` remains the product surface.
- FIRSTGAME pilot helper only preconfigures the selected Player GameObject.
- FIRSTGAME pilot helper does not run technical materialization itself.
- Apply/Rebuild remains owned by the package `PlayerComposer` Inspector.
- Reset remains disabled by default.
- Reset participant policy remains `None` by default.
- Previous FIRSTGAME facades are not removed in this cut.

## Out of scope

```text
package changes
QAFramework changes
smoke
validator
PlayerRecipe
PlayerRuntimeContext
movement ownership
spawn
join
save/progression
removal of legacy FIRSTGAME tools
```

## Acceptance criteria

PASS if:

- FIRSTGAME compiles;
- selecting a Player and running Configure Selected Player Composer adds/configures official `PlayerComposer`;
- official PlayerComposer Validate succeeds when required fields are present;
- official PlayerComposer Apply/Rebuild creates bindings;
- second Apply/Rebuild is idempotent;
- reset participant is not created by default;
- FIRSTGAME helper is not the main Apply/Rebuild product surface.

## Suggested commit message

```text
FIRSTGAME: pilot official PlayerComposer
```
