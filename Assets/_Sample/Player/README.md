# Player Samples

This sample family demonstrates Player contracts that are not already covered
by the Getting Started / Minimal Game sample.

## Baseline

`Assets/_Sample/GettingStarted/MinimalGame/` is the canonical reference for
the minimal Scene-Provided Player flow.

It already demonstrates:

- Scene-Provided Local Player;
- Scene-Provided Logical Player;
- Player admission on Activity entry;
- logical Actor preparation;
- gameplay readiness;
- mounted first-person camera;
- minimal Move / Look gameplay input.

These contracts are intentionally not duplicated in the Player sample family.

## Player Demonstrations

The Player sample family is reserved for scenarios that introduce a distinct
Player contract beyond the Getting Started baseline.

Planned demonstrations include:

1. Manager-Provisioned Player;
2. Character Selection;
3. Local Multiplayer.

A new demonstration belongs here only when it adds a distinct Player behavior,
provisioning model, participation model, or authoring workflow.

Variants that belong to the same application should preferably be represented
as Routes, Activities, or scenarios instead of duplicating GameApplications.

## Shared Assets

Do not create a shared asset layer preemptively.

`Shared/` should only exist when two or more concrete Player demonstrations
genuinely reuse the same asset without introducing ownership or dependency
between otherwise independent samples.