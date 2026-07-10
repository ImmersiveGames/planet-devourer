# P3D — FIRSTGAME Test Movement

## Decision

Movement remains game-owned.

The Immersive Framework only controls whether the `Player` action map is available through `UnityPlayerInputGateAdapter`.

The FIRSTGAME test component:

```text
PlayerInput Player/Move
  -> FirstGameTestPlayerMovement
    -> CharacterController.Move
```

## Files

```text
Assets/_Project/Scripts/Gameplay/FirstGameTestPlayerMovement.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameP3DTestMovementInstaller.cs
Assets/_Project/Documentation/P3D-FIRSTGAME-TEST-MOVEMENT.md
```

## Manual setup

Select `PlayerPrototype` and add:

```text
FIRSTGAME > Gameplay > Test Player Movement
```

Or run:

```text
FIRSTGAME
  Immersive Framework
    Player
      P3D Add Test Movement
```

Required root components:

```text
PlayerInput
CharacterController
FirstGameTestPlayerMovement
```

Default configuration:

```text
Action Map Name: Player
Move Action Name: Move
Move Speed: 5
Gravity: 20
Face Movement Direction: true
Rotation Speed: 720
```

## Expected behavior

Before the framework releases the Gate:

```text
Player/Move disabled
movement input ignored
gravity still applied
```

After Activity readiness and Gate release:

```text
Player/Move enabled
WASD / configured bindings move the CharacterController
```

## Out of scope

- framework movement API;
- reusable movement contracts;
- camera-relative movement;
- jumping;
- animation;
- acceleration;
- gameplay-grade locomotion.

## Suggested commit

```text
P3D — add FIRSTGAME test movement
```
