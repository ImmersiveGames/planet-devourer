# Demo 01 — Routes, Activities, Lifecycle Events and Readiness

Esta pasta demonstra os modelos M01, M02 e M03 do FIRSTGAME.

```text
M01  Route and Activity
M02  Lifecycle Events
M03  Activity Readiness
```

O objetivo é mostrar como um projeto consumidor declara lugares, momentos, conteúdo pertencente a cada escopo, callbacks de lifecycle e readiness operacional sem criar um fluxo paralelo ao framework.

## Como executar

Ative:

```text
Assets/_Project/Demo 01 - Routes and Activities/Demo01-GameApplication.asset
```

em:

```text
Assets/_Project/Settings/ImmersiveFramework/Resources/
└── ImmersiveFrameworkSettings.asset
```

A aplicação inicia pela Route:

```text
Assets/_Project/Demo 01 - Routes and Activities/Data/Demo01StartupMenu.asset
```

Cena inicial:

```text
Assets/_Project/Demo 01 - Routes and Activities/Scenes/Demo01StartScene.unity
```

Este README usa os nomes reais presentes no repositório. Não renomeie uma pasta isoladamente pelo sistema de arquivos; qualquer normalização futura deve ser feita dentro do Unity, preservando referências e GUIDs.

# Estrutura principal

```text
Demo 01 - Routes and Activities/
├── Demo01-GameApplication.asset
├── Data/
│   ├── Demo01StartupMenu.asset
│   ├── Activity Readiness/
│   │   ├── Activities/
│   │   └── Routes/
│   └── Routes and Activities/
│       ├── Activities/
│       └── Routes/
├── Prefabs/
│   ├── Activity Readiness/
│   └── Routes and Activities/
├── Scenes/
│   ├── Demo01StartScene.unity
│   ├── Activity Readiness/
│   └── Routes and Activities/
└── Scripts/
    ├── Activity Readiness/
    └── Routes and Activities/
```

# M01 — Route and Activity

## O que o modelo demonstra

```text
Route
  representa o destino/lugar do jogo;
  possui cena principal;
  pode possuir cenas adicionais Route-owned;
  pode iniciar uma Activity.

Activity
  representa o momento/modo dentro da Route;
  pode trocar sem recarregar a Route inteira;
  pode possuir cenas Activity-owned;
  emite lifecycle próprio.
```

A demo utiliza dois destinos visuais baseados na mesma cena principal e diferencia a composição por conteúdo adicional da Route e da Activity.

## Assets para localizar

Routes:

```text
Assets/_Project/Demo 01 - Routes and Activities/Data/Routes and Activities/Routes/
├── Sample_Route_Fields.asset
└── Sample_Route_Forest.asset
```

Route Content Profiles atualmente utilizados pela composição dos ambientes:

```text
Assets/_Project/Demo 01 - Routes and Activities/Data/Activity Readiness/Routes/Profiles/
├── Additive_Fields.asset
└── Additive_Forest.asset
```

Activities e seus profiles:

```text
Assets/_Project/Demo 01 - Routes and Activities/Data/Activity Readiness/Activities/
├── Sample_Activities_Cows.asset
├── Sample_Activites_Chickens.asset
└── Profiles/
    ├── Activity_Cow_Content_Profile.asset
    └── Activity_Chickens_Content_Profile.asset
```

Cenas:

```text
Assets/_Project/Demo 01 - Routes and Activities/Scenes/Routes and Activities/
├── RoutesContents/
│   ├── Sample_Environment.unity
│   └── Additives/
│       ├── Sample_Add_Fields.unity
│       └── Sample_Add_Forest.unity
└── ActivitiesContents/
    ├── SampleCows.unity
    └── SampleChickens.unity
```

## Composição esperada

```text
Route Fields ou Route Forest
├── Primary Scene
│   └── Sample_Environment
├── Route Content Profile
│   ├── navegação compartilhada da Route
│   └── cena aditiva específica do destino
└── Startup Activity
    └── Activity Content Profile
        └── cena aditiva específica do momento
```

A identidade funcional vem dos assets e IDs; nomes de GameObjects e nomes de arquivo servem apenas para apresentação e diagnóstico.

## Fluxo em Play Mode

```text
Menu solicita Route
→ framework libera a Route anterior
→ carrega Sample_Environment como Primary Scene
→ carrega cenas do Route Content Profile
→ cria o runtime scope da Route
→ entra na Startup Activity
→ carrega conteúdo Activity-owned
→ emite Entered
```

Ao trocar somente a Activity:

```text
Activity anterior sai
→ seu conteúdo é liberado
→ nova Activity entra
→ seu conteúdo é carregado
→ a Route e seu conteúdo continuam ativos
```

## Hierarquia funcional recomendada ao replicar

Na Primary Scene:

```text
Environment_Root
├── geometria persistente durante a Route
├── pontos de referência visuais
└── superfícies que pertencem ao lugar, não à Activity
```

Na cena Route-owned:

```text
RouteContent_Root
├── navegação da Route
├── decoração específica do destino
└── componentes de lifecycle de Route, quando necessários
```

Na cena Activity-owned:

