using UnityEngine;

// Estilos e utilitários de interface (IMGUI/OnGUI), escalados para uma tela virtual de 720 px de altura.
// IMGUI foi escolhido na versão parcial por não depender de prefabs nem de TextMeshPro.
public static class EstiloUI
{
    public const float AlturaVirtual = 720f;

    static GUIStyle titulo, texto, textoPequeno, botao;
    static Texture2D fundoBotao, fundoBotaoHover;

    public static float Escala => Screen.height / AlturaVirtual;
    public static float LarguraVirtual => Screen.width / Escala;

    public static void AplicarEscala()
    {
        float s = Escala;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1f));
    }

    public static GUIStyle Titulo => titulo ??= Novo(46, Paleta.Ambar, FontStyle.Bold);
    public static GUIStyle Texto => texto ??= Novo(22, Paleta.TextoClaro, FontStyle.Normal);
    public static GUIStyle TextoPequeno => textoPequeno ??= Novo(16, new Color(0.65f, 0.65f, 0.68f), FontStyle.Normal);

    public static GUIStyle Botao
    {
        get
        {
            if (botao == null)
            {
                fundoBotao = Cor(new Color(0.10f, 0.11f, 0.15f, 0.95f));
                fundoBotaoHover = Cor(new Color(0.22f, 0.17f, 0.10f, 0.95f));
                botao = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 22,
                    alignment = TextAnchor.MiddleCenter
                };
                botao.normal.background = fundoBotao;
                botao.normal.textColor = Paleta.TextoClaro;
                botao.hover.background = fundoBotaoHover;
                botao.hover.textColor = Paleta.Ambar;
                botao.active.background = fundoBotaoHover;
                botao.active.textColor = Color.white;
                botao.focused.background = fundoBotao;
                botao.focused.textColor = Paleta.TextoClaro;
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

    public static void Escurecer(Color cor)
    {
        Retangulo(new Rect(0, 0, LarguraVirtual, AlturaVirtual), cor);
    }

    // Botão centralizado horizontalmente; devolve true quando clicado.
    public static bool BotaoCentral(float y, string rotulo, float largura = 280f)
    {
        var r = new Rect((LarguraVirtual - largura) / 2f, y, largura, 52f);
        Retangulo(new Rect(r.x - 1, r.y - 1, r.width + 2, r.height + 2), new Color(Paleta.Ambar.r, Paleta.Ambar.g, Paleta.Ambar.b, 0.5f));
        return GUI.Button(r, rotulo, Botao);
    }

    public static void TextoCentral(float y, string conteudo, GUIStyle estilo, float altura = 60f)
    {
        GUI.Label(new Rect(0, y, LarguraVirtual, altura), conteudo, estilo);
    }

    static GUIStyle Novo(int tamanho, Color cor, FontStyle fonte)
    {
        var s = new GUIStyle(GUI.skin.label)
        {
            fontSize = tamanho,
            fontStyle = fonte,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            richText = true
        };
        s.normal.textColor = cor;
        return s;
    }

    static Texture2D Cor(Color c)
    {
        var t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        return t;
    }
}
