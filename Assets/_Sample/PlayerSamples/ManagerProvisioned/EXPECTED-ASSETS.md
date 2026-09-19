# Expected Unity Assets

Materialize through Unity:


```text
GameApplication_ManagerProvisioned.asset
PlayerSessionProfile_ManagerProvisioned.asset
manager/provisioning authoring composition
supporting Route / Activity / Scene assets
optional application-local HUB when multiple compatible Scenarios exist
```

Camera composition:

```text
Manager Provisioned Camera.prefab
  one Unity Camera + CinemachineBrain
  CameraOutputAuthoring -> CameraOutput_Main
  Default Camera Rig -> CameraRigBehavior_Fixed
  Gameplay Camera Rig -> CameraRigBehavior_ThirdPerson
  CameraSharedComposition
    SubjectPolicy = AllAvailableSubjects
    OutputDefinition = CameraOutput_Main
    CompositionRig = Gameplay Camera Rig
    requestPrecedence = 50
```
