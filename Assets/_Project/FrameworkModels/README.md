# FIRSTGAME Framework Models

Status: folder architecture ready  
Date: 2026-07-29

This directory contains independent, designer-readable demonstration models for the Immersive Framework.

## Architecture rules

- `Shared` contains only genuinely generic presentation pieces.
- Each `M##_FeatureName` root is independent from the other models.
- Internal folders are created only when that model enters `Authoring`.
- No model may depend on another model's assets.
- Framework contracts and reusable product solutions remain in `com.immersive.framework`.
- Negative and regression cases remain in `QAFramework`.

## Current cut

This cut creates folder roots only. It does not create scenes, prefabs, assets, scripts, bootstrap code, or runtime behavior.

Next implementation cut: `M02_LifecycleEvents`.
