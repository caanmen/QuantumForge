using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Dimension1RelicsVisualUI : MonoBehaviour
{
    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private Dimension1CommandCenterUI commandCenter;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] hideWhileOpen;
    [SerializeField] private TMP_Text[] metalAmounts;
    [SerializeField] private TMP_Text[] metalRates;
    [SerializeField] private Button[] relicButtons;
    [SerializeField] private RectTransform[] relicCardRoots;
    [SerializeField] private Image[] relicBorders;
    [SerializeField] private Image[] relicFills;
    [SerializeField] private Image[] relicSelectionGlows;
    [SerializeField] private Graphic[] relicTierFrames;
    [SerializeField] private GameObject[] relicReticles;
    [SerializeField] private Image[] relicArts;
    [SerializeField] private TMP_Text[] relicNames;
    [SerializeField] private TMP_Text[] relicTiers;
    [SerializeField] private TMP_Text[] relicStatuses;
    [SerializeField] private Sprite[] relicSprites;
    [SerializeField] private TMP_Text discoveredCounter;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private TMP_Text previousPageGlyph;
    [SerializeField] private TMP_Text nextPageGlyph;
    [SerializeField] private TMP_Text pageIndicator;
    [SerializeField] private Image detailArt;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailOrigin;
    [SerializeField] private TMP_Text detailLevel;
    [SerializeField] private TMP_Text nextMilestone;
    [SerializeField] private TMP_Text effectPrimary;
    [SerializeField] private TMP_Text effectSecondary;
    [SerializeField] private TMP_Text[] costNames;
    [SerializeField] private TMP_Text[] costRequired;
    [SerializeField] private TMP_Text[] costOwned;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeButtonLabel;
    [SerializeField] private bool referencePreviewForVisualQa;

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

    private static readonly string[] DisplayNames =
    {
        "BRÚJULA\nDE DERIVA",
        "TALADRO\nANTIGUO",
        "CRISTAL\nANALÍTICO",
        "REGISTRO DE\nNAVEGACIÓN\nPERDIDO",
        "CONTENEDOR\nMODULAR",
        "NÚCLEO DE\nPROSPECCIÓN",
        "ANTENA\nFRACTURADA",
        "SELLO DE\nEXTRACCIÓN",
        "PLACA DE\nEXPLORADOR",
        "MAPA ESTELAR\nINCOMPLETO",
        "ECO DEL\nCUARTO 1",
        "GANCHO DE\nEXTRACCIÓN",
        "ARCHIVO DE\nMATRICES",
        "RESONADOR DE\nTRAZAS",
        "ALEACIÓN\nRECORDADA",
        "NÚCLEO DE\nBODEGA ANTIGUA",
        "FRAGMENTO DE\nCALIBRACIÓN",
        "SENSOR DE\nFRECUENCIA RARA",
        "SELLO\nTRIANGULAR",
        "MEMORIA DE\nMÁQUINA"
    };

    private const int CardsPerPage = 8;

    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9BA9B3");
    private static readonly Color Fill = Hex("04121B", 248);
    private static readonly Color FillSelected = Hex("1A1508", 252);

    private int selectedRelicIndex = 2;
    private int currentPage;
    private float refreshTimer;
    private bool[] hiddenPreviousStates;
    private VerticalNavigationUI verticalNavigation;

    public void SetReferencePreviewForVisualQa(bool enabled)
    {
        bool enteringPreview = enabled && !referencePreviewForVisualQa;
        referencePreviewForVisualQa = enabled;
        if (enteringPreview)
        {
            currentPage = 0;
            selectedRelicIndex = 2;
        }
        Refresh();
    }

    public int CurrentPage => currentPage;

    public void ShowPageForVisualQa(int page)
    {
        if (!referencePreviewForVisualQa) return;
        SetPage(page);
    }

    public void OpenCommandCenter()
    {
        if (panel != null) panel.OnClickCloseRelicChamberPanel();
        if (commandCenter != null) commandCenter.ShowCommandCenterScreen();
    }

    public void OpenGalaxy()
    {
        if (panel == null) return;
        panel.OnClickCloseRelicChamberPanel();
        panel.OnClickOpenGalaxyPanel();
    }

    public void OpenExplore()
    {
        if (panel != null) panel.OnClickCloseRelicChamberPanel();
        if (commandCenter != null) commandCenter.ShowExploreScreen();
    }

    public void OpenHangar()
    {
        if (panel == null) return;
        panel.OnClickCloseRelicChamberPanel();
        panel.OnClickOpenHangarPanel();
    }

    public void OpenTree()
    {
        if (panel == null) return;
        panel.OnClickCloseRelicChamberPanel();
        panel.OnClickOpenDimension1TreePanel();
    }

    public void SelectRelic0() => SelectRelicSlot(0);
    public void SelectRelic1() => SelectRelicSlot(1);
    public void SelectRelic2() => SelectRelicSlot(2);
    public void SelectRelic3() => SelectRelicSlot(3);
    public void SelectRelic4() => SelectRelicSlot(4);
    public void SelectRelic5() => SelectRelicSlot(5);
    public void SelectRelic6() => SelectRelicSlot(6);
    public void SelectRelic7() => SelectRelicSlot(7);

    public void PreviousPage() => SetPage(currentPage - 1);
    public void NextPage() => SetPage(currentPage + 1);

    public void UpgradeSelected()
    {
        if (panel == null || referencePreviewForVisualQa) return;
        panel.SelectRelicChamberRelic(RelicIds[selectedRelicIndex]);
        panel.OnClickUpgradeSelectedRelic();
        refreshTimer = 0f;
        Refresh();
    }

    private void OnEnable()
    {
        if (verticalNavigation == null)
            verticalNavigation = FindFirstObjectByType<VerticalNavigationUI>(FindObjectsInactive.Include);
        if (verticalNavigation != null) verticalNavigation.SetNavigationSuppressed(true, this);
        ApplyExclusiveNavigation(true);
        currentPage = Mathf.Clamp(currentPage, 0, PageCount - 1);
        selectedRelicIndex = Mathf.Clamp(selectedRelicIndex, 0, RelicIds.Length - 1);
        refreshTimer = 0f;
        Refresh();
    }

    private void OnDisable()
    {
        ApplyExclusiveNavigation(false);
        if (verticalNavigation != null) verticalNavigation.SetNavigationSuppressed(false, this);
    }

    private void Update()
    {
        if (hideWhileOpen != null)
            foreach (GameObject target in hideWhileOpen)
                if (target != null && target.activeSelf) target.SetActive(false);

        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = .25f;
        Refresh();
    }

    private int PageCount => Mathf.CeilToInt(RelicIds.Length / (float)CardsPerPage);

    private void SetPage(int page)
    {
        int clamped = Mathf.Clamp(page, 0, PageCount - 1);
        if (clamped == currentPage)
        {
            Refresh();
            return;
        }

        currentPage = clamped;
        int firstIndex = currentPage * CardsPerPage;
        int lastIndex = Mathf.Min(firstIndex + CardsPerPage, RelicIds.Length) - 1;
        if (selectedRelicIndex < firstIndex || selectedRelicIndex > lastIndex)
            selectedRelicIndex = firstIndex;
        if (!referencePreviewForVisualQa && panel != null)
            panel.SelectRelicChamberRelic(RelicIds[selectedRelicIndex]);
        Refresh();
    }

    private void SelectRelicSlot(int slot)
    {
        int index = currentPage * CardsPerPage + slot;
        if (index < 0 || index >= RelicIds.Length) return;
        selectedRelicIndex = index;
        if (!referencePreviewForVisualQa && panel != null)
            panel.SelectRelicChamberRelic(RelicIds[index]);
        Refresh();
    }

    private void Refresh()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (referencePreviewForVisualQa)
        {
            ApplyReferencePreview();
            return;
        }

        GameState state = GameState.I;
        if (state == null) return;
        state.EnsureDimension1State();

        string[] metalIds =
        {
            Dimension1System.MetalIron,
            Dimension1System.MetalAluminum,
            Dimension1System.MetalNickel
        };
        for (int i = 0; i < metalIds.Length; i++)
        {
            Set(metalAmounts, i, FormatAmount(state.GetD1MetalAmount(metalIds[i])));
            Set(metalRates, i, "+" + FormatAmount(Dimension1System.GetMetalProductionPerSecond(state, metalIds[i])) + "/s");
        }

        int discovered = 0;
        foreach (string relicId in Dimension1System.Dimension1RelicIds)
            if (state.IsD1RelicUnlocked(relicId)) discovered++;
        if (discoveredCounter != null)
            discoveredCounter.text = discovered + " / " + Dimension1System.Dimension1RelicIds.Length + " DESCUBIERTAS";

        RefreshPageCards(state, false);
        ApplyDetail(state, RelicIds[selectedRelicIndex]);
    }

    private void ApplyReferencePreview()
    {
        string[] amounts = { "5.98M", "4.73M", "4.70M" };
        string[] rates = { "+0.30/s", "+0.08/s", "+0.03/s" };
        for (int i = 0; i < 3; i++)
        {
            Set(metalAmounts, i, amounts[i]);
            Set(metalRates, i, rates[i]);
        }
        if (discoveredCounter != null) discoveredCounter.text = "12 / 20 DESCUBIERTAS";
        RefreshPageCards(null, true);

        if (currentPage == 0 && selectedRelicIndex == 2)
        {
            if (detailArt != null && relicSprites != null && relicSprites.Length > 2) detailArt.sprite = relicSprites[2];
            if (detailName != null) detailName.text = "CRISTAL ANALÍTICO";
            if (detailOrigin != null) detailOrigin.text = "ORIGEN: ANILLO DE RESTOS";
            if (detailLevel != null) detailLevel.text = "25 / 100";
            if (nextMilestone != null) nextMilestone.text = "NIVEL 50";
            if (effectPrimary != null) effectPrimary.text = "Fragmentos y matrices con\nSonda Analítica: <color=#F4A70B>+0.5 pp</color>";
            if (effectSecondary != null) effectSecondary.text = "Reliquias en destinos de\ninvestigación: <color=#F4A70B>+0.2 pp</color>";
            SetCost(0, "HIERRO", "250K", "5.98M");
            SetCost(1, "ALUMINIO", "120K", "4.73M");
            SetCost(2, "NÍQUEL", "60K", "4.70M");
            if (upgradeButton != null) upgradeButton.interactable = true;
            if (upgradeButtonLabel != null) upgradeButtonLabel.text = "MEJORAR RELIQUIA";
            return;
        }

        GameState state = GameState.I;
        if (state != null)
        {
            state.EnsureDimension1State();
            ApplyDetail(state, RelicIds[selectedRelicIndex]);
            if (detailArt != null) detailArt.color = Color.white;
        }
    }

    private void RefreshPageCards(GameState state, bool revealForVisualQa)
    {
        int firstIndex = currentPage * CardsPerPage;
        int visibleCount = Mathf.Min(CardsPerPage, RelicIds.Length - firstIndex);
        for (int slot = 0; slot < CardsPerPage; slot++)
        {
            bool active = slot < visibleCount;
            if (relicCardRoots != null && slot < relicCardRoots.Length && relicCardRoots[slot] != null)
            {
                relicCardRoots[slot].gameObject.SetActive(active);
                if (active) PositionCard(slot, visibleCount);
            }
            if (!active) continue;

            int index = firstIndex + slot;
            string relicId = RelicIds[index];
            if (relicReticles != null && slot < relicReticles.Length && relicReticles[slot] != null)
                relicReticles[slot].SetActive(relicId == Dimension1System.RelicAnalyticCrystal);
            if (relicArts != null && slot < relicArts.Length && relicArts[slot] != null)
                relicArts[slot].sprite = relicSprites != null && index < relicSprites.Length ? relicSprites[index] : null;
            if (relicNames != null && slot < relicNames.Length && relicNames[slot] != null)
            {
                relicNames[slot].text = DisplayNames[index];
                relicNames[slot].fontSize = CardFontSize(DisplayNames[index]);
            }
            if (relicTiers != null && slot < relicTiers.Length && relicTiers[slot] != null)
                relicTiers[slot].text = "TIER " + Dimension1System.GetDimension1RelicTier(relicId);
            bool unlocked = revealForVisualQa || (state != null && state.IsD1RelicUnlocked(relicId));
            ApplyCardState(slot, index, index == selectedRelicIndex, unlocked);
        }

        if (pageIndicator != null) pageIndicator.text = (currentPage + 1) + " / " + PageCount;
        bool hasPrevious = currentPage > 0;
        bool hasNext = currentPage < PageCount - 1;
        if (previousPageButton != null) previousPageButton.interactable = hasPrevious;
        if (nextPageButton != null) nextPageButton.interactable = hasNext;
        if (previousPageGlyph != null) previousPageGlyph.color = hasPrevious ? Cyan : Secondary;
        if (nextPageGlyph != null) nextPageGlyph.color = hasNext ? Cyan : Secondary;
    }

    private void PositionCard(int slot, int visibleCount)
    {
        if (relicCardRoots == null || slot < 0 || slot >= relicCardRoots.Length || relicCardRoots[slot] == null) return;
        int col = slot % 4;
        int row = slot / 4;
        if (visibleCount <= 4)
        {
            col = slot % 2;
            row = slot / 2;
            relicCardRoots[slot].anchoredPosition = new Vector2(290f + col * 252f, -(310f + row * 397f));
            return;
        }
        relicCardRoots[slot].anchoredPosition = new Vector2(38f + col * 252f, -(310f + row * 397f));
    }

    private static float CardFontSize(string value)
    {
        string flat = value.Replace("\n", "");
        if (value.Split('\n').Length >= 3 || flat.Length >= 24) return 18f;
        if (flat.Length >= 19) return 20f;
        return 23f;
    }

    private void ApplyCardState(int slot, int relicIndex, bool selected, bool unlocked)
    {
        if (relicIndex < 0 || relicIndex >= RelicIds.Length) return;
        if (relicBorders != null && slot < relicBorders.Length && relicBorders[slot] != null)
            relicBorders[slot].color = selected ? Amber : unlocked ? CyanMuted : Hex("52616A");
        if (relicFills != null && slot < relicFills.Length && relicFills[slot] != null)
            relicFills[slot].color = selected ? FillSelected : Fill;
        if (relicSelectionGlows != null && slot < relicSelectionGlows.Length && relicSelectionGlows[slot] != null)
            relicSelectionGlows[slot].gameObject.SetActive(selected);
        if (relicArts != null && slot < relicArts.Length && relicArts[slot] != null)
            relicArts[slot].color = unlocked ? Color.white : new Color(.45f, .49f, .52f, .55f);
        if (relicNames != null && slot < relicNames.Length && relicNames[slot] != null)
            relicNames[slot].color = unlocked ? Primary : Secondary;
        if (relicTiers != null && slot < relicTiers.Length && relicTiers[slot] != null)
            relicTiers[slot].color = selected ? Amber : unlocked ? Cyan : Secondary;
        if (relicTierFrames != null && slot < relicTierFrames.Length && relicTierFrames[slot] != null)
            relicTierFrames[slot].color = selected ? Amber : unlocked ? CyanMuted : Hex("52616A");
        if (relicStatuses != null && slot < relicStatuses.Length && relicStatuses[slot] != null)
        {
            relicStatuses[slot].text = unlocked ? "DESCUBIERTA" : "BLOQUEADA";
            relicStatuses[slot].color = selected ? Amber : unlocked ? Cyan : Secondary;
        }
        if (relicButtons != null && slot < relicButtons.Length && relicButtons[slot] != null)
            relicButtons[slot].interactable = true;
    }

    private void ApplyDetail(GameState state, string relicId)
    {
        int index = System.Array.IndexOf(RelicIds, relicId);
        bool unlocked = state.IsD1RelicUnlocked(relicId);
        int level = unlocked ? state.GetD1RelicLevel(relicId) : 0;
        if (detailArt != null && index >= 0 && relicSprites != null && index < relicSprites.Length)
        {
            detailArt.sprite = relicSprites[index];
            detailArt.color = unlocked ? Color.white : new Color(.42f, .46f, .50f, .58f);
        }
        if (detailName != null) detailName.text = DisplayNames[index].Replace("\n", " ");
        if (detailOrigin != null) detailOrigin.text = "ORIGEN: " + SectorName(Dimension1System.GetDimension1RelicSectorId(relicId));
        if (detailLevel != null) detailLevel.text = level + " / " + Dimension1System.Dimension1RelicMaxLevel;
        int next = level >= Dimension1System.Dimension1RelicMaxLevel
            ? Dimension1System.Dimension1RelicMaxLevel
            : Mathf.Min(Dimension1System.Dimension1RelicMaxLevel,
                ((level / Dimension1System.Dimension1RelicMilestoneStep) + 1) * Dimension1System.Dimension1RelicMilestoneStep);
        if (nextMilestone != null) nextMilestone.text = next >= 100 ? "NIVEL MÁXIMO" : "NIVEL " + next;

        double primary = Dimension1System.GetDimension1RelicPrimaryBonusForLevel(relicId, level);
        double secondary = Dimension1System.GetDimension1RelicSecondaryBonusForLevel(relicId, level);
        string pendingImpact = PendingRelicImpact(relicId);
        if (!string.IsNullOrEmpty(pendingImpact))
        {
            if (effectPrimary != null)
                effectPrimary.text = "Impacto previsto: <color=#F4A70B>" + pendingImpact + "</color>";
            if (effectSecondary != null)
                effectSecondary.text = "Valores numéricos pendientes de definición;\nno aplica bonus por ahora.";
        }
        else
        {
            if (effectPrimary != null) effectPrimary.text = "Bonificación primaria de reliquia: <color=#F4A70B>+" + FormatPercentPoints(primary) + " pp</color>";
            if (effectSecondary != null) effectSecondary.text = "Bonificación secundaria de reliquia: <color=#F4A70B>+" + FormatPercentPoints(secondary) + " pp</color>";
        }
        if (relicId == Dimension1System.RelicAnalyticCrystal)
        {
            if (effectPrimary != null) effectPrimary.text = "Fragmentos y matrices con\nSonda Analítica: <color=#F4A70B>+" + FormatPercentPoints(primary) + " pp</color>";
            if (effectSecondary != null) effectSecondary.text = "Reliquias en destinos de\ninvestigación: <color=#F4A70B>+" + FormatPercentPoints(secondary) + " pp</color>";
        }

        bool hasCost = Dimension1System.TryGetNextDimension1RelicUpgradeCost(state, relicId,
            out _, out double le, out double traces, out string metal1, out double amount1,
            out string metal2, out double amount2);
        if (hasCost)
        {
            SetCost(0, "LE", FormatAmount(le), FormatAmount(state.LE));
            SetCost(1, "TRAZAS", FormatAmount(traces), FormatAmount(state.Traces));
            string metals = MetalName(metal1) + (string.IsNullOrEmpty(metal2) ? "" : " / " + MetalName(metal2));
            string required = FormatAmount(amount1) + (string.IsNullOrEmpty(metal2) ? "" : " / " + FormatAmount(amount2));
            string owned = FormatAmount(state.GetD1MetalAmount(metal1)) +
                (string.IsNullOrEmpty(metal2) ? "" : " / " + FormatAmount(state.GetD1MetalAmount(metal2)));
            SetCost(2, metals, required, owned);
        }
        else
        {
            SetCost(0, unlocked ? "NIVEL" : "ESTADO", unlocked ? "MÁXIMO" : "BLOQUEADA", "—");
            SetCost(1, "COSTE", "—", "—");
            SetCost(2, "RECURSOS", "—", "—");
        }
        if (upgradeButton != null) upgradeButton.interactable = hasCost && Dimension1System.CanUpgradeDimension1Relic(state, relicId);
        if (upgradeButtonLabel != null) upgradeButtonLabel.text = hasCost ? "MEJORAR RELIQUIA" : unlocked ? "NIVEL MÁXIMO" : "RELIQUIA BLOQUEADA";
    }

    private static string PendingRelicImpact(string relicId)
    {
        if (relicId == Dimension1System.RelicIncompleteStarMap) return "variedad de destinos";
        if (relicId == Dimension1System.RelicTracesResonator) return "Trazas";
        if (relicId == Dimension1System.RelicCalibrationFragment) return "Modulador de Fase";
        if (relicId == Dimension1System.RelicRareFrequencySensor) return "puntos especiales";
        if (relicId == Dimension1System.RelicTriangularSeal) return "Triángulo";
        if (relicId == Dimension1System.RelicMachineMemory) return "Máquina / Cuarto 2";
        return null;
    }

    private void SetCost(int index, string name, string required, string owned)
    {
        Set(costNames, index, name);
        Set(costRequired, index, required);
        Set(costOwned, index, owned);
    }

    private void ApplyExclusiveNavigation(bool hide)
    {
        if (hideWhileOpen == null) return;
        if (hide)
        {
            hiddenPreviousStates = new bool[hideWhileOpen.Length];
            for (int i = 0; i < hideWhileOpen.Length; i++)
            {
                GameObject target = hideWhileOpen[i];
                if (target == null) continue;
                hiddenPreviousStates[i] = target.activeSelf;
                target.SetActive(false);
            }
            return;
        }
        if (hiddenPreviousStates == null) return;
        for (int i = 0; i < hideWhileOpen.Length && i < hiddenPreviousStates.Length; i++)
            if (hideWhileOpen[i] != null) hideWhileOpen[i].SetActive(hiddenPreviousStates[i]);
        hiddenPreviousStates = null;
    }

    private static string SectorName(string sectorId)
    {
        if (sectorId == Dimension1System.Sector01OuterRim) return "BORDE EXTERIOR";
        if (sectorId == Dimension1System.Sector02DebrisRing) return "ANILLO DE RESTOS";
        if (sectorId == Dimension1System.Sector03AncientOrbits) return "ÓRBITAS ANTIGUAS";
        if (sectorId == Dimension1System.Sector04SilentFrontier) return "FRONTERA SILENCIOSA";
        return "DIMENSIÓN 1";
    }

    private static string MetalName(string id)
    {
        if (id == Dimension1System.MetalIron) return "HIERRO";
        if (id == Dimension1System.MetalAluminum) return "ALUMINIO";
        if (id == Dimension1System.MetalNickel) return "NÍQUEL";
        if (id == Dimension1System.MetalLithium) return "LITIO";
        if (id == Dimension1System.MetalPlatinum) return "PLATINO";
        if (id == Dimension1System.MetalIridium) return "IRIDIO";
        if (id == Dimension1System.MetalTungsten) return "TUNGSTENO";
        return string.IsNullOrEmpty(id) ? "METAL" : id.ToUpperInvariant();
    }

    private static string FormatPercentPoints(double value) => (value * 100d).ToString("0.#");

    private static string FormatAmount(double value)
    {
        value = System.Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        return value.ToString("0.##");
    }

    private static void Set(TMP_Text[] targets, int index, string value)
    {
        if (targets != null && index >= 0 && index < targets.Length && targets[index] != null)
            targets[index].text = value;
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
