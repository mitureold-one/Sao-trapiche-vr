using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FixBugsCriticos : MonoBehaviour
{
    [MenuItem("Trapiche/QA - Fix PortalLobby (cena ativa)")]
    public static void CorrigirCenaAtiva()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        int fixes = 0;

        foreach (var root in scene.GetRootGameObjects())
        {
            var portal = FindInChildren(root, "PortalLobby");
            if (portal == null) continue;

            // 1. Remover PortalLoader
            var pl = portal.GetComponent<PortalLoader>();
            if (pl != null)
            {
                Undo.DestroyObjectImmediate(pl);
                Debug.Log("[Fix] PortalLoader removido de PortalLobby");
                fixes++;
            }

            // 2. SphereCollider nao deve ser trigger
            var sphere = portal.GetComponent<SphereCollider>();
            if (sphere != null && sphere.isTrigger)
            {
                Undo.RecordObject(sphere, "Fix isTrigger");
                sphere.isTrigger = false;
                Debug.Log("[Fix] SphereCollider.isTrigger = false");
                fixes++;
            }

            // 3. Adicionar SceneLoader(Lobby) se nao tiver
            var sl = portal.GetComponent<SceneLoader>();
            if (sl == null)
            {
                sl = Undo.AddComponent<SceneLoader>(portal);
                sl.nomeCena    = "Lobby";
                sl.duracaoFade = 1.2f;
                sl.corFade     = Color.black;
                Debug.Log("[Fix] SceneLoader(Lobby) adicionado ao PortalLobby");
                fixes++;
            }
            else if (sl.nomeCena != "Lobby")
            {
                Undo.RecordObject(sl, "Fix nomeCena");
                sl.nomeCena = "Lobby";
                Debug.Log("[Fix] SceneLoader.nomeCena corrigido para Lobby");
                fixes++;
            }
        }

        if (fixes > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Fix] {fixes} correcoes salvas em {scene.name}");
            EditorUtility.DisplayDialog("Fix OK", $"{fixes} correcoes em {scene.name}", "OK");
        }
        else
        {
            Debug.Log($"[Info] Nada a corrigir em {scene.name}");
            EditorUtility.DisplayDialog("Fix", $"Nada a corrigir em {scene.name}", "OK");
        }
    }

    static GameObject FindInChildren(GameObject root, string name)
    {
        if (root.name == name) return root;
        foreach (Transform child in root.transform)
        {
            var found = FindInChildren(child.gameObject, name);
            if (found != null) return found;
        }
        return null;
    }
}