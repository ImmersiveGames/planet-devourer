# Player Demonstration Program — Reorganization Changeset

**Date:** 2026-08-06  
**Type:** Documentation / roadmap organization  
**Runtime changes:** None  
**Package changes:** None  
**QA changes:** None  
**FIRSTGAME scene or asset changes:** None

## Objective

Replace the ambiguous global `PLAYER-Dxx` demonstration sequence with identifiers that state which FIRSTGAME demo owns each proof.

## Canonical structure

```text
Demo 02 — Player source and physical lifetime
Demo 03 — Local multiplayer foundations
Demo 04 — Participation and Actor-policy scenarios
IF-PLAYER-Pxx — Package/product evolution
```

## Mapping

```text
PLAYER-D01 → DEMO02-MODEL-02
PLAYER-D02 → DEMO02-MODEL-03
PLAYER-D03 → DEMO03-MULTI-01
PLAYER-D04 → DEMO03-MULTI-02
PLAYER-D05 → DEMO04-SCENARIO-01
PLAYER-D06 → DEMO04-SCENARIO-02
PLAYER-D07 → DEMO02-MODEL-04
PLAYER-Pxx → IF-PLAYER-Pxx
```

`DEMO02-MODEL-01` is the existing Route-owned Scene-Provided Player proof formerly tracked as M06.

## Status after reorganization

```text
Demo 02 implementation proofs: closed
Demo 02 comparison documentation: refresh pending
Demo 03 first cut: DEMO03-MULTI-01
Demo 04: planned after Demo 03
```

## Suggested documentation commit

```text
docs: reorganize Player demonstrations by FIRSTGAME demo
```
