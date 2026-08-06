# FIRSTGAME — Immersive Framework Consumer Demo

**Status:** integração ativa como consumidor real  
**Última atualização:** 2026-08-06  
**Unity:** 6.5

`planet-devourer` é o consumidor real de `com.immersive.framework`.

O projeto demonstra como um usuário encontra, configura, executa e diagnostica as superfícies do framework em um jogo Unity real. Ele não é a fonte oficial dos contratos do framework e não deve criar autoridades paralelas para contornar superfícies ainda ausentes.

## Fontes oficiais

```text
Framework / produto
  ImmersiveGames/com.immersive.framework

QA técnico
  rinnocenti/QAFramework

Consumidor real
  ImmersiveGames/planet-devourer
```

Responsabilidades:

```text
com.immersive.framework
  runtime, contracts, authoring, tooling, validators, docs e APIs oficiais

QAFramework
  smokes técnicos, casos negativos, regressões e matrizes contratuais

FIRSTGAME
  criação manual, integração real, legibilidade, usabilidade e fluxo de jogo
```

Não restaurar scripts legados, copiar fixtures do QA, copiar `ProjectSettings` de projetos anteriores ou criar facades locais que substituam contratos do package.

# Organização da documentação

A documentação ativa fica próxima da composição que descreve:

```text
Assets/_Project/README.md
  visão global, estado dos modelos, limites e índice

Assets/_Project/Demo01/README.md
  M01, M02 e M03: uso, configuração, validação e findings

Assets/_Project/Demo02/README.md
  M06 e M07: uso, configuração, validação e findings
```

Regras:

```text
um README por demonstração;
status global somente neste README;
fluxo de uso e findings no README da demonstração;
sem cópias paralelas do mesmo estado em Documentation/;
planos extensos e ADRs pertencem ao package oficial;
matrizes negativas pertencem ao QAFramework;
evidência de consumidor permanece resumida no README da demo.
```

Um documento separado só deve existir quando tiver lifetime próprio e não puder ser mantido sem duplicar o README, por exemplo uma especificação formal, ADR oficial ou relatório externo congelado.

# Estado atual dos modelos

| Modelo | Demonstração | Estado |
|---|---|---|
| M01 | Route and Activity | Demonstrado na Demo 01 |
| M02 | Lifecycle Events | Demonstrado na Demo 01 |
| M03 | Activity Readiness | Demonstrado na Demo 01 |
| M04 | Content Anchors | Revogado da sequência atual |
| M05 | Anchor Materialization | Revogado da sequência atual |
| M06 | Scene-Provided Logical Player | Baseline consumidor demonstrado na Demo 02 |
| M07 | Manager-Provisioned Logical Player | Baseline consumidor demonstrado na Demo 02 |
| M08 | Participation Policies | Ainda não montado como demonstração dedicada |

Use `Demonstrado` somente quando houver uma jornada de Play Mode registrada no README correspondente.

# Índice das demonstrações

## Demo 01 — Routes, Activities, Lifecycle e Readiness

Documenta:

```text
M01 Route and Activity
M02 Lifecycle Events
M03 Activity Readiness
```

Inclui:

```text
ownership Route-owned e Activity-owned;
callbacks de Scene, Route e Activity;
ObserveOnly, WaitVisible e WaitCovered;
Required e Optional participants;
Loading determinístico por readiness;
saída, cleanup e reentrada;
findings de authoring e diagnóstico.
```

Consulte [Demo01/README.md](Demo01/README.md).

## Demo 02 — Provisioned Players

Documenta:

```text
M06 Scene-Provided Logical Player
M07 Manager-Provisioned Logical Player
```

Inclui:

```text
Player Slot, Local Player Host e Actor Mount;
Actor scene-owned versus manager-provisioned;
admission e default Actor;
Joining fechado/aberto;
Activity Restart;
Route exit/reentry com estado de Session preservado;
validação manual e findings de produto.
```

Consulte [Demo02/README.md](Demo02/README.md).

# Vocabulário de status

```text
Present in Git
  assets, cenas ou código existem no repositório

Authoring Ready
  referências authoring e validação estática estão configuradas

Runtime Implemented
  o package contém o caminho runtime oficial

Consumer Demonstrated
  uma jornada FIRSTGAME foi executada e registrada

QA Proven
  o QAFramework prova contratos e casos negativos

Blocked by Framework
  o fluxo desejado ainda não possui superfície oficial no package
```

`Consumer Demonstrated` não substitui `QA Proven`.

# Limites atuais de Player

Implementado e usado no FIRSTGAME:

```text
Scene-Provided Player
Manager-Provisioned Player
Session-scoped Slot participation
configured default Actor
contextual Actor preparation/materialization
Activity restart and reentry
Route exit and reentry
```

Ainda sem fluxo completo de produto:

```text
Session-Persistent Logical Player
public Session Leave
explicit arbitrary Actor selection
join Actor-less aguardando escolha
Actor replacement after preparation
disconnect/reconnect
multiplayer Camera and Pause policy
```

Não simular essas features com singleton, busca global, manager consumidor ou uso direto de internals do package.

# Critério de manutenção

Ao atualizar uma demonstração:

```text
1. atualizar o README da própria demo;
2. atualizar a tabela global somente se o status do modelo mudou;
3. registrar jornada e limitações sem transformar observação em contrato;
4. encaminhar problema técnico ao QAFramework;
5. encaminhar solução reutilizável ao package;
6. remover texto duplicado ou obsoleto no mesmo corte.
```

# Próximo trabalho de Player

```text
1. manter M06 e M07 como baselines congelados;
2. montar Activity-owned Scene-Provided Player;
3. demonstrar Dynamic Capacity e late join;
4. provar dois Manager-Provisioned Players com devices explícitos;
5. transformar Participation Policies em Activities compreensíveis;
6. definir o corte oficial de seleção explícita de Actor antes da Demo 03 correspondente.
```
