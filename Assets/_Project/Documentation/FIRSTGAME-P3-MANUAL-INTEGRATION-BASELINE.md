# FIRSTGAME P3 manual integration baseline

Status: Historical baseline  
Recorded before the current manual integration sequence  
Superseded as current status by: `FIRSTGAME-CURRENT-STATE.md`

## Purpose

This file preserves the clean consumer baseline that existed before the current Player, Pause and Camera compositions were authored.

At that baseline:

```text
FIRSTGAME hygiene was complete
canonical P3 integration was not yet authored
legacy setup and repair tooling had been removed
```

Those statements are historical and must not be read as the current repository state.

## Rules that remain active

Do not:

- use legacy setup scripts;
- restore removed components;
- copy QA fixtures;
- copy serialized assets from QA;
- create local facades for official contracts;
- infer Slot, Actor or Player identity from names or Unity player index;
- introduce silent discovery or repair to hide product friction.

## Current continuation

The current FIRSTGAME now contains Git-visible application, Persistent Content, Pause, Scene-Provided Player and Player Camera compositions.

Use:

```text
FIRSTGAME-CURRENT-STATE.md
TEST-SCENARIOS.md
PLAYER-VARIANTS.md
```

for the active development state and validation sequence.
