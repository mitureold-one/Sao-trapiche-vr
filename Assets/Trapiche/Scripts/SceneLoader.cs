using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Anexe na EsferaAntesDepois ou em qualquer objeto interagivel.
/// Arraste o nome da cena destino no Inspector.
/// Usa fade preto para evitar desconforto no Quest 2.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Cena destino")]
    public string nomeCena = "Trapiche";

    [Header("Fade")]
    public float duracaoFade = 1.2f;
    public Color corFade     = Color.black;

    private XRSimpleInteractable _interactable;
    private bool _carregando = false;

    void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        if (_interactable == null)
            _interactable = gameObject.AddComponent<XRSimpleInteractable>();

        _interactable.selectEntered.AddListener(OnSelecionado);
    }

    void OnDestroy()
    {
        if (_interactable != null)
            _interactable.selectEntered.RemoveListener(OnSelecionado);
    }

    void OnSelecionado(SelectEnterEventArgs args)
    {
        if (!_carregando) StartCoroutine(CarregarCena());
    }

    IEnumerator CarregarCena()
    {
        _carregando = true;

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f));

        // Carrega cena em background
        var op = SceneManager.LoadSceneAsync(nomeCena);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;
    }

    IEnumerator Fade(float de, float para)
    {
        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            float alpha = Mathf.Lerp(de, para, tempo / duracaoFade);
            FadeScreen.SetAlpha(alpha, corFade);
            yield return null;
        }
        FadeScreen.SetAlpha(para, corFade);
    }
}
