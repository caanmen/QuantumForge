using System;


public static class D3FacilitySystem
{
    public static D3FacilityState GetFacility(Dimension3State state, string facilityId)
    {
        if (state == null || state.facilities == null) return null;
        for (int i = 0; i < state.facilities.Count; i++)
        {
            D3FacilityState facility = state.facilities[i];
            if (facility != null && facility.facilityId == facilityId) return facility;
        }
        return null;
    }

    public static int GetProcessBankLevel(Dimension3State state)
    {
        D3FacilityState facility = GetFacility(state, Dimension3Catalog.FacilityProcessBank);
        return facility == null || !facility.built ? 0 : Math.Max(0, facility.level);
    }

    public static int GetFacilityLevel(Dimension3State state, string facilityId)
    {
        D3FacilityState facility = GetFacility(state, facilityId);
        return facility == null || !facility.built ? 0 : Math.Max(0, facility.level);
    }

    public static double GetRequiredEffectiveCapacity(int level)
    {
        switch (level)
        {
            case 1: return 2.0;
            case 2: return 5.0;
            case 3: return 10.0;
            case 4: return 20.0;
            case 5: return 35.0;
            default: return double.PositiveInfinity;
        }
    }

    public static string GetCapacityChannel(string facilityId)
    {
        if (facilityId == Dimension3Catalog.FacilityProductionConsole)
            return Dimension3Catalog.ChannelConsoleCapacity;
        if (facilityId == Dimension3Catalog.FacilityDiagnosticBank)
            return Dimension3Catalog.ChannelDiagnosticCapacity;
        if (facilityId == Dimension3Catalog.FacilityExpeditionPort)
            return Dimension3Catalog.ChannelPortCapacity;
        if (facilityId == Dimension3Catalog.FacilityAutomationCore)
            return Dimension3Catalog.ChannelCoreCoordination;
        return "";
    }

    public static string GetCoordinationChannel(string facilityId)
    {
        if (facilityId == Dimension3Catalog.FacilityProductionConsole)
            return Dimension3Catalog.ChannelConsoleCoordination;
        if (facilityId == Dimension3Catalog.FacilityDiagnosticBank)
            return Dimension3Catalog.ChannelDiagnosticCoordination;
        if (facilityId == Dimension3Catalog.FacilityExpeditionPort)
            return Dimension3Catalog.ChannelPortCoordination;
        return "";
    }

    public static double GetEffectiveCapacity(Dimension3State state, string facilityId)
    {
        if (facilityId == Dimension3Catalog.FacilityAutomationCore)
        {
            double total = 0.0;
            if (state != null && state.assignments != null)
                for (int i = 0; i < state.assignments.Count; i++)
                {
                    D3AssignmentState assignment = state.assignments[i];
                    if (assignment == null || assignment.installationId != facilityId ||
                        assignment.channelId != Dimension3Catalog.ChannelCoreCoordination ||
                        assignment.stabilizedAmount <= 0L) continue;
                    double contribution = assignment.stabilizedAmount *
                        Dimension3Catalog.GetMkPower(assignment.mk);
                    if (assignment.traitId == Dimension3Catalog.TraitCoordinator)
                        contribution *= 1.25;
                    total += contribution;
                }
            return Math.Sqrt(Math.Max(0.0, total));
        }
        string capacityChannel = GetCapacityChannel(facilityId);
        string coordinationChannel = GetCoordinationChannel(facilityId);
        if (string.IsNullOrWhiteSpace(capacityChannel)) return 0.0;
        double coordination = Math.Sqrt(D3PowerSystem.GetRawChannelPower(
            state, facilityId, coordinationChannel)) * 0.25;
        double multiplier = 1.0 + coordination / 100.0;
        return Math.Sqrt(D3PowerSystem.GetRawChannelPower(
            state, facilityId, capacityChannel)) * multiplier;
    }

    public static int GetAutomationCoreRoutineLimit(Dimension3State state)
    {
        string core = Dimension3Catalog.FacilityAutomationCore;
        if (IsFunctionActive(state, core, 4)) return 5;
        if (IsFunctionActive(state, core, 3)) return 4;
        if (IsFunctionActive(state, core, 2)) return 3;
        if (IsFunctionActive(state, core, 1)) return 2;
        return 1;
    }

    public static int GetAutomationCoreProfileLimit(Dimension3State state)
    {
        string core = Dimension3Catalog.FacilityAutomationCore;
        if (IsFunctionActive(state, core, 4)) return 2;
        if (IsFunctionActive(state, core, 2)) return 1;
        return 0;
    }

    public static double GetAutomationCoreEfficiencyMultiplier(
        Dimension3State state)
    {
        string core = Dimension3Catalog.FacilityAutomationCore;
        if (IsFunctionActive(state, core, 3)) return 1.10;
        if (IsFunctionActive(state, core, 1)) return 1.05;
        return 1.0;
    }

    public static bool CanRunExternalAutomationOffline(Dimension3State state)
    {
        return IsFunctionActive(
            state, Dimension3Catalog.FacilityAutomationCore, 5);
    }

    public static bool IsFunctionActive(
        Dimension3State state, string facilityId, int functionLevel)
    {
        return functionLevel >= 1 && GetFacilityLevel(state, facilityId) >= functionLevel &&
               GetEffectiveCapacity(state, facilityId) + 0.000001 >=
               GetRequiredEffectiveCapacity(functionLevel);
    }

    public static bool IsProcessBankChannelUnlocked(Dimension3State state, string channelId)
    {
        int level = GetProcessBankLevel(state);
        switch (channelId)
        {
            case Dimension3Catalog.ChannelProcessPower: return level >= 1;
            case Dimension3Catalog.ChannelProcessTime: return level >= 2;
            case Dimension3Catalog.ChannelProcessCost: return level >= 3;
            case Dimension3Catalog.ChannelProcessCoordination: return level >= 4;
            default: return false;
        }
    }

    public static D3AssignmentState GetAssignment(
        Dimension3State state, string installationId, string channelId,
        int mk, string traitId)
    {
        if (state == null || state.assignments == null) return null;
        for (int i = 0; i < state.assignments.Count; i++)
        {
            D3AssignmentState assignment = state.assignments[i];
            if (assignment != null && assignment.installationId == installationId &&
                assignment.channelId == channelId && assignment.mk == mk &&
                assignment.traitId == traitId) return assignment;
        }
        return null;
    }

    public static bool TrySetProcessBankAssignment(
        GameState gameState, string channelId, int mk, string traitId,
        long targetAmount, out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        Dimension3State state = gameState.dimension3;
        if (!Dimension3Catalog.IsProcessBankChannel(channelId) ||
            !IsProcessBankChannelUnlocked(state, channelId))
        {
            reason = "Ese canal todavía no está desbloqueado.";
            return false;
        }
        if (Dimension3Catalog.GetMkPower(mk) <= 0 ||
            !Dimension3Catalog.IsTraitId(traitId) || targetAmount < 0L)
        {
            reason = "Asignación inválida.";
            return false;
        }

        D3AssignmentState assignment = GetAssignment(
            state, Dimension3Catalog.FacilityProcessBank, channelId, mk, traitId);
        long current = assignment == null ? 0L : Math.Max(0L, assignment.amount);
        if (targetAmount > current &&
            D3InventorySystem.GetAvailableAutomatonAmount(state, mk, traitId) <
                targetAmount - current)
        {
            reason = "No hay suficientes autómatas libres.";
            return false;
        }

        if (assignment == null)
        {
            assignment = new D3AssignmentState
            {
                installationId = Dimension3Catalog.FacilityProcessBank,
                channelId = channelId,
                mk = mk,
                traitId = traitId
            };
            state.assignments.Add(assignment);
        }

        assignment.amount = targetAmount;
        assignment.stabilizedAmount = Math.Min(
            Math.Max(0L, assignment.stabilizedAmount), targetAmount);
        if (targetAmount > assignment.stabilizedAmount)
            assignment.stabilizationRemainingSeconds =
                Dimension3Catalog.AssignmentStabilizationSeconds;
        else
            assignment.stabilizationRemainingSeconds = 0.0;

        if (targetAmount == 0L && assignment.stabilizedAmount == 0L)
            state.assignments.Remove(assignment);
        return true;
    }

    public static bool TrySetFacilityAssignment(
        GameState gameState, string facilityId, string channelId, int mk,
        string traitId, long targetAmount, out string reason)
    {
        if (facilityId == Dimension3Catalog.FacilityProcessBank)
            return TrySetProcessBankAssignment(
                gameState, channelId, mk, traitId, targetAmount, out reason);

        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        Dimension3State state = gameState.dimension3;
        if (!Dimension3Catalog.IsConnectedFacility(facilityId) ||
            !Dimension3Catalog.IsFacilityChannel(facilityId, channelId) ||
            GetFacilityLevel(state, facilityId) <= 0)
        {
            reason = "Instalación o canal no disponible.";
            return false;
        }
        if (Dimension3Catalog.GetMkPower(mk) <= 0 ||
            !Dimension3Catalog.IsTraitId(traitId) || targetAmount < 0L)
        {
            reason = "Asignación inválida.";
            return false;
        }

        D3AssignmentState assignment = GetAssignment(
            state, facilityId, channelId, mk, traitId);
        long current = assignment == null ? 0L : Math.Max(0L, assignment.amount);
        if (targetAmount > current &&
            D3InventorySystem.GetAvailableAutomatonAmount(state, mk, traitId) <
            targetAmount - current)
        {
            reason = "No hay suficientes autómatas libres.";
            return false;
        }
        if (assignment == null)
        {
            assignment = new D3AssignmentState
            {
                installationId = facilityId,
                channelId = channelId,
                mk = mk,
                traitId = traitId
            };
            state.assignments.Add(assignment);
        }
        assignment.amount = targetAmount;
        assignment.stabilizedAmount = Math.Min(
            Math.Max(0L, assignment.stabilizedAmount), targetAmount);
        assignment.stabilizationRemainingSeconds =
            targetAmount > assignment.stabilizedAmount
                ? Dimension3Catalog.AssignmentStabilizationSeconds
                : 0.0;
        if (targetAmount == 0L && assignment.stabilizedAmount == 0L)
            state.assignments.Remove(assignment);
        return true;
    }

    public static void AdvanceStabilization(Dimension3State state, double seconds)
    {
        if (state == null || state.assignments == null || seconds <= 0.0) return;
        for (int i = state.assignments.Count - 1; i >= 0; i--)
        {
            D3AssignmentState assignment = state.assignments[i];
            if (assignment == null)
            {
                state.assignments.RemoveAt(i);
                continue;
            }
            assignment.amount = Math.Max(0L, assignment.amount);
            assignment.stabilizedAmount = Math.Min(
                assignment.amount, Math.Max(0L, assignment.stabilizedAmount));
            if (assignment.amount > assignment.stabilizedAmount)
            {
                assignment.stabilizationRemainingSeconds = Math.Max(
                    0.0, assignment.stabilizationRemainingSeconds - seconds);
                if (assignment.stabilizationRemainingSeconds <= 0.0)
                    assignment.stabilizedAmount = assignment.amount;
            }
            if (assignment.amount == 0L) state.assignments.RemoveAt(i);
        }
    }

