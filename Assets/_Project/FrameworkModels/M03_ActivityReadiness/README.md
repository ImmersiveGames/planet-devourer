# M03 Activity Readiness

Status: Closed  
Validation date: 2026-07-31

## Purpose

O M03 demonstra que uma Activity pode estar carregada e ativa, mas ainda não estar pronta para uso.

O caso visual do modelo é simples:

```text
Activity entra
→ um objeto inicia uma preparação visual
→ enquanto o objeto se move, a Activity fica NotReady
→ a preparação termina
→ o participant chama CompletePreparation()
→ o framework recompõe a readiness
→ a Activity fica Ready
```

O movimento não é a regra de readiness. Ele é apenas a representação visual de um trabalho de preparação.

## Status

```text
Package authoring surface       Passed
FIRSTGAME manual authoring      Passed
FIRSTGAME Play Mode review      Passed
FIRSTGAME exit and re-entry     Passed
Post-removal regression         Passed
Roadmap status                  Closed
Validation date                 2026-07-31
Negative QA coverage            Deferred to QAFramework
```

## Validation Record

Em 2026-07-31, M01, M02 e M03 foram executados novamente manualmente em Play Mode. Os três modelos continuaram funcionando e não foi observada regressão causada pela remoção do domínio anterior.

Este registro não representa compilação automatizada, execução de CI, profiling ou cobertura completa de casos negativos. Os cenários negativos e regressões técnicas ampliadas continuam sob responsabilidade do QAFramework.

## Documentation

- [Concept Guide](Documentation/M03_CONCEPT_GUIDE.md)
- [Authoring Guide](Documentation/M03_AUTHORING_GUIDE.md)
- [Play Mode Validation Guide](Documentation/M03_PLAY_MODE_GUIDE.md)
- [Re-entry Guide](Documentation/M03_REENTRY_SETUP.md)
- [Troubleshooting](Documentation/M03_TROUBLESHOOTING.md)
- [UX Findings and QA Follow-ups](Documentation/M03_UX_FINDINGS_AND_QA_FOLLOWUPS.md)

## What This Model Demonstrates

- uma contribuição authorável de readiness;
- um `ActivityReadinessParticipant` obrigatório;
- início da preparação controlado pelo runtime;
- conclusão semântica por `CompletePreparation()`;
- Activity agregada em `NotReady` enquanto a preparação está pendente;
- apresentação oficial por `ActivityReadinessEvents`;
- conteúdo preparado liberado somente em `Ready`;
- release do participant ao sair da Activity;
- nova ocorrência de readiness ao reentrar.

## Mental Model

```text
Scene loaded != Activity Ready
```

A cena pode ter sido carregada corretamente e o baseline técnico pode estar válido. Mesmo assim, uma dependência authorável obrigatória pode manter a Activity em `NotReady`.

No M03:

```text
Baseline técnico = Ready
Required participant = Preparing
Resultado agregado = NotReady
```

Depois da conclusão:

```text
Baseline técnico = Ready
Required participant = Completed
Resultado agregado = Ready
```

## Required Package Features

- `ActivityReadinessParticipant`;
- `ActivityReadinessEvents`;
- `ActivityReadinessSnapshot`;
- `GameApplicationAsset`;
- `RouteAsset`;
- `ActivityAsset`;
- `ActivityContentProfileAsset`;
- `ActivityRequestTrigger`.

## Assets

```text
Application/
  GA_M03_Readiness.asset

Routes/
  Route_M03_Readiness.asset

Activities/
  Activity_M03_Preparation.asset
  Activity_M03_Intermission.asset

Profiles/
  ActivityContent_M03_Preparation.asset
  ActivityContent_M03_Intermission.asset
```

## Scenes

```text
Scenes/
  M03_Boot.unity
  M03_Route.unity
  M03_Activity_Add.unity
  M03_Intermission_Add.unity
```

## Prefabs

```text
Prefabs/
  PF_M03_PreparationParticipant.prefab
  PF_M03_ReadinessDisplay.prefab
  PF_M03_PreparedContent.prefab
  PF_M03_ActivityNavigation.prefab
```

## Consumer Scripts

```text
Scripts/
  M03PreparationSequence.cs
  M03ReadinessPresenter.cs
```

These scripts belong to the FIRSTGAME demonstration and use the namespace:

```csharp
FirstGame.FrameworkModels.ActivityReadiness
```

They are not framework runtime authority.

