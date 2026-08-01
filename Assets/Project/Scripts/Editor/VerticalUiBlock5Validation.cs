#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock5Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    private static readonly string[] CanonicalIds =
    {
        "emission_focus",
        "containment_tuning",
        "tetraquark_stabilization",
        "triangle_unlock_1",
        "triangle_impulse_tuning",
        "triangle_synergy_resonance",
        "triangle_persistence_anchor"
    };

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Block 5 Upgrades")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var failures = new List<string>();
        GameObject panel = FindNamed(scene, "Panel_Mejoras");
        GameObject f2Panel = FindNamed(scene, "F2Panel");
        GameObject shell = FindNamed(scene, "VerticalUpgradesShell");
        GameObject legacy = FindNamed(scene, "LegacyF2Layout");

        Check(panel != null, "Falta Panel_Mejoras.", failures);
        Check(f2Panel != null && panel != null &&
            f2Panel.transform.IsChildOf(panel.transform),
            "F2Panel no esta dentro de Panel_Mejoras.", failures);
        Check(shell != null && f2Panel != null &&
            shell.transform.parent == f2Panel.transform,
            "Falta el shell vertical independiente de Mejoras.", failures);
        Check(CountNamed(scene, "VerticalUpgradesShell") == 1,
            "VerticalUpgradesShell esta duplicado.", failures);
        Check(legacy != null && !legacy.activeSelf,
            "El layout F2 heredado no esta preservado e inactivo.", failures);

        if (shell != null)
        {
            ValidateTitle(shell.transform, failures);
            ValidateScroll(shell.transform, failures);
            ValidateSections(shell.transform, failures);
            ValidateRows(shell.transform, failures);
        }
        ValidateController(f2Panel, failures);
        ValidateNavigation(panel, failures);
        ValidateNoRowsInGeneration(scene, failures);
        ValidateRetiredRows(shell, legacy, scene, failures);
        ValidateLocalization(failures);
        Finish(failures);
    }

    private static void ValidateTitle(Transform shell, List<string> failures)
    {
        Transform title = FindDirectChild(shell, "Title");
        LocalizedTMP localized = title != null
            ? title.GetComponent<LocalizedTMP>()
            : null;
        Check(title != null && title.GetComponent<TextMeshProUGUI>() != null,
            "Falta el titulo MEJORAS.", failures);
        Check(localized != null && localized.key == "upgrades.title",
            "El titulo de Mejoras no esta localizado.", failures);
    }

    private static void ValidateScroll(Transform shell, List<string> failures)
    {
        Transform scrollTransform = FindDirectChild(shell, "UpgradesScroll");
        ScrollRect scroll = scrollTransform != null
            ? scrollTransform.GetComponent<ScrollRect>()
            : null;
        Check(scroll != null && scroll.vertical && !scroll.horizontal,
            "Mejoras no usa un ScrollRect vertical.", failures);
        Check(scroll != null && scroll.content != null && scroll.viewport != null,
            "El ScrollRect de Mejoras no tiene viewport/content.", failures);
        if (scroll != null && scroll.content != null)
        {
            Check(scroll.content.GetComponent<VerticalLayoutGroup>() != null,
                "El contenido de Mejoras no es modular.", failures);
            Check(scroll.content.GetComponent<ContentSizeFitter>() != null,
                "El contenido de Mejoras no crece verticalmente.", failures);
        }
    }

    private static void ValidateSections(Transform shell, List<string> failures)
    {
        Transform scroll = FindDirectChild(shell, "UpgradesScroll");
        Transform viewport = scroll != null ? FindDirectChild(scroll, "Viewport") : null;
        Transform content = viewport != null ? FindDirectChild(viewport, "Content") : null;
        string[] names = { "Section_Production", "Section_Traces", "Section_Triangle" };
        string[] keys =
        {
            "upgrades.section.production",
            "upgrades.section.traces",
            "upgrades.section.triangle"
        };
        for (int i = 0; i < names.Length; i++)
        {
            Transform section = content != null ? FindDirectChild(content, names[i]) : null;
            Check(section != null, "Falta " + names[i] + ".", failures);
            if (section == null) continue;
            Check(section.GetSiblingIndex() == i,
                names[i] + " no respeta el orden aprobado.", failures);
            Check(section.GetComponent<CanvasGroup>() != null &&
                section.GetComponent<LayoutElement>() != null,
                names[i] + " no admite ocultacion progresiva sin huecos.", failures);
            Transform header = FindDirectChild(section, "Header");
            Transform label = header != null ? FindDirectChild(header, "Label") : null;
            LocalizedTMP localized = label != null
                ? label.GetComponent<LocalizedTMP>()
                : null;
            Check(localized != null && localized.key == keys[i],
                names[i] + " no tiene encabezado localizado.", failures);
            Transform rows = FindDirectChild(section, "Rows");
            Check(rows != null && rows.GetComponent<VerticalLayoutGroup>() != null &&
                rows.GetComponent<ContentSizeFitter>() != null,
                names[i] + " no contiene una lista extensible.", failures);
        }
    }

    private static void ValidateRows(Transform shell, List<string> failures)
    {
        F2UpgradeRowUI[] rows = shell.GetComponentsInChildren<F2UpgradeRowUI>(true);
        Check(rows.Length == CanonicalIds.Length,
            "El shell no contiene exactamente las siete mejoras canonicas.", failures);
        foreach (string id in CanonicalIds)
        {
            F2UpgradeRowUI[] matches = rows.Where(row => row.UpgradeId == id).ToArray();
            Check(matches.Length == 1,
                "La fila " + id + " falta o esta duplicada.", failures);
            if (matches.Length != 1) continue;
            F2UpgradeRowUI row = matches[0];
            Check(row.TitleText != null && row.TierText != null &&
                row.CostText != null && row.DescriptionText != null &&
                row.BuyButton != null,
                id + " tiene referencias visuales rotas.", failures);
            LayoutElement layout = row.GetComponent<LayoutElement>();
            Check(layout != null && layout.preferredHeight >= 170f,
                id + " no tiene altura tactil suficiente.", failures);
            if (row.BuyButton != null)
            {
                RectTransform buttonRect = row.BuyButton.GetComponent<RectTransform>();
                Check(buttonRect != null && buttonRect.sizeDelta.x >= 200f &&
                    buttonRect.sizeDelta.y >= 64f,
                    id + " tiene un boton demasiado pequeno.", failures);
                Check(row.BuyButton.onClick.GetPersistentEventCount() == 0,
                    id + " conserva listeners serializados que pueden duplicar compras.",
                    failures);
            }
        }

        string[] production = { "emission_focus", "containment_tuning" };
        string[] traces = { "tetraquark_stabilization" };
        string[] triangle =
        {
            "triangle_unlock_1", "triangle_impulse_tuning",
            "triangle_synergy_resonance", "triangle_persistence_anchor"
        };
        ValidateGroup(shell, "Section_Production", production, failures);
        ValidateGroup(shell, "Section_Traces", traces, failures);
        ValidateGroup(shell, "Section_Triangle", triangle, failures);
    }

    private static void ValidateGroup(
        Transform shell,
        string sectionName,
        string[] expected,
        List<string> failures)
    {
        Transform section = shell.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(current => current.name == sectionName);
        Transform rowsRoot = section != null ? FindDirectChild(section, "Rows") : null;
        if (rowsRoot == null) return;
        F2UpgradeRowUI[] rows = rowsRoot.GetComponentsInChildren<F2UpgradeRowUI>(true);
        Check(rows.Select(row => row.UpgradeId).SequenceEqual(expected),
            sectionName + " no contiene las mejoras aprobadas en orden.", failures);
    }

    private static void ValidateController(GameObject f2Panel, List<string> failures)
    {
        VerticalUpgradesScreenUI screen = f2Panel != null
            ? f2Panel.GetComponent<VerticalUpgradesScreenUI>()
            : null;
        Check(screen != null, "Falta VerticalUpgradesScreenUI.", failures);
        if (screen == null) return;
        Check(screen.content != null && screen.productionSection != null &&
            screen.tracesSection != null && screen.triangleSection != null,
            "VerticalUpgradesScreenUI tiene secciones sin conectar.", failures);
        Check(screen.rows != null && screen.rows.Length == CanonicalIds.Length &&
            screen.rows.All(row => row != null),
            "VerticalUpgradesScreenUI no controla las siete filas.", failures);
    }

    private static void ValidateNavigation(GameObject panel, List<string> failures)
    {
        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        Check(tabs != null && tabs.panelMejoras == panel,
            "TabsUI no abre la pantalla vertical de Mejoras.", failures);
    }

    private static void ValidateNoRowsInGeneration(
        Scene scene,
        List<string> failures)
    {
        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        if (tabs?.panelGeneracion == null) return;
        Check(tabs.panelGeneracion.GetComponentsInChildren<F2UpgradeRowUI>(true).Length == 0,
            "Hay filas F2 dentro de Generacion.", failures);
    }

    private static void ValidateRetiredRows(
        GameObject shell,
        GameObject legacy,
        Scene scene,
        List<string> failures)
    {
        var canonical = new HashSet<string>(CanonicalIds);
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (F2UpgradeRowUI row in root.GetComponentsInChildren<F2UpgradeRowUI>(true))
            {
                if (canonical.Contains(row.UpgradeId)) continue;
                Check(shell == null || !row.transform.IsChildOf(shell.transform),
                    "Una mejora retirada aparece en el shell vertical.", failures);
                Check(legacy != null && row.transform.IsChildOf(legacy.transform),
                    "Una mejora retirada no quedo aislada en el respaldo heredado.",
                    failures);
            }
        }
    }

    private static void ValidateLocalization(List<string> failures)
    {
        string[] paths =
        {
            "Assets/Project/Resources/Localization/lang_es.json",
            "Assets/Project/Resources/Localization/lang_en.json"
        };
        string[] keys =
        {
            "upgrades.title",
            "upgrades.section.production",
            "upgrades.section.traces",
            "upgrades.section.triangle"
        };
        foreach (string path in paths)
        {
            string text = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
            foreach (string key in keys)
                Check(text.Contains("\"" + key + "\""),
                    path + " no contiene " + key + ".", failures);
        }
    }

    private static GameObject FindNamed(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    return current.gameObject;
        return null;
    }

    private static int CountNamed(Scene scene, string name)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    count++;
        return count;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == name)
                return parent.GetChild(i);
        return null;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[Vertical UI Block 5] PASS | independent upgrades | " +
                "three progressive sections | seven canonical rows | localized | no duplicate listeners");
            return;
        }
        foreach (string failure in failures)
            Debug.LogError("[Vertical UI Block 5] " + failure);
        throw new InvalidOperationException(
            "Vertical UI Block 5 fallo con " + failures.Count + " error(es).");
    }
}
#endif
