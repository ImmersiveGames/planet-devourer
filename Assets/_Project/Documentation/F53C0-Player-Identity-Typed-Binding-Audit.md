# F53C0 - FIRSTGAME Player Identity, Typed Binding and Adapter Chain Audit

Status: Documented; F53C1 follow-up applied
Date: 2026-07-09
Scope: `Assets/_Project`

## Summary

The real FIRSTGAME player is mostly wired by typed component references. F53C0 identified the remaining weak points as editor-only lookup by object name and repeated identity/action-map strings that can drift before a facade exists.

F53C1 applies the accepted follow-up for the highest-risk item: editor tooling must resolve the real player by canonical declarations and `PlayerInput`, not by `GameObject.name`.

No runtime, scene, prefab or gameplay asset was changed by this audit note.

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

## F53C1 Follow-up Status

| Area | F53C0 finding | F53C1 status |
| --- | --- | --- |
| Player validator lookup | `FirstGameRealPlayerBindingValidator` preferred `input.name == "PlayerPrototype"` | Replaced with `FirstGamePlayerIdentityResolver` using `PlayerActorDeclaration`/`PlayerSlotDeclaration`/`PlayerInput` |
| Camera setup lookup | `FirstGameCameraCutSetup` used `FindInScene(scene, "PlayerPrototype")` for player target | Replaced with canonical resolver and typed `Transform` target assignment |
| Object name usage | Object name could affect proof result | Object name remains diagnostic-only in logs |
| Ambiguity handling | Fallback could pick an arbitrary `PlayerInput` | Resolver fails explicitly on zero/multiple/partial/divergent candidates |
| F53B log shape | Existing proof did not show identity source | Log now includes `identitySource`, `resolutionSource`, `resolvedByName`, `actorId`, and `playerSlotId` |

## String Classification

Allowed: log, reason, display and evidence text.

- `displayName`, `diagnosticTag`, `reason`, target names and log fields.
- `GameObject.name` only for diagnostics.

Tolerated: Unity Input System names, with validation.

- `Player`, `Global`, `UI`, `Pause`, `Move`.
- Acceptable only because the code resolves them through `InputActionAsset.FindActionMap` / `FindAction` before use.

Wrong for object/component binding:

- Any lookup that requires `GameObject.name == "PlayerPrototype"`.
- Any editor setup path that silently assigns an arbitrary `PlayerInput` when canonical declarations are missing or ambiguous.

## Canonical Player Identity

| Question | Answer |
| --- | --- |
| Which component declares the slot? | `PlayerSlotDeclaration` |
| Which component declares the actor id? | `PlayerActorDeclaration` |
| Which component should source reset identity? | `UnityResetSubjectAdapter.sourcePlayerActor -> PlayerActorDeclaration` |
| Which component should feed future save/progression? | `PlayerActorDeclaration` for actor identity; `PlayerSlotDeclaration` only for player seat |
| Which data is visual/human? | `GameObject.name`, display names, target names |
| Which data is stable identity? | `PlayerSlotId`, `ActorId`, `ResetSubjectId`, `ObjectEntryId` in their own domains |
| Which data is runtime reference? | `PlayerInput`, `Transform`, `Camera`, declaration component references |

Do not compare identities from different domains. `player.1`, `firstgame.player`, `Actor:firstgame.player` and `ObjectEntry:firstgame.player` are related evidence, not interchangeable keys.

## PlayerView / Camera Chain

The real FIRSTGAME scene still does not prove F51 PlayerView target components:

```text
PlayerViewBindingTargetBehaviour
PlayerViewCameraTargetBindingTargetBehaviour
PlayerViewCameraActivationTargetBehaviour
```

The current camera path remains:

```text
FirstGameCameraAnchors
  FrameworkCameraAnchorHost
    trackingTarget = resolved player Transform
    lookAtTarget = resolved player Transform
```

F53C1 improves how the editor setup resolves that transform. It does not decide whether the final camera target should remain the player root or move to a child anchor such as `CameraTarget` / `LookAtTarget`.

## Remaining Findings After F53C1

Severity: Medium

- Real FIRSTGAME still has no PlayerView / camera target / camera activation proof using F51 target components.

Severity: Low

- Slot/action-map strings are duplicated across several components. Current validation keeps this safe enough, but a future facade should remove drift.

Severity: Low

- `FG_Gameplay.unity` previously contained old `m_EditorClassIdentifier` text for the resettable component while the script GUID pointed to `FirstGamePlayerResettableState.cs`. Confirm Unity import/Inspector shows the expected component before treating this as clean.

## Recommended Order

```text
F53C1 - FIRSTGAME Player Identity Canonicalization        Applied as delta
F53C2 - FIRSTGAME Real PlayerView / Camera Wiring Proof   Next
F53C3 - FIRSTGAME Full Player Binding Chain Proof
F53D  - Canonical Player Binding Authoring Facade
```

## Manual Validation Checklist

- Open `FG_Gameplay.unity` and confirm the real player imports without missing scripts.
- Confirm the resettable component appears as `FirstGamePlayerResettableState`.
- Confirm `PlayerInput.actions` contains `Player/Move`, `Global/Pause` and `UI` as expected.
- Run `FIRSTGAME > Immersive Framework > Validate Real Player Binding` and confirm:

```text
identitySource='PlayerActorDeclaration+PlayerSlotDeclaration'
resolvedByName='False'
failureReason='None'
```

- Run camera setup and confirm `FrameworkCameraAnchorHost` references the resolved player transform.
