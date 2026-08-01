using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class MachineCube3DPrototypeDisplayUI : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private MachineCube3DPrototypeController controller;
    [SerializeField] private MachineCubeVisualUI machineVisual;
    [SerializeField] private LayerMask nodeLayer;
    [SerializeField] private float swipeThresholdPixels = 64f;

    private Vector2 _dragStart;

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
            machineVisual?.SelectNode(node.NodeId);
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
