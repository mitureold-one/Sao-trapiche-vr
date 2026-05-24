using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SetupAntesState
{
    [MenuItem("Trapiche/Setup Estado ANTES (nao toca no depois)")]
    static void Setup()
    {
        var esfera = GameObject.Find("EsferaAntesDepois");
        if (esfera == null) { Debug.LogError("EsferaAntesDepois nao encontrada"); return; }

        var ctrl = esfera.GetComponent<AntesDepoisController>();
        if (ctrl == null) { Debug.LogError("AntesDepoisController nao encontrado"); return; }

        // Objetos apenas Antes
        ctrl.objetosApenaAntes.Clear();
        var gramaAntes = GameObject.Find("GramaAntes");
        if (gramaAntes != null) { ctrl.objetosApenaAntes.Add(gramaAntes); Debug.Log("Antes: GramaAntes adicionado"); }

        // Objetos apenas Depois - NAO TOCA, so adiciona se estiver vazio
        if (ctrl.objetosApenaDepois.Count == 0)
        {
            var gramaDepois = GameObject.Find("GramaDepois");
            if (gramaDepois != null) ctrl.objetosApenaDepois.Add(gramaDepois);
            var telhado = GameObject.Find("Predio1Telhado");
            if (telhado != null) ctrl.objetosApenaDepois.Add(telhado);
            Debug.Log("Depois: lista estava vazia, adicionou GramaDepois e Predio1Telhado");
        }
        else Debug.Log("Depois: lista ja tinha " + ctrl.objetosApenaDepois.Count + " objetos, NAO TOCOU");

        // Swaps - le os materiais ATUAIS e seta como matsAntes
        // NAO SOBRESCREVE matsDepois se ja existir
        var renderers = new (string name, string path)[] {
            ("Predio1",  "Edificios/Predio1"),
            ("Predio2",  "Edificios/Predio2"),
            ("Chamine1", "Edificios/Chamine1"),
            ("Chamine2", "Edificios/Chamine2"),
            ("Chão",     "Ambiente/Chão"),
        };

        // Limpa swaps existentes
        ctrl.swaps.Clear();

        foreach (var r in renderers)
        {
            var go = GameObject.Find(r.name);
            if (go == null) continue;
            var rend = go.GetComponent<Renderer>();
            if (rend == null) continue;

            var swap = new RendererSwap();
            swap.renderer = rend;

            // matsAntes = materiais atuais na cena
            foreach (var m in rend.sharedMaterials)
                swap.matsAntes.Add(m);

            // matsDepois = mesmos por enquanto (usuario vai arrastar depois)
            foreach (var m in rend.sharedMaterials)
                swap.matsDepois.Add(m);

            ctrl.swaps.Add(swap);
            Debug.Log($"Swap adicionado: {r.name} com {rend.sharedMaterials.Length} materiais");
        }

        EditorUtility.SetDirty(esfera);
        Debug.Log("=== ESTADO ANTES CONFIGURADO ===");
        Debug.Log("IMPORTANTE: Arraste os materiais do DEPOIS manualmente em cada swap.matsDepois no Inspector!");
    }
}