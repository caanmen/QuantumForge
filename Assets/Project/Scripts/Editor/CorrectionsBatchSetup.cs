#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public static class CorrectionsBatchSetup
{
    public static void ConfigureAndValidateBatch()
    {
        try
        {
            VerticalGenerationVisualPolish.ApplyVisualPolish();
            GenerationLabQuarantineSetup.Apply();
            GenerationLabQuarantineSetup.Validate();

            MachineMonolith2DSetup.Configure();
            MachineFusionPanelVisualSetup.Configure();
            MachineMonolith2DSetup.Validate();

            PrestigeDimensionTransitionSetup.Configure();
            PrestigeDimensionTransitionValidation.Validate();

            AssetDatabase.SaveAssets();
            Debug.Log("[Corrections Batch Setup] PASS | Generacion | Mezclas | " +
                "Monolito 9/8/8 | transicion de prestigio");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }
}
#endif
