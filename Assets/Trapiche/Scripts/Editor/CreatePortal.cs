using UnityEngine;
using UnityEditor;

public class CreatePortal
{
    [MenuItem("Trapiche/Criar Portal Lobby")]
    static void Create()
    {
        var botao = GameObject.Find("BotaoVoltar");
        if (botao == null) { Debug.LogError("BotaoVoltar nao encontrado"); return; }

        // Renomeia
        botao.name = "PortalLobby";

        // Remove MeshFilter e MeshRenderer antigos
        var oldMf = botao.GetComponent<MeshFilter>();
        var oldMr = botao.GetComponent<MeshRenderer>();
        if (oldMf != null) GameObject.DestroyImmediate(oldMf);
        if (oldMr != null) GameObject.DestroyImmediate(oldMr);

        // Cria material de portal emissivo
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        var mat = new Material(shader);
        mat.name = "Portal_URP";
        mat.SetColor("_BaseColor", new Color(0.0f, 0.5f, 1.0f, 0.8f));
        mat.SetColor("_EmissionColor", new Color(0.0f, 0.8f, 2.0f) * 3f);
        mat.EnableKeyword("_EMISSION");
        mat.SetFloat("_Surface", 1); // Transparent
        mat.SetFloat("_Blend", 0);
        mat.renderQueue = 3000;
        AssetDatabase.CreateAsset(mat, "Assets/Trapiche/Materials/Portal_URP.mat");
        AssetDatabase.SaveAssets();

        // Frame do portal — quad vertical
        var frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
        frame.name = "PortalFrame";
        frame.transform.SetParent(botao.transform);
        frame.transform.localPosition = Vector3.zero;
        frame.transform.localRotation = Quaternion.identity;
        frame.transform.localScale = new Vector3(2f, 3f, 1f);
        frame.GetComponent<Renderer>().sharedMaterial = mat;
        GameObject.DestroyImmediate(frame.GetComponent<MeshCollider>());

        // Borda do portal — quad ligeiramente maior
        var border = GameObject.CreatePrimitive(PrimitiveType.Quad);
        border.name = "PortalBorder";
        border.transform.SetParent(botao.transform);
        border.transform.localPosition = new Vector3(0, 0, 0.01f);
        border.transform.localRotation = Quaternion.identity;
        border.transform.localScale = new Vector3(2.2f, 3.2f, 1f);
        var borderMat = new Material(shader);
        borderMat.name = "PortalBorder_URP";
        borderMat.SetColor("_BaseColor", new Color(1f, 1f, 1f));
        borderMat.SetColor("_EmissionColor", new Color(0.5f, 0.8f, 1.0f) * 2f);
        borderMat.EnableKeyword("_EMISSION");
        AssetDatabase.CreateAsset(borderMat, "Assets/Trapiche/Materials/PortalBorder_URP.mat");
        border.GetComponent<Renderer>().sharedMaterial = borderMat;
        GameObject.DestroyImmediate(border.GetComponent<MeshCollider>());

        // Posiciona o portal verticalmente
        botao.transform.localScale = Vector3.one;

        // Ajusta SphereCollider para cobrir o portal
        var col = botao.GetComponent<SphereCollider>();
        if (col != null) { col.radius = 1.2f; col.center = Vector3.zero; }

        AssetDatabase.SaveAssets();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Portal criado!");
    }
}