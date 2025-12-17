using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tipo de bonus que un edificio aplica a la producción global de LE.
/// </summary>
[Serializable]
public enum BuildingBonusType
{
    None,
    MultiplierLE,
    FlatLE
}

/// <summary>
/// Definición de un edificio, cargada desde buildings.json
/// </summary>
[Serializable]
public class BuildingDef
{
    [Header("Identidad")]
    public string id;
    public string displayName;
    [TextArea]
    public string description;

    [Header("Costes")]
    public double baseCost = 10.0;
    public double costMult = 1.15;

    [Header("Producción clásica (Fases antiguas)")]
    // Producción base de LE/s si NO usamos ticks
    public double baseLEps = 0.0;

    [Header("Producción por tick (F8)")]
    // Intervalo de tick en segundos. Si es <= 0, el edificio se considera "continuo".
    public double tickInterval = 0.0;

    // LE base generado por tick, a nivel 1.
    public double lePerTickBase = 0.0;

    // EM base generado por tick, a nivel 1 (lo usaremos más adelante si queremos que
    // ciertos edificios generen EM por tick).
    public double emPerTickBase = 0.0;

    [Header("Tier y bonus especiales")]
    public int tier = 1;
    public BuildingBonusType bonusType = BuildingBonusType.None;
    public double bonusPerLevel = 0.0;

    [Header("Desbloqueo (gating suave)")]
    public double unlockMinLE = 0.0;
    public double unlockMinTotalLEps = 0.0;
    public string unlockRequireId;
    public int unlockRequireLevel = 0;
}

/// <summary>
/// Contenedor para que JsonUtility pueda leer { "buildings": [...] }.
/// </summary>
[Serializable]
public class BuildingCollection
{
    public List<BuildingDef> buildings;
}
