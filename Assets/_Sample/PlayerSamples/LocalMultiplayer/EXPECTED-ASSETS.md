# Local Multiplayer Assets

Status: **MATERIALIZED / BIDIRECTIONAL LEAVE-REJOIN + DEVICE OWNERSHIP PLAY MODE PROVEN — 2026-09-07**

The historical public Slot/device/input ownership blocker is closed for the current Local Multiplayer implementation path. This file records the materialized application rather than a blocked future asset list.

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
PlayerSessionScopedSlotObservation.InputOwnership
```

Session changes invalidate the tutorial cache. Canonical observation is read later through a deferred/coalesced refresh instead of synchronously inside `PlayerSessionChange` publication.

The keyboard/gamepad simulator exists only to provide deterministic test InputDevices in the sample. It does not own Slot assignment, Player input routing or Session state.

## Proven lifecycle and ownership slice

```text
P1 Join
P1 Leave
P1 Rejoin with placement reapplied
P2 Join
both Players active / Activity ready
P1 Leave while P2 remains active
P1 Rejoin / Activity completes again
P2 Leave while P1 remains active
P2 Rejoin
repeated P1/P2 Leave/Rejoin cycles
UI derived from current Slot occupancy
P1 and P2 retain distinct current device ownership
already-owned device blocked by tutorial before Join
```

The validated happy path produced no Framework duplicate-device rejection and no transient `RegisteredHost.NotRegistered` ownership diagnostic.

## Remaining proof before closure

```text
actual gameplay no-cross-control between P1 and P2
explicit extra-Join behavior while both configured Slots are occupied
Close Joining behavior
Reopen Joining behavior when applicable
```

Do not introduce sample-owned Slot, device or input authority to complete those remaining proofs.