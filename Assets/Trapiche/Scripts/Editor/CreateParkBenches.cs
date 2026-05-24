using UnityEngine;
using UnityEditor;

public class CreateParkBenches
{
    [MenuItem("Trapiche/Create Park Benches")]
    static void Create()
    {
        var urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null) { Debug.LogError("URP Lit nao encontrado"); return; }

        // Material concreto
        var matConcrete = new Material(urpLit);
        matConcrete.name = "Concrete_URP";
        matConcrete.SetColor("_BaseColor", new Color(0.72f, 0.72f, 0.70f));
        matConcrete.SetFloat("_Smoothness", 0.1f);
        matConcrete.SetFloat("_Metallic", 0f);
        AssetDatabase.CreateAsset(matConcrete, "Assets/Trapiche/Materials/Concrete_URP.mat");

        // Material madeira
        var matWood = new Material(urpLit);
        matWood.name = "Wood_URP";
        matWood.SetColor("_BaseColor", new Color(0.55f, 0.27f, 0.07f));
        matWood.SetFloat("_Smoothness", 0.25f);
        matWood.SetFloat("_Metallic", 0f);
        AssetDatabase.CreateAsset(matWood, "Assets/Trapiche/Materials/Wood_URP.mat");

        // Material terra/grama do centro
        var matSoil = new Material(urpLit);
        matSoil.name = "Soil_URP";
        matSoil.SetColor("_BaseColor", new Color(0.25f, 0.18f, 0.10f));
        matSoil.SetFloat("_Smoothness", 0.0f);
        AssetDatabase.CreateAsset(matSoil, "Assets/Trapiche/Materials/Soil_URP.mat");

        AssetDatabase.SaveAssets();

        // Grupo pai
        var depois = GameObject.Find("GramaDepois") ?? new GameObject("GramaDepois");
        var benchGroup = new GameObject("BancosDepois");
        benchGroup.transform.SetParent(depois.transform);

        // Posicoes dos 3 bancos
        Vector3[] positions = {
            new Vector3(-8f, 0f, -5f),
            new Vector3(0f,  0f, -8f),
            new Vector3(8f,  0f, -5f),
        };

        string treePath = "Assets/TerrainDemoScene_URP/Prefabs/Trees/Pines/Pine_A/Pine_A.prefab";
        var treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(treePath);

        foreach (var pos in positions)
        {
            var bench = new GameObject("BancoPraca");
            bench.transform.SetParent(benchGroup.transform);
            bench.transform.position = pos;

            float size = 2.8f;   // lado do quadrado
            float legH = 0.45f;  // altura do bloco de concreto
            float legW = 0.35f;  // largura do bloco
            float slatH = 0.07f; // altura da ripas de madeira
            float slatW = 0.12f; // largura de cada ripa
            float seatY = legH + slatH / 2f;

            // 4 blocos de concreto nos cantos
            Vector3[] corners = {
                new Vector3( size/2f, legH/2f,  size/2f),
                new Vector3(-size/2f, legH/2f,  size/2f),
                new Vector3( size/2f, legH/2f, -size/2f),
                new Vector3(-size/2f, legH/2f, -size/2f),
            };
            foreach (var c in corners)
            {
                var leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.name = "Concrete_Leg";
                leg.transform.SetParent(bench.transform);
                leg.transform.localPosition = c;
                leg.transform.localScale = new Vector3(legW, legH, legW);
                leg.GetComponent<Renderer>().sharedMaterial = matConcrete;
            }

            // Ripas de madeira — 4 lados do quadrado
            int slats = 5;
            float spacing = size / (slats + 1f);

            // Lado frente e fundo (ao longo de X)
            for (int i = 1; i <= slats; i++)
            {
                float xPos = -size/2f + i * spacing;
                foreach (float zSide in new[] { size/2f - slatW/2f, -size/2f + slatW/2f })
                {
                    var slat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    slat.name = "Wood_Slat";
                    slat.transform.SetParent(bench.transform);
                    slat.transform.localPosition = new Vector3(xPos, seatY, zSide);
                    slat.transform.localScale = new Vector3(slatW, slatH, slatW * 1.2f);
                    slat.GetComponent<Renderer>().sharedMaterial = matWood;
                }
            }

            // Lado esquerdo e direito (ao longo de Z)
            for (int i = 1; i <= slats; i++)
            {
                float zPos = -size/2f + i * spacing;
                foreach (float xSide in new[] { size/2f - slatW/2f, -size/2f + slatW/2f })
                {
                    var slat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    slat.name = "Wood_Slat";
                    slat.transform.SetParent(bench.transform);
                    slat.transform.localPosition = new Vector3(xSide, seatY, zPos);
                    slat.transform.localScale = new Vector3(slatW * 1.2f, slatH, slatW);
                    slat.GetComponent<Renderer>().sharedMaterial = matWood;
                }
            }

            // Terra no centro
            var soil = GameObject.CreatePrimitive(PrimitiveType.Cube);
            soil.name = "Soil_Center";
            soil.transform.SetParent(bench.transform);
            soil.transform.localPosition = new Vector3(0, 0.02f, 0);
            soil.transform.localScale = new Vector3(size - legW * 2f, 0.05f, size - legW * 2f);
            soil.GetComponent<Renderer>().sharedMaterial = matSoil;

            // Arvore no centro
            if (treePrefab != null)
            {
                var tree = (GameObject)PrefabUtility.InstantiatePrefab(treePrefab);
                tree.transform.SetParent(bench.transform);
                tree.transform.localPosition = new Vector3(0, 0, 0);
                tree.transform.localScale = Vector3.one * 0.6f;
            }
        }

        Debug.Log("3 bancos de praca criados em GramaDepois/BancosDepois!");
    }
}
