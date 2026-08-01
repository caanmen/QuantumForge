#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public static class QaBlock0Validation
{
    [MenuItem("Tools/Quantum Forge/QA/Validate Block 0")]
    public static void ValidateBlock0()
    {
        var failures = new List<string>();
        GameState previous = GameState.I;
        try
        {
            ValidateLegacySaveMigration(failures);
            ValidateOnlineAndOfflineNeutralization(failures);
            ValidateRetiredContent(failures);
        }
        finally
        {
            SetGameStateSingleton(previous);
        }

        if (failures.Count == 0)
        {
            Debug.Log("[QA Block 0] PASS | migración heredada | EM/ADP/WHF en cero | " +
                "LE sin emMult | offline neutral | HUD/edificios | investigación/logros");
            return;
        }

        Debug.LogError("[QA Block 0] FAIL\n- " + string.Join("\n- ", failures));
    }

    public static void ValidateBlock0AndFullRegressionsBatch()
    {
        PresentationBlockP7Validation.ValidateP7AndFullRegressionsBatch();
        ValidateBlock0();
    }

    private static void ValidateLegacySaveMigration(List<string> failures)
    {
        GameState state = CreateState("QA Block 0 Save Migration");
        try
        {
            state.LE = 12345.0;
            state.Traces = 678.0;
            state.EM = 91.0;
            state.emMult = 4.0;
            state.ADP = 92.0;
            state.WHF = 93.0;
            state.totalADPGenerada = 94.0;
            state.totalWHFGenerada = 95.0;
            state.researchGlobalLEMult = 8.0;

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
                totalWHFGenerada = state.totalWHFGenerada,
                dimension2 = Dimension2System.CreateInitialState(),
                dimension3 = Dimension3System.CreateInitialState(),
                convergence = ConvergenceSystem.CreateInitialState()
            };

            SaveService.ApplyRemovedLegacyResourcesMigration(legacy, state);
            CheckLegacyZero(legacy, state, failures);
            Check(AreClose(state.LE, 12345.0) && AreClose(state.Traces, 678.0),
                "La migración altera LE o Trazas.", failures);
            Check(legacy.dimension2 != null && legacy.dimension3 != null &&
                legacy.convergence != null,
                "La migración elimina Dimensión 2, Dimensión 3 o Convergencia.", failures);

            string json = JsonUtility.ToJson(legacy);
            SaveData roundTrip = JsonUtility.FromJson<SaveData>(json);
            Check(roundTrip != null &&
                roundTrip.removedLegacyResourcesVersion ==
                    SaveService.RemovedLegacyResourcesVersion &&
                AreClose(roundTrip.EM, 0.0) && AreClose(roundTrip.emMult, 0.0) &&
                AreClose(roundTrip.ADP, 0.0) && AreClose(roundTrip.WHF, 0.0) &&
                AreClose(roundTrip.totalADPGenerada, 0.0) &&
                AreClose(roundTrip.totalWHFGenerada, 0.0),
                "Guardar y recargar no conserva la neutralización/versionado.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateOnlineAndOfflineNeutralization(List<string> failures)
    {
        GameState state = CreateState("QA Block 0 Runtime");
        try
        {
            state.dimension2 = Dimension2System.CreateInitialState();
            state.dimension3 = Dimension3System.CreateInitialState();
            state.convergence = ConvergenceSystem.CreateInitialState();
            state.baseLEps = 1.0;
            state.LE = 100.0;
            state.emMult = 99.0;
            double withLegacyMultiplier = state.GetTotalLEps();
            state.emMult = 0.0;
            double withoutLegacyMultiplier = state.GetTotalLEps();
            Check(AreClose(withLegacyMultiplier, withoutLegacyMultiplier),
                "emMult todavía modifica CalculateTotalLEps.", failures);

            state.EM = 10.0;
            state.emMult = 2.0;
            state.ADP = 20.0;
            state.WHF = 30.0;
            state.totalADPGenerada = 40.0;
            state.totalWHFGenerada = 50.0;
            double leBefore = state.LE;
            state.Tick(3600.0);
            CheckRuntimeZero(state, "GameState.Tick(3600)", failures);
            Check(state.LE > leBefore, "Neutralizar recursos detiene la producción activa de LE.",
                failures);

            state.EM = 10.0;
            state.emMult = 2.0;
            state.ADP = 20.0;
            state.WHF = 30.0;
            state.totalADPGenerada = 40.0;
            state.totalWHFGenerada = 50.0;
            state.ApplyOfflineBaseProgress(3600.0);
            CheckRuntimeZero(state, "ApplyOfflineBaseProgress(3600)", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateRetiredContent(List<string> failures)
    {
        TextAsset researchAsset = Resources.Load<TextAsset>("Data/research");
        ResearchDefList research = researchAsset == null
            ? null : JsonUtility.FromJson<ResearchDefList>(researchAsset.text);
        Check(research != null && research.research != null &&
            research.research.Count == 0,
            "research.json no contiene una lista activa vacía.", failures);

        TextAsset achievementsAsset = Resources.Load<TextAsset>("Data/achievements");
        string achievements = achievementsAsset == null ? "" : achievementsAsset.text;
        Check(!achievements.Contains("ach_first_em") &&
            !achievements.Contains("ach_first_research") &&
            !achievements.Contains("ach_research_6") &&
            !achievements.Contains("ReachEM") &&
            !achievements.Contains("PurchasedResearchCount"),
            "achievements.json conserva metas retiradas de EM/investigaciones.", failures);

        MonoScript hud = AssetDatabase.LoadAssetAtPath<MonoScript>(
            "Assets/Project/Scripts/UI/HUD.cs");
        MonoScript row = AssetDatabase.LoadAssetAtPath<MonoScript>(
            "Assets/Project/Scripts/Buildings/BuildingRowUI.cs");
        Check(hud != null && !hud.text.Contains("emText.SetText") &&
            !hud.text.Contains("adpText.SetText") &&
            !hud.text.Contains("whfText.SetText"),
            "HUD todavía actualiza EM, ADP o WHF.", failures);
        Check(row != null && !row.text.Contains("emTick") &&
            !row.text.Contains("gameState.emMult") &&
            !row.text.Contains(" EM\"") ,
            "BuildingRowUI todavía calcula o muestra EM.", failures);

        TextAsset es = Resources.Load<TextAsset>("Localization/lang_es");
        TextAsset en = Resources.Load<TextAsset>("Localization/lang_en");
        ValidateActiveMetaCopy(es == null ? "" : es.text, "ES", failures);
        ValidateActiveMetaCopy(en == null ? "" : en.text, "EN", failures);
    }

    private static void ValidateActiveMetaCopy(
        string json, string language, List<string> failures)
    {
        string[] keys = { "meta.warn_nogain", "meta.warn_reset", "meta.warning" };
        foreach (string key in keys)
        {
            int keyIndex = json.IndexOf("\"" + key + "\"", StringComparison.Ordinal);
            int lineEnd = keyIndex < 0 ? -1 : json.IndexOf('\n', keyIndex);
            string line = keyIndex < 0 ? "" : json.Substring(keyIndex,
                (lineEnd < 0 ? json.Length : lineEnd) - keyIndex);
            Check(keyIndex >= 0 && !ContainsRetiredToken(line),
                "Texto activo " + key + " conserva recursos retirados (" + language + ").",
                failures);
        }
    }

    private static bool ContainsRetiredToken(string value)
    {
        return value.Contains("EM") || value.Contains("ADP") || value.Contains("WHF");
    }

    private static void CheckLegacyZero(
        SaveData data, GameState state, List<string> failures)
    {
        Check(data.removedLegacyResourcesVersion ==
                SaveService.RemovedLegacyResourcesVersion &&
            AreClose(data.EM, 0.0) && AreClose(data.emMult, 0.0) &&
            AreClose(data.ADP, 0.0) && AreClose(data.WHF, 0.0) &&
            AreClose(data.totalADPGenerada, 0.0) &&
            AreClose(data.totalWHFGenerada, 0.0),
            "SaveData heredado no migra todos los recursos retirados.", failures);
        CheckRuntimeZero(state, "migración de GameState", failures);
        Check(AreClose(state.researchGlobalLEMult, 1.0),
            "La migración no restablece researchGlobalLEMult a 1.", failures);
    }

    private static void CheckRuntimeZero(
        GameState state, string context, List<string> failures)
    {
        Check(AreClose(state.EM, 0.0) && AreClose(state.emMult, 0.0) &&
            AreClose(state.ADP, 0.0) && AreClose(state.WHF, 0.0) &&
            AreClose(state.totalADPGenerada, 0.0) &&
            AreClose(state.totalWHFGenerada, 0.0),
            context + " deja algún recurso retirado distinto de cero.", failures);
    }

    private static GameState CreateState(string name)
    {
        var target = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        target.SetActive(false);
        return target.AddComponent<GameState>();
    }

    private static void SetGameStateSingleton(GameState state)
    {
        PropertyInfo property = typeof(GameState).GetProperty(
            "I", BindingFlags.Public | BindingFlags.Static);
        property?.SetValue(null, state, null);
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.000001;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif
