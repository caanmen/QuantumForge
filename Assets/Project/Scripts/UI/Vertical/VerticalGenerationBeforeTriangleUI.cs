using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Supervisa el estado temprano de Generacion. La economia y las compras
/// permanecen en BuildingListUI/BuildingRowUI; este componente solo mantiene
/// la presentacion vertical y su contrato de ausencia de pistas.
/// </summary>
public sealed class VerticalGenerationBeforeTriangleUI : MonoBehaviour
{
    public GameObject beforeTriangleRoot;
    public GameObject triangleRoot;
    public ScrollRect artifactScroll;
    public BuildingListUI buildingList;

    [SerializeField] private float refreshInterval = 0.25f;
    private float timer;

    private void OnEnable()
    {
        RefreshState();
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer < refreshInterval)
            return;
        timer = 0f;
        RefreshState();
    }

    public void RefreshState()
    {
        if (buildingList != null && !buildingList.EnsureInitialized())
            return;
        bool unlocked = GameState.I != null && GameState.I.triangleSystemUnlocked;
        if (beforeTriangleRoot != null && beforeTriangleRoot.activeSelf == unlocked)
            beforeTriangleRoot.SetActive(!unlocked);
        if (triangleRoot != null && triangleRoot.activeSelf != unlocked)
            triangleRoot.SetActive(unlocked);
    }
}
