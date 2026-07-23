using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2ContainmentPanelUI : MonoBehaviour
{
    public GameObject containmentAttemptRoot;
    public GameObject majorPactRoot;
    public TMP_Text stateText;
    public TMP_Text probabilityText;
    public TMP_Text cooldownText;
    public TMP_Text rulesText;
    public TMP_Text assignmentText;
    public TMP_Text attemptsText;
    public TMP_Text lastResultText;
    public Button attemptButton;
    public Button assignOneButton;
    public Button assignTenButton;
    public Button assignAllButton;
    public Button releaseOneButton;
    public Button releaseAllButton;
    public TMP_Text majorPactStateText;
    public TMP_Text stabilityText;
    public TMP_Dropdown majorPactLineDropdown;
    public TMP_Text majorPactLineText;
    public TMP_Text majorPactLastResultText;
    public Button establishMajorPactButton;
    public Button upgradeMajorPactLineButton;

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
        string status = state.entityContained
            ? "ENTE CONTENIDO — PACTO MAYOR PREPARADO"
            : state.containmentAvailable
                ? "CONTENCIÓN DISPONIBLE"
                : "CONTENCIÓN BLOQUEADA";
        SetText(stateText, status);
        SetText(
            probabilityText,
            "Dominio total: " + dominance.ToString("0.##") + "% | Probabilidad: " +
            (D2Civilization2System.GetContainmentSuccessProbability(state) * 100.0)
                .ToString("0.##") + "%"
        );
        SetText(
            cooldownText,
            state.containmentCooldownSeconds > 0.0
                ? "Reintento disponible en " + FormatDuration(state.containmentCooldownSeconds)
                : state.entityContained
                    ? "La Contención es permanente."
                    : "Sin cooldown."
        );
        SetText(
            rulesText,
            "Fallo: +20% Amenaza y -5% de Miembros regionales sin Protección. " +
            "Éxito: cesan las marcas y se prepara el pacto mayor."
        );
        SetText(
            assignmentText,
            "SOSTENIMIENTO — Asignados: " +
            state.membersAssignedToContainment.ToString("N0") +
            " | Disponibles: " + state.membersAvailable.ToString("N0")
        );
        SetText(
            attemptsText,
            "Intentos: " + state.totalContainmentAttempts.ToString("N0") +
            " | Fallos: " + state.totalContainmentFailures.ToString("N0")
        );
        SetText(
            lastResultText,
            string.IsNullOrEmpty(state.lastContainmentResult)
                ? "Todavía no se ha intentado la Contención."
                : state.lastContainmentResult
        );

        string lineId = GetSelectedMajorPactLineId();
        int level = D2Civilization2System.GetMajorPactLineLevel(state, lineId);
        int nextLevel = Mathf.Min(level + 1,
            D2Civilization2System.MaxMajorPactLineLevel);
        SetText(majorPactStateText, state.majorPactEstablished
            ? "PACTO MAYOR DE CIVILIZACION 2 — ESTABLECIDO"
            : "ENTE CONTENIDO — PACTO MAYOR PREPARADO");
        SetText(stabilityText,
            "ESTABILIDAD DE CONTENCIÓN: " + state.containmentStability.ToString("0.##") +
            " | Fragmentos de Control: " + state.controlFragments.ToString("N0"));
        SetText(majorPactLineText,
            D2Civilization2System.GetMajorPactLineName(lineId) + " — Nivel " +
            level + "/3\n" + D2Civilization2System.GetMajorPactLineDescription(lineId) +
            (level < D2Civilization2System.MaxMajorPactLineLevel
                ? "\nSiguiente: " +
                  D2Civilization2System.GetMajorPactStabilityCost(nextLevel).ToString("0") +
                  " Estabilidad + " +
                  D2Civilization2System.GetMajorPactFragmentCost(nextLevel).ToString("N0") +
                  " Fragmentos"
                : "\nNIVEL MÁXIMO"));
        SetText(majorPactLastResultText,
            string.IsNullOrEmpty(state.lastMajorPactResult)
                ? "Establece el pacto y asigna Miembros para generar Estabilidad."
                : state.lastMajorPactResult);

        SetInteractable(attemptButton, D2Civilization2System.CanAttemptContainment(state));
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
    }

    private void Attempt()
    {
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
}
