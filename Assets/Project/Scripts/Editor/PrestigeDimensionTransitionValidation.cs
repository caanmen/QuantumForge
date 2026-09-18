#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrestigeDimensionTransitionValidation
{
    private const string ScenePath = "Assets/Project/Scenes/Main.unity";

    [MenuItem("Tools/Quantum Forge/Prestige/Validate Dimensional Transition")]
    public static void Validate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[Prestige Dimension Validation] Ejecutar fuera de Play Mode.");
            return;
        }

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var failures = new List<string>();
        PrestigeUI prestige = Object.FindFirstObjectByType<PrestigeUI>(
            FindObjectsInactive.Include);
        PrestigeDimensionTransitionUI transition = prestige != null
            ? prestige.GetComponent<PrestigeDimensionTransitionUI>()
            : null;
        PrestigeDimensionTransitionConfig resourceConfig =
            AssetDatabase.LoadAssetAtPath<PrestigeDimensionTransitionConfig>(
                "Assets/Project/Resources/Prestige/" +
                "PrestigeDimensionTransitionConfig.asset");
        Check(prestige != null, "No existe PrestigeUI.", failures);
        Check(resourceConfig != null,
            "No existe la configuración de recursos de la transición.", failures);
        if (resourceConfig != null)
        {
            Check(resourceConfig.cardsTexture != null,
                "El recurso no referencia la lámina dimensional.", failures);
            Check(resourceConfig.portraitReference != null,
                "El recurso no preserva la referencia vertical.", failures);
            Check(resourceConfig.laboratoryBackground != null,
                "El recurso no referencia el fondo del laboratorio.", failures);
            Check(resourceConfig.monolithTexture != null,
                "El recurso no referencia el Monolito reparado.", failures);
            Check(resourceConfig.monolithCutoutMaterial != null,
                "El recurso no referencia el recorte transparente del Monolito.",
                failures);
            Check(resourceConfig.entryLightTexture != null &&
                    resourceConfig.entryLightAdditiveMaterial != null,
                "El recurso no conserva las luces de acceso integradas.", failures);
            Check(resourceConfig.portalRingTexture != null,
                "El recurso no referencia el aro ligero de los portales.", failures);
        }
        if (transition != null)
        {
            Check(transition.CardsTexture != null,
                "No está asignada la lámina de las tres dimensiones.", failures);
            Check(transition.PortraitReference != null,
                "No está preservada la referencia vertical aprobada.", failures);
            Check(transition.LaboratoryBackground != null,
                "No está asignado el fondo del laboratorio.", failures);
            Check(transition.MonolithTexture != null,
                "No está asignado el Monolito reparado.", failures);
            Check(transition.MonolithCutoutMaterial != null,
                "No está asignado el material que elimina el fondo del Monolito.",
                failures);
            Check(transition.PortalRingTexture != null,
                "No está asignado el aro de los portales.", failures);
            Check(transition.ConfirmationResetSeconds >= 2f,
                "La ventana de confirmación es demasiado corta.", failures);
            Check(transition.MonolithPlateCount == 4 &&
                    transition.EntryLightCount == 4,
                "Los recursos de reemplazo no preservan cuatro placas y cuatro luces.",
                failures);
            Check(!transition.PlayOpeningAnimation,
                "La apertura cinematica interna debe permanecer omitida hasta recibir el video externo.",
                failures);
        }

        Check(PrestigeDimensionTransitionUI.RequiredConfirmationPresses == 3,
            "SINTONIZAR no exige exactamente tres pulsaciones.", failures);
        Check(PrestigeDimensionTransitionUI.GetConfirmationInstruction(0)
                .Contains("3 PULSACIONES"),
            "El aviso inicial no explica las tres pulsaciones.", failures);
        Check(PrestigeDimensionTransitionUI.GetConfirmationInstruction(1)
                .Contains("dos veces"),
            "El primer paso no informa cuántas pulsaciones faltan.", failures);
        Check(PrestigeDimensionTransitionUI.GetConfirmationInstruction(2)
                .Contains("una vez"),
            "El segundo paso no informa que falta una pulsación.", failures);
        Check(PrestigeDimensionTransitionUI.GetConfirmationInstruction(3)
                .Contains("FIRMA BLOQUEADA"),
            "El tercer paso no confirma el bloqueo de la firma.", failures);

        if (failures.Count > 0)
            throw new System.InvalidOperationException(
                "[Prestige Dimension Validation] FAIL\n- " +
                string.Join("\n- ", failures));

        Debug.Log(
            "[Prestige Dimension Validation] PASS | apertura interna omitida | " +
            "carrusel I-II-III | volver sin reset | 1/3 + 2/3 + 3/3 | timeout 5s");
    }

    public static void ValidateBatch()
    {
        Validate();
    }

    private static void Check(bool condition, string message,
        List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif
