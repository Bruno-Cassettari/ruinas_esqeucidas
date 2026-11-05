# Ruínas Esquecidas — Etapa 4 (versão jogável parcial)

Escopo do cronograma até 06/11: movimentação (corrida e pulo), colisões, obstáculos com dano, sistema de vida, menu e HUD básicos e a Fase 1 jogável. Sprites provisórios são gerados em código, sem nenhum asset.

## Como abrir
1. Unity Hub → **Add → Add project from disk** → selecione esta pasta (`Documents/RuinasEsquecidas`).
2. Escolha o editor **Unity 6 LTS** quando o Hub pedir a versão.
3. Na primeira abertura, as cenas `Menu` e `Jogo` são criadas sozinhas em `Assets/Scenes`. Se não forem, use o menu **Ruínas Esquecidas → Montar cenas do jogo**.
4. Abra `Assets/Scenes/Menu.unity` e aperte **Play**.

Controles: **A/D ou setas** para andar, **Espaço / W / seta para cima** para pular (segurar pula mais alto).

## Scripts
| Script | Responsável | O que faz |
|---|---|---|
| `Jogador/PlayerController.cs` | Bruno | Corrida, pulo (coyote time + jump buffer), recuo ao levar dano |
| `Jogador/Entrada.cs` | Bruno | Teclado/controle (Input System novo ou antigo) |
| `Jogador/CameraSeguir.cs` | Bruno | Câmera suave limitada à fase |
| `Jogador/GameManager.cs` | Bruno | Monta a fase, reinicia ao morrer, tela de fase concluída |
| `Fases/ObstaculoDano.cs` | Bruno | Dano ao toque (espinhos) |
| `Fases/ZonaDeQueda.cs` | Bruno | Abismo |
| `Fases/Objetivo.cs` | Bruno | Saída da fase |
| `Interface/MenuPrincipal.cs`, `Interface/EstiloUI.cs` | Bruno | Menu inicial (Jogar/Sair) e estilos |
| `Fases/Fases.cs` | João Pedro | Mapa da Fase 1 |
| `Fases/ConstrutorDeFase.cs` | João Pedro | Monta a fase a partir do mapa |
| `Jogador/VidaJogador.cs` | João Pedro | 3 corações e invulnerabilidade |
| `Interface/HUD.cs` | João Pedro | Corações e "FASE 1/3" |
| `Sistemas/Paleta.cs`, `Sistemas/GeradorSprites.cs` | João Pedro | Cores e sprites provisórios |
| `Editor/MontarCenas.cs` | João Pedro | Cria as cenas e configura o build |

Legenda do mapa: `#` sólido · `-` plataforma atravessável · `^` espinho · `P` início · `E` saída · `.` vazio (buraco = abismo).

## Arte e créditos
- **Cenário:** "Dark Dungeon - Side-Scroller Tileset", de Raou (https://raou.itch.io/dark-dun). Pacote gratuito, com tiles de 16x16 px e uma tocha animada.
- **Personagem:** "Hooded Protagonist", de Penzilla Design (https://penzilla.itch.io/hooded-protagonist). A licença **exige o crédito "Graphics created by Penzilla Design"** na página do jogo e no relatório. Para uso comercial, o autor pede a compra pelo preço sugerido. Para um trabalho acadêmico, basta o crédito.
- Os arquivos ficam em `Assets/Resources/Arte` (`Cenario/` e `Heroi/`). O script `ConfigurarArte` importa as imagens como pixel art. Se a pasta for removida, o jogo volta a usar os sprites provisórios.

| Script | Responsável | O que faz |
|---|---|---|
| `Sistemas/ArteJogo.cs` | João Pedro | Recorta tiles, objetos, tocha e quadros do personagem |
| `Jogador/AnimacaoJogador.cs` | João Pedro | Animação parado/correndo/pulo/dano |
| `Fases/FundoParallax.cs` | João Pedro | Fundo em camadas (paredes com arcos, colunas e tochas) |
| `Fases/AnimacaoSimples.cs` | João Pedro | Animação das tochas |
| `Editor/ConfigurarArte.cs` | João Pedro | Importação das imagens como pixel art |

## Ainda não feito (próximas semanas)
- 09–13/11: Fases 2 e 3, telas de vitória e derrota
- 16–20/11: arte final, parallax, áudio
- 23–27/11: build WebGL e itch.io
