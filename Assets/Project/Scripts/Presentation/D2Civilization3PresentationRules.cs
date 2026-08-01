using System;
using System.Collections.Generic;


public static class D2Civilization3PresentationRules
{
    public const double ZoneTeaserRange = 10.0;

    public static bool HasExcavationProgress(D2Civilization3State state)
    {
        if (state?.zones == null) return false;
        foreach (D2C3ZoneState zone in state.zones)
            if (zone != null && (zone.excavationActive ||
                zone.totalExcavationsCompleted > 0L ||
                D2Civilization3System.GetTotalRemains(zone) > 0L)) return true;
        return false;
    }

    public static bool HasAnalysisProgress(D2Civilization3State state)
    {
        if (state?.zones == null) return false;
        foreach (D2C3ZoneState zone in state.zones)
            if (zone != null && (zone.analysisActive ||
                zone.totalAnalysesCompleted > 0L || zone.researchProgress > 0.0))
                return true;
        return false;
    }

    public static bool HasClueProgress(D2Civilization3State state)
    {
        if (state?.zones == null) return false;
        foreach (D2C3ZoneState zone in state.zones)
            if (zone != null && (zone.anomalyClues > 0L ||
                zone.anomalyClueProgress > 0.0)) return true;
        return false;
    }

    public static bool HasAnomalyProgress(D2Civilization3State state)
    {
        if (state?.zones == null) return false;
        foreach (D2C3ZoneState zone in state.zones)
            if (zone != null && (zone.anomalyRevealed || zone.anomalyRead ||
                zone.anomalousData > 0L)) return true;
        return false;
    }

    public static string[] GetVisibleZoneIds(D2Civilization3State state)
    {
        var result = new List<string> { D2Civilization3System.Zone1Id };
        D2C3ZoneState zone1 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone1Id);
        D2C3ZoneState zone2 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone2Id);
        D2C3ZoneState zone3 = D2Civilization3System.GetZone(
            state, D2Civilization3System.Zone3Id);
        if ((zone2 != null && zone2.unlocked) ||
            (zone1 != null && zone1.researchProgress >=
                D2Civilization3System.Zone2UnlockResearchRequirement -
                ZoneTeaserRange))
            result.Add(D2Civilization3System.Zone2Id);
        if (zone2 != null && zone2.unlocked &&
            ((zone3 != null && zone3.unlocked) ||
             zone2.researchProgress >=
                D2Civilization3System.Zone3UnlockResearchRequirement -
                ZoneTeaserRange))
            result.Add(D2Civilization3System.Zone3Id);
        return result.ToArray();
    }

    public static string[] GetPossessedQualityIds(D2C3ZoneState zone)
    {
        var result = new List<string>();
        if (zone == null) return result.ToArray();
        if (zone.lowQualityRemains > 0L ||
            zone.analysisQualityId == D2Civilization3System.LowQualityId)
            result.Add(D2Civilization3System.LowQualityId);
        if (zone.mediumQualityRemains > 0L ||
            zone.analysisQualityId == D2Civilization3System.MediumQualityId)
            result.Add(D2Civilization3System.MediumQualityId);
        if (zone.highQualityRemains > 0L ||
            zone.analysisQualityId == D2Civilization3System.HighQualityId)
            result.Add(D2Civilization3System.HighQualityId);
        return result.ToArray();
    }

    public static string GetCurrentArchiveDiscovery(D2Civilization3State state)
    {
        if (state == null || !state.archiveUnlocked)
            return "Completa el primer análisis para descubrir el Archivo I.";
        if (state.archiveLevel < 2)
            return "Próximo descubrimiento: Zona 1 al 40%.";
        if (state.archiveLevel < 3)
            return "Próximo descubrimiento: Zona 2 al 40%.";
        if (state.archiveLevel < 4)
            return "Próximo descubrimiento: Zona 3 al 30%.";
        return "Cadena conocida completada. Abre DETALLES para consultar mejoras.";
    }

    public static bool Contains(string[] values, string id)
    {
        return Array.IndexOf(values ?? Array.Empty<string>(), id) >= 0;
    }
}
