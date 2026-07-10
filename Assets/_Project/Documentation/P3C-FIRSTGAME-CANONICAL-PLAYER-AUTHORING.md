# P3C — FIRSTGAME Canonical Player Authoring

## Objective

Create the real FIRSTGAME Player from a clean Gameplay scene using the official `PlayerRecipe` and `PlayerComposer` product flow.

## Type

Real integration / product UX.

## Scope

- create or update the FIRSTGAME `PlayerRecipe`;
- create exactly one logical Player object;
- require and configure `PlayerInput`;
- configure `PlayerComposer`;
- run the official Apply/Rebuild;
- generate camera anchors as children of the Player;
- create a visible local prototype;
- add a `CharacterController` for the next movement cut.

## Out of scope

- movement execution;
- runtime action-map switching proof;
- CameraComposer;
- Cinemachine Follow/LookAt;
- Reset participation;
- goal/gameplay loop.

## Files created

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameP3CPlayerAuthoringInstaller.cs
Assets/_Project/Documentation/P3C-FIRSTGAME-CANONICAL-PLAYER-AUTHORING.md
```

The installer creates or updates:

```text
Assets/_Project/Settings/Player/FG_PlayerRecipe.asset
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

## Usage

1. Open and save `FG_Gameplay`.
2. Run:

```text
FIRSTGAME
  Immersive Framework
    Player
      P3C Build Canonical Player
```

3. Save the scene after inspecting the result.

## Expected hierarchy

```text
PlayerPrototype
├── Visual
├── Anchors
│   ├── CameraTarget
│   └── LookAtTarget
└── technical materialization created by PlayerComposer
```

Expected root components include:

```text
PlayerInput
PlayerComposer
PlayerActorDeclaration
PlayerSlotDeclaration
UnityPlayerInputGateAdapter
CharacterController
```

## Expected log

```text
[P3C_FIRSTGAME_PLAYER_AUTHORING] status='Succeeded'
```

## Product authority

```text
FG_PlayerRecipe
  -> reusable defaults

PlayerComposer
  -> effective scene-instance authoring authority

Apply/Rebuild
  -> typed declarations, Gate and camera anchors

FIRSTGAME
  -> visible prototype and later movement implementation
```

## Acceptance

- active scene is explicitly `FG_Gameplay`;
- exactly one InputActionAsset contains the `Player` map;
- exactly one PlayerComposer exists after execution;
- PlayerInput points to the real action asset;
- authored map is `Player`;
- camera anchors are generated under the Player;
- second execution is idempotent;
- no movement, camera runtime or Reset behavior is implied as complete.

## Suggested commit

```text
P3C — create canonical FIRSTGAME Player authoring
```
