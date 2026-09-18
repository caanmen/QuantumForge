#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class QaPanelSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string SafeRootName = "QA_SafeArea";
    private const string ToolsButtonName = "QA_ToolsButton";
    private const string PanelRootName = "QA_PanelRoot";

    [MenuItem("Tools/Quantum Forge/QA/Configure Block 3 Panel")]
    public static void ConfigureBlock3Panel()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject hud = FindInScene(scene, "HUD");
        GameObject speedButtonObject = FindInScene(scene, "BtnDevMultiplier");
        GameObject legacyResetObject = FindInScene(scene, "BtnDevReset");
        GameObject verticalQaObject = FindInScene(scene, "Nav_QA");
        Button verticalQaButton = verticalQaObject != null
            ? verticalQaObject.GetComponent<Button>()
            : null;
        if (hud == null || speedButtonObject == null)
        {
            Debug.LogError("[QA Block 3 Setup] Falta HUD o BtnDevMultiplier.");
            return;
        }

        if (legacyResetObject != null)
            Object.DestroyImmediate(legacyResetObject);

        GameObject safeRootObject = FindDirectChild(hud.transform, SafeRootName);
        if (safeRootObject == null)
            safeRootObject = CreateUIObject(SafeRootName, hud.transform);
        RectTransform safeRoot = safeRootObject.GetComponent<RectTransform>();
        Stretch(safeRoot);
        safeRootObject.transform.SetAsLastSibling();

        speedButtonObject.transform.SetParent(safeRoot, false);
        ConfigureAccessRect(speedButtonObject.GetComponent<RectTransform>(),
            new Vector2(16f, -16f), new Vector2(150f, 52f));

        DestroyDirectChild(safeRoot, ToolsButtonName);
        DestroyDirectChild(safeRoot, PanelRootName);

        QaPanelUI panel = safeRootObject.GetComponent<QaPanelUI>();
        if (panel == null)
            panel = safeRootObject.AddComponent<QaPanelUI>();
        QaPanelUI[] duplicatePanels = safeRootObject.GetComponents<QaPanelUI>();
        for (int index = 1; index < duplicatePanels.Length; index++)
            Object.DestroyImmediate(duplicatePanels[index]);

        Button toolsButton = CreateButton(
            ToolsButtonName, safeRoot, "HERRAMIENTAS QA");
        ConfigureAccessRect(toolsButton.GetComponent<RectTransform>(),
            new Vector2(176f, -16f), new Vector2(220f, 52f));
        if (verticalQaButton != null)
            toolsButton.gameObject.SetActive(false);

        GameObject panelRoot = CreateUIObject(PanelRootName, safeRoot);
        Stretch(panelRoot.GetComponent<RectTransform>());
        Image dimmer = panelRoot.AddComponent<Image>();
        dimmer.color = new Color(0.01f, 0.02f, 0.04f, 0.92f);
        dimmer.raycastTarget = true;

        GameObject card = CreateUIObject("QA_Card", panelRoot.transform);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.07f, 0.06f);
        cardRect.anchorMax = new Vector2(0.93f, 0.94f);
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.055f, 0.075f, 0.12f, 1f);
        cardImage.raycastTarget = true;

        ScrollRect scroll = CreateScroll(card.transform, out Transform content);
        var speedButtons = new List<Button>();
        var advanceButtons = new List<Button>();
        var saveButtons = new List<Button>();
        var loadButtons = new List<Button>();
        var checkpointStatusTexts = new List<TMP_Text>();

        CreateTextItem(content, "QA_Title",
            "MODO QA - NO REPRESENTA EL BALANCE FINAL", 24f, 58f,
            TextAlignmentOptions.Center, new Color(1f, 0.72f, 0.22f));
        TMP_Text status = CreateTextItem(content, "QA_SpeedStatus",
            "VELOCIDAD ACTUAL: QA x1", 21f, 48f,
            TextAlignmentOptions.Center, Color.white);
        TMP_Text operationStatus = CreateTextItem(content, "QA_OperationStatus",
            "LISTO", 18f, 64f, TextAlignmentOptions.Center,
            new Color(0.65f, 0.92f, 0.72f));

        CreateSection(content, "VELOCIDAD");
        Transform speedRow = CreateRow(content, "QA_SpeedRow");
        speedButtons.Add(CreateRowButton(speedRow, "QA_Speed1", "x1"));
        speedButtons.Add(CreateRowButton(speedRow, "QA_Speed5", "x5"));
        speedButtons.Add(CreateRowButton(speedRow, "QA_Speed10", "x10"));
        speedButtons.Add(CreateRowButton(speedRow, "QA_Speed20", "x20"));

        CreateSection(content, "AVANZAR TIEMPO");
        Transform shortAdvanceRow = CreateRow(content, "QA_AdvanceShortRow");
        advanceButtons.Add(CreateRowButton(shortAdvanceRow, "QA_Advance5", "+5 MIN"));
        advanceButtons.Add(CreateRowButton(shortAdvanceRow, "QA_Advance30", "+30 MIN"));
        advanceButtons.Add(CreateRowButton(shortAdvanceRow, "QA_Advance60", "+1 H"));
        Transform longAdvanceRow = CreateRow(content, "QA_AdvanceLongRow");
        advanceButtons.Add(CreateRowButton(longAdvanceRow, "QA_Advance480", "+8 H"));
        advanceButtons.Add(CreateRowButton(longAdvanceRow, "QA_Advance720", "+12 H"));
        advanceButtons.Add(CreateRowButton(longAdvanceRow, "QA_Advance1440", "+24 H"));

        CreateSection(content, "CHECKPOINTS");
        CreateCheckpointRow(content, 'A', saveButtons, loadButtons,
            checkpointStatusTexts);
        CreateCheckpointRow(content, 'B', saveButtons, loadButtons,
            checkpointStatusTexts);
        CreateCheckpointRow(content, 'C', saveButtons, loadButtons,
            checkpointStatusTexts);

        CreateSection(content, "PARTIDA");
        Button resetSaveButton = CreateButton(
            "QA_ResetSave", content, "BORRAR PARTIDA Y REINICIAR");
        resetSaveButton.targetGraphic.color =
            new Color(0.46f, 0.11f, 0.13f, 1f);
        LayoutElement resetLayout = resetSaveButton.gameObject
            .AddComponent<LayoutElement>();
        resetLayout.minHeight = 58f;
        resetLayout.preferredHeight = 58f;

        Button closeButton = CreateButton("QA_Close", content, "CERRAR");
        LayoutElement closeLayout = closeButton.gameObject.AddComponent<LayoutElement>();
        closeLayout.minHeight = 58f;
        closeLayout.preferredHeight = 58f;

        GameObject confirmationRoot = CreateConfirmation(
            panelRoot.transform, out TMP_Text confirmationText,
            out Button confirmationAccept, out Button confirmationCancel);

        panel.safeAreaRoot = safeRoot;
        panel.toolsButton = verticalQaButton != null
            ? verticalQaButton
            : toolsButton;
        panel.panelRoot = panelRoot;
        panel.scrollRect = scroll;
        panel.speedStatusText = status;
        panel.operationStatusText = operationStatus;
        panel.speedButtons = speedButtons.ToArray();
        panel.advanceButtons = advanceButtons.ToArray();
        panel.checkpointSaveButtons = saveButtons.ToArray();
        panel.checkpointLoadButtons = loadButtons.ToArray();
        panel.checkpointStatusTexts = checkpointStatusTexts.ToArray();
        panel.resetSaveButton = resetSaveButton;
        panel.closeButton = closeButton;
        panel.confirmationRoot = confirmationRoot;
        panel.confirmationText = confirmationText;
        panel.confirmationAcceptButton = confirmationAccept;
        panel.confirmationCancelButton = confirmationCancel;

        confirmationRoot.SetActive(false);
        panelRoot.SetActive(false);
        safeRootObject.SetActive(true);

        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
        {
            Debug.LogError("[QA Block 3 Setup] No se pudo guardar Main.unity.");
            return;
        }

        Debug.Log("[QA Block 3 Setup] COMPLETE | Safe Area | acceso | scroll | " +
            "velocidades | avance | checkpoints | reinicio confirmado | confirmación");
    }

    public static void ConfigureTwiceAndValidateBatch()
    {
        ConfigureBlock3Panel();
        ConfigureBlock3Panel();
        QaBlock3Validation.ValidateBlock3();
    }

    private static ScrollRect CreateScroll(Transform parent, out Transform content)
    {
        GameObject scrollObject = CreateUIObject("QA_Scroll", parent);
        RectTransform scrollRectTransform = scrollObject.GetComponent<RectTransform>();
        Stretch(scrollRectTransform);
        scrollRectTransform.offsetMin = new Vector2(20f, 20f);
        scrollRectTransform.offsetMax = new Vector2(-20f, -20f);
        ScrollRect scroll = scrollObject.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 30f;

        GameObject viewport = CreateUIObject("Viewport", scrollObject.transform);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        Stretch(viewportRect);
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(0f, 0f, 0f, 0.01f);
        viewport.AddComponent<RectMask2D>();

        GameObject contentObject = CreateUIObject("Content", viewport.transform);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;
        VerticalLayoutGroup layout = contentObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = contentObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRect;
        content = contentObject.transform;
        return scroll;
    }

    private static Transform CreateRow(Transform parent, string name)
    {
        GameObject row = CreateUIObject(name, parent);
        HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        LayoutElement element = row.AddComponent<LayoutElement>();
        element.minHeight = 58f;
        element.preferredHeight = 58f;
        return row.transform;
    }

    private static Button CreateRowButton(Transform parent, string name, string text)
    {
        Button button = CreateButton(name, parent, text);
        LayoutElement element = button.gameObject.AddComponent<LayoutElement>();
        element.minHeight = 52f;
        element.preferredHeight = 56f;
        element.flexibleWidth = 1f;
        return button;
    }

    private static void CreateCheckpointRow(
        Transform parent, char slot, List<Button> saveButtons,
        List<Button> loadButtons, List<TMP_Text> statusTexts)
    {
        Transform row = CreateRow(parent, "QA_Checkpoint" + slot);
        LayoutElement rowLayout = row.GetComponent<LayoutElement>();
        rowLayout.minHeight = 68f;
        rowLayout.preferredHeight = 68f;
        GameObject slotInfo = CreateUIObject("QA_SlotInfo" + slot, row);
        VerticalLayoutGroup slotLayout =
            slotInfo.AddComponent<VerticalLayoutGroup>();
        slotLayout.spacing = 1f;
        slotLayout.childAlignment = TextAnchor.MiddleCenter;
        slotLayout.childControlWidth = true;
        slotLayout.childControlHeight = true;
        slotLayout.childForceExpandWidth = true;
        slotLayout.childForceExpandHeight = false;
        LayoutElement slotInfoLayout = slotInfo.AddComponent<LayoutElement>();
        slotInfoLayout.minWidth = 150f;
        slotInfoLayout.flexibleWidth = 0.7f;

        TMP_Text label = CreateText("Slot " + slot, slotInfo.transform,
            "SLOT " + slot, 18f, TextAlignmentOptions.Center, Color.white);
        LayoutElement labelLayout = label.gameObject.AddComponent<LayoutElement>();
        labelLayout.preferredHeight = 24f;
        TMP_Text status = CreateText("Status " + slot, slotInfo.transform,
            "VACÍO", 16f, TextAlignmentOptions.Center,
            new Color(0.60f, 0.68f, 0.76f));
        LayoutElement statusLayout = status.gameObject.AddComponent<LayoutElement>();
        statusLayout.preferredHeight = 40f;
        statusTexts.Add(status);
        saveButtons.Add(CreateRowButton(row, "QA_Save" + slot, "GUARDAR"));
        loadButtons.Add(CreateRowButton(row, "QA_Load" + slot, "CARGAR"));
    }

    private static void CreateSection(Transform parent, string text)
    {
        CreateTextItem(parent, "QA_Section_" + text.Replace(" ", "_"), text,
            20f, 42f, TextAlignmentOptions.Left,
            new Color(0.45f, 0.82f, 1f));
    }

    private static TMP_Text CreateTextItem(
        Transform parent, string name, string text, float fontSize,
        float height, TextAlignmentOptions alignment, Color color)
    {
        TMP_Text label = CreateText(name, parent, text, fontSize, alignment, color);
        LayoutElement layout = label.gameObject.AddComponent<LayoutElement>();
        layout.minHeight = height;
        layout.preferredHeight = height;
        return label;
    }

    private static GameObject CreateConfirmation(
        Transform parent, out TMP_Text message, out Button accept,
        out Button cancel)
    {
        GameObject root = CreateUIObject("QA_Confirmation", parent);
        Stretch(root.GetComponent<RectTransform>());
        Image blocker = root.AddComponent<Image>();
        blocker.color = new Color(0f, 0f, 0f, 0.78f);
        blocker.raycastTarget = true;

        GameObject card = CreateUIObject("ConfirmationCard", root.transform);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(660f, 280f);
        Image image = card.AddComponent<Image>();
        image.color = new Color(0.08f, 0.11f, 0.18f, 1f);

        message = CreateText("Message", card.transform, "CONFIRMAR", 22f,
            TextAlignmentOptions.Center, Color.white);
        RectTransform messageRect = message.rectTransform;
        messageRect.anchorMin = new Vector2(0.08f, 0.38f);
        messageRect.anchorMax = new Vector2(0.92f, 0.90f);
        messageRect.offsetMin = Vector2.zero;
        messageRect.offsetMax = Vector2.zero;

        accept = CreateButton("Accept", card.transform, "CONFIRMAR");
        ConfigureBottomButton(accept.GetComponent<RectTransform>(), 0.30f);
        cancel = CreateButton("Cancel", card.transform, "CANCELAR");
        ConfigureBottomButton(cancel.GetComponent<RectTransform>(), 0.70f);
        return root;
    }

    private static void ConfigureBottomButton(RectTransform rect, float anchorX)
    {
        rect.anchorMin = new Vector2(anchorX, 0.18f);
        rect.anchorMax = new Vector2(anchorX, 0.18f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(220f, 58f);
        rect.anchoredPosition = Vector2.zero;
    }

    private static Button CreateButton(string name, Transform parent, string text)
    {
        GameObject buttonObject = CreateUIObject(name, parent);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.13f, 0.26f, 0.36f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick = new Button.ButtonClickedEvent();
        TMP_Text label = CreateText("Label", buttonObject.transform, text, 18f,
            TextAlignmentOptions.Center, Color.white);
        Stretch(label.rectTransform);
        return button;
    }

    private static TMP_Text CreateText(
        string name, Transform parent, string text, float size,
        TextAlignmentOptions alignment, Color color)
    {
        GameObject textObject = CreateUIObject(name, parent);
        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.alignment = alignment;
        label.color = color;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.raycastTarget = false;
        return label;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject result = new GameObject(
            name, typeof(RectTransform), typeof(CanvasRenderer));
        result.layer = 5;
        result.transform.SetParent(parent, false);
        return result;
    }

    private static void ConfigureAccessRect(
        RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static GameObject FindInScene(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform current in transforms)
            {
                if (current.name == name)
                    return current.gameObject;
            }
        }
        return null;
    }

    private static GameObject FindDirectChild(Transform parent, string name)
    {
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }

    private static void DestroyDirectChild(Transform parent, string name)
    {
        GameObject child = FindDirectChild(parent, name);
        if (child != null)
            Object.DestroyImmediate(child);
    }
}
#endif
