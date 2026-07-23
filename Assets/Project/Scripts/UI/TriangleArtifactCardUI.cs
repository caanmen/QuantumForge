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
        if (backgroundImage != null)
            backgroundImage.raycastTarget = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Los artefactos son vértices fijos en el Triángulo rediseñado.
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Sin arrastre.
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Sin arrastre.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Sin arrastre.
    }

    public void SetSelected(bool selected)
    {
        if (backgroundImage == null)
            return;

        backgroundImage.color = selected ? selectedColor : normalColor;
    }
}
