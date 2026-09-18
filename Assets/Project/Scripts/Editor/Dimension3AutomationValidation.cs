#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3AutomationValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Automation Full")]
    public static void ValidateAll()
    {
        Dimension3AutomationVisualSetup.ValidateCurrentBatch();
        Dimension3Block6FacilitiesValidation.ValidateBlock6Facilities();
        Dimension3Block6AutomationValidation.ValidateBlock6Automation();
        Dimension3Block6OfflineValidation.ValidateBlock6Offline();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3AutomationVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3AutomationVisualSetup.ValidateCurrentBatch();
        Debug.Log("[D3 Automation Full Validation] PASS | composición/táctil | rutina online/perfil/reservas/parada | Main | layout móvil");
    }

    public static void ValidateAllBatch() => ValidateAll();
}
#endif
