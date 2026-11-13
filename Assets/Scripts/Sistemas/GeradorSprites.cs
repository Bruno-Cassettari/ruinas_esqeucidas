using UnityEngine;

// Cria sprites simples em código, para o jogo rodar sem nenhum asset de arte.
// Na etapa de arte final, basta trocar o sprite dos SpriteRenderers pelos assets definitivos.
public static class GeradorSprites
{
    static Sprite quadrado, triangulo, losango;
    static Texture2D coracao, coracaoVazio, pixel;

    public static Sprite Quadrado
    {
        get
        {
            if (quadrado == null)
            {
                var tex = NovaTextura(8, 8);
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                        tex.SetPixel(x, y, Color.white);
                tex.Apply();
                quadrado = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
            }
            return quadrado;
        }
    }

    // Triângulo apontando para cima (espinho). Ocupa 1 unidade de largura.
    public static Sprite Triangulo
    {
        get
        {
            if (triangulo == null)
            {
                const int n = 16;
                var tex = NovaTextura(n, n);
                for (int y = 0; y < n; y++)
                {
                    float meia = (n - y) / 2f;
                    for (int x = 0; x < n; x++)
                    {
                        bool dentro = Mathf.Abs(x + 0.5f - n / 2f) <= meia;
                        tex.SetPixel(x, y, dentro ? Color.white : Color.clear);
                    }
                }
                tex.Apply();
                triangulo = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
            }
            return triangulo;
        }
    }

    // Losango (Coração de Aster).
    public static Sprite Losango
    {
        get
        {
            if (losango == null)
            {
                const int n = 16;
                var tex = NovaTextura(n, n);
                for (int y = 0; y < n; y++)
                    for (int x = 0; x < n; x++)
                    {
                        float d = Mathf.Abs(x + 0.5f - n / 2f) + Mathf.Abs(y + 0.5f - n / 2f);
                        tex.SetPixel(x, y, d <= n / 2f ? Color.white : Color.clear);
                    }
                tex.Apply();
                losango = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
            }
            return losango;
        }
    }

    // Espinhos bem visíveis: duas pontas claras com contorno escuro e ponta vermelha (16x16 px).
    static Sprite espinhos;
    public static Sprite Espinhos
    {
        get
        {
            if (espinhos != null) return espinhos;
            const int n = 16;
            var tex = NovaTextura(n, n);
            var claro = new Color(0.93f, 0.91f, 0.96f);
            var sombra = new Color(0.62f, 0.6f, 0.7f);
            var contorno = new Color(0.08f, 0.06f, 0.1f);
            var ponta = new Color(0.95f, 0.25f, 0.2f);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    tex.SetPixel(x, y, Color.clear);
                    int cx = x < 8 ? 4 : 12;            // centro de cada ponta
                    int meia = (11 - y) / 2;            // largura diminui com a altura (base y=0, topo y=11)
                    int d = Mathf.Abs(x - cx);
                    if (y > 11 || meia < 0) continue;
                    if (d <= meia) tex.SetPixel(x, y, y >= 8 ? ponta : (x <= cx ? claro : sombra));
                    else if (d == meia + 1) tex.SetPixel(x, y, contorno);
                }
            tex.Apply();
            espinhos = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
            return espinhos;
        }
    }

    // Texturas usadas na interface (OnGUI).
    public static Texture2D Pixel
    {
        get
        {
            if (pixel == null)
            {
                pixel = NovaTextura(1, 1);
                pixel.SetPixel(0, 0, Color.white);
                pixel.Apply();
            }
            return pixel;
        }
    }

    public static Texture2D Coracao => coracao != null ? coracao : (coracao = CriarCoracao(true));
    public static Texture2D CoracaoVazio => coracaoVazio != null ? coracaoVazio : (coracaoVazio = CriarCoracao(false));

    // Coração em pixel art 13x12: contorno escuro, brilho no alto à esquerda e sombra embaixo à direita.
    // O vazio mantém só o contorno âmbar apagado, com o miolo escuro.
    static Texture2D CriarCoracao(bool cheio)
    {
        string[] desenho =
        {
            "..XXX...XXX..",
            ".XHHFX.XFFFX.",
            "XHHFFFXFFFFSX",
            "XHFFFFFFFFFSX",
            "XFFFFFFFFFFSX",
            "XFFFFFFFFFSSX",
            ".XFFFFFFFFSX.",
            "..XFFFFFFSX..",
            "...XFFFFSX...",
            "....XFFSX....",
            ".....XSX.....",
            "......X......",
        };
        Color contorno = cheio ? new Color(0.16f, 0.05f, 0.04f) : new Color(0.45f, 0.28f, 0.10f);
        Color brilho = new Color(1f, 0.92f, 0.62f);
        Color miolo = Paleta.Ambar;
        Color sombra = new Color(0.78f, 0.38f, 0.08f);
        Color vazio = new Color(0.08f, 0.06f, 0.10f, 0.75f);

        int w = desenho[0].Length, h = desenho.Length;
        var tex = NovaTextura(w, h);
        for (int linha = 0; linha < h; linha++)
            for (int x = 0; x < w; x++)
            {
                char c = desenho[linha][x];
                Color cor = c switch
                {
                    'X' => contorno,
                    'H' => cheio ? brilho : vazio,
                    'F' => cheio ? miolo : vazio,
                    'S' => cheio ? sombra : vazio,
                    _ => Color.clear
                };
                tex.SetPixel(x, h - 1 - linha, cor);
            }
        tex.Apply();
        return tex;
    }

    static Texture2D NovaTextura(int w, int h)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }
}
