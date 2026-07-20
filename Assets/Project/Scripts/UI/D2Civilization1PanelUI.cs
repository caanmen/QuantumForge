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
            SetInteractable(showVeiledThresholdButton, veiledThresholdUnlocked);

        if (veiledThresholdPanelUI != null)
            veiledThresholdPanelUI.Refresh();
    }

    public void ShowRefugeSection()
    {
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
}
