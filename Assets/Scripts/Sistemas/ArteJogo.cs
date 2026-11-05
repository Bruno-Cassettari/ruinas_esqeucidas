using System.Collections.Generic;
using UnityEngine;

// Carrega a arte de Assets/Resources/Arte:
//  - Cenário: "Dark Dungeon - Side-Scroller Tileset" (Raou), tiles de 16x16 px.
//  - Personagem: "Hooded Protagonist" (Penzilla Design), quadros de 32x32 px.
// Se a pasta não existir, o jogo continua com os sprites provisórios (GeradorSprites).
public static class ArteJogo
{
    const int TamanhoTile = 16;      // 16 px = 1 unidade do Unity
    const int TamanhoQuadro = 32;    // quadro do personagem
    const float PpuHeroi = 20f;      // personagem (~26 px de altura) fica com ~1,3 unidade

    static Texture2D tileset, folhaHeroi, tocha;
    static readonly Dictionary<Vector2Int, Sprite> tiles = new Dictionary<Vector2Int, Sprite>();
    static readonly Dictionary<RectInt, Sprite> recortes = new Dictionary<RectInt, Sprite>();
    static Sprite[] heroiParado, heroiCorrendo, heroiSubindo, heroiCaindo, tochaQuadros;
    static Sprite heroiDano;

    static Texture2D Tileset => tileset != null ? tileset : (tileset = Resources.Load<Texture2D>("Arte/Cenario/Tilesetv3"));
    static Texture2D FolhaHeroi => folhaHeroi != null ? folhaHeroi : (folhaHeroi = Resources.Load<Texture2D>("Arte/Heroi/AnimationSheet_Character"));

    public static bool Disponivel => Tileset != null;
    public static bool HeroiDisponivel => FolhaHeroi != null;

    // ---------- Cenário ----------

    // Tile pela coluna/linha do tileset, contando de cima para baixo (como no editor de imagem).
    public static Sprite Tile(int coluna, int linha)
    {
        var chave = new Vector2Int(coluna, linha);
        if (tiles.TryGetValue(chave, out var s)) return s;
        var tex = Tileset;
        if (tex == null) return null;
        var rect = new Rect(coluna * TamanhoTile, tex.height - (linha + 1) * TamanhoTile, TamanhoTile, TamanhoTile);
        s = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), TamanhoTile, 0, SpriteMeshType.FullRect);
        tiles[chave] = s;
        return s;
    }

    // Recorte de vários tiles (colunas c0..c1-1, linhas l0..l1-1), com pivô na base.
    public static Sprite Recorte(int c0, int l0, int c1, int l1)
    {
        var chave = new RectInt(c0, l0, c1 - c0, l1 - l0);
        if (recortes.TryGetValue(chave, out var s)) return s;
        var tex = Tileset;
        if (tex == null) return null;
        var rect = new Rect(c0 * TamanhoTile, tex.height - l1 * TamanhoTile, (c1 - c0) * TamanhoTile, (l1 - l0) * TamanhoTile);
        s = Sprite.Create(tex, rect, new Vector2(0.5f, 0f), TamanhoTile, 0, SpriteMeshType.FullRect);
        recortes[chave] = s;
        return s;
    }

    // Tocha animada (spr_torch.png: 3 quadros de 28x27 px).
    public static Sprite[] Tocha
    {
        get
        {
            if (tochaQuadros != null) return tochaQuadros;
            if (tocha == null) tocha = Resources.Load<Texture2D>("Arte/Cenario/spr_torch");
            if (tocha == null) return tochaQuadros = new Sprite[0];
            int n = 3, w = tocha.width / n;
            tochaQuadros = new Sprite[n];
            for (int i = 0; i < n; i++)
                tochaQuadros[i] = Sprite.Create(tocha, new Rect(i * w, 0, w, tocha.height), new Vector2(0.5f, 0.5f), TamanhoTile, 0, SpriteMeshType.FullRect);
            return tochaQuadros;
        }
    }

    // ---------- Personagem ----------
    // Linhas da folha: 0 parado, 3 correndo, 5 pulando, 7 morte/dano.

    public static Sprite[] HeroiParado => heroiParado ??= Quadros(0, 0, 1);
    public static Sprite[] HeroiCorrendo => heroiCorrendo ??= Quadros(3, 0, 7);
    public static Sprite[] HeroiSubindo => heroiSubindo ??= Quadros(5, 2, 3);
    public static Sprite[] HeroiCaindo => heroiCaindo ??= Quadros(5, 5, 6);
    public static Sprite HeroiDano => heroiDano != null ? heroiDano : (heroiDano = Quadro(7, 1));

    static Sprite[] Quadros(int linha, int deColuna, int ateColuna)
    {
        var lista = new List<Sprite>();
        for (int c = deColuna; c <= ateColuna; c++)
        {
            var s = Quadro(linha, c);
            if (s != null) lista.Add(s);
        }
        return lista.ToArray();
    }

    static Sprite Quadro(int linha, int coluna)
    {
        var tex = FolhaHeroi;
        if (tex == null) return null;
        var rect = new Rect(coluna * TamanhoQuadro, tex.height - (linha + 1) * TamanhoQuadro, TamanhoQuadro, TamanhoQuadro);
        return Sprite.Create(tex, rect, new Vector2(0.5f, 0f), PpuHeroi, 0, SpriteMeshType.FullRect);
    }
}
