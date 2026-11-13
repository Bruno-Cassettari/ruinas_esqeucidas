using System.Collections.Generic;
using UnityEngine;

// Lê o mapa ASCII de uma fase (Fases.cs) e cria os objetos na cena:
// blocos, plataformas, espinhos, sentinelas, inscrições, saída/artefato e a zona de queda.
// Cada caractere = 1 unidade do Unity.
public static class ConstrutorDeFase
{
    public struct Resultado
    {
        public Vector2 inicioJogador;
        public Rect limites;
    }

    public static Resultado Construir(DadosFase dados, Transform raiz)
    {
        var resultado = new Resultado();
        string[] mapa = dados.mapa;
        int altura = mapa.Length;
        int largura = 0;
        foreach (var linha in mapa) largura = Mathf.Max(largura, linha.Length);

        // Blocos sólidos ficam sob um CompositeCollider2D: vira uma única forma,
        // sem "emendas" entre blocos onde o personagem poderia enroscar.
        var solidos = new GameObject("Solidos");
        solidos.transform.SetParent(raiz, false);
        var rbSolidos = solidos.AddComponent<Rigidbody2D>();
        rbSolidos.bodyType = RigidbodyType2D.Static;
        var composite = solidos.AddComponent<CompositeCollider2D>();
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
        composite.generationType = CompositeCollider2D.GenerationType.Manual; // gera uma vez só, no fim

        var obstaculos = NovoGrupo("Obstaculos", raiz);
        var objetos = NovoGrupo("Objetos", raiz);
        var inscricoes = new List<Vector2>();

        for (int linha = 0; linha < altura; linha++)
        {
            for (int x = 0; x < mapa[linha].Length; x++)
            {
                char c = mapa[linha][x];
                int y = altura - 1 - linha;
                var pos = new Vector2(x, y);

                switch (c)
                {
                    case '#':
                        CriarBloco(pos, mapa, x, linha, solidos.transform);
                        break;
                    case '-':
                        CriarPlataformaAtravessavel(pos, objetos);
                        break;
                    case '^':
                        CriarEspinho(pos, obstaculos);
                        break;
                    case 'M':
                        CriarSentinela(pos, Vector2.right, 2.5f, obstaculos, x);
                        break;
                    case 'V':
                        CriarSentinela(pos + Vector2.up * 2f, Vector2.up, 2f, obstaculos, x); // sobe e desce a partir do chão
                        break;
                    case 'P':
                        resultado.inicioJogador = pos;
                        break;
                    case 'I':
                        inscricoes.Add(pos);
                        break;
                    case 'E':
                        CriarSaida(pos, objetos);
                        break;
                    case 'A':
                        CriarArtefato(pos, objetos);
                        break;
                    case '.':
                        if (Celula(mapa, x, linha + 1) == '#') CriarDecoracao(pos, x, objetos);
                        break;
                }
            }
        }

        composite.GenerateGeometry();

        // Inscrições recebem os textos na ordem em que aparecem (da esquerda para a direita).
        inscricoes.Sort((a, b) => a.x.CompareTo(b.x));
        for (int i = 0; i < inscricoes.Count; i++)
        {
            string texto = dados.inscricoes != null && i < dados.inscricoes.Length ? dados.inscricoes[i] : "...";
            CriarInscricao(inscricoes[i], texto, objetos);
        }

        resultado.limites = new Rect(-0.5f, -0.5f, largura, altura + 4f);
        CriarZonaDeQueda(largura, raiz);
        return resultado;
    }

    static char Celula(string[] mapa, int x, int linha)
    {
        if (linha < 0 || linha >= mapa.Length || x < 0 || x >= mapa[linha].Length) return '.';
        return mapa[linha][x];
    }

    static Transform NovoGrupo(string nome, Transform raiz)
    {
        var go = new GameObject(nome);
        go.transform.SetParent(raiz, false);
        return go.transform;
    }

    static GameObject NovoSprite(string nome, Vector2 pos, Sprite sprite, Color cor, Transform pai, int ordem = 0)
    {
        var go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = cor;
        sr.sortingOrder = ordem;
        return go;
    }

    static void CriarBloco(Vector2 pos, string[] mapa, int x, int linha, Transform pai)
    {
        GameObject go;
        var tile = EscolherTile(mapa, x, linha);
        if (tile != null)
        {
            go = NovoSprite("Bloco", pos, tile, Color.white, pai);
        }
        else
        {
            bool topo = Celula(mapa, x, linha - 1) != '#';
            go = NovoSprite("Bloco", pos, GeradorSprites.Quadrado, topo ? Paleta.PedraTopo : Paleta.Pedra, pai);
        }
        var bc = go.AddComponent<BoxCollider2D>();
        bc.size = Vector2.one;
        bc.compositeOperation = Collider2D.CompositeOperation.Merge;
    }

    // Escolhe o tile do Dark Dungeon pela posição do bloco: laje de pedra no topo,
    // tijolos logo abaixo e pedras escuras no fundo.
    static Sprite EscolherTile(string[] mapa, int x, int linha)
    {
        if (!ArteJogo.Disponivel) return null;
        bool esquerda = Celula(mapa, x - 1, linha) == '#';
        bool direita = Celula(mapa, x + 1, linha) == '#';
        int profundidade = 0;
        while (Celula(mapa, x, linha - 1 - profundidade) == '#') profundidade++;

        if (profundidade == 0)
            return !esquerda ? ArteJogo.Tile(9, 9) : !direita ? ArteJogo.Tile(11, 9) : ArteJogo.Tile(10, 9);
        if (profundidade <= 2)
            return ArteJogo.Tile(6 + (x + linha) % 2, 15);
        return ArteJogo.Tile(9 + x % 2, 12);
    }

    // Só lápides como decoração de chão: armaduras paradas e velas foram retiradas porque
    // se confundiam com as sentinelas e os espinhos (que dão dano).
    static void CriarDecoracao(Vector2 pos, int x, Transform pai)
    {
        if (!ArteJogo.Disponivel) return;
        if ((x * 37 + 11) % 17 != 11) return;
        var sprite = ArteJogo.Lapide;
        if (sprite == null) return;
        NovoSprite("Decoracao", pos + new Vector2(0f, -0.5f), sprite, new Color(0.75f, 0.75f, 0.85f), pai, -2);
    }

    static void CriarPlataformaAtravessavel(Vector2 pos, Transform pai)
    {
        GameObject go;
        if (ArteJogo.Viga != null)
        {
            go = NovoSprite("Plataforma", pos, ArteJogo.Viga, Color.white, pai);
        }
        else
        {
            go = NovoSprite("Plataforma", pos + new Vector2(0f, 0.375f), GeradorSprites.Quadrado, Paleta.Plataforma, pai);
            go.transform.localScale = new Vector3(1f, 0.25f, 1f);
        }
        var bc = go.AddComponent<BoxCollider2D>();
        bc.usedByEffector = true;
        if (ArteJogo.Viga != null) { bc.size = new Vector2(1f, 0.3f); bc.offset = new Vector2(0f, 0.35f); }
        var ef = go.AddComponent<PlatformEffector2D>();
        ef.useOneWay = true;
        ef.surfaceArc = 160f;
    }

    // Espinhos claros com ponta vermelha e um brilho avermelhado atrás, para não se confundirem
    // com o cenário (problema apontado no teste da Etapa 4).
    static void CriarEspinho(Vector2 pos, Transform pai)
    {
        var go = NovoSprite("Espinho", pos, GeradorSprites.Espinhos, Color.white, pai, 2);
        var brilho = NovoSprite("Brilho", pos + new Vector2(0f, -0.15f), GeradorSprites.Losango, new Color(1f, 0.2f, 0.15f, 0.22f), pai, 1);
        brilho.transform.localScale = new Vector3(1.6f, 0.9f, 1f);
        var bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(0.75f, 0.45f);  // hitbox menor que o desenho: mais justo para o jogador
        bc.offset = new Vector2(0f, -0.25f);
        go.AddComponent<ObstaculoDano>();
    }

    // Sentinelas: armadura que patrulha (M) ou relicário com caveira que sobe e desce (V).
    static void CriarSentinela(Vector2 pos, Vector2 direcao, float amplitude, Transform pai, int semente)
    {
        bool horizontal = direcao.x != 0f;
        Sprite sprite = horizontal ? ArteJogo.Armadura : ArteJogo.Relicario;
        GameObject go;
        SpriteRenderer visual = null;
        if (sprite != null)
        {
            go = new GameObject(horizontal ? "Armadura" : "Relicario");
            go.transform.SetParent(pai, false);
            go.transform.position = pos;
            var v = NovoSprite("Visual", pos + (horizontal ? new Vector2(0f, -0.5f) : new Vector2(0f, -0.5f)), sprite, new Color(1f, 0.82f, 0.82f), go.transform, 3);
            visual = v.GetComponent<SpriteRenderer>();
            var olho = NovoSprite("Olho", pos + (horizontal ? new Vector2(0f, 1.15f) : new Vector2(0f, 0f)), GeradorSprites.Losango, new Color(1f, 0.25f, 0.15f, 0.35f), go.transform, 2);
            olho.transform.localScale = horizontal ? new Vector3(0.9f, 0.9f, 1f) : new Vector3(1.6f, 1.6f, 1f);
        }
        else
        {
            go = NovoSprite("Sentinela", pos, GeradorSprites.Quadrado, Paleta.Sentinela, pai, 3);
            go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        }

        go.AddComponent<Rigidbody2D>();
        var bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        if (sprite != null && horizontal) { bc.size = new Vector2(0.8f, 1.7f); bc.offset = new Vector2(0f, 0.35f); }
        else bc.size = new Vector2(0.8f, 0.8f);
        go.AddComponent<ObstaculoDano>();
        var s = go.AddComponent<Sentinela>();
        s.direcao = direcao;
        s.amplitude = amplitude;
        s.fase = semente * 0.7f;
        s.velocidade = horizontal ? 1.2f : 1.6f;
        s.visual = horizontal ? visual : null;
    }

