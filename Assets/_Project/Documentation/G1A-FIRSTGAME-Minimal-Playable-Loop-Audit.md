# G1A — FIRSTGAME Minimal Playable Loop Audit

Status: Completed — static audit
Last updated: 2026-07-10
Decision: Saída B — falta uma peça singular, game-owned, no FIRSTGAME.

## Resumo executivo

O FIRSTGAME já compõe a entrada Menu → Gameplay, a `Startup Activity`, Player controlável, câmera, Pause, Reset e Activity Restart. Ainda não há uma ação do jogador que conclua um objetivo de gameplay, nem uma mudança de estado de objetivo observável, ligada ao Player e restaurada pelo Restart. Os botões existentes exercitam infraestrutura e não são um objetivo jogável.

O G1B deve criar apenas um objetivo local no FIRSTGAME: uma zona de chegada com collider trigger, uma alteração visual explícita e um componente game-owned que restaure seu próprio estado através do `UnityResetSubjectAdapter` já usado no loop. Não é necessária API, contrato ou runtime novo de package.

## Fluxo auditado

```text
FG_Menu / Button_StartGame
  -> RouteRequestTrigger(reason=firstgame.start.game)
  -> FG_GameplayRoute
  -> FG_Activity_A (startupActivity)
  -> PlayerPrototype / PlayerInput map=Player
  -> UnityPlayerInputGateAdapter libera o controle após entrada
  -> CameraComposer consome CameraTarget e LookAtTarget
```

`FG_GameplayRoute` declara `FG_Activity_A` como `startupActivity`; ambos usam `transitionGateMode: 30`. A prova P2G anterior documenta a observação de entrada, Gate liberado, action map habilitado e deslocamento real. Esta auditoria não reexecutou Unity, Play Mode ou smoke.

## Inventário e decisão por superfície

| Evidência | Responsabilidade | Categoria | Decisão |
|---|---|---|---|
| `FG_Menu.unity` / `Button_StartGame` | Solicita a rota Gameplay (`firstgame.start.game`). | ProductSurface | KEEP |
| `FG_GameplayRoute.asset` + `FG_Activity_A.asset` | Entrada de gameplay e Startup Activity. | FrameworkCore consumption | KEEP |
| `FG_Gameplay.unity` / `PlayerPrototype` | `PlayerInput`, `UnityPlayerInputGateAdapter`, `PlayerComposer`, `UnityTransformResetParticipant`, `UnityResetSubjectAdapter`. | ComposerOrAuthoring + UnityAdapter | KEEP, condicionado à confirmação de importação abaixo. |
| `FirstGamePlayerMover.cs` | Consome `Player/Move` e executa movimento no jogo. | RuntimeAuthority game-owned | KEEP |
| `FIRSTGAME_CameraComposerRig` | Câmera única, com referências explícitas ao `PlayerComposer`, `CameraTarget` e `LookAtTarget`. | ComposerOrAuthoring | KEEP |
| `FG_UIGlobal.unity` / `FG_PauseKeyboard`, `PauseControls`, `PauseSurface` | Pause/Resume e superfície residente; o atalho usa `Global/Pause`. | ProductSurface + UnityAdapter | KEEP; validar em Play Mode dentro do loop. |
| `Button_RestartActivity` | `ActivityRestartTrigger`, Activity atual e seleção `CurrentRouteAndActivitySubjects`. | ProductSurface | KEEP |
| `Button_ResetPlayer` e `Button_ResetRoom` | Reset explícito de sujeito e grupo; ambos usam seleção sem fallback silencioso. | ProductSurface | KEEP como diagnóstico, não como objetivo. |
| `Button_SpawnRuntimeBox`, troca/clear de Activity e botões de reset | Ferramentas de prova/infraestrutura, não uma meta causada pelo Player. | DiagnosticsOnly | Não reutilizar como objetivo. |
| `FirstGamePlayerResettableState.cs` | Estado resetável do Player, mutável apenas por Context Menu e sem feedback visual. | RuntimeAuthority game-owned | Não usar como objetivo. |
| Nenhum `OnTriggerEnter`, collider com `m_IsTrigger: 1` ou controlador de objetivo em `Assets/_Project`. | Objetivo jogador→estado concluído. | Missing | Criar no G1B, somente no FIRSTGAME. |

## Player, estado inicial e restauração

### Player técnico

```text
Cena: FG_Gameplay.unity
Localização para diagnóstico: PlayerPrototype
Identidade tipada: actorId=player.actor; playerSlotId=player.1
Activity owner: FG_Activity_A
```

O `PlayerComposer` referencia `CameraTarget` e `LookAtTarget`, participa do Gate e mantém Reset de Composer desabilitado por política. O reset ativo é explícito e separado: `UnityTransformResetParticipant` captura posição, rotação e escala locais, enquanto o `UnityResetSubjectAdapter` descobre participantes locais incluindo `IUnityResettable`.

Estado inicial do Player: transform local `{0,0,0}`, action map `Player`, Gate de entrada já liberado conforme a prova P2G. O `ObjectResetTrigger` de `Button_ResetPlayer` aponta diretamente ao sujeito do Player.

### Restart técnico

```text
Localização para diagnóstico: Button_RestartActivity
Componente: ActivityRestartTrigger
Activity alvo: Activity atual (target ausente, requireTargetActivityIsCurrent=true)
Seleção: CurrentRouteAndActivitySubjects
Ordem: reset pré-clear -> clear Activity -> re-enter Activity
```

Na `FG_Activity_A`, a seleção inclui o sujeito do Player porque ele declara escopo Activity e o mesmo `activityOwner`. Portanto o mecanismo para restaurar Player e um futuro objetivo pertencente à Activity já existe, desde que ambos estejam registrados como sujeitos participantes dessa seleção.

## Candidato de objetivo

Não existe candidato aceitável já conectado ao Player.

- `Button_ClearActivity` e os botões de troca de Activity alteram fluxo técnico por UI; não são alcance/ativação no mundo pelo Player.
- `Button_SpawnRuntimeBox` cria uma instância, mas o spawner não é uma meta concluível, não registra conclusão e não reverte a própria criação em Activity Restart.
- `Cube`, `Sphere` e `Capsule` têm colliders não-trigger (`m_IsTrigger: 0`) e nenhuma semântica de interação.
- `FirstGamePlayerResettableState` tem estado inteiro interno e reset, mas não é acionado pelo jogador durante o loop e não tem feedback observável.

Classificação do objetivo: `MISSING`.

### Peça mínima aprovada para G1B

Um único objeto de objetivo game-owned, associado explicitamente ao Player por referência de componente/Collider — nunca por nome de GameObject — deve:

1. detectar a entrada do Player em uma zona trigger;
2. trocar uma única evidência visual explícita (por exemplo, indicador ativo ou material/cor); 
3. manter um booleano local `completed` e ignorar repetição após conclusão;
4. restaurar booleano e visual ao baseline como participante do `UnityResetSubjectAdapter` de escopo Activity;
5. permanecer no FIRSTGAME, sem criar abstração de objetivo/interação/missão no framework.

Estado inicial: zona disponível, `completed=false`, indicador de conclusão inativo/baseline.

Estado concluído: uma única entrada válida do Player torna `completed=true` e ativa a evidência visual.

Reset explícito: o participante do objetivo restabelece `completed=false` e o visual baseline. Activity Restart seleciona o sujeito do objetivo junto ao sujeito do Player antes do re-entry da `FG_Activity_A`.

## Pause, câmera e repetibilidade

Pause está presente em `FG_UIGlobal.unity`: `PauseRequestTrigger`, `PauseInputActionTrigger` em `Global/Pause`, botões Pause/Resume e `UnityPauseResidentSurfaceAdapter`. A P2G prova controle na entrada, mas não há prova integrada de Pause → objetivo → Resume. Esse comportamento é uma validação manual obrigatória do G1B/G1C; não foi inferido como aprovado somente pela configuração estática.

A câmera permanece com uma única autoridade `PlayerComposer → CameraComposer`, conforme `C8B5-FIRSTGAME-CANONICAL-CAMERA-USAGE-PROOF.md`; não adicionar binding de câmera por Route/Activity neste corte.

Repetibilidade alvo:

```text
concluir objetivo -> Activity Restart -> Player e objetivo no baseline
-> Activity Ready/Gate liberado -> Player e câmera funcionam -> concluir novamente
```

## Gap e risco encontrados

| Severidade | Evidência | Impacto | Ação |
|---|---|---|---|
| P1 | A cena serializa `FirstGame.FirstGamePlayerMover`, `_Project.Scripts.FirstGamePlayerResetProbe` e `_Project.Scripts.FirstGameRuntimeObjectSpawner`; os fontes atuais declaram outros namespaces/tipos, e `FirstGamePlayerResetProbe` não foi localizado. | Não é seguro assumir que a cena importa sem Missing Script ou que Player/Reset/Spawner funcionem. | Antes de G1B, abrir `FG_Gameplay`, confirmar Console sem erro/Missing Script e salvar somente se o Unity reserializar validamente. Corrigir no owner FIRSTGAME se houver falha. |
| P1 | Ausência de trigger/objetivo game-owned em `Assets/_Project`. | Não há propósito além de mover. | Criar somente a peça singular aprovada acima. |
| P2 | Pause e Restart existem, mas não há prova integrada do loop. | O comportamento real após Pause/Restart permanece não confirmado. | Executar a checklist manual de G1C. |

## Decisão de G1B

**Saída B — falta uma peça singular.**

G1B cria exclusivamente no FIRSTGAME um objetivo de alcance resetável, com estado e feedback visual locais. O framework já oferece seleção, execução de Reset, Activity Restart, Gate, Pause e CameraComposer suficientes para a composição. Não há evidência de uma lacuna recorrente que justifique produto oficial.

## O que não mudar agora

- Nenhum package `com.immersive.*`, QAFramework, contrato público ou runtime global.
- Nenhum sistema genérico de objetivo, interação, missão, inventário ou checkpoint.
- Nenhum fallback por reload de cena.
- Nenhum binding de câmera por Route/Activity.
- Nenhuma identidade por nome de GameObject; nomes acima são apenas localização e diagnóstico.

## Checklist manual para G1B/G1C

1. Abrir `FG_Gameplay` e confirmar importação limpa dos componentes citados no risco P1.
2. Entrar por `FG_Menu` → `Button_StartGame`; confirmar route concluída, Activity `Ready`, Gate liberado e map `Player` ativo.
3. Antes de concluir: Pause, verificar movimento bloqueado e objetivo inalterado; Resume, verificar map e movimento restaurados.
4. Concluir o objetivo; verificar exatamente uma alteração visual e nenhuma dupla conclusão.
5. Depois de concluir: Pause/Resume; verificar que o estado concluído é preservado.
6. Acionar `ActivityRestartTrigger`; verificar reset do Player e objetivo, re-entry `Ready`, controle e `CameraComposer` operantes.
7. Concluir o objetivo uma segunda vez.
