# FIRSTGAME Project Assets

O FIRSTGAME e o projeto consumidor real minimo do Immersive Framework 1.0.

Ele prova que o framework e usavel para iniciar um jogo pequeno em Unity 6.5, sem virar QA sintetico e sem carregar documentacao canonica do package.

Root canonico:

```text
Assets/_Project/
```

## Papel do FIRSTGAME

```text
QA prova que o framework funciona tecnicamente.
FIRSTGAME prova que o framework e usavel.
Package contem contratos/runtime/editor/docs oficiais.
```

O FIRSTGAME deve demonstrar apenas um fluxo real minimo:

```text
Menu
Start Game
Gameplay
Player minimo
Pause
Reset Player
Reset Room
Restart Activity
Spawn Runtime Box
Transition / Loading / Pause surfaces
```

Nao e objetivo deste projeto criar gameplay completo, sistema grande, arquitetura propria paralela ou smoke sintetico.

## Estrutura principal

```text
Assets/_Project/
  Prefabs/
  Scenes/
  ScriptableObjects/
  Scripts/
  Settings/
  UI/
  README.md
```

Cenas principais:

```text
Assets/_Project/Scenes/Menu/FG_Menu.unity
Assets/_Project/Scenes/Menu/FG_UIGlobal.unity
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Settings do framework no projeto consumidor:

```text
Assets/_Project/Settings/ImmersiveFramework/Resources/ImmersiveFrameworkSettings.asset
```

## Como rodar

1. Abra a cena:

```text
Assets/_Project/Scenes/Menu/FG_Menu.unity
```

2. Entre em Play Mode.
3. Clique em `Start Game`.
4. A rota de gameplay deve carregar `FG_Gameplay` com transition/loading do `FG_UIGlobal`.
5. O player minimo deve ficar ativo em `FG Activity A`.

Log esperado no Start Game:

```text
Route Request completed
reason='firstgame.start.game'
targetRoute='Gameplay Route'
activity='FG Activity A'
activityState='Active'
```

## Botoes principais

### Menu

| Botao | Prova | Reason esperado |
| --- | --- | --- |
| `Start Game` | troca da Menu Route para Gameplay Route | `firstgame.start.game` |

### Gameplay

| Botao | Prova | Reason esperado |
| --- | --- | --- |
| `Spawn Runtime Box` | instancia objeto runtime que entra no reset registry | `firstgame.runtime.box` |
| `Reset Player` | reseta apenas o player | `firstgame.reset.player` |
| `Reset Room` | reseta subjects atuais da rota/activity | `firstgame.reset.room` |
| `Restart Activity` | roda reset + clear + reenter da activity ativa | `firstgame.restart.activity` |
| `Go Activity B` | troca para activity B | `firstgame.activity.go-b` |
| `Restore Activity A` | volta para activity A | `firstgame.activity.restore-a` |
| `Clear Activity` | limpa a activity ativa | `firstgame.activity.clear` |

### UIGlobal / Pause

| Acao | Prova | Reason esperado |
| --- | --- | --- |
| Botao/trigger de pause | toggle visual/logico de pause | `firstgame.pause.toggle` |
| Teclado/input global | pause via `PauseInputActionTrigger` | `firstgame.pause.keyboard.toggle` |

## Modelo atual do player

O `PlayerPrototype` demonstra o modelo minimo de gameplay object participando do framework:

```text
PlayerPrototype
  ObjectEntryDeclaration
    objectEntryId = firstgame.player

  PlayerInput
  FirstGamePlayerMover

  PlayerActorDeclaration
    actorId = firstgame.player
    playerInput = PlayerInput

  PlayerSlotDeclaration
    slotId = player.1
    playerInput = PlayerInput

  PlayerSlotOccupancy
    player.1 -> firstgame.player
    source = PlayerActorDeclaration

  UnityPlayerInputGateAdapter
    sourceSlot = PlayerSlotDeclaration

  UnityResetSubjectAdapter
    sourcePlayerActor = PlayerActorDeclaration
    resolved subjectId = Actor:firstgame.player
    scope = Activity

  UnityTransformResetParticipant
  FirstGamePlayerResettableState : IUnityResettable
```

Responsabilidades:

```text
UnityTransformResetParticipant
  reseta posicao/rotacao/escala.

FirstGamePlayerResettableState
  demonstra estado logico resetavel de gameplay.
```

O jogo nao deve acessar `ResetRegistry` diretamente. Componentes de gameplay participam do reset implementando `IUnityResettable` ou usando participants Unity fornecidos pelo framework.

Ids esperados:

```text
Actor:firstgame.player
firstgame.player
firstgame.player.resettable-state
firstgame.reset.player
firstgame.reset.room
firstgame.restart.activity
```

## Modelo atual do runtime object

Prefab:

```text
Assets/_Project/Prefabs/FG_RuntimeBox.prefab
```

Modelo esperado:

```text
FG_RuntimeBox
  BoxVisual
  UnityResetSubjectAdapter
    idGeneration = RuntimeInstanceId
    runtimeSubjectIdPrefix = firstgame.runtime.box

  UnityTransformResetParticipant
```

Ao clicar `Spawn Runtime Box`, o spawner cria instancias com nomes incrementais:

```text
FG_RuntimeBox (Runtime 1)
FG_RuntimeBox (Runtime 2)
```

Logs esperados:

```text
[FIRSTGAME_RUNTIME_OBJECT] spawned. reason='firstgame.runtime.box'
```

E registro no reset:

```text
Unity Reset Subject Adapter registered reset subject
subjectId='firstgame.runtime.box#1'
origin='RuntimeRegistered'
participants='1'
```

## Smoke manual recomendado

Use este fluxo para validar que o FIRSTGAME continua usavel apos alteracoes simples.

### Smoke principal

1. Abrir `FG_Menu`.
2. Entrar em Play Mode.
3. Clicar `Start Game`.
4. Mover o player.
5. Usar Pause.
6. Usar Resume.
7. Clicar `Spawn Runtime Box`.
8. Clicar `Reset Player`.
9. Clicar `Reset Room`.
10. Clicar `Restart Activity`.
11. Clicar `Go Activity B`.
12. Clicar `Restore Activity A`.
13. Clicar `Clear Activity`.

### Ordem importante

`Restart Activity` precisa ser testado enquanto existe activity ativa.

Correto:

```text
Start Game
Restart Activity
Clear Activity
```

Incorreto para validar restart:

```text
Start Game
Clear Activity
Restart Activity
```

Depois de `Clear Activity`, o runtime fica sem activity ativa. Nesse estado, `Restart Activity` deve falhar com `RejectedNoActiveActivity`, e isso e comportamento correto.

## Resultados esperados

### Boot

```text
Boot succeeded
startupRoute='Menu Route'
primaryScene='FG_Menu'
Loading surface resolved
Pause surface resolved
Transition surface resolved
```

### Start Game

```text
Route Request completed
reason='firstgame.start.game'
kind='Succeeded'
targetRoute='Gameplay Route'
activity='FG Activity A'
activityState='Active'
```

### Player reset registration

```text
Unity Reset Subject Adapter registered reset subject
subjectId='Actor:firstgame.player'
scope='Activity'
participants='2'
```

### Reset Player

```text
Object Reset Request completed
reason='firstgame.reset.player'
status='Succeeded'
subjectId='Actor:firstgame.player'
subjects='1'
participants='2'
blockingIssues='0'
```

### Reset Room com um runtime box

```text
Object Reset Group Request completed
reason='firstgame.reset.room'
status='Succeeded'
subjects='2'
participants='3'
blockingIssues='0'
```

### Reset Room com dois runtime boxes

```text
Object Reset Group Request completed
reason='firstgame.reset.room'
status='Succeeded'
subjects='3'
participants='4'
blockingIssues='0'
```

### Restart Activity

```text
Activity Restart Request completed
reason='firstgame.restart.activity'
status='Succeeded'
resetStatus='Succeeded'
clearStatus='Succeeded'
reenterStatus='Succeeded'
```

### Activity demo

```text
Activity Request completed
reason='firstgame.activity.go-b'
kind='Succeeded'
activity='FG Activity B'
```

```text
Activity Request completed
reason='firstgame.activity.restore-a'
kind='Succeeded'
activity='FG Activity A'
```

```text
Activity Request completed
reason='firstgame.activity.clear'
kind='Succeeded'
activity='<none>'
activityState='None'
```

## Troubleshooting rapido

### `WaitingForRuntime` / `WaitingForOwner` no inicio

Esses logs sao normais durante o ciclo de inicializacao Unity.

O ponto importante e aparecer depois:

```text
Unity Reset Subject Adapter registered reset subject
```

### `Restart Activity` falha com `RejectedNoActiveActivity`

Causa comum: `Clear Activity` foi acionado antes do restart.

Resolucao: teste `Restart Activity` antes de `Clear Activity`.

### `Reset Room` mostra menos subjects do que esperado

Verifique se os runtime boxes foram spawnados antes do reset e se ainda existe activity ativa.

Exemplos:

```text
Player apenas:
subjects='1'
participants='2'

Player + 1 Runtime Box:
subjects='2'
participants='3'

Player + 2 Runtime Boxes:
subjects='3'
participants='4'
```

## O que nao deve entrar neste projeto

- roots de QA sintetico;
- runners de smoke sintetico;
- objetos ou scripts `QA_*`;
- cenas ou assets finais do QA Harness;
- documentacao canonica do framework;
- nomes temporarios como `Probe`, `Test` ou `Smoke` quando o arquivo ja representa comportamento final de jogo;
- assets de consumidor dentro do package.

## Politica de documentacao

Documentacao canonica do framework fica no package:

```text
Packages/com.immersive.framework/Documentation~/
```

FIRSTGAME mantem apenas notas operacionais de uso real:

```text
Assets/_Project/README.md
Assets/_Project/Documentation/
```

Nao criar documentacao nova em:

```text
Assets/_Documentation/
```

## Politica de alteracao segura

- renomes de MonoBehaviours anexados em cenas/prefabs devem ser feitos via Unity Editor;
- evitar editar `.unity` manualmente para componentes serializados;
- manter assets de FIRSTGAME em `Assets/_Project/`;
- nao mover assets para o package;
- nao criar sistemas grandes para provar um fluxo pequeno.

