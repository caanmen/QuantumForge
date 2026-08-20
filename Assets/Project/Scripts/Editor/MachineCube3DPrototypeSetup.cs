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
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MachineCube3DPrototypeSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";
    private const string AssetFolder = "Assets/Project/UI/Vertical/Machine/Prototype3D";
    private const string RenderTexturePath = AssetFolder + "/MachineCube3DPrototype.renderTexture";
    private const string DamageGlowProfilePath =
        AssetFolder + "/MachineCubeDamageGlow.asset";
    private const string WornMetalAlbedoPath =
        AssetFolder + "/M3D_WornMetal_Albedo.png";
    private const string WornMetalNormalPath =
        AssetFolder + "/M3D_WornMetal_NormalSource.png";
    private const string IndustrialMetalAlbedoPath =
        "Assets/Project/UI/Vertical/Machine/CustomTextures/" +
        "QF_IndustrialDarkMetal_v1/QF_IndustrialDarkMetal_BaseColor_1024.png";
    private const string IndustrialMetalNormalPath =
        "Assets/Project/UI/Vertical/Machine/CustomTextures/" +
        "QF_IndustrialDarkMetal_v1/QF_IndustrialDarkMetal_Normal_1024.png";
    private const string BeveledBoxMeshPath = AssetFolder + "/M3D_BeveledBox.asset";
    private const string Face1TexturePath =
        "Assets/Project/UI/Vertical/Machine/machine_face_1_base.png";
    private const string Face2TexturePath =
        "Assets/Project/UI/Vertical/Machine/machine_face_2_base.png";
    private const string Face3TexturePath =
        "Assets/Project/UI/Vertical/Machine/machine_face_3_base.png";
    private const string Face4TexturePath =
        "Assets/Project/UI/Vertical/Machine/machine_face_4_base.png";
    private const int PrototypeLayer = 30;
    private const int Face1PublicSocketCount = 7;
    private const int Face2PublicSocketCount = 11;
    private const int Face3PublicSocketCount = 7;
    private const int Face4PublicSocketCount = 10;
    private const int DamagedSkinGrid = 24;
    private const float DamagedSkinSize = 7.42f;
    private const float DamagedSkinFrontZ = 0.16f;
    private const float DamagedSkinBackZ = -0.16f;

    private static readonly Vector2[] Face1NodePositions =
    {
        P(.21f,.75f), P(.72f,.74f), P(.20f,.52f), P(.50f,.50f),
        P(.74f,.53f), P(.71f,.27f), P(.50f,.13f), P(.20f,.28f),
        P(.50f,.82f)
    };

    private static readonly string[] Face1NodeIds =
    {
        "z1_energy_coupling_1", "z1_traces_channel_1", "z1_protocol_reading",
        "z1_triangle_anchor_1", "z1_artifact_calibration", "z1_flow_distributor",
        "z1_room1_synchronizer", "z1_hidden_energy_echo",
        "z1_hidden_residual_adjustment"
    };

    private static readonly SocketShape[] Face1NodeShapes =
    {
        SocketShape.Octagonal, SocketShape.Octagonal, SocketShape.Horizontal,
        SocketShape.Octagonal, SocketShape.Compact, SocketShape.Octagonal,
        SocketShape.Horizontal, SocketShape.Compact, SocketShape.Compact
    };

    private static readonly Vector2[] Face2NodePositions =
    {
        P(.20f,.76f), P(.49f,.77f), P(.74f,.73f), P(.18f,.54f), P(.49f,.53f),
        P(.75f,.53f), P(.18f,.29f), P(.38f,.28f), P(.72f,.28f),
        P(.61f,.43f), P(.48f,.12f), P(.88f,.48f), P(.88f,.77f)
    };

    private static readonly string[] Face2NodeIds =
    {
        "z2_fusion_table", "z2_fusion_slot_2", "z2_mix_stabilizer_1",
        "z2_composition_reading", "z2_fusion_slot_3", "z2_residual_catalyst_1",
        "z2_fusion_time_control_1", "z2_catalyst_tuning",
        "z2_stable_reaction_chamber", "z2_guided_synthesis",
        "z2_synthesis_core", "z2_hidden_catalyst_filter",
        "z2_hidden_chamber_cooling"
    };

    private static readonly SocketShape[] Face2NodeShapes =
    {
        SocketShape.Octagonal, SocketShape.Horizontal, SocketShape.Diamond,
        SocketShape.Compact, SocketShape.Octagonal, SocketShape.Compact,
        SocketShape.Vertical, SocketShape.Circular, SocketShape.Horizontal,
        SocketShape.Compact, SocketShape.Horizontal, SocketShape.Compact,
        SocketShape.Compact
    };

    private static readonly Vector2[] Face3NodePositions =
    {
        P(.20f,.76f), P(.75f,.77f), P(.20f,.52f), P(.50f,.53f),
        P(.76f,.50f), P(.20f,.27f), P(.76f,.26f), P(.50f,.18f),
        P(.50f,.82f)
    };

    private static readonly string[] Face3NodeIds =
    {
        "z3_internal_diagnostics", "z3_auxiliary_conduits",
        "z3_compensation_circuit", "z3_machine_memory", "z3_sync_core",
        "z3_structural_reinforcement", "z3_convergence_channel",
        "z3_hidden_failure_marker", "z3_hidden_secondary_conduit"
    };

    private static readonly SocketShape[] Face3NodeShapes =
    {
        SocketShape.Octagonal, SocketShape.Horizontal, SocketShape.Compact,
        SocketShape.Octagonal, SocketShape.Circular, SocketShape.Horizontal,
        SocketShape.Diamond, SocketShape.Compact, SocketShape.Compact
    };

    private static readonly Vector2[] Face4NodePositions =
    {
        P(.21f,.76f), P(.75f,.76f), P(.22f,.61f), P(.51f,.67f),
        P(.50f,.50f), P(.76f,.51f), P(.18f,.38f), P(.49f,.32f),
        P(.75f,.31f), P(.21f,.18f), P(.40f,.16f), P(.68f,.18f)
    };

    private static readonly string[] Face4NodeIds =
    {
        "z4_basic_chamber", "z4_seed_reading_1", "z4_archive_expansion_1",
        "z4_initial_stability_1", "z4_controlled_sync_1",
        "z4_tuned_containment_1", "z4_safe_rewind_1", "z4_seed_slot_3",
        "z4_pure_materialization_1", "z4_seed_slot_4",
        "z4_hidden_minor_chronal_pulse", "z4_hidden_resonant_archive"
    };

    private static readonly SocketShape[] Face4NodeShapes =
    {
        SocketShape.Octagonal, SocketShape.Diamond, SocketShape.Compact,
        SocketShape.Horizontal, SocketShape.Circular, SocketShape.Compact,
        SocketShape.Vertical, SocketShape.Octagonal, SocketShape.Horizontal,
        SocketShape.Octagonal, SocketShape.Compact, SocketShape.Compact
    };

    [MenuItem("Tools/Quantum Forge/Machine/Configure True 3D Prototype")]
    public static void ConfigurePrototype()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        EnsureFolder(AssetFolder);
        ConfigureGeneratedTexture(WornMetalAlbedoPath, false);
        ConfigureGeneratedTexture(WornMetalNormalPath, true);

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
        Texture2D face3Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Face3TexturePath);
        Texture2D face4Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Face4TexturePath);
        Texture2D wornMetal = AssetDatabase.LoadAssetAtPath<Texture2D>(WornMetalAlbedoPath);
        Texture2D wornNormal = AssetDatabase.LoadAssetAtPath<Texture2D>(WornMetalNormalPath);
        Texture2D industrialMetal = AssetDatabase.LoadAssetAtPath<Texture2D>(
            IndustrialMetalAlbedoPath);
        Texture2D industrialNormal = AssetDatabase.LoadAssetAtPath<Texture2D>(
            IndustrialMetalNormalPath);
        Require(face1Texture != null && face2Texture != null &&
                face3Texture != null && face4Texture != null,
            "Faltan texturas de referencia para el prototipo 3D.");
        Require(wornMetal != null && wornNormal != null,
            "Faltan las texturas PBR de metal desgastado del cubo.");
        Require(industrialMetal != null && industrialNormal != null,
            "Faltan las texturas PBR usadas por las caras 1 y 2.");

        PrototypeMaterials materials = CreateMaterials(face1Texture, face2Texture,
            face3Texture, face4Texture, wornMetal, wornNormal, industrialMetal,
            industrialNormal);

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

        GameObject cubeCore = CreateBeveledBox("CubeCore", physicalCube, Vector3.zero,
            new Vector3(7.45f, 7.45f, 7.45f), Quaternion.identity, materials.body);
        AddModule(cubeCore, "cube.core", -1, MachineCube3DModuleRole.Core);
        BuildPhysicalFace(physicalCube, 0, new Vector3(0f, 0f, 3.82f),
            Quaternion.identity, materials.face1, materials, Face1NodePositions,
            Face1NodeIds, Face1NodeShapes, Face1PublicSocketCount,
            materials.blueEmission);
        BuildPhysicalFace(physicalCube, 1, new Vector3(3.82f, 0f, 0f),
            Quaternion.Euler(0f, 90f, 0f), materials.face2, materials,
            Face2NodePositions, Face2NodeIds, Face2NodeShapes,
            Face2PublicSocketCount, materials.purpleEmission);
        BuildPhysicalFace(physicalCube, 2, new Vector3(0f, 0f, -3.82f),
            Quaternion.Euler(0f, 180f, 0f), materials.face3, materials,
            Face3NodePositions, Face3NodeIds, Face3NodeShapes,
            Face3PublicSocketCount, materials.orangeEmission);
        BuildPhysicalFace(physicalCube, 3, new Vector3(-3.82f, 0f, 0f),
            Quaternion.Euler(0f, -90f, 0f), materials.face4, materials,
            Face4NodePositions, Face4NodeIds, Face4NodeShapes,
            Face4PublicSocketCount, materials.greenEmission);
        BuildSharedCornerAssembly(physicalCube, materials);
        MachineCube3DPbrPrototypeSetup.ApplyCubeArtDirection(prototypeRoot);
        MachineCubeFace1V3Setup.ApplyToGeneratedPrototype(prototypeRoot);
        MachineCubeFace2V1Setup.ApplyToGeneratedPrototype(prototypeRoot);
        MachineCubeFace1V3Setup.ApplyFace3ToGeneratedPrototype(prototypeRoot);
        MachineCubeFace1V3Setup.ApplyFace4ToGeneratedPrototype(prototypeRoot);
        ApplyFace1MetalFinishToWholeCube(physicalCube);
        NormalizeNodeLightPalettes(physicalCube, materials);
        RemoveNonNodeLights(physicalCube, materials);
        foreach (MachineCube3DFace face in
            physicalCube.GetComponentsInChildren<MachineCube3DFace>(true))
        {
            face.RebuildCache();
        }

        Camera camera = BuildCamera(prototypeRoot.transform, target);
        BuildDamageGlowVolume(prototypeRoot.transform);
        BuildLights(prototypeRoot.transform);

        MachineCube3DPrototypeDisplayUI displayInput =
            displayObject.AddComponent<MachineCube3DPrototypeDisplayUI>();
        MachineCubeVisualUI machineVisual = panel.GetComponent<MachineCubeVisualUI>();
        SerializedObject inputSo = new SerializedObject(displayInput);
        SetObject(inputSo, "controller", controller);
        SetObject(inputSo, "machineVisual", machineVisual);
        SetObject(inputSo, "machinePanel", panel);
        inputSo.FindProperty("nodeLayer").intValue = 1 << PrototypeLayer;
        inputSo.FindProperty("swipeThresholdPixels").floatValue = 64f;
        inputSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject controllerSo = new SerializedObject(controller);
        SetObject(controllerSo, "cubeRoot", physicalCube);
        SetObject(controllerSo, "prototypeCamera", camera);
        SetObject(controllerSo, "targetTexture", target);
        SetObject(controllerSo, "display", display);
        SetObject(controllerSo, "visualStateController", visualState);
        controllerSo.FindProperty("rotationDuration").floatValue = 0.72f;
        controllerSo.FindProperty("restingYawOffsetDegrees").floatValue = 14f;
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
        Debug.Log("[Machine Cube Modular 3D] CONFIGURED | 4 physical faces | " +
            "35 public sockets + 8 secrets | movable modules | perspective camera | URP materials");
    }

    public static void ConfigurePrototypeBatch()
    {
        ConfigurePrototype();
        ValidatePrototype();
    }

    [MenuItem("Tools/Quantum Forge/Machine/Repair True 3D Display Binding")]
    public static void RepairDisplayBinding()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        Require(panel != null, "MachinePanelUI no disponible en Main.unity.");

        Transform visualRoot = panel.transform.Find("MachineCubeVisualRoot");
        Require(visualRoot != null, "Falta MachineCubeVisualRoot.");
        Transform viewport = visualRoot.Find("FaceViewport");
        Require(viewport != null, "Falta FaceViewport.");

        GameObject prototypeRoot = FindSceneObject("MachineCube3DPrototypeRoot");
        Require(prototypeRoot != null, "Falta MachineCube3DPrototypeRoot.");
        MachineCube3DPrototypeController controller =
            prototypeRoot.GetComponent<MachineCube3DPrototypeController>();
        Require(controller != null, "Falta MachineCube3DPrototypeController.");

        RenderTexture target = AssetDatabase.LoadAssetAtPath<RenderTexture>(
            RenderTexturePath);
        Require(target != null, "Falta el RenderTexture del prototipo 3D.");

        Transform displayTransform = viewport.Find("MachineCube3DPrototypeDisplay");
        GameObject displayObject = displayTransform != null
            ? displayTransform.gameObject
            : CreateRect("MachineCube3DPrototypeDisplay", viewport,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        RawImage display = displayObject.GetComponent<RawImage>();
        if (display == null)
            display = displayObject.AddComponent<RawImage>();
        display.texture = target;
        display.color = Color.white;
        display.raycastTarget = true;

        MachineCube3DPrototypeDisplayUI displayInput =
            displayObject.GetComponent<MachineCube3DPrototypeDisplayUI>();
        if (displayInput == null)
            displayInput = displayObject.AddComponent<MachineCube3DPrototypeDisplayUI>();
        MachineCubeVisualUI machineVisual = panel.GetComponent<MachineCubeVisualUI>();
        Require(machineVisual != null, "Falta MachineCubeVisualUI.");

        SerializedObject inputSo = new SerializedObject(displayInput);
        SetObject(inputSo, "controller", controller);
        SetObject(inputSo, "machineVisual", machineVisual);
        SetObject(inputSo, "machinePanel", panel);
        inputSo.FindProperty("nodeLayer").intValue = 1 << PrototypeLayer;
        inputSo.FindProperty("swipeThresholdPixels").floatValue = 64f;
        inputSo.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject controllerSo = new SerializedObject(controller);
        SetObject(controllerSo, "targetTexture", target);
        SetObject(controllerSo, "display", display);
        controllerSo.ApplyModifiedPropertiesWithoutUndo();

        displayObject.transform.SetAsLastSibling();
        displayObject.SetActive(false);
        EditorUtility.SetDirty(displayObject);
        EditorUtility.SetDirty(prototypeRoot);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[Machine Cube Modular 3D] DISPLAY REPAIRED | RawImage + " +
            "RenderTexture + controller binding restored");
    }

    public static void RepairDisplayBindingBatch()
    {
        try
        {
            RepairDisplayBinding();
            ValidatePrototype();
            Debug.Log("[Machine Cube Modular 3D] DISPLAY REPAIR PASS");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
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
        Require(root.transform.Find("PhysicalCubeRoot/PhysicalFace_3") != null,
            "Falta la Cara 3 fÃ­sica.");
        Require(root.transform.Find("PhysicalCubeRoot/PhysicalFace_4") != null,
            "Falta la Cara 4 fÃ­sica.");
        Require(root.GetComponentsInChildren<MachineCube3DFace>(true).Length == 4,
            "El cubo modular debe registrar exactamente cuatro caras.");
        Require(root.GetComponentsInChildren<MachineCube3DModule>(true).Length >= 100,
            "La geometrÃ­a no estÃ¡ separada en mÃ³dulos identificables.");
        Require(root.GetComponentsInChildren<MeshRenderer>(true).Length >= 430,
            "El prototipo no contiene suficiente geometrÃ­a de relieve.");
        Require(root.GetComponentsInChildren<MachineCube3DNode>(true).Length == 43,
            "El prototipo debe contener 35 sockets publicos y 8 secretos.");
        Require(root.GetComponentsInChildren<Collider>(true).Length == 43,
            "Solo los 43 sockets fisicos deben conservar colliders.");
        Require(root.GetComponentsInChildren<MachineCube3DNode>(true)
                .All(node => node.transform.Find("SelectionFeedback") != null),
            "Cada nodo debe incluir su luz fisica de seleccion.");
        Require(Resources.FindObjectsOfTypeAll<GameObject>()
                .Count(item => item.name == "OptionalDetailGeometry" &&
                    item.scene == root.scene) == 4,
            "Cada cara debe incluir su grupo de detalle escalable.");
        foreach (MachineCube3DFace face in
            root.GetComponentsInChildren<MachineCube3DFace>(true))
        {
            Transform skin = face.transform.Find(
                "StructureModules/PhysicalMetalBackplate");
            Mesh damagedMesh = skin != null
                ? skin.GetComponent<MeshFilter>()?.sharedMesh
                : null;
            Transform damageStages = face.transform.Find(
                "StateModules/FaceDamageDetails");
            Transform repairPatches = face.transform.Find(
                "StateModules/FaceRepairPatches");
            Require(damagedMesh != null && damagedMesh.subMeshCount == 2 &&
                    damagedMesh.GetIndexCount(0) < (uint)(DamagedSkinGrid *
                        DamagedSkinGrid * 6),
                $"Cara {face.FaceIndex + 1} no contiene una piel realmente perforada.");
            Require(damageStages != null && repairPatches != null &&
                    damageStages.childCount == 3 && repairPatches.childCount == 3,
                $"Cara {face.FaceIndex + 1} no empareja tres brechas con tres parches.");
            if (face.FaceIndex >= 2)
            {
                Transform authoredFace = face.transform.Find("BlenderVisualRoot");
                Require(authoredFace != null && face.transform.Find(
                        "ContinuousFaceBorder") == null,
                    $"Cara {face.FaceIndex + 1} no conserva la carcasa " +
                    "maestra de Blender.");
            }
        }
        Require(!root.GetComponentsInChildren<Renderer>(true).Any(renderer =>
                renderer.gameObject.activeSelf &&
                renderer.GetComponentInParent<MachineCube3DNode>() == null &&
                renderer.sharedMaterials.Any(IsDedicatedLightMaterial)),
            "Existen luces activas fuera de los nodos fisicos.");
        Require(!root.GetComponentsInChildren<MachineCube3DNode>(true).Any(node =>
                node.GetComponentsInChildren<Renderer>(true).Any(renderer =>
                    renderer.sharedMaterials.Any(material =>
                        IsDedicatedLightMaterial(material) &&
                        !IsCorrectFaceLight(material, node.FaceIndex)))),
            "Un nodo conserva una luz de color ajeno a su cara.");
        Require(Enumerable.Range(0, 4).All(index =>
                Mathf.Abs(Mathf.DeltaAngle(-90f * index,
                    controller.GetFaceRotationDegrees(index)) - 14f) < 0.01f),
            "Todas las caras deben reposar mostrando el lateral derecho.");
        ValidateNodeCoverage(root);
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
        Debug.Log("[Machine Cube Modular 3D] PASS | real modular geometry | " +
            "4 faces | 7 + 11 + 7 + 10 public sockets | 8 prepared secrets | " +
            "progressive damage | 3 quality profiles");
    }

    private static void ValidateNodeCoverage(GameObject root)
    {
        MachineCube3DNode[] face1Nodes = root.transform
            .Find("PhysicalCubeRoot/PhysicalFace_1")
            .GetComponentsInChildren<MachineCube3DNode>(true);
        MachineCube3DNode[] face2Nodes = root.transform
            .Find("PhysicalCubeRoot/PhysicalFace_2")
            .GetComponentsInChildren<MachineCube3DNode>(true);
        MachineCube3DNode[] face3Nodes = root.transform
            .Find("PhysicalCubeRoot/PhysicalFace_3")
            .GetComponentsInChildren<MachineCube3DNode>(true);
        MachineCube3DNode[] face4Nodes = root.transform
            .Find("PhysicalCubeRoot/PhysicalFace_4")
            .GetComponentsInChildren<MachineCube3DNode>(true);
        Require(face1Nodes.Length == 9 && face2Nodes.Length == 13 &&
                face3Nodes.Length == 9 && face4Nodes.Length == 12,
            "La geometria no reserva todos los sockets de las cuatro caras.");
        Require(face1Nodes.Count(node => node.gameObject.activeSelf) == 7,
            "Cuarto 1 debe iniciar con exactamente 7 sockets publicos.");
        Require(face2Nodes.Count(node => node.gameObject.activeSelf) == 11,
            "Fusiones debe iniciar con exactamente 11 sockets publicos.");
        Require(face3Nodes.Count(node => node.gameObject.activeSelf) == 7,
            "Soporte Interno debe iniciar con exactamente 7 sockets publicos.");
        Require(face4Nodes.Count(node => node.gameObject.activeSelf) == 10,
            "Camara de Anclajes debe iniciar con exactamente 10 sockets publicos.");
        Require(face1Nodes.Select(node => node.SlotIndex).OrderBy(index => index)
                .SequenceEqual(Enumerable.Range(0, 9)),
            "Los indices fisicos de Cuarto 1 no estan completos.");
        Require(face2Nodes.Select(node => node.SlotIndex).OrderBy(index => index)
                .SequenceEqual(Enumerable.Range(0, 13)),
            "Los indices fisicos de Fusiones no estan completos.");
        Require(face3Nodes.Select(node => node.SlotIndex).OrderBy(index => index)
                .SequenceEqual(Enumerable.Range(0, 9)),
            "Los indices fisicos de Soporte Interno no estan completos.");
        Require(face4Nodes.Select(node => node.SlotIndex).OrderBy(index => index)
                .SequenceEqual(Enumerable.Range(0, 12)),
            "Los indices fisicos de Camara de Anclajes no estan completos.");

        TextAsset json = Resources.Load<TextAsset>("Data/machine_nodes");
        Require(json != null, "No carga el catalogo de nodos de maquina.");
        MachineNodeDefList catalog = JsonUtility.FromJson<MachineNodeDefList>(json.text);
        Require(catalog?.nodes != null, "El catalogo de nodos es invalido.");
        Require(CountCatalogBranches(catalog.nodes, MachineZoneType.Room1Link,
                false) == 7 &&
            CountCatalogBranches(catalog.nodes, MachineZoneType.Room1Link, true) == 9,
            "El catalogo de Cuarto 1 ya no coincide con 7 publicos + 2 secretos.");
        Require(CountCatalogBranches(catalog.nodes, MachineZoneType.FusionSector,
                false) == 11 &&
            CountCatalogBranches(catalog.nodes, MachineZoneType.FusionSector, true) == 13,
            "El catalogo de Fusiones ya no coincide con 11 publicos + 2 secretos.");
        Require(CountCatalogBranches(catalog.nodes, MachineZoneType.InternalSupport,
                false) == 7 &&
            CountCatalogBranches(catalog.nodes, MachineZoneType.InternalSupport, true) == 9,
            "El catalogo de Soporte Interno ya no coincide con 7 publicos + 2 secretos.");
        Require(CountCatalogBranches(catalog.nodes, MachineZoneType.InstantChamber,
                false) == 10 &&
            CountCatalogBranches(catalog.nodes, MachineZoneType.InstantChamber, true) == 12,
            "El catalogo de Camara ya no coincide con 10 publicos + 2 secretos.");
    }

    private static int CountCatalogBranches(IEnumerable<MachineNodeDef> nodes,
        MachineZoneType zone, bool includeHidden)
    {
        HashSet<string> branches = new();
        foreach (MachineNodeDef node in nodes)
        {
            if (node == null || node.zone != zone || (!includeHidden && node.hidden))
                continue;
            branches.Add(string.IsNullOrWhiteSpace(node.tierGroup)
                ? node.id
                : "tier:" + node.tierGroup);
        }
        return branches.Count;
    }

    private static void BuildPhysicalFace(Transform cubeRoot, int faceIndex,
        Vector3 localPosition, Quaternion localRotation, Material surfaceMaterial,
        PrototypeMaterials materials, IReadOnlyList<Vector2> nodePositions,
        IReadOnlyList<string> nodeIds, IReadOnlyList<SocketShape> nodeShapes,
        int publicSocketCount, Material emissionMaterial)
    {
        GameObject faceObject = new GameObject("PhysicalFace_" + (faceIndex + 1));
        faceObject.layer = PrototypeLayer;
        Transform face = faceObject.transform;
        face.SetParent(cubeRoot, false);
        face.localPosition = localPosition;
        face.localRotation = localRotation;

        Transform structureRoot = CreateModuleRoot("StructureModules", face,
            $"face.{faceIndex + 1}.structure", faceIndex,
            MachineCube3DModuleRole.Frame);
        Transform nodeRoot = CreateModuleRoot("NodeModules", face,
            $"face.{faceIndex + 1}.nodes", faceIndex,
            MachineCube3DModuleRole.NodeSocket);
        Transform stateRoot = new GameObject("StateModules").transform;
        stateRoot.gameObject.layer = PrototypeLayer;
        stateRoot.SetParent(face, false);

        FaceBreachSpec[] breachSpecs = GetFaceBreachSpecs(faceIndex);
        GameObject surface = CreateDamagedFaceSkin("PhysicalMetalBackplate",
            structureRoot, faceIndex, breachSpecs, surfaceMaterial,
            materials.damageMark);
        AddModule(surface, $"face.{faceIndex + 1}.surface", faceIndex,
            MachineCube3DModuleRole.Surface);
        CreateBeveledBox("RearArmor", structureRoot, new Vector3(0f, 0f, -0.34f),
            new Vector3(7.82f, 7.82f, 0.25f), Quaternion.identity,
            materials.damageMark);

        BuildOuterFrame(structureRoot, materials, emissionMaterial);
        BuildModularSurface(structureRoot, materials, emissionMaterial,
            faceIndex, nodePositions);
        BuildPanelRelief(structureRoot, materials);
        BuildLayeredFaceArmor(structureRoot, materials, emissionMaterial, faceIndex);
        BuildFaceServicePanels(structureRoot, materials, emissionMaterial,
            faceIndex, nodePositions);
        BuildFaceStateDetails(stateRoot, materials, emissionMaterial, faceIndex,
            nodePositions, breachSpecs, surfaceMaterial);

        for (int i = 0; i < nodePositions.Count && i < nodeIds.Count &&
            i < nodeShapes.Count; i++)
        {
            Vector3 position = ToFacePosition(nodePositions[i], 0.14f);
            BuildNode(nodeRoot, faceIndex, i, nodeIds[i], position,
                nodeShapes[i], materials, emissionMaterial, i < publicSocketCount);
        }

        MachineCube3DFace modularFace = faceObject.AddComponent<MachineCube3DFace>();
        modularFace.Configure(faceIndex, (MachineZoneType)(faceIndex + 1),
            structureRoot, nodeRoot, stateRoot);
    }

    private static void BuildOuterFrame(Transform face, PrototypeMaterials materials,
        Material emissionMaterial)
    {
        CreateBeveledBox("FrameTopBackbone", face, new Vector3(0f, 3.76f, -0.08f),
            new Vector3(7.46f, 0.32f, 0.58f), Quaternion.identity, materials.body);
        CreateBeveledBox("FrameBottomBackbone", face, new Vector3(0f, -3.76f, -0.08f),
            new Vector3(7.46f, 0.32f, 0.58f), Quaternion.identity, materials.body);
        CreateBeveledBox("FrameLeftBackbone", face, new Vector3(-3.76f, 0f, -0.08f),
            new Vector3(0.32f, 7.46f, 0.58f), Quaternion.identity, materials.body);
        CreateBeveledBox("FrameRightBackbone", face, new Vector3(3.76f, 0f, -0.08f),
            new Vector3(0.32f, 7.46f, 0.58f), Quaternion.identity, materials.body);

        CreateBeveledBox("FrameTopCap", face, new Vector3(0f, 3.69f, 0.17f),
            new Vector3(6.92f, 0.085f, 0.075f), Quaternion.identity, materials.trim);
        CreateBeveledBox("FrameBottomCap", face, new Vector3(0f, -3.69f, 0.17f),
            new Vector3(6.92f, 0.085f, 0.075f), Quaternion.identity, materials.trim);
        CreateBeveledBox("FrameLeftCap", face, new Vector3(-3.69f, 0f, 0.17f),
            new Vector3(0.085f, 6.92f, 0.075f), Quaternion.identity, materials.trim);
        CreateBeveledBox("FrameRightCap", face, new Vector3(3.69f, 0f, 0.17f),
            new Vector3(0.085f, 6.92f, 0.075f), Quaternion.identity, materials.trim);

        Vector2[] corners =
        {
            new Vector2(-3.18f, 3.18f), new Vector2(3.18f, 3.18f),
            new Vector2(-3.18f, -3.18f), new Vector2(3.18f, -3.18f)
        };
        for (int i = 0; i < corners.Length; i++)
        {
            CreateBeveledBox("ChamferCorner_" + (i + 1), face,
                new Vector3(corners[i].x * 1.12f, corners[i].y * 1.12f, -0.04f),
                new Vector3(0.82f, 0.28f, 0.62f),
                Quaternion.Euler(0f, 0f, i is 0 or 3 ? 45f : -45f), materials.frame);
        }
    }

    private static void BuildSharedCornerAssembly(Transform cube,
        PrototypeMaterials materials)
    {
        Vector3[] positions =
        {
            new Vector3(3.70f, 0f, 3.70f), new Vector3(3.70f, 0f, -3.70f),
            new Vector3(-3.70f, 0f, -3.70f), new Vector3(-3.70f, 0f, 3.70f)
        };
        float[] rotations = { 45f, 135f, 225f, 315f };
        for (int corner = 0; corner < positions.Length; corner++)
        {
            Transform hinge = CreateModuleRoot(
                "SharedMechanicalCorner_" + (corner + 1), cube,
                "cube.corner." + (corner + 1), -1,
                MachineCube3DModuleRole.Frame);
            hinge.localPosition = positions[corner];
            hinge.localRotation = Quaternion.Euler(0f, rotations[corner], 0f);

            CreateBeveledBox("CornerSpine", hinge, Vector3.zero,
                new Vector3(.34f, 7.28f, .38f), Quaternion.identity, materials.body);
            CreateBeveledBox("CornerSpineCap", hinge, new Vector3(0f, 0f, .25f),
                new Vector3(.16f, 6.86f, .09f), Quaternion.identity, materials.frame);
            float[] bandHeights = { -2.85f, -1.0f, 1.0f, 2.85f };
            for (int i = 0; i < bandHeights.Length; i++)
            {
                CreateBeveledBox("CornerBand_" + (i + 1), hinge,
                    new Vector3(0f, bandHeights[i], .02f),
                    new Vector3(.44f, .14f, .46f), Quaternion.identity,
                    materials.trim);
            }
            CreateCylinder("CornerConduitLeft", hinge,
                new Vector3(-.13f, 0f, .28f), new Vector3(.035f, 3.35f, .035f),
                Quaternion.identity, materials.channel);
            CreateCylinder("CornerConduitRight", hinge,
                new Vector3(.13f, 0f, .28f), new Vector3(.035f, 3.35f, .035f),
                Quaternion.identity, materials.channel);
        }
    }

    private static void ReinforceProceduralFaceBorders(Transform cube,
        PrototypeMaterials materials)
    {
        Material approvedFrame = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/Project/Art/MachineCubeBlender/MaterialsV3/" +
            "QF_V3_Unity_Frame.mat") ?? materials.frame;
        Material approvedPlate = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/Project/Art/MachineCubeBlender/MaterialsV3/" +
            "QF_V3_Unity_Plate.mat") ?? materials.module;
        Material approvedNode = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/Project/Art/MachineCubeBlender/MaterialsV3/" +
            "QF_V3_Unity_Node.mat") ?? materials.trim;
        // Face 3 now uses the same authored Blender shell as Faces 1 and 2.
        // Only Face 4 still needs a temporary procedural border.
        for (int faceIndex = 3; faceIndex <= 3; faceIndex++)
        {
            Transform face = cube.Find("PhysicalFace_" + (faceIndex + 1));
            if (face == null)
                throw new InvalidOperationException(
                    $"Falta la Cara {faceIndex + 1} para reforzar su borde.");
            Transform border = CreateModuleRoot("ContinuousFaceBorder", face,
                $"face.{faceIndex + 1}.continuous_border", faceIndex,
                MachineCube3DModuleRole.Frame);

            // Match the approved Blender silhouette used by Faces 1 and 2:
            // three nested structural layers plus segmented perimeter armour.
            CreateFaceBorderLayer("OuterFrame", border, 8.48f, 0.38f,
                0.44f, 0.46f, approvedFrame);
            CreateFaceBorderLayer("MiddleFrame", border, 7.88f, 0.22f,
                0.34f, 0.53f, approvedPlate);
            CreateFaceBorderLayer("InnerFrame", border, 7.52f, 0.12f,
                0.22f, 0.60f, approvedNode);

            float[] topX = { -3.10f, -1.82f, -0.42f, 1.02f, 2.42f, 3.45f };
            float[] bottomX = { -3.05f, -1.73f, -0.38f, 1.02f, 2.38f, 3.42f };
            for (int i = 0; i < topX.Length; i++)
            {
                float width = i == 0 || i == topX.Length - 1 ? 0.92f : 1.12f;
                CreateBeveledBox($"TopArmour_{i + 1:00}", border,
                    new Vector3(topX[i], 3.73f, 0.69f),
                    new Vector3(width, 0.30f, 0.20f), Quaternion.identity,
                    approvedPlate);
            }
            for (int i = 0; i < bottomX.Length; i++)
            {
                float width = i == 0 || i == bottomX.Length - 1 ? 0.98f : 1.14f;
                CreateBeveledBox($"BottomArmour_{i + 1:00}", border,
                    new Vector3(bottomX[i], -3.70f, 0.69f),
                    new Vector3(width, 0.28f, 0.20f), Quaternion.identity,
                    approvedPlate);
            }

            float[] leftY = { 3.05f, 2.08f, -2.18f, -3.10f };
            for (int i = 0; i < leftY.Length; i++)
            {
                float height = i == 0 || i == leftY.Length - 1 ? 0.72f : 0.66f;
                CreateBeveledBox($"LeftArmour_{i + 1:00}", border,
                    new Vector3(-3.74f, leftY[i], 0.69f),
                    new Vector3(0.30f, height, 0.20f), Quaternion.identity,
                    approvedPlate);
            }

            float[] rightY = { 3.12f, 2.08f, 0.96f, -0.18f, -1.34f, -2.55f, -3.30f };
            for (int i = 0; i < rightY.Length; i++)
            {
                float height = i == 2 || i == 4 ? 0.82f : 0.70f;
                CreateBeveledBox($"RightArmour_{i + 1:00}", border,
                    new Vector3(3.73f, rightY[i], 0.69f),
                    new Vector3(0.30f, height, 0.20f), Quaternion.identity,
                    approvedPlate);
            }
        }
    }

    private static void CreateFaceBorderLayer(string name, Transform parent,
        float size, float thickness, float depth, float z, Material material)
    {
        CreateBeveledBox(name + "Top", parent,
            new Vector3(0f, size * 0.5f, z),
            new Vector3(size, thickness, depth), Quaternion.identity, material);
        CreateBeveledBox(name + "Bottom", parent,
            new Vector3(0f, -size * 0.5f, z),
            new Vector3(size, thickness, depth), Quaternion.identity, material);
        CreateBeveledBox(name + "Left", parent,
            new Vector3(-size * 0.5f, 0f, z),
            new Vector3(thickness, size, depth), Quaternion.identity, material);
        CreateBeveledBox(name + "Right", parent,
            new Vector3(size * 0.5f, 0f, z),
            new Vector3(thickness, size, depth), Quaternion.identity, material);
    }

    private static void RemoveNonNodeLights(Transform cube,
        PrototypeMaterials materials)
    {
        Material[] forbiddenOutsideNodes =
        {
            materials.blueEmission, materials.purpleEmission,
            materials.orangeEmission, materials.greenEmission,
            materials.warningEmission, materials.repairEmission
        };
        int removed = 0;
        foreach (Renderer renderer in cube.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer.GetComponentInParent<MachineCube3DNode>() != null)
                continue;
            bool light = renderer.sharedMaterials.Any(material =>
                material != null &&
                (forbiddenOutsideNodes.Contains(material) ||
                 IsDedicatedLightMaterial(material)));
            if (!light)
                continue;
            renderer.gameObject.SetActive(false);
            removed++;
        }
        Debug.Log($"[Machine Cube Lights] node-only cleanup | removed={removed}");
    }

    private static void NormalizeNodeLightPalettes(Transform cube,
        PrototypeMaterials materials)
    {
        Material face1Accent = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/Project/Art/MachineCubeBlender/MaterialsV3/" +
            "QF_V3_Unity_Cyan.mat") ?? materials.blueEmission;
        Material face2Accent = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/Project/Art/MachineCubeBlender/MaterialsFace2V1/" +
            "QF_V3_Unity_Purple.mat") ?? materials.purpleEmission;
        Material[] accents =
        {
            face1Accent, face2Accent, materials.orangeEmission,
            materials.greenEmission
        };
        int remapped = 0;
        foreach (MachineCube3DNode node in
            cube.GetComponentsInChildren<MachineCube3DNode>(true))
        {
            Material accent = accents[Mathf.Clamp(node.FaceIndex, 0, 3)];
            foreach (Renderer renderer in
                node.GetComponentsInChildren<Renderer>(true))
            {
                Material[] slots = renderer.sharedMaterials;
                bool changed = false;
                for (int slot = 0; slot < slots.Length; slot++)
                {
                    if (!IsDedicatedLightMaterial(slots[slot]) ||
                        slots[slot] == accent)
                        continue;
                    slots[slot] = accent;
                    changed = true;
                    remapped++;
                }
                if (changed)
                    renderer.sharedMaterials = slots;
            }
        }
        Debug.Log($"[Machine Cube Lights] face palette normalization | " +
            $"remapped={remapped}");
    }

    private static void ApplyFace1MetalFinishToWholeCube(Transform cube)
    {
        const string authoredFolder =
            "Assets/Project/Art/MachineCubeBlender/MaterialsV3/";
        Material plate = AssetDatabase.LoadAssetAtPath<Material>(authoredFolder +
            "QF_V3_Unity_Plate.mat");
        Material frame = AssetDatabase.LoadAssetAtPath<Material>(authoredFolder +
            "QF_V3_Unity_Frame.mat");
        Material node = AssetDatabase.LoadAssetAtPath<Material>(authoredFolder +
            "QF_V3_Unity_Node.mat");
        Material inner = AssetDatabase.LoadAssetAtPath<Material>(authoredFolder +
            "QF_V3_Unity_Inner.mat");
        Material carbon = AssetDatabase.LoadAssetAtPath<Material>(authoredFolder +
            "QF_V3_Unity_Carbon.mat");
        Material torn = AssetDatabase.LoadAssetAtPath<Material>(authoredFolder +
            "QF_V3_Unity_Torn.mat");
        Material copper = AssetDatabase.LoadAssetAtPath<Material>(authoredFolder +
            "QF_V3_Unity_Copper.mat");
        Material mixedBackplate = AssetDatabase.LoadAssetAtPath<Material>(
            AssetFolder + "/Materials/PBR/M3D_PBR_Cube_CarbonCavity48.mat");
        Require(plate != null && frame != null && node != null && inner != null &&
                carbon != null && torn != null && copper != null &&
                mixedBackplate != null,
            "Faltan materiales para extender el acabado de la Cara 1 al cubo.");

        int remapped = 0;
        int[] perFace = new int[4];
        foreach (Renderer renderer in cube.GetComponentsInChildren<Renderer>(true))
        {
            bool keepMixedBackplate = renderer.name == "PhysicalMetalBackplate" ||
                HasAncestorNamed(renderer.transform, "FaceRepairPatches");
            Material[] slots = renderer.sharedMaterials;
            bool changed = false;
            for (int slot = 0; slot < slots.Length; slot++)
            {
                Material current = slots[slot];
                if (current == null)
                    continue;
                Material replacement = current.name switch
                {
                    "M3D_Face1Surface" => keepMixedBackplate
                        ? mixedBackplate : plate,
                    "M3D_Face2Surface" => keepMixedBackplate
                        ? mixedBackplate : plate,
                    "M3D_Face3Surface" => keepMixedBackplate
                        ? mixedBackplate : plate,
                    "M3D_Face4Surface" => keepMixedBackplate
                        ? mixedBackplate : plate,
                    "M3D_Body" => plate,
                    "M3D_Frame" => frame,
                    "M3D_Trim" => frame,
                    "M3D_Module" => node,
                    "M3D_Channel" => inner,
                    "M3D_DamageMark" => keepMixedBackplate
                        ? mixedBackplate : carbon,
                    "M3D_PBR_Cube_Surface35" => keepMixedBackplate
                        ? mixedBackplate : plate,
                    "M3D_PBR_Cube_Armor32" => plate,
                    "M3D_PBR_Cube_ExposedFrame32" => frame,
                    "M3D_PBR_Cube_Module33" => node,
                    "M3D_PBR_Cube_Conduit33" => inner,
                    "M3D_PBR_Cube_Socket33" => carbon,
                    "M3D_PBR_Cube_CarbonCavity48" => keepMixedBackplate
                        ? mixedBackplate : carbon,
                    "M3D_PBR_Cube_TornArmor48" => torn,
                    "QF_V3_Unity_Skin" => keepMixedBackplate
                        ? mixedBackplate : plate,
                    "QF_V3_Unity_Plate" => plate,
                    "QF_V3_Unity_Frame" => frame,
                    "QF_V3_Unity_Node" => node,
                    "QF_V3_Unity_Inner" => inner,
                    "QF_V3_Unity_Carbon" => carbon,
                    "QF_V3_Unity_Torn" => torn,
                    "QF_V3_Unity_Copper" => copper,
                    _ => null
                };
                if (replacement == null || replacement == current)
                    continue;
                slots[slot] = replacement;
                changed = true;
                remapped++;
                int faceIndex = GetPhysicalFaceIndex(renderer.transform, cube);
                if (faceIndex >= 0)
                    perFace[faceIndex]++;
            }
            if (changed)
                renderer.sharedMaterials = slots;
        }

        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            Transform face = cube.Find("PhysicalFace_" + (faceIndex + 1));
            Require(face != null, "Falta una cara al validar el acabado metalico.");
            Renderer backplate = face.GetComponentsInChildren<Renderer>(true)
                .FirstOrDefault(item => item.name == "PhysicalMetalBackplate");
            Require(backplate != null && backplate.sharedMaterials.Any(item =>
                    item == mixedBackplate),
                $"La Cara {faceIndex + 1} no conserva el fondo metalico mixto.");
            Transform patches = face.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(item => item.name == "FaceRepairPatches");
            if (patches != null)
            {
                Require(patches.GetComponentsInChildren<Renderer>(true).All(item =>
                        item.sharedMaterials.Any(material =>
                            material == mixedBackplate)),
                    $"Los parches de la Cara {faceIndex + 1} no conservan " +
                    "el segundo acabado metalico.");
            }
        }
        Debug.Log($"[Machine Cube Materials] FACE 1 FINISH ON FULL CUBE | " +
            $"remapped={remapped} | faces={string.Join(",", perFace)} | " +
            "industrial metal + carbon backplate mix");
    }

    private static bool HasAncestorNamed(Transform current, string name)
    {
        for (Transform item = current; item != null; item = item.parent)
            if (item.name == name)
                return true;
        return false;
    }

    private static int GetPhysicalFaceIndex(Transform current, Transform cube)
    {
        for (Transform item = current; item != null && item != cube;
            item = item.parent)
        {
            if (!item.name.StartsWith("PhysicalFace_", StringComparison.Ordinal))
                continue;
            return int.TryParse(item.name.Substring("PhysicalFace_".Length),
                out int oneBased) ? oneBased - 1 : -1;
        }
        return -1;
    }

    private static bool IsCorrectFaceLight(Material material, int faceIndex)
    {
        if (material == null)
            return false;
        string name = material.name;
        return faceIndex switch
        {
            0 => name.Contains("QF_V3_Unity_Cyan", StringComparison.Ordinal),
            1 => name.Contains("QF_V3_Unity_Purple", StringComparison.Ordinal),
            2 => name.Equals("M3D_OrangeEmission", StringComparison.Ordinal),
            3 => name.Equals("M3D_GreenEmission", StringComparison.Ordinal),
            _ => false
        };
    }

    private static bool IsDedicatedLightMaterial(Material material)
    {
        if (material == null)
            return false;
        string name = material.name;
        return name.Equals("M3D_BlueEmission", StringComparison.Ordinal) ||
            name.Equals("M3D_PurpleEmission", StringComparison.Ordinal) ||
            name.Equals("M3D_OrangeEmission", StringComparison.Ordinal) ||
            name.Equals("M3D_GreenEmission", StringComparison.Ordinal) ||
            name.Equals("M3D_WarningEmission", StringComparison.Ordinal) ||
            name.Equals("M3D_RepairEmission", StringComparison.Ordinal) ||
            name.Contains("QF_V3_Unity_Cyan", StringComparison.Ordinal) ||
            name.Contains("QF_V3_Unity_Purple", StringComparison.Ordinal) ||
            name.Contains("QF_V3_Unity_Amber", StringComparison.Ordinal);
    }

    private static void BuildPanelRelief(Transform face, PrototypeMaterials materials)
    {
        float[] xPositions = { -1.55f, -1.41f, 1.32f, 1.46f };
        for (int i = 0; i < xPositions.Length; i++)
        {
            CreateCylinder("StructuralConduit_" + (i + 1), face,
                new Vector3(xPositions[i], 0f, 0.155f),
                new Vector3(0.027f, 2.65f, 0.027f), Quaternion.identity,
                materials.trim);
        }
        for (int i = -2; i <= 2; i++)
        {
            float y = i * 1.18f;
            CreateBox("ArmorJoinLeft_" + (i + 3), face,
                new Vector3(-2.33f, y, 0.20f), new Vector3(0.42f, 0.045f, 0.09f),
                Quaternion.identity, materials.channel);
            CreateBox("ArmorJoinRight_" + (i + 3), face,
                new Vector3(2.33f, y, 0.20f), new Vector3(0.42f, 0.045f, 0.09f),
                Quaternion.identity, materials.channel);
        }
    }

    private static void BuildModularSurface(Transform face,
        PrototypeMaterials materials, Material accentMaterial, int faceIndex,
        IReadOnlyList<Vector2> nodePositions)
    {
        Transform network = CreateModuleRoot("PhysicalConduitNetwork", face,
            $"face.{faceIndex + 1}.conduit_network", faceIndex,
            MachineCube3DModuleRole.Conduit,
            MachineCube3DQualityLevel.Balanced);
        float busX = faceIndex % 2 == 0 ? -0.78f : 0.78f;
        CreateBeveledBox("PrimaryVerticalBus", network,
            new Vector3(busX, 0f, 0.285f), new Vector3(0.115f, 6.35f, 0.095f),
            Quaternion.identity, materials.channel);
        CreateBeveledBox("SecondaryVerticalBus", network,
            new Vector3(busX + (faceIndex % 2 == 0 ? 0.16f : -0.16f), 0f, 0.278f),
            new Vector3(0.070f, 6.05f, 0.070f), Quaternion.identity,
            materials.trim);

        float tertiaryX = busX + (faceIndex % 2 == 0 ? -0.16f : 0.16f);
        CreateBeveledBox("TertiaryVerticalBus", network,
            new Vector3(tertiaryX, 0f, 0.274f),
            new Vector3(0.052f, 5.86f, 0.060f), Quaternion.identity,
            materials.channel);

        float[] clampY = { -2.48f, -0.84f, 0.84f, 2.48f };
        for (int i = 0; i < clampY.Length; i++)
        {
            CreateBeveledBox($"BusClamp_{i + 1}", network,
                new Vector3(busX, clampY[i], 0.326f),
                new Vector3(0.43f, 0.12f, 0.105f), Quaternion.identity,
                i % 2 == 0 ? materials.module : materials.frame);
            CreateBeveledBox($"BusClampLight_{i + 1}", network,
                new Vector3(busX, clampY[i], 0.386f),
                new Vector3(0.10f, 0.026f, 0.020f), Quaternion.identity,
                i % 2 == faceIndex % 2 ? accentMaterial : materials.trim);
        }

        for (int i = 0; i < nodePositions.Count; i++)
        {
            Vector3 socket = ToFacePosition(nodePositions[i], 0.29f);
            float xStart = Mathf.Lerp(socket.x, busX, 0.5f);
            float horizontalLength = Mathf.Abs(socket.x - busX);
            if (horizontalLength > 0.12f)
            {
                for (int lane = -1; lane <= 1; lane++)
                {
                    CreateBeveledBox($"Branch_{i + 1:00}_H_{lane + 2}", network,
                        new Vector3(xStart, socket.y + lane * 0.072f,
                            0.286f + (lane + 1) * 0.006f),
                        new Vector3(horizontalLength, lane == 0 ? 0.060f : 0.043f,
                            lane == 0 ? 0.078f : 0.060f),
                        Quaternion.identity,
                        lane == 0 ? materials.trim : materials.channel);
                }
            }
            float elbowY = socket.y + (socket.y >= 0f ? -0.13f : 0.13f);
            CreateCylinder($"Branch_{i + 1:00}_Joint", network,
                new Vector3(busX, elbowY, 0.305f),
                new Vector3(0.065f, 0.025f, 0.065f),
                Quaternion.Euler(90f, 0f, 0f),
                i % 3 == 0 ? accentMaterial : materials.trim);

            if (i % 3 == 1)
            {
                CreateBeveledBox($"Branch_{i + 1:00}_Relay", network,
                    new Vector3(xStart, socket.y, 0.344f),
                    new Vector3(0.30f, 0.20f, 0.11f), Quaternion.identity,
                    materials.module);
                CreateBeveledBox($"Branch_{i + 1:00}_RelayLight", network,
                    new Vector3(xStart, socket.y, 0.406f),
                    new Vector3(0.12f, 0.030f, 0.020f), Quaternion.identity,
                    accentMaterial);
            }
        }
    }

    private static void BuildLayeredFaceArmor(Transform face,
        PrototypeMaterials materials, Material accentMaterial, int faceIndex)
    {
        GameObject optional = new GameObject("OptionalDetailGeometry");
        optional.layer = PrototypeLayer;
        optional.transform.SetParent(face, false);
        AddModule(optional, $"face.{faceIndex + 1}.details", faceIndex,
            MachineCube3DModuleRole.Detail, MachineCube3DQualityLevel.Balanced);
        float[] boltSteps = { -2.35f, -0.78f, 0.78f, 2.35f };
        for (int i = 0; i < boltSteps.Length; i++)
        {
            float panelStep = boltSteps[i];
            CreateBeveledBox("TopArmorSegment_" + (i + 1), optional.transform,
                new Vector3(panelStep, 3.43f, 0.29f),
                new Vector3(1.28f, 0.30f, 0.22f), Quaternion.identity,
                i % 2 == 0 ? materials.frame : materials.body);
            CreateBeveledBox("BottomArmorSegment_" + (i + 1), optional.transform,
                new Vector3(panelStep, -3.43f, 0.29f),
                new Vector3(1.28f, 0.30f, 0.22f), Quaternion.identity,
                i % 2 == 0 ? materials.body : materials.frame);
            CreateBeveledBox("LeftArmorSegment_" + (i + 1), optional.transform,
                new Vector3(-3.43f, panelStep, 0.29f),
                new Vector3(0.30f, 1.28f, 0.22f), Quaternion.identity,
                i % 2 == 0 ? materials.frame : materials.body);
            CreateBeveledBox("RightArmorSegment_" + (i + 1), optional.transform,
                new Vector3(3.43f, panelStep, 0.29f),
                new Vector3(0.30f, 1.28f, 0.22f), Quaternion.identity,
                i % 2 == 0 ? materials.body : materials.frame);

            bool verticalAccent = (i + faceIndex) % 2 == 0;
            CreateBeveledBox("TopStatus_" + (i + 1), optional.transform,
                new Vector3(panelStep, 3.24f, 0.42f),
                new Vector3(verticalAccent ? 0.09f : 0.20f, 0.035f, 0.035f),
                Quaternion.identity, accentMaterial);
            CreateBeveledBox("SideStatus_" + (i + 1), optional.transform,
                new Vector3(3.24f, panelStep, 0.42f),
                new Vector3(0.035f, verticalAccent ? 0.20f : 0.09f, 0.035f),
                Quaternion.identity, accentMaterial);

            CreateCylinder("TopFastener_" + (i + 1), optional.transform,
                new Vector3(boltSteps[i], 3.50f, 0.41f),
                new Vector3(0.055f, 0.035f, 0.055f),
                Quaternion.Euler(90f, 0f, 0f), materials.trim);
            CreateCylinder("BottomFastener_" + (i + 1), optional.transform,
                new Vector3(boltSteps[i], -3.50f, 0.41f),
                new Vector3(0.055f, 0.035f, 0.055f),
                Quaternion.Euler(90f, 0f, 0f), materials.trim);
            CreateCylinder("LeftFastener_" + (i + 1), optional.transform,
                new Vector3(-3.50f, boltSteps[i], 0.41f),
                new Vector3(0.055f, 0.035f, 0.055f),
                Quaternion.Euler(90f, 0f, 0f), materials.trim);
            CreateCylinder("RightFastener_" + (i + 1), optional.transform,
                new Vector3(3.50f, boltSteps[i], 0.41f),
                new Vector3(0.055f, 0.035f, 0.055f),
                Quaternion.Euler(90f, 0f, 0f), materials.trim);
        }

        CreateBeveledBox("TopServiceManifold", optional.transform,
            new Vector3(0f, 3.02f, 0.30f), new Vector3(1.05f, 0.26f, 0.20f),
            Quaternion.identity, materials.module);
        CreateBeveledBox("BottomServiceManifold", optional.transform,
            new Vector3(0f, -3.02f, 0.30f), new Vector3(1.05f, 0.26f, 0.20f),
            Quaternion.identity, materials.module);
        for (int lane = -1; lane <= 1; lane++)
        {
            CreateCylinder("TopManifoldPipe_" + (lane + 2), optional.transform,
                new Vector3(lane * 0.15f, 2.72f, 0.30f),
                new Vector3(0.035f, 0.33f, 0.035f),
                Quaternion.identity, materials.channel);
            CreateCylinder("BottomManifoldPipe_" + (lane + 2), optional.transform,
                new Vector3(lane * 0.15f, -2.72f, 0.30f),
                new Vector3(0.035f, 0.33f, 0.035f),
                Quaternion.identity, materials.channel);
        }

        CreateBeveledBox("InnerTopRail", optional.transform,
            new Vector3(0f, 3.16f, 0.265f), new Vector3(5.60f, 0.085f, 0.12f),
            Quaternion.identity, materials.trim);
        CreateBeveledBox("InnerBottomRail", optional.transform,
            new Vector3(0f, -3.16f, 0.265f), new Vector3(5.60f, 0.085f, 0.12f),
            Quaternion.identity, materials.trim);
        CreateBeveledBox("InnerLeftRail", optional.transform,
            new Vector3(-3.16f, 0f, 0.265f), new Vector3(0.085f, 5.60f, 0.12f),
            Quaternion.identity, materials.trim);
        CreateBeveledBox("InnerRightRail", optional.transform,
            new Vector3(3.16f, 0f, 0.265f), new Vector3(0.085f, 5.60f, 0.12f),
            Quaternion.identity, materials.trim);

        Vector2[] innerCorners =
        {
            new Vector2(-3.12f, 3.12f), new Vector2(3.12f, 3.12f),
            new Vector2(-3.12f, -3.12f), new Vector2(3.12f, -3.12f)
        };
        for (int i = 0; i < innerCorners.Length; i++)
        {
            CreateBeveledBox("InnerCornerBrace_" + (i + 1), optional.transform,
                new Vector3(innerCorners[i].x, innerCorners[i].y, 0.33f),
                new Vector3(0.62f, 0.18f, 0.18f),
                Quaternion.Euler(0f, 0f, i is 0 or 3 ? -45f : 45f),
                materials.frame);
        }
    }

    private static void BuildFaceServicePanels(Transform face,
        PrototypeMaterials materials, Material accentMaterial, int faceIndex,
        IReadOnlyList<Vector2> nodePositions)
    {
        Transform panels = CreateModuleRoot("IndustrialServicePanels", face,
            $"face.{faceIndex + 1}.service_panels", faceIndex,
            MachineCube3DModuleRole.Armor, MachineCube3DQualityLevel.Balanced);

        Vector2[] candidates =
        {
            new Vector2(-2.62f, 2.50f), new Vector2(-1.32f, 2.56f),
            new Vector2(0f, 2.52f), new Vector2(1.34f, 2.54f),
            new Vector2(2.60f, 2.48f), new Vector2(-2.68f, 1.22f),
            new Vector2(-1.34f, 1.24f), new Vector2(0f, 1.28f),
            new Vector2(1.38f, 1.20f), new Vector2(2.64f, 1.26f),
            new Vector2(-2.62f, -0.06f), new Vector2(-1.28f, -0.04f),
            new Vector2(0.04f, 0f), new Vector2(1.34f, -0.06f),
            new Vector2(2.62f, 0.02f), new Vector2(-2.66f, -1.34f),
            new Vector2(-1.32f, -1.30f), new Vector2(0f, -1.28f),
            new Vector2(1.36f, -1.34f), new Vector2(2.62f, -1.26f),
            new Vector2(-2.58f, -2.56f), new Vector2(-1.28f, -2.50f),
            new Vector2(0.04f, -2.54f), new Vector2(1.34f, -2.50f),
            new Vector2(2.60f, -2.54f)
        };

        int created = 0;
        for (int i = 0; i < candidates.Length && created < 6; i++)
        {
            Vector2 candidate = candidates[(i + faceIndex * 4) % candidates.Length];
            if (IsNearNode(candidate, nodePositions, 0.86f))
                continue;
            float width = 0.82f + ((i + faceIndex) % 3) * 0.20f;
            float height = 0.54f + ((i + faceIndex * 2) % 2) * 0.20f;
            float angle = ((i + faceIndex) % 5 - 2) * 2.2f;
            CreateBeveledBox($"ServicePlate_{created + 1:00}", panels,
                new Vector3(candidate.x, candidate.y, 0.205f),
                new Vector3(width, height, 0.105f),
                Quaternion.Euler(0f, 0f, angle),
                created % 3 == 0 ? materials.frame : materials.body);
            CreateBeveledBox($"ServicePlate_{created + 1:00}_Seam", panels,
                new Vector3(candidate.x, candidate.y + height * 0.27f, 0.264f),
                new Vector3(width * 0.64f, 0.025f, 0.022f),
                Quaternion.Euler(0f, 0f, angle), materials.trim);
            CreateCylinder($"ServicePlate_{created + 1:00}_BoltA", panels,
                new Vector3(candidate.x - width * 0.31f,
                    candidate.y - height * 0.28f, 0.277f),
                new Vector3(0.030f, 0.014f, 0.030f),
                Quaternion.Euler(90f, 0f, 0f), materials.trim);
            CreateCylinder($"ServicePlate_{created + 1:00}_BoltB", panels,
                new Vector3(candidate.x + width * 0.31f,
                    candidate.y + height * 0.28f, 0.277f),
                new Vector3(0.030f, 0.014f, 0.030f),
                Quaternion.Euler(90f, 0f, 0f), materials.trim);
            if (created % 3 == 1)
                CreateBeveledBox($"ServicePlate_{created + 1:00}_Status", panels,
                    new Vector3(candidate.x, candidate.y - height * 0.31f, 0.276f),
                    new Vector3(width * 0.20f, 0.026f, 0.020f),
                    Quaternion.Euler(0f, 0f, angle), accentMaterial);
            created++;
        }

        Transform high = CreateModuleRoot("HighDensityMechanicalGreebles", face,
            $"face.{faceIndex + 1}.high_greebles", faceIndex,
            MachineCube3DModuleRole.Detail, MachineCube3DQualityLevel.High);
        for (int i = 0; i < 14; i++)
        {
            float x = -2.85f + (i % 5) * 1.42f;
            float y = -2.70f + (i / 5) * 2.58f + ((i + faceIndex) % 2) * 0.22f;
            Vector2 position = new Vector2(x, y);
            if (IsNearNode(position, nodePositions, 0.72f))
                continue;
            bool vertical = (i + faceIndex) % 2 == 0;
            CreateBeveledBox($"MicroAccess_{i + 1:00}", high,
                new Vector3(x, y, 0.285f),
                vertical ? new Vector3(0.15f, 0.38f, 0.10f)
                    : new Vector3(0.38f, 0.15f, 0.10f),
                Quaternion.identity, i % 3 == 0 ? materials.module : materials.channel);
            CreateBeveledBox($"MicroAccess_{i + 1:00}_Rib", high,
                new Vector3(x, y, 0.342f),
                vertical ? new Vector3(0.035f, 0.22f, 0.020f)
                    : new Vector3(0.22f, 0.035f, 0.020f),
                Quaternion.identity, i % 4 == 0 ? accentMaterial : materials.trim);
        }
    }

    private static bool IsNearNode(Vector2 facePosition,
        IReadOnlyList<Vector2> nodePositions, float minimumDistance)
    {
        for (int i = 0; i < nodePositions.Count; i++)
        {
            Vector3 node = ToFacePosition(nodePositions[i], 0f);
            if (Vector2.Distance(facePosition, new Vector2(node.x, node.y)) <
                minimumDistance)
                return true;
        }
        return false;
    }

    private static void CreateShortChannels(Transform nodeRoot, Vector3 facePosition,
        Material channelMaterial, int faceIndex, int index)
    {
        Transform channels = CreateModuleRoot("ConduitModule", nodeRoot,
            $"face.{faceIndex + 1}.node.{index + 1}.conduits", faceIndex,
            MachineCube3DModuleRole.Conduit);
        float direction = facePosition.y >= 0f ? -1f : 1f;
        float length = 0.55f + (index % 3) * 0.12f;
        for (int lane = -1; lane <= 1; lane++)
        {
            CreateBox($"NodeChannel_{index + 1}_{lane + 2}", channels,
                new Vector3(lane * 0.085f,
                    direction * (0.58f + length * 0.5f), 0.015f),
                new Vector3(0.030f, length, 0.045f), Quaternion.identity,
                channelMaterial);
        }
    }

    private static void BuildNode(Transform face, int faceIndex, int nodeIndex,
        string nodeId, Vector3 position,
        SocketShape shape, PrototypeMaterials materials, Material emissionMaterial,
        bool initiallyVisible)
    {
        GameObject root = new GameObject($"Node3D_{nodeIndex + 1:00}_{nodeId}");
        root.layer = PrototypeLayer;
        root.transform.SetParent(face, false);
        root.transform.localPosition = position;
        AddModule(root, $"face.{faceIndex + 1}.node.{nodeIndex + 1}", faceIndex,
            MachineCube3DModuleRole.NodeSocket);
        CreateShortChannels(root.transform, position, materials.channel,
            faceIndex, nodeIndex);
        MachineCube3DNode node = root.AddComponent<MachineCube3DNode>();
        SerializedObject nodeSo = new SerializedObject(node);
        nodeSo.FindProperty("nodeId").stringValue = nodeId;
        nodeSo.FindProperty("faceIndex").intValue = faceIndex;
        nodeSo.FindProperty("slotIndex").intValue = nodeIndex;
        nodeSo.ApplyModifiedPropertiesWithoutUndo();

        Vector2 dimensions = GetNodeDimensions(faceIndex, nodeIndex);
        float width = dimensions.x;
        float height = dimensions.y;
        BuildOpenSocketHousing(root.transform, width, height, shape, materials);

        BuildNodeDepthDetails(root.transform, width, height, materials,
            emissionMaterial, shape, faceIndex, nodeIndex);

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0f, 0.20f);
        collider.size = new Vector3(width * 1.18f, height * 1.18f, 0.62f);
        collider.enabled = initiallyVisible;
        root.SetActive(initiallyVisible);
    }

    private static void BuildOpenSocketHousing(Transform root, float width,
        float height, SocketShape shape, PrototypeMaterials materials)
    {
        Transform frame = new GameObject("MechanicalSocketFrame").transform;
        frame.gameObject.layer = PrototypeLayer;
        frame.SetParent(root, false);

        frame.localRotation = Quaternion.Euler(0f, 0f,
            shape == SocketShape.Diamond ? 45f : 0f);
        if (shape == SocketShape.Diamond)
        {
            width *= 0.78f;
            height *= 0.78f;
        }

        CreateBeveledBox("SocketRecess", frame,
            new Vector3(0f, 0f, 0.025f),
            new Vector3(width * 0.92f, height * 0.90f, 0.20f),
            Quaternion.identity, materials.damageMark);
        CreateBeveledBox("SocketInnerPlate", frame,
            new Vector3(0f, 0f, 0.085f),
            new Vector3(width * 0.60f, height * 0.56f, 0.050f),
            Quaternion.identity, materials.damageMark);

        float innerWidth = width * 0.69f;
        float innerHeight = height * 0.65f;
        CreateBeveledBox("SocketInnerRimTop", frame,
            new Vector3(0f, innerHeight * 0.50f, 0.125f),
            new Vector3(innerWidth, 0.045f, 0.080f), Quaternion.identity,
            materials.channel);
        CreateBeveledBox("SocketInnerRimBottom", frame,
            new Vector3(0f, -innerHeight * 0.50f, 0.125f),
            new Vector3(innerWidth, 0.045f, 0.080f), Quaternion.identity,
            materials.channel);
        CreateBeveledBox("SocketInnerRimLeft", frame,
            new Vector3(-innerWidth * 0.50f, 0f, 0.125f),
            new Vector3(0.045f, innerHeight, 0.080f), Quaternion.identity,
            materials.channel);
        CreateBeveledBox("SocketInnerRimRight", frame,
            new Vector3(innerWidth * 0.50f, 0f, 0.125f),
            new Vector3(0.045f, innerHeight, 0.080f), Quaternion.identity,
            materials.channel);

        if (shape == SocketShape.Circular)
        {
            BuildCircularSocketHousing(frame, width, height, materials);
            return;
        }

        float rail = shape == SocketShape.Compact ? 0.065f : 0.082f;
        float depth = shape == SocketShape.Compact ? 0.14f : 0.18f;
        float frontZ = 0.145f;
        bool beveled = shape == SocketShape.Octagonal ||
            shape == SocketShape.Diamond;
        float horizontalLength = width * (beveled ? 0.66f : 0.88f);
        float verticalLength = height * (beveled ? 0.66f : 0.88f);

        CreateBeveledBox("HousingTop", frame,
            new Vector3(0f, height * 0.50f, frontZ),
            new Vector3(horizontalLength, rail, depth), Quaternion.identity,
            materials.module);
        CreateBeveledBox("HousingBottom", frame,
            new Vector3(0f, -height * 0.50f, frontZ),
            new Vector3(horizontalLength, rail, depth), Quaternion.identity,
            materials.module);
        CreateBeveledBox("HousingLeft", frame,
            new Vector3(-width * 0.50f, 0f, frontZ),
            new Vector3(rail, verticalLength, depth), Quaternion.identity,
            materials.module);
        CreateBeveledBox("HousingRight", frame,
            new Vector3(width * 0.50f, 0f, frontZ),
            new Vector3(rail, verticalLength, depth), Quaternion.identity,
            materials.module);

        if (beveled)
        {
            float cornerLength = Mathf.Min(width, height) * 0.30f;
            float cornerX = width * 0.39f;
            float cornerY = height * 0.39f;
            CreateBeveledBox("HousingCornerTL", frame,
                new Vector3(-cornerX, cornerY, frontZ),
                new Vector3(cornerLength, rail, depth),
                Quaternion.Euler(0f, 0f, -45f), materials.trim);
            CreateBeveledBox("HousingCornerTR", frame,
                new Vector3(cornerX, cornerY, frontZ),
                new Vector3(cornerLength, rail, depth),
                Quaternion.Euler(0f, 0f, 45f), materials.trim);
            CreateBeveledBox("HousingCornerBL", frame,
                new Vector3(-cornerX, -cornerY, frontZ),
                new Vector3(cornerLength, rail, depth),
                Quaternion.Euler(0f, 0f, 45f), materials.trim);
            CreateBeveledBox("HousingCornerBR", frame,
                new Vector3(cornerX, -cornerY, frontZ),
                new Vector3(cornerLength, rail, depth),
                Quaternion.Euler(0f, 0f, -45f), materials.trim);
        }

        CreateBeveledBox("InnerShadowTop", frame,
            new Vector3(0f, height * 0.43f, 0.055f),
            new Vector3(width * 0.72f, 0.060f, 0.12f), Quaternion.identity,
            materials.channel);
        CreateBeveledBox("InnerShadowBottom", frame,
            new Vector3(0f, -height * 0.43f, 0.055f),
            new Vector3(width * 0.72f, 0.060f, 0.12f), Quaternion.identity,
            materials.channel);

    }

    private static void BuildCircularSocketHousing(Transform frame, float width,
        float height, PrototypeMaterials materials)
    {
        const int segmentCount = 10;
        float radiusX = width * .47f;
        float radiusY = height * .47f;
        float length = Mathf.Min(width, height) * .27f;
        for (int i = 0; i < segmentCount; i++)
        {
            float angle = i * Mathf.PI * 2f / segmentCount;
            Vector3 position = new Vector3(Mathf.Cos(angle) * radiusX,
                Mathf.Sin(angle) * radiusY, .12f);
            Quaternion rotation = Quaternion.Euler(0f, 0f,
                angle * Mathf.Rad2Deg + 90f);
            CreateBeveledBox("CircularHousing_" + (i + 1), frame, position,
                new Vector3(length, .075f, .18f), rotation,
                i % 2 == 0 ? materials.trim : materials.module);
        }
    }

    private static void BuildNodeDepthDetails(Transform root, float width, float height,
        PrototypeMaterials materials, Material operationalMaterial, SocketShape shape,
        int faceIndex, int nodeIndex)
    {
        CreateBox("OperationalEmitter", root,
            new Vector3(0f, -height * 0.49f, 0.245f),
            new Vector3(0.22f, 0.045f, 0.035f),
            Quaternion.identity, operationalMaterial);

        GameObject optional = new GameObject("OptionalNodeDetailGeometry");
        optional.layer = PrototypeLayer;
        optional.transform.SetParent(root, false);
        MachineCube3DModule ownerModule = root.GetComponent<MachineCube3DModule>();
        int ownerFace = ownerModule != null ? ownerModule.FaceIndex : -1;
        AddModule(optional, (ownerModule != null ? ownerModule.ModuleId : "node") +
            ".details", ownerFace, MachineCube3DModuleRole.Detail,
            MachineCube3DQualityLevel.Balanced);
        BuildSocketInterior(optional.transform, width, height, shape,
            materials, operationalMaterial);
        CreateCylinder("FastenerUpperLeft", optional.transform,
            new Vector3(-width * 0.41f, height * 0.41f, 0.25f),
            new Vector3(0.045f, 0.028f, 0.045f),
            Quaternion.Euler(90f, 0f, 0f), materials.trim);
        CreateCylinder("FastenerLowerRight", optional.transform,
            new Vector3(width * 0.41f, -height * 0.41f, 0.25f),
            new Vector3(0.045f, 0.028f, 0.045f),
            Quaternion.Euler(90f, 0f, 0f), materials.trim);
        CreateCylinder("FastenerUpperRight", optional.transform,
            new Vector3(width * 0.41f, height * 0.41f, 0.25f),
            new Vector3(0.040f, 0.025f, 0.040f),
            Quaternion.Euler(90f, 0f, 0f), materials.trim);
        CreateCylinder("FastenerLowerLeft", optional.transform,
            new Vector3(-width * 0.41f, -height * 0.41f, 0.25f),
            new Vector3(0.040f, 0.025f, 0.040f),
            Quaternion.Euler(90f, 0f, 0f), materials.trim);

        GameObject selection = new GameObject("SelectionFeedback");
        selection.layer = PrototypeLayer;
        selection.transform.SetParent(root, false);
        float selectionZ = 0.365f;
        CreateBox("Selection_Underlight", selection.transform,
            new Vector3(0f, -height * 0.43f, selectionZ),
            new Vector3(0.22f, 0.034f, 0.038f),
            Quaternion.identity, operationalMaterial);
        selection.SetActive(false);

        GameObject damageDetails = new GameObject("DamageDetails");
        damageDetails.layer = PrototypeLayer;
        damageDetails.transform.SetParent(root, false);
        AddModule(damageDetails,
            (ownerModule != null ? ownerModule.ModuleId : "node") + ".damage",
            ownerFace, MachineCube3DModuleRole.Damage);
        int damageSeed = 211 + faceIndex * 47 + nodeIndex * 13;
        float direction = (faceIndex + nodeIndex) % 2 == 0 ? 1f : -1f;
        CreateJaggedPrism("DamagedRecess", damageDetails.transform,
            new Vector3(0f, 0f, 0.205f),
            new Vector3(width * 0.72f, height * 0.62f, 0.11f),
            Quaternion.Euler(0f, 0f, direction * 7f), materials.damageMark,
            damageSeed);
        CreateJaggedPrism("CollapsedHousingShard_A", damageDetails.transform,
            new Vector3(-direction * width * 0.28f, height * 0.20f, 0.315f),
            new Vector3(width * 0.46f, height * 0.12f, 0.085f),
            Quaternion.Euler(direction * 11f, -direction * 9f,
                direction * 27f), materials.module, damageSeed + 3);
        CreateJaggedPrism("CollapsedHousingShard_B", damageDetails.transform,
            new Vector3(direction * width * 0.24f, -height * 0.24f, 0.325f),
            new Vector3(width * 0.34f, height * 0.10f, 0.078f),
            Quaternion.Euler(-direction * 8f, direction * 6f,
                -direction * 34f), materials.trim, damageSeed + 7);
        CreateBeveledBox("SeveredNodeCircuit_A", damageDetails.transform,
            new Vector3(-width * 0.10f, -height * 0.02f, 0.352f),
            new Vector3(0.030f, height * 0.34f, 0.026f),
            Quaternion.Euler(0f, 0f, direction * 21f), materials.trim);
        CreateBeveledBox("SeveredNodeCircuit_B", damageDetails.transform,
            new Vector3(width * 0.12f, height * 0.05f, 0.354f),
            new Vector3(0.026f, height * 0.25f, 0.024f),
            Quaternion.Euler(0f, 0f, -direction * 29f), materials.channel);
        GameObject damageEmitter = new GameObject("DamageEmitter");
        damageEmitter.layer = PrototypeLayer;
        damageEmitter.transform.SetParent(damageDetails.transform, false);
        CreateCylinder("DamageLamp", damageEmitter.transform,
            new Vector3(width * 0.37f, height * 0.36f, 0.305f),
            new Vector3(0.038f, 0.016f, 0.038f),
            Quaternion.Euler(90f, 0f, 0f), operationalMaterial);
        CreateBox("DamageCoreGlow", damageEmitter.transform,
            new Vector3(0f, -height * 0.03f, 0.270f),
            new Vector3(width * 0.18f, height * 0.060f, 0.010f),
            Quaternion.Euler(0f, 0f, direction * 4f), operationalMaterial);
        CreateBox("DamageFractureGlow_A", damageEmitter.transform,
            new Vector3(-direction * width * 0.24f, height * 0.14f, 0.365f),
            new Vector3(width * 0.10f, 0.005f, 0.008f),
            Quaternion.Euler(0f, 0f, direction * 34f), operationalMaterial);
        CreateBox("DamageFractureGlow_B", damageEmitter.transform,
            new Vector3(direction * width * 0.21f, -height * 0.18f, 0.365f),
            new Vector3(width * 0.08f, 0.005f, 0.008f),
            Quaternion.Euler(0f, 0f, -direction * 29f), operationalMaterial);
        CreateBox("DamageIdentityUnderlight", damageEmitter.transform,
            new Vector3(0f, -height * 0.43f, 0.335f),
            new Vector3(0.22f, 0.009f, 0.012f),
            Quaternion.identity, operationalMaterial);
        CreateDamagePointLight(damageEmitter.transform,
            new Vector3(0f, -height * 0.02f, 0.245f),
            Mathf.Max(width, height) * 0.56f);

        GameObject repairEmitter = CreateBox("RepairEmitter", root,
            new Vector3(-width * 0.43f, height * 0.43f, 0.275f),
            new Vector3(0.10f, 0.045f, 0.035f), Quaternion.identity,
            operationalMaterial);
        repairEmitter.SetActive(false);
    }

    private static void BuildSocketInterior(Transform parent, float width,
        float height, SocketShape shape, PrototypeMaterials materials,
        Material accentMaterial)
    {
        float z = 0.205f;
        if (shape == SocketShape.Horizontal)
        {
            for (int i = -2; i <= 2; i++)
                CreateBeveledBox("CoilRib_" + (i + 3), parent,
                    new Vector3(i * width * 0.105f, 0f, z),
                    new Vector3(width * 0.055f, height * 0.46f, 0.075f),
                    Quaternion.identity, i == 0 ? materials.trim : materials.module);
            return;
        }
        if (shape == SocketShape.Vertical)
        {
            for (int i = -2; i <= 2; i++)
                CreateBeveledBox("CoolingFin_" + (i + 3), parent,
                    new Vector3(0f, i * height * 0.095f, z),
                    new Vector3(width * 0.48f, height * 0.045f, 0.070f),
                    Quaternion.identity, i == 0 ? materials.trim : materials.module);
            return;
        }
        if (shape == SocketShape.Compact)
        {
            for (int y = -1; y <= 1; y += 2)
            for (int x = -1; x <= 1; x += 2)
                CreateCylinder($"Indicator_{x}_{y}", parent,
                    new Vector3(x * width * 0.17f, y * height * 0.16f, z),
                    new Vector3(0.075f, 0.025f, 0.075f),
                    Quaternion.Euler(90f, 0f, 0f), accentMaterial);
            return;
        }
        if (shape == SocketShape.Circular)
        {
            const int segments = 8;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                CreateBeveledBox("InnerRing_" + (i + 1), parent,
                    new Vector3(Mathf.Cos(angle) * width * 0.22f,
                        Mathf.Sin(angle) * height * 0.22f, z),
                    new Vector3(Mathf.Min(width, height) * 0.15f, 0.045f, 0.07f),
                    Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg + 90f),
                    i % 2 == 0 ? materials.trim : materials.module);
            }
            CreateCylinder("InnerHub", parent, new Vector3(0f, 0f, z + 0.015f),
                new Vector3(width * 0.13f, 0.035f, height * 0.13f),
                Quaternion.Euler(90f, 0f, 0f), materials.channel);
            return;
        }

        CreateBeveledBox("CoreCartridge", parent, new Vector3(0f, 0f, z),
            new Vector3(width * 0.42f, height * 0.38f, 0.085f),
            Quaternion.identity, materials.module);
        CreateBeveledBox("CoreClampTop", parent,
            new Vector3(0f, height * 0.25f, z + 0.015f),
            new Vector3(width * 0.30f, 0.055f, 0.07f),
            Quaternion.identity, materials.trim);
        CreateBeveledBox("CoreClampBottom", parent,
            new Vector3(0f, -height * 0.25f, z + 0.015f),
            new Vector3(width * 0.30f, 0.055f, 0.07f),
            Quaternion.identity, materials.trim);
    }

    private static void CreateSelectionBracket(Transform parent, string suffix,
        float xSign, float ySign, float width, float height, float horizontalLength,
        float verticalLength, float thickness, float z, Material material)
    {
        CreateBox("Selection_" + suffix + "_H", parent,
            new Vector3(xSign * (width * 0.5f - horizontalLength * 0.5f),
                ySign * height * 0.5f, z),
            new Vector3(horizontalLength, thickness, 0.038f),
            Quaternion.identity, material);
        CreateBox("Selection_" + suffix + "_V", parent,
            new Vector3(xSign * width * 0.5f,
                ySign * (height * 0.5f - verticalLength * 0.5f), z),
            new Vector3(thickness, verticalLength, 0.038f),
            Quaternion.identity, material);
    }

    private static FaceBreachSpec[] GetFaceBreachSpecs(int faceIndex)
    {
        return faceIndex switch
        {
            0 => new[]
            {
                new FaceBreachSpec(new Vector2(-2.05f, 0.12f),
                    new Vector2(1.72f, 2.18f), -8f, 11),
                new FaceBreachSpec(new Vector2(1.55f, -1.58f),
                    new Vector2(1.30f, 1.42f), 13f, 23),
                new FaceBreachSpec(new Vector2(-1.48f, -3.22f),
                    new Vector2(1.50f, 1.02f), -4f, 37)
            },
            1 => new[]
            {
                new FaceBreachSpec(new Vector2(1.96f, 0.08f),
                    new Vector2(1.62f, 1.88f), 9f, 43),
                new FaceBreachSpec(new Vector2(-1.74f, -1.46f),
                    new Vector2(1.18f, 1.30f), -14f, 59),
                new FaceBreachSpec(new Vector2(2.18f, 3.20f),
                    new Vector2(1.30f, 0.94f), 5f, 71)
            },
            2 => new[]
            {
                new FaceBreachSpec(new Vector2(-2.02f, 0.10f),
                    new Vector2(1.56f, 1.86f), -11f, 83),
                new FaceBreachSpec(new Vector2(1.74f, -1.72f),
                    new Vector2(1.20f, 1.32f), 16f, 97),
                new FaceBreachSpec(new Vector2(-1.96f, 3.18f),
                    new Vector2(1.32f, 0.92f), -5f, 109)
            },
            _ => new[]
            {
                new FaceBreachSpec(new Vector2(1.94f, -0.10f),
                    new Vector2(1.60f, 1.90f), 10f, 127),
                new FaceBreachSpec(new Vector2(-1.50f, -1.46f),
                    new Vector2(1.22f, 1.34f), -15f, 139),
                new FaceBreachSpec(new Vector2(1.92f, 3.18f),
                    new Vector2(1.34f, 0.94f), 6f, 151)
            }
        };
    }

    private static void BuildFaceStateDetails(Transform face,
        PrototypeMaterials materials, Material accentMaterial, int faceIndex,
        IReadOnlyList<Vector2> nodePositions, FaceBreachSpec[] breachSpecs,
        Material surfaceMaterial)
    {
        GameObject damage = new GameObject("FaceDamageDetails");
        damage.layer = PrototypeLayer;
        damage.transform.SetParent(face, false);
        AddModule(damage, $"face.{faceIndex + 1}.damage", faceIndex,
            MachineCube3DModuleRole.Damage);
        for (int i = 0; i < breachSpecs.Length; i++)
        {
            Transform breach = CreateModuleRoot("PhysicalBreach_" + (i + 1),
                damage.transform, $"face.{faceIndex + 1}.breach.{i + 1}",
                faceIndex, MachineCube3DModuleRole.Damage);
            breach.localPosition = new Vector3(breachSpecs[i].center.x,
                breachSpecs[i].center.y, 0f);
            BuildBlastBreach(breach, breachSpecs[i], materials,
                accentMaterial, i, faceIndex);
        }

        GameObject patches = new GameObject("FaceRepairPatches");
        patches.layer = PrototypeLayer;
        patches.transform.SetParent(face, false);
        AddModule(patches, $"face.{faceIndex + 1}.repair_patches", faceIndex,
            MachineCube3DModuleRole.Armor);
        for (int i = 0; i < breachSpecs.Length; i++)
        {
            GameObject patch = CreateFaceRepairPatch(patches.transform, faceIndex,
                breachSpecs, i, surfaceMaterial);
            patch.SetActive(false);
        }

        GameObject warnings = new GameObject("FaceWarningLights");
        warnings.layer = PrototypeLayer;
        warnings.transform.SetParent(face, false);
        AddModule(warnings, $"face.{faceIndex + 1}.warnings", faceIndex,
            MachineCube3DModuleRole.WarningLight);
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
        AddModule(repairs, $"face.{faceIndex + 1}.repair_lights", faceIndex,
            MachineCube3DModuleRole.RepairLight);
        BuildRepairCircuitRoutes(repairs.transform, materials, accentMaterial,
            faceIndex, nodePositions);
    }

    private static void BuildRepairCircuitRoutes(Transform parent,
        PrototypeMaterials materials, Material accentMaterial, int faceIndex,
        IReadOnlyList<Vector2> nodePositions)
    {
        float busX = faceIndex % 2 == 0 ? -0.78f : 0.78f;
        for (int segment = 0; segment < 4; segment++)
        {
            Transform route = CreateModuleRoot($"RestoredBus_{segment + 1}", parent,
                $"face.{faceIndex + 1}.repair_bus.{segment + 1}", faceIndex,
                MachineCube3DModuleRole.RepairLight);
            route.gameObject.SetActive(false);
            float y = -2.37f + segment * 1.58f;
            CreateBeveledBox("LiveBus", route, new Vector3(busX, y, 0.438f),
                new Vector3(0.020f, 1.18f, 0.020f), Quaternion.identity,
                accentMaterial);
        }

        int routeCount = Mathf.Min(nodePositions.Count, 9);
        for (int i = 0; i < routeCount; i++)
        {
            Transform route = CreateModuleRoot($"RestoredNodeRoute_{i + 1:00}",
                parent, $"face.{faceIndex + 1}.repair_route.{i + 1}", faceIndex,
                MachineCube3DModuleRole.RepairLight);
            route.gameObject.SetActive(false);
            Vector3 socket = ToFacePosition(nodePositions[i], 0.438f);
            float length = Mathf.Abs(socket.x - busX);
            if (length > 0.12f)
            {
                CreateBeveledBox("LiveBranch", route,
                    new Vector3((socket.x + busX) * 0.5f, socket.y, 0.438f),
                    new Vector3(length, 0.018f, 0.018f), Quaternion.identity,
                    accentMaterial);
            }
            CreateCylinder("LiveJoint", route,
                new Vector3(busX, socket.y, 0.445f),
                new Vector3(0.043f, 0.013f, 0.043f),
                Quaternion.Euler(90f, 0f, 0f), accentMaterial);
        }
    }

    private static void BuildBlastBreach(Transform breach, FaceBreachSpec spec,
        PrototypeMaterials materials, Material accentMaterial, int breachIndex,
        int faceIndex)
    {
        Vector2 size = spec.size;
        float direction = (faceIndex + breachIndex) % 2 == 0 ? 1f : -1f;
        float rotation = spec.rotation;

        CreateJaggedPrism("CarbonizedCavity", breach,
            new Vector3(0f, 0f, -0.17f),
            new Vector3(size.x * 0.86f, size.y * 0.82f, 0.055f),
            Quaternion.Euler(0f, 0f, rotation), materials.damageMark,
            spec.seed);
        CreateJaggedPrism("ExposedInnerLayer", breach,
            new Vector3(size.x * 0.06f, -size.y * 0.03f, -0.06f),
            new Vector3(size.x * 0.58f, size.y * 0.50f, 0.070f),
            Quaternion.Euler(direction * 3f, -direction * 4f, -rotation * 0.5f),
            materials.channel, spec.seed + 3);

        // Irregular surface scarring extends beyond the missing metal. The
        // separated strips preserve the open silhouette instead of covering
        // the real perforation with a flat soot decal.
        CreateJaggedPrism("BlastScarring_Down", breach,
            new Vector3(-direction * size.x * 0.30f, -size.y * 0.62f, 0.181f),
            new Vector3(size.x * 0.22f,
                size.y * (breachIndex == 0 ? 0.78f : 0.46f), 0.018f),
            Quaternion.Euler(0f, 0f, rotation + direction * 11f),
            materials.damageMark, spec.seed + 17);
        CreateJaggedPrism("BlastScarring_Upper", breach,
            new Vector3(direction * size.x * 0.36f, size.y * 0.49f, 0.182f),
            new Vector3(size.x * 0.42f, size.y * 0.16f, 0.017f),
            Quaternion.Euler(0f, 0f, rotation - direction * 16f),
            materials.damageMark, spec.seed + 19);
        CreateJaggedPrism("BlastScarring_Side", breach,
            new Vector3(direction * size.x * 0.53f, -size.y * 0.18f, 0.183f),
            new Vector3(size.x * 0.17f, size.y * 0.46f, 0.016f),
            Quaternion.Euler(0f, 0f, rotation + direction * 8f),
            materials.damageMark, spec.seed + 21);

        if (breachIndex == 0)
        {
            CreateJaggedPrism("OxidizedBlastTrail", breach,
                new Vector3(-direction * size.x * 0.12f, -size.y * 0.88f,
                    0.193f),
                new Vector3(size.x * 0.34f, size.y * 0.62f, 0.028f),
                Quaternion.Euler(direction * 3f, -direction * 2f,
                    rotation - direction * 7f), materials.module,
                spec.seed + 29);
        }

        CreateBeveledBox("BentInternalBrace", breach,
            new Vector3(-size.x * 0.05f, 0f, 0.10f),
            new Vector3(size.x * 0.72f, 0.075f, 0.085f),
            Quaternion.Euler(direction * 8f, -direction * 9f,
                direction * 28f), materials.frame);
        CreateJaggedPrism("BuckledArmorPlate", breach,
            new Vector3(direction * size.x * 0.39f, size.y * 0.24f, 0.31f),
            new Vector3(size.x * 0.48f, size.y * 0.22f, 0.10f),
            Quaternion.Euler(direction * 14f, -direction * 11f,
                direction * 23f), materials.module, spec.seed + 5);

        CreateJaggedPrism("TornRimTop", breach,
            new Vector3(-size.x * 0.10f, size.y * 0.43f, 0.25f),
            new Vector3(size.x * 0.58f, 0.085f, 0.09f),
            Quaternion.Euler(direction * 8f, 0f, direction * 13f),
            materials.trim, spec.seed + 7);
        CreateJaggedPrism("TornRimBottom", breach,
            new Vector3(size.x * 0.12f, -size.y * 0.42f, 0.26f),
            new Vector3(size.x * 0.46f, 0.080f, 0.085f),
            Quaternion.Euler(-direction * 6f, direction * 4f,
                -direction * 18f), materials.module, spec.seed + 11);
        CreateJaggedPrism("TornRimSide", breach,
            new Vector3(-direction * size.x * 0.42f, -size.y * 0.05f, 0.255f),
            new Vector3(0.080f, size.y * 0.48f, 0.085f),
            Quaternion.Euler(direction * 5f, -direction * 7f,
                direction * 11f), materials.trim, spec.seed + 13);

        // Broken embedded circuitry: two exposed copper-dark paths and one
        // surviving low-power segment make the damage functional, not cosmetic.
        CreateBeveledBox("SeveredCircuitA", breach,
            new Vector3(-size.x * 0.14f, -size.y * 0.05f, 0.30f),
            new Vector3(0.040f, size.y * 0.44f, 0.034f),
            Quaternion.Euler(0f, 0f, direction * 18f), materials.trim);
        CreateBeveledBox("SeveredCircuitB", breach,
            new Vector3(size.x * 0.15f, size.y * 0.03f, 0.305f),
            new Vector3(0.038f, size.y * 0.32f, 0.032f),
            Quaternion.Euler(0f, 0f, -direction * 27f), materials.channel);
        CreateBeveledBox("LiveCircuitFragment", breach,
            new Vector3(size.x * 0.02f, -size.y * 0.22f, 0.322f),
            new Vector3(size.x * 0.18f, 0.026f, 0.024f),
            Quaternion.identity, accentMaterial);
        CreateCylinder("ElectricalLeak", breach,
            new Vector3(size.x * 0.12f, -size.y * 0.25f, 0.336f),
            new Vector3(0.025f, 0.011f, 0.025f),
            Quaternion.Euler(90f, 0f, 0f), materials.warningEmission);
    }

    private static Vector2 GetNodeDimensions(int faceIndex, int nodeIndex)
    {
        Vector2[] face1 =
        {
            new Vector2(1.30f, 1.26f), new Vector2(1.18f, 1.14f),
            new Vector2(1.38f, 1.12f), new Vector2(1.30f, 1.24f),
            new Vector2(0.96f, 1.02f), new Vector2(1.18f, 1.12f),
            new Vector2(1.38f, 1.02f), new Vector2(0.86f, 0.72f),
            new Vector2(0.86f, 0.72f)
        };
        Vector2[] face2 =
        {
            new Vector2(1.28f, 1.30f), new Vector2(1.48f, 0.94f),
            new Vector2(1.25f, 1.24f), new Vector2(0.96f, 1.05f),
            new Vector2(1.42f, 1.32f), new Vector2(0.94f, 1.02f),
            new Vector2(1.15f, 1.18f), new Vector2(1.20f, 1.16f),
            new Vector2(1.40f, 1.08f), new Vector2(0.76f, 0.92f),
            new Vector2(1.02f, 0.68f), new Vector2(0.72f, 0.82f),
            new Vector2(0.72f, 0.82f)
        };
        Vector2[] face3 =
        {
            new Vector2(1.24f, 1.20f), new Vector2(1.42f, 0.94f),
            new Vector2(1.04f, 1.02f), new Vector2(1.26f, 1.20f),
            new Vector2(1.18f, 1.16f), new Vector2(1.36f, 0.98f),
            new Vector2(1.22f, 1.18f), new Vector2(0.76f, 0.76f),
            new Vector2(0.76f, 0.76f)
        };
        Vector2[] face4 =
        {
            new Vector2(1.28f, 1.24f), new Vector2(1.18f, 1.16f),
            new Vector2(0.94f, 0.98f), new Vector2(1.42f, 0.94f),
            new Vector2(1.22f, 1.18f), new Vector2(0.96f, 1.00f),
            new Vector2(1.05f, 1.18f), new Vector2(1.24f, 1.20f),
            new Vector2(1.40f, 0.98f), new Vector2(1.20f, 1.16f),
            new Vector2(0.72f, 0.78f), new Vector2(0.72f, 0.78f)
        };
        Vector2[] source = faceIndex switch
        {
            0 => face1,
            1 => face2,
            2 => face3,
            _ => face4
        };
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
        // The cube itself supplies the small concept-art yaw on every face. Aim
        // slightly farther to the right than the physical pivot. The visible
        // right-hand depth makes the silhouette heavier on that side; this optical
        // correction centers the complete cube inside the UI chamber.
        cameraObject.transform.localPosition = new Vector3(0f, 0.35f, 20f);
        Vector3 framingTarget = new Vector3(0.72f, 0f, 0f);
        cameraObject.transform.localRotation = Quaternion.LookRotation(
            (framingTarget - cameraObject.transform.localPosition).normalized,
            Vector3.up);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        // Transparent clear lets the approved accident-lab chamber remain visible
        // around the physical geometry in the RawImage.
        camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        camera.orthographic = false;
        camera.fieldOfView = 34.5f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 60f;
        camera.cullingMask = 1 << PrototypeLayer;
        camera.allowHDR = true;
        camera.allowMSAA = true;
        camera.targetTexture = target;
        camera.depth = -20f;
        camera.enabled = false;
        UniversalAdditionalCameraData cameraData =
            cameraObject.AddComponent<UniversalAdditionalCameraData>();
        // Preserve the transparent clear color in the RenderTexture so the shared
        // accident-lab background remains visible around the physical cube.
        cameraData.renderPostProcessing = false;
        cameraData.volumeLayerMask = 1 << PrototypeLayer;
        return camera;
    }

    private static void BuildDamageGlowVolume(Transform parent)
    {
        VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(
            DamageGlowProfilePath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            profile.name = "MachineCubeDamageGlow";
            AssetDatabase.CreateAsset(profile, DamageGlowProfilePath);
        }

        if (!profile.TryGet(out Bloom bloom))
            bloom = profile.Add<Bloom>(true);
        bloom.active = true;
        bloom.threshold.Override(0.72f);
        bloom.intensity.Override(0.30f);
        bloom.scatter.Override(0.44f);
        bloom.highQualityFiltering.Override(true);
        EditorUtility.SetDirty(profile);

        GameObject volumeObject = new GameObject("MachineCubeDamageGlowVolume");
        volumeObject.layer = PrototypeLayer;
        volumeObject.transform.SetParent(parent, false);
        Volume volume = volumeObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 100f;
        volume.sharedProfile = profile;
    }

    private static void CreateDamagePointLight(Transform parent,
        Vector3 localPosition, float range)
    {
        GameObject lightObject = new GameObject("DamageSpillLight");
        lightObject.layer = PrototypeLayer;
        lightObject.transform.SetParent(parent, false);
        lightObject.transform.localPosition = localPosition;
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.035f, 0.012f);
        light.intensity = 1.25f;
        light.range = Mathf.Clamp(range, 0.46f, 0.70f);
        light.shadows = LightShadows.None;
        light.cullingMask = 1 << PrototypeLayer;
        light.renderMode = LightRenderMode.ForcePixel;
    }

    private static void BuildLights(Transform parent)
    {
        GameObject keyObject = new GameObject("IndustrialKeyLight");
        keyObject.layer = PrototypeLayer;
        keyObject.transform.SetParent(parent, false);
        keyObject.transform.localRotation = Quaternion.Euler(38f, 142f, 0f);
        Light key = keyObject.AddComponent<Light>();
        key.type = LightType.Directional;
        key.color = new Color(0.86f, 0.82f, 0.77f);
        key.intensity = 3.10f;
        key.cullingMask = 1 << PrototypeLayer;
        key.shadows = LightShadows.Soft;
        key.shadowResolution = LightShadowResolution.Low;

        GameObject fillObject = new GameObject("IndustrialFillLight");
        fillObject.layer = PrototypeLayer;
        fillObject.transform.SetParent(parent, false);
        fillObject.transform.localRotation = Quaternion.Euler(-15f, 210f, 0f);
        Light fill = fillObject.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.color = new Color(0.30f, 0.40f, 0.50f);
        fill.intensity = 0.82f;
        fill.cullingMask = 1 << PrototypeLayer;
        fill.shadows = LightShadows.None;

        GameObject rimObject = new GameObject("IndustrialRimLight");
        rimObject.layer = PrototypeLayer;
        rimObject.transform.SetParent(parent, false);
        rimObject.transform.localRotation = Quaternion.Euler(18f, 35f, 0f);
        Light rim = rimObject.AddComponent<Light>();
        rim.type = LightType.Directional;
        rim.color = new Color(0.08f, 0.25f, 0.34f);
        rim.intensity = 0.86f;
        rim.cullingMask = 1 << PrototypeLayer;
        rim.shadows = LightShadows.None;
    }

    private static PrototypeMaterials CreateMaterials(Texture2D face1, Texture2D face2,
        Texture2D face3, Texture2D face4, Texture2D wornMetal, Texture2D wornNormal,
        Texture2D industrialMetal, Texture2D industrialNormal)
    {
        return new PrototypeMaterials
        {
            body = CreateTexturedMetalMaterial("M3D_Body",
                new Color(0.25f, 0.25f, 0.24f), 0.56f, 0.16f,
                wornMetal, wornNormal, 2.2f, 0.28f, 0.03f),
            frame = CreateTexturedMetalMaterial("M3D_Frame",
                new Color(0.30f, 0.29f, 0.27f), 0.64f, 0.20f,
                wornMetal, wornNormal, 3.0f, 0.34f, 0.03f),
            trim = CreateTexturedMetalMaterial("M3D_Trim",
                new Color(0.34f, 0.31f, 0.25f), 0.70f, 0.22f,
                wornMetal, wornNormal, 4.0f, 0.38f, 0.02f),
            module = CreateTexturedMetalMaterial("M3D_Module",
                new Color(0.27f, 0.27f, 0.26f), 0.60f, 0.17f,
                wornMetal, wornNormal, 3.4f, 0.36f, 0.03f),
            channel = CreateTexturedMetalMaterial("M3D_Channel",
                new Color(0.12f, 0.16f, 0.18f), 0.45f, 0.10f,
                wornMetal, wornNormal, 4.5f, 0.24f, 0.02f),
            blueEmission = CreateMaterial("M3D_BlueEmission",
                new Color(0.004f, 0.075f, 0.14f), 0.30f, 0.46f,
                new Color(0.00f, 0.42f, 0.76f) * 1.15f),
            purpleEmission = CreateMaterial("M3D_PurpleEmission",
                new Color(0.065f, 0.008f, 0.12f), 0.30f, 0.46f,
                new Color(0.48f, 0.05f, 0.78f) * 1.15f),
            orangeEmission = CreateMaterial("M3D_OrangeEmission",
                new Color(0.14f, 0.030f, 0.002f), 0.30f, 0.46f,
                new Color(0.98f, 0.22f, 0.010f) * 1.15f),
            greenEmission = CreateMaterial("M3D_GreenEmission",
                new Color(0.004f, 0.13f, 0.09f), 0.30f, 0.46f,
                new Color(0.015f, 0.62f, 0.48f) * 1.15f),
            damageMark = CreateMaterial("M3D_DamageMark",
                new Color(0.055f, 0.020f, 0.010f), 0.22f, 0.10f),
            warningEmission = CreateMaterial("M3D_WarningEmission",
                new Color(0.20f, 0.025f, 0.006f), 0.28f, 0.46f,
                new Color(1.00f, 0.12f, 0.012f) * 1.85f),
            repairEmission = CreateMaterial("M3D_RepairEmission",
                new Color(0.008f, 0.18f, 0.105f), 0.30f, 0.54f,
                new Color(0.04f, 1.00f, 0.56f) * 1.65f),
            // The cube no longer uses a complete face illustration as its skin.
            // Faces 3 and 4 use the exact BaseColor and normal textures assigned
            // to the authored skin material on faces 1 and 2.
            face1 = CreateTexturedMetalMaterial("M3D_Face1Surface",
                new Color(0.25f, 0.25f, 0.24f), 0.42f, 0.13f,
                wornMetal, wornNormal, 1.20f, 0.50f, 0.03f),
            face2 = CreateTexturedMetalMaterial("M3D_Face2Surface",
                new Color(0.25f, 0.23f, 0.26f), 0.42f, 0.13f,
                wornMetal, wornNormal, 1.20f, 0.50f, 0.03f),
            face3 = CreateTexturedMetalMaterial("M3D_Face3Surface",
                new Color(0.88f, 0.90f, 0.92f), 0.76f, 0.34f,
                industrialMetal, industrialNormal, 1.0f, 1.0f),
            face4 = CreateTexturedMetalMaterial("M3D_Face4Surface",
                new Color(0.88f, 0.90f, 0.92f), 0.76f, 0.34f,
                industrialMetal, industrialNormal, 1.0f, 1.0f)
        };
    }

    private static Material CreateTexturedMetalMaterial(string name, Color tint,
        float metallic, float smoothness, Texture2D albedo, Texture2D normal,
        float tiling, float normalStrength, float ambientLift = 0f)
    {
        Material material = CreateMaterial(name, tint, metallic, smoothness);
        material.SetTexture("_BaseMap", albedo);
        material.SetTextureScale("_BaseMap", new Vector2(tiling, tiling));
        if (material.HasProperty("_MainTex"))
        {
            material.SetTexture("_MainTex", albedo);
            material.SetTextureScale("_MainTex", new Vector2(tiling, tiling));
        }
        material.EnableKeyword("_NORMALMAP");
        material.SetTexture("_BumpMap", normal);
        material.SetFloat("_BumpScale", normalStrength);
        if (ambientLift > 0f)
        {
            material.EnableKeyword("_EMISSION");
            material.SetTexture("_EmissionMap", albedo);
            material.SetTextureScale("_EmissionMap", new Vector2(tiling, tiling));
            material.SetColor("_EmissionColor", tint * ambientLift);
            material.globalIlluminationFlags =
                MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        EditorUtility.SetDirty(material);
        return material;
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
        material.enableInstancing = true;
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
        string path = AssetFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        Require(shader != null,
            "No se encontro Universal Render Pipeline/Unlit.");
        if (material == null)
        {
            material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }
        material.shader = shader;
        material.enableInstancing = true;
        material.SetColor("_BaseColor", tint);
        material.SetTexture("_BaseMap", texture);
        material.SetTextureOffset("_BaseMap", new Vector2(1f, 0f));
        material.SetTextureScale("_BaseMap", new Vector2(-1f, 1f));
        if (material.HasProperty("_MainTex"))
        {
            material.SetTexture("_MainTex", texture);
            material.SetTextureOffset("_MainTex", new Vector2(1f, 0f));
            material.SetTextureScale("_MainTex", new Vector2(-1f, 1f));
        }
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
        else if (target.width != 1024 || target.height != 1024 ||
                 target.antiAliasing != 2)
        {
            target.Release();
            target.width = 1024;
            target.height = 1024;
            target.antiAliasing = 2;
            EditorUtility.SetDirty(target);
        }
        return target;
    }

    private static GameObject CreateDamagedFaceSkin(string name, Transform parent,
        int faceIndex, FaceBreachSpec[] breachSpecs, Material surfaceMaterial,
        Material cavityMaterial)
    {
        string path = AssetFolder + $"/M3D_Face{faceIndex + 1}_DamagedSkin.asset";
        Mesh mesh = SaveOrUpdateMesh(path,
            BuildFaceSkinMesh(faceIndex, breachSpecs, -1));
        GameObject gameObject = new GameObject(name);
        gameObject.layer = PrototypeLayer;
        gameObject.transform.SetParent(parent, false);
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = new[] { surfaceMaterial, cavityMaterial };
        renderer.shadowCastingMode = ShadowCastingMode.On;
        renderer.receiveShadows = true;
        return gameObject;
    }

    private static GameObject CreateFaceRepairPatch(Transform parent, int faceIndex,
        FaceBreachSpec[] breachSpecs, int breachIndex, Material surfaceMaterial)
    {
        string name = $"RepairPatch_{breachIndex + 1}";
        string path = AssetFolder +
            $"/M3D_Face{faceIndex + 1}_{name}.asset";
        Mesh mesh = SaveOrUpdateMesh(path,
            BuildFaceSkinMesh(faceIndex, breachSpecs, breachIndex));
        GameObject gameObject = new GameObject(name);
        gameObject.layer = PrototypeLayer;
        gameObject.transform.SetParent(parent, false);
        AddModule(gameObject,
            $"face.{faceIndex + 1}.repair_patch.{breachIndex + 1}", faceIndex,
            MachineCube3DModuleRole.Armor);
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = surfaceMaterial;
        renderer.shadowCastingMode = ShadowCastingMode.On;
        renderer.receiveShadows = true;
        return gameObject;
    }

    private static Mesh BuildFaceSkinMesh(int faceIndex,
        FaceBreachSpec[] breachSpecs, int patchIndex)
    {
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var surfaceTriangles = new List<int>();
        var cavityTriangles = new List<int>();
        float cellSize = DamagedSkinSize / DamagedSkinGrid;
        float half = DamagedSkinSize * 0.5f;

        for (int y = 0; y < DamagedSkinGrid; y++)
        for (int x = 0; x < DamagedSkinGrid; x++)
        {
            Vector2 center = new Vector2(-half + (x + 0.5f) * cellSize,
                -half + (y + 0.5f) * cellSize);
            int breach = GetBreachIndex(center, faceIndex, x, y, breachSpecs);
            bool include = patchIndex < 0 ? breach < 0 : breach == patchIndex;
            if (!include)
                continue;

            float x0 = -half + x * cellSize;
            float x1 = x0 + cellSize;
            float y0 = -half + y * cellSize;
            float y1 = y0 + cellSize;
            AddMeshQuad(vertices, uvs, surfaceTriangles,
                new Vector3(x0, y0, DamagedSkinFrontZ),
                new Vector3(x1, y0, DamagedSkinFrontZ),
                new Vector3(x1, y1, DamagedSkinFrontZ),
                new Vector3(x0, y1, DamagedSkinFrontZ), half);

            if (patchIndex >= 0)
                continue;

            AddHoleWallIfNeeded(vertices, uvs, cavityTriangles, faceIndex,
                breachSpecs, x, y, -1, 0,
                new Vector3(x0, y0, DamagedSkinFrontZ),
                new Vector3(x0, y1, DamagedSkinFrontZ), half);
            AddHoleWallIfNeeded(vertices, uvs, cavityTriangles, faceIndex,
                breachSpecs, x, y, 1, 0,
                new Vector3(x1, y1, DamagedSkinFrontZ),
                new Vector3(x1, y0, DamagedSkinFrontZ), half);
            AddHoleWallIfNeeded(vertices, uvs, cavityTriangles, faceIndex,
                breachSpecs, x, y, 0, -1,
                new Vector3(x1, y0, DamagedSkinFrontZ),
                new Vector3(x0, y0, DamagedSkinFrontZ), half);
            AddHoleWallIfNeeded(vertices, uvs, cavityTriangles, faceIndex,
                breachSpecs, x, y, 0, 1,
                new Vector3(x0, y1, DamagedSkinFrontZ),
                new Vector3(x1, y1, DamagedSkinFrontZ), half);
        }

        Mesh mesh = new Mesh
        {
            name = patchIndex < 0
                ? $"M3D_Face{faceIndex + 1}_DamagedSkin"
                : $"M3D_Face{faceIndex + 1}_RepairPatch_{patchIndex + 1}"
        };
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.subMeshCount = patchIndex < 0 ? 2 : 1;
        mesh.SetTriangles(surfaceTriangles, 0);
        if (patchIndex < 0)
            mesh.SetTriangles(cavityTriangles, 1);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AddHoleWallIfNeeded(List<Vector3> vertices,
        List<Vector2> uvs, List<int> triangles, int faceIndex,
        FaceBreachSpec[] breachSpecs, int x, int y, int offsetX, int offsetY,
        Vector3 edgeA, Vector3 edgeB, float uvHalf)
    {
        int neighborX = x + offsetX;
        int neighborY = y + offsetY;
        if (neighborX < 0 || neighborX >= DamagedSkinGrid ||
            neighborY < 0 || neighborY >= DamagedSkinGrid)
        {
            return;
        }
        float cellSize = DamagedSkinSize / DamagedSkinGrid;
        Vector2 neighborCenter = new Vector2(
            -uvHalf + (neighborX + 0.5f) * cellSize,
            -uvHalf + (neighborY + 0.5f) * cellSize);
        if (GetBreachIndex(neighborCenter, faceIndex, neighborX, neighborY,
                breachSpecs) < 0)
        {
            return;
        }
        Vector3 backA = new Vector3(edgeA.x, edgeA.y, DamagedSkinBackZ);
        Vector3 backB = new Vector3(edgeB.x, edgeB.y, DamagedSkinBackZ);
        AddMeshQuad(vertices, uvs, triangles, edgeA, edgeB, backB, backA,
            uvHalf);
    }

    private static int GetBreachIndex(Vector2 point, int faceIndex, int cellX,
        int cellY, FaceBreachSpec[] specs)
    {
        for (int i = 0; i < specs.Length; i++)
        {
            FaceBreachSpec spec = specs[i];
            Vector2 local = point - spec.center;
            float radians = -spec.rotation * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            Vector2 rotated = new Vector2(local.x * cos - local.y * sin,
                local.x * sin + local.y * cos);
            float radiusX = Mathf.Max(0.1f, spec.size.x * 0.5f);
            float radiusY = Mathf.Max(0.1f, spec.size.y * 0.5f);
            float ellipse = rotated.x * rotated.x / (radiusX * radiusX) +
                rotated.y * rotated.y / (radiusY * radiusY);
            int hash = unchecked(spec.seed * 83492791 ^ cellX * 73856093 ^
                cellY * 19349663 ^ faceIndex * 265443576);
            float noise = (hash & 1023) / 1023f;
            float boundary = Mathf.Lerp(0.72f, 1.18f, noise);
            if (ellipse <= boundary)
                return i;
        }
        return -1;
    }

    private static void AddMeshQuad(List<Vector3> vertices, List<Vector2> uvs,
        List<int> triangles, Vector3 a, Vector3 b, Vector3 c, Vector3 d,
        float uvHalf)
    {
        int start = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        float uvScale = Mathf.Max(0.001f, uvHalf * 2f);
        uvs.Add(new Vector2((a.x + uvHalf) / uvScale,
            (a.y + uvHalf) / uvScale));
        uvs.Add(new Vector2((b.x + uvHalf) / uvScale,
            (b.y + uvHalf) / uvScale));
        uvs.Add(new Vector2((c.x + uvHalf) / uvScale,
            (c.y + uvHalf) / uvScale));
        uvs.Add(new Vector2((d.x + uvHalf) / uvScale,
            (d.y + uvHalf) / uvScale));
        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
        triangles.Add(start);
        triangles.Add(start + 2);
        triangles.Add(start + 3);
    }

    private static Mesh SaveOrUpdateMesh(string path, Mesh generated)
    {
        Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (existing == null)
        {
            AssetDatabase.CreateAsset(generated, path);
            return generated;
        }
        EditorUtility.CopySerialized(generated, existing);
        existing.name = generated.name;
        UnityEngine.Object.DestroyImmediate(generated);
        EditorUtility.SetDirty(existing);
        return existing;
    }

    private static GameObject CreateJaggedPrism(string name, Transform parent,
        Vector3 position, Vector3 scale, Quaternion rotation, Material material,
        int seed)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.layer = PrototypeLayer;
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
        gameObject.transform.localScale = scale;
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.sharedMesh = GetOrCreateJaggedPrismMesh(seed);
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.On;
        renderer.receiveShadows = true;
        return gameObject;
    }

    private static Mesh GetOrCreateJaggedPrismMesh(int seed)
    {
        int pattern = Mathf.Abs(seed) % 12;
        string path = AssetFolder + $"/M3D_JaggedPrism_{pattern:00}.asset";
        Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (existing != null)
            return existing;

        int count = 10 + pattern % 3;
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var triangles = new List<int>();
        vertices.Add(new Vector3(0f, 0f, 0.5f));
        uvs.Add(new Vector2(0.5f, 0.5f));
        vertices.Add(new Vector3(0f, 0f, -0.5f));
        uvs.Add(new Vector2(0.5f, 0.5f));
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            int hash = unchecked((pattern + 17) * 83492791 ^ i * 19349663);
            float noise = (hash & 1023) / 1023f;
            float radius = Mathf.Lerp(0.72f, 1.0f, noise);
            if ((i + pattern) % 5 == 0)
                radius *= 0.72f;
            float x = Mathf.Cos(angle) * 0.5f * radius;
            float y = Mathf.Sin(angle) * 0.5f * radius;
            vertices.Add(new Vector3(x, y, 0.5f));
            uvs.Add(new Vector2(x + 0.5f, y + 0.5f));
            vertices.Add(new Vector3(x, y, -0.5f));
            uvs.Add(new Vector2(x + 0.5f, y + 0.5f));
        }
        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;
            int front = 2 + i * 2;
            int back = front + 1;
            int nextFront = 2 + next * 2;
            int nextBack = nextFront + 1;
            triangles.Add(0);
            triangles.Add(front);
            triangles.Add(nextFront);
            triangles.Add(1);
            triangles.Add(nextBack);
            triangles.Add(back);
            triangles.Add(front);
            triangles.Add(back);
            triangles.Add(nextBack);
            triangles.Add(front);
            triangles.Add(nextBack);
            triangles.Add(nextFront);
        }
        Mesh mesh = new Mesh { name = $"M3D_JaggedPrism_{pattern:00}" };
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh, path);
        return mesh;
    }

    private static GameObject CreateBeveledBox(string name, Transform parent,
        Vector3 position, Vector3 scale, Quaternion rotation, Material material)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.layer = PrototypeLayer;
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
        gameObject.transform.localScale = scale;
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.sharedMesh = GetOrCreateBeveledBoxMesh();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.On;
        renderer.receiveShadows = true;
        return gameObject;
    }

    private static Transform CreateModuleRoot(string name, Transform parent,
        string moduleId, int faceIndex, MachineCube3DModuleRole role,
        MachineCube3DQualityLevel minimumQuality = MachineCube3DQualityLevel.Low)
    {
        GameObject root = new GameObject(name);
        root.layer = PrototypeLayer;
        root.transform.SetParent(parent, false);
        AddModule(root, moduleId, faceIndex, role, minimumQuality);
        return root.transform;
    }

    private static MachineCube3DModule AddModule(GameObject target,
        string moduleId, int faceIndex, MachineCube3DModuleRole role,
        MachineCube3DQualityLevel minimumQuality = MachineCube3DQualityLevel.Low)
    {
        MachineCube3DModule module = target.GetComponent<MachineCube3DModule>();
        if (module == null)
            module = target.AddComponent<MachineCube3DModule>();
        module.Configure(moduleId, faceIndex, role, minimumQuality);
        return module;
    }

    private static Mesh GetOrCreateBeveledBoxMesh()
    {
        Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(BeveledBoxMeshPath);
        if (existing != null)
            return existing;

        const float inner = 0.42f;
        List<Vector3> vertices = new();
        List<Vector2> uvs = new();
        List<int> triangles = new();

        AddBevelPolygon(vertices, uvs, triangles,
            new Vector3(-inner, -inner, .5f), new Vector3(inner, -inner, .5f),
            new Vector3(inner, inner, .5f), new Vector3(-inner, inner, .5f));
        AddBevelPolygon(vertices, uvs, triangles,
            new Vector3(-inner, -inner, -.5f), new Vector3(-inner, inner, -.5f),
            new Vector3(inner, inner, -.5f), new Vector3(inner, -inner, -.5f));
        AddBevelPolygon(vertices, uvs, triangles,
            new Vector3(.5f, -inner, -inner), new Vector3(.5f, inner, -inner),
            new Vector3(.5f, inner, inner), new Vector3(.5f, -inner, inner));
        AddBevelPolygon(vertices, uvs, triangles,
            new Vector3(-.5f, -inner, -inner), new Vector3(-.5f, -inner, inner),
            new Vector3(-.5f, inner, inner), new Vector3(-.5f, inner, -inner));
        AddBevelPolygon(vertices, uvs, triangles,
            new Vector3(-inner, .5f, -inner), new Vector3(-inner, .5f, inner),
            new Vector3(inner, .5f, inner), new Vector3(inner, .5f, -inner));
        AddBevelPolygon(vertices, uvs, triangles,
            new Vector3(-inner, -.5f, -inner), new Vector3(inner, -.5f, -inner),
            new Vector3(inner, -.5f, inner), new Vector3(-inner, -.5f, inner));

        foreach (float sx in new[] { -1f, 1f })
        foreach (float sy in new[] { -1f, 1f })
            AddBevelPolygon(vertices, uvs, triangles,
                new Vector3(sx * .5f, sy * inner, -inner),
                new Vector3(sx * inner, sy * .5f, -inner),
                new Vector3(sx * inner, sy * .5f, inner),
                new Vector3(sx * .5f, sy * inner, inner));
        foreach (float sx in new[] { -1f, 1f })
        foreach (float sz in new[] { -1f, 1f })
            AddBevelPolygon(vertices, uvs, triangles,
                new Vector3(sx * .5f, -inner, sz * inner),
                new Vector3(sx * inner, -inner, sz * .5f),
                new Vector3(sx * inner, inner, sz * .5f),
                new Vector3(sx * .5f, inner, sz * inner));
        foreach (float sy in new[] { -1f, 1f })
        foreach (float sz in new[] { -1f, 1f })
            AddBevelPolygon(vertices, uvs, triangles,
                new Vector3(-inner, sy * .5f, sz * inner),
                new Vector3(-inner, sy * inner, sz * .5f),
                new Vector3(inner, sy * inner, sz * .5f),
                new Vector3(inner, sy * .5f, sz * inner));

        foreach (float sx in new[] { -1f, 1f })
        foreach (float sy in new[] { -1f, 1f })
        foreach (float sz in new[] { -1f, 1f })
            AddBevelPolygon(vertices, uvs, triangles,
                new Vector3(sx * .5f, sy * inner, sz * inner),
                new Vector3(sx * inner, sy * .5f, sz * inner),
                new Vector3(sx * inner, sy * inner, sz * .5f));

        Mesh mesh = new Mesh { name = "M3D_BeveledBox" };
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh, BeveledBoxMeshPath);
        return mesh;
    }

    private static void AddBevelPolygon(List<Vector3> vertices, List<Vector2> uvs,
        List<int> triangles, params Vector3[] polygon)
    {
        Vector3 centroid = Vector3.zero;
        for (int i = 0; i < polygon.Length; i++)
            centroid += polygon[i];
        centroid /= polygon.Length;
        Vector3 normal = Vector3.Cross(polygon[1] - polygon[0],
            polygon[2] - polygon[0]);
        if (Vector3.Dot(normal, centroid) < 0f)
            Array.Reverse(polygon);

        int start = vertices.Count;
        normal = Vector3.Cross(polygon[1] - polygon[0], polygon[2] - polygon[0]);
        Vector3 absoluteNormal = new Vector3(Mathf.Abs(normal.x),
            Mathf.Abs(normal.y), Mathf.Abs(normal.z));
        for (int i = 0; i < polygon.Length; i++)
        {
            Vector3 vertex = polygon[i];
            vertices.Add(vertex);
            if (absoluteNormal.z >= absoluteNormal.x &&
                absoluteNormal.z >= absoluteNormal.y)
                uvs.Add(new Vector2(vertex.x + .5f, vertex.y + .5f));
            else if (absoluteNormal.x >= absoluteNormal.y)
                uvs.Add(new Vector2(vertex.z + .5f, vertex.y + .5f));
            else
                uvs.Add(new Vector2(vertex.x + .5f, vertex.z + .5f));
        }
        for (int i = 1; i < polygon.Length - 1; i++)
        {
            triangles.Add(start);
            triangles.Add(start + i);
            triangles.Add(start + i + 1);
        }
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
        return new Vector3((0.5f - normalized.x) * 7.55f,
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

    private static void ConfigureGeneratedTexture(string path, bool normalMap)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        Require(importer != null, "No se pudo importar textura: " + path);
        bool changed = false;
        if (importer.wrapMode != TextureWrapMode.Repeat)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            changed = true;
        }
        if (importer.maxTextureSize != 1024)
        {
            importer.maxTextureSize = 1024;
            changed = true;
        }
        if (importer.textureCompression != TextureImporterCompression.CompressedHQ)
        {
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            changed = true;
        }
        TextureImporterType expectedType = normalMap
            ? TextureImporterType.NormalMap
            : TextureImporterType.Default;
        if (importer.textureType != expectedType)
        {
            importer.textureType = expectedType;
            changed = true;
        }
        if (normalMap && !importer.convertToNormalmap)
        {
            importer.convertToNormalmap = true;
            importer.heightmapScale = 0.12f;
            changed = true;
        }
        if (changed)
            importer.SaveAndReimport();
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

    private enum SocketShape
    {
        Octagonal,
        Horizontal,
        Vertical,
        Compact,
        Circular,
        Diamond
    }

    private readonly struct FaceBreachSpec
    {
        public readonly Vector2 center;
        public readonly Vector2 size;
        public readonly float rotation;
        public readonly int seed;

        public FaceBreachSpec(Vector2 center, Vector2 size, float rotation,
            int seed)
        {
            this.center = center;
            this.size = size;
            this.rotation = rotation;
            this.seed = seed;
        }
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
        public Material orangeEmission;
        public Material greenEmission;
        public Material damageMark;
        public Material warningEmission;
        public Material repairEmission;
        public Material face1;
        public Material face2;
        public Material face3;
        public Material face4;
    }
}
#endif
