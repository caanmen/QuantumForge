#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ConvergenceFullValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Full C1-C6")]
    public static void ValidateFull()
    {
        ConvergenceStateValidation.ValidateMinimumState();
        ConvergenceSynchronizationValidation.ValidateSynchronizationState();
        ConvergenceTelemetryValidation.ValidateTelemetry();
        ConvergenceBoardResolverValidation.ValidateBoardResolver();
        ConvergenceStartupPulseValidation.ValidateStartupPulse();
        ConvergenceC1ToC3Validation.Validate();
        ConvergenceC4C5Validation.Validate();
        ConvergenceEndpointValidation.ValidateEndpoint();
        ConvergenceMatrixValidation.ValidateMandatoryMatrix();
        ConvergenceAuditCorrectionValidation.ValidateCorrections();
        DimensionCompletionValidation.ValidateDimensionCompletion();
        DimensionDiscoveryMigrationValidation.ValidateDiscoveryMigration();
        MachineOperationalBoundaryValidation.ValidateMachineOperationalBoundary();
        Prestige1ProgressionValidation.ValidatePrestige1Discovery();
        Dimension1FinalValidation.ValidateFinalSaveCompatibility();
        D1TreePointsValidation.ValidateD1TreePoints();
        PrestigeSignatureUIValidation.ValidatePrestigeSignaturesUI();
        Dimension3Block7FullValidation.ValidateFullThrough7D();
        EditorSceneManager.OpenScene("Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        Dimension2Block1UISetup.ValidateBlock1();
        Debug.Log("[Convergence Full] COMPLETE | revisar PASS/FAIL anteriores");
    }
    public static void ValidateFullBatch() { ValidateFull(); }
}
#endif
