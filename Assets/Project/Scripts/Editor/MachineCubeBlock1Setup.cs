#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MachineCubeBlock1Setup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";
    private const string FontPath =
        "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";
    private const string UpgradeModuleFramePath =
        "Assets/Project/UI/Vertical/UpgradesPolish/qf_upgrade_module_frame.png";
    private const string UpgradeButtonFramePath =
        "Assets/Project/UI/Vertical/UpgradesPolish/qf_upgrade_button_frame.png";
    private const string ResourceLeIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_le.png";
    private const string ResourceTracesIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_traces.png";
    private const string SelectionRingPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_gauge_progress_ring.png";
    private static readonly string[] EffectIconPaths =
    {
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_le.png",
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_traces.png",
        "Assets/Project/UI/Vertical/Generated/qf_icon_prestige.png",
        "Assets/Project/UI/Vertical/Generated/qf_icon_upgrades.png",
        "Assets/Project/UI/Vertical/GenerationPolish/qf_node_tetraquark.png",
        "Assets/Project/UI/Vertical/Generated/qf_icon_research.png",
        "Assets/Project/UI/Vertical/GenerationPolish/qf_node_modulator.png",
        "Assets/Project/UI/Vertical/Generated/qf_icon_dimension1.png",
        "Assets/Project/UI/Vertical/Generated/qf_icon_dimension3.png",
        "Assets/Project/UI/Vertical/GenerationPolish/qf_node_higgs.png"
    };
    private static readonly string[] FaceTexturePaths =
    {
        "Assets/Project/UI/Vertical/Machine/machine_face_1_reference_v2.png",
        "Assets/Project/UI/Vertical/Machine/machine_face_2_reference_v2.png",
        "Assets/Project/UI/Vertical/Machine/machine_face_3_reference_v2.png",
        "Assets/Project/UI/Vertical/Machine/machine_face_4_reference_v2.png"
    };

    private static readonly Color Background = Hex("010609");
    private static readonly Color Panel = Hex("071017", 244);
    private static readonly Color TextPrimary = Hex("E8EAEC");
    private static readonly Color TextSecondary = Hex("87929B");
    private static readonly Color Selection = Hex("6DD7FF");
    private static readonly Color[] Accents =
    {
        Hex("00C9FF"), Hex("A94CFF"), Hex("F0A018"), Hex("00D7C1")
    };

    [MenuItem("Tools/Quantum Forge/Machine/Configure Block 1 Cube")]
    public static void ConfigureBlock1Cube()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ConfigureTextureImports();

        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "No se encontró MachinePanelUI en Main.unity.");
        GameObject panelObject = panel.gameObject;
        Require(panelObject.name == "MachinePanelRoot",
            "MachinePanelUI no pertenece a MachinePanelRoot.");
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        Require(panelRect != null, "MachinePanelRoot no tiene RectTransform.");
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(-16f, -20f);

        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        TMP_FontAsset font = theme != null && theme.primaryFont != null
            ? theme.primaryFont
            : AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        Require(font != null, "No se encontró la fuente Rajdhani de la UI vertical.");

        Transform oldRoot = panelObject.transform.Find("MachineCubeVisualRoot");
        if (oldRoot != null)
            UnityEngine.Object.DestroyImmediate(oldRoot.gameObject);

        MachineCubeVisualUI visual = panelObject.GetComponent<MachineCubeVisualUI>();
        if (visual == null)
            visual = panelObject.AddComponent<MachineCubeVisualUI>();

        GameObject root = CreateRect("MachineCubeVisualRoot", panelObject.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        CanvasGroup interactionGroup = root.AddComponent<CanvasGroup>();

        Image background = root.AddComponent<Image>();
        background.color = Background;
        background.raycastTarget = true;

        if (theme != null && theme.backgroundGrid != null)
        {
            GameObject gridObject = CreateRect("CircuitGrid", root.transform,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Image grid = gridObject.AddComponent<Image>();
            grid.sprite = theme.backgroundGrid;
            grid.type = Image.Type.Tiled;
            grid.color = new Color(0.08f, 0.34f, 0.48f, 0.12f);
            grid.raycastTarget = false;
        }

        Sprite panelSprite = theme != null ? theme.panelFrame : null;
        Sprite buttonSprite = AssetDatabase.LoadAssetAtPath<Sprite>(UpgradeButtonFramePath);
        if (buttonSprite == null && theme != null)
            buttonSprite = theme.buttonFrame;
        Sprite selectedSprite = theme != null ? theme.selectedButtonFrame : null;
        Sprite moduleSprite = AssetDatabase.LoadAssetAtPath<Sprite>(UpgradeModuleFramePath);
        if (moduleSprite == null)
            moduleSprite = panelSprite;
        Sprite leIcon = AssetDatabase.LoadAssetAtPath<Sprite>(ResourceLeIconPath);
        Sprite tracesIcon = AssetDatabase.LoadAssetAtPath<Sprite>(ResourceTracesIconPath);
        Sprite selectionRing = AssetDatabase.LoadAssetAtPath<Sprite>(SelectionRingPath);
        Sprite[] effectIcons = new Sprite[EffectIconPaths.Length];
        for (int i = 0; i < EffectIconPaths.Length; i++)
            effectIcons[i] = AssetDatabase.LoadAssetAtPath<Sprite>(EffectIconPaths[i]);
        Require(moduleSprite != null && buttonSprite != null && leIcon != null &&
            tracesIcon != null && selectionRing != null,
            "Faltan recursos compartidos de Mejoras/Triángulo.");

        HeaderParts header = BuildHeader(root.transform, font, moduleSprite,
            leIcon, tracesIcon);
        for (int i = 0; i < effectIcons.Length; i++)
            Require(effectIcons[i] != null, "Missing shared machine pictogram: " + EffectIconPaths[i]);
        SectorParts sector = BuildSectorHeader(root.transform, font, panelSprite);

        GameObject viewportObject = CreateRect("FaceViewport", root.transform,
            new Vector2(0.005f, 0.245f), new Vector2(0.995f, 0.825f),
            Vector2.zero, Vector2.zero);
        RectTransform viewport = viewportObject.GetComponent<RectTransform>();
        RectMask2D mask = viewportObject.AddComponent<RectMask2D>();
        mask.padding = new Vector4(-8f, -8f, -8f, -8f);
        Image viewportRaycast = viewportObject.AddComponent<Image>();
        viewportRaycast.color = new Color(0f, 0f, 0f, 0.001f);
        viewportRaycast.raycastTarget = true;

        MachineCubeFaceViewUI[] faces = new MachineCubeFaceViewUI[4];
        for (int i = 0; i < 4; i++)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(FaceTexturePaths[i]);
            Require(texture != null, "Falta textura de cara: " + FaceTexturePaths[i]);
            faces[i] = BuildFace(viewport, i, texture, font, selectionRing,
                effectIcons);
        }
        RotationRigParts rotationRig = BuildRotationRig(viewport);

        Image[] faceDots = BuildRotationHint(root.transform, font, selectionRing);

        GameObject guideObject = CreateRect("SelectionGuide", root.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-1.5f, -1.5f), new Vector2(1.5f, 1.5f));
        Image guideImage = guideObject.AddComponent<Image>();
        guideImage.color = new Color(0.82f, 0.94f, 1f, 0.78f);
        guideImage.raycastTarget = false;
        RectTransform guide = guideObject.GetComponent<RectTransform>();

        Button previousFace = BuildArrow(root.transform, "PreviousFaceButton", "<",
            new Vector2(0.000f, 0.42f), new Vector2(0.075f, 0.58f), font, buttonSprite);
        Button nextFace = BuildArrow(root.transform, "NextFaceButton", ">",
            new Vector2(0.925f, 0.42f), new Vector2(1.000f, 0.58f), font, buttonSprite);

        CardParts card = BuildNodeCard(root.transform, font, buttonSprite,
            moduleSprite, effectIcons);

        SerializedObject visualSo = new SerializedObject(visual);
        SetObject(visualSo, "machinePanel", panel);
        SetObject(visualSo, "visualContentRoot", root);
        SetObject(visualSo, "faceViewport", viewport);
        SetObject(visualSo, "interactionGroup", interactionGroup);
        SetObjectArray(visualSo, "faces", faces);
        SetObject(visualSo, "previousFaceButton", previousFace);
        SetObject(visualSo, "nextFaceButton", nextFace);
        SetObject(visualSo, "rotationRigRoot", rotationRig.root);
        SetObject(visualSo, "rotationFromFace", rotationRig.from);
        SetObject(visualSo, "rotationToFace", rotationRig.to);
        SetObject(visualSo, "rotationEdgeShadow", rotationRig.edgeShadow);
        SetObject(visualSo, "rotationEdgeHighlight", rotationRig.edgeHighlight);
        visualSo.FindProperty("rotationDuration").floatValue = 0.52f;
        visualSo.FindProperty("rotationCameraDistance").floatValue = 3.4f;
        visualSo.FindProperty("rotationMidFramingScale").floatValue = 0.92f;
        SetObject(visualSo, "faceIndexText", sector.faceIndex);
        SetObject(visualSo, "faceTitleText", sector.faceTitle);
        SetObject(visualSo, "leResourceText", header.le);
        SetObject(visualSo, "tracesResourceText", header.traces);
        SetObject(visualSo, "globalProgressText", header.progress);
        SetObject(visualSo, "convergenceText", header.convergence);
        SetObject(visualSo, "globalProgressFill", header.progressFill);
        SetObjectArray(visualSo, "faceDots", faceDots);
        SetObject(visualSo, "selectionGuide", guide);
        SetObject(visualSo, "selectedCardRect", card.root);
        SetObject(visualSo, "selectedNameText", card.name);
        SetObject(visualSo, "selectedIconText", card.icon);
        SetObject(visualSo, "selectedIconImage", card.pictogram);
        SetObject(visualSo, "selectedStateText", card.state);
        SetObject(visualSo, "selectedDescriptionText", card.description);
        SetObject(visualSo, "selectedEffectText", card.effect);
        SetObject(visualSo, "selectedCostText", card.cost);
        SetObject(visualSo, "selectedRequirementsText", card.requirements);
        SetObject(visualSo, "selectedFaceProgressText", card.faceProgress);
        SetIconSprites(visualSo, effectIcons);
        visualSo.ApplyModifiedPropertiesWithoutUndo();

        MachineCubeSwipeSurface swipe = viewportObject.AddComponent<MachineCubeSwipeSurface>();
        SerializedObject swipeSo = new SerializedObject(swipe);
        SetObject(swipeSo, "controller", visual);
        swipeSo.FindProperty("thresholdPixels").floatValue = 64f;
        swipeSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject panelSo = new SerializedObject(panel);
        SetObject(panelSo, "cubeVisual", visual);
        SetObject(panelSo, "btnPrevNode", null);
        SetObject(panelSo, "btnNextNode", null);
        SetObject(panelSo, "btnRepairNode", card.repair);
        SetObject(panelSo, "btnAnalyzeNode", card.analyze);
        SetObject(panelSo, "btnFusionPanel", card.fusion);
        SetObject(panelSo, "btnOpenSeedsPanel", card.seeds);
        SetObject(panelSo, "btnZone1", null);
        SetObject(panelSo, "btnZone2", null);
        SetObject(panelSo, "btnZone3", null);
        SetObject(panelSo, "btnZone4", null);
        panelSo.ApplyModifiedPropertiesWithoutUndo();

        DisableLegacyRoot(panelObject.transform, "MachineRepairViewRoot");
        DisableLegacyRoot(panelObject.transform, "MachineNodeViewRoot");
        DisableLegacyRoot(panelObject.transform, "LegacyFusionPanel");
        DisableLegacyRoot(panelObject.transform, "BtnFusionPanel");
        DisableLegacyRoot(panelObject.transform, "BtnBackToNodesFromFusion");
        DisableLegacyRoot(panelObject.transform, "MachineDiagnostics_Text");
        DisableLegacyRoot(panelObject.transform, "BtnAnalyzeNode");
        DisableLegacyRoot(panelObject.transform, "NodeAnalysis_Text");
        DisableLegacyRoot(panelObject.transform, "BtnInstantHelp");

        EditorUtility.SetDirty(panelObject);
        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Machine Cube Block 1] CONFIGURED | 4 faces | swipe | native node card");
    }

    public static void ConfigureBlock1CubeBatch()
    {
        ConfigureBlock1Cube();
    }

    private static void ConfigureTextureImports()
    {
        foreach (string path in FaceTexturePaths)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Require(importer != null, "No se pudo importar " + path);
            importer.textureType = TextureImporterType.Default;
            importer.mipmapEnabled = false;
            importer.sRGBTexture = true;
            importer.alphaIsTransparency = false;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SaveAndReimport();
        }
    }

    private static HeaderParts BuildHeader(Transform parent, TMP_FontAsset font,
        Sprite moduleSprite, Sprite leIcon, Sprite tracesIcon)
    {
        GameObject header = CreateRect("MachineHeader", parent,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject lePanel = CreatePanel("MachineResource_LE", header.transform,
            new Vector2(0.055f, 0.946f), new Vector2(0.492f, 0.995f), moduleSprite,
            new Color(0.42f, 0.46f, 0.48f, 0.96f));
        AddIcon("Icon", lePanel.transform, leIcon,
            new Vector2(0.07f, 0.12f), new Vector2(0.25f, 0.88f));

        GameObject tracesPanel = CreatePanel("MachineResource_Traces", header.transform,
            new Vector2(0.508f, 0.946f), new Vector2(0.945f, 0.995f), moduleSprite,
            new Color(0.42f, 0.46f, 0.48f, 0.96f));
        AddIcon("Icon", tracesPanel.transform, tracesIcon,
            new Vector2(0.07f, 0.12f), new Vector2(0.25f, 0.88f));

        GameObject titlePanel = CreatePanel("MachineTitlePlate", header.transform,
            new Vector2(0.055f, 0.884f), new Vector2(0.945f, 0.942f), moduleSprite,
            new Color(0.50f, 0.53f, 0.54f, 0.98f));

        TextMeshProUGUI title = CreateText("Title", titlePanel.transform, "MÁQUINA",
            new Vector2(0.25f, 0.24f), new Vector2(0.75f, 0.96f), font, 48f,
            TextPrimary, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        title.characterSpacing = 5f;

        AddTitleLight(titlePanel.transform, "LeftLight", 0.08f, 0.27f);
        AddTitleLight(titlePanel.transform, "RightLight", 0.73f, 0.92f);

        TextMeshProUGUI le = CreateText("LE", lePanel.transform, "LE  —",
            new Vector2(0.27f, 0.10f), new Vector2(0.94f, 0.90f), font, 24f,
            TextPrimary, TextAlignmentOptions.Left);
        TextMeshProUGUI traces = CreateText("Traces", tracesPanel.transform, "TRAZAS  —",
            new Vector2(0.27f, 0.10f), new Vector2(0.94f, 0.90f), font, 24f,
            TextPrimary, TextAlignmentOptions.Left);

        TextMeshProUGUI progress = CreateText("TotalProgress", titlePanel.transform,
            "REPARACIÓN TOTAL  0%  /  80%",
            new Vector2(0.035f, 0.015f), new Vector2(0.61f, 0.25f), font, 16f,
            TextSecondary, TextAlignmentOptions.Left);
        TextMeshProUGUI convergence = CreateText("Convergence", titlePanel.transform,
            "CANAL: BLOQUEADO", new Vector2(0.61f, 0.015f), new Vector2(0.965f, 0.25f),
            font, 16f, TextSecondary, TextAlignmentOptions.Right);

        GameObject track = CreateRect("ProgressTrack", titlePanel.transform,
            new Vector2(0.035f, 0.006f), new Vector2(0.965f, 0.025f),
            Vector2.zero, Vector2.zero);
        Image trackImage = track.AddComponent<Image>();
        trackImage.color = Hex("17242B");
        trackImage.raycastTarget = false;
        GameObject fillObject = CreateRect("Fill", track.transform, Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero);
        Image fill = fillObject.AddComponent<Image>();
        fill.color = Accents[1];
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = 0f;
        fill.raycastTarget = false;

        return new HeaderParts { le = le, traces = traces, progress = progress,
            convergence = convergence, progressFill = fill };
    }

    private static void AddTitleLight(Transform parent, string name,
        float anchorMinX, float anchorMaxX)
    {
        GameObject lightObject = CreateRect(name, parent,
            new Vector2(anchorMinX, 0.54f), new Vector2(anchorMaxX, 0.57f),
            Vector2.zero, Vector2.zero);
        Image light = lightObject.AddComponent<Image>();
        light.color = new Color(0.78f, 0.91f, 1f, 0.72f);
        light.raycastTarget = false;
    }

    private static SectorParts BuildSectorHeader(Transform parent, TMP_FontAsset font,
        Sprite panelSprite)
    {
        GameObject sector = CreateRect("MachineSectorHeader", parent,
            new Vector2(0.10f, 0.825f), new Vector2(0.90f, 0.884f),
            Vector2.zero, Vector2.zero);
        TextMeshProUGUI faceTitle = CreateText("FaceTitle", sector.transform,
            "SECTOR DE FUSIONES", new Vector2(0.04f, 0.38f), new Vector2(0.96f, 0.98f),
            font, 34f, TextPrimary, TextAlignmentOptions.Center);
        faceTitle.fontStyle = FontStyles.Bold;
        faceTitle.characterSpacing = 2f;
        TextMeshProUGUI faceIndex = CreateText("FaceIndex", sector.transform,
            "CARA 2 / 4", new Vector2(0.04f, 0.01f), new Vector2(0.96f, 0.43f),
            font, 24f, Accents[1], TextAlignmentOptions.Center);
        return new SectorParts { faceTitle = faceTitle, faceIndex = faceIndex };
    }

    private static MachineCubeFaceViewUI BuildFace(RectTransform parent, int index,
        Texture2D texture, TMP_FontAsset font, Sprite selectionRing,
        Sprite[] effectIcons)
    {
        GameObject faceObject = CreateRect("MachineFace_" + (index + 1), parent,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        CanvasGroup group = faceObject.AddComponent<CanvasGroup>();

        GameObject square = CreateRect("BoardSquare", faceObject.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        square.AddComponent<RectMask2D>();
        AspectRatioFitter fitter = square.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = 1f;

        GameObject artObject = CreateRect("BaseArtwork", square.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        RawImage art = artObject.AddComponent<RawImage>();
        art.texture = texture;
        art.color = Color.white;
        art.raycastTarget = false;

        GameObject circuitObject = CreateRect("ValidatedCircuits", square.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        RectTransform circuitLayer = circuitObject.GetComponent<RectTransform>();

        Vector2[] positions = GetNodePositions(index);
        MachineCubeNodeVisualUI[] slots = new MachineCubeNodeVisualUI[positions.Length];
        for (int i = 0; i < positions.Length; i++)
            slots[i] = BuildNodeSlot(square.transform, i, positions[i], font,
                selectionRing, Accents[index], effectIcons);

        TextMeshProUGUI progress = CreateText("FaceProgress", faceObject.transform,
            "REPARACIÓN DE CARA  0%", new Vector2(0.20f, 0.015f),
            new Vector2(0.80f, 0.075f), font, 22f, Accents[index],
            TextAlignmentOptions.Center);
        progress.fontStyle = FontStyles.Bold;
        progress.gameObject.SetActive(false);

        MachineCubeFaceViewUI face = faceObject.AddComponent<MachineCubeFaceViewUI>();
        SerializedObject so = new SerializedObject(face);
        so.FindProperty("zone").enumValueIndex = index + 1;
        SetObject(so, "faceRect", faceObject.GetComponent<RectTransform>());
        SetObject(so, "canvasGroup", group);
        SetObject(so, "progressText", progress);
        SetObject(so, "baseArtwork", art);
        SetObject(so, "circuitLayer", circuitLayer);
        SetObjectArray(so, "nodeSlots", slots);
        so.ApplyModifiedPropertiesWithoutUndo();
        faceObject.SetActive(index == 1);
        return face;
    }

    private static RotationRigParts BuildRotationRig(RectTransform parent)
    {
        GameObject root = CreateRect("PhysicalRotationRig", parent,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject fromObject = CreateRect("OutgoingPerspectiveFace", root.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        MachineCubePerspectiveFaceGraphic from =
            fromObject.AddComponent<MachineCubePerspectiveFaceGraphic>();
        from.color = Color.white;
        from.raycastTarget = false;

        GameObject toObject = CreateRect("IncomingPerspectiveFace", root.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        MachineCubePerspectiveFaceGraphic to =
            toObject.AddComponent<MachineCubePerspectiveFaceGraphic>();
        to.color = Color.white;
        to.raycastTarget = false;

        GameObject shadowObject = CreateRect("SharedEdgeShadow", root.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-9f, -200f), new Vector2(9f, 200f));
        Image shadow = shadowObject.AddComponent<Image>();
        shadow.color = new Color(0.005f, 0.008f, 0.01f, 0.78f);
        shadow.raycastTarget = false;

        GameObject highlightObject = CreateRect("SharedEdgeHighlight", shadowObject.transform,
            new Vector2(0.5f, 0f), new Vector2(0.5f, 1f),
            new Vector2(-1.5f, 0f), new Vector2(1.5f, 0f));
        Image highlight = highlightObject.AddComponent<Image>();
        highlight.color = new Color(0.42f, 0.46f, 0.47f, 0.68f);
        highlight.raycastTarget = false;

        root.SetActive(false);
        return new RotationRigParts
        {
            root = root,
            from = from,
            to = to,
            edgeShadow = shadowObject.GetComponent<RectTransform>(),
            edgeHighlight = highlight
        };
    }

    private static Image[] BuildRotationHint(Transform parent, TMP_FontAsset font,
        Sprite ringSprite)
    {
        GameObject arcObject = CreateRect("RotationArc", parent,
            new Vector2(0.12f, 0.178f), new Vector2(0.88f, 0.267f),
            Vector2.zero, Vector2.zero);
        Image arc = arcObject.AddComponent<Image>();
        arc.sprite = ringSprite;
        arc.preserveAspect = false;
        arc.color = new Color(0.48f, 0.58f, 0.62f, 0.22f);
        arc.raycastTarget = false;
        arcObject.transform.SetSiblingIndex(Mathf.Min(1, parent.childCount - 1));

        TextMeshProUGUI hint = CreateText("SwipeHint", parent, "DESLIZA PARA ROTAR",
            new Vector2(0.28f, 0.216f), new Vector2(0.72f, 0.243f), font, 18f,
            new Color(0.55f, 0.59f, 0.61f, 0.95f), TextAlignmentOptions.Center);
        hint.characterSpacing = 1.5f;

        Image[] dots = new Image[4];
        const float spacing = 0.034f;
        float start = 0.5f - spacing * 1.5f;
        for (int i = 0; i < dots.Length; i++)
        {
            float x = start + spacing * i;
            GameObject dotObject = CreateRect("FaceDot_" + (i + 1), parent,
                new Vector2(x, 0.204f), new Vector2(x, 0.204f),
                new Vector2(-7f, -7f), new Vector2(7f, 7f));
            Image dot = dotObject.AddComponent<Image>();
            dot.sprite = ringSprite;
            dot.preserveAspect = true;
            dot.color = i == 1
                ? new Color(0.86f, 0.95f, 1f, 1f)
                : new Color(0.34f, 0.38f, 0.40f, 0.82f);
            dot.raycastTarget = false;
            dots[i] = dot;
        }
        return dots;
    }

    private static MachineCubeNodeVisualUI BuildNodeSlot(Transform parent, int index,
        Vector2 normalizedPosition, TMP_FontAsset font, Sprite selectionRing,
        Color accent, Sprite[] effectIcons)
    {
        GameObject root = CreateRect("NodeSlot_" + (index + 1).ToString("00"), parent,
            normalizedPosition, normalizedPosition, new Vector2(-44f, -44f),
            new Vector2(44f, 44f));

        GameObject haloObject = CreateRect("SelectionHalo", root.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-58f, -58f), new Vector2(58f, 58f));
        Image halo = haloObject.AddComponent<Image>();
        halo.sprite = selectionRing;
        halo.type = Image.Type.Simple;
        halo.preserveAspect = true;
        halo.color = new Color(0.82f, 0.94f, 1f, 0.92f);
        halo.raycastTarget = false;

        GameObject buttonObject = CreateRect("HitTarget", root.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image frame = buttonObject.AddComponent<Image>();
        frame.sprite = null;
        frame.color = new Color(0f, 0f, 0f, 0.001f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = frame;
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        GameObject pictogramObject = CreateRect("Pictogram", buttonObject.transform,
            new Vector2(0.25f, 0.25f), new Vector2(0.75f, 0.75f),
            Vector2.zero, Vector2.zero);
        Image pictogram = pictogramObject.AddComponent<Image>();
        pictogram.sprite = effectIcons[9];
        pictogram.preserveAspect = true;
        pictogram.color = new Color(0.82f, 0.88f, 0.92f, 0.9f);
        pictogram.raycastTarget = false;

        TextMeshProUGUI tier = CreateText("Tier", buttonObject.transform, "",
            new Vector2(0.58f, 0.03f), new Vector2(0.94f, 0.28f), font, 13f,
            TextPrimary, TextAlignmentOptions.BottomRight);

        MachineCubeNodeVisualUI node = root.AddComponent<MachineCubeNodeVisualUI>();
        SerializedObject so = new SerializedObject(node);
        SetObject(so, "button", button);
        SetObject(so, "frameImage", frame);
        SetObject(so, "haloImage", halo);
        SetObject(so, "pictogramImage", pictogram);
        SetObject(so, "stateGlyphText", null);
        SetObject(so, "tierText", tier);
        SetIconSprites(so, effectIcons);
        so.ApplyModifiedPropertiesWithoutUndo();
        return node;
    }

    private static CardParts BuildNodeCard(Transform parent, TMP_FontAsset font,
        Sprite buttonSprite, Sprite moduleSprite, Sprite[] effectIcons)
    {
        GameObject card = CreatePanel("SelectedNodeCard", parent,
            new Vector2(0.020f, 0.012f), new Vector2(0.980f, 0.188f), moduleSprite,
            new Color(0.46f, 0.50f, 0.52f, 0.96f));

        GameObject iconPlate = CreatePanel("SelectedNodeIcon", card.transform,
            new Vector2(0.022f, 0.16f), new Vector2(0.185f, 0.90f), moduleSprite,
            new Color(0.38f, 0.43f, 0.46f, 0.98f));
        GameObject pictogramObject = CreateRect("Pictogram", iconPlate.transform,
            new Vector2(0.12f, 0.20f), new Vector2(0.88f, 0.86f),
            Vector2.zero, Vector2.zero);
        Image pictogram = pictogramObject.AddComponent<Image>();
        pictogram.sprite = effectIcons[9];
        pictogram.preserveAspect = true;
        pictogram.color = Color.white;
        pictogram.raycastTarget = false;
        TextMeshProUGUI icon = CreateText("EffectCode", iconPlate.transform, "--",
            new Vector2(0.08f, 0.02f), new Vector2(0.92f, 0.24f), font, 18f,
            Selection, TextAlignmentOptions.Center);
        icon.fontStyle = FontStyles.Bold;

        GameObject cardRail = CreateRect("AccentRail", card.transform,
            new Vector2(0.010f, 0.12f), new Vector2(0.014f, 0.88f),
            Vector2.zero, Vector2.zero);
        Image cardRailImage = cardRail.AddComponent<Image>();
        cardRailImage.color = new Color(0.68f, 0.78f, 0.82f, 0.48f);
        cardRailImage.raycastTarget = false;

        TextMeshProUGUI name = CreateText("NodeName", card.transform, "SELECCIONA UN NODO",
            new Vector2(0.205f, 0.75f), new Vector2(0.73f, 0.96f), font, 32f,
            TextPrimary, TextAlignmentOptions.Left);
        name.fontStyle = FontStyles.Bold;
        TextMeshProUGUI state = CreateText("NodeState", card.transform,
            "ESTADO: DESCONOCIDO", new Vector2(0.205f, 0.59f), new Vector2(0.73f, 0.77f),
            font, 22f, Selection, TextAlignmentOptions.Left);
        TextMeshProUGUI description = CreateText("NodeDescription", card.transform,
            "Explora los circuitos de la cara actual.", new Vector2(0.205f, 0.37f),
            new Vector2(0.73f, 0.61f), font, 21f, TextSecondary,
            TextAlignmentOptions.TopLeft);
        description.textWrappingMode = TextWrappingModes.Normal;
        TextMeshProUGUI effect = CreateText("NodeEffect", card.transform, "EFECTO  —",
            new Vector2(0.205f, 0.25f), new Vector2(0.73f, 0.39f), font, 19f,
            TextPrimary, TextAlignmentOptions.Left);
        TextMeshProUGUI requirements = CreateText("NodeRequirements", card.transform,
            "REQUISITOS  —", new Vector2(0.205f, 0.12f), new Vector2(0.73f, 0.27f),
            font, 18f, Hex("CDA15B"), TextAlignmentOptions.Left);
        requirements.textWrappingMode = TextWrappingModes.Normal;
        TextMeshProUGUI cost = CreateText("NodeCost", card.transform, "COSTE  —",
            new Vector2(0.205f, 0.02f), new Vector2(0.73f, 0.14f), font, 18f,
            Hex("9BB7C4"), TextAlignmentOptions.Left);

        Button repair = BuildCardButton(card.transform, "RepairNode", "REPARAR",
            new Vector2(0.755f, 0.54f), new Vector2(0.975f, 0.88f), font, buttonSprite, 24f);
        Button analyze = BuildCardButton(card.transform, "AnalyzeNode", "ANALIZAR",
            new Vector2(0.755f, 0.18f), new Vector2(0.975f, 0.50f), font, buttonSprite, 22f);
        Button fusion = BuildCardButton(card.transform, "OpenFusion", "PANEL DE FUSIÓN",
            new Vector2(0.755f, 0.18f), new Vector2(0.975f, 0.50f), font, buttonSprite, 18f);
        Button seeds = BuildCardButton(card.transform, "OpenAnchors", "ANCLAJES",
            new Vector2(0.755f, 0.18f), new Vector2(0.975f, 0.50f), font, buttonSprite, 19f);
        fusion.gameObject.SetActive(false);
        seeds.gameObject.SetActive(false);

        TextMeshProUGUI faceProgress = CreateText("FaceProgress", card.transform,
            "PROGRESO DEL SECTOR  0%", new Vector2(0.755f, 0.015f),
            new Vector2(0.975f, 0.145f), font, 15f, TextSecondary,
            TextAlignmentOptions.Center);
        faceProgress.fontStyle = FontStyles.Normal;

        return new CardParts { root = card.GetComponent<RectTransform>(), icon = icon,
            pictogram = pictogram,
            name = name,
            state = state, description = description,
            effect = effect, cost = cost, requirements = requirements,
            faceProgress = faceProgress, repair = repair,
            analyze = analyze, fusion = fusion, seeds = seeds };
    }

    private static Button BuildArrow(Transform parent, string name, string glyph,
        Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font, Sprite sprite)
    {
        Button button = BuildCardButton(parent, name, glyph, anchorMin, anchorMax,
            font, sprite, 70f);
        if (button.targetGraphic is Image image)
        {
            image.sprite = null;
            image.color = new Color(0f, 0f, 0f, 0.001f);
        }
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.62f, 0.84f, 1f, 1f);
        button.colors = colors;
        return button;
    }

    private static Button BuildCardButton(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font, Sprite sprite,
        float fontSize)
    {
        GameObject buttonObject = CreateRect(name, parent, anchorMin, anchorMax,
            Vector2.zero, Vector2.zero);
        Image image = buttonObject.AddComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = new Color(0.38f, 0.42f, 0.44f, 0.96f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        TextMeshProUGUI text = CreateText("Label", buttonObject.transform, label,
            new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.94f), font, fontSize,
            TextPrimary, TextAlignmentOptions.Center);
        text.fontStyle = FontStyles.Bold;
        return button;
    }

    private static Vector2[] GetNodePositions(int faceIndex)
    {
        return faceIndex switch
        {
            0 => new[]
            {
                P(.21f,.75f), P(.72f,.74f), P(.20f,.52f), P(.50f,.50f),
                P(.74f,.53f), P(.20f,.28f), P(.71f,.27f), P(.50f,.13f),
                P(.50f,.82f), P(.88f,.48f), P(.10f,.42f), P(.37f,.68f), P(.63f,.34f)
            },
            1 => new[]
            {
                P(.20f,.76f), P(.49f,.77f), P(.74f,.73f), P(.18f,.54f),
                P(.49f,.53f), P(.75f,.53f), P(.18f,.29f), P(.38f,.28f),
                P(.72f,.28f), P(.61f,.43f), P(.48f,.12f), P(.88f,.48f), P(.88f,.77f)
            },
            2 => new[]
            {
                P(.20f,.76f), P(.75f,.77f), P(.20f,.52f), P(.50f,.53f),
                P(.76f,.50f), P(.20f,.27f), P(.76f,.26f), P(.50f,.18f),
                P(.50f,.82f), P(.10f,.42f), P(.89f,.42f), P(.34f,.35f), P(.66f,.35f)
            },
            _ => new[]
            {
                P(.21f,.76f), P(.75f,.76f), P(.22f,.61f), P(.51f,.67f),
                P(.50f,.50f), P(.76f,.51f), P(.18f,.38f), P(.49f,.32f),
                P(.75f,.31f), P(.21f,.18f), P(.40f,.16f), P(.68f,.18f), P(.88f,.46f)
            }
        };
    }

    private static Vector2 P(float x, float y) => new Vector2(x, y);

    private static GameObject CreatePanel(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Sprite sprite, Color color)
    {
        GameObject panel = CreateRect(name, parent, anchorMin, anchorMax,
            Vector2.zero, Vector2.zero);
        Image image = panel.AddComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;
        return panel;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent,
        string value, Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font,
        float fontSize, Color color, TextAlignmentOptions alignment)
    {
        GameObject textObject = CreateRect(name, parent, anchorMin, anchorMax,
            Vector2.zero, Vector2.zero);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.enableAutoSizing = true;
        text.fontSizeMin = Mathf.Max(14f, fontSize * 0.72f);
        text.fontSizeMax = fontSize;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        return text;
    }

    private static void AddIcon(string name, Transform parent, Sprite sprite,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject iconObject = CreateRect(name, parent, anchorMin, anchorMax,
            Vector2.zero, Vector2.zero);
        Image icon = iconObject.AddComponent<Image>();
        icon.sprite = sprite;
        icon.preserveAspect = true;
        icon.color = Color.white;
        icon.raycastTarget = false;
    }

    private static GameObject CreateRect(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = parent.gameObject.layer;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.localScale = Vector3.one;
        return gameObject;
    }

    private static void DisableLegacyRoot(Transform parent, string name)
    {
        Transform target = FindDescendant(parent, name);
        if (target != null)
            target.gameObject.SetActive(false);
    }

    private static Transform FindDescendant(Transform root, string name)
    {
        foreach (Transform child in root)
        {
            if (child.name == name)
                return child;
            Transform nested = FindDescendant(child, name);
            if (nested != null)
                return nested;
        }
        return null;
    }

    private static void SetObject(SerializedObject serializedObject, string property,
        UnityEngine.Object value)
    {
        SerializedProperty target = serializedObject.FindProperty(property);
        Require(target != null, "Falta propiedad serializada: " + property);
        target.objectReferenceValue = value;
    }

    private static void SetObjectArray<T>(SerializedObject serializedObject,
        string property, T[] values) where T : UnityEngine.Object
    {
        SerializedProperty array = serializedObject.FindProperty(property);
        Require(array != null, "Falta arreglo serializado: " + property);
        array.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
            array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private static void SetIconSprites(SerializedObject serializedObject, Sprite[] icons)
    {
        string[] properties =
        {
            "iconLe", "iconTraces", "iconTriangle", "iconArtifact", "iconFusion",
            "iconDiagnostic", "iconStructure", "iconConvergence", "iconAnchor",
            "iconSynthesis"
        };
        Require(icons != null && icons.Length == properties.Length,
            "Machine pictogram set is incomplete.");
        for (int i = 0; i < properties.Length; i++)
            SetObject(serializedObject, properties[i], icons[i]);
    }

    private static Color Hex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private sealed class HeaderParts
    {
        public TextMeshProUGUI le;
        public TextMeshProUGUI traces;
        public TextMeshProUGUI progress;
        public TextMeshProUGUI convergence;
        public Image progressFill;
    }

    private sealed class SectorParts
    {
        public TextMeshProUGUI faceTitle;
        public TextMeshProUGUI faceIndex;
    }

    private sealed class CardParts
    {
        public RectTransform root;
        public TextMeshProUGUI icon;
        public Image pictogram;
        public TextMeshProUGUI name;
        public TextMeshProUGUI state;
        public TextMeshProUGUI description;
        public TextMeshProUGUI effect;
        public TextMeshProUGUI cost;
        public TextMeshProUGUI requirements;
        public TextMeshProUGUI faceProgress;
        public Button repair;
        public Button analyze;
        public Button fusion;
        public Button seeds;
    }

    private sealed class RotationRigParts
    {
        public GameObject root;
        public MachineCubePerspectiveFaceGraphic from;
        public MachineCubePerspectiveFaceGraphic to;
        public RectTransform edgeShadow;
        public Image edgeHighlight;
    }
}
#endif
