# Player Samples

Status: **PLAYER SCOPE GOVERNED BY FG-ADR-002 REVISION 10 — LOCAL MULTIPLAYER CAMERA-032 SHARED-GROUP CONSUMER PASS 2026-09-22**

Canonical Player sample authority:

```text
Assets/Documentation~/Architecture/ADRs/
  FG-ADR-002-Player-Sample-Scope-and-Demonstration-Architecture.md
```

General sample-program strategy remains in FG-ADR-001.

## Naming model

The Player samples use short product-facing names while preserving the runtime provisioning terms.

```text
Local Player Host
  common technical host for one local Player

Scene Player
  Local Player Host already authored in a Scene
  HostProvisioning = SceneProvided

Player Provisioning
  Session/UIGlobal authority that can create Local Player Hosts
  HostProvisioning = ManagerProvisioned
```

`SceneProvided` and `ManagerProvisioned` remain valid runtime modes. They are not presented as two peer Player Host object types.

## Canonical Scene Player coverage

`Assets/_Sample/GettingStarted/MinimalGame/` is the canonical executable reference for the Scene Player flow.

It demonstrates the current Scene-Provided Player composition, gameplay readiness, gameplay input binding, camera and minimal Move / Look behavior.

These contracts are intentionally **not duplicated** as a dedicated Scene Player Demonstration Application under Player.

## Current Player demonstrations

| Demonstration | Runtime / initial policy | Status | Meaning |
|---|---|---|---|
| Getting Started / Minimal Game | `SceneProvided` | **CANONICAL / PROVEN** | Scene Player reference |
| Player Provisioning | `ManagerProvisioned` + configured Default Actor | **MATERIALIZED / PLAY MODE PROVEN** | Session-authorized Local Player Host creation/provisioning |
| Character Selection | `ManagerProvisioned` + `ActorResolution = LeaveUnresolved` | **CAMERA-032-D/E UNITY VALIDATED — 2026-09-22** | Explicit Actor choice plus Player-specific Third Person Camera selection |
| Local Multiplayer | `ManagerProvisioned` + two configured Slots | **CAMERA-032 SHARED-GROUP UNITY PASS — 2026-09-22** | Two-Player Join/Leave/Rejoin plus one shared Group Camera Presentation |

This is the current implementation sequence, not a permanent closed catalog.

## Current shared Player prefab baseline

The Player samples now have concrete cross-application reuse, so the shared technical prefab baseline is materialized under:

```text
Assets/_Sample/PlayerSamples/Shared/Prefabs/
  FG_Player.prefab
  FG_PlayerActor.prefab
  FG_Presentation.prefab
```

These prefabs capture reusable technical composition. They do **not** move `GameApplication`, `PlayerSessionProfile`, Route, Activity or application-specific authority into Shared.

Concrete Actor presentation variants are authored separately and may derive from the shared `FG_Presentation` baseline.

## Player Provisioning

Player Provisioning demonstrates the smallest coherent public consumer path for Session-authorized Local Player Host provisioning:

```text
Player Provisioning authority
  -> Local Player Host Prefab
  -> explicit Join request
  -> Local Player Host instance
  -> configured Default Actor selection / preparation
  -> Player Actor Runtime Host
  -> configured Actor Presentation
  -> admission
  -> Session ownership
  -> GameplayReady
```

The provisioning object is **not itself a Player Host**.

The application is Play Mode proven, including Join, gameplay, Camera and Leave/Rejoin behavior. See its local README for the exact evidence and composition.

## Current public Player Session surface

Canonical distinction:

```text
PlayerSessionObserver
  = read / presentation observation

IPlayerSessionScopedAccess
  = provider-neutral scoped Session observation + request surface

explicit Player Session Command Trigger
  = authored request/change component
```

The documented public command family contains:

```text
PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionSelectActorCommandTrigger
PlayerSessionDefaultActorSelectionCommandTrigger
PlayerSessionReplaceActorSelectionCommandTrigger
PlayerSessionClearActorSelectionCommandTrigger
PlayerSessionLeaveCommandTrigger
```

Each command component represents one request and owns only its own typed result evidence.

`PlayerSessionObserver` remains read-only. `IPlayerSessionScopedAccess` provides the current scoped Session snapshot/observation surface, including `Changed` and `TryGetObservation(...)`, without requiring a global Session authority.

## Character Selection — closed on current Player architecture

Character Selection keeps the same canonical Session intent:

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

The lifecycle originally proven on 2026-08-28 remains:

