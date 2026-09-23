# Getting Started

Status: **CAMERA-032-D/E MIGRATED LOCALLY — Unity revalidation pending**
UPM promotion: **PENDING package finalization/import proof**

## Demonstration Application

```text
Minimal Game
```

Purpose: demonstrate the **minimum coherent Framework application**.

Getting Started proves navigation, not gameplay.

## Canonical Scene-Provided coverage

Getting Started / Minimal Game is the sample program's **canonical Scene-Provided Player reference**.

The Player sample family does not require a separate dedicated Scene-Provided Demonstration Application for the same baseline. Any future dedicated Scene-Provided Player sample requires evidence of a distinct consumer contract.

Player-specific sample scope is governed by `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

## Current composition

```text
GameApplication
PlayerSessionProfile
Persistent Content
one Route
one Activity
Scene-Provided Player
GameplayReady participation
explicit Actor Camera Subject

GameApplication Camera Session
  -> one explicit physical Camera Output
  -> Output-owned Fixed Default Camera Rig
  -> Player 1 -> Output binding
  -> Player 1 -> Camera Presentation binding

Activity Camera Presentation
  -> ExplicitSelection
  -> Mounted / First Person rig
  -> normal CameraRequest arbitration

minimal Move / Look navigation
```

Camera ownership follows IF-ADR-032:

```text
Scene Player / Actor Presentation
  -> ActorCameraSubjectAuthoring
  -> Camera Subject evidence

GameApplication Camera Session
  -> PF_CameraOutput_Main
  -> CameraOutput_Main
  -> physical Unity Camera + CinemachineBrain
  -> Fixed Default Camera Rig
  -> Player 1 -> Output
  -> Player 1 -> CameraPresentation_MinimalGame_Player

Activity_MinimalGame
  -> CameraPresentation_MinimalGame_Player
  -> live Mounted / First Person Presentation occurrence
```

Persistent Content does **not** own Camera Outputs, Player Camera policies or `CameraSharedComposition`.

Getting Started is single-player and Scene-Provided. It does not require a `PlayerInputManager` split-screen layout authority. The physical Unity Camera remains full-screen and the Framework does not author or write `Camera.rect`.

## Unity validation target

The migrated sample must prove:

```text
Framework boot succeeded
Camera Session Output materialized
Activity Ready
blockingIssues = 0
Scene Player admitted
Player Camera Subject available
Player Camera Presentation selection attached
Mounted / First Person Presentation active
Move received
Look received
```

See `MinimalGame/README.md` for the runnable composition and inspection path.

## Program status

The authoring migration is materialized. Unity Play Mode revalidation is required before Getting Started / Sample 00 is marked closed again.

Official UPM release remains a later program-wide finalization step. The final `GettingStarted` group must still be promoted into `com.immersive.framework/Samples~/GettingStarted` and validated from a real Package Manager import before being called release-ready.
