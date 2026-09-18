#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Dimension1RelicEffectsValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ActiveKey = "QF.D1RelicEffects.Active";
    private const string FailedKey = "QF.D1RelicEffects.Failed";
    private const string FrameKey = "QF.D1RelicEffects.Frame";

    private static readonly HashSet<string> PendingIds = new HashSet<string>();

    // Orden visual de las tres páginas, distinto del catálogo funcional por diseño.
    private static readonly string[] VisualIds =
    {
        Dimension1System.RelicDriftCompass,
        Dimension1System.RelicAncientDrill,
        Dimension1System.RelicAnalyticCrystal,
        Dimension1System.RelicLostNavigationRecord,
        Dimension1System.RelicModularContainer,
        Dimension1System.RelicProspectingCore,
        Dimension1System.RelicFracturedAntenna,
        Dimension1System.RelicExtractionSeal,
        Dimension1System.RelicExplorerPlate,
        Dimension1System.RelicIncompleteStarMap,
        Dimension1System.RelicRoom1Echo,
        Dimension1System.RelicExtractionHook,
        Dimension1System.RelicMatrixArchive,
        Dimension1System.RelicTracesResonator,
        Dimension1System.RelicRememberedAlloy,
        Dimension1System.RelicAncientCargoCore,
        Dimension1System.RelicCalibrationFragment,
        Dimension1System.RelicRareFrequencySensor,
        Dimension1System.RelicTriangularSeal,
        Dimension1System.RelicMachineMemory
    };

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailedKey, false);
        SessionState.SetInt(FrameKey, 0);
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            SaveService.SuppressWritesForVisualQa = true;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SaveService.SuppressWritesForVisualQa = false;
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailedKey, false);
            Debug.Log(failed
                ? "[D1 Relic Effects] FAIL"
                : "[D1 Relic Effects] PASS | 20 definidas + 3 efectos finales conectados + 20 selecciones + mejora");
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        int frame = SessionState.GetInt(FrameKey, 0) + 1;
        SessionState.SetInt(FrameKey, frame);
        if (frame < 28) return;
        EditorApplication.update -= Tick;
        try
        {
            Validate();
        }
        catch (Exception exception)
        {
            SessionState.SetBool(FailedKey, true);
            Debug.LogException(exception);
        }
        EditorApplication.isPlaying = false;
    }

    private static void Validate()
    {
        GameState state = GameState.I != null
            ? GameState.I
            : UnityEngine.Object.FindFirstObjectByType<GameState>(FindObjectsInactive.Include);
        Dimension1RelicsVisualUI visual = UnityEngine.Object.FindFirstObjectByType<Dimension1RelicsVisualUI>(FindObjectsInactive.Include);
        Require(state != null, "Falta GameState.");
        Require(visual != null, "Falta Dimension1RelicsVisualUI.");
        Require(Dimension1System.Dimension1RelicIds.Length == 20, "El catálogo activo no contiene 20 reliquias.");

        int definedCount = 0;
        int pendingCount = 0;
        foreach (string relicId in Dimension1System.Dimension1RelicIds)
        {
            double previousPrimary = 0.0;
            double previousSecondary = 0.0;
            bool pending = PendingIds.Contains(relicId);
            foreach (int milestone in new[] { 25, 50, 75, 100 })
            {
                bool defined = Dimension1System.TryGetDimension1RelicMilestoneBonuses(
                    relicId, milestone, out double primary, out double secondary);
                Require(defined != pending,
                    relicId + (pending ? " recibió un bonus inventado." : " perdió su bonus definido."));
                if (!defined) continue;
                Require(primary > previousPrimary && secondary > previousSecondary,
                    relicId + " no progresa de forma monótona en ambos efectos.");
                previousPrimary = primary;
                previousSecondary = secondary;
            }
            if (pending)
            {
                pendingCount++;
                Require(Nearly(Dimension1System.GetDimension1RelicPrimaryBonusForLevel(relicId, 100), 0.0) &&
                        Nearly(Dimension1System.GetDimension1RelicSecondaryBonusForLevel(relicId, 100), 0.0),
                    relicId + " aplica valores aunque continúa pendiente.");
            }
            else definedCount++;
        }
        Require(definedCount == 20 && pendingCount == 0,
            "Cobertura inesperada: definidas=" + definedCount + " pendientes=" + pendingCount + ".");

        RequireMilestone(Dimension1System.RelicFracturedAntenna, 25, .00375, .015);
        RequireMilestone(Dimension1System.RelicFracturedAntenna, 100, .015, .06);
        RequireMilestone(Dimension1System.RelicTracesResonator, 25, .0075, .0025);
        RequireMilestone(Dimension1System.RelicTracesResonator, 100, .03, .01);
        RequireMilestone(Dimension1System.RelicCalibrationFragment, 25, .01, .0025);
        RequireMilestone(Dimension1System.RelicCalibrationFragment, 100, .04, .01);
        RequireMilestone(Dimension1System.RelicTriangularSeal, 25, .0075, .0025);
        RequireMilestone(Dimension1System.RelicTriangularSeal, 100, .03, .01);
        RequireMilestone(Dimension1System.RelicIncompleteStarMap, 25, .01, .005);
        RequireMilestone(Dimension1System.RelicIncompleteStarMap, 100, .04, .02);
        RequireMilestone(Dimension1System.RelicRareFrequencySensor, 25, .005, .002);
        RequireMilestone(Dimension1System.RelicRareFrequencySensor, 100, .02, .008);
        RequireMilestone(Dimension1System.RelicMachineMemory, 25, .005, .0025);
        RequireMilestone(Dimension1System.RelicMachineMemory, 100, .02, .01);

        state.ResetDimension1MvpState();
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        state.UnlockD1Sector(Dimension1System.Sector01OuterRim);
        Require(state.TrySelectD1Sector(Dimension1System.Sector01OuterRim), "No se pudo seleccionar el sector de prueba.");

        ValidateApprovedRelicConsumers(state);

        state.SetD1RelicLevel(Dimension1System.RelicFracturedAntenna, 0);
        Require(Dimension1System.TryScanSimpleDestination(state), "No comenzó el escaneo base.");
        double baseScanSeconds = state.dimension1ScanTotalSeconds;
        ResetScan(state);
        state.SetD1RelicLevel(Dimension1System.RelicFracturedAntenna, 100);
        Require(Dimension1System.TryScanSimpleDestination(state), "No comenzó el escaneo con Antena.");
        double antennaScanSeconds = state.dimension1ScanTotalSeconds;
        Require(Nearly(antennaScanSeconds, baseScanSeconds * .94),
            "La reducción secundaria de Antena no llegó al temporizador real.");
        Require(Nearly(Dimension1System.GetFracturedAntennaExtraScanDestinationChancePreview(state), .015),
            "El efecto primario de Antena no conserva 1.5 pp a nivel 100.");
        ResetScan(state);

        foreach (string relicId in Dimension1System.Dimension1RelicIds)
            state.SetD1RelicLevel(relicId, 1);
        state.SetD1RelicLevel(Dimension1System.RelicFracturedAntenna, 100);
        visual.SetReferencePreviewForVisualQa(false);
        SelectAllVisualRelics(visual);
        MoveToPage(visual, 0);
        visual.SelectRelic6();
        TMP_Text secondaryText = FindNamedChild(visual.transform, "EffectSecondary")
            ?.GetComponentInChildren<TMP_Text>(true);
        Require(secondaryText != null && secondaryText.text.Contains("6%") &&
                !secondaryText.text.ToLowerInvariant().Contains("pendiente"),
            "La Cámara no muestra el efecto secundario real de Antena.");

        AssertDefinedRelicText(
            visual,
            state,
            Dimension1System.RelicTracesResonator,
            1,
            5,
            "3%",
            "1%"
        );
        AssertDefinedRelicText(
            visual,
            state,
            Dimension1System.RelicCalibrationFragment,
            2,
            0,
            "4%",
            "1 pp"
        );
        AssertDefinedRelicText(
            visual,
            state,
            Dimension1System.RelicTriangularSeal,
            2,
            2,
            "3%",
            "1%"
        );
        AssertDefinedRelicText(
            visual,
            state,
            Dimension1System.RelicIncompleteStarMap,
            1,
            1,
            "4%",
            "2%"
        );
        AssertDefinedRelicText(
            visual,
            state,
            Dimension1System.RelicRareFrequencySensor,
            2,
            1,
            "2 puntos porcentuales",
            "0.8 puntos porcentuales"
        );
        AssertDefinedRelicText(
            visual,
            state,
            Dimension1System.RelicMachineMemory,
            2,
            3,
            "2%",
            "1%"
        );

        // Compra real desde el skin, sin escribir el guardado personal.
        state.SetD1RelicLevel(Dimension1System.RelicDriftCompass, 24);
        state.LE = Math.Max(state.LE, 1e12);
        state.Traces = Math.Max(state.Traces, 1e12);
        foreach (string metalId in new[]
        {
            Dimension1System.MetalIron, Dimension1System.MetalCopper,
            Dimension1System.MetalAluminum, Dimension1System.MetalTitanium,
            Dimension1System.MetalNickel, Dimension1System.MetalCobalt,
            Dimension1System.MetalLithium, Dimension1System.MetalPlatinum,
            Dimension1System.MetalTungsten, Dimension1System.MetalIridium
        }) state.AddD1Metal(metalId, 1e12);
        MoveToPage(visual, 0);
        visual.SelectRelic0();
        visual.UpgradeSelected();
        Require(state.GetD1RelicLevel(Dimension1System.RelicDriftCompass) == 25,
            "La mejora desde la Cámara no alcanzó el nivel siguiente.");
    }

    private static void SelectAllVisualRelics(Dimension1RelicsVisualUI visual)
    {
        MoveToPage(visual, 0);
        for (int index = 0; index < VisualIds.Length; index++)
        {
            int page = index / 8;
            MoveToPage(visual, page);
            SelectSlot(visual, index % 8);
            Require(visual.SelectedRelicId == VisualIds[index],
                "La tarjeta visual " + index + " seleccionó un ID distinto.");
            TMP_Text primary = FindNamedChild(visual.transform, "EffectPrimary")
                ?.GetComponentInChildren<TMP_Text>(true);
            TMP_Text secondary = FindNamedChild(visual.transform, "EffectSecondary")
                ?.GetComponentInChildren<TMP_Text>(true);
            Require(primary != null && secondary != null,
                "Faltan los textos de efectos de " + VisualIds[index] + ".");
            if (!PendingIds.Contains(VisualIds[index]))
            {
                Require(!primary.text.Contains("Bonificación primaria") &&
                        !secondary.text.Contains("Bonificación secundaria") &&
                        !primary.text.ToLowerInvariant().Contains("pendiente") &&
                        !secondary.text.ToLowerInvariant().Contains("pendiente"),
                    VisualIds[index] + " conserva una descripción genérica o pendiente.");
            }
        }
    }

    private static void ValidateApprovedRelicConsumers(GameState state)
    {
        BuildingState observer = state.GetBuildingState("vacuum_observer");
        BuildingState tracesBuilding = state.GetBuildingState("casimir_panel");
        BuildingState energyBuilding = state.GetBuildingState("fluctuation_antenna");
        Require(observer != null && tracesBuilding != null && energyBuilding != null,
            "Faltan los tres artefactos necesarios para validar las reliquias.");
        observer.level = 1;
        tracesBuilding.level = 1;
        energyBuilding.level = 1;

        state.triangleSystemUnlocked = true;
        state.triangleActiveCircuit = TriangleCircuitType.None;

        state.SetD1RelicLevel(Dimension1System.RelicTracesResonator, 0);
        double baseTraces = state.CalculateTracesPs();
        double baseGeneratorTraceCost = state.GetTriangleEnergyGeneratorTraceCost();
        Require(baseTraces > 0.0 && baseGeneratorTraceCost > 0.0,
            "Las bases de Trazas no están disponibles para validar Resonador.");

        F2UpgradeManager upgrades = F2UpgradeManager.I;
        Require(upgrades != null, "Falta F2UpgradeManager para validar costes de Mejoras.");
        upgrades.ApplyLoadedPurchasedTiers(new List<SavedF2UpgradeTier>());
        const string traceUpgradeId = "triangle_impulse_tuning";
        double baseUpgradeTraceCost = upgrades.GetNextCost(traceUpgradeId);
        Require(baseUpgradeTraceCost > 0.0,
            "No existe un coste de Trazas base para la mejora de prueba.");

        state.SetD1RelicLevel(Dimension1System.RelicTracesResonator, 100);
        Require(Nearly(state.CalculateTracesPs(), baseTraces * 1.03),
            "Resonador no aumenta 3 % la producción real de Trazas.");
        Require(Nearly(state.GetTriangleEnergyGeneratorTraceCost(),
                baseGeneratorTraceCost * .99),
            "Resonador no reduce 1 % el coste de Trazas del Captador.");
        Require(Nearly(upgrades.GetNextCost(traceUpgradeId),
                baseUpgradeTraceCost * .99),
            "Resonador no reduce 1 % el coste de Trazas de Mejoras.");
        UpgradeStudyState studyState = UpgradeStudySystem.EnsureState(state);
        if (!studyState.discoveredIds.Contains(traceUpgradeId))
            studyState.discoveredIds.Add(traceUpgradeId);
        state.Traces = Math.Max(state.Traces, 10000.0);
        double tracesBeforeUpgrade = state.Traces;
        Require(upgrades.TryBuy(traceUpgradeId) &&
                Nearly(state.Traces,
                    tracesBeforeUpgrade - baseUpgradeTraceCost * .99),
            "La compra real de Mejoras no descontó el coste reducido por Resonador.");
        upgrades.ApplyLoadedPurchasedTiers(new List<SavedF2UpgradeTier>());

        state.SetD1RelicLevel(Dimension1System.RelicCalibrationFragment, 0);
        state.triangleActiveCircuit = TriangleCircuitType.LE;
        state.triangleSynchronization = .5f;
        state.triangleSynchronizationBaseRatePerSecond = 1.0 / 180.0;
        double baseSynchronizationSeconds = state.GetTriangleSynchronizationRemainingSeconds();
        state.SetD1RelicLevel(Dimension1System.RelicCalibrationFragment, 100);
        double boostedSynchronizationSeconds = state.GetTriangleSynchronizationRemainingSeconds();
        Require(Nearly(boostedSynchronizationSeconds,
                baseSynchronizationSeconds / 1.04),
            "Fragmento no aumenta 4 % la sincronización real del Triángulo.");

        studyState.activeStudyId = "study_triangle_impulse_tuning";
        studyState.activeProgressSeconds = 0.0;
        studyState.tuningStage = 0;
        UpgradeStudySystem.GetTuningTarget(state, out float amplitude, out float frequency);
        UpgradeStudySystem.SetTuningValues(state, amplitude, frequency);
        Require(UpgradeStudySystem.TryApplyActiveTuning(
                state, out _, out double tunedSeconds),
            "La sintonización correcta del estudio no pudo aplicarse.");
        UpgradeStudyDef activeStudy = UpgradeStudySystem.GetStudy(
            "study_triangle_impulse_tuning");
        Require(activeStudy != null && Nearly(tunedSeconds, activeStudy.durationSeconds * .19),
            "Fragmento no eleva el avance de sintonización de 18 % a 19 %.");

        state.SetD1RelicLevel(Dimension1System.RelicTriangularSeal, 0);
        state.triangleSynchronization = 1f;
        double baseProtocolMultiplier = state.GetTriangleLEMultiplier();
        state.triangleActiveCircuit = TriangleCircuitType.None;
        double baseEnergy = state.CalculateTriangleEnergyPerSecond();
        Require(baseProtocolMultiplier > 1.0 && baseEnergy > 0.0,
            "Las bases del Triángulo no están disponibles para validar Sello.");

        state.SetD1RelicLevel(Dimension1System.RelicTriangularSeal, 100);
        state.triangleActiveCircuit = TriangleCircuitType.LE;
        double boostedProtocolMultiplier = state.GetTriangleLEMultiplier();
        Require(Nearly(boostedProtocolMultiplier - 1.0,
                (baseProtocolMultiplier - 1.0) * 1.03),
            "Sello no aumenta 3 % relativo la potencia del protocolo activo.");
        state.triangleActiveCircuit = TriangleCircuitType.None;
        Require(Nearly(state.CalculateTriangleEnergyPerSecond(), baseEnergy * 1.01),
            "Sello no aumenta 1 % la producción real de Energía del Triángulo.");

        state.SetD1RelicLevel(Dimension1System.RelicIncompleteStarMap, 0);
        float baseCommonWeight = Dimension1System.GetDimension1AdjustedScanDestinationWeight(
            state,
            Dimension1System.Sector01OuterRim,
            Dimension1System.DestinationMineralBelt
        );
        float baseRareWeight = Dimension1System.GetDimension1AdjustedScanDestinationWeight(
            state,
            Dimension1System.Sector01OuterRim,
            Dimension1System.DestinationAbandonedShip
        );
        state.SetD1RelicLevel(Dimension1System.RelicIncompleteStarMap, 100);
        float adjustedCommonWeight = Dimension1System.GetDimension1AdjustedScanDestinationWeight(
            state,
            Dimension1System.Sector01OuterRim,
            Dimension1System.DestinationMineralBelt
        );
        float adjustedRareWeight = Dimension1System.GetDimension1AdjustedScanDestinationWeight(
            state,
            Dimension1System.Sector01OuterRim,
            Dimension1System.DestinationAbandonedShip
        );
        Require(adjustedCommonWeight < baseCommonWeight && adjustedRareWeight > baseRareWeight,
            "Mapa Estelar no acerca los pesos reales del sector a una oferta más variada.");
        Require(Nearly(Dimension1System.GetIncompleteStarMapRepetitionReduction(state), .04) &&
                Nearly(Dimension1System.GetIncompleteStarMapVarietyBonus(state), .02),
            "Mapa Estelar no conserva sus dos efectos a nivel 100.");

        state.SetD1RelicLevel(Dimension1System.RelicRareFrequencySensor, 0);
        float baseSpecialPointChance = Dimension1System.GetD1SpecialPointScanChance(state);
        state.SetD1RelicLevel(Dimension1System.RelicRareFrequencySensor, 100);
        float sensorSpecialPointChance = Dimension1System.GetD1SpecialPointScanChance(state);
        Require(Nearly(sensorSpecialPointChance, baseSpecialPointChance + .02),
            "Sensor de Frecuencias no añade 2 puntos porcentuales a los puntos especiales.");
        Require(Nearly(Dimension1System.GetRareFrequencySensorCategoryPromotionChance(state), .008) &&
                Dimension1System.ShouldRareFrequencySensorPromoteCategory(state, .007f) &&
                !Dimension1System.ShouldRareFrequencySensorPromoteCategory(state, .009f),
            "Sensor de Frecuencias no aplica correctamente la promoción de categoría.");
        Require(Dimension1System.IsD1HigherCategorySpecialPoint(
                    Dimension1System.D1SpecialPointRelicEcho) &&
                Dimension1System.IsD1HigherCategorySpecialPoint(
                    Dimension1System.D1SpecialPointMatrixTrace) &&
                !Dimension1System.IsD1HigherCategorySpecialPoint(
                    Dimension1System.D1SpecialPointMineralDeposit),
            "Las categorías superiores del Sensor no corresponden al sistema existente.");

        MachineManager machine = MachineManager.I;
        Require(machine != null, "Falta MachineManager para validar Memoria de Máquina.");
        List<MachineNodeDef> visibleNodes = machine.GetAllNodes(false);
        var repairedNodeIds = new List<string>();
        for (int i = 0; i < visibleNodes.Count; i++)
        {
            MachineNodeDef node = visibleNodes[i];
            if (node != null)
                repairedNodeIds.Add(node.id);
        }
        machine.LoadProgressFromSave(new SaveData
        {
            machineIntroSeen = true,
            machineUnlocked = true,
            machineAllZonesUnlocked = true,
            machineRepairedNodeIds = repairedNodeIds
        });
        state.SetD1RelicLevel(Dimension1System.RelicMachineMemory, 0);
        double baseNodeEffect = machine.GetTotalEffectValue(MachineNodeEffectType.GlobalLEBonus);
        double baseRepairImpact = machine.GetZoneProgressSyncBonus(MachineZoneType.Room1Link);
        Require(baseNodeEffect > 0.0 && baseRepairImpact > 0.0,
            "Las bases de la Máquina no están disponibles para validar Memoria de Máquina.");
        state.SetD1RelicLevel(Dimension1System.RelicMachineMemory, 100);
        Require(Nearly(machine.GetTotalEffectValue(MachineNodeEffectType.GlobalLEBonus),
                baseNodeEffect * 1.02),
            "Memoria de Máquina no aumenta 2 % los efectos numéricos de nodos.");
        Require(Nearly(machine.GetZoneProgressSyncBonus(MachineZoneType.Room1Link),
                baseRepairImpact * 1.01),
            "Memoria de Máquina no aumenta 1 % los bonus derivados de reparación.");

        studyState.activeStudyId = "";
        studyState.activeProgressSeconds = 0.0;
        studyState.tuningStage = 0;
    }

    private static void AssertDefinedRelicText(
        Dimension1RelicsVisualUI visual,
        GameState state,
        string relicId,
        int page,
        int slot,
        string primaryValue,
        string secondaryValue
    )
    {
        state.SetD1RelicLevel(relicId, 100);
        MoveToPage(visual, page);
        SelectSlot(visual, slot);
        TMP_Text primary = FindNamedChild(visual.transform, "EffectPrimary")
            ?.GetComponentInChildren<TMP_Text>(true);
        TMP_Text secondary = FindNamedChild(visual.transform, "EffectSecondary")
            ?.GetComponentInChildren<TMP_Text>(true);
        Require(primary != null && primary.text.Contains(primaryValue) &&
                !primary.text.ToLowerInvariant().Contains("pendiente"),
            "La Cámara no muestra el efecto primario real de " + relicId + ".");
        Require(secondary != null && secondary.text.Contains(secondaryValue) &&
                !secondary.text.ToLowerInvariant().Contains("pendiente"),
            "La Cámara no muestra el efecto secundario real de " + relicId + ".");
    }

    private static void MoveToPage(Dimension1RelicsVisualUI visual, int page)
    {
        while (visual.CurrentPage < page) visual.NextPage();
        while (visual.CurrentPage > page) visual.PreviousPage();
    }

    private static void SelectSlot(Dimension1RelicsVisualUI visual, int slot)
    {
        switch (slot)
        {
            case 0: visual.SelectRelic0(); break;
            case 1: visual.SelectRelic1(); break;
            case 2: visual.SelectRelic2(); break;
            case 3: visual.SelectRelic3(); break;
            case 4: visual.SelectRelic4(); break;
            case 5: visual.SelectRelic5(); break;
            case 6: visual.SelectRelic6(); break;
            case 7: visual.SelectRelic7(); break;
            default: throw new InvalidOperationException("Slot visual inválido: " + slot);
        }
    }

    private static Transform FindNamedChild(Transform root, string name)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static void ResetScan(GameState state)
    {
        state.dimension1ScanActive = false;
        state.dimension1ActiveScanSectorId = "";
        state.dimension1ScanRemainingSeconds = 0.0;
        state.dimension1ScanTotalSeconds = 0.0;
        state.dimension1ScannedDestinations.Clear();
        state.dimension1PreviousScannedDestinationIds.Clear();
    }

    private static void RequireMilestone(string relicId, int level, double primary, double secondary)
    {
        Require(Dimension1System.TryGetDimension1RelicMilestoneBonuses(
            relicId, level, out double actualPrimary, out double actualSecondary),
            "No existe el hito esperado de " + relicId + ".");
        Require(Nearly(actualPrimary, primary) && Nearly(actualSecondary, secondary),
            "Valores incorrectos para " + relicId + " nivel " + level + ".");
    }

    private static bool Nearly(double a, double b) => Math.Abs(a - b) <= 0.000001;

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif
