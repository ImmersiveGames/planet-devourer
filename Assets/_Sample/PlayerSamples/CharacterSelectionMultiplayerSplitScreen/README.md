# Character Selection Multiplayer Split Screen

This sample uses two configured Camera Session Outputs and one Individual Session Camera Assignment. The Assignment is the only Player Slot to Output authority:

`Camera/CameraAssignment_CharacterSelectionMultiplayer.asset` is the reusable Session Camera Assignment asset referenced by the Game Application.

- `PlayerSlotProfile_CharacterSelectionMultiplayer_P1` → `CameraOutput_CharacterSelection_P1`
- `PlayerSlotProfile_CharacterSelectionMultiplayer_P2` → `CameraOutput_CharacterSelection_P2`

`CameraSessionConfiguration` contains only the two physical Output prefabs. The Assignment declares both Outputs and maps each member Slot to its Output. The Framework derives the exact `PlayerInput.camera` association from the Assignment and current Player Host evidence. Join, Leave, Rejoin and Host replacement reconcile that association; shutdown clears it.

## Ownership

The Framework chooses the Individual split-screen regime and associates each current PlayerInput with its configured Output Camera. `PlayerInputManager` owns viewport geometry. Framework Camera code does not write `Camera.rect` or `Camera.pixelRect`.

A SharedGroup Assignment uses one shared Output occurrence and creates no per-Player Output topology. It does not request split-screen. SessionScoped cameras also create no Player binding and can remain normal cameras with zero Players when their target policy allows it.

## Validation in Unity

After package import, validate in the sample scene:

1. P1 receives Output P1 and `PlayerInput.camera` references Output P1's Unity Camera.
2. P2 receives Output P2 and `PlayerInput.camera` references Output P2's Unity Camera.
3. Leave clears only the leaving Player's association; Rejoin uses the current Slot-to-Output mapping.
4. Actor replacement keeps the existing Assignment/Output association.
5. Leaving all Players and shutting down clears PlayerInput associations and releases the Session Outputs.
6. `PlayerInputManager` recomposes viewport geometry while Framework code leaves `Camera.rect` and `Camera.pixelRect` untouched.

Unity import, compile and Play Mode validation are pending for this Assignment-owned topology revision. Earlier sample certification covers its dated Presentation-era implementation only.
