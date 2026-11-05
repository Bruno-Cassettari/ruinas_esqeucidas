using UnityEngine;

// Faixa invisível abaixo da fase: cair no abismo = derrota.
public class ZonaDeQueda : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D outro)
    {
        var vida = outro.GetComponentInParent<VidaJogador>();
        if (vida != null) vida.Matar();
    }
}
