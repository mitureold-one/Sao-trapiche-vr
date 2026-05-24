using UnityEngine;
using UnityEditor;
using System.IO;

public class ConvertMaterials
{
    [MenuItem("Trapiche/Convert Yughues Concrete to URP")]
    static void Convert()
    {
        string folder = "Assets/YughuesFreeManmadeMaterials/Materials";
        var guids = AssetDatabase.FindAssets("t:Material", new[] { folder });
        var urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null) { Debug.LogError("URP Lit nao encontrado"); return; }

        int count = 0;
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            // Salva texturas antes de trocar o shader
            var mainTex   = mat.HasProperty("_MainTex")   ? mat.GetTexture("_MainTex")   : null;
            var bumpMap   = mat.HasProperty("_BumpMap")   ? mat.GetTexture("_BumpMap")   : null;
            var specGloss = mat.HasProperty("_SpecGlossMap") ? mat.GetTexture("_SpecGlossMap") : null;
            var color     = mat.HasProperty("_Color")     ? mat.GetColor("_Color")       : Color.white;

            mat.shader = urpShader;

            if (mainTex   != null) mat.SetTexture("_BaseMap", mainTex);
            if (bumpMap   != null) { mat.SetTexture("_BumpMap", bumpMap); mat.EnableKeyword("_NORMALMAP"); }
            if (specGloss != null) mat.SetTexture("_MetallicGlossMap", specGloss);
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.1f);
            mat.SetFloat("_Metallic", 0f);

            EditorUtility.SetDirty(mat);
            count++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Convertidos {count} materiais para URP!");
    }
}