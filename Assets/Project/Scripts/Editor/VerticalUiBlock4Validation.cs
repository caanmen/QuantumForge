#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class VerticalUiBlock4Validation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/Vertical UI/Validate Block 4 Generation Triangle")]
    public static void Validate()
    {
        var failures = new List<string>();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        TabsUI tabs = UnityEngine.Object.FindFirstObjectByType<TabsUI>(
            FindObjectsInactive.Include);
        VerticalGenerationBeforeTriangleUI stateController =
            UnityEngine.Object.FindFirstObjectByType<VerticalGenerationBeforeTriangleUI>(
                FindObjectsInactive.Include);
        GameObject root = FindNamed(scene, "GenerationTriangleRoot");

        Check(root != null, "Falta GenerationTriangleRoot.", failures);
        Check(tabs != null && tabs.generationTriangleLayout == root,
            "TabsUI no referencia GenerationTriangleRoot.", failures);
        Check(stateController != null && stateController.triangleRoot == root,
            "El controlador de estados no referencia el nuevo Triangulo.", failures);
        Check(root != null && !root.activeSelf,
            "La escena base revela el estado avanzado.", failures);

        if (root != null)
        {
            ValidateScrollAndStructure(root, failures);
            ValidateTriangleLogic(root, failures);
            ValidateArtifactCards(root, failures);
            ValidateNoLegacyPresentation(root, failures);
        }
        ValidateSharedEconomy(failures);
        ValidateLegacyBackup(scene, failures);
        ValidateNoDuplicates(scene, failures);
        Finish(failures);
    }

    private static void ValidateScrollAndStructure(
        GameObject root, List<string> failures)
    {
        ScrollRect scroll = root.GetComponentInChildren<ScrollRect>(true);
        Check(scroll != null && scroll.vertical && !scroll.horizontal &&
            scroll.viewport != null && scroll.content != null,
            "El Triangulo avanzado no usa ScrollRect vertical.", failures);
        Check(root.transform.Find("TriangleScroll/Viewport/Content/TriangleGenerationTitle") != null,
            "Falta el titulo de Generacion avanzada.", failures);
        Check(root.transform.Find("TriangleScroll/Viewport/Content/TriangleFocus") != null,
            "Falta el foco compacto del Triangulo.", failures);
        Check(root.transform.Find("TriangleScroll/Viewport/Content/CircuitSelectors") != null,
            "Faltan los selectores de circuito.", failures);
        Check(root.transform.Find("TriangleScroll/Viewport/Content/TriangleArtifactCards") != null,
            "Faltan las tarjetas compactas de artefactos.", failures);
    }

    private static void ValidateTriangleLogic(
        GameObject root, List<string> failures)
    {
        TrianglePanelUI panel = root.GetComponentInChildren<TrianglePanelUI>(true);
        VerticalTrianglePresentationUI presentation =
            root.GetComponentInChildren<VerticalTrianglePresentationUI>(true);
        Check(panel != null, "No se reutiliza TrianglePanelUI.", failures);
        Check(presentation != null && presentation.energyLine != null &&
            presentation.experimentalLine != null && presentation.phaseLine != null &&
            presentation.centerGlow != null,
            "Las tres lineas dinamicas o el nucleo no estan conectados.", failures);

        TriangleSlotUI[] slots = root.GetComponentsInChildren<TriangleSlotUI>(true);
        Check(slots.Length == 3, "Deben existir tres selectores TriangleSlotUI.", failures);
        var roles = new HashSet<int>();
        foreach (TriangleSlotUI slot in slots)
        {
            SerializedObject serialized = new SerializedObject(slot);
            roles.Add(serialized.FindProperty("slotRole").enumValueIndex);
            Button button = slot.GetComponent<Button>();
            RectTransform rect = slot.transform as RectTransform;
            Check(button != null && rect != null && rect.rect.height >= 100f,
                slot.name + " no es un control tactil suficiente.", failures);
        }
        Check(roles.Contains((int)TriangleSlotRole.Primary) &&
            roles.Contains((int)TriangleSlotRole.Reinforcement) &&
            roles.Contains((int)TriangleSlotRole.Alteration),
            "Los selectores no cubren Energia, Experimental y Fase.", failures);

        if (panel != null)
        {
            SerializedObject serialized = new SerializedObject(panel);
            string[] refs =
            {
                "assignedLabelPrimary", "assignedLabelReinforcement",
                "assignedLabelAlteration", "protocolStatusLabel",
                "modulatorModeLabel", "modulatorEffectLabel"
            };
            foreach (string field in refs)
                Check(serialized.FindProperty(field).objectReferenceValue != null,
                    "TrianglePanelUI no conecta " + field + ".", failures);
        }

        string panelSource = File.ReadAllText(
            "Assets/Project/Scripts/UI/TrianglePanelUI.cs");
        string slotSource = File.ReadAllText(
            "Assets/Project/Scripts/UI/TriangleSlotUI.cs");
        string presentationSource = File.ReadAllText(
            "Assets/Project/Scripts/UI/Vertical/VerticalTrianglePresentationUI.cs");
        Check(panelSource.Contains("GetTriangleSynchronizationRemainingSeconds()") &&
            panelSource.Contains("GetTriangleLEMultiplier()") &&
            panelSource.Contains("GetTriangleTracesMultiplier()") &&
            panelSource.Contains("GetTrianglePhaseAnalysisSpeedMultiplier()"),
            "Estado, tiempo o efectos dejaron de usar formulas canonicas.", failures);
        Check(slotSource.Contains("GameState.I.SetTriangleCircuit(circuit)"),
            "Los selectores no llaman SetTriangleCircuit.", failures);
        Check(presentationSource.Contains("state.triangleActiveCircuit") &&
            presentationSource.Contains("state.IsTrianglePhaseUnlocked()"),
            "La iluminacion no sigue circuito activo y gate de Fase.", failures);
    }

    private static void ValidateArtifactCards(
        GameObject root, List<string> failures)
    {
        VerticalTriangleArtifactCardUI[] cards =
            root.GetComponentsInChildren<VerticalTriangleArtifactCardUI>(true);
        Check(cards.Length == 3, "Deben existir tres tarjetas de vertices.", failures);
        var ids = new HashSet<string>();
        foreach (VerticalTriangleArtifactCardUI card in cards)
        {
            ids.Add(card.buildingId);
            Check(card.nameText != null && card.stateText != null &&
                card.icon != null && card.buyButton != null,
                card.name + " tiene referencias incompletas.", failures);
            RectTransform buttonRect = card.buyButton != null
                ? card.buyButton.transform as RectTransform
                : null;
            LayoutElement buttonLayout = card.buyButton != null
                ? card.buyButton.GetComponent<LayoutElement>()
                : null;
            Check(buttonRect != null && buttonLayout != null &&
                buttonLayout.minWidth >= 120f && buttonLayout.minHeight >= 50f,
                card.name + " tiene compra demasiado pequena.", failures);
        }
        Check(ids.Contains("vacuum_observer") && ids.Contains("casimir_panel") &&
            ids.Contains("fluctuation_antenna"),
            "Las tarjetas no representan los tres edificios reales.", failures);
    }

    private static void ValidateSharedEconomy(List<string> failures)
    {
        string rowSource = File.ReadAllText(
            "Assets/Project/Scripts/Buildings/BuildingRowUI.cs");
        string cardSource = File.ReadAllText(
            "Assets/Project/Scripts/UI/Vertical/VerticalTriangleArtifactCardUI.cs");
        string purchaseSource = File.ReadAllText(
            "Assets/Project/Scripts/Buildings/BuildingPurchaseService.cs");
        string listSource = File.ReadAllText(
            "Assets/Project/Scripts/Buildings/BuildingListUI.cs");
        Check(rowSource.Contains("BuildingPurchaseService.TryPurchase(gameState, state)") &&
            cardSource.Contains("BuildingPurchaseService.TryPurchase(GameState.I, state)"),
            "Las dos presentaciones no comparten la misma compra.", failures);
        Check(purchaseSource.Contains("BuildingUnlock.IsUnlocked(state.def)") &&
            purchaseSource.Contains("gameState.GetEffectiveBuildingCost(state)") &&
            purchaseSource.Contains("gameState.LE -= effectiveCost") &&
            purchaseSource.Contains("state.OnPurchased()") &&
            purchaseSource.Contains("D3ConsoleSystem.RecordManualBuildingPurchase"),
            "El servicio compartido altero el contrato de compra.", failures);
        Check(listSource.Contains("public bool EnsureInitialized()") &&
            listSource.Contains("if (initialized)") &&
            listSource.Contains("gs.RegisterBuildingState(state)"),
            "El bootstrap de edificios no es unico e idempotente.", failures);
        Check(cardSource.Contains("buildingId == \"fluctuation_antenna\" && state.level > 0") &&
            cardSource.Contains("buyButton.interactable = !modulatorOwned"),
            "El Modulador permite compras repetidas.", failures);
    }

    private static void ValidateNoLegacyPresentation(
        GameObject root, List<string> failures)
    {
        Check(root.GetComponentsInChildren<F2UpgradeRowUI>(true).Length == 0,
            "Mejoras F2 reaparecieron en Generacion.", failures);
        foreach (TextMeshProUGUI text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            string value = (text.text ?? string.Empty).ToLowerInvariant();
            Check(!value.Contains("detectado") && !value.Contains("detected"),
                "La presentacion detectado sigue visible en " + text.name + ".", failures);
        }
    }

    private static void ValidateLegacyBackup(Scene scene, List<string> failures)
    {
        GameObject legacy = FindNamed(scene, "LegacyGenerationTriangleLayout");
        Check(legacy != null && !legacy.activeSelf,
            "El layout horizontal heredado no se conservo inactivo.", failures);
        Check(legacy != null && legacy.GetComponentInChildren<TrianglePanelUI>(true) != null,
            "El respaldo heredado perdio su logica original.", failures);
    }

    private static void ValidateNoDuplicates(Scene scene, List<string> failures)
    {
        string[] names =
        {
            "GenerationTriangleRoot", "TriangleScroll", "TriangleFocus",
            "CircuitSelectors", "Circuit_Energy", "Circuit_Experimental",
            "Circuit_Phase", "TriangleArtifactCards", "TriangleCard_Higgs",
            "TriangleCard_Tetra", "TriangleCard_Modulator"
        };
        foreach (string name in names)
            Check(CountNamed(scene, name) == 1,
                name + " esta ausente o duplicado.", failures);
    }

    private static GameObject FindNamed(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    return current.gameObject;
        return null;
    }

    private static int CountNamed(Scene scene, string name)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                if (current.name == name)
                    count++;
        return count;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }

    private static void Finish(List<string> failures)
    {
        if (failures.Count == 0)
        {
            Debug.Log("[Vertical UI Block 4] PASS | triangle focus | three circuits | " +
                "phase gate | real synchronization and effects | shared purchases | no detected cards");
            return;
        }
        foreach (string failure in failures)
            Debug.LogError("[Vertical UI Block 4] " + failure);
        throw new InvalidOperationException(
            "Vertical UI Block 4 fallo con " + failures.Count + " error(es).");
    }
}
#endif
