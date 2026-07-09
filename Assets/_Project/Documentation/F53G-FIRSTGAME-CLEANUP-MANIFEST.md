# F53G - FIRSTGAME Cleanup Manifest

Status: Ready for Unity validation
Date: 2026-07-09
Scope: FIRSTGAME-only (`Assets/_Project`)

## Objetivo

Limpar o FIRSTGAME depois da sequencia F53C1-F53F, removendo tools temporarios, validators intermediarios, docs obsoletas e menus redundantes que foram substituidos pela facade canonica.

## Escopo

Auditado e alterado:

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration
Assets/_Project/Scripts/Editor/GameCamera
Assets/_Project/Documentation
```

Verificado sem alterar:

```text
Assets/_Project/Scripts/FrameworkProof
Assets/_Project/Scripts/Editor/FrameworkProof
Assets/_Project/ScriptableObjects/ImmersiveFramework
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
Assets/_Project/Scenes/Menu/FG_Menu.unity
```

## Fora De Escopo

```text
Packages/com.immersive.framework
QA Harness
runtime lifecycle
movement
InputAction routing
gameplay command execution
actor spawning
save/progression
formal PlayerView contracts
scene visual restructuring
```

## Arquivos Criados

```text
Assets/_Project/Documentation/F53G-FIRSTGAME-CLEANUP-MANIFEST.md
Assets/_Project/Documentation/F53G-FIRSTGAME-CLEANUP-MANIFEST.md.meta
```

## Arquivos Alterados

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameRealPlayerBindingValidator.cs
Assets/_Project/Documentation/F53F-Player-Binding-Operational-Guide.md
```

## Arquivos Removidos

```text
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameRealPlayerCameraTargetValidator.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameRealPlayerCameraTargetValidator.cs.meta
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameFullPlayerBindingChainValidator.cs
Assets/_Project/Scripts/Editor/FrameworkIntegration/FirstGameFullPlayerBindingChainValidator.cs.meta
Assets/_Project/Documentation/F53B-Real-Player-Binding-Wiring.md
Assets/_Project/Documentation/F53B-Real-Player-Binding-Wiring.md.meta
Assets/_Project/Documentation/F53C0-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53C0-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53C0-Player-Identity-Typed-Binding-Audit.md
Assets/_Project/Documentation/F53C0-Player-Identity-Typed-Binding-Audit.md.meta
Assets/_Project/Documentation/F53C1-B1-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53C1-B1-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53C1-B2-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53C1-B2-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53C1-B3-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53C1-B3-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53C1-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53C1-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53C1-Player-Identity-Canonicalization.md
Assets/_Project/Documentation/F53C1-Player-Identity-Canonicalization.md.meta
Assets/_Project/Documentation/F53C2-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53C2-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53C2-Real-Player-Camera-Target-Proof.md
Assets/_Project/Documentation/F53C2-Real-Player-Camera-Target-Proof.md.meta
Assets/_Project/Documentation/F53C3-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53C3-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53C3-Full-Player-Binding-Chain-Proof.md
Assets/_Project/Documentation/F53C3-Full-Player-Binding-Chain-Proof.md.meta
Assets/_Project/Documentation/F53D-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53D-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53D-Canonical-Player-Binding-Authoring-Facade.md
Assets/_Project/Documentation/F53D-Canonical-Player-Binding-Authoring-Facade.md.meta
Assets/_Project/Documentation/F53E-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53E-FIRSTGAME-DELTA-MANIFEST.md.meta
Assets/_Project/Documentation/F53E-Player-Binding-Facade-Repair-Proof.md
Assets/_Project/Documentation/F53E-Player-Binding-Facade-Repair-Proof.md.meta
Assets/_Project/Documentation/F53F-FIRSTGAME-DELTA-MANIFEST.md
Assets/_Project/Documentation/F53F-FIRSTGAME-DELTA-MANIFEST.md.meta
```

## Menus Mantidos

```text
FIRSTGAME > Immersive Framework > Validate Real Player Binding
FIRSTGAME > Immersive Framework > Configure Route-Activity Camera
FIRSTGAME > Immersive Framework > Validate Canonical Player Binding Facade
FIRSTGAME > Immersive Framework > Apply Canonical Player Binding Facade
FIRSTGAME > Immersive Framework > Run Canonical Player Binding Facade Repair Proof
```

## Menus Removidos Ou Movidos

Removidos:

```text
FIRSTGAME > Immersive Framework > Ensure Selected Player Binding Components
FIRSTGAME > Immersive Framework > Cleanup F53A Preflight Proof Assets
FIRSTGAME > Immersive Framework > Validate Real Player Camera Target
FIRSTGAME > Immersive Framework > Validate Full Player Binding Chain
```

Nao havia menu ativo em:

```text
Tools > FIRSTGAME > ...
```

## Itens Analisados E Mantidos

