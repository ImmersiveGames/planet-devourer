# Game Flow

## Purpose

Game Flow demonstrates how one coherent Framework application moves between Routes and Activities and how contextual content follows that lifecycle.

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

Basic Flow
  Route_BasicFlow
  SCN_GameFlow_Basic
  Startup Activity = Activity_Basic_A

  Activity_Basic_A <-> Activity_Basic_B

  Route-owned scene content
    remains present while the Activity changes

  Activity-local content
    Visitors A -> Activity_Basic_A
    Visitors B -> Activity_Basic_B
    activated/deactivated through ActivityContentBinding

  Activity-owned scene composition
    Activity_Basic_A -> SCN_GameFlow_Basic_A
    Activity_Basic_B -> SCN_GameFlow_Basic_B

  contextual BGM
    Activity A BGM -> Activity B BGM

  return to HUB
  Activity = None
  BGM intent = Silence
```

The Basic Flow cycle, Activity-local visibility, Activity-owned scene composition, contextual BGM behavior and return-to-HUB teardown are proven in Play Mode.

Composition / Visibility is intentionally absorbed into Basic Flow instead of being materialized as a separate scenario. The same Activity switch demonstrates both local visibility changes inside the Route scene and load/release of Activity-owned scenes.

## Shared content

`GameFlow/Shared/` is for content reusable across Game Flow scenarios/applications. Application authority remains local to `GameFlowShowcase/`.

Do not create hidden dependencies on sibling top-level sample groups. Cross-group `_Sample/Shared` dependencies must be resolved before final UPM promotion.

## Transversal coverage

Game Flow is the natural home for contextual Route/Activity Camera and BGM behavior when those concepts arise naturally.

The current Basic Flow already proves contextual Route/Activity BGM behavior. Camera/Audio should remain supporting or ambient unless a later scenario explicitly needs one of them to teach a new contract. Optional Audio package boundaries must remain explicit.
