using UnityEngine;

// Estilos e utilitários de interface (IMGUI/OnGUI), escalados para uma tela virtual de 720 px de altura.
// Fontes (Assets/Resources/Fontes, licença SIL OFL 1.1):
//   Jersey 10     — pixel art grossa, para os títulos (menu, fases, pausa, vitória e derrota)
//   Pixelify Sans — pixel art legível, para HUD, botões e textos
public static class EstiloUI
{
    public const float AlturaVirtual = 720f;

    static GUIStyle titulo, tituloGrande, texto, textoPequeno, textoDestaque, hud, botao;
    static Font fonteTitulo, fonteTexto;
    static bool fontesCarregadas;

    public static float Escala => Screen.height / AlturaVirtual;
    public static float LarguraVirtual => Screen.width / Escala;

    public static readonly Color Sombra = new Color(0f, 0f, 0f, 0.85f);
    public static readonly Color AmbarEscuro = new Color(0.55f, 0.33f, 0.08f);

    public static void AplicarEscala()
    {
        float s = Escala;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1f));
    }

    static void CarregarFontes()
    {
        if (fontesCarregadas) return;
        fontesCarregadas = true;
        fonteTitulo = Resources.Load<Font>("Fontes/Jersey10");
        fonteTexto = Resources.Load<Font>("Fontes/PixelifySans");
    }

    public static GUIStyle TituloGrande => tituloGrande ??= Novo(fonteTituloOuPadrao(), 96, Paleta.Ambar);
    public static GUIStyle Titulo => titulo ??= Novo(fonteTituloOuPadrao(), 68, Paleta.Ambar);
    public static GUIStyle Texto => texto ??= Novo(fonteTextoOuPadrao(), 22, Paleta.TextoClaro);
    public static GUIStyle TextoPequeno => textoPequeno ??= Novo(fonteTextoOuPadrao(), 17, new Color(0.72f, 0.70f, 0.76f));
    public static GUIStyle TextoDestaque => textoDestaque ??= Novo(fonteTextoOuPadrao(), 22, Paleta.Ambar);
    public static GUIStyle Hud => hud ??= Novo(fonteTextoOuPadrao(), 24, Paleta.TextoClaro);

    static Font fonteTituloOuPadrao() { CarregarFontes(); return fonteTitulo; }
    static Font fonteTextoOuPadrao() { CarregarFontes(); return fonteTexto; }

    // Botão em pixel art: fundo escuro, moldura âmbar de cantos cortados; acende ao passar o mouse.
    public static GUIStyle Botao
    {
        get
        {
            if (botao == null)
            {
                botao = new GUIStyle(GUI.skin.button)
                {
                    font = fonteTextoOuPadrao(),
                    fontSize = 24,
                    alignment = TextAnchor.MiddleCenter,
                    border = new RectOffset(4, 4, 4, 4)
                };
                var normal = TexturaBotao(new Color(0.07f, 0.06f, 0.10f, 0.92f), AmbarEscuro);
                var hover = TexturaBotao(new Color(0.20f, 0.13f, 0.06f, 0.95f), Paleta.Ambar);
                botao.normal.background = normal;
                botao.normal.textColor = Paleta.TextoClaro;
                botao.focused.background = normal;
                botao.focused.textColor = Paleta.TextoClaro;
                botao.hover.background = hover;
                botao.hover.textColor = Paleta.Ambar;
                botao.active.background = hover;
                botao.active.textColor = Color.white;
            }
            return botao;
        }
    }

    public static void Retangulo(Rect r, Color cor)
    {
        var antes = GUI.color;
        GUI.color = cor;
        GUI.DrawTexture(r, GeradorSprites.Pixel);
        GUI.color = antes;
    }

    // Painel escuro com moldura âmbar fina (usado no HUD e nas inscrições).
    public static void Painel(Rect r, float alpha = 1f)
    {
        Retangulo(r, new Color(0.03f, 0.02f, 0.05f, 0.78f * alpha));
        var borda = new Color(AmbarEscuro.r, AmbarEscuro.g, AmbarEscuro.b, alpha);
        Retangulo(new Rect(r.x + 2, r.y, r.width - 4, 2), borda);
        Retangulo(new Rect(r.x + 2, r.yMax - 2, r.width - 4, 2), borda);
        Retangulo(new Rect(r.x, r.y + 2, 2, r.height - 4), borda);
        Retangulo(new Rect(r.xMax - 2, r.y + 2, 2, r.height - 4), borda);
    }

    public static void Escurecer(Color cor)
    {
        Retangulo(new Rect(0, 0, LarguraVirtual, AlturaVirtual), cor);
    }

    // Texto com sombra, para ler bem sobre o fundo escuro.
    public static void Rotulo(Rect r, string conteudo, GUIStyle estilo)
    {
        var corOriginal = estilo.normal.textColor;
        var corGui = GUI.color;
        estilo.normal.textColor = new Color(Sombra.r, Sombra.g, Sombra.b, Sombra.a * corOriginal.a);
        GUI.Label(new Rect(r.x + 3, r.y + 3, r.width, r.height), conteudo, estilo);
        estilo.normal.textColor = corOriginal;
        GUI.color = corGui;
        GUI.Label(r, conteudo, estilo);
    }

    public static void TextoCentral(float y, string conteudo, GUIStyle estilo, float altura = 60f)
    {
        Rotulo(new Rect(40, y, LarguraVirtual - 80, altura), conteudo, estilo);
    }

    // Botão centralizado horizontalmente; devolve true quando clicado.
    public static bool BotaoCentral(float y, string rotulo, float largura = 300f)
    {
        var r = new Rect((LarguraVirtual - largura) / 2f, y, largura, 54f);
        return GUI.Button(r, rotulo, Botao);
    }

    static GUIStyle Novo(Font fonte, int tamanho, Color cor)
    {
        var s = new GUIStyle(GUI.skin.label)
        {
            fontSize = tamanho,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            richText = true
        };
        if (fonte != null) s.font = fonte;
        else s.fontStyle = FontStyle.Bold;
        s.normal.textColor = cor;
        return s;
    }

    // Textura 12x12 para 9-slice: contorno preto, moldura colorida de 2 px com cantos cortados e fundo.
    static Texture2D TexturaBotao(Color fundo, Color moldura)
    {
        const int n = 12;
        var t = new Texture2D(n, n, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        var preto = new Color(0f, 0f, 0f, 0.9f);
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                int d = Mathf.Min(Mathf.Min(x, n - 1 - x), Mathf.Min(y, n - 1 - y));
                bool canto = (x < 2 || x > n - 3) && (y < 2 || y > n - 3);
                Color c;
                if (canto && d == 0) c = Color.clear;
                else if (d == 0) c = preto;
                else if (d <= 2 && !(canto && d == 1)) c = moldura;
                else c = fundo;
                t.SetPixel(x, y, c);
            }
        t.Apply();
        return t;
    }
}
