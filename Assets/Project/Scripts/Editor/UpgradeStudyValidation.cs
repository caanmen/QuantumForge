#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class UpgradeStudyValidation
{
    [MenuItem("Tools/Quantum Forge/Studies/Validate Logic")]
    public static void ValidateLogic()
    {
        GameObject host = new GameObject("UpgradeStudyValidationHost");
        GameState previousState = GameState.I;
        F2UpgradeManager previousManager = F2UpgradeManager.I;
        try
        {
            GameState state = host.AddComponent<GameState>();
            F2UpgradeManager manager = host.AddComponent<F2UpgradeManager>();
            SetSingleton(typeof(GameState), state);
            SetSingleton(typeof(F2UpgradeManager), manager);
            typeof(F2UpgradeManager).GetMethod("LoadDefs",
                BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(manager, null);
            SaveService.LastLoadedBuildingLevels = null;
            RegisterArtifact(state, "vacuum_observer");
            RegisterArtifact(state, "casimir_panel");
            RegisterArtifact(state, "fluctuation_antenna");
            UpgradeStudySystem.ReloadCatalog();
            UpgradeStudySystem.ResetForNewRun(state);

            CheckCatalog();
            ValidateTriangleEnergyEconomy(state);
            ValidateSingleActiveAndConclusion(state);
            ValidateIndependentProgressAndTuning(state);
            ValidateGlobalTraceResonance(state, manager);
            ValidateSynchronizationMemory(state);
            ValidateKeycardProject(state);
            ValidateStateRoundTrip(state);
            ValidateFragmentScope(state);
            ValidateLegacyMigration(state, manager);

            Debug.Log("[Upgrade Studies Validation] PASS | 9 studies | Energy cost | online/offline clock | active tuning | manual reveal | round-trip | keycard gate | no early fragment bonus");
        }
        finally
        {
            SetSingleton(typeof(GameState), previousState);
            SetSingleton(typeof(F2UpgradeManager), previousManager);
            UnityEngine.Object.DestroyImmediate(host);
        }
    }

    private static void CheckCatalog()
    {
        var expected = new Dictionary<string, double>
        {
            ["study_emission_focus"] = 90,
            ["study_containment_tuning"] = 90,
            ["study_tetraquark_stabilization"] = 180,
            ["study_triangle_unlock_1"] = 180,
            ["study_triangle_impulse_tuning"] = 240,
            ["study_triangle_synergy_resonance"] = 240,
            ["study_triangle_persistence_anchor"] = 300,
            ["study_triangle_energy_efficiency"] = 300,
            ["study_experimental_chamber_keycard"] = 360
        };
        foreach (var item in expected)
        {
            UpgradeStudyDef def = UpgradeStudySystem.GetStudy(item.Key);
            Require(def != null && Math.Abs(def.durationSeconds - item.Value) < 0.001,
                "Duración inválida para " + item.Key);
        }
    }

    private static void ValidateSingleActiveAndConclusion(GameState state)
    {
        Require(UpgradeStudySystem.TryStartStudy(state, "study_emission_focus"),
            "No inició el primer estudio de Higgs.");
        Require(!UpgradeStudySystem.TryStartStudy(state, "study_containment_tuning"),
            "Permitió dos estudios simultáneos.");
        UpgradeStudySystem.Advance(state, 89.0);
        Require(!UpgradeStudySystem.IsConclusionPending(state),
            "Concluyó antes de tiempo.");
        UpgradeStudySystem.Advance(state, 1.0);
        Require(UpgradeStudySystem.IsConclusionPending(state),
            "No creó la conclusión manual.");
        Require(UpgradeStudySystem.TryRevealConclusion(state, out string unlockId) &&
            unlockId == "emission_focus" &&
            UpgradeStudySystem.IsDiscovered(state, unlockId),
            "No reveló Emisión Calibrada.");
    }

    private static void ValidateTriangleEnergyEconomy(GameState state)
    {
        state.triangleSystemUnlocked = false;
        state.triangleActiveCircuit = TriangleCircuitType.None;
        state.triangleSynchronization = 0f;
        state.triangleSynchronizationBaseRatePerSecond = 0.0;
        state.triangleEnergy = 0.0;
        state.LE = 1000.0;
        state.Traces = 100.0;

        BuildingState generator = state.GetBuildingState("fluctuation_antenna");
        Require(generator != null && generator.level == 1,
            "El Modulador no quedó como primer nivel del Captador.");
        Require(Math.Abs(state.CalculateTriangleEnergyPerSecond() - 1.0) < 0.0001,
            "El Captador comprado no produce 1 Energía/s antes de Acople.");
        double preview = state.GetBuildingNextLevelTriangleEnergyPerSecond(generator);
        double leCost = state.GetTriangleEnergyGeneratorLECost();
        double traceCost = state.GetTriangleEnergyGeneratorTraceCost();
        double leBefore = state.LE;
        double tracesBefore = state.Traces;
        Require(BuildingPurchaseService.TryPurchase(state, generator) &&
            generator.level == 2,
            "No se pudo comprar un nivel real del Captador.");
        Require(Math.Abs(state.LE - (leBefore - leCost)) < 0.0001 &&
            Math.Abs(state.Traces - (tracesBefore - traceCost)) < 0.0001,
            "El Captador no descontó LE y Trazas de forma atómica.");
        Require(Math.Abs(preview - 0.05) < 0.0001 &&
            Math.Abs(state.CalculateTriangleEnergyPerSecond() - 1.05) < 0.0001,
            "La ganancia mostrada del Captador no coincide con la producción real.");
        Require(!state.CanUseTriangleCircuits() &&
                state.triangleActiveCircuit == TriangleCircuitType.None,
            "Producir Energía antes de Acople desbloqueó circuitos por accidente.");

        MethodInfo generateEnergy = typeof(GameState).GetMethod(
            "GenerateTriangleEnergy", BindingFlags.Instance | BindingFlags.NonPublic);
        Require(generateEnergy != null,
            "No se encontró la ruta real de generación de Energía.");
        generateEnergy.Invoke(state, new object[] { 20.0 });
        Require(Math.Abs(state.triangleEnergy - 21.0) < 0.0001,
            "El Captador nivel 2 no acumuló 21 de Energía en 20 segundos.");

        state.triangleSystemUnlocked = true;
        state.triangleActiveCircuit = TriangleCircuitType.Energy;
        state.triangleSynchronization = 1f;

        Require(state.SetTriangleCircuit(TriangleCircuitType.Phase),
            "No se pudo elegir el enfoque Energía.");
        Require(Math.Abs(state.triangleSynchronization - 0.5f) < 0.0001,
            "Cambiar de enfoque no reinició la sincronización al 50%.");
        state.triangleSynchronization = 1f;
        Require(Math.Abs(state.CalculateTriangleEnergyPerSecond() - 1.365) < 0.0001,
            "El enfoque Energía no aplica su 30% a la producción real.");

        state.triangleEnergy = 0.0;
        TriangleOfflineReport report = state.ApplyOfflineBaseProgress(10.0);
        Require(report != null && report.triangleEnergyGained > 0.0 &&
            Math.Abs(state.triangleEnergy - report.triangleEnergyGained) < 0.0001,
            "La Energía no progresa correctamente offline.");

        generator.level = 1;
        state.triangleActiveCircuit = TriangleCircuitType.None;
        state.triangleSynchronization = 0f;
        state.triangleEnergy = 0.0;
        state.LE = 0.0;
        state.Traces = 0.0;
        UpgradeStudySystem.ResetForNewRun(state);
    }

    private static void ValidateIndependentProgressAndTuning(GameState state)
    {
        state.triangleEnergy = 2000.0;
        state.triangleSystemUnlocked = true;
        state.triangleActiveCircuit = TriangleCircuitType.Energy;
        state.triangleSynchronization = 0f;
        double energyBefore = state.triangleEnergy;
        Require(UpgradeStudySystem.TryStartStudy(state,
            "study_triangle_impulse_tuning"),
            "El estudio de LE exigió sincronización previa.");
        Require(Math.Abs(state.triangleEnergy - (energyBefore - 120.0)) < 0.001,
            "El coste de Energía no se descontó exactamente una vez.");
        UpgradeStudySystem.Advance(state, 100.0);
        state.triangleActiveCircuit = TriangleCircuitType.Experimental;
        UpgradeStudySystem.Advance(state, 100.0);
        Require(Math.Abs(state.upgradeStudies.activeProgressSeconds - 200.0) < 0.001,
            "Cambiar de enfoque pausó una investigación ya iniciada.");
        UpgradeStudySystem.GetTuningTarget(state, out float amplitude, out float frequency);
        UpgradeStudySystem.SetTuningValues(state, amplitude, frequency < 0.5f ? 1f : 0f);
        Require(!UpgradeStudySystem.IsTuningReady(state) &&
            !UpgradeStudySystem.TryApplyActiveTuning(state, out _, out _),
            "Una sola barra correcta permitió estabilizar la firma.");
        UpgradeStudySystem.SetTuningValues(state, amplitude, frequency);
        Require(UpgradeStudySystem.TryApplyActiveTuning(state, out double accuracy,
            out double secondsApplied) &&
            accuracy >= UpgradeStudySystem.TuningMinimumChannelAccuracy &&
            secondsApplied > 0.0,
            "La sintonización activa correcta no aceleró el estudio.");
        Require(UpgradeStudySystem.TryRevealConclusion(state, out _),
            "La aceleración activa no permitió concluir el estudio de LE.");

        state.triangleActiveCircuit = TriangleCircuitType.Experimental;
        state.triangleSynchronization = 1f;
        UpgradeStudySystem.RecordCircuitSynchronized(state,
            TriangleCircuitType.Experimental);
        Require(UpgradeStudySystem.TryStartStudy(state,
            "study_triangle_synergy_resonance"),
            "No inició el estudio de Trazas.");
        UpgradeStudySystem.Advance(state, 240.0);
        Require(UpgradeStudySystem.TryRevealConclusion(state, out _),
            "No reveló Resonancia de Trazas.");
    }

    private static void ValidateSynchronizationMemory(GameState state)
    {
        Require(UpgradeStudySystem.GetStartBlockReason(state,
            "study_triangle_persistence_anchor") ==
            UpgradeStudyBlockReason.CircuitSwitchRequired,
            "Memoria no exigió un cambio de circuito.");
        UpgradeStudySystem.RecordCircuitSwitch(state,
            TriangleCircuitType.Energy, TriangleCircuitType.Experimental);
        UpgradeStudySystem.RecordCircuitSynchronized(state,
            TriangleCircuitType.Energy);
        UpgradeStudySystem.RecordCircuitSynchronized(state,
            TriangleCircuitType.Experimental);
        Require(UpgradeStudySystem.TryStartStudy(state,
            "study_triangle_persistence_anchor"),
            "Memoria no aceptó un circuito cambiado y sincronizado.");
        UpgradeStudySystem.Advance(state, 300.0);
        Require(UpgradeStudySystem.TryRevealConclusion(state, out _),
            "No reveló Memoria de Sincronía.");

        state.triangleActiveCircuit = TriangleCircuitType.Phase;
        state.triangleSynchronization = 1f;
        UpgradeStudySystem.RecordCircuitSynchronized(state, TriangleCircuitType.Phase);
        Require(UpgradeStudySystem.TryStartStudy(state,
            "study_triangle_energy_efficiency"),
            "No inició el estudio del enfoque Energía.");
        UpgradeStudySystem.Advance(state, 300.0);
        Require(UpgradeStudySystem.TryRevealConclusion(state, out _),
            "No reveló Captación Resonante.");
    }

    private static void ValidateGlobalTraceResonance(
        GameState state, F2UpgradeManager manager)
    {
        state.triangleSystemUnlocked = true;
        state.triangleSynchronization = 1f;
        state.Traces = 1000.0;
        Require(manager.TryBuy("triangle_synergy_resonance"),
            "No se pudo comprar Rastreo Resonante tras revelarlo.");

        state.triangleActiveCircuit = TriangleCircuitType.Experimental;
        Require(Math.Abs(state.GetTriangleTracesMultiplier() - 1.243) < 0.0001,
            "Rastreo Resonante no sumó +13% global al circuito de Trazas.");
        state.triangleActiveCircuit = TriangleCircuitType.Energy;
        Require(Math.Abs(state.GetTriangleTracesMultiplier() - 1.017) < 0.0001,
            "Rastreo Resonante dejó de actuar al cambiar al circuito de LE.");

        Require(manager.TryBuy("triangle_synergy_resonance"),
            "No se pudo comprar el segundo nivel de Rastreo Resonante.");
        state.triangleActiveCircuit = TriangleCircuitType.Experimental;
        Require(Math.Abs(state.GetTriangleTracesMultiplier() - 1.265) < 0.0001,
            "Rastreo Resonante no aplicó su +15% global final.");
    }

    private static void ValidateKeycardProject(GameState state)
    {
        state.triangleEnergy = Math.Max(state.triangleEnergy, 300.0);
        Require(UpgradeStudySystem.TryStartStudy(state,
            "study_experimental_chamber_keycard"),
            "El proyecto de keycard no respetó los dos estudios previos.");
        UpgradeStudySystem.Advance(state, 360.0);
        Require(UpgradeStudySystem.TryRevealConclusion(state, out string unlockId) &&
            unlockId == UpgradeStudySystem.KeycardProjectUnlockId,
            "No reveló el proyecto de keycard.");
        state.LE = state.experimentalChamberKeycardLeCost;
        state.Traces = state.experimentalChamberKeycardTraceCost;
        Require(state.CanBuyExperimentalChamberKeycard(),
            "La keycard siguió bloqueada tras descubrir el proyecto.");
    }

    private static void ValidateStateRoundTrip(GameState state)
    {
        state.upgradeStudies.hideCompletedUpgrades = true;
        string json = JsonUtility.ToJson(state.upgradeStudies);
        UpgradeStudyState loaded = JsonUtility.FromJson<UpgradeStudyState>(json);
        UpgradeStudySystem.ApplyLoadedState(state, loaded);
        Require(state.upgradeStudies.hideCompletedUpgrades &&
            UpgradeStudySystem.IsKeycardProjectDiscovered(state),
            "El estado de estudios no sobrevivió guardar/cargar.");
    }

    private static void ValidateFragmentScope(GameState state)
    {
        state.triangleActiveCircuit = TriangleCircuitType.Experimental;
        state.triangleSynchronization = 1f;
        state.experimentalChamberUnlocked = false;
        Require(Math.Abs(state.GetTriangleFragmentMultiplier() - 1.0) < 0.0001,
            "Apareció un bonus de fragmentos antes del Cuarto 2.");
        state.experimentalChamberUnlocked = true;
        state.triangleSynchronization = 0.5f;
        Require(Math.Abs(state.GetTriangleFragmentMultiplier() - 1.03) < 0.0001,
            "El bonus de fragmentos no escaló con la sincronización.");
        state.triangleSynchronization = 1f;
        Require(Math.Abs(state.GetTriangleFragmentMultiplier() - 1.06) < 0.0001,
            "El Circuito Experimental no otorgó su +6% base tras abrir Cuarto 2.");

        state.fragmentCondensation = 0;
        state.fragmentConfinement = 0;
        state.fragmentResidualInterference = 0;
        state.fragmentCondensationProgress = 0.0;
        state.fragmentConfinementProgress = 0.0;
        state.fragmentResidualInterferenceProgress = 0.0;
        TriangleOfflineReport report = state.ApplyOfflineBaseProgress(500.0);
        Require(report.condensationGained == 17 &&
            report.confinementGained == 17 &&
            report.residualInterferenceGained == 17,
            "El +6% de fragmentos no coincidió durante el progreso offline.");

        state.triangleActiveCircuit = TriangleCircuitType.Energy;
        Require(Math.Abs(state.GetTriangleFragmentMultiplier() - 1.0) < 0.0001,
            "El bonus de fragmentos permaneció activo fuera de Experimental.");
    }

    private static void ValidateLegacyMigration(
        GameState state, F2UpgradeManager manager)
    {
        manager.ApplyLoadedPurchasedTiers(new List<SavedF2UpgradeTier>
        {
            new SavedF2UpgradeTier { id = "emission_focus", purchasedTiers = 1 }
        }, F2UpgradeManager.ProgressionMigrationVersion);
        state.triangleSystemUnlocked = true;
        state.experimentalChamberUnlocked = false;
        UpgradeStudySystem.ApplyLoadedState(state, null);
        Require(UpgradeStudySystem.IsDiscovered(state, "emission_focus"),
            "La migración antigua ocultó una mejora comprada.");
        Require(UpgradeStudySystem.IsKeycardProjectDiscovered(state),
            "La migración antigua ocultó una keycard anteriormente visible.");
    }

    private static void SetSingleton(Type type, object value)
    {
        FieldInfo field = type.GetField("<I>k__BackingField",
            BindingFlags.Static | BindingFlags.NonPublic);
        if (field == null)
            throw new InvalidOperationException("No se encontró singleton de " + type.Name);
        field.SetValue(null, value);
    }

    private static void RegisterArtifact(GameState state, string id)
    {
        BuildingDef def = new BuildingDef { id = id, baseCost = 1.0, costMult = 1.1 };
        BuildingState building = new BuildingState { def = def, level = 1 };
        state.RegisterBuildingState(building);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif
