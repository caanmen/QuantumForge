using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Dimension1GalaxyVisualUI : MonoBehaviour
{
    [System.Serializable]
    public sealed class MetalChip
    {
        public GameObject root;
        public TMP_Text nameText;
        public TMP_Text amountText;
        public TMP_Text rateText;
        public Image marker;
    }

    [System.Serializable]
    public sealed class SectorNodeView
    {
        public string sectorId;
        public RectTransform root;
        public Image planet;
        public Image orbitRing;
        public Image glow;
        public Image labelPlate;
        public Image[] routes;
        public GameObject lockBadge;
        public TMP_Text titleText;
        public TMP_Text stateText;
    }

    [SerializeField] private Dimension1PanelUI panel;
    [SerializeField] private TMP_Text currentSectorText;
    [SerializeField] private TMP_Text unlockedSectorText;
    [SerializeField] private MetalChip[] metalChips;
    [SerializeField] private SectorNodeView[] sectorNodes;
    [SerializeField] private Image[] routeLines;
    [SerializeField] private RectTransform starLayer;
    [SerializeField] private RectTransform nebulaLayer;
    [SerializeField] private RectTransform[] rotatingBodies;
    [SerializeField] private RectTransform[] driftingAsteroids;
    [SerializeField] private TMP_Text selectedTitleText;
    [SerializeField] private TMP_Text selectedExplorationsText;
    [SerializeField] private TMP_Text selectedStatusText;
    [SerializeField] private TMP_Text selectedDestinationsText;
    [SerializeField] private TMP_Text selectedRequirementsText;
    [SerializeField] private Image selectedPlanetPreview;
    [SerializeField] private Image secondaryPlanetPreview;
    [SerializeField] private GameObject[] hideWhileOpen;
    [SerializeField] private float refreshInterval = 0.25f;

    private float refreshTimer;
    private float nextAnimationTime;
    private Vector2 starOrigin;
    private Vector2 nebulaOrigin;
    private Vector2[] asteroidOrigins;
    private string lastPreviewSectorId;
    private bool[] hiddenPreviousStates;

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

    private void Awake()
    {
        if (starLayer != null) starOrigin = starLayer.anchoredPosition;
        if (nebulaLayer != null) nebulaOrigin = nebulaLayer.anchoredPosition;
        if (driftingAsteroids == null) return;

        asteroidOrigins = new Vector2[driftingAsteroids.Length];
        for (int i = 0; i < driftingAsteroids.Length; i++)
            if (driftingAsteroids[i] != null)
                asteroidOrigins[i] = driftingAsteroids[i].anchoredPosition;
    }

    private void OnEnable()
    {
        ApplyExclusiveNavigation(true);
        refreshTimer = 0f;
        lastPreviewSectorId = "";
        Refresh();
    }

    private void OnDisable()
    {
        ApplyExclusiveNavigation(false);
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

    private void Update()
    {
        if (hideWhileOpen != null)
            foreach (GameObject target in hideWhileOpen)
                if (target != null && target.activeSelf) target.SetActive(false);

        float time = Time.unscaledTime;
        if (time >= nextAnimationTime)
        {
            nextAnimationTime = time + (1f / 30f);
            Animate(time);
        }

        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer > 0f) return;
        refreshTimer = Mathf.Max(0.08f, refreshInterval);
        Refresh();
    }

    private void Animate(float time)
    {
        if (starLayer != null)
            starLayer.anchoredPosition = starOrigin + new Vector2(
                Mathf.Sin(time * 0.045f) * 6f,
                Mathf.Cos(time * 0.038f) * 5f);

        if (nebulaLayer != null)
            nebulaLayer.anchoredPosition = nebulaOrigin + new Vector2(
                Mathf.Cos(time * 0.032f) * 4f,
                Mathf.Sin(time * 0.027f) * 3f);

        if (rotatingBodies != null)
        {
            for (int i = 0; i < rotatingBodies.Length; i++)
            {
                RectTransform body = rotatingBodies[i];
                if (body == null) continue;
                float direction = (i & 1) == 0 ? 1f : -1f;
                body.localRotation = Quaternion.Euler(0f, 0f,
                    time * direction * (2.2f + i * 0.3f));
            }
        }

        if (sectorNodes != null)
        {
            string preview = GetPreviewSectorId();
            for (int i = 0; i < sectorNodes.Length; i++)
            {
                SectorNodeView node = sectorNodes[i];
                if (node == null || node.root == null) continue;
                float target = node.sectorId == preview
                    ? 1.018f + Mathf.Sin(time * 3.1f) * 0.018f
                    : 1f;
                node.root.localScale = Vector3.one * target;
            }
        }

        if (routeLines != null)
        {
            for (int i = 0; i < routeLines.Length; i++)
            {
                Image line = routeLines[i];
                if (line == null) continue;
                Color color = line.color;
                color.a = Mathf.Clamp01(color.a * 0.82f +
                    (0.50f + 0.18f * Mathf.Sin(time * 1.7f + i)) * 0.18f);
                line.color = color;
            }
        }

        if (driftingAsteroids == null || asteroidOrigins == null) return;
        for (int i = 0; i < driftingAsteroids.Length; i++)
        {
            RectTransform asteroid = driftingAsteroids[i];
            if (asteroid == null) continue;
            float phase = i * 1.37f;
            asteroid.anchoredPosition = asteroidOrigins[i] + new Vector2(
                Mathf.Sin(time * 0.12f + phase) * (7f + i),
                Mathf.Cos(time * 0.09f + phase) * (4f + i * 0.35f));
            asteroid.localRotation = Quaternion.Euler(0f, 0f,
                time * (2f + i * 0.2f) + phase * 12f);
        }
    }

    private void Refresh()
    {
        GameState state = GameState.I;
        if (state == null || !state.dimension01Unlocked)
        {
            SetHeader("DIMENSIÓN NO DISPONIBLE", "0/5 SECTORES");
            HideAllMetalChips();
            return;
        }

        state.EnsureDimension1State();
        int unlocked = 0;
        if (state.dimension1Sectors != null)
            foreach (D1SectorState sector in state.dimension1Sectors)
                if (sector != null && sector.unlocked) unlocked++;

        string current = Dimension1System.GetDimension1SectorVisualName(
            state.dimension1SelectedSectorId);
        SetHeader("ACTUAL · " + current.ToUpperInvariant(), unlocked + "/5 SECTORES");
        RefreshMetalChips(state);
        RefreshNodes(state);

        string preview = GetPreviewSectorId();
        if (preview != lastPreviewSectorId)
        {
            lastPreviewSectorId = preview;
            RefreshSelectedPanel(state, preview);
        }
        else
        {
            RefreshSelectedPanel(state, preview);
        }
    }

    private void RefreshNodes(GameState state)
    {
        if (sectorNodes == null) return;
        string preview = GetPreviewSectorId();

        for (int i = 0; i < sectorNodes.Length; i++)
        {
            SectorNodeView node = sectorNodes[i];
            if (node == null) continue;
            D1SectorState sector = FindSector(state, node.sectorId);
            bool unlocked = sector != null && sector.unlocked;
            bool current = state.dimension1SelectedSectorId == node.sectorId;
            bool selected = preview == node.sectorId;
            bool centerLocked = node.sectorId == Dimension1System.Sector05GalacticCenter && !unlocked;
            Color accent = selected ? Hex("F4B545") :
                centerLocked ? Hex("FF5B58") : unlocked ? Hex("55CFFF") : Hex("667586");

            if (node.orbitRing != null) node.orbitRing.color = accent;
            if (node.glow != null)
            {
                Color glow = accent;
                glow.a = selected ? 0.48f : unlocked ? 0.20f : 0.08f;
                node.glow.color = glow;
            }
            if (node.labelPlate != null)
            {
                Color plate = selected ? Hex("4B3515", 242) : Hex("07111B", 238);
                node.labelPlate.color = plate;
            }
            if (node.planet != null)
                node.planet.color = unlocked ? Color.white : Hex("697682", 205);
            if (node.lockBadge != null) node.lockBadge.SetActive(!unlocked);
            if (node.titleText != null) node.titleText.text = GetShortSectorName(node.sectorId);
            if (node.stateText != null)
            {
                int count = sector == null ? 0 : Mathf.Max(0, sector.completedExplorations);
                node.stateText.text = current
                    ? "ACTUAL · " + count + " EXPEDICIONES"
                    : unlocked ? count + " EXPEDICIONES" : "REQUISITOS PENDIENTES";
                node.stateText.color = selected ? Hex("FFD56E") : unlocked ? Hex("62D8F4") : Hex("A8B0B8");
            }

            if (node.routes != null)
            {
                Color route = selected ? Hex("F4B545") : unlocked ? Hex("4FCFF0") : Hex("3E4A56");
                route.a = unlocked || selected ? 0.72f : 0.35f;
                foreach (Image routeImage in node.routes)
                    if (routeImage != null) routeImage.color = route;
            }
        }
    }

    private void RefreshSelectedPanel(GameState state, string sectorId)
    {
        D1SectorState sector = FindSector(state, sectorId);
        bool unlocked = sector != null && sector.unlocked;
        bool current = state.dimension1SelectedSectorId == sectorId;
        int explorations = sector == null ? 0 : Mathf.Max(0, sector.completedExplorations);

        if (selectedTitleText != null)
            selectedTitleText.text = GetShortSectorName(sectorId);
        if (selectedExplorationsText != null)
            selectedExplorationsText.text = explorations.ToString();
        if (selectedStatusText != null)
            selectedStatusText.text = current ? "SECTOR ACTUAL" : unlocked ? "SECTOR DISPONIBLE" : "SECTOR BLOQUEADO";
        if (selectedDestinationsText != null)
            selectedDestinationsText.text = BuildDestinationsText(sectorId);
        if (selectedRequirementsText != null)
            selectedRequirementsText.text = unlocked
                ? BuildPlanetsText(sectorId)
                : BuildRequirementsText(state, sectorId);

        SectorNodeView previewNode = FindNode(sectorId);
        if (previewNode != null && previewNode.planet != null)
        {
            if (selectedPlanetPreview != null)
            {
                selectedPlanetPreview.sprite = previewNode.planet.sprite;
                selectedPlanetPreview.color = Color.white;
            }
            if (secondaryPlanetPreview != null)
            {
                secondaryPlanetPreview.sprite = previewNode.planet.sprite;
                secondaryPlanetPreview.color = Hex("B8D8E6", 210);
            }
        }
    }

    private string GetPreviewSectorId()
    {
        if (panel != null && Dimension1System.IsDimension1SectorId(panel.GalaxyPreviewSectorId))
            return panel.GalaxyPreviewSectorId;
        GameState state = GameState.I;
        return state != null ? state.dimension1SelectedSectorId : Dimension1System.Sector01OuterRim;
    }

    private SectorNodeView FindNode(string sectorId)
    {
        if (sectorNodes == null) return null;
        foreach (SectorNodeView node in sectorNodes)
            if (node != null && node.sectorId == sectorId) return node;
        return null;
    }

    private static D1SectorState FindSector(GameState state, string sectorId)
    {
        if (state == null || state.dimension1Sectors == null) return null;
        foreach (D1SectorState sector in state.dimension1Sectors)
            if (sector != null && sector.sectorId == sectorId) return sector;
        return null;
    }

    private static string BuildDestinationsText(string sectorId)
    {
        string[] ids = Dimension1System.GetDimension1SectorDestinationIds(sectorId);
        if (ids == null || ids.Length == 0) return "ARK · MISIÓN CENTRAL";
        var lines = new List<string>();
        for (int i = 0; i < ids.Length && i < 4; i++)
            lines.Add("  ◆  " + GetDestinationName(ids[i]));
        return string.Join("\n", lines);
    }

    private static string BuildPlanetsText(string sectorId)
    {
        if (sectorId == Dimension1System.Sector05GalacticCenter)
            return "SIN PLANETAS · ARK DETECTADA";
        if (sectorId == Dimension1System.Sector02DebrisRing)
            return "PLANETAS\nPLANETA 3";
        if (sectorId == Dimension1System.Sector01OuterRim)
            return "PLANETAS\nPLANETA 1  ·  PLANETA 2";
        if (sectorId == Dimension1System.Sector03AncientOrbits)
            return "PLANETAS\nPLANETA 4  ·  PLANETA 5";
        return "PLANETAS\nPLANETA 6  ·  PLANETA 7";
    }

    private static string BuildRequirementsText(GameState state, string sectorId)
    {
        List<D1SectorRequirementStatus> requirements =
            Dimension1System.GetD1SectorUnlockRequirements(state, sectorId);
        if (requirements == null || requirements.Count == 0)
            return "REQUISITOS\nPENDIENTE DE CARTOGRAFÍA";
        int met = 0;
        foreach (D1SectorRequirementStatus requirement in requirements)
            if (requirement != null && requirement.met) met++;
        return "REQUISITOS\n" + met + "/" + requirements.Count + " COMPLETADOS";
    }

    private static string GetShortSectorName(string sectorId)
    {
        if (sectorId == Dimension1System.Sector01OuterRim) return "BORDE EXTERIOR";
        if (sectorId == Dimension1System.Sector02DebrisRing) return "ANILLO DE RESTOS";
        if (sectorId == Dimension1System.Sector03AncientOrbits) return "ÓRBITAS ANTIGUAS";
        if (sectorId == Dimension1System.Sector04SilentFrontier) return "FRONTERA SILENCIOSA";
        if (sectorId == Dimension1System.Sector05GalacticCenter) return "CENTRO GALÁCTICO";
        return sectorId ?? "SECTOR";
    }

    private static string GetDestinationName(string id)
    {
        if (id == Dimension1System.DestinationMineralBelt) return "Cinturón Mineral";
        if (id == Dimension1System.DestinationShipGraveyard) return "Cementerio de Naves";
        if (id == Dimension1System.DestinationAbandonedShip) return "Nave Abandonada";
        if (id == Dimension1System.DestinationOrbitalRuin) return "Ruina Orbital";
        if (id == Dimension1System.DestinationDriftingProbes) return "Sondas a la Deriva";
        if (id == Dimension1System.DestinationLaboratory) return "Laboratorio";
        if (id == Dimension1System.DestinationAbandonedStation) return "Estación Abandonada";
        if (id == Dimension1System.DestinationMinorAnomaly) return "Anomalía Menor";
        if (id == Dimension1System.DestinationAncientStructure) return "Estructura Antigua";
        if (id == Dimension1System.DestinationUnstableZone) return "Zona Inestable";
        return id;
    }

    private void SetHeader(string sector, string unlocked)
    {
        if (currentSectorText != null) currentSectorText.text = sector;
        if (unlockedSectorText != null) unlockedSectorText.text = unlocked;
    }

    private void HideAllMetalChips()
    {
        if (metalChips == null) return;
        foreach (MetalChip chip in metalChips)
            if (chip != null && chip.root != null) chip.root.SetActive(false);
    }

    private void RefreshMetalChips(GameState state)
    {
        if (metalChips == null) return;
        int slot = 0;
        foreach (string metalId in MetalIds)
        {
            if (slot >= metalChips.Length) break;
            if (!Dimension1System.IsMetalUnlockedForDimension1(state, metalId)) continue;
            MetalChip chip = metalChips[slot++];
            if (chip == null) continue;
            if (chip.root != null) chip.root.SetActive(true);
            if (chip.nameText != null) chip.nameText.text = GetMetalName(metalId).ToUpperInvariant();
            if (chip.amountText != null) chip.amountText.text = FormatAmount(state.GetD1MetalAmount(metalId));
            if (chip.rateText != null)
                chip.rateText.text = "+" + FormatAmount(
                    Dimension1System.GetMetalProductionPerSecond(state, metalId)) + "/s";
            if (chip.marker != null) chip.marker.color = GetMetalColor(metalId);
        }
        for (int i = slot; i < metalChips.Length; i++)
            if (metalChips[i] != null && metalChips[i].root != null)
                metalChips[i].root.SetActive(false);
    }

    private static string FormatAmount(double value)
    {
        if (value >= 1000000d) return (value / 1000000d).ToString("0.##") + "M";
        if (value >= 1000d) return (value / 1000d).ToString("0.##") + "K";
        return value.ToString("0.##");
    }

    private static string GetMetalName(string id)
    {
        if (id == Dimension1System.MetalIron) return "Hierro";
        if (id == Dimension1System.MetalCopper) return "Cobre";
        if (id == Dimension1System.MetalAluminum) return "Aluminio";
        if (id == Dimension1System.MetalTitanium) return "Titanio";
        if (id == Dimension1System.MetalNickel) return "Níquel";
        if (id == Dimension1System.MetalCobalt) return "Cobalto";
        if (id == Dimension1System.MetalLithium) return "Litio";
        if (id == Dimension1System.MetalTungsten) return "Tungsteno";
        if (id == Dimension1System.MetalPlatinum) return "Platino";
        if (id == Dimension1System.MetalIridium) return "Iridio";
        return id ?? "Metal";
    }

    private static Color GetMetalColor(string id)
    {
        if (id == Dimension1System.MetalIron) return Hex("AEB8C4");
        if (id == Dimension1System.MetalCopper) return Hex("D8844B");
        if (id == Dimension1System.MetalAluminum) return Hex("DDE8EE");
        if (id == Dimension1System.MetalTitanium) return Hex("7CB7D7");
        if (id == Dimension1System.MetalNickel) return Hex("C6C19A");
        if (id == Dimension1System.MetalCobalt) return Hex("4F8FE8");
        if (id == Dimension1System.MetalLithium) return Hex("C76DEB");
        if (id == Dimension1System.MetalTungsten) return Hex("8793A4");
        if (id == Dimension1System.MetalPlatinum) return Hex("D8F2F1");
        if (id == Dimension1System.MetalIridium) return Hex("A977F1");
        return Color.white;
    }

    private static Color Hex(string value, byte alpha = 255)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        color.a = alpha / 255f;
        return color;
    }
}
