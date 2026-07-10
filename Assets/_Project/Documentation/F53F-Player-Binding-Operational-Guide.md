# F53F - FIRSTGAME Player Binding Operational Guide

Status: Superseded by official PlayerComposer authoring flow
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Keep one operational guide for the accepted FIRSTGAME player binding evidence after F53C1-F53F. The active authoring workflow is now the official package `PlayerComposer`.

This guide does not define new runtime behavior. It only describes support evidence and points designers to the official Composer flow.

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

## Current Menus

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

### Configure Route-Activity Camera (Legacy / Diagnostic)

Purpose: compatibility diagnostic for the existing FIRSTGAME Route/Activity camera setup. It is not the primary Camera Product Surface; use the CameraComposer Proof below for current camera authoring.

Expected log evidence:

```text
[FIRSTGAME_CAMERA_SETUP]
Canonical FIRSTGAME player resolved
resolvedByName='False'
Route/Activity camera setup configured
```

### PlayerComposer Pilot

Purpose: configure the official package `PlayerComposer` intent on the selected `PlayerPrototype`, then use the official Inspector Validate and Apply/Rebuild actions.

## Recommended Smoke

Open:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Run:

```text
1. FIRSTGAME > Immersive Framework > Camera Composer Proof > Configure Gameplay CameraComposer Proof
2. FIRSTGAME > Immersive Framework > Player Composer Pilot > Configure Selected Player Composer
3. In the official PlayerComposer Inspector, run Validate
4. Run Apply/Rebuild
5. Run Apply/Rebuild again to confirm idempotence

Run `Configure Route-Activity Camera` only when explicitly validating legacy lifecycle compatibility.
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

The camera anchor target drifted. Use the official PlayerComposer flow:

```text
FIRSTGAME > Immersive Framework > Player Composer Pilot > Configure Selected Player Composer
PlayerComposer Inspector > Apply/Rebuild
```

Then validate again.

### `UnityPlayerInputBridgeReferenceMismatch`

The bridge target points to the wrong `PlayerInput` reference. Run the official PlayerComposer Apply/Rebuild path.

### `UnityPlayerInputActivationActionMapMismatch`

The activation target action map drifted from `Player`. Run the official PlayerComposer Apply/Rebuild path.

## Commit Message

```text
F53G: clean up FIRSTGAME player binding tooling
```
