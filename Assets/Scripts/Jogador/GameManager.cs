using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Controla o fluxo da cena "Jogo": monta a fase, cria o jogador e a câmera,
// reinicia a fase quando o jogador morre e mostra a conclusão da fase.
// Etapa 4: versão parcial, só com a Fase 1. Telas de vitória/derrota completas entram em 09-13/11.
public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    // Fase atual (0 = Fase 1). Estático para sobreviver ao recarregamento da cena.
    public static int faseAtual = 0;

    public const string CenaMenu = "Menu";

    PlayerController jogador;
    VidaJogador vida;
    bool faseConcluida;
    bool reiniciando;

    void Awake()
    {
        Instancia = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        faseAtual = Mathf.Clamp(faseAtual, 0, Fases.Todas.Length - 1);
        var dados = Fases.Todas[faseAtual];

        var raizFase = new GameObject("Fase " + (faseAtual + 1) + " - " + dados.nome).transform;
        var fase = ConstrutorDeFase.Construir(dados, raizFase);

        jogador = CriarJogador(fase.inicioJogador);
        vida = jogador.GetComponent<VidaJogador>();
        vida.AoMorrer += AoMorrer;

        var hud = gameObject.AddComponent<HUD>();
        hud.vida = vida;
        hud.faseNumero = faseAtual + 1;
        hud.totalFases = Fases.TotalPlanejado;
        hud.nomeFase = dados.nome;

        ConfigurarCamera(dados, fase.limites);
    }

    // ---------- Fluxo ----------

    public void ConcluirFase()
    {
        if (faseConcluida || reiniciando) return;

        if (faseAtual < Fases.Todas.Length - 1)
        {
            faseAtual++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        // Última fase disponível na versão parcial.
        faseConcluida = true;
        jogador.Parar();
        Time.timeScale = 0f;
    }

    // Vida zerada ou queda no abismo: por enquanto a fase reinicia sozinha.
    void AoMorrer()
    {
        if (reiniciando || faseConcluida) return;
        reiniciando = true;
        jogador.Parar();
        StartCoroutine(ReiniciarDepois(1f));
    }

    IEnumerator ReiniciarDepois(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        ReiniciarFase();
    }

    void ReiniciarFase()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void IrParaMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(CenaMenu);
    }

    // ---------- Montagem ----------

    PlayerController CriarJogador(Vector2 posicao)
    {
        var go = new GameObject("Elias");
        go.transform.position = posicao;

        const float alturaColisor = 1.15f;
        SpriteRenderer visualHeroi = null;
        if (ArteJogo.HeroiDisponivel && ArteJogo.HeroiParado.Length > 0)
        {
            // Personagem encapuzado (Hooded Protagonist, Penzilla Design), com os pés na base do colisor.
            var v = new GameObject("Visual");
            v.transform.SetParent(go.transform, false);
            v.transform.localPosition = new Vector3(0f, -alturaColisor / 2f, 0f);
            visualHeroi = v.AddComponent<SpriteRenderer>();
            visualHeroi.sprite = ArteJogo.HeroiParado[0];
            visualHeroi.sortingOrder = 5;
        }
        else
        {
            // Visual provisório: corpo com capuz, mochila e a pequena luz âmbar no peito.
            CriarParte("Mochila", go.transform, new Vector2(-0.3f, 0.05f), new Vector2(0.25f, 0.5f), new Color(0.28f, 0.22f, 0.16f), 4);
            CriarParte("Corpo", go.transform, Vector2.zero, new Vector2(0.62f, 0.9f), Paleta.Jogador, 5);
            CriarParte("Capuz", go.transform, new Vector2(0.04f, 0.33f), new Vector2(0.5f, 0.28f), new Color(0.09f, 0.10f, 0.13f), 6);
            CriarParte("Luz", go.transform, new Vector2(0.14f, 0.02f), new Vector2(0.14f, 0.14f), Paleta.Ambar, 7);
        }

        go.transform.position = posicao + new Vector2(0f, alturaColisor / 2f - 0.45f);
        go.AddComponent<Rigidbody2D>();
        var col = go.AddComponent<CapsuleCollider2D>();
        col.direction = CapsuleDirection2D.Vertical;
        col.size = visualHeroi != null ? new Vector2(0.55f, alturaColisor) : new Vector2(0.62f, 0.92f);

        var controle = go.AddComponent<PlayerController>();
        go.AddComponent<VidaJogador>();
        if (visualHeroi != null) go.AddComponent<AnimacaoJogador>().visual = visualHeroi;
        return controle;
    }

    static void CriarParte(string nome, Transform pai, Vector2 local, Vector2 tamanho, Color cor, int ordem)
    {
        var p = new GameObject(nome);
        p.transform.SetParent(pai, false);
        p.transform.localPosition = local;
        p.transform.localScale = new Vector3(tamanho.x, tamanho.y, 1f);
        var sr = p.AddComponent<SpriteRenderer>();
        sr.sprite = GeradorSprites.Quadrado;
        sr.color = cor;
        sr.sortingOrder = ordem;
    }

    void ConfigurarCamera(DadosFase dados, Rect limites)
    {
        var cam = Camera.main;
        if (cam == null)
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            cam = go.AddComponent<Camera>();
            go.AddComponent<AudioListener>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = ArteJogo.Disponivel ? new Color(0.035f, 0.03f, 0.06f) : dados.corFundo;

        var seguir = cam.GetComponent<CameraSeguir>();
        if (seguir == null) seguir = cam.gameObject.AddComponent<CameraSeguir>();
        seguir.alvo = jogador.transform;
        seguir.DefinirLimites(limites);
        seguir.PosicionarImediato();

        if (!ArteJogo.Disponivel) return;

        // Fundo em camadas (Dark Dungeon): paredes com arcos ao longe e colunas com tochas mais perto.
        var fundos = new GameObject("Fundos").transform;
        float largura = limites.width;
        var parede = ArteJogo.Recorte(10, 28, 17, 36);
        FundoParallax.Criar("parede", parede, cam.transform, 0.6f, 0.9f, -5.5f, -100, largura, fundos, new Color(0.42f, 0.4f, 0.52f));
        FundoParallax.Criar("parede-alta", parede, cam.transform, 0.6f, 0.9f, 2.5f, -101, largura, fundos, new Color(0.25f, 0.24f, 0.32f));

        var coluna = ArteJogo.Recorte(5, 9, 6, 13);
        var corColuna = new Color(0.62f, 0.6f, 0.72f);
        FundoParallax.Criar("coluna", coluna, cam.transform, 0.3f, 1f, -6f, -90, largura, fundos, corColuna, 9f);
        FundoParallax.Criar("coluna-alta", coluna, cam.transform, 0.3f, 1f, -2f, -90, largura, fundos, corColuna, 9f);

        var tochas = ArteJogo.Tocha;
        if (tochas.Length > 0)
        {
            var t = FundoParallax.Criar("tocha", tochas[0], cam.transform, 0.3f, 1f, -3.2f, -89, largura, fundos, Color.white, 9f);
            if (t != null)
                foreach (Transform filho in t.transform)
                    filho.gameObject.AddComponent<AnimacaoSimples>().quadros = tochas;
        }
    }

    // ---------- Tela de fase concluída ----------

    void OnGUI()
    {
        GUI.depth = 0;
        if (!faseConcluida) return;
        EstiloUI.AplicarEscala();

        EstiloUI.Escurecer(new Color(0f, 0f, 0f, 0.8f));
        EstiloUI.TextoCentral(170, "Fase 1 concluída!", EstiloUI.Titulo, 70);
        EstiloUI.TextoCentral(245, "Versão parcial (Etapa 4) — as Fases 2 e 3 estão em desenvolvimento.", EstiloUI.TextoPequeno, 30);
        if (EstiloUI.BotaoCentral(320, "Jogar novamente")) { faseAtual = 0; ReiniciarFase(); }
        if (EstiloUI.BotaoCentral(390, "Menu inicial")) IrParaMenu();
    }
}
