#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class Dimension3Block7FullValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Full Through 7D")]
    public static void ValidateFullThrough7D()
    {
        Run();
    }

    public static void ValidateFullThrough7C()
    {
        Run();
    }

    // Alias conservado para comandos batch y accesos anteriores.
    public static void ValidateFullThrough7A()
    {
        Run();
    }

    private static void Run()
    {
        Dimension3Block1Validation.ValidateBlock1Core();
        Dimension3Block2Validation.ValidateBlock2Core();
        Dimension3Block3Validation.ValidateBlock3Core();
        Dimension3Block4Validation.ValidateBlock4Core();
        Dimension3Block5Validation.ValidateBlock5Core();
        Dimension3Block6CatalogValidation.ValidateBlock6Catalog();
        Dimension3Block6FacilitiesValidation.ValidateBlock6Facilities();
        Dimension3Block6AutomationValidation.ValidateBlock6Automation();
        Dimension3Block6OfflineValidation.ValidateBlock6Offline();
        Dimension3Block7AValidation.ValidateBlock7A();
        Dimension3Block7BValidation.ValidateBlock7B();
        Dimension3Block7CValidation.ValidateBlock7C();
        Dimension3Block7DValidation.ValidateBlock7D();
        Debug.Log("[D3 Full Through 7D] FIN | todas las líneas anteriores deben indicar PASS");
    }
}
#endif
