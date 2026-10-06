# Character Selection

Status: **sample-specific Fixed Follow composition authored; Unity validation pending**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

Character Selection keeps the existing `ManagerProvisioned` host provisioning and `LeaveUnresolved` actor selection flow. Its application loads a CharacterSelection-specific persistent scene whose provisioning prefab variant points to the CharacterSelection player variant; `ManagerProvisioned` assets remain unchanged.

## Player and Actor composition

```text
GameApplication_CharacterSelection
  Persistent Content -> CharacterSelection_Persistent
    Local_Player_Provisioning instance override
      FG_Player_CharacterSelection
        FG_PlayerActor_CharacterSelection
          PlayerGameplayInputReader
          CharacterController
          CharacterSelectionActorMovement
          ActorCameraSubjectAuthoring -> FollowObservation
          (no MinimalThirdPersonLook)
```

Farmer and Cow are selectable visual-only presentations. They do not add input, movement, look, CharacterController, or Camera Subject authority. The selected Actor remains the single owner of gameplay input and movement.

`FollowObservation` is a neutral Actor anchor at local X=0, Z=0 and Y=1. It does not orbit with Actor rotation and is independent of the shared Third Person `CameraMount`.

## Camera composition

The startup `CameraAssignment_CharacterSelection` remains `IndividualPerPlayer`, with the existing explicit ManagerProvisioned player slot, `MemberActorTargets`, and `CameraOutput_Main`. It uses `PF_Player_FixedFollow_Presentation` and `CameraBehavior_CharacterSelection_FixedFollow`. `CinemachineFollow` uses WorldSpace binding with offset `(0, 8, -10)`; the rig has fixed downward rotation and no LookAt, `CinemachineHardLookAt`, or `CinemachineThirdPersonFollow`. It follows only the neutral Actor observation position; Actor rotation and mouse-look do not control camera position or orientation. `CharacterSelectionActorMovement` reads Move only, moves on world XZ and rotates the Actor root to its movement direction. Route and Activity do not own Camera selection or Output lifecycle.

## Manual validation order

1. Open `GameApplication_CharacterSelection` and join the ManagerProvisioned player. Confirm the Player reaches `GameplayReady`, movement works, and there is exactly one `PlayerGameplayInputReader` on the Actor.
2. Select Farmer, then Cow. Confirm the selected Actor remains visual-only and the same Player occurrence is used.
3. Confirm the Individual camera follows Actor translation at the configured offset. Rotate the Actor while stationary and confirm the observation and camera shot do not orbit.
4. Leave and rejoin, then verify selection and camera assignment recover.

**Current evidence:** static asset references and YAML composition only. The Unity import, compile, GameplayReady, and Follow checks above have not been run; no runtime PASS is claimed.
