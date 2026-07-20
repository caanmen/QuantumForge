using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D2VeiledThresholdPanelUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text revelationText;
    public TMP_Text placeText;
    public TMP_Text pendingText;
    public TMP_Text resourcesText;
    public TMP_Text acolytesText;
    public TMP_Dropdown lineDropdown;
    public TMP_Text lineText;
    public Button prepareButton;
    public Button assignAcolyteButton;
    public Button releaseAcolyteButton;
    public Button upgradeLineButton;

    private float _refreshTimer;

    private void Awake()
    {
        if (prepareButton != null) prepareButton.onClick.AddListener(Prepare);
        if (assignAcolyteButton != null) assignAcolyteButton.onClick.AddListener(AssignAcolyte);
        if (releaseAcolyteButton != null) releaseAcolyteButton.onClick.AddListener(ReleaseAcolyte);
        if (upgradeLineButton != null) upgradeLineButton.onClick.AddListener(UpgradeLine);
        if (lineDropdown != null)
        {
            lineDropdown.ClearOptions();
            var options = new List<string>();
            foreach (string id in D2BondSystem.LineIds)
                options.Add(D2BondSystem.GetDisplayName(id));
            lineDropdown.AddOptions(options);
            lineDropdown.onValueChanged.AddListener(_ => Refresh());
        }
    }

    private void OnEnable()
    {
        _refreshTimer = 0f;
        Refresh();
    }

    private void Update()
    {
        _refreshTimer -= Time.unscaledDeltaTime;
        if (_refreshTimer > 0f) return;
        _refreshTimer = 0.2f;
        Refresh();
    }

    public void Refresh()
    {
        GameState gameState = GameState.I;
        if (!Dimension2System.CanAccessDimension2(gameState)) return;

        gameState.EnsureDimension2State();
        D2Civilization1State state = gameState.dimension2.civilization1;
        bool unlocked = D2VeiledThresholdSystem.IsUnlocked(state);
        bool prepared = state.bondPlacePrepared;

        SetText(titleText, unlocked ? "UMBRAL VELADO" : "ALGO PERMANECE EN SILENCIO");
        SetText(revelationText, unlocked ? "ALGO RESPONDE" :
            "Confianza: " + state.trust.ToString("0.##") + "/" +
            D2VeiledThresholdSystem.UnlockTrustRequired.ToString("0"));
        SetText(placeText, unlocked
            ? prepared ? "El Lugar de Vínculo está preparado."
                : "La civilización ha permitido preparar el Lugar de Vínculo."
            : "El Lugar de Vínculo todavía no se ha manifestado.");

        D2AltarState incense = D2AltarSystem.GetAltar(state, D2AltarSystem.IncenseAltarId);
        D2AltarState cloth = D2AltarSystem.GetAltar(state, D2AltarSystem.SacredClothAltarId);
        D2AltarState stone = D2AltarSystem.GetAltar(state, D2AltarSystem.CarvedStoneAltarId);
        SetText(resourcesText, unlocked
            ? "Incienso: " + (incense?.offeringAmount ?? 0.0).ToString("0.##") +
              " | Tela sagrada: " + (cloth?.offeringAmount ?? 0.0).ToString("0.##") +
              " | Piedra tallada: " + (stone?.offeringAmount ?? 0.0).ToString("0.##") +
              " | Progreso: " + state.bondProgress.ToString("0.##")
            : "Recursos del vínculo ocultos.");
        SetText(acolytesText, prepared
            ? "Acólitos disponibles: " + state.acolytesAvailable.ToString("N0") +
              " | Asignados: " + state.acolytesAssignedToBond.ToString("N0")
            : "Los Acólitos podrán sostener el vínculo después de prepararlo.");

        string lineId = GetSelectedLineId();
        int level = D2BondSystem.GetLevel(state, lineId);
        int nextLevel = Mathf.Min(level + 1, D2BondSystem.MaxLineLevel);
        SetText(lineText, prepared
            ? D2BondSystem.GetDisplayName(lineId) + " — Nivel " + level + "/3\n" +
              D2BondSystem.GetEffectDescription(lineId) +
              (level < D2BondSystem.MaxLineLevel
                  ? "\nSiguiente: " + D2BondSystem.GetProgressCost(nextLevel).ToString("0") +
                    " progreso y " + D2BondSystem.GetOfferingCost(nextLevel).ToString("0") +
                    " de cada Ofrenda avanzada."
                  : "\nNivel máximo alcanzado.")
            : "Las cinco líneas se revelarán al preparar el Lugar de Vínculo.");
        SetText(pendingText, string.IsNullOrEmpty(state.lastBondResult)
            ? unlocked ? "Los costes y porcentajes son provisionales para pruebas."
                : "Se manifestará al alcanzar 500 de Confianza."
            : state.lastBondResult);

        SetActive(lineDropdown, prepared);
        SetInteractable(prepareButton, D2BondSystem.CanPrepare(gameState));
        SetInteractable(assignAcolyteButton, prepared && state.acolytesAvailable > 0L);
        SetInteractable(releaseAcolyteButton, prepared && state.acolytesAssignedToBond > 0L);
        SetInteractable(upgradeLineButton, D2BondSystem.CanUpgrade(gameState, lineId));
    }

    private string GetSelectedLineId()
    {
        int index = lineDropdown != null ? lineDropdown.value : 0;
        return D2BondSystem.LineIds[Mathf.Clamp(index, 0, D2BondSystem.LineIds.Length - 1)];
    }

    private void Prepare() { D2BondSystem.TryPrepare(GameState.I); RefreshAll(); }
    private void AssignAcolyte() { D2BondSystem.TryAssignAcolytes(GameState.I, 1L); RefreshAll(); }
    private void ReleaseAcolyte() { D2BondSystem.TryReleaseAcolytes(GameState.I, 1L); RefreshAll(); }
    private void UpgradeLine() { D2BondSystem.TryUpgrade(GameState.I, GetSelectedLineId()); RefreshAll(); }

    private void RefreshAll()
    {
        D2Civilization1PanelUI parent = GetComponentInParent<D2Civilization1PanelUI>(true);
        if (parent != null) parent.Refresh(); else Refresh();
    }

    private static void SetText(TMP_Text text, string value) { if (text != null) text.text = value; }
    private static void SetInteractable(Button button, bool value) { if (button != null) button.interactable = value; }
    private static void SetActive(Component component, bool value) { if (component != null) component.gameObject.SetActive(value); }
}
