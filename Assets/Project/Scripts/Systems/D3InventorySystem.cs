using System;


public static class D3InventorySystem
{
    public static long GetPartAmount(Dimension3State state, string partId, int version)
    {
        D3PartStackState stack = FindPartStack(state, partId, version);
        return stack == null ? 0L : Math.Max(0L, stack.amount);
    }

    public static void AddParts(
        Dimension3State state,
        string partId,
        int version,
        long amount
    )
    {
        if (state == null || !Dimension3Catalog.IsPartId(partId) ||
            Dimension3Catalog.GetPartDefinition(version) == null || amount <= 0L)
        {
            return;
        }

        D3PartStackState stack = FindPartStack(state, partId, version);

        if (stack == null)
        {
            stack = new D3PartStackState
            {
                partId = partId,
                version = version,
                amount = 0L
            };
            state.parts.Add(stack);
        }

        stack.amount = SafeAdd(stack.amount, amount);
    }

    public static bool TryConsumeParts(
        Dimension3State state,
        string partId,
        int version,
        long amount
    )
    {
        if (amount <= 0L)
            return false;

        D3PartStackState stack = FindPartStack(state, partId, version);
        if (stack == null || stack.amount < amount)
            return false;

        stack.amount -= amount;
        return true;
    }

    public static bool HasCompletePartSet(Dimension3State state, int version, long quantity)
    {
        if (state == null || quantity <= 0L)
            return false;

        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
        {
            if (GetPartAmount(state, Dimension3Catalog.PartIds[i], version) < quantity)
                return false;
        }

        return true;
    }

    public static long GetAutomatonAmount(
        Dimension3State state,
        int mk,
        string traitId
    )
    {
        D3AutomatonStackState stack = FindAutomatonStack(state, mk, traitId);
        return stack == null ? 0L : Math.Max(0L, stack.totalAmount);
    }

    public static long GetAssignedAutomatonAmount(
        Dimension3State state,
        int mk,
        string traitId)
    {
        if (state == null || state.assignments == null)
            return 0L;

        long total = 0L;
        for (int i = 0; i < state.assignments.Count; i++)
        {
            D3AssignmentState assignment = state.assignments[i];
            if (assignment != null && assignment.mk == mk &&
                assignment.traitId == traitId && assignment.amount > 0L)
            {
                total = SafeAdd(total, assignment.amount);
            }
        }
        return total;
    }

    public static long GetReservedAutomatonAmount(
        Dimension3State state,
        int mk,
        string traitId)
    {
        if (state == null || state.queues == null)
            return 0L;

        long total = 0L;
        for (int queueIndex = 0; queueIndex < state.queues.Count; queueIndex++)
        {
            D3QueueState queue = state.queues[queueIndex];
            if (queue == null || queue.jobs == null) continue;
            for (int jobIndex = 0; jobIndex < queue.jobs.Count; jobIndex++)
            {
                D3JobState job = queue.jobs[jobIndex];
                if (job == null || job.reservedAutomatons == null) continue;
                for (int reserveIndex = 0; reserveIndex < job.reservedAutomatons.Count; reserveIndex++)
                {
                    D3ReservedAutomatonState reserve = job.reservedAutomatons[reserveIndex];
                    if (reserve != null && reserve.mk == mk &&
                        reserve.traitId == traitId && reserve.amount > 0L)
                    {
                        total = SafeAdd(total, reserve.amount);
                    }
                }
            }
        }
        return total;
    }

    public static long GetAvailableAutomatonAmount(
        Dimension3State state,
        int mk,
        string traitId)
    {
        long total = GetAutomatonAmount(state, mk, traitId);
        long unavailable = SafeAdd(
            GetAssignedAutomatonAmount(state, mk, traitId),
            GetReservedAutomatonAmount(state, mk, traitId)
        );
        return Math.Max(0L, total - unavailable);
    }

    public static long GetAssemblyCount(Dimension3State state, int mk)
    {
        if (state == null || state.totalAssembledByMk == null)
            return 0L;
        for (int i = 0; i < state.totalAssembledByMk.Count; i++)
        {
            D3MkAssemblyCountState count = state.totalAssembledByMk[i];
            if (count != null && count.mk == mk)
                return Math.Max(0L, count.amount);
        }
        return 0L;
    }

    public static void AddAutomatons(
        Dimension3State state,
        int mk,
        string traitId,
        long amount
    )
    {
        if (state == null || Dimension3Catalog.GetMkPower(mk) <= 0 ||
            string.IsNullOrWhiteSpace(traitId) || amount <= 0L)
        {
            return;
        }

        D3AutomatonStackState stack = FindAutomatonStack(state, mk, traitId);
        if (stack == null)
        {
            stack = new D3AutomatonStackState
            {
                mk = mk,
                traitId = traitId,
                totalAmount = 0L
            };
            state.automatons.Add(stack);
        }

        stack.totalAmount = SafeAdd(stack.totalAmount, amount);
    }

    public static void AddAssemblyCount(Dimension3State state, int mk, long amount)
    {
        if (state == null || Dimension3Catalog.GetMkPower(mk) <= 0 || amount <= 0L)
            return;

        D3MkAssemblyCountState countState = null;
        for (int i = 0; i < state.totalAssembledByMk.Count; i++)
        {
            D3MkAssemblyCountState candidate = state.totalAssembledByMk[i];
            if (candidate != null && candidate.mk == mk)
            {
                countState = candidate;
                break;
            }
        }

        if (countState == null)
        {
            countState = new D3MkAssemblyCountState { mk = mk };
            state.totalAssembledByMk.Add(countState);
        }

        countState.amount = SafeAdd(countState.amount, amount);
    }

    public static void RefundConsumedParts(Dimension3State state, D3JobState job)
    {
        if (state == null || job == null || job.consumedParts == null)
            return;

        for (int i = 0; i < job.consumedParts.Count; i++)
        {
            D3PartAmountState part = job.consumedParts[i];
            if (part == null)
                continue;

            AddParts(state, part.partId, part.version, part.amount);
        }
    }

    private static D3PartStackState FindPartStack(
        Dimension3State state,
        string partId,
        int version
    )
    {
        if (state == null || state.parts == null)
            return null;

        for (int i = 0; i < state.parts.Count; i++)
        {
            D3PartStackState stack = state.parts[i];
            if (stack != null && stack.partId == partId && stack.version == version)
                return stack;
        }

        return null;
    }

    private static D3AutomatonStackState FindAutomatonStack(
        Dimension3State state,
        int mk,
        string traitId
    )
    {
        if (state == null || state.automatons == null)
            return null;

        for (int i = 0; i < state.automatons.Count; i++)
        {
            D3AutomatonStackState stack = state.automatons[i];
            if (stack != null && stack.mk == mk && stack.traitId == traitId)
                return stack;
        }

        return null;
    }

    private static long SafeAdd(long current, long amount)
    {
        if (current < 0L)
            current = 0L;

        if (long.MaxValue - current < amount)
            return long.MaxValue;

        return current + amount;
    }
}
