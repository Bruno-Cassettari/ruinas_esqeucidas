using UnityEngine;

// HUD básico (Documento de História, seção 23): três corações no canto superior esquerdo,
// "FASE n/3" no canto superior direito, o nome da fase ao entrar e a caixa de inscrições.
public class HUD : MonoBehaviour
{
    public static HUD Instancia { get; private set; }

    public VidaJogador vida;
    public int faseNumero = 1;
    public int totalFases = 3;
    public string nomeFase = "";

    float tempoTitulo = 3f;
    string inscricaoAtual, ultimaInscricao;
    float alphaInscricao;

    void Awake() { Instancia = this; }

    public void MostrarInscricao(string texto) { inscricaoAtual = texto; }

    public void EsconderInscricao(string texto)
    {
        if (inscricaoAtual == texto) inscricaoAtual = null;
    }

    int vidaAnterior = -1;
    float tempoPulso;       // anima o coração perdido
    int coracaoPerdido = -1;

    void Update()
    {
        tempoTitulo -= Time.deltaTime;
        float alvo = string.IsNullOrEmpty(inscricaoAtual) ? 0f : 1f;
        alphaInscricao = Mathf.MoveTowards(alphaInscricao, alvo, Time.unscaledDeltaTime * 4f);

        if (vida != null)
        {
            if (vidaAnterior >= 0 && vida.Vida < vidaAnterior) { coracaoPerdido = vida.Vida; tempoPulso = 0.5f; }
            vidaAnterior = vida.Vida;
        }
        tempoPulso -= Time.deltaTime;
    }

    void OnGUI()
    {
        GUI.depth = 10; // telas de pausa, vitória e derrota ficam por cima
        EstiloUI.AplicarEscala();
        float largura = EstiloUI.LarguraVirtual;

        // Corações, num painel escuro
        if (vida != null)
        {
            const float w = 39f, h = 36f, passo = 46f;
            EstiloUI.Painel(new Rect(16, 14, 22 + vida.vidaMaxima * passo - (passo - w), h + 20));
            for (int i = 0; i < vida.vidaMaxima; i++)
            {
                var r = new Rect(27 + i * passo, 24, w, h);
                bool cheio = i < vida.Vida;
                if (i == coracaoPerdido && tempoPulso > 0f)
                {
                    // Pisca e "salta" no momento do dano
                    float k = tempoPulso / 0.5f;
                    float esc = 1f + 0.35f * Mathf.Sin(k * Mathf.PI);
                    r = new Rect(r.center.x - w * esc / 2f, r.center.y - h * esc / 2f, w * esc, h * esc);
                    cheio = Mathf.Repeat(tempoPulso, 0.12f) < 0.06f;
                }
                GUI.DrawTexture(r, cheio ? GeradorSprites.Coracao : GeradorSprites.CoracaoVazio);
            }
        }

        // Fase atual
        var painelFase = new Rect(largura - 166, 14, 150, 56);
        EstiloUI.Painel(painelFase);
        EstiloUI.Rotulo(painelFase, $"Fase {faseNumero}/{totalFases}", EstiloUI.Hud);

        // Nome da fase ao entrar (some em ~1 s no fim)
        if (tempoTitulo > 0f)
        {
            var cor = GUI.color;
            GUI.color = new Color(1, 1, 1, Mathf.Clamp01(tempoTitulo));
            EstiloUI.TextoCentral(140, nomeFase, EstiloUI.Titulo, 90);
            EstiloUI.TextoCentral(228, $"Fase {faseNumero}", EstiloUI.TextoPequeno, 30);
            GUI.color = cor;
        }

        // Inscrição (texto narrativo das ruínas)
        if (!string.IsNullOrEmpty(inscricaoAtual)) ultimaInscricao = inscricaoAtual;
        if (alphaInscricao > 0f && !string.IsNullOrEmpty(ultimaInscricao))
        {
            var caixa = new Rect(largura / 2f - 430, EstiloUI.AlturaVirtual - 150, 860, 112);
            var corGui = GUI.color;
            GUI.color = new Color(1, 1, 1, alphaInscricao);
            EstiloUI.Painel(caixa, alphaInscricao);
            EstiloUI.Rotulo(new Rect(caixa.x + 28, caixa.y + 8, caixa.width - 56, caixa.height - 16), ultimaInscricao, EstiloUI.Texto);
            GUI.color = corGui;
        }
    }
}
