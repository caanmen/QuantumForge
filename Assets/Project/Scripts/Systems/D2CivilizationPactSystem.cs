using System;
using System.Collections.Generic;


public static class D2CivilizationPactSystem
{
    public const string HospitalityId = "d2_c1_pact_hospitality";
    public const string OpenPathId = "d2_c1_pact_open_path";
    public const string ConsecrationId = "d2_c1_pact_consecration";
    public const string SilentVowId = "d2_c1_pact_silent_vow";
    public const string InnerDoorId = "d2_c1_pact_inner_door";

    public const double UnlockTrustRequired = 200.0;
    public const int UnlockNovitiateLevelRequired = 2;
    public const int InitialActiveSlots = 1;
    public const int AdvancedActiveSlots = 2;
    public const double SecondSlotTrustRequired = 400.0;
    public const int SecondSlotNovitiateLevelRequired = 4;
    public const long SecondSlotAcolyteCost = 10L;
    public const double SecondSlotWaxCost = 300.0;
    public const double SecondSlotBreadCost = 300.0;

    public const double HospitalityFollowerBonus = 0.35;
    public const double HospitalityBreadPerMinute = 1.0;
    public const double OpenPathRewardBonus = 0.25;
    public const double OpenPathOfferingCostIncrease = 0.25;
    public const double ConsecrationAcolyteBonus = 0.25;
    public const double ConsecrationDurationIncrease = 0.30;
    public const double SilentVowTrustBonus = 0.50;
    public const double SilentVowFollowerReduction = 0.90;
    public const double InnerDoorBondProgressBonus = 0.25;

    public static readonly string[] PactIds =
    {
        HospitalityId,
        OpenPathId,
        ConsecrationId,
        SilentVowId,
        InnerDoorId
    };

    public static void EnsureState(D2Civilization1State state)
    {
        if (state == null)
            return;

        if (state.civilizationPacts == null)
            state.civilizationPacts = new List<D2CivilizationPactState>();

        var sanitized = new List<D2CivilizationPactState>(PactIds.Length);
        foreach (string pactId in PactIds)
        {
            D2CivilizationPactState pact = FindPact(state.civilizationPacts, pactId) ??
                new D2CivilizationPactState();
            pact.pactId = pactId;
            if (!pact.active)
                pact.suspended = false;
            if (pactId != HospitalityId)
                pact.suspended = false;
            sanitized.Add(pact);
        }
        state.civilizationPacts = sanitized;

        SanitizeFraction(ref state.pactPilgrimageFollowerRewardProgress);
        SanitizeFraction(ref state.pactConsecrationAcolyteProgress);
        if (state.lastCivilizationPactResult == null)
            state.lastCivilizationPactResult = "";

        int slots = GetActiveSlotLimit(state);
        int active = 0;
        foreach (D2CivilizationPactState pact in state.civilizationPacts)
        {
            if (!pact.active)
                continue;

            active++;
            if (active <= slots)
                continue;

            pact.active = false;
            pact.suspended = false;
        }
    }

    public static bool ArePactsUnlocked(D2Civilization1State state)
    {
        return state != null && state.trust >= UnlockTrustRequired &&
            state.novitiateLevel >= UnlockNovitiateLevelRequired;
    }

    public static int GetActiveSlotLimit(D2Civilization1State state)
    {
        return state != null && state.secondCivilizationPactSlotUnlocked
            ? AdvancedActiveSlots
            : InitialActiveSlots;
    }

    public static int GetActivePactCount(D2Civilization1State state)
    {
        if (state?.civilizationPacts == null)
            return 0;

        int count = 0;
        foreach (D2CivilizationPactState pact in state.civilizationPacts)
        {
            if (pact != null && pact.active)
                count++;
        }
        return count;
    }

    public static D2CivilizationPactState GetPact(
        D2Civilization1State state,
        string pactId
    )
    {
        if (state == null || !IsPactId(pactId))
            return null;

        EnsureState(state);
        return FindPact(state.civilizationPacts, pactId);
    }

    public static bool IsPactActive(D2Civilization1State state, string pactId)
    {
        D2CivilizationPactState pact = GetPact(state, pactId);
        return pact != null && pact.active && !pact.suspended;
    }

