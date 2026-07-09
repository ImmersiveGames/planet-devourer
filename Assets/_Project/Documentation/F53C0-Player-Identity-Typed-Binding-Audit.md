# F53C0 - FIRSTGAME Player Identity, Typed Binding and Adapter Chain Audit

Status: Documented
Date: 2026-07-09
Scope: `Assets/_Project`

## Summary

The real FIRSTGAME player is already mostly wired by typed component references. The remaining weak points are editor-only object lookup by name and repeated identity/action-map strings that can drift before a facade exists.

No runtime, scene, prefab or asset was changed by this audit.

## Real Player Snapshot

Current real player object in `Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity`:

```text
PlayerPrototype
  ObjectEntryDeclaration
  FirstGamePlayerMover
  PlayerInput
  UnityPlayerInputGateAdapter
  UnityTransformResetParticipant
  UnityResetSubjectAdapter
  FirstGamePlayerResettableState
  PlayerActorDeclaration
  PlayerSlotDeclaration
  PlayerSlotOccupancy
  UnityPlayerInputActivationTargetBehaviour
  UnityPlayerInputBridgeTargetBehaviour
  PlayerControlBindingTargetBehaviour
```

Canonical identity:

- slot: `PlayerSlotDeclaration.slotId = player.1`
- actor: `PlayerActorDeclaration.actorId = firstgame.player`
- reset subject: `UnityResetSubjectAdapter.sourcePlayerActor = PlayerActorDeclaration`, resolved as `Actor:firstgame.player`
- object entry: `ObjectEntryDeclaration.objectEntryId = firstgame.player`

`GameObject.name = PlayerPrototype` is only a human/editor diagnostic label.

## Main Matrix

| Area | Current source | Uses string? | Should be type? | Risk | Recommended action |
| --- | --- | --- | --- | --- | --- |
| Player object | `m_Name: PlayerPrototype` | Yes | Yes | Rename breaks editor tools | Find by `PlayerActorDeclaration`/`PlayerSlotDeclaration` |
| Player slot | `PlayerSlotDeclaration.slotId = player.1` | ID | Partial | OK identity, weak bind if copied | Keep ID; use declaration reference for binding |
| Actor id | `PlayerActorDeclaration.actorId = firstgame.player` | ID | Partial | OK reset/save identity | Keep declaration as canonical source |
| Reset subject | `sourcePlayerActor` reference | No | Yes | OK | Keep; do not fill separate `subjectId` for player |
| PlayerInput | direct references in mover/gate/F52 targets | No | Yes | OK | Keep direct references |
| Gate action map | `gameplayActionMapName = Player` | Yes | Tolerated | Drift from activation/mover | Validate against asset and consolidate later |
| Activation action map | `actionMapName = Player` | Yes | Tolerated | Can diverge from gate | Validate against asset and source from facade later |
| Mover action map/action | `Player/Move` | Yes | Tolerated | Consumer gameplay config drift | OK for FIRSTGAME mover; validated before use |
| Pause action | `Global/Pause` in `FG_UIGlobal` | Yes | Tolerated | Asset rename breaks pause | Already validated against `InputActionAsset` |
| Camera target | `FrameworkCameraAnchorHost.trackingTarget/lookAtTarget` | No in scene | Yes | OK at runtime | Keep typed `Transform`; choose root vs child anchor later |
| Camera setup | `FindInScene(scene, "PlayerPrototype")` | Yes | Yes | Recreates fragile wiring | Replace in F53C1/F53C2 |
| F52 validator | `input.name == PlayerPrototype` fallback | Yes | Yes | Can validate wrong player | Resolve by canonical declarations |
| F51 PlayerView chain | Not present on real player | N/A | Yes | No FIRSTGAME proof yet | Add proof after identity canonicalization |
| Resettable state | file is `FirstGamePlayerResettableState` | Metadata drift | Yes | Scene still shows old `m_EditorClassIdentifier` text | Validate import in Unity; repair via Editor only if needed |

## String-Based Links Found

Wrong for future object/component binding:

- `FirstGameRealPlayerBindingValidator` uses `input.name == "PlayerPrototype"` as preferred fallback.
- `FirstGameCameraCutSetup` uses `FindInScene(scene, "PlayerPrototype")`.
- `FirstGameCameraCutSetup` also locates scene roots/rigs by name for setup idempotence. Those are editor setup names, not player identity, but the player target should move to declaration-based discovery.

Tolerated:

- `Player`, `Global`, `UI`, `Pause`, `Move` because the code resolves them through `InputActionAsset`.
- `player.1`, `firstgame.player`, `Actor:firstgame.player` as identity/evidence text, not object lookup.

Allowed:

- display names, target names, reasons and diagnostic tags.

## PlayerView / Camera Chain

The real FIRSTGAME scene does not yet contain:

```text
PlayerViewBindingTargetBehaviour
PlayerViewCameraTargetBindingTargetBehaviour
PlayerViewCameraActivationTargetBehaviour
```

The active camera chain uses framework route/activity camera components and `FrameworkCameraAnchorHost` with direct `Transform` references to the player root. This is acceptable for current camera behavior but does not prove the F51 PlayerView chain on the real player.

Next proof should decide whether camera should target:

- `PlayerPrototype.transform`; or
- a child such as `CameraTarget` / `LookAtTarget`.

## PlayerControl / Input Chain

Current chain is valid for F53B:

- `PlayerInput` exists and references an `InputActionAsset`.
- expected gameplay action map `Player` exists.
- `PlayerControlBindingTargetBehaviour` exists.
- `UnityPlayerInputBridgeTargetBehaviour` exists and references the real `PlayerInput`.
- `UnityPlayerInputActivationTargetBehaviour` exists and references the real `PlayerInput`.

Risk:

- `UnityPlayerInputGateAdapter.gameplayActionMapName`, `UnityPlayerInputActivationTargetBehaviour.actionMapName`, `FirstGamePlayerMover.actionMapName` and `PlayerInput.m_DefaultActionMap` are separate string fields.
- They are currently all `Player`; the future facade should make this one validated authoring value.

## Reset Subject / Identity

The real player reset setup is correct for the current proof:

```text
UnityResetSubjectAdapter
  idGeneration = AuthoredStableId
  subjectId = empty
  sourcePlayerActor = PlayerActorDeclaration
  scope = Activity
```

`Actor:firstgame.player` is valid as reset subject ID/evidence. The source must remain `PlayerActorDeclaration`, not a loose string field.

`scope = Activity` is acceptable for current Activity reset/restart behavior. Revisit before save/progression or persistent route player behavior.

## PlayerPrototype Ergonomics

Canonical now:

- `PlayerInput`
- `PlayerSlotDeclaration`
- `PlayerActorDeclaration`
- `PlayerSlotOccupancy`
- `UnityResetSubjectAdapter`

Necessary now:

- `FirstGamePlayerMover`
- `UnityPlayerInputGateAdapter`
- `UnityTransformResetParticipant`
- `FirstGamePlayerResettableState`
- F52 targets used by the accepted proof

Needed later:

- F51 PlayerView/camera target/camera activation targets
- explicit camera anchor child if root targeting is not precise enough

Diagnostic:

- object name `PlayerPrototype`
- display names, reason strings, diagnostic tags

Candidate facade after proofs:

- a single authoring component that references canonical declarations, `PlayerInput`, camera target and F51/F52 target components, validates them and applies configuration without hidden lookup.

## Findings

Severity: Medium

- FIRSTGAME editor tooling still uses player object name as a lookup path. This is editor-only, but it is the highest-risk pattern to remove before facade work.

Severity: Medium

- Real FIRSTGAME has no PlayerView / camera target / camera activation proof using F51 target components. The current camera route/activity path is valid but does not prove the player binding chain.

Severity: Low

- Slot/action-map strings are duplicated across several components. Current validation keeps this safe enough, but a facade should remove drift.

Severity: Low

- `FG_Gameplay.unity` still contains old `m_EditorClassIdentifier` text for the resettable component while the script GUID points to `FirstGamePlayerResettableState.cs`. Confirm Unity import/Inspector shows the expected component before treating this as clean.

## Final Recommendations

1. Remove `PlayerPrototype` name lookup from FIRSTGAME player validation/setup.
2. Create typed editor/facade references to `PlayerSlotDeclaration`, `PlayerActorDeclaration`, `PlayerInput`, camera target `Transform` and optional `Camera`.
3. Keep canonical identity as `PlayerSlotDeclaration` for slot and `PlayerActorDeclaration` for actor/reset/save evidence.
4. Prove F51 view/camera targets on the real player before full chain proof.
5. Run identity canonicalization before facade work.
6. Create the facade only after F53C1-F53C3, and make it authoring-only with fail-fast validation.

Recommended order:

```text
F53C1 - FIRSTGAME Player Identity Canonicalization
F53C2 - FIRSTGAME Real PlayerView / Camera Wiring Proof
F53C3 - FIRSTGAME Full Player Binding Chain Proof
F53D  - Canonical Player Binding Authoring Facade
```

## Manual Validation Checklist

- Open `FG_Gameplay.unity` and confirm `PlayerPrototype` still imports without missing scripts.
- Confirm the resettable component appears as `FirstGamePlayerResettableState`.
- Confirm `PlayerInput.actions` contains `Player/Move`, `Global/Pause` and `UI` as expected.
- Confirm `FrameworkCameraAnchorHost` still references the intended player transform.
- Run the existing F53B editor validation only after import is clean.
