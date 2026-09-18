#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class Dimension1SelectedRedesignApply
{
    [MenuItem("Quantum Forge/Dimension 1/Apply Selected Explore Command Galaxy Redesign")]
    public static void Apply()
    {
        Dimension1CommandCenterSetup.Install();
        Dimension1GalaxyPremiumSetup.ConfigureReferenceV11();
        Dimension1ExploreReferenceSetup.Install();
        Dimension1MetalsInventorySetup.Install();
        AssetDatabase.SaveAssets();
        Debug.Log("[D1 Selected Redesign] APPLY_PASS | Centro de Mando | Carta Galáctica | Explorar | Metales");
    }
}
#endif
