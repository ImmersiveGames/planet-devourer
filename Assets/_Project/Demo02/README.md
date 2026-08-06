# Demo 02 — Provisioned Players

**Status:** M06 e M07 demonstrados como baselines de consumidor  
**Última validação principal:** 2026-08-06  
**M08:** ainda não montado como demonstração dedicada  
**Unity:** 6.5

A Demo 02 compara duas origens de Logical Player:

```text
M06 — Scene-Provided Logical Player
  a cena já contém o Local Player Host e o Actor físico.

M07 — Manager-Provisioned Logical Player
  uma requisição explícita pede ao PlayerInputManager para criar o Host físico.
```

As duas variações convergem para o mesmo `PlayerSlotId` tipado e para a autoridade de participação da Session. Elas diferem em origem física, ownership e lifecycle.

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

A aplicação inicia pelo menu da Demo 02 e oferece entradas separadas para Scene-Provided e Manager-Provisioned.

# 2. Configuração compartilhada

`Demo02-GameApplication.asset` declara:

```text
Application Name
  Demo 02 Game Application

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

O Slot configurado define o assento estável da Session e o `defaultActorProfile` usado nas variações atuais.

Não usar `PlayerInput.playerIndex`, ordem de hierarquia ou nome de GameObject como `PlayerSlotId`.

# 3. Vocabulário canônico

| Conceito | Responsabilidade |
|---|---|
| Player Slot | assento estável de participação configurado pela Game Application |
| Logical Player | participante da Session associado a um Slot tipado |
| Local Player Host | objeto físico Unity que normalmente possui `PlayerInput` |
| Actor Profile | intenção authoring para identidade/composição de Actor |
| Logical Actor | identidade runtime correlacionada ao Logical Player |
| Actor Mount | ponto físico explícito onde o Actor é materializado ou adotado |
| Activity participation | projeção e requisitos aplicados por uma Activity |

Um Logical Player não implica automaticamente que o Actor já foi escolhido, preparado, materializado ou liberado para gameplay. Na variação M07 atual, porém, o `defaultActorProfile` é preparado pelo lifecycle após o Join válido.

# 4. M06 — Scene-Provided Logical Player

## 4.1 Intenção de produto

Use quando a Route ou Activity já contém a composição física do Player.

```text
SceneProvidedPlayer
├── PlayerInput
├── LocalPlayerHostAuthoring
├── SceneLocalPlayerAdmissionAuthoring
└── ActorMount
    └── PlayerActor_SceneProvided
```

O Actor é scene-owned. O framework valida e admite a composição existente; ele não deve instanciar silenciosamente um Actor duplicado.

## 4.2 Assets relevantes

```text
Assets/_Project/Demo02/Data/LocalProvisionedPlayer/
Assets/_Project/Demo02/Prefabs/LocalProvisionedPlayer/
Assets/_Project/Demo02/Scenes/LocalProvisionedPlayer/
Assets/_Project/Demo02/Scripts/LocalProvisionedPlayer/
```

A pasta histórica `LocalProvisionedPlayer` representa o caminho Scene-Provided do M06. Não interpretar esse nome como o M07 Manager-Provisioned.

## 4.3 Authoring principal

O Host técnico deve possuir:

```text
um único PlayerInput no mesmo root de LocalPlayerHostAuthoring;
ActorMount como child explícito;
um único PlayerActorDeclaration sob o ActorMount;
SceneLocalPlayerAdmissionAuthoring configurado com Slot, ActorProfile e Actor da cena.
```

Sequência authoring:

```text
1. configurar PlayerSlotProfile e ActorProfile;
2. colocar o Host e o Actor canônico na cena;
3. configurar o Scene-Provided Player Composer;
4. executar Apply / Rebuild;
5. executar Validate;
6. configurar a Activity com Explicit Slots e Logical Actors Prepared.
```

`Apply / Rebuild` deve ser idempotente. Ele grava evidence tipada, não executa gameplay e não atribui identidade runtime em Edit Mode.

## 4.4 Fluxo runtime

```text
Route/Activity entra
→ composer resolve Host, Slot e Actor authored
→ framework admite o Scene-Provided Logical Player
→ Actor existente é adotado
→ PlayerInput evidence é vinculada
→ Logical Actors Prepared é satisfeito
→ gameplay fica disponível
```

Saída:

```text
Activity/Route sai
→ admission e evidence contextual são liberadas
→ unload da cena destrói os objetos scene-owned
```

Reentrada cria nova ocorrência e nova admissão, sem Actor duplicado ou Slot stale.

## 4.5 Evidência aceita do M06

```text
PASS — Host scene-provided admitido
PASS — Slot estável atribuído
PASS — Actor existente adotado
PASS — movement disponível
PASS — release na saída
PASS — reentrada válida
PASS — Activity Restart readmite sem duplicação
PASS — Apply / Rebuild e Validate coerentes
```

# 5. M07 — Manager-Provisioned Logical Player

## 5.1 Intenção de produto

Use quando uma requisição explícita deve criar o Local Player Host por meio do `PlayerInputManager`.

Assets principais:

```text
Assets/_Project/Demo02/Data/ManagerProvisionedPlayer/
├── Activities/ActivityManagerPlayer.asset
├── Commands/Manager Provisioned Player Command Channel.asset
└── Routes/RouteManagerPlayer.asset

