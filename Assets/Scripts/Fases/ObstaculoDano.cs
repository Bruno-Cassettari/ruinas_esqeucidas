using UnityEngine;

// Qualquer obstáculo que causa dano ao toque (espinhos e sentinelas). Não há combate.
// Precisa de um Collider2D marcado como Trigger.
public class ObstaculoDano : MonoBehaviour
{
    public int dano = 1;

    void OnTriggerEnter2D(Collider2D outro) { Acertar(outro); }
    void OnTriggerStay2D(Collider2D outro) { Acertar(outro); } // volta a ferir quando a invulnerabilidade acaba

    void Acertar(Collider2D outro)
    {
        var vida = outro.GetComponentInParent<VidaJogador>();
        if (vida != null) vida.LevarDano(dano, transform.position);
    }
}
