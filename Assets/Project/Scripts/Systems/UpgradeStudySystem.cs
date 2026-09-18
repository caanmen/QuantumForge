using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeStudyCircuitRequirement
{
    None = 0,
    Energy = 1,
    Experimental = 2,
    AnyCoreCircuit = 3,
    TriangleEnergy = 4
}

public enum UpgradeStudyBlockReason
{
    None = 0,
    AlreadyDiscovered,
    AnotherStudyActive,
    StudyAlreadyActive,
    ConclusionPending,
    MissingArtifact,
    MissingVertices,
    TriangleLocked,
    CircuitNotSynchronized,
    CircuitSwitchRequired,
    MissingDiscovery,
    InsufficientEnergy,
    UnknownStudy
}

[Serializable]
public sealed class UpgradeStudyDef
{
    public string id;
    public string unlockId;
    public string requiredBuildingId;
    public bool requiresAllTriangleVertices;
    public bool requiresTriangleUnlocked;
    public UpgradeStudyCircuitRequirement requiredCircuit;
    public int minimumCircuitSwitches;
    public List<string> requiredDiscoveredIds;
    public double durationSeconds;
    public double energyCost;
    public double tuningBoostFraction = 0.18;
    public int tuningStages = 3;
    public bool unlocksKeycardProject;
}

[Serializable]
public sealed class UpgradeStudyDefList
{
    public List<UpgradeStudyDef> studies;
}

[Serializable]
public sealed class UpgradeStudyState
{
    public int saveVersion;
    public List<string> discoveredIds = new();
    public string activeStudyId = "";
    public double activeProgressSeconds;
    public int synchronizedCircuitMask;
    public int circuitSwitchCount;
    public bool hideCompletedUpgrades;
    public int tuningStage;
    public float tuningAmplitude = 0.5f;
    public float tuningFrequency = 0.5f;
}

/// <summary>
/// Descubrimiento de mejoras del juego base mediante estudios de artefactos.
/// No entrega recursos: solo revela filas que después conservan su coste normal.
/// </summary>
public static class UpgradeStudySystem
{
    public const double TuningMinimumChannelAccuracy = 0.90;
    public const double TuningPerfectChannelAccuracy = 0.98;
    public const double TuningPerfectBoostFraction = 0.25;
    public const int CurrentSaveVersion = 2;
    public const string KeycardProjectUnlockId = "project_experimental_chamber_keycard";

    private static readonly Dictionary<string, UpgradeStudyDef> DefsById = new();
    private static readonly Dictionary<string, UpgradeStudyDef> DefsByUnlockId = new();
    private static bool catalogLoaded;

    public static IReadOnlyCollection<UpgradeStudyDef> Definitions
    {
        get
        {
            EnsureCatalog();
            return DefsById.Values;
        }
    }

    public static void ReloadCatalog()
    {
        catalogLoaded = false;
        EnsureCatalog();
    }

    private static void EnsureCatalog()
    {
        if (catalogLoaded) return;
        catalogLoaded = true;
        DefsById.Clear();
        DefsByUnlockId.Clear();

        TextAsset json = Resources.Load<TextAsset>("Data/upgrade_studies");
        if (json == null)
        {
            Debug.LogError("[UpgradeStudySystem] Falta Data/upgrade_studies.json");
            return;
        }

        UpgradeStudyDefList list = JsonUtility.FromJson<UpgradeStudyDefList>(json.text);
        if (list?.studies == null)
        {
            Debug.LogError("[UpgradeStudySystem] Catálogo de estudios inválido.");
            return;
        }

        foreach (UpgradeStudyDef def in list.studies)
        {
            if (def == null || string.IsNullOrWhiteSpace(def.id) ||
                string.IsNullOrWhiteSpace(def.unlockId))
                continue;

            def.durationSeconds = Math.Max(1.0, def.durationSeconds);
            def.energyCost = Math.Max(0.0, def.energyCost);
            def.tuningBoostFraction = Math.Max(0.0,
                Math.Min(0.50, def.tuningBoostFraction));
            def.tuningStages = Mathf.Clamp(def.tuningStages, 0, 5);
            def.requiredDiscoveredIds ??= new List<string>();
            DefsById[def.id] = def;
            DefsByUnlockId[def.unlockId] = def;
        }
    }

    public static UpgradeStudyState EnsureState(GameState state)
    {
        if (state == null) return null;
        state.upgradeStudies ??= new UpgradeStudyState();
        state.upgradeStudies.discoveredIds ??= new List<string>();
        state.upgradeStudies.activeStudyId ??= "";
        state.upgradeStudies.activeProgressSeconds = Math.Max(
            0.0, state.upgradeStudies.activeProgressSeconds);
        state.upgradeStudies.synchronizedCircuitMask &= 0x7;
        state.upgradeStudies.circuitSwitchCount = Math.Max(
            0, state.upgradeStudies.circuitSwitchCount);
        state.upgradeStudies.tuningStage = Math.Max(
            0, state.upgradeStudies.tuningStage);
        state.upgradeStudies.tuningAmplitude = Mathf.Clamp01(
            state.upgradeStudies.tuningAmplitude);
        state.upgradeStudies.tuningFrequency = Mathf.Clamp01(
            state.upgradeStudies.tuningFrequency);
        return state.upgradeStudies;
    }

    public static UpgradeStudyDef GetStudy(string studyId)
    {
        EnsureCatalog();
        if (string.IsNullOrWhiteSpace(studyId)) return null;
        DefsById.TryGetValue(studyId, out UpgradeStudyDef def);
        return def;
    }

    public static UpgradeStudyDef GetStudyForUnlock(string unlockId)
    {
        EnsureCatalog();
        if (string.IsNullOrWhiteSpace(unlockId)) return null;
        DefsByUnlockId.TryGetValue(unlockId, out UpgradeStudyDef def);
        return def;
    }

    public static bool IsDiscovered(GameState state, string unlockId)
    {
        UpgradeStudyState progress = EnsureState(state);
        return progress != null && !string.IsNullOrWhiteSpace(unlockId) &&
            progress.discoveredIds.Contains(unlockId);
    }

    public static bool IsKeycardProjectDiscovered(GameState state) =>
        IsDiscovered(state, KeycardProjectUnlockId);

    public static bool ShouldShowStudyOpportunity(GameState state, string unlockId)
    {
        UpgradeStudyDef def = GetStudyForUnlock(unlockId);
        if (state == null || def == null) return false;
        if (IsDiscovered(state, unlockId)) return true;
        return MeetsStaticPrerequisites(state, def, false, out _);
    }

    public static UpgradeStudyBlockReason GetStartBlockReason(
        GameState state, string studyId)
    {
        UpgradeStudyDef def = GetStudy(studyId);
        UpgradeStudyState progress = EnsureState(state);
        if (state == null || progress == null || def == null)
            return UpgradeStudyBlockReason.UnknownStudy;
        if (IsDiscovered(state, def.unlockId))
            return UpgradeStudyBlockReason.AlreadyDiscovered;

        if (!string.IsNullOrEmpty(progress.activeStudyId))
        {
            if (progress.activeStudyId != studyId)
                return UpgradeStudyBlockReason.AnotherStudyActive;
            if (IsConclusionPending(state))
                return UpgradeStudyBlockReason.ConclusionPending;
            return UpgradeStudyBlockReason.StudyAlreadyActive;
        }

        if (!MeetsStaticPrerequisites(state, def, true, out UpgradeStudyBlockReason reason))
            return reason;
        if (!MeetsCircuitCondition(state, def))
            return UpgradeStudyBlockReason.CircuitNotSynchronized;
        if (state.triangleEnergy + 0.0000001 < def.energyCost)
            return UpgradeStudyBlockReason.InsufficientEnergy;
        return UpgradeStudyBlockReason.None;
    }

    public static bool TryStartStudy(GameState state, string studyId)
    {
        if (GetStartBlockReason(state, studyId) != UpgradeStudyBlockReason.None)
            return false;
        UpgradeStudyState progress = EnsureState(state);
        UpgradeStudyDef def = GetStudy(studyId);
        if (def == null || !state.TrySpendTriangleEnergy(def.energyCost))
            return false;
        progress.activeStudyId = studyId;
        progress.activeProgressSeconds = 0.0;
        progress.tuningStage = 0;
        ResetTuningControlsToChallenge(state);
        return true;
    }

    public static void Advance(GameState state, double seconds)
    {
        UpgradeStudyState progress = EnsureState(state);
        if (progress == null || seconds <= 0.0 ||
            string.IsNullOrEmpty(progress.activeStudyId))
            return;

        UpgradeStudyDef def = GetStudy(progress.activeStudyId);
        if (def == null)
        {
            progress.activeStudyId = "";
            progress.activeProgressSeconds = 0.0;
            return;
        }

        if (progress.activeProgressSeconds >= def.durationSeconds)
            return;

        progress.activeProgressSeconds = Math.Min(
            def.durationSeconds,
            progress.activeProgressSeconds + seconds);
    }

    public static bool IsConclusionPending(GameState state)
    {
        UpgradeStudyState progress = EnsureState(state);
        UpgradeStudyDef def = progress == null ? null : GetStudy(progress.activeStudyId);
        return def != null &&
            progress.activeProgressSeconds >= def.durationSeconds - 0.000001;
    }

    public static bool TryRevealConclusion(GameState state, out string unlockId)
    {
        unlockId = "";
        UpgradeStudyState progress = EnsureState(state);
        UpgradeStudyDef def = progress == null ? null : GetStudy(progress.activeStudyId);
        if (def == null || !IsConclusionPending(state)) return false;

        unlockId = def.unlockId;
        AddDiscovered(progress, unlockId);
        progress.activeStudyId = "";
        progress.activeProgressSeconds = 0.0;
        progress.tuningStage = 0;
        return true;
    }

    public static UpgradeStudyDef GetActiveStudy(GameState state)
    {
        UpgradeStudyState progress = EnsureState(state);
        return progress == null ? null : GetStudy(progress.activeStudyId);
    }

    public static double GetActiveProgress01(GameState state)
    {
        UpgradeStudyState progress = EnsureState(state);
        UpgradeStudyDef def = progress == null ? null : GetStudy(progress.activeStudyId);
        if (def == null || def.durationSeconds <= 0.0) return 0.0;
        return Math.Max(0.0, Math.Min(1.0,
            progress.activeProgressSeconds / def.durationSeconds));
    }

    public static double GetRemainingSeconds(GameState state)
    {
        UpgradeStudyState progress = EnsureState(state);
        UpgradeStudyDef def = progress == null ? null : GetStudy(progress.activeStudyId);
        return def == null ? 0.0 : Math.Max(
            0.0, def.durationSeconds - progress.activeProgressSeconds);
    }

    public static UpgradeStudyBlockReason GetActivePauseReason(GameState state)
    {
        UpgradeStudyState progress = EnsureState(state);
        UpgradeStudyDef def = progress == null ? null : GetStudy(progress.activeStudyId);
        if (def == null) return UpgradeStudyBlockReason.UnknownStudy;
        if (IsConclusionPending(state)) return UpgradeStudyBlockReason.ConclusionPending;
        return UpgradeStudyBlockReason.None;
    }

    public static bool IsActiveStudyProgressing(GameState state) =>
        GetActivePauseReason(state) == UpgradeStudyBlockReason.None;

    public static bool IsStudyActiveForUnlock(GameState state, string unlockId)
    {
        UpgradeStudyDef active = GetActiveStudy(state);
        return active != null && active.unlockId == unlockId;
    }

    public static int GetTuningStage(GameState state)
    {
        UpgradeStudyState progress = EnsureState(state);
        return progress?.tuningStage ?? 0;
    }

    public static int GetTuningStageCount(GameState state)
    {
        UpgradeStudyDef def = GetActiveStudy(state);
        return def?.tuningStages ?? 0;
    }

    public static double GetActiveTuningBoostFraction(GameState state)
    {
        UpgradeStudyDef def = GetActiveStudy(state);
        if (def == null) return 0.0;
        return GetTuningBoostFractionForAccuracy(
            state, GetTuningAccuracy(state));
    }

    public static double GetTuningBoostFractionForAccuracy(
        GameState state, double accuracy)
    {
        UpgradeStudyDef def = GetActiveStudy(state);
        if (def == null) return 0.0;
        double baseBoost = GetTuningBaseBoostFraction(
            accuracy, def.tuningBoostFraction);
        double relicBonus =
            Dimension1System.GetCalibrationFragmentStudyTuningBonus(state);
        return Math.Max(0.0, Math.Min(
            0.50,
            baseBoost + relicBonus
        ));
    }

    public static double GetTuningBaseBoostFraction(
        double accuracy, double standardBoostFraction = 0.18)
    {
        return accuracy >= TuningPerfectChannelAccuracy
            ? TuningPerfectBoostFraction
            : Math.Max(0.0, Math.Min(0.50, standardBoostFraction));
    }

    public static void SetTuningValues(
        GameState state, float amplitude, float frequency)
    {
        UpgradeStudyState progress = EnsureState(state);
        if (progress == null || string.IsNullOrEmpty(progress.activeStudyId)) return;
        progress.tuningAmplitude = Mathf.Clamp01(amplitude);
        progress.tuningFrequency = Mathf.Clamp01(frequency);
    }

    public static void GetTuningTarget(
        GameState state, out float amplitude, out float frequency)
    {
        amplitude = 0.5f;
        frequency = 0.5f;
        UpgradeStudyState progress = EnsureState(state);
        UpgradeStudyDef def = GetActiveStudy(state);
        if (progress == null || def == null) return;

        int seed = StableHash(def.id) ^ (progress.tuningStage + 1) * 7919;
        uint bits = unchecked((uint)seed);
        amplitude = 0.18f + ((bits & 1023u) / 1023f) * 0.64f;
        frequency = 0.18f + (((bits >> 10) & 1023u) / 1023f) * 0.64f;
    }

    public static double GetTuningAccuracy(GameState state)
    {
        UpgradeStudyState progress = EnsureState(state);
        if (progress == null || GetActiveStudy(state) == null) return 0.0;
        GetTuningTarget(state, out float targetAmplitude, out float targetFrequency);
        double amplitudeAccuracy = 1.0 -
            Math.Abs(progress.tuningAmplitude - targetAmplitude);
        double frequencyAccuracy = 1.0 -
            Math.Abs(progress.tuningFrequency - targetFrequency);
        return Math.Max(0.0, Math.Min(1.0,
            Math.Min(amplitudeAccuracy, frequencyAccuracy)));
    }

    public static void GetTuningChannelAccuracies(
        GameState state, out double amplitudeAccuracy, out double frequencyAccuracy)
    {
        amplitudeAccuracy = 0.0;
        frequencyAccuracy = 0.0;
        UpgradeStudyState progress = EnsureState(state);
        if (progress == null || GetActiveStudy(state) == null) return;
        GetTuningTarget(state, out float targetAmplitude, out float targetFrequency);
        amplitudeAccuracy = Math.Max(0.0, Math.Min(1.0,
            1.0 - Math.Abs(progress.tuningAmplitude - targetAmplitude)));
        frequencyAccuracy = Math.Max(0.0, Math.Min(1.0,
            1.0 - Math.Abs(progress.tuningFrequency - targetFrequency)));
    }

    public static bool IsTuningReady(GameState state)
    {
        GetTuningChannelAccuracies(
            state, out double amplitudeAccuracy, out double frequencyAccuracy);
        return amplitudeAccuracy >= TuningMinimumChannelAccuracy &&
            frequencyAccuracy >= TuningMinimumChannelAccuracy;
    }

    public static bool TryApplyActiveTuning(
        GameState state, out double accuracy, out double secondsApplied)
    {
        accuracy = GetTuningAccuracy(state);
        secondsApplied = 0.0;
        UpgradeStudyState progress = EnsureState(state);
        UpgradeStudyDef def = GetActiveStudy(state);
        if (progress == null || def == null || IsConclusionPending(state) ||
            progress.tuningStage >= def.tuningStages || !IsTuningReady(state))
            return false;

        secondsApplied = def.durationSeconds *
            GetTuningBoostFractionForAccuracy(state, accuracy);
        progress.activeProgressSeconds = Math.Min(
            def.durationSeconds, progress.activeProgressSeconds + secondsApplied);
        progress.tuningStage++;
        ResetTuningControlsToChallenge(state);
        return true;
    }

    private static void ResetTuningControlsToChallenge(GameState state)
    {
        UpgradeStudyState progress = EnsureState(state);
        if (progress == null || GetActiveStudy(state) == null) return;
        GetTuningTarget(state, out float targetAmplitude, out float targetFrequency);
        progress.tuningAmplitude = targetAmplitude < 0.5f ? 0.82f : 0.18f;
        progress.tuningFrequency = targetFrequency < 0.5f ? 0.78f : 0.22f;
    }

    private static int StableHash(string value)
    {
        unchecked
        {
            int hash = 17;
            foreach (char character in value ?? string.Empty)
                hash = hash * 31 + character;
            return hash;
        }
    }

    public static void RecordCircuitSwitch(
        GameState state, TriangleCircuitType previous, TriangleCircuitType next)
    {
        if (state == null || previous == TriangleCircuitType.None ||
            next == TriangleCircuitType.None || previous == next)
            return;
        EnsureState(state).circuitSwitchCount++;
    }

    public static void RecordCircuitSynchronized(
        GameState state, TriangleCircuitType circuit)
    {
        UpgradeStudyState progress = EnsureState(state);
        if (progress == null) return;
        int bit = CircuitBit(circuit);
        if (bit != 0) progress.synchronizedCircuitMask |= bit;
    }

    public static bool HasCircuitSynchronized(
        GameState state, TriangleCircuitType circuit)
    {
        UpgradeStudyState progress = EnsureState(state);
        int bit = CircuitBit(circuit);
        return progress != null && bit != 0 &&
            (progress.synchronizedCircuitMask & bit) != 0;
    }

    public static void ApplyLoadedState(
        GameState state, UpgradeStudyState loaded)
    {
        if (state == null) return;
        bool legacy = loaded == null || loaded.saveVersion < CurrentSaveVersion;
        state.upgradeStudies = loaded ?? new UpgradeStudyState();
        UpgradeStudyState progress = EnsureState(state);

        if (legacy)
            DiscoverLegacyVisibleContent(state, progress);

        EnsurePurchasedUpgradesDiscovered(progress);
        progress.saveVersion = CurrentSaveVersion;

        UpgradeStudyDef active = GetStudy(progress.activeStudyId);
        if (active == null || IsDiscovered(state, active.unlockId))
        {
            progress.activeStudyId = "";
            progress.activeProgressSeconds = 0.0;
            progress.tuningStage = 0;
        }
        else
        {
            progress.activeProgressSeconds = Math.Min(
                progress.activeProgressSeconds, active.durationSeconds);
        }
    }

    public static void ResetForNewRun(GameState state)
    {
        if (state == null) return;
        state.upgradeStudies = new UpgradeStudyState
        {
            saveVersion = CurrentSaveVersion,
            discoveredIds = new List<string>(),
            activeStudyId = "",
            activeProgressSeconds = 0.0,
            synchronizedCircuitMask = 0,
            circuitSwitchCount = 0,
            hideCompletedUpgrades = false,
            tuningStage = 0,
            tuningAmplitude = 0.5f,
            tuningFrequency = 0.5f
        };
    }

    private static bool MeetsStaticPrerequisites(
        GameState state,
        UpgradeStudyDef def,
        bool includeSwitchRequirement,
        out UpgradeStudyBlockReason reason)
    {
        reason = UpgradeStudyBlockReason.None;
        if (!string.IsNullOrEmpty(def.requiredBuildingId) &&
            state.GetBuildingLevel(def.requiredBuildingId) < 1)
        {
            reason = UpgradeStudyBlockReason.MissingArtifact;
            return false;
        }
        if (def.requiresAllTriangleVertices && !state.HasAllTriangleVertices())
        {
            reason = UpgradeStudyBlockReason.MissingVertices;
            return false;
        }
        if (def.requiresTriangleUnlocked && !state.CanUseTriangleCircuits())
        {
            reason = UpgradeStudyBlockReason.TriangleLocked;
            return false;
        }

        UpgradeStudyState progress = EnsureState(state);
        foreach (string requiredId in def.requiredDiscoveredIds)
        {
            if (!IsDiscovered(state, requiredId))
            {
                reason = UpgradeStudyBlockReason.MissingDiscovery;
                return false;
            }
        }

        if (includeSwitchRequirement &&
            progress.circuitSwitchCount < Math.Max(0, def.minimumCircuitSwitches))
        {
            reason = UpgradeStudyBlockReason.CircuitSwitchRequired;
            return false;
        }
        return true;
    }

    private static bool MeetsCircuitCondition(GameState state, UpgradeStudyDef def)
    {
        if (def.requiredCircuit == UpgradeStudyCircuitRequirement.None)
            return true;
        if (def.requiredCircuit == UpgradeStudyCircuitRequirement.Energy)
            return HasCircuitSynchronized(state, TriangleCircuitType.LE);
        if (def.requiredCircuit == UpgradeStudyCircuitRequirement.Experimental)
            return HasCircuitSynchronized(state, TriangleCircuitType.Traces);
        if (def.requiredCircuit == UpgradeStudyCircuitRequirement.TriangleEnergy)
            return HasCircuitSynchronized(state, TriangleCircuitType.TriangleEnergy);
        return HasCircuitSynchronized(state, TriangleCircuitType.LE) &&
            HasCircuitSynchronized(state, TriangleCircuitType.Traces);
    }

    private static void DiscoverLegacyVisibleContent(
        GameState state, UpgradeStudyState progress)
    {
        EnsureCatalog();
        foreach (UpgradeStudyDef def in DefsById.Values)
        {
            if (def.unlocksKeycardProject)
            {
                if (state.triangleSystemUnlocked || state.experimentalChamberUnlocked)
                    AddDiscovered(progress, def.unlockId);
                continue;
            }

            bool purchased = F2UpgradeManager.I != null &&
                F2UpgradeManager.I.GetPurchasedTierCount(def.unlockId) > 0;
            bool wasPreviouslyVisible = F2UpgradeManager.I != null &&
                F2UpgradeManager.I.MeetsPrerequisites(def.unlockId);
            if (purchased || wasPreviouslyVisible)
                AddDiscovered(progress, def.unlockId);
        }
    }

    private static void EnsurePurchasedUpgradesDiscovered(UpgradeStudyState progress)
    {
        if (progress == null || F2UpgradeManager.I == null) return;
        EnsureCatalog();
        foreach (UpgradeStudyDef def in DefsById.Values)
        {
            if (!def.unlocksKeycardProject &&
                F2UpgradeManager.I.GetPurchasedTierCount(def.unlockId) > 0)
                AddDiscovered(progress, def.unlockId);
        }
    }

    private static void AddDiscovered(UpgradeStudyState progress, string unlockId)
    {
        if (progress == null || string.IsNullOrWhiteSpace(unlockId)) return;
        if (!progress.discoveredIds.Contains(unlockId))
            progress.discoveredIds.Add(unlockId);
    }

    private static int CircuitBit(TriangleCircuitType circuit)
    {
        if (circuit == TriangleCircuitType.LE) return 1;
        if (circuit == TriangleCircuitType.Traces) return 2;
        if (circuit == TriangleCircuitType.TriangleEnergy) return 4;
        return 0;
    }
}
