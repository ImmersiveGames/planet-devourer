# Immersive Framework Samples — Authoring Workspace

This folder is intentionally named `_Sample/`.

## Current operational location

```text
Repository
  ImmersiveGames/planet-devourer

Branch
  main

Authoring root
  Assets/_Sample/
```

`Assets/_Sample/` is the **visible development and proving workspace** for creating and editing the official Immersive Framework samples in Unity.

```text
Assets/_Sample/
  authoring/proving workspace
  visible in Project Browser
  normal Asset Database participation
  current operational tree for sample construction
```

Final official package distribution belongs to:

```text
com.immersive.framework/
  Samples~/
```

Do not ship `_Sample/` as the final UPM sample root.

## Current construction status

```text
Getting Started
  Minimal Game
    AUTHORING COMPLETE
    PLAY MODE PROVEN
    UPM PROMOTION PENDING

Game Flow
  next implementation area

Player
  planned after/alongside the sample-program sequence

Advanced Context
  planned

Persistence
  planned
```

`AUTHORING COMPLETE` means the configured sample and its consumer-facing runtime behavior are materialized and proven in the visible workspace.

It does not replace the final release gate:

```text
promote group into com.immersive.framework/Samples~
  -> Package Manager Import
  -> validate imported references
  -> validate imported Play Mode
```

## Documentation authority

Sample strategy:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-001-Immersive-Framework-Sample-and-Demonstration-Strategy.md
```

Operational sample guide/status:

```text
Assets/Documentation~/Architecture/Plans/Samples/README.md
```

The strategy defines the sample-program grammar. The operational guide records the current working branch, construction progress and promotion boundary.