```text
Open Joining
  -> Join
  -> Slot Joined
  -> Actor unresolved
  -> Preparing / WaitingForActorSelection
        ↓
PlayerSessionObserver.OnPlayerJoined
  -> show Character Selection UI
        ↓
application-owned ActorProfile choices
  -> Farmer / Cow
        ↓
PlayerSessionSelectActorCommandTrigger.Invoke()
        ↓
Framework-owned selection commit
  -> Actor preparation
  -> physical materialization
  -> Activity participation / GameplayReady
        ↓
PlayerSessionObserver.OnActorSelected
  -> hide Character Selection UI
```

Leave/Rejoin returns to `WaitingForActorSelection` and supports another explicit choice without passing through a failed readiness state.

### Current Actor presentation chain

The physical composition was rebuilt after the Player Actor / Presentation architecture changes.

Current Character Selection Actor assets are:

```text
ActorProfile_Farmer
  -> PresentationPrefab = FG_FarmerPresentation

ActorProfile_Cow
  -> PresentationPrefab = FG_CowPresentation
```

The concrete prefabs are authored under:

```text
Assets/_Sample/PlayerSamples/Player/Players/
  FG_FarmerPresentation.prefab
  FG_CowPresentation.prefab
```

Both use the shared `FG_Presentation` technical baseline. The selected `ActorProfile` therefore determines the presentation materialized by the current Player Actor runtime composition.

The old `LogicalActorHostPrefab` composition is not part of the current sample contract.

### Character Selection presentation boundary

The sample owns presentation and choice catalog only.

`CharacterSelection_UI.unity` uses a Route-scoped `PlayerSessionObserver` to show/hide the selection controls:

```text
On Player Joined  -> show
On Actor Selected -> hide
On Player Left    -> hide
```

Each button keeps `PlayerSessionSelectActorCommandTrigger` as the authority for the Actor choice.

The sample-owned `CharacterSelectionActorButtonPresenter` reads that command's `ActorProfile` and projects:

```text
ActorProfile.DisplayName -> label
ActorProfile.Icon        -> image
```

No second ActorProfile reference is authored in the presenter.

### LeaveUnresolved semantics

For Character Selection:

```text
Joined + no selected Actor
  = valid pending state
  = Preparing / WaitingForActorSelection
```

The Framework does not invoke Default Actor resolution in this state. Explicit selection advances the lifecycle.

Normal Character Selection demonstrates **initial explicit Select** only. Do not expose Replace/Clear merely because those APIs exist, and do not use Default selection as a hidden fallback.

### Character Selection proof history

Historical lifecycle proof — **2026-08-28**:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
sessionChangeObservation = PASS
designerEventProjection = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

Current physical-composition reproof — **2026-09-05**:

```text
Join
-> WaitingForActorSelection
-> Farmer / Cow explicit selection
-> correct PresentationPrefab materialized
-> Follow camera functional
-> gameplay movement/input functional
-> GameplayReady
-> Leave / Rejoin
-> fresh explicit selection functional
```

The 2026-09-05 proof remains historical Player/materialization evidence. Character Selection was subsequently migrated and validated on the CAMERA-032-D/E model on 2026-09-22:

~~~text
GameApplication -> one explicit Session Output
Route -> CameraPresentation_Default_Route
Activity -> CameraPresentation_ThirdPerson
Player Slot -> CameraPresentation_ThirdPerson ExplicitSelection
Farmer/Cow -> fresh Actor Camera Subject occurrence
Leave -> Route Presentation
Rejoin + fresh selection -> Third Person Presentation
~~~

Final UPM promotion/import proof remains pending at the Player group level.

## Scoped binding and command availability

Keep authoring configuration and runtime availability distinct:

```text
valid Route / Activity authoring
  !=
current scoped access Bound
```

A valid command can temporarily be runtime-unbound and must reject without global lookup, alternate Session authority or direct mutation.

Presentation gating must use public scoped observation/binding evidence rather than a fallback authority.

## Local Multiplayer — shared Group Camera consumer

Local Multiplayer is materialized under:

~~~text
Assets/_Sample/PlayerSamples/LocalMultiplayer/
~~~

The historical public Slot/device/input ownership blocker is closed for the implemented path.

The Player flow remains:

~~~text
Open Joining
  -> InputDevice intent
  -> PlayerSessionJoinCommandTrigger
  -> Framework allocates next available configured Slot
  -> Local Player Host admitted
  -> configured Default Actor selected/prepared
  -> Actor Presentation materialized
  -> Activity placement
  -> GameplayReady
~~~

The sample has two configured Slots and derives UI/device state from `IPlayerSessionScopedAccess` current observation. Historical successful Join count is never occupancy authority.

### Camera-032 topology

Local Multiplayer is a shared-screen sample:

~~~text
GameApplication_LocalMultiplayer
  Camera Session
    PF_CameraOutput_Main
    Player Output Bindings = []
    Player Presentation Bindings = []

Route_LocalMultiplayer
  -> CameraPresentation_Default_Route [200 / Cut]

Activity_LocalMultiplayer
  -> CameraPresentation_LocalMultiplayer [300 / Blend]
     Subject Policy = AllAvailableSubjects
     Rig = PF_LocalMultiplayer_Activity_Presentation

PF_LocalMultiplayer_Activity_Presentation
  -> CameraRigComposer
     -> CameraBehavior_Group
     -> CinemachineTargetGroup
     -> CinemachineGroupFraming
~~~

Players contribute Camera Subjects only:

~~~text
FG_FarmerPresentationGroup
  -> ActorCameraSubjectAuthoring
     Observation Transform
     Framing Radius

FG_CowPresentationGroup
  -> ActorCameraSubjectAuthoring
     Observation Transform
     Framing Radius
~~~

An empty observation target is valid. Group member extent is supplied by the Subject Framing Radius; `CameraBehavior_Group.MemberRadius` is fallback only.

No Player Slot -> Camera Presentation binding is authored because this sample deliberately uses one `AllAvailableSubjects` Group Presentation. CAMERA-032-E `ExplicitSelection` belongs to Player-specific camera samples such as Character Selection, not to this shared-screen topology.

### Consumer proof — 2026-09-22

~~~text
Camera Session Outputs materialized = 1

Route Presentation
  CameraPresentation_Default_Route
  scope = Route
  transition = Cut
  precedence = 200

Activity Presentation
  CameraPresentation_LocalMultiplayer
  scope = Activity
  transition = Blend
  precedence = 300

P1 -> one Group Subject
P2 -> second Group Subject
same Group Presentation frames both

Actor Framing Radius authored
  -> Target Group member radius reflects Actor extent
  -> close framing caused by a dimensionless observation target is corrected
~~~

This supplies current consumer evidence for IF-ADR-032 Route/Activity Presentation participation and one-Output GameApplication Camera Session materialization. It does not certify two Outputs, split-screen or Player `ExplicitSelection`.

### Player proof history

The 2026-09-07 Player lifecycle/device proof remains valid:

~~~text
P1/P2 Join
P1/P2 Leave
P1/P2 Rejoin as fresh occurrences
placement reapplication
current occupancy UI
distinct device ownership
already-owned device blocked before Join
~~~

### Remaining sample-level proof

~~~text
actual gameplay input no-cross-control between P1 and P2
explicit extra-Join behavior while both Slots are occupied
Close Joining behavior while current Players remain preserved
Reopen Joining behavior where applicable
visual Group Camera tuning
~~~

Do not introduce sample-owned Slot/device/input/Camera authority for those remaining cases.


## Application / Scenario rule

```text
materially incompatible initial Player Session intent
  -> separate Demonstration Application

compatible runtime behavior
  -> Scenario inside that application
```

Character Selection is a separate application because its creation-time Actor Resolution intent is intentionally different:

```text
Player Provisioning
  ResolveConfiguredDefault

Character Selection
  LeaveUnresolved
```

Participation, physical lifetime, placement, Input/Pause and Leave/Rejoin do not automatically create new GameApplications.

Samples demonstrate representative canonical usage; exhaustive combinations belong in QAFramework.

## Player/Shared

`Player/Shared` is created by **concrete reuse**, not by taxonomy alone.

The current shared technical prefab baseline is justified because `FG_Player`, `FG_PlayerActor` and `FG_Presentation` are reused across Player demonstrations.

Canonical rule remains:

```text
used by one Player application
  -> keep local

concretely reused by two or more Player applications
  -> promote reusable technical/presentation content to Player/Shared
```

Application/session authority always remains local to the owning Demonstration Application.

## Public-surface gate

Player samples consume public/product Framework APIs.

If a required public Player contract is missing, the demonstration remains blocked at that boundary. Sample code must not hide a product gap with internal discovery, reflection, direct runtime mutation, parallel registries or silent fallbacks.

Character Selection and Local Multiplayer satisfy the Player public-surface gate on their current compositions. Both are migrated to the current Camera Presentation model; remaining Player sample work is concrete demonstration behavior, while full Camera certification remains Framework/QA work.