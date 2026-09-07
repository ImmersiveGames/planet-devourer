# Local Multiplayer Assets

Status: **MATERIALIZED / FIRST LIFECYCLE SLICE PLAY MODE PROVEN — 2026-09-07**

The historical public Slot/device/input ownership blocker is closed for the current Local Multiplayer implementation path. This file now records the materialized application rather than a blocked future asset list.

## Current application-owned assets

```text
GameApplication_LocalMultiplayer.asset

Player/
  PlayerSessionProfile_LocalMultiplayer.asset
  PlayerSlotProfile_LocalMultiplayer_P1.asset
  PlayerSlotProfile_LocalMultiplayer_P2.asset

Routes/
  Route_LocalMultiplayer.asset
  MultiplayerRouteContentProfile.asset

Activities/
  Activity_LocalMultiplayer.asset

Scenes/
  LocalMultiplayer_Persistent.unity
  LocalMultiplayer.unity
  LocalMUltiplayerUI.unity

Scripts/
  LocalMultiplayerJoinInputSource.cs
  LocalMultiplayerJoinTutorialController.cs
  LocalMultiplayerKeyboardGamepadSimulator.cs
```

The application reuses the current Player technical/presentation baseline where configured. Application/session authority remains local to Local Multiplayer.

## Current public composition

The sample uses public Framework Player Session commands and scoped observation rather than parallel sample-owned authority:

```text
Open Joining command
Join command from InputDevice
Leave command per configured Slot
IPlayerSessionScopedAccess.Changed
IPlayerSessionScopedAccess.TryGetObservation(...)
PlayerSessionScopedSlotObservation.IsJoined
```

The keyboard/gamepad simulator exists only to provide deterministic test InputDevices in the sample. It does not own Slot assignment, Player input routing or Session state.

## Proven lifecycle slice

```text
P1 Join
P1 Leave
P1 Rejoin with placement reapplied
P2 Join
both Players active / Activity ready
P1 Leave while P2 remains active
P1 Rejoin / Activity completes again
UI derived from current Slot occupancy
```

## Remaining proof before closure

```text
P2 Leave/Rejoin while P1 remains active
P1/P2 independent gameplay input ownership
both-Slots-occupied behavior
Close/Reopen Joining behavior
```

Do not introduce sample-owned Slot, device or input authority to complete those remaining proofs.