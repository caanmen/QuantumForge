using UnityEngine;

/// <summary>
/// Lógica de desbloqueo (gating suave) de edificios.
/// Usa los campos de BuildingDef:
/// - unlockMinLE
/// - unlockMinTotalLEps
/// - unlockRequireId / unlockRequireLevel
/// 
/// Reglas:
/// - Si el edificio ya tiene nivel > 0, SIEMPRE se considera desbloqueado.
/// - Si tiene unlockRequireId, el requisito principal es el nivel de ese edificio.
/// - Los requisitos por LE solo se aplican cuando NO hay unlockRequireId.
/// </summary>
public static class BuildingUnlock
{
    public static bool IsUnlocked(BuildingDef def)
    {
        if (def == null) return false;

        var gs = GameState.I;
        if (gs == null) return false;

        // 0) Si ya tiene nivel > 0 → siempre desbloqueado
        int currentLevel = gs.GetBuildingLevel(def.id);
        if (currentLevel > 0)
            return true;

        bool hasPrereqBuilding = !string.IsNullOrEmpty(def.unlockRequireId)
                                 && def.unlockRequireLevel > 0;

        // 1) Si hay edificio requisito, usamos SOLO ese requisito para early buildings
        if (hasPrereqBuilding)
        {
            int otherLevel = gs.GetBuildingLevel(def.unlockRequireId);
            if (otherLevel < def.unlockRequireLevel)
                return false;

            // Si el edificio requerido ya cumple el nivel,
            // NO miramos LE mínima ni LE/s mínima.
            return true;
        }

        // 2) Si NO hay edificio requisito, usamos requisitos por LE / LEps

        // LE mínima actual
        if (def.unlockMinLE > 0.0)
        {
            if (gs.LE < def.unlockMinLE)
                return false;
        }

        // LE/s mínima
        if (def.unlockMinTotalLEps > 0.0)
        {
            double totalLEps = gs.GetTotalLEps();
            if (totalLEps < def.unlockMinTotalLEps)
                return false;
        }

        // Si pasó todo lo anterior, se considera desbloqueado
        return true;
    }
}
