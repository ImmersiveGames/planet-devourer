# Character Selection

Status: **PLANNED / BLOCKED — 2026-08-22**

Canonical Player sample authority: `FG-ADR-002 — Player Sample Scope and Demonstration Architecture`.

Character Selection remains a planned Player Demonstration Application, but it is **not ready for implementation**.

Blocker:

```text
public arbitrary Actor-selection surface
```

The sample must wait until normal game-owned UI can observe the relevant Actor state, request an arbitrary supported Actor through an official public command, and observe the confirmed result.

Do not bridge this gap with private/internal runtime access, reflection, direct Session mutation or parallel sample-owned Actor-selection authority.
