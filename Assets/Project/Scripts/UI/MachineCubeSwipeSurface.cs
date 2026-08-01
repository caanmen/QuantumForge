using UnityEngine;
using UnityEngine.EventSystems;

public sealed class MachineCubeSwipeSurface : MonoBehaviour,
    IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private MachineCubeVisualUI controller;
    [SerializeField] private float thresholdPixels = 64f;
    private Vector2 _start;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _start = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float delta = eventData.position.x - _start.x;
        if (Mathf.Abs(delta) < thresholdPixels)
            return;

        controller?.RotateBy(delta < 0f ? 1 : -1);
    }
}
