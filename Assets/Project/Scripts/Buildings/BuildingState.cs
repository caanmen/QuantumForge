using System;

[Serializable]
public class BuildingState
{
    public BuildingDef def;

    // Nivel actual del edificio
    public int level = 0;

    // Coste actual de la siguiente compra
    public double currentCost = 0.0;

    // ⏱ F8: timer acumulado para los ticks de este edificio
    public float tickTimer = 0f;

    

    /// <summary>
    /// Inicializa el estado a partir de la definición.
    /// Llamado desde BuildingListUI.
    /// </summary>
    public void InitFromDef(BuildingDef def)
    {
        this.def = def;

        if (level < 0)
            level = 0;

        // Si no hay coste inicial, recomputar desde baseCost
        if (currentCost <= 0.0)
        {
            if (def != null)
            {
                currentCost = def.baseCost;
                for (int i = 0; i < level; i++)
                {
                    currentCost *= def.costMult;
                }
            }
            else
            {
                currentCost = 0.0;
            }
        }

        // Reset del timer de ticks
        tickTimer = 0f;
    }

    /// <summary>
    /// ¿El jugador puede comprar un nivel más de este edificio?
    /// </summary>
    public bool CanAfford(double currentLE)
    {
        if (def == null) return false;

        if (currentCost <= 0.0)
        {
            currentCost = def.baseCost;
            for (int i = 0; i < level; i++)
            {
                currentCost *= def.costMult;
            }
        }

        return currentLE >= currentCost;
    }

    /// <summary>
    /// Llamado cuando se compra un nivel de este edificio.
    /// Actualiza nivel y coste siguiente.
    /// </summary>
    public void OnPurchased()
    {
        if (def == null) return;

        if (currentCost <= 0.0)
        {
            currentCost = def.baseCost;
            for (int i = 0; i < level; i++)
            {
                currentCost *= def.costMult;
            }
        }

        // Si estaba en 0 y lo compras, arranca el ciclo desde 0
        if (level == 0)
            tickTimer = 0f;

        level++;
        currentCost *= def.costMult;
    }


    /// <summary>
    /// Reset de edificio para un nuevo run de prestigio.
    /// </summary>
    public void ResetForPrestige()
    {
        level = 0;
        tickTimer = 0f;

        if (def != null)
        {
            currentCost = def.baseCost;
        }
        else
        {
            currentCost = 0.0;
        }
    }

    /// <summary>
    /// Producción media de LE/s que aporta este edificio.
    /// 
    /// - Si el edificio tiene tickInterval y lePerTickBase > 0 → lo tratamos como
    ///   "por tick" y devolvemos (LE por tick / intervalo).
    /// - Si no, lo tratamos como edificio clásico de LE/s: baseLEps * nivel.
    /// 
    /// NOTA: aquí devolvemos el valor "base", sin multiplicadores globales (EM, research, etc.).
    /// </summary>
    public double GetLEps()
    {
        if (def == null || level <= 0)
            return 0.0;

        // F8: edificios con ticks
        if (def.tickInterval > 0.0 && def.lePerTickBase > 0.0)
        {
            double lePerTick = def.lePerTickBase * level;

            // El buff especial del B1 por el B2 lo aplicaremos en GameState,
            // donde tenemos acceso a todos los edificios. Aquí solo devolvemos
            // la producción base de este edificio.
            return lePerTick / def.tickInterval;
        }

        // Comportamiento clásico (Fases anteriores): LE/s directo
        if (def.baseLEps > 0.0)
        {
            return def.baseLEps * level;
        }

        return 0.0;
    }
}
