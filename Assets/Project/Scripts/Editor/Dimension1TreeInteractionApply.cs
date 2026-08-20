#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1TreeInteractionApply
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string RootName = "D1_TreeVisualRoot";

    [MenuItem("Quantum Forge/Dimension 1/Apply Tree Navigation Interaction Safely")]
    public static void Apply()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Transform root = FindSceneTransform(scene, RootName);
        if (root == null) throw new InvalidOperationException("Falta " + RootName + ".");

        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        Dimension1CommandCenterUI commandCenter = FindSceneComponent<Dimension1CommandCenterUI>(scene);
        if (panel == null || commandCenter == null)
            throw new InvalidOperationException("Faltan controladores canónicos de Dimensión 1.");

        GraphicRaycaster raycaster = root.GetComponent<GraphicRaycaster>();
        if (raycaster == null) raycaster = root.gameObject.AddComponent<GraphicRaycaster>();

        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group == null) group = root.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;

        Dimension1TreeNavigationUI navigation = root.GetComponent<Dimension1TreeNavigationUI>();
        if (navigation == null) navigation = root.gameObject.AddComponent<Dimension1TreeNavigationUI>();

        SerializedObject serializedNavigation = new SerializedObject(navigation);
        Assign(serializedNavigation, "panel", panel);
        Assign(serializedNavigation, "commandCenter", commandCenter);
        Assign(serializedNavigation, "canvasGroup", group);
        SetObjectArray(serializedNavigation, "hideWhileOpen", FindNavigationRoots(scene));
        serializedNavigation.ApplyModifiedPropertiesWithoutUndo();

        Button command = EnsureButton(FindDirectChild(root, "CommandCenter") as RectTransform);
        ConfigureClick(command, navigation.OpenCommandCenter, true);

        Transform bottomNavigation = FindDirectChild(root, "BottomNavigation");
        if (bottomNavigation == null) throw new InvalidOperationException("Falta BottomNavigation del Árbol.");
        ConfigureClick(EnsureButton(FindDirectChild(bottomNavigation, "Nav_GALAXIA") as RectTransform),
            navigation.OpenGalaxy, true);
        ConfigureClick(EnsureButton(FindDirectChild(bottomNavigation, "Nav_EXPLORAR") as RectTransform),
            navigation.OpenExplore, true);
        ConfigureClick(EnsureButton(FindDirectChild(bottomNavigation, "Nav_HANGAR") as RectTransform),
            navigation.OpenHangar, true);
        ConfigureClick(EnsureButton(FindDirectChild(bottomNavigation, "Nav_RELIQUIAS") as RectTransform),
            navigation.OpenRelics, true);
        ConfigureClick(EnsureButton(FindDirectChild(bottomNavigation, "Nav_ÁRBOL") as RectTransform),
            null, false);

        Validate(root);
        EditorUtility.SetDirty(root);
        EditorUtility.SetDirty(group);
        EditorUtility.SetDirty(raycaster);
        EditorUtility.SetDirty(navigation);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");

        Debug.Log("[D1 Tree Interaction] APPLY_PASS | navegación local interactiva | navegación global suprimida");
    }

    private static Button EnsureButton(RectTransform root)
    {
        if (root == null) throw new InvalidOperationException("Falta una tarjeta navegable del Árbol.");
        Button button = root.GetComponent<Button>();
        if (button == null) button = root.gameObject.AddComponent<Button>();
        Transform fill = FindDirectChild(root, "Fill");
        button.targetGraphic = fill != null ? fill.GetComponent<Image>() : null;
        if (button.targetGraphic == null)
            throw new InvalidOperationException("La tarjeta " + root.name + " no tiene superficie Fill.");
        button.targetGraphic.raycastTarget = true;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, .88f);
        colors.pressedColor = new Color(.72f, .88f, .94f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(.72f, .72f, .72f, .72f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = .08f;
        button.colors = colors;
        return button;
    }

    private static void ConfigureClick(Button button, UnityAction action, bool interactable)
    {
        for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            UnityEventTools.RemovePersistentListener(button.onClick, i);
        button.onClick.RemoveAllListeners();
        button.interactable = interactable;
        if (action != null) UnityEventTools.AddPersistentListener(button.onClick, action);
        EditorUtility.SetDirty(button);
    }

    private static void Validate(Transform root)
    {
        if (root.GetComponent<Canvas>() == null || root.GetComponent<GraphicRaycaster>() == null ||
            root.GetComponent<Dimension1TreeNavigationUI>() == null)
            throw new InvalidOperationException("La raíz del Árbol no quedó interactiva.");
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        if (group == null || !group.interactable || !group.blocksRaycasts)
            throw new InvalidOperationException("El Árbol todavía bloquea sus propios clics.");

        Transform nav = FindDirectChild(root, "BottomNavigation");
        string[] names = { "Nav_GALAXIA", "Nav_EXPLORAR", "Nav_HANGAR", "Nav_RELIQUIAS", "Nav_ÁRBOL" };
        for (int i = 0; i < names.Length; i++)
        {
            Button button = FindDirectChild(nav, names[i])?.GetComponent<Button>();
            if (button == null) throw new InvalidOperationException("Falta botón real: " + names[i]);
            if (button.targetGraphic == null || !button.targetGraphic.raycastTarget)
                throw new InvalidOperationException("Botón sin superficie de raycast: " + names[i]);
            if (i < 4 && (!button.interactable || button.onClick.GetPersistentEventCount() != 1))
                throw new InvalidOperationException("Botón sin ruta funcional: " + names[i]);
            if (i == 4 && button.interactable)
                throw new InvalidOperationException("La tarjeta Árbol seleccionada debe permanecer inactiva.");
        }

        Button command = FindDirectChild(root, "CommandCenter")?.GetComponent<Button>();
        if (command == null || !command.interactable || command.onClick.GetPersistentEventCount() != 1)
            throw new InvalidOperationException("Centro no quedó conectado desde el Árbol.");
        if (command.targetGraphic == null || !command.targetGraphic.raycastTarget)
            throw new InvalidOperationException("Centro no tiene una superficie de raycast.");
    }

    private static GameObject[] FindNavigationRoots(Scene scene)
    {
        var result = new List<GameObject>();
        string[] names = { "PrimaryNavigationSlot", "SecondaryNavigationSlot", "MachineContextTabs" };
        foreach (string name in names)
        {
            Transform target = FindSceneTransform(scene, name);
            if (target != null) result.Add(target.gameObject);
        }
        return result.ToArray();
    }

    private static void Assign(SerializedObject serializedObject, string propertyName,
        UnityEngine.Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null) throw new InvalidOperationException("No existe propiedad serializada: " + propertyName);
        property.objectReferenceValue = value;
    }

    private static void SetObjectArray<T>(SerializedObject serializedObject, string propertyName,
        T[] values) where T : UnityEngine.Object
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null) throw new InvalidOperationException("No existe arreglo serializado: " + propertyName);
        property.arraySize = values == null ? 0 : values.Length;
        for (int i = 0; values != null && i < values.Length; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
        {
            T component = sceneRoot.GetComponentInChildren<T>(true);
            if (component != null) return component;
        }
        return null;
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
}
#endif
