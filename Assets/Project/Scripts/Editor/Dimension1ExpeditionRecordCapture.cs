#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// QA determinista del registro visual. Los datos sembrados viven solo durante
/// Play Mode porque SaveService permanece bloqueado durante toda la ejecución.
/// </summary>
public static class Dimension1ExpeditionRecordCapture
{
    private const string ActiveKey = "QF.D1ExpeditionRecordCapture.Active";
    private const string FrameKey = "QF.D1ExpeditionRecordCapture.Frame";
    private const string FailureKey = "QF.D1ExpeditionRecordCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath(
        "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_1/REGISTRO_EXPEDICIONES");
    private static string Output1080 => Path.Combine(OutputDirectory, "CAPTURA_CANDIDATA_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory, "CAPTURA_CANDIDATA_720x1280.png");

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
        DeleteIfPresent(Output1080);
        DeleteIfPresent(Output720);
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
            Debug.Log((failed ? "[D1 Expedition Record Capture] FAIL | " :
                "[D1 Expedition Record Capture] PASS | ruta real + 20 registros + ARK 21/21 + filtros + scroll + cierre + 1080x1920 + 720x1280 | ") + Output1080);
            EditorApplication.Exit(failed ? 1 : 0);
        }
    }

    private static void Tick()
    {
        try
        {
            int frame = SessionState.GetInt(FrameKey, 0) + 1;
            SessionState.SetInt(FrameKey, frame);
            if (frame == 24) Prepare();
            if (frame >= 24 && frame <= 58) ForceVisible();
            if (frame != 58) return;

            Dimension1ExpeditionRecordUI visual = RequireVisual();
            Transform root = visual.transform;
            ValidateVisible(root);
            ValidateSummaryIcons(root);
            ValidateRows(root, 20);
            RenderToPng(1080, 1920, Output1080);
            RenderToPng(720, 1280, Output720);
            ValidateFiltersAndScroll(visual, root);
            ValidateRewardCardinality(visual, root);
            ValidateCentralKeyCapacity(visual, root);
            ValidateEmptyFilter(visual, root);
            ValidateClose(visual, root);

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

    private static void Prepare()
    {
        SaveService.SuppressWritesForVisualQa = true;
        TabsUI tabs = TabsUI.Instance != null ? TabsUI.Instance : UnityEngine.Object.FindFirstObjectByType<TabsUI>();
        if (tabs != null) tabs.ShowDimension1();
        SetSceneObjectActive("Dimension1Panel", true);
        SetSceneObjectActive("Dimension1MainContent", true);
        HideUnrelated();
        HideReturnReport();

        GameState state = GameState.I;
        if (state == null) throw new InvalidOperationException("Falta GameState para el registro.");
        state.EnsureDimension1State();
        state.dimension1CentralAccessKeyObtained = false;
        state.dimension1RecentExplorationRecords = BuildRecords();

        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>();
        if (panel == null) throw new InvalidOperationException("Falta Dimension1PanelUI.");
        panel.OnClickOpenExplorationRecord();
        PrepareCanvasesForCapture();
        ForceVisible();
    }

    private static List<D1ExplorationRecordEntry> BuildRecords()
    {
        string[] sectors =
        {
            Dimension1System.Sector01OuterRim,
            Dimension1System.Sector02DebrisRing,
            Dimension1System.Sector03AncientOrbits,
            Dimension1System.Sector04SilentFrontier
        };
        string[][] destinations =
        {
            new[] { Dimension1System.DestinationMineralBelt, Dimension1System.DestinationShipGraveyard,
                Dimension1System.DestinationDriftingProbes, Dimension1System.DestinationAbandonedShip },
            new[] { Dimension1System.DestinationShipGraveyard, Dimension1System.DestinationMineralBelt,
                Dimension1System.DestinationAbandonedShip, Dimension1System.DestinationDriftingProbes },
            new[] { Dimension1System.DestinationAbandonedShip, Dimension1System.DestinationOrbitalRuin,
                Dimension1System.DestinationLaboratory, Dimension1System.DestinationAbandonedStation },
            new[] { Dimension1System.DestinationOrbitalRuin, Dimension1System.DestinationMinorAnomaly,
                Dimension1System.DestinationAncientStructure, Dimension1System.DestinationUnstableZone }
        };
        string[] ships =
        {
            Dimension1System.ShipLightProbe,
            Dimension1System.ShipExtractorDrone,
            Dimension1System.ShipAnalyticProbe,
            Dimension1System.ShipCargoShip
        };
        string[] metals =
        {
            Dimension1System.MetalIron,
            Dimension1System.MetalAluminum,
            Dimension1System.MetalNickel,
            Dimension1System.MetalLithium
        };

        var result = new List<D1ExplorationRecordEntry>();
        for (int i = 0; i < 19; i++)
        {
            // La expedición 120 pertenece a Órbitas Antiguas; mover la última
            // entrada del patrón a Frontera deja exactamente 5 casos por sector.
            int sectorIndex = i == 18 ? 3 : i % 4;
            string mainShip = ships[i % ships.Length];
            bool coordinated = i % 3 == 1;
            string supportShip = coordinated ? ships[(i + 1) % ships.Length] : "";
            var relics = new List<D1RelicRewardEntry>();
            if (i % 4 == 0)
                relics.Add(new D1RelicRewardEntry { relicId = Dimension1System.RelicFracturedAntenna });
            var matrices = new List<D1BlueprintAmount>();
            if (i % 5 == 0)
                matrices.Add(new D1BlueprintAmount { blueprintId = Dimension1System.BlueprintCargoFrame, amount = 1 });
            result.Add(new D1ExplorationRecordEntry
            {
                resultId = 101 + i,
                shipId = mainShip,
                supportShipId = supportShip,
                coordinatedMission = coordinated,
                synergyId = coordinated ? Dimension1System.GetD1SynergyId(mainShip, supportShip) : "",
                destinationId = destinations[sectorIndex][(i / 4) % 4],
                sectorId = sectors[sectorIndex],
                rewards = new List<D1MetalAmount>
                {
                    new D1MetalAmount { metalId = metals[i % 4], amount = 820d + i * 37d },
                    new D1MetalAmount { metalId = metals[(i + 1) % 4], amount = 310d + i * 19d }
                },
                blueprintFragments = i % 6 == 0 ? 3 : 0,
                specificBlueprintRewards = matrices,
                relicRewards = relics,
                totalSeconds = 620d + i * 31d
            });
        }

        result.Add(new D1ExplorationRecordEntry
        {
            resultId = 120,
            shipId = Dimension1System.ShipAnalyticProbe,
            destinationId = Dimension1System.DestinationLaboratory,
            sectorId = Dimension1System.Sector03AncientOrbits,
            rewards = new List<D1MetalAmount>
            {
                new D1MetalAmount { metalId = Dimension1System.MetalNickel, amount = 1240d },
                new D1MetalAmount { metalId = Dimension1System.MetalLithium, amount = 380d }
            },
            specificBlueprintRewards = new List<D1BlueprintAmount>(),
            relicRewards = new List<D1RelicRewardEntry>
            {
                new D1RelicRewardEntry { relicId = Dimension1System.RelicFracturedAntenna }
            },
            totalSeconds = 1122d
        });
        return result;
    }

    private static void ForceVisible()
    {
        Dimension1ExpeditionRecordUI visual = RequireVisual();
        // El estado cargado puede volver a conceder ARK por sus requisitos ya
        // cumplidos. La captura base exige 20 expediciones; ARK se prueba aparte.
        if (GameState.I != null) GameState.I.dimension1CentralAccessKeyObtained = false;
        HideReturnReport();
        for (Transform current = visual.transform; current != null; current = current.parent)
            current.gameObject.SetActive(true);
        visual.transform.SetAsLastSibling();
        visual.RefreshFromState(GameState.I, true);
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisible(Transform root)
    {
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        TMP_Text title = FindChild(root, "Heading")?.GetComponent<TMP_Text>();
        TMP_Text count = FindChild(root, "RecentCount")?.GetComponent<TMP_Text>();
        if (!root.gameObject.activeInHierarchy || group == null || group.alpha < .99f ||
            !group.interactable || !group.blocksRaycasts)
            throw new InvalidOperationException("El registro no quedó visible y bloqueante.");
        if (title == null || title.text != "REGISTRO DE EXPEDICIONES")
            throw new InvalidOperationException("Título del registro inválido.");
        if (count == null || count.text != "20 REGISTROS RECIENTES")
            throw new InvalidOperationException("El contador real de 20 registros no se presentó.");
        if (FindChild(root, "RecordRow_20") == null)
            throw new InvalidOperationException("Falta la reserva visual para ARK.");
    }

    private static void ValidateSummaryIcons(Transform root)
    {
        string[] labels = { "EXPEDICIONES", "RELIQUIAS", "METALES" };
        string[] spriteNames =
        {
            "d1_record_summary_expeditions_candidate_v1",
            "d1_record_summary_relics_candidate_v1",
            "d1_record_summary_metals_candidate_v1"
        };
        var seen = new HashSet<Sprite>();
        for (int i = 0; i < labels.Length; i++)
        {
            Image icon = FindChild(root, labels[i] + "Icon")?.GetComponent<Image>();
            if (icon == null || icon.sprite == null || icon.sprite.name != spriteNames[i])
                throw new InvalidOperationException("El resumen no usa el icono específico de " + labels[i] + ".");
            if (icon.material == null || icon.material.name != "d1_nav_premium_black_key")
                throw new InvalidOperationException("El icono de " + labels[i] + " perdió su recorte canónico.");
            if (!icon.preserveAspect || icon.color != Color.white || icon.raycastTarget)
                throw new InvalidOperationException("Tratamiento visual inválido en " + labels[i] + ".");
            if (!seen.Add(icon.sprite))
                throw new InvalidOperationException("Dos totales reutilizan el mismo icono sin identidad propia.");
        }
    }

    private static void ValidateFiltersAndScroll(Dimension1ExpeditionRecordUI visual, Transform root)
    {
        visual.SelectRelics();
        Canvas.ForceUpdateCanvases();
        ValidateRows(root, 6);

        visual.SelectCoordinated();
        Canvas.ForceUpdateCanvases();
        ValidateRows(root, 6);

        for (int sector = 0; sector < 4; sector++)
        {
            visual.CycleSector();
            Canvas.ForceUpdateCanvases();
            ValidateRows(root, 5);
        }

        visual.SelectAll();
        Canvas.ForceUpdateCanvases();
        ValidateRows(root, 20);
        ScrollRect scroll = FindChild(root, "RecordViewport")?.GetComponent<ScrollRect>();
        RectTransform content = FindChild(root, "Content") as RectTransform;
        if (scroll == null || content == null || content.rect.height < 4039f)
            throw new InvalidOperationException("El scroll no contiene los 20 registros completos.");
        scroll.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
        if (scroll.verticalNormalizedPosition > .01f || !FindChild(root, "RecordRow_19").gameObject.activeSelf)
            throw new InvalidOperationException("El registro más antiguo no es alcanzable al final del scroll.");
    }

    private static void ValidateCentralKeyCapacity(Dimension1ExpeditionRecordUI visual, Transform root)
    {
        GameState.I.dimension1CentralAccessKeyObtained = true;
        visual.SelectAll();
        Canvas.ForceUpdateCanvases();
        ValidateRows(root, 21);
        TMP_Text fixedLabel = FindChild(FindChild(root, "RecordRow_0"), "Destination")?.GetComponent<TMP_Text>();
        if (fixedLabel == null || fixedLabel.text != "CLAVE DE ACCESO CENTRAL")
            throw new InvalidOperationException("La entrada fija de ARK no conserva su lugar.");
        GameState.I.dimension1CentralAccessKeyObtained = false;
        visual.SelectAll();
    }

    private static void ValidateRewardCardinality(Dimension1ExpeditionRecordUI visual, Transform root)
    {
        D1ExplorationRecordEntry latest = GameState.I.dimension1RecentExplorationRecords[
            GameState.I.dimension1RecentExplorationRecords.Count - 1];
        List<D1MetalAmount> saved = latest.rewards;
        latest.rewards = new List<D1MetalAmount>
        {
            new D1MetalAmount { metalId = Dimension1System.MetalIron, amount = 100d },
            new D1MetalAmount { metalId = Dimension1System.MetalAluminum, amount = 200d },
            new D1MetalAmount { metalId = Dimension1System.MetalNickel, amount = 300d },
            new D1MetalAmount { metalId = Dimension1System.MetalLithium, amount = 400d }
        };
        visual.SelectAll();
        Canvas.ForceUpdateCanvases();
        Transform first = FindChild(root, "RecordRow_0");
        for (int i = 0; i < 4; i++)
            if (FindChild(first, "RewardChip_" + i)?.gameObject.activeSelf != true)
                throw new InvalidOperationException("No se representaron cuatro metales en una expedición.");

        latest.rewards = new List<D1MetalAmount>();
        visual.SelectAll();
        Canvas.ForceUpdateCanvases();
        for (int i = 0; i < 4; i++)
            if (FindChild(first, "RewardChip_" + i)?.gameObject.activeSelf == true)
                throw new InvalidOperationException("Una ficha de metal quedó visible en el estado vacío.");
        if (FindChild(first, "Rewards")?.gameObject.activeSelf != true)
            throw new InvalidOperationException("Falta el estado SIN METALES.");

        latest.rewards = saved;
        visual.SelectAll();
    }

    private static void ValidateEmptyFilter(Dimension1ExpeditionRecordUI visual, Transform root)
    {
        List<D1ExplorationRecordEntry> saved = GameState.I.dimension1RecentExplorationRecords;
        GameState.I.dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>
        {
            new D1ExplorationRecordEntry
            {
                resultId = 1,
                shipId = Dimension1System.ShipLightProbe,
                destinationId = Dimension1System.DestinationMineralBelt,
                sectorId = Dimension1System.Sector01OuterRim,
                rewards = new List<D1MetalAmount>(),
                specificBlueprintRewards = new List<D1BlueprintAmount>(),
                relicRewards = new List<D1RelicRewardEntry>()
            }
        };
        visual.SelectRelics();
        Canvas.ForceUpdateCanvases();
        ValidateRows(root, 0);
        TMP_Text empty = FindChild(root, "EmptyState")?.GetComponent<TMP_Text>();
        if (empty == null || !empty.gameObject.activeSelf || empty.text != "SIN REGISTROS PARA ESTE FILTRO")
            throw new InvalidOperationException("El estado vacío filtrado no es explícito.");
        GameState.I.dimension1RecentExplorationRecords = saved;
        visual.SelectAll();
    }

    private static void ValidateClose(Dimension1ExpeditionRecordUI visual, Transform root)
    {
        visual.Close();
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group.alpha != 0f || group.interactable || group.blocksRaycasts)
            throw new InvalidOperationException("El registro conserva raycasts después de volver.");
    }

    private static void ValidateRows(Transform root, int expected)
    {
        int active = 0;
        for (int i = 0; i <= Dimension1System.Dimension1RecentExplorationHistoryLimit; i++)
        {
            Transform row = FindChild(root, "RecordRow_" + i);
            if (row == null) throw new InvalidOperationException("Falta RecordRow_" + i + ".");
            if (row.gameObject.activeSelf) active++;
        }
        if (active != expected)
            throw new InvalidOperationException("Filas visibles: " + active + "; esperadas: " + expected + ".");
    }

    private static Dimension1ExpeditionRecordUI RequireVisual()
    {
        Transform root = FindSceneTransform("D1_ExpeditionRecordVisualRoot");
        Dimension1ExpeditionRecordUI visual = root != null ? root.GetComponent<Dimension1ExpeditionRecordUI>() : null;
        if (visual == null) throw new InvalidOperationException("Falta D1_ExpeditionRecordVisualRoot.");
        return visual;
    }

    private static void HideUnrelated()
    {
        string[] hide =
        {
            "Dimension2Panel", "Dimension3Panel", "Panel_Generacion", "Panel_Lab", "Panel_Logros",
            "Panel_Ajustes", "Panel_HUD", "HUD", "BottomDrawer", "PrimaryNavigationSlot",
            "SecondaryNavigationSlot", "MachineContextTabs", "PrestigePanel", "MetaPrestigePanel",
            "QA_PanelRoot", "QA_ToolsButton", "PresentationReturnReport"
        };
        foreach (string name in hide) SetSceneObjectActive(name, false);
    }

    private static void HideReturnReport()
    {
        PresentationReturnReportService.Consume();
        foreach (PresentationReturnReportUI report in
                 UnityEngine.Object.FindObjectsByType<PresentationReturnReportUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            report.gameObject.SetActive(false);
    }

    private static void PrepareCanvasesForCapture()
    {
        Camera camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("No hay cámara para captura.");
        foreach (Canvas canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude,
            FindObjectsSortMode.None))
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) continue;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1f;
        }
        Canvas.ForceUpdateCanvases();
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null ? Camera.main : UnityEngine.Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new InvalidOperationException("No hay cámara para captura.");
        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude,
            FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var cameras = new Camera[canvases.Length];
        for (int i = 0; i < canvases.Length; i++)
        {
            modes[i] = canvases[i].renderMode;
            cameras[i] = canvases[i].worldCamera;
            if (modes[i] != RenderMode.ScreenSpaceOverlay) continue;
            canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
            canvases[i].worldCamera = camera;
            canvases[i].planeDistance = 1f;
        }
        RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = modes[i];
                canvases[i].worldCamera = cameras[i];
            }
        }
    }

    private static Transform FindSceneTransform(string name)
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll<Transform>())
            if (transform.gameObject.scene.IsValid() && transform.name == name) return transform;
        return null;
    }

    private static T FindSceneComponent<T>() where T : Component
    {
        foreach (T component in Resources.FindObjectsOfTypeAll<T>())
            if (component.gameObject.scene.IsValid()) return component;
        return null;
    }

    private static Transform FindChild(Transform root, string name)
    {
        if (root == null) return null;
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static void SetSceneObjectActive(string name, bool active)
    {
        Transform target = FindSceneTransform(name);
        if (target != null) target.gameObject.SetActive(active);
    }

    private static void DeleteIfPresent(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }
}
#endif
