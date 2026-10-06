# Character Selection — Expected Unity Assets

Status: **Implemented / Integrated / Manual Play Mode: PASS**

## Application and provisioning

```text
GameApplication_CharacterSelection
  Startup Route = Route_Character Selection
  Player Session Profile = PlayerSessionProfile_CharacterSelection
    HostProvisioning = ManagerProvisioned
    ActorResolution = LeaveUnresolved
  Persistent Content = CharacterSelection_Persistent.unity
    Local_Player_Provisioning instance override
      localPlayerHostPrefab = FG_Player_CharacterSelection.prefab
        playerActorRuntimeHostPrefab = FG_PlayerActor_CharacterSelection.prefab
```

The persistent scene and prefab variants are sample-specific. Shared `ManagerProvisioned` assets are unchanged. The player explicitly selects Farmer or Cow from the selection UI.

## Actor ownership

`FG_PlayerActor_CharacterSelection` retains `PlayerGameplayInputReader` and `CharacterController`, removes `MinimalPlayerMovement` and `MinimalThirdPersonLook`, and adds `CharacterSelectionActorMovement`. That movement reads Move, translates in world XZ, and rotates the Actor root toward the movement direction; it does not consume Look.

The Actor's `ActorCameraSubjectAuthoring` observes `FollowObservation` at local `(0, 1, 0)`, a neutral anchor with X/Z zero. Farmer and Cow presentations are visual-only: their nested presentation has no duplicated input reader, CharacterController, movement, look, or ActorCameraSubject authority. The shared Third Person `CameraMount` is unchanged and is not used by this camera.

## Camera assets and contract

```text
CameraAssignment_CharacterSelection
  Occurrence = IndividualPerPlayer
  Member Slot = PlayerSlotProfile_ManagerProvisioned
  Target Policy = MemberActorTargets
  Output = CameraOutput_Main
  Rig = PF_Player_FixedFollow_Presentation
    Behavior = CameraBehavior_CharacterSelection_FixedFollow
```

The sample-specific rig uses `CinemachineFollow` in WorldSpace with offset `(0, 8, -10)` and fixed downward rotation. It has no LookAt, `CinemachineHardLookAt`, or `CinemachineThirdPersonFollow`. The startup Assignment remains owned by Camera Session; Route and Activity do not own Camera selection or Output lifecycle.

## Validation

**Manual Play Mode: PASS.** Player provisioning reaches `GameplayReady`; explicit Farmer/Cow selection works; Actor movement and rotation work without Look input; Fixed Follow tracks Actor translation and does not orbit when the Actor rotates in place; Leave/Rejoin recovers.

Automated tests and QA certification remain pending.
