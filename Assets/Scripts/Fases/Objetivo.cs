using UnityEngine;

// Saída da fase (passagem para a próxima região) ou o Coração de Aster (vitória).
public class Objetivo : MonoBehaviour
{
    public bool ehArtefato;

    Vector3 posicaoInicial;

    void Start() { posicaoInicial = transform.position; }

    void Update()
    {
        // O artefato flutua e pulsa sobre o pedestal.
        if (ehArtefato)
        {
            transform.position = posicaoInicial + Vector3.up * Mathf.Sin(Time.time * 2f) * 0.12f;
            float s = 1f + Mathf.Sin(Time.time * 3f) * 0.05f;
            transform.localScale = new Vector3(0.7f * s, 0.9f * s, 1f);
        }
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<VidaJogador>() == null || GameManager.Instancia == null) return;

        if (ehArtefato) GameManager.Instancia.Vitoria();
        else GameManager.Instancia.ConcluirFase();
    }
}
