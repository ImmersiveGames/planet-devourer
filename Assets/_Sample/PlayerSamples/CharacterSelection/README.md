# Character Selection

Status: **CLOSED / PLAY MODE REPROVEN — 2026-09-05**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`, Revision 5.

Character Selection is a materialized Player Demonstration Application. It proves the public `LeaveUnresolved` flow from Join through explicit Actor choice to Framework-owned preparation, current Player Actor materialization and `GameplayReady`.

The original consumer proof was completed on **2026-08-28**. The sample was subsequently rebuilt for the current Player composition introduced after the Player Actor / Presentation architecture changes and was **reproven in Play Mode on 2026-09-05**.

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
  -> Manager-Provisioned Player Actor Runtime Host
  -> ActorProfile.PresentationPrefab materialization
  -> Activity participation / GameplayReady
        ↓
PlayerSessionObserver.OnActorSelected
  -> hide Character Selection Controls
```

Leave/Rejoin returns the Session Slot to `WaitingForActorSelection` and allows a fresh explicit choice.

## Current Player composition — 2026-09-05

The current sample no longer relies on the pre-ADR-023 logical-host composition.

The shared Player prefab baseline used by the Player samples is materialized under:

```text
Assets/_Sample/PlayerSamples/Shared/Prefabs/
  FG_Player.prefab
  FG_PlayerActor.prefab
  FG_Presentation.prefab
```

Character Selection owns the Actor choices while reusing the current shared technical composition.

```text
ActorProfile_Farmer
  -> PresentationPrefab
     -> FG_FarmerPresentation
        -> FG_Presentation baseline

ActorProfile_Cow
  -> PresentationPrefab
     -> FG_CowPresentation
        -> FG_Presentation baseline
```

The concrete presentation prefabs are currently authored under:

```text
Assets/_Sample/PlayerSamples/Player/Players/
  FG_FarmerPresentation.prefab
  FG_CowPresentation.prefab
```

This keeps the Player Actor Runtime Host / Presentation boundary explicit: the Framework materializes the selected Actor's configured `PresentationPrefab`; the sample does not create a parallel logical Actor host.

The Character Selection presentations also provide the sample gameplay/presentation behavior used in the proof, including Player gameplay input consumption, Follow camera composition and the character-specific visible presentation.

## Current Route / UI composition

The Character Selection Route reuses the compatible Manager-Provisioned gameplay scene as its base composition and adds `CharacterSelection_UI.unity` as Route Content.

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

ActorProfile.PresentationPrefab
  selected Actor presentation contract used by the current Player composition
```

The broader public Actor-selection family also contains Default, Replace and Clear, but this sample intentionally demonstrates initial explicit Select only.

## Ownership boundary

The sample/game owns:

```text
which ActorProfile choices are presented
button layout and visual presentation
ActorProfile DisplayName/Icon projection
concrete Farmer/Cow Presentation authoring
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
Player Actor Runtime Host lifecycle
PresentationPrefab materialization
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

### Historical proof — 2026-08-28

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

### Current-composition reproof — 2026-09-05

After rebuilding the Player prefab chain and migrating Actor profiles to `PresentationPrefab`, consumer Play Mode was rerun successfully with the current composition.

The closure verifies the intended user-visible path:

```text
Join
  -> WaitingForActorSelection
  -> select Farmer or Cow
  -> selected Actor prepared
  -> correct PresentationPrefab materialized
  -> Follow camera active for the selected presentation
  -> gameplay movement/input functional
  -> GameplayReady
  -> Leave
  -> Rejoin
  -> fresh explicit Actor selection
```

This 2026-09-05 proof supersedes the old physical-composition evidence for sample authoring while retaining the 2026-08-28 lifecycle proof as historical evidence.

## Release status

Character Selection is now **closed for authoring and consumer Play Mode proof** under `Assets/_Sample/` on the current Player architecture.

The next Player construction target is **Local Multiplayer**, beginning with a re-audit of the current public Slot/device/input contract.

This does not claim final UPM release completion. Promotion to `com.immersive.framework/Samples~/` and Package Manager import proof remain part of the later Player sample-group finalization gate.
