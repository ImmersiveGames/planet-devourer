# Character Selection

Status: **AUTHORING COMPLETE / PLAY MODE PROVEN — 2026-08-28**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`, Revision 4.

Character Selection is a materialized Player Demonstration Application. It proves the public `LeaveUnresolved` flow from Join through explicit Actor choice to Framework-owned preparation, materialization and `GameplayReady`.

## Canonical application intent

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

The runtime flow is:

```text
Open Joining
  -> Join
  -> Slot Joined
  -> Actor unresolved
  -> Preparing / WaitingForActorSelection
        ↓
PlayerSessionObserver.OnPlayerJoined
  -> show Character Selection Controls
        ↓
game-owned Actor choice UI
  -> Farmer / Cow ActorProfile choices
        ↓
PlayerSessionSelectActorCommandTrigger.Invoke()
        ↓
Framework commits Actor selection
        ↓
Actor preparation
  -> Manager-Provisioned physical materialization
  -> Activity participation / GameplayReady
        ↓
PlayerSessionObserver.OnActorSelected
  -> hide Character Selection Controls
```

Leave/Rejoin returns the same Session Slot to `WaitingForActorSelection` and allows a fresh explicit choice.

## Current composition

The Character Selection Route reuses the existing Manager-Provisioned primary scene as its base composition and adds `CharacterSelection_UI.unity` as Route Content.

```text
Route_Character Selection
├── Primary Scene: ManagerProvisioned.unity
│   ├── Open / Close Joining controls
│   ├── Join / Leave controls
│   └── Manager-Provisioned gameplay environment
│
└── Route Content: CharacterSelection_UI.unity
    ├── Player Session Observer
    └── Character Selection Controls
        ├── Farmer button
        └── Cow button
```

The Route-content selection commands use `Scope = Route`. The inherited joining controls in the Primary Scene remain Activity-scoped according to their existing composition.

## Player Session observation

`PlayerSessionObserver` is read-only and remains outside the panel that it controls.

The sample uses its designer-facing UnityEvents only for presentation:

```text
On Player Joined
  -> Character Selection Controls.SetActive(true)

On Actor Selected
  -> Character Selection Controls.SetActive(false)

On Player Left
  -> Character Selection Controls.SetActive(false)
```

`On Actor Cleared` is intentionally not used to show the panel because Actor clear occurs during Leave before the terminal Player Left event.

## ActorProfile-driven buttons

Each selection button keeps `PlayerSessionSelectActorCommandTrigger` as the authority for which `ActorProfile` will be selected.

The sample-owned `CharacterSelectionActorButtonPresenter` reads that same command's `ActorProfile` and projects presentation only:

```text
PlayerSessionSelectActorCommandTrigger.ActorProfile
  ├── DisplayName -> button label
  └── Icon        -> button image
```

The presenter does not select Actors, mutate Session state, register a second ActorProfile reference, discover Players or configure the Button click.

Button clicks remain explicit Inspector wiring to the corresponding command trigger.

## Public surfaces demonstrated

```text
PlayerSessionObserver
  read-only Session lifecycle presentation

PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionLeaveCommandTrigger
  inherited Manager-Provisioned joining/lifetime controls

PlayerSessionSelectActorCommandTrigger
  explicit initial Actor choice
```

The broader public Actor-selection family also contains Default, Replace and Clear, but this sample intentionally demonstrates initial explicit Select only.

## Ownership boundary

The sample/game owns:

```text
which ActorProfile choices are presented
button layout and visual presentation
ActorProfile DisplayName/Icon projection
UI visibility wiring
user interaction
```

The Framework owns:

```text
Joined Slot validity
Session Actor-resolution policy
selection revision and commit
duplicate-selection policy
Actor preparation barrier
physical Actor materialization
Activity admission/readiness
```

Do not bridge the flow with private/internal runtime access, reflection, direct Session mutation, sample-specific Player discovery, parallel Actor-selection authority or hidden fallback.

## LeaveUnresolved contract

`LeaveUnresolved` means an unselected Joined Slot is a legitimate pending state:

```text
Joined
  -> no selected Actor
  -> Preparing
  -> WaitingForActorSelection
```

The Framework does not invoke Default Actor resolution in this state. Explicit selection is what advances the lifecycle.

## Validation evidence

Consumer Play Mode validation proved:

```text
Open Joining -> Succeeded
Join -> SucceededJoined
Joined + unresolved Actor -> WaitingForActorSelection
gate remains held while selection is pending
Farmer Select -> SucceededSelected -> Prepared -> Materialized -> GameplayReady
Leave -> WaitingForJoin
Rejoin -> WaitingForActorSelection
Cow Select -> SucceededSelected -> Prepared -> Materialized -> GameplayReady
```

The corresponding Framework Full Player aggregate also passed:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
sessionChangeObservation = PASS
designerEventProjection = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

## Release status

This closes Character Selection **authoring / consumer Play Mode proof** under `Assets/_Sample/`.

It does not claim final UPM release completion. Promotion to `com.immersive.framework/Samples~/` and Package Manager import proof remain part of the later Player sample-group finalization gate.
