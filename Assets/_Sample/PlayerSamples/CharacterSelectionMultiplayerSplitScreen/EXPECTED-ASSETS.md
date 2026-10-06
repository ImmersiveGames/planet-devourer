# Character Selection Multiplayer Split Screen — Expected Unity Assets

Status: **Implemented / Integrated / Manual Play Mode: PASS**

## Application, Slots and provisioning

```text
GameApplication_CharacterSelectionMultiplayerSplitScreen
  Player Session Profile = PlayerSessionProfile_CharacterSelectionMultiplayerSplitScreen
    Slots = PlayerSlotProfile_CharacterSelectionMultiplayer_P1, P2
    HostProvisioning = ManagerProvisioned
    ActorResolution = LeaveUnresolved
  Persistent Content = CharacterSelectionMultiplayerSplitScreen_Persistent.unity
```

The application configures two Camera Output prefabs and starts `CameraAssignment_CharacterSelectionMultiplayer`. The selection scene presents independent selection controls for P1 and P2. Each joined Player explicitly selects Farmer or Cow; Actor replacement remains per Slot.

Both Players use the CharacterSelection host/Actor composition: one `PlayerGameplayInputReader`, one `CharacterController`, `CharacterSelectionActorMovement`, and an Actor-owned `ActorCameraSubjectAuthoring` targeting the neutral `FollowObservation` anchor at local `(0, 1, 0)`. Movement reads Move, translates on world XZ and rotates the Actor root toward movement. It has no mouse-look. Farmer/Cow presentations remain visual-only and do not duplicate gameplay or Camera Subject ownership. `CameraMount` ThirdPerson is not used or changed.

## Camera Assignment and Output mappings

```text
CameraAssignment_CharacterSelectionMultiplayer
  Occurrence = IndividualPerPlayer
  Membership = ExplicitPlayerSlots (P1, P2)
  Target Policy = MemberActorTargets
  Rig = PF_Player_FixedFollow_Presentation
    Behavior = CameraBehavior_CharacterSelection_FixedFollow
  P1 -> CameraOutput_CharacterSelection_P1
  P2 -> CameraOutput_CharacterSelection_P2
```

The Fixed Follow rig uses WorldSpace follow offset `(0, 8, -10)`, fixed downward rotation, and no LookAt, `CinemachineHardLookAt`, or `CinemachineThirdPersonFollow`. Each bound Output follows its Player's neutral observation. `PlayerInputManager` owns split-screen rectangles; Framework Camera code does not author `Camera.rect` or `Camera.pixelRect`.

## Physical Output participation and fallback

- **0 Players:** exactly one physical Fallback Camera remains enabled; the other mapped Output Camera is disabled.
- **1 Player:** only its mapped Output Camera participates; an unbound Output cannot overlap it.
- **2 Players:** both mapped Output Cameras participate in split-screen.
- **Leave/Rejoin:** the current binding controls participation; when the final Player leaves, one physical Fallback remains, and rejoin restores the mapped Output.

Assignment reservation and logical Output Fallback coverage remain intact regardless of physical participation.

## Validation

**Manual Play Mode: PASS.** P1/P2 reach `GameplayReady`, select independently, move and rotate their Actors, and receive Fixed Follow per viewport. Actor rotation does not move the neutral Subject or orbit the camera. The 0 → 1 → 2 → 1 → 0 Player sequence and Rejoin preserve physical camera coverage and prevent inactive Outputs from covering active viewports.

Automated tests and QA certification remain pending.
