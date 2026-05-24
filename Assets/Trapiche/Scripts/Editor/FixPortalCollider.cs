using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FixPortalCollider
{
    [MenuItem("Trapiche/Fix Portal Collider")]
    static void Fix()
    {
        var portal = GameObject.Find("PortalLobby");
        if (portal == null) { Debug.LogError("PortalLobby nao encontrado"); return; }

        var interactable = portal.GetComponent<XRSimpleInteractable>();
        if (interactable == null) { Debug.LogError("XRSimpleInteractable nao encontrado"); return; }

        var col = portal.GetComponent<SphereCollider>();
        if (col == null) { col = portal.AddComponent<SphereCollider>(); }

        col.isTrigger = false;
        col.radius = 1.5f;
        col.center = new Vector3(0, 1.5f, 0);

        // Registra o collider no interactable
        interactable.colliders.Clear();
        interactable.colliders.Add(col);

        EditorUtility.SetDirty(portal);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("Collider registrado no XRSimpleInteractable!");
    }
}