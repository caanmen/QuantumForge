#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class Dimension3ProductionFloorVisualSetup
{
    private const string MainScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ThemePath =
        "Assets/Project/UI/Vertical/Generated/VerticalUiTheme.asset";
    private const string BackplatePath =
        "Assets/Project/Art/Dimension3/ProductionFloor/D3_ProductionFloor_CleanBasePlate_v3.png";
    private const string ReferencePath =
        "Assets/Project/Art/Dimension3/ProductionFloor/D3_ProductionFloor_Reference_CodeReal_v1.jpg";
    private const string ControlPlatesPath =
        "Assets/Project/Art/Dimension3/ProductionFloor/D3_ControlPlates_Atlas_v1.png";
    private const string AssemblyBlueprintPath =
        "Assets/Project/Art/Dimension3/ProductionFloor/D3_AssemblyPreview_Blueprint_v1.png";
    private const string IconKeyMaterialPath =
        "Assets/Project/Art/Dimension3/ProductionFloor/D3_ReferenceIconKey.mat";
    private const float ContentSourceHeight = 1920f;
    // The approved portrait reference is slightly wider than 9:16. Preserve its
    // aspect ratio instead of stretching the artwork to the full 1920 px height.
    private const float ContentDisplayHeight = 1890f;
    private const float ContentScaleY = ContentDisplayHeight / ContentSourceHeight;
    private static readonly Vector2 FactoryViewportCompensation = new Vector2(-15f, 17f);

    private static readonly Color Brass = Hex("B47A28");
    private static readonly Color Amber = Hex("D89A2B");
    private static readonly Color Cyan = Hex("3DB8B1");
    private static readonly Color TextMain = Hex("D8C8AA");
    private static readonly Color TextMuted = Hex("9B8B70");
    private static readonly Color ButtonTint = Color.white;
    private static readonly Color FieldTint = Color.white;

    private static readonly Rect AmberPlateUv =
        new Rect(0.073f, 0.569f, 0.857f, 0.282f);
    private static readonly Rect DarkPlateUv =
        new Rect(0.073f, 0.160f, 0.857f, 0.271f);
    private static readonly Rect[] PartCardUvRects =
    {
        PixelUv(110f, 738f, 176f, 169f, 1080f, 1920f),
        PixelUv(287f, 738f, 177f, 169f, 1080f, 1920f),
        PixelUv(464f, 738f, 178f, 169f, 1080f, 1920f),
        PixelUv(642f, 738f, 177f, 169f, 1080f, 1920f),
        PixelUv(819f, 738f, 179f, 169f, 1080f, 1920f)
    };
    private static readonly Rect[] ResourceIconUvRects =
    {
        PixelUv(48f, 125f, 72f, 72f, 1080f, 1920f),
        PixelUv(397f, 124f, 70f, 72f, 1080f, 1920f),
        PixelUv(741f, 122f, 72f, 74f, 1080f, 1920f)
    };
    private static readonly Rect[] UtilityIconUvRects =
    {
        PixelUv(24f, 1690f, 86f, 88f, 1080f, 1920f),
        PixelUv(380f, 1690f, 88f, 88f, 1080f, 1920f),
        PixelUv(729f, 1690f, 90f, 88f, 1080f, 1920f)
    };
    private static readonly Rect[] NavigationIconUvRects =
    {
        PixelUv(72f, 1806f, 78f, 82f, 1080f, 1920f),
        PixelUv(418f, 1806f, 80f, 82f, 1080f, 1920f),
        PixelUv(758f, 1806f, 82f, 82f, 1080f, 1920f)
    };

    private static Texture2D controlPlates;
    private static Material iconKeyMaterial;

    [MenuItem("Tools/Quantum Forge/Dimension 3/Apply Production Floor Visual")]
    public static void ConfigureOnly()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

        Dimension3PanelUI panel = Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        if (panel == null || panel.factoryRoot == null)
        {
            Debug.LogError("[D3 Production Floor] Falta Dimension3PanelUI o D3_Factory.");
            return;
        }

        ApplyTo(panel);
        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
        {
            Debug.LogError("[D3 Production Floor] No se pudo guardar Main.unity.");
            return;
        }

        ValidateCurrent();
        Debug.Log("[D3 Production Floor] APPLY PASS");
    }

    public static void ConfigureOnlyBatch()
    {
        ConfigureOnly();
    }

    public static void ApplyTo(Dimension3PanelUI panel)
    {
        if (panel == null || panel.factoryRoot == null)
            return;

        EnsureTextureImports();
        RectTransform factoryRect = panel.factoryRoot.GetComponent<RectTransform>();
        if (factoryRect != null)
        {
            factoryRect.anchoredPosition = FactoryViewportCompensation;
            factoryRect.sizeDelta = Vector2.zero;
        }
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        Sprite backplate = AssetDatabase.LoadAssetAtPath<Sprite>(BackplatePath);
        Texture2D reference = AssetDatabase.LoadAssetAtPath<Texture2D>(ReferencePath);
        controlPlates = AssetDatabase.LoadAssetAtPath<Texture2D>(ControlPlatesPath);
        iconKeyMaterial = GetOrCreateIconKeyMaterial();
        Texture2D assemblyBlueprint =
            AssetDatabase.LoadAssetAtPath<Texture2D>(AssemblyBlueprintPath);
        if (theme == null || backplate == null || reference == null ||
            controlPlates == null || assemblyBlueprint == null || iconKeyMaterial == null)
        {
            Debug.LogError("[D3 Production Floor] No se pudieron cargar los recursos visuales.");
            return;
        }

        Transform oldVisual = panel.factoryRoot.transform.Find("D3_ProductionFloorVisual");
        if (oldVisual != null)
            Object.DestroyImmediate(oldVisual.gameObject);

        GameObject visualRoot = Create("D3_ProductionFloorVisual", panel.factoryRoot.transform);
        SetTopLeftRaw(visualRoot.GetComponent<RectTransform>(),
            0f, 0f, 1080f, ContentDisplayHeight);
        RawImage plate = visualRoot.AddComponent<RawImage>();
        plate.texture = backplate.texture;
        plate.uvRect = new Rect(0f, 0f, 1f, 1f);
        plate.color = Color.white;
        plate.raycastTarget = false;
        visualRoot.transform.SetAsFirstSibling();

        D3ProductionFloorSkinUI skin = visualRoot.AddComponent<D3ProductionFloorSkinUI>();
        skin.leText = CreateResourceGroup("LE", visualRoot.transform, reference, 0,
            "0 LE", 36f, theme, Brass, TextMain, 30, 137, 330, 90, 70, 70, 10f);
        skin.tracesText = CreateResourceGroup("Traces", visualRoot.transform, reference, 1,
            "0 TRAZAS", 36f, theme, Cyan, Cyan, 367, 137, 320, 90, 70, 70, 10f);
        skin.automatonsText = CreateResourceGroup("Automatons", visualRoot.transform, reference, 2,
            "0 AUTÓMATAS", 36f, theme, Amber, Amber, 692, 137, 338, 90, 70, 70, 10f);
        CreateText("RobotMk", visualRoot.transform, "MK1", 19f,
            TextAlignmentOptions.Center, theme, TextMuted, 807, 389, 120, 38);
        skin.assemblyCostText = CreateText("AssemblyCost", visualRoot.transform,
            "COSTE", 22f, TextAlignmentOptions.Center, theme, Cyan,
            50, 1298, 478, 52);
        skin.assemblyCostText.richText = true;
        skin.previewTitleText = CreateText("PreviewTitle", visualRoot.transform,
            "PREVISIÓN MK ×1", 31f, TextAlignmentOptions.Center, theme, TextMain,
            41, 1372, 548, 64);
        skin.productionInventoryText = CreateText("ProductionInventory", visualRoot.transform,
            "0", 28f, TextAlignmentOptions.Center, theme, TextMain,
            842, 958, 150, 70);
        CreateAssemblyBlueprint(visualRoot.transform, reference);
        CreateText("AssemblySection", visualRoot.transform, "ENSAMBLE", 34f,
            TextAlignmentOptions.Center, theme, TextMain, 70, 1053, 440, 56);
        CreateText("PowerSection", visualRoot.transform, "POTENCIA DE PROCESO", 32f,
            TextAlignmentOptions.Center, theme, TextMain, 600, 1053, 415, 56);
        panel.productionFloorSkin = skin;

        TMP_Text title = FindText(panel.factoryRoot.transform, "Title");
        StyleText(title, theme, TextMain, 58f, TextAlignmentOptions.Center,
            165, 41, 750, 82);
        if (title != null)
        {
            title.text = "PLANTA DE PRODUCCIÓN";
            title.characterSpacing = -12f;
        }

        StyleText(panel.factoryStatusText, theme, TextMain, 42f,
            TextAlignmentOptions.Center, 228, 225, 624, 88);
        if (panel.factoryStatusText != null)
            panel.factoryStatusText.characterSpacing = -7f;
        CreateText("ProductionVersionLabel", visualRoot.transform, "CHASIS", 26f,
            TextAlignmentOptions.Center, theme, TextMuted, 69, 925, 320, 48);
        CreateText("ProductionQuantityLabel", visualRoot.transform, "CANTIDAD", 26f,
            TextAlignmentOptions.Center, theme, TextMuted, 376, 925, 235, 48);
        CreateText("ProductionCostLabel", visualRoot.transform, "COSTE", 26f,
            TextAlignmentOptions.Center, theme, TextMuted, 613, 925, 200, 48);
        CreateText("ProductionInventoryLabel", visualRoot.transform, "INVENTARIO", 26f,
            TextAlignmentOptions.Center, theme, TextMuted, 821, 925, 185, 48);
        StyleText(panel.productionTitleText, theme, Cyan, 24f,
            TextAlignmentOptions.Center, 613, 958, 200, 70);

        StyleDropdown(panel.productionVersionDropdown, theme, 31, 959, 320, 68);
        if (panel.productionVersionDropdown != null &&
            panel.productionVersionDropdown.captionText != null)
            panel.productionVersionDropdown.captionText.alignment =
                TextAlignmentOptions.Center;
        StyleDropdown(panel.productionQuantityDropdown, theme, 388, 959, 210, 68);
        skin.productionQuantityStepper =
            StyleQuantityStepper(panel.productionQuantityDropdown, theme);

        StylePartButton(panel.produceChassisButton, theme, reference, 0,
            135, 741, 150, 162);
        StylePartButton(panel.produceMotorButton, theme, reference, 1,
            295, 741, 150, 162);
        StylePartButton(panel.produceToolButton, theme, reference, 2,
            458, 741, 152, 162);
        StylePartButton(panel.produceControlButton, theme, reference, 3,
            618, 741, 150, 162);
        StylePartButton(panel.produceRegulatorButton, theme, reference, 4,
            780, 741, 158, 162);

        StyleText(panel.inventoryText, theme, Cyan, 20f,
            TextAlignmentOptions.TopLeft, 330, 1444, 270, 170);

        StyleDropdown(panel.assemblyMkDropdown, theme, 94, 1107, 390, 58, true);
        CreateText("AssemblyQuantityLabel", visualRoot.transform, "CANTIDAD", 22f,
            TextAlignmentOptions.Center, theme, TextMuted, 91, 1163, 165, 48);
        StyleDropdown(panel.assemblyQuantityDropdown, theme, 264, 1165, 214, 58, true);
        skin.assemblyQuantityStepper =
            StyleQuantityStepper(panel.assemblyQuantityDropdown, theme);
        StyleButton(panel.assembleMk1Button, theme, 75, 1231, 430, 64, Amber, 48f);
        ConstrainButtonLabel(panel.assembleMk1Button, 20f, 20f, 8f, 8f, 34f, 48f);
        StyleText(panel.costPreviewText, theme, Cyan, 22f,
            TextAlignmentOptions.TopLeft, 360, 1444, 240, 190);
        if (panel.costPreviewText != null)
            panel.costPreviewText.richText = true;

        StyleText(panel.assignmentText, theme, TextMain, 17f,
            TextAlignmentOptions.TopLeft, 592, 1074, 423, 84);
        StyleDropdown(panel.assignmentMkDropdown, theme, 592, 1167, 115, 58);
        StyleDropdown(panel.assignmentTraitDropdown, theme, 716, 1167, 139, 58);
        StyleDropdown(panel.assignmentChannelDropdown, theme, 864, 1167, 151, 58);
        skin.assignmentDisclosure = CreateAssignmentDisclosure(
            visualRoot.transform, panel, theme);
        StyleButton(panel.addAssignmentButton, theme, 665, 1172, 285, 50, Amber, 34f);
        ConstrainButtonLabel(panel.addAssignmentButton, 14f, 14f, 8f, 8f, 26f, 34f);
        StyleButton(panel.removeAssignmentButton, theme, 650, 1257, 315, 58, TextMuted, 38f, false);
        OffsetButtonLabelOptically(panel.removeAssignmentButton, 4f, -6f);
        StyleText(panel.powerText, theme, Cyan, 14f,
            TextAlignmentOptions.TopLeft, 592, 1312, 423, 58);
        StyleButton(panel.upgradeProcessBankButton, theme, 650, 1390, 365, 50,
            Brass, 15f);

        StyleText(panel.queueText, theme, TextMain, 13f,
            TextAlignmentOptions.TopLeft, 650, 1500, 365, 44);
        StyleButton(panel.openQueuesButton, theme, 656, 1556, 340, 70, Amber, 29f);
        ConstrainButtonLabel(panel.openQueuesButton, 28f, 28f, 12f, 12f, 22f, 29f);
        StyleButton(panel.cancelPartJobButton, theme, 650, 1612, 175, 54,
            TextMuted, 13f);
        StyleButton(panel.cancelAssemblyJobButton, theme, 840, 1612, 175, 54,
            TextMuted, 13f);

        StyleButton(panel.openCalibrationButton, theme, 45, 1656, 322, 94,
            Brass, 18f, false);
        StyleButton(panel.openResearchButton, theme, 379, 1656, 322, 94,
            Cyan, 17f, false);
        StyleButton(panel.openFacilitiesButton, theme, 713, 1656, 322, 94,
            Cyan, 16f, false);
        CreateButtonReferenceIcon(panel.openCalibrationButton, reference,
            UtilityIconUvRects[0], Brass, 5, 3, 70, 72);
        CreateButtonReferenceIcon(panel.openResearchButton, reference,
            UtilityIconUvRects[1], Cyan, 3, 3, 72, 72);
        CreateButtonReferenceIcon(panel.openFacilitiesButton, reference,
            UtilityIconUvRects[2], Cyan, 2, 3, 74, 72);
        CenterButtonContent(panel.openCalibrationButton, 8f, 20f);
        CenterButtonContent(panel.openResearchButton, 8f, 20f);
        CenterButtonContent(panel.openFacilitiesButton, 8f, 20f);
        Button plantNavigation = CreateNavigationButton(visualRoot.transform, "PlantNavigation", "PLANTA",
            theme, 45, 1765, 322, 108, Brass, null);
        Button workshopNavigation = CreateNavigationButton(visualRoot.transform, "WorkshopNavigation", "TALLER",
            theme, 379, 1765, 322, 108, TextMuted, panel.openCalibrationButton);
        Button controlNavigation = CreateNavigationButton(visualRoot.transform, "ControlNavigation", "CONTROL",
            theme, 713, 1765, 322, 108, TextMuted, panel.openFacilitiesButton);
        CreateButtonReferenceIcon(plantNavigation, reference,
            NavigationIconUvRects[0], Amber, 16, 8, 78, 82);
        CreateButtonReferenceIcon(workshopNavigation, reference,
            NavigationIconUvRects[1], TextMuted, 16, 8, 80, 82);
        CreateButtonReferenceIcon(controlNavigation, reference,
            NavigationIconUvRects[2], TextMuted, 16, 8, 82, 82);
        CenterButtonContent(plantNavigation, 12f, 14f, -10f);
        CenterButtonContent(workshopNavigation, 12f, 14f, 4f);
        CenterButtonContent(controlNavigation, 12f, 14f, 5f);
        StyleText(panel.noticeText, theme, TextMuted, 13f,
            TextAlignmentOptions.Center, 660, 955, 350, 42);
        if (panel.noticeText != null)
            panel.noticeText.gameObject.SetActive(false);
        if (panel.contextualHelpButton != null)
            panel.contextualHelpButton.gameObject.SetActive(false);
        if (panel.closeDimension3Button != null)
            panel.closeDimension3Button.gameObject.SetActive(false);

        StylePresentation(panel, theme);
        EditorUtility.SetDirty(panel);
        EditorUtility.SetDirty(skin);
    }

    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Production Floor Visual")]
    public static void ValidateCurrent()
    {
        Dimension3PanelUI panel = Object.FindFirstObjectByType<Dimension3PanelUI>(
            FindObjectsInactive.Include);
        bool valid = panel != null && panel.factoryRoot != null &&
            panel.productionFloorSkin != null && panel.productionFloorSkin.leText != null &&
            panel.productionFloorSkin.tracesText != null &&
            panel.productionFloorSkin.automatonsText != null &&
            panel.productionFloorSkin.productionInventoryText != null &&
            panel.productionFloorSkin.previewTitleText != null &&
            panel.productionFloorSkin.assemblyCostText != null &&
            panel.productionFloorSkin.assignmentDisclosure != null &&
            panel.productionFloorSkin.productionQuantityStepper != null &&
            panel.productionFloorSkin.assemblyQuantityStepper != null;

        Transform visual = panel == null || panel.factoryRoot == null
            ? null
            : panel.factoryRoot.transform.Find("D3_ProductionFloorVisual");
        RawImage plate = visual == null ? null : visual.GetComponent<RawImage>();
        valid &= plate != null && plate.texture != null && !plate.raycastTarget;
        Transform blueprint = visual == null ? null : visual.Find("AssemblyBlueprint");
        RawImage blueprintImage = blueprint == null
            ? null
            : blueprint.GetComponent<RawImage>();
        valid &= blueprintImage != null && blueprintImage.texture != null &&
            !blueprintImage.raycastTarget;
        valid &= visual != null && visual.Find("PlantNavigation") != null &&
            visual.Find("WorkshopNavigation") != null &&
            visual.Find("ControlNavigation") != null;
        valid &= panel != null && panel.produceChassisButton != null &&
            panel.produceMotorButton != null && panel.produceToolButton != null &&
            panel.produceControlButton != null && panel.produceRegulatorButton != null;
        if (panel != null)
        {
            valid &= HasMobileTarget(panel.produceChassisButton);
            valid &= HasMobileTarget(panel.produceMotorButton);
            valid &= HasMobileTarget(panel.produceToolButton);
            valid &= HasMobileTarget(panel.produceControlButton);
            valid &= HasMobileTarget(panel.produceRegulatorButton);
            valid &= HasMobileTarget(panel.assembleMk1Button);
            valid &= HasMobileTarget(panel.openCalibrationButton);
            valid &= HasMobileTarget(panel.openResearchButton);
            valid &= HasMobileTarget(panel.openFacilitiesButton);
            valid &= HasPartIcon(panel.produceChassisButton);
            valid &= HasPartIcon(panel.produceMotorButton);
            valid &= HasPartIcon(panel.produceToolButton);
            valid &= HasPartIcon(panel.produceControlButton);
            valid &= HasPartIcon(panel.produceRegulatorButton);
            valid &= HasQuantityStepper(panel.productionQuantityDropdown);
            valid &= HasQuantityStepper(panel.assemblyQuantityDropdown);
            valid &= HasSpanishFactoryLabels(panel.factoryRoot.transform);
        }

        if (!valid)
            Debug.LogError("[D3 Production Floor] VALIDATION FAIL");
        else
            Debug.Log("[D3 Production Floor] VALIDATION PASS");
    }

    public static void ValidateCurrentBatch()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        ValidateCurrent();
    }

    private static void StylePresentation(Dimension3PanelUI panel, VerticalUiTheme theme)
    {
        StyleButton(panel.objectiveCard == null ? null : panel.objectiveCard.primaryActionButton,
            theme, 405, 1320, 270, 72, Amber, 19f);
        StyleText(panel.coachmarkText, theme, TextMain, 17f,
            TextAlignmentOptions.Center, 84, 1110, 350, 82);
        StyleButton(panel.closeHelpButton, theme, 405, 1420, 270, 72, Amber, 20f);
        StyleButton(panel.continueFirstCycleButton, theme, 220, 1400, 280, 78, Amber, 20f);
        StyleButton(panel.replayFirstCycleButton, theme, 580, 1400, 280, 78, Brass, 20f);
        StyleButton(panel.closeFirstCycleButton, theme, 405, 1510, 270, 70, TextMuted, 18f);
    }

    private static void StylePartButton(Button button, VerticalUiTheme theme,
        Texture2D atlas, int iconIndex,
        float x, float y, float width, float height)
    {
        StyleButton(button, theme, x, y, width, height, Brass, 13f, false);
        if (button == null)
            return;

        RemoveChild(button.transform, "D3_PartIcon");
        RemoveChild(button.transform, "D3_PartCard");
        GameObject cardObject = Create("D3_PartCard", button.transform);
        StretchRect(cardObject.GetComponent<RectTransform>());
        RawImage card = cardObject.AddComponent<RawImage>();
        card.texture = atlas;
        card.uvRect = PartCardUvRects[Mathf.Clamp(
            iconIndex, 0, PartCardUvRects.Length - 1)];
        card.color = Color.white;
        card.raycastTarget = false;
        cardObject.transform.SetAsFirstSibling();
        button.targetGraphic = card;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Hex("FFE0A0");
        colors.pressedColor = Hex("C27E24");
        colors.selectedColor = Hex("F4C05C");
        colors.disabledColor = Hex("5B5549", 210);
        colors.colorMultiplier = 1f;
        button.colors = colors;

        TMP_Text label = button == null ? null : button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
            label.gameObject.SetActive(false);
    }

    private static void StyleButton(Button button, VerticalUiTheme theme,
        float x, float y, float width, float height, Color accent, float fontSize,
        bool usePlate = true)
    {
        if (button == null)
            return;

        SetTopLeftContent(button.GetComponent<RectTransform>(), x, y, width, height);
        Image hitArea = button.GetComponent<Image>();
        if (hitArea != null)
        {
            hitArea.sprite = null;
            hitArea.type = Image.Type.Simple;
            hitArea.color = Color.clear;
            hitArea.raycastTarget = true;
        }

        ColorBlock colors = button.colors;
        if (usePlate)
        {
            RawImage plate = CreateControlPlate(button.transform, accent == Amber);
            button.targetGraphic = plate;
            colors.normalColor = Color.white;
            colors.highlightedColor = Hex("FFE0A0");
            colors.pressedColor = Hex("B87522");
            colors.selectedColor = Hex("F0C06A");
            colors.disabledColor = Hex("665F52", 210);
        }
        else
        {
            RemoveChild(button.transform, "D3_ControlPlate");
            button.targetGraphic = hitArea;
            colors.normalColor = Color.clear;
            colors.highlightedColor = WithAlpha(accent, 0.20f);
            colors.pressedColor = WithAlpha(accent, 0.32f);
            colors.selectedColor = WithAlpha(accent, 0.16f);
            colors.disabledColor = Hex("080806", 150);
        }
        colors.colorMultiplier = 1f;
        button.colors = colors;

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            StretchRect(label.rectTransform);
            label.font = theme.primaryFont;
            label.fontSize = fontSize;
            label.enableAutoSizing = false;
            label.fontStyle = FontStyles.Bold;
            label.color = accent == Amber ? Hex("22170B") : accent;
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;
        }
    }

    private static void StyleDropdown(TMP_Dropdown dropdown, VerticalUiTheme theme,
        float x, float y, float width, float height, bool usePlate = false)
    {
        if (dropdown == null)
            return;

        dropdown.enabled = true;
        SetTopLeftContent(dropdown.GetComponent<RectTransform>(), x, y, width, height);
        Image hitArea = dropdown.GetComponent<Image>();
        if (hitArea != null)
        {
            hitArea.sprite = null;
            hitArea.type = Image.Type.Simple;
            hitArea.color = Color.clear;
            hitArea.raycastTarget = true;
        }
        RawImage plate = null;
        if (usePlate)
            plate = CreateControlPlate(dropdown.transform, false);
        else
            RemoveChild(dropdown.transform, "D3_ControlPlate");
        dropdown.targetGraphic = plate != null ? plate : hitArea;
        ColorBlock colors = dropdown.colors;
        colors.normalColor = usePlate ? Color.white : Color.clear;
        colors.highlightedColor = usePlate ? Hex("FFE0A0") : WithAlpha(Amber, 0.10f);
        colors.pressedColor = usePlate ? Hex("B87522") : WithAlpha(Amber, 0.18f);
        colors.selectedColor = usePlate ? Hex("F0C06A") : Color.clear;
        colors.disabledColor = usePlate ? Hex("665F52", 210) : Color.clear;
        colors.colorMultiplier = 1f;
        dropdown.colors = colors;

        if (dropdown.captionText != null)
        {
            dropdown.captionText.gameObject.SetActive(true);
            dropdown.captionText.font = theme.primaryFont;
            dropdown.captionText.fontSize = 27f;
            dropdown.captionText.fontStyle = FontStyles.Bold;
            dropdown.captionText.color = TextMain;
            dropdown.captionText.alignment = TextAlignmentOptions.Left;
        }
        RemoveChild(dropdown.transform, "D3_DropdownArrow");
        GameObject arrowObject = Create("D3_DropdownArrow", dropdown.transform);
        SetTopLeftRaw(arrowObject.GetComponent<RectTransform>(),
            width - 46f, 0f, 38f, height);
        TextMeshProUGUI arrow = arrowObject.AddComponent<TextMeshProUGUI>();
        arrow.text = "▼";
        arrow.font = theme.primaryFont;
        arrow.fontSize = 16f;
        arrow.color = TextMuted;
        arrow.alignment = TextAlignmentOptions.Center;
        arrow.raycastTarget = false;
        if (dropdown.itemText != null)
        {
            dropdown.itemText.font = theme.primaryFont;
            dropdown.itemText.fontSize = 17f;
            dropdown.itemText.color = TextMain;
        }
    }

    private static D3DropdownStepperUI StyleQuantityStepper(
        TMP_Dropdown dropdown, VerticalUiTheme theme)
    {
        if (dropdown == null)
            return null;

        RemoveChild(dropdown.transform, "D3_QuantityStepper");
        RemoveChild(dropdown.transform, "D3_DropdownArrow");
        if (dropdown.captionText != null)
            dropdown.captionText.gameObject.SetActive(false);
        dropdown.enabled = false;

        GameObject overlay = Create("D3_QuantityStepper", dropdown.transform);
        overlay.SetActive(false);
        StretchRect(overlay.GetComponent<RectTransform>());
        float width = dropdown.GetComponent<RectTransform>().sizeDelta.x;
        float height = dropdown.GetComponent<RectTransform>().sizeDelta.y;
        float side = Mathf.Min(58f, width * 0.29f);
        Button minus = CreateMiniButton(
            overlay.transform, "Minus", "−", theme, 0f, 0f, side, height);
        Button plus = CreateMiniButton(
            overlay.transform, "Plus", "+", theme, width - side, 0f, side, height);

        GameObject valueObject = Create("Value", overlay.transform);
        SetTopLeftRaw(valueObject.GetComponent<RectTransform>(),
            side, 0f, width - side * 2f, height);
        TextMeshProUGUI value = valueObject.AddComponent<TextMeshProUGUI>();
        value.font = theme.primaryFont;
        value.fontSize = 29f;
        value.fontStyle = FontStyles.Bold;
        value.color = TextMain;
        value.alignment = TextAlignmentOptions.Center;
        value.raycastTarget = false;

        D3DropdownStepperUI stepper = overlay.AddComponent<D3DropdownStepperUI>();
        stepper.dropdown = dropdown;
        stepper.minusButton = minus;
        stepper.plusButton = plus;
        stepper.valueText = value;
        overlay.SetActive(true);
        return stepper;
    }

    private static Button CreateMiniButton(
        Transform parent, string name, string value, VerticalUiTheme theme,
        float x, float y, float width, float height)
    {
        GameObject target = Create(name, parent);
        SetTopLeftRaw(target.GetComponent<RectTransform>(), x, y, width, height);
        Image hitArea = target.AddComponent<Image>();
        hitArea.color = Color.clear;
        hitArea.raycastTarget = true;
        Button button = target.AddComponent<Button>();
        button.targetGraphic = hitArea;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.clear;
        colors.highlightedColor = WithAlpha(Amber, 0.10f);
        colors.pressedColor = WithAlpha(Amber, 0.18f);
        colors.selectedColor = Color.clear;
        colors.disabledColor = Color.clear;
        colors.colorMultiplier = 1f;
        button.colors = colors;

        GameObject labelObject = Create("Label", target.transform);
        StretchRect(labelObject.GetComponent<RectTransform>());
        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = value;
        label.font = theme.primaryFont;
        label.fontSize = 34f;
        label.fontStyle = FontStyles.Bold;
        label.color = Amber;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        return button;
    }

    private static RawImage CreateControlPlate(Transform parent, bool primary)
    {
        RemoveChild(parent, "D3_ControlPlate");
        GameObject target = Create("D3_ControlPlate", parent);
        StretchRect(target.GetComponent<RectTransform>());
        RawImage plate = target.AddComponent<RawImage>();
        plate.texture = controlPlates;
        plate.uvRect = primary ? AmberPlateUv : DarkPlateUv;
        plate.color = Color.white;
        plate.raycastTarget = false;
        target.transform.SetAsFirstSibling();
        return plate;
    }

    private static D3AssignmentDisclosureUI CreateAssignmentDisclosure(
        Transform parent, Dimension3PanelUI panel, VerticalUiTheme theme)
    {
        GameObject container = Create("D3_AssignmentDisclosure", parent);
        container.SetActive(false);
        StretchRect(container.GetComponent<RectTransform>());

        GameObject buttonObject = Create("Selector", container.transform);
        buttonObject.AddComponent<Image>();
        Button button = buttonObject.AddComponent<Button>();
        GameObject labelObject = Create("Label", buttonObject.transform);
        StretchRect(labelObject.GetComponent<RectTransform>());
        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = "MK1 NORMAL";
        StyleButton(button, theme, 620, 1095, 390, 74, Cyan, 28f, false);

        D3AssignmentDisclosureUI disclosure =
            container.AddComponent<D3AssignmentDisclosureUI>();
        disclosure.toggleButton = button;
        disclosure.label = label;
        disclosure.statusText = panel.assignmentText;
        disclosure.mkDropdown = panel.assignmentMkDropdown;
        disclosure.traitDropdown = panel.assignmentTraitDropdown;
        disclosure.channelDropdown = panel.assignmentChannelDropdown;
        disclosure.addButton = panel.addAssignmentButton;
        disclosure.removeButton = panel.removeAssignmentButton;
        container.SetActive(true);
        return disclosure;
    }

    private static Button CreateNavigationButton(
        Transform parent, string name, string value, VerticalUiTheme theme,
        float x, float y, float width, float height, Color accent, Button target)
    {
        GameObject buttonObject = Create(name, parent);
        buttonObject.SetActive(false);
        buttonObject.AddComponent<Image>();
        Button button = buttonObject.AddComponent<Button>();
        GameObject labelObject = Create("Label", buttonObject.transform);
        StretchRect(labelObject.GetComponent<RectTransform>());
        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = value;
        StyleButton(button, theme, x, y, width, height, accent, 55f, false);
        if (target != null)
        {
            D3ButtonRelayUI relay = buttonObject.AddComponent<D3ButtonRelayUI>();
            relay.sourceButton = button;
            relay.targetButton = target;
        }
        buttonObject.SetActive(true);
        return button;
    }

    private static void StyleText(TMP_Text text, VerticalUiTheme theme, Color color,
        float fontSize, TextAlignmentOptions alignment,
        float x, float y, float width, float height)
    {
        if (text == null)
            return;
        SetTopLeftContent(text.rectTransform, x, y, width, height);
        text.font = theme.primaryFont;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent,
        string value, float size, TextAlignmentOptions alignment,
        VerticalUiTheme theme, Color color,
        float x, float y, float width, float height)
    {
        GameObject target = Create(name, parent);
        TextMeshProUGUI label = target.AddComponent<TextMeshProUGUI>();
        label.text = value;
        StyleText(label, theme, color, size, alignment, x, y, width, height);
        return label;
    }

    private static void CreateAssemblyBlueprint(Transform parent, Texture2D texture)
    {
        GameObject target = Create("AssemblyBlueprint", parent);
        SetTopLeftContent(target.GetComponent<RectTransform>(), 22f, 1416f, 290f, 250f);
        RawImage image = target.AddComponent<RawImage>();
        image.texture = texture;
        image.uvRect = PixelUv(22f, 1416f, 290f, 250f, 1080f, 1920f);
        image.color = Color.white;
        image.raycastTarget = false;
    }

    private static void CreateButtonReferenceIcon(
        Button button, Texture2D texture, Rect uv, Color tint,
        float x, float y, float width, float height)
    {
        if (button == null)
            return;
        RemoveChild(button.transform, "D3_ReferenceIcon");
        GameObject target = Create("D3_ReferenceIcon", button.transform);
        SetTopLeftRaw(target.GetComponent<RectTransform>(), x, y, width, height);
        RawImage image = target.AddComponent<RawImage>();
        image.texture = texture;
        image.uvRect = uv;
        image.material = iconKeyMaterial;
        image.color = Color.white;
        image.raycastTarget = false;
    }

    private static TextMeshProUGUI CreateResourceGroup(
        string name, Transform parent, Texture2D texture, int iconIndex,
        string value, float fontSize, VerticalUiTheme theme,
        Color iconTint, Color textColor,
        float x, float y, float width, float height,
        float iconWidth, float iconHeight, float gap)
    {
        GameObject container = Create(name + "ResourceGroup", parent);
        SetTopLeftContent(container.GetComponent<RectTransform>(),
            x, y, width, height);

        GameObject iconObject = Create("D3_ReferenceIcon", container.transform);
        SetTopLeftRaw(iconObject.GetComponent<RectTransform>(),
            0f, 0f, iconWidth, iconHeight);
        RawImage icon = iconObject.AddComponent<RawImage>();
        icon.texture = texture;
        icon.uvRect = ResourceIconUvRects[Mathf.Clamp(
            iconIndex, 0, ResourceIconUvRects.Length - 1)];
        icon.material = iconKeyMaterial;
        icon.color = Color.white;
        icon.raycastTarget = false;

        GameObject labelObject = Create("Label", container.transform);
        StretchRect(labelObject.GetComponent<RectTransform>());
        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = value;
        label.font = theme.primaryFont;
        label.fontSize = fontSize;
        label.fontStyle = FontStyles.Bold;
        label.color = textColor;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.raycastTarget = false;

        D3CenteredIconLabelUI layout =
            container.AddComponent<D3CenteredIconLabelUI>();
        layout.Configure(icon.rectTransform, label, gap);
        return label;
    }

    private static void ConstrainButtonLabel(
        Button button, float left, float right, float top, float bottom,
        float minFontSize, float maxFontSize)
    {
        if (button == null)
            return;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label == null)
            return;
        RectTransform rect = label.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
        rect.localScale = Vector3.one;
        label.enableAutoSizing = true;
        label.fontSizeMin = minFontSize;
        label.fontSizeMax = maxFontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.overflowMode = TextOverflowModes.Truncate;
        label.raycastTarget = false;
    }

    private static void OffsetButtonLabelOptically(
        Button button, float horizontalOffset, float verticalOffset)
    {
        if (button == null)
            return;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label == null)
            return;
        label.rectTransform.anchoredPosition =
            new Vector2(horizontalOffset, -verticalOffset);
        label.raycastTarget = false;
    }

    private static void CenterButtonContent(
        Button button, float gap, float verticalOffset,
        float horizontalOffset = 0f)
    {
        if (button == null)
            return;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        Transform iconTransform = button.transform.Find("D3_ReferenceIcon");
        RectTransform icon = iconTransform == null
            ? null
            : iconTransform.GetComponent<RectTransform>();
        if (label == null || icon == null)
            return;
        D3CenteredIconLabelUI layout =
            button.GetComponent<D3CenteredIconLabelUI>();
        if (layout == null)
            layout = button.gameObject.AddComponent<D3CenteredIconLabelUI>();
        layout.Configure(icon, label, gap, verticalOffset, horizontalOffset);
        EditorUtility.SetDirty(layout);
    }

    private static Material GetOrCreateIconKeyMaterial()
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(IconKeyMaterialPath);
        if (material != null)
            return material;
        Shader shader = Shader.Find("UI/D3ReferenceIconKey");
        if (shader == null)
            return null;
        material = new Material(shader)
        {
            name = "D3 Reference Icon Key"
        };
        AssetDatabase.CreateAsset(material, IconKeyMaterialPath);
        AssetDatabase.SaveAssets();
        return material;
    }

    private static void CreateReferenceIcon(
        Transform parent, Texture2D texture, int index,
        float x, float y, float width, float height)
    {
        GameObject target = Create("ResourceIcon" + index, parent);
        SetTopLeftContent(target.GetComponent<RectTransform>(), x, y, width, height);
        RawImage image = target.AddComponent<RawImage>();
        image.texture = texture;
        image.uvRect = ResourceIconUvRects[Mathf.Clamp(
            index, 0, ResourceIconUvRects.Length - 1)];
        image.color = Color.white;
        image.raycastTarget = false;
    }

    private static TMP_Text FindText(Transform root, string name)
    {
        Transform child = root == null ? null : root.Find(name);
        return child == null ? null : child.GetComponent<TMP_Text>();
    }

    private static GameObject Create(string name, Transform parent)
    {
        GameObject target = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        target.transform.SetParent(parent, false);
        return target;
    }

    private static void SetTopLeftContent(RectTransform rect,
        float x, float y, float width, float height)
    {
        SetTopLeftRaw(rect, x, y * ContentScaleY, width, height * ContentScaleY);
    }

    private static void SetTopLeftRaw(RectTransform rect,
        float x, float y, float width, float height)
    {
        if (rect == null)
            return;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        rect.localScale = Vector3.one;
    }

    private static void StretchRect(RectTransform rect)
    {
        if (rect == null)
            return;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static void RemoveChild(Transform parent, string name)
    {
        Transform child = parent == null ? null : parent.Find(name);
        if (child != null)
            Object.DestroyImmediate(child.gameObject);
    }

    private static Rect PixelUv(
        float x, float yFromTop, float width, float height,
        float textureWidth, float textureHeight)
    {
        return new Rect(
            x / textureWidth,
            (textureHeight - yFromTop - height) / textureHeight,
            width / textureWidth,
            height / textureHeight);
    }

    private static void EnsureTextureImports()
    {
        AssetDatabase.ImportAsset(BackplatePath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(BackplatePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = false;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        ConfigureUiTexture(ReferencePath);
        ConfigureUiTexture(ControlPlatesPath);

        AssetDatabase.ImportAsset(
            AssemblyBlueprintPath, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter blueprintImporter =
            AssetImporter.GetAtPath(AssemblyBlueprintPath) as TextureImporter;
        if (blueprintImporter != null)
        {
            blueprintImporter.textureType = TextureImporterType.Default;
            blueprintImporter.mipmapEnabled = false;
            blueprintImporter.alphaIsTransparency = false;
            blueprintImporter.wrapMode = TextureWrapMode.Clamp;
            blueprintImporter.maxTextureSize = 2048;
            blueprintImporter.textureCompression = TextureImporterCompression.Uncompressed;
            blueprintImporter.SaveAndReimport();
        }
    }

    private static void ConfigureUiTexture(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;
        importer.textureType = TextureImporterType.Default;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static bool HasMobileTarget(Button button)
    {
        if (button == null)
            return false;
        RectTransform rect = button.GetComponent<RectTransform>();
        return rect != null && rect.sizeDelta.x >= 120f && rect.sizeDelta.y >= 48f;
    }

    private static bool HasPartIcon(Button button)
    {
        if (button == null)
            return false;
        Transform icon = button.transform.Find("D3_PartCard");
        RawImage image = icon == null ? null : icon.GetComponent<RawImage>();
        return image != null && image.texture != null && !image.raycastTarget;
    }

    private static bool HasQuantityStepper(TMP_Dropdown dropdown)
    {
        if (dropdown == null)
            return false;
        Transform overlay = dropdown.transform.Find("D3_QuantityStepper");
        D3DropdownStepperUI stepper = overlay == null
            ? null
            : overlay.GetComponent<D3DropdownStepperUI>();
        return stepper != null && stepper.minusButton != null &&
            stepper.plusButton != null && stepper.valueText != null;
    }

    private static bool HasSpanishFactoryLabels(Transform root)
    {
        if (root == null)
            return false;
        string[] forbidden =
        {
            "FACTORY", "ASSEMBLE", "PRODUCTION", "RESEARCH", "FACILITIES",
            "QUEUES", "CLOSE", "HELP", "POWER", "DRIVE SYSTEM"
        };
        TMP_Text[] labels = root.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < labels.Length; i++)
        {
            string value = labels[i] == null
                ? ""
                : (labels[i].text ?? "").ToUpperInvariant();
            for (int term = 0; term < forbidden.Length; term++)
                if (value.Contains(forbidden[term]))
                    return false;
        }
        return true;
    }

    private static Color WithAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    private static Color Hex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
#endif
