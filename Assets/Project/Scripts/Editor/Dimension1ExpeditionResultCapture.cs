#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1ExpeditionResultCapture
{
    private const string ActiveKey = "QF.D1ExpeditionResultCapture.Active";
    private const string FrameKey = "QF.D1ExpeditionResultCapture.Frame";
    private const string FailureKey = "QF.D1ExpeditionResultCapture.Failed";
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private static string OutputDirectory => Path.GetFullPath("Logs/VisualQA/Dimension1/ExpeditionResult");
    private static string Output1080 => Path.Combine(OutputDirectory, "ExpeditionResult_1080x1920.png");
    private static string Output720 => Path.Combine(OutputDirectory, "ExpeditionResult_720x1280.png");
    private static string OutputFour1080 => Path.Combine(OutputDirectory, "ExpeditionResult_4Metals_1080x1920.png");
    private static string OutputFour720 => Path.Combine(OutputDirectory, "ExpeditionResult_4Metals_720x1280.png");

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
        if (File.Exists(OutputFour1080)) File.Delete(OutputFour1080);
        if (File.Exists(OutputFour720)) File.Delete(OutputFour720);
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
            Debug.Log((failed ? "[D1 Expedition Result Capture] FAIL | " :
                "[D1 Expedition Result Capture] PASS | 1080x1920 + 720x1280 | 0-4 metales + columnas alineadas + cierre seguro | ") + Output1080);
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
            if (frame >= 24 && frame <= 56) ForceVisible();
            if (frame != 56) return;
            ValidateVisible();
            RenderToPng(1080, 1920, Output1080);
            RenderToPng(720, 1280, Output720);
            ValidateCloseDoesNotBlock();
            ValidateAllMetalCountsAndRenderFour();
            ValidateEmptyAndDuplicateStates();
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

        GameState state = GameState.I;
        if (state == null) throw new InvalidOperationException("Falta GameState para captura.");
        state.EnsureDimension1State();
        int resultId = Mathf.Max(1, state.dimension1LastExplorationResultId + 1);
        var rewards = new List<D1MetalAmount>
        {
            new D1MetalAmount { metalId = Dimension1System.MetalAluminum, amount = 1240.0 },
            new D1MetalAmount { metalId = Dimension1System.MetalLithium, amount = 380.0 }
        };
        var matrices = new List<D1BlueprintAmount>
        {
            new D1BlueprintAmount { blueprintId = Dimension1System.BlueprintCargoFrame, amount = 1 }
        };
        var relics = new List<D1RelicRewardEntry>
        {
            new D1RelicRewardEntry { relicId = Dimension1System.RelicFracturedAntenna, wasDuplicate = false }
        };
        state.dimension1LastExplorationResultId = resultId;
        state.dimension1LastExplorationDestinationId = Dimension1System.DestinationLaboratory;
        state.dimension1LastExplorationRewards = rewards;
        state.dimension1LastExplorationBlueprintFragments = 3;
        state.dimension1LastExplorationSpecificBlueprints = matrices;
        state.dimension1LastExplorationRelics = relics;
        if (state.dimension1RecentExplorationRecords == null)
            state.dimension1RecentExplorationRecords = new List<D1ExplorationRecordEntry>();
        state.dimension1RecentExplorationRecords.Add(new D1ExplorationRecordEntry
        {
            resultId = resultId,
            shipId = Dimension1System.ShipAnalyticProbe,
            destinationId = Dimension1System.DestinationLaboratory,
            sectorId = Dimension1System.Sector03AncientOrbits,
            rewards = rewards,
            blueprintFragments = 3,
            specificBlueprintRewards = matrices,
            relicRewards = relics,
            totalSeconds = 1122.0,
            specialPointId = Dimension1System.D1SpecialPointRelicEcho
        });
        PrepareCanvasesForCapture();
        ForceVisible();
    }

    private static void ForceVisible()
    {
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        if (root == null) throw new InvalidOperationException("Falta D1_ExpeditionResultVisualRoot.");
        for (Transform current = root; current != null; current = current.parent)
            current.gameObject.SetActive(true);
        root.SetAsLastSibling();
        Dimension1ExpeditionResultUI visual = root.GetComponent<Dimension1ExpeditionResultUI>();
        if (visual == null) throw new InvalidOperationException("Falta el controlador de resultado.");
        visual.RefreshFromState(GameState.I, true);
        Canvas.ForceUpdateCanvases();
    }

    private static void ValidateVisible()
    {
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        CanvasGroup group = root != null ? root.GetComponent<CanvasGroup>() : null;
        if (root == null || !root.gameObject.activeInHierarchy || group == null ||
            group.alpha < .99f || !group.interactable || !group.blocksRaycasts)
            throw new InvalidOperationException("El resultado no quedó visible y bloqueante.");
        TMP_Text title = FindChild(root, "Title")?.GetComponent<TMP_Text>();
        TMP_Text destination = FindChild(root, "Destination")?.GetComponent<TMP_Text>();
        TMP_Text special = FindChild(FindChild(root, "SpecialPointBanner"), "Label")?.GetComponent<TMP_Text>();
        if (title == null || title.text != "EXPEDICIÓN COMPLETADA")
            throw new InvalidOperationException("Título de resultado inválido.");
        if (destination == null || destination.text != "LABORATORIO")
            throw new InvalidOperationException("El destino real no llegó al resultado.");
        if (special == null || !special.text.Contains("ECO DE RELIQUIA"))
            throw new InvalidOperationException("El punto especial real no llegó al resultado.");
        ValidateCenteredImage(root, "CompletionBadge");
        ValidateCenteredImage(root, "RelicIcon");
        if (FindChild(root, "MetalReward_0") == null || FindChild(root, "MetalReward_3") == null ||
            FindChild(root, "SpecificMatrixReward") == null)
            throw new InvalidOperationException("Faltan recompensas representadas.");
        ValidateTwoColumnAlignment(root);
    }

    private static void ValidateTwoColumnAlignment(Transform root)
    {
        RectTransform metal0 = FindChild(root, "MetalReward_0") as RectTransform;
        RectTransform metal1 = FindChild(root, "MetalReward_1") as RectTransform;
        RectTransform fragment = FindChild(root, "FragmentReward") as RectTransform;
        RectTransform matrix = FindChild(root, "SpecificMatrixReward") as RectTransform;
        if (metal0 == null || metal1 == null || fragment == null || matrix == null)
            throw new InvalidOperationException("No se pudieron comparar las columnas de recompensas.");
        if (Mathf.Abs(metal0.anchoredPosition.x - fragment.anchoredPosition.x) > .1f ||
            Mathf.Abs(metal1.anchoredPosition.x - matrix.anchoredPosition.x) > .1f ||
            Mathf.Abs(metal0.rect.width - fragment.rect.width) > .1f ||
            Mathf.Abs(metal1.rect.width - matrix.rect.width) > .1f)
            throw new InvalidOperationException("Metales y Matrices no comparten columnas exactas.");
    }

    private static void ValidateAllMetalCountsAndRenderFour()
    {
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        Dimension1ExpeditionResultUI visual = root != null
            ? root.GetComponent<Dimension1ExpeditionResultUI>()
            : null;
        GameState state = GameState.I;
        if (root == null || visual == null || state == null)
            throw new InvalidOperationException("No se pudo validar la cardinalidad de metales.");

        string[] ids =
        {
            Dimension1System.MetalAluminum,
            Dimension1System.MetalTitanium,
            Dimension1System.MetalLithium,
            Dimension1System.MetalCobalt
        };

        for (int count = 1; count <= 4; count++)
        {
            int resultId = state.dimension1LastExplorationResultId + 1;
            var rewards = new List<D1MetalAmount>();
            for (int i = 0; i < count; i++)
                rewards.Add(new D1MetalAmount { metalId = ids[i], amount = 125.0 + i * 75.0 });
            state.dimension1LastExplorationResultId = resultId;
            state.dimension1LastExplorationDestinationId = Dimension1System.DestinationLaboratory;
            state.dimension1RecentExplorationRecords.Add(new D1ExplorationRecordEntry
            {
                resultId = resultId,
                shipId = Dimension1System.ShipAnalyticProbe,
                destinationId = Dimension1System.DestinationLaboratory,
                sectorId = Dimension1System.Sector03AncientOrbits,
                rewards = rewards,
                specificBlueprintRewards = new List<D1BlueprintAmount>(),
                relicRewards = new List<D1RelicRewardEntry>(),
                totalSeconds = 1122.0
            });
            visual.RefreshFromState(state, true);
            Canvas.ForceUpdateCanvases();
            ValidateMetalSlotGeometry(root, count);
        }

        RenderToPng(1080, 1920, OutputFour1080);
        RenderToPng(720, 1280, OutputFour720);
    }

    private static void ValidateMetalSlotGeometry(Transform root, int expected)
    {
        RectTransform panel = FindChild(root, "MetalsPanel") as RectTransform;
        if (panel == null) throw new InvalidOperationException("Falta MetalsPanel.");
        float previousRight = float.NegativeInfinity;
        for (int i = 0; i < 4; i++)
        {
            RectTransform slot = FindChild(root, "MetalReward_" + i) as RectTransform;
            bool shouldBeActive = i < expected;
            if (slot == null || slot.gameObject.activeSelf != shouldBeActive)
                throw new InvalidOperationException("Estado incorrecto de MetalReward_" + i + " para " + expected + " metales.");
            if (!shouldBeActive) continue;
            float left = slot.anchoredPosition.x;
            float right = left + slot.rect.width;
            if (left < -.1f || right > panel.rect.width + .1f || left < previousRight - .1f)
                throw new InvalidOperationException("Los metales se salen o se superponen con " + expected + " recompensas.");
            previousRight = right;
            Image icon = FindChild(slot, "Icon")?.GetComponent<Image>();
            if (icon == null || icon.sprite == null || !icon.preserveAspect)
                throw new InvalidOperationException("MetalReward_" + i + " no conserva su icono.");
        }
    }

    private static void ValidateCloseDoesNotBlock()
    {
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        Dimension1ExpeditionResultUI visual = root.GetComponent<Dimension1ExpeditionResultUI>();
        visual.Continue();
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group.alpha != 0f || group.interactable || group.blocksRaycasts)
            throw new InvalidOperationException("El resultado conserva raycasts después de continuar.");
    }

    private static void ValidateEmptyAndDuplicateStates()
    {
        Transform root = FindSceneTransform("D1_ExpeditionResultVisualRoot");
        Dimension1ExpeditionResultUI visual = root.GetComponent<Dimension1ExpeditionResultUI>();
        GameState state = GameState.I;

        int emptyId = state.dimension1LastExplorationResultId + 1;
        state.dimension1LastExplorationResultId = emptyId;
        state.dimension1RecentExplorationRecords.Add(new D1ExplorationRecordEntry
        {
            resultId = emptyId,
            shipId = Dimension1System.ShipLightProbe,
            destinationId = Dimension1System.DestinationMineralBelt,
            sectorId = Dimension1System.Sector01OuterRim,
            rewards = new List<D1MetalAmount>(),
            specificBlueprintRewards = new List<D1BlueprintAmount>(),
            relicRewards = new List<D1RelicRewardEntry>()
        });
        visual.RefreshFromState(state, true);
        if (FindChild(root, "MetalsEmpty")?.gameObject.activeSelf != true ||
            FindChild(root, "MatricesEmpty")?.gameObject.activeSelf != true ||
            FindChild(root, "SpecialPointBanner")?.gameObject.activeSelf == true)
            throw new InvalidOperationException("Los estados vacíos del resultado son inválidos.");
        TMP_Text emptyRelic = FindChild(root, "RelicName")?.GetComponent<TMP_Text>();
        if (emptyRelic == null || emptyRelic.text != "NINGUNA RELIQUIA ENCONTRADA")
            throw new InvalidOperationException("El estado sin reliquia no es explícito.");

        int duplicateId = emptyId + 1;
        state.dimension1LastExplorationResultId = duplicateId;
        state.dimension1RecentExplorationRecords.Add(new D1ExplorationRecordEntry
        {
            resultId = duplicateId,
            shipId = Dimension1System.ShipExtractorDrone,
            destinationId = Dimension1System.DestinationOrbitalRuin,
            sectorId = Dimension1System.Sector03AncientOrbits,
            rewards = new List<D1MetalAmount>(),
            specificBlueprintRewards = new List<D1BlueprintAmount>(),
            relicRewards = new List<D1RelicRewardEntry>
            {
                new D1RelicRewardEntry
                {
                    relicId = Dimension1System.RelicFracturedAntenna,
                    wasDuplicate = true,
                    duplicateMetalId = Dimension1System.MetalIron,
                    duplicateMetalAmount = 42.0
                }
            }
        });
        visual.RefreshFromState(state, true);
        TMP_Text heading = FindChild(root, "RelicHeading")?.GetComponent<TMP_Text>();
        TMP_Text description = FindChild(root, "RelicDescription")?.GetComponent<TMP_Text>();
        if (heading == null || heading.text != "RELIQUIA RECUPERADA" ||
            description == null || !description.text.Contains("+42"))
            throw new InvalidOperationException("El estado de reliquia duplicada es inválido.");

        visual.RefreshFromState(state, false);
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group.alpha != 0f || group.interactable || group.blocksRaycasts)
            throw new InvalidOperationException("Los estados alternos no cerraron el modal.");
    }

    private static void ValidateCenteredImage(Transform root, string name)
    {
        Image image = FindChild(root, name)?.GetComponent<Image>();
        if (image == null || image.sprite == null || !image.preserveAspect)
            throw new InvalidOperationException(name + " no conserva su asset centrado.");
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
}
#endif
