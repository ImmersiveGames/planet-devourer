# F53A — FIRSTGAME Player Binding Usability Proof

## Status

Preflight proof delta, patched by F53A-B1.

## Objective

Prove that FIRSTGAME can consume the accepted Immersive Framework PlayerView/PlayerControl binding surfaces without creating new framework contracts in the game project.

## What this delta adds

```text
Assets/_Project/Scripts/FrameworkProof/FirstGame.FrameworkProof.Runtime.asmdef
Assets/_Project/Scripts/FrameworkProof/FirstGamePlayerBindingUsabilityProbe.cs
Assets/_Project/Scripts/Editor/FrameworkProof/FirstGame.FrameworkProof.Editor.asmdef
Assets/_Project/Scripts/Editor/FrameworkProof/FirstGamePlayerBindingUsabilityProbeCreator.cs
Assets/_Project/Documentation/F53A-PlayerBinding-Usability-Proof.md
```

## F53A-B1 strictness patch

The preflight status is now strict:

```text
expectedGameplayActionMap must be True for status='Succeeded'
```

The creator no longer creates a second `PlayerInput` on the temporary probe object. It reuses an existing scene `PlayerInput`, preferably one with the `Player` action map. This avoids pairing/control-scheme noise from a temporary duplicate `PlayerInput`.

## How to run

1. Import the delta into FIRSTGAME.
2. Wait for Unity recompilation.
3. Open a FIRSTGAME scene that already contains the real player `PlayerInput`.
4. Run:

```text
FIRSTGAME > Immersive Framework > Create Player Binding Usability Probe
```

5. Select the created GameObject.
6. Run the probe from the component context menu:

```text
Run FIRSTGAME Player Binding Preflight
```

Expected log:

```text
[F53A_FIRSTGAME_PLAYER_BINDING_PREFLIGHT] status='Succeeded'
```

The expected successful line must include:

```text
playerInput='True'
inputActions='True'
expectedGameplayActionMap='True'
movement='False'
actorSpawning='False'
gameplayCommandExecution='False'
```

## What this proves

```text
FIRSTGAME compiles against Immersive.Framework.PlayerBinding
FIRSTGAME compiles against UnityEngine.InputSystem.PlayerInput
FIRSTGAME can create explicit local proof components
FIRSTGAME can reference the real scene PlayerInput
FIRSTGAME can validate the expected gameplay action map
FIRSTGAME keeps movement false
FIRSTGAME keeps actor spawning false
FIRSTGAME keeps gameplay command execution false
```

## What this does not prove yet

```text
real production scene binding
real route/activity lifecycle integration
real gameplay control
movement
input-to-command mapping
actor spawning
```

## Boundary

F53A is a consumer-side preflight only. It must not be promoted into framework runtime behavior.
