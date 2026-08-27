# Expected Unity Assets

Status: **READY FOR MATERIALIZATION / NOT YET PLAY MODE PROVEN — 2026-08-26**

The previous public arbitrary Actor-selection blocker is closed. Character Selection may now be materialized using only official public Player surfaces.

Expected application-local assets may include:

```text
GameApplication_CharacterSelection.asset
PlayerSessionProfile_CharacterSelection.asset
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved

supporting Player Slot / participation profiles
at least two application-owned ActorProfile choices when suitable assets exist
Local Player Host prefab for Manager-Provisioned acquisition
supporting Route / Activity assets
supporting Scene assets
game-owned Character Selection UI
public Player Session Join / Select Actor command components
optional PlayerSessionObserver for read-only UI evidence
optional application-local HUB only when multiple compatible Scenarios justify it
```

Expected runtime teaching path:

```text
Join
  -> Joined Slot
  -> unresolved Actor

Character choice
  -> PlayerSessionSelectActorCommandTrigger
  -> PlayerActorSelectionResult

Framework lifecycle
  -> selection commit
  -> Actor preparation
  -> Manager-Provisioned materialization
  -> Activity admission / GameplayReady
```

Do not materialize sample-owned internal Actor-selection infrastructure.

Do not add:

```text
private/internal Player runtime access
reflection-based Player discovery
direct Session mutation
parallel Actor registry/selection authority
hidden Default Actor fallback
sample-owned Actor preparation/materialization
physical hot-swap behavior
Local Multiplayer Slot/device/input architecture
```

`PlayerSessionReplaceActorSelectionCommandTrigger` and `PlayerSessionClearActorSelectionCommandTrigger` exist as public lifecycle commands but are not required assets for the initial Character Selection demonstration.

This file describes the expected materialization boundary only. Asset existence and Play Mode behavior must be recorded after implementation and validation; they are not claimed by this status update.
