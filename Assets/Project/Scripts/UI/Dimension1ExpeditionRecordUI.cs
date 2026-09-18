using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presentación visual del historial real de expediciones D1. No crea ni altera registros.
/// </summary>
public sealed class Dimension1ExpeditionRecordUI : MonoBehaviour
{
    [Serializable]
    public sealed class RowView
    {
        public GameObject root;
        public RectTransform rect;
        public Image preview;
        public GameObject centralAccessIcon;
        public TMP_Text indexText;
        public TMP_Text destinationText;
        public TMP_Text sectorText;
        public Image mainShipIcon;
        public Image supportShipIcon;
        public TMP_Text shipText;
        public TMP_Text synergyText;
        public GameObject completedIcon;
        public GameObject coordinatedIcon;
        public TMP_Text statusText;
        public TMP_Text rewardsText;
        public GameObject[] rewardBadges;
        public TMP_Text[] rewardBadgeTexts;
        public GameObject extraBadge;
        public TMP_Text extraBadgeText;
    }

    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button backButton;
    [SerializeField] private Button bottomBackButton;
    [SerializeField] private Button allMetalsButton;
    [SerializeField] private Button[] filterButtons;
    [SerializeField] private Image[] filterBorders;
    [SerializeField] private Image[] filterFills;
    [SerializeField] private TMP_Text[] filterLabels;
    [SerializeField] private TMP_Text recentCountText;
    [SerializeField] private TMP_Text emptyText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RowView[] rows;
    [SerializeField] private TMP_Text totalExpeditionsText;
    [SerializeField] private TMP_Text totalRelicsText;
    [SerializeField] private TMP_Text totalMetalsText;
    [SerializeField] private TMP_Text[] headerAmounts;
    [SerializeField] private TMP_Text[] headerRates;
    [SerializeField] private Sprite[] destinationSprites;
    [SerializeField] private Sprite fallbackDestinationSprite;
    [SerializeField] private Sprite[] shipSprites;

    private static readonly string[] DestinationIds =
    {
        Dimension1System.DestinationMineralBelt,
        Dimension1System.DestinationShipGraveyard,
        Dimension1System.DestinationDriftingProbes,
        Dimension1System.DestinationAbandonedShip,
        Dimension1System.DestinationOrbitalRuin,
        Dimension1System.DestinationLaboratory,
        Dimension1System.DestinationAbandonedStation,
        Dimension1System.DestinationMinorAnomaly,
        Dimension1System.DestinationAncientStructure,
        Dimension1System.DestinationUnstableZone
    };

    private static readonly string[] ShipIds =
    {
        Dimension1System.ShipLightProbe,
        Dimension1System.ShipExtractorDrone,
        Dimension1System.ShipAnalyticProbe,
        Dimension1System.ShipCargoShip
    };

    private static readonly string[] SectorIds =
    {
        Dimension1System.Sector01OuterRim,
        Dimension1System.Sector02DebrisRing,
        Dimension1System.Sector03AncientOrbits,
        Dimension1System.Sector04SilentFrontier
    };

    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanMuted = Hex("087FA9", 230);
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color AmberFill = Hex("1A1407", 254);
    private static readonly Color NormalFill = Hex("031019", 252);
    private static readonly Color Primary = Hex("EAF1F4");
    private static readonly Color Secondary = Hex("99A6B0");

    private GameState currentState;
    private int filterIndex;
    private int sectorIndex;
    private bool visible;
    private float refreshTimer;

    public void Configure(
        Dimension1PanelUI configuredPanel, CanvasGroup configuredCanvasGroup,
        Button configuredBack, Button configuredBottomBack, Button configuredAllMetals,
        Button[] configuredFilters, Image[] configuredFilterBorders,
        Image[] configuredFilterFills, TMP_Text[] configuredFilterLabels,
        TMP_Text configuredRecentCount, TMP_Text configuredEmpty,
        ScrollRect configuredScroll, RectTransform configuredContent,
        RowView[] configuredRows, TMP_Text configuredTotalExpeditions,
        TMP_Text configuredTotalRelics, TMP_Text configuredTotalMetals,
        TMP_Text[] configuredHeaderAmounts, TMP_Text[] configuredHeaderRates,
        Sprite[] configuredDestinationSprites, Sprite configuredFallbackDestination,
        Sprite[] configuredShipSprites)
    {
        panel = configuredPanel;
        canvasGroup = configuredCanvasGroup;
        backButton = configuredBack;
        bottomBackButton = configuredBottomBack;
        allMetalsButton = configuredAllMetals;
        filterButtons = configuredFilters;
        filterBorders = configuredFilterBorders;
        filterFills = configuredFilterFills;
        filterLabels = configuredFilterLabels;
        recentCountText = configuredRecentCount;
        emptyText = configuredEmpty;
        scrollRect = configuredScroll;
        content = configuredContent;
        rows = configuredRows;
        totalExpeditionsText = configuredTotalExpeditions;
        totalRelicsText = configuredTotalRelics;
        totalMetalsText = configuredTotalMetals;
        headerAmounts = configuredHeaderAmounts;
        headerRates = configuredHeaderRates;
        destinationSprites = configuredDestinationSprites;
        fallbackDestinationSprite = configuredFallbackDestination;
        shipSprites = configuredShipSprites;
    }

    private void Awake() => SetVisible(false);

    private void Update()
    {
        if (!visible) return;
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = .5f;
        RefreshData(false);
    }

    public void RefreshFromState(GameState state, bool shouldShow)
    {
        currentState = state;
        if (!shouldShow || state == null)
        {
            SetVisible(false);
            return;
        }

        transform.SetAsLastSibling();
        SetVisible(true);
        RefreshData(false);
    }

    public void OpenFromState(GameState state)
    {
        if (state == null) return;
        currentState = state;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        transform.SetAsLastSibling();
        SetVisible(true);
        RefreshData(true);
    }

    public void Close()
    {
        SetVisible(false);
        if (panel != null) panel.OnClickCloseExplorationRecord();
    }

    public void OpenMetals()
    {
        if (Dimension1MetalsInventoryUI.I != null) Dimension1MetalsInventoryUI.I.Open();
    }

    public void SelectAll() => SetFilter(0);
    public void SelectRelics() => SetFilter(1);
    public void SelectCoordinated() => SetFilter(2);

    public void CycleSector()
    {
        if (filterIndex != 3) sectorIndex = 0;
        else sectorIndex = (sectorIndex + 1) % SectorIds.Length;
        filterIndex = 3;
        RefreshData(true);
    }

    private void SetFilter(int index)
    {
        filterIndex = Mathf.Clamp(index, 0, 3);
        RefreshData(true);
    }

    private void RefreshData(bool resetScroll)
    {
        if (currentState == null) return;
        currentState.EnsureDimension1State();
        RefreshHeader();
        RefreshFilterAppearance();

        List<D1ExplorationRecordEntry> all = currentState.dimension1RecentExplorationRecords ??
            new List<D1ExplorationRecordEntry>();
        int totalCount = 0;
        int relicCount = 0;
        double metalTotal = 0d;
        foreach (D1ExplorationRecordEntry entry in all)
        {
            if (!IsDisplayable(entry)) continue;
            totalCount++;
            if (HasRelic(entry)) relicCount++;
            metalTotal += SumMetals(entry);
        }

        if (recentCountText != null)
            recentCountText.text = totalCount == 1 ? "1 REGISTRO RECIENTE" : totalCount + " REGISTROS RECIENTES";
        if (totalExpeditionsText != null) totalExpeditionsText.text = totalCount.ToString();
        if (totalRelicsText != null) totalRelicsText.text = relicCount.ToString();
        if (totalMetalsText != null) totalMetalsText.text = FormatAmount(metalTotal);

        int visibleRows = 0;
        if (currentState.dimension1CentralAccessKeyObtained && filterIndex == 0 && visibleRows < RowCapacity)
            ConfigureCentralKeyRow(rows[visibleRows++]);

        for (int i = all.Count - 1; i >= 0 && visibleRows < RowCapacity; i--)
        {
            D1ExplorationRecordEntry entry = all[i];
            if (!IsDisplayable(entry) || !MatchesFilter(entry)) continue;
            ConfigureRow(rows[visibleRows++], entry);
        }

        for (int i = visibleRows; rows != null && i < rows.Length; i++)
            if (rows[i]?.root != null) rows[i].root.SetActive(false);

        bool hasVisible = visibleRows > 0;
        if (emptyText != null)
        {
            emptyText.gameObject.SetActive(!hasVisible);
            emptyText.text = totalCount == 0
                ? "SIN EXPEDICIONES COMPLETADAS"
                : "SIN REGISTROS PARA ESTE FILTRO";
        }

        float contentHeight = Mathf.Max(1210f, visibleRows * 252f);
        if (content != null) content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
        if (scrollRect != null && resetScroll) scrollRect.verticalNormalizedPosition = 1f;
    }

    private void RefreshHeader()
    {
        Dimension1HeaderMetalsUI.Refresh(transform, currentState,
            currentState.dimension1SelectedSectorId);
    }

