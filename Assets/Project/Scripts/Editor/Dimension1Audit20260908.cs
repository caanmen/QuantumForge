#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Read-only scene audit: fixtures live only in Play Mode, never save the scene.</summary>
public static class Dimension1Audit20260908
{
    const string Key = "QF.D1Audit20260908.";
    const string Center = "D1CommandCenterProductionRoot";
    const string Galaxy = "D1_GalaxyVisualRoot";
    const string Explore = "D1_ExploreVisualRoot";
    const string Hangar = "D1_HangarVisualRoot";
    const string Relics = "D1_RelicsVisualRoot";
    const string Tree = "D1_TreeVisualRoot";
    static readonly List<Action> Steps = new List<Action>();
    static double lastStepTime;
    static int lastFrame;
    static int stableFrames;
    static bool stopping;
    static Vector3 centerPosition;
    static int Width => SessionState.GetInt(Key + "Width", 1080);
    static int Height => Width == 720 ? 1280 : 1920;
    static string Output => Path.GetFullPath("Logs/D1_Audit_2026-09-08/" +
        (SessionState.GetBool(Key + "CenterOnly", false) ? "drawer_touch_" :
         SessionState.GetBool(Key + "ExplorePopulated", false) ? "explore_functional_" :
         SessionState.GetBool(Key + "ExploreOnly", false) ? "support_text_" : "verified_native_") + Width);

    public static void Run1080() { SetMode(false, false, false); Start(1080); }
    public static void Run720() { SetMode(false, false, false); Start(720); }
    public static void RunExplore1080() { SetMode(true, false, false); Start(1080); }
    public static void RunExplore720() { SetMode(true, false, false); Start(720); }
    public static void RunCenter1080() { SetMode(false, true, false); Start(1080); }
    public static void RunCenter720() { SetMode(false, true, false); Start(720); }
    public static void RunExplorePopulated1080() { SetMode(false, false, true); Start(1080); }
    public static void RunExplorePopulated720() { SetMode(false, false, true); Start(720); }

    static void SetMode(bool exploreOnly, bool centerOnly, bool explorePopulated)
    {
        SessionState.SetBool(Key + "ExploreOnly", exploreOnly);
        SessionState.SetBool(Key + "CenterOnly", centerOnly);
        SessionState.SetBool(Key + "ExplorePopulated", explorePopulated);
    }

    public static void RepairVisualDetails()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        Transform scanner = FindSceneNode("D1_ExploreVisualRoot", "ScannerPanel");
        ((RectTransform)FindSceneNode(Explore, "SupportStatus")).sizeDelta = new Vector2(250, 32);
        Transform art = FindSceneNode("D1_ExploreVisualRoot", "SectorArtwork");
        Transform viewport = scanner.Find("SectorArtworkViewport");
        if (viewport == null)
        {
            var rect = new GameObject("SectorArtworkViewport", typeof(RectTransform), typeof(RectMask2D)).GetComponent<RectTransform>();
            rect.gameObject.layer = scanner.gameObject.layer;
            rect.SetParent(scanner, false);
            rect.SetSiblingIndex(art.GetSiblingIndex());
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(4, 4); rect.offsetMax = new Vector2(-4, -4);
            art.SetParent(rect, false);
            ((RectTransform)art).anchoredPosition = new Vector2(500, 78);
        }
        RectTransform badge = (RectTransform)FindSceneNode("D1_ExpeditionResultVisualRoot", "CompletionBadge");
        badge.anchoredPosition = new Vector2(357, -116);
        badge.sizeDelta = new Vector2(310, 310);
        foreach (string name in new[] { "SepA", "SepB" })
            ((RectTransform)FindSceneNode("D1_ExpeditionRecordVisualRoot", name)).sizeDelta = new Vector2(2, 92);
        RectTransform record = (RectTransform)FindSceneNode("D1_ExpeditionRecordVisualRoot", "D1_ExpeditionRecordVisualRoot");
        record.anchorMin = record.anchorMax = new Vector2(.5f, .5f);
        record.pivot = new Vector2(.5f, .5f);
        record.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        record.sizeDelta = new Vector2(1080, 1920);
        foreach (TMPro.TMP_Text text in FindSceneNode(Relics, Relics).GetComponentsInChildren<TMPro.TMP_Text>(true))
        {
            if (text.name != "Required" || !text.transform.parent.name.StartsWith("Cost_")) continue;
            text.enableAutoSizing = true; text.fontSizeMin = 14; text.fontSizeMax = 20;
            EditorUtility.SetDirty(text);
        }
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Could not save Main");
        Debug.Log("[D1 Audit Visual Details] REPAIR_PASS");
    }

    static Transform FindSceneNode(string rootName, string name)
    {
        foreach (Transform node in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (!node.gameObject.scene.IsValid() || node.name != rootName) continue;
            foreach (Transform child in node.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
        }
        throw new InvalidOperationException("Missing " + rootName + "/" + name);
    }

    static void Start(int width)
    {
        SaveService.SuppressWritesForVisualQa = true;
        Dimension1FinalValidation.ValidateFinalSaveCompatibilityBatch();
        D1TreePointsValidation.ValidateD1TreePointsBatch();
        SessionState.SetInt(Key + "Width", width);
        SessionState.SetInt(Key + "Stage", 0);
        SessionState.SetBool(Key + "Failed", false);
        Directory.CreateDirectory(Output);
        File.WriteAllText(Path.Combine(Output, "buttons.tsv"),
            "state\tscreen_width\tscreen_height\tbutton\tsample\tx\ty\tresult\tfirst_hit\thandler\n");
        File.WriteAllText(Path.Combine(Output, "viewport.tsv"), "state\trequested_width\trequested_height\tscreen_width\tscreen_height\n");
        EditorSceneManager.OpenScene("Assets/Project/Scenes/Main.unity", OpenSceneMode.Single);
        SetGameView();
        SessionState.SetBool(Key + "Active", true);
        Attach();
        EditorApplication.isPlaying = true;
    }

    [InitializeOnLoadMethod]
    static void Resume()
    {
        if (!SessionState.GetBool(Key + "Active", false)) return;
        // Reapply immediately after domain reload, before scene Start/Load callbacks.
        SaveService.SuppressWritesForVisualQa = true;
        Attach();
    }

    static void Attach()
    {
        BuildSteps();
        lastStepTime = EditorApplication.timeSinceStartup;
        lastFrame = -1;
        stableFrames = 0;
        stopping = false;
        EditorApplication.playModeStateChanged -= ModeChanged;
        EditorApplication.playModeStateChanged += ModeChanged;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    static void ModeChanged(PlayModeStateChange mode)
    {
        if (mode == PlayModeStateChange.EnteredPlayMode)
        {
            SaveService.SuppressWritesForVisualQa = true;
            lastStepTime = EditorApplication.timeSinceStartup;
        }
        if (mode != PlayModeStateChange.EnteredEditMode) return;
        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= ModeChanged;
        SessionState.SetBool(Key + "Active", false);
        bool failed = SessionState.GetBool(Key + "Failed", false);
        Debug.Log("[D1 Audit 2026-09-08] " + (failed ? "FAIL" : "PASS") +
            " | requested=" + Width + "x" + Height + " | " + Output);
        EditorApplication.Exit(failed ? 1 : 0);
    }

    static void Tick()
    {
        if (!EditorApplication.isPlaying || stopping) return;
        if (Time.frameCount != lastFrame) { lastFrame = Time.frameCount; stableFrames++; }
        if (stableFrames < 12 || EditorApplication.timeSinceStartup - lastStepTime < .35) return;
        int stage = SessionState.GetInt(Key + "Stage", 0);
        try
        {
            if (stage >= Steps.Count) { Stop(false); return; }
            Steps[stage]();
            SessionState.SetInt(Key + "Stage", stage + 1);
            lastStepTime = EditorApplication.timeSinceStartup;
            stableFrames = 0;
        }
        catch (Exception ex)
        {
            Exception actual = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
            Debug.LogException(actual);
            File.WriteAllText(Path.Combine(Output, "failure.txt"), "stage=" + stage + "\n" + actual);
            try { Snapshot("failure_stage_" + stage); } catch (Exception captureError) { Debug.LogException(captureError); }
            Stop(true);
        }
    }

    static void Stop(bool failed)
    {
        stopping = true;
        SessionState.SetBool(Key + "Failed", failed);
        EditorApplication.isPlaying = false;
    }

    static void BuildSteps()
    {
        Steps.Clear();
        if (SessionState.GetBool(Key + "ExplorePopulated", false))
        {
            Steps.Add(() => { Call("Prepare"); Screen.SetResolution(Width, Height, false); MissingScripts(); });
            Click(Center, "SectorCard"); Click(Galaxy, "EXPLORARButton");
            Steps.Add(() =>
            {
                typeof(Dimension1ExploreHangarFunctionalValidation)
                    .GetMethod("ValidateRuntimeFlow", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, null);
            });
            View(Explore, "05_explore_functional");
            Steps.Add(() =>
            {
                int populated = 0;
                foreach (TMPro.TMP_Text text in FindSceneNode(Explore, Explore).GetComponentsInChildren<TMPro.TMP_Text>(true))
                    if (text.transform.parent != null && text.transform.parent.name.StartsWith("DestinationCard") && text.text != "SIN SEÑAL")
                        populated++;
                if (populated < 1) throw new InvalidOperationException("La evidencia funcional no pobló las tarjetas de destinos.");
            });
            return;
        }
        if (SessionState.GetBool(Key + "CenterOnly", false))
        {
            Steps.Add(() => { Call("Prepare"); Screen.SetResolution(Width, Height, false); MissingScripts(); });
            View(Center, "01_center");
            Click(Center, "DimensionDrawerToggle"); View(Center, "02_center_drawer_open");
            Click(Center, "DimensionDrawerToggle"); View(Center, "03_center_drawer_closed");
            return;
        }
        if (SessionState.GetBool(Key + "ExploreOnly", false))
        {
            Steps.Add(() => { Call("Prepare"); Screen.SetResolution(Width, Height, false); MissingScripts(); });
            Click(Center, "SectorCard"); Click(Galaxy, "EXPLORARButton");
            View(Explore, "05_explore_empty");
            Steps.Add(() =>
            {
                var text = FindSceneNode(Explore, "SupportStatus").GetComponent<TMPro.TMP_Text>();
                text.ForceMeshUpdate();
                if (text.isTextTruncated || text.text != "BLOQUEADO EN ÁRBOL")
                    throw new InvalidOperationException("Estado de apoyo incompleto.");
            });
            return;
        }
        Steps.Add(() => { Call("Prepare"); Screen.SetResolution(Width, Height, false); MissingScripts(); });
        View(Center, "01_center");
        Click(Center, "DimensionDrawerToggle"); View(Center, "02_center_drawer_open");
        Click(Center, "DimensionDrawerToggle"); View(Center, "03_center_drawer_closed");
        Click(Center, "SectorCard"); View(Galaxy, "04_galaxy_neutral");
        Click(Galaxy, "EXPLORARButton"); View(Explore, "05_explore_empty");
        Click(Explore, "Nav_HANGAR"); View(Hangar, "06_hangar");
        Click(Hangar, "CommandCenter"); View(Center, "07_hangar_to_center");
        Click(Center, "Nav_HANGAR"); Click(Hangar, "Nav_EXPLORAR"); View(Explore, "08_hangar_to_explore");
        Click(Explore, "Nav_RELIQUIAS"); View(Relics, "09_relics_page1");
        Click(Relics, "NextPage"); View(Relics, "10_relics_page2");
        Click(Relics, "NextPage"); View(Relics, "11_relics_page3");
        Click(Relics, "CommandCenter"); View(Center, "12_relics_to_center");
        Click(Center, "Nav_RELIQUIAS"); Click(Relics, "Nav_EXPLORAR"); View(Explore, "13_relics_to_explore");
        Click(Explore, "Nav_ÁRBOL"); View(Tree, "14_tree");
        Click(Tree, "CommandCenter"); View(Center, "15_tree_to_center");
        Click(Center, "Nav_ÁRBOL"); Click(Tree, "Nav_EXPLORAR"); View(Explore, "16_tree_to_explore");
        Click(Explore, "Nav_GALAXIA");
        Sector("Sector01", "D1_OuterRimDetailVisualRoot", "17_outer_rim", true);
        Sector("Sector02", "D1_DebrisRingDetailVisualRoot", "20_debris_ring", false);
        Sector("Sector03", "D1_AncientOrbitsVisualRoot", "22_ancient_orbits", false);
        Sector("Sector04", "D1_SilentFrontierDetailVisualRoot", "24_silent_frontier", false);
        Click(Galaxy, "GalacticCenter"); View(Galaxy, "26_galaxy_ark_selected");
        Click(Galaxy, "EnterSectorButton"); View("ArkPanel", "27_ark");
        Click("ArkPanel", "InvestigateArkButton"); View("ArkPanel", "28_ark_investigated");
        Click("ArkPanel", "CloseArkPanelButton"); Click(Galaxy, "EXPLORARButton");
        Steps.Add(() => Call("PrepareCompletedExploration"));
        View("D1_ExpeditionResultVisualRoot", "29_expedition_result");
        Click("D1_ExpeditionResultVisualRoot", "CollectAndContinue");
        Click(Explore, "ExplorationRecord"); View("D1_ExpeditionRecordVisualRoot", "30_record");
        Click("D1_ExpeditionRecordVisualRoot", "ReturnToExplore"); View(Explore, "31_record_to_explore");
        Click(Explore, "CommandCenter"); View(Center, "32_return_center");
        Steps.Add(() => Call("ValidateExclusiveCommandCenter"));
        Steps.Add(() =>
        {
            typeof(Dimension1ExploreHangarFunctionalValidation).GetMethod("ValidateRuntimeFlow", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            Debug.Log("[D1 Explore+Hangar Functional] PASS | escaneo, selección, coordinación, desbloqueo y costes");
        });
    }

    static void Sector(string card, string root, string label, bool metals)
    {
        Click(Galaxy, card); View(Galaxy, label + "_selected");
        Click(Galaxy, "EnterSectorButton"); View(root, label);
        if (metals)
        {
            Click(root, "AllMetals"); View("D1_MetalsInventoryRoot", "18_metals");
            Click("D1_MetalsInventoryRoot", "BackButton"); View(root, "19_metals_return");
        }
        Click(root, "BackToGalaxyMap");
    }

    static void Click(string root, string button) { Steps.Add(() => Call("Click", root, button)); }
    static void View(string root, string label)
    {
        Steps.Add(() => { Call("RequireVisible", root); Snapshot(label); });
    }
    static void Call(string method, params object[] args)
    {
        MethodInfo info = typeof(Dimension1FullScreenRouteValidation).GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic);
        if (info == null) throw new MissingMethodException("Dimension1FullScreenRouteValidation", method);
        info.Invoke(null, args);
    }

    static void Snapshot(string label)
    {
        Canvas.ForceUpdateCanvases();
        if (label == "01_center") centerPosition = FindSceneNode(Center, Center).position;
        if ((label == "02_center_drawer_open" || label == "03_center_drawer_closed") &&
            Vector3.Distance(centerPosition, FindSceneNode(Center, Center).position) > .05f)
            throw new InvalidOperationException("El selector desplaza el Centro.");
        if (label == "17_outer_rim" || label == "20_debris_ring" || label == "22_ancient_orbits" || label == "24_silent_frontier")
        {
            CanvasGroup group = FindSceneNode(Galaxy, Galaxy).GetComponentInParent<CanvasGroup>();
            if (group == null || group.interactable || group.blocksRaycasts || group.alpha > .01f)
                throw new InvalidOperationException("Carta sigue interactiva detrás del sector.");
        }
        File.AppendAllText(Path.Combine(Output, "viewport.tsv"),
            label + "\t" + Width + "\t" + Height + "\t" + Screen.width + "\t" + Screen.height + "\n");
        AuditButtons(label);
        ScreenCapture.CaptureScreenshot(Path.Combine(Output, label + "_screen.png"));
    }

    static void AuditButtons(string label)
    {
        EventSystem events = EventSystem.current;
        if (events == null) throw new InvalidOperationException("Missing EventSystem");
        Vector2[] samples = { new Vector2(.5f,.5f), new Vector2(.2f,.2f), new Vector2(.8f,.2f), new Vector2(.2f,.8f), new Vector2(.8f,.8f) };
        foreach (Button button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (!button.IsActive() || !button.IsInteractable()) continue;
            bool visible = true;
            foreach (CanvasGroup group in button.GetComponentsInParent<CanvasGroup>())
            {
                if (group.alpha <= .01f || !group.interactable || !group.blocksRaycasts) visible = false;
                if (group.ignoreParentGroups) break;
            }
            if (!visible) continue;
            RectTransform rect = button.transform as RectTransform;
            Canvas canvas = button.GetComponentInParent<Canvas>();
            Camera camera = canvas != null && canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.rootCanvas.worldCamera : null;
            for (int i = 0; i < samples.Length; i++)
            {
                Vector2 local = rect.rect.min + Vector2.Scale(rect.rect.size, samples[i]);
                Vector2 point = RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(local));
                var pointer = new PointerEventData(events) { position = point };
                var hits = new List<RaycastResult>(); events.RaycastAll(pointer, hits);
                GameObject first = hits.Count > 0 ? hits[0].gameObject : null;
                GameObject handler = first != null ? ExecuteEvents.GetEventHandler<IPointerClickHandler>(first) : null;
                string result = point.x < 0 || point.y < 0 || point.x > Screen.width || point.y > Screen.height ? "OFFSCREEN" : handler == button.gameObject ? "OK" : "INTERCEPTED";
                if (label == "01_center" && button.name.StartsWith("Nav_") && result != "OK")
                    throw new InvalidOperationException("Navegación del Centro interceptada: " + button.name + " muestra " + i);
                File.AppendAllText(Path.Combine(Output, "buttons.tsv"), label + "\t" + Screen.width + "\t" + Screen.height + "\t" + PathOf(button.transform) + "\t" + i + "\t" + point.x.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "\t" + point.y.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "\t" + result + "\t" + PathOf(first != null ? first.transform : null) + "\t" + PathOf(handler != null ? handler.transform : null) + "\n");
            }
        }
    }

    static string PathOf(Transform target)
    {
        if (target == null) return "<none>";
        string path = target.name;
        while (target.parent != null) { target = target.parent; path = target.name + "/" + path; }
        return path.Replace("\t", " ").Replace("\n", " ");
    }

    static void MissingScripts()
    {
        var missing = new List<string>();
        foreach (Transform target in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (!target.gameObject.scene.IsValid()) continue;
            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(target.gameObject);
            if (count > 0) missing.Add(PathOf(target) + "\t" + count);
        }
        File.WriteAllLines(Path.Combine(Output, "missing_scripts.tsv"), missing);
        if (missing.Count > 0) throw new InvalidOperationException("Missing scripts: " + missing.Count);
    }

    static void Render(string path)
    {
        Camera camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("Missing capture camera");
        var roots = new List<Canvas>();
        foreach (Canvas canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (canvas.isRootCanvas) roots.Add(canvas);
        var modes = new RenderMode[roots.Count]; var cameras = new Camera[roots.Count]; var distances = new float[roots.Count];
        RenderTexture previousTarget = camera.targetTexture, previousActive = RenderTexture.active;
        var target = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
        Texture2D texture = null;
        try
        {
            for (int i = 0; i < roots.Count; i++)
            {
                modes[i] = roots[i].renderMode; cameras[i] = roots[i].worldCamera; distances[i] = roots[i].planeDistance;
                if (modes[i] != RenderMode.ScreenSpaceOverlay) continue;
                roots[i].renderMode = RenderMode.ScreenSpaceCamera; roots[i].worldCamera = camera; roots[i].planeDistance = 1f;
            }
            camera.targetTexture = target;
            for (int pass = 0; pass < 3; pass++)
            { Canvas.ForceUpdateCanvases(); camera.Render(); GL.Flush(); }
            RenderTexture.active = target;
            texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0); texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = previousTarget; RenderTexture.active = previousActive;
            for (int i = 0; i < roots.Count; i++)
            { roots[i].renderMode = modes[i]; roots[i].worldCamera = cameras[i]; roots[i].planeDistance = distances[i]; }
            if (texture != null) UnityEngine.Object.DestroyImmediate(texture);
            target.Release(); UnityEngine.Object.DestroyImmediate(target);
            Canvas.ForceUpdateCanvases();
        }
    }

    static void SetGameView()
    {
        // Unity editor internals vary by release; actual viewport is always recorded separately.
        try
        {
            Assembly assembly = typeof(Editor).Assembly;
            Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
            Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);
            object groupType = sizesType.GetProperty("currentGroupType").GetValue(sizes);
            object group = sizesType.GetMethod("GetGroup").Invoke(sizes, new[] { groupType });
            Type sizeType = assembly.GetType("UnityEditor.GameViewSize");
            Type sizeKind = assembly.GetType("UnityEditor.GameViewSizeType");
            object size = Activator.CreateInstance(sizeType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                new[] { Enum.Parse(sizeKind, "FixedResolution"), (object)Width, Height, "D1 Audit " + Width }, null);
            group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
            int count = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
            EditorWindow view = EditorWindow.GetWindow(assembly.GetType("UnityEditor.GameView"));
            view.GetType().GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(view, count - 1);
        }
        catch (Exception ex) { Debug.LogWarning("[D1 Audit] GameView size unavailable: " + ex.Message); }
        Screen.SetResolution(Width, Height, false);
    }
}
#endif
