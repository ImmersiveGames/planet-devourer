# Local Multiplayer

Status: **NEXT / PRE-IMPLEMENTATION PUBLIC-CONTRACT RE-AUDIT — 2026-09-05**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`, Revision 5.

Local Multiplayer is now the **next Player Demonstration Application work item** after Character Selection closure.

It is **not yet ready for prefab/sample construction**. The first step is to re-audit the current Framework public surface because the previous blocker was confirmed against an older Player implementation.

## Previous blocker

Last confirmed: **2026-08-26**.

The missing public boundary was:

```text
Slot / device / input ownership and observation contract
```

A normal consumer needed a canonical public way to author and observe the relationship among:

```text
local participant / device intent
Slot association
Player admission
input ownership / routing
effective device / control-scheme evidence
release / reuse when applicable
```

At that time, the ordinary `PlayerSessionJoinCommandTrigger` did not expose exact-Slot Join and did not by itself provide a complete durable Slot-to-device/InputUser/control-scheme observation contract.

## Why this is being re-audited

Character Selection and the Player sample prefab chain have since been rebuilt on the current Player Actor / Presentation architecture.

The Framework also received later Player cuts after the August blocker was recorded.

Therefore the August conclusion must **not** be copied forward as current truth without checking the current product surface.

Current documentation state is intentionally:

```text
previous blocker = recorded historical gate
current blocker state = not yet revalidated
next action = audit current public Framework contract
```

## Required audit questions

Before authoring Local Multiplayer, establish from the current public API/runtime:

```text
1. Who owns the local Player Slot?

2. How does a second local participant request Join?

3. How is a device / InputUser / control-scheme association established for a Slot?

4. How is gameplay input routed so each PlayerGameplayInputReader receives only its Player's input?

5. What public evidence allows the game/sample to observe the Slot <-> participant/device association?

6. How is that ownership released or reused on Leave / Rejoin?
```

## Exit criterion for implementation

Local Multiplayer may move from audit to construction only when the public product surface supports a canonical consumer path equivalent to:

```text
local participant / device intent
  -> deterministic Slot association
  -> Join / admission
  -> correct Player input ownership
  -> observable Slot / device / control-scheme state
  -> correct release / reuse lifecycle
```

without parallel sample-owned authority.

## Non-goals during the audit

Do not bridge a missing contract with:

```text
sample-owned Slot registry
sample-owned device ownership
sample-owned input router
hidden PlayerInput / InputUser discovery
reflection
private/internal Player runtime access
a second Session authority
silent fallback
```

Arbitrary Actor Selection may be reused later if the final multiplayer scenario needs it, but Actor selection is not the missing Local Multiplayer ownership boundary.

Split-screen is not implied by this README. Camera/output topology should be decided only after the local participant/Slot/input contract is known.

## Current sequencing

```text
Scene Player                 CLOSED / PROVEN
Player Provisioning          CLOSED / PROVEN
Character Selection          CLOSED / REPROVEN 2026-09-05
Local Multiplayer            <- CURRENT NEXT ITEM
  public-contract re-audit
  then canonical setup if unblocked
```
