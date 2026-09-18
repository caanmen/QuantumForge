using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presentación visual del ARK. Dimension1PanelUI conserva la propiedad de las
/// acciones; este componente únicamente refleja los datos reales del sistema.
/// </summary>
public sealed class Dimension1ArkVisualUI : MonoBehaviour
{
    [Serializable]
    public sealed class MissionView
    {
        public string missionId;
        public Image frame;
        public Image glow;
        public Image artwork;
        public TMP_Text title;
        public TMP_Text status;
        public TMP_Text timer;
    }

    [SerializeField] private Dimension1MetalsInventoryUI metalsInventory;
    [SerializeField] private TMP_Text[] metalAmounts;
    [SerializeField] private TMP_Text[] metalRates;
    [SerializeField] private TMP_Text investigationTitle;
    [SerializeField] private TMP_Text investigationSubtitle;
    [SerializeField] private MissionView[] missions;
    [SerializeField] private TMP_Text accessProgress;
    [SerializeField] private Graphic[] echoFrames;
    [SerializeField] private Graphic[] echoCores;
    [SerializeField] private TMP_Text[] echoStatuses;
    [SerializeField] private Image finalFrame;
    [SerializeField] private TMP_Text finalLabel;
    [SerializeField] private TMP_Text feedback;

    private static readonly Color Cyan = Hex("16B9E8");
    private static readonly Color CyanMuted = Hex("07536C", 184);
    private static readonly Color BlueprintMuted = Hex("71C7DA", 214);
    private static readonly Color Amber = Hex("D99A12");
    private static readonly Color Green = Hex("00C69A");
    private static readonly Color Disabled = Hex("5E6870");
    private static readonly Color Dark = Hex("10171D", 248);
    private float refreshTimer;

    public void Configure(
        Dimension1MetalsInventoryUI configuredMetalsInventory,
        TMP_Text[] configuredMetalAmounts,
        TMP_Text[] configuredMetalRates,
        TMP_Text configuredInvestigationTitle,
        TMP_Text configuredInvestigationSubtitle,
        MissionView[] configuredMissions,
        TMP_Text configuredAccessProgress,
        Graphic[] configuredEchoFrames,
        Graphic[] configuredEchoCores,
        TMP_Text[] configuredEchoStatuses,
        Image configuredFinalFrame,
        TMP_Text configuredFinalLabel,
        TMP_Text configuredFeedback)
    {
        metalsInventory = configuredMetalsInventory;
        metalAmounts = configuredMetalAmounts;
        metalRates = configuredMetalRates;
        investigationTitle = configuredInvestigationTitle;
        investigationSubtitle = configuredInvestigationSubtitle;
        missions = configuredMissions;
        accessProgress = configuredAccessProgress;
        echoFrames = configuredEchoFrames;
        echoCores = configuredEchoCores;
        echoStatuses = configuredEchoStatuses;
        finalFrame = configuredFinalFrame;
        finalLabel = configuredFinalLabel;
        feedback = configuredFeedback;
    }

    private void OnEnable()
    {
        refreshTimer = 0f;
        RefreshData();
    }

    private void Update()
    {
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = .2f;
        RefreshData();
    }

    public void OpenMetals()
    {
        if (metalsInventory != null) metalsInventory.Open();
    }

