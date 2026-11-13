using UnityEngine;

// Inscrição nas ruínas: ao se aproximar, o texto aparece na parte de baixo da tela.
public class Inscricao : MonoBehaviour
{
    [TextArea] public string texto;

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<VidaJogador>() != null && HUD.Instancia != null)
            HUD.Instancia.MostrarInscricao(texto);
    }

    void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<VidaJogador>() != null && HUD.Instancia != null)
            HUD.Instancia.EsconderInscricao(texto);
    }
}
