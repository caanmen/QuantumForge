using System;
using System.Collections.Generic;


public static class D2Civilization3System
{
    public const string Zone1Id = "d2_c3_zone_1";
    public const string Zone2Id = "d2_c3_zone_2";
    public const string Zone3Id = "d2_c3_zone_3";
    public const string LowQualityId = "d2_c3_quality_low";
    public const string MediumQualityId = "d2_c3_quality_medium";
    public const string HighQualityId = "d2_c3_quality_high";
    public const double ExcavationDurationSeconds = 30.0;
    public const double AnalysisDurationSeconds = 30.0;
    public const double AnalysisSpeedMilestone = 40.0;
    public const double AnalysisSpeedMultiplier = 0.95;
    public const double BonusRemainsMilestone = 20.0;
    public const double BonusRemainsPerExcavation = 0.05;
    public const double KnowledgeBonusMilestone = 80.0;
    public const double KnowledgeBonusMultiplier = 1.10;
    public const double FieldScholarWaxCost = 10.0;
    public const double FieldScholarBreadCost = 10.0;
    public const double InscriptionScholarWaxCost = 20.0;
    public const double InscriptionScholarBreadCost = 20.0;
    public const double SealsScholarWaxCost = 30.0;
    public const double SealsScholarBreadCost = 30.0;
    public const double Zone2IncenseCost = 25.0;
    public const double Zone2SacredClothCost = 25.0;
    public const double Zone2UnlockResearchRequirement = 60.0;
    public const double Zone3IncenseCost = 50.0;
    public const double Zone3SacredClothCost = 50.0;
    public const double Zone3CarvedStoneCost = 50.0;
    public const double Zone3UnlockResearchRequirement = 60.0;
    public const double ClueDetectionResearchRequirement = 20.0;
    public const double ArchiveLevel3ResearchRequirement = 40.0;
    public const double ArchiveLevel4ResearchRequirement = 30.0;
    public const long BasicAnomalyClueRequirement = 8L;
    public const long SymbolicAnomalyClueRequirement = 10L;
    public const long DeepAnomalyClueRequirement = 12L;
    public const double BasicAnomalyKnowledgeCost = 25.0;
    public const long BasicAnomalyResourceCost = 15L;
    public const double SymbolicAnomalyKnowledgeCost = 40.0;
    public const long SymbolicAnomalyResourceCost = 25L;
    public const double DeepAnomalyKnowledgeCost = 60.0;
    public const long DeepAnomalyResourceCost = 35L;
    public const double EntityResearchSecondsPerPercent = 30.0;
    public const double EntityResearchKnowledgePerPercent = 1.0;
    public const double EntityResearchMilestone30 = 30.0;
    public const double EntityResearchMilestone60 = 60.0;
    public const double EntityResearchMilestone85 = 85.0;
    public const double EntityResearchMilestone100 = 100.0;
    public const long EntityMilestone30ZoneResourceCost = 25L;
    public const long EntityMilestone60ZoneResourceCost = 35L;
    public const long EntityMilestone85ZoneResourceCost = 45L;
    public const long EntityMilestone100EachZoneResourceCost = 50L;
    public const long EntityMilestone30KnowledgeReward = 1L;
    public const long EntityMilestone60KnowledgeReward = 2L;
    public const long EntityMilestone85KnowledgeReward = 3L;
    public const string ResonantExpeditionLineId = "d2_c3_pact_resonant_expedition";
    public const string EndlessArchiveLineId = "d2_c3_pact_endless_archive";
    public const string SharedMemoryLineId = "d2_c3_pact_shared_memory";
    public const string ModulatorResonanceLineId = "d2_c3_pact_modulator_resonance";
    public const string FirstThresholdChronicleLineId = "d2_c3_pact_first_threshold_chronicle";
    public const int MaxEntityPactLineLevel = 3;
    public const double ResonantExpeditionBonusPerLevel = 0.10;
    public const double EndlessArchiveBonusPerLevel = 0.10;
    public const double SharedMemoryBonusPerLevel = 0.03;
    public const double ModulatorCalibrationBonusPerLevel = 0.05;
    public const int Prestige1PreviewBonusPerLevel = 1;
    public const double Zone1CompletionExtraRemainsBonus = 0.10;
    public const double Zone2CompletionAnalysisDurationMultiplier = 0.90;
    public const double Zone3CompletionRewardBonus = 0.10;
    public const int MaxScholarLevel = 3;
    public const double ScholarAnalysisSpeedBonusPerLevel = 0.05;
    public const double ScholarRewardBonusPerLevel = 0.05;
    public const string StratifiedCartographyUpgradeId = "d2_c3_archive_stratified_cartography";
    public const string AnomalousConcordanceUpgradeId = "d2_c3_archive_anomalous_concordance";
    public const string DeepExegesisUpgradeId = "d2_c3_archive_deep_exegesis";
    public const double StratifiedCartographyExcavationSpeedBonus = 0.05;
    public const double AnomalousConcordanceClueBonus = 0.10;
    public const double DeepExegesisCostReduction = 0.10;

    public static readonly string[] ZoneIds = { Zone1Id, Zone2Id, Zone3Id };
    public static readonly string[] EntityPactLineIds =
    {
        ResonantExpeditionLineId,
        EndlessArchiveLineId,
        SharedMemoryLineId,
        ModulatorResonanceLineId,
        FirstThresholdChronicleLineId
    };
    public static readonly string[] ArchiveUpgradeIds =
    {
        StratifiedCartographyUpgradeId,
        AnomalousConcordanceUpgradeId,
        DeepExegesisUpgradeId
    };

    public static void EnsureState(D2Civilization3State state)
    {
        if (state == null)
            return;

        if (state.zones == null)
            state.zones = new List<D2C3ZoneState>();
        state.ancientKnowledge = ClampNonNegative(state.ancientKnowledge);
        for (int i = state.zones.Count - 1; i >= 0; i--)
        {
            D2C3ZoneState zone = state.zones[i];
            if (zone == null || Array.IndexOf(ZoneIds, zone.zoneId) < 0 ||
                HasDuplicateBefore(state, i))
            {
                state.zones.RemoveAt(i);
            }
        }

        foreach (string zoneId in ZoneIds)
        {
            D2C3ZoneState zone = GetZone(state, zoneId);
            if (zone == null)
            {
                zone = new D2C3ZoneState { zoneId = zoneId };
                state.zones.Add(zone);
            }

            if (zoneId == Zone1Id)
                zone.unlocked = true;
            zone.lowQualityRemains = Math.Max(0L, zone.lowQualityRemains);
            zone.mediumQualityRemains = Math.Max(0L, zone.mediumQualityRemains);
            zone.highQualityRemains = Math.Max(0L, zone.highQualityRemains);
            zone.totalExcavationsCompleted = Math.Max(0L, zone.totalExcavationsCompleted);
            zone.zoneResourceAmount = Math.Max(0L, zone.zoneResourceAmount);
            zone.zoneResourceRewardProgress = Math.Clamp(
                ClampNonNegative(zone.zoneResourceRewardProgress), 0.0, 0.999999999);
            zone.researchProgress = Math.Clamp(ClampNonNegative(zone.researchProgress), 0.0, 100.0);
            zone.totalAnalysesCompleted = Math.Max(0L, zone.totalAnalysesCompleted);
            zone.bonusRemainsProgress = Math.Clamp(
                ClampNonNegative(zone.bonusRemainsProgress),
                0.0,
                0.999999999
            );
            zone.anomalyClues = Math.Max(0L, zone.anomalyClues);
            zone.anomalyClueProgress = Math.Clamp(
                ClampNonNegative(zone.anomalyClueProgress),
                0.0,
                0.999999999
            );
            zone.anomalousData = Math.Max(0L, zone.anomalousData);
            if (zone.anomalyClues >= GetAnomalyClueRequirement(zone.zoneId))
                zone.anomalyRevealed = true;
            if (zone.anomalyRead)
                zone.anomalyRevealed = true;
            zone.excavationRemainingSeconds = ClampDuration(
                zone.excavationRemainingSeconds,
                zone.excavationActive ? GetExcavationDuration(state) : 0.0
            );
            if (!zone.excavationActive)
                zone.excavationRemainingSeconds = 0.0;
            zone.analysisRemainingSeconds = ClampAnalysisDuration(
                zone.analysisRemainingSeconds,
                zone.analysisActive ? GetAnalysisDuration(state, zone) : 0.0
            );
            if (!zone.analysisActive)
            {
                zone.analysisRemainingSeconds = 0.0;
                zone.analysisQualityId = "";
            }
            else if (!IsQualityId(zone.analysisQualityId))
            {
                zone.analysisActive = false;
                zone.analysisRemainingSeconds = 0.0;
                zone.analysisQualityId = "";
            }
            if (!zone.unlocked)
            {
                zone.excavationActive = false;
                zone.excavationRemainingSeconds = 0.0;
                zone.analysisActive = false;
                zone.analysisRemainingSeconds = 0.0;
                zone.analysisQualityId = "";
            }
            zone.scholarLevel = zone.scholarHired
                ? Math.Clamp(Math.Max(1, zone.scholarLevel), 1, MaxScholarLevel)
                : 0;
        }

        D2C3ZoneState zone1State = GetZone(state, Zone1Id);
        D2C3ZoneState zone2State = GetZone(state, Zone2Id);
        if (zone1State != null && zone1State.totalAnalysesCompleted > 0L)
            state.archiveUnlocked = true;
        state.archiveLevel = state.archiveUnlocked ? Math.Max(1, state.archiveLevel) : 0;
        if (zone1State != null && zone1State.researchProgress >= 40.0)
        {
            state.archiveUnlocked = true;
            state.archiveLevel = Math.Max(2, state.archiveLevel);
        }
        if (zone2State != null && zone2State.unlocked &&
            zone2State.researchProgress >= ClueDetectionResearchRequirement)
        {
            state.anomalyClueDetectionUnlocked = true;
        }
        if (zone2State != null && zone2State.unlocked &&
            zone2State.researchProgress >= ArchiveLevel3ResearchRequirement)
        {
            state.archiveUnlocked = true;
            state.archiveLevel = Math.Max(3, state.archiveLevel);
        }
        D2C3ZoneState zone3State = GetZone(state, Zone3Id);
        if (zone3State != null && zone3State.unlocked &&
            zone3State.researchProgress >= ArchiveLevel4ResearchRequirement)
        {
            state.archiveUnlocked = true;
            state.archiveLevel = Math.Max(4, state.archiveLevel);
        }

        state.entityResearchProgress = Math.Clamp(
            ClampNonNegative(state.entityResearchProgress), 0.0, 100.0);
        state.entityKnowledge = Math.Max(0L, state.entityKnowledge);
        if (HasAllAnomalousData(state) || state.entityResearchProgress > 0.0 ||
            state.entityResearchMilestone30Completed ||
            state.entityResearchMilestone60Completed ||
            state.entityResearchMilestone85Completed ||
            state.entityResearchMilestone100Completed)
        {
            state.entityResearchUnlocked = true;
        }
        if (state.entityResearchMilestone30Completed)
            state.entityResearchProgress = Math.Max(EntityResearchMilestone30,
                state.entityResearchProgress);
        if (state.entityResearchMilestone60Completed)
        {
            state.entityResearchMilestone30Completed = true;
            state.entityResearchProgress = Math.Max(EntityResearchMilestone60,
                state.entityResearchProgress);
        }
        if (state.entityResearchMilestone85Completed)
        {
            state.entityResearchMilestone30Completed = true;
            state.entityResearchMilestone60Completed = true;
            state.entityResearchProgress = Math.Max(EntityResearchMilestone85,
                state.entityResearchProgress);
        }
        if (state.entityResearchMilestone100Completed)
        {
            state.entityResearchMilestone30Completed = true;
            state.entityResearchMilestone60Completed = true;
            state.entityResearchMilestone85Completed = true;
            state.entityResearchProgress = EntityResearchMilestone100;
            state.entityPactAvailable = true;
        }
        if (!state.entityResearchUnlocked || state.entityResearchMilestone100Completed ||
            IsAtUnpaidEntityResearchMilestone(state))
        {
            state.entityResearchActive = false;
        }

        EnsureEntityPactState(state);

        if (!IsSelectableZone(state, state.selectedZoneId))
            state.selectedZoneId = Zone1Id;
        if (state.lastResult == null)
            state.lastResult = "";
        state.progressVersion = Dimension2System.Civilization3ProgressVersion;
    }

    public static void Tick(GameState gameState, double seconds)
    {
        if (gameState?.dimension2?.civilization3 == null || seconds <= 0.0 ||
            double.IsNaN(seconds) || double.IsInfinity(seconds))
        {
            return;
        }

        D2Civilization3State state = gameState.dimension2.civilization3;
        EnsureState(state);
        if (!gameState.dimension2.civilization3Unlocked)
            return;

        foreach (D2C3ZoneState zone in state.zones)
        {
            AdvanceExcavation(state, zone, seconds);
            AdvanceAnalysis(state, zone, seconds);
        }
        AdvanceEntityResearch(state, seconds);
    }

    public static void ApplyOfflineProgress(GameState gameState, double seconds)
    {
        Tick(gameState, seconds);
    }

    public static D2C3ZoneState GetZone(D2Civilization3State state, string zoneId)
    {
        if (state?.zones == null)
            return null;
        foreach (D2C3ZoneState zone in state.zones)
            if (zone != null && zone.zoneId == zoneId) return zone;
        return null;
    }

    public static D2C3ZoneState GetSelectedZone(D2Civilization3State state)
    {
        D2C3ZoneState zone = GetZone(state, state?.selectedZoneId);
        return zone != null && zone.unlocked ? zone : GetZone(state, Zone1Id);
    }

    public static bool IsSelectableZone(D2Civilization3State state, string zoneId)
    {
        D2C3ZoneState zone = GetZone(state, zoneId);
        return zone != null && zone.unlocked;
    }

    public static bool TrySelectZone(GameState gameState, string zoneId)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            !IsSelectableZone(state, zoneId))
        {
            return false;
        }
        state.selectedZoneId = zoneId;
        return true;
    }

    public static string GetZoneName(string zoneId)
    {
        switch (zoneId)
        {
            case Zone1Id: return "Entrada Sepultada";
            case Zone2Id: return "Galería de Inscripciones";
            case Zone3Id: return "Santuario Sellado";
            default: return "Zona desconocida";
        }
    }

    public static string GetZoneResourceName(string zoneId)
    {
        switch (zoneId)
        {
            case Zone1Id: return "Fragmentos Base";
            case Zone2Id: return "Inscripciones Parciales";
            case Zone3Id: return "Sellos Antiguos";
            default: return "Recurso desconocido";
        }
    }

    public static string GetScholarName(string zoneId)
    {
        switch (zoneId)
        {
            case Zone2Id: return "Erudito de Inscripciones";
            case Zone3Id: return "Erudito de Sellos";
            default: return "Erudito de Campo";
        }
    }

    public static string GetClueName(string zoneId)
    {
        switch (zoneId)
        {
            case Zone2Id: return "Indicios Anómalos Simbólicos";
            case Zone3Id: return "Indicios Anómalos Profundos";
            default: return "Indicios Anómalos Básicos";
        }
    }

    public static string GetAnomalyName(string zoneId)
    {
        switch (zoneId)
        {
            case Zone2Id: return "Anomalía Simbólica";
            case Zone3Id: return "Anomalía Profunda";
            default: return "Anomalía Básica";
        }
    }

    public static string GetAnomalousDataName(string zoneId)
    {
        switch (zoneId)
        {
            case Zone2Id: return "Datos Anómalos Simbólicos";
            case Zone3Id: return "Datos Anómalos Profundos";
            default: return "Datos Anómalos Básicos";
        }
    }

    public static long GetAnomalyClueRequirement(string zoneId)
    {
        if (zoneId == Zone2Id) return SymbolicAnomalyClueRequirement;
        return zoneId == Zone3Id ? DeepAnomalyClueRequirement : BasicAnomalyClueRequirement;
    }

    public static double GetAnomalyKnowledgeCost(string zoneId)
    {
        if (zoneId == Zone1Id) return BasicAnomalyKnowledgeCost;
        return zoneId == Zone2Id ? SymbolicAnomalyKnowledgeCost : DeepAnomalyKnowledgeCost;
    }

    public static long GetAnomalyResourceCost(string zoneId)
    {
        if (zoneId == Zone1Id) return BasicAnomalyResourceCost;
        return zoneId == Zone2Id ? SymbolicAnomalyResourceCost : DeepAnomalyResourceCost;
    }

    public static double GetEffectiveAnomalyKnowledgeCost(
        D2Civilization3State state,
        string zoneId
    )
    {
        double multiplier = state != null && state.deepExegesisUnlocked
            ? 1.0 - DeepExegesisCostReduction
            : 1.0;
        return GetAnomalyKnowledgeCost(zoneId) * multiplier;
    }

    public static long GetEffectiveAnomalyResourceCost(
        D2Civilization3State state,
        string zoneId
    )
    {
        double multiplier = state != null && state.deepExegesisUnlocked
            ? 1.0 - DeepExegesisCostReduction
            : 1.0;
        return Math.Max(1L, (long)Math.Ceiling(GetAnomalyResourceCost(zoneId) * multiplier));
    }

    public static bool CanReadAnomaly(GameState gameState, string zoneId)
    {
        if (!TryGetState(gameState, out D2Civilization3State state))
        {
            return false;
        }
        D2C3ZoneState zone = GetZone(state, zoneId);
        int requiredArchiveLevel = zoneId == Zone3Id ? 4 : 3;
        return state.archiveLevel >= requiredArchiveLevel && zone != null &&
            zone.unlocked && zone.anomalyRevealed &&
            !zone.anomalyRead &&
            state.ancientKnowledge >= GetEffectiveAnomalyKnowledgeCost(state, zoneId) &&
            zone.zoneResourceAmount >= GetEffectiveAnomalyResourceCost(state, zoneId);
    }

    public static bool TryReadAnomaly(GameState gameState, string zoneId)
    {
        if (!CanReadAnomaly(gameState, zoneId))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2C3ZoneState zone = GetZone(state, zoneId);
        state.ancientKnowledge -= GetEffectiveAnomalyKnowledgeCost(state, zoneId);
        zone.zoneResourceAmount -= GetEffectiveAnomalyResourceCost(state, zoneId);
        zone.anomalyRead = true;
        zone.anomalousData = SaturatingAdd(zone.anomalousData, 1L);
        state.lastResult = GetAnomalyName(zoneId) + " leída y archivada. +1 " +
            GetAnomalousDataName(zoneId) + ".";
        return true;
    }

    public static bool HasAllAnomalousData(D2Civilization3State state)
    {
        D2C3ZoneState zone1 = GetZone(state, Zone1Id);
        D2C3ZoneState zone2 = GetZone(state, Zone2Id);
        D2C3ZoneState zone3 = GetZone(state, Zone3Id);
        return zone1 != null && zone2 != null && zone3 != null &&
            zone1.anomalousData > 0L && zone2.anomalousData > 0L &&
            zone3.anomalousData > 0L;
    }

    public static bool CanStartEntityResearch(GameState gameState)
    {
        if (!TryGetState(gameState, out D2Civilization3State state))
            return false;
        return state.entityResearchUnlocked && !state.entityResearchActive &&
            !state.entityResearchMilestone100Completed &&
            !IsAtUnpaidEntityResearchMilestone(state);
    }

    public static bool TryStartEntityResearch(GameState gameState)
    {
        if (!CanStartEntityResearch(gameState))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        state.entityResearchActive = true;
        state.lastResult = state.ancientKnowledge > 0.0
            ? "Investigación del Ente iniciada."
            : "Investigación del Ente iniciada; espera Conocimiento Antiguo.";
        return true;
    }

    public static bool TryPauseEntityResearch(GameState gameState)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            !state.entityResearchActive)
        {
            return false;
        }
        state.entityResearchActive = false;
        state.lastResult = "Investigación del Ente pausada.";
        return true;
    }

    public static double GetPendingEntityResearchMilestone(D2Civilization3State state)
    {
        if (state == null || !state.entityResearchMilestone30Completed)
            return EntityResearchMilestone30;
        if (!state.entityResearchMilestone60Completed)
            return EntityResearchMilestone60;
        if (!state.entityResearchMilestone85Completed)
            return EntityResearchMilestone85;
        return EntityResearchMilestone100;
    }

    public static bool CanPayEntityResearchMilestone(GameState gameState)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            !state.entityResearchUnlocked || !IsAtUnpaidEntityResearchMilestone(state))
        {
            return false;
        }
        D2C3ZoneState zone1 = GetZone(state, Zone1Id);
        D2C3ZoneState zone2 = GetZone(state, Zone2Id);
        D2C3ZoneState zone3 = GetZone(state, Zone3Id);
        if (!state.entityResearchMilestone30Completed)
            return zone1.zoneResourceAmount >= EntityMilestone30ZoneResourceCost &&
                zone1.anomalousData >= 1L;
        if (!state.entityResearchMilestone60Completed)
            return zone2.zoneResourceAmount >= EntityMilestone60ZoneResourceCost &&
                zone2.anomalousData >= 1L;
        if (!state.entityResearchMilestone85Completed)
            return zone3.zoneResourceAmount >= EntityMilestone85ZoneResourceCost &&
                zone3.anomalousData >= 1L;
        return zone1.zoneResourceAmount >= EntityMilestone100EachZoneResourceCost &&
            zone2.zoneResourceAmount >= EntityMilestone100EachZoneResourceCost &&
            zone3.zoneResourceAmount >= EntityMilestone100EachZoneResourceCost;
    }

    public static bool TryPayEntityResearchMilestone(GameState gameState)
    {
        if (!CanPayEntityResearchMilestone(gameState))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2C3ZoneState zone1 = GetZone(state, Zone1Id);
        D2C3ZoneState zone2 = GetZone(state, Zone2Id);
        D2C3ZoneState zone3 = GetZone(state, Zone3Id);
        if (!state.entityResearchMilestone30Completed)
        {
            zone1.zoneResourceAmount -= EntityMilestone30ZoneResourceCost;
            zone1.anomalousData--;
            state.entityResearchMilestone30Completed = true;
            state.entityKnowledge = SaturatingAdd(
                state.entityKnowledge, EntityMilestone30KnowledgeReward);
            state.lastResult = "Hito 30% completado. +1 Conocimiento del Ente.";
        }
        else if (!state.entityResearchMilestone60Completed)
        {
            zone2.zoneResourceAmount -= EntityMilestone60ZoneResourceCost;
            zone2.anomalousData--;
            state.entityResearchMilestone60Completed = true;
            state.entityKnowledge = SaturatingAdd(
                state.entityKnowledge, EntityMilestone60KnowledgeReward);
            state.lastResult = "Hito 60% completado. +2 Conocimiento del Ente.";
        }
        else if (!state.entityResearchMilestone85Completed)
        {
            zone3.zoneResourceAmount -= EntityMilestone85ZoneResourceCost;
            zone3.anomalousData--;
            state.entityResearchMilestone85Completed = true;
            state.entityKnowledge = SaturatingAdd(
                state.entityKnowledge, EntityMilestone85KnowledgeReward);
            state.lastResult = "Hito 85% completado. +3 Conocimiento del Ente.";
        }
        else
        {
            zone1.zoneResourceAmount -= EntityMilestone100EachZoneResourceCost;
            zone2.zoneResourceAmount -= EntityMilestone100EachZoneResourceCost;
            zone3.zoneResourceAmount -= EntityMilestone100EachZoneResourceCost;
            state.entityResearchMilestone100Completed = true;
            state.entityPactAvailable = true;
            state.lastResult = "Investigación completada. El Pacto con el Ente está preparado.";
        }
        state.entityResearchActive = false;
        return true;
    }

    public static bool CanEstablishEntityPact(GameState gameState)
    {
        return TryGetState(gameState, out D2Civilization3State state) &&
            state.entityPactAvailable && !state.entityPactEstablished;
    }

    public static bool TryEstablishEntityPact(GameState gameState)
    {
        if (!CanEstablishEntityPact(gameState))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        state.entityPactEstablished = true;
        state.lastEntityPactResult =
            "Pacto con el Ente establecido. Sus cinco líneas ya pueden desarrollarse.";
        state.lastResult = state.lastEntityPactResult;
        return true;
    }

    public static int GetEntityPactLineLevel(D2Civilization3State state, string lineId)
    {
        return Math.Clamp(FindEntityPactLine(state, lineId)?.level ?? 0,
            0, MaxEntityPactLineLevel);
    }

    public static bool CanUpgradeEntityPactLine(GameState gameState, string lineId)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            !state.entityPactEstablished || !IsEntityPactLineId(lineId))
        {
            return false;
        }
        int nextLevel = GetEntityPactLineLevel(state, lineId) + 1;
        if (nextLevel > MaxEntityPactLineLevel ||
            state.entityKnowledge < GetEntityPactKnowledgeRequirement(nextLevel))
        {
            return false;
        }
        long resourceCost = GetEntityPactZoneResourceCost(nextLevel);
        return state.ancientKnowledge >= GetEntityPactAncientKnowledgeCost(nextLevel) &&
            GetZone(state, Zone1Id).zoneResourceAmount >= resourceCost &&
            GetZone(state, Zone2Id).zoneResourceAmount >= resourceCost &&
            GetZone(state, Zone3Id).zoneResourceAmount >= resourceCost;
    }

    public static bool TryUpgradeEntityPactLine(GameState gameState, string lineId)
    {
        if (!CanUpgradeEntityPactLine(gameState, lineId))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2EntityPactLineState line = FindEntityPactLine(state, lineId);
        int nextLevel = line.level + 1;
        long resourceCost = GetEntityPactZoneResourceCost(nextLevel);
        state.ancientKnowledge -= GetEntityPactAncientKnowledgeCost(nextLevel);
        foreach (string zoneId in ZoneIds)
            GetZone(state, zoneId).zoneResourceAmount -= resourceCost;
        line.level = nextLevel;
        state.lastEntityPactResult = GetEntityPactLineName(lineId) +
            " mejorada a nivel " + nextLevel + ".";
        state.lastResult = state.lastEntityPactResult;
        return true;
    }

    public static double GetEntityPactAncientKnowledgeCost(int nextLevel)
    {
        return 50.0 * Math.Clamp(nextLevel, 1, MaxEntityPactLineLevel);
    }

    public static long GetEntityPactZoneResourceCost(int nextLevel)
    {
        return 25L * Math.Clamp(nextLevel, 1, MaxEntityPactLineLevel);
    }

    public static long GetEntityPactKnowledgeRequirement(int nextLevel)
    {
        switch (Math.Clamp(nextLevel, 1, MaxEntityPactLineLevel))
        {
            case 1: return 1L;
            case 2: return 3L;
            default: return 6L;
        }
    }

    public static string GetEntityPactLineName(string lineId)
    {
        if (lineId == ResonantExpeditionLineId) return "Expedición Resonante";
        if (lineId == EndlessArchiveLineId) return "Archivo Inagotable";
        if (lineId == SharedMemoryLineId) return "Memoria Compartida";
        if (lineId == ModulatorResonanceLineId) return "Resonancia del Modulador";
        return lineId == FirstThresholdChronicleLineId
            ? "Crónica del Primer Umbral"
            : "Línea desconocida";
    }

    public static string GetEntityPactLineDescription(string lineId)
    {
        if (lineId == ResonantExpeditionLineId)
            return "+10% acumulación de restos adicionales por nivel";
        if (lineId == EndlessArchiveLineId)
            return "+10% Conocimiento Antiguo y recursos de zona por análisis por nivel";
        if (lineId == SharedMemoryLineId)
            return "+3% resultados positivos repetibles de Civilizaciones 1 y 2 por nivel";
        if (lineId == ModulatorResonanceLineId)
            return "+5% velocidad de calibración del Modulador por nivel";
        if (lineId == FirstThresholdChronicleLineId)
            return "+1 punto de vista previa de Prestigio 1 por nivel";
        return "Sin efecto";
    }

    public static double GetModulatorCalibrationMultiplier(GameState gameState)
    {
        D2Civilization3State state = gameState?.dimension2?.civilization3;
        if (state == null || !state.entityPactEstablished)
            return 1.0;
        return 1.0 + GetEntityPactLineLevel(state, ModulatorResonanceLineId) *
            ModulatorCalibrationBonusPerLevel;
    }

    public static int GetPrestige1PreviewBonus(GameState gameState)
    {
        D2Civilization3State state = gameState?.dimension2?.civilization3;
        if (state == null || !state.entityPactEstablished)
            return 0;
        return GetEntityPactLineLevel(state, FirstThresholdChronicleLineId) *
            Prestige1PreviewBonusPerLevel;
    }

    public static double GetSharedMemoryMultiplier(GameState gameState)
    {
        D2Civilization3State state = gameState?.dimension2?.civilization3;
        if (state == null || !state.entityPactEstablished)
            return 1.0;
        return 1.0 + GetEntityPactLineLevel(state, SharedMemoryLineId) *
            SharedMemoryBonusPerLevel;
    }

    public static double GetAnalysisRewardMultiplier(D2Civilization3State state)
    {
        double bonus = 0.0;
        if (state != null && state.entityPactEstablished)
            bonus += GetEntityPactLineLevel(state, EndlessArchiveLineId) *
                EndlessArchiveBonusPerLevel;
        D2C3ZoneState zone3 = GetZone(state, Zone3Id);
        if (zone3 != null && zone3.researchProgress >= 100.0)
            bonus += Zone3CompletionRewardBonus;
        return 1.0 + bonus;
    }

    public static double GetExtraRemainsProgressPerExcavation(
        D2Civilization3State state,
        D2C3ZoneState zone
    )
    {
        double progress = zone != null && zone.researchProgress >= BonusRemainsMilestone
            ? BonusRemainsPerExcavation
            : 0.0;
        D2C3ZoneState zone1 = GetZone(state, Zone1Id);
        if (zone1 != null && zone1.researchProgress >= 100.0)
            progress += Zone1CompletionExtraRemainsBonus;
        if (state != null && state.entityPactEstablished)
            progress += GetEntityPactLineLevel(state, ResonantExpeditionLineId) *
                ResonantExpeditionBonusPerLevel;
        return progress;
    }

    public static long GetTotalRemains(D2C3ZoneState zone)
    {
        if (zone == null)
            return 0L;
        return SaturatingAdd(
            SaturatingAdd(zone.lowQualityRemains, zone.mediumQualityRemains),
            zone.highQualityRemains
        );
    }

    public static double GetAnalysisDuration(D2C3ZoneState zone)
    {
        return zone != null && zone.researchProgress >= AnalysisSpeedMilestone
            ? AnalysisDurationSeconds * AnalysisSpeedMultiplier
            : AnalysisDurationSeconds;
    }

    public static double GetAnalysisDuration(
        D2Civilization3State state,
        D2C3ZoneState zone
    )
    {
        double duration = GetAnalysisDuration(zone);
        D2C3ZoneState zone2 = GetZone(state, Zone2Id);
        if (zone2 != null && zone2.researchProgress >= 100.0)
            duration *= Zone2CompletionAnalysisDurationMultiplier;
        duration *= GetScholarAnalysisDurationMultiplier(zone);
        return duration;
    }

    public static double GetExcavationDuration(D2Civilization3State state)
    {
        return state != null && state.stratifiedCartographyUnlocked
            ? ExcavationDurationSeconds * (1.0 - StratifiedCartographyExcavationSpeedBonus)
            : ExcavationDurationSeconds;
    }

    public static double GetScholarAnalysisDurationMultiplier(D2C3ZoneState zone)
    {
        int bonusLevels = Math.Max(0, (zone?.scholarLevel ?? 0) - 1);
        return Math.Max(0.1, 1.0 - bonusLevels * ScholarAnalysisSpeedBonusPerLevel);
    }

    public static double GetScholarRewardMultiplier(D2C3ZoneState zone)
    {
        int bonusLevels = Math.Max(0, (zone?.scholarLevel ?? 0) - 1);
        return 1.0 + bonusLevels * ScholarRewardBonusPerLevel;
    }

    public static double GetAncientKnowledgeReward(string qualityId, D2C3ZoneState zone)
    {
        double reward = qualityId == HighQualityId ? 8.0 :
            qualityId == MediumQualityId ? 3.0 : 1.0;
        if (zone != null && zone.researchProgress >= KnowledgeBonusMilestone)
            reward *= KnowledgeBonusMultiplier;
        return reward;
    }

    public static double GetAncientKnowledgeReward(
        D2Civilization3State state,
        string qualityId,
        D2C3ZoneState zone
    )
    {
        return GetAncientKnowledgeReward(qualityId, zone) *
            GetAnalysisRewardMultiplier(state) * GetScholarRewardMultiplier(zone);
    }

    public static long GetZoneResourceReward(string qualityId)
    {
        return qualityId == HighQualityId ? 4L : qualityId == MediumQualityId ? 2L : 1L;
    }

    public static double GetResearchReward(string qualityId)
    {
        return qualityId == HighQualityId ? 8.0 : qualityId == MediumQualityId ? 3.0 : 1.0;
    }

    public static double GetClueProgressReward(string qualityId)
    {
        return qualityId == HighQualityId ? 0.18 :
            qualityId == MediumQualityId ? 0.08 : 0.03;
    }

    public static string GetQualityForRoll(string zoneId, double roll)
    {
        roll = Math.Clamp(roll, 0.0, 0.999999999);
        double lowThreshold;
        double mediumThreshold;
        switch (zoneId)
        {
            case Zone2Id: lowThreshold = 0.50; mediumThreshold = 0.85; break;
            case Zone3Id: lowThreshold = 0.30; mediumThreshold = 0.75; break;
            default: lowThreshold = 0.70; mediumThreshold = 0.95; break;
        }
        if (roll < lowThreshold) return LowQualityId;
        return roll < mediumThreshold ? MediumQualityId : HighQualityId;
    }

    public static bool TryStartExcavation(GameState gameState, string zoneId)
    {
        if (!TryGetState(gameState, out D2Civilization3State state))
            return false;
        D2C3ZoneState zone = GetZone(state, zoneId);
        if (zone == null || !zone.unlocked || zone.excavationActive)
            return false;
        zone.excavationActive = true;
        zone.excavationRemainingSeconds = GetExcavationDuration(state);
        state.lastResult = "Excavación iniciada en " + GetZoneName(zoneId) + ".";
        return true;
    }

    public static bool CanUnlockZone2(GameState gameState)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            gameState.dimension2.civilization1 == null)
        {
            return false;
        }
        D2C3ZoneState zone1 = GetZone(state, Zone1Id);
        D2C3ZoneState zone2 = GetZone(state, Zone2Id);
        D2AltarState incense = GetAltar(gameState, D2AltarSystem.IncenseAltarId);
        D2AltarState cloth = GetAltar(gameState, D2AltarSystem.SacredClothAltarId);
        return zone1 != null && zone2 != null && !zone2.unlocked &&
            zone1.researchProgress >= Zone2UnlockResearchRequirement &&
            incense != null && cloth != null &&
            incense.offeringAmount >= Zone2IncenseCost &&
            cloth.offeringAmount >= Zone2SacredClothCost;
    }

    public static bool TryUnlockZone2(GameState gameState)
    {
        if (!CanUnlockZone2(gameState))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        GetAltar(gameState, D2AltarSystem.IncenseAltarId).offeringAmount -= Zone2IncenseCost;
        GetAltar(gameState, D2AltarSystem.SacredClothAltarId).offeringAmount -=
            Zone2SacredClothCost;
        GetZone(state, Zone2Id).unlocked = true;
        state.selectedZoneId = Zone2Id;
        state.lastResult = "Zona 2 destapada: Galería de Inscripciones.";
        return true;
    }

    public static bool CanUnlockZone3(GameState gameState)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            gameState.dimension2.civilization1 == null)
        {
            return false;
        }
        D2C3ZoneState zone2 = GetZone(state, Zone2Id);
        D2C3ZoneState zone3 = GetZone(state, Zone3Id);
        D2AltarState incense = GetAltar(gameState, D2AltarSystem.IncenseAltarId);
        D2AltarState cloth = GetAltar(gameState, D2AltarSystem.SacredClothAltarId);
        D2AltarState stone = GetAltar(gameState, D2AltarSystem.CarvedStoneAltarId);
        return zone2 != null && zone2.unlocked && zone3 != null && !zone3.unlocked &&
            zone2.researchProgress >= Zone3UnlockResearchRequirement &&
            incense != null && cloth != null && stone != null &&
            incense.offeringAmount >= Zone3IncenseCost &&
            cloth.offeringAmount >= Zone3SacredClothCost &&
            stone.offeringAmount >= Zone3CarvedStoneCost;
    }

    public static bool TryUnlockZone3(GameState gameState)
    {
        if (!CanUnlockZone3(gameState))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        GetAltar(gameState, D2AltarSystem.IncenseAltarId).offeringAmount -= Zone3IncenseCost;
        GetAltar(gameState, D2AltarSystem.SacredClothAltarId).offeringAmount -=
            Zone3SacredClothCost;
        GetAltar(gameState, D2AltarSystem.CarvedStoneAltarId).offeringAmount -=
            Zone3CarvedStoneCost;
        GetZone(state, Zone3Id).unlocked = true;
        state.selectedZoneId = Zone3Id;
        state.lastResult = "Zona 3 destapada: Santuario Sellado.";
        return true;
    }

    public static bool CanStartAnalysis(GameState gameState, string zoneId, string qualityId)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            !IsQualityId(qualityId))
        {
            return false;
        }
        D2C3ZoneState zone = GetZone(state, zoneId);
        return zone != null && zone.unlocked && zone.scholarHired &&
            !zone.analysisActive && GetRemains(zone, qualityId) > 0L;
    }

    public static bool TryStartAnalysis(GameState gameState, string zoneId, string qualityId)
    {
        if (!CanStartAnalysis(gameState, zoneId, qualityId))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2C3ZoneState zone = GetZone(state, zoneId);
        RemoveRemain(zone, qualityId);
        zone.analysisActive = true;
        zone.analysisQualityId = qualityId;
        zone.analysisRemainingSeconds = GetAnalysisDuration(state, zone);
        state.lastResult = "Análisis iniciado con un resto de calidad " +
            GetQualityName(qualityId).ToLowerInvariant() + ".";
        return true;
    }

    public static bool CanHireScholar(GameState gameState, string zoneId)
    {
        if (gameState?.dimension2?.civilization1 == null ||
            gameState.dimension2.civilization3 == null)
        {
            return false;
        }
        D2C3ZoneState zone = GetZone(gameState.dimension2.civilization3, zoneId);
        if (zone == null || !zone.unlocked || zone.scholarHired)
            return false;
        D2AltarState wax = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.WaxAltarId
        );
        D2AltarState bread = D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.RitualBreadAltarId
        );
        double waxCost = GetScholarWaxCost(zoneId);
        double breadCost = GetScholarBreadCost(zoneId);
        return wax != null && bread != null && wax.offeringAmount >= waxCost &&
            bread.offeringAmount >= breadCost;
    }

    public static bool TryHireScholar(GameState gameState, string zoneId)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            !CanHireScholar(gameState, zoneId))
        {
            return false;
        }
        double waxCost = GetScholarWaxCost(zoneId);
        double breadCost = GetScholarBreadCost(zoneId);
        D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.WaxAltarId
        ).offeringAmount -= waxCost;
        D2AltarSystem.GetAltar(
            gameState.dimension2.civilization1,
            D2AltarSystem.RitualBreadAltarId
        ).offeringAmount -= breadCost;
        GetZone(state, zoneId).scholarHired = true;
        GetZone(state, zoneId).scholarLevel = 1;
        state.lastResult = GetScholarName(zoneId) + " contratado.";
        return true;
    }

    public static double GetScholarUpgradeKnowledgeCost(string zoneId, int nextLevel)
    {
        int levelIndex = Math.Clamp(nextLevel, 2, MaxScholarLevel) - 2;
        if (zoneId == Zone3Id)
            return levelIndex == 0 ? 60.0 : 120.0;
        if (zoneId == Zone2Id)
            return levelIndex == 0 ? 45.0 : 90.0;
        return levelIndex == 0 ? 30.0 : 60.0;
    }

    public static long GetScholarUpgradeResourceCost(string zoneId, int nextLevel)
    {
        int levelIndex = Math.Clamp(nextLevel, 2, MaxScholarLevel) - 2;
        if (zoneId == Zone3Id)
            return levelIndex == 0 ? 40L : 80L;
        if (zoneId == Zone2Id)
            return levelIndex == 0 ? 30L : 60L;
        return levelIndex == 0 ? 20L : 40L;
    }

    public static long GetScholarUpgradeEntityKnowledgeRequirement(int nextLevel)
    {
        return nextLevel >= 3 ? 6L : 3L;
    }

    public static bool CanUpgradeScholar(GameState gameState, string zoneId)
    {
        if (!TryGetState(gameState, out D2Civilization3State state))
            return false;
        D2C3ZoneState zone = GetZone(state, zoneId);
        if (zone == null || !zone.unlocked || !zone.scholarHired ||
            zone.scholarLevel >= MaxScholarLevel)
        {
            return false;
        }
        int nextLevel = zone.scholarLevel + 1;
        return state.entityKnowledge >= GetScholarUpgradeEntityKnowledgeRequirement(nextLevel) &&
            state.ancientKnowledge >= GetScholarUpgradeKnowledgeCost(zoneId, nextLevel) &&
            zone.zoneResourceAmount >= GetScholarUpgradeResourceCost(zoneId, nextLevel);
    }

    public static bool TryUpgradeScholar(GameState gameState, string zoneId)
    {
        if (!CanUpgradeScholar(gameState, zoneId))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2C3ZoneState zone = GetZone(state, zoneId);
        int nextLevel = zone.scholarLevel + 1;
        state.ancientKnowledge -= GetScholarUpgradeKnowledgeCost(zoneId, nextLevel);
        zone.zoneResourceAmount -= GetScholarUpgradeResourceCost(zoneId, nextLevel);
        zone.scholarLevel = nextLevel;
        state.lastResult = GetScholarName(zoneId) + " mejorado a nivel " + nextLevel + ".";
        return true;
    }

    public static string GetArchiveUpgradeName(string upgradeId)
    {
        if (upgradeId == StratifiedCartographyUpgradeId)
            return "Cartografía Estratificada";
        if (upgradeId == AnomalousConcordanceUpgradeId)
            return "Concordancia Anómala";
        return upgradeId == DeepExegesisUpgradeId
            ? "Exégesis Profunda"
            : "Mejora desconocida";
    }

    public static string GetArchiveUpgradeDescription(string upgradeId)
    {
        if (upgradeId == StratifiedCartographyUpgradeId)
            return "+5% velocidad de excavación en las tres zonas";
        if (upgradeId == AnomalousConcordanceUpgradeId)
            return "+10% acumulación de Indicios Anómalos";
        return upgradeId == DeepExegesisUpgradeId
            ? "-10% costes de lectura de Anomalías"
            : "Sin efecto";
    }

    public static int GetArchiveUpgradeRequiredLevel(string upgradeId)
    {
        if (upgradeId == StratifiedCartographyUpgradeId) return 2;
        return upgradeId == AnomalousConcordanceUpgradeId ? 3 : 4;
    }

    public static long GetArchiveUpgradeEntityKnowledgeRequirement(string upgradeId)
    {
        if (upgradeId == StratifiedCartographyUpgradeId) return 1L;
        return upgradeId == AnomalousConcordanceUpgradeId ? 3L : 6L;
    }

    public static double GetArchiveUpgradeKnowledgeCost(string upgradeId)
    {
        if (upgradeId == StratifiedCartographyUpgradeId) return 50.0;
        return upgradeId == AnomalousConcordanceUpgradeId ? 75.0 : 100.0;
    }

    public static long GetArchiveUpgradeResourceCost(string upgradeId)
    {
        if (upgradeId == StratifiedCartographyUpgradeId) return 25L;
        return upgradeId == AnomalousConcordanceUpgradeId ? 35L : 50L;
    }

    public static string GetArchiveUpgradeZoneId(string upgradeId)
    {
        if (upgradeId == StratifiedCartographyUpgradeId) return Zone1Id;
        return upgradeId == AnomalousConcordanceUpgradeId ? Zone2Id : Zone3Id;
    }

    public static bool IsArchiveUpgradeUnlocked(
        D2Civilization3State state,
        string upgradeId
    )
    {
        if (state == null)
            return false;
        if (upgradeId == StratifiedCartographyUpgradeId)
            return state.stratifiedCartographyUnlocked;
        if (upgradeId == AnomalousConcordanceUpgradeId)
            return state.anomalousConcordanceUnlocked;
        return upgradeId == DeepExegesisUpgradeId && state.deepExegesisUnlocked;
    }

    public static bool CanUnlockArchiveUpgrade(GameState gameState, string upgradeId)
    {
        if (!TryGetState(gameState, out D2Civilization3State state) ||
            Array.IndexOf(ArchiveUpgradeIds, upgradeId) < 0 ||
            IsArchiveUpgradeUnlocked(state, upgradeId) ||
            state.archiveLevel < GetArchiveUpgradeRequiredLevel(upgradeId) ||
            state.entityKnowledge < GetArchiveUpgradeEntityKnowledgeRequirement(upgradeId) ||
            state.ancientKnowledge < GetArchiveUpgradeKnowledgeCost(upgradeId))
        {
            return false;
        }
        D2C3ZoneState zone = GetZone(state, GetArchiveUpgradeZoneId(upgradeId));
        return zone != null && zone.unlocked &&
            zone.zoneResourceAmount >= GetArchiveUpgradeResourceCost(upgradeId);
    }

    public static bool TryUnlockArchiveUpgrade(GameState gameState, string upgradeId)
    {
        if (!CanUnlockArchiveUpgrade(gameState, upgradeId))
            return false;
        D2Civilization3State state = gameState.dimension2.civilization3;
        D2C3ZoneState zone = GetZone(state, GetArchiveUpgradeZoneId(upgradeId));
        state.ancientKnowledge -= GetArchiveUpgradeKnowledgeCost(upgradeId);
        zone.zoneResourceAmount -= GetArchiveUpgradeResourceCost(upgradeId);
        if (upgradeId == StratifiedCartographyUpgradeId)
            state.stratifiedCartographyUnlocked = true;
        else if (upgradeId == AnomalousConcordanceUpgradeId)
            state.anomalousConcordanceUnlocked = true;
        else
            state.deepExegesisUnlocked = true;
        state.lastResult = GetArchiveUpgradeName(upgradeId) + " desbloqueada.";
        return true;
    }

    public static bool CanHireFieldScholar(GameState gameState)
    {
        return CanHireScholar(gameState, Zone1Id);
    }

    public static bool TryHireFieldScholar(GameState gameState)
    {
        return TryHireScholar(gameState, Zone1Id);
    }

    public static bool ValidateState(D2Civilization3State state, out string result)
    {
        if (state == null)
        {
            result = "Estado de Civilización 3 ausente.";
            return false;
        }
        EnsureState(state);
        if (state.zones == null || state.zones.Count != ZoneIds.Length ||
            !IsSelectableZone(state, state.selectedZoneId))
        {
            result = "Estado base de Civilización 3 inválido.";
            return false;
        }
        foreach (string zoneId in ZoneIds)
        {
            D2C3ZoneState zone = GetZone(state, zoneId);
            if (zone == null || zone.lowQualityRemains < 0L ||
                zone.mediumQualityRemains < 0L || zone.highQualityRemains < 0L ||
                zone.totalExcavationsCompleted < 0L ||
                zone.zoneResourceAmount < 0L || zone.researchProgress < 0.0 ||
                zone.zoneResourceRewardProgress < 0.0 ||
                zone.zoneResourceRewardProgress >= 1.0 ||
                zone.researchProgress > 100.0 || zone.totalAnalysesCompleted < 0L ||
                zone.bonusRemainsProgress < 0.0 || zone.bonusRemainsProgress >= 1.0 ||
                zone.anomalyClues < 0L || zone.anomalyClueProgress < 0.0 ||
                zone.anomalyClueProgress >= 1.0 ||
                zone.anomalousData < 0L || (zone.anomalyRead && !zone.anomalyRevealed) ||
                (zone.scholarHired &&
                    (zone.scholarLevel < 1 || zone.scholarLevel > MaxScholarLevel)) ||
                (!zone.scholarHired && zone.scholarLevel != 0) ||
                zone.excavationRemainingSeconds < 0.0 ||
                zone.excavationRemainingSeconds > GetExcavationDuration(state) ||
                (!zone.excavationActive && zone.excavationRemainingSeconds != 0.0) ||
                zone.analysisRemainingSeconds < 0.0 ||
                zone.analysisRemainingSeconds > AnalysisDurationSeconds ||
                (!zone.analysisActive &&
                    (zone.analysisRemainingSeconds != 0.0 ||
                     !string.IsNullOrEmpty(zone.analysisQualityId))) ||
                (zone.analysisActive && !IsQualityId(zone.analysisQualityId)))
            {
                result = "Estado de zona arqueológica inválido.";
                return false;
            }
        }
        if (state.ancientKnowledge < 0.0 ||
            (state.archiveUnlocked && state.archiveLevel < 1) ||
            (!state.archiveUnlocked && state.archiveLevel != 0) ||
            state.entityResearchProgress < 0.0 ||
            state.entityResearchProgress > EntityResearchMilestone100 ||
            state.entityKnowledge < 0L ||
            (state.entityResearchMilestone100Completed && !state.entityPactAvailable) ||
            (state.entityPactEstablished && !state.entityPactAvailable) ||
            state.entityPactLines == null ||
            state.entityPactLines.Count != EntityPactLineIds.Length ||
            (state.stratifiedCartographyUnlocked && state.archiveLevel < 2) ||
            (state.anomalousConcordanceUnlocked && state.archiveLevel < 3) ||
            (state.deepExegesisUnlocked && state.archiveLevel < 4))
        {
            result = "Estado de recursos o Archivo inválido.";
            return false;
        }
        if (!GetZone(state, Zone1Id).unlocked ||
            (GetZone(state, Zone3Id).unlocked && !GetZone(state, Zone2Id).unlocked))
        {
            result = "Desbloqueos iniciales de Civilización 3 inválidos.";
            return false;
        }
        result = "Estado de Civilización 3 válido.";
        return true;
    }

    private static void AdvanceExcavation(
        D2Civilization3State state,
        D2C3ZoneState zone,
        double seconds
    )
    {
        if (zone == null || !zone.unlocked || !zone.excavationActive)
            return;
        double remaining = seconds;
        int guard = 0;
        while (remaining > 0.000001 && zone.excavationActive && guard++ < 100000)
        {
            double step = Math.Min(remaining, zone.excavationRemainingSeconds);
            zone.excavationRemainingSeconds -= step;
            remaining -= step;
            if (zone.excavationRemainingSeconds > 0.000001)
                continue;
            CompleteExcavation(state, zone, UnityEngine.Random.value);
            zone.excavationActive = false;
        }
    }

    private static void CompleteExcavation(
        D2Civilization3State state,
        D2C3ZoneState zone,
        double roll
    )
    {
        string quality = GetQualityForRoll(zone.zoneId, roll);
        AddRemain(zone, quality, 1L);
        bool bonusRemain = false;
        double bonusProgress = GetExtraRemainsProgressPerExcavation(state, zone);
        if (bonusProgress > 0.0)
        {
            zone.bonusRemainsProgress += bonusProgress;
            while (zone.bonusRemainsProgress >= 1.0)
            {
                zone.bonusRemainsProgress -= 1.0;
                AddRemain(zone, quality, 1L);
                bonusRemain = true;
            }
        }
        zone.totalExcavationsCompleted = SaturatingAdd(zone.totalExcavationsCompleted, 1L);
        state.lastResult = "Excavación completada en " + GetZoneName(zone.zoneId) +
            ": resto de calidad " +
            GetQualityName(quality).ToLowerInvariant() +
            (bonusRemain ? " y un resto adicional por el hito de 20%." : ".");
    }

    private static void AdvanceAnalysis(
        D2Civilization3State state,
        D2C3ZoneState zone,
        double seconds
    )
    {
        if (zone == null || !zone.unlocked || !zone.analysisActive)
            return;
        zone.analysisRemainingSeconds -= Math.Min(seconds, zone.analysisRemainingSeconds);
        if (zone.analysisRemainingSeconds > 0.000001)
            return;

        string qualityId = zone.analysisQualityId;
        double knowledge = GetAncientKnowledgeReward(state, qualityId, zone);
        double accumulatedResource = zone.zoneResourceRewardProgress +
            (GetZoneResourceReward(qualityId) * GetAnalysisRewardMultiplier(state) *
             GetScholarRewardMultiplier(zone));
        long resource = SafeFloorToLong(accumulatedResource);
        zone.zoneResourceRewardProgress = accumulatedResource - resource;
        double research = GetResearchReward(qualityId);
        state.ancientKnowledge = SafeAdd(state.ancientKnowledge, knowledge);
        zone.zoneResourceAmount = SaturatingAdd(zone.zoneResourceAmount, resource);
        zone.researchProgress = Math.Min(100.0, zone.researchProgress + research);
        zone.totalAnalysesCompleted = SaturatingAdd(zone.totalAnalysesCompleted, 1L);
        zone.analysisActive = false;
        zone.analysisRemainingSeconds = 0.0;
        zone.analysisQualityId = "";
        if (!state.archiveUnlocked)
        {
            state.archiveUnlocked = true;
            state.archiveLevel = 1;
        }
        if (zone.zoneId == Zone1Id && zone.researchProgress >= 40.0)
        {
            state.archiveUnlocked = true;
            state.archiveLevel = Math.Max(2, state.archiveLevel);
        }
        D2C3ZoneState zone2 = GetZone(state, Zone2Id);
        if (zone2 != null && zone2.unlocked &&
            zone2.researchProgress >= ClueDetectionResearchRequirement)
        {
            state.anomalyClueDetectionUnlocked = true;
        }
        if (zone2 != null && zone2.unlocked &&
            zone2.researchProgress >= ArchiveLevel3ResearchRequirement)
        {
            state.archiveUnlocked = true;
            state.archiveLevel = Math.Max(3, state.archiveLevel);
        }
        D2C3ZoneState zone3 = GetZone(state, Zone3Id);
        if (zone3 != null && zone3.unlocked &&
            zone3.researchProgress >= ArchiveLevel4ResearchRequirement)
        {
            state.archiveUnlocked = true;
            state.archiveLevel = Math.Max(4, state.archiveLevel);
        }
        bool anomalyWasRevealed = zone.anomalyRevealed;
        bool clueEarned = AdvanceClueProgress(state, zone, qualityId);
        state.lastResult = "Análisis completado: +" + knowledge.ToString("0.##") +
            " Conocimiento Antiguo, +" + resource + " " +
            GetZoneResourceName(zone.zoneId) + " y +" + research.ToString("0.##") +
            "% de Investigación." + (clueEarned
                ? " +1 " + GetClueName(zone.zoneId) + "."
                : "") + (!anomalyWasRevealed && zone.anomalyRevealed
                    ? " " + GetAnomalyName(zone.zoneId) + " revelada."
                    : "");
    }

    private static void AdvanceEntityResearch(D2Civilization3State state, double seconds)
    {
        if (state == null || !state.entityResearchUnlocked ||
            !state.entityResearchActive || state.entityResearchMilestone100Completed ||
            seconds <= 0.0)
        {
            return;
        }
        if (IsAtUnpaidEntityResearchMilestone(state))
        {
            state.entityResearchActive = false;
            return;
        }
        double target = GetPendingEntityResearchMilestone(state);
        double possibleByTime = seconds / EntityResearchSecondsPerPercent;
        double possibleByKnowledge = state.ancientKnowledge /
            EntityResearchKnowledgePerPercent;
        double progress = Math.Min(
            Math.Min(possibleByTime, possibleByKnowledge),
            Math.Max(0.0, target - state.entityResearchProgress));
        if (progress <= 0.0)
            return;
        state.entityResearchProgress += progress;
        state.ancientKnowledge = Math.Max(
            0.0,
            state.ancientKnowledge - progress * EntityResearchKnowledgePerPercent);
        if (state.entityResearchProgress >= target - 0.000001)
        {
            state.entityResearchProgress = target;
            state.entityResearchActive = false;
            state.lastResult = "Investigación detenida en el hito " +
                target.ToString("0") + "%. Completa su requisito para continuar.";
        }
    }

    private static bool IsAtUnpaidEntityResearchMilestone(D2Civilization3State state)
    {
        if (state == null)
            return false;
        double target = GetPendingEntityResearchMilestone(state);
        bool completed = target == EntityResearchMilestone30
            ? state.entityResearchMilestone30Completed
            : target == EntityResearchMilestone60
                ? state.entityResearchMilestone60Completed
                : target == EntityResearchMilestone85
                    ? state.entityResearchMilestone85Completed
                    : state.entityResearchMilestone100Completed;
        return !completed && state.entityResearchProgress >= target - 0.000001;
    }

    private static bool AdvanceClueProgress(
        D2Civilization3State state,
        D2C3ZoneState zone,
        string qualityId
    )
    {
        if (!state.anomalyClueDetectionUnlocked || zone == null || !zone.unlocked)
            return false;
        double archiveMultiplier = state.anomalousConcordanceUnlocked
            ? 1.0 + AnomalousConcordanceClueBonus
            : 1.0;
        zone.anomalyClueProgress += GetClueProgressReward(qualityId) * archiveMultiplier;
        if (zone.anomalyClueProgress < 1.0)
            return false;
        zone.anomalyClueProgress -= 1.0;
        zone.anomalyClues = SaturatingAdd(zone.anomalyClues, 1L);
        if (zone.anomalyClues >= GetAnomalyClueRequirement(zone.zoneId))
            zone.anomalyRevealed = true;
        return true;
    }

    private static long GetRemains(D2C3ZoneState zone, string qualityId)
    {
        if (qualityId == HighQualityId) return zone.highQualityRemains;
        return qualityId == MediumQualityId
            ? zone.mediumQualityRemains
            : zone.lowQualityRemains;
    }

    private static void RemoveRemain(D2C3ZoneState zone, string qualityId)
    {
        if (qualityId == HighQualityId) zone.highQualityRemains--;
        else if (qualityId == MediumQualityId) zone.mediumQualityRemains--;
        else zone.lowQualityRemains--;
    }

    private static void AddRemain(D2C3ZoneState zone, string qualityId, long amount)
    {
        if (qualityId == HighQualityId)
            zone.highQualityRemains = SaturatingAdd(zone.highQualityRemains, amount);
        else if (qualityId == MediumQualityId)
            zone.mediumQualityRemains = SaturatingAdd(zone.mediumQualityRemains, amount);
        else
            zone.lowQualityRemains = SaturatingAdd(zone.lowQualityRemains, amount);
    }

    private static bool IsQualityId(string qualityId)
    {
        return qualityId == LowQualityId || qualityId == MediumQualityId ||
            qualityId == HighQualityId;
    }

    public static string GetQualityName(string qualityId)
    {
        if (qualityId == HighQualityId) return "Alta";
        return qualityId == MediumQualityId ? "Media" : "Baja";
    }

    private static bool TryGetState(GameState gameState, out D2Civilization3State state)
    {
        state = null;
        if (gameState?.dimension2?.civilization3 == null ||
            !gameState.dimension2.civilization3Unlocked)
        {
            return false;
        }
        state = gameState.dimension2.civilization3;
        EnsureState(state);
        return true;
    }

    private static D2AltarState GetAltar(GameState gameState, string altarId)
    {
        return D2AltarSystem.GetAltar(gameState.dimension2.civilization1, altarId);
    }

    private static double GetScholarWaxCost(string zoneId)
    {
        if (zoneId == Zone3Id) return SealsScholarWaxCost;
        return zoneId == Zone2Id ? InscriptionScholarWaxCost : FieldScholarWaxCost;
    }

    private static double GetScholarBreadCost(string zoneId)
    {
        if (zoneId == Zone3Id) return SealsScholarBreadCost;
        return zoneId == Zone2Id ? InscriptionScholarBreadCost : FieldScholarBreadCost;
    }

    private static bool HasDuplicateBefore(D2Civilization3State state, int index)
    {
        string id = state.zones[index].zoneId;
        for (int i = 0; i < index; i++)
            if (state.zones[i] != null && state.zones[i].zoneId == id) return true;
        return false;
    }

    private static void EnsureEntityPactState(D2Civilization3State state)
    {
        if (state.entityPactLines == null)
            state.entityPactLines = new List<D2EntityPactLineState>();
        for (int i = state.entityPactLines.Count - 1; i >= 0; i--)
        {
            D2EntityPactLineState line = state.entityPactLines[i];
            if (line == null || !IsEntityPactLineId(line.lineId) ||
                HasEarlierEntityPactLineDuplicate(state, i))
            {
                state.entityPactLines.RemoveAt(i);
            }
        }
        foreach (string lineId in EntityPactLineIds)
        {
            D2EntityPactLineState line = FindEntityPactLine(state, lineId);
            if (line == null)
            {
                line = new D2EntityPactLineState { lineId = lineId };
                state.entityPactLines.Add(line);
            }
            line.level = Math.Clamp(line.level, 0, MaxEntityPactLineLevel);
            if (!state.entityPactEstablished)
                line.level = 0;
        }
        if (!state.entityPactAvailable)
            state.entityPactEstablished = false;
        if (state.lastEntityPactResult == null)
            state.lastEntityPactResult = "";
    }

    private static bool IsEntityPactLineId(string lineId)
    {
        return Array.IndexOf(EntityPactLineIds, lineId) >= 0;
    }

    private static D2EntityPactLineState FindEntityPactLine(
        D2Civilization3State state,
        string lineId
    )
    {
        if (state?.entityPactLines == null)
            return null;
        foreach (D2EntityPactLineState line in state.entityPactLines)
            if (line != null && line.lineId == lineId) return line;
        return null;
    }

    private static bool HasEarlierEntityPactLineDuplicate(
        D2Civilization3State state,
        int index
    )
    {
        string lineId = state.entityPactLines[index].lineId;
        for (int i = 0; i < index; i++)
            if (state.entityPactLines[i] != null &&
                state.entityPactLines[i].lineId == lineId) return true;
        return false;
    }

    private static double ClampDuration(double value, double fallback)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return fallback;
        return Math.Clamp(value, 0.0, ExcavationDurationSeconds);
    }

    private static double ClampAnalysisDuration(double value, double fallback)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return fallback;
        return Math.Clamp(value, 0.0, AnalysisDurationSeconds);
    }

    private static double ClampNonNegative(double value)
    {
        return double.IsNaN(value) || double.IsInfinity(value) ? 0.0 : Math.Max(0.0, value);
    }

    private static double SafeAdd(double left, double right)
    {
        double result = left + right;
        return double.IsInfinity(result) || double.IsNaN(result) ? double.MaxValue : result;
    }

    private static long SafeFloorToLong(double value)
    {
        if (double.IsNaN(value) || value <= 0.0)
            return 0L;
        if (double.IsInfinity(value) || value >= long.MaxValue)
            return long.MaxValue;
        return (long)Math.Floor(value);
    }

    private static long SaturatingAdd(long left, long right)
    {
        if (right > 0L && left > long.MaxValue - right)
            return long.MaxValue;
        return left + right;
    }
}
