using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TrapicheControllerSetup
{
    [MenuItem("Trapiche/Setup AntesDepois Controller")]
    static void Setup()
    {
        var esfera = GameObject.Find("EsferaAntesDepois");
        if (esfera == null) { Debug.LogError("EsferaAntesDepois nao encontrada"); return; }

        var ctrl = esfera.GetComponent<AntesDepoisController>();
        if (ctrl == null) { Debug.LogError("AntesDepoisController nao encontrado"); return; }

        // --- Objetos apenas Antes ---
        ctrl.objetosApenaAntes.Clear();
        var gramaAntes = GameObject.Find("GramaAntes");
        if (gramaAntes != null) ctrl.objetosApenaAntes.Add(gramaAntes);

        // --- Objetos apenas Depois ---
        ctrl.objetosApenaDepois.Clear();
        var gramaDepois = GameObject.Find("GramaDepois");
        if (gramaDepois != null) ctrl.objetosApenaDepois.Add(gramaDepois);

        var predio1Telhado = GameObject.Find("Predio1Telhado");
        if (predio1Telhado != null) ctrl.objetosApenaDepois.Add(predio1Telhado);

        // Bancos
        foreach (var name in new[] { "Banco (1)", "Banco (2)", "Banco (3)" }) {
            var b = GameObject.Find(name);
            if (b != null) ctrl.objetosApenaDepois.Add(b);
        }

        // --- Swaps de material ---
        ctrl.swaps.Clear();

        // Materiais Antes
        var matParedeDegradada = AssetDatabase.LoadAssetAtPath<Material>("Assets/Trapiche/Materials/Parede.mat");
        var matChaoAntes       = AssetDatabase.LoadAssetAtPath<Material>("Assets/YughuesFreeConcreteMaterials/Materials/M_YFCM_PrecastD4.mat");

        // Materiais Depois
        var matParedeNova = AssetDatabase.LoadAssetAtPath<Material>("Assets/Trapiche/Materials/Parede.mat");
        var matChaoDepois = AssetDatabase.LoadAssetAtPath<Material>("Assets/YughuesFreeConcreteMaterials/Materials/M_YFCM_PrecastD16.mat");

        // Predio1 — troca materiais
        var predio1 = GameObject.Find("Predio1");
        if (predio1 != null)
        {
            var rend = predio1.GetComponent<Renderer>();
            if (rend != null)
            {
                var swap = new RendererSwap();
                swap.renderer = rend;
                // Antes: materiais atuais
                foreach (var m in rend.sharedMaterials) swap.matsAntes.Add(m);
                // Depois: mesmos por enquanto (pode customizar)
                foreach (var m in rend.sharedMaterials) swap.matsDepois.Add(m);
                ctrl.swaps.Add(swap);
            }
        }

        // Chao
        var chao = GameObject.Find("Chão");
        if (chao != null)
        {
            var rend = chao.GetComponent<Renderer>();
            if (rend != null && matChaoAntes != null && matChaoDepois != null)
            {
                var swap = new RendererSwap();
                swap.renderer = rend;
                swap.matsAntes.Add(matChaoAntes);
                swap.matsDepois.Add(matChaoDepois);
                ctrl.swaps.Add(swap);
            }
        }

        EditorUtility.SetDirty(esfera);
        Debug.Log("AntesDepoisController configurado! " +
                  $"Antes: {ctrl.objetosApenaAntes.Count} objetos | " +
                  $"Depois: {ctrl.objetosApenaDepois.Count} objetos | " +
                  $"Swaps: {ctrl.swaps.Count}");
    }
}