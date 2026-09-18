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
    private float entryBindTimer;
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
        entryBindTimer -= Time.unscaledDeltaTime;
        if (entryBindTimer <= 0f)
        {
            entryBindTimer = .5f;
            BindEntryButtons();
        }

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

    public Sprite GetMetalIconSprite(string metalId)
    {
        int index = Array.IndexOf(MetalIds, metalId);
        return index >= 0 && metalIcons != null && index < metalIcons.Length && metalIcons[index] != null
            ? metalIcons[index].sprite
            : null;
    }

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
        for (int i = entryButtons.Count - 1; i >= 0; i--)
            if (entryButtons[i] == null) entryButtons.RemoveAt(i);

        Transform owner = panel != null ? panel.transform : transform.parent;
        if (owner == null) return;
        foreach (Transform entry in owner.GetComponentsInChildren<Transform>(true))
        {
            if (entry == null) continue;
            string objectName = entry.name;
            if (objectName != "MetalsButton" && objectName != "AllMetals") continue;

            Image hit = entry.GetComponent<Image>();
            if (hit == null)
            {
                hit = entry.gameObject.AddComponent<Image>();
                hit.color = new Color(1f, 1f, 1f, .001f);
            }
            hit.raycastTarget = true;

            Button button = entry.GetComponent<Button>();
            if (button == null) button = entry.gameObject.AddComponent<Button>();
            if (button == backButton) continue;
            button.targetGraphic = hit;
            button.transition = Selectable.Transition.None;
            button.interactable = true;
            button.onClick.RemoveListener(Open);
            if (!HasPersistentMetalsRoute(button) && !IsInsideCommandCenter(entry))
                button.onClick.AddListener(Open);
            if (!entryButtons.Contains(button)) entryButtons.Add(button);
        }
    }

    private static bool HasPersistentMetalsRoute(Button button)
    {
        if (button == null) return false;
        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            string method = button.onClick.GetPersistentMethodName(i);
            if (button.onClick.GetPersistentTarget(i) != null &&
                (method == "Open" || method == "OpenMetals" || method == "ToggleMetalsDrawer"))
                return true;
        }
        return false;
    }

    private static bool IsInsideCommandCenter(Transform entry)
    {
        for (Transform current = entry; current != null; current = current.parent)
            if (current.name == "D1CommandCenterProductionRoot") return true;
        return false;
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

/// <summary>
/// Único propietario de los tres metales resumidos del encabezado D1.
/// Los metales se obtienen de los planetas reales del sector, en orden de planeta
/// y de producción, y el inventario compartido conserva el acceso a los diez.
/// </summary>
public static class Dimension1HeaderMetalsUI
{
    private static readonly string[] OuterRimMetals =
    {
        Dimension1System.MetalIron,
        Dimension1System.MetalCopper,
        Dimension1System.MetalAluminum
    };

    private static readonly string[] DebrisRingMetals =
    {
        Dimension1System.MetalNickel,
        Dimension1System.MetalCobalt
    };

    private static readonly string[] AncientOrbitsMetals =
    {
        Dimension1System.MetalLithium,
        Dimension1System.MetalTungsten,
        Dimension1System.MetalPlatinum
    };

    private static readonly string[] SilentFrontierMetals =
    {
        Dimension1System.MetalIridium,
        Dimension1System.MetalCobalt,
        Dimension1System.MetalTungsten
    };

    public static IReadOnlyList<string> GetSectorMetalIds(string sectorId)
    {
        if (sectorId == Dimension1System.Sector02DebrisRing) return DebrisRingMetals;
        if (sectorId == Dimension1System.Sector03AncientOrbits) return AncientOrbitsMetals;
        if (sectorId == Dimension1System.Sector04SilentFrontier) return SilentFrontierMetals;
        return OuterRimMetals;
    }

    public static void Refresh(Transform screenRoot, GameState state, string sectorId)
    {
        if (screenRoot == null || state == null) return;
        IReadOnlyList<string> ids = GetSectorMetalIds(sectorId);
        string[] cardNames = CardNames(screenRoot.name);

        for (int i = 0; i < cardNames.Length; i++)
        {
            Transform card = FindDescendant(screenRoot, cardNames[i]);
            if (card == null) continue;
            bool hasMetal = i < ids.Count;
            card.gameObject.SetActive(hasMetal);
            if (!hasMetal) continue;

            string metalId = ids[i];
            SetText(card, "Name", MetalName(metalId));
            SetText(card, "Amount", FormatAmount(state.GetD1MetalAmount(metalId)));
            SetText(card, "Value", FormatAmount(state.GetD1MetalAmount(metalId)));
            SetText(card, "Rate", "+" + FormatAmount(
                Dimension1System.GetMetalProductionPerSecond(state, metalId)) + "/s");
            RefreshIcon(card, metalId);
        }
    }

    private static string[] CardNames(string rootName)
    {
        if (rootName == "D1CommandCenterProductionRoot")
            return new[] { "Resource_HIERRO", "Resource_ALUMINIO", "Resource_NÍQUEL" };
        if (rootName == "D1_GalaxyVisualRoot")
            return new[] { "Metal_HIERRO", "Metal_ALUMINIO", "Metal_NÍQUEL" };
        return new[] { "Metal_0", "Metal_1", "Metal_2" };
    }

    private static void RefreshIcon(Transform card, string metalId)
    {
        Sprite sprite = Dimension1MetalsInventoryUI.I != null
            ? Dimension1MetalsInventoryUI.I.GetMetalIconSprite(metalId)
            : null;
        Transform iconTransform = FindDirectChild(card, "SectorMetalIcon");
        if (sprite == null)
        {
            if (iconTransform != null) iconTransform.gameObject.SetActive(false);
            return;
        }

        if (iconTransform == null)
        {
            GameObject iconObject = new GameObject("SectorMetalIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObject.layer = card.gameObject.layer;
            iconObject.transform.SetParent(card, false);
            iconTransform = iconObject.transform;
            RectTransform rect = (RectTransform)iconTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2(27f, 0f);
            rect.sizeDelta = new Vector2(44f, 62f);
            iconTransform.SetAsLastSibling();
        }

        foreach (Transform child in card)
        {
            if (child == iconTransform || IsCardStructureOrText(child)) continue;
            child.gameObject.SetActive(false);
        }

        Image image = iconTransform.GetComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = false;
        iconTransform.gameObject.SetActive(true);
    }

    private static bool IsCardStructureOrText(Transform child)
    {
        string name = child.name;
        return name == "Shadow" || name == "Fill" || name == "Border" ||
               name == "PanelFill" || name == "OuterLine" || name == "InnerLine" ||
               name.EndsWith("_Border", StringComparison.Ordinal) ||
               name.EndsWith("_InnerBorder", StringComparison.Ordinal) ||
               child.GetComponent<TMP_Text>() != null || child.GetComponent<Text>() != null;
    }

    private static void SetText(Transform parent, string childName, string value)
    {
        Transform child = FindDirectChild(parent, childName);
        TMP_Text text = child != null ? child.GetComponent<TMP_Text>() : null;
        if (text != null) text.text = value;
        else
        {
            Text legacyText = child != null ? child.GetComponent<Text>() : null;
            if (legacyText != null) legacyText.text = value;
        }
    }

    private static Transform FindDescendant(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private static Transform FindDirectChild(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == name) return parent.GetChild(i);
        return null;
    }

    private static string MetalName(string metalId)
    {
        if (metalId == Dimension1System.MetalIron) return "HIERRO";
        if (metalId == Dimension1System.MetalCopper) return "COBRE";
        if (metalId == Dimension1System.MetalAluminum) return "ALUMINIO";
        if (metalId == Dimension1System.MetalTitanium) return "TITANIO";
        if (metalId == Dimension1System.MetalNickel) return "NÍQUEL";
        if (metalId == Dimension1System.MetalCobalt) return "COBALTO";
        if (metalId == Dimension1System.MetalLithium) return "LITIO";
        if (metalId == Dimension1System.MetalTungsten) return "TUNGSTENO";
        if (metalId == Dimension1System.MetalPlatinum) return "PLATINO";
        if (metalId == Dimension1System.MetalIridium) return "IRIDIO";
        return "METAL";
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
}
