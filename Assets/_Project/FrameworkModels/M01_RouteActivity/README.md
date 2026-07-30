# M01 — Route and Activity

Status: Closed  
Started: 2026-07-29  
Closed: 2026-07-30

## Purpose

Demonstrar a composição mínima de `GameApplication`, `Route` e `Activity` sem Player, gameplay Camera,
Reset ou Pause.

## Final flow

```text
Boot
→ Menu Route
→ Gameplay Route + startup Activity A
→ Activity B
→ Activity A
→ Menu Route
→ Gameplay Route + startup Activity A novamente
```

## Final authoring graph

```text
GA_M01_RouteActivity
  Content Scene: M01_PersistentContent
  Startup Route: Route_M01_Menu
  Local Player Slots: empty
  Validation Mode: Standard

Route_M01_Menu
  Primary Scene: M01_Menu
  First Activity: None

Route_M01_Gameplay
  Primary Scene: M01_Gameplay
  First Activity: Activity_M01_A

Activity_M01_A
  Profile: ActivityContent_M01_A
  Scene: M01_ActivityA_Add
  Projection: No Slots

Activity_M01_B
  Profile: ActivityContent_M01_B
  Scene: M01_ActivityB_Add
  Projection: No Slots
```

## Runtime policy proven

```text
configured Slots = 0
→ Player participation = NotConfigured
→ Player Actor preparation = NotConfigured
→ Player gameplay = NotConfigured
→ Scene Local Player admission = NotConfigured
→ no Player/Slot/host fallback
→ Game Flow starts normally
```

## Evidence

```text
[GAME_APPLICATION_VALIDATION_SCOPE_SMOKE]
status='Passed' cases='3'
zeroSlots='ValidOptional'
localScope='Isolated'
projectAudit='Explicit'

[ZERO_SLOT_BOOTSTRAP_COMPOSITION_POLICY_SMOKE]
status='Passed' cases='5'
zeroSlots='PlayerRuntimeDisabled'
sceneAdmission='NotConfigured'
configuredSlots='PlayerRuntimeEnabled'

[P3F_SESSION_SLOT_RUNTIME_SMOKE]
status='Passed' cases='17'

[M01_ZERO_PLAYER_BOOT_SMOKE]
status='Passed' cases='5'
configuredSlots='0'
playerRuntime='NotConfigured'
sceneAdmission='NotConfigured'
startupScene='M01_Menu'
```

Play Mode also proved:

```text
Menu → Gameplay succeeded;
Gameplay startup Activity A reached Ready;
A → B and B → A each loaded 1 and released 1 Activity scene;
return to Menu released the active Activity scene;
reentry started Activity A again;
activitySceneLedgerStale = 0;
blockingIssues = 0.
```

## Acceptance

- [x] Application validates with zero Player Slots.
- [x] Menu starts without an Activity.
- [x] Gameplay starts Activity A.
- [x] Activity A/B switching works repeatedly.
- [x] Gameplay remains while Activity content changes.
- [x] Back to Menu releases Gameplay and Activity content.
- [x] Reentry starts Activity A again.
- [x] No Player fallback is created.
- [x] No blocking issue was reported.

## Current Context Display

`PF_M01_CurrentContextDisplay` remains a presentation-only placeholder and is not part of the acceptance.
The package currently exposes no public typed authoring binding for Current Route and Current Activity. Do not
fill it through reflection, `FindObjectOfType`, internals or a global service locator.

## UX findings

| ID | Area | Observation | Destination |
|---|---|---|---|
| UX-M01-001 | Validation/runtime | Player was treated as universally required. | Fixed in package and covered by QA. |
| UX-M01-002 | Persistent Content | The minimum source includes Camera, Loading, Transition and Pause. | Review application Recipes/Templates. |
| UX-M01-003 | Context presentation | No public typed binding for Current Route/Activity. | Package product surface. |
| UX-M01-004 | Diagnostics | Debug request logs are too verbose for normal use. | Separate Info, Debug and Trace evidence. |

## Reusable pieces

```text
PF_M01_RouteNavigation
PF_M01_ActivityNavigation
```

These prefabs remain candidates, not official templates. They must be proven in another model before promotion.

## Final state

```text
Authoring: Complete
Technical QA: Passed
Play Mode Review: Passed
UX Review: Recorded
Model: Closed
```
