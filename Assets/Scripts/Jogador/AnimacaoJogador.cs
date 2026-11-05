using UnityEngine;

// Troca os quadros do personagem conforme o estado: parado, correndo, subindo/caindo ou levando dano.
public class AnimacaoJogador : MonoBehaviour
{
    public SpriteRenderer visual;

    PlayerController controle;
    VidaJogador vida;
    Rigidbody2D rb;
    float tempo;
    Sprite[] atual;

    void Awake()
    {
        controle = GetComponent<PlayerController>();
        vida = GetComponent<VidaJogador>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (visual == null) return;

        // Logo após o dano, mostra o quadro de "atingido".
        if (vida != null && Time.time - vida.UltimoDano < 0.3f && ArteJogo.HeroiDano != null)
        {
            visual.sprite = ArteJogo.HeroiDano;
            return;
        }

        Vector2 v = rb.linearVelocity;
        Sprite[] alvo;
        float fps;
        if (!controle.NoChao) { alvo = v.y > 0f ? ArteJogo.HeroiSubindo : ArteJogo.HeroiCaindo; fps = 8f; }
        else if (Mathf.Abs(v.x) > 0.5f) { alvo = ArteJogo.HeroiCorrendo; fps = 14f; }
        else { alvo = ArteJogo.HeroiParado; fps = 2.5f; }

        if (alvo == null || alvo.Length == 0) return;
        if (alvo != atual) { atual = alvo; tempo = 0f; }
        tempo += Time.deltaTime;

        int quadro = (int)(tempo * fps);
        // No ar, segura o último quadro em vez de repetir.
        quadro = controle.NoChao ? quadro % alvo.Length : Mathf.Min(quadro, alvo.Length - 1);
        visual.sprite = alvo[quadro];
    }
}
