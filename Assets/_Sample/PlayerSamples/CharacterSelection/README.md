# Character Selection

Status: **CAMERA-029-F MIGRATED LOCALLY — prior Player lifecycle proof preserved; Camera Unity revalidation pending**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.
Canonical Camera composition authority: `IF-ADR-029 — Camera Composition, Group Presentation and Camera View Removal`.

Character Selection remains the Player Demonstration Application for explicit initial Actor choice:

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

The Player lifecycle contract is unchanged. The Camera composition is migrated to the current `Subject(s) -> Composition -> Rig -> Request -> Output` model.

## Runtime flow

```text
Open Joining
  -> Join
  -> Slot Joined
  -> Actor unresolved
  -> Preparing / WaitingForActorSelection

PlayerSessionObserver.OnPlayerJoined
  -> show Character Selection Controls

Farmer / Cow choice
  -> PlayerSessionSelectActorCommandTrigger
  -> Framework commits Actor selection
  -> Actor preparation
  -> selected ActorProfile.PresentationPrefab
  -> GameplayReady
  -> Actor Camera Subject available

PlayerSessionObserver.OnActorSelected
  -> hide Character Selection Controls
```

Leave/Rejoin returns the Session Slot to `WaitingForActorSelection` and requires a fresh explicit Actor choice.

## Current Actor presentation composition

Character Selection owns the Farmer/Cow choices and reuses the shared Player presentation baseline:

```text
Assets/_Sample/PlayerSamples/Shared/Prefabs/
  FG_Player.prefab
  FG_PlayerActor.prefab
  FG_Presentation.prefab

ActorProfile_Farmer
  -> FG_FarmerPresentation

ActorProfile_Cow
  -> FG_CowPresentation
```

Both concrete presentations now follow the same Camera-facing boundary:

```text
Actor Presentation
  -> gameplay input
  -> minimal Player movement
  -> minimal Third Person look
  -> ActorCameraSubjectAuthoring
     Observation Transform = CameraMount
```

They do **not** contain a gameplay `CameraRequest`, `CameraRigComposer`, Cinemachine camera, or Player-owned gameplay Camera authority.

## Camera composition

Character Selection reuses `ManagerProvisioned_Persistent.unity`, so it also reuses the already migrated Manager-Provisioned Camera composition. It does not create a CharacterSelection-specific Camera Composition.

```text
selected Actor occurrence
  -> ActorCameraSubjectAuthoring / CameraMount
  -> Camera Subject availability

Manager Provisioned Camera
  -> Camera Output / CameraOutput_Main
     Default Camera Rig = Fixed
  -> CameraSharedComposition
     SubjectPolicy = AllAvailableSubjects
     Composition Rig = Gameplay Camera Rig
     request precedence = 50
  -> Gameplay Camera Rig
     CameraRigBehavior_ThirdPerson
```

Expected Camera lifecycle:

```text
before Actor selection
  -> no selected Actor Subject
  -> Fixed Default

select Farmer or Cow
  -> selected Actor occurrence materializes
  -> Subject becomes available
  -> Composition publishes gameplay request
  -> ThirdPerson Gameplay Rig

Leave
  -> Actor occurrence ends
  -> Subject becomes unavailable
  -> Composition request releases
  -> Fixed Default

Rejoin + fresh Actor selection
  -> fresh Actor occurrence only
  -> fresh Subject
  -> ThirdPerson Gameplay Rig
```

## Route / UI composition

```text
Route_Character Selection
├── Primary Scene: ManagerProvisioned.unity
└── Route Content: CharacterSelection_UI.unity
    ├── PlayerSessionObserver
    └── Character Selection Controls
        ├── Farmer button
        └── Cow button
```

The UI remains presentation-only. Selection authority stays in `PlayerSessionSelectActorCommandTrigger`.

## Ownership boundary

The sample/game owns:

```text
which ActorProfile choices are presented
button layout and visual presentation
Farmer/Cow concrete presentation authoring
minimal gameplay movement/look
the exact Actor Camera observation Transform
```

The Framework owns:

```text
Session Player lifecycle
Actor-selection commit and preparation
Actor occurrence lifetime
Camera Subject publication lifetime
Camera Composition arbitration
Camera request participation
Camera Output and Fixed Default fallback
```

Do not add private Player discovery, direct Session mutation, parallel Actor-selection authority, Player-owned Camera request/rig/output, or a second CharacterSelection Camera Composition.

## Validation evidence

Historical Player lifecycle and materialization proofs from **2026-08-28** and **2026-09-05** remain evidence for the Character Selection contract.

The **2026-09-19 CAMERA-029-F migration changes the physical Camera composition**, so those earlier runs are not treated as validation of the new Camera path.

Current state:

```text
Implemented
  -> Farmer/Cow legacy local Camera rigs removed
  -> MinimalFollowMovement dependency on the old local Camera removed
  -> ActorCameraSubjectAuthoring added explicitly
  -> CameraMount is the authored observation Transform
  -> shared Manager-Provisioned Camera Composition reused

Static repository verification
  -> pending final branch diff review

Unity / consumer Play Mode
  -> NOT RUN for this migration

QA Framework
  -> NOT RUN for this migration

Integrated / validated
  -> NO
```

Required consumer reproof:

```text
Join
-> WaitingForActorSelection
-> select Farmer
-> correct PresentationPrefab
-> Subject = Farmer CameraMount
-> ThirdPerson Gameplay Rig
-> movement/look functional
-> Leave -> Fixed Default
-> Rejoin
-> select Cow
-> Subject = Cow CameraMount
-> ThirdPerson Gameplay Rig
-> movement/look functional
```
