# Game Flow Showcase

## Demonstrates

One coherent GameApplication containing a sample HUB plus compatible Game Flow scenarios.

Current proven scenario:

```text
Basic Flow
  Route_Hub -> Route_BasicFlow
  Startup Activity A
  Activity A <-> Activity B
  return to Route_Hub
  Activity -> None
```

The HUB selects sample topics. It is sample navigation, not Framework authority or gameplay progression.

## Run

```text
Application
  GameApplication_GameFlow.asset

Entry scene
  Scenes/SCN_GameFlow_Hub.unity
```

Set `GameApplication_GameFlow.asset` Active through the official Framework Inspector surface, then enter Play Mode.

## Observe

Current expected/proven observations:

```text
boot
  -> SCN_GameFlow_Persistence loaded as Persistent Content
  -> Route_Hub becomes current
  -> no Startup Activity
  -> Activity = None

enter Basic Flow
  -> Route_BasicFlow
  -> Activity_Basic_A

switch Activity
  -> Activity_Basic_A <-> Activity_Basic_B

return to HUB
  -> Basic Flow Activity tears down
  -> Route_Hub becomes current
  -> Activity = None
```

The cycle is repeatable.

## Inspect

```text
GameApplication_GameFlow.asset
Routes/Route_Hub.asset
Routes/Route_BasicFlow.asset
Activities/Activity_Basic_A.asset
Activities/Activity_Basic_B.asset
Activities/ActivityContent_Basic_A.asset
Activities/ActivityContent_Basic_B.asset
Scenes/SCN_GameFlow_Persistence.unity
Scenes/SCN_GameFlow_Hub.unity
Scenes/SCN_GameFlow_Basic.unity
Scenes/SCN_GameFlow_Basic_A.unity
Scenes/SCN_GameFlow_Basic_B.unity
```

The GameApplication currently keeps Player Session disabled because Player is not required to explain this Basic Flow vertical.

## Framework concepts

```text
GameApplication ownership
Persistent Content
Route ownership
Route with no Startup Activity
Route with Startup Activity
Activity composition
Activity-local content/visibility
explicit Route request
explicit Activity request
teardown and return to Activity None
```

Contextual Activity BGM has been materialized for the Basic Flow Activities in the current authoring tree, but it is not marked **PROVEN** here until runtime playback/lifecycle behavior is verified.

## Evolutionary scenarios

The remaining catalog is intentionally not frozen. Current candidates continue to include:

```text
Composition / Visibility
Transition
Loading & Readiness
Restart / Recovery
contextual Camera / Audio coverage where natural
```
