# Demo 01 — Routes, Activities, Lifecycle Events and Readiness

**Status:** M01, M02 e M03 demonstrados como prova de consumidor  
**Última validação principal:** 2026-08-04  
**Unity:** 6.5

A Demo 01 reúne:

```text
M01  Route and Activity
M02  Lifecycle Events
M03  Activity Readiness
```

O objetivo é mostrar como um projeto consumidor declara lugares, momentos, conteúdo por escopo, callbacks de lifecycle e readiness operacional sem criar uma autoridade paralela ao framework.

# 1. Como executar

Ative:

```text
Assets/_Project/Demo01/Demo01-GameApplication.asset
```

em:

```text
Assets/_Project/Settings/ImmersiveFramework/Resources/
└── ImmersiveFrameworkSettings.asset
```

Abra a cena inicial da Demo 01 e entre em Play Mode com o Console limpo. A aplicação inicia pelo menu da própria demonstração e oferece entradas separadas para Routes/Activities e Activity Readiness.

# 2. Estrutura principal

```text
Assets/_Project/Demo01/
├── Demo01-GameApplication.asset
├── Data/
│   ├── Activity Readiness/
│   └── Routes and Activities/
├── Prefabs/
│   ├── Activity Readiness/
│   └── Routes and Activities/
├── Scenes/
│   ├── Activity Readiness/
│   └── Routes and Activities/
└── Scripts/
    ├── Activity Readiness/
    └── Routes and Activities/
```

A identidade funcional vem dos assets e IDs. Nomes de GameObjects e arquivos servem para apresentação e diagnóstico, não como autoridade runtime.

# 3. M01 — Route and Activity

## 3.1 Intenção de produto

```text
Route
  representa um destino/lugar;
  possui Primary Scene;
  pode possuir conteúdo Route-owned;
  pode iniciar uma Activity.

Activity
  representa um momento/modo dentro da Route;
  pode trocar sem recarregar a Route inteira;
  pode possuir conteúdo Activity-owned;
  possui lifecycle próprio.
```

A demo utiliza destinos visuais baseados em uma composição compartilhada e diferencia cada ocorrência por conteúdo adicional da Route e da Activity.

## 3.2 Ownership esperado

```text
Route
├── Primary Scene
│   └── ambiente base
├── Route Content Profile
│   ├── navegação compartilhada
│   └── conteúdo específico do destino
└── Startup Activity
    └── Activity Content Profile
        └── conteúdo específico do momento
```

Regras:

```text
conteúdo que sobrevive à troca de Activity não fica em cena Activity-owned;
navegação compartilhada pertence à Route;
uma mesma cena não deve ser declarada simultaneamente por owners concorrentes;
cleanup deve acompanhar o owner que carregou o conteúdo.
```

## 3.3 Fluxo runtime

```text
menu solicita Route
→ framework libera a Route anterior
→ carrega Primary Scene
→ carrega Route Content
→ cria a ocorrência da Route
→ entra na Startup Activity
→ carrega Activity Content
→ publica lifecycle Entered
```

Troca somente de Activity:

```text
Activity anterior sai
→ conteúdo Activity-owned é liberado
→ nova Activity entra
→ novo conteúdo Activity-owned é carregado
→ Route e Route Content permanecem ativos
```

## 3.4 Aceite do M01

```text
[ ] entrada na Route correta
[ ] Primary Scene carregada uma vez
[ ] conteúdo Route-owned presente
[ ] Startup Activity iniciada
[ ] troca de Activity sem recarregar a Route inteira
[ ] saída libera o owner correto
[ ] reentrada não duplica conteúdo
```

# 4. M02 — Lifecycle Events

## 4.1 Eventos demonstrados

```text
Scene
  Available
  Releasing

Route
  Entered
  Exited

Activity
  Entered
  Exited
```

Os callbacks podem atualizar UI, iniciar animação, habilitar interação ou registrar evidência. Eles não escolhem Route/Activity por conta própria e não substituem a autoridade do framework.

## 4.2 Papel dos scripts consumidores

Os presenters e reporters da Demo 01 são superfícies de apresentação e diagnóstico do FIRSTGAME.

Eles devem:

```text
receber referências explícitas;
mostrar o último evento e o escopo associado;
manter callbacks pequenos;
limpar estado local no release/exit.
```

Eles não devem:

```text
resolver objetos por nome global;
criar uma segunda máquina de estados;
requisitar outra Route/Activity como efeito oculto do callback;
interpretar UI como fonte de verdade.
```

