using UnityEngine;

// Câmera que segue o jogador com suavização, olhando um pouco à frente,
// sem mostrar fora dos limites da fase.
[RequireComponent(typeof(Camera))]
public class CameraSeguir : MonoBehaviour
{
    public Transform alvo;
    public float suavizacao = 0.15f;
    public float olharAFrente = 1.5f;
    public Vector2 deslocamento = new Vector2(0f, 1.5f);

    Rect limites;
    bool temLimites;
    Vector3 velocidade;
    Camera cam;

    void Awake() { cam = GetComponent<Camera>(); }

    public void DefinirLimites(Rect r) { limites = r; temLimites = true; }

    public void PosicionarImediato()
    {
        if (alvo == null) return;
        transform.position = Limitar(Destino());
    }

    void LateUpdate()
    {
        if (alvo == null) return;
        Vector3 destino = Limitar(Destino());
        transform.position = Vector3.SmoothDamp(transform.position, destino, ref velocidade, suavizacao);
    }

    Vector3 Destino()
    {
        float frente = alvo.localScale.x >= 0 ? olharAFrente : -olharAFrente;
        return new Vector3(alvo.position.x + frente + deslocamento.x, alvo.position.y + deslocamento.y, -10f);
    }

    Vector3 Limitar(Vector3 p)
    {
        if (!temLimites) return p;
        float meiaAltura = cam.orthographicSize;
        float meiaLargura = meiaAltura * cam.aspect;

        p.x = limites.width > meiaLargura * 2f
            ? Mathf.Clamp(p.x, limites.xMin + meiaLargura, limites.xMax - meiaLargura)
            : limites.center.x;
        p.y = limites.height > meiaAltura * 2f
            ? Mathf.Clamp(p.y, limites.yMin + meiaAltura, limites.yMax - meiaAltura)
            : Mathf.Max(p.y, limites.yMin + meiaAltura);
        return p;
    }
}
