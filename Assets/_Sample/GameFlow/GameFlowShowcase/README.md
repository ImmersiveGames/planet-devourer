# Game Flow Showcase

## Demonstrates

One coherent GameApplication containing a sample HUB plus compatible Game Flow scenarios.

Current proven scenario:

```text
Basic Flow
  Route_Hub -> Route_BasicFlow
  Startup Activity = Activity_Basic_A
  Activity_Basic_A <-> Activity_Basic_B
  Route-owned scene content remains loaded while the Activity changes
  Activity-local content changes visibility inside the Route scene
  Activity-owned scene content is loaded/released with the active Activity
  contextual BGM follows the active Route/Activity intent
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
  -> Route_Hub publishes explicit BGM Silence

enter Basic Flow
  -> Route_BasicFlow
  -> SCN_GameFlow_Basic becomes the Route Primary Scene
  -> Activity_Basic_A becomes active
  -> SCN_GameFlow_Basic_A is materialized
  -> Activity A local content is visible
  -> Activity B local content is hidden
  -> Activity A BGM is presented

switch Activity A -> B
  -> SCN_GameFlow_Basic remains the Route Primary Scene
  -> Activity_Basic_A exits
  -> SCN_GameFlow_Basic_A is released
  -> Activity_Basic_B enters
  -> SCN_GameFlow_Basic_B is materialized
  -> Activity A local content is hidden
  -> Activity B local content is visible
  -> Activity B BGM is presented

return to HUB
  -> active Basic Flow Activity tears down
  -> Activity-owned scene content is released
  -> Route_Hub becomes current
  -> Activity = None
  -> Route_Hub explicit Silence is restored by destination Route intent
```

The cycle is repeatable.

## Content composition

`SCN_GameFlow_Basic.unity` is the Route Primary Scene for `Route_BasicFlow.asset`. It remains the scene-owned Route composition while `Activity_Basic_A` and `Activity_Basic_B` switch.

The Basic Flow deliberately demonstrates two different Activity-scoped content forms:

```text
SCN_GameFlow_Basic.unity
  Route-owned content
    environment / walls
      remains present while Route_BasicFlow is active

  Activity-local content
    Visitors A
      ActivityContentBinding -> Activity_Basic_A

    Visitors B
      ActivityContentBinding -> Activity_Basic_B

    Activity navigation buttons
      visibility also follows the relevant Activity

SCN_GameFlow_Basic_A.unity
  Activity-owned scene content
    materialized while Activity_Basic_A is active

SCN_GameFlow_Basic_B.unity
  Activity-owned scene content
    materialized while Activity_Basic_B is active
```

Activity-local content is authored inside the Route scene and is activated/deactivated by `ActivityContentBinding`. It is not loaded or unloaded as an Activity scene.

Activity-owned scene content is separate composition declared by `ActivityContent_Basic_A.asset` and `ActivityContent_Basic_B.asset`; those scenes are materialized and released with their Activities.

This distinction is intentional:

```text
Activity-local visibility
  !=
Activity scene materialization
```

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
Activity-local content / visibility
Activity-owned scene composition
explicit Route request
explicit Activity request
contextual Route / Activity BGM intent
teardown and return to Activity None
```

The contextual BGM path is **PROVEN** for the current Basic Flow cycle:

```text
Route_Hub Silence
  -> Activity_Basic_A BGM
  -> Activity_Basic_B BGM
  -> Route_Hub Silence
```

Owner exit preserves the confirmed BGM presentation; the destination Route/Activity intent determines the next presentation.

## Evolutionary scenarios

Composition / Visibility is no longer a separate planned scenario: its basic contract is demonstrated directly inside Basic Flow through `ActivityContentBinding` plus Activity-owned scene composition.

The remaining catalog is intentionally not frozen. Current candidates include:

```text
Transition
Loading & Readiness
Restart / Recovery
contextual Camera coverage where natural
additional Audio coverage only where it teaches a new contract
```
