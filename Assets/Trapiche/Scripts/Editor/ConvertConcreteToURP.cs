using UnityEngine;
using UnityEditor;

public class ConvertConcreteToURP
{
    [MenuItem("Trapiche/Convert Concrete to URP")]
    static void Convert()
    {
        var urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null) { Debug.LogError("URP Lit nao encontrado"); return; }

        string folder = "Assets/YughuesFreeConcreteMaterials";
        var guids = AssetDatabase.FindAssets("t:Material", new[] { folder });
        int count = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            var mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
            var bumpMap = mat.HasProperty("_BumpMap") ? mat.GetTexture("_BumpMap") : null;
            var specMap = mat.HasProperty("_SpecGlossMap") ? mat.GetTexture("_SpecGlossMap") : null;
            var color   = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;

            mat.shader = urpShader;

            if (mainTex != null) mat.SetTexture("_BaseMap", mainTex);
            if (bumpMap != null) { mat.SetTexture("_BumpMap", bumpMap); mat.EnableKeyword("_NORMALMAP"); }
            if (specMap != null) mat.SetTexture("_MetallicGlossMap", specMap);
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.1f);
            mat.SetFloat("_Metallic", 0f);

            EditorUtility.SetDirty(mat);
            count++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Convertidos {count} materiais de concreto para URP!");
    }
}