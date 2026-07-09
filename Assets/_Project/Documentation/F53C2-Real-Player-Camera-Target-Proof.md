# F53C2 - FIRSTGAME Real Player Camera Target Proof

Status: Ready for smoke  
Date: 2026-07-09  
Scope: `Assets/_Project`

## Objective

Prove the current FIRSTGAME real-player camera target binding without creating new runtime lifecycle, movement, actor spawning, gameplay command execution or save/progression.

This cut validates the camera path that exists today:

```text
PlayerActorDeclaration + PlayerSlotDeclaration + PlayerInput
  -> resolved real player Transform
  -> FrameworkCameraAnchorHost.trackingTarget
  -> FrameworkCameraAnchorHost.lookAtTarget
```

## Scope

Created an editor-only validator:

```text
FIRSTGAME > Immersive Framework > Validate Real Player Camera Target
```

The validator checks:

```text
1. The active scene is FG_Gameplay.
2. The real player resolves through FirstGamePlayerIdentityResolver.
3. Resolution remains declaration-based and resolvedByName=False.
4. A FrameworkCameraAnchorHost exists in the scene.
5. trackingTarget points to the resolved player Transform.
6. lookAtTarget points to the resolved player Transform.
7. The proof does not create test objects, PlayerInput, movement, actor spawning or gameplay command execution.
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
facade final
new runtime contracts
formal PlayerView binding target contracts
```

## Important boundary

The current package does not provide the formal PlayerView target components named in the F53C0 audit:

```text
PlayerViewBindingTargetBehaviour
PlayerViewCameraTargetBindingTargetBehaviour
PlayerViewCameraActivationTargetBehaviour
```

Therefore F53C2 does not invent them in FIRSTGAME. It proves the real camera target path that is already accepted: `FrameworkCameraAnchorHost` with typed `Transform` references.

A future package cut should introduce formal PlayerView contracts if that model is still desired.

## Expected smoke

Open:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Run:

```text
FIRSTGAME > Immersive Framework > Validate Real Player Camera Target
```

Expected:

```text
[F53C2_FIRSTGAME_REAL_PLAYER_CAMERA_TARGET] status='Succeeded'
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
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

Then run:

```text
FIRSTGAME > Immersive Framework > Validate Real Player Binding
```

Expected: existing F53B validation still succeeds.

## Acceptance criteria

```text
1. No runtime code was created.
2. No movement, input routing, gameplay command execution, actor spawning or save/progression was created.
3. No temporary FIRSTGAME probe object was created.
4. Camera target proof resolves the real player through declarations, not GameObject.name.
5. trackingTarget and lookAtTarget are typed Transform references.
6. Both references point to the resolved real player Transform.
7. F53B real player binding validation continues to pass.
8. The lack of formal PlayerView target contracts is documented rather than hidden.
```

## Architectural gain

F53C2 separates the currently valid FIRSTGAME camera proof from a future PlayerView contract design. The real game is now proven to have a declaration-resolved player and typed camera target references, while the framework avoids premature PlayerView runtime contracts.
