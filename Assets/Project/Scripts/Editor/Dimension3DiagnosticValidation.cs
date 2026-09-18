#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3DiagnosticValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Diagnostic Full")]
    public static void ValidateAll()
    {
        Dimension3DiagnosticVisualSetup.ValidateCurrentBatch();
        Dimension3Block7CValidation.ValidateBlock7C();
        Dimension3Block7DValidation.ValidateBlock7D();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3DiagnosticVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3DiagnosticVisualSetup.ValidateCurrentBatch();
        Debug.Log("[D3 Diagnostic Full Validation] PASS | composición/táctil | ajustes/recetas/rutinas N3-N5 | Main | layout móvil");
    }

    public static void ValidateAllBatch() => ValidateAll();
}
#endif
