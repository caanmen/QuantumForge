#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class QaBlock1Validation
{
    private const string AvailabilityOverrideField =
        "availabilityOverrideForValidation";

    [MenuItem("Tools/Quantum Forge/QA/Validate Block 1")]
    public static void ValidateBlock1()
    {
        var failures = new List<string>();
        float initialTimeScale = Time.timeScale;
        FieldInfo availabilityOverride = typeof(QaRuntimeService).GetField(
            AvailabilityOverrideField,
            BindingFlags.NonPublic | BindingFlags.Static);

        try
        {
            Check(availabilityOverride != null,
                "No se encontró el control privado de disponibilidad.", failures);
            if (availabilityOverride == null)
                Finish(failures);

            availabilityOverride.SetValue(null, true);
            QaRuntimeService.ResetToNormalSpeed();

            ValidateAllowedSpeeds(failures);
            ValidateCycle(failures);
            ValidateInvalidSpeeds(failures);
            ValidateUnavailableMode(availabilityOverride, failures);
            ValidateInvalidOnlineSeconds(failures);

            Check(QaRuntimeService.WasAccelerationUsedThisSession,
                "No se registró el uso de aceleración durante la sesión.", failures);
            Check(Time.timeScale == initialTimeScale && Time.timeScale == 1f,
                "El servicio alteró Time.timeScale.", failures);
        }
        finally
        {
            if (availabilityOverride != null)
                availabilityOverride.SetValue(null, null);
            QaRuntimeService.ResetToNormalSpeed();
        }

        Finish(failures);
    }

    private static void ValidateAllowedSpeeds(List<string> failures)
    {
        ValidateScale(1f, 10.0, failures);
        ValidateScale(5f, 50.0, failures);
        ValidateScale(10f, 100.0, failures);
        ValidateScale(20f, 200.0, failures);
    }

    private static void ValidateScale(
        float multiplier, double expected, List<string> failures)
    {
        bool accepted = QaRuntimeService.TrySetSpeed(multiplier);
        double actual = QaRuntimeService.ScaleOnlineSeconds(10.0);
        Check(accepted && QaRuntimeService.SimulationMultiplier == multiplier &&
            Math.Abs(actual - expected) < 0.000001,
            "x" + multiplier.ToString("0") +
            " no escala 10 s a " + expected.ToString("0") + " s.", failures);
    }

    private static void ValidateCycle(List<string> failures)
    {
        QaRuntimeService.ResetToNormalSpeed();
        float[] expected = { 5f, 10f, 20f, 1f };
        for (int index = 0; index < expected.Length; index++)
        {
            float actual = QaRuntimeService.CycleSpeed();
            Check(actual == expected[index] &&
                QaRuntimeService.SimulationMultiplier == expected[index],
                "El ciclo de velocidad no respeta x1/x5/x10/x20/x1.", failures);
        }
    }

    private static void ValidateInvalidSpeeds(List<string> failures)
    {
        float[] invalid =
        {
            float.NaN,
            float.PositiveInfinity,
            float.NegativeInfinity,
            0f,
            -1f,
            2f
        };

        foreach (float value in invalid)
        {
            QaRuntimeService.TrySetSpeed(20f);
            bool accepted = QaRuntimeService.TrySetSpeed(value);
            Check(!accepted && QaRuntimeService.SimulationMultiplier == 1f &&
                !QaRuntimeService.IsAccelerated,
                "Una velocidad inválida fue aceptada o no dejó x1.", failures);
        }
    }

    private static void ValidateUnavailableMode(
        FieldInfo availabilityOverride, List<string> failures)
    {
        QaRuntimeService.TrySetSpeed(20f);
        availabilityOverride.SetValue(null, false);

        Check(!QaRuntimeService.IsAvailable &&
            QaRuntimeService.SimulationMultiplier == 1f &&
            !QaRuntimeService.IsAccelerated &&
            Math.Abs(QaRuntimeService.ScaleOnlineSeconds(10.0) - 10.0) <
                0.000001,
            "El modo no disponible no fuerza velocidad x1.", failures);
        Check(!QaRuntimeService.TrySetSpeed(5f) &&
            QaRuntimeService.SimulationMultiplier == 1f,
            "El modo no disponible permitió activar QA.", failures);

        availabilityOverride.SetValue(null, true);
    }

    private static void ValidateInvalidOnlineSeconds(List<string> failures)
    {
        double[] invalid =
        {
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity,
            -0.001
        };

        foreach (double value in invalid)
        {
            bool rejected = false;
            try
            {
                QaRuntimeService.ScaleOnlineSeconds(value);
            }
            catch (ArgumentOutOfRangeException)
            {
                rejected = true;
            }

            Check(rejected, "Se aceptaron segundos online inválidos.", failures);
        }
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[QA Block 1] PASS | autoridad única | x1/x5/x10/x20 | " +
                "inválidos a x1 | build pública a x1 | Time.timeScale intacto");
            return;
        }

        Debug.LogError("[QA Block 1] FAIL\n- " +
            string.Join("\n- ", failures));
        throw new InvalidOperationException(
            "El Bloque 1 no superó su validación.");
    }

    private static void Check(
        bool condition, string failure, List<string> failures)
    {
        if (!condition)
            failures.Add(failure);
    }
}
#endif
