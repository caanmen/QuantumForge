#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3QueuesValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Queues Full")]
    public static void ValidateAll()
    {
        bool failed = false;
        Application.LogCallback capture = (message, stack, type) =>
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                failed = true;
        };
        Application.logMessageReceived += capture;
        try { RunChecks(); }
        finally { Application.logMessageReceived -= capture; }
        if (failed)
            throw new System.InvalidOperationException("[D3 Queues Full Validation] FAIL | revisar los errores de los validadores anteriores.");
        Debug.Log("[D3 Queues Full Validation] PASS | composición/táctil | colas/progreso/cancelación | D3 1-7D | Main | layout móvil");
    }

    private static void RunChecks()
    {
        Dimension3QueuesVisualSetup.ValidateCurrentBatch();
        Dimension3Block7FullValidation.ValidateFullThrough7D();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3QueuesVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3QueuesVisualSetup.ValidateCurrentBatch();
    }

    public static void ValidateAllBatch() => ValidateAll();
}
#endif
