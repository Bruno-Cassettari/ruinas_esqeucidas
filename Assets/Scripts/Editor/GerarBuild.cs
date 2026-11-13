using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Menu do editor: "Ruínas Esquecidas > Gerar build WebGL (itch.io)" e "Gerar build Windows".
// As builds vão para a pasta Builds/ (fora do Git, pelo .gitignore).
public static class GerarBuild
{
    [MenuItem("Ruínas Esquecidas/Gerar build WebGL (itch.io)")]
    public static void WebGL()
    {
        Configurar();
        // O itch.io não envia os cabeçalhos de compressão; o fallback deixa o navegador descompactar sozinho.
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.decompressionFallback = true;
        Gerar(BuildTarget.WebGL, "Builds/WebGL");
    }

    [MenuItem("Ruínas Esquecidas/Gerar build Windows")]
    public static void Windows()
    {
        Configurar();
        Gerar(BuildTarget.StandaloneWindows64, "Builds/Windows/RuinasEsquecidas.exe");
    }

    static void Configurar()
    {
        PlayerSettings.productName = "Ruínas Esquecidas";
        PlayerSettings.companyName = "Bruno e João Pedro - UNITAU";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.defaultWebScreenWidth = 1280;
        PlayerSettings.defaultWebScreenHeight = 720;
        PlayerSettings.runInBackground = true;
    }

    static void Gerar(BuildTarget alvo, string caminho)
    {
        if (EditorBuildSettings.scenes.Length == 0) MontarCenas.Montar();
        var cenas = EditorBuildSettings.scenes.Where(c => c.enabled).Select(c => c.path).ToArray();

        var relatorio = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = cenas,
            locationPathName = caminho,
            target = alvo,
            options = BuildOptions.None
        });

        var resumo = relatorio.summary;
        if (resumo.result == BuildResult.Succeeded)
        {
            Debug.Log($"[Ruínas] Build {alvo} pronta em {caminho} ({resumo.totalSize / (1024 * 1024)} MB).");
            EditorUtility.RevealInFinder(caminho);
            EditorUtility.DisplayDialog("Ruínas Esquecidas", $"Build {alvo} gerada em:\n{caminho}", "OK");
        }
        else
        {
            Debug.LogError($"[Ruínas] Build {alvo} falhou: {resumo.result}, {resumo.totalErrors} erro(s).");
            EditorUtility.DisplayDialog("Ruínas Esquecidas",
                $"A build falhou ({resumo.result}). Veja a aba Console.\n\nPara WebGL, confirme que o módulo \"WebGL Build Support\" está instalado no Unity Hub.", "OK");
        }
    }
}
