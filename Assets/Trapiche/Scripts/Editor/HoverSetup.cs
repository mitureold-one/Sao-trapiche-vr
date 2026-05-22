using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class HoverSetup
{
    [MenuItem("Trapiche/Adicionar HoverFeedback nas Esferas")]
    public static void Adicionar()
    {
        int adicionados = 0;

        // Procura nas duas cenas carregadas
        foreach (string nome in new[] { "BotaoEntrar", "EsferaAntesDepois" })
        {
            var go = GameObject.Find(nome);
            if (go == null) { Debug.LogWarning($"{nome} nao encontrado na cena aberta."); continue; }

            var hf = go.GetComponent<HoverFeedback>();
            if (hf != null) { Debug.Log($"{nome}: HoverFeedback ja existe."); continue; }

            go.AddComponent<HoverFeedback>();
            EditorUtility.SetDirty(go);
            Debug.Log($"{nome}: HoverFeedback adicionado.");
            adicionados++;
        }

        if (adicionados > 0)
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
