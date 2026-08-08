#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Installs the authored Blender face as a reversible visual layer. The
/// generated logical rig remains authoritative for nodes, colliders, saves and
/// rotation; only its legacy renderers are disabled.
/// </summary>
public static class MachineCubeBlenderVisualSetup
{
    private const string ModelPath =
        "Assets/Project/Art/MachineCubeBlender/QF_MachineCube_ConceptV2.fbx";
    private const string PbrFolder =
        "Assets/Project/UI/Vertical/Machine/Prototype3D/Materials/PBR/";
    private const string BaseFolder =
        "Assets/Project/UI/Vertical/Machine/Prototype3D/";
    private const string BlenderMaterialFolder =
        "Assets/Project/Art/MachineCubeBlender/Materials";
    private const int PrototypeLayer = 30;

    [MenuItem("Tools/Quantum Forge/Machine/Install Blender Visual")]
    public static void ConfigureBlenderPreview()
    {
        ConfigureImporter();
        MachineCube3DPrototypeSetup.ConfigurePrototype();
        ValidateInstalledVisual();
    }

    public static void ConfigureBlenderPreviewBatch()
    {
        ConfigureBlenderPreview();
    }

    public static bool ApplyToGeneratedPrototype(GameObject prototypeRoot)
    {
        if (prototypeRoot == null)
            return false;
        GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (modelAsset == null)
        {
            Debug.LogWarning("[Machine Cube Blender] FBX no disponible; se conserva " +
                "el visual procedural de respaldo.");
            return false;
        }

        Transform cubeRoot = FindDeepChild(prototypeRoot.transform,
            "PhysicalCubeRoot");
        if (cubeRoot == null)
            throw new InvalidOperationException("Falta PhysicalCubeRoot.");

        var installedRoots = new List<Transform>(4);
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            Transform face = FindDeepChild(cubeRoot,
                "PhysicalFace_" + (faceIndex + 1));
            if (face == null)
                throw new InvalidOperationException("Falta PhysicalFace_" +
                    (faceIndex + 1) + ".");

            Transform previous = face.Find("BlenderVisualRoot");
            if (previous != null)
                UnityEngine.Object.DestroyImmediate(previous.gameObject);

            GameObject instance = PrefabUtility.InstantiatePrefab(modelAsset,
                face) as GameObject;
            if (instance == null)
            {
                instance = UnityEngine.Object.Instantiate(modelAsset, face);
                PrefabUtility.UnpackPrefabInstance(instance,
                    PrefabUnpackMode.Completely,
                    InteractionMode.AutomatedAction);
            }
            instance.name = "BlenderVisualRoot";
            instance.transform.localPosition = Vector3.zero;
            // Blender source uses X horizontal, Z vertical and -Y outward.
            // This rotation maps it to Unity X/Y with local +Z outward.
            instance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            instance.transform.localScale = Vector3.one;
            SetLayerRecursively(instance, PrototypeLayer);
            ConfigureStateRoots(instance.transform);
            RemapMaterials(instance.transform, faceIndex);
            installedRoots.Add(instance.transform);
        }

        Renderer[] allRenderers = cubeRoot.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < allRenderers.Length; i++)
        {
            Renderer renderer = allRenderers[i];
            bool authored = IsUnderInstalledRoot(renderer.transform,
                installedRoots);
            bool authoredDecorativeNode = authored &&
                IsAuthoredDecorativeNode(renderer.transform);
            // Keep only the dedicated logical-node selection brackets from the
            // procedural rig. They are normally inactive and become visible
            // only for the selected collider. Disabling them here made clicks
            // update the card without any feedback on the cube itself.
            bool selectionFeedback = IsUnderNamedParent(renderer.transform,
                "SelectionFeedback");
            // The authored Blender face contains four composition-only nodes,
            // while gameplay owns 35 public + 8 hidden nodes. Decorative nodes
            // are hidden and the physical NodeModules rig is retained so every
            // visible socket, collider and selection bracket shares a position.
            bool physicalGameplayNode = IsUnderNamedParent(renderer.transform,
                "NodeModules");
            renderer.enabled = (authored && !authoredDecorativeNode) ||
                physicalGameplayNode || selectionFeedback;
        }

        MachineCube3DFace[] faces = cubeRoot.GetComponentsInChildren<
            MachineCube3DFace>(true);
        for (int i = 0; i < faces.Length; i++)
            faces[i]?.RebuildCache();
        MachineCube3DVisualStateController visualState =
            prototypeRoot.GetComponent<MachineCube3DVisualStateController>();
        visualState?.RebuildCache();

        Bounds localBounds = CalculateLocalBounds(cubeRoot, installedRoots[0]);
        if (localBounds.size.x < 8f || localBounds.size.y < 8f ||
            localBounds.size.z > 2.2f)
        {
            throw new InvalidOperationException(
                $"Orientación/bounds FBX incorrectos: {localBounds.size:F3}");
        }
        Debug.Log($"[Machine Cube Blender] INSTALLED | faces=4 | " +
            $"master bounds={localBounds.size:F3} | legacy visual retained disabled");
        return true;
    }

    private static void ConfigureImporter()
    {
        AssetDatabase.ImportAsset(ModelPath, ImportAssetOptions.ForceUpdate);
        ModelImporter importer = AssetImporter.GetAtPath(ModelPath) as ModelImporter;
        if (importer == null)
            throw new InvalidOperationException("No se pudo configurar el FBX de Blender.");
        importer.globalScale = 1f;
        importer.importAnimation = false;
        importer.importBlendShapes = false;
        importer.importCameras = false;
        importer.importLights = false;
        importer.generateSecondaryUV = false;
        importer.isReadable = false;
        importer.meshCompression = ModelImporterMeshCompression.Medium;
        importer.importNormals = ModelImporterNormals.Import;
        importer.importTangents = ModelImporterTangents.CalculateMikk;
        importer.optimizeMeshPolygons = true;
        importer.optimizeMeshVertices = true;
        importer.SaveAndReimport();
    }

    private static void ConfigureStateRoots(Transform root)
    {
        Transform damage = FindDeepChild(root, "DAMAGE_STATES");
        Transform repairs = FindDeepChild(root, "REPAIR_STATES");
        if (damage == null || repairs == null)
            throw new InvalidOperationException(
                "El FBX no contiene DAMAGE_STATES/REPAIR_STATES.");
        damage.gameObject.SetActive(true);
        repairs.gameObject.SetActive(true);
        for (int i = 0; i < damage.childCount; i++)
            damage.GetChild(i).gameObject.SetActive(true);
        for (int i = 0; i < repairs.childCount; i++)
            repairs.GetChild(i).gameObject.SetActive(false);
    }

    private static void RemapMaterials(Transform root, int faceIndex)
    {
        EnsureBlenderMaterialFolder();
        // The legacy materials were intentionally almost black because the
        // procedural mesh had little real depth. The authored mesh needs more
        // diffuse response so its cavities and layered silhouette survive the
        // isolated black-background render texture. These are independent
        // clones, so the reversible procedural fallback is not altered.
        Material surface = CreateBlenderMaterial("QF_B_Surface",
            LoadMaterial("M3D_PBR_Cube_Surface35.mat"),
            new Color(0.48f, 0.41f, 0.34f), 0.50f, 0.10f, 0.52f, 1.24f);
        Material surfaceDark = CreateBlenderMaterial("QF_B_SurfaceDark",
            LoadMaterial("M3D_PBR_Cube_Surface35.mat"),
            new Color(0.34f, 0.32f, 0.29f), 0.56f, 0.075f, 0.62f, 1.30f);
        Material surfaceRust = CreateBlenderMaterial("QF_B_SurfaceRust",
            LoadMaterial("M3D_PBR_Cube_Surface35.mat"),
            new Color(0.52f, 0.32f, 0.18f), 0.34f, 0.045f, 0.72f, 1.38f);
        Material armor = CreateBlenderMaterial("QF_B_Armor",
            LoadMaterial("M3D_PBR_Cube_Armor32.mat"),
            new Color(0.45f, 0.41f, 0.35f), 0.64f, 0.11f, 0.46f, 1.18f);
        Material armorDark = CreateBlenderMaterial("QF_B_ArmorDark",
            LoadMaterial("M3D_PBR_Cube_Armor32.mat"),
            new Color(0.31f, 0.30f, 0.28f), 0.70f, 0.075f, 0.56f, 1.24f);
        Material frame = CreateBlenderMaterial("QF_B_ExposedFrame",
            LoadMaterial("M3D_PBR_Cube_ExposedFrame32.mat"),
            new Color(0.39f, 0.36f, 0.32f), 0.74f, 0.12f, 0.42f, 1.14f);
        Material module = CreateBlenderMaterial("QF_B_Module",
            LoadMaterial("M3D_PBR_Cube_Module33.mat"),
            new Color(0.43f, 0.39f, 0.33f), 0.60f, 0.095f, 0.55f, 1.28f);
        Material moduleDark = CreateBlenderMaterial("QF_B_ModuleDark",
            LoadMaterial("M3D_PBR_Cube_Module33.mat"),
            new Color(0.28f, 0.28f, 0.26f), 0.68f, 0.065f, 0.62f, 1.34f);
        Material socket = CreateBlenderMaterial("QF_B_Inner",
            LoadMaterial("M3D_PBR_Cube_Socket33.mat"),
            new Color(0.20f, 0.19f, 0.17f), 0.62f, 0.09f, 0.48f, 1.25f);
        Material cavity = CreateBlenderMaterial("QF_B_Cavity",
            LoadMaterial("M3D_PBR_Cube_CarbonCavity48.mat"),
            new Color(0.055f, 0.045f, 0.038f), 0.30f, 0.035f, 0.58f, 1.30f);
        Material soot = CreateBlenderMaterial("QF_B_Soot",
            LoadMaterial("M3D_PBR_Cube_CarbonCavity48.mat"),
            new Color(0.032f, 0.025f, 0.020f), 0.08f, 0.015f, 0.62f, 1.35f);
        Material torn = CreateBlenderMaterial("QF_B_TornMetal",
            LoadMaterial("M3D_PBR_Cube_TornArmor48.mat"),
            new Color(0.56f, 0.46f, 0.36f), 0.68f, 0.11f, 0.48f, 1.18f);
        Material conduit = CreateBlenderMaterial("QF_B_Conduit",
            LoadMaterial("M3D_PBR_Cube_Conduit33.mat"),
            new Color(0.28f, 0.20f, 0.15f), 0.78f, 0.13f, 0.62f, 1.28f);
        Material oxide = CreateBlenderMaterial("QF_B_Oxide",
            LoadMaterial("M3D_PBR_Cube_TornArmor48.mat"),
            new Color(0.48f, 0.24f, 0.11f), 0.38f, 0.055f, 0.72f, 1.32f);
        Material warning = AssetDatabase.LoadAssetAtPath<Material>(
            BaseFolder + "M3D_WarningEmission.mat");
        string[] emissionNames =
        {
            "M3D_BlueEmission.mat", "M3D_PurpleEmission.mat",
            "M3D_OrangeEmission.mat", "M3D_GreenEmission.mat"
        };
        Material emission = AssetDatabase.LoadAssetAtPath<Material>(BaseFolder +
            emissionNames[Mathf.Clamp(faceIndex, 0, emissionNames.Length - 1)]);

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] slots = renderers[i].sharedMaterials;
            for (int slot = 0; slot < slots.Length; slot++)
            {
                string sourceName = slots[slot] != null ? slots[slot].name : "";
                int duplicateSuffix = sourceName.IndexOf('.');
                string materialKey = duplicateSuffix > 0
                    ? sourceName.Substring(0, duplicateSuffix)
                    : sourceName;
                int visualVariant = StableVariant(renderers[i].name, 5);
                slots[slot] = materialKey switch
                {
                    "M_SURFACE_35" => PickAgedSurface(visualVariant, surface,
                        surfaceDark, surfaceRust),
                    "C2_PBR_SKIN_35" => PickAgedSurface(visualVariant, surface,
                        surfaceDark, surfaceRust),
                    "C2_SKIN_GRAPHITE" => visualVariant <= 1
                        ? surfaceDark : surface,
                    "M_ARMOR_32" => visualVariant <= 1 ? armorDark : armor,
                    "C2_PBR_ARMOR_32" => visualVariant <= 1 ? armorDark : armor,
                    "C2_ARMOR_PLATE" => visualVariant <= 1 ? armorDark : armor,
                    "M_EXPOSED_METAL" => frame,
                    "C2_PBR_FRAME_32" => frame,
                    "C2_HEAVY_FRAME" => frame,
                    "M_NODE_33" => visualVariant == 0 ? moduleDark : module,
                    "C2_PBR_NODE_33" => visualVariant == 0 ? moduleDark : module,
                    "C2_NODE_EDGE" => visualVariant == 0 ? moduleDark : module,
                    "M_INNER_DARK" => socket,
                    "C2_INNER_STRUCTURE" => socket,
                    "M_DAMAGE_48" => cavity,
                    "C2_CARBON_VOID" => cavity,
                    "M_SOOT" => soot,
                    "C2_MOTTLED_SOOT" => soot,
                    "C2_CHARRED_FRACTURE" => soot,
                    "C2_EDGE_OXIDE" => oxide,
                    "M_TORN_METAL" => torn,
                    "C2_PBR_TORN_48" => torn,
                    "C2_TORN_RAW_METAL" => torn,
                    "M_COPPER_DARK" => conduit,
                    "C2_BURNT_COPPER" => conduit,
                    "M_CIRCUIT_DARK" => conduit,
                    "C2_DEAD_CIRCUIT" => conduit,
                    "C2_EXPOSED_MACHINERY" => conduit,
                    "M_EMISSION_FACE" => emission,
                    "C2_CYAN" => emission,
                    "M_WARNING" => warning,
                    "C2_AMBER" => warning,
                    _ => surface
                };
            }
            renderers[i].sharedMaterials = slots;
        }
    }

    private static Material LoadMaterial(string fileName)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(PbrFolder +
            fileName);
        if (material == null)
            throw new InvalidOperationException("Falta material PBR: " + fileName);
        return material;
    }

    private static void EnsureBlenderMaterialFolder()
    {
        if (!AssetDatabase.IsValidFolder(BlenderMaterialFolder))
            AssetDatabase.CreateFolder(
                "Assets/Project/Art/MachineCubeBlender", "Materials");
    }

    private static Material CreateBlenderMaterial(string name, Material source,
        Color tint, float metallic, float smoothness, float textureScale,
        float bumpScale)
    {
        string path = BlenderMaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(source) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }
        else
        {
            material.CopyPropertiesFromMaterial(source);
            material.shader = source.shader;
            material.name = name;
        }

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", tint);
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", tint);
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);
        Vector2 tiling = Vector2.one * textureScale;
        if (material.HasProperty("_BaseMap"))
            material.SetTextureScale("_BaseMap", tiling);
        if (material.HasProperty("_MainTex"))
            material.SetTextureScale("_MainTex", tiling);
        if (material.HasProperty("_BumpMap"))
        {
            material.SetTextureScale("_BumpMap", tiling);
            material.SetFloat("_BumpScale", bumpScale);
        }
        if (material.HasProperty("_MetallicGlossMap"))
            material.SetTextureScale("_MetallicGlossMap", tiling);
        if (material.HasProperty("_OcclusionMap"))
        {
            material.SetTextureScale("_OcclusionMap", tiling);
            material.SetFloat("_OcclusionStrength", 0.88f);
        }
        EditorUtility.SetDirty(material);
        return material;
    }

    private static bool IsUnderInstalledRoot(Transform target,
        List<Transform> roots)
    {
        for (int i = 0; i < roots.Count; i++)
            if (target == roots[i] || target.IsChildOf(roots[i]))
                return true;
        return false;
    }

    private static bool IsUnderNamedParent(Transform target, string parentName)
    {
        for (Transform current = target; current != null;
             current = current.parent)
        {
            if (current.name == parentName)
                return true;
        }
        return false;
    }

    private static bool IsAuthoredDecorativeNode(Transform target)
    {
        for (Transform current = target; current != null;
             current = current.parent)
        {
            string objectName = current.name;
            if (objectName == "NODES" ||
                objectName.StartsWith("C2_Node_", StringComparison.Ordinal) ||
                objectName.StartsWith("C2_RepairNode_", StringComparison.Ordinal) ||
                objectName.StartsWith("C2_SecondaryBrokenNode",
                    StringComparison.Ordinal) ||
                objectName.StartsWith("C2_BottomBrokenNode",
                    StringComparison.Ordinal))
            {
                return true;
            }
            if (objectName == "BlenderVisualRoot")
                break;
        }
        return false;
    }

    private static Material PickAgedSurface(int variant, Material standard,
        Material dark, Material rust)
    {
        if (variant == 0)
            return rust;
        if (variant <= 2)
            return dark;
        return standard;
    }

    private static int StableVariant(string value, int count)
    {
        unchecked
        {
            uint hash = 2166136261u;
            string source = value ?? string.Empty;
            for (int i = 0; i < source.Length; i++)
            {
                hash ^= source[i];
                hash *= 16777619u;
            }
            return (int)(hash % (uint)Mathf.Max(1, count));
        }
    }

    private static Bounds CalculateLocalBounds(Transform cubeRoot,
        Transform visualRoot)
    {
        Renderer[] renderers = visualRoot.GetComponentsInChildren<Renderer>(true);
        Bounds result = new Bounds(Vector3.zero, Vector3.zero);
        bool initialized = false;
        for (int i = 0; i < renderers.Length; i++)
        {
            Bounds bounds = renderers[i].bounds;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            for (int corner = 0; corner < 8; corner++)
            {
                Vector3 world = new Vector3((corner & 1) == 0 ? min.x : max.x,
                    (corner & 2) == 0 ? min.y : max.y,
                    (corner & 4) == 0 ? min.z : max.z);
                Vector3 local = cubeRoot.InverseTransformPoint(world);
                if (!initialized)
                {
                    result = new Bounds(local, Vector3.zero);
                    initialized = true;
                }
                else
                    result.Encapsulate(local);
            }
        }
        return result;
    }

    private static void ValidateInstalledVisual()
    {
        GameObject root = GameObject.Find("MachineCube3DPrototypeRoot");
        if (root == null)
            throw new InvalidOperationException("No se generó el prototipo 3D.");
        Transform cubeRoot = FindDeepChild(root.transform, "PhysicalCubeRoot");
        int installed = 0;
        for (int i = 1; i <= 4; i++)
        {
            Transform face = FindDeepChild(cubeRoot, "PhysicalFace_" + i);
            Transform visual = face != null ? face.Find("BlenderVisualRoot") : null;
            if (visual == null)
                throw new InvalidOperationException("Falta BlenderVisualRoot en cara " + i);
            if (FindDeepChild(visual, "DAMAGE_STATES") == null ||
                FindDeepChild(visual, "REPAIR_STATES") == null)
                throw new InvalidOperationException("Estados Blender incompletos en cara " + i);
            installed++;
        }
        MachineCube3DNode[] nodes = root.GetComponentsInChildren<MachineCube3DNode>(true);
        BoxCollider[] colliders = root.GetComponentsInChildren<BoxCollider>(true);
        if (nodes.Length != 43 || colliders.Length != 43)
            throw new InvalidOperationException($"Rig alterado: nodes={nodes.Length}, " +
                $"colliders={colliders.Length}");
        int visiblePhysicalNodes = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            MachineCube3DNode node = nodes[i];
            if (node == null || !node.gameObject.activeSelf)
                continue;
            Renderer[] nodeRenderers = node.GetComponentsInChildren<Renderer>(true);
            bool hasPhysicalRenderer = false;
            for (int rendererIndex = 0; rendererIndex < nodeRenderers.Length;
                 rendererIndex++)
            {
                Renderer renderer = nodeRenderers[rendererIndex];
                if (renderer.enabled && !IsUnderNamedParent(renderer.transform,
                        "SelectionFeedback"))
                {
                    hasPhysicalRenderer = true;
                    break;
                }
            }
            if (hasPhysicalRenderer)
                visiblePhysicalNodes++;
        }
        if (visiblePhysicalNodes != 35)
            throw new InvalidOperationException(
                $"Correspondencia visual incorrecta: physical public nodes=" +
                $"{visiblePhysicalNodes}, expected=35");
        Debug.Log($"[Machine Cube Blender] PASS | visuals={installed} | " +
            "nodes=43 | colliders=43 | physical public nodes=35 | rig preserved");
    }

    private static void SetLayerRecursively(GameObject root, int layer)
    {
        root.layer = layer;
        for (int i = 0; i < root.transform.childCount; i++)
            SetLayerRecursively(root.transform.GetChild(i).gameObject, layer);
    }

    private static Transform FindDeepChild(Transform root, string name)
    {
        if (root == null)
            return null;
        if (root.name == name)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform result = FindDeepChild(root.GetChild(i), name);
            if (result != null)
                return result;
        }
        return null;
    }
}
#endif
