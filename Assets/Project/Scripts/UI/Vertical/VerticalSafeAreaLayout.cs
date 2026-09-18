using UnityEngine;

[DisallowMultipleComponent]
public sealed class VerticalSafeAreaLayout : MonoBehaviour
{
    public RectTransform safeAreaRoot;
    public RectTransform headerSlot;
    public RectTransform contentSlot;
    public RectTransform secondaryNavigationSlot;
    public RectTransform primaryNavigationSlot;

    [Header("Medidas en referencia 1080 x 1920")]
    [Min(0f)] public float horizontalMargin = 24f;
    // El contenido debe cubrir el ancho util de la Safe Area para que el fondo
    // tecnico de la raiz no aparezca como una franja lateral. Las barras
    // conservan horizontalMargin como separacion visual independiente.
    [Min(0f)] public float contentHorizontalMargin = 0f;
    // La cabecera de recursos pertenece a cada pantalla. HeaderSlot queda
    // disponible para futuras cabeceras globales, pero no reserva espacio.
    [Min(0f)] public float headerHeight = 0f;
    [Min(0f)] public float primaryNavigationHeight = 132f;
    [Min(0f)] public float secondaryNavigationHeight = 104f;
    [Min(0f)] public float verticalGap = 16f;

    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;
    private Rect lastSafeArea;
    private bool lastSecondaryNavigationActive;
    private bool secondaryNavigationOverlay;

    public void SetSecondaryNavigationOverlay(bool overlay)
    {
        if (secondaryNavigationOverlay == overlay) return;
        secondaryNavigationOverlay = overlay;
        ApplyLayout();
    }

    private void Awake() => ApplyLayout();

    private void OnEnable() => ApplyLayout();

    private void Update()
    {
        bool secondaryActive = secondaryNavigationSlot != null &&
            secondaryNavigationSlot.gameObject.activeSelf && !secondaryNavigationOverlay;
        if (lastScreenWidth != Screen.width ||
            lastScreenHeight != Screen.height ||
            lastSafeArea != Screen.safeArea ||
            lastSecondaryNavigationActive != secondaryActive)
        {
            ApplyLayout();
        }
    }

    [ContextMenu("Apply vertical safe area layout")]
    public void ApplyLayout()
    {
        if (safeAreaRoot == null)
            return;

        Rect safeArea = Screen.safeArea;
        if (Screen.width <= 0 || Screen.height <= 0 ||
            safeArea.width <= 0f || safeArea.height <= 0f)
        {
            safeArea = new Rect(0f, 0f,
                Mathf.Max(1, Screen.width), Mathf.Max(1, Screen.height));
        }

        CalculateSafeAnchors(safeArea,
            Mathf.Max(1, Screen.width), Mathf.Max(1, Screen.height),
            out Vector2 safeMin, out Vector2 safeMax);
        safeAreaRoot.anchorMin = safeMin;
        safeAreaRoot.anchorMax = safeMax;
        safeAreaRoot.pivot = new Vector2(0.5f, 0.5f);
        safeAreaRoot.anchoredPosition = Vector2.zero;
        safeAreaRoot.sizeDelta = Vector2.zero;

        ConfigureTopSlot(headerSlot, horizontalMargin, headerHeight, verticalGap);
        ConfigureBottomSlot(primaryNavigationSlot, horizontalMargin,
            primaryNavigationHeight, verticalGap);

        bool secondaryActive = secondaryNavigationSlot != null &&
            secondaryNavigationSlot.gameObject.activeSelf && !secondaryNavigationOverlay;
        float secondaryBottom = primaryNavigationHeight + verticalGap * 2f;
        ConfigureBottomSlot(secondaryNavigationSlot, horizontalMargin,
            secondaryNavigationHeight, secondaryBottom);

        if (contentSlot != null)
        {
            Stretch(contentSlot);
            float bottom = primaryNavigationHeight + verticalGap * 2f;
            if (secondaryActive)
                bottom += secondaryNavigationHeight + verticalGap;
            contentSlot.offsetMin = new Vector2(contentHorizontalMargin, bottom);
            contentSlot.offsetMax = new Vector2(-contentHorizontalMargin,
                -(headerHeight + verticalGap * 2f));
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        lastSafeArea = Screen.safeArea;
        lastSecondaryNavigationActive = secondaryActive;
    }

    public static void CalculateSafeAnchors(
        Rect safeArea,
        int screenWidth,
        int screenHeight,
        out Vector2 min,
        out Vector2 max)
    {
        float width = Mathf.Max(1, screenWidth);
        float height = Mathf.Max(1, screenHeight);
        min = new Vector2(
            Mathf.Clamp01(safeArea.xMin / width),
            Mathf.Clamp01(safeArea.yMin / height));
        max = new Vector2(
            Mathf.Clamp01(safeArea.xMax / width),
            Mathf.Clamp01(safeArea.yMax / height));
        max.x = Mathf.Max(min.x, max.x);
        max.y = Mathf.Max(min.y, max.y);
    }

    private static void ConfigureTopSlot(
        RectTransform rect, float margin, float height, float top)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -top);
        rect.sizeDelta = new Vector2(-margin * 2f, height);
    }

    private static void ConfigureBottomSlot(
        RectTransform rect, float margin, float height, float bottom)
    {
        if (rect == null)
            return;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, bottom);
        rect.sizeDelta = new Vector2(-margin * 2f, height);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }
}
