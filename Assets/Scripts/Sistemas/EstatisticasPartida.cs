using UnityEngine;

// Tempo e mortes da partida atual (da fase 1 até o artefato) e o recorde de tempo salvo.
// Estático para sobreviver ao recarregamento da cena entre as fases.
public static class EstatisticasPartida
{
    const string ChaveRecorde = "RuinasEsquecidas.RecordeTempo";

    public static float Tempo { get; private set; }
    public static int Mortes { get; private set; }

    public static void Reiniciar()
    {
        Tempo = 0f;
        Mortes = 0;
    }

    public static void Somar(float segundos) => Tempo += segundos;

    public static void RegistrarMorte() => Mortes++;

    // Recorde em segundos; 0 quando ainda não existe.
    public static float Recorde => PlayerPrefs.GetFloat(ChaveRecorde, 0f);

    // Chamado na vitória: salva o tempo se for o melhor. Devolve true quando é um novo recorde.
    public static bool Concluir()
    {
        float recorde = Recorde;
        if (recorde > 0f && Tempo >= recorde) return false;
        PlayerPrefs.SetFloat(ChaveRecorde, Tempo);
        PlayerPrefs.Save();
        return true;
    }

    public static string Formatar(float segundos)
    {
        int total = Mathf.FloorToInt(segundos);
        return $"{total / 60}:{total % 60:00}";
    }

    public static string TextoMortes()
    {
        if (Mortes == 0) return "nenhuma morte";
        return Mortes == 1 ? "1 morte" : $"{Mortes} mortes";
    }
}
