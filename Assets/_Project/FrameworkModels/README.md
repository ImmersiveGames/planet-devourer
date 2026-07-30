# FIRSTGAME Framework Models

Status: M01 authoring active  
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

`F0 Folder Architecture` is closed. `M01_RouteActivity` has entered `Authoring`.

The current change creates only the internal folders and README required to start M01. It does not create scenes, prefabs, ScriptableObject assets, scripts, bootstrap code, or runtime behavior.

Current roadmap checkpoint: `M01 — Etapa 1: Game Application`.
