using UnityEngine;

// HUD básico (Documento de História, seção 23): três corações no canto superior esquerdo,
// "FASE n/3" no canto superior direito e o nome da fase ao entrar.
public class HUD : MonoBehaviour
{
    public VidaJogador vida;
    public int faseNumero = 1;
    public int totalFases = 3;
    public string nomeFase = "";

    float tempoTitulo = 3f;

    void Update()
    {
        tempoTitulo -= Time.deltaTime;
    }

    void OnGUI()
    {
        GUI.depth = 10; // a tela de fase concluída fica por cima
        EstiloUI.AplicarEscala();
        float largura = EstiloUI.LarguraVirtual;

        // Corações
        if (vida != null)
        {
            for (int i = 0; i < vida.vidaMaxima; i++)
            {
                var tex = i < vida.Vida ? GeradorSprites.Coracao : GeradorSprites.CoracaoVazio;
                GUI.DrawTexture(new Rect(24 + i * 50, 22, 42, 36), tex);
            }
        }

        // Fase atual
        var estiloFase = new GUIStyle(EstiloUI.Texto) { alignment = TextAnchor.MiddleRight };
        GUI.Label(new Rect(largura - 224, 18, 200, 44), $"FASE {faseNumero}/{totalFases}", estiloFase);

        // Nome da fase ao entrar (some em ~1 s no fim)
        if (tempoTitulo > 0f)
        {
            var cor = GUI.color;
            GUI.color = new Color(1, 1, 1, Mathf.Clamp01(tempoTitulo));
            EstiloUI.TextoCentral(150, nomeFase, EstiloUI.Titulo, 70);
            EstiloUI.TextoCentral(215, $"Fase {faseNumero}", EstiloUI.TextoPequeno, 30);
            GUI.color = cor;
        }
    }
}
