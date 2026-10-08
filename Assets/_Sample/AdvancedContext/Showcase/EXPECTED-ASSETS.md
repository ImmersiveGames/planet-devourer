# Planned Unity Assets — Not Yet Authored

This file is a planning checklist, not a runnable sample or a current authoring
recipe. No assets listed below are present yet. Do not create old Camera
Presentation/Request types. When the sample is approved for implementation,
derive its concrete composition from the current Session Camera surfaces and
the working Game Flow consumer example.

```text
GameApplication_AdvancedContext.asset
HUB / Menu composition
Route / Activity / Scene for one focused Camera Assignment change
SessionCameraAssignmentAsset values with explicit rig, membership, target and Output mapping
ISessionCameraAssignmentCommandConsumer or SessionCameraAssignmentCommandTrigger
ActorCameraSubjectAuthoring on the exact target Actor occurrence
optional Audio/provider scenarios only when com.immersive.audio requirement is explicit
```

Use one physical Output unless the demonstrated contract explicitly requires an
individual-per-Player Output topology. `PlayerInputManager` owns split-screen
viewport geometry; the Framework does not author camera rectangles.
