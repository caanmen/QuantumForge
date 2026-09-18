using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2ContainmentPanelUI : MonoBehaviour
{
    public GameObject containmentAttemptRoot;
    public GameObject majorPactRoot;
    public TMP_Text stateText;
    public TMP_Text availableMembersText;
    public TMP_Text dominanceHeaderText;
    public TMP_Text attemptsHeaderText;
    public TMP_Text failuresHeaderText;
    public TMP_Text probabilityText;
    public TMP_Text probabilityDetailText;
    public TMP_Text dominanceCardText;
    public TMP_Text cooldownText;
    public TMP_Text rulesText;
    public TMP_Text failureRuleText;
    public TMP_Text successRuleText;
    public TMP_Text assignmentText;
    public TMP_Text assignedMembersText;
    public TMP_Text assignmentAvailableText;
    public TMP_Text attemptsText;
    public TMP_Text lastResultText;
    public Button attemptButton;
    public Button assignOneButton;
    public Button assignTenButton;
    public Button assignAllButton;
    public Button releaseOneButton;
    public Button releaseAllButton;
    public TMP_Text majorPactStateText;
    public TMP_Text majorPactMilestoneText;
    public TMP_Text majorPactAvailableMembersText;
    public TMP_Text majorPactAssignedHeaderText;
    public TMP_Text stabilityText;
    public TMP_Text majorPactFragmentsHeaderText;
    public TMP_Dropdown majorPactLineDropdown;
    public TMP_Text majorPactLineText;
    public TMP_Text[] majorPactLineLevelTexts;
    public TMP_Text majorPactDetailEffectText;
    public TMP_Text majorPactDetailCostText;
    public TMP_Text majorPactAssignedValueText;
    public TMP_Text majorPactFooterTitleText;
    public TMP_Text majorPactLastResultText;
    public Button[] majorPactLineButtons;
    public GameObject[] majorPactLineSelections;
    public GameObject[] majorPactCentralIcons;
    public GameObject[] majorPactDetailIcons;
    public Button establishMajorPactButton;
    public Button upgradeMajorPactLineButton;
    private bool _lowChanceConfirmationArmed;

    private void Awake()
    {
        if (attemptButton != null)
            attemptButton.onClick.AddListener(Attempt);
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
        if (establishMajorPactButton != null)
            establishMajorPactButton.onClick.AddListener(EstablishMajorPact);
        if (upgradeMajorPactLineButton != null)
            upgradeMajorPactLineButton.onClick.AddListener(UpgradeMajorPactLine);
        if (majorPactLineDropdown != null)
        {
            majorPactLineDropdown.ClearOptions();
            var options = new List<string>();
            foreach (string lineId in D2Civilization2System.MajorPactLineIds)
                options.Add(D2Civilization2System.GetMajorPactLineName(lineId));
            majorPactLineDropdown.AddOptions(options);
            majorPactLineDropdown.onValueChanged.AddListener(_ => Refresh());
        }
        if (majorPactLineButtons != null)
        {
            for (int index = 0; index < majorPactLineButtons.Length; index++)
            {
                int capturedIndex = index;
                if (majorPactLineButtons[index] != null)
                    majorPactLineButtons[index].onClick.AddListener(
                        () => SelectMajorPactLine(capturedIndex));
            }
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.civilization2 == null)
            return;

        gameState.EnsureDimension2State();
        D2Civilization2State state = gameState.dimension2.civilization2;
        bool majorPactPhase = state.entityContained;
        if (containmentAttemptRoot != null)
            containmentAttemptRoot.SetActive(!majorPactPhase);
        if (majorPactRoot != null)
            majorPactRoot.SetActive(majorPactPhase);
        double dominance = D2Civilization2System.GetTotalDominance(state);
        double probability =
            D2Civilization2System.GetContainmentSuccessProbability(state);
        bool requiresConfirmation =
            D2Civilization2PresentationRules.RequiresContainmentConfirmation(state);
        if (!requiresConfirmation)
            _lowChanceConfirmationArmed = false;
        string status = state.entityContained
            ? "ENTE CONTENIDO\nPACTO MAYOR PREPARADO"
            : state.containmentAvailable
                ? "CONTENCIÓN\nDISPONIBLE"
                : "CONTENCIÓN\nBLOQUEADA";
        SetText(stateText, status);
        SetText(availableMembersText, state.membersAvailable.ToString("N0"));
        SetText(dominanceHeaderText, dominance.ToString("0.##") + "%");
        SetText(attemptsHeaderText, state.totalContainmentAttempts.ToString("N0"));
        SetText(failuresHeaderText, state.totalContainmentFailures.ToString("N0"));
        SetText(probabilityText, (probability * 100.0).ToString("0.##") + "%");
        SetText(probabilityDetailText, probability < 0.50
            ? "REDUCE DOMINIO O MEJORA PROTECCIÓN\nANTES DE CONFIRMAR."
            : "PROBABILIDAD FAVORABLE\nSEGÚN EL ESTADO ACTUAL.");
        SetText(dominanceCardText, dominance.ToString("0.##") + "%");
        SetText(
            cooldownText,
            state.containmentCooldownSeconds > 0.0
                ? "REINTENTO EN\n" + FormatDuration(state.containmentCooldownSeconds)
                : state.entityContained
                    ? "CONTENCIÓN\nPERMANENTE"
                    : "SIN COOLDOWN"
        );
        SetText(rulesText, "");
        SetText(failureRuleText,
            "<color=#E55339>FALLO</color> · +20% AMENAZA Y -5% DE MIEMBROS\n" +
            "REGIONALES SIN PROTECCIÓN.");
        SetText(successRuleText,
            "<color=#789A72>ÉXITO</color> · CESAN LAS MARCAS Y SE PREPARA\n" +
            "EL PACTO MAYOR.");
        SetText(assignmentText,
            "SOSTENIMIENTO — Asignados: " +
            state.membersAssignedToContainment.ToString("N0") +
            " | Disponibles: " + state.membersAvailable.ToString("N0"));
        SetText(assignedMembersText,
            "ASIGNADOS  " + state.membersAssignedToContainment.ToString("N0"));
        SetText(assignmentAvailableText,
            "DISPONIBLES  " + state.membersAvailable.ToString("N0"));
        SetText(attemptsText, state.totalContainmentAttempts.ToString("N0"));
        SetText(lastResultText, BuildLastResult(state));

        string lineId = GetSelectedMajorPactLineId();
        int level = D2Civilization2System.GetMajorPactLineLevel(state, lineId);
        int nextLevel = Mathf.Min(level + 1,
            D2Civilization2System.MaxMajorPactLineLevel);
        SetText(majorPactStateText, state.majorPactEstablished
            ? "Ente Contenido · Pacto Mayor Establecido"
            : "Ente Contenido · Pacto Mayor Disponible");
        SetText(majorPactMilestoneText, state.majorPactEstablished
            ? "Hito Reconocido para Cerrar Dimensión 2."
            : "Establece el Pacto Mayor para habilitar sus líneas.");
        SetText(majorPactAvailableMembersText, state.membersAvailable.ToString("N0"));
        SetText(majorPactAssignedHeaderText,
            state.membersAssignedToContainment.ToString("N0"));
        SetText(stabilityText, state.containmentStability.ToString("0"));
        SetText(majorPactFragmentsHeaderText, state.controlFragments.ToString("N0"));
        SetText(majorPactLineText,
            D2Civilization2System.GetMajorPactLineName(lineId) +
            " — Nivel " + level + " / 3");
        SetText(majorPactDetailEffectText,
            D2Civilization2System.GetMajorPactLineDescription(lineId));
        SetText(majorPactDetailCostText,
            level < D2Civilization2System.MaxMajorPactLineLevel
                ? "Siguiente · " +
                  D2Civilization2System.GetMajorPactStabilityCost(nextLevel).ToString("0") +
                  " Estabilidad + " +
                  D2Civilization2System.GetMajorPactFragmentCost(nextLevel).ToString("N0") +
                  " Fragmentos"
                : "Nivel Máximo");
        SetText(majorPactAssignedValueText,
            state.membersAssignedToContainment.ToString("N0"));
        SetText(majorPactFooterTitleText, state.majorPactEstablished
            ? "Pacto Mayor Establecido" : "Pacto Mayor Disponible");
        SetText(majorPactLastResultText, state.majorPactEstablished
            ? "LA CONTENCIÓN YA GENERA ESTABILIDAD."
            : "ESTABLECE EL PACTO Y ASIGNA MIEMBROS PARA GENERAR ESTABILIDAD.");
        RefreshMajorPactLineVisuals(state);

        SetInteractable(attemptButton, D2Civilization2System.CanAttemptContainment(state));
        SetButtonLabel(attemptButton, _lowChanceConfirmationArmed
            ? "CONFIRMAR INTENTO CON RIESGO"
            : requiresConfirmation ? "INTENTAR · REQUIERE CONFIRMACIÓN"
                : "INTENTAR CONTENCIÓN");
        bool canSustain = state.entityContained;
        SetInteractable(assignOneButton, canSustain && state.membersAvailable > 0L);
        SetInteractable(assignTenButton, canSustain && state.membersAvailable > 0L);
        SetInteractable(assignAllButton, canSustain && state.membersAvailable > 0L);
        SetInteractable(releaseOneButton, state.membersAssignedToContainment > 0L);
        SetInteractable(releaseAllButton, state.membersAssignedToContainment > 0L);
        if (establishMajorPactButton != null)
        {
            establishMajorPactButton.gameObject.SetActive(!state.majorPactEstablished);
            SetInteractable(establishMajorPactButton,
                D2Civilization2System.CanEstablishMajorPact(gameState));
        }
        SetInteractable(upgradeMajorPactLineButton,
            D2Civilization2System.CanUpgradeMajorPactLine(gameState, lineId));
        if (majorPactLineButtons != null)
            foreach (Button lineButton in majorPactLineButtons)
                SetInteractable(lineButton, state.majorPactEstablished);
        SetActive(majorPactLineDropdown, state.majorPactEstablished);
        SetActive(majorPactLineText, state.majorPactEstablished);
        SetActive(majorPactDetailEffectText, state.majorPactEstablished);
        SetActive(majorPactDetailCostText, state.majorPactEstablished);
        SetActive(upgradeMajorPactLineButton, state.majorPactEstablished);
        SetActive(stabilityText, true);
        SetActive(assignOneButton, !state.entityContained || state.majorPactEstablished);
        SetActive(assignTenButton, !state.entityContained || state.majorPactEstablished);
        SetActive(assignAllButton, !state.entityContained || state.majorPactEstablished);
        SetActive(releaseOneButton, !state.entityContained || state.majorPactEstablished);
        SetActive(releaseAllButton, !state.entityContained || state.majorPactEstablished);
        SetActive(assignmentText, !state.entityContained || state.majorPactEstablished);
    }

    private void Attempt()
    {
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        if (state == null) return;
        if (D2Civilization2PresentationRules.RequiresContainmentConfirmation(state) &&
            !_lowChanceConfirmationArmed)
        {
            _lowChanceConfirmationArmed = true;
            SetText(lastResultText,
                "CONFIRMACIÓN NECESARIA: el fallo aumenta Amenaza y puede causar pérdidas regionales. Pulsa de nuevo para continuar.");
            SetButtonLabel(attemptButton, "CONFIRMAR INTENTO CON RIESGO");
            return;
        }
        _lowChanceConfirmationArmed = false;
        D2Civilization2System.TryAttemptContainment(GameState.I);
        Refresh();
    }

    private void Assign(long amount)
    {
        D2Civilization2System.TryAssignMembersToContainment(GameState.I, amount);
        Refresh();
    }

    private void AssignAll()
    {
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        if (state != null)
            D2Civilization2System.TryAssignMembersToContainment(
                GameState.I,
                state.membersAvailable
            );
        Refresh();
    }

    private void Release(long amount)
    {
        D2Civilization2System.TryReleaseMembersFromContainment(GameState.I, amount);
        Refresh();
    }

    private void ReleaseAll()
    {
        D2Civilization2State state = GameState.I?.dimension2?.civilization2;
        if (state != null)
            D2Civilization2System.TryReleaseMembersFromContainment(
                GameState.I,
                state.membersAssignedToContainment
            );
        Refresh();
    }

    private string GetSelectedMajorPactLineId()
    {
        int index = majorPactLineDropdown != null ? majorPactLineDropdown.value : 0;
        return D2Civilization2System.MajorPactLineIds[
            Mathf.Clamp(index, 0, D2Civilization2System.MajorPactLineIds.Length - 1)];
    }

    private void SelectMajorPactLine(int index)
    {
        if (majorPactLineDropdown != null)
            majorPactLineDropdown.SetValueWithoutNotify(Mathf.Clamp(
                index, 0, D2Civilization2System.MajorPactLineIds.Length - 1));
        Refresh();
    }

    private void RefreshMajorPactLineVisuals(D2Civilization2State state)
    {
        int selectedIndex = majorPactLineDropdown != null
            ? Mathf.Clamp(majorPactLineDropdown.value, 0,
                D2Civilization2System.MajorPactLineIds.Length - 1)
            : 0;
        for (int index = 0; index < D2Civilization2System.MajorPactLineIds.Length; index++)
        {
            if (majorPactLineLevelTexts != null && index < majorPactLineLevelTexts.Length)
            {
                int lineLevel = D2Civilization2System.GetMajorPactLineLevel(
                    state, D2Civilization2System.MajorPactLineIds[index]);
                SetText(majorPactLineLevelTexts[index], "NIVEL " + lineLevel + "/3");
            }
            SetActiveAt(majorPactLineSelections, index,
                state.majorPactEstablished && index == selectedIndex);
            SetActiveAt(majorPactCentralIcons, index, index == selectedIndex);
            SetActiveAt(majorPactDetailIcons, index, index == selectedIndex);
        }
    }

    private void EstablishMajorPact()
    {
        D2Civilization2System.TryEstablishMajorPact(GameState.I);
        Refresh();
    }

    private void UpgradeMajorPactLine()
    {
        D2Civilization2System.TryUpgradeMajorPactLine(
            GameState.I, GetSelectedMajorPactLineId());
        Refresh();
    }

    private static string FormatDuration(double seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return (totalSeconds / 60).ToString("00") + ":" +
            (totalSeconds % 60).ToString("00");
    }

    private static string BuildLastResult(D2Civilization2State state)
    {
        if (state.entityContained)
            return "CONTENCIÓN EXITOSA · EL PACTO MAYOR ESTÁ PREPARADO";
        if (state.totalContainmentFailures > 0L ||
            (!string.IsNullOrEmpty(state.lastContainmentResult) &&
             state.lastContainmentResult.IndexOf(
                 "fall", System.StringComparison.OrdinalIgnoreCase) >= 0))
        {
            return "EL INTENTO ANTERIOR FALLÓ · " +
                (state.containmentCooldownSeconds > 0.0
                    ? "REINTENTO EN " + FormatDuration(state.containmentCooldownSeconds)
                    : "NUEVO INTENTO DISPONIBLE");
        }
        return "NINGÚN INTENTO REALIZADO";
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }

    private static void SetInteractable(Button target, bool value)
    {
        if (target != null)
            target.interactable = value;
    }

    private static void SetActive(Component component, bool active)
    {
        if (component != null) component.gameObject.SetActive(active);
    }

    private static void SetActiveAt(GameObject[] values, int index, bool active)
    {
        if (values != null && index >= 0 && index < values.Length && values[index] != null)
            values[index].SetActive(active);
    }

    private static void SetButtonLabel(Button button, string value)
    {
        if (button == null) return;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = value;
    }
}
