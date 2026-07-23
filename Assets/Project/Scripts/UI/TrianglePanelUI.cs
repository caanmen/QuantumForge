using TMPro;
using UnityEngine;

public class TrianglePanelUI : MonoBehaviour
{
    [Header("Assigned labels")]
    [SerializeField] private TextMeshProUGUI assignedLabelPrimary;
    [SerializeField] private TextMeshProUGUI assignedLabelReinforcement;
    [SerializeField] private TextMeshProUGUI assignedLabelAlteration;
    [SerializeField] private TextMeshProUGUI protocolStatusLabel;
    [SerializeField] private TextMeshProUGUI modulatorModeLabel;
    [SerializeField] private TextMeshProUGUI modulatorEffectLabel;

    [Header("Refresh")]
    [SerializeField] private float refreshInterval = 0.15f;

    private float refreshTimer = 0f;

    private string lastPrimaryId = "";
    private string lastReinforcementId = "";
    private string lastAlterationId = "";
    private string lastProtocolText = "";
    private string lastModulatorModeText = "";
    private string lastModulatorEffectText = "";
    private int lastLocalizationRevision = -1;

    private void OnEnable()
    {
        lastLocalizationRevision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision
            : -1;

        RefreshAll(force: true);
    }

    private void Update()
    {
        bool forceRefresh = false;

        int currentLocalizationRevision = LocalizationManager.I != null
            ? LocalizationManager.I.Revision
            : -1;

        if (currentLocalizationRevision != lastLocalizationRevision)
        {
            lastLocalizationRevision = currentLocalizationRevision;
            forceRefresh = true;
        }

        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f && !forceRefresh)
            return;

        refreshTimer = refreshInterval;
        RefreshAll(force: forceRefresh);
    }

    private void RefreshAll(bool force)
    {
        if (GameState.I == null)
        {
            SetLabelIfChanged(assignedLabelPrimary, "", ref lastPrimaryId, force);
            SetLabelIfChanged(assignedLabelReinforcement, "", ref lastReinforcementId, force);
            SetLabelIfChanged(assignedLabelAlteration, "", ref lastAlterationId, force);
            SetLabelIfChanged(protocolStatusLabel, GetProtocolStatusText(), ref lastProtocolText, force);
            SetLabelIfChanged(modulatorModeLabel, GetModulatorModeText(), ref lastModulatorModeText, force);
            SetLabelIfChanged(modulatorEffectLabel, GetModulatorEffectText(), ref lastModulatorEffectText, force);
            return;
        }

        SetLabelIfChanged(
            assignedLabelPrimary,
            GetCircuitButtonText(TriangleCircuitType.Energy),
            ref lastPrimaryId,
            force);

        SetLabelIfChanged(
            assignedLabelReinforcement,
            GetCircuitButtonText(TriangleCircuitType.Experimental),
            ref lastReinforcementId,
            force);

        SetLabelIfChanged(
            assignedLabelAlteration,
            GetCircuitButtonText(TriangleCircuitType.Phase),
            ref lastAlterationId,
            force);

        SetLabelIfChanged(
            protocolStatusLabel,
            GetProtocolStatusText(),
            ref lastProtocolText,
            force);

        SetLabelIfChanged(
            modulatorModeLabel,
            GetModulatorModeText(),
            ref lastModulatorModeText,
            force);

        SetLabelIfChanged(
            modulatorEffectLabel,
            GetModulatorEffectText(),
            ref lastModulatorEffectText,
            force);
    }

    private void SetLabelIfChanged(
        TextMeshProUGUI label,
        string displayText,
        ref string lastValue,
        bool force)
    {
        if (label == null)
            return;

        if (!force && lastValue == displayText)
            return;

        lastValue = displayText;
        label.text = displayText;
    }

    private string GetCircuitButtonText(TriangleCircuitType circuit)
    {
        var lm = LocalizationManager.I;
        switch (circuit)
        {
            case TriangleCircuitType.Energy:
                return lm != null ? lm.T("triangle.circuit.energy.button") : "ENERGÍA\nHiggs + Modulador";
            case TriangleCircuitType.Experimental:
                return lm != null ? lm.T("triangle.circuit.experimental.button") : "EXPERIMENTAL\nHiggs + Tetra";
            case TriangleCircuitType.Phase:
                if (GameState.I != null && !GameState.I.IsTrianglePhaseUnlocked())
                    return lm != null ? lm.T("triangle.circuit.phase.locked_button") : "FASE\nBloqueado: Máquina";
                return lm != null ? lm.T("triangle.circuit.phase.button") : "FASE\nTetra + Modulador";
            default:
                return "";
        }
    }

    private string GetProtocolStatusText()
    {
        var lm = LocalizationManager.I;

        string label = lm != null
            ? lm.T("triangle.circuit.label")
            : "Circuito";

        string protocolName;

        if (GameState.I == null || !GameState.I.IsTriangleFullyConfiguredWithBaseArtifacts())
        {
            protocolName = lm != null
                ? lm.T("triangle.protocol.none")
                : "Inactivo";
        }
        else
        {
            switch (GameState.I.triangleActiveCircuit)
            {
                case TriangleCircuitType.Energy:
                    protocolName = lm != null
                        ? lm.T("triangle.circuit.energy")
                        : "Energía";
                    break;

                case TriangleCircuitType.Experimental:
                    protocolName = lm != null
                        ? lm.T("triangle.circuit.experimental")
                        : "Experimental";
                    break;

                case TriangleCircuitType.Phase:
                    protocolName = lm != null
                        ? lm.T("triangle.circuit.phase")
                        : "Fase";
                    break;

                default:
                    protocolName = lm != null
                        ? lm.T("triangle.protocol.none")
                        : "Inactivo";
                    break;
            }
        }

        return $"{label}: {protocolName}";
    }

    private string GetModulatorModeText()
    {
        var lm = LocalizationManager.I;

        string label = lm != null
            ? lm.T("triangle.synchronization.label")
            : "Sincronización";

        if (GameState.I == null || !GameState.I.IsPhaseModulatorOwned())
        {
            return $"{label}: 0%";
        }
        int synchronizationPercent = Mathf.RoundToInt(GameState.I.triangleSynchronization * 100f);
        return $"{label}: {synchronizationPercent}%";
    }

    private string GetModulatorEffectText()
    {
        var lm = LocalizationManager.I;

        string label = lm != null
            ? lm.T("triangle.circuit.effect.label")
            : "Efecto";

        if (GameState.I == null || !GameState.I.IsPhaseModulatorOwned())
        {
            string inactive = lm != null
                ? lm.T("triangle.modulator.effect.inactive")
                : "Sin efecto";

            return $"{label}: {inactive}";
        }

        switch (GameState.I.triangleActiveCircuit)
        {
            case TriangleCircuitType.Energy:
            {
                double leBonus = (GameState.I.GetTriangleLEMultiplier() - 1.0) * 100.0;
                string format = lm != null
                    ? lm.T("triangle.circuit.energy.effect_format")
                    : "Efecto: +{0:0.#}% LE · -10% Trazas";
                return string.Format(format, leBonus);
            }
            case TriangleCircuitType.Experimental:
            {
                double tracesBonus = (GameState.I.GetTriangleTracesMultiplier() - 1.0) * 100.0;
                double fragmentsBonus = (GameState.I.GetTriangleFragmentMultiplier() - 1.0) * 100.0;
                string format = lm != null
                    ? lm.T("triangle.circuit.experimental.effect_format")
                    : "Efecto: +{0:0.#}% Trazas · +{1:0.#}% fragmentos · -10% LE";
                return string.Format(format, tracesBonus, fragmentsBonus);
            }
            case TriangleCircuitType.Phase:
            {
                double analysisBonus = (GameState.I.GetTrianglePhaseAnalysisSpeedMultiplier() - 1.0) * 100.0;
                double routineBonus = (GameState.I.GetTriangleD3RoutineSpeedMultiplier() - 1.0) * 100.0;
                string format = lm != null
                    ? lm.T("triangle.circuit.phase.effect_format")
                    : "Efecto: +{0:0.#}% análisis · +{1:0.#}% rutinas N3 · -10% LE/Trazas";
                return string.Format(format, analysisBonus, routineBonus);
            }

            default:
            {
                string inactive = lm != null
                    ? lm.T("triangle.modulator.effect.inactive")
                    : "Sin efecto";
                return $"{label}: {inactive}";
            }
        }
    }
}
