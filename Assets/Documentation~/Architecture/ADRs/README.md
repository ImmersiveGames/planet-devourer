# FirstGame / Sample Architecture ADR Index

Last updated: **2026-08-28**

This folder contains architecture decisions for the Immersive Framework sample and demonstration program authored in `planet-devourer`.

## Canonical ADRs

| ADR | Scope | Status |
|---|---|---|
| `FG-ADR-001 — Immersive Framework Sample and Demonstration Strategy` | General sample-program grammar, authoring/distribution strategy, Demonstration Application vs Scenario model, transversal coverage | **FROZEN BASELINE — REVISION 13 / PLAYER SCOPE DELEGATED TO FG-ADR-002** |
| `FG-ADR-002 — Player Sample Scope and Demonstration Architecture` | Player-specific coverage, application sequence, blockers, Scene Player canonical reference and `Player/Shared` rule | **ACCEPTED — REVISION 4** |

## Authority split

```text
FG-ADR-001
  general sample-program strategy
  Player-specific catalog/status delegated

FG-ADR-002
  Player-specific sample architecture
  sequencing / blockers / Player terminology
```

Do not reproduce a separate frozen Player application catalog in FG-ADR-001.

Dated Player examples still present in the general FG-ADR-001 operational snapshot are historical/subordinate to the current FG-ADR-002 and operational status guide. FG-ADR-001 is not reopened solely to duplicate Player delivery status.

## Current Player direction

```text
Getting Started / Minimal Game
  canonical Scene Player / SceneProvided coverage
  PROVEN

Player Provisioning
  ManagerProvisioned
  MATERIALIZED / PLAY MODE PROVEN

Character Selection
  ManagerProvisioned
  ActorResolution = LeaveUnresolved
  AUTHORING COMPLETE / PLAY MODE PROVEN 2026-08-28
  public PlayerSessionObserver + explicit Select Actor composition proven

Local Multiplayer
  NEXT PLANNED PLAYER DEMONSTRATION
  PLANNED / BLOCKED
  requires public Slot/device/input ownership/observation contract

Player/Shared
  only after concrete reuse justifies promotion of reusable presentation/content
  never as shared application/session authority
```

## Character Selection closure evidence

The consumer sample now proves:

```text
Join
  -> Joined + unresolved Actor
  -> Preparing / WaitingForActorSelection
  -> PlayerSessionObserver shows selection UI
  -> explicit Farmer/Cow ActorProfile selection
  -> Framework preparation/materialization
  -> GameplayReady
  -> observer hides selection UI
  -> Leave/Rejoin repeats the explicit-selection path
```

The sample-owned button presenter reads `DisplayName + Icon` from the `ActorProfile` already configured on `PlayerSessionSelectActorCommandTrigger`; it does not create another Actor-selection authority.

Framework Full Player certification after the `LeaveUnresolved` reconcile fix is:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
sessionChangeObservation = PASS
designerEventProjection = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

The 2026-08-26 public arbitrary Actor-selection closure removed the previous Character Selection blocker. The 2026-08-28 consumer proof closes Character Selection authoring/proving. Neither change delivers exact-Slot public Join or the Local Multiplayer Slot/device/input contract.

Operational construction status is tracked in:

```text
Assets/Documentation~/Architecture/Plans/Samples/README.md
```