    public static bool TryQueueProcessBankUpgrade(GameState gameState, out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        int targetLevel = GetProcessBankLevel(gameState.dimension3) + 1;
        if (targetLevel > 5)
        {
            reason = "El Banco de Procesos ya alcanzó su nivel máximo.";
            return false;
        }
        D3FacilityLevelDefinition definition =
            Dimension3Catalog.GetProcessBankLevelDefinition(targetLevel);
        if (definition == null)
        {
            reason = "Mejora del Banco no disponible.";
            return false;
        }
        if (D3InventorySystem.GetAssemblyCount(
                gameState.dimension3, definition.requiredAssemblyMk) <
            definition.requiredAssemblyAmount)
        {
            reason = "No se cumple el requisito de ensamblajes para esta mejora.";
            return false;
        }
        D3QueueState queue = D3JobQueueSystem.GetQueue(
            gameState.dimension3, Dimension3Catalog.QueueFacility);
        if (queue == null || queue.jobs.Count > 0)
        {
            reason = "Ya existe una mejora de instalación en curso.";
            return false;
        }

        double leCost = D3PowerSystem.GetModifiedCost(
            gameState.dimension3, definition.leCost);
        double tracesCost = D3PowerSystem.GetModifiedCost(
            gameState.dimension3, definition.tracesCost);
        double duration = Math.Max(0.1, definition.durationSeconds);
        if (gameState.LE + 0.000001 < leCost ||
            gameState.Traces + 0.000001 < tracesCost)
        {
            reason = "Recursos insuficientes para mejorar el Banco.";
            return false;
        }

        gameState.LE -= leCost;
        gameState.Traces -= tracesCost;
        D3JobState job = new D3JobState
        {
            jobType = Dimension3Catalog.JobFacilityUpgrade,
            targetId = Dimension3Catalog.FacilityProcessBank,
            version = targetLevel,
            quantity = 1L,
            baseDurationSeconds = duration,
            remainingSeconds = duration,
            paidLE = leCost,
            paidTraces = tracesCost,
            usesDynamicBankSpeed = true
        };
        if (D3JobQueueSystem.EnqueueCommittedJob(
                gameState.dimension3, Dimension3Catalog.QueueFacility,
                job, out reason)) return true;
        gameState.LE += leCost;
        gameState.Traces += tracesCost;
        return false;
    }

