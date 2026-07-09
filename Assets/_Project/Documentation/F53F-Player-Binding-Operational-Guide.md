# F53F - FIRSTGAME Player Binding Operational Guide

Status: Canonical after F53G cleanup
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Keep one operational guide for the accepted FIRSTGAME player binding workflow after F53C1-F53F. Historical proof docs and per-cut manifests were removed by F53G because their useful content is represented here and in the F53G cleanup manifest.

This guide does not define new runtime behavior. It only describes the current editor tooling and smoke flow.

## Canonical Identity

The real FIRSTGAME player is identified by declarations and typed references, not by `GameObject.name`.

```text
PlayerActorDeclaration.actorId = firstgame.player
PlayerSlotDeclaration.slotId = player.1
PlayerInput = explicit typed evidence
FrameworkCameraAnchorHost trackingTarget/lookAtTarget = player Transform
```

`PlayerPrototype` may appear in logs and Inspector labels. It is diagnostic text only.

Required evidence in successful logs:

```text
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
failureReason='None'
```

## Canonical Menus

All current tools live under:

```text
FIRSTGAME > Immersive Framework
```

### Validate Real Player Binding

Purpose: quick support smoke for the real player identity and F52 PlayerControl / Unity PlayerInput target chain.

Expected log:

```text
[F53B_FIRSTGAME_REAL_PLAYER_BINDING] status='Succeeded'
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
playerInput='True'
inputActions='True'
expectedGameplayActionMapFound='True'
playerControlBindingTarget='True'
unityPlayerInputBridgeTarget='True'
unityPlayerInputActivationTarget='True'
failureReason='None'
```

### Configure Route-Activity Camera

Purpose: configure the existing FIRSTGAME route/activity camera setup and assign typed `Transform` targets through `FrameworkCameraAnchorHost`.

Expected log evidence:

```text
[FIRSTGAME_CAMERA_SETUP]
Canonical FIRSTGAME player resolved
resolvedByName='False'
Route/Activity camera setup configured
```

### Validate Canonical Player Binding Facade

Purpose: canonical validation for identity, PlayerInput/F52 references and camera anchor references.

Expected log:

```text
[F53D_FIRSTGAME_PLAYER_BINDING_FACADE] status='Succeeded'
mode='validate'
resolvedByName='False'
facadeCentralizedReferences='True'
failureReason='None'
```

### Apply Canonical Player Binding Facade

Purpose: repair authoring drift by reapplying typed references and expected IDs/action-map values.

This tool may configure existing authoring components. It must not create gameplay behavior, runtime lifecycle, movement, actor spawning or save/progression.

### Run Canonical Player Binding Facade Repair Proof

Purpose: create controlled temporary drift, invoke the facade apply path, and prove the drift was repaired.

Expected log:

```text
[F53E_FIRSTGAME_PLAYER_BINDING_FACADE_REPAIR_PROOF] status='Succeeded'
repairSucceeded='True'
typedReferencesRepaired='True'
failureReason='None'
```

## Recommended Smoke

Open:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Run:

```text
1. FIRSTGAME > Immersive Framework > Validate Real Player Binding
2. FIRSTGAME > Immersive Framework > Configure Route-Activity Camera
3. FIRSTGAME > Immersive Framework > Validate Canonical Player Binding Facade
4. FIRSTGAME > Immersive Framework > Apply Canonical Player Binding Facade
5. FIRSTGAME > Immersive Framework > Run Canonical Player Binding Facade Repair Proof
6. FIRSTGAME > Immersive Framework > Validate Canonical Player Binding Facade
```

All successful logs must keep:

```text
resolvedByName='False'
failureReason='None'
```

## Manual Inspector Checks

In `FG_Gameplay`, inspect the real player object:

```text
PlayerPrototype
```

Expected canonical components:

```text
PlayerActorDeclaration
PlayerSlotDeclaration
PlayerInput
PlayerControlBindingTargetBehaviour
UnityPlayerInputBridgeTargetBehaviour
UnityPlayerInputActivationTargetBehaviour
```

Inspect:

```text
FirstGameCameraAnchors
```

Expected component and references:

```text
FrameworkCameraAnchorHost
trackingTarget = PlayerPrototype Transform
lookAtTarget = PlayerPrototype Transform
```

The visible object name is acceptable as an Inspector label. It is not a lookup key.

## Retired By F53G

The following intermediate tools were removed because the facade validation covers the same accepted evidence:

```text
Validate Real Player Camera Target
Validate Full Player Binding Chain
```

The following menus were removed because the facade apply/repair flow replaces them or the temporary assets no longer exist:

```text
Ensure Selected Player Binding Components
Cleanup F53A Preflight Proof Assets
```

## Explicitly Out Of Scope

Still not implemented by this sequence:

```text
movement
InputAction routing
gameplay command execution
actor spawning
save/progression
runtime lifecycle
formal PlayerView binding target contracts
formal PlayerView camera activation contracts
```

## Failure Interpretation

### `resolvedByName='True'`

Invalid. The player must be resolved by canonical declarations and typed components.

### `MissingPlayerActorDeclaration`

The real player no longer exposes the canonical actor declaration.

### `MissingPlayerSlotDeclaration`

The real player no longer exposes the canonical player slot declaration.

### `MissingPlayerInput`

The real player no longer exposes Unity `PlayerInput` evidence.

### `MissingInputActions`

`PlayerInput.actions` is missing.

### `MissingPlayerActionMap`

The expected `Player` action map was not found in the assigned `InputActionAsset`.

### `CameraAnchorTargetMismatch`

The camera anchor target drifted. Run:

```text
FIRSTGAME > Immersive Framework > Apply Canonical Player Binding Facade
```

Then validate again.

### `UnityPlayerInputBridgeReferenceMismatch`

The bridge target points to the wrong `PlayerInput` reference. Run the facade apply path.

### `UnityPlayerInputActivationActionMapMismatch`

The activation target action map drifted from `Player`. Run the facade apply path.

## Commit Message

```text
F53G: clean up FIRSTGAME player binding tooling
```
