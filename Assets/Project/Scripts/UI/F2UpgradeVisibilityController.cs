using UnityEngine;

public class F2UpgradeVisibilityController : MonoBehaviour
{
    private float _nextRefresh;

    private void Awake()
    {
        if (Application.isPlaying)
            DontDestroyOnLoad(gameObject);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureRuntimeController()
    {
        F2UpgradeVisibilityController existing =
            FindFirstObjectByType<F2UpgradeVisibilityController>();
        if (existing != null)
        {
            DontDestroyOnLoad(existing.gameObject);
            existing.RefreshAll();
            return;
        }

        new GameObject("F2UpgradeVisibilityRuntime")
            .AddComponent<F2UpgradeVisibilityController>();
    }

    private void Update()
    {
        if (Time.unscaledTime < _nextRefresh) return;
        _nextRefresh = Time.unscaledTime + 0.20f;
        RefreshAll();
    }

    public void RefreshAll()
    {
        if (F2UpgradeManager.I == null) return;
        var rows = FindObjectsByType<F2UpgradeRowUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var row in rows)
        {
            if (row == null) continue;
            bool visible = F2UpgradeManager.I.ShouldBeVisible(row.UpgradeId);
            UpgradeStudyState studyState = GameState.I != null
                ? UpgradeStudySystem.EnsureState(GameState.I)
                : null;
            if (visible && studyState != null && studyState.hideCompletedUpgrades &&
                F2UpgradeManager.I.IsMaxed(row.UpgradeId))
                visible = false;
            if (row.gameObject.activeSelf != visible) row.gameObject.SetActive(visible);
            if (visible) row.RefreshNow();
        }
    }
}
