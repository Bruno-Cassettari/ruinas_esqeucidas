using UnityEngine;

// Saída da fase: ao entrar, a fase é concluída.
public class Objetivo : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<VidaJogador>() != null && GameManager.Instancia != null)
            GameManager.Instancia.ConcluirFase();
    }
}
