using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class TriangleOfflineReportUI : MonoBehaviour
{
    private static bool displayedThisSession;
    private GameObject panel;
    private TextMeshProUGUI reportText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeReport()
    {
        if (FindFirstObjectByType<TriangleOfflineReportUI>() != null) return;
        GameObject root = new GameObject("TriangleOfflineReportRuntime");
        DontDestroyOnLoad(root);
        root.AddComponent<TriangleOfflineReportUI>().Build();
    }

    private void Build()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        panel = CreateRect("OfflineReportPanel", transform,
            new Vector2(640f, 420f), Vector2.zero);
        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.045f, 0.075f, 0.12f, 0.98f);
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.2f, 0.8f, 1f, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);

        TextMeshProUGUI title = CreateText("Title", panel.transform,
            new Vector2(570f, 55f), new Vector2(0f, 155f), 28f);
        title.text = "INFORME OFFLINE DEL TRIÁNGULO";
        title.alignment = TextAlignmentOptions.Center;

        reportText = CreateText("Report", panel.transform,
            new Vector2(550f, 245f), new Vector2(0f, 10f), 21f);
        reportText.alignment = TextAlignmentOptions.TopLeft;

        GameObject buttonObject = CreateRect("Close", panel.transform,
            new Vector2(210f, 52f), new Vector2(0f, -165f));
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.08f, 0.45f, 0.65f, 1f);
        Button closeButton = buttonObject.AddComponent<Button>();
        closeButton.onClick.AddListener(Close);
        TextMeshProUGUI closeText = CreateText("Label", buttonObject.transform,
            new Vector2(200f, 46f), Vector2.zero, 22f);
        closeText.text = "CONTINUAR";
        closeText.alignment = TextAlignmentOptions.Center;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (displayedThisSession || panel == null || GameState.I == null) return;
        TriangleOfflineReport report = GameState.I.lastTriangleOfflineReport;
        if (report == null || !report.hasResults || report.appliedSeconds <= 0.0)
            return;

        displayedThisSession = true;
        reportText.text = BuildReport(report);
        panel.SetActive(true);
    }

    private static string BuildReport(TriangleOfflineReport report)
    {
        string circuit = report.circuit == (int)TriangleCircuitType.Energy
            ? "Energía" : report.circuit == (int)TriangleCircuitType.Experimental
            ? "Experimental" : report.circuit == (int)TriangleCircuitType.Phase
            ? "Fase" : "Sin circuito";
        int hours = (int)(report.appliedSeconds / 3600.0);
        int minutes = (int)((report.appliedSeconds % 3600.0) / 60.0);
        int totalFragments = report.condensationGained + report.confinementGained +
            report.residualInterferenceGained;

        string value = "Tiempo simulado: " + hours + " h " + minutes + " min" +
            "\nCircuito utilizado: " + circuit +
            "\n\nLE obtenida: " + report.leGained.ToString("N0") +
            "\nTrazas obtenidas: " + report.tracesGained.ToString("N1") +
            "\nFragmentos obtenidos: " + totalFragments;
        if (report.phaseAnalysisSecondsApplied > report.appliedSeconds + 0.01)
        {
            double extra = report.phaseAnalysisSecondsApplied - report.appliedSeconds;
            value += "\nProgreso adicional de análisis por Fase: " +
                extra.ToString("N0") + " s";
        }
        return value;
    }

    private void Close()
    {
        if (panel != null) panel.SetActive(false);
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

    private static TextMeshProUGUI CreateText(
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
}
