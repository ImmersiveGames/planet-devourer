# F53C3 - FIRSTGAME Full Player Binding Chain Proof

Status: Ready for smoke
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Prove the accepted FIRSTGAME real player binding chain in one editor-only validator:

```text
canonical player identity
F52 PlayerControl / Unity PlayerInput bridge / Unity PlayerInput activation evidence
current route/activity camera target binding
```

## What this cut validates

Menu:

```text
FIRSTGAME > Immersive Framework > Validate Full Player Binding Chain
```

Expected proof chain:

```text
PlayerActorDeclaration.actorId = firstgame.player
PlayerSlotDeclaration.slotId = player.1
PlayerInput exists
PlayerInput.actions exists
Player action map exists
PlayerControlBindingTargetBehaviour exists
UnityPlayerInputBridgeTargetBehaviour exists and references the resolved PlayerInput
UnityPlayerInputBridgeTargetBehaviour.expectedPlayerSlotId = player.1
UnityPlayerInputActivationTargetBehaviour exists and references the resolved PlayerInput
UnityPlayerInputActivationTargetBehaviour.expectedPlayerSlotId = player.1
UnityPlayerInputActivationTargetBehaviour.actionMapName = Player
FrameworkCameraAnchorHost.trackingTarget = resolved player Transform
FrameworkCameraAnchorHost.lookAtTarget = resolved player Transform
```

## Out of scope

This cut does not implement:

```text
movement
InputAction routing
gameplay command execution
actor spawning
save/progression
runtime lifecycle
formal PlayerView contracts
new package runtime contracts
```

## Expected smoke

Open:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Run:

```text
FIRSTGAME > Immersive Framework > Validate Full Player Binding Chain
```

Expected log:

```text
[F53C3_FIRSTGAME_FULL_PLAYER_BINDING_CHAIN] status='Succeeded'
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
formalPlayerViewBindingTargets='False'
movement='False'
actorSpawning='False'
gameplayCommandExecution='False'
failureReason='None'
```

## Notes

`formalPlayerViewBindingTargets='False'` is expected. F53C3 consolidates the current accepted player binding proof; it does not create the future formal PlayerView target contracts.

## Acceptance criteria

```text
1. No runtime component was created.
2. No gameplay, movement, input routing or save/progression was created.
3. The proof resolves the real player from canonical declarations, not GameObject.name.
4. F52 player binding components are present and internally coherent.
5. Bridge and activation targets reference the resolved PlayerInput.
6. Activation target points to the expected Player action map.
7. Camera anchor targets point to the resolved player Transform.
8. Formal PlayerView targets remain explicitly not implemented in this cut.
9. Failures are explicit and diagnostic.
```
