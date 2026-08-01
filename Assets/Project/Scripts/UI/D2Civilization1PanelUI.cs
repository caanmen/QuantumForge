using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2Civilization1PanelUI : MonoBehaviour
{
    public GameObject refugeSectionRoot;
    public GameObject altarsSectionRoot;
    public GameObject pilgrimagesSectionRoot;
    public GameObject novitiateSectionRoot;
    public GameObject ritesSectionRoot;
    public GameObject pactsSectionRoot;
    public GameObject veiledThresholdSectionRoot;
    public Button showRefugeButton;
    public Button showAltarsButton;
    public Button showPilgrimagesButton;
    public Button showNovitiateButton;
    public Button showRitesButton;
    public Button showPactsButton;
    public Button showVeiledThresholdButton;
    public D2AltarsPanelUI altarsPanelUI;
    public D2PilgrimagesPanelUI pilgrimagesPanelUI;
    public D2NovitiatePanelUI novitiatePanelUI;
    public D2RitesPanelUI ritesPanelUI;
    public D2CivilizationPactsPanelUI pactsPanelUI;
    public D2VeiledThresholdPanelUI veiledThresholdPanelUI;
    public TMP_Text followersText;
    public TMP_Text arrivalText;
    public TMP_Text refugeText;
    public TMP_Text assignmentText;
    public Slider arrivalProgressSlider;
    public Button assignOneButton;
    public Button assignTenButton;
    public Button assignAllButton;
    public Button releaseOneButton;
    public Button releaseAllButton;
    public Button upgradeRefugeButton;
    public TMP_Text upgradeRefugeButtonText;
    public TMP_Text objectiveText;

    private float _refreshTimer;

    private void Awake()
    {
        if (showRefugeButton != null)
            showRefugeButton.onClick.AddListener(ShowRefugeSection);

        if (showAltarsButton != null)
            showAltarsButton.onClick.AddListener(ShowAltarsSection);

        if (showPilgrimagesButton != null)
            showPilgrimagesButton.onClick.AddListener(ShowPilgrimagesSection);

        if (showNovitiateButton != null)
            showNovitiateButton.onClick.AddListener(ShowNovitiateSection);

        if (showRitesButton != null)
            showRitesButton.onClick.AddListener(ShowRitesSection);

        if (showPactsButton != null)
            showPactsButton.onClick.AddListener(ShowPactsSection);

        if (showVeiledThresholdButton != null)
            showVeiledThresholdButton.onClick.AddListener(ShowVeiledThresholdSection);

        if (assignOneButton != null)
            assignOneButton.onClick.AddListener(AssignOne);

        if (assignTenButton != null)
            assignTenButton.onClick.AddListener(AssignTen);

        if (assignAllButton != null)
            assignAllButton.onClick.AddListener(AssignAll);

        if (releaseOneButton != null)
            releaseOneButton.onClick.AddListener(ReleaseOne);

        if (releaseAllButton != null)
            releaseAllButton.onClick.AddListener(ReleaseAll);

        if (upgradeRefugeButton != null)
            upgradeRefugeButton.onClick.AddListener(UpgradeRefuge);
    }

    private void OnEnable()
    {
        _refreshTimer = 0f;
        ShowRefugeSection();
        Refresh();
    }

    private void Update()
    {
        _refreshTimer -= Time.unscaledDeltaTime;
        if (_refreshTimer > 0f)
            return;

        _refreshTimer = 0.2f;
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState))
            return;

        gameState.EnsureDimension2State();
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2Civilization1System.EnsureState(state);

        long totalFollowers = D2Civilization1System.GetTotalFollowers(state);
        double arrivalPerSecond = D2Civilization1System.GetFollowerArrivalPerSecond(state);
        double assignmentMultiplier = D2Civilization1System.GetAssignedFollowerMultiplier(state);
        long upgradeCost = D2Civilization1System.GetNextRefugeUpgradeCost(state);
        bool refugeMaxed = state.refugeLevel >= D2Civilization1System.MaxRefugeLevel;
        bool veiledThresholdUnlocked = D2VeiledThresholdSystem.IsUnlocked(state);
        RefreshProgressivePresentation(gameState, state);

        SetText(
            followersText,
            "SEGUIDORES\n" +
            "Disponibles: " + state.followersAvailable.ToString("N0") +
            "   |   Totales presentes: " + totalFollowers.ToString("N0") +
            "   |   Llegados históricamente: " + state.totalFollowersReceived.ToString("N0")
        );

        SetText(
            arrivalText,
            "Llegada: " + arrivalPerSecond.ToString("0.000") + "/s" +
            "   (" + (arrivalPerSecond * 60.0).ToString("0.00") + "/min)"
        );

        SetText(
            refugeText,
            "REFUGIO DE PEREGRINOS — Nivel " + state.refugeLevel +
            "/" + D2Civilization1System.MaxRefugeLevel
        );

        SetText(
            assignmentText,
            "Asignados al Refugio: " + state.followersAssignedToRefuge.ToString("N0") +
            "   |   Multiplicador por apoyo: ×" + assignmentMultiplier.ToString("0.000")
        );

        if (arrivalProgressSlider != null)
        {
            arrivalProgressSlider.minValue = 0f;
            arrivalProgressSlider.maxValue = 1f;
            arrivalProgressSlider.value = (float)state.followerArrivalProgress;
        }

        SetInteractable(assignOneButton, state.followersAvailable >= 1L);
        SetInteractable(assignTenButton, state.followersAvailable >= 1L);
        SetInteractable(assignAllButton, state.followersAvailable >= 1L);
        SetInteractable(releaseOneButton, state.followersAssignedToRefuge >= 1L);
        SetInteractable(releaseAllButton, state.followersAssignedToRefuge >= 1L);
        SetInteractable(
            upgradeRefugeButton,
            !refugeMaxed && D2Civilization1System.CanUpgradeRefuge(gameState)
        );

        SetText(
            upgradeRefugeButtonText,
            refugeMaxed
                ? "REFUGIO AL MÁXIMO"
                : "MEJORAR REFUGIO\nCoste: " + upgradeCost.ToString("N0") + " Seguidores"
        );

        if (altarsPanelUI != null)
            altarsPanelUI.Refresh();

        if (pilgrimagesPanelUI != null)
            pilgrimagesPanelUI.Refresh();

        if (novitiatePanelUI != null)
            novitiatePanelUI.Refresh();

        if (ritesPanelUI != null)
            ritesPanelUI.Refresh();

        if (pactsPanelUI != null)
            pactsPanelUI.Refresh();

        if (showVeiledThresholdButton != null)
        {
            SetInteractable(showVeiledThresholdButton, veiledThresholdUnlocked);
            TMP_Text label = showVeiledThresholdButton.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                label.text = D2BondSystem.IsMajorPactEstablished(state) ? "PACTO" : "UMBRAL";
        }

        if (veiledThresholdPanelUI != null)
            veiledThresholdPanelUI.Refresh();
    }

    private void RefreshProgressivePresentation(
        GameState gameState, D2Civilization1State state)
    {
        bool refugeLearned =
            D2Civilization1PresentationRules.HasLearnedRefuge(state);
        FeaturePresentationState altars = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C1Altars);
        FeaturePresentationState pilgrimages = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C1Pilgrimages);
        FeaturePresentationState novitiate = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C1Novitiate);
        FeaturePresentationState rites = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C1Rites);
        FeaturePresentationState pacts = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C1Pacts);
        FeaturePresentationState threshold = D2PresentationRules.GetFeatureState(
            gameState, PresentationFeatureIds.D2C1VeiledThreshold);

        SetActive(showRefugeButton, true);
        SetActive(showAltarsButton, altars.IsVisible);
        SetActive(showPilgrimagesButton, pilgrimages.IsVisible);
        SetActive(showNovitiateButton, novitiate.IsVisible);
        SetActive(showRitesButton, rites.IsVisible);
        SetActive(showPactsButton, pacts.IsVisible);
        SetActive(showVeiledThresholdButton, threshold.IsVisible);
        SetInteractable(showAltarsButton, altars.CanOpen);
        SetInteractable(showPilgrimagesButton, pilgrimages.CanOpen);
        SetInteractable(showNovitiateButton, novitiate.CanOpen);
        SetInteractable(showRitesButton, rites.CanOpen);
        SetInteractable(showPactsButton, pacts.CanOpen);
        SetInteractable(showVeiledThresholdButton, threshold.CanOpen);
        SetButtonLabel(showAltarsButton, "ALTARES", altars.isNew);
        SetButtonLabel(showPilgrimagesButton, "PEREGRINACIONES", pilgrimages.isNew);
        SetButtonLabel(showNovitiateButton, "NOVICIADO", novitiate.isNew);
        SetButtonLabel(showRitesButton, "RITOS", rites.isNew);
        SetButtonLabel(showPactsButton,
            pacts.CanOpen ? "PACTOS" : "PACTOS · PRÓXIMO", pacts.isNew);
        SetButtonLabel(showVeiledThresholdButton, "UMBRAL", threshold.isNew);

        SetActive(assignTenButton, refugeLearned);
        SetActive(assignAllButton, refugeLearned);
        SetActive(releaseAllButton, refugeLearned);
        SetActive(upgradeRefugeButton, refugeLearned);

        string objective;
        if (!refugeLearned)
            objective = "AHORA · Asigna 1 Seguidor al Refugio.\nDESPUÉS · Los Altares responderán.";
        else if (!pilgrimages.IsVisible)
            objective = "AHORA · Asigna 1 Seguidor a Cera y 1 a Pan; reúne 2 de cada Ofrenda.\nDESPUÉS · Peregrinación Corta.";
        else if (state.totalPilgrimagesCompleted == 0L)
            objective = "AHORA · Inicia la Peregrinación Corta.\nDESPUÉS · Confianza y nuevas rutas.";
        else if (novitiate.CanOpen && state.totalAcolytesCreated == 0L)
            objective = "AHORA · Forma tu primer Acólito.\nDESPUÉS · Ritos del Santuario.";
        else if (rites.CanOpen && D2RiteSystem.GetActiveRiteCount(state) == 0)
            objective = "AHORA · Activa un Rito asignando una unidad.\nDESPUÉS · Fortalece la Confianza.";
        else if (threshold.CanOpen && !state.bondPlacePrepared)
            objective = "AHORA · Prepara el Lugar de Vínculo.\nDESPUÉS · Asigna Acólitos al vínculo.";
        else if (pacts.CanOpen)
            objective = "AHORA · Inspecciona un Pacto y confirma su compromiso.\nDESPUÉS · Otro territorio a 300 de Confianza.";
        else if (pacts.IsVisible)
            objective = "AHORA · Acércate al requisito real de Pactos.\nDESPUÉS · Compromisos permanentes.";
        else
            objective = "AHORA · Eleva la Confianza hacia 300.\nDESPUÉS · Se localizará otro territorio.";
        SetText(objectiveText, objective);
    }

    public void ShowRefugeSection()
    {
        RecognizeSection(PresentationFeatureIds.D2C1Refuge);
        if (refugeSectionRoot != null)
            refugeSectionRoot.SetActive(true);

        if (altarsSectionRoot != null)
            altarsSectionRoot.SetActive(false);

        if (pilgrimagesSectionRoot != null)
            pilgrimagesSectionRoot.SetActive(false);

        if (novitiateSectionRoot != null)
            novitiateSectionRoot.SetActive(false);

        if (ritesSectionRoot != null)
            ritesSectionRoot.SetActive(false);
        if (pactsSectionRoot != null)
            pactsSectionRoot.SetActive(false);
        if (veiledThresholdSectionRoot != null)
            veiledThresholdSectionRoot.SetActive(false);

        Refresh();
    }

    public void ShowAltarsSection()
    {
        RecognizeSection(PresentationFeatureIds.D2C1Altars);
        if (refugeSectionRoot != null)
            refugeSectionRoot.SetActive(false);

        if (altarsSectionRoot != null)
            altarsSectionRoot.SetActive(true);

        if (pilgrimagesSectionRoot != null)
            pilgrimagesSectionRoot.SetActive(false);

        if (novitiateSectionRoot != null)
            novitiateSectionRoot.SetActive(false);

        if (ritesSectionRoot != null)
            ritesSectionRoot.SetActive(false);
        if (pactsSectionRoot != null)
            pactsSectionRoot.SetActive(false);
        if (veiledThresholdSectionRoot != null)
            veiledThresholdSectionRoot.SetActive(false);

        if (altarsPanelUI != null)
        {
            altarsPanelUI.ConfigureDropdown();
            altarsPanelUI.Refresh();
        }
    }

    public void ShowPilgrimagesSection()
    {
        RecognizeSection(PresentationFeatureIds.D2C1Pilgrimages);
        if (refugeSectionRoot != null)
            refugeSectionRoot.SetActive(false);

        if (altarsSectionRoot != null)
            altarsSectionRoot.SetActive(false);

        if (pilgrimagesSectionRoot != null)
            pilgrimagesSectionRoot.SetActive(true);

        if (novitiateSectionRoot != null)
            novitiateSectionRoot.SetActive(false);

        if (ritesSectionRoot != null)
            ritesSectionRoot.SetActive(false);
        if (pactsSectionRoot != null)
            pactsSectionRoot.SetActive(false);
        if (veiledThresholdSectionRoot != null)
            veiledThresholdSectionRoot.SetActive(false);

        if (pilgrimagesPanelUI != null)
            pilgrimagesPanelUI.Refresh();
    }

    public void ShowNovitiateSection()
    {
        RecognizeSection(PresentationFeatureIds.D2C1Novitiate);
        if (refugeSectionRoot != null)
            refugeSectionRoot.SetActive(false);
        if (altarsSectionRoot != null)
            altarsSectionRoot.SetActive(false);
        if (pilgrimagesSectionRoot != null)
            pilgrimagesSectionRoot.SetActive(false);
        if (novitiateSectionRoot != null)
            novitiateSectionRoot.SetActive(true);
        if (ritesSectionRoot != null)
            ritesSectionRoot.SetActive(false);
        if (pactsSectionRoot != null)
            pactsSectionRoot.SetActive(false);
        if (veiledThresholdSectionRoot != null)
            veiledThresholdSectionRoot.SetActive(false);
        if (novitiatePanelUI != null)
            novitiatePanelUI.Refresh();
    }

    public void ShowRitesSection()
    {
        RecognizeSection(PresentationFeatureIds.D2C1Rites);
        if (refugeSectionRoot != null)
            refugeSectionRoot.SetActive(false);
        if (altarsSectionRoot != null)
            altarsSectionRoot.SetActive(false);
        if (pilgrimagesSectionRoot != null)
            pilgrimagesSectionRoot.SetActive(false);
        if (novitiateSectionRoot != null)
            novitiateSectionRoot.SetActive(false);
        if (ritesSectionRoot != null)
            ritesSectionRoot.SetActive(true);
        if (pactsSectionRoot != null)
            pactsSectionRoot.SetActive(false);
        if (veiledThresholdSectionRoot != null)
            veiledThresholdSectionRoot.SetActive(false);
        if (ritesPanelUI != null)
        {
            ritesPanelUI.ConfigureDropdown();
            ritesPanelUI.Refresh();
        }
    }

    public void ShowPactsSection()
    {
        RecognizeSection(PresentationFeatureIds.D2C1Pacts);
        if (refugeSectionRoot != null)
            refugeSectionRoot.SetActive(false);
        if (altarsSectionRoot != null)
            altarsSectionRoot.SetActive(false);
        if (pilgrimagesSectionRoot != null)
            pilgrimagesSectionRoot.SetActive(false);
        if (novitiateSectionRoot != null)
            novitiateSectionRoot.SetActive(false);
        if (ritesSectionRoot != null)
            ritesSectionRoot.SetActive(false);
        if (pactsSectionRoot != null)
            pactsSectionRoot.SetActive(true);
        if (veiledThresholdSectionRoot != null)
            veiledThresholdSectionRoot.SetActive(false);
        if (pactsPanelUI != null)
        {
            pactsPanelUI.ConfigureDropdown();
            pactsPanelUI.Refresh();
        }
    }

    public void ShowVeiledThresholdSection()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState))
            return;

        gameState.EnsureDimension2State();
        if (!D2VeiledThresholdSystem.IsUnlocked(gameState.dimension2.civilization1))
            return;
        RecognizeSection(PresentationFeatureIds.D2C1VeiledThreshold);

        if (refugeSectionRoot != null)
            refugeSectionRoot.SetActive(false);
        if (altarsSectionRoot != null)
            altarsSectionRoot.SetActive(false);
        if (pilgrimagesSectionRoot != null)
            pilgrimagesSectionRoot.SetActive(false);
        if (novitiateSectionRoot != null)
            novitiateSectionRoot.SetActive(false);
        if (ritesSectionRoot != null)
            ritesSectionRoot.SetActive(false);
        if (pactsSectionRoot != null)
            pactsSectionRoot.SetActive(false);
        if (veiledThresholdSectionRoot != null)
            veiledThresholdSectionRoot.SetActive(true);
        if (veiledThresholdPanelUI != null)
            veiledThresholdPanelUI.Refresh();
    }

    private static void RecognizeSection(string featureId)
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.presentation == null) return;
        D2PresentationRouter.RememberScreen(gameState, featureId);
        PresentationStateUtility.Acknowledge(
            gameState.dimension2.presentation, featureId);
    }

    public void AssignOne()
    {
        D2Civilization1System.TryAssignFollowersToRefuge(GameState.I, 1L);
        Refresh();
    }

    public void AssignTen()
    {
        D2Civilization1System.TryAssignFollowersToRefuge(GameState.I, 10L);
        Refresh();
    }

    public void AssignAll()
    {
        D2Civilization1System.TryAssignAllFollowersToRefuge(GameState.I);
        Refresh();
    }

    public void ReleaseOne()
    {
        D2Civilization1System.TryReleaseFollowersFromRefuge(GameState.I, 1L);
        Refresh();
    }

    public void ReleaseAll()
    {
        D2Civilization1System.TryReleaseAllFollowersFromRefuge(GameState.I);
        Refresh();
    }

    public void UpgradeRefuge()
    {
        D2Civilization1System.TryUpgradeRefuge(GameState.I);
        Refresh();
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private static void SetInteractable(Button button, bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }

    private static void SetActive(Component component, bool active)
    {
        if (component != null) component.gameObject.SetActive(active);
    }

    private static void SetButtonLabel(Button button, string label, bool isNew)
    {
        if (button == null) return;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = (isNew ? "NUEVO · " : "") + label;
    }
}
