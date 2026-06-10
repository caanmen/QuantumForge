using UnityEngine;
using UnityEngine.EventSystems;

public class TriangleSlotUI : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [Header("Slot")]
    [SerializeField] private TriangleSlotRole slotRole;

    [Header("Refs")]
    [SerializeField] private TriangleSelectionUI selectionUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameState.I == null || !GameState.I.triangleSystemUnlocked)
            return;

        if (selectionUI == null)
            return;

        string selectedBuildingId = selectionUI.GetSelectedBuildingId();

        if (!string.IsNullOrEmpty(selectedBuildingId))
        {
            bool changed = GameState.I.AssignTriangleBuilding(slotRole, selectedBuildingId);
            if (changed)
                selectionUI.ClearSelection();

            return;
        }

        string currentBuildingId = GameState.I.GetTriangleBuildingId(slotRole);
        if (string.IsNullOrEmpty(currentBuildingId))
            return;

        selectionUI.SelectVirtualBuilding(currentBuildingId);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameState.I == null || !GameState.I.triangleSystemUnlocked)
            return;

        if (selectionUI == null)
            return;

        string currentBuildingId = GameState.I.GetTriangleBuildingId(slotRole);
        if (string.IsNullOrEmpty(currentBuildingId))
            return;

        selectionUI.BeginVirtualDrag(currentBuildingId, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (selectionUI == null)
            return;

        selectionUI.UpdateDragVisualPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (selectionUI == null)
            return;

        selectionUI.EndVirtualDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (GameState.I == null || !GameState.I.triangleSystemUnlocked)
            return;

        if (selectionUI == null)
            return;

        if (eventData.pointerDrag != null)
        {
            TriangleArtifactCardUI draggedCard = eventData.pointerDrag.GetComponent<TriangleArtifactCardUI>();
            if (draggedCard != null && !string.IsNullOrEmpty(draggedCard.BuildingId))
            {
                bool changedFromDrawer = GameState.I.AssignTriangleBuilding(slotRole, draggedCard.BuildingId);
                if (changedFromDrawer)
                    selectionUI.ClearSelection();

                return;
            }
        }

        if (selectionUI.IsDraggingFromTriangleSlot())
        {
            string selectedBuildingId = selectionUI.GetSelectedBuildingId();
            if (string.IsNullOrEmpty(selectedBuildingId))
                return;

            bool changedFromSlot = GameState.I.AssignTriangleBuilding(slotRole, selectedBuildingId);
            if (changedFromSlot)
                selectionUI.ClearSelection();
        }
    }
}