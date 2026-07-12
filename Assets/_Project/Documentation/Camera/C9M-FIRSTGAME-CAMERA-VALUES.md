# C9M — FIRSTGAME Camera Values

## Output

| Field | Value |
|---|---|
| Output Id | `camera.output.main` |
| Initialize On Awake | `true` |
| Log Diagnostics | `true` |

## Route request

| Field | Value |
|---|---|
| Scope Id | `firstgame.route.gameplay.camera` |
| Request Id | `firstgame.camera.request.route.gameplay` |
| Precedence | `10` |
| Tie Breaker Id | `firstgame.route.gameplay` |

## Local Player request

| Field | Value |
|---|---|
| Eligibility Scope Id | `firstgame.player.local.1.camera` |
| Request Id | `firstgame.camera.request.player.local.1` |
| Precedence | `50` |
| Tie Breaker Id | `firstgame.player.local.1` |
| Eligible On Enable | `true` |
| Release On Disable | `true` |

## Activity request

| Field | Value |
|---|---|
| Scope Id | `firstgame.activity.gameplay.camera-override` |
| Request Id | `firstgame.camera.request.activity.gameplay-override` |
| Precedence | `100` |
| Tie Breaker Id | `firstgame.activity.gameplay-override` |

## Expected arbitration

```text
Activity 100
> Player 50
> Route 10
```

## Follow framing

| Rig | Follow Offset | Intent |
|---|---|---|
| Route | `(0, 12, -14)` | Wide gameplay overview |
| Local Player | `(0, 5, -8)` | Playable follow framing |
| Activity | `(8, 10, -8)` | Distinct lateral override |

These values are also serialized in each `CameraRigComposer`, so a later
Apply/Rebuild preserves the intended framing.

## Canonical references

```text
PlayerComposer:
  PlayerPrototype

Player Follow:
  PlayerPrototype/Anchors/CameraTarget

Player LookAt:
  PlayerPrototype/Anchors/LookAtTarget

Output:
  Camera/Output/Gameplay Camera

Route Rig:
  Camera/Rigs/Route Rig

Player Rig:
  Camera/Rigs/Player Rig

Activity Rig:
  Camera/Rigs/Activity Override Rig
```
