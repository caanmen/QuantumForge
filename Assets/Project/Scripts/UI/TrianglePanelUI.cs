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

        string primaryId = GameState.I.GetTriangleBuildingId(TriangleSlotRole.Primary);
        string reinforcementId = GameState.I.GetTriangleBuildingId(TriangleSlotRole.Reinforcement);
        string alterationId = GameState.I.GetTriangleBuildingId(TriangleSlotRole.Alteration);

        SetLabelIfChanged(
            assignedLabelPrimary,
            GetShortName(primaryId),
            ref lastPrimaryId,
            force);

        SetLabelIfChanged(
            assignedLabelReinforcement,
            GetShortName(reinforcementId),
            ref lastReinforcementId,
            force);

        SetLabelIfChanged(
            assignedLabelAlteration,
            GetShortName(alterationId),
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

    private string GetShortName(string buildingId)
    {
        switch (buildingId)
        {
            case "vacuum_observer":
                return "Higgs";

            case "casimir_panel":
                return "Tetra";

            case "fluctuation_antenna":
                return "Modulator";

            default:
                return "";
        }
    }

    private string GetProtocolStatusText()
    {
        var lm = LocalizationManager.I;

        string label = lm != null
            ? lm.T("triangle.protocol.label")
            : "Protocolo";

        string protocolName;

        if (GameState.I == null || !GameState.I.IsTriangleFullyConfiguredWithBaseArtifacts())
        {
            protocolName = lm != null
                ? lm.T("triangle.protocol.none")
                : "Inactivo";
        }
        else
        {
            switch (GameState.I.GetActiveTriangleProtocol())
            {
                case TriangleProtocolType.Impulso:
                    protocolName = lm != null
                        ? lm.T("triangle.protocol.impulso")
                        : "Impulso";
                    break;

                case TriangleProtocolType.Sinergia:
                    protocolName = lm != null
                        ? lm.T("triangle.protocol.sinergia")
                        : "Sinergia";
                    break;

                case TriangleProtocolType.Persistencia:
                    protocolName = lm != null
                        ? lm.T("triangle.protocol.persistencia")
                        : "Persistencia";
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
            ? lm.T("triangle.modulator.mode.label")
            : "Fase";

        if (GameState.I == null || !GameState.I.IsPhaseModulatorOwned())
        {
            string inactive = lm != null
                ? lm.T("triangle.modulator.mode.none")
                : "Inactiva";

            return $"{label}: {inactive}";
        }

        string modeName;
        switch (GameState.I.phaseModulatorMode)
        {
            case PhaseModulatorMode.Expansion:
                modeName = lm != null ? lm.T("triangle.modulator.mode.expansion") : "Expansión";
                break;

            case PhaseModulatorMode.Conservation:
                modeName = lm != null ? lm.T("triangle.modulator.mode.conservation") : "Conservación";
                break;

            case PhaseModulatorMode.Attunement:
                modeName = lm != null ? lm.T("triangle.modulator.mode.attunement") : "Sintonía";
                break;

            default:
                modeName = lm != null ? lm.T("triangle.modulator.mode.none") : "Inactiva";
                break;
        }

        int calibrationPercent = Mathf.RoundToInt(GameState.I.phaseModulatorCalibration * 100f);
        return $"{label}: {modeName} ({calibrationPercent}%)";
    }

    private string GetModulatorEffectText()
    {
        var lm = LocalizationManager.I;

        string label = lm != null
            ? lm.T("triangle.modulator.effect.label")
            : "Efecto";

        if (GameState.I == null || !GameState.I.IsPhaseModulatorOwned())
        {
            string inactive = lm != null
                ? lm.T("triangle.modulator.effect.inactive")
                : "Sin efecto";

            return $"{label}: {inactive}";
        }

        switch (GameState.I.phaseModulatorMode)
        {
            case PhaseModulatorMode.Expansion:
            {
                float bonusPercent = GameState.I.GetPhaseModulatorExpansionTickBonus() * 100f;
                string prefix = lm != null
                    ? lm.T("triangle.modulator.effect.expansion")
                    : "Ticks más rápidos";
                return $"{label}: {prefix} +{bonusPercent:0.0}%";
            }

            case PhaseModulatorMode.Conservation:
            {
                float discountPercent = GameState.I.GetPhaseModulatorConservationDiscount() * 100f;
                string prefix = lm != null
                    ? lm.T("triangle.modulator.effect.conservation")
                    : "Costes reducidos";
                return $"{label}: {prefix} -{discountPercent:0.0}%";
            }

            case PhaseModulatorMode.Attunement:
            {
                string reserved = lm != null
                    ? lm.T("triangle.modulator.effect.attunement")
                    : "Reservado para Prestigio 1";
                return $"{label}: {reserved}";
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