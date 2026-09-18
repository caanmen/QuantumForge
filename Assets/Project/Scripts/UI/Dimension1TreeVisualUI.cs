using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Propietario único de los datos y estados visuales del Árbol Cuántico D1.
/// La composición la reconstruye Dimension1TreeReferenceSetup; este componente
/// conecta esa composición con Dimension1System y GameState.
/// </summary>
public sealed class Dimension1TreeVisualUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] metalAmounts;
    [SerializeField] private TMP_Text[] metalRates;
    [SerializeField] private TMP_Text availablePoints;
    [SerializeField] private Button[] nodeButtons;
    [SerializeField] private Image[] nodeFills;
    [SerializeField] private Image[] nodeBorders;
    [SerializeField] private Image[] nodeSelectionGlows;
    [SerializeField] private GameObject[] nodeLockOverlays;
    [SerializeField] private TMP_Text[] nodeNames;
    [SerializeField] private TMP_Text[] nodeNumbers;
    [SerializeField] private TMP_Text[] nodeProgress;
    [SerializeField] private GameObject[] detailIconRoots;
    [SerializeField] private TMP_Text selectedName;
    [SerializeField] private TMP_Text selectedLevel;
    [SerializeField] private TMP_Text effectPrimary;
    [SerializeField] private TMP_Text effectSecondary;
    [SerializeField] private TMP_Text costValue;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionLabel;
    [SerializeField] private Image actionFill;
    [SerializeField] private Image actionBorder;
    [SerializeField] private bool referencePreviewForVisualQa;

    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanBright = Hex("8DEAFF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9EABB4");
    private static readonly Color Locked = Hex("687680");
    private static readonly Color Fill = Hex("03121C", 247);
    private static readonly Color AmberFill = Hex("171105", 249);
    private static readonly Color LockedFill = Hex("071016", 249);

    private static readonly int[] PreviewDefaults = { 1, 1, 1, 1, 2, 1, 1, 0, 0, 0 };
    private readonly int[] previewTiers = new int[10];

    private int selectedNodeIndex = 8;
    private int previewPoints = 3;
    private float refreshTimer;

    public int SelectedNodeIndex => selectedNodeIndex;

    public void Configure(
        TMP_Text[] configuredMetalAmounts,
        TMP_Text[] configuredMetalRates,
        TMP_Text configuredAvailablePoints,
        Button[] configuredNodeButtons,
        Image[] configuredNodeFills,
        Image[] configuredNodeBorders,
        Image[] configuredNodeSelectionGlows,
        GameObject[] configuredNodeLockOverlays,
        TMP_Text[] configuredNodeNames,
        TMP_Text[] configuredNodeNumbers,
        TMP_Text[] configuredNodeProgress,
        GameObject[] configuredDetailIconRoots,
        TMP_Text configuredSelectedName,
        TMP_Text configuredSelectedLevel,
        TMP_Text configuredEffectPrimary,
        TMP_Text configuredEffectSecondary,
        TMP_Text configuredCostValue,
        Button configuredActionButton,
        TMP_Text configuredActionLabel,
        Image configuredActionFill,
        Image configuredActionBorder)
    {
        metalAmounts = configuredMetalAmounts;
        metalRates = configuredMetalRates;
        availablePoints = configuredAvailablePoints;
        nodeButtons = configuredNodeButtons;
        nodeFills = configuredNodeFills;
        nodeBorders = configuredNodeBorders;
        nodeSelectionGlows = configuredNodeSelectionGlows;
        nodeLockOverlays = configuredNodeLockOverlays;
        nodeNames = configuredNodeNames;
        nodeNumbers = configuredNodeNumbers;
        nodeProgress = configuredNodeProgress;
        detailIconRoots = configuredDetailIconRoots;
        selectedName = configuredSelectedName;
        selectedLevel = configuredSelectedLevel;
        effectPrimary = configuredEffectPrimary;
        effectSecondary = configuredEffectSecondary;
        costValue = configuredCostValue;
        actionButton = configuredActionButton;
        actionLabel = configuredActionLabel;
        actionFill = configuredActionFill;
        actionBorder = configuredActionBorder;
    }

    public void SetReferencePreviewForVisualQa(bool enabled)
    {
        referencePreviewForVisualQa = enabled;
        selectedNodeIndex = 8;
        previewPoints = 3;
        for (int i = 0; i < previewTiers.Length; i++)
            previewTiers[i] = PreviewDefaults[i];
        Refresh();
    }

    public void SelectNode(int index)
    {
        if (index < 0 || index >= Dimension1System.Dimension1TreeNodeIds.Length)
            return;
        selectedNodeIndex = index;
        Refresh();
    }

    public void BuySelectedNode()
    {
        string nodeId = SelectedNodeId;
        if (string.IsNullOrEmpty(nodeId)) return;

        if (referencePreviewForVisualQa)
        {
            int tier = GetTier(null, selectedNodeIndex);
            int maxTier = Dimension1System.GetDimension1TreeNodeMaxTier(nodeId);
            if (tier >= maxTier) return;
            int cost = Dimension1System.GetDimension1TreeNodeCost(nodeId, tier + 1);
            if (previewPoints < cost) return;
            previewPoints -= cost;
            previewTiers[selectedNodeIndex] = tier + 1;
            Refresh();
            return;
        }

        GameState state = GameState.I;
        if (state == null || !Dimension1System.TryBuyDimension1TreeNode(state, nodeId))
        {
            Refresh();
            return;
        }

        if (SaveService.I != null) SaveService.I.Save();
        Refresh();
    }

    private string SelectedNodeId =>
        selectedNodeIndex >= 0 && selectedNodeIndex < Dimension1System.Dimension1TreeNodeIds.Length
            ? Dimension1System.Dimension1TreeNodeIds[selectedNodeIndex]
            : string.Empty;

    private void OnEnable()
    {
        selectedNodeIndex = Mathf.Clamp(selectedNodeIndex, 0,
            Dimension1System.Dimension1TreeNodeIds.Length - 1);
        refreshTimer = 0f;
        Refresh();
    }

    private void Update()
    {
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = .25f;
        Refresh();
    }

    private void Refresh()
    {
        GameState state = referencePreviewForVisualQa ? null : GameState.I;
        if (!referencePreviewForVisualQa && state == null) return;
        if (state != null) state.EnsureDimension1State();

        RefreshMetals(state);
        int points = referencePreviewForVisualQa ? previewPoints : Mathf.Max(0, state.d1TreePoints);
        if (availablePoints != null) availablePoints.text = points.ToString();

        for (int i = 0; i < Dimension1System.Dimension1TreeNodeIds.Length; i++)
            RefreshNode(state, points, i);
        RefreshDetail(state, points);
    }

    private void RefreshMetals(GameState state)
    {
        string[] previewAmounts = { "5.98M", "4.73M", "4.70M" };
        string[] previewRates = { "+0.30/s", "+0.08/s", "+0.03/s" };
        if (!referencePreviewForVisualQa)
        {
            Dimension1HeaderMetalsUI.Refresh(transform, state, state.dimension1SelectedSectorId);
            return;
        }
        for (int i = 0; i < 3; i++)
        {
            Set(metalAmounts, i, previewAmounts[i]);
            Set(metalRates, i, previewRates[i]);
        }
    }

    private void RefreshNode(GameState state, int points, int index)
    {
        string nodeId = Dimension1System.Dimension1TreeNodeIds[index];
        int tier = GetTier(state, index);
        int maxTier = Dimension1System.GetDimension1TreeNodeMaxTier(nodeId);
        int targetTier = Mathf.Min(tier + 1, maxTier);
        bool selected = index == selectedNodeIndex;
        bool complete = tier >= maxTier;
        bool prerequisite = referencePreviewForVisualQa || complete ||
            Dimension1System.MeetsDimension1TreeNodePrerequisite(state, nodeId, targetTier);
        bool blocked = !complete && !prerequisite;

        Set(nodeNames, index, Dimension1System.GetDimension1TreeNodeVisualName(nodeId).ToUpperInvariant());
        Set(nodeNumbers, index, (index + 1).ToString());
        Set(nodeProgress, index, tier + " / " + maxTier);
        SetActive(nodeSelectionGlows, index, selected);
        SetActive(nodeLockOverlays, index, blocked);

        Color accent = selected ? Amber : blocked ? Locked : Cyan;
        SetColor(nodeBorders, index, accent);
        SetColor(nodeFills, index, selected ? AmberFill : blocked ? LockedFill : Fill);
        SetColor(nodeNumbers, index, accent);
        SetColor(nodeProgress, index, accent);
        SetColor(nodeNames, index, selected ? Amber : blocked ? Locked : Primary);
        if (nodeButtons != null && index < nodeButtons.Length && nodeButtons[index] != null)
            nodeButtons[index].interactable = true;
    }

    private void RefreshDetail(GameState state, int points)
    {
        string nodeId = SelectedNodeId;
        if (string.IsNullOrEmpty(nodeId)) return;
        int tier = GetTier(state, selectedNodeIndex);
        int maxTier = Dimension1System.GetDimension1TreeNodeMaxTier(nodeId);
        int targetTier = Mathf.Min(tier + 1, maxTier);
        bool complete = tier >= maxTier;
        bool prerequisite = referencePreviewForVisualQa || complete ||
            Dimension1System.MeetsDimension1TreeNodePrerequisite(state, nodeId, targetTier);
        int cost = complete ? 0 : Dimension1System.GetDimension1TreeNodeCost(nodeId, targetTier);
        bool canBuy = !complete && prerequisite && points >= cost;

        if (detailIconRoots != null)
            for (int i = 0; i < detailIconRoots.Length; i++)
                if (detailIconRoots[i] != null) detailIconRoots[i].SetActive(i == selectedNodeIndex);

        if (selectedName != null)
            selectedName.text = Dimension1System.GetDimension1TreeNodeVisualName(nodeId).ToUpperInvariant();
        if (selectedLevel != null) selectedLevel.text = "NIVEL " + tier + " / " + maxTier;

        GetEffectLines(nodeId, state, targetTier, out string primary, out string secondary);
        if (effectPrimary != null) effectPrimary.text = primary;
        if (effectSecondary != null) effectSecondary.text = secondary;
        if (costValue != null)
            costValue.text = complete ? "COMPLETADO" : cost + (cost == 1 ? " PUNTO" : " PUNTOS");

        string label;
        if (complete) label = "MÁXIMO";
        else if (!prerequisite) label = "REQUISITOS PENDIENTES";
        else if (points < cost) label = "FALTAN PUNTOS";
        else label = tier == 0 ? "DESBLOQUEAR" : "MEJORAR NIVEL " + targetTier;
        if (actionLabel != null) actionLabel.text = label;
        if (actionButton != null) actionButton.interactable = canBuy;
        if (actionFill != null) actionFill.color = canBuy ? AmberFill : LockedFill;
        if (actionBorder != null) actionBorder.color = canBuy ? Amber : Locked;
        if (actionLabel != null) actionLabel.color = canBuy ? Amber : Locked;
    }

    private static void GetEffectLines(string nodeId, GameState state, int targetTier,
        out string primary, out string secondary)
    {
        string description = Dimension1System.GetDimension1TreeNodeDescription(nodeId)
            .Replace("\n\n", " ").Replace("\n", " ").Trim();
        int separator = description.IndexOf(" y ", System.StringComparison.OrdinalIgnoreCase);
        if (separator > 0 && description.Length < 130)
        {
            primary = description.Substring(0, separator).Trim().ToUpperInvariant();
            secondary = description.Substring(separator + 3).Trim().ToUpperInvariant();
            return;
        }

        int colon = description.IndexOf(':');
        if (colon > 0 && description.Length < 130)
        {
            primary = description.Substring(0, colon).Trim().ToUpperInvariant();
            secondary = description.Substring(colon + 1).Trim().ToUpperInvariant();
            return;
        }

        if (nodeId == Dimension1System.D1TreeFleetCoordination)
        {
            primary = "PERMITE ENVIAR 2 NAVES A UNA EXPLORACIÓN";
            secondary = "DURACIÓN ×2.5 · METALES ×4.0 · RELIQUIA +8%";
            return;
        }

        primary = description.ToUpperInvariant();
        secondary = Dimension1System.GetDimension1TreeNodeUnlockSummary(state, nodeId, targetTier)
            .ToUpperInvariant();
    }

    private int GetTier(GameState state, int index)
    {
        if (referencePreviewForVisualQa)
            return index >= 0 && index < previewTiers.Length ? previewTiers[index] : 0;
        return state.GetD1TreeNodeTier(Dimension1System.Dimension1TreeNodeIds[index]);
    }

    private static string FormatAmount(double value)
    {
        value = System.Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        return value.ToString(value >= 10d ? "0" : "0.##");
    }

    private static void Set(TMP_Text[] values, int index, string value)
    {
        if (values != null && index >= 0 && index < values.Length && values[index] != null)
            values[index].text = value;
    }

    private static void SetColor(TMP_Text[] values, int index, Color color)
    {
        if (values != null && index >= 0 && index < values.Length && values[index] != null)
            values[index].color = color;
    }

    private static void SetColor(Image[] values, int index, Color color)
    {
        if (values != null && index >= 0 && index < values.Length && values[index] != null)
            values[index].color = color;
    }

    private static void SetActive(Image[] values, int index, bool active)
    {
        if (values != null && index >= 0 && index < values.Length && values[index] != null)
            values[index].gameObject.SetActive(active);
    }

    private static void SetActive(GameObject[] values, int index, bool active)
    {
        if (values != null && index >= 0 && index < values.Length && values[index] != null)
            values[index].SetActive(active);
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
