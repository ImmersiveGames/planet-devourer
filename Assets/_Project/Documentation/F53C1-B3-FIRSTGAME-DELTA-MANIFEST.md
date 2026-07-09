# F53C1-B3 - FIRSTGAME Menu Consolidation Delta Manifest

Status: Ready for Unity import
Scope: `Assets/_Project`
Date: 2026-07-09

## Objective

Consolidate the FIRSTGAME camera setup editor tool under the same top-level menu used by the player binding validation tools.

## Changed files

```text
Assets/_Project/Scripts/Editor/GameCamera/FirstGameCameraCutSetup.cs
```

## Created files

```text
Assets/_Project/Documentation/F53C1-B3-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53C1-B3-FIRSTGAME-DELTA-MANIFEST.md.meta
```

## Removed files

```text
None
```

## Functional change

Moved the camera setup menu from:

```text
Tools > FIRSTGAME > Camera > Configure Route-Activity Camera
```

to:

```text
FIRSTGAME > Immersive Framework > Configure Route-Activity Camera
```

## Out of scope

```text
No runtime change.
No gameplay/movement/input routing change.
No player identity logic change.
No scene or prefab change.
No package change.
```

## Smoke

1. Compile Unity.
2. Confirm this menu exists:

```text
FIRSTGAME > Immersive Framework > Configure Route-Activity Camera
```

3. Run it from `FG_Gameplay` / regular project context.
4. Expected log:

```text
[FIRSTGAME_CAMERA_SETUP] Canonical FIRSTGAME player resolved. resolvedByName='False'
[FIRSTGAME_CAMERA_SETUP] Route/Activity camera setup configured.
```

5. Run:

```text
FIRSTGAME > Immersive Framework > Validate Real Player Binding
```

Expected result remains `status='Succeeded'` and `resolvedByName='False'`.

## Suggested commit

```text
F53C1-B3: consolidate FIRSTGAME framework editor menus
```
