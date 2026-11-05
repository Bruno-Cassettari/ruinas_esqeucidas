using UnityEngine;

// Camadas de fundo que se movem mais devagar que a câmera, dando profundidade (parallax).
// Cada camada repete o mesmo sprite na horizontal para cobrir a fase inteira.
public class FundoParallax : MonoBehaviour
{
    public Transform cameraAlvo;
    public float fatorX = 0.5f;   // 0 = acompanha o cenário, 1 = fica parado em relação à câmera
    public float fatorY = 0.8f;
    public float alturaBase;     // deslocamento vertical

    void LateUpdate()
    {
        if (cameraAlvo == null) return;
        Vector3 c = cameraAlvo.position;
        transform.position = new Vector3(c.x * fatorX, c.y * fatorY + alturaBase, 0f);
    }

    // espacamento = distância entre cópias (0 = encostadas umas nas outras).
    public static FundoParallax Criar(string nome, Sprite sprite, Transform cam, float fatorX, float fatorY, float alturaBase,
                                      int ordem, float larguraFase, Transform pai, Color cor, float espacamento = 0f)
    {
        if (sprite == null) return null;

        var raiz = new GameObject("Fundo - " + nome);
        raiz.transform.SetParent(pai, false);
        var p = raiz.AddComponent<FundoParallax>();
        p.cameraAlvo = cam;
        p.fatorX = fatorX;
        p.fatorY = fatorY;
        p.alturaBase = alturaBase;

        float passo = Mathf.Max(sprite.bounds.size.x - 0.01f, espacamento);
        float inicio = -passo * 2f - 12f;
        float fim = larguraFase * (1f - fatorX) + passo * 2f + 12f;
        for (float x = inicio; x < fim; x += passo)
        {
            var go = new GameObject(nome);
            go.transform.SetParent(raiz.transform, false);
            go.transform.localPosition = new Vector3(x, 0f, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = cor;
            sr.sortingOrder = ordem;
        }
        p.LateUpdate();
        return p;
    }
}
