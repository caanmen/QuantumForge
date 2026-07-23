using System;
using System.Text;


public sealed class D3FactoryCostPreview
{
    public long quantity;
    public long missingPartsTotal;
    public double missingPartsLE;
    public double missingPartsTraces;
    public double assemblyLE;
    public double assemblyTraces;
    public double estimatedSeconds;

    public double TotalLE => missingPartsLE + assemblyLE;
    public double TotalTraces => missingPartsTraces + assemblyTraces;

    public string ToDisplayText()
    {
        var builder = new StringBuilder();
        builder.Append("PREVISIÓN MK ×").Append(quantity);
        builder.Append(" | piezas faltantes: ").Append(missingPartsTotal);
        builder.Append("\nPiezas: ").Append(Format(missingPartsLE)).Append(" LE + ")
            .Append(Format(missingPartsTraces)).Append(" T");
        builder.Append(" | Ensamble: ").Append(Format(assemblyLE)).Append(" LE + ")
            .Append(Format(assemblyTraces)).Append(" T");
        builder.Append(" | Total: ").Append(Format(TotalLE)).Append(" LE + ")
            .Append(Format(TotalTraces)).Append(" T | ≈")
            .Append(Math.Ceiling(estimatedSeconds)).Append(" s");
        return builder.ToString();
    }

    private static string Format(double value)
    {
        if (value >= 1000000000.0) return (value / 1000000000.0).ToString("0.##") + "B";
        if (value >= 1000000.0) return (value / 1000000.0).ToString("0.##") + "M";
        if (value >= 1000.0) return (value / 1000.0).ToString("0.##") + "K";
        return Math.Floor(Math.Max(0.0, value)).ToString("0");
    }
}

public static class D3FactoryPreviewSystem
{
    public static D3FactoryCostPreview GetAssemblyPreview(
        GameState gameState, int mk, long quantity)
    {
        if (gameState == null || gameState.dimension3 == null || quantity <= 0L)
            return null;
        D3CostTimeDefinition part = Dimension3Catalog.GetPartDefinition(mk);
        D3CostTimeDefinition assembly = Dimension3Catalog.GetNormalAssemblyDefinition(mk);
        if (part == null || assembly == null) return null;

        long missingTotal = 0L;
        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
        {
            long owned = D3InventorySystem.GetPartAmount(
                gameState.dimension3, Dimension3Catalog.PartIds[i], mk);
            missingTotal += Math.Max(0L, quantity - owned);
        }

        double rate = D3PowerSystem.GetDynamicWorkRate(gameState.dimension3);
        var preview = new D3FactoryCostPreview
        {
            quantity = quantity,
            missingPartsTotal = missingTotal,
            missingPartsLE = D3PowerSystem.GetModifiedCost(gameState.dimension3,
                part.leCost * missingTotal),
            missingPartsTraces = D3PowerSystem.GetModifiedCost(gameState.dimension3,
                part.tracesCost * missingTotal),
            assemblyLE = D3PowerSystem.GetModifiedCost(gameState.dimension3,
                assembly.leCost * quantity),
            assemblyTraces = D3PowerSystem.GetModifiedCost(gameState.dimension3,
                assembly.tracesCost * quantity),
            estimatedSeconds = (part.durationSeconds * missingTotal +
                assembly.durationSeconds * quantity) / rate
        };
        return preview;
    }
}
