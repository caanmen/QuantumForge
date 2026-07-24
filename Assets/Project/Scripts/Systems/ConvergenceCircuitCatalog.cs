public static class ConvergenceCircuitCatalog
{
    public const string StartupPulseCircuitId =
        "convergence_circuit_001_startup_pulse";
    public static double StartupPulseBaseLEProductionBonus = 0.10;
    public const int ExperimentalBoardRadius = 2;

    public static bool IsKnownCircuit(string circuitId)
    {
        return circuitId == StartupPulseCircuitId;
    }

    public static bool IsValidRotation(int rotationDegrees)
    {
        return rotationDegrees == 0 || rotationDegrees == 90 ||
            rotationDegrees == 180 || rotationDegrees == 270;
    }

    public static bool IsValidBoardCoordinate(int x, int y)
    {
        return x >= -ExperimentalBoardRadius && x <= ExperimentalBoardRadius &&
            y >= -ExperimentalBoardRadius && y <= ExperimentalBoardRadius;
    }
}
