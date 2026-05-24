using UnityEngine;
using UnityEditor;

public class SetupTrapicheDepois
{
    [MenuItem("Trapiche/Setup TrapicheDepois")]
    static void Setup()
    {
        // Chao concreto
        var chao = GameObject.Find("Chão");
        if (chao != null)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/YughuesFreeConcreteMaterials/Materials/M_YFCM_PrecastD4.mat");
            if (mat != null)
            {
                chao.GetComponent<Renderer>().sharedMaterial = mat;
                Debug.Log("Chao: concreto aplicado");
            }
        }

        // Remove EsferaAntesDepois (nao precisa mais)
        var esfera = GameObject.Find("EsferaAntesDepois");
        if (esfera != null) { GameObject.DestroyImmediate(esfera); Debug.Log("EsferaAntesDepois removida"); }

        EditorUtility.SetDirty(GameObject.Find("Ambiente"));
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("TrapicheDepois configurado!");
    }
}