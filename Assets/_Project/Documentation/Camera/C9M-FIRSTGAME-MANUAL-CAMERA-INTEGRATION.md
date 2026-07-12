# C9M — FIRSTGAME Manual Camera Integration

## Objective

Prove the real FIRSTGAME camera flow using the official framework product surface:

```text
Route camera
-> Local Player camera
-> Activity override
-> Local Player restoration
-> Route restoration when Player eligibility is released
```

## Type

```text
real integration
product usability
manual scene authoring
```

## Rules for this cut

```text
No scene builder.
No installer.
No automatic GameObject creation.
No scene or prefab file is included in this package.
No FIRSTGAME-specific camera runtime authority is introduced.
```

All GameObjects and component wiring are created manually in the Unity Inspector.

---

# 1. Existing FIRSTGAME baseline

Use:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Expected existing Player:

```text
PlayerPrototype
├── Visual
└── Anchors
    ├── CameraTarget
    └── LookAtTarget
```

Expected Player root components include:

```text
PlayerInput
PlayerComposer
PlayerActorDeclaration
PlayerSlotDeclaration
UnityPlayerInputGateAdapter
CharacterController
FirstGameTestPlayerMovement
```

Do not rebuild or replace the Player.

---

# 2. Final hierarchy to create manually

Create this organization in `FG_Gameplay`:

```text
Camera
├── Output
│   └── Gameplay Camera
├── Rigs
│   ├── Route Rig
│   ├── Player Rig
│   └── Activity Override Rig
└── Bindings
    ├── Route Camera Binding
    └── Activity Camera Binding
```

Add the Player binding directly to:

```text
PlayerPrototype
```

The `Camera`, `Rigs` and `Bindings` objects are organizational only. They are not runtime authorities.

---

# 3. Output

## GameObject

```text
Camera/Output/Gameplay Camera
```

## Components

```text
UnityEngine.Camera
CinemachineBrain
CameraOutputSessionBinding
```

## CameraOutputSessionBinding

```text
Output Id: camera.output.main
Unity Camera: Gameplay Camera
Cinemachine Brain: Gameplay Camera
Initialize On Awake: true
Log Diagnostics: true
```

There must be exactly one output authority for this viewport.

Do not use:

```text
Camera.main lookup
a second active AudioListener
another CameraOutputSessionBinding for camera.output.main
```

---

# 4. Route rig

## GameObject

```text
Camera/Rigs/Route Rig
```

## Component

```text
CameraRigComposer
```

Create or assign its materialized Cinemachine Camera through the official composer workflow.

Suggested intent:

```text
stationary or environmental framing
lower precedence than Player and Activity
Follow Target: explicit scene Transform
Look At Target: explicit scene Transform when required
```

Create scene targets manually when needed, for example:

```text
Camera/Targets/Route Follow
Camera/Targets/Route LookAt
```

The target objects are presentation targets only.

---

# 5. Player rig

## GameObject

```text
Camera/Rigs/Player Rig
```

## Component

```text
CameraRigComposer
```

Configure the rig to use the real Player anchors:

```text
Follow Target:
  PlayerPrototype/Anchors/CameraTarget

Look At Target:
  PlayerPrototype/Anchors/LookAtTarget
```

Run the official `CameraRigComposer` Apply/Rebuild flow.

Confirm that the materialized Cinemachine Camera references the same targets exposed by `PlayerComposer`.

Do not attach camera-selection logic to `PlayerComposer`.

---

# 6. Activity override rig

## GameObject

```text
Camera/Rigs/Activity Override Rig
```

## Component

```text
CameraRigComposer
```

Use an obvious test framing so the override is visually distinguishable from the Player camera.

Suggested test intent:

```text
fixed overview
side angle
goal/objective framing
```

Create explicit target objects when needed.

---

# 7. Route camera request binding

## GameObject

```text
Camera/Bindings/Route Camera Binding
```

This object must participate in the canonical Route content lifecycle already used by the Gameplay Route.

## Component

```text
RouteCameraRequestBinding
```

## Configuration

```text
Assigned Route:
  the real FIRSTGAME Gameplay Route asset

Scope Id:
  firstgame.route.gameplay.camera

Request Id:
  firstgame.camera.request.route.gameplay

Output Session:
  Camera/Output/Gameplay Camera

Rig Composer:
  Camera/Rigs/Route Rig

Target Source:
  explicit Route target Transform

Precedence:
  10

Tie Breaker Id:
  firstgame.route.gameplay

Log Diagnostics:
  true
```

Expected lifecycle:

```text
Gameplay Route enter
-> Route request Published

Gameplay Route exit
-> Route request Released
```

---

# 8. Local Player camera request binding

## GameObject

```text
PlayerPrototype
```

## Component

```text
LocalPlayerCameraRequestBinding
```

## Configuration

```text
Player Composer:
  PlayerPrototype.PlayerComposer

Eligibility Scope Id:
  firstgame.player.local.1.camera

Request Id:
  firstgame.camera.request.player.local.1

Output Session:
  Camera/Output/Gameplay Camera

Rig Composer:
  Camera/Rigs/Player Rig

Precedence:
  50

Tie Breaker Id:
  firstgame.player.local.1

Eligible On Enable:
  true

Release On Disable:
  true

Log Diagnostics:
  true
```

For the current single-player FIRSTGAME proof:

```text
Eligible On Enable = true
```

This is an explicit single-player policy for this proof.

When a real local-player eligibility runtime exists later:

```text
Eligible On Enable = false
runtime calls SetLocalPlayerEligible(true/false)
```

Do not infer local ownership through:

```text
PlayerInput.playerIndex
GameObject name
tag
Camera.main
global lookup
```

---

# 9. Activity camera request binding

Use the real Activity content object that receives the canonical Activity lifecycle.

Do not place the binding on an unrelated persistent object.

## Component

```text
ActivityCameraRequestBinding
```

## Configuration

```text
Assigned Activity:
  the real FIRSTGAME Gameplay Activity asset

Scope Id:
  firstgame.activity.gameplay.camera-override

Request Id:
  firstgame.camera.request.activity.gameplay-override

Output Session:
  Camera/Output/Gameplay Camera

Rig Composer:
  Camera/Rigs/Activity Override Rig

Target Source:
  explicit Activity target Transform

Precedence:
  100

Tie Breaker Id:
  firstgame.activity.gameplay-override

Log Diagnostics:
  true
```

Expected lifecycle:

```text
Activity enter
-> Activity request Published

Activity exit
-> Activity request Released
-> previous Player request restored
```

---

# 10. Required precedence

Use:

```text
Route:    10
Player:   50
Activity: 100
```

Expected winner order:

```text
Activity > Player > Route
```

Do not use Cinemachine priority as independent cross-owner arbitration.

The framework request precedence is the authority. Cinemachine remains the presentation engine.

---

# 11. Manual validation sequence

The committed `FirstGameC9MCameraValidationControls` component exposes the
canonical operations without disabling lifecycle objects:

```text
F6  Clear the current Activity through ActivityRequestTrigger
F7  Request FG Activity A through ActivityRequestTrigger
F8  Set Local Player camera eligibility to false
F9  Set Local Player camera eligibility to true
F10 Capture current output/winner evidence without changing state
```

Each completed operation emits one `[C9M_FIRSTGAME_CAMERA] evidence` record
containing the admitted request count, winner request, typed owner,
precedence, rig and the three binding statuses. The controller observes and
reports the framework state; it never selects a winner or changes a rig
directly.

## Test A — Gameplay entry

Enter the Gameplay Route normally.

Expected:

```text
Route binding: Published
Player binding: Published
Activity binding: Published when startup Activity is active
Winner: Activity
```

When the startup Activity is not intended to override immediately, configure the override on a separate test Activity instead.

## Test B — Activity release

Press `F6`. This invokes `ActivityRequestTrigger.ClearActivity()` through the
real framework flow.

Expected:

```text
Activity binding: Released
Player binding: still Published
Winner: Player
visible camera returns to Player rig
```

## Test C — Activity re-entry

Press `F7`. This invokes `ActivityRequestTrigger.RequestActivity()` for the
real `FG Activity A` asset.

Expected:

```text
Activity binding: Published
Winner: Activity
visible camera changes to Activity rig
```

