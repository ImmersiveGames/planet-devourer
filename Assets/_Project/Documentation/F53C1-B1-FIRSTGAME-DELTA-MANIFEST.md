# F53C1-B1 - FIRSTGAME Delta Manifest

Status: Hotfix delta
Scope: `Assets/_Project`
Date: 2026-07-09

## Reason

Fix compile error caused by `FirstGameCameraCutSetup.cs` importing `FirstGame.FrameworkIntegration.Editor` across FIRSTGAME editor assemblies.

## Files changed

```text
Assets/_Project/Scripts/Editor/GameCamera/FirstGameCameraCutSetup.cs
```

## Files created

```text
Assets/_Project/Documentation/F53C1-B1-FIRSTGAME-DELTA-MANIFEST.md
```

## Files removed

```text
None
```

## Notes

`FirstGameCameraCutSetup` now resolves the canonical FIRSTGAME player with local editor-only scene resolution. This avoids cross-assembly dependency on the FrameworkIntegration editor namespace while preserving the F53C1 rule: player resolution uses `PlayerActorDeclaration`, `PlayerSlotDeclaration` and `PlayerInput`, not `GameObject.name`.

## Expected smoke

```text
FIRSTGAME > Immersive Framework > Validate Real Player Binding
Tools > FIRSTGAME > Camera > Configure Route-Activity Camera
```

Expected evidence remains:

```text
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
```
