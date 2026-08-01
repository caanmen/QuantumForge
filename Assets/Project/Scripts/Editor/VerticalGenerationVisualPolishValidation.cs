#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalGenerationVisualPolishValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string PolishFolder =
        "Assets/Project/UI/Vertical/GenerationPolish";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Generation Visual Polish")]
    public static void Validate()
    {
        VerticalUiBlock4Validation.Validate();
        VerticalUiFinalValidation.Validate();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var failures = new List<string>();
        VerticalGenerationPolishUI[] polish = FindAll<VerticalGenerationPolishUI>(scene);
        Check(polish.Length == 1,
            "Debe existir exactamente un VerticalGenerationPolishUI.", failures);

        GameObject header = FindNamed(scene, "VerticalGenerationHeader");
        GameObject root = FindNamed(scene, "GenerationTriangleRoot");
        GameObject focus = FindNamed(scene, "TriangleFocus");
        VerticalSafeAreaLayout safe = FindAll<VerticalSafeAreaLayout>(scene).FirstOrDefault();
        Check(header != null && root != null && focus != null,
            "Falta la estructura principal de Generación pulida.", failures);

        if (header != null)
        {
            RectTransform rect = (RectTransform)header.transform;
            Check(Near(rect.rect.height, 92f, 1f) && rect.sizeDelta.x <= -240f,
                "La cabecera no conserva la proporción compacta objetivo.", failures);
        }
        Check(safe != null && Near(safe.headerHeight, 0f, 0.1f),
            "HeaderSlot todavia reserva espacio duplicado sobre Generacion.", failures);
        if (root != null)
        {
            RectTransform rect = (RectTransform)root.transform;
            Check(Near(rect.offsetMin.x, 96f, 1f) &&
                Near(rect.offsetMax.x, -96f, 1f),
                "La columna avanzada no conserva el ancho central objetivo.", failures);
            Transform content = root.transform.Find("TriangleScroll/Viewport/Content");
            Check(content != null && content.Find("GenerationTitleFrame") != null,
                "Falta el marco tecnológico del título.", failures);
            Check(content != null && content.Find("CircuitSelectors") != null &&
                content.Find("TriangleArtifactCards") != null,
                "Selectores o artefactos dejaron de conservar la jerarquía validada.", failures);
        }

        string[] requiredNames =
        {
            "EnergyGauge", "GaugeCircuit", "GaugeProgress", "GaugeProgressRing",
            "TechnologyGrid", "FocusInnerFrame",
            "BeamGlow", "BeamCore", "BeamSegments", "BeamChevrons", "EnergyFlow",
            "StateBorder", "CircuitIcon", "AccentBar"
        };
        foreach (string name in requiredNames)
            Check(CountNamed(scene, name) > 0,
                "Falta la capa visual " + name + ".", failures);

        if (focus != null)
        {
            RectTransform rect = (RectTransform)focus.transform;
            Check(Near(rect.rect.height, 1090f, 1f),
                "TriangleFocus no conserva la altura compacta objetivo.", failures);
            Image[] nodeImages = new[]
            {
                FindImage(focus.transform, "Vertex_Higgs/Icon"),
                FindImage(focus.transform, "Vertex_Tetra/Icon"),
                FindImage(focus.transform, "Vertex_Modulator/Icon")
            };
            Check(nodeImages.All(image => image != null && image.sprite != null &&
                image.sprite.name.StartsWith("qf_node_", StringComparison.Ordinal)),
                "Los vértices no usan los nodos mecánicos definitivos.", failures);
            RectTransform higgsRect =
                focus.transform.Find("Vertex_Higgs") as RectTransform;
            RectTransform tetraRect =
                focus.transform.Find("Vertex_Tetra") as RectTransform;
            RectTransform gaugeRect =
                focus.transform.Find("EnergyGauge") as RectTransform;
            Check(higgsRect != null && tetraRect != null &&
                Near(higgsRect.anchoredPosition.y, 400f, 1f) &&
                Near(tetraRect.anchoredPosition.y, 400f, 1f),
                "Los nodos superiores no tienen la altura del mockup.", failures);
            Check(gaugeRect != null && Near(gaugeRect.anchoredPosition.y, 265f, 1f),
                "El medidor central no tiene la altura del mockup.", failures);
            string[] corners = { "Corner_TL", "Corner_TR", "Corner_BL", "Corner_BR" };
            Check(corners.All(name => focus.transform.Find(name) == null ||
                !focus.transform.Find(name).gameObject.activeSelf),
                "Persisten reticulas grandes en las esquinas del panel.", failures);
        }

        VerticalTriangleArtifactCardUI[] cards =
            FindAll<VerticalTriangleArtifactCardUI>(scene);
        Check(cards.Length == 3, "Deben conservarse tres filas de artefactos.", failures);
        foreach (VerticalTriangleArtifactCardUI card in cards)
        {
            LayoutElement row = card.GetComponent<LayoutElement>();
            LayoutElement buy = card.buyButton != null
                ? card.buyButton.GetComponent<LayoutElement>()
                : null;
            Check(row != null && row.preferredHeight <= 90f,
                card.name + " no es una fila compacta.", failures);
            Check(buy != null && buy.minWidth >= 120f && buy.minHeight >= 50f,
                card.name + " perdió el tamaño táctil mínimo.", failures);
        }

        string[] spritePaths =
        {
            PolishFolder + "/qf_node_higgs.png",
            PolishFolder + "/qf_node_tetraquark.png",
            PolishFolder + "/qf_node_modulator.png",
            PolishFolder + "/qf_energy_gauge.png",
            PolishFolder + "/qf_gauge_progress_ring.png",
            PolishFolder + "/qf_circuit_energy.png",
            PolishFolder + "/qf_circuit_experimental.png",
            PolishFolder + "/qf_circuit_phase.png",
            PolishFolder + "/qf_resource_le.png",
            PolishFolder + "/qf_resource_traces.png",
            PolishFolder + "/qf_beam_chevrons.png"
        };
        foreach (string path in spritePaths)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            TextureImporterPlatformSettings android = importer != null
                ? importer.GetPlatformTextureSettings("Android")
                : null;
            Check(sprite != null && importer != null &&
                importer.textureType == TextureImporterType.Sprite &&
                importer.alphaIsTransparency && !importer.mipmapEnabled &&
                android != null && android.overridden &&
                android.format == TextureImporterFormat.ASTC_4x4,
                path + " no tiene importación UI/Android correcta.", failures);
        }

        int missing = 0;
        foreach (GameObject sceneRoot in scene.GetRootGameObjects())
            foreach (Transform current in sceneRoot.GetComponentsInChildren<Transform>(true))
                missing += current.GetComponents<Component>().Count(component => component == null);
        Check(missing == 0, "La escena contiene Missing Scripts: " + missing + ".", failures);

        if (failures.Count == 0)
        {
            Debug.Log("[Generation Visual Polish Validation] PASS | composición compacta | " +
                "nodos mecánicos | medidor | haces | filas | táctil | responsive base");
            return;
        }
        foreach (string failure in failures)
            Debug.LogError("[Generation Visual Polish Validation] " + failure);
        throw new InvalidOperationException(
            "Generation Visual Polish falló con " + failures.Count + " error(es).");
    }

    private static Image FindImage(Transform root, string path)
    {
        return root.Find(path)?.GetComponent<Image>();
    }

    private static T[] FindAll<T>(Scene scene) where T : Component
    {
        var result = new List<T>();
        foreach (GameObject root in scene.GetRootGameObjects())
            result.AddRange(root.GetComponentsInChildren<T>(true));
        return result.ToArray();
    }

    private static GameObject FindNamed(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name) return current.gameObject;
        return null;
    }

    private static int CountNamed(Scene scene, string name)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name) count++;
        return count;
    }

    private static bool Near(float actual, float expected, float tolerance)
    {
        return Mathf.Abs(actual - expected) <= tolerance;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif
