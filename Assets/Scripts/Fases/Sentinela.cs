using UnityEngine;

// Obstáculo que só se move em padrão fixo (ida e volta), sem perseguir o jogador:
// armaduras que patrulham na horizontal e relicários que sobem e descem.
// Causa dano ao toque por meio do componente ObstaculoDano.
[RequireComponent(typeof(Rigidbody2D))]
public class Sentinela : MonoBehaviour
{
    public Vector2 direcao = Vector2.right;
    public float amplitude = 2.5f;   // distância máxima a partir do ponto inicial
    public float velocidade = 1.6f;  // rapidez do ciclo
    public float fase;               // desloca o ciclo para as sentinelas não andarem juntas
    public SpriteRenderer visual;    // virado para o lado em que anda

    Rigidbody2D rb;
    Vector2 origem;
    float ultimoX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        origem = transform.position;
        ultimoX = origem.x;
    }

    void FixedUpdate()
    {
        float t = Mathf.Sin(Time.time * velocidade + fase);
        Vector2 alvo = origem + direcao.normalized * amplitude * t;
        rb.MovePosition(alvo);

        if (visual != null && Mathf.Abs(alvo.x - ultimoX) > 0.0001f)
            visual.flipX = alvo.x < ultimoX;
        ultimoX = alvo.x;
    }
}
