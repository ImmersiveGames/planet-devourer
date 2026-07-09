# F53E - FIRSTGAME Player Binding Facade Apply/Repair Proof

Status: Ready for Smoke
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Prove that the FIRSTGAME canonical player binding authoring facade can repair controlled serialized reference drift without adding runtime behavior.

The proof is editor-only and uses the real FIRSTGAME player already accepted in F53C1-F53D.

## Scope

Created:

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameCanonicalPlayerBindingFacadeRepairProof.cs
```

The new tool is:

```text
FIRSTGAME > Immersive Framework > Run Canonical Player Binding Facade Repair Proof
```

## What the proof does

The tool:

1. Requires the active scene to be `Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity`.
2. Resolves the real player through `FirstGamePlayerIdentityResolver`.
3. Requires the current state to be canonical before drifting anything.
4. Creates controlled temporary drift in:
   - `UnityPlayerInputBridgeTargetBehaviour.playerInput`
   - `UnityPlayerInputBridgeTargetBehaviour.expectedPlayerSlotId`
   - `UnityPlayerInputActivationTargetBehaviour.playerInput`
   - `UnityPlayerInputActivationTargetBehaviour.expectedPlayerSlotId`
   - `UnityPlayerInputActivationTargetBehaviour.actionMapName`
   - `FrameworkCameraAnchorHost.trackingTarget`
   - `FrameworkCameraAnchorHost.lookAtTarget`
5. Invokes `FirstGameCanonicalPlayerBindingAuthoringFacade.ApplyCanonicalPlayerBindingFacade()`.
6. Validates that the references were repaired to the canonical real player chain.
7. Restores the original canonical state if repair fails or an exception occurs.

## Out of scope

The proof does not implement:

```text
runtime lifecycle
movement
InputAction routing
gameplay command execution
actor spawning
save/progression
formal PlayerView contracts
new package contracts
```

It also does not create test objects or create a new `PlayerInput`.

## Expected smoke

Open:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Run:

```text
FIRSTGAME > Immersive Framework > Run Canonical Player Binding Facade Repair Proof
```

Expected log:

```text
[F53E_FIRSTGAME_PLAYER_BINDING_FACADE_REPAIR_PROOF] status='Succeeded'
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
initialCanonical='True'
controlledDriftCreated='True'
facadeApplyInvoked='True'
repairSucceeded='True'
typedReferencesRepaired='True'
createdPlayerControlBindingTarget='False'
createdUnityPlayerInputBridgeTarget='False'
createdUnityPlayerInputActivationTarget='False'
createdTestObject='False'
createdPlayerInput='False'
movement='False'
actorSpawning='False'
gameplayCommandExecution='False'
failureReason='None'
```

The facade apply tool also logs its own `[F53D_FIRSTGAME_PLAYER_BINDING_FACADE]` line during the proof. That is expected because F53E is explicitly proving the apply path.

## Acceptance criteria

```text
1. Project compiles.
2. F53E repair proof succeeds.
3. Controlled drift is created by the proof.
4. Facade apply is invoked.
5. References are repaired to the canonical player.
6. No runtime lifecycle, movement, spawning, gameplay execution or save/progression is introduced.
7. No test object or PlayerInput is created.
8. If repair fails, original references are restored.
```

## Architectural gain

F53D proved the facade can validate centralized references. F53E proves the same facade can repair controlled drift safely, making the authoring tool useful for real project maintenance instead of being only a passive validator.

## Suggested commit message

```text
F53E: prove canonical player binding facade repair path
```
