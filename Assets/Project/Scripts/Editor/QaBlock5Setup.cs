#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QaBlock5Setup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/QA/Configure Block 5")]
    public static void ConfigureBlock5()
    {
        QaBlock4Setup.ConfigureBlock4();
        Scene scene = SceneManager.GetActiveScene();
        QaPanelUI panel = UnityEngine.Object.FindFirstObjectByType<QaPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null)
            throw new InvalidOperationException("No se encontró QaPanelUI.");

        QaCheckpointService[] services =
            UnityEngine.Object.FindObjectsByType<QaCheckpointService>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
        QaCheckpointService service;
        if (services.Length == 0)
        {
            service = panel.safeAreaRoot.gameObject.AddComponent<
                QaCheckpointService>();
        }
        else
        {
            service = services[0];
            for (int index = 1; index < services.Length; index++)
                UnityEngine.Object.DestroyImmediate(services[index]);
        }

        service.panel = panel;
        EditorUtility.SetDirty(service);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");

        Debug.Log("[QA Block 5 Setup] COMPLETE | servicio único | " +
            "slots A/B/C | pre_restore | panel conectado");
    }

    public static void ConfigureTwiceAndValidateBatch()
    {
        ConfigureBlock5();
        ConfigureBlock5();
        QaBlock5Validation.ValidateBlock5();
        EditorApplication.Exit(0);
    }
}
#endif
