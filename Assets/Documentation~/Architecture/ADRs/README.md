# FirstGame / Sample Architecture ADR Index

Last updated: **2026-08-22**

This folder contains architecture decisions for the Immersive Framework sample and demonstration program authored in `planet-devourer`.

## Canonical ADRs

| ADR | Scope | Status |
|---|---|---|
| `FG-ADR-001 — Immersive Framework Sample and Demonstration Strategy` | General sample-program grammar, authoring/distribution strategy, Demonstration Application vs Scenario model, transversal coverage | **FROZEN BASELINE — REVISION 11** |
| `FG-ADR-002 — Player Sample Scope and Demonstration Architecture` | Player-specific coverage, application sequence, blockers, Scene-Provided canonical reference and `Player/Shared` rule | **ACCEPTED — REVISION 1** |

## Authority split

```text
FG-ADR-001
  general sample-program strategy

FG-ADR-002
  Player-specific sample architecture
```

Do not reproduce a separate frozen Player application catalog in FG-ADR-001 or operational READMEs.

Current Player direction is:

```text
Getting Started / Minimal Game
  canonical Scene-Provided coverage

Manager-Provisioned
  next Player Demonstration Application

Character Selection
  planned / blocked by public arbitrary Actor-selection surface

Local Multiplayer
  planned / blocked by public Slot/device/input contract

Player/Shared
  only after concrete reuse
```

Operational construction status is tracked in:

```text
Assets/Documentation~/Architecture/Plans/Samples/README.md
```
