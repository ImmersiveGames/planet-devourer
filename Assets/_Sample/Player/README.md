# Player Samples

Status: **PLAYER SCOPE GOVERNED BY FG-ADR-002 — TERMINOLOGY REVISION 2026-08-24**

Canonical Player sample authority:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
```

General sample-program strategy remains in FG-ADR-001.

## Naming model

The Player samples use short product-facing names while preserving the existing runtime provisioning terms.

```text
Local Player Host
  common technical host for one local Player

Scene Player
  Local Player Host already authored in a Scene
  HostProvisioning = SceneProvided

Player Provisioning
  Session/UIGlobal authority that can create Local Player Hosts
  HostProvisioning = ManagerProvisioned
```

`SceneProvided` and `ManagerProvisioned` remain valid runtime modes. They are not presented as two peer Player Host object types.

## Canonical Scene Player coverage

`Assets/_Sample/GettingStarted/MinimalGame/` is the canonical executable reference for the Scene Player flow.

It already demonstrates:

- a Scene-authored Local Player Host;
- `HostProvisioning = SceneProvided`;
- Scene Logical Player authoring;
- admission on Activity entry;
- logical Actor preparation;
- gameplay readiness;
- gameplay input binding;
- Mounted / First Person Camera;
- minimal Move / Look gameplay input.

These contracts are intentionally **not duplicated** as a dedicated Scene Player Demonstration Application under Player.

A dedicated Scene Player application should be added only if future implementation evidence reveals a distinct consumer contract that Getting Started cannot demonstrate clearly.

## Current Player demonstrations

| Demonstration | Runtime provisioning | Status | Meaning |
|---|---|---|---|
| Getting Started / Minimal Game | `SceneProvided` | **CANONICAL / PROVEN** | Scene Player reference |
| Player Provisioning | `ManagerProvisioned` | **NEXT PLAYER APPLICATION** | Session-authorized Local Player Host creation/provisioning |
| Character Selection | depends on final application model | **PLANNED / BLOCKED** | Requires a sufficient public arbitrary Actor-selection surface |
| Local Multiplayer | depends on final application model | **PLANNED / BLOCKED** | Requires a sufficient public Slot/device/input contract |

This is the current implementation sequence, not a permanent closed catalog.

## Player Provisioning

Player Provisioning is the next Player Demonstration Application.

Its first cut should prove the smallest coherent public consumer path for Session-authorized Local Player Host provisioning:

```text
Player Provisioning authority
  -> Local Player Host Prefab
  -> explicit Join request
  -> Local Player Host instance
  -> admission
  -> Session ownership
```

The provisioning object is **not itself a Player Host**.

Compatible Player behaviors remain Scenarios by default and should be added only when they clarify the provisioning contract.

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
