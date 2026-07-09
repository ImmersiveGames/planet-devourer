# FIRSTGAME Player Binding Facade Removal Manifest

Status: Ready for Unity validation
Date: 2026-07-09
Scope: `Assets/_Project`

## Cut

FIRSTGAME player-binding local route removal after official `PlayerComposer` validation.

## Objective

Remove the legacy FIRSTGAME-only player-binding authoring scripts and leave `PlayerComposer` as the main and single authoring flow for Player setup.

## Files Removed

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingAuthoringFacade.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingAuthoringFacade.cs.meta
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingFacadeRepairProof.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingFacadeRepairProof.cs.meta
```

## Files Changed

```text
Assets/_Project/Documentation/Product-Transition-Audit-FIRSTGAME.md
Assets/_Project/Documentation/F53F-Player-Binding-Operational-Guide.md
Assets/_Project/Documentation/F53G-FIRSTGAME-CLEANUP-MANIFEST.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-LEGACY-MENU-DEMOTION.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-LEGACY-MENU-DEMOTION-MANIFEST.md
```

## Files Created

```text
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-FACADE-REMOVAL.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-FACADE-REMOVAL.md.meta
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-FACADE-REMOVAL-MANIFEST.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-BINDING-FACADE-REMOVAL-MANIFEST.md.meta
```

## Out Of Scope

```text
com.immersive.framework changes
QAFramework changes
PlayerComposer changes
scene or prefab manual edits
new facade
new smoke
new validator
Unity build, Play Mode, smoke or batchmode execution
```

## Validation

Static validation performed:

```text
- removed script files no longer exist under Assets/_Project;
- no code references to removed types remain under Assets/_Project;
- FirstGamePlayerComposerPilot.cs remains under Assets/_Project;
- no files outside Assets/_Project were edited by this cut.
```

Manual validation required:

```text
1. Unity compiles.
2. Old local player-binding menus are absent, including from Legacy Debug.
3. Player Composer Pilot menu remains available.
4. Select PlayerPrototype.
5. Run Configure Selected Player Composer.
6. In the official PlayerComposer Inspector, run Validate.
7. Run Apply/Rebuild twice.
```

Expected result:

```text
Validation succeeded
Apply/Rebuild completed with blocked='0'
Second Apply/Rebuild with created='0' and repaired='0'
```

## Suggested Commit Message

```text
FIRSTGAME: remove legacy player binding facade
```
