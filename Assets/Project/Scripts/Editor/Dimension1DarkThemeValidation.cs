#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Dimension1DarkThemeValidation
{
    private struct RectSnapshot
    {
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector2 pivot;
        public Vector3 localScale;
    }

    [MenuItem("Quantum Forge/Validation/Dimension 1 Dark Theme")]
    public static void Validate()
    {
        EditorSceneManager.OpenScene("Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        var theme = Object.FindFirstObjectByType<Dimension1DarkThemeRuntime>(FindObjectsInactive.Include);
        if (theme == null) throw new System.Exception("Falta Dimension1DarkThemeRuntime en Main.");

        var layoutBefore = CaptureLayout();
        var protectedGraphics = CaptureProtectedGraphics();
        var protectedButtons = CaptureProtectedButtons();
        theme.ApplyTheme();

        ValidateLayoutUnchanged(layoutBefore);
        ValidateProtectedGraphics(protectedGraphics);
        ValidateProtectedButtons(protectedButtons);

        int checkedButtons = 0;
        foreach (Button button in Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!IsThemeScope(button.transform) || IsUnder(button.transform, "Dimension2Panel") || IsUnder(button.transform, "Dimension3Panel")) continue;
            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label == null) continue;
            float ratio = Contrast(label.color, button.colors.normalColor);
            if (ratio < 4.5f) throw new System.Exception($"Contraste insuficiente en {button.name}: {ratio:0.00}:1");
            checkedButtons++;
        }
        if (checkedButtons == 0) throw new System.Exception("No se validaron botones de Dimensión 1.");

        int checkedTexts = 0;
        foreach (TMP_Text label in Object.FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!IsThemeScope(label.transform) || IsProtected(label.transform)) continue;
            Color background = FindEffectiveBackground(label.transform);
            float ratio = Contrast(label.color, background);
            if (ratio < 4.5f)
                throw new System.Exception(
                    $"Contraste insuficiente en texto {label.name}: {ratio:0.00}:1");
            checkedTexts++;
        }
        if (checkedTexts == 0) throw new System.Exception("No se validaron textos de Dimensión 1.");

        Debug.Log(
            $"[Dimension1DarkThemeValidation] PASS · botones={checkedButtons} · " +
            $"textos={checkedTexts} · layout intacto · D2/D3 sin cambios.");
    }

    private static Dictionary<int, RectSnapshot> CaptureLayout()
    {
        var result = new Dictionary<int, RectSnapshot>();
        foreach (RectTransform rect in Object.FindObjectsByType<RectTransform>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            result[rect.GetInstanceID()] = new RectSnapshot
            {
                anchorMin = rect.anchorMin,
                anchorMax = rect.anchorMax,
                anchoredPosition = rect.anchoredPosition,
                sizeDelta = rect.sizeDelta,
                pivot = rect.pivot,
                localScale = rect.localScale
            };
        }
        return result;
    }

    private static void ValidateLayoutUnchanged(Dictionary<int, RectSnapshot> before)
    {
        RectTransform[] current = Object.FindObjectsByType<RectTransform>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (current.Length != before.Count)
            throw new System.Exception("El tema creó o eliminó RectTransforms.");
        foreach (RectTransform rect in current)
        {
            if (!before.TryGetValue(rect.GetInstanceID(), out RectSnapshot old))
                throw new System.Exception($"RectTransform nuevo durante tema: {rect.name}");
            if (rect.anchorMin != old.anchorMin || rect.anchorMax != old.anchorMax ||
                rect.anchoredPosition != old.anchoredPosition || rect.sizeDelta != old.sizeDelta ||
                rect.pivot != old.pivot || rect.localScale != old.localScale)
                throw new System.Exception($"El tema alteró el layout de {rect.name}.");
        }
    }

    private static Dictionary<int, Color> CaptureProtectedGraphics()
    {
        var result = new Dictionary<int, Color>();
        foreach (Graphic graphic in Object.FindObjectsByType<Graphic>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (IsProtected(graphic.transform))
                result[graphic.GetInstanceID()] = graphic.color;
        return result;
    }

    private static void ValidateProtectedGraphics(Dictionary<int, Color> before)
    {
        foreach (Graphic graphic in Object.FindObjectsByType<Graphic>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (before.TryGetValue(graphic.GetInstanceID(), out Color old) &&
                !Near(graphic.color, old))
                throw new System.Exception($"El tema alteró D2/D3: {graphic.name}.");
    }

    private static Dictionary<int, ColorBlock> CaptureProtectedButtons()
    {
        var result = new Dictionary<int, ColorBlock>();
        foreach (Button button in Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (IsProtected(button.transform))
                result[button.GetInstanceID()] = button.colors;
        return result;
    }

    private static void ValidateProtectedButtons(Dictionary<int, ColorBlock> before)
    {
        foreach (Button button in Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!before.TryGetValue(button.GetInstanceID(), out ColorBlock old)) continue;
            ColorBlock now = button.colors;
            if (!Near(now.normalColor, old.normalColor) ||
                !Near(now.highlightedColor, old.highlightedColor) ||
                !Near(now.pressedColor, old.pressedColor) ||
                !Near(now.selectedColor, old.selectedColor) ||
                !Near(now.disabledColor, old.disabledColor))
                throw new System.Exception($"El tema alteró un botón D2/D3: {button.name}.");
        }
    }

    private static Color FindEffectiveBackground(Transform transform)
    {
        Button button = transform.GetComponentInParent<Button>(true);
        if (button != null && !IsProtected(button.transform))
            return button.colors.normalColor;

        for (Transform current = transform.parent; current != null; current = current.parent)
        {
            Image image = current.GetComponent<Image>();
            if (image == null) continue;
            return Composite(image.color, Dimension1DarkThemePalette.Background);
        }
        return Dimension1DarkThemePalette.Background;
    }

    private static Color Composite(Color foreground, Color background)
    {
        float a = Mathf.Clamp01(foreground.a);
        return new Color(
            foreground.r * a + background.r * (1f - a),
            foreground.g * a + background.g * (1f - a),
            foreground.b * a + background.b * (1f - a),
            1f);
    }

    private static bool IsThemeScope(Transform t)
    {
        string[] roots = { "Canvas", "Panel_HUD", "HUD", "Panel_Generacion", "Panel_Lab", "Panel_Logros", "BottomDrawer", "Dimension1Panel", "QA_ToolsButton", "QA_PanelRoot", "Panel_AchievementPopup", "PrestigePanel", "MetaPrestigePanel" };
        for (; t != null; t = t.parent)
            foreach (string root in roots) if (t.name == root) return true;
        return false;
    }

    private static bool IsUnder(Transform t, string name)
    {
        for (; t != null; t = t.parent) if (t.name == name) return true;
        return false;
    }

    private static bool IsProtected(Transform t) =>
        IsUnder(t, "Dimension2Panel") || IsUnder(t, "Dimension3Panel");

    private static bool Near(Color a, Color b) =>
        Mathf.Abs(a.r - b.r) < 0.0001f && Mathf.Abs(a.g - b.g) < 0.0001f &&
        Mathf.Abs(a.b - b.b) < 0.0001f && Mathf.Abs(a.a - b.a) < 0.0001f;

    private static float Contrast(Color a, Color b)
    {
        float la = Luminance(a), lb = Luminance(b);
        return (Mathf.Max(la, lb) + 0.05f) / (Mathf.Min(la, lb) + 0.05f);
    }

    private static float Luminance(Color c) => 0.2126f * Linear(c.r) + 0.7152f * Linear(c.g) + 0.0722f * Linear(c.b);
    private static float Linear(float c) => c <= 0.03928f ? c / 12.92f : Mathf.Pow((c + 0.055f) / 1.055f, 2.4f);
}
#endif
