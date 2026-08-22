# Game Flow Showcase

## Demonstrates

One coherent GameApplication containing a sample HUB plus compatible Game Flow scenarios.

Current proven scenario:

```text
Basic Flow
  Route_Hub -> Route_BasicFlow
    Route transition = Fade cover/reveal
    Route loading presentation = enabled

  Startup Activity = Activity_Basic_A

  Activity_Basic_A <-> Activity_Basic_B
    target policy = Seamless

  Activity_Basic_A -> Activity_Basic_C
  Activity_Basic_B -> Activity_Basic_C
    target policy = Fade

  Activity_Basic_C -> Activity_Basic_A / Activity_Basic_B
    target policy = Seamless

  Route-owned scene content remains loaded while the Activity changes
  Activity-local content changes visibility inside the Route scene
  Activity-owned scene content is loaded/released for A/B
  Activity C proves a valid Activity with no ActivityContentProfile and no Activity-owned scene
  contextual BGM follows explicit intent and preserves the previous confirmed presentation when C publishes no new intent

  Route_BasicFlow -> Route_Hub
    Route transition = Fade cover/reveal
    Route loading presentation = enabled
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
  -> persistent Transition adapter resolved
  -> persistent Loading adapter resolved
  -> Route_Hub becomes current
  -> no Startup Activity
  -> Activity = None
  -> Route_Hub publishes explicit BGM Silence

enter Basic Flow
  -> Route transition uses Fade cover/reveal
  -> Loading surface participates in the Route operation
  -> Route_BasicFlow
  -> SCN_GameFlow_Basic becomes the Route Primary Scene
  -> Activity_Basic_A becomes active
  -> SCN_GameFlow_Basic_A is materialized
  -> Activity A local content is visible
  -> Activity B local content is hidden
  -> Activity A BGM is presented

switch Activity A -> B
  -> Activity target policy is Seamless
  -> Transition surface is skipped by Activity policy
  -> Loading presentation is skipped by Activity policy
  -> SCN_GameFlow_Basic remains the Route Primary Scene
  -> Activity_Basic_A exits
  -> SCN_GameFlow_Basic_A is released
  -> Activity_Basic_B enters
  -> SCN_GameFlow_Basic_B is materialized
  -> Activity A local content is hidden
  -> Activity B local content is visible
  -> Activity B BGM is presented

switch Activity A or B -> C
  -> Activity target policy is Fade
  -> Transition surface performs Fade cover/reveal
  -> canonical Loading presentation remains skipped by the Activity Fade policy
  -> previous Activity-owned scene is released
  -> Activity_Basic_C becomes active
  -> Activity_Basic_C has no ActivityContentProfile
  -> no Activity-owned scene is materialized for C
  -> Activity A local content is hidden
  -> Activity B local content is hidden
  -> Activity C has no new BGM intent
  -> confirmed BGM from A or B is preserved

switch Activity C -> A or B
  -> target Activity policy is Seamless
  -> target Activity-owned scene is materialized
  -> corresponding Activity-local content becomes visible
  -> target Activity BGM intent is applied

return to HUB
  -> Route transition uses Fade cover/reveal
  -> Loading surface participates in the Route operation
  -> active Basic Flow Activity tears down
  -> any Activity-owned scene content is released
  -> Route_Hub becomes current
  -> Activity = None
  -> Route_Hub explicit Silence is restored by destination Route intent
```

The cycle is repeatable and the proven paths complete with `blockingIssues=0`.

## Content composition

`SCN_GameFlow_Basic.unity` is the Route Primary Scene for `Route_BasicFlow.asset`. It remains the scene-owned Route composition while `Activity_Basic_A`, `Activity_Basic_B` and `Activity_Basic_C` switch.

The Basic Flow deliberately demonstrates three Activity/content cases:

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

  Route-owned navigation
    Go to Activity C
      no ActivityContentBinding required
      remains available independently of A/B local visibility

SCN_GameFlow_Basic_A.unity
  Activity-owned scene content
    materialized while Activity_Basic_A is active

SCN_GameFlow_Basic_B.unity
  Activity-owned scene content
    materialized while Activity_Basic_B is active

Activity_Basic_C
  no ActivityContentProfile
  no Activity-owned scene
  no C-specific Activity-local content
```

Activity-local content is authored inside the Route scene and is activated/deactivated by `ActivityContentBinding`. It is not loaded or unloaded as an Activity scene.

Activity-owned scene content is separate composition declared by `ActivityContent_Basic_A.asset` and `ActivityContent_Basic_B.asset`; those scenes are materialized and released with their Activities.

Activity C intentionally has no Activity Content Profile. Its negative proof is that entering C releases the previous A/B Activity scene and leaves A/B local content hidden without inventing C content.

This distinction is intentional:

```text
Activity-local visibility
  !=
Activity scene materialization
  !=
requirement that every Activity own content
```

## Transition and Loading presentation

`SCN_GameFlow_Persistence.unity` now explicitly contains the persistent presentation adapters used by this sample:

```text
Transition
  UnityFadeCurtainEffectAdapter

Loading
  UnityLoadingSurfaceAdapter
```

The runtime resolves those adapters from Persistent Content. The sample proves two presentation rules:

```text
Route switch
  -> always covered through the Route Transition envelope
  -> uses the configured Fade surface
  -> uses the configured Loading surface during Route work

Activity request
  -> presentation follows the target ActivityVisualTransitionMode

  A/B target = Seamless
    -> no visual Transition
    -> no Loading presentation

  C target = Fade
    -> Fade cover/reveal
    -> no Loading presentation
```

This closes the baseline Transition demonstration without creating a separate Transition Route or HUB item. Readiness-governed `FadeWithLoading`, `WaitCovered`, participant-aware progress and recovery remain separate later proof.

## Inspect

```text
GameApplication_GameFlow.asset
Routes/Route_Hub.asset
Routes/Route_BasicFlow.asset
Activities/Activity_Basic_A.asset
Activities/Activity_Basic_B.asset
Activities/Activity_Basic_C.asset
Activities/ActivityContent_Basic_A.asset
Activities/ActivityContent_Basic_B.asset
Scenes/SCN_GameFlow_Persistence.unity
Scenes/SCN_GameFlow_Hub.unity
Scenes/SCN_GameFlow_Basic.unity
Scenes/SCN_GameFlow_Basic_A.unity
Scenes/SCN_GameFlow_Basic_B.unity
```

There is intentionally no `ActivityContent_Basic_C.asset` and no `SCN_GameFlow_Basic_C.unity`.

The GameApplication currently keeps Player Session disabled because Player is not required to explain this Basic Flow vertical.

## Framework concepts

```text
GameApplication ownership
Persistent Content
persistent Transition presentation
persistent Loading presentation
Route ownership
Route with no Startup Activity
Route with Startup Activity
mandatory covered Route switch
Activity-local content / visibility
Activity-owned scene composition
valid content-less Activity
explicit Route request
explicit Activity request
Activity Seamless presentation
Activity Fade presentation
contextual Route / Activity BGM intent
BGM no-request preservation
teardown and return to Activity None
```

The contextual BGM path is **PROVEN** for the current Basic Flow cycle:

```text
Route_Hub Silence
  -> Activity_Basic_A BGM
  -> Activity_Basic_B BGM
  -> Route_Hub Silence
```

The no-request preservation case is also **PROVEN**:

```text
Activity_Basic_A BGM
  -> Activity_Basic_C publishes no new BGM intent
  -> Activity A BGM remains confirmed

Activity_Basic_B BGM
  -> Activity_Basic_C publishes no new BGM intent
  -> Activity B BGM remains confirmed
```

Owner exit preserves the confirmed BGM presentation; only an explicit destination Play or Silence intent changes it.

## Evolutionary scenarios

Composition / Visibility is no longer a separate planned scenario: its basic contract is demonstrated directly inside Basic Flow through `ActivityContentBinding`, Activity-owned scene composition and the negative content-less Activity C case.

Basic Transition presentation is also no longer a separate planned scenario: Route Fade + Loading, Activity Seamless and Activity Fade are demonstrated directly in the same coherent Basic Flow cycle.

The remaining catalog is intentionally not frozen. Current candidates include:

```text
Loading Readiness / participant-aware progress
Restart / Recovery
contextual Camera coverage where natural
additional Audio coverage only where it teaches a new contract
```
