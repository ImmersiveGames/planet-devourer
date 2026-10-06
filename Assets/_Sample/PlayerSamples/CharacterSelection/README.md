# Character Selection

Status: **Implemented / Integrated / Manual Play Mode: PASS**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

Character Selection keeps the existing `ManagerProvisioned` host provisioning and `LeaveUnresolved` actor selection flow. Its application loads a CharacterSelection-specific persistent scene whose provisioning prefab variant points to the CharacterSelection player variant. Shared `ManagerProvisioned` assets remain unchanged.

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

Farmer and Cow are selectable visual-only presentations. They do not add input, movement, look, CharacterController, or Camera Subject authority. The selected Actor remains the single owner of gameplay input and movement. Selection is explicit: the player chooses Farmer or Cow through the sample selection UI.

`FollowObservation` is a neutral Actor anchor at local X=0, Z=0 and Y=1. It does not orbit with Actor rotation and is independent of the shared Third Person `CameraMount`.

`CharacterSelectionActorMovement` reads Move only, moves on the world XZ plane, and rotates the Actor root toward its effective movement direction. It does not read Look or control the Camera.

## Camera composition

The startup `CameraAssignment_CharacterSelection` remains `IndividualPerPlayer`, with the existing explicit ManagerProvisioned player slot, `MemberActorTargets`, and `CameraOutput_Main`. It uses `PF_Player_FixedFollow_Presentation` and `CameraBehavior_CharacterSelection_FixedFollow`. `CinemachineFollow` uses WorldSpace binding with offset `(0, 8, -10)`; the rig has fixed downward rotation and no LookAt, `CinemachineHardLookAt`, or `CinemachineThirdPersonFollow`. It follows only the neutral Actor observation position; Actor rotation and mouse-look do not control camera position or orientation. Route and Activity do not own Camera selection or Output lifecycle.

## Manual validation

**PASS:** Player joins and reaches `GameplayReady`; Farmer and Cow selection works; movement translates and rotates the Actor; Fixed Follow tracks translation at the configured offset; rotating the stationary Actor does not orbit the observation or camera; Leave/Rejoin recovers the Player and camera assignment.

Automated tests and QA certification remain pending.
