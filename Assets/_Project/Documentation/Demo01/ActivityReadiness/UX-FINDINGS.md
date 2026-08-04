# M03 Activity Readiness — UX Findings

**Date:** 2026-08-04  
**Feature cut:** `FIRSTGAME-M03-READY-PROGRESS-01`  
**Repository:** `ImmersiveGames/planet-devourer`  
**Documented Git baseline:** `a32e2ea8c620c22ae8072763490936eba4be952d`

## Outcome

The FIRSTGAME proof reached the intended runtime result:

```text
4 independent Required participants
1 Optional participant kept pending
determinate persistent Loading
100% only after aggregate Ready
successful reveal
successful Intermission exit
successful reentry with a new occurrence
```

The supplied Play Mode evidence showed successful occurrences `4` and `6`, each ending with:

```text
Required completed = 4
Required total = 4
Required pending = 0
Optional pending = 1
Loading progress = 100%
Activity Request = Succeeded
blockingIssues = 0
```

No missing public runtime contract was discovered during the final happy-path integration. The principal findings concern authoring safety, diagnostics clarity and reusable product tooling.

## Positive findings

### Policy authoring is understandable

The final Activity configuration is concise and communicates intent:

```text
WaitCovered
FadeWithLoading
InputInteractionAndGameplay
```

The distinction between `WaitVisible` and `WaitCovered` is understandable when both are presented in the same Route.

### Required and Optional semantics are teachable

Four Required participants visibly produce four progress increments. One Optional participant can remain pending without changing the denominator or blocking `Ready`.

### Runtime authority separation held

FIRSTGAME did not need to resolve the persistent Loading adapter, calculate the authoritative percentage, find a runtime host, or create a second Loading authority.

### Reentry behaved correctly

Intermission released the first occurrence and owned scene. Reentry created a new occurrence and repeated the four independent completions without stale progress or gate residue.

## Findings and disposition

| Id | Finding | Ownership | Severity | Disposition |
|---|---|---|---|---|
| `M03-UX-01` | An inactive `ActivityReadinessParticipant` is still discovered because the framework discovery scope includes inactive children. Inactive therefore does not mean excluded from readiness. | Package authoring/docs | High | Clarify explicitly in the usage guide and Advanced/Debug diagnostics. Consider authoring validation that reports captured inactive participants. |
| `M03-UX-02` | Repeated UnityEvent fields with identical component types made it easy to target the aggregate `ReadinessPreparationArea` instead of the participant-local area. Three participants initially called the wrong target. | Package authoring/template | High | A Recipe/Composer or validator should verify that each participant callback targets its expected local bridge and should surface the resolved target object. |
| `M03-UX-03` | A duplicate Participant Id survived manual authoring until static review. | Package validation | High | Add or strengthen duplicate-ID validation in the authoring flow and show all captured IDs in Advanced/Debug. |
| `M03-UX-04` | The participant composition was initially authored as scene overrides instead of in the reusable Prefab Variant. | FIRSTGAME workflow / package template | Medium | Keep the scene as one clean Variant instance. A future template should make prefab ownership and Apply/Rebuild intent explicit. |
| `M03-UX-05` | Adding `Wait Covered` required a separate manual update to the Intermission button visibility list. The Activity worked, but exit navigation was initially unreachable. | Package authoring/navigation | High | Add navigation reachability validation or a composer that updates related visibility rules idempotently. |
| `M03-UX-06` | The persistent Loading adapter supports determinate progress, but its progress references and hidden-state behavior require manual inspection and wiring. | Package sample/docs | Medium | Document the canonical progress-capable persistent Loading setup and provide an official reusable template when product shape is stable. |
| `M03-UX-07` | Applying prefab overrides during the integration can move unrelated changes into shared prefabs, as observed with incidental persistent presentation/camera edits during review. | FIRSTGAME change hygiene | Medium | Review prefab overrides per component before Apply and keep unrelated shared-prefab changes in separate commits. |
| `M03-UX-08` | Raw Game Flow diagnostics are comprehensive but dense. A user can prove the contract, but interpreting nested readiness, transition and loading fields requires technical familiarity. | Package diagnostics | Medium | Provide a compact Activity readiness summary in Advanced/Debug while preserving full evidence logs. |
| `M03-UX-09` | The local scenario presenter and persistent Loading both show progress-like visuals, which can blur authority boundaries. | FIRSTGAME presentation/docs | Low | Label the local panel as scenario explanation and state that only the framework-owned persistent Loading is authoritative. |
| `M03-UX-10` | Observe Only and Wait Covered reuse different participant shapes: one aggregate participant versus four independent participants. Reuse is possible, but the conversion is not obvious without documentation. | Package guide/template | Medium | Document when to use one aggregate participant and when multiple independent participants are needed for participant-granular Loading progress. |

## Routing by repository

### Remains in FIRSTGAME

```text
Chicken movement and targets
local explanatory panel
scenario visual layout
menu wording
prefab organization for this demonstration
```

### Should migrate to package authoring/product work

```text
participant Recipe/Composer or template
duplicate Participant Id validation
captured inactive participant diagnostics
callback-target validation
canonical progress-capable Loading template
navigation reachability validation
compact Advanced/Debug readiness summary
```

### Remains in QAFramework

```text
Required failure
Required release before completion
occurrence invalidation
cancellation
late old occurrence completion
duplicate terminal behavior
startup parity
```

No new negative QA requirement was discovered beyond the existing participant-aware readiness matrix.

### Package documentation closure

The package documentation cut should cover:

```text
when to use ObserveOnly
when to use WaitVisible
when to use WaitCovered
Fade versus FadeWithLoading
Required denominator
Optional diagnostics
aggregate versus independent participants
inactive participant discovery behavior
failure semantics
Advanced/Debug evidence
FIRSTGAME reference paths
```

This FIRSTGAME document does not replace the canonical package usage guide or ADR updates.

## Product assessment

### Technical acceptance

```text
PASS — direct WaitCovered request
PASS — 4 Required captured and completed
PASS — 1 Optional captured and pending
PASS — determinate Loading
PASS — 100% at aggregate Ready
PASS — successful Loading release and reveal
PASS — successful Intermission exit
PASS — clean second occurrence
PASS — no blocking issue in supplied evidence
```

### Product acceptance

```text
PASS — user can create and configure the feature manually
PASS — the three policies can be compared in one Route
PASS — Required participant completion is visible in Loading
PASS — Optional semantics are demonstrable
PASS — the scenario remains inspectable and reusable
PARTIAL — repeated manual wiring is still error-prone
PARTIAL — canonical package authoring tooling is not yet product-complete
```

## Closure decision

`FIRSTGAME-M03-READY-PROGRESS-01` can close as a real consumer proof.

This does not close package product completeness. Package-owned authoring findings must be resolved or explicitly deferred in the later package UX/documentation cuts.
