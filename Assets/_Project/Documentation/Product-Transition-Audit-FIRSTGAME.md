# Product Transition Audit - FIRSTGAME

Status: auditoria de produto, sem implementacao
Data: 2026-07-09
Escopo: `Assets/_Project`

## 1. Resumo executivo

O FIRSTGAME prova que o Immersive Framework ja consegue rodar um fluxo real minimo: Menu, Start Game, Gameplay, Player, Pause, Reset, Restart Activity, Runtime Box, Loading e Transition. A prova, porem, ainda e mais tecnica do que authoravel. Um designer conseguiria operar o projeto se ele ja estivesse pronto, mas dificilmente criaria o mesmo fluxo do zero sem conhecer os componentes internos, a ordem de menus, os IDs esperados e os validadores.

O maior problema de usabilidade e que a intencao de produto esta fragmentada entre:

- GameObjects com muitos componentes tecnicos do framework.
- Assets `RouteAsset` e `ActivityAsset` com `routeContentProfile` / `activityContentProfile` vazios.
- Menus editor-only `FIRSTGAME > Immersive Framework` que aplicam, validam ou reparam wiring.
- Documentacao que descreve smoke/logs esperados em vez de um fluxo authoring-first.
- Runtime real parcial: existem triggers e surfaces, mas Player/Input/Camera/Reset ainda dependem de bindings e adapters visiveis demais.

Conclusao de produto: o package oficial nao deve copiar as solucoes FIRSTGAME como definitivas. Ele deve formalizar conceitos de produto que o FIRSTGAME revelou: Recipes, Composers, Apply/Rebuild idempotente, `_Framework/_Bindings` materializado, runtime authority escopada e Inspector designer-first com detalhes tecnicos escondidos em Advanced/Debug.

## 2. Problemas reais de usabilidade encontrados

### P0 - Criacao do zero depende de conhecimento tecnico

Evidencia:

- `Assets/_Project/README.md` lista o modelo do `PlayerPrototype` como uma cadeia de componentes: `ObjectEntryDeclaration`, `PlayerInput`, `PlayerActorDeclaration`, `PlayerSlotDeclaration`, `PlayerSlotOccupancy`, `UnityPlayerInputGateAdapter`, `UnityResetSubjectAdapter`, `UnityTransformResetParticipant`, `FirstGamePlayerResettableState`.
- `Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity` contem essa cadeia diretamente no player (`ObjectEntryDeclaration`, `UnityPlayerInputGateAdapter`, `UnityResetSubjectAdapter`, `PlayerActorDeclaration`, `PlayerSlotDeclaration`, `PlayerSlotOccupancy`, `UnityPlayerInputActivationTargetBehaviour`, `UnityPlayerInputBridgeTargetBehaviour`, `PlayerControlBindingTargetBehaviour`).
- `Assets/_Project/Documentation/F53F-Player-Binding-Operational-Guide.md` orienta rodar validadores/appliers em ordem, nao criar um Player por um fluxo de authoring.

Impacto: contratos podem passar validacao, mas a experiencia de criacao ainda exige que o usuario entenda a arquitetura interna.

Owner correto futuro: `com.immersive.framework` / ProductSurface + ComposerOrAuthoring.

### P0 - Validators e menus viraram fluxo principal

Evidencia:

- Menus ativos historicos: `Validate Real Player Binding`, `Configure Route-Activity Camera` e ferramentas locais antigas de player binding.
- As ferramentas locais antigas centralizavam validacao/aplicacao de identity, PlayerInput/F52 targets e camera anchors no consumidor.
- A prova local antiga criava drift controlado e provava reparo fora do package oficial.
- A doc `F53F-Player-Binding-Operational-Guide.md` foi atualizada para apontar ao `PlayerComposer` oficial como fluxo principal.

Impacto: o framework parece usavel quando o caminho feliz ja foi montado, mas a UX real e "rode ferramenta/validador", nao "crie e configure um recurso".

Owner correto futuro: Editor tooling oficial com Composer + Apply/Rebuild; validators devem virar DiagnosticsOnly.

### P1 - Inspector expoe componentes internos como UX principal

Evidencia:

- Camera: `FrameworkCameraDirector`, `FrameworkRouteCameraBinding`, `FrameworkActivityCameraBinding`, `FrameworkCameraAnchorHost`, `FrameworkCinemachineRigApplier`, `RouteContentBinding`.
- Route/Activity: `ActivityLocalVisibilityAdapter`, `RouteContentBinding`, `RouteRequestTrigger`, `ActivityRequestTrigger`, `ActivityRestartTrigger`.
- Reset: `UnityResetSubjectAdapter`, `UnityTransformResetParticipant`, `ObjectResetTrigger`, `ObjectResetGroupTrigger`.
- Pause/Loading/Transition: `UnityPauseResidentSurfaceAdapter`, `PauseRequestTrigger`, `PauseInputActionTrigger`, `UnityLoadingSurfaceAdapter`, `UnityFadeCurtainEffectAdapter`.

Impacto: a cena e tecnicamente correta, mas o Inspector nao comunica intencao de designer. Ele comunica adapters, triggers e bindings.

Owner correto futuro: `_Framework/_Bindings` como materializacao tecnica + Composers designer-first.

### P1 - Falta Recipe/Profile/Template para sistemas recorrentes

Evidencia:

- `FG_MenuRoute.asset` e `FG_GameplayRoute.asset` tem `routeContentProfile: {fileID: 0}`.
- Todas as `ActivityAsset` atuais tem `activityContentProfile: {fileID: 0}`.
- Audio tem `AudioBgmCueAsset`, mas o binding de BGM fica manualizado nos roots de cena.
- Camera e Player usam scripts editor-only FIRSTGAME para reconstruir setup.

Impacto: a intencao reutilizavel nao esta em assets authoring claros. O jogo carrega wiring concreto em cena.

Owner correto futuro: RecipeOrProfile no package oficial.

### P1 - Runtime authority existe parcialmente, mas nao e o centro da UX

Evidencia:

- O README espera logs como `Boot succeeded`, `Route Request completed`, `Object Reset Request completed`, `Activity Restart Request completed`.
- As cenas usam triggers que chamam runtime (`RouteRequestTrigger`, `ActivityRequestTrigger`, `ObjectResetTrigger`, `PauseRequestTrigger`).
- Player/Input/Camera dependem de bindings locais e facades editor-only para manter referencias canonicas.

Impacto: ha runtime real para Route/Activity/Reset/Pause/Loading/Transition, mas a autoridade de configuracao ainda esta espalhada em componentes de cena e ferramentas editor.

Owner correto futuro: Runtime Context/Session escopado no framework, sem lookup global e sem expor pipeline interno como UX.

## 3. Matriz por sistema

### Player

Evidencia principal:

- `PlayerPrototype` em `FG_Gameplay.unity`.
- `FirstGamePlayerMover.cs`.
- `FirstGamePlayerResettableState.cs`.
- `FirstGamePlayerIdentityResolver.cs`.
- Ferramentas editor-only locais antigas de player binding, agora substituidas pelo `PlayerComposer` oficial.

1. Um designer conseguiria criar isso do zero? Nao de forma confiavel. Precisaria saber quais declaracoes, bindings, reset participants, PlayerInput e IDs usar.
2. O Inspector e compreensivel? Parcialmente. `PlayerInput` e `FirstGamePlayerMover` sao claros; `PlayerActorDeclaration`, `PlayerSlotDeclaration`, `PlayerSlotOccupancy`, `UnityPlayerInputGateAdapter`, `PlayerControlBindingTargetBehaviour`, bridge e activation target sao tecnicos.
3. O que esta misturado entre gameplay e framework? Movimento gameplay fica ao lado de identity, slot, reset subject, gate adapter e player binding targets.
4. Quais componentes tecnicos poluem o GameObject? `ObjectEntryDeclaration`, `PlayerActorDeclaration`, `PlayerSlotDeclaration`, `PlayerSlotOccupancy`, `UnityPlayerInputGateAdapter`, `UnityResetSubjectAdapter`, `UnityTransformResetParticipant`, `PlayerControlBindingTargetBehaviour`, `UnityPlayerInputBridgeTargetBehaviour`, `UnityPlayerInputActivationTargetBehaviour`.
5. O que deveria virar Recipe? `Player Recipe`: Player identity, slot, input action map, reset scope, camera target role e default movement/input expectations.
6. O que deveria virar Composer? `Player Composer`: adiciona/valida PlayerInput, actor/slot, reset subject, input gate, camera anchors e binding targets.
7. O que deveria ser materializado em `_Framework/_Bindings`? Binding targets de player control/input, reset subject adapter quando derivado, slot occupancy derivado e camera anchor host ligado ao player.
8. O que precisa de runtime authority? `Player Runtime Context`: player slot atual, actor atual, input activation/gate state, reset registration e camera target current.
9. O que hoje so e provado por validator/smoke? Identity canonica, PlayerInput/action map, bridge/activation target, camera anchor targets e drift repair.
10. O que deveria migrar para o package oficial? Um fluxo oficial de `Create Player` / `Player Composer` / `Apply Player Bindings`, nao os IDs ou nomes FIRSTGAME.

Severidade: Alta. O Player e a principal prova de usabilidade e hoje e o ponto com maior vazamento tecnico.

### Camera

Evidencia principal:

- `FirstGameCameraCutSetup.cs`.
- `Documentation/Camera/POST-RESET-H2-FIRSTGAME-Camera-Route-Activity.md`.
- `FirstGameCameraRoot`, `FirstGameCameraAnchors`, `MenuRoute_CameraRig`, `GameplayRoute_CameraRig`, `ActivityA_CameraRig`.

1. Um designer conseguiria criar isso do zero? Nao. O fluxo depende do menu `Configure Route-Activity Camera` e de nomes/paths hardcoded no tool FIRSTGAME.
2. O Inspector e compreensivel? Parcialmente para rigs Cinemachine; pouco para `FrameworkCameraDirector`, `FrameworkRouteCameraBinding`, `FrameworkActivityCameraBinding`, `FrameworkCameraAnchorHost`, `FrameworkCinemachineRigApplier`.
3. O que esta misturado entre gameplay e framework? Rigs de camera de jogo convivem com bindings de Route/Activity, anchors de player, policies e director tecnico.
4. Quais componentes tecnicos poluem o GameObject? `FrameworkCameraDirector`, `FrameworkRouteCameraBinding`, `FrameworkActivityCameraBinding`, `FrameworkCameraAnchorHost`, `FrameworkCinemachineRigApplier`, `RouteContentBinding`, `ActivityLocalVisibilityAdapter`.
5. O que deveria virar Recipe? `Route Camera Recipe` e `Activity Camera Recipe`: default rig, activity policy, anchor source, priority e transition/logging policy.
6. O que deveria virar Composer? `Camera Composer`: cria root, rigs, anchors e bindings por Route/Activity com Apply/Rebuild.
7. O que deveria ser materializado em `_Framework/_Bindings`? `FrameworkCameraRoot`, route/activity camera bindings, anchor hosts e rig applier.
8. O que precisa de runtime authority? `Camera Runtime Context`: current route rig, current activity rig, retained activity rig, active anchor set e transition state.
9. O que hoje so e provado por validator/smoke? Camera route/activity mapping, target transform em anchors, remocao de `Camera`/`CinemachineBrain` de rigs e comportamento de fallback/retain.
10. O que deveria migrar para o package oficial? Composer/Recipe para camera route/activity e um Inspector orientado a "Route Camera" / "Activity Camera", nao o setup FIRSTGAME.

Severidade: Alta. A camera e funcional, mas authoring depende de ferramenta local e de entendimento de adapters.

### Route / Activity

Evidencia principal:

- `FG_GameApplication.asset`.
- `FG_MenuRoute.asset`, `FG_GameplayRoute.asset`.
- `FG_Activity_A/B/C/D.asset`.
- Triggers e content roots em `FG_Menu.unity` e `FG_Gameplay.unity`.

1. Um designer conseguiria criar isso do zero? Parcialmente. Assets de Route/Activity existem, mas falta Create flow completo que cria scene, content root, bindings, startup activity e buttons.
2. O Inspector e compreensivel? Os assets `RouteAsset`/`ActivityAsset` sao razoaveis, mas `transitionGateMode` numerico e profiles vazios dificultam. Na cena, triggers e adapters sao tecnicos.
3. O que esta misturado entre gameplay e framework? UI buttons de gameplay chamam diretamente triggers framework; roots de activity misturam visibilidade, BGM e camera binding.
4. Quais componentes tecnicos poluem o GameObject? `RouteRequestTrigger`, `ActivityRequestTrigger`, `ActivityRestartTrigger`, `RouteContentBinding`, `ActivityLocalVisibilityAdapter`.
5. O que deveria virar Recipe? `Route Recipe` e `Activity Recipe`: scene, startup activity, transition policy, content groups, BGM, camera, reset scope.
6. O que deveria virar Composer? `Route/Activity Composer`: cria assets, roots, bindings e botao/trigger opcional.
7. O que deveria ser materializado em `_Framework/_Bindings`? Route content bindings, activity visibility adapters, route/activity trigger adapters opcionais e route-owned audio/camera bindings.
8. O que precisa de runtime authority? `Route Runtime Context` e `Activity Runtime Context`: current route, current activity, transition gate, loaded scenes, active content groups.
9. O que hoje so e provado por validator/smoke? Sequencia Menu -> Gameplay, startup activity A, Clear Activity, Restart Activity e activity-specific camera/BGM policies.
10. O que deveria migrar para o package oficial? Create menu/wizard para Route + Activity + content roots; nao os nomes `FG_*`.

Severidade: Alta. Route/Activity e o centro do produto e ainda exige setup manual em muitos pontos.

### Objects / Reset

Evidencia principal:

- `UnityResetSubjectAdapter` e `UnityTransformResetParticipant` no player e no prefab `FG_RuntimeBox`.
- `ObjectResetTrigger`, `ObjectResetGroupTrigger`, `ActivityRestartTrigger` nos botoes.
- `FirstGamePlayerResettableState.cs` implementa `IUnityResettable`.
- `FirstGameRuntimeObjectSpawner.cs` cria runtime box.

1. Um designer conseguiria criar isso do zero? Parcialmente. Reset de transform e player state exigem entender subject, participant, scope, requiredness, groupId e reasons.
2. O Inspector e compreensivel? Pouco. `Reset Subject`, `Transform`, `Reset Room` sao claros como intencao; os adapters e enums de id/scope nao sao designer-first.
3. O que esta misturado entre gameplay e framework? Estado de gameplay resetavel implementa interfaces do framework; spawner gameplay cria prefabs que ja carregam reset subject framework.
4. Quais componentes tecnicos poluem o GameObject? `UnityResetSubjectAdapter`, `UnityTransformResetParticipant`, `ObjectResetTrigger`, `ObjectResetGroupTrigger`, `ActivityRestartTrigger`.
5. O que deveria virar Recipe? `Resettable Object Recipe`, `Runtime Spawned Resettable Recipe`, `Reset Group Recipe`.
6. O que deveria virar Composer? `Reset Composer`: adiciona subject/participants a prefab ou objeto de cena e cria triggers opcionais.
7. O que deveria ser materializado em `_Framework/_Bindings`? Subject adapters e generated reset participants quando derivaveis; triggers conectados a UI podem ficar como authoring controls.
8. O que precisa de runtime authority? `Reset Runtime Context`: registry por route/activity/session, subjects ativos, participants, blocking issues e ownership de runtime-spawned subjects.
9. O que hoje so e provado por validator/smoke? Registro do player, registro de runtime boxes, contagem de subjects/participants, Reset Player, Reset Room e Restart Activity.
10. O que deveria migrar para o package oficial? Recipes e Composer de reset; nao o `FirstGameRuntimeObjectSpawner`.

Severidade: Media-Alta. Funciona, mas o modelo mental de subject/participant ainda e tecnico demais.

### Input

Evidencia principal:

- `PlayerInput` no `PlayerPrototype`.
- `FirstGamePlayerMover.cs` usa strings `actionMapName = "Player"` e `moveActionName = "Move"`.
- `UnityPlayerInputGateAdapter`, `UnityPlayerInputBridgeTargetBehaviour`, `UnityPlayerInputActivationTargetBehaviour`.
- `PauseInputActionTrigger` em `FG_UIGlobal.unity`.

1. Um designer conseguiria criar isso do zero? Nao sem guia tecnico. Precisa saber action maps, slot IDs, bridge/activation targets e gate adapter.
2. O Inspector e compreensivel? `PlayerInput` e action names sao conhecidos para usuario Unity; bridge/gate/activation sao framework internals.
3. O que esta misturado entre gameplay e framework? Movimento direto por `FirstGamePlayerMover` usa PlayerInput enquanto framework tambem controla gate/activation.
4. Quais componentes tecnicos poluem o GameObject? `UnityPlayerInputGateAdapter`, `UnityPlayerInputBridgeTargetBehaviour`, `UnityPlayerInputActivationTargetBehaviour`, `PauseInputActionTrigger`.
5. O que deveria virar Recipe? `Input Recipe`: action asset, gameplay map, UI map, pause map/action, player slot binding e input gate policy.
6. O que deveria virar Composer? `Input Composer`: aplica PlayerInput, player slot, activation target, bridge target, pause keyboard trigger e UI module bindings.
7. O que deveria ser materializado em `_Framework/_Bindings`? Bridge/activation/gate targets e pause input trigger quando derivados de um Input Profile.
8. O que precisa de runtime authority? `Input Runtime Context`: active action map per player slot, pause/UI override, acceptance/gate state.
9. O que hoje so e provado por validator/smoke? Player action map existe, bridge/activation apontam para PlayerInput correto, pause keyboard usa map/action correto.
10. O que deveria migrar para o package oficial? Input Profile + Player Input Composer + Pause Input Composer.

Severidade: Alta. Input e um dos pontos onde a UX tecnica mais aparece.

### Content / Anchors

Evidencia principal:

- `FirstGameCameraAnchors` com `FrameworkCameraAnchorHost`.
- `ActivityA_ContentRoot`, `ActivityB_ContentRoot`, `ActivityC_RouteFallback_ContentRoot`, `ActivityD_StopBgm_ContentRoot`.
- `RouteContentBinding` nos roots de camera/audio.
- `ActivityLocalVisibilityAdapter` nos content roots.

1. Um designer conseguiria criar isso do zero? Parcialmente. Ele criaria roots e conteudo, mas nao saberia quais bindings/adapters aplicar ou como nomear IDs locais.
2. O Inspector e compreensivel? Content roots sao claros; `localContentId`, anchors e adapters sao tecnicos.
3. O que esta misturado entre gameplay e framework? Conteudo de activity, audio, camera, visibility e route ownership ficam no mesmo GameObject.
4. Quais componentes tecnicos poluem o GameObject? `RouteContentBinding`, `ActivityLocalVisibilityAdapter`, `FrameworkCameraAnchorHost`, activity camera/BGM bindings.
5. O que deveria virar Recipe? `Activity Content Recipe`: visible group, camera, audio, anchors, reset scope.
6. O que deveria virar Composer? `Content Composer`: cria content root por Activity/Route e materializa adapters necessarios.
7. O que deveria ser materializado em `_Framework/_Bindings`? Content bindings, visibility adapters, anchor hosts e per-activity service bindings.
8. O que precisa de runtime authority? `Content Runtime Context`: active route content, active activity content, anchors resolvidos e visibility state.
9. O que hoje so e provado por validator/smoke? Activity A/B/C/D visibility, route fallback camera, retained activity camera e BGM policies.
10. O que deveria migrar para o package oficial? Composer de content roots e anchor binding; nao a estrutura exata de roots do FIRSTGAME.

Severidade: Media-Alta. O conteudo esta organizado, mas a autoria ainda depende de saber adapters.

### Pause / Loading / Transition

Evidencia principal:

- `FG_UIGlobal.unity`.
- `UnityFadeCurtainEffectAdapter`, `UnityLoadingSurfaceAdapter`, `UnityPauseResidentSurfaceAdapter`.
- `PauseRequestTrigger` em botoes e `PauseInputActionTrigger` para teclado.
- `FG_GameApplication.asset` referencia `globalUiScenePath: Assets/_Project/Scenes/Menu/FG_UIGlobal.unity`.

1. Um designer conseguiria criar isso do zero? Parcialmente. Conseguiria montar UI, mas nao saberia conectar surfaces/triggers sem template.
2. O Inspector e compreensivel? Visualmente sim; componentes `Unity Loading Surface Adapter`, `Unity Pause Resident Surface Adapter` e `UnityFadeCurtainEffectAdapter` ainda sao termos tecnicos.
3. O que esta misturado entre gameplay e framework? A cena global UI mistura canvas visual, transition effect, loading surface, pause surface, pause controls e input trigger.
4. Quais componentes tecnicos poluem o GameObject? `UnityFadeCurtainEffectAdapter`, `UnityLoadingSurfaceAdapter`, `UnityPauseResidentSurfaceAdapter`, `PauseRequestTrigger`, `PauseInputActionTrigger`.
5. O que deveria virar Recipe? `Global UI Recipe`: pause surface, loading surface, transition surface, pause controls e input action.
6. O que deveria virar Composer? `Global UI Composer`: cria/valida `FG_UIGlobal` equivalente, surfaces e triggers.
7. O que deveria ser materializado em `_Framework/_Bindings`? Surface adapters e input triggers derivados do Global UI Recipe.
8. O que precisa de runtime authority? `Global UI Runtime Context`: pause state, loading state/progress, active transition effect e input mode.
9. O que hoje so e provado por validator/smoke? Logs `Loading surface resolved`, `Pause surface resolved`, `Transition surface resolved`, pause via botao/input e progress visual.
10. O que deveria migrar para o package oficial? Template/Composer de global UI e surface profiles. Nao migrar o visual FIRSTGAME.

Severidade: Media. E uma area mais proxima de produto, mas ainda falta wizard/template.

### Save / Preferences / Progression

Evidencia principal:

- Busca em `Assets/_Project` nao encontrou implementacao de Save/Preferences/Progression.
- A documentacao marca `save/progression` como explicitamente fora de escopo.
- Existem elementos de `LoadingSurface` com progress visual, mas isso nao e progression/save.

1. Um designer conseguiria criar isso do zero? Nao se depender do framework atual demonstrado no FIRSTGAME; nao ha fluxo provado.
2. O Inspector e compreensivel? Nao aplicavel; nao ha surface.
3. O que esta misturado entre gameplay e framework? Nada ativo; o risco e futuro.
4. Quais componentes tecnicos poluem o GameObject? Nenhum identificado.
5. O que deveria virar Recipe? `Preferences Recipe`, `Save Slot Recipe`, `Progression Recipe`, somente depois de conceito publico.
6. O que deveria virar Composer? `Save/Preferences Composer`, quando houver runtime authority.
7. O que deveria ser materializado em `_Framework/_Bindings`? Storage bindings, save slot UI bindings e migration diagnostics, se o package formalizar isso.
8. O que precisa de runtime authority? `Save Runtime Context` / `Preferences Runtime Context` com lifecycle, storage policy e error reporting.
9. O que hoje so e provado por validator/smoke? Nada no FIRSTGAME.
10. O que deveria migrar para o package oficial? Nada ainda a partir do FIRSTGAME; primeiro definir produto.

Severidade: Media por ausencia. Nao bloquear Player/Route/Camera, mas nao vender como coberto.

## 4. Pontos onde FIRSTGAME depende demais de setup manual

- Player binding depende de IDs fixos: `firstgame.player`, `player.1`, action map `Player`.
- Camera setup depende do script `FirstGameCameraCutSetup.cs`, com paths fixos de cenas/assets e nomes de roots/rigs.
- Routes/Activities sao assets manuais com profiles vazios, e a cena carrega os bindings concretos.
- UI buttons chamam triggers framework diretamente e precisam de `reason` manual.
- Reset depende de subject adapters, participants, scopes, group IDs e reasons configurados em cena/prefab.
- Global UI depende de cena separada referenciada por path em `FG_GameApplication.asset`.
- Audio BGM tem cue assets, mas a ligacao route/activity -> BGM esta em bindings de cena.
- O README ensina smoke e logs esperados, mas nao um fluxo "Create > Configure > Apply > Play".

## 5. Pontos onde validators substituem UX

- `Validate Real Player Binding` prova que a cadeia existe, mas nao ajuda o designer a cria-la.
- Ferramentas locais antigas de player binding provavam references e repair, mas nao eram surface oficial do package.
- O `PlayerComposer` oficial substitui o proto-Composer local como authoring principal.
- A prova local antiga de repair era smoke tecnico demais para UX principal.
- `Configure Route-Activity Camera` faz papel de Composer, mas vive no consumidor e usa paths/nomeclatura FIRSTGAME.
- A documentacao de Player Binding lista logs esperados e failure reasons; isso e diagnostico, nao fluxo de criacao.

## 6. Componentes tecnicos que deveriam ser escondidos/materializados

Materializar em `_Framework/_Bindings` ou esconder em Advanced/Debug:

- `PlayerControlBindingTargetBehaviour`
- `UnityPlayerInputBridgeTargetBehaviour`
- `UnityPlayerInputActivationTargetBehaviour`
- `UnityPlayerInputGateAdapter`
- `PlayerSlotOccupancy`
- `UnityResetSubjectAdapter`
- `UnityTransformResetParticipant`, quando gerado por Recipe
- `FrameworkCameraAnchorHost`
- `FrameworkRouteCameraBinding`
- `FrameworkActivityCameraBinding`
- `FrameworkCameraDirector`
- `FrameworkCinemachineRigApplier`
- `RouteContentBinding`
- `ActivityLocalVisibilityAdapter`
- `FrameworkRouteBgmBinding`
- `FrameworkActivityBgmBinding`
- `AudioRuntimeHost`, quando for infraestrutura
- `UnityFadeCurtainEffectAdapter`
- `UnityLoadingSurfaceAdapter`
- `UnityPauseResidentSurfaceAdapter`
- `PauseInputActionTrigger`, quando derivado de Input/Profile

Componentes que podem continuar visiveis se renomeados/empacotados como authoring:

- Route request control em UI.
- Activity request control em UI.
- Reset player/room control em UI.
- Pause control em UI.
- Player gameplay component (`FirstGamePlayerMover` ou equivalente do jogo).

## 7. Candidatos a Recipe/Composer no package

### Recipes / Profiles

- `Game Application Recipe`: startup route, global UI, validation policy, logging profile.
- `Route Recipe`: scene, startup activity, transition gate, route content groups, route BGM, route camera.
- `Activity Recipe`: content root, visibility policy, activity BGM, activity camera, reset policy.
- `Player Recipe`: actor id, player slot, PlayerInput action maps, camera target, reset subject scope.
- `Input Recipe`: gameplay map, UI map, pause action, slot activation/gate policy.
- `Camera Recipe`: route rig, activity rig, anchor source, priority, fallback/retain policy.
- `Resettable Object Recipe`: subject id policy, scope, participants, transform reset flags.
- `Global UI Recipe`: loading surface, pause surface, transition surface, input bindings.
- `BGM Recipe`: route/activity BGM mapping and stop/fallback policy.

### Composers / Authoring

- `Game Application Composer`: cria settings + application asset + global UI binding.
- `Route Composer`: cria RouteAsset, scene binding, route content root e optional UI trigger.
- `Activity Composer`: cria ActivityAsset, content root, visibility/audio/camera bindings.
- `Player Composer`: cria player bindings, input bridge, reset subject, camera anchors.
- `Camera Composer`: cria route/activity rigs, anchors e director bindings.
- `Reset Composer`: aplica subject/participants a scene objects e prefabs.
- `Global UI Composer`: cria pause/loading/transition surfaces e pause input.
- `Audio Composer`: aplica BGM director + route/activity BGM bindings.

Regra: esses Composers nao devem copiar nomes ou paths FIRSTGAME. Devem receber parametros e gerar materializacao idempotente.

## 8. Candidatos a Runtime Context / Session

- `Route Runtime Context`: current route, loaded scene, transition gate, route content state.
- `Activity Runtime Context`: current activity, startup activity, restart/clear/reenter authority.
- `Player Runtime Context`: current actor/player slot, input activation state, camera target, reset subject.
- `Input Runtime Context`: action map ownership, pause/UI override, input gate acceptance.
- `Camera Runtime Context`: active route rig, active activity rig, retained activity camera, anchor set.
- `Reset Runtime Context`: reset subjects, participants, scope filtering, runtime-spawned subject lifecycle.
- `Global UI Runtime Context`: pause/loading/transition surfaces and current state.
- `Audio Runtime Context`: route/activity BGM current cue, fallback/stop policy, fade state.

Nao criar uma `Session` generica agora. O FIRSTGAME aponta para contextos escopados por dominio. Se uma Session oficial surgir, ela deve orquestrar contexts tipados, nao substituir ownership de cada modulo.

## 9. Sugestao de ordem de produto, sem implementar

1. Formalizar linguagem publica: `Game Application`, `Route`, `Activity`, `Player`, `Global UI`, `Camera`, `Resettable Object`, `Input Profile`.
2. Criar `Player Composer` oficial primeiro, porque Player e o maior vazamento tecnico e desbloqueia Input, Camera anchors e Reset.
3. Criar `Route/Activity Composer` com Recipes/Profiles, substituindo `routeContentProfile` / `activityContentProfile` vazios por uso real.
4. Criar `Camera Composer` oficial, absorvendo o papel do `FirstGameCameraCutSetup` sem paths FIRSTGAME.
5. Criar `Global UI Recipe/Composer` para Pause, Loading e Transition.
6. Criar `Reset Composer` para subject/participant setup em scene object e prefab.
7. Criar `Input Profile` + `Input Composer`, reduzindo bridge/gate/activation targets visiveis.
8. Mover validators atuais para Diagnostics: eles devem confirmar Apply/Rebuild, nao ser o caminho principal.
9. Materializar bindings em `_Framework/_Bindings` com nomes de dominio e Advanced/Debug para detalhes.
10. So depois avaliar Save/Preferences/Progression; o FIRSTGAME ainda nao prova esse dominio.

## O que nao transformar em definitivo

- Nao migrar paths `Assets/_Project/...` para o package.
- Nao copiar IDs `firstgame.*` ou nomes `FG_*`.
- Nao copiar `FirstGameCameraCutSetup` como implementation oficial.
- Nao transformar ferramentas locais antigas de FIRSTGAME em API publica sem redesign de produto.
- Nao usar validators/proofs como workflow oficial.
- Nao criar uma Session generica para esconder falta de ownership.
- Nao mover assets de consumidor para package.

## Validacao realizada

- Leitura estatica de `Assets/_Project`.
- Inspecao de scripts runtime/editor, cenas, prefab, assets, settings e documentacao.
- Nao foi executado Unity, Play Mode, build, smoke ou batchmode.

## Checklist manual recomendado

- Abrir `FG_Menu` e confirmar se um usuario entende o fluxo sem rodar menus de validacao.
- Abrir `FG_Gameplay` e inspecionar `PlayerPrototype`; contar quantos componentes sao intencao de jogo versus materializacao tecnica.
- Abrir `FirstGameCameraRoot` e `FirstGameCameraAnchors`; validar se a camera e authoravel sem `Configure Route-Activity Camera`.
- Abrir `FG_UIGlobal`; validar se Pause/Loading/Transition podem ser recriados por template.
- Tentar criar uma nova Activity E manualmente; registrar quais componentes/IDs/references o designer precisa conhecer.
- Tentar criar um novo resettable prefab; registrar se subject/participant/scope/idGeneration sao compreensiveis.