## Composition

### Preparation participant

```text
PF_M03_PreparationParticipant
├── Visual Placeholder
├── Label
├── Framework Components
│   └── ActivityReadinessParticipant
└── Bindings
    └── M03PreparationSequence
```

Required bindings:

```text
PreparationStarted
→ M03PreparationSequence.BeginPreparation

PreparationReleased
→ M03PreparationSequence.ReleasePreparation
```

The sequence references the same `ActivityReadinessParticipant`.

### Readiness display

```text
PF_M03_ReadinessDisplay
├── Waiting Visual
├── Ready Visual
├── Status Label
├── Detail Label
├── Framework Components
│   └── ActivityReadinessEvents
└── Bindings
    └── M03ReadinessPresenter
```

Required bindings:

```text
ActivityReadinessEvents.Preparing
→ M03ReadinessPresenter.ShowPreparing

ActivityReadinessEvents.Ready
→ M03ReadinessPresenter.ShowReady

ActivityReadinessEvents.NotReady
→ M03ReadinessPresenter.ShowNotReady
```

The scene instance also references `PF_M03_PreparedContent`.

### Route navigation

`PF_M03_ActivityNavigation` remains in `M03_Route`, so it stays available while Activity-owned scenes are replaced.

```text
Leave Preparation
→ Activity_M03_Intermission

Return to Preparation
→ Activity_M03_Preparation
```

## Runtime Flow

```text
GA_M03_Readiness
→ Route_M03_Readiness
→ Activity_M03_Preparation
→ M03_Activity_Add loaded
→ participant discovered
→ participant Preparing
→ presentation Waiting
→ visual coroutine completes
→ CompletePreparation()
→ aggregate Ready
→ presentation Ready
→ prepared content available
```

## Re-entry Flow

```text
Preparation occurrence 1
→ Waiting
→ Ready
→ Leave Preparation
→ participant Released
→ M03_Activity_Add unloaded
→ M03_Intermission_Add loaded
→ Return to Preparation
→ M03_Intermission_Add unloaded
→ M03_Activity_Add loaded again
→ Preparation occurrence 2
→ Waiting
→ Ready
```

## Validated Checklist

- [x] Activity entra em estado de preparação.
- [x] O participant oficial participa da readiness.
- [x] Preparing/Waiting é apresentado visualmente.
- [x] Ready é apresentado após a conclusão.
- [x] A Activity não depende do controlador geral da demo para declarar readiness.
- [x] A troca de Activity libera a ocorrência anterior.
- [x] A reentrada inicia nova preparação.
- [x] Não foi observado estado stale na reentrada.
- [x] M01–M03 continuam funcionando após a remoção do domínio anterior.

## Expected Result

- `Waiting` appears when preparation starts;
- the Activity is `NotReady` while the visual sequence is running;
- the prepared content is unavailable before `Ready`;
- the sequence calls `CompletePreparation()` only after completion;
- the framework publishes the new aggregate state;
- the presenter reacts without polling or deciding readiness;
- leaving the Activity releases the participant;
- re-entry starts a new preparation normally.

## Reusable Pieces

| Piece | Role |
|---|---|
| `ActivityReadinessParticipant` | Official framework authoring/runtime contract |
| `ActivityReadinessEvents` | Official presentation bridge |
| `ActivityReadinessSnapshot` | Official read-only presentation state |
| `M03PreparationSequence` | FIRSTGAME consumer example |
| `M03ReadinessPresenter` | FIRSTGAME consumer presentation |
| M03 create/configure tools | Local demo setup helpers, not product runtime authority |
| Visuals and labels | Replaceable without changing the readiness contract |

## Important Boundary

```text
Designer declares intent.
Consumer performs preparation.
Framework decides readiness.
Presenter displays the official result.
```

The presenter must not become the authority. The coroutine must not set a separate global readiness flag. `CompletePreparation()` is the semantic handoff to the framework.

## UX Findings

See [UX Findings and QA Follow-ups](Documentation/M03_UX_FINDINGS_AND_QA_FOLLOWUPS.md).

## QA Follow-ups

Negative and regression scenarios belong to `QAFramework`, not to the FIRSTGAME happy-path model. They are listed in the QA follow-up document and remain deferred until an official QA cut is opened.

Examples that remain outside this manual happy-path validation include:

- required participant failure;
- optional participant failure;
- timeout;
- participant absent;
- fault injection;
- stress and repeated transition coverage.
