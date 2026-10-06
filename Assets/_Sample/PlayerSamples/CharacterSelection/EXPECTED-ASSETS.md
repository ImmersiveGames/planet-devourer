# Character Selection — Expected Unity Assets

Status: **sample-specific Fixed Follow composition authored; Unity import and runtime validation pending**

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

The persistent scene and prefab variants are sample-specific. `ManagerProvisioned` and its shared host/prefabs are not modified. Farmer and Cow ActorProfiles continue to select their existing visual presentations.

## Actor ownership

`FG_PlayerActor_CharacterSelection` inherits the shared Player Actor composition, removes `MinimalPlayerMovement` and `MinimalThirdPersonLook`, and overrides the camera observation anchor to `FollowObservation` at local `(0, 1, 0)`. It retains `PlayerGameplayInputReader` and `CharacterController`, and adds the sample-specific `CharacterSelectionActorMovement`.

`FG_FarmerPresentation` and `FG_CowPresentation` are visual-only: the nested `FG_Presentation` input reader and CharacterController are removed, and no movement, look, or ActorCameraSubject components are added by those prefabs.

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

The sample-specific rig uses `CinemachineFollow` in WorldSpace with offset `(0, 8, -10)` and fixed downward rotation. It has no LookAt, `CinemachineHardLookAt`, or `CinemachineThirdPersonFollow`. The Actor's neutral `FollowObservation` is used as the target; the shared Third Person `CameraMount` is not changed. The Assignment remains startup-owned by Camera Session, with the existing Output topology and Route/Activity boundaries intact.

## Validation status

Static YAML and GUID checks do not confirm Unity import or runtime behavior. Validate in this order:

1. Join and confirm Player `GameplayReady`, single Actor input reader, and movement.
2. Select Farmer and Cow and confirm the Actor replacements remain ready.
3. Confirm Individual Follow maintains its base offset while the Actor translates; stationary Actor rotation does not orbit the observation or camera.
4. Confirm Leave/Rejoin recovers through the same ManagerProvisioned flow.

No Unity compile, Play Mode, or manual runtime validation has yet been performed.
