#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3CalibrationValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Calibration Full")]
    public static void ValidateAll()
    {
        Dimension3CalibrationVisualSetup.ValidateCurrentBatch();
        Dimension3Block3Validation.ValidateBlock3Core();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3CalibrationVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3CalibrationVisualSetup.ValidateCurrentBatch();
        Debug.Log("[D3 Calibration Full Validation] PASS | composición/táctil | sistema de calibración | Main | layout móvil");
    }

    public static void ValidateAllBatch()
    {
        ValidateAll();
    }
}
#endif
