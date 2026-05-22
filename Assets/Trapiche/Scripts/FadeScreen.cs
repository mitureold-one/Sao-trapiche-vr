using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton que gerencia o fade de tela no VR.
/// Adicione o prefab FadeScreen na cena ou ele se cria automaticamente.
/// </summary>
public class FadeScreen : MonoBehaviour
{
    public static FadeScreen Instance { get; private set; }

    private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWrite   = Shader.PropertyToID("_ZWrite");

    private MeshRenderer _renderer;
    private Material     _material;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _renderer = GetComponent<MeshRenderer>();
        _material = _renderer.material;
        SetAlpha(0f, Color.black);
    }

    public static void SetAlpha(float alpha, Color cor)
    {
        if (Instance == null) Create();
        cor.a = alpha;
        Instance._material.color = cor;

        // Liga/desliga o renderer para nao gastar GPU quando invisivel
        Instance._renderer.enabled = alpha > 0.001f;
    }

    static void Create()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "FadeScreen";
        Destroy(go.GetComponent<MeshCollider>());

        // Shader transparente simples
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        var mat    = new Material(shader);

        // Habilita transparencia no URP Unlit
        mat.SetFloat("_Surface", 1f);           // Transparent
        mat.SetFloat(SrcBlend,   (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat(DstBlend,   (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat(ZWrite,     0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = 3000;

        go.GetComponent<MeshRenderer>().material = mat;
        go.AddComponent<FadeScreen>();
    }

    void LateUpdate()
    {
        // Sempre na frente da camera principal
        Camera cam = Camera.main;
        if (cam == null) return;

        transform.position = cam.transform.position + cam.transform.forward * 0.31f;
        transform.rotation = cam.transform.rotation;
        transform.localScale = new Vector3(
            cam.aspect * 2f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 0.31f,
            2f         * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 0.31f,
            1f
        ) * 1.05f; // 5% maior para cobrir as bordas
    }
}
