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

`Assets/_Sample/` is the visible development and proving workspace for creating and validating Immersive Framework consumer samples in Unity. Final official package distribution belongs to `com.immersive.framework/Samples~/`; do not ship `_Sample/` as the final UPM sample root.

## Where to start

Use the sample-local README for composition and execution instructions, and the operational status guide for construction/proof state.

- Getting Started / Minimal Game: `GettingStarted/MinimalGame/README.md`
- Reset consumer proof for Minimal Game: `GettingStarted/MinimalGame/RESET-USAGE.md`
- Game Flow: `GameFlow/README.md` and the GameFlowShowcase README
- Player samples: `PlayerSamples/README.md`
- Shared authoring assets: `Shared/README.md`
- Operational program status: `Assets/Documentation~/Architecture/Plans/Samples/README.md`

## Current authoring picture

The detailed program status lives in the operational guide. Current consumer documentation records:

- Getting Started / Minimal Game is the canonical Scene-Provided consumer base; its current Session Camera Assignment composition is authored, while migrated Play Mode proof remains to be confirmed.
- Game Flow contains Session Camera Assignment command consumers and Route-scoped Activity transition observation.
- Player includes Manager-Provisioned, Character Selection, Local Multiplayer and Split Screen samples. Local Multiplayer SharedGroup has manual PASS (2026-10-06); its Runtime Gameplay Availability consumer has manual PASS (2026-10-08). Other validation remains sample-specific.
- Advanced Context and Persistence remain separate program areas.
- UPM promotion/import proof remains a later release gate.

## Reset proof slice

The Reset consumer proof is closed for the current authoring/proving phase and is intentionally demonstrated in the consumer project rather than as a second Reset architecture.

The proof covers:

```text
Object / Direct
Object / Stable
Composition / Direct - Descendants
Composition / Stable - Explicit Members
CurrentActivity
CurrentRoute
Activity Restart
Multiple Participants
```

See `GettingStarted/MinimalGame/RESET-USAGE.md` for the closed authoring intent, execution steps and validation evidence. UPM promotion/import validation remains separate.

## Documentation authority

Sample strategy:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-001-Immersive-Framework-Sample-and-Demonstration-Strategy.md
```

Player-specific strategy:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
```

Operational sample guide/status:

```text
Assets/Documentation~/Architecture/Plans/Samples/README.md
```

The ADRs define sample-program boundaries. The operational guide records current construction/proof state. Sample-local READMEs teach the concrete consumer composition.
