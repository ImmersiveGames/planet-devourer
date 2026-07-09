# F53D - Canonical Player Binding Authoring Facade

Status: Proposed Delta
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Create a FIRSTGAME editor-only authoring facade that centralizes the already-proven real player references:

- `PlayerActorDeclaration`
- `PlayerSlotDeclaration`
- `PlayerInput`
- `PlayerControlBindingTargetBehaviour`
- `UnityPlayerInputBridgeTargetBehaviour`
- `UnityPlayerInputActivationTargetBehaviour`
- `FrameworkCameraAnchorHost.trackingTarget`
- `FrameworkCameraAnchorHost.lookAtTarget`

The facade is not a runtime lifecycle, input router, movement system, actor spawner or save/progression entrypoint.

## Tools

```text
FIRSTGAME > Immersive Framework > Validate Canonical Player Binding Facade
FIRSTGAME > Immersive Framework > Apply Canonical Player Binding Facade
```

## Behavior

### Validate

Validates the active `FG_Gameplay` scene and reports whether the player binding chain is centralized and coherent.

It checks:

```text
identitySource = PlayerActorDeclaration+PlayerSlotDeclaration
resolvedByName = False
PlayerInput exists
InputActionAsset exists
Player action map exists
PlayerControlBindingTargetBehaviour exists
UnityPlayerInputBridgeTargetBehaviour references the resolved PlayerInput
UnityPlayerInputBridgeTargetBehaviour expected slot is player.1
UnityPlayerInputActivationTargetBehaviour references the resolved PlayerInput
UnityPlayerInputActivationTargetBehaviour expected slot is player.1
UnityPlayerInputActivationTargetBehaviour action map is Player
FrameworkCameraAnchorHost trackingTarget points to the resolved player transform
FrameworkCameraAnchorHost lookAtTarget points to the resolved player transform
```

### Apply

Applies only explicit editor authoring configuration:

```text
adds missing F52 target components to the resolved player object
sets bridge/activation PlayerInput references
sets expected slot id fields to player.1
sets activation action map to Player
sets camera anchor tracking/lookAt targets to the resolved player transform when the anchor host is unambiguous
```

It does not create a test player, create a `PlayerInput`, route actions, enable movement, spawn actors, execute gameplay commands or create runtime lifecycle.

## Expected Smoke

Open:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Run:

```text
FIRSTGAME > Immersive Framework > Validate Canonical Player Binding Facade
```

Expected:

```text
[F53D_FIRSTGAME_PLAYER_BINDING_FACADE] status='Succeeded' mode='validate'
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
playerInput='True'
inputActions='True'
expectedGameplayActionMapFound='True'
playerControlBindingTarget='True'
unityPlayerInputBridgeTarget='True'
unityPlayerInputBridgePlayerInputMatchesResolved='True'
unityPlayerInputBridgeExpectedSlotMatches='True'
unityPlayerInputActivationTarget='True'
unityPlayerInputActivationPlayerInputMatchesResolved='True'
unityPlayerInputActivationExpectedSlotMatches='True'
unityPlayerInputActivationActionMapMatchesExpected='True'
trackingTargetMatchesPlayer='True'
lookAtTargetMatchesPlayer='True'
typedTransformBinding='True'
targetIsPlayerRoot='True'
facadeCentralizedReferences='True'
movement='False'
actorSpawning='False'
gameplayCommandExecution='False'
failureReason='None'
```

If validation fails because a reference drifted, run:

```text
FIRSTGAME > Immersive Framework > Apply Canonical Player Binding Facade
```

Then run validation again.

## Acceptance Criteria

```text
1. No runtime code was added.
2. No package code was changed.
3. No scene or prefab asset is required in the delta.
4. The facade resolves the player through canonical declarations, not GameObject.name.
5. The facade centralizes F52 PlayerInput target references.
6. The facade centralizes camera anchor target references.
7. Missing or ambiguous camera anchor hosts fail explicitly.
8. No movement, input routing, actor spawning, gameplay command execution or save/progression is created.
```

## Architectural Gain

F53D turns the separate successful proofs from F53C1-F53C3 into one authoring surface. The FIRSTGAME player binding chain becomes easier to validate and repair without introducing runtime ownership or hidden lookup.
