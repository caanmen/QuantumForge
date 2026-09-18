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
        ValidateProgressiveFaceReveal(failures);
        ValidateProgressContract(failures);
        ValidateSeedRetirementMigration(failures);
        ValidateRequirementContract(failures);
        ValidateFourFaceNavigationContract(failures);
        ValidateScene(failures);

        if (failures.Count > 0)
            throw new InvalidOperationException("[Machine Cube Block 1] FAIL\n- " +
                string.Join("\n- ", failures));

        Debug.Log("[Machine Cube Block 1] PASS | 36 public | 6 secret | " +
            "29/36 threshold | 21 retired | retired-node refunds preserved");
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

        List<MachineNodeDef> activeNodes = data.nodes
            .Where(node => node != null && !node.retired).ToList();
        List<MachineNodeDef> retiredNodes = data.nodes
            .Where(node => node != null && node.retired).ToList();
        int publicCount = activeNodes.Count(node => !node.hidden);
        int hiddenCount = activeNodes.Count(node => node.hidden);
        Check(publicCount == 36, "El catálogo activo ya no contiene 36 nodos públicos.", failures);
        Check(hiddenCount == 6, "El catálogo activo ya no contiene 6 nodos secretos.", failures);
        Check(retiredNodes.Count(node => !node.hidden) == 19 &&
              retiredNodes.Count(node => node.hidden) == 2,
            "El catálogo no conserva 19 nodos públicos y 2 secretos retirados.",
            failures);
        Check(retiredNodes.Count(node => node.zone != MachineZoneType.InstantChamber) == 1 &&
              retiredNodes.Any(node => node.id == "z3_convergence_channel"),
            "El Canal de Convergencia no es el único nodo retirado fuera de Semillas.", failures);
        Check(activeNodes.All(node => node.zone != MachineZoneType.InstantChamber),
            "El sector de Semillas todavía contiene nodos activos.", failures);
        Check(Mathf.CeilToInt(publicCount * 0.8f) == 29,
            "El umbral de 80% ya no equivale a 29 nodos públicos.", failures);

        Dictionary<MachineZoneType, int> expectedDisplayCounts = new()
        {
            { MachineZoneType.Room1Link, 7 },
            { MachineZoneType.FusionSector, 11 },
            { MachineZoneType.InternalSupport, 6 }
        };
        foreach (KeyValuePair<MachineZoneType, int> expected in expectedDisplayCounts)
        {
            int displayCount = CountDisplayBranches(activeNodes, expected.Key, false);
            Check(displayCount == expected.Value,
                expected.Key + " debe mostrar " + expected.Value +
                " sockets públicos y muestra " + displayCount + ".", failures);
            int withSecrets = CountDisplayBranches(activeNodes, expected.Key, true);
            Check(withSecrets <= 13,
                expected.Key + " supera los 13 sockets reutilizables al revelar secretos.",
                failures);
        }

        HashSet<string> ids = activeNodes
            .Select(node => node.id).ToHashSet();
        foreach (MachineNodeDef node in activeNodes)
        {
            if (node?.requiredNodeIds == null)
                continue;
            foreach (string requirement in node.requiredNodeIds)
                Check(ids.Contains(requirement),
                    node.id + " referencia requisito inexistente " + requirement + ".",
                    failures);
        }

        MachineNodeDef captureCore = data.nodes.FirstOrDefault(node =>
            node != null && node.id == "z2_fusion_table");
        Check(captureCore != null &&
            captureCore.effectType == MachineNodeEffectType.TriangleEnergyBaseBonus &&
            Math.Abs(captureCore.effectValue - 0.25) < 0.000001,
            "El antiguo nodo de Mezclas no fue reemplazado por Núcleo de Captación.",
            failures);
        MachineNodeDef secondSlot = data.nodes.FirstOrDefault(node =>
            node != null && node.id == "z2_fusion_slot_2");
        Check(secondSlot != null && (secondSlot.requiredNodeIds == null ||
            !secondSlot.requiredNodeIds.Contains("z2_fusion_table")),
            "Ranura de Fusión II todavía depende del antiguo nodo de Mezclas.",
            failures);

        List<MachineNodeDef> publicNodes = activeNodes.Where(node => !node.hidden).ToList();
        Check(Math.Abs(publicNodes.Sum(node => node.cost?.le ?? 0.0) - 32050000.0) < 0.001,
            "El coste público activo ya no suma 32.050.000 LE.", failures);
        Check(Math.Abs(publicNodes.Sum(node => node.cost?.traces ?? 0.0) - 29795.0) < 0.001,
            "El coste público activo ya no suma 29.795 Trazas.", failures);
        Check(publicNodes.Sum(node => node.cost?.hallazgo ?? 0) == 27 &&
              publicNodes.Sum(node => node.cost?.muestra ?? 0) == 25 &&
              publicNodes.Sum(node => node.cost?.lecturaIncompleta ?? 0) == 21 &&
              publicNodes.Sum(node => node.cost?.compuestoUtil ?? 0) == 8,
            "Los materiales públicos ya no conservan los totales 27/25/21/8.", failures);
    }

    private static void ValidateProgressContract(List<string> failures)
    {
        GameObject host = new GameObject("Machine Cube Validation Manager");
        MachineManager manager = host.AddComponent<MachineManager>();
        try
        {
            manager.LoadProgressFromSave(new SaveData
            {
                machineUnlocked = true,
                machineAllZonesUnlocked = true,
                machineRepairedNodeIds = new List<string>()
            });
            Check(manager.GetUnlockedFusionSlotCount() == 1,
                "La Máquina descubierta no entrega la ranura base de Mezclas.",
                failures);

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
                machineRepairedNodeIds = publicIds.Take(28).ToList()
            };
            manager.LoadProgressFromSave(save);
            Check(!manager.HasEnoughRepairForPrestige1(),
                "28/36 activa incorrectamente el 80%.", failures);

            save.machineRepairedNodeIds = publicIds.Take(29).ToList();
            manager.LoadProgressFromSave(save);
            Check(manager.HasEnoughRepairForPrestige1(),
                "29/36 no activa el 80%.", failures);
            Check(manager.SelectedMachineFaceIndex == 3,
                "La cara seleccionada no se carga.", failures);

            SaveData roundTrip = new SaveData();
            manager.WriteProgressToSave(roundTrip);
            Check(roundTrip.machineSelectedFaceIndex == 3,
                "La cara seleccionada no se guarda.", failures);
            Check(roundTrip.machineSeedRetirementMigrationVersion ==
                    MachineManager.SeedRetirementMigrationVersion,
                "La versión de retiro de Semillas no se guarda.", failures);

            double beforeSecrets = manager.GetTotalMachineRepairProgress01();
            save.machineRepairedNodeIds.AddRange(hiddenIds);
            manager.LoadProgressFromSave(save);
            Check(Math.Abs(manager.GetTotalMachineRepairProgress01() - beforeSecrets) < 0.000001,
                "Los secretos alteran el progreso global.", failures);

            foreach (MachineZoneType zone in new[] { MachineZoneType.Room1Link,
                MachineZoneType.FusionSector, MachineZoneType.InternalSupport })
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

    private static void ValidateProgressiveFaceReveal(List<string> failures)
    {
        TextAsset json = Resources.Load<TextAsset>("Data/machine_nodes");
        MachineNodeDefList data = json != null
            ? JsonUtility.FromJson<MachineNodeDefList>(json.text)
            : null;
        if (data?.nodes == null)
        {
            Check(false, "No se pudo validar la revelación progresiva de caras.", failures);
            return;
        }

        List<MachineNodeDef> publicNodes = data.nodes
            .Where(node => node != null && !node.retired && !node.hidden)
            .ToList();
        var repaired = new HashSet<string>();
        int guard = publicNodes.Count * 4;
        bool changed = true;
        while (changed && guard-- > 0)
        {
            changed = false;
            int stage = MachineMonolith2DVisualUI.GetRepairStage(
                publicNodes.Count > 0
                    ? (double)repaired.Count / publicNodes.Count
                    : 0.0);
            foreach (MachineNodeDef node in publicNodes)
            {
                if (repaired.Contains(node.id))
                    continue;
                string branch = string.IsNullOrWhiteSpace(node.tierGroup)
                    ? node.id
                    : node.tierGroup;
                if (!MachineMonolith2DVisualUI.IsBranchVisibleAtStage(branch, stage))
                    continue;
                if (node.requiredNodeIds != null &&
                    node.requiredNodeIds.Any(required => !repaired.Contains(required)))
                {
                    continue;
                }
                repaired.Add(node.id);
                changed = true;
            }
        }

        Check(repaired.Count == publicNodes.Count,
            "La revelación 3/5/7/8 bloquea la reparación en " + repaired.Count +
            "/" + publicNodes.Count + " nodos públicos.", failures);
        Check(repaired.Count >= Mathf.CeilToInt(publicNodes.Count * .8f),
            "Las caras progresivas no permiten alcanzar el 80% de reparación.", failures);
    }

    private static void ValidateSeedRetirementMigration(List<string> failures)
    {
        GameObject host = new GameObject("Machine Seed Retirement Validation Manager");
        MachineManager manager = host.AddComponent<MachineManager>();
        try
        {
            SaveData save = new SaveData
            {
                LE = 10.0,
                Traces = 20.0,
                experimentalHallazgos = 3,
                experimentalMuestras = 4,
                experimentalLecturasIncompletas = 5,
                chronalStableInstants = 2,
                chronalArchivedInstants = 2,
                machineRepairedNodeIds = new List<string>
                {
                    "z1_energy_coupling_1",
                    "z4_basic_chamber",
                    "z4_archive_expansion_2",
                    "z3_convergence_channel"
                },
                machineAnalyzedNodeIds = new List<string>
                {
                    "z4_seed_reading_1"
                },
                machineAnalysisNodeId = "z4_seed_reading_1",
                machineAnalysisRemainingSeconds = 2.5
            };

            bool migrated = manager.ApplySeedRetirementMigration(save);
            Check(migrated, "La migración de Semillas no se ejecutó.", failures);
            Check(save.machineSeedRetirementMigrationVersion ==
                    MachineManager.SeedRetirementMigrationVersion,
                "La migración de Semillas no registra su versión.", failures);
            Check(save.machineRepairedNodeIds.SequenceEqual(
                    new[] { "z1_energy_coupling_1" }),
                "La migración no retiró todos los nodos obsoletos.", failures);
            Check(save.machineAnalyzedNodeIds.Count == 0 &&
                  string.IsNullOrEmpty(save.machineAnalysisNodeId) &&
                  Math.Abs(save.machineAnalysisRemainingSeconds) < 0.000001,
                "La migración conserva análisis activos del sector retirado.", failures);
            Check(Math.Abs(save.LE - 3820010.0) < 0.001 &&
                  Math.Abs(save.Traces - 3720.0) < 0.001,
                "La migración no devuelve el coste nominal de LE y Trazas.", failures);
            Check(save.experimentalHallazgos == 6 &&
                  save.experimentalMuestras == 7 &&
                  save.experimentalLecturasIncompletas == 6 &&
                  save.experimentalCompuestosUtiles == 1,
                "La migración no devuelve los materiales experimentales.", failures);
            Check(save.chronalStableInstants == 3 &&
                  save.chronalArchivedInstants == 3,
                "La migración no devuelve el Anclaje Estable consumido.", failures);

            double leAfterFirstMigration = save.LE;
            Check(!manager.ApplySeedRetirementMigration(save) &&
                  Math.Abs(save.LE - leAfterFirstMigration) < 0.001,
                "La devolución puede aplicarse más de una vez.", failures);
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

            MachineNodeDef target = manager.GetDef("z3_structural_reinforcement");
            MachineNodeDef exactRequired = manager.GetDef("z3_machine_memory");
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

        Transform monolithRoot = panelObject.transform.Find("MachineMonolith2DRoot");
        if (monolithRoot != null)
        {
            Check(panelObject.GetComponents<MachineMonolith2DVisualUI>().Length == 1,
                "Debe existir un solo MachineMonolith2DVisualUI.", failures);
            Check(panelObject.GetComponent<MachineCubeVisualUI>() == null &&
                panelObject.transform.Find("MachineCubeVisualRoot") == null,
                "La composición 3D antigua sigue activa junto al Monolito 2D.",
                failures);
            try
            {
                MachineMonolith2DSetup.Validate();
            }
            catch (Exception exception)
            {
                failures.Add("El Monolito 2D no superó su validación estructural: " +
                    exception.Message);
            }

            VerticalSettingsPanelUI settings =
                UnityEngine.Object.FindFirstObjectByType<VerticalSettingsPanelUI>(
                    FindObjectsInactive.Include);
            Check(settings != null &&
                settings.transform.Find("Machine3DGraphicsCard") == null,
                "La tarjeta obsoleta de gráficos 3D reapareció en Ajustes.", failures);
            return;
        }

        failures.Add("Falta MachineMonolith2DRoot.");
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
