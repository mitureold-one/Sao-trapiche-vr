using UnityEngine;
using UnityEditor;

public class FixEsferaAntes
{
    [MenuItem("Trapiche/Fix EsferaAntes Mesh")]
    static void Fix()
    {
        var go = GameObject.Find("EsferaAntes");
        if (go == null) { Debug.LogError("EsferaAntes nao encontrada"); return; }

        var mf = go.GetComponent<MeshFilter>();
        if (mf == null) mf = go.AddComponent<MeshFilter>();

        // Usa o mesh de esfera primitiva do Unity
        var tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mf.sharedMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
        GameObject.DestroyImmediate(tempSphere);

        var mr = go.GetComponent<MeshRenderer>();
        if (mr == null) mr = go.AddComponent<MeshRenderer>();

        var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Trapiche/Materials/Esfera_Antes.mat");
        if (mat != null) mr.sharedMaterial = mat;

        go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        EditorUtility.SetDirty(go);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("EsferaAntes mesh corrigida!");
    }
}