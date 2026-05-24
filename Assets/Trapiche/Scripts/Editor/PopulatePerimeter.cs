using UnityEngine;
using UnityEditor;

public class PopulatePerimeter
{
    [MenuItem("Trapiche/Populate Perimeter")]
    static void Populate()
    {
        var arvoresGroup = GameObject.Find("Arvores");
        var rochasGroup = GameObject.Find("Rochas");
        if (arvoresGroup == null || rochasGroup == null) { Debug.LogError("Grupos nao encontrados"); return; }

        string p = "Assets/TerrainDemoScene_URP/Prefabs/Trees/";
        string rp = "Assets/TerrainDemoScene_URP/Prefabs/Rocks/";
        string[] pines = {
            p+"Pines/Pine_A/Pine_A.prefab", p+"Pines/Pine_B/Pine_B.prefab",
            p+"Pines/Pine_C/Pine_C.prefab", p+"Pines/Pine_D/Pine_D.prefab",
            p+"Conifer/Conifer.prefab", p+"Cypress/Cypress.prefab"
        };

        float[,] trees = {
            {-20,30,15,1.2f,0}, {-8,32,200,1.4f,1}, {5,30,80,1.1f,2}, {18,31,310,1.3f,4},
            {-32,28,45,1.2f,5}, {30,28,120,1.0f,3},
            {-22,-42,170,1.3f,1}, {-8,-44,260,1.5f,4}, {6,-42,90,1.2f,0}, {20,-43,330,1.1f,2},
            {-32,-38,55,1.0f,3}, {30,-40,200,1.3f,5},
            {-35,15,10,1.2f,2}, {-36,2,190,1.4f,0}, {-34,-12,270,1.1f,4}, {-37,-25,80,1.3f,1},
            {35,15,140,1.2f,3}, {36,2,20,1.1f,5}, {34,-12,300,1.4f,2}, {37,-25,170,1.0f,0}
        };

        for (int i = 0; i < trees.GetLength(0); i++) {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pines[(int)trees[i,4]]);
            if (prefab == null) continue;
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.SetParent(arvoresGroup.transform);
            go.transform.position = new Vector3(trees[i,0], 0, trees[i,1]);
            go.transform.eulerAngles = new Vector3(0, trees[i,2], 0);
            go.transform.localScale = Vector3.one * trees[i,3];
        }

        float[,] rocks = {
            {-38,-3,-45,20,10,7,10,0}, {38,-3,-45,200,11,8,11,0},
            {0,-3,-52,90,14,9,14,1}, {-38,-2,28,45,8,6,8,2}, {38,-2,28,135,9,6,9,2}
        };
        string[] rockPrefabs = {rp+"Rock_D.prefab", rp+"Rock_Overgrown_D.prefab", rp+"Rock_Overgrown_A.prefab"};

        for (int i = 0; i < rocks.GetLength(0); i++) {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(rockPrefabs[(int)rocks[i,7]]);
            if (prefab == null) { Debug.LogWarning("Nao encontrado: " + rockPrefabs[(int)rocks[i,7]]); continue; }
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.SetParent(rochasGroup.transform);
            go.transform.position = new Vector3(rocks[i,0], rocks[i,1], rocks[i,2]);
            go.transform.eulerAngles = new Vector3(0, rocks[i,3], 0);
            go.transform.localScale = new Vector3(rocks[i,4], rocks[i,5], rocks[i,6]);
        }

        Debug.Log("Perimetro populado com " + trees.GetLength(0) + " arvores e " + rocks.GetLength(0) + " rochas!");
    }
}