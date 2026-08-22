# Player Samples

Status: **PLAYER SCOPE GOVERNED BY FG-ADR-002 — 2026-08-22**

Canonical Player sample authority:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
```

General sample-program strategy remains in FG-ADR-001.

## Canonical Scene-Provided coverage

`Assets/_Sample/GettingStarted/MinimalGame/` is the canonical executable reference for the Scene-Provided Player flow.

It already demonstrates:

- Scene-Provided Local Player;
- Scene-Provided Logical Player;
- admission on Activity entry;
- logical Actor preparation;
- gameplay readiness;
- gameplay input binding;
- Mounted / First Person Camera;
- minimal Move / Look gameplay input.

These contracts are intentionally **not duplicated** as a dedicated Scene-Provided Demonstration Application under Player.

A dedicated Scene-Provided Player application should be added only if future implementation evidence reveals a distinct consumer contract that Getting Started cannot demonstrate clearly.

## Current Player demonstrations

| Demonstration | Status | Meaning |
|---|---|---|
| Getting Started / Minimal Game | **CANONICAL / PROVEN** | Scene-Provided Player reference |
| Manager-Provisioned | **NEXT PLAYER APPLICATION** | Next distinct provisioning/application path to materialize |
| Character Selection | **PLANNED / BLOCKED** | Requires a sufficient public arbitrary Actor-selection surface |
| Local Multiplayer | **PLANNED / BLOCKED** | Requires a sufficient public Slot/device/input contract |

This is the current implementation sequence, not a permanent closed catalog.

## Manager-Provisioned

Manager-Provisioned is the next Player Demonstration Application.

Its first cut should prove the smallest coherent public consumer path for Session-authorized provisioning. Compatible Player behaviors remain Scenarios by default and should be added only when they clarify the provisioning contract.

## Character Selection

Character Selection remains planned but must not be implemented while the sample would need to bypass the public Framework surface.

The blocker is the public ability for game-owned UI to:

```text
observe eligible/current Actor state
  -> request an arbitrary supported Actor selection
  -> observe the confirmed result
```

without private/internal access, reflection, direct Session mutation or parallel sample-owned Actor authority.

## Local Multiplayer

Local Multiplayer remains planned but must not be implemented while the sample would need to invent its own Slot/device/input architecture.

The blocker is a sufficient public contract for:

```text
local participant / device intent
  -> Slot association
  -> Player admission
  -> correct input ownership/routing
  -> observable occupancy
  -> release/reuse when applicable
```

without parallel sample-owned Slot, device or input authority.

## Application / Scenario rule

```text
materially incompatible initial Player Session intent
  -> separate Demonstration Application

compatible runtime behavior
  -> Scenario inside that application
```

Participation, physical lifetime, placement, Input/Pause and Leave/Rejoin do not automatically create new GameApplications.

Samples demonstrate representative canonical usage; exhaustive combinations belong in QAFramework.

## Player/Shared

`Player/Shared` is **not** a required layer and must not be populated preemptively.

```text
used by one Player application
  -> keep local

concretely reused by two or more Player applications
  -> promote the reusable content to Player/Shared
```

Application/session authority always remains local to the owning Demonstration Application.

An existing empty or placeholder `Shared/` scaffold does not establish shared ownership.

## Public-surface gate

Player samples consume public/product Framework APIs.

If a required public Player contract is missing, the demonstration remains **PLANNED / BLOCKED**. Sample code must not hide the product gap with internal discovery, reflection, direct runtime mutation, parallel registries or silent fallbacks.
