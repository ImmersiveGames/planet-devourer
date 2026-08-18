# Samples Authoring Guide and Status

Last updated: **2026-08-17**

Canonical strategy:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-001-Immersive-Framework-Sample-and-Demonstration-Strategy.md
```

This file is the **operational guide/status surface** for the active sample-construction program. The ADR defines the frozen strategy; this guide records where current work happens, which sample is closed for authoring, and what remains before final UPM release.

## Current operational baseline

```text
Repository
  ImmersiveGames/planet-devourer

Branch
  main

Verified implementation baseline before this documentation cut
  73ed9eff75d387f5eb250900e24df7e323961754
  "Camera rig Apply"

Authoring workspace
  Assets/_Sample/
```

The older `FirstGame` branch references in Revision 10 planning material describe the historical authoring baseline. Current implementation work is on `main`.

## Current sample progress

| Order | Group / Demonstration | Authoring / Play Mode | UPM release |
|---|---|---|---|
| 00 | Getting Started / Minimal Game | **COMPLETE / PROVEN** | Pending promotion + Package Manager import proof |
| 01 | Game Flow | Next implementation area | Pending |
| 02 | Player | Planned | Pending |
| 03 | Advanced Context | Planned | Pending |
| 04 | Persistence | Planned | Pending |

### Sample 00 closure

Getting Started / Minimal Game satisfies the current authoring-phase goal:

```text
one GameApplication
one PlayerSessionProfile
Persistent Content
one Route
one Activity
one gameplay scene
Scene-Provided Player
GameplayReady participation
Mounted / First Person Camera
minimal movement/look Input
```

Observed Play Mode terminal evidence:

```text
Framework boot succeeded
Activity readiness = Ready
blockingIssues = 0
Camera Output initialized
Default Camera Rig = Session Camera Rig
Player gameplay binding READY
Move input received
Look input received
```

Therefore:

```text
Sample 00 implementation work
  CLOSED

next sample implementation cut
  MAY BEGIN
```

## Completion vocabulary

Do not collapse authoring completion and package release into one status.

```text
AUTHORING COMPLETE / PLAY MODE PROVEN
  sample content is materialized under Assets/_Sample/
  intended runtime behavior works in the authoring/proving project
  canonical composition is inspectable

UPM RELEASE COMPLETE
  mature group is promoted to com.immersive.framework/Samples~/
  package.json samples metadata is active
  selected group is imported through Package Manager into a clean consumer project
  imported references/scenes/prefabs/assets resolve
  Play Mode works from the imported copy
```

A sample group may be closed for construction and allow work to advance while its later UPM promotion gate remains pending.

## Authoring workflow

```text
AUTHORING / PROVING
  planet-devourer:main
  Assets/_Sample/
  visible in Unity
  normal Project Browser / Inspector / Play Mode work

GROUP CLOSURE
  verify intended composition
  verify intended observable behavior
  record runtime evidence
  update local/group README and this status guide

FINALIZATION
  resolve cross-group Assets/_Sample/Shared dependencies
  materialize/promote mature sample groups into com.immersive.framework/Samples~/
  activate/update package.json samples metadata

RELEASE VALIDATION
  install/use final framework package
  Package Manager -> Import each group
  validate references, scenes, prefabs, ScriptableObjects and Play Mode
```

## Rules

- the current `main` tree is the operational truth for active sample construction;
- `_Sample/` is authoring/proving infrastructure, not the shipped UPM sample root;
- `Assets/_Project/` is the real FIRSTGAME/game structure and is not the sample taxonomy;
- final groups must not depend on sibling sample groups;
- authoring-only `Assets/_Sample/Shared` cross-group dependencies must be resolved before package finalization;
- final consumer behavior is proven from the Package Manager imported copy;
- finishing a sample's authoring phase does not silently claim UPM release validation;
- the canonical ADR filename is stable; structural strategy changes belong in that ADR, while ordinary construction progress belongs here and in sample-local README files.

## Next implementation cut

With Sample 00 closed for authoring/proving, the sample-program sequence may advance to:

```text
Sample 01
  Game Flow
```

Game Flow should continue to follow the frozen strategy: one initial Demonstration Application, a sample HUB/Menu when useful, and evolutionary scenarios for Route/Activity, content/visibility, Transition, Loading/Readiness and Restart/Recovery.
