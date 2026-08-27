# Local Multiplayer

Status: **PLANNED / BLOCKED — RECONFIRMED 2026-08-26**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`, Revision 3.

Local Multiplayer remains a planned Player Demonstration Application, but it is **not ready for implementation**.

The 2026-08-26 arbitrary Actor-selection closure does not remove this blocker.

Current missing public boundary:

```text
Slot / device / input ownership and observation contract
```

A normal consumer still needs a canonical public way to author and observe the relationship among:

```text
local participant / device intent
Slot association
Player admission
input ownership / routing
effective device / control-scheme evidence
release / reuse when applicable
```

The current ordinary `PlayerSessionJoinCommandTrigger` does not expose exact-Slot Join and does not by itself provide a complete durable Slot-to-device/InputUser/control-scheme observation contract.

Arbitrary Actor Selection may be reused later where appropriate, but it is not the missing Local Multiplayer authority.

Do not bridge this gap with parallel sample-owned Slot, device or input authority, hidden PlayerInput discovery or another Session registry.
