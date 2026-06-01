# computer_graphic_game

Jogo de labirinto 3D desenvolvido em Unity 6. O objetivo é levar o personagem da entrada até a saída do labirinto gerado aleatoriamente, desviando de obstáculos fixos e móveis.

## Requisitos

- [Unity Hub](https://unity.com/download)
- Unity **6000.3.10f1** (instale via Unity Hub)

## Como rodar após clonar

### 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
```

### 2. Abrir o projeto no Unity Hub

1. Abra o **Unity Hub**
2. Clique em **Open** (ou **Add project from disk**)
3. Navegue até a pasta clonada e selecione-a
4. Aguarde o Unity importar os pacotes (pode demorar alguns minutos na primeira vez)

### 3. Abrir a cena

Na aba **Project**, navegue até:
```
Assets → Scenes → SampleScene
```
Dê duplo clique em **SampleScene** para abri-la.

### 4. Pressionar Play

Clique no botão **▶ Play** na parte superior do Editor. O labirinto é gerado automaticamente ao iniciar — nenhuma configuração adicional é necessária.

## Controles

| Tecla | Ação |
|---|---|
| `W` / `↑` | Mover para frente |
| `S` / `↓` | Mover para trás |
| `A` / `←` | Mover para a esquerda |
| `D` / `→` | Mover para a direita |

o zero é porque meu telcado ezta com a zetinha quebrada!

## Regras do jogo

- O personagem deve manter **pelo menos 1 metro de distância** de cada parede. Se ficar mais perto, volta ao início.
- O personagem deve manter **pelo menos 0,5 metros de distância** dos obstáculos. Se ficar mais perto, volta ao início.
- O objetivo é sair do labirinto chegando à **área verde** no canto oposto à entrada (área azul).

## Estrutura do labirinto

- Labirinto gerado aleatoriamente a cada partida (algoritmo DFS)
- Grade de 7×7 células → mínimo de 64 paredes
- 6 obstáculos fixos (vermelho) com NavMesh Obstacle
- 6 obstáculos móveis (amarelo) com NavMesh Obstacle que patrulham os corredores
- NavMesh bakeado em runtime sobre o piso

## Pacotes utilizados

| Pacote | Versão |
|---|---|
| Universal Render Pipeline (URP) | 17.3.0 |
| Input System | 1.19.0 |
| AI Navigation (NavMesh) | 2.0.12 |
| ProBuilder | 6.0.9 |


# Occlusion Culling

Para aplicar a oclusão os objetos da cena precisam ser estáticos, portanto, não pode ser gerados em tempo de execução porque não será possível fazer o bake, foi ajustar o o script MazeGenerator.cs oara dar uma nova opção ao ContextMenu, que gera os objetos da cena antes de rodar e com isso é possível fazer o bake.

Clique no objeto que está com o Script do MazeGenerator.cs, clique nos três pontinhos (⋮) do script do inspecionar, e clique na opção/método "Gerar Labirinto", com isso basta fazer o bake seguindo os passo: Window -> Rendering ->  Occlusion Culling -> Bake.