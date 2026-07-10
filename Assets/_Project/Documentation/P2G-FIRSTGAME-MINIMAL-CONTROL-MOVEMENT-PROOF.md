# P2G — FIRSTGAME Minimal Control and Movement Proof

## Objective

Prove in the real FIRSTGAME consumer that the framework-owned control availability reaches a game-owned movement consumer.

## Type

Real integration / product usability proof.

## Scope

```text
FIRSTGAME real FG_Gameplay scene
official PlayerComposer
real PlayerInput
official UnityPlayerInputGateAdapter
game-owned FirstGamePlayerMover
real user Move input
real PlayerPrototype displacement
```

## Out of scope

```text
framework-owned movement
input injection
PlayerInputManager
generic spawn/join
new runtime context
new binding authority
package changes
QAFramework changes
```

## Files created

```text
Assets/_Project/Scripts/Player/Diagnostics/FirstGameP2GMinimalControlMovementProof.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameP2GMinimalControlMovementProofInstaller.cs
Assets/_Project/Documentation/P2G-FIRSTGAME-MINIMAL-CONTROL-MOVEMENT-PROOF.md
```

The installer modifies and saves:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

only after the user runs the menu successfully.

## Product surface affected

```text
PlayerComposer real consumer flow
Player control availability
game-owned movement boundary
```

## Expected use flow

1. Import the delta into FIRSTGAME.
2. Run:

```text
FIRSTGAME > Immersive Framework > Player > P2G Install Minimal Control Movement Proof
```

3. Enter Play Mode through the normal FIRSTGAME Menu -> Gameplay flow.
4. Wait for:

```text
[P2G_FIRSTGAME_MINIMAL_CONTROL_MOVEMENT] status='AwaitingInput'
```

5. Use the configured Move controls before the observation window ends.

## Expected smoke

```text
[P2G_FIRSTGAME_MINIMAL_CONTROL_MOVEMENT]
status='Succeeded'
passed='11'
failed='0'
cases='11'
```

## Technical acceptance

```text
real PlayerComposer resolved uniquely
real PlayerInput reused
real UnityPlayerInputGateAdapter reused
real FirstGamePlayerMover referenced explicitly
entry Transition Gate releases
Player action map is enabled
Move action is observed
PlayerPrototype displacement is observed
exactly one PlayerInput exists in gameplay scene
no package or QA change
```

## Product acceptance

```text
the real game player can be controlled
movement remains game-owned
framework Gate controls availability without owning movement
the proof runs through the normal FIRSTGAME flow
```

## Architectural gain

Closes the boundary:

```text
Framework:
  authoring + PlayerInput reference + Gate availability

FIRSTGAME:
  Move action semantics + movement execution
```

## Usability gain

Proves the official Player product surface can support a minimally playable real consumer without requiring a framework character controller.

## Suggested commit

```text
P2G — prove FIRSTGAME minimal control and game-owned movement
```