## 4.3 Aceite do M02

```text
[ ] Scene Available ocorre antes do uso do conteúdo
[ ] Scene Releasing acompanha o release correto
[ ] Route Entered/Exited acompanha a ocorrência correta
[ ] Activity Entered/Exited acompanha cada troca
[ ] callbacks não duplicam após reentrada
[ ] UI permanece passiva
```

# 5. M03 — Activity Readiness

## 5.1 Intenção de produto

Activity Readiness agrega participantes registrados na ocorrência atual da Activity.

```text
Required
  bloqueia o estado Ready enquanto não concluir.

Optional
  aparece no diagnóstico, mas não bloqueia Ready.
```

A demonstração compara três políticas de entrada:

| Policy | Apresentação | Comportamento esperado |
|---|---|---|
| `ObserveOnly` | apresentação existente | readiness é observada sem reter cover ou gates |
| `WaitVisible` | conteúdo visível | preparação fica visível enquanto capabilities configuradas permanecem gated |
| `WaitCovered` | `FadeWithLoading` | a Activity permanece coberta até todos os Required concluírem |

`WaitCovered` é a prova semelhante a produção: o Loading determinístico é dirigido pelos participantes do framework, não por scripts de gameplay.

## 5.2 Fluxo da demonstração

```text
menu da Demo 01
→ Activity Readiness Route
→ Observe Only
→ Intermission
→ Wait Visible ou Wait Covered
→ Intermission
→ nova entrada ou repetição de Wait Covered
```

Para `WaitCovered`:

```text
Activity request
→ FadeWithLoading inicia
→ faixa técnica de Loading avança
→ quatro Required participants concluem de forma independente
→ Optional permanece pendente
→ aggregate readiness fica Ready
→ Loading chega a 100%
→ Loading fecha
→ cover revela a Activity concluída
```

## 5.3 Configuração canônica de Wait Covered

```text
Activity Entry Readiness Policy = WaitCovered
Visual Transition Mode          = FadeWithLoading
Transition Gate Mode            = InputInteractionAndGameplay
Content Profile                 = ActivityContent_ReadinessWaitCovered
```

A cena Activity-owned contém uma instância do cenário Wait Covered. Ela não deve conter:

```text
segundo Loading Canvas;
FrameworkRuntimeHost local;
bootstrap paralelo;
autoridade de Loading do consumidor.
```

## 5.4 Participantes

A composição validada usa:

| Ordem | Participant Id | Requiredness | Fonte de conclusão |
|---:|---|---|---|
| 10 | `m03.wait-covered.chicken-01` | Required | Chicken 01 alcança o target |
| 20 | `m03.wait-covered.chicken-02` | Required | Chicken 02 alcança o target |
| 30 | `m03.wait-covered.chicken-03` | Required | Chicken 03 alcança o target |
| 40 | `m03.wait-covered.chicken-04` | Required | Chicken 04 alcança o target |
| 50 | `m03.wait-covered.optional` | Optional | intencionalmente pendente |

O framework conta participantes, não Chickens. Movimento e target são apenas apresentação de consumidor e evidência de conclusão.

Cada Required usa uma ponte local explícita entre o `ActivityReadinessParticipant` e sua área de preparação. O Optional não possui callback de conclusão.

## 5.5 Loading persistente

A apresentação persistente usa uma única superfície oficial com suporte a progresso determinístico.

Esperado:

```text
progress-capable surface;
determinate progress habilitado;
progresso visível durante Loading;
100% somente quando o aggregate readiness fica Ready;
progresso oculto e resetado quando Loading fecha;
hidden state aplicado no Awake.
```

FIRSTGAME não resolve nem atualiza diretamente essa superfície.

## 5.6 Evidência esperada

Uma ocorrência limpa de `WaitCovered` produz:

```text
4 Required capturados
4 Required concluídos
0 Required pendentes
1 Optional capturado
1 Optional pendente
Loading progress = 100%
Activity Request = Succeeded
blockingIssues = 0
```

A sequência visual esperada é monotônica: cada Required concluído produz um avanço independente após a faixa técnica.

## 5.7 Saída e reentrada

```text
primeira ocorrência WaitCovered conclui
→ Intermission entra
→ participantes da ocorrência são liberados
→ cena Activity-owned descarrega
→ segunda requisição cria outra ocorrência
→ quatro Required concluem novamente
→ Loading chega a 100%
→ nenhum estado stale permanece
```

A evidência manual registrada mostrou ocorrências bem-sucedidas distintas, com cleanup e novo denominator.

