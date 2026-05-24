using UnityEngine;
using UnityEditor;

public class CleanupScene
{
    [MenuItem("Trapiche/Cleanup Duplicatas")]
    static void Cleanup()
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        
        // Remove duplicatas de GramaDepois, GramaAntes, Pine_A soltos
        var gramaDepoisList = new System.Collections.Generic.List<GameObject>();
        var gramaAntesList  = new System.Collections.Generic.List<GameObject>();

        foreach (var go in all)
        {
            if (go.scene.name != "Trapiche") continue;
            if (go.name == "GramaDepois") gramaDepoisList.Add(go);
            if (go.name == "GramaAntes")  gramaAntesList.Add(go);
        }

        // Mantém só o que tem filhos, deleta os vazios
        foreach (var list in new[] { gramaDepoisList, gramaAntesList })
        {
            var withChildren    = list.FindAll(g => g.transform.childCount > 0);
            var withoutChildren = list.FindAll(g => g.transform.childCount == 0);

            // Se tem um com filhos, deleta os sem filhos
            if (withChildren.Count > 0)
                foreach (var g in withoutChildren) { Debug.Log("Deletando vazio: " + g.name); GameObject.DestroyImmediate(g); }
            // Se todos vazios, mantém só o primeiro
            else if (withoutChildren.Count > 1)
                for (int i = 1; i < withoutChildren.Count; i++) { Debug.Log("Deletando extra: " + withoutChildren[i].name); GameObject.DestroyImmediate(withoutChildren[i]); }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Cleanup concluído!");
    }
}