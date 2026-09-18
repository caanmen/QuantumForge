#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MobileQaFriendlyLayoutValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/QA/Validate Mobile Friendly Layout")]
    public static void Validate()
    {
        var failures = new List<string>();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        MobileQaFriendlyLayout layout =
            UnityEngine.Object.FindFirstObjectByType<MobileQaFriendlyLayout>(
                FindObjectsInactive.Include);
        Check(layout != null, "Falta MobileQaFriendlyLayout.", failures);
        if (layout != null)
        {
            Check(layout.mobileSafeAreaRoot != null &&
                layout.mobileSafeAreaRoot.name == "MobileSafeAreaRoot",
                "Falta el Safe Area de utilidades.", failures);
            Check(layout.languageButton != null &&
                layout.languageButton.parent == layout.mobileSafeAreaRoot,
                "Idioma no está dentro del Safe Area.", failures);
            Check(layout.devResetButton != null &&
                layout.devResetButton.parent == layout.mobileSafeAreaRoot,
                "RESET no esta dentro del Safe Area.", failures);
            Check(layout.bottomDrawer != null &&
                layout.bottomDrawer.parent == layout.mobileSafeAreaRoot,
                "La navegación no está dentro del Safe Area.", failures);
            Check(layout.drawerHandle != null &&
                layout.drawerHandle.parent == layout.mobileSafeAreaRoot,
                "El control MOSTRAR/OCULTAR puede desaparecer con la barra.",
                failures);
            Check(layout.qaSpeedButton != null &&
                layout.qaSpeedButton.parent == layout.qaSafeAreaRoot,
                "El acceso QA de velocidad salió de su Safe Area.", failures);
            if (layout.qaToolsButton != null)
            {
                Check(layout.qaToolsButton.parent == layout.qaSafeAreaRoot &&
                    layout.qaToolsButton.sizeDelta.y == 52f,
                    "El acceso QA auxiliar salió de su Safe Area.", failures);
            }
            else
            {
                Check(CountNamed(scene, "Nav_QA") == 1,
                    "Falta tanto el acceso QA auxiliar como la pestaña QA vertical.",
                    failures);
            }
            Check(layout.qaSpeedButton != null &&
                layout.qaSpeedButton.sizeDelta.y == 52f &&
                layout.languageButton.sizeDelta.y <= 36f,
                "Los accesos de utilidad no conservan el tamaño compacto.", failures);
            Check(layout.qaSpeedButton != null &&
                !layout.qaSpeedButton.gameObject.activeSelf,
                "QA xN externo sigue ocupando espacio fuera del panel.", failures);
            Check(layout.devResetButton.anchorMin == Vector2.zero &&
                layout.devResetButton.anchorMax == Vector2.zero &&
                layout.devResetButton.pivot == Vector2.zero &&
                layout.devResetButton.anchoredPosition.x >= 16f &&
                layout.devResetButton.anchoredPosition.y >= 16f,
                "RESET puede salir del borde en pantallas ultrapanoramicas.",
                failures);
            float maximumNavigationHeight = layout.forcePortraitLayout ? 112f : 58f;
            Check(layout.bottomDrawer.sizeDelta.y <= maximumNavigationHeight,
                "La navegación inferior volvió a crecer.", failures);
            Check(layout.bottomDrawer.anchoredPosition.y >= 16f,
                "La navegacion inferior no conserva margen seguro.", failures);
            Check(HasNavigationClearance(layout.dimension2ContentRoot, layout),
                "Dimensión 2 no reserva espacio para la navegación.", failures);
            Check(HasNavigationClearance(layout.dimension3ContentRoot, layout),
                "Dimensión 3 no reserva espacio para la navegación.", failures);
            Check(layout.drawerHandle.anchorMin == new Vector2(1f, 0f) &&
                layout.drawerHandle.anchorMax == new Vector2(1f, 0f) &&
                layout.drawerHandle.anchoredPosition.y <=
                    (layout.forcePortraitLayout ? 136f : 16f) &&
                layout.drawerHandle.sizeDelta.y <=
                    (layout.forcePortraitLayout ? 40f : 34f),
                "MOSTRAR/OCULTAR volvió a invadir el contenido central.",
                failures);
            ValidateNavigationButtons(layout.navButtonsContainer, failures);
        }

        foreach (float width in new[] { 1000f, 1920f, 2400f, 2560f })
            Check(!MobileQaFriendlyLayout.ShouldStackResourceHud(width),
                "El HUD no debe comprimir el contenido para ancho " + width + ".",
                failures);

        Check(CountNamed(scene, "MobileSafeAreaRoot") == 1,
            "MobileSafeAreaRoot está ausente o duplicado.", failures);
        Finish(failures);
    }

    private static void ValidateNavigationButtons(
        RectTransform container, List<string> failures)
    {
        Check(container != null, "Falta NavButtonsContainer.", failures);
        if (container == null)
            return;
        MobileQaFriendlyLayout layout =
            container.GetComponentInParent<MobileQaFriendlyLayout>();
        float maximumHeight = layout != null && layout.forcePortraitLayout
            ? 72f : 34f;
        for (int index = 0; index < container.childCount; index++)
        {
            RectTransform child = container.GetChild(index) as RectTransform;
            Check(child != null && child.sizeDelta.y <= maximumHeight,
                container.GetChild(index).name +
                " dejó de ser compacto.", failures);
        }
    }

    private static bool HasNavigationClearance(
        RectTransform content, MobileQaFriendlyLayout legacyLayout)
    {
        if (content == null)
            return false;
        VerticalSafeAreaLayout vertical =
            content.GetComponentInParent<VerticalSafeAreaLayout>();
        if (vertical != null && vertical.contentSlot != null &&
            content.parent == vertical.contentSlot)
            return true;
        return content.offsetMin.y >=
            (legacyLayout.forcePortraitLayout ? 170f : 90f);
    }

    private static int CountNamed(Scene scene, string name)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            {
                if (current.name == name)
                    count++;
            }
        }
        return count;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[Mobile QA Friendly Layout] PASS | 1000/1920/2400/2560 " +
                "| Safe Area | accesos | recursos | navegación táctil");
            return;
        }
        foreach (string failure in failures)
            Debug.LogError("[Mobile QA Friendly Layout] " + failure);
        throw new InvalidOperationException(
            "Mobile QA Friendly Layout falló con " + failures.Count + " error(es).");
    }
}
#endif
