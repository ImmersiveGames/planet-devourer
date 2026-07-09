# F53C1 - FIRSTGAME Player Identity Canonicalization

Status: Implemented delta
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Remove the remaining editor-only lookup of the real FIRSTGAME player by `GameObject.name` and resolve the real player through canonical typed components:

```text
PlayerActorDeclaration
PlayerSlotDeclaration
PlayerInput
```

This cut does not create runtime lifecycle, movement, gameplay command execution, actor spawning, save/progression, a final facade, or a PlayerView proof.

## Decision

`GameObject.name` remains allowed only as diagnostic text. It is no longer an accepted mechanism for resolving the real FIRSTGAME player in editor validation or camera setup.

Canonical identity is now resolved as:

```text
identitySource = PlayerActorDeclaration+PlayerSlotDeclaration
actorId = Actor:firstgame.player or firstgame.player
playerSlotId = PlayerSlot:player.1 or player.1
resolvedByName = False
```

The resolver accepts Unity's domain-prefixed diagnostic `ToString()` output and the raw authored value because framework identity structs may print diagnostic prefixes while serialized authoring stores raw IDs.

## Files Changed

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameRealPlayerBindingValidator.cs
Assets/_Project/Scripts/Editor/GameCamera/FirstGameCameraCutSetup.cs
Assets/_Project/Documentation/F53C0-Player-Identity-Typed-Binding-Audit.md
```

## Files Created

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGamePlayerIdentityResolver.cs
Assets/_Project/Documentation/F53C1-Player-Identity-Canonicalization.md
Assets/_Project/Documentation/F53C1-FIRSTGAME-DELTA-MANIFEST.md
```

## Resolver Behavior

Priority:

```text
1. Selected object, only if it contains a coherent PlayerActorDeclaration/PlayerSlotDeclaration/PlayerInput set in the target scene.
2. Unique PlayerActorDeclaration with actorId firstgame.player.
3. Unique PlayerSlotDeclaration with slotId player.1.
4. Unique PlayerInput with coherent canonical declarations.
5. Explicit failure if none or multiple candidates exist.
```

Failure is explicit for:

```text
InvalidScene
MissingPlayerCandidateGameObject
MissingPlayerActorDeclaration
MissingPlayerSlotDeclaration
MissingPlayerInput
MissingInputActions
MissingPlayerActionMap
UnexpectedPlayerIdentity
DivergentPlayerActorAndSlotDeclaration
MultipleMatchingPlayerActorDeclarations
MultipleMatchingPlayerSlotDeclarations
MultipleCoherentPlayerInputCandidates
NoCoherentPlayerInputCandidate
```

No fallback to `PlayerPrototype` or any object name exists.

## Validator Change

`FIRSTGAME/Immersive Framework/Validate Real Player Binding` now logs identity evidence in addition to the existing F53B proof fields:

```text
[F53B_FIRSTGAME_REAL_PLAYER_BINDING]
status='Succeeded'
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
canonicalPlayerObject='True'
playerInput='True'
expectedGameplayActionMapFound='True'
movement='False'
actorSpawning='False'
gameplayCommandExecution='False'
failureReason='None'
```

`canonicalPlayerObject` is retained for compatibility with the existing F53B log shape, but its meaning is now canonical player identity, not object-name equality.

## Camera Setup Change

`FirstGameCameraCutSetup` no longer calls:

```csharp
FindInScene(scene, "PlayerPrototype")
```

for player target resolution.

It resolves the player with `FirstGamePlayerIdentityResolver` and assigns typed transform references:

```text
FrameworkCameraAnchorHost.trackingTarget = resolved player Transform
FrameworkCameraAnchorHost.lookAtTarget = resolved player Transform
```

If the real player cannot be resolved, camera setup fails with a blocking diagnostic instead of leaving anchors unset silently.

## Smoke Expected

1. Open `Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity`.
2. Run `FIRSTGAME > Immersive Framework > Validate Real Player Binding` with or without the real player selected.
3. Expected log contains:

```text
[F53B_FIRSTGAME_REAL_PLAYER_BINDING] status='Succeeded'
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
playerInput='True'
expectedGameplayActionMapFound='True'
movement='False'
actorSpawning='False'
gameplayCommandExecution='False'
failureReason='None'
```

4. Run `Tools > FIRSTGAME > Camera > Configure Route-Activity Camera`.
5. Expected camera setup log contains:

```text
[FIRSTGAME_CAMERA_SETUP] Canonical FIRSTGAME player resolved.
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
```

6. Confirm `FirstGameCameraAnchors.FrameworkCameraAnchorHost.trackingTarget/lookAtTarget` point to the resolved player transform.

## Acceptance Criteria

```text
1. No runtime code was created.
2. No movement, input routing, gameplay command execution, actor spawning or save/progression was created.
3. No temporary FIRSTGAME probe object was created.
4. Real player is resolved by PlayerActorDeclaration/PlayerSlotDeclaration/PlayerInput.
5. GameObject.name is not required for lookup.
6. Camera setup does not use FindInScene(scene, "PlayerPrototype") for player target resolution.
7. F53B validation remains PASS.
8. Ambiguity fails explicitly.
9. Manifest lists created/changed/removed files.
```

## Architectural Gain

This closes the last fragile FIRSTGAME editor path where a player rename could produce a false validation or silently break camera target repair. The game now proves the same boundary expected from the future authoring facade: object names are labels; declarations and typed references carry identity.

## Suggested Commit Message

```text
F53C1: canonicalize real player identity lookup
```
