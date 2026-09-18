#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class Dimension1ScreenCorrections20260822
{
    [MenuItem("Quantum Forge/Dimension 1/Apply Screen Corrections 2026-08-22")]
    public static void ApplyAll()
    {
        Dimension1GalaxyPremiumSetup.ConfigureReferenceV11();
        Dimension1ExploreReferenceSetup.Install();
        Dimension1AncientOrbitsSetup.Install();
        Dimension1SectorDetailsSetup.Install();
        Dimension1ExpeditionRecordSetup.Install();
        Dimension1SharedShellApply.Apply();

        Dimension1ExploreReferenceSetup.Validate();
        Dimension1AncientOrbitsSetup.Validate();
        Dimension1SectorDetailsSetup.Validate();
        Dimension1ExpeditionRecordSetup.Validate();

        AssetDatabase.SaveAssets();
        Debug.Log(
            "[D1 Screen Corrections 2026-08-22] APPLY_PASS | " +
            "órbitas individuales | detalles visibles | planetas sin destinos duplicados | " +
            "registro legible | encabezado y estrellas sincronizados");
    }
}
#endif
