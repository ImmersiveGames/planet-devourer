# Character Selection Multiplayer Split-Screen

Status: **CAMERA-032-D/E UNITY CONSUMER PASS — explicit per-Player Actor selection + two-Output split-screen + Leave/Rejoin proven 2026-09-23**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

Canonical Camera authority: `IF-ADR-032 — Camera Unified Authority, Session Outputs, Presentations, Subjects and Lifecycle`.

This sample combines the explicit initial Actor-selection flow from `CharacterSelection` with the two-Player / two-Output split-screen topology.

## Purpose

The sample proves this distinction independently for P1 and P2:

~~~text
Joined Player
  !=
selected Actor
  !=
camera-ready Player Presentation
~~~

Session policy:

~~~text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
Configured Slots = 2
~~~

Joining a Player creates the technical Host and reserves the configured Slot. It does **not** select Farmer/Cow automatically.

## Runtime flow

~~~text
Boot
  -> no Players
  -> no selected Actors
  -> two Session Camera Outputs exist
  -> Player-bound Outputs are in zero-Player continuity mode

Open Joining
  -> Join P1
  -> P1 technical Host exists
  -> P1 Actor unresolved
  -> P1 Character Selection menu appears

P1 selects Farmer or Cow
  -> PlayerSessionSelectActorCommandTrigger
  -> exact P1 Actor selection committed
  -> Actor prepared/materialized
  -> GameplayReady
  -> P1 ThirdPerson Presentation becomes eligible
  -> Output P1 renders P1

Join P2
  -> P2 technical Host exists
  -> P2 Actor unresolved
  -> P2 Character Selection menu appears
  -> split-screen is active from exact Player -> Output association

P2 selects Farmer or Cow
  -> exact P2 Actor selection committed
  -> P2 Actor prepared/materialized
  -> P2 ThirdPerson Presentation becomes eligible
  -> Output P2 renders P2

Leave
  -> exact Player occurrence released
  -> old Actor selection cleared
  -> old Actor / Camera Subject occurrence released
  -> surviving Player returns to full-screen when only one remains

Rejoin
  -> fresh technical Host
  -> Actor unresolved again
  -> fresh explicit Farmer/Cow choice required
  -> split-screen restored when both Player Outputs participate

final Leave
  -> zero Players
  -> no retained Actor selection / materialization
~~~

## Camera topology

The GameApplication owns explicit Session Camera capacity.

~~~text
GameApplication_CharacterSelectionMultiplayerSplitScreen

Camera Session
  Output Prefabs
    Output P1
    Output P2

  Player Output Bindings
    PlayerSlotProfile_CharacterSelectionMultiplayer_P1
      -> Output P1

    PlayerSlotProfile_CharacterSelectionMultiplayer_P2
      -> Output P2

  Player Presentation Bindings
    P1
      -> CameraPresentation_CharacterSelection_P1_ThirdPerson

    P2
      -> CameraPresentation_CharacterSelection_P2_ThirdPerson
~~~

The two Outputs are explicit Session capacity. Player count does not create physical Cameras.

`PlayerInputManager` remains the owner of Unity automatic split-screen layout and `Camera.rect` recomposition.

## Route and Activity Presentations

The Route owns one neutral Presentation per physical Output:

~~~text
Route_CharacterSelectionMultiplayerSplitScreen

CameraPresentation_CharacterSelection_P1_Route
  -> Output P1

CameraPresentation_CharacterSelection_P2_Route
  -> Output P2
~~~

The Activity owns one Player-specific ThirdPerson Presentation per Output:

~~~text
Activity_CharacterSelectionMultiplayerSplitScreen

CameraPresentation_CharacterSelection_P1_ThirdPerson
  Subject Policy = ExplicitSelection
  -> Output P1

CameraPresentation_CharacterSelection_P2_ThirdPerson
  Subject Policy = ExplicitSelection
  -> Output P2
~~~

Before a Slot has selected an Actor, its ThirdPerson Presentation has no eligible selected Camera Subject. The Route Presentation therefore remains effective for that Output.

P1 and P2 selection are independent; one Player becoming camera-ready does not satisfy the other Player's Presentation.

## Character Selection UI

Character Selection is deliberately separate from the Join tutorial UI.

Relevant structure:

~~~text
LocalMultiplayerMenu
├── JoinInput
├── CharacterSelectionSessionObserver
└── Canvas_MenuHub
    ├── Commands
    ├── Menu_SelectionActor
    │   ├── Player1_CharacterSelection
    │   │   ├── Farmer
    │   │   └── Cow
    │   └── Player2_CharacterSelection
    │       ├── Farmer
    │       └── Cow
    └── Menu_JoinNavegation
