# F53B — FIRSTGAME Real Player Binding Wiring

## Goal

Validate the accepted framework PlayerControl / Unity PlayerInput chain on the real FIRSTGAME player object instead of keeping a synthetic proof object.

## Canonical player object

```text
PlayerPrototype
```

Expected components on the same object:

```text
PlayerInput
PlayerControlBindingTargetBehaviour
UnityPlayerInputBridgeTargetBehaviour
UnityPlayerInputActivationTargetBehaviour
```

Expected input action map:

```text
Player
```

Expected player slot:

```text
player.1
```

## Menus

### Validate real wiring

```text
FIRSTGAME > Immersive Framework > Validate Real Player Binding
```

This only validates the current scene. It does not create objects, create PlayerInput, save the scene, read InputActions, enable movement or run gameplay.

Expected log:

```text
[F53B_FIRSTGAME_REAL_PLAYER_BINDING] status='Succeeded'
```

### Ensure selected real player wiring

```text
FIRSTGAME > Immersive Framework > Ensure Selected Player Binding Components
```

Use this with `PlayerPrototype` selected. It adds/configures only the accepted F52 framework target components on the real player object. It does not create a test object and does not save the scene automatically.

### Cleanup old F53A proof assets

```text
FIRSTGAME > Immersive Framework > Cleanup F53A Preflight Proof Assets
```

This removes the temporary preflight proof files created for F53A:

```text
Assets/_Project/Scripts/FrameworkProof
Assets/_Project/Scripts/Editor/FrameworkProof
Assets/_Project/Documentation/F53A-PlayerBinding-Usability-Proof.md
```

## Boundary

F53B does not implement movement or gameplay.

```text
createdTestObject='False'
createdPlayerInput='False'
movement='False'
actorSpawning='False'
gameplayCommandExecution='False'
```
