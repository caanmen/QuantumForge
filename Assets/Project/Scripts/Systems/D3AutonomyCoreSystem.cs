/// <summary>
/// Gestiona el hito único que cierra la ruta de la Dimensión 3 para el
/// siguiente Prestigio 1. La integración es intencionalmente manual.
/// </summary>
public static class D3AutonomyCoreSystem
{
    public const int RequiredCoreLevel = 5;
    public const int RequiredMk = 6;

    public static bool HasIntegrated(Dimension3State state)
    {
        return state != null && state.autonomyCoreIntegrated;
    }

    public static bool CanIntegrate(GameState gameState, out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "La Dimensión 3 está bloqueada.";
            return false;
        }

        Dimension3System.EnsureState(gameState);
        Dimension3State state = gameState.dimension3;
        if (state.autonomyCoreIntegrated)
        {
            reason = "El Núcleo de Autonomía ya fue integrado.";
            return false;
        }

        if (D3FacilitySystem.GetFacilityLevel(
                state, Dimension3Catalog.FacilityAutomationCore) < RequiredCoreLevel)
        {
            reason = "Construye el Núcleo de Automatización hasta el nivel 5.";
            return false;
        }

        D3AssignmentState assignment = D3FacilitySystem.GetAssignment(
            state, Dimension3Catalog.FacilityAutomationCore,
            Dimension3Catalog.ChannelCoreCoordination, RequiredMk,
            Dimension3Catalog.TraitCoordinator);
        if (assignment == null || assignment.stabilizedAmount < 1L)
        {
            reason = "Asigna y estabiliza un MK6 Coordinador en el Núcleo.";
            return false;
        }

        return true;
    }

    public static bool TryIntegrate(GameState gameState, out string reason)
    {
        if (!CanIntegrate(gameState, out reason)) return false;

        gameState.dimension3.autonomyCoreIntegrated = true;
        SaveService.I?.Save();
        reason = gameState.IsPrestige1CycleComplete()
            ? "Núcleo de Autonomía integrado. El ciclo de Prestigio 1 está completo; Prestigio 2 llegará en una expansión futura."
            : "Núcleo de Autonomía integrado. El próximo Prestigio 1 ya puede abrirse al completar la Máquina.";
        return true;
    }
}
