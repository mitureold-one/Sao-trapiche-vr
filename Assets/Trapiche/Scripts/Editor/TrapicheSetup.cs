using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class TrapicheSetup
{
    // ── Paths ───────────────────────────────────────────────────────────────
    const string SKYBOX  = "Assets/AllSkyFree/Cold Sunset/Cold Sunset.mat";
    const string PBS     = "Assets/PBS Materials Variety Pack/";
    const string FREE    = "Assets/0_free_pack/Materials/";

    // ANTES — paredes branco-cinza sujas, tijolo exposto acinzentado (foto real)
    const string A_PAREDE  = FREE + "plaster_02_white.mat";   // reboco branco sujo
    const string A_TIJOLO  = FREE + "bricks_03_grey.mat";     // tijolo exposto acinzentado
    const string A_TELHADO = FREE + "wood_plates_05.mat";     // madeira deteriorada
    const string A_PISO    = FREE + "paving_02.mat";

    // DEPOIS — restaurado: creme/amarelado + telhado terracota (foto dez/2025)
    const string D_PAREDE  = FREE + "plaster_02_yellow.mat";  // creme/amarelado como na foto
    const string D_TIJOLO  = FREE + "bricks_03_red.mat";      // tijolo vermelho limpo
    const string D_TELHADO = FREE + "bricks_03_red.mat";      // terracota no telhado
    const string D_PISO    = FREE + "granite_01.mat";         // granito claro restaurado

    [MenuItem("Trapiche/Aplicar Materiais e Configurar Cena")]
    public static void Configurar()
    {
        // 1. Skybox
        var skybox = Load<Material>(SKYBOX);
        if (skybox != null) { RenderSettings.skybox = skybox; DynamicGI.UpdateEnvironment(); Debug.Log("Skybox aplicado."); }

        // 2. Chao — antes: paralelepípedo, depois: granito claro
        AplicarMat("Ambiente/Chão", A_PISO);

        // Adiciona chao ao swap do controlador
        var alvosChao = new[]{ (path:"Ambiente/Chão", a:new[]{A_PISO}, d:new[]{D_PISO}) };

        // 3. Predios — 2 slots: [0]=reboco [1]=tijolo
        Swap("Edificios/Predio1",       new[]{A_PAREDE, A_TIJOLO},  new[]{D_PAREDE, D_TIJOLO});
        Swap("Edificios/Predio2",       new[]{A_PAREDE, A_TIJOLO},  new[]{D_PAREDE, D_TIJOLO});
        Swap("Edificios/Predio1Telhado",new[]{A_TELHADO},           new[]{D_TELHADO});
        Swap("Edificios/Predio2Telhado",new[]{A_TELHADO},           new[]{D_TELHADO});
        Swap("Edificios/Chamine1",      new[]{A_TIJOLO},            new[]{D_TIJOLO});
        Swap("Edificios/Chamine2",      new[]{A_TIJOLO},            new[]{D_TIJOLO});

        // 4. Adiciona arvores (grupo desativavel para teste de performance)
        AdicionarArvores();

        // 5. Configura controlador
        ConfigurarControlador();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("Setup do Trapiche concluido!");
    }

    static void AdicionarArvores()
    {
        // Cria grupo pai desativavel — facil de ligar/desligar no Quest 2
        var grpExistente = GameObject.Find("Vegetacao");
        if (grpExistente != null) return; // ja existe, nao duplica

        var grp = new GameObject("Vegetacao");

        // Palmeira direita (como na foto)
        var palmeira = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Procedural Tree/Prefabs/Ash Tree.prefab");
        if (palmeira != null)
        {
            var t1 = (GameObject)PrefabUtility.InstantiatePrefab(palmeira);
            t1.name = "Arvore_Direita";
            t1.transform.SetParent(grp.transform);
            t1.transform.position = new Vector3(8f, 0f, 14f);
            t1.transform.localScale = Vector3.one * 1.8f;
        }

        // Arvore grande esquerda/fundo
        var oak = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Procedural Tree/Prefabs/Oak Tree.prefab");
        if (oak != null)
        {
            var t2 = (GameObject)PrefabUtility.InstantiatePrefab(oak);
            t2.name = "Arvore_Fundo";
            t2.transform.SetParent(grp.transform);
            t2.transform.position = new Vector3(-10f, 0f, -8f);
            t2.transform.localScale = Vector3.one * 2.2f;
        }

        EditorUtility.SetDirty(grp);
        Debug.Log("Vegetacao adicionada (desative o grupo 'Vegetacao' se cair FPS no Quest 2).");
    }

    static void Swap(string goPath, string[] antes, string[] depois)
    {
        var go = GameObject.Find(goPath);
        if (go == null) { Debug.LogWarning("Nao encontrado: " + goPath); return; }
        var r = go.GetComponent<Renderer>();
        if (r == null) return;

        var mats = r.sharedMaterials;
        for (int i = 0; i < mats.Length; i++)
            if (i < antes.Length) { var m = Load<Material>(antes[i]); if (m != null) mats[i] = m; }
        r.sharedMaterials = mats;
        EditorUtility.SetDirty(go);
    }

    static void AplicarMat(string goPath, string matPath)
    {
        var go = GameObject.Find(goPath);
        if (go == null) { Debug.LogWarning("Nao encontrado: " + goPath); return; }
        var r = go.GetComponent<Renderer>();
        if (r == null) return;
        var m = Load<Material>(matPath);
        if (m != null) { r.sharedMaterial = m; EditorUtility.SetDirty(go); }
    }

    static void ConfigurarControlador()
    {
        var esferaGO = GameObject.Find("EsferaAntesDepois");
        if (esferaGO == null) { Debug.LogWarning("EsferaAntesDepois nao encontrada!"); return; }

        var ctrl = esferaGO.GetComponent<AntesDepoisController>()
                ?? esferaGO.AddComponent<AntesDepoisController>();

        ctrl.materialAntes  = Load<Material>("Assets/Trapiche/Materials/Esfera_Antes.mat");
        ctrl.materialDepois = Load<Material>("Assets/Trapiche/Materials/Esfera_Depois.mat");
        ctrl.swaps.Clear();

        var alvos = new[]{
            (path:"Ambiente/Chão",            a:new[]{A_PISO},            d:new[]{D_PISO}),
            (path:"Edificios/Predio1",        a:new[]{A_PAREDE,A_TIJOLO}, d:new[]{D_PAREDE,D_TIJOLO}),
            (path:"Edificios/Predio2",        a:new[]{A_PAREDE,A_TIJOLO}, d:new[]{D_PAREDE,D_TIJOLO}),
            (path:"Edificios/Predio1Telhado", a:new[]{A_TELHADO},         d:new[]{D_TELHADO}),
            (path:"Edificios/Predio2Telhado", a:new[]{A_TELHADO},         d:new[]{D_TELHADO}),
            (path:"Edificios/Chamine1",       a:new[]{A_TIJOLO},          d:new[]{D_TIJOLO}),
            (path:"Edificios/Chamine2",       a:new[]{A_TIJOLO},          d:new[]{D_TIJOLO}),
        };

        foreach (var alvo in alvos)
        {
            var go = GameObject.Find(alvo.path);
            if (go == null) continue;
            var r = go.GetComponent<Renderer>();
            if (r == null) continue;
            var sw = new RendererSwap { renderer = r };
            foreach (var p in alvo.a) sw.matsAntes.Add(Load<Material>(p));
            foreach (var p in alvo.d) sw.matsDepois.Add(Load<Material>(p));
            ctrl.swaps.Add(sw);
        }

        EditorUtility.SetDirty(esferaGO);
        Debug.Log($"AntesDepoisController: {ctrl.swaps.Count} renderers configurados.");
    }

    static T Load<T>(string path) where T : UnityEngine.Object =>
        AssetDatabase.LoadAssetAtPath<T>(path);
}
