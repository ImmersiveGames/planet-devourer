# Player Shared

Status: **CONDITIONAL — NOT A REQUIRED PLAYER LAYER**

`Player/Shared` exists only as a valid promotion scope when **two or more concrete Player Demonstration Applications actually reuse the same content**.

Canonical rule:

```text
no concrete cross-application reuse
  -> keep content local
  -> do not establish Player/Shared ownership

concrete reuse exists
  -> promote only the reusable content
```

Application/session-specific authority always stays local to the owning Demonstration Application.

An existing placeholder/scaffold folder does not establish an architectural requirement to populate or preserve a Player Shared layer.
