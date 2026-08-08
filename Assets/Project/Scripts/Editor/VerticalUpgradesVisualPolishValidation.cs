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

public static class VerticalUpgradesVisualPolishValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string PolishFolder =
        "Assets/Project/UI/Vertical/UpgradesPolish";

    private static readonly string[] CanonicalIds =
    {
        "emission_focus",
        "containment_tuning",
        "tetraquark_stabilization",
        "triangle_unlock_1",
        "triangle_impulse_tuning",
        "triangle_synergy_resonance",
        "triangle_persistence_anchor",
        "triangle_energy_efficiency"
    };

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Upgrades Visual Polish")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var failures = new List<string>();
        GameObject panel = FindNamed(scene, "Panel_Mejoras");
        GameObject header = FindNamed(scene, "VerticalUpgradesHeader");
        GameObject generationHeader = FindNamed(scene, "VerticalGenerationHeader");
        GameObject shell = FindNamed(scene, "VerticalUpgradesShell");
        VerticalUpgradesPolishUI polish =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesPolishUI>(
                FindObjectsInactive.Include);
        VerticalUpgradesScreenUI screen =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesScreenUI>(
                FindObjectsInactive.Include);
        KeycardPurchaseUI keycard =
            UnityEngine.Object.FindFirstObjectByType<KeycardPurchaseUI>(
                FindObjectsInactive.Include);

        Check(panel != null && header != null && shell != null && polish != null,
            "La jerarquia de pulido de Mejoras esta incompleta.", failures);
        Check(screen != null && keycard != null && screen.keycardRow == keycard,
            "La Keycard no esta conectada a la pantalla vertical activa.", failures);
        Check(CountNamed(scene, "VerticalUpgradesHeader") == 1 &&
            FindAll<VerticalUpgradesPolishUI>(scene).Length == 1,
            "La cabecera o el controlador visual de Mejoras esta duplicado.", failures);
        Check(header != null && panel != null && header.transform.parent == panel.transform,
            "La cabecera de Mejoras no vive directamente en Panel_Mejoras.", failures);

        if (header != null)
        {
            RectTransform rect = (RectTransform)header.transform;
            Check(Near(rect.anchoredPosition.y, -34f, 0.1f) &&
                Near(rect.sizeDelta.y, 92f, 0.1f) &&
                Near(rect.sizeDelta.x, -250f, 0.1f),
                "La cabecera de Mejoras no coincide con la de Generacion.", failures);
            Check(header.transform.Find("UpgradeResource_LE/Icon") != null &&
                header.transform.Find("UpgradeResource_LE/Value") != null &&
                header.transform.Find("UpgradeResource_Traces/Icon") != null &&
                header.transform.Find("UpgradeResource_Traces/Value") != null &&
                header.transform.Find("UpgradeResource_Energy/Icon") != null &&
                header.transform.Find("UpgradeResource_Energy/Value") != null,
                "La cabecera no contiene los tres contadores mecánicos.", failures);
            TextMeshProUGUI[] values =
                header.GetComponentsInChildren<TextMeshProUGUI>(true);
            Check(values.Where(value => value.name == "Value")
                .All(value => value.fontSize >= 29f && value.fontSizeMin >= 22f),
                "Los contadores de Mejoras son demasiado pequenos para movil.", failures);
        }

        if (polish != null)
        {
            Check(polish.theme != null && polish.leText != null &&
                polish.tracesText != null && polish.energyText != null &&
                polish.sourceLeText != null && polish.sourceTracesText != null &&
                polish.sourceEnergyText != null,
                "VerticalUpgradesPolishUI tiene referencias de cabecera rotas.", failures);
            Check(generationHeader != null && polish.sourceLeText != null &&
                polish.sourceLeText.transform.IsChildOf(generationHeader.transform) &&
                polish.sourceTracesText.transform.IsChildOf(generationHeader.transform),
                "Los contadores de Mejoras no reflejan el HUD real de Generacion.", failures);
            Check(polish.sections != null && polish.sections.Length == 3 &&
                polish.sections.All(item => item != null && item.railGlow != null &&
                    item.railCore != null && item.iconGlow != null && item.icon != null),
                "Las tres secciones no tienen rail e icono mecanico conectados.", failures);
            Check(polish.rows != null && polish.rows.Length == CanonicalIds.Length &&
                polish.rows.All(item => item != null && item.row != null &&
                    item.frame != null && item.innerFrame != null &&
                    item.icon != null && item.buttonFrame != null),
                "Las ocho filas no estan conectadas al controlador visual.", failures);
        }

        if (shell != null)
        {
            RectTransform rect = (RectTransform)shell.transform;
            Check(Near(rect.offsetMin.x, 96f, 0.1f) &&
                Near(rect.offsetMax.x, -96f, 0.1f) &&
                Near(rect.offsetMax.y, -148f, 0.1f),
                "El shell no reserva cabecera ni margenes como Generacion.", failures);
            Check(shell.transform.Find("UpgradesTitleFrame/AccentLeft") != null &&
                shell.transform.Find("UpgradesTitleFrame/AccentRight") != null,
                "Falta la barra mecanica del titulo MEJORAS.", failures);
            ValidateSections(shell.transform, failures);
            ValidateRows(shell.transform, failures);
            ValidateKeycard(shell.transform, keycard, failures);
        }

        ValidateAssets(failures);
        ValidateNoGameplayDuplication(scene, failures);
        Finish(failures);
    }

    private static void ValidateSections(Transform shell, List<string> failures)
    {
        string[] names =
        {
            "Section_Production", "Section_Traces", "Section_Triangle"
        };
        foreach (string name in names)
        {
            Transform section = FindDescendant(shell, name);
            Check(section != null, "Falta " + name + ".", failures);
            if (section == null) continue;
            Transform header = section.Find("Header");
            Check(header != null && header.Find("MechanicalIcon") != null &&
                header.Find("IconGlow") != null && header.Find("RailBase") != null &&
                header.Find("RailGlow") != null && header.Find("RailCore") != null &&
                header.Find("RailSegments") != null && header.Find("RailCap") != null,
                name + " no conserva el encabezado tecnologico aprobado.", failures);
            LayoutElement layout = header != null
                ? header.GetComponent<LayoutElement>()
                : null;
            Check(layout != null && layout.preferredHeight >= 104f,
                name + " tiene un encabezado demasiado pequeno.", failures);
            Check(CountDescendants(section, "MechanicalIcon") >= 1 &&
                CountDescendants(section, "RailCore") == 1,
                name + " tiene iconos o railes duplicados.", failures);
        }
    }

    private static void ValidateRows(Transform shell, List<string> failures)
    {
        F2UpgradeRowUI[] rows = shell.GetComponentsInChildren<F2UpgradeRowUI>(true);
        foreach (string id in CanonicalIds)
        {
            F2UpgradeRowUI[] matches = rows.Where(row => row.UpgradeId == id).ToArray();
            Check(matches.Length == 1, id + " falta o esta duplicada.", failures);
            if (matches.Length != 1) continue;
            F2UpgradeRowUI row = matches[0];
            LayoutElement layout = row.GetComponent<LayoutElement>();
            RectTransform button = row.BuyButton != null
                ? (RectTransform)row.BuyButton.transform
                : null;
            Check(layout != null && layout.preferredHeight >= 170f &&
                layout.preferredHeight <= 184f,
                id + " no usa la altura compacta aprobada.", failures);
            Check(button != null && button.sizeDelta.x >= 200f &&
                button.sizeDelta.y >= 68f,
                id + " no conserva un boton hexagonal tactil.", failures);
            Check(row.transform.Find("RowInnerFrame") != null &&
                row.transform.Find("MechanicalIcon") != null &&
                row.transform.Find("IconGlow") != null &&
                row.transform.Find("IconDivider") != null,
                id + " no tiene modulo e icono mecanico completos.", failures);
            Check(row.BuyButton == null ||
                row.BuyButton.onClick.GetPersistentEventCount() == 0,
                id + " agrego un listener serializado de compra.", failures);
            Check(row.CostText != null && row.CostText.fontSize >= 23f &&
                row.CostText.fontSizeMin >= 18f && row.TierText != null &&
                row.TierText.fontSize >= 22f && row.TierText.fontSizeMin >= 17f,
                id + " conserva numeros demasiado pequenos para movil.", failures);
        }

        F2UpgradeRowUI unlock = rows.FirstOrDefault(
            row => row.UpgradeId == "triangle_unlock_1");
        Check(unlock != null && unlock.transform.Find("IconSymbol") != null &&
            unlock.transform.Find("IconSymbol").gameObject.activeSelf,
            "Acople de Vertices no muestra el simbolo triangular.", failures);
    }

    private static void ValidateKeycard(
        Transform shell,
        KeycardPurchaseUI keycard,
        List<string> failures)
    {
        Transform triangleSection = FindDescendant(shell, "Section_Triangle");
        Check(keycard != null && triangleSection != null &&
            keycard.transform.parent == triangleSection,
            "La Keycard sigue fuera de la seccion activa del Triangulo.", failures);
        if (keycard == null) return;

        LayoutElement layout = keycard.GetComponent<LayoutElement>();
        RectTransform button = keycard.buyButton != null
            ? (RectTransform)keycard.buyButton.transform
            : null;
        Check(layout != null && layout.preferredHeight >= 196f,
            "La Keycard no tiene altura util en la lista vertical.", failures);
        Check(button != null && button.sizeDelta.x >= 200f &&
            button.sizeDelta.y >= 68f,
            "La Keycard no conserva un boton tactil legible.", failures);
        Check(keycard.transform.Find("KeycardInnerFrame") != null &&
            keycard.transform.Find("KeycardAccent") != null,
            "La Keycard no usa el modulo visual vertical.", failures);
        Check(keycard.costText != null && keycard.costText.fontSize >= 22f &&
            keycard.costText.fontSizeMin >= 17f,
            "El coste de la Keycard es demasiado pequeno para movil.", failures);
        Check(keycard.buyButton != null &&
            keycard.buyButton.onClick.GetPersistentEventCount() == 1 &&
            keycard.buyButton.onClick.GetPersistentTarget(0) == keycard &&
            keycard.buyButton.onClick.GetPersistentMethodName(0) ==
                nameof(KeycardPurchaseUI.OnClickBuyKeycard),
            "El boton de Keycard perdio su accion funcional.", failures);
    }

    private static void ValidateAssets(List<string> failures)
    {
        foreach (string file in new[]
        {
            "qf_upgrade_module_frame.png", "qf_upgrade_button_frame.png"
        })
        {
            string path = PolishFolder + "/" + file;
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            TextureImporterPlatformSettings android = importer != null
                ? importer.GetPlatformTextureSettings("Android")
                : default;
            Check(sprite != null && importer != null &&
                importer.textureType == TextureImporterType.Sprite &&
                importer.spriteBorder.x > 0f && !importer.mipmapEnabled,
                path + " no esta importado como sprite 9-slice movil.", failures);
            Check(importer != null && android.overridden &&
                android.format == TextureImporterFormat.ASTC_4x4,
                path + " no conserva compresion Android ASTC.", failures);
        }
    }

    private static void ValidateNoGameplayDuplication(
        Scene scene, List<string> failures)
    {
        F2UpgradeRowUI[] rows = FindAll<F2UpgradeRowUI>(scene);
        foreach (string id in CanonicalIds)
            Check(rows.Count(row => row.UpgradeId == id) == 1,
                "La capa visual duplico la mejora " + id + ".", failures);
        Check(FindAll<F2UpgradeManager>(scene).Length <= 1,
            "La capa visual duplico F2UpgradeManager.", failures);
        Check(FindAll<KeycardPurchaseUI>(scene).Length == 1,
            "La Keycard falta o fue duplicada.", failures);
    }

    private static T[] FindAll<T>(Scene scene) where T : Component
    {
        var result = new List<T>();
        foreach (GameObject root in scene.GetRootGameObjects())
            result.AddRange(root.GetComponentsInChildren<T>(true));
        return result.ToArray();
    }

    private static Transform FindDescendant(Transform root, string name)
    {
        foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            if (current.name == name) return current;
        return null;
    }

    private static int CountDescendants(Transform root, string name)
    {
        int count = 0;
        foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
            if (current.name == name) count++;
        return count;
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

    private static bool Near(float actual, float expected, float tolerance) =>
        Mathf.Abs(actual - expected) <= tolerance;

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[Upgrades Visual Polish] VALIDATION PASS | cabecera real | " +
                "titulo mecanico | tres railes | ocho modulos compactos | " +
                "iconos | tactil | sin logica duplicada");
            return;
        }
        foreach (string failure in failures)
            Debug.LogError("[Upgrades Visual Polish] " + failure);
        throw new InvalidOperationException(
            "Upgrades Visual Polish fallo con " + failures.Count + " error(es).");
    }
}
#endif
