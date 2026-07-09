# FIRSTGAME PlayerComposer Pilot InputSystem Reference Fix Manifest

Status: Compile fix
Date: 2026-07-09
Project: `planet-devourer` / FIRSTGAME

## Corte

Correcao pequena do piloto FIRSTGAME para evitar dependencia direta do assembly `Unity.InputSystem` no script editor local.

## Objetivo

Corrigir erro de compilacao causado por `using UnityEngine.InputSystem` em um assembly editor do FIRSTGAME que nao referencia diretamente `Unity.InputSystem`.

## Arquivos criados

```text
Assets/_Project/Documentation/Product/FIRSTGAME-PLAYER-COMPOSER-PILOT-INPUTSYSTEM-REFERENCE-FIX-MANIFEST.md
```

## Arquivos alterados

```text
Assets/_Project/Scripts/Editor/ImmersiveFramework/FirstGamePlayerComposerPilot.cs
```

## Arquivos removidos

```text
none
```

## Decisao

O helper FIRSTGAME nao deve depender diretamente de `UnityEngine.InputSystem.PlayerInput`.

Ele resolve o componente por tipo em runtime/editor:

```text
UnityEngine.InputSystem.PlayerInput, Unity.InputSystem
```

E configura o `PlayerComposer` oficial via `SerializedObject`.

## Fora de escopo

```text
asmdef
package
QAFramework
FIRSTGAME gameplay
PlayerComposer package
facades antigas
smoke
runtime
```

## Criterios de aceite

Este corte e PASS se:

- o erro `UnityEngine.InputSystem does not exist in namespace UnityEngine` desaparecer;
- o erro `PlayerInput could not be found` desaparecer;
- o menu FIRSTGAME continuar configurando o `PlayerComposer` oficial;
- nenhuma facade local virar API oficial;
- nenhum asmdef for alterado.

## Commit message sugerida

```text
FIRSTGAME: remove direct InputSystem dependency from PlayerComposer pilot
```
