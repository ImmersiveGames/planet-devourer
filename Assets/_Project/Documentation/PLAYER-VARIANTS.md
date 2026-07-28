# FIRSTGAME — Logical Player Variants

Status: Current comparison guide  
Last updated: 2026-07-28

## Core rule

The framework has one Session-scoped Logical Player participation authority.

```text
Manager-Provisioned Logical Player
Scene-Provided Logical Player
Session-Persistent Logical Player
  -> PlayerParticipationRuntimeContext
  -> typed PlayerSlotId
```

The source describes how a Logical Player enters the Session. It does not create a separate Player runtime or Slot registry.

## Canonical terms

```text
PlayerSlotProfile / PlayerSlotId
  stable participation seat

Logical Player
  Session participant associated with one Slot

Local Player Host
  optional physical Unity Input System host
  commonly contains PlayerInput and LocalPlayerHostAuthoring

ActorProfile
  immutable selectable Actor identity

Logical Actor
  contextual Actor identity/runtime state associated with the Logical Player

Actor materialization / presentation
  concrete gameplay and visual content
```

A Logical Player alone does not imply Host, Actor selection, Logical Actor, materialization, Camera, input or gameplay readiness.

## Source selection

| Question | Choose |
|---|---|
| Does the Route or Activity scene already contain the Player Host/Actor? | Scene-Provided |
| Must a device join create the Player Host at runtime? | Manager-Provisioned |
| Must the Logical Player identity exist outside all Routes/Activities? | Session-Persistent — not yet available |

## Comparison

| Dimension | Scene-Provided | Manager-Provisioned | Session-Persistent |
|---|---|---|---|
| Logical Player origin | Route/Activity scene | explicit join transaction | Application/Session composition |
| Physical Host origin | already authored | created by `PlayerInputManager` | optional preexisting Session content or later preparation |
| Slot authority | framework | framework | framework Session authority |
| Slot pre-authored on prefab | no | no | no |
| Actor already present | commonly yes | normally no | optional |
| Physical ownership | external scene-owned | framework-provisioned/context-owned | session-persistent physical ownership |
| Route/Activity owns Logical Player identity | no | no | no |
| Current package runtime | implemented | implemented | not implemented |
| Current FIRSTGAME state | active test | next test | blocked |

---

## 1. Scene-Provided Logical Player

### Use when

A Route or Activity scene already owns the concrete Player composition.

Typical consumer cases:

```text
single-player level with one authored protagonist
character already positioned in the scene
scene-specific vehicle or controllable Actor
```

### Canonical prefab boundaries

```text
Actor_PlayerSceneProvided
  PlayerActorDeclaration
  Actor gameplay components
  movement
  Camera anchors
  Visual

Player_SceneProvided
  PlayerInput
  LocalPlayerHostAuthoring
  SceneLocalPlayerAdmissionAuthoring
  Actor Mount
    Actor_PlayerSceneProvided
```

The `ActorProfile.LogicalActorHostPrefab` references the nested Actor prefab, never the outer Player composition.

### Runtime behavior

```text
scene loads
-> framework resolves explicit Scene-Provided authoring
-> Slot is reserved/admitted
-> existing Host is validated
-> existing Logical Actor is correlated/adopted
-> contextual input/Camera/gameplay eligibility proceeds
-> Activity/Route exit releases contextual evidence
-> scene owns physical destruction
```

The framework must not silently instantiate, destroy, deactivate or duplicate provided parts.

### FIRSTGAME mapping

```text
Route
  FG_PlayerSceneProvider

Primary scene
  SceneProvidedGameplay

Prefabs
  Actor_PlayerSceneProvided
  Player_SceneProvided
  Player_SceneProvided_With_Pause
  Player_SceneProvided_With_Camera
```

### Current state

```text
package runtime implemented
FIRSTGAME authoring present
Player Camera authoring present
focused Play Mode proof in progress
```

---

## 2. Manager-Provisioned Logical Player

### Use when

A join action must create a local Player Host at runtime.

Typical consumer cases:

```text
couch multiplayer
press-to-join
runtime device pairing
optional second local Player
```

### Canonical composition

Persistent Content:

```text
Local Player Provisioning
  PlayerInputManager
  LocalPlayerProvisioningAuthoring
  LocalPlayerProvisioningHostRegistration
```

Player prefab:

```text
Player_ManagerProvisioned
  PlayerInput
  LocalPlayerHostAuthoring
  Actor Mount
    empty before Actor preparation
```

### Runtime behavior

```text
explicit authorized join
-> reserve ordered free Slot
-> PlayerInputManager manual join
-> validate created Host
-> admit one Logical Player
-> bind typed PlayerSlotId
-> select/prepare Actor according to policy
-> commit
```

A failure must explicitly roll back the Slot reservation, Host evidence and physical instance owned by the failed transaction.

### Authority boundaries

`PlayerInputManager`:

```text
creates the physical Host
pairs devices
provides Unity player index as diagnostics
```

Framework:

```text
authorizes join
reserves Slot
owns typed Slot assignment
admits Logical Player
controls commit/rollback
```

### FIRSTGAME mapping

Not yet rebuilt on the current package surface.

It must receive a separate menu entry, Route and scene so its creation and ownership differences remain visible beside the Scene-Provided variant.

### Current state

```text
package runtime implemented
package authoring exists
FIRSTGAME consumer assembly planned next
```

---

## 3. Session-Persistent Logical Player

### Use when

The Logical Player identity must exist at Session scope, outside any Route or Activity.

Intended behavior:

```text
Game Application / Session
  -> Session-Persistent Logical Player
  -> PlayerParticipationRuntimeContext

Route / Activity
  -> projects and consumes the Logical Player
  -> may prepare/adopt contextual Actor content
  -> does not own Session identity or lifetime
```

Potential examples:

```text
one signed-in local participant crossing menu and gameplay Routes
persistent local party membership
Session-owned Player identity with Activity-specific Actor materialization
```

### Important distinction

A Player prefab placed in `Conteiner Scene` is not automatically a Session-Persistent Logical Player.

Without an official contract it would only be a persistent GameObject. It would not establish:

```text
typed Logical Player admission
Slot assignment authority
physical ownership evidence
Actor correlation
contextual release policy
materialization reconciliation
```

### Current state

```text
architecture accepted
runtime not implemented
product authoring not implemented
QA proof absent
FIRSTGAME blocked
```

FIRSTGAME must not invent this missing authority. The solution belongs in `com.immersive.framework`, followed by QA and then consumer proof.

---

## Assembly order in FIRSTGAME

```text
1. Close Scene-Provided admission and Camera proof.
2. Build a separate Manager-Provisioned comparison Route.
3. Record UX friction and reusable product gaps.
4. Implement Session-Persistent only through an official package cut.
5. Converge the selected production variant into the real game loop.
```

## Rejected shortcuts

Do not:

- infer Slot from `PlayerInput.playerIndex`;
- pre-author a generic Slot identity on Player prefabs;
- use object names, tags or hierarchy search as identity;
- allow `PlayerInputManager` to choose framework Slot or Actor policy;
- create a second Player runtime or Slot registry;
- silently adopt the first Player found;
- treat Actor/materialization as inherent to Logical Player admission;
- create a local Session-Persistent workaround in FIRSTGAME.
