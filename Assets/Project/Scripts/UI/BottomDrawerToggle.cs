using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottomDrawerToggle : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform bottomDrawer;
    [SerializeField] private Button handleButton;
    [SerializeField] private TextMeshProUGUI handleText;
    [SerializeField] private GameObject drawerContent;

    [Header("Estados")]
    [SerializeField] private bool startsOpen = true;

    [Header("Posiciones")]
    [SerializeField] private float openY = 0f;
    [SerializeField] private float closedY = -110f;
    

    private bool isOpen;

    private void Awake()
    {
        if (handleButton != null)
        {
            handleButton.onClick.AddListener(ToggleDrawer);
        }

        isOpen = startsOpen;
        ApplyStateInstant();
    }

    private void ToggleDrawer()
    {
        isOpen = !isOpen;
        ApplyStateInstant();
    }

    private void ApplyStateInstant()
    {
        if (bottomDrawer != null)
        {
            Vector2 pos = bottomDrawer.anchoredPosition;
            pos.y = isOpen ? openY : closedY;
            bottomDrawer.anchoredPosition = pos;
        }
        if (handleText != null)
        {
            handleText.text = isOpen ? "▲" : "▼";
        }
        if (drawerContent != null)
        {
            drawerContent.SetActive(isOpen);
        }
    }
}