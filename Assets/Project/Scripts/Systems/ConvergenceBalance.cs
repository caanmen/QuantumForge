using System;

public static class ConvergenceBalance
{
    // Balance experimental aprobado para pruebas. Debe recalibrarse con ciclos
    // reales antes de declararse definitivo.
    public const double TentativeBaseStabilityRequirement = 120.0;
    public const double TentativeCircuitGrowth = 0.55;

    // Ajuste central experimental para las actividades que sincronizan una
    // dimensión. Se mantiene separado del requisito total para poder
    // balancear el ritmo sin alterar la curva de circuitos.
    public const double SecondsPerStabilityPoint = 5.0;

    public static double GetRequiredStabilityForNextCircuit(int ownedCircuitCount)
    {
        int circuits = Math.Max(0, ownedCircuitCount);
        double growth = 1.0 + TentativeCircuitGrowth * circuits;
        return Math.Ceiling(TentativeBaseStabilityRequirement * growth * growth);
    }

    public static double GetStabilityForBaseWorkSeconds(double baseWorkSeconds)
    {
        if (double.IsNaN(baseWorkSeconds) || double.IsInfinity(baseWorkSeconds))
            return 0.0;

        return Math.Max(0.0, baseWorkSeconds) / SecondsPerStabilityPoint;
    }
}
