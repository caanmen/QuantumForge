using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class D2PilgrimagesPanelUI : MonoBehaviour
{
    public TMP_Text headerTrustText;
    public TMP_Text headerFollowersText;
    public TMP_Text headerWaxText;
    public TMP_Text headerBreadText;
    public TMP_Text shortBodyText;
    public TMP_Text shortRewardText;
    public TMP_Text mediumBodyText;
    public TMP_Text mediumRewardText;
    public TMP_Text longBodyText;
    public TMP_Text longRewardText;
    public TMP_Text guidedLongBodyText;
    public TMP_Text guidedLongRewardText;
    public TMP_Text sacredBodyText;
    public TMP_Text sacredRewardText;
    public TMP_Text selectedTitleText;
    public TMP_Text activePilgrimageText;
    public TMP_Text supportText;
    public TMP_Text materialBonusText;
    public TMP_Text lastResultText;
    public TMP_Text primaryActionButtonText;
    public GameObject shortSelectionOverlay;

    // Los cinco botones son selectores de tarjeta; iniciar pertenece únicamente al CTA.
    public Button startShortButton;
    public Button startMediumButton;
    public Button startLongButton;
    public Button startGuidedLongButton;
    public Button startSacredButton;
    public Button primaryActionButton;
    public Button addSupportButton;
    public Button removeSupportButton;
    public Button cancelButton;
    public Button backButton;
    public Button refugeNavButton;
    public Button altarsNavButton;
    public Button novitiateNavButton;
    public Button ritesNavButton;
    public Button pactsNavButton;
    public Button thresholdNavButton;

    // Compatibilidad con escenas anteriores; el constructor canónico ya no los muestra.
    public TMP_Text trustText;
    public Slider trustSlider;
    public TMP_Text resourcesText;

    private string _selectedPilgrimageId = D2PilgrimageSystem.ShortId;
    private float _refreshTimer;
    private bool _showOfflineExplanation;
    private const string OfflineHelpId = "d2.c1.pilgrimages.offline_help";

    public string SelectedPilgrimageId => _selectedPilgrimageId;

    private void Awake()
    {
        Add(startShortButton, () => SelectPilgrimage(D2PilgrimageSystem.ShortId));
        Add(startMediumButton, () => SelectPilgrimage(D2PilgrimageSystem.MediumId));
        Add(startLongButton, () => SelectPilgrimage(D2PilgrimageSystem.LongId));
        Add(startGuidedLongButton, () => SelectPilgrimage(D2PilgrimageSystem.GuidedLongId));
        Add(startSacredButton, () => SelectPilgrimage(D2PilgrimageSystem.SacredId));
        Add(primaryActionButton, PerformPrimaryAction);
        Add(cancelButton, CancelActive);
        Add(addSupportButton, () => ChangeSupport(1L));
        Add(removeSupportButton, () => ChangeSupport(-1L));
        Add(backButton, ShowMap);
        Add(refugeNavButton, () => Parent()?.ShowRefugeSection());
        Add(altarsNavButton, () => Parent()?.ShowAltarsSection());
        Add(novitiateNavButton, () => Parent()?.ShowNovitiateSection());
        Add(ritesNavButton, () => Parent()?.ShowRitesSection());
        Add(pactsNavButton, () => Parent()?.ShowPactsSection());
        Add(thresholdNavButton, () => Parent()?.ShowVeiledThresholdSection());
    }

    private void OnEnable()
    {
        _refreshTimer = 0f;
        D2Civilization1State state = GameState.I?.dimension2?.civilization1;
        if (state?.activePilgrimage != null && state.activePilgrimage.active &&
            D2PilgrimageSystem.IsPilgrimageId(state.activePilgrimage.pilgrimageId))
            _selectedPilgrimageId = state.activePilgrimage.pilgrimageId;
        _showOfflineExplanation = state != null && state.shortPilgrimagesCompleted > 0L &&
            !PresentationStateUtility.Contains(
                GameState.I.dimension2.presentation.acknowledgedFeatureIds, OfflineHelpId);
        Refresh();
    }

    private void OnDisable()
    {
        if (_showOfflineExplanation && GameState.I?.dimension2 != null)
            PresentationStateUtility.Acknowledge(GameState.I.dimension2.presentation, OfflineHelpId);
        _showOfflineExplanation = false;
    }

    private void Update()
    {
        _refreshTimer -= Time.unscaledDeltaTime;
        if (_refreshTimer > 0f) return;
        _refreshTimer = 0.2f;
        Refresh();
    }

    public void SelectPilgrimage(string pilgrimageId)
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState) ||
            !D2PilgrimageSystem.IsPilgrimageId(pilgrimageId)) return;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.activePilgrimage.active ||
            !D2PilgrimageSystem.IsUnlocked(state, pilgrimageId)) return;
        _selectedPilgrimageId = pilgrimageId;
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState)) return;
        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId);
        D2PilgrimageState active = state.activePilgrimage;

        if (!D2PilgrimageSystem.IsPilgrimageId(_selectedPilgrimageId))
            _selectedPilgrimageId = D2PilgrimageSystem.ShortId;
        if (active.active) _selectedPilgrimageId = active.pilgrimageId;

        Set(headerTrustText, Number(state.trust) + " / 500");
        Set(headerFollowersText, state.followersAvailable.ToString("N0", CultureInfo.InvariantCulture));
        Set(headerWaxText, (wax?.offeringAmount ?? 0d).ToString("N0", CultureInfo.InvariantCulture));
        Set(headerBreadText, (bread?.offeringAmount ?? 0d).ToString("N0", CultureInfo.InvariantCulture));
        Set(trustText, "CONFIANZA: " + Number(state.trust) + "/500");
        if (trustSlider != null)
        {
            trustSlider.minValue = 0f;
            trustSlider.maxValue = (float)D2PilgrimageSystem.MaxTrust;
            trustSlider.value = (float)state.trust;
        }
        Set(resourcesText, "SEGUIDORES " + state.followersAvailable.ToString("N0", CultureInfo.InvariantCulture) +
            " · CERA " + (wax?.offeringAmount ?? 0d).ToString("N0", CultureInfo.InvariantCulture) +
            " · PAN RITUAL " + (bread?.offeringAmount ?? 0d).ToString("N0", CultureInfo.InvariantCulture));

        RefreshCard(state, D2PilgrimageSystem.ShortId, shortBodyText, shortRewardText);
        RefreshCard(state, D2PilgrimageSystem.MediumId, mediumBodyText, mediumRewardText);
        RefreshCard(state, D2PilgrimageSystem.LongId, longBodyText, longRewardText);
        RefreshCard(state, D2PilgrimageSystem.GuidedLongId, guidedLongBodyText, guidedLongRewardText);
        RefreshCard(state, D2PilgrimageSystem.SacredId, sacredBodyText, sacredRewardText);

        long support = active.active
            ? active.supportFollowersCommitted
            : state.pilgrimageSupportFollowersSelected;
        Set(supportText, support.ToString(CultureInfo.InvariantCulture) + " / 4\nSEGUIDORES");
        Set(materialBonusText, "+" +
            (D2PilgrimageSystem.GetSupportRewardBonus(support) * 100d)
                .ToString("0.#", CultureInfo.InvariantCulture) + "%");
        Set(selectedTitleText, D2PilgrimageSystem.GetDisplayName(_selectedPilgrimageId).ToUpperInvariant());

        if (active.active)
        {
            Set(activePilgrimageText,
                D2PilgrimageSystem.GetDisplayName(active.pilgrimageId).ToUpperInvariant() +
                "\nRESTANTE " + FormatTime(active.remainingSeconds));
            Set(primaryActionButtonText, "CANCELAR\nPEREGRINACIÓN");
        }
        else
        {
            Set(activePilgrimageText, "NO HAY UNA\nPEREGRINACIÓN ACTIVA");
            Set(primaryActionButtonText, "INICIAR\n" +
                D2PilgrimageSystem.GetDisplayName(_selectedPilgrimageId).ToUpperInvariant());
        }

        string result = state.lastPilgrimageResult ?? string.Empty;
        if (_showOfflineExplanation)
            result += (result.Length > 0 ? "\n" : string.Empty) +
                "Las Peregrinaciones continúan durante una ausencia.";
        Set(lastResultText, result);

        bool selectionEnabled = !active.active;
        SetInteractable(startShortButton, selectionEnabled &&
            D2PilgrimageSystem.IsUnlocked(state, D2PilgrimageSystem.ShortId));
        SetInteractable(startMediumButton, selectionEnabled &&
            D2PilgrimageSystem.IsUnlocked(state, D2PilgrimageSystem.MediumId));
        SetInteractable(startLongButton, selectionEnabled &&
            D2PilgrimageSystem.IsUnlocked(state, D2PilgrimageSystem.LongId));
        SetInteractable(startGuidedLongButton, selectionEnabled &&
            D2PilgrimageSystem.IsUnlocked(state, D2PilgrimageSystem.GuidedLongId));
        SetInteractable(startSacredButton, selectionEnabled &&
            D2PilgrimageSystem.IsUnlocked(state, D2PilgrimageSystem.SacredId));
        SetInteractable(primaryActionButton, active.active ||
            D2PilgrimageSystem.CanStart(gameState, _selectedPilgrimageId));
        SetInteractable(addSupportButton, !active.active &&
            state.pilgrimageSupportFollowersSelected < D2PilgrimageSystem.MaxSupportFollowers &&
            state.pilgrimageSupportFollowersSelected < state.followersAvailable);
        SetInteractable(removeSupportButton, !active.active &&
            state.pilgrimageSupportFollowersSelected > 0L);
        SetInteractable(cancelButton, active.active);
        if (cancelButton != null) cancelButton.gameObject.SetActive(false);
        if (shortSelectionOverlay != null)
            shortSelectionOverlay.SetActive(!active.active &&
                _selectedPilgrimageId == D2PilgrimageSystem.ShortId);

        // Las cinco tarjetas conservan siempre su geometría, incluso bloqueadas.
        SetActive(startShortButton, true);
        SetActive(startMediumButton, true);
        SetActive(startLongButton, true);
        SetActive(startGuidedLongButton, true);
        SetActive(startSacredButton, true);
    }

    public void StartShort() { _selectedPilgrimageId = D2PilgrimageSystem.ShortId; TryStartSelected(); }
    public void StartMedium() { _selectedPilgrimageId = D2PilgrimageSystem.MediumId; TryStartSelected(); }
    public void StartLong() { _selectedPilgrimageId = D2PilgrimageSystem.LongId; TryStartSelected(); }
    public void StartGuidedLong() { _selectedPilgrimageId = D2PilgrimageSystem.GuidedLongId; TryStartSelected(); }
    public void StartSacred() { _selectedPilgrimageId = D2PilgrimageSystem.SacredId; TryStartSelected(); }

    public void CancelActive()
    {
        D2PilgrimageSystem.TryCancel(GameState.I);
        RefreshAll();
    }

    private void PerformPrimaryAction()
    {
        D2PilgrimageState active = GameState.I?.dimension2?.civilization1?.activePilgrimage;
        if (active != null && active.active) CancelActive();
        else TryStartSelected();
    }

    private void TryStartSelected()
    {
        D2PilgrimageSystem.TryStart(GameState.I, _selectedPilgrimageId);
        RefreshAll();
    }

    private void ChangeSupport(long delta)
    {
        D2PilgrimageSystem.TryChangeSupportFollowers(GameState.I, delta);
        RefreshAll();
    }

    private static void RefreshCard(
        D2Civilization1State state, string id, TMP_Text body, TMP_Text reward)
    {
        long followers = D2PilgrimageSystem.GetFollowersRequired(id);
        long acolytes = D2PilgrimageSystem.GetAcolytesRequired(id);
        string bodyValue = FormatTime(D2PilgrimageSystem.GetDurationSeconds(id)) + "\n" +
            followers.ToString(CultureInfo.InvariantCulture) +
            (followers == 1L ? " SEGUIDOR" : " SEGUIDORES") + "\n";
        if (acolytes > 0L)
            bodyValue += acolytes.ToString(CultureInfo.InvariantCulture) +
                (acolytes == 1L ? " ACÓLITO\n" : " ACÓLITOS\n");
        bodyValue += Number(D2PilgrimageSystem.GetEffectiveWaxCost(state, id)) + " CERA\n" +
            Number(D2PilgrimageSystem.GetEffectiveBreadCost(state, id)) + " PAN RITUAL\n+" +
            Number(D2PilgrimageSystem.GetEffectiveTrustReward(state, id)) + " CONFIANZA";
        Set(body, bodyValue);

        string rewardValue = "RECOMPENSA: +" + Number(D2PilgrimageSystem.GetOfferingReward(id)) +
            " CERA/PAN RITUAL";
        if (id == D2PilgrimageSystem.MediumId)
            rewardValue += "\n25% +1 SEGUIDOR";
        else if (id == D2PilgrimageSystem.LongId || id == D2PilgrimageSystem.GuidedLongId)
            rewardValue += "\n+1 SEGUIDOR";
        else if (id == D2PilgrimageSystem.SacredId)
            rewardValue += "\n+2 SEGUIDORES";
        Set(reward, rewardValue);
    }

    private void RefreshAll()
    {
        D2Civilization1PanelUI parent = Parent();
        if (parent != null) parent.Refresh(); else Refresh();
    }

    private D2Civilization1PanelUI Parent() =>
        GetComponentInParent<D2Civilization1PanelUI>(true);

    private void ShowMap()
    {
        Dimension2PanelUI panel = GetComponentInParent<Dimension2PanelUI>(true);
        if (panel != null) panel.ShowMap();
    }

    private static string FormatTime(double seconds)
    {
        int totalSeconds = Math.Max(0, (int)Math.Ceiling(seconds));
        return (totalSeconds / 60).ToString("00", CultureInfo.InvariantCulture) + ":" +
            (totalSeconds % 60).ToString("00", CultureInfo.InvariantCulture);
    }

    private static string Number(double value) =>
        value.ToString("0.##", CultureInfo.InvariantCulture);
    private static void Set(TMP_Text text, string value) { if (text != null) text.text = value; }
    private static void SetInteractable(Button button, bool value) { if (button != null) button.interactable = value; }
    private static void SetActive(Component component, bool value) { if (component != null) component.gameObject.SetActive(value); }
    private static void Add(Button button, UnityEngine.Events.UnityAction action) { if (button != null) button.onClick.AddListener(action); }
}
