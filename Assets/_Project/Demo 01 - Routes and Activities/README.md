# 🗺️ FIRSTGAME — Lugar, Momento e Reação: como o Immersive Framework monta o contexto do jogo

> Guia para quem vai desenhar cenas com o framework — não é preciso saber C# para entender isso, só entender o jogo.

## A ideia em uma frase

O jogador nunca está só "numa cena". Ele está num **lugar** (Route), vivendo um **momento** (Activity) dentro dele, e o mundo ao redor **reage** a esse momento (Visibility Adapter).

```text
lugar        →  Floresta ou Campos
momento      →  Vacas ou Galinhas
reação       →  Abelhas ou Maçãs aparecem/somem
```

Quatro perguntas, quatro sistemas:

| Pergunta | Quem responde | Exemplo na demo |
|---|---|---|
| Onde o jogador está? | **Route** | Campos |
| O que está acontecendo aqui? | **Activity** | Vacas |
| O que o mundo faz por causa disso? | **Visibility Adapter** | Abelhas aparecem |
| Isso já aconteceu de verdade? | **Lifecycle Events** | `Activity Entered` disparado |

Guarde essa tabela — o resto do documento é só destrinchar cada linha dela.

---

## 1. Route: o lugar

Quando o jogador escolhe **Campos**, o framework não faz um `LoadScene` solto. Ele monta o destino inteiro:

```text
entra na Route Campos
   → resolve as cenas que pertencem a ela
   → carrega conteúdo compartilhado
   → liga a Activity inicial configurada
   → avisa todo mundo que a Route entrou
```

Pense na Route como uma **caixa de mudança completa**, não como uma cena isolada:

```text
Route Campos
├── cena principal (Sample_Environment)
├── cenas adicionais
├── conteúdo compartilhado
└── Activity inicial
```

É por isso que, como designer, você trata "Campos" como um destino de verdade do jogo — não uma sequência de scripts carregando cena por cena na mão.

---

## 2. Activity: o momento

Dentro de uma Route, o framework pode trocar de **Activity** sem sair do lugar. Na demo: **Vacas** e **Galinhas**.

A sacada boa aqui: **trocar de Activity não recarrega a Route.**

```text
Campos + Vacas   →  jogador muda o modo  →   Campos + Galinhas
        ↑                                            ↑
   Route continua carregada, só o "momento" mudou
```

Isso importa na prática porque evita o soluço de recarregar o mundo inteiro toda vez que o jogo muda de estado — só o que precisa trocar, troca.

---

## 3. Visibility Adapter: a reação do mundo

Abelhas e Maçãs **não são Activities** — são conteúdo local que só *reage* ao que já foi decidido. Elas não escolhem nada, só obedecem:

```text
Bee To Cows Activities      → ligado à Activity "Vacas"
Apple To Chikens Activities → ligado à Activity "Galinhas"
```

O `ActivityLocalVisibilityAdapter` é quem faz essa ponte: ele observa qual Activity está ativa e liga/desliga o objeto conforme o caso.

```text
Activity Vacas entra     →  Abelhas aparecem, Maçãs somem
Activity Galinhas entra  →  Maçãs aparecem, Abelhas somem
```

**Regra de ouro para montar uma cena:** cada "pacote" de conteúdo que deve entrar e sair junto vira um root, com um adapter nele — não um adapter por objetinho solto.

```text
Activity_Cows_Content
├── ActivityLocalVisibilityAdapter
├── ActivityContentLifecycleEvents
├── Bees
├── Effects
└── Interactions
```

---

## 4. Lifecycle Events: a prova de que aconteceu

Os eventos de lifecycle não são só logs decorativos — são o gancho para você conectar comportamento (animação, som, HUD) sem inventar um sistema paralelo.

| Nível | Entrada | Saída |
|---|---|---|
| Scene | `Available` | `Releasing` |
| Route | `Entered` | `Exited` |
| Activity | `Entered` | `Exited` |

Exemplo de uso real:

```text
Activity Vacas Entered  → toca animação das Abelhas, liga interação, atualiza HUD
Activity Vacas Exited   → para o comportamento, limpa estado, prepara a saída
```

E o **Canvas de diagnóstico** existe só para responder, ao vivo: *"os eventos que eu esperava realmente foram chamados?"* Ele não decide nada, só mostra evidência:

```text
ROUTE    | Sample Fields         | ENTERED: CALLED | EXITED: WAITING
ACTIVITY | Sample Activity Cows  | ENTERED: CALLED | EXITED: WAITING
```

Para detalhe pesado, o Console tem o canal `FIRSTGAME_LIFECYCLE`.

---

## 5. A demo do começo ao fim

Escolhendo **Campos → Vacas**:

```text
1. Route Campos é solicitada
2. Cenas da Route carregam
3. Route entra                      → ROUTE Entered
4. Activity inicial (Vacas) entra   → ACTIVITY Entered
5. Adapters atualizam objetos       → Abelhas ativas / Maçãs inativas
6. Canvas confirma os callbacks
```

Trocando para **Galinhas**, sem sair de Campos:

```text
Sample Activity Cows     → Exited
Sample Activity Chikens  → Entered
Bee To Cows Activities   → inativo
Apple To Chikens         → ativo
```

A Route nunca sai do ar — só o "momento" trocou. Essa é a parte mais importante para entender de verdade.

---

## 6. Levando o modelo para outro jogo

O padrão é sempre: **Route dá o lugar → Activity dá o modo → Adapter faz o mundo reagir.**

### 🏘️ RPG / mundo aberto

```text
Route: Vila
Activities: Exploração · Comércio · Diálogo

Comércio    → bancas abrem, UI de loja liga
Diálogo     → NPCs de conversa aparecem, câmera e UI de diálogo ligam
Exploração  → tudo isso desliga, NPCs andando ligam
```

### 🌲 Jogo de exploração

```text
Route: Floresta
Activities: Exploração · Combate · Coleta

Adapters: HUD de combate · inimigos locais · recursos coletáveis · UI de exploração
```

### 🏎️ Jogo de corrida

```text
Route: Circuito
Activities: Preparação · Corrida · Resultado

Adapters: grid de largada · cronômetro · placar final · elementos de pista
```

### 🏗️ Jogo de construção

```text
Route: Base Principal
Activities: Exploração · Construção · Gerenciamento

Adapters: ferramentas de construção · HUD de recursos · seleção de estruturas
```

---

## 7. Montando uma Route ou Activity nova — checklist rápido

**Nova Route**, pergunte-se:
- Que lugar o jogador está entrando?
- Quais cenas formam esse lugar?
- Existe uma Activity inicial?
- Que conteúdo deve durar a Route inteira?

**Nova Activity**, pergunte-se:
- Que modo está entrando?
- O que aparece? O que some?
- O que começa e o que termina?

Depois: crie o asset → associe à Route certa → organize os objetos em roots → adicione o `ActivityLocalVisibilityAdapter` em cada root → ligue `Entered`/`Exited` onde precisar de reação extra.

---

## 8. O que é do framework e o que é seu

| Framework faz | Você (jogo) faz |
|---|---|
| Resolve a Route | Cria os assets |
| Carrega a composição | Organiza os objetos em roots |
| Define a Activity ativa | Configura os bindings |
| Emite os lifecycle events | Liga os callbacks |
| Avisa os adapters | Decide o que aparece em cada momento |

---

## 9. Armadilhas comuns — não faça isso

- ❌ Ativar/desativar manualmente objetos que já têm adapter
- ❌ Trocar de Activity via `FindObjectOfType`
- ❌ Criar um singleton de navegação paralelo
- ❌ Duplicar callbacks com um event bus estático
- ❌ Recarregar a Route inteira só pra trocar de Activity
- ❌ Usar o nome do GameObject como identidade

Todos esses padrões tornam a montagem opaca — a composição precisa continuar legível no Inspector.

---

## 10. Como saber se está tudo certo

Rode a sequência completa e confira no Canvas/Console:

```text
Entrada da Route     → Scene Available → Route Entered → Startup Activity Entered
Troca de Activity    → Previous Activity Exited → Target Activity Entered
Saída da Route       → Activity Exited → Route Exited → Scene Releasing
Visibilidade         → conteúdo da Activity ativa visível / demais ocultos
```

Se você consegue responder "qual Route está ativa, qual Activity está ativa, o que devia aparecer e o Canvas confirmou o callback" — a montagem está correta.

---

## Resumo de uma linha

> **Route cria o lugar. Activity cria o momento. Visibility Adapter muda o mundo local. Lifecycle Events provam que tudo aconteceu de verdade.**