    private void RefreshData()
    {
        GameState state = GameState.I;
        if (state == null) return;
        state.EnsureDimension1State();

        Dimension1HeaderMetalsUI.Refresh(transform, state, state.dimension1SelectedSectorId);

        if (investigationTitle != null)
            investigationTitle.text = state.dimension1ArkInvestigated
                ? "ARK INVESTIGADA"
                : "SEÑAL DEL ARK";
        if (investigationSubtitle != null)
            investigationSubtitle.text = state.dimension1ArkInvestigated
                ? "La entrada espera una coincidencia."
                : "Investiga la nave para revelar su patrón de acceso.";

        int synchronized = 0;
        if (missions != null)
        {
            foreach (MissionView view in missions)
            {
                D1CentralSyncMissionState mission =
                    Dimension1System.GetD1CentralSyncMission(state, view.missionId);
                if (mission != null && (mission.completed || mission.active)) synchronized++;
                RefreshMission(state, view, mission);
            }
        }

        if (accessProgress != null)
            accessProgress.text = synchronized + "/4 ECOS";

        for (int i = 0; i < 4; i++)
        {
            D1CentralSyncMissionState mission = missions != null && i < missions.Length
                ? Dimension1System.GetD1CentralSyncMission(state, missions[i].missionId)
                : null;
            bool done = mission != null && mission.completed;
            bool active = mission != null && mission.active;
            Color color = done ? Green : active ? Cyan : Disabled;
            if (echoFrames != null && i < echoFrames.Length && echoFrames[i] != null)
            {
                Transform diagram = echoFrames[i].transform.parent;
                Graphic[] pieces = diagram != null
                    ? diagram.GetComponentsInChildren<Graphic>(true)
                    : Array.Empty<Graphic>();
                foreach (Graphic piece in pieces)
                    if (echoCores == null || i >= echoCores.Length || piece != echoCores[i])
                        piece.color = color;
            }
            if (echoCores != null && i < echoCores.Length && echoCores[i] != null)
            {
                echoCores[i].color = color;
                echoCores[i].gameObject.SetActive(done || active);
            }
            if (echoStatuses != null && i < echoStatuses.Length && echoStatuses[i] != null)
            {
                echoStatuses[i].text = done ? "COMPLETADO" : active ? "ACTIVO" : "INACTIVO";
                echoStatuses[i].color = color;
            }
        }

        bool discovered = state.dimension1GalacticAnchorDiscovered;
        bool activeFinal = state.dimension1ArkFinalMissionActive;
        bool ready = Dimension1System.AreD1ArkRequirementsMet(state);
        if (finalFrame != null)
            finalFrame.color = discovered ? Green : activeFinal || ready ? Amber : Disabled;
        if (finalLabel != null)
        {
            finalLabel.text = discovered
                ? "ANCLA GALÁCTICA OBTENIDA"
                : activeFinal
                    ? "ARK EN CURSO · " + FormatSeconds(state.dimension1ArkFinalMissionRemainingSeconds)
                    : "ENTRAR A ARK · 90 MIN";
            finalLabel.color = discovered ? Green : activeFinal || ready ? Amber : Disabled;
        }
        if (feedback != null)
            feedback.text = !state.dimension1ArkInvestigated
                ? "TOCA LA SEÑAL PARA INVESTIGAR"
                : !ready && !activeFinal && !discovered
                    ? "REQUISITOS DE ACCESO PENDIENTES"
                    : "";
    }

    private static void RefreshMission(
        GameState state,
        MissionView view,
        D1CentralSyncMissionState mission)
    {
        if (view == null) return;
        bool completed = mission != null && mission.completed;
        bool active = mission != null && mission.active;
        bool canStart = Dimension1System.CanStartD1CentralSyncMission(
            state, view.missionId, out _);
        Color stateColor = completed ? Green : active ? Amber : canStart ? Cyan : Disabled;

        if (view.frame != null)
        {
            Color frameColor = active ? Amber : CyanMuted;
            frameColor.a = active ? .78f : .72f;
            view.frame.color = frameColor;
        }
        if (view.glow != null)
        {
            Color glow = active ? Amber : Dark;
            glow.a = active ? .025f : 0f;
            view.glow.color = glow;
        }
        if (view.title != null) view.title.color = active ? Amber : Cyan;
        if (view.artwork != null)
            view.artwork.color = active ? Amber : completed || canStart ? BlueprintMuted : Disabled;
        if (view.status != null)
        {
            view.status.text = completed
                ? "COMPLETADA"
                : active
                    ? "ACTIVA"
                    : canStart
                        ? "INICIAR · 60 MIN"
                        : "PENDIENTE · 60 MIN";
            view.status.color = stateColor;
        }
        if (view.timer != null)
        {
            view.timer.gameObject.SetActive(active);
            view.timer.text = active ? FormatSeconds(mission.remainingSeconds) : "";
            view.timer.color = Amber;
        }
    }

    private static void Set(TMP_Text[] values, int index, string value)
    {
        if (values != null && index < values.Length && values[index] != null)
            values[index].text = value;
    }

    private static string FormatAmount(double value)
    {
        value = Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        if (value >= 100d) return value.ToString("0");
        if (value >= 10d) return value.ToString("0.#");
        return value.ToString("0.##");
    }

    private static string FormatSeconds(double seconds)
    {
        int total = Mathf.Max(0, Mathf.CeilToInt((float)seconds));
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }

    private static Color Hex(string html, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + html, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
