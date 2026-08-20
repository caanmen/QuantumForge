#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1ArkCapture
{
    private const string ActiveKey = "QF.D1ArkCapture.Active";
    private const string FrameKey = "QF.D1ArkCapture.Frame";
    private const string FailureKey = "QF.D1ArkCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/ArkCenter");
    private static string Output1080 => Path.Combine(OutputDirectory, "Ark_center_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory, "Ark_center_720x1280.png");

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        if (EditorApplication.isPlaying)
        {
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
    }

    public static void Run()
    {
        Directory.CreateDirectory(OutputDirectory);
        if (File.Exists(Output1080)) File.Delete(Output1080);
        if (File.Exists(Output720)) File.Delete(Output720);
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetBool(FailureKey, false);
        SessionState.SetInt(FrameKey, 0);
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
            Debug.Log((failed ? "[D1 Ark Capture] FAIL | " :
                "[D1 Ark Capture] PASS | open + back + metals + sync real | 1080x1920 + 720x1280 | ") + Output1080);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame >= 16) SuppressReports();
            if (frame == 22) PrepareAndOpen();
            if (frame == 34) TestMetalsRoundTrip();
            if (frame == 46) TestSyncAction();
            if (frame == 58) TestBackAndReopen();
            if (frame >= 59) ForceVisible();
            if (frame != 82) return;
            ValidateVisible();
            RenderToPng(1080, 1920, Output1080);
            RenderToPng(720, 1280, Output720);
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

    private static void PrepareAndOpen()
    {
        GameState state = GameState.I != null ? GameState.I : UnityEngine.Object.FindFirstObjectByType<GameState>();
        if (state == null) throw new InvalidOperationException("GameState no existe.");
        state.dimension01Unlocked = true;
        state.EnsureDimension1State();
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == Dimension1System.Sector05GalacticCenter) sector.unlocked = true;
        state.dimension1SelectedSectorId = Dimension1System.Sector05GalacticCenter;
        state.dimension1ArkInvestigated = true;
        state.dimension1CentralAccessKeyObtained = false;
        state.dimension1CentralSyncEstablished = false;
        state.dimension1ArkFinalMissionActive = false;
        state.dimension1GalacticAnchorDiscovered = false;
        EnsureMetal(state, Dimension1System.MetalIron, 5980000d);
        EnsureMetal(state, Dimension1System.MetalAluminum, 4730000d);
        EnsureMetal(state, Dimension1System.MetalNickel, 4700000d);
        SetMission(state, Dimension1System.D1CentralSyncOuter, false, true, 0d);
        SetMission(state, Dimension1System.D1CentralSyncDebris, true, false, 2538d);
        SetMission(state, Dimension1System.D1CentralSyncAncient, false, false, 0d);
        SetMission(state, Dimension1System.D1CentralSyncSilent, false, false, 0d);
        EnsureShipAvailable(state, Dimension1System.ShipAnalyticProbe);
        EnsureShipAvailable(state, Dimension1System.ShipCargoShip);
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs != null) tabs.ShowDimension1();
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        if (panel == null) throw new InvalidOperationException("Dimension1PanelUI no existe.");
        panel.OnClickOpenArkPanel();
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void TestMetalsRoundTrip()
    {
        FindChild(FindSceneTransform("ArkPanel"), "AllMetals")?.GetComponent<Button>()?.onClick.Invoke();
        CanvasGroup inventory = FindSceneTransform("D1_MetalsInventoryRoot")?.GetComponent<CanvasGroup>();
        if (inventory == null || inventory.alpha < .99f || !inventory.blocksRaycasts)
            throw new InvalidOperationException("10 METALES no abrió el inventario real.");
        Button back = FindChild(FindSceneTransform("D1_MetalsInventoryRoot"), "BackButton")?.GetComponent<Button>();
        if (back == null) throw new InvalidOperationException("El inventario no tiene regreso.");
        back.onClick.Invoke();
    }

    private static void TestSyncAction()
    {
        GameState state = GameState.I;
        D1CentralSyncMissionState mission = Dimension1System.GetD1CentralSyncMission(
            state, Dimension1System.D1CentralSyncAncient);
        Button button = FindChild(FindSceneTransform("Mission_2"), "MissionAction")?.GetComponent<Button>();
        if (button == null || !button.interactable) throw new InvalidOperationException("Sincronía Antigua no está accionable.");
        if (!Dimension1System.CanStartD1CentralSyncMission(state,
            Dimension1System.D1CentralSyncAncient, out string blocked))
            throw new InvalidOperationException("La sincronía real aparece bloqueada: " + blocked);
        button.onClick.Invoke();
        if (mission == null || !mission.active) throw new InvalidOperationException("El botón real no inició la sincronía.");
        mission.active = false;
        mission.completed = false;
        mission.remainingSeconds = 0d;
        EnsureShipAvailable(state, Dimension1System.ShipAnalyticProbe);
    }

    private static void TestBackAndReopen()
    {
        Transform root = FindSceneTransform("ArkPanel");
        Button back = FindChild(root, "CloseArkPanelButton")?.GetComponent<Button>();
        if (back == null) throw new InvalidOperationException("Falta regreso del ARK.");
        back.onClick.Invoke();
        if (root.gameObject.activeSelf) throw new InvalidOperationException("Regresar no cerró el ARK.");
        EnsureCenterSelected();
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        panel.OnClickOpenArkPanel();
    }

    private static void ForceVisible()
    {
        Transform root = FindSceneTransform("ArkPanel");
        if (root == null) throw new InvalidOperationException("Falta ArkPanel.");
        EnsureCenterSelected();
        Dimension1PanelUI panel = UnityEngine.Object.FindFirstObjectByType<Dimension1PanelUI>(FindObjectsInactive.Include);
        if (panel != null) panel.OnClickOpenArkPanel();
        Transform current = root;
        while (current != null) { current.gameObject.SetActive(true); current = current.parent; }
        HideUnrelated();
        Canvas.ForceUpdateCanvases();
    }

    private static void EnsureCenterSelected()
    {
        GameState state = GameState.I;
        if (state == null) return;
        state.dimension1SelectedSectorId = Dimension1System.Sector05GalacticCenter;
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == Dimension1System.Sector05GalacticCenter)
                sector.unlocked = true;
    }

    private static void ValidateVisible()
    {
        Transform root = FindSceneTransform("ArkPanel");
        if (root == null || !root.gameObject.activeInHierarchy)
        {
            string chain = "";
            Transform current = root;
            while (current != null)
            {
                chain += current.name + "[self=" + current.gameObject.activeSelf + "] <- ";
                current = current.parent;
            }
            throw new InvalidOperationException("ARK no está visible. " + chain);
        }
        string[] required = { "Header", "Hero", "ArkHeroArt", "Mission_0", "Mission_3", "AccessKey", "FinalMission" };
        foreach (string name in required)
            if (FindChild(root, name) == null) throw new InvalidOperationException("Falta bloque visible: " + name);
        if (root.GetComponent<Dimension1ArkVisualUI>() == null)
            throw new InvalidOperationException("Falta el controlador visual del ARK.");
    }

    private static void SetMission(GameState state, string id, bool active, bool completed, double remaining)
    {
        D1CentralSyncMissionState mission = Dimension1System.GetD1CentralSyncMission(state, id);
        if (mission == null) throw new InvalidOperationException("Falta misión " + id);
        mission.active = active; mission.completed = completed;
        mission.totalSeconds = Dimension1System.D1CentralSyncMissionDurationSeconds;
        mission.remainingSeconds = remaining;
    }

    private static void EnsureShipAvailable(GameState state, string id)
    {
        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship == null || ship.shipId != id) continue;
            ship.unlocked = true;
            ship.explorationActive = false;
            ship.activeDestinationId = "";
            ship.activeSpecialPointId = "";
            ship.activeSectorId = "";
            ship.coordinatedMission = false;
            ship.coordinatedSupportReserved = false;
            ship.coordinatedSupportShipId = "";
            ship.coordinatedMainShipId = "";
            ship.arkMissionReserved = false;
            ship.arkMissionId = "";
            return;
        }
    }

    private static void EnsureMetal(GameState state, string id, double minimum)
    {
        double current = state.GetD1MetalAmount(id);
        if (current < minimum) state.AddD1Metal(id, minimum - current);
    }

    private static void SuppressReports()
    {
        PresentationReturnReportService.Consume();
        foreach (PresentationReturnReportUI report in UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(FindObjectsInactive.Include, FindObjectsSortMode.None)) report.gameObject.SetActive(false);
        foreach (TriangleOfflineReportUI report in UnityEngine.Object.FindObjectsByType<TriangleOfflineReportUI>(FindObjectsInactive.Include, FindObjectsSortMode.None)) report.gameObject.SetActive(false);
    }

    private static void HideUnrelated()
    {
        string[] hide = { "Dimension2Panel", "Dimension3Panel", "Panel_Generacion", "Panel_Lab", "Panel_Logros",
            "Panel_Ajustes", "BottomDrawer", "PrimaryNavigationSlot", "SecondaryNavigationSlot",
            "MachineContextTabs", "PrestigePanel", "MetaPrestigePanel", "QA_PanelRoot", "QA_ToolsButton" };
        foreach (string name in hide)
        {
            Transform target = FindSceneTransform(name); if (target != null) target.gameObject.SetActive(false);
        }
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("No hay cámara para captura.");
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length]; var cameras = new Camera[canvases.Length];
        for (int i = 0; i < canvases.Length; i++)
        {
            modes[i] = canvases[i].renderMode; cameras[i] = canvases[i].worldCamera;
            if (modes[i] == RenderMode.ScreenSpaceOverlay)
            { canvases[i].renderMode = RenderMode.ScreenSpaceCamera; canvases[i].worldCamera = camera; canvases[i].planeDistance = 1f; }
        }
        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture; RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target; Canvas.ForceUpdateCanvases(); camera.Render(); Canvas.ForceUpdateCanvases(); camera.Render();
            RenderTexture.active = target; Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0); image.Apply(); File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget; RenderTexture.active = previousActive; target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            for (int i = 0; i < canvases.Length; i++) { canvases[i].renderMode = modes[i]; canvases[i].worldCamera = cameras[i]; }
        }
    }

    private static Transform FindSceneTransform(string name)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == name) return transform;
        return null;
    }
    private static Transform FindChild(Transform root, string name)
    {
        if (root == null) return null;
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true)) if (transform.name == name) return transform;
        return null;
    }
}
#endif
