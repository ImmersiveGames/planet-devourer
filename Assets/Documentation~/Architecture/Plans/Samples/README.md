# Samples Authoring Plan

Canonical strategy:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-001-Immersive-Framework-Sample-and-Demonstration-Strategy.md
```

Current operational baseline:

```text
Repository
  ImmersiveGames/planet-devourer

Branch
  FirstGame

Authoring workspace
  Assets/_Sample/
```

Final official UPM distribution:

```text
com.immersive.framework/
  Samples~/
```

Workflow:

```text
AUTHORING
  planet-devourer:FirstGame
  Assets/_Sample/
  visible in Unity
  normal Project Browser / Inspector / Play Mode work

FINALIZATION
  resolve cross-group Assets/_Sample/Shared dependencies
  materialize/promote mature sample groups into com.immersive.framework/Samples~/
  activate/update package.json samples metadata

RELEASE VALIDATION
  install/use final framework package
  Package Manager -> Import each group
  validate references, scenes, prefabs, ScriptableObjects and Play Mode
```

Rules:

- the current `FirstGame` branch tree is the provisional operational truth;
- `_Sample/` is authoring infrastructure, not the shipped UPM sample root;
- `Assets/_Project/` is the future real-game structure and is not the sample taxonomy;
- final groups must not depend on sibling sample groups;
- authoring-only `Assets/_Sample/Shared` cross-group dependencies must be resolved before package finalization;
- final consumer behavior is proven from the Package Manager imported copy;
- the ADR filename is stable; revision is maintained inside the ADR and in Git history.
