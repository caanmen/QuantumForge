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
     
     
    private int GetMaxLevel()
        {
            if (def == null) return int.MaxValue;

            return int.MaxValue;
        }

    public static double CalculateCostForLevel(BuildingDef definition, int level)
    {
        if (definition == null) return 0.0;
        int safeLevel = Math.Max(0, level);
        if (definition.id == "fluctuation_antenna")
            return definition.baseCost * Math.Pow(definition.costMult, safeLevel);

        // El inicio conserva la curva original. Después del nivel 25 la
        // pendiente se suaviza para que un nivel ordinario no escale a horas.
        double exponent = safeLevel <= 25
            ? safeLevel
            : safeLevel <= 75
                ? 25.0 + (safeLevel - 25) * 0.55
                : 52.5 + (safeLevel - 75) * 0.30;
        return definition.baseCost * Math.Pow(definition.costMult, exponent);
    }

    

    /// <summary>
    /// Inicializa el estado a partir de la definición.
    /// Llamado desde BuildingListUI.
    /// </summary>
    public void InitFromDef(BuildingDef def)
    {
        this.def = def;

        if (level < 0)
            level = 0;

        int maxLevel = GetMaxLevel();
        if (level > maxLevel)
            level = maxLevel;

        // Si no hay coste inicial, recomputar desde baseCost
        if (currentCost <= 0.0)
        {
            if (def != null)
            {
                currentCost = CalculateCostForLevel(def, level);
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
                
                if (level >= GetMaxLevel())
                
                return false;

        if (currentCost <= 0.0)
        {
            currentCost = CalculateCostForLevel(def, level);
        }

        return currentLE >= currentCost;
    }

        public bool IsAtMaxLevel()
    {
        if (def == null) return true;
        return level >= GetMaxLevel();
    }

    /// <summary>
    /// Llamado cuando se compra un nivel de este edificio.
    /// Actualiza nivel y coste siguiente.
    /// </summary>
    public void OnPurchased()
    {
        if (def == null) return;

            if (level >= GetMaxLevel())
            
            return;

        if (currentCost <= 0.0)
        {
            currentCost = CalculateCostForLevel(def, level);
        }

        // Si estaba en 0 y lo compras, arranca el ciclo desde 0
        if (level == 0)
            tickTimer = 0f;

        level++;
        currentCost = CalculateCostForLevel(def, level);
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
            currentCost = CalculateCostForLevel(def, 0);
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
            lePerTick *= GameState.GetArtifactLevelMilestoneMultiplier(
                def.id, level);

            // El buff especial del B1 por el B2 lo aplicaremos en GameState,
            // donde tenemos acceso a todos los edificios. Aquí solo devolvemos
            // la producción base de este edificio.
            double interval = def.tickInterval;
            if (def.id == "vacuum_observer" && F2UpgradeManager.I != null)
                interval *= F2UpgradeManager.I.GetContainmentCycleMultiplier();
            return lePerTick / System.Math.Max(0.0001, interval);
        }

        // Comportamiento clásico (Fases anteriores): LE/s directo
        if (def.baseLEps > 0.0)
        {
            return def.baseLEps * level *
                GameState.GetArtifactLevelMilestoneMultiplier(def.id, level);
        }

        return 0.0;
    }
}
