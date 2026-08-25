# Player Provisioning (Manager-Provisioned runtime mode)

Status: **CURRENT PLAYER PROVISIONING CUT — PLAY MODE PROVEN 2026-08-24**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

`Player Provisioning` is the product-facing Demonstration Application for:

```text
HostProvisioning = ManagerProvisioned
```

The existing folder/asset names may still use `ManagerProvisioned`; runtime terminology remains valid. The product composition demonstrated here is **Player Provisioning**: Session-authorized authority creates a Local Player Host when Join is requested.

## Purpose

Demonstrate the smallest coherent public consumer path for:

```text
Player Provisioning authority
  -> explicit Join request
  -> Local Player Host creation
  -> default Actor selection/preparation
  -> physical Actor materialization
  -> Activity participation / readiness
  -> gameplay input
  -> Player Camera
  -> Leave / Rejoin occurrence
```

Application count represents materially different application/session intent, not feature count. Compatible Player capabilities remain Scenarios by default.

## Current composition

```text
one GameApplication
one PlayerSessionProfile
HostProvisioning = ManagerProvisioned
one supported Player Slot
initial Joining open
Persistent Content
one Route
one Startup Activity
Local Player Provisioning authority
Local Player Host prefab
Logical Player Actor
initial placement
minimal movement
Third Person Camera
Activity-owned ambient BGM
```

## Player runtime evidence

The current Play Mode proof reached:

```text
Player Session
  -> ManagerProvisioned
  -> one supported Slot
  -> Joining open

Provisioning runtime
  -> Ready

Before Join
  -> WaitingForJoin
  -> hostCount = 0
  -> Slot available

Join
  -> SucceededJoined
  -> Local Player Host created
  -> Actor selected/prepared/materialized
  -> gameplay admitted
  -> GameplayReady

Movement / Look
  -> minimal WASD movement
  -> horizontal Look rotates Player yaw
  -> vertical Look controls camera pitch

Player Camera
  -> Third Person
  -> follows current Player

Leave / Rejoin
  -> Leave releases current occurrence
  -> later Join creates a new occurrence
```

## Audio as supporting/ambient composition

Audio is not the primary subject of this Player demonstration. It is used naturally as a transversal supporting/ambient feature.

Current BGM composition:

```text
Route
  no FrameworkRouteBgmBinding

Activity: Manager Provisioned
  FrameworkActivityBgmBinding
    BGM = BGM_Antiguidade
```

Observed runtime result:

```text
Activity Enter
  -> FrameworkBgmDirector
  -> operation = Apply
  -> outcome = Applied
  -> requestedBgm = BGM_Antiguidade
  -> confirmedBgm = BGM_Antiguidade
```

This proves a useful current Audio contract in a real consumer composition:

> `FrameworkActivityBgmBinding` does not require a `FrameworkRouteBgmBinding`.

Route and Activity BGM authoring are independent. The Activity owns only its own BGM intent; there is no Route -> Activity BGM authoring reference.

## Camera composition

The current Player Camera uses a Player-owned Third Person rig:

```text
Local Player Logical
├─ Player Gameplay Camera
│   Required
│   precedence = 50
├─ Camera Tracking Pivot
└─ Third Person Camera Rig
    ├─ Camera Rig Composer
    │   Model = Third Person
    └─ Cinemachine Camera (materialized by composer)
```

The persistent Session Camera Rig remains the explicit Camera Output Default. Join makes the eligible Player request take the output; release returns presentation to the Default when no normal winner remains.

## Run / observe

```text
Play
  -> application boots
  -> Startup Activity enters
  -> Activity BGM applies
  -> provisioning waits for Join

Join
  -> Player Host and Actor materialize
  -> readiness completes
  -> Third Person Camera becomes active
  -> movement/look are available

Leave
  -> current Player occurrence is released

Join again
  -> new occurrence is provisioned
```

## Inspect

```text
GameApplication
  -> PlayerSessionProfile
      HostProvisioning = ManagerProvisioned
  -> Persistent Content
      Local Player Provisioning
      Camera Output / Default Camera
      AudioRuntimeHost + FrameworkBgmDirector
  -> Route
  -> Activity
      Activity BGM Binding / BGM_Antiguidade
  -> Local Player Host Prefab
  -> Logical Player Actor
      gameplay input
      movement/look
      Player Gameplay Camera
      Third Person Camera Rig
```

## Boundary

Do not add sample-owned Player discovery, Slot registries, device authority, hidden Actor mutation, parallel Camera authority or Audio fallback logic.

The sample consumes public Framework surfaces. If a later Player Scenario requires a missing public contract, that is product evidence and must not be hidden inside sample code.
