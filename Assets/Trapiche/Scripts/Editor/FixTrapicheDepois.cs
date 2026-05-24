using UnityEngine;
using UnityEditor;

public class FixTrapicheDepois
{
    [MenuItem("Trapiche/Fix Bancos TrapicheDepois")]
    static void Fix()
    {
        var gramaDepois = GameObject.Find("GramaDepois");

        // Encontra todos os bancos na cena incluindo inativos
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        int count = 0;
        foreach (var go in allObjects)
        {
            if (go.scene.name != "TrapicheDepois") continue;
            if (go.name.StartsWith("Banco") || go.name.StartsWith("Pine_A"))
            {
                go.SetActive(true);
                if (gramaDepois != null) go.transform.SetParent(gramaDepois.transform, true);
                Debug.Log("Ativado e movido: " + go.name);
                count++;
            }
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"Total: {count} objetos corrigidos");
    }
}