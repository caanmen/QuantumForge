using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class D2NovitiatePanelUI : MonoBehaviour
{
    public TMP_Text headerFollowersText, headerAcolytesText, headerPresentText, headerWaxText, headerBreadText;
    public TMP_Text levelText, batchAcolytesText, batchDurationText;
    public TMP_Text batchFollowerCostText, batchWaxCostText, batchBreadCostText;
    public TMP_Text supportText, activeTrainingText, startTrainingButtonText, startCostText;
    public TMP_Text upgradeButtonText, upgradeCostText;
    public Button addSupportButton, removeSupportButton, startTrainingButton, cancelTrainingButton, upgradeButton;
    public Button backButton, refugeNavButton, altarsNavButton, pilgrimagesNavButton;
    public Button ritesNavButton, pactsNavButton, thresholdNavButton;

    // Compatibilidad serializada con escenas anteriores.
    public TMP_Text acolytesText, resourcesText, batchText, lastResultText;

    private float _refreshTimer;
    private static readonly CultureInfo StableCulture = CultureInfo.InvariantCulture;

    private void Awake()
    {
        Add(startTrainingButton, StartTraining);
        Add(cancelTrainingButton, CancelTraining);
        Add(upgradeButton, Upgrade);
        Add(addSupportButton, () => ChangeSupport(1L));
        Add(removeSupportButton, () => ChangeSupport(-1L));
        Add(backButton, ShowMap);
        Add(refugeNavButton, () => Parent()?.ShowRefugeSection());
        Add(altarsNavButton, () => Parent()?.ShowAltarsSection());
        Add(pilgrimagesNavButton, () => Parent()?.ShowPilgrimagesSection());
        Add(ritesNavButton, () => Parent()?.ShowRitesSection());
        Add(pactsNavButton, () => Parent()?.ShowPactsSection());
        Add(thresholdNavButton, () => Parent()?.ShowVeiledThresholdSection());
    }

    private void OnEnable() { _refreshTimer = 0f; Refresh(); }

    private void Update()
    {
        _refreshTimer -= Time.unscaledDeltaTime;
        if (_refreshTimer > 0f) return;
        _refreshTimer = 0.2f;
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState)) return;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        int level = state.novitiateLevel;
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        double offeringCost = D2NovitiateSystem.GetOfferingCost(level);
        D2NovitiateTrainingState active = state.activeNovitiateTraining;
        long support = active.active ? active.supportFollowersCommitted : state.novitiateSupportFollowersSelected;

        SetText(headerFollowersText, FormatLong(state.followersAvailable));
        SetText(headerAcolytesText, FormatLong(state.acolytesAvailable));
        SetText(headerPresentText, FormatLong(D2NovitiateSystem.GetTotalAcolytes(state)));
        SetText(headerWaxText, FormatWhole(wax?.offeringAmount ?? 0.0));
        SetText(headerBreadText, FormatWhole(bread?.offeringAmount ?? 0.0));
        SetText(levelText, "NIVEL " + Roman(level));
        SetText(batchAcolytesText, D2NovitiateSystem.GetCurrentAcolytesPerBatch(state, level)
            .ToString("0.##", StableCulture) + "\nACÓLITOS");
        SetText(batchDurationText, FormatTime(D2NovitiateSystem.GetCurrentDurationSeconds(state, level)));
        SetText(batchFollowerCostText, FormatLong(D2NovitiateSystem.GetFollowerCost(level)) + " SEGUIDORES");
        SetText(batchWaxCostText, FormatWhole(offeringCost) + " CERA");
        SetText(batchBreadCostText, FormatWhole(offeringCost) + " PAN RITUAL");
        SetText(supportText, FormatLong(support) + " / 4\nSEGUIDORES");
        SetText(activeTrainingText, active.active
            ? "FORMACIÓN EN CURSO · " + FormatTime(D2NovitiateSystem.GetDisplayedRemainingSeconds(state))
            : "NO HAY UNA TANDA EN FORMACIÓN");
        SetText(startTrainingButtonText, active.active ? "FORMACIÓN EN CURSO" : "FORMAR TANDA");
        SetText(startCostText, FormatLong(D2NovitiateSystem.GetFollowerCost(level)) + " SEGUIDORES     " +
            FormatWhole(offeringCost) + " CERA     " + FormatWhole(offeringCost) + " PAN RITUAL");

        bool maxed = level >= D2NovitiateSystem.MaxLevel;
        double upgradeCost = D2NovitiateSystem.GetUpgradeOfferingCost(level);
        SetText(upgradeButtonText, maxed ? "NOVICIADO AL MÁXIMO" : "MEJORAR A NIVEL " + Roman(level + 1));
        SetText(upgradeCostText, maxed ? string.Empty :
            FormatLong(D2NovitiateSystem.GetUpgradeFollowerCost(level)) + " SEGUIDORES     " +
            FormatWhole(upgradeCost) + " CERA     " + FormatWhole(upgradeCost) + " PAN RITUAL");

        SetInteractable(startTrainingButton, D2NovitiateSystem.CanStartTraining(gameState));
        SetInteractable(cancelTrainingButton, active.active);
        SetInteractable(addSupportButton, !active.active &&
            state.novitiateSupportFollowersSelected < D2NovitiateSystem.MaxSupportFollowers &&
            state.novitiateSupportFollowersSelected < state.followersAvailable);
        SetInteractable(removeSupportButton, !active.active && state.novitiateSupportFollowersSelected > 0L);
        SetInteractable(upgradeButton, !maxed && D2NovitiateSystem.CanUpgrade(gameState));
    }

    public void StartTraining() { D2NovitiateSystem.TryStartTraining(GameState.I); RefreshAll(); }
    public void CancelTraining() { D2NovitiateSystem.TryCancelTraining(GameState.I); RefreshAll(); }
    public void Upgrade() { D2NovitiateSystem.TryUpgrade(GameState.I); RefreshAll(); }
    private void ChangeSupport(long delta) { D2NovitiateSystem.TryChangeSupportFollowers(GameState.I, delta); RefreshAll(); }

    private void ShowMap()
    {
        Dimension2PanelUI panel = GetComponentInParent<Dimension2PanelUI>(true);
        if (panel != null) panel.ShowMap();
    }

    private D2Civilization1PanelUI Parent() => GetComponentInParent<D2Civilization1PanelUI>(true);
    private void RefreshAll() { D2Civilization1PanelUI parent = Parent(); if (parent != null) parent.Refresh(); else Refresh(); }
    private static string FormatLong(long value) => value.ToString("N0", StableCulture);
    private static string FormatWhole(double value) => value.ToString("N0", StableCulture);
    private static string Roman(int value)
    {
        string[] values = { "", "I", "II", "III", "IV", "V" };
        return value >= 1 && value < values.Length ? values[value] : value.ToString(StableCulture);
    }
    private static string FormatTime(double seconds)
    {
        int total = Math.Max(0, (int)Math.Ceiling(seconds));
        return (total / 60).ToString("00", StableCulture) + ":" + (total % 60).ToString("00", StableCulture);
    }
    private static void Add(Button button, UnityEngine.Events.UnityAction action) { if (button != null) button.onClick.AddListener(action); }
    private static void SetText(TMP_Text target, string value) { if (target != null) target.text = value; }
    private static void SetInteractable(Button target, bool value) { if (target != null) target.interactable = value; }
}
