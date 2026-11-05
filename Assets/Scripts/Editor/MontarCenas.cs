using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Menu do editor: "Ruínas Esquecidas > Montar cenas do jogo".
// Cria Assets/Scenes/Menu.unity e Assets/Scenes/Jogo.unity e coloca as duas no Build Profile.
[InitializeOnLoad]
public static class MontarCenas
{
    // Na primeira vez que o projeto abre, as cenas são montadas sozinhas.
    static MontarCenas()
    {
        EditorApplication.delayCall += () =>
        {
            if (!File.Exists(CaminhoMenu) && !EditorApplication.isPlayingOrWillChangePlaymode)
                Montar();
        };
    }

    const string Pasta = "Assets/Scenes";
    const string CaminhoMenu = Pasta + "/Menu.unity";
    const string CaminhoJogo = Pasta + "/Jogo.unity";

    [MenuItem("Ruínas Esquecidas/Montar cenas do jogo")]
    public static void Montar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Directory.CreateDirectory(Pasta);

        var menu = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CriarCamera();
        new GameObject("MenuPrincipal").AddComponent<MenuPrincipal>();
        EditorSceneManager.SaveScene(menu, CaminhoMenu);

        var jogo = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CriarCamera();
        new GameObject("GameManager").AddComponent<GameManager>();
        EditorSceneManager.SaveScene(jogo, CaminhoJogo);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(CaminhoMenu, true),
            new EditorBuildSettingsScene(CaminhoJogo, true)
        };

        EditorSceneManager.OpenScene(CaminhoMenu);
        EditorUtility.DisplayDialog("Ruínas Esquecidas",
            "Cenas criadas em Assets/Scenes e adicionadas ao build.\n\nAperte Play na cena Menu para jogar.", "OK");
    }

    static void CriarCamera()
    {
        var go = new GameObject("Main Camera") { tag = "MainCamera" };
        go.transform.position = new Vector3(0f, 0f, -10f);
        var cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Paleta.Fundo;
        go.AddComponent<AudioListener>();
    }
}
