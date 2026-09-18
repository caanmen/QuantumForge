#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Dimension1SectorDetailsSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string ArtPath = "Assets/Project/UI/Dimension1/Generated";
    private const string MetalPath = ArtPath + "/MetalsInventory";
    private const string CandidatePath = ArtPath + "/Candidates/SectorDetails";
    private const string AncientPath = ArtPath + "/Candidates/AncientOrbits";
    private const string DebrisRingArtPath =
        ArtPath + "/Candidates/DebrisRing/d1_debris_ring_option_1_dense_orbit.png";
    private const string TemplateRootName = "D1_AncientOrbitsVisualRoot";

    private sealed class PlanetSpec
    {
        public string id;
        public string title;
        public string metalsLabel;
        public string primary;
        public string secondary;
        public string tertiary;
        public string artPath;
        public string productionArtPath;
        public Color tint = Color.white;
    }

    private sealed class SectorSpec
    {
        public string rootName;
        public string sectorId;
        public string title;
        public string subtitle;
        public PlanetSpec[] planets;
        public string[] destinationIds;
        public string[] destinationLabels;
        public string[] destinationArtPaths;
    }

    [MenuItem("Quantum Forge/Dimension 1/Install Remaining Sector Detail Subscreens")]
    public static void Install()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        foreach (string path in NewCandidatePaths()) PrepareSprite(path);

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Dimension1PanelUI panel = FindSceneComponent<Dimension1PanelUI>(scene);
        Dimension1CommandCenterUI commandCenter = FindSceneComponent<Dimension1CommandCenterUI>(scene);
        Dimension1MetalsInventoryUI metals = FindSceneComponent<Dimension1MetalsInventoryUI>(scene);
        Transform template = FindSceneTransform(scene, TemplateRootName);
        if (panel == null || commandCenter == null || metals == null || template == null ||
            template.GetComponent<Dimension1AncientOrbitsUI>() == null)
        {
            throw new InvalidOperationException(
                "La familia de sectores requiere la subpantalla canónica de Órbitas Antiguas.");
        }

        SectorSpec[] specs = BuildSpecs();
        var visuals = new List<Dimension1SectorDetailUI>();
        foreach (SectorSpec spec in specs)
            visuals.Add(BuildSector(scene, panel, commandCenter, metals, template, spec));

        SerializedObject panelSerialized = new SerializedObject(panel);
        SerializedProperty details = panelSerialized.FindProperty("sectorDetailUIs");
        if (details == null) throw new InvalidOperationException("Falta el enlace sectorDetailUIs.");
        details.arraySize = visuals.Count;
        for (int i = 0; i < visuals.Count; i++)
            details.GetArrayElementAtIndex(i).objectReferenceValue = visuals[i];
        panelSerialized.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(panel);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Unity no pudo guardar Main.unity.");

        ValidateInternal(scene);
        Debug.Log("[D1 Sector Details] INSTALL_PASS | sectores 1, 2 y 4 | datos reales | IDs estables");
    }

    [MenuItem("Quantum Forge/Dimension 1/Validate Remaining Sector Detail Subscreens")]
    public static void Validate()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateInternal(scene);
        Debug.Log("[D1 Sector Details] VALIDATION_PASS");
    }

    private static Dimension1SectorDetailUI BuildSector(
        Scene scene,
        Dimension1PanelUI panel,
        Dimension1CommandCenterUI commandCenter,
        Dimension1MetalsInventoryUI metals,
        Transform template,
        SectorSpec spec)
    {
        Transform previous = FindSceneTransform(scene, spec.rootName);
        if (previous != null) UnityEngine.Object.DestroyImmediate(previous.gameObject);

        GameObject clone = UnityEngine.Object.Instantiate(template.gameObject, panel.transform);
        clone.name = spec.rootName;
        RectTransform root = clone.GetComponent<RectTransform>();
        root.SetAsLastSibling();
        root.anchoredPosition = Dimension1SharedLayoutTokens.RootOffset;

        Dimension1AncientOrbitsUI oldController = clone.GetComponent<Dimension1AncientOrbitsUI>();
        if (oldController != null) UnityEngine.Object.DestroyImmediate(oldController);
        Dimension1SectorDetailUI visual = clone.AddComponent<Dimension1SectorDetailUI>();
        CanvasGroup group = RequireComponent<CanvasGroup>(clone.transform);
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        SetText(root, "Heading/Title", spec.title);
        SetText(root, "Heading/Subtitle", spec.subtitle);

        Transform firstPlanet = Require(root, "Planet4");
        Transform secondPlanet = Require(root, "Planet5");
        var planetViews = new List<Dimension1SectorDetailUI.PlanetCardView>();
        ApplyPlanet(firstPlanet, spec.planets[0]);
        planetViews.Add(CreatePlanetView(firstPlanet, spec.planets[0]));
        firstPlanet.name = "Planet_" + spec.planets[0].id;

        if (spec.planets.Length > 1)
        {
            ApplyPlanet(secondPlanet, spec.planets[1]);
            planetViews.Add(CreatePlanetView(secondPlanet, spec.planets[1]));
            secondPlanet.name = "Planet_" + spec.planets[1].id;
        }
        else
        {
            RectTransform centered = firstPlanet as RectTransform;
            centered.anchoredPosition = new Vector2(centered.anchoredPosition.x, -512f);
            secondPlanet.gameObject.SetActive(false);
            secondPlanet.name = "UnusedPlanetCard";
        }

        Button back = RequireComponentAt<Button>(root, "Heading/BackButton");
        Button backToMap = RequireComponentAt<Button>(root, "BackToGalaxyMap");
        RectTransform backToMapRect = backToMap.transform as RectTransform;
        if (backToMapRect != null)
            backToMapRect.anchoredPosition = new Vector2(
                backToMapRect.anchoredPosition.x,
                spec.planets.Length > 1 ? -1314f : -1060f);
        Button allMetals = RequireComponentAt<Button>(root, "AllMetals");
        Reset(back);
        Reset(backToMap);
        Reset(allMetals);
        foreach (Dimension1SectorDetailUI.PlanetCardView view in planetViews) Reset(view.actionButton);

        TMP_Text[] amounts = new TMP_Text[3];
        TMP_Text[] rates = new TMP_Text[3];
        for (int i = 0; i < 3; i++)
        {
            amounts[i] = RequireComponentAt<TMP_Text>(root, "Metal_" + i + "/Amount");
            rates[i] = RequireComponentAt<TMP_Text>(root, "Metal_" + i + "/Rate");
        }

        string[] navNames = { "Nav_GALAXIA", "Nav_EXPLORAR", "Nav_HANGAR", "Nav_RELIQUIAS", "Nav_ÁRBOL" };
        Button[] navigation = new Button[navNames.Length];
        for (int i = 0; i < navigation.Length; i++)
        {
            navigation[i] = RequireComponentAt<Button>(root, "BottomNavigation/" + navNames[i]);
            Reset(navigation[i]);
        }

        visual.Configure(spec.sectorId, panel, commandCenter, metals, group, back, backToMap,
            allMetals, amounts, rates, planetViews.ToArray(), Array.Empty<string>(),
            Array.Empty<Button>(), Array.Empty<TMP_Text>(), navigation);

        Add(back.onClick, visual.CloseToGalaxy);
        Add(backToMap.onClick, visual.CloseToGalaxy);
        Add(allMetals.onClick, visual.OpenMetals);
        Add(planetViews[0].actionButton.onClick, visual.ActOnPlanet0);
        if (planetViews.Count > 1) Add(planetViews[1].actionButton.onClick, visual.ActOnPlanet1);
        Add(navigation[0].onClick, visual.OpenGalaxy);
        Add(navigation[1].onClick, visual.OpenExplore);
        Add(navigation[2].onClick, visual.OpenHangar);
        Add(navigation[3].onClick, visual.OpenRelics);
        Add(navigation[4].onClick, visual.OpenTree);

        Dimension1SharedShellApply.ApplyToRoot(root);
        EditorUtility.SetDirty(visual);
        return visual;
    }

    private static void ApplyPlanet(Transform card, PlanetSpec spec)
    {
        SetText(card, "PlanetTitle", spec.title);
        SetText(card, "Metals", spec.metalsLabel);
        Image planet = RequireComponentAt<Image>(card, "PlanetWell/PlanetArt");
        planet.sprite = LoadRequiredSprite(spec.artPath);
        planet.color = spec.tint;
        planet.preserveAspect = true;
        RectTransform rect = planet.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = Vector2.zero;
        Image production = RequireComponentAt<Image>(card, "ProductionMetal");
        production.sprite = LoadRequiredSprite(spec.productionArtPath);
        production.color = Color.white;
        production.preserveAspect = true;
    }

    private static Dimension1SectorDetailUI.PlanetCardView CreatePlanetView(
        Transform card, PlanetSpec spec)
    {
        return new Dimension1SectorDetailUI.PlanetCardView
        {
            planetId = spec.id,
            primaryMetalId = spec.primary,
            secondaryMetalId = spec.secondary,
            tertiaryMetalId = spec.tertiary,
            levelText = RequireComponentAt<TMP_Text>(card, "LevelBadge/LevelValue"),
            productionText = RequireComponentAt<TMP_Text>(card, "ProductionValue"),
            progressText = RequireComponentAt<TMP_Text>(card, "ProgressValue"),
            progressFill = RequireComponentAt<Image>(card, "ProgressBackground/ProgressFill"),
            actionText = RequireComponentAt<TMP_Text>(card, "ActionButton/ActionLabel"),
            costText = RequireComponentAt<TMP_Text>(card, "ActionButton/Cost"),
            actionButton = RequireComponentAt<Button>(card, "ActionButton")
        };
    }

    private static SectorSpec[] BuildSpecs()
    {
        string abandonedShip = AncientPath + "/d1_destination_abandoned_ship_v2.png";
        string orbitalRuin = AncientPath + "/d1_destination_orbital_ruin_v2.png";
        string mineralBelt = CandidatePath + "/d1_destination_mineral_belt_v2.png";
        string graveyard = CandidatePath + "/d1_destination_ship_graveyard_v2.png";
        string probes = CandidatePath + "/d1_destination_drifting_probes_v2.png";
        string anomaly = CandidatePath + "/d1_destination_minor_anomaly_v2.png";
        string structure = CandidatePath + "/d1_destination_ancient_structure_v2.png";
        string unstable = CandidatePath + "/d1_destination_unstable_zone_v2.png";

        return new[]
        {
            new SectorSpec
            {
                rootName = "D1_OuterRimDetailVisualRoot",
                sectorId = Dimension1System.Sector01OuterRim,
                title = "BORDE EXTERIOR",
                subtitle = "GALAXIA / SECTOR 1",
                planets = new[]
                {
                    Planet(Dimension1System.Planet01, "PLANETA 1", "HIERRO / COBRE",
                        Dimension1System.MetalIron, Dimension1System.MetalCopper, null,
                        ArtPath + "/d1_body_planet_blue_v3.png", Metal("iron")),
                    Planet(Dimension1System.Planet02, "PLANETA 2", "ALUMINIO / TITANIO",
                        Dimension1System.MetalAluminum, Dimension1System.MetalTitanium, null,
                        ArtPath + "/d1_body_planet_molten_alpha_v2.png", Metal("aluminum"))
                },
                destinationIds = Dimension1System.GetDimension1SectorDestinationIds(
                    Dimension1System.Sector01OuterRim),
                destinationLabels = new[] { "CINTURÓN\nMINERAL", "CEMENTERIO\nDE NAVES", "SONDAS A\nLA DERIVA", "NAVE\nABANDONADA" },
                destinationArtPaths = new[] { mineralBelt, graveyard, probes, abandonedShip }
            },
            new SectorSpec
            {
                rootName = "D1_DebrisRingDetailVisualRoot",
                sectorId = Dimension1System.Sector02DebrisRing,
                title = "ANILLO DE RESTOS",
                subtitle = "GALAXIA / SECTOR 2",
                planets = new[]
                {
                    Planet(Dimension1System.Planet03, "PLANETA 3", "NÍQUEL / COBALTO",
                        Dimension1System.MetalNickel, Dimension1System.MetalCobalt, null,
                        DebrisRingArtPath, Metal("nickel"))
                },
                destinationIds = Dimension1System.GetDimension1SectorDestinationIds(
                    Dimension1System.Sector02DebrisRing),
                destinationLabels = new[] { "CEMENTERIO\nDE NAVES", "CINTURÓN\nMINERAL", "NAVE\nABANDONADA", "SONDAS A\nLA DERIVA" },
                destinationArtPaths = new[] { graveyard, mineralBelt, abandonedShip, probes }
            },
            new SectorSpec
            {
                rootName = "D1_SilentFrontierDetailVisualRoot",
                sectorId = Dimension1System.Sector04SilentFrontier,
                title = "FRONTERA SILENCIOSA",
                subtitle = "GALAXIA / SECTOR 4",
                planets = new[]
                {
                    Planet(Dimension1System.Planet06, "PLANETA 6", "IRIDIO / COBALTO",
                        Dimension1System.MetalIridium, Dimension1System.MetalCobalt, null,
                        ArtPath + "/d1_body_planet_silent_v3.png", Metal("iridium"), Hex("8D969C", 220)),
                    Planet(Dimension1System.Planet07, "PLANETA 7", "TUNGSTENO / PLATINO / IRIDIO",
                        Dimension1System.MetalTungsten, Dimension1System.MetalPlatinum,
                        Dimension1System.MetalIridium, ArtPath + "/d1_body_planet_silent_v3.png",
                        Metal("tungsten"), Hex("7D858B", 215))
                },
                destinationIds = Dimension1System.GetDimension1SectorDestinationIds(
                    Dimension1System.Sector04SilentFrontier),
                destinationLabels = new[] { "RUINA\nORBITAL", "ANOMALÍA\nMENOR", "ESTRUCTURA\nANTIGUA", "ZONA\nINESTABLE" },
                destinationArtPaths = new[] { orbitalRuin, anomaly, structure, unstable }
            }
        };
    }

    private static PlanetSpec Planet(string id, string title, string metalsLabel,
        string primary, string secondary, string tertiary, string artPath,
        string productionArtPath, Color? tint = null)
    {
        return new PlanetSpec
        {
            id = id,
            title = title,
            metalsLabel = metalsLabel,
            primary = primary,
            secondary = secondary,
            tertiary = tertiary,
            artPath = artPath,
            productionArtPath = productionArtPath,
            tint = tint ?? Color.white
        };
    }

    private static string Metal(string name) => MetalPath + "/d1_metal_" + name + "_v5.png";

    private static string[] NewCandidatePaths()
    {
        return new[]
        {
            DebrisRingArtPath,
            CandidatePath + "/d1_destination_mineral_belt_v2.png",
            CandidatePath + "/d1_destination_ship_graveyard_v2.png",
            CandidatePath + "/d1_destination_drifting_probes_v2.png",
            CandidatePath + "/d1_destination_minor_anomaly_v2.png",
            CandidatePath + "/d1_destination_ancient_structure_v2.png",
            CandidatePath + "/d1_destination_unstable_zone_v2.png"
        };
    }

    private static void ValidateInternal(Scene scene)
    {
        SectorSpec[] specs = BuildSpecs();
        foreach (SectorSpec spec in specs)
        {
            Transform root = FindSceneTransform(scene, spec.rootName);
            Dimension1SectorDetailUI visual = root != null
                ? root.GetComponent<Dimension1SectorDetailUI>() : null;
            if (root == null || visual == null || visual.SectorId != spec.sectorId)
                throw new InvalidOperationException("Falta la subpantalla de " + spec.title + ".");
            if (root.GetComponentsInChildren<Button>(true).Length < 8)
                throw new InvalidOperationException("Faltan interacciones en " + spec.title + ".");

            for (int i = 0; i < spec.planets.Length; i++)
            {
                Transform card = Require(root, "Planet_" + spec.planets[i].id);
                RectTransform art = Require(card, "PlanetWell/PlanetArt") as RectTransform;
                if (art.anchorMin != new Vector2(.5f, .5f) || art.anchorMax != new Vector2(.5f, .5f) ||
                    art.pivot != new Vector2(.5f, .5f) || art.anchoredPosition.sqrMagnitude > .0001f)
                {
                    throw new InvalidOperationException("Planeta descentrado en " + spec.title + ".");
                }
            }
        }
    }

    private static void SetText(Transform root, string path, string value)
    {
        RequireComponentAt<TMP_Text>(root, path).text = value;
    }

    private static Transform Require(Transform root, string path)
    {
        Transform found = root.Find(path);
        if (found == null) throw new InvalidOperationException("Falta " + root.name + "/" + path + ".");
        return found;
    }

    private static T RequireComponentAt<T>(Transform root, string path) where T : Component
    {
        return RequireComponent<T>(Require(root, path));
    }

    private static T RequireComponent<T>(Transform root) where T : Component
    {
        T value = root.GetComponent<T>();
        if (value == null) throw new InvalidOperationException("Falta " + typeof(T).Name + " en " + root.name + ".");
        return value;
    }

    private static void Reset(Button button)
    {
        button.onClick = new Button.ButtonClickedEvent();
        EditorUtility.SetDirty(button);
    }

    private static void Add(UnityEvent value, UnityAction action)
    {
        UnityEventTools.AddPersistentListener(value, action);
    }

    private static void PrepareSprite(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("No se pudo importar " + path + ".");
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.maxTextureSize = 1024;
        importer.SaveAndReimport();
    }

    private static Sprite LoadRequiredSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null) throw new InvalidOperationException("Falta el sprite " + path + ".");
        return sprite;
    }

    private static T FindSceneComponent<T>(Scene scene) where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T found = root.GetComponentInChildren<T>(true);
            if (found != null) return found;
        }
        return null;
    }

    private static Transform FindSceneTransform(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child;
        return null;
    }

    private static Color Hex(string html, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + html, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
#endif
