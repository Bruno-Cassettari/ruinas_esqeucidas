using UnityEngine;

// Trilha ambiente e efeitos sonoros. Os arquivos ficam em Assets/Resources/Audio:
//   trilha.ogg   "Dungeon Ambience", de yd (OpenGameArt, CC0)
//   pulo, dano, derrota, passagem, vitoria   pacotes da Kenney (kenney.nl, CC0)
public class AudioJogo : MonoBehaviour
{
    public static AudioJogo Instancia { get; private set; }

    [Range(0f, 1f)] public float volumeTrilha = 0.45f;
    [Range(0f, 1f)] public float volumeEfeitos = 0.8f;
    public bool tocarTrilha = true;

    AudioSource fonteTrilha, fonteEfeitos;
    AudioClip pulo, dano, derrota, passagem, vitoria;

    void Awake()
    {
        Instancia = this;
        pulo = Carregar("pulo");
        dano = Carregar("dano");
        derrota = Carregar("derrota");
        passagem = Carregar("passagem");
        vitoria = Carregar("vitoria");

        fonteEfeitos = gameObject.AddComponent<AudioSource>();
        fonteEfeitos.playOnAwake = false;
        fonteEfeitos.volume = volumeEfeitos;

        fonteTrilha = gameObject.AddComponent<AudioSource>();
        fonteTrilha.playOnAwake = false;
        fonteTrilha.loop = true;
        fonteTrilha.volume = volumeTrilha;
        fonteTrilha.clip = Carregar("trilha");
        if (tocarTrilha && fonteTrilha.clip != null) fonteTrilha.Play();
    }

    static AudioClip Carregar(string nome) => Resources.Load<AudioClip>("Audio/" + nome);

    void Tocar(AudioClip clip, float volume = 1f)
    {
        if (clip != null) fonteEfeitos.PlayOneShot(clip, volume);
    }

    public void Pulo() { Tocar(pulo, 0.5f); }
    public void Dano() { Tocar(dano); }
    public void Passagem() { Tocar(passagem, 0.8f); }

    public void Vitoria()
    {
        fonteTrilha.volume = volumeTrilha * 0.3f;
        Tocar(vitoria);
    }

    public void Derrota()
    {
        fonteTrilha.volume = volumeTrilha * 0.3f;
        Tocar(derrota);
    }
}