    // Obelisco com uma marca âmbar: ao se aproximar, o texto da inscrição aparece no HUD.
    static void CriarInscricao(Vector2 pos, string texto, Transform pai)
    {
        if (ArteJogo.Obelisco != null)
        {
            NovoSprite("Obelisco", pos + new Vector2(0f, -0.5f), ArteJogo.Obelisco, new Color(0.85f, 0.82f, 0.95f), pai, -1);
            var marca = NovoSprite("Marca", pos + new Vector2(0f, 1.1f), GeradorSprites.Losango, Paleta.Ambar, pai, 0);
            marca.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
            var brilho = NovoSprite("Brilho", pos + new Vector2(0f, 1.1f), GeradorSprites.Losango, Paleta.AmbarFraco, pai, -1);
            brilho.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        else
        {
            var go = NovoSprite("Inscricao", pos + new Vector2(0f, 0.2f), GeradorSprites.Quadrado, Paleta.AmbarFraco, pai, -1);
            go.transform.localScale = new Vector3(0.6f, 1.4f, 1f);
        }

        var gatilho = new GameObject("AreaLeitura");
        gatilho.transform.SetParent(pai, false);
        gatilho.transform.position = pos + new Vector2(0f, 0.5f);
        var bc = gatilho.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(3f, 2.5f);
        gatilho.AddComponent<Inscricao>().texto = texto;
    }

    static void CriarSaida(Vector2 pos, Transform pai)
    {
        GameObject go;
        var arco = ArteJogo.ArcoDePedra;
        if (arco != null)
        {
            // Arco de pedra aberto, com a luz âmbar acima: a passagem para a próxima região.
            go = NovoSprite("Saida", pos + new Vector2(0f, -0.5f), arco, Color.white, pai, -1);
            NovoSprite("Luz", pos + new Vector2(0f, 3.1f), ArteJogo.Luz, Color.white, pai, 0);
        }
        else
        {
            go = NovoSprite("Saida", pos + new Vector2(0f, 0.5f), GeradorSprites.Quadrado, new Color(0.02f, 0.02f, 0.03f), pai, -1);
            go.transform.localScale = new Vector3(1.4f, 2f, 1f);
            var moldura = NovoSprite("Moldura", go.transform.position + new Vector3(0f, 1.05f, 0f), GeradorSprites.Quadrado, Paleta.Ambar, pai, 0);
            moldura.transform.localScale = new Vector3(1.7f, 0.12f, 1f);
        }

        var bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        if (arco != null) { bc.size = new Vector2(1f, 2f); bc.offset = new Vector2(0f, 1f); }
        go.AddComponent<Objetivo>();
    }

    // Coração de Aster: cristal âmbar flutuando sobre um pedestal. Tocar nele = vitória.
    static void CriarArtefato(Vector2 pos, Transform pai)
    {
        if (ArteJogo.Disponivel)
            NovoSprite("Pedestal", pos + new Vector2(0f, -0.1f), ArteJogo.Tile(10, 9), Color.white, pai, 0);
        else
            NovoSprite("Pedestal", pos + new Vector2(0f, -0.25f), GeradorSprites.Quadrado, Paleta.PedraTopo, pai, 0)
                .transform.localScale = new Vector3(1.2f, 0.5f, 1f);

        var go = NovoSprite("CoracaoDeAster", pos + new Vector2(0f, 1f), GeradorSprites.Losango, Paleta.Ambar, pai, 4);
        go.transform.localScale = new Vector3(0.7f, 0.9f, 1f);
        var brilho = NovoSprite("Brilho", go.transform.position, GeradorSprites.Losango, Paleta.AmbarFraco, go.transform, 3);
        brilho.transform.localScale = new Vector3(2.4f, 2.4f, 1f);

        var bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(1.4f, 1.4f);
        go.AddComponent<Objetivo>().ehArtefato = true;
    }

    static void CriarZonaDeQueda(int largura, Transform raiz)
    {
        var go = new GameObject("ZonaDeQueda");
        go.transform.SetParent(raiz, false);
        go.transform.position = new Vector2(largura / 2f, -4f);
        var bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(largura + 40f, 2f);
        go.AddComponent<ZonaDeQueda>();
    }
}
