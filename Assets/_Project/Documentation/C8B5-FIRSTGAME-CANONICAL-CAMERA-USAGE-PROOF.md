# C8B5 — FIRSTGAME Canonical Camera Usage Proof

## Objetivo

Provar que o FIRSTGAME usa a superfície oficial `CameraComposer` com `PlayerComposer`, sem criar um fluxo concorrente de câmera por Route/Activity.

## Escopo

- `FirstGameCameraComposerProof`.
- `CameraComposer` configurado para o `PlayerComposer` real da cena Gameplay.
- Materialização Cinemachine idempotente.

## Fora de escopo

- Route/Activity camera no FIRSTGAME.
- `FrameworkCinemachineCameraOutputSource` em gameplay real.
- Alterações no package ou no QA Harness.
- Blends, multiplayer, spectator e fallback por nome.

## Fluxo oficial

```text
FIRSTGAME > Immersive Framework > Camera Composer Proof > Configure Gameplay CameraComposer Proof
```

O designer configura a câmera através de `CameraComposer`; o target vem do `PlayerComposer` e a aplicação é idempotente.

## Decisão de produto

O FIRSTGAME não precisa de Route/Activity camera canônico neste corte. O gameplay atual possui uma única câmera de produto baseada em Player/CameraComposer. Não foi criada integração artificial apenas para exercitar Route/Activity.

## Critérios de aceite técnico

- FIRSTGAME compila.
- CameraComposer Proof retorna `status='Succeeded'`.
- `createdSecond='0'` e `blockedSecond='0'`.
- `resolvedByName='False'`.
- Nenhuma referência ativa a `FrameworkCameraDirector`, `FrameworkCameraAnchorHost`, `FrameworkCinemachineRigApplier` ou `Configure Route-Activity Camera`.
- Nenhum Missing Script nas cenas tocadas.

## Critérios de aceite de produto

- CameraComposer permanece a única superfície de câmera visível no FIRSTGAME.
- Não existe fluxo legacy concorrente.
- Route/Activity camera canônico só será introduzido quando houver necessidade real de produto.

## Ganho arquitetural

O consumidor real deixa de carregar uma integração técnica de Route/Activity sem uso e mantém uma única autoridade de authoring: PlayerComposer → CameraComposer.

## Ganho de usabilidade

O designer encontra um único caminho claro para configurar a câmera de gameplay, sem menus de compatibilidade ou rigs paralelos.
