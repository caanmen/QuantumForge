using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriangleArtifactCardUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Identity")]
    [SerializeField] private string buildingId;

    [Header("Refs")]
    [SerializeField] private TriangleSelectionUI selectionUI;
    [SerializeField] private Image backgroundImage;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.75f, 0.9f, 1f, 1f);

    public string BuildingId => buildingId;

    private void Awake()
    {
        SetSelected(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectionUI == null)
            return;

        selectionUI.SelectCard(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (selectionUI == null)
            return;

        selectionUI.BeginCardDrag(this, eventData.position);
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

        selectionUI.EndCardDrag();
    }

    public void SetSelected(bool selected)
    {
        if (backgroundImage == null)
            return;

        backgroundImage.color = selected ? selectedColor : normalColor;
    }
}