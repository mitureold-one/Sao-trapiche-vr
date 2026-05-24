using UnityEngine;
using UnityEditor;

public class CreatePortal3D
{
    [MenuItem("Trapiche/Criar Portal 3D")]
    static void Create()
    {
        var portal = GameObject.Find("PortalLobby");
        if (portal == null) { Debug.LogError("PortalLobby nao encontrado"); return; }

        var urp = Shader.Find("Universal Render Pipeline/Lit");

        // Material portal azul emissivo
        var matPortal = AssetDatabase.LoadAssetAtPath<Material>("Assets/Trapiche/Materials/Portal_URP.mat");
        if (matPortal == null) matPortal = new Material(urp);
        matPortal.SetColor("_BaseColor", new Color(0f, 0.6f, 1f, 0.9f));
        matPortal.SetColor("_EmissionColor", new Color(0f, 1f, 2.5f) * 2.5f);
        matPortal.EnableKeyword("_EMISSION");
        matPortal.SetFloat("_Cull", 0);
        EditorUtility.SetDirty(matPortal);

        // Material pedra para o arco
        var matArco = new Material(urp);
        matArco.SetColor("_BaseColor", new Color(0.4f, 0.4f, 0.4f));
        matArco.SetFloat("_Smoothness", 0.1f);
        AssetDatabase.CreateAsset(matArco, "Assets/Trapiche/Materials/PortalArco_URP.mat");
        AssetDatabase.SaveAssets();

        float w = 1.8f;   // largura do portal
        float h = 3.0f;   // altura
        float thick = 0.25f; // espessura do arco

        // Coluna esquerda
        CreatePillar(portal, "ColunaEsq", new Vector3(-w/2f, h/2f, 0), h, thick, matArco);
        // Coluna direita
        CreatePillar(portal, "ColunaDir", new Vector3(w/2f, h/2f, 0), h, thick, matArco);
        // Verga superior
        CreateBeam(portal, "Verga", new Vector3(0, h, 0), w + thick, thick, matArco);

        // Plano emissivo no centro
        var center = GameObject.CreatePrimitive(PrimitiveType.Quad);
        center.name = "PortalCenter";
        center.transform.SetParent(portal.transform);
        center.transform.localPosition = new Vector3(0, h/2f, 0);
        center.transform.localEulerAngles = new Vector3(0, 0, 0);
        center.transform.localScale = new Vector3(w, h, 1);
        center.GetComponent<Renderer>().sharedMaterial = matPortal;
        GameObject.DestroyImmediate(center.GetComponent<MeshCollider>());

        // Ajusta collider
        var col = portal.GetComponent<SphereCollider>();
        if (col != null) { col.radius = 1.5f; col.center = new Vector3(0, h/2f, 0); }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Portal 3D criado!");
    }

    static void CreatePillar(GameObject parent, string name, Vector3 pos, float height, float thick, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = pos;
        go.transform.localScale = new Vector3(thick, height/2f, thick);
        go.GetComponent<Renderer>().sharedMaterial = mat;
        GameObject.DestroyImmediate(go.GetComponent<CapsuleCollider>());
    }

    static void CreateBeam(GameObject parent, string name, Vector3 pos, float width, float thick, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent.transform);
        go.transform.localPosition = pos;
        go.transform.localScale = new Vector3(width, thick, thick);
        go.GetComponent<Renderer>().sharedMaterial = mat;
        GameObject.DestroyImmediate(go.GetComponent<BoxCollider>());
    }
}