# FIRSTGAME — Persistent Camera Authority

The main camera output is authored in `FG_UIGlobal`, not `FG_Gameplay`.
Run **Immersive Framework/FIRSTGAME/Install Persistent Camera Authority** once
in Unity to materialize the persistent output and migrate the gameplay scene.

Expected flow:

```text
Player 50
Activity override 100
Route override 200
Session transition override 300
```

The Session override is requested only after the fade/loading cover is closed
and released before it opens. During normal gameplay the Player request wins.

The in-game `Camera Override Authority` panel exposes explicit Activity, Route
and Session request/release actions. It is diagnostic tooling, not gameplay
authority.
