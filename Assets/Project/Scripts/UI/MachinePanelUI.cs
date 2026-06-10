using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachinePanelUI : MonoBehaviour
{
    [Header("Textos principales")]
    [SerializeField] private TextMeshProUGUI machineTitleText;
    [SerializeField] private TextMeshProUGUI machineStateText;
    [SerializeField] private TextMeshProUGUI zoneTitleText;
    [SerializeField] private TextMeshProUGUI zoneDescriptionText;
    [SerializeField] private TextMeshProUGUI fusionStateText;
    [SerializeField] private TextMeshProUGUI nodeListText;
    [SerializeField] private TextMeshProUGUI machineNoticeText;
    [SerializeField] private TextMeshProUGUI machineDiagnosticsText;
        
    [Header("Panel de fusión")]
    [SerializeField] private GameObject legacyFusionPanel;
    [SerializeField] private Button btnBackToNodesFromFusion;
    [SerializeField] private Button btnFusionPanel;
    
    [Header("Vistas")]
    [SerializeField] private GameObject machineRepairViewRoot;
    [SerializeField] private GameObject machineNodeViewRoot;

    [Header("Selección de nodos")]
    [SerializeField] private TextMeshProUGUI selectedNodeText;
    [SerializeField] private Button btnPrevNode;
    [SerializeField] private Button btnNextNode;
    [SerializeField] private Button btnRepairNode;
    [SerializeField] private Button btnAnalyzeNode;
    [SerializeField] private TextMeshProUGUI nodeAnalysisText;
    [Header("Botones de sectores")]
    [SerializeField] private Button btnZone1;
    [SerializeField] private Button btnZone2;
    [SerializeField] private Button btnZone3;
    [SerializeField] private Button btnZone4;

    [Header("Cámara de Anclajes")]
    [SerializeField] private Button btnInstantChamberHelp;
    [SerializeField] private Button btnOpenSeedsPanel;
    [SerializeField] private GameObject instantSeedsViewRoot;
    [SerializeField] private Button btnBackToMachine;
    [SerializeField] private TextMeshProUGUI seedsSlotsText;
    [SerializeField] private Button btnCreateSeed;
    [SerializeField] private Button btnFormInstant;
    [SerializeField] private Button btnStabilizeInstant;
    [SerializeField] private Slider stabilizationIntensitySlider;
    [SerializeField] private TextMeshProUGUI stabilizationPreviewText;
    [SerializeField] private Button btnRewindInstant;
    [SerializeField] private Button btnMaterializeInstant;
    [SerializeField] private Button btnDiscardInstant;

    private MachineZoneType _currentZone = MachineZoneType.FusionSector;
    private int _selectedNodeIndex = 0;
    private bool _fusionPanelVisible;
    private string _lastAnalyzedNodeId = "";
    private readonly Dictionary<MachineZoneType, int> _selectedNodeIndexByZone =
    
    new Dictionary<MachineZoneType, int>();

    private const float NodeAnalysisDurationSeconds = 3f;
    private string _analyzingNodeId = "";
    private float _analysisRemainingSeconds = 0f;

    private void Awake()
    {
        if (btnZone1 != null)
            btnZone1.onClick.AddListener(() => SelectZone(MachineZoneType.Room1Link));

        if (btnZone2 != null)
            btnZone2.onClick.AddListener(() => SelectZone(MachineZoneType.FusionSector));

        if (btnZone3 != null)
            btnZone3.onClick.AddListener(() => SelectZone(MachineZoneType.InternalSupport));

        if (btnZone4 != null)
            btnZone4.onClick.AddListener(() => SelectZone(MachineZoneType.InstantChamber));

        if (btnPrevNode != null)
            btnPrevNode.onClick.AddListener(SelectPreviousNode);

        if (btnNextNode != null)
            btnNextNode.onClick.AddListener(SelectNextNode);

        if (btnRepairNode != null)
            btnRepairNode.onClick.AddListener(RepairSelectedNode);

        if (btnAnalyzeNode != null)
            btnAnalyzeNode.onClick.AddListener(AnalyzeSelectedNode);

        if (btnFusionPanel != null)
            btnFusionPanel.onClick.AddListener(OpenFusionPanel);

        if (btnBackToNodesFromFusion != null)
            btnBackToNodesFromFusion.onClick.AddListener(CloseFusionPanel);

        if (btnOpenSeedsPanel != null)
            btnOpenSeedsPanel.onClick.AddListener(OpenSeedsPanelPlaceholder);

        if (btnInstantChamberHelp != null)
            btnInstantChamberHelp.onClick.AddListener(ShowContextHelpPopup);
        
        if (btnBackToMachine != null)
            btnBackToMachine.onClick.AddListener(CloseSeedsView);
            
        if (btnCreateSeed != null)
            btnCreateSeed.onClick.AddListener(CreateChronalSeed);

        if (btnFormInstant != null)
            btnFormInstant.onClick.AddListener(FormInstantPlaceholder);

        if (btnStabilizeInstant != null)
            btnStabilizeInstant.onClick.AddListener(StabilizeInstant);
            
        if (btnRewindInstant != null)
            btnRewindInstant.onClick.AddListener(RewindInstant);

        if (btnMaterializeInstant != null)
            btnMaterializeInstant.onClick.AddListener(MaterializeInstant);

        if (btnDiscardInstant != null)
            btnDiscardInstant.onClick.AddListener(DiscardInstant);
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {

        if (instantSeedsViewRoot != null && instantSeedsViewRoot.activeSelf)
        {
            RefreshSeedsView();
        }

        if (string.IsNullOrWhiteSpace(_analyzingNodeId))
            return;

        _analysisRemainingSeconds -= Time.deltaTime;

        if (_analysisRemainingSeconds > 0f)
        {
            RefreshNodeAnalysis();
            return;
        }

        string finishedNodeId = _analyzingNodeId;

        _analyzingNodeId = "";
        _analysisRemainingSeconds = 0f;

        if (MachineManager.I != null)
        {
            MachineManager.I.MarkNodeAnalyzed(finishedNodeId);
            _lastAnalyzedNodeId = finishedNodeId;
        }

        Refresh();
    }

    public void Refresh()
    {
        if (MachineManager.I == null)
        {
            SetText(machineTitleText, "Máquina");
            SetText(machineStateText, "Estado: sistema no disponible");
            SetText(nodeListText, "MachineManager no encontrado.");
            return;
        }

        EnsureCurrentZoneIsAccessible();
        ClampSelectedNodeIndex();

        RefreshHeader();
        RefreshZoneButtons();
        RefreshSelectedZone();
        RefreshNodeList();
        RefreshSelectedNodeControls();
        RefreshNotice();
        RefreshDiagnostics();
        RefreshNodeAnalysis();
        RefreshFusionPanelVisibility();
        RefreshInstantChamberButtons();
        RefreshContextHelpButton();
    }

    private void EnsureCurrentZoneIsAccessible()
    {
        if (MachineManager.I.CanAccessZone(_currentZone))
            return;

        if (MachineManager.I.CanAccessZone(MachineZoneType.FusionSector))
        {
            _currentZone = MachineZoneType.FusionSector;
        }
    }

    private void RefreshHeader()
    {
        SetText(machineTitleText, "Máquina del Cuarto 2");

        if (!MachineManager.I.MachineUnlocked)
        {
            SetText(machineStateText, "Estado: estructura detectada");
            SetText(fusionStateText, "Panel de Fusión: sin acceso");            
            return;
        }

        string zoneState = MachineManager.I.MachineAllZonesUnlocked
            ? "sectores estabilizados"
            : "acceso parcial";

        SetText(machineStateText, "Estado: " + zoneState);

        string fusionState = MachineManager.I.MachineFusionPanelUnlocked
            ? "Panel de Fusión: activo"
            : "Panel de Fusión: bloqueado";

        SetText(fusionStateText, fusionState);
    }

    private void RefreshZoneButtons()
    {
        bool allZonesUnlocked = MachineManager.I.MachineAllZonesUnlocked;

        SetupZoneButton(btnZone1, MachineZoneType.Room1Link, allZonesUnlocked ? "Enlace" : "???");
        SetupZoneButton(btnZone2, MachineZoneType.FusionSector, allZonesUnlocked ? "Fusiones" : "Sector activo");
        SetupZoneButton(btnZone3, MachineZoneType.InternalSupport, allZonesUnlocked ? "Soporte" : "???");
        SetupZoneButton(btnZone4, MachineZoneType.InstantChamber, allZonesUnlocked ? "Cámara" : "???");
    }

    private void SetupZoneButton(Button button, MachineZoneType zone, string label)
    {
        if (button == null)
            return;

        bool canAccess = MachineManager.I.CanAccessZone(zone);
        button.interactable = canAccess;

        TextMeshProUGUI labelText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (labelText != null)
            labelText.text = label;
    }

    private void SelectZone(MachineZoneType zone)
    {
        if (MachineManager.I == null)
            return;

        if (!MachineManager.I.CanAccessZone(zone))
        {
            Refresh();
            return;
        }

        SaveSelectedNodeIndexForCurrentZone();

        _currentZone = zone;
        _selectedNodeIndex = GetSavedSelectedNodeIndexForZone(zone);
        _lastAnalyzedNodeId = "";

        _fusionPanelVisible = false;

        if (instantSeedsViewRoot != null)
            instantSeedsViewRoot.SetActive(false);

        Refresh();
    }

    private void SaveSelectedNodeIndexForCurrentZone()
    {
        _selectedNodeIndexByZone[_currentZone] = _selectedNodeIndex;
    }

    private int GetSavedSelectedNodeIndexForZone(MachineZoneType zone)
    {
        if (_selectedNodeIndexByZone.TryGetValue(zone, out int index))
            return index;

        return 0;
    }

    private void RefreshSelectedZone()
    {
        switch (_currentZone)
        {
            case MachineZoneType.Room1Link:
                SetText(zoneTitleText, "Enlace con el Cuarto 1");
                SetText(zoneDescriptionText, "Conecta la Máquina con los sistemas principales del primer cuarto.");
                break;

            case MachineZoneType.FusionSector:
                SetText(zoneTitleText, MachineManager.I.MachineAllZonesUnlocked ? "Sector de Fusiones" : "Sector activo");
                SetText(zoneDescriptionText, "Repara los primeros sistemas para obtener materiales experimentales.");
                break;

            case MachineZoneType.InternalSupport:
                SetText(zoneTitleText, "Soporte Interno");
                SetText(zoneDescriptionText, "Sistemas internos de diagnóstico, estabilidad y avance de la Máquina.");
                break;

            case MachineZoneType.InstantChamber:
                SetText(zoneTitleText, "Cámara de Anclajes");

                if (MachineManager.I.InstantChamberCoreUnlocked)
                {
                    SetText(
                        zoneDescriptionText,
                        "Cámara Básica activa.\n" +
                        "Semillas Dimensionales, Anclajes y Archivo quedan preparados."
                    );
                }
                else
                {
                    SetText(
                        zoneDescriptionText,
                        "Sistema inactivo.\n" +
                        "Repara Cámara Básica para iniciar la Cámara de Anclajes."
                    );
                }

                break;

            default:
                SetText(zoneTitleText, "Sector desconocido");
                SetText(zoneDescriptionText, "");
                break;
        }
    }

    private void RefreshNodeList()
    {
        if (nodeListText == null)
            return;

        if (!MachineManager.I.CanAccessZone(_currentZone))
        {
            nodeListText.text = "Sector no accesible.";
            return;
        }

        List<MachineNodeDef> nodes = MachineManager.I.GetDisplayNodesByZone(_currentZone);

        if (nodes == null || nodes.Count == 0)
        {
            nodeListText.text = "No hay nodos configurados para este sector.";
            return;
        }

        ClampSelectedNodeIndex();

        StringBuilder sb = new StringBuilder();

        int visibleRadius = 2;
        int startIndex = Mathf.Max(0, _selectedNodeIndex - visibleRadius);
        int endIndex = Mathf.Min(nodes.Count - 1, _selectedNodeIndex + visibleRadius);

        if (startIndex > 0)
        {
            sb.AppendLine("...");
        }

        for (int i = startIndex; i <= endIndex; i++)
        {
            MachineNodeDef node = nodes[i];

            if (node == null)
                continue;

        bool repaired = MachineManager.I.IsNodeRepaired(node.id);
        bool analyzed = MachineManager.I.IsNodeAnalyzed(node.id);
        bool damageResolved = MachineManager.I.IsNodeDamageResolved(node.id);
        bool damagedWithoutAnalysis = node.damaged && !repaired && !damageResolved && !analyzed;
        bool canRepair = MachineManager.I.CanRepairNode(node.id, out string reason);

        string status = repaired
            ? "REPARADO"
            : damagedWithoutAnalysis
                ? "DAÑADO"
                : canRepair
                    ? "DISPONIBLE"
                    : "BLOQUEADO";

        string damaged = node.damaged && !repaired && !damageResolved ? " | DAÑADO" : "";
        string selectedMarker = i == _selectedNodeIndex ? ">> " : "   ";
        sb.AppendLine(selectedMarker + node.name + damaged);
        sb.AppendLine("Estado: " + status);

        if (!repaired && !damagedWithoutAnalysis)
        {
            MachineNodeCostDef effectiveCost = MachineManager.I.GetEffectiveNodeCost(node);
            sb.AppendLine("Costo: " + FormatCost(effectiveCost));

            if (!canRepair)
                sb.AppendLine("Falta: " + reason);
        }

            sb.AppendLine();
        }

        if (endIndex < nodes.Count - 1)
        {
            sb.AppendLine("...");
        }

        sb.AppendLine("Nodo " + (_selectedNodeIndex + 1) + " / " + nodes.Count);

        nodeListText.text = sb.ToString();
    }


        private List<MachineNodeDef> GetCurrentZoneNodes()
    {
        if (MachineManager.I == null)
            return new List<MachineNodeDef>();

        if (!MachineManager.I.CanAccessZone(_currentZone))
            return new List<MachineNodeDef>();

        List<MachineNodeDef> nodes = MachineManager.I.GetDisplayNodesByZone(_currentZone);

        if (nodes == null)
            return new List<MachineNodeDef>();

        return nodes;
    }

    private void ClampSelectedNodeIndex()
    {
        List<MachineNodeDef> nodes = GetCurrentZoneNodes();

        if (nodes.Count <= 0)
        {
            _selectedNodeIndex = 0;
            return;
        }

        if (_selectedNodeIndex < 0)
            _selectedNodeIndex = 0;

        if (_selectedNodeIndex >= nodes.Count)
            _selectedNodeIndex = nodes.Count - 1;
    }

    private MachineNodeDef GetSelectedNode()
    {
        List<MachineNodeDef> nodes = GetCurrentZoneNodes();

        if (nodes.Count <= 0)
            return null;

        ClampSelectedNodeIndex();

        return nodes[_selectedNodeIndex];
    }

    private void SelectPreviousNode()
    {
        List<MachineNodeDef> nodes = GetCurrentZoneNodes();

        if (nodes.Count <= 0)
            return;

        _selectedNodeIndex--;

        if (_selectedNodeIndex < 0)
            _selectedNodeIndex = nodes.Count - 1;

        _lastAnalyzedNodeId = "";
        SaveSelectedNodeIndexForCurrentZone();

    Refresh();

    }

    private void SelectNextNode()
    {
        List<MachineNodeDef> nodes = GetCurrentZoneNodes();

        if (nodes.Count <= 0)
            return;

        _selectedNodeIndex++;

        if (_selectedNodeIndex >= nodes.Count)
            _selectedNodeIndex = 0;

        _lastAnalyzedNodeId = "";
        SaveSelectedNodeIndexForCurrentZone();

        Refresh();
    }

    private void RepairSelectedNode()
    {
        if (MachineManager.I == null)
            return;

        MachineNodeDef selectedNode = GetSelectedNode();

        if (selectedNode == null)
            return;

        bool selectedNodeHasTierGroup = !string.IsNullOrWhiteSpace(selectedNode.tierGroup);

        bool repaired = MachineManager.I.TryRepairNode(selectedNode.id);

        if (repaired)
        {
            ShowZoneIntroPopupIfNeeded(selectedNode);
            SaveSelectedNodeIndexForCurrentZone();
        }

        Refresh();
    }

    private void ShowZoneIntroPopupIfNeeded(MachineNodeDef repairedNode)
    {
        if (repairedNode == null)
            return;

        if (AchievementPopupUI.I == null)
            return;

        if (repairedNode.id == "z2_fusion_table")
        {
            AchievementPopupUI.I.ShowPopup(
                "Fusiones",
                "Aquí puedes combinar fragmentos experimentales para obtener resultados inestables."
            );
            return;
        }

        if (repairedNode.id == "z4_basic_chamber")
        {
            ShowInstantChamberHelpPopup();
            return;
        }

        if (repairedNode.id == "z3_convergence_channel")
        {
            AchievementPopupUI.I.ShowPopup(
                "Convergencia detectada",
                "La Máquina ha alcanzado una estabilidad suficiente para preparar el Prestigio 1.\n\n" +
                "La apertura dimensional todavía no se ejecutará automáticamente."
            );
            return;
        }
    }

    private void RefreshSelectedNodeControls()
    {
        MachineNodeDef selectedNode = GetSelectedNode();

        if (selectedNode == null)
        {
            SetText(selectedNodeText, "Nodo seleccionado: -");

            if (btnRepairNode != null)
            {
                btnRepairNode.interactable = false;

                TextMeshProUGUI repairButtonText = btnRepairNode.GetComponentInChildren<TextMeshProUGUI>();

                if (repairButtonText != null)
                {
                    repairButtonText.text = "Comprar";
                }
            }

            return;
        }

        bool repaired = MachineManager.I.IsNodeRepaired(selectedNode.id);
        bool analyzed = MachineManager.I.IsNodeAnalyzed(selectedNode.id);
        bool damageResolved = MachineManager.I.IsNodeDamageResolved(selectedNode.id);
        bool damagedWithoutAnalysis = selectedNode.damaged && !repaired && !damageResolved && !analyzed;
        bool canRepair = MachineManager.I.CanRepairNode(selectedNode.id, out string reason);

        if (damagedWithoutAnalysis)
        {
            SetText(selectedNodeText, "Nodo seleccionado: Nodo dañado");
        }
        else
        {
            SetText(selectedNodeText, BuildSelectedNodeInfoText(selectedNode));
        }

        if (btnRepairNode != null)
        {
            btnRepairNode.interactable = !repaired && canRepair;

            TextMeshProUGUI repairButtonText = btnRepairNode.GetComponentInChildren<TextMeshProUGUI>();

            if (repairButtonText != null)
            {
                bool shouldSayRepair = selectedNode.damaged && !damageResolved && MachineManager.I.IsNodeAnalyzed(selectedNode.id);
                repairButtonText.text = shouldSayRepair ? "Reparar" : "Comprar";
            }
        }
    }

    private string BuildSelectedNodeInfoText(MachineNodeDef node)
    {
        if (node == null)
            return "Nodo seleccionado: -";

        return "Nodo seleccionado:\n" + node.name;
    }

    private string BuildSelectedNodeExtraInfoText(MachineNodeDef node)
    {
        if (node == null || MachineManager.I == null)
            return "";

        bool repaired = MachineManager.I.IsNodeRepaired(node.id);

        if (node.id == "z3_machine_memory" && repaired)
        {
            return BuildMachineMemoryProgressSummary();
        }

        return "";
    }

    private string BuildMachineMemoryProgressSummary()
    {
        return
            "Memoria de Máquina\n\n" +
            "Progreso de reparación:\n" +
            BuildZoneProgressLine(MachineZoneType.Room1Link) + "\n" +
            BuildZoneProgressLine(MachineZoneType.FusionSector) + "\n" +
            BuildZoneProgressLine(MachineZoneType.InternalSupport) + "\n" +
            BuildZoneProgressLine(MachineZoneType.InstantChamber);
    }

    private string BuildZoneProgressLine(MachineZoneType zone)
    {
        if (MachineManager.I == null)
            return "";

        List<MachineNodeDef> nodes = MachineManager.I.GetNodesByZone(zone);

        int total = 0;
        int repaired = 0;

        foreach (MachineNodeDef node in nodes)
        {
            if (node == null)
                continue;

            total++;

            if (MachineManager.I.IsNodeRepaired(node.id))
            {
                repaired++;
            }
        }

        return GetZoneDisplayName(zone) + ": " + repaired + "/" + total;
    }

    [ContextMenu("DEBUG: Refresh Machine UI")]
    private void DebugRefresh()
    {
        Refresh();
    }

    private MachineNodeDef GetDiagnosticRecommendedNode(out string reason, out bool canRepair)
    {
        reason = "";
        canRepair = false;

        if (MachineManager.I == null)
        {
            reason = "Máquina no disponible.";
            return null;
        }

        MachineZoneType[] zones =
        {
            MachineZoneType.Room1Link,
            MachineZoneType.FusionSector,
            MachineZoneType.InternalSupport,
            MachineZoneType.InstantChamber
        };

        MachineNodeDef firstPendingNode = null;
        string firstPendingReason = "";

        foreach (MachineZoneType zone in zones)
        {
            List<MachineNodeDef> nodes = MachineManager.I.GetDisplayNodesByZone(zone);

            if (nodes == null)
                continue;

            foreach (MachineNodeDef node in nodes)
            {
                if (node == null)
                    continue;

                if (MachineManager.I.IsNodeRepaired(node.id))
                    continue;

                if (MachineManager.I.CanRepairNode(node.id, out string nodeReason))
                {
                    reason = "Disponible para reparar.";
                    canRepair = true;
                    return node;
                }

                if (firstPendingNode == null)
                {
                    firstPendingNode = node;
                    firstPendingReason = nodeReason;
                }
            }
        }

        if (firstPendingNode != null)
        {
            reason = firstPendingReason;
            canRepair = false;
            return firstPendingNode;
        }

        reason = "No hay nodos pendientes visibles.";
        return null;
    }

    private string BuildDiagnosticRecommendationText(bool failureMarkerActive)
    {
        MachineNodeDef recommendedNode = GetDiagnosticRecommendedNode(
            out string reason,
            out bool canRepair
        );

        if (recommendedNode == null)
            return "\nRecomendación: sin nodos pendientes visibles.";

        string text = "\nRecomendación: " + recommendedNode.name;

        if (failureMarkerActive)
        {
            text += "\nMotivo: " + reason;

            if (canRepair)
                text += "\nEstado: listo para reparar.";
            else
                text += "\nEstado: requiere atención.";
        }

        return text;
    }

    private void RefreshDiagnostics()
    {
        if (machineDiagnosticsText == null)
            return;

        if (MachineManager.I == null || !MachineManager.I.MachineUnlocked)
        {
            machineDiagnosticsText.text = "";
            return;
        }

        if (_currentZone != MachineZoneType.InternalSupport)
        {
            machineDiagnosticsText.text = "";
            return;
        }

        bool diagnosticsUnlocked =
            MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.UnlockDiagnostics) > 0.0;

        if (!diagnosticsUnlocked)
        {
            machineDiagnosticsText.text = "Diagnóstico interno: bloqueado";
            return;
        }

        int totalNodes = GetVisibleMachineNodeCount();
        int repairedNodes = GetVisibleRepairedMachineNodeCount();

        int progressPercent = 0;

        if (totalNodes > 0)
        {
            progressPercent = Mathf.RoundToInt((float)repairedNodes / totalNodes * 100f);
        }

        string fusionPanelState = MachineManager.I.MachineFusionPanelUnlocked ? "activo" : "bloqueado";

        bool failureMarkerActive =
            MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.DiagnosticReasonMarker) > 0.0;

        string failureMarkerState = failureMarkerActive ? "activo" : "bloqueado";

        machineDiagnosticsText.text =
            "Diagnóstico interno: activo\n" +
            "Nodos reparados: " + repairedNodes + " / " + totalNodes + "\n" +
            "Progreso estructural: " + progressPercent + "%\n" +
            "Panel de fusión: " + fusionPanelState + "\n" +
            "Marcador de Fallas: " + failureMarkerState +
            "\n" +
            BuildDiagnosticRecommendationText(failureMarkerActive);
            
    }

    private int GetVisibleMachineNodeCount()
    {
        int total = 0;

        total += GetVisibleNodeCountByZone(MachineZoneType.Room1Link);
        total += GetVisibleNodeCountByZone(MachineZoneType.FusionSector);
        total += GetVisibleNodeCountByZone(MachineZoneType.InternalSupport);
        total += GetVisibleNodeCountByZone(MachineZoneType.InstantChamber);

        return total;
    }

    private int GetVisibleRepairedMachineNodeCount()
    {
        int total = 0;

        total += GetVisibleRepairedNodeCountByZone(MachineZoneType.Room1Link);
        total += GetVisibleRepairedNodeCountByZone(MachineZoneType.FusionSector);
        total += GetVisibleRepairedNodeCountByZone(MachineZoneType.InternalSupport);
        total += GetVisibleRepairedNodeCountByZone(MachineZoneType.InstantChamber);

        return total;
    }

    private int GetVisibleNodeCountByZone(MachineZoneType zone)
    {
        if (MachineManager.I == null)
            return 0;

        List<MachineNodeDef> nodes = MachineManager.I.GetDisplayNodesByZone(zone);

        if (nodes == null)
            return 0;

        return nodes.Count;
    }

    private int GetVisibleRepairedNodeCountByZone(MachineZoneType zone)
    {
        if (MachineManager.I == null)
            return 0;

        List<MachineNodeDef> nodes = MachineManager.I.GetDisplayNodesByZone(zone);

        if (nodes == null)
            return 0;

        int count = 0;

        foreach (MachineNodeDef node in nodes)
        {
            if (node == null)
                continue;

            if (MachineManager.I.IsNodeRepaired(node.id))
                count++;
        }

        return count;
    }

    private bool HasNodeAnalysisUnlocked()
    {
        if (MachineManager.I == null)
            return false;

        return MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.UnlockDiagnostics) > 0.0;
    }

    private void RefreshNodeAnalysis()
    {
        bool unlocked = HasNodeAnalysisUnlocked();
        bool showAnalysisUI = unlocked && !_fusionPanelVisible;

        MachineNodeDef selectedNode = GetSelectedNode();

        bool selectedNodeRepaired =
            selectedNode != null
            && MachineManager.I != null
            && MachineManager.I.IsNodeRepaired(selectedNode.id);

        bool selectedNodeAnalyzed =
            selectedNode != null
            && MachineManager.I != null
            && MachineManager.I.IsNodeAnalyzed(selectedNode.id);

        bool selectedNodeDamageResolved =
            selectedNode != null
            && MachineManager.I != null
            && MachineManager.I.IsNodeDamageResolved(selectedNode.id);

        bool selectedNodeDamaged =
            selectedNode != null
            && selectedNode.damaged
            && !selectedNodeDamageResolved;

        bool isAnalyzingAnyNode =
            !string.IsNullOrWhiteSpace(_analyzingNodeId);

        bool isSelectedNodeBeingAnalyzed =
            selectedNode != null
            && _analyzingNodeId == selectedNode.id;

        bool canAnalyzeSelectedNode =
            selectedNode != null
            && selectedNodeDamaged
            && !selectedNodeRepaired
            && !selectedNodeAnalyzed
            && !isAnalyzingAnyNode;

        if (btnAnalyzeNode != null)
        {
            btnAnalyzeNode.gameObject.SetActive(showAnalysisUI);
            btnAnalyzeNode.interactable = showAnalysisUI && canAnalyzeSelectedNode;
        }

        if (nodeAnalysisText == null)
            return;

        if (!showAnalysisUI || selectedNode == null)
        {
            nodeAnalysisText.text = "";
            return;
        }

        if (isSelectedNodeBeingAnalyzed)
        {
            int seconds = Mathf.CeilToInt(_analysisRemainingSeconds);
            nodeAnalysisText.text = "Analizando nodo...\nTiempo restante: " + seconds + "s";
            return;
        }

        nodeAnalysisText.text = BuildSelectedNodeExtraInfoText(selectedNode);    }

    private void AnalyzeSelectedNode()
    {
        MachineNodeDef selectedNode = GetSelectedNode();

        if (selectedNode == null)
            return;

        if (!HasNodeAnalysisUnlocked())
            return;

        if (!selectedNode.damaged)
            return;

        if (MachineManager.I.IsNodeRepaired(selectedNode.id))
            return;

        if (MachineManager.I.IsNodeAnalyzed(selectedNode.id))
            return;

        if (!string.IsNullOrWhiteSpace(_analyzingNodeId))
            return;

        _analyzingNodeId = selectedNode.id;
        _analysisRemainingSeconds = NodeAnalysisDurationSeconds;
        _lastAnalyzedNodeId = "";

        Refresh();
    }

    private string BuildNodeAnalysisText(MachineNodeDef node)
    {
        if (node == null || MachineManager.I == null)
            return "";

        bool repaired = MachineManager.I.IsNodeRepaired(node.id);

        if (repaired)
            return "";

        if (!node.damaged)
            return "";

        return
            "Análisis estructural:\n" +
            node.name + "\n" +
            "Nodo dañado detectado.\n" +
            "Requiere estabilización experimental.";
    }

    private string GetZoneDisplayName(MachineZoneType zone)
    {
        switch (zone)
        {
            case MachineZoneType.Room1Link:
                return "Enlace con el Cuarto 1";

            case MachineZoneType.FusionSector:
                return MachineManager.I != null && MachineManager.I.MachineAllZonesUnlocked
                    ? "Sector de Fusiones"
                    : "Sector activo";

            case MachineZoneType.InternalSupport:
                return "Soporte Interno";

            case MachineZoneType.InstantChamber:
                return "Cámara de Anclajes";

            default:
                return "Zona desconocida";
        }
    }

    private void RefreshNotice()
    {
        if (machineNoticeText == null)
            return;

        if (MachineManager.I == null)
        {
            machineNoticeText.text = "";
            return;
        }

        if (!MachineManager.I.MachineUnlocked)
        {
            machineNoticeText.text = "La estructura de la Máquina aún no responde.";
            return;
        }

        if (MachineManager.I.Prestige1Prepared)
        {
            machineNoticeText.text = "Convergencia estable: apertura dimensional preparada. Falta conectar el sistema definitivo.";
            return;
        }

        if (MachineManager.I.InstantChamberCoreUnlocked)
        {
            machineNoticeText.text = "Cámara activa.";
            return;
        }

        if (MachineManager.I.MachineAllZonesUnlocked)
        {
            machineNoticeText.text = "La Máquina ha estabilizado nuevas secciones.";
            return;
        }

        if (MachineManager.I.MachineFusionPanelUnlocked)
        {
            machineNoticeText.text = "Señal detectada: el Panel de Fusión está disponible. Usa fusiones para obtener materiales experimentales.";
            return;
        }

        machineNoticeText.text = "Repara los primeros módulos del sector activo.";
    }

    private void RefreshFusionPanelVisibility()
    {
        bool fusionUnlocked = MachineManager.I != null
            && MachineManager.I.MachineFusionPanelUnlocked;

        if (!fusionUnlocked)
            _fusionPanelVisible = false;

        if (legacyFusionPanel != null)
            legacyFusionPanel.SetActive(fusionUnlocked && _fusionPanelVisible);

        if (machineRepairViewRoot != null)
            machineRepairViewRoot.SetActive(!_fusionPanelVisible);

        if (machineNodeViewRoot != null)
            machineNodeViewRoot.SetActive(!_fusionPanelVisible);

        bool seedsViewOpen = instantSeedsViewRoot != null && instantSeedsViewRoot.activeSelf;

        if (btnFusionPanel != null)
        {
            btnFusionPanel.gameObject.SetActive(fusionUnlocked && !_fusionPanelVisible && !seedsViewOpen);
            btnFusionPanel.interactable = fusionUnlocked;

            TextMeshProUGUI labelText = btnFusionPanel.GetComponentInChildren<TextMeshProUGUI>();

            if (labelText != null)
                labelText.text = fusionUnlocked ? "Panel de Fusión" : "???";
        }

        if (btnBackToNodesFromFusion != null)
        {
            btnBackToNodesFromFusion.gameObject.SetActive(fusionUnlocked && _fusionPanelVisible);

            TextMeshProUGUI labelText = btnBackToNodesFromFusion.GetComponentInChildren<TextMeshProUGUI>();

            if (labelText != null)
                labelText.text = "Ver nodos";
        }
    }

    private void OpenFusionPanel()
    {

        if (btnInstantChamberHelp != null)
            btnInstantChamberHelp.gameObject.SetActive(false);

        if (MachineManager.I == null)
            return;

        if (!MachineManager.I.MachineFusionPanelUnlocked)
        {
            _fusionPanelVisible = false;
            Refresh();
            return;
        }

        _fusionPanelVisible = true;

        if (instantSeedsViewRoot != null)
            instantSeedsViewRoot.SetActive(false);

        Refresh();
    }

    private void CloseFusionPanel()
    {
        _fusionPanelVisible = false;
        Refresh();
    }

    private string FormatCost(MachineNodeCostDef cost)
    {
        if (cost == null)
            return "sin costo";

        List<string> parts = new List<string>();

        if (cost.le > 0)
            parts.Add(cost.le.ToString("0") + " LE");

        if (cost.traces > 0)
            parts.Add(cost.traces.ToString("0") + " Trazas");

        if (cost.hallazgo > 0)
            parts.Add(cost.hallazgo + " Hallazgo");

        if (cost.muestra > 0)
            parts.Add(cost.muestra + " Muestra");

        if (cost.lecturaIncompleta > 0)
            parts.Add(cost.lecturaIncompleta + " Lectura Incompleta");

        if (cost.compuestoUtil > 0)
            parts.Add(cost.compuestoUtil + " Compuesto Útil");

        if (cost.pureInstant > 0)
            parts.Add(cost.pureInstant + " Anclaje Puro");

        if (cost.stableInstant > 0)
            parts.Add(cost.stableInstant + " Anclaje Estable");

        if (cost.forcedInstant > 0)
            parts.Add(cost.forcedInstant + " Anclaje Forzado");

        if (parts.Count == 0)
            return "sin costo";

        return string.Join(" + ", parts);
    }

    private void SetText(TextMeshProUGUI text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private void RefreshContextHelpButton()
    {
        bool seedsViewOpen = instantSeedsViewRoot != null && instantSeedsViewRoot.activeSelf;
        bool showHelp = seedsViewOpen || _fusionPanelVisible;

        if (btnInstantChamberHelp != null)
        {
            btnInstantChamberHelp.gameObject.SetActive(showHelp);

            TextMeshProUGUI labelText = btnInstantChamberHelp.GetComponentInChildren<TextMeshProUGUI>();
            if (labelText != null)
                labelText.text = "?";
        }
    }

    private void RefreshInstantChamberButtons()
    {
        bool instantChamberUnlocked =
            MachineManager.I != null && MachineManager.I.InstantChamberCoreUnlocked;

        bool seedsViewOpen = instantSeedsViewRoot != null && instantSeedsViewRoot.activeSelf;

        bool showSeedsButton =
            instantChamberUnlocked &&
            !_fusionPanelVisible &&
            !seedsViewOpen;

        if (btnOpenSeedsPanel != null)
        {
            btnOpenSeedsPanel.gameObject.SetActive(showSeedsButton);

            TextMeshProUGUI labelText = btnOpenSeedsPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (labelText != null)
                labelText.text = "Semillas";
        }
    }

    private void ShowContextHelpPopup()
    {
        bool seedsViewOpen = instantSeedsViewRoot != null && instantSeedsViewRoot.activeSelf;

        if (seedsViewOpen)
        {
            ShowInstantChamberHelpPopup();
            return;
        }

        if (_fusionPanelVisible)
        {
            ShowFusionPanelHelpPopup();
            return;
        }
    }

    private void ShowInstantChamberHelpPopup()
    {
        if (AchievementPopupUI.I == null)
            return;

        AchievementPopupUI.I.ShowPopup(
            "Cámara de Anclajes",
            "Crea Semillas Dimensionales y déjalas madurar.\n\n" +
            "Cuando una Semilla Dimensional madura, puedes formar un Anclaje Inestable.\n\n" +
            "Luego puedes estabilizarlo, compensar tensión, fijarlo y guardarlo en el Archivo."
        );
    }

    private void ShowFusionPanelHelpPopup()
    {
        if (AchievementPopupUI.I == null)
            return;

        AchievementPopupUI.I.ShowPopup(
            "Panel de Fusión",
            "Combina dos fragmentos y un catalizador para obtener materiales experimentales.\n\n" +
            "El modo de ensayo cambia el riesgo y la recompensa.\n\n" +
            "Síntesis Guiada permite elegir una intención de resultado, pero no garantiza que salga."
        );
    }

    private void FormInstantPlaceholder()
    {
        if (GameState.I == null)
            return;

        if (GameState.I.chronalMatureSeedsStored <= 0)
        {
            if (AchievementPopupUI.I != null)
            {
                AchievementPopupUI.I.ShowPopup(
                    "Anclajes",
                    "No hay Semillas Dimensionales maduras disponibles."
                );
            }

            return;
        }

        if (GameState.I.chronalInstant != null && GameState.I.chronalInstant.hasInstant)
        {
            if (AchievementPopupUI.I != null)
            {
                AchievementPopupUI.I.ShowPopup(
                    "Anclajes",
                    "Ya hay un Anclaje Inestable activo."
                );
            }

            return;
        }

        bool formed = GameState.I.TryFormChronalInstant();

        if (formed && AchievementPopupUI.I != null)
        {
            AchievementPopupUI.I.ShowPopup(
                "Anclaje formado",
                "Una Semilla Dimensional madura se convirtió en un Anclaje Inestable."
            );
        }

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshSeedsView();
    }

    private double GetSelectedStabilizationIntensity()
    {
        if (stabilizationIntensitySlider == null)
            return 50.0;

        double value = stabilizationIntensitySlider.value;

        if (GameState.I != null)
            return GameState.I.NormalizeChronalStabilizationIntensity(value);

        return value;
    }

    private void StabilizeInstant()
    {
        if (GameState.I == null)
            return;

        double intensity = GetSelectedStabilizationIntensity();
        bool done = GameState.I.TryStabilizeChronalInstant(intensity);

        if (!done && AchievementPopupUI.I != null)
        {
            AchievementPopupUI.I.ShowPopup(
                "Anclajes",
                "No hay Anclaje activo para estabilizar."
            );
        }

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshSeedsView();
    }

    private void RewindInstant()
    {
        if (GameState.I == null)
            return;

        bool done = GameState.I.TryRewindChronalInstant();

        if (!done && AchievementPopupUI.I != null)
        {
            AchievementPopupUI.I.ShowPopup(
                "Anclajes",
                "No hay Anclaje activo para compensar."
            );
        }

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshSeedsView();
    }

    private void MaterializeInstant()
    {
        if (GameState.I == null)
            return;

        bool done = GameState.I.TryMaterializeChronalInstant();

        if (done)
        {
            if (AchievementPopupUI.I != null)
            {
                AchievementPopupUI.I.ShowPopup(
                    "Anclaje fijado",
                    "Resultado obtenido: " + GameState.I.lastChronalMaterializationQuality
                );
            }
        }
        else
        {
            if (AchievementPopupUI.I != null)
            {
            string message;

            if (GameState.I.IsChronalArchiveFull())
            {
                message = "El Archivo de Anclajes está lleno.";
            }
            else
            {
                message =
                    "Necesitas un Anclaje activo con al menos " +
                    GameState.I.GetChronalMaterializationThreshold().ToString("0") +
                    "% de estabilidad.";
            }

            AchievementPopupUI.I.ShowPopup(
                "Anclajes",
                message
            );
            }
        }

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshSeedsView();
    }

    private void DiscardInstant()
    {
        if (GameState.I == null)
            return;

        bool done = GameState.I.TryDiscardChronalInstant();

        if (done && AchievementPopupUI.I != null)
        {
            AchievementPopupUI.I.ShowPopup(
                "Anclaje descartado",
                "El Anclaje Inestable fue eliminado."
            );
        }

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshSeedsView();
    }

    private void OpenSeedsPanelPlaceholder()
    {
        OpenSeedsView();
    }

    private void OpenSeedsView()
    {
        _fusionPanelVisible = false;

        if (legacyFusionPanel != null)
            legacyFusionPanel.SetActive(false);

        if (machineRepairViewRoot != null)
            machineRepairViewRoot.SetActive(false);

        if (instantSeedsViewRoot != null)
            instantSeedsViewRoot.SetActive(true);

        RefreshContextHelpButton();

        if (btnFusionPanel != null)
            btnFusionPanel.gameObject.SetActive(false);

        if (btnBackToNodesFromFusion != null)
            btnBackToNodesFromFusion.gameObject.SetActive(false);

        if (btnAnalyzeNode != null)
            btnAnalyzeNode.gameObject.SetActive(false);

        if (machineDiagnosticsText != null)
            machineDiagnosticsText.gameObject.SetActive(false);

        if (nodeAnalysisText != null)
            nodeAnalysisText.gameObject.SetActive(false);

        if (GameState.I != null)
            GameState.I.EnsureChronalSeedSlots();

        RefreshSeedsView();
    }

    private void CloseSeedsView()
    {
        if (instantSeedsViewRoot != null)
            instantSeedsViewRoot.SetActive(false);

        if (machineRepairViewRoot != null)
            machineRepairViewRoot.SetActive(true);

        if (btnInstantChamberHelp != null)
            btnInstantChamberHelp.gameObject.SetActive(false);

        if (machineDiagnosticsText != null)
            machineDiagnosticsText.gameObject.SetActive(true);

        if (nodeAnalysisText != null)
            nodeAnalysisText.gameObject.SetActive(true);

        if (btnAnalyzeNode != null)
            btnAnalyzeNode.gameObject.SetActive(true);

        Refresh();
    }

    private void CreateChronalSeed()
    {
        if (GameState.I == null)
            return;

        bool created = GameState.I.TryCreateChronalSeed();

        if (!created && AchievementPopupUI.I != null)
        {
            AchievementPopupUI.I.ShowPopup(
                "Semillas Dimensionales",
                "No hay slots disponibles."
            );
        }

        if (SaveService.I != null)
            SaveService.I.Save();

        RefreshSeedsView();
    }

    private void RefreshSeedsView()
    {
        if (GameState.I == null)
            return;

            GameState.I.EnsureChronalSeedSlots();

        if (seedsSlotsText != null)
        {
            seedsSlotsText.text =
                GameState.I.GetChronalSeedsStatusText() +
                "\n" +
                GameState.I.GetChronalInstantStatusText();
        }

        if (btnFormInstant != null)
        {
            bool hasMatureSeed = GameState.I.chronalMatureSeedsStored > 0;
            bool hasActiveInstant =
                GameState.I.chronalInstant != null &&
                GameState.I.chronalInstant.hasInstant;

            btnFormInstant.interactable = hasMatureSeed && !hasActiveInstant;
        }

            bool hasInstant =
            GameState.I.chronalInstant != null &&
            GameState.I.chronalInstant.hasInstant;

        double stability = hasInstant ? GameState.I.chronalInstant.stability : 0.0;
        double materializationThreshold = GameState.I.GetChronalMaterializationThreshold();

        if (stabilizationPreviewText != null)
        {
            double intensity = GetSelectedStabilizationIntensity();
            stabilizationPreviewText.text = GameState.I.GetChronalStabilizationPreviewText(intensity);
        }

        if (stabilizationIntensitySlider != null)
            stabilizationIntensitySlider.interactable = hasInstant;

        if (btnStabilizeInstant != null)
            btnStabilizeInstant.interactable = hasInstant;

        if (btnRewindInstant != null)
            btnRewindInstant.interactable = hasInstant;

        if (btnMaterializeInstant != null)
            btnMaterializeInstant.interactable =
            hasInstant &&
            stability >= materializationThreshold &&
            !GameState.I.IsChronalArchiveFull();

        if (btnDiscardInstant != null)
            btnDiscardInstant.interactable = hasInstant;
    }
}