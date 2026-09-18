#if UNITY_EDITOR
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalGenerationVisualPolish
{
    private const float TriangleVerticalOffset = 48f;
    // Azul propio de Higgs: claramente más profundo que el cian general.
    private static readonly Color HiggsBlue = new(0.01f, 0.30f, 0.76f, 1f);
    private static readonly Color32 HiggsBlue32 = new(3, 77, 194, 255);
    private static readonly Color HiggsIconTint = new(0.52f, 0.68f, 0.88f, 1f);

    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";
    private const string PolishFolder =
        "Assets/Project/UI/Vertical/GenerationPolish";
    private const string HiggsPath = PolishFolder + "/qf_node_higgs.png";
    private const string TetraPath = PolishFolder + "/qf_node_tetraquark.png";
    private const string ModulatorPath = PolishFolder + "/qf_node_modulator.png";
    private const string GaugePath = PolishFolder + "/qf_energy_gauge.png";
    private const string GaugeRingPath = PolishFolder + "/qf_gauge_progress_ring.png";
    private const string CircuitEnergyPath = PolishFolder + "/qf_circuit_energy.png";
    private const string CircuitExperimentalPath = PolishFolder + "/qf_circuit_experimental.png";
    private const string CircuitPhasePath = PolishFolder + "/qf_circuit_phase.png";
    private const string ResourceLePath = PolishFolder + "/qf_resource_le.png";
    private const string ResourceTracesPath = PolishFolder + "/qf_resource_traces.png";
    private const string BeamChevronsPath = PolishFolder + "/qf_beam_chevrons.png";
    private const string SelectorFramePath = PolishFolder + "/qf_selector_frame.png";
    private const string EarlyRowPrefabPath =
        "Assets/Project/UI/Vertical/Generated/VerticalBuildingRow.prefab";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Apply Generation Visual Polish")]
    public static void ApplyVisualPolish()
    {
        GeneratePolishSprites();
        ConfigureImports();

        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        Sprite higgs = AssetDatabase.LoadAssetAtPath<Sprite>(HiggsPath);
        Sprite tetra = AssetDatabase.LoadAssetAtPath<Sprite>(TetraPath);
        Sprite modulator = AssetDatabase.LoadAssetAtPath<Sprite>(ModulatorPath);
        Sprite gauge = AssetDatabase.LoadAssetAtPath<Sprite>(GaugePath);
        Sprite gaugeRing = AssetDatabase.LoadAssetAtPath<Sprite>(GaugeRingPath);
        Sprite circuitEnergy = AssetDatabase.LoadAssetAtPath<Sprite>(CircuitEnergyPath);
        Sprite circuitExperimental = AssetDatabase.LoadAssetAtPath<Sprite>(CircuitExperimentalPath);
        Sprite circuitPhase = AssetDatabase.LoadAssetAtPath<Sprite>(CircuitPhasePath);
        Sprite resourceLe = AssetDatabase.LoadAssetAtPath<Sprite>(ResourceLePath);
        Sprite resourceTraces = AssetDatabase.LoadAssetAtPath<Sprite>(ResourceTracesPath);
        Sprite beamChevrons = AssetDatabase.LoadAssetAtPath<Sprite>(BeamChevronsPath);
        Sprite selectorFrame = AssetDatabase.LoadAssetAtPath<Sprite>(SelectorFramePath);
        Require(theme != null, "No se encontró VerticalUiTheme.");
        Require(higgs != null && tetra != null && modulator != null && gauge != null &&
            gaugeRing != null && circuitEnergy != null && circuitExperimental != null &&
            circuitPhase != null && resourceLe != null && resourceTraces != null &&
            beamChevrons != null && selectorFrame != null,
            "No se importaron todos los sprites de pulido de Generación.");

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject panel = FindUnique(scene, "Panel_Generacion");
        GameObject header = FindUnique(scene, "VerticalGenerationHeader");
        GameObject root = FindUnique(scene, "GenerationTriangleRoot");
        GameObject beforeRoot = FindUnique(scene, "GenerationBeforeTriangleRoot");
        Require(panel != null && header != null && root != null && beforeRoot != null,
            "La jerarquía vertical de Generación está incompleta.");

        VerticalSafeAreaLayout safeArea =
            UnityEngine.Object.FindFirstObjectByType<VerticalSafeAreaLayout>(
                FindObjectsInactive.Include);
        Require(safeArea != null, "No se encontro VerticalSafeAreaLayout.");
        safeArea.headerHeight = 0f;
        safeArea.ApplyLayout();
        EditorUtility.SetDirty(safeArea);

        StyleHeader(header, resourceLe, resourceTraces, circuitPhase, theme);
        StyleGenerationRoot(root, beforeRoot, higgs, tetra, modulator, gauge,
            gaugeRing, circuitEnergy, circuitExperimental, circuitPhase,
            beamChevrons, selectorFrame, theme);
        StyleBeforeTriangleRoot(beforeRoot, theme);
        StyleEarlyBuildingRowPrefab(higgs, tetra, modulator, theme);

        EditorSceneManager.MarkSceneDirty(scene);
        Require(EditorSceneManager.SaveScene(scene), "No se pudo guardar Main.unity.");
        AssetDatabase.SaveAssets();
        Debug.Log("[Generation Visual Polish] APPLIED | composición objetivo | " +
            "nodos mecánicos | medidor | haces | circuitos | tres filas compactas");
    }

    [MenuItem("Tools/Quantum Forge/Vertical UI/Fix Modulator Label Overlap")]
    public static void FixModulatorLabelOverlap()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject root = FindUnique(scene, "GenerationTriangleRoot");
        Transform label = root?.transform.Find(
            "TriangleScroll/Viewport/Content/TriangleFocus/Vertex_Modulator/Label");
        Require(label != null, "No se encontró la etiqueta del Modulador.");

        RectTransform rect = (RectTransform)label;
        rect.anchoredPosition = new Vector2(0f, 76f);
        EditorUtility.SetDirty(rect);
        EditorSceneManager.MarkSceneDirty(scene);
        Require(EditorSceneManager.SaveScene(scene), "No se pudo guardar Main.unity.");
        Debug.Log("[Generation Visual Polish] MODULATOR LABEL OVERLAP FIXED");
    }

    [MenuItem("Tools/Quantum Forge/Vertical UI/Apply Higgs Blue Only")]
    public static void ApplyHiggsBlueOnly()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject root = FindUnique(scene, "GenerationTriangleRoot");
        Require(root != null, "Falta GenerationTriangleRoot.");
        Transform content = root.transform.Find("TriangleScroll/Viewport/Content");
        Require(content != null, "Falta Content del Triangulo.");

        Transform focus = content.Find("TriangleFocus");
        Transform higgs = focus?.Find("Vertex_Higgs");
        Transform beam = focus?.Find("Line_Energy");
        Require(higgs != null && beam != null, "Faltan los visuales de Higgs.");

        higgs.GetComponent<Image>().color = WithAlpha(HiggsBlue, 0.27f);
        SetColor(higgs.Find("Icon"), HiggsIconTint);
        higgs.Find("Label").GetComponent<TextMeshProUGUI>().color = HiggsBlue;
        SetColor(beam.Find("BeamGlow"), WithAlpha(HiggsBlue, 0.075f));
        SetColor(beam.Find("BeamCore"), WithAlpha(HiggsBlue, 0.28f));
        SetColor(beam.Find("BeamSegments"), WithAlpha(HiggsBlue, 0.10f));
        SetColor(beam.Find("BeamChevrons"), HiggsBlue);
        SetColor(beam.Find("BeamStartConnector"), WithAlpha(HiggsBlue, 0.28f));
        SetColor(beam.Find("BeamEndConnector"), WithAlpha(HiggsBlue, 0.28f));
        SetColor(beam.Find("EnergyFlow"), HiggsBlue);

        VerticalGenerationPolishUI polish = focus.GetComponent<VerticalGenerationPolishUI>();
        Require(polish != null, "Falta VerticalGenerationPolishUI.");
        polish.energyColor = HiggsBlue;
        EditorUtility.SetDirty(polish);

        Transform circuit = content.Find("CircuitSelectors/Circuit_Energy");
        Require(circuit != null, "Falta Circuit_Energy.");
        SetColor(circuit.Find("StateBorder"), WithAlpha(HiggsBlue, 0.46f));
        SetColor(circuit.Find("CircuitIcon"), new Color(0.80f, 0.783f, 0.922f, 1f));

        Transform card = content.Find("TriangleArtifactCards/TriangleCard_Higgs");
        Require(card != null, "Falta TriangleCard_Higgs.");
        SetColor(card.Find("AccentBar"), HiggsBlue);
        SetColor(card.Find("Icon"), HiggsIconTint);
        SetColor(card.Find("State"), HiggsBlue);

        EditorSceneManager.MarkSceneDirty(scene);
        Require(EditorSceneManager.SaveScene(scene), "No se pudo guardar Main.unity.");
        AssetDatabase.SaveAssets();
        Debug.Log("[Generation Visual Polish] HIGGS BLUE APPLIED | deeper blue");
    }

    private static Color WithAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    private static void SetColor(Transform target, Color color)
    {
        Require(target != null, "Falta un elemento visual de Higgs.");
        Graphic graphic = target.GetComponent<Graphic>();
        Require(graphic != null, target.name + " no tiene Graphic.");
        graphic.color = color;
        EditorUtility.SetDirty(graphic);
    }

    [MenuItem("Tools/Quantum Forge/Vertical UI/Apply and Validate Generation Visual Polish")]
    public static void ApplyAndValidateBatch()
    {
        try
        {
            ApplyVisualPolish();
            ApplyVisualPolish();
            VerticalGenerationVisualPolishValidation.Validate();
            Debug.Log("[Generation Visual Polish] PASS | aplicado dos veces y validado");
            EditorApplication.Exit(0);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            EditorApplication.Exit(1);
        }
    }

    private static void ConfigureImports()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        string[] paths =
        {
            HiggsPath, TetraPath, ModulatorPath, GaugePath, GaugeRingPath,
            CircuitEnergyPath, CircuitExperimentalPath, CircuitPhasePath,
            ResourceLePath, ResourceTracesPath, BeamChevronsPath, SelectorFramePath
        };
        foreach (string path in paths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Require(importer != null, "No se pudo configurar " + path + ".");
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = path == BeamChevronsPath
                ? TextureWrapMode.Repeat
                : TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.maxTextureSize = 1024;
            if (path == SelectorFramePath)
                importer.spriteBorder = new Vector4(16f, 16f, 16f, 16f);
            TextureImporterPlatformSettings android =
                importer.GetPlatformTextureSettings("Android");
            android.overridden = true;
            android.maxTextureSize = 1024;
            android.format = TextureImporterFormat.ASTC_4x4;
            android.compressionQuality = 100;
            importer.SetPlatformTextureSettings(android);
            importer.SaveAndReimport();
        }
    }

    private static void StyleHeader(
        GameObject header,
        Sprite leIcon,
        Sprite tracesIcon,
        Sprite energyIcon,
        VerticalUiTheme theme)
    {
        RectTransform headerRect = (RectTransform)header.transform;
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0f, -34f);
        headerRect.sizeDelta = new Vector2(-250f, 92f);

        Transform le = header.transform.Find("Resource_LE");
        Transform traces = header.transform.Find("Resource_Traces");
        Require(le != null && traces != null, "Faltan recursos base de Generación.");
        Transform energy = header.transform.Find("Resource_Energy");
        if (energy == null)
        {
            GameObject clone = UnityEngine.Object.Instantiate(traces.gameObject, header.transform);
            clone.name = "Resource_Energy";
            energy = clone.transform;
        }
        SetAnchors((RectTransform)le, new Vector2(0f, 0f), new Vector2(0.32f, 1f));
        SetAnchors((RectTransform)traces, new Vector2(0.34f, 0f), new Vector2(0.66f, 1f));
        SetAnchors((RectTransform)energy, new Vector2(0.68f, 0f), new Vector2(1f, 1f));
        StyleResourcePanel(le, leIcon, theme.energy, theme);
        StyleResourcePanel(traces, tracesIcon, theme.traces, theme);
        StyleResourcePanel(energy, energyIcon, theme.triangle, theme);
        TextMeshProUGUI energyValue = energy.Find("Value")?.GetComponent<TextMeshProUGUI>();
        Require(energyValue != null, "Resource_Energy perdió Value.");
        energyValue.SetText("ENERGÍA 0\n+0.00/s");
        HUD hud = UnityEngine.Object.FindFirstObjectByType<HUD>(FindObjectsInactive.Include);
        Require(hud != null, "Falta HUD para enlazar Energía.");
        hud.energyText = energyValue;
        EditorUtility.SetDirty(hud);
        ApplyFont(header.transform, theme);
    }

    private static void StyleResourcePanel(
        Transform resource,
        Sprite sprite,
        Color accent,
        VerticalUiTheme theme)
    {
        Require(resource != null, "Falta un panel de recurso de Generación.");
        Image background = resource.GetComponent<Image>();
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = Color.white;

        Image inner = CreateImage("TechInnerBorder", resource, theme.buttonFrame,
            new Color(accent.r, accent.g, accent.b, 0.20f));
        Stretch(inner.rectTransform, 7f);
        inner.type = Image.Type.Sliced;
        inner.transform.SetAsFirstSibling();

        Transform iconTransform = resource.Find("Icon");
        Require(iconTransform != null, resource.name + " perdió Icon.");
        RectTransform iconRect = (RectTransform)iconTransform;
        iconRect.anchoredPosition = new Vector2(24f, 0f);
        iconRect.sizeDelta = new Vector2(58f, 58f);
        Image icon = iconTransform.GetComponent<Image>();
        icon.sprite = sprite;
        icon.color = Color.white;

        TextMeshProUGUI value = resource.Find("Value")?.GetComponent<TextMeshProUGUI>();
        Require(value != null, resource.name + " perdió Value.");
        value.fontSize = 29f;
        value.fontSizeMax = 29f;
        value.fontSizeMin = 22f;
        value.fontStyle = FontStyles.Normal;
        value.characterSpacing = 0.5f;
        value.rectTransform.offsetMin = new Vector2(92f, 6f);
        value.rectTransform.offsetMax = new Vector2(-12f, -8f);
    }

    private static void StyleGenerationRoot(
        GameObject root,
        GameObject beforeRoot,
        Sprite higgs,
        Sprite tetra,
        Sprite modulator,
        Sprite gauge,
        Sprite gaugeRing,
        Sprite circuitEnergy,
        Sprite circuitExperimental,
        Sprite circuitPhase,
        Sprite beamChevrons,
        Sprite selectorFrame,
        VerticalUiTheme theme)
    {
        ConfigureGenerationBounds((RectTransform)root.transform);
        ConfigureGenerationBounds((RectTransform)beforeRoot.transform);

        Transform content = root.transform.Find("TriangleScroll/Viewport/Content");
        Require(content != null, "Falta Content del Triángulo.");
        Transform legacyObservatory = content.Find("TriangleObservatory");
        if (legacyObservatory != null)
            UnityEngine.Object.DestroyImmediate(legacyObservatory.gameObject);
        RectTransform contentRect = (RectTransform)content;
        contentRect.sizeDelta = new Vector2(0f, 1488f);

        TextMeshProUGUI title = content.Find("TriangleGenerationTitle")
            ?.GetComponent<TextMeshProUGUI>();
        Require(title != null, "Falta TriangleGenerationTitle.");
        SetTopRect(title.rectTransform, TriangleVerticalOffset, 78f, 8f, -8f);
        title.fontSize = 38f;
        title.fontSizeMax = 38f;
        title.fontSizeMin = 26f;
        title.fontStyle = FontStyles.Normal;
        title.characterSpacing = 5f;

        Image titleFrame = CreateImage("GenerationTitleFrame", content,
            theme.panelFrame, Color.white);
        titleFrame.type = Image.Type.Sliced;
        SetTopRect(titleFrame.rectTransform, TriangleVerticalOffset, 78f, 8f, -8f);
        titleFrame.transform.SetSiblingIndex(title.transform.GetSiblingIndex());
        title.transform.SetSiblingIndex(titleFrame.transform.GetSiblingIndex() + 1);
        CreateTitleAccents(titleFrame.transform, theme.energy);

        Transform focus = content.Find("TriangleFocus");
        Require(focus != null, "Falta TriangleFocus.");
        RectTransform focusRect = (RectTransform)focus;
        SetTopRect(focusRect, 88f + TriangleVerticalOffset, 1348f, 0f, 0f);
        Image focusImage = focus.GetComponent<Image>();
        focusImage.sprite = theme.panelFrame;
        focusImage.type = Image.Type.Sliced;
        focusImage.color = Color.white;

        AddFocusDecor(focus, theme);
        RectTransform higgsNode = StyleVertex(
            focus.Find("Vertex_Higgs"), higgs, HiggsBlue,
            new Vector2(-250f, 515f), new Vector2(214f, 214f), true, theme);
        higgsNode.Find("Icon").GetComponent<Image>().color = HiggsIconTint;
        RectTransform tetraNode = StyleVertex(
            focus.Find("Vertex_Tetra"), tetra, theme.traces,
            new Vector2(250f, 515f), new Vector2(220f, 220f), true, theme);
        RectTransform modulatorNode = StyleVertex(
            focus.Find("Vertex_Modulator"), modulator, theme.triangle,
            new Vector2(0f, 130f), new Vector2(220f, 220f), false, theme);
        TextMeshProUGUI modulatorLabel = modulatorNode.Find("Label")
            ?.GetComponent<TextMeshProUGUI>();
        Require(modulatorLabel != null, "El Modulador perdio su etiqueta.");
        // Reserve a stable gap above the synchronization status on mobile.
        modulatorLabel.rectTransform.anchoredPosition = new Vector2(0f, 76f);

        Image experimentalCore = StyleBeam(focus.Find("Line_Experimental"),
            new Vector2(-250f, 515f), new Vector2(250f, 515f), 26f,
            theme.traces, beamChevrons, gaugeRing, theme,
            out Image experimentalRoot, out Image experimentalGlow,
            out Image experimentalHotCore, out TextMeshProUGUI experimentalFlow,
            out RawImage experimentalChevrons,
            out Image experimentalStartConnector,
            out Image experimentalEndConnector);
        Image energyCore = StyleBeam(focus.Find("Line_Energy"),
            new Vector2(-250f, 515f), new Vector2(0f, 130f), 28f,
            HiggsBlue, beamChevrons, gaugeRing, theme,
            out Image energyRoot, out Image energyGlow,
            out Image energyHotCore, out TextMeshProUGUI energyFlow,
            out RawImage energyChevrons,
            out Image energyStartConnector, out Image energyEndConnector);
        Image phaseCore = StyleBeam(focus.Find("Line_Phase"),
            new Vector2(250f, 515f), new Vector2(0f, 130f), 28f,
            theme.triangle, beamChevrons, gaugeRing, theme,
            out Image phaseRoot, out Image phaseGlow,
            out Image phaseHotCore, out TextMeshProUGUI phaseFlow,
            out RawImage phaseChevrons,
            out Image phaseStartConnector, out Image phaseEndConnector);
        focus.Find("Line_Experimental").SetSiblingIndex(2);
        focus.Find("Line_Energy").SetSiblingIndex(3);
        focus.Find("Line_Phase").SetSiblingIndex(4);

        Transform centerGlow = focus.Find("SynchronizationCore");
        Require(centerGlow != null, "Falta SynchronizationCore.");
        SetCentered((RectTransform)centerGlow, new Vector2(0f, 330f),
            new Vector2(232f, 232f));
        centerGlow.SetSiblingIndex(Mathf.Min(5, focus.childCount - 1));

        VerticalGenerationPolishUI polish = GetOrAdd<VerticalGenerationPolishUI>(
            focus.gameObject);
        BuildGauge(focus, gauge, gaugeRing, theme, polish);

        TextMeshProUGUI protocol = focus.Find("ProtocolStatus")
            ?.GetComponent<TextMeshProUGUI>();
        Require(protocol != null, "Falta ProtocolStatus.");
        protocol.gameObject.SetActive(false);
        TextMeshProUGUI synchronization = focus.Find("SynchronizationStatus")
            ?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI effect = focus.Find("CircuitEffect")
            ?.GetComponent<TextMeshProUGUI>();
        Require(synchronization != null && effect != null,
            "Faltan textos de sincronización o efecto.");
        SetCentered(synchronization.rectTransform, new Vector2(0f, 18f),
            new Vector2(680f, 42f));
        synchronization.fontSize = 26f;
        synchronization.fontSizeMax = 26f;
        synchronization.fontSizeMin = 20f;
        SetCentered(effect.rectTransform, new Vector2(0f, -24f),
            new Vector2(720f, 38f));
        effect.fontSize = 22f;
        effect.fontSizeMax = 22f;
        effect.fontSizeMin = 17f;

        Transform circuits = content.Find("CircuitSelectors");
        Transform cards = content.Find("TriangleArtifactCards");
        Transform artifactsTitle = content.Find("TriangleArtifactsTitle");
        Require(circuits != null && cards != null && artifactsTitle != null,
            "Faltan selectores o tarjetas del Triángulo.");
        artifactsTitle.gameObject.SetActive(true);
        TextMeshProUGUI purchasesTitle = artifactsTitle.GetComponent<TextMeshProUGUI>();
        if (purchasesTitle != null)
        {
            purchasesTitle.SetText("COMPRAS");
            purchasesTitle.fontSize = 24f;
            purchasesTitle.fontSizeMax = 24f;
            purchasesTitle.fontSizeMin = 18f;
            purchasesTitle.characterSpacing = 3f;
        }

        SetTopRect((RectTransform)circuits,
            820f + TriangleVerticalOffset, 100f, 8f, -8f);
        StyleCircuits(circuits, circuitEnergy, circuitExperimental,
            circuitPhase, selectorFrame, theme, polish);
        Image purchasesFrame = CreateImage("TrianglePurchasesFrame", content,
            theme.panelFrame, Color.white);
        purchasesFrame.type = Image.Type.Sliced;
        SetTopRect(purchasesFrame.rectTransform,
            928f + TriangleVerticalOffset, 508f, 8f, -8f);
        purchasesFrame.transform.SetSiblingIndex(artifactsTitle.GetSiblingIndex());
        SetTopRect((RectTransform)artifactsTitle,
            936f + TriangleVerticalOffset, 52f, 28f, -28f);
        artifactsTitle.SetSiblingIndex(purchasesFrame.transform.GetSiblingIndex() + 1);
        SetTopRect((RectTransform)cards,
            992f + TriangleVerticalOffset, 426f, 22f, -22f);
        cards.SetSiblingIndex(artifactsTitle.GetSiblingIndex() + 1);
        StyleCards(cards, higgs, tetra, modulator, theme);

        polish.energyBeamRoot = energyRoot;
        polish.experimentalBeamRoot = experimentalRoot;
        polish.phaseBeamRoot = phaseRoot;
        polish.energyBeamGlow = energyGlow;
        polish.experimentalBeamGlow = experimentalGlow;
        polish.phaseBeamGlow = phaseGlow;
        polish.energyBeamCore = energyCore;
        polish.experimentalBeamCore = experimentalCore;
        polish.phaseBeamCore = phaseCore;
        polish.energyBeamHotCore = energyHotCore;
        polish.experimentalBeamHotCore = experimentalHotCore;
        polish.phaseBeamHotCore = phaseHotCore;
        polish.energyChevrons = energyChevrons;
        polish.experimentalChevrons = experimentalChevrons;
        polish.phaseChevrons = phaseChevrons;
        polish.energyStartConnector = energyStartConnector;
        polish.energyEndConnector = energyEndConnector;
        polish.experimentalStartConnector = experimentalStartConnector;
        polish.experimentalEndConnector = experimentalEndConnector;
        polish.phaseStartConnector = phaseStartConnector;
        polish.phaseEndConnector = phaseEndConnector;
        polish.energyFlow = energyFlow;
        polish.experimentalFlow = experimentalFlow;
        polish.phaseFlow = phaseFlow;
        polish.higgsNode = higgsNode;
        polish.tetraNode = tetraNode;
        polish.modulatorNode = modulatorNode;
        polish.synchronizationText = synchronization;
        polish.effectText = effect;
        polish.energyColor = HiggsBlue;
        polish.experimentalColor = theme.traces;
        polish.phaseColor = theme.triangle;
        ApplyFont(content, theme);
        EditorUtility.SetDirty(polish);
    }

    private static void ConfigureGenerationBounds(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(16f, 20f);
        rect.offsetMax = new Vector2(-16f, -148f);
    }

    private static void StyleBeforeTriangleRoot(
        GameObject beforeRoot,
        VerticalUiTheme theme)
    {
        TextMeshProUGUI title = beforeRoot.transform.Find("GenerationTitle")
            ?.GetComponent<TextMeshProUGUI>();
        Transform frameTransform = beforeRoot.transform.Find("ArtifactsFrame");
        Require(title != null && frameTransform != null,
            "La presentacion anterior al Triangulo esta incompleta.");

        SetTopRect(title.rectTransform, 0f, 78f, 8f, -8f);
        title.fontSize = 38f;
        title.fontSizeMax = 38f;
        title.fontSizeMin = 26f;
        title.fontStyle = FontStyles.Normal;
        title.characterSpacing = 5f;

        Image titleFrame = CreateImage("EarlyGenerationTitleFrame",
            beforeRoot.transform, theme.panelFrame, Color.white);
        titleFrame.type = Image.Type.Sliced;
        SetTopRect(titleFrame.rectTransform, 0f, 78f, 8f, -8f);
        titleFrame.transform.SetSiblingIndex(0);
        title.transform.SetSiblingIndex(1);
        frameTransform.SetSiblingIndex(2);
        CreateTitleAccents(titleFrame.transform, theme.energy);

        RectTransform frameRect = (RectTransform)frameTransform;
        SetTopRect(frameRect, 88f, 720f, 0f, 0f);
        Image frame = frameTransform.GetComponent<Image>();
        Require(frame != null, "ArtifactsFrame perdio su Image.");
        frame.sprite = theme.panelFrame;
        frame.type = Image.Type.Sliced;
        frame.color = Color.white;

        Image grid = CreateImage("EarlyTechnologyGrid", frameTransform,
            theme.backgroundGrid, new Color(0.08f, 0.38f, 0.52f, 0.10f));
        Stretch(grid.rectTransform, 18f);
        grid.type = Image.Type.Tiled;
        grid.transform.SetAsFirstSibling();

        Image inner = CreateImage("EarlyInnerFrame", frameTransform,
            theme.panelFrame, new Color(0.34f, 0.58f, 0.75f, 0.42f));
        Stretch(inner.rectTransform, 14f);
        inner.type = Image.Type.Sliced;
        inner.transform.SetSiblingIndex(1);

        TextMeshProUGUI section = frameTransform.Find("ArtifactsTitle")
            ?.GetComponent<TextMeshProUGUI>();
        Require(section != null, "ArtifactsFrame perdio ArtifactsTitle.");
        section.fontSize = 25f;
        section.fontSizeMax = 25f;
        section.fontSizeMin = 18f;
        section.characterSpacing = 2f;
        section.color = theme.energy;
        section.rectTransform.anchoredPosition = new Vector2(0f, -9f);
        section.rectTransform.sizeDelta = new Vector2(-76f, 64f);

        Image sectionRail = CreateImage("EarlySectionRail", frameTransform,
            null, new Color(theme.energy.r, theme.energy.g, theme.energy.b, 0.72f));
        sectionRail.rectTransform.anchorMin = new Vector2(0f, 1f);
        sectionRail.rectTransform.anchorMax = new Vector2(0f, 1f);
        sectionRail.rectTransform.pivot = new Vector2(0f, 1f);
        sectionRail.rectTransform.anchoredPosition = new Vector2(38f, -66f);
        sectionRail.rectTransform.sizeDelta = new Vector2(210f, 3f);

        Transform scroll = frameTransform.Find("ArtifactsScroll");
        Require(scroll != null, "ArtifactsFrame perdio ArtifactsScroll.");
        RectTransform scrollRect = (RectTransform)scroll;
        scrollRect.offsetMin = new Vector2(28f, 26f);
        scrollRect.offsetMax = new Vector2(-28f, -82f);
        Transform list = scroll.Find("Viewport/KnownArtifactsList");
        Require(list != null, "Falta KnownArtifactsList anterior al Triangulo.");
        VerticalLayoutGroup layout = list.GetComponent<VerticalLayoutGroup>();
        Require(layout != null, "KnownArtifactsList perdio VerticalLayoutGroup.");
        layout.padding = new RectOffset(8, 8, 8, 18);
        layout.spacing = 12f;

        ApplyFont(beforeRoot.transform, theme);
    }

    private static void StyleEarlyBuildingRowPrefab(
        Sprite higgs,
        Sprite tetra,
        Sprite modulator,
        VerticalUiTheme theme)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(EarlyRowPrefabPath);
        Require(root != null, "No se pudo abrir VerticalBuildingRow.prefab.");
        try
        {
            Image background = root.GetComponent<Image>();
            BuildingRowUI row = root.GetComponent<BuildingRowUI>();
            LayoutElement rowLayout = root.GetComponent<LayoutElement>();
            Require(background != null && row != null && rowLayout != null,
                "VerticalBuildingRow perdio sus componentes principales.");
            background.sprite = theme.panelFrame;
            background.type = Image.Type.Sliced;
            background.color = new Color(0.68f, 0.82f, 0.95f, 1f);
            rowLayout.minHeight = 190f;
            rowLayout.preferredHeight = 204f;
            rowLayout.flexibleHeight = 0f;

            Image inner = CreateImage("EarlyCardInnerFrame", root.transform,
                theme.buttonFrame,
                new Color(theme.border.r, theme.border.g, theme.border.b, 0.28f));
            Stretch(inner.rectTransform, 7f);
            inner.type = Image.Type.Sliced;
            inner.transform.SetAsFirstSibling();

            Image accent = CreateImage("AccentBar", root.transform, null, theme.energy);
            accent.rectTransform.anchorMin = new Vector2(0f, 0.14f);
            accent.rectTransform.anchorMax = new Vector2(0f, 0.86f);
            accent.rectTransform.pivot = new Vector2(0f, 0.5f);
            accent.rectTransform.anchoredPosition = new Vector2(9f, 0f);
            accent.rectTransform.sizeDelta = new Vector2(4f, 0f);

            Transform iconTransform = root.transform.Find("ArtifactIcon");
            Require(iconTransform != null, "VerticalBuildingRow perdio ArtifactIcon.");
            RectTransform iconRect = (RectTransform)iconTransform;
            iconRect.anchoredPosition = new Vector2(24f, 0f);
            iconRect.sizeDelta = new Vector2(150f, 150f);
            Image icon = iconTransform.GetComponent<Image>();
            icon.preserveAspect = true;

            Image glow = CreateImage("ArtifactGlow", iconTransform,
                theme.softGlow,
                new Color(theme.energy.r, theme.energy.g, theme.energy.b, 0.12f));
            Stretch(glow.rectTransform, -10f);
            glow.transform.SetAsFirstSibling();

            TextMeshProUGUI name = root.transform.Find("Name")
                ?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI stats = root.transform.Find("Stats")
                ?.GetComponent<TextMeshProUGUI>();
            Require(name != null && stats != null,
                "VerticalBuildingRow perdio sus textos.");
            name.fontSize = 26f;
            name.fontSizeMax = 26f;
            name.fontSizeMin = 20f;
            name.color = theme.primaryText;
            name.characterSpacing = 0.8f;
            SetAnchors(name.rectTransform, new Vector2(0.22f, 0.57f),
                new Vector2(0.74f, 0.92f));
            stats.fontSize = 22f;
            stats.fontSizeMax = 22f;
            stats.fontSizeMin = 17f;
            stats.color = Color.Lerp(theme.secondaryText, theme.primaryText, 0.30f);
            SetAnchors(stats.rectTransform, new Vector2(0.22f, 0.17f),
                new Vector2(0.74f, 0.60f));

            Transform buyTransform = root.transform.Find("BuyButton");
            Require(buyTransform != null, "VerticalBuildingRow perdio BuyButton.");
            RectTransform buyRect = (RectTransform)buyTransform;
            SetAnchors(buyRect, new Vector2(0.77f, 0.27f),
                new Vector2(0.97f, 0.76f));
            Image buyImage = buyTransform.GetComponent<Image>();
            buyImage.sprite = theme.selectedButtonFrame;
            buyImage.type = Image.Type.Sliced;
            buyImage.color = new Color(0.58f, 0.78f, 0.96f, 1f);
            LayoutElement buyLayout = GetOrAdd<LayoutElement>(buyTransform.gameObject);
            buyLayout.minWidth = 120f;
            buyLayout.minHeight = 50f;
            TextMeshProUGUI buyLabel = buyTransform.Find("Label")
                ?.GetComponent<TextMeshProUGUI>();
            Require(buyLabel != null, "VerticalBuildingRow perdio Label de compra.");
            buyLabel.fontSize = 22f;
            buyLabel.fontSizeMax = 22f;
            buyLabel.fontSizeMin = 17f;
            buyLabel.characterSpacing = 0.8f;

            Transform tickTrack = root.transform.Find("TickTrack");
            Require(tickTrack != null, "VerticalBuildingRow perdio TickTrack.");
            RectTransform trackRect = (RectTransform)tickTrack;
            trackRect.anchorMin = new Vector2(0.22f, 0.09f);
            trackRect.anchorMax = new Vector2(0.74f, 0.09f);
            trackRect.sizeDelta = new Vector2(0f, 6f);

            row.higgsIcon = higgs;
            row.tetraIcon = tetra;
            row.modulatorIcon = modulator;
            ApplyFont(root.transform, theme);
            EditorUtility.SetDirty(row);
            PrefabUtility.SaveAsPrefabAsset(root, EarlyRowPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void AddFocusDecor(Transform focus, VerticalUiTheme theme)
    {
        Image grid = CreateImage("TechnologyGrid", focus,
            theme.backgroundGrid, new Color(0.08f, 0.38f, 0.52f, 0.10f));
        Stretch(grid.rectTransform, 18f);
        grid.type = Image.Type.Tiled;
        grid.transform.SetAsFirstSibling();

        Image inner = CreateImage("FocusInnerFrame", focus,
            theme.panelFrame, new Color(0.34f, 0.58f, 0.75f, 0.42f));
        Stretch(inner.rectTransform, 14f);
        inner.type = Image.Type.Sliced;
        inner.transform.SetSiblingIndex(1);

        SetGeneratedDecorationActive(focus, "Corner_TL", false);
        SetGeneratedDecorationActive(focus, "Corner_TR", false);
        SetGeneratedDecorationActive(focus, "Corner_BL", false);
        SetGeneratedDecorationActive(focus, "Corner_BR", false);
    }

    private static RectTransform StyleVertex(
        Transform vertex,
        Sprite sprite,
        Color accent,
        Vector2 position,
        Vector2 size,
        bool labelAbove,
        VerticalUiTheme theme)
    {
        Require(vertex != null, "Falta un vértice del Triángulo.");
        RectTransform rect = (RectTransform)vertex;
        SetCentered(rect, position, size);
        Image halo = vertex.GetComponent<Image>();
        halo.sprite = theme.softGlow;
        halo.color = new Color(accent.r, accent.g, accent.b, 0.27f);

        Transform iconTransform = vertex.Find("Icon");
        Require(iconTransform != null, vertex.name + " perdió Icon.");
        RectTransform iconRect = (RectTransform)iconTransform;
        SetCentered(iconRect, new Vector2(0f, 4f), size * 0.94f);
        Image icon = iconTransform.GetComponent<Image>();
        icon.sprite = sprite;
        icon.color = Color.white;
        icon.preserveAspect = true;

        TextMeshProUGUI label = vertex.Find("Label")?.GetComponent<TextMeshProUGUI>();
        Require(label != null, vertex.name + " perdió Label.");
        label.fontSize = 21f;
        label.fontSizeMax = 21f;
        label.fontSizeMin = 16f;
        label.fontStyle = FontStyles.Normal;
        label.characterSpacing = 1.5f;
        label.color = accent;
        label.rectTransform.anchorMin = new Vector2(0f, labelAbove ? 1f : 0f);
        label.rectTransform.anchorMax = new Vector2(1f, labelAbove ? 1f : 0f);
        label.rectTransform.pivot = new Vector2(0.5f, labelAbove ? 0f : 1f);
        label.rectTransform.anchoredPosition = new Vector2(0f, labelAbove ? 3f : -4f);
        label.rectTransform.sizeDelta = new Vector2(40f, 36f);
        return rect;
    }

    private static Image StyleBeam(
        Transform line,
        Vector2 from,
        Vector2 to,
        float thickness,
        Color accent,
        Sprite chevronSprite,
        Sprite connectorSprite,
        VerticalUiTheme theme,
        out Image root,
        out Image glow,
        out Image hotCore,
        out TextMeshProUGUI flow,
        out RawImage chevrons,
        out Image startConnector,
        out Image endConnector)
    {
        Require(line != null, "Falta una conexión del Triángulo.");
        RectTransform rect = (RectTransform)line;
        Vector2 delta = to - from;
        SetCentered(rect, (from + to) * 0.5f,
            new Vector2(delta.magnitude, thickness));
        rect.localEulerAngles = new Vector3(0f, 0f,
            Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        root = line.GetComponent<Image>();
        root.color = new Color(0.018f, 0.050f, 0.068f, 0.96f);

        glow = CreateImage("BeamGlow", line, theme.softGlow,
            new Color(accent.r, accent.g, accent.b, 0.075f));
        Stretch(glow.rectTransform, -10f, -8f, -10f, -8f);
        glow.type = Image.Type.Sliced;
        glow.transform.SetAsFirstSibling();

        Image core = CreateImage("BeamCore", line, null,
            new Color(accent.r, accent.g, accent.b, 0.28f));
        Stretch(core.rectTransform, 2f, 9f, 2f, 9f);

        hotCore = CreateImage("BeamHotCore", line, null,
            new Color(1f, 1f, 1f, 0.06f));
        Stretch(hotCore.rectTransform, 5f, 12f, 5f, 12f);

        Image upperRail = CreateImage("BeamUpperRail", line, null,
            new Color(0.22f, 0.34f, 0.40f, 0.62f));
        upperRail.rectTransform.anchorMin = new Vector2(0f, 1f);
        upperRail.rectTransform.anchorMax = Vector2.one;
        upperRail.rectTransform.offsetMin = new Vector2(0f, -2f);
        upperRail.rectTransform.offsetMax = Vector2.zero;
        Image lowerRail = CreateImage("BeamLowerRail", line, null,
            new Color(0.01f, 0.03f, 0.05f, 0.95f));
        lowerRail.rectTransform.anchorMin = Vector2.zero;
        lowerRail.rectTransform.anchorMax = new Vector2(1f, 0f);
        lowerRail.rectTransform.offsetMin = Vector2.zero;
        lowerRail.rectTransform.offsetMax = new Vector2(0f, 2f);

        TextMeshProUGUI segments = CreateText("BeamSegments", line,
            ">  >  >  >  >  >  >  >  >", 15f,
            TextAlignmentOptions.Center,
            new Color(accent.r, accent.g, accent.b, 0.10f), theme);
        Stretch(segments.rectTransform, 8f, 0f, 8f, 0f);
        segments.textWrappingMode = TextWrappingModes.NoWrap;

        chevrons = CreateRawImage("BeamChevrons", line,
            chevronSprite != null ? chevronSprite.texture : null, accent);
        float repeats = Mathf.Max(1f, delta.magnitude / 72f);
        chevrons.uvRect = new Rect(0f, 0f, repeats, 1f);
        Stretch(chevrons.rectTransform, 12f, 8f, 12f, 8f);
        chevrons.gameObject.SetActive(false);

        startConnector = CreateImage("BeamStartConnector", line,
            connectorSprite, new Color(accent.r, accent.g, accent.b, 0.28f));
        ConfigureBeamConnector(startConnector.rectTransform, true);
        endConnector = CreateImage("BeamEndConnector", line,
            connectorSprite, new Color(accent.r, accent.g, accent.b, 0.28f));
        ConfigureBeamConnector(endConnector.rectTransform, false);

        flow = CreateText("EnergyFlow", line, "›  ›  ›  ›  ›",
            17f, TextAlignmentOptions.Center, accent, theme);
        flow.text = ">   >   >   >   >   >";
        Stretch(flow.rectTransform, -18f, -2f, -18f, -2f);
        flow.fontStyle = FontStyles.Bold;
        flow.textWrappingMode = TextWrappingModes.NoWrap;
        flow.raycastTarget = false;
        flow.transform.SetAsLastSibling();
        chevrons.transform.SetAsLastSibling();
        startConnector.transform.SetAsLastSibling();
        endConnector.transform.SetAsLastSibling();
        return core;
    }

    private static void ConfigureBeamConnector(RectTransform rect, bool start)
    {
        float anchor = start ? 0f : 1f;
        rect.anchorMin = new Vector2(anchor, 0.5f);
        rect.anchorMax = new Vector2(anchor, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(start ? 2f : -2f, 0f);
        rect.sizeDelta = new Vector2(30f, 30f);
    }

    private static void BuildGauge(
        Transform focus,
        Sprite gauge,
        Sprite gaugeRing,
        VerticalUiTheme theme,
        VerticalGenerationPolishUI polish)
    {
        GameObject root = GetOrCreateUi("EnergyGauge", focus);
        SetCentered((RectTransform)root.transform, new Vector2(0f, 330f),
            new Vector2(210f, 210f));

        Image glow = CreateImage("GaugeGlow", root.transform, theme.softGlow,
            new Color(theme.energy.r, theme.energy.g, theme.energy.b, 0.13f));
        Stretch(glow.rectTransform, -18f);

        Image backdrop = CreateImage("GaugeBackdrop", root.transform, gaugeRing,
            new Color(0.025f, 0.095f, 0.13f, 0.98f));
        Stretch(backdrop.rectTransform, 12f);

        Image frame = GetOrAdd<Image>(root);
        frame.sprite = gauge;
        frame.color = new Color(0.40f, 0.58f, 0.66f, 0.68f);
        frame.preserveAspect = true;
        frame.raycastTarget = false;

        Image progressRing = CreateImage("GaugeProgressRing", root.transform,
            gaugeRing, theme.energy);
        Stretch(progressRing.rectTransform, 12f);
        progressRing.type = Image.Type.Filled;
        progressRing.fillMethod = Image.FillMethod.Radial360;
        progressRing.fillOrigin = 2;
        progressRing.fillClockwise = true;
        progressRing.fillAmount = 0f;

        TextMeshProUGUI circuit = CreateText("GaugeCircuit", root.transform,
            "ENERGÍA", 19f, TextAlignmentOptions.Center, theme.primaryText, theme);
        SetCentered(circuit.rectTransform, new Vector2(0f, 42f),
            new Vector2(144f, 30f));
        circuit.characterSpacing = 1.5f;
        TextMeshProUGUI progress = CreateText("GaugeProgress", root.transform,
            "0% · 90 s", 24f, TextAlignmentOptions.Center, theme.energy, theme);
        progress.fontSizeMin = 19f;
        SetCentered(progress.rectTransform, new Vector2(0f, -10f),
            new Vector2(156f, 48f));

        polish.gaugeFrame = frame;
        polish.gaugeGlow = glow;
        polish.gaugeProgressRing = progressRing;
        polish.gaugeCircuitText = circuit;
        polish.gaugeProgressText = progress;
    }

    private static void StyleCircuits(
        Transform circuits,
        Sprite energyIcon,
        Sprite experimentalIcon,
        Sprite phaseIcon,
        Sprite selectorFrame,
        VerticalUiTheme theme,
        VerticalGenerationPolishUI polish)
    {
        HorizontalLayoutGroup layout = circuits.GetComponent<HorizontalLayoutGroup>();
        Require(layout != null, "CircuitSelectors perdió HorizontalLayoutGroup.");
        layout.padding = new RectOffset(2, 2, 2, 2);
        layout.spacing = 10f;

        polish.energyCircuitBorder = StyleCircuit(
            circuits.Find("Circuit_Energy"), energyIcon, selectorFrame, HiggsBlue, theme);
        polish.experimentalCircuitBorder = StyleCircuit(
            circuits.Find("Circuit_Experimental"), experimentalIcon, selectorFrame, theme.traces, theme);
        polish.phaseCircuitBorder = StyleCircuit(
            circuits.Find("Circuit_Phase"), phaseIcon,
            selectorFrame, theme.triangle, theme);
    }

    private static Image StyleCircuit(
        Transform circuit,
        Sprite iconSprite,
        Sprite selectorFrame,
        Color accent,
        VerticalUiTheme theme)
    {
        Require(circuit != null, "Falta un selector de circuito.");
        Image background = circuit.GetComponent<Image>();
        background.sprite = theme.buttonFrame;
        background.type = Image.Type.Sliced;
        background.color = theme.deepSurface;
        Button button = circuit.GetComponent<Button>();
        button.targetGraphic = background;

        TriangleSlotUI slot = circuit.GetComponent<TriangleSlotUI>();
        SerializedObject serialized = new SerializedObject(slot);
        serialized.FindProperty("useLayeredVisuals").boolValue = true;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        LayoutElement element = GetOrAdd<LayoutElement>(circuit.gameObject);
        element.minHeight = 96f;
        element.preferredHeight = 100f;

        Image border = CreateImage("StateBorder", circuit,
            selectorFrame,
            new Color(accent.r, accent.g, accent.b, 0.46f));
        border.type = Image.Type.Sliced;
        Stretch(border.rectTransform, 0f);
        border.transform.SetAsFirstSibling();

        Image icon = CreateImage("CircuitIcon", circuit, iconSprite, Color.white);
        icon.preserveAspect = true;
        icon.rectTransform.anchorMin = new Vector2(0f, 0.5f);
        icon.rectTransform.anchorMax = new Vector2(0f, 0.5f);
        icon.rectTransform.pivot = new Vector2(0f, 0.5f);
        icon.rectTransform.anchoredPosition = new Vector2(14f, 0f);
        icon.rectTransform.sizeDelta = new Vector2(54f, 54f);

        TextMeshProUGUI label = circuit.Find("Label")?.GetComponent<TextMeshProUGUI>();
        Require(label != null, circuit.name + " perdió Label.");
        label.fontSize = 21f;
        label.fontSizeMax = 21f;
        label.fontSizeMin = 16f;
        label.fontStyle = FontStyles.Normal;
        label.characterSpacing = 0.5f;
        label.rectTransform.anchorMin = new Vector2(0.25f, 0f);
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = Vector2.zero;
        label.rectTransform.offsetMax = new Vector2(-7f, 0f);
        return border;
    }

    private static void StyleCards(
        Transform cards,
        Sprite higgs,
        Sprite tetra,
        Sprite modulator,
        VerticalUiTheme theme)
    {
        VerticalLayoutGroup layout = cards.GetComponent<VerticalLayoutGroup>();
        if (layout == null)
        {
            HorizontalLayoutGroup horizontal =
                cards.GetComponent<HorizontalLayoutGroup>();
            if (horizontal != null)
                UnityEngine.Object.DestroyImmediate(horizontal);
            layout = cards.gameObject.AddComponent<VerticalLayoutGroup>();
        }
        layout.spacing = 8f;
        layout.padding = new RectOffset(4, 4, 4, 4);
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;

        StyleCard(cards.Find("TriangleCard_Higgs"), higgs, HiggsBlue,
            higgs, tetra, modulator, theme);
        cards.Find("TriangleCard_Higgs/Icon").GetComponent<Image>().color = HiggsIconTint;
        StyleCard(cards.Find("TriangleCard_Tetra"), tetra, theme.traces,
            higgs, tetra, modulator, theme);
        StyleCard(cards.Find("TriangleCard_Modulator"), modulator, theme.triangle,
            higgs, tetra, modulator, theme);
    }

    private static void StyleCard(
        Transform card,
        Sprite iconSprite,
        Color accent,
        Sprite higgs,
        Sprite tetra,
        Sprite modulator,
        VerticalUiTheme theme)
    {
        Require(card != null, "Falta una fila de artefacto.");
        Image background = card.GetComponent<Image>();
        background.sprite = theme.panelFrame;
        background.type = Image.Type.Sliced;
        background.color = new Color(0.68f, 0.82f, 0.95f, 1f);
        LayoutElement rowLayout = card.GetComponent<LayoutElement>();
        rowLayout.minHeight = 132f;
        rowLayout.preferredHeight = 132f;
        rowLayout.flexibleHeight = 0f;

        Image accentBar = CreateImage("AccentBar", card, null, accent);
        accentBar.rectTransform.anchorMin = new Vector2(0f, 0.16f);
        accentBar.rectTransform.anchorMax = new Vector2(0f, 0.84f);
        accentBar.rectTransform.pivot = new Vector2(0f, 0.5f);
        accentBar.rectTransform.anchoredPosition = new Vector2(7f, 0f);
        accentBar.rectTransform.sizeDelta = new Vector2(4f, 0f);

        Transform iconTransform = card.Find("Icon");
        RectTransform iconRect = (RectTransform)iconTransform;
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(18f, 0f);
        iconRect.sizeDelta = new Vector2(88f, 88f);
        Image icon = iconTransform.GetComponent<Image>();
        icon.sprite = iconSprite;
        icon.color = Color.white;

        TextMeshProUGUI name = card.Find("Name")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI state = card.Find("State")?.GetComponent<TextMeshProUGUI>();
        Require(name != null && state != null, card.name + " perdió textos.");
        name.fontSize = 27f;
        name.fontSizeMax = 27f;
        name.fontSizeMin = 21f;
        name.fontStyle = FontStyles.Normal;
        name.characterSpacing = 0.8f;
        SetAnchors(name.rectTransform, new Vector2(0.15f, 0.48f),
            new Vector2(0.71f, 0.94f));
        state.fontSize = 23f;
        state.fontSizeMax = 23f;
        state.fontSizeMin = 18f;
        state.fontStyle = FontStyles.Bold;
        state.color = accent;
        SetAnchors(state.rectTransform, new Vector2(0.15f, 0.08f),
            new Vector2(0.71f, 0.50f));

        Transform buyTransform = card.Find("BuyButton");
        RectTransform buyRect = (RectTransform)buyTransform;
        SetAnchors(buyRect, new Vector2(0.72f, 0.20f),
            new Vector2(0.985f, 0.80f));
        Image buyImage = buyTransform.GetComponent<Image>();
        buyImage.sprite = theme.selectedButtonFrame;
        buyImage.type = Image.Type.Sliced;
        buyImage.color = new Color(0.58f, 0.78f, 0.96f, 1f);
        LayoutElement buyLayout = buyTransform.GetComponent<LayoutElement>();
        buyLayout.minWidth = 176f;
        buyLayout.minHeight = 72f;
        buyLayout.preferredWidth = 184f;
        buyLayout.preferredHeight = 76f;
        TextMeshProUGUI buyLabel = buyTransform.Find("Label")
            ?.GetComponent<TextMeshProUGUI>();
        Require(buyLabel != null, card.name + " perdió el texto de compra.");
        buyLabel.fontSize = 22f;
        buyLabel.fontSizeMax = 22f;
        buyLabel.fontSizeMin = 14f;
        buyLabel.characterSpacing = 0.8f;

        VerticalTriangleArtifactCardUI controller =
            card.GetComponent<VerticalTriangleArtifactCardUI>();
        controller.higgsIcon = higgs;
        controller.tetraIcon = tetra;
        controller.modulatorIcon = modulator;
        EditorUtility.SetDirty(controller);
    }

    private static void GeneratePolishSprites()
    {
        Directory.CreateDirectory(Path.GetFullPath(PolishFolder));

        Color32 clear = new(0, 0, 0, 0);
        Color32 white = new(255, 255, 255, 255);
        Color32 soft = new(255, 255, 255, 150);

        Color32[] ring = NewPixels(256, clear);
        DrawRing(ring, 256, new Vector2(127.5f, 127.5f), 104f, 10f, white);
        DrawRing(ring, 256, new Vector2(127.5f, 127.5f), 88f, 2f, soft);
        for (int angle = 0; angle < 360; angle += 15)
        {
            float radians = angle * Mathf.Deg2Rad;
            Vector2 direction = new(Mathf.Sin(radians), Mathf.Cos(radians));
            DrawLine(ring, 256, 256,
                new Vector2(127.5f, 127.5f) + direction * 113f,
                new Vector2(127.5f, 127.5f) + direction * 121f, 2f, soft);
        }
        WriteTexture(GaugeRingPath, 256, 256, ring);

        Color32[] beamChevrons = NewPixels(64, clear);
        for (int centerX = 16; centerX <= 48; centerX += 32)
        {
            DrawLine(beamChevrons, 64, 64,
                new Vector2(centerX - 7f, 21f), new Vector2(centerX + 5f, 32f),
                3f, white);
            DrawLine(beamChevrons, 64, 64,
                new Vector2(centerX + 5f, 32f), new Vector2(centerX - 7f, 43f),
                3f, white);
        }
        WriteTexture(BeamChevronsPath, 64, 64, beamChevrons);

        Color32[] selectorFrame = new Color32[96 * 64];
        Vector2[] selectorPoints =
        {
            new(1f, 11f), new(11f, 1f), new(84f, 1f), new(94f, 11f),
            new(94f, 52f), new(84f, 62f), new(11f, 62f), new(1f, 52f)
        };
        for (int i = 0; i < selectorPoints.Length; i++)
            DrawLine(selectorFrame, 96, 64, selectorPoints[i],
                selectorPoints[(i + 1) % selectorPoints.Length], 4f, white);
        WriteTexture(SelectorFramePath, 96, 64, selectorFrame);

        Color32[] energy = NewPixels(128, clear);
        DrawDiagramTriangle(energy, HiggsBlue32, false);
        WriteTexture(CircuitEnergyPath, 128, 128, energy);

        Color32[] experimental = NewPixels(128, clear);
        DrawDiagramTriangle(experimental, new Color32(205, 83, 255, 255), true);
        WriteTexture(CircuitExperimentalPath, 128, 128, experimental);

        Color32[] phase = NewPixels(128, clear);
        Color32 steel = new(142, 160, 177, 255);
        DrawLine(phase, 128, 128, new Vector2(40f, 59f), new Vector2(40f, 42f), 7f, steel);
        DrawLine(phase, 128, 128, new Vector2(40f, 42f), new Vector2(49f, 28f), 7f, steel);
        DrawLine(phase, 128, 128, new Vector2(49f, 28f), new Vector2(64f, 22f), 7f, steel);
        DrawLine(phase, 128, 128, new Vector2(64f, 22f), new Vector2(79f, 28f), 7f, steel);
        DrawLine(phase, 128, 128, new Vector2(79f, 28f), new Vector2(88f, 42f), 7f, steel);
        DrawLine(phase, 128, 128, new Vector2(88f, 42f), new Vector2(88f, 59f), 7f, steel);
        FillRect(phase, 128, 128, 32, 55, 96, 105, new Color32(38, 49, 60, 255));
        DrawRect(phase, 128, 128, 32, 55, 96, 105, 5, steel);
        DrawCircle(phase, 128, 128, new Vector2(64f, 78f), 6f, white, true);
        DrawLine(phase, 128, 128, new Vector2(64f, 82f), new Vector2(64f, 94f), 4f, white);
        WriteTexture(CircuitPhasePath, 128, 128, phase);

        Color32 cyan = new(0, 213, 255, 255);
        Color32 violet = new(205, 83, 255, 255);
        Color32[] le = NewPixels(128, clear);
        DrawRing(le, 128, new Vector2(64f, 64f), 38f, 2f, soft);
        DrawRing(le, 128, new Vector2(64f, 64f), 25f, 3f, cyan);
        DrawCircle(le, 128, 128, new Vector2(64f, 64f), 10f, white, true);
        DrawLine(le, 128, 128, new Vector2(14f, 64f), new Vector2(114f, 64f), 2f, cyan);
        DrawLine(le, 128, 128, new Vector2(64f, 14f), new Vector2(64f, 114f), 2f, cyan);
        WriteTexture(ResourceLePath, 128, 128, le);

        Color32[] traces = NewPixels(128, clear);
        DrawRing(traces, 128, new Vector2(64f, 64f), 35f, 3f, violet);
        DrawRing(traces, 128, new Vector2(64f, 64f), 18f, 2f, soft);
        DrawLine(traces, 128, 128, new Vector2(64f, 16f), new Vector2(64f, 112f), 2f, violet);
        DrawLine(traces, 128, 128, new Vector2(16f, 64f), new Vector2(112f, 64f), 2f, violet);
        DrawCircle(traces, 128, 128, new Vector2(64f, 64f), 5f, white, true);
        WriteTexture(ResourceTracesPath, 128, 128, traces);
    }

    private static void DrawDiagramTriangle(Color32[] pixels, Color32 color, bool inverted)
    {
        Vector2 a = inverted ? new Vector2(64f, 100f) : new Vector2(64f, 20f);
        Vector2 b = inverted ? new Vector2(22f, 30f) : new Vector2(22f, 98f);
        Vector2 c = inverted ? new Vector2(106f, 30f) : new Vector2(106f, 98f);
        DrawLine(pixels, 128, 128, a, b, 4f, color);
        DrawLine(pixels, 128, 128, b, c, 4f, color);
        DrawLine(pixels, 128, 128, c, a, 4f, color);
        foreach (Vector2 point in new[] { a, b, c })
        {
            DrawCircle(pixels, 128, 128, point, 10f, color, true);
            DrawCircle(pixels, 128, 128, point, 4f, new Color32(235, 250, 255, 255), true);
        }
    }

    private static Color32[] NewPixels(int count, Color32 color)
    {
        Color32[] pixels = new Color32[count * count];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        return pixels;
    }

    private static void DrawRing(
        Color32[] pixels, int size, Vector2 center, float radius, float thickness, Color32 color)
    {
        float inner = Mathf.Max(0f, radius - thickness * 0.5f);
        float outer = radius + thickness * 0.5f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance >= inner && distance <= outer)
                    pixels[y * size + x] = color;
            }
    }

    private static void DrawCircle(
        Color32[] pixels, int width, int height, Vector2 center,
        float radius, Color32 color, bool filled)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radius - 1f));
        int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(center.x + radius + 1f));
        int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radius - 1f));
        int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(center.y + radius + 1f));
        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (filled ? distance <= radius : Mathf.Abs(distance - radius) <= 1.5f)
                    pixels[y * width + x] = color;
            }
    }

    private static void DrawLine(
        Color32[] pixels, int width, int height,
        Vector2 from, Vector2 to, float thickness, Color32 color)
    {
        Vector2 segment = to - from;
        float lengthSquared = Mathf.Max(0.001f, segment.sqrMagnitude);
        int minX = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(from.x, to.x) - thickness));
        int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(Mathf.Max(from.x, to.x) + thickness));
        int minY = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(from.y, to.y) - thickness));
        int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(Mathf.Max(from.y, to.y) + thickness));
        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                Vector2 point = new(x, y);
                float t = Mathf.Clamp01(Vector2.Dot(point - from, segment) / lengthSquared);
                if (Vector2.Distance(point, from + segment * t) <= thickness * 0.5f)
                    pixels[y * width + x] = color;
            }
    }

    private static void FillRect(
        Color32[] pixels, int width, int height,
        int left, int bottom, int right, int top, Color32 color)
    {
        for (int y = Mathf.Max(0, bottom); y <= Mathf.Min(height - 1, top); y++)
            for (int x = Mathf.Max(0, left); x <= Mathf.Min(width - 1, right); x++)
                pixels[y * width + x] = color;
    }

    private static void DrawRect(
        Color32[] pixels, int width, int height,
        int left, int bottom, int right, int top, int thickness, Color32 color)
    {
        FillRect(pixels, width, height, left, bottom, right, bottom + thickness, color);
        FillRect(pixels, width, height, left, top - thickness, right, top, color);
        FillRect(pixels, width, height, left, bottom, left + thickness, top, color);
        FillRect(pixels, width, height, right - thickness, bottom, right, top, color);
    }

    private static void WriteTexture(
        string assetPath, int width, int height, Color32[] pixels)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false, false);
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        byte[] png = texture.EncodeToPNG();
        UnityEngine.Object.DestroyImmediate(texture);
        string absolutePath = Path.GetFullPath(assetPath);
        if (!File.Exists(absolutePath) || !BytesEqual(File.ReadAllBytes(absolutePath), png))
            File.WriteAllBytes(absolutePath, png);
    }

    private static bool BytesEqual(byte[] left, byte[] right)
    {
        if (left.Length != right.Length) return false;
        for (int i = 0; i < left.Length; i++)
            if (left[i] != right[i]) return false;
        return true;
    }

    private static void ApplyFont(Transform root, VerticalUiTheme theme)
    {
        if (root == null || theme == null || theme.primaryFont == null) return;
        foreach (TextMeshProUGUI text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            text.font = theme.primaryFont;
    }

    private static void SetGeneratedDecorationActive(
        Transform parent, string name, bool active)
    {
        Transform decoration = parent.Find(name);
        if (decoration != null) decoration.gameObject.SetActive(active);
    }

    private static void CreateTitleAccents(Transform frame, Color accent)
    {
        Image left = CreateImage("AccentLeft", frame, null, accent);
        left.rectTransform.anchorMin = new Vector2(0.10f, 0.5f);
        left.rectTransform.anchorMax = new Vector2(0.29f, 0.5f);
        left.rectTransform.sizeDelta = new Vector2(0f, 3f);
        left.rectTransform.anchoredPosition = Vector2.zero;
        Image right = CreateImage("AccentRight", frame, null, accent);
        right.rectTransform.anchorMin = new Vector2(0.71f, 0.5f);
        right.rectTransform.anchorMax = new Vector2(0.90f, 0.5f);
        right.rectTransform.sizeDelta = new Vector2(0f, 3f);
        right.rectTransform.anchoredPosition = Vector2.zero;
    }

    private static void CreateCorner(
        string name,
        Transform parent,
        Vector2 anchor,
        Vector2 direction,
        Color color)
    {
        GameObject root = GetOrCreateUi(name, parent);
        RectTransform rect = (RectTransform)root.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = new Vector2(direction.x * 12f, direction.y * 12f);
        rect.sizeDelta = new Vector2(68f, 68f);

        Image horizontal = CreateImage("Horizontal", root.transform, null, color);
        SetCentered(horizontal.rectTransform,
            new Vector2(direction.x * 7f, 0f), new Vector2(54f, 3f));
        Image vertical = CreateImage("Vertical", root.transform, null, color);
        SetCentered(vertical.rectTransform,
            new Vector2(0f, direction.y * 7f), new Vector2(3f, 54f));
    }

    private static Image CreateImage(
        string name,
        Transform parent,
        Sprite sprite,
        Color color)
    {
        GameObject go = GetOrCreateUi(name, parent);
        Image image = GetOrAdd<Image>(go);
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static RawImage CreateRawImage(
        string name,
        Transform parent,
        Texture texture,
        Color color)
    {
        GameObject go = GetOrCreateUi(name, parent);
        Image legacyImage = go.GetComponent<Image>();
        if (legacyImage != null)
            UnityEngine.Object.DestroyImmediate(legacyImage);
        RawImage image = GetOrAdd<RawImage>(go);
        image.texture = texture;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        string value,
        float size,
        TextAlignmentOptions alignment,
        Color color,
        VerticalUiTheme theme)
    {
        GameObject go = GetOrCreateUi(name, parent);
        TextMeshProUGUI text = GetOrAdd<TextMeshProUGUI>(go);
        text.text = value;
        text.font = theme.primaryFont != null ? theme.primaryFont : TMP_Settings.defaultFontAsset;
        text.fontSize = size;
        text.fontSizeMax = size;
        text.fontSizeMin = Mathf.Max(11f, size - 7f);
        text.enableAutoSizing = true;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.margin = new Vector4(2f, 2f, 2f, 2f);
        return text;
    }

    private static GameObject GetOrCreateUi(string name, Transform parent)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;
        GameObject go = new GameObject(name,
            typeof(RectTransform), typeof(CanvasRenderer));
        go.layer = 5;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static T GetOrAdd<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        return component != null ? component : go.AddComponent<T>();
    }

    private static void SetTopRect(
        RectTransform rect,
        float top,
        float height,
        float left,
        float right)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2((left + right) * 0.5f, -top);
        rect.sizeDelta = new Vector2(right - left, height);
    }

    private static void SetCentered(
        RectTransform rect,
        Vector2 position,
        Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void SetAnchors(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Stretch(RectTransform rect, float inset)
    {
        Stretch(rect, inset, inset, inset, inset);
    }

    private static void Stretch(
        RectTransform rect,
        float left,
        float bottom,
        float right,
        float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    private static GameObject FindUnique(Scene scene, string name)
    {
        GameObject match = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name != name) continue;
                if (match != null)
                    throw new InvalidOperationException(name + " está duplicado.");
                match = current.gameObject;
            }
        }
        return match;
    }

    private static Color Hex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif
