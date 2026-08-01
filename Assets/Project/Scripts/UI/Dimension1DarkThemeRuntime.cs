using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dimension1DarkThemeRuntime : MonoBehaviour
{
    private static readonly string[] RootNames =
    {
        "Canvas", "Panel_HUD", "HUD", "Panel_Generacion", "Panel_Lab", "Panel_Logros",
        "BottomDrawer", "Dimension1Panel", "QA_ToolsButton", "QA_PanelRoot",
        "Panel_AchievementPopup", "PrestigePanel", "MetaPrestigePanel"
    };

    private void Awake() => ApplyTheme();

    private IEnumerator Start()
    {
        // BuildingListUI, ResearchUI y AchievementListUI crean filas en Start.
        // Reaplicar al frame siguiente garantiza que esas instancias también
        // reciban el tema sin depender del orden de ejecución de Unity.
        yield return null;
        ApplyTheme();
    }

    [ContextMenu("Apply Dimension 1 dark theme")]
    public void ApplyTheme()
    {
        Camera mainCamera = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();
        if (mainCamera != null) mainCamera.backgroundColor = Dimension1DarkThemePalette.Background;
        foreach (string rootName in RootNames)
        {
            GameObject root = FindNamedObject(rootName);
            if (root != null) ApplyToRoot(root);
        }
    }

    private static GameObject FindNamedObject(string objectName)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == objectName) return transform.gameObject;
        return null;
    }

    private static void ApplyToRoot(GameObject root)
    {
        if (IsExcludedFromTheme(root.transform)) return;
        foreach (Image image in root.GetComponentsInChildren<Image>(true))
        {
            if (IsExcludedFromTheme(image.transform) || image.GetComponent<Button>() != null) continue;
            string n = image.name.ToLowerInvariant();
            if (n.Contains("fill")) image.color = Dimension1DarkThemePalette.Accent;
            else if (n.Contains("background") || n.Contains("viewport") || n.Contains("panel")) image.color = Dimension1DarkThemePalette.DeepSurface;
            else image.color = Dimension1DarkThemePalette.Panel;
        }
        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (IsExcludedFromTheme(text.transform)) continue;
            string n = text.name.ToLowerInvariant();
            text.color = n.Contains("cost") || n.Contains("description") || n.Contains("stats")
                ? Dimension1DarkThemePalette.SecondaryText : Dimension1DarkThemePalette.PrimaryText;
        }
        foreach (Button button in root.GetComponentsInChildren<Button>(true))
        {
            if (IsExcludedFromTheme(button.transform)) continue;
            ApplyButtonColors(button);
        }
    }

    public static void ApplyButtonColors(Button button, bool completed = false)
    {
        if (button == null) return;
        string n = button.name.ToLowerInvariant();
        bool reset = n.Contains("reset");
        bool qa = n.StartsWith("qa_") || n.Contains("dev");
        ColorBlock colors = button.colors;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        colors.normalColor = completed ? Dimension1DarkThemePalette.Completed : reset ? Dimension1DarkThemePalette.ResetNormal : qa ? Dimension1DarkThemePalette.QaNormal : Dimension1DarkThemePalette.ButtonNormal;
        colors.highlightedColor = reset ? Dimension1DarkThemePalette.ResetHighlighted : Dimension1DarkThemePalette.ButtonHighlighted;
        colors.pressedColor = Dimension1DarkThemePalette.ButtonPressed;
        colors.selectedColor = Dimension1DarkThemePalette.ButtonSelected;
        colors.disabledColor = completed ? Dimension1DarkThemePalette.Completed : Dimension1DarkThemePalette.ButtonDisabled;
        button.colors = colors;
        if (button.targetGraphic != null)
            button.targetGraphic.color = Color.white;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.color = completed ? Dimension1DarkThemePalette.Hex("DFFFEF") : Dimension1DarkThemePalette.Hex("F3F7FF");
    }

    public static bool IsExcludedFromTheme(Transform transform)
    {
        for (Transform current = transform; current != null; current = current.parent)
        {
            if (current.name == "Dimension2Panel" || current.name == "Dimension3Panel")
                return true;
            if (current.GetComponent<VerticalUiSkinRoot>() != null)
                return true;
        }
        return false;
    }
}
