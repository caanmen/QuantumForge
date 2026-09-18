#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3FacilitiesValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Facilities Nucleus Full")]
    public static void ValidateAll()
    {
        Dimension3FacilitiesVisualSetup.ValidateCurrentBatch();
        Dimension3Block5Validation.ValidateBlock5Core();
        Dimension3Block6FacilitiesValidation.ValidateBlock6Facilities();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3FacilitiesVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3FacilitiesVisualSetup.ValidateCurrentBatch();
        Debug.Log("[D3 Facilities Full Validation] PASS | composición/táctil | instalaciones N1-N5 | Main | layout móvil");
    }

    public static void ValidateAllBatch() => ValidateAll();
}
#endif
