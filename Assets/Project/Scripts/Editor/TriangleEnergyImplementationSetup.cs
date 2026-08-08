#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TriangleEnergyImplementationSetup
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/Triangle Energy/Configure and Validate")]
    public static void ConfigureAndValidateBatch()
    {
        try
        {
            UpgradeStudyObservatorySetup.ConfigureAndValidateBatch();
            VerticalGenerationVisualPolish.ApplyVisualPolish();
            VerticalGenerationVisualPolish.ApplyVisualPolish();
            VerticalUpgradesVisualPolish.ApplyVisualPolish();
            VerticalUpgradesVisualPolish.ApplyVisualPolish();
            ValidateScene();
            UpgradeStudyValidation.ValidateLogic();
            Debug.Log("[Triangle Energy] PASS | scene configured twice | studies | three resources | compact generation");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ValidateScene()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        HUD hud = UnityEngine.Object.FindFirstObjectByType<HUD>(FindObjectsInactive.Include);
        VerticalUpgradesScreenUI upgrades =
            UnityEngine.Object.FindFirstObjectByType<VerticalUpgradesScreenUI>(
                FindObjectsInactive.Include);
        GameObject generationHeader = FindNamed(scene, "VerticalGenerationHeader");
        GameObject upgradesHeader = FindNamed(scene, "VerticalUpgradesHeader");
        GameObject studyConsole = FindNamed(scene, "UpgradeStudyConsole");
        GameObject legacyObservatory = FindNamed(scene, "TriangleObservatory");
        GameObject purchasesFrame = FindNamed(scene, "TrianglePurchasesFrame");

        Require(hud != null && hud.leText != null && hud.tracesText != null &&
            hud.energyText != null, "El HUD no enlaza los tres recursos.");
        Require(generationHeader != null &&
            generationHeader.transform.Find("Resource_LE") != null &&
            generationHeader.transform.Find("Resource_Traces") != null &&
            generationHeader.transform.Find("Resource_Energy") != null,
            "La cabecera de Generación no contiene tres recursos.");
        Require(upgradesHeader != null &&
            upgradesHeader.transform.Find("UpgradeResource_Energy") != null,
            "Mejoras no muestra la Energía usada por los estudios.");
        Require(legacyObservatory == null && purchasesFrame != null,
            "La pantalla principal no retiró el Observatorio antiguo.");
        Require(upgrades != null && upgrades.content != null && studyConsole != null &&
            studyConsole.transform.IsChildOf(upgrades.content) &&
            studyConsole.GetComponent<VerticalTriangleObservatoryUI>() != null,
            "La consola de estudios no vive dentro de Mejoras.");
        Require(upgrades.GetComponentsInChildren<F2UpgradeRowUI>(true)
            .Any(row => row.UpgradeId == "triangle_energy_efficiency"),
            "Falta la mejora Captación Resonante.");
        Require(CountNamed(scene, "Resource_Energy") == 1 &&
            CountNamed(scene, "UpgradeStudyConsole") == 1 &&
            CountNamed(scene, "TrianglePurchasesFrame") == 1,
            "La configuración no es idempotente.");

        int missingScripts = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Transform current in root.GetComponentsInChildren<Transform>(true))
                missingScripts += current.GetComponents<Component>()
                    .Count(component => component == null);
        Require(missingScripts == 0,
            "La escena contiene Missing Scripts: " + missingScripts + ".");
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

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
#endif
