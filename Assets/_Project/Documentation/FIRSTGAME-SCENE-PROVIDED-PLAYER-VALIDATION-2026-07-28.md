# FIRSTGAME — Scene-Provided Player Validation

Status: Baseline recorded; PLAYER-DIAG-1 manual regression pending
Date: 2026-07-28

## Scope

Focused Route: `FG Player Scene Provider`  
Activity: `Activity_PlayerLocalProvider`  
Scene: `SceneProvidedGameplay`  
Prefab: `Player_SceneProvided_With_Camera`

The recorded repository baseline is listed in `FIRSTGAME-CURRENT-STATE.md`.
This document does not claim a new Git inspection or Unity execution.

## Authoring observed

- `LocalPlayerHostAuthoring`
- `SceneLocalPlayerAdmissionAuthoring`
- `PlayerActorDeclaration`
- `PlayerGameplayCameraAuthoring`
- `PausePlayerInputBinding`
- Reset Subject Adapter and Transform Reset Participant

## Recorded comparison baseline

| Check | Recorded state |
|---|---|
| Boot, Menu → Gameplay and Scene-Provided admission | Approved |
| Player Slot `player.1`, Host joined and Logical Actor | Approved |
| Activity Ready, movement, camera and Pause | Approved |
| Object Reset, Group Reset and Activity Restart operation | Approved |
| Gameplay → Menu and same-session reentry | Approved |
| Stop after simple flow | Approved |
| Stop after Activity Restart | PLAYER-DIAG-1 regression pending |
| Persistent release diagnostics | PLAYER-DIAG-1 manual verification pending |
| Runtime-Provisioned Player / second Player / multiplayer | Not tested / out of scope |

## Required PLAYER-DIAG-1 manual matrix

1. Menu → Gameplay → Menu → Stop.
2. Menu → Gameplay → Menu → Gameplay → Menu → Stop.
3. Menu → Gameplay → Activity Restart → Menu → Stop.
4. Menu → Gameplay → Activity Restart → Activity Restart → Menu → Stop.

For each, record active admission during Gameplay, Slot `player.1`, Actor, Host,
Composer state, and the persistent `FrameworkRuntimeHost` Advanced/Debug release
snapshot after returning to Menu. The expected post-release state is zero active
Scene-Provided admissions, a reusable Slot, no retained Host evidence and no
exception.

## Open finding outside this cut

Reset Subjects may deregister in `SceneReleasing`, register again through
`update-retry`, then deregister during `on-disable`. This is a non-blocking
technical finding indicating transient unload recomposition. It requires its own
Reset cut and is not normalized by PLAYER-DIAG-1.

## Limits

This Scene-Provided path is a comparison baseline. It does not replace the future
runtime-provisioned Player path, a second Player, or multiplayer validation.
