using UnityEngine;
using UnityEditor;

public class FixArvoresURP
{
    [MenuItem("Trapiche/Corrigir Texturas das Arvores (URP)")]
    public static void Corrigir()
    {
        // Usa o shader URP customizado que porta o original fielmente
        var shader = Shader.Find("Nature/Procedural Tree URP");
        if (shader == null) { Debug.LogError("Shader 'Nature/Procedural Tree URP' nao encontrado! Reimporte os assets."); return; }

        // Guids lidos diretamente dos .mat files
        var arvores = new[] {
            new { nome="Ash Tree",      bark="0604b01d2dbe3af4693bffccb2770ce4", leaf="d473b0108f3b4814496e34bfb01b21a9" },
            new { nome="Elm Tree",      bark="713a489322050504985ccb9fde65b5f0", leaf="a52d430bdc1ebb4459ed19cfe7a4b03b" },
            new { nome="Magnolia Tree", bark="c6ee6ccc2b893e14c9c232e44444ba9e", leaf="ff199dcefb702624b83584c6ef88f6e6" },
            new { nome="Oak Tree",      bark="063e28f38f0cd274b8b2a088b2300002", leaf="a58499fbd0c4d4846ba0d23558f85827" },
            new { nome="Pine Tree",     bark="e325313e0d694cc418de570db6f85280", leaf="1ce70ef5789aba04c9d72b94699b9eb5" },
            new { nome="Poplar Tree",   bark="1990cb7888ee9334b850149cd8e04d1b", leaf="337fa0565a2beb8468a5ec914073f004" },
        };

        foreach (var a in arvores)
        {
            string matPath = $"Assets/Procedural Tree/Materials/{a.nome}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null) { Debug.LogWarning("Nao encontrado: " + matPath); continue; }

            var barkTex = AssetDatabase.LoadAssetAtPath<Texture>(
                AssetDatabase.GUIDToAssetPath(a.bark));
            var leafTex = AssetDatabase.LoadAssetAtPath<Texture>(
                AssetDatabase.GUIDToAssetPath(a.leaf));

            mat.shader = shader;
            mat.SetTexture("_BarkMap", barkTex);
            mat.SetTexture("_LeafMap", leafTex);
            mat.SetColor("_BarkColor", Color.white);
            mat.SetColor("_LeafColor", Color.white);
            mat.SetFloat("_Cutoff", 0.4f);
            mat.SetFloat("_BarkSmoothness", 0.12f);
            mat.SetFloat("_LeafSmoothness", 0.05f);
            mat.SetFloat("_LeafTranslucency", 0.4f);

            EditorUtility.SetDirty(mat);
            Debug.Log($"{a.nome} | bark={barkTex?.name ?? "NULL"} | leaf={leafTex?.name ?? "NULL"}");
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Arvores corrigidas!");
    }
}
