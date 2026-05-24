using UnityEngine;
using UnityEditor;

public class RemoveController
{
    [MenuItem("Trapiche/Remover AntesDepoisController")]
    static void Remove()
    {
        var esfera = GameObject.Find("EsferaAntesDepois");
        if (esfera == null) { Debug.LogError("EsferaAntesDepois nao encontrada"); return; }

        var ctrl = esfera.GetComponent<AntesDepoisController>();
        if (ctrl != null)
        {
            GameObject.DestroyImmediate(ctrl);
            Debug.Log("AntesDepoisController removido!");
        }
        else Debug.Log("Nao tinha controller.");

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}