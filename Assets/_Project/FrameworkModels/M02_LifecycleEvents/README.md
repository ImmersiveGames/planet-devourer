# M02 — Lifecycle Events

Status: Closed — FIRSTGAME proof passed  
Roadmap order: 2  
Started: 2026-07-30  
Closed in FIRSTGAME: 2026-07-30  
QAFramework regression: Deferred by roadmap decision

## Purpose

Demonstrar que um usuário do framework consegue autorar objetos que reagem ao lifecycle oficial de Scene,
Route e Activity por Inspector, sem criar autoridade paralela no jogo consumidor.

O M02 é diferente do M01:

```text
M01
  prova que Route e Activity transitam corretamente.

M02
  prova que objetos autorados pelo consumidor recebem e apresentam os callbacks reais.
```

## Closure status

```text
Package authoring surface       Passed
FIRSTGAME manual authoring      Passed
FIRSTGAME Play Mode smoke       Passed
Route A → Route B → Route A     Passed
Activity idempotence            Passed
QAFramework deterministic QA    Deferred
```

O adiamento de QA não invalida a prova de produto do FIRSTGAME. Ele apenas mantém os casos sintéticos e
negativos fora deste corte.

## Official surfaces used

```text
Scene
  SceneLifecycleEvents
  → SceneLifecycleRuntime
  → Available / Releasing UnityEvents

Route
  RouteContentBinding
  + RouteContentLifecycleEvents
  → Entered / Exited UnityEvents

Activity
  ActivityLocalVisibilityAdapter
  + ActivityContentLifecycleEvents
  → Entered / Exited UnityEvents
```

`SceneLifecycleEvents` foi adicionado ao package como ponte pública de Inspector. Route e Activity reutilizam
as superfícies oficiais que já existiam.

## Consumer presentation

```text
FirstGame.FrameworkModels.M02.M02LifecycleVisualPresenter
```

Responsabilidades permitidas:

```text
atualizar Label;
atualizar Visual Placeholder;
armazenar o último evento;
manter contador local de apresentação;
emitir log estruturado [M02_LIFECYCLE].
```

O presenter não resolve Route ou Activity, não observa `SceneManager`, não usa polling e não decide lifecycle.

## Authoring hierarchy

Os componentes técnicos podem ficar no root ou em descendentes dos roots explícitos da cena. A hierarquia é
organização do usuário, não identidade runtime.

```text
PF_M02_SceneLifecycleObject
├── Visual Placeholder
├── Label
├── Framework Components (Configure Manually)
│   └── SceneLifecycleEvents
└── Bindings (Configure Manually)
    └── M02LifecycleVisualPresenter

PF_M02_RouteLifecycleObject
├── Visual Placeholder
├── Label
├── Framework Components (Configure Manually)
│   ├── RouteContentBinding
│   └── RouteContentLifecycleEvents
└── Bindings (Configure Manually)
    └── M02LifecycleVisualPresenter

PF_M02_ActivityLifecycleObject
├── Visual Placeholder
├── Label
├── Framework Components (Configure Manually)
│   ├── ActivityLocalVisibilityAdapter
│   └── ActivityContentLifecycleEvents
└── Bindings (Configure Manually)
    └── M02LifecycleVisualPresenter
```

A descoberta parte dos roots explícitos entregues pelo runtime e atravessa descendentes, inclusive inativos.
Não existe busca global. Route/Activity assets e `localContentId` continuam explícitos e preservam as
fronteiras de escopo.

## Per-instance assignments

O prefab-base permanece reutilizável. A identidade específica é atribuída como override da instância:

```text
M02_RouteA
  PF_M02_RouteLifecycleObject.Route = Route_M02_A

M02_RouteB
  PF_M02_RouteLifecycleObject.Route = Route_M02_B

M02_ActivityA_Add
  PF_M02_ActivityLifecycleObject.Activity = Activity_M02_A

M02_ActivityB_Add
  PF_M02_ActivityLifecycleObject.Activity = Activity_M02_B
```

Não aplicar essas referências ao prefab-base.

## Startup semantics

```text
Build Profile index 0
  M02_Boot

Game Application
  Startup Route = Route_M02_A
  Content Scene = M02_PersistentContent

Route_M02_A
  Primary Scene = M02_RouteA
  First Activity = Activity_M02_A
```

O M02 possui zero Local Player Slots e não reutiliza assets do M01.

## Inventory

```text
M02_LifecycleEvents/
├── Application/GA_M02_Lifecycle.asset
├── Routes/
│   ├── Route_M02_A.asset
│   └── Route_M02_B.asset
├── Activities/
│   ├── Activity_M02_A.asset
│   └── Activity_M02_B.asset
├── Profiles/
│   ├── ActivityContent_M02_A.asset
│   └── ActivityContent_M02_B.asset
├── Scenes/
│   ├── M02_PersistentContent.unity
│   ├── M02_Boot.unity
│   ├── M02_RouteA.unity
│   ├── M02_RouteB.unity
│   ├── M02_ActivityA_Add.unity
│   └── M02_ActivityB_Add.unity
└── Prefabs/
    ├── PF_M02_SceneLifecycleObject.prefab
    ├── PF_M02_RouteLifecycleObject.prefab
    └── PF_M02_ActivityLifecycleObject.prefab
```

## Validated Play Mode flow

```text
M02_Boot
→ Route A + Activity A
→ request Activity A again: IgnoredAlreadyActive
→ Activity B
→ Route B + Activity B
→ request Activity B again: IgnoredAlreadyActive
→ Route A + Activity A
```

Observed consumer events:

```text
Boot
  Route A: Scene Available
  Route A: Entered
  Activity A: Entered

Activity A → B
  Activity A: Exited
  Activity B: Entered

Route A → B
  Activity B: Exited
  Route A: Exited
  Route A: Scene Releasing
  Route B: Scene Available
  Route B: Entered
  Activity B: Entered

Route B → A
  Activity B: Exited
  Route B: Exited
  Route B: Scene Releasing
  Route A: Scene Available
  Route A: Entered
  Activity A: Entered
```

Runtime evidence:

```text
routeContentEnterBindings = 1
routeContentEnterReceivers = 1
routeContentExitBindings = 1
routeContentExitReceivers = 1
activityContentEnterBindings = 1
activityContentEnterReceivers = 1
activityContentExitBindings = 1
activityContentExitReceivers = 1
failed = 0
blockingIssues = 0
activitySceneLedgerStale = 0
```

Requests para a Activity já ativa produzem `IgnoredAlreadyActive`, zero side effects e nenhum novo log de
Enter/Exit do presenter.

## Product findings

### UX-M02-001 — M01 navigation names are model-specific

Os prefabs visuais de navegação do M01 puderam ser reutilizados, mas seus nomes ainda são específicos do M01.
Destino futuro: `FIRSTGAME Shared/Navigation` ou template oficial quando o shape amadurecer.

### UX-M02-002 — Scene lifecycle needed a public Inspector bridge

A superfície pública `SceneLifecycleEvents` foi criada no package e validada no FIRSTGAME.

### UX-M02-003 — Prefab restructuring can stale scene overrides

Mover `RouteContentBinding` para outro GameObject alterou seu `fileID`. As cenas mantiveram overrides apontando
para o componente removido, produzindo zero bindings apesar de a descoberta hierárquica estar correta.

Regra prática:

```text
após mover ou recriar componentes dentro de prefab,
revisar overrides de instância e referências serializadas.
```

Uma validação futura pode detectar overrides órfãos, mas isso não bloqueia o modelo atual.

### UX-M02-004 — Initial presentation is not a framework event

O presenter atual também registra o estado visual inicial. Isso faz o primeiro callback real aparecer com
contador maior que um. A limpeza é local e não bloqueante; não exige alteração no package.

## Deferred QA follow-up

```text
QA-M02-001
Feature: Activity lifecycle authoring
Contract: repeated request for the active Activity does not duplicate Enter/Exit
Status: Deferred by roadmap decision
Destination: QAFramework
```

Casos de falha de receiver, required/optional e ordem sintética permanecem fora do FIRSTGAME e poderão ser
formalizados no QAFramework em corte posterior.

## Closure criteria

- [x] Usuário consegue criar e organizar os três prefabs de lifecycle.
- [x] Usuário consegue atribuir Route e Activity por override de instância.
- [x] UnityEvents são explícitos e compreensíveis no Inspector.
- [x] Scene Available/Releasing chegam ao presenter.
- [x] Route Enter/Exit chegam ao presenter.
- [x] Activity Enter/Exit chegam ao presenter.
- [x] Reentrada Route B → Route A restaura Activity A.
- [x] Requests `IgnoredAlreadyActive` não duplicam callbacks.
- [x] Logs `[M02_LIFECYCLE]` comprovam a cadeia até o consumidor.
- [x] `blockingIssues = 0`, `failed = 0` e ledger sem stale.
- [ ] Regressão sintética no QAFramework — deferred, não bloqueante para o roadmap atual.

## Next model

```text
M03 — Activity Readiness
```

O próximo modelo deve provar required/optional readiness e a experiência de diagnóstico correspondente, sem
reabrir o M02.
