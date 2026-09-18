#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1CommandCenterSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string CommandEmblemPath =
        "Assets/Project/UI/Dimension1/Generated/Candidates/CommandCenter/" +
        "d1_command_center_emblem_candidate_v1.png";
    private const float Width = 1080f;
    private const float Height = 1920f;
    private const float ObjectiveY = -472f;
    private const float ObjectiveHeight = 128f;
    private const float ProgressY = -602f;
    private const float ProgressHeight = 116f;
    private const float DrawerToggleY = -702f;
    private const float DrawerToggleHeight = 68f;
    private const float DrawerToggleWidth = 360f;

    public static void RepairAuditLayout()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Transform root = FindSceneTransform(scene, "D1CommandCenterProductionRoot");
        if (root == null) throw new InvalidOperationException("Falta Centro de Mando.");
        SetAuditPanel(root, "CurrentObjective", ObjectiveY, ObjectiveHeight);
        SetAuditPanel(root, "GlobalProgress", ProgressY, ProgressHeight);
        SetAuditPanel(root, "DimensionDrawerToggle", DrawerToggleY, DrawerToggleHeight, DrawerToggleWidth);
        Text drawerLabel = root.Find("DimensionDrawerToggle/Label")?.GetComponent<Text>();
        if (drawerLabel == null) throw new InvalidOperationException("Falta etiqueta de Dimensiones.");
        drawerLabel.rectTransform.sizeDelta = new Vector2(330f, 48f);
        drawerLabel.fontSize = 18;
        EditorUtility.SetDirty(drawerLabel);
        Transform hangar = FindSceneTransform(scene, "D1_HangarVisualRoot");
        if (hangar == null) throw new InvalidOperationException("Falta Hangar.");
        int costs = 0;
        foreach (TMPro.TMP_Text text in hangar.GetComponentsInChildren<TMPro.TMP_Text>(true))
        {
            if (text.name != "CostValue") continue;
            text.enableAutoSizing = true;
            text.fontSizeMin = 16f;
            text.fontSizeMax = 28f;
            EditorUtility.SetDirty(text);
            costs++;
        }
        if (costs == 0) throw new InvalidOperationException("Faltan textos de coste.");
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.");
        Debug.Log("[D1 Audit Layout] REPAIR_PASS | Centro y costes Hangar");
    }

    private static void SetAuditPanel(Transform root, string name, float y, float height, float width = 1044f)
    {
        RectTransform rect = root.Find(name) as RectTransform;
        if (rect == null) throw new InvalidOperationException("Falta " + name);
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, y);
        rect.sizeDelta = new Vector2(width, height);
        (rect.Find(name + "_Border") as RectTransform).sizeDelta = rect.sizeDelta;
        (rect.Find(name + "_InnerBorder") as RectTransform).sizeDelta = rect.sizeDelta - new Vector2(13, 13);
    }

    private static readonly Color Background = Hex("01070C");
    private static readonly Color PanelBase = Hex("031019", 0.98f);
    private static readonly Color PanelSoft = Hex("061722", 0.96f);
    private static readonly Color PanelInner = Hex("020C13", 0.94f);
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanBright = Hex("91EEFF");
    private static readonly Color CyanMuted = Hex("087BA7");
    private static readonly Color Steel = Hex("B8C4CC");
    private static readonly Color White = Hex("EDF2F5");
    private static readonly Color Amber = Hex("F5A719");

    private static Font font;
    private static Sprite frame;
    private static Sprite galaxyIcon;
    private static Sprite starfield;
    private static Sprite commandEmblem;

    private sealed class BuildReferences
    {
        public readonly List<Text> metalValues = new List<Text>();
        public readonly List<Text> metalRates = new List<Text>();
        public readonly List<Text> drawerMetalValues = new List<Text>();
        public Text sectorValue;
        public Text scannerValue;
        public Text fleetValue;
        public Text relicsValue;
        public Text treeValue;
        public Text expeditionsValue;
        public Text objectiveValue;
        public Text progressValue;
        public Dimension1CommandCenterLineGraphic progressLine;
        public Button metalsButton;
        public GameObject metalsDrawer;
        public readonly List<Button> galaxyButtons = new List<Button>();
        public readonly List<Button> exploreButtons = new List<Button>();
        public readonly List<Button> hangarButtons = new List<Button>();
        public readonly List<Button> relicButtons = new List<Button>();
        public readonly List<Button> treeButtons = new List<Button>();
        public readonly List<GameObject> navigationCards = new List<GameObject>();
        public Button dimensionDrawerToggle;
        public Text dimensionDrawerToggleLabel;
        public RectTransform crystal;
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Command Center Screen")]
    public static void Install()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Transform panel = FindSceneTransform(scene, "Dimension1Panel");
        if (panel == null) throw new InvalidOperationException("No existe Dimension1Panel en Main.unity.");

        font = AssetDatabase.LoadAssetAtPath<Font>(
            "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium.ttf");
        frame = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Project/UI/Dimension1/Generated/d1_premium_frame_v4.png");
        galaxyIcon = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Project/UI/Dimension1/Generated/d1_nav_galaxy_v3.png");
        starfield = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Project/UI/Dimension1/Generated/d1_starfield.png");
        EnsureSpriteImport(CommandEmblemPath);
        commandEmblem = AssetDatabase.LoadAssetAtPath<Sprite>(CommandEmblemPath);
        if (font == null || frame == null || galaxyIcon == null || starfield == null ||
            commandEmblem == null)
            throw new InvalidOperationException("Faltan recursos visuales del Centro de Mando.");

        Transform previous = FindChild(panel, "D1CommandCenterProductionRoot");
        if (previous != null) UnityEngine.Object.DestroyImmediate(previous.gameObject);

        GameObject root = Panel("D1CommandCenterProductionRoot", panel, Dimension1SharedLayoutTokens.RootOffset,
            new Vector2(Width, Height), Background);
        root.transform.SetAsLastSibling();
        root.AddComponent<Dimension1VisualSkinRoot>();
        CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
        Dimension1CommandCenterUI controller = root.AddComponent<Dimension1CommandCenterUI>();

        var refs = new BuildReferences();
        CreateBackdrop(root.transform);
        Image outer = CreateImage("OuterFrame", root.transform, frame, new Vector2(0f, 2f),
            new Vector2(Dimension1SharedLayoutTokens.OuterFrameWidth,
                Dimension1SharedLayoutTokens.OuterFrameHeight), Hex("087FA9", 0.72f));
        outer.type = Image.Type.Sliced;
        outer.raycastTarget = false;
        CreateHeader(root.transform, refs);
        CreateCommandBanner(root.transform);
        CreateDashboard(root.transform, refs);
        CreateMissionCards(root.transform, refs);
        CreateBottomNavigation(root.transform, refs);
        Dimension1SharedShellApply.ApplyToRoot(root.GetComponent<RectTransform>());
        CreateMetalsDrawer(root.transform, refs);

        GameObject legacyMain = FindChild(panel, "Dimension1MainContent")?.gameObject;
        GameObject[] blockingPanels = FindObjects(panel,
            "GalaxyPanel", "HangarPanel", "RelicChamberPanel", "Dimension1TreePanel",
            "ArkPanel", "ExplorationRewardsPanel", "Exploration Record Panel");
        VerticalNavigationUI verticalNavigation = UnityEngine.Object.FindFirstObjectByType<VerticalNavigationUI>(
            FindObjectsInactive.Include);
        if (verticalNavigation != null && verticalNavigation.tabs != null &&
            verticalNavigation.dimension2Button != null)
        {
            // El carrusel visible debe usar el mismo boton que TabsUI escucha.
            // La escena conservaba una referencia antigua y oculta de Dimension 2.
            verticalNavigation.tabs.btnDimension2 = verticalNavigation.dimension2Button;
            EditorUtility.SetDirty(verticalNavigation.tabs);
        }

        controller.Configure(
            canvasGroup,
            legacyMain,
            blockingPanels,
            verticalNavigation,
            refs.metalValues.ToArray(),
            refs.metalRates.ToArray(),
            refs.drawerMetalValues.ToArray(),
            refs.sectorValue,
            refs.scannerValue,
            refs.fleetValue,
            refs.relicsValue,
            refs.treeValue,
            refs.expeditionsValue,
            refs.objectiveValue,
            refs.progressValue,
            refs.progressLine,
            refs.metalsButton,
            refs.metalsDrawer,
            refs.galaxyButtons.ToArray(),
            refs.exploreButtons.ToArray(),
            refs.hangarButtons.ToArray(),
            refs.relicButtons.ToArray(),
            refs.treeButtons.ToArray(),
            FindButton(panel, "OpenGalaxyPanelButton"),
            FindButton(panel, "Btn_ScanD1"),
            FindButton(panel, "OpenHangarPanelButton"),
            FindButton(panel, "OpenRelicChamberPanelButton"),
            FindButton(panel, "OpenDimension1TreePanelButton"),
            refs.dimensionDrawerToggle,
            refs.dimensionDrawerToggleLabel,
            refs.navigationCards.ToArray(),
            refs.crystal);

        Transform exploreRoot = FindSceneTransform(scene, "D1_ExploreVisualRoot");
        if (exploreRoot != null)
        {
            controller.ConfigureExploreScreen(exploreRoot.gameObject);
            Dimension1ExploreVisualUI exploreVisual =
                exploreRoot.GetComponent<Dimension1ExploreVisualUI>();
            if (exploreVisual != null)
                exploreVisual.ConfigureCommandCenter(controller);
        }

        if (legacyMain != null)
            legacyMain.SetActive(false);
        RebindCommandCenterConsumers(scene, controller);
        refs.metalsDrawer.SetActive(false);
        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");

        ValidateInternal(scene);
        Debug.Log("[D1 Command Center] INSTALL_PASS | Main.unity | datos reales | navegación conectada");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Command Center Screen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Command Center] VALIDATION_PASS");
    }

    [MenuItem("Quantum Forge/Dimension 1/Repair Explore Navigation Link")]
    public static void RepairExploreNavigationLink()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1CommandCenterUI controller =
            FindSceneTransform(scene, "D1CommandCenterProductionRoot")?
                .GetComponent<Dimension1CommandCenterUI>();
        Dimension1ExploreVisualUI exploreVisual =
            FindSceneTransform(scene, "D1_ExploreVisualRoot")?
                .GetComponent<Dimension1ExploreVisualUI>();
        if (controller == null || exploreVisual == null)
            throw new InvalidOperationException(
                "No se encontraron Centro de Mando y Explorar modernos.");

        controller.ConfigureExploreScreen(exploreVisual.gameObject);
        exploreVisual.ConfigureCommandCenter(controller);
        RebindCommandCenterConsumers(scene, controller);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(exploreVisual);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");

        ValidateCommandCenterConsumers(scene, controller);
        Debug.Log("[D1 Explore Navigation Link] REPAIR_PASS | todos los enlaces al Centro persistidos");
    }

    private static bool IsCommandCenterConsumer(MonoBehaviour component)
    {
        return component is Dimension1ExploreVisualUI || component is Dimension1GalaxyVisualUI ||
            component is Dimension1HangarVisualUI || component is Dimension1RelicsVisualUI ||
            component is Dimension1TreeNavigationUI || component is Dimension1AncientOrbitsUI ||
            component is Dimension1SectorDetailUI;
    }

    private static void RebindCommandCenterConsumers(Scene scene, Dimension1CommandCenterUI controller)
    {
        // Recreating the hub invalidates incoming references, including inactive screens.
        // Rebind every D1 consumer without rebuilding its artwork or changing its state.
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        foreach (MonoBehaviour component in sceneRoot.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (!IsCommandCenterConsumer(component)) continue;
            var serialized = new SerializedObject(component);
            SerializedProperty link = serialized.FindProperty("commandCenter");
            if (link == null) throw new InvalidOperationException("Falta enlace de Centro en " + component.GetType().Name);
            if (link.objectReferenceValue == controller) continue;
            link.objectReferenceValue = controller;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(component);
        }
        ValidateCommandCenterConsumers(scene, controller);
    }

    private static void ValidateCommandCenterConsumers(Scene scene, Dimension1CommandCenterUI controller)
    {
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        foreach (MonoBehaviour component in sceneRoot.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (!IsCommandCenterConsumer(component)) continue;
            var serialized = new SerializedObject(component);
            SerializedProperty link = serialized.FindProperty("commandCenter");
            if (link == null || link.objectReferenceValue != controller)
                throw new InvalidOperationException("Enlace de Centro roto en " + component.GetType().Name);
        }
    }

    [MenuItem("Quantum Forge/Dimension 1/Capture Installed Command Center")]
    public static void CaptureInstalled()
    {
        CaptureInstalledState(false);
    }

    [MenuItem("Quantum Forge/Dimension 1/Capture Command Center Drawer Open")]
    public static void CaptureInstalledDrawerOpen()
    {
        CaptureInstalledState(true);
    }

    private static void CaptureInstalledState(bool drawerOpen)
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        string[] hide =
        {
            "Dimension2Panel", "Dimension3Panel", "Panel_Generacion", "Panel_Lab",
            "Panel_Logros", "Panel_Ajustes", "Panel_HUD", "HUD", "BottomDrawer",
            "PrimaryNavigationSlot", "PrestigePanel", "MetaPrestigePanel",
            "QA_PanelRoot", "QA_ToolsButton"
        };
        foreach (string name in hide) SetSceneObjectActive(scene, name, false);
        SetSceneObjectActive(scene, "SecondaryNavigationSlot", drawerOpen);

        SetSceneObjectActive(scene, "Dimension1Panel", true);
        SetSceneObjectActive(scene, "Dimension1MainContent", false);
        string[] close =
        {
            "GalaxyPanel", "HangarPanel", "RelicChamberPanel", "Dimension1TreePanel",
            "ArkPanel", "ExplorationRewardsPanel", "Exploration Record Panel"
        };
        foreach (string name in close) SetSceneObjectActive(scene, name, false);

        Transform root = FindSceneTransform(scene, "D1CommandCenterProductionRoot");
        if (root == null) throw new InvalidOperationException("Falta la pantalla instalada.");
        root.gameObject.SetActive(true);
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        if (drawerOpen)
        {
            Transform secondary = FindSceneTransform(scene, "SecondaryNavigationSlot");
            if (secondary is RectTransform secondaryRect)
            {
                secondaryRect.anchoredPosition = new Vector2(secondaryRect.anchoredPosition.x, 16f);

                // En una captura de Edit Mode no se ejecuta RefreshAvailability. Activar
                // los controles reales existentes evita documentar una bandeja vacía.
                foreach (string systemName in new[]
                {
                    "System_Room2", "System_Dimension1", "System_Dimension2", "System_Dimension3"
                })
                {
                    Transform systemButton = FindChild(secondaryRect, systemName);
                    if (systemButton != null) systemButton.gameObject.SetActive(true);
                }
                Transform prestige = FindChild(secondaryRect, "System_Prestige");
                if (prestige != null) prestige.gameObject.SetActive(false);
            }

            string[] commandNavigation =
            {
                "Nav_GALAXIA", "Nav_EXPLORAR", "Nav_HANGAR", "Nav_RELIQUIAS", "Nav_ÁRBOL"
            };
            foreach (string name in commandNavigation)
            {
                Transform card = FindChild(root, name);
                if (card != null) card.gameObject.SetActive(false);
            }

            Transform toggle = FindChild(root, "DimensionDrawerToggle");
            Text label = toggle == null ? null : FindChild(toggle, "Label")?.GetComponent<Text>();
            if (label != null) label.text = "CENTRO DE MANDO ▲";

            int visibleSystems = 0;
            if (secondary != null)
            {
                foreach (Button button in secondary.GetComponentsInChildren<Button>(false))
                    if (button.gameObject.activeInHierarchy) visibleSystems++;
            }
            if (visibleSystems == 0)
                throw new InvalidOperationException("La captura del cajón no contiene controles visibles.");
        }

        string output1080 = Path.GetFullPath(
            drawerOpen
                ? "Logs/VisualQA/dimension1_command_center_drawer_open_1080x1920.png"
                : "Logs/VisualQA/dimension1_command_center_integrated_1080x1920.png");
        string output720 = Path.GetFullPath(
            drawerOpen
                ? "Logs/VisualQA/dimension1_command_center_drawer_open_720x1280.png"
                : "Logs/VisualQA/dimension1_command_center_integrated_720x1280.png");
        Directory.CreateDirectory(Path.GetDirectoryName(output1080));
        RenderToPng(1080, 1920, output1080);
        RenderToPng(720, 1280, output720);
        Debug.Log("[D1 Command Center] CAPTURE_PASS | 1080x1920 + 720x1280 | " + output1080);
    }

    private static void ValidateInternal(Scene scene)
    {
        Transform root = FindSceneTransform(scene, "D1CommandCenterProductionRoot");
        if (root == null) throw new InvalidOperationException("Falta la pantalla de Centro de Mando.");
        ValidateCommandCenterConsumers(scene, root.GetComponent<Dimension1CommandCenterUI>());
        if (root.GetComponent<Dimension1CommandCenterUI>() == null ||
            root.GetComponent<Dimension1VisualSkinRoot>() == null ||
            root.GetComponent<CanvasGroup>() == null)
        {
            throw new InvalidOperationException("La raíz del Centro de Mando está incompleta.");
        }

        string[] required =
        {
            "CommandBanner", "SectorCard", "ScannerCard", "FleetCard", "RelicsCard",
            "TreeCard", "ExpeditionsCard", "QuantumCrystal", "CurrentObjective",
            "GlobalProgress", "Nav_GALAXIA", "Nav_EXPLORAR", "Nav_HANGAR",
            "Nav_RELIQUIAS", "Nav_ÁRBOL", "DimensionDrawerToggle", "MetalsDrawer"
        };
        foreach (string name in required)
            if (FindChild(root, name) == null)
                throw new InvalidOperationException("Elemento faltante: " + name);

        if (FindChild(root, "CommandEmblemArtwork") == null)
            throw new InvalidOperationException("Falta el nuevo emblema del Centro de Mando.");

        foreach (string cardName in new[]
        {
            "SectorCard", "ScannerCard", "FleetCard", "RelicsCard", "TreeCard",
            "ExpeditionsCard"
        })
        {
            Transform card = FindChild(root, cardName);
            if (FindChild(card, "DetailedLineIcon") == null)
                throw new InvalidOperationException(
                    "Falta el icono lineal detallado de " + cardName + ".");
        }

        ValidateMissionTextClearance(root, "CurrentObjective");
        ValidateMissionTextClearance(root, "GlobalProgress");

        if (root.GetComponentsInChildren<Button>(true).Length < 12)
            throw new InvalidOperationException("Faltan controles interactivos del Centro de Mando.");
        if (root.GetComponentsInChildren<Text>(true).Length < 30)
            throw new InvalidOperationException("Faltan textos del Centro de Mando.");
        if (UnityEngine.Object.FindFirstObjectByType<VerticalNavigationUI>(FindObjectsInactive.Include) == null)
            throw new InvalidOperationException("Falta el controlador del selector global de dimensiones.");
    }

    private static void ValidateMissionTextClearance(Transform root, string panelName)
    {
        foreach (string textName in new[] { "Label", "Value" })
        {
            RectTransform rect = FindChild(FindChild(root, panelName), textName) as RectTransform;
            if (rect == null)
                throw new InvalidOperationException("Falta " + panelName + "/" + textName + ".");
            float left = rect.anchoredPosition.x - rect.sizeDelta.x * rect.pivot.x;
            if (left < -355f)
                throw new InvalidOperationException(panelName + "/" + textName +
                    " invade el espacio reservado al icono.");
        }
    }

    private static void SetSceneObjectActive(Scene scene, string name, bool active)
    {
        Transform transform = FindSceneTransform(scene, name);
        if (transform == null) return;
        if (active)
        {
            for (Transform current = transform; current != null; current = current.parent)
                current.gameObject.SetActive(true);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }

    private static void RenderToPng(int width, int height, string path)
    {
        Camera camera = Camera.main != null ? Camera.main :
            UnityEngine.Object.FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
        if (camera == null) throw new InvalidOperationException("No hay cámara para la captura.");

        Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var cameras = new Camera[canvases.Length];
        for (int i = 0; i < canvases.Length; i++)
        {
            modes[i] = canvases[i].renderMode;
            cameras[i] = canvases[i].worldCamera;
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
            for (int pass = 0; pass < 8; pass++)
            {
                Canvas.ForceUpdateCanvases();
                camera.Render();
                GL.Flush();
            }
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

    private static void CreateBackdrop(Transform parent)
    {
        for (int x = 0; x <= Width; x += 90)
            Line("GridV", parent, new[]
            {
                new Vector2(x - Width * 0.5f, -Height * 0.5f),
                new Vector2(x - Width * 0.5f, Height * 0.5f)
            }, 1f, Hex("0B3D50", 0.08f));
        for (int y = 0; y <= Height; y += 90)
            Line("GridH", parent, new[]
            {
                new Vector2(-Width * 0.5f, y - Height * 0.5f),
                new Vector2(Width * 0.5f, y - Height * 0.5f)
            }, 1f, Hex("0B3D50", 0.065f));

        Line("TopLeftBracket", parent, new[]
        {
            new Vector2(-526, 882), new Vector2(-526, 922), new Vector2(-507, 942),
            new Vector2(-458, 942), new Vector2(-446, 949), new Vector2(-356, 949)
        }, 3f, CyanMuted);
        Line("TopRightBracket", parent, new[]
        {
            new Vector2(526, 882), new Vector2(526, 922), new Vector2(507, 942),
            new Vector2(458, 942), new Vector2(446, 949), new Vector2(356, 949)
        }, 3f, CyanMuted);
    }

    private static void CreateHeader(Transform parent, BuildReferences refs)
    {
        Text("DimensionTitle", parent, "DIMENSIÓN 1", new Vector2(0, 908),
            new Vector2(500, 62), 48, White, TextAnchor.MiddleCenter, FontStyle.Bold);
        CreateTitleCircuit(parent, -1f);
        CreateTitleCircuit(parent, 1f);

        string[] names = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        string[] values = { "5.98M", "4.73M", "4.70M" };
        string[] rates = { "+0.30/s", "+0.08/s", "+0.03/s" };
        float[] xs = { -405f, -137f, 132f };

        for (int i = 0; i < names.Length; i++)
        {
            GameObject chip = FramedPanel("Resource_" + names[i], parent,
                new Vector2(xs[i], 813), new Vector2(i == 0 ? 250 : 252, 92),
                PanelSoft, CyanMuted, 0.8f);
            CreateMetalGlyph(chip.transform, new Vector2(-92, 1), 24, i);
            Text("Name", chip.transform, names[i], new Vector2(5, 20),
                new Vector2(120, 24), 18, Steel, TextAnchor.MiddleLeft);
            Text value = Text("Value", chip.transform, values[i], new Vector2(5, -17),
                new Vector2(126, 38), 30, White, TextAnchor.MiddleLeft);
            Text rate = Text("Rate", chip.transform, rates[i], new Vector2(83, -17),
                new Vector2(76, 30), 16, Cyan, TextAnchor.MiddleRight);
            refs.metalValues.Add(value);
            refs.metalRates.Add(rate);
        }

        GameObject metals = FramedPanel("MetalsButton", parent, new Vector2(410, 813),
            new Vector2(244, 92), PanelSoft, CyanMuted, 0.8f);
        refs.metalsButton = metals.AddComponent<Button>();
        refs.metalsButton.targetGraphic = metals.GetComponent<Image>();
        Text("Label", metals.transform, "10 METALES⌄", Vector2.zero,
            new Vector2(210, 46), 22, Cyan, TextAnchor.MiddleCenter);
    }

    private static void CreateTitleCircuit(Transform parent, float direction)
    {
        float s = direction;
        Line("TitleCircuit", parent, new[]
        {
            new Vector2(178 * s, 906), new Vector2(240 * s, 906),
            new Vector2(255 * s, 918), new Vector2(320 * s, 918),
            new Vector2(330 * s, 908), new Vector2(410 * s, 908)
        }, 2f, CyanMuted);
        Dot("TitleNode", parent, new Vector2(178 * s, 906), 5f, CyanMuted);
        Dot("TitleNode", parent, new Vector2(255 * s, 918), 5f, CyanMuted);
        Dot("TitleNode", parent, new Vector2(330 * s, 908), 4f, CyanMuted);
        Dot("TitleNode", parent, new Vector2(410 * s, 908), 5f, CyanMuted);
    }

    private static void CreateCommandBanner(Transform parent)
    {
        GameObject banner = FramedPanel("CommandBanner", parent, new Vector2(0, 646),
            new Vector2(1044, 190), PanelBase, CyanMuted, 0.86f);
        GameObject emblem = Panel("CommandEmblem", banner.transform,
            new Vector2(-402, 0), new Vector2(145, 145), Color.clear);
        CreateHexagon(emblem.transform, Vector2.zero, 68, CyanMuted, 3f);
        Image emblemArt = CreateImage("CommandEmblemArtwork", emblem.transform, commandEmblem,
            Vector2.zero, new Vector2(124, 124), Color.white);
        emblemArt.type = Image.Type.Simple;
        emblemArt.preserveAspect = true;
        emblemArt.raycastTarget = false;
        Text("Title", banner.transform, "CENTRO DE MANDO", new Vector2(104, 27),
            new Vector2(720, 88), 54, White, TextAnchor.MiddleLeft);
        Text("Subtitle", banner.transform, "Tu base de operaciones en la Dimensión 1.",
            new Vector2(88, -39), new Vector2(740, 40), 25, Cyan, TextAnchor.MiddleLeft);
    }

    private static void CreateDashboard(Transform parent, BuildReferences refs)
    {
        GameObject dashboard = Panel("CommandDashboard", parent, new Vector2(0, 45),
            new Vector2(1080, 900), Color.clear);
        Button sectorButton = CreateSideCard(dashboard.transform, "SectorCard", "SECTOR ACTUAL",
            "ÓRBITAS\nANTIGUAS", new Vector2(-389, 303), Amber, IconKind.Radar,
            out refs.sectorValue);
        Button scannerButton = CreateSideCard(dashboard.transform, "ScannerCard", "ESCÁNER",
            "NIVEL 3/15", new Vector2(-389, 0), Cyan, IconKind.Scanner,
            out refs.scannerValue);
        Button fleetButton = CreateSideCard(dashboard.transform, "FleetCard", "FLOTA",
            "4 NAVES", new Vector2(-389, -303), Cyan, IconKind.Ship,
            out refs.fleetValue);
        Button relicButton = CreateSideCard(dashboard.transform, "RelicsCard", "RELIQUIAS",
            "12/20", new Vector2(389, 303), Cyan, IconKind.Relic,
            out refs.relicsValue);
        Button treeButton = CreateSideCard(dashboard.transform, "TreeCard", "PUNTOS DEL\nÁRBOL",
            "3", new Vector2(389, 0), Cyan, IconKind.Tree,
            out refs.treeValue);
        Button expeditionButton = CreateSideCard(dashboard.transform, "ExpeditionsCard",
            "EXPEDICIONES\nACTIVAS", "1", new Vector2(389, -303), Cyan,
            IconKind.Compass, out refs.expeditionsValue);

        refs.galaxyButtons.Add(sectorButton);
        refs.exploreButtons.Add(scannerButton);
        refs.hangarButtons.Add(fleetButton);
        refs.relicButtons.Add(relicButton);
        refs.treeButtons.Add(treeButton);
        refs.exploreButtons.Add(expeditionButton);

        GameObject hologram = Panel("CentralHologram", dashboard.transform,
            new Vector2(0, -4), new Vector2(500, 880), Color.clear);
        Image stars = CreateImage("CommandStarfield", hologram.transform, starfield,
            Vector2.zero, new Vector2(480, 850), Hex("7FD9FF", 0.18f));
        stars.preserveAspect = false;
        stars.raycastTarget = false;
        CreateHolographicInstrument(hologram.transform);
        refs.crystal = CreateCrystal(hologram.transform, new Vector2(0, 28));

        CreateCircuitConnection(dashboard.transform, new Vector2(-244, 303), new Vector2(-177, 245));
        CreateCircuitConnection(dashboard.transform, new Vector2(-244, 0), new Vector2(-127, -18));
        CreateCircuitConnection(dashboard.transform, new Vector2(-244, -303), new Vector2(-164, -300));
        CreateCircuitConnection(dashboard.transform, new Vector2(244, 303), new Vector2(177, 245));
        CreateCircuitConnection(dashboard.transform, new Vector2(244, 0), new Vector2(127, -18));
        CreateCircuitConnection(dashboard.transform, new Vector2(244, -303), new Vector2(164, -300));
    }

    private enum IconKind { Radar, Scanner, Ship, Relic, Tree, Compass }

    private static Button CreateSideCard(Transform parent, string name, string label,
        string value, Vector2 position, Color valueColor, IconKind icon, out Text valueText)
    {
        GameObject card = FramedPanel(name, parent, position, new Vector2(286, 284),
            PanelInner, CyanMuted, 0.76f);
        Button button = card.AddComponent<Button>();
        button.targetGraphic = card.GetComponent<Image>();
        GameObject iconRoot = Panel("DetailedLineIcon", card.transform,
            new Vector2(0, 70), new Vector2(180, 132), Color.clear);
        CreateCardIcon(iconRoot.transform, icon, Vector2.zero);
        Text("Label", card.transform, label, new Vector2(0, -27),
            new Vector2(242, 58), 24, Steel, TextAnchor.MiddleCenter);
        valueText = Text("Value", card.transform, value, new Vector2(0, -94),
            new Vector2(244, 76), 28, valueColor, TextAnchor.MiddleCenter);
        Dot("CornerMark", card.transform, new Vector2(-108, 116), 2.5f, CyanMuted);
        Dot("CornerMark", card.transform, new Vector2(108, -116), 2.5f, CyanMuted);
        return button;
    }

    private static void CreateHolographicInstrument(Transform parent)
    {
        Vector2 center = new Vector2(0, 40);
        Circle("OrbitOuter", parent, center, 226, 62, 2f, Hex("0ABFF5", 0.70f));
        Circle("OrbitTelemetry", parent, center, 210, 58, 1.2f,
            Hex("54DFFF", 0.35f), true);
        Circle("OrbitMid", parent, center, 190, 56, 2f, Hex("0ABFF5", 0.70f));
        Circle("OrbitInner", parent, center, 150, 48, 2f, Hex("0ABFF5", 0.50f));
        Circle("OrbitDotted", parent, center, 118, 42, 2.4f, Hex("3AD6FF", 0.44f), true);
        for (int i = 0; i < 24; i++)
        {
            float angle = i * Mathf.PI * 2f / 24f;
            Vector2 inner = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 207f;
            Vector2 outer = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) *
                (i % 3 == 0 ? 228f : 216f);
            Line("RadialTick", parent, new[] { inner, outer }, i % 3 == 0 ? 2f : 1f,
                Hex("22C8F5", i % 3 == 0 ? 0.65f : 0.35f));
        }
        Line("TelemetryLeft", parent, new[]
        {
            new Vector2(-242, 40), new Vector2(-205, 40), new Vector2(-184, 61),
            new Vector2(-166, 61)
        }, 1.7f, Hex("65E5FF", 0.56f));
        Line("TelemetryRight", parent, new[]
        {
            new Vector2(242, 40), new Vector2(205, 40), new Vector2(184, 61),
            new Vector2(166, 61)
        }, 1.7f, Hex("65E5FF", 0.56f));
        Dot("TelemetryAmberLeft", parent, new Vector2(-205, 40), 3.5f, Amber);
        Dot("TelemetryAmberRight", parent, new Vector2(205, 40), 3.5f, Amber);
        Ellipse("PedestalOuter", parent, new Vector2(0, -307), 198, 68, 60, 2.2f, Hex("18C8FF", 0.90f));
        Line("PedestalMechanicalFrame", parent, new[]
        {
            new Vector2(-158, -307), new Vector2(-126, -341),
            new Vector2(126, -341), new Vector2(158, -307),
            new Vector2(126, -273), new Vector2(-126, -273)
        }, 2f, Hex("087BA7", 0.86f), true);
        Ellipse("PedestalMid", parent, new Vector2(0, -307), 150, 50, 56, 2f, Hex("18C8FF", 0.90f));
        Ellipse("PedestalTelemetry", parent, new Vector2(0, -307), 121, 40, 52,
            1.1f, Hex("91EEFF", 0.48f));
        Ellipse("PedestalInner", parent, new Vector2(0, -307), 93, 31, 48, 2f, CyanBright);
        Ellipse("PedestalCore", parent, new Vector2(0, -307), 35, 13, 36, 3f, CyanBright);
        Line("PedestalLowerRail", parent, new[]
        {
            new Vector2(-112, -354), new Vector2(112, -354)
        }, 2f, Hex("18C8FF", 0.48f));
        for (int i = -3; i <= 3; i++)
        {
            Line("PedestalVent", parent, new[]
            {
                new Vector2(i * 24f, -343), new Vector2(i * 24f, -359)
            }, 1.2f, Hex("3AD6FF", 0.52f));
        }
        Line("EnergyBeam", parent, new[] { new Vector2(0, -310), new Vector2(0, 328) },
            4f, Hex("51DFFF", 0.86f));
        for (int i = 0; i < 68; i++)
        {
            float a = i * 2.39996323f;
            float r = 28f + (i * 31 % 165);
            Vector2 p = center + new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r * 1.18f);
            Dot("StarParticle", parent, p, i % 9 == 0 ? 2.5f : 1.35f,
                Hex("38D6FF", i % 4 == 0 ? 0.9f : 0.58f));
        }
    }

    private static RectTransform CreateCrystal(Transform parent, Vector2 center)
    {
        GameObject root = Panel("QuantumCrystal", parent, center, new Vector2(300, 560), Color.clear);
        Vector2[] outer =
        {
            new Vector2(0, 250), new Vector2(72, 128), new Vector2(64, -125),
            new Vector2(0, -250), new Vector2(-64, -125), new Vector2(-72, 128)
        };
        Vector2 upperCore = new Vector2(0, 70);
        Vector2 lowerCore = new Vector2(0, -28);
        Polygon("FacetFillTopLeft", root.transform,
            new[] { outer[0], outer[5], upperCore }, Hex("0586C4", 0.52f));
        Polygon("FacetFillTopRight", root.transform,
            new[] { outer[0], upperCore, outer[1] }, Hex("00B9F2", 0.39f));
        Polygon("FacetFillMidLeft", root.transform,
            new[] { outer[5], upperCore, lowerCore, outer[4] }, Hex("006FAE", 0.34f));
        Polygon("FacetFillMidRight", root.transform,
            new[] { upperCore, outer[1], outer[2], lowerCore }, Hex("009DDB", 0.40f));
        Polygon("FacetFillBottomLeft", root.transform,
            new[] { outer[4], lowerCore, outer[3] }, Hex("007FBC", 0.46f));
        Polygon("FacetFillBottomRight", root.transform,
            new[] { lowerCore, outer[2], outer[3] }, Hex("00B6E9", 0.35f));
        Line("CrystalOuterGlow", root.transform, outer, 15f, Hex("19CAFF", 0.22f), true);
        Line("CrystalOuter", root.transform, outer, 5.5f, CyanBright, true);
        Line("CrystalInnerShell", root.transform, new[]
        {
            new Vector2(0, 224), new Vector2(55, 119), new Vector2(49, -115),
            new Vector2(0, -222), new Vector2(-49, -115), new Vector2(-55, 119)
        }, 1.5f, Hex("BDF6FF", 0.48f), true);
        Line("CrystalSpine", root.transform, new[] { outer[0], outer[3] }, 3f, Hex("D5FCFF", 0.95f));
        Line("CrystalUpperFacet", root.transform, new[] { outer[5], upperCore, outer[1] }, 3.2f, CyanBright);
        Line("CrystalLowerFacet", root.transform, new[] { outer[4], lowerCore, outer[2] }, 3.2f, CyanBright);
        Line("CrystalFacetLeft", root.transform,
            new[] { outer[0], outer[5], upperCore, outer[4], outer[3] }, 2f, Cyan);
        Line("CrystalFacetRight", root.transform,
            new[] { outer[0], outer[1], upperCore, outer[2], outer[3] }, 2f, Cyan);
        Line("CrystalCrownFacet", root.transform, new[]
        {
            new Vector2(-42, 127), new Vector2(0, 178), new Vector2(42, 127),
            upperCore, new Vector2(-42, 127)
        }, 1.7f, Hex("D7FBFF", 0.72f));
        Line("CrystalLowerCrownFacet", root.transform, new[]
        {
            new Vector2(-43, -122), new Vector2(0, -76), new Vector2(43, -122),
            lowerCore, new Vector2(-43, -122)
        }, 1.7f, Hex("91EEFF", 0.68f));

        Polygon("CoreAura", root.transform, new[]
        {
            new Vector2(0, 48), new Vector2(31, 8), new Vector2(0, -38),
            new Vector2(-31, 8)
        }, Hex("18C8FF", 0.18f));
        Polygon("CoreDiamond", root.transform, new[]
        {
            new Vector2(0, 35), new Vector2(22, 7), new Vector2(0, -26),
            new Vector2(-22, 7)
        }, Hex("91EEFF", 0.32f));
        Line("CoreDiamondOutline", root.transform, new[]
        {
            new Vector2(0, 35), new Vector2(22, 7), new Vector2(0, -26),
            new Vector2(-22, 7)
        }, 2.2f, Hex("D7FBFF", 0.94f), true);
        Circle("CoreTelemetryOuter", root.transform, new Vector2(0, 7), 38, 40,
            1.1f, Hex("91EEFF", 0.42f), true);
        Circle("CoreTelemetryInner", root.transform, new Vector2(0, 7), 25, 34,
            1.2f, Hex("91EEFF", 0.62f));
        Dot("CrystalCoreGlow", root.transform, new Vector2(0, 7), 10f, Hex("91EEFF", 0.48f));
        Dot("CrystalCore", root.transform, new Vector2(0, 7), 4.5f, Hex("EFFFFF"));
        Line("CoreCrossHorizontal", root.transform,
            new[] { new Vector2(-47, 7), new Vector2(47, 7) }, 1.2f, Hex("91EEFF", 0.58f));

        CreateFloatingCrystalShard(root.transform, new Vector2(-96, 72), 1f);
        CreateFloatingCrystalShard(root.transform, new Vector2(96, 83), 0.92f);
        CreateFloatingCrystalShard(root.transform, new Vector2(-95, -91), 0.88f);
        CreateFloatingCrystalShard(root.transform, new Vector2(94, -84), 0.82f);

        for (int i = 0; i < 28; i++)
        {
            float y = -205 + i * 15.4f;
            float x = ((i * 37) % 116) - 58;
            Dot("CrystalSpark", root.transform, new Vector2(x, y), i % 6 == 0 ? 2.2f : 1.15f,
                Hex("C6F9FF", i % 3 == 0 ? 0.95f : 0.65f));
        }
        return root.GetComponent<RectTransform>();
    }

    private static void CreateFloatingCrystalShard(Transform parent, Vector2 center, float scale)
    {
        Vector2[] shard =
        {
            center + new Vector2(0, 25) * scale,
            center + new Vector2(12, 4) * scale,
            center + new Vector2(4, -25) * scale,
            center + new Vector2(-12, -4) * scale
        };
        Polygon("FloatingShardFill", parent, shard, Hex("0789BA", 0.34f));
        Line("FloatingShard", parent, shard, 1.8f * scale, CyanBright, true);
        Line("FloatingShardFacet", parent, new[]
        {
            shard[0], center, shard[2], shard[1], center, shard[3]
        }, 1.05f * scale, Hex("BDF6FF", 0.72f));
        Dot("FloatingShardCore", parent, center, 2.3f * scale, CyanBright);
        Ellipse("FloatingShardOrbit", parent, center, 20f * scale, 7f * scale,
            24, 0.9f * scale, Hex("18C8FF", 0.32f));
    }

    private static void CreateCircuitConnection(Transform parent, Vector2 cardEdge, Vector2 coreEdge)
    {
        float direction = Mathf.Sign(coreEdge.x - cardEdge.x);
        Vector2 midA = cardEdge + new Vector2(direction * 22, 0);
        Vector2 midB = new Vector2((midA.x + coreEdge.x) * 0.5f, coreEdge.y);
        Line("CircuitLink", parent, new[] { cardEdge, midA, midB, coreEdge }, 2.2f, Cyan);
        Dot("CircuitNode", parent, cardEdge, 5f, CyanBright);
        Dot("CircuitNode", parent, coreEdge, 4f, Cyan);
    }

    private static void CreateMissionCards(Transform parent, BuildReferences refs)
    {
        GameObject objective = FramedPanel("CurrentObjective", parent, new Vector2(0, ObjectiveY),
            new Vector2(1044, ObjectiveHeight), PanelBase, CyanMuted, 0.78f);
        CreateRadarGlyph(objective.transform, new Vector2(-421, 0), 42, Cyan);
        // El texto comienza después del glifo. Mantener el borde izquierdo en -350
        // evita la superposición tanto a 1080x1920 como al escalar a 720x1280.
        Text("Label", objective.transform, "RECOMENDACIÓN ACTUAL", new Vector2(-115, 24),
            new Vector2(470, 38), 26, Cyan, TextAnchor.MiddleLeft);
        refs.objectiveValue = Text("Value", objective.transform, "Explora 3 señales desconocidas.",
            new Vector2(20, -28), new Vector2(740, 42), 30, Amber, TextAnchor.MiddleLeft);

        GameObject progress = FramedPanel("GlobalProgress", parent, new Vector2(0, ProgressY),
            new Vector2(1044, ProgressHeight), PanelBase, CyanMuted, 0.78f);
        CreateCompassGlyph(progress.transform, new Vector2(-421, 0), 41, Cyan);
        Text("Label", progress.transform, "PROGRESO PRESTIGIO 1", new Vector2(-115, 20),
            new Vector2(470, 36), 25, Cyan, TextAnchor.MiddleLeft);
        refs.progressValue = Text("Value", progress.transform, "42%", new Vector2(-115, -27),
            new Vector2(470, 44), 34, Amber, TextAnchor.MiddleLeft);
        Circle("RingBack", progress.transform, new Vector2(428, 0), 42, 52, 9f, Hex("17465B"));
        refs.progressLine = Line("RingProgress", progress.transform, BuildProgressPoints(0.42f), 9f, Cyan);
    }

    private static IList<Vector2> BuildProgressPoints(float progress)
    {
        const int segments = 52;
        var points = new List<Vector2>();
        int count = Mathf.Max(1, Mathf.RoundToInt(segments * Mathf.Clamp01(progress)));
        for (int i = 0; i <= count; i++)
        {
            float angle = Mathf.PI * 0.5f - Mathf.PI * 2f * i / segments;
            points.Add(new Vector2(428, 0) + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 42f);
        }
        return points;
    }

    private static void CreateBottomNavigation(Transform parent, BuildReferences refs)
    {
        string[] labels = { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };
        for (int i = 0; i < labels.Length; i++)
        {
            float centerX = Dimension1SharedLayoutTokens.NavigationX +
                Dimension1SharedLayoutTokens.NavigationCardX(i) +
                Dimension1SharedLayoutTokens.NavigationCardWidth * 0.5f - Width * 0.5f;
            float centerY = Height * 0.5f -
                (Dimension1SharedLayoutTokens.NavigationY +
                 Dimension1SharedLayoutTokens.NavigationCardY +
                 Dimension1SharedLayoutTokens.NavigationCardHeight * 0.5f);
            GameObject card = FramedPanel("Nav_" + labels[i], parent,
                new Vector2(centerX, centerY),
                new Vector2(Dimension1SharedLayoutTokens.NavigationCardWidth,
                    Dimension1SharedLayoutTokens.NavigationCardHeight),
                PanelBase, CyanMuted, 0.72f);
            refs.navigationCards.Add(card);
            Button button = card.AddComponent<Button>();
            button.targetGraphic = card.GetComponent<Image>();
            if (i == 0)
            {
                Image icon = CreateImage("Icon", card.transform, galaxyIcon, new Vector2(0, 29), new Vector2(83, 78), Cyan);
                icon.preserveAspect = true;
                refs.galaxyButtons.Add(button);
            }
            else if (i == 1)
            {
                CreateRadarGlyph(card.transform, new Vector2(0, 29), 34, Cyan);
                refs.exploreButtons.Add(button);
            }
            else if (i == 2)
            {
                CreateShipGlyph(card.transform, new Vector2(0, 29), 0.78f, Cyan);
                refs.hangarButtons.Add(button);
            }
            else if (i == 3)
            {
                CreateRelicGlyph(card.transform, new Vector2(0, 29), 0.78f, Cyan);
                refs.relicButtons.Add(button);
            }
            else
            {
                CreateTreeGlyph(card.transform, new Vector2(0, 29), 39, Cyan);
                refs.treeButtons.Add(button);
            }
            Text("Label", card.transform, labels[i], new Vector2(0, -47),
                new Vector2(181, 34), 22, Cyan, TextAnchor.MiddleCenter);
        }

        GameObject drawerToggle = FramedPanel("DimensionDrawerToggle", parent,
            new Vector2(0, DrawerToggleY), new Vector2(DrawerToggleWidth, DrawerToggleHeight),
            Hex("020D14", 0.98f), CyanMuted, 0.78f);
        refs.dimensionDrawerToggle = drawerToggle.AddComponent<Button>();
        refs.dimensionDrawerToggle.targetGraphic = drawerToggle.GetComponent<Image>();
        refs.dimensionDrawerToggleLabel = Text("Label", drawerToggle.transform,
            "DIMENSIONES ▼", Vector2.zero, new Vector2(330, 48),
            18, Cyan, TextAnchor.MiddleCenter, FontStyle.Bold);
    }

    private static void CreateMetalsDrawer(Transform parent, BuildReferences refs)
    {
        GameObject drawer = FramedPanel("MetalsDrawer", parent, new Vector2(350, 560),
            new Vector2(350, 520), Hex("020D14", 0.995f), Cyan, 0.92f);
        drawer.transform.SetAsLastSibling();
        Text("Title", drawer.transform, "INVENTARIO DE METALES", new Vector2(0, 222),
            new Vector2(310, 42), 23, White, TextAnchor.MiddleCenter, FontStyle.Bold);
        string[] names =
        {
            "HIERRO", "COBRE", "ALUMINIO", "TITANIO", "NÍQUEL",
            "COBALTO", "LITIO", "TUNGSTENO", "PLATINO", "IRIDIO"
        };
        string[] values =
        {
            "5.98M", "3.62M", "4.73M", "2.11M", "4.70M",
            "1.83M", "1.27M", "1.05M", "0.64M", "0.42M"
        };
        for (int i = 0; i < names.Length; i++)
        {
            float y = 172 - i * 39;
            Text("MetalName", drawer.transform, names[i], new Vector2(-80, y),
                new Vector2(160, 30), 17, Steel, TextAnchor.MiddleLeft);
            refs.drawerMetalValues.Add(Text("MetalValue", drawer.transform, values[i],
                new Vector2(95, y), new Vector2(110, 30), 17, White, TextAnchor.MiddleRight));
        }
        Text("Footer", drawer.transform, "TOTAL · 10 METALES", new Vector2(0, -225),
            new Vector2(300, 34), 18, Cyan, TextAnchor.MiddleCenter);
        refs.metalsDrawer = drawer;
    }

    private static void CreateCardIcon(Transform parent, IconKind kind, Vector2 center)
    {
        CreateCardIconTechnicalShell(parent, center);
        Line("HaloTicks", parent, new[]
        {
            center + new Vector2(-70, 0), center + new Vector2(-58, 0),
            center + new Vector2(-52, 0)
        }, 1.5f, Hex("56DFFF", 0.72f));
        Line("HaloTicks", parent, new[]
        {
            center + new Vector2(52, 0), center + new Vector2(58, 0),
            center + new Vector2(70, 0)
        }, 1.5f, Hex("56DFFF", 0.72f));

        switch (kind)
        {
            case IconKind.Radar: CreateSectorCardGlyph(parent, center); break;
            case IconKind.Scanner: CreateScannerCardGlyph(parent, center); break;
            case IconKind.Ship: CreateFleetCardGlyph(parent, center); break;
            case IconKind.Relic: CreateRelicCardGlyph(parent, center); break;
            case IconKind.Tree: CreateTreeCardGlyph(parent, center); break;
            case IconKind.Compass: CreateExpeditionCardGlyph(parent, center); break;
        }
    }

    private static void CreateCardIconTechnicalShell(Transform parent, Vector2 center)
    {
        var shell = new List<Vector2>();
        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.PI * 2f * i / 6f + Mathf.PI * 0.5f;
            shell.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 59f);
        }
        Line("TechnicalShell", parent, shell, 1.15f, Hex("168DB4", 0.58f), true);
        Circle("TechnicalTelemetry", parent, center, 50, 44, 0.9f,
            Hex("168DB4", 0.34f), true);
        Line("ShellTopTicks", parent, new[]
        {
            center + new Vector2(-10, 58), center + new Vector2(-10, 51),
            center + new Vector2(10, 51), center + new Vector2(10, 58)
        }, 1.1f, Hex("56DFFF", 0.52f));
        Line("ShellBottomTicks", parent, new[]
        {
            center + new Vector2(-10, -58), center + new Vector2(-10, -51),
            center + new Vector2(10, -51), center + new Vector2(10, -58)
        }, 1.1f, Hex("56DFFF", 0.52f));
    }

    private static void CreateSectorCardGlyph(Transform parent, Vector2 center)
    {
        Ellipse("SectorOrbitOuter", parent, center, 55, 28, 42, 1.7f,
            Hex("55DFFF", 0.84f));
        Ellipse("SectorOrbitInner", parent, center, 40, 19, 36, 1.05f,
            Hex("55DFFF", 0.48f));
        Circle("SectorWorld", parent, center + new Vector2(-8, 1), 20, 32, 2.4f, Cyan);
        Polygon("SectorWorldShade", parent, new[]
        {
            center + new Vector2(-8, 21), center + new Vector2(4, 13),
            center + new Vector2(8, -4), center + new Vector2(-2, -18),
            center + new Vector2(-20, -10), center + new Vector2(-27, 5)
        }, Hex("0789BA", 0.36f));
        Line("SectorMeridian", parent, new[]
        {
            center + new Vector2(-8, 20), center + new Vector2(-14, 7),
            center + new Vector2(-14, -8), center + new Vector2(-8, -19)
        }, 1.45f, Hex("A9F2FF", 0.78f));
        Line("SectorLatitude", parent, new[]
        {
            center + new Vector2(-23, -2), center + new Vector2(-8, 4),
            center + new Vector2(10, 0)
        }, 1.3f, Hex("A9F2FF", 0.68f));
        Dot("SectorMoon", parent, center + new Vector2(43, 11), 5.2f, CyanBright);
        Circle("SectorMoonLock", parent, center + new Vector2(43, 11), 10, 20,
            1.15f, Hex("8CEAFF", 0.58f));
        Dot("SectorBeacon", parent, center + new Vector2(-46, -17), 3.7f, Amber);
        Dot("SectorWaypoint", parent, center + new Vector2(24, -21), 2.7f, CyanBright);
        Circle("SectorWaypointLock", parent, center + new Vector2(24, -21), 7, 18,
            0.9f, Hex("8CEAFF", 0.42f));
    }

    private static void CreateScannerCardGlyph(Transform parent, Vector2 center)
    {
        Polygon("ScannerEmitterFill", parent, new[]
        {
            center + new Vector2(0, -37), center + new Vector2(15, -11),
            center + new Vector2(0, 5), center + new Vector2(-15, -11)
        }, Hex("0789BA", 0.43f));
        Line("ScannerEmitter", parent, new[]
        {
            center + new Vector2(0, -37), center + new Vector2(15, -11),
            center + new Vector2(0, 5), center + new Vector2(-15, -11)
        }, 2.5f, CyanBright, true);
        Line("ScannerEmitterFacet", parent, new[]
        {
            center + new Vector2(0, -33), center + new Vector2(0, 1),
            center + new Vector2(10, -11), center + new Vector2(0, -20),
            center + new Vector2(-10, -11)
        }, 1.35f, Hex("BDF6FF", 0.78f));
        Line("ScannerConeLeft", parent, new[]
        {
            center + new Vector2(0, 4), center + new Vector2(-38, 36)
        }, 1.6f, Hex("65E5FF", 0.64f));
        Line("ScannerConeRight", parent, new[]
        {
            center + new Vector2(0, 4), center + new Vector2(38, 36)
        }, 1.6f, Hex("65E5FF", 0.64f));
        Ellipse("ScannerPulseNear", parent, center + new Vector2(0, 14), 26, 9, 30,
            1.45f, Hex("9AF0FF", 0.76f));
        Ellipse("ScannerPulseMid", parent, center + new Vector2(0, 24), 35, 12, 34,
            1.25f, Hex("9AF0FF", 0.58f));
        Ellipse("ScannerPulseFar", parent, center + new Vector2(0, 34), 45, 16, 38,
            1.75f, Cyan);
        Dot("ScannerSignalA", parent, center + new Vector2(-30, 29), 3.5f, CyanBright);
        Dot("ScannerSignalB", parent, center + new Vector2(24, 22), 3f, Cyan);
        Dot("ScannerSignalC", parent, center + new Vector2(4, 40), 2.5f, Amber);
        Line("ScannerBase", parent, new[]
        {
            center + new Vector2(-25, -43), center + new Vector2(25, -43)
        }, 2.2f, Cyan);
        Line("ScannerBaseInner", parent, new[]
        {
            center + new Vector2(-14, -48), center + new Vector2(14, -48)
        }, 1.3f, CyanBright);
    }

    private static void CreateFleetCardGlyph(Transform parent, Vector2 center)
    {
        CreateShipGlyph(parent, center + new Vector2(0, 10), 0.70f, CyanBright);
        CreateShipGlyph(parent, center + new Vector2(-38, -21), 0.36f, Cyan);
        CreateShipGlyph(parent, center + new Vector2(38, -21), 0.36f, Cyan);
        Line("FleetLinkLeft", parent, new[]
        {
            center + new Vector2(-31, -8), center + new Vector2(-15, 2)
        }, 1.35f, Hex("55DFFF", 0.58f), true);
        Line("FleetLinkRight", parent, new[]
        {
            center + new Vector2(31, -8), center + new Vector2(15, 2)
        }, 1.35f, Hex("55DFFF", 0.58f), true);
        Line("FleetFormationLeft", parent, new[]
        {
            center + new Vector2(-48, 16), center + new Vector2(-29, 37),
            center + new Vector2(-17, 43)
        }, 1.15f, Hex("8CEAFF", 0.48f), true);
        Line("FleetFormationRight", parent, new[]
        {
            center + new Vector2(48, 16), center + new Vector2(29, 37),
            center + new Vector2(17, 43)
        }, 1.15f, Hex("8CEAFF", 0.48f), true);
        Dot("FleetCommandNode", parent, center + new Vector2(0, -39), 3.7f, Amber);
        Dot("FleetTelemetryLeft", parent, center + new Vector2(-55, 0), 2.4f, CyanBright);
        Dot("FleetTelemetryRight", parent, center + new Vector2(55, 0), 2.4f, CyanBright);
    }

    private static void CreateRelicCardGlyph(Transform parent, Vector2 center)
    {
        CreateHexagon(parent, center, 50, Hex("55DFFF", 0.48f), 1.3f);
        CreateRelicGlyph(parent, center + new Vector2(0, 4), 0.84f, CyanBright);
        Ellipse("RelicContainment", parent, center + new Vector2(0, -31),
            43, 13, 38, 1.1f, Hex("8DEBFF", 0.45f));
        Dot("RelicSideNode", parent, center + new Vector2(-50, 0), 3.1f, Cyan);
        Dot("RelicSideNode", parent, center + new Vector2(50, 0), 3.1f, Cyan);
        Line("RelicEnergyLinkLeft", parent, new[]
        {
            center + new Vector2(-50, 0), center + new Vector2(-34, 0)
        }, 1.45f, Hex("8DEBFF", 0.72f));
        Line("RelicEnergyLinkRight", parent, new[]
        {
            center + new Vector2(34, 0), center + new Vector2(50, 0)
        }, 1.45f, Hex("8DEBFF", 0.72f));
        Dot("RelicParticle", parent, center + new Vector2(-28, 31), 1.9f, CyanBright);
        Dot("RelicParticle", parent, center + new Vector2(30, 29), 1.9f, CyanBright);
        Dot("RelicParticle", parent, center + new Vector2(-32, -29), 1.7f, Cyan);
        Dot("RelicParticle", parent, center + new Vector2(34, -27), 1.7f, Cyan);
    }

    private static void CreateTreeCardGlyph(Transform parent, Vector2 center)
    {
        CreateTreeGlyph(parent, center + new Vector2(0, 1), 46, Cyan);
        Circle("TreeNetworkOuter", parent, center + new Vector2(0, 5), 50, 42,
            1.05f, Hex("55DFFF", 0.38f), true);
        Circle("TreeNetworkInner", parent, center + new Vector2(0, 5), 34, 36,
            0.85f, Hex("55DFFF", 0.28f));
        Line("TreeSecondaryBranchLeft", parent, new[]
        {
            center + new Vector2(-15, 4), center + new Vector2(-30, -5),
            center + new Vector2(-43, -18)
        }, 1.25f, Hex("8DEBFF", 0.65f));
        Line("TreeSecondaryBranchRight", parent, new[]
        {
            center + new Vector2(15, 8), center + new Vector2(31, -2),
            center + new Vector2(44, -15)
        }, 1.25f, Hex("8DEBFF", 0.65f));
        Dot("TreeSecondaryNode", parent, center + new Vector2(-43, -18), 2.8f, CyanBright);
        Dot("TreeSecondaryNode", parent, center + new Vector2(44, -15), 2.8f, CyanBright);
        Line("TreeDataBase", parent, new[]
        {
            center + new Vector2(-42, -40), center + new Vector2(-20, -46),
            center + new Vector2(0, -40), center + new Vector2(20, -46),
            center + new Vector2(42, -40)
        }, 1.55f, Hex("8DEBFF", 0.70f));
        Dot("TreeRootNode", parent, center + new Vector2(-42, -40), 2.8f, CyanBright);
        Dot("TreeRootNode", parent, center + new Vector2(42, -40), 2.8f, CyanBright);
        Dot("TreeTierCore", parent, center + new Vector2(0, 4), 3.2f, Amber);
    }

    private static void CreateExpeditionCardGlyph(Transform parent, Vector2 center)
    {
        Circle("ExpeditionRouteOuter", parent, center, 51, 46, 1.8f,
            Hex("55DFFF", 0.76f), true);
        Circle("ExpeditionRouteInner", parent, center, 40, 40, 0.95f,
            Hex("55DFFF", 0.36f));
        CreateShipGlyph(parent, center + new Vector2(0, 7), 0.52f, CyanBright);
        Dot("RouteStart", parent, center + new Vector2(-40, 28), 4.2f, Amber);
        Circle("RouteStartLock", parent, center + new Vector2(-40, 28), 8, 18,
            1f, Hex("F5A719", 0.62f));
        Dot("RouteWaypoint", parent, center + new Vector2(42, 18), 3.6f, Cyan);
        Dot("RouteDestination", parent, center + new Vector2(25, -44), 4.6f, CyanBright);
        Circle("RouteDestinationLock", parent, center + new Vector2(25, -44), 9, 20,
            1f, Hex("8DEBFF", 0.55f));
        Line("RouteVector", parent, new[]
        {
            center + new Vector2(-40, 28), center + new Vector2(-9, 39),
            center + new Vector2(28, 30), center + new Vector2(42, 18)
        }, 1.65f, Hex("91EEFF", 0.80f), true);
        Line("RouteReturn", parent, new[]
        {
            center + new Vector2(39, 3), center + new Vector2(39, -19),
            center + new Vector2(25, -44)
        }, 1.15f, Hex("8DEBFF", 0.48f), true);
        Line("RouteArrow", parent, new[]
        {
            center + new Vector2(35, 24), center + new Vector2(42, 18),
            center + new Vector2(32, 15)
        }, 1.8f, CyanBright);
    }

    private static void CreateRadarGlyph(Transform parent, Vector2 center, float radius, Color color)
    {
        Circle("RadarOuter", parent, center, radius, 42, 2f, color);
        Circle("RadarInner", parent, center, radius * 0.66f, 36, 1.3f, Hex("39D7FF", 0.72f));
        Circle("RadarCore", parent, center, radius * 0.23f, 24, 3f, CyanBright);
        Line("RadarH", parent, new[] { center + new Vector2(-radius - 9, 0), center + new Vector2(radius + 9, 0) }, 1.2f, color);
        Line("RadarV", parent, new[] { center + new Vector2(0, -radius - 9), center + new Vector2(0, radius + 9) }, 1.2f, color);
        Line("RadarSweep", parent, new[] { center, center + new Vector2(radius * 0.72f, radius * 0.48f) }, 2f, CyanBright);
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 0.25f;
            Vector2 a = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (radius - 6f);
            Vector2 b = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Line("RadarTick", parent, new[] { a, b }, 1.3f, Hex("91EEFF", 0.78f));
        }
    }

    private static void CreateShipGlyph(Transform parent, Vector2 center, float scale, Color color)
    {
        Vector2[] hull =
        {
            center + new Vector2(0, 48) * scale, center + new Vector2(17, 16) * scale,
            center + new Vector2(43, -22) * scale, center + new Vector2(15, -14) * scale,
            center + new Vector2(0, -42) * scale, center + new Vector2(-15, -14) * scale,
            center + new Vector2(-43, -22) * scale, center + new Vector2(-17, 16) * scale
        };
        Polygon("ShipFill", parent, hull, Hex("079BC9", 0.38f));
        Line("ShipHull", parent, hull, 3f * scale, color, true);
        Line("ShipCockpit", parent, new[]
        {
            center + new Vector2(0, 29) * scale, center + new Vector2(8, 6) * scale,
            center + new Vector2(0, -6) * scale, center + new Vector2(-8, 6) * scale
        }, 2.4f * scale, CyanBright, true);
        Line("ShipSpine", parent, new[]
        {
            center + new Vector2(0, 44) * scale,
            center + new Vector2(0, -35) * scale
        }, 1.8f * scale, Hex("91EEFF", 0.82f));
        Line("ShipWingPanel", parent, new[]
        {
            center + new Vector2(-15, 13) * scale,
            center + new Vector2(-32, -17) * scale,
            center + new Vector2(-15, -10) * scale
        }, 1.6f * scale, Hex("5DDEFF", 0.78f));
        Line("ShipWingPanel", parent, new[]
        {
            center + new Vector2(15, 13) * scale,
            center + new Vector2(32, -17) * scale,
            center + new Vector2(15, -10) * scale
        }, 1.6f * scale, Hex("5DDEFF", 0.78f));
        Dot("Engine", parent, center + new Vector2(-12, -28) * scale,
            3.4f * scale, CyanBright);
        Dot("Engine", parent, center + new Vector2(12, -28) * scale,
            3.4f * scale, CyanBright);
        Line("EngineTrail", parent, new[]
        {
            center + new Vector2(-12, -33) * scale,
            center + new Vector2(-12, -49) * scale
        }, 2f * scale, Hex("38D8FF", 0.62f));
        Line("EngineTrail", parent, new[]
        {
            center + new Vector2(12, -33) * scale,
            center + new Vector2(12, -49) * scale
        }, 2f * scale, Hex("38D8FF", 0.62f));
    }

    private static void CreateRelicGlyph(Transform parent, Vector2 center, float scale, Color color)
    {
        Vector2[] body =
        {
            center + new Vector2(0, 51) * scale, center + new Vector2(16, 23) * scale,
            center + new Vector2(12, -19) * scale, center + new Vector2(0, -43) * scale,
            center + new Vector2(-12, -19) * scale, center + new Vector2(-16, 23) * scale
        };
        Polygon("RelicFill", parent, body, Hex("087EB5", 0.35f));
        Line("RelicBody", parent, body, 3f * scale, color, true);
        Line("RelicSpine", parent, new[]
        {
            center + new Vector2(0, 43) * scale, center + new Vector2(0, -40) * scale
        }, 2f * scale, CyanBright);
        Line("RelicFacet", parent, new[]
        {
            center + new Vector2(0, 45) * scale,
            center + new Vector2(-12, 18) * scale,
            center + new Vector2(0, 4) * scale,
            center + new Vector2(12, 18) * scale,
            center + new Vector2(0, 45) * scale
        }, 1.7f * scale, Hex("BDF6FF", 0.88f));
        Line("RelicFacet", parent, new[]
        {
            center + new Vector2(-11, -17) * scale,
            center + new Vector2(0, 4) * scale,
            center + new Vector2(11, -17) * scale
        }, 1.7f * scale, Hex("64DFFF", 0.78f));
        Ellipse("RelicOrbitOuter", parent, center + new Vector2(0, -34) * scale,
            45 * scale, 13 * scale, 38, 1.8f * scale, color);
        Ellipse("RelicOrbitInner", parent, center + new Vector2(0, -34) * scale,
            27 * scale, 7 * scale, 32, 1.4f * scale, CyanBright);
        Dot("RelicCore", parent, center + new Vector2(0, 4) * scale,
            4.2f * scale, CyanBright);
    }

    private static void CreateTreeGlyph(Transform parent, Vector2 center, float radius, Color color)
    {
        float s = radius / 53f;
        Vector2 basePoint = center + new Vector2(0, -42) * s;
        Line("TreeTrunk", parent, new[]
        {
            basePoint, center + new Vector2(-3, -12) * s,
            center + new Vector2(3, 15) * s, center + new Vector2(0, 46) * s
        }, 3.2f, color);
        Vector2[][] branches =
        {
            new[] { center + new Vector2(-1, -17) * s, center + new Vector2(-22, -1) * s, center + new Vector2(-43, 4) * s },
            new[] { center + new Vector2(1, -9) * s, center + new Vector2(24, 4) * s, center + new Vector2(44, 12) * s },
            new[] { center + new Vector2(1, 2) * s, center + new Vector2(-21, 19) * s, center + new Vector2(-38, 30) * s },
            new[] { center + new Vector2(2, 11) * s, center + new Vector2(23, 25) * s, center + new Vector2(37, 38) * s }
        };
        foreach (Vector2[] branch in branches)
        {
            Line("TreeBranch", parent, branch, 2f, color);
            Vector2 node = branch[branch.Length - 1];
            Dot("TreeNode", parent, node, 3f * s, CyanBright);
            Circle("TreeNodeOrbit", parent, node, 7f * s, 18, 1.1f,
                Hex("91EEFF", 0.62f));
        }
        Dot("TreeCore", parent, center + new Vector2(0, 1) * s, 4f * s, CyanBright);
        Line("TreeRoots", parent, new[]
        {
            basePoint + new Vector2(-37, -7) * s, basePoint,
            basePoint + new Vector2(37, -7) * s
        }, 2.4f, color);
    }

    private static void CreateCompassGlyph(Transform parent, Vector2 center, float radius, Color color)
    {
        Circle("CompassRing", parent, center, radius, 44, 2f, color);
        Circle("CompassInner", parent, center, radius * 0.58f, 36, 1.2f,
            Hex("64DFFF", 0.62f), true);
        Vector2[] star =
        {
            center + new Vector2(0, radius * 0.72f), center + new Vector2(8, 8),
            center + new Vector2(radius * 0.72f, 0), center + new Vector2(8, -8),
            center + new Vector2(0, -radius * 0.72f), center + new Vector2(-8, -8),
            center + new Vector2(-radius * 0.72f, 0), center + new Vector2(-8, 8)
        };
        Line("CompassStar", parent, star, 2.5f, CyanBright, true);
        Dot("CompassCore", parent, center, 4.2f, CyanBright);
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 0.25f;
            Vector2 a = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (radius - 7f);
            Vector2 b = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (radius + 1f);
            Line("CompassTick", parent, new[] { a, b }, 1.4f,
                Hex("91EEFF", i % 2 == 0 ? 0.9f : 0.55f));
        }
        Dot("RouteNode", parent, center + new Vector2(-33, 23), 3.4f, Cyan);
        Dot("RouteNode", parent, center + new Vector2(30, -25), 3.4f, CyanBright);
        Line("RoutePath", parent, new[]
        {
            center + new Vector2(-33, 23), center + new Vector2(-8, 13),
            center + new Vector2(11, -9), center + new Vector2(30, -25)
        }, 1.5f, Hex("5DDEFF", 0.75f), false);
    }

    private static void CreateCommandGlyph(Transform parent)
    {
        Color c = Hex("36CFFF", 0.78f);
        Line("CommandOuter", parent, new[]
        {
            new Vector2(0, 50), new Vector2(45, 25), new Vector2(45, -26),
            new Vector2(0, -52), new Vector2(-45, -26), new Vector2(-45, 25)
        }, 9f, c, true);
        Line("CommandInner", parent, new[]
        {
            new Vector2(-27, -24), new Vector2(-27, 15), new Vector2(0, 32),
            new Vector2(28, 16), new Vector2(28, -24), new Vector2(7, -35),
            new Vector2(7, 5), new Vector2(-8, 5), new Vector2(-8, -26)
        }, 8f, c);
    }

    private static void CreateHexagon(Transform parent, Vector2 center, float radius, Color color, float thickness)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < 6; i++)
        {
            float a = Mathf.PI * 2f * i / 6f + Mathf.PI * 0.5f;
            points.Add(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius);
        }
        Line("Hexagon", parent, points, thickness, color, true);
    }

    private static void CreateMetalGlyph(Transform parent, Vector2 position, float radius, int variant)
    {
        Color metal = variant == 0 ? Hex("BFCAD2") : variant == 1 ? Hex("D8E4EA") : Hex("C6C19A");
        if (variant == 1)
        {
            Vector2[] ingot =
            {
                position + new Vector2(-16, -21), position + new Vector2(7, -24),
                position + new Vector2(21, 18), position + new Vector2(-3, 21)
            };
            Line("Ingot", parent, ingot, 4f, metal, true);
            Line("IngotFacet", parent, new[] { ingot[0], position + new Vector2(1, 10), ingot[2] }, 2f, CyanMuted);
            return;
        }
        for (int i = 0; i < 3; i++)
        {
            Vector2 c = position + new Vector2((i - 1) * 15, i == 1 ? 9 : -4);
            float r = radius * (i == 1 ? 0.62f : 0.48f);
            Vector2[] ore =
            {
                c + new Vector2(-r, -r * 0.35f), c + new Vector2(-r * 0.35f, r),
                c + new Vector2(r * 0.7f, r * 0.6f), c + new Vector2(r, -r * 0.55f),
                c + new Vector2(0, -r)
            };
            Line("Ore", parent, ore, 2.5f, metal, true);
            Line("OreFacet", parent, new[] { ore[1], c, ore[3] }, 1.3f, CyanMuted);
        }
    }

    private static GameObject FramedPanel(string name, Transform parent, Vector2 position,
        Vector2 size, Color fill, Color border, float frameAlpha)
    {
        GameObject panel = Panel(name, parent, position, size, fill);
        Image borderImage = CreateImage(name + "_Border", panel.transform, frame, Vector2.zero, size, border);
        borderImage.type = Image.Type.Sliced;
        Color borderColor = border;
        borderColor.a *= Mathf.Clamp01(frameAlpha);
        borderImage.color = borderColor;
        borderImage.raycastTarget = false;
        Image inner = CreateImage(name + "_InnerBorder", panel.transform, frame, Vector2.zero,
            size - new Vector2(13, 13), Hex("1ACBFF", 0.18f));
        inner.type = Image.Type.Sliced;
        inner.raycastTarget = false;
        return panel;
    }

    private static GameObject Panel(string name, Transform parent, Vector2 position,
        Vector2 size, Color color, Sprite sprite = null)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        Image image = go.GetComponent<Image>();
        image.color = color;
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        return go;
    }

    private static Image CreateImage(string name, Transform parent, Sprite sprite, Vector2 position,
        Vector2 size, Color color)
    {
        return Panel(name, parent, position, size, color, sprite).GetComponent<Image>();
    }

    private static void EnsureSpriteImport(string assetPath)
    {
        string absolutePath = Path.GetFullPath(assetPath);
        if (!File.Exists(absolutePath))
            throw new InvalidOperationException("No existe el asset requerido: " + assetPath);

        AssetDatabase.ImportAsset(assetPath,
            ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
            throw new InvalidOperationException("No se pudo importar como textura: " + assetPath);

        bool changed = importer.textureType != TextureImporterType.Sprite ||
            importer.spriteImportMode != SpriteImportMode.Single ||
            !importer.alphaIsTransparency || importer.mipmapEnabled ||
            importer.wrapMode != TextureWrapMode.Clamp ||
            importer.textureCompression != TextureImporterCompression.Uncompressed ||
            importer.maxTextureSize != 512;

        if (!changed)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 512;
        importer.SaveAndReimport();
    }

    private static Text Text(string name, Transform parent, string value, Vector2 position,
        Vector2 size, int fontSize, Color color, TextAnchor anchor,
        FontStyle fontStyle = FontStyle.Normal)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        Text text = go.GetComponent<Text>();
        text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.lineSpacing = 0.88f;
        text.raycastTarget = false;
        return text;
    }

    private static Dimension1CommandCenterLineGraphic Line(string name, Transform parent,
        IList<Vector2> points, float thickness, Color color, bool closed = false)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Dimension1CommandCenterLineGraphic));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(Width, Height);
        rect.anchoredPosition = Vector2.zero;
        var line = go.GetComponent<Dimension1CommandCenterLineGraphic>();
        line.color = color;
        line.raycastTarget = false;
        line.SetLine(points, thickness, closed);
        return line;
    }

    private static Dimension1CommandCenterPolygonGraphic Polygon(string name, Transform parent,
        IList<Vector2> points, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Dimension1CommandCenterPolygonGraphic));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(Width, Height);
        rect.anchoredPosition = Vector2.zero;
        var polygon = go.GetComponent<Dimension1CommandCenterPolygonGraphic>();
        polygon.color = color;
        polygon.raycastTarget = false;
        polygon.SetPolygon(points);
        return polygon;
    }

    private static void Circle(string name, Transform parent, Vector2 center, float radius,
        int segments, float thickness, Color color, bool dotted = false)
    {
        if (dotted)
        {
            for (int i = 0; i < segments; i += 2)
            {
                float a = Mathf.PI * 2f * i / segments;
                Dot(name, parent, center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius,
                    thickness, color);
            }
            return;
        }
        var points = new List<Vector2>();
        for (int i = 0; i < segments; i++)
        {
            float a = Mathf.PI * 2f * i / segments;
            points.Add(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius);
        }
        Line(name, parent, points, thickness, color, true);
    }

    private static void Ellipse(string name, Transform parent, Vector2 center, float radiusX,
        float radiusY, int segments, float thickness, Color color)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < segments; i++)
        {
            float a = Mathf.PI * 2f * i / segments;
            points.Add(center + new Vector2(Mathf.Cos(a) * radiusX, Mathf.Sin(a) * radiusY));
        }
        Line(name, parent, points, thickness, color, true);
    }

    private static void Dot(string name, Transform parent, Vector2 position, float radius, Color color)
    {
        Image dot = CreateImage(name, parent, null, position, Vector2.one * radius * 2f, color);
        dot.raycastTarget = false;
    }

    private static Button FindButton(Transform parent, string name)
    {
        Transform child = FindChild(parent, name);
        return child == null ? null : child.GetComponent<Button>();
    }

    private static GameObject[] FindObjects(Transform parent, params string[] names)
    {
        var result = new List<GameObject>();
        foreach (string name in names)
        {
            Transform child = FindChild(parent, name);
            if (child != null) result.Add(child.gameObject);
        }
        return result.ToArray();
    }

    private static GameObject[] FindSceneObjects(Scene scene, params string[] names)
    {
        var result = new List<GameObject>();
        foreach (string name in names)
        {
            Transform child = FindSceneTransform(scene, name);
            if (child != null) result.Add(child.gameObject);
        }
        return result.ToArray();
    }

    private static Transform FindSceneTransform(Scene scene, string name)
    {
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        {
            Transform found = FindChild(sceneRoot.transform, name);
            if (found != null) return found;
        }
        return null;
    }

    private static Transform FindChild(Transform parent, string name)
    {
        if (parent == null) return null;
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static Color Hex(string value, float alpha = 1f)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha;
        return color;
    }
}
#endif
