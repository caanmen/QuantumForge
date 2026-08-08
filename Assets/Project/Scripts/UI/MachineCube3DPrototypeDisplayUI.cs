using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class MachineCube3DPrototypeDisplayUI : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private MachineCube3DPrototypeController controller;
    [SerializeField] private MachineCubeVisualUI machineVisual;
    [SerializeField] private MachinePanelUI machinePanel;
    [SerializeField] private LayerMask nodeLayer;
    [SerializeField] private float swipeThresholdPixels = 64f;
    [SerializeField, Min(0.08f)] private float selectionSyncInterval = 0.15f;

    private Vector2 _dragStart;
    private float _nextSelectionSyncTime;
    private string _lastSyncedSelection = "";

    private void Awake()
    {
        if (machinePanel == null)
            machinePanel = GetComponentInParent<MachinePanelUI>(true);
        if (machinePanel == null)
            machinePanel = FindFirstObjectByType<MachinePanelUI>(
                FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        _nextSelectionSyncTime = 0f;
        RefreshSelectedNode(true);
    }

    private void Update()
    {
        if (Time.unscaledTime < _nextSelectionSyncTime)
            return;
        _nextSelectionSyncTime = Time.unscaledTime + selectionSyncInterval;
        RefreshSelectedNode();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || controller.IsRotating ||
            controller.PrototypeCamera == null)
            return;

        RectTransform rect = transform as RectTransform;
        if (rect == null || !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, eventData.position, eventData.pressEventCamera, out Vector2 local))
            return;

        Rect bounds = rect.rect;
        float u = Mathf.InverseLerp(bounds.xMin, bounds.xMax, local.x);
        float v = Mathf.InverseLerp(bounds.yMin, bounds.yMax, local.y);
        Ray ray = controller.PrototypeCamera.ViewportPointToRay(new Vector3(u, v, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, nodeLayer,
                QueryTriggerInteraction.Ignore))
            return;

        MachineCube3DNode node = hit.collider.GetComponentInParent<MachineCube3DNode>();
        if (node != null && !string.IsNullOrWhiteSpace(node.NodeId))
        {
            machineVisual?.SelectNode(node.NodeId);
            string selectedId = machinePanel != null
                ? machinePanel.SelectedNodeId
                : node.NodeId;
            _lastSyncedSelection = selectedId ?? "";
            controller.SetSelectedNode(_lastSyncedSelection);
        }
    }

    /// <summary>
    /// Keeps physical selection synchronized with arrows, repaired-tier
    /// auto-advance and restored panel state, not only pointer clicks.
    /// </summary>
    public void RefreshSelectedNode(bool force = false)
    {
        if (controller == null || machinePanel == null)
            return;
        string selectedId = machinePanel.SelectedNodeId ?? "";
        if (!force && selectedId == _lastSyncedSelection)
            return;
        _lastSyncedSelection = selectedId;
        controller.SetSelectedNode(selectedId);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStart = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float delta = eventData.position.x - _dragStart.x;
        if (Mathf.Abs(delta) < swipeThresholdPixels)
            return;
        machineVisual?.RotateBy(delta < 0f ? 1 : -1);
    }
}
