#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Mantiene el marco compartido de las seis pantallas verticales D1. Hangar es
/// la autoridad visual para el encabezado y la navegación; el contenido central
/// y sus controladores permanecen bajo la propiedad de cada pantalla.
/// </summary>
public static class Dimension1SharedShellApply
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string FramePath = "Assets/Project/UI/Dimension1/Generated/d1_premium_frame_v4.png";
    private const string FillPath = "Assets/Project/UI/Dimension1/Generated/d1_panel_fill_v4.png";
    private const string FontPath = "Assets/Project/UI/Vertical/Fonts/Rajdhani-Medium SDF.asset";

    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9EABB4");
    private static readonly Color NormalFill = Hex("04121B", 248);
    private static readonly Color SelectedFill = Hex("1A1508", 252);

    private static readonly string[] RootNames =
    {
        "D1CommandCenterProductionRoot",
        "D1_GalaxyVisualRoot",
        "D1_ExploreVisualRoot",
        "D1_HangarVisualRoot",
        "D1_RelicsVisualRoot",
        "D1_TreeVisualRoot"
    };

    private static readonly string[] NavigationLabels =
        { "GALAXIA", "EXPLORAR", "HANGAR", "RELIQUIAS", "ÁRBOL" };

    [MenuItem("Quantum Forge/Dimension 1/Apply Shared Hangar Chrome Safely")]
    public static void Apply()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Sprite frame = LoadRequired<Sprite>(FramePath, "marco canónico D1");
        Sprite fill = LoadRequired<Sprite>(FillPath, "relleno canónico D1");
        TMP_FontAsset font = LoadRequired<TMP_FontAsset>(FontPath, "fuente Rajdhani D1");
        RectTransform hangar = FindSceneTransform(scene, "D1_HangarVisualRoot") as RectTransform;
        if (hangar == null) throw new InvalidOperationException("Falta la autoridad visual D1_HangarVisualRoot.");

        Dimension1PremiumNavigationApply.PrepareAssets();

        // Hangar se normaliza primero para que siempre sea una fuente limpia.
        ApplyToRootInternal(hangar, hangar, frame, fill, font);
        foreach (string rootName in RootNames)
        {
            RectTransform root = FindSceneTransform(scene, rootName) as RectTransform;
            if (root == null) throw new InvalidOperationException("Falta la raíz existente: " + rootName);
            if (root != hangar) ApplyToRootInternal(root, hangar, frame, fill, font);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");

        Validate(scene);
        Debug.Log("[D1 Shared Shell] APPLY_PASS | autoridad Hangar | encabezado y navegación unificados | contenido interno preservado");
    }

    /// <summary>
    /// Reaplica el contrato compartido al final de un constructor individual.
    /// Evita que una reconstrucción futura vuelva a introducir márgenes propios.
    /// </summary>
    public static void ApplyToRoot(RectTransform root)
    {
        if (root == null) throw new ArgumentNullException(nameof(root));
        Sprite frame = LoadRequired<Sprite>(FramePath, "marco canónico D1");
        Sprite fill = LoadRequired<Sprite>(FillPath, "relleno canónico D1");
        TMP_FontAsset font = LoadRequired<TMP_FontAsset>(FontPath, "fuente Rajdhani D1");
        RectTransform hangar = FindSceneTransform(root.gameObject.scene, "D1_HangarVisualRoot") as RectTransform;
        if (hangar == null) throw new InvalidOperationException("Falta D1_HangarVisualRoot para normalizar " + root.name + ".");
        Dimension1PremiumNavigationApply.PrepareAssets();
        ApplyToRootInternal(root, hangar, frame, fill, font);
    }

    public static void ApplyFrameToSubscreen(RectTransform root)
    {
        if (root == null) throw new ArgumentNullException(nameof(root));
        Sprite frame = LoadRequired<Sprite>(FramePath, "marco canónico D1");
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(Dimension1SharedLayoutTokens.Width,
            Dimension1SharedLayoutTokens.Height);
        EnsureOuterFrame(root, frame);
        EditorUtility.SetDirty(root);
    }

    private static void ApplyToRootInternal(RectTransform root, RectTransform hangar, Sprite frame,
        Sprite fill, TMP_FontAsset font)
    {
        root.anchorMin = root.anchorMax = new Vector2(.5f, .5f);
        root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;
        root.sizeDelta = new Vector2(Dimension1SharedLayoutTokens.Width,
            Dimension1SharedLayoutTokens.Height);

        EnsureOuterFrame(root, frame);
        NormalizeHeader(root, hangar, frame, fill, font);
        NormalizeSectorHeading(root, font);
        NormalizeExploreContent(root);
        NormalizeTreeContentStart(root);
        NormalizeNavigation(root, frame, fill, font);
        Dimension1PremiumNavigationApply.ApplyToRoot(root, SelectedIndex(root.name));
        EditorUtility.SetDirty(root);
    }

    private static void NormalizeExploreContent(RectTransform root)
    {
        if (root.name != "D1_ExploreVisualRoot") return;
        // La composición visual elegida en 2026-08-21 posee un selector de modo
        // propio. Sus bloques ya están construidos con el contrato 1080x1920 y
        // no deben recibir las coordenadas de la composición radar anterior.
        if (FindDirectChild(root, "ModePanel") != null)
        {
            SetTopIfPresent(root, "ScannerPanel", 62f, 276f, 954f, 350f);
            SetTopIfPresent(root, "DestinationPanel", 62f, 640f, 954f, 350f);
            SetTopIfPresent(root, "ShipPanel", 62f, 1004f, 462f, 224f);
            SetTopIfPresent(root, "SupportPanel", 550f, 1004f, 466f, 224f);
            SetTopIfPresent(root, "ModePanel", 62f, 1242f, 954f, 146f);
            SetTopIfPresent(root, "ActiveExpedition", 62f, 1402f, 954f, 146f);
            SetTopIfPresent(root, "StartExpedition", 62f, 1562f, 544f, 122f);
            SetTopIfPresent(root, "ExplorationRecord", 648f, 1562f, 368f, 122f);
            return;
        }
        SetTopIfPresent(root, "ScannerPanel", 62f, 334f, 460f, 628f);
        SetTopIfPresent(root, "DestinationPanel", 550f, 334f, 468f, 319f);
        SetTopIfPresent(root, "ShipPanel", 550f, 674f, 468f, 300f);
        SetTopIfPresent(root, "SupportPanel", 62f, 1002f, 954f, 224f);
        SetTopIfPresent(root, "ActiveExpedition", 62f, 1272f, 954f, 216f);
        SetTopIfPresent(root, "StartExpedition", 62f, 1513f, 544f, 122f);
        SetTopIfPresent(root, "ExplorationRecord", 648f, 1513f, 368f, 122f);
    }

    private static void NormalizeSectorHeading(RectTransform root, TMP_FontAsset font)
    {
        if (!IsSectorDetailRoot(root.name)) return;
        RectTransform heading = FindDirectChild(root, "Heading") as RectTransform;
        if (heading == null) return;

        // El botón de regreso pertenece al encabezado compartido. El nombre del
        // sector y su ruta forman una banda propia inmediatamente antes del contenido.
        SetTop(heading, 0f, 0f, Dimension1SharedLayoutTokens.Width, 258f);
        SetGraphicEnabled(FindDirectChild(heading, "Fill"), false);
        SetGraphicEnabled(FindDirectChild(heading, "Border"), false);

        RectTransform back = FindDirectChild(heading, "BackButton") as RectTransform;
        if (back != null) SetTop(back, 28f, 58f, 84f, 72f);

        TMP_Text title = FindDirectChild(heading, "Title")?.GetComponent<TMP_Text>();
        if (title != null)
        {
            string value = title.text;
            SetTop(title.rectTransform, 142f, 184f, 796f, 44f);
            ConfigureTmp(title, value, 30f, FontStyles.Bold, Primary,
                TextAlignmentOptions.Center);
            title.characterSpacing = 2f;
        }

        TMP_Text subtitle = FindDirectChild(heading, "Subtitle")?.GetComponent<TMP_Text>();
        if (subtitle != null)
        {
            string value = subtitle.text;
            SetTop(subtitle.rectTransform, 242f, 228f, 596f, 24f);
            ConfigureTmp(subtitle, value, 18f, FontStyles.Normal, Cyan,
                TextAlignmentOptions.Center);
            subtitle.characterSpacing = 1.5f;
        }
    }

    private static bool IsSectorDetailRoot(string rootName)
    {
        return rootName == "D1_AncientOrbitsVisualRoot" ||
            rootName == "D1_OuterRimDetailVisualRoot" ||
            rootName == "D1_DebrisRingDetailVisualRoot" ||
            rootName == "D1_SilentFrontierDetailVisualRoot";
    }

    private static void SetGraphicEnabled(Transform transform, bool enabled)
    {
        Graphic graphic = transform != null ? transform.GetComponent<Graphic>() : null;
        if (graphic != null) graphic.enabled = enabled;
    }

    private static void SetTopIfPresent(Transform parent, string name, float x, float y,
        float width, float height)
    {
        RectTransform rect = FindDirectChild(parent, name) as RectTransform;
        if (rect != null) SetTop(rect, x, y, width, height);
    }

    private static void NormalizeTreeContentStart(RectTransform root)
    {
        if (root.name != "D1_TreeVisualRoot") return;
        RectTransform titleBand = FindDirectChild(root, "TreeTitleBand") as RectTransform;
        if (titleBand != null) SetTop(titleBand, 18f, 190f, 1044f, 79f);
        RectTransform glyph = FindDirectChild(root, "TreeGlyph") as RectTransform;
        if (glyph != null) SetTop(glyph, 42f, 205f, 43f, 43f);
    }

    private static void EnsureOuterFrame(RectTransform root, Sprite frame)
    {
        Image image = EnsureImage(root, "OuterFrame");
        SetTop(image.rectTransform,
            Dimension1SharedLayoutTokens.OuterFrameX,
            Dimension1SharedLayoutTokens.OuterFrameY,
            Dimension1SharedLayoutTokens.OuterFrameWidth,
            Dimension1SharedLayoutTokens.OuterFrameHeight);
        image.sprite = frame;
        image.type = Image.Type.Sliced;
        image.color = WithAlpha(CyanMuted, 185);
        image.raycastTarget = false;
        image.transform.SetSiblingIndex(Mathf.Min(2, root.childCount - 1));
        EditorUtility.SetDirty(image);
    }

    private static void NormalizeHeader(RectTransform root, RectTransform hangar, Sprite frame,
        Sprite fill, TMP_FontAsset font)
    {
        Transform oldTitle = FindDirectChild(root, "DimensionTitle");
        if (oldTitle != null) oldTitle.gameObject.SetActive(false);

        TMP_Text title = EnsureTmp(root, "SharedDimensionTitle", font);
        SetTop(title.rectTransform,
            Dimension1SharedLayoutTokens.HeaderTitleX,
            Dimension1SharedLayoutTokens.HeaderTitleY,
            Dimension1SharedLayoutTokens.HeaderTitleWidth,
            Dimension1SharedLayoutTokens.HeaderTitleHeight);
        ConfigureTmp(title, "DIMENSIÓN 1", Dimension1SharedLayoutTokens.HeaderTitleFontSize,
            FontStyles.Bold, Primary, TextAlignmentOptions.Center);
        title.characterSpacing = 4f;
        title.transform.SetAsLastSibling();

        if (root.name != "D1CommandCenterProductionRoot")
        {
            RectTransform command = FindHeaderCard(root, "CommandCenter", "CommandCenterBack");
            RectTransform source = FindHeaderCard(hangar, "CommandCenter");
            if (command != null)
            {
                SetTop(command, Dimension1SharedLayoutTokens.CommandCenterX,
                    Dimension1SharedLayoutTokens.CommandCenterY,
                    Dimension1SharedLayoutTokens.CommandCenterWidth,
                    Dimension1SharedLayoutTokens.CommandCenterHeight);
                NormalizeCardFrame(command, frame, fill, false);
                NormalizeHeaderText(command, "Label", font, "CENTRO\nDE MANDO", 16f,
                    FontStyles.Bold, Secondary, TextAlignmentOptions.Center, 52f, 27f, 112f, 48f);
                CopyHangarArt(source, command);
            }
        }

        string[] sourceMetals = { "Metal_0", "Metal_1", "Metal_2" };
        string[] targetMetals = MetalCardNames(root.name);
        string[] metalNames = { "HIERRO", "ALUMINIO", "NÍQUEL" };
        for (int i = 0; i < 3; i++)
        {
            RectTransform card = FindHeaderCard(root, targetMetals[i]);
            RectTransform source = FindHeaderCard(hangar, sourceMetals[i]);
            if (card == null) continue;
            SetTop(card, Dimension1SharedLayoutTokens.MetalCardX +
                Dimension1SharedLayoutTokens.MetalCardStep * i,
                Dimension1SharedLayoutTokens.MetalCardY,
                Dimension1SharedLayoutTokens.MetalCardWidth,
                Dimension1SharedLayoutTokens.MetalCardHeight);
            NormalizeCardFrame(card, frame, fill, false);
            NormalizeHeaderText(card, "Name", font, metalNames[i], 15f, FontStyles.Bold,
                Secondary, TextAlignmentOptions.Left, 51f, 12f, 124f, 23f);
            NormalizeHeaderText(card, FindAmountName(card), font, null, 27f, FontStyles.Normal,
                Primary, TextAlignmentOptions.Left, 50f, 32f, 124f, 39f);
            NormalizeHeaderText(card, "Rate", font, null, 15f, FontStyles.Bold,
                Cyan, TextAlignmentOptions.Right, 105f, 67f, 70f, 23f);
            CopyHangarArt(source, card);
        }

        bool usesAllMetals = root.name == "D1_GalaxyVisualRoot" ||
            root.name == "D1_AncientOrbitsVisualRoot" ||
            root.name == "D1_OuterRimDetailVisualRoot" ||
            root.name == "D1_DebrisRingDetailVisualRoot" ||
            root.name == "D1_SilentFrontierDetailVisualRoot";
        RectTransform allMetals = FindHeaderCard(root, usesAllMetals ? "AllMetals" : "MetalsButton");
        RectTransform allMetalsSource = FindHeaderCard(hangar, "MetalsButton");
        if (allMetals != null)
        {
            EnsureMetalsButton(allMetals, root.gameObject.scene);
            SetTop(allMetals, Dimension1SharedLayoutTokens.AllMetalsX,
                Dimension1SharedLayoutTokens.AllMetalsY,
                Dimension1SharedLayoutTokens.AllMetalsWidth,
                Dimension1SharedLayoutTokens.AllMetalsHeight);
            NormalizeCardFrame(allMetals, frame, fill, false);
            NormalizeHeaderText(allMetals, "Label", font, "10 METALES", 19f, FontStyles.Bold,
                Cyan, TextAlignmentOptions.Center, 55f, 29f, 145f, 36f);
            CopyHangarArt(allMetalsSource, allMetals);
        }
    }

    private static void EnsureMetalsButton(RectTransform card, Scene scene)
    {
        Button button = card.GetComponent<Button>();
        if (button == null) button = card.gameObject.AddComponent<Button>();
        button.interactable = true;
        button.transition = Selectable.Transition.None;

        Transform inventoryRoot = FindSceneTransform(scene, "D1_MetalsInventoryRoot");
        Dimension1MetalsInventoryUI inventory = inventoryRoot != null
            ? inventoryRoot.GetComponent<Dimension1MetalsInventoryUI>()
            : null;
        if (inventory != null)
        {
            while (button.onClick.GetPersistentEventCount() > 0)
                UnityEventTools.RemovePersistentListener(button.onClick, 0);
            UnityEventTools.AddPersistentListener(button.onClick, inventory.Open);
        }
        EditorUtility.SetDirty(card.gameObject);
    }

    private static void NormalizeNavigation(RectTransform root, Sprite frame, Sprite fill,
        TMP_FontAsset font)
    {
        Transform navigation = FindDirectChild(root, "BottomNavigation") ??
            FindDirectChild(root, "D1BottomNavigation");
        Transform cardParent = navigation != null ? navigation : root;

        if (navigation is RectTransform navRect)
            SetTop(navRect, Dimension1SharedLayoutTokens.NavigationX,
                Dimension1SharedLayoutTokens.NavigationY,
                Dimension1SharedLayoutTokens.NavigationWidth,
                Dimension1SharedLayoutTokens.NavigationHeight);

        List<RectTransform> cards = CollectNavigationCards(cardParent);
        if (cards.Count != 5)
            throw new InvalidOperationException("Navegación incompleta en " + root.name + ".");

        int selected = SelectedIndex(root.name);
        for (int i = 0; i < cards.Count; i++)
        {
            float x = Dimension1SharedLayoutTokens.NavigationCardX(i);
            float y = Dimension1SharedLayoutTokens.NavigationCardY;
            if (navigation == null)
            {
                x += Dimension1SharedLayoutTokens.NavigationX;
                y += Dimension1SharedLayoutTokens.NavigationY;
            }
            SetTop(cards[i], x, y,
                Dimension1SharedLayoutTokens.NavigationCardWidth,
                Dimension1SharedLayoutTokens.NavigationCardHeight);

            bool isSelected = i == selected;
            NormalizeCardFrame(cards[i], frame, fill, isSelected);
            Transform legacyLabel = FindDirectChild(cards[i], "Label");
            if (legacyLabel != null) legacyLabel.gameObject.SetActive(false);
            TMP_Text label = EnsureTmp(cards[i], "SharedNavigationLabel", font);
            SetTop(label.rectTransform,
                Dimension1SharedLayoutTokens.NavigationLabelX,
                Dimension1SharedLayoutTokens.NavigationLabelY,
                Dimension1SharedLayoutTokens.NavigationLabelWidth,
                Dimension1SharedLayoutTokens.NavigationLabelHeight);
            ConfigureTmp(label, NavigationLabels[i],
                Dimension1SharedLayoutTokens.NavigationLabelFontSize, FontStyles.Bold,
                isSelected ? Amber : Cyan, TextAlignmentOptions.Center);
            label.transform.SetAsLastSibling();

            Button button = cards[i].GetComponent<Button>();
            if (button != null) button.interactable = !isSelected;
        }

        NormalizeSelectedPointer(root, cardParent as RectTransform, selected);
    }

    private static void NormalizeSelectedPointer(RectTransform root, RectTransform navigation,
        int selectedIndex)
    {
        if (navigation == null || selectedIndex < 0) return;
        Transform pointerTransform = FindDirectChild(navigation, "SelectedPointer");
        Dimension1CommandCenterPolygonGraphic polygon;
        if (pointerTransform == null)
        {
            GameObject go = new GameObject("SelectedPointer", typeof(RectTransform));
            go.layer = root.gameObject.layer;
            go.transform.SetParent(navigation, false);
            Stretch((RectTransform)go.transform);
            polygon = go.AddComponent<Dimension1CommandCenterPolygonGraphic>();
        }
        else
        {
            pointerTransform.gameObject.SetActive(true);
            polygon = pointerTransform.GetComponent<Dimension1CommandCenterPolygonGraphic>();
            if (polygon == null)
            {
                Graphic previousGraphic = pointerTransform.GetComponent<Graphic>();
                if (previousGraphic != null) UnityEngine.Object.DestroyImmediate(previousGraphic);
                polygon = pointerTransform.gameObject.AddComponent<Dimension1CommandCenterPolygonGraphic>();
            }
            Stretch((RectTransform)pointerTransform);
        }

        float step = Dimension1SharedLayoutTokens.NavigationCardStep;
        polygon.color = Amber;
        polygon.raycastTarget = false;
        polygon.SetPolygon(new[]
        {
            new Vector2(-424f + selectedIndex * step, 88f),
            new Vector2(-410f + selectedIndex * step, 108f),
            new Vector2(-396f + selectedIndex * step, 88f)
        });
        polygon.transform.SetAsLastSibling();
        EditorUtility.SetDirty(polygon);
    }

    private static void NormalizeCardFrame(RectTransform card, Sprite frame, Sprite fill,
        bool selected)
    {
        for (int i = 0; i < card.childCount; i++)
        {
            Transform child = card.GetChild(i);
            string childName = child.name;
            if (childName == "PanelFill" || childName == "OuterLine" || childName == "InnerLine" ||
                childName.EndsWith("_Border", StringComparison.Ordinal) ||
                childName.EndsWith("_InnerBorder", StringComparison.Ordinal))
                child.gameObject.SetActive(false);
        }

        Image shadow = EnsureImage(card, "Shadow");
        Stretch(shadow.rectTransform, new Vector2(5f, -5f), new Vector2(5f, -5f));
        shadow.sprite = fill;
        shadow.type = Image.Type.Sliced;
        shadow.color = Hex("000000", 120);
        shadow.raycastTarget = false;
        shadow.transform.SetSiblingIndex(0);

        Image panelFill = EnsureImage(card, "Fill");
        Stretch(panelFill.rectTransform);
        panelFill.sprite = fill;
        panelFill.type = Image.Type.Sliced;
        panelFill.color = selected ? SelectedFill : NormalFill;
        panelFill.raycastTarget = false;
        panelFill.transform.SetSiblingIndex(1);

        Image border = EnsureImage(card, "Border");
        Stretch(border.rectTransform);
        border.sprite = frame;
        border.type = Image.Type.Sliced;
        border.color = WithAlpha(selected ? Amber : CyanMuted, 205);
        border.raycastTarget = false;
        border.transform.SetSiblingIndex(2);

        Button button = card.GetComponent<Button>();
        if (button != null)
        {
            Image hit = card.GetComponent<Image>();
            if (hit == null) hit = card.gameObject.AddComponent<Image>();
            hit.sprite = null;
            hit.color = new Color(1f, 1f, 1f, .001f);
            hit.raycastTarget = true;
            button.targetGraphic = hit;
        }
        EditorUtility.SetDirty(card);
    }

    private static void CopyHangarArt(RectTransform source, RectTransform target)
    {
        if (source == null || target == null || source == target) return;

        Transform oldContainer = FindDirectChild(target, "SharedHangarArt");
        if (oldContainer != null) UnityEngine.Object.DestroyImmediate(oldContainer.gameObject);

        for (int i = 0; i < target.childCount; i++)
        {
            Transform child = target.GetChild(i);
            if (IsStructureOrText(child)) continue;
            child.gameObject.SetActive(false);
        }

        GameObject containerObject = new GameObject("SharedHangarArt", typeof(RectTransform));
        containerObject.layer = target.gameObject.layer;
        containerObject.transform.SetParent(target, false);
        RectTransform container = (RectTransform)containerObject.transform;
        Stretch(container);
        container.SetSiblingIndex(Mathf.Min(3, target.childCount - 1));

        for (int i = 0; i < source.childCount; i++)
        {
            Transform sourceChild = source.GetChild(i);
            if (IsStructureOrText(sourceChild)) continue;
            GameObject clone = UnityEngine.Object.Instantiate(sourceChild.gameObject, container, false);
            clone.name = sourceChild.name;
            clone.SetActive(true);
            foreach (Graphic graphic in clone.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = false;
        }
        EditorUtility.SetDirty(containerObject);
    }

    private static bool IsStructureOrText(Transform child)
    {
        string name = child.name;
        return name == "Shadow" || name == "Fill" || name == "Border" ||
               name == "PanelFill" || name == "OuterLine" || name == "InnerLine" ||
               name == "SharedHangarArt" || name == "PremiumNavigationIcon" ||
               name.EndsWith("_Border", StringComparison.Ordinal) ||
               name.EndsWith("_InnerBorder", StringComparison.Ordinal) ||
               child.GetComponent<TMP_Text>() != null || child.GetComponent<Text>() != null;
    }

    private static void NormalizeHeaderText(RectTransform parent, string childName,
        TMP_FontAsset font, string value, float size, FontStyles style, Color color,
        TextAlignmentOptions alignment, float x, float y, float width, float height)
    {
        Transform child = FindDirectChild(parent, childName);
        if (child == null) return;
        SetTop((RectTransform)child, x, y, width, height);

        TMP_Text tmp = child.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            tmp.font = font;
            if (value != null) tmp.text = value;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            tmp.overflowMode = TextOverflowModes.Truncate;
            tmp.raycastTarget = false;
            EditorUtility.SetDirty(tmp);
            return;
        }

        Text legacy = child.GetComponent<Text>();
        if (legacy != null)
        {
            if (value != null) legacy.text = value;
            legacy.fontSize = Mathf.RoundToInt(size);
            legacy.fontStyle = style == FontStyles.Bold ? FontStyle.Bold : FontStyle.Normal;
            legacy.color = color;
            legacy.alignment = ToLegacyAlignment(alignment);
            legacy.raycastTarget = false;
            EditorUtility.SetDirty(legacy);
        }
    }

    private static TMP_Text EnsureTmp(Transform parent, string name, TMP_FontAsset font)
    {
        Transform existing = FindDirectChild(parent, name);
        TMP_Text text;
        if (existing == null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer),
                typeof(TextMeshProUGUI));
            go.layer = parent.gameObject.layer;
            go.transform.SetParent(parent, false);
            text = go.GetComponent<TMP_Text>();
        }
        else
        {
            existing.gameObject.SetActive(true);
            text = existing.GetComponent<TMP_Text>();
            if (text == null) text = existing.gameObject.AddComponent<TextMeshProUGUI>();
        }
        text.font = font;
        return text;
    }

    private static void ConfigureTmp(TMP_Text text, string value, float size, FontStyles style,
        Color color, TextAlignmentOptions alignment)
    {
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Truncate;
        text.raycastTarget = false;
        EditorUtility.SetDirty(text);
    }

    private static Image EnsureImage(Transform parent, string name)
    {
        Transform existing = FindDirectChild(parent, name);
        Image image;
        if (existing == null)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Image));
            go.layer = parent.gameObject.layer;
            go.transform.SetParent(parent, false);
            image = go.GetComponent<Image>();
        }
        else
        {
            existing.gameObject.SetActive(true);
            image = existing.GetComponent<Image>();
            if (image == null) image = existing.gameObject.AddComponent<Image>();
        }
        return image;
    }

    private static RectTransform FindHeaderCard(Transform root, params string[] names)
    {
        foreach (string name in names)
        {
            Transform child = FindDirectChild(root, name);
            if (child is RectTransform rect) return rect;
        }
        return null;
    }

    private static string[] MetalCardNames(string rootName)
    {
        if (rootName == "D1CommandCenterProductionRoot")
            return new[] { "Resource_HIERRO", "Resource_ALUMINIO", "Resource_NÍQUEL" };
        if (rootName == "D1_GalaxyVisualRoot")
            return new[] { "Metal_HIERRO", "Metal_ALUMINIO", "Metal_NÍQUEL" };
        return new[] { "Metal_0", "Metal_1", "Metal_2" };
    }

    private static string FindAmountName(RectTransform card)
    {
        if (FindDirectChild(card, "Amount") != null) return "Amount";
        return "Value";
    }

    private static List<RectTransform> CollectNavigationCards(Transform parent)
    {
        var cards = new List<RectTransform>();
        bool commandCenterRoot = parent.name == "D1CommandCenterProductionRoot";
        for (int i = 0; i < parent.childCount; i++)
        {
            RectTransform child = parent.GetChild(i) as RectTransform;
            if (child == null || child.name == "SelectedPointer") continue;
            if (child.name.StartsWith("Nav_", StringComparison.Ordinal) ||
                (!commandCenterRoot && child.name.EndsWith("Button", StringComparison.Ordinal)))
                cards.Add(child);
        }
        cards.Sort((a, b) => a.anchoredPosition.x.CompareTo(b.anchoredPosition.x));
        return cards;
    }

    private static int SelectedIndex(string rootName)
    {
        if (rootName == "D1_GalaxyVisualRoot") return 0;
        if (rootName == "D1_ExploreVisualRoot") return 1;
        if (rootName == "D1_HangarVisualRoot") return 2;
        if (rootName == "D1_RelicsVisualRoot") return 3;
        if (rootName == "D1_TreeVisualRoot") return 4;
        if (rootName == "D1_AncientOrbitsVisualRoot" ||
            rootName == "D1_OuterRimDetailVisualRoot" ||
            rootName == "D1_DebrisRingDetailVisualRoot" ||
            rootName == "D1_SilentFrontierDetailVisualRoot") return 0;
        return -1;
    }

    private static void Validate(Scene scene)
    {
        foreach (string rootName in RootNames)
        {
            RectTransform root = FindSceneTransform(scene, rootName) as RectTransform;
            RectTransform outer = FindDirectChild(root, "OuterFrame") as RectTransform;
            if (!MatchesTop(outer, Dimension1SharedLayoutTokens.OuterFrameX,
                    Dimension1SharedLayoutTokens.OuterFrameY,
                    Dimension1SharedLayoutTokens.OuterFrameWidth,
                    Dimension1SharedLayoutTokens.OuterFrameHeight))
                throw new InvalidOperationException("Marco compartido inválido en " + rootName);

            RectTransform title = FindDirectChild(root, "SharedDimensionTitle") as RectTransform;
            if (!MatchesTop(title, Dimension1SharedLayoutTokens.HeaderTitleX,
                    Dimension1SharedLayoutTokens.HeaderTitleY,
                    Dimension1SharedLayoutTokens.HeaderTitleWidth,
                    Dimension1SharedLayoutTokens.HeaderTitleHeight))
                throw new InvalidOperationException("Título compartido inválido en " + rootName);

            bool usesAllMetals = rootName == "D1_GalaxyVisualRoot" ||
                rootName == "D1_AncientOrbitsVisualRoot" ||
                IsSectorDetailRoot(rootName);
            RectTransform metalsEntry = FindHeaderCard(root,
                usesAllMetals ? "AllMetals" : "MetalsButton");
            Button metalsButton = metalsEntry != null ? metalsEntry.GetComponent<Button>() : null;
            Image metalsHit = metalsEntry != null ? metalsEntry.GetComponent<Image>() : null;
            if (metalsButton == null || metalsHit == null ||
                metalsButton.targetGraphic != metalsHit || !metalsButton.interactable ||
                !metalsHit.raycastTarget)
            {
                throw new InvalidOperationException(
                    "Acceso compartido a 10 METALES inválido en " + rootName);
            }

            bool opensInventory = false;
            for (int i = 0; i < metalsButton.onClick.GetPersistentEventCount(); i++)
            {
                if (metalsButton.onClick.GetPersistentTarget(i) != null &&
                    metalsButton.onClick.GetPersistentMethodName(i) == "Open")
                {
                    opensInventory = true;
                    break;
                }
            }
            if (!opensInventory)
                throw new InvalidOperationException(
                    "10 METALES no conserva su ruta persistente en " + rootName);

            Transform nav = FindDirectChild(root, "BottomNavigation") ??
                FindDirectChild(root, "D1BottomNavigation");
            List<RectTransform> cards = CollectNavigationCards(nav != null ? nav : root);
            if (cards.Count != 5)
                throw new InvalidOperationException("La navegación no tiene cinco tarjetas en " + rootName);
            foreach (RectTransform card in cards)
            {
                RectTransform label = FindDirectChild(card, "SharedNavigationLabel") as RectTransform;
                if (!MatchesTop(label, Dimension1SharedLayoutTokens.NavigationLabelX,
                        Dimension1SharedLayoutTokens.NavigationLabelY,
                        Dimension1SharedLayoutTokens.NavigationLabelWidth,
                        Dimension1SharedLayoutTokens.NavigationLabelHeight))
                    throw new InvalidOperationException("Etiqueta inferior inválida en " + rootName);
            }

            if (IsSectorDetailRoot(rootName))
            {
                RectTransform heading = FindDirectChild(root, "Heading") as RectTransform;
                RectTransform sectorTitle = FindDirectChild(heading, "Title") as RectTransform;
                if (!MatchesTop(heading, 0f, 0f, Dimension1SharedLayoutTokens.Width, 258f) ||
                    !MatchesTop(sectorTitle, 142f, 184f, 796f, 44f))
                    throw new InvalidOperationException("Encabezado de sector inválido en " + rootName);
            }
        }
    }

    private static bool MatchesTop(RectTransform rect, float x, float y, float width, float height)
    {
        return rect != null && Approximately(rect.anchoredPosition, new Vector2(x, -y)) &&
               Approximately(rect.sizeDelta, new Vector2(width, height));
    }

    private static bool Approximately(Vector2 a, Vector2 b) =>
        Mathf.Abs(a.x - b.x) < .01f && Mathf.Abs(a.y - b.y) < .01f;

    private static void SetTop(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
    }

    private static void Stretch(RectTransform rect) => Stretch(rect, Vector2.zero, Vector2.zero);

    private static void Stretch(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(.5f, .5f);
        rect.offsetMin = min;
        rect.offsetMax = -max;
    }

    private static TextAnchor ToLegacyAlignment(TextAlignmentOptions alignment)
    {
        if (alignment == TextAlignmentOptions.Right) return TextAnchor.MiddleRight;
        if (alignment == TextAlignmentOptions.Left) return TextAnchor.MiddleLeft;
        return TextAnchor.MiddleCenter;
    }

    private static T LoadRequired<T>(string path, string label) where T : UnityEngine.Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null) throw new InvalidOperationException("Falta " + label + ": " + path);
        return asset;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        if (parent == null) return null;
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == name) return parent.GetChild(i);
        return null;
    }

    private static Transform FindSceneTransform(Scene scene, string name)
    {
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
            foreach (Transform child in sceneRoot.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
        return null;
    }

    private static Color Hex(string hex, byte alpha = 255)
    {
        if (!ColorUtility.TryParseHtmlString("#" + hex, out Color color)) return Color.white;
        color.a = alpha / 255f;
        return color;
    }

    private static Color WithAlpha(Color color, byte alpha)
    {
        color.a = alpha / 255f;
        return color;
    }
}
#endif
