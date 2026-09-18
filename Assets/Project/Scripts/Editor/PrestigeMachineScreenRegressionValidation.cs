#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public static class PrestigeMachineScreenRegressionValidation
{
    public static void RunBatch()
    {
        try
        {
            PrestigeMachineScreenValidation.Validate();
            PrestigeDimensionTransitionValidation.Validate();
            Prestige1ProgressionValidation.ValidatePrestige1Discovery();
            QaMainSceneIntegrityValidation.ValidateMainSceneIntegrity();
            VerticalUiFinalValidation.Validate();
            Debug.Log("[Prestige Machine Regression] PASS | screen + transition + six discovery orders + scene integrity + vertical UI");
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
