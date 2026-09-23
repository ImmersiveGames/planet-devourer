# Application

Owns application/session-wide authority and persistent composition.

## Planned shape

```text
Application/
├── GameApplication/
├── Session/
└── Persistent/
    ├── Scenes/
    └── Prefabs/
```

Expected first authoritative assets:

```text
GameApplication/GameApplication_PlanetDevourer.asset
Session/PlayerSessionProfile_PlanetDevourer.asset
Persistent/Scenes/<persistent-content-scene>.unity
```

Create these from the current official Framework product surfaces.
