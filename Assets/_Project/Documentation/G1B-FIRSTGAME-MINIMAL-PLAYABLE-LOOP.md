# G1B — FIRSTGAME Minimal Playable Loop

## Objective

Replace the fragmented technical gameplay presentation with one coherent, repeatable FIRSTGAME loop built on the official framework contracts already present in the project.

## Architectural boundary

```text
FIRSTGAME owns:
  local gameplay condition
  local state and presentation

Immersive Framework owns:
  Route / Activity lifecycle
  Player control availability
  Camera composition
  Pause / Resume
  Reset
  Activity Restart
```

No generic objective, victory, game-over, mission, interaction or global manager is introduced.

## Installed flow

```text
Menu
-> Gameplay Route
-> Startup Activity
-> Player receives control
-> Player crosses a small physical play space
-> Player reaches GoalTrigger
-> GoalCore changes position and color
-> Pause / Resume preserves the completed state
-> R requests the existing Object Reset Group
-> T requests Activity Restart
-> Reset restores the goal
-> framework reset restores the Player
-> the loop can be completed again
```

## Files

### Created

```text
Assets/_Project/Scripts/Gameplay/FirstGameMinimalLoopObjective.cs
Assets/_Project/Scripts/Gameplay/FirstGameMinimalLoopControls.cs
Assets/_Project/Scripts/Gameplay/Diagnostics/FirstGameG1BLoopProof.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameG1BGameplaySceneBuilder.cs
Assets/_Project/Documentation/G1B-FIRSTGAME-MINIMAL-PLAYABLE-LOOP.md
```

### Modified by the builder

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

## Install

1. Import this ZIP preserving folders.
2. Wait for Unity compilation.
3. Run:

```text
FIRSTGAME > Immersive Framework > Gameplay > G1B Rebuild Minimal Playable Loop
```

4. Enter Play Mode through the normal Menu -> Gameplay flow.

## Controls

```text
Configured Move input: move Player
Escape: existing Pause / Resume flow
R: Object Reset Group
T: Activity Restart
```

## Expected hierarchy addition

```text
G1B_MinimalPlayableLoop
├── Environment
├── Gameplay
│   ├── MinimalGoal
│   │   ├── GoalTrigger
│   │   └── GoalCore
│   └── LoopControls
├── Diagnostics
└── Instructions
```

## Cleanup performed by the builder

Known temporary fixtures are removed from the scene when present:

```text
TestProb
Button_ResetRoom
Button_RestartActivity
FirstGameP2GMinimalControlMovementProof
FirstGamePlayerResetProbe
previous G1B_MinimalPlayableLoop root
```

The canonical PlayerComposer, PlayerInput, game-owned mover, camera composition, bindings and framework runtime materialization are preserved.

## Expected logs

```text
[G1B_FIRSTGAME_GAMEPLAY_BUILDER] status='Succeeded'
[G1B_FIRSTGAME_MINIMAL_PLAYABLE_LOOP] status='Ready' failed='0'
[G1B_FIRSTGAME_LOOP] status='ObjectiveCompleted'
[G1B_FIRSTGAME_MINIMAL_PLAYABLE_LOOP] status='LoopCompleted'
[G1B_FIRSTGAME_LOOP] status='ObjectiveRestored'
[G1B_FIRSTGAME_MINIMAL_PLAYABLE_LOOP] status='LoopRestored'
```

## Acceptance

```text
compiles in FIRSTGAME
no package change
no QA change
one real PlayerComposer
one real PlayerInput
camera target remains valid
objective completion is visible
Pause / Resume preserves state
Reset restores objective and Player
Activity Restart restores the loop
loop can be completed again
```

## Suggested commit

```text
G1B — rebuild FIRSTGAME gameplay as a repeatable minimal loop
```
