#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class Dimension3Block6FullValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Full Block 6")]
    public static void ValidateFullBlock6()
    {
        Dimension3Block1Validation.ValidateBlock1Core();
        Dimension3Block6CatalogValidation.ValidateBlock6Catalog();
        Dimension3Block6FacilitiesValidation.ValidateBlock6Facilities();
        Dimension3Block6AutomationValidation.ValidateBlock6Automation();
        Dimension3Block6OfflineValidation.ValidateBlock6Offline();
        Debug.Log("[D3 Block 6 Full] FIN | revisar que todas las lineas anteriores sean PASS");
    }
}
#endif