| Item | Classificacao | Motivo |
| --- | --- | --- |
| `FirstGamePlayerIdentityResolver.cs` | KEEP_CANONICAL | Fonte editor-only canonica para resolver o player por `PlayerActorDeclaration`, `PlayerSlotDeclaration` e `PlayerInput`, sem `GameObject.name`. |
| `FirstGameCanonicalPlayerBindingAuthoringFacade.cs` | KEEP_CANONICAL | Valida e aplica a cadeia canonica completa: identity, F52 targets e camera anchors. |
| `FirstGameCanonicalPlayerBindingFacadeRepairProof.cs` | KEEP_CANONICAL | Smoke operacional do repair path exigido pelo corte. |
| `FirstGameRealPlayerBindingValidator.cs` | KEEP_SUPPORT | Smoke rapido de suporte para F52 real player binding; menus de apply/cleanup foram removidos. |
| `FirstGameCameraCutSetup.cs` | KEEP_CANONICAL | Tool canonico para configurar camera route/activity sob `FIRSTGAME > Immersive Framework`. |
| `F53F-Player-Binding-Operational-Guide.md` | KEEP_CANONICAL | Guia operacional unico depois da limpeza F53G. |
| `Documentation/Camera/POST-RESET-H2-FIRSTGAME-Camera-Route-Activity.md` | KEEP_SUPPORT | Documento de camera route/activity fora da cadeia F53 player binding. |
| `ScriptableObjects/ImmersiveFramework` | KEEP_CANONICAL | Assets reais de Route/Activity/GameApplication; sem lixo F53. |
| `FG_Gameplay.unity` / `FG_Menu.unity` | KEEP_CANONICAL | Cenas reais do FIRSTGAME; nenhuma referencia quebrada exigiu edicao manual. |

## Itens Analisados E Removidos

| Item | Classificacao | Motivo |
| --- | --- | --- |
| `FirstGameRealPlayerCameraTargetValidator.cs` | REMOVE_OBSOLETE | Validator F53C2 intermediario; a facade valida os mesmos camera anchors. |
| `FirstGameFullPlayerBindingChainValidator.cs` | REMOVE_OBSOLETE | Validator F53C3 intermediario; substituido por `Validate Canonical Player Binding Facade`. |
| F53B-F53E docs e manifests | REMOVE_OBSOLETE | Conteudo operacional consolidado em F53F e neste F53G manifest. |
| F53C1-B1/B2/B3 manifests | REMOVE_OBSOLETE | Hotfix history removido da navegacao ativa; Git history preserva detalhe. |
| `F53F-FIRSTGAME-DELTA-MANIFEST.md` | REMOVE_OBSOLETE | Substituido pelo manifesto F53G como inventario atual. |
| `Ensure Selected Player Binding Components` | REMOVE_OBSOLETE | Facade apply cobre repair/configuracao canonica. |
| `Cleanup F53A Preflight Proof Assets` | REMOVE_TEMPORARY | Pastas/assets F53A temporarios nao existem mais. |

## Itens Que Precisam Revisao Manual

```text
none
```

## Smoke Esperado

Depois do import/compile Unity, abrir:

```text
Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity
```

Rodar:

```text
FIRSTGAME > Immersive Framework > Validate Canonical Player Binding Facade
```

Esperado:

```text
[F53D_FIRSTGAME_PLAYER_BINDING_FACADE] status='Succeeded'
resolvedByName='False'
facadeCentralizedReferences='True'
failureReason='None'
```

Rodar:

```text
FIRSTGAME > Immersive Framework > Run Canonical Player Binding Facade Repair Proof
```

Esperado:

```text
[F53E_FIRSTGAME_PLAYER_BINDING_FACADE_REPAIR_PROOF] status='Succeeded'
repairSucceeded='True'
typedReferencesRepaired='True'
failureReason='None'
```

Suporte opcional mantido:

```text
FIRSTGAME > Immersive Framework > Validate Real Player Binding
```

Esperado:

```text
[F53B_FIRSTGAME_REAL_PLAYER_BINDING] status='Succeeded'
resolvedByName='False'
failureReason='None'
```

## Criterios De Aceite

```text
1. Unity compila.
2. Nenhum runtime novo foi criado.
3. Nenhum movement/input routing/gameplay/spawning/save foi criado.
4. Menus FIRSTGAME estao consolidados.
5. Tools temporarios/duplicados foram removidos ou justificados.
6. Docs obsoletas foram removidas ou consolidadas no F53F/F53G.
7. Facade validate continua PASS.
8. Facade repair proof continua PASS.
9. Nenhum lookup funcional por GameObject.name voltou.
10. Manifest explica exatamente o que foi mantido/removido e por que.
```

## Ganho Arquitetural

- FIRSTGAME fica com um fluxo operacional curto e focado na facade canonica.
- A cadeia de player continua baseada em `PlayerActorDeclaration`, `PlayerSlotDeclaration`, `PlayerInput` e referencias tipadas.
- Menus intermediarios nao competem mais com o caminho canonico.
- Docs de proof/hotfix nao poluem a navegacao ativa.

## Commit Message Sugerida

```text
F53G: clean up FIRSTGAME player binding tooling
```
