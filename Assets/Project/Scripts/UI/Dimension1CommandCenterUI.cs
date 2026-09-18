using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Dimension1CommandCenterUI : MonoBehaviour
{
    [Header("Visibilidad")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject legacyMainContent;
    [SerializeField] private GameObject[] blockingPanels;
    [SerializeField] private VerticalNavigationUI verticalNavigation;

    [Header("Datos")]
    [SerializeField] private Text[] metalValueTexts;
    [SerializeField] private Text[] metalRateTexts;
    [SerializeField] private Text[] drawerMetalValueTexts;
    [SerializeField] private Text sectorValueText;
    [SerializeField] private Text scannerValueText;
    [SerializeField] private Text fleetValueText;
    [SerializeField] private Text relicsValueText;
    [SerializeField] private Text treePointsValueText;
    [SerializeField] private Text activeExpeditionsValueText;
    [SerializeField] private Text objectiveValueText;
    [SerializeField] private Text progressValueText;
    [SerializeField] private Dimension1CommandCenterLineGraphic progressLine;

    [Header("Interacción")]
    [SerializeField] private Button metalsButton;
    [SerializeField] private GameObject metalsDrawer;
    [SerializeField] private Button[] galaxyButtons;
    [SerializeField] private Button[] exploreButtons;
    [SerializeField] private Button[] hangarButtons;
    [SerializeField] private Button[] relicButtons;
    [SerializeField] private Button[] treeButtons;
    [SerializeField] private Button galaxyProxy;
    [SerializeField] private Button exploreProxy;
    [SerializeField] private Button hangarProxy;
    [SerializeField] private Button relicProxy;
    [SerializeField] private Button treeProxy;
    [SerializeField] private Button dimensionDrawerToggle;
    [SerializeField] private Text dimensionDrawerToggleLabel;
    [SerializeField] private GameObject[] dimension1NavigationCards;
    [SerializeField] private GameObject exploreScreenRoot;

    [Header("Animación")]
    [SerializeField] private RectTransform crystal;

    private float refreshTimer;
    private float animationTime;
    private bool listenersBound;
    private bool commandCenterDrawerModeApplied;
    private bool dimensionDrawerExpanded;
    private bool commandCenterRequested = true;

    public void ConfigureExploreScreen(GameObject configuredExploreScreenRoot)
    {
        exploreScreenRoot = configuredExploreScreenRoot;
        Dimension1ExploreVisualUI exploreVisual =
            exploreScreenRoot != null
                ? exploreScreenRoot.GetComponent<Dimension1ExploreVisualUI>()
                : null;
        if (exploreVisual != null)
            exploreVisual.ConfigureCommandCenter(this);
    }

    public void ShowCommandCenterScreen()
    {
        commandCenterRequested = true;
        Dimension1PanelUI functionalPanel =
            GetComponentInParent<Dimension1PanelUI>(true);
        if (functionalPanel != null)
            functionalPanel.PrepareCommandCenterForUi();

        if (exploreScreenRoot != null)
            exploreScreenRoot.SetActive(false);

        transform.SetAsLastSibling();
        RefreshVisibility();
    }

    public void ShowExploreScreen()
    {
        if (!ResolveExploreScreen())
            return;

        commandCenterRequested = false;
        exploreScreenRoot.transform.SetAsLastSibling();
        exploreScreenRoot.SetActive(true);

        Dimension1PanelUI functionalPanel =
            GetComponentInParent<Dimension1PanelUI>(true);
        if (functionalPanel != null)
            functionalPanel.PrepareExploreForUi();

        Dimension1ExploreVisualUI exploreVisual =
            exploreScreenRoot.GetComponent<Dimension1ExploreVisualUI>();
        if (exploreVisual != null)
            exploreVisual.PrepareForPresentationForUi();

        RefreshVisibility();
    }

    public void HideExploreScreenForSecondaryUi()
    {
        if (exploreScreenRoot != null && exploreScreenRoot.activeSelf)
            exploreScreenRoot.SetActive(false);
        RefreshVisibility();
    }

    public void Configure(
        CanvasGroup configuredCanvasGroup,
        GameObject configuredLegacyMainContent,
        GameObject[] configuredBlockingPanels,
        VerticalNavigationUI configuredVerticalNavigation,
        Text[] configuredMetalValues,
        Text[] configuredMetalRates,
        Text[] configuredDrawerMetalValues,
        Text configuredSectorValue,
        Text configuredScannerValue,
        Text configuredFleetValue,
        Text configuredRelicsValue,
        Text configuredTreePointsValue,
        Text configuredActiveExpeditionsValue,
        Text configuredObjectiveValue,
        Text configuredProgressValue,
        Dimension1CommandCenterLineGraphic configuredProgressLine,
        Button configuredMetalsButton,
        GameObject configuredMetalsDrawer,
        Button[] configuredGalaxyButtons,
        Button[] configuredExploreButtons,
        Button[] configuredHangarButtons,
        Button[] configuredRelicButtons,
        Button[] configuredTreeButtons,
        Button configuredGalaxyProxy,
        Button configuredExploreProxy,
        Button configuredHangarProxy,
        Button configuredRelicProxy,
        Button configuredTreeProxy,
        Button configuredDimensionDrawerToggle,
        Text configuredDimensionDrawerToggleLabel,
        GameObject[] configuredDimension1NavigationCards,
        RectTransform configuredCrystal)
    {
        canvasGroup = configuredCanvasGroup;
        legacyMainContent = configuredLegacyMainContent;
        blockingPanels = configuredBlockingPanels;
        verticalNavigation = configuredVerticalNavigation;
        metalValueTexts = configuredMetalValues;
        metalRateTexts = configuredMetalRates;
        drawerMetalValueTexts = configuredDrawerMetalValues;
        sectorValueText = configuredSectorValue;
        scannerValueText = configuredScannerValue;
        fleetValueText = configuredFleetValue;
        relicsValueText = configuredRelicsValue;
        treePointsValueText = configuredTreePointsValue;
        activeExpeditionsValueText = configuredActiveExpeditionsValue;
        objectiveValueText = configuredObjectiveValue;
        progressValueText = configuredProgressValue;
        progressLine = configuredProgressLine;
        metalsButton = configuredMetalsButton;
        metalsDrawer = configuredMetalsDrawer;
        galaxyButtons = configuredGalaxyButtons;
        exploreButtons = configuredExploreButtons;
        hangarButtons = configuredHangarButtons;
        relicButtons = configuredRelicButtons;
        treeButtons = configuredTreeButtons;
        galaxyProxy = configuredGalaxyProxy;
        exploreProxy = configuredExploreProxy;
        hangarProxy = configuredHangarProxy;
        relicProxy = configuredRelicProxy;
        treeProxy = configuredTreeProxy;
        dimensionDrawerToggle = configuredDimensionDrawerToggle;
        dimensionDrawerToggleLabel = configuredDimensionDrawerToggleLabel;
        dimension1NavigationCards = configuredDimension1NavigationCards;
        crystal = configuredCrystal;
    }

    private void OnEnable()
    {
        ResolveExploreScreen();
        BindListeners();
        if (exploreScreenRoot != null && exploreScreenRoot.activeSelf)
            commandCenterRequested = false;
        else if (commandCenterRequested)
        {
            Dimension1PanelUI functionalPanel =
                GetComponentInParent<Dimension1PanelUI>(true);
            if (functionalPanel != null)
                functionalPanel.SetModernCommandCenterPresentedForUi(true);
            transform.SetAsLastSibling();
        }
        RefreshVisibility();
        RefreshData();
    }

    private bool ResolveExploreScreen()
    {
        if (exploreScreenRoot != null)
            return true;

        Dimension1ExploreVisualUI exploreVisual =
            FindFirstObjectByType<Dimension1ExploreVisualUI>(FindObjectsInactive.Include);
        if (exploreVisual == null)
            return false;

        exploreScreenRoot = exploreVisual.gameObject;
        exploreVisual.ConfigureCommandCenter(this);
        return true;
    }

    private void OnDisable()
    {
        UnbindListeners();
        SetCommandCenterDrawerMode(false);
    }

    private void Update()
    {
        RefreshVisibility();

        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer <= 0f)
        {
            refreshTimer = 0.25f;
            RefreshData();
        }

        animationTime += Time.unscaledDeltaTime;
        if (crystal != null)
        {
            float pulse = 1f + Mathf.Sin(animationTime * 1.35f) * 0.012f;
            crystal.localScale = new Vector3(pulse, pulse, 1f);
        }
    }

    private void RefreshVisibility()
    {
        if (canvasGroup == null) return;

        bool visible = commandCenterRequested;
        if (commandCenterRequested && legacyMainContent != null && legacyMainContent.activeSelf)
            legacyMainContent.SetActive(false);
        if (exploreScreenRoot != null && exploreScreenRoot.activeSelf)
            visible = false;
        if (blockingPanels != null)
        {
            foreach (GameObject panel in blockingPanels)
            {
                if (panel != null && panel.activeInHierarchy)
                {
                    visible = false;
                    break;
                }
            }
        }

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
        SetCommandCenterDrawerMode(visible);
        if (!visible && metalsDrawer != null && metalsDrawer.activeSelf)
            metalsDrawer.SetActive(false);
    }

    private void SetCommandCenterDrawerMode(bool active)
    {
        if (commandCenterDrawerModeApplied == active)
            return;

        commandCenterDrawerModeApplied = active;
        dimensionDrawerExpanded = false;
        if (verticalNavigation != null)
            verticalNavigation.SetCommandCenterDrawerMode(active);
        SetDimension1NavigationVisible(true);
        RefreshDimensionDrawerLabel();
    }

    private void ToggleDimensionDrawer()
    {
        if (!commandCenterDrawerModeApplied)
            return;

        dimensionDrawerExpanded = !dimensionDrawerExpanded;
        if (verticalNavigation != null)
            verticalNavigation.SetCommandCenterDrawerExpanded(dimensionDrawerExpanded);
        SetDimension1NavigationVisible(!dimensionDrawerExpanded);
        RefreshDimensionDrawerLabel();
    }

    private void SetDimension1NavigationVisible(bool visible)
    {
        if (dimension1NavigationCards == null)
            return;
        foreach (GameObject card in dimension1NavigationCards)
            if (card != null) card.SetActive(visible);
    }

    private void RefreshDimensionDrawerLabel()
    {
        if (dimensionDrawerToggleLabel != null)
        {
            dimensionDrawerToggleLabel.text = dimensionDrawerExpanded
                ? "CENTRO DE MANDO ▲"
                : "DIMENSIONES ▼";
        }
    }

    private void RefreshData()
    {
        GameState state = GameState.I;
        if (state == null) return;

        state.EnsureDimension1State();
        Dimension1HeaderMetalsUI.Refresh(transform, state, state.dimension1SelectedSectorId);

        if (drawerMetalValueTexts != null)
        {
            for (int i = 0; i < drawerMetalValueTexts.Length && i < Dimension1System.StarterMetals.Length; i++)
            {
                if (drawerMetalValueTexts[i] != null)
                    drawerMetalValueTexts[i].text = FormatAmount(
                        state.GetD1MetalAmount(Dimension1System.StarterMetals[i]));
            }
        }

        int unlockedShips = 0;
        int activeExpeditions = 0;
        if (state.dimension1Ships != null)
        {
            foreach (D1ShipState ship in state.dimension1Ships)
            {
                if (ship == null || !ship.unlocked ||
                    !Dimension1System.IsShipActiveInDimension1Base(ship.shipId))
                {
                    continue;
                }

                unlockedShips++;
                if (ship.explorationActive) activeExpeditions++;
            }
        }
        if (state.dimension1ArkFinalMissionActive) activeExpeditions++;

        int unlockedRelics = 0;
        if (state.dimension1Relics != null)
        {
            foreach (D1RelicState relic in state.dimension1Relics)
                if (relic != null && (relic.unlocked || relic.level > 0)) unlockedRelics++;
        }

        int progressPoints = Dimension1System.CalculateD1TreePointsFromProgress(state);
        int progressCap = Mathf.Max(1, Dimension1System.Dimension1Prestige1PreviewPointCap);
        int progressPercent = Mathf.Clamp(Mathf.RoundToInt(progressPoints * 100f / progressCap), 0, 100);

        SetText(sectorValueText, GetSectorName(state.dimension1SelectedSectorId));
        SetText(scannerValueText, "NIVEL " + Mathf.Clamp(state.dimension1ScannerLevel, 1, 15) + "/15");
        SetText(fleetValueText, unlockedShips + (unlockedShips == 1 ? " NAVE" : " NAVES"));
        SetText(relicsValueText, unlockedRelics + "/" + Dimension1System.Dimension1RelicIds.Length);
        SetText(treePointsValueText, Mathf.Max(0, state.d1TreePoints).ToString());
        SetText(activeExpeditionsValueText, activeExpeditions.ToString());
        SetText(progressValueText, progressPercent + "%");
        SetProgress(progressPercent / 100f);

        string objective = state.dimension1GalacticAnchorDiscovered
            ? "Ancla galáctica asegurada."
            : state.dimension1ScanActive
                ? "Espera el resultado del escáner."
                : activeExpeditions > 0
                    ? "Completa las expediciones activas."
                    : "Explora 3 señales desconocidas.";
        SetText(objectiveValueText, objective);
    }

    private void SetProgress(float progress01)
    {
        if (progressLine == null) return;
        const int segments = 52;
        var points = new List<Vector2>();
        int count = Mathf.Max(1, Mathf.RoundToInt(segments * Mathf.Clamp01(progress01)));
        for (int i = 0; i <= count; i++)
        {
            float angle = Mathf.PI * 0.5f - Mathf.PI * 2f * i / segments;
            points.Add(new Vector2(428, 0) + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 42f);
        }
        progressLine.SetLine(points, 9f);
    }

    private void BindListeners()
    {
        if (listenersBound) return;
        listenersBound = true;
        if (metalsButton != null && !HasPersistentMetalsRoute(metalsButton))
            metalsButton.onClick.AddListener(ToggleMetalsDrawer);
        if (dimensionDrawerToggle != null) dimensionDrawerToggle.onClick.AddListener(ToggleDimensionDrawer);
        BindGroup(galaxyButtons, OpenGalaxy);
        BindGroup(exploreButtons, OpenExplore);
        BindGroup(hangarButtons, OpenHangar);
        BindGroup(relicButtons, OpenRelics);
        BindGroup(treeButtons, OpenTree);
    }

    private void UnbindListeners()
    {
        if (!listenersBound) return;
        listenersBound = false;
        if (metalsButton != null) metalsButton.onClick.RemoveListener(ToggleMetalsDrawer);
        if (dimensionDrawerToggle != null) dimensionDrawerToggle.onClick.RemoveListener(ToggleDimensionDrawer);
        UnbindGroup(galaxyButtons, OpenGalaxy);
        UnbindGroup(exploreButtons, OpenExplore);
        UnbindGroup(hangarButtons, OpenHangar);
        UnbindGroup(relicButtons, OpenRelics);
        UnbindGroup(treeButtons, OpenTree);
    }

    private static void BindGroup(Button[] buttons, UnityEngine.Events.UnityAction action)
    {
        if (buttons == null) return;
        foreach (Button button in buttons) if (button != null) button.onClick.AddListener(action);
    }

    private static void UnbindGroup(Button[] buttons, UnityEngine.Events.UnityAction action)
    {
        if (buttons == null) return;
        foreach (Button button in buttons) if (button != null) button.onClick.RemoveListener(action);
    }

    private void ToggleMetalsDrawer()
    {
        if (Dimension1MetalsInventoryUI.I != null)
        {
            if (metalsDrawer != null) metalsDrawer.SetActive(false);
            Dimension1MetalsInventoryUI.I.Open();
            return;
        }
        if (metalsDrawer != null) metalsDrawer.SetActive(!metalsDrawer.activeSelf);
    }

    private static bool HasPersistentMetalsRoute(Button button)
    {
        if (button == null) return false;
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            string method = button.onClick.GetPersistentMethodName(i);
            if (button.onClick.GetPersistentTarget(i) != null &&
                (method == "Open" || method == "OpenMetals" || method == "ToggleMetalsDrawer"))
                return true;
        }
        return false;
    }

    private void OpenGalaxy()
    {
        if (exploreScreenRoot != null) exploreScreenRoot.SetActive(false);
        InvokeProxy(galaxyProxy);
    }

    private void OpenExplore()
    {
        if (exploreScreenRoot != null)
        {
            ShowExploreScreen();
            return;
        }
        InvokeProxy(exploreProxy);
    }

    private void OpenHangar()
    {
        if (exploreScreenRoot != null) exploreScreenRoot.SetActive(false);
        InvokeProxy(hangarProxy);
    }

    private void OpenRelics()
    {
        if (exploreScreenRoot != null) exploreScreenRoot.SetActive(false);
        InvokeProxy(relicProxy);
    }

    private void OpenTree()
    {
        if (exploreScreenRoot != null) exploreScreenRoot.SetActive(false);
        InvokeProxy(treeProxy);
    }

    private static void InvokeProxy(Button proxy)
    {
        if (proxy != null && proxy.interactable) proxy.onClick.Invoke();
    }

    private static void SetText(Text text, string value)
    {
        if (text != null) text.text = value;
    }

    private static string FormatAmount(double value)
    {
        value = System.Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        return value.ToString("0.##");
    }

    private static string GetSectorName(string sectorId)
    {
        if (sectorId == Dimension1System.Sector01OuterRim) return "BORDE\nEXTERIOR";
        if (sectorId == Dimension1System.Sector02DebrisRing) return "ANILLO DE\nRESTOS";
        if (sectorId == Dimension1System.Sector03AncientOrbits) return "ÓRBITAS\nANTIGUAS";
        if (sectorId == Dimension1System.Sector04SilentFrontier) return "FRONTERA\nSILENCIOSA";
        if (sectorId == Dimension1System.Sector05GalacticCenter) return "CENTRO\nGALÁCTICO";
        return "SECTOR\nDESCONOCIDO";
    }
}
