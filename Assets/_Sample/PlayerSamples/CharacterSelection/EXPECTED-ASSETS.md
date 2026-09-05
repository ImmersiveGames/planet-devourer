# Expected Unity Assets

Status: **MATERIALIZED / PLAY MODE REPROVEN — 2026-09-05**

This file records the materialized Character Selection boundary on the **current Player Actor / Presentation architecture**.

The original Character Selection application was proven on 2026-08-28. After the Player prefab architecture was rebuilt, the sample assets were migrated and consumer Play Mode was reproven on 2026-09-05.

## Materialized application intent

```text
GameApplication_CharacterSelection.asset

PlayerSessionProfile_CharacterSelection.asset
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
```

Character Selection intentionally differs from the default-resolving Player Provisioning application at Session creation time, so it remains a separate Demonstration Application.

## Current materialized composition

```text
CharacterSelection/
  GameApplication_CharacterSelection.asset

  Player/
    PlayerSessionProfile_CharacterSelection.asset
    ActorProfile_Farmer.asset
    ActorProfile_Cow.asset

  Routes/
    Route_Character Selection.asset

  Activities/
    Character Selection Activity

  Scenes/
    CharacterSelection_UI.unity
```

The current shared Player technical prefab baseline is:

```text
Assets/_Sample/PlayerSamples/Shared/Prefabs/
  FG_Player.prefab
  FG_PlayerActor.prefab
  FG_Presentation.prefab
```

The concrete Character Selection presentations are:

```text
Assets/_Sample/PlayerSamples/Player/Players/
  FG_FarmerPresentation.prefab
  FG_CowPresentation.prefab
```

Both concrete presentations derive from the shared `FG_Presentation` prefab baseline.

## ActorProfile -> Presentation contract

The Character Selection Actor profiles now use the current public Actor presentation field:

```text
ActorProfile_Farmer
  -> presentationPrefab = FG_FarmerPresentation

ActorProfile_Cow
  -> presentationPrefab = FG_CowPresentation
```

The old `LogicalActorHostPrefab` composition is not part of the current sample.

Runtime teaching path:

```text
selected ActorProfile
  -> Actor preparation
  -> Player Actor Runtime Host
  -> Presentation Mount
  -> ActorProfile.PresentationPrefab
  -> selected concrete Presentation
```

The sample authors the concrete presentation; Framework runtime remains responsible for Actor preparation and physical materialization.

## Presentation behavior used by the proof

The concrete Farmer/Cow presentations provide the sample gameplay-facing presentation needed by Character Selection, including:

```text
Player gameplay input consumption
character-specific visible presentation
Follow camera authoring / rig composition
minimal movement used by the sample proof
```

The purpose of these components is to prove that selecting a different `ActorProfile` results in the correct usable Player presentation rather than only changing logical Session state.

## Route / UI composition

The Character Selection Route reuses the compatible Manager-Provisioned gameplay scene and adds `CharacterSelection_UI.unity` as Route Content.

`CharacterSelection_UI.unity` contains a Route-scoped `PlayerSessionObserver` outside the selection panel.

Its presentation wiring is:

```text
On Player Joined
  -> show Character Selection Controls

On Actor Selected
  -> hide Character Selection Controls

On Player Left
  -> hide Character Selection Controls
```

The selection controls contain the Farmer and Cow choices.

Each choice uses:

```text
PlayerSessionSelectActorCommandTrigger
  -> exact PlayerSlotProfile
  -> exact ActorProfile

CharacterSelectionActorButtonPresenter
  -> reads the command's ActorProfile
  -> ActorProfile.DisplayName -> UI label
  -> ActorProfile.Icon        -> UI image
```

The presenter is sample-owned presentation only. It does not own selection authority or Session state.

## Runtime teaching path

```text
Open Joining
  -> Join
  -> Joined Slot
  -> unresolved Actor
  -> WaitingForActorSelection

Character choice
  -> PlayerSessionSelectActorCommandTrigger
  -> PlayerActorSelectionResult

Framework lifecycle
  -> selection commit
  -> Actor preparation
  -> Player Actor Runtime Host
  -> selected PresentationPrefab materialization
  -> Activity admission / GameplayReady

Leave / Rejoin
  -> WaitingForJoin
  -> Joined + unresolved Actor
  -> WaitingForActorSelection
  -> another explicit Actor choice
```

## Explicit non-goals

Do not add:

```text
private/internal Player runtime access
reflection-based Player discovery
direct Session mutation
parallel Actor registry/selection authority
hidden Default Actor fallback
sample-owned Actor preparation/materialization
physical Actor hot-swap behavior
Local Multiplayer Slot/device/input architecture
```

`PlayerSessionReplaceActorSelectionCommandTrigger` and `PlayerSessionClearActorSelectionCommandTrigger` remain valid public lifecycle commands but are not part of this initial Character Selection demonstration.

## Proof status

### Historical lifecycle proof — 2026-08-28

Play Mode consumer validation confirmed Farmer and Cow selection through the explicit `LeaveUnresolved` lifecycle, including Leave/Rejoin.

Framework certification also reported:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

### Current physical-composition proof — 2026-09-05

After migration to `ActorProfile.PresentationPrefab` and the current shared Player prefab baseline, consumer Play Mode was rerun successfully.

The closure verifies:

```text
Join
-> WaitingForActorSelection
-> select Farmer / Cow
-> correct PresentationPrefab materialized
-> Follow camera functional
-> gameplay movement/input functional
-> GameplayReady
-> Leave / Rejoin
-> fresh selection remains functional
```

The remaining release gate is final Player UPM promotion/import validation, not Character Selection materialization.
