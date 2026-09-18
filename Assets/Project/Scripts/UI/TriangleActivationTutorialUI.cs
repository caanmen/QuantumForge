using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class TriangleActivationTutorialUI : MonoBehaviour
{
    private static TriangleActivationTutorialUI instance;
    private CanvasGroup canvasGroup;

    public static void ShowAfterUnlock()
    {
        if (GameState.I == null || !GameState.I.triangleSystemUnlocked ||
            GameState.I.triangleActivationTutorialSeen)
            return;

        if (instance == null)
        {
            GameObject root = new("TriangleActivationTutorial",
                typeof(RectTransform));
            instance = root.AddComponent<TriangleActivationTutorialUI>();
            instance.Build();
        }

        GameState.I.triangleActivationTutorialSeen = true;
        SaveService.I?.Save();
        instance.gameObject.SetActive(true);
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.Fade(0f, 1f, 0.20f, false));
    }

    private void Build()
    {
        VerticalUiSkinRoot skin = FindFirstObjectByType<VerticalUiSkinRoot>(
            FindObjectsInactive.Include);
        VerticalUiTheme theme = skin != null ? skin.theme : null;

        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5200;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        gameObject.AddComponent<GraphicRaycaster>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        Image shade = CreateStretch("Shade", transform,
            new Color(0f, 0.008f, 0.018f, 0.84f));
        shade.raycastTarget = true;

        GameObject panel = CreateRect("Panel", transform,
            new Vector2(900f, 760f), new Vector2(0f, 40f));
        Image panelImage = panel.AddComponent<Image>();
        panelImage.sprite = theme != null ? theme.panelFrame : null;
        panelImage.type = panelImage.sprite != null
            ? Image.Type.Sliced : Image.Type.Simple;
        panelImage.color = theme != null
            ? theme.panel : new Color(0.018f, 0.071f, 0.102f, 1f);
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = theme != null
            ? theme.triangle : new Color(1f, 0.595f, 0.035f, 1f);
        outline.effectDistance = new Vector2(2f, -2f);

        Color primary = theme != null ? theme.primaryText : Color.white;
        Color secondary = theme != null
            ? theme.secondaryText : new Color(.65f, .75f, .84f, 1f);
        Color accent = theme != null
            ? theme.triangle : new Color(1f, .595f, .035f, 1f);
        TMP_FontAsset font = theme != null && theme.primaryFont != null
            ? theme.primaryFont : TMP_Settings.defaultFontAsset;
        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;

        CreateText("Title", panel.transform,
            english ? "TRIANGLE ACTIVE" : "TRIÁNGULO ACTIVO",
            new Vector2(780f, 82f), new Vector2(0f, 270f), 40f,
            FontStyles.Bold, accent, font, TextAlignmentOptions.Center);
        CreateText("Body", panel.transform, english
            ? "The Triangle connects Higgs, Tetraquark, and the Phase Modulator.\n\n" +
              "Choose a circuit and maintain its synchronization: the higher it is, " +
              "the stronger its effect. Switching circuits temporarily reduces synchronization."
            : "El Triángulo conecta Higgs, Tetraquark y el Modulador de Fase.\n\n" +
              "Elige un circuito y mantén su sincronización: cuanto mayor sea, " +
              "mayor será su efecto. Cambiar de circuito reduce temporalmente " +
              "la sincronización.",
            new Vector2(750f, 390f), new Vector2(0f, 25f), 29f,
            FontStyles.Normal, primary, font, TextAlignmentOptions.Center);
        CreateText("Hint", panel.transform,
            english
                ? "You can switch circuits from this Generation screen."
                : "Puedes cambiar el circuito desde esta pantalla de Generación.",
            new Vector2(720f, 64f), new Vector2(0f, -190f), 22f,
            FontStyles.Normal, secondary, font, TextAlignmentOptions.Center);

        Button close = CreateButton("Close", panel.transform,
            new Vector2(580f, 88f), new Vector2(0f, -292f),
            theme, accent, font, english ? "GOT IT" : "ENTENDIDO");
        close.onClick.AddListener(Close);
    }

    private void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(canvasGroup != null ? canvasGroup.alpha : 1f,
            0f, 0.16f, true));
    }

    private IEnumerator Fade(float from, float to, float duration, bool destroy)
    {
        canvasGroup.blocksRaycasts = to > 0f;
        canvasGroup.interactable = to > 0f;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to,
                duration > 0f ? time / duration : 1f);
            yield return null;
        }
        canvasGroup.alpha = to;
        if (destroy) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private static Image CreateStretch(string name, Transform parent, Color color)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static GameObject CreateRect(
        string name, Transform parent, Vector2 size, Vector2 position)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(CanvasRenderer));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return go;
    }

    private static TextMeshProUGUI CreateText(
        string name, Transform parent, string value, Vector2 size,
        Vector2 position, float fontSize, FontStyles style, Color color,
        TMP_FontAsset font, TextAlignmentOptions alignment)
    {
        GameObject go = CreateRect(name, parent, size, position);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = font;
        text.fontSize = fontSize;
        text.fontSizeMax = fontSize;
        text.fontSizeMin = Mathf.Max(18f, fontSize - 7f);
        text.enableAutoSizing = true;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }

    private static Button CreateButton(
        string name, Transform parent, Vector2 size, Vector2 position,
        VerticalUiTheme theme, Color accent, TMP_FontAsset font, string label)
    {
        GameObject go = CreateRect(name, parent, size, position);
        Image image = go.AddComponent<Image>();
        image.sprite = theme != null ? theme.buttonFrame : null;
        image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = new Color(.025f, .055f, .072f, 1f);
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = accent;
        outline.effectDistance = new Vector2(2f, -2f);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        CreateText("Label", go.transform, label, size, Vector2.zero,
            27f, FontStyles.Bold, accent, font, TextAlignmentOptions.Center);
        return button;
    }
}
