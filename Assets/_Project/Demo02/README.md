# Demo 02 — Provisioned Players

Esta pasta agrupa os modelos de Player do FIRSTGAME.

O escopo documentado neste arquivo está congelado no M06:

```text
M06  Scene-Provided Player — concluído
M07  Manager-Provisioned Player — fora deste corte documental
M08  Participation Policies — fora deste corte documental
```

Pastas posteriores podem existir no repositório, mas não devem ser usadas como prova de que M07 ou M08 estão concluídos.

# Como executar o M06

Ative:

```text
Assets/_Project/Demo 02 - Provisioned Players/Demo02-GameApplication.asset
```

em:

```text
Assets/_Project/Settings/ImmersiveFramework/Resources/
└── ImmersiveFrameworkSettings.asset
```

A aplicação inicia por:

```text
Assets/_Project/Demo 02 - Provisioned Players/Data/Demo02StartupMenu.asset
```

Cena do menu:

```text
Assets/_Project/Demo 02 - Provisioned Players/Demo02StartMenu.unity
```

# Estrutura relevante ao M06

```text
Demo 02 - Provisioned Players/
├── Demo02-GameApplication.asset
├── Demo02StartMenu.unity
├── Data/
│   ├── Demo02StartupMenu.asset
│   ├── SinglePlayerSlotProfile.asset
│   ├── Actor_SceneProvidedPlayer.asset
│   └── LocalProvisionedPlayer/
│       ├── Activities/
│       │   └── ActivityLocalPlayer.asset
│       └── Routes/
│           ├── RouteLocalPlayer.asset
│           └── Profile/
│               └── RouteLocalPlayerContentProfile.asset
├── Prefabs/
│   └── LocalProvisionedPlayer/
│       ├── LocalPlayer_RoutesNavigation.prefab
│       └── Actors/
│           └── PlayerActor_SceneProvided.prefab
├── Scenes/
│   └── LocalProvisionedPlayer/
│       ├── SceneLocalPlayer.unity
│       └── Additive/
│           └── SceneLocalPlayerAdd.unity
└── Scripts/
    └── LocalProvisionedPlayer/
        └── SceneProvidedPlayerMovement.cs
```

A pasta histórica `LocalProvisionedPlayer` representa o M06 Scene-Provided Player. Não confunda esse nome com o fluxo Manager-Provisioned do M07.

# M06 — Scene-Provided Player

## O que o modelo demonstra

```text
A cena já contém o Player físico.
A Activity não instancia esse Player.
O framework admite a instância existente em um Slot configurado.
O Logical Actor permanece scene-owned.
O PlayerInput pertence ao Host técnico.
O gameplay simples pertence ao jogo consumidor.
```

O modelo diferencia cinco conceitos:

| Conceito | Responsabilidade |
|---|---|
| Player Slot | Assento configurado na Session |
| Local Player Host | Objeto técnico que possui `PlayerInput` e admission evidence |
| Actor Mount | Ponto explícito sob o Host onde o Logical Actor existe |
| Logical Player Actor | Identidade/representação de gameplay admitida pela Activity |
| Scene-Provided Player Composer | Intenção authorável que conecta Slot, Host, ActorProfile e Actor da cena |

# Configuração de assets

## GameApplication

Asset:

```text
Assets/_Project/Demo 02 - Provisioned Players/Demo02-GameApplication.asset
```

Configuração relevante:

```text
Application Name
  Demo 02 Game Application

Startup Route
  Demo02StartupMenu

Local Player Slots
  SinglePlayerSlotProfile

Persistent Content
  Shared_PersistentContent

Validation Mode
  Standard
```

## Player Slot Profile

Asset:

```text
Assets/_Project/Demo 02 - Provisioned Players/Data/SinglePlayerSlotProfile.asset
```

Configuração relevante:

```text
Player Slot ID
  player.1

Display Name
  Player 1

Default Actor Profile
  Actor_SceneProvidedPlayer
```

Use IDs próprios ao replicar. Não copie `player.1` se ele colidir com a identidade do projeto consumidor.

## Actor Profile

Asset:

```text
Assets/_Project/Demo 02 - Provisioned Players/Data/Actor_SceneProvidedPlayer.asset
```

Configuração relevante:

```text
Actor Kind
  Player

Actor Role
  Protagonist

Logical Actor Host Prefab
  PlayerActor_SceneProvided.prefab
```

O `ActorProfile` declara a composição lógica canônica; ele não instancia o prefab por conta própria neste fluxo.

## Route

Asset:

```text
Assets/_Project/Demo 02 - Provisioned Players/Data/LocalProvisionedPlayer/Routes/
└── RouteLocalPlayer.asset
```

Configuração relevante:

```text
Route Name
  Scene-Provided Player

Primary Scene
  SceneLocalPlayer

Route Content Profile
  RouteLocalPlayerContentProfile

Startup Activity
  ActivityLocalPlayer
```

## Route Content Profile

Asset:

```text
Assets/_Project/Demo 02 - Provisioned Players/Data/LocalProvisionedPlayer/Routes/Profile/
└── RouteLocalPlayerContentProfile.asset
```

Declara:

```text
SceneLocalPlayerAdd
  Required
  Route-owned
```

Essa cena contém a navegação compartilhada, incluindo `Back To Menu`. Como pertence à Route, permanece disponível durante qualquer Activity futura dentro da mesma Route.

## Activity

Asset:

```text
Assets/_Project/Demo 02 - Provisioned Players/Data/LocalProvisionedPlayer/Activities/
└── ActivityLocalPlayer.asset
```

Configuração relevante:

```text
Projection Mode
  Explicit Slots

Explicit Slots
  SinglePlayerSlotProfile

Zero Participant Policy
  Rejected

Requirement Level
  Logical Actors Prepared

Activity Content Profile
  None
```

O Player está na Primary Scene da Route; por isso a Activity não precisa carregar outra cena para possuí-lo. A Activity opera a participação lógica da instância existente.

# Prefab canônico do Logical Actor

Prefab:

```text
Assets/_Project/Demo 02 - Provisioned Players/Prefabs/LocalProvisionedPlayer/Actors/
└── PlayerActor_SceneProvided.prefab
```

Hierarquia funcional:

```text
PlayerActor_SceneProvided
├── PlayerActorDeclaration
├── CharacterController
├── SceneProvidedPlayerMovement
└── Visual
    └── modelo do personagem
```

Regras:

```text
não adicionar PlayerInput ao Actor;
não desempacotar a instância usada na cena;
manter exatamente um PlayerActorDeclaration canônico;
manter o prefab atribuído no ActorProfile;
movimento e visual são responsabilidade do jogo consumidor.
```

# Hierarquia da cena

Cena:

```text
Assets/_Project/Demo 02 - Provisioned Players/Scenes/LocalProvisionedPlayer/
└── SceneLocalPlayer.unity
```

Hierarquia relevante:

```text
SceneProvidedPlayer
├── PlayerInput
├── LocalPlayerHostAuthoring
├── SceneLocalPlayerAdmissionAuthoring
└── ActorMount
    └── PlayerActor_SceneProvided (prefab instance)
        ├── PlayerActorDeclaration
        ├── CharacterController
        ├── SceneProvidedPlayerMovement
        └── Visual
```

## Root `SceneProvidedPlayer`

### PlayerInput

```text
Actions
  InputActionAsset do projeto

Default Action Map
  mapa que contém Move
```

Deve existir exatamente um `PlayerInput` em toda a hierarquia do Host e ele deve estar no mesmo GameObject de `LocalPlayerHostAuthoring`.

### LocalPlayerHostAuthoring

```text
Player Input
  PlayerInput do mesmo root

Actor Mount
  filho ActorMount
```

O Host é técnico:

```text
possui PlayerInput;
recebe admission evidence;
conhece o Slot joined;
não seleciona ActorProfile;
não executa gameplay.
```

### SceneLocalPlayerAdmissionAuthoring

No Inspector aparece como `Scene-Provided Player` / `Scene-Provided Player Composer`, conforme a versão do package.

Configuração:

```text
Player Slot Profile
  SinglePlayerSlotProfile

Actor Profile
  Actor_SceneProvidedPlayer

Scene Logical Player Actor
  PlayerActorDeclaration da instância sob ActorMount

Admission Timing
  On Activity Enter
```

### ActorMount

```text
filho do Host;
não é o próprio root;
não contém outro PlayerInput;
contém exatamente um PlayerActorDeclaration;
contém uma instância do prefab definido pelo ActorProfile.
```

# Apply / Rebuild e Validate

Depois de configurar o composer:

```text
1. Apply / Rebuild
2. Validate
```

`Apply / Rebuild` não cria gameplay nem atribui identidade runtime. Ele confirma que o Actor da cena vem do prefab canônico do `ActorProfile` e grava evidence tipada no composer.

Resultado esperado na primeira aplicação:

```text
status
  Valid

succeeded
  True

createdEvidence
  True

updatedEvidence
  True
```

Uma segunda aplicação sem mudança deve ser idempotente:

```text
createdEvidence
  False

updatedEvidence
  False
```

Em `Advanced / Debug`:

```text
Typed Actor Evidence
  true

Evidence Actor Profile
  Actor_SceneProvidedPlayer

Evidence Actor Prefab
  PlayerActor_SceneProvided
```

# Movimento do consumidor

Script:

```text
Assets/_Project/Demo 02 - Provisioned Players/Scripts/LocalProvisionedPlayer/
└── SceneProvidedPlayerMovement.cs
```

O script lê a action `Move` do `PlayerInput` injetado no `PlayerActorDeclaration` após a admissão.

Ele não deve:

```text
usar Keyboard.current como autoridade;
procurar o Host por nome;
ativar actions desabilitadas pelo framework;
criar fallback quando não existe PlayerInput;
executar gameplay no authoring component.
```

O `CharacterController` e a gravidade simples são suficientes para a demonstração. Câmera seguindo o Player pertence ao M10, não ao M06.

# Navegação Route-owned

Cena:

```text
Assets/_Project/Demo 02 - Provisioned Players/Scenes/LocalProvisionedPlayer/Additive/
└── SceneLocalPlayerAdd.unity
```

Conteúdo principal:

```text
Canvas
└── LocalPlayer_RoutesNavigation
    └── Back To Menu
```

Essa cena pertence ao `RouteLocalPlayerContentProfile`. Não a declare também em um `ActivityContentProfile`.

# Fluxo runtime esperado

```text
Demo02StartMenu
→ solicita RouteLocalPlayer
→ SceneLocalPlayer é carregada
→ SceneLocalPlayerAdd é carregada como Route content
→ ActivityLocalPlayer entra
→ composer solicita admissão
→ SinglePlayerSlotProfile é reservado/joined
→ Actor da cena é adotado
→ PlayerInput evidence é vinculada ao Actor
→ requirement Logical Actors Prepared é satisfeito
→ Activity fica Ready
→ movimento funciona
```

No Inspector durante Play Mode:

```text
Runtime
  Ready

Admission
  Admitted

Host Joined
  true

Active Admission
  true

Actor Ownership
  ExternalSceneOwned
```

# Saída e reentrada

Ao selecionar `Back To Menu`:

```text
Activity sai
→ participant recebe Exit
→ admissão é liberada
→ Slot deixa de estar Joined
→ conteúdo Route-owned é descarregado
→ Primary Scene é substituída pelo menu
```

Na reentrada:

