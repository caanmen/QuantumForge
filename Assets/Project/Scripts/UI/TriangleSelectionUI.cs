using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriangleSelectionUI : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] private List<TriangleArtifactCardUI> cards = new();

    [Header("Drag Visual")]
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private Vector2 dragVisualSize = new Vector2(92f, 84f);
    [SerializeField] private Color dragVisualColor = new Color(1f, 1f, 1f, 0.95f);

    private TriangleArtifactCardUI currentSelected;
    private string selectedVirtualBuildingId;
    private bool draggingFromTriangleSlot;

    private GameObject dragVisualObject;
    private RectTransform dragVisualRect;
    private Image dragVisualImage;
    private Text dragVisualText;

    private void Start()
    {
        ClearSelection();
    }

    public void SelectCard(TriangleArtifactCardUI card)
    {
        if (card == null)
            return;

        if (currentSelected == card && string.IsNullOrEmpty(selectedVirtualBuildingId))
        {
            ClearSelection();
            return;
        }

        selectedVirtualBuildingId = null;
        draggingFromTriangleSlot = false;
        currentSelected = card;
        RefreshSelectionVisuals();
    }

    public void SelectVirtualBuilding(string buildingId)
    {
        if (string.IsNullOrEmpty(buildingId))
            return;

        currentSelected = null;
        selectedVirtualBuildingId = buildingId;
        draggingFromTriangleSlot = false;
        RefreshSelectionVisuals();
    }

    public void BeginVirtualDrag(string buildingId, Vector2 screenPosition)
    {
        if (string.IsNullOrEmpty(buildingId))
            return;

        currentSelected = null;
        selectedVirtualBuildingId = buildingId;
        draggingFromTriangleSlot = true;
        RefreshSelectionVisuals();

        ShowDragVisual(buildingId, screenPosition);
    }

    public void EndVirtualDrag()
    {
        draggingFromTriangleSlot = false;
        HideDragVisual();
    }

    public bool IsDraggingFromTriangleSlot()
    {
        return draggingFromTriangleSlot;
    }

    public void ClearSelection()
    {
        currentSelected = null;
        selectedVirtualBuildingId = null;
        draggingFromTriangleSlot = false;
        RefreshSelectionVisuals();
        HideDragVisual();
    }

    public TriangleArtifactCardUI GetSelectedCard()
    {
        return currentSelected;
    }

    public string GetSelectedBuildingId()
    {
        if (currentSelected != null && !string.IsNullOrEmpty(currentSelected.BuildingId))
            return currentSelected.BuildingId;

        return selectedVirtualBuildingId;
    }

    public void BeginCardDrag(TriangleArtifactCardUI card, Vector2 screenPosition)
    {
        if (card == null || string.IsNullOrEmpty(card.BuildingId))
            return;

        SelectCard(card);
        ShowDragVisual(card.BuildingId, screenPosition);
    }

    public void UpdateDragVisualPosition(Vector2 screenPosition)
    {
        if (dragVisualRect == null || rootCanvas == null)
            return;

        RectTransform canvasRect = rootCanvas.transform as RectTransform;
        if (canvasRect == null)
            return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPosition,
                rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
                out localPoint))
        {
            dragVisualRect.anchoredPosition = localPoint;
        }
    }

    public void EndCardDrag()
    {
        HideDragVisual();
    }

    private void ShowDragVisual(string buildingId, Vector2 screenPosition)
    {
        EnsureDragVisualCreated();

        if (dragVisualObject == null)
            return;

        dragVisualObject.SetActive(true);

        if (dragVisualText != null)
            dragVisualText.text = GetShortLabel(buildingId);

        if (dragVisualImage != null)
            dragVisualImage.color = GetDragColor(buildingId);

        UpdateDragVisualPosition(screenPosition);
    }

    private void HideDragVisual()
    {
        if (dragVisualObject != null)
            dragVisualObject.SetActive(false);
    }

    private void EnsureDragVisualCreated()
    {
        if (dragVisualObject != null)
            return;

        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>();

        if (rootCanvas == null)
            return;

        dragVisualObject = new GameObject("TriangleDragVisual");
        dragVisualObject.transform.SetParent(rootCanvas.transform, false);

        dragVisualRect = dragVisualObject.AddComponent<RectTransform>();
        dragVisualRect.sizeDelta = dragVisualSize;
        dragVisualRect.anchorMin = new Vector2(0.5f, 0.5f);
        dragVisualRect.anchorMax = new Vector2(0.5f, 0.5f);
        dragVisualRect.pivot = new Vector2(0.5f, 0.5f);

        dragVisualImage = dragVisualObject.AddComponent<Image>();
        dragVisualImage.color = dragVisualColor;
        dragVisualImage.raycastTarget = false;

        CanvasGroup cg = dragVisualObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(dragVisualObject.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(6f, 6f);
        textRect.offsetMax = new Vector2(-6f, -6f);

        dragVisualText = textObj.AddComponent<Text>();
        dragVisualText.alignment = TextAnchor.MiddleCenter;
        dragVisualText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        dragVisualText.fontSize = 20;
        dragVisualText.color = Color.white;
        dragVisualText.raycastTarget = false;

        dragVisualObject.SetActive(false);
    }

    private void RefreshSelectionVisuals()
    {
        if (cards == null)
            return;

        foreach (var card in cards)
        {
            if (card == null)
                continue;

            bool isSelectedCard =
                currentSelected == card ||
                (!string.IsNullOrEmpty(selectedVirtualBuildingId) && card.BuildingId == selectedVirtualBuildingId);

            card.SetSelected(isSelectedCard);
        }
    }

    private string GetShortLabel(string buildingId)
    {
        switch (buildingId)
        {
            case "vacuum_observer":
                return "Higgs";
            case "casimir_panel":
                return "Tetra";
            case "fluctuation_antenna":
                return "Mod";
            default:
                return "?";
        }
    }

    private Color GetDragColor(string buildingId)
    {
        switch (buildingId)
        {
            case "vacuum_observer":
                return new Color(0.20f, 0.68f, 0.24f, 0.96f); // verde tipo Higgs

            case "casimir_panel":
                return new Color(0.88f, 0.43f, 0.43f, 0.96f); // rojo tipo Tetra

            case "fluctuation_antenna":
                return new Color(0.70f, 0.75f, 0.23f, 0.96f); // amarillo/oliva Modulator

            default:
                return dragVisualColor;
        }
    }
}