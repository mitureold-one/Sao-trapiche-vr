using UnityEditor;

public class BuildConfig
{
    [MenuItem("Trapiche/Configurar Build Settings")]
    public static void Configurar()
    {
        var cenas = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/Lobby.unity",   true),
            new EditorBuildSettingsScene("Assets/Scenes/Trapiche.unity", true),
        };
        EditorBuildSettings.scenes = cenas;
        UnityEngine.Debug.Log("Build Settings: Lobby(0) + Trapiche(1). SampleScene removida.");
    }
}
