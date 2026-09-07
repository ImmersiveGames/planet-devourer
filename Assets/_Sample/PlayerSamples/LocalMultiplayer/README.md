# Local Multiplayer

Status: **MATERIALIZED / FIRST LIFECYCLE SLICE PLAY MODE PROVEN — 2026-09-07**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`, Revision 6.

Local Multiplayer is an active **Player Demonstration Application** under:

```text
Assets/_Sample/PlayerSamples/LocalMultiplayer/
```

The historical August blocker for public Slot/device/input ownership and observation has been re-audited against the current Framework and is **closed for the implemented sample path**. The sample does not own a parallel Slot registry, device ownership authority, input router or Session authority.

## Current purpose

The sample demonstrates a canonical local two-Player Manager-Provisioned flow:

```text
Open Joining
  -> request Join from an InputDevice
  -> Framework allocates the next available configured Slot
  -> Local Player Host is created/admitted
  -> configured Default Actor is selected/prepared
  -> Player Actor / Presentation is materialized
  -> Activity placement is applied
  -> GameplayReady
```

The current configured Slots are:

```text
player.1
player.2
```

The sample tutorial/UI observes the current Session state rather than counting historical Join operations.

## Public consumer boundary

Commands remain explicit Framework product commands:

```text
PlayerSessionOpenJoiningCommandTrigger
PlayerSessionCloseJoiningCommandTrigger
PlayerSessionJoinCommandTrigger
PlayerSessionLeaveCommandTrigger
```

The tutorial observes current Slot occupancy through the scoped public Session surface:

```text
IPlayerSessionScopedAccess
  -> Changed
  -> TryGetObservation(...)
  -> PlayerSessionScopedObservationSnapshot
  -> Slots
  -> PlayerSessionScopedSlotObservation.IsJoined
```

This means presentation is derived from the authoritative current Session state:

```text
P1 Available + P2 Available
  -> Waiting for Player 1

P1 Joined + P2 Available
  -> Player 1 joined; waiting for Player 2

P1 Available + P2 Joined
  -> Player 2 joined; waiting for Player 1

P1 Joined + P2 Joined
  -> Completed / both Players joined
```

`LocalMultiplayerJoinTutorialController` therefore does **not** use a historical successful-Join counter as Player occupancy authority.

## Materialized application

Current application-owned assets include:

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

The keyboard/gamepad simulator creates test InputDevices for this sample; it does not become Slot or input-ownership authority.

## Play Mode proof — 2026-09-07

The first Local Multiplayer lifecycle slice is proven in consumer Play Mode.

Observed flow:

```text
Open Joining
-> Join P1
-> P1 prepared / materialized / GameplayReady
-> UI = Player 1 joined; waiting for Player 2

Leave P1
-> exact P1 occurrence released
-> player.1 returns to Available
-> Actor / Presentation / technical host evidence released
-> UI = Waiting for Player 1

Rejoin P1
-> same logical Slot player.1
-> new Session Player occurrence
-> new host / assignment / Actor materialization
-> Activity placement reapplied correctly
-> UI remains waiting for Player 2

Join P2
-> player.2 joins
-> configured Actor prepared/materialized
-> both Players GameplayReady
-> Activity readiness completes
-> UI = both Players joined

Leave P1 while P2 remains Joined
-> P1 resources released
-> P2 remains active
-> UI = Player 2 joined; waiting for Player 1

Rejoin P1
-> fresh P1 occurrence
-> placement reapplied
-> Activity returns to completed readiness
-> UI returns to both Players joined
```

Manual visual validation also confirmed that the status UI tracks the actual Slot occupancy and that the rejoined Player is positioned at the correct authored placement.

## Framework / QA evidence supporting this slice

The corresponding current Framework Player lifecycle is technically certified by the Full Player QA aggregate:

```text
PLAYER QA CERTIFIED
17/17
```

This includes the corrected Leave/Rejoin reconciliation and Activity relocation lifecycle used by the sample.

## Still to prove before Local Multiplayer closure

The Demonstration Application is **not closed yet**.

Next consumer proofs should cover at least:

```text
Leave / Rejoin P2 while P1 remains active
independent input/device ownership for P1 and P2
no cross-control between Players
behavior when both configured Slots are already occupied
Close Joining behavior and preservation of already Joined Players
reopening Joining after Close when applicable
```

Additional presentation/camera topology such as split-screen remains outside the current proven slice and should be introduced only if the intended sample scope requires it.

## Non-goals

Do not add:

```text
sample-owned Slot registry
sample-owned device ownership
sample-owned input routing authority
hidden PlayerInput / InputUser discovery
reflection
private/internal Player runtime access
a second Session authority
silent fallback
```

The Framework remains authoritative for Player Session, Slot allocation, Join/Leave lifecycle, Actor preparation, placement, gameplay admission and input ownership. The sample owns only demonstration interaction/presentation and test-device simulation.