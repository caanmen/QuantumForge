#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class SolMediumBlockValidation
{
    [MenuItem("Quantum Forge/Validation/SOL Medium Full Block")]
    public static void Run()
    {
        F2ProgressionMigrationValidation.Validate();
        TriangleRedesignValidation.Validate();
        MobileButtonLegibilityValidation.Validate();
        BuildingSceneReloadValidation.Validate();
        AndroidPauseResumeValidation.Validate();
        AndroidOfflineResumeBudgetValidation.Validate();
        QaGlobalAccelerationValidation.ValidateGlobalAcceleration();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        SpanishReleaseReadinessValidation.Validate();
        Dimension1DarkThemeValidation.Validate();
        Debug.Log("[SolMediumBlockValidation] PASS · progresión · migración · producción · guardado · Android · QA · tema oscuro.");
    }
}
#endif
