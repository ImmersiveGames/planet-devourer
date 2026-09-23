# Shared

Global sample content reused across multiple top-level UPM Sample Groups.

Promotion rule:

```text
used by one scene/feature
  -> keep local

used by multiple scenes in one Demonstration Application
  -> <Application>/Shared

used by multiple applications in one group
  -> <Group>/Shared

used by multiple groups
  -> Samples~/Shared
```

Do not hide authoritative application configuration here merely to deduplicate assets.
