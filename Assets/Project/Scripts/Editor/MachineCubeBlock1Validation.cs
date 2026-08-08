#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MachineCubeBlock1Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/Machine/Validate Block 1 Cube")]
    public static void Validate()
    {
        List<string> failures = new();
        ValidateCatalog(failures);
        ValidateProgressContract(failures);
        ValidateRequirementContract(failures);
        ValidateFourFaceNavigationContract(failures);
        ValidateScene(failures);

        if (failures.Count > 0)
            throw new InvalidOperationException("[Machine Cube Block 1] FAIL\n- " +
                string.Join("\n- ", failures));

        Debug.Log("[Machine Cube Block 1] PASS | 55 public | 8 secret | " +
            "44/55 threshold | exact requirements | 4 functional faces");
    }

    public static void ValidateBatch()
    {
        Validate();
    }

    private static void ValidateCatalog(List<string> failures)
    {
        TextAsset json = Resources.Load<TextAsset>("Data/machine_nodes");
        Check(json != null, "No carga Resources/Data/machine_nodes.json.", failures);
        if (json == null)
            return;

        MachineNodeDefList data = JsonUtility.FromJson<MachineNodeDefList>(json.text);
        Check(data?.nodes != null, "Catálogo de nodos inválido.", failures);
        if (data?.nodes == null)
            return;

        int publicCount = data.nodes.Count(node => node != null && !node.hidden);
        int hiddenCount = data.nodes.Count(node => node != null && node.hidden);
        Check(publicCount == 55, "El catálogo ya no contiene 55 nodos públicos.", failures);
        Check(hiddenCount == 8, "El catálogo ya no contiene 8 nodos secretos.", failures);
        Check(Mathf.CeilToInt(publicCount * 0.8f) == 44,
            "El umbral de 80% ya no equivale a 44 nodos públicos.", failures);

        Dictionary<MachineZoneType, int> expectedDisplayCounts = new()
        {
            { MachineZoneType.Room1Link, 7 },
            { MachineZoneType.FusionSector, 11 },
            { MachineZoneType.InternalSupport, 7 },
            { MachineZoneType.InstantChamber, 10 }
        };
        foreach (KeyValuePair<MachineZoneType, int> expected in expectedDisplayCounts)
        {
            int displayCount = CountDisplayBranches(data.nodes, expected.Key, false);
            Check(displayCount == expected.Value,
                expected.Key + " debe mostrar " + expected.Value +
                " sockets públicos y muestra " + displayCount + ".", failures);
            int withSecrets = CountDisplayBranches(data.nodes, expected.Key, true);
            Check(withSecrets <= 13,
                expected.Key + " supera los 13 sockets reutilizables al revelar secretos.",
                failures);
        }

        HashSet<string> ids = data.nodes.Where(node => node != null)
            .Select(node => node.id).ToHashSet();
        foreach (MachineNodeDef node in data.nodes)
        {
            if (node?.requiredNodeIds == null)
                continue;
            foreach (string requirement in node.requiredNodeIds)
                Check(ids.Contains(requirement),
                    node.id + " referencia requisito inexistente " + requirement + ".",
                    failures);
        }
    }

    private static void ValidateProgressContract(List<string> failures)
    {
        GameObject host = new GameObject("Machine Cube Validation Manager");
        MachineManager manager = host.AddComponent<MachineManager>();
        try
        {
            List<string> publicIds = manager.GetAllNodes(true)
                .Where(node => node != null && !node.hidden)
                .Select(node => node.id).ToList();
            List<string> hiddenIds = manager.GetAllNodes(true)
                .Where(node => node != null && node.hidden)
                .Select(node => node.id).ToList();

            SaveData save = new SaveData
            {
                machineUnlocked = true,
                machineAllZonesUnlocked = true,
                machineSelectedFaceIndex = 3,
                machineRepairedNodeIds = publicIds.Take(43).ToList()
            };
            manager.LoadProgressFromSave(save);
            Check(!manager.HasEnoughRepairForPrestige1(),
                "43/55 activa incorrectamente el 80%.", failures);

            save.machineRepairedNodeIds = publicIds.Take(44).ToList();
            manager.LoadProgressFromSave(save);
            Check(manager.HasEnoughRepairForPrestige1(),
                "44/55 no activa el 80%.", failures);
            Check(manager.SelectedMachineFaceIndex == 3,
                "La cara seleccionada no se carga.", failures);

            SaveData roundTrip = new SaveData();
            manager.WriteProgressToSave(roundTrip);
            Check(roundTrip.machineSelectedFaceIndex == 3,
                "La cara seleccionada no se guarda.", failures);

            double beforeSecrets = manager.GetTotalMachineRepairProgress01();
            save.machineRepairedNodeIds.AddRange(hiddenIds);
            manager.LoadProgressFromSave(save);
            Check(Math.Abs(manager.GetTotalMachineRepairProgress01() - beforeSecrets) < 0.000001,
                "Los secretos alteran el progreso global.", failures);

            foreach (MachineZoneType zone in new[] { MachineZoneType.Room1Link,
                MachineZoneType.FusionSector, MachineZoneType.InternalSupport,
                MachineZoneType.InstantChamber })
            {
                List<string> zonePublic = manager.GetAllNodes(true)
                    .Where(node => node != null && node.zone == zone && !node.hidden)
                    .Select(node => node.id).ToList();
                List<string> zoneHidden = manager.GetAllNodes(true)
                    .Where(node => node != null && node.zone == zone && node.hidden)
                    .Select(node => node.id).ToList();
                save.machineRepairedNodeIds = new List<string>(zonePublic);
                manager.LoadProgressFromSave(save);
                double publicOnly = manager.GetZoneRepairProgress01(zone);
                save.machineRepairedNodeIds.AddRange(zoneHidden);
                manager.LoadProgressFromSave(save);
                Check(Math.Abs(manager.GetZoneRepairProgress01(zone) - publicOnly) < 0.000001,
                    "Los secretos alteran el progreso de " + zone + ".", failures);
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            ResetMachineSingleton();
        }
    }

    private static void ValidateRequirementContract(List<string> failures)
    {
        string source = File.ReadAllText(
            "Assets/Project/Scripts/Systems/MachineManager.cs");
        Check(!source.Contains("FreeMachineNodePurchaseMode = true"),
            "FreeMachineNodePurchaseMode sigue omitiendo requisitos.", failures);
        Check(source.Contains("if (GetDef(nodeId) != null)"),
            "La carga no filtra IDs de nodos desconocidos.", failures);

        GameObject host = new GameObject("Machine Requirement Validation Manager");
        MachineManager manager = host.AddComponent<MachineManager>();
        try
        {
            MethodInfo method = typeof(MachineManager).GetMethod(
                "IsRequirementSatisfiedForNode",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Check(method != null, "No existe el validador interno de requisitos.", failures);
            if (method == null)
                return;

            MachineNodeDef target = manager.GetDef("z3_convergence_channel");
            MachineNodeDef exactRequired = manager.GetDef("z3_structural_reinforcement");
            SaveData save = new SaveData
            {
                machineUnlocked = true,
                machineAllZonesUnlocked = true,
                machineRepairedNodeIds = new List<string>()
            };
            manager.LoadProgressFromSave(save);
            bool blocked = (bool)method.Invoke(manager,
                new object[] { target, exactRequired.id });
            Check(!blocked, "Un requisito ausente se considera satisfecho.", failures);

            save.machineRepairedNodeIds.Add(exactRequired.id);
            manager.LoadProgressFromSave(save);
            bool satisfied = (bool)method.Invoke(manager,
                new object[] { target, exactRequired.id });
            Check(satisfied, "El ID requerido exacto no satisface el requisito.", failures);

            MachineNodeDef tierTwo = manager.GetDef("z1_energy_coupling_2");
            MachineNodeDef externalTarget = manager.GetAllNodes(true).FirstOrDefault(node =>
                node?.requiredNodeIds != null &&
                node.requiredNodeIds.Contains(tierTwo.id) &&
                node.tierGroup != tierTwo.tierGroup);
            if (externalTarget != null)
            {
                save.machineRepairedNodeIds = new List<string> { "z1_energy_coupling_1" };
                manager.LoadProgressFromSave(save);
                bool wrongTier = (bool)method.Invoke(manager,
                    new object[] { externalTarget, tierTwo.id });
                Check(!wrongTier,
                    "Un tier inferior sustituye el ID exacto requerido.", failures);
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            ResetMachineSingleton();
        }
    }

    private static void ValidateFourFaceNavigationContract(List<string> failures)
    {
        Check(MachineCubeVisualUI.GetAdjacentFaceIndex(0, 1) == 1,
            "La cara 1 no navega a la cara 2.", failures);
        Check(MachineCubeVisualUI.GetAdjacentFaceIndex(1, 1) == 2,
            "La frontera 3D no permite navegar de la cara 2 a la 3.", failures);
        Check(MachineCubeVisualUI.GetAdjacentFaceIndex(2, 1) == 3,
            "La cara 3 no navega a la cara 4.", failures);
        Check(MachineCubeVisualUI.GetAdjacentFaceIndex(3, 1) == -1 &&
              MachineCubeVisualUI.GetAdjacentFaceIndex(0, -1) == -1,
            "La navegaciÃ³n de caras no respeta los extremos 1 y 4.", failures);

        TextAsset json = Resources.Load<TextAsset>("Data/machine_nodes");
        MachineNodeDefList data = json != null
            ? JsonUtility.FromJson<MachineNodeDefList>(json.text)
            : null;
        MachineNodeDef diagnostics = data?.nodes?.FirstOrDefault(node =>
            node != null && node.id == "z3_internal_diagnostics");
        Check(diagnostics != null && diagnostics.zone == MachineZoneType.InternalSupport,
            "El diagnÃ³stico que desbloquea el anÃ¡lisis no estÃ¡ en la cara 3 accesible.",
            failures);
    }

    private static void ValidateScene(List<string> failures)
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        MachinePanelUI panel = UnityEngine.Object.FindFirstObjectByType<MachinePanelUI>(
            FindObjectsInactive.Include);
        GameObject panelObject = panel != null ? panel.gameObject : null;
        Check(panelObject != null, "Falta MachinePanelRoot.", failures);
        if (panelObject == null)
            return;
        Check(panelObject.name == "MachinePanelRoot",
            "MachinePanelUI no pertenece a MachinePanelRoot.", failures);

        Check(panelObject.GetComponents<MachinePanelUI>().Length == 1,
            "Debe existir un solo MachinePanelUI.", failures);
        Check(panelObject.GetComponents<MachineCubeVisualUI>().Length == 1,
            "Debe existir un solo MachineCubeVisualUI.", failures);
        MachineCubeVisualUI visual = panelObject.GetComponent<MachineCubeVisualUI>();
        Transform root = panelObject.transform.Find("MachineCubeVisualRoot");
        Check(root != null, "Falta MachineCubeVisualRoot.", failures);
        if (root == null)
            return;

        MachineCubeFaceViewUI[] faces = root.GetComponentsInChildren<MachineCubeFaceViewUI>(true);
        Check(faces.Length == 4, "El cubo no contiene exactamente cuatro caras.", failures);
        if (visual != null)
        {
            SerializedObject visualSo = new SerializedObject(visual);
            CheckReference(visualSo, "machinePanel", failures);
            CheckReference(visualSo, "visualContentRoot", failures);
            CheckReference(visualSo, "faceViewport", failures);
            CheckReference(visualSo, "interactionGroup", failures);
            CheckReferenceArray(visualSo, "faces", 4, failures);
            SerializedProperty faceReferences = visualSo.FindProperty("faces");
            HashSet<MachineCubeFaceViewUI> uniqueFaces = new();
            if (faceReferences != null && faceReferences.arraySize == 4)
            {
                for (int i = 0; i < faceReferences.arraySize; i++)
                {
                    MachineCubeFaceViewUI faceReference = faceReferences
                        .GetArrayElementAtIndex(i).objectReferenceValue as MachineCubeFaceViewUI;
                    if (faceReference == null)
                        continue;
                    Check(uniqueFaces.Add(faceReference),
                        "El array de caras contiene referencias duplicadas.", failures);
                    Check(faceReference.Zone == (MachineZoneType)(i + 1),
                        $"La cara {i + 1} no corresponde a su zona serializada.", failures);
                }
            }
            CheckReference(visualSo, "previousFaceButton", failures);
            CheckReference(visualSo, "nextFaceButton", failures);
            CheckReference(visualSo, "rotationRigRoot", failures);
            CheckReference(visualSo, "rotationFromFace", failures);
            CheckReference(visualSo, "rotationToFace", failures);
            CheckReference(visualSo, "rotationEdgeShadow", failures);
            CheckReference(visualSo, "rotationEdgeHighlight", failures);
            CheckReferenceArray(visualSo, "faceDots", 4, failures);
            CheckReference(visualSo, "selectedCardRect", failures);
            CheckReference(visualSo, "selectedFaceProgressText", failures);
        }
        Transform viewport = root.Find("FaceViewport");
        Transform rotationRig = viewport != null
            ? viewport.Find("PhysicalRotationRig")
            : null;
        Check(rotationRig != null, "Falta PhysicalRotationRig dentro de FaceViewport.", failures);
        if (rotationRig != null)
        {
            Check(rotationRig.GetComponentsInChildren<MachineCubePerspectiveFaceGraphic>(true)
                    .Length == 2,
                "PhysicalRotationRig debe contener dos caras de perspectiva.", failures);
            Check(!rotationRig.gameObject.activeSelf,
                "PhysicalRotationRig debe permanecer oculto en reposo.", failures);
            Check(rotationRig.Find("SharedEdgeShadow/SharedEdgeHighlight") != null,
                "PhysicalRotationRig no contiene la arista industrial compartida.", failures);
        }
        foreach (MachineCubeFaceViewUI face in faces)
        {
            int slots = face.GetComponentsInChildren<MachineCubeNodeVisualUI>(true).Length;
            Check(slots == 13, face.name + " no contiene 13 sockets reutilizables.", failures);
            SerializedObject faceSo = new SerializedObject(face);
            CheckReference(faceSo, "faceRect", failures);
            CheckReference(faceSo, "canvasGroup", failures);
            CheckReference(faceSo, "progressText", failures);
            CheckReference(faceSo, "circuitLayer", failures);
            CheckReferenceArray(faceSo, "nodeSlots", 13, failures);
        }
        MachineCubeSwipeSurface[] swipes =
            root.GetComponentsInChildren<MachineCubeSwipeSurface>(true);
        Check(swipes.Length == 1,
            "Falta la superficie de gesto horizontal.", failures);
        if (swipes.Length == 1)
            CheckReference(new SerializedObject(swipes[0]), "controller", failures);
        Check(root.GetComponentsInChildren<Button>(true).Count(button =>
                button.name == "PreviousFaceButton" || button.name == "NextFaceButton") == 2,
            "Faltan flechas izquierda/derecha.", failures);
        Check(root.GetComponentsInChildren<MachineCubeCircuitLineUI>(true).Length == 0,
            "El redise\u00f1o no debe contener conexiones de circuito dibujadas.", failures);

        SerializedObject panelSo = new SerializedObject(panel);
        CheckReference(panelSo, "cubeVisual", failures);
        CheckReferenceMissing(panelSo, "btnPrevNode", failures);
        CheckReferenceMissing(panelSo, "btnNextNode", failures);
        CheckReference(panelSo, "btnRepairNode", failures);
        CheckReference(panelSo, "btnAnalyzeNode", failures);
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }

    private static int CountDisplayBranches(IEnumerable<MachineNodeDef> nodes,
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

    private static void CheckReference(SerializedObject serializedObject,
        string propertyName, List<string> failures)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        Check(property != null && property.objectReferenceValue != null,
            serializedObject.targetObject.name + "." + propertyName +
            " no tiene referencia serializada.", failures);
    }

    private static void CheckReferenceMissing(SerializedObject serializedObject,
        string propertyName, List<string> failures)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        Check(property != null && property.objectReferenceValue == null,
            serializedObject.targetObject.name + "." + propertyName +
            " debe permanecer sin referencia en el redise\u00f1o.", failures);
    }

    private static void CheckReferenceArray(SerializedObject serializedObject,
        string propertyName, int expectedSize, List<string> failures)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        Check(property != null && property.isArray && property.arraySize == expectedSize,
            serializedObject.targetObject.name + "." + propertyName +
            " no tiene tamaño " + expectedSize + ".", failures);
        if (property == null || !property.isArray)
            return;
        for (int i = 0; i < property.arraySize; i++)
            Check(property.GetArrayElementAtIndex(i).objectReferenceValue != null,
                serializedObject.targetObject.name + "." + propertyName + "[" + i +
                "] no tiene referencia serializada.", failures);
    }

    private static void ResetMachineSingleton()
    {
        FieldInfo backingField = typeof(MachineManager).GetField("<I>k__BackingField",
            BindingFlags.Static | BindingFlags.NonPublic);
        backingField?.SetValue(null, null);
    }
}
#endif
