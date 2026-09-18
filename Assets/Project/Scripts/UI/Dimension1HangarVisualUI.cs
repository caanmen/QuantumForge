using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Dimension1HangarVisualUI : MonoBehaviour
{
    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private Dimension1CommandCenterUI commandCenter;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject[] hideWhileOpen;
    [SerializeField] private TMP_Text[] metalAmounts;
    [SerializeField] private TMP_Text[] metalRates;
    [SerializeField] private Button[] shipButtons;
    [SerializeField] private Image[] shipBorders;
    [SerializeField] private Image[] shipFills;
    [SerializeField] private GameObject[] shipArtRoots;
    [SerializeField] private GameObject[] largeShipArtRoots;
    [SerializeField] private TMP_Text[] shipLabels;
    [SerializeField] private Button[] partButtons;
    [SerializeField] private Image[] partBorders;
    [SerializeField] private Image[] partFills;
    [SerializeField] private GameObject[] partIconRoots;
    [SerializeField] private TMP_Text[] partLabels;
    [SerializeField] private TMP_Text[] partLevels;
    [SerializeField] private TMP_Text[] partValues;
    [SerializeField] private Image[] partBars;
    [SerializeField] private TMP_Text[] costNames;
    [SerializeField] private TMP_Text[] costValues;
    [SerializeField] private TMP_Text upgradeButtonLabel;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text missionBonus;
    [SerializeField] private bool referencePreviewForVisualQa;

    private static readonly string[] ShipIds =
    {
        Dimension1System.ShipLightProbe,
        Dimension1System.ShipExtractorDrone,
        Dimension1System.ShipAnalyticProbe,
        Dimension1System.ShipCargoShip
    };

    private static readonly string[] PartIds =
    {
        Dimension1System.ShipPartCargo,
        Dimension1System.ShipPartSpeed,
        Dimension1System.ShipPartArmor,
        Dimension1System.ShipPartSensors
    };

    private static readonly string[] PartNames = { "CARGA", "VELOCIDAD", "BLINDAJE", "SENSORES" };
    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9BA9B3");
    private static readonly Color Fill = Hex("04121B", 248);
    private static readonly Color FillSelected = Hex("1A1508", 252);

    private int selectedShipIndex;
    private int selectedPartIndex = 1;
    private float refreshTimer;
    private bool[] hiddenPreviousStates;
    private VerticalNavigationUI verticalNavigation;

    public void SetReferencePreviewForVisualQa(bool enabled)
    {
        referencePreviewForVisualQa = enabled;
        selectedShipIndex = 0;
        selectedPartIndex = 1;
        Refresh();
    }

    public void OpenCommandCenter()
    {
        if (panel != null) panel.OnClickCloseHangarPanel();
        if (commandCenter != null) commandCenter.ShowCommandCenterScreen();
    }

    public void OpenGalaxy()
    {
        if (panel == null) return;
        panel.OnClickCloseHangarPanel();
        panel.OnClickOpenGalaxyPanel();
    }

    public void OpenExplore()
    {
        if (panel != null) panel.OnClickCloseHangarPanel();
        if (commandCenter != null) commandCenter.ShowExploreScreen();
    }

    public void OpenRelics()
    {
        if (panel == null) return;
        panel.OnClickCloseHangarPanel();
        panel.OnClickOpenRelicChamberPanel();
    }

    public void OpenTree()
    {
        if (panel == null) return;
        panel.OnClickCloseHangarPanel();
        panel.OnClickOpenDimension1TreePanel();
    }

    public void SelectShip0() => SelectShip(0);
    public void SelectShip1() => SelectShip(1);
    public void SelectShip2() => SelectShip(2);
    public void SelectShip3() => SelectShip(3);
    public void SelectCargo() => SelectPart(0);
    public void SelectSpeed() => SelectPart(1);
    public void SelectArmor() => SelectPart(2);
    public void SelectSensors() => SelectPart(3);

    public void UpgradeSelected()
    {
        if (panel == null || referencePreviewForVisualQa) return;
        GameState state = GameState.I;
        D1ShipState selected = FindShip(state, ShipIds[selectedShipIndex]);
        if (selected == null) return;
        if (!selected.unlocked)
        {
            panel.TryUnlockHangarShipForUi(selected.shipId);
            refreshTimer = 0f;
            Refresh();
            return;
        }
        switch (selectedPartIndex)
        {
            case 0: panel.OnClickUpgradeHangarShipCargo(); break;
            case 1: panel.OnClickUpgradeHangarShipSpeed(); break;
            case 2: panel.OnClickUpgradeHangarShipArmor(); break;
            case 3: panel.OnClickUpgradeHangarShipSensors(); break;
        }
        refreshTimer = 0f;
        Refresh();
    }

    private void OnEnable()
    {
        // Si Hangar se abrió desde Explorar, cerrar primero su raíz visual evita que
        // ambas pantallas conserven simultáneamente la autoridad sobre la navegación.
        if (commandCenter != null) commandCenter.HideExploreScreenForSecondaryUi();
        if (verticalNavigation == null)
            verticalNavigation = FindFirstObjectByType<VerticalNavigationUI>(FindObjectsInactive.Include);
        if (verticalNavigation != null) verticalNavigation.SetNavigationSuppressed(true, this);
        ApplyExclusiveNavigation(true);
        selectedShipIndex = Mathf.Clamp(selectedShipIndex, 0, ShipIds.Length - 1);
        selectedPartIndex = Mathf.Clamp(selectedPartIndex, 0, PartIds.Length - 1);
        refreshTimer = 0f;
        Refresh();
    }

    private void OnDisable()
    {
        ApplyExclusiveNavigation(false);
        if (verticalNavigation != null) verticalNavigation.SetNavigationSuppressed(false, this);
    }

    private void Update()
    {
        if (hideWhileOpen != null)
            foreach (GameObject target in hideWhileOpen)
                if (target != null && target.activeSelf) target.SetActive(false);

        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = 0.25f;
        Refresh();
    }

    private void SelectShip(int index)
    {
        if (index < 0 || index >= ShipIds.Length) return;
        selectedShipIndex = index;
        if (!referencePreviewForVisualQa && panel != null)
            panel.OnHangarShipDropdownChanged(index);
        Refresh();
    }

    private void SelectPart(int index)
    {
        if (index < 0 || index >= PartIds.Length) return;
        selectedPartIndex = index;
        Refresh();
    }

    private void Refresh()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (referencePreviewForVisualQa)
        {
            ApplyReferencePreview();
            return;
        }

        GameState state = GameState.I;
        if (state == null) return;
        state.EnsureDimension1State();
        Dimension1HeaderMetalsUI.Refresh(transform, state, state.dimension1SelectedSectorId);

        bool[] unlocked = new bool[ShipIds.Length];
        for (int i = 0; i < ShipIds.Length; i++)
        {
            D1ShipState ship = FindShip(state, ShipIds[i]);
            unlocked[i] = ship != null && ship.unlocked;
        }
        RefreshShipCards(unlocked);

        D1ShipState selected = FindShip(state, ShipIds[selectedShipIndex]);
        int[] levels =
        {
            selected == null ? 0 : selected.cargoLevel,
            selected == null ? 0 : selected.speedLevel,
            selected == null ? 0 : selected.armorLevel,
            selected == null ? 0 : selected.sensorsLevel
        };
        for (int i = 0; i < PartIds.Length; i++)
        {
            int level = Mathf.Clamp(levels[i], 0, Dimension1System.Dimension1ShipPartMaxLevel);
            Set(partLevels, i, "NIVEL " + level + "/" + Dimension1System.Dimension1ShipPartMaxLevel);
            Set(partValues, i, BuildPartValue(ShipIds[selectedShipIndex], i, level));
            SetBar(i, level / (float)Dimension1System.Dimension1ShipPartMaxLevel);
        }
        RefreshPartCards();
        RefreshUpgrade(state, selected);
    }

    private void ApplyReferencePreview()
    {
        string[] amounts = { "5.98M", "4.73M", "4.70M" };
        string[] rates = { "+0.30/s", "+0.08/s", "+0.03/s" };
        for (int i = 0; i < 3; i++)
        {
            Set(metalAmounts, i, amounts[i]);
            Set(metalRates, i, rates[i]);
        }
        RefreshShipCards(new[] { true, true, true, true });
        int[] previewLevels = { 2, 2, 1, 2 };
        float[] bars = { .53f, .62f, .48f, .55f };
        for (int i = 0; i < 4; i++)
        {
            Set(partLevels, i, "NIVEL " + previewLevels[i] + "/4");
            Set(partValues, i, BuildPartValue(
                Dimension1System.ShipLightProbe,
                i,
                previewLevels[i]
            ));
            SetBar(i, bars[i]);
        }
        RefreshPartCards();
        Set(costNames, 0, "HIERRO");
        Set(costValues, 0, "45K");
        Set(costNames, 1, "MATRIZ");
        Set(costValues, 1, "12");
        if (upgradeButtonLabel != null)
            upgradeButtonLabel.text = "MEJORAR\n" + PartNames[selectedPartIndex];
        if (upgradeButton != null) upgradeButton.interactable = true;
        if (missionBonus != null)
            missionBonus.text = BuildPartEffectDescription(
                Dimension1System.ShipLightProbe,
                selectedPartIndex,
                previewLevels[selectedPartIndex]
            );
    }

    private void RefreshShipCards(bool[] unlocked)
    {
        for (int i = 0; i < ShipIds.Length; i++)
        {
            bool selected = i == selectedShipIndex;
            bool available = unlocked != null && i < unlocked.Length && unlocked[i];
            if (shipBorders != null && i < shipBorders.Length && shipBorders[i] != null)
                shipBorders[i].color = selected ? Amber : available ? CyanMuted : Hex("506471");
            if (shipFills != null && i < shipFills.Length && shipFills[i] != null)
                shipFills[i].color = selected ? FillSelected : Fill;
            if (shipLabels != null && i < shipLabels.Length && shipLabels[i] != null)
                shipLabels[i].color = selected ? Amber : available ? Cyan : Secondary;
            if (shipArtRoots != null && i < shipArtRoots.Length)
                TintGraphics(shipArtRoots[i], selected ? Amber : available ? Cyan : Secondary, 1f);
            if (largeShipArtRoots != null && i < largeShipArtRoots.Length && largeShipArtRoots[i] != null)
                largeShipArtRoots[i].SetActive(selected);
            if (shipButtons != null && i < shipButtons.Length && shipButtons[i] != null)
                shipButtons[i].interactable = true;
        }
    }

    private void RefreshPartCards()
    {
        for (int i = 0; i < PartIds.Length; i++)
        {
            bool selected = i == selectedPartIndex;
            if (partBorders != null && i < partBorders.Length && partBorders[i] != null)
                partBorders[i].color = selected ? Amber : CyanMuted;
            if (partFills != null && i < partFills.Length && partFills[i] != null)
                partFills[i].color = selected ? FillSelected : Fill;
            if (partLabels != null && i < partLabels.Length && partLabels[i] != null)
                partLabels[i].color = selected ? Amber : Cyan;
            if (partIconRoots != null && i < partIconRoots.Length)
                TintGraphics(partIconRoots[i], selected ? Amber : Cyan, 1f);
            if (partBars != null && i < partBars.Length && partBars[i] != null)
                partBars[i].color = selected ? Amber : Hex("3BAED9");
        }
    }

    private void RefreshUpgrade(GameState state, D1ShipState selected)
    {
        string shipId = ShipIds[selectedShipIndex];
        string partId = PartIds[selectedPartIndex];
        if (upgradeButtonLabel != null) upgradeButtonLabel.text = "MEJORAR\n" + PartNames[selectedPartIndex];
        if (selected == null || !selected.unlocked)
        {
            RefreshUnlock(state, shipId);
            return;
        }

        if (Dimension1System.TryGetNextShipPartUpgradeCost(state, shipId, partId,
            out _, out string metal1, out double amount1, out string metal2, out double amount2))
        {
            SetCost(MetalName(metal1), FormatAmount(amount1),
                string.IsNullOrEmpty(metal2) ? "SIN COSTE EXTRA" : MetalName(metal2),
                string.IsNullOrEmpty(metal2) ? "—" : FormatAmount(amount2));
            if (upgradeButton != null) upgradeButton.interactable = Dimension1System.CanUpgradeShipPart(state, shipId, partId);
        }
        else if (Dimension1System.TryGetNextAdvancedShipPartUpgradeCost(state, shipId, partId,
            out int nextLevel, out string advancedMetal1, out double advancedAmount1,
            out string advancedMetal2, out double advancedAmount2, out int blueprintCost))
        {
            int specific = Dimension1System.GetRequiredSpecificShipUpgradeMatrixCost(shipId, nextLevel);
            string secondName = MetalName(advancedMetal2);
            string secondValue = FormatAmount(advancedAmount2);
            int matrices = specific > 0 ? specific : blueprintCost;
            if (matrices > 0)
            {
                secondName = string.IsNullOrEmpty(advancedMetal2) ? "MATRIZ" : secondName + " + MATRIZ";
                secondValue = string.IsNullOrEmpty(advancedMetal2) ? matrices.ToString() : secondValue + " + " + matrices;
            }
            SetCost(MetalName(advancedMetal1), FormatAmount(advancedAmount1), secondName, secondValue);
            if (upgradeButton != null) upgradeButton.interactable = Dimension1System.CanUpgradeAdvancedShipPart(state, shipId, partId);
        }
        else
        {
            SetCost("NIVEL", "MÁXIMO", "COSTE", "—");
            if (upgradeButtonLabel != null) upgradeButtonLabel.text = "NIVEL\nMÁXIMO";
            if (upgradeButton != null) upgradeButton.interactable = false;
        }

        int currentLevel = GetPartLevel(selected, selectedPartIndex);
        SetMissionBonus(BuildPartEffectDescription(
            shipId,
            selectedPartIndex,
            currentLevel));
    }

    private void RefreshUnlock(GameState state, string shipId)
    {
        if (upgradeButtonLabel != null) upgradeButtonLabel.text = "DESBLOQUEAR\nNAVE";
        if (!Dimension1System.TryGetShipUnlockCost(
            shipId,
            out string metal1, out double amount1,
            out string metal2, out double amount2,
            out string metal3, out double amount3,
            out string metal4, out double amount4,
            out int matrixCost))
        {
            SetCost("NAVE", "NO DISPONIBLE", "REQUISITOS", "—");
            if (upgradeButton != null) upgradeButton.interactable = false;
            SetMissionBonus("NAVE FUERA DE LA FLOTA ACTIVA");
            return;
        }

        string secondName = MetalName(metal2);
        string secondValue = FormatAmount(amount2);
        if (!string.IsNullOrEmpty(metal3))
        {
            secondName += " + " + MetalName(metal3);
            secondValue += " + " + FormatAmount(amount3);
        }
        if (!string.IsNullOrEmpty(metal4))
        {
            secondName += " + " + MetalName(metal4);
            secondValue += " + " + FormatAmount(amount4);
        }
        if (matrixCost > 0)
        {
            secondName += " + MATRIZ";
            secondValue += " + " + matrixCost;
        }
        SetCost(MetalName(metal1), FormatAmount(amount1), secondName, secondValue);
        if (upgradeButton != null)
            upgradeButton.interactable = Dimension1System.CanUnlockShip(state, shipId);

        if (Dimension1System.UsesSpecificShipMatricesForUnlock(shipId) && matrixCost > 0)
        {
            int owned = Dimension1System.GetOwnedRequiredSpecificShipMatrixCount(state, shipId);
            SetMissionBonus("MATRICES DE NAVE: " + owned + " / " + matrixCost);
        }
        else if (matrixCost > 0)
        {
            SetMissionBonus("MATRICES COMPLETADAS REQUERIDAS: " + matrixCost);
        }
        else
        {
            SetMissionBonus("REÚNE LOS METALES PARA CONSTRUIR ESTA NAVE");
        }
    }

    private void SetCost(string name1, string value1, string name2, string value2)
    {
        Set(costNames, 0, name1);
        Set(costValues, 0, value1);
        Set(costNames, 1, name2);
        Set(costValues, 1, value2);
    }

    private void SetMissionBonus(string value)
    {
        if (missionBonus != null) missionBonus.text = value;
    }

    private static int GetPartLevel(D1ShipState ship, int partIndex)
    {
        if (ship == null) return 0;
        switch (partIndex)
        {
            case 0: return ship.cargoLevel;
            case 1: return ship.speedLevel;
            case 2: return ship.armorLevel;
            default: return ship.sensorsLevel;
        }
    }

    private static string BuildPartValue(string shipId, int partIndex, int level)
    {
        switch (partIndex)
        {
            case 0:
                return "HASTA ×" + FormatEffect(MaxCargoMultiplier(shipId, level));
            case 1:
                return "TIEMPO ×" + FormatEffect(
                    Dimension1System.GetShipSpeedDurationMultiplierPreview(level));
            case 2:
                return "METALES +" + FormatEffect(
                    Mathf.Max(0f, (float)((MaxArmorMultiplier(shipId, level) - 1d) * 100d))) + "%";
            default:
                return "FRAGMENTOS +" + FormatEffect(
                    MaxSensorBonus(shipId, level) * 100f) + "%";
        }
    }

    private static string BuildPartEffectDescription(
        string shipId,
        int partIndex,
        int currentLevel)
    {
        int maxLevel = Dimension1System.Dimension1ShipPartMaxLevel;
        int safeLevel = Mathf.Clamp(currentLevel, 0, maxLevel);
        int nextLevel = Mathf.Min(maxLevel, safeLevel + 1);
        string separator = safeLevel < maxLevel ? " → " : " · MÁXIMO";

        switch (partIndex)
        {
            case 0:
                return "MATERIALES: HASTA ×" + FormatEffect(MaxCargoMultiplier(shipId, safeLevel)) +
                    (safeLevel < maxLevel
                        ? separator + "×" + FormatEffect(MaxCargoMultiplier(shipId, nextLevel))
                        : separator) + " SEGÚN DESTINO";
            case 1:
                return "TIEMPO: ×" + FormatEffect(
                    Dimension1System.GetShipSpeedDurationMultiplierPreview(safeLevel)) +
                    (safeLevel < maxLevel
                        ? separator + "×" + FormatEffect(
                            Dimension1System.GetShipSpeedDurationMultiplierPreview(nextLevel))
                        : separator) + " EN RUTAS COMPATIBLES";
            case 2:
                float currentArmor = Mathf.Max(0f,
                    (float)((MaxArmorMultiplier(shipId, safeLevel) - 1d) * 100d));
                float nextArmor = Mathf.Max(0f,
                    (float)((MaxArmorMultiplier(shipId, nextLevel) - 1d) * 100d));
                return "METALES OBTENIDOS: HASTA +" + FormatEffect(currentArmor) + "%" +
                    (safeLevel < maxLevel
                        ? separator + "+" + FormatEffect(nextArmor) + "%"
                        : separator) + " SEGÚN DESTINO";
            default:
                string sensorText = "PROB. DE FRAGMENTO: +" +
                    FormatEffect(MaxSensorBonus(shipId, safeLevel) * 100f) + "%" +
                    (safeLevel < maxLevel
                        ? separator + "+" +
                            FormatEffect(MaxSensorBonus(shipId, nextLevel) * 100f) + "%"
                        : separator);
                sensorText += " · MATRIZ ESPECÍFICA: +" +
                    FormatEffect(MaxSpecificSensorBonus(shipId, safeLevel) * 100f) + "%";
                if (safeLevel < maxLevel)
                    sensorText += " → +" +
                        FormatEffect(MaxSpecificSensorBonus(shipId, nextLevel) * 100f) + "%";
                if (shipId == Dimension1System.ShipAnalyticProbe)
                {
                    sensorText += " · BARRIDO: " +
                        Dimension1System.GetAnalyticProbeScanDurationPreviewSeconds(safeLevel).ToString("0.0") + "s";
                    if (safeLevel < maxLevel)
                        sensorText += " → " +
                            Dimension1System.GetAnalyticProbeScanDurationPreviewSeconds(nextLevel).ToString("0.0") + "s";
                }
                return sensorText;
        }
    }

    private static double MaxCargoMultiplier(string shipId, int level)
    {
        double maximum = 1d;
        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
            foreach (string destinationId in Dimension1System.GetDimension1SectorDestinationIds(sectorId))
                maximum = System.Math.Max(maximum,
                    Dimension1System.GetShipCargoRewardMultiplierPreview(
                        shipId, destinationId, level));
        return maximum;
    }

    private static double MaxArmorMultiplier(string shipId, int level)
    {
        double maximum = 1d;
        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
            foreach (string destinationId in Dimension1System.GetDimension1SectorDestinationIds(sectorId))
                maximum = System.Math.Max(maximum,
                    Dimension1System.GetShipArmorRewardMultiplierPreview(
                        shipId, destinationId, level));
        return maximum;
    }

    private static float MaxSensorBonus(string shipId, int level)
    {
        float maximum = 0f;
        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
            foreach (string destinationId in Dimension1System.GetDimension1SectorDestinationIds(sectorId))
                maximum = Mathf.Max(maximum,
                    Dimension1System.GetShipSensorBlueprintFragmentBonusPreview(
                        shipId, destinationId, level));
        return maximum;
    }

    private static float MaxSpecificSensorBonus(string shipId, int level)
    {
        float maximum = 0f;
        foreach (string sectorId in Dimension1System.Dimension1SectorIds)
            foreach (string destinationId in Dimension1System.GetDimension1SectorDestinationIds(sectorId))
                maximum = Mathf.Max(maximum,
                    Dimension1System.GetShipSensorSpecificBlueprintBonusPreview(
                        shipId, destinationId, level));
        return maximum;
    }

    private static string FormatEffect(double value)
    {
        return value.ToString("0.##");
    }

    private void SetBar(int index, float amount)
    {
        if (partBars == null || index < 0 || index >= partBars.Length || partBars[index] == null) return;
        RectTransform rect = partBars[index].rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = new Vector2(Mathf.Clamp01(amount), 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void ApplyExclusiveNavigation(bool hide)
    {
        if (hideWhileOpen == null) return;
        if (hide)
        {
            hiddenPreviousStates = new bool[hideWhileOpen.Length];
            for (int i = 0; i < hideWhileOpen.Length; i++)
            {
                GameObject target = hideWhileOpen[i];
                if (target == null) continue;
                hiddenPreviousStates[i] = target.activeSelf;
                target.SetActive(false);
            }
            return;
        }
        if (hiddenPreviousStates == null) return;
        for (int i = 0; i < hideWhileOpen.Length && i < hiddenPreviousStates.Length; i++)
            if (hideWhileOpen[i] != null) hideWhileOpen[i].SetActive(hiddenPreviousStates[i]);
        hiddenPreviousStates = null;
    }

    private static D1ShipState FindShip(GameState state, string id)
    {
        if (state == null || state.dimension1Ships == null) return null;
        foreach (D1ShipState ship in state.dimension1Ships)
            if (ship != null && ship.shipId == id) return ship;
        return null;
    }

    private static void TintGraphics(GameObject root, Color tint, float alphaMultiplier)
    {
        if (root == null) return;
        foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(true))
        {
            Color current = graphic.color;
            graphic.color = new Color(tint.r, tint.g, tint.b, current.a * alphaMultiplier);
        }
    }

    private static string MetalName(string id)
    {
        if (id == Dimension1System.MetalIron) return "HIERRO";
        if (id == Dimension1System.MetalAluminum) return "ALUMINIO";
        if (id == Dimension1System.MetalNickel) return "NÍQUEL";
        if (id == Dimension1System.MetalCopper) return "COBRE";
        if (id == Dimension1System.MetalTitanium) return "TITANIO";
        if (id == Dimension1System.MetalCobalt) return "COBALTO";
        if (id == Dimension1System.MetalPlatinum) return "PLATINO";
        if (id == Dimension1System.MetalTungsten) return "TUNGSTENO";
        if (id == Dimension1System.MetalIridium) return "IRIDIO";
        return string.IsNullOrEmpty(id) ? "RECURSO" : id.ToUpperInvariant();
    }

    private static string FormatAmount(double value)
    {
        value = System.Math.Max(0d, value);
        if (value >= 1000000000d) return (value / 1000000000d).ToString("0.##") + "B";
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        return value.ToString("0.##");
    }

    private static void Set(TMP_Text[] targets, int index, string value)
    {
        if (targets != null && index >= 0 && index < targets.Length && targets[index] != null)
            targets[index].text = value;
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
