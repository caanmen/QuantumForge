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
    private const string EarlyRowPrefabPath =
        "Assets/Project/UI/Vertical/Generated/VerticalBuildingRow.prefab";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Generation Visual Polish")]
    public static void Validate()
    {
        VerticalUiFinalValidation.Validate();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var failures = new List<string>();
        VerticalGenerationPolishUI[] polish = FindAll<VerticalGenerationPolishUI>(scene);
        Check(polish.Length == 1,
            "Debe existir exactamente un VerticalGenerationPolishUI.", failures);

        GameObject header = FindNamed(scene, "VerticalGenerationHeader");
        GameObject beforeRoot = FindNamed(scene, "GenerationBeforeTriangleRoot");
        GameObject root = FindNamed(scene, "GenerationTriangleRoot");
        GameObject focus = FindNamed(scene, "TriangleFocus");
        VerticalSafeAreaLayout safe = FindAll<VerticalSafeAreaLayout>(scene).FirstOrDefault();
        Check(header != null && beforeRoot != null && root != null && focus != null,
            "Falta la estructura principal de Generación pulida.", failures);

        if (header != null)
        {
            RectTransform rect = (RectTransform)header.transform;
            Check(Near(rect.rect.height, 92f, 1f) && rect.sizeDelta.x <= -240f,
                "La cabecera no conserva la proporción compacta objetivo.", failures);
            TextMeshProUGUI[] values =
                header.GetComponentsInChildren<TextMeshProUGUI>(true);
            Check(header.transform.Find("Resource_LE") != null &&
                header.transform.Find("Resource_Traces") != null &&
                header.transform.Find("Resource_Energy") != null,
                "La cabecera debe mostrar LE, Trazas y Energía.", failures);
            HUD hud = FindAll<HUD>(scene).FirstOrDefault();
            Check(hud != null && hud.leText != null && hud.tracesText != null &&
                hud.energyText != null,
                "El HUD no tiene enlazados los tres recursos.", failures);
            Check(values.Where(value => value.name == "Value")
                .All(value => value.fontSize >= 29f && value.fontSizeMin >= 22f),
                "Los contadores de recursos son demasiado pequenos para movil.", failures);
        }
        Check(safe != null && Near(safe.headerHeight, 0f, 0.1f),
            "HeaderSlot todavia reserva espacio duplicado sobre Generacion.", failures);
        if (root != null)
        {
            RectTransform rect = (RectTransform)root.transform;
            Check(Near(rect.offsetMin.x, 16f, 1f) &&
                Near(rect.offsetMax.x, -16f, 1f),
                "La columna avanzada no conserva el ancho central objetivo.", failures);
            Transform content = root.transform.Find("TriangleScroll/Viewport/Content");
            Check(content != null && content.Find("GenerationTitleFrame") != null,
                "Falta el marco tecnológico del título.", failures);
            Check(content != null && content.Find("CircuitSelectors") != null &&
                content.Find("TriangleArtifactCards") != null,
                "Selectores o artefactos dejaron de conservar la jerarquía validada.", failures);
            if (content is RectTransform contentRect)
                Check(Near(contentRect.sizeDelta.y, 1488f, 1f),
                    "El contenido avanzado no conserva el margen superior y el cierre inferior.", failures);
            Check(content != null && content.Find("TriangleObservatory") == null &&
                content.Find("TrianglePurchasesFrame") != null,
                "La pantalla principal conserva el Observatorio antiguo o perdió Compras.", failures);
            RectTransform purchases = content?.Find("TrianglePurchasesFrame") as RectTransform;
            Check(purchases != null && Near(purchases.rect.height, 508f, 1f),
                "Compras no aprovecha el espacio inferior objetivo.", failures);
            VerticalLayoutGroup purchaseLayout = content?.Find("TriangleArtifactCards")
                ?.GetComponent<VerticalLayoutGroup>();
            Check(purchaseLayout != null && purchaseLayout.childControlWidth &&
                purchaseLayout.childForceExpandWidth,
                "Las filas de compras no ocupan todo el ancho disponible.", failures);
        }

        if (beforeRoot != null)
        {
            RectTransform rect = (RectTransform)beforeRoot.transform;
            Check(Near(rect.offsetMin.x, 16f, 1f) &&
                Near(rect.offsetMax.x, -16f, 1f),
                "La columna anterior al Triangulo no coincide con la avanzada.", failures);
            Transform frame = beforeRoot.transform.Find("ArtifactsFrame");
            Check(beforeRoot.transform.Find("EarlyGenerationTitleFrame") != null &&
                frame != null && frame.Find("EarlyTechnologyGrid") != null &&
                frame.Find("EarlyInnerFrame") != null &&
                frame.Find("EarlySectionRail") != null,
                "Faltan capas tecnologicas anteriores al Triangulo.", failures);
            BuildingListUI list = beforeRoot.GetComponentInChildren<BuildingListUI>(true);
            Check(list != null && list.buildingRowPrefab != null &&
                AssetDatabase.GetAssetPath(list.buildingRowPrefab) == EarlyRowPrefabPath,
                "La lista temprana no conserva VerticalBuildingRow.prefab.", failures);
        }

        GameObject earlyRowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            EarlyRowPrefabPath);
        BuildingRowUI earlyRow = earlyRowPrefab != null
            ? earlyRowPrefab.GetComponent<BuildingRowUI>()
            : null;
        Check(earlyRow != null && earlyRow.higgsIcon != null &&
            earlyRow.tetraIcon != null && earlyRow.modulatorIcon != null &&
            earlyRow.higgsIcon.name == "qf_node_higgs" &&
            earlyRow.tetraIcon.name == "qf_node_tetraquark" &&
            earlyRow.modulatorIcon.name == "qf_node_modulator",
            "Las filas tempranas no usan los nodos mecanicos definitivos.", failures);
        if (earlyRowPrefab != null)
        {
            LayoutElement earlyLayout = earlyRowPrefab.GetComponent<LayoutElement>();
            Transform earlyBuy = earlyRowPrefab.transform.Find("BuyButton");
            LayoutElement earlyBuyLayout = earlyBuy != null
                ? earlyBuy.GetComponent<LayoutElement>()
                : null;
            Check(earlyRowPrefab.transform.Find("EarlyCardInnerFrame") != null &&
                earlyRowPrefab.transform.Find("AccentBar") != null &&
                earlyRowPrefab.transform.Find("ArtifactIcon/ArtifactGlow") != null,
                "La fila temprana carece de profundidad visual mecanica.", failures);
            Check(earlyLayout != null && earlyLayout.preferredHeight >= 190f,
                "La fila temprana perdio su altura tactil.", failures);
            Check(earlyBuyLayout != null && earlyBuyLayout.minWidth >= 120f &&
                earlyBuyLayout.minHeight >= 50f,
                "La compra temprana perdio su minimo tactil.", failures);
            TextMeshProUGUI earlyStats = earlyRowPrefab.transform.Find("Stats")
                ?.GetComponent<TextMeshProUGUI>();
            Check(earlyStats != null && earlyStats.fontSize >= 22f &&
                earlyStats.fontSizeMin >= 17f,
                "Los costes y tasas tempranos son demasiado pequenos para movil.", failures);
        }

        string[] requiredNames =
        {
            "EnergyGauge", "GaugeCircuit", "GaugeProgress", "GaugeProgressRing",
            "TechnologyGrid", "FocusInnerFrame",
            "BeamGlow", "BeamCore", "BeamHotCore", "BeamSegments", "BeamChevrons",
            "BeamStartConnector", "BeamEndConnector", "EnergyFlow",
            "StateBorder", "CircuitIcon", "AccentBar"
        };
        foreach (string name in requiredNames)
            Check(CountNamed(scene, name) > 0,
                "Falta la capa visual " + name + ".", failures);

        if (focus != null)
        {
            RectTransform rect = (RectTransform)focus.transform;
            Check(Near(rect.rect.height, 1348f, 1f),
                "TriangleFocus no aprovecha la altura objetivo.", failures);
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
                Near(higgsRect.anchoredPosition.y, 515f, 1f) &&
                Near(tetraRect.anchoredPosition.y, 515f, 1f),
                "Los nodos superiores no tienen la altura del mockup.", failures);
            Check(gaugeRect != null && Near(gaugeRect.anchoredPosition.y, 330f, 1f),
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
            Check(row != null && row.preferredHeight >= 130f &&
                row.preferredHeight <= 136f,
                card.name + " no tiene la nueva altura táctil.", failures);
            Check(buy != null && buy.minWidth >= 170f && buy.minHeight >= 70f,
                card.name + " perdió el tamaño táctil mínimo.", failures);
            Check(card.stateText != null && card.stateText.fontSize >= 22f &&
                card.stateText.fontSizeMin >= 18f,
                card.name + " tiene costes demasiado pequenos para movil.", failures);
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
            PolishFolder + "/qf_beam_chevrons.png",
            PolishFolder + "/qf_selector_frame.png"
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
