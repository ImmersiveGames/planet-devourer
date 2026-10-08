# Advanced Context Showcase

Status: **Planning placeholder — no runnable Showcase composition is authored here.**

This folder does not currently teach a consumer setup. The earlier proposal for
runtime switching between Camera Presentation/Request assets describes a
superseded model and must not be used as an implementation guide.

If this gap is materialized, use only current Framework surfaces and follow the
already integrated Game Flow example in `Assets/_Sample/GameFlow/GameFlowShowcase`:

```text
GameApplication startup Session Camera Assignment
  -> explicit Session Camera Assignment command port for runtime changes
  -> explicit Assignment / Output / Subject configuration
  -> Route-scoped consumer may observe committed Activity transitions
```

The Session owns Assignment activation, replacement, clear and Output routing.
Route/Activity transition may change Subject eligibility; Route and Activity do
not own Camera selection. Any sample must name its exact Player, Assignment,
Output and rig assets before it is described as runnable.

Optional Audio/provider scenarios may coexist here only when their dependency
boundaries are explicit. The Game Flow README remains the working example for the
current command and observer contracts.
