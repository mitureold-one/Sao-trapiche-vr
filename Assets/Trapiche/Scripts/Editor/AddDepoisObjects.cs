using UnityEngine;
using UnityEditor;

public class AddDepoisObjects
{
    [MenuItem("Trapiche/Adicionar Objetos no DEPOIS")]
    static void Add()
    {
        var esfera = GameObject.Find("EsferaAntesDepois");
        var ctrl = esfera.GetComponent<AntesDepoisController>();

        ctrl.objetosApenaDepois.Clear();

        var gramaDepois = GameObject.Find("GramaDepois");
        if (gramaDepois != null) { ctrl.objetosApenaDepois.Add(gramaDepois); Debug.Log("GramaDepois adicionado"); }

        var telhado = GameObject.Find("Predio1Telhado");
        if (telhado != null) { ctrl.objetosApenaDepois.Add(telhado); Debug.Log("Predio1Telhado adicionado"); }

        EditorUtility.SetDirty(esfera);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"objetosApenaDepois: {ctrl.objetosApenaDepois.Count} objetos");
    }
}