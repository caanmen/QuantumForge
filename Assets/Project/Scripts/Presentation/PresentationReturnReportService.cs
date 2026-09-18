using System;
using System.Collections.Generic;


public sealed class PresentationReturnSnapshot
{
    public double le;
    public double traces;
    public double triangleEnergy;
    public readonly HashSet<string> d2Features =
        new HashSet<string>(StringComparer.Ordinal);
    public readonly HashSet<string> d3Features =
        new HashSet<string>(StringComparer.Ordinal);
}

public sealed class PresentationReturnReport
{
    public double elapsedSeconds;
    public double appliedSeconds;
    public double d2AppliedSeconds;
    public double d3AppliedSeconds;
    public double leDelta;
    public double tracesDelta;
    public double triangleEnergyDelta;
    public int targetDimension;
    public string targetScreenId = "";
    public readonly List<string> newsKeys = new List<string>();
    public string newFeatureId = "";
}

public static class PresentationReturnReportService
{
    public const double SignificantAbsenceSeconds = 60.0;
    public const int MaxNewsItems = 3;
    public static PresentationReturnReport PendingReport { get; private set; }
    public static bool UnifiedReportPreparedThisLoad { get; private set; }
    public static event Action ReportPrepared;

    public static PresentationReturnSnapshot Capture(GameState gameState)
    {
        var snapshot = new PresentationReturnSnapshot();
        if (gameState == null) return snapshot;
        snapshot.le = gameState.LE;
        snapshot.traces = gameState.Traces;
        snapshot.triangleEnergy = gameState.triangleEnergy;
        gameState.EnsureDimension2State();
        gameState.EnsureDimension3State();
        Copy(gameState.dimension2?.presentation?.introducedFeatureIds,
            snapshot.d2Features);
        Copy(gameState.dimension3?.presentation?.introducedFeatureIds,
            snapshot.d3Features);
        return snapshot;
    }

    public static PresentationReturnReport Prepare(
        PresentationReturnSnapshot before,
        GameState gameState,
        double elapsedSeconds,
        double d2AppliedSeconds,
        double d3AppliedSeconds,
        double baseAppliedSeconds = 0.0)
    {
        PendingReport = Build(before, gameState, elapsedSeconds,
            d2AppliedSeconds, d3AppliedSeconds, baseAppliedSeconds);
        // Esta capa decide si corresponde mostrar algo. Incluso una ausencia corta
        // debe suprimir reportes legacy para evitar un modal obligatorio.
        UnifiedReportPreparedThisLoad = true;
        // La UI ya existe antes de GameState.Start. Avisarla aqui permite mostrar
        // el resultado en el mismo arranque, sin depender de que un Update futuro
        // descubra el informe pendiente.
        ReportPrepared?.Invoke();
        return PendingReport;
    }

    public static PresentationReturnReport Consume()
    {
        PresentationReturnReport value = PendingReport;
        PendingReport = null;
        return value;
    }

    public static PresentationReturnReport Build(
        PresentationReturnSnapshot before,
        GameState gameState,
        double elapsedSeconds,
        double d2AppliedSeconds,
        double d3AppliedSeconds,
        double baseAppliedSeconds = 0.0)
    {
        if (gameState == null || elapsedSeconds < SignificantAbsenceSeconds)
            return null;
        before ??= new PresentationReturnSnapshot();
        gameState.EnsureDimension2State();
        gameState.EnsureDimension3State();
        D2PresentationRules.EnsurePresentationState(gameState);
        D3PresentationRules.EnsurePresentationState(gameState);

        var report = new PresentationReturnReport
        {
            elapsedSeconds = Math.Max(0.0, elapsedSeconds),
            appliedSeconds = Math.Max(baseAppliedSeconds,
                Math.Max(d2AppliedSeconds, d3AppliedSeconds)),
            d2AppliedSeconds = Math.Max(0.0, d2AppliedSeconds),
            d3AppliedSeconds = Math.Max(0.0, d3AppliedSeconds),
            leDelta = SafeDelta(gameState.LE, before.le),
            tracesDelta = SafeDelta(gameState.Traces, before.traces),
            triangleEnergyDelta = SafeDelta(
                gameState.triangleEnergy, before.triangleEnergy)
        };
        if (d2AppliedSeconds > 0.0) Add(report, "return.d2");
        if (d3AppliedSeconds > 0.0) Add(report, "return.d3");

        string newD2 = FindNewFeature(
            gameState.dimension2.presentation.introducedFeatureIds,
            before.d2Features);
        string newD3 = FindNewFeature(
            gameState.dimension3.presentation.introducedFeatureIds,
            before.d3Features);
        if (!string.IsNullOrEmpty(newD2) || !string.IsNullOrEmpty(newD3))
        {
            report.newFeatureId = !string.IsNullOrEmpty(newD3) ? newD3 : newD2;
            Add(report, "return.new");
        }
        bool preferD3 = !string.IsNullOrEmpty(newD3) ||
            (string.IsNullOrEmpty(newD2) && d3AppliedSeconds > 0.0);
        if (preferD3 && Dimension3System.CanAccessDimension3(gameState))
        {
            report.targetDimension = 3;
            report.targetScreenId = D3PresentationRouter.ResolveSafeScreen(
                gameState, gameState.dimension3.presentation.lastScreenId);
        }
        else if (Dimension2System.CanAccessDimension2(gameState))
        {
            report.targetDimension = 2;
            report.targetScreenId = D2PresentationRouter.ResolveSafeScreen(
                gameState, gameState.dimension2.presentation.lastScreenId);
        }
        else if (Dimension3System.CanAccessDimension3(gameState))
        {
            report.targetDimension = 3;
            report.targetScreenId = D3PresentationRouter.ResolveSafeScreen(
                gameState, gameState.dimension3.presentation.lastScreenId);
        }
        return report;
    }

    public static bool HasValidTarget(
        GameState gameState, PresentationReturnReport report)
    {
        if (gameState == null || report == null) return false;
        return report.targetDimension == 2
            ? D2PresentationRouter.CanOpen(gameState, report.targetScreenId)
            : report.targetDimension == 3 &&
              D3PresentationRouter.CanOpen(gameState, report.targetScreenId);
    }

    private static void Add(PresentationReturnReport report, string key)
    {
        if (report.newsKeys.Count < MaxNewsItems &&
            !report.newsKeys.Contains(key)) report.newsKeys.Add(key);
    }

    private static void Copy(List<string> source, HashSet<string> target)
    {
        if (source == null) return;
        foreach (string id in source)
            if (!string.IsNullOrEmpty(id)) target.Add(id);
    }

    private static double SafeDelta(double current, double previous)
    {
        double value = current - previous;
        return double.IsNaN(value) || double.IsInfinity(value) ? 0.0 : value;
    }

    private static string FindNewFeature(
        List<string> current, HashSet<string> previous)
    {
        if (current == null) return "";
        foreach (string id in current)
            if (!string.IsNullOrEmpty(id) && !previous.Contains(id)) return id;
        return "";
    }
}