    public static bool TryQueueFacilityUpgrade(
        GameState gameState, string facilityId, out string reason)
    {
        if (facilityId == Dimension3Catalog.FacilityProcessBank)
            return TryQueueProcessBankUpgrade(gameState, out reason);
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState) ||
            !Dimension3Catalog.IsConnectedFacility(facilityId))
        {
            reason = "Instalación no disponible.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        Dimension3State state = gameState.dimension3;
        int targetLevel = GetFacilityLevel(state, facilityId) + 1;
        D3FacilityLevelDefinition definition =
            Dimension3Catalog.GetFacilityLevelDefinition(facilityId, targetLevel);
        if (definition == null)
        {
            reason = "La instalación ya alcanzó su nivel máximo.";
            return false;
        }
        if (definition.requiredAssemblyMk > 0 &&
            D3InventorySystem.GetAssemblyCount(state, definition.requiredAssemblyMk) <
            definition.requiredAssemblyAmount)
        {
            reason = "No se cumple el requisito de ensamblajes.";
            return false;
        }
        if (facilityId == Dimension3Catalog.FacilityDiagnosticBank)
        {
            if (MachineManager.I == null || !MachineManager.I.MachineUnlocked)
            {
                reason = "La Máquina todavía no está desbloqueada.";
                return false;
            }
            if (targetLevel == 2 && !MachineManager.I.NodeAnalysisUnlocked)
            {
                reason = "El Banco de Diagnóstico N2 requiere Análisis de Nodos.";
                return false;
            }
            if (targetLevel == 4 && !MachineManager.I.MachineFusionPanelUnlocked)
            {
                reason = "El Panel de Fusión todavía no está desbloqueado.";
                return false;
            }
        }
        if (facilityId == Dimension3Catalog.FacilityExpeditionPort)
        {
            if (targetLevel == 1 &&
                gameState.dimension1ManualSimpleDestinationIds.Count < 1)
            {
                reason = "El Puerto requiere una exploración simple manual completada.";
                return false;
            }
            if (targetLevel == 2 &&
                gameState.dimension1ManualSimpleDestinationIds.Count < 3)
            {
                reason = "El Puerto N2 requiere tres destinos simples conocidos.";
                return false;
            }
            if (targetLevel == 3 &&
                gameState.dimension1CompletedSimpleExplorations < 10)
            {
                reason = "El Puerto N3 requiere diez exploraciones simples.";
                return false;
            }
            if (targetLevel == 4 && CountUnlockedExtractors(gameState) < 2)
            {
                reason = "El Puerto N4 requiere dos extractores planetarios desbloqueados.";
                return false;
            }
        }
        if (facilityId == Dimension3Catalog.FacilityAutomationCore &&
            targetLevel == 1)
        {
            string[] requiredFacilities =
            {
                Dimension3Catalog.FacilityProcessBank,
                Dimension3Catalog.FacilityProductionConsole,
                Dimension3Catalog.FacilityDiagnosticBank,
                Dimension3Catalog.FacilityExpeditionPort
            };
            for (int i = 0; i < requiredFacilities.Length; i++)
                if (GetFacilityLevel(state, requiredFacilities[i]) < 1)
                {
                    reason = "El Núcleo requiere las otras cuatro instalaciones en N1.";
                    return false;
                }
        }
        D3QueueState queue = D3JobQueueSystem.GetQueue(
            state, Dimension3Catalog.QueueFacility);
        if (queue == null || queue.jobs.Count > 0)
        {
            reason = "Ya existe una construcción o mejora en curso.";
            return false;
        }
        double leCost = D3PowerSystem.GetModifiedCost(state, definition.leCost);
        double tracesCost = D3PowerSystem.GetModifiedCost(state, definition.tracesCost);
        double duration = Math.Max(0.1, definition.durationSeconds);
        if (gameState.LE + 0.000001 < leCost ||
            gameState.Traces + 0.000001 < tracesCost)
        {
            reason = "Recursos insuficientes para la instalación.";
            return false;
        }
        gameState.LE -= leCost;
        gameState.Traces -= tracesCost;
        D3JobState job = new D3JobState
        {
            jobType = Dimension3Catalog.JobFacilityUpgrade,
            targetId = facilityId,
            version = targetLevel,
            quantity = 1L,
            baseDurationSeconds = duration,
            remainingSeconds = duration,
            paidLE = leCost,
            paidTraces = tracesCost,
            usesDynamicBankSpeed = true
        };
        if (D3JobQueueSystem.EnqueueCommittedJob(
                state, Dimension3Catalog.QueueFacility, job, out reason)) return true;
        gameState.LE += leCost;
        gameState.Traces += tracesCost;
        return false;
    }

    private static int CountUnlockedExtractors(GameState gameState)
    {
        if (gameState == null || gameState.dimension1Planets == null) return 0;
        int count = 0;
        for (int i = 0; i < gameState.dimension1Planets.Count; i++)
        {
            D1PlanetState planet = gameState.dimension1Planets[i];
            if (planet != null && planet.unlocked && planet.extractorTier > 0)
                count++;
        }
        return count;
    }

    public static void CompleteFacilityUpgrade(Dimension3State state, D3JobState job)
    {
        if (state == null || job == null ||
            (!Dimension3Catalog.IsConnectedFacility(job.targetId) &&
             job.targetId != Dimension3Catalog.FacilityProcessBank)) return;
        D3FacilityState facility = GetFacility(state, job.targetId);
        if (facility == null) return;
        facility.built = true;
        facility.level = Math.Max(facility.level, Math.Min(5, job.version));
    }
}
