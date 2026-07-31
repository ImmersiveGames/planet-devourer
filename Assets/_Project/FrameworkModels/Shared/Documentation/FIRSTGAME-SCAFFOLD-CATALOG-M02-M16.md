# FIRSTGAME Scaffold Catalog — M02, M03, M06–M16

Status: M03 closed; M06 is the next model  
Date: 2026-07-31

The unified scaffold removes repetitive asset, scene and prefab creation. It does not configure the models or change their roadmap state.

## Command

```text
Tools > Immersive Framework > FIRSTGAME > Scaffolds > Create Missing M02, M03, M06-M16
```

## Guarantees

```text
preserves existing files;
creates only missing files;
does not assign references between authoring assets;
does not add framework components to prefabs;
does not mount prefabs into scenes;
does not install bootstrap;
does not edit Build Profiles or ProjectSettings;
logs optional ScriptableObject types that are not yet available.
```

## Estado da demonstração

| Modelo | Estado | Observação |
|---|---|---|
| M02 — Lifecycle Events | Closed | Revalidado manualmente após a remoção do domínio anterior. |
| M03 — Activity Readiness | Closed | Authoring, Waiting → Ready, saída e reentrada revalidados em Play Mode. |
| M06 — Scene-Provided Player | Pending | Próximo modelo da demonstração. |

### Registro de regressão

Em 2026-07-31, M01, M02 e M03 foram executados novamente em Play Mode. Os três modelos continuaram funcionando e não foi observada regressão causada pela remoção do domínio anterior.

Esta atualização registra validação manual da demonstração. Ela não representa compilação automatizada, execução de CI, cobertura completa de casos negativos ou validação antecipada de M06. Casos negativos continuam sob responsabilidade do QAFramework.

## Lacuna intencional entre M03 e M06

M04 e M05 não fazem parte do catálogo atual.

Os dois modelos pertenciam a um domínio de ownership e materialização retirado do framework oficial durante a simplificação arquitetural. Como esse domínio não existe mais no produto, manter essas demonstrações criaria documentação e integração sem contrato oficial correspondente.

Os identificadores posteriores não foram renumerados. M06–M16 mantêm seus números para preservar rastreabilidade entre documentos, assets, histórico de desenvolvimento e referências já estabelecidas.

A sequência ativa é:

```text
M01
→ M02
→ M03
→ M06
→ M07
→ M08
→ M09
→ M10
→ M11
→ M12
→ M13
→ M14
→ M15
→ M16
```

M04 e M05 não são modelos pendentes, reservados ou desativados. Eles simplesmente não fazem parte da demonstração atual.

## Próximo modelo

### M06 — Scene-Provided Player

M06 demonstra como um Player já authorado na cena é apresentado ao framework, ocupa um Slot, recebe um Actor e participa oficialmente da Activity.

O modelo não cria o Player dinamicamente. Provisionamento via `PlayerInputManager` pertence ao M07.

## Models

### M02 — Lifecycle Events

Status: Closed  
Root: `Assets/_Project/FrameworkModels/M02_LifecycleEvents`  
Assets: 7 planned scaffold entries  
Scenes: 5  
Prefabs: 3

### M03 — Activity Readiness

Status: Closed  
Validation date: 2026-07-31  
Root: `Assets/_Project/FrameworkModels/M03_ActivityReadiness`  
Assets: 4 planned scaffold entries  
Scenes: 3  
Prefabs: 3

Validated flow:

```text
Activity enters
→ participant starts preparation
→ presentation shows Preparing/Waiting
→ Activity remains Not Ready
→ participant completes
→ presentation shows Ready
→ user changes Activity
→ user returns to the preparation Activity
→ a new readiness occurrence starts
→ preparation completes again without stale state
```

### M06 — Scene-Provided Player

Status: Pending — next model  
Root: `Assets/_Project/FrameworkModels/M06_SceneProvidedPlayer`  
Assets: 6 planned scaffold entries  
Scenes: 3  
Prefabs: 3

### M07 — Manager-Provisioned Player

Root: `Assets/_Project/FrameworkModels/M07_ManagerProvisionedPlayer`  
Assets: 6 planned scaffold entries  
Scenes: 3  
Prefabs: 5

### M08 — Participation Policies

Root: `Assets/_Project/FrameworkModels/M08_ParticipationPolicies`  
Assets: 7 planned scaffold entries  
Scenes: 2  
Prefabs: 3

### M09 — Input Gate

Root: `Assets/_Project/FrameworkModels/M09_InputGate`  
Assets: 4 planned scaffold entries  
Scenes: 3  
Prefabs: 4

### M10 — Player Camera

Root: `Assets/_Project/FrameworkModels/M10_PlayerCamera`  
Assets: 5 planned scaffold entries  
Scenes: 3  
Prefabs: 4

### M11 — Object Reset

Root: `Assets/_Project/FrameworkModels/M11_ObjectReset`  
Assets: 4 planned scaffold entries  
Scenes: 3  
Prefabs: 5

### M12 — Activity Restart

Root: `Assets/_Project/FrameworkModels/M12_ActivityRestart`  
Assets: 4 planned scaffold entries  
Scenes: 3  
Prefabs: 4

### M13 — Pause

Root: `Assets/_Project/FrameworkModels/M13_Pause`  
Assets: 6 planned scaffold entries  
Scenes: 4  
Prefabs: 5

### M14 — Transition and Loading

Root: `Assets/_Project/FrameworkModels/M14_TransitionLoading`  
Assets: 7 planned scaffold entries  
Scenes: 5  
Prefabs: 4

### M15 — Camera Overrides

Root: `Assets/_Project/FrameworkModels/M15_CameraOverrides`  
Assets: 6 planned scaffold entries  
Scenes: 4  
Prefabs: 3

### M16 — BGM

Root: `Assets/_Project/FrameworkModels/M16_Bgm`  
Assets: 8 planned scaffold entries  
Scenes: 5  
Prefabs: 4