```text
ActivityContent_Root
├── conteúdo visual do momento
├── interações daquele momento
├── ActivityLocalVisibilityAdapter, quando aplicável
└── callbacks/presenters de lifecycle
```

Não coloque conteúdo que precisa sobreviver à troca de Activity dentro de uma cena Activity-owned.

## Como replicar em outro projeto

Crie ou identifique:

```text
GameApplicationAsset
Startup Menu Route
uma Route por destino
um Route Content Profile por composição adicional
uma ou mais Activities
um Activity Content Profile por conjunto de cenas Activity-owned
```

Configure na ordem:

```text
1. GameApplication → Startup Route
2. Route → Primary Scene
3. Route → Route Content Profile
4. Route → Startup Activity
5. Activity → Activity Content Profile
6. botões → requests tipadas para Route/Activity
```

Teste obrigatoriamente:

```text
entrada na Route;
troca de Activity sem recarregar a Primary Scene;
saída da Route;
reentrada sem conteúdo duplicado.
```

# M02 — Lifecycle Events

## O que o modelo demonstra

O framework emite eventos claros para a disponibilidade e liberação das cenas, entrada e saída da Route e entrada e saída da Activity.

```text
Scene
  Available
  Releasing

Route
  Entered
  Exited

Activity
  Entered
  Exited
```

Os callbacks podem atualizar UI, iniciar animação, ligar interação ou registrar evidência. Eles não devem escolher a Route/Activity por conta própria nem criar uma autoridade paralela.

## Prefabs e scripts para localizar

Prefabs:

```text
Assets/_Project/Demo 01 - Routes and Activities/Prefabs/Routes and Activities/UI/
├── Canvas-Lifecyle.prefab
├── RoutesAndActivities_RoutesNavigation.prefab
└── RoutesAndActivities_ActivityNavigation.prefab
```

Scripts do consumidor:

```text
Assets/_Project/Demo 01 - Routes and Activities/Scripts/Routes and Activities/
├── LifecycleCanvasEventReporter.cs
├── LifecycleCanvasEventTypes.cs
├── LifecycleCanvasPresenter.cs
└── Editor/
    ├── LifecycleCanvasPrefabInstaller.cs
    └── LifecycleCanvasPresenterEditor.cs
```

Esses scripts são apresentação e diagnóstico do FIRSTGAME. Eles não são autoridade runtime do framework.

## Hierarquia funcional

```text
Canvas-Lifecyle
├── área de status de Scene
├── área de status de Route
├── área de status de Activity
└── Last Event
```

Os objetos que recebem callbacks devem manter referências explícitas para o presenter ou usar UnityEvents configurados no Inspector. Não resolva o canvas por nome ou busca global.

## Como replicar em outro projeto

```text
1. Escolha o objeto que precisa reagir ao lifecycle.
2. Adicione a superfície oficial de eventos correspondente ao escopo.
3. Conecte Entered/Exited ou Available/Releasing por referência explícita.
4. Mantenha o callback pequeno e pertencente ao jogo consumidor.
5. Use o console e uma UI simples apenas para diagnóstico.
```

Exemplo:

```text
Activity Entered
→ iniciar animação local
→ habilitar interação
→ atualizar HUD

Activity Exited
→ parar comportamento
→ limpar estado local
→ ocultar apresentação
```

## Aceite do M02

```text
Scene Available ocorre antes do uso do conteúdo;
Route Entered/Exited acompanha a ocorrência correta;
Activity Entered/Exited acompanha cada troca;
callbacks não disparam duplicados após reentrada;
UI de diagnóstico não decide lifecycle.
```

# M03 — Activity Readiness

## O que o modelo demonstra

Activity Readiness agrega participantes registrados na ocorrência atual da Activity.

```text
Required participant
  bloqueia Ready enquanto não concluir.

Optional participant
  aparece no diagnóstico, mas não bloqueia Ready.
```

A demonstração possui duas Activities:

```text
Preparation
Intermission
```

O conteúdo de navegação pertence à Route e permanece disponível durante a troca entre Activities.

## Assets para localizar

Route:

```text
Assets/_Project/Demo 01 - Routes and Activities/Data/Activity Readiness/Routes/
└── RouteReadiness.asset
```

Route Content Profile:

```text
Assets/_Project/Demo 01 - Routes and Activities/Data/Routes and Activities/Routes/Profile/
└── RouteContent_Readines.asset
```

Activities:

```text
Assets/_Project/Demo 01 - Routes and Activities/Data/Routes and Activities/Activities/
├── ActivityReadiness_Preparation.asset
├── ActivityReadiness_Intermission.asset
└── Profiles/
    ├── ActivityContent_Preparation.asset
    └── ActivityContent_Intermission.asset
```

Cenas:

```text
Assets/_Project/Demo 01 - Routes and Activities/Scenes/Activity Readiness/
├── Activity_Readiness.unity
├── ActivityReadinessMenu.unity
├── Activity_Readiness_Intermission.unity
└── ActivitiesContent/
    └── Activity_Readiness_Add.unity
```

Prefabs:

