using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{

        // F3 / Cuarto 2 - recursos
    public int fragmentCondensation;
    public int fragmentConfinement;
    public int fragmentResidualInterference;

    public int experimentalHallazgos;
    public int experimentalMuestras;
    public int experimentalLecturasIncompletas;
    public int experimentalCompuestosUtiles;
    public int synthesisCoreFusionCounter;
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
    public string trianglePrimaryBuildingId;
    public string triangleReinforcementBuildingId;
    public string triangleAlterationBuildingId;
    public List<string> machineRepairedNodeIds = new List<string>();
    public List<string> machineAnalyzedNodeIds = new List<string>();
    public bool machineIntroSeen;
    public bool machineUnlocked;
    public bool machineFusionPanelUnlocked;
    public bool machineAllZonesUnlocked;
    public double LE;
    public double Traces;
    public double VP;
    // Mid-game: recurso EM
    public double EM;
    public double emMult;
    public int phaseModulatorMode;
    public float phaseModulatorCalibration;

    // public float trianglePersistenceMaturation; // lógica vieja de Persistencia
    public double trianglePersistenceReserveSeconds;
    // F6.1: Moneda de prestigio

    // Prestigio 1 - Convergencia
    public int prestige1Count;
    public bool hasDonePrestige1;
    public int prestige1Points;
    public int prestige1BestClaimedPreviewPoints;
    // F6.1: Máximo LE alcanzado en el run
    public double maxLEAlcanzado;
    public List<SavedF2UpgradeTier> f2UpgradeTiers = new();
    // F7: Recursos late-game
    public double ADP;
    public double WHF;
    // F7: Prestigio 2 (Lambda)
    public double Lambda;
    // F7: Estadísticas acumuladas para meta-prestigio
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
    public bool dimension01Unlocked;
    public bool dimension02Unlocked;
    public bool dimension03Unlocked;
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
}

public class SaveService : MonoBehaviour
{
    public static List<string> LastLoadedResearchIds;
    public static List<string> LastLoadedAchievementIds;
    public static List<SavedBuildingLevel> LastLoadedBuildingLevels;



    public static SaveService I { get; private set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    [Tooltip("Autosave cada N segundos.")]
    public int autosaveSeconds = 30;

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
        if (pause)
        {
#if UNITY_EDITOR
            Debug.Log("[SaveService] OnApplicationPause(true) -> Save()");
#endif
            Save();
        }
    }

    public void Save()
    {
        if (GameState.I == null)
        {
        #if UNITY_EDITOR
            Debug.LogWarning("[SaveService] Save() cancelado: GameState.I es null.");
        #endif
            return;
        }

        var data = new SaveData
        {
        LE = GameState.I.LE,
        Traces = GameState.I.Traces,
        VP = GameState.I.VP,
        EM = GameState.I.EM,
        emMult = GameState.I.emMult,
        baseLEps = GameState.I.baseLEps,
        phaseModulatorMode = (int)GameState.I.phaseModulatorMode,
        phaseModulatorCalibration = GameState.I.phaseModulatorCalibration,
        trianglePersistenceReserveSeconds = GameState.I.trianglePersistenceReserveSeconds,

        // Prestigio 1 - Convergencia
        prestige1Count = GameState.I.prestige1Count,
        hasDonePrestige1 = GameState.I.hasDonePrestige1,
        prestige1Points = GameState.I.prestige1Points,
        prestige1BestClaimedPreviewPoints = GameState.I.prestige1BestClaimedPreviewPoints,

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
        dimension01Unlocked = GameState.I.dimension01Unlocked,
        dimension02Unlocked = GameState.I.dimension02Unlocked,
        dimension03Unlocked = GameState.I.dimension03Unlocked,
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

        fragmentCondensation = GameState.I.fragmentCondensation,
        fragmentConfinement = GameState.I.fragmentConfinement,
        fragmentResidualInterference = GameState.I.fragmentResidualInterference,

        experimentalHallazgos = GameState.I.experimentalHallazgos,
        experimentalMuestras = GameState.I.experimentalMuestras,
        experimentalLecturasIncompletas = GameState.I.experimentalLecturasIncompletas,
        experimentalCompuestosUtiles = GameState.I.experimentalCompuestosUtiles,
        synthesisCoreFusionCounter = GameState.I.synthesisCoreFusionCounter,
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
        File.WriteAllText(SavePath, json);

#if UNITY_EDITOR
        Debug.Log($"[SaveService] Saved: {SavePath}");
#endif
    }

    public void Load()
    {
#if UNITY_EDITOR
        Debug.Log($"[SaveService] Load() llamado. Existe archivo? {File.Exists(SavePath)} ({SavePath})");
#endif

        if (GameState.I == null) return;

        if (!File.Exists(SavePath))
        {
            InitNewGame();
            return;
        }

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);

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
        GameState.I.EM = data.EM;
        GameState.I.emMult = data.emMult;
        GameState.I.baseLEps = data.baseLEps;
        GameState.I.phaseModulatorMode = (PhaseModulatorMode)data.phaseModulatorMode;
        GameState.I.phaseModulatorCalibration = data.phaseModulatorCalibration;
        // GameState.I.trianglePersistenceMaturation = data.trianglePersistenceMaturation;
        GameState.I.trianglePersistenceReserveSeconds = data.trianglePersistenceReserveSeconds;
        GameState.I.experimentalChamberUnlocked = data.experimentalChamberUnlocked;
        GameState.I.experimentalChamberInitialPackGranted = data.experimentalChamberInitialPackGranted;
        // Sistema de dimensiones
        GameState.I.dimension01Unlocked = data.dimension01Unlocked;
        GameState.I.dimension02Unlocked = data.dimension02Unlocked;
        GameState.I.dimension03Unlocked = data.dimension03Unlocked;
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

        // Prestigio 1 - Convergencia
        GameState.I.prestige1Count = data.prestige1Count;
        GameState.I.hasDonePrestige1 = data.hasDonePrestige1;
        GameState.I.prestige1Points = data.prestige1Points;
        GameState.I.prestige1BestClaimedPreviewPoints = data.prestige1BestClaimedPreviewPoints;
        GameState.I.EnsureDimension1State();

        // Migración para partidas viejas:
        // si el jugador ya hizo Prestigio 1, el sistema de dimensiones debe quedar preparado.
        if (GameState.I.hasDonePrestige1)
        {
            GameState.I.UnlockDimensionSystemAfterPrestige1();
        }

        // F6.1: prestigio viejo
        GameState.I.maxLEAlcanzado = data.maxLEAlcanzado;

        // F7: recursos late-game y Lambda
        GameState.I.ADP = data.ADP;
        GameState.I.WHF = data.WHF;
        GameState.I.Lambda = data.Lambda;
        GameState.I.totalADPGenerada = data.totalADPGenerada;
        GameState.I.totalWHFGenerada = data.totalWHFGenerada;


        // F7.5: meta-upgrades
        GameState.I.metaEmBoost1Bought = data.metaEmBoost1Bought;

        
        // 🆕 Guardar niveles de edificios para aplicarlos después
        SaveService.LastLoadedBuildingLevels = data.buildingLevels ?? new List<SavedBuildingLevel>();
        GameState.I.fragmentCondensation = data.fragmentCondensation;
        GameState.I.fragmentConfinement = data.fragmentConfinement;
        GameState.I.fragmentResidualInterference = data.fragmentResidualInterference;

        GameState.I.experimentalHallazgos = data.experimentalHallazgos;
        GameState.I.experimentalMuestras = data.experimentalMuestras;
        GameState.I.experimentalLecturasIncompletas = data.experimentalLecturasIncompletas;
        GameState.I.experimentalCompuestosUtiles = data.experimentalCompuestosUtiles;
        GameState.I.synthesisCoreFusionCounter = data.synthesisCoreFusionCounter;
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
        GameState.I.trianglePrimaryBuildingId = data.trianglePrimaryBuildingId ?? "";
        GameState.I.triangleReinforcementBuildingId = data.triangleReinforcementBuildingId ?? "";
        GameState.I.triangleAlterationBuildingId = data.triangleAlterationBuildingId ?? "";
        GameState.I.SanitizeTriangleConfiguration();

        if (MachineManager.I != null)
        {
            MachineManager.I.LoadProgressFromSave(data);
        }

        

        if (F2UpgradeManager.I != null)
        {
            F2UpgradeManager.I.ApplyLoadedPurchasedTiers(data.f2UpgradeTiers);
        }

        long nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        double offlineSeconds = Math.Max(0.0, nowUnix - data.lastUnix);

        GameState.I.ApplyOfflineTrianglePersistenceReserve(offlineSeconds);

        // Sistema de dimensiones
        // minería offline con cap inicial de 12 horas.
        double d1OfflineApplied = Dimension1System.ApplyOfflineMining(GameState.I, offlineSeconds);

        #if UNITY_EDITOR
        if (d1OfflineApplied > 0.0)
        {
            Debug.Log("[D1] Offline minería aplicado: " + d1OfflineApplied.ToString("0") + " segundos.");
        }
        #endif

        TabsUI tabsUI = FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        if (tabsUI != null)
        
        {
            tabsUI.RefreshGenerationLayoutFromOutside();
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

#if UNITY_EDITOR
        Debug.Log("[SaveService] Loaded.");
#endif
    }

    private void InitNewGame()
    {
        if (GameState.I == null) return;

        // ✅ Starter: lo mínimo para poder comprar el primer edificio (coste 10)
        GameState.I.LE = 10.0;
        GameState.I.VP = 0.0;

        GameState.I.EM = 0.0;
        GameState.I.emMult = 0.0;

        GameState.I.baseLEps = 0.0;
        GameState.I.phaseModulatorMode = PhaseModulatorMode.None;
        GameState.I.phaseModulatorCalibration = 0f;
        GameState.I.trianglePersistenceReserveSeconds = 0.0;
        GameState.I.experimentalChamberUnlocked = false;
        GameState.I.experimentalChamberInitialPackGranted = false;
        // Sistema de dimensiones
        GameState.I.dimension01Unlocked = false;
        GameState.I.dimension02Unlocked = false;
        GameState.I.dimension03Unlocked = false;
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
        GameState.I.prestige1Points = 0;
        GameState.I.prestige1BestClaimedPreviewPoints = 0;
        GameState.I.dimension1BlueprintFragments = 0;
        GameState.I.dimension1LastExplorationBlueprintFragments = 0;
        GameState.I.dimension1LastExplorationResultId = 0;
        GameState.I.EnsureDimension1State();

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
        GameState.I.chronalArchivedInstants = 0;

        GameState.I.experimentalMixLog = new List<ExperimentalMixLogEntry>();
        GameState.I.guidedSynthesisIntent = 0;
        GameState.I.triangleSystemUnlocked = false;
        GameState.I.trianglePrimaryBuildingId = "";
        GameState.I.triangleReinforcementBuildingId = "";
        GameState.I.triangleAlterationBuildingId = "";

        GameState.I.prestige1Count = 0;
        GameState.I.hasDonePrestige1 = false;

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
            MachineManager.I.ClearProgress();
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
        if (File.Exists(SavePath)) File.Delete(SavePath);
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
            GameState.I.chronalArchivedInstants = 0;

            GameState.I.experimentalMixLog = new List<ExperimentalMixLogEntry>();
            GameState.I.guidedSynthesisIntent = 0;
            GameState.I.triangleSystemUnlocked = false;
            GameState.I.trianglePrimaryBuildingId = "";
            GameState.I.triangleReinforcementBuildingId = "";
            GameState.I.triangleAlterationBuildingId = "";
            // Sistema de dimensiones
            GameState.I.ResetDimensionSystemState();
        }
        

        if (F2UpgradeManager.I != null)
        {
            F2UpgradeManager.I.DebugResetAllPurchases();
        }

        if (MachineManager.I != null)
        {
            MachineManager.I.ClearProgress();
        }

    #if UNITY_EDITOR
        Debug.Log("[SaveService] Save deleted.");
    #endif
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
                MachineManager.I.ClearProgress();
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
