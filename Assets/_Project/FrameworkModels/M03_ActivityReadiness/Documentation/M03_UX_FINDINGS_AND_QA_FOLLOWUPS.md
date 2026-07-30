# M03 UX Findings and QA Follow-ups

## FIRSTGAME Result

```text
Model: M03 Activity Readiness
Type: UX/product + real integration
Status: Passed
Flow: Enter → Waiting → Ready → Exit → Intermission → Re-enter → Waiting → Ready
```

## UX Findings

| ID | Area | Observation | Impact | Destination |
|---|---|---|---|---|
| UX-M03-001 | Creation | The recurring setup requires participant, sequence, events, presenter and several UnityEvent bindings. Local explicit tools reduced errors, but the package has no official designer-first composer for this repeated shape. | Medium | Package / Template |
| UX-M03-002 | Inspector | `Requiredness` is visible, but the relation between participant state and aggregate Activity readiness is not immediately explained. | Medium | Package / Docs |
| UX-M03-003 | Composition | The presenter-to-prepared-content reference is a concrete scene-instance binding and cannot be understood from the prefab alone. | Medium | Docs / Composer |
| UX-M03-004 | Runtime | The happy path and re-entry work without polling, global lookup or consumer-owned readiness authority. | Positive | FIRSTGAME evidence |
| UX-M03-005 | Diagnostics | Boot diagnostics can show `BaselineReady` and aggregate `NotReady` with similar field names, which can look contradictory. | Medium | Package |
| UX-M03-006 | Diagnostics | Expected authorable `NotReady` can be counted as a failed lifecycle stage even when the Activity request is `Succeeded`. | Medium | Package |
| UX-M03-007 | Reuse | `ActivityReadinessParticipant` and `ActivityReadinessEvents` are reusable framework surfaces; the sequence and presenter are consumer examples. | Positive | Sample / Docs |
| UX-M03-008 | Re-entry | Route-scoped navigation is necessary because Activity-owned UI is unloaded during Activity replacement. | Low | Docs / Template |

## Product Direction

A future official surface could follow:

```text
Activity Readiness Recipe/Profile
→ Activity Readiness Composer
→ explicit participant materialization
→ official runtime authority
→ embedded diagnostics
```

It should preserve:

- explicit participant identity;
- visible Required/Optional semantics;
- explicit UnityEvent or typed callback surfaces;
- Advanced/Debug access to materialized components;
- no silent fallback;
- no hidden global manager.

## QA Follow-ups

These scenarios must be implemented in `QAFramework`, not in FIRSTGAME.

### QA-M03-001 — Required Participant Failure

```text
Source Model: M03 Activity Readiness
Feature: Required authorable readiness participant
Contract: A failed required participant keeps aggregate readiness NotReady
Scenario: Required participant calls FailPreparation with an explicit reason
Expected Result: Aggregate remains NotReady and publishes a stable diagnostic reason
Risk: Activity becomes usable after required preparation failed
Suggested QA Fixture: Synthetic required participant with controlled failure
Priority: High
```

### QA-M03-002 — Optional Participant Failure

```text
Source Model: M03 Activity Readiness
Feature: Optional authorable readiness participant
Contract: Optional failure does not block aggregate Ready
Scenario: Optional participant fails while all required participants complete
Expected Result: Activity becomes Ready with explicit optional failure diagnostics
Risk: Optional dependency accidentally blocks gameplay
Suggested QA Fixture: Required success plus optional controlled failure
Priority: High
```

### QA-M03-003 — Invalid Participant Authoring

```text
Source Model: M03 Activity Readiness
Feature: Participant discovery validation
Contract: Participant Id and Requiredness must be valid
Scenario: Empty Participant Id or Unknown Requiredness
Expected Result: Explicit discovery issue; no silent fallback
Risk: Misconfigured dependency is ignored
Suggested QA Fixture: Invalid participant variants
Priority: High
```

### QA-M03-004 — Duplicate Completion

```text
Source Model: M03 Activity Readiness
Feature: Participant terminal state
Contract: Identical or repeated completion does not publish a second semantic transition
Scenario: CompletePreparation is called twice
Expected Result: First call completes; second call is rejected or ignored with diagnostics
Risk: Duplicate Ready updates and presentation callbacks
Suggested QA Fixture: Controlled participant invocation counter
Priority: Medium
```

### QA-M03-005 — Late Completion After Release

```text
Source Model: M03 Activity Readiness
Feature: Occurrence lifetime
Contract: A released participant cannot complete a previous occurrence
Scenario: Activity exits while preparation is running, then the old callback completes
Expected Result: Late completion is rejected and cannot change current readiness
Risk: Stale asynchronous work corrupts the new Activity
Suggested QA Fixture: Deferred completion handle
Priority: High
```

### QA-M03-006 — Stale Occurrence After Re-entry

```text
Source Model: M03 Activity Readiness
Feature: Readiness occurrence identity
Contract: Updates are scoped to the current Activity occurrence
Scenario: Exit and re-enter the same Activity, then publish an update from occurrence 1
Expected Result: Occurrence 2 remains authoritative
Risk: Re-entry state is overwritten by stale updates
Suggested QA Fixture: Two controlled occurrences
Priority: High
```

### QA-M03-007 — Multiple Required and Optional Participants

```text
Source Model: M03 Activity Readiness
Feature: Aggregate readiness
Contract: All required participants must complete; optional participants do not block
Scenario: Multiple participants finish in different orders
Expected Result: Stable aggregate counts, reasons and revisions
Risk: Order-dependent readiness
Suggested QA Fixture: Deterministic participant matrix
Priority: Medium
```

### QA-M03-008 — Presentation Revision Deduplication

```text
Source Model: M03 Activity Readiness
Feature: ActivityReadinessEvents
Contract: An identical revision is not presented twice
Scenario: Publish the same snapshot revision repeatedly
Expected Result: One presentation callback
Risk: Duplicate UI reactions
Suggested QA Fixture: Official presentation bridge harness
Priority: Medium
```

## Out of Scope for FIRSTGAME

```text
fault injection
timeouts
stale occurrence simulation
duplicate completion buttons
assertion panels
stress loops
synthetic optional/required matrices
```

FIRSTGAME remains the real-use happy path. QA owns synthetic negative and regression coverage.
