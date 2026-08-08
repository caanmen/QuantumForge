using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class VerticalUpgradesScreenUI : MonoBehaviour
{
    private static readonly string[] ProductionIds =
    {
        "emission_focus",
        "containment_tuning"
    };

    private static readonly string[] TraceIds =
    {
        "tetraquark_stabilization"
    };

    private static readonly string[] TriangleIds =
    {
        "triangle_unlock_1",
        "triangle_impulse_tuning",
        "triangle_synergy_resonance",
        "triangle_persistence_anchor",
        "triangle_energy_efficiency"
    };

    public RectTransform content;
    public RectTransform productionSection;
    public RectTransform tracesSection;
    public RectTransform triangleSection;
    public F2UpgradeRowUI[] rows;
    public KeycardPurchaseUI keycardRow;
    public Button hideCompletedButton;
    public TMP_Text hideCompletedLabel;

    private const float RefreshInterval = 0.20f;
    private float _nextRefresh;

    private void OnEnable()
    {
        EnsureHideCompletedBinding();
        _nextRefresh = 0f;
        RefreshNow();
    }

    private void Update()
    {
        if (Time.unscaledTime < _nextRefresh) return;
        _nextRefresh = Time.unscaledTime + RefreshInterval;
        RefreshNow();
    }

    [ContextMenu("Refresh vertical upgrades")]
    public void RefreshNow()
    {
        F2UpgradeManager manager = F2UpgradeManager.I;
        if (manager == null) return;

        EnsureKeycardBinding();

        if (rows != null)
        {
            foreach (F2UpgradeRowUI row in rows)
            {
                if (row == null) continue;
                bool visible = ShouldShowRow(manager, row.UpgradeId);
                if (row.gameObject.activeSelf != visible)
                    row.gameObject.SetActive(visible);
                if (visible)
                    row.RefreshNow();
            }
        }

        SetSectionVisible(productionSection, HasAnyVisible(manager, ProductionIds));
        SetSectionVisible(tracesSection, HasAnyVisible(manager, TraceIds));
        bool keycardVisible = GameState.I != null &&
            UpgradeStudySystem.ShouldShowStudyOpportunity(GameState.I,
                UpgradeStudySystem.KeycardProjectUnlockId) &&
            !GameState.I.experimentalChamberUnlocked;
        if (keycardRow != null && keycardRow.gameObject.activeSelf != keycardVisible)
            keycardRow.gameObject.SetActive(keycardVisible);

        SetSectionVisible(triangleSection,
            HasAnyVisible(manager, TriangleIds) || keycardVisible);

        if (content != null)
            LayoutRebuilder.MarkLayoutForRebuild(content);

        RefreshHideCompletedLabel();
    }

    public bool HasAnyVisible(F2UpgradeManager manager, string[] ids)
    {
        if (manager == null || ids == null) return false;
        foreach (string id in ids)
            if (ShouldShowRow(manager, id))
                return true;
        return false;
    }

    public void OnClickToggleCompleted()
    {
        if (GameState.I == null) return;
        UpgradeStudyState state = UpgradeStudySystem.EnsureState(GameState.I);
        state.hideCompletedUpgrades = !state.hideCompletedUpgrades;
        SaveService.I?.Save();
        RefreshNow();
    }

    private bool ShouldShowRow(F2UpgradeManager manager, string id)
    {
        if (manager == null || !manager.ShouldBeVisible(id)) return false;
        UpgradeStudyState state = UpgradeStudySystem.EnsureState(GameState.I);
        return state == null || !state.hideCompletedUpgrades || !manager.IsMaxed(id);
    }

    private void EnsureHideCompletedBinding()
    {
        if (hideCompletedButton == null)
        {
            Transform target = transform.Find("Scroll/Viewport/Content/HideCompletedButton");
            if (target == null && content != null)
                target = content.Find("HideCompletedButton");
            if (target != null) hideCompletedButton = target.GetComponent<Button>();
        }
        if (hideCompletedLabel == null && hideCompletedButton != null)
            hideCompletedLabel = hideCompletedButton.GetComponentInChildren<TMP_Text>(true);
        if (hideCompletedButton != null)
        {
            hideCompletedButton.onClick.RemoveListener(OnClickToggleCompleted);
            hideCompletedButton.onClick.AddListener(OnClickToggleCompleted);
        }
    }

    private void RefreshHideCompletedLabel()
    {
        if (hideCompletedLabel == null) return;
        bool hidden = GameState.I != null &&
            UpgradeStudySystem.EnsureState(GameState.I).hideCompletedUpgrades;
        string key = hidden ? "upgrades.hide_completed.on" : "upgrades.hide_completed.off";
        string fallback = hidden ? "Completadas: ocultas" : "Ocultar completadas";
        string localized = LocalizationManager.I != null
            ? LocalizationManager.I.T(key)
            : fallback;
        hideCompletedLabel.SetText(string.IsNullOrEmpty(localized) || localized == key
            ? fallback
            : localized);
    }

    private void EnsureKeycardBinding()
    {
        if (keycardRow != null || triangleSection == null) return;
        keycardRow = triangleSection.GetComponentInChildren<KeycardPurchaseUI>(true);
    }

    private static void SetSectionVisible(RectTransform section, bool visible)
    {
        if (section == null) return;

        LayoutElement layout = section.GetComponent<LayoutElement>();
        if (layout != null)
            layout.ignoreLayout = !visible;

        CanvasGroup group = section.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = visible ? 1f : 0f;
            group.interactable = visible;
            group.blocksRaycasts = visible;
        }
    }
}
