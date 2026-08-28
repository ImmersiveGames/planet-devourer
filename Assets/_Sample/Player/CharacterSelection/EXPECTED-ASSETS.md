# Expected Unity Assets

Status: **MATERIALIZED / PLAY MODE PROVEN — 2026-08-28**

This file now records the materialized Character Selection boundary rather than a future asset plan. The earlier arbitrary Actor-selection blocker is closed and the application has been proven using public Player surfaces only.

## Materialized application intent

```text
GameApplication_CharacterSelection.asset

PlayerSessionProfile_CharacterSelection.asset
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
```

Character Selection intentionally differs from the default-resolving Player Provisioning application at Session creation time, so it remains a separate Demonstration Application.

## Materialized composition

The current authoring composition includes:

```text
GameApplication_CharacterSelection.asset
PlayerSessionProfile_CharacterSelection.asset

Routes/
  Route_Character Selection.asset

Activities/
  Character Selection Activity

Scenes/
  CharacterSelection_UI.unity

Player/
  Character Selection Player/session assets

ActorProfile choices
  Farmer
  Cow

Scripts/
  CharacterSelectionActorButtonPresenter.cs
```

The Character Selection Route currently reuses the existing Manager-Provisioned primary scene and adds `CharacterSelection_UI.unity` as Route Content.

That reuse belongs to the current Player authoring group. Final UPM promotion may reorganize genuinely reusable presentation/content when useful, but it must not move application/session authority into a shared layer merely for deduplication.

## UI composition

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

The selection controls contain at least two explicit Actor choices.

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
  -> Manager-Provisioned materialization
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

Play Mode consumer validation confirmed Farmer and Cow selection through the same explicit lifecycle, including Leave/Rejoin.

Framework certification also reports:

```text
historicalFullPlayer = 25/25
leaveUnresolved = PASS
mandatoryContracts = 30
executedContracts = 30
passedContracts = 30
```

The remaining release gate is final Player UPM promotion/import validation, not Character Selection materialization.
