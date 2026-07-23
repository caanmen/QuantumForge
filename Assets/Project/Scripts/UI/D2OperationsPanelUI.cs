using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2OperationsPanelUI : MonoBehaviour
{
    public D2Civilization2PanelUI civilization2PanelUI;
    public TMP_Dropdown operationDropdown;
    public TMP_Text operationStateText;
    public TMP_Text effectText;
    public TMP_Text regionalMembersText;
    public TMP_Text assignmentText;
    public Button assignOneButton;
    public Button assignFiveButton;
    public Button assignAllButton;
    public Button releaseOneButton;
    public Button releaseAllButton;

    private void Awake()
    {
        if (operationDropdown != null)
            operationDropdown.onValueChanged.AddListener(_ => Refresh());
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
        string regionId = GetSelectedRegionId();
        D2RegionState region = D2Civilization2System.GetRegion(state, regionId);
        string operationId = GetSelectedOperationId();
        D2OperationState operation = D2Civilization2System.GetOperation(region, operationId);
        if (region == null || operation == null)
            return;

        long requirement = D2Civilization2System.GetOperationRequirement(operationId);
        bool active = D2Civilization2System.IsOperationActive(operation);
        SetText(
            operationStateText,
            D2Civilization2System.GetOperationDisplayName(operationId).ToUpperInvariant() +
            " — " + (active ? "ACTIVA" : "INACTIVA") +
            "\nRequiere " + requirement.ToString("N0") + " Miembros para funcionar."
        );
        SetText(effectText, BuildEffectText(operationId, operation, active));
        SetText(
            regionalMembersText,
            D2Civilization2System.GetRegionDisplayName(regionId) + " — Sin destinar: " +
            D2Civilization2System.GetRegionIdleMembers(region).ToString("N0") +
            " | En operaciones: " +
            D2Civilization2System.GetMembersAssignedToOperations(region).ToString("N0")
        );
        SetText(
            assignmentText,
            "Asignados a " + D2Civilization2System.GetOperationDisplayName(operationId) +
            ": " + operation.membersAssigned.ToString("N0")
        );

        bool hasIdle = D2Civilization2System.GetRegionIdleMembers(region) > 0L;
        SetInteractable(assignOneButton, hasIdle);
        SetInteractable(assignFiveButton, hasIdle);
        SetInteractable(assignAllButton, hasIdle);
        SetInteractable(releaseOneButton, operation.membersAssigned > 0L);
        SetInteractable(releaseAllButton, operation.membersAssigned > 0L);
    }

    private void PopulateDropdown()
    {
        if (operationDropdown == null ||
            operationDropdown.options.Count == D2Civilization2System.OperationIds.Length)
        {
            return;
        }

        int selected = operationDropdown.value;
        List<string> options = new List<string>();
        foreach (string operationId in D2Civilization2System.OperationIds)
            options.Add(D2Civilization2System.GetOperationDisplayName(operationId));
        operationDropdown.ClearOptions();
        operationDropdown.AddOptions(options);
        operationDropdown.value = Mathf.Clamp(selected, 0, options.Count - 1);
        operationDropdown.RefreshShownValue();
    }

    private string GetSelectedOperationId()
    {
        int index = operationDropdown != null ? operationDropdown.value : 0;
        return D2Civilization2System.OperationIds[
            Mathf.Clamp(index, 0, D2Civilization2System.OperationIds.Length - 1)
        ];
    }

    private static string BuildEffectText(
        string operationId,
        D2OperationState operation,
        bool active
    )
    {
        string description = D2Civilization2System.GetOperationDescription(operationId);
        if (!active)
            return description + "\nSin efectos hasta alcanzar el requisito.";

        switch (operationId)
        {
            case D2Civilization2System.RescueOperationId:
                return description + "\nMiembros: +" +
                    (System.Math.Sqrt(operation.membersAssigned) *
                     D2Civilization2System.RescueMemberFactorPerMinute).ToString("0.###") +
                    "/min | Dominio: -0.05/min | Amenaza: +0.20/min";
            case D2Civilization2System.ProtectionOperationId:
                return description + "\nMiembros: +" +
                    (System.Math.Sqrt(operation.membersAssigned) *
                     D2Civilization2System.ProtectionMemberFactorPerMinute).ToString("0.###") +
                    "/min | Cobertura: +" +
                    (System.Math.Sqrt(operation.membersAssigned) *
                     D2Civilization2System.ProtectionCoverageFactorPerMinute).ToString("0.###") +
                    "/min\nDominio: -0.02/min | Amenaza: -0.35/min";
            case D2Civilization2System.EspionageOperationId:
                return description +
                    "\nPrepara -5% para la próxima Represalia." +
                    "\nDominio: -0.12/min | Amenaza: +0.25/min";
            case D2Civilization2System.SabotageOperationId:
                return description + "\nDominio: -0.30/min | Amenaza: +0.60/min";
            default:
                return description;
        }
    }

    private void Assign(long amount)
    {
        D2Civilization2System.TryAssignMembersToOperation(
            GameState.I,
            GetSelectedRegionId(),
            GetSelectedOperationId(),
            amount
        );
        RefreshAll();
    }

    private void AssignAll()
    {
        D2Civilization2System.TryAssignAllMembersToOperation(
            GameState.I,
            GetSelectedRegionId(),
            GetSelectedOperationId()
        );
        RefreshAll();
    }

    private void Release(long amount)
    {
        D2Civilization2System.TryReleaseMembersFromOperation(
            GameState.I,
            GetSelectedRegionId(),
            GetSelectedOperationId(),
            amount
        );
        RefreshAll();
    }

    private void ReleaseAll()
    {
        D2Civilization2System.TryReleaseAllMembersFromOperation(
            GameState.I,
            GetSelectedRegionId(),
            GetSelectedOperationId()
        );
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
}
