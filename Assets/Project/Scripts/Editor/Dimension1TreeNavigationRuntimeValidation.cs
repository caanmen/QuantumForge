#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1TreeNavigationRuntimeValidation
{
    private const string ActiveKey = "QF.D1TreeNavigationValidation.Active";
    private const string FrameKey = "QF.D1TreeNavigationValidation.Frame";
    private const string StageKey = "QF.D1TreeNavigationValidation.Stage";
    private const string FailureKey = "QF.D1TreeNavigationValidation.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static readonly Dictionary<string, Vector2[]> StabilityCorners =
        new Dictionary<string, Vector2[]>();
    private static float stabilityStartTime;

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (!EditorApplication.isPlaying) return;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailureKey, false);
        SessionState.SetInt(FrameKey, 0);
        SessionState.SetInt(StageKey, 0);
        StabilityCorners.Clear();
        stabilityStartTime = 0f;
        SaveService.SuppressWritesForVisualQa = true;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            SaveService.SuppressWritesForVisualQa = true;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SaveService.SuppressWritesForVisualQa = false;
            SessionState.SetBool(ActiveKey, false);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            bool failed = SessionState.GetBool(FailureKey, false);
            Debug.Log(failed
                ? "[D1 Tree Navigation Runtime] FAIL"
                : "[D1 Tree Navigation Runtime] PASS | raycast físico | Centro, Galaxia, Explorar, Hangar y Reliquias | layout estable 10 s");
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame == 2)
                Screen.SetResolution(1080, 1920, false);
            if (frame == 20)
            {
                PrepareBatchViewport();
                TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance :
                    UnityEngine.Object.FindFirstObjectByType<TabsUI>(FindObjectsInactive.Include);
                if (tabs != null) tabs.ShowDimension1();
                PrepareCommandCenter();
                SessionState.SetInt(StageKey, 0);
            }
            if (frame < 30 || (frame - 30) % 12 != 0) return;

            int stage = SessionState.GetInt(StageKey, 0);
            if (!ExecuteStage(stage)) return;
            SessionState.SetInt(StageKey, stage + 1);
            if (stage < 11) return;

            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SessionState.SetBool(FailureKey, true);
            EditorApplication.update -= Tick;
            EditorApplication.isPlaying = false;
        }
    }

    private static void PrepareBatchViewport()
    {
        // El Game View batch de Unity permanece en 640x480 aunque Screen.SetResolution
        // solicite una salida vertical. Para que el raycast siga siendo físico y no
        // apunte fuera del viewport, la prueba ajusta únicamente el escalado temporal
        // del Play Mode; la escena no se guarda con este valor.
        if (Screen.width < Screen.height) return;
        foreach (CanvasScaler scaler in UnityEngine.Object.FindObjectsByType<CanvasScaler>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                scaler.matchWidthOrHeight = 1f;
        }
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.name == "ReturnModal") transform.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
    }

    private static bool ExecuteStage(int stage)
    {
        switch (stage)
        {
            case 0:
                Click("D1CommandCenterProductionRoot", "Nav_ÁRBOL");
                break;
            case 1:
                ValidateTreeVisible();
                Click("D1_TreeVisualRoot", "CommandCenter");
                break;
            case 2:
                ValidateTreeClosed("Centro de Mando");
                OpenTreeByRealClick();
                break;
            case 3:
                ValidateTreeVisible();
                Click("D1_TreeVisualRoot", "Nav_GALAXIA");
                break;
            case 4:
                ValidateTarget("GalaxyPanel", "Galaxia");
                Panel().OnClickCloseGalaxyPanel();
                OpenTreeByRealClick();
                break;
            case 5:
                ValidateTreeVisible();
                Click("D1_TreeVisualRoot", "Nav_EXPLORAR");
                break;
            case 6:
                ValidateTarget("D1_ExploreVisualRoot", "Explorar");
                CommandCenter().ShowCommandCenterScreen();
                OpenTreeByRealClick();
                break;
            case 7:
                ValidateTreeVisible();
                Click("D1_TreeVisualRoot", "Nav_HANGAR");
                break;
            case 8:
                ValidateTarget("HangarPanel", "Hangar");
                Panel().OnClickCloseHangarPanel();
                OpenTreeByRealClick();
                break;
            case 9:
                ValidateTreeVisible();
                Click("D1_TreeVisualRoot", "Nav_RELIQUIAS");
                break;
            case 10:
                ValidateTarget("RelicChamberPanel", "Reliquias");
                break;
            case 11:
                if (StabilityCorners.Count == 0)
                {
                    Panel().OnClickCloseRelicChamberPanel();
                    CommandCenter().ShowExploreScreen();
                    ValidateTarget("D1_ExploreVisualRoot", "Explorar");
                    BeginStabilityValidation();
                }
                return ValidateStabilityFrame();
        }

        return true;
    }

    private static void PrepareCommandCenter()
    {
        Dimension1PanelUI panel = Panel();
        panel.OnClickCloseDimension1TreePanel();
        panel.OnClickCloseGalaxyPanel();
        panel.OnClickCloseHangarPanel();
        panel.OnClickCloseRelicChamberPanel();
        CommandCenter().ShowCommandCenterScreen();
    }

    private static void OpenTreeByRealClick()
    {
        PrepareCommandCenter();
        Click("D1CommandCenterProductionRoot", "Nav_ÁRBOL");
    }

    private static void Click(string rootName, string name)
    {
        Transform root = FindSceneTransform(rootName);
        Transform target = FindChild(root, name);
        Button button = target != null ? target.GetComponent<Button>() : null;
        if (button == null || !button.interactable)
            throw new InvalidOperationException("No se puede pulsar " + name + ".");
        EventSystem eventSystem = EventSystem.current != null
            ? EventSystem.current
            : UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (eventSystem == null) throw new InvalidOperationException("No existe EventSystem para el clic real.");
        Vector2 screenCenter = ButtonScreenCenter(button);
        var pointer = new PointerEventData(eventSystem)
        {
            button = PointerEventData.InputButton.Left,
            pointerId = -1,
            position = screenCenter
        };
        var raycasts = new List<RaycastResult>();
        eventSystem.RaycastAll(pointer, raycasts);
        if (raycasts.Count == 0)
        {
            Canvas canvas = button.GetComponentInParent<Canvas>();
            Graphic targetGraphic = button.targetGraphic;
            throw new InvalidOperationException(
                "El raycast real no alcanzó " + name +
                ". Screen=" + Screen.width + "x" + Screen.height +
                ", punto=" + screenCenter.ToString("F1") +
                ", objetivo=" + (targetGraphic != null ? targetGraphic.name : "ninguno") +
                ", raycast=" + (targetGraphic != null && targetGraphic.raycastTarget) +
                ", canvas=" + (canvas != null ? canvas.renderMode.ToString() : "ninguno") + ".");
        }

        GameObject firstHit = raycasts[0].gameObject;
        GameObject clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(firstHit);
        if (clickHandler != button.gameObject)
            throw new InvalidOperationException(
                "El clic sobre " + name + " fue interceptado por " +
                firstHit.name + " (handler: " +
                (clickHandler != null ? clickHandler.name : "ninguno") + ").");

        eventSystem.SetSelectedGameObject(button.gameObject, pointer);
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(firstHit, pointer, ExecuteEvents.pointerUpHandler);
        if (ExecuteEvents.ExecuteHierarchy(firstHit, pointer,
            ExecuteEvents.pointerClickHandler) == null)
            throw new InvalidOperationException("UGUI no procesó el clic sobre " + name + ".");
    }

    private static Vector2 ButtonScreenCenter(Button button)
    {
        RectTransform rect = button.transform as RectTransform;
        if (rect == null)
            throw new InvalidOperationException("El botón no tiene RectTransform.");
        Canvas canvas = button.GetComponentInParent<Canvas>();
        Canvas rootCanvas = canvas != null ? canvas.rootCanvas : null;
        Camera camera = rootCanvas != null &&
            rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? rootCanvas.worldCamera
            : null;
        return RectTransformUtility.WorldToScreenPoint(camera,
            rect.TransformPoint(rect.rect.center));
    }

    private static void BeginStabilityValidation()
    {
        StabilityCorners.Clear();
        string[] names = { "VerticalSafeAreaRoot", "ContentSlot", "D1_ExploreVisualRoot" };
        foreach (string name in names)
        {
            RectTransform rect = FindSceneTransform(name) as RectTransform;
            if (rect == null || !rect.gameObject.activeInHierarchy)
                throw new InvalidOperationException("No se puede medir estabilidad de " + name + ".");
            StabilityCorners[name] = GetScreenCorners(rect);
        }
        stabilityStartTime = Time.unscaledTime;
    }

    private static bool ValidateStabilityFrame()
    {
        VerticalNavigationUI navigation =
            UnityEngine.Object.FindFirstObjectByType<VerticalNavigationUI>(FindObjectsInactive.Include);
        if (navigation == null || !navigation.NavigationSuppressed)
            throw new InvalidOperationException("Explorar perdió la supresión de navegación.");
        Transform secondary = FindSceneTransform("SecondaryNavigationSlot");
        if (secondary != null && secondary.gameObject.activeSelf)
            throw new InvalidOperationException("La navegación secundaria reapareció durante la espera.");

        foreach (KeyValuePair<string, Vector2[]> pair in StabilityCorners)
        {
            RectTransform rect = FindSceneTransform(pair.Key) as RectTransform;
            Vector2[] current = GetScreenCorners(rect);
            for (int i = 0; i < current.Length; i++)
                if (Vector2.Distance(current[i], pair.Value[i]) > 0.05f)
                    throw new InvalidOperationException(
                        pair.Key + " saltó " +
                        Vector2.Distance(current[i], pair.Value[i]).ToString("0.###") +
                        " px durante la prueba.");
        }

        return Time.unscaledTime - stabilityStartTime >= 10f;
    }

    private static Vector2[] GetScreenCorners(RectTransform rect)
    {
        if (rect == null)
            throw new InvalidOperationException("Falta RectTransform para medir estabilidad.");
        var world = new Vector3[4];
        rect.GetWorldCorners(world);
        Canvas canvas = rect.GetComponentInParent<Canvas>();
        Canvas rootCanvas = canvas != null ? canvas.rootCanvas : null;
        Camera camera = rootCanvas != null &&
            rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? rootCanvas.worldCamera
            : null;
        var result = new Vector2[4];
        for (int i = 0; i < world.Length; i++)
            result[i] = RectTransformUtility.WorldToScreenPoint(camera, world[i]);
        return result;
    }

    private static void ValidateTreeVisible()
    {
        Transform root = FindSceneTransform("D1_TreeVisualRoot");
        if (root == null || !root.gameObject.activeInHierarchy)
            throw new InvalidOperationException("El Árbol no quedó visible en el flujo real.");
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group == null || !group.interactable || !group.blocksRaycasts)
            throw new InvalidOperationException("El Árbol visible no acepta clics.");
        string[] hidden = { "PrimaryNavigationSlot", "SecondaryNavigationSlot", "MachineContextTabs" };
        foreach (string name in hidden)
        {
            Transform target = FindSceneTransform(name);
            if (target != null && target.gameObject.activeSelf)
                throw new InvalidOperationException("Navegación global superpuesta: " + name);
        }
    }

    private static void ValidateTreeClosed(string destination)
    {
        Transform root = FindSceneTransform("D1_TreeVisualRoot");
        if (root != null && root.gameObject.activeInHierarchy)
            throw new InvalidOperationException("El Árbol siguió abierto al ir a " + destination + ".");
    }

    private static void ValidateTarget(string name, string destination)
    {
        ValidateTreeClosed(destination);
        Transform target = FindSceneTransform(name);
        if (target == null || !target.gameObject.activeInHierarchy)
            throw new InvalidOperationException(destination + " no quedó visible.");
    }

    private static Dimension1PanelUI Panel()
    {
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        if (panel == null) throw new InvalidOperationException("Falta Dimension1PanelUI.");
        return panel;
    }

    private static Dimension1CommandCenterUI CommandCenter()
    {
        Dimension1CommandCenterUI command =
            UnityEngine.Object.FindFirstObjectByType<Dimension1CommandCenterUI>(FindObjectsInactive.Include);
        if (command == null) throw new InvalidOperationException("Falta Dimension1CommandCenterUI.");
        return command;
    }

    private static Transform FindChild(Transform parent, string name)
    {
        if (parent == null) return null;
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static Transform FindSceneTransform(string name)
    {
        Scene scene = SceneManager.GetActiveScene();
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
        return null;
    }
}
#endif
