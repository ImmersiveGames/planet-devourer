# C6 — FIRSTGAME CameraComposer Proof Manifest

Status: consumer proof setup
Date: 2026-07-09
Scope: FIRSTGAME / `Assets/_Project`

## Objective

Prove the official Camera Product Surface in the real consumer project by configuring a real FIRSTGAME gameplay camera through `CameraComposer` and the real `PlayerComposer` on the player.

## Scope

```text
FIRSTGAME editor-only proof helper
FIRSTGAME documentation
real scene validation instructions
```

## Out of scope

```text
com.immersive.framework package changes
QAFramework changes
new framework contracts
CameraManager / singleton / service locator
Camera.main fallback
multiplayer camera
Route/Activity camera rewrite
```

## Files created

```text
Assets/_Project/Scripts/Editor/Camera/FirstGameCameraComposerProof.cs
Assets/_Project/Scripts/Editor/Camera/FirstGameCameraComposerProof.cs.meta
Assets/_Project/Documentation/FIRSTGAME-CAMERA-COMPOSER-PROOF.md
Assets/_Project/Documentation/FIRSTGAME-CAMERA-COMPOSER-PROOF.md.meta
Assets/_Project/Documentation/C6-FIRSTGAME-CAMERA-COMPOSER-PROOF-MANIFEST.md
Assets/_Project/Documentation/C6-FIRSTGAME-CAMERA-COMPOSER-PROOF-MANIFEST.md.meta
```

## Files changed

```text
none in the delta
```

The proof tool modifies and saves `Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity` only when the user runs the proof menu successfully in Unity.

## Files removed

```text
none
```

## Product surface affected

```text
CameraRecipe / CameraComposer
PlayerComposer camera anchors
Cinemachine rig materialization
```

## Flow of use expected

```text
1. Open Unity in FIRSTGAME.
2. Run FIRSTGAME > Immersive Framework > Camera Composer Proof > Configure Gameplay CameraComposer Proof.
3. Inspect FIRSTGAME_CameraComposerRig.
4. Verify the official CameraComposer references the real PlayerComposer.
5. Verify the CinemachineCamera follows CameraTarget and looks at LookAtTarget.
```

## Manual proof expected

Expected proof log:

```text
[FIRSTGAME][CameraComposerProof] status='Succeeded' ... createdSecond='0' blockedSecond='0' resolvedByName='False'
```

Expected package log:

```text
[Immersive.Framework][CameraComposer] Apply/Rebuild completed. camera='FIRSTGAME_CameraComposerRig' mode='SinglePlayerFollowCamera' source='PlayerComposer' ... blocked='0'
```

## Technical acceptance criteria

```text
FIRSTGAME compiles.
Proof menu appears.
Proof resolves exactly one PlayerComposer by typed ActorId/PlayerSlotId.
Proof resolves gameplay Unity Camera through CinemachineBrain evidence.
CameraComposer references the real PlayerComposer explicitly.
CameraComposer resolves CameraTarget and LookAtTarget.
Apply/Rebuild succeeds twice.
Second Apply/Rebuild has created=0 and blocked=0.
No Camera.main fallback is used.
No global manager/singleton/service locator is introduced.
Package and QAFramework are unchanged.
```

## Product acceptance criteria

```text
A real FIRSTGAME scene can use the official CameraComposer.
The designer can inspect the camera intent on a CameraComposer.
The PlayerPrototype provides the camera targets.
The CameraComposer consumes the targets explicitly.
The Cinemachine rig is visible and debuggable.
Old local camera setup is no longer the product proof path.
```

## Architectural gain

Moves FIRSTGAME camera proof from local/legacy camera setup toward the official package Camera Product Surface.

## Usability gain

A user can configure and inspect the real gameplay camera through a product-level Composer instead of low-level camera bindings.

## Risks

```text
The proof tool must be run once to modify and save FG_Gameplay.unity.
If multiple CinemachineBrain components exist, the proof blocks instead of guessing.
If multiple PlayerComposers share the same typed identity, the proof blocks instead of guessing.
```

## Suggested commit message

```text
FirstGame: prove CameraComposer with PlayerPrototype
```
