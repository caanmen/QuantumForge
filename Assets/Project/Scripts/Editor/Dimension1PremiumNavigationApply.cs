#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1PremiumNavigationApply
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated/NavigationPremium";
    private const string MaterialPath = ArtPath + "/d1_nav_premium_black_key.mat";
    private const string SelectedMaterialPath = ArtPath + "/d1_nav_premium_black_key_selected.mat";

    private static readonly string[] IconPaths =
    {
        ArtPath + "/d1_nav_galaxy_premium_v1.png",
        ArtPath + "/d1_nav_explore_premium_v1.png",
        ArtPath + "/d1_nav_hangar_premium_v1.png",
        ArtPath + "/d1_nav_relics_premium_v1.png",
        ArtPath + "/d1_nav_tree_premium_v1.png"
    };

    private static readonly string[] RootNames =
    {
        "D1CommandCenterProductionRoot", "D1_GalaxyVisualRoot", "D1_ExploreVisualRoot",
        "D1_HangarVisualRoot", "D1_RelicsVisualRoot", "D1_TreeVisualRoot"
    };

    [MenuItem("Quantum Forge/Dimension 1/Apply Premium Navigation Icons Safely")]
    public static void Apply()
    {
        PrepareAssets();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        for (int i = 0; i < RootNames.Length; i++)
        {
            Transform root = FindSceneTransform(scene, RootNames[i]);
            if (root == null) throw new InvalidOperationException("Falta " + RootNames[i] + ".");
            ApplyToRoot(root, i == 0 ? -1 : i - 1);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");
        Validate(scene);
        Debug.Log("[D1 Premium Navigation] APPLY_PASS | 6 pantallas | 5 iconos detallados canónicos");
    }

    public static void PrepareAssets()
    {
        foreach (string path in IconPaths) ConfigureSprite(path);
        Shader shader = Shader.Find("UI/QuantumForgeBlackKey");
        if (shader == null) throw new InvalidOperationException("Falta shader UI/QuantumForgeBlackKey.");
        EnsureMaterial(MaterialPath, shader, false);
        EnsureMaterial(SelectedMaterialPath, shader, true);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    public static void ApplyToRoot(Transform root, int selectedIndex)
    {
        if (root == null) throw new ArgumentNullException(nameof(root));
        Material normal = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        Material selected = AssetDatabase.LoadAssetAtPath<Material>(SelectedMaterialPath);
        if (normal == null || selected == null)
        {
            PrepareAssets();
            normal = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            selected = AssetDatabase.LoadAssetAtPath<Material>(SelectedMaterialPath);
        }

        Transform navigation = FindDirectChild(root, "BottomNavigation") ??
            FindDirectChild(root, "D1BottomNavigation");
        Transform cardParent = navigation != null ? navigation : root;
        List<RectTransform> cards = CollectCards(cardParent);
        if (cards.Count != 5)
            throw new InvalidOperationException("Navegación incompleta en " + root.name + ".");

        for (int i = 0; i < cards.Count; i++)
        {
            HideLegacyIcon(cards[i]);
            Transform existing = FindDirectChild(cards[i], "PremiumNavigationIcon");
            Image icon;
            if (existing == null)
            {
                GameObject go = new GameObject("PremiumNavigationIcon", typeof(RectTransform),
                    typeof(CanvasRenderer), typeof(Image));
                go.layer = 5;
                go.transform.SetParent(cards[i], false);
                icon = go.GetComponent<Image>();
            }
            else
            {
                icon = existing.GetComponent<Image>();
                if (icon == null) icon = existing.gameObject.AddComponent<Image>();
                existing.gameObject.SetActive(true);
            }

            RectTransform rect = icon.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2(0f, 27f);
            rect.sizeDelta = new Vector2(116f, 108f);
            icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(IconPaths[i]);
            icon.material = i == selectedIndex ? selected : normal;
            icon.color = i == selectedIndex ? new Color32(244, 167, 11, 255) : Color.white;
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.transform.SetSiblingIndex(Mathf.Max(0, cards[i].childCount - 2));
            EditorUtility.SetDirty(icon);
        }
    }

    private static void HideLegacyIcon(RectTransform card)
    {
        for (int i = 0; i < card.childCount; i++)
        {
            Transform child = card.GetChild(i);
            string name = child.name;
            if (name == "PremiumNavigationIcon" || name == "Label" || name == "Fill" ||
                name == "Border" || name == "Shadow" || name == "PanelFill" ||
                name == "OuterLine" || name == "InnerLine" ||
                name.EndsWith("_Border", StringComparison.Ordinal) ||
                name.EndsWith("_InnerBorder", StringComparison.Ordinal) ||
                child.GetComponent<TMP_Text>() != null || child.GetComponent<Text>() != null)
                continue;
            child.gameObject.SetActive(false);
            EditorUtility.SetDirty(child.gameObject);
        }
    }

    private static List<RectTransform> CollectCards(Transform parent)
    {
        var cards = new List<RectTransform>();
        bool commandCenter = parent.name == "D1CommandCenterProductionRoot";
        for (int i = 0; i < parent.childCount; i++)
        {
            RectTransform child = parent.GetChild(i) as RectTransform;
            if (child == null || child.name == "SelectedPointer") continue;
            if (child.name.StartsWith("Nav_", StringComparison.Ordinal) ||
                (!commandCenter && child.name.EndsWith("Button", StringComparison.Ordinal)))
                cards.Add(child);
        }
        cards.Sort((a, b) => a.anchoredPosition.x.CompareTo(b.anchoredPosition.x));
        return cards;
    }

    private static void ConfigureSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("No se pudo importar " + path + ".");
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.maxTextureSize = 512;
        importer.SaveAndReimport();
    }

    private static void EnsureMaterial(string path, Shader shader, bool selected)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(shader)
            {
                name = selected ? "D1 Premium Navigation Black Key Selected" :
                    "D1 Premium Navigation Black Key"
            };
            AssetDatabase.CreateAsset(material, path);
        }
        material.shader = shader;
        material.SetFloat("_UseTint", selected ? 1f : 0f);
        EditorUtility.SetDirty(material);
    }

    private static void Validate(Scene scene)
    {
        foreach (string rootName in RootNames)
        {
            Transform root = FindSceneTransform(scene, rootName);
            Transform navigation = FindDirectChild(root, "BottomNavigation") ??
                FindDirectChild(root, "D1BottomNavigation");
            List<RectTransform> cards = CollectCards(navigation != null ? navigation : root);
            for (int i = 0; i < cards.Count; i++)
            {
                Image icon = FindDirectChild(cards[i], "PremiumNavigationIcon")?.GetComponent<Image>();
                if (icon == null || icon.sprite == null || icon.material == null || icon.raycastTarget)
                    throw new InvalidOperationException("Icono premium inválido en " + rootName + ".");
            }
        }
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
