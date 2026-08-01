#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MobileQaFriendlyLayoutSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string SafeRootName = "MobileSafeAreaRoot";

    [MenuItem("Tools/Quantum Forge/QA/Configure Mobile Friendly Layout")]
    public static void ConfigureMobileFriendlyLayout()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject hud = Require(scene, "HUD");
        GameObject language = Require(scene, "BtnLanguage");
        GameObject reset = Require(scene, "BtnDevReset");
        GameObject qaSafeArea = Require(scene, "QA_SafeArea");
        GameObject speed = Require(scene, "BtnDevMultiplier");
        GameObject tools = Require(scene, "QA_ToolsButton");
        GameObject resourceHud = Require(scene, "Panel_HUD");
        GameObject leText = Require(scene, "LE_Text");
        GameObject tracesText = Require(scene, "Traces_Text");
        GameObject topArea = Require(scene, "TopArea");
        GameObject bottomDrawer = Require(scene, "BottomDrawer");
        GameObject drawerHandle = RequireActive(scene, "DrawerHandle");
        GameObject drawerContent = Require(scene, "DrawerContent");
        GameObject navButtons = Require(scene, "NavButtonsContainer");
        GameObject dimension2 = Require(scene, "Dimension2Panel");
        GameObject dimension3 = Require(scene, "Dimension3Panel");

        GameObject safeRoot = FindDirectChild(hud.transform, SafeRootName);
        if (safeRoot == null)
            safeRoot = new GameObject(SafeRootName, typeof(RectTransform));
        safeRoot.layer = 5;
        safeRoot.transform.SetParent(hud.transform, false);
        Stretch((RectTransform)safeRoot.transform);

        language.transform.SetParent(safeRoot.transform, false);
        reset.transform.SetParent(safeRoot.transform, false);
        bottomDrawer.transform.SetParent(safeRoot.transform, false);
        drawerHandle.transform.SetParent(safeRoot.transform, false);
        drawerHandle.transform.SetAsLastSibling();
        speed.SetActive(false);

        MobileQaFriendlyLayout layout = hud.GetComponent<MobileQaFriendlyLayout>();
        if (layout == null)
            layout = hud.AddComponent<MobileQaFriendlyLayout>();
        MobileQaFriendlyLayout[] duplicates =
            hud.GetComponents<MobileQaFriendlyLayout>();
        for (int index = 1; index < duplicates.Length; index++)
            UnityEngine.Object.DestroyImmediate(duplicates[index]);

        layout.mobileSafeAreaRoot = (RectTransform)safeRoot.transform;
        layout.qaSafeAreaRoot = (RectTransform)qaSafeArea.transform;
        layout.qaSpeedButton = (RectTransform)speed.transform;
        layout.qaToolsButton = (RectTransform)tools.transform;
        layout.languageButton = (RectTransform)language.transform;
        layout.devResetButton = (RectTransform)reset.transform;
        layout.resourceHud = (RectTransform)resourceHud.transform;
        layout.leText = (RectTransform)leText.transform;
        layout.tracesText = (RectTransform)tracesText.transform;
        layout.generationTopArea = (RectTransform)topArea.transform;
        layout.bottomDrawer = (RectTransform)bottomDrawer.transform;
        layout.drawerHandle = (RectTransform)drawerHandle.transform;
        layout.drawerContent = (RectTransform)drawerContent.transform;
        layout.navButtonsContainer = (RectTransform)navButtons.transform;
        layout.dimension2ContentRoot = (RectTransform)dimension2.transform;
        layout.dimension3ContentRoot = (RectTransform)dimension3.transform;

        BottomDrawerToggle toggle = bottomDrawer.GetComponent<BottomDrawerToggle>();
        if (toggle != null)
            toggle.ConfigureLayout(true, 16f, 16f);

        layout.ApplyLayout();
        EditorUtility.SetDirty(layout);
        if (toggle != null)
            EditorUtility.SetDirty(toggle);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("No se pudo guardar Main.unity.");

        Debug.Log("[Mobile QA Friendly Layout] COMPLETE | Safe Area global " +
            "para utilidades | accesos sin solape | recursos | navegación táctil");
    }

    public static void ConfigureAndValidateBatch()
    {
        ConfigureMobileFriendlyLayout();
        MobileQaFriendlyLayoutValidation.Validate();
    }

    private static GameObject Require(Scene scene, string name)
    {
        GameObject result = FindInScene(scene, name);
        if (result == null)
            throw new InvalidOperationException("Falta " + name + " en Main.unity.");
        return result;
    }

    private static GameObject RequireActive(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name == name && current.gameObject.activeSelf)
                    return current.gameObject;
            }
        }

        throw new InvalidOperationException(
            "Falta " + name + " activo en Main.unity.");
    }

    private static GameObject FindInScene(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
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

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }
}
#endif
