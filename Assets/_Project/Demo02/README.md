# Demo 02 — Player Source and Physical Lifetime

**Status:** fechada  
**Última validação principal:** 2026-08-06  
**Unity:** 6.5  
**Escopo:** um Player local; origem do Host e do Actor; ownership físico; lifetime de Route, Activity e Session

A Demo 02 responde:

> De onde o Player vem, quem possui fisicamente seu Host e Actor e por quanto tempo essa composição permanece?

A demonstração contém três modelos executáveis e uma comparação consolidada:

| Identificador | Modelo | Estado |
|---|---|---|
| `DEMO02-MODEL-01` | Scene-Provided Player — Route-Owned | Fechado |
| `DEMO02-MODEL-02` | Manager-Provisioned Player — Single Local Player | Fechado |
| `DEMO02-MODEL-03` | Scene-Provided Player — Activity-Owned | Fechado |
| `DEMO02-MODEL-04` | Player Source and Lifetime Comparison | Fechado |

Referências históricas:

```text
M06              → DEMO02-MODEL-01
M07 / PLAYER-D01 → DEMO02-MODEL-02
PLAYER-D02       → DEMO02-MODEL-03
PLAYER-D07       → DEMO02-MODEL-04
```

# 1. Como executar

Ative:

```text
Assets/_Project/Demo02/Demo02-GameApplication.asset
```

em:

```text
Assets/_Project/Settings/ImmersiveFramework/Resources/
└── ImmersiveFrameworkSettings.asset
```

A aplicação inicia em `Demo02StartMenu`.

Rótulos finais recomendados para o menu:

```text
Scene-Provided Player — Route-Owned
Scene-Provided Player — Activity-Owned
Manager-Provisioned Player — Session-Scoped Host
```

Os rótulos devem ensinar o modelo oficial. Evitar `Local Provisioned Player`, porque esse nome pode ser confundido com `Manager-Provisioned Player`.

# 2. Configuração compartilhada

`Demo02-GameApplication.asset` declara a infraestrutura comum:

```text
Startup Route
  Demo02StartupMenu

Configured Local Player Slots
  SinglePlayerSlotProfile

Initial Dynamic Capacity
  1

Initial Joining State
  Closed

Persistent Content
  Demo02 Persistent
```

O Slot é o assento estável da Session. Ele não deve ser inferido por `PlayerInput.playerIndex`, nome de GameObject ou ordem da Hierarchy.

# 3. Vocabulário canônico

| Conceito | Responsabilidade |
|---|---|
| Player Slot | assento estável de participação na Session |
| Logical Player | participante associado a um `PlayerSlotId` |
| Local Player Host | objeto físico Unity que normalmente possui `PlayerInput` |
| Actor Profile | intenção reutilizável de composição do Actor |
| Logical Actor | identidade runtime correlacionada ao Logical Player |
| Actor Mount | ponto físico explícito de adoção ou materialização |
| Actor materialization | criação ou adoção da representação física de gameplay |
| Activity participation | projeção e requisitos aplicados pela Activity |

As etapas são separadas:

```text
Player admitido
≠ Actor selecionado
≠ Logical Actor preparado
≠ Actor físico materializado
≠ gameplay admitido
```

Nos modelos atuais, o `defaultActorProfile` do Slot permite que o lifecycle prepare o Actor sem uma escolha interativa adicional.

# 4. DEMO02-MODEL-01 — Scene-Provided Player — Route-Owned

## 4.1 Intenção

Use quando a composição física do Player deve permanecer enquanto a Route permanecer ativa.

```text
Route Primary Scene owns
├── Local Player Host
├── PlayerInput
├── Actor Mount
└── scene-provided Actor
```

Estrutura principal:

```text
SceneProvidedPlayer
├── PlayerInput
├── LocalPlayerHostAuthoring
├── SceneLocalPlayerAdmissionAuthoring
└── ActorMount
    └── PlayerActor_SceneProvided
```

Assets principais:

