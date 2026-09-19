# Player Provisioning — Joining Control

Status: **CAMERA-029-F MIGRATED LOCALLY — UNITY VALIDATION PENDING**

This sample reuses the Manager-Provisioned Player and persistent Camera composition while demonstrating explicit Joining control.

## Session policy

```text
HostProvisioning = ManagerProvisioned
initialJoiningOpen = false
one supported Player Slot
```

Open/Close Join controls belong to the Joining Control Activity only. They are not added to the ManagerProvisioned Activity.

## Camera composition

```text
Actor Presentation
  -> ActorCameraSubjectAuthoring
     Observation Transform = CameraMount

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

The Actor supplies only Camera Subject evidence. Camera Composition owns the gameplay request; Player provisioning, `PlayerInput` and the Player GameObject do not own a gameplay Camera request or rig.

## Expected flow

```text
boot with Joining closed
  -> no Player / no Subject
  -> Fixed Default

Join while closed
  -> rejected

Open Join -> Join
  -> new Actor occurrence
  -> Camera Subject available
  -> ThirdPerson Gameplay Rig

Close Join
  -> current Player remains

Leave
  -> Subject unavailable
  -> Composition request released
  -> Fixed Default

Join while closed
  -> rejected

Open Join -> Rejoin
  -> new Actor occurrence only
  -> ThirdPerson Gameplay Rig
```

## Boundary

- `PlayerInputManager` remains the physical split-screen owner.
- The Framework does not write `Camera.rect` from Camera Composition.
- No Route/Activity Camera override participates in this flow.
- No Player-owned gameplay Camera authoring is required.
