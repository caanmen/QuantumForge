using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2Civilization2PanelUI : MonoBehaviour
{
    private float _refreshTimer;

    public Dimension2PanelUI dimension2PanelUI;
    public GameObject regionSectionRoot;
    public GameObject operationsSectionRoot;
    public GameObject defenseSectionRoot;
    public GameObject resistanceSectionRoot;
    public GameObject alertSectionRoot;
    public GameObject containmentSectionRoot;
    public Button showRegionsButton;
    public Button showOperationsButton;
    public Button showDefenseButton;
    public Button showResistanceButton;
    public Button showAlertButton;
    public Button showContainmentButton;
    public TMP_Dropdown regionDropdown;
    public D2OperationsPanelUI operationsPanelUI;
    public D2ReprisalsPanelUI reprisalsPanelUI;
    public D2ResistancePanelUI resistancePanelUI;
    public D2AlertPanelUI alertPanelUI;
    public D2ContainmentPanelUI containmentPanelUI;
    public TMP_Text membersText;
    public TMP_Text dominanceText;
    public Slider dominanceSlider;
    public TMP_Text region1Text;
    public TMP_Text region2Text;
    public TMP_Text region3Text;
    public TMP_Text region4Text;
    public TMP_Text assignmentText;
    public TMP_Text lastResultText;
    public Button assignOneButton;
    public Button assignTenButton;
    public Button assignAllButton;
    public Button releaseOneButton;
    public Button releaseAllButton;
    public Button backToMapButton;

    private readonly List<string> _selectableRegionIds = new List<string>();

    private void Awake()
    {
        if (showRegionsButton != null)
            showRegionsButton.onClick.AddListener(ShowRegions);
        if (showOperationsButton != null)
            showOperationsButton.onClick.AddListener(ShowOperations);
        if (showDefenseButton != null)
            showDefenseButton.onClick.AddListener(ShowDefense);
        if (showResistanceButton != null)
            showResistanceButton.onClick.AddListener(ShowResistance);
        if (showAlertButton != null)
            showAlertButton.onClick.AddListener(ShowAlert);
        if (showContainmentButton != null)
            showContainmentButton.onClick.AddListener(ShowContainment);
        if (regionDropdown != null)
            regionDropdown.onValueChanged.AddListener(SelectRegionFromDropdown);
        if (assignOneButton != null)
            assignOneButton.onClick.AddListener(() => Assign(1L));
        if (assignTenButton != null)
            assignTenButton.onClick.AddListener(() => Assign(10L));
        if (assignAllButton != null)
            assignAllButton.onClick.AddListener(AssignAll);
        if (releaseOneButton != null)
            releaseOneButton.onClick.AddListener(() => Release(1L));
        if (releaseAllButton != null)
            releaseAllButton.onClick.AddListener(ReleaseAll);
        if (backToMapButton != null)
            backToMapButton.onClick.AddListener(BackToMap);

        ShowRegions();
    }

    private void OnEnable()
    {
        _refreshTimer = 0f;
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
        if (gameState?.dimension2?.civilization2 == null)
            return;

        gameState.EnsureDimension2State();
        D2Civilization2State state = gameState.dimension2.civilization2;
        PopulateRegionDropdown(state);
        D2RegionState selectedRegion = D2Civilization2System.GetSelectedRegion(state);

        long assigned = D2Civilization2System.GetAssignedMembers(state);
        SetText(
            membersText,
            "MIEMBROS DE RESISTENCIA — Disponibles: " +
            state.membersAvailable.ToString("N0") + " | Asignados: " +
            assigned.ToString("N0") + " | Total: " +
            D2Civilization2System.GetTotalMembers(state).ToString("N0")
        );

        double totalDominance = D2Civilization2System.GetTotalDominance(state);
        SetText(dominanceText, "DOMINIO TOTAL: " + totalDominance.ToString("0.##") + "%");
        if (dominanceSlider != null)
        {
            dominanceSlider.minValue = 0f;
            dominanceSlider.maxValue = 100f;
            dominanceSlider.value = (float)totalDominance;
        }

        SetText(region1Text, BuildRegionText(state, D2Civilization2System.Region1Id, 0.0));
        SetText(
            region2Text,
            BuildRegionText(
                state,
                D2Civilization2System.Region2Id,
                D2Civilization2System.Region2UnlockDominance
            )
        );
        SetText(
            region3Text,
            BuildRegionText(
                state,
                D2Civilization2System.Region3Id,
                D2Civilization2System.Region3UnlockDominance
            )
        );
        SetText(region4Text, "REGIÓN 4 — ACTUALIZACIÓN FUTURA");
        SetText(
            assignmentText,
            "Asignación regional — " +
            D2Civilization2System.GetRegionDisplayName(selectedRegion.regionId) + ": " +
            selectedRegion.membersAssigned.ToString("N0") +
            " Miembro(s) | Sin destinar: " +
            D2Civilization2System.GetRegionIdleMembers(selectedRegion).ToString("N0")
        );
        SetText(
            lastResultText,
            string.IsNullOrEmpty(state.lastResult)
                ? "La Resistencia está preparada para organizar sus primeras operaciones."
                : state.lastResult
        );

        SetInteractable(assignOneButton, state.membersAvailable >= 1L);
        SetInteractable(assignTenButton, state.membersAvailable >= 1L);
        SetInteractable(assignAllButton, state.membersAvailable >= 1L);
        bool hasIdleRegionalMembers =
            D2Civilization2System.GetRegionIdleMembers(selectedRegion) >= 1L;
        SetInteractable(releaseOneButton, hasIdleRegionalMembers);
        SetInteractable(releaseAllButton, hasIdleRegionalMembers);

        if (operationsPanelUI != null)
            operationsPanelUI.Refresh();
        if (reprisalsPanelUI != null)
            reprisalsPanelUI.Refresh();
        if (resistancePanelUI != null)
            resistancePanelUI.Refresh();
        if (alertPanelUI != null)
            alertPanelUI.Refresh();
        if (showContainmentButton != null)
        {
            TMP_Text label = showContainmentButton.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                label.text = state.entityContained ? "PACTO" : "CONTENCIÓN";
        }
        if (containmentPanelUI != null)
            containmentPanelUI.Refresh();
    }

    public void ShowRegions()
    {
        SetSection(regionSectionRoot, true);
        SetSection(operationsSectionRoot, false);
        SetSection(defenseSectionRoot, false);
        SetSection(resistanceSectionRoot, false);
        SetSection(alertSectionRoot, false);
        SetSection(containmentSectionRoot, false);
        SetRegionDropdownVisible(true);
        Refresh();
    }

    public void ShowOperations()
    {
        SetSection(regionSectionRoot, false);
        SetSection(operationsSectionRoot, true);
        SetSection(defenseSectionRoot, false);
        SetSection(resistanceSectionRoot, false);
        SetSection(alertSectionRoot, false);
        SetSection(containmentSectionRoot, false);
        SetRegionDropdownVisible(true);
        Refresh();
    }

    public void ShowDefense()
    {
        SetSection(regionSectionRoot, false);
        SetSection(operationsSectionRoot, false);
        SetSection(defenseSectionRoot, true);
        SetSection(resistanceSectionRoot, false);
        SetSection(alertSectionRoot, false);
        SetSection(containmentSectionRoot, false);
        SetRegionDropdownVisible(true);
        Refresh();
    }

    public void ShowResistance()
    {
        SetSection(regionSectionRoot, false);
        SetSection(operationsSectionRoot, false);
        SetSection(defenseSectionRoot, false);
        SetSection(resistanceSectionRoot, true);
        SetSection(alertSectionRoot, false);
        SetSection(containmentSectionRoot, false);
        SetRegionDropdownVisible(false);
        Refresh();
    }

    public void ShowAlert()
    {
        SetSection(regionSectionRoot, false);
        SetSection(operationsSectionRoot, false);
        SetSection(defenseSectionRoot, false);
        SetSection(resistanceSectionRoot, false);
        SetSection(alertSectionRoot, true);
        SetSection(containmentSectionRoot, false);
        SetRegionDropdownVisible(false);
        Refresh();
    }

    public void ShowContainment()
    {
        SetSection(regionSectionRoot, false);
        SetSection(operationsSectionRoot, false);
        SetSection(defenseSectionRoot, false);
        SetSection(resistanceSectionRoot, false);
        SetSection(alertSectionRoot, false);
        SetSection(containmentSectionRoot, true);
        SetRegionDropdownVisible(false);
        Refresh();
    }

    private void SetRegionDropdownVisible(bool visible)
    {
        if (regionDropdown != null)
            regionDropdown.gameObject.SetActive(visible);
    }

    private void Assign(long amount)
    {
        D2Civilization2System.TryAssignMembers(
            GameState.I,
            GetSelectedRegionId(),
            amount
        );
        Refresh();
    }

    private void AssignAll()
    {
        D2Civilization2System.TryAssignAllMembers(
            GameState.I,
            GetSelectedRegionId()
        );
        Refresh();
    }

    private void Release(long amount)
    {
        D2Civilization2System.TryReleaseMembers(
            GameState.I,
            GetSelectedRegionId(),
            amount
        );
        Refresh();
    }

    private void ReleaseAll()
    {
        D2Civilization2System.TryReleaseAllMembers(
            GameState.I,
            GetSelectedRegionId()
        );
        Refresh();
    }

    private void BackToMap()
    {
        if (dimension2PanelUI != null)
            dimension2PanelUI.ShowMap();
    }

    public string GetSelectedRegionId()
    {
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        D2RegionState region = D2Civilization2System.GetSelectedRegion(state);
        return region != null ? region.regionId : D2Civilization2System.Region1Id;
    }

    private void PopulateRegionDropdown(D2Civilization2State state)
    {
        if (regionDropdown == null)
            return;

        _selectableRegionIds.Clear();
        foreach (string regionId in D2Civilization2System.RegionIds)
        {
            if (D2Civilization2System.IsSelectableRegion(state, regionId))
                _selectableRegionIds.Add(regionId);
        }

        bool rebuild = regionDropdown.options.Count != _selectableRegionIds.Count;
        if (!rebuild)
        {
            for (int i = 0; i < _selectableRegionIds.Count; i++)
            {
                if (regionDropdown.options[i].text !=
                    D2Civilization2System.GetRegionDisplayName(_selectableRegionIds[i]))
                {
                    rebuild = true;
                    break;
                }
            }
        }

        if (rebuild)
        {
            List<string> labels = new List<string>();
            foreach (string regionId in _selectableRegionIds)
                labels.Add(D2Civilization2System.GetRegionDisplayName(regionId));
            regionDropdown.ClearOptions();
            regionDropdown.AddOptions(labels);
        }

        int selectedIndex = Mathf.Max(0, _selectableRegionIds.IndexOf(state.selectedRegionId));
        regionDropdown.SetValueWithoutNotify(selectedIndex);
        regionDropdown.RefreshShownValue();
    }

    private void SelectRegionFromDropdown(int index)
    {
        if (index < 0 || index >= _selectableRegionIds.Count)
            return;

        D2Civilization2System.TrySelectRegion(GameState.I, _selectableRegionIds[index]);
        Refresh();
    }

    private static string BuildRegionText(
        D2Civilization2State state,
        string regionId,
        double unlockDominance
    )
    {
        D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
        string name = D2Civilization2System.GetRegionDisplayName(regionId).ToUpperInvariant();
        if (region == null || !region.unlocked)
        {
            return name + " — BLOQUEADA\nSe abre con Dominio total en " +
                unlockDominance.ToString("0") + "%";
        }

        return name + " — DISPONIBLE\nDominio: " + region.dominance.ToString("0.##") +
            "% | Amenaza: " + region.threat.ToString("0.##") + "% | Miembros: " +
            region.membersAssigned.ToString("N0");
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private static void SetInteractable(Button button, bool value)
    {
        if (button != null)
            button.interactable = value;
    }

    private static void SetSection(GameObject section, bool active)
    {
        if (section != null)
            section.SetActive(active);
    }
}
