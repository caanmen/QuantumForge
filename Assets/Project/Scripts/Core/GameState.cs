using UnityEngine;
using System.Collections.Generic;

    public enum PhaseModulatorMode

    {
        None = 0,
        Expansion = 1,
        Conservation = 2,
        Attunement = 3
    }

    public enum ExperimentalFragmentType
    {
        None = 0,
        Condensation = 1,
        Confinement = 2,
        ResidualInterference = 3
    }

    public enum ExperimentalCatalystType
    {
        None = 0,
        Alpha = 1,
        Beta = 2
    }

    public enum ExperimentalResultType
    {
        None = 0,
        Hallazgo = 1,
        Muestra = 2,
        LecturaIncompleta = 3,
        CompuestoUtil = 4
    }

    public enum ExperimentalMixState
    {
        NoRegistrada = 0,
        Inestable = 1,
        Parcial = 2,
        Catalogada = 3
    }

    [System.Serializable]
    public class ExperimentalMixLogEntry
    {
        public string mixKey;
        public string discoveredName;
        public int fragmentA;
        public int fragmentB;
        public int catalyst;
        public int lastResult;
        public int bestResult;
        public int mixState;
        public int timesExecuted;
        public int accumulatedErrors;
        public bool discovered;
    }

    public enum TriangleSlotRole
    {
        None = 0,
        Primary = 1,
        Reinforcement = 2,
        Alteration = 3
    }

    public enum TriangleProtocolType
    {
        None = 0,
        Impulso = 1,
        Sinergia = 2,
        Persistencia = 3
    }

    [System.Serializable]
    public class TriangleSlotState
    {
        public int role;
        public string buildingId;
    }

    [System.Serializable]
    public class ChronalSeedSlotState
    {
        public bool hasSeed;
        public double progressSeconds;
        public bool mature;
    }

    [System.Serializable]
    public class ChronalInstantState
    {
        public bool hasInstant;
        public double stability;
        public double tension;
    }

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Recursos básicos")]
    public double LE = 0.0;   // Luz de Energía (recurso principal)
    public double Traces = 0.0;    // Trazas (recurso secundario temprano)
    public double VP = 0.0;   // Vacuum Points (recurso raro, aún sin lógica)
    [Header("F3 / Cuarto 2 - Recursos experimentales")]
    public int fragmentCondensation = 0;
    public int fragmentConfinement = 0;
    public int fragmentResidualInterference = 0;
    public double fragmentCondensationProgress = 0.0;
    public double fragmentConfinementProgress = 0.0;
    public double fragmentResidualInterferenceProgress = 0.0;
    [Header("F3 / Cuarto 2 - Estado")]
    public bool experimentalChamberUnlocked = false;
    public bool experimentalChamberInitialPackGranted = false;
    [Header("F3 / Cuarto 2 - Keycard")]
    public double experimentalChamberKeycardLeCost = 150000.0;
    public double experimentalChamberKeycardTraceCost = 250.0;
    public int experimentalHallazgos = 0;
    public int experimentalMuestras = 0;
    public int experimentalLecturasIncompletas = 0;
    public int experimentalCompuestosUtiles = 0;
    [Header("F3 / Cuarto 2 - Núcleo de Síntesis")]
    public int synthesisCoreFusionCounter = 0;

    [Header("Zona 4 - Semillas Dimensionales")]
    public double chronalSeedDurationSeconds = 5.0; // prueba temporal
    public List<ChronalSeedSlotState> chronalSeedSlots = new List<ChronalSeedSlotState>();
    public int chronalMatureSeedsStored = 0;
    
    [Header("Zona 4 - Anclajes Inestables")]
    public ChronalInstantState chronalInstant = new ChronalInstantState();
    public int chronalMaterializedInstants = 0;

    public int chronalPureInstants = 0;
    public int chronalStableInstants = 0;
    public int chronalForcedInstants = 0;

    [Header("Zona 4 - Archivo de Anclajes")]
    public int chronalArchivedInstants = 0;

    public string lastChronalMaterializationQuality = "";

    [Header("F3 / Cuarto 2 - Registro experimental")]
    public List<ExperimentalMixLogEntry> experimentalMixLog = new List<ExperimentalMixLogEntry>();

    [Header("F3 / Cuarto 2 - Síntesis Guiada")]
    public int guidedSynthesisIntent = 0;

    [Header("Prestigio 1 - Convergencia")]

    [Tooltip("Cantidad de veces que el jugador ha realizado Prestigio 1.")]
    public int prestige1Count = 0;

    [Tooltip("Indica si el jugador ya realizó Prestigio 1 al menos una vez.")]
    public bool hasDonePrestige1 = false;

    [Tooltip("Puntos disponibles para comprar nodos del Árbol Dimensional D1.")]
    public int prestige1Points = 0;

    [Tooltip("Mayor cantidad de Puntos de Prestigio 1 ya reclamada desde el preview.")]
    public int prestige1BestClaimedPreviewPoints = 0;

    // Máximo de LE alcanzado en el run actual.
    // Se conserva porque todavía sirve como estadística interna del run.
    public double maxLEAlcanzado = 0.0;

    [Header("Sistema de Dimensiones")]

    [Tooltip("Indica si la Dimensión 1 está desbloqueada después de Prestigio 1.")]
    public bool dimension01Unlocked = false;

    [Tooltip("Indica si la Dimensión 2 está preparada después de Prestigio 1. No tiene UI activa todavía.")]
    public bool dimension02Unlocked = false;

    [Tooltip("Indica si la Dimensión 3 está preparada después de Prestigio 1. No tiene UI activa todavía.")]
    public bool dimension03Unlocked = false;

    [Tooltip("Metales acumulados de Dimensión 1.")]
    public List<D1MetalAmount> dimension1Metals = new List<D1MetalAmount>();

    [Tooltip("Estado de planetas/extractores de Dimensión 1.")]
    public List<D1PlanetState> dimension1Planets = new List<D1PlanetState>();

    [Tooltip("Estado de los sectores de Dimensión 1.")]
    public List<D1SectorState> dimension1Sectors = new List<D1SectorState>();

    [Tooltip("Sector de Dimensión 1 en cuyo contexto se encuentra el jugador.")]
    public string dimension1SelectedSectorId = "";

    [Tooltip("Sector donde se inició el barrido de escáner activo.")]
    public string dimension1ActiveScanSectorId = "";

    [Tooltip("Estado de naves de Dimensión 1.")]
    public List<D1ShipState> dimension1Ships = new List<D1ShipState>();

    [Tooltip("Destinos detectados por el escáner de Dimensión 1.")]
    public List<D1ScannedDestinationState> dimension1ScannedDestinations = new List<D1ScannedDestinationState>();

    [Tooltip("IDs del escaneo anterior conservados solo mientras hay un nuevo escaneo activo.")]
    public List<string> dimension1PreviousScannedDestinationIds = new List<string>();

    [Tooltip("Indica si el escáner de Dimensión 1 está haciendo un barrido.")]

    public bool dimension1ScanActive;

    [Tooltip("Tiempo restante del barrido de escáner de Dimensión 1.")]
    public double dimension1ScanRemainingSeconds;

    [Tooltip("Tiempo total del barrido de escáner de Dimensión 1.")]
    public double dimension1ScanTotalSeconds;

    [Tooltip("Nivel del escáner de Dimensión 1. 0 = básico, 1-3 = mejoras.")]
    public int dimension1ScannerLevel = 0;

    [Tooltip("Último destino completado por exploración en Dimensión 1.")]
    public string dimension1LastExplorationDestinationId = "";

    [Tooltip("Últimas recompensas obtenidas por exploración en Dimensión 1.")]
    public List<D1MetalAmount> dimension1LastExplorationRewards = new List<D1MetalAmount>();

    [Tooltip("Registro reciente de exploraciones completadas en Dimensión 1.")]
    public List<D1ExplorationRecordEntry> dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>();

    [Tooltip("Blueprints específicos acumulados en Dimensión 1.")]
    public List<D1BlueprintAmount> dimension1Blueprints = new List<D1BlueprintAmount>();

    [Tooltip("Reliquias desbloqueadas y niveles de Dimensión 1.")]
    public List<D1RelicState> dimension1Relics = new List<D1RelicState>();

    [Tooltip("Nodos comprados del Árbol Dimensional D1.")]
    public List<D1TreeNodeState> dimension1TreeNodes = new List<D1TreeNodeState>();

    [Tooltip("Fragmentos de blueprint acumulados en Dimensión 1.")]
    public int dimension1BlueprintFragments = 0;

    [Tooltip("Fragmentos de blueprint obtenidos en la última exploración.")]
    public int dimension1LastExplorationBlueprintFragments = 0;

    [Tooltip("Blueprints específicos obtenidos en la última exploración.")]
    public List<D1BlueprintAmount> dimension1LastExplorationSpecificBlueprints = new List<D1BlueprintAmount>();

    [Tooltip("Reliquias obtenidas en la última exploración.")]
    public List<D1RelicRewardEntry> dimension1LastExplorationRelics = new List<D1RelicRewardEntry>();

    [Tooltip("Contador interno para detectar nuevas exploraciones completadas en la UI.")]
    public int dimension1LastExplorationResultId = 0;

    [Header("Recursos avanzados (placeholder)")]
    [Tooltip("Recurso para el futuro sistema de BEC (aún sin implementar).")]
    public double BEC = 0.0;  // condensado de Bose-Einstein (futuro)

    // F7: Recursos late-game (por run)
    [Header("Recursos late-game (F7)")]
    [Tooltip("Recurso late-game generado en runs avanzadas. Se resetea con cualquier prestigio.")]
    public double ADP = 0.0;

    [Tooltip("Fragmentos de agujero de gusano (WHF). Recurso muy raro por run, se resetea con cualquier prestigio.")]
    public double WHF = 0.0;

    // F7: Prestigio 2 (Lambda)
    [Header("Prestigio 2 (Lambda)")]
    [Tooltip("Moneda de meta-prestigio (Prestigio 2). No se pierde al resetear runs ni en Prestigio 1.")]
    public double Lambda = 0.0;

        // F7.5: Meta-upgrades comprados con Λ
    [Header("Meta-upgrades (Prestigio 2)")]

    [Tooltip("Upgrade de Lambda: +15% EM base permanente.")]
    public bool metaEmBoost1Bought = false;

    [Tooltip("Total de ADP generada a lo largo de todas las runs (placeholder F7).")]
    public double totalADPGenerada = 0.0;

    [Tooltip("Total de fragmentos de Wormhole generados a lo largo de todas las runs (placeholder F7).")]
    public double totalWHFGenerada = 0.0;

    [Header("Recurso EM (mid-game)")]
    [Tooltip("Campo electromagnético acumulado. Se usará como multiplicador global de LE/s.")]
    public double EM = 0.0;

    [Tooltip("Multiplicador adicional global de LE/s generado por el sistema EM.")]
    public double emMult = 0.0;

    [Tooltip("Multiplicador global de LE/s proveniente de investigaciones.")]
    public double researchGlobalLEMult = 1.0;   // se recalcula desde ResearchManager


    [Header("Producción base (sin edificios)")]

    [Header("Modulador de Fase")]
    [Tooltip("Fase seleccionada actualmente por el Modulador.")]
    public PhaseModulatorMode phaseModulatorMode = PhaseModulatorMode.None;

    [Tooltip("Calibración actual de la fase seleccionada (0 a 1).")]
    [Range(0f, 1f)]
    public float phaseModulatorCalibration = 0f;

    [Tooltip("Velocidad base de calibración por segundo.")]
    public float phaseModulatorCalibrationPerSecond = 0.05f;

    [Tooltip("Bonus máximo de velocidad de tick para la fase Expansión (0.15 = 15%).")]
    [Range(0f, 1f)]
    public float phaseModulatorExpansionMaxTickSpeedBonus = 0.70f;

    [Tooltip("Descuento máximo de costo para la fase Conservación (0.15 = 15%).")]
    [Range(0f, 0.5f)]
    public float phaseModulatorConservationMaxCostReduction = 0.15f;

    [Header("Sistema triangular - Cuarto 1")]

    [Tooltip("Se activa cuando el jugador compra el desbloqueo Acople de Vértices.")]
    public bool triangleSystemUnlocked = false;

    [Tooltip("Artefacto asignado al slot Principal.")]
    public string trianglePrimaryBuildingId = "";

    [Tooltip("Artefacto asignado al slot Refuerzo.")]
    public string triangleReinforcementBuildingId = "";

    [Tooltip("Artefacto asignado al slot Alteración.")]
    public string triangleAlterationBuildingId = "";

    [Header("Triángulo - Persistencia")]

    public float trianglePersistenceActiveUsageFactor = 0.70f;
    public double trianglePersistenceReserveSeconds = 0.0;
    public double trianglePersistenceReserveBaseMaxSeconds = 10800.0; // 3 horas
    public double trianglePersistenceOfflineSecondsPerReserveHour = 14400.0; // 4h offline = 1h reserva
    public double trianglePersistenceBuffHiggsMultiplier = 1.15;
    public double trianglePersistenceBuffTetraMultiplier = 1.15;

    public double baseLEps = 0.0;
  
    // Lista de edificios que producen LE (se llena desde la UI / BuildingList)
    private List<BuildingState> buildingStates = new List<BuildingState>();

    // Exporta los niveles de edificios para el sistema de guardado
    public List<SavedBuildingLevel> GetBuildingLevelsForSave()
    {
        var list = new List<SavedBuildingLevel>();

        if (buildingStates == null) return list;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue; // solo guardamos los que tengan nivel

            list.Add(new SavedBuildingLevel
            {
                id = b.def.id,
                level = b.level
            });
        }

        return list;
    }

    // Aplica niveles cargados desde el save
    public void ApplyBuildingLevelsFromSave(List<SavedBuildingLevel> saved)
    {
        if (saved == null || buildingStates == null) return;

        foreach (var sb in saved)
        {
            if (string.IsNullOrEmpty(sb.id)) continue;

            foreach (var b in buildingStates)
            {
                if (b == null || b.def == null) continue;
                if (b.def.id == sb.id)
                {
                    b.level = sb.level;

                    // Recalcular el coste correcto de la siguiente compra
                    b.currentCost = b.def.baseCost;
                    for (int i = 0; i < b.level; i++)
                    {
                        b.currentCost *= b.def.costMult;
                    }

                    break;
                }
            }
        }

            // Si más adelante tienes un recalculo específico, lo puedes llamar aquí.
            // De momento, CalculateTotalLEps() ya usa buildingStates.
    }

        public int GetFragmentCount(ExperimentalFragmentType type)
    {
        switch (type)
        {
            case ExperimentalFragmentType.Condensation:
                return fragmentCondensation;

            case ExperimentalFragmentType.Confinement:
                return fragmentConfinement;

            case ExperimentalFragmentType.ResidualInterference:
                return fragmentResidualInterference;

            default:
                return 0;
        }
    }

    public void AddFragment(ExperimentalFragmentType type, int amount)
    {
        if (amount <= 0) return;

        switch (type)
        {
            case ExperimentalFragmentType.Condensation:
                fragmentCondensation += amount;
                break;

            case ExperimentalFragmentType.Confinement:
                fragmentConfinement += amount;
                break;

            case ExperimentalFragmentType.ResidualInterference:
                fragmentResidualInterference += amount;
                break;
        }
    }

    public bool ConsumeFragment(ExperimentalFragmentType type, int amount)
    {
        if (amount <= 0) return true;

        int current = GetFragmentCount(type);
        if (current < amount) return false;

        switch (type)
        {
            case ExperimentalFragmentType.Condensation:
                fragmentCondensation -= amount;
                break;

            case ExperimentalFragmentType.Confinement:
                fragmentConfinement -= amount;
                break;

            case ExperimentalFragmentType.ResidualInterference:
                fragmentResidualInterference -= amount;
                break;
        }

        return true;
    }

    public void AddExperimentalResult(ExperimentalResultType resultType, int amount = 1)
    {
        if (amount <= 0) return;

        switch (resultType)
        {
            case ExperimentalResultType.Hallazgo:
                experimentalHallazgos += amount;
                break;

            case ExperimentalResultType.Muestra:
                experimentalMuestras += amount;
                break;

            case ExperimentalResultType.LecturaIncompleta:
                experimentalLecturasIncompletas += amount;
                break;

            case ExperimentalResultType.CompuestoUtil:
                experimentalCompuestosUtiles += amount;
                break;
        }
    }

    public string BuildExperimentalMixKey(
        ExperimentalFragmentType fragmentA,
        ExperimentalFragmentType fragmentB,
        ExperimentalCatalystType catalyst)
    {
        int a = (int)fragmentA;
        int b = (int)fragmentB;

        if (a > b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        return $"mix_{a}_{b}_cat_{(int)catalyst}";
    }

    public ExperimentalMixLogEntry GetOrCreateExperimentalMixEntry(
        ExperimentalFragmentType fragmentA,
        ExperimentalFragmentType fragmentB,
        ExperimentalCatalystType catalyst)
    {
        string key = BuildExperimentalMixKey(fragmentA, fragmentB, catalyst);

        if (experimentalMixLog == null)
            experimentalMixLog = new List<ExperimentalMixLogEntry>();

        foreach (var entry in experimentalMixLog)
        {
            if (entry != null && entry.mixKey == key)
                return entry;
        }

        var newEntry = new ExperimentalMixLogEntry
        {
            mixKey = key,
            discoveredName = "",
            fragmentA = (int)fragmentA,
            fragmentB = (int)fragmentB,
            catalyst = (int)catalyst,
            lastResult = (int)ExperimentalResultType.None,
            bestResult = (int)ExperimentalResultType.None,
            mixState = (int)ExperimentalMixState.NoRegistrada,
            timesExecuted = 0,
            accumulatedErrors = 0,
            discovered = false
        };

        experimentalMixLog.Add(newEntry);
        return newEntry;
    }
    public int GetChronalSeedSlotCount()
    {
        int slots = 2;

        if (MachineManager.I != null)
        {
            double slotBonus = MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.SeedSlotBonus);
            slots += Mathf.FloorToInt((float)slotBonus);
        }

        return Mathf.Clamp(slots, 2, 4);
    }

    public void EnsureChronalSeedSlots()
    {
        if (chronalSeedSlots == null)
            chronalSeedSlots = new List<ChronalSeedSlotState>();

        int targetCount = GetChronalSeedSlotCount();

        while (chronalSeedSlots.Count < targetCount)
        {
            chronalSeedSlots.Add(new ChronalSeedSlotState());
        }

        while (chronalSeedSlots.Count > targetCount)
        {
            chronalSeedSlots.RemoveAt(chronalSeedSlots.Count - 1);
        }
    }

    public bool TryCreateChronalSeed()
    {
        EnsureChronalSeedSlots();

        for (int i = 0; i < chronalSeedSlots.Count; i++)
        {
            ChronalSeedSlotState slot = chronalSeedSlots[i];

            if (slot == null)
            {
                slot = new ChronalSeedSlotState();
                chronalSeedSlots[i] = slot;
            }

            if (slot.hasSeed)
                continue;

            slot.hasSeed = true;
            slot.progressSeconds = 0.0;
            slot.mature = false;

            return true;
        }

        return false;
    }

    public void UpdateChronalSeeds(double dt)
    {
        EnsureChronalSeedSlots();

        for (int i = 0; i < chronalSeedSlots.Count; i++)
        {
            ChronalSeedSlotState slot = chronalSeedSlots[i];

            if (slot == null || !slot.hasSeed)
                continue;

            slot.progressSeconds += dt;

            if (slot.progressSeconds >= chronalSeedDurationSeconds)
            {
                chronalMatureSeedsStored += 1;

                slot.hasSeed = false;
                slot.progressSeconds = 0.0;
                slot.mature = false;
            }
        }
    }

    public int GetChronalSeedReadingLevel()
    {
        if (MachineManager.I == null)
            return 0;

        double value = MachineManager.I.GetTotalEffectValue(
            MachineNodeEffectType.SeedReadingBonus
        );

        return Mathf.Clamp(Mathf.FloorToInt((float)value), 0, 2);
    }

    public string GetChronalSeedsStatusText()
    {
        EnsureChronalSeedSlots();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        int readingLevel = GetChronalSeedReadingLevel();

        sb.AppendLine("Reserva de Semillas Dimensionales Maduras: " + chronalMatureSeedsStored);
        sb.AppendLine("Slots de incubación: " + chronalSeedSlots.Count);

        if (readingLevel <= 0)
            sb.AppendLine("Lectura de maduración: No calibrada");
        else if (readingLevel == 1)
            sb.AppendLine("Lectura de maduración: Porcentaje visible");
        else
            sb.AppendLine("Lectura de maduración: Tiempo restante visible");

        sb.AppendLine();

        for (int i = 0; i < chronalSeedSlots.Count; i++)
        {
            ChronalSeedSlotState slot = chronalSeedSlots[i];

            if (slot == null || !slot.hasSeed)
            {
                sb.AppendLine("Slot " + (i + 1) + ": Vacío");
                continue;
            }

            if (slot.mature)
            {
                sb.AppendLine("Slot " + (i + 1) + ": Lista para mover a reserva");
                continue;
            }

            double progress = 0.0;

            if (chronalSeedDurationSeconds > 0.0)
                progress = slot.progressSeconds / chronalSeedDurationSeconds;

            progress = Mathf.Clamp01((float)progress);

            int percent = Mathf.FloorToInt((float)(progress * 100.0));
            double remaining = System.Math.Max(0.0, chronalSeedDurationSeconds - slot.progressSeconds);

            if (readingLevel <= 0)
            {
                sb.AppendLine("Slot " + (i + 1) + ": Madurando");
            }
            else if (readingLevel == 1)
            {
                sb.AppendLine("Slot " + (i + 1) + ": Madurando " + percent + "%");
            }
            else
            {
                sb.AppendLine("Slot " + (i + 1) + ": Madurando " + percent + "%");
                sb.AppendLine("Tiempo restante: " + remaining.ToString("0.0") + "s");
            }
        }

        return sb.ToString();
    }

    public double GetChronalInstantInitialStability()
    {
        double stability = 30.0;

        if (MachineManager.I != null)
        {
            double bonus = MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.InstantInitialStabilityBonus);
            stability += bonus * 100.0;
        }

        return System.Math.Min(100.0, stability);
    }

    public bool TryFormChronalInstant()
    {
        if (chronalMatureSeedsStored <= 0)
            return false;

        if (chronalInstant == null)
            chronalInstant = new ChronalInstantState();

        if (chronalInstant.hasInstant)
            return false;

        chronalMatureSeedsStored -= 1;

        chronalInstant.hasInstant = true;
        chronalInstant.stability = GetChronalInstantInitialStability();
        chronalInstant.tension = 0.0;

        return true;
    }

    public string GetChronalInstantStatusText()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine("Archivo de Anclajes: " + GetChronalArchiveUsed() + " / " + GetChronalArchiveCapacity());
        sb.AppendLine("Anclajes fijados: " + chronalMaterializedInstants);
        sb.AppendLine("Anclajes disponibles:");
        sb.AppendLine("- Puros: " + chronalPureInstants);
        sb.AppendLine("- Estables: " + chronalStableInstants);
        sb.AppendLine("- Forzados: " + chronalForcedInstants);
        sb.AppendLine();

        if (chronalInstant == null || !chronalInstant.hasInstant)
        {
            sb.AppendLine("Anclaje Inestable: ninguno");
            return sb.ToString();
        }   

        sb.AppendLine("Ancaje Inestable");
        sb.AppendLine("Estabilidad: " + chronalInstant.stability.ToString("0") + "%");
        sb.AppendLine("Tensión: " + chronalInstant.tension.ToString("0") + "%");
        sb.AppendLine("Umbral de fijación: " + GetChronalMaterializationThreshold().ToString("0") + "%");
        sb.AppendLine(GetChronalRewindPreviewText());

        return sb.ToString();
    }

    public int NormalizeChronalStabilizationIntensity(double intensityPercent)
    {
        int normalized = Mathf.RoundToInt((float)(intensityPercent / 10.0)) * 10;
        return Mathf.Clamp(normalized, 10, 100);
    }

    public double GetChronalStabilityGainForIntensity(double intensityPercent)
    {
        int intensity = NormalizeChronalStabilizationIntensity(intensityPercent);
        double baseStabilityGain = intensity * 0.2;

        double syncBonus = 0.0;

        if (MachineManager.I != null)
        {
            syncBonus = MachineManager.I.GetTotalEffectValue(
                MachineNodeEffectType.SynchronizeStabilityBonus
            );
        }

        return baseStabilityGain + syncBonus;
    }

    public double GetChronalTensionGainForIntensity(double intensityPercent)
    {
        int intensity = NormalizeChronalStabilizationIntensity(intensityPercent);

        double baseTensionGain;

        switch (intensity)
        {
            case 10: baseTensionGain = 0.0; break;
            case 20: baseTensionGain = 1.0; break;
            case 30: baseTensionGain = 2.0; break;
            case 40: baseTensionGain = 3.0; break;
            case 50: baseTensionGain = 5.0; break;
            case 60: baseTensionGain = 7.0; break;
            case 70: baseTensionGain = 9.0; break;
            case 80: baseTensionGain = 12.0; break;
            case 90: baseTensionGain = 15.0; break;
            case 100: baseTensionGain = 18.0; break;
            default: baseTensionGain = 5.0; break;
        }

        double containmentReduction = 0.0;

        if (MachineManager.I != null)
        {
            containmentReduction = MachineManager.I.GetTotalEffectValue(
                MachineNodeEffectType.TensionContainmentBonus
            );
        }

        return System.Math.Max(0.0, baseTensionGain - containmentReduction);
    }

    public string GetChronalStabilizationPreviewText(double intensityPercent)
    {
        int intensity = NormalizeChronalStabilizationIntensity(intensityPercent);
        double stabilityGain = GetChronalStabilityGainForIntensity(intensity);
        double tensionGain = GetChronalTensionGainForIntensity(intensity);

        return
            "Intensidad de estabilización: " + intensity + "%\n" +
            "Efecto previsto: +" + stabilityGain.ToString("0") +
            "% estabilidad / +" + tensionGain.ToString("0") +
            "% tensión";
    }

    public bool TryStabilizeChronalInstant(double intensityPercent = 50.0)
    {
        if (chronalInstant == null || !chronalInstant.hasInstant)
            return false;

        double stabilityGain = GetChronalStabilityGainForIntensity(intensityPercent);
        double tensionGain = GetChronalTensionGainForIntensity(intensityPercent);

        chronalInstant.stability = System.Math.Min(100.0, chronalInstant.stability + stabilityGain);
        chronalInstant.tension = System.Math.Min(100.0, chronalInstant.tension + tensionGain);

        return true;
    }

    public int GetChronalArchiveUsed()
    {
        return Mathf.Max(
            0,
            chronalPureInstants + chronalStableInstants + chronalForcedInstants
        );
    }

    public int GetChronalArchiveCapacity()
    {
        int capacity = 30;

        if (MachineManager.I != null)
        {
            double bonus = MachineManager.I.GetTotalEffectValue(
                MachineNodeEffectType.ArchiveSlotBonus
            );

            capacity += Mathf.FloorToInt((float)bonus);
        }

        return Mathf.Max(0, capacity);
    }

    public bool IsChronalArchiveFull()
    {
        return GetChronalArchiveUsed() >= GetChronalArchiveCapacity();
    }

    public double GetChronalMaterializationThreshold()
    {
        double threshold = 70.0;

        if (MachineManager.I != null)
        {
            double reduction = MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.PureMaterialThresholdReduction);
            threshold -= reduction * 100.0;
        }

        return System.Math.Max(50.0, threshold);
    }

    public string GetChronalMaterializationQualityName(double tension)
    {
        if (tension <= 15.0)
            return "Anclaje Puro";

        if (tension <= 30.0)
            return "Anclaje Estable";

        return "Anclaje Forzado";
    }

    public bool TryMaterializeChronalInstant()
    {
        
        if (chronalInstant == null || !chronalInstant.hasInstant)
            return false;

        if (IsChronalArchiveFull())
            return false;

        double threshold = GetChronalMaterializationThreshold();

        if (chronalInstant.stability < threshold)
            return false;

        string qualityName = GetChronalMaterializationQualityName(chronalInstant.tension);
        lastChronalMaterializationQuality = qualityName;

        chronalMaterializedInstants += 1;
        chronalArchivedInstants += 1;

        if (qualityName == "Anclaje Puro")
            chronalPureInstants += 1;
        else if (qualityName == "Anclaje Estable")
            chronalStableInstants += 1;
        else
            chronalForcedInstants += 1;

        chronalInstant.hasInstant = false;
        chronalInstant.stability = 0.0;
        chronalInstant.tension = 0.0;

        return true;
    }

    public double GetChronalRewindTensionReduction()
    {
        return 10.0;
    }

    public double GetChronalRewindStabilityLoss()
    {
        double baseLoss = 5.0;
        double safeRewindBonus = 0.0;

        if (MachineManager.I != null)
        {
            safeRewindBonus = MachineManager.I.GetTotalEffectValue(
                MachineNodeEffectType.SafeRewindBonus
            );
        }

        return System.Math.Max(1.0, baseLoss - safeRewindBonus);
    }

    public string GetChronalRewindPreviewText()
    {
        double tensionReduction = GetChronalRewindTensionReduction();
        double stabilityLoss = GetChronalRewindStabilityLoss();

        return
            "Compensar previsto: -" + tensionReduction.ToString("0") +
            "% tensión / -" + stabilityLoss.ToString("0") +
            "% estabilidad";
    }

    public bool TryRewindChronalInstant()
    {
        if (chronalInstant == null || !chronalInstant.hasInstant)
            return false;

        double tensionReduction = GetChronalRewindTensionReduction();
        double stabilityLoss = GetChronalRewindStabilityLoss();

        chronalInstant.tension = System.Math.Max(0.0, chronalInstant.tension - tensionReduction);
        chronalInstant.stability = System.Math.Max(0.0, chronalInstant.stability - stabilityLoss);

        return true;
    }

    public bool TryDiscardChronalInstant()
    {
        if (chronalInstant == null || !chronalInstant.hasInstant)
            return false;

        chronalInstant.hasInstant = false;
        chronalInstant.stability = 0.0;
        chronalInstant.tension = 0.0;

        return true;
    }

    public void GenerateExperimentalFragments(double dt)
    {
        if (!experimentalChamberUnlocked)
            return;

        if (buildingStates == null || buildingStates.Count == 0)
            return;

        bool hasHiggs = false;
        bool hasTetra = false;
        bool hasModulator = false;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue;

            switch (b.def.id)
            {
                case "vacuum_observer":
                    hasHiggs = true;
                    break;

                case "casimir_panel":
                    hasTetra = true;
                    break;

                case "fluctuation_antenna":
                    hasModulator = true;
                    break;
            }
        }

        // Cadencia base provisional:
        // 1 fragmento cada 30 segundos por artefacto activo
        double gainPerSecond = 1.0 / 30.0;

        if (hasHiggs)
        {
            fragmentCondensationProgress += gainPerSecond * dt;

            while (fragmentCondensationProgress >= 1.0)
            {
                fragmentCondensation += 1;
                fragmentCondensationProgress -= 1.0;
                Debug.Log("F3 DEBUG: +1 Fragmento de Condensación");
            }
        }

        if (hasTetra)
        {
            fragmentConfinementProgress += gainPerSecond * dt;

           while (fragmentConfinementProgress >= 1.0)
            {
                fragmentConfinement += 1;
                fragmentConfinementProgress -= 1.0;
                Debug.Log("F3 DEBUG: +1 Fragmento de Confinamiento");
            }
        }

        if (hasModulator)
        {
            fragmentResidualInterferenceProgress += gainPerSecond * dt;

            while (fragmentResidualInterferenceProgress >= 1.0)
            {
                fragmentResidualInterference += 1;
                fragmentResidualInterferenceProgress -= 1.0;
                Debug.Log("F3 DEBUG: +1 Interferencia Residual");
            }
        }
    }

        public void UnlockExperimentalChamber()
    {
        experimentalChamberUnlocked = true;

        if (!experimentalChamberInitialPackGranted)
        {
            fragmentCondensation += 5;
            fragmentConfinement += 5;
            fragmentResidualInterference += 5;

            experimentalChamberInitialPackGranted = true;
        }
    }

        public bool HasExperimentalChamberKeycardRequirements()
    {
        if (experimentalChamberUnlocked)
            return false;

        bool hasHiggs = GetBuildingLevel("vacuum_observer") >= 1;
        bool hasTetra = GetBuildingLevel("casimir_panel") >= 1;
        bool hasModulator = GetBuildingLevel("fluctuation_antenna") >= 1;

        return hasHiggs && hasTetra && hasModulator;
    }

    public bool CanBuyExperimentalChamberKeycard()
    {
        if (!HasExperimentalChamberKeycardRequirements())
            return false;

        return LE >= experimentalChamberKeycardLeCost
            && Traces >= experimentalChamberKeycardTraceCost;
    }

    public bool TryBuyExperimentalChamberKeycard()
    {
        if (!CanBuyExperimentalChamberKeycard())
            return false;

        LE -= experimentalChamberKeycardLeCost;
        Traces -= experimentalChamberKeycardTraceCost;

        UnlockExperimentalChamber();
        return true;
    }

    [ContextMenu("DEBUG: Buy Experimental Chamber Keycard")]
    public void DebugBuyExperimentalChamberKeycard()
    {
        bool ok = TryBuyExperimentalChamberKeycard();
        Debug.Log($"[F3] DEBUG Buy Keycard => {ok} | LE={LE:0.##} | Traces={Traces:0.##} | Unlocked={experimentalChamberUnlocked}");
    }

    [ContextMenu("DEBUG: Force Unlock Experimental Chamber")]
    public void DebugUnlockExperimentalChamber()
    {
        UnlockExperimentalChamber();
        Debug.Log("[F3] DEBUG Force Unlock Experimental Chamber");
    }


    [Header("Decoherencia (soft cap) - DESACTIVADA POR AHORA")]
    [Tooltip("Por ahora no afecta la producción. Más adelante se reutilizará.")]
    public bool useDecoherence = false;   // << clave: queda en false

    [Tooltip("A partir de esta cantidad de LE almacenada empezaría la decoherencia (futuro).")]
    public double decoStartLE = 3000.0;

    [Tooltip("Qué tan rápido caería la producción cuando te pases del umbral (futuro).")]
    public double decoStrength = 0.00004;

    [Tooltip("Factor mínimo de producción (0.6 = nunca baja de 60%) (futuro).")]
    public double decoMinFactor = 0.6;

    // Debug: acumulador de tiempo para logs

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
    }

    private void Start()
    {
        // Cuando el GameState ya está creado, pedimos cargar el save
        if (SaveService.I != null)
        {
            SaveService.I.Load();
        }

        EnsureDimension1State();
    }

    public void EnsureDimension1State()
    {
        bool requiresSectorMigration =
            dimension1Sectors == null ||
            dimension1Sectors.Count == 0;

        if (dimension1Metals == null)
            dimension1Metals = new List<D1MetalAmount>();

        if (dimension1Planets == null)
            dimension1Planets = new List<D1PlanetState>();

        if (dimension1Sectors == null)
            dimension1Sectors = new List<D1SectorState>();

        if (dimension1Ships == null)
            dimension1Ships = new List<D1ShipState>();

        if (dimension1ScannedDestinations == null)
            dimension1ScannedDestinations = new List<D1ScannedDestinationState>();

        if (dimension1PreviousScannedDestinationIds == null)
            dimension1PreviousScannedDestinationIds = new List<string>();

        if (dimension1LastExplorationRewards == null)
            dimension1LastExplorationRewards = new List<D1MetalAmount>();

        if (dimension1LastExplorationSpecificBlueprints == null)
            dimension1LastExplorationSpecificBlueprints = new List<D1BlueprintAmount>();

        if (dimension1LastExplorationRelics == null)
            dimension1LastExplorationRelics = new List<D1RelicRewardEntry>();

        if (dimension1RecentExplorationRecords == null)
            dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>();

        if (dimension1Blueprints == null)
            dimension1Blueprints = new List<D1BlueprintAmount>();

        if (dimension1Relics == null)
            dimension1Relics = new List<D1RelicState>();

        if (dimension1TreeNodes == null)
            dimension1TreeNodes = new List<D1TreeNodeState>();

        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
        {
            GetOrCreateD1Sector(sectorId);
        }

        if (requiresSectorMigration)
            MigrateDimension1LegacySectorProgress();

        SanitizeDimension1StateValues();
        MigrateDimension1LegacyShipIds();
        ClampDimension1ShipPartLevels();
        ClampDimension1RelicLevels();

        foreach (string metalId in Dimension1System.StarterMetals)
        {
            GetOrCreateD1Metal(metalId);
        }

        foreach (string planetId in Dimension1System.StarterPlanets)
        {
            GetOrCreateD1Planet(planetId);
        }

        foreach (string shipId in Dimension1System.Dimension1ShipIds)
        {
            GetOrCreateD1Ship(shipId);
        }

        foreach (string blueprintId in Dimension1System.Dimension1BlueprintIds)
        {
            GetOrCreateD1Blueprint(blueprintId);
        }

        foreach (string relicId in Dimension1System.Dimension1RelicIds)
        {
            GetOrCreateD1Relic(relicId);
        }

        foreach (string nodeId in Dimension1System.Dimension1TreeNodeIds)
        {
            GetOrCreateD1TreeNode(nodeId);
        }
    }

    private void SanitizeDimension1StateValues()
    {
        SanitizeDimension1Sectors();

        dimension1ScannerLevel = Mathf.Clamp(
            dimension1ScannerLevel,
            0,
            Dimension1System.SimpleScannerMaxLevel
        );

        dimension1ScanRemainingSeconds = SanitizeD1NonNegative(
            dimension1ScanRemainingSeconds
        );
        dimension1ScanTotalSeconds = SanitizeD1NonNegative(
            dimension1ScanTotalSeconds
        );

        if (dimension1ScanActive)
        {
            if (!Dimension1System.IsDimension1ExplorationSectorId(
                dimension1ActiveScanSectorId
            ))
            {
                dimension1ActiveScanSectorId =
                    Dimension1System.IsDimension1ExplorationSectorId(
                        dimension1SelectedSectorId
                    )
                        ? dimension1SelectedSectorId
                        : Dimension1System.Sector01OuterRim;
            }

            dimension1ScanTotalSeconds = System.Math.Max(
                dimension1ScanTotalSeconds,
                dimension1ScanRemainingSeconds
            );
        }
        else
        {
            dimension1ActiveScanSectorId = "";
            dimension1ScanRemainingSeconds = 0.0;
            dimension1ScanTotalSeconds = 0.0;
        }

        dimension1BlueprintFragments = Mathf.Max(0, dimension1BlueprintFragments);
        dimension1LastExplorationBlueprintFragments = Mathf.Max(
            0,
            dimension1LastExplorationBlueprintFragments
        );
        dimension1LastExplorationResultId = Mathf.Max(
            0,
            dimension1LastExplorationResultId
        );
        prestige1Points = Mathf.Max(0, prestige1Points);
        prestige1BestClaimedPreviewPoints = Mathf.Max(
            0,
            prestige1BestClaimedPreviewPoints
        );
        dimension1LastExplorationDestinationId =
            dimension1LastExplorationDestinationId ?? "";

        SanitizeD1MetalAmounts(dimension1Metals);
        SanitizeD1MetalAmounts(dimension1LastExplorationRewards);
        SanitizeD1BlueprintAmounts(dimension1Blueprints);
        SanitizeD1BlueprintAmounts(dimension1LastExplorationSpecificBlueprints);
        SanitizeD1RelicRewards(dimension1LastExplorationRelics);

        dimension1Planets.RemoveAll(planet => planet == null);

        foreach (D1PlanetState planet in dimension1Planets)
        {
            planet.planetId = planet.planetId ?? "";
            planet.extractorTier = Mathf.Max(0, planet.extractorTier);

            if (planet.extractorTier > 0)
                planet.unlocked = true;
            else if (planet.unlocked)
                planet.extractorTier = 1;
        }

        dimension1ScannedDestinations.RemoveAll(
            destination =>
                destination == null ||
                string.IsNullOrEmpty(destination.destinationId)
        );

        foreach (D1ScannedDestinationState destination in dimension1ScannedDestinations)
        {
            destination.specialPointId = destination.specialPointId ?? "";
            destination.sectorId = Dimension1System.IsDimension1ExplorationSectorId(
                destination.sectorId
            )
                ? destination.sectorId
                : Dimension1System.IsDimension1ExplorationSectorId(
                    dimension1SelectedSectorId
                )
                    ? dimension1SelectedSectorId
                    : Dimension1System.Sector01OuterRim;
        }

        dimension1ScannedDestinations.RemoveAll(
            destination =>
                destination == null ||
                !Dimension1System.IsDestinationInDimension1Sector(
                    destination.destinationId,
                    destination.sectorId
                )
        );

        for (int i = dimension1PreviousScannedDestinationIds.Count - 1; i >= 0; i--)
        {
            string destinationId = dimension1PreviousScannedDestinationIds[i];

            if (string.IsNullOrEmpty(destinationId) ||
                dimension1PreviousScannedDestinationIds.IndexOf(destinationId) != i)
            {
                dimension1PreviousScannedDestinationIds.RemoveAt(i);
            }
        }

        if (!dimension1ScanActive)
            dimension1PreviousScannedDestinationIds.Clear();

        dimension1TreeNodes.RemoveAll(node => node == null);

        foreach (D1TreeNodeState node in dimension1TreeNodes)
        {
            node.nodeId = node.nodeId ?? "";
            node.tier = Mathf.Max(0, node.tier);
        }

        SanitizeD1ExplorationHistory();
    }

    private void SanitizeD1ExplorationHistory()
    {
        dimension1RecentExplorationRecords.RemoveAll(entry => entry == null);

        foreach (D1ExplorationRecordEntry entry in dimension1RecentExplorationRecords)
        {
            entry.resultId = Mathf.Max(0, entry.resultId);
            entry.shipId = entry.shipId ?? "";
            entry.destinationId = entry.destinationId ?? "";
            entry.sectorId = Dimension1System.IsDimension1ExplorationSectorId(
                entry.sectorId
            )
                ? entry.sectorId
                : Dimension1System.Sector01OuterRim;
            entry.blueprintFragments = Mathf.Max(0, entry.blueprintFragments);

            if (entry.rewards == null)
                entry.rewards = new List<D1MetalAmount>();

            if (entry.specificBlueprintRewards == null)
                entry.specificBlueprintRewards = new List<D1BlueprintAmount>();

            if (entry.relicRewards == null)
                entry.relicRewards = new List<D1RelicRewardEntry>();

            SanitizeD1MetalAmounts(entry.rewards);
            SanitizeD1BlueprintAmounts(entry.specificBlueprintRewards);
            SanitizeD1RelicRewards(entry.relicRewards);
        }

        while (dimension1RecentExplorationRecords.Count >
            Dimension1System.Dimension1RecentExplorationHistoryLimit)
        {
            dimension1RecentExplorationRecords.RemoveAt(0);
        }
    }

    private static void SanitizeD1MetalAmounts(List<D1MetalAmount> amounts)
    {
        if (amounts == null)
            return;

        amounts.RemoveAll(amount => amount == null);

        foreach (D1MetalAmount amount in amounts)
        {
            amount.metalId = amount.metalId ?? "";
            amount.amount = SanitizeD1NonNegative(amount.amount);
        }
    }

    private static void SanitizeD1BlueprintAmounts(List<D1BlueprintAmount> amounts)
    {
        if (amounts == null)
            return;

        amounts.RemoveAll(amount => amount == null);

        foreach (D1BlueprintAmount amount in amounts)
        {
            amount.blueprintId = amount.blueprintId ?? "";
            amount.amount = Mathf.Max(0, amount.amount);
        }
    }

    private static void SanitizeD1RelicRewards(List<D1RelicRewardEntry> rewards)
    {
        if (rewards == null)
            return;

        rewards.RemoveAll(reward => reward == null);

        foreach (D1RelicRewardEntry reward in rewards)
        {
            reward.relicId = reward.relicId ?? "";
            reward.duplicateMetalId = reward.duplicateMetalId ?? "";
            reward.duplicateMetalAmount = SanitizeD1NonNegative(
                reward.duplicateMetalAmount
            );
        }
    }

    private static double SanitizeD1NonNegative(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0.0)
            return 0.0;

        return value;
    }

    private void SanitizeDimension1Sectors()
    {
        if (dimension1Sectors == null)
            dimension1Sectors = new List<D1SectorState>();

        var normalized = new Dictionary<string, D1SectorState>();

        foreach (D1SectorState sector in dimension1Sectors)
        {
            if (sector == null ||
                !Dimension1System.IsDimension1SectorId(sector.sectorId))
            {
                continue;
            }

            if (!normalized.TryGetValue(sector.sectorId, out D1SectorState current))
            {
                normalized.Add(
                    sector.sectorId,
                    new D1SectorState
                    {
                        sectorId = sector.sectorId,
                        unlocked = sector.unlocked,
                        completedExplorations = Mathf.Max(
                            0,
                            sector.completedExplorations
                        )
                    }
                );
                continue;
            }

            current.unlocked = current.unlocked || sector.unlocked;
            current.completedExplorations = Mathf.Max(
                current.completedExplorations,
                sector.completedExplorations
            );
        }

        dimension1Sectors.Clear();

        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
        {
            if (!normalized.TryGetValue(sectorId, out D1SectorState sector))
            {
                sector = new D1SectorState
                {
                    sectorId = sectorId,
                    unlocked = false,
                    completedExplorations = 0
                };
            }

            dimension1Sectors.Add(sector);
        }

        D1SectorState firstSector = FindD1SectorState(
            Dimension1System.Sector01OuterRim
        );

        if (firstSector != null)
            firstSector.unlocked = true;

        if (!Dimension1System.IsDimension1SectorId(dimension1SelectedSectorId))
            dimension1SelectedSectorId = Dimension1System.Sector01OuterRim;

        D1SectorState selectedSector = FindD1SectorState(
            dimension1SelectedSectorId
        );

        if (selectedSector == null || !selectedSector.unlocked)
            dimension1SelectedSectorId = Dimension1System.Sector01OuterRim;
    }

    private void MigrateDimension1LegacySectorProgress()
    {
        D1SectorState firstSector = GetOrCreateD1Sector(
            Dimension1System.Sector01OuterRim
        );

        firstSector.unlocked = true;
        firstSector.completedExplorations = Mathf.Max(
            firstSector.completedExplorations,
            Mathf.Max(0, dimension1LastExplorationResultId)
        );

        bool reachedSector2 = IsD1PlanetUnlockedForSectorMigration(
            Dimension1System.Planet03
        );
        bool reachedSector3 =
            IsD1PlanetUnlockedForSectorMigration(Dimension1System.Planet04) ||
            IsD1PlanetUnlockedForSectorMigration(Dimension1System.Planet05);
        bool reachedSector4 =
            IsD1PlanetUnlockedForSectorMigration(Dimension1System.Planet06) ||
            IsD1PlanetUnlockedForSectorMigration(Dimension1System.Planet07);

        if (reachedSector2 || reachedSector3 || reachedSector4)
        {
            GetOrCreateD1Sector(
                Dimension1System.Sector02DebrisRing
            ).unlocked = true;
        }

        if (reachedSector3 || reachedSector4)
        {
            GetOrCreateD1Sector(
                Dimension1System.Sector03AncientOrbits
            ).unlocked = true;
        }

        if (reachedSector4)
        {
            GetOrCreateD1Sector(
                Dimension1System.Sector04SilentFrontier
            ).unlocked = true;
        }

        dimension1SelectedSectorId = Dimension1System.Sector01OuterRim;

        if (dimension1ScanActive)
        {
            dimension1ActiveScanSectorId =
                Dimension1System.Sector01OuterRim;
        }
    }

    private bool IsD1PlanetUnlockedForSectorMigration(string planetId)
    {
        if (dimension1Planets == null || string.IsNullOrEmpty(planetId))
            return false;

        foreach (D1PlanetState planet in dimension1Planets)
        {
            if (planet != null &&
                planet.planetId == planetId &&
                planet.unlocked)
            {
                return true;
            }
        }

        return false;
    }

    private void MigrateDimension1LegacyShipIds()
    {
        if (dimension1Ships == null)
            return;

        D1ShipState legacyCargoShip = null;
        D1ShipState cargoShip = null;

        foreach (D1ShipState ship in dimension1Ships)
        {
            if (ship == null)
                continue;

            if (ship.shipId == Dimension1System.LegacyCargoShipId)
                legacyCargoShip = ship;

            if (ship.shipId == Dimension1System.ShipCargoShip)
                cargoShip = ship;
        }

        if (legacyCargoShip == null)
            return;

        if (cargoShip == null)
        {
            legacyCargoShip.shipId = Dimension1System.ShipCargoShip;
            return;
        }

        cargoShip.unlocked = cargoShip.unlocked || legacyCargoShip.unlocked;

        if (!cargoShip.explorationActive && legacyCargoShip.explorationActive)
        {
            cargoShip.explorationActive = true;
            cargoShip.activeDestinationId = legacyCargoShip.activeDestinationId;
            cargoShip.activeSpecialPointId = legacyCargoShip.activeSpecialPointId;
            cargoShip.activeSectorId = legacyCargoShip.activeSectorId;
            cargoShip.explorationRemainingSeconds = legacyCargoShip.explorationRemainingSeconds;
            cargoShip.explorationTotalSeconds = legacyCargoShip.explorationTotalSeconds;
        }

        cargoShip.cargoLevel = Mathf.Max(cargoShip.cargoLevel, legacyCargoShip.cargoLevel);
        cargoShip.speedLevel = Mathf.Max(cargoShip.speedLevel, legacyCargoShip.speedLevel);
        cargoShip.armorLevel = Mathf.Max(cargoShip.armorLevel, legacyCargoShip.armorLevel);
        cargoShip.sensorsLevel = Mathf.Max(cargoShip.sensorsLevel, legacyCargoShip.sensorsLevel);

        dimension1Ships.Remove(legacyCargoShip);
    }

    private void ClampDimension1ShipPartLevels()
    {
        if (dimension1Ships == null)
            return;

        dimension1Ships.RemoveAll(ship => ship == null);
        HashSet<string> activeDestinationKeys = new HashSet<string>();

        foreach (D1ShipState ship in dimension1Ships)
        {
            ship.shipId = ship.shipId ?? "";
            ship.activeDestinationId = ship.activeDestinationId ?? "";
            ship.activeSpecialPointId = ship.activeSpecialPointId ?? "";
            ship.activeSectorId = ship.activeSectorId ?? "";

            ship.cargoLevel = Mathf.Max(0, ship.cargoLevel);
            ship.speedLevel = Mathf.Max(0, ship.speedLevel);
            ship.armorLevel = Mathf.Max(0, ship.armorLevel);
            ship.sensorsLevel = Mathf.Max(0, ship.sensorsLevel);

            ship.explorationRemainingSeconds = SanitizeD1NonNegative(
                ship.explorationRemainingSeconds
            );
            ship.explorationTotalSeconds = SanitizeD1NonNegative(
                ship.explorationTotalSeconds
            );

            if (ship.explorationActive && string.IsNullOrEmpty(ship.activeDestinationId))
                ship.explorationActive = false;

            if (ship.explorationActive &&
                !Dimension1System.IsDimension1ExplorationSectorId(
                    ship.activeSectorId
                ))
            {
                ship.activeSectorId =
                    Dimension1System.IsDimension1ExplorationSectorId(
                        dimension1SelectedSectorId
                    )
                        ? dimension1SelectedSectorId
                        : Dimension1System.Sector01OuterRim;
            }

            if (ship.explorationActive &&
                Dimension1System.IsShipActiveInDimension1Base(ship.shipId) &&
                !activeDestinationKeys.Add(
                    ship.activeSectorId + "|" + ship.activeDestinationId
                ))
            {
                ship.explorationActive = false;
            }

            if (!ship.explorationActive)
            {
                ship.activeDestinationId = "";
                ship.activeSpecialPointId = "";
                ship.activeSectorId = "";
                ship.explorationRemainingSeconds = 0.0;
                ship.explorationTotalSeconds = 0.0;
            }
            else
            {
                ship.unlocked = true;
                ship.explorationTotalSeconds = System.Math.Max(
                    ship.explorationTotalSeconds,
                    ship.explorationRemainingSeconds
                );
            }
        }
    }

    private void ClampDimension1RelicLevels()
    {
        if (dimension1Relics == null)
            return;

        dimension1Relics.RemoveAll(relic => relic == null);

        foreach (D1RelicState relic in dimension1Relics)
        {
            relic.relicId = relic.relicId ?? "";
            relic.level = Mathf.Max(0, relic.level);

            if (relic.level > 0)
                relic.unlocked = true;
            else if (relic.unlocked)
                relic.level = 1;
        }
    }

    private D1SectorState FindD1SectorState(string sectorId)
    {
        if (dimension1Sectors == null || string.IsNullOrEmpty(sectorId))
            return null;

        foreach (D1SectorState sector in dimension1Sectors)
        {
            if (sector != null && sector.sectorId == sectorId)
                return sector;
        }

        return null;
    }

    private D1SectorState GetOrCreateD1Sector(string sectorId)
    {
        D1SectorState sector = FindD1SectorState(sectorId);

        if (sector != null)
            return sector;

        sector = new D1SectorState
        {
            sectorId = sectorId,
            unlocked = sectorId == Dimension1System.Sector01OuterRim,
            completedExplorations = 0
        };

        dimension1Sectors.Add(sector);
        return sector;
    }

    public bool IsD1SectorUnlocked(string sectorId)
    {
        if (!Dimension1System.IsDimension1SectorId(sectorId))
            return false;

        EnsureDimension1State();

        D1SectorState sector = FindD1SectorState(sectorId);
        return sector != null && sector.unlocked;
    }

    public int GetD1SectorExplorationCount(string sectorId)
    {
        if (!Dimension1System.IsDimension1SectorId(sectorId))
            return 0;

        EnsureDimension1State();

        D1SectorState sector = FindD1SectorState(sectorId);
        return sector != null ? Mathf.Max(0, sector.completedExplorations) : 0;
    }

    public bool UnlockD1Sector(string sectorId)
    {
        if (!Dimension1System.IsDimension1SectorId(sectorId))
            return false;

        EnsureDimension1State();

        D1SectorState sector = GetOrCreateD1Sector(sectorId);
        sector.unlocked = true;
        return true;
    }

    public bool TrySelectD1Sector(string sectorId)
    {
        if (!Dimension1System.IsDimension1SectorId(sectorId))
            return false;

        EnsureDimension1State();

        D1SectorState sector = FindD1SectorState(sectorId);

        if (sector == null || !sector.unlocked)
            return false;

        if (dimension1ScanActive &&
            dimension1ActiveScanSectorId != sectorId)
        {
            return false;
        }

        if (dimension1SelectedSectorId == sectorId)
            return true;

        dimension1SelectedSectorId = sectorId;
        dimension1ScannedDestinations.Clear();
        dimension1PreviousScannedDestinationIds.Clear();
        return true;
    }

    public bool AddD1SectorExplorationCount(string sectorId, int amount)
    {
        if (!Dimension1System.IsDimension1ExplorationSectorId(sectorId))
            return false;

        if (amount <= 0)
            return false;

        EnsureDimension1State();

        D1SectorState sector = FindD1SectorState(sectorId);

        if (sector == null || !sector.unlocked)
            return false;

        long total = (long)sector.completedExplorations + amount;
        sector.completedExplorations = total > int.MaxValue
            ? int.MaxValue
            : (int)total;

        return true;
    }

    private D1MetalAmount GetOrCreateD1Metal(string metalId)
    {
        foreach (var metal in dimension1Metals)
        {
            if (metal != null && metal.metalId == metalId)
                return metal;
        }

        var newMetal = new D1MetalAmount
        {
            metalId = metalId,
            amount = 0.0
        };

        dimension1Metals.Add(newMetal);
        return newMetal;
    }

    private D1PlanetState GetOrCreateD1Planet(string planetId)
    {
        foreach (var planet in dimension1Planets)
        {
            if (planet != null && planet.planetId == planetId)
                return planet;
        }

        var newPlanet = new D1PlanetState
        {
            planetId = planetId,
            unlocked = false,
            extractorTier = 0
        };

        dimension1Planets.Add(newPlanet);
        return newPlanet;
    }

    private D1ShipState GetOrCreateD1Ship(string shipId)
    {
        foreach (var ship in dimension1Ships)
        {
            if (ship != null && ship.shipId == shipId)
                return ship;
        }

        var newShip = new D1ShipState
        {
            shipId = shipId,
            unlocked = false,
            explorationActive = false,
            activeDestinationId = "",
            activeSpecialPointId = "",
            activeSectorId = "",
            explorationRemainingSeconds = 0.0,
            explorationTotalSeconds = 0.0
        };

        dimension1Ships.Add(newShip);
        return newShip;
    }

    private D1BlueprintAmount GetOrCreateD1Blueprint(string blueprintId)
    {
        foreach (D1BlueprintAmount blueprint in dimension1Blueprints)
        {
            if (blueprint != null && blueprint.blueprintId == blueprintId)
                return blueprint;
        }

        D1BlueprintAmount newBlueprint = new D1BlueprintAmount
        {
            blueprintId = blueprintId,
            amount = 0
        };

        dimension1Blueprints.Add(newBlueprint);
        return newBlueprint;
    }

    private D1RelicState GetOrCreateD1Relic(string relicId)
    {
        foreach (D1RelicState relic in dimension1Relics)
        {
            if (relic != null && relic.relicId == relicId)
                return relic;
        }

        D1RelicState newRelic = new D1RelicState
        {
            relicId = relicId,
            unlocked = false,
            level = 0
        };

        dimension1Relics.Add(newRelic);
        return newRelic;
    }

    private D1TreeNodeState GetOrCreateD1TreeNode(string nodeId)
    {
        foreach (D1TreeNodeState node in dimension1TreeNodes)
        {
            if (node != null && node.nodeId == nodeId)
                return node;
        }

        D1TreeNodeState newNode = new D1TreeNodeState
        {
            nodeId = nodeId,
            tier = 0
        };

        dimension1TreeNodes.Add(newNode);
        return newNode;
    }

    public double GetD1MetalAmount(string metalId)
    {
        EnsureDimension1State();
        return GetOrCreateD1Metal(metalId).amount;
    }

    public void AddD1Metal(string metalId, double amount)
    {
        if (amount <= 0.0)
            return;

        EnsureDimension1State();
        GetOrCreateD1Metal(metalId).amount += amount;
    }

    public bool SpendD1Metal(string metalId, double amount)
    {
        if (amount <= 0.0)
            return true;

        EnsureDimension1State();

        D1MetalAmount metal = GetOrCreateD1Metal(metalId);

        if (metal.amount < amount)
            return false;

        metal.amount -= amount;
        return true;
    }

    public int GetD1BlueprintAmount(string blueprintId)
    {
        if (string.IsNullOrEmpty(blueprintId))
            return 0;

        EnsureDimension1State();
        return GetOrCreateD1Blueprint(blueprintId).amount;
    }

    public void AddD1Blueprint(string blueprintId, int amount)
    {
        if (string.IsNullOrEmpty(blueprintId))
            return;

        if (amount <= 0)
            return;

        if (!Dimension1System.IsDimension1BlueprintId(blueprintId))
            return;

        EnsureDimension1State();
        GetOrCreateD1Blueprint(blueprintId).amount += amount;
    }

    public bool SpendD1Blueprint(string blueprintId, int amount)
    {
        if (string.IsNullOrEmpty(blueprintId))
            return false;

        if (amount <= 0)
            return true;

        if (!Dimension1System.IsDimension1BlueprintId(blueprintId))
            return false;

        EnsureDimension1State();

        D1BlueprintAmount blueprint = GetOrCreateD1Blueprint(blueprintId);

        if (blueprint.amount < amount)
            return false;

        blueprint.amount -= amount;
        return true;
    }

    public bool IsD1RelicUnlocked(string relicId)
    {
        if (!Dimension1System.IsDimension1RelicId(relicId))
            return false;

        EnsureDimension1State();

        D1RelicState relic = GetOrCreateD1Relic(relicId);
        return relic.unlocked;
    }

    public int GetD1RelicLevel(string relicId)
    {
        if (!Dimension1System.IsDimension1RelicId(relicId))
            return 0;

        EnsureDimension1State();

        D1RelicState relic = GetOrCreateD1Relic(relicId);
        return Dimension1System.ClampDimension1RelicLevel(relic.level);
    }

    public bool UnlockD1Relic(string relicId)
    {
        if (!Dimension1System.IsDimension1RelicId(relicId))
            return false;

        EnsureDimension1State();

        D1RelicState relic = GetOrCreateD1Relic(relicId);
        relic.unlocked = true;

        if (relic.level <= 0)
            relic.level = 1;

        relic.level = Dimension1System.ClampDimension1RelicLevel(relic.level);

        return true;
    }

    public bool SetD1RelicLevel(string relicId, int level)
    {
        if (!Dimension1System.IsDimension1RelicId(relicId))
            return false;

        EnsureDimension1State();

        D1RelicState relic = GetOrCreateD1Relic(relicId);
        int clampedLevel = Dimension1System.ClampDimension1RelicLevel(level);

        relic.level = clampedLevel;
        relic.unlocked = clampedLevel > 0;

        return true;
    }

    public bool TryAddD1RelicLevel(string relicId, int levelsToAdd)
    {
        if (levelsToAdd <= 0)
            return false;

        if (!UnlockD1Relic(relicId))
            return false;

        int currentLevel = GetD1RelicLevel(relicId);
        return SetD1RelicLevel(relicId, currentLevel + levelsToAdd);
    }

    public int GetD1TreeNodeTier(string nodeId)
    {
        if (!Dimension1System.IsDimension1TreeNodeId(nodeId))
            return 0;

        EnsureDimension1State();

        D1TreeNodeState node = GetOrCreateD1TreeNode(nodeId);

        return Dimension1System.ClampDimension1TreeNodeTier(
            nodeId,
            node.tier
        );
    }

    public bool IsD1TreeNodeUnlocked(string nodeId)
    {
        return GetD1TreeNodeTier(nodeId) > 0;
    }

    public bool SetD1TreeNodeTier(string nodeId, int tier)
    {
        if (!Dimension1System.IsDimension1TreeNodeId(nodeId))
            return false;

        EnsureDimension1State();

        D1TreeNodeState node = GetOrCreateD1TreeNode(nodeId);

        node.tier = Dimension1System.ClampDimension1TreeNodeTier(
            nodeId,
            tier
        );

        return true;
    }

    public bool AddPrestige1Points(int amount)
    {
        if (amount <= 0)
            return false;

        prestige1Points += amount;
        return true;
    }

    public void UnlockDimensionSystemAfterPrestige1()
    {
        dimension01Unlocked = true;
        dimension02Unlocked = true;
        dimension03Unlocked = true;

        EnsureDimension1State();

        D1PlanetState firstPlanet = GetOrCreateD1Planet(Dimension1System.Planet01);
        firstPlanet.unlocked = true;

        if (firstPlanet.extractorTier <= 0)
            firstPlanet.extractorTier = 1;

        D1ShipState lightProbe = GetOrCreateD1Ship(Dimension1System.ShipLightProbe);
        lightProbe.unlocked = true;
    }

    // Método temporal de compatibilidad. Borrar cuando no queden referencias antiguas.
    public void UnlockDimension1Mvp()
    {
        UnlockDimensionSystemAfterPrestige1();
    }

    public void ResetDimensionSystemState()
    {
        dimension01Unlocked = false;
        dimension02Unlocked = false;
        dimension03Unlocked = false;

        dimension1Metals = new List<D1MetalAmount>();
        dimension1Planets = new List<D1PlanetState>();
        dimension1Sectors = new List<D1SectorState>();
        dimension1SelectedSectorId = "";
        dimension1ActiveScanSectorId = "";
        dimension1Ships = new List<D1ShipState>();
        dimension1ScannedDestinations = new List<D1ScannedDestinationState>();
        dimension1PreviousScannedDestinationIds = new List<string>();
        dimension1ScanActive = false;
        dimension1ScanRemainingSeconds = 0.0;
        dimension1ScanTotalSeconds = 0.0;
        dimension1ScannerLevel = 0;
        dimension1LastExplorationDestinationId = "";
        dimension1LastExplorationRewards = new List<D1MetalAmount>();
        dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>();
        dimension1Blueprints = new List<D1BlueprintAmount>();
        dimension1Relics = new List<D1RelicState>();
        dimension1TreeNodes = new List<D1TreeNodeState>();
        prestige1Points = 0;
        prestige1BestClaimedPreviewPoints = 0;
        dimension1LastExplorationSpecificBlueprints = new List<D1BlueprintAmount>();
        dimension1LastExplorationRelics = new List<D1RelicRewardEntry>();
        dimension1BlueprintFragments = 0;
        dimension1LastExplorationBlueprintFragments = 0;
        dimension1LastExplorationResultId = 0;

        EnsureDimension1State();
    }

    // Método temporal de compatibilidad. Borrar cuando no queden referencias antiguas.
    public void ResetDimension1MvpState()
    {
        ResetDimensionSystemState();
    }

#if UNITY_EDITOR
    [ContextMenu("D1 DEBUG: Validate Part 1 Integrity")]
    private void DebugValidateD1Part1Integrity()
    {
        EnsureDimension1State();

        List<string> failures = new List<string>();

        if (Dimension1System.StarterPlanets.Length != 7)
            failures.Add("El catálogo activo no contiene exactamente 7 planetas.");

        if (Dimension1System.StarterMetals.Length != 10)
            failures.Add("El catálogo activo no contiene exactamente 10 metales.");

        if (Dimension1System.Dimension1ActiveShipIds.Length != 4)
            failures.Add("El catálogo activo no contiene exactamente 4 naves.");

        if (Dimension1System.Dimension1SectorIds.Length != 5)
            failures.Add("El catálogo no contiene exactamente 5 sectores.");

        if (dimension1Sectors.Count != Dimension1System.Dimension1SectorIds.Length)
            failures.Add("El estado persistente no contiene exactamente 5 sectores.");

        if (!IsD1SectorUnlocked(Dimension1System.Sector01OuterRim))
            failures.Add("El Sector 1 no está desbloqueado.");

        if (!IsD1SectorUnlocked(dimension1SelectedSectorId))
            failures.Add("El sector seleccionado no está desbloqueado.");

        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
        {
            string[] destinations =
                Dimension1System.GetDimension1SectorDestinationIds(sectorId);

            if (sectorId == Dimension1System.Sector05GalacticCenter)
            {
                if (destinations.Length != 0)
                    failures.Add("El Centro Galáctico contiene destinos normales.");

                continue;
            }

            if (destinations.Length != 4)
            {
                failures.Add(
                    Dimension1System.GetDimension1SectorVisualName(sectorId) +
                    " no contiene exactamente 4 destinos."
                );
            }

            float totalWeight = 0.0f;

            foreach (string destinationId in destinations)
            {
                totalWeight +=
                    Dimension1System.GetDimension1SectorDestinationWeight(
                        sectorId,
                        destinationId
                    );
            }

            if (Mathf.Abs(totalWeight - 1.0f) > 0.0001f)
            {
                failures.Add(
                    Dimension1System.GetDimension1SectorVisualName(sectorId) +
                    " no suma 100% de peso."
                );
            }
        }

        foreach (string planetId in Dimension1System.StarterPlanets)
        {
            if (!Dimension1System.IsDimension1ExplorationSectorId(
                Dimension1System.GetDimension1PlanetSectorId(planetId)
            ))
            {
                failures.Add("Un planeta no está asignado a un sector normal.");
            }
        }

        if (Dimension1System.Dimension1RelicIds.Length != 10)
            failures.Add("El catálogo activo no contiene exactamente 10 reliquias.");

        if (Dimension1System.Dimension1TreeNodeIds.Length != 10)
            failures.Add("El catálogo activo no contiene exactamente 10 nodos.");

        if (dimension1ScannerLevel < 0 ||
            dimension1ScannerLevel > Dimension1System.SimpleScannerMaxLevel)
        {
            failures.Add("El nivel del escáner está fuera del rango 0-3.");
        }

        if (dimension1ScanActive &&
            (dimension1ScanRemainingSeconds < 0.0 ||
             dimension1ScanTotalSeconds < dimension1ScanRemainingSeconds))
        {
            failures.Add("Los tiempos del escaneo activo no son coherentes.");
        }

        if (dimension1ScanActive &&
            !Dimension1System.IsDimension1ExplorationSectorId(
                dimension1ActiveScanSectorId
            ))
        {
            failures.Add("El escaneo activo no tiene un sector de origen válido.");
        }

        foreach (D1SectorState sector in dimension1Sectors)
        {
            if (sector == null)
            {
                failures.Add("Existe un sector nulo.");
                continue;
            }

            if (sector.completedExplorations < 0)
                failures.Add("Un sector tiene exploraciones negativas.");
        }

        foreach (D1ScannedDestinationState destination in dimension1ScannedDestinations)
        {
            if (destination != null &&
                !Dimension1System.IsDimension1ExplorationSectorId(
                    destination.sectorId
                ))
            {
                failures.Add("Un destino escaneado no tiene sector válido.");
            }

            if (destination != null &&
                !Dimension1System.IsDestinationInDimension1Sector(
                    destination.destinationId,
                    destination.sectorId
                ))
            {
                failures.Add("Un destino escaneado no pertenece a su sector.");
            }
        }

        if (dimension1RecentExplorationRecords.Count >
            Dimension1System.Dimension1RecentExplorationHistoryLimit)
        {
            failures.Add("El historial supera las 20 exploraciones.");
        }

        HashSet<string> activeMissionDestinations = new HashSet<string>();
        int activeMissions = 0;
        int frozenFutureMissions = 0;

        foreach (D1ShipState ship in dimension1Ships)
        {
            if (ship == null || !ship.explorationActive)
                continue;

            if (!Dimension1System.IsShipActiveInDimension1Base(ship.shipId))
            {
                frozenFutureMissions++;
                continue;
            }

            activeMissions++;

            if (string.IsNullOrEmpty(ship.activeDestinationId))
                failures.Add("Una misión activa no tiene destino.");
            else if (!activeMissionDestinations.Add(
                ship.activeSectorId + "|" + ship.activeDestinationId
            ))
            {
                failures.Add(
                    "Dos naves activas comparten el mismo destino y sector."
                );
            }

            if (!Dimension1System.IsDimension1ExplorationSectorId(
                ship.activeSectorId
            ))
            {
                failures.Add("Una misión activa no tiene sector de origen válido.");
            }

            if (ship.explorationRemainingSeconds < 0.0 ||
                ship.explorationTotalSeconds < ship.explorationRemainingSeconds)
            {
                failures.Add("Una misión activa tiene tiempos incoherentes.");
            }
        }

        int d1PrestigePoints =
            Dimension1System.CalculatePrestige1PointsFromDimension1(this);

        if (d1PrestigePoints < 0 ||
            d1PrestigePoints > Dimension1System.Dimension1Prestige1PreviewPointCap)
        {
            failures.Add("El aporte P1 de D1 está fuera del rango 0-12.");
        }

        string summary =
            "[D1 Parte 1 Integrity] " +
            (failures.Count == 0 ? "PASS" : "FAIL") +
            " | Misiones activas: " + activeMissions +
            " | Misiones futuras congeladas: " + frozenFutureMissions +
            " | Historial: " + dimension1RecentExplorationRecords.Count + "/" +
            Dimension1System.Dimension1RecentExplorationHistoryLimit +
            " | P1 D1: " + d1PrestigePoints + "/" +
            Dimension1System.Dimension1Prestige1PreviewPointCap;

        if (failures.Count == 0)
        {
            Debug.Log(summary);
            return;
        }

        Debug.LogError(summary + "\n- " + string.Join("\n- ", failures));
    }

    [ContextMenu("D1 DEBUG: Ensure State")]
    private void DebugEnsureDimension1State()
    {
        EnsureDimension1State();
        Debug.Log("[D1] Estado base inicializado.");
    }

    [ContextMenu("D1 DEBUG: Print Galaxy Sectors")]
    private void DebugPrintD1GalaxySectors()
    {
        EnsureDimension1State();

        var lines = new List<string>();

        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
        {
            D1SectorState sector = FindD1SectorState(sectorId);
            lines.Add(
                Dimension1System.GetDimension1SectorVisualName(sectorId) +
                " | " +
                (sector != null && sector.unlocked ? "DESBLOQUEADO" : "BLOQUEADO") +
                " | Exploraciones: " +
                (sector != null ? sector.completedExplorations : 0)
            );
        }

        Debug.Log("[D1 Galaxy]\n" + string.Join("\n", lines));
    }

    [ContextMenu("D1 DEBUG: Print Selected Sector Scan Pool")]
    private void DebugPrintD1SelectedSectorScanPool()
    {
        EnsureDimension1State();

        string sectorId = dimension1SelectedSectorId;
        string[] destinations =
            Dimension1System.GetDimension1SectorDestinationIds(sectorId);
        var lines = new List<string>();

        foreach (string destinationId in destinations)
        {
            float weight =
                Dimension1System.GetDimension1SectorDestinationWeight(
                    sectorId,
                    destinationId
                );

            lines.Add(
                destinationId +
                " | " +
                (weight * 100.0f).ToString("0.##") +
                "%"
            );
        }

        string content = lines.Count > 0
            ? string.Join("\n", lines)
            : "Sin destinos normales.";

        Debug.Log(
            "[D1 Scan Pool] " +
            Dimension1System.GetDimension1SectorVisualName(sectorId) +
            "\n" +
            content +
            "\nPunto especial base: " +
            (Dimension1System.GetDimension1SectorSpecialPointBaseChance(
                sectorId
            ) * 100.0f).ToString("0.##") +
            "%"
        );
    }

    [ContextMenu("D1 DEBUG: Force Unlock Next Sector")]
    private void DebugForceUnlockNextD1Sector()
    {
        EnsureDimension1State();

        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
        {
            if (IsD1SectorUnlocked(sectorId))
                continue;

            UnlockD1Sector(sectorId);
            Debug.Log(
                "[D1 Galaxy] Desbloqueado: " +
                Dimension1System.GetDimension1SectorVisualName(sectorId)
            );
            return;
        }

        Debug.Log("[D1 Galaxy] Todos los sectores ya están desbloqueados.");
    }

    [ContextMenu("D1 DEBUG: Force Selected Sector")]
    private void DebugForceSelectedD1Sector()
    {
        EnsureDimension1State();

        int currentIndex = System.Array.IndexOf(
            Dimension1System.Dimension1SectorIds,
            dimension1SelectedSectorId
        );

        for (int offset = 1;
             offset <= Dimension1System.Dimension1SectorIds.Length;
             offset++)
        {
            int index = (currentIndex + offset) %
                Dimension1System.Dimension1SectorIds.Length;
            string sectorId = Dimension1System.Dimension1SectorIds[index];

            if (!IsD1SectorUnlocked(sectorId))
                continue;

            if (!TrySelectD1Sector(sectorId))
            {
                Debug.LogWarning(
                    "[D1 Galaxy] No se puede cambiar de sector durante un escaneo activo."
                );
                return;
            }

            Debug.Log(
                "[D1 Galaxy] Sector seleccionado: " +
                Dimension1System.GetDimension1SectorVisualName(sectorId)
            );
            return;
        }
    }

    [ContextMenu("D1 DEBUG: Print Selected Sector")]
    private void DebugPrintSelectedD1Sector()
    {
        EnsureDimension1State();
        Debug.Log(
            "[D1 Galaxy] Sector seleccionado: " +
            Dimension1System.GetDimension1SectorVisualName(
                dimension1SelectedSectorId
            )
        );
    }

    [ContextMenu("D1 DEBUG: Add 50 Sector Exploration Count")]
    private void DebugAddD1SectorExplorationCount()
    {
        EnsureDimension1State();

        bool added = AddD1SectorExplorationCount(
            dimension1SelectedSectorId,
            50
        );

        Debug.Log(
            "[D1 Galaxy] +50 exploraciones => " + added +
            " | " +
            Dimension1System.GetDimension1SectorVisualName(
                dimension1SelectedSectorId
            ) +
            ": " +
            GetD1SectorExplorationCount(dimension1SelectedSectorId)
        );
    }

    [ContextMenu("D1 DEBUG: Unlock Dimension System")]
    private void DebugUnlockDimensionSystem()
    {
        UnlockDimensionSystemAfterPrestige1();

        if (TabsUI.Instance != null)
        {
            TabsUI.Instance.RefreshDimension1ButtonVisibility();
        }

        D1PlanetState planet = null;

        foreach (D1PlanetState candidate in dimension1Planets)
        {
            if (candidate != null && candidate.planetId == Dimension1System.Planet01)
            {
                planet = candidate;
                break;
            }
        }

        int tier = planet != null ? planet.extractorTier : 0;
        bool unlocked = planet != null && planet.unlocked;

        Debug.Log(
            "[D1] Sistema de dimensiones preparado. " +
            "dimension01Unlocked: " + dimension01Unlocked +
            " | dimension02Unlocked: " + dimension02Unlocked +
            " | dimension03Unlocked: " + dimension03Unlocked +
            " | Planeta 1 unlocked: " + unlocked +
            " | Tier: " + tier
        );
    }

    [ContextMenu("D1 DEBUG: Print Metals")]
    private void DebugPrintDimension1Metals()
    {
        EnsureDimension1State();

        Debug.Log(
            "[D1] Metales => " +
            "Hierro: " + GetD1MetalAmount(Dimension1System.MetalIron).ToString("0.00") +
            " | Cobre: " + GetD1MetalAmount(Dimension1System.MetalCopper).ToString("0.00") +
            " | Aluminio: " + GetD1MetalAmount(Dimension1System.MetalAluminum).ToString("0.00") +
            " | Titanio: " + GetD1MetalAmount(Dimension1System.MetalTitanium).ToString("0.00") +
            " | Níquel: " + GetD1MetalAmount(Dimension1System.MetalNickel).ToString("0.00") +
            " | Cobalto: " + GetD1MetalAmount(Dimension1System.MetalCobalt).ToString("0.00")
        );
    }

    [ContextMenu("D1 DEBUG: Print Planet 1")]
    private void DebugPrintD1Planet01()
    {
        EnsureDimension1State();

        D1PlanetState planet = null;

        foreach (D1PlanetState candidate in dimension1Planets)
        {
            if (candidate != null && candidate.planetId == Dimension1System.Planet01)
            {
                planet = candidate;
                break;
            }
        }

        if (planet == null)
        {
            Debug.Log("[D1] Planeta 1 no existe en la lista.");
            return;
        }

        Debug.Log(
            "[D1] Planeta 1 => " +
            "unlocked: " + planet.unlocked +
            " | extractorTier: " + planet.extractorTier
        );
    }

    [ContextMenu("D1 DEBUG: Upgrade Planet 2 Extractor")]
    private void DebugUpgradeD1Planet02Extractor()
    {
        bool upgraded = Dimension1System.TryUpgradeExtractor(this, Dimension1System.Planet02);

        if (upgraded)
        {
            EnsureDimension1State();

            D1PlanetState planet = null;

            foreach (D1PlanetState candidate in dimension1Planets)
            {
                if (candidate != null && candidate.planetId == Dimension1System.Planet02)
                {
                    planet = candidate;
                    break;
                }
            }

            int tier = planet != null ? planet.extractorTier : 0;

            Debug.Log("[D1] Extractor de Planeta 2 mejorado. Tier actual: " + tier);
        }
        else
        {
            Debug.Log("[D1] No se pudo mejorar el extractor de Planeta 2. Revisa si tienes suficiente Aluminio.");
        }
    }

    [ContextMenu("D1 DEBUG: Unlock Planet 2")]
    private void DebugUnlockD1Planet02()
    {
        bool unlocked = Dimension1System.TryUnlockPlanet(this, Dimension1System.Planet02);

        if (unlocked)
        {
            Debug.Log("[D1] Planeta 2 desbloqueado. Ahora debería producir Aluminio.");
        }
        else
        {
            Debug.Log("[D1] No se pudo desbloquear Planeta 2. Necesitas Hierro y Cobre suficientes.");
        }
    }

    [ContextMenu("D1 DEBUG: Upgrade Planet 1 Extractor")]
    private void DebugUpgradeD1Planet01Extractor()
    {
        bool upgraded = Dimension1System.TryUpgradeExtractor(this, Dimension1System.Planet01);

        if (upgraded)
        {
            EnsureDimension1State();

            D1PlanetState planet = null;

            foreach (D1PlanetState candidate in dimension1Planets)
            {
                if (candidate != null && candidate.planetId == Dimension1System.Planet01)
                {
                    planet = candidate;
                    break;
                }
            }

            int tier = planet != null ? planet.extractorTier : 0;

            Debug.Log("[D1] Extractor de Planeta 1 mejorado. Tier actual: " + tier);
        }
        else
        {
            Debug.Log("[D1] No se pudo mejorar el extractor de Planeta 1. Revisa si tienes suficiente Hierro.");
        }
    }

    [ContextMenu("D1 DEBUG: Simulate 1h Offline Mining")]
    private void DebugSimulateD1OneHourOfflineMining()
    {
        double appliedSeconds = Dimension1System.ApplyOfflineMining(this, 3600.0);

        Debug.Log(
            "[D1] Simulación offline minería: " +
            appliedSeconds.ToString("0") +
            " segundos aplicados."
        );
    }

    [ContextMenu("D1 DEBUG: Print Planet 2")]
    private void DebugPrintD1Planet02()
    {
        EnsureDimension1State();

        D1PlanetState planet = null;

        foreach (D1PlanetState candidate in dimension1Planets)
        {
            if (candidate != null && candidate.planetId == Dimension1System.Planet02)
            {
                planet = candidate;
                break;
            }
        }

        if (planet == null)
        {
            Debug.Log("[D1] Planeta 2 no existe en la lista.");
            return;
        }

        Debug.Log(
            "[D1] Planeta 2 => " +
            "unlocked: " + planet.unlocked +
            " | extractorTier: " + planet.extractorTier
        );
    }

    [ContextMenu("D1 DEBUG: Unlock Planet 3")]
    private void DebugUnlockD1Planet03()
    {
        bool unlocked = Dimension1System.TryUnlockPlanet(this, Dimension1System.Planet03);

        if (unlocked)
        {
            Debug.Log("[D1] Planeta 3 desbloqueado. Ahora debería producir Níquel.");
        }
        else
        {
            Debug.Log("[D1] No se pudo desbloquear Planeta 3. Necesitas Aluminio y Titanio suficientes.");
        }
    }

    [ContextMenu("D1 DEBUG: Print Planet 3")]
    private void DebugPrintD1Planet03()
    {
        EnsureDimension1State();

        D1PlanetState planet = null;

        foreach (D1PlanetState candidate in dimension1Planets)
        {
            if (candidate != null && candidate.planetId == Dimension1System.Planet03)
            {
                planet = candidate;
                break;
            }
        }

        if (planet == null)
        {
            Debug.Log("[D1] Planeta 3 no existe en la lista.");
            return;
        }

        Debug.Log(
            "[D1] Planeta 3 => " +
            "unlocked: " + planet.unlocked +
            " | extractorTier: " + planet.extractorTier
        );
    }

    [ContextMenu("D1 DEBUG: Upgrade Planet 3 Extractor")]
    private void DebugUpgradeD1Planet03Extractor()
    {
        bool upgraded = Dimension1System.TryUpgradeExtractor(this, Dimension1System.Planet03);

        if (upgraded)
        {
            EnsureDimension1State();

            D1PlanetState planet = null;

            foreach (D1PlanetState candidate in dimension1Planets)
            {
                if (candidate != null && candidate.planetId == Dimension1System.Planet03)
                {
                    planet = candidate;
                    break;
                }
            }

            int tier = planet != null ? planet.extractorTier : 0;

            Debug.Log("[D1] Extractor de Planeta 3 mejorado. Tier actual: " + tier);
        }
        else
        {
            Debug.Log("[D1] No se pudo mejorar el extractor de Planeta 3. Revisa si tienes suficiente Níquel.");
        }
    }

    [ContextMenu("D1 DEBUG: Add 1 Adaptive Matrix")]
    private void DebugAddOneD1AdaptiveMatrix()
    {
        EnsureDimension1State();

        dimension1BlueprintFragments += Dimension1System.BlueprintFragmentsPerBlueprint;

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1] +1 Matriz Adaptativa de Nave. " +
            "Fragmentos totales: " + dimension1BlueprintFragments +
            " | Adaptativas disponibles: " + Dimension1System.GetCompletedBlueprintCount(this)
        );
    }

    [ContextMenu("D1 DEBUG: Add All Specific Ship Matrices")]
    private void DebugAddCargoSpecificMatrices()
    {
        EnsureDimension1State();

        AddD1Blueprint(Dimension1System.BlueprintCargoFrame, 1);
        AddD1Blueprint(Dimension1System.BlueprintCargoHold, 1);
        AddD1Blueprint(Dimension1System.BlueprintCargoStabilizer, 1);
        AddD1Blueprint(Dimension1System.BlueprintRescueFrame, 1);
        AddD1Blueprint(Dimension1System.BlueprintRescueBeacon, 1);
        AddD1Blueprint(Dimension1System.BlueprintRescueRecoveryBay, 1);
        AddD1Blueprint(Dimension1System.BlueprintRescueProtectionMatrix, 1);
        AddD1Blueprint(Dimension1System.BlueprintConvergenceChassis, 1);
        AddD1Blueprint(Dimension1System.BlueprintConvergenceCore, 1);
        AddD1Blueprint(Dimension1System.BlueprintConvergenceMatrix, 1);
        AddD1Blueprint(Dimension1System.BlueprintAnomalousArmor, 1);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1] Matrices específicas de Nave de Carga agregadas. " +
            "Chasis=" + GetD1BlueprintAmount(Dimension1System.BlueprintCargoFrame) +
            " | Bodega=" + GetD1BlueprintAmount(Dimension1System.BlueprintCargoHold) +
            " | Estabilizador=" + GetD1BlueprintAmount(Dimension1System.BlueprintCargoStabilizer)
        );
    }

    [ContextMenu("D1 DEBUG: Validate Specific Ship Matrix Coverage")]
    private void DebugValidateAdvancedShipMatrixCoverage()
    {
        EnsureDimension1State();

        Debug.Log("[D1 Matrix Coverage] Validando matrices de Nave de Carga...");

        Debug.Log(
            BuildAdvancedShipMatrixCoverageDebugLine(
                Dimension1System.ShipCargoShip,
                "Nave de Carga"
            )
        );

        Debug.Log(
            BuildAdvancedShipMatrixCoverageDebugLine(
                Dimension1System.ShipRescueShip,
                "Nave de Rescate"
            )
        );

        Debug.Log(
            BuildAdvancedShipMatrixCoverageDebugLine(
                Dimension1System.ShipConvergenceShip,
                "Nave de Convergencia"
            )
        );

    }

    private string BuildAdvancedShipMatrixCoverageDebugLine(string shipId, string shipName)
    {
        bool hasRequiredMatrixList = Dimension1System.TryGetRequiredShipBlueprintIds(
            shipId,
            out string[] matrixIds
        );

        int required = hasRequiredMatrixList && matrixIds != null ? matrixIds.Length : 0;
        int ownedSpecific = Dimension1System.GetOwnedRequiredSpecificShipMatrixCount(this, shipId);
        int missingSpecific = Dimension1System.GetMissingRequiredSpecificShipMatrixCount(this, shipId);
        int adaptiveAvailable = Dimension1System.GetCompletedBlueprintCount(this);
        bool canCover = Dimension1System.CanCoverRequiredShipMatrices(this, shipId);

        bool unlocked = false;

        if (dimension1Ships != null)
        {
            foreach (D1ShipState ship in dimension1Ships)
            {
                if (ship != null && ship.shipId == shipId)
                {
                    unlocked = ship.unlocked;
                    break;
                }
            }
        }

        string matrixBreakdown = "";

        if (hasRequiredMatrixList && matrixIds != null)
        {
            foreach (string matrixId in matrixIds)
            {
                if (!string.IsNullOrEmpty(matrixBreakdown))
                    matrixBreakdown += " | ";

                matrixBreakdown += matrixId + "=" + GetD1BlueprintAmount(matrixId);
            }
        }

        return
            "[D1 Matrix Coverage] " +
            shipName +
            " | Desbloqueada=" + unlocked +
            " | Requeridas=" + required +
            " | Específicas propias=" + ownedSpecific +
            " | Faltantes=" + missingSpecific +
            " | Adaptativas disponibles=" + adaptiveAvailable +
            " | Puede cubrir=" + canCover +
            " | Detalle: " + matrixBreakdown;
    }

    [ContextMenu("D1 DEBUG: Print Relics")]
    private void DebugPrintD1Relics()
    {
        EnsureDimension1State();

        Debug.Log("[D1 Relics] Estado actual de Reliquias:");

        DebugPrintD1RelicLine(Dimension1System.RelicDriftCompass, "Brújula de Deriva");
        DebugPrintD1RelicLine(Dimension1System.RelicAncientCargoCore, "Núcleo de Bodega Antigua");
        DebugPrintD1RelicLine(Dimension1System.RelicLostNavigationRecord, "Registro de Navegación Perdido");
        DebugPrintD1RelicLine(Dimension1System.RelicDormantEcho, "Eco de Reliquia Dormida");

        DebugPrintD1RelicLine(Dimension1System.RelicExplorerPlate, "Placa de Explorador");
        DebugPrintD1RelicLine(Dimension1System.RelicExtractionHook, "Gancho de Extracción");
        DebugPrintD1RelicLine(Dimension1System.RelicAnalyticCrystal, "Cristal Analítico");
        DebugPrintD1RelicLine(Dimension1System.RelicModularContainer, "Contenedor Modular");

        DebugPrintD1RelicLine(Dimension1System.RelicAncientDrill, "Taladro Antiguo");
        DebugPrintD1RelicLine(Dimension1System.RelicRoom1Echo, "Eco del Cuarto 1");
    }

    private void DebugPrintD1RelicLine(string relicId, string relicName)
    {
        bool unlocked = IsD1RelicUnlocked(relicId);
        int level = GetD1RelicLevel(relicId);

        Debug.Log(
            "[D1 Relics] " +
            relicName +
            " | ID=" +
            relicId +
            " | Desbloqueada=" +
            unlocked +
            " | Nivel=" +
            level
        );
    }

    [ContextMenu("D1 DEBUG: Unlock Drift Compass Relic")]
    private void DebugUnlockD1DriftCompassRelic()
    {
        EnsureDimension1State();

        UnlockD1Relic(Dimension1System.RelicDriftCompass);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1] Reliquia desbloqueada: Brújula de Deriva | Nivel: " +
            GetD1RelicLevel(Dimension1System.RelicDriftCompass)
        );
    }

    [ContextMenu("D1 DEBUG: Add +1 Level Drift Compass Relic")]
    private void DebugAddLevelD1DriftCompassRelic()
    {
        EnsureDimension1State();

        TryAddD1RelicLevel(Dimension1System.RelicDriftCompass, 1);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1] Brújula de Deriva nivel: " +
            GetD1RelicLevel(Dimension1System.RelicDriftCompass) +
            " | Hito: " +
            Dimension1System.GetDimension1RelicMilestone(
                GetD1RelicLevel(Dimension1System.RelicDriftCompass)
            )
        );
    }

    [ContextMenu("D1 DEBUG: Add Ship Upgrade Test Resources")]
    private void DebugAddD1ShipUpgradeTestResources()
    {
        EnsureDimension1State();

        AddD1Metal(Dimension1System.MetalIron, 1000000.0);
        AddD1Metal(Dimension1System.MetalCopper, 1000000.0);
        AddD1Metal(Dimension1System.MetalAluminum, 1000000.0);
        AddD1Metal(Dimension1System.MetalTitanium, 1000000.0);
        AddD1Metal(Dimension1System.MetalNickel, 1000000.0);
        AddD1Metal(Dimension1System.MetalCobalt, 1000000.0);
        AddD1Metal(Dimension1System.MetalLithium, 1000000.0);
        AddD1Metal(Dimension1System.MetalTungsten, 1000000.0);
        AddD1Metal(Dimension1System.MetalPlatinum, 1000000.0);
        AddD1Metal(Dimension1System.MetalIridium, 1000000.0);

        dimension1BlueprintFragments += Dimension1System.BlueprintFragmentsPerBlueprint * 30;

        AddD1Blueprint(Dimension1System.BlueprintCargoFrame, 10);
        AddD1Blueprint(Dimension1System.BlueprintCargoHold, 10);
        AddD1Blueprint(Dimension1System.BlueprintCargoStabilizer, 10);
        AddD1Blueprint(Dimension1System.BlueprintRescueFrame, 10);
        AddD1Blueprint(Dimension1System.BlueprintRescueBeacon, 10);
        AddD1Blueprint(Dimension1System.BlueprintRescueRecoveryBay, 10);
        AddD1Blueprint(Dimension1System.BlueprintRescueProtectionMatrix, 10);
        AddD1Blueprint(Dimension1System.BlueprintConvergenceChassis, 10);
        AddD1Blueprint(Dimension1System.BlueprintConvergenceCore, 10);
        AddD1Blueprint(Dimension1System.BlueprintConvergenceMatrix, 10);
        AddD1Blueprint(Dimension1System.BlueprintAnomalousArmor, 10);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1] DEBUG: recursos de prueba para Taller de Naves agregados. " +
            "Adaptativas disponibles: " + Dimension1System.GetCompletedBlueprintCount(this)
        );
    }

    [ContextMenu("D1 DEBUG: Add Relic Upgrade Test Resources")]
    private void DebugAddD1RelicUpgradeTestResources()
    {
        EnsureDimension1State();

        LE += 1000000.0;
        Traces += 100000.0;

        AddD1Metal(Dimension1System.MetalIron, 1000000.0);
        AddD1Metal(Dimension1System.MetalCopper, 1000000.0);
        AddD1Metal(Dimension1System.MetalAluminum, 1000000.0);
        AddD1Metal(Dimension1System.MetalTitanium, 1000000.0);
        AddD1Metal(Dimension1System.MetalNickel, 1000000.0);
        AddD1Metal(Dimension1System.MetalCobalt, 1000000.0);
        AddD1Metal(Dimension1System.MetalLithium, 1000000.0);
        AddD1Metal(Dimension1System.MetalTungsten, 1000000.0);
        AddD1Metal(Dimension1System.MetalPlatinum, 1000000.0);
        AddD1Metal(Dimension1System.MetalIridium, 1000000.0);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log("[D1] DEBUG: recursos de prueba agregados para subir Reliquias.");
    }

    [ContextMenu("D1 DEBUG: Upgrade Drift Compass Relic With Cost")]
    private void DebugUpgradeD1DriftCompassRelicWithCost()
    {
        EnsureDimension1State();

        string relicId = Dimension1System.RelicDriftCompass;

        if (!IsD1RelicUnlocked(relicId))
            UnlockD1Relic(relicId);

        bool upgraded = Dimension1System.TryUpgradeDimension1Relic(this, relicId);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1] Mejorar Brújula de Deriva con costo => " +
            upgraded +
            " | Nivel: " +
            GetD1RelicLevel(relicId) +
            " | Hito: " +
            Dimension1System.GetDimension1RelicMilestone(GetD1RelicLevel(relicId))
        );
    }

    [ContextMenu("D1 DEBUG: Max Effect Relics")]
    private void DebugMaxD1EffectRelics()
    {
        EnsureDimension1State();

        SetD1RelicLevel(Dimension1System.RelicDriftCompass, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicAncientCargoCore, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicLostNavigationRecord, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicDormantEcho, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicExplorerPlate, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicExtractionHook, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicAnalyticCrystal, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicModularContainer, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicAncientDrill, Dimension1System.Dimension1RelicMaxLevel);
        SetD1RelicLevel(Dimension1System.RelicRoom1Echo, Dimension1System.Dimension1RelicMaxLevel);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log("[D1] DEBUG: Reliquias base al nivel maximo.");
    }

    [ContextMenu("D1 DEBUG: Add +50 Prestige 1 Points")]
    private void DebugAddPrestige1PointsForD1Tree()
    {
        EnsureDimension1State();

        AddPrestige1Points(50);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log("[D1] +50 Puntos de Prestigio 1 agregados. Total: " + prestige1Points);
    }

    [ContextMenu("D1 DEBUG: Buy Destination Reading Node")]
    private void DebugBuyD1DestinationReadingNode()
    {
        EnsureDimension1State();

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeExplorationDestinationReading
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Lectura de Destinos => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeExplorationDestinationReading) +
            " | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Missing Matrix Priority Node")]
    private void DebugBuyD1BlueprintPriorityNode()
    {
        EnsureDimension1State();

        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeRecoveryCopyRegistry);
        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeRecoveryPartialRecovery);

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeRecoveryBlueprintPriority
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Prioridad de Matriz Faltante => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeRecoveryBlueprintPriority) +
            " | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Hidden Find Tracking Node")]
    private void DebugBuyD1HiddenFindTrackingNode()
    {
        EnsureDimension1State();

        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeExplorationDestinationReading);

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeExplorationHiddenFindTracking
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Rastreo de Hallazgos Ocultos => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeExplorationHiddenFindTracking) +
            " | Bonus: +" +
            (Dimension1System.GetD1TreeHiddenFindQualityBonus(this) * 100f).ToString("0.#") +
            " puntos porcentuales | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Scan Memory Node")]
    private void DebugBuyD1ScanMemoryNode()
    {
        EnsureDimension1State();

        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeExplorationDestinationReading);
        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeExplorationHiddenFindTracking);

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeExplorationScanMemory
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Memoria de Escaneo => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeExplorationScanMemory) +
            " | Reducción repetición: -" +
            (Dimension1System.GetD1TreeScanMemoryRepetitionReduction(this) * 100f).ToString("0.#") +
            "% | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Copy Registry Node")]
    private void DebugBuyD1CopyRegistryNode()
    {
        EnsureDimension1State();

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeRecoveryCopyRegistry
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Registro de Copias => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeRecoveryCopyRegistry) +
            " | Bonus conversión duplicada: +" +
            (Dimension1System.GetD1TreeDuplicateRelicConversionBonus(this) * 100f).ToString("0.#") +
            "% | Conversión actual: " +
            Dimension1System.GetD1DuplicateRelicConversionPreviewAmount(this).ToString("0") +
            " Hierro | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Material Recovery Node")]
    private void DebugBuyD1PartialRecoveryNode()
    {
        EnsureDimension1State();

        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeRecoveryCopyRegistry);

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeRecoveryPartialRecovery
        );

        Dimension1System.GetD1TreePartialRecoveryValues(
            this,
            out float chance,
            out float recoveredAmount
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Recuperación de Materiales => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeRecoveryPartialRecovery) +
            " | Chance: " +
            (chance * 100f).ToString("0.#") +
            "% | Recupera: +" +
            (recoveredAmount * 100f).ToString("0.#") +
            "% | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Hangar Preparation Node")]
    private void DebugBuyD1HangarPreparationNode()
    {
        EnsureDimension1State();

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeFleetHangarPreparation
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Preparación de Hangar => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeFleetHangarPreparation) +
            " | Materiales individuales: +" +
            (Dimension1System.GetD1TreeSingleShipEfficiencyBonus(this) * 100f).ToString("0.#") +
            "% | Duración individual: -" +
            (Dimension1System.GetD1TreeSingleShipEfficiencyBonus(this) * 100f).ToString("0.#") +
            "% | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Route Optimization Node")]
    private void DebugBuyD1SupportFormationNode()
    {
        EnsureDimension1State();

        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeFleetHangarPreparation);

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeFleetSupportFormation
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Optimización de Ruta => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeFleetSupportFormation) +
            " | Reducción de duración individual: -" +
            (Dimension1System.GetD1TreeSupportFormationValue(this) * 100f).ToString("0.#") +
            "% | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Unstable Zone Stabilization Node")]
    private void DebugBuyD1UnstableZoneStabilizationNode()
    {
        EnsureDimension1State();

        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeExplorationDestinationReading);
        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeConvergenceSpecialDestinationReading);

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeConvergenceUnstableZoneStabilization
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Estabilización Zona Inestable => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeConvergenceUnstableZoneStabilization) +
            " | Reducción duración en Zona Inestable: -" +
            (Dimension1System.GetD1TreeUnstableZoneDurationReduction(this) * 100f).ToString("0.#") +
            "% | Conservación final: +" +
            (Dimension1System.GetD1TreeUnstableZoneRareRewardProtection(this) * 100f).ToString("0.#") +
            "% | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy Special Destination Reading Node")]
    private void DebugBuyD1SpecialDestinationReadingNode()
    {
        EnsureDimension1State();

        TryBuyD1TreeNodeForDebug(Dimension1System.D1TreeExplorationDestinationReading);

        bool bought = Dimension1System.TryBuyDimension1TreeNode(
            this,
            Dimension1System.D1TreeConvergenceSpecialDestinationReading
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Tree] Comprar Lectura de Destinos Especiales => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeConvergenceSpecialDestinationReading) +
            " | Bonus detección especial: +" +
            (Dimension1System.GetD1TreeSpecialDestinationDetectionBonus(this) * 100f).ToString("0.#") +
            " puntos porcentuales | Chance total puntos especiales: " +
            (Dimension1System.GetD1SpecialPointScanChance(this) * 100f).ToString("0.#") +
            "% | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Print Tree Nodes")]
    private void DebugPrintD1TreeNodes()
    {
        EnsureDimension1State();

        Debug.Log(
            "[D1 Tree] Estado actual | Puntos restantes: " +
            prestige1Points
        );

        foreach (string nodeId in Dimension1System.Dimension1TreeNodeIds)
        {
            int tier = GetD1TreeNodeTier(nodeId);
            int maxTier = Dimension1System.GetDimension1TreeNodeMaxTier(nodeId);

            if (tier <= 0)
                continue;

            Debug.Log(
                "[D1 Tree] " +
                Dimension1System.GetDimension1TreeNodeVisualName(nodeId) +
                " (" +
                nodeId +
                ")" +
                " | Tier: " +
                tier +
                "/" +
                maxTier
            );
        }
    }

    [ContextMenu("D1 DEBUG: Print Exploration Bonuses")]
    private void DebugPrintD1ExplorationBonuses()
    {
        EnsureDimension1State();

        D1ShipState ship = GetFirstUnlockedD1ShipForDebug();

        if (ship == null)
        {
            Debug.LogWarning("[D1 Bonuses] No hay nave desbloqueada para probar.");
            return;
        }

        Dimension1System.GetD1TreePartialRecoveryValues(
            this,
            out float partialRecoveryChance,
            out float partialRecoveryAmount
        );

        Debug.Log(
            "[D1 Bonuses] Resumen general" +
            " | Nave usada: " +
            GetD1DebugShipName(ship.shipId) +
            " | Preparación Hangar: +" +
            (Dimension1System.GetD1TreeSingleShipEfficiencyBonus(this) * 100f).ToString("0.#") +
            "% | Optimización de Ruta: -" +
            (Dimension1System.GetD1TreeSupportFormationValue(this) * 100f).ToString("0.#") +
            "% | Prioridad Matriz: +" +
            (Dimension1System.GetD1TreeBlueprintPriorityBonus(this) * 100f).ToString("0.#") +
            "% | Rastreo Oculto: +" +
            (Dimension1System.GetD1TreeHiddenFindQualityBonus(this) * 100f).ToString("0.#") +
            "% | Recuperación de Materiales: " +
            (partialRecoveryChance * 100f).ToString("0.#") +
            "% de +" +
            (partialRecoveryAmount * 100f).ToString("0.#") +
            "%"
        );

        if (dimension1ScannedDestinations == null ||
            dimension1ScannedDestinations.Count == 0)
        {
            Debug.LogWarning("[D1 Bonuses] No hay destinos escaneados. Haz un escaneo primero.");
            return;
        }

        foreach (D1ScannedDestinationState destination in dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available)
                continue;

            if (string.IsNullOrEmpty(destination.destinationId))
                continue;

            DebugPrintD1ExplorationBonusLine(destination.destinationId, ship);
        }
    }

    [ContextMenu("D1 DEBUG: Force Special Point")]
    private void DebugForceD1SpecialPoint()
    {
        EnsureDimension1State();

        bool assigned =
            Dimension1System.TryForceD1SpecialPointOnFirstCompatibleScannedDestination(this);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Special Point] Forzar punto especial => " +
            assigned
        );

        DebugPrintD1SpecialPoints();
    }

    [ContextMenu("D1 DEBUG: Force Special Point Relic")]
    private void DebugForceD1SpecialPointRelic()
    {
        DebugForceSpecificD1SpecialPoint(Dimension1System.D1SpecialPointRelicEcho);
    }

    [ContextMenu("D1 DEBUG: Force Special Point Matrix")]
    private void DebugForceD1SpecialPointMatrix()
    {
        DebugForceSpecificD1SpecialPoint(Dimension1System.D1SpecialPointMatrixTrace);
    }

    [ContextMenu("D1 DEBUG: Force Special Point Mineral")]
    private void DebugForceD1SpecialPointMineral()
    {
        DebugForceSpecificD1SpecialPoint(Dimension1System.D1SpecialPointMineralDeposit);
    }

    [ContextMenu("D1 DEBUG: Force Special Point Duration")]
    private void DebugForceD1SpecialPointDuration()
    {
        DebugForceSpecificD1SpecialPoint(Dimension1System.D1SpecialPointUnstableReading);
    }

    private void DebugForceSpecificD1SpecialPoint(string specialPointId)
    {
        EnsureDimension1State();

        bool assigned =
            Dimension1System.TryForceD1SpecialPointOnFirstCompatibleScannedDestination(
                this,
                specialPointId
            );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Special Point] Forzar " +
            Dimension1System.GetD1SpecialPointVisualName(specialPointId) +
            " => " +
            assigned
        );

        DebugPrintD1SpecialPoints();
    }

    [ContextMenu("D1 DEBUG: Print Special Points")]
    private void DebugPrintD1SpecialPoints()
    {
        EnsureDimension1State();

        if (dimension1ScannedDestinations == null ||
            dimension1ScannedDestinations.Count == 0)
        {
            Debug.Log("[D1 Special Point] No hay destinos escaneados.");
            return;
        }

        foreach (D1ScannedDestinationState destination in dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available)
                continue;

            Debug.Log(
                "[D1 Special Point] Destino: " +
                destination.destinationId +
                " | Punto: " +
                (
                    string.IsNullOrEmpty(destination.specialPointId)
                        ? "Ninguno"
                        : Dimension1System.GetD1SpecialPointVisualName(destination.specialPointId)
                )
            );
        }
    }

    private void DebugPrintD1ExplorationBonusLine(
        string destinationId,
        D1ShipState ship
    )
    {
        double duration =
            Dimension1System.GetSimpleExplorationDurationPreviewSeconds(
                this,
                destinationId,
                ship
            );

        float fragmentChance =
            Dimension1System.GetSimpleBlueprintFragmentChance(
                this,
                destinationId,
                ship
            );

        float specificMatrixChance =
            Dimension1System.GetSpecificBlueprintChancePreview(
                this,
                destinationId,
                ship
            );

        float relicChance =
            Dimension1System.GetExplorationRelicChancePreview(
                this,
                destinationId,
                ship
            );

        float unstableZoneReduction = 0.0f;

        if (destinationId == Dimension1System.DestinationUnstableZone)
        {
            unstableZoneReduction =
                Dimension1System.GetD1TreeUnstableZoneDurationReduction(this);
        }

        Debug.Log(
            "[D1 Bonuses] " +
            GetD1DebugDestinationName(destinationId) +
            " | Nave: " +
            GetD1DebugShipName(ship.shipId) +
            " | Duración: " +
            duration.ToString("0.0") +
            "s | Fragmento adaptativo: " +
            (fragmentChance * 100f).ToString("0.#") +
            "% | Matriz específica: " +
            (specificMatrixChance * 100f).ToString("0.#") +
            "% | Reliquia: " +
            (relicChance * 100f).ToString("0.#") +
            "% | Estabilización zona: -" +
            (unstableZoneReduction * 100f).ToString("0.#") +
            "%"
        );
    }

    private D1ShipState GetFirstUnlockedD1ShipForDebug()
    {
        if (dimension1Ships == null)
            return null;

        foreach (D1ShipState ship in dimension1Ships)
        {
            if (ship == null)
                continue;

            if (ship.unlocked)
                return ship;
        }

        return null;
    }

    private string GetD1DebugShipName(string shipId)
    {
        switch (shipId)
        {
            case Dimension1System.ShipLightProbe:
                return "Sonda Ligera";

            case Dimension1System.ShipExtractorDrone:
                return "Dron Extractor";

            case Dimension1System.ShipAnalyticProbe:
                return "Sonda Analítica";

            case Dimension1System.ShipCargoShip:
                return "Nave de Carga";

            case Dimension1System.ShipRescueShip:
                return "Nave de Rescate";

            case Dimension1System.ShipConvergenceShip:
                return "Nave de Convergencia";

            default:
                return shipId;
        }
    }

    private string GetD1DebugDestinationName(string destinationId)
    {
        switch (destinationId)
        {
            case Dimension1System.DestinationMineralBelt:
                return "Cinturón Mineral";

            case Dimension1System.DestinationShipGraveyard:
                return "Cementerio de Naves";

            case Dimension1System.DestinationAbandonedShip:
                return "Nave Abandonada";

            case Dimension1System.DestinationOrbitalRuin:
                return "Ruina Orbital";

            case Dimension1System.DestinationDriftingProbes:
                return "Sondas a la Deriva";

            case Dimension1System.DestinationLaboratory:
                return "Laboratorio";

            case Dimension1System.DestinationAbandonedStation:
                return "Estación Abandonada";

            case Dimension1System.DestinationMinorAnomaly:
                return "Anomalía Menor";

            case Dimension1System.DestinationAncientStructure:
                return "Estructura Antigua";

            case Dimension1System.DestinationUnstableZone:
                return "Zona Inestable";

            default:
                return destinationId;
        }
    }

    [ContextMenu("D1 DEBUG: Force Duplicate Drift Compass Relic")]
    private void DebugForceDuplicateD1DriftCompassRelic()
    {
        EnsureDimension1State();

        string relicId = Dimension1System.RelicDriftCompass;
        string metalId = Dimension1System.MetalIron;
        double amount = Dimension1System.GetD1DuplicateRelicConversionPreviewAmount(this);

        UnlockD1Relic(relicId);
        AddD1Metal(metalId, amount);

        dimension1LastExplorationRelics = new List<D1RelicRewardEntry>
    {
        new D1RelicRewardEntry
        {
            relicId = relicId,
            wasDuplicate = true,
            duplicateMetalId = metalId,
            duplicateMetalAmount = amount
        }
    };

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[D1 Relic] Reliquia repetida forzada: Brújula de Deriva => +" +
            amount.ToString("0") +
            " Hierro | Registro de Copias Tier: " +
            GetD1TreeNodeTier(Dimension1System.D1TreeRecoveryCopyRegistry) +
            " | Bonus: +" +
            (Dimension1System.GetD1TreeDuplicateRelicConversionBonus(this) * 100f).ToString("0.#") +
            "%"
        );
    }

    private void TryBuyD1TreeNodeForDebug(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId))
            return;

        if (GetD1TreeNodeTier(nodeId) > 0)
            return;

        bool bought = Dimension1System.TryBuyDimension1TreeNode(this, nodeId);

        Debug.Log(
            "[D1 Tree] Debug prerequisito " +
            nodeId +
            " => " +
            bought +
            " | Tier: " +
            GetD1TreeNodeTier(nodeId) +
            " | Puntos restantes: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Buy D1 Tree First Nodes")]
    private void DebugBuyD1TreeFirstNodes()
    {
        EnsureDimension1State();

        string[] nodesToBuy =
        {
        Dimension1System.D1TreeExplorationDestinationReading,
        Dimension1System.D1TreeFleetHangarPreparation,
        Dimension1System.D1TreeRecoveryCopyRegistry,
        Dimension1System.D1TreeConvergenceSpecialDestinationReading
    };

        foreach (string nodeId in nodesToBuy)
        {
            if (GetD1TreeNodeTier(nodeId) > 0)
            {
                Debug.Log(
                    "[D1 Tree] Nodo inicial ya comprado: " +
                    nodeId +
                    " | Tier: " +
                    GetD1TreeNodeTier(nodeId) +
                    " | Puntos restantes: " +
                    prestige1Points
                );

                continue;
            }

            bool bought = Dimension1System.TryBuyDimension1TreeNode(this, nodeId);

            Debug.Log(
                "[D1 Tree] Comprar nodo inicial: " +
                nodeId +
                " => " +
                bought +
                " | Tier: " +
                GetD1TreeNodeTier(nodeId) +
                " | Puntos restantes: " +
                prestige1Points
            );
        }

        if (SaveService.I != null)
            SaveService.I.Save();
    }

    [ContextMenu("P1 DEBUG: Preview Prestige 1 Points")]
    private void DebugPreviewPrestige1Points()
    {
        EnsureDimension1State();
        ActualizarMaxLE();

        int basePoints = Dimension1System.CalculatePrestige1PointsFromBaseGame(this);
        int planetPoints = Dimension1System.CalculatePrestige1PointsFromD1Planets(this);
        int shipPoints = Dimension1System.CalculatePrestige1PointsFromD1Ships(this);
        int relicPoints = Dimension1System.CalculatePrestige1PointsFromD1Relics(this);
        int treePoints = Dimension1System.CalculatePrestige1PointsFromD1Tree(this);
        int scannerPoints = Dimension1System.CalculatePrestige1PointsFromD1Scanner(this);
        int d1Points = Dimension1System.CalculatePrestige1PointsFromDimension1(this);
        int totalPreview = Dimension1System.CalculatePrestige1PointsPreview(this);

        Debug.Log(
            "[P1 Preview] Base: " +
            basePoints +
            " | D1: " +
            d1Points +
            "/" +
            Dimension1System.Dimension1Prestige1PreviewPointCap +
            " | Total preview: " +
            totalPreview +
            " | Puntos disponibles actuales: " +
            prestige1Points +
            "\n[D1 Breakdown] Planetas: " +
            planetPoints +
            " | Naves: " +
            shipPoints +
            " | Reliquias: " +
            relicPoints +
            " | Árbol: " +
            treePoints +
            " | Escáner: " +
            scannerPoints
        );
    }

    [ContextMenu("P1 DEBUG: Claim Prestige 1 Preview Points")]
    private void DebugClaimPrestige1PreviewPoints()
    {
        EnsureDimension1State();
        ActualizarMaxLE();

        int previewPoints = Dimension1System.CalculatePrestige1PointsPreview(this);
        int claimableBefore = Dimension1System.CalculateClaimablePrestige1Points(this);

        bool claimed = Dimension1System.TryClaimPrestige1PreviewPoints(
            this,
            out int claimedPoints
        );

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[P1 Claim] Resultado: " +
            claimed +
            " | Preview: " +
            previewPoints +
            " | Reclamables antes: " +
            claimableBefore +
            " | Reclamados ahora: " +
            claimedPoints +
            " | Mejor preview reclamado: " +
            prestige1BestClaimedPreviewPoints +
            " | Puntos disponibles: " +
            prestige1Points
        );
    }

    [ContextMenu("D1 DEBUG: Preview Tree Effect Values")]
    private void DebugPreviewD1TreeEffectValues()
    {
        EnsureDimension1State();

        Dimension1System.GetD1TreePartialRecoveryValues(
            this,
            out float partialRecoveryChance,
            out float partialRecoveryAmount
        );

        Debug.Log(
            "[D1 Tree Effects]" +
            "\nExploración:" +
            " DestinationReading=" +
            Dimension1System.HasD1TreeDestinationReading(this) +
            " | HiddenFindQuality=+" +
            (Dimension1System.GetD1TreeHiddenFindQualityBonus(this) * 100.0f).ToString("0.#") +
            "%" +
            " | ScanMemory=-" +
            (Dimension1System.GetD1TreeScanMemoryRepetitionReduction(this) * 100.0f).ToString("0.#") +
            "%" +
            "\nFlota:" +
            " SingleShipEfficiency=+" +
            (Dimension1System.GetD1TreeSingleShipEfficiencyBonus(this) * 100.0f).ToString("0.#") +
            "%" +
            " | RouteOptimization=-" +
            (Dimension1System.GetD1TreeRouteOptimizationDurationReduction(this) * 100.0f).ToString("0.#") +
            "%" +
            "\nRecuperación:" +
            " DuplicateRelicConversion=+" +
            (Dimension1System.GetD1TreeDuplicateRelicConversionBonus(this) * 100.0f).ToString("0.#") +
            "%" +
            " | MaterialRecoveryChance=" +
            (partialRecoveryChance * 100.0f).ToString("0.#") +
            "%" +
            " | MaterialRecoveryAmount=" +
            (partialRecoveryAmount * 100.0f).ToString("0.#") +
            "%" +
            " | BlueprintPriority=+" +
            (Dimension1System.GetD1TreeBlueprintPriorityBonus(this) * 100.0f).ToString("0.#") +
            "%" +
            "\nLecturas especiales:" +
            " | SpecialDestinationDetection=+" +
            (Dimension1System.GetD1TreeSpecialDestinationDetectionBonus(this) * 100.0f).ToString("0.#") +
            "%" +
            " | UnstableDuration=-" +
            (Dimension1System.GetD1TreeUnstableZoneDurationReduction(this) * 100.0f).ToString("0.#") +
            "%" +
            " | UnstableRareProtection=+" +
            (Dimension1System.GetD1TreeUnstableZoneRareRewardProtection(this) * 100.0f).ToString("0.#") +
            "%"
        );
    }

    [ContextMenu("D1 DEBUG: Max Mining Relics")]
    private void DebugMaxD1MiningRelics()
    {
        EnsureDimension1State();

        SetD1RelicLevel(Dimension1System.RelicAncientDrill, Dimension1System.Dimension1RelicMaxLevel);

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log("[D1] DEBUG: Reliquias de mineria base al nivel maximo.");
    }

    [ContextMenu("D1 DEBUG: Preview Specific Matrix Chances")]
    private void DebugPreviewSpecificMatrixChances()
    {
        EnsureDimension1State();

        Debug.Log("[D1 Specific Matrix Chances]");

        Debug.Log(
            "[D1 Specific Matrix Chances] Sonda Ligera / Cementerio: " +
            GetSpecificMatrixChanceDebugText(
                Dimension1System.ShipLightProbe,
                Dimension1System.DestinationShipGraveyard
            )
        );

        Debug.Log(
            "[D1 Specific Matrix Chances] Sonda Analítica / Laboratorio: " +
            GetSpecificMatrixChanceDebugText(
                Dimension1System.ShipAnalyticProbe,
                Dimension1System.DestinationLaboratory
            )
        );

        Debug.Log(
            "[D1 Specific Matrix Chances] Nave de Carga / Nave Abandonada: " +
            GetSpecificMatrixChanceDebugText(
                Dimension1System.ShipCargoShip,
                Dimension1System.DestinationAbandonedShip
            )
        );

        Debug.Log(
            "[D1 Specific Matrix Chances] Nave de Rescate / Estacion Abandonada: " +
            GetSpecificMatrixChanceDebugText(
                Dimension1System.ShipRescueShip,
                Dimension1System.DestinationAbandonedStation
            )
        );

        Debug.Log(
            "[D1 Specific Matrix Chances] Nave de Convergencia / Zona Inestable: " +
            GetSpecificMatrixChanceDebugText(
                Dimension1System.ShipConvergenceShip,
                Dimension1System.DestinationUnstableZone
            )
        );

    }

    private string GetSpecificMatrixChanceDebugText(string shipId, string destinationId)
    {
        D1ShipState ship = null;

        if (dimension1Ships != null)
        {
            foreach (D1ShipState candidate in dimension1Ships)
            {
                if (candidate != null && candidate.shipId == shipId)
                {
                    ship = candidate;
                    break;
                }
            }
        }

        float chance = Dimension1System.GetSpecificBlueprintChancePreview(
            this,
            destinationId,
            ship
        );

        int sensorsLevel = ship != null ? ship.sensorsLevel : 0;

        return
            "Sensores " +
            sensorsLevel +
            " | Chance " +
            (chance * 100.0f).ToString("0.##") +
            "%";
    }

#endif


    /// <summary>
    /// Avanza el juego dt segundos (lógica principal de producción).
    /// </summary>
    public void Tick(double dt)
{
    // 1) Producir EM...
    double emPs = CalculateEMps();
    if (emPs > 0.0)
    {
        EM += emPs * dt;

    }

    // 2) Actualizar el multiplicador EM
    emMult = CalculateEMMultiplier();

    // 3) Producir LE
    //    - CalculateTotalLEps() se sigue usando para HUD y lógica de desbloqueos.
    //    - PERO ya NO sumamos LE usando esa fórmula directamente.
    double totalLEps = CalculateTotalLEps(); // <-- solo informativo / HUD

    GenerateLEFromBaseAndBuildings(dt);
    GenerateExperimentalFragments(dt);
    UpdateChronalSeeds(dt);

    // Dimensión 1: minería planetaria y exploración por segundo.
    Dimension1System.Tick(this, dt);

    // 🔹 F7.3: Producir ADP
    double adpPs = CalculateADPps();
    if (adpPs > 0.0)
    {
        ADP += adpPs * dt;
        totalADPGenerada += adpPs * dt;
    }

    // 🔹 F7.4: WHF (Wormhole Fragments)
    double whfPs = CalculateWHFps();
    if (whfPs > 0.0)
    {
        WHF += whfPs * dt;
        totalWHFGenerada += whfPs * dt;
    }


    // 5) Calibración del Modulador de Fase
    UpdatePhaseModulatorCalibration(dt);

    // 6) Consumo online de la reserva de Persistencia
    UpdateTrianglePersistenceReserveConsumption(dt);

    // 7) Lógica vieja de Persistencia desactivada por migración
    // UpdateTrianglePersistenceMaturation(dt);

    // F6.1: registrar el máximo LE alcanzado
    ActualizarMaxLE();
}




    // F6.1: Actualiza el máximo LE alcanzado en este run
    public void ActualizarMaxLE()
    {
        if (LE > maxLEAlcanzado)
        {
            maxLEAlcanzado = LE;
        }
    }

    public bool IsPhaseModulatorOwned()
        {
            return GetBuildingLevel("fluctuation_antenna") > 0;
        }

        public bool IsAttunementUnlocked()
        {
            return hasDonePrestige1 || prestige1Count > 0;
        }

    public void SetPhaseModulatorMode(PhaseModulatorMode newMode)
        {
            if (!IsPhaseModulatorOwned())
                return;

            if (newMode == PhaseModulatorMode.Attunement && !IsAttunementUnlocked())
                return;

            if (phaseModulatorMode != newMode)
            {
                phaseModulatorMode = newMode;
                phaseModulatorCalibration = 0f;
            }
        }

        public double GetPhaseModulatorEffectivenessMultiplier()
        {
            if (!IsPhaseModulatorOwned())
                return 1.0;

            double multiplier = 1.0;

            multiplier *= GetTriangleSynergyModulatorMultiplier();

            return multiplier;
        }
    
        public float GetPhaseModulatorExpansionTickBonus()
        {
            if (!IsPhaseModulatorOwned())
                return 0f;

            if (phaseModulatorMode != PhaseModulatorMode.Expansion)
                return 0f;

            double effectiveness = GetPhaseModulatorEffectivenessMultiplier();
            return (float)(phaseModulatorExpansionMaxTickSpeedBonus * phaseModulatorCalibration * effectiveness);
        }

        public float GetPhaseModulatorConservationDiscount()
        {
            if (!IsPhaseModulatorOwned())
                return 0f;

            if (phaseModulatorMode != PhaseModulatorMode.Conservation)
                return 0f;

            double effectiveness = GetPhaseModulatorEffectivenessMultiplier();
            return (float)(phaseModulatorConservationMaxCostReduction * phaseModulatorCalibration * effectiveness);
        }

    public double GetEffectiveBuildingCost(BuildingState building)
    {
        if (building == null || building.def == null)
            return 0.0;

        double effectiveCost = building.currentCost;

        bool affectedByConservation =
            building.def.id == "vacuum_observer" ||
            building.def.id == "casimir_panel";

        if (affectedByConservation)
        {
            float conservationDiscount = GetPhaseModulatorConservationDiscount();
            if (conservationDiscount > 0f)
            {
                effectiveCost *= (1.0 - conservationDiscount);
            }
        }

        if (effectiveCost < 0.0)
            effectiveCost = 0.0;

        return effectiveCost;
    }

    public double GetMachineArtifactProductionMultiplier(string buildingId)
    {
        if (string.IsNullOrWhiteSpace(buildingId))
            return 1.0;

        bool isRoom1Artifact =
            buildingId == "vacuum_observer" ||
            buildingId == "casimir_panel";

        if (!isRoom1Artifact)
            return 1.0;

        double factor = 1.0;

        if (MachineManager.I != null)
        {
            factor += MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.ArtifactBonus);
        }

        return factor;
    }

    private bool IsRoom1LEArtifactBuilding(string buildingId)
    {
        return
            buildingId == "vacuum_observer" ||
            buildingId == "casimir_panel";
    }

    private double GetRoom1EchoGlobalLEMultiplier()
    {
        return 1.0 + Dimension1System.GetRoom1EchoGlobalLEBonus(this);
    }

    private double GetRoom1EchoArtifactLEMultiplier(string buildingId)
    {
        if (!IsRoom1LEArtifactBuilding(buildingId))
            return 1.0;

        return 1.0 + Dimension1System.GetRoom1EchoArtifactLEBonus(this);
    }

    public double GetMachineTriangleBonusMultiplier()
    {
        double factor = 1.0;

        if (MachineManager.I != null)
        {
            factor += MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.TriangleBonus);
        }

        return factor;
    }

    public double GetMachineRoom1GlobalMultiplier()
    {
        double factor = 1.0;

        if (MachineManager.I != null)
        {
            factor += MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.Room1GlobalBonus);
            factor += MachineManager.I.GetZoneProgressSyncBonus(MachineZoneType.Room1Link);
        }

        return factor;
    }

        public bool IsTriangleSystemActive()
    {
        if (!triangleSystemUnlocked)
            return false;

        if (string.IsNullOrEmpty(trianglePrimaryBuildingId))
            return false;

        if (string.IsNullOrEmpty(triangleReinforcementBuildingId))
            return false;

        if (string.IsNullOrEmpty(triangleAlterationBuildingId))
            return false;

        // No permitir duplicados
        if (trianglePrimaryBuildingId == triangleReinforcementBuildingId)
            return false;

        if (trianglePrimaryBuildingId == triangleAlterationBuildingId)
            return false;

        if (triangleReinforcementBuildingId == triangleAlterationBuildingId)
            return false;

        return true;
    }

        public bool IsTriangleFullyConfiguredWithBaseArtifacts()
    {
        if (!IsTriangleSystemActive())
            return false;

        bool hasHiggs = false;
        bool hasTetra = false;
        bool hasModulator = false;

        string[] ids =
        {
            trianglePrimaryBuildingId,
            triangleReinforcementBuildingId,
            triangleAlterationBuildingId
        };

        foreach (string id in ids)
        {
            if (id == "vacuum_observer")
                hasHiggs = true;
            else if (id == "casimir_panel")
                hasTetra = true;
            else if (id == "fluctuation_antenna")
                hasModulator = true;
        }

        return hasHiggs && hasTetra && hasModulator;
    }

    public TriangleSlotRole GetPhaseModulatorTrianglePosition()
    {
        if (!IsTriangleFullyConfiguredWithBaseArtifacts())
            return TriangleSlotRole.None;

        if (trianglePrimaryBuildingId == "fluctuation_antenna")
            return TriangleSlotRole.Primary;

        if (triangleReinforcementBuildingId == "fluctuation_antenna")
            return TriangleSlotRole.Reinforcement;

        if (triangleAlterationBuildingId == "fluctuation_antenna")
            return TriangleSlotRole.Alteration;

        return TriangleSlotRole.None;
    }

    public TriangleProtocolType GetActiveTriangleProtocol()
    {
        TriangleSlotRole modulatorPosition = GetPhaseModulatorTrianglePosition();

        switch (modulatorPosition)
        {
            case TriangleSlotRole.Primary:
                return TriangleProtocolType.Impulso;

            case TriangleSlotRole.Reinforcement:
                return TriangleProtocolType.Sinergia;

            case TriangleSlotRole.Alteration:
                return TriangleProtocolType.Persistencia;

            default:
                return TriangleProtocolType.None;
        }
    }

    public string GetActiveTriangleProtocolId()
    {
        TriangleProtocolType protocol = GetActiveTriangleProtocol();

        switch (protocol)
        {
            case TriangleProtocolType.Impulso:
                return "impulso";

            case TriangleProtocolType.Sinergia:
                return "sinergia";

            case TriangleProtocolType.Persistencia:
                return "persistencia";

            default:
                return "none";
        }
    }

    public double GetTriangleProtocolBaseMultiplier()
    {
        TriangleProtocolType protocol = GetActiveTriangleProtocol();

        switch (protocol)
        {
            case TriangleProtocolType.Impulso:
                return 1.12; // +12% LE

            case TriangleProtocolType.Sinergia:
                return 1.10; // base provisional para Higgs/Tetra

            case TriangleProtocolType.Persistencia:
                return 1.18; // base provisional del modulador al 100%

            default:
                return 1.0;
        }
    }

        public double GetTriangleImpulseLEMultiplier()
    {
        if (!IsTriangleSystemActive()) return 1.0;
        if (GetActiveTriangleProtocol() != TriangleProtocolType.Impulso) return 1.0;

        int tier = 0;
        if (F2UpgradeManager.I != null)
            tier = F2UpgradeManager.I.GetTriangleImpulseTuningTier();

        double baseMultiplier;

        switch (tier)
        {
            case 1:
                baseMultiplier = 1.16;
                break;

            case 2:
                baseMultiplier = 1.20;
                break;

            default:
                baseMultiplier = 1.12;
                break;
        }

        return 1.0 + ((baseMultiplier - 1.0) * GetMachineTriangleBonusMultiplier());
    }

    public double GetTriangleSynergyBuildingMultiplier(string buildingId)
    {
        if (!IsTriangleSystemActive()) return 1.0;
        if (GetActiveTriangleProtocol() != TriangleProtocolType.Sinergia) return 1.0;

        int tier = 0;
        if (F2UpgradeManager.I != null)
            tier = F2UpgradeManager.I.GetTriangleSynergyResonanceTier();

        double value = 1.0;

        switch (tier)
        {
            case 1:
                if (buildingId == "vacuum_observer") value = 1.13;
                else if (buildingId == "casimir_panel") value = 1.13;
                break;

            case 2:
                if (buildingId == "vacuum_observer") value = 1.15;
                else if (buildingId == "casimir_panel") value = 1.15;
                break;

            default:
                if (buildingId == "vacuum_observer") value = 1.10;
                else if (buildingId == "casimir_panel") value = 1.10;
                break;
        }

        return 1.0 + ((value - 1.0) * GetMachineTriangleBonusMultiplier());   
    }

    public double GetTriangleSynergyModulatorMultiplier()
    {
        if (!IsTriangleSystemActive()) return 1.0;
        if (GetActiveTriangleProtocol() != TriangleProtocolType.Sinergia) return 1.0;

        int tier = 0;
        if (F2UpgradeManager.I != null)
            tier = F2UpgradeManager.I.GetTriangleSynergyResonanceTier();

        double baseMultiplier;

        switch (tier)
        {
            case 1:
                baseMultiplier = 1.08;
                break;

            case 2:
                baseMultiplier = 1.10;
                break;

            default:
                baseMultiplier = 1.06;
                break;
        }

        return 1.0 + ((baseMultiplier - 1.0) * GetMachineTriangleBonusMultiplier());
    }


    public double GetTrianglePersistenceReserveMaxSeconds()
    {
        int tier = 0;
        if (F2UpgradeManager.I != null)
            tier = F2UpgradeManager.I.GetTrianglePersistenceAnchorTier();

        switch (tier)
        {
            case 2:
                return 14400.0; // 4 horas
            default:
                return trianglePersistenceReserveBaseMaxSeconds; // 3 horas
        }
    }

    public double GetTrianglePersistenceOfflineSecondsPerReserveHour()
    {
        int tier = 0;
        if (F2UpgradeManager.I != null)
            tier = F2UpgradeManager.I.GetTrianglePersistenceAnchorTier();

        switch (tier)
        {
            case 1:
            case 2:
                return 12600.0; // 3h30m offline = 1h reserva
            default:
                return trianglePersistenceOfflineSecondsPerReserveHour; // 4h offline = 1h reserva
        }
    }

    public bool HasTrianglePersistenceReserveActive()
    {
        return trianglePersistenceReserveSeconds > 0.0;
    }

    public double GetTrianglePersistenceReserveBuildingMultiplier(string buildingId)
    {
        if (!HasTrianglePersistenceReserveActive())
            return 1.0;

        double baseMultiplier = 1.0;

        if (buildingId == "vacuum_observer")
            baseMultiplier = trianglePersistenceBuffHiggsMultiplier;

        if (buildingId == "casimir_panel")
            baseMultiplier = trianglePersistenceBuffTetraMultiplier;

        return 1.0 + ((baseMultiplier - 1.0) * GetMachineTriangleBonusMultiplier());
    }

    public void ApplyOfflineTrianglePersistenceReserve(double offlineSeconds)
    {
        if (offlineSeconds <= 0.0) return;
        if (!IsTriangleFullyConfiguredWithBaseArtifacts()) return;
        if (GetActiveTriangleProtocol() != TriangleProtocolType.Persistencia) return;

        double secondsPerReserveHour = GetTrianglePersistenceOfflineSecondsPerReserveHour();
        if (secondsPerReserveHour <= 0.0) return;

        double reserveSecondsToAdd = (offlineSeconds / secondsPerReserveHour) * 3600.0;
        double maxReserve = GetTrianglePersistenceReserveMaxSeconds();
        

        trianglePersistenceReserveSeconds += reserveSecondsToAdd;

        if (trianglePersistenceReserveSeconds > maxReserve)
            trianglePersistenceReserveSeconds = maxReserve;
    }

    private void UpdateTrianglePersistenceReserveConsumption(double dt)
    {
        if (dt <= 0.0) return;
        if (trianglePersistenceReserveSeconds <= 0.0) return;

        trianglePersistenceReserveSeconds -= dt;

        if (trianglePersistenceReserveSeconds < 0.0)
            trianglePersistenceReserveSeconds = 0.0;
    }

    public bool IsValidTriangleBuildingId(string buildingId)
    {
        return buildingId == "vacuum_observer"
            || buildingId == "casimir_panel"
            || buildingId == "fluctuation_antenna";
    }

    public bool IsTriangleSlotFilled(TriangleSlotRole role)
    {
        switch (role)
        {
            case TriangleSlotRole.Primary:
                return !string.IsNullOrEmpty(trianglePrimaryBuildingId);

            case TriangleSlotRole.Reinforcement:
                return !string.IsNullOrEmpty(triangleReinforcementBuildingId);

            case TriangleSlotRole.Alteration:
                return !string.IsNullOrEmpty(triangleAlterationBuildingId);

            default:
                return false;
        }
    }

    public string GetTriangleBuildingId(TriangleSlotRole role)
    {
        switch (role)
        {
            case TriangleSlotRole.Primary:
                return trianglePrimaryBuildingId;

            case TriangleSlotRole.Reinforcement:
                return triangleReinforcementBuildingId;

            case TriangleSlotRole.Alteration:
                return triangleAlterationBuildingId;

            default:
                return "";
        }
    }

    private void SetTriangleBuildingId(TriangleSlotRole role, string buildingId)
    {
        switch (role)
        {
            case TriangleSlotRole.Primary:
                trianglePrimaryBuildingId = buildingId;
                break;

            case TriangleSlotRole.Reinforcement:
                triangleReinforcementBuildingId = buildingId;
                break;

            case TriangleSlotRole.Alteration:
                triangleAlterationBuildingId = buildingId;
                break;
        }
    }

    public void ClearTriangleSlot(TriangleSlotRole role)
    {
        switch (role)
        {
            case TriangleSlotRole.Primary:
                trianglePrimaryBuildingId = "";
                break;

            case TriangleSlotRole.Reinforcement:
                triangleReinforcementBuildingId = "";
                break;

            case TriangleSlotRole.Alteration:
                triangleAlterationBuildingId = "";
                break;
        }
    }

    public void ClearTriangleConfiguration()
    {
        trianglePrimaryBuildingId = "";
        triangleReinforcementBuildingId = "";
        triangleAlterationBuildingId = "";
    }

    public bool AssignTriangleBuilding(TriangleSlotRole role, string buildingId)
    {
        if (!triangleSystemUnlocked)
            return false;

        if (!IsValidTriangleBuildingId(buildingId))
            return false;

        // Slot destino actual
        string targetCurrent = GetTriangleBuildingId(role);

        // Detectar en qué slot está actualmente ese artefacto
        TriangleSlotRole? sourceRole = null;

        if (trianglePrimaryBuildingId == buildingId)
            sourceRole = TriangleSlotRole.Primary;
        else if (triangleReinforcementBuildingId == buildingId)
            sourceRole = TriangleSlotRole.Reinforcement;
        else if (triangleAlterationBuildingId == buildingId)
            sourceRole = TriangleSlotRole.Alteration;

        // Si ya está en el mismo slot, no hacemos nada
        if (sourceRole.HasValue && sourceRole.Value == role)
            return true;

        // Caso 1: el artefacto viene de otro slot del triángulo
        // -> hacemos intercambio (swap) con lo que haya en el slot destino
        if (sourceRole.HasValue)
        {
            SetTriangleBuildingId(sourceRole.Value, targetCurrent);
            SetTriangleBuildingId(role, buildingId);
            return true;
        }

        // Caso 2: viene del drawer / fuera del triángulo
        // -> solo asignamos al slot destino
        // si el destino ya tenía algo, ese artefacto "vuelve al drawer"
        SetTriangleBuildingId(role, buildingId);
        return true;
    }

    public void SanitizeTriangleConfiguration()
    {
        if (!IsValidTriangleBuildingId(trianglePrimaryBuildingId))
            trianglePrimaryBuildingId = "";

        if (!IsValidTriangleBuildingId(triangleReinforcementBuildingId))
            triangleReinforcementBuildingId = "";

        if (!IsValidTriangleBuildingId(triangleAlterationBuildingId))
            triangleAlterationBuildingId = "";

        // Evitar duplicados después de cargar
        if (!string.IsNullOrEmpty(trianglePrimaryBuildingId) &&
            trianglePrimaryBuildingId == triangleReinforcementBuildingId)
        {
            triangleReinforcementBuildingId = "";
        }

        if (!string.IsNullOrEmpty(trianglePrimaryBuildingId) &&
            trianglePrimaryBuildingId == triangleAlterationBuildingId)
        {
            triangleAlterationBuildingId = "";
        }

        if (!string.IsNullOrEmpty(triangleReinforcementBuildingId) &&
            triangleReinforcementBuildingId == triangleAlterationBuildingId)
        {
            triangleAlterationBuildingId = "";
        }
    }

        private void UpdatePhaseModulatorCalibration(double dt)
    {
        if (!IsPhaseModulatorOwned())
        {
            phaseModulatorMode = PhaseModulatorMode.None;
            phaseModulatorCalibration = 0f;
            return;
        }

        if (phaseModulatorMode == PhaseModulatorMode.None)
        {
            phaseModulatorCalibration = 0f;
            return;
        }

        phaseModulatorCalibration += phaseModulatorCalibrationPerSecond * (float)dt;
        if (phaseModulatorCalibration > 1f)
            phaseModulatorCalibration = 1f;
    }

    public void DebugResetRunState()
    {
    // 🔹 Recursos básicos
    LE = 10.0;
    VP = 0.0;
        
    // 🔹 Reset de recursos secundarios
    EM = 0.0;
    emMult = 0.0;
    
    // 🔹 Producción base sin edificios
    baseLEps = 0.0;

    // 🔹 Reset del máximo de LE alcanzado en el run
    maxLEAlcanzado = 0.0;   
 
    // 🔹 Reset de edificios (nivel 0)
    if (buildingStates != null)
    {
        foreach (var b in buildingStates)
        {
            if (b == null) continue;
            b.ResetForPrestige();   // este método ya existe en BuildingState
        }
    }

    // Si más adelante quieres, aquí también puedes resetear BEC, EM, ADP, WHF, etc.
    }


    /// <summary>
    /// Multiplicador para la generación de EM (ej: +15% si el upgrade está comprado).
    /// </summary>
    public double GetMetaEMGenerationMultiplier()
    {
        double mult = 1.0;

        if (metaEmBoost1Bought)
        {
            mult *= 1.15; // +15% EM
        }

        return mult;
    }


    /// <summary>
    /// Calcula la producción total de LE/s:
    /// - producción base
    /// - producción de edificios
    /// - bonus globales
    /// (Por ahora SIN decoherencia).
    /// </summary>
    private double CalculateTotalLEps()
{
    double baseProd = baseLEps;
    double fromBuildings = 0.0;
    double multiplier = 1.0;
    double flatBonus = 0.0;

    foreach (var b in buildingStates)
    {
        if (b == null || b.def == null) continue;

        double buildingProd = b.GetLEps();

        // Sinergia del triángulo solo afecta Higgs y Tetra
        buildingProd *= GetTriangleSynergyBuildingMultiplier(b.def.id);

        // Persistencia nueva por reserva activa
        buildingProd *= GetTrianglePersistenceReserveBuildingMultiplier(b.def.id);

        // Máquina / Zona 1: Calibración de Artefactos
        buildingProd *= GetMachineArtifactProductionMultiplier(b.def.id);
        buildingProd *= GetRoom1EchoArtifactLEMultiplier(b.def.id);

        fromBuildings += buildingProd;

        if (b.level <= 0) continue;

        switch (b.def.bonusType)
        {
            case BuildingBonusType.None:
                break;

            case BuildingBonusType.MultiplierLE:
                multiplier += b.def.bonusPerLevel * b.level;
                break;

            case BuildingBonusType.FlatLE:
                flatBonus += b.def.bonusPerLevel * b.level;
                break;
        }
    }

    // EM
    double emFactor = 1.0 + emMult;

    // Research (lo que ya tienes)
    double researchFactor = researchGlobalLEMult;

    // 🔥 Achievements
    double achFactor = 1.0;
    if (AchievementManager.I != null)
    {
        achFactor = AchievementManager.I.GetGlobalLEFactor();
    }
    
    // 🔥 F6.4: factor de prestigio
    double prestigeFactor = 1.0;

    double f2UpgradeFactor = 1.0;
    if (F2UpgradeManager.I != null)
    {
        f2UpgradeFactor += F2UpgradeManager.I.GetTotalGlobalLEMultBonus();
    }

    double triangleImpulseFactor = GetTriangleImpulseLEMultiplier();

    double machineLEFactor = 1.0;
    if (MachineManager.I != null)
    {
        machineLEFactor += MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.GlobalLEBonus);
    }

    double room1GlobalFactor = GetMachineRoom1GlobalMultiplier();
    double room1EchoGlobalFactor = GetRoom1EchoGlobalLEMultiplier();

    double rawTotalBeforeRoom1Echo = ((baseProd + fromBuildings) * triangleImpulseFactor)
                    * multiplier
                    * emFactor
                    * researchFactor
                    * achFactor
                    * prestigeFactor
                    * f2UpgradeFactor
                    * machineLEFactor
                    * room1GlobalFactor
                    + flatBonus;

    double rawTotal = rawTotalBeforeRoom1Echo * room1EchoGlobalFactor;

    return rawTotal;
    }


    /// <summary>
    /// Calcula cuánta EM/s generan los edificios relacionados con EM.
    /// </summary>
    
    private double CalculateEMps()
{
    double emPs = 0.0;

    foreach (var b in buildingStates)
    {
        if (b == null || b.def == null) continue;
        if (b.level <= 0) continue;

        switch (b.def.id)
        {
            case "em_field_emitter":
                emPs += 0.5 * b.level;
                break;

            case "em_field_array":
                emPs += 1.0 * b.level;
                break;

            case "micro_collider":
                emPs += 2.0 * b.level;
                break;
        }
    }

    // Aplicar multiplicador de investigaciones (Cosecha EM I/II)
    if (ResearchManager.I != null)
    {
        emPs *= ResearchManager.I.GetEMGenerationFactor();
    }

    // F7.5: meta-upgrades de Λ que afectan la generación de EM
    emPs *= GetMetaEMGenerationMultiplier();

    return emPs;
    }

    /// <summary>
    /// F7.4: Calcula cuántos fragmentos de Wormhole (WHF) se generan por segundo.
    /// Depende de la cantidad de 'wormhole_generator' y de la ADP actual.
    /// </summary>
    private double CalculateWHFps()
    {
        double whfPs = 0.0;

        int wormholeGenerators = 0;

        foreach (var b in buildingStates)   // 👉 usa la misma colección que en CalculateTotalLEps
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue;

            switch (b.def.id)
            {
                case "wormhole_generator":
                    wormholeGenerators += b.level;  // o b.count si tu clase usa otro nombre
                    break;
            }
        }

        // Sin generadores o sin ADP práctica, no generamos nada
        if (wormholeGenerators <= 0 || ADP <= 0.0)
            return 0.0;

        // WHF base por generador: extremadamente bajo
        double basePerGenerator = 0.00001; // 1e-5 WHF/s por generador

        // Factor por ADP (a más ADP, más WHF)
        // Log10 suaviza para que no se dispare demasiado.
        double adpFactor = System.Math.Log10(1.0 + ADP);
        if (adpFactor <= 0.0) return 0.0;

        whfPs = wormholeGenerators * basePerGenerator * adpFactor;

        return whfPs;
    }


    /// <summary>
    /// F7.3: Calcula cuánta ADP/s se genera a partir de los edificios
    /// 'adp_reactor' y 'sc_matrix', usando EM como factor suave.
    /// </summary>
    private double CalculateADPps()
    {
        double adpPs = 0.0;

        int adpReactors = 0;
        int scMatrices = 0;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue;

            switch (b.def.id)
            {
                case "adp_reactor":
                    adpReactors += b.level;
                    break;

                case "sc_matrix":
                    scMatrices += b.level;
                    break;
            }
        }

        if (adpReactors <= 0)
            return 0.0;

        // ADP base por reactor (lo ajustamos luego en balance)
        double basePerReactor = 0.001; // ADP/s por reactor

        // Bonus por Matriz SC (ej: +10% ADP por cada una)
        double scBonusMult = 1.0 + 0.10 * scMatrices;

        // EM como factor suave
        double emFactor = 1.0 + System.Math.Log10(1.0 + EM);

        adpPs = adpReactors * basePerReactor * scBonusMult * emFactor;

        return adpPs;
    }


/// <summary>
/// Convierte el EM acumulado en un multiplicador suave de producción de LE.
/// </summary>
private double CalculateEMMultiplier()
{
    if (EM <= 0.0) return 0.0;

    // Cada 100 EM aporta ~5% extra, con rendimientos decrecientes (sqrt)
    double k = 0.15; // 5% base
    double normalized = EM / 100.0;

    return k * System.Math.Sqrt(normalized);
}


    /// <summary>
    /// Placeholder: por ahora no se usa.
    /// </summary>
    private double ApplyDecoherence(double rawLEps)
    {
        // Devuelve tal cual, sin cambios.
        return rawLEps;

        // Cuando queramos reusar esta mecánica, aquí se reactivará la lógica.
    }

    public void RegisterBuildingState(BuildingState state)
    {
        if (state == null) return;
        if (!buildingStates.Contains(state))
        {
            buildingStates.Add(state);
        }

        // 🆕 Si hay niveles cargados desde el save, aplicarlos
        if (SaveService.LastLoadedBuildingLevels != null &&
            SaveService.LastLoadedBuildingLevels.Count > 0)
        {
            ApplyBuildingLevelsFromSave(SaveService.LastLoadedBuildingLevels);
        }
    }


    /// <summary>
    /// Devuelve la producción total de LE por segundo.
    /// </summary>
    public double GetTotalLEps()
    {
        return CalculateTotalLEps();
    }

    public double CalculateTracesPs()
    {
        int casimirLevel = GetBuildingLevel("casimir_panel");
        if (casimirLevel <= 0)
            return 0.0;

        double tracesPerSecond = 0.03 * casimirLevel;

        // Sinergia del triángulo también mejora al Tetra en trazas
        tracesPerSecond *= GetTriangleSynergyBuildingMultiplier("casimir_panel");

        // Máquina / Zona 1: Calibración de Artefactos también afecta al Núcleo Tetraquark
        tracesPerSecond *= GetMachineArtifactProductionMultiplier("casimir_panel");

        if (F2UpgradeManager.I != null)
        {
            tracesPerSecond *= (1.0 + F2UpgradeManager.I.GetResidualAnalysisBonus());
        }

        double machineTracesFactor = 1.0;
        if (MachineManager.I != null)
        {
            machineTracesFactor += MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.TracesBonus);
        }

        tracesPerSecond *= machineTracesFactor;
        tracesPerSecond *= GetMachineRoom1GlobalMultiplier();

        return tracesPerSecond;
    }

    /// <summary>
    /// Devuelve el nivel actual de un edificio por id.
    /// </summary>
    public int GetBuildingLevel(string id)
    {
        if (string.IsNullOrEmpty(id)) return 0;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.def.id == id)
            {
                return b.level;
            }
        }

        return 0;
    }


    public bool CanDoPrestige1()
    {
        if (MachineManager.I == null)
            return false;

        return MachineManager.I.HasEnoughRepairForPrestige1()
            && MachineManager.I.Prestige1Prepared;
    }

    public string GetPrestige1StatusText()
    {
        if (MachineManager.I == null)
            return "Máquina no disponible.";

        double repairPct = MachineManager.I.GetTotalMachineRepairProgress01() * 100.0;

        if (!MachineManager.I.HasEnoughRepairForPrestige1())
            return $"Reparación de Máquina: {repairPct:0}% / 80%";

        if (!MachineManager.I.Prestige1Prepared)
            return "Falta activar el Canal de Convergencia.";

        int previewPoints = Dimension1System.CalculatePrestige1PointsPreview(this);
        int claimablePoints = Dimension1System.CalculateClaimablePrestige1Points(this);

        return
            "Prestigio 1 disponible." +
            "\nPuntos nuevos: +" +
            claimablePoints +
            " | Preview total: " +
            previewPoints +
            " | Disponibles: " +
            prestige1Points;
    }

    public bool DoPrestige1Reset()
    {
        if (!CanDoPrestige1())
        {
            Debug.Log("[GameState] Prestigio 1 no disponible todavía: " + GetPrestige1StatusText());
            return false;
        }

        EnsureDimension1State();
        ActualizarMaxLE();

        int previewBeforeReset = Dimension1System.CalculatePrestige1PointsPreview(this);
        int claimableBeforeReset = Dimension1System.CalculateClaimablePrestige1Points(this);

        bool claimed = Dimension1System.TryClaimPrestige1PreviewPoints(
            this,
            out int claimedPoints
        );

        prestige1Count++;
        hasDonePrestige1 = true;

        ResetGameBaseForPrestige1();

        // Al hacer Prestigio 1, se prepara el sistema de dimensiones.
        // Por ahora solo Dimensión 1 tiene contenido jugable.
        UnlockDimensionSystemAfterPrestige1();

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log(
            "[GameState] Prestigio 1 realizado. Total: " +
            prestige1Count +
            " | Preview antes del reset: " +
            previewBeforeReset +
            " | Reclamables antes: " +
            claimableBeforeReset +
            " | Reclamo ejecutado: " +
            claimed +
            " | Puntos ganados: " +
            claimedPoints +
            " | Puntos P1 disponibles: " +
            prestige1Points
        );

        return true;
    }

    private void ResetGameBaseForPrestige1()
    {
        // Recursos base
        LE = 10.0;
        Traces = 0.0;
        VP = 0.0;

        // Recursos avanzados / viejos de run
        BEC = 0.0;
        EM = 0.0;
        emMult = 0.0;
        ADP = 0.0;
        WHF = 0.0;
        baseLEps = 0.0;
        maxLEAlcanzado = 0.0;
        useDecoherence = false;

        // Modulador de Fase
        phaseModulatorMode = PhaseModulatorMode.None;
        phaseModulatorCalibration = 0f;

        // Triángulo
        triangleSystemUnlocked = false;
        trianglePrimaryBuildingId = "";
        triangleReinforcementBuildingId = "";
        triangleAlterationBuildingId = "";
        trianglePersistenceReserveSeconds = 0.0;

        // Cuarto 2 operativo
        experimentalChamberUnlocked = false;
        experimentalChamberInitialPackGranted = false;

        fragmentCondensation = 0;
        fragmentConfinement = 0;
        fragmentResidualInterference = 0;

        fragmentCondensationProgress = 0.0;
        fragmentConfinementProgress = 0.0;
        fragmentResidualInterferenceProgress = 0.0;

        experimentalHallazgos = 0;
        experimentalMuestras = 0;
        experimentalLecturasIncompletas = 0;
        experimentalCompuestosUtiles = 0;

        synthesisCoreFusionCounter = 0;
        guidedSynthesisIntent = 0;

        // Zona 4 actual: Semillas Dimensionales / Anclajes.
        // Aunque internamente aún se llamen chronal, los reseteamos como sistema activo del run.
        chronalSeedSlots = new List<ChronalSeedSlotState>();
        chronalMatureSeedsStored = 0;

        chronalInstant = new ChronalInstantState();

        chronalMaterializedInstants = 0;
        chronalPureInstants = 0;
        chronalStableInstants = 0;
        chronalForcedInstants = 0;
        chronalArchivedInstants = 0;
        lastChronalMaterializationQuality = "";

        EnsureChronalSeedSlots();

        // IMPORTANTE:
        // experimentalMixLog NO se borra.
        // Ese es el conocimiento descubierto de fusiones.

        // Artefactos / edificios
        if (buildingStates != null)
        {
            foreach (var b in buildingStates)
            {
                if (b == null)
                    continue;

                b.ResetForPrestige();
            }
        }

        // Upgrades F2
        if (F2UpgradeManager.I != null)
        {
            F2UpgradeManager.I.DebugResetAllPurchases();
        }

        // Investigación/base vieja del run
        SaveService.LastLoadedResearchIds = new List<string>();

        if (ResearchManager.I != null)
        {
            ResearchManager.I.ApplyLoadedResearch(SaveService.LastLoadedResearchIds);
        }

        // Máquina / Cuarto 2
        if (MachineManager.I != null)
        {
            MachineManager.I.ClearProgress();
        }

        // Evita que el guardado anterior reaplique niveles viejos en UI
        SaveService.LastLoadedBuildingLevels = new List<SavedBuildingLevel>();

        ActualizarMaxLE();

        TabsUI tabsUI = FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
        if (tabsUI != null)
        {
            tabsUI.RefreshRoom2ButtonVisibility();
            tabsUI.ShowGeneracion();
            tabsUI.RefreshGenerationLayoutFromOutside();
        }
    }

    // F6.3: lógica de reset del run (sin tocar ENT ni upgrades de prestigio)
    private void ResetRunForPrestige()
    {
        // Reset recursos básicos
        LE = 0.0;
        VP = 0.0;

        // Reset recursos avanzados
        BEC = 0.0;
        EM = 0.0;
        emMult = 0.0;
        ADP = 0.0;
        WHF = 0.0;

        // Multiplicadores de investigación (LOS DEJAMOS como están por ahora
        // porque más adelante podríamos decidir si el prestigio los borra o no).
        // researchGlobalLEMult se recalcula desde ResearchManager, así que no lo tocamos.

        // Reset decoherencia (por si la activamos en el futuro)
        useDecoherence = false;
        maxLEAlcanzado = 0.0;

           // Reset de edificios: por ahora dejamos los niveles en 0.
        foreach (var b in buildingStates)
        {
            if (b == null) continue;
            b.ResetForPrestige();
        }
    }


    // Pon esto DENTRO de la clase GameState
    /// <summary>
    /// F8: Genera LE usando:
    /// - baseLEps (producción base continua)
    /// - edificios:
    ///     * si NO tienen tickInterval -> se comportan como antes (LE/s continuo)
    ///     * si tienen tickInterval y lePerTickBase -> generan LE por tick
    /// 
    /// Los multiplicadores globales (EM, research, achievements) se aplican igual
    /// que en CalculateTotalLEps(), para que el HUD y la producción real estén alineados.
    /// </summary>
    private void GenerateLEFromBaseAndBuildings(double dt)
{
    // Seguridad
    if (dt <= 0.0) return;

    // 1) Multiplicadores globales y bonus planos (para LE)
    double multiplier = 1.0;
    double flatBonus = 0.0;

    if (buildingStates != null)
    {
        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue;

            switch (b.def.bonusType)
            {
                case BuildingBonusType.MultiplierLE:
                    multiplier += b.def.bonusPerLevel * b.level;
                    break;

                case BuildingBonusType.FlatLE:
                    flatBonus += b.def.bonusPerLevel * b.level;
                    break;
            }
        }
    }

    double emFactor = 1.0 + emMult;
    double researchFactor = researchGlobalLEMult;

    double achFactor = 1.0;
    if (AchievementManager.I != null)
    {
        achFactor = AchievementManager.I.GetGlobalLEFactor();
    }

    // 🔥 Igual que en CalculateTotalLEps()
    double prestigeFactor = 1.0;

    double f2UpgradeFactor = 1.0;
    if (F2UpgradeManager.I != null)
    {
        f2UpgradeFactor += F2UpgradeManager.I.GetTotalGlobalLEMultBonus();
    }

    double triangleImpulseFactor = GetTriangleImpulseLEMultiplier();

    double machineLEFactor = 1.0;
    if (MachineManager.I != null)
    {
        machineLEFactor += MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.GlobalLEBonus);
    }

    double worldMult = triangleImpulseFactor
                    * multiplier
                    * emFactor
                    * researchFactor
                    * achFactor
                    * prestigeFactor
                    * f2UpgradeFactor
                    * machineLEFactor;

    if (worldMult <= 0) worldMult = 1.0;

    double room1EchoGlobalFactor = GetRoom1EchoGlobalLEMultiplier();

    // 2) Producción base continua (sin edificios)
    if (baseLEps > 0.0)
    {
        LE += baseLEps * worldMult * room1EchoGlobalFactor * dt;
    }

            // 3) Producción de edificios (ticks + continuo)
if (buildingStates != null)
{
    foreach (var b in buildingStates)
    {
        if (b == null || b.def == null) continue;
        if (b.level <= 0) continue;

        var def = b.def;

            if (def.tickInterval > 0.0 && def.lePerTickBase > 0.0)
            {
                float interval = (float)def.tickInterval;

                float expansionBonus = GetPhaseModulatorExpansionTickBonus();
                if (expansionBonus > 0f)
                {
                    interval *= (1f - expansionBonus);
                }

                interval = Mathf.Max(0.0001f, interval);

                b.tickTimer += (float)dt;

            if (b.tickTimer < interval)
                continue;

            int ticks = (int)(b.tickTimer / interval);
            if (ticks <= 0) continue;

            b.tickTimer -= ticks * interval;

            double lePerTick = def.lePerTickBase * b.level;

            // Sinergia del triángulo para Higgs / Tetra
            lePerTick *= GetTriangleSynergyBuildingMultiplier(def.id);

            // Persistencia nueva por reserva activa
            lePerTick *= GetTrianglePersistenceReserveBuildingMultiplier(def.id);

            // Máquina / Zona 1: Calibración de Artefactos
            lePerTick *= GetMachineArtifactProductionMultiplier(def.id);
            lePerTick *= GetRoom1EchoArtifactLEMultiplier(def.id);

            if (F2UpgradeManager.I != null)
            {
                if (def.id == "vacuum_observer")
                {
                    lePerTick *= (1.0 + F2UpgradeManager.I.GetContainmentTuningBonus());
                }

                if (def.id == "casimir_panel")
                {
                    lePerTick *= (1.0 + F2UpgradeManager.I.GetTetraquarkStabilizationBonus());
                }
            }

                double leGain =
                    lePerTick *
                    worldMult *
                    GetMachineRoom1GlobalMultiplier() *
                    room1EchoGlobalFactor *
                    ticks;

                if (def.id != "fluctuation_antenna")
                {
                    LE += leGain;
                }

            if (def.id == "casimir_panel")
            {
                double tracesPerTick = 0.03 * b.level;

                // Máquina / Zona 1: Calibración de Artefactos también afecta al Núcleo Tetraquark
                tracesPerTick *= GetMachineArtifactProductionMultiplier(def.id);

                if (F2UpgradeManager.I != null)
                {
                    tracesPerTick *= (1.0 + F2UpgradeManager.I.GetResidualAnalysisBonus());
                }

                double machineTracesFactor = 1.0;
                if (MachineManager.I != null)
                {
                    machineTracesFactor += MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.TracesBonus);
                }

                tracesPerTick *= machineTracesFactor;
                tracesPerTick *= GetMachineRoom1GlobalMultiplier();

                double tracesGain = tracesPerTick * ticks;
                Traces += tracesGain;
            }

            // EM por tick (si aplica)
            if (def.emPerTickBase > 0.0)
            {
                double emGenFactor = 1.0;

                if (ResearchManager.I != null)
                    emGenFactor *= ResearchManager.I.GetEMGenerationFactor();

                emGenFactor *= GetMetaEMGenerationMultiplier();

                double emPerTick = def.emPerTickBase * b.level * emGenFactor;
                double emGain = emPerTick * ticks;

                EM += emGain;

            }
        }
        else
        {
            // 3B) Edificios clásicos (LE/s continuo)
            if (def.baseLEps > 0.0)
            {
                double leps = def.baseLEps * b.level;
                leps *= GetRoom1EchoArtifactLEMultiplier(def.id);
                LE += leps * worldMult * room1EchoGlobalFactor * dt;
            }
        }
    }
}

        // 4) Bonus plano (LE/s constantes)
        if (flatBonus > 0.0)
        {
            LE += flatBonus * room1EchoGlobalFactor * dt;
        }

    }
}


