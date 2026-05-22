using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class LobbyPainelBuilder
{
    [MenuItem("Trapiche/Montar Paineis do Lobby")]
    public static void MontarPaineis()
    {
        GameObject paineis = GameObject.Find("Paineis");
        if (paineis == null) { Debug.LogError("'Paineis' nao encontrado!"); return; }

        LimparCanvasAntigos();

        // PainelTitulo: (0, 3.2, 9) scale (4, 0.9, 0.05)
        CriarCanvas(paineis, "CanvasTitulo", new Vector3(0f, 3.2f, 8.97f), 4f, 0.9f,
            new TxtCfg("TextTitulo", "COMPLEXO SAO TRAPICHE", 130,
                new Vector2(0.04f, 0.28f), new Vector2(0.96f, 0.95f),
                new Color(0.4f, 0.92f, 1f), FontStyles.Bold),
            new TxtCfg("TextSubtitulo", "Sao Luis, Maranhao", 65,
                new Vector2(0.04f, 0.02f), new Vector2(0.96f, 0.28f),
                new Color(0.6f, 0.85f, 1f), FontStyles.Normal)
        );

        // PainelHistoria: (0, 1.9, 9.05) scale (4, 2.0, 0.05)
        CriarCanvas(paineis, "CanvasHistoria", new Vector3(0f, 1.9f, 9.02f), 4f, 2.0f,
            new TxtCfg("TextHistoria",
                "O Complexo Sao Trapiche e um conjunto arquitetonico historico " +
                "localizado no centro de Sao Luis.\n\n" +
                "Construido no seculo XIX, serviu como entreposto comercial " +
                "durante o ciclo do algodao, sendo um dos mais importantes da regiao.\n\n" +
                "Esta experiencia apresenta o antes e depois da restauracao, " +
                "preservando a memoria da arquitetura colonial maranhense.",
                58,
                new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f),
                new Color(0.82f, 0.93f, 1f), FontStyles.Normal)
        );

        // BotaoEntrar: esfera em (0, 1.1, 7), raio = 0.225
        // Label flutua 0.5m acima do topo da esfera (topo = 1.325, label = 1.825)
        GameObject botao = GameObject.Find("BotaoEntrar");
        if (botao != null)
        {
            Vector3 esfPos = botao.transform.position;
            float raio = botao.transform.localScale.y * 0.5f;
            Vector3 pos = new Vector3(esfPos.x, esfPos.y + raio + 0.5f, esfPos.z - 0.05f);
            CriarCanvas(null, "CanvasBotao", pos, 1.8f, 0.25f,
                new TxtCfg("TextBotao", "ENTRAR NO TRAPICHE", 100,
                    new Vector2(0.01f, 0.05f), new Vector2(0.99f, 0.95f),
                    new Color(0.4f, 0.92f, 1f), FontStyles.Bold)
            );
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Paineis do void room montados!");
    }

    static void LimparCanvasAntigos()
    {
        foreach (string nome in new[]{"CanvasTitulo","CanvasHistoria","CanvasBotao"})
        {
            GameObject go = GameObject.Find(nome);
            if (go != null) { Object.DestroyImmediate(go); }
        }
    }

    static void CriarCanvas(GameObject parent, string nome, Vector3 worldPos,
        float largura, float altura, params TxtCfg[] textos)
    {
        GameObject go = new GameObject(nome);
        if (parent != null)
            go.transform.SetParent(parent.transform, worldPositionStays: true);

        go.transform.position      = worldPos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale    = Vector3.one * 0.001f;

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        go.AddComponent<GraphicRaycaster>();

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(largura * 1000f, altura * 1000f);

        foreach (var cfg in textos)
        {
            GameObject tgo = new GameObject(cfg.nome);
            tgo.transform.SetParent(go.transform, false);

            TextMeshProUGUI tmp   = tgo.AddComponent<TextMeshProUGUI>();
            tmp.text               = cfg.conteudo;
            tmp.fontSize           = cfg.fontSize;
            tmp.color              = cfg.cor;
            tmp.fontStyle          = cfg.estilo;
            tmp.alignment          = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = true;
            tmp.raycastTarget      = false;

            RectTransform trt = tgo.GetComponent<RectTransform>();
            trt.anchorMin = cfg.anchorMin;
            trt.anchorMax = cfg.anchorMax;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;
        }
    }

    struct TxtCfg
    {
        public string nome, conteudo;
        public int fontSize;
        public Vector2 anchorMin, anchorMax;
        public Color cor;
        public FontStyles estilo;

        public TxtCfg(string nome, string conteudo, int fontSize,
            Vector2 anchorMin, Vector2 anchorMax, Color cor, FontStyles estilo)
        {
            this.nome=nome; this.conteudo=conteudo; this.fontSize=fontSize;
            this.anchorMin=anchorMin; this.anchorMax=anchorMax;
            this.cor=cor; this.estilo=estilo;
        }
    }
}
