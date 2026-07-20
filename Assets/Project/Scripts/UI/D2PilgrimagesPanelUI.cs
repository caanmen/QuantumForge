using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2PilgrimagesPanelUI : MonoBehaviour
{
    public TMP_Text trustText;
    public Slider trustSlider;
    public TMP_Text resourcesText;
    public TMP_Text activePilgrimageText;
    public TMP_Text supportText;
    public Button addSupportButton;
    public Button removeSupportButton;
    public TMP_Text lastResultText;
    public Button startShortButton;
    public Button startMediumButton;
    public Button startLongButton;
    public Button startGuidedLongButton;
    public Button startSacredButton;
    public Button cancelButton;

    private float _refreshTimer;

    private void Awake()
    {
        if (startShortButton != null)
            startShortButton.onClick.AddListener(StartShort);
        if (startMediumButton != null)
            startMediumButton.onClick.AddListener(StartMedium);
        if (startLongButton != null)
            startLongButton.onClick.AddListener(StartLong);
        if (startGuidedLongButton != null)
            startGuidedLongButton.onClick.AddListener(StartGuidedLong);
        if (startSacredButton != null)
            startSacredButton.onClick.AddListener(StartSacred);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelActive);
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
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );

        if (trustText != null)
        {
            trustText.text =
                "CONFIANZA: " + state.trust.ToString("0") + "/500" +
                "   |   Civ 2: " +
                (gameState.dimension2.civilization2Unlocked ? "DESBLOQUEADA" : "a 300") +
                "   |   Umbral Velado: " +
                (state.entityContactAvailable ? "ALGO RESPONDE" : "a 500");
        }

        if (trustSlider != null)
        {
            trustSlider.minValue = 0f;
            trustSlider.maxValue = (float)D2PilgrimageSystem.MaxTrust;
            trustSlider.value = (float)state.trust;
        }

        if (resourcesText != null)
        {
            resourcesText.text =
                "Disponibles — Seguidores: " + state.followersAvailable.ToString("N0") +
                "   |   Cera: " + (wax?.offeringAmount ?? 0.0).ToString("N2") +
                "   |   Pan ritual: " + (bread?.offeringAmount ?? 0.0).ToString("N2");
        }

        D2PilgrimageState active = state.activePilgrimage;
        if (supportText != null)
        {
            long support = active.active
                ? active.supportFollowersCommitted
                : state.pilgrimageSupportFollowersSelected;
            supportText.text = "Apoyo adicional: " + support.ToString("N0") +
                "/4 Seguidores — Bonus material: +" +
                (D2PilgrimageSystem.GetSupportRewardBonus(support) * 100.0)
                    .ToString("0.#") + "%";
        }
        if (activePilgrimageText != null)
        {
            activePilgrimageText.text = active.active
                ? D2PilgrimageSystem.GetDisplayName(active.pilgrimageId) +
                  " en curso — Restante: " + FormatTime(active.remainingSeconds) +
                  " — Seguidores ocupados: " + active.followersCommitted.ToString("N0") +
                  (active.supportFollowersCommitted > 0L
                      ? " + " + active.supportFollowersCommitted.ToString("N0") + " de apoyo"
                      : "") +
                  (active.acolytesCommitted > 0L
                      ? " — Acólitos ocupados: " + active.acolytesCommitted.ToString("N0")
                      : "")
                : "No hay una Peregrinación activa.";
        }

        if (lastResultText != null)
        {
            lastResultText.text = string.IsNullOrEmpty(state.lastPilgrimageResult)
                ? "Las recompensas se entregan automáticamente al completar."
                : state.lastPilgrimageResult;
        }

        RefreshButtonLabel(state, startShortButton, D2PilgrimageSystem.ShortId);
        RefreshButtonLabel(state, startMediumButton, D2PilgrimageSystem.MediumId);
        RefreshButtonLabel(state, startLongButton, D2PilgrimageSystem.LongId);
        RefreshButtonLabel(state, startGuidedLongButton, D2PilgrimageSystem.GuidedLongId);
        RefreshButtonLabel(state, startSacredButton, D2PilgrimageSystem.SacredId);

        SetInteractable(
            startShortButton,
            D2PilgrimageSystem.CanStart(gameState, D2PilgrimageSystem.ShortId)
        );
        SetInteractable(
            startMediumButton,
            D2PilgrimageSystem.CanStart(gameState, D2PilgrimageSystem.MediumId)
        );
        SetInteractable(
            startLongButton,
            D2PilgrimageSystem.CanStart(gameState, D2PilgrimageSystem.LongId)
        );
        SetInteractable(
            startGuidedLongButton,
            D2PilgrimageSystem.CanStart(gameState, D2PilgrimageSystem.GuidedLongId)
        );
        SetInteractable(
            startSacredButton,
            D2PilgrimageSystem.CanStart(gameState, D2PilgrimageSystem.SacredId)
        );
        SetInteractable(cancelButton, active.active);
        SetInteractable(addSupportButton, !active.active &&
            state.pilgrimageSupportFollowersSelected < D2PilgrimageSystem.MaxSupportFollowers &&
            state.pilgrimageSupportFollowersSelected < state.followersAvailable);
        SetInteractable(removeSupportButton, !active.active &&
            state.pilgrimageSupportFollowersSelected > 0L);
    }

    public void StartShort() => TryStartPilgrimage(D2PilgrimageSystem.ShortId);
    public void StartMedium() => TryStartPilgrimage(D2PilgrimageSystem.MediumId);
    public void StartLong() => TryStartPilgrimage(D2PilgrimageSystem.LongId);
    public void StartGuidedLong() =>
        TryStartPilgrimage(D2PilgrimageSystem.GuidedLongId);
    public void StartSacred() => TryStartPilgrimage(D2PilgrimageSystem.SacredId);

    public void CancelActive()
    {
        D2PilgrimageSystem.TryCancel(GameState.I);
        RefreshAll();
    }

    private void ChangeSupport(long delta)
    {
        D2PilgrimageSystem.TryChangeSupportFollowers(GameState.I, delta);
        RefreshAll();
    }

    private void TryStartPilgrimage(string pilgrimageId)
    {
        D2PilgrimageSystem.TryStart(GameState.I, pilgrimageId);
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
        int totalSeconds = Math.Max(0, (int)Math.Ceiling(seconds));
        return (totalSeconds / 60).ToString("00") + ":" +
            (totalSeconds % 60).ToString("00");
    }

    private static void SetInteractable(Button button, bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }

    private static void RefreshButtonLabel(
        D2Civilization1State state,
        Button button,
        string pilgrimageId
    )
    {
        if (button == null)
            return;

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label == null)
            return;

        string name = D2PilgrimageSystem.GetDisplayName(pilgrimageId)
            .Replace("Peregrinación ", "").ToUpperInvariant();
        long followers = D2PilgrimageSystem.GetFollowersRequired(pilgrimageId);
        long acolytes = D2PilgrimageSystem.GetAcolytesRequired(pilgrimageId);
        label.text = name + " · " +
            FormatTime(D2PilgrimageSystem.GetDurationSeconds(pilgrimageId)) + "\n" +
            followers.ToString("N0") + " Seg" +
            (acolytes > 0L ? " · " + acolytes.ToString("N0") + " Acólito" : "") +
            " · " + D2PilgrimageSystem.GetEffectiveWaxCost(state, pilgrimageId)
                .ToString("0.##") + " Cera/Pan\n+" +
            D2PilgrimageSystem.GetEffectiveTrustReward(state, pilgrimageId)
                .ToString("0.##") + " Confianza";
    }
}
