using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2NovitiatePanelUI : MonoBehaviour
{
    public TMP_Text acolytesText;
    public TMP_Text resourcesText;
    public TMP_Text batchText;
    public TMP_Text activeTrainingText;
    public TMP_Text supportText;
    public Button addSupportButton;
    public Button removeSupportButton;
    public TMP_Text lastResultText;
    public Button startTrainingButton;
    public TMP_Text startTrainingButtonText;
    public Button cancelTrainingButton;
    public Button upgradeButton;
    public TMP_Text upgradeButtonText;

    private float _refreshTimer;

    private void Awake()
    {
        if (startTrainingButton != null)
            startTrainingButton.onClick.AddListener(StartTraining);
        if (cancelTrainingButton != null)
            cancelTrainingButton.onClick.AddListener(CancelTraining);
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(Upgrade);
        if (addSupportButton != null)
            addSupportButton.onClick.AddListener(() => ChangeSupport(1L));
        if (removeSupportButton != null)
            removeSupportButton.onClick.AddListener(() => ChangeSupport(-1L));
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
        if (!Dimension2System.CanAccessDimension2(gameState))
            return;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        int level = state.novitiateLevel;
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        long totalAcolytes = D2NovitiateSystem.GetTotalAcolytes(state);

        SetText(
            acolytesText,
            "NOVICIADO — Nivel " + level + "/5" +
            "   |   Acólitos disponibles: " + state.acolytesAvailable.ToString("N0") +
            "   |   Totales presentes: " + totalAcolytes.ToString("N0")
        );
        SetText(
            resourcesText,
            "Seguidores disponibles: " + state.followersAvailable.ToString("N0") +
            "   |   Cera: " + (wax?.offeringAmount ?? 0.0).ToString("N2") +
            "   |   Pan ritual: " + (bread?.offeringAmount ?? 0.0).ToString("N2")
        );
        SetText(
            batchText,
            "Tanda actual: " + D2NovitiateSystem.GetCurrentAcolytesPerBatch(state, level)
                .ToString("0.##") +
            " Acólitos en " + FormatTime(
                D2NovitiateSystem.GetCurrentDurationSeconds(state, level)
            ) +
            " — Convierte " + D2NovitiateSystem.GetFollowerCost(level).ToString("N0") +
            " Seguidores" +
            (D2CivilizationPactSystem.IsPactActive(
                state,
                D2CivilizationPactSystem.InnerDoorId
            ) ? " — BLOQUEADO POR PUERTA INTERIOR" : "")
        );

        D2NovitiateTrainingState active = state.activeNovitiateTraining;
        long support = active.active
            ? active.supportFollowersCommitted
            : state.novitiateSupportFollowersSelected;
        SetText(
            supportText,
            "Apoyo adicional: " + support.ToString("N0") +
            "/4 Seguidores — Reducción propia: " +
            (D2NovitiateSystem.GetSupportDurationReduction(support) * 100.0)
                .ToString("0.#") + "%"
        );
        SetText(
            activeTrainingText,
            active.active
                ? "Formación en curso — Restante: " + FormatTime(
                    D2NovitiateSystem.GetDisplayedRemainingSeconds(state)
                  ) +
                  " — Resultado: " + active.acolytesToCreate.ToString("N0") + " Acólitos" +
                  (active.supportFollowersCommitted > 0L
                      ? " — Apoyo: " + active.supportFollowersCommitted.ToString("N0")
                      : "")
                : "No hay una tanda en formación."
        );
        SetText(
            lastResultText,
            string.IsNullOrEmpty(state.lastNovitiateResult)
                ? "Los Seguidores se convierten en Acólitos al completar la tanda."
                : state.lastNovitiateResult
        );

        double offeringCost = D2NovitiateSystem.GetOfferingCost(level);
        SetText(
            startTrainingButtonText,
            "FORMAR TANDA\nCoste: " +
            D2NovitiateSystem.GetFollowerCost(level).ToString("N0") +
            " Seguidores · " + offeringCost.ToString("0") +
            " Cera · " + offeringCost.ToString("0") + " Pan"
        );
        SetInteractable(
            startTrainingButton,
            D2NovitiateSystem.CanStartTraining(gameState)
        );
        SetInteractable(cancelTrainingButton, active.active);
        SetInteractable(addSupportButton, !active.active &&
            state.novitiateSupportFollowersSelected < D2NovitiateSystem.MaxSupportFollowers &&
            state.novitiateSupportFollowersSelected < state.followersAvailable);
        SetInteractable(removeSupportButton, !active.active &&
            state.novitiateSupportFollowersSelected > 0L);

        bool maxed = level >= D2NovitiateSystem.MaxLevel;
        double upgradeOfferingCost = D2NovitiateSystem.GetUpgradeOfferingCost(level);
        SetText(
            upgradeButtonText,
            maxed
                ? "NOVICIADO AL MÁXIMO"
                : "MEJORAR A NIVEL " + (level + 1) + "\nCoste: " +
                  D2NovitiateSystem.GetUpgradeFollowerCost(level).ToString("N0") +
                  " Seguidores · " + upgradeOfferingCost.ToString("0") +
                  " Cera · " + upgradeOfferingCost.ToString("0") + " Pan"
        );
        SetInteractable(upgradeButton, !maxed && D2NovitiateSystem.CanUpgrade(gameState));
    }

    public void StartTraining()
    {
        D2NovitiateSystem.TryStartTraining(GameState.I);
        RefreshAll();
    }

    public void CancelTraining()
    {
        D2NovitiateSystem.TryCancelTraining(GameState.I);
        RefreshAll();
    }

    public void Upgrade()
    {
        D2NovitiateSystem.TryUpgrade(GameState.I);
        RefreshAll();
    }

    private void ChangeSupport(long delta)
    {
        D2NovitiateSystem.TryChangeSupportFollowers(GameState.I, delta);
        RefreshAll();
    }

    private void RefreshAll()
    {
        D2Civilization1PanelUI parent = GetComponentInParent<D2Civilization1PanelUI>(true);
        if (parent != null)
            parent.Refresh();
        else
            Refresh();
    }

    private static string FormatTime(double seconds)
    {
        int total = Math.Max(0, (int)Math.Ceiling(seconds));
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
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
