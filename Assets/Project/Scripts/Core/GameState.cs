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

    // Si luego quieres ZPE u otro recurso, lo añadimos aquí:
    // public double ZPE = 0.0;

    [Header("Producción base (sin edificios)")]
    public double baseLEps = 0.5;   // producción base sin edificios

    // Lista de edificios que producen LE (se llena desde la UI / BuildingList)
    private List<BuildingState> buildingStates = new List<BuildingState>();

    [Header("Decoherencia (soft cap)")]
    [Tooltip("Activa o desactiva la pérdida suave de producción.")]
    public bool useDecoherence = true;

    [Tooltip("A partir de esta cantidad de LE almacenada empieza la decoherencia.")]
    public double decoStartLE = 1000.0;

    [Tooltip("Fuerza de la decoherencia. Valores pequeños = efecto suave.")]
    public double decoStrength = 0.0005;

    [Tooltip("Factor mínimo de producción (ej: 0.2 = 20% de la producción máxima).")]
    public double decoMinFactor = 0.2;

    // temporizador para logs
    private float _dbg;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        // por si el timescale quedó en 0
        Time.timeScale = 1f;
    }

    private void Update()
    {
        double dt = Time.unscaledDeltaTime;
        Tick(dt);

        // LOG cada segundo para verificar que corre
        _dbg += Time.unscaledDeltaTime;
        if (_dbg >= 1f)
        {
            double totalLEps = GetTotalLEps();
            Debug.Log($"[GameState] LE = {LE:0.000} | LE/s total (con decoherencia) = {totalLEps:0.00}");
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

        // Aquí en el futuro podremos generar VP, BEC, etc.
        // Por ahora no hacemos nada con esos recursos.
    }

    /// <summary>
    /// Calcula la producción total de LE/s:
    /// - producción base
    /// - + producción de cada edificio
    /// - bonus globales (MultiplierLE y FlatLE)
    /// - y aplica la decoherencia (soft cap) si está activada.
    /// </summary>
    private double CalculateTotalLEps()
    {
        // Producción base de LE/s (sin edificios)
        double baseProd = baseLEps;

        // Producción directa de edificios
        double fromBuildings = 0.0;

        // Bonus globales
        double multiplier = 1.0; // 1 = 100%
        double flatBonus = 0.0;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;

            // Producción base de este edificio (según nivel)
            double buildingProd = b.GetLEps();
            fromBuildings += buildingProd;

            // Si no tiene niveles, no aporta bonus
            if (b.level <= 0) continue;

            // Bonus según el tipo definido en BuildingDef
            switch (b.def.bonusType)
            {
                case BuildingBonusType.MultiplierLE:
                    // Ej: bonusPerLevel = 0.05 → +5% por nivel
                    multiplier += b.def.bonusPerLevel * b.level;
                    break;

                case BuildingBonusType.FlatLE:
                    // LE/s plano extra
                    flatBonus += b.def.bonusPerLevel * b.level;
                    break;
            }
        }

        // Producción total "cruda" antes de decoherencia
        double rawTotal = (baseProd + fromBuildings) * multiplier + flatBonus;

        // Aplicar decoherencia si está activada
        if (useDecoherence)
        {
            rawTotal = ApplyDecoherence(rawTotal);
        }

        return rawTotal;
    }

    /// <summary>
    /// Aplica una pérdida suave (soft cap) a la producción total de LE/s
    /// basada en la cantidad actual de LE almacenada.
    /// </summary>
    private double ApplyDecoherence(double rawLEps)
    {
        if (rawLEps <= 0.0) return 0.0;

        // Si aún no alcanzamos el umbral, no hay decoherencia
        if (LE <= decoStartLE)
            return rawLEps;

        double exceso = LE - decoStartLE;

        // factor va bajando a medida que el exceso crece
        double factor = 1.0 / (1.0 + decoStrength * exceso);

        // clamp entre decoMinFactor y 1
        if (factor < decoMinFactor) factor = decoMinFactor;
        if (factor > 1.0) factor = 1.0;

        return rawLEps * factor;
    }

    /// <summary>
    /// Registrar un edificio para que su producción se tenga en cuenta.
    /// Lo llama BuildingListUI cuando crea cada BuildingState.
    /// </summary>
    public void RegisterBuildingState(BuildingState state)
    {
        if (state == null) return;
        if (!buildingStates.Contains(state))
        {
            buildingStates.Add(state);
        }
    }

    /// <summary>
    /// Devuelve la producción total de LE por segundo (con decoherencia incluida).
    /// </summary>
    public double GetTotalLEps()
    {
        return CalculateTotalLEps();
    }

    /// <summary>
    /// Devuelve el nivel actual de un edificio por id.
    /// Si no existe en la lista, devuelve 0.
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