```text
Assets/_Project/Demo 01 - Routes and Activities/Prefabs/Activity Readiness/
├── Readiness Participant.prefab
└── Ui/
    └── Canvas_ActivityReadinessNavigation.prefab
```

Scripts do consumidor:

```text
Assets/_Project/Demo 01 - Routes and Activities/Scripts/Activity Readiness/
├── ReadinessPreparationArea.cs
├── ReadinessPreparationSequence.cs
└── ReadinessProgressPresenter.cs
```

## Ownership da composição

```text
RouteReadiness
├── Primary Scene
│   └── Activity_Readiness
├── Route Content Profile
│   └── ActivityReadinessMenu
└── Startup Activity
    └── ActivityReadiness_Preparation
```

Preparation possui conteúdo próprio:

```text
ActivityContent_Preparation
└── Activity_Readiness_Add
```

Intermission possui conteúdo próprio:

```text
ActivityContent_Intermission
└── Activity_Readiness_Intermission
```

A navegação não deve ser declarada simultaneamente como conteúdo de Route e de Activity.

## Hierarquia funcional recomendada

Primary Scene:

```text
Activity_Readiness
└── ambiente compartilhado pela Route
```

Route-owned navigation scene:

```text
ActivityReadinessMenu
└── Canvas_ActivityReadinessNavigation
    ├── Preparation
    ├── Intermission
    └── Back To Menu
```

Activity-owned content:

```text
ActivityContent_Root
├── Readiness Participant (Required)
├── participante Optional permanentemente pendente
├── ReadinessPreparationArea / sequência de preparação
└── ReadinessProgressPresenter
```

A hierarquia exata pode variar, mas ownership e referências devem permanecer explícitos.

## Fluxo em Play Mode

```text
Route entra
→ conteúdo Route-owned fica disponível
→ Preparation inicia
→ participantes são registrados na ocorrência
→ Required fica Pending
→ progresso é apresentado
→ Required conclui
→ Activity fica Ready
```

Ao trocar para Intermission:

```text
Preparation sai
→ participantes da ocorrência são liberados
→ cena de Preparation é descarregada
→ Intermission entra
→ novos participantes pertencem à nova ocorrência
```

Ao voltar para Preparation, não pode haver handles ou participantes stale da ocorrência anterior.

## Semântica importante do corte atual

No runtime atual, readiness representa estado operacional pós-transição. Ela não é, por si só, um gate de revelação visual ou de entrada.

```text
transição pode concluir
→ Activity fica ativa
→ readiness continua Preparing/NotReady
→ depois alcança Ready
```

Portanto, não use esta demo para afirmar que o fade/loading permanecerá cobrindo o conteúdo até `Ready`. Essa política de reveal/gameplay release exige uma superfície de produto própria e não faz parte do M03 implementado.

## Como replicar em outro projeto

```text
1. Crie a Route e mantenha a navegação como Route-owned.
2. Crie uma Activity por momento operacional.
3. Declare as cenas Activity-owned em seus profiles.
4. Coloque participantes de readiness no conteúdo da Activity.
5. Marque como Required somente o que realmente bloqueia Ready.
6. Use Optional para diagnóstico não bloqueante.
7. Conecte um presenter passivo para mostrar estado/progresso.
8. Teste troca, saída e reentrada.
```

Não crie um manager global para controlar readiness. Não faça fallback silencioso quando um participante obrigatório falhar.

## Aceite do M03

```text
Required Pending impede Ready;
Required Complete permite Ready;
Optional Pending não bloqueia Ready;
participants pertencem à ocorrência correta;
troca de Activity libera o estado anterior;
Back To Menu libera Route e Activity;
reentrada cria uma ocorrência limpa;
nenhum warning/error bloqueante.
```

# O que é reutilizável

```text
padrão GameApplication → Route → Activity;
separação Route-owned / Activity-owned;
prefabs de navegação como apresentação;
presenter de lifecycle/readiness como consumidor passivo;
organização de conteúdo por root;
sequência de testes de entrada, troca, saída e reentrada.
```

Não copie automaticamente IDs, GUIDs, `ProjectSettings` ou a cena persistente. Recrie a intenção no projeto consumidor e use IDs próprios e estáveis.

# Diagnóstico de problemas

## A Route entra, mas a Activity não

Verifique:

```text
Startup Activity atribuída;
Activity ID válido;
Activity Content Profile válido;
cenas no Build Settings;
nenhuma falha bloqueante no diagnóstico da Route.
```

## A cena aditiva aparece duplicada

Verifique se ela foi declarada em mais de um owner:

```text
Primary Scene;
Route Content Profile;
Activity Content Profile.
```

Uma cena deve possuir uma autoridade de composição clara para cada ocorrência.

## O conteúdo visual não acompanha a Activity

Verifique:

```text
binding do ActivityLocalVisibilityAdapter;
ActivityAsset correto, não nome de GameObject;
root de conteúdo correto;
callbacks Entered/Exited;
cleanup na saída.
```

## Readiness nunca fica Ready

Verifique os participantes Required e seus diagnósticos. Um Optional pendente é esperado e não deve bloquear. Não transforme falha obrigatória em sucesso por timeout ou fallback.
