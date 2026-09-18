# Getting Started

Status: **SAMPLE 00 COMPLETE FOR AUTHORING / PLAY MODE PROOF — CAMERA COMPOSITION NORMALIZED 2026-09-17**  
UPM promotion: **PENDING package finalization/import proof**

## Demonstration Application

```text
Minimal Game
```

Purpose: demonstrate the **minimum coherent Framework application**.

Getting Started proves navigation, not gameplay.

## Canonical Scene-Provided coverage

Getting Started / Minimal Game is the sample program's **canonical Scene-Provided Player reference**.

The Player sample family does not require a separate dedicated Scene-Provided Demonstration Application under Player for the same baseline. Any future dedicated Scene-Provided Player sample requires evidence of a distinct consumer contract.

Player-specific sample scope is governed by `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

## Current result

Minimal Game now provides and proves the intended baseline:

```text
GameApplication
PlayerSessionProfile
Persistent Content
one Route
one Activity
Scene-Provided Player
GameplayReady participation
explicit Actor Camera Subject
typed Camera View -> Camera Output association
Output-owned Default Camera Rig
Mounted / First Person presentation
minimal Move / Look navigation
```

Camera presentation follows the current logical/physical ownership split:

```text
Scene Player / Actor Presentation
  -> ActorCameraSubjectAuthoring
  -> Camera Subject evidence

CameraSharedComposition
  -> CameraView_Gameplay
  -> CameraOutput_Main

CameraOutputAuthoring
  -> physical Unity Camera + CinemachineBrain
  -> explicit Default Camera Rig
```

Getting Started is single-player and Scene-Provided. It does not require a `PlayerInputManager` split-screen layout authority. The physical Unity Camera remains full-screen and the Framework does not author or write `Camera.rect`.

The current Play Mode proof finishes with:

```text
Framework boot succeeded
Activity Ready
blockingIssues = 0
Camera Output initialized
Scene Player admitted
Move received
Look received
```

See `MinimalGame/README.md` for the runnable composition and inspection path.

## Program status

For sample construction, Getting Started / Sample 00 is **closed**.

Official UPM release remains a later program-wide finalization step. The final `GettingStarted` group must still be promoted into `com.immersive.framework/Samples~/GettingStarted` and validated from a real Package Manager import before being called release-ready.
