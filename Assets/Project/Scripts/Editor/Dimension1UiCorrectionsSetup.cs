#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class Dimension1UiCorrectionsSetup
{
    [MenuItem("Quantum Forge/Dimension 1/Apply 2026-08-21 UI Corrections")]
    public static void ApplyAll()
    {
        Dimension1CommandCenterSetup.Install();
        Dimension1GalaxyPremiumSetup.ConfigureReferenceV11();
        Dimension1ExploreReferenceSetup.Install();
        Dimension1HangarReferenceSetup.Install();
        Dimension1AncientOrbitsSetup.Install();
        Dimension1SectorDetailsSetup.Install();
        Dimension1ExpeditionResultSetup.Install();

        Dimension1CommandCenterSetup.Validate();
        Dimension1ExploreReferenceSetup.Validate();
        Dimension1HangarReferenceSetup.Validate();
        Dimension1AncientOrbitsSetup.Validate();
        Dimension1SectorDetailsSetup.Validate();
        Dimension1ExpeditionResultSetup.Validate();

        AssetDatabase.SaveAssets();
        Debug.Log(
            "[D1 UI Corrections 2026-08-21] APPLY_PASS | " +
            "Centro, Carta, Explorar, Hangar, sectores y resultado sincronizados"
        );
    }
}
#endif