    private void RefreshFilterAppearance()
    {
        for (int i = 0; i < 4; i++)
        {
            bool selected = i == filterIndex;
            if (filterBorders != null && i < filterBorders.Length && filterBorders[i] != null)
                filterBorders[i].color = selected ? Cyan : CyanMuted;
            if (filterFills != null && i < filterFills.Length && filterFills[i] != null)
                filterFills[i].color = selected ? Hex("051721", 252) : NormalFill;
            if (filterLabels != null && i < filterLabels.Length && filterLabels[i] != null)
            {
                filterLabels[i].color = selected ? Cyan : CyanMuted;
                if (i == 3)
                    filterLabels[i].text = selected
                        ? Dimension1System.GetDimension1SectorVisualName(SectorIds[sectorIndex]).ToUpperInvariant() + "  V"
                        : "POR SECTOR  V";
            }
        }
    }

    private bool MatchesFilter(D1ExplorationRecordEntry entry)
    {
        if (filterIndex == 1) return HasRelic(entry);
        if (filterIndex == 2) return entry.coordinatedMission;
        if (filterIndex == 3) return entry.sectorId == SectorIds[sectorIndex];
        return true;
    }

    private void ConfigureRow(RowView row, D1ExplorationRecordEntry entry)
    {
        if (row == null || entry == null) return;
        row.root.SetActive(true);
        if (row.preview != null) row.preview.gameObject.SetActive(true);
        if (row.centralAccessIcon != null) row.centralAccessIcon.SetActive(false);
        ApplySprite(row.preview, FindSprite(DestinationIds, destinationSprites, entry.destinationId) ??
            fallbackDestinationSprite);
        row.indexText.text = "EXPEDICIÓN\n#" + entry.resultId;
        row.destinationText.text = DestinationName(entry.destinationId).ToUpperInvariant();
        row.sectorText.text = Dimension1System.GetDimension1SectorVisualName(entry.sectorId).ToUpperInvariant();
        ApplySprite(row.mainShipIcon, FindSprite(ShipIds, shipSprites, entry.shipId));
        bool coordinated = entry.coordinatedMission && !string.IsNullOrEmpty(entry.supportShipId);
        if (row.mainShipIcon != null)
        {
            Vector2 position = row.mainShipIcon.rectTransform.anchoredPosition;
            position.x = coordinated ? 479f : 527f;
            row.mainShipIcon.rectTransform.anchoredPosition = position;
            row.mainShipIcon.color = Cyan;
        }
        row.supportShipIcon.gameObject.SetActive(coordinated);
        if (coordinated)
        {
            ApplySprite(row.supportShipIcon, FindSprite(ShipIds, shipSprites, entry.supportShipId));
            row.supportShipIcon.color = Amber;
        }
        row.shipText.text = coordinated
            ? ShipName(entry.shipId).ToUpperInvariant() + " +\n" + ShipName(entry.supportShipId).ToUpperInvariant()
            : ShipName(entry.shipId).ToUpperInvariant();
        row.synergyText.gameObject.SetActive(coordinated && !string.IsNullOrEmpty(entry.synergyId));
        if (row.synergyText.gameObject.activeSelf)
            row.synergyText.text = "SINERGIA: " +
                Dimension1System.GetD1SynergyVisualName(entry.synergyId).ToUpperInvariant();
        row.completedIcon.SetActive(!coordinated);
        row.coordinatedIcon.SetActive(coordinated);
        row.statusText.text = coordinated ? "COORDINADA" : "COMPLETADA";
        int visibleRewards = ConfigureRewardBadges(row, entry);
        row.rewardsText.gameObject.SetActive(visibleRewards == 0);
        if (visibleRewards == 0) row.rewardsText.text = "SIN METALES";

        string badge = BuildExtraBadge(entry);
        bool hasBadge = !string.IsNullOrEmpty(badge);
        row.extraBadge.SetActive(hasBadge);
        if (hasBadge) row.extraBadgeText.text = badge;
    }

    private void ConfigureCentralKeyRow(RowView row)
    {
        if (row == null) return;
        row.root.SetActive(true);
        if (row.preview != null) row.preview.gameObject.SetActive(false);
        if (row.centralAccessIcon != null) row.centralAccessIcon.SetActive(true);
        row.indexText.text = "REGISTRO\nFIJO";
        row.destinationText.text = "CLAVE DE ACCESO CENTRAL";
        row.sectorText.text = "CENTRO GALÁCTICO";
        row.mainShipIcon.enabled = false;
        row.supportShipIcon.gameObject.SetActive(false);
        row.shipText.text = "CUATRO ECOS\nSINCRONIZADOS";
        row.synergyText.gameObject.SetActive(false);
        row.completedIcon.SetActive(true);
        row.coordinatedIcon.SetActive(false);
        row.statusText.text = "AUTORIZADA";
        HideRewardBadges(row);
        row.rewardsText.gameObject.SetActive(true);
        row.rewardsText.text = "ENTRADA A ARK HABILITADA";
        row.extraBadge.SetActive(false);
    }

    private string BuildMetalSummary(D1ExplorationRecordEntry entry)
    {
        if (entry.rewards == null || entry.rewards.Count == 0) return "SIN METALES";
        var parts = new List<string>();
        foreach (D1MetalAmount reward in entry.rewards)
            if (reward != null && !string.IsNullOrEmpty(reward.metalId) && reward.amount > 0d)
                parts.Add(MetalSymbol(reward.metalId) + " +" + FormatAmount(reward.amount));
        return parts.Count > 0 ? string.Join("  ·  ", parts) : "SIN METALES";
    }

    private static int ConfigureRewardBadges(RowView row, D1ExplorationRecordEntry entry)
    {
        HideRewardBadges(row);
        if (entry?.rewards == null || row.rewardBadges == null || row.rewardBadgeTexts == null)
            return 0;
        int visible = 0;
        foreach (D1MetalAmount reward in entry.rewards)
        {
            if (reward == null || string.IsNullOrEmpty(reward.metalId) || reward.amount <= 0d)
                continue;
            if (visible >= row.rewardBadges.Length || visible >= row.rewardBadgeTexts.Length)
                break;
            if (row.rewardBadges[visible] != null) row.rewardBadges[visible].SetActive(true);
            if (row.rewardBadgeTexts[visible] != null)
                row.rewardBadgeTexts[visible].text = MetalSymbol(reward.metalId) + "  " + FormatAmount(reward.amount);
            visible++;
        }
        return visible;
    }

    private static void HideRewardBadges(RowView row)
    {
        if (row?.rewardBadges == null) return;
        foreach (GameObject badge in row.rewardBadges)
            if (badge != null) badge.SetActive(false);
    }

    private static string BuildExtraBadge(D1ExplorationRecordEntry entry)
    {
        if (HasRelic(entry)) return "RELIQUIA";
        if (entry.specificBlueprintRewards != null)
            foreach (D1BlueprintAmount reward in entry.specificBlueprintRewards)
                if (reward != null && !string.IsNullOrEmpty(reward.blueprintId) && reward.amount > 0)
                    return "MATRIZ";
        if (entry.blueprintFragments > 0) return "+" + entry.blueprintFragments + " FRAGMENTOS";
        return "";
    }

    private static bool HasRelic(D1ExplorationRecordEntry entry)
    {
        if (entry?.relicRewards == null) return false;
        foreach (D1RelicRewardEntry reward in entry.relicRewards)
            if (reward != null && !string.IsNullOrEmpty(reward.relicId)) return true;
        return false;
    }

    private static bool IsDisplayable(D1ExplorationRecordEntry entry)
    {
        return entry != null && Dimension1System.IsShipActiveInDimension1Base(entry.shipId);
    }

    private static double SumMetals(D1ExplorationRecordEntry entry)
    {
        double total = 0d;
        if (entry?.rewards == null) return total;
        foreach (D1MetalAmount reward in entry.rewards)
            if (reward != null && reward.amount > 0d) total += reward.amount;
        return total;
    }

    private string DestinationName(string id) => panel != null
        ? panel.GetD1DestinationVisualNameForUi(id) : id;

    private string ShipName(string id) => panel != null
        ? panel.GetD1ShipVisualNameForUi(id) : Dimension1System.GetDimension1ShipVisualName(id);

    private static string MetalSymbol(string id)
    {
        if (id == Dimension1System.MetalIron) return "FE";
        if (id == Dimension1System.MetalCopper) return "CU";
        if (id == Dimension1System.MetalAluminum) return "AL";
        if (id == Dimension1System.MetalTitanium) return "TI";
        if (id == Dimension1System.MetalNickel) return "NI";
        if (id == Dimension1System.MetalCobalt) return "CO";
        if (id == Dimension1System.MetalLithium) return "LI";
        if (id == Dimension1System.MetalTungsten) return "W";
        if (id == Dimension1System.MetalPlatinum) return "PT";
        if (id == Dimension1System.MetalIridium) return "IR";
        return "--";
    }

    private static Sprite FindSprite(string[] ids, Sprite[] sprites, string id)
    {
        if (ids == null || sprites == null) return null;
        int count = Mathf.Min(ids.Length, sprites.Length);
        for (int i = 0; i < count; i++) if (ids[i] == id) return sprites[i];
        return null;
    }

    private static void ApplySprite(Image image, Sprite sprite)
    {
        if (image == null) return;
        image.sprite = sprite;
        image.enabled = sprite != null;
        image.color = Color.white;
        image.preserveAspect = true;
    }

    private static string FormatAmount(double value)
    {
        value = Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        if (value >= 100d) return value.ToString("0");
        return value.ToString("0.#");
    }

    private void SetVisible(bool value)
    {
        visible = value;
        if (canvasGroup == null) return;
        canvasGroup.alpha = value ? 1f : 0f;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }

    private int RowCapacity => rows != null ? rows.Length : 0;

    private static Color Hex(string html, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + html, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