# 6. Validação consolidada

## 6.1 Aceite técnico do M03

```text
PASS — WaitCovered direto
PASS — 4 Required capturados e concluídos
PASS — 1 Optional capturado e pendente
PASS — Loading determinístico
PASS — 100% no aggregate Ready
PASS — Loading release e reveal
PASS — saída para Intermission
PASS — segunda ocorrência limpa
PASS — nenhum blocking issue na evidência fornecida
```

## 6.2 Aceite de produto do M03

```text
PASS — usuário consegue criar e configurar manualmente
PASS — três policies podem ser comparadas na mesma Route
PASS — Required completion é visível no Loading
PASS — Optional não bloqueia e é demonstrável
PASS — cenário é inspecionável e reutilizável
PARTIAL — wiring manual repetido ainda é suscetível a erro
PARTIAL — tooling oficial de authoring ainda não está completo
```

# 7. Findings de UX e produto

| ID | Finding | Severidade | Destino |
|---|---|---:|---|
| M03-UX-01 | Participante inativo ainda pode ser descoberto porque o escopo inclui children inativos | Alta | package docs/authoring/diagnostics |
| M03-UX-02 | UnityEvents repetidos facilitaram apontar para a ponte errada | Alta | package template/validator |
| M03-UX-03 | Participant Id duplicado sobreviveu ao authoring manual | Alta | package validation |
| M03-UX-04 | A composição foi inicialmente gravada como scene overrides em vez do Prefab Variant | Média | FIRSTGAME workflow/template |
| M03-UX-05 | Nova Activity exigiu atualização manual da visibilidade de navegação | Alta | package authoring/navigation |
| M03-UX-06 | Loading determinístico exige wiring manual e inspeção cuidadosa | Média | package sample/template/docs |
| M03-UX-07 | Apply de overrides pode carregar alterações não relacionadas para prefabs compartilhados | Média | higiene de mudanças FIRSTGAME |
| M03-UX-08 | Diagnóstico completo é denso para o usuário comum | Média | package Advanced/Debug summary |
| M03-UX-09 | Presenter local e Loading persistente podem parecer duas autoridades | Baixa | apresentação/docs FIRSTGAME |
| M03-UX-10 | Aggregate participant e participantes independentes têm usos diferentes pouco óbvios | Média | package guide/template |

Disposição:

```text
FIRSTGAME mantém apresentação, cenário e menu;
package deve amadurecer authoring, validation e Advanced/Debug;
QAFramework mantém failure, cancellation, invalidation e late completion.
```

# 8. Limites de autoridade

FIRSTGAME possui:

```text
assets de Route e Activity;
cenários visuais;
movimento dos Chickens;
pontes finas de conclusão;
navegação;
configuração da apresentação persistente;
evidência e findings de consumidor.
```

O framework possui:

```text
discovery e captura por ocorrência;
agregação Required/Optional;
denominator autoritativo;
envelope técnico/readiness de Loading;
publicação de 100%;
ordem de hide/reveal;
retenção e release de gates;
diagnóstico terminal.
```

FIRSTGAME não deve calcular o progresso autoritativo, escrever no Loading, resolver runtime host por busca global ou tratar Optional como parte do denominator de sucesso.

# 9. Troubleshooting

## Route entra, Activity não

Verifique:

```text
Startup Activity;
Activity ID;
Content Profile;
cenas no Build Settings;
falha bloqueante no diagnóstico da Route.
```

## Cena aditiva duplicada

Confirme que ela não está declarada ao mesmo tempo como:

```text
Primary Scene;
Route Content;
Activity Content.
```

## Readiness nunca fica Ready

Verifique:

```text
Required pendente;
Participant Id duplicado;
callback apontando para a ponte errada;
participante inativo ainda capturado;
conclusão enviada para ocorrência antiga;
falha obrigatória explícita.
```

Não transformar falha obrigatória em sucesso por timeout ou fallback.

# 10. Checklist de reutilização

```text
[ ] definir GameApplication e Startup Route
[ ] separar Primary Scene, Route Content e Activity Content
[ ] conectar lifecycle por referências explícitas
[ ] escolher ObserveOnly, WaitVisible ou WaitCovered
[ ] marcar Required somente quando realmente bloqueia Ready
[ ] manter Participant Id único
[ ] usar participantes independentes para increments independentes
[ ] configurar uma única superfície persistente de Loading
[ ] testar entrada, troca, saída e reentrada
[ ] confirmar cleanup por ocorrência
[ ] manter casos negativos no QAFramework
```
