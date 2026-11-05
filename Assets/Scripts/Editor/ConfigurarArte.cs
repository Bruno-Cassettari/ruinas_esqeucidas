using UnityEditor;
using UnityEngine;

// Configura as imagens de Assets/Resources/Arte para pixel art:
// tipo Sprite, filtro Point (sem borrar), sem compressão e sem mipmaps.
// Também corrige imagens que já tinham sido importadas antes deste script existir.
[InitializeOnLoad]
public class ConfigurarArte : AssetPostprocessor
{
    const string Pasta = "Assets/Resources/Arte";

    static ConfigurarArte()
    {
        EditorApplication.delayCall += CorrigirExistentes;
    }

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(Pasta)) return;
        Aplicar((TextureImporter)assetImporter);
    }

    static void Aplicar(TextureImporter imp)
    {
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.filterMode = FilterMode.Point;
        imp.mipmapEnabled = false;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.alphaIsTransparency = true;
        imp.wrapMode = TextureWrapMode.Clamp;
        imp.npotScale = TextureImporterNPOTScale.None;
    }

    static void CorrigirExistentes()
    {
        foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Pasta }))
        {
            string caminho = AssetDatabase.GUIDToAssetPath(guid);
            var imp = AssetImporter.GetAtPath(caminho) as TextureImporter;
            if (imp == null) continue;
            if (imp.textureType == TextureImporterType.Sprite && imp.filterMode == FilterMode.Point &&
                imp.textureCompression == TextureImporterCompression.Uncompressed) continue;
            Aplicar(imp);
            imp.SaveAndReimport();
        }
    }
}
