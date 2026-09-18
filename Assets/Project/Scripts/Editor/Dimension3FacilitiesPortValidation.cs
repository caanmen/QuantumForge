#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3FacilitiesPortValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Facilities Port Full")]
    public static void ValidateAll()
    {
        Dimension3FacilitiesVisualSetup.ValidateCurrentBatch();
        Dimension3Block5Validation.ValidateBlock5Core();
        Dimension3Block6FacilitiesValidation.ValidateBlock6Facilities();
        Dimension3Block6AutomationValidation.ValidateBlock6Automation();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3FacilitiesVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3FacilitiesVisualSetup.ValidateCurrentBatch();
        Debug.Log("[D3 Facilities Port Full Validation] PASS | variante Puerto N1 | enlace D1 | asignación/estabilización | instalaciones/automatización | Main | layout móvil");
    }

    public static void ValidateAllBatch() => ValidateAll();
}
#endif
