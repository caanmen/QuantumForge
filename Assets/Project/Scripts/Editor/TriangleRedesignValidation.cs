#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TriangleRedesignValidation
{
    [MenuItem("Tools/Quantum Forge/Game Base/Validate Triangle Redesign")]
    public static void Validate()
    {
        var failures = new List<string>();
        ValidateCircuits(failures);
        ValidateLegacyMigration(failures);
        ValidateOfflineProduction(failures);
        ValidateD3SettingsMigration(failures);
        ValidateBaseProgressionGate(failures);
        ValidateMachinePrestigeGate(failures);
        ValidateSecondPrestigePreservesDimensions(failures);
        ValidateAutonomyCoreMilestone(failures);
        ValidateFinalPrestigeCycleClosure(failures);

        if (failures.Count == 0)
            Debug.Log("[Triangle Redesign] PASS | circuitos | sacrificios | " +
                "migración | sincronización | offline | Consola N3");
        else
            Debug.LogError("[Triangle Redesign] FAIL\n- " +
                string.Join("\n- ", failures));
    }

    private static void ValidateCircuits(List<string> failures)
    {
        GameState state = CreateState("Triangle Circuits");
        try
        {
            state.triangleActiveCircuit = TriangleCircuitType.Energy;
            state.triangleSynchronization = 1f;
            Check(Near(state.GetTriangleLEMultiplier(), 1.12),
                "Energía no otorga +12% LE.", failures);
            Check(Near(state.GetTriangleTracesMultiplier(), 0.90),
                "Energía no aplica -10% Trazas.", failures);

            state.triangleActiveCircuit = TriangleCircuitType.Experimental;
            Check(Near(state.GetTriangleLEMultiplier(), 0.90),
                "Experimental no aplica -10% LE.", failures);
            Check(Near(state.GetTriangleTracesMultiplier(), 1.10),
                "Experimental no otorga +10% Trazas.", failures);
            Check(Near(state.GetTriangleFragmentMultiplier(), 1.06),
                "Experimental no otorga +6% fragmentos.", failures);

            state.triangleActiveCircuit = TriangleCircuitType.Energy;
            state.triangleSynchronization = 1f;
            state.SetTriangleCircuit(TriangleCircuitType.Experimental, false);
            Check(Near(state.triangleSynchronization,
                    GameState.TriangleSwitchSynchronization),
                "El cambio no comienza en 75% de sincronización.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateLegacyMigration(List<string> failures)
    {
        GameState state = CreateState("Triangle Migration");
        try
        {
            state.triangleActiveCircuit = TriangleCircuitType.None;
            state.trianglePrimaryBuildingId = "fluctuation_antenna";
            state.triangleReinforcementBuildingId = "vacuum_observer";
            state.triangleAlterationBuildingId = "casimir_panel";
            state.phaseModulatorMode = PhaseModulatorMode.Conservation;
            state.phaseModulatorCalibration = 0.4f;
            state.SanitizeTriangleCircuit(true);
            Check(state.triangleActiveCircuit == TriangleCircuitType.Energy,
                "La permutación antigua no tiene prioridad en la migración.", failures);
            Check(state.triangleSynchronization >= 0.75f,
                "La migración redujo la sincronización por debajo del mínimo.", failures);
            Check(state.trianglePersistenceReserveSeconds == 0.0,
                "La reserva antigua de Persistencia no fue retirada.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateOfflineProduction(List<string> failures)
    {
        GameState state = CreateState("Triangle Offline");
        try
        {
            state.triangleActiveCircuit = TriangleCircuitType.Experimental;
            state.triangleSynchronization = 1f;
            double leBefore = state.LE;
            double tracesBefore = state.Traces;
            TriangleOfflineReport report = state.ApplyOfflineBaseProgress(60.0);
            Check(report.appliedSeconds == 60.0 && report.hasResults,
                "No se creó el informe offline.", failures);
            Check(state.LE > leBefore && state.Traces > tracesBefore,
                "El juego base no produjo LE/Trazas offline.", failures);
            Check(report.condensationGained >= 2 &&
                  report.confinementGained >= 2 &&
                  report.residualInterferenceGained >= 2,
                "Experimental no produjo fragmentos offline.", failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateD3SettingsMigration(List<string> failures)
    {
        var settings = new D3ConsoleSettingsState
        {
            triangleCircuitVersion = 0,
            preferredModulatorMode = (int)PhaseModulatorMode.Conservation
        };
        settings.manuallySelectedModulatorModes.Add(
            (int)PhaseModulatorMode.Conservation);
        D3ConsoleSystem.NormalizeSettings(settings);
        Check(settings.triangleCircuitVersion == 1 &&
              settings.preferredTriangleCircuit ==
                (int)TriangleCircuitType.Experimental &&
              settings.manuallySelectedTriangleCircuits.Contains(
                (int)TriangleCircuitType.Experimental),
            "La Consola N3 no migró la fase antigua a circuito.", failures);
    }

    private static void ValidateBaseProgressionGate(List<string> failures)
    {
        GameState state = CreateState("Base Progression Gate");
        GameObject machineObject = new GameObject("Progression Machine")
            { hideFlags = HideFlags.HideAndDontSave };
        machineObject.SetActive(false);
        MachineManager machine = machineObject.AddComponent<MachineManager>();

        try
        {
            state.experimentalChamberUnlocked = false;
            state.experimentalChamberInitialPackGranted = false;
            state.LE = state.experimentalChamberKeycardLeCost;
            state.Traces = state.experimentalChamberKeycardTraceCost;
            state.triangleActiveCircuit = TriangleCircuitType.None;
            state.triangleSynchronization = 1f;

            Check(state.HasExperimentalChamberArtifactRequirements(),
                "Los tres artefactos base no satisfacen su requisito.", failures);
            Check(!state.HasExperimentalChamberTriangleRequirement(),
                "La keycard acepta un Triángulo sin circuito activo.", failures);
            Check(!state.TryBuyExperimentalChamberKeycard(),
                "El Cuarto 2 puede saltarse el Triángulo.", failures);
            Check(!state.TryUnlockMachineFromExperimentalChamber(machine),
                "La Máquina se desbloquea antes que el Cuarto 2.", failures);

            state.triangleActiveCircuit = TriangleCircuitType.Energy;
            Check(state.HasExperimentalChamberTriangleRequirement(),
                "Un circuito activo no habilita el requisito del Triángulo.", failures);
            Check(state.TryBuyExperimentalChamberKeycard(),
                "La keycard no se puede comprar con todos sus requisitos.", failures);
            Check(state.experimentalChamberUnlocked,
                "La compra no abrió el Cuarto 2.", failures);
            Check(state.TryUnlockMachineFromExperimentalChamber(machine) &&
                  machine.MachineUnlocked,
                "El Cuarto 2 no conduce al desbloqueo normal de la Máquina.", failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(machineObject);
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateMachinePrestigeGate(List<string> failures)
    {
        GameState state = CreateState("Machine Prestige Gate");
        GameObject machineObject = new GameObject("Prestige Machine")
            { hideFlags = HideFlags.HideAndDontSave };
        MachineManager machine = machineObject.AddComponent<MachineManager>();

        try
        {
            state.hasDonePrestige1 = false;
            state.maxLEAlcanzado = 1500000.0;

            List<MachineNodeDef> visibleNodes = machine.GetAllNodes(false);
            int requiredRepairCount = Mathf.CeilToInt(visibleNodes.Count * 0.80f);
            var repairedIds = new List<string>();

            for (int i = 0; i < visibleNodes.Count &&
                 repairedIds.Count < requiredRepairCount; i++)
            {
                MachineNodeDef node = visibleNodes[i];
                if (node == null || node.id == "z3_convergence_channel")
                    continue;
                repairedIds.Add(node.id);
            }

            var save = new SaveData
            {
                machineIntroSeen = true,
                machineUnlocked = false,
                machineAllZonesUnlocked = true,
                machineRepairedNodeIds = new List<string>(repairedIds)
            };

            machine.LoadProgressFromSave(save);
            Check(machine.HasEnoughRepairForPrestige1(),
                "El 80% de reparación visible no se reconoce.", failures);
            Check(!state.CanDoPrestige1(machine),
                "Prestigio 1 se habilita con la Máquina bloqueada.", failures);
            Check(!TabsUI.ShouldShowPrestige1Button(state, machine),
                "La pestaña Prestigio aparece antes de descubrir la Máquina.", failures);

            save.machineUnlocked = true;
            machine.LoadProgressFromSave(save);
            Check(TabsUI.ShouldShowPrestige1Button(state, machine),
                "La pestaña Prestigio no aparece al desbloquear la Máquina.", failures);
            Check(!state.CanDoPrestige1(machine),
                "El 80% permite Prestigio 1 sin Canal de Convergencia.", failures);

            repairedIds.Add("z3_convergence_channel");
            save.machineRepairedNodeIds = new List<string>(repairedIds);
            machine.LoadProgressFromSave(save);
            Check(state.CanDoPrestige1(machine),
                "Máquina al 80% con Convergencia no habilita Prestigio 1.", failures);
            state.hasDonePrestige1 = true;
            state.prestige1Count = 1;
            Check(!TabsUI.ShouldShowPrestige1Button(state, machine),
                "La pestaña Prestigio aparece sin un hito dimensional posterior.", failures);

            state.dimension01Unlocked = true;
            state.dimension1GalacticAnchorDiscovered = true;
            state.prestige1CurrentDimensionId = 2;
            Check(!state.HasDimensionMilestoneForNextPrestige1(),
                "Un hito de otra dimensión habilita un Prestigio que no le corresponde.",
                failures);
            state.prestige1CurrentDimensionId = 1;
            Check(state.HasDimensionMilestoneForNextPrestige1(),
                "El hito de la dimensión elegida no habilita el siguiente Prestigio.",
                failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(machineObject);
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateAutonomyCoreMilestone(List<string> failures)
    {
        GameState state = CreateState("D3 Autonomy Milestone");
        try
        {
            state.dimension03Unlocked = true;
            Dimension3System.EnsureState(state);
            D3FacilityState core = D3FacilitySystem.GetFacility(
                state.dimension3, Dimension3Catalog.FacilityAutomationCore);
            core.built = true;
            core.level = D3AutonomyCoreSystem.RequiredCoreLevel;

            Check(!D3AutonomyCoreSystem.CanIntegrate(state, out _),
                "El Núcleo de Autonomía no exige un MK6 Coordinador estable.", failures);

            state.dimension3.assignments.Add(new D3AssignmentState
            {
                installationId = Dimension3Catalog.FacilityAutomationCore,
                channelId = Dimension3Catalog.ChannelCoreCoordination,
                mk = D3AutonomyCoreSystem.RequiredMk,
                traitId = Dimension3Catalog.TraitCoordinator,
                amount = 1L,
                stabilizedAmount = 1L
            });
            Check(D3AutonomyCoreSystem.TryIntegrate(state, out _),
                "No se puede integrar el Núcleo de Autonomía con sus requisitos.",
                failures);
            state.prestige1CurrentDimensionId = 3;
            Check(state.dimension3.autonomyCoreIntegrated &&
                  state.HasDimensionMilestoneForNextPrestige1(),
                "El Núcleo de Autonomía no habilita el siguiente Prestigio 1.",
                failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }

    private static void ValidateSecondPrestigePreservesDimensions(
        List<string> failures)
    {
        GameState state = CreateState("Second Prestige Preservation");
        GameObject machineObject = new GameObject("Second Prestige Machine")
            { hideFlags = HideFlags.HideAndDontSave };
        machineObject.SetActive(false);
        MachineManager machine = machineObject.AddComponent<MachineManager>();

        try
        {
            state.prestige1Count = 1;
            state.hasDonePrestige1 = true;
            state.prestige1CurrentDimensionId = 1;
            state.dimension01Unlocked = true;
            state.dimension1GalacticAnchorDiscovered = true;
            state.dimension1Metals.Add(new D1MetalAmount
            {
                metalId = Dimension1System.MetalIron,
                amount = 37.0
            });
            state.LE = 1500000.0;
            state.Traces = 999.0;
            state.fragmentCondensation = 9;

            List<MachineNodeDef> visibleNodes = machine.GetAllNodes(false);
            int requiredRepairCount = Mathf.CeilToInt(visibleNodes.Count * 0.80f);
            var repairedIds = new List<string>();
            for (int i = 0; i < visibleNodes.Count &&
                 repairedIds.Count < requiredRepairCount; i++)
            {
                MachineNodeDef node = visibleNodes[i];
                if (node != null && node.id != "z3_convergence_channel")
                    repairedIds.Add(node.id);
            }
            repairedIds.Add("z3_convergence_channel");
            machine.LoadProgressFromSave(new SaveData
            {
                machineIntroSeen = true,
                machineUnlocked = true,
                machineAllZonesUnlocked = true,
                machineRepairedNodeIds = repairedIds
            });

            Check(state.DoPrestige1Reset(2, machine),
                "El segundo Prestigio válido no se pudo ejecutar.", failures);
            Check(state.prestige1Count == 2 && state.prestige1CurrentDimensionId == 2 &&
                  state.dimension01Unlocked && state.dimension02Unlocked &&
                  !state.dimension03Unlocked,
                "El segundo Prestigio no revela solo la nueva dimensión.", failures);
            Check(state.dimension1GalacticAnchorDiscovered &&
                  Near(state.GetD1MetalAmount(Dimension1System.MetalIron), 37.0),
                "El segundo Prestigio borró progreso de Dimensión 1.", failures);
            Check(Near(state.LE, 10.0) && Near(state.Traces, 0.0) &&
                  state.fragmentCondensation == 0 && !machine.MachineUnlocked,
                "El segundo Prestigio no reinició correctamente el juego base.",
                failures);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(machineObject);
            UnityEngine.Object.DestroyImmediate(state.gameObject);
        }
    }

    private static void ValidateFinalPrestigeCycleClosure(List<string> failures)
    {
        GameState state = CreateState("Final Prestige Cycle");
        try
        {
            state.dimension01Unlocked = true;
            state.dimension02Unlocked = true;
            state.dimension03Unlocked = true;
            state.dimension1GalacticAnchorDiscovered = true;
            state.dimension2 = new Dimension2State();
            Dimension2System.EnsureState(state);
            state.dimension2.civilization2.majorPactEstablished = true;
            state.dimension3 = Dimension3System.CreateInitialState();
            state.dimension3.autonomyCoreIntegrated = true;
            Check(state.IsPrestige1CycleComplete() &&
                  !state.HasAvailableDimensionForPrestige1Selection(),
                "El ciclo P1 no se reconoce como cerrado tras los tres hitos.",
                failures);
        }
        finally { UnityEngine.Object.DestroyImmediate(state.gameObject); }
    }


    private static GameState CreateState(string name)
    {
        GameObject go = new GameObject(name) { hideFlags = HideFlags.HideAndDontSave };
        go.SetActive(false);
        GameState state = go.AddComponent<GameState>();
        state.LE = 10.0;
        state.Traces = 0.0;
        state.triangleSystemUnlocked = true;
        state.experimentalChamberUnlocked = true;
        AddBuilding(state, "vacuum_observer", 1.0, 1.0, 1);
        AddBuilding(state, "casimir_panel", 1.0, 1.0, 1);
        AddBuilding(state, "fluctuation_antenna", 0.0, 1.0, 1);
        return state;
    }

    private static void AddBuilding(
        GameState state, string id, double lePerTick, double interval, int level)
    {
        var definition = new BuildingDef
        {
            id = id,
            baseCost = 10.0,
            costMult = 2.0,
            lePerTickBase = lePerTick,
            tickInterval = interval
        };
        var building = new BuildingState { level = level };
        building.InitFromDef(definition);
        state.RegisterBuildingState(building);
    }

    private static bool Near(double a, double b)
    {
        return Math.Abs(a - b) < 0.000001;
    }

    private static void Check(
        bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }
}
#endif
