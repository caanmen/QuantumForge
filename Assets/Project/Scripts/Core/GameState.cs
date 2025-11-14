using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Recursos básicos")]
    public double LE = 0.0;   // Luz de Energía (recurso principal)
    public double VP = 0.0;   // Vacuum Points (recurso raro, aún sin lógica)

    [Header("Recursos avanzados (placeholder)")]
    [Tooltip("Recurso para el futuro sistema de BEC (aún sin implementar).")]
    public double BEC = 0.0;  // condensado de Bose-Einstein (futuro)

    [Header("Producción base (sin edificios)")]
    public double baseLEps = 0.5;   // producción base sin edificios

    // Lista de edificios que producen LE (se llena desde la UI / BuildingList)
    private List<BuildingState> buildingStates = new List<BuildingState>();

    [Header("Decoherencia (soft cap) - DESACTIVADA POR AHORA")]
    [Tooltip("Por ahora no afecta la producción. Más adelante se reutilizará.")]
    public bool useDecoherence = false;   // << clave: queda en false

    [Tooltip("A partir de esta cantidad de LE almacenada empezaría la decoherencia (futuro).")]
    public double decoStartLE = 3000.0;

    [Tooltip("Qué tan rápido caería la producción cuando te pases del umbral (futuro).")]
    public double decoStrength = 0.00004;

    [Tooltip("Factor mínimo de producción (0.6 = nunca baja de 60%) (futuro).")]
    public double decoMinFactor = 0.6;

    // Debug: acumulador de tiempo para logs
    private float _dbg = 0f;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        double dt = Time.unscaledDeltaTime;
        Tick(dt);

        _dbg += Time.unscaledDeltaTime;
        if (_dbg >= 1f)
        {
            double totalLEps = GetTotalLEps();
            Debug.Log($"[GameState] LE = {LE:0.000} | LE/s total = {totalLEps:0.00}");
            _dbg = 0f;
        }
    }

    /// <summary>
    /// Avanza el juego dt segundos (lógica principal de producción).
    /// </summary>
    public void Tick(double dt)
    {
        double totalLEps = CalculateTotalLEps();
        LE += totalLEps * dt;
    }

    /// <summary>
    /// Calcula la producción total de LE/s:
    /// - producción base
    /// - producción de edificios
    /// - bonus globales
    /// (Por ahora SIN decoherencia).
    /// </summary>
    private double CalculateTotalLEps()
    {
        double baseProd = baseLEps;
        double fromBuildings = 0.0;
        double multiplier = 1.0;
        double flatBonus = 0.0;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;

            double buildingProd = b.GetLEps();
            fromBuildings += buildingProd;

            if (b.level <= 0) continue;

            switch (b.def.bonusType)
            {
                case BuildingBonusType.None:
                    break;

                case BuildingBonusType.MultiplierLE:
                    multiplier += b.def.bonusPerLevel * b.level;
                    break;

                case BuildingBonusType.FlatLE:
                    flatBonus += b.def.bonusPerLevel * b.level;
                    break;
            }
        }

        double rawTotal = (baseProd + fromBuildings) * multiplier + flatBonus;

        // Por ahora NO aplicamos decoherencia
        // Si en el futuro queremos reactivarla, se hace aquí.
        // if (useDecoherence)
        //     rawTotal = ApplyDecoherence(rawTotal);

        return rawTotal;
    }

    /// <summary>
    /// Placeholder: por ahora no se usa.
    /// </summary>
    private double ApplyDecoherence(double rawLEps)
    {
        // Devuelve tal cual, sin cambios.
        return rawLEps;

        // Cuando queramos reusar esta mecánica, aquí se reactivará la lógica.
    }

    public void RegisterBuildingState(BuildingState state)
    {
        if (state == null) return;
        if (!buildingStates.Contains(state))
        {
            buildingStates.Add(state);
        }
    }

    /// <summary>
    /// Devuelve la producción total de LE por segundo.
    /// </summary>
    public double GetTotalLEps()
    {
        return CalculateTotalLEps();
    }

    /// <summary>
    /// Devuelve el nivel actual de un edificio por id.
    /// </summary>
    public int GetBuildingLevel(string id)
    {
        if (string.IsNullOrEmpty(id)) return 0;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.def.id == id)
            {
                return b.level;
            }
        }

        return 0;
    }
}
