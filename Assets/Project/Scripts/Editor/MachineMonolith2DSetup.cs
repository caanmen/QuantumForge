#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MachineMonolith2DSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";
    private const string FontPath =
        "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";
    private const string ModuleCardPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_module_card_metal_v2.png";
    private const string ResourceFramePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_counter_metal_v2.png";
    private const string SelectorFramePath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_selector_frame.png";
    private const string LeIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_le.png";
    private const string TracesIconPath =
        "Assets/Project/UI/Vertical/GenerationPolish/qf_resource_traces.png";
    private const string OverviewPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_overview_progression_clean_v03.png";
    private const string LabBackgroundPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_destroyed_industrial_lab_background_square_v06.png";
    private const string CloseLabBackgroundPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_destroyed_lab_close_dolly_square_v27.png";
    private const string EntryLightPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_entry_light_depth_pulse_emission_v01.png";
    private const string SymbolShaderPath =
        "Assets/Project/Shaders/UI/MachineMonolithSymbolKey.shader";
    private const string BlackKeyShaderPath =
        "Assets/Project/Shaders/UI/MachineMonolithBlackKey.shader";
    private const string LightKeyShaderPath =
        "Assets/Project/Shaders/UI/MachineMonolithLightKey.shader";
    private const string OverviewCutoutShaderPath =
        "Assets/Project/Shaders/UI/MachineMonolithOverviewCutout.shader";
    private const string AdditiveLightShaderPath =
        "Assets/Project/Shaders/UI/MachineMonolithAdditiveLight.shader";
    private const string FittedSupportShaderPath =
        "Assets/Project/Shaders/UI/MachineMonolithFittedSupport.shader";
    private const string FittedSupportBackTexturePath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_fitted_pedestal_back_v39.png";
    private const string FittedSupportFrontTexturePath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_fitted_pedestal_front_v39.png";
    private const string SymbolMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_symbol_key.mat";
    private const string BlackKeyMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_black_key.mat";
    private const string OverviewCutoutMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_overview_cutout.mat";
    private const string LightKeyMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_light_key.mat";
    private const string AdditiveLightMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_entry_light_additive.mat";
    private const string FittedSupportBackMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_fitted_support_back.mat";
    private const string FittedSupportFrontMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Monolith2D/machine_monolith_fitted_support_front.mat";
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

    private static readonly string[] SectorPaths =
    {
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_1_exposed_surface_progression_v35.png",
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_2_exposed_surface_progression_v43.png",
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_3_lower_right_progression_aligned_v57.png",
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_4_lower_left_progression_v55.png"
    };

    private static readonly string[] SectorSymbolPaths =
    {
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_1_progression_v02.png",
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_1_progression_v02.png",
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_1_progression_v02.png",
        "Assets/Project/UI/Vertical/Machine/Monolith2D/monolith_sector_4_reserved.png"
    };

    private static readonly Color Background = Hex("010609");
    private static readonly Color Panel = Hex("071017", 246);
    private static readonly Color TextPrimary = Hex("E8EAEC");
    private static readonly Color TextSecondary = Hex("87929B");
    private static readonly Color Cyan = Hex("5ADFFF");
    private static readonly Color Purple = Hex("A94CFF");
    private const float OverviewPedestalAspectRatio = .96f;
    private static readonly Vector2 OverviewPedestalPosition = new Vector2(0f, -2f);

    [MenuItem("Tools/Quantum Forge/Machine/Configure Monolith 2D")]
    public static void Configure()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ConfigureTextureImports();

        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Room2PanelUI room = UnityEngine.Object.FindFirstObjectByType<Room2PanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "No se encontró MachinePanelUI en Main.unity.");
        Require(room != null, "No se encontró Room2PanelUI en Main.unity.");
        Require(panel.gameObject.name == "MachinePanelRoot",
            "MachinePanelUI no pertenece a MachinePanelRoot.");

        GameObject panelObject = panel.gameObject;
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        Require(panelRect != null, "MachinePanelRoot no tiene RectTransform.");
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(-16f, -20f);

        DestroyChild(panelObject.transform, "MachineCubeVisualRoot");
        DestroyChild(panelObject.transform, "MachineMonolith2DRoot");
        DestroyComponent<MachineCubeVisualUI>(panelObject);
        DestroyComponent<MachineMonolith2DVisualUI>(panelObject);

        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        TMP_FontAsset font = theme != null && theme.primaryFont != null
            ? theme.primaryFont
            : AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        Require(font != null, "No se encontró la fuente canónica de la UI vertical.");

        Sprite moduleCard = AssetDatabase.LoadAssetAtPath<Sprite>(ModuleCardPath);
        Sprite resourceFrame = AssetDatabase.LoadAssetAtPath<Sprite>(ResourceFramePath);
        Sprite selectorFrame = AssetDatabase.LoadAssetAtPath<Sprite>(SelectorFramePath);
        Sprite leIcon = AssetDatabase.LoadAssetAtPath<Sprite>(LeIconPath);
        Sprite tracesIcon = AssetDatabase.LoadAssetAtPath<Sprite>(TracesIconPath);
        Require(moduleCard != null && resourceFrame != null && selectorFrame != null,
            "Faltan los marcos metálicos aprobados de Generación/Mejoras.");
        Texture2D overviewTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(OverviewPath);
        Texture2D labBackground = AssetDatabase.LoadAssetAtPath<Texture2D>(LabBackgroundPath);
        Texture2D closeLabBackground = AssetDatabase.LoadAssetAtPath<Texture2D>(
            CloseLabBackgroundPath);
        Texture2D entryLightTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(EntryLightPath);
        Texture2D supportBackTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            FittedSupportBackTexturePath);
        Texture2D supportFrontTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            FittedSupportFrontTexturePath);
        Texture2D[] sectorTextures = new Texture2D[SectorPaths.Length];
        Texture2D[] sectorSymbolTextures = new Texture2D[SectorSymbolPaths.Length];
        for (int i = 0; i < SectorPaths.Length; i++)
        {
            sectorTextures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(SectorPaths[i]);
            sectorSymbolTextures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(
                SectorSymbolPaths[i]);
        }
        Require(overviewTexture != null, "Falta el arte aprobado del Monolito completo.");
        Require(labBackground != null, "Falta el fondo aprobado del laboratorio abandonado.");
        Require(closeLabBackground != null,
            "Falta el fondo cercano aprobado del laboratorio abandonado.");
        Require(entryLightTexture != null, "Falta la luz de entrada aprobada del Monolito.");
        Require(supportBackTexture != null && supportFrontTexture != null,
            "Faltan las capas transparentes V39 del pedestal aprobado.");
        for (int i = 0; i < sectorTextures.Length; i++)
        {
            Require(sectorTextures[i] != null, "Falta el arte aprobado del sector " + (i + 1));
            Require(sectorSymbolTextures[i] != null,
                "Falta la fuente canónica de símbolos del sector " + (i + 1));
        }

        Material symbolMaterial = GetOrCreateMaterial(SymbolMaterialPath,
            "UI/QuantumForge/MonolithSymbolKey");
        Material lightKeyMaterial = GetOrCreateMaterial(LightKeyMaterialPath,
            "UI/QuantumForge/MonolithLightKey");
        Material additiveLightMaterial = GetOrCreateMaterial(
            AdditiveLightMaterialPath,
            "UI/QuantumForge/MonolithAdditiveLight");
        Material supportBackMaterial = GetOrCreateMaterial(
            FittedSupportBackMaterialPath,
            "UI/QuantumForge/MonolithFittedSupport");
        Material supportFrontMaterial = GetOrCreateMaterial(
            FittedSupportFrontMaterialPath,
            "UI/QuantumForge/MonolithFittedSupport");
        supportBackMaterial.SetFloat("_Layer", 0f);
        supportFrontMaterial.SetFloat("_Layer", 1f);
        supportBackMaterial.SetFloat("_FootprintStage", 0f);
        supportFrontMaterial.SetFloat("_FootprintStage", 0f);
        supportBackMaterial.SetTexture("_FootprintTex", overviewTexture);
        supportFrontMaterial.SetTexture("_FootprintTex", overviewTexture);
        supportBackMaterial.SetTexture("_SupportTex", supportBackTexture);
        supportFrontMaterial.SetTexture("_SupportTex", supportFrontTexture);
        EditorUtility.SetDirty(supportBackMaterial);
        EditorUtility.SetDirty(supportFrontMaterial);
        Sprite[] effectIcons = new Sprite[EffectIconPaths.Length];
        for (int i = 0; i < effectIcons.Length; i++)
            effectIcons[i] = AssetDatabase.LoadAssetAtPath<Sprite>(EffectIconPaths[i]);

        GameObject root = CreateRect("MachineMonolith2DRoot", panelObject.transform,
            Vector2.zero, Vector2.one);
        Image background = root.AddComponent<Image>();
        background.color = Background;
        background.raycastTarget = true;

        if (theme != null && theme.backgroundGrid != null)
        {
            GameObject gridObject = CreateRect("CircuitGrid", root.transform,
                Vector2.zero, Vector2.one);
            Image grid = gridObject.AddComponent<Image>();
            grid.sprite = theme.backgroundGrid;
            grid.type = Image.Type.Tiled;
            grid.color = new Color(.08f, .34f, .48f, .10f);
            grid.raycastTarget = false;
        }

        HeaderParts header = BuildHeader(root.transform, font, resourceFrame,
            leIcon, tracesIcon);
        TabParts tabs = BuildTabs(root.transform, font, selectorFrame);
        GameObject primary = CreateRect("MachinePrimaryContent", root.transform,
            Vector2.zero, Vector2.one);
        ViewHeaderParts viewHeader = BuildViewHeader(primary.transform, font, selectorFrame);

        GameObject viewport = CreateRect("MonolithViewport", primary.transform,
            new Vector2(.035f, .225f), new Vector2(.965f, .795f));
        Image viewportBackground = viewport.AddComponent<Image>();
        viewportBackground.color = new Color(.003f, .012f, .018f, .96f);
        viewportBackground.raycastTarget = false;
        viewport.AddComponent<RectMask2D>();

        BackgroundParts backgrounds = BuildLabBackground(viewport.transform,
            labBackground, closeLabBackground);

        SupportForegroundParts supportBackground = BuildOverviewSupport(
            viewport.transform, "OverviewFittedSupportBack", supportBackMaterial);
        OverviewParts overview = BuildOverview(viewport.transform, overviewTexture,
            lightKeyMaterial, entryLightTexture, additiveLightMaterial);
        SupportForegroundParts supportForeground = BuildOverviewSupport(
            viewport.transform, "OverviewFittedSupportFront", supportFrontMaterial);
        SectorParts sector = BuildSector(viewport.transform, sectorTextures[0],
            sectorSymbolTextures[0], symbolMaterial, font);
        CanvasGroup transitionVeil = BuildTransitionVeil(viewport.transform);
        CardParts card = BuildNodeCard(primary.transform, font, resourceFrame,
            moduleCard, selectorFrame, effectIcons);
        // Mezclas es el único propietario de su botón Volver. Reutilizar el
        // botón canónico evita que una reconstrucción aislada del Monolito
        // cree un duplicado sin el listener configurado por MachinePanelUI.
        Button fusionBack = room.transform.Find(
            "LegacyFusionPanel/FusionBackToNodes")?.GetComponent<Button>();

        MachineMonolith2DVisualUI visual = panelObject.AddComponent<MachineMonolith2DVisualUI>();
        SerializedObject visualSo = new SerializedObject(visual);
        SetObject(visualSo, "machinePanel", panel);
        SetObject(visualSo, "visualContentRoot", root);
        SetObject(visualSo, "primaryContentRoot", primary);
        SetObject(visualSo, "overviewRoot", overview.root);
        SetObject(visualSo, "sectorRoot", sector.root);
        SetObject(visualSo, "nodeCardRoot", card.root);
        SetObject(visualSo, "viewportRect", viewport.GetComponent<RectTransform>());
        SetObject(visualSo, "overviewLabBackgroundRoot", backgrounds.overviewRoot);
        SetObject(visualSo, "closeLabBackgroundRoot", backgrounds.closeRoot);
        SetObject(visualSo, "overviewLabBackgroundRect", backgrounds.overviewRect);
        visualSo.FindProperty("overviewLabBackgroundRestPosition").vector2Value =
            backgrounds.overviewRect.anchoredPosition;
        visualSo.FindProperty("overviewLabBackgroundRestScale").vector3Value =
            backgrounds.overviewRect.localScale;
        SetObject(visualSo, "overviewLabBackgroundGroup", backgrounds.overviewGroup);
        SetObject(visualSo, "closeLabBackgroundGroup", backgrounds.closeGroup);
        SetObject(visualSo, "overviewSupportBackgroundRoot", supportBackground.root);
        SetObject(visualSo, "overviewSupportBackgroundRect", supportBackground.rect);
        visualSo.FindProperty("overviewSupportBackgroundRestPosition").vector2Value =
            supportBackground.rect.anchoredPosition;
        visualSo.FindProperty("overviewSupportBackgroundRestScale").vector3Value =
            supportBackground.rect.localScale;
        SetObject(visualSo, "overviewSupportBackgroundGroup", supportBackground.group);
        SetObject(visualSo, "overviewSupportForegroundRoot", supportForeground.root);
        SetObject(visualSo, "overviewSupportForegroundRect", supportForeground.rect);
        visualSo.FindProperty("overviewSupportForegroundRestPosition").vector2Value =
            supportForeground.rect.anchoredPosition;
        visualSo.FindProperty("overviewSupportForegroundRestScale").vector3Value =
            supportForeground.rect.localScale;
        SetObject(visualSo, "overviewSupportForegroundGroup", supportForeground.group);
        SetObject(visualSo, "overviewArtwork", overview.artwork);
        visualSo.FindProperty("overviewRestPosition").vector2Value =
            overview.artwork.rectTransform.anchoredPosition;
        visualSo.FindProperty("overviewRestScale").vector3Value =
            overview.artwork.rectTransform.localScale;
        SetObject(visualSo, "sectorArtwork", sector.artwork);
        SetObject(visualSo, "overviewCanvasGroup", overview.canvasGroup);
        SetObject(visualSo, "sectorCanvasGroup", sector.canvasGroup);
        SetObject(visualSo, "transitionVeilGroup", transitionVeil);
        SetObject(visualSo, "overviewProgressionSheet", overviewTexture);
        SetObjectArray(visualSo, "sectorProgressionSheets", sectorTextures);
        SetObjectArray(visualSo, "sectorSymbolSheets", sectorSymbolTextures);
        SetObjectArray(visualSo, "sectorButtons", overview.sectorButtons);
        SetObjectArray(visualSo, "overviewEntryLights", overview.entryLights);
        SetObject(visualSo, "backToOverviewButton", viewHeader.back);
        SetObjectArray(visualSo, "nodeButtons", sector.nodeButtons);
        SetObjectArray(visualSo, "nodeSymbolImages", sector.symbolImages);
        SetObject(visualSo, "reservedFaceMarker", sector.reservedMarker);
        SetObject(visualSo, "viewTitleText", viewHeader.title);
        SetObject(visualSo, "viewIndexText", viewHeader.index);
        SetObject(visualSo, "leResourceText", header.le);
        SetObject(visualSo, "tracesResourceText", header.traces);
        SetObject(visualSo, "globalProgressText", header.progress);
        SetObject(visualSo, "convergenceText", header.convergence);
        SetObject(visualSo, "globalProgressFill", header.progressFill);
        SetObject(visualSo, "selectedIconImage", card.pictogram);
        SetObject(visualSo, "selectedIconText", card.icon);
        SetObject(visualSo, "selectedNameText", card.name);
        SetObject(visualSo, "selectedStateText", card.state);
        SetObject(visualSo, "selectedDescriptionText", card.description);
        SetObject(visualSo, "selectedEffectText", card.effect);
        SetObject(visualSo, "selectedRequirementsText", card.requirements);
        SetObject(visualSo, "selectedCostText", card.cost);
        SetObject(visualSo, "selectedSectorProgressText", card.sectorProgress);
        SetIconSprites(visualSo, effectIcons);
        visualSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject panelSo = new SerializedObject(panel);
        SetObject(panelSo, "cubeVisual", null);
        SetObject(panelSo, "monolithVisual", visual);
        SetObject(panelSo, "btnNodesTab", tabs.nodes);
        SetObject(panelSo, "btnFusionPanel", tabs.mixes);
        SetObject(panelSo, "btnSeedsTab", null);
        SetObject(panelSo, "btnPrestigeTab", tabs.prestige);
        SetObject(panelSo, "btnPrevNode", null);
        SetObject(panelSo, "btnNextNode", null);
        SetObject(panelSo, "btnRepairNode", card.repair);
        SetObject(panelSo, "btnAnalyzeNode", card.analyze);
        SetObject(panelSo, "selectedNodeText", card.name);
        SetObject(panelSo, "nodeAnalysisText", card.description);
        SetObject(panelSo, "btnBackToNodesFromFusion", fusionBack);
        SetObject(panelSo, "btnOpenSeedsPanel", null);
        SetObject(panelSo, "btnZone1", null);
        SetObject(panelSo, "btnZone2", null);
        SetObject(panelSo, "btnZone3", null);
        SetObject(panelSo, "btnZone4", null);
        panelSo.ApplyModifiedPropertiesWithoutUndo();

        DisableLegacyRoot(panelObject.transform, "MachineRepairViewRoot");
        DisableLegacyRoot(panelObject.transform, "MachineNodeViewRoot");
        DisableLegacyRoot(panelObject.transform, "BtnFusionPanel");
        DisableLegacyRoot(panelObject.transform, "BtnBackToNodesFromFusion");
        DisableLegacyRoot(panelObject.transform, "MachineDiagnostics_Text");
        DisableLegacyRoot(panelObject.transform, "BtnAnalyzeNode");
        DisableLegacyRoot(panelObject.transform, "NodeAnalysis_Text");
        DisableLegacyRoot(panelObject.transform, "BtnInstantHelp");
        DisableLegacyRoot(panelObject.transform, "InstantSeedsViewRoot");

        PrestigeMachineScreenSetup.ConfigureInOpenScene(scene);

        EditorUtility.SetDirty(panelObject);
        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Machine Monolith 2D] CONFIGURED | overview + 4 faces | 8 + 8 + 8 + 0 progressive signatures | original data zones and progression preserved | canonical 2D art");
    }

    public static void ConfigureBatch()
    {
        Configure();
        MachineFusionPanelVisualSetup.Configure();
        Validate();
    }

    [MenuItem("Tools/Quantum Forge/Machine/Configure Approved V39 Overview Pedestal Only")]
    public static void ConfigureOverviewSupportOnly()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ConfigureTextureImport(OverviewPath);
        ConfigureTextureImport(LabBackgroundPath);
        ConfigureTextureImport(FittedSupportBackTexturePath, 2048, true);
        ConfigureTextureImport(FittedSupportFrontTexturePath, 2048, true);

        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        MachineMonolith2DVisualUI visual = UnityEngine.Object.FindFirstObjectByType<MachineMonolith2DVisualUI>(
            FindObjectsInactive.Include);
        Require(panel != null && visual != null,
            "La escena no contiene la integración 2D existente del Monolito.");

        Transform viewport = panel.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport");
        Transform overviewRoot = viewport?.Find("MonolithOverview");
        RawImage overviewBackground = viewport?.Find("HumanLabOverviewBackground")
            ?.GetComponent<RawImage>();
        Require(viewport != null && overviewRoot != null && overviewBackground != null,
            "Falta la jerarquía existente de la vista general del Monolito.");

        Texture2D overviewTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(OverviewPath);
        Texture2D labBackground = AssetDatabase.LoadAssetAtPath<Texture2D>(LabBackgroundPath);
        Texture2D supportBackTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            FittedSupportBackTexturePath);
        Texture2D supportFrontTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            FittedSupportFrontTexturePath);
        Require(overviewTexture != null && labBackground != null &&
                supportBackTexture != null && supportFrontTexture != null,
            "Faltan assets canónicos o capas V39 del pedestal.");

        overviewBackground.texture = labBackground;
        overviewBackground.raycastTarget = false;
        EditorUtility.SetDirty(overviewBackground);

        DestroyChild(viewport, "OverviewSupportForeground");
        DestroyChild(viewport, "OverviewFittedSupportBack");
        DestroyChild(viewport, "OverviewFittedSupportFront");

        Material supportBackMaterial = GetOrCreateMaterial(
            FittedSupportBackMaterialPath,
            "UI/QuantumForge/MonolithFittedSupport");
        Material supportFrontMaterial = GetOrCreateMaterial(
            FittedSupportFrontMaterialPath,
            "UI/QuantumForge/MonolithFittedSupport");
        supportBackMaterial.SetFloat("_Layer", 0f);
        supportFrontMaterial.SetFloat("_Layer", 1f);
        supportBackMaterial.SetFloat("_FootprintStage", 0f);
        supportFrontMaterial.SetFloat("_FootprintStage", 0f);
        supportBackMaterial.SetTexture("_FootprintTex", overviewTexture);
        supportFrontMaterial.SetTexture("_FootprintTex", overviewTexture);
        supportBackMaterial.SetTexture("_SupportTex", supportBackTexture);
        supportFrontMaterial.SetTexture("_SupportTex", supportFrontTexture);
        EditorUtility.SetDirty(supportBackMaterial);
        EditorUtility.SetDirty(supportFrontMaterial);

        int overviewIndex = overviewRoot.GetSiblingIndex();
        SupportForegroundParts supportBack = BuildOverviewSupport(
            viewport, "OverviewFittedSupportBack", supportBackMaterial);
        supportBack.root.transform.SetSiblingIndex(overviewIndex);
        SupportForegroundParts supportFront = BuildOverviewSupport(
            viewport, "OverviewFittedSupportFront", supportFrontMaterial);
        supportFront.root.transform.SetSiblingIndex(overviewRoot.GetSiblingIndex() + 1);

        SerializedObject visualSo = new SerializedObject(visual);
        SetObject(visualSo, "overviewSupportBackgroundRoot", supportBack.root);
        SetObject(visualSo, "overviewSupportBackgroundRect", supportBack.rect);
        visualSo.FindProperty("overviewSupportBackgroundRestPosition").vector2Value =
            supportBack.rect.anchoredPosition;
        visualSo.FindProperty("overviewSupportBackgroundRestScale").vector3Value =
            supportBack.rect.localScale;
        SetObject(visualSo, "overviewSupportBackgroundGroup", supportBack.group);
        SetObject(visualSo, "overviewSupportForegroundRoot", supportFront.root);
        SetObject(visualSo, "overviewSupportForegroundRect", supportFront.rect);
        visualSo.FindProperty("overviewSupportForegroundRestPosition").vector2Value =
            supportFront.rect.anchoredPosition;
        visualSo.FindProperty("overviewSupportForegroundRestScale").vector3Value =
            supportFront.rect.localScale;
        SetObject(visualSo, "overviewSupportForegroundGroup", supportFront.group);
        visualSo.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(visual);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Machine Monolith 2D] V39 OVERVIEW PEDESTAL CONFIGURED | V06 lab preserved | canonical Monolith transform preserved | visible back bed + two-tier front transparent atlases");
    }

    public static void ConfigureOverviewSupportBatch()
    {
        ConfigureOverviewSupportOnly();
        ValidateOverviewSupportOnly();
    }

    public static void ValidateOverviewSupportOnly()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "No se encontró MachinePanelUI en Main.unity.");
        Transform viewport = panel.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport");
        RawImage background = viewport?.Find("HumanLabOverviewBackground")
            ?.GetComponent<RawImage>();
        RawImage supportBack = viewport?.Find("OverviewFittedSupportBack")
            ?.GetComponent<RawImage>();
        RawImage supportFront = viewport?.Find("OverviewFittedSupportFront")
            ?.GetComponent<RawImage>();
        RectTransform overviewArtwork = viewport?.Find(
            "MonolithOverview/OverviewArtwork") as RectTransform;
        Require(background != null && background.texture != null &&
                AssetDatabase.GetAssetPath(background.texture) == LabBackgroundPath,
            "La vista general no conserva el laboratorio limpio V06.");
        Require(IsFittedSupport(supportBack, 0f, FittedSupportBackTexturePath) &&
                IsFittedSupport(supportFront, 1f, FittedSupportFrontTexturePath),
            "Las capas transparentes V39 del pedestal no están conectadas correctamente.");
        Require(overviewArtwork != null &&
                Mathf.Abs(overviewArtwork.localScale.x -
                    MachineMonolith2DVisualUI.OverviewDisplayScale) < .01f &&
                Vector2.Distance(overviewArtwork.anchoredPosition,
                    MachineMonolith2DVisualUI.OverviewDisplayPosition) < 1f,
            "El Monolito dejó de conservar la escala 0,72 o la posición (0,34).");
        Require(HasFittedPedestalTransform(supportBack.rectTransform) &&
                HasFittedPedestalTransform(supportFront.rectTransform),
            "El pedestal V39 no conserva su extensión vertical y línea de contacto.");
        Require(supportBack.transform.GetSiblingIndex() <
                    overviewArtwork.parent.GetSiblingIndex() &&
                supportFront.transform.GetSiblingIndex() >
                    overviewArtwork.parent.GetSiblingIndex(),
            "El orden debe ser cama trasera, Monolito y labio frontal.");
        Require(supportBack.raycastTarget == false && supportFront.raycastTarget == false,
            "Las capas decorativas del receptáculo no deben bloquear el toque.");
        Require(viewport.Find("OverviewSupportForeground") == null,
            "La capa frontal raster antigua sigue presente.");
        Require(scene.isLoaded, "Main.unity no quedó cargada para validar.");
        Debug.Log("[Machine Monolith 2D] V39 OVERVIEW PEDESTAL VALIDATION PASS | V06 pixel authority | canonical V03 Monolith | locked 0.72 + (0,34) | visible shallow bed behind | compact two-tier front ahead | non-interactive layers");
    }

    [MenuItem("Tools/Quantum Forge/Machine/Validate Monolith 2D")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Room2PanelUI room = UnityEngine.Object.FindFirstObjectByType<Room2PanelUI>(
            FindObjectsInactive.Include);
        MachineMonolith2DVisualUI visual = UnityEngine.Object.FindFirstObjectByType<MachineMonolith2DVisualUI>(
            FindObjectsInactive.Include);
        Require(panel != null && room != null && visual != null,
            "La escena no contiene la integración 2D del Monolito.");
        Require(panel.transform.Find("MachineMonolith2DRoot") != null,
            "Falta MachineMonolith2DRoot.");
        Require(panel.transform.Find("MachineCubeVisualRoot") == null,
            "La composición antigua del cubo sigue presente.");
        Require(panel.GetComponent<MachineCubeVisualUI>() == null,
            "MachineCubeVisualUI sigue siendo propietario visual de la pantalla.");

        SerializedObject visualSo = new SerializedObject(visual);
        Require(visualSo.FindProperty("overviewProgressionSheet").objectReferenceValue != null,
            "Falta la hoja de progresión del Monolito.");
        Require(visualSo.FindProperty("overviewLabBackgroundRoot").objectReferenceValue != null &&
            visualSo.FindProperty("closeLabBackgroundRoot").objectReferenceValue != null,
            "Deben existir fondos separados para la vista general y los cuatro sectores cercanos.");
        RawImage closeBackground = panel.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport/HumanLabCloseBackground")
            ?.GetComponent<RawImage>();
        Require(closeBackground != null && closeBackground.texture != null &&
            AssetDatabase.GetAssetPath(closeBackground.texture) == CloseLabBackgroundPath,
            "El acercamiento debe usar el laboratorio cercano canónico V27.");
        RawImage overviewBackground = panel.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport/HumanLabOverviewBackground")
            ?.GetComponent<RawImage>();
        Require(overviewBackground != null && overviewBackground.texture != null &&
            AssetDatabase.GetAssetPath(overviewBackground.texture) == LabBackgroundPath,
            "La vista general debe usar el laboratorio limpio sin receptáculo aproximado horneado.");
        string supportRoot =
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport/";
        RawImage supportBack = panel.transform.Find(supportRoot +
            "OverviewFittedSupportBack")?.GetComponent<RawImage>();
        RawImage supportFront = panel.transform.Find(supportRoot +
            "OverviewFittedSupportFront")?.GetComponent<RawImage>();
        Require(IsFittedSupport(supportBack, 0f, FittedSupportBackTexturePath) &&
                IsFittedSupport(supportFront, 1f, FittedSupportFrontTexturePath),
            "El pedestal V39 debe existir detrás y delante con sus capas transparentes aprobadas.");
        Require(visualSo.FindProperty("sectorProgressionSheets").arraySize == 4,
            "Deben existir cuatro hojas de sector.");
        SerializedProperty sectorSheets = visualSo.FindProperty("sectorProgressionSheets");
        for (int index = 0; index < SectorPaths.Length; index++)
        {
            Texture2D sheet = sectorSheets.GetArrayElementAtIndex(index)
                .objectReferenceValue as Texture2D;
            Require(sheet != null && AssetDatabase.GetAssetPath(sheet) == SectorPaths[index],
                "El sector " + (index + 1) +
                " no usa la hoja aprobada V35/V43/V55 correspondiente a su cara física.");
        }
        SerializedProperty symbolSheets = visualSo.FindProperty("sectorSymbolSheets");
        Require(symbolSheets != null && symbolSheets.arraySize == 4,
            "Deben existir cuatro referencias de hojas de firmas.");
        for (int index = 0; index < 3; index++)
        {
            Texture2D sheet = symbolSheets.GetArrayElementAtIndex(index)
                .objectReferenceValue as Texture2D;
            Require(sheet != null && AssetDatabase.GetAssetPath(sheet) ==
                    SectorSymbolPaths[0],
                "Los sectores funcionales deben reutilizar la hoja de firmas canónica de S1.");
        }
        RawImage sectorArtwork = panel.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport/MonolithSectorCloseup/SectorArtwork")
            ?.GetComponent<RawImage>();
        SerializedObject sectorArtworkSo = sectorArtwork != null
            ? new SerializedObject(sectorArtwork) : null;
        Require(sectorArtworkSo != null &&
                sectorArtworkSo.FindProperty("m_Material").objectReferenceValue == null,
            "Las caras V35/V43 deben usar su alfa real sin máscara de silueta aproximada.");
        Require(visualSo.FindProperty("sectorButtons").arraySize == 4,
            "Deben existir cuatro accesos de sector.");
        Require(visualSo.FindProperty("overviewEntryLights").arraySize == 4,
            "Deben existir cuatro luces de entrada, una por cada cara.");
        RectTransform overviewArtwork = panel.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport/MonolithOverview/OverviewArtwork")
            as RectTransform;
        Require(overviewArtwork != null &&
            Mathf.Abs(overviewArtwork.localScale.x - .72f) < .01f &&
            Vector2.Distance(overviewArtwork.anchoredPosition,
                new Vector2(0f, 34f)) < 1f,
            "El Monolito general debe conservar el tamaño y apoyo V26 bloqueados.");
        RectTransform supportBackRect = supportBack != null
            ? supportBack.rectTransform : null;
        RectTransform supportFrontRect = supportFront != null
            ? supportFront.rectTransform : null;
        Require(HasFittedPedestalTransform(supportBackRect) &&
            HasFittedPedestalTransform(supportFrontRect),
            "Las dos capas del pedestal deben conservar su extensión vertical y línea de contacto.");
        Require(supportBackRect.GetSiblingIndex() < overviewArtwork.parent.GetSiblingIndex() &&
                supportFrontRect.GetSiblingIndex() > overviewArtwork.parent.GetSiblingIndex(),
            "El receptáculo debe envolver al Monolito: fondo detrás y labio frontal delante.");
        for (int index = 0; index < 4; index++)
        {
            RectTransform light = overviewArtwork.Find(
                "FaceEntryLight_" + (index + 1)) as RectTransform;
            Require(light != null && light.parent == overviewArtwork &&
                Vector2.Distance(light.anchorMin,
                    MachineMonolith2DVisualUI.GetOverviewEntryLightAnchor(index, 0)) < .002f &&
                Vector2.Distance(light.anchorMax,
                    MachineMonolith2DVisualUI.GetOverviewEntryLightAnchor(index, 0)) < .002f &&
                Vector2.Distance(light.sizeDelta,
                    MachineMonolith2DVisualUI.GetOverviewEntryLightSize(0)) < .1f &&
                light.anchoredPosition.sqrMagnitude < .01f &&
                Vector3.Distance(light.localScale, Vector3.one) < .001f,
                "La luz " + (index + 1) + " no está integrada en su hendidura dañada.");
        }
        Require(panel.transform.Find(
                "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport/MonolithOverview/OverviewArtwork/ReservedFaceMarker") == null,
            "El marcador ??? no debe aparecer en la vista general.");
        Require(visualSo.FindProperty("nodeButtons").arraySize == 11,
            "El acercamiento debe admitir once puntos táctiles como máximo.");
        Require(visualSo.FindProperty("nodeSymbolImages").arraySize == 11,
            "Cada punto táctil debe conservar su firma alienígena original.");
        TextMeshProUGUI reservedMarker = panel.transform.Find(
            "MachineMonolith2DRoot/MachinePrimaryContent/MonolithViewport/MonolithSectorCloseup/SectorArtwork/ReservedFaceMarker")
            ?.GetComponent<TextMeshProUGUI>();
        Require(reservedMarker != null && reservedMarker.text == "???",
            "El ??? debe existir como capa separada y únicamente dentro de la cuarta cara.");
        Require(panel.transform.Find("MachineMonolith2DRoot/MachinePrimaryContent") != null,
            "Falta la separación entre cabecera persistente y contenido primario.");
        Button prestigeTab = panel.transform.Find(
            "MachineMonolith2DRoot/MachineContextTabs/PrestigeTab")?.GetComponent<Button>();
        Require(prestigeTab != null && prestigeTab.interactable,
            "Falta la tercera pestaña PRESTIGIO dentro de la Máquina.");
        Button canonicalFusionBack = room.transform.Find(
            "LegacyFusionPanel/FusionBackToNodes")?.GetComponent<Button>();
        SerializedObject panelSo = new SerializedObject(panel);
        Button wiredFusionBack = panelSo.FindProperty("btnBackToNodesFromFusion")
            ?.objectReferenceValue as Button;
        Transform duplicateFusionBack = panel.transform.Find(
            "MachineMonolith2DRoot/FusionBackToNodes");
        Require(canonicalFusionBack != null &&
                wiredFusionBack == canonicalFusionBack &&
                duplicateFusionBack == null,
            "Mezclas debe ser el único propietario del botón Volver y mantenerlo enlazado. " +
            $"canónico={(canonicalFusionBack != null ? canonicalFusionBack.name : "null")}, " +
            $"enlazado={(wiredFusionBack != null ? wiredFusionBack.name : "null")}, " +
            $"coinciden={wiredFusionBack == canonicalFusionBack}, " +
            $"duplicado={(duplicateFusionBack != null ? duplicateFusionBack.name : "null")}.");
        SerializedProperty fourthButton = visualSo.FindProperty("sectorButtons")
            .GetArrayElementAtIndex(3);
        Require(fourthButton.objectReferenceValue is Button reservedButton &&
            reservedButton.interactable,
            "El sector 4 debe permanecer oscuro, pero permitir entrar a su cara.");

        int s1 = MachineManagerDisplayCount(MachineZoneType.Room1Link);
        int s2 = MachineManagerDisplayCount(MachineZoneType.FusionSector);
        int s3 = MachineManagerDisplayCount(MachineZoneType.InternalSupport);
        int s4 = MachineManagerDisplayCount(MachineZoneType.InstantChamber);
        Require(s1 == 7 && s2 == 11 && s3 == 6 && s4 == 0,
            $"Conteos visuales incorrectos: {s1} + {s2} + {s3} + {s4}.");
        Require(MachineMonolith2DVisualUI.GetFaceDisplayNodeCount(0) == 8 &&
                MachineMonolith2DVisualUI.GetFaceDisplayNodeCount(1) == 8 &&
                MachineMonolith2DVisualUI.GetFaceDisplayNodeCount(2) == 8 &&
                MachineMonolith2DVisualUI.GetFaceDisplayNodeCount(3) == 0,
            "Las caras físicas no conservan la distribución aprobada 8 + 8 + 8 + 0.");
        for (int stage = 0; stage < 4; stage++)
        {
            int expected = new[] { 3, 5, 7, 8 }[stage];
            Require(MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(0, stage) == expected &&
                    MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(1, stage) == expected &&
                    MachineMonolith2DVisualUI.GetFaceVisibleNodeCount(2, stage) == expected,
                "La revelación progresiva 3/5/7/8 dejó de ser estable en la etapa " + stage + ".");
        }
        Require(MachineMonolith2DVisualUI.GetContentZoneForFace(0) ==
                    MachineZoneType.FusionSector &&
                MachineMonolith2DVisualUI.GetContentZoneForFace(1) ==
                    MachineZoneType.Room1Link &&
                MachineMonolith2DVisualUI.GetContentZoneForFace(2) ==
                    MachineZoneType.InternalSupport,
            "Las zonas fuente dejaron de conservar sus identificadores persistentes.");
        Require(scene.isLoaded, "Main.unity no quedó cargada para validar.");
        Debug.Log("[Machine Monolith 2D] STRUCTURE PASS | faces 1/2 upper-left/upper-right and 3/4 lower-right/lower-left | dedicated flat-top V55 lower geometry | physical faces 8 + 8 + 8 + 0 | progressive 3/5/7/8 reveal without movement | original Fusion/Room1/Internal data zones and save ids | fissure-integrated entry lights | global 20/40/60 stages | 32 px symbols + 80 px touch targets | face 4 reserved | rendered surface containment and visual approval require separate capture validation");
    }

    private static bool IsFittedSupport(RawImage image, float expectedLayer,
        string expectedSupportTexturePath)
    {
        return image != null && image.material != null && image.material.shader != null &&
            image.material.shader.name == "UI/QuantumForge/MonolithFittedSupport" &&
            image.material.HasProperty("_FootprintStage") &&
            Mathf.Abs(image.material.GetFloat("_Layer") - expectedLayer) < .01f &&
            image.material.GetTexture("_FootprintTex") != null &&
            AssetDatabase.GetAssetPath(image.material.GetTexture("_FootprintTex")) ==
                OverviewPath &&
            image.material.GetTexture("_SupportTex") != null &&
            AssetDatabase.GetAssetPath(image.material.GetTexture("_SupportTex")) ==
                expectedSupportTexturePath;
    }

    private static bool HasSameFittedTransform(RectTransform overview,
        RectTransform support)
    {
        AspectRatioFitter overviewFitter = overview != null
            ? overview.GetComponent<AspectRatioFitter>() : null;
        AspectRatioFitter supportFitter = support != null
            ? support.GetComponent<AspectRatioFitter>() : null;
        return overview != null && support != null &&
            Vector3.Distance(overview.localScale, support.localScale) < .001f &&
            Vector2.Distance(overview.anchoredPosition, support.anchoredPosition) < .01f &&
            Vector2.Distance(overview.anchorMin, support.anchorMin) < .001f &&
            Vector2.Distance(overview.anchorMax, support.anchorMax) < .001f &&
            Vector2.Distance(overview.pivot, support.pivot) < .001f &&
            Vector2.Distance(overview.sizeDelta, support.sizeDelta) < .01f &&
            overviewFitter != null && supportFitter != null &&
            overviewFitter.aspectMode == supportFitter.aspectMode &&
            Mathf.Abs(overviewFitter.aspectRatio - supportFitter.aspectRatio) < .001f &&
            overview.GetSiblingIndex() != support.GetSiblingIndex();
    }

    private static bool HasFittedPedestalTransform(RectTransform support)
    {
        AspectRatioFitter fitter = support != null
            ? support.GetComponent<AspectRatioFitter>() : null;
        return support != null && fitter != null &&
            fitter.aspectMode == AspectRatioFitter.AspectMode.FitInParent &&
            Mathf.Abs(fitter.aspectRatio - OverviewPedestalAspectRatio) < .001f &&
            Mathf.Abs(support.localScale.x -
                MachineMonolith2DVisualUI.OverviewDisplayScale) < .001f &&
            Mathf.Abs(support.localScale.y -
                MachineMonolith2DVisualUI.OverviewDisplayScale) < .001f &&
            Vector2.Distance(support.anchoredPosition, OverviewPedestalPosition) < .5f;
    }

    private static int MachineManagerDisplayCount(MachineZoneType zone)
    {
        MachineManager manager = UnityEngine.Object.FindFirstObjectByType<MachineManager>(
            FindObjectsInactive.Include);
        Require(manager != null, "MachineManager no está presente en Main.unity.");
        return manager.GetDisplayNodesByZone(zone).Count;
    }

    private static void ConfigureTextureImports()
    {
        ConfigureTextureImport(OverviewPath);
        ConfigureTextureImport(LabBackgroundPath);
        ConfigureTextureImport(CloseLabBackgroundPath);
        ConfigureTextureImport(EntryLightPath, 512);
        ConfigureTextureImport(FittedSupportBackTexturePath, 2048, true);
        ConfigureTextureImport(FittedSupportFrontTexturePath, 2048, true);
        foreach (string path in SectorPaths)
            ConfigureTextureImport(path, 2048, true);
        foreach (string path in SectorSymbolPaths)
            ConfigureTextureImport(path);
    }

    private static void ConfigureTextureImport(string path, int maxTextureSize = 2048,
        bool alphaIsTransparency = false)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        Require(importer != null, "No se pudo importar " + path);
        importer.textureType = TextureImporterType.Default;
        importer.mipmapEnabled = false;
        importer.sRGBTexture = true;
        importer.alphaIsTransparency = alphaIsTransparency;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = maxTextureSize;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
    }

    private static HeaderParts BuildHeader(Transform parent, TMP_FontAsset font,
        Sprite resourceFrame, Sprite leIcon, Sprite tracesIcon)
    {
        GameObject header = CreateRect("MachineHeader", parent,
            Vector2.zero, Vector2.one);
        GameObject lePanel = CreatePanel("MachineResource_LE", header.transform,
            new Vector2(.065f, .946f), new Vector2(.493f, .995f), resourceFrame,
            new Color(.82f, .86f, .87f, .96f));
        GameObject tracesPanel = CreatePanel("MachineResource_Traces", header.transform,
            new Vector2(.507f, .946f), new Vector2(.935f, .995f), resourceFrame,
            new Color(.82f, .86f, .87f, .96f));
        AddIcon("Icon", lePanel.transform, leIcon);
        AddIcon("Icon", tracesPanel.transform, tracesIcon);
        TextMeshProUGUI le = CreateText("LE", lePanel.transform, "LE  —",
            new Vector2(.27f, .10f), new Vector2(.94f, .90f), font, 28f,
            TextPrimary, TextAlignmentOptions.Left);
        TextMeshProUGUI traces = CreateText("Traces", tracesPanel.transform, "TRAZAS  —",
            new Vector2(.27f, .10f), new Vector2(.94f, .90f), font, 28f,
            TextPrimary, TextAlignmentOptions.Left);

        GameObject titlePanel = CreatePanel("MachineTitlePlate", header.transform,
            new Vector2(.065f, .884f), new Vector2(.935f, .942f), resourceFrame,
            new Color(.72f, .76f, .77f, .98f));
        TextMeshProUGUI title = CreateText("Title", titlePanel.transform, "MÁQUINA",
            new Vector2(.25f, .28f), new Vector2(.75f, .86f), font, 48f,
            TextPrimary, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        title.characterSpacing = 5f;
        AddTitleLight(titlePanel.transform, "LeftLight", .08f, .27f);
        AddTitleLight(titlePanel.transform, "RightLight", .73f, .92f);
        TextMeshProUGUI progress = CreateText("TotalProgress", titlePanel.transform,
            "REPARACIÓN TOTAL  0%  /  80%", new Vector2(.055f, .12f),
            new Vector2(.61f, .28f), font, 18f, TextSecondary,
            TextAlignmentOptions.Left);
        TextMeshProUGUI convergence = CreateText("Convergence", titlePanel.transform,
            string.Empty, new Vector2(.61f, .12f), new Vector2(.945f, .28f),
            font, 18f, TextSecondary, TextAlignmentOptions.Right);
        convergence.gameObject.SetActive(false);

        GameObject track = CreateRect("ProgressTrack", titlePanel.transform,
            new Vector2(.055f, .095f), new Vector2(.945f, .115f));
        Image trackImage = track.AddComponent<Image>();
        trackImage.color = Hex("17242B");
        trackImage.raycastTarget = false;
        GameObject fillObject = CreateRect("Fill", track.transform, Vector2.zero, Vector2.one);
        Image fill = fillObject.AddComponent<Image>();
        fill.color = Cyan;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillAmount = 0f;
        fill.raycastTarget = false;

        return new HeaderParts
        {
            le = le, traces = traces, progress = progress,
            convergence = convergence, progressFill = fill
        };
    }

    private static TabParts BuildTabs(Transform parent, TMP_FontAsset font,
        Sprite buttonFrame)
    {
        GameObject tabs = CreateRect("MachineContextTabs", parent,
            new Vector2(.11f, .84f), new Vector2(.89f, .88f));
        Button nodes = CreateButton("NodesTab", tabs.transform, "NODOS",
            new Vector2(0f, .04f), new Vector2(.32f, .96f), font, buttonFrame,
            Cyan, 22f);
        Button mixes = CreateButton("MixesTab", tabs.transform, "MEZCLAS",
            new Vector2(.34f, .04f), new Vector2(.66f, .96f), font, buttonFrame,
            Purple, 22f);
        Button prestige = CreateButton("PrestigeTab", tabs.transform, "PRESTIGIO",
            new Vector2(.68f, .04f), new Vector2(1f, .96f), font, buttonFrame,
            new Color(.34f, .43f, .48f, .92f), 20f);
        return new TabParts { nodes = nodes, mixes = mixes, prestige = prestige };
    }

    private static ViewHeaderParts BuildViewHeader(Transform parent, TMP_FontAsset font,
        Sprite buttonFrame)
    {
        GameObject holder = CreateRect("MonolithViewHeader", parent,
            new Vector2(.05f, .795f), new Vector2(.95f, .838f));
        Button back = CreateButton("BackToOverview", holder.transform, "‹  VOLVER",
            new Vector2(0f, .12f), new Vector2(.22f, .88f), font, buttonFrame,
            new Color(.45f, .50f, .53f, .95f), 20f);
        TextMeshProUGUI title = CreateText("ViewTitle", holder.transform, "MONOLITO",
            new Vector2(.05f, .35f), new Vector2(.95f, .98f), font, 31f,
            TextPrimary, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        title.characterSpacing = 1.8f;
        TextMeshProUGUI index = CreateText("ViewIndex", holder.transform,
            string.Empty, new Vector2(.05f, .02f), new Vector2(.95f, .40f),
            font, 20f, Cyan, TextAlignmentOptions.Center);
        back.gameObject.SetActive(true);
        return new ViewHeaderParts { back = back, title = title, index = index };
    }

    private static OverviewParts BuildOverview(Transform parent, Texture2D texture,
        Material cutoutMaterial, Texture2D entryLightTexture,
        Material additiveLightMaterial)
    {
        GameObject root = CreateRect("MonolithOverview", parent, Vector2.zero, Vector2.one);
        CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
        GameObject artObject = CreateRect("OverviewArtwork", root.transform,
            Vector2.zero, Vector2.one);
        AspectRatioFitter fitter = artObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = 1.04f;
        artObject.transform.localScale = new Vector3(
            MachineMonolith2DVisualUI.OverviewDisplayScale,
            MachineMonolith2DVisualUI.OverviewDisplayScale, 1f);
        artObject.GetComponent<RectTransform>().anchoredPosition =
            MachineMonolith2DVisualUI.OverviewDisplayPosition;
        RawImage artwork = artObject.AddComponent<RawImage>();
        artwork.texture = texture;
        artwork.uvRect = new Rect(0f, .5f, .5f, .5f);
        artwork.material = cutoutMaterial;
        artwork.raycastTarget = false;

        Vector4[] regions =
        {
            new Vector4(.14f, .47f, .49f, .92f),
            new Vector4(.51f, .47f, .86f, .92f),
            new Vector4(.51f, .08f, .86f, .50f),
            new Vector4(.14f, .08f, .49f, .50f)
        };
        Button[] sectorButtons = new Button[4];
        for (int i = 0; i < sectorButtons.Length; i++)
        {
            Vector4 region = regions[i];
            GameObject hit = CreateRect("SectorTouch_" + (i + 1), artObject.transform,
                new Vector2(region.x, region.y), new Vector2(region.z, region.w));
            Image image = hit.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, .001f);
            sectorButtons[i] = hit.AddComponent<Button>();
            sectorButtons[i].targetGraphic = image;
        }

        GameObject[] entryLights = new GameObject[4];
        for (int i = 0; i < entryLights.Length; i++)
        {
            Vector2 lightPosition = MachineMonolith2DVisualUI.GetOverviewEntryLightAnchor(i, 0);
            GameObject lightRoot = CreatePointRect("FaceEntryLight_" + (i + 1),
                artObject.transform, lightPosition, 104f);
            RectTransform lightRect = lightRoot.GetComponent<RectTransform>();
            lightRect.sizeDelta = MachineMonolith2DVisualUI.GetOverviewEntryLightSize(0);
            lightRect.localRotation = Quaternion.Euler(0f, 0f,
                MachineMonolith2DVisualUI.GetOverviewEntryLightRotation(i));
            GameObject slotObject = CreateRect("AccessSlot", lightRoot.transform,
                new Vector2(.34f, .05f), new Vector2(.66f, .95f));
            Image slot = slotObject.AddComponent<Image>();
            slot.color = new Color(.008f, .018f, .024f, .78f);
            slot.raycastTarget = false;
            BuildEntryRail(lightRoot.transform, "RailLeft",
                new Vector2(.25f, .08f), new Vector2(.33f, .92f));
            BuildEntryRail(lightRoot.transform, "RailRight",
                new Vector2(.67f, .08f), new Vector2(.75f, .92f));
            BuildEntryRail(lightRoot.transform, "CapTop",
                new Vector2(.31f, .91f), new Vector2(.69f, .97f));
            BuildEntryRail(lightRoot.transform, "CapBottom",
                new Vector2(.31f, .03f), new Vector2(.69f, .09f));

            Vector4[] brokenSegments =
            {
                new Vector4(.27f, .10f, .57f, .32f),
                new Vector4(.42f, .38f, .70f, .60f),
                new Vector4(.31f, .68f, .60f, .90f)
            };
            for (int segment = 0; segment < brokenSegments.Length; segment++)
            {
                Vector4 region = brokenSegments[segment];
                GameObject emitterObject = CreateRect(
                    "EmbeddedEmitter_" + (segment + 1), lightRoot.transform,
                    new Vector2(region.x, region.y),
                    new Vector2(region.z, region.w));
                RawImage emitter = emitterObject.AddComponent<RawImage>();
                emitter.texture = entryLightTexture;
                emitter.material = additiveLightMaterial;
                emitter.uvRect = new Rect(.28f, .16f, .44f, .68f);
                emitter.color = new Color(.68f, .92f, 1f,
                    segment == 1 ? .78f : .61f);
                emitter.raycastTarget = false;
            }
            entryLights[i] = lightRoot;
        }
        return new OverviewParts
        {
            root = root, artwork = artwork, canvasGroup = canvasGroup,
            sectorButtons = sectorButtons, entryLights = entryLights
        };
    }

    private static void BuildEntryRail(
        Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject railObject = CreateRect(name, parent, anchorMin, anchorMax);
        Image rail = railObject.AddComponent<Image>();
        rail.color = new Color(.15f, .19f, .21f, .86f);
        rail.raycastTarget = false;
    }

    private static Rect GetFinalSymbolUvRect(Vector2 position)
    {
        const float normalizedWidth = .14f;
        const float normalizedHeight = .079f;
        return new Rect(
            .5f + (position.x - normalizedWidth * .5f) * .5f,
            (position.y - normalizedHeight * .5f) * .5f,
            normalizedWidth * .5f,
            normalizedHeight * .5f);
    }

    private static SectorParts BuildSector(Transform parent, Texture2D texture,
        Texture2D symbolTexture, Material symbolMaterial, TMP_FontAsset font)
    {
        GameObject root = CreateRect("MonolithSectorCloseup", parent,
            Vector2.zero, Vector2.one);
        CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
        GameObject artObject = CreateRect("SectorArtwork", root.transform,
            Vector2.zero, Vector2.one);
        AspectRatioFitter fitter = artObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = .5625f;
        artObject.transform.localScale = new Vector3(
            MachineMonolith2DVisualUI.GetSectorCloseWidthScale(0),
            MachineMonolith2DVisualUI.SectorCloseScale, 1f);
        RawImage artwork = artObject.AddComponent<RawImage>();
        artwork.texture = texture;
        artwork.uvRect = new Rect(0f, .5f, .5f, .5f);
        artwork.material = null;
        artwork.raycastTarget = false;

        TextMeshProUGUI reservedMarker = CreateText("ReservedFaceMarker",
            artObject.transform, "???", new Vector2(.40f, .43f),
            new Vector2(.64f, .57f), font, 44f,
            new Color(.52f, .60f, .64f, .82f), TextAlignmentOptions.Center);
        reservedMarker.fontStyle = FontStyles.Bold;
        reservedMarker.characterSpacing = 5f;
        reservedMarker.raycastTarget = false;
        reservedMarker.gameObject.SetActive(false);

        Button[] buttons = new Button[11];
        RawImage[] symbolImages = new RawImage[11];
        for (int i = 0; i < buttons.Length; i++)
        {
            GameObject hit = CreatePointRect("NodeTouch_" + (i + 1).ToString("00"),
                artObject.transform, new Vector2(.5f, .5f),
                MachineMonolith2DVisualUI.NodeTouchDisplaySize);
            Image hitImage = hit.AddComponent<Image>();
            hitImage.color = new Color(0f, 0f, 0f, .001f);
            Button button = hit.AddComponent<Button>();
            button.targetGraphic = hitImage;

            GameObject symbolObject = CreatePointRect("AlienSignature",
                hit.transform, new Vector2(.5f, .5f),
                MachineMonolith2DVisualUI.NodeSymbolDisplaySize);
            RawImage symbol = symbolObject.AddComponent<RawImage>();
            symbol.texture = symbolTexture;
            symbol.material = symbolMaterial;
            symbol.color = new Color(.76f, .98f, 1f,
                MachineMonolith2DVisualUI.DamagedNodeAlpha);
            symbol.raycastTarget = false;
            symbolObject.SetActive(false);
            buttons[i] = button;
            symbolImages[i] = symbol;
        }
        root.SetActive(false);
        return new SectorParts
        {
            root = root, artwork = artwork, canvasGroup = canvasGroup,
            nodeButtons = buttons, symbolImages = symbolImages,
            reservedMarker = reservedMarker
        };
    }

    private static CardParts BuildNodeCard(Transform parent, TMP_FontAsset font,
        Sprite resourceFrame, Sprite moduleCard, Sprite selectorFrame,
        Sprite[] effectIcons)
    {
        GameObject root = CreatePanel("MonolithNodeCard", parent,
            new Vector2(.025f, .012f), new Vector2(.975f, .215f), resourceFrame,
            new Color(.82f, .85f, .86f, .98f));

        GameObject iconPlate = CreatePanel("SelectedNodeIcon", root.transform,
            new Vector2(.055f, .16f), new Vector2(.185f, .88f), moduleCard,
            new Color(.62f, .67f, .69f, .98f));
        GameObject pictogramObject = CreateRect("Pictogram", iconPlate.transform,
            new Vector2(.12f, .20f), new Vector2(.88f, .86f));
        Image pictogram = pictogramObject.AddComponent<Image>();
        pictogram.sprite = effectIcons != null && effectIcons.Length > 9
            ? effectIcons[9] : null;
        pictogram.preserveAspect = true;
        pictogram.color = Color.white;
        pictogram.raycastTarget = false;
        TextMeshProUGUI icon = CreateText("EffectCode", iconPlate.transform, "--",
            new Vector2(.08f, .02f), new Vector2(.92f, .24f), font, 18f,
            Cyan, TextAlignmentOptions.Center);
        icon.fontStyle = FontStyles.Bold;

        TextMeshProUGUI name = CreateText("NodeName", root.transform,
            "SELECCIONA UN NODO", new Vector2(.205f, .70f),
            new Vector2(.73f, .83f), font, 27f, TextPrimary,
            TextAlignmentOptions.Left);
        name.fontStyle = FontStyles.Bold;
        TextMeshProUGUI state = CreateText("NodeState", root.transform,
            "ESTADO  •  DESCONOCIDO", new Vector2(.205f, .60f),
            new Vector2(.73f, .70f), font, 19f, Cyan,
            TextAlignmentOptions.Left);
        TextMeshProUGUI description = CreateText("NodeDescription", root.transform,
            "Explora las firmas de la cara actual.", new Vector2(.205f, .47f),
            new Vector2(.73f, .60f), font, 19f, TextSecondary,
            TextAlignmentOptions.TopLeft);
        description.textWrappingMode = TextWrappingModes.Normal;
        description.maxVisibleLines = 2;
        TextMeshProUGUI effect = CreateText("NodeEffect", root.transform,
            "EFECTO  —", new Vector2(.205f, .38f), new Vector2(.73f, .47f),
            font, 19f, TextPrimary, TextAlignmentOptions.Left);
        effect.textWrappingMode = TextWrappingModes.Normal;
        effect.maxVisibleLines = 2;
        TextMeshProUGUI requirements = CreateText("NodeRequirements", root.transform,
            "REQUISITOS  —", new Vector2(.205f, .30f), new Vector2(.73f, .38f),
            font, 19f, Hex("CDA15B"), TextAlignmentOptions.Left);
        requirements.textWrappingMode = TextWrappingModes.Normal;
        requirements.maxVisibleLines = 2;
        TextMeshProUGUI cost = CreateText("NodeCost", root.transform,
            "COSTE  —", new Vector2(.205f, .22f), new Vector2(.70f, .30f),
            font, 19f, Hex("9BB7C4"), TextAlignmentOptions.Left);

        Button repair = CreateButton("RepairNode", root.transform, "REPARAR",
            new Vector2(.745f, .45f), new Vector2(.895f, .74f), font, selectorFrame,
            Cyan, 21f);
        Button analyze = CreateButton("AnalyzeNode", root.transform, "ANALIZAR",
            new Vector2(.745f, .45f), new Vector2(.895f, .74f), font, selectorFrame,
            new Color(.72f, .49f, .17f, .98f), 21f);
        TextMeshProUGUI sectorProgress = CreateText("SectorProgress", root.transform,
            "PROGRESO DEL SECTOR  —", new Vector2(.705f, .22f),
            new Vector2(.915f, .32f), font, 18f, TextSecondary,
            TextAlignmentOptions.Center);
        return new CardParts
        {
            root = root, pictogram = pictogram, icon = icon, name = name,
            state = state, description = description, effect = effect,
            requirements = requirements, cost = cost,
            sectorProgress = sectorProgress, repair = repair, analyze = analyze
        };
    }

    private static BackgroundParts BuildLabBackground(Transform parent,
        Texture2D overviewTexture, Texture2D closeTexture)
    {
        GameObject overviewObject = CreateRect("HumanLabOverviewBackground", parent,
            Vector2.zero, Vector2.one);
        CanvasGroup overviewGroup = ConfigureLabBackground(overviewObject, overviewTexture);

        GameObject closeObject = CreateRect("HumanLabCloseBackground", parent,
            Vector2.zero, Vector2.one);
        CanvasGroup closeGroup = ConfigureLabBackground(closeObject, closeTexture);
        closeObject.SetActive(false);

        return new BackgroundParts
        {
            overviewRoot = overviewObject,
            closeRoot = closeObject,
            overviewRect = overviewObject.GetComponent<RectTransform>(),
            overviewGroup = overviewGroup,
            closeGroup = closeGroup
        };
    }

    private static SupportForegroundParts BuildOverviewSupport(
        Transform parent, string objectName, Material material)
    {
        GameObject root = CreateRect(objectName, parent,
            Vector2.zero, Vector2.one);
        AspectRatioFitter fitter = root.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = OverviewPedestalAspectRatio;
        root.transform.localScale = new Vector3(
            MachineMonolith2DVisualUI.OverviewDisplayScale,
            MachineMonolith2DVisualUI.OverviewDisplayScale, 1f);
        root.GetComponent<RectTransform>().anchoredPosition = OverviewPedestalPosition;
        CanvasGroup group = root.AddComponent<CanvasGroup>();
        group.alpha = 1f;
        group.interactable = false;
        group.blocksRaycasts = false;
        RawImage image = root.AddComponent<RawImage>();
        image.texture = Texture2D.whiteTexture;
        image.material = material;
        image.color = Color.white;
        image.raycastTarget = false;
        return new SupportForegroundParts
        {
            root = root,
            rect = root.GetComponent<RectTransform>(),
            group = group
        };
    }

    private static CanvasGroup ConfigureLabBackground(GameObject target, Texture2D texture)
    {
        AspectRatioFitter fitter = target.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = 1f;
        CanvasGroup group = target.AddComponent<CanvasGroup>();
        group.alpha = 1f;
        group.interactable = false;
        group.blocksRaycasts = false;
        RawImage background = target.AddComponent<RawImage>();
        background.texture = texture;
        background.color = new Color(.78f, .80f, .81f, 1f);
        background.raycastTarget = false;
        return group;
    }

    private static CanvasGroup BuildTransitionVeil(Transform parent)
    {
        GameObject veilObject = CreateRect("AlienTransitionVeil", parent,
            Vector2.zero, Vector2.one);
        CanvasGroup group = veilObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
        Image veil = veilObject.AddComponent<Image>();
        veil.color = new Color(.002f, .012f, .018f, .98f);
        veil.raycastTarget = false;

        return group;
    }

    private static void AddTitleLight(Transform parent, string name,
        float anchorMinX, float anchorMaxX)
    {
        GameObject lightObject = CreateRect(name, parent,
            new Vector2(anchorMinX, .54f), new Vector2(anchorMaxX, .57f));
        Image light = lightObject.AddComponent<Image>();
        light.color = new Color(.78f, .91f, 1f, .72f);
        light.raycastTarget = false;
    }

    private static GameObject CreatePanel(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Sprite sprite, Color color)
    {
        GameObject panel = CreateRect(name, parent, anchorMin, anchorMax);
        Image image = panel.AddComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null && sprite.border.sqrMagnitude > 0f
            ? Image.Type.Sliced
            : Image.Type.Simple;
        image.color = color;
        return panel;
    }

    private static Button CreateButton(string name, Transform parent, string label,
        Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font, Sprite sprite,
        Color color, float fontSize)
    {
        GameObject root = CreateRect(name, parent, anchorMin, anchorMax);
        Image image = root.AddComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null && sprite.border.sqrMagnitude > 0f
            ? Image.Type.Sliced
            : Image.Type.Simple;
        image.color = color;
        Button button = root.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
        colors.pressedColor = new Color(.75f, .88f, .92f, 1f);
        colors.disabledColor = new Color(.35f, .38f, .40f, .75f);
        button.colors = colors;
        TextMeshProUGUI text = CreateText("Label", root.transform, label,
            new Vector2(.06f, .08f), new Vector2(.94f, .92f), font, fontSize,
            TextPrimary, TextAlignmentOptions.Center);
        text.fontStyle = FontStyles.Bold;
        text.raycastTarget = false;
        return button;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent,
        string value, Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font,
        float fontSize, Color color, TextAlignmentOptions alignment)
    {
        GameObject root = CreateRect(name, parent, anchorMin, anchorMax);
        TextMeshProUGUI text = root.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = font;
        text.fontSize = fontSize;
        text.fontSizeMin = Mathf.Max(14f, fontSize * .72f);
        text.fontSizeMax = fontSize;
        text.enableAutoSizing = true;
        text.color = color;
        text.alignment = alignment;
        text.raycastTarget = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        return text;
    }

    private static void AddIcon(string name, Transform parent, Sprite sprite)
    {
        if (sprite == null)
            return;
        GameObject iconObject = CreateRect(name, parent,
            new Vector2(.07f, .12f), new Vector2(.25f, .88f));
        Image icon = iconObject.AddComponent<Image>();
        icon.sprite = sprite;
        icon.preserveAspect = true;
        icon.color = Color.white;
        icon.raycastTarget = false;
    }

    private static GameObject CreateRect(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject root = new GameObject(name, typeof(RectTransform));
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        return root;
    }

    private static GameObject CreatePointRect(string name, Transform parent,
        Vector2 anchor, float size)
    {
        GameObject root = CreateRect(name, parent, anchor, anchor);
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size, size);
        rect.anchoredPosition = Vector2.zero;
        return root;
    }

    private static void DestroyChild(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        if (child != null)
            UnityEngine.Object.DestroyImmediate(child.gameObject);
    }

    private static void DestroyComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        if (component != null)
            UnityEngine.Object.DestroyImmediate(component);
    }

    private static void DisableLegacyRoot(Transform parent, string name)
    {
        Transform child = parent.Find(name);
        if (child != null)
            child.gameObject.SetActive(false);
    }

    private static void SetObject(SerializedObject serializedObject, string property,
        UnityEngine.Object value)
    {
        SerializedProperty field = serializedObject.FindProperty(property);
        Require(field != null, "No existe el campo serializado: " + property);
        field.objectReferenceValue = value;
    }

    private static void SetObjectArray<T>(SerializedObject serializedObject,
        string property, T[] values) where T : UnityEngine.Object
    {
        SerializedProperty field = serializedObject.FindProperty(property);
        Require(field != null, "No existe el arreglo serializado: " + property);
        field.arraySize = values != null ? values.Length : 0;
        for (int i = 0; values != null && i < values.Length; i++)
            field.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
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
            "El conjunto de pictogramas de la Máquina está incompleto.");
        for (int i = 0; i < properties.Length; i++)
            SetObject(serializedObject, properties[i], icons[i]);
    }

    private static Material GetOrCreateMaterial(string materialPath, string shaderName)
    {
        string shaderPath = shaderName.Contains("SymbolKey")
            ? SymbolShaderPath
            : shaderName.Contains("LightKey")
                ? LightKeyShaderPath
            : shaderName.Contains("OverviewCutout")
                ? OverviewCutoutShaderPath
            : shaderName.Contains("FittedSupport")
                ? FittedSupportShaderPath
                : shaderName.Contains("AdditiveLight")
                    ? AdditiveLightShaderPath
                    : BlackKeyShaderPath;
        AssetDatabase.ImportAsset(shaderPath, ImportAssetOptions.ForceSynchronousImport);
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
        Require(shader != null, "No se pudo cargar el shader " + shaderName + ".");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(shader) { name = System.IO.Path.GetFileNameWithoutExtension(materialPath) };
            AssetDatabase.CreateAsset(material, materialPath);
        }
        else if (material.shader != shader)
        {
            material.shader = shader;
            EditorUtility.SetDirty(material);
        }
        return material;
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        if (!ColorUtility.TryParseHtmlString("#" + value, out Color color))
            color = Color.white;
        color.a = alpha / 255f;
        return color;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private struct HeaderParts
    {
        public TextMeshProUGUI le;
        public TextMeshProUGUI traces;
        public TextMeshProUGUI progress;
        public TextMeshProUGUI convergence;
        public Image progressFill;
    }

    private struct TabParts
    {
        public Button nodes;
        public Button mixes;
        public Button prestige;
    }

    private struct ViewHeaderParts
    {
        public Button back;
        public TextMeshProUGUI title;
        public TextMeshProUGUI index;
    }

    private struct OverviewParts
    {
        public GameObject root;
        public RawImage artwork;
        public CanvasGroup canvasGroup;
        public Button[] sectorButtons;
        public GameObject[] entryLights;
    }

    private struct BackgroundParts
    {
        public GameObject overviewRoot;
        public GameObject closeRoot;
        public RectTransform overviewRect;
        public CanvasGroup overviewGroup;
        public CanvasGroup closeGroup;
    }

    private struct SectorParts
    {
        public GameObject root;
        public RawImage artwork;
        public CanvasGroup canvasGroup;
        public Button[] nodeButtons;
        public RawImage[] symbolImages;
        public TextMeshProUGUI reservedMarker;
    }

    private struct SupportForegroundParts
    {
        public GameObject root;
        public RectTransform rect;
        public CanvasGroup group;
    }

    private struct CardParts
    {
        public GameObject root;
        public Image pictogram;
        public TextMeshProUGUI icon;
        public TextMeshProUGUI name;
        public TextMeshProUGUI state;
        public TextMeshProUGUI description;
        public TextMeshProUGUI effect;
        public TextMeshProUGUI requirements;
        public TextMeshProUGUI cost;
        public TextMeshProUGUI sectorProgress;
        public Button repair;
        public Button analyze;
    }
}
#endif
