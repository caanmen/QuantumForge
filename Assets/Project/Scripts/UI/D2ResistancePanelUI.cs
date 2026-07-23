using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2ResistancePanelUI : MonoBehaviour
{
    public TMP_Dropdown upgradeDropdown;
    public TMP_Text fragmentsText;
    public TMP_Text upgradeText;
    public Button upgradeButton;
    public TMP_Dropdown pactDropdown;
    public TMP_Text pactText;
    public TMP_Text pactMembersText;
    public TMP_Text exhaustedText;
    public TMP_Text penaltiesText;
    public Button activateButton;
    public Button reinforceOneButton;
    public Button reinforceTenButton;
    public Button cancelButton;

    private void Awake()
    {
        PopulateDropdowns();
        if (upgradeDropdown != null)
            upgradeDropdown.onValueChanged.AddListener(_ => Refresh());
        if (pactDropdown != null)
            pactDropdown.onValueChanged.AddListener(_ => Refresh());
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeSelected);
        if (activateButton != null)
            activateButton.onClick.AddListener(ActivateSelectedPact);
        if (reinforceOneButton != null)
            reinforceOneButton.onClick.AddListener(() => ReinforceSelectedPact(1L));
        if (reinforceTenButton != null)
            reinforceTenButton.onClick.AddListener(() => ReinforceSelectedPact(10L));
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelSelectedPact);
    }

    private void OnEnable()
    {
        PopulateDropdowns();
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (gameState?.dimension2?.civilization2 == null)
            return;

        D2Civilization2State state = gameState.dimension2.civilization2;
        string upgradeId = GetSelectedUpgradeId();
        int level = D2Civilization2System.GetUpgradeLevel(state, upgradeId);
        long cost = D2Civilization2System.GetUpgradeCost(level + 1);
        SetText(fragmentsText, "FRAGMENTOS DE CONTROL: " + state.controlFragments.ToString("N0"));
        SetText(
            upgradeText,
            D2Civilization2System.GetUpgradeName(upgradeId).ToUpperInvariant() +
            " — NIVEL " + level + "/" + D2Civilization2System.MaxUpgradeLevel + "\n" +
            D2Civilization2System.GetUpgradeEffectDescription(upgradeId) +
            (level < D2Civilization2System.MaxUpgradeLevel
                ? " Próximo coste: " + cost.ToString("N0") + " Fragmentos."
                : " Nivel máximo alcanzado.")
        );
        SetInteractable(
            upgradeButton,
            level < D2Civilization2System.MaxUpgradeLevel && state.controlFragments >= cost
        );

        string pactId = GetSelectedPactId();
        D2ResistancePactState pact = D2Civilization2System.GetResistancePact(state, pactId);
        bool active = pact != null && pact.active;
        SetText(
            pactText,
            D2Civilization2System.GetResistancePactName(pactId).ToUpperInvariant() +
            " — " + (active ? "ACTIVO" : "INACTIVO") + "\n" +
            D2Civilization2System.GetPactDescription(pactId)
        );
        SetText(
            pactMembersText,
            "Comprometidos: " + (pact != null ? pact.membersAssigned : 0L).ToString("N0") +
            " | Disponibles: " + state.membersAvailable.ToString("N0") +
            (active
                ? " | Próximo desgaste: " +
                    Mathf.CeilToInt((float)(D2Civilization2System.GetPactWearInterval(pactId) -
                    pact.wearProgressSeconds)) + " s"
                : " | Requisito: " +
                    D2Civilization2System.GetPactInitialRequirement(pactId).ToString("N0"))
        );
        SetText(
            exhaustedText,
            "MIEMBROS AGOTADOS: " +
            D2Civilization2System.GetExhaustedMembers(state).ToString("N0") +
            " — regresan 5 minutos después del desgaste."
        );
        SetText(
            penaltiesText,
            "PENALIZACIONES — Refugios: " + FormatSeconds(state.hiddenSheltersPenaltySeconds) +
            " | Campanas: " + FormatSeconds(state.silencedBellsPenaltySeconds) +
            " | Cuchillos: " + FormatSeconds(state.knivesPenaltySeconds)
        );

        long requirement = D2Civilization2System.GetPactInitialRequirement(pactId);
        SetInteractable(activateButton, !active && state.membersAvailable >= requirement);
        SetInteractable(reinforceOneButton, active && state.membersAvailable >= 1L);
        SetInteractable(reinforceTenButton, active && state.membersAvailable >= 1L);
        SetInteractable(cancelButton, active);
    }

    private void PopulateDropdowns()
    {
        if (upgradeDropdown != null &&
            upgradeDropdown.options.Count != D2Civilization2System.UpgradeIds.Length)
        {
            var labels = new List<string>();
            foreach (string id in D2Civilization2System.UpgradeIds)
                labels.Add(D2Civilization2System.GetUpgradeName(id));
            upgradeDropdown.ClearOptions();
            upgradeDropdown.AddOptions(labels);
        }

        if (pactDropdown != null &&
            pactDropdown.options.Count != D2Civilization2System.ResistancePactIds.Length)
        {
            var labels = new List<string>();
            foreach (string id in D2Civilization2System.ResistancePactIds)
                labels.Add(D2Civilization2System.GetResistancePactName(id));
            pactDropdown.ClearOptions();
            pactDropdown.AddOptions(labels);
        }
    }

    private void UpgradeSelected()
    {
        D2Civilization2System.TryUpgradeResistance(GameState.I, GetSelectedUpgradeId());
        Refresh();
    }

    private void ActivateSelectedPact()
    {
        D2Civilization2System.TryActivateResistancePact(GameState.I, GetSelectedPactId());
        Refresh();
    }

    private void ReinforceSelectedPact(long amount)
    {
        D2Civilization2System.TryReinforceResistancePact(
            GameState.I,
            GetSelectedPactId(),
            amount
        );
        Refresh();
    }

    private void CancelSelectedPact()
    {
        D2Civilization2System.TryCancelResistancePact(GameState.I, GetSelectedPactId());
        Refresh();
    }

    private string GetSelectedUpgradeId()
    {
        int index = upgradeDropdown != null ? upgradeDropdown.value : 0;
        index = Mathf.Clamp(index, 0, D2Civilization2System.UpgradeIds.Length - 1);
        return D2Civilization2System.UpgradeIds[index];
    }

    private string GetSelectedPactId()
    {
        int index = pactDropdown != null ? pactDropdown.value : 0;
        index = Mathf.Clamp(index, 0, D2Civilization2System.ResistancePactIds.Length - 1);
        return D2Civilization2System.ResistancePactIds[index];
    }

    private static string FormatSeconds(double seconds)
    {
        return seconds > 0.0 ? Mathf.CeilToInt((float)seconds) + " s" : "ninguna";
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
