#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Dimension1DarkThemeSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    private static readonly string[] AuthoredVisualRootNames =
    {
        "D1CommandCenterProductionRoot",
        "D1_GalaxyVisualRoot",
        "D1_ExploreVisualRoot",
        "D1_HangarVisualRoot",
        "D1_RelicsVisualRoot",
        "D1_TreeVisualRoot",
        "D1_MetalsInventoryRoot",
        "D1_AncientOrbitsVisualRoot",
        "D1_OuterRimDetailVisualRoot",
        "D1_DebrisRingDetailVisualRoot",
        "D1_SilentFrontierDetailVisualRoot",
        "ArkPanel",
        "D1_ExpeditionResultVisualRoot",
        "D1_ExpeditionRecordVisualRoot"
    };

    [MenuItem("Quantum Forge/Dimension 1/Apply Dark Theme")]
    public static void ApplyDarkTheme()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject host = GameObject.Find("Canvas");
        if (host == null) throw new System.InvalidOperationException("No se encontró Canvas en Main.");
        var theme = host.GetComponent<Dimension1DarkThemeRuntime>();
        if (theme == null) theme = host.AddComponent<Dimension1DarkThemeRuntime>();
        theme.ApplyTheme();
        EditorUtility.SetDirty(host);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Dimension1DarkThemeSetup] Tema oscuro aplicado sin modificar layout.");
    }

    [MenuItem("Quantum Forge/Dimension 1/Repair Authored Visual Root Markers")]
    public static void RepairAuthoredVisualRootMarkers()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        int repairedMissingScripts = 0;
        int addedMarkers = 0;
        int foundRoots = 0;

        foreach (string rootName in AuthoredVisualRootNames)
        {
            GameObject root = FindSceneObject(rootName);
            if (root == null) continue;
            foundRoots++;

            int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(root);
            if (missingCount > 0)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(root);
                repairedMissingScripts += missingCount;
            }

            if (root.GetComponent<Dimension1VisualSkinRoot>() == null)
            {
                root.AddComponent<Dimension1VisualSkinRoot>();
                addedMarkers++;
            }

            EditorUtility.SetDirty(root);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new System.InvalidOperationException("Unity no pudo guardar Main.unity.");

        foreach (string rootName in AuthoredVisualRootNames)
        {
            GameObject root = FindSceneObject(rootName);
            if (root != null && root.GetComponent<Dimension1VisualSkinRoot>() == null)
                throw new System.InvalidOperationException("No se pudo restaurar el marcador de " + rootName + ".");
        }

        Debug.Log($"[Dimension1DarkThemeSetup] VISUAL_ROOT_REPAIR_PASS | raíces={foundRoots} | " +
                  $"scripts perdidos retirados={repairedMissingScripts} | marcadores añadidos={addedMarkers}");
    }

    private static GameObject FindSceneObject(string objectName)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (transform.gameObject.scene.IsValid() && transform.name == objectName)
                return transform.gameObject;
        }
        return null;
    }
}
#endif
