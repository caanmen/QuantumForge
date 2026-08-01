#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class QaGlobalAccelerationValidation
{
    [MenuItem("Tools/Quantum Forge/QA/Validate Global Acceleration")]
    public static void ValidateGlobalAcceleration()
    {
        var failures = new List<string>();
        string fixtureRoot = Path.Combine("Library", "QaGlobalAccelerationValidation");

        try
        {
            ValidateRetiredResourcesAndMigration(failures);
            ValidateCurrentAndLegacySaveFiles(fixtureRoot, failures);

            QaBlock0Validation.ValidateBlock0();
            QaBlock1Validation.ValidateBlock1();
            QaBlock2Validation.ValidateBlock2();
            QaBlock3Validation.ValidateBlock3();
            QaBlock4Validation.ValidateBlock4();
            QaBlock5Validation.ValidateBlock5();
        }
        finally
        {
            if (Directory.Exists(fixtureRoot))
                Directory.Delete(fixtureRoot, true);
            QaRuntimeService.ResetToNormalSpeed();
        }

        if (failures.Count > 0)
        {
            Debug.LogError("[QA Global Acceleration] FAIL\n- " +
                string.Join("\n- ", failures));
            throw new InvalidOperationException(
                "La validacion global de aceleracion QA no fue superada.");
        }

        Debug.Log("[QA Global Acceleration] PASS | recursos retirados | " +
            "migracion legacy | saves actual/antiguo | x1/x5/x10/x20 | " +
            "invalidos rechazados | escala unica D1/D2/D3 | Maquina/Cuarto 2 | " +
            "UI y offline reales | avance exacto sin doble escala | " +
            "checkpoints validos/corruptos/pre_restore | QA publica oculta");
    }

    public static void ValidateGlobalAccelerationBatch()
    {
        try
        {
            ValidateGlobalAcceleration();
            Debug.Log("[QA Global Acceleration Batch] PASS");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ValidateRetiredResourcesAndMigration(
        List<string> failures)
    {
        GameObject target = new GameObject("QA Global Legacy Migration")
            { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        GameState state = target.AddComponent<GameState>();
        try
        {
            state.LE = 4321.0;
            state.Traces = 123.0;
            state.EM = 10.0;
            state.emMult = 11.0;
            state.ADP = 12.0;
            state.WHF = 13.0;
            state.totalADPGenerada = 14.0;
            state.totalWHFGenerada = 15.0;

            var legacy = new SaveData
            {
                removedLegacyResourcesVersion = 0,
                LE = state.LE,
                Traces = state.Traces,
                EM = state.EM,
                emMult = state.emMult,
                ADP = state.ADP,
                WHF = state.WHF,
                totalADPGenerada = state.totalADPGenerada,
                totalWHFGenerada = state.totalWHFGenerada
            };

            SaveService.ApplyRemovedLegacyResourcesMigration(legacy, state);
            Check(legacy.removedLegacyResourcesVersion ==
                    SaveService.RemovedLegacyResourcesVersion &&
                IsZero(legacy.EM) && IsZero(legacy.emMult) &&
                IsZero(legacy.ADP) && IsZero(legacy.WHF) &&
                IsZero(legacy.totalADPGenerada) &&
                IsZero(legacy.totalWHFGenerada) &&
                IsZero(state.EM) && IsZero(state.emMult) &&
                IsZero(state.ADP) && IsZero(state.WHF) &&
                IsZero(state.totalADPGenerada) &&
                IsZero(state.totalWHFGenerada),
                "La migracion no neutraliza todos los recursos retirados.",
                failures);
            Check(Math.Abs(state.LE - 4321.0) < 0.000001 &&
                Math.Abs(state.Traces - 123.0) < 0.000001,
                "La migracion de recursos retirados altera LE o Trazas.",
                failures);

            string research = File.ReadAllText(
                "Assets/Project/Resources/Data/research.json");
            string achievements = File.ReadAllText(
                "Assets/Project/Resources/Data/achievements.json");
            Check(research.Contains("\"research\": []") &&
                !achievements.Contains("ach_first_em") &&
                !achievements.Contains("ReachEM"),
                "El contenido retirado conserva entradas activas.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    private static void ValidateCurrentAndLegacySaveFiles(
        string fixtureRoot, List<string> failures)
    {
        if (Directory.Exists(fixtureRoot))
            Directory.Delete(fixtureRoot, true);
        Directory.CreateDirectory(fixtureRoot);

        string currentPath = Path.Combine(fixtureRoot, "current.json");
        var current = new SaveData
        {
            LE = 55.0,
            Traces = 8.0,
            removedLegacyResourcesVersion =
                SaveService.RemovedLegacyResourcesVersion,
            lastUnix = 123456L,
            dimension2 = Dimension2System.CreateInitialState(),
            dimension3 = Dimension3System.CreateInitialState(),
            convergence = ConvergenceSystem.CreateInitialState()
        };
        bool currentWritten = SaveService.TryWriteAtomicJson(
            currentPath, JsonUtility.ToJson(current, true), out string currentError);
        bool currentRead = SaveService.TryReadSaveData(
            currentPath, out SaveData loadedCurrent);
        Check(currentWritten && currentRead && loadedCurrent != null &&
            Math.Abs(loadedCurrent.LE - 55.0) < 0.000001 &&
            loadedCurrent.dimension2 != null && loadedCurrent.dimension3 != null &&
            loadedCurrent.convergence != null,
            "El save actual no se escribe/carga: " + currentError, failures);

        string legacyPath = Path.Combine(fixtureRoot, "legacy.json");
        File.WriteAllText(legacyPath,
            "{\"LE\":77.0,\"Traces\":9.0,\"EM\":5.0," +
            "\"emMult\":6.0,\"ADP\":7.0,\"WHF\":8.0,\"lastUnix\":42}");
        bool legacyRead = SaveService.TryReadSaveData(
            legacyPath, out SaveData loadedLegacy);
        Check(legacyRead && loadedLegacy != null &&
            Math.Abs(loadedLegacy.LE - 77.0) < 0.000001 &&
            Math.Abs(loadedLegacy.EM - 5.0) < 0.000001,
            "El formato de save antiguo ya no puede leerse.", failures);

        GameObject target = new GameObject("QA Global Old Save")
            { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        GameState state = target.AddComponent<GameState>();
        try
        {
            SaveService.ApplyRemovedLegacyResourcesMigration(loadedLegacy, state);
            Check(IsZero(loadedLegacy.EM) && IsZero(loadedLegacy.emMult) &&
                IsZero(loadedLegacy.ADP) && IsZero(loadedLegacy.WHF),
                "El save antiguo se lee pero no completa su migracion.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(target);
        }
    }

    private static bool IsZero(double value)
    {
        return Math.Abs(value) < 0.000001;
    }

    private static void Check(
        bool condition, string failure, List<string> failures)
    {
        if (!condition)
            failures.Add(failure);
    }
}
#endif
