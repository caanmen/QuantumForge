using System;

/// <summary>
/// Definición "estática" de un edificio.
/// Esta clase describe cómo es un edificio:
/// - Id interno
/// - Nombre que se muestra
/// - Descripción
/// - Coste base
/// - Multiplicador de coste por nivel
/// - Producción base de LE por segundo
/// </summary>
[Serializable]
public class BuildingDef
{
    // Identificador interno único (usado en el JSON y en el código)
    public string id;

    // Nombre que verá el jugador en la UI (por ejemplo: "Observador de Vacío")
    public string displayName;

    // Texto descriptivo opcional para tooltips
    public string description;

    // Coste inicial en LE del nivel 1
    public double baseCost;

    // Multiplicador de coste por cada nivel adicional (ej: 1.15 = +15% por nivel)
    public double costMult;

    // Producción base de LE por segundo para el nivel 1
    public double baseLEps;
}
