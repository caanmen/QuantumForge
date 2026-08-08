using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class PrestigeDimensionSwipeSurface : MonoBehaviour,
    IBeginDragHandler, IEndDragHandler
{
    private PrestigeDimensionTransitionUI _owner;
    private Vector2 _dragStart;

    public void Initialize(PrestigeDimensionTransitionUI owner)
    {
        _owner = owner;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStart = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_owner == null)
            return;

        float delta = eventData.position.x - _dragStart.x;
        if (Mathf.Abs(delta) < 64f)
            return;

        _owner.MoveSelection(delta < 0f ? 1 : -1);
    }
}
