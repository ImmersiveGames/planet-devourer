# FIRSTGAME — Immersive Framework consumer demo

Status: Active integration demo  
Last updated: 2026-07-28

`planet-devourer` is the real-game consumer of `com.immersive.framework`.

The current project is intentionally being assembled in a developer-realistic order. The menu exposes focused Routes and scenes so one framework composition can be understood and validated at a time. These test entries are development UX; they are not the final public game menu.

## Source-of-truth rule

Use the current Git state of these repositories:

```text
Framework / product
  ImmersiveGames/com.immersive.framework

Technical QA
  rinnocenti/QAFramework

Real consumer
  ImmersiveGames/planet-devourer
```

Do not restore legacy setup scripts, copy QA fixtures, copy serialized QA assets, or create local framework facades.

## Current application composition

The current Build Settings contain:

```text
Assets/_Project/Scenes/Menu/FG_UIGlobal.unity
Assets/_Project/Scenes/Menu/FG_Menu.unity
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
Assets/_Project/Scenes/System/Conteiner Scene.unity
Assets/_Project/Scenes/Gameplay/SceneProvidedGameplay.unity
```

`FG_GameApplication.asset` currently declares:

```text
startup Route
four ordered local Player Slot Profiles
Actor duplicate-selection policy
Conteiner Scene as Persistent Content
```

## Current development entry points

The menu currently exposes two Git-visible Route requests:

```text
Start Game
  reason: firstgame.start.game
  opens the general gameplay path

Player Local Test
  reason: firstgame.playerlocal.test
  opens FG_PlayerSceneProvider
  primary scene: SceneProvidedGameplay
```

The focused Player path currently uses:

```text
Actor_PlayerSceneProvided.prefab
Player_SceneProvided.prefab
Player_SceneProvided_With_Pause.prefab
Player_SceneProvided_With_Camera.prefab
```

## Current assembly sequence

```text
Game Application and Routes
-> Persistent Content
-> persistent Camera Output
-> Pause outside the Player
-> Pause bound to the Player
-> Scene-Provided Logical Player
-> Player gameplay Camera
-> Manager-Provisioned Logical Player
-> Session-Persistent Logical Player
-> real gameplay vertical slice
```

The first six items have Git-visible assets or implementation. The Scene-Provided Player with Camera remains the current integration-validation focus. Manager-Provisioned is the next consumer assembly. Session-Persistent is architecturally accepted but is not yet an available package product surface.

## Status vocabulary

```text
Present in Git
  Serialized assets or code exist in the current repository.

Authoring Ready
  Required authoring references and stored validation evidence exist.

Runtime Implemented
  The package contains the runtime path.

Manual Proof Required
  A Unity Play Mode result must still be executed and recorded.

Blocked by Framework
  The accepted product shape has no official runtime/authoring surface yet.
```

Do not use `Passed` unless the corresponding manual or automated evidence is recorded.

## Documentation

- [Current State](Documentation/FIRSTGAME-CURRENT-STATE.md)
- [Test Scenarios](Documentation/TEST-SCENARIOS.md)
- [Player Variants](Documentation/PLAYER-VARIANTS.md)
- [Historical P3 Baseline](Documentation/FIRSTGAME-P3-MANUAL-INTEGRATION-BASELINE.md)

## Immediate next action

Close the Scene-Provided Player with Camera scenario by recording Play Mode evidence for:

```text
Route entry
Slot admission
existing Actor adoption
movement eligibility
Camera request/output
Player Pause
application-only Pause
Route exit and release
second entry without duplication
```

Then create a separate Manager-Provisioned Route and scene using only the current package surfaces.
