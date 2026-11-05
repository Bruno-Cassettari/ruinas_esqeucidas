using UnityEngine;

// Lê o mapa ASCII de uma fase (Fases.cs) e cria os objetos na cena:
// blocos, plataformas, espinhos, saída da fase e a zona de queda.
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
                    case 'P':
                        resultado.inicioJogador = pos;
                        break;
                    case 'E':
                        CriarSaida(pos, objetos);
                        break;
                    case '.':
                        if (Celula(mapa, x, linha + 1) == '#') CriarDecoracao(pos, x, objetos);
                        break;
                }
            }
        }

        composite.GenerateGeometry();

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

    // Armaduras, velas e lápides sobre o chão (só visual, sem colisão).
    static void CriarDecoracao(Vector2 pos, int x, Transform pai)
    {
        if (!ArteJogo.Disponivel) return;
        int sorteio = (x * 37 + 11) % 17;
        Sprite sprite = sorteio switch
        {
            0 => ArteJogo.Recorte(0, 26, 1, 28),   // armadura
            5 => ArteJogo.Recorte(4, 19, 5, 20),   // velas
            11 => ArteJogo.Recorte(14, 10, 16, 13), // lápide
            _ => null
        };
        if (sprite == null) return;
        NovoSprite("Decoracao", pos + new Vector2(0f, -0.5f), sprite, new Color(0.8f, 0.8f, 0.9f), pai, -2);
    }

    static void CriarPlataformaAtravessavel(Vector2 pos, Transform pai)
    {
        var go = NovoSprite("Plataforma", pos + new Vector2(0f, 0.375f), GeradorSprites.Quadrado, Paleta.Plataforma, pai);
        go.transform.localScale = new Vector3(1f, 0.25f, 1f);
        var bc = go.AddComponent<BoxCollider2D>();
        bc.usedByEffector = true;
        var ef = go.AddComponent<PlatformEffector2D>();
        ef.useOneWay = true;
        ef.surfaceArc = 160f;
    }

    static void CriarEspinho(Vector2 pos, Transform pai)
    {
        var tileEspinho = ArteJogo.Disponivel ? ArteJogo.Tile(4, 13) : null;
        var go = tileEspinho != null
            ? NovoSprite("Espinho", pos, tileEspinho, Color.white, pai, 1)
            : NovoSprite("Espinho", pos, GeradorSprites.Triangulo, Paleta.Espinho, pai, 1);
        var bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(0.7f, 0.45f);   // hitbox menor que o desenho: mais justo para o jogador
        bc.offset = new Vector2(0f, -0.25f);
        go.AddComponent<ObstaculoDano>();
    }

    static void CriarSaida(Vector2 pos, Transform pai)
    {
        GameObject go;
        var arco = ArteJogo.Recorte(1, 31, 2, 34);
        if (arco != null)
        {
            // Arco de pedra aberto, com a luz âmbar acima: a passagem para a próxima região.
            go = NovoSprite("Saida", pos + new Vector2(0f, -0.5f), arco, Color.white, pai, -1);
            go.transform.localScale = new Vector3(1.4f, 1.1f, 1f);
            var luz = NovoSprite("Luz", pos + new Vector2(0f, 3.4f), ArteJogo.Tile(5, 24), Color.white, pai, 0);
            luz.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
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
        if (arco != null) { bc.size = new Vector2(0.8f, 2f); bc.offset = new Vector2(0f, 1f); }
        go.AddComponent<Objetivo>();
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
