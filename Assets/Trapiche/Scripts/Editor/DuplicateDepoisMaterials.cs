using UnityEngine;
using UnityEditor;
using System.IO;

public class DuplicateDepoisMaterials
{
    [MenuItem("Trapiche/Duplicar Materiais para Estado DEPOIS")]
    static void Duplicate()
    {
        var esfera = GameObject.Find("EsferaAntesDepois");
        if (esfera == null) { Debug.LogError("EsferaAntesDepois nao encontrada"); return; }

        var ctrl = esfera.GetComponent<AntesDepoisController>();
        if (ctrl == null) { Debug.LogError("AntesDepoisController nao encontrado"); return; }

        string depoisFolder = "Assets/Trapiche/Materials/Depois";
        if (!AssetDatabase.IsValidFolder(depoisFolder))
            AssetDatabase.CreateFolder("Assets/Trapiche/Materials", "Depois");

        foreach (var swap in ctrl.swaps)
        {
            if (swap.renderer == null) continue;
            string goName = swap.renderer.gameObject.name;

            swap.matsDepois.Clear();

            foreach (var mat in swap.matsAntes)
            {
                if (mat == null) { swap.matsDepois.Add(null); continue; }

                // Cria copia do material
                string newPath = $"{depoisFolder}/{mat.name}_Depois.mat";
                
                // Se ja existe, usa o existente
                var existing = AssetDatabase.LoadAssetAtPath<Material>(newPath);
                if (existing != null)
                {
                    swap.matsDepois.Add(existing);
                    Debug.Log($"Ja existe: {newPath}");
                    continue;
                }

                // Duplica
                string srcPath = AssetDatabase.GetAssetPath(mat);
                if (string.IsNullOrEmpty(srcPath))
                {
                    // Material de instancia, cria novo
                    var newMat = new Material(mat);
                    AssetDatabase.CreateAsset(newMat, newPath);
                    swap.matsDepois.Add(newMat);
                }
                else
                {
                    AssetDatabase.CopyAsset(srcPath, newPath);
                    var newMat = AssetDatabase.LoadAssetAtPath<Material>(newPath);
                    swap.matsDepois.Add(newMat);
                }
                Debug.Log($"{goName}: copiado {mat.name} -> {newPath}");
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(esfera);
        Debug.Log("=== Materiais do DEPOIS duplicados! ===");
        Debug.Log("Agora aplique as texturas do DEPOIS nos materiais em Assets/Trapiche/Materials/Depois/");
    }
}