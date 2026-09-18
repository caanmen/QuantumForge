using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vista detallada del resultado de una expedición D1. No concede recompensas:
/// sólo presenta el registro que Dimension1System ya guardó.
/// </summary>
public sealed class Dimension1ExpeditionResultUI : MonoBehaviour
{
    [Serializable]
    public sealed class RewardSlot
    {
        public GameObject root;
        public RectTransform rect;
        public Image icon;
        public TMP_Text nameText;
        public TMP_Text amountText;
    }

    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text destinationText;
    [SerializeField] private Image shipIcon;
    [SerializeField] private TMP_Text shipText;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private RewardSlot[] metalSlots;
    [SerializeField] private TMP_Text metalsEmptyText;
    [SerializeField] private RewardSlot fragmentSlot;
    [SerializeField] private RewardSlot matrixSlot;
    [SerializeField] private TMP_Text matricesEmptyText;
    [SerializeField] private GameObject relicPanel;
    [SerializeField] private Image relicIcon;
    [SerializeField] private TMP_Text relicHeading;
    [SerializeField] private TMP_Text relicName;
    [SerializeField] private TMP_Text relicTier;
    [SerializeField] private TMP_Text relicOrigin;
    [SerializeField] private TMP_Text relicDescription;
    [SerializeField] private GameObject specialPointBanner;
    [SerializeField] private TMP_Text specialPointText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button relicsButton;
    [SerializeField] private Sprite[] metalSprites;
    [SerializeField] private Sprite[] shipSprites;
    [SerializeField] private Sprite matrixFragmentSprite;
    [SerializeField] private Sprite matrixSpecificSprite;
    [SerializeField] private Sprite[] relicSprites;

    private static readonly string[] MetalIds =
    {
        Dimension1System.MetalIron, Dimension1System.MetalCopper,
        Dimension1System.MetalAluminum, Dimension1System.MetalTitanium,
        Dimension1System.MetalNickel, Dimension1System.MetalCobalt,
        Dimension1System.MetalLithium, Dimension1System.MetalTungsten,
        Dimension1System.MetalPlatinum, Dimension1System.MetalIridium
    };

    private static readonly string[] ShipIds =
    {
        Dimension1System.ShipLightProbe, Dimension1System.ShipExtractorDrone,
        Dimension1System.ShipAnalyticProbe, Dimension1System.ShipCargoShip
    };

    // Debe conservar exactamente el orden de sprites de la Cámara de Reliquias.
    private static readonly string[] RelicIds =
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

    public void Configure(
        Dimension1PanelUI configuredPanel, CanvasGroup configuredCanvasGroup,
        TMP_Text configuredDestinationText, Image configuredShipIcon,
        TMP_Text configuredShipText, TMP_Text configuredDurationText,
        RewardSlot[] configuredMetalSlots, TMP_Text configuredMetalsEmptyText,
        RewardSlot configuredFragmentSlot, RewardSlot configuredMatrixSlot,
        TMP_Text configuredMatricesEmptyText, GameObject configuredRelicPanel,
        Image configuredRelicIcon, TMP_Text configuredRelicHeading,
        TMP_Text configuredRelicName, TMP_Text configuredRelicTier,
        TMP_Text configuredRelicOrigin, TMP_Text configuredRelicDescription,
        GameObject configuredSpecialPointBanner, TMP_Text configuredSpecialPointText,
        Button configuredContinueButton, Button configuredRelicsButton,
        Sprite[] configuredMetalSprites, Sprite[] configuredShipSprites,
        Sprite configuredMatrixFragmentSprite, Sprite configuredMatrixSpecificSprite,
        Sprite[] configuredRelicSprites)
    {
        panel = configuredPanel;
        canvasGroup = configuredCanvasGroup;
        destinationText = configuredDestinationText;
        shipIcon = configuredShipIcon;
        shipText = configuredShipText;
        durationText = configuredDurationText;
        metalSlots = configuredMetalSlots;
        metalsEmptyText = configuredMetalsEmptyText;
        fragmentSlot = configuredFragmentSlot;
        matrixSlot = configuredMatrixSlot;
        matricesEmptyText = configuredMatricesEmptyText;
        relicPanel = configuredRelicPanel;
        relicIcon = configuredRelicIcon;
        relicHeading = configuredRelicHeading;
        relicName = configuredRelicName;
        relicTier = configuredRelicTier;
        relicOrigin = configuredRelicOrigin;
        relicDescription = configuredRelicDescription;
        specialPointBanner = configuredSpecialPointBanner;
        specialPointText = configuredSpecialPointText;
        continueButton = configuredContinueButton;
        relicsButton = configuredRelicsButton;
        metalSprites = configuredMetalSprites;
        shipSprites = configuredShipSprites;
        matrixFragmentSprite = configuredMatrixFragmentSprite;
        matrixSpecificSprite = configuredMatrixSpecificSprite;
        relicSprites = configuredRelicSprites;
    }

    private void Awake() => SetVisible(false);

    public void RefreshFromState(GameState state, bool shouldShow, int resultId = 0)
    {
        if (!shouldShow || state == null)
        {
            SetVisible(false);
            return;
        }

        D1ExplorationRecordEntry record = FindRecord(state, resultId);
        if (record == null)
        {
            SetVisible(false);
            return;
        }

        transform.SetAsLastSibling();
        SetVisible(true);
        destinationText.text = VisualDestination(record.destinationId).ToUpperInvariant();
        shipText.text = BuildShipText(record);
        durationText.text = record.totalSeconds > 0.0 ? FormatDuration(record.totalSeconds) : "";
        ApplySprite(shipIcon, FindSprite(ShipIds, shipSprites, record.shipId));
        RefreshMetals(record.rewards);
        RefreshMatrices(record);
        RefreshRelic(record);
        RefreshSpecialPoint(record.specialPointId);
    }

    public void Continue()
    {
        SetVisible(false);
        if (panel != null) panel.OnClickCloseExplorationRewards();
    }

    public void ViewRelics()
    {
        SetVisible(false);
        if (panel != null) panel.OnClickViewLastExplorationRelics();
    }

    private void RefreshMetals(List<D1MetalAmount> rewards)
    {
        var valid = new List<D1MetalAmount>();
        if (rewards != null)
            foreach (D1MetalAmount reward in rewards)
                if (reward != null && !string.IsNullOrEmpty(reward.metalId) && reward.amount > 0.0)
                    valid.Add(reward);

        int shown = Mathf.Min(valid.Count, metalSlots != null ? metalSlots.Length : 0);
        if (metalsEmptyText != null) metalsEmptyText.gameObject.SetActive(shown == 0);
        for (int i = 0; metalSlots != null && i < metalSlots.Length; i++)
        {
            RewardSlot slot = metalSlots[i];
            bool active = slot != null && i < shown;
            if (slot?.root != null) slot.root.SetActive(active);
            if (!active) continue;
            D1MetalAmount reward = valid[i];
            slot.nameText.text = VisualMetal(reward.metalId).ToUpperInvariant();
            slot.amountText.text = "+" + FormatAmount(reward.amount);
            ApplySprite(slot.icon, FindSprite(MetalIds, metalSprites, reward.metalId));
        }
        LayoutMetalSlots(metalSlots, shown);
    }

    private void RefreshMatrices(D1ExplorationRecordEntry record)
    {
        bool hasFragments = record.blueprintFragments > 0;
        D1BlueprintAmount specific = null;
        if (record.specificBlueprintRewards != null)
            foreach (D1BlueprintAmount reward in record.specificBlueprintRewards)
                if (reward != null && !string.IsNullOrEmpty(reward.blueprintId) && reward.amount > 0)
                { specific = reward; break; }

        SetSlot(fragmentSlot, hasFragments, matrixFragmentSprite,
            "FRAGMENTOS DE\nMATRIZ ADAPTATIVA", "+" + record.blueprintFragments);
        SetSlot(matrixSlot, specific != null, matrixSpecificSprite,
            specific != null ? VisualBlueprint(specific.blueprintId).ToUpperInvariant() : "",
            specific != null ? "+" + specific.amount : "");
        if (matricesEmptyText != null)
            matricesEmptyText.gameObject.SetActive(!hasFragments && specific == null);

        if (hasFragments && specific != null)
        {
            SetX(fragmentSlot, 92f);
            SetX(matrixSlot, 548f);
        }
        else if (hasFragments) SetX(fragmentSlot, 320f);
        else if (specific != null) SetX(matrixSlot, 320f);
    }

    private void RefreshRelic(D1ExplorationRecordEntry record)
    {
        D1RelicRewardEntry reward = null;
        if (record.relicRewards != null)
            foreach (D1RelicRewardEntry candidate in record.relicRewards)
                if (candidate != null && !string.IsNullOrEmpty(candidate.relicId))
                { reward = candidate; break; }

        if (relicPanel != null) relicPanel.SetActive(true);
        if (reward == null)
        {
            relicHeading.text = "RELIQUIAS";
            relicName.text = "NINGUNA RELIQUIA ENCONTRADA";
            relicTier.text = "";
            relicOrigin.text = "";
            relicDescription.text = "La expedición no recuperó reliquias.";
            ApplySprite(relicIcon, null);
            if (relicsButton != null) relicsButton.gameObject.SetActive(false);
            return;
        }

        int tier = Dimension1System.GetDimension1RelicTier(reward.relicId);
        string name = Dimension1System.GetDimension1RelicVisualName(reward.relicId);
        relicHeading.text = reward.wasDuplicate ? "RELIQUIA RECUPERADA" : "RELIQUIA DESCUBIERTA";
        relicName.text = name.ToUpperInvariant();
        relicTier.text = "TIER " + tier;
        relicOrigin.text = "NUEVA RELIQUIA DE " + SectorName(
            Dimension1System.GetDimension1RelicSectorId(reward.relicId)).ToUpperInvariant() + ".";
        relicDescription.text = reward.wasDuplicate
            ? "Duplicado convertido en +" + FormatAmount(reward.duplicateMetalAmount) + " " +
                VisualMetal(reward.duplicateMetalId).ToUpperInvariant() + "."
            : "Añadida a la Cámara de Reliquias para su consulta y mejora.";
        ApplySprite(relicIcon, FindSprite(RelicIds, relicSprites, reward.relicId));
        if (relicsButton != null) relicsButton.gameObject.SetActive(true);
    }

    private void RefreshSpecialPoint(string specialPointId)
    {
        bool active = !string.IsNullOrEmpty(specialPointId);
        if (specialPointBanner != null) specialPointBanner.SetActive(active);
        if (active && specialPointText != null)
            specialPointText.text = "PUNTO ESPECIAL · " +
                Dimension1System.GetD1SpecialPointVisualName(specialPointId).ToUpperInvariant();
    }

    private D1ExplorationRecordEntry FindRecord(GameState state, int resultId)
    {
        if (state.dimension1RecentExplorationRecords == null) return null;
        int requestedId = resultId > 0 ? resultId : state.dimension1LastExplorationResultId;
        for (int i = state.dimension1RecentExplorationRecords.Count - 1; i >= 0; i--)
        {
            D1ExplorationRecordEntry entry = state.dimension1RecentExplorationRecords[i];
            if (entry != null && entry.resultId == requestedId) return entry;
        }
        return null;
    }

    private string BuildShipText(D1ExplorationRecordEntry record)
    {
        string value = VisualShip(record.shipId).ToUpperInvariant();
        if (record.coordinatedMission && !string.IsNullOrEmpty(record.supportShipId))
            value += " + " + VisualShip(record.supportShipId).ToUpperInvariant();
        return value;
    }

    private string VisualMetal(string id) => panel != null ? panel.GetD1MetalVisualNameForUi(id) : id;
    private string VisualShip(string id) => panel != null ? panel.GetD1ShipVisualNameForUi(id) : id;
    private string VisualDestination(string id) => panel != null ? panel.GetD1DestinationVisualNameForUi(id) : id;
    private string VisualBlueprint(string id) => panel != null ? panel.GetD1BlueprintVisualNameForUi(id) : id;

    private static string SectorName(string sectorId)
    {
        return Dimension1System.GetDimension1SectorVisualName(sectorId);
    }

    private static void SetSlot(RewardSlot slot, bool active, Sprite sprite, string name, string amount)
    {
        if (slot?.root != null) slot.root.SetActive(active);
        if (!active || slot == null) return;
        ApplySprite(slot.icon, sprite);
        if (slot.nameText != null) slot.nameText.text = name;
        if (slot.amountText != null) slot.amountText.text = amount;
    }

    private static void SetX(RewardSlot slot, float x)
    {
        if (slot?.rect == null) return;
        Vector2 p = slot.rect.anchoredPosition;
        slot.rect.anchoredPosition = new Vector2(x, p.y);
    }

    private static void LayoutMetalSlots(RewardSlot[] slots, int count)
    {
        if (slots == null || count <= 0) return;

        float slotWidth;
        float start;
        float gap;
        bool compact = count >= 4;

        if (count == 1)
        {
            slotWidth = 360f;
            start = 320f;
            gap = 0f;
        }
        else if (count == 2)
        {
            // Comparte exactamente las columnas de FragmentReward y SpecificMatrixReward.
            slotWidth = 360f;
            start = 92f;
            gap = 96f;
        }
        else if (count == 3)
        {
            slotWidth = 280f;
            start = 24f;
            gap = 24f;
        }
        else
        {
            slotWidth = 210f;
            start = 24f;
            gap = 16f;
        }

        for (int i = 0; i < count; i++)
        {
            if (slots[i]?.rect == null) continue;
            slots[i].rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotWidth);
            SetX(slots[i], start + i * (slotWidth + gap));
            ApplyMetalSlotDensity(slots[i], slotWidth, compact);
        }
    }

    private static void ApplyMetalSlotDensity(RewardSlot slot, float width, bool compact)
    {
        if (slot == null) return;
        RectTransform badge = slot.icon != null ? slot.icon.rectTransform.parent as RectTransform : null;
        if (badge != null)
            SetTop(badge, 0f, compact ? 20f : 4f, compact ? 104f : 126f,
                compact ? 104f : 126f);

        if (slot.icon != null)
        {
            float inset = compact ? 10f : 13f;
            RectTransform icon = slot.icon.rectTransform;
            icon.anchorMin = Vector2.zero;
            icon.anchorMax = Vector2.one;
            icon.pivot = new Vector2(.5f, .5f);
            icon.offsetMin = new Vector2(inset, inset);
            icon.offsetMax = new Vector2(-inset, -inset);
        }

        float textX = compact ? 112f : 144f;
        float textWidth = Mathf.Max(88f, width - textX - 2f);
        if (slot.nameText != null)
        {
            SetTop(slot.nameText.rectTransform, textX, compact ? 27f : 12f,
                textWidth, compact ? 40f : 64f);
            slot.nameText.fontSize = compact ? 18f : 24f;
            slot.nameText.textWrappingMode = compact
                ? TextWrappingModes.NoWrap
                : TextWrappingModes.Normal;
        }

        if (slot.amountText != null)
        {
            SetTop(slot.amountText.rectTransform, textX, compact ? 72f : 77f,
                textWidth, compact ? 44f : 56f);
            slot.amountText.fontSize = compact ? 29f : 40f;
        }
    }

    private static void SetTop(RectTransform rect, float x, float y, float width, float height)
    {
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
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

    private static string FormatDuration(double seconds)
    {
        int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        int minutes = total / 60;
        int remaining = total % 60;
        return minutes > 0 ? minutes + "M " + remaining.ToString("00") + "S" : remaining + "S";
    }

    private static string FormatAmount(double amount)
    {
        if (amount >= 1000000.0) return (amount / 1000000.0).ToString("0.##") + "M";
        if (amount >= 1000.0) return amount.ToString("#,0");
        return amount.ToString(amount >= 100.0 ? "0" : "0.#");
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}
