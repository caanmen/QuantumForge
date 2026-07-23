#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class Dimension3Block3Validation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 3 Core")]
    public static void ValidateBlock3Core()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError("[D3 Block 3] Ejecutar fuera de Play Mode.");
            return;
        }
        var failures = new List<string>();
        ValidateFiveMinigames(failures);
        ValidateAffinity(failures);
        ValidateProfiles(failures);
        ValidateTraitAssembly(failures);
        if (failures.Count == 0)
        {
            Debug.Log(
                "[D3 Block 3] PASS | cinco minijuegos | redondeos | " +
                "afinidad | empate | perfiles N1-N3 | ensamblaje con rasgo | JSON"
            );
        }
        else Debug.LogError("[D3 Block 3] FAIL\n- " + string.Join("\n- ", failures));
    }

    private static void ValidateFiveMinigames(List<string> failures)
    {
        string reason;
        int reading;
        Check(D3CalibrationSystem.TryCalculateReading(new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartChassis,
                valueA = 20, valueB = 20, valueC = 60
            }, out reading, out reason) && reading == 40,
            "Fórmula de Chasis incorrecta: " + reason, failures);
        Check(!D3CalibrationSystem.TryCalculateReading(new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartChassis,
                valueA = 20, valueB = 20, valueC = 59
            }, out reading, out reason),
            "Chasis acepta una distribución distinta de 100.", failures);
        Check(D3CalibrationSystem.TryCalculateReading(new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartMotor,
                valueA = 80, valueB = 80, valueC = 80
            }, out reading, out reason) && reading == 80,
            "Fórmula de Sistema Motriz incorrecta.", failures);
        Check(D3CalibrationSystem.TryCalculateReading(new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartTool,
                optionA = 2, valueA = 73
            }, out reading, out reason) && reading == 65,
            "Fórmula/redondeo de Herramienta incorrecto.", failures);
        Check(D3CalibrationSystem.TryCalculateReading(new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartControl,
                valueA = 0, valueB = 2, valueC = 1,
                optionA = 0, optionB = 1, optionC = 2
            }, out reading, out reason) && reading == 33,
            "Fórmula de Módulo de Control incorrecta.", failures);
        Check(!D3CalibrationSystem.TryCalculateReading(new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartControl,
                valueA = 0, valueB = 0, valueC = 1,
                optionA = 0, optionB = 1, optionC = 2
            }, out reading, out reason),
            "Control permite reutilizar un nodo.", failures);
        Check(D3CalibrationSystem.TryCalculateReading(new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartRegulator,
                valueA = 70, valueB = 30
            }, out reading, out reason) && reading == 70,
            "Fórmula de Regulador incorrecta.", failures);
    }

    private static void ValidateAffinity(List<string> failures)
    {
        var partial = new List<D3CalibrationReadingState>
        {
            new D3CalibrationReadingState
                { partId = Dimension3Catalog.PartChassis, reading = 45 }
        };
        D3CalibrationEvaluation ambiguous = D3CalibrationSystem.Evaluate(partial);
        Check(ambiguous.calibratedCount == 1 && ambiguous.ambiguous &&
              ambiguous.resultTraitId == Dimension3Catalog.TraitNormal,
            "No detecta empate provisional dentro de 0,01.", failures);

        List<D3CalibrationControlState> fastControls = CreateFastControls();
        List<D3CalibrationReadingState> readings = CalculateReadings(fastControls, failures);
        D3CalibrationEvaluation evaluation = D3CalibrationSystem.Evaluate(readings);
        Check(evaluation.calibratedCount == 5 && !evaluation.ambiguous &&
              evaluation.resultTraitId == Dimension3Catalog.TraitFast &&
              evaluation.affinity >= 90.0,
            "El patrón Rápido no produce rasgo y afinidad alta.", failures);

        var low = new List<D3CalibrationReadingState>();
        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
            low.Add(new D3CalibrationReadingState
                { partId = Dimension3Catalog.PartIds[i], reading = 0 });
        Check(D3CalibrationSystem.Evaluate(low).resultTraitId ==
              Dimension3Catalog.TraitNormal,
            "Una afinidad insuficiente concede un rasgo.", failures);
    }

    private static void ValidateProfiles(List<string> failures)
    {
        GameState state = CreateState("D3 Block 3 Profiles");
        try
        {
            string reason;
            List<D3CalibrationControlState> controls = CreateFastControls();
            Check(D3CalibrationSystem.TrySaveProfile(
                    state, "Rápido conocido", controls, out reason),
                "Banco N1 no guarda perfil descubierto: " + reason, failures);
            List<D3CalibrationControlState> loaded;
            Check(!D3CalibrationSystem.TryLoadProfile(state, out loaded, out reason),
                "Banco N1 carga un perfil antes de nivel 2.", failures);
            D3FacilitySystem.GetFacility(state.dimension3,
                Dimension3Catalog.FacilityProcessBank).level = 2;
            Check(D3CalibrationSystem.TryLoadProfile(state, out loaded, out reason) &&
                  loaded != null && loaded.Count == 5,
                "Banco N2 no carga perfil completo: " + reason, failures);
            D3FacilitySystem.GetFacility(state.dimension3,
                Dimension3Catalog.FacilityProcessBank).level = 3;
            Check(D3CalibrationSystem.CanAutoRepeatOnePiece(state),
                "Banco N3 no habilita repetición automática de una pieza.", failures);

            string json = JsonUtility.ToJson(state.dimension3);
            Dimension3State restored = JsonUtility.FromJson<Dimension3State>(json);
            Check(restored != null && restored.calibrationProfiles.Count == 1 &&
                  restored.calibrationProfiles[0].controls.Count == 5,
                "JSON pierde los controles exactos del perfil.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateTraitAssembly(List<string> failures)
    {
        GameState state = CreateState("D3 Block 3 Assembly");
        try
        {
            state.LE = 100000.0;
            state.Traces = 1000.0;
            for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
                D3InventorySystem.AddParts(state.dimension3,
                    Dimension3Catalog.PartIds[i], 1, 1L);
            List<D3CalibrationReadingState> readings = CalculateReadings(
                CreateFastControls(), failures);
            string reason;
            Check(Dimension3System.TryQueueTraitAssembly(
                    state, 1, readings, out reason),
                "No encola ensamblaje calibrado: " + reason, failures);
            D3JobState job = D3JobQueueSystem.GetQueue(
                state.dimension3, Dimension3Catalog.QueueAssembly).jobs[0];
            Check(job.resultTraitId == Dimension3Catalog.TraitFast &&
                  AreClose(job.paidLE, 2500.0) &&
                  AreClose(job.paidTraces, 7.0) &&
                  AreClose(job.baseDurationSeconds, 36.0) &&
                  job.calibrationReadings.Count == 5,
                "Costo, tiempo, lecturas o resultado fijado incorrecto.", failures);
            Dimension3System.Tick(state, 36.0);
            Check(D3InventorySystem.GetAutomatonAmount(
                    state.dimension3, 1, Dimension3Catalog.TraitFast) == 1L,
                "El trabajo calibrado no entrega MK1 Rápido.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static List<D3CalibrationControlState> CreateFastControls()
    {
        return new List<D3CalibrationControlState>
        {
            new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartChassis,
                valueA = 20, valueB = 20, valueC = 60
            },
            new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartMotor,
                valueA = 80, valueB = 80, valueC = 80
            },
            new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartTool,
                optionA = 2, valueA = 73
            },
            new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartControl,
                valueA = 0, valueB = 2, valueC = 1,
                optionA = 0, optionB = 1, optionC = 2
            },
            new D3CalibrationControlState
            {
                partId = Dimension3Catalog.PartRegulator,
                valueA = 70, valueB = 30
            }
        };
    }

    private static List<D3CalibrationReadingState> CalculateReadings(
        List<D3CalibrationControlState> controls, List<string> failures)
    {
        var readings = new List<D3CalibrationReadingState>();
        for (int i = 0; i < controls.Count; i++)
        {
            int reading;
            string reason;
            if (!D3CalibrationSystem.TryCalculateReading(
                    controls[i], out reading, out reason))
            {
                failures.Add("No calcula control de prueba: " + reason);
                continue;
            }
            readings.Add(new D3CalibrationReadingState
                { partId = controls[i].partId, reading = reading });
        }
        return readings;
    }

    private static GameState CreateState(string name)
    {
        var gameObject = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        gameObject.SetActive(false);
        GameState state = gameObject.AddComponent<GameState>();
        state.dimension03Unlocked = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        Dimension3System.EnsureState(state);
        return state;
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.0001;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif
