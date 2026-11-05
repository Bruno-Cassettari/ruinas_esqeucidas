using UnityEngine;

// Movimentação do Elias: corrida, pulo e colisão (Rigidbody2D + CapsuleCollider2D).
// Usa coyote time e jump buffer (plano de contingência da Etapa 3 para "pulo com sensação ruim").
[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Corrida")]
    public float velocidade = 7f;
    public float aceleracao = 70f;
    public float desaceleracao = 80f;

    [Header("Pulo")]
    public float forcaPulo = 13.5f;
    public float gravidade = 3f;
    public float multiplicadorQueda = 1.6f;    // cai mais rápido do que sobe
    public float multiplicadorPuloCurto = 2.4f; // soltar o botão encurta o pulo
    public float velocidadeQuedaMax = 20f;
    public float coyoteTime = 0.1f;             // ainda pode pular logo após sair da borda
    public float jumpBuffer = 0.12f;            // aperto um pouco antes de tocar o chão conta

    public bool controleAtivo = true;
    public bool NoChao { get; private set; }
    public System.Action AoPular;

    Rigidbody2D rb;
    CapsuleCollider2D col;
    SpriteRenderer visual;
    readonly RaycastHit2D[] hits = new RaycastHit2D[8];
    ContactFilter2D filtroChao;

    float entradaH;
    float tempoDesdeChao = 99f;
    float tempoDesdeApertoPulo = 99f;
    float travaControle;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        visual = GetComponentInChildren<SpriteRenderer>();

        rb.gravityScale = gravidade;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // evita atravessar paredes
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;                  // triggers continuam detectando parado

        // Sem atrito, para o personagem não "grudar" nas paredes.
        col.sharedMaterial = new PhysicsMaterial2D("SemAtrito") { friction = 0f, bounciness = 0f };

        filtroChao = new ContactFilter2D();
        filtroChao.useTriggers = false;
    }

    void Update()
    {
        if (!controleAtivo) { entradaH = 0f; return; }

        entradaH = Entrada.Horizontal();
        tempoDesdeApertoPulo += Time.deltaTime;
        if (Entrada.PuloPressionado()) tempoDesdeApertoPulo = 0f;

        if (visual != null && entradaH != 0f)
            transform.localScale = new Vector3(entradaH < 0 ? -1f : 1f, 1f, 1f);
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        Vector2 v = rb.linearVelocity;

        NoChao = v.y <= 0.01f && ChecarChao();
        tempoDesdeChao = NoChao ? 0f : tempoDesdeChao + dt;
        travaControle -= dt;

        // Corrida com aceleração simples.
        if (travaControle <= 0f)
        {
            float alvo = entradaH * velocidade;
            float taxa = Mathf.Abs(alvo) > 0.01f ? aceleracao : desaceleracao;
            v.x = Mathf.MoveTowards(v.x, alvo, taxa * dt);
        }

        // Pulo (com coyote time + jump buffer).
        if (controleAtivo && travaControle <= 0f && tempoDesdeApertoPulo <= jumpBuffer && tempoDesdeChao <= coyoteTime)
        {
            v.y = forcaPulo;
            tempoDesdeApertoPulo = 99f;
            tempoDesdeChao = 99f;
            AoPular?.Invoke();
        }

        // Gravidade variável: queda mais pesada e pulo curto ao soltar o botão.
        if (v.y < 0f) rb.gravityScale = gravidade * multiplicadorQueda;
        else if (v.y > 0f && !Entrada.PuloSegurado()) rb.gravityScale = gravidade * multiplicadorPuloCurto;
        else rb.gravityScale = gravidade;

        v.y = Mathf.Max(v.y, -velocidadeQuedaMax);
        rb.linearVelocity = v;
    }

    bool ChecarChao()
    {
        int n = col.Cast(Vector2.down, filtroChao, hits, 0.06f);
        for (int i = 0; i < n; i++)
            if (hits[i].normal.y > 0.6f) return true;
        return false;
    }

    // Empurrão ao levar dano, para longe da origem do dano.
    public void Recuar(Vector2 origem)
    {
        float lado = Mathf.Sign(transform.position.x - origem.x);
        if (lado == 0f) lado = 1f;
        rb.linearVelocity = new Vector2(lado * 6f, 9f);
        travaControle = 0.25f;
    }

    public void Parar()
    {
        controleAtivo = false;
        rb.linearVelocity = Vector2.zero;
    }
}
