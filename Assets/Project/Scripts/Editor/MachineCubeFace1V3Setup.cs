#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Installs the approved authored shell and node kit for Faces 1, 3 and 4.
/// </summary>
public static class MachineCubeFace1V3Setup
{
    private const string Face1ModelPath =
        "Assets/Project/Art/MachineCubeBlender/" +
        "QF_MachineCube_Face1_NodeKitV2.fbx";
    private const string Face3ModelPath =
        "Assets/Project/Art/MachineCubeBlender/" +
        "QF_MachineCube_Face3_NodeKitV2.fbx";
    private const string Face4ModelPath =
        "Assets/Project/Art/MachineCubeBlender/" +
        "QF_MachineCube_Face4_NodeKitV2.fbx";
    private const string TextureFolder =
        "Assets/Project/UI/Vertical/Machine/CustomTextures/QF_IndustrialDarkMetal_v1/";
    private const string MaterialFolder =
        "Assets/Project/Art/MachineCubeBlender/MaterialsV3";
    private const string TemplateMaterialPath =
        "Assets/Project/UI/Vertical/Machine/Prototype3D/Materials/PBR/" +
        "M3D_PBR_Cube_Surface35.mat";
    private const int PrototypeLayer = 30;

    private static readonly NodeSpec[] Face1Nodes =
    {
        new("z1_energy_coupling_1", 1.24f, 1.18f, true),
        new("z1_traces_channel_1", 1.14f, 1.08f, true),
        new("z1_protocol_reading", 1.30f, 0.80f, true),
        new("z1_triangle_anchor_1", 1.24f, 1.18f, true),
        new("z1_artifact_calibration", 0.84f, 0.80f, true),
        new("z1_flow_distributor", 1.16f, 1.04f, true),
        new("z1_room1_synchronizer", 1.34f, 0.76f, true),
        new("z1_hidden_energy_echo", 0.76f, 0.72f, false),
        new("z1_hidden_residual_adjustment", 0.76f, 0.70f, false),
    };

    private static readonly NodeSpec[] Face3Nodes =
    {
        new("z3_internal_diagnostics", 1.15f, 1.08f, true),
        new("z3_auxiliary_conduits", 1.34f, 0.80f, true),
        new("z3_compensation_circuit", 0.90f, 0.84f, true),
        new("z3_machine_memory", 1.18f, 1.10f, true),
        new("z3_sync_core", 1.18f, 1.12f, true),
        new("z3_structural_reinforcement", 1.34f, 0.78f, true),
        new("z3_convergence_channel", 1.14f, 1.08f, true),
        new("z3_hidden_failure_marker", 0.76f, 0.72f, false),
        new("z3_hidden_secondary_conduit", 0.76f, 0.70f, false),
    };

    private static readonly NodeSpec[] Face4Nodes =
    {
        new("z4_basic_chamber", 1.15f, 1.08f, true),
        new("z4_seed_reading_1", 1.12f, 1.06f, true),
        new("z4_archive_expansion_1", 0.84f, 0.80f, true),
        new("z4_initial_stability_1", 1.24f, 0.78f, true),
        new("z4_controlled_sync_1", 1.18f, 1.12f, true),
        new("z4_tuned_containment_1", 0.84f, 0.80f, true),
        new("z4_safe_rewind_1", 0.78f, 1.12f, true),
        new("z4_seed_slot_3", 1.12f, 1.08f, true),
        new("z4_pure_materialization_1", 1.24f, 0.78f, true),
        new("z4_seed_slot_4", 1.08f, 1.02f, true),
        new("z4_hidden_minor_chronal_pulse", 0.72f, 0.70f, false),
        new("z4_hidden_resonant_archive", 0.72f, 0.70f, false),
    };

    public static bool ApplyToGeneratedPrototype(GameObject prototypeRoot)
    {
        return ApplyAuthoredFace(prototypeRoot, 0, Face1ModelPath,
            Face1Nodes, "Face1 V3");
    }

    public static bool ApplyFace3ToGeneratedPrototype(GameObject prototypeRoot)
    {
        return ApplyAuthoredFace(prototypeRoot, 2, Face3ModelPath,
            Face3Nodes, "Face3 V1");
    }

    public static bool ApplyFace4ToGeneratedPrototype(GameObject prototypeRoot)
    {
        return ApplyAuthoredFace(prototypeRoot, 3, Face4ModelPath,
            Face4Nodes, "Face4 V1");
    }

    private static bool ApplyAuthoredFace(GameObject prototypeRoot,
        int faceIndex, string modelPath, NodeSpec[] specs, string label)
    {
        if (prototypeRoot == null)
            return false;
        ConfigureAssets(modelPath, label);
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (model == null)
        {
            Debug.LogWarning($"[Machine {label}] FBX no disponible; se conserva " +
                $"el visual procedural de la Cara {faceIndex + 1}.");
            return false;
        }

        Transform cube = FindDeepChild(prototypeRoot.transform,
            "PhysicalCubeRoot");
        Transform face = FindDeepChild(cube,
            "PhysicalFace_" + (faceIndex + 1));
        if (face == null)
            throw new InvalidOperationException(
                "Falta PhysicalFace_" + (faceIndex + 1) + ".");

        Transform previous = face.Find("BlenderVisualRoot");
        if (previous != null)
            UnityEngine.Object.DestroyImmediate(previous.gameObject);

        GameObject instance = PrefabUtility.InstantiatePrefab(model, face)
            as GameObject;
        if (instance == null)
        {
            instance = UnityEngine.Object.Instantiate(model, face);
            PrefabUtility.UnpackPrefabInstance(instance,
                PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
        instance.name = "BlenderVisualRoot";
        instance.transform.localPosition = Vector3.zero;
        // The V3 exporter already converts Blender -Z/Y axes to Unity. An
        // additional -90 degree rotation would lay the face horizontally.
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        SetLayerRecursively(instance, PrototypeLayer);

        DisableRenderers(FindDeepChild(face, "StateModules"));
        ConfigureReferenceBackplate(cube, face, instance.transform);
        RemoveProceduralFaceNodes(face, label);
        RemapMaterials(instance.transform);
        BindAuthoredAnchors(instance.transform, specs, faceIndex, label);

        MachineCube3DFace modularFace = face.GetComponent<MachineCube3DFace>();
        modularFace?.RebuildCache();
        MachineCube3DVisualStateController visualState =
            prototypeRoot.GetComponent<MachineCube3DVisualStateController>();
        visualState?.RebuildCache();
        ValidateAuthoredFace(prototypeRoot, face, instance.transform, specs,
            label);
        int publicCount = specs.Count(item => item.isPublic);
        Debug.Log($"[Machine {label}] INSTALLED | public={publicCount} | " +
            $"secret={specs.Length - publicCount} | selection=underlight | " +
            $"per-node states={specs.Length}");
        return true;
    }

    private static void ConfigureAssets(string modelPath, string label)
    {
        AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceUpdate);
        ModelImporter importer = AssetImporter.GetAtPath(modelPath) as ModelImporter;
        if (importer == null)
            throw new InvalidOperationException(
                "No se pudo importar " + label + ".");
        importer.globalScale = 1f;
        importer.importAnimation = false;
        importer.importBlendShapes = false;
        importer.importCameras = false;
        importer.importLights = false;
        importer.importVisibility = false;
        importer.preserveHierarchy = true;
        importer.generateSecondaryUV = false;
        importer.isReadable = false;
        importer.meshCompression = ModelImporterMeshCompression.Medium;
        importer.importNormals = ModelImporterNormals.Import;
        importer.importTangents = ModelImporterTangents.CalculateMikk;
        importer.optimizeMeshPolygons = true;
        importer.optimizeMeshVertices = true;
        importer.SaveAndReimport();

        ConfigureTexture("QF_IndustrialDarkMetal_Normal_1024.png", true, false);
        ConfigureTexture("QF_IndustrialDarkMetal_Occlusion_1024.png", false, true);
        ConfigureTexture("QF_IndustrialDarkMetal_Metallic_1024.png", false, true);
        ConfigureTexture("QF_IndustrialDarkMetal_Roughness_1024.png", false, true);
    }

    private static void ConfigureTexture(string fileName, bool normal,
        bool linear)
    {
        string path = TextureFolder + fileName;
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            throw new InvalidOperationException("Falta textura V3: " + path);
        importer.textureType = normal
            ? TextureImporterType.NormalMap
            : TextureImporterType.Default;
        importer.sRGBTexture = !normal && !linear;
        importer.mipmapEnabled = true;
        importer.maxTextureSize = 1024;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
    }

    private static void RemoveProceduralFaceNodes(Transform face, string label)
    {
        Transform nodeRoot = FindDeepChild(face, "NodeModules");
        if (nodeRoot == null)
            throw new InvalidOperationException(
                "Falta NodeModules en " + label + ".");
        for (int i = nodeRoot.childCount - 1; i >= 0; i--)
            UnityEngine.Object.DestroyImmediate(nodeRoot.GetChild(i).gameObject);
    }

    private static void BindAuthoredAnchors(Transform visualRoot,
        NodeSpec[] specs, int faceIndex, string label)
    {
        Transform anchors = FindDeepChild(visualRoot, "NODE_ANCHORS");
        if (anchors == null || anchors.childCount != specs.Length)
            throw new InvalidOperationException(
                $"{label} debe contener {specs.Length} anchors.");

        Transform[] ordered = anchors.Cast<Transform>()
            .OrderBy(item => item.name, StringComparer.Ordinal).ToArray();
        for (int slot = 0; slot < specs.Length; slot++)
        {
            Transform anchor = ordered[slot];
            NodeSpec spec = specs[slot];
            if (!anchor.name.StartsWith($"NODE_{slot:00}_",
                    StringComparison.Ordinal) ||
                !anchor.name.Contains(spec.id, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Anchor de {label} fuera de contrato en slot {slot}: " +
                    anchor.name);
            }

            MachineCube3DNode node = anchor.gameObject.AddComponent<
                MachineCube3DNode>();
            SerializedObject nodeSo = new SerializedObject(node);
            nodeSo.FindProperty("nodeId").stringValue = spec.id;
            nodeSo.FindProperty("faceIndex").intValue = faceIndex;
            nodeSo.FindProperty("slotIndex").intValue = slot;
            nodeSo.FindProperty("selectedEmission").floatValue = 8.0f;
            nodeSo.ApplyModifiedPropertiesWithoutUndo();

            BoxCollider collider = anchor.gameObject.AddComponent<BoxCollider>();
            collider.center = Vector3.zero;
            collider.size = new Vector3(spec.width * 1.18f, 2.00f,
                spec.height * 1.18f);
            collider.enabled = spec.isPublic;

            Transform damaged = FindChildStartingWith(anchor, "DAMAGED_STATE_");
            Transform repaired = FindChildStartingWith(anchor, "REPAIRED_STATE_");
            Transform selection = FindChildStartingWith(anchor,
                "SELECTION_FEEDBACK_");
            if (damaged == null || repaired == null || selection == null)
                throw new InvalidOperationException(
                    "Estados incompletos en " + anchor.name);
            ReplaceStatusStrip(repaired, "RepairedNode_OperationalStrip",
                0.22f);
            ReplaceStatusStrip(damaged, "DamagedNode_WarningStrip", 0.22f);
            AddDamageWarningGeometry(damaged, spec);
            selection.name = "SelectionFeedback";
            Renderer selectionRenderer = selection
                .GetComponentInChildren<Renderer>(true);
            selectionRenderer.transform.localScale = Vector3.Scale(
                selectionRenderer.transform.localScale,
                new Vector3(1.0f, 1.0f, 1.80f));
            NormalizeRendererWorldWidth(selectionRenderer, 0.22f);
            damaged.gameObject.SetActive(spec.isPublic);
            repaired.gameObject.SetActive(false);
            selection.gameObject.SetActive(false);
            anchor.gameObject.SetActive(spec.isPublic);
        }
    }

    private static void RemapMaterials(Transform root)
    {
        EnsureFolder("Assets/Project/Art/MachineCubeBlender", "MaterialsV3");
        Material skin = CreateIndustrialMaterial("QF_V3_Unity_Skin",
            new Color(0.88f, 0.90f, 0.92f), 0.76f, 0.34f, 1.00f, 1.00f);
        Material plate = CreateIndustrialMaterial("QF_V3_Unity_Plate",
            new Color(0.78f, 0.80f, 0.82f), 0.80f, 0.30f, 1.30f, 1.05f);
        Material frame = CreateIndustrialMaterial("QF_V3_Unity_Frame",
            new Color(0.66f, 0.68f, 0.70f), 0.82f, 0.28f, 1.65f, 1.10f);
        Material node = CreateIndustrialMaterial("QF_V3_Unity_Node",
            new Color(0.80f, 0.77f, 0.70f), 0.78f, 0.31f, 1.45f, 1.05f);
        Material inner = CreateIndustrialMaterial("QF_V3_Unity_Inner",
            new Color(0.24f, 0.25f, 0.26f), 0.48f, 0.18f, 1.70f, 0.70f);
        Material torn = CreateIndustrialMaterial("QF_V3_Unity_Torn",
            new Color(0.54f, 0.48f, 0.40f), 0.72f, 0.26f, 1.55f, 1.20f);
        Material carbon = CreateIndustrialMaterial("QF_V3_Unity_Carbon",
            new Color(0.10f, 0.09f, 0.08f), 0.18f, 0.08f, 1.80f, 0.65f);
        Material copper = CreateIndustrialMaterial("QF_V3_Unity_Copper",
            new Color(0.45f, 0.18f, 0.055f), 0.86f, 0.32f, 1.55f, 0.90f);
        Material cyan = CreateEmissionMaterial("QF_V3_Unity_Cyan",
            new Color(0.00f, 0.55f, 0.82f), 4.2f);
        Material warning = CreateEmissionMaterial("QF_V3_Unity_Warning",
            new Color(1.00f, 0.025f, 0.055f), 3.8f);

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            Material[] slots = renderer.sharedMaterials;
            for (int i = 0; i < slots.Length; i++)
            {
                string key = NormalizeMaterialName(slots[i]);
                slots[i] = key switch
                {
                    "M3D_Face3Surface" => slots[i],
                    "QF_V3_DARK_SKIN" => skin,
                    "QF_V3_DARK_PLATE" => plate,
                    "QF_V3_DARK_FRAME" => frame,
                    "QF_V3_DARK_NODE" => node,
                    "C2_INNER_STRUCTURE" => inner,
                    "C2_CARBON_VOID" => carbon,
                    "C2_CHARRED_FRACTURE" => carbon,
                    "C2_MOTTLED_SOOT" => carbon,
                    "C2_PBR_TORN_48" => torn,
                    "C2_BURNT_COPPER" => copper,
                    "C2_EDGE_OXIDE" => copper,
                    "C2_EXPOSED_MACHINERY" => inner,
                    "C2_DEAD_CIRCUIT" => inner,
                    "C2_CYAN" => cyan,
                    "C2_AMBER" => warning,
                    "QF_NODE_METAL" => frame,
                    "QF_NODE_DARK" => inner,
                    "QF_NODE_CABLE" => carbon,
                    "QF_NODE_COPPER" => copper,
                    "QF_NODE_CYAN" => cyan,
                    "QF_NODE_WARNING" => warning,
                    _ => plate
                };
            }
            renderer.sharedMaterials = slots;
        }
    }

    private static Material CreateIndustrialMaterial(string name, Color tint,
        float metallic, float smoothness, float tiling, float bumpScale)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material template = AssetDatabase.LoadAssetAtPath<Material>(
            TemplateMaterialPath);
        if (template == null)
            throw new InvalidOperationException("Falta material URP base V3.");
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(template) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }
        else
        {
            material.CopyPropertiesFromMaterial(template);
            material.shader = template.shader;
            material.name = name;
        }
        Texture2D albedo = LoadTexture("QF_IndustrialDarkMetal_BaseColor_1024.png");
        Texture2D normal = LoadTexture("QF_IndustrialDarkMetal_Normal_1024.png");
        Texture2D ao = LoadTexture("QF_IndustrialDarkMetal_Occlusion_1024.png");
        if (material.HasProperty("_BaseMap"))
        {
            material.SetTexture("_BaseMap", albedo);
            material.SetTextureScale("_BaseMap", Vector2.one * tiling);
            material.SetColor("_BaseColor", tint);
        }
        if (material.HasProperty("_BumpMap"))
        {
            material.SetTexture("_BumpMap", normal);
            material.SetTextureScale("_BumpMap", Vector2.one * tiling);
            material.SetFloat("_BumpScale", bumpScale);
            material.EnableKeyword("_NORMALMAP");
        }
        if (material.HasProperty("_OcclusionMap"))
        {
            material.SetTexture("_OcclusionMap", ao);
            material.SetTextureScale("_OcclusionMap", Vector2.one * tiling);
            material.SetFloat("_OcclusionStrength", 0.90f);
            material.EnableKeyword("_OCCLUSIONMAP");
        }
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void AddDamageWarningGeometry(Transform damaged,
        NodeSpec spec)
    {
        Transform internalLed = damaged.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(item => item.name.Contains(
                "InternalWarningLED", StringComparison.Ordinal));
        if (internalLed != null)
        {
            Transform authoredRoot = new GameObject(
                "InternalDamageLight").transform;
            authoredRoot.gameObject.layer = PrototypeLayer;
            authoredRoot.SetParent(damaged, false);
            Vector3 authoredPosition = damaged.InverseTransformPoint(
                internalLed.position);
            CreateDamagePointLight(authoredRoot, spec, authoredPosition);
            return;
        }

        Material warning = AssetDatabase.LoadAssetAtPath<Material>(
            MaterialFolder + "/QF_V3_Unity_Warning.mat");
        if (warning == null)
            throw new InvalidOperationException("Falta warning material Face1.");

        Transform root = new GameObject("DamageWarningGeometry").transform;
        root.gameObject.layer = PrototypeLayer;
        root.SetParent(damaged, false);
        CreateWarningBox("DamageCoreGlow", root,
            new Vector3(0f, -0.82f, 0f),
            new Vector3(spec.width * 0.18f, 0.010f, spec.height * 0.060f),
            Quaternion.identity, warning);
        CreateWarningBox("DamageFractureGlow_A", root,
            new Vector3(-spec.width * 0.18f, -0.925f,
                spec.height * 0.13f),
            new Vector3(spec.width * 0.10f, 0.006f, 0.006f),
            Quaternion.Euler(0f, 34f, 0f), warning);
        CreateWarningBox("DamageFractureGlow_B", root,
            new Vector3(spec.width * 0.17f, -0.925f,
                -spec.height * 0.15f),
            new Vector3(spec.width * 0.08f, 0.006f, 0.006f),
            Quaternion.Euler(0f, -29f, 0f), warning);
        CreateDamagePointLight(root, spec,
            new Vector3(0f, -0.70f, 0f));
    }

    private static void CreateDamagePointLight(Transform parent, NodeSpec spec,
        Vector3 localPosition)
    {
        GameObject lightObject = new GameObject("DamageSpillLight");
        lightObject.layer = PrototypeLayer;
        lightObject.transform.SetParent(parent, false);
        lightObject.transform.localPosition = localPosition;
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.035f, 0.012f);
        light.intensity = 1.25f;
        light.range = Mathf.Clamp(Mathf.Max(spec.width, spec.height) * 0.56f,
            0.46f, 0.70f);
        light.shadows = LightShadows.None;
        light.cullingMask = 1 << PrototypeLayer;
        light.renderMode = LightRenderMode.ForcePixel;
    }

    private static void CreateWarningBox(string name, Transform parent,
        Vector3 localPosition, Vector3 localScale, Quaternion localRotation,
        Material material)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.layer = PrototypeLayer;
        box.transform.SetParent(parent, false);
        box.transform.localPosition = localPosition;
        box.transform.localRotation = localRotation;
        box.transform.localScale = localScale;
        UnityEngine.Object.DestroyImmediate(box.GetComponent<Collider>());
        box.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void ReplaceStatusStrip(Transform state, string namePrefix,
        float targetWidth)
    {
        Transform original = state.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(item => item.name.StartsWith(namePrefix,
                StringComparison.Ordinal));
        Renderer source = original != null
            ? original.GetComponentInChildren<Renderer>(true)
            : null;
        if (source == null)
            throw new InvalidOperationException("Falta barra original " +
                namePrefix + " en " + state.name);

        Vector3 center = state.InverseTransformPoint(
            source.transform.TransformPoint(source.localBounds.center));
        Quaternion rotation = Quaternion.Inverse(state.rotation) *
            source.transform.rotation;
        Material material = source.sharedMaterial;
        Renderer[] originalRenderers = original
            .GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < originalRenderers.Length; i++)
            originalRenderers[i].enabled = false;

        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line.name = "UnifiedStatusLine";
        line.layer = PrototypeLayer;
        line.transform.SetParent(state, false);
        line.transform.localPosition = center;
        line.transform.localRotation = rotation;
        line.transform.localScale = Vector3.one;
        float inheritedX = Vector3.Distance(
            line.transform.TransformPoint(Vector3.left * 0.5f),
            line.transform.TransformPoint(Vector3.right * 0.5f));
        float inheritedY = Vector3.Distance(
            line.transform.TransformPoint(Vector3.down * 0.5f),
            line.transform.TransformPoint(Vector3.up * 0.5f));
        float inheritedZ = Vector3.Distance(
            line.transform.TransformPoint(Vector3.back * 0.5f),
            line.transform.TransformPoint(Vector3.forward * 0.5f));
        line.transform.localScale = new Vector3(
            targetWidth / Mathf.Max(0.0001f, inheritedX),
            0.035f / Mathf.Max(0.0001f, inheritedY),
            0.032f / Mathf.Max(0.0001f, inheritedZ));
        UnityEngine.Object.DestroyImmediate(line.GetComponent<Collider>());
        line.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void NormalizeRendererWorldWidth(Renderer renderer,
        float targetWidth)
    {
        Bounds bounds = renderer.localBounds;
        Vector3 localStart = bounds.center - Vector3.right * bounds.extents.x;
        Vector3 localEnd = bounds.center + Vector3.right * bounds.extents.x;
        float currentWidth = Vector3.Distance(
            renderer.transform.TransformPoint(localStart),
            renderer.transform.TransformPoint(localEnd));
        Vector3 scale = renderer.transform.localScale;
        scale.x *= targetWidth / Mathf.Max(0.0001f, currentWidth);
        renderer.transform.localScale = scale;
    }

    private static Material CreateEmissionMaterial(string name, Color color,
        float intensity)
    {
        Material material = CreateIndustrialMaterial(name, color * 0.12f,
            0.25f, 0.40f, 1f, 0.35f);
        if (material.HasProperty("_EmissionColor"))
            material.SetColor("_EmissionColor", color * intensity);
        material.EnableKeyword("_EMISSION");
        material.SetKeyword(new UnityEngine.Rendering.LocalKeyword(
            material.shader, "_EMISSION"), true);
        material.globalIlluminationFlags =
            MaterialGlobalIlluminationFlags.RealtimeEmissive;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Texture2D LoadTexture(string fileName)
    {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(
            TextureFolder + fileName);
        if (texture == null)
            throw new InvalidOperationException("Falta textura V3: " + fileName);
        return texture;
    }

    private static void ValidateAuthoredFace(GameObject root, Transform face,
        Transform visual, NodeSpec[] specs, string label)
    {
        Transform anchors = FindDeepChild(visual, "NODE_ANCHORS");
        MachineCube3DNode[] nodes = anchors.GetComponentsInChildren<
            MachineCube3DNode>(true);
        BoxCollider[] colliders = anchors.GetComponentsInChildren<BoxCollider>(true);
        int publicNodes = nodes.Count(item => item.gameObject.activeSelf);
        int expectedPublic = specs.Count(item => item.isPublic);
        if (nodes.Length != specs.Length || colliders.Length != specs.Length ||
            publicNodes != expectedPublic)
            throw new InvalidOperationException(
                $"Rig {label} inválido: nodes={nodes.Length}, " +
                $"colliders={colliders.Length}, public={publicNodes}");
        foreach (MachineCube3DNode node in nodes)
        {
            Transform light = node.transform.Find("SelectionFeedback");
            if (light == null || light.GetComponentsInChildren<Renderer>(true).Length != 1)
                throw new InvalidOperationException(
                    "La selección debe ser una sola luz en " + node.name);
        }
        if (root.GetComponentsInChildren<MachineCube3DNode>(true).Length != 43 ||
            root.GetComponentsInChildren<Collider>(true).Length != 43)
            throw new InvalidOperationException(
                $"El reemplazo {label} alteró el rig 43/43.");
        if (face.GetComponent<MachineCube3DFace>() == null)
            throw new InvalidOperationException(
                label + " perdió MachineCube3DFace.");
        Bounds bounds = CalculateLocalBounds(face, visual);
        if (bounds.size.x < 7.4f || bounds.size.y < 7.4f ||
            bounds.size.z > 3.0f)
        {
            throw new InvalidOperationException(
                $"Orientación {label} inválida: bounds={bounds.size:F3}");
        }
    }

    private static string NormalizeMaterialName(Material material)
    {
        string value = material != null ? material.name : string.Empty;
        int dot = value.IndexOf('.');
        return dot > 0 ? value.Substring(0, dot) : value;
    }

    private static void DisableRenderers(Transform root)
    {
        if (root == null)
            return;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            renderer.enabled = false;
    }

    private static void ConfigureReferenceBackplate(Transform cube,
        Transform targetFace, Transform authoredVisual)
    {
        Transform targetStructure = FindDeepChild(targetFace, "StructureModules");
        DisableRenderers(targetStructure);
        Transform targetBackplate = FindDeepChild(targetStructure,
            "PhysicalMetalBackplate");
        Transform referenceFace = FindDeepChild(cube, "PhysicalFace_3");
        Transform referenceBackplate = FindDeepChild(referenceFace,
            "PhysicalMetalBackplate");
        Renderer targetRenderer = targetBackplate != null
            ? targetBackplate.GetComponent<Renderer>() : null;
        Renderer referenceRenderer = referenceBackplate != null
            ? referenceBackplate.GetComponent<Renderer>() : null;
        if (targetRenderer == null || referenceRenderer == null)
            throw new InvalidOperationException(
                "No se pudo compartir el acabado de la Cara 3 con la Cara 1.");

        targetRenderer.sharedMaterials = referenceRenderer.sharedMaterials;
        targetRenderer.enabled = true;

        Material referenceSurface = referenceRenderer.sharedMaterials[0];
        foreach (Renderer renderer in
            authoredVisual.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials = renderer.sharedMaterials;
            bool changed = false;
            for (int i = 0; i < materials.Length; i++)
            {
                if (NormalizeMaterialName(materials[i]) != "QF_V3_DARK_SKIN")
                    continue;
                materials[i] = referenceSurface;
                changed = true;
            }
            if (changed)
            {
                renderer.sharedMaterials = materials;
                renderer.enabled = true;
            }
        }

        Transform repairPatches = FindDeepChild(targetFace,
            "FaceRepairPatches");
        if (repairPatches != null)
        {
            foreach (Renderer patchRenderer in
                repairPatches.GetComponentsInChildren<Renderer>(true))
            {
                Material[] patchMaterials = patchRenderer.sharedMaterials;
                for (int i = 0; i < patchMaterials.Length; i++)
                    patchMaterials[i] = referenceSurface;
                patchRenderer.sharedMaterials = patchMaterials;
                patchRenderer.enabled = true;
            }
        }
    }

    private static Bounds CalculateLocalBounds(Transform face, Transform visual)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>(true);
        bool initialized = false;
        Bounds result = new Bounds(Vector3.zero, Vector3.zero);
        foreach (Renderer renderer in renderers)
        {
            Bounds worldBounds = renderer.bounds;
            Vector3 min = worldBounds.min;
            Vector3 max = worldBounds.max;
            for (int corner = 0; corner < 8; corner++)
            {
                Vector3 world = new Vector3(
                    (corner & 1) == 0 ? min.x : max.x,
                    (corner & 2) == 0 ? min.y : max.y,
                    (corner & 4) == 0 ? min.z : max.z);
                Vector3 local = face.InverseTransformPoint(world);
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

    private static void EnsureFolder(string parent, string name)
    {
        string path = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, name);
    }

    private static Transform FindChildStartingWith(Transform root, string prefix)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name.StartsWith(prefix, StringComparison.Ordinal))
                return child;
        }
        return null;
    }

    private static Transform FindDeepChild(Transform root, string name)
    {
        if (root == null)
            return null;
        if (root.name == name)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindDeepChild(root.GetChild(i), name);
            if (found != null)
                return found;
        }
        return null;
    }

    private static void SetLayerRecursively(GameObject root, int layer)
    {
        root.layer = layer;
        for (int i = 0; i < root.transform.childCount; i++)
            SetLayerRecursively(root.transform.GetChild(i).gameObject, layer);
    }

    private readonly struct NodeSpec
    {
        public readonly string id;
        public readonly float width;
        public readonly float height;
        public readonly bool isPublic;

        public NodeSpec(string id, float width, float height, bool isPublic)
        {
            this.id = id;
            this.width = width;
            this.height = height;
            this.isPublic = isPublic;
        }
    }
}
#endif
