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
        "triangle_persistence_anchor"
    };

    public RectTransform content;
    public RectTransform productionSection;
    public RectTransform tracesSection;
    public RectTransform triangleSection;
    public F2UpgradeRowUI[] rows;

    private const float RefreshInterval = 0.20f;
    private float _nextRefresh;

    private void OnEnable()
    {
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

        if (rows != null)
        {
            foreach (F2UpgradeRowUI row in rows)
            {
                if (row == null) continue;
                bool visible = manager.ShouldBeVisible(row.UpgradeId);
                if (row.gameObject.activeSelf != visible)
                    row.gameObject.SetActive(visible);
                if (visible)
                    row.RefreshNow();
            }
        }

        SetSectionVisible(productionSection,
            HasAnyVisible(manager, ProductionIds));
        SetSectionVisible(tracesSection,
            HasAnyVisible(manager, TraceIds));
        SetSectionVisible(triangleSection,
            HasAnyVisible(manager, TriangleIds));

        if (content != null)
            LayoutRebuilder.MarkLayoutForRebuild(content);
    }

    public static bool HasAnyVisible(F2UpgradeManager manager, string[] ids)
    {
        if (manager == null || ids == null) return false;
        foreach (string id in ids)
            if (manager.ShouldBeVisible(id))
                return true;
        return false;
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
