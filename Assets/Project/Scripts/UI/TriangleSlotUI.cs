using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Header("Circuit colors")]
    [SerializeField] private Color inactiveColor = new Color(0.22f, 0.28f, 0.36f, 0.9f);
    [SerializeField] private Color activeColor = new Color(0.25f, 0.85f, 1f, 1f);
    [SerializeField] private Color lockedColor = new Color(0.18f, 0.18f, 0.2f, 0.65f);

    private Image backgroundImage;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }

    private void Update()
    {
        RefreshVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TriangleCircuitType circuit = GetCircuit();
        if (GameState.I == null || circuit == TriangleCircuitType.None) return;
        GameState.I.SetTriangleCircuit(circuit);
        RefreshVisual();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Las ranuras antiguas ahora son selectores de circuito, no elementos arrastrables.
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Sin arrastre en el Triángulo rediseñado.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Sin arrastre en el Triángulo rediseñado.
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Soltar artefactos ya no modifica el circuito.
    }

    private TriangleCircuitType GetCircuit()
    {
        if (slotRole == TriangleSlotRole.Primary) return TriangleCircuitType.Energy;
        if (slotRole == TriangleSlotRole.Reinforcement) return TriangleCircuitType.Experimental;
        if (slotRole == TriangleSlotRole.Alteration) return TriangleCircuitType.Phase;
        return TriangleCircuitType.None;
    }

    private void RefreshVisual()
    {
        if (backgroundImage == null) return;
        TriangleCircuitType circuit = GetCircuit();
        bool locked = GameState.I == null || !GameState.I.triangleSystemUnlocked ||
            !GameState.I.AreTriangleVerticesAvailable() ||
            (circuit == TriangleCircuitType.Phase && !GameState.I.IsTrianglePhaseUnlocked());
        if (locked)
            backgroundImage.color = lockedColor;
        else if (GameState.I.triangleActiveCircuit == circuit)
            backgroundImage.color = activeColor;
        else
            backgroundImage.color = inactiveColor;
    }
}
