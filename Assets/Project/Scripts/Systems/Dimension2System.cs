using System;
using UnityEngine;


public static class Dimension2System
{
    public const int ProgressVersion = 1;
    public const int Civilization1ProgressVersion = 9;
    public const int Civilization2ProgressVersion = 1;
    public const int Civilization3ProgressVersion = 1;

    public const string Civilization1TerritoryId = "d2_civilization_1";
    public const string Civilization2TerritoryId = "d2_civilization_2";
    public const string Civilization3TerritoryId = "d2_civilization_3";

    public const double OfflineProgressCapSeconds = 12.0 * 60.0 * 60.0;

    public static readonly string[] TerritoryIds =
    {
        Civilization1TerritoryId,
        Civilization2TerritoryId,
        Civilization3TerritoryId
    };

    public static void EnsureState(GameState state)
    {
        if (state == null)
            return;

        if (state.dimension2 == null)
            state.dimension2 = CreateInitialState();

        Dimension2State dimension2 = state.dimension2;
        dimension2.progressVersion = ProgressVersion;
        dimension2.civilization1Unlocked = true;

        if (dimension2.civilization1 == null)
            dimension2.civilization1 = new D2Civilization1State();

        if (dimension2.civilization2 == null)
            dimension2.civilization2 = new D2Civilization2State();

        if (dimension2.civilization3 == null)
            dimension2.civilization3 = new D2Civilization3State();

        D2Civilization1System.EnsureState(dimension2.civilization1);
        D2PilgrimageSystem.EnsureState(state);
        D2VeiledThresholdSystem.EnsureState(dimension2.civilization1);
        dimension2.civilization2.progressVersion = Civilization2ProgressVersion;
        dimension2.civilization3.progressVersion = Civilization3ProgressVersion;

        if (!IsTerritoryId(dimension2.selectedTerritoryId) ||
            !IsTerritoryUnlocked(dimension2, dimension2.selectedTerritoryId))
        {
            dimension2.selectedTerritoryId = Civilization1TerritoryId;
        }
    }

    public static Dimension2State CreateInitialState()
    {
        return new Dimension2State
        {
            progressVersion = ProgressVersion,
            firstEntrySeen = false,
            selectedTerritoryId = Civilization1TerritoryId,
            civilization1Unlocked = true,
            civilization2Unlocked = false,
            civilization3Unlocked = false,
            civilization1 = new D2Civilization1State(),
            civilization2 = new D2Civilization2State(),
            civilization3 = new D2Civilization3State()
        };
    }

    public static void ResetState(GameState state)
    {
        if (state == null)
            return;

        state.dimension2 = CreateInitialState();
        EnsureState(state);
    }

    public static bool CanAccessDimension2(GameState state)
    {
        return state != null && state.dimension02Unlocked;
    }

    public static bool IsTerritoryId(string territoryId)
    {
        return territoryId == Civilization1TerritoryId ||
               territoryId == Civilization2TerritoryId ||
               territoryId == Civilization3TerritoryId;
    }

    public static bool IsTerritoryUnlocked(GameState state, string territoryId)
    {
        if (!CanAccessDimension2(state))
            return false;

        EnsureState(state);
        return IsTerritoryUnlocked(state.dimension2, territoryId);
    }

    public static bool IsTerritoryUnlocked(Dimension2State state, string territoryId)
    {
        if (state == null)
            return false;

        switch (territoryId)
        {
            case Civilization1TerritoryId:
                return state.civilization1Unlocked;
            case Civilization2TerritoryId:
                return state.civilization2Unlocked;
            case Civilization3TerritoryId:
                return state.civilization3Unlocked;
            default:
                return false;
        }
    }

    public static bool TrySelectTerritory(GameState state, string territoryId)
    {
        if (!IsTerritoryUnlocked(state, territoryId))
            return false;

        state.dimension2.selectedTerritoryId = territoryId;
        return true;
    }

    public static void MarkFirstEntrySeen(GameState state)
    {
        if (!CanAccessDimension2(state))
            return;

        EnsureState(state);
        state.dimension2.firstEntrySeen = true;
    }

    public static string GetTerritoryDisplayName(string territoryId)
    {
        switch (territoryId)
        {
            case Civilization1TerritoryId:
                return "Civilización 1 — Santuario de Peregrinos";
            case Civilization2TerritoryId:
                return "Civilización 2 — Territorios Sometidos";
            case Civilization3TerritoryId:
                return "Civilización 3 — Ruinas Sepultadas";
            default:
                return "Territorio desconocido";
        }
    }

    public static void Tick(GameState state, double dt)
    {
        if (!CanAccessDimension2(state) || dt <= 0.0 || double.IsNaN(dt))
            return;

        EnsureState(state);

        D2Civilization1System.Tick(state, dt);
    }

    public static double ApplyOfflineProgress(GameState state, double offlineSeconds)
    {
        if (!CanAccessDimension2(state) || offlineSeconds <= 0.0 || double.IsNaN(offlineSeconds))
            return 0.0;

        EnsureState(state);
        double appliedSeconds = Math.Min(offlineSeconds, OfflineProgressCapSeconds);
        D2Civilization1System.ApplyOfflineProgress(state, appliedSeconds);
        return appliedSeconds;
    }

    public static bool ValidateState(GameState state, out string result)
    {
        if (state == null)
        {
            result = "GameState es null.";
            return false;
        }

        EnsureState(state);
        Dimension2State dimension2 = state.dimension2;

        if (dimension2.progressVersion != ProgressVersion)
        {
            result = "Versión raíz D2 inválida.";
            return false;
        }

        if (!dimension2.civilization1Unlocked)
        {
            result = "Civilización 1 debe estar disponible dentro de D2.";
            return false;
        }

        if (!IsTerritoryId(dimension2.selectedTerritoryId))
        {
            result = "Territorio seleccionado inválido.";
            return false;
        }

        if (!IsTerritoryUnlocked(dimension2, dimension2.selectedTerritoryId))
        {
            result = "El territorio seleccionado está bloqueado.";
            return false;
        }

        if (dimension2.civilization1 == null ||
            dimension2.civilization2 == null ||
            dimension2.civilization3 == null)
        {
            result = "Falta el estado preparado de una civilización.";
            return false;
        }

        if (!D2Civilization1System.ValidateState(dimension2.civilization1, out result))
            return false;

        if (!D2PilgrimageSystem.ValidateState(state, out result))
            return false;

        if (!D2VeiledThresholdSystem.ValidateState(dimension2.civilization1, out result))
            return false;

        result = "Estado de Dimensión 2 válido.";
        return true;
    }
}
