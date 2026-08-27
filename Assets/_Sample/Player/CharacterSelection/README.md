# Character Selection

Status: **NEXT PLAYER CUT / PUBLIC SURFACE UNBLOCKED — 2026-08-26**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`, Revision 3.

Character Selection is ready for sample materialization. This status means the required public Player surface exists; it does **not** claim that the Character Selection application itself is already implemented or Play Mode proven.

## Canonical application intent

```text
HostProvisioning = ManagerProvisioned
ActorResolution = LeaveUnresolved
```

The initial flow is:

```text
Join
  -> Slot Joined
  -> Actor unresolved
        ↓
game-owned Character Selection UI
  -> presents eligible application-owned ActorProfile choices
        ↓
PlayerSessionSelectActorCommandTrigger.Invoke()
        ↓
PlayerActorSelectionResult
        ↓
Framework commits Actor selection
        ↓
existing Actor preparation / Manager-Provisioned materialization
        ↓
Activity participation / GameplayReady
```

## Public surfaces

Use only the official Player consumer surface:

```text
PlayerSessionObserver
  read-only Session / Slot / Actor evidence when presentation needs it

PlayerSessionJoinCommandTrigger
  ordinary Join

PlayerSessionSelectActorCommandTrigger
  explicit initial Actor choice
```

The broader public Actor-selection family also contains Default, Replace and Clear, but the normal Character Selection sample should demonstrate **initial Select** only unless a distinct scenario later justifies another operation.

## Ownership boundary

The sample/game owns:

```text
which ActorProfile choices are presented
labels / portraits / visual presentation
UI interaction
```

The Framework owns:

```text
Joined Slot validity
selection revision
selection commit
duplicate-selection policy
Actor preparation barrier
physical Actor materialization
Activity admission/readiness
```

Do not bridge the flow with private/internal runtime access, reflection, direct Session mutation, sample-specific Player discovery, parallel Actor-selection authority or hidden fallback.

## No Default fallback / no hot swap

With `ActorResolution = LeaveUnresolved`, normal flow must not invoke Default Actor selection. The Slot intentionally remains unresolved until the user chooses an Actor.

`Replace Actor Selection` is not physical Actor hot swap. Character Selection does not demonstrate replacement of an already prepared/admitted physical Actor.

## Command availability

A command can be authoring-valid while its scoped runtime access is not yet Bound. If the sample needs to gate interaction, use only public binding/readiness evidence. Do not hide `PLAYER-COMMAND-SURFACE-READINESS / DEFERRED` with global lookup or another Session authority.

## Completion gate for this sample

The sample becomes **PLAY MODE PROVEN** only after consumer validation confirms at minimum:

```text
Join succeeds with Actor unresolved
at least two meaningful Actor choices are presented
explicit Select commits the chosen Actor
existing preparation/materialization continues without sample-owned orchestration
Activity reaches the intended readiness / GameplayReady state
no Default fallback occurs
no internal/private Player API is required
```
