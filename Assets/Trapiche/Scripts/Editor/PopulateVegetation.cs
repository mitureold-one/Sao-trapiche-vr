using UnityEngine;
using UnityEditor;

public class PopulateVegetation
{
    [MenuItem("Trapiche/Populate Vegetation")]
    static void Populate()
    {
        string basePath = "Assets/TerrainDemoScene_URP/Prefabs/Details/";
        
        var parent = GameObject.Find("GramaAntes");
        if (parent == null) { Debug.LogError("GramaAntes nao encontrado"); return; }

        var data = new (string prefab, float x, float z, float scale, float rotY)[]
        {
            ("Grass_C", -17.5f, 5.3f, 2.7f, 151.9f),
            ("Grass_C", -20.7f, -18.2f, 4.0f, 202.0f),
            ("Grass_A", 9.5f, 3.6f, 3.1f, 212.1f),
            ("Shrub", 13.6f, -27.7f, 2.9f, 152.1f),
            ("Shrub", -9.8f, -18.3f, 3.3f, 33.4f),
            ("Grass_D", -17.7f, 10.1f, 3.2f, 15.6f),
            ("Grass_A", -1.8f, -22.4f, 2.7f, 105.5f),
            ("Fern_C", 5.7f, 11.8f, 3.9f, 253.6f),
            ("Fern_B", -20.0f, -17.7f, 5.0f, 307.9f),
            ("Grass_B", 16.1f, -10.9f, 4.1f, 131.3f),
            ("Fern_B", -5.7f, -18.6f, 4.3f, 246.1f),
            ("Grass_C", -18.9f, 0.6f, 4.3f, 58.8f),
            ("Heather_A", -5.3f, 16.5f, 4.2f, 79.1f),
            ("Bush_A", -7.7f, 6.6f, 3.1f, 11.6f),
            ("Fern_A", -8.1f, -16.0f, 4.8f, 204.2f),
            ("Grass_B", 9.6f, -18.4f, 3.5f, 329.2f),
            ("Fern_A", -1.8f, -16.1f, 4.4f, 194.0f),
            ("Grass_D", 10.9f, -8.7f, 3.5f, 79.0f),
            ("Bush_B", 21.9f, -5.1f, 4.4f, 310.0f),
            ("Heather_A", -15.3f, -20.8f, 3.6f, 22.9f),
            ("Grass_C", -5.2f, 16.8f, 3.1f, 199.2f),
            ("Bush_B", 19.5f, 2.6f, 4.2f, 193.3f),
            ("Bush_B", -10.3f, 0.8f, 3.2f, 56.9f),
            ("Heather_B", -21.9f, 4.5f, 3.2f, 180.2f),
            ("Heather_A", -14.1f, 13.1f, 3.2f, 230.0f),
            ("Shrub", 4.8f, -21.1f, 2.9f, 343.3f),
            ("Grass_D", 18.6f, 13.3f, 3.3f, 7.0f),
            ("Shrub", 18.9f, 11.5f, 3.3f, 20.9f),
            ("Bush_B", 16.6f, 14.6f, 4.3f, 293.8f),
            ("Bush_Red", 21.0f, -4.0f, 2.8f, 171.1f),
            ("Grass_D", 2.2f, -16.1f, 3.6f, 76.2f),
            ("Fern_A", 1.7f, 4.8f, 4.3f, 143.6f),
            ("Grass_C", 7.6f, -11.2f, 3.6f, 89.2f),
            ("Grass_C", -19.2f, -27.1f, 3.1f, 79.3f),
            ("Fern_A", -18.9f, 0.4f, 2.7f, 11.3f),
            ("Fern_B", -7.5f, -4.9f, 4.2f, 77.1f),
            ("Grass_D", -16.2f, 14.1f, 3.9f, 87.5f),
            ("Bush_B", -1.2f, -9.7f, 2.7f, 155.2f),
            ("Heather_B", -3.4f, -7.0f, 2.6f, 235.2f),
            ("Bush_A", -12.0f, -3.5f, 3.8f, 45.0f),
        };

        int count = 0;
        foreach (var d in data)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(basePath + d.prefab + ".prefab");
            if (prefab == null) { Debug.LogWarning("Prefab nao encontrado: " + d.prefab); continue; }
            
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.SetParent(parent.transform);
            go.transform.position = new Vector3(d.x, 0, d.z);
            go.transform.localEulerAngles = new Vector3(0, d.rotY, 0);
            go.transform.localScale = Vector3.one * d.scale;
            count++;
        }

        Debug.Log($"Adicionados {count} objetos de vegetacao em GramaAntes!");
    }
}
