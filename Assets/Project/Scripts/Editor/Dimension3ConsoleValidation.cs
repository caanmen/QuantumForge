#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3ConsoleValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Console Full")]
    public static void ValidateAll()
    {
        Dimension3ConsoleVisualSetup.ValidateCurrentBatch();
        Dimension3Block7BValidation.ValidateBlock7B();
        Dimension3Block7DValidation.ValidateBlock7D();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3ConsoleVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3ConsoleVisualSetup.ValidateCurrentBatch();
        Debug.Log("[D3 Console Full Validation] PASS | composición/táctil | políticas/reservas/autorizaciones | Main | layout móvil");
    }

    public static void ValidateAllBatch() => ValidateAll();
}
#endif
