using System.Collections;
using UnityEngine;

// Sistema de vida: 3 corações, invulnerabilidade curta após o dano (personagem pisca).
public class VidaJogador : MonoBehaviour
{
    public int vidaMaxima = 3;
    public float tempoInvulneravel = 1.2f;

    public int Vida { get; private set; }
    public bool Morto => Vida <= 0;
    public bool Invulneravel => Time.time < invulneravelAte;
    public float UltimoDano { get; private set; } = -99f;

    public System.Action AoLevarDano;
    public System.Action AoMorrer;

    float invulneravelAte;
    PlayerController controle;
    SpriteRenderer[] visuais;

    void Awake()
    {
        Vida = vidaMaxima;
        controle = GetComponent<PlayerController>();
        visuais = GetComponentsInChildren<SpriteRenderer>();
    }

    public void LevarDano(int quantidade, Vector2 origem)
    {
        if (Morto || Invulneravel) return;

        Vida = Mathf.Max(0, Vida - quantidade);
        UltimoDano = Time.time;
        AoLevarDano?.Invoke();

        if (Morto)
        {
            AoMorrer?.Invoke();
            return;
        }

        invulneravelAte = Time.time + tempoInvulneravel;
        if (controle != null) controle.Recuar(origem);
        StartCoroutine(Piscar());
    }

    // Queda no abismo: perde todos os corações de uma vez (requisito da Etapa 2).
    public void Matar()
    {
        if (Morto) return;
        Vida = 0;
        AoLevarDano?.Invoke();
        AoMorrer?.Invoke();
    }

    IEnumerator Piscar()
    {
        bool aceso = true;
        while (Invulneravel)
        {
            aceso = !aceso;
            foreach (var sr in visuais) sr.enabled = aceso;
            yield return new WaitForSeconds(0.08f);
        }
        foreach (var sr in visuais) sr.enabled = true;
    }
}
