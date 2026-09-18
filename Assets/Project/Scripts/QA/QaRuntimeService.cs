using System;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Autoridad única para la velocidad de simulación usada por herramientas QA.
/// No modifica Time.timeScale ni forma parte del estado guardado del juego.
/// </summary>
public static class QaRuntimeService
{
    private static readonly float[] AllowedMultipliers = { 1f, 5f, 10f, 20f };

    private static float simulationMultiplier = 1f;
    private static bool wasAccelerationUsedThisSession;

    // Solo se modifica por reflexión desde el validador de editor para cubrir
    // el comportamiento de una build pública sin exponer una API de runtime.
    private static bool? availabilityOverrideForValidation = null;

    public static bool IsAvailable
    {
        get
        {
            if (availabilityOverrideForValidation.HasValue)
                return availabilityOverrideForValidation.Value;

            return Application.isEditor || Debug.isDebugBuild;
        }
    }

    public static float SimulationMultiplier
    {
        get
        {
            if (!IsAvailable)
            {
                simulationMultiplier = 1f;
                return 1f;
            }

            if (!IsAllowedMultiplier(simulationMultiplier))
                simulationMultiplier = 1f;

            return simulationMultiplier;
        }
    }

    public static bool IsAccelerated => SimulationMultiplier > 1f;

    /// <summary>
    /// Indicador de diagnóstico. No interviene en fórmulas, recompensas ni saves.
    /// </summary>
    public static bool WasAccelerationUsedThisSession =>
        wasAccelerationUsedThisSession;

    public static event Action<float> SpeedChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void DisableConflictingRenderingDebugger()
    {
        if (!ShouldDisableRenderingDebugger(
                Application.isEditor, Debug.isDebugBuild))
        {
            return;
        }

        // El Rendering Debugger de URP usa doble toque con tres dedos y crea
        // un Canvas que captura los toques del panel QA. Las herramientas QA
        // propias permanecen disponibles mediante Debug.isDebugBuild.
        DebugManager.instance.displayRuntimeUI = false;
        DebugManager.instance.enableRuntimeUI = false;
    }

    private static bool ShouldDisableRenderingDebugger(
        bool isEditor, bool isDebugBuild)
    {
        return !isEditor && isDebugBuild;
    }

    public static bool TrySetSpeed(float multiplier)
    {
        if (!IsAvailable || !IsAllowedMultiplier(multiplier))
        {
            SetSpeedInternal(1f);
            return false;
        }

        SetSpeedInternal(multiplier);
        if (multiplier > 1f)
        {
            wasAccelerationUsedThisSession = true;
            Debug.Log("[QA Runtime] Velocidad de simulación usada: x" +
                multiplier.ToString("0"));
        }

        return true;
    }

    public static float CycleSpeed()
    {
        if (!IsAvailable)
        {
            SetSpeedInternal(1f);
            return 1f;
        }

        float current = SimulationMultiplier;
        for (int index = 0; index < AllowedMultipliers.Length; index++)
        {
            if (AllowedMultipliers[index] != current)
                continue;

            float next = AllowedMultipliers[
                (index + 1) % AllowedMultipliers.Length];
            TrySetSpeed(next);
            return next;
        }

        SetSpeedInternal(1f);
        return 1f;
    }

    public static double ScaleOnlineSeconds(double realSeconds)
    {
        if (double.IsNaN(realSeconds) || double.IsInfinity(realSeconds) ||
            realSeconds < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(realSeconds),
                "Los segundos online deben ser finitos y mayores o iguales a cero.");
        }

        return realSeconds * SimulationMultiplier;
    }

    public static void ResetToNormalSpeed()
    {
        SetSpeedInternal(1f);
    }

    private static bool IsAllowedMultiplier(float multiplier)
    {
        if (float.IsNaN(multiplier) || float.IsInfinity(multiplier))
            return false;

        for (int index = 0; index < AllowedMultipliers.Length; index++)
        {
            if (multiplier == AllowedMultipliers[index])
                return true;
        }

        return false;
    }

    private static void SetSpeedInternal(float multiplier)
    {
        float previous = simulationMultiplier;
        simulationMultiplier = multiplier;
        if (previous != multiplier)
            SpeedChanged?.Invoke(multiplier);
    }
}