```text
Assets/_Project/Demo02/Data/LocalProvisionedPlayer/
Assets/_Project/Demo02/Prefabs/LocalProvisionedPlayer/
Assets/_Project/Demo02/Scenes/LocalProvisionedPlayer/
Assets/_Project/Demo02/Scripts/LocalProvisionedPlayer/
```

A pasta histórica `LocalProvisionedPlayer` representa o modelo Scene-Provided. O nome da pasta não deve ser usado como terminologia oficial de produto.

## 4.2 Authoring

```text
1. configurar PlayerSlotProfile e ActorProfile;
2. colocar Host e Actor canônico na Primary Scene da Route;
3. configurar o Scene-Provided Player Composer;
4. executar Apply / Rebuild;
5. executar Validate;
6. configurar a Activity com o Slot e requisito adequado.
```

`Apply / Rebuild` materializa evidence authoring de forma idempotente. Ele não executa gameplay em Edit Mode.

## 4.3 Runtime e lifetime

```text
Route entra
→ Primary Scene fornece Host e Actor
→ framework admite a composição existente
→ Actor authored é adotado
→ Activities podem usar o Player
```

Durante mudanças de Activity na mesma Route:

```text
Route permanece
→ Host permanece
→ Actor físico permanece
→ Transform e estado físico podem permanecer
```

Ao sair da Route:

```text
Route sai
→ Primary Scene descarrega
→ Host e Actor scene-owned desaparecem
→ admission contextual é liberada
```

## 4.4 Evidência aceita

```text
PASS — Host scene-provided admitido
PASS — Slot tipado atribuído
PASS — Actor existente adotado
PASS — movement disponível
PASS — Activity changes não removem a composição Route-owned
PASS — Route exit libera a composição
PASS — reentrada válida
PASS — sem Actor duplicado
```

# 5. DEMO02-MODEL-02 — Manager-Provisioned Player — Single Local Player

## 5.1 Intenção

Use quando uma operação explícita de Join deve pedir ao `PlayerInputManager` que crie o Local Player Host.

Assets principais:

```text
Assets/_Project/Demo02/Data/ManagerProvisionedPlayer/
Assets/_Project/Demo02/Prefabs/ManagerProvisionedPlayer/
Assets/_Project/Demo02/Scenes/ManagerProvisionedPlayer/
Assets/_Project/Demo02/Scripts/ManagerProvisionedPlayer/
```

Composição persistente:

```text
PlayerInputManager
LocalPlayerProvisioningAuthoring
LocalPlayerProvisioningHostRegistration
ManagerProvisionedPlayerCommandReceiver
```

Prefab do Host:

```text
Player_ManagerProvisioned
├── PlayerInput
├── LocalPlayerHostAuthoring
└── ActorMount
```

O Host é a infraestrutura física do participante. O comportamento de gameplay pertence ao Actor materializado sob o mount.

## 5.2 Fluxo canônico

```text
Open Joining
→ Request Join
→ PlayerInputManager cria o Host
→ framework reserva e admite o Slot
→ lifecycle usa o default Actor do Slot
→ Logical Actor é preparado
→ Actor físico é materializado
→ readiness conclui
→ gameplay fica disponível
```

Controles primários:

```text
Open Joining
Request Join
Close Joining
Restart Activity
Back To Menu
```

Não ensinar como fluxo primário:

```text
Select Default Actor
Request Join + Select Default Actor
```

Na configuração atual, o Join válido já conduz à preparação do `defaultActorProfile`. Um pedido posterior é redundante e deve ser rejeitado explicitamente.

## 5.3 Lifetime

Session-scoped:

```text
Player Slot Joined
Local Player Host criado pelo manager
PlayerInput/device correlation
Joining state
seleção/default Actor associada ao participante
```

Activity-contextual:

```text
readiness contribution
Actor materialization para a ocorrência
estado de gameplay da Activity
Loading e gates
```

Consequências demonstradas:

```text
Close Joining não remove Player existente;
Activity Restart não exige novo Join;
Route exit/reentry pode preservar o participante da Session;
Back To Menu não equivale a Session Leave.
```

## 5.4 Evidência aceita

```text
PASS — Request Join fechado é rejeitado sem estado parcial
PASS — Open Joining habilita a entrada
PASS — exatamente um Host é criado
PASS — exatamente um Actor é materializado
PASS — default Actor não exige segundo comando
PASS — segundo Join não duplica objetos
PASS — Close Joining não expulsa o Player
PASS — Activity Restart recompõe o Actor sem novo Join
PASS — Route reentry preserva participação da Session
PASS — saída durante espera libera Loading e gates
```

# 6. DEMO02-MODEL-03 — Scene-Provided Player — Activity-Owned

## 6.1 Intenção

Use quando o Host e o Actor devem existir somente durante uma Activity específica.

```text
Route owns
├── ambiente persistente
└── navegação persistente

Player Activity owns
└── SceneProvidedPlayer
    ├── PlayerInput
    ├── LocalPlayerHostAuthoring
    ├── SceneLocalPlayerAdmissionAuthoring
    └── ActorMount
        └── PlayerActor_SceneProvided
```

Assets principais:

```text
Assets/_Project/Demo02/Data/ActivityOwnedPlayer/
Assets/_Project/Demo02/Scenes/ActivityOwnedPlayer/
```

Composição:

```text
Route_ActivityOwnedPlayer
├── Primary Scene
│   └── SceneActivityOwnedPlayerEnvironment
├── Route Content
│   └── SceneActivityOwnedPlayerNavigation
└── First Activity
    └── Activity_ActivityOwnedPlayerIntermission

Activity_ActivityOwnedPlayer
└── SceneActivityOwnedPlayer
    └── SceneProvidedPlayer

Activity_ActivityOwnedPlayerIntermission
└── SceneActivityOwnedPlayerIntermission
```

## 6.2 Configuração decisiva

A cena que possui o Player usa:

```text
Load Mode
  Additive

Release Policy
  Release On Activity Change
```

A Player Activity projeta o Slot configurado e requer o nível de readiness escolhido para o Actor. A Intermission usa zero Slots e não contém Host ou Actor.

## 6.3 Fluxo runtime

```text
Entrar na Route
→ ambiente e navegação carregam
→ Intermission inicia sem Player

Entrar na Player Activity
→ cena Activity-owned carrega
→ Host e Actor authored aparecem
→ framework admite a composição
→ movement funciona

Voltar para Intermission
→ Player Activity sai
→ cena do Player descarrega
→ Host e Actor desaparecem
→ admission é liberada

Reentrar na Player Activity
→ nova ocorrência física é carregada
→ Player é readmitido
→ existe exatamente um Host e um Actor
```

## 6.4 Estado físico após reentrada

O Actor retorna ao Transform authored da cena.

```text
Activity sai
→ cena é descarregada
→ Actor físico deixa de existir

Activity reentra
→ cena é carregada novamente
→ nova ocorrência usa o estado authored
```

Isso é comportamento esperado, não falha. Quando o jogo precisa preservar o mesmo objeto e sua posição entre Activities, o modelo adequado é o Scene-Provided Player Route-owned.

## 6.5 Evidência aceita

```text
PASS — Route inicia sem Player
PASS — Player Activity admite a composição scene-provided
PASS — movement funciona
PASS — Intermission libera Host e Actor
PASS — Slot volta a ficar disponível
PASS — reentrada cria uma única composição válida
PASS — Actor retorna ao estado authored
PASS — sem duplicação
```

# 7. DEMO02-MODEL-04 — Player Source and Lifetime Comparison

## 7.1 Comparação canônica

| Modelo | Quem fornece o Host | Quem fornece o Actor físico | Lifetime físico principal | Entrada do usuário |
|---|---|---|---|---|
| Scene-Provided — Route-Owned | Primary Scene da Route | Primary Scene da Route | Route | entrar na Route |
| Scene-Provided — Activity-Owned | cena de conteúdo da Activity | cena de conteúdo da Activity | Activity | entrar na Player Activity |
| Manager-Provisioned | `PlayerInputManager` após Join | framework a partir do `ActorProfile` | Host na Session; Actor contextual | abrir Joining e solicitar Join |
| Session-Persistent | indisponível | contrato futuro | Session | ainda sem fluxo oficial |

