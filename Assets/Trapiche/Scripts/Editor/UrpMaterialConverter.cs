using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UrpMaterialConverter
{
    [MenuItem("Trapiche/Converter Materiais para URP")]
    public static void Converter()
    {
        var urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null) { Debug.LogError("Shader URP/Lit nao encontrado!"); return; }

        string[] pastas = {
            "Assets/0_free_pack",
            "Assets/PBS Materials Variety Pack",
            "Assets/YughuesFreeManmadeMaterials"
            // Procedural Tree excluido — usa shader URP customizado proprio
        };

        // Só converte shaders Standard Built-in (fileID 46 = Standard, 45 = Standard Specular)
        var shadersBuiltin = new HashSet<string> {
            "Standard", "Standard (Specular setup)",
            "Legacy Shaders/Diffuse", "Legacy Shaders/Bumped Diffuse"
        };

        string[] guids = AssetDatabase.FindAssets("t:Material", pastas);
        int convertidos = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            // Ignora materiais que nao sao Standard Built-in
            if (!shadersBuiltin.Contains(mat.shader.name)) continue;

            var mainTex   = mat.HasProperty("_MainTex")      ? mat.GetTexture("_MainTex")      : null;
            var bumpMap   = mat.HasProperty("_BumpMap")       ? mat.GetTexture("_BumpMap")       : null;
            var specGloss = mat.HasProperty("_SpecGlossMap")  ? mat.GetTexture("_SpecGlossMap")  : null;
            var occMap    = mat.HasProperty("_OcclusionMap")  ? mat.GetTexture("_OcclusionMap")  : null;
            var color     = mat.HasProperty("_Color")         ? mat.GetColor("_Color")           : Color.white;

            mat.shader = urpLit;

            // Folhas de arvore: detecta alpha cutout pelo nome ou renderQueue
            bool ehFolha = path.ToLower().Contains("leaf") || path.ToLower().Contains("leave")
                        || path.ToLower().Contains("folha") || path.ToLower().Contains("foliage");
            if (ehFolha)
            {
                mat.SetFloat("_Surface", 0f);          // Opaque
                mat.SetFloat("_AlphaClip", 1f);        // Habilita cutout
                mat.SetFloat("_Cutoff", 0.5f);
                mat.EnableKeyword("_ALPHATEST_ON");
                mat.renderQueue = 2450;                // AlphaTest queue
            }

            if (mainTex   != null) mat.SetTexture("_BaseMap",          mainTex);
            if (bumpMap   != null) mat.SetTexture("_BumpMap",          bumpMap);
            if (specGloss != null) mat.SetTexture("_MetallicGlossMap", specGloss);
            if (occMap    != null) mat.SetTexture("_OcclusionMap",     occMap);
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.25f);

            EditorUtility.SetDirty(mat);
            convertidos++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Convertidos {convertidos} materiais para URP/Lit.");
    }
}
