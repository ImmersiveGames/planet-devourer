# FirstGame / Sample Architecture ADR Index

Last updated: **2026-08-26**

This folder contains architecture decisions for the Immersive Framework sample and demonstration program authored in `planet-devourer`.

## Canonical ADRs

| ADR | Scope | Status |
|---|---|---|
| `FG-ADR-001 — Immersive Framework Sample and Demonstration Strategy` | General sample-program grammar, authoring/distribution strategy, Demonstration Application vs Scenario model, transversal coverage | **FROZEN BASELINE — REVISION 11** |
| `FG-ADR-002 — Player Sample Scope and Demonstration Architecture` | Player-specific coverage, application sequence, blockers, Scene Player canonical reference and `Player/Shared` rule | **ACCEPTED — REVISION 3** |

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
  canonical Scene Player / SceneProvided coverage
  PROVEN

Player Provisioning
  ManagerProvisioned
  MATERIALIZED / PLAY MODE PROVEN

Character Selection
  NEXT / PUBLIC SURFACE UNBLOCKED
  ActorResolution = LeaveUnresolved
  public explicit Actor-selection commands available

Local Multiplayer
  PLANNED / BLOCKED
  requires public Slot/device/input ownership/observation contract

Player/Shared
  only after concrete reuse
```

The 2026-08-26 Framework Player Actor-selection closure removes the previous Character Selection blocker. It does not deliver exact-Slot public Join or the Local Multiplayer Slot/device/input contract.

Operational construction status is tracked in:

```text
Assets/Documentation~/Architecture/Plans/Samples/README.md
```
