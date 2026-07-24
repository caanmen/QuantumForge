#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PrestigeSignatureUIValidation
{
    [MenuItem("Tools/Quantum Forge/Convergence/Validate Prestige Signatures UI")]
    public static void ValidatePrestigeSignaturesUI()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Prestige Signatures UI] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        Check(PrestigeUI.GetDimensionSignatureName(1) == "Firma Dimensional Alfa",
            "D1 no conserva la Firma Alfa.", failures);
        Check(PrestigeUI.GetDimensionSignatureName(2) == "Firma Dimensional Beta",
            "D2 no conserva la Firma Beta.", failures);
        Check(PrestigeUI.GetDimensionSignatureName(3) == "Firma Dimensional Gamma",
            "D3 no conserva la Firma Gamma.", failures);
        Check(PrestigeUI.GetDimensionSelectionLabel(2, true) ==
              "Firma Dimensional Beta",
            "Una firma disponible expone o etiqueta incorrectamente su dimensión.",
            failures);
        Check(PrestigeUI.GetDimensionSelectionLabel(3, false) ==
              "Firma Dimensional Gamma — ya sintonizada",
            "Una firma ya elegida no conserva su identidad de firma.", failures);

        if (failures.Count == 0)
        {
            Debug.Log("[Prestige Signatures UI] PASS | Alfa | Beta | Gamma | estados");
            return;
        }

        Debug.LogError("[Prestige Signatures UI] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidatePrestigeSignaturesUIBatch()
    {
        ValidatePrestigeSignaturesUI();
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif
