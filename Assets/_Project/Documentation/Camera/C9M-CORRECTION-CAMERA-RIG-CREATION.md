# C9M Correction — Camera Rig Creation

After applying package cut C9N, restart the manual C9M proof from a clean scene state.

## Delete the incorrect materialization

Remove the temporary camera hierarchy created during the failed proof.

Do not delete the existing canonical Player or gameplay objects.

## Recreate output manually

```text
Gameplay Camera
├── Unity Camera
├── CinemachineBrain
└── CameraOutputSessionBinding
```

## Recreate Player Rig manually

Create:

```text
Player Rig
└── CameraRigComposer
```

Configure the PlayerComposer target source and run Apply/Rebuild.

Expected result:

```text
Player Rig
├── CameraRigComposer
└── Cinemachine Camera
```

Unexpected and blocking:

```text
Unity Camera under Player Rig
CinemachineBrain under Player Rig
AudioListener under Player Rig
```

## Expected first Apply

The exact counters may vary because target assignments count as repairs, but object creation must be:

```text
one Cinemachine Camera
zero Unity Cameras
zero CinemachineBrains
```

## Expected second Apply

```text
created='0'
blocked='0'
```
