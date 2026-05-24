using UnityEngine;
using UnityEditor;

public class SetupChaoSwap
{
    [MenuItem("Trapiche/Setup Swap Chao")]
    static void Setup()
    {
        var esfera = GameObject.Find("EsferaAntesDepois");
        var ctrl = esfera.GetComponent<AntesDepoisController>();

        var chao = GameObject.Find("Chão");
        var rend = chao.GetComponent<Renderer>();

        var matAntes  = AssetDatabase.LoadAssetAtPath<Material>("Assets/Trapiche/Materials/GrassGround_URP.mat");
        var matDepois = AssetDatabase.LoadAssetAtPath<Material>("Assets/YughuesFreeConcreteMaterials/Materials/M_YFCM_PrecastD4.mat");

        ctrl.swaps.Clear();

        var swap = new RendererSwap();
        swap.renderer = rend;
        swap.matsAntes.Add(matAntes);
        swap.matsDepois.Add(matDepois);
        ctrl.swaps.Add(swap);

        // Objetos
        ctrl.objetosApenaAntes.Clear();
        ctrl.objetosApenaDepois.Clear();

        var gramaAntes = GameObject.Find("GramaAntes");
        if (gramaAntes != null) ctrl.objetosApenaAntes.Add(gramaAntes);

        var gramaDepois = GameObject.Find("GramaDepois");
        if (gramaDepois != null) ctrl.objetosApenaDepois.Add(gramaDepois);

        var telhado = GameObject.Find("Predio1Telhado");
        if (telhado != null) ctrl.objetosApenaDepois.Add(telhado);

        EditorUtility.SetDirty(esfera);
        Debug.Log("Swap do chao configurado! Antes=GrassGround, Depois=PrecastD4");
    }
}