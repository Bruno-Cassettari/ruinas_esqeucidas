using UnityEngine;
using UnityEngine.SceneManagement;

// Menu inicial (cena "Menu"): Jogar, Créditos e Sair, com os controles na parte de baixo.
public class MenuPrincipal : MonoBehaviour
{
    public const string CenaJogo = "Jogo";

    bool mostrandoCreditos;

    // Créditos: (título da seção ou null, linhas). Seções sem título aparecem como texto simples.
    static readonly (string, string[])[] Creditos =
    {
        (null, new[] { "Projeto da disciplina Programação de Jogos · UNITAU, ADS, 2026", "Prof. Moisés Freitas" }),
        ("Desenvolvimento", new[] { "Bruno Cassettari Pereira", "João Pedro Costa Barbosa" }),
        ("Arte", new[] { "Cenário: \"Dark Dungeon\", de Raou", "Personagem: Graphics created by Penzilla Design" }),
        ("Música", new[] { "\"Ancient Ruins\", de Ted Kerr (Wolfgang_), OpenGameArt · CC-BY 3.0" }),
        ("Efeitos sonoros", new[] { "Kenney (kenney.nl) · CC0" }),
        ("Fontes", new[] { "Jersey 10 e Pixelify Sans · Google Fonts, SIL OFL 1.1" }),
        (null, new[] { "Feito com Unity 6" }),
    };

    void Update()
    {
        if (mostrandoCreditos && Entrada.Pausa()) mostrandoCreditos = false;
    }

    void Start()
    {
        Time.timeScale = 1f;
        var cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Paleta.Fundo;
            cam.transform.position = new Vector3(0f, 0f, -10f);
        }

        // Trilha ambiente também no menu.
        gameObject.AddComponent<AudioJogo>();

        // Fundo do menu com a arte do Dark Dungeon (se estiver no projeto).
        var parede = ArteJogo.Recorte(10, 28, 17, 36);
        for (int i = -2; i <= 2; i++)
        {
            CriarFundo(parede, new Vector2(i * 7f, -6f), -100, new Color(0.4f, 0.38f, 0.5f));
            CriarFundo(parede, new Vector2(i * 7f, 2f), -101, new Color(0.22f, 0.21f, 0.3f));
        }
        var tochas = ArteJogo.Tocha;
        if (tochas.Length > 0)
            foreach (float x in new[] { -6f, 6f })
            {
                var t = CriarFundo(tochas[0], new Vector2(x, -1f), -90, Color.white);
                t.AddComponent<AnimacaoSimples>().quadros = tochas;
            }
    }

    static GameObject CriarFundo(Sprite sprite, Vector2 posicao, int ordem, Color cor)
    {
        if (sprite == null) return null;
        var go = new GameObject("Fundo");
        go.transform.position = posicao;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = cor;
        sr.sortingOrder = ordem;
        return go;
    }

    void OnGUI()
    {
        EstiloUI.AplicarEscala();

        if (mostrandoCreditos) { DesenharCreditos(); return; }

        // Pequeno losango âmbar pulsando (o Coração de Aster) acima do título.
        float pulso = 0.6f + 0.4f * Mathf.Sin(Time.time * 2f);
        var antes = GUI.color;
        float cx = EstiloUI.LarguraVirtual / 2f;
        GUI.color = new Color(Paleta.Ambar.r, Paleta.Ambar.g, Paleta.Ambar.b, 0.25f * pulso);
        GUI.DrawTexture(new Rect(cx - 30, 92, 60, 60), GeradorSprites.Losango.texture);
        GUI.color = new Color(Paleta.Ambar.r, Paleta.Ambar.g, Paleta.Ambar.b, pulso);
        GUI.DrawTexture(new Rect(cx - 14, 108, 28, 28), GeradorSprites.Losango.texture);
        GUI.color = antes;

        EstiloUI.TextoCentral(145, "Ruínas Esquecidas", EstiloUI.TituloGrande, 120);
        EstiloUI.TextoCentral(268, "\"Quando tudo é esquecido, alguém precisa lembrar.\"", EstiloUI.TextoPequeno, 30);

        if (EstiloUI.BotaoCentral(345, "Jogar"))
        {
            GameManager.faseAtual = 0;
            EstatisticasPartida.Reiniciar();
            SceneManager.LoadScene(CenaJogo);
        }
        if (EstiloUI.BotaoCentral(415, "Créditos")) mostrandoCreditos = true;
#if !UNITY_WEBGL
        // No navegador (WebGL) não existe "fechar o jogo"; o botão Sair só aparece no Windows.
        if (EstiloUI.BotaoCentral(485, "Sair")) Sair();
#endif

        EstiloUI.TextoCentral(EstiloUI.AlturaVirtual - 70, "A/D ou setas: andar    Espaço: pular    Esc ou P: pausar", EstiloUI.TextoPequeno, 30);
    }

    void DesenharCreditos()
    {
        EstiloUI.Escurecer(new Color(0f, 0f, 0f, 0.82f));
        EstiloUI.TextoCentral(28, "Créditos", EstiloUI.Titulo, 80);

        float y = 120f;
        foreach (var (secao, linhas) in Creditos)
        {
            if (secao != null)
            {
                EstiloUI.TextoCentral(y, secao, EstiloUI.TextoDestaque, 30);
                y += 30f;
            }
            foreach (var linha in linhas)
            {
                EstiloUI.TextoCentral(y, linha, EstiloUI.TextoPequeno, 24);
                y += 24f;
            }
            y += 12f;
        }

        if (EstiloUI.BotaoCentral(610, "Voltar")) mostrandoCreditos = false;
    }

    void Sair()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