Assets/_Project/Demo02/Prefabs/ManagerProvisionedPlayer/Player/
└── Player_ManagerProvisioned.prefab

Assets/_Project/Demo02/Scenes/ManagerProvisionedPlayer/
├── SceneManagerPlayer.unity
├── Additive/SceneManagerPlayerMenu.unity
└── Activity/SceneManagerPlayerActivity.unity

Assets/_Project/Demo02/Scripts/ManagerProvisionedPlayer/Commands/
├── ManagerProvisionedPlayerCommandChannel.cs
├── ManagerProvisionedPlayerCommandEmitter.cs
└── ManagerProvisionedPlayerCommandReceiver.cs
```

## 5.2 Composição persistente

A composição persistente expõe os endpoints oficiais necessários:

```text
PlayerInputManager
LocalPlayerProvisioningAuthoring
LocalPlayerProvisioningHostRegistration
LocalPlayerActorSelectionRequestAuthoring
ManagerProvisionedPlayerCommandReceiver
```

O prefab do Host contém:

```text
PlayerInput
LocalPlayerHostAuthoring
ActorMount
```

O Host não executa gameplay. O Actor materializado sob o mount contém o comportamento consumidor.

## 5.3 Fluxo canônico do M07

```text
Open Joining
→ Request Join
→ PlayerInputManager cria o Host
→ framework reserva/admite o Slot configurado
→ lifecycle prepara o default Actor do Slot
→ Actor é materializado sob ActorMount
→ readiness da Activity conclui
→ Loading fecha
→ movement fica disponível
```

Controles primários corretos:

```text
Open Joining
Request Join
Close Joining
Restart Activity
Back To Menu
```

Controles que permanecem desativados nesta variação:

```text
Select Default Actor
Request Join + Select Default Actor
```

Motivo:

```text
SinglePlayerSlotProfile já declara defaultActorProfile;
Join válido admite o Host;
lifecycle da Activity prepara/materializa o Actor configurado;
um pedido posterior de seleção é redundante e recebe RejectedLogicalActorAlreadyPrepared.
```

A correção de produto é não ensinar o comando redundante. O runtime deve continuar rejeitando explicitamente a operação inválida.

## 5.4 Hierarquia esperada após Join

```text
DontDestroyOnLoad
└── Immersive Framework Runtime
    └── Player Activity Readiness
        └── Player_ManagerProvisioned(Clone)
            └── ActorMount
                └── PlayerActor_SceneProvided
```

Contagens esperadas:

```text
Local Player Hosts = 1
ActorMounts = 1
physical Actors = 1
```

O nome `PlayerActor_SceneProvided` é um finding de nomenclatura; não altera a funcionalidade.

# 6. Lifetime de Session e Activity

## 6.1 Session-scoped

```text
Player Slot joined;
Manager-Provisioned Local Player Host;
PlayerInput/device correlation;
Joining state;
seleção/default Actor associada ao participante.
```

## 6.2 Activity-contextual

```text
readiness contribution;
Actor materialization contextual;
gameplay state da ocorrência;
Activity content;
Loading/gates da transição.
```

Consequências demonstradas:

```text
Close Joining não remove Player admitido;
Activity Restart não exige novo Join;
Route exit/reentry pode preservar o Player da Session;
saída durante espera cancela a ocorrência, mas não fecha Joining automaticamente;
Back To Menu não é Session Leave.
```

O framework ainda não possui um fluxo público completo de `Session Leave` demonstrado no FIRSTGAME.

# 7. Jornadas manuais validadas do M07

## 7.1 Matriz

| Caso | Resultado |
|---|---|
| Runtime com um Slot, capacity 1 e Joining fechado | Passed |
| Request Join enquanto fechado | `RejectedJoiningClosed`, sem estado parcial |
| Open Joining após rejeição | Passed |
| Request Join autorizado | Host, Slot, `PlayerInput` e ActorMount correlacionados |
| Default Actor lifecycle | Actor aparece sem segundo comando |
| Movement | Passed |
| Segundo Request Join | rejeição explícita por capacidade, sem duplicação |
| Close Joining após admissão | Player permanece operacional |
| Activity final readiness | pronta sem blocking issues |
| Activity Restart | clear/reentry sem novo Join |
| Restart position | Actor retorna ao ponto inicial |
| Restart hierarchy | um Host, um mount, um Actor |
| Restart input | movement restaurado |
| Route exit | conteúdo e roots contextuais liberados |
| Route return após Join | Player da Session preservado; Actor retorna sem novo Join |
| Exit while waiting | espera cancelada, Loading/gates liberados |
| Joining após saída da espera | permanece aberto |
| Reentrada após saída da espera | Request Join direto funciona |

## 7.2 Rejeição e recuperação

```text
Request Join com Joining fechado
→ rejeição explícita
→ Open Joining
→ Request Join
→ Actor aparece
→ movement funciona
```

Não houve fallback silencioso, Host parcial ou transação envenenada.

## 7.3 Happy path e proteção de capacidade

```text
Open Joining
→ Request Join
→ Host + Actor
→ movement
→ Request Join novamente
→ rejeição por capacidade
→ hierarquia continua com um Host e um Actor
```

## 7.4 Activity Restart

```text
Join válido
→ mover Actor
→ Restart Activity
→ Actor retorna ao spawn
→ movement funciona
→ sem duplicação
```

A prova manual confirma o contrato consumidor. Ela não afirma identidade exata da instância Unity antes/depois porque não foi registrado um occurrence identifier do Actor.

## 7.5 Route exit e reentrada

```text
Join válido
→ Back To Menu
→ entrar no M07 novamente
→ nenhum novo Join
→ Actor retorna
→ movement funciona
```

Isso prova preservação do participante da Session e recomposição contextual da Activity.

## 7.6 Saída durante espera

```text
entrar no M07
→ Open Joining
→ não executar Join
→ Back To Menu durante a espera
→ entrar novamente
→ Request Join diretamente
```

A nova Route substitui a autoridade anterior, a espera é cancelada, Loading/gates são liberados e a próxima transação válida funciona.

# 8. Loading e readiness

Sequência observada:

```text
Activity entry começa
→ cenas técnicas carregam
→ Activity aguarda Player obrigatório
→ Slot entra
→ Logical Actor é preparado
→ Actor físico é materializado
→ contribuição de readiness fica Ready
→ aggregate Activity fica Ready
→ Loading fecha
→ gameplay gate libera
```

Classificação da evidência:

```text
comportamento visual: confirmado
resultado final: confirmado
correlação temporal completa nos logs: parcial
```

Scripts FIRSTGAME não devem localizar o Loading adapter, escrever progresso, recalcular readiness ou usar parsing de logs como autoridade.

# 9. Findings de UX e produto

| ID | Finding | Severidade | Destino / ação |
|---|---|---:|---|
| M07-UX-01 | `Select Default Actor` é redundante após o Join atual | Média | manter desativado no M07 |
| M07-UX-02 | `Request Join + Select Default Actor` ensina sequência incorreta | Média | manter desativado |
| M07-DIAG-01 | Rejeições esperadas são apresentadas como `Debug.LogError` | Baixa | reclassificar depois sem esconder status tipado |
| M07-DIAG-02 | Route replacement intencional durante espera parece falha | Média | revisar vocabulário package/presentation |
| M07-DIAG-03 | Sequência Loading/readiness é mais clara visualmente que nos logs | Média | melhorar observabilidade no package |
| M07-DIAG-04 | Restart sem Reset Subjects produz warning não bloqueante | Baixa | registrar; não criar subject artificial |
| M07-NAME-01 | Actor compartilhado tem nome Scene-Provided no M07 | Baixa | normalizar nome em cleanup futuro |
| M07-AUTH-01 | Setup exige vários componentes técnicos | Média | acumular evidência antes de criar Recipe/Composer |
| M07-DOC-01 | README antigo congelava o escopo no M06 | Alta | corrigido por esta consolidação |

Classificação recomendada para comandos:

| Resultado | Apresentação |
|---|---|
| operação bem-sucedida | Info / success |
| rejeição de regra esperada | Info ou Warning com status tipado |
| binding ausente/configuração inválida | Error |
| exception/estado impossível | Error |

# 10. Limites do baseline M07

Incluído:

```text
um Slot configurado;
capacity 1;
Joining inicialmente fechado;
Host Manager-Provisioned;
default Actor;
movement;
Activity Restart;
Route exit/reentry;
exit while waiting.
```

Não avaliado:

```text
Camera;
Pause;
múltiplos Players;
escolha explícita entre Actors;
Session Leave;
disconnect/reconnect;
split-screen.
```

# 11. Features bloqueadas por package

Exigem contrato/superfície oficial antes de demonstração canônica:

```text
seleção explícita entre ActorProfiles arbitrários;
Join que permanece Actor-less aguardando escolha;
public Session Leave;
Actor replacement após preparação;
Session-Persistent Logical Player;
disconnect/reconnect;
multiplayer Camera e Pause policy.
```

FIRSTGAME pode definir a UX desejada, mas não deve criar uma autoridade runtime paralela.

# 12. Próximas demonstrações

Ordem recomendada:

```text
1. Activity-owned Scene-Provided Player;
2. Dynamic Capacity e late join;
3. dois Manager-Provisioned Players com devices explícitos;
4. Participation Policies como Activities compreensíveis;
5. Demo de seleção explícita de Actor após corte oficial do package.
```

A próxima variação de seleção deve separar claramente:

```text
Open Joining
→ Request Join
→ Slot joined sem Actor preparado
→ apresentar opções
→ selecionar ActorProfile
→ confirmar
→ preparar/materializar Actor
```

Esse fluxo não faz parte do M07 atual.

# 13. Checklist de aceite

## M06

```text
[ ] Slot e default ActorProfile configurados
[ ] Host scene-provided possui um PlayerInput
[ ] ActorMount explícito
[ ] Actor é instância do prefab canônico
[ ] Apply / Rebuild idempotente
[ ] Validate válido
[ ] Activity usa Explicit Slots + Logical Actors Prepared
[ ] entrada, movement, saída e reentrada funcionam
[ ] sem duplicação
```

## M07

```text
[ ] Joining começa fechado
[ ] Request Join fechado é rejeitado explicitamente
[ ] Open Joining habilita a transação
[ ] Join cria um Host e um Actor
[ ] default Actor não exige seleção separada
[ ] segundo Join não duplica objetos
[ ] Close Joining não remove Player existente
[ ] Restart recompõe Actor sem novo Join
[ ] Route reentry preserva participação da Session
[ ] exit while waiting libera Loading e gates
[ ] nenhum blocking issue no happy path
```
