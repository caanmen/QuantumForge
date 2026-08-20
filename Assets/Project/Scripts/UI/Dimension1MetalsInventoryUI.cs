using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Subpantalla compartida del inventario de metales de Dimensión 1.
/// Se mantiene activa y oculta mediante CanvasGroup para poder enlazar todos
/// los accesos del encabezado sin alterar el estado de la pantalla de origen.
/// </summary>
public sealed class Dimension1MetalsInventoryUI : MonoBehaviour
{
    public static Dimension1MetalsInventoryUI I { get; private set; }

    private static readonly string[] MetalIds =
    {
        Dimension1System.MetalIron,
        Dimension1System.MetalCopper,
        Dimension1System.MetalAluminum,
        Dimension1System.MetalTitanium,
        Dimension1System.MetalNickel,
        Dimension1System.MetalCobalt,
        Dimension1System.MetalLithium,
        Dimension1System.MetalTungsten,
        Dimension1System.MetalPlatinum,
        Dimension1System.MetalIridium
    };

    private static readonly string[] MetalNames =
    {
        "HIERRO", "COBRE", "ALUMINIO", "TITANIO", "NÍQUEL",
        "COBALTO", "LITIO", "TUNGSTENO", "PLATINO", "IRIDIO"
    };

    private static readonly Color Cyan = Hex("18C8FF");
    private static readonly Color CyanMuted = Hex("087FA9");
    private static readonly Color Amber = Hex("F4A70B");
    private static readonly Color AmberFill = Hex("1A1508", 252);
    private static readonly Color NormalFill = Hex("04121B", 248);
    private static readonly Color Primary = Hex("EDF4F7");
    private static readonly Color Secondary = Hex("9EABB4");
    private static readonly Color Locked = Hex("687680");

    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button backButton;
    [SerializeField] private Button[] filterButtons;
    [SerializeField] private Image[] filterBorders;
    [SerializeField] private Image[] filterFills;
    [SerializeField] private TMP_Text[] filterLabels;

    [SerializeField] private TMP_Text totalMetalsText;
    [SerializeField] private TMP_Text unlockedMetalsText;
    [SerializeField] private TMP_Text totalProductionText;
    [SerializeField] private TMP_Text highestMetalText;
    [SerializeField] private TMP_Text highestProductionText;

    [SerializeField] private RectTransform[] metalCards;
    [SerializeField] private Image[] metalBorders;
    [SerializeField] private Image[] metalIcons;
    [SerializeField] private TMP_Text[] metalNameTexts;
    [SerializeField] private TMP_Text[] metalAmountTexts;
    [SerializeField] private TMP_Text[] metalRateTexts;
    [SerializeField] private TMP_Text[] metalSourceTexts;
    [SerializeField] private TMP_Text[] metalLockedTexts;
    [SerializeField] private TMP_Text[] metalRequirementTexts;

    private readonly List<Button> entryButtons = new List<Button>();
    private int filterIndex;
    private float refreshTimer;
    private bool visible;
    private bool localListenersBound;

    public void Configure(
        Dimension1PanelUI configuredPanel,
        CanvasGroup configuredCanvasGroup,
        Button configuredBackButton,
        Button[] configuredFilterButtons,
        Image[] configuredFilterBorders,
        Image[] configuredFilterFills,
        TMP_Text[] configuredFilterLabels,
        TMP_Text configuredTotalMetals,
        TMP_Text configuredUnlockedMetals,
        TMP_Text configuredTotalProduction,
        TMP_Text configuredHighestMetal,
        TMP_Text configuredHighestProduction,
        RectTransform[] configuredCards,
        Image[] configuredBorders,
        Image[] configuredIcons,
        TMP_Text[] configuredNames,
        TMP_Text[] configuredAmounts,
        TMP_Text[] configuredRates,
        TMP_Text[] configuredSources,
        TMP_Text[] configuredLocked,
        TMP_Text[] configuredRequirements)
    {
        panel = configuredPanel;
        canvasGroup = configuredCanvasGroup;
        backButton = configuredBackButton;
        filterButtons = configuredFilterButtons;
        filterBorders = configuredFilterBorders;
        filterFills = configuredFilterFills;
        filterLabels = configuredFilterLabels;
        totalMetalsText = configuredTotalMetals;
        unlockedMetalsText = configuredUnlockedMetals;
        totalProductionText = configuredTotalProduction;
        highestMetalText = configuredHighestMetal;
        highestProductionText = configuredHighestProduction;
        metalCards = configuredCards;
        metalBorders = configuredBorders;
        metalIcons = configuredIcons;
        metalNameTexts = configuredNames;
        metalAmountTexts = configuredAmounts;
        metalRateTexts = configuredRates;
        metalSourceTexts = configuredSources;
        metalLockedTexts = configuredLocked;
        metalRequirementTexts = configuredRequirements;
    }

    private void Awake()
    {
        I = this;
        SetVisible(false);
    }

    private void Start()
    {
        BindLocalListeners();
        BindEntryButtons();
        RefreshData();
    }

    private void OnDestroy()
    {
        UnbindLocalListeners();
        foreach (Button button in entryButtons)
            if (button != null) button.onClick.RemoveListener(Open);
        entryButtons.Clear();
        if (I == this) I = null;
    }

    private void Update()
    {
        if (!visible) return;
        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer <= 0f)
        {
            refreshTimer = .25f;
            RefreshData();
        }
    }

    public void Open()
    {
        filterIndex = 0;
        transform.SetAsLastSibling();
        SetVisible(true);
        RefreshData();
    }

    public void Close()
    {
        SetVisible(false);
    }

    public void SelectAll() { SetFilter(0); }
    public void SelectProducing() { SetFilter(1); }
    public void SelectLocked() { SetFilter(2); }

    private void SetFilter(int index)
    {
        filterIndex = Mathf.Clamp(index, 0, 2);
        RefreshData();
    }

    private void SetVisible(bool value)
    {
        visible = value;
        if (canvasGroup == null) return;
        canvasGroup.alpha = value ? 1f : 0f;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }

    private void BindLocalListeners()
    {
        if (localListenersBound) return;
        localListenersBound = true;
        if (backButton != null) backButton.onClick.AddListener(Close);
        if (filterButtons != null && filterButtons.Length >= 3)
        {
            if (filterButtons[0] != null) filterButtons[0].onClick.AddListener(SelectAll);
            if (filterButtons[1] != null) filterButtons[1].onClick.AddListener(SelectProducing);
            if (filterButtons[2] != null) filterButtons[2].onClick.AddListener(SelectLocked);
        }
    }

    private void UnbindLocalListeners()
    {
        if (!localListenersBound) return;
        localListenersBound = false;
        if (backButton != null) backButton.onClick.RemoveListener(Close);
        if (filterButtons != null && filterButtons.Length >= 3)
        {
            if (filterButtons[0] != null) filterButtons[0].onClick.RemoveListener(SelectAll);
            if (filterButtons[1] != null) filterButtons[1].onClick.RemoveListener(SelectProducing);
            if (filterButtons[2] != null) filterButtons[2].onClick.RemoveListener(SelectLocked);
        }
    }

    private void BindEntryButtons()
    {
        Transform owner = panel != null ? panel.transform : transform.parent;
        if (owner == null) return;
        foreach (Button button in owner.GetComponentsInChildren<Button>(true))
        {
            if (button == null || button == backButton) continue;
            string objectName = button.transform.name;
            if (objectName != "MetalsButton" && objectName != "AllMetals") continue;
            button.onClick.RemoveListener(Open);
            button.onClick.AddListener(Open);
            entryButtons.Add(button);
        }
    }

    private void RefreshData()
    {
        RefreshFilterAppearance();
        GameState state = GameState.I;
        if (state == null) return;
        state.EnsureDimension1State();

        int unlockedCount = 0;
        double totalProduction = 0d;
        double highestProduction = -1d;
        int highestIndex = 0;
        int visibleIndex = 0;

        for (int i = 0; i < MetalIds.Length; i++)
        {
            string metalId = MetalIds[i];
            bool unlocked = Dimension1System.IsMetalUnlockedForDimension1(state, metalId);
            double production = Dimension1System.GetMetalProductionPerSecond(state, metalId);
            if (unlocked) unlockedCount++;
            totalProduction += production;
            if (production > highestProduction)
            {
                highestProduction = production;
                highestIndex = i;
            }

            bool show = filterIndex == 0 || filterIndex == 1 && unlocked && production > 0d ||
                        filterIndex == 2 && !unlocked;
            if (metalCards != null && i < metalCards.Length && metalCards[i] != null)
            {
                metalCards[i].gameObject.SetActive(show);
                if (show)
                {
                    int column = visibleIndex % 2;
                    int row = visibleIndex / 2;
                    SetTop(metalCards[i], 26f + column * 524f, 400f + row * 256f, 504f, 236f);
                    visibleIndex++;
                }
            }

            Set(metalNameTexts, i, MetalNames[i]);
            Set(metalAmountTexts, i, FormatAmount(state.GetD1MetalAmount(metalId)));
            Set(metalRateTexts, i, "+" + FormatAmount(production) + "/s");
            Set(metalSourceTexts, i, GetSourceLabel(state, metalId));
            Set(metalLockedTexts, i, "BLOQUEADO");
            Set(metalRequirementTexts, i, GetRequirement(state, metalId));

            SetActive(metalAmountTexts, i, unlocked);
            SetActive(metalRateTexts, i, unlocked);
            SetActive(metalSourceTexts, i, unlocked);
            SetActive(metalLockedTexts, i, !unlocked);
            SetActive(metalRequirementTexts, i, !unlocked);
            if (metalBorders != null && i < metalBorders.Length && metalBorders[i] != null)
                metalBorders[i].color = unlocked ? CyanMuted : Hex("37505D", 190);
            if (metalIcons != null && i < metalIcons.Length && metalIcons[i] != null)
                metalIcons[i].color = unlocked ? Color.white : new Color(.42f, .48f, .52f, .72f);
            if (metalNameTexts != null && i < metalNameTexts.Length && metalNameTexts[i] != null)
                metalNameTexts[i].color = unlocked ? Secondary : Locked;
        }

        if (totalMetalsText != null)
            totalMetalsText.text = "<size=43><color=#EDF4F7>" + MetalIds.Length +
                "</color></size>\n<size=23><color=#9EABB4>METALES</color></size>";
        if (unlockedMetalsText != null)
            unlockedMetalsText.text = "<size=43><color=#EDF4F7>" + unlockedCount +
                "</color></size>\n<size=18><color=#9EABB4>DESBLOQUEADOS</color></size>";
        if (totalProductionText != null) totalProductionText.text = "+" + FormatAmount(totalProduction) + "/s";
        if (highestMetalText != null)
            highestMetalText.text = highestProduction > 0d ? MetalNames[highestIndex] : "SIN PRODUCCIÓN";
        if (highestProductionText != null)
            highestProductionText.text = "+" + FormatAmount(Math.Max(0d, highestProduction)) + "/s";
    }

    private void RefreshFilterAppearance()
    {
        for (int i = 0; i < 3; i++)
        {
            bool selected = i == filterIndex;
            if (filterBorders != null && i < filterBorders.Length && filterBorders[i] != null)
                filterBorders[i].color = selected ? Amber : CyanMuted;
            if (filterFills != null && i < filterFills.Length && filterFills[i] != null)
                filterFills[i].color = selected ? AmberFill : NormalFill;
            if (filterLabels != null && i < filterLabels.Length && filterLabels[i] != null)
                filterLabels[i].color = selected ? Amber : Cyan;
        }
    }

    private static string GetSourceLabel(GameState state, string metalId)
    {
        if (state.dimension1Planets != null)
        {
            foreach (D1PlanetState planet in state.dimension1Planets)
            {
                if (planet == null || !planet.unlocked) continue;
                if (Dimension1System.GetPlanetMetalProductionPerSecond(planet, metalId) > 0d)
                    return "PLANETA " + PlanetNumber(planet.planetId);
            }
        }
        return "PLANETA " + CanonicalPlanet(metalId);
    }

    private static string GetRequirement(GameState state, string metalId)
    {
        int planetNumber = CanonicalPlanet(metalId);
        D1PlanetState planet = FindPlanet(state, planetNumber);
        if (planet == null || !planet.unlocked)
            return "DESBLOQUEA PLANETA " + planetNumber;
        int extractorRequirement = IsSecondaryMetal(metalId) ? 10 : 1;
        return planet.extractorTier < extractorRequirement
            ? "EXTRACTOR NIVEL " + extractorRequirement
            : "REQUISITOS PENDIENTES";
    }

    private static D1PlanetState FindPlanet(GameState state, int number)
    {
        if (state == null || state.dimension1Planets == null) return null;
        string id = "planet_" + number.ToString("00");
        foreach (D1PlanetState planet in state.dimension1Planets)
            if (planet != null && planet.planetId == id) return planet;
        return null;
    }

    private static int CanonicalPlanet(string metalId)
    {
        if (metalId == Dimension1System.MetalIron || metalId == Dimension1System.MetalCopper) return 1;
        if (metalId == Dimension1System.MetalAluminum || metalId == Dimension1System.MetalTitanium) return 2;
        if (metalId == Dimension1System.MetalNickel || metalId == Dimension1System.MetalCobalt) return 3;
        if (metalId == Dimension1System.MetalLithium || metalId == Dimension1System.MetalTungsten) return 4;
        if (metalId == Dimension1System.MetalPlatinum) return 5;
        return 6;
    }

    private static bool IsSecondaryMetal(string metalId)
    {
        return metalId == Dimension1System.MetalCopper ||
               metalId == Dimension1System.MetalTitanium ||
               metalId == Dimension1System.MetalCobalt ||
               metalId == Dimension1System.MetalTungsten;
    }

    private static int PlanetNumber(string planetId)
    {
        if (string.IsNullOrEmpty(planetId)) return 0;
        int separator = planetId.LastIndexOf('_');
        return separator >= 0 && int.TryParse(planetId.Substring(separator + 1), out int number)
            ? number : 0;
    }

    private static void Set(TMP_Text[] texts, int index, string value)
    {
        if (texts != null && index < texts.Length && texts[index] != null) texts[index].text = value;
    }

    private static void SetActive(TMP_Text[] texts, int index, bool active)
    {
        if (texts != null && index < texts.Length && texts[index] != null)
            texts[index].gameObject.SetActive(active);
    }

    private static void SetTop(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
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

    private static Color Hex(string html, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + html, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
