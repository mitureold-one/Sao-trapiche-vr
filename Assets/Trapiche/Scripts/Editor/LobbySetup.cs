using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class LobbySetup
{
    [MenuItem("Trapiche/Configurar Lobby (SceneLoader)")]
    public static void Configurar()
    {
        // Garante que estamos na cena Lobby
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            Debug.LogWarning("Abra a cena Lobby antes de executar este menu.");
            return;
        }

        // BotaoEntrar — adiciona SceneLoader
        var botao = GameObject.Find("BotaoEntrar");
        if (botao != null)
        {
            var loader = botao.GetComponent<SceneLoader>();
            if (loader == null) loader = botao.AddComponent<SceneLoader>();
            loader.nomeCena    = "Trapiche";
            loader.duracaoFade = 1.2f;
            EditorUtility.SetDirty(botao);
            Debug.Log("SceneLoader adicionado ao BotaoEntrar -> Trapiche");
        }
        else Debug.LogWarning("BotaoEntrar nao encontrado!");

        // FadeScreen — cria como GameObject persistente na cena
        var fadeExistente = GameObject.Find("FadeScreen");
        if (fadeExistente == null)
        {
            var fadeGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
            fadeGO.name = "FadeScreen";
            Object.DestroyImmediate(fadeGO.GetComponent<MeshCollider>());

            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            var mat    = new Material(shader);
            mat.SetFloat("_Surface", 1f);
            mat.renderQueue = 3000;
            fadeGO.GetComponent<MeshRenderer>().sharedMaterial = mat;
            fadeGO.AddComponent<FadeScreen>();

            EditorUtility.SetDirty(fadeGO);
            Debug.Log("FadeScreen criado na cena Lobby");
        }
        else Debug.Log("FadeScreen ja existe.");

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Lobby configurado!");
    }
}
