#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Dimension3Block4Validation
{
    [MenuItem("Tools/Quantum Forge/Dimension 3/Validate Block 4 Core")]
    public static void ValidateBlock4Core()
    {
        var failures = new List<string>();
        ValidateResearchReservations(failures);
        ValidateHigherMkSupports(failures);
        if (failures.Count == 0)
            Debug.Log("[D3 Block 4] PASS | V4-V6 | prerrequisitos | investigadores | cancelación | MK5-MK6 apoyos");
        else Debug.LogError("[D3 Block 4] FAIL\n- " + string.Join("\n- ", failures));
    }

    private static void ValidateResearchReservations(List<string> failures)
    {
        GameState state = CreateState("D3 B4 Research");
        try
        {
            state.LE = 10000000; state.Traces = 10000;
            D3FacilitySystem.GetFacility(state.dimension3,
                Dimension3Catalog.FacilityProcessBank).level = 4;
            D3InventorySystem.AddAssemblyCount(state.dimension3, 3, 10);
            D3InventorySystem.AddAutomatons(state.dimension3, 3,
                Dimension3Catalog.TraitNormal, 4);
            var team = new List<D3ReservedAutomatonState>
            {
                new D3ReservedAutomatonState
                    { mk = 3, traitId = Dimension3Catalog.TraitNormal, amount = 4 }
            };
            string reason;
            Check(Dimension3System.TryQueueResearch(state,
                    Dimension3Catalog.PartChassis, 4, team, out reason),
                "No encola V4 válida: " + reason, failures);
            Check(D3InventorySystem.GetAvailableAutomatonAmount(state.dimension3, 3,
                    Dimension3Catalog.TraitNormal) == 0,
                "Investigadores reservados continúan libres.", failures);
            D3JobState job = D3JobQueueSystem.GetQueue(state.dimension3,
                Dimension3Catalog.QueueResearch).jobs[0];
            Check(Dimension3System.TryCancelJob(state,
                    Dimension3Catalog.QueueResearch, job.jobId, out reason),
                "No cancela investigación: " + reason, failures);
            Check(D3InventorySystem.GetAvailableAutomatonAmount(state.dimension3, 3,
                    Dimension3Catalog.TraitNormal) == 4,
                "Cancelar no libera investigadores.", failures);
            Check(Dimension3System.TryQueueResearch(state,
                    Dimension3Catalog.PartChassis, 4, team, out reason),
                "No reencola V4: " + reason, failures);
            Dimension3System.Tick(state, 100000);
            Check(D3ResearchSystem.IsCompleted(state.dimension3,
                    Dimension3Catalog.PartChassis, 4),
                "V4 no se completa.", failures);
            Check(D3ProductionSystem.IsPartVersionUnlocked(state,
                    Dimension3Catalog.PartChassis, 4),
                "La investigación no desbloquea su pieza V4.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateHigherMkSupports(List<string> failures)
    {
        GameState state = CreateState("D3 B4 Supports");
        try
        {
            state.LE = 10000000; state.Traces = 10000;
            for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
            {
                AddResearch(state.dimension3, Dimension3Catalog.PartIds[i], 5);
                D3InventorySystem.AddParts(state.dimension3,
                    Dimension3Catalog.PartIds[i], 5, 1);
            }
            D3InventorySystem.AddAutomatons(state.dimension3, 4,
                Dimension3Catalog.TraitNormal, 5);
            string reason;
            Check(Dimension3System.TryQueueNormalAssembly(state, 5, 1, out reason),
                "No encola MK5 con cinco apoyos MK4: " + reason, failures);
            Check(D3InventorySystem.GetAvailableAutomatonAmount(state.dimension3, 4,
                    Dimension3Catalog.TraitNormal) == 0,
                "Apoyos MK4 no quedan reservados.", failures);
            Dimension3System.Tick(state, 100000);
            Check(D3InventorySystem.GetAutomatonAmount(state.dimension3, 4,
                    Dimension3Catalog.TraitNormal) == 5,
                "Ensamblar MK5 consume los apoyos MK4.", failures);
            Check(D3InventorySystem.GetAutomatonAmount(state.dimension3, 5,
                    Dimension3Catalog.TraitNormal) == 1,
                "Ensamblaje no entrega MK5.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void AddResearch(Dimension3State state, string partId, int version)
    {
        state.research.Add(new D3ResearchState
        {
            researchId = Dimension3Catalog.GetResearchId(partId, version), completed = true
        });
    }
    private static GameState CreateState(string name)
    {
        var go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.dimension03Unlocked = true;
        state.dimension3 = Dimension3System.CreateInitialState();
        Dimension3System.EnsureState(state);
        return state;
    }
    private static void Check(bool condition, string message, List<string> failures)
    { if (!condition) failures.Add(message); }
}
#endif
