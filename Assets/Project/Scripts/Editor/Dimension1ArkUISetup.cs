using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1ArkUISetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Quantum Forge/Dimension 1/Configure Ark UI")]
    public static void ConfigureArkUI()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);

        if (panel == null)
            throw new InvalidOperationException("Dimension1PanelUI no existe en Main.unity.");

        Transform mainContent = FindChild(panel.transform, "Dimension1MainContent");
        GameObject galaxyPanel = FindChild(panel.transform, "GalaxyPanel")?.gameObject;
        Button openGalaxyButton = FindChildComponent<Button>(
            panel.transform,
            "OpenGalaxyPanelButton"
        );
        Button buttonTemplate = FindChildComponent<Button>(
            panel.transform,
            "GalaxySector1Button"
        );
        TextMeshProUGUI textTemplate = FindChildComponent<TextMeshProUGUI>(
            panel.transform,
            "GalaxySectorSummaryText"
        );

        if (mainContent == null || galaxyPanel == null || openGalaxyButton == null ||
            buttonTemplate == null || textTemplate == null)
        {
            throw new InvalidOperationException(
                "Faltan controles base de Galaxia para crear la interfaz de Ark."
            );
        }

        GameObject arkPanel = GetOrCreateArkPanel(panel.transform, galaxyPanel);
        TextMeshProUGUI arkInfoText = GetOrCreateArkInfoText(
            arkPanel.transform,
            textTemplate
        );
        Button openArkButton = GetOrCreateButton(
            mainContent,
            "OpenArkPanelButton",
            openGalaxyButton,
            "Investigar Ark",
            new Vector2(500f, -145f),
            new Vector2(220f, 38f)
        );
        Button closeArkButton = GetOrCreateButton(
            arkPanel.transform,
            "CloseArkPanelButton",
            buttonTemplate,
            "Volver",
            new Vector2(560f, 335f),
            new Vector2(220f, 38f)
        );
        Button investigateArkButton = GetOrCreateButton(
            arkPanel.transform,
            "InvestigateArkButton",
            buttonTemplate,
            "Investigar Ark",
            new Vector2(500f, 245f),
            new Vector2(300f, 42f)
        );
        Button outerSyncButton = GetOrCreateButton(
            arkPanel.transform,
            "StartOuterSyncButton",
            buttonTemplate,
            "Sincronía Exterior",
            new Vector2(500f, 155f),
            new Vector2(300f, 44f)
        );
        Button debrisSyncButton = GetOrCreateButton(
            arkPanel.transform,
            "StartDebrisSyncButton",
            buttonTemplate,
            "Sincronía de Restos",
            new Vector2(500f, 95f),
            new Vector2(300f, 44f)
        );
        Button ancientSyncButton = GetOrCreateButton(
            arkPanel.transform,
            "StartAncientSyncButton",
            buttonTemplate,
            "Sincronía Antigua",
            new Vector2(500f, 35f),
            new Vector2(300f, 44f)
        );
        Button silentSyncButton = GetOrCreateButton(
            arkPanel.transform,
            "StartSilentSyncButton",
            buttonTemplate,
            "Sincronía Silenciosa",
            new Vector2(500f, -25f),
            new Vector2(300f, 44f)
        );
        Button enterArkButton = GetOrCreateButton(
            arkPanel.transform,
            "EnterArkButton",
            buttonTemplate,
            "Entrar a Ark (90 min)",
            new Vector2(500f, -120f),
            new Vector2(300f, 48f)
        );

        SerializedObject serializedPanel = new SerializedObject(panel);
        AssignReference(serializedPanel, "arkPanel", arkPanel);
        AssignReference(serializedPanel, "openArkPanelButton", openArkButton);
        AssignReference(serializedPanel, "closeArkPanelButton", closeArkButton);
        AssignReference(serializedPanel, "arkInfoText", arkInfoText);
        AssignReference(serializedPanel, "investigateArkButton", investigateArkButton);
        AssignReference(serializedPanel, "startOuterSyncButton", outerSyncButton);
        AssignReference(serializedPanel, "startDebrisSyncButton", debrisSyncButton);
        AssignReference(serializedPanel, "startAncientSyncButton", ancientSyncButton);
        AssignReference(serializedPanel, "startSilentSyncButton", silentSyncButton);
        AssignReference(serializedPanel, "enterArkButton", enterArkButton);
        serializedPanel.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);

        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");

        ValidateSavedConnections(panel);
        Debug.Log("[D1 Ark UI] CONFIGURATION_PASS");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Ark UI")]
    public static void ValidateArkUI()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);

        if (panel == null)
            throw new InvalidOperationException("Dimension1PanelUI no existe en Main.unity.");

        ValidateSavedConnections(panel);

        if (Dimension1System.D1CentralSyncMissionIds.Length != 4 ||
            Dimension1System.D1CentralSyncMissionDurationSeconds != 3600.0 ||
            Dimension1System.D1ArkFinalMissionDurationSeconds != 5400.0)
        {
            throw new InvalidOperationException("Las reglas temporales de Ark no coinciden.");
        }

        Debug.Log("[D1 Ark UI] VALIDATION_PASS | 4 sync | 12 requirements | 60m/90m");
    }

    private static GameObject GetOrCreateArkPanel(Transform root, GameObject template)
    {
        Transform existing = FindChild(root, "ArkPanel");
        GameObject result;

        if (existing != null)
        {
            result = existing.gameObject;
        }
        else
        {
            result = UnityEngine.Object.Instantiate(
                template,
                template.transform.parent,
                false
            );
            result.name = "ArkPanel";

            for (int index = result.transform.childCount - 1; index >= 0; index--)
                UnityEngine.Object.DestroyImmediate(result.transform.GetChild(index).gameObject);
        }

        result.SetActive(false);
        return result;
    }

    private static TextMeshProUGUI GetOrCreateArkInfoText(
        Transform parent,
        TextMeshProUGUI template
    )
    {
        Transform existing = FindChild(parent, "ArkInfoText");
        TextMeshProUGUI text = existing != null
            ? existing.GetComponent<TextMeshProUGUI>()
            : UnityEngine.Object.Instantiate(template, parent, false);

        if (text == null)
            throw new InvalidOperationException("No se pudo crear el texto de Ark.");

        text.gameObject.name = "ArkInfoText";
        LocalizedTMP localized = text.GetComponent<LocalizedTMP>();

        if (localized != null)
            UnityEngine.Object.DestroyImmediate(localized);

        text.text = "ARK — CENTRO GALÁCTICO";
        text.fontSize = 16f;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(-270f, 0f);
        rect.sizeDelta = new Vector2(720f, 760f);
        return text;
    }

    private static Button GetOrCreateButton(
        Transform parent,
        string objectName,
        Button template,
        string labelText,
        Vector2 position,
        Vector2 size
    )
    {
        Transform existing = FindChild(parent, objectName);
        Button button = existing != null
            ? existing.GetComponent<Button>()
            : UnityEngine.Object.Instantiate(template, parent, false);

        if (button == null)
            throw new InvalidOperationException("No se pudo crear " + objectName + ".");

        button.gameObject.name = objectName;
        button.onClick = new Button.ButtonClickedEvent();
        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);

        if (label != null)
        {
            LocalizedTMP localized = label.GetComponent<LocalizedTMP>();

            if (localized != null)
                UnityEngine.Object.DestroyImmediate(localized);

            label.text = labelText;
            label.fontSize = 16f;
        }

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        button.gameObject.SetActive(false);
        return button;
    }

    private static void AssignReference(
        SerializedObject target,
        string propertyName,
        UnityEngine.Object value
    )
    {
        SerializedProperty property = target.FindProperty(propertyName);

        if (property == null)
            throw new InvalidOperationException("No existe el campo: " + propertyName);

        property.objectReferenceValue = value;
    }

    private static void ValidateSavedConnections(Dimension1PanelUI panel)
    {
        SerializedObject serializedPanel = new SerializedObject(panel);
        string[] fields =
        {
            "arkPanel",
            "openArkPanelButton",
            "closeArkPanelButton",
            "arkInfoText",
            "investigateArkButton",
            "startOuterSyncButton",
            "startDebrisSyncButton",
            "startAncientSyncButton",
            "startSilentSyncButton",
            "enterArkButton"
        };

        foreach (string field in fields)
        {
            SerializedProperty property = serializedPanel.FindProperty(field);

            if (property == null || property.objectReferenceValue == null)
                throw new InvalidOperationException("Conexión faltante: " + field);
        }

        if (FindChild(panel.transform, "ArkPanel") == null)
            throw new InvalidOperationException("ArkPanel no existe en la jerarquía.");
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T component = root.GetComponentInChildren<T>(true);

            if (component != null)
                return component;
        }

        return null;
    }

    private static T FindChildComponent<T>(Transform parent, string objectName)
        where T : Component
    {
        Transform child = FindChild(parent, objectName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private static Transform FindChild(Transform parent, string objectName)
    {
        if (parent == null)
            return null;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == objectName)
                return child;
        }

        return null;
    }
}
