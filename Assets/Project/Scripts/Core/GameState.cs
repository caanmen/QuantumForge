using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Recursos")]
    public double LE = 0.0;
    public double VP = 0.0;

    [Header("Producción (por segundo)")]
    public double baseLEps = 0.5;

    // Lista de edificios que producen LE
    private List<BuildingState> buildingStates = new List<BuildingState>();

    private float _dbg; // temporizador para logs

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f; // por si el timescale quedó en 0
    }

    private void Update()
    {
        // En vez de sumar solo baseLEps, usamos la lógica de Tick
        double dt = Time.unscaledDeltaTime;
        Tick(dt);

        // LOG cada segundo para verificar que corre
        _dbg += Time.unscaledDeltaTime;
        if (_dbg >= 1f)
        {
            // Opcional: calcular LE/s total para el log
            double totalLEps = baseLEps;
            foreach (var b in buildingStates)
            {
                if (b == null) continue;
                totalLEps += b.GetLEps();
            }

            Debug.Log($"[GameState] LE = {LE:0.000} | LE/s total = {totalLEps:0.00}");
            _dbg = 0f;
        }
    }

    public void Tick(double dt)
    {
        // Producción base
        double totalLEps = baseLEps;

        // Sumar producción de todos los edificios registrados
        foreach (var b in buildingStates)
        {
            if (b == null) continue;
            totalLEps += b.GetLEps();
        }

        // Aplicar la producción total de LE/s al recurso LE
        LE += totalLEps * dt;
    }

    /// <summary>
    /// Registrar un edificio para que su producción se tenga en cuenta en Tick().
    /// Lo llamaremos desde BuildingListUI cuando creemos cada BuildingState.
    /// </summary>
    public void RegisterBuildingState(BuildingState state)
    {
        if (state == null)
            return;

        if (!buildingStates.Contains(state))
        {
            buildingStates.Add(state);
        }
    }

        /// <summary>
    /// Devuelve la producción total de LE por segundo:
    /// producción base + todos los edificios.
    /// </summary>
    public double GetTotalLEps()
    {
        double totalLEps = baseLEps;

        foreach (var b in buildingStates)
        {
            if (b == null) continue;
            totalLEps += b.GetLEps();
        }

        return totalLEps;
    }

}
