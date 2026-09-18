#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3ResearchValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Research Full")]
    public static void ValidateAll()
    {
        Dimension3ResearchVisualSetup.ValidateCurrentBatch();
        Dimension3Block4Validation.ValidateBlock4Core();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3ResearchVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3ResearchVisualSetup.ValidateCurrentBatch();
        Debug.Log("[D3 Research Full Validation] PASS | composición/táctil | investigación V4-V6 | Main | layout móvil");
    }

    public static void ValidateAllBatch() => ValidateAll();
}
#endif
