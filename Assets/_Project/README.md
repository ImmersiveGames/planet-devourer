# FIRSTGAME consumer baseline

O FIRSTGAME é o consumidor real do `com.immersive.framework`.

O workspace está na baseline de higiene H6A: o package permanece instalado e o conteúdo específico do jogo foi preservado, mas a integração canônica P3 de Player, Actor e Camera ainda não foi montada.

## Estado atual

- rotas, activities, loading, pause, reset, áudio, UI e conteúdo de gameplay permanecem no projeto;
- `InputSystem_Actions.inputactions`, movimento e targets neutros permanecem reutilizáveis;
- integrações antigas de Player/Slot/Binding/Camera foram removidas;
- tooling automático de setup, repair, migration e provas locais foi removido;
- nenhuma configuração P3 deve ser considerada pronta.

`FG_GameplayBkp` foi preservada somente porque contém gameplay exclusivo de loop, reset, runtime spawning e áudio. Ela não representa uma arquitetura de integração válida.

## Próximo corte

A montagem mínima P3 será feita manualmente, usando apenas as superfícies públicas do package. Não copie fixtures ou assets serializados do QA e não recrie facades locais.

Consulte [FIRSTGAME-P3-MANUAL-INTEGRATION-BASELINE.md](Documentation/FIRSTGAME-P3-MANUAL-INTEGRATION-BASELINE.md) para o status formal desta baseline.
