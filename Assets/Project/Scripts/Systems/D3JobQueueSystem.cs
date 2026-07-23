using System;


public static class D3JobQueueSystem
{
    private const double CompletionEpsilon = 0.000001;

    public static D3QueueState GetQueue(Dimension3State state, string queueId)
    {
        if (state == null || state.queues == null)
            return null;

        for (int i = 0; i < state.queues.Count; i++)
        {
            D3QueueState queue = state.queues[i];
            if (queue != null && queue.queueId == queueId)
                return queue;
        }

        return null;
    }

    public static int GetJobCount(Dimension3State state, string queueId)
    {
        D3QueueState queue = GetQueue(state, queueId);
        return queue == null || queue.jobs == null ? 0 : queue.jobs.Count;
    }

    public static bool CanAcceptJob(Dimension3State state, string queueId)
    {
        D3QueueState queue = GetQueue(state, queueId);
        return queue != null && queue.jobs != null &&
               queue.jobs.Count < Dimension3Catalog.MaxQueueEntries;
    }

    public static bool EnqueueCommittedJob(
        Dimension3State state,
        string queueId,
        D3JobState job,
        out string reason
    )
    {
        reason = "";

        if (state == null || job == null || !Dimension3Catalog.IsQueueId(queueId))
        {
            reason = "Trabajo o cola inválidos.";
            return false;
        }

        D3QueueState queue = GetQueue(state, queueId);
        if (queue == null || queue.jobs == null)
        {
            reason = "La cola no está preparada.";
            return false;
        }

        if (queue.jobs.Count >= Dimension3Catalog.MaxQueueEntries)
        {
            reason = "La cola está llena.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(job.jobId))
            job.jobId = CreateJobId(state, queueId);

        job.baseDurationSeconds = Math.Max(0.1, job.baseDurationSeconds);
        job.remainingSeconds = Math.Max(0.1, job.remainingSeconds);
        job.started = false;
        queue.jobs.Add(job);
        StartFrontJob(queue);
        return true;
    }

    public static bool TryCancelJob(
        GameState gameState,
        string queueId,
        string jobId,
        out string reason
    )
    {
        reason = "";
        if (gameState == null || gameState.dimension3 == null)
        {
            reason = "Estado de Fábrica no disponible.";
            return false;
        }

        D3QueueState queue = GetQueue(gameState.dimension3, queueId);
        if (queue == null || queue.jobs == null)
        {
            reason = "Cola no disponible.";
            return false;
        }

        int index = -1;
        for (int i = 0; i < queue.jobs.Count; i++)
        {
            D3JobState candidate = queue.jobs[i];
            if (candidate != null && candidate.jobId == jobId)
            {
                index = i;
                break;
            }
        }

        if (index < 0)
        {
            reason = "Trabajo no encontrado.";
            return false;
        }

        D3JobState job = queue.jobs[index];
        if (!job.started)
        {
            gameState.LE += Math.Max(0.0, job.paidLE);
            gameState.Traces += Math.Max(0.0, job.paidTraces);
            D3InventorySystem.RefundConsumedParts(gameState.dimension3, job);
        }

        queue.jobs.RemoveAt(index);
        StartFrontJob(queue);
        return true;
    }

    public static void AdvanceAllQueues(GameState gameState, double seconds)
    {
        if (gameState == null || gameState.dimension3 == null ||
            seconds <= 0.0 || double.IsNaN(seconds) || double.IsInfinity(seconds))
        {
            return;
        }

        for (int i = 0; i < Dimension3Catalog.QueueIds.Length; i++)
            AdvanceQueue(gameState, Dimension3Catalog.QueueIds[i], seconds);
    }

    private static void AdvanceQueue(GameState gameState, string queueId, double seconds)
    {
        D3QueueState queue = GetQueue(gameState.dimension3, queueId);
        if (queue == null || queue.jobs == null || queue.jobs.Count == 0)
            return;

        double remainingWindow = seconds;
        int safety = 0;

        while (remainingWindow > CompletionEpsilon && queue.jobs.Count > 0)
        {
            safety++;
            if (safety > 100000)
                break;

            StartFrontJob(queue);
            D3JobState job = queue.jobs[0];
            if (job == null)
            {
                queue.jobs.RemoveAt(0);
                continue;
            }

            double workRate = job.usesDynamicBankSpeed
                ? D3PowerSystem.GetDynamicWorkRate(gameState.dimension3)
                : 1.0;
            double requiredWork = Math.Max(0.0, job.remainingSeconds);
            double requiredRealSeconds = requiredWork / workRate;
            if (requiredRealSeconds > remainingWindow + CompletionEpsilon)
            {
                job.remainingSeconds = Math.Max(
                    0.0, requiredWork - remainingWindow * workRate);
                remainingWindow = 0.0;
                break;
            }

            remainingWindow = Math.Max(
                0.0, remainingWindow - requiredRealSeconds);
            CompleteJob(gameState, job);
            queue.jobs.RemoveAt(0);
        }

        StartFrontJob(queue);
    }

    private static void CompleteJob(GameState gameState, D3JobState job)
    {
        if (job.jobType == Dimension3Catalog.JobPartProduction)
        {
            D3InventorySystem.AddParts(
                gameState.dimension3,
                job.targetId,
                job.version,
                job.quantity
            );
            return;
        }

        if (job.jobType == Dimension3Catalog.JobAssembly)
        {
            D3InventorySystem.AddAutomatons(
                gameState.dimension3,
                job.mk,
                string.IsNullOrWhiteSpace(job.resultTraitId)
                    ? Dimension3Catalog.TraitNormal
                    : job.resultTraitId,
                job.quantity
            );
            D3InventorySystem.AddAssemblyCount(gameState.dimension3, job.mk, job.quantity);
            return;
        }

        if (job.jobType == Dimension3Catalog.JobFacilityUpgrade)
            D3FacilitySystem.CompleteFacilityUpgrade(gameState.dimension3, job);
        if (job.jobType == Dimension3Catalog.JobResearch)
            D3ResearchSystem.CompleteResearch(gameState.dimension3, job);
    }

    private static void StartFrontJob(D3QueueState queue)
    {
        if (queue == null || queue.jobs == null || queue.jobs.Count == 0)
            return;

        D3JobState job = queue.jobs[0];
        if (job != null)
            job.started = true;
    }

    private static string CreateJobId(Dimension3State state, string queueId)
    {
        long sequence = Math.Max(1L, state.nextJobSequence);
        state.nextJobSequence = sequence == long.MaxValue ? long.MaxValue : sequence + 1L;
        return "d3_" + queueId + "_" + sequence;
    }
}
