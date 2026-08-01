using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public sealed class PresentationReturnReportUI : MonoBehaviour
{
    private GameObject _panel;
    private TMP_Text _title;
    private TMP_Text _body;
    private TMP_Text _closeLabel;
    private TMP_Text _openLabel;
    private PresentationReturnReport _report;
    private int _localizationRevision = -1;

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
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5100;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        _panel = CreateRect("ReturnPanel", transform,
            new Vector2(680f, 440f), Vector2.zero);
        Image background = _panel.AddComponent<Image>();
        background.color = new Color(0.045f, 0.075f, 0.12f, 0.98f);
        Outline outline = _panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.2f, 0.8f, 1f, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);

        _title = CreateText("Title", _panel.transform,
            new Vector2(610f, 58f), new Vector2(0f, 164f), 29f);
        _title.alignment = TextAlignmentOptions.Center;
        _body = CreateText("Body", _panel.transform,
            new Vector2(590f, 250f), new Vector2(0f, 15f), 21f);
        _body.alignment = TextAlignmentOptions.TopLeft;

        Button close = CreateButton("Close", _panel.transform,
            new Vector2(220f, 54f), new Vector2(-125f, -170f), out _closeLabel);
        close.onClick.AddListener(Close);
        Button open = CreateButton("OpenObjective", _panel.transform,
            new Vector2(250f, 54f), new Vector2(125f, -170f), out _openLabel);
        open.onClick.AddListener(OpenObjective);
        _panel.SetActive(false);
    }

    private void Update()
    {
        if (_panel == null) return;
        if (_report == null)
        {
            _report = PresentationReturnReportService.Consume();
            if (_report == null) return;
            _panel.SetActive(true);
            RefreshText();
        }
        int revision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision : -1;
        if (_panel.activeSelf && revision != _localizationRevision)
            RefreshText();
    }

    private void RefreshText()
    {
        if (_report == null) return;
        _localizationRevision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision : -1;
        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
        _title.text = PresentationTextCatalog.Get("return.title", english) +
            " · " + FormatDuration(_report.elapsedSeconds, english);
        var body = new StringBuilder();
        body.AppendFormat(PresentationTextCatalog.Get("return.applied", english),
            FormatDuration(_report.appliedSeconds, english));
        for (int i = 0; i < _report.newsKeys.Count; i++)
        {
            body.Append("\n\n• ");
            string template = PresentationTextCatalog.Get(
                _report.newsKeys[i], english);
            body.Append(_report.newsKeys[i] == "return.new"
                ? string.Format(template,
                    GetFeatureName(_report.newFeatureId, english))
                : template);
        }
        _body.text = body.ToString();
        _closeLabel.text = PresentationTextCatalog.Get("return.continue", english);
        _openLabel.text = PresentationTextCatalog.Get("return.open", english);
    }

    private void Close()
    {
        if (_panel != null) _panel.SetActive(false);
        _report = null;
    }

    private void OpenObjective()
    {
        if (_report == null || GameState.I == null) { Close(); return; }
        if (_report.targetDimension == 2 && TabsUI.Instance != null)
        {
            D2PresentationRouter.RememberScreen(
                GameState.I, _report.targetScreenId);
            TabsUI.Instance.ShowDimension2();
        }
        else if (_report.targetDimension == 3 && TabsUI.Instance != null)
        {
            D3PresentationRouter.RememberScreen(
                GameState.I, _report.targetScreenId);
            TabsUI.Instance.ShowDimension3();
        }
        Close();
    }

    private static string GetFeatureName(string id, bool english)
    {
        if (string.IsNullOrEmpty(id))
            return PresentationTextCatalog.Get("feature.unknown", english);
        string value = id.Replace("d2.", "").Replace("d3.", "")
            .Replace('_', ' ').Replace('.', ' ');
        return value;
    }

    private static string FormatDuration(double seconds, bool english)
    {
        int totalMinutes = Mathf.Max(0, Mathf.FloorToInt((float)seconds / 60f));
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return hours > 0
            ? hours + (english ? " h " : " h ") + minutes + " min"
            : minutes + " min";
    }

    private static GameObject CreateRect(
        string name, Transform parent, Vector2 size, Vector2 position)
    {
        GameObject value = new GameObject(name, typeof(RectTransform));
        value.transform.SetParent(parent, false);
        RectTransform rect = value.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return value;
    }

    private static TMP_Text CreateText(
        string name, Transform parent, Vector2 size, Vector2 position,
        float fontSize)
    {
        GameObject value = CreateRect(name, parent, size, position);
        TextMeshProUGUI text = value.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.color = Color.white;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private static Button CreateButton(
        string name, Transform parent, Vector2 size, Vector2 position,
        out TMP_Text label)
    {
        GameObject value = CreateRect(name, parent, size, position);
        Image image = value.AddComponent<Image>();
        image.color = new Color(0.08f, 0.45f, 0.65f, 1f);
        Button button = value.AddComponent<Button>();
        label = CreateText("Label", value.transform,
            size - new Vector2(10f, 8f), Vector2.zero, 21f);
        label.alignment = TextAlignmentOptions.Center;
        return button;
    }
}