```text
nova instância de cena
→ nova ocorrência da Activity
→ nova admissão válida
→ sem Actor duplicado
→ sem Slot stale
→ movimento volta a funcionar
```

# Como replicar em outro projeto

## Assets

Crie ou identifique:

```text
GameApplicationAsset
PlayerSlotProfile
ActorProfile
RouteAsset
RouteContentProfileAsset
ActivityAsset
Logical Actor Host prefab
```

## Cenas

Crie ou identifique:

```text
menu;
Primary Scene com o Scene-Provided Player;
cena Route-owned para navegação compartilhada, se necessária.
```

## Montagem

```text
1. Adicione o Slot ao GameApplication.
2. Aponte o default ActorProfile do Slot.
3. Aponte o Logical Actor Host prefab no ActorProfile.
4. Coloque o Host técnico na Primary Scene.
5. Adicione PlayerInput e LocalPlayerHostAuthoring no mesmo root.
6. Crie ActorMount como filho explícito.
7. Instancie o prefab canônico do Actor sob ActorMount.
8. Adicione SceneLocalPlayerAdmissionAuthoring ao root.
9. Configure Slot, ActorProfile, Actor e Admission Timing.
10. Execute Apply / Rebuild e Validate.
11. Configure a Activity com Explicit Slots + Logical Actors Prepared.
12. Teste entrada, movimento, saída e reentrada.
```

# Erros comuns

## `MissingProfileEvidence`

Causa normal:

```text
Validate foi executado antes de Apply / Rebuild;
ou o Actor não está ligado a um prefab source.
```

Ação:

```text
confirme a instância do prefab;
execute Apply / Rebuild;
depois execute Validate.
```

## `IncompatibleProfileEvidence`

Verifique:

```text
ActorProfile.LogicalActorHostPrefab;
prefab source da instância da cena;
Scene Logical Player Actor selecionado;
instância não desempacotada.
```

Não compare apenas nomes: dois prefabs diferentes podem possuir o mesmo nome.

## Activity fica NotReady

Verifique:

```text
Slot está no GameApplication;
Activity projeta o Slot correto;
composer está no mesmo root do Host;
ActorProfile é Player / Protagonist;
Actor está sob ActorMount;
Apply / Rebuild e Validate passaram;
nenhum erro de admissão aparece no diagnóstico.
```

## Mensagem `Local Player provisioning is not configured`

No M06 isso é esperado. Ela se refere ao fluxo Manager-Provisioned, que não é usado pelo Scene-Provided Player.

# Critério de aceite do M06

```text
[ ] GameApplication possui o Slot
[ ] Slot possui default ActorProfile
[ ] ActorProfile aponta para o prefab canônico
[ ] cena contém um único Host
[ ] Host contém um único PlayerInput
[ ] ActorMount é explícito
[ ] Actor é instância do prefab canônico
[ ] Apply / Rebuild é válido e idempotente
[ ] Validate é válido
[ ] Activity usa Explicit Slots
[ ] requirement é Logical Actors Prepared
[ ] admissão ocorre no Activity Enter
[ ] Activity fica Ready
[ ] Actor recebe PlayerInput
[ ] movimento funciona
[ ] saída libera a admissão
[ ] reentrada não duplica Slot, Host ou Actor
[ ] nenhum warning/error bloqueante
```

# O que copiar e o que recriar

Pode ser reutilizado como referência:

```text
hierarquia Host → ActorMount → Actor;
separação entre Host técnico e gameplay;
sequência de Apply / Rebuild e Validate;
script simples de movimento;
padrão de navegação Route-owned.
```

Deve ser recriado no projeto consumidor:

```text
IDs de Slot e ActorProfile;
InputActionAsset;
GameApplication;
Routes e Activities;
modelo visual;
paths de cenas;
Persistent Content adequado ao projeto.
```

Não copie `ProjectSettings`, GUIDs ou IDs funcionais apenas para fazer a demonstração “passar”.
