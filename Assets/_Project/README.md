# FIRSTGAME — Immersive Framework Consumer Demo

**Status:** integração ativa como consumidor real  
**Última atualização:** 2026-08-06  
**Unity:** 6.5

`planet-devourer` é o consumidor real de `com.immersive.framework`.

O projeto demonstra como um usuário encontra, configura, executa e diagnostica superfícies do framework em um jogo Unity real. Ele não é a fonte oficial dos contratos do framework e não deve criar autoridades paralelas para contornar APIs ausentes.

# 1. Responsabilidades

```text
com.immersive.framework
  runtime, contracts, authoring, tooling, validators, docs e APIs oficiais

QAFramework
  smokes técnicos, casos negativos, regressões e matrizes contratuais

FIRSTGAME
  criação manual, integração real, legibilidade, usabilidade e fluxo de jogo
```

Não restaurar scripts legados, copiar fixtures do QA, copiar `ProjectSettings` de projetos anteriores ou criar facades locais que substituam contratos do package.

# 2. Organização da documentação

```text
Assets/_Project/README.md
  visão global, estado das demos, limites e navegação

Assets/_Project/Demo01/README.md
  Routes, Activities, lifecycle e readiness

Assets/_Project/Demo02/README.md
  Player source e physical lifetime

Assets/_Project/Demo03/README.md
  local multiplayer foundations, quando criada

Assets/_Project/Demo04/README.md
  participation scenarios, quando criada
```

Regras:

```text
um README por demonstração;
status global somente neste README;
fluxo de uso e findings no README da demo;
sem cópias paralelas do mesmo estado;
ADRs e contratos oficiais pertencem ao package;
matrizes negativas pertencem ao QAFramework.
```

# 3. Estado das demonstrações

| Demo | Propósito | Estado |
|---|---|---|
| Demo 01 | Routes, Activities, lifecycle e readiness | Fechada |
| Demo 02 | Player source e physical lifetime | Fechada |
| Demo 03 | Local multiplayer foundations | Próxima demo ativa |
| Demo 04 | Participation and Actor-policy scenarios | Planejada após Demo 03 |

# 4. Demo 01 — Routes, Activities, Lifecycle e Readiness

Identificadores históricos:

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

# 5. Demo 02 — Player Source and Physical Lifetime

Status:

```text
DEMO02-MODEL-01 — Scene-Provided Player — Route-Owned — Closed
DEMO02-MODEL-02 — Manager-Provisioned Player — Single Local Player — Closed
DEMO02-MODEL-03 — Scene-Provided Player — Activity-Owned — Closed
DEMO02-MODEL-04 — Player Source and Lifetime Comparison — Closed
```

A Demo 02 compara:

```text
Scene-Provided Route-owned
  Host e Actor pertencem à Route.

Scene-Provided Activity-owned
  Host e Actor pertencem à Activity e são liberados na saída.

Manager-Provisioned
  Join solicita a criação do Host; o Actor é preparado pelo lifecycle.
```

Consulte [Demo02/README.md](Demo02/README.md).

# 6. Demo 03 — Local Multiplayer Foundations

Próximos cortes:

```text
DEMO03-MULTI-01
  Dynamic Capacity and Late Join

DEMO03-MULTI-02
  Two Local Players with Explicit Devices
```

Responsabilidade:

```text
múltiplos Player Slots;
Joining aberto ou fechado;
Dynamic Capacity;
late join;
múltiplos Local Player Hosts;
dispositivos explícitos;
múltiplos Actors físicos;
input independente.
```

Fora do primeiro bloco:

```text
network multiplayer;
split-screen;
multiplayer Camera;
multiplayer Pause;
Session Leave;
disconnect/reconnect.
```

# 7. Demo 04 — Participation Scenarios

Planejamento:

```text
DEMO04-SCENARIO-01
  Activity Participation Policies

DEMO04-SCENARIO-02
  Shared and Unique Actor Defaults
```

A Demo 04 deve consumir a infraestrutura multiplayer comprovada pela Demo 03. Ela não deve recriar admission ou provisioning paralelos.

# 8. Vocabulário de status

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

# 9. Limites atuais de Player

Implementado e demonstrado:

```text
Scene-Provided Player — Route-owned
Scene-Provided Player — Activity-owned
Manager-Provisioned Player — single local Player
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

Não simular essas features com singleton, busca global, manager consumidor ou acesso direto a internals do package.

# 10. Critério de manutenção

```text
1. atualizar o README da própria demo;
2. atualizar a tabela global apenas quando o status mudar;
3. registrar jornadas e limitações sem transformar observação em contrato;
4. encaminhar problema técnico ao QAFramework;
5. encaminhar solução reutilizável ao package;
6. remover texto duplicado ou obsoleto no mesmo corte.
```

# 11. Próximo trabalho ativo

```text
DEMO03-MULTI-01 — Dynamic Capacity and Late Join
```
