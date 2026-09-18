using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Supervisa el estado temprano de Generacion. La economia y las compras
/// permanecen en BuildingListUI/BuildingRowUI; este componente solo mantiene
/// la presentacion vertical y su contrato de ausencia de pistas.
/// </summary>
public sealed class VerticalGenerationBeforeTriangleUI : MonoBehaviour
{
    private const float ExpandedContentHeight = 1620f;
    private const float CompactContentHeight = 1480f;
    private const float ExpandedFocusHeight = 850f;
    private const float CompactFocusHeight = 710f;
    private const float ExpandedCircuitsTop = 860f;
    private const float CompactCircuitsTop = 720f;
    private const float ExpandedPurchasesTop = 1128f;
    private const float CompactPurchasesTop = 988f;

    public GameObject beforeTriangleRoot;
    public GameObject triangleRoot;
    public ScrollRect artifactScroll;
    public BuildingListUI buildingList;

    [SerializeField] private float refreshInterval = 0.25f;
    private float timer;
    private bool? compactLayoutApplied;

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
        RefreshAdvancedLayout();
    }

    private void RefreshAdvancedLayout()
    {
        ScrollRect advancedScroll = triangleRoot != null
            ? triangleRoot.transform.Find("TriangleScroll")?.GetComponent<ScrollRect>()
            : null;
        if (advancedScroll == null || advancedScroll.content == null)
            return;

        VerticalNavigationUI navigation = TabsUI.Instance != null
            ? TabsUI.Instance.verticalNavigation
            : null;
        bool compact = navigation != null &&
            navigation.secondaryNavigationRoot != null &&
            navigation.secondaryNavigationRoot.gameObject.activeSelf;
        if (compactLayoutApplied.HasValue && compactLayoutApplied.Value == compact)
            return;

        RectTransform content = advancedScroll.content;
        Transform focus = content.Find("TriangleFocus");
        Transform circuits = content.Find("CircuitSelectors");
        Transform purchasesFrame = content.Find("TrianglePurchasesFrame");
        Transform purchasesTitle = content.Find("TriangleArtifactsTitle");
        Transform cards = content.Find("TriangleArtifactCards");
        if (focus == null || circuits == null || purchasesFrame == null ||
            purchasesTitle == null || cards == null)
            return;

        float contentHeight = compact ? CompactContentHeight : ExpandedContentHeight;
        float focusHeight = compact ? CompactFocusHeight : ExpandedFocusHeight;
        float circuitsTop = compact ? CompactCircuitsTop : ExpandedCircuitsTop;
        float purchasesTop = compact ? CompactPurchasesTop : ExpandedPurchasesTop;
        content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);
        SetTopRect((RectTransform)focus, 0f, focusHeight, 0f, 0f);
        ApplyFocusGeometry(focus, compact);
        SetTopRect((RectTransform)circuits, circuitsTop, 260f, 10f, -10f);
        SetTopRect((RectTransform)purchasesFrame, purchasesTop, 470f, 10f, -10f);
        SetTopRect((RectTransform)purchasesTitle, purchasesTop + 6f, 42f, 28f, -28f);
        SetTopRect((RectTransform)cards, purchasesTop + 48f, 390f, 22f, -22f);

        compactLayoutApplied = compact;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        advancedScroll.StopMovement();
        advancedScroll.verticalNormalizedPosition = Mathf.Clamp01(
            advancedScroll.verticalNormalizedPosition);
    }

    private static void SetTopRect(
        RectTransform rect, float top, float height, float left, float right)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2((left + right) * 0.5f, -top);
        rect.sizeDelta = new Vector2(-(left - right), height);
    }

    private static void ApplyFocusGeometry(Transform focus, bool compact)
    {
        Vector2 higgsPosition = new(-202f, compact ? 145f : 215f);
        Vector2 tetraPosition = new(202f, compact ? 145f : 215f);
        Vector2 modulatorPosition = new(0f, compact ? -150f : -220f);
        SetPosition(focus.Find("Vertex_Higgs"), higgsPosition);
        SetPosition(focus.Find("Vertex_Tetra"), tetraPosition);
        SetPosition(focus.Find("Vertex_Modulator"), modulatorPosition);
        ConfigureBeam(focus.Find("Line_Experimental"),
            higgsPosition, tetraPosition, 24f);
        ConfigureBeam(focus.Find("Line_Energy"),
            higgsPosition, modulatorPosition, 26f);
        ConfigureBeam(focus.Find("Line_Phase"),
            tetraPosition, modulatorPosition, 26f);
        SetPosition(focus.Find("EnergyGauge"),
            new Vector2(0f, compact ? -24f : 46f));
    }

    private static void SetPosition(Transform target, Vector2 position)
    {
        if (target is RectTransform rect)
            rect.anchoredPosition = position;
    }

    private static void ConfigureBeam(
        Transform target, Vector2 from, Vector2 to, float thickness)
    {
        if (target is not RectTransform rect)
            return;
        Vector2 delta = to - from;
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = (from + to) * .5f;
        rect.sizeDelta = new Vector2(delta.magnitude, thickness);
        rect.localEulerAngles = new Vector3(
            0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }
}
