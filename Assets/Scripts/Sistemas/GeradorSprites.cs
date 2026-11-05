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

    public static Texture2D Coracao => coracao != null ? coracao : (coracao = CriarCoracao(Paleta.Ambar));
    public static Texture2D CoracaoVazio => coracaoVazio != null ? coracaoVazio : (coracaoVazio = CriarCoracao(new Color(0.3f, 0.3f, 0.34f)));

    static Texture2D CriarCoracao(Color cor)
    {
        string[] desenho =
        {
            ".XX.XX.",
            "XXXXXXX",
            "XXXXXXX",
            ".XXXXX.",
            "..XXX..",
            "...X...",
        };
        int w = desenho[0].Length, h = desenho.Length;
        var tex = NovaTextura(w, h);
        for (int linha = 0; linha < h; linha++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, h - 1 - linha, desenho[linha][x] == 'X' ? cor : Color.clear);
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
