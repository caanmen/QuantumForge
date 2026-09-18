using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Dimension1ExploreVisualUI : MonoBehaviour
{
    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private Dimension1CommandCenterUI commandCenter;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] blockingPanels;
    [SerializeField] private GameObject[] hideWhileOpen;
    [SerializeField] private TMP_Text[] metalAmounts;
    [SerializeField] private TMP_Text[] metalRates;
    [SerializeField] private TMP_Text sectorName;
    [SerializeField] private Image sectorArtwork;
    [SerializeField] private string[] sectorArtworkIds;
    [SerializeField] private Sprite[] sectorArtworkSprites;
    [SerializeField] private TMP_Text scannerLevel;
    [SerializeField] private TMP_Text destinationName;
    [SerializeField] private TMP_Text destinationLevel;
    [SerializeField] private TMP_Text destinationDistance;
    [SerializeField] private TMP_Text destinationCount;
    [SerializeField] private TMP_Text shipName;
    [SerializeField] private TMP_Text shipStatus;
    [SerializeField] private TMP_Text shipMetricLabel;
    [SerializeField] private TMP_Text shipSpeed;
    [SerializeField] private Image shipIllustration;
    [SerializeField] private Image supportIllustration;
    [SerializeField] private Sprite[] shipIllustrations;
    [SerializeField] private TMP_Text supportName;
    [SerializeField] private TMP_Text supportStatus;
    [SerializeField] private TMP_Text supportBonus;
    [SerializeField] private TMP_Text supportAvailability;
    [SerializeField] private TMP_Text activeShip;
    [SerializeField] private TMP_Text activeDestination;
    [SerializeField] private TMP_Text activeTimer;
    [SerializeField] private TMP_Text activeCount;
    [SerializeField] private TMP_Text scannerUpgradeLabel;
    [SerializeField] private Button scannerUpgradeButton;
    [SerializeField] private TMP_Text startButtonLabel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button[] destinationCardButtons;
    [SerializeField] private Image[] destinationCardImages;
    [SerializeField] private Image[] destinationCardBorders;
    [SerializeField] private TMP_Text[] destinationCardLabels;
    [SerializeField] private string[] destinationArtIds;
    [SerializeField] private Sprite[] destinationArtSprites;
    [SerializeField] private Image simpleModeBorder;
    [SerializeField] private Image coordinatedModeBorder;
    [SerializeField] private TMP_Text simpleModeStatus;
    [SerializeField] private TMP_Text coordinatedModeStatus;
    [SerializeField] private GameObject previewDetailsOverlay;
    [SerializeField] private TMP_Text previewDetailsText;
    [SerializeField] private RectTransform previewDetailsContent;
    [SerializeField] private ScrollRect previewDetailsScroll;
    [SerializeField] private bool referencePreviewForVisualQa;

    private float refreshTimer;
    private bool exclusiveNavigationPending;
    private bool[] hiddenPreviousStates;
    private VerticalNavigationUI verticalNavigation;
    private int activeExpeditionIndex;
    private bool ensuringSelections;
    private Button runtimeOpenDetailsButton;
    private Button runtimeCloseDetailsButton;
    private Button runtimeRecordButton;
    private bool runtimeOpenDetailsBound;
    private bool runtimeCloseDetailsBound;
    private bool runtimeRecordBound;

    public void SetReferencePreviewForVisualQa(bool enabled)
    {
        referencePreviewForVisualQa = enabled;
        Refresh();
    }

    public void ConfigureCommandCenter(Dimension1CommandCenterUI configuredCommandCenter)
    {
        commandCenter = configuredCommandCenter;
    }

    public void OpenCommandCenter()
    {
        ResolveCommandCenter();
        if (commandCenter != null)
            commandCenter.ShowCommandCenterScreen();
        else
            gameObject.SetActive(false);
    }

    public void OpenExpeditionRecord()
    {
        if (panel == null || referencePreviewForVisualQa) return;
        ClosePreviewDetails();
        panel.OnClickOpenExplorationRecord();
    }

    public void SelectPreviousDestination() => CycleDestination(-1);
    public void SelectNextDestination() => CycleDestination(1);
    public void SelectDestinationCard0() => SelectDestinationCard(0);
    public void SelectDestinationCard1() => SelectDestinationCard(1);
    public void SelectDestinationCard2() => SelectDestinationCard(2);
    public void SelectDestinationCard3() => SelectDestinationCard(3);
    public void SelectPreviousShip() => CycleShip(-1);
    public void SelectNextShip() => CycleShip(1);
    public void SelectPreviousSupport() => CycleSupport(-1);
    public void SelectNextSupport() => CycleSupport(1);

    public void ToggleCoordinatedSupport()
    {
        if (panel == null || referencePreviewForVisualQa) return;
        panel.SetCoordinatedModeForUi(!panel.IsCoordinatedModeForUi());
        refreshTimer = 0f;
        Refresh();
    }

    public void SetSimpleMode()
    {
        if (panel == null || referencePreviewForVisualQa) return;
        panel.SetCoordinatedModeForUi(false);
        refreshTimer = 0f;
        Refresh();
    }

    public void SetCoordinatedMode()
    {
        if (panel == null || referencePreviewForVisualQa) return;
        panel.SetCoordinatedModeForUi(true);
        refreshTimer = 0f;
        Refresh();
    }

    public void SelectPreviousActiveExpedition()
    {
        activeExpeditionIndex--;
        refreshTimer = 0f;
        Refresh();
    }

    public void SelectNextActiveExpedition()
    {
        activeExpeditionIndex++;
        refreshTimer = 0f;
        Refresh();
    }

    public void UpgradeScanner()
    {
        if (panel == null || referencePreviewForVisualQa) return;
        panel.OnClickUpgradeScanner();
        refreshTimer = 0f;
        Refresh();
    }

    public void RefreshFromStateForUi()
    {
        refreshTimer = 0f;
        Refresh();
    }

    public void PrepareForPresentationForUi()
    {
        ClosePreviewDetails();
        PrepareInteractionFallbacks();
        if (panel != null)
            panel.SuppressLegacyExplorationPreviewForUi();
        refreshTimer = 0f;
        RefreshVisibility();
        Refresh();
    }

    public void ExecutePrimaryAction()
    {
        if (panel == null || referencePreviewForVisualQa) return;
        GameState state = GameState.I;
        if (state == null || state.dimension1ScanActive) return;
        if (panel.GetAvailableDestinationCountForUi() <= 0)
            panel.OnClickScanSimpleDestination();
        else
        {
            panel.EnsureDefaultExploreSelectionsForUi();
            panel.OnClickStartLightProbeExploration();
        }
        refreshTimer = 0f;
        Refresh();
    }

    public void OpenPreviewDetails()
    {
        if (previewDetailsOverlay == null || referencePreviewForVisualQa) return;
        previewDetailsOverlay.transform.SetAsLastSibling();
        previewDetailsOverlay.SetActive(true);
        RefreshPreviewDetails();
        Canvas.ForceUpdateCanvases();
        if (previewDetailsScroll == null)
            previewDetailsScroll = previewDetailsOverlay.GetComponentInChildren<ScrollRect>(true);
        if (previewDetailsScroll != null)
        {
            previewDetailsScroll.StopMovement();
            previewDetailsScroll.verticalNormalizedPosition = 1f;
        }
        Canvas.ForceUpdateCanvases();
    }

    public void ClosePreviewDetails()
    {
        if (previewDetailsOverlay != null) previewDetailsOverlay.SetActive(false);
    }

    private void OnEnable()
    {
        ResolveCommandCenter();
        exclusiveNavigationPending = true;
        if (verticalNavigation == null)
            verticalNavigation = FindFirstObjectByType<VerticalNavigationUI>(FindObjectsInactive.Include);
        if (verticalNavigation != null)
            verticalNavigation.SetNavigationSuppressed(true, this);
        if (panel != null)
            panel.SuppressLegacyExplorationPreviewForUi();
        PrepareInteractionFallbacks();
        refreshTimer = 0f;
        RefreshVisibility();
        Refresh();
    }

    private void ResolveCommandCenter()
    {
        if (commandCenter == null)
            commandCenter = FindFirstObjectByType<Dimension1CommandCenterUI>(
                FindObjectsInactive.Include);
    }

    private void OnDisable()
    {
        ClosePreviewDetails();
        RemoveInteractionFallbacks();
        exclusiveNavigationPending = false;
        ApplyExclusiveNavigation(false);
        if (verticalNavigation != null)
            verticalNavigation.SetNavigationSuppressed(false, this);
    }

    private void Update()
    {
        if (exclusiveNavigationPending)
        {
            exclusiveNavigationPending = false;
            ApplyExclusiveNavigation(true);
        }

        if (hideWhileOpen != null)
            foreach (GameObject target in hideWhileOpen)
                if (target != null && target.activeSelf) target.SetActive(false);

        RefreshVisibility();
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = 0.25f;
        Refresh();
    }

    private void RefreshVisibility()
    {
        if (canvasGroup == null) return;
        bool visible = panel == null || !panel.IsExplorationRecordOpenForUi();
        if (blockingPanels != null)
            foreach (GameObject blocker in blockingPanels)
                if (blocker != null && blocker.activeInHierarchy)
                {
                    visible = false;
                    break;
                }
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
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

    private void Refresh()
    {
        if (referencePreviewForVisualQa)
        {
            ApplyReferencePreview();
            return;
        }

        GameState state = GameState.I;
        if (state == null) return;
        state.EnsureDimension1State();
        Set(sectorName, Dimension1System.GetDimension1SectorVisualName(
            state.dimension1SelectedSectorId).Replace(" - ", " · ").ToUpperInvariant());
        RefreshSectorArtwork(state.dimension1SelectedSectorId);
        Dimension1HeaderMetalsUI.Refresh(transform, state, state.dimension1SelectedSectorId);
        Set(scannerLevel, Mathf.Clamp(state.dimension1ScannerLevel, 1, 15) + "/15");

        EnsureDefaultSelections(state);
        D1ScannedDestinationState destination = panel != null
            ? panel.GetSelectedAvailableDestinationForUi()
            : FirstDestination(state, out _);
        int destinationTotal = panel != null
            ? panel.GetAvailableDestinationCountForUi()
            : CountDestinations(state);
        Set(destinationCount, destinationTotal.ToString());
        if (state.dimension1ScanActive)
        {
            Set(destinationName, "ESCANEANDO SECTOR");
            Set(destinationLevel, "SEÑAL EN PROCESO");
            Set(destinationDistance, FormatTimer(state.dimension1ScanRemainingSeconds));
        }
        else
        {
            Set(destinationName, destination == null ? "SIN SEÑALES" : DestinationName(destination.destinationId));
            Set(destinationLevel, destination == null ? "ESCANEA PARA BUSCAR" : "SEÑAL DISPONIBLE");
            Set(destinationDistance, destination == null ? "—" : "DESTINO " + GetSelectedOrdinal(destination, state));
        }
        RefreshDestinationCards(state, destination);

        D1ShipState availableShip = panel != null ? panel.GetSelectedAvailableShipForUi() : FirstAvailableShip(state);
        Set(shipName, availableShip == null ? "SIN NAVE" : ShipName(availableShip.shipId));
        Set(shipStatus, availableShip == null ? "NO DISPONIBLE" : "DISPONIBLE");
        Set(shipMetricLabel, availableShip == null ? "ESTADO" : ShipMetricLabel(availableShip.shipId));
        Set(shipSpeed, availableShip == null ? "—" : "NIVEL " + ShipMetricLevel(availableShip));
        RefreshShipIllustration(shipIllustration, availableShip);

        bool coordinationUnlocked = panel != null && panel.CanUseCoordinatedModeForUi();
        bool coordinated = coordinationUnlocked && panel.IsCoordinatedModeForUi();
        D1ShipState support = coordinated ? panel.GetSelectedSupportShipForUi() : null;
        RefreshShipIllustration(supportIllustration, support);
        Set(supportName, support == null ? "SIN NAVE DE APOYO" : ShipName(support.shipId));
        Set(supportStatus, !coordinationUnlocked ? "BLOQUEADO EN ÁRBOL" : coordinated ?
            (support == null ? "ELIGE UNA NAVE" : "COORDINADA ACTIVA") : "APOYO DESACTIVADO");
        Set(supportBonus,
            !coordinated
                ? "MISIÓN SIMPLE"
                : support == null
                    ? "SELECCIONA APOYO"
                    : "×4 METALES · ×2.5 TIEMPO\n" + panel.GetSelectedSynergyNameForUi());
        Set(supportAvailability, !coordinationUnlocked ? "● NO DISPONIBLE" : coordinated ?
            (support == null ? "● SELECCIONA APOYO · TOCA PARA VOLVER" : "● TOCA PARA VOLVER A SIMPLE") :
            "● TOCA PARA ACTIVAR");
        RefreshModeCards(coordinationUnlocked, coordinated);

        D1ShipState[] activeShips = ActiveShips(state);
        if (activeShips.Length == 0) activeExpeditionIndex = 0;
        else activeExpeditionIndex = (activeExpeditionIndex % activeShips.Length + activeShips.Length) % activeShips.Length;
        D1ShipState active = activeShips.Length == 0 ? null : activeShips[activeExpeditionIndex];
        Set(activeShip, active == null ? "SIN EXPEDICIÓN ACTIVA" : ShipName(active.shipId));
        Set(activeDestination, active == null ? "ESCANEA UNA SEÑAL PARA EMPEZAR" :
            DestinationName(active.activeDestinationId) + (active.coordinatedMission ? " · COORDINADA" : ""));
        Set(activeTimer, active == null ? "--:--:--" : FormatTimer(active.explorationRemainingSeconds));
        Set(activeCount, activeShips.Length == 0 ? "0 / 0" : (activeExpeditionIndex + 1) + " / " + activeShips.Length);

        RefreshScannerUpgrade(state);
        RefreshPrimaryAction(state, destination, availableShip);
        if (previewDetailsOverlay != null && previewDetailsOverlay.activeSelf)
            RefreshPreviewDetails();
    }

    private void ApplyReferencePreview()
    {
        Set(sectorName, Dimension1System.GetDimension1SectorVisualName(
            Dimension1System.Sector01OuterRim).Replace(" - ", " · ").ToUpperInvariant());
        RefreshSectorArtwork(Dimension1System.Sector01OuterRim);
        string[] amounts = { "5.98M", "4.73M", "4.70M" };
        string[] rates = { "+0.30/s", "+0.08/s", "+0.03/s" };
        for (int i = 0; i < 3; i++)
        {
            Set(metalAmounts, i, amounts[i]);
            Set(metalRates, i, rates[i]);
        }
        Set(scannerLevel, "3/15");
        Set(destinationName, "SEÑAL DESCONOCIDA");
        Set(destinationLevel, "NIVEL 2");
        Set(destinationDistance, "2.41 UA");
        Set(destinationCount, "3");
        Set(shipName, "SONDA LIGERA");
        Set(shipStatus, "DISPONIBLE");
        Set(shipMetricLabel, "VELOCIDAD");
        Set(shipSpeed, "120 UA/s");
        RefreshShipIllustration(shipIllustration, FindShipByIdForPreview(Dimension1System.ShipLightProbe));
        RefreshShipIllustration(supportIllustration, FindShipByIdForPreview(Dimension1System.ShipExtractorDrone));
        Set(supportName, "DRON EXTRACTOR");
        Set(supportStatus, "DISPONIBLE");
        Set(supportBonus, "×4 METALES · ×2.5 TIEMPO");
        Set(supportAvailability, "● DISPONIBLE");
        Set(activeShip, "SONDA ANALÍTICA");
        Set(activeDestination, "SEÑAL DESCONOCIDA NIVEL 1");
        Set(activeTimer, "00:28:45");
        Set(activeCount, "1 / 2");
        Set(scannerUpgradeLabel, "MEJORAR ESCÁNER");
        Set(startButtonLabel, "INICIAR EXPEDICIÓN");
        if (scannerUpgradeButton != null) scannerUpgradeButton.interactable = true;
        if (startButton != null) startButton.interactable = true;
    }

    private void RefreshSectorArtwork(string sectorId)
    {
        if (sectorArtwork == null) return;
        Sprite selected = null;
        int count = Mathf.Min(
            sectorArtworkIds != null ? sectorArtworkIds.Length : 0,
            sectorArtworkSprites != null ? sectorArtworkSprites.Length : 0);
        for (int i = 0; i < count; i++)
        {
            if (sectorArtworkIds[i] != sectorId) continue;
            selected = sectorArtworkSprites[i];
            break;
        }

        sectorArtwork.sprite = selected;
        sectorArtwork.enabled = selected != null;
    }

    private void CycleDestination(int direction)
    {
        if (panel == null || referencePreviewForVisualQa) return;
        panel.CycleAvailableDestinationForUi(direction);
        refreshTimer = 0f;
        Refresh();
    }

    private void SelectDestinationCard(int index)
    {
        if (panel == null || referencePreviewForVisualQa) return;
        GameState state = GameState.I;
        D1ScannedDestinationState[] destinations = AvailableDestinations(state);
        if (index < 0 || index >= destinations.Length) return;
        D1ScannedDestinationState destination = destinations[index];
        if (destination == null) return;
        panel.TrySelectAvailableDestinationForUi(destination.destinationId, destination.sectorId);
        refreshTimer = 0f;
        Refresh();
    }

    private void CycleShip(int direction)
    {
        if (panel == null || referencePreviewForVisualQa) return;
        panel.CycleAvailableShipForUi(direction);
        refreshTimer = 0f;
        Refresh();
    }

    private void CycleSupport(int direction)
    {
        if (panel == null || referencePreviewForVisualQa) return;
        if (!panel.IsCoordinatedModeForUi())
            panel.SetCoordinatedModeForUi(true);
        else
            panel.CycleSupportShipForUi(direction);
        refreshTimer = 0f;
        Refresh();
    }

    private void EnsureDefaultSelections(GameState state)
    {
        if (panel == null || ensuringSelections || state.dimension1ScanActive ||
            panel.GetAvailableDestinationCountForUi() <= 0) return;
        if (panel.GetSelectedAvailableDestinationForUi() != null &&
            panel.GetSelectedAvailableShipForUi() != null) return;
        ensuringSelections = true;
        panel.EnsureDefaultExploreSelectionsForUi();
        ensuringSelections = false;
    }

    private void RefreshScannerUpgrade(GameState state)
    {
        bool maxed = Dimension1System.IsSimpleScannerMaxed(state);
        Set(scannerUpgradeLabel, maxed ? "ESCÁNER MÁXIMO" : "MEJORAR ESCÁNER");
        if (scannerUpgradeButton != null)
            scannerUpgradeButton.interactable = !maxed && Dimension1System.CanUpgradeSimpleScanner(state);
    }

    private void RefreshPrimaryAction(GameState state, D1ScannedDestinationState destination, D1ShipState ship)
    {
        if (state.dimension1ScanActive)
        {
            Set(startButtonLabel, "ESCANEANDO · " + FormatTimer(state.dimension1ScanRemainingSeconds));
            if (startButton != null) startButton.interactable = false;
            return;
        }

        if ((panel != null ? panel.GetAvailableDestinationCountForUi() : CountDestinations(state)) <= 0)
        {
            Set(startButtonLabel, "ESCANEAR SECTOR");
            if (startButton != null) startButton.interactable = Dimension1System.CanScanSimpleDestination(state);
            return;
        }

        bool coordinated = panel != null && panel.IsCoordinatedModeForUi();
        Set(startButtonLabel, coordinated
            ? "INICIAR EXPEDICIÓN COORDINADA"
            : "INICIAR EXPEDICIÓN");
        if (startButton != null)
            startButton.interactable = panel != null ? panel.CanStartSelectedExplorationForUi() :
                destination != null && ship != null;
    }

    private void RefreshPreviewDetails()
    {
        if (previewDetailsText == null && previewDetailsOverlay != null)
            previewDetailsText = FindNamedComponent<TMP_Text>(previewDetailsOverlay.transform, "DetailsText");
        if (previewDetailsText == null) return;
        previewDetailsText.gameObject.SetActive(true);
        Color detailsColor = previewDetailsText.color;
        detailsColor.a = 1f;
        previewDetailsText.color = detailsColor;
        string details = panel != null ? panel.GetSelectedExplorationPreviewForUi() : "";
        previewDetailsText.text = string.IsNullOrWhiteSpace(details)
            ? "Estado:\nNo hay información disponible para la selección actual."
            : details;
        previewDetailsText.ForceMeshUpdate();
        float height = Mathf.Max(1160f, Mathf.Ceil(previewDetailsText.preferredHeight) + 40f);
        previewDetailsText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height - 24f);
        if (previewDetailsContent == null && previewDetailsOverlay != null)
            previewDetailsContent = FindNamedComponent<RectTransform>(previewDetailsOverlay.transform, "Content");
        if (previewDetailsContent != null)
            previewDetailsContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    private void RefreshDestinationCards(GameState state, D1ScannedDestinationState selected)
    {
        D1ScannedDestinationState[] destinations = AvailableDestinations(state);
        int slots = destinationCardLabels != null ? destinationCardLabels.Length : 0;
        for (int i = 0; i < slots; i++)
        {
            D1ScannedDestinationState destination = i < destinations.Length ? destinations[i] : null;
            bool hasSignal = destination != null;
            bool isSelected = hasSignal && selected != null && destination.destinationId == selected.destinationId;

            if (destinationCardLabels[i] != null)
            {
                destinationCardLabels[i].text = hasSignal ? DestinationName(destination.destinationId) : "SIN SEÑAL";
                destinationCardLabels[i].color = isSelected
                    ? new Color32(244, 167, 11, 255)
                    : hasSignal ? new Color32(237, 244, 247, 255) : new Color32(116, 137, 150, 220);
            }
            if (destinationCardButtons != null && i < destinationCardButtons.Length && destinationCardButtons[i] != null)
                destinationCardButtons[i].interactable = hasSignal;
            if (destinationCardBorders != null && i < destinationCardBorders.Length && destinationCardBorders[i] != null)
                destinationCardBorders[i].color = isSelected
                    ? new Color32(244, 167, 11, 255)
                    : hasSignal ? new Color32(24, 200, 255, 205) : new Color32(64, 86, 98, 180);
            if (destinationCardImages != null && i < destinationCardImages.Length && destinationCardImages[i] != null)
            {
                Sprite art = hasSignal ? FindDestinationArt(destination.destinationId) : null;
                destinationCardImages[i].sprite = art;
                destinationCardImages[i].enabled = art != null;
                destinationCardImages[i].color = hasSignal ? Color.white : new Color(.25f, .32f, .36f, .45f);
            }
        }
    }

    private void RefreshModeCards(bool coordinationUnlocked, bool coordinated)
    {
        if (simpleModeBorder != null)
            simpleModeBorder.color = !coordinated
                ? new Color32(244, 167, 11, 255)
                : new Color32(8, 123, 167, 205);
        if (coordinatedModeBorder != null)
            coordinatedModeBorder.color = coordinated
                ? new Color32(244, 167, 11, 255)
                : new Color32(8, 123, 167, 205);
        Set(simpleModeStatus, coordinated ? "RÁPIDO Y DIRECTO" : "MODO ACTIVO");
        Set(coordinatedModeStatus, !coordinationUnlocked
            ? "BLOQUEADA EN ÁRBOL"
            : coordinated ? "MODO ACTIVO" : "MAYORES RECOMPENSAS");
    }

    private Sprite FindDestinationArt(string destinationId)
    {
        if (destinationArtIds == null || destinationArtSprites == null) return null;
        int count = Mathf.Min(destinationArtIds.Length, destinationArtSprites.Length);
        for (int i = 0; i < count; i++)
            if (destinationArtIds[i] == destinationId) return destinationArtSprites[i];
        return null;
    }

    private static D1ScannedDestinationState[] AvailableDestinations(GameState state)
    {
        if (state == null || state.dimension1ScannedDestinations == null)
            return System.Array.Empty<D1ScannedDestinationState>();
        var result = new System.Collections.Generic.List<D1ScannedDestinationState>();
        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
            if (destination != null && destination.available &&
                destination.sectorId == state.dimension1SelectedSectorId)
                result.Add(destination);
        return result.ToArray();
    }

    private void PrepareInteractionFallbacks()
    {
        if (runtimeOpenDetailsButton == null)
            runtimeOpenDetailsButton = FindNamedComponent<Button>(transform, "OpenDetails");
        if (runtimeOpenDetailsButton != null && !runtimeOpenDetailsBound &&
            !HasPersistentMethod(runtimeOpenDetailsButton, nameof(OpenPreviewDetails)))
        {
            runtimeOpenDetailsButton.onClick.AddListener(OpenPreviewDetails);
            runtimeOpenDetailsBound = true;
        }

        if (runtimeCloseDetailsButton == null && previewDetailsOverlay != null)
            runtimeCloseDetailsButton = FindNamedComponent<Button>(previewDetailsOverlay.transform, "CloseDetails");
        if (runtimeCloseDetailsButton != null && !runtimeCloseDetailsBound &&
            !HasPersistentMethod(runtimeCloseDetailsButton, nameof(ClosePreviewDetails)))
        {
            runtimeCloseDetailsButton.onClick.AddListener(ClosePreviewDetails);
            runtimeCloseDetailsBound = true;
        }

        if (runtimeRecordButton == null)
            runtimeRecordButton = FindNamedComponent<Button>(transform, "ExplorationRecord");
        if (runtimeRecordButton != null && !runtimeRecordBound &&
            !HasPersistentMethod(runtimeRecordButton, nameof(OpenExpeditionRecord)) &&
            !HasPersistentMethod(runtimeRecordButton, "OnClickOpenExplorationRecord"))
        {
            runtimeRecordButton.onClick.AddListener(OpenExpeditionRecord);
            runtimeRecordBound = true;
        }

        Button coordinatedToggle = FindNamedComponent<Button>(transform, "ToggleCoordinated");
        if (coordinatedToggle != null && coordinatedToggle.transform is RectTransform toggleRect)
        {
            toggleRect.anchorMin = Vector2.zero;
            toggleRect.anchorMax = Vector2.one;
            toggleRect.pivot = new Vector2(.5f, .5f);
            toggleRect.offsetMin = new Vector2(16f, 16f);
            toggleRect.offsetMax = new Vector2(-16f, -16f);
        }
    }

    private void RemoveInteractionFallbacks()
    {
        if (runtimeOpenDetailsBound && runtimeOpenDetailsButton != null)
            runtimeOpenDetailsButton.onClick.RemoveListener(OpenPreviewDetails);
        if (runtimeCloseDetailsBound && runtimeCloseDetailsButton != null)
            runtimeCloseDetailsButton.onClick.RemoveListener(ClosePreviewDetails);
        if (runtimeRecordBound && runtimeRecordButton != null)
            runtimeRecordButton.onClick.RemoveListener(OpenExpeditionRecord);
        runtimeOpenDetailsBound = false;
        runtimeCloseDetailsBound = false;
        runtimeRecordBound = false;
    }

    private static bool HasPersistentMethod(Button button, string methodName)
    {
        if (button == null) return false;
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            if (button.onClick.GetPersistentMethodName(i) == methodName) return true;
        return false;
    }

    private static T FindNamedComponent<T>(Transform root, string objectName) where T : Component
    {
        if (root == null) return null;
        foreach (T component in root.GetComponentsInChildren<T>(true))
            if (component != null && component.transform.name == objectName) return component;
        return null;
    }

    private static D1ScannedDestinationState FirstDestination(GameState state, out int total)
    {
        total = 0;
        D1ScannedDestinationState first = null;
        if (state.dimension1ScannedDestinations == null) return null;
        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available ||
                destination.sectorId != state.dimension1SelectedSectorId) continue;
            total++;
            if (first == null) first = destination;
        }
        return first;
    }

    private static int CountDestinations(GameState state)
    {
        FirstDestination(state, out int total);
        return total;
    }

    private static int GetSelectedOrdinal(D1ScannedDestinationState selected, GameState state)
    {
        if (selected == null || state.dimension1ScannedDestinations == null) return 0;
        int ordinal = 0;
        foreach (D1ScannedDestinationState destination in state.dimension1ScannedDestinations)
        {
            if (destination == null || !destination.available ||
                destination.sectorId != state.dimension1SelectedSectorId) continue;
            ordinal++;
            if (ReferenceEquals(destination, selected)) return ordinal;
        }
        return 0;
    }

    private static D1ShipState FirstAvailableShip(GameState state)
    {
        if (state.dimension1Ships == null) return null;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.unlocked && Dimension1System.IsShipActiveInDimension1Base(ship.shipId) &&
                !Dimension1System.IsD1ShipBusy(ship)) return ship;
        return null;
    }

    private static D1ShipState[] ActiveShips(GameState state)
    {
        if (state.dimension1Ships == null) return System.Array.Empty<D1ShipState>();
        var result = new System.Collections.Generic.List<D1ShipState>();
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.explorationActive) result.Add(ship);
        return result.ToArray();
    }

    private static D1ShipState FindShip(GameState state, string id)
    {
        if (state.dimension1Ships == null) return null;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.shipId == id) return ship;
        return null;
    }

    private static string ShipName(string id)
    {
        if (id == Dimension1System.ShipLightProbe) return "SONDA LIGERA";
        if (id == Dimension1System.ShipExtractorDrone) return "DRON EXTRACTOR";
        if (id == Dimension1System.ShipAnalyticProbe) return "SONDA ANALÍTICA";
        if (id == Dimension1System.ShipCargoShip) return "NAVE DE CARGA";
        return string.IsNullOrEmpty(id) ? "NAVE" : id.ToUpperInvariant();
    }

    private void RefreshShipIllustration(Image target, D1ShipState ship)
    {
        if (target == null) return;
        int index = ShipIllustrationIndex(ship != null ? ship.shipId : "");
        bool available = index >= 0 && shipIllustrations != null && index < shipIllustrations.Length &&
            shipIllustrations[index] != null;
        target.enabled = available;
        if (available) target.sprite = shipIllustrations[index];
    }

    private static int ShipIllustrationIndex(string shipId)
    {
        if (shipId == Dimension1System.ShipLightProbe) return 0;
        if (shipId == Dimension1System.ShipExtractorDrone) return 1;
        if (shipId == Dimension1System.ShipAnalyticProbe) return 2;
        if (shipId == Dimension1System.ShipCargoShip) return 3;
        return -1;
    }

    private static string ShipMetricLabel(string shipId)
    {
        if (shipId == Dimension1System.ShipAnalyticProbe) return "SENSORES";
        if (shipId == Dimension1System.ShipExtractorDrone || shipId == Dimension1System.ShipCargoShip)
            return "CARGA";
        return "VELOCIDAD";
    }

    private static int ShipMetricLevel(D1ShipState ship)
    {
        if (ship == null) return 0;
        if (ship.shipId == Dimension1System.ShipAnalyticProbe) return Mathf.Max(0, ship.sensorsLevel);
        if (ship.shipId == Dimension1System.ShipExtractorDrone || ship.shipId == Dimension1System.ShipCargoShip)
            return Mathf.Max(0, ship.cargoLevel);
        return Mathf.Max(0, ship.speedLevel);
    }

    private static D1ShipState FindShipByIdForPreview(string shipId)
    {
        return new D1ShipState { shipId = shipId };
    }

    private static string DestinationName(string id)
    {
        if (id == Dimension1System.DestinationMineralBelt) return "CINTURÓN MINERAL";
        if (id == Dimension1System.DestinationShipGraveyard) return "CEMENTERIO DE NAVES";
        if (id == Dimension1System.DestinationOrbitalRuin) return "RUINA ORBITAL";
        if (id == Dimension1System.DestinationDriftingProbes) return "SONDAS A LA DERIVA";
        if (id == Dimension1System.DestinationLaboratory) return "LABORATORIO";
        if (id == Dimension1System.DestinationAbandonedShip) return "NAVE ABANDONADA";
        if (id == Dimension1System.DestinationAbandonedStation) return "ESTACIÓN ABANDONADA";
        if (id == Dimension1System.DestinationMinorAnomaly) return "ANOMALÍA MENOR";
        if (id == Dimension1System.DestinationAncientStructure) return "ESTRUCTURA ANTIGUA";
        if (id == Dimension1System.DestinationUnstableZone) return "ZONA INESTABLE";
        return string.IsNullOrEmpty(id) ? "SEÑAL DESCONOCIDA" : id.ToUpperInvariant();
    }

    private static string FormatTimer(double seconds)
    {
        int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return string.Format("{0:00}:{1:00}:{2:00}", total / 3600, total / 60 % 60, total % 60);
    }

    private static string FormatAmount(double value)
    {
        value = System.Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        return value.ToString("0.##");
    }

    private static void Set(TMP_Text target, string value) { if (target != null) target.text = value; }
    private static void Set(TMP_Text[] targets, int index, string value)
    {
        if (targets != null && index >= 0 && index < targets.Length && targets[index] != null)
            targets[index].text = value;
    }
}
