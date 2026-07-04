# POST-RESET-B2 - FIRSTGAME Naming Cleanup

## Objetivo

Limpar nomes temporarios no FIRSTGAME sem quebrar serializacao Unity.

O corte separa:

- renomes seguros;
- movimentos seguros com `.meta` preservado;
- renomes que devem ser feitos via Unity Editor porque a cena referencia o script ou GameObject.

## Arquivos auditados

| Arquivo | GUID / referencia | Decisao |
|---|---|---|
| `Assets/_Project/Scripts/FirstGamePlayerMover.cs` | GUID `20cf0f842d0ff7845abc6ca6bfcef9da`; referenciado por `FG_Gameplay.unity`. | Movido com `.meta` preservado para `Scripts/Runtime/Player/`. |
| `Assets/_Project/Scripts/FirstGamePlayerResetProbe.cs` | GUID `7d31b2df5fc0f6645894005132ffaa27`; referenciado por `FG_Gameplay.unity`. | Movido com `.meta` preservado; rename deferido. |
| `Assets/_Project/Scripts/FirstGameRuntimeObjectSpawner.cs` | GUID `cf027eda0e046694c896a3779a70303f`; referenciado por `FG_Gameplay.unity`. | Movido com `.meta` preservado para `Scripts/Runtime/RuntimeObjects/`. |
| `Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity` | Contem GameObject `TestProb` e `m_EditorClassIdentifier` de `FirstGamePlayerResetProbe`. | Nao alterado automaticamente. |
| `Assets/_Project/Prefabs/FG_RuntimeBox.prefab` | Contem `LightProbe`/`ReflectionProbe` fields Unity. | Falso positivo; nao alterar. |
| `Assets/Settings/*.asset` | Contem `ProbeVolume`, `CopyPasteTestComponent*` e campos internos URP. | Falso positivo Unity/package settings; nao alterar. |

## Renomes feitos

Nenhum rename de classe, arquivo de script ou GameObject foi feito.

Motivo: o nome temporario real `FirstGamePlayerResetProbe` esta anexado em `FG_Gameplay.unity`. Renomear classe/arquivo fora do Unity Editor pode causar Missing Script.

## Movimentos feitos

| Origem | Destino |
|---|---|
| `Assets/_Project/Scripts/FirstGamePlayerMover.cs` | `Assets/_Project/Scripts/Runtime/Player/FirstGamePlayerMover.cs` |
| `Assets/_Project/Scripts/FirstGamePlayerMover.cs.meta` | `Assets/_Project/Scripts/Runtime/Player/FirstGamePlayerMover.cs.meta` |
| `Assets/_Project/Scripts/FirstGamePlayerResetProbe.cs` | `Assets/_Project/Scripts/Runtime/Player/FirstGamePlayerResetProbe.cs` |
| `Assets/_Project/Scripts/FirstGamePlayerResetProbe.cs.meta` | `Assets/_Project/Scripts/Runtime/Player/FirstGamePlayerResetProbe.cs.meta` |
| `Assets/_Project/Scripts/FirstGameRuntimeObjectSpawner.cs` | `Assets/_Project/Scripts/Runtime/RuntimeObjects/FirstGameRuntimeObjectSpawner.cs` |
| `Assets/_Project/Scripts/FirstGameRuntimeObjectSpawner.cs.meta` | `Assets/_Project/Scripts/Runtime/RuntimeObjects/FirstGameRuntimeObjectSpawner.cs.meta` |

Os GUIDs foram preservados.

## Renomes deferidos para Unity Editor

| Atual | Recomendado | Onde esta referenciado | Instrucao |
|---|---|---|---|
| `FirstGamePlayerResetProbe` | `FirstGamePlayerResettableState` | `Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity`, GUID `7d31b2df5fc0f6645894005132ffaa27`, `m_EditorClassIdentifier: Assembly-CSharp::_Project.Scripts.FirstGamePlayerResetProbe` | Renomear via Unity Editor/IDE com Unity aberto, confirmar que o componente nao vira Missing Script e salvar a cena. |
| GameObject `TestProb` | Nome semantico de player/reset state, por exemplo `PlayerResettableState` ou `PlayerRuntimeState` | `Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity` | Renomear o GameObject no Hierarchy pelo Unity Editor e salvar a cena. |

## Riscos evitados

- Nao houve troca manual de GUID.
- Nao houve rename de MonoBehaviour anexado em cena.
- Nao houve edicao de cenas/prefabs/assets serializados.
- Nao houve alteracao de runtime do framework, QA project ou ProjectSettings.

## Ajuste pos-import

Depois que os scripts foram movidos para dentro de `Assets/_Project/Scripts/Runtime/`, `FirstGamePlayerMover` passou a compilar dentro de `Project.Runtime.asmdef`.

`Project.Runtime.asmdef` foi atualizado para referenciar `Unity.InputSystem`, porque `FirstGamePlayerMover` usa:

- `UnityEngine.InputSystem.PlayerInput`;
- `UnityEngine.InputSystem.InputAction`;
- `UnityEngine.InputSystem.InputActionMap`.

O package `com.unity.inputsystem` ja esta declarado em `Packages/manifest.json`.

## Validacao textual

Buscas obrigatorias em `C:\Projetos\planet-devourer\Assets`:

| Busca | Classificacao |
|---|---|
| `Probe` | `deferred manual` para `FirstGamePlayerResetProbe` e `TestProb`; `false positive` para `LightProbe`, `ReflectionProbe`, `ProbeVolume`. |
| `Test` | `deferred manual` para `TestProb`; `false positive` para `ZTest`, `CopyPasteTestComponent*`, `VolumeComponent*Tests` e textos de README/relatorio. |
| `Smoke` | `accepted dev-only name` apenas em README/relatorio explicando a regra; nenhum runner de smoke encontrado no FIRSTGAME. |
| `QA_` | `accepted dev-only name` apenas em README/relatorio explicando a regra; nenhum asset `QA_` encontrado no FIRSTGAME. |
| Framework QA root literal | Sem ocorrencias em assets/codigo do FIRSTGAME. |

## Proximo smoke manual recomendado

No Unity Editor, depois de import/compile:

1. Abrir `Assets/_Project/Scenes/Menu/FG_Menu.unity`.
2. Entrar no fluxo para `FG_Gameplay`.
3. Confirmar que o player continua com `FirstGamePlayerMover`.
4. Spawnar runtime boxes.
5. Usar Reset Room.
6. Usar Restart Activity.
7. Confirmar que nenhum componente aparece como Missing Script em `FG_Gameplay.unity`.
