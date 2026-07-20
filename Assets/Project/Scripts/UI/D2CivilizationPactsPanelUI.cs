using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2CivilizationPactsPanelUI : MonoBehaviour
{
    public TMP_Dropdown pactDropdown;
    public TMP_Text slotsText;
    public TMP_Text pactStateText;
    public TMP_Text benefitText;
    public TMP_Text commitmentText;
    public TMP_Text resourcesText;
    public TMP_Text lastResultText;
    public Button activateButton;
    public TMP_Text activateButtonText;
    public Button cancelButton;
    public Button unlockSecondSlotButton;
    public TMP_Text unlockSecondSlotButtonText;

    private float _refreshTimer;

    private void Awake()
    {
        ConfigureDropdown();
        if (pactDropdown != null)
            pactDropdown.onValueChanged.AddListener(OnPactSelectionChanged);
        if (activateButton != null)
            activateButton.onClick.AddListener(ActivateSelected);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelSelected);
        if (unlockSecondSlotButton != null)
            unlockSecondSlotButton.onClick.AddListener(UnlockSecondSlot);
    }

    private void OnEnable()
    {
        ConfigureDropdown();
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

    public void ConfigureDropdown()
    {
        if (pactDropdown == null)
            return;

        int selected = Mathf.Clamp(
            pactDropdown.value,
            0,
            D2CivilizationPactSystem.PactIds.Length - 1
        );
        var options = new List<TMP_Dropdown.OptionData>();
        foreach (string pactId in D2CivilizationPactSystem.PactIds)
        {
            options.Add(new TMP_Dropdown.OptionData(
                D2CivilizationPactSystem.GetDisplayName(pactId)
            ));
        }

        pactDropdown.ClearOptions();
        pactDropdown.AddOptions(options);
        pactDropdown.SetValueWithoutNotify(selected);
        pactDropdown.RefreshShownValue();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState))
            return;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        string pactId = GetSelectedPactId();
        D2CivilizationPactState pact = D2CivilizationPactSystem.GetPact(state, pactId);
        if (pact == null)
            return;

        bool unlocked = D2CivilizationPactSystem.ArePactsUnlocked(state);
        int activeCount = D2CivilizationPactSystem.GetActivePactCount(state);
        int slotLimit = D2CivilizationPactSystem.GetActiveSlotLimit(state);
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );

        SetText(
            slotsText,
            unlocked
                ? "PACTOS ACTIVOS: " + activeCount + "/" + slotLimit +
                  (state.secondCivilizationPactSlotUnlocked
                      ? " — SEGUNDO ESPACIO DESBLOQUEADO"
                      : "")
                : "PACTOS BLOQUEADOS — Requiere 200 Confianza y Noviciado nivel 2."
        );
        SetText(
            pactStateText,
            D2CivilizationPactSystem.GetDisplayName(pactId) + " — " +
            (pact.active
                ? pact.suspended ? "SUSPENDIDO POR FALTA DE PAN" : "ACTIVO"
                : "INACTIVO")
        );
        SetText(
            benefitText,
            "BENEFICIO: " + D2CivilizationPactSystem.GetBenefitDescription(pactId)
        );
        SetText(
            commitmentText,
            "COMPROMISO: " + D2CivilizationPactSystem.GetCommitmentDescription(pactId)
        );
        SetText(
            resourcesText,
            "Confianza: " + state.trust.ToString("0.##") +
            "   |   Acólitos: " + state.acolytesAvailable.ToString("N0") +
            "   |   Cera: " + (wax?.offeringAmount ?? 0.0).ToString("N2") +
            "   |   Pan: " + (bread?.offeringAmount ?? 0.0).ToString("N2")
        );
        SetText(
            lastResultText,
            string.IsNullOrEmpty(state.lastCivilizationPactResult)
                ? "Cancelar no devuelve el coste de activación."
                : state.lastCivilizationPactResult
        );
        SetText(
            activateButtonText,
            pact.active
                ? pact.suspended ? "PACTO SUSPENDIDO" : "PACTO ACTIVO"
                : "ACTIVAR\n" +
                  D2CivilizationPactSystem.GetWaxActivationCost(pactId).ToString("0") +
                  " Cera · " +
                  D2CivilizationPactSystem.GetBreadActivationCost(pactId).ToString("0") +
                  " Pan"
        );
        SetInteractable(
            activateButton,
            D2CivilizationPactSystem.CanActivate(gameState, pactId)
        );
        SetInteractable(cancelButton, pact.active);
        SetText(
            unlockSecondSlotButtonText,
            state.secondCivilizationPactSlotUnlocked
                ? "SEGUNDO ESPACIO DESBLOQUEADO"
                : "DESBLOQUEAR SEGUNDO ESPACIO\n400 Confianza · Noviciado 4 · " +
                  "10 Acólitos · 300 Cera · 300 Pan"
        );
        SetInteractable(
            unlockSecondSlotButton,
            D2CivilizationPactSystem.CanUnlockSecondSlot(gameState)
        );
    }

    public void ActivateSelected()
    {
        D2CivilizationPactSystem.TryActivate(GameState.I, GetSelectedPactId());
        RefreshAll();
    }

    public void CancelSelected()
    {
        D2CivilizationPactSystem.TryCancel(GameState.I, GetSelectedPactId());
        RefreshAll();
    }

    public void UnlockSecondSlot()
    {
        D2CivilizationPactSystem.TryUnlockSecondSlot(GameState.I);
        RefreshAll();
    }

    private void OnPactSelectionChanged(int _)
    {
        Refresh();
    }

    private string GetSelectedPactId()
    {
        int index = pactDropdown != null ? pactDropdown.value : 0;
        index = Mathf.Clamp(index, 0, D2CivilizationPactSystem.PactIds.Length - 1);
        return D2CivilizationPactSystem.PactIds[index];
    }

    private void RefreshAll()
    {
        D2Civilization1PanelUI parent = GetComponentInParent<D2Civilization1PanelUI>(true);
        if (parent != null)
            parent.Refresh();
        else
            Refresh();
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target != null)
            target.text = value;
    }

    private static void SetInteractable(Button button, bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }
}
