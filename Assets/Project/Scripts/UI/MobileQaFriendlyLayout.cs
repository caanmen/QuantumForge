using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MobileQaFriendlyLayout : MonoBehaviour
{
    private const float BottomSafeInset = 16f;
    private const float BottomContentClearance = 90f;
    private const float PortraitBottomContentClearance = 170f;
    private const float PortraitNavigationHeight = 112f;
    private const float ButtonAuditInterval = 1f;
    private const float MobileButtonMinimumFontSize = 10f;
    private const float MobileButtonMaximumFontSize = 28f;
    private const float MobileButtonTextMargin = 4f;

    public RectTransform mobileSafeAreaRoot;
    public RectTransform qaSafeAreaRoot;
    public RectTransform qaSpeedButton;
    public RectTransform qaToolsButton;
    public RectTransform languageButton;
    public RectTransform devResetButton;
    public RectTransform resourceHud;
    public RectTransform leText;
    public RectTransform tracesText;
    public RectTransform generationTopArea;
    public RectTransform bottomDrawer;
    public RectTransform drawerHandle;
    public RectTransform drawerContent;
    public RectTransform navButtonsContainer;
    public RectTransform dimension2ContentRoot;
    public RectTransform dimension3ContentRoot;
    public bool forcePortraitLayout;

    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;
    private Rect lastSafeArea;
    private int lastVisibleNavigationCount = -1;
    private float nextButtonAuditTime;
    private readonly HashSet<int> configuredButtonLabels = new();

    private void Awake()
    {
        ApplyLayout();
    }

    private void OnEnable()
    {
        ApplyLayout();
    }

    private void Update()
    {
        bool screenChanged = lastScreenWidth != Screen.width ||
            lastScreenHeight != Screen.height || lastSafeArea != Screen.safeArea;
        int visibleNavigationCount = CountVisibleNavigationButtons();
        if (screenChanged || visibleNavigationCount != lastVisibleNavigationCount)
        {
            ApplyLayout();
        }

        if (Time.unscaledTime >= nextButtonAuditTime)
        {
            ConfigureAllLoadedButtonLabels();
            nextButtonAuditTime = Time.unscaledTime + ButtonAuditInterval;
        }
    }

    public void ApplyLayout()
    {
        bool portrait = UsePortraitLayout;
        ApplySafeArea();
        float utilityBottom = portrait
            ? PortraitNavigationHeight + BottomSafeInset * 2f
            : BottomSafeInset;
        ConfigureBottomUtilityButton(qaToolsButton, 150f, 52f, 16f,
            utilityBottom);
        ConfigureBottomUtilityButton(languageButton, 90f, 36f, 178f,
            utilityBottom);
        ConfigureDevResetButton(portrait);
        if (qaSpeedButton != null)
        {
            qaSpeedButton.sizeDelta = new Vector2(132f, 52f);
            qaSpeedButton.gameObject.SetActive(false);
        }

        ConfigureResourceHud(portrait);
        ConfigureBottomNavigation(portrait);
        ConfigureDimensionContentClearance(dimension2ContentRoot, portrait);
        ConfigureDimensionContentClearance(dimension3ContentRoot, portrait);
        ConfigureAllLoadedButtonLabels();

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        lastSafeArea = Screen.safeArea;
        lastVisibleNavigationCount = CountVisibleNavigationButtons();
    }

    private void ConfigureAllLoadedButtonLabels()
    {
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button == null)
                continue;

            TMP_Text[] tmpLabels =
                button.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text label in tmpLabels)
            {
                if (label != null && configuredButtonLabels.Add(label.GetInstanceID()))
                    ConfigureMobileLabel(label);
            }

            Text[] legacyLabels = button.GetComponentsInChildren<Text>(true);
            foreach (Text label in legacyLabels)
            {
                if (label != null && configuredButtonLabels.Add(label.GetInstanceID()))
                    ConfigureMobileLabel(label);
            }
        }
    }

    public static void ConfigureButtonForMobile(Button button)
    {
        if (button == null)
            return;

        foreach (TMP_Text label in button.GetComponentsInChildren<TMP_Text>(true))
            ConfigureMobileLabel(label);
        foreach (Text label in button.GetComponentsInChildren<Text>(true))
            ConfigureMobileLabel(label);
    }

    private static void ConfigureMobileLabel(TMP_Text label)
    {
        if (label == null)
            return;

        float currentSize = label.fontSize > 0f
            ? label.fontSize
            : MobileButtonMaximumFontSize;
        float minimumSize = label.fontSizeMin > 0f
            ? Mathf.Min(label.fontSizeMin, MobileButtonMinimumFontSize)
            : MobileButtonMinimumFontSize;

        label.enableAutoSizing = true;
        label.fontSizeMin = minimumSize;
        label.fontSizeMax = Mathf.Max(minimumSize,
            Mathf.Min(currentSize, MobileButtonMaximumFontSize));
        label.textWrappingMode = TextWrappingModes.Normal;
        label.margin = new Vector4(
            Mathf.Max(label.margin.x, MobileButtonTextMargin),
            Mathf.Max(label.margin.y, MobileButtonTextMargin),
            Mathf.Max(label.margin.z, MobileButtonTextMargin),
            Mathf.Max(label.margin.w, MobileButtonTextMargin));
    }

    private static void ConfigureMobileLabel(Text label)
    {
        if (label == null)
            return;

        int currentSize = Mathf.Max(1, label.fontSize);
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = Mathf.Min(
            Mathf.Max(1, label.resizeTextMinSize),
            Mathf.RoundToInt(MobileButtonMinimumFontSize));
        label.resizeTextMaxSize = Mathf.Max(
            label.resizeTextMinSize,
            Mathf.Min(currentSize, Mathf.RoundToInt(MobileButtonMaximumFontSize)));
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Truncate;
    }

    public static bool ShouldStackResourceHud(float safeCanvasWidth)
    {
        return false;
    }

    private bool UsePortraitLayout => forcePortraitLayout ||
        Screen.height >= Screen.width;

    private void ApplySafeArea()
    {
        if (mobileSafeAreaRoot == null || Screen.width <= 0 || Screen.height <= 0)
            return;

        Rect safeArea = Screen.safeArea;
        Vector2 min = safeArea.position;
        Vector2 max = safeArea.position + safeArea.size;
        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;
        mobileSafeAreaRoot.anchorMin = min;
        mobileSafeAreaRoot.anchorMax = max;
        mobileSafeAreaRoot.offsetMin = Vector2.zero;
        mobileSafeAreaRoot.offsetMax = Vector2.zero;

        // QaPanelUI applies the same safe area independently. Keep its root
        // stretched in the scene so the two systems never accumulate offsets.
        if (qaSafeAreaRoot != null)
        {
            qaSafeAreaRoot.offsetMin = Vector2.zero;
            qaSafeAreaRoot.offsetMax = Vector2.zero;
        }
    }

    private static void ConfigureBottomUtilityButton(
        RectTransform rect, float width, float height, float rightInset,
        float bottomInset)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = new Vector2(-rightInset, bottomInset);
        rect.sizeDelta = new Vector2(width, height);
        TMP_Text label = rect.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
            label.fontSize = 14f;
    }

    private void ConfigureResourceHud(bool portrait)
    {
        if (resourceHud != null)
        {
            resourceHud.anchorMin = new Vector2(0f, 1f);
            resourceHud.anchorMax = new Vector2(portrait ? 1f : 0f, 1f);
            resourceHud.pivot = new Vector2(portrait ? 0.5f : 0f, 1f);
            resourceHud.anchoredPosition = new Vector2(0f, portrait ? -24f : -16f);
            resourceHud.sizeDelta = portrait
                ? new Vector2(-48f, 126f)
                : new Vector2(560f, 76f);
        }

        ConfigureResourceText(leText, 0f, 0.5f, portrait);
        ConfigureResourceText(tracesText, 0.5f, 1f, portrait);

        if (generationTopArea != null)
            Stretch(generationTopArea);
    }

    private void ConfigureDevResetButton(bool portrait)
    {
        if (devResetButton == null)
            return;

        devResetButton.anchorMin = Vector2.zero;
        devResetButton.anchorMax = Vector2.zero;
        devResetButton.pivot = Vector2.zero;
        devResetButton.anchoredPosition =
            new Vector2(BottomSafeInset, portrait
                ? PortraitNavigationHeight + BottomSafeInset * 2f
                : BottomSafeInset);
        devResetButton.sizeDelta = new Vector2(160f, 30f);
    }

    private static void ConfigureResourceText(
        RectTransform rect, float minX, float maxX, bool portrait)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(minX, 0f);
        rect.anchorMax = new Vector2(maxX, 1f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.offsetMin = new Vector2(10f, 4f);
        rect.offsetMax = new Vector2(-10f, -4f);
        TMP_Text label = rect.GetComponent<TMP_Text>();
        if (label != null)
        {
            label.fontSize = portrait ? 34f : 28f;
            label.enableAutoSizing = true;
            label.fontSizeMin = 18f;
            label.fontSizeMax = portrait ? 34f : 28f;
        }
    }

    private void ConfigureBottomNavigation(bool portrait)
    {
        if (bottomDrawer != null)
        {
            bottomDrawer.anchorMin = new Vector2(0f, 0f);
            bottomDrawer.anchorMax = new Vector2(1f, 0f);
            bottomDrawer.pivot = new Vector2(0.5f, 0f);
            bottomDrawer.anchoredPosition = portrait
                ? new Vector2(0f, BottomSafeInset)
                : new Vector2(-135f, BottomSafeInset);
            bottomDrawer.sizeDelta = portrait
                ? new Vector2(-32f, PortraitNavigationHeight)
                : new Vector2(-550f, 58f);
        }

        if (drawerHandle != null)
        {
            drawerHandle.anchorMin = new Vector2(1f, 0f);
            drawerHandle.anchorMax = new Vector2(1f, 0f);
            drawerHandle.pivot = new Vector2(1f, 0f);
            drawerHandle.anchoredPosition = portrait
                ? new Vector2(-20f, PortraitNavigationHeight + 24f)
                : new Vector2(-274f, 16f);
            drawerHandle.sizeDelta = portrait
                ? new Vector2(110f, 40f)
                : new Vector2(120f, 34f);

            TMP_Text handleLabel =
                drawerHandle.GetComponentInChildren<TMP_Text>(true);
            if (handleLabel != null)
            {
                handleLabel.fontSize = portrait ? 16f : 14f;
                handleLabel.enableAutoSizing = true;
                handleLabel.fontSizeMin = 11f;
                handleLabel.fontSizeMax = portrait ? 16f : 14f;
            }
        }

        if (drawerContent != null)
        {
            Stretch(drawerContent);
            drawerContent.offsetMin = new Vector2(0f, 4f);
            drawerContent.offsetMax = new Vector2(0f, -4f);
        }

        if (navButtonsContainer == null)
            return;

        Stretch(navButtonsContainer);
        navButtonsContainer.offsetMin = new Vector2(4f, 0f);
        navButtonsContainer.offsetMax = new Vector2(-4f, 0f);
        HorizontalLayoutGroup group =
            navButtonsContainer.GetComponent<HorizontalLayoutGroup>();
        if (group != null)
        {
            group.padding = new RectOffset(0, 0, 0, 0);
            group.spacing = 6f;
            group.childAlignment = TextAnchor.MiddleCenter;
            group.childControlWidth = false;
            group.childControlHeight = false;
            group.childForceExpandWidth = false;
            group.childForceExpandHeight = false;
        }

        int count = CountVisibleNavigationButtons();
        float containerWidth = navButtonsContainer.rect.width;
        if (portrait && containerWidth <= 0f)
            containerWidth = 1016f;
        float available = Mathf.Max(0f, containerWidth -
            Mathf.Max(0, count - 1) * 6f);
        float width = count > 0
            ? Mathf.Clamp(available / count, portrait ? 86f : 90f,
                portrait ? 112f : 150f)
            : (portrait ? 104f : 140f);
        for (int index = 0; index < navButtonsContainer.childCount; index++)
        {
            RectTransform child = navButtonsContainer.GetChild(index) as RectTransform;
            if (child == null)
                continue;
            child.sizeDelta = new Vector2(width, portrait ? 72f : 34f);
            TMP_Text label = child.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.fontSize = portrait ? 16f : 14f;
                label.enableAutoSizing = true;
                label.fontSizeMin = 11f;
                label.fontSizeMax = portrait ? 16f : 14f;
            }
        }
    }

    private static void ConfigureDimensionContentClearance(
        RectTransform root, bool portrait)
    {
        if (root == null)
            return;

        VerticalSafeAreaLayout verticalLayout =
            root.GetComponentInParent<VerticalSafeAreaLayout>();
        if (verticalLayout != null &&
            verticalLayout.contentSlot != null &&
            root.parent == verticalLayout.contentSlot)
        {
            Vector2 verticalBottomLeft = root.offsetMin;
            verticalBottomLeft.y = 0f;
            root.offsetMin = verticalBottomLeft;
            return;
        }

        Vector2 bottomLeft = root.offsetMin;
        bottomLeft.y = portrait
            ? PortraitBottomContentClearance
            : BottomContentClearance;
        root.offsetMin = bottomLeft;
    }

    private int CountVisibleNavigationButtons()
    {
        if (navButtonsContainer == null)
            return 0;
        int count = 0;
        for (int index = 0; index < navButtonsContainer.childCount; index++)
        {
            if (navButtonsContainer.GetChild(index).gameObject.activeSelf)
                count++;
        }
        return count;
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
