using UnityEngine;
using UnityEditor;

public class SceneDumper
{
    [MenuItem("Trapiche/Logar Posicoes da Cena")]
    public static void LogarPosicoes()
    {
        string[] nomes = { "Chão", "Predio1", "Predio1Telhado", "Predio2", "Predio2Telhado", "Chamine1", "Chamine2" };
        foreach (string nome in nomes)
        {
            GameObject go = GameObject.Find(nome);
            if (go == null) { Debug.Log($"{nome} -> NAO ENCONTRADO"); continue; }
            var r = go.GetComponent<Renderer>();
            string bounds = r != null ? $" | bounds center({r.bounds.center.x:F2},{r.bounds.center.y:F2},{r.bounds.center.z:F2}) size({r.bounds.size.x:F2},{r.bounds.size.y:F2},{r.bounds.size.z:F2})" : "";
            Debug.Log($"{nome} -> pos({go.transform.position.x:F2},{go.transform.position.y:F2},{go.transform.position.z:F2}) rot({go.transform.eulerAngles.y:F0}){bounds}");
        }
    }
}
