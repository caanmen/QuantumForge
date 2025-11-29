using System;

/// <summary>
/// Estado en runtime de un edificio concreto.
/// Usa la definición estática (BuildingDef) como plantilla
/// y guarda cuántos niveles tienes y cuánto cuesta el siguiente.
/// </summary>
[Serializable]
public class BuildingState
{
    // Definición estática (datos que vienen del JSON)
    public BuildingDef def;

    // Nivel actual del edificio (cuántas veces lo has comprado)
    public int level;

    // Coste actual para comprar el siguiente nivel
    public double currentCost;

    /// <summary>
    /// Inicializa el estado a partir de una definición.
    /// </summary>
    public void InitFromDef(BuildingDef def)
    {
        this.def = def;
        level = 0;
        currentCost = def.baseCost;
    }

    /// <summary>
    /// Producción actual de LE/s según el nivel.
    /// </summary>
    public double GetLEps()
    {
        if (def == null || level <= 0)
            return 0.0;

        return def.baseLEps * level;
    }

    /// <summary>
    /// ¿El jugador puede pagar el siguiente nivel con la LE actual?
    /// </summary>
    public bool CanAfford(double currentLE)
    {
        return currentLE >= currentCost;
    }

    /// <summary>
    /// Llamar cuando se compra un nivel:
    /// - Sube el nivel
    /// - Aumenta el coste según el multiplicador
    /// </summary>
    public void OnPurchased()
    {
        if (def == null)
            return;

        level++;
        currentCost *= def.costMult;
    }

        // F6.3: Reset básico para prestigio.
    // Por ahora solo ponemos el nivel en 0; los costes se recalculan
    // donde toque (normalmente en la UI de la fila del edificio).
    public void ResetForPrestige()
    {
        level = 0;
    }

}
