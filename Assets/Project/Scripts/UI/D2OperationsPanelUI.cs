using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2OperationsPanelUI : MonoBehaviour
{
    private static readonly string[] CardOperationIds =
    {
        D2Civilization2System.RescueOperationId,
        D2Civilization2System.ProtectionOperationId,
        D2Civilization2System.EspionageOperationId,
        D2Civilization2System.SabotageOperationId
    };

    public D2Civilization2PanelUI civilization2PanelUI;
    public TMP_Dropdown operationDropdown;

    // Campos históricos conservados para mantener el contrato serializado.
    public TMP_Text operationStateText;
    public TMP_Text effectText;
    public TMP_Text regionalMembersText;
    public TMP_Text assignmentText;

    [Header("Operaciones de Resistencia — presentación V4")]
    public TMP_Text regionNameText;
    public TMP_Text regionalIdleValueText;
    public TMP_Text regionalOperationsValueText;
    public Button[] operationButtons;
    public GameObject[] operationSelectionOverlays;
    public Image[] operationStatusFillImages;
    public TMP_Text[] operationStateTexts;
    public TMP_Text[] operationAssignedValueTexts;
    public TMP_Text[] operationRequirementValueTexts;
    public TMP_Text detailNameText;
    public TMP_Text detailStateText;
    public Image detailStatusFillImage;
    public TMP_Text detailDescriptionText;
    public TMP_Text memberRateText;
    public TMP_Text dominanceRateText;
    public TMP_Text threatRateText;
    public TMP_Text assignmentNameText;
    public TMP_Text assignmentValueText;

    public Button assignOneButton;
    public Button assignFiveButton;
    public Button assignAllButton;
    public Button releaseOneButton;
    public Button releaseAllButton;

    private readonly SafeDropdownOptionMap<string> _operationOptions =
        new SafeDropdownOptionMap<string>(StringComparer.Ordinal);
    private string _selectedOperationId = D2Civilization2System.RescueOperationId;
    private bool _isPopulatingDropdown;

    private void Awake()
    {
        if (operationDropdown != null)
            operationDropdown.onValueChanged.AddListener(SelectFromDropdown);
        if (assignOneButton != null)
            assignOneButton.onClick.AddListener(() => Assign(1L));
        if (assignFiveButton != null)
            assignFiveButton.onClick.AddListener(() => Assign(5L));
        if (assignAllButton != null)
            assignAllButton.onClick.AddListener(AssignAll);
        if (releaseOneButton != null)
            releaseOneButton.onClick.AddListener(() => Release(1L));
        if (releaseAllButton != null)
            releaseAllButton.onClick.AddListener(ReleaseAll);
        if (operationButtons != null)
        {
            for (int i = 0; i < operationButtons.Length; i++)
            {
                int captured = i;
                if (operationButtons[i] != null)
                    operationButtons[i].onClick.AddListener(() => SelectOperationCard(captured));
            }
        }

        PopulateDropdown();
    }

    private void OnEnable()
    {
        PopulateDropdown();
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.civilization2 == null)
            return;

        gameState.EnsureDimension2State();
        D2Civilization2State state = gameState.dimension2.civilization2;
        PopulateDropdown();
        string regionId = GetSelectedRegionId();
        D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
        if (region == null)
            return;

        string operationId = GetSelectedOperationId();
        D2OperationState operation = D2Civilization2System.GetOperation(region, operationId);
        if (operation == null)
            return;

        string[] visibleOperationIds =
            D2Civilization2PresentationRules.GetVisibleOperationIds(state);
        SetText(regionNameText,
            D2Civilization2System.GetRegionDisplayName(regionId).ToUpperInvariant());
        SetText(regionalMembersText,
            D2Civilization2System.GetRegionDisplayName(regionId).ToUpperInvariant());
        SetText(regionalIdleValueText,
            D2Civilization2System.GetRegionIdleMembers(region).ToString("N0"));
        SetText(regionalOperationsValueText,
            D2Civilization2System.GetMembersAssignedToOperations(region).ToString("N0"));

        for (int i = 0; i < CardOperationIds.Length; i++)
        {
            string cardOperationId = CardOperationIds[i];
            D2OperationState cardOperation =
                D2Civilization2System.GetOperation(region, cardOperationId);
            bool visible = Contains(visibleOperationIds, cardOperationId);
            bool active = D2Civilization2System.IsOperationActive(cardOperation);
            SetArrayText(operationStateTexts, i, active ? "ACTIVA" : "INACTIVA");
            SetArrayText(operationAssignedValueTexts, i,
                cardOperation != null ? cardOperation.membersAssigned.ToString("N0") : "0");
            SetArrayText(operationRequirementValueTexts, i,
                D2Civilization2System.GetOperationRequirement(cardOperationId).ToString("N0"));
            SetArrayActive(operationSelectionOverlays, i,
                visible && cardOperationId == operationId);
            SetArrayInteractable(operationButtons, i, visible);
            SetStatusFill(i, active);
        }

        bool selectedActive = D2Civilization2System.IsOperationActive(operation);
        string displayName = D2Civilization2System.GetOperationDisplayName(operationId);
        SetText(detailNameText, displayName.ToUpperInvariant());
        SetText(detailStateText, selectedActive ? "ACTIVA" : "INACTIVA");
        if (detailStateText != null)
            detailStateText.color = selectedActive
                ? new Color32(142, 175, 112, 255)
                : new Color32(162, 142, 114, 255);
        if (detailStatusFillImage != null)
            detailStatusFillImage.color = selectedActive
                ? new Color32(53, 83, 40, 150)
                : new Color32(15, 17, 15, 75);
        SetText(operationStateText, selectedActive ? "ACTIVA" : "INACTIVA");
        string description = D2Civilization2System.GetOperationDescription(operationId)
            .ToUpperInvariant();
        SetText(detailDescriptionText, description);
        SetText(effectText, description);
        SetText(memberRateText, BuildMemberRate(operationId, operation, selectedActive));
        SetText(dominanceRateText, BuildDominanceRate(operationId, selectedActive));
        SetText(threatRateText, BuildThreatRate(operationId, selectedActive));
        string assignmentName = "ASIGNADOS A " + displayName.ToUpperInvariant();
        SetText(assignmentNameText, assignmentName);
        SetText(assignmentText, assignmentName);
        SetText(assignmentValueText, operation.membersAssigned.ToString("N0"));

        bool hasIdle = D2Civilization2System.GetRegionIdleMembers(region) > 0L;
        SetInteractable(assignOneButton, hasIdle);
        SetInteractable(assignFiveButton, hasIdle);
        SetInteractable(assignAllButton, hasIdle);
        SetInteractable(releaseOneButton, operation.membersAssigned > 0L);
        SetInteractable(releaseAllButton, operation.membersAssigned > 0L);
        bool learned = operation.membersAssigned > 0L || visibleOperationIds.Length > 1;
        SetActive(assignFiveButton, learned);
        SetActive(assignAllButton, learned);
        SetActive(releaseAllButton, learned);
    }

    private void PopulateDropdown()
    {
        if (operationDropdown == null || _isPopulatingDropdown)
            return;

        _isPopulatingDropdown = true;
        try
        {
            D2Civilization2State state = GameState.I != null && GameState.I.dimension2 != null
                ? GameState.I.dimension2.civilization2 : null;
            string preferred = string.IsNullOrEmpty(_selectedOperationId)
                ? _operationOptions.ResolveOrDefault(
                    operationDropdown.value, D2Civilization2System.RescueOperationId)
                : _selectedOperationId;
            int selectedIndex = _operationOptions.Rebuild(
                operationDropdown,
                D2Civilization2PresentationRules.GetVisibleOperationIds(state),
                D2Civilization2System.GetOperationDisplayName,
                preferred
            );
            _selectedOperationId = _operationOptions.ResolveOrDefault(
                selectedIndex, D2Civilization2System.RescueOperationId);
        }
        finally
        {
            _isPopulatingDropdown = false;
        }
    }

    private void SelectFromDropdown(int visibleIndex)
    {
        if (_isPopulatingDropdown)
            return;
        _selectedOperationId = _operationOptions.ResolveOrDefault(
            visibleIndex, D2Civilization2System.RescueOperationId);
        Refresh();
    }

    public void SelectOperationCard(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= CardOperationIds.Length)
            return;

        string operationId = CardOperationIds[cardIndex];
        int visibleIndex = _operationOptions.IndexOf(operationId);
        if (visibleIndex < 0)
            return;

        _selectedOperationId = operationId;
        if (operationDropdown != null)
            operationDropdown.SetValueWithoutNotify(visibleIndex);
        Refresh();
    }

    public string GetSelectedOperationId()
    {
        return string.IsNullOrEmpty(_selectedOperationId)
            ? D2Civilization2System.RescueOperationId
            : _selectedOperationId;
    }

    private static string BuildMemberRate(
        string operationId, D2OperationState operation, bool active)
    {
        if (!active || operation == null)
            return "—";

        double rate;
        switch (operationId)
        {
            case D2Civilization2System.RescueOperationId:
                rate = Math.Sqrt(operation.membersAssigned) *
                    D2Civilization2System.RescueMemberFactorPerMinute;
                break;
            case D2Civilization2System.ProtectionOperationId:
                rate = Math.Sqrt(operation.membersAssigned) *
                    D2Civilization2System.ProtectionMemberFactorPerMinute;
                break;
            default:
                return "—";
        }
        return "+" + rate.ToString("0.000", CultureInfo.InvariantCulture) + " / MIN";
    }

    private static string BuildDominanceRate(string operationId, bool active)
    {
        if (!active) return "—";
        double rate;
        switch (operationId)
        {
            case D2Civilization2System.RescueOperationId:
                rate = D2Civilization2System.RescueDominancePerMinute;
                break;
            case D2Civilization2System.ProtectionOperationId:
                rate = D2Civilization2System.ProtectionDominancePerMinute;
                break;
            case D2Civilization2System.EspionageOperationId:
                rate = D2Civilization2System.EspionageDominancePerMinute;
                break;
            default:
                rate = D2Civilization2System.SabotageDominancePerMinute;
                break;
        }
        return "−" + rate.ToString("0.00", CultureInfo.InvariantCulture) + " / MIN";
    }

    private static string BuildThreatRate(string operationId, bool active)
    {
        if (!active) return "—";
        double rate;
        switch (operationId)
        {
            case D2Civilization2System.RescueOperationId:
                rate = D2Civilization2System.RescueThreatPerMinute;
                break;
            case D2Civilization2System.ProtectionOperationId:
                rate = D2Civilization2System.ProtectionThreatPerMinute;
                break;
            case D2Civilization2System.EspionageOperationId:
                rate = D2Civilization2System.EspionageThreatPerMinute;
                break;
            default:
                rate = D2Civilization2System.SabotageThreatPerMinute;
                break;
        }
        string sign = rate < 0.0 ? "−" : "+";
        return sign + Math.Abs(rate).ToString("0.00", CultureInfo.InvariantCulture) + " / MIN";
    }

    private void Assign(long amount)
    {
        D2Civilization2System.TryAssignMembersToOperation(
            GameState.I, GetSelectedRegionId(), GetSelectedOperationId(), amount);
        RefreshAll();
    }

    private void AssignAll()
    {
        D2Civilization2System.TryAssignAllMembersToOperation(
            GameState.I, GetSelectedRegionId(), GetSelectedOperationId());
        RefreshAll();
    }

    private void Release(long amount)
    {
        D2Civilization2System.TryReleaseMembersFromOperation(
            GameState.I, GetSelectedRegionId(), GetSelectedOperationId(), amount);
        RefreshAll();
    }

    private void ReleaseAll()
    {
        D2Civilization2System.TryReleaseAllMembersFromOperation(
            GameState.I, GetSelectedRegionId(), GetSelectedOperationId());
        RefreshAll();
    }

    private void RefreshAll()
    {
        if (civilization2PanelUI != null)
            civilization2PanelUI.Refresh();
        else
            Refresh();
    }

    private string GetSelectedRegionId()
    {
        return civilization2PanelUI != null
            ? civilization2PanelUI.GetSelectedRegionId()
            : D2Civilization2System.Region1Id;
    }

    private void SetStatusFill(int index, bool active)
    {
        if (operationStatusFillImages == null || index < 0 ||
            index >= operationStatusFillImages.Length ||
            operationStatusFillImages[index] == null)
            return;
        operationStatusFillImages[index].color = active
            ? new Color32(53, 83, 40, 150)
            : new Color32(15, 17, 15, 75);
        if (operationStateTexts != null && index < operationStateTexts.Length &&
            operationStateTexts[index] != null)
            operationStateTexts[index].color = active
                ? new Color32(142, 175, 112, 255)
                : new Color32(162, 142, 114, 255);
        if (operationAssignedValueTexts != null && index < operationAssignedValueTexts.Length &&
            operationAssignedValueTexts[index] != null)
            operationAssignedValueTexts[index].color = active
                ? new Color32(142, 175, 112, 255)
                : new Color32(162, 142, 114, 255);
    }

    private static bool Contains(string[] values, string target)
    {
        if (values == null) return false;
        for (int i = 0; i < values.Length; i++)
            if (string.Equals(values[i], target, StringComparison.Ordinal)) return true;
        return false;
    }

    private static void SetArrayText(TMP_Text[] texts, int index, string value)
    {
        if (texts != null && index >= 0 && index < texts.Length)
            SetText(texts[index], value);
    }

    private static void SetArrayActive(GameObject[] objects, int index, bool active)
    {
        if (objects != null && index >= 0 && index < objects.Length && objects[index] != null)
            objects[index].SetActive(active);
    }

    private static void SetArrayInteractable(Button[] buttons, int index, bool interactable)
    {
        if (buttons != null && index >= 0 && index < buttons.Length && buttons[index] != null)
            buttons[index].interactable = interactable;
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text != null) text.text = value;
    }

    private static void SetInteractable(Button button, bool value)
    {
        if (button != null) button.interactable = value;
    }

    private static void SetActive(Component component, bool active)
    {
        if (component != null) component.gameObject.SetActive(active);
    }
}
