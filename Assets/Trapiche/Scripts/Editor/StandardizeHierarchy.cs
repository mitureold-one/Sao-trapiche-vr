using UnityEngine;
using UnityEditor;

public class StandardizeHierarchy
{
    [MenuItem("Trapiche/Padronizar Hierarquia")]
    static void Standardize()
    {
        // Bancos
        RenameObject("Banco (1)", "Banco_01");
        RenameObject("Banco (2)", "Banco_02");
        RenameObject("Banco (3)", "Banco_03");

        // Rochas
        RenameObject("Rock_06_A_LOD2 (1)", "Rocha_01");
        RenameObject("Rock_06_A_LOD2 (2)", "Rocha_02");

        // Portal
        RenameObject("ColunaEsq",   "Portal_ColunaEsq");
        RenameObject("ColunaDir",   "Portal_ColunaDir");
        RenameObject("Verga",       "Portal_Verga");
        RenameObject("PortalCenter","Portal_Centro");

        // Arvores nos bancos
        RenameChildrenOfBancos();

        // Cubos dos bancos
        RenameBancoCubes();

        // Planes desconhecidos
        CheckPlanes();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Hierarquia padronizada!");
    }

    static void RenameObject(string oldName, string newName)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go.scene.name == null || go.scene.name == "") continue;
            if (go.name == oldName)
            {
                go.name = newName;
                Debug.Log($"Renomeado: {oldName} -> {newName}");
                return;
            }
        }
        Debug.LogWarning($"Nao encontrado: {oldName}");
    }

    static void RenameChildrenOfBancos()
    {
        int idx = 1;
        foreach (var bName in new[] { "Banco_01", "Banco_02", "Banco_03" })
        {
            var banco = GameObject.Find(bName);
            if (banco == null) continue;
            foreach (Transform child in banco.transform)
            {
                if (child.name.StartsWith("Pine_A"))
                {
                    child.name = $"Arvore_Banco_{idx:D2}";
                    idx++;
                }
            }
        }
    }

    static void RenameBancoCubes()
    {
        foreach (var bName in new[] { "Banco_01", "Banco_02", "Banco_03" })
        {
            var banco = GameObject.Find(bName);
            if (banco == null) continue;
            var cubes = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in banco.transform)
                if (child.name.StartsWith("Cube")) cubes.Add(child);

            // Por posicao: cantos (Cube, Cube(1), Cube(5), Cube(6)) e ripas
            int canto = 1, ripa = 1;
            foreach (var c in cubes)
            {
                var s = c.localScale;
                bool isCanto = Mathf.Abs(s.x - s.z) < 0.1f && s.y > s.x * 0.5f;
                if (isCanto) c.name = $"Concreto_Canto_{canto++:D2}";
                else c.name = $"Madeira_Ripa_{ripa++:D2}";
            }
        }
    }

    static void CheckPlanes()
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        int idx = 1;
        foreach (var go in all)
        {
            if (go.scene.name == null || go.scene.name == "") continue;
            if (go.name == "Plane" || go.name.StartsWith("Plane ("))
            {
                go.name = $"Plano_{idx:D2}";
                Debug.Log($"Plane renomeado para Plano_{idx:D2} em: {GetPath(go)}");
                idx++;
            }
        }
    }

    static string GetPath(GameObject go)
    {
        string path = go.name;
        var t = go.transform.parent;
        while (t != null) { path = t.name + "/" + path; t = t.parent; }
        return path;
    }
}