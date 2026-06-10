using System.Collections.Generic;
using UnityEngine;

public class MachineManager : MonoBehaviour
{
    public static MachineManager I { get; private set; }
    private static readonly bool FreeMachineNodePurchaseMode = true;
    private readonly Dictionary<string, MachineNodeDef> _defsById = new();
    private readonly List<MachineNodeDef> _allNodes = new();
    private readonly HashSet<string> _repairedNodeIds = new();
    private readonly HashSet<string> _analyzedNodeIds = new();
    private bool _machineIntroSeen;
    private bool _machineUnlocked;
    private bool _machineFusionPanelUnlocked;
    private bool _machineAllZonesUnlocked;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        LoadDefs();
    }

    private void LoadDefs()
    {
        _defsById.Clear();
        _allNodes.Clear();

        TextAsset json = Resources.Load<TextAsset>("Data/machine_nodes");

        if (json == null)
        {
            Debug.LogWarning("[MachineManager] No se encontró Resources/Data/machine_nodes.json");
            return;
        }

        MachineNodeDefList data = JsonUtility.FromJson<MachineNodeDefList>(json.text);

        if (data == null || data.nodes == null)
        {
            Debug.LogWarning("[MachineManager] JSON inválido o vacío.");
            return;
        }

        foreach (MachineNodeDef def in data.nodes)
        {
            if (def == null || string.IsNullOrWhiteSpace(def.id))
                continue;

            if (_defsById.ContainsKey(def.id))
            {
                Debug.LogWarning("[MachineManager] Nodo duplicado ignorado: " + def.id);
                continue;
            }

            _defsById[def.id] = def;
            _allNodes.Add(def);
        }

        Debug.Log("[MachineManager] Nodos cargados: " + _allNodes.Count);
    }

    public MachineNodeDef GetDef(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        _defsById.TryGetValue(id, out MachineNodeDef def);
        return def;
    }

    public List<MachineNodeDef> GetNodesByZone(MachineZoneType zone, bool includeHidden = false)
    {
        List<MachineNodeDef> result = new();

        foreach (MachineNodeDef def in _allNodes)
        {
            if (def == null)
                continue;

            if (def.zone != zone)
                continue;

            bool hiddenSubnodesRevealed = GetTotalEffectValue(MachineNodeEffectType.RevealHiddenSubnodes) > 0.0;

            if (def.hidden && !includeHidden && !hiddenSubnodesRevealed)
                continue;

            result.Add(def);
        }

        return result;
    }

    public List<MachineNodeDef> GetDisplayNodesByZone(MachineZoneType zone, bool includeHidden = false)
    {
        List<MachineNodeDef> result = new();
        Dictionary<string, List<MachineNodeDef>> tierGroups = new();
        HashSet<string> addedTierGroups = new();

        bool hiddenSubnodesRevealed = GetTotalEffectValue(MachineNodeEffectType.RevealHiddenSubnodes) > 0.0;

        foreach (MachineNodeDef def in _allNodes)
        {
            if (def == null)
                continue;

            if (def.zone != zone)
                continue;

            if (def.hidden && !includeHidden && !hiddenSubnodesRevealed)
                continue;

            if (string.IsNullOrWhiteSpace(def.tierGroup))
                continue;

            if (!tierGroups.ContainsKey(def.tierGroup))
                tierGroups[def.tierGroup] = new List<MachineNodeDef>();

            tierGroups[def.tierGroup].Add(def);
        }

        foreach (MachineNodeDef def in _allNodes)
        {
            if (def == null)
                continue;

            if (def.zone != zone)
                continue;

            if (def.hidden && !includeHidden && !hiddenSubnodesRevealed)
                continue;

            if (string.IsNullOrWhiteSpace(def.tierGroup))
            {
                result.Add(def);
                continue;
            }

            if (addedTierGroups.Contains(def.tierGroup))
                continue;

            MachineNodeDef tierNodeToShow = GetTierNodeToShow(tierGroups[def.tierGroup]);

            if (tierNodeToShow != null)
                result.Add(tierNodeToShow);

            addedTierGroups.Add(def.tierGroup);
        }

        return result;
    }

    private MachineNodeDef GetTierNodeToShow(List<MachineNodeDef> tierNodes)
    {
        if (tierNodes == null || tierNodes.Count <= 0)
            return null;

        tierNodes.Sort((a, b) => a.tierIndex.CompareTo(b.tierIndex));

        MachineNodeDef highestTier = tierNodes[tierNodes.Count - 1];

        foreach (MachineNodeDef tierNode in tierNodes)
        {
            if (tierNode == null)
                continue;

            if (!IsNodeRepaired(tierNode.id))
                return tierNode;
        }

        return highestTier;
    }

    [ContextMenu("DEBUG: Print Machine Nodes")]
    private void DebugPrintNodes()
    {
        Debug.Log("[MachineManager] Total nodos: " + _allNodes.Count);

        foreach (MachineNodeDef def in _allNodes)
        {
            Debug.Log($"[MachineManager] {def.id} | {def.name} | Zona: {def.zone}");
        }
    }

    public void LoadProgressFromSave(SaveData data)
    {
        _repairedNodeIds.Clear();
        _analyzedNodeIds.Clear();

        if (data == null)
            return;

        if (data.machineRepairedNodeIds != null)
        {
            foreach (string nodeId in data.machineRepairedNodeIds)
            {
                if (string.IsNullOrWhiteSpace(nodeId))
                    continue;

                _repairedNodeIds.Add(nodeId);
            }
        }

        if (data.machineAnalyzedNodeIds != null)
        {
            foreach (string nodeId in data.machineAnalyzedNodeIds)
            {
                if (string.IsNullOrWhiteSpace(nodeId))
                    continue;

                _analyzedNodeIds.Add(nodeId);
            }
        }

        _machineIntroSeen = data.machineIntroSeen;
        _machineUnlocked = data.machineUnlocked;
        _machineFusionPanelUnlocked = data.machineFusionPanelUnlocked;
        _machineAllZonesUnlocked = data.machineAllZonesUnlocked || data.machineUnlocked;

        Debug.Log(
            "[MachineManager] Progreso cargado. Nodos reparados: "
            + _repairedNodeIds.Count
            + " | Nodos analizados: "
            + _analyzedNodeIds.Count
        );
    }

    public void WriteProgressToSave(SaveData data)
    {
        if (data == null)
            return;

        data.machineRepairedNodeIds = new List<string>(_repairedNodeIds);
        data.machineAnalyzedNodeIds = new List<string>(_analyzedNodeIds);

        data.machineIntroSeen = _machineIntroSeen;
        data.machineUnlocked = _machineUnlocked;
        data.machineFusionPanelUnlocked = _machineFusionPanelUnlocked;
        data.machineAllZonesUnlocked = MachineAllZonesUnlocked;
    }

    public bool IsNodeRepaired(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        return _repairedNodeIds.Contains(nodeId);
    }

    public bool IsNodeAnalyzed(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        MachineNodeDef def = GetDef(nodeId);

        if (def == null)
            return false;

        string analysisKey = GetNodeAnalysisKey(def);

        return
            _analyzedNodeIds.Contains(analysisKey) ||
            _analyzedNodeIds.Contains(nodeId);
    }

    public void MarkNodeAnalyzed(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return;

        MachineNodeDef def = GetDef(nodeId);

        if (def == null)
            return;

        if (!def.damaged)
            return;

        if (_repairedNodeIds.Contains(nodeId))
            return;

        string analysisKey = GetNodeAnalysisKey(def);
        _analyzedNodeIds.Add(analysisKey);

        if (SaveService.I != null)
        {
            SaveService.I.Save();
        }
    }

    private string GetNodeAnalysisKey(MachineNodeDef def)
    {
        if (def == null)
            return "";

        if (!string.IsNullOrWhiteSpace(def.tierGroup))
            return "tierGroup:" + def.tierGroup;

        return def.id;
    }
    private bool IsRequirementSatisfiedForNode(MachineNodeDef currentDef, string requiredId)
    {
        if (string.IsNullOrWhiteSpace(requiredId))
            return true;

        if (IsNodeRepaired(requiredId))
            return true;

        MachineNodeDef requiredDef = GetDef(requiredId);

        if (requiredDef == null)
            return false;

        bool requirementIsTier =
            !string.IsNullOrWhiteSpace(requiredDef.tierGroup);

        bool currentIsSameTierGroup =
            currentDef != null
            && !string.IsNullOrWhiteSpace(currentDef.tierGroup)
            && currentDef.tierGroup == requiredDef.tierGroup;

        // Dentro de una misma línea de tiers, se exige el tier exacto anterior.
        // Ejemplo: Acople III sí necesita Acople II.
        if (requirementIsTier && currentIsSameTierGroup)
            return false;

        // Para nodos externos, cualquier tier comprado de esa línea cuenta como requisito.
        if (requirementIsTier)
        {
            foreach (string repairedNodeId in _repairedNodeIds)
            {
                MachineNodeDef repairedDef = GetDef(repairedNodeId);

                if (repairedDef == null)
                    continue;

                if (repairedDef.tierGroup == requiredDef.tierGroup)
                    return true;
            }
        }

        return false;
    }

    private bool ShouldEnforceNodeRequirement(MachineNodeDef currentDef, string requiredId)
    {
        if (!FreeMachineNodePurchaseMode)
            return true;

        if (currentDef == null)
            return true;

        if (string.IsNullOrWhiteSpace(requiredId))
            return false;

        MachineNodeDef requiredDef = GetDef(requiredId);

        if (requiredDef == null)
            return false;

        bool currentIsTier =
            !string.IsNullOrWhiteSpace(currentDef.tierGroup);

        bool requiredIsSameTier =
            !string.IsNullOrWhiteSpace(requiredDef.tierGroup)
            && currentDef.tierGroup == requiredDef.tierGroup;

        // En modo libre solo mantenemos el orden interno de tiers.
        // Ejemplo: Acople III sigue necesitando Acople II.
        return currentIsTier && requiredIsSameTier;
    }

    public bool IsNodeDamageResolved(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return false;

        MachineNodeDef def = GetDef(nodeId);

        if (def == null)
            return false;

        if (!def.damaged)
            return false;

        if (_repairedNodeIds.Contains(nodeId))
            return true;

        if (string.IsNullOrWhiteSpace(def.tierGroup))
            return false;

        foreach (string repairedNodeId in _repairedNodeIds)
        {
            MachineNodeDef repairedDef = GetDef(repairedNodeId);

            if (repairedDef == null)
                continue;

            if (repairedDef.tierGroup == def.tierGroup)
                return true;
        }

        return false;
    }

    public bool IsBlockedByFusionMaterial(string nodeId, out string missingMaterialName)
    {
        missingMaterialName = "";

        if (GameState.I == null)
            return false;

        MachineNodeDef def = GetDef(nodeId);

        if (def == null)
            return false;

        if (IsNodeRepaired(nodeId))
            return false;

        if (def.cost == null)
            return false;

        MachineNodeCostDef cost = GetEffectiveNodeCost(def);

        if (cost == null)
            return false;

        if (def.requiredNodeIds != null)
        {
            foreach (string requiredId in def.requiredNodeIds)
            {
                if (string.IsNullOrWhiteSpace(requiredId))
                    continue;

                if (!ShouldEnforceNodeRequirement(def, requiredId))
                    continue;

                if (!IsRequirementSatisfiedForNode(def, requiredId))
                    return false;
            }
        }

        if (cost.hallazgo > 0 && GameState.I.experimentalHallazgos < cost.hallazgo)
        {
            missingMaterialName = "Hallazgo";
            return true;
        }

        if (cost.muestra > 0 && GameState.I.experimentalMuestras < cost.muestra)
        {
            missingMaterialName = "Muestra";
            return true;
        }

        if (cost.lecturaIncompleta > 0 && GameState.I.experimentalLecturasIncompletas < cost.lecturaIncompleta)
        {
            missingMaterialName = "Lectura Incompleta";
            return true;
        }

        if (cost.compuestoUtil > 0 && GameState.I.experimentalCompuestosUtiles < cost.compuestoUtil)
        {
            missingMaterialName = "Compuesto Útil";
            return true;
        }

        return false;
    }

    private bool NodeRequiresFusionMaterial(MachineNodeDef def)
    {
        if (def == null || def.cost == null)
            return false;

        return def.cost.hallazgo > 0
            || def.cost.muestra > 0
            || def.cost.lecturaIncompleta > 0
            || def.cost.compuestoUtil > 0;
    }

    public MachineNodeCostDef GetEffectiveNodeCost(MachineNodeDef def)
    {
        if (def == null || def.cost == null)
            return null;

        MachineNodeCostDef effectiveCost = new MachineNodeCostDef
        {
            le = def.cost.le,
            traces = def.cost.traces,
            hallazgo = def.cost.hallazgo,
            muestra = def.cost.muestra,
            lecturaIncompleta = def.cost.lecturaIncompleta,
            compuestoUtil = def.cost.compuestoUtil,

            pureInstant = def.cost.pureInstant,
            stableInstant = def.cost.stableInstant,
            forcedInstant = def.cost.forcedInstant
        };

        if (ShouldApplyCompensationCircuitToNode(def))
        {
            effectiveCost.le *= 0.95;
            effectiveCost.traces *= 0.95;

            effectiveCost.hallazgo = ReduceSimpleOrMediumMaterialCost(effectiveCost.hallazgo);
            effectiveCost.muestra = ReduceSimpleOrMediumMaterialCost(effectiveCost.muestra);
            effectiveCost.lecturaIncompleta = ReduceSimpleOrMediumMaterialCost(effectiveCost.lecturaIncompleta);

            // Compuesto Útil queda fuera por ahora porque lo estamos tratando como material alto/avanzado.
        }

        double internalSupportDiscount = GetZoneProgressSyncBonus(MachineZoneType.InternalSupport);

        if (internalSupportDiscount > 0.0)
        {
            effectiveCost.le *= (1.0 - internalSupportDiscount);
            effectiveCost.traces *= (1.0 - internalSupportDiscount);
        }

        double structuralReinforcementDiscount = GetStructuralReinforcementDiscount();

        if (structuralReinforcementDiscount > 0.0)
        {
            effectiveCost.le *= (1.0 - structuralReinforcementDiscount);
            effectiveCost.traces *= (1.0 - structuralReinforcementDiscount);
        }

        double internalSupportBonus = GetTotalEffectValue(MachineNodeEffectType.InternalSupportBonus);

        if (internalSupportBonus > 0.0)
        {
            effectiveCost.le *= (1.0 - internalSupportBonus);
            effectiveCost.traces *= (1.0 - internalSupportBonus);
        }

        return effectiveCost;
    }

    private MachineZoneType GetLeastRepairedZone()
    {
        MachineZoneType[] zones =
        {
            MachineZoneType.Room1Link,
            MachineZoneType.FusionSector,
            MachineZoneType.InternalSupport,
            MachineZoneType.InstantChamber
        };

        MachineZoneType leastZone = MachineZoneType.None;
        double lowestProgress = double.MaxValue;

        foreach (MachineZoneType zone in zones)
        {
            double progress = GetZoneRepairProgress01(zone);

            if (progress < lowestProgress)
            {
                lowestProgress = progress;
                leastZone = zone;
            }
        }

        return leastZone;
    }

    private bool IsFinalMachineNode(MachineNodeDef def)
    {
        if (def == null)
            return false;

        return
            def.id == "z1_room1_synchronizer" ||
            def.id == "z2_synthesis_core" ||
            def.id == "z3_convergence_channel" ||
            def.id == "z4_pure_materialization_2";
    }

    private bool ShouldApplyCompensationCircuitToNode(MachineNodeDef def)
    {
        if (def == null)
            return false;

        if (GetTotalEffectValue(MachineNodeEffectType.CompensationCircuit) <= 0.0)
            return false;

        if (IsFinalMachineNode(def))
            return false;

        MachineZoneType leastZone = GetLeastRepairedZone();

        return def.zone == leastZone;
    }

    private int ReduceSimpleOrMediumMaterialCost(int amount)
    {
        if (amount >= 3)
            return amount - 1;

        return amount;
    }

    public bool CanRepairNode(string nodeId, out string reason)
    {
        reason = "";

        if (GameState.I == null)
        {
            reason = "GameState no disponible.";
            return false;
        }

        MachineNodeDef def = GetDef(nodeId);

        if (def == null)
        {
            reason = "Nodo no encontrado.";
            return false;
        }

        if (IsNodeRepaired(nodeId))
        {
            reason = "Nodo ya reparado.";
            return false;
        }

        if (def.damaged && !IsNodeDamageResolved(nodeId) && !IsNodeAnalyzed(nodeId))
        {
            reason = "Nodo dañado sin analizar.";
            return false;
        }

        if (def.requiredNodeIds != null)
        {
            foreach (string requiredId in def.requiredNodeIds)
            {
                if (string.IsNullOrWhiteSpace(requiredId))
                    continue;

                if (!ShouldEnforceNodeRequirement(def, requiredId))
                    continue;

                if (!IsRequirementSatisfiedForNode(def, requiredId))
                {
                    reason = "Falta reparar nodo requerido: " + requiredId;
                    return false;
                }
            }
        }

        MachineNodeCostDef cost = GetEffectiveNodeCost(def);

        if (cost == null)
        {
            reason = "Costo inválido.";
            return false;
        }

        if (GameState.I.LE < cost.le)
        {
            reason = "Falta LE.";
            return false;
        }

        if (GameState.I.Traces < cost.traces)
        {
            reason = "Faltan Trazas.";
            return false;
        }

        if (GameState.I.experimentalHallazgos < cost.hallazgo)
        {
            reason = "Faltan Hallazgos.";
            return false;
        }

        if (GameState.I.experimentalMuestras < cost.muestra)
        {
            reason = "Faltan Muestras.";
            return false;
        }

        if (GameState.I.experimentalLecturasIncompletas < cost.lecturaIncompleta)
        {
            reason = "Faltan Lecturas Incompletas.";
            return false;
        }

        if (GameState.I.experimentalCompuestosUtiles < cost.compuestoUtil)
        {
            reason = "Faltan Compuestos Útiles.";
            return false;
        }

        if (GameState.I.chronalPureInstants < cost.pureInstant)
        {
            reason = "Faltan Anclajes Puros.";
            return false;
        }

        if (GameState.I.chronalStableInstants < cost.stableInstant)
        {
            reason = "Faltan Anclajes Estables.";
            return false;
        }

        if (GameState.I.chronalForcedInstants < cost.forcedInstant)
        {
            reason = "Faltan Anclajes Forzados.";
            return false;
        }

        reason = "Disponible.";
        return true;
    }

    public bool TryRepairNode(string nodeId)
    {
    if (!CanRepairNode(nodeId, out string reason))
    {
        if (IsBlockedByFusionMaterial(nodeId, out string missingMaterialName))
        {
            Debug.LogWarning(
                "[MachineManager] Nodo bloqueado por material de fusión: "
                + nodeId
                + " | Falta: "
                + missingMaterialName
            );

            UnlockFusionPanel();
        }
        else
        {
            Debug.LogWarning("[MachineManager] No se puede reparar " + nodeId + ": " + reason);
        }

        return false;
    }

        MachineNodeDef def = GetDef(nodeId);
        MachineNodeCostDef cost = GetEffectiveNodeCost(def);

        GameState.I.LE -= cost.le;
        GameState.I.Traces -= cost.traces;

        GameState.I.experimentalHallazgos -= cost.hallazgo;
        GameState.I.experimentalMuestras -= cost.muestra;
        GameState.I.experimentalLecturasIncompletas -= cost.lecturaIncompleta;
        GameState.I.experimentalCompuestosUtiles -= cost.compuestoUtil;

        GameState.I.chronalPureInstants -= cost.pureInstant;
        GameState.I.chronalStableInstants -= cost.stableInstant;
        GameState.I.chronalForcedInstants -= cost.forcedInstant;

        int consumedInstants =
            cost.pureInstant +
            cost.stableInstant +
            cost.forcedInstant;

        GameState.I.chronalArchivedInstants =
            Mathf.Max(0, GameState.I.chronalArchivedInstants - consumedInstants);

        _repairedNodeIds.Add(nodeId);

        ApplyNodeEffect(def);

        if (def.effectType == MachineNodeEffectType.UnlockFusionSlot && def.effectValue >= 1.0)
        {
            UnlockFusionPanel();
        }

        if (NodeRequiresFusionMaterial(def))
        {
            UnlockFusionPanel();
            UnlockAllMachineZones();
        }

        Debug.Log("[MachineManager] Nodo reparado: " + def.id + " | " + def.name);

        if (SaveService.I != null)
        {
            SaveService.I.Save();
        }

        return true;
    }

    private void ApplyNodeEffect(MachineNodeDef def)
    {
        if (def == null)
            return;

        // Por ahora solo registramos el efecto.
        // La aplicación real de bonus la haremos después, para no romper producción ni balance.
        Debug.Log(
            "[MachineManager] Efecto pendiente de aplicar: "
            + def.effectType
            + " | Valor: "
            + def.effectValue
        );
    }

    public double GetTotalEffectValue(MachineNodeEffectType effectType)
    {
        double total = 0.0;

        foreach (string nodeId in _repairedNodeIds)
        {
            MachineNodeDef def = GetDef(nodeId);

            if (def == null)
                continue;

            if (def.effectType != effectType)
                continue;

            total += def.effectValue;
        }

        return total;
    }

    public bool HasZoneProgressSyncCore()
    {
        return GetTotalEffectValue(MachineNodeEffectType.ZoneProgressSyncBonus) > 0.0;
    }

    public double GetZoneRepairProgress01(MachineZoneType zone)
    {
        List<MachineNodeDef> nodes = GetNodesByZone(zone);

        if (nodes == null || nodes.Count <= 0)
            return 0.0;

        int total = 0;
        int repaired = 0;

        foreach (MachineNodeDef node in nodes)
        {
            if (node == null)
                continue;

            total++;

            if (IsNodeRepaired(node.id))
            {
                repaired++;
            }
        }

        if (total <= 0)
            return 0.0;

        return (double)repaired / total;
    }

    public double GetZoneProgressSyncBonus(MachineZoneType zone)
    {
        if (!HasZoneProgressSyncCore())
            return 0.0;

        double progress = GetZoneRepairProgress01(zone);

        if (progress >= 1.0)
            return 0.08;

        if (progress >= 0.75)
            return 0.06;

        if (progress >= 0.50)
            return 0.04;

        if (progress >= 0.25)
            return 0.02;

        return 0.0;
    }

    private double GetStructuralReinforcementDiscount()
    {
        if (GetTotalEffectValue(MachineNodeEffectType.StructuralSupportBonus) <= 0.0)
            return 0.0;

        int repairedCount = _repairedNodeIds.Count;

        if (repairedCount >= 30)
            return 0.06;

        if (repairedCount >= 20)
            return 0.04;

        if (repairedCount >= 10)
            return 0.02;

        return 0.0;
    }

    public int GetUnlockedFusionSlotCount()
    {
        int maxSlots = 0;

        foreach (string nodeId in _repairedNodeIds)
        {
            MachineNodeDef def = GetDef(nodeId);

            if (def == null)
                continue;

            if (def.effectType != MachineNodeEffectType.UnlockFusionSlot)
                continue;

            int slotsFromNode = Mathf.RoundToInt((float)def.effectValue);

            if (slotsFromNode > maxSlots)
                maxSlots = slotsFromNode;
        }

        return maxSlots;
    }

    public bool MachineIntroSeen => _machineIntroSeen;
    public bool MachineUnlocked => _machineUnlocked;
    public bool MachineFusionPanelUnlocked => _machineFusionPanelUnlocked;
    public bool MachineAllZonesUnlocked => _machineUnlocked || _machineAllZonesUnlocked;

    public bool Prestige1Prepared =>
        GetTotalEffectValue(MachineNodeEffectType.EnablePrestige1) > 0.0;
        

    public double GetTotalMachineRepairProgress01()
    {
        int total = 0;
        int repaired = 0;

        foreach (MachineNodeDef node in _allNodes)
        {
            if (node == null)
                continue;

            // Por ahora contamos todos los nodos visibles/no ocultos del diseño base.
            // Los ocultos los dejamos fuera para no bloquear Prestigio 1 por contenido secreto.
            if (node.hidden)
                continue;

            total++;

            if (IsNodeRepaired(node.id))
            {
                repaired++;
            }
        }

        if (total <= 0)
            return 0.0;

        return (double)repaired / total;
    }

    public bool HasEnoughRepairForPrestige1()
    {
        return GetTotalMachineRepairProgress01() >= 0.80;
    }

    public bool InstantChamberCoreUnlocked =>
        GetTotalEffectValue(MachineNodeEffectType.UnlockInstantChamber) > 0.0;

    public void MarkIntroSeenAndUnlockMachine()
    {
        _machineIntroSeen = true;
        _machineUnlocked = true;
        _machineAllZonesUnlocked = true;

        Debug.Log("[MachineManager] Intro vista. Máquina desbloqueada con todas sus zonas visibles.");

        if (SaveService.I != null)
        {
            SaveService.I.Save();
        }
    }

    public void UnlockFusionPanel()
    {
        if (_machineFusionPanelUnlocked)
            return;

        _machineFusionPanelUnlocked = true;

        Debug.Log("[MachineManager] Panel de Fusiones desbloqueado.");

        if (SaveService.I != null)
        {
            SaveService.I.Save();
        }
    }

    public void UnlockAllMachineZones()
    {
        if (_machineAllZonesUnlocked)
            return;

        _machineAllZonesUnlocked = true;

        Debug.Log("[MachineManager] Todas las zonas de la Máquina desbloqueadas.");

        if (SaveService.I != null)
        {
            SaveService.I.Save();
        }
    }

    public bool CanAccessZone(MachineZoneType zone)
    {
        return _machineUnlocked;
    }

    [ContextMenu("DEBUG: Repair First Machine Node")]
    private void DebugRepairFirstNode()
    {
        if (_allNodes.Count <= 0)
        {
            Debug.LogWarning("[MachineManager] No hay nodos para reparar.");
            return;
        }

        MachineNodeDef firstNode = _allNodes[0];

        TryRepairNode(firstNode.id);
    }

    [ContextMenu("DEBUG: Repair First Available Machine Node")]
    private void DebugRepairFirstAvailableNode()
    {
        if (_allNodes.Count <= 0)
        {
            Debug.LogWarning("[MachineManager] No hay nodos para reparar.");
            return;
        }

        foreach (MachineNodeDef node in _allNodes)
        {
            if (node == null)
                continue;

            if (IsNodeRepaired(node.id))
                continue;

        if (CanRepairNode(node.id, out string reason))
        {
            TryRepairNode(node.id);
            return;
        }

        Debug.Log("[MachineManager] Nodo no disponible: " + node.id + " | Razón: " + reason);

        if (IsBlockedByFusionMaterial(node.id, out string missingMaterialName))
        {
            Debug.LogWarning(
                "[MachineManager] Nodo bloqueado por material de fusión: "
                + node.id
                + " | Falta: "
                + missingMaterialName
            );

            UnlockFusionPanel();
        }

            Debug.Log("[MachineManager] Nodo no disponible: " + node.id + " | Razón: " + reason);
        }

        Debug.LogWarning("[MachineManager] No hay nodos disponibles para reparar.");
    }

    [ContextMenu("DEBUG: Print Machine Node Status")]
    private void DebugPrintMachineNodeStatus()
    {
        Debug.Log("[MachineManager] ===== ESTADO DE NODOS =====");
        Debug.Log("[MachineManager] Total nodos cargados: " + _allNodes.Count);
        Debug.Log("[MachineManager] Total nodos reparados: " + _repairedNodeIds.Count);

        foreach (MachineNodeDef node in _allNodes)
        {
            if (node == null)
                continue;

            bool repaired = IsNodeRepaired(node.id);
            bool canRepair = CanRepairNode(node.id, out string reason);

            Debug.Log(
                "[MachineManager] "
                + node.id
                + " | "
                + node.name
                + " | Zona: "
                + node.zone
                + " | Dañado: "
                + node.damaged
                + " | Reparado: "
                + repaired
                + " | Puede reparar: "
                + canRepair
                + " | Razón: "
                + reason
            );
        }

        Debug.Log("[MachineManager] ===========================");
    }

    [ContextMenu("DEBUG: Print Fusion Material Blocks")]
    private void DebugPrintFusionMaterialBlocks()
    {
        Debug.Log("[MachineManager] ===== NODOS BLOQUEADOS POR MATERIAL DE FUSIÓN =====");

        foreach (MachineNodeDef node in _allNodes)
        {
            if (node == null)
                continue;

            if (IsBlockedByFusionMaterial(node.id, out string missingMaterialName))
            {
                Debug.Log(
                    "[MachineManager] "
                    + node.id
                    + " | "
                    + node.name
                    + " | Falta: "
                    + missingMaterialName
                );
            }
        }

        Debug.Log("[MachineManager] ==================================================");
    }

    public void ClearProgress()
    {
        _repairedNodeIds.Clear();
        _analyzedNodeIds.Clear();

        _machineIntroSeen = false;
        _machineUnlocked = false;
        _machineFusionPanelUnlocked = false;
        _machineAllZonesUnlocked = false;

        Debug.Log("[MachineManager] Progreso limpiado.");
    }

    [ContextMenu("DEBUG: Unlock Machine Intro")]
    private void DebugUnlockMachineIntro()
    {
        MarkIntroSeenAndUnlockMachine();
    }

    [ContextMenu("DEBUG: Unlock Fusion Panel")]
    private void DebugUnlockFusionPanel()
    {
        UnlockFusionPanel();
    }

    [ContextMenu("DEBUG: Unlock All Machine Zones")]
    private void DebugUnlockAllMachineZones()
    {
        UnlockAllMachineZones();
    }

    [ContextMenu("DEBUG: Print Machine Flow State")]
    private void DebugPrintMachineFlowState()
    {
        Debug.Log(
            "[MachineManager] Flow State"
            + " | IntroSeen: " + _machineIntroSeen
            + " | MachineUnlocked: " + _machineUnlocked
            + " | FusionPanelUnlocked: " + _machineFusionPanelUnlocked
            + " | AllZonesUnlocked: " + _machineAllZonesUnlocked
        );

        Debug.Log("[MachineManager] CanAccess Zona 1: " + CanAccessZone(MachineZoneType.Room1Link));
        Debug.Log("[MachineManager] CanAccess Zona 2: " + CanAccessZone(MachineZoneType.FusionSector));
        Debug.Log("[MachineManager] CanAccess Zona 3: " + CanAccessZone(MachineZoneType.InternalSupport));
        Debug.Log("[MachineManager] CanAccess Zona 4: " + CanAccessZone(MachineZoneType.InstantChamber));
    }
}