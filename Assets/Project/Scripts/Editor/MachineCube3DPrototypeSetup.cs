#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MachineCube3DPrototypeSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string AssetFolder = "Assets/Project/UI/Vertical/Machine/Prototype3D";
    private const string RenderTexturePath = AssetFolder + "/MachineCube3DPrototype.renderTexture";
    private const string Face1TexturePath =
        "Assets/Project/UI/Vertical/Machine/machine_face_1_reference_v2.png";
    private const string Face2TexturePath =
        "Assets/Project/UI/Vertical/Machine/machine_face_2_reference_v2.png";
    private const int PrototypeLayer = 30;

    private static readonly Vector2[] Face1NodePositions =
    {
        P(.21f,.75f), P(.72f,.74f), P(.20f,.52f), P(.50f,.50f),
        P(.74f,.53f), P(.20f,.28f), P(.71f,.27f)
    };

    private static readonly string[] Face1NodeIds =
    {
        "z1_energy_coupling_1", "z1_traces_channel_1", "z1_protocol_reading",
        "z1_triangle_anchor_1", "z1_artifact_calibration", "z1_flow_distributor",
        "z1_room1_synchronizer"
    };

    private static readonly Vector2[] Face2NodePositions =
    {
        P(.20f,.76f), P(.49f,.77f), P(.74f,.73f), P(.18f,.54f), P(.49f,.53f)
    };

    private static readonly string[] Face2NodeIds =
    {
        "z2_fusion_table", "z2_fusion_slot_2", "z2_mix_stabilizer_1",
        "z2_composition_reading", "z2_fusion_slot_3"
    };

    [MenuItem("Tools/Quantum Forge/Machine/Configure True 3D Prototype")]
    public static void ConfigurePrototype()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        EnsureFolder(AssetFolder);

        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "MachinePanelUI no disponible en Main.unity.");
        Transform visualRoot = panel.transform.Find("MachineCubeVisualRoot");
        Require(visualRoot != null, "Falta MachineCubeVisualRoot.");
        Transform viewport = visualRoot.Find("FaceViewport");
        Require(viewport != null, "Falta FaceViewport.");

        DeleteSceneObject("MachineCube3DPrototypeRoot");
        Transform previousDisplay = viewport.Find("MachineCube3DPrototypeDisplay");
        if (previousDisplay != null)
            UnityEngine.Object.DestroyImmediate(previousDisplay.gameObject);

        RenderTexture target = CreateOrUpdateRenderTexture();
        Texture2D face1Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Face1TexturePath);
        Texture2D face2Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Face2TexturePath);
        Require(face1Texture != null && face2Texture != null,
            "Faltan texturas de referencia para el prototipo 3D.");

        PrototypeMaterials materials = CreateMaterials(face1Texture, face2Texture);

        GameObject displayObject = CreateRect("MachineCube3DPrototypeDisplay", viewport,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        RawImage display = displayObject.AddComponent<RawImage>();
        display.texture = target;
        display.color = Color.white;
        display.raycastTarget = true;
        displayObject.SetActive(false);
        displayObject.transform.SetAsLastSibling();

        GameObject prototypeRoot = new GameObject("MachineCube3DPrototypeRoot");
        prototypeRoot.layer = PrototypeLayer;
        prototypeRoot.transform.position = new Vector3(0f, 1000f, 0f);
        MachineCube3DPrototypeController controller =
            prototypeRoot.AddComponent<MachineCube3DPrototypeController>();
        MachineCube3DVisualStateController visualState =
            prototypeRoot.AddComponent<MachineCube3DVisualStateController>();

        Transform physicalCube = new GameObject("PhysicalCubeRoot").transform;
        physicalCube.SetParent(prototypeRoot.transform, false);
        SetLayerRecursively(physicalCube.gameObject, PrototypeLayer);

        CreateBox("CubeCore", physicalCube, Vector3.zero,
            new Vector3(7.45f, 7.45f, 7.45f), Quaternion.identity, materials.body);
        BuildPhysicalFace(physicalCube, 0, new Vector3(0f, 0f, 3.82f),
            Quaternion.identity, materials.face1, materials, Face1NodePositions,
            Face1NodeIds, materials.blueEmission);
        BuildPhysicalFace(physicalCube, 1, new Vector3(3.82f, 0f, 0f),
            Quaternion.Euler(0f, 90f, 0f), materials.face2, materials,
            Face2NodePositions, Face2NodeIds, materials.purpleEmission);

        Camera camera = BuildCamera(prototypeRoot.transform, target);
        BuildLights(prototypeRoot.transform);

        MachineCube3DPrototypeDisplayUI displayInput =
            displayObject.AddComponent<MachineCube3DPrototypeDisplayUI>();
        MachineCubeVisualUI machineVisual = panel.GetComponent<MachineCubeVisualUI>();
        SerializedObject inputSo = new SerializedObject(displayInput);
        SetObject(inputSo, "controller", controller);
        SetObject(inputSo, "machineVisual", machineVisual);
        inputSo.FindProperty("nodeLayer").intValue = 1 << PrototypeLayer;
        inputSo.FindProperty("swipeThresholdPixels").floatValue = 64f;
        inputSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject controllerSo = new SerializedObject(controller);
        SetObject(controllerSo, "cubeRoot", physicalCube);
        SetObject(controllerSo, "prototypeCamera", camera);
        SetObject(controllerSo, "targetTexture", target);
        SetObject(controllerSo, "display", display);
        controllerSo.FindProperty("rotationDuration").floatValue = 0.72f;
        controllerSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject stateSo = new SerializedObject(visualState);
        SetObject(stateSo, "prototypeController", controller);
        stateSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject visualSo = new SerializedObject(machineVisual);
        SetObject(visualSo, "true3DController", controller);
        visualSo.FindProperty("useTrue3DForBuiltFaces").boolValue = true;
        visualSo.ApplyModifiedPropertiesWithoutUndo();

        ConfigureGraphicsSettingsUI();

        controller.SetFaceImmediate(0);
        controller.ShowPrototype(false);

        EditorUtility.SetDirty(prototypeRoot);
        EditorUtility.SetDirty(displayObject);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Machine Cube True 3D Prototype] CONFIGURED | physical cube | " +
            "2 connected faces | 12 raised nodes | perspective camera | URP materials");
    }

    public static void ConfigurePrototypeBatch()
    {
        ConfigurePrototype();
        ValidatePrototype();
    }

    public static void ValidatePrototype()
    {
        GameObject root = FindSceneObject("MachineCube3DPrototypeRoot");
        Require(root != null, "Falta MachineCube3DPrototypeRoot.");
        MachineCube3DPrototypeController controller =
            root.GetComponent<MachineCube3DPrototypeController>();
        Require(controller != null, "Falta MachineCube3DPrototypeController.");
        Require(root.GetComponent<MachineCube3DVisualStateController>() != null,
            "Falta MachineCube3DVisualStateController.");
        Require(root.transform.Find("PhysicalCubeRoot/PhysicalFace_1") != null,
            "Falta la Cara 1 fÃ­sica.");
        Require(root.transform.Find("PhysicalCubeRoot/PhysicalFace_2") != null,
            "Falta la Cara 2 fÃ­sica.");
        Require(root.GetComponentsInChildren<MeshRenderer>(true).Length >= 70,
            "El prototipo no contiene suficiente geometrÃ­a de relieve.");
        Require(root.GetComponentsInChildren<MachineCube3DNode>(true).Length == 12,
            "El prototipo debe contener 12 nodos 3D independientes.");
        Require(root.GetComponentsInChildren<Collider>(true).Length == 12,
            "Solo los 12 nodos deben conservar colliders.");
        Camera camera = root.GetComponentInChildren<Camera>(true);
        Require(camera != null && !camera.orthographic,
            "La cÃ¡mara del prototipo debe usar perspectiva.");
        RenderTexture target = AssetDatabase.LoadAssetAtPath<RenderTexture>(RenderTexturePath);
        Require(target != null && target.width == 1024 && target.height == 1024,
            "El RenderTexture del prototipo no es 1024x1024.");
        RawImage display = UnityEngine.Object.FindObjectsByType<RawImage>(
                FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(image => image.name == "MachineCube3DPrototypeDisplay");
        Require(display != null && display.texture == target &&
            !display.gameObject.activeSelf,
            "La salida 3D debe existir y permanecer oculta fuera de la prueba.");
        VerticalSettingsPanelUI settings =
            UnityEngine.Object.FindFirstObjectByType<VerticalSettingsPanelUI>(
                FindObjectsInactive.Include);
        Require(settings != null && settings.lowQualityButton != null &&
            settings.balancedQualityButton != null &&
            settings.highQualityButton != null && settings.currentQualityText != null,
            "Faltan los controles de calidad del cubo en Ajustes.");
        Debug.Log("[Machine Cube True 3D Prototype] PASS | real geometry | " +
            "2 faces | 12 nodes | progressive damage | 3 quality profiles");
    }

    private static void BuildPhysicalFace(Transform cubeRoot, int faceIndex,
        Vector3 localPosition, Quaternion localRotation, Material surfaceMaterial,
        PrototypeMaterials materials, IReadOnlyList<Vector2> nodePositions,
        IReadOnlyList<string> nodeIds, Material emissionMaterial)
    {
        GameObject faceObject = new GameObject("PhysicalFace_" + (faceIndex + 1));
        faceObject.layer = PrototypeLayer;
        Transform face = faceObject.transform;
        face.SetParent(cubeRoot, false);
        face.localPosition = localPosition;
        face.localRotation = localRotation;

        CreateBox("TexturedMetalPlate", face, new Vector3(0f, 0f, 0.02f),
            new Vector3(7.55f, 7.55f, 0.22f), Quaternion.identity, surfaceMaterial);
        CreateBox("RearArmor", face, new Vector3(0f, 0f, -0.20f),
            new Vector3(7.82f, 7.82f, 0.30f), Quaternion.identity, materials.body);

        BuildOuterFrame(face, materials, emissionMaterial);
        BuildPanelRelief(face, materials);
        BuildFaceStateDetails(face, materials, emissionMaterial, faceIndex);

        for (int i = 0; i < nodePositions.Count && i < nodeIds.Count; i++)
        {
            Vector3 position = ToFacePosition(nodePositions[i], 0.14f);
            CreateShortChannels(face, position, materials.channel, i);
            BuildNode(face, faceIndex, i, nodeIds[i], nodePositions[i], position,
                surfaceMaterial, materials, emissionMaterial);
        }
    }

    private static void BuildOuterFrame(Transform face, PrototypeMaterials materials,
        Material emissionMaterial)
    {
        CreateBox("FrameTopBackbone", face, new Vector3(0f, 3.76f, 0.08f),
            new Vector3(7.46f, 0.18f, 0.62f), Quaternion.identity, materials.body);
        CreateBox("FrameBottomBackbone", face, new Vector3(0f, -3.76f, 0.08f),
            new Vector3(7.46f, 0.18f, 0.62f), Quaternion.identity, materials.body);
        CreateBox("FrameLeftBackbone", face, new Vector3(-3.76f, 0f, 0.08f),
            new Vector3(0.18f, 7.46f, 0.62f), Quaternion.identity, materials.body);
        CreateBox("FrameRightBackbone", face, new Vector3(3.76f, 0f, 0.08f),
            new Vector3(0.18f, 7.46f, 0.62f), Quaternion.identity, materials.body);

        Vector2[] corners =
        {
            new Vector2(-3.18f, 3.18f), new Vector2(3.18f, 3.18f),
            new Vector2(-3.18f, -3.18f), new Vector2(3.18f, -3.18f)
        };
        for (int i = 0; i < corners.Length; i++)
        {
            CreateBox("ChamferCorner_" + (i + 1), face,
                new Vector3(corners[i].x * 1.12f, corners[i].y * 1.12f, 0.14f),
                new Vector3(0.62f, 0.18f, 0.66f),
                Quaternion.Euler(0f, 0f, i is 0 or 3 ? 45f : -45f), materials.frame);
        }
    }

    private static void BuildPanelRelief(Transform face, PrototypeMaterials materials)
    {
        float[] xPositions = { -1.55f, -1.41f, 1.32f, 1.46f };
        for (int i = 0; i < xPositions.Length; i++)
        {
            CreateCylinder("StructuralConduit_" + (i + 1), face,
                new Vector3(xPositions[i], 0f, 0.20f),
                new Vector3(0.035f, 2.65f, 0.035f), Quaternion.identity,
                materials.trim);
        }
        for (int i = -2; i <= 2; i++)
        {
            float y = i * 1.18f;
            CreateBox("ArmorJoinLeft_" + (i + 3), face,
                new Vector3(-2.33f, y, 0.15f), new Vector3(0.42f, 0.035f, 0.04f),
                Quaternion.identity, materials.channel);
            CreateBox("ArmorJoinRight_" + (i + 3), face,
                new Vector3(2.33f, y, 0.15f), new Vector3(0.42f, 0.035f, 0.04f),
                Quaternion.identity, materials.channel);
        }
    }

    private static void CreateShortChannels(Transform face, Vector3 nodePosition,
        Material channelMaterial, int index)
    {
        float direction = nodePosition.y >= 0f ? -1f : 1f;
        float length = 0.55f + (index % 3) * 0.12f;
        for (int lane = -1; lane <= 1; lane++)
        {
            CreateBox($"NodeChannel_{index + 1}_{lane + 2}", face,
                new Vector3(nodePosition.x + lane * 0.085f,
                    nodePosition.y + direction * (0.58f + length * 0.5f), 0.16f),
                new Vector3(0.035f, length, 0.045f), Quaternion.identity,
                channelMaterial);
        }
    }

    private static void BuildNode(Transform face, int faceIndex, int nodeIndex,
        string nodeId, Vector2 normalizedPosition, Vector3 position,
        Material surfaceMaterial, PrototypeMaterials materials, Material emissionMaterial)
    {
        GameObject root = new GameObject($"Node3D_{nodeIndex + 1:00}_{nodeId}");
        root.layer = PrototypeLayer;
        root.transform.SetParent(face, false);
        root.transform.localPosition = position;
        MachineCube3DNode node = root.AddComponent<MachineCube3DNode>();
        SerializedObject nodeSo = new SerializedObject(node);
        nodeSo.FindProperty("nodeId").stringValue = nodeId;
        nodeSo.FindProperty("faceIndex").intValue = faceIndex;
        nodeSo.ApplyModifiedPropertiesWithoutUndo();

        Vector2 dimensions = GetNodeDimensions(faceIndex, nodeIndex);
        float width = dimensions.x;
        float height = dimensions.y;
        CreateBox("HousingDepth", root.transform, new Vector3(0f, -0.015f, 0.07f),
            new Vector3(width * 0.96f, height * 0.96f, 0.12f), Quaternion.identity,
            materials.module);

        Material patchMaterial = CreatePatchMaterial(surfaceMaterial, faceIndex,
            nodeIndex, normalizedPosition, new Vector2(width / 7.55f, height / 7.55f));
        CreateBox("ReferenceSurface", root.transform,
            new Vector3(0f, 0f, 0.14f), new Vector3(width, height, 0.025f),
            Quaternion.identity, patchMaterial);

        BuildNodeDepthDetails(root.transform, width, height, materials,
            emissionMaterial);

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0f, 0.08f);
        collider.size = new Vector3(width * 1.16f, height * 1.16f, 0.42f);
    }

    private static void BuildNodeDepthDetails(Transform root, float width, float height,
        PrototypeMaterials materials, Material operationalMaterial)
    {
        float rim = 0.035f;
        CreateBox("DepthRimTop", root, new Vector3(0f, height * 0.49f, 0.16f),
            new Vector3(width * 0.76f, rim, 0.045f), Quaternion.identity, materials.trim);
        CreateBox("DepthRimBottom", root, new Vector3(0f, -height * 0.49f, 0.16f),
            new Vector3(width * 0.76f, rim, 0.045f), Quaternion.identity, materials.trim);
        CreateBox("DepthRimLeft", root, new Vector3(-width * 0.49f, 0f, 0.16f),
            new Vector3(rim, height * 0.76f, 0.045f), Quaternion.identity, materials.trim);
        CreateBox("DepthRimRight", root, new Vector3(width * 0.49f, 0f, 0.16f),
            new Vector3(rim, height * 0.76f, 0.045f), Quaternion.identity, materials.trim);

        CreateBox("OperationalEmitter", root,
            new Vector3(0f, -height * 0.49f, 0.205f),
            new Vector3(Mathf.Min(0.19f, width * 0.20f), 0.035f, 0.025f),
            Quaternion.identity, operationalMaterial);

        GameObject damageDetails = new GameObject("DamageDetails");
        damageDetails.layer = PrototypeLayer;
        damageDetails.transform.SetParent(root, false);
        CreateBox("DamageScar_1", damageDetails.transform,
            new Vector3(-width * 0.12f, height * 0.08f, 0.185f),
            new Vector3(width * 0.28f, 0.022f, 0.018f),
            Quaternion.Euler(0f, 0f, 24f), materials.damageMark);
        CreateBox("DamageScar_2", damageDetails.transform,
            new Vector3(width * 0.07f, -height * 0.06f, 0.186f),
            new Vector3(width * 0.20f, 0.018f, 0.018f),
            Quaternion.Euler(0f, 0f, -38f), materials.damageMark);
        CreateBox("DamageEmitter", damageDetails.transform,
            new Vector3(width * 0.43f, height * 0.43f, 0.205f),
            new Vector3(0.075f, 0.035f, 0.025f), Quaternion.identity,
            materials.warningEmission);

        GameObject repairEmitter = CreateBox("RepairEmitter", root,
            new Vector3(-width * 0.43f, height * 0.43f, 0.205f),
            new Vector3(0.095f, 0.035f, 0.025f), Quaternion.identity,
            materials.repairEmission);
        repairEmitter.SetActive(false);
    }

    private static void BuildFaceStateDetails(Transform face, PrototypeMaterials materials,
        Material accentMaterial, int faceIndex)
    {
        GameObject damage = new GameObject("FaceDamageDetails");
        damage.layer = PrototypeLayer;
        damage.transform.SetParent(face, false);
        Vector2[] scarCenters = faceIndex == 0
            ? new[] { new Vector2(-1.0f, 1.35f), new Vector2(1.55f, -0.55f),
                new Vector2(-1.75f, -1.55f) }
            : new[] { new Vector2(1.15f, 1.40f), new Vector2(-1.45f, -0.65f),
                new Vector2(1.65f, -1.55f) };
        for (int i = 0; i < scarCenters.Length; i++)
        {
            CreateBox("FaceScar_" + (i + 1), damage.transform,
                new Vector3(scarCenters[i].x, scarCenters[i].y, 0.155f),
                new Vector3(0.42f, 0.025f, 0.018f),
                Quaternion.Euler(0f, 0f, i % 2 == 0 ? 28f : -34f),
                materials.damageMark);
        }

        GameObject warnings = new GameObject("FaceWarningLights");
        warnings.layer = PrototypeLayer;
        warnings.transform.SetParent(face, false);
        Vector2[] warningPositions =
        {
            new Vector2(-2.75f, 2.82f), new Vector2(2.75f, 2.82f),
            new Vector2(-2.75f, -2.82f), new Vector2(2.75f, -2.82f)
        };
        for (int i = 0; i < warningPositions.Length; i++)
        {
            CreateBox("WarningLight_" + (i + 1), warnings.transform,
                new Vector3(warningPositions[i].x, warningPositions[i].y, 0.19f),
                new Vector3(0.11f, 0.035f, 0.025f), Quaternion.identity,
                materials.warningEmission);
        }

        GameObject repairs = new GameObject("FaceRepairLights");
        repairs.layer = PrototypeLayer;
        repairs.transform.SetParent(face, false);
        Vector2[] repairPositions =
        {
            new Vector2(-2.05f, 2.94f), new Vector2(0f, 2.94f),
            new Vector2(2.05f, 2.94f), new Vector2(2.94f, 1.10f),
            new Vector2(2.94f, -1.10f), new Vector2(2.05f, -2.94f),
            new Vector2(0f, -2.94f), new Vector2(-2.05f, -2.94f)
        };
        for (int i = 0; i < repairPositions.Length; i++)
        {
            bool vertical = Mathf.Abs(repairPositions[i].x) > 2.9f;
            CreateBox("RepairLight_" + (i + 1), repairs.transform,
                new Vector3(repairPositions[i].x, repairPositions[i].y, 0.19f),
                vertical ? new Vector3(0.035f, 0.16f, 0.025f)
                    : new Vector3(0.16f, 0.035f, 0.025f),
                Quaternion.identity, accentMaterial);
        }
    }

    private static Vector2 GetNodeDimensions(int faceIndex, int nodeIndex)
    {
        Vector2[] face1 =
        {
            new Vector2(1.30f, 1.26f), new Vector2(1.18f, 1.14f),
            new Vector2(1.38f, 1.12f), new Vector2(1.30f, 1.24f),
            new Vector2(0.96f, 1.02f), new Vector2(1.18f, 1.12f),
            new Vector2(1.38f, 1.02f)
        };
        Vector2[] face2 =
        {
            new Vector2(1.28f, 1.30f), new Vector2(1.48f, 0.94f),
            new Vector2(1.25f, 1.24f), new Vector2(0.96f, 1.05f),
            new Vector2(1.42f, 1.32f)
        };
        Vector2[] source = faceIndex == 0 ? face1 : face2;
        return source[Mathf.Clamp(nodeIndex, 0, source.Length - 1)];
    }

    private static Material CreatePatchMaterial(Material surfaceMaterial, int faceIndex,
        int nodeIndex, Vector2 center,
        Vector2 normalizedSize)
    {
        Vector2 cropOffset = new Vector2(0.04f, 0.045f);
        Vector2 cropScale = new Vector2(0.81f, 0.945f);
        Vector2 sourceSize = Vector2.Scale(normalizedSize, cropScale);
        Vector2 sourceOffset = cropOffset + Vector2.Scale(
            center - normalizedSize * 0.5f, cropScale);
        string materialName = $"M3D_F{faceIndex + 1}_Node_{nodeIndex + 1:00}";
        string path = AssetFolder + "/" + materialName + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(surfaceMaterial.shader) { name = materialName };
            AssetDatabase.CreateAsset(material, path);
        }
        material.CopyPropertiesFromMaterial(surfaceMaterial);
        material.name = materialName;
        material.SetTextureScale("_BaseMap", sourceSize);
        material.SetTextureOffset("_BaseMap", sourceOffset);
        if (material.HasProperty("_MainTex"))
        {
            material.SetTextureScale("_MainTex", sourceSize);
            material.SetTextureOffset("_MainTex", sourceOffset);
        }
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Camera BuildCamera(Transform parent, RenderTexture target)
    {
        GameObject cameraObject = new GameObject("MachineCube3DPrototypeCamera");
        cameraObject.layer = PrototypeLayer;
        cameraObject.transform.SetParent(parent, false);
        cameraObject.transform.localPosition = new Vector3(0f, 0f, 20f);
        cameraObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.002f, 0.006f, 0.009f, 1f);
        camera.orthographic = false;
        camera.fieldOfView = 36f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 60f;
        camera.cullingMask = 1 << PrototypeLayer;
        camera.allowHDR = true;
        camera.allowMSAA = true;
        camera.targetTexture = target;
        camera.depth = -20f;
        camera.enabled = false;
        return camera;
    }

    private static void BuildLights(Transform parent)
    {
        GameObject keyObject = new GameObject("IndustrialKeyLight");
        keyObject.layer = PrototypeLayer;
        keyObject.transform.SetParent(parent, false);
        keyObject.transform.localRotation = Quaternion.Euler(32f, 145f, 0f);
        Light key = keyObject.AddComponent<Light>();
        key.type = LightType.Directional;
        key.color = new Color(0.72f, 0.86f, 1f);
        key.intensity = 2.0f;
        key.cullingMask = 1 << PrototypeLayer;
        key.shadows = LightShadows.Soft;
        key.shadowResolution = LightShadowResolution.Low;

        GameObject fillObject = new GameObject("IndustrialFillLight");
        fillObject.layer = PrototypeLayer;
        fillObject.transform.SetParent(parent, false);
        fillObject.transform.localRotation = Quaternion.Euler(-15f, 210f, 0f);
        Light fill = fillObject.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.color = new Color(0.26f, 0.48f, 0.62f);
        fill.intensity = 0.8f;
        fill.cullingMask = 1 << PrototypeLayer;
        fill.shadows = LightShadows.None;
    }

    private static PrototypeMaterials CreateMaterials(Texture2D face1, Texture2D face2)
    {
        return new PrototypeMaterials
        {
            body = CreateMaterial("M3D_Body", new Color(0.05f, 0.06f, 0.07f),
                0.88f, 0.28f),
            frame = CreateMaterial("M3D_Frame", new Color(0.075f, 0.082f, 0.09f),
                0.92f, 0.30f),
            trim = CreateMaterial("M3D_Trim", new Color(0.10f, 0.11f, 0.12f),
                0.94f, 0.48f),
            module = CreateMaterial("M3D_Module", new Color(0.055f, 0.065f, 0.075f),
                0.86f, 0.26f),
            channel = CreateMaterial("M3D_Channel", new Color(0.018f, 0.035f, 0.045f),
                0.48f, 0.24f),
            blueEmission = CreateMaterial("M3D_BlueEmission",
                new Color(0.01f, 0.15f, 0.28f), 0.35f, 0.58f,
                new Color(0.01f, 0.58f, 1f) * 1.4f),
            purpleEmission = CreateMaterial("M3D_PurpleEmission",
                new Color(0.13f, 0.02f, 0.25f), 0.35f, 0.58f,
                new Color(0.62f, 0.08f, 1f) * 1.4f),
            damageMark = CreateMaterial("M3D_DamageMark",
                new Color(0.055f, 0.020f, 0.010f), 0.22f, 0.10f),
            warningEmission = CreateMaterial("M3D_WarningEmission",
                new Color(0.20f, 0.025f, 0.006f), 0.28f, 0.46f,
                new Color(1.00f, 0.12f, 0.012f) * 1.85f),
            repairEmission = CreateMaterial("M3D_RepairEmission",
                new Color(0.008f, 0.18f, 0.105f), 0.30f, 0.54f,
                new Color(0.04f, 1.00f, 0.56f) * 1.65f),
            face1 = CreateSurfaceMaterial("M3D_Face1Surface", face1,
                new Color(0.94f, 0.95f, 0.96f)),
            face2 = CreateSurfaceMaterial("M3D_Face2Surface", face2,
                new Color(0.92f, 0.90f, 0.96f))
        };
    }

    private static Material CreateMaterial(string name, Color baseColor,
        float metallic, float smoothness, Color? emission = null)
    {
        string path = AssetFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        Require(shader != null, "No se encontrÃ³ Universal Render Pipeline/Lit.");
        if (material == null)
        {
            material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }
        material.shader = shader;
        material.SetColor("_BaseColor", baseColor);
        material.SetFloat("_Metallic", metallic);
        material.SetFloat("_Smoothness", smoothness);
        if (emission.HasValue)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", emission.Value);
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        else
        {
            material.DisableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", Color.black);
        }
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Material CreateSurfaceMaterial(string name, Texture2D texture,
        Color tint)
    {
        Material material = CreateMaterial(name, tint, 0.72f, 0.30f);
        material.SetTexture("_BaseMap", texture);
        material.SetTextureOffset("_BaseMap", new Vector2(0.04f, 0.045f));
        material.SetTextureScale("_BaseMap", new Vector2(0.81f, 0.945f));
        EditorUtility.SetDirty(material);
        return material;
    }

    private static RenderTexture CreateOrUpdateRenderTexture()
    {
        RenderTexture target = AssetDatabase.LoadAssetAtPath<RenderTexture>(RenderTexturePath);
        if (target == null)
        {
            target = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32)
            {
                name = "MachineCube3DPrototype",
                antiAliasing = 2,
                useMipMap = false,
                autoGenerateMips = false,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            AssetDatabase.CreateAsset(target, RenderTexturePath);
        }
        return target;
    }

    private static GameObject CreateBox(string name, Transform parent, Vector3 position,
        Vector3 scale, Quaternion rotation, Material material)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.name = name;
        gameObject.layer = PrototypeLayer;
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
        gameObject.transform.localScale = scale;
        RemoveCollider(gameObject);
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.On;
        renderer.receiveShadows = true;
        return gameObject;
    }

    private static GameObject CreateCylinder(string name, Transform parent,
        Vector3 position, Vector3 scale, Quaternion rotation, Material material)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        gameObject.name = name;
        gameObject.layer = PrototypeLayer;
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
        gameObject.transform.localScale = scale;
        RemoveCollider(gameObject);
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.On;
        renderer.receiveShadows = true;
        return gameObject;
    }

    private static Vector3 ToFacePosition(Vector2 normalized, float z)
    {
        return new Vector3((normalized.x - 0.5f) * 7.55f,
            (normalized.y - 0.5f) * 7.55f, z);
    }

    private static void RemoveCollider(GameObject gameObject)
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider != null)
            UnityEngine.Object.DestroyImmediate(collider);
    }

    private static void SetLayerRecursively(GameObject root, int layer)
    {
        root.layer = layer;
        foreach (Transform child in root.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    private static void EnsureFolder(string path)
    {
        string normalized = path.Replace('\\', '/');
        string[] parts = normalized.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private static GameObject CreateRect(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = parent.gameObject.layer;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.localScale = Vector3.one;
        return gameObject;
    }

    private static void ConfigureGraphicsSettingsUI()
    {
        VerticalSettingsPanelUI settings =
            UnityEngine.Object.FindFirstObjectByType<VerticalSettingsPanelUI>(
                FindObjectsInactive.Include);
        Require(settings != null, "No se encontro VerticalSettingsPanelUI.");

        Transform existing = settings.transform.Find("Machine3DGraphicsCard");
        if (existing != null)
            UnityEngine.Object.DestroyImmediate(existing.gameObject);

        GameObject card = CreateRect("Machine3DGraphicsCard", settings.transform,
            new Vector2(0.5f, 0.27f), new Vector2(0.5f, 0.27f),
            new Vector2(-410f, -185f), new Vector2(410f, 185f));
        Image languageCard = settings.transform.Find("LanguageCard")
            ?.GetComponent<Image>();
        Image cardImage = card.AddComponent<Image>();
        cardImage.sprite = languageCard != null ? languageCard.sprite : null;
        cardImage.type = cardImage.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        cardImage.color = languageCard != null
            ? languageCard.color
            : new Color(0.025f, 0.055f, 0.070f, 0.96f);

        TMP_FontAsset font = settings.currentLanguageText != null
            ? settings.currentLanguageText.font
            : TMP_Settings.defaultFontAsset;
        TMP_Text title = CreateSettingsText("Machine3DQualityTitle", card.transform,
            "CALIDAD DEL CUBO 3D", font, 28f, new Color(0.15f, 0.86f, 1f),
            new Vector2(0.5f, 0.82f), new Vector2(720f, 54f));
        title.fontStyle = FontStyles.Bold;
        TMP_Text status = CreateSettingsText("CurrentMachine3DQuality", card.transform,
            "Calidad del cubo: Equilibrada", font, 22f, Color.white,
            new Vector2(0.5f, 0.63f), new Vector2(720f, 48f));
        CreateSettingsText("Machine3DQualityDescription", card.transform,
            "BAJA conserva el modelo 3D y reduce resolucion, sombras y refresco.",
            font, 16f, new Color(0.64f, 0.74f, 0.79f),
            new Vector2(0.5f, 0.49f), new Vector2(720f, 42f));

        Sprite buttonSprite = settings.spanishButton != null
            ? settings.spanishButton.GetComponent<Image>()?.sprite
            : null;
        Button low = CreateQualityButton("Machine3DQualityLow", card.transform,
            "BAJA", font, buttonSprite, new Vector2(0.19f, 0.22f));
        Button balanced = CreateQualityButton("Machine3DQualityBalanced", card.transform,
            "EQUILIBRADA", font, buttonSprite, new Vector2(0.50f, 0.22f));
        Button high = CreateQualityButton("Machine3DQualityHigh", card.transform,
            "ALTA", font, buttonSprite, new Vector2(0.81f, 0.22f));

        settings.lowQualityButton = low;
        settings.balancedQualityButton = balanced;
        settings.highQualityButton = high;
        settings.currentQualityText = status;
        EditorUtility.SetDirty(settings);
    }

    private static TMP_Text CreateSettingsText(string name, Transform parent,
        string content, TMP_FontAsset font, float fontSize, Color color,
        Vector2 anchor, Vector2 size)
    {
        GameObject textObject = CreateRect(name, parent, anchor, anchor,
            -size * 0.5f, size * 0.5f);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.font = font;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.raycastTarget = false;
        return text;
    }

    private static Button CreateQualityButton(string name, Transform parent,
        string label, TMP_FontAsset font, Sprite sprite, Vector2 anchor)
    {
        GameObject buttonObject = CreateRect(name, parent, anchor, anchor,
            new Vector2(-108f, -43f), new Vector2(108f, 43f));
        Image image = buttonObject.AddComponent<Image>();
        image.sprite = sprite;
        image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = sprite != null
            ? Color.white
            : new Color(0.04f, 0.13f, 0.17f, 1f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.45f, 0.92f, 1f);
        colors.pressedColor = new Color(0.18f, 0.70f, 0.86f);
        colors.disabledColor = new Color(0.12f, 0.74f, 0.92f, 0.72f);
        button.colors = colors;

        GameObject labelObject = CreateRect("Label", buttonObject.transform,
            Vector2.zero, Vector2.one, new Vector2(8f, 5f), new Vector2(-8f, -5f));
        TextMeshProUGUI text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.font = font;
        text.fontSize = 19f;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.raycastTarget = false;
        return button;
    }

    private static void DeleteSceneObject(string name)
    {
        GameObject existing = FindSceneObject(name);
        if (existing != null)
            UnityEngine.Object.DestroyImmediate(existing);
    }

    private static GameObject FindSceneObject(string name)
    {
        return Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(gameObject => gameObject.name == name &&
                gameObject.scene.IsValid() && gameObject.hideFlags == HideFlags.None);
    }

    private static void SetObject(SerializedObject serializedObject, string property,
        UnityEngine.Object value)
    {
        SerializedProperty target = serializedObject.FindProperty(property);
        Require(target != null, "Falta propiedad serializada: " + property);
        target.objectReferenceValue = value;
    }

    private static Vector2 P(float x, float y) => new Vector2(x, y);

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private sealed class PrototypeMaterials
    {
        public Material body;
        public Material frame;
        public Material trim;
        public Material module;
        public Material channel;
        public Material blueEmission;
        public Material purpleEmission;
        public Material damageMark;
        public Material warningEmission;
        public Material repairEmission;
        public Material face1;
        public Material face2;
    }
}
#endif
