# FIRSTGAME CameraComposer Proof

Status: C6 consumer proof setup
Date: 2026-07-09
Scope: FIRSTGAME / `Assets/_Project`

## Objective

Prove that FIRSTGAME can consume the official package Camera Product Surface:

```text
CameraComposer
PlayerComposer target source
Cinemachine rig materialization
Apply/Rebuild idempotence
```

This proof does not define framework contracts and does not replace the official package flow.

## Scene

Open or let the proof tool open:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

## Expected source player

The real FIRSTGAME player is the scene player with typed framework identity:

```text
PlayerComposer.ActorId = player.actor
PlayerComposer.PlayerSlotId = player.1
```

The proof resolves the player by these typed IDs, not by `GameObject.name`.

## Menu

Run:

```text
FIRSTGAME > Immersive Framework > Camera Composer Proof > Configure Gameplay CameraComposer Proof
```

The tool:

```text
1. Opens FG_Gameplay if needed.
2. Resolves exactly one PlayerComposer by ActorId/PlayerSlotId.
3. Resolves exactly one gameplay Unity Camera by CinemachineBrain evidence.
4. Creates or reuses a CameraComposer that explicitly references the PlayerComposer.
5. Configures SinglePlayerFollowCamera intent.
6. Runs Validate.
7. Runs Apply/Rebuild twice.
8. Saves the scene when successful.
```

## Expected created object

If no CameraComposer already references the real PlayerComposer, the tool creates:

```text
FIRSTGAME_CameraComposerRig
  CameraComposer
  FIRSTGAME Cinemachine Camera
```

The Unity Camera remains the existing gameplay camera that already owns the CinemachineBrain.

## Expected log

```text
[FIRSTGAME][CameraComposerProof] status='Succeeded' camera='FIRSTGAME_CameraComposerRig' playerActorId='player.actor' playerSlotId='player.1' followTarget='CameraTarget' lookAtTarget='LookAtTarget' createdSecond='0' blockedSecond='0' resolvedByName='False'
```

The official package log should also appear:

```text
[Immersive.Framework][CameraComposer] Apply/Rebuild completed. camera='FIRSTGAME_CameraComposerRig' mode='SinglePlayerFollowCamera' source='PlayerComposer' ... blocked='0'
```

On the second Apply/Rebuild:

```text
created='0'
blocked='0'
```

## What this proves

```text
FIRSTGAME has a real PlayerComposer source.
CameraComposer consumes PlayerComposer.CameraTarget.
CameraComposer consumes PlayerComposer.LookAtTarget.
Cinemachine rig is materialized by official package tooling.
Apply/Rebuild is idempotent in the real consumer scene.
No Camera.main fallback is used by the proof.
No runtime manager/singleton/service locator is introduced.
```

## What this does not prove

```text
multiplayer camera
split-screen
Route/Activity camera rewrite
runtime CameraContext
QAFramework regression
```

## Legacy note

Older FIRSTGAME route/activity camera setup remains support/legacy evidence. The active camera product proof is the official `CameraComposer` flow.
