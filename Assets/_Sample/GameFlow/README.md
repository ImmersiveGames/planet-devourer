# Game Flow

## Purpose

Game Flow demonstrates how one coherent Framework application moves between Routes and Activities and how contextual content and presentation follow that lifecycle.

Frozen structural shape:

```text
one Demonstration Application
one GameApplication
Sample HUB / Menu
multiple compatible Scenarios as needed
```

Scenario content remains evolutionary.

## Current Demonstration Application

```text
GameFlowShowcase/
  GameApplication_GameFlow.asset
```

The current application intentionally starts in a sample HUB. The HUB is navigation only; it is not runtime authority or gameplay progression.

The Game Flow Showcase authors Session Camera policy per Route through one Route-scoped adapter in each Route primary scene. Basic Flow maps A/B to their respective Assignments and C to none. Readiness maps C/D/E to one fixed Readiness Assignment, keeping the view stable across the readiness test. Camera remains Session-owned and single-writer; Route and Activity assets have no Camera fields.

## Run

1. Select `GameFlowShowcase/GameApplication_GameFlow.asset` and use the Framework **Set Active** action when it is not already active.
2. Open `GameFlowShowcase/Scenes/SCN_GameFlow_Hub.unity` for inspection if desired.
3. Enter Play Mode.
4. Use the HUB to enter the currently materialized Game Flow scenarios.

## Current proven vertical

```text
HUB
  Route_Hub
  SCN_GameFlow_Hub
  no Startup Activity
  Activity = None
  BGM intent = Silence

  Route switch -> Basic Flow
    Transition = Fade cover/reveal
    Loading surface = active for Route work

Basic Flow
  Route_BasicFlow
  SCN_GameFlow_Basic
  Startup Activity = Activity_Basic_A

  Activity_Basic_A <-> Activity_Basic_B
    target policy = Seamless

  Activity_Basic_A -> Activity_Basic_C
  Activity_Basic_B -> Activity_Basic_C
    target policy = Fade

  Activity_Basic_C -> Activity_Basic_A / Activity_Basic_B
    target policy = Seamless

  Route-owned scene content
    remains present while the Activity changes

  Activity-local content
    Visitors A -> Activity_Basic_A
    Visitors B -> Activity_Basic_B
    activated/deactivated through ActivityContentBinding

  Activity-owned scene composition
    Activity_Basic_A -> SCN_GameFlow_Basic_A
    Activity_Basic_B -> SCN_GameFlow_Basic_B

  content-less Activity proof
    Activity_Basic_C has no ActivityContentProfile
    no Activity-owned scene is materialized for C
    A/B Activity-local content is hidden while C is active

  contextual BGM
    Activity A BGM -> Activity B BGM
    Activity C publishes no new BGM intent
    A -> C preserves Activity A BGM
    B -> C preserves Activity B BGM

  return to HUB
    Route switch = Fade cover/reveal + Loading
    Activity = None
    BGM intent = Silence
```

The Basic Flow cycle, Activity-local visibility, Activity-owned scene composition, content-less Activity isolation, contextual BGM replacement/preservation, Route cover/loading presentation and return-to-HUB teardown are proven in Play Mode.

Composition / Visibility remains intentionally absorbed into Basic Flow instead of being materialized as a separate scenario. Activity A/B demonstrate both local visibility changes inside the Route scene and load/release of Activity-owned scenes. Activity C provides the negative case: it is a valid Activity with no Activity Content Profile, so A/B content does not leak into it.

Basic Transition presentation is also absorbed into this same flow rather than becoming a separate HUB scenario. Route switches use the persistent Transition + Loading surfaces, while Activity requests demonstrate authored `Seamless` and `Fade` presentation policies.

## Shared content

`GameFlow/Shared/` is for content reusable across Game Flow scenarios/applications. Application authority remains local to `GameFlowShowcase/`.

Do not create hidden dependencies on sibling top-level sample groups. Cross-group `_Sample/Shared` dependencies must be resolved before final UPM promotion.

## Transversal coverage

Game Flow may issue sample-specific Session Camera Assignment commands from a Route-scoped observer of committed Activity transitions. Camera Assignment state and Output presentation remain Session-owned; Route and Activity assets do not own Camera, and the sample does not use CameraRequest.

Basic Flow and Readiness demonstrate Route-scoped transition-observer Session Camera policy: Basic Flow uses Activate/Replace/Clear for A/B, while Readiness activates one fixed Assignment for C and keeps it unchanged across C/D/E before clearing on Route exit. The Session remains the Camera single writer. The sample also demonstrates contextual BGM behavior and the distinction between explicit Silence, explicit Play and no-request preservation. Optional Audio package boundaries remain explicit.

The Route-scoped Camera observer proof is closed as of 2026-10-04: Framework EditMode **169/169**, RouteLifecycle observer **6/6**, Camera Editor **74/74**, and GameFlow Play Mode `Hub -> A -> B -> A -> C -> B -> Hub` completed with `blockingIssues=0`. The adapter exists once in the Route-owned `SCN_GameFlow_Basic`; Activity A/B scenes do not own copies.

