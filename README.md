# FIRSTGAME — Lifecycle Billboard

## Conteúdo

- `Assets/_Project/Prefabs/UI.meta`
- `Assets/_Project/Prefabs/UI/PF_LifecycleBillboard.prefab`
- `Assets/_Project/Prefabs/UI/PF_LifecycleBillboard.prefab.meta`
- `Patches/Sample_Environment.LifecycleBillboard.patch`

## Escopo

Este corte cria somente a apresentação estática do billboard:

- World Space Canvas;
- painel e espaçamento;
- campos separados para Scene, Route, Activity e Last Event;
- textos iniciais neutros (`NOT REPORTED` / `WAITING FOR CALLBACK`);
- nenhum script;
- nenhum componente de lifecycle;
- nenhuma configuração automática de Route ou Activity.

## Hierarquia

```text
LifecycleBillboard
└── Panel
    ├── AccentBar
    ├── Title
    ├── Subtitle
    ├── SceneLabel
    ├── SceneStatusValue
    ├── RouteLabel
    ├── RouteStatusValue
    ├── ActivityLabel
    ├── ActivityStatusValue
    ├── Divider
    ├── LastEventLabel
    └── LastEventValue
```

Os nomes `SceneStatusValue`, `RouteStatusValue`, `ActivityStatusValue` e `LastEventValue` foram preservados como pontos explícitos para a futura ligação do presenter.

## Aplicação

A partir da raiz do projeto `planet-devourer`:

```bash
git apply --check Patches/Sample_Environment.LifecycleBillboard.patch
git apply Patches/Sample_Environment.LifecycleBillboard.patch
```

Depois, copie a pasta `Assets` deste pacote sobre a pasta `Assets` do projeto antes de abrir a cena no Unity.

A patch foi preparada para a cena:

```text
Assets/_Project/Scenes/RotesContents/Sample_Environment.unity
Git blob: 82ad475ac8983128b19c52d547ea1a4687acc1aa
```

Ela instancia o prefab como filho de `Environment_Root`, na posição local:

```text
Position: (0, 2.4, -6)
Rotation: (0, 0, 0)
Prefab scale: (0.008, 0.008, 0.008)
```

Caso `git apply --check` falhe, a cena mudou desde esse blob. Não force a aplicação; abra o prefab e arraste-o manualmente para `Environment_Root`.

## Validação manual no Unity

1. Abrir `Sample_Environment.unity`.
2. Confirmar `Environment_Root/LifecycleBillboard`.
3. Confirmar que o Canvas está em `World Space`.
4. Verificar legibilidade e ajustar somente posição/rotação conforme a câmera real.
5. Não adicionar lifecycle ou presenter neste corte.

## Commit sugerido

```text
feat(firstgame): add static lifecycle status billboard
```
