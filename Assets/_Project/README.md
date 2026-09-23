# Planet Devourer — Project Structure

FIRSTGAME is a real consumer of `com.immersive.framework`.

## Ownership model

```text
Application
  application/session-wide configuration and persistent composition

World
  Routes, Activities, world scenes and world-specific prefabs

Player
  Session Player configuration, Slots, Actors, provisioning, input and Player-specific UI

Gameplay
  game-owned mechanics, interactions and runtime systems

Presentation
  Camera, UI and Audio composition

Persistence
  Preferences and Progression owned by the game

Content
  presentation/source assets with no runtime authority

Development
  diagnostics and editor helpers for the consumer project

Settings
  project-level configuration, including Immersive Framework settings

Documentation
  integration notes, decisions and project guidance
```

## Rules

- one canonical FIRSTGAME `GameApplication`;
- no DemoXX / MXX ownership;
- no local copies of Framework contracts;
- no hidden global manager/service locator;
- no silent fallback;
- Camera and Audio are transversal;
- persistent/runtime authority must remain explicit;
- technical helpers belong in `Development`;
- presentation/source assets belong in `Content`;
- Git history is the backup for anything deleted from the previous FIRSTGAME.
