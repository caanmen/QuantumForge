using UnityEngine;

public class F2UpgradeVisibilityController : MonoBehaviour
{
    [Header("Trace rows")]
    [SerializeField] private GameObject residualAnalysisRow;
    [SerializeField] private GameObject patternMappingRow;

    [Header("Unlock condition")]
    [SerializeField] private string requiredBuildingId = "fluctuation_antenna";
    [SerializeField] private int requiredLevel = 1;

    private void Update()
    {
        bool showTraceRows = false;

        if (GameState.I != null)
        {
            showTraceRows = GameState.I.GetBuildingLevel(requiredBuildingId) >= requiredLevel;
        }

        SetRowVisible(residualAnalysisRow, showTraceRows);
        SetRowVisible(patternMappingRow, showTraceRows);
    }

    private void SetRowVisible(GameObject row, bool visible)
    {
        if (row == null) return;

        if (row.activeSelf != visible)
            row.SetActive(visible);
    }
}