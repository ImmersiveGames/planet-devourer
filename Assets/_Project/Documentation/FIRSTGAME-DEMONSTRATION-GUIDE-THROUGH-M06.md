# FIRSTGAME — Guia Consolidado dos Modelos Implementados até M06

Status: documentação do consumidor real  
Projeto: `ImmersiveGames/planet-devourer`  
Framework: `ImmersiveGames/com.immersive.framework`  
Escopo: M01, M02, M03 e M06

# 1. Objetivo

O FIRSTGAME demonstra como um usuário real encontra, configura e conecta as superfícies do Immersive Framework em um projeto Unity 6.5.

A prova de produto responde:

```text
como a feature é criada;
onde a intenção é editada;
quais componentes aparecem na hierarquia;
quem possui cada cena ou objeto;
qual é a autoridade runtime;
como o resultado é diagnosticado;
como ocorre cleanup e reentrada.
```

Casos negativos e regressões pertencem ao `QAFramework`.

# 2. Estado dos modelos

```text
M01 Route and Activity       concluído na Demo 01
M02 Lifecycle Events         concluído na Demo 01
M03 Activity Readiness       concluído na Demo 01
M04 Content Anchors          revogado/removido
M05 Anchor Materialization   revogado/removido
M06 Scene-Provided Player    concluído na Demo 02
```

# 3. Arquitetura comprovada

## Application e lifecycle

```text
GameApplication
→ Startup Route
→ Primary Scene + Route Content
→ Startup Activity
→ Activity Content
→ lifecycle callbacks
→ release determinístico
```

## Readiness

```text
Activity occurrence
→ participants registrados
→ Required agrega readiness bloqueante
→ Optional agrega diagnóstico não bloqueante
→ estado Ready/NotReady exposto
```

O corte atual não usa readiness como reveal gate.

## Scene-Provided Player

```text
Session Slot
→ Scene-Provided Player Composer
→ Local Player Host
→ Actor Mount
→ scene-authored Logical Actor
→ admission durante Activity Enter
→ Logical Actors Prepared
```

# 4. Camadas do produto

| Camada | Evidência no FIRSTGAME |
|---|---|
| Intenção | GameApplication, Route, Activity, profiles, Slot e ActorProfile |
| Composer/Authoring | SceneLocalPlayerAdmissionAuthoring e superfícies de lifecycle/readiness |
| Materialização técnica | cenas carregadas, handles, Actor evidence e bindings |
| Runtime escopado | Route/Activity contexts, participation Session e occurrence readiness |
| Diagnostics | Inspector Advanced/Debug, logs e presenters passivos |

# 5. Fluxos de validação

## Demo 01

```text
Menu
→ Route Fields/Forest
→ startup Activity
→ troca de Activity
→ lifecycle visível
→ saída e reentrada
```

```text
Menu
→ Route Activity Readiness
→ Preparation Pending
→ Required Complete
→ Ready
→ Intermission
→ voltar e reentrar sem estado stale
```

## Demo 02 / M06

```text
Menu
→ RouteLocalPlayer
→ ActivityLocalPlayer
→ Scene-Provided Player admitted
→ Actor recebe PlayerInput
→ Activity Ready
→ movimento
→ Back To Menu
→ release
→ reentrada limpa
```

# 6. Achados de produto

## Readiness não controla revelação

A implementação atual permite que a transição termine e a Activity fique visível enquanto readiness ainda está pendente. Isso é coerente com o contrato operacional atual, mas não cobre políticas como “manter fade até Ready” ou “mostrar o mundo, mas bloquear gameplay até Ready”.

Essa diferença deve permanecer explícita na documentação e no futuro produto de loading/reveal.

## Evidence de prefab precisa usar identidade Unity correta

Durante a montagem do M06, `Apply / Rebuild` revelou um falso mismatch ao comparar `UnityEngine.Object` por `ReferenceEquals`. A correção adequada preserva:

```text
Editor
  igualdade Unity e identidade persistente de asset;

Runtime
  igualdade Unity sem dependência de AssetDatabase.
```

O caso reforça que o FIRSTGAME é uma prova de UX real: a composição estava correta, mas a superfície oficial rejeitava o usuário.

## Navegação deve possuir owner claro

No M03 e M06, botões de navegação que precisam permanecer durante várias Activities pertencem à Route, não a uma Activity específica.

```text
Route-owned navigation
→ permanece durante troca de Activity
→ é liberada na saída da Route
```

# 7. Regras para novos modelos

Cada novo modelo deve documentar:

```text
Purpose
Scope
Out of Scope
Assets
Scenes
Hierarchy
Inspector Configuration
Ownership
Play Mode Flow
Diagnostics
Cleanup and Reentry
Reusable Pieces
UX Findings
QA Follow-ups
```

A criação básica de arquivos pode ser resumida por nome, local e tipo. Hierarquia, referências, enums, ownership e lifecycle devem ser detalhados.

# 8. Não usar como base

```text
GameJam2025
PEGA
_ImmersiveGames.NewScripts
ProjectSettings do projeto antigo
IDs/GUIDs copiados de outra aplicação
```

O projeto atual é novo e deve usar namespaces e superfícies oficiais do package.

# 9. Critério de promoção

Uma feature demonstrada no FIRSTGAME não está automaticamente concluída no produto. Promoção requer:

```text
contrato oficial no package;
authoring compreensível;
runtime real;
diagnóstico;
prova integrada;
QA técnico apropriado;
documentação curta;
cleanup e reentrada.
```

# 10. Próxima fronteira

O próximo modelo após este escopo é M07 Manager-Provisioned Player. Ele deve provar uma composição diferente do M06:

```text
M06
  a cena já fornece o Player e o framework admite a instância.

M07
  um manager oficial provisiona o Host a partir de prefab autorizado.
```

M07 não deve reutilizar um Player pré-colocado na cena nem transformar o M06 em um fluxo híbrido.
