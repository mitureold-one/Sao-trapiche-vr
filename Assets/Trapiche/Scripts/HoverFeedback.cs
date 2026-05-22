using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

/// <summary>
/// Feedback visual de hover para objetos XR interagiveis.
/// Adicione em qualquer GameObject com XRSimpleInteractable.
/// Ao ser mirado pelo ray interactor: escala e brilho aumentam suavemente.
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class HoverFeedback : MonoBehaviour
{
    [Header("Escala")]
    [Tooltip("Quanto a esfera cresce ao ser mirada (1.0 = sem mudanca)")]
    public float escalaHover   = 1.18f;
    public float duracaoEscala = 0.15f;

    [Header("Emissao")]
    [Tooltip("Multiplicador de brilho no hover (ex: 2 = dobro do brilho)")]
    public float multiplicadorEmissao = 2.5f;
    public float duracaoEmissao       = 0.15f;

    // ── estado interno ────────────────────────────────────────────────────
    private XRSimpleInteractable _interactable;
    private Renderer             _renderer;
    private Vector3              _escalaOriginal;
    private Color                _emissaoOriginal;
    private MaterialPropertyBlock _mpb;
    private Coroutine            _corEscala;
    private Coroutine            _corEmissao;
    private bool                 _emissaoAtiva;

    void Awake()
    {
        _interactable   = GetComponent<XRSimpleInteractable>();
        _renderer       = GetComponent<Renderer>();
        _escalaOriginal = transform.localScale;
        _mpb            = new MaterialPropertyBlock();

        // Detecta cor de emissao do material atual
        if (_renderer != null)
        {
            _renderer.GetPropertyBlock(_mpb);
            _emissaoOriginal = _renderer.sharedMaterial.HasProperty("_EmissionColor")
                ? _renderer.sharedMaterial.GetColor("_EmissionColor")
                : Color.black;

            // Garante que emissao esta habilitada no material
            _renderer.sharedMaterial.EnableKeyword("_EMISSION");
            _emissaoAtiva = true;
        }

        _interactable.hoverEntered.AddListener(_ => IniciarHover());
        _interactable.hoverExited.AddListener(_ => EncerrarHover());
    }

    void OnDestroy()
    {
        if (_interactable == null) return;
        _interactable.hoverEntered.RemoveAllListeners();
        _interactable.hoverExited.RemoveAllListeners();
    }

    void IniciarHover()
    {
        if (_corEscala  != null) StopCoroutine(_corEscala);
        if (_corEmissao != null) StopCoroutine(_corEmissao);
        _corEscala  = StartCoroutine(AnimarEscala(_escalaOriginal * escalaHover, duracaoEscala));
        _corEmissao = StartCoroutine(AnimarEmissao(_emissaoOriginal * multiplicadorEmissao, duracaoEmissao));
    }

    void EncerrarHover()
    {
        if (_corEscala  != null) StopCoroutine(_corEscala);
        if (_corEmissao != null) StopCoroutine(_corEmissao);
        _corEscala  = StartCoroutine(AnimarEscala(_escalaOriginal, duracaoEscala));
        _corEmissao = StartCoroutine(AnimarEmissao(_emissaoOriginal, duracaoEmissao));
    }

    IEnumerator AnimarEscala(Vector3 alvo, float duracao)
    {
        Vector3 inicio = transform.localScale;
        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(inicio, alvo, t / duracao);
            yield return null;
        }
        transform.localScale = alvo;
    }

    IEnumerator AnimarEmissao(Color alvo, float duracao)
    {
        if (_renderer == null || !_emissaoAtiva) yield break;
        _renderer.GetPropertyBlock(_mpb);
        Color inicio = _mpb.GetColor("_EmissionColor");
        // Se nao tinha valor no MPB, usa o do material
        if (inicio == Color.clear) inicio = _emissaoOriginal;

        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            _mpb.SetColor("_EmissionColor", Color.Lerp(inicio, alvo, t / duracao));
            _renderer.SetPropertyBlock(_mpb);
            yield return null;
        }
        _mpb.SetColor("_EmissionColor", alvo);
        _renderer.SetPropertyBlock(_mpb);
    }
}
