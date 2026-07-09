# FIRSTGAME Player Binding Facade Removal

Status: Ready for Unity validation
Date: 2026-07-09
Scope: `Assets/_Project`

## 1. Objective

Remove the old local FIRSTGAME player-binding authoring route after the official package `PlayerComposer` was validated in the consumer project.

The single player authoring flow is now:

```text
PlayerComposer official package component
Inspector Validate
Inspector Apply/Rebuild
Composer diagnostics
```

## 2. Why Remove

The local route was useful while the package Composer did not exist yet. After the official Composer passed in FIRSTGAME, keeping the local route under debug menus would allow regressions back to consumer-only tooling.

FIRSTGAME must prove product usability, not become the permanent owner of framework authoring.

## 3. Removed Files

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingAuthoringFacade.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingAuthoringFacade.cs.meta
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingFacadeRepairProof.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingFacadeRepairProof.cs.meta
```

## 4. Official Replacement Flow

```text
1. Select PlayerPrototype.
2. Run FIRSTGAME > Immersive Framework > Player Composer Pilot > Configure Selected Player Composer.
3. Use the official PlayerComposer Inspector.
4. Run Validate.
5. Run Apply/Rebuild.
6. Run Apply/Rebuild again to confirm idempotence.
```

Expected evidence:

```text
Validation succeeded
Apply/Rebuild completed with blocked='0'
Second Apply/Rebuild with created='0' and repaired='0'
```

## 5. What Was Not Removed

```text
Assets/_Project/Scripts/Editor/ImmersiveFramework/FirstGamePlayerComposerPilot.cs
Assets/_Project/Documentation/Product/FirstGame-PlayerComposer-Pilot.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-COMPOSER-PILOT-PASS.md
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-COMPOSER-PILOT-PASS-MANIFEST.md
```

No package files were changed.

No QAFramework files were changed.

No scenes or prefabs were manually edited.

No new facade, smoke or validator was created.

## 6. Expected Validation

Manual validation after applying this cut:

```text
1. Unity compiles.
2. Old local player-binding menus are absent, including from Legacy Debug.
3. Player Composer Pilot menu remains available.
4. Select PlayerPrototype.
5. Run Configure Selected Player Composer.
6. In the official PlayerComposer Inspector, run Validate.
7. Run Apply/Rebuild twice.
```

## 7. Acceptance Criteria

This cut is PASS if:

```text
- the old local scripts are removed;
- old local player-binding menus are gone;
- no code references to the removed types remain;
- FirstGamePlayerComposerPilot remains;
- PlayerComposer remains the main player authoring flow;
- no com.immersive.framework files are changed;
- no QAFramework files are changed;
- Unity compiles;
- PlayerComposer validates and applies on PlayerPrototype;
- second Apply/Rebuild is idempotent.
```

## 8. Suggested Commit Message

```text
FIRSTGAME: remove legacy player binding facade
```
