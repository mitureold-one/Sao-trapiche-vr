using UnityEngine;
using UnityEditor;

public class ReimportArvores
{
    [MenuItem("Trapiche/Reimportar Materiais das Arvores")]
    public static void Reimportar()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Procedural Tree" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log("Reimportado: " + path);
        }
        AssetDatabase.Refresh();
        Debug.Log("Arvores reimportadas!");
    }
}
