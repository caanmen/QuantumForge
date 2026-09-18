#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PrestigeMachineScreenSetup
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

    private static readonly Color TextPrimary = Hex("E8EAEC");
    private static readonly Color TextSecondary = Hex("87929B");
    private static readonly Color Cyan = Hex("5ADFFF");
    private static readonly Color Purple = Hex("A94CFF");
    private static readonly Color Panel = Hex("071017", 248);
    private static readonly Color Muted = Hex("33434C", 235);
    private static readonly Color LockedText = Hex("8E9AA3");

    [MenuItem("Tools/Quantum Forge/Prestige/Configure Approved Machine Screen")]
    public static void Configure()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ConfigureInOpenScene(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Prestige Machine Screen] CONFIGURED | approved static composition | canonical machine art");
    }

    public static void ConfigureInOpenScene(Scene scene)
    {
        PrestigeUI prestige = Object.FindFirstObjectByType<PrestigeUI>(FindObjectsInactive.Include);
        MachinePanelUI machinePanel = Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(prestige != null, "No se encontró PrestigeUI en Main.unity.");
        Require(machinePanel != null, "No se encontró MachinePanelUI en Main.unity.");

        Transform source = machinePanel.transform.Find("MachineMonolith2DRoot");
        Require(source != null,
            "Falta MachineMonolith2DRoot; configura primero el Monolito 2D aprobado.");

        Transform existing = prestige.transform.Find("PrestigeMachineScreenRoot");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        for (int i = 0; i < prestige.transform.childCount; i++)
            prestige.transform.GetChild(i).gameObject.SetActive(false);

        GameObject root = Object.Instantiate(source.gameObject, prestige.transform);
        root.name = "PrestigeMachineScreenRoot";
        root.SetActive(true);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        rootRect.localScale = Vector3.one;

        TMP_FontAsset font = LoadFont();
        Sprite moduleCard = AssetDatabase.LoadAssetAtPath<Sprite>(ModuleCardPath);
        Sprite resourceFrame = AssetDatabase.LoadAssetAtPath<Sprite>(ResourceFramePath);
        Sprite selectorFrame = AssetDatabase.LoadAssetAtPath<Sprite>(SelectorFramePath);
        Require(font != null && moduleCard != null && resourceFrame != null &&
                selectorFrame != null,
            "Faltan recursos canónicos para la pantalla de Prestigio.");

        Button nodes = RequireComponent<Button>(root.transform,
            "MachineContextTabs/NodesTab");
        Button mixes = RequireComponent<Button>(root.transform,
            "MachineContextTabs/MixesTab");
        Button prestigeTab = RequireComponent<Button>(root.transform,
            "MachineContextTabs/PrestigeTab");
        RectTransform tabsRect = RequireTransform(root.transform, "MachineContextTabs")
            .GetComponent<RectTransform>();
        SetAnchors(tabsRect, new Vector2(.215f, .834f), new Vector2(.785f, .871f));
        SetAnchors(nodes.GetComponent<RectTransform>(), new Vector2(0f, 0f),
            new Vector2(.314f, 1f));
        SetAnchors(mixes.GetComponent<RectTransform>(), new Vector2(.334f, 0f),
            new Vector2(.648f, 1f));
        SetAnchors(prestigeTab.GetComponent<RectTransform>(), new Vector2(.668f, 0f),
            Vector2.one);
        StyleTab(nodes, Hex("1F4B57", 245), Hex("60808B"), true);
        StyleTab(mixes, Hex("2A163D", 245), Hex("673D84"), true);
        StyleTab(prestigeTab, Hex("18112C", 250), Hex("B85CFF"), false);
        GameObject activeLine = CreatePanel("ActiveUnderline", prestigeTab.transform,
            new Vector2(.10f, -.18f), new Vector2(.90f, -.10f), null, Purple);
        activeLine.GetComponent<Image>().raycastTarget = false;

        Transform primary = RequireTransform(root.transform, "MachinePrimaryContent");
        Transform viewHeader = RequireTransform(primary, "MonolithViewHeader");
        SetAnchors(viewHeader.GetComponent<RectTransform>(), new Vector2(.06f, .748f),
            new Vector2(.94f, .813f));
        Image viewHeaderBackground = viewHeader.GetComponent<Image>();
        if (viewHeaderBackground == null)
            viewHeaderBackground = viewHeader.gameObject.AddComponent<Image>();
        viewHeaderBackground.color = new Color(.008f, .018f, .052f, .86f);
        viewHeaderBackground.raycastTarget = false;
        GameObject back = RequireTransform(viewHeader, "BackToOverview").gameObject;
        back.SetActive(false);
        TextMeshProUGUI title = RequireComponent<TextMeshProUGUI>(viewHeader, "ViewTitle");
        TextMeshProUGUI subtitle = RequireComponent<TextMeshProUGUI>(viewHeader, "ViewIndex");
        title.text = "PROTOCOLO DE PRESTIGIO 1";
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize = 36f;
        title.fontStyle = FontStyles.Normal;
        title.characterSpacing = 0f;
        SetAnchors(title.rectTransform, new Vector2(.02f, .42f), new Vector2(.98f, .96f));
        subtitle.text = "LA MÁQUINA PREPARA UNA RUTA DIMENSIONAL";
        subtitle.alignment = TextAlignmentOptions.Center;
        subtitle.fontSize = 19f;
        SetAnchors(subtitle.rectTransform, new Vector2(.02f, .02f), new Vector2(.98f, .43f));
        GameObject protocolUnderline = CreatePanel("ProtocolUnderline", viewHeader,
            new Vector2(.03f, .14f), new Vector2(.97f, .16f), null,
            Hex("00CDEE", 150));
        protocolUnderline.GetComponent<Image>().raycastTarget = false;
        TextMeshProUGUI badge = CreateStatusBadge(primary, font, selectorFrame);

        Transform viewport = RequireTransform(primary, "MonolithViewport");
        // La referencia aprobada conserva el encuadre canónico de Máquina y coloca
        // el panel de estado por encima. Recortar el viewport encogía el Monolito.
        SetAnchors(viewport.GetComponent<RectTransform>(), new Vector2(.035f, .225f),
            new Vector2(.965f, .795f));
        Disable(viewport, "HumanLabCloseBackground");
        Disable(viewport, "MonolithSectorCloseup");
        Disable(viewport, "TransitionVeil");
        Transform overview = RequireTransform(viewport, "MonolithOverview");
        overview.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -190f);
        DisableAllButtonsAndRaycasts(viewport);
        foreach (Transform child in overview.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Contains("EntryLight") || child.name.Contains("SectorTouch"))
                child.gameObject.SetActive(false);
        }

        Disable(primary, "MonolithNodeCard");
        Disable(root.transform, "FusionBackToNodes");
        RawImage overviewArtwork = RequireComponent<RawImage>(overview, "OverviewArtwork");
        overviewArtwork.rectTransform.localScale = new Vector3(.82f, .82f, 1f);

        StatusParts status = BuildStatusPanel(primary, font, selectorFrame);
        BuildPrestigeSignature(root.transform);

        // El protocolo, su estado y el panel informativo se leen por encima del arte.
        viewHeader.SetAsLastSibling();
        badge.transform.parent.SetAsLastSibling();
        RequireTransform(primary, "PrestigeStatusPanel").SetAsLastSibling();

        PrestigeMachineScreenUI controller = root.GetComponent<PrestigeMachineScreenUI>();
        if (controller == null)
            controller = root.AddComponent<PrestigeMachineScreenUI>();
        SerializedObject so = new SerializedObject(controller);
        SetObject(so, "prestigeUI", prestige);
        SetObject(so, "machinePanel", machinePanel);
        SetObject(so, "nodesButton", nodes);
        SetObject(so, "mixesButton", mixes);
        SetObject(so, "prestigeButton", prestigeTab);
        SetObject(so, "actionButton", status.action);
        SetObject(so, "leText", RequireComponent<TextMeshProUGUI>(root.transform,
            "MachineHeader/MachineResource_LE/LE"));
        SetObject(so, "tracesText", RequireComponent<TextMeshProUGUI>(root.transform,
            "MachineHeader/MachineResource_Traces/Traces"));
        SetObject(so, "totalProgressText", RequireComponent<TextMeshProUGUI>(root.transform,
            "MachineHeader/MachineTitlePlate/TotalProgress"));
        SetObject(so, "channelText", RequireComponent<TextMeshProUGUI>(root.transform,
            "MachineHeader/MachineTitlePlate/Convergence"));
        SetObject(so, "totalProgressFill", RequireComponent<Image>(root.transform,
            "MachineHeader/MachineTitlePlate/ProgressTrack/Fill"));
        SetObject(so, "statusBadgeText", badge);
        SetObject(so, "repairValueText", status.repairValue);
        SetObject(so, "repairProgressFill", status.repairFill);
        SetObject(so, "repairRequirementText", status.repairRequirementText);
        SetObject(so, "repairRequirementPanel", status.repairRequirementPanel);
        SetObject(so, "repairRequirementStateText", status.repairRequirementStateText);
        SetObject(so, "channelRequirementText", status.channelRequirementText);
        SetObject(so, "channelRequirementPanel", status.channelRequirementPanel);
        SetObject(so, "channelRequirementStateText", status.channelRequirementStateText);
        SetObject(so, "statusText", status.status);
        SetObject(so, "actionText", status.actionLabel);
        SetObject(so, "overviewArtwork", overviewArtwork);
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(root);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(prestige);
        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static StatusParts BuildStatusPanel(Transform parent, TMP_FontAsset font,
        Sprite selectorFrame)
    {
        Transform old = parent.Find("PrestigeStatusPanel");
        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        GameObject root = CreateChamferedPanel("PrestigeStatusPanel", parent,
            new Vector2(.04f, .017f), new Vector2(.96f, .351f),
            Hex("030914", 248), Hex("1F90B0", 230), 3f, 18f);

        CreateText("ProgressTitle", root.transform, "REPARACIÓN DE LA MÁQUINA",
            new Vector2(.03f, .902f), new Vector2(.72f, .975f), font, 23f,
            TextPrimary, TextAlignmentOptions.Left, FontStyles.Normal);
        TextMeshProUGUI repairValue = CreateText("ProgressValue", root.transform, "0%",
            new Vector2(.73f, .902f), new Vector2(.955f, .98f), font, 31f,
            Hex("F4B155"), TextAlignmentOptions.Right, FontStyles.Normal);
        GameObject progressDivider = CreatePanel("ProgressDivider", root.transform,
            new Vector2(.03f, .887f), new Vector2(.97f, .891f), null,
            Hex("345568", 220));
        progressDivider.GetComponent<Image>().raycastTarget = false;
        GameObject repairTrack = CreatePanel("RepairTrack", root.transform,
            new Vector2(.03f, .804f), new Vector2(.97f, .851f), null, Hex("0C1C26"));
        Outline trackOutline = repairTrack.AddComponent<Outline>();
        trackOutline.effectColor = Hex("365968");
        trackOutline.effectDistance = new Vector2(1f, -1f);
        GameObject repairFillObject = CreateRect("Fill", repairTrack.transform,
            new Vector2(.004f, .12f), new Vector2(.996f, .88f));
        Image repairFill = repairFillObject.AddComponent<Image>();
        repairFill.color = Cyan;
        repairFill.type = Image.Type.Filled;
        repairFill.fillMethod = Image.FillMethod.Horizontal;
        repairFill.raycastTarget = false;
        GameObject threshold = CreateRect("Threshold80", repairTrack.transform,
            new Vector2(.798f, -.34f), new Vector2(.802f, 1.34f));
        Image thresholdImage = threshold.AddComponent<Image>();
        thresholdImage.color = Hex("F5A541");
        thresholdImage.raycastTarget = false;
        CreateText("ThresholdLabel", root.transform, "80% REQUERIDO",
            new Vector2(.785f, .745f), new Vector2(.97f, .795f), font, 16f,
            Hex("E6AA5E"), TextAlignmentOptions.Center, FontStyles.Normal);

        CreateText("RequirementsTitle", root.transform, "REQUISITOS",
            new Vector2(.03f, .665f), new Vector2(.97f, .72f), font, 21f,
            Hex("84D1E0"), TextAlignmentOptions.Left, FontStyles.Normal);

        Graphic repairRequirementPanel =
            CreateChamferedPanel("RepairRequirement", root.transform,
                new Vector2(.03f, .575f), new Vector2(.97f, .65f),
                Hex("08111E", 245), Hex("223C4B"), 1f, 5f)
            .GetComponent<PrestigeChamferedPanelGraphic>();
        TextMeshProUGUI repairRequirementText = CreateText("Label",
            repairRequirementPanel.transform, "REPARACIÓN AL 80%", new Vector2(.055f, .1f),
            new Vector2(.70f, .9f), font, 20f, TextPrimary,
            TextAlignmentOptions.Left, FontStyles.Normal);
        AddRequirementDot(repairRequirementPanel.transform, Hex("EBA03C"));
        TextMeshProUGUI repairRequirementStateText = CreateText("State",
            repairRequirementPanel.transform, "PENDIENTE", new Vector2(.70f, .1f),
            new Vector2(.975f, .9f), font, 18f, Hex("EFA64C"),
            TextAlignmentOptions.Right, FontStyles.Normal);

        Graphic channelRequirementPanel =
            CreateChamferedPanel("ChannelRequirement", root.transform,
                new Vector2(.03f, .48f), new Vector2(.97f, .555f),
                Hex("08111E", 245), Hex("223C4B"), 1f, 5f)
            .GetComponent<PrestigeChamferedPanelGraphic>();
        TextMeshProUGUI channelRequirementText = CreateText("Label",
            channelRequirementPanel.transform, "CANAL DE CONVERGENCIA",
            new Vector2(.055f, .1f), new Vector2(.70f, .9f), font, 20f,
            TextPrimary, TextAlignmentOptions.Left, FontStyles.Normal);
        AddRequirementDot(channelRequirementPanel.transform, Hex("EBA03C"));
        TextMeshProUGUI channelRequirementStateText = CreateText("State",
            channelRequirementPanel.transform, "BLOQUEADO", new Vector2(.70f, .1f),
            new Vector2(.975f, .9f), font, 18f, Hex("EFA64C"),
            TextAlignmentOptions.Right, FontStyles.Normal);

        GameObject divider = CreatePanel("EffectsDivider", root.transform,
            new Vector2(.03f, .435f), new Vector2(.97f, .438f), null, Hex("304B5B", 210));
        divider.GetComponent<Image>().raycastTarget = false;
        CreateText("EffectsTitle", root.transform, "AL ACTIVAR",
            new Vector2(.03f, .36f), new Vector2(.97f, .42f), font, 19f,
            Hex("84D1E0"), TextAlignmentOptions.Left, FontStyles.Normal);
        BuildEffectLine(root.transform, "ResetEffect", .305f, "REINICIA",
            "LABORATORIO BASE", font, Hex("EE9A3E"));
        BuildEffectLine(root.transform, "PreserveEffect", .235f, "CONSERVA",
            "CONOCIMIENTO Y DIMENSIONES", font, Hex("5EDAD2"));
        BuildEffectLine(root.transform, "RevealEffect", .165f, "REVELA",
            "UNA FIRMA DIMENSIONAL", font, Hex("BC63FF"));

        TextMeshProUGUI status = CreateText("CurrentStatus", root.transform,
            string.Empty, new Vector2(.03f, .125f), new Vector2(.97f, .16f),
            font, 14f, TextSecondary, TextAlignmentOptions.Center, FontStyles.Normal);
        status.gameObject.SetActive(false);
        Button action = CreateChamferedButton("PrestigeAction", root.transform,
            "REQUISITOS PENDIENTES", new Vector2(.12f, .019f), new Vector2(.88f, .155f),
            font, Hex("121822"), Hex("697781"), 3f, 16f, 28f);
        action.GetComponentInChildren<TextMeshProUGUI>(true).fontStyle = FontStyles.Normal;
        TextMeshProUGUI actionLabel = action.GetComponentInChildren<TextMeshProUGUI>(true);

        return new StatusParts
        {
            repairValue = repairValue,
            repairFill = repairFill,
            repairRequirementPanel = repairRequirementPanel,
            repairRequirementText = repairRequirementText,
            repairRequirementStateText = repairRequirementStateText,
            channelRequirementPanel = channelRequirementPanel,
            channelRequirementText = channelRequirementText,
            channelRequirementStateText = channelRequirementStateText,
            status = status,
            action = action,
            actionLabel = actionLabel
        };
    }

    private static void BuildEffectLine(Transform parent, string name, float minY,
        string heading, string detail, TMP_FontAsset font, Color accent)
    {
        GameObject line = CreateRect(name, parent, new Vector2(.05f, minY),
            new Vector2(.95f, minY + .055f));
        CreateText("Heading", line.transform, heading, new Vector2(0f, 0f),
            new Vector2(.22f, 1f), font, 18f, accent, TextAlignmentOptions.Left,
            FontStyles.Normal);
        CreateText("Detail", line.transform, detail, new Vector2(.23f, 0f),
            Vector2.one, font, 18f, TextPrimary, TextAlignmentOptions.Left,
            FontStyles.Normal);
    }

    private static TextMeshProUGUI CreateStatusBadge(Transform parent, TMP_FontAsset font,
        Sprite selectorFrame)
    {
        GameObject badge = CreateChamferedPanel("PrestigeStatusBadge", parent,
            new Vector2(.755f, .711f), new Vector2(.91f, .743f),
            Hex("271710", 235), Hex("E99936", 230), 2f, 9f);
        return CreateText("Label", badge.transform, "BLOQUEADO", new Vector2(.05f, .08f),
            new Vector2(.95f, .92f), font, 20f, Hex("F4B25B"),
            TextAlignmentOptions.Center, FontStyles.Normal);
    }

    private static void BuildPrestigeSignature(Transform parent)
    {
        Transform old = parent.Find("PrestigeSignature");
        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        GameObject signature = CreateRect("PrestigeSignature", parent,
            new Vector2(.366f, .005f), new Vector2(.634f, .009f));
        CreatePanel("Cyan", signature.transform, new Vector2(0f, 0f),
            new Vector2(.39f, 1f), null, Hex("00CDEE", 220));
        CreatePanel("Purple", signature.transform, new Vector2(.39f, 0f),
            new Vector2(.78f, 1f), null, Hex("B053FF", 220));
        CreatePanel("Amber", signature.transform, new Vector2(.78f, 0f),
            Vector2.one, null, Hex("EE9131", 220));
    }

    private static TMP_FontAsset LoadFont()
    {
        VerticalUiTheme theme = AssetDatabase.LoadAssetAtPath<VerticalUiTheme>(ThemePath);
        return theme != null && theme.primaryFont != null
            ? theme.primaryFont
            : AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
    }

    private static void StyleTab(Button button, Color fill, Color border, bool interactable)
    {
        button.interactable = interactable;
        Image image = button.GetComponent<Image>();
        if (image != null)
            Object.DestroyImmediate(image);
        PrestigeChamferedPanelGraphic panel =
            button.gameObject.GetComponent<PrestigeChamferedPanelGraphic>();
        if (panel == null)
            panel = button.gameObject.AddComponent<PrestigeChamferedPanelGraphic>();
        panel.SetStyle(fill, border, 3f, 11f);
        panel.raycastTarget = true;
        button.targetGraphic = panel;
        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.fontSize = 22f;
            label.fontStyle = FontStyles.Normal;
        }
    }

    private static void AddRequirementDot(Transform parent, Color color)
    {
        TextMeshProUGUI ring = CreateText("StateDot", parent, "○",
            new Vector2(.012f, .04f), new Vector2(.045f, .96f), LoadFont(), 30f,
            color, TextAlignmentOptions.Center, FontStyles.Normal);
        ring.enableAutoSizing = false;
    }

    private static void AddOutline(GameObject target, Color color, float distance)
    {
        Outline outline = target.GetComponent<Outline>();
        if (outline == null)
            outline = target.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = new Vector2(distance, -distance);
        outline.useGraphicAlpha = true;
    }

    private static void DisableAllButtonsAndRaycasts(Transform root)
    {
        foreach (Button button in root.GetComponentsInChildren<Button>(true))
            button.gameObject.SetActive(false);
        foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(true))
            graphic.raycastTarget = false;
    }

    private static void Disable(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        if (child != null)
            child.gameObject.SetActive(false);
    }

    private static T RequireComponent<T>(Transform parent, string path) where T : Component
    {
        Transform child = RequireTransform(parent, path);
        T component = child.GetComponent<T>();
        Require(component != null, "Falta " + typeof(T).Name + " en " + path + ".");
        return component;
    }

    private static Transform RequireTransform(Transform parent, string path)
    {
        Transform child = parent.Find(path);
        Require(child != null, "Falta el objeto requerido: " + path + ".");
        return child;
    }

    private static GameObject CreatePanel(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Sprite sprite, Color color)
    {
        GameObject panel = CreateRect(name, parent, anchorMin, anchorMax);
        Image image = panel.AddComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null && sprite.border.sqrMagnitude > 0f
            ? Image.Type.Sliced : Image.Type.Simple;
        image.color = color;
        image.raycastTarget = false;
        return panel;
    }

    private static GameObject CreateChamferedPanel(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Color fill, Color border,
        float borderWidth, float cutSize)
    {
        GameObject panel = CreateRect(name, parent, anchorMin, anchorMax);
        PrestigeChamferedPanelGraphic graphic =
            panel.AddComponent<PrestigeChamferedPanelGraphic>();
        graphic.SetStyle(fill, border, borderWidth, cutSize);
        graphic.raycastTarget = false;
        return panel;
    }

    private static Button CreateChamferedButton(string name, Transform parent,
        string label, Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font,
        Color fill, Color border, float borderWidth, float cutSize, float fontSize)
    {
        GameObject root = CreateChamferedPanel(name, parent, anchorMin, anchorMax,
            fill, border, borderWidth, cutSize);
        PrestigeChamferedPanelGraphic graphic =
            root.GetComponent<PrestigeChamferedPanelGraphic>();
        graphic.raycastTarget = true;
        Button button = root.AddComponent<Button>();
        button.targetGraphic = graphic;
        ColorBlock colors = button.colors;
        colors.disabledColor = new Color(.68f, .72f, .75f, .72f);
        colors.pressedColor = new Color(.78f, .88f, .94f, 1f);
        button.colors = colors;
        CreateText("Label", root.transform, label, new Vector2(.04f, .08f),
            new Vector2(.96f, .92f), font, fontSize, TextPrimary,
            TextAlignmentOptions.Center, FontStyles.Normal);
        return button;
    }

    private static Button CreateButton(string name, Transform parent, string label,
        Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font, Sprite sprite,
        Color color, float fontSize)
    {
        GameObject root = CreatePanel(name, parent, anchorMin, anchorMax, sprite, color);
        Image image = root.GetComponent<Image>();
        image.raycastTarget = true;
        Button button = root.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.disabledColor = new Color(.45f, .48f, .50f, .78f);
        colors.pressedColor = new Color(.75f, .88f, .92f, 1f);
        button.colors = colors;
        CreateText("Label", root.transform, label, new Vector2(.04f, .08f),
            new Vector2(.96f, .92f), font, fontSize, TextPrimary,
            TextAlignmentOptions.Center, FontStyles.Bold);
        return button;
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string value,
        Vector2 anchorMin, Vector2 anchorMax, TMP_FontAsset font, float fontSize,
        Color color, TextAlignmentOptions alignment, FontStyles style)
    {
        GameObject root = CreateRect(name, parent, anchorMin, anchorMax);
        TextMeshProUGUI text = root.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.font = font;
        text.fontSize = fontSize;
        text.fontSizeMin = Mathf.Max(12f, fontSize * .70f);
        text.fontSizeMax = fontSize;
        text.enableAutoSizing = true;
        text.color = color;
        text.alignment = alignment;
        text.fontStyle = style;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.raycastTarget = false;
        return text;
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

    private static void SetAnchors(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetObject(SerializedObject serializedObject, string property,
        Object value)
    {
        SerializedProperty field = serializedObject.FindProperty(property);
        Require(field != null, "No existe el campo serializado: " + property + ".");
        field.objectReferenceValue = value;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new System.InvalidOperationException(message);
    }

    private static Color Hex(string rgb, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + rgb, out Color color);
        color.a = alpha / 255f;
        return color;
    }

    private sealed class StatusParts
    {
        public TextMeshProUGUI repairValue;
        public Image repairFill;
        public Graphic repairRequirementPanel;
        public TextMeshProUGUI repairRequirementText;
        public TextMeshProUGUI repairRequirementStateText;
        public Graphic channelRequirementPanel;
        public TextMeshProUGUI channelRequirementText;
        public TextMeshProUGUI channelRequirementStateText;
        public TextMeshProUGUI status;
        public Button action;
        public TextMeshProUGUI actionLabel;
    }
}
#endif
