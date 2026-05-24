using UnityEngine;
using UnityEditor;

public class CreateGroundMaterial
{
    [MenuItem("Trapiche/Create Grass Ground URP Material")]
    static void Create()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) { Debug.LogError("URP Lit shader not found"); return; }

        string basePath = "Assets/Cartoon_Texture_Pack/GRASS/GRASS_Dense/GRASS_Dense_Tint_01/Textures/";

        var diffuse = AssetDatabase.LoadAssetAtPath<Texture2D>(
            basePath + "Grass_Dense_Tint_01_Base_Basecolor_A.png");
        var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(
            basePath + "Grass_Dense_Tint_01_Base_Normal.png");
        var ao = AssetDatabase.LoadAssetAtPath<Texture2D>(
            basePath + "Grass_Dense_Tint_01_Base_AO.png");

        var mat = new Material(shader);
        mat.name = "GrassGround_URP";

        if (diffuse != null) mat.SetTexture("_BaseMap", diffuse);
        else mat.SetColor("_BaseColor", new Color(0.2f, 0.45f, 0.1f));

        if (normal != null) { mat.SetTexture("_BumpMap", normal); mat.EnableKeyword("_NORMALMAP"); }
        if (ao != null) mat.SetTexture("_OcclusionMap", ao);

        mat.SetFloat("_Smoothness", 0.05f);
        mat.SetFloat("_Metallic", 0f);
        mat.SetTextureScale("_BaseMap", new Vector2(10, 10));

        string savePath = "Assets/Trapiche/Materials/GrassGround_URP.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(savePath) != null)
            AssetDatabase.DeleteAsset(savePath);

        AssetDatabase.CreateAsset(mat, savePath);
        AssetDatabase.SaveAssets();

        var chao = GameObject.Find("Chão");
        if (chao != null)
        {
            var rend = chao.GetComponent<Renderer>();
            if (rend != null) { rend.sharedMaterial = mat; Debug.Log("Aplicado no Chão!"); }
        }

        Debug.Log("Textura: " + (diffuse != null ? "OK" : "NAO ENCONTRADA"));
    }
}
