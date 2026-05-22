using System.Collections;
using UnityEngine;

/// <summary>
/// Coloque em qualquer GameObject na cena de destino.
/// Ao carregar, faz fade in automatico a partir do preto.
/// </summary>
public class SceneFadeIn : MonoBehaviour
{
    [Tooltip("Duracao do fade in em segundos")]
    public float duracao = 1.2f;

    IEnumerator Start()
    {
        // Garante que comeca preto mesmo se FadeScreen nao existia ainda
        FadeScreen.SetAlpha(1f, Color.black);

        yield return null; // espera um frame para a cena terminar de inicializar

        float tempo = 0f;
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            FadeScreen.SetAlpha(1f - (tempo / duracao), Color.black);
            yield return null;
        }

        FadeScreen.SetAlpha(0f, Color.black);
    }
}
