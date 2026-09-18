#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class PrestigeMachineScreenValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/Prestige/Validate Approved Machine Screen")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        PrestigeUI prestige = Object.FindFirstObjectByType<PrestigeUI>(FindObjectsInactive.Include);
        MachinePanelUI machine = Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(prestige != null && machine != null,
            "Faltan PrestigeUI o MachinePanelUI.");

        Transform root = prestige.transform.Find("PrestigeMachineScreenRoot");
        Require(root != null && root.gameObject.activeSelf,
            "PrestigeMachineScreenRoot no existe o quedó inactivo.");
        Require(root.GetComponent<PrestigeMachineScreenUI>() != null,
            "Falta PrestigeMachineScreenUI.");
        Require(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(root.gameObject) == 0,
            "La pantalla contiene scripts perdidos.");

        int screenCount = 0;
        foreach (PrestigeMachineScreenUI candidate in Object.FindObjectsByType<PrestigeMachineScreenUI>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (candidate.gameObject.scene == scene) screenCount++;
        Require(screenCount == 1, "Debe existir una única pantalla integrada de Prestigio.");

        Transform source = machine.transform.Find("MachineMonolith2DRoot");
        Require(source != null, "Falta la fuente visual canónica de la Máquina.");
        RawImage sourceArt = Find<RawImage>(source,
            "MachinePrimaryContent/MonolithViewport/MonolithOverview/OverviewArtwork");
        RawImage prestigeArt = Find<RawImage>(root,
            "MachinePrimaryContent/MonolithViewport/MonolithOverview/OverviewArtwork");
        Require(sourceArt.texture != null && sourceArt.texture == prestigeArt.texture,
            "Prestigio no reutiliza el arte canónico del Monolito.");

        Require(Find<TextMeshProUGUI>(root,
            "MachinePrimaryContent/MonolithViewHeader/ViewTitle").text ==
            "PROTOCOLO DE PRESTIGIO 1", "Título de Prestigio incorrecto.");
        Require(Find<TextMeshProUGUI>(root,
            "MachinePrimaryContent/MonolithViewHeader/ViewIndex").text ==
            "LA MÁQUINA PREPARA UNA RUTA DIMENSIONAL", "Subtítulo incorrecto.");
        Require(FindTransform(root, "MachinePrimaryContent/PrestigeStatusPanel") != null,
            "Falta el panel funcional de estado.");
        Require(!FindTransform(root,
            "MachinePrimaryContent/MonolithViewport/MonolithSectorCloseup").gameObject.activeSelf,
            "La vista de sector no debe aparecer dentro de Prestigio.");
        Require(!FindTransform(root, "MachinePrimaryContent/MonolithNodeCard").gameObject.activeSelf,
            "La ficha de nodo no debe aparecer dentro de Prestigio.");

        Button prestigeTab = Find<Button>(root, "MachineContextTabs/PrestigeTab");
        Button nodesTab = Find<Button>(root, "MachineContextTabs/NodesTab");
        Button mixesTab = Find<Button>(root, "MachineContextTabs/MixesTab");
        Require(!prestigeTab.interactable && nodesTab.interactable && mixesTab.interactable,
            "El estado de las pestañas contextuales es incorrecto.");
        Require(Find<Button>(root,
            "MachinePrimaryContent/PrestigeStatusPanel/PrestigeAction") != null,
            "Falta la acción principal de Prestigio.");

        RectTransform viewport = FindTransform(root,
            "MachinePrimaryContent/MonolithViewport").GetComponent<RectTransform>();
        Require(viewport.anchorMin.y >= .33f && viewport.anchorMax.y <= .74f,
            "El Monolito invade el panel inferior de estado.");

        for (int i = 0; i < prestige.transform.childCount; i++)
        {
            Transform child = prestige.transform.GetChild(i);
            if (child != root)
                Require(!child.gameObject.activeSelf,
                    "Contenido viejo todavía visible en PrestigePanel: " + child.name);
        }

        SerializedObject so = new SerializedObject(root.GetComponent<PrestigeMachineScreenUI>());
        string[] requiredReferences =
        {
            "prestigeUI", "machinePanel", "nodesButton", "mixesButton", "prestigeButton",
            "actionButton", "leText", "tracesText", "totalProgressText", "channelText",
            "totalProgressFill", "statusBadgeText", "repairValueText", "repairProgressFill",
            "repairRequirementText", "repairRequirementPanel", "repairRequirementStateText",
            "channelRequirementText", "channelRequirementPanel",
            "channelRequirementStateText", "statusText", "actionText", "overviewArtwork"
        };
        foreach (string property in requiredReferences)
        {
            SerializedProperty field = so.FindProperty(property);
            Require(field != null && field.objectReferenceValue != null,
                "Referencia serializada ausente: " + property);
        }

        Debug.Log("[Prestige Machine Screen Validation] PASS | canonical machine family | old UI hidden | live requirements | nodes + mixes + prestige navigation | static 1080/720-safe anchors");
    }

    public static void ValidateBatch()
    {
        try
        {
            Validate();
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static T Find<T>(Transform root, string path) where T : Component
    {
        Transform target = FindTransform(root, path);
        T component = target.GetComponent<T>();
        Require(component != null, "Falta " + typeof(T).Name + " en " + path + ".");
        return component;
    }

    private static Transform FindTransform(Transform root, string path)
    {
        Transform target = root.Find(path);
        Require(target != null, "Falta el objeto requerido: " + path + ".");
        return target;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
#endif
