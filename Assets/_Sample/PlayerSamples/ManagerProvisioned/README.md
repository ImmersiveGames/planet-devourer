# Player Provisioning (Manager-Provisioned)

This sample demonstrates one local Player Slot provisioned by the Framework and an application-owned Third Person Camera Assignment.

## Player composition

```text
GameApplication_ManagerProvisioned
  Player Session enabled
  Default Profile = PlayerSessionProfile_ManagerProvisioned

PlayerSessionProfile_ManagerProvisioned
  Host Provisioning = ManagerProvisioned
  Supported Slot = PlayerSlotProfile_ManagerProvisioned
  Initial Joining Open = true

Local_Player_Provisioning prefab
  PlayerInputManager (manual Join, one local Player)
  LocalPlayerProvisioningAuthoring
    Local Player Host prefab = configured Manager-Provisioned host

PlayerSlotProfile_ManagerProvisioned
  Default Actor = ActorProfile_ManagerProvisionedPlayer

ActorProfile_ManagerProvisionedPlayer
  Presentation Prefab = Manager Provisioned Actor Presentation

Manager Provisioned Actor Presentation
  MinimalPlayerMovement
  MinimalThirdPersonLook
    Tracking Pivot
  ActorCameraSubjectAuthoring
    Observation Transform = Tracking Pivot
```

Join provisions a Local Player Host, prepares and materializes the selected Actor occurrence, and admits it to the configured Activity. The Activity explicitly includes the Manager-Provisioned Slot, requires `GameplayReady`, and has an authored relocation anchor. Leave releases that Player/Actor occurrence. Rejoin creates a new Actor and Subject occurrence; the camera follows the new occurrence through the same Slot Assignment.

The Player composition is unchanged by the Camera migration.

## Camera composition

```text
GameApplication_ManagerProvisioned
  Camera Session
    PF_CameraFallback (Output prefab)
  Startup Camera Assignments
    CameraAssignment_ManagerProvisioned_ThirdPerson

CameraAssignment_ManagerProvisioned_ThirdPerson
  Occurrence = IndividualPerPlayer
  Membership = Explicit Player Slots
  Member = PlayerSlotProfile_ManagerProvisioned
  Target = member Actor Subjects
  Output = CameraOutput_Main
  Slot mapping:
    PlayerSlotProfile_ManagerProvisioned -> CameraOutput_Main
  Rig = PF_Player_ThirdPerson_Presentation
```

`PF_CameraFallback` is the physical Output prefab; it owns the Unity Camera, Cinemachine Brain, and persistent fallback rig. Its `CameraOutputAuthoring` uses `CameraOutput_Main`. The Third Person rig is the current shared rig prefab configured with `CameraRigComposer` and `CameraBehavior_ThirdPerson`; its materialized Cinemachine Camera uses `CinemachineThirdPersonFollow`. The Actor occurrence supplies the exact Subject using its `ActorCameraSubjectAuthoring.ObservationTransform`.

The Assignment stays configured for the Session even when no Player has joined. With no eligible Player occurrence, the Output presents its fallback. Join makes the Slot's Third Person occurrence eligible. Leave releases the occurrence and returns the Output to fallback. Rejoin provides a fresh Actor/Subject occurrence and Third Person becomes eligible again.

Route and Activity assets do not own or select a Camera. Route or Activity changes do not replace the startup Assignment; ordinary lifecycle changes may affect Player/Actor eligibility while the application Assignment remains active.

## Supporting Activity and audio

`Activity_ManagerProvisioned` projects the explicit Manager-Provisioned Slot, requires `GameplayReady`, and uses the sample relocation anchor. `Activity_ManagerProvisionedOIther` remains a no-Player Activity for the alternate Route. Neither Activity nor either Route contains Camera selection authoring.

The main Activity's BGM binding remains supporting ambient composition and is independent of Camera assignment.

## Observe in Unity

1. Open `ManagerProvisioned` and enter Play Mode.
2. Before Join, confirm the `CameraOutput_Main` Output shows its fallback rig.
3. Join and confirm the Third Person view tracks the Actor through the Tracking Pivot.
4. Leave and confirm the Output returns to fallback.
5. Join again and confirm the new Actor occurrence is followed.
6. Change Route/Activity and confirm no Camera Assignment is selected by that transition.

Unity import, authoring validation and Play Mode behavior must be checked in the consuming project.
