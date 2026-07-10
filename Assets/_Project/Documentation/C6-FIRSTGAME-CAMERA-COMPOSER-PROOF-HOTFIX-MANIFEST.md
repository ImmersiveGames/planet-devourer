# C6 — FIRSTGAME CameraComposer Proof Hotfix Manifest

Status: hotfix
Date: 2026-07-09
Scope: `Assets/_Project`

## Objective

Fix the FIRSTGAME CameraComposer proof helper so it does not require a compile-time reference to the framework editor namespace from the consumer project.

## Problem

The first C6 proof helper imported:

```text
Immersive.Framework.Editor.CameraAuthoring
```

In FIRSTGAME, that namespace was not visible to the consumer editor assembly, causing:

```text
CS0234: The type or namespace name 'Editor' does not exist in the namespace 'Immersive.Framework'
```

## Fix

The proof helper now resolves `CameraComposerApplyRebuildUtility` at editor runtime from loaded assemblies and invokes its public `Validate` and `ApplyOrRebuild` methods.

This keeps the proof using the official package editor utility without making FIRSTGAME depend on the framework editor namespace at compile time.

## Files changed

```text
Assets/_Project/Scripts/Editor/Camera/FirstGameCameraComposerProof.cs
```

## Files created

```text
Assets/_Project/Documentation/C6-FIRSTGAME-CAMERA-COMPOSER-PROOF-HOTFIX-MANIFEST.md
```

## Out of scope

```text
Package official changes
QAFramework changes
Runtime behavior
CameraManager / singleton / service locator
Camera.main fallback
```

## Expected validation

Unity compiles, then run:

```text
FIRSTGAME > Immersive Framework > Camera Composer Proof > Configure Gameplay CameraComposer Proof
```

Expected proof log:

```text
[FIRSTGAME][CameraComposerProof] status='Succeeded' ... resolvedByName='False'
```

## Commit message

```text
FirstGame: fix CameraComposer proof editor boundary
```
