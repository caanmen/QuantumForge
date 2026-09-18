#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class QaBlock4Setup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/QA/Configure Block 4 Fast Forward")]
    public static void ConfigureBlock4()
    {
        QaPanelSetup.ConfigureBlock3Panel();
        Scene scene = SceneManager.GetActiveScene();
        QaPanelUI panel = Object.FindFirstObjectByType<QaPanelUI>(
            FindObjectsInactive.Include);
        if (panel == null)
        {
            Debug.LogError("[QA Block 4 Setup] Falta QaPanelUI.");
            return;
        }

        GameObject host = panel.safeAreaRoot != null
            ? panel.safeAreaRoot.gameObject
            : panel.gameObject;
        QaFastForwardRunner runner = host.GetComponent<QaFastForwardRunner>();
        if (runner == null)
            runner = host.AddComponent<QaFastForwardRunner>();

        QaFastForwardRunner[] runners =
            host.GetComponents<QaFastForwardRunner>();
        for (int index = 1; index < runners.Length; index++)
            Object.DestroyImmediate(runners[index]);

        runner.panel = panel;
        panel.SetOperationStatus("LISTO");
        EditorUtility.SetDirty(runner);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
        {
            Debug.LogError("[QA Block 4 Setup] No se pudo guardar Main.unity.");
            return;
        }

        Debug.Log("[QA Block 4 Setup] COMPLETE | runner único | panel conectado");
    }

    public static void ConfigureTwiceAndValidateBatch()
    {
        ConfigureBlock4();
        ConfigureBlock4();
        QaBlock3Validation.ValidateBlock3();
        QaBlock4Validation.ValidateBlock4();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Debug.Log("[QA Block 4 Setup] FULL PASS | panel | avance | escena");
    }
}
#endif
