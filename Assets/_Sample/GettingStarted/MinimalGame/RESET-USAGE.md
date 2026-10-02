# Minimal Game — Reset Consumer Usage

Status: **consumer scenarios 1-8 manually validated through 2026-10-02; final Phase 8 asset-layout commit still needs reconciliation with the latest manual configuration**
UPM promotion: **not implied by this proof**

This guide records how Reset is consumed in the Minimal Game authoring/proving workspace. Framework semantics are defined by IF-ADR-035 and the Framework `Reset-Usage.md`; this document is the consumer demonstration.

## What this sample teaches

The Reset slice is deliberately incremental:

| Phase | Scenario | Contract demonstrated | Validation |
|---|---|---|---|
| 1 | Object Direct | Direct typed Resettable targeting | PASS |
| 2 | Object Stable | Stable cross-scene ObjectEntry targeting | PASS |
| 3 | Composition Direct / Descendants | Structural set resolution | PASS |
| 4 | Composition Stable / Explicit Members | Stable composition lookup + explicit members | PASS |
| 5 | CurrentActivity | Activity membership selection | PASS |
| 6 | CurrentRoute | Route umbrella selection, including current Activity context | PASS |
| 7 | Activity Restart | Reset surviving state before Clear/Reenter | PASS |
| 8 | Multiple Participants | One Resettable executing multiple capabilities | PASS manually; asset-layout reconciliation pending |

Direct local triggers are sufficient where the scenario is about target semantics. Shared UI is used where cross-scene/stable/current-scope/restart behavior is the concept being demonstrated.

## Core authoring model

```text
Resettable
  executable Reset subject / hierarchy boundary

Reset capability
  restores one kind of local state

ResetComposition
  resolves a set of Resettables
  is not a Reset subject

ResetRequestTrigger
  requests Object / Composition / CurrentActivity / CurrentRoute

ActivityRestartTrigger
  orchestrates Reset -> Clear -> Reenter
```

Ownership and membership are separate. The Visitors content is useful because it is Route-owned and can still opt into Activity Reset membership when the scenario requires surviving state to reset with the Activity.

## Phase 1 — Object Direct

Gameplay object:

```text
NPC - Object Direct
  Resettable
  UnityTransformResetParticipant
```

Local trigger:

```text
Trigger - Object Direct
  ResetRequestTrigger
    Target = Object
    Reference = Direct
    Resettable = NPC - Object Direct
```

Validation: move the NPC, request Reset, confirm Transform returns to baseline.

## Phase 2 — Object Stable

Gameplay object:

```text
NPC - Object Stable
  ObjectEntryDeclaration
  Resettable
  UnityTransformResetParticipant
```

The shared Reset UI addresses it through Object / Stable with the Route owner selector. The stable reference is cross-scene identity; it is not the runtime Reset subject ID.

Validation: move the NPC, request the stable Object Reset, confirm the current physical occurrence returns to baseline.

## Phase 3 — Composition Direct / Descendants

Conceptual hierarchy:

```text
Composition - Direct Descendants
  ResetComposition
    Member Mode = Descendants

  NPC A - Composition Descendant
    Resettable
    UnityTransformResetParticipant

  NPC B - Composition Descendant
    Resettable
    UnityTransformResetParticipant
```

The local request targets the `ResetComposition` directly.

Validation: move both member NPCs individually and confirm one request restores both.

## Phase 4 — Composition Stable / Explicit Members

Conceptual authoring:

```text
Composition - Stable Explicit Members
  ObjectEntryDeclaration
  ResetComposition
    Member Mode = Explicit Members
    Members:
      NPC A - Composition Explicit
      NPC B - Composition Explicit
```

Each member is an independent Resettable. The shared UI resolves the composition through the generic stable ObjectEntry boundary and then resolves its current typed members.

Validation: move both explicit members and confirm both restore.

## Phase 5 — CurrentActivity

```text
NPC - Current Activity
  Resettable
    Membership = Activity
  UnityTransformResetParticipant
```

Shared UI:

```text
ResetRequestTrigger
  Target = CurrentActivity
```

`CurrentActivity` selects effective Activity membership in the current context and excludes Route membership.

## Phase 6 — CurrentRoute

```text
NPC - Current Route
  Resettable
    Membership = Route
  UnityTransformResetParticipant
```

Shared UI:

```text
ResetRequestTrigger
  Target = CurrentRoute
```

The validated semantic is `Activity ⊂ Route`. CurrentRoute is the Route umbrella: it includes current-Route Route membership and the applicable Activity-membership subjects in the current Route/current Activity context, while excluding unrelated contexts.

## Phase 7 — Activity Restart

Surviving state:

```text
NPC - Activity Restart Surviving State
  Route-owned content
  Resettable
    Membership = Activity
  UnityTransformResetParticipant
```

UI uses `ActivityRestartTrigger`, not a plain ResetRequestTrigger:

```text
Target Activity = None
Use Current When Target Missing = true
Require Target Is Current = true
Reset Target = CurrentActivity
```

Validated sequence:

```text
Reset surviving Route-owned / Activity-membership state
  -> Activity Clear
  -> Activity Reenter
```

The successful run restored both the CurrentActivity NPC and the surviving Route-owned NPC before Clear/Reenter.

## Phase 8 — Multiple Participants

The final intended teaching hierarchy is:

```text
Multiple Participants
  Resettable
    Membership = Route

  ├─ NPC - Transform Participant
  │    UnityTransformResetParticipant
  │      Target = self Transform
  │      Requiredness = Required
  │      Order = 0
  │
  └─ NPC - Active Participant
       UnityGameObjectActiveResetParticipant
         Target = self GameObject
         Requiredness = Required
         Order = 10
```

The Resettable belongs on the root. The two capabilities belong on different child NPCs inside the same Resettable hierarchy boundary. Neither child needs another Resettable.

Test:

1. Move `NPC - Transform Participant`.
2. Disable `NPC - Active Participant`.
3. Send one Object / Direct request to the root Resettable.
4. Confirm the first NPC returns to its Transform baseline.
5. Confirm the second NPC becomes active again.

This demonstrates one runtime Reset subject executing two participants that restore different kinds of state on different GameObjects.

The manual run produced successful immediate verification for both Transform and GameObject active-state restoration. The repository's current `Reset Compositions` asset snapshot predates the final two-child Phase 8 layout, so the next Unity asset commit must reconcile the physical prefab with this validated configuration before this phase is considered repository-integrated.

## Shared Reset UI

The committed ActivityRestart scene currently contains Reset controls equivalent to:

```text
btn_ResetObject
  Object / Stable

btn_ResetCompositionStable
  Composition / Stable

btn_ResetCurrentActivity
  CurrentActivity

btn_ResetCurrentRoute
  CurrentRoute

btn_RestartActivity
  ActivityRestartTrigger
```

Direct scenarios can remain local because the teaching goal is direct targeting, not UI composition.

## Evidence rule

Do not mark a phase validated only because the Inspector looks correct.

For each scenario distinguish:

```text
AUTHORING
TRIGGER
UI (when relevant)
RUNTIME EXECUTION
VISUAL RESULT
LOG EVIDENCE
INTEGRATED
VALIDATED
```

For participants with focused immediate-verification diagnostics, keep `verificationSucceeded=True` as runtime evidence. For aggregate/lifecycle scenarios, also retain the terminal Reset/Restart result that proves selected subject/participant counts and Clear/Reenter status when applicable.

## Related Framework documentation

- `Documentation~/Guides/Reset-Usage.md`
- `Documentation~/API/Public-API.md`
- `Documentation~/Architecture/ADRs/IF-ADR-035-Reset-Composition-Ownership-Membership-and-Targeting.md`
