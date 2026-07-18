using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Dimension1FinalValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 1/Validate Final Save Compatibility")]
    public static void ValidateFinalSaveCompatibility()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError(
                "[D1 Final Save Compatibility] Ejecutar fuera de Play Mode."
            );
            return;
        }

        var failures = new List<string>();

        ValidateFreshState(failures);
        ValidateLegacyState(failures);
        ValidateSaveDataJsonCompatibility(failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[D1 Final Save Compatibility] PASS | " +
                "partida nueva | partida antigua | JSON actual"
            );
            return;
        }

        Debug.LogError(
            "[D1 Final Save Compatibility] FAIL\n- " +
            string.Join("\n- ", failures)
        );
    }

    public static void ValidateFinalSaveCompatibilityBatch()
    {
        ValidateFinalSaveCompatibility();
    }

    private static void ValidateFreshState(List<string> failures)
    {
        GameState state = CreateTestState("D1 Fresh State Validation");

        try
        {
            state.EnsureDimension1State();

            Check(state.dimension1Metals.Count == 10,
                "Partida nueva: no inicializa 10 metales.", failures);
            Check(state.dimension1Planets.Count == 7,
                "Partida nueva: no inicializa 7 planetas.", failures);
            Check(state.dimension1Sectors.Count == 5,
                "Partida nueva: no inicializa 5 sectores.", failures);
            Check(state.dimension1Ships.Count == 6,
                "Partida nueva: no conserva los 6 estados de nave.", failures);
            Check(state.dimension1Relics.Count == 20,
                "Partida nueva: no inicializa 20 reliquias.", failures);
            Check(state.dimension1TreeNodes.Count == 10,
                "Partida nueva: no inicializa 10 nodos.", failures);
            Check(state.dimension1CentralSyncMissions.Count == 4,
                "Partida nueva: no inicializa 4 sincronias.", failures);
            Check(!state.dimension1CentralAccessKeyObtained &&
                  !state.dimension1GalacticAnchorDiscovered,
                "Partida nueva: inicia con progreso de Ark concedido.", failures);
            ValidateCurrentVersions(state, "Partida nueva", failures);
        }
        finally
        {
            Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateLegacyState(List<string> failures)
    {
        GameState state = CreateTestState("D1 Legacy State Validation");

        try
        {
            state.dimension1Metals = null;
            state.dimension1Planets = null;
            state.dimension1Sectors = null;
            state.dimension1ScannedDestinations = null;
            state.dimension1Relics = null;
            state.dimension1RelicPityStates = null;
            state.dimension1TreeNodes = null;
            state.dimension1CentralSyncMissions = null;
            state.dimension1ArkFinalMissionShipIds = null;
            state.dimension1ScannerLevel = 0;
            state.dimension1CompletedCoordinatedMissions = -4;
            state.dimension1ArkFinalMissionRemainingSeconds = -20.0;
            state.dimension1ArkFinalMissionTotalSeconds = -10.0;
            state.dimension1ScannerProgressVersion = 0;
            state.dimension1RelicProgressVersion = 0;
            state.dimension1TreeProgressVersion = 0;
            state.dimension1CoordinatedMissionProgressVersion = 0;
            state.dimension1ArkProgressVersion = 0;
            state.dimension1Ships = new List<D1ShipState>
            {
                new D1ShipState
                {
                    shipId = Dimension1System.LegacyCargoShipId,
                    unlocked = true,
                    cargoLevel = 3,
                    speedLevel = 2
                },
                new D1ShipState
                {
                    shipId = Dimension1System.ShipRescueShip,
                    unlocked = true,
                    armorLevel = 4
                },
                new D1ShipState
                {
                    shipId = Dimension1System.ShipConvergenceShip,
                    unlocked = true,
                    sensorsLevel = 3
                }
            };

            state.EnsureDimension1State();

            D1ShipState cargo = FindShip(state, Dimension1System.ShipCargoShip);
            D1ShipState rescue = FindShip(state, Dimension1System.ShipRescueShip);
            D1ShipState convergence = FindShip(
                state,
                Dimension1System.ShipConvergenceShip
            );

            Check(cargo != null && cargo.unlocked &&
                  cargo.cargoLevel == 3 && cargo.speedLevel == 2,
                "Partida antigua: no migra la Nave de Carga historica.", failures);
            Check(rescue != null && rescue.unlocked && rescue.armorLevel == 4,
                "Partida antigua: borra datos de la Nave de Rescate.", failures);
            Check(convergence != null && convergence.unlocked &&
                  convergence.sensorsLevel == 3,
                "Partida antigua: borra datos de la Nave de Convergencia.", failures);
            Check(!Dimension1System.IsShipActiveInDimension1Base(
                    Dimension1System.ShipRescueShip) &&
                  !Dimension1System.IsShipActiveInDimension1Base(
                    Dimension1System.ShipConvergenceShip),
                "Partida antigua: activa naves futuras en Parte 1.", failures);
            Check(state.dimension1CompletedCoordinatedMissions == 0,
                "Partida antigua: conserva un contador coordinado invalido.", failures);
            Check(state.dimension1ScannerLevel ==
                  Dimension1System.SimpleScannerMinLevel,
                "Partida antigua: no corrige el nivel del escaner.", failures);
            Check(state.dimension1CentralSyncMissions.Count == 4,
                "Partida antigua: no crea las sincronias de Ark.", failures);
            Check(state.dimension1ArkFinalMissionRemainingSeconds == 0.0 &&
                  state.dimension1ArkFinalMissionTotalSeconds == 0.0,
                "Partida antigua: no corrige tiempos negativos de Ark.", failures);
            ValidateCurrentVersions(state, "Partida antigua", failures);
        }
        finally
        {
            Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateSaveDataJsonCompatibility(List<string> failures)
    {
        const string legacyJson =
            "{\"LE\":100,\"maxLEAlcanzado\":100," +
            "\"dimension01Unlocked\":true," +
            "\"dimension1Ships\":[{\"shipId\":\"ship_rescue_ship\"," +
            "\"unlocked\":true,\"armorLevel\":4}]}";

        SaveData legacy = JsonUtility.FromJson<SaveData>(legacyJson);

        Check(legacy != null && legacy.dimension01Unlocked,
            "JSON antiguo: no se puede deserializar.", failures);
        Check(legacy != null && legacy.dimension1Ships != null &&
              legacy.dimension1Ships.Count == 1 &&
              legacy.dimension1Ships[0].armorLevel == 4,
            "JSON antiguo: no conserva datos historicos de naves.", failures);

        var current = new SaveData
        {
            LE = 100.0,
            dimension1ArkProgressVersion =
                Dimension1System.Dimension1ArkProgressVersion,
            dimension1CentralAccessKeyObtained = true,
            dimension1GalacticAnchorDiscovered = true,
            dimension1CentralSyncMissions = new List<D1CentralSyncMissionState>
            {
                new D1CentralSyncMissionState
                {
                    missionId = Dimension1System.D1CentralSyncMissionIds[0],
                    completed = true
                }
            }
        };

        SaveData roundTrip = JsonUtility.FromJson<SaveData>(
            JsonUtility.ToJson(current)
        );

        Check(roundTrip != null &&
              roundTrip.dimension1CentralAccessKeyObtained &&
              roundTrip.dimension1GalacticAnchorDiscovered &&
              roundTrip.dimension1CentralSyncMissions != null &&
              roundTrip.dimension1CentralSyncMissions.Count == 1,
            "JSON actual: no conserva Clave, sincronias y Ancla.", failures);
    }

    private static GameState CreateTestState(string name)
    {
        var gameObject = new GameObject(name)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        gameObject.SetActive(false);
        return gameObject.AddComponent<GameState>();
    }

    private static D1ShipState FindShip(GameState state, string shipId)
    {
        foreach (D1ShipState ship in state.dimension1Ships)
        {
            if (ship != null && ship.shipId == shipId)
                return ship;
        }

        return null;
    }

    private static void ValidateCurrentVersions(
        GameState state,
        string label,
        List<string> failures
    )
    {
        Check(state.dimension1ScannerProgressVersion ==
              Dimension1System.SimpleScannerProgressVersion,
            label + ": migracion del escaner desactualizada.", failures);
        Check(state.dimension1RelicProgressVersion ==
              Dimension1System.Dimension1RelicProgressVersion,
            label + ": migracion de reliquias desactualizada.", failures);
        Check(state.dimension1TreeProgressVersion ==
              Dimension1System.Dimension1TreeProgressVersion,
            label + ": migracion del arbol desactualizada.", failures);
        Check(state.dimension1CoordinatedMissionProgressVersion ==
              Dimension1System.Dimension1CoordinatedMissionProgressVersion,
            label + ": migracion coordinada desactualizada.", failures);
        Check(state.dimension1ArkProgressVersion ==
              Dimension1System.Dimension1ArkProgressVersion,
            label + ": migracion de Ark desactualizada.", failures);
    }

    private static void Check(
        bool condition,
        string failure,
        List<string> failures
    )
    {
        if (!condition)
            failures.Add(failure);
    }
}
