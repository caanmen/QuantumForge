public static class D2VeiledThresholdSystem
{
    public const double UnlockTrustRequired = 500.0;

    public static void EnsureState(D2Civilization1State state)
    {
        if (state != null && state.trust >= UnlockTrustRequired)
            state.entityContactAvailable = true;
    }

    public static bool IsUnlocked(D2Civilization1State state)
    {
        EnsureState(state);
        return state != null && state.entityContactAvailable;
    }

    public static bool ValidateState(D2Civilization1State state, out string result)
    {
        if (state == null)
        {
            result = "Estado del Umbral Velado ausente.";
            return false;
        }

        EnsureState(state);
        if (state.trust >= UnlockTrustRequired && !state.entityContactAvailable)
        {
            result = "El Umbral Velado no quedó desbloqueado al alcanzar 500 de Confianza.";
            return false;
        }

        result = "Estado del Umbral Velado válido.";
        return true;
    }
}