    public static bool CanActivate(GameState gameState, string pactId)
    {
        if (!Dimension2System.CanAccessDimension2(gameState) || !IsPactId(pactId))
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2CivilizationPactState pact = GetPact(state, pactId);
        if (!ArePactsUnlocked(state) || pact == null || pact.active ||
            GetActivePactCount(state) >= GetActiveSlotLimit(state))
        {
            return false;
        }

        if (pactId == InnerDoorId && !state.entityContactAvailable)
            return false;

        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );
        return wax != null && wax.offeringAmount >= GetWaxActivationCost(pactId) &&
            bread != null && bread.offeringAmount >= GetBreadActivationCost(pactId);
    }

    public static bool TryActivate(GameState gameState, string pactId)
    {
        if (!CanActivate(gameState, pactId))
            return false;

        D2Civilization1State state = gameState.dimension2.civilization1;
        D2CivilizationPactState pact = GetPact(state, pactId);
        D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId).offeringAmount -=
            GetWaxActivationCost(pactId);
        D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId).offeringAmount -=
            GetBreadActivationCost(pactId);
        pact.active = true;
        pact.suspended = false;
        state.lastCivilizationPactResult = GetDisplayName(pactId) + " activado.";
        return true;
    }

    public static bool TryCancel(GameState gameState, string pactId)
    {
        if (!Dimension2System.CanAccessDimension2(gameState) || !IsPactId(pactId))
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        D2CivilizationPactState pact = GetPact(state, pactId);
        if (pact == null || !pact.active)
            return false;

        pact.active = false;
        pact.suspended = false;
        state.lastCivilizationPactResult = GetDisplayName(pactId) +
            " cancelado. El coste de activación no se devolvió.";
        return true;
    }

    public static bool CanUnlockSecondSlot(GameState gameState)
    {
        if (!Dimension2System.CanAccessDimension2(gameState))
            return false;

        Dimension2System.EnsureState(gameState);
        D2Civilization1State state = gameState.dimension2.civilization1;
        if (state.secondCivilizationPactSlotUnlocked ||
            state.trust < SecondSlotTrustRequired ||
            state.novitiateLevel < SecondSlotNovitiateLevelRequired ||
            state.acolytesAvailable < SecondSlotAcolyteCost)
        {
            return false;
        }

        D2AltarState wax = D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId);
        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );
        return wax != null && wax.offeringAmount >= SecondSlotWaxCost &&
            bread != null && bread.offeringAmount >= SecondSlotBreadCost;
    }

    public static bool TryUnlockSecondSlot(GameState gameState)
    {
        if (!CanUnlockSecondSlot(gameState))
            return false;

        D2Civilization1State state = gameState.dimension2.civilization1;
        state.acolytesAvailable -= SecondSlotAcolyteCost;
        D2AltarSystem.GetAltar(state, D2AltarSystem.WaxAltarId).offeringAmount -=
            SecondSlotWaxCost;
        D2AltarSystem.GetAltar(state, D2AltarSystem.RitualBreadAltarId).offeringAmount -=
            SecondSlotBreadCost;
        state.secondCivilizationPactSlotUnlocked = true;
        state.lastCivilizationPactResult = "Segundo espacio de Pacto desbloqueado.";
        return true;
    }

    public static double ConsumeHospitalityMaintenance(
        D2Civilization1State state,
        double seconds
    )
    {
        if (state == null || seconds <= 0.0 || double.IsNaN(seconds) ||
            double.IsInfinity(seconds))
        {
            return 0.0;
        }

        D2CivilizationPactState pact = GetPact(state, HospitalityId);
        if (pact == null || !pact.active)
            return 0.0;

        D2AltarState bread = D2AltarSystem.GetAltar(
            state,
            D2AltarSystem.RitualBreadAltarId
        );
        if (bread == null)
        {
            pact.suspended = true;
            return 0.0;
        }

        double costPerSecond = HospitalityBreadPerMinute / 60.0;
        double supportedSeconds = Math.Min(
            seconds,
            bread.offeringAmount / costPerSecond
        );
        double cost = supportedSeconds * costPerSecond;
        bread.offeringAmount = Math.Max(0.0, bread.offeringAmount - cost);
        pact.suspended = supportedSeconds + 0.000001 < seconds;
        return supportedSeconds;
    }

    public static double GetWaxActivationCost(string pactId)
    {
        switch (pactId)
        {
            case HospitalityId: return 60.0;
            case OpenPathId: return 90.0;
            case ConsecrationId: return 120.0;
            case SilentVowId: return 150.0;
            case InnerDoorId: return 200.0;
            default: return 0.0;
        }
    }

    public static double GetBreadActivationCost(string pactId)
    {
        return pactId == HospitalityId ? 90.0 : GetWaxActivationCost(pactId);
    }

    public static string GetDisplayName(string pactId)
    {
        switch (pactId)
        {
            case HospitalityId: return "Pacto de Hospedaje";
            case OpenPathId: return "Pacto del Camino Abierto";
            case ConsecrationId: return "Pacto de Consagración";
            case SilentVowId: return "Pacto del Voto Silencioso";
            case InnerDoorId: return "Pacto de la Puerta Interior";
            default: return "Pacto desconocido";
        }
    }

    public static string GetBenefitDescription(string pactId)
    {
        switch (pactId)
        {
            case HospitalityId: return "+35% llegada de Seguidores";
            case OpenPathId: return "+25% recompensas de Peregrinaciones";
            case ConsecrationId: return "+25% Acólitos formados";
            case SilentVowId: return "+50% Confianza de Peregrinaciones";
            case InnerDoorId: return "+25% progreso del Lugar de Vínculo";
            default: return "Sin beneficio";
        }
    }

    public static string GetCommitmentDescription(string pactId)
    {
        switch (pactId)
        {
            case HospitalityId: return "Consume 1 Pan ritual por minuto";
            case OpenPathId: return "+25% costes de Ofrendas de Peregrinaciones";
            case ConsecrationId: return "+30% duración de formación";
            case SilentVowId: return "−90% llegada de Seguidores";
            case InnerDoorId: return "Impide iniciar o mejorar el Noviciado";
            default: return "Sin compromiso";
        }
    }

    public static bool ValidateState(D2Civilization1State state, out string result)
    {
        if (state == null)
        {
            result = "Estado de Pactos de Civilización ausente.";
            return false;
        }

        EnsureState(state);
        if (state.civilizationPacts.Count != PactIds.Length)
        {
            result = "Catálogo de Pactos de Civilización incompleto.";
            return false;
        }

        if (GetActivePactCount(state) > GetActiveSlotLimit(state))
        {
            result = "Hay más Pactos activos que espacios disponibles.";
            return false;
        }

        foreach (string pactId in PactIds)
        {
            D2CivilizationPactState pact = FindPact(state.civilizationPacts, pactId);
            if (pact == null || (!pact.active && pact.suspended) ||
                (pactId != HospitalityId && pact.suspended))
            {
                result = "Estado de Pacto inválido: " + pactId + ".";
                return false;
            }
        }

        if (!IsFraction(state.pactPilgrimageFollowerRewardProgress) ||
            !IsFraction(state.pactConsecrationAcolyteProgress))
        {
            result = "Acumulador fraccional de Pactos fuera de rango.";
            return false;
        }

        result = "Estado de Pactos de Civilización válido.";
        return true;
    }

    public static bool IsPactId(string pactId)
    {
        foreach (string id in PactIds)
        {
            if (id == pactId)
                return true;
        }
        return false;
    }

    private static D2CivilizationPactState FindPact(
        List<D2CivilizationPactState> pacts,
        string pactId
    )
    {
        if (pacts == null)
            return null;

        foreach (D2CivilizationPactState pact in pacts)
        {
            if (pact != null && pact.pactId == pactId)
                return pact;
        }
        return null;
    }

    private static void SanitizeFraction(ref double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0.0)
            value = 0.0;
        value -= Math.Floor(value);
    }

    private static bool IsFraction(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value) &&
            value >= 0.0 && value < 1.0;
    }
}
