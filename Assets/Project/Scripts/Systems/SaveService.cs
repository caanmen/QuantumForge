using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public enum SaveFailureInjectionPoint
{
    None = 0,
    BeforeTempWrite = 1,
    AfterTempWrite = 2,
    AfterTempValidation = 3,
    BeforeReplace = 4
}

[Serializable]
public class SaveData
{
    public int saveSchemaVersion;
    public int removedLegacyResourcesVersion;
    public int f2ProgressionMigrationVersion;
    public int machineSeedRetirementMigrationVersion;
    public UpgradeStudyState upgradeStudies;

        // F3 / Cuarto 2 - recursos
    public int fragmentCondensation;
    public int fragmentConfinement;
    public int fragmentResidualInterference;
    public double fragmentCondensationProgress;
    public double fragmentConfinementProgress;
    public double fragmentResidualInterferenceProgress;

    public int experimentalHallazgos;
    public int experimentalMuestras;
    public int experimentalLecturasIncompletas;
    public int experimentalCompuestosUtiles;
    public int synthesisCoreFusionCounter;
    public int fusionInstability;
    public double fusionCooldownRemainingSeconds;
    public ExperimentalPendingFusionState pendingFusion;
    public List<ChronalSeedSlotState> chronalSeedSlots;
    public int chronalMatureSeedsStored;
    public ChronalInstantState chronalInstant;
    public int chronalMaterializedInstants;
    public int chronalPureInstants;
    public int chronalStableInstants;
    public int chronalForcedInstants;
    public int chronalArchivedInstants;
    // F3 / Cuarto 2 - log
    public List<ExperimentalMixLogEntry> experimentalMixLog;
    public int guidedSynthesisIntent;

        // Cuarto 1 - sistema triangular
    public bool triangleSystemUnlocked;
    public bool triangleActivationTutorialSeen;
    public int triangleCircuitSaveVersion;
    public int triangleActiveCircuit;
    public float triangleSynchronization;
    public double triangleSynchronizationBaseRatePerSecond;
    public int triangleEnergySaveVersion;
    public double triangleEnergy;
    public string trianglePrimaryBuildingId;
    public string triangleReinforcementBuildingId;
    public string triangleAlterationBuildingId;
    public List<string> machineRepairedNodeIds = new List<string>();
    public List<string> machineAnalyzedNodeIds = new List<string>();
    public string machineAnalysisNodeId;
    public double machineAnalysisRemainingSeconds;
    public bool machineIntroSeen;
    public bool machineUnlocked;
    public bool machineFusionPanelUnlocked;
    public bool machineAllZonesUnlocked;
    public int machineSelectedFaceIndex = 1;
    public double LE;
    public double Traces;
    public double VP;
    // Compatibilidad heredada: conservar para leer JSON anterior al retiro.
    public double EM;
    public double emMult;
    public int phaseModulatorMode;
    public float phaseModulatorCalibration;

    // public float trianglePersistenceMaturation; // lógica vieja de Persistencia
    public double trianglePersistenceReserveSeconds;
    // F6.1: Moneda de prestigio

    // Legado: moneda anterior asociada a Prestigio 1.
    public int prestige1Count;
    public bool hasDonePrestige1;
    public int prestige1Points;
    public int prestige1BestClaimedPreviewPoints;
    public int prestige1CurrentDimensionId;
    // D1 - moneda exclusiva del Árbol Dimensional.
    public int d1TreePointsSaveVersion;
    public int d1TreePoints;
    public int d1TreePointsProgressBaseline;
    // F6.1: Máximo LE alcanzado en el run
    public double maxLEAlcanzado;
    public List<SavedF2UpgradeTier> f2UpgradeTiers = new();
    // Compatibilidad heredada: conservar para leer JSON anterior al retiro.
    public double ADP;
    public double WHF;
    // F7: Prestigio 2 (Lambda)
    public double Lambda;
    // Compatibilidad heredada: conservar para leer JSON anterior al retiro.
    public double totalADPGenerada;
    public double totalWHFGenerada;
    // Lista de investigaciones compradas (ids)
    public List<string> purchasedResearchIds;
    // Lista de logros desbloqueados (ids)
    public List<string> unlockedAchievementIds;
    public double baseLEps;
    public long lastUnix;

    // F7.5: Meta-upgrades comprados con Λ
    public bool metaEntBoost1Bought;
    public bool metaEmBoost1Bought;
    // Niveles de edificios
    public List<SavedBuildingLevel> buildingLevels;
    public bool experimentalChamberUnlocked;
    public bool experimentalChamberInitialPackGranted;

    // Sistema de dimensiones
    public int dimensionDiscoverySaveVersion;
    public bool dimension01Unlocked;
    public bool dimension02Unlocked;
    public bool dimension03Unlocked;
    public Dimension2State dimension2;
    public Dimension3State dimension3;
    public ConvergenceState convergence;
    public List<D1MetalAmount> dimension1Metals;
    public List<D1PlanetState> dimension1Planets;
    public List<D1SectorState> dimension1Sectors;
    public string dimension1SelectedSectorId;
    public string dimension1ActiveScanSectorId;
    public List<D1ShipState> dimension1Ships;
    public int dimension1CoordinatedMissionProgressVersion;
    public int dimension1CompletedCoordinatedMissions;
    public int dimension1ArkProgressVersion;
    public bool dimension1ArkInvestigated;
    public List<D1CentralSyncMissionState> dimension1CentralSyncMissions;
    public bool dimension1CentralSyncEstablished;
    public bool dimension1CentralAccessKeyObtained;
    public bool dimension1CentralAccessKeyLogSeen;
    public bool dimension1ArkFinalMissionActive;
    public double dimension1ArkFinalMissionRemainingSeconds;
    public double dimension1ArkFinalMissionTotalSeconds;
    public List<string> dimension1ArkFinalMissionShipIds;
    public bool dimension1GalacticAnchorDiscovered;
    public List<D1ScannedDestinationState> dimension1ScannedDestinations;
    public List<string> dimension1PreviousScannedDestinationIds;
    public bool dimension1ScanActive;
    public double dimension1ScanRemainingSeconds;
    public double dimension1ScanTotalSeconds;
    public int dimension1ScannerLevel;
    public int dimension1ScannerProgressVersion;
    public string dimension1LastExplorationDestinationId;
    public List<D1MetalAmount> dimension1LastExplorationRewards;
    public List<D1BlueprintAmount> dimension1LastExplorationSpecificBlueprints;
    public List<D1RelicRewardEntry> dimension1LastExplorationRelics;
    public List<D1ExplorationRecordEntry> dimension1RecentExplorationRecords;
    public List<D1BlueprintAmount> dimension1Blueprints;
    public List<D1RelicState> dimension1Relics;
    public List<D1RelicPityState> dimension1RelicPityStates;
    public int dimension1RelicProgressVersion;
    public List<D1TreeNodeState> dimension1TreeNodes;
    public int dimension1TreeProgressVersion;
    public int dimension1BlueprintFragments;
    public int dimension1LastExplorationBlueprintFragments;
    public int dimension1LastExplorationResultId;
    public bool dimension1ManualSimpleScanCompleted;
    public List<string> dimension1ManualSimpleDestinationIds;
    public string dimension1LastManualSimpleDestinationId;
    public List<string> dimension1ManualExtractorUpgradePlanetIds;
    public int dimension1AutomationHistoryProgressVersion;
    public int dimension1CompletedSimpleExplorations;
}

public class SaveService : MonoBehaviour
{
    public const int CurrentSaveSchemaVersion = 2;
    public const int HistoricalBackupCount = 3;
    public const int RemovedLegacyResourcesVersion = 1;
    public static List<string> LastLoadedResearchIds;
    public static List<string> LastLoadedAchievementIds;
    public static List<SavedBuildingLevel> LastLoadedBuildingLevels;



    public static SaveService I { get; private set; }

#if UNITY_EDITOR
    // Test instrumentation only. Never redirect a player build or touch the
    // player's persistent directory while exercising Load/Save in the Editor.
    private static string editorValidationSaveDirectory;
    public static string EditorValidationSaveDirectory
    {
        get => editorValidationSaveDirectory;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                editorValidationSaveDirectory = null;
                return;
            }
            string root = Path.GetFullPath(Path.Combine(Application.dataPath,
                "../Logs/ReleaseD1D3")) + Path.DirectorySeparatorChar;
            string target = Path.GetFullPath(value);
            if (!target.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Validation saves must be inside " + root);
            editorValidationSaveDirectory = target;
        }
    }
#endif
    private string SavePath
    {
        get
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(editorValidationSaveDirectory))
                return Path.Combine(editorValidationSaveDirectory, "save.json");
#endif
            return Path.Combine(Application.persistentDataPath, "save.json");
        }
    }
    private string SaveBackupPath => SavePath + ".bak";

    public string CurrentSavePath => SavePath;

    public static SaveFailureInjectionPoint FailureInjectionPoint = SaveFailureInjectionPoint.None;
    private static bool suppressWritesForVisualQa;
    public static bool SuppressWritesForVisualQa
    {
        get => suppressWritesForVisualQa;
        set
        {
            // Los validadores terminan cerrando el proceso. Si liberan la guarda justo
            // antes de Exit, OnApplicationQuit puede guardar el estado ficticio de
            // captura. Una vez activada, la supresión queda enclavada hasta que finaliza
            // esta sesión de Unity.
            if (suppressWritesForVisualQa && !value)
                return;

            suppressWritesForVisualQa = value;
        }
    }

    [Tooltip("Autosave cada N segundos.")]
    public int autosaveSeconds = 30;

    private bool resumePending;
    private bool pauseSaveSucceeded;
    private bool historyCapturedThisSession;
    public bool HasLoadFailure { get; private set; }

    private void Awake()
    {
        // Versión simple: este objeto de la escena es SIEMPRE la instancia
        I = this;

#if UNITY_EDITOR
        Debug.Log("[SaveService] Awake()");
#endif
    }

    private void Start()
    {
#if UNITY_EDITOR
        Debug.Log("[SaveService] Start()");
#endif
        // OJO: el Load lo hará GameState.Start(), cuando GameState.I ya exista
        InvokeRepeating(nameof(Save), autosaveSeconds, autosaveSeconds);
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        Debug.Log("[SaveService] OnApplicationQuit -> Save()");
#endif
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        TickSystem.I?.ResetAccumulator();

        // Al salir de Play Mode el Editor puede enviar pause=true despues de
        // haber destruido GameState. No es una ausencia real ni hay estado que
        // guardar; tampoco debe dejar armada una reanudacion para mas tarde.
        if (!Application.isPlaying || GameState.I == null)
        {
            resumePending = false;
            pauseSaveSucceeded = false;
            return;
        }

        if (pause)
        {
#if UNITY_EDITOR
            Debug.Log("[SaveService] OnApplicationPause(true) -> Save()");
#endif
            resumePending = true;
            string error = null;
            pauseSaveSucceeded = SuppressWritesForVisualQa || TrySave(out error);
            if (!pauseSaveSucceeded)
                Debug.LogError("[SaveService] No se pudo guardar al pausar: " + error);
            return;
        }

        // Unity también puede enviar pause=false durante el arranque. Solo se
        // procesa una reanudación si antes recibimos una pausa real.
        if (!resumePending)
            return;

        resumePending = false;
        bool canRestoreFromPauseSave = pauseSaveSucceeded;
        pauseSaveSucceeded = false;
        if (!canRestoreFromPauseSave || GameState.I == null)
            return;

        try
        {
            // El save de la pausa contiene el instante exacto de salida. Load
            // aplica el tiempo ausente mediante las reglas offline ya validadas.
            Load();

            // Persistir inmediatamente el estado ya reanudado evita volver a
            // conceder el mismo intervalo si Android mata el proceso enseguida.
            Save();
#if UNITY_EDITOR
            Debug.Log("[SaveService] Reanudacion completada desde pausa.");
#endif
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    public void Save()
    {
        if (HasLoadFailure)
        {
            Debug.LogWarning("[SaveService] Guardado bloqueado: la partida no se pudo cargar. Reintenta la carga antes de guardar.");
            return;
        }
        if (SuppressWritesForVisualQa)
        {
#if UNITY_EDITOR
            Debug.Log("[SaveService] Escritura suprimida durante QA visual.");
#endif
            return;
        }
        if (!TrySave(out string error))
            Debug.LogError("[SaveService] No se pudo guardar: " + error);
    }

    public bool TrySave(out string error)
    {
        error = null;
        if (HasLoadFailure)
        {
            error = "Guardado bloqueado para conservar la partida que no se pudo cargar.";
            return false;
        }
        if (SuppressWritesForVisualQa)
        {
#if UNITY_EDITOR
            Debug.Log("[SaveService] TrySave suprimido durante QA visual.");
#endif
            return true;
        }
        if (GameState.I == null)
        {
        #if UNITY_EDITOR
            Debug.LogWarning("[SaveService] Save() cancelado: GameState.I es null.");
        #endif
            error = "GameState.I es null.";
            return false;
        }

        if (ConvergenceSystem.IsFutureVersion(GameState.I.convergence))
        {
            error = "El estado de Convergencia pertenece a una versiÃ³n futura.";
            return false;
        }

        ApplyRemovedLegacyResourcesMigration(null, GameState.I);

        var data = new SaveData
        {
        saveSchemaVersion = CurrentSaveSchemaVersion,
        removedLegacyResourcesVersion = RemovedLegacyResourcesVersion,
        f2ProgressionMigrationVersion = F2UpgradeManager.ProgressionMigrationVersion,
        machineSeedRetirementMigrationVersion =
            MachineManager.SeedRetirementMigrationVersion,
        LE = GameState.I.LE,
        Traces = GameState.I.Traces,
        VP = GameState.I.VP,
        EM = GameState.I.EM,
        emMult = GameState.I.emMult,
        baseLEps = GameState.I.baseLEps,
        phaseModulatorMode = (int)GameState.I.phaseModulatorMode,
        phaseModulatorCalibration = GameState.I.phaseModulatorCalibration,
        trianglePersistenceReserveSeconds = GameState.I.trianglePersistenceReserveSeconds,
        triangleCircuitSaveVersion = 2,
        triangleEnergySaveVersion = 1,
        triangleEnergy = GameState.I.triangleEnergy,
        triangleActiveCircuit = (int)GameState.I.triangleActiveCircuit,
        triangleSynchronization = GameState.I.triangleSynchronization,
        triangleSynchronizationBaseRatePerSecond =
            GameState.I.triangleSynchronizationBaseRatePerSecond,
        upgradeStudies = GameState.I.upgradeStudies,

        // Prestigio 1 - descubrimiento dimensional
        prestige1Count = GameState.I.prestige1Count,
        hasDonePrestige1 = GameState.I.hasDonePrestige1,
        prestige1CurrentDimensionId = GameState.I.prestige1CurrentDimensionId,
        d1TreePointsSaveVersion = 1,
        d1TreePoints = GameState.I.d1TreePoints,
        d1TreePointsProgressBaseline = GameState.I.d1TreePointsProgressBaseline,

            // F6.1: prestigio viejo
        maxLEAlcanzado = GameState.I.maxLEAlcanzado,

        // F7: recursos late-game
        ADP = GameState.I.ADP,
        WHF = GameState.I.WHF,

        // F7: prestigio 2 (Lambda) + estadísticas
        Lambda = GameState.I.Lambda,
        totalADPGenerada = GameState.I.totalADPGenerada,
        totalWHFGenerada = GameState.I.totalWHFGenerada,


        // F7.5: meta-upgrades
        metaEmBoost1Bought = GameState.I.metaEmBoost1Bought,

         // 🆕 Niveles de edificios
        buildingLevels = GameState.I.GetBuildingLevelsForSave(),
        experimentalChamberUnlocked = GameState.I.experimentalChamberUnlocked,
        experimentalChamberInitialPackGranted = GameState.I.experimentalChamberInitialPackGranted,
        // Sistema de dimensiones
        dimensionDiscoverySaveVersion = 1,
        dimension01Unlocked = GameState.I.dimension01Unlocked,
        dimension02Unlocked = GameState.I.dimension02Unlocked,
        dimension03Unlocked = GameState.I.dimension03Unlocked,
        dimension2 = GameState.I.dimension2,
        dimension3 = GameState.I.dimension3,
        convergence = GameState.I.convergence,
        dimension1Metals = GameState.I.dimension1Metals,
        dimension1Planets = GameState.I.dimension1Planets,
        dimension1Sectors = GameState.I.dimension1Sectors,
        dimension1SelectedSectorId = GameState.I.dimension1SelectedSectorId,
        dimension1ActiveScanSectorId = GameState.I.dimension1ActiveScanSectorId,
        dimension1Ships = GameState.I.dimension1Ships,
        dimension1CoordinatedMissionProgressVersion = GameState.I.dimension1CoordinatedMissionProgressVersion,
        dimension1CompletedCoordinatedMissions = GameState.I.dimension1CompletedCoordinatedMissions,
        dimension1ArkProgressVersion = GameState.I.dimension1ArkProgressVersion,
        dimension1ArkInvestigated = GameState.I.dimension1ArkInvestigated,
        dimension1CentralSyncMissions = GameState.I.dimension1CentralSyncMissions,
        dimension1CentralSyncEstablished = GameState.I.dimension1CentralSyncEstablished,
        dimension1CentralAccessKeyObtained = GameState.I.dimension1CentralAccessKeyObtained,
        dimension1CentralAccessKeyLogSeen = GameState.I.dimension1CentralAccessKeyLogSeen,
        dimension1ArkFinalMissionActive = GameState.I.dimension1ArkFinalMissionActive,
        dimension1ArkFinalMissionRemainingSeconds = GameState.I.dimension1ArkFinalMissionRemainingSeconds,
        dimension1ArkFinalMissionTotalSeconds = GameState.I.dimension1ArkFinalMissionTotalSeconds,
        dimension1ArkFinalMissionShipIds = GameState.I.dimension1ArkFinalMissionShipIds,
        dimension1GalacticAnchorDiscovered = GameState.I.dimension1GalacticAnchorDiscovered,
        dimension1ScannedDestinations = GameState.I.dimension1ScannedDestinations,
        dimension1PreviousScannedDestinationIds = GameState.I.dimension1PreviousScannedDestinationIds,
        dimension1ScanActive = GameState.I.dimension1ScanActive,
        dimension1ScanRemainingSeconds = GameState.I.dimension1ScanRemainingSeconds,
        dimension1ScanTotalSeconds = GameState.I.dimension1ScanTotalSeconds,
        dimension1ScannerLevel = GameState.I.dimension1ScannerLevel,
        dimension1ScannerProgressVersion = GameState.I.dimension1ScannerProgressVersion,
        dimension1LastExplorationDestinationId = GameState.I.dimension1LastExplorationDestinationId,
        dimension1LastExplorationRewards = GameState.I.dimension1LastExplorationRewards,
        dimension1LastExplorationSpecificBlueprints = GameState.I.dimension1LastExplorationSpecificBlueprints,
        dimension1LastExplorationRelics = GameState.I.dimension1LastExplorationRelics,
        dimension1RecentExplorationRecords = GameState.I.dimension1RecentExplorationRecords,
        dimension1Blueprints = GameState.I.dimension1Blueprints,
        dimension1Relics = GameState.I.dimension1Relics,
        dimension1RelicPityStates = GameState.I.dimension1RelicPityStates,
        dimension1RelicProgressVersion = GameState.I.dimension1RelicProgressVersion,
        dimension1TreeNodes = GameState.I.dimension1TreeNodes,
        dimension1TreeProgressVersion = GameState.I.dimension1TreeProgressVersion,
        dimension1BlueprintFragments = GameState.I.dimension1BlueprintFragments,
        dimension1LastExplorationBlueprintFragments = GameState.I.dimension1LastExplorationBlueprintFragments,
        dimension1LastExplorationResultId = GameState.I.dimension1LastExplorationResultId,
        dimension1ManualSimpleScanCompleted = GameState.I.dimension1ManualSimpleScanCompleted,
        dimension1ManualSimpleDestinationIds = GameState.I.dimension1ManualSimpleDestinationIds,
        dimension1LastManualSimpleDestinationId = GameState.I.dimension1LastManualSimpleDestinationId,
        dimension1ManualExtractorUpgradePlanetIds = GameState.I.dimension1ManualExtractorUpgradePlanetIds,
        dimension1AutomationHistoryProgressVersion = GameState.I.dimension1AutomationHistoryProgressVersion,
        dimension1CompletedSimpleExplorations = GameState.I.dimension1CompletedSimpleExplorations,

        fragmentCondensation = GameState.I.fragmentCondensation,
        fragmentConfinement = GameState.I.fragmentConfinement,
        fragmentResidualInterference = GameState.I.fragmentResidualInterference,
        fragmentCondensationProgress = GameState.I.fragmentCondensationProgress,
        fragmentConfinementProgress = GameState.I.fragmentConfinementProgress,
        fragmentResidualInterferenceProgress = GameState.I.fragmentResidualInterferenceProgress,

        experimentalHallazgos = GameState.I.experimentalHallazgos,
        experimentalMuestras = GameState.I.experimentalMuestras,
        experimentalLecturasIncompletas = GameState.I.experimentalLecturasIncompletas,
        experimentalCompuestosUtiles = GameState.I.experimentalCompuestosUtiles,
        synthesisCoreFusionCounter = GameState.I.synthesisCoreFusionCounter,
        fusionInstability = GameState.I.fusionInstability,
        fusionCooldownRemainingSeconds = GameState.I.fusionCooldownRemainingSeconds,
        pendingFusion = GameState.I.pendingFusion,
        chronalSeedSlots = GameState.I.chronalSeedSlots,
        chronalMatureSeedsStored = GameState.I.chronalMatureSeedsStored,
        chronalInstant = GameState.I.chronalInstant,
        chronalMaterializedInstants = GameState.I.chronalMaterializedInstants,
        chronalPureInstants = GameState.I.chronalPureInstants,
        chronalStableInstants = GameState.I.chronalStableInstants,
        chronalForcedInstants = GameState.I.chronalForcedInstants,
        chronalArchivedInstants = GameState.I.chronalArchivedInstants,

        experimentalMixLog = GameState.I.experimentalMixLog,
        guidedSynthesisIntent = GameState.I.guidedSynthesisIntent,
        triangleSystemUnlocked = GameState.I.triangleSystemUnlocked,
        triangleActivationTutorialSeen = GameState.I.triangleActivationTutorialSeen,
        trianglePrimaryBuildingId = GameState.I.trianglePrimaryBuildingId,
        triangleReinforcementBuildingId = GameState.I.triangleReinforcementBuildingId,
        triangleAlterationBuildingId = GameState.I.triangleAlterationBuildingId,
        

        f2UpgradeTiers = (F2UpgradeManager.I != null)
            ? F2UpgradeManager.I.GetPurchasedTiersForSave()
            : new List<SavedF2UpgradeTier>(),
        purchasedResearchIds = (ResearchManager.I != null)
            ? ResearchManager.I.GetPurchasedIds()
            : LastLoadedResearchIds,
        unlockedAchievementIds = (AchievementManager.I != null)
            ? AchievementManager.I.GetUnlockedIds()
            : LastLoadedAchievementIds,
        lastUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    };

        if (MachineManager.I != null)
        {
            MachineManager.I.WriteProgressToSave(data);
        }

        var json = JsonUtility.ToJson(data, prettyPrint: true);
        if (!TryWriteAtomicJson(SavePath, json, out error))
            return false;

#if UNITY_EDITOR
        Debug.Log($"[SaveService] Saved: {SavePath}");
#endif
        return true;
    }

    public void Load()
    {
        HasLoadFailure = true;
        try { LoadCore(); }
        catch
        {
            HasLoadFailure = true;
            throw;
        }
    }

    private void LoadCore()
    {
#if UNITY_EDITOR
        Debug.Log($"[SaveService] Load() llamado. Existe archivo? {File.Exists(SavePath)} ({SavePath})");
#endif

        if (GameState.I == null) return;

        if (!TryFindRecoverableSave(SavePath, out SaveData data,
                out string loadedPath, out string recoveryError))
        {
            if (!HasAnySaveCandidate(SavePath))
                InitNewGame();
            else
                Debug.LogError("[SaveService] No se modificara la partida: " +
                    recoveryError);
            return;
        }

        int loadedSchemaVersion = data.saveSchemaVersion;
        bool recoveredFromFallback = !PathsEqual(loadedPath, SavePath);
        bool requiresImmediateSave = recoveredFromFallback ||
            loadedSchemaVersion < CurrentSaveSchemaVersion;

        if (!SuppressWritesForVisualQa && !historyCapturedThisSession)
        {
            if (!TryCreateHistoricalSnapshot(
                    SavePath, loadedPath, out string historyError))
            {
                Debug.LogError("[SaveService] Carga cancelada: no se pudo " +
                    "respaldar la partida. " + historyError);
                return;
            }
            historyCapturedThisSession = true;
        }

        if (recoveredFromFallback)
            Debug.LogWarning("[SaveService] Partida recuperada desde " + loadedPath);

        MigrateSaveData(data);
        ApplyRemovedLegacyResourcesMigration(data, GameState.I);
        if (MachineManager.I != null)
            MachineManager.I.ApplySeedRetirementMigration(data);

        bool noBuildings = (data.buildingLevels == null || data.buildingLevels.Count == 0);
        bool looksFresh = data.LE <= 0.0 && data.maxLEAlcanzado <= 0.0 && data.Lambda <= 0.0 && noBuildings;

        if (looksFresh)
        {
            InitNewGame();
            return;
        }


        GameState.I.LE = data.LE;
        GameState.I.Traces = data.Traces;
        GameState.I.VP = data.VP;
        GameState.I.EM = 0.0;
        GameState.I.emMult = 0.0;
        GameState.I.baseLEps = data.baseLEps;
        GameState.I.phaseModulatorMode = (PhaseModulatorMode)data.phaseModulatorMode;
        GameState.I.phaseModulatorCalibration = data.phaseModulatorCalibration;
        GameState.I.triangleActiveCircuit = (TriangleCircuitType)data.triangleActiveCircuit;
        GameState.I.triangleSynchronization = data.triangleSynchronization;
        GameState.I.triangleSynchronizationBaseRatePerSecond =
            data.triangleSynchronizationBaseRatePerSecond;
        GameState.I.triangleEnergy = data.triangleEnergySaveVersion >= 1 &&
            !double.IsNaN(data.triangleEnergy) &&
            !double.IsInfinity(data.triangleEnergy)
                ? System.Math.Max(0.0, data.triangleEnergy)
                : 0.0;
        // GameState.I.trianglePersistenceMaturation = data.trianglePersistenceMaturation;
        GameState.I.trianglePersistenceReserveSeconds = data.trianglePersistenceReserveSeconds;
        GameState.I.experimentalChamberUnlocked = data.experimentalChamberUnlocked;
        GameState.I.experimentalChamberInitialPackGranted = data.experimentalChamberInitialPackGranted;
        // Sistema de dimensiones
        GameState.I.dimension01Unlocked = data.dimension01Unlocked;
        GameState.I.dimension02Unlocked = data.dimension02Unlocked;
        GameState.I.dimension03Unlocked = data.dimension03Unlocked;
        GameState.I.dimension2 = data.dimension2 ?? Dimension2System.CreateInitialState();
        GameState.I.dimension3 = data.dimension3 ?? Dimension3System.CreateInitialState();
        GameState.I.convergence = data.convergence ?? ConvergenceSystem.CreateInitialState();
        GameState.I.dimension1Metals = data.dimension1Metals ?? new List<D1MetalAmount>();
        GameState.I.dimension1Planets = data.dimension1Planets ?? new List<D1PlanetState>();
        GameState.I.dimension1Sectors = data.dimension1Sectors ?? new List<D1SectorState>();
        GameState.I.dimension1SelectedSectorId = data.dimension1SelectedSectorId ?? "";
        GameState.I.dimension1ActiveScanSectorId = data.dimension1ActiveScanSectorId ?? "";
        GameState.I.dimension1Ships = data.dimension1Ships ?? new List<D1ShipState>();
        GameState.I.dimension1CoordinatedMissionProgressVersion = data.dimension1CoordinatedMissionProgressVersion;
        GameState.I.dimension1CompletedCoordinatedMissions = data.dimension1CompletedCoordinatedMissions;
        GameState.I.dimension1ArkProgressVersion = data.dimension1ArkProgressVersion;
        GameState.I.dimension1ArkInvestigated = data.dimension1ArkInvestigated;
        GameState.I.dimension1CentralSyncMissions = data.dimension1CentralSyncMissions ?? new List<D1CentralSyncMissionState>();
        GameState.I.dimension1CentralSyncEstablished = data.dimension1CentralSyncEstablished;
        GameState.I.dimension1CentralAccessKeyObtained = data.dimension1CentralAccessKeyObtained;
        GameState.I.dimension1CentralAccessKeyLogSeen = data.dimension1CentralAccessKeyLogSeen;
        GameState.I.dimension1ArkFinalMissionActive = data.dimension1ArkFinalMissionActive;
        GameState.I.dimension1ArkFinalMissionRemainingSeconds = data.dimension1ArkFinalMissionRemainingSeconds;
        GameState.I.dimension1ArkFinalMissionTotalSeconds = data.dimension1ArkFinalMissionTotalSeconds;
        GameState.I.dimension1ArkFinalMissionShipIds = data.dimension1ArkFinalMissionShipIds ?? new List<string>();
        GameState.I.dimension1GalacticAnchorDiscovered = data.dimension1GalacticAnchorDiscovered;
        GameState.I.dimension1ScannedDestinations = data.dimension1ScannedDestinations ?? new List<D1ScannedDestinationState>();
        GameState.I.dimension1PreviousScannedDestinationIds = data.dimension1PreviousScannedDestinationIds ?? new List<string>();
        GameState.I.dimension1ScanActive = data.dimension1ScanActive;
        GameState.I.dimension1ScanRemainingSeconds = data.dimension1ScanRemainingSeconds;
        GameState.I.dimension1ScanTotalSeconds = data.dimension1ScanTotalSeconds;
        GameState.I.dimension1ScannerLevel = data.dimension1ScannerLevel;
        GameState.I.dimension1ScannerProgressVersion = data.dimension1ScannerProgressVersion;
        GameState.I.dimension1LastExplorationDestinationId = data.dimension1LastExplorationDestinationId ?? "";
        GameState.I.dimension1LastExplorationRewards = data.dimension1LastExplorationRewards ?? new List<D1MetalAmount>();
        GameState.I.dimension1LastExplorationSpecificBlueprints = data.dimension1LastExplorationSpecificBlueprints ?? new List<D1BlueprintAmount>();
        GameState.I.dimension1LastExplorationRelics = data.dimension1LastExplorationRelics ?? new List<D1RelicRewardEntry>();
        GameState.I.dimension1RecentExplorationRecords = data.dimension1RecentExplorationRecords ?? new List<D1ExplorationRecordEntry>();
        GameState.I.dimension1Blueprints = data.dimension1Blueprints ?? new List<D1BlueprintAmount>();
        GameState.I.dimension1Relics = data.dimension1Relics ?? new List<D1RelicState>();
        GameState.I.dimension1RelicPityStates = data.dimension1RelicPityStates ?? new List<D1RelicPityState>();
        GameState.I.dimension1RelicProgressVersion = data.dimension1RelicProgressVersion;
        GameState.I.dimension1TreeNodes = data.dimension1TreeNodes ?? new List<D1TreeNodeState>();
        GameState.I.dimension1TreeProgressVersion = data.dimension1TreeProgressVersion;
        GameState.I.dimension1BlueprintFragments = data.dimension1BlueprintFragments;
        GameState.I.dimension1LastExplorationBlueprintFragments = data.dimension1LastExplorationBlueprintFragments;
        GameState.I.dimension1LastExplorationResultId = data.dimension1LastExplorationResultId;
        GameState.I.dimension1ManualSimpleScanCompleted = data.dimension1ManualSimpleScanCompleted;
        GameState.I.dimension1ManualSimpleDestinationIds = data.dimension1ManualSimpleDestinationIds ?? new List<string>();
        GameState.I.dimension1LastManualSimpleDestinationId =
            data.dimension1LastManualSimpleDestinationId ?? "";
        GameState.I.dimension1ManualExtractorUpgradePlanetIds = data.dimension1ManualExtractorUpgradePlanetIds ?? new List<string>();
        GameState.I.dimension1AutomationHistoryProgressVersion = data.dimension1AutomationHistoryProgressVersion;
        GameState.I.dimension1CompletedSimpleExplorations = data.dimension1CompletedSimpleExplorations;

        // Prestigio 1 - descubrimiento dimensional y moneda D1.
        GameState.I.prestige1Count = data.prestige1Count;
        GameState.I.hasDonePrestige1 = data.hasDonePrestige1;
        bool hasD1TreePointSave = data.d1TreePointsSaveVersion >= 1;
        GameState.I.d1TreePoints = hasD1TreePointSave
            ? data.d1TreePoints
            : data.prestige1Points;
        GameState.I.d1TreePointsProgressBaseline = hasD1TreePointSave
            ? data.d1TreePointsProgressBaseline
            : 0;
        GameState.I.prestige1CurrentDimensionId = Mathf.Clamp(
            data.prestige1CurrentDimensionId, 0, 3);

        // Compatibilidad con saves antiguos que registraron el contador antes
        // de que existiera la bandera explícita de Prestigio 1 completado.
        if (GameState.I.prestige1Count > 0)
            GameState.I.hasDonePrestige1 = true;

        GameState.I.EnsureDimension1State();
        GameState.I.EnsureDimension2State();
        GameState.I.EnsureDimension3State();
        GameState.I.EnsureConvergenceState();

        // La moneda previa conserva su saldo y nodos comprados. Su nuevo baseline
        // usa la fórmula histórica para conservar el crédito adicional del nuevo balance.
        // Migración para partidas previas a la selección de una sola dimensión.
        // Las partidas nuevas conservan exactamente qué dimensiones revelaron.
        ApplyDimensionDiscoveryMigration(
            GameState.I,
            data.dimensionDiscoverySaveVersion
        );

        if (!hasD1TreePointSave)
        {
            GameState.I.d1TreePointsProgressBaseline =
                Dimension1System.CalculateLegacyD1TreePointsBaseline(GameState.I);
        }

        // F6.1: prestigio viejo
        GameState.I.maxLEAlcanzado = data.maxLEAlcanzado;

        // F7: recursos late-game y Lambda
        GameState.I.ADP = 0.0;
        GameState.I.WHF = 0.0;
        GameState.I.Lambda = data.Lambda;
        GameState.I.totalADPGenerada = 0.0;
        GameState.I.totalWHFGenerada = 0.0;
        GameState.I.researchGlobalLEMult = 1.0;


        // F7.5: meta-upgrades
        GameState.I.metaEmBoost1Bought = data.metaEmBoost1Bought;

        
        // 🆕 Guardar niveles de edificios para aplicarlos después
        SaveService.LastLoadedBuildingLevels = data.buildingLevels ?? new List<SavedBuildingLevel>();
        GameState.I.fragmentCondensation = data.fragmentCondensation;
        GameState.I.fragmentConfinement = data.fragmentConfinement;
        GameState.I.fragmentResidualInterference = data.fragmentResidualInterference;
        GameState.I.fragmentCondensationProgress = System.Math.Max(
            0.0, data.fragmentCondensationProgress);
        GameState.I.fragmentConfinementProgress = System.Math.Max(
            0.0, data.fragmentConfinementProgress);
        GameState.I.fragmentResidualInterferenceProgress = System.Math.Max(
            0.0, data.fragmentResidualInterferenceProgress);

        GameState.I.experimentalHallazgos = data.experimentalHallazgos;
        GameState.I.experimentalMuestras = data.experimentalMuestras;
        GameState.I.experimentalLecturasIncompletas = data.experimentalLecturasIncompletas;
        GameState.I.experimentalCompuestosUtiles = data.experimentalCompuestosUtiles;
        GameState.I.synthesisCoreFusionCounter = data.synthesisCoreFusionCounter;
        GameState.I.fusionInstability = System.Math.Clamp(
            data.fusionInstability, 0, GameState.FusionInstabilityMax);
        GameState.I.fusionCooldownRemainingSeconds =
            double.IsNaN(data.fusionCooldownRemainingSeconds) ||
            double.IsInfinity(data.fusionCooldownRemainingSeconds)
                ? 0.0
                : System.Math.Max(0.0, data.fusionCooldownRemainingSeconds);
        GameState.I.pendingFusion = data.pendingFusion ??
            new ExperimentalPendingFusionState();
        GameState.I.chronalSeedSlots = data.chronalSeedSlots ?? new List<ChronalSeedSlotState>();
        GameState.I.chronalMatureSeedsStored = data.chronalMatureSeedsStored;
        GameState.I.EnsureChronalSeedSlots();
        GameState.I.chronalInstant = data.chronalInstant ?? new ChronalInstantState();
        GameState.I.chronalMaterializedInstants = data.chronalMaterializedInstants;
        GameState.I.chronalPureInstants = data.chronalPureInstants;
        GameState.I.chronalStableInstants = data.chronalStableInstants;
        GameState.I.chronalForcedInstants = data.chronalForcedInstants;
        GameState.I.chronalArchivedInstants = data.chronalArchivedInstants;

        GameState.I.experimentalMixLog = data.experimentalMixLog ?? new List<ExperimentalMixLogEntry>();
        GameState.I.guidedSynthesisIntent = Mathf.Clamp(data.guidedSynthesisIntent, 0, 4);
        GameState.I.triangleSystemUnlocked = data.triangleSystemUnlocked;
        GameState.I.triangleActivationTutorialSeen =
            data.triangleActivationTutorialSeen;
        GameState.I.trianglePrimaryBuildingId = data.trianglePrimaryBuildingId ?? "";
        GameState.I.triangleReinforcementBuildingId = data.triangleReinforcementBuildingId ?? "";
        GameState.I.triangleAlterationBuildingId = data.triangleAlterationBuildingId ?? "";
        GameState.I.SanitizeTriangleConfiguration();

        if (MachineManager.I != null)
        {
            MachineManager.I.LoadProgressFromSave(data);
        }

        if (ConvergenceSystem.IsFutureVersion(GameState.I.convergence))
        {
            Debug.LogError("[SaveService] Save de Convergencia de una versiÃ³n futura; no se sobrescribirÃ¡.");
            return;
        }
        HasLoadFailure = false;
        // El recovery puede reiniciar el juego base: necesita la MÃ¡quina restaurada.
        ConvergenceCircuitSystem.RecoverTransaction(GameState.I);

        if (F2UpgradeManager.I != null)
        {
            F2UpgradeManager.I.ApplyLoadedPurchasedTiers(
                data.f2UpgradeTiers, data.f2ProgressionMigrationVersion);
        }
        UpgradeStudySystem.ApplyLoadedState(GameState.I, data.upgradeStudies);

        GameState.I.SanitizeTriangleCircuit(data.triangleCircuitSaveVersion < 1);

        // En un arranque en frio BuildingListUI todavia no ha registrado los
        // productores. Deben reconstruirse antes del cálculo offline para que
        // los niveles guardados sí generen recursos durante la ausencia.
        GameState.I.PrepareBuildingLevelsForOffline(
            LastLoadedBuildingLevels,
            BuildingDatabase.I != null ? BuildingDatabase.I.buildings : null);

        PresentationReturnSnapshot presentationBeforeOffline =
            PresentationReturnReportService.Capture(GameState.I);
        long nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        double offlineSeconds = Math.Max(0.0, nowUnix - data.lastUnix);
        GameState.I.fusionCooldownRemainingSeconds = Math.Max(
            0.0, GameState.I.fusionCooldownRemainingSeconds - offlineSeconds);
        ConvergenceTelemetrySystem.RecordOfflineElapsed(GameState.I, offlineSeconds);
        double baseOfflineApplied = Math.Min(
            offlineSeconds, Dimension1System.DefaultOfflineCapSeconds);
        // Con D3 abierta, DiagnosticSystem es el único dueño del reloj del análisis.
        if (!GameState.I.dimension03Unlocked && MachineManager.I != null)
            MachineManager.I.ApplyOfflineAnalysis(baseOfflineApplied);

        // Sistema de dimensiones
        // minería offline con cap inicial de 12 horas.
        bool d3RunsExternalOffline =
            D3AutomationSystem.CanRunAutomationOffline(GameState.I);
        double d1OfflineApplied = d3RunsExternalOffline
            ? Math.Min(offlineSeconds, Dimension1System.DefaultOfflineCapSeconds)
            : Dimension1System.ApplyOfflineMining(GameState.I, offlineSeconds);
        double d2OfflineApplied = Dimension2System.ApplyOfflineProgress(GameState.I, offlineSeconds);
        var totalBaseReport = new TriangleOfflineReport();
        double d3OfflineApplied = Dimension3System.ApplyOfflineProgress(GameState.I, offlineSeconds,
            seconds =>
            {
                var part = GameState.I.ApplyOfflineBaseProgress(seconds);
                totalBaseReport.appliedSeconds += part.appliedSeconds;
                totalBaseReport.circuit = part.circuit;
                totalBaseReport.leGained += part.leGained;
                totalBaseReport.tracesGained += part.tracesGained;
                totalBaseReport.triangleEnergyGained += part.triangleEnergyGained;
                totalBaseReport.condensationGained += part.condensationGained;
                totalBaseReport.confinementGained += part.confinementGained;
                totalBaseReport.residualInterferenceGained += part.residualInterferenceGained;
                totalBaseReport.phaseAnalysisSecondsApplied += part.phaseAnalysisSecondsApplied;
                totalBaseReport.hasResults |= part.hasResults;
            });
        if (GameState.I.dimension03Unlocked)
            GameState.I.lastTriangleOfflineReport = totalBaseReport;
        if (!GameState.I.dimension03Unlocked)
            GameState.I.ApplyOfflineBaseProgress(baseOfflineApplied);

        // El informe se prepara al final para que el balance incluya todas las
        // fuentes offline sin volver a entregar ni recalcular recompensas.
        PresentationReturnReportService.Prepare(
            presentationBeforeOffline,
            GameState.I,
            offlineSeconds,
            d2OfflineApplied,
            d3OfflineApplied,
            baseOfflineApplied);

        #if UNITY_EDITOR
        if (d1OfflineApplied > 0.0)
        {
            Debug.Log("[D1] Offline minería aplicado: " + d1OfflineApplied.ToString("0") + " segundos.");
        }

        if (d2OfflineApplied > 0.0)
        {
            Debug.Log("[D2] Ventana offline preparada: " + d2OfflineApplied.ToString("0") + " segundos.");
        }

        if (d3OfflineApplied > 0.0)
        {
            Debug.Log("[D3] Progreso offline aplicado: " + d3OfflineApplied.ToString("0") + " segundos.");
        }
        #endif

        TabsUI tabsUI = FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        if (tabsUI != null)
        
        {
            tabsUI.RefreshGenerationLayoutFromOutside();
            tabsUI.RefreshDimension1ButtonVisibility();
            tabsUI.RefreshDimension2ButtonVisibility();
            tabsUI.RefreshDimension3ButtonVisibility();
            tabsUI.RefreshPrestigeButtonVisibility();
        }

        // Nos aseguramos de que el máximo quede coherente
        GameState.I.ActualizarMaxLE();

        LastLoadedResearchIds = data.purchasedResearchIds ?? new List<string>();
        LastLoadedAchievementIds = data.unlockedAchievementIds ?? new List<string>();

        if (ResearchManager.I != null)
        {
            ResearchManager.I.ApplyLoadedResearch(LastLoadedResearchIds);
        }


        if (AchievementManager.I != null)
        {
            AchievementManager.I.ApplyLoadedAchievements(LastLoadedAchievementIds);
        }

        if (!SuppressWritesForVisualQa && requiresImmediateSave && !TrySave(out string recoverySaveError))
            Debug.LogError("[SaveService] La partida se cargo, pero no se pudo " +
                "confirmar la recuperacion: " + recoverySaveError);

#if UNITY_EDITOR
        Debug.Log("[SaveService] Loaded.");
#endif
    }

    public static void ApplyDimensionDiscoveryMigration(
        GameState state,
        int savedVersion)
    {
        if (state == null)
            return;

        int unlockedCount = CountUnlockedDimensions(state);

        // Version 0 corresponde al modelo antiguo, que sólo registraba que
        // Prestigio 1 había ocurrido y activaba las tres dimensiones juntas.
        if (savedVersion < 1 && state.hasDonePrestige1 && unlockedCount == 0)
        {
            state.UnlockDimensionSystemAfterPrestige1();
            state.prestige1CurrentDimensionId = 0;
            return;
        }

        if (unlockedCount == 0)
        {
            state.prestige1CurrentDimensionId = 0;
            return;
        }

        // Con las tres dimensiones descubiertas, la dimensión actual no vuelve
        // a controlar una selección de Prestigio 1.
        if (unlockedCount == 3)
        {
            state.prestige1CurrentDimensionId = 0;
            return;
        }

        if (state.prestige1CurrentDimensionId > 0 &&
            state.IsDimensionUnlockedAfterPrestige1(
                state.prestige1CurrentDimensionId))
        {
            return;
        }

        if (unlockedCount == 1)
        {
            state.prestige1CurrentDimensionId = GetFirstUnlockedDimensionId(state);
            return;
        }

        if (unlockedCount == 2)
        {
            int incompleteDimensionId = GetOnlyIncompleteUnlockedDimensionId(state);
            state.prestige1CurrentDimensionId = incompleteDimensionId > 0
                ? incompleteDimensionId
                : GetFirstUnlockedDimensionId(state);
            return;
        }

        state.prestige1CurrentDimensionId = 0;
    }

    private static int CountUnlockedDimensions(GameState state)
    {
        int count = 0;
        for (int dimensionId = 1; dimensionId <= 3; dimensionId++)
        {
            if (state.IsDimensionUnlockedAfterPrestige1(dimensionId))
                count++;
        }
        return count;
    }

    private static int GetFirstUnlockedDimensionId(GameState state)
    {
        for (int dimensionId = 1; dimensionId <= 3; dimensionId++)
        {
            if (state.IsDimensionUnlockedAfterPrestige1(dimensionId))
                return dimensionId;
        }
        return 0;
    }

    private static int GetOnlyIncompleteUnlockedDimensionId(GameState state)
    {
        int result = 0;
        for (int dimensionId = 1; dimensionId <= 3; dimensionId++)
        {
            if (!state.IsDimensionUnlockedAfterPrestige1(dimensionId) ||
                state.IsDimensionMilestoneComplete(dimensionId))
            {
                continue;
            }

            if (result != 0)
                return 0;

            result = dimensionId;
        }
        return result;
    }

    public static bool TryWriteAtomicJson(string savePath, string json, out string error)
    {
        error = null;
        string tempPath = savePath + ".tmp";
        string backupPath = savePath + ".bak";
        try
        {
            if (FailureInjectionPoint == SaveFailureInjectionPoint.BeforeTempWrite)
                throw new IOException("Fallo inyectado antes del temporal.");
            string directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(json);
                writer.Flush();
                stream.Flush(true);
            }
            if (FailureInjectionPoint == SaveFailureInjectionPoint.AfterTempWrite)
                throw new IOException("Fallo inyectado despuÃ©s del temporal.");
            if (!TryReadSaveData(tempPath, out _))
                throw new InvalidDataException("El save temporal no se puede deserializar.");
            if (FailureInjectionPoint == SaveFailureInjectionPoint.AfterTempValidation)
                throw new IOException("Fallo inyectado despuÃ©s de validar el temporal.");
            if (FailureInjectionPoint == SaveFailureInjectionPoint.BeforeReplace)
                throw new IOException("Fallo inyectado antes de reemplazar el save.");

            if (File.Exists(savePath))
                File.Replace(tempPath, savePath, backupPath, true);
            else
                File.Move(tempPath, savePath);
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
        finally
        {
            FailureInjectionPoint = SaveFailureInjectionPoint.None;
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    public static bool TryReadSaveData(string path, out SaveData data)
    {
        data = null;
        try
        {
            if (!File.Exists(path)) return false;
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(json);
            return data != null;
        }
        catch (Exception) { data = null; return false; }
    }

    public static string GetHistoricalSavePath(string savePath, int index)
    {
        if (index < 1 || index > HistoricalBackupCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return savePath + ".history." + index;
    }

    public static bool TryFindRecoverableSave(
        string savePath,
        out SaveData data,
        out string sourcePath,
        out string error)
    {
        data = null;
        sourcePath = null;
        error = null;

        var candidates = new List<string>
        {
            savePath,
            savePath + ".bak"
        };
        for (int index = 1; index <= HistoricalBackupCount; index++)
            candidates.Add(GetHistoricalSavePath(savePath, index));

        foreach (string candidatePath in candidates)
        {
            if (!TryReadSaveData(candidatePath, out SaveData candidate))
                continue;

            if (candidate.saveSchemaVersion > CurrentSaveSchemaVersion)
            {
                error = "El archivo " + candidatePath +
                    " pertenece a un schema futuro (" +
                    candidate.saveSchemaVersion + ").";
                return false;
            }

            if (!IsPlausibleSaveData(candidate))
                continue;

            data = candidate;
            sourcePath = candidatePath;
            return true;
        }

        error = "save.json, save.json.bak y los tres historicos no son legibles.";
        return false;
    }

    public static bool TryCreateHistoricalSnapshot(
        string savePath,
        string sourcePath,
        out string error)
    {
        error = null;
        try
        {
            if (!TryReadSaveData(sourcePath, out SaveData source) ||
                !IsPlausibleSaveData(source))
                throw new InvalidDataException(
                    "La fuente del respaldo no contiene una partida valida.");

            string directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            for (int index = HistoricalBackupCount; index >= 2; index--)
            {
                string previous = GetHistoricalSavePath(savePath, index - 1);
                string destination = GetHistoricalSavePath(savePath, index);
                if (File.Exists(previous))
                    File.Copy(previous, destination, true);
            }

            string newest = GetHistoricalSavePath(savePath, 1);
            if (!PathsEqual(sourcePath, newest))
                File.Copy(sourcePath, newest, true);

            if (!TryReadSaveData(newest, out SaveData confirmed) ||
                !IsPlausibleSaveData(confirmed))
                throw new InvalidDataException(
                    "El respaldo historico no supero la validacion final.");

            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return false;
        }
    }

    private static bool HasAnySaveCandidate(string savePath)
    {
        if (File.Exists(savePath) || File.Exists(savePath + ".bak"))
            return true;
        for (int index = 1; index <= HistoricalBackupCount; index++)
            if (File.Exists(GetHistoricalSavePath(savePath, index))) return true;
        return false;
    }

    private static bool IsPlausibleSaveData(SaveData data)
    {
        return data != null && data.lastUnix > 0L &&
            !double.IsNaN(data.LE) && !double.IsInfinity(data.LE) &&
            !double.IsNaN(data.Traces) && !double.IsInfinity(data.Traces);
    }

    private static bool PathsEqual(string left, string right)
    {
        if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right))
            return false;
        return string.Equals(
            Path.GetFullPath(left).TrimEnd(Path.DirectorySeparatorChar),
            Path.GetFullPath(right).TrimEnd(Path.DirectorySeparatorChar),
            StringComparison.OrdinalIgnoreCase);
    }

    private static void MigrateSaveData(SaveData data)
    {
        if (data == null) return;
        if (data.saveSchemaVersion < CurrentSaveSchemaVersion)
            data.saveSchemaVersion = CurrentSaveSchemaVersion;
    }

    public static void ApplyRemovedLegacyResourcesMigration(
        SaveData data, GameState state)
    {
        if (data != null)
        {
            data.EM = 0.0;
            data.emMult = 0.0;
            data.ADP = 0.0;
            data.WHF = 0.0;
            data.totalADPGenerada = 0.0;
            data.totalWHFGenerada = 0.0;
            data.removedLegacyResourcesVersion = RemovedLegacyResourcesVersion;
        }

        if (state == null) return;
        state.EM = 0.0;
        state.emMult = 0.0;
        state.ADP = 0.0;
        state.WHF = 0.0;
        state.totalADPGenerada = 0.0;
        state.totalWHFGenerada = 0.0;
        state.researchGlobalLEMult = 1.0;
    }

    private void InitNewGame()
    {
        if (GameState.I == null) return;
        HasLoadFailure = false;

        // ✅ Starter: lo mínimo para poder comprar el primer edificio (coste 10)
        GameState.I.LE = 10.0;
        GameState.I.VP = 0.0;
        GameState.I.Traces = 0.0;

        GameState.I.EM = 0.0;
        GameState.I.emMult = 0.0;
        GameState.I.researchGlobalLEMult = 1.0;

        GameState.I.baseLEps = 0.0;
        GameState.I.phaseModulatorMode = PhaseModulatorMode.None;
        GameState.I.phaseModulatorCalibration = 0f;
        GameState.I.trianglePersistenceReserveSeconds = 0.0;
        GameState.I.triangleActiveCircuit = TriangleCircuitType.None;
        GameState.I.triangleSynchronization = 0f;
        GameState.I.triangleSynchronizationBaseRatePerSecond = 0.0;
        GameState.I.triangleEnergy = 0.0;
        GameState.I.experimentalChamberUnlocked = false;
        GameState.I.experimentalChamberInitialPackGranted = false;
        // Sistema de dimensiones
        GameState.I.dimension01Unlocked = false;
        GameState.I.dimension02Unlocked = false;
        GameState.I.dimension03Unlocked = false;
        GameState.I.dimension2 = Dimension2System.CreateInitialState();
        GameState.I.dimension3 = Dimension3System.CreateInitialState();
        GameState.I.convergence = ConvergenceSystem.CreateInitialState();
        GameState.I.dimension1Metals = new List<D1MetalAmount>();
        GameState.I.dimension1Planets = new List<D1PlanetState>();
        GameState.I.dimension1Sectors = new List<D1SectorState>();
        GameState.I.dimension1SelectedSectorId = "";
        GameState.I.dimension1ActiveScanSectorId = "";
        GameState.I.dimension1Ships = new List<D1ShipState>();
        GameState.I.dimension1CoordinatedMissionProgressVersion =
            Dimension1System.Dimension1CoordinatedMissionProgressVersion;
        GameState.I.dimension1CompletedCoordinatedMissions = 0;
        GameState.I.dimension1ArkProgressVersion =
            Dimension1System.Dimension1ArkProgressVersion;
        GameState.I.dimension1ArkInvestigated = false;
        GameState.I.dimension1CentralSyncMissions = new List<D1CentralSyncMissionState>();
        GameState.I.dimension1CentralSyncEstablished = false;
        GameState.I.dimension1CentralAccessKeyObtained = false;
        GameState.I.dimension1CentralAccessKeyLogSeen = false;
        GameState.I.dimension1ArkFinalMissionActive = false;
        GameState.I.dimension1ArkFinalMissionRemainingSeconds = 0.0;
        GameState.I.dimension1ArkFinalMissionTotalSeconds = 0.0;
        GameState.I.dimension1ArkFinalMissionShipIds = new List<string>();
        GameState.I.dimension1GalacticAnchorDiscovered = false;
        GameState.I.dimension1ScannedDestinations = new List<D1ScannedDestinationState>();
        GameState.I.dimension1PreviousScannedDestinationIds = new List<string>();
        GameState.I.dimension1ScanActive = false;
        GameState.I.dimension1ScanRemainingSeconds = 0.0;
        GameState.I.dimension1ScanTotalSeconds = 0.0;
        GameState.I.dimension1ScannerLevel = Dimension1System.SimpleScannerMinLevel;
        GameState.I.dimension1ScannerProgressVersion =
            Dimension1System.SimpleScannerProgressVersion;
        GameState.I.dimension1LastExplorationDestinationId = "";
        GameState.I.dimension1LastExplorationRewards = new List<D1MetalAmount>();
        GameState.I.dimension1LastExplorationSpecificBlueprints = new List<D1BlueprintAmount>();
        GameState.I.dimension1LastExplorationRelics = new List<D1RelicRewardEntry>();
        GameState.I.dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>();
        GameState.I.dimension1Blueprints = new List<D1BlueprintAmount>();
        GameState.I.dimension1Relics = new List<D1RelicState>();
        GameState.I.dimension1RelicPityStates = new List<D1RelicPityState>();
        GameState.I.dimension1RelicProgressVersion =
            Dimension1System.Dimension1RelicProgressVersion;
        GameState.I.dimension1TreeNodes = new List<D1TreeNodeState>();
        GameState.I.dimension1TreeProgressVersion =
            Dimension1System.Dimension1TreeProgressVersion;
        GameState.I.d1TreePoints = 0;
        GameState.I.d1TreePointsProgressBaseline = 0;
        GameState.I.dimension1BlueprintFragments = 0;
        GameState.I.dimension1LastExplorationBlueprintFragments = 0;
        GameState.I.dimension1LastExplorationResultId = 0;
        GameState.I.EnsureDimension1State();
        GameState.I.EnsureDimension2State();
        GameState.I.EnsureDimension3State();

        GameState.I.fragmentCondensation = 0;
        GameState.I.fragmentConfinement = 0;
        GameState.I.fragmentResidualInterference = 0;

        GameState.I.fragmentCondensationProgress = 0.0;
        GameState.I.fragmentConfinementProgress = 0.0;
        GameState.I.fragmentResidualInterferenceProgress = 0.0;

        GameState.I.experimentalHallazgos = 0;
        GameState.I.experimentalMuestras = 0;
        GameState.I.experimentalLecturasIncompletas = 0;
        GameState.I.experimentalCompuestosUtiles = 0;
        GameState.I.synthesisCoreFusionCounter = 0;
        GameState.I.fusionInstability = 0;
        GameState.I.fusionCooldownRemainingSeconds = 0.0;
        GameState.I.pendingFusion = new ExperimentalPendingFusionState();
        GameState.I.chronalArchivedInstants = 0;

        GameState.I.experimentalMixLog = new List<ExperimentalMixLogEntry>();
        GameState.I.guidedSynthesisIntent = 0;
        GameState.I.triangleSystemUnlocked = false;
        GameState.I.triangleActivationTutorialSeen = false;
        GameState.I.triangleActiveCircuit = TriangleCircuitType.None;
        GameState.I.triangleSynchronization = 0f;
        GameState.I.triangleSynchronizationBaseRatePerSecond = 0.0;
        GameState.I.triangleEnergy = 0.0;
        GameState.I.trianglePrimaryBuildingId = "";
        GameState.I.triangleReinforcementBuildingId = "";
        GameState.I.triangleAlterationBuildingId = "";
        UpgradeStudySystem.ResetForNewRun(GameState.I);

        GameState.I.prestige1Count = 0;
        GameState.I.hasDonePrestige1 = false;
        GameState.I.prestige1CurrentDimensionId = 0;

        GameState.I.maxLEAlcanzado = 0.0;

        GameState.I.ADP = 0.0;
        GameState.I.WHF = 0.0;

        // Meta-progreso se mantiene en 0 para un save nuevo
        GameState.I.Lambda = 0.0;
        GameState.I.totalADPGenerada = 0.0;
        GameState.I.totalWHFGenerada = 0.0;

        GameState.I.metaEmBoost1Bought = false;

        // Limpiar caches de load para evitar arrastrar algo raro
        LastLoadedResearchIds = new List<string>();
        LastLoadedAchievementIds = new List<string>();
        LastLoadedBuildingLevels = new List<SavedBuildingLevel>();

        if (MachineManager.I != null)
        {
            MachineManager.I.ResetOperationalProgress();
        }

        // Aplica listas vacías si existen managers
        if (ResearchManager.I != null) ResearchManager.I.ApplyLoadedResearch(LastLoadedResearchIds);
        if (AchievementManager.I != null) AchievementManager.I.ApplyLoadedAchievements(LastLoadedAchievementIds);

        // Dejar max coherente
        GameState.I.ActualizarMaxLE();

        // ✅ Crear el save inmediatamente (en Android es clave)
        Save();
    }

    [ContextMenu("Reset Save (simple)")]
    public void ResetSave()
    {
        if (!TryResetToNewGame(out string error))
            throw new IOException(error);
    }

    public bool TryResetToNewGame(out string error)
    {
        error = null;
        if (SuppressWritesForVisualQa)
        {
            error = "El reinicio está bloqueado durante una captura QA protegida.";
            return false;
        }
        if (GameState.I == null)
        {
            error = "GameState.I es null.";
            return false;
        }

        try
        {
            HasLoadFailure = false;

            // InitNewGame es la autoridad de una partida nueva, pero durante un
            // reset en caliente también hay que vaciar estados runtime que no
            // existen todavía durante el primer arranque normal.
            GameState.I.DebugResetRunState();
            if (F2UpgradeManager.I != null)
                F2UpgradeManager.I.DebugResetAllPurchases();
            if (MachineManager.I != null)
                MachineManager.I.ResetOperationalProgress();

            InitNewGame();

            if (!TryReadSaveData(SavePath, out _))
            {
                error = "La partida nueva no pudo verificarse después del reinicio.";
                return false;
            }

            // El guardado atómico conserva el save anterior hasta que el nuevo
            // ya es legible. Sólo entonces se eliminan sus copias recuperables.
            DeleteIfPresent(SaveBackupPath);
            DeleteIfPresent(SavePath + ".tmp");
            for (int index = 1; index <= HistoricalBackupCount; index++)
                DeleteIfPresent(GetHistoricalSavePath(SavePath, index));
            historyCapturedThisSession = false;

#if UNITY_EDITOR
            Debug.Log("[SaveService] Partida nueva creada después del reinicio QA.");
#endif
            return true;
        }
        catch (Exception exception)
        {
            error = "No se pudo reiniciar la partida: " + exception.Message;
            return false;
        }
    }

    private static void DeleteIfPresent(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    #if UNITY_EDITOR
    [ContextMenu("DEBUG: Reset Save (completo)")]
    private void DebugResetSave()
    {
        try
        {
            // 1) Borrar el archivo de save en disco
            if (File.Exists(SavePath))
            {   
                File.Delete(SavePath);
                Debug.Log($"[SaveService] DEBUG: Save borrado en {SavePath}");
            }
            else
            {
                Debug.Log("[SaveService] DEBUG: No había archivo de save para borrar.");
            }

            // 2) Limpiar los buffers estáticos en memoria
            LastLoadedResearchIds    = null;
            LastLoadedAchievementIds = null;
            LastLoadedBuildingLevels = null;

            // 3) Resetear el GameState en memoria (LE, base, edificios, prestigio, etc.)
            if (GameState.I != null)
            {
                GameState.I.DebugResetRunState();
                GameState.I.Traces = 0.0;

                GameState.I.experimentalChamberUnlocked = false;
                GameState.I.experimentalChamberInitialPackGranted = false;

                GameState.I.fragmentCondensation = 0;
                GameState.I.fragmentConfinement = 0;
                GameState.I.fragmentResidualInterference = 0;

                GameState.I.fragmentCondensationProgress = 0.0;
                GameState.I.fragmentConfinementProgress = 0.0;
                GameState.I.fragmentResidualInterferenceProgress = 0.0;

                GameState.I.experimentalHallazgos = 0;
                GameState.I.experimentalMuestras = 0;
                GameState.I.experimentalLecturasIncompletas = 0;
                GameState.I.experimentalCompuestosUtiles = 0;
                GameState.I.fusionInstability = 0;
                GameState.I.fusionCooldownRemainingSeconds = 0.0;
                GameState.I.pendingFusion = new ExperimentalPendingFusionState();
                GameState.I.chronalArchivedInstants = 0;

                GameState.I.experimentalMixLog = new List<ExperimentalMixLogEntry>();
                GameState.I.guidedSynthesisIntent = 0;
                // Sistema de dimensiones
                GameState.I.ResetDimensionSystemState();
            }

                if (F2UpgradeManager.I != null)
            {
                F2UpgradeManager.I.DebugResetAllPurchases();
            }

            if (MachineManager.I != null)
            {
                MachineManager.I.ResetOperationalProgress();
            }

            // 4) Resetear logros en memoria
            if (AchievementManager.I != null)
            {
                // Marcar TODOS los logros como bloqueados
                foreach (var kv in AchievementManager.I.states)
                {
                    kv.Value.unlocked = false;
                }

                // Recalcular bonus global
                AchievementManager.I.SendMessage(
                    "RecalculateBonuses",
                    SendMessageOptions.DontRequireReceiver
                );

                // Refrescar la UI de la lista de logros si está abierta
                var listUI = FindFirstObjectByType<AchievementListUI>();
                if (listUI != null)
                {
                    listUI.Refresh();
                }
            }

            if (TabsUI.Instance != null)
            {
                TabsUI.Instance.RefreshRoom2ButtonVisibility();
                TabsUI.Instance.RefreshDimension1ButtonVisibility();
                TabsUI.Instance.RefreshDimension2ButtonVisibility();
                TabsUI.Instance.RefreshDimension3ButtonVisibility();
                TabsUI.Instance.RefreshPrestigeButtonVisibility();
                TabsUI.Instance.ShowGeneracion();
                TabsUI.Instance.RefreshGenerationLayoutFromOutside();
            }

            Debug.Log("[SaveService] DEBUG: Reset completo aplicado en memoria.");
        }
        catch (Exception ex)
        {
            Debug.LogError("[SaveService] DEBUG: Error al borrar el save: " + ex.Message);
        }
    }



    [ContextMenu("DEBUG: Save Now")]
    private void DebugSaveNow()
    {
        Save();
        Debug.Log("[SaveService] DEBUG: Save Now ejecutado.");
    }
#endif
}
