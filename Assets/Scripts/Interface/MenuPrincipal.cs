using UnityEngine;
using UnityEngine.SceneManagement;

// Menu inicial (cena "Menu"): Jogar e Sair, com os controles na parte de baixo.
public class MenuPrincipal : MonoBehaviour
{
    public const string CenaJogo = "Jogo";

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

        // Pequeno losango âmbar pulsando (o Coração de Aster) acima do título.
        float pulso = 0.6f + 0.4f * Mathf.Sin(Time.time * 2f);
        var antes = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, pulso);
        float cx = EstiloUI.LarguraVirtual / 2f;
        GUI.DrawTexture(new Rect(cx - 14, 120, 28, 28), GeradorSprites.Losango.texture);
        GUI.color = antes;

        EstiloUI.TextoCentral(170, "RUÍNAS ESQUECIDAS", EstiloUI.Titulo, 80);
        EstiloUI.TextoCentral(245, "\"Quando tudo é esquecido, alguém precisa lembrar.\"", EstiloUI.TextoPequeno, 30);

        if (EstiloUI.BotaoCentral(340, "Jogar"))
        {
            GameManager.faseAtual = 0;
            SceneManager.LoadScene(CenaJogo);
        }
        if (EstiloUI.BotaoCentral(410, "Sair")) Sair();

        EstiloUI.TextoCentral(EstiloUI.AlturaVirtual - 70, "A/D ou setas: andar    Espaço: pular", EstiloUI.TextoPequeno, 30);
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
