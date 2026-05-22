using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Controla o estado Antes/Depois da cena do Trapiche.
/// Anexe este script a EsferaAntesDepois.
/// Arraste os renderers dos predios em "Renderers Edificios",
/// e defina os materiais de antes e depois para cada um.
/// </summary>
public class AntesDepoisController : MonoBehaviour
{
    public enum Estado { Antes, Depois }

    [Header("Estado Atual")]
    public Estado estadoAtual = Estado.Antes;

    [Header("Material da Esfera")]
    public Material materialAntes;
    public Material materialDepois;

    [Header("Edificios - Antes/Depois")]
    public List<RendererSwap> swaps = new List<RendererSwap>();

    [Header("Objetos exclusivos de cada estado")]
    public List<GameObject> objetosApenaAntes  = new List<GameObject>();
    public List<GameObject> objetosApenaDepois = new List<GameObject>();

    private Renderer _esferaRenderer;
    private XRSimpleInteractable _interactable;

    void Awake()
    {
        _esferaRenderer = GetComponent<Renderer>();

        _interactable = GetComponent<XRSimpleInteractable>();
        if (_interactable == null)
            _interactable = gameObject.AddComponent<XRSimpleInteractable>();

        _interactable.selectEntered.AddListener(OnSelecionado);
    }

    void Start() => AplicarEstado(estadoAtual, force: true);

    void OnDestroy()
    {
        if (_interactable != null)
            _interactable.selectEntered.RemoveListener(OnSelecionado);
    }

    void OnSelecionado(SelectEnterEventArgs args) => Alternar();

    public void Alternar()
    {
        estadoAtual = (estadoAtual == Estado.Antes) ? Estado.Depois : Estado.Antes;
        AplicarEstado(estadoAtual);
    }

    void AplicarEstado(Estado estado, bool force = false)
    {
        bool isDepois = (estado == Estado.Depois);

        // Esfera muda de cor para indicar o estado
        if (_esferaRenderer != null)
            _esferaRenderer.material = isDepois ? materialDepois : materialAntes;

        // Troca materiais dos edificios
        foreach (var swap in swaps)
        {
            if (swap.renderer == null) continue;
            var mats = swap.renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                if (i < swap.matsAntes.Count && i < swap.matsDepois.Count)
                    mats[i] = isDepois ? swap.matsDepois[i] : swap.matsAntes[i];
            }
            swap.renderer.materials = mats;
        }

        // Ativa/desativa objetos exclusivos
        foreach (var go in objetosApenaAntes)
            if (go != null) go.SetActive(!isDepois);

        foreach (var go in objetosApenaDepois)
            if (go != null) go.SetActive(isDepois);
    }
}

[System.Serializable]
public class RendererSwap
{
    public Renderer renderer;
    public List<Material> matsAntes  = new List<Material>();
    public List<Material> matsDepois = new List<Material>();
}
