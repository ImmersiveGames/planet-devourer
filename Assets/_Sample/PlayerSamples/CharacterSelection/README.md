# Character Selection

Status: **Session Camera Assignment asset migrated locally — Unity import/Play Mode validation pending**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

Character Selection demonstrates an explicit initial Actor choice after a Player Host has joined:

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

The key distinction from JoiningControl is that a joined Player exists before any Actor / Camera Subject exists.

## Player runtime flow

```text
Open Joining
  -> Join
  -> Slot Joined
  -> Actor unresolved
  -> WaitingForActorSelection

Farmer / Cow choice
  -> PlayerSessionSelectActorCommandTrigger
  -> Actor selection committed
  -> Actor prepared/materialized
  -> GameplayReady

Leave
  -> current Player / Actor occurrence released

Rejoin
  -> new Player Host
  -> WaitingForActorSelection again
  -> fresh explicit Actor choice required
```

The Route-additive `CharacterSelection_UI.unity` remains presentation-only. Player Session command components consume scoped Framework access and do not own Session authority.

## Actor presentation boundary

Character Selection owns two ActorProfile choices:

```text
ActorProfile_Farmer
  -> FG_FarmerPresentation

ActorProfile_Cow
  -> FG_CowPresentation
```

Both concrete presentations expose the same Camera-facing boundary:

```text
Actor Presentation
  gameplay input
  minimal Player movement
  minimal Third Person look

  ActorCameraSubjectAuthoring
    Observation Transform
      -> same tracking pivot used by Third Person look
```

They do not contain:

```text
CameraOutputAuthoring
CameraRigComposer
CinemachineCamera
Framework camera selection or Output ownership
```

## Camera composition

`GameApplication_CharacterSelection` configures the physical `PF_CameraOutput_Main` and references `CameraAssignment_CharacterSelection.asset` in `StartupCameraAssignments`. This Individual Assignment maps `PlayerSlotProfile_ManagerProvisioned` to `CameraOutput_Main`, uses the Third Person rig prefab directly and targets the current member Actor Subject. The Framework derives the exact `PlayerInput.camera` association from the Assignment.

The Route and Activity remain Camera-free. Before Actor selection, the Assignment occurrence has no valid required Subject and the physical Output presents its Fallback. Selecting Farmer or Cow publishes that Actor occurrence's Subject and the same Assignment occurrence becomes eligible. Leave/Rejoin and Actor replacement reconcile exact occurrence evidence without Activity or Route Camera selection.

The Framework chooses the Individual split-screen regime where applicable. `PlayerInputManager` owns viewport geometry; Framework Camera code does not write `Camera.rect` or `Camera.pixelRect`.

## Camera lifecycle

```text
Boot -> Session Camera Assignment starts; Output Fallback covers missing target
Join -> Player Host exists; no Actor Subject; Fallback remains
Select Farmer / Cow -> exact Actor Subject resolves; PlayerInput.camera maps to CameraOutput_Main
Leave -> Player camera association clears; Fallback covers the Output
Rejoin -> new Player occurrence; fresh explicit Actor selection is required
Actor replacement -> Assignment/Output association remains; current Subject is refreshed
Shutdown -> PlayerInput.camera association and Session occurrence are cleared
```
## Route / UI composition

```text
Route_Character Selection
├── Primary Scene: ManagerProvisioned.unity
└── Route Content: CharacterSelection_UI.unity
    ├── PlayerSessionObserver
    └── Character Selection Controls
        ├── Farmer -> ActorProfile_Farmer
        └── Cow -> ActorProfile_Cow
```

The Actor selection controls are Route-scoped because the selection UI is Route Content. Joining / Leave controls in the primary gameplay scene remain Activity-scoped.

## Verification status

The Assignment-owned camera migration is authored locally but has not been re-imported or exercised in Unity. Validate the Player-to-Output association on Join, Actor selection, Leave, Rejoin and Actor replacement; then confirm shutdown clears the association and releases camera occurrences.