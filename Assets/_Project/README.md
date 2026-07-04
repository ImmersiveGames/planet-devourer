# FIRSTGAME Project Assets

Este projeto prova que o Immersive Framework e utilizavel para comecar um jogo real minimo.

Root canonico:

```text
Assets/_Project/
```

## Proposito

- manter um fluxo jogavel inicial;
- demonstrar menu, gameplay e UI global residentes;
- validar reset real, restart de activity, pause, transition/loading e objetos runtime;
- conter scripts, prefabs, materiais e assets simples de jogo.

FIRSTGAME nao e QA sintetico. Ele deve parecer um consumidor real pequeno, nao um laboratorio de smokes.

POST-RESET-B2: nomes temporarios como `Probe`, `Test`, `Smoke` e `QA_` nao devem permanecer no FIRSTGAME quando o arquivo ja representa exemplo final de jogo.

## Fluxo manual esperado

Use as cenas de `Assets/_Project/Scenes/`:

- `Menu/FG_Menu.unity`;
- `Menu/FG_UIGlobal.unity`;
- `Gameplay/FG_Gameplay.unity`.

Fluxo esperado:

1. iniciar pelo menu;
2. entrar em gameplay;
3. mover o player minimo;
4. spawnar runtime boxes;
5. usar Reset Room;
6. usar Restart Activity;
7. validar que a experiencia continua coerente depois do reset/restart.

## Componentes demonstrados

- player minimo;
- runtime object spawner;
- runtime box prefab;
- framework settings locais;
- routes e activities de FIRSTGAME;
- reset por `UnityResetSubjectAdapter` e `IUnityResettable`;
- transition/loading/pause reais quando configurados nas cenas.

## O que nao deve entrar neste projeto

- roots de QA sintetico;
- runners de smoke sintetico;
- objetos ou scripts `QA_*`;
- nomes temporarios como `Probe`, `Test` ou `Smoke` quando o arquivo ja representa comportamento final de jogo;
- documentacao historica/canonica do framework.

## Diferenca para QA

```text
QA prova que o framework funciona tecnicamente.
FIRSTGAME prova que o framework e usavel para comecar um jogo.
```

Renomes de scripts anexados em cenas/prefabs devem ser feitos por uma migracao via Unity Editor para evitar Missing Script.