~~~

Responsibilities:

~~~text
LocalMultiplayerJoinTutorialController
  -> Joining UI only
  -> Open / Join / Leave / current occupancy

CharacterSelectionMultiplayerSelectionController
  -> Character Selection visibility only
  -> observes authoritative PlayerSession snapshot
  -> never commits Actor selection itself

PlayerSessionSelectActorCommandTrigger
  -> actual Framework Actor-selection command
~~~

The selection controller treats `PlayerSessionObserver.Changed` only as invalidation. It refreshes the canonical Session observation later from `Update`, after the mutation that raised the event has finished publishing Host/Actor evidence.

This avoids querying input ownership in the middle of a Manager-provisioned Join transaction.

## Actor choices

The sample reuses the Character Selection Actor choices:

~~~text
ActorProfile_Farmer
ActorProfile_Cow
~~~

Each selection is explicitly targeted to the corresponding configured Player Slot.

~~~text
P1 Farmer button -> P1 + Farmer
P1 Cow button    -> P1 + Cow

P2 Farmer button -> P2 + Farmer
P2 Cow button    -> P2 + Cow
~~~

Both Slot profiles intentionally have:

~~~text
Default Actor = None
~~~

and the Session profile intentionally has:

~~~text
Actor Resolution = LeaveUnresolved
~~~

## Ownership boundary

The sample/game owns:

~~~text
Join/selection UI presentation
which Actor choices are offered
per-slot button wiring
Route/Activity Camera Presentation declarations
GameApplication Player -> Output bindings
GameApplication Player -> Presentation bindings
concrete Actor presentation authoring
~~~

The Framework owns:

~~~text
Player Session / Slot lifecycle
technical Host provisioning
Actor-selection commit
Actor preparation/materialization
Actor occurrence lifetime
Camera Subject publication lifetime
Player -> Camera Subject selection
Camera Presentation runtime
CameraRequest arbitration
physical Camera Output lifecycle
PlayerInput.camera association
~~~

Unity `PlayerInputManager` owns automatic physical split-screen layout.

Do not add sample-owned Session mutation, Player discovery, Camera Subject registry, Camera Output arbitration, direct `Camera.rect` writes or a second Actor-selection authority.

## Unity consumer proof — 2026-09-23

Manual Play Mode confirmed:

~~~text
Boot
  actorResolutionPolicy = LeaveUnresolved
  Camera Session Outputs materialized = 2

Join P1
  -> SucceededJoined
  -> Actor remains unprepared
  -> ownership diagnostic Succeeded

P1 selects Cow
  -> SucceededSelected
  -> GameplayReady

Join P2
  -> SucceededJoined
  -> P2 remains unresolved
  -> readiness = WaitingForActorSelection

P2 selects Farmer
  -> SucceededSelected
  -> joined = 2
  -> selected = 2
  -> readiness = Completed

Leave P1
  -> SucceededLeft
  -> activityReleased = True
  -> provisioningReleased = True
  -> partialRelease = False
  -> Actor selection cleared

Rejoin P1
  -> SucceededJoined
  -> fresh Host
  -> Actor unresolved again

P1 selects Farmer
  -> fresh explicit selection
  -> Actor prepared/materialized again

final Leave P1 + Leave P2
  -> both SucceededLeft
  -> hostCount = 0
  -> joined = 0
  -> selected = 0
~~~

The earlier transient `FRAMEWORK_PLAYER_INPUT_OWNERSHIP_DIAG / RegisteredHost.NotRegistered` warning was removed by deferring Character Selection observation out of the synchronous Session-change callback.

## Application shutdown

The gameplay lifecycle above completes successfully before Play Mode exit.

A Framework-side application-quit Camera teardown warning was observed when normal request release attempted to restore a Default CinemachineCamera after Unity had already invalidated the physical Camera scene object.

That issue belonged to terminal Framework shutdown ordering, not to this sample's authoring. The Framework now has an application-quit-only terminal Presentation cleanup path that avoids normal winner/Default re-application after Unity physical Camera scene validity is gone.

A Unity rerun on 2026-09-23 validated the correction: both Activity ThirdPerson Presentations and both Route Presentations released cleanly, both Session Outputs reached `TornDown`, and the previous invalid Default CinemachineCamera warning did not recur.
