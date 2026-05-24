using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalLoader : MonoBehaviour
{
    public string nomeCena = "Lobby";
    public float duracaoFade = 1.2f;
    public Color corFade = Color.black;
    private bool _carregando = false;

    void OnTriggerEnter(Collider other)
    {
        if (_carregando) return;
        if (other.CompareTag("MainCamera") || other.name.Contains("Camera") || other.name.Contains("XR"))
            StartCoroutine(CarregarCena());
    }

    IEnumerator CarregarCena()
    {
        _carregando = true;
        yield return StartCoroutine(Fade(0f, 1f));
        var op = SceneManager.LoadSceneAsync(nomeCena);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f) yield return null;
        op.allowSceneActivation = true;
    }

    IEnumerator Fade(float de, float para)
    {
        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            FadeScreen.SetAlpha(Mathf.Lerp(de, para, tempo / duracaoFade), corFade);
            yield return null;
        }
        FadeScreen.SetAlpha(para, corFade);
    }
}