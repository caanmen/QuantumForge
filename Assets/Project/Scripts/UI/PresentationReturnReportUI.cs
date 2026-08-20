using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class PresentationReturnReportUI : MonoBehaviour
{
    private static readonly Color FallbackBackground =
        new Color(0.008f, 0.027f, 0.043f, 0.985f);
    private static readonly Color FallbackPanel =
        new Color(0.018f, 0.071f, 0.102f, 0.98f);
    private static readonly Color FallbackBorder =
        new Color(0.075f, 0.365f, 0.56f, 0.92f);
    private static readonly Color FallbackPrimary =
        new Color(0.925f, 0.957f, 1f, 1f);
    private static readonly Color FallbackSecondary =
        new Color(0.6f, 0.71f, 0.82f, 1f);
    private static readonly Color FallbackCyan =
        new Color(0f, 0.835f, 1f, 1f);
    private static readonly Color FallbackViolet =
        new Color(0.725f, 0.31f, 0.925f, 1f);
    private static readonly Color FallbackAmber =
        new Color(1f, 0.595f, 0.035f, 1f);

    private GameObject _modalRoot;
    private TMP_Text _title;
    private TMP_Text _duration;
    private TMP_Text _balanceTitle;
    private TMP_Text _leBalance;
    private TMP_Text _tracesBalance;
    private TMP_Text _energyBalance;
    private TMP_Text _noBalance;
    private TMP_Text _continueLabel;
    private GameObject _leChip;
    private GameObject _tracesChip;
    private GameObject _energyChip;
    private PresentationReturnReport _report;
    private VerticalUiTheme _theme;
    private int _localizationRevision = -1;
    private bool _subscribedToReportService;

    private Color Background => _theme != null ? _theme.background : FallbackBackground;
    private Color Panel => _theme != null ? _theme.panel : FallbackPanel;
    private Color Border => _theme != null ? _theme.border : FallbackBorder;
    private Color Primary => _theme != null ? _theme.primaryText : FallbackPrimary;
    private Color Secondary => _theme != null ? _theme.secondaryText : FallbackSecondary;
    private Color Cyan => _theme != null ? _theme.energy : FallbackCyan;
    private Color Violet => _theme != null ? _theme.traces : FallbackViolet;
    private Color Amber => _theme != null ? _theme.triangle : FallbackAmber;
    private TMP_FontAsset Font => _theme != null ? _theme.primaryFont : null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeReport()
    {
        if (FindFirstObjectByType<PresentationReturnReportUI>() != null) return;
        GameObject root = new GameObject("PresentationReturnReportRuntime");
        DontDestroyOnLoad(root);
        root.AddComponent<PresentationReturnReportUI>().Build();
    }

    private void Build()
    {
        VerticalUiSkinRoot skin = FindFirstObjectByType<VerticalUiSkinRoot>(
            FindObjectsInactive.Include);
        _theme = skin != null ? skin.theme : null;

        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5100;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        gameObject.AddComponent<GraphicRaycaster>();

        _modalRoot = CreateStretch("ReturnModal", transform);
        Image shade = _modalRoot.AddComponent<Image>();
        shade.color = new Color(0f, 0.008f, 0.018f, 0.78f);
        shade.raycastTarget = true;

        GameObject glow = CreateRect("PanelGlow", _modalRoot.transform,
            new Vector2(930f, 740f), Vector2.zero);
        Image glowImage = glow.AddComponent<Image>();
        ApplyFrame(glowImage, Cyan.WithAlpha(0.12f));
        glowImage.raycastTarget = false;

        GameObject panelObject = CreateRect("ReturnPanel", _modalRoot.transform,
            new Vector2(900f, 710f), Vector2.zero);
        Image panelImage = panelObject.AddComponent<Image>();
        ApplyFrame(panelImage, Background);
        Outline panelOutline = panelObject.AddComponent<Outline>();
        panelOutline.effectColor = Cyan.WithAlpha(0.74f);
        panelOutline.effectDistance = new Vector2(2f, -2f);

        CreateAccentCap(panelObject.transform, new Vector2(0f, 337f));

        _title = CreateText("Title", panelObject.transform,
            new Vector2(760f, 58f), new Vector2(0f, 278f), 40f,
            FontStyles.Bold, Primary, TextAlignmentOptions.Center);
        _duration = CreateText("Duration", panelObject.transform,
            new Vector2(790f, 102f), new Vector2(0f, 195f), 36f,
            FontStyles.Bold, Cyan, TextAlignmentOptions.Center);
        CreateLine(panelObject.transform, new Vector2(0f, 133f),
            new Vector2(750f, 2f), Border);

        _balanceTitle = CreateText("BalanceTitle", panelObject.transform,
            new Vector2(640f, 38f), new Vector2(0f, 92f), 25f,
            FontStyles.Bold, Cyan, TextAlignmentOptions.Center);
        CreateLine(panelObject.transform, new Vector2(-330f, 92f),
            new Vector2(110f, 2f), Border);
        CreateLine(panelObject.transform, new Vector2(330f, 92f),
            new Vector2(110f, 2f), Border);

        _leChip = CreateBalanceChip(panelObject.transform, "LeBalance", Cyan,
            out _leBalance);
        _tracesChip = CreateBalanceChip(panelObject.transform, "TracesBalance", Violet,
            out _tracesBalance);
        _energyChip = CreateBalanceChip(panelObject.transform, "EnergyBalance", Amber,
            out _energyBalance);
        _noBalance = CreateText("NoBalance", panelObject.transform,
            new Vector2(760f, 90f), new Vector2(0f, -20f), 25f,
            FontStyles.Normal, Secondary, TextAlignmentOptions.Center);

        Button close = CreateButton("Continue", panelObject.transform,
            new Vector2(700f, 86f), new Vector2(0f, -268f), out _continueLabel);
        close.navigation = new Navigation { mode = Navigation.Mode.None };
        close.onClick.AddListener(Close);
        _modalRoot.SetActive(false);
        SubscribeToReportService();
    }

    private void Update()
    {
        if (_modalRoot == null) return;
        TryShowPendingReport();

        int revision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision : -1;
        if (_modalRoot.activeSelf && revision != _localizationRevision)
            RefreshText();
    }

    private void SubscribeToReportService()
    {
        if (_subscribedToReportService) return;
        PresentationReturnReportService.ReportPrepared += TryShowPendingReport;
        _subscribedToReportService = true;
        TryShowPendingReport();
    }

    private void OnDestroy()
    {
        if (!_subscribedToReportService) return;
        PresentationReturnReportService.ReportPrepared -= TryShowPendingReport;
        _subscribedToReportService = false;
    }

    private void TryShowPendingReport()
    {
        if (_modalRoot == null || _report != null) return;
        PresentationReturnReport pending = PresentationReturnReportService.Consume();
        if (pending == null) return;

        _report = pending;
        _modalRoot.transform.SetAsLastSibling();
        _modalRoot.SetActive(true);
        RefreshText();
    }

    private void RefreshText()
    {
        if (_report == null) return;
        _localizationRevision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision : -1;
        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;

        _title.text = PresentationTextCatalog.Get("return.title", english);
        string elapsed = FormatDuration(_report.elapsedSeconds, english, true);
        string applied = FormatDuration(_report.appliedSeconds, english, true);
        _duration.text = string.Format(
                PresentationTextCatalog.Get("return.away_time", english), elapsed) +
            "\n" + string.Format(
                PresentationTextCatalog.Get("return.applied_time", english), applied);
        _balanceTitle.text = PresentationTextCatalog.Get("return.balance", english);
        _continueLabel.text = PresentationTextCatalog.Get("return.continue", english);
        _noBalance.text = PresentationTextCatalog.Get("return.balance.none", english);

        SetBalanceChip(_leChip, _leBalance, _report.leDelta, "LE");
        SetBalanceChip(_tracesChip, _tracesBalance,
            _report.tracesDelta, english ? "TRACES" : "TRAZAS");
        SetBalanceChip(_energyChip, _energyBalance,
            _report.triangleEnergyDelta, english ? "ENERGY" : "ENERGÍA");
        RelayoutBalanceChips();
    }

    private void SetBalanceChip(
        GameObject chip, TMP_Text label, double value, string currency)
    {
        bool visible = Math.Abs(value) >= 0.0005;
        chip.SetActive(visible);
        if (visible) label.text = currency + "\n" + FormatDelta(value);
    }

    private void RelayoutBalanceChips()
    {
        var active = new List<GameObject>();
        if (_leChip.activeSelf) active.Add(_leChip);
        if (_tracesChip.activeSelf) active.Add(_tracesChip);
        if (_energyChip.activeSelf) active.Add(_energyChip);
        _noBalance.gameObject.SetActive(active.Count == 0);
        if (active.Count == 0) return;

        float available = 780f;
        float gap = 14f;
        float width = (available - gap * (active.Count - 1)) / active.Count;
        float total = width * active.Count + gap * (active.Count - 1);
        float left = -total * 0.5f + width * 0.5f;
        for (int i = 0; i < active.Count; i++)
        {
            RectTransform rect = active[i].GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, 132f);
            rect.anchoredPosition = new Vector2(left + i * (width + gap), -20f);
        }
    }

    private void Close()
    {
        if (_modalRoot != null) _modalRoot.SetActive(false);
        _report = null;
    }

    private GameObject CreateBalanceChip(Transform parent, string name,
        Color accent, out TMP_Text label)
    {
        GameObject chip = CreateRect(name, parent,
            new Vector2(250f, 132f), new Vector2(0f, -20f));
        Image background = chip.AddComponent<Image>();
        ApplyFrame(background, Panel);
        Outline outline = chip.AddComponent<Outline>();
        outline.effectColor = accent.WithAlpha(0.72f);
        outline.effectDistance = new Vector2(1f, -1f);
        CreateLine(chip.transform, new Vector2(0f, 64f),
            new Vector2(90f, 3f), accent);
        label = CreateText("Value", chip.transform, new Vector2(230f, 108f),
            new Vector2(0f, -2f), 28f, FontStyles.Bold, Primary,
            TextAlignmentOptions.Center);
        label.enableAutoSizing = true;
        label.fontSizeMin = 18f;
        label.fontSizeMax = 28f;
        return chip;
    }

    private Button CreateButton(string name, Transform parent, Vector2 size,
        Vector2 position, out TMP_Text label)
    {
        GameObject value = CreateRect(name, parent, size, position);
        Image image = value.AddComponent<Image>();
        Sprite sprite = _theme != null ? _theme.selectedButtonFrame : null;
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = sprite != null ? Color.white : Panel;
        Outline outline = value.AddComponent<Outline>();
        outline.effectColor = Cyan.WithAlpha(0.95f);
        outline.effectDistance = new Vector2(2f, -2f);
        Button button = value.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.72f, 0.96f, 1f, 1f);
        colors.pressedColor = new Color(0.52f, 0.76f, 0.86f, 1f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        label = CreateText("Label", value.transform,
            size - new Vector2(24f, 12f), Vector2.zero, 31f,
            FontStyles.Bold, Primary, TextAlignmentOptions.Center);
        return button;
    }

    private void ApplyFrame(Image image, Color color)
    {
        Sprite sprite = _theme != null ? _theme.panelFrame : null;
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
    }

    private void CreateAccentCap(Transform parent, Vector2 position)
    {
        CreateLine(parent, position, new Vector2(150f, 5f), Cyan);
        CreateLine(parent, position + new Vector2(0f, -8f),
            new Vector2(82f, 2f), Cyan.WithAlpha(0.45f));
    }

    private static void CreateLine(
        Transform parent, Vector2 position, Vector2 size, Color color)
    {
        GameObject line = CreateRect("Accent", parent, size, position);
        Image image = line.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
    }

    private TMP_Text CreateText(string name, Transform parent, Vector2 size,
        Vector2 position, float fontSize, FontStyles style, Color color,
        TextAlignmentOptions alignment)
    {
        GameObject value = CreateRect(name, parent, size, position);
        TextMeshProUGUI text = value.AddComponent<TextMeshProUGUI>();
        text.font = Font;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private static string FormatDuration(
        double seconds, bool english, bool upper)
    {
        int totalMinutes = Mathf.Max(0, Mathf.FloorToInt((float)seconds / 60f));
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        string value = hours > 0
            ? hours + " h " + minutes + " min"
            : minutes + " min";
        return upper ? value.ToUpperInvariant() : value;
    }

    private static string FormatDelta(double value)
    {
        string sign = value >= 0.0 ? "+" : "−";
        return sign + FormatCompact(Math.Abs(value));
    }

    private static string FormatCompact(double value)
    {
        if (value >= 1000000000.0) return (value / 1000000000.0).ToString("0.##") + "B";
        if (value >= 1000000.0) return (value / 1000000.0).ToString("0.##") + "M";
        if (value >= 1000.0) return (value / 1000.0).ToString("0.##") + "K";
        return value.ToString(value >= 100.0 ? "0" : "0.##");
    }

    private static GameObject CreateStretch(string name, Transform parent)
    {
        GameObject value = new GameObject(name, typeof(RectTransform));
        value.layer = 5;
        value.transform.SetParent(parent, false);
        RectTransform rect = value.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return value;
    }

    private static GameObject CreateRect(
        string name, Transform parent, Vector2 size, Vector2 position)
    {
        GameObject value = new GameObject(name, typeof(RectTransform));
        value.layer = 5;
        value.transform.SetParent(parent, false);
        RectTransform rect = value.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return value;
    }
}

internal static class PresentationReturnColorExtensions
{
    public static Color WithAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}
