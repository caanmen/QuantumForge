#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class D1TreePointsValidation
{
    [MenuItem("Tools/Quantum Forge/Dimension 1/Validate Tree Points")]
    public static void ValidateD1TreePoints()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[D1 Tree Points] Ejecutar fuera de Play Mode.");
            return;
        }

        var failures = new List<string>();
        ValidateExternalProgressIsIgnored(failures);
        ValidateAutomaticD1Credit(failures);
        ValidateSaveCompatibility(failures);

        if (failures.Count == 0)
        {
            Debug.Log(
                "[D1 Tree Points] PASS | aislamiento D1 | acreditación única | saves"
            );
            return;
        }

        Debug.LogError("[D1 Tree Points] FAIL\n- " +
            string.Join("\n- ", failures));
    }

    public static void ValidateD1TreePointsBatch()
    {
        ValidateD1TreePoints();
    }

    private static void ValidateExternalProgressIsIgnored(List<string> failures)
    {
        GameState state = CreateState("D1 Tree External Isolation");
        try
        {
            state.LE = 1000000000.0;
            state.maxLEAlcanzado = 1000000000.0;
            state.dimension02Unlocked = true;
            Dimension2System.EnsureState(state);
            state.dimension2.civilization3Unlocked = true;
            state.dimension2.civilization3.entityPactEstablished = true;

            Check(Dimension1System.CalculateD1TreePointsFromProgress(state) == 0,
                "El juego base o D2 aportan puntos sin progreso D1.", failures);
            Check(!Dimension1System.SyncD1TreePointsFromProgress(state, out _),
                "El juego base o D2 acreditan puntos del Árbol D1.", failures);
            Check(state.d1TreePoints == 0,
                "El saldo D1 cambió por una fuente externa.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateAutomaticD1Credit(List<string> failures)
    {
        GameState state = CreateState("D1 Tree Automatic Credit");
        try
        {
            state.dimension01Unlocked = true;
            state.EnsureDimension1State();

            D1PlanetState planet = FindPlanet(state, Dimension1System.Planet01);
            D1ShipState ship = FindShip(state, Dimension1System.ShipLightProbe);
            Check(planet != null && ship != null,
                "D1 no inicializa el planeta o la nave inicial.", failures);
            if (planet == null || ship == null)
                return;

            planet.unlocked = true;
            ship.unlocked = true;

            int expectedProgress =
                Dimension1System.CalculateD1TreePointsFromProgress(state);
            Check(expectedProgress > 0,
                "El progreso mínimo de D1 no genera puntos del Árbol.", failures);
            Check(Dimension1System.SyncD1TreePointsFromProgress(
                    state, out int credited) && credited == expectedProgress,
                "El primer progreso D1 no se acredita completo una sola vez.", failures);
            Check(state.d1TreePoints == expectedProgress &&
                  state.d1TreePointsProgressBaseline == expectedProgress,
                "Saldo o baseline D1 incorrecto después de acreditar.", failures);
            Check(!Dimension1System.SyncD1TreePointsFromProgress(state, out _),
                "El mismo progreso D1 se acredita dos veces.", failures);

            state.LE = 1000000000.0;
            state.maxLEAlcanzado = 1000000000.0;
            state.dimension02Unlocked = true;
            Dimension2System.EnsureState(state);
            state.dimension2.civilization3Unlocked = true;
            state.dimension2.civilization3.entityPactEstablished = true;

            Check(!Dimension1System.SyncD1TreePointsFromProgress(state, out _),
                "Un cambio externo acredita puntos D1.", failures);
            Check(state.d1TreePoints == expectedProgress,
                "Un cambio externo modificó el saldo D1.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateSaveCompatibility(List<string> failures)
    {
        SaveData legacy = JsonUtility.FromJson<SaveData>(
            "{\"prestige1Points\":7,\"prestige1BestClaimedPreviewPoints\":9}"
        );
        Check(legacy != null && legacy.d1TreePointsSaveVersion == 0 &&
              legacy.prestige1Points == 7 &&
              legacy.prestige1BestClaimedPreviewPoints == 9,
            "El JSON antiguo no conserva la moneda anterior para migración.",
            failures);

        var current = new SaveData
        {
            d1TreePointsSaveVersion = 1,
            d1TreePoints = 11,
            d1TreePointsProgressBaseline = 13
        };
        SaveData roundTrip = JsonUtility.FromJson<SaveData>(
            JsonUtility.ToJson(current)
        );
        Check(roundTrip != null && roundTrip.d1TreePointsSaveVersion == 1 &&
              roundTrip.d1TreePoints == 11 &&
              roundTrip.d1TreePointsProgressBaseline == 13,
            "El save actual no conserva saldo y baseline del Árbol D1.",
            failures);
    }

    private static GameState CreateState(string name)
    {
        var gameObject = new GameObject(name)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        gameObject.SetActive(false);
        return gameObject.AddComponent<GameState>();
    }

    private static D1PlanetState FindPlanet(GameState state, string planetId)
    {
        foreach (D1PlanetState planet in state.dimension1Planets)
        {
            if (planet != null && planet.planetId == planetId)
                return planet;
        }
        return null;
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

    private static void Check(
        bool condition,
        string message,
        List<string> failures)
    {
        if (!condition)
            failures.Add(message);
    }
}
#endif
