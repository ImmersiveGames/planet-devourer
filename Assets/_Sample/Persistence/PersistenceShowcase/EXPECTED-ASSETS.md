# Expected Unity Assets

Materialize through Unity:

```text
GameApplication_Persistence.asset
technical HUB / Menu
Preferences Activity / Scene or equivalent panel composition
Progression Save Activity / Scene or equivalent panel composition
official Progression Save profile/configuration required by the Framework
minimal semantic/visual progression state
```

Canonical flows:

```text
Preferences
  change -> persist -> restart/re-enter -> restore

Progression Save
  modify -> save -> restart/re-enter -> load/restore -> observe
```

Not required initially:

```text
Snapshot
third-party backend swapping
cloud/platform provider matrices
```
