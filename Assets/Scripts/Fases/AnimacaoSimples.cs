using UnityEngine;

// Repete uma lista de sprites em loop (ex.: tochas).
[RequireComponent(typeof(SpriteRenderer))]
public class AnimacaoSimples : MonoBehaviour
{
    public Sprite[] quadros;
    public float quadrosPorSegundo = 8f;

    SpriteRenderer sr;
    float deslocamento;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        deslocamento = Random.value * 3f; // tochas não piscam todas juntas
    }

    void Update()
    {
        if (quadros == null || quadros.Length == 0) return;
        int i = (int)((Time.time + deslocamento) * quadrosPorSegundo) % quadros.Length;
        sr.sprite = quadros[i];
    }
}