## 7.2 Regra de escolha

Use **Route-owned** quando:

```text
o mesmo Player deve permanecer fisicamente entre Activities;
posição e estado físico devem sobreviver às trocas;
a Route representa um espaço contínuo de gameplay.
```

Use **Activity-owned** quando:

```text
o Player só deve existir em Activities específicas;
a saída da Activity deve remover Host e Actor;
a reentrada deve começar do estado authored.
```

Use **Manager-Provisioned** quando:

```text
a entrada depende de Join;
o Host deve ser criado sob demanda;
dispositivo e PlayerInput precisam ser correlacionados;
a participação deve sobreviver além de uma Activity.
```

Não simular **Session-Persistent Player** colocando arbitrariamente um prefab em Persistent Content. O modelo ainda exige contrato oficial de package.

## 7.3 Diferença essencial

```text
Scene-Provided
  a cena fornece uma composição física existente.

Manager-Provisioned
  uma operação de Join solicita a criação do Host.

Route-owned versus Activity-owned
  define o lifetime da composição scene-provided.
```

# 8. Findings consolidados

| ID | Finding | Classificação | Destino |
|---|---|---|---|
| `D02-NAME-01` | `LocalProvisionedPlayer` é nome histórico ambíguo | não bloqueante | usar terminologia canônica em UI/docs |
| `D02-AUTH-01` | Scene-Provided ficou fácil com prefabs reutilizáveis | positivo | manter composição explícita |
| `D02-AUTH-02` | Route/Activity authoring já estava compreendido após Demo 01 | positivo | não repetir tutorial básico |
| `D02-LIFE-01` | Activity-owned retorna ao estado authored após reentrada | esperado | documentado como escolha de lifetime |
| `M07-UX-01` | seleção default explícita é redundante após Join atual | não bloqueante | manter ação fora do fluxo primário |
| `M07-DIAG-01` | rejeições esperadas aparecem como Error | melhoria de diagnóstico | package futuro |
| `M07-DIAG-02` | observabilidade temporal de Loading/readiness é parcial | melhoria de diagnóstico | package futuro |
| `M07-AUTH-01` | Manager-Provisioned exige vários componentes técnicos | evidência de produto | acumular antes de Recipe/Composer |

# 9. Limites da Demo 02

Incluído:

```text
um Player local;
Scene-Provided Route-owned;
Scene-Provided Activity-owned;
Manager-Provisioned por Join;
default Actor;
movement;
Activity Restart;
Route exit/reentry;
release e reentrada de Activity-owned.
```

Fora de escopo:

```text
múltiplos Players;
Dynamic Capacity como demonstração dedicada;
late join multiplayer;
dispositivos explícitos para dois Players;
Participation Policies como cenários;
seleção interativa de Actor;
Session Leave;
Session-Persistent Player;
disconnect/reconnect;
split-screen;
multiplayer Camera e Pause.
```

Esses temas continuam na Demo 03, Demo 04 ou em cortes oficiais do package.

# 10. Critério de fechamento

```text
[x] menu identifica os três modelos por source e lifetime
[x] Route-owned permanece entre Activities da mesma Route
[x] Activity-owned desaparece na Intermission e retorna sem duplicação
[x] Manager-Provisioned cria Host somente após Join
[x] README explica quando usar cada modelo
[x] Session-Persistent aparece somente como indisponível
[x] não há runtime paralelo no FIRSTGAME
[x] package e QA não foram alterados por este corte documental
```

Quando esses itens estão confirmados:

```text
DEMO02-MODEL-01 — Closed
DEMO02-MODEL-02 — Closed
DEMO02-MODEL-03 — Closed
DEMO02-MODEL-04 — Closed
Demo 02 — Closed
```

# 11. Próximo bloco

```text
Demo 03 — Local Multiplayer Foundations

DEMO03-MULTI-01
  Dynamic Capacity and Late Join

DEMO03-MULTI-02
  Two Local Players with Explicit Devices
```
