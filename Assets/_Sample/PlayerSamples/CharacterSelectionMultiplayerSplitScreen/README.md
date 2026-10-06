# Character Selection Multiplayer Split Screen

Status: **Implemented / Integrated / Manual Play Mode: PASS**

The sample provisions P1 and P2 through `PlayerSessionProfile_CharacterSelectionMultiplayerSplitScreen` and presents an independent Farmer/Cow selection UI for each joined Slot. Both Players use the same CharacterSelection Actor composition as the single-player sample: one `PlayerGameplayInputReader`, one `CharacterController`, `CharacterSelectionActorMovement` for planar movement and Actor rotation, and a neutral `FollowObservation` camera subject. Farmer and Cow remain visual-only. No mouse-look controls the camera.

## Camera assignment and Outputs

`CameraAssignment_CharacterSelectionMultiplayer` is `IndividualPerPlayer` with `ExplicitPlayerSlots`, `MemberActorTargets`, and one configured Output mapping per Slot:

- `PlayerSlotProfile_CharacterSelectionMultiplayer_P1` → `CameraOutput_CharacterSelection_P1`
- `PlayerSlotProfile_CharacterSelectionMultiplayer_P2` → `CameraOutput_CharacterSelection_P2`

Both mappings use `PF_Player_FixedFollow_Presentation` with the CharacterSelection Fixed Follow behavior. Each Output follows its bound Actor's neutral observation with the fixed high, distant, world-space offset. Actor rotation does not orbit the camera. Camera Session owns Assignment and Output lifecycle; `PlayerInputManager` owns split-screen viewport rectangles.

## Player-count and fallback behavior

- **0 Players:** exactly one physical Fallback Camera covers the view; the other mapped Output Camera is disabled.
- **1 Player:** only that Player's bound Output participates; an unbound Output cannot render over it.
- **2 Players:** P1 and P2 Outputs participate in split-screen.
- **Leave/Rejoin:** bindings and physical Output participation update on the same configured Outputs; at zero Players, one physical Fallback remains.

Assignment reservation and logical per-Output Fallback coverage remain intact throughout. Framework Camera code does not write `Camera.rect` or `Camera.pixelRect`.

## Manual validation

**PASS:** P1 and P2 each reach `GameplayReady`; each can explicitly select Farmer or Cow independently; movement translates and rotates each Actor; each camera follows its neutral Actor subject without orbiting on in-place rotation; 0 → 1 → 2 → 1 → 0 Players preserves one physical camera at zero and prevents unbound Outputs from overlapping active Players; Rejoin restores the mapped Output.

Automated tests and QA certification remain pending.
