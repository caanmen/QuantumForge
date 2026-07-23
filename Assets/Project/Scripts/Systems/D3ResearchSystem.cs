using System;
using System.Collections.Generic;


public sealed class D3ResearchTeamModifiers
{
    public double requirementPower;
    public double progressMultiplier = 1.0;
    public double timeMultiplier = 1.0;
    public double costMultiplier = 1.0;
    public double coordinationPercent;
}


public static class D3ResearchSystem
{
    public static bool IsCompleted(Dimension3State state, string partId, int version)
    {
        if (state == null || state.research == null) return false;
        string id = Dimension3Catalog.GetResearchId(partId, version);
        for (int i = 0; i < state.research.Count; i++)
        {
            D3ResearchState research = state.research[i];
            if (research != null && research.researchId == id) return research.completed;
        }
        return false;
    }

    public static bool AreAllCompleted(Dimension3State state, int version)
    {
        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
            if (!IsCompleted(state, Dimension3Catalog.PartIds[i], version)) return false;
        return true;
    }

    public static D3ResearchTeamModifiers CalculateTeamModifiers(
        IList<D3ReservedAutomatonState> team)
    {
        double normalPower = 0.0;
        double fastPower = 0.0;
        double efficientPower = 0.0;
        double coordinatorPower = 0.0;
        if (team != null)
        {
            for (int i = 0; i < team.Count; i++)
            {
                D3ReservedAutomatonState member = team[i];
                if (member == null || member.amount <= 0L) continue;
                double power = member.amount * Dimension3Catalog.GetMkPower(member.mk);
                if (member.traitId == Dimension3Catalog.TraitNormal) normalPower += power * 1.25;
                else if (member.traitId == Dimension3Catalog.TraitFast) fastPower += power * 1.25;
                else if (member.traitId == Dimension3Catalog.TraitEfficient) efficientPower += power * 1.25;
                else if (member.traitId == Dimension3Catalog.TraitCoordinator) coordinatorPower += power * 1.25;
            }
        }
        double coordination = Math.Sqrt(coordinatorPower) * 0.35;
        double group = 1.0 + coordination / 100.0;
        double progressBonus = Math.Sqrt(normalPower) * 0.50 * group;
        double timeBonus = Math.Sqrt(fastPower) * 0.25 * group;
        double costBonus = Math.Sqrt(efficientPower) * 0.20 * group;
        return new D3ResearchTeamModifiers
        {
            requirementPower = (normalPower + fastPower + efficientPower + coordinatorPower) * group,
            progressMultiplier = 1.0 + progressBonus / 100.0,
            timeMultiplier = 1.0 / (1.0 + timeBonus / 100.0),
            costMultiplier = 1.0 / (1.0 + costBonus / 100.0),
            coordinationPercent = coordination
        };
    }

    public static bool TryQueueResearch(
        GameState gameState, string partId, int version,
        IList<D3ReservedAutomatonState> requestedTeam, out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        D3ResearchDefinition definition =
            Dimension3Catalog.GetResearchDefinition(partId, version);
        if (definition == null)
        {
            reason = "Investigación inválida.";
            return false;
        }
        if (IsCompleted(gameState.dimension3, partId, version))
        {
            reason = "Esa investigación ya está completada.";
            return false;
        }
        if (HasQueuedResearch(gameState.dimension3, definition.researchId))
        {
            reason = "Esa investigación ya está en la cola.";
            return false;
        }
        if (!ValidatePrerequisites(gameState, partId, version, out reason)) return false;
        if (!D3JobQueueSystem.CanAcceptJob(
                gameState.dimension3, Dimension3Catalog.QueueResearch))
        {
            reason = "La cola de investigación está llena.";
            return false;
        }

        List<D3ReservedAutomatonState> team;
        if (!NormalizeAndValidateTeam(
                gameState.dimension3, requestedTeam, out team, out reason)) return false;
        D3ResearchTeamModifiers teamModifiers = CalculateTeamModifiers(team);
        if (teamModifiers.requirementPower + 0.000001 < definition.minimumPower)
        {
            reason = "El equipo no alcanza la potencia mínima de investigación.";
            return false;
        }

        D3ProcessModifiers bank = D3PowerSystem.GetProcessBankModifiers(gameState.dimension3);
        double leCost = Math.Ceiling(definition.leCost * bank.costMultiplier *
            teamModifiers.costMultiplier);
        double tracesCost = Math.Ceiling(definition.tracesCost * bank.costMultiplier *
            teamModifiers.costMultiplier);
        // El equipo reservado fija su propia eficiencia al confirmar. La potencia
        // del Banco, en cambio, se aplica mientras el trabajo está avanzando.
        double duration = Math.Max(0.1, definition.durationSeconds *
            teamModifiers.timeMultiplier /
            Math.Max(0.000001, teamModifiers.progressMultiplier));
        if (gameState.LE + 0.000001 < leCost ||
            gameState.Traces + 0.000001 < tracesCost)
        {
            reason = "Recursos insuficientes para la investigación.";
            return false;
        }

        gameState.LE -= leCost;
        gameState.Traces -= tracesCost;
        var job = new D3JobState
        {
            jobType = Dimension3Catalog.JobResearch,
            targetId = definition.researchId,
            version = version,
            baseDurationSeconds = duration,
            remainingSeconds = duration,
            paidLE = leCost,
            paidTraces = tracesCost,
            usesDynamicBankSpeed = true
        };
        for (int i = 0; i < team.Count; i++) job.reservedAutomatons.Add(team[i]);
        if (D3JobQueueSystem.EnqueueCommittedJob(
                gameState.dimension3, Dimension3Catalog.QueueResearch,
                job, out reason)) return true;
        gameState.LE += leCost;
        gameState.Traces += tracesCost;
        return false;
    }

    public static void CompleteResearch(Dimension3State state, D3JobState job)
    {
        if (state == null || job == null ||
            job.jobType != Dimension3Catalog.JobResearch) return;
        D3ResearchState target = null;
        for (int i = 0; i < state.research.Count; i++)
            if (state.research[i] != null && state.research[i].researchId == job.targetId)
                target = state.research[i];
        if (target == null)
        {
            target = new D3ResearchState { researchId = job.targetId };
            state.research.Add(target);
        }
        target.completed = true;
    }

    public static bool ValidatePrerequisites(
        GameState gameState, string partId, int version, out string reason)
    {
        reason = "";
        int bankLevel = D3FacilitySystem.GetProcessBankLevel(gameState.dimension3);
        if (version == 4)
        {
            if (bankLevel < 4 ||
                D3InventorySystem.GetAssemblyCount(gameState.dimension3, 3) < 10L)
            {
                reason = "V4 requiere Banco nivel 4 y 10 MK3 ensamblados.";
                return false;
            }
            return true;
        }
        if (version == 5)
        {
            if (!IsCompleted(gameState.dimension3, partId, 4) || bankLevel < 5 ||
                D3InventorySystem.GetAssemblyCount(gameState.dimension3, 4) < 5L)
            {
                reason = "V5 requiere su V4, Banco nivel 5 y 5 MK4 ensamblados.";
                return false;
            }
            return true;
        }
        if (version == 6)
        {
            D3FacilityState core = D3FacilitySystem.GetFacility(
                gameState.dimension3, Dimension3Catalog.FacilityAutomationCore);
            if (!IsCompleted(gameState.dimension3, partId, 5) ||
                core == null || !core.built || core.level < 4 ||
                D3InventorySystem.GetAssemblyCount(gameState.dimension3, 5) < 5L)
            {
                reason = "V6 requiere su V5, Núcleo nivel 4 y 5 MK5 ensamblados.";
                return false;
            }
            return true;
        }
        reason = "Versión de investigación inválida.";
        return false;
    }

    private static bool NormalizeAndValidateTeam(
        Dimension3State state, IList<D3ReservedAutomatonState> requested,
        out List<D3ReservedAutomatonState> team, out string reason)
    {
        team = new List<D3ReservedAutomatonState>();
        reason = "";
        if (requested == null || requested.Count == 0)
        {
            reason = "Selecciona al menos un investigador.";
            return false;
        }
        for (int i = 0; i < requested.Count; i++)
        {
            D3ReservedAutomatonState member = requested[i];
            if (member == null || member.amount <= 0L ||
                Dimension3Catalog.GetMkPower(member.mk) <= 0 ||
                !Dimension3Catalog.IsTraitId(member.traitId))
            {
                reason = "Equipo de investigación inválido.";
                return false;
            }
            D3ReservedAutomatonState existing = FindMember(team, member.mk, member.traitId);
            if (existing == null)
            {
                existing = new D3ReservedAutomatonState
                    { mk = member.mk, traitId = member.traitId };
                team.Add(existing);
            }
            existing.amount += member.amount;
        }
        for (int i = 0; i < team.Count; i++)
        {
            D3ReservedAutomatonState member = team[i];
            if (D3InventorySystem.GetAvailableAutomatonAmount(
                    state, member.mk, member.traitId) < member.amount)
            {
                reason = "El equipo incluye autómatas que no están libres.";
                return false;
            }
        }
        return true;
    }

    private static D3ReservedAutomatonState FindMember(
        List<D3ReservedAutomatonState> team, int mk, string traitId)
    {
        for (int i = 0; i < team.Count; i++)
            if (team[i].mk == mk && team[i].traitId == traitId) return team[i];
        return null;
    }

    private static bool HasQueuedResearch(Dimension3State state, string researchId)
    {
        D3QueueState queue = D3JobQueueSystem.GetQueue(
            state, Dimension3Catalog.QueueResearch);
        if (queue == null || queue.jobs == null) return false;
        for (int i = 0; i < queue.jobs.Count; i++)
            if (queue.jobs[i] != null && queue.jobs[i].targetId == researchId) return true;
        return false;
    }
}
