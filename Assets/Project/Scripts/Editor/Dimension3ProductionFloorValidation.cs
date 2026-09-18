#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


public static class Dimension3ProductionFloorValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Production Floor Full")]
    public static void ValidateAll()
    {
        Dimension3ProductionFloorVisualSetup.ValidateCurrentBatch();
        Dimension3Block7FullValidation.ValidateFullThrough7D();
        QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
        Dimension3ProductionFloorVisualSetup.ValidateCurrentBatch();
        MobileQaFriendlyLayoutValidation.Validate();
        Dimension3ProductionFloorVisualSetup.ValidateCurrentBatch();
        Debug.Log("[D3 Production Floor Full Validation] PASS | visual/táctil/español de Planta | D3 1-7D | Main | layout móvil");
    }

    public static void ValidateAllBatch()
    {
        ValidateAll();
    }
}
#endif
