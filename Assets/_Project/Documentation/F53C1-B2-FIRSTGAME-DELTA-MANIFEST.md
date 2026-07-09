# F53C1-B2 FIRSTGAME Delta Manifest

Status: Hotfix
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Fix the compile break introduced by F53C1-B1 in `FirstGameCameraCutSetup.cs` when the GameCamera editor assembly does not reference `Unity.InputSystem` directly.

## Changed

```text
Assets/_Project/Scripts/Editor/GameCamera/FirstGameCameraCutSetup.cs
```

## Created

```text
Assets/_Project/Documentation/F53C1-B2-FIRSTGAME-DELTA-MANIFEST.md
```

## Removed

```text
None
```

## Notes

- Removed direct `using UnityEngine.InputSystem` from the camera setup file.
- Removed direct compile-time use of the `PlayerInput` type from the camera setup file.
- Kept canonical player resolution based on `PlayerActorDeclaration`, `PlayerSlotDeclaration` and PlayerInput component evidence.
- PlayerInput evidence is detected editor-only as a `Component` by its runtime type name, avoiding a new asmdef dependency from GameCamera to Input System.
- The player object is still not resolved by `GameObject.name`.
- Input action map validation remains stricter in `FirstGameRealPlayerBindingValidator`, which already compiles in the integration editor assembly.

## Expected Smoke

```text
1. Unity compile succeeds.
2. Run: FIRSTGAME > Immersive Framework > Validate Real Player Binding
3. Run: Tools > FIRSTGAME > Camera > Configure Route-Activity Camera
4. Confirm camera setup resolves player with resolvedByName='False'.
```
