using System;
using System.Collections.Generic;


public sealed class D3CalibrationEvaluation
{
    public int calibratedCount;
    public string candidateTraitId = Dimension3Catalog.TraitNormal;
    public string resultTraitId = Dimension3Catalog.TraitNormal;
    public double affinity;
    public bool ambiguous;

    public string GetAffinityBand()
    {
        if (ambiguous) return "Ambigua";
        if (affinity < 60.0) return "Insuficiente";
        if (affinity < 75.0) return "Baja";
        if (affinity < 90.0) return "Media";
        return "Alta";
    }
}


public static class D3CalibrationSystem
{
    private const double TieTolerance = 0.01;

    public static bool TryCalculateReading(
        D3CalibrationControlState controls,
        out int reading,
        out string reason)
    {
        reading = 0;
        reason = "";
        if (controls == null || !Dimension3Catalog.IsPartId(controls.partId))
        {
            reason = "Controles de calibración inválidos.";
            return false;
        }

        switch (controls.partId)
        {
            case Dimension3Catalog.PartChassis:
                if (!InRange(controls.valueA) || !InRange(controls.valueB) ||
                    !InRange(controls.valueC) ||
                    controls.valueA + controls.valueB + controls.valueC != 100)
                {
                    reason = "El Chasis debe distribuir exactamente 100 puntos.";
                    return false;
                }
                double chassisBase = controls.valueA * 0.2 +
                    controls.valueB * 0.5 + controls.valueC * 0.3;
                reading = RoundAndClamp((chassisBase - 20.0) / 30.0 * 100.0);
                return true;

            case Dimension3Catalog.PartMotor:
                if (!ThreeValuesInRange(controls))
                {
                    reason = "Los tres controles motrices deben estar entre 0 y 100.";
                    return false;
                }
                reading = RoundAndClamp(controls.valueA * 0.45 +
                    controls.valueB * 0.35 + controls.valueC * 0.20);
                return true;

            case Dimension3Catalog.PartTool:
                if (controls.optionA < 0 || controls.optionA > 3 ||
                    !InRange(controls.valueA))
                {
                    reason = "Zona o intensidad de Herramienta inválida.";
                    return false;
                }
                int[] bases = { 20, 40, 60, 80 };
                reading = RoundAndClamp(bases[controls.optionA] * 0.6 +
                    controls.valueA * 0.4);
                return true;

            case Dimension3Catalog.PartControl:
                if (!IsPermutation(controls.valueA, controls.valueB, controls.valueC) ||
                    !IsPermutation(controls.optionA, controls.optionB, controls.optionC))
                {
                    reason = "Cada entrada, nodo y salida debe utilizarse exactamente una vez.";
                    return false;
                }
                int sum = 0;
                int[] nodes = { controls.valueA, controls.valueB, controls.valueC };
                int[] outputs = { controls.optionA, controls.optionB, controls.optionC };
                for (int input = 0; input < 3; input++)
                {
                    bool nodeChanged = nodes[input] != input;
                    bool outputChanged = outputs[input] != input;
                    sum += !nodeChanged && !outputChanged ? 15 :
                        nodeChanged && outputChanged ? 35 : 25;
                }
                reading = RoundAndClamp((sum - 45.0) / 60.0 * 100.0);
                return true;

            case Dimension3Catalog.PartRegulator:
                if (!InRange(controls.valueA) || !InRange(controls.valueB))
                {
                    reason = "Voltaje y Resistencia deben estar entre 0 y 100.";
                    return false;
                }
                reading = RoundAndClamp(controls.valueA * 0.6 +
                    (100 - controls.valueB) * 0.4);
                return true;
        }

        reason = "Pieza sin minijuego de calibración.";
        return false;
    }

    public static D3CalibrationEvaluation Evaluate(
        IList<D3CalibrationReadingState> readings)
    {
        var evaluation = new D3CalibrationEvaluation();
        var valid = new List<D3CalibrationReadingState>();
        if (readings != null)
        {
            for (int i = 0; i < readings.Count; i++)
            {
                D3CalibrationReadingState reading = readings[i];
                if (reading == null || !Dimension3Catalog.IsPartId(reading.partId) ||
                    reading.reading < 0 || reading.reading > 100 ||
                    ContainsPart(valid, reading.partId)) continue;
                valid.Add(reading);
            }
        }
        evaluation.calibratedCount = valid.Count;
        if (valid.Count == 0) return evaluation;

        string[] traits =
        {
            Dimension3Catalog.TraitFast,
            Dimension3Catalog.TraitEfficient,
            Dimension3Catalog.TraitCoordinator
        };
        double best = double.MinValue;
        double second = double.MinValue;
        string bestTrait = Dimension3Catalog.TraitNormal;
        for (int traitIndex = 0; traitIndex < traits.Length; traitIndex++)
        {
            double squared = 0.0;
            for (int readingIndex = 0; readingIndex < valid.Count; readingIndex++)
            {
                D3CalibrationReadingState reading = valid[readingIndex];
                double delta = reading.reading -
                    Dimension3Catalog.GetTraitPatternReading(
                        traits[traitIndex], reading.partId);
                squared += delta * delta;
            }
            double affinity = Math.Max(0.0, Math.Min(100.0,
                100.0 - Math.Sqrt(squared / valid.Count)));
            if (affinity > best)
            {
                second = best;
                best = affinity;
                bestTrait = traits[traitIndex];
            }
            else if (affinity > second) second = affinity;
        }

        evaluation.candidateTraitId = bestTrait;
        evaluation.affinity = best;
        evaluation.ambiguous = Math.Abs(best - second) <= TieTolerance;
        if (valid.Count == Dimension3Catalog.PartIds.Length &&
            !evaluation.ambiguous && best >= 60.0)
        {
            evaluation.resultTraitId = bestTrait;
        }
        return evaluation;
    }

    public static bool TrySaveProfile(
        GameState gameState,
        string displayName,
        IList<D3CalibrationControlState> controls,
        out string reason)
    {
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        if (D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) < 1)
        {
            reason = "Se requiere Banco de Procesos nivel 1.";
            return false;
        }
        List<D3CalibrationControlState> normalized;
        List<D3CalibrationReadingState> readings;
        if (!TryNormalizeCompleteControls(controls, out normalized, out readings, out reason))
            return false;

        D3CalibrationProfileState profile = gameState.dimension3.calibrationProfiles.Count > 0
            ? gameState.dimension3.calibrationProfiles[0]
            : null;
        if (profile == null)
        {
            profile = new D3CalibrationProfileState { profileId = "calibration_profile_1" };
            gameState.dimension3.calibrationProfiles.Clear();
            gameState.dimension3.calibrationProfiles.Add(profile);
        }
        profile.displayName = string.IsNullOrWhiteSpace(displayName)
            ? "Calibración guardada" : displayName.Trim();
        profile.controls = normalized;
        profile.readings = readings;
        return true;
    }

    public static bool TryLoadProfile(
        GameState gameState,
        out List<D3CalibrationControlState> controls,
        out string reason)
    {
        controls = null;
        reason = "";
        if (!Dimension3System.CanAccessDimension3(gameState))
        {
            reason = "Dimensión 3 está bloqueada.";
            return false;
        }
        Dimension3System.EnsureState(gameState);
        if (D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) < 2)
        {
            reason = "Cargar perfiles requiere Banco de Procesos nivel 2.";
            return false;
        }
        if (gameState.dimension3.calibrationProfiles.Count == 0 ||
            gameState.dimension3.calibrationProfiles[0] == null)
        {
            reason = "No existe una calibración guardada.";
            return false;
        }
        List<D3CalibrationReadingState> ignored;
        return TryNormalizeCompleteControls(
            gameState.dimension3.calibrationProfiles[0].controls,
            out controls, out ignored, out reason);
    }

    public static bool CanAutoRepeatOnePiece(GameState gameState)
    {
        return Dimension3System.CanAccessDimension3(gameState) &&
            D3FacilitySystem.GetProcessBankLevel(gameState.dimension3) >= 3;
    }

    public static D3CalibrationControlState CloneControls(
        D3CalibrationControlState source)
    {
        return source == null ? null : new D3CalibrationControlState
        {
            partId = source.partId,
            valueA = source.valueA,
            valueB = source.valueB,
            valueC = source.valueC,
            optionA = source.optionA,
            optionB = source.optionB,
            optionC = source.optionC
        };
    }

    private static bool TryNormalizeCompleteControls(
        IList<D3CalibrationControlState> controls,
        out List<D3CalibrationControlState> normalized,
        out List<D3CalibrationReadingState> readings,
        out string reason)
    {
        normalized = new List<D3CalibrationControlState>();
        readings = new List<D3CalibrationReadingState>();
        reason = "";
        for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
        {
            string partId = Dimension3Catalog.PartIds[i];
            D3CalibrationControlState found = FindControls(controls, partId);
            int reading;
            if (found == null || !TryCalculateReading(found, out reading, out reason))
            {
                if (string.IsNullOrEmpty(reason)) reason = "Falta calibrar " + partId + ".";
                return false;
            }
            normalized.Add(CloneControls(found));
            readings.Add(new D3CalibrationReadingState
                { partId = partId, reading = reading });
        }
        return true;
    }

    private static D3CalibrationControlState FindControls(
        IList<D3CalibrationControlState> controls, string partId)
    {
        if (controls == null) return null;
        for (int i = 0; i < controls.Count; i++)
            if (controls[i] != null && controls[i].partId == partId) return controls[i];
        return null;
    }

    private static bool ContainsPart(
        List<D3CalibrationReadingState> readings, string partId)
    {
        for (int i = 0; i < readings.Count; i++)
            if (readings[i].partId == partId) return true;
        return false;
    }

    private static bool InRange(int value) { return value >= 0 && value <= 100; }
    private static bool ThreeValuesInRange(D3CalibrationControlState controls)
    {
        return InRange(controls.valueA) && InRange(controls.valueB) &&
            InRange(controls.valueC);
    }
    private static bool IsPermutation(int a, int b, int c)
    {
        return a >= 0 && a <= 2 && b >= 0 && b <= 2 && c >= 0 && c <= 2 &&
            a != b && a != c && b != c;
    }
    private static int RoundAndClamp(double value)
    {
        return Math.Max(0, Math.Min(100,
            (int)Math.Round(value, MidpointRounding.AwayFromZero)));
    }
}
