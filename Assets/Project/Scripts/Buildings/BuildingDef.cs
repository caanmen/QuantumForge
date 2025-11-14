using System;
using UnityEngine;

/// <summary>
/// Tipo de bonus pasivo que puede dar un edificio.
/// </summary>
[Serializable]
public enum BuildingBonusType
{
    None = 0,          // sin bonus
    MultiplierLE = 1,  // multiplica la producción total de LE/s
    FlatLE = 2         // suma LE/s plano
}

/// <summary>
/// Definición "estática" de un edificio.
/// Describe cómo es un edificio: id, nombre, costes, producción, tier, bonus y requisitos.
/// </summary>
[Serializable]
public class BuildingDef
{
    // Identificador interno único (usado en JSON y código)
    public string id;

    // Nombre visible en la UI (ej: "Observador de Vacío")
    public string displayName;

    // Descripción para tooltips
    public string description;

    // Coste inicial en LE del nivel 1
    public double baseCost;

    // Multiplicador de coste por cada nivel adicional (ej: 1.15 = +15% por nivel)
    public double costMult;

    // Producción base de LE por segundo para el nivel 1
    public double baseLEps;

    // ---------- F3.1: progresión y bonus ----------

    // Tier de progresión (1 = básico, 2 = más caro, etc.)
    public int tier = 1;

    // Tipo de bonus pasivo del edificio
    public BuildingBonusType bonusType = BuildingBonusType.None;

    // Valor del bonus por nivel:
    // - Si bonusType = MultiplierLE → 0.05 = +5% LE/s por nivel
    // - Si bonusType = FlatLE      → cantidad fija de LE/s por nivel
    public double bonusPerLevel = 0.0;

    // ---------- F3.2: requisitos de desbloqueo (gating suave) ----------

    // LE mínimo actual para que el edificio se desbloquee (0 = sin requisito)
    public double unlockMinLE = 0.0;

    // LE/s total mínimo para que se desbloquee (0 = sin requisito)
    public double unlockMinTotalLEps = 0.0;

    // Edificio requerido para desbloquear (id) – opcional
    public string unlockRequireId;

    // Nivel mínimo del edificio requerido (si unlockRequireId no es null/empty)
    public int unlockRequireLevel = 0;
}
