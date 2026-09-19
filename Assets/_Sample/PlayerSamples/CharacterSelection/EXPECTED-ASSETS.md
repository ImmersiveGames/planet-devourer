# Character Selection — Expected Unity Assets

Status: **CAMERA-029-F MIGRATED LOCALLY — UNITY / QA REVALIDATION PENDING**

This file records the Character Selection materialization boundary after migration to the current Camera composition model.

## Application intent

```text
GameApplication_CharacterSelection.asset

PlayerSessionProfile_CharacterSelection.asset
  HostProvisioning = ManagerProvisioned
  ActorResolution = LeaveUnresolved
```

Character Selection remains distinct from default-resolving Player Provisioning because initial Actor resolution is explicit.

## Materialized application

```text
CharacterSelection/
  GameApplication_CharacterSelection.asset

  Player/
    PlayerSessionProfile_CharacterSelection.asset
    ActorProfile_Farmer.asset
    ActorProfile_Cow.asset

  Routes/
    Route_Character Selection.asset
    Route Content Character Selection.asset

  Activities/
    Activity_Character Selection.asset

  Scenes/
    CharacterSelection_UI.unity

  Scripts/
    CharacterSelectionActorButtonPresenter.cs
```

## Actor presentation boundary

```text
ActorProfile_Farmer
  -> presentationPrefab = FG_FarmerPresentation

ActorProfile_Cow
  -> presentationPrefab = FG_CowPresentation
```

Both concrete presentations derive from the shared `FG_Presentation` baseline and are expected to contain:

```text
PlayerGameplayInputReader
CharacterController
minimal Player movement
minimal Third Person look
ActorCameraSubjectAuthoring
  -> Observation Transform = CameraMount
character-specific visual
```

They must not contain:

```text
PlayerGameplayCameraAuthoring
CameraRigComposer
CinemachineCamera
CinemachineFollow / CinemachineThirdPersonFollow
a gameplay CameraRequest owner
a Camera Output
```

## Camera composition reuse

Character Selection reuses the Manager-Provisioned persistent composition referenced by its GameApplication:

```text
ManagerProvisioned_Persistent.unity
  -> Manager Provisioned Camera
     -> CameraOutput_Main
        -> Fixed Default Camera Rig
     -> CameraSharedComposition
        SubjectPolicy = AllAvailableSubjects
        Composition Rig = ThirdPerson Gameplay Rig
        request precedence = 50
```

The selected Actor contributes only Camera Subject evidence. No CharacterSelection-specific Camera Composition is expected.

## Route / UI boundary

```text
Route_Character Selection
  -> Primary Scene = ManagerProvisioned.unity
  -> Route Content = CharacterSelection_UI.unity

CharacterSelection_UI
  -> PlayerSessionObserver
  -> Farmer / Cow selection controls
  -> PlayerSessionSelectActorCommandTrigger
  -> CharacterSelectionActorButtonPresenter
```

UI code remains presentation-only and does not own Player or Camera lifecycle.

## Expected runtime path

```text
Join
  -> WaitingForActorSelection
  -> Fixed Default Camera remains

Select Farmer / Cow
  -> Actor selection commit
  -> Actor preparation/materialization
  -> ActorCameraSubjectAuthoring publishes exact CameraMount evidence
  -> CameraSharedComposition becomes eligible
  -> ThirdPerson Gameplay Rig
  -> GameplayReady

Leave
  -> Actor occurrence ends
  -> Camera Subject becomes unavailable
  -> Composition request releases
  -> Fixed Default

Rejoin
  -> WaitingForActorSelection
  -> fresh explicit Actor selection
  -> fresh Subject occurrence
  -> ThirdPerson Gameplay Rig
```

## Validation status

Historical Character Selection Player proofs remain useful for Player lifecycle behavior, but they predate this Camera migration.

For CAMERA-029-F Character Selection migration:

```text
Implemented = YES
Static repository verification = PASS (scope, legacy Camera ownership, YAML headers and key serialized references reviewed)
Unity tested = NO
QA Framework tested = NO
Integrated = NO
Validated = NO
```

Required reproof is a real consumer Play Mode run covering Farmer and Cow selection, Leave -> Fixed Default, Rejoin, fresh selection, ThirdPerson Camera, movement/look and GameplayReady.
