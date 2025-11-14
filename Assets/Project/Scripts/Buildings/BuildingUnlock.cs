using UnityEngine;

/// <summary>
/// Lógica de desbloqueo (gating suave) de edificios.
/// Usa los campos de BuildingDef:
/// - unlockMinLE
/// - unlockMinTotalLEps
/// - unlockRequireId / unlockRequireLevel
/// 
/// Regla importante:
/// - Si el edificio ya tiene nivel > 0, se considera siempre DESBLOQUEADO,
///   aunque luego baje el LE o cambien otras condiciones.
/// </summary>
public static class BuildingUnlock
{
    /// <summary>
    /// Devuelve true si el edificio ya está desbloqueado según el estado actual del juego.
    /// </summary>
    public static bool IsUnlocked(BuildingDef def)
    {
        if (def == null) return false;

        var gs = GameState.I;
        if (gs == null) return false;

        // 0) Si este edificio ya tiene nivel > 0, se considera desbloqueado para siempre.
        int ownLevel = gs.GetBuildingLevel(def.id);
        if (ownLevel > 0)
        {
            return true;
        }

        // 1) Requisito de LE actual (solo mientras el nivel es 0)
        if (def.unlockMinLE > 0.0 && gs.LE < def.unlockMinLE)
            return false;

        // 2) Requisito de LE/s total (solo mientras el nivel es 0)
        double totalLEps = gs.GetTotalLEps();
        if (def.unlockMinTotalLEps > 0.0 && totalLEps < def.unlockMinTotalLEps)
            return false;

        // 3) Requisito de otro edificio a cierto nivel (solo mientras el nivel es 0)
        if (!string.IsNullOrEmpty(def.unlockRequireId) && def.unlockRequireLevel > 0)
        {
            int otherLevel = gs.GetBuildingLevel(def.unlockRequireId);
            if (otherLevel < def.unlockRequireLevel)
                return false;
        }

        // Si pasa todos los requisitos, está desbloqueado (nivel 0 y listo para comprar)
        return true;
    }
}
