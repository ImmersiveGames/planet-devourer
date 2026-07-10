# C8A — FIRSTGAME Camera Legacy Transition Manifest

Status: FIRSTGAME guidance-only transition

## Current FIRSTGAME product proof

The supported consumer proof is:

```text
FIRSTGAME > Immersive Framework > Camera Composer Proof > Configure Gameplay CameraComposer Proof
```

It proves the official `CameraComposer` with the real `PlayerComposer`, explicit `CameraTarget` / `LookAtTarget`, Cinemachine materialization and idempotent Apply/Rebuild.

## Legacy diagnostic path

`FIRSTGAME > Immersive Framework > Configure Route-Activity Camera` and `FirstGameCameraCutSetup.cs` remain available only for Route/Activity lifecycle compatibility diagnostics. They are not the main Camera Product Surface workflow.

The legacy path uses `FrameworkCameraDirector`, Route/Activity bindings and `FrameworkCameraAnchorHost`; it must not be presented as a replacement for the official CameraComposer proof.

## C8A boundary

- Preserve the C6 CameraComposer proof.
- Do not remove the legacy setup while Route/Activity compatibility still depends on it.
- Do not introduce `Camera.main`, global lookup, singleton or CameraManager behavior.
- Defer runtime Route/Activity rewrite to a separately specified C8B cut.

## Validation

Run the CameraComposer proof as the product validation. Run `Configure Route-Activity Camera` only when explicitly testing legacy lifecycle compatibility.

## Suggested commit message

`FirstGame: demote legacy camera setup flow`