## Test D — Player eligibility release

After leaving the Activity, press `F8`. This calls on the Player binding:

```csharp
SetLocalPlayerEligible(false)
```

Do not disable the component or `PlayerPrototype`; that would mix Camera
eligibility evidence with unrelated Player lifetime and input changes.

Expected:

```text
Player binding: Released
Route binding: still Published
Winner: Route
```

Press `F9` to re-enable eligibility:

```csharp
SetLocalPlayerEligible(true)
```

Expected:

```text
Player binding: Published
Winner: Player
```

## Test E — Route exit

Leave the Gameplay Route through the real Route request flow.

Expected:

```text
Activity request: Released or absent
Player request: Released when Player object/lifetime ends
Route request: Released
CameraOutputContext: no stale Gameplay winner
```

---

# 12. Required logs

Expected representative evidence:

```text
[FRAMEWORK_CAMERA] Route Camera Request Binding status='Published'
[FRAMEWORK_CAMERA] Local Player Camera Request Binding status='Published'
[FRAMEWORK_CAMERA] Activity Camera Request Binding status='Published'

[FRAMEWORK_CAMERA] Activity Camera Request Binding status='Released'
[FRAMEWORK_CAMERA] Local Player Camera Request Binding status='Released'
[FRAMEWORK_CAMERA] Route Camera Request Binding status='Released'

[C9M_FIRSTGAME_CAMERA] evidence step='gameplay-entry' ... winnerOwner='Activity:...' winnerPrecedence='100' ... blockingIssues='0'
[C9M_FIRSTGAME_CAMERA] evidence step='activity-cleared' ... winnerOwner='LocalPlayer:player.1' winnerPrecedence='50' ... blockingIssues='0'
[C9M_FIRSTGAME_CAMERA] evidence step='activity-requested' ... winnerOwner='Activity:...' winnerPrecedence='100' ... blockingIssues='0'
[C9M_FIRSTGAME_CAMERA] evidence step='player-released' ... winnerOwner='Route:...' winnerPrecedence='10' ... blockingIssues='0'
[C9M_FIRSTGAME_CAMERA] evidence step='player-eligible' ... winnerOwner='LocalPlayer:player.1' winnerPrecedence='50' ... blockingIssues='0'
```

The exact winner must follow:

```text
Activity
Player
Route
```

No blocking issue should occur in the valid configuration.

---

# 13. Failure checks

Temporarily validate at least one negative case without saving the invalid scene state.

Recommended case:

```text
clear Eligibility Scope Id
```

Expected:

```text
Local Player Camera Request Binding status='Blocked'
diagnostic mentions explicit stable eligibility scope id
```

Restore the valid value before saving.

Other valid negative checks:

```text
missing CameraOutputSessionBinding
missing PlayerComposer
missing Player CameraTarget
missing rig composer
missing request id
missing tie-breaker id
```

No invalid configuration may fall back silently.

---

# 14. Product acceptance

```text
designer can identify the output
designer can identify Route, Player and Activity rigs
designer can see where each request is authored
Player uses real PlayerComposer anchors
Activity override is visually understandable
release restores the previous camera
Advanced diagnostics expose request state
no technical smoke menu is required for normal use
```

---

# 15. Technical acceptance

```text
FIRSTGAME compiles
Gameplay Route enters successfully
Player movement remains functional
one CameraOutputSession owns camera.output.main
Route, Player and Activity publish valid requests
Activity release restores Player
Player release restores Route
Route exit removes stale requests
no Camera.main fallback
no global singleton or service locator
no direct owner priority competition
```

---

# 16. Files changed by this package

```text
Created:
Assets/_Project/Documentation/Camera/C9M-FIRSTGAME-MANUAL-CAMERA-INTEGRATION.md
Assets/_Project/Documentation/Camera/C9M-FIRSTGAME-CAMERA-VALUES.md
Assets/_Project/Documentation/Camera/C9M-FIRSTGAME-VALIDATION-REPORT-TEMPLATE.md

Modified:
none

Removed:
none
```

Scene and object changes are intentionally performed manually and are not included in the ZIP.

---

# 17. Suggested commit

```text
C9M — integrate canonical camera requests in FIRSTGAME
```
