#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Reconstruye de forma repetible las pantallas afectadas por la segunda tanda
/// de reparaciones, siempre desde sus herramientas canonicas.
/// </summary>
public static class TandaReparaciones2Setup
{
    [MenuItem("Tools/Quantum Forge/QA/Apply Tanda Reparaciones 2")]
    public static void ApplyAndValidateBatch()
    {
        try
        {
            VerticalUiBlock5Setup.ConfigureBlock5Upgrades();
            UpgradeStudyObservatorySetup.ConfigureObservatory();
            VerticalUpgradesVisualPolish.ApplyVisualPolish();
            VerticalGenerationVisualPolish.ApplyVisualPolish();
            // El pulido base usa la geometria anterior. Se valida antes de que el
            // laboratorio vigente restablezca sus alturas canonicas.
            VerticalGenerationVisualPolishValidation.Validate();
            GenerationLabQuarantineSetup.Apply();
            MachineFusionPanelVisualSetup.ConfigureBatch();
            MachineMonolith2DSetup.ConfigureBatch();

            VerticalUiBlock5Validation.Validate();
            VerticalUpgradesVisualPolishValidation.Validate();
            GenerationLabQuarantineSetup.Validate();
            TriangleRedesignValidation.Validate();
            UpgradeStudyValidation.ValidateLogic();
            GameplayCorrectionsValidation.Validate();

            Debug.Log("[Tanda Reparaciones 2] APPLY PASS | Generacion | " +
                "Mejoras fijas | Mezclas limpias | Monolito y caras");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    public static void ValidateOnlyBatch()
    {
        try
        {
            VerticalUiBlock5Validation.Validate();
            VerticalUpgradesVisualPolishValidation.Validate();
            GenerationLabQuarantineSetup.Validate();
            TriangleRedesignValidation.Validate();
            UpgradeStudyValidation.ValidateLogic();
            GameplayCorrectionsValidation.Validate();
            Debug.Log("[Tanda Reparaciones 2] VALIDATION PASS");
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
