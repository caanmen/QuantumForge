#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock1Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Block 1 Base")]
    public static void Validate()
    {
        var failures = new List<string>();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        Check(PlayerSettings.defaultInterfaceOrientation == UIOrientation.Portrait,
            "La orientacion principal no es Portrait.", failures);
        Check(PlayerSettings.allowedAutorotateToPortrait,
            "Portrait no esta permitido.", failures);
        Check(!PlayerSettings.allowedAutorotateToPortraitUpsideDown &&
            !PlayerSettings.allowedAutorotateToLandscapeLeft &&
            !PlayerSettings.allowedAutorotateToLandscapeRight,
            "Persisten rotaciones no aprobadas.", failures);
        Check(PlayerSettings.defaultScreenWidth == 1080 &&
            PlayerSettings.defaultScreenHeight == 1920,
            "La resolucion base no es 1080 x 1920.", failures);

        Canvas canvas = FindNamed<Canvas>(scene, "Canvas");
        Check(canvas != null, "Falta Canvas.", failures);
        CanvasScaler scaler = canvas != null ? canvas.GetComponent<CanvasScaler>() : null;
        Check(scaler != null &&
            scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize &&
            Approximately(scaler.referenceResolution, new Vector2(1080f, 1920f)) &&
            Mathf.Approximately(scaler.matchWidthOrHeight, 0.5f),
            "CanvasScaler no conserva la referencia vertical aprobada.", failures);

        Check(CountNamed(scene, "VerticalUIRoot") == 1,
            "VerticalUIRoot esta ausente o duplicado.", failures);
        Check(CountNamed(scene, "VerticalSafeAreaRoot") == 1,
            "VerticalSafeAreaRoot esta ausente o duplicado.", failures);

        VerticalUiSkinRoot skin = UnityEngine.Object.FindFirstObjectByType<VerticalUiSkinRoot>(
            FindObjectsInactive.Include);
        VerticalSafeAreaLayout layout =
            UnityEngine.Object.FindFirstObjectByType<VerticalSafeAreaLayout>(
                FindObjectsInactive.Include);
        Check(skin != null && skin.name == "VerticalUIRoot",
            "Falta el marcador del nuevo skin.", failures);
        Check(layout != null && layout.transform == skin?.transform,
            "Falta VerticalSafeAreaLayout en la raiz vertical.", failures);

        if (skin != null)
        {
            Check(skin.transform.parent != null && skin.transform.parent.name == "HUD",
                "VerticalUIRoot no depende directamente de HUD.", failures);
            bool navigationOwnsContent =
                skin.GetComponent<VerticalNavigationUI>() != null;
            Check(skin.transform.GetSiblingIndex() == 0 || navigationOwnsContent,
                "VerticalUIRoot tiene un orden incompatible con su contenido.", failures);
            Check(Dimension1DarkThemeRuntime.IsExcludedFromTheme(skin.transform),
                "El tema heredado puede recolorear la nueva UI.", failures);
        }

        if (layout != null)
        {
            CheckNamed(layout.safeAreaRoot, "VerticalSafeAreaRoot",
                "Safe Area", failures);
            CheckNamed(layout.headerSlot, "HeaderSlot", "cabecera", failures);
            CheckNamed(layout.contentSlot, "ContentSlot", "contenido", failures);
            CheckNamed(layout.secondaryNavigationSlot, "SecondaryNavigationSlot",
                "navegacion secundaria", failures);
            CheckNamed(layout.primaryNavigationSlot, "PrimaryNavigationSlot",
                "navegacion principal", failures);
            Check(layout.secondaryNavigationSlot != null &&
                !layout.secondaryNavigationSlot.gameObject.activeSelf,
                "La barra secundaria debe iniciar ausente.", failures);
            layout.ApplyLayout();
            Check(Mathf.Approximately(layout.horizontalMargin, 24f),
                "Las barras perdieron su margen horizontal canonico de 24 px.",
                failures);
            Check(Mathf.Approximately(layout.contentHorizontalMargin, 0f) &&
                Mathf.Approximately(layout.contentSlot.offsetMin.x, 0f) &&
                Mathf.Approximately(layout.contentSlot.offsetMax.x, 0f),
                "ContentSlot deja visible el fondo tecnico por los laterales.",
                failures);
            Check(Mathf.Approximately(layout.primaryNavigationSlot.sizeDelta.x, -48f),
                "La correccion del contenido altero el margen de navegacion.",
                failures);
        }

        ValidateTheme(skin != null ? skin.theme : null, failures);
        ValidateBackground(skin, failures);
        ValidateSafeAreaMath(failures);

        MobileQaFriendlyLayout legacy =
            UnityEngine.Object.FindFirstObjectByType<MobileQaFriendlyLayout>(
                FindObjectsInactive.Include);
        Check(legacy != null && legacy.forcePortraitLayout,
            "El adaptador heredado no esta fijado a portrait.", failures);

        Finish(failures);
    }

    private static void ValidateTheme(VerticalUiTheme theme, List<string> failures)
    {
        Check(theme != null, "Falta VerticalUiTheme.", failures);
        Check(theme == AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath),
            "La raiz no referencia el tema canonico.", failures);
        if (theme == null)
            return;
        Check(theme.backgroundGrid != null && theme.panelFrame != null &&
            theme.buttonFrame != null && theme.selectedButtonFrame != null &&
            theme.softGlow != null,
            "Faltan sprites modulares en VerticalUiTheme.", failures);
        Check(theme.primaryFont != null && theme.primaryFont is TMP_FontAsset,
            "Falta la fuente SDF compartida.", failures);
    }

    private static void ValidateBackground(
        VerticalUiSkinRoot skin, List<string> failures)
    {
        if (skin == null)
            return;
        Transform background = skin.transform.Find("Background");
        Transform grid = skin.transform.Find("BackgroundGrid");
        Image backgroundImage = background != null ? background.GetComponent<Image>() : null;
        Image gridImage = grid != null ? grid.GetComponent<Image>() : null;
        Check(backgroundImage != null && !backgroundImage.raycastTarget,
            "El fondo base falta o intercepta raycasts.", failures);
        Check(gridImage != null && !gridImage.raycastTarget &&
            gridImage.type == Image.Type.Tiled,
            "La reticula modular falta o intercepta raycasts.", failures);
        Check(grid != null && !grid.gameObject.activeSelf,
            "BackgroundGrid vuelve a quedar visible como borde tecnico.", failures);
    }

    private static void ValidateSafeAreaMath(List<string> failures)
    {
        ValidateSafeAreaCase(new Rect(0f, 0f, 1080f, 1920f), 1080, 1920,
            Vector2.zero, Vector2.one, "9:16", failures);
        ValidateSafeAreaCase(new Rect(0f, 84f, 1080f, 2172f), 1080, 2340,
            new Vector2(0f, 84f / 2340f), new Vector2(1f, 2256f / 2340f),
            "9:19.5", failures);
        ValidateSafeAreaCase(new Rect(12f, 64f, 696f, 1472f), 720, 1600,
            new Vector2(12f / 720f, 64f / 1600f),
            new Vector2(708f / 720f, 1536f / 1600f),
            "vertical estrecha", failures);
    }

    private static void ValidateSafeAreaCase(
        Rect safe,
        int width,
        int height,
        Vector2 expectedMin,
        Vector2 expectedMax,
        string label,
        List<string> failures)
    {
        VerticalSafeAreaLayout.CalculateSafeAnchors(
            safe, width, height, out Vector2 min, out Vector2 max);
        Check(Approximately(min, expectedMin) && Approximately(max, expectedMax),
            "Safe Area incorrecta para " + label + ".", failures);
    }

    private static T FindNamed<T>(Scene scene, string name) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (T component in root.GetComponentsInChildren<T>(true))
                if (component.name == name)
                    return component;
        }
        return null;
    }

    private static int CountNamed(Scene scene, string name)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    count++;
        return count;
    }

    private static void CheckNamed(
        RectTransform value, string expected, string label, List<string> failures)
    {
        Check(value != null && value.name == expected,
            "Falta la ranura de " + label + ".", failures);
    }

    private static bool Approximately(Vector2 left, Vector2 right)
    {
        return Mathf.Abs(left.x - right.x) < 0.0001f &&
            Mathf.Abs(left.y - right.y) < 0.0001f;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[Vertical UI Block 1] PASS | portrait | 1080x1920 | " +
                "Safe Area 9:16, 9:19.5 y estrecha | skin protegido | assets Android");
            return;
        }
        foreach (string failure in failures)
            Debug.LogError("[Vertical UI Block 1] " + failure);
        throw new InvalidOperationException(
            "Vertical UI Block 1 fallo con " + failures.Count + " error(es).");
    }
}
#endif
