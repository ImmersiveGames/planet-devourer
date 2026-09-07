# FirstGame / Sample Architecture ADR Index

Last updated: **2026-09-07**

This folder contains architecture decisions for the Immersive Framework sample and demonstration program authored in `planet-devourer`.

## Canonical ADRs

| ADR | Scope | Status |
|---|---|---|
| `FG-ADR-001 — Immersive Framework Sample and Demonstration Strategy` | General sample-program grammar, authoring/distribution strategy, Demonstration Application vs Scenario model, transversal coverage | **FROZEN BASELINE — REVISION 13 / PLAYER SCOPE DELEGATED TO FG-ADR-002** |
| `FG-ADR-002 — Player Sample Scope and Demonstration Architecture` | Player-specific coverage, application sequence, blockers, Scene Player canonical reference and `Player/Shared` rule | **ACCEPTED — REVISION 6** |

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

Ordinary construction/proof progress after an accepted ADR revision is tracked in the operational Samples guide and sample/group READMEs unless the architecture decision itself changes.

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
  CLOSED / PLAY MODE REPROVEN 2026-09-05
  current Player Actor / Presentation composition proven

Local Multiplayer
  ManagerProvisioned
  two configured local Player Slots
  historical Slot/device/input blocker closed for implemented path
  MATERIALIZED
  BIDIRECTIONAL LEAVE / REJOIN + DEVICE OWNERSHIP PLAY MODE PROVEN 2026-09-07
  ACTIVE CONSTRUCTION CONTINUES

Player/Shared
  materialized from concrete reuse
  reusable technical baseline = FG_Player / FG_PlayerActor / FG_Presentation
  never shared application/session authority
```

## Character Selection closure evidence

The consumer sample proves:

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

The current physical composition was reproven on 2026-09-05 using `ActorProfile.PresentationPrefab`, `FG_FarmerPresentation` / `FG_CowPresentation`, Follow camera and gameplay input/movement.

## Local Multiplayer current evidence

FG-ADR-002 Revision 6 records that the previous public Slot/device/input blocker was re-audited and is closed for the implemented Manager-Provisioned two-Slot path. The architecture decision remains unchanged; the operational consumer proof has now advanced beyond the first slice recorded in that revision.

Current consumer composition uses:

```text
PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionLeaveCommandTrigger

IPlayerSessionScopedAccess.Changed
  -> deferred/coalesced refresh
IPlayerSessionScopedAccess.TryGetObservation(...)
typed per-Slot IsJoined evidence
typed per-Slot InputOwnership evidence
```

Current Play Mode evidence on 2026-09-07 covers:

```text
P1 Join / Leave / Rejoin as fresh occurrences
P2 Join / Leave / Rejoin as fresh occurrences
Activity placement reapplied
UI derived from current Slot occupancy
both Players active / Activity readiness complete
P1 preservation while P2 leaves
P2 preservation while P1 leaves
distinct current device ownership for P1 and P2
already-owned device blocked by tutorial before Join
repeated bidirectional Leave/Rejoin cycles
```

The later run produced six successful Join operations and six successful Leave operations. It contained no `RejectedDeviceAlreadyOwned`, no transient `RegisteredHost.NotRegistered`, no failed ownership diagnostic, no warning and no error.

Current Framework Full Player certification is:

```text
PLAYER QA CERTIFIED
17/17
```

Local Multiplayer is not closed yet. Remaining consumer proof is intentionally narrower:

```text
actual gameplay no-cross-control between P1 and P2
explicit extra-Join behavior when both configured Slots are occupied
Close Joining while existing Players remain joined
Reopen Joining behavior when applicable
```

Operational construction status is tracked in:

```text
Assets/Documentation~/Architecture/Plans/Samples/README.md
```
