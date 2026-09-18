using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Room2PanelUI : MonoBehaviour
{
    private const float UiRefreshInterval = 0.1f;

    [Header("Referencias principales")]
    [SerializeField] private GameObject closedBlock;
    [SerializeField] private GameObject openedBlock;
    [SerializeField] private GameObject machinePanelRoot;

    [SerializeField] private Button mixButton;
    [SerializeField] private Button logButton;
    [SerializeField] private GameObject logPanel;
    [SerializeField] private TextMeshProUGUI logContentText;
    [SerializeField] private Button logCloseButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private TextMeshProUGUI fusionSlotsText;
    [SerializeField] private TextMeshProUGUI compositionReadingText;

    [Header("Slots")]
    [SerializeField] private Button fragmentSlotAButton;
    [SerializeField] private Button fragmentSlotBButton;
    [SerializeField] private Button catalystSlotButton;
    [SerializeField] private Button fragmentSlotAPreviousButton;
    [SerializeField] private Button fragmentSlotANextButton;
    [SerializeField] private Button fragmentSlotBPreviousButton;
    [SerializeField] private Button fragmentSlotBNextButton;
    [SerializeField] private Button catalystPreviousButton;
    [SerializeField] private Button catalystNextButton;

    [SerializeField] private TextMeshProUGUI fragmentSlotAText;
    [SerializeField] private TextMeshProUGUI fragmentSlotBText;
    [SerializeField] private TextMeshProUGUI catalystSlotText;
    [SerializeField] private Image fragmentSlotAIcon;
    [SerializeField] private Image fragmentSlotBIcon;
    [SerializeField] private Image catalystSlotIcon;
    [SerializeField] private Sprite fragmentCondensationIcon;
    [SerializeField] private Sprite fragmentConfinementIcon;
    [SerializeField] private Sprite fragmentResidualIcon;
    [SerializeField] private Sprite catalystAlphaIcon;
    [SerializeField] private Sprite catalystBetaIcon;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Button modeButton;
    [SerializeField] private TextMeshProUGUI modeButtonText;

    [SerializeField] private TextMeshProUGUI guidedIntentText;
    [SerializeField] private Button guidedIntentButton;
    [SerializeField] private TextMeshProUGUI guidedIntentButtonText;

    [SerializeField] private TextMeshProUGUI instabilityText;
    [SerializeField] private Button coolButton;
    [SerializeField] private TextMeshProUGUI coolButtonText;
    [SerializeField] private TextMeshProUGUI logButtonText;

    private ExperimentalFragmentType selectedFragmentA = ExperimentalFragmentType.None;
    private ExperimentalFragmentType selectedFragmentB = ExperimentalFragmentType.None;
    private ExperimentalCatalystType selectedCatalyst = ExperimentalCatalystType.None;
    private int currentInstability = 0;
    private int lastRewardAmount = 0;
    private int lastInstabilityGain = 0;
    private ExperimentalResultType lastFusionResult = ExperimentalResultType.None;
    private bool hasCompletedFusion;
    private int completedFusionSerial;
    private const int instabilityLowMax = 5;
    private const int instabilityMediumMax = 12;
    private const int coolEnergyCost = 30;
    private const int coolInstabilityReduction = 5;
    private int lastLocalizationRevision = -1;
    private bool isShowingLogPreview = false;
    private float nextUiRefreshTime;

    [Header("Fusión - Tiempo")]
    [SerializeField] private double baseFusionCooldownSeconds = 5.0;
    private double currentFusionCooldownSeconds = 0.0;

    private enum TrialMode
    {
        Safe,
        Balanced,
        Forced
    }

    private enum GuidedSynthesisIntent
    {
        None,
        Hallazgo,
        Muestra,
        LecturaIncompleta,
        CompuestoUtil
    }

    private TrialMode currentTrialMode = TrialMode.Balanced;
    private GuidedSynthesisIntent currentGuidedIntent = GuidedSynthesisIntent.None;

    public bool FusionSelectionComplete =>
        selectedFragmentA != ExperimentalFragmentType.None &&
        selectedFragmentB != ExperimentalFragmentType.None &&
        selectedCatalyst != ExperimentalCatalystType.None;
    public int UnlockedFusionSlotCount => GetUnlockedFusionSlotCount();
    public bool HasUnlockedFusionSlot => UnlockedFusionSlotCount > 0;
    public bool HasRequiredFragmentsForSelection =>
        FusionSelectionComplete && HasRequiredFragmentsForCurrentSelection();
    public bool CanExecuteFusion =>
        GameState.I != null &&
        GameState.I.experimentalChamberUnlocked &&
        HasUnlockedFusionSlot &&
        FusionSelectionComplete &&
        HasRequiredFragmentsForSelection &&
        !FusionInProgress &&
        !FusionCoolingDown;
    public string FusionStatusTitle => BuildFusionStatusTitle();
    public string FusionGuidanceMessage => BuildFusionGuidanceMessage();
    public bool FragmentASelected => selectedFragmentA != ExperimentalFragmentType.None;
    public bool FragmentBSelected => selectedFragmentB != ExperimentalFragmentType.None;
    public bool CatalystSelected => selectedCatalyst != ExperimentalCatalystType.None;
    public int CurrentInstability => currentInstability;
    public float CurrentFusionRisk01 => (float)GetCurrentFusionFailureChance();
    public bool FusionCoolingDown => IsFusionCoolingDown();
    public bool FusionInProgress =>
        GameState.I != null && GameState.I.pendingFusion != null &&
        GameState.I.pendingFusion.active;
    public double FusionCooldownRemaining => currentFusionCooldownSeconds;
    public bool CanCoolCurrentInstability =>
        GameState.I != null && currentInstability > 0 &&
        GameState.I.triangleEnergy >= coolEnergyCost;
    public bool HasCompletedFusion => hasCompletedFusion;
    public int CompletedFusionSerial => completedFusionSerial;
    public ExperimentalResultType LastFusionResult => lastFusionResult;
    public int LastFusionRewardAmount => lastRewardAmount;
    public int LastFusionInstabilityGain => lastInstabilityGain;
    public int SynthesisCoreCounter => GameState.I != null
        ? GameState.I.synthesisCoreFusionCounter
        : 0;
    public string InstabilityStateDisplayName => GetInstabilityStateName();
    public string LastFusionResultDisplayName => GetResultDisplayName(lastFusionResult);
    public string LastFusionRewardDisplayText =>
        BuildRewardDisplayText(lastFusionResult, lastRewardAmount);
    public bool IsEnglishUi => IsEnglishLanguage();
    public int ExperimentalLogEntryCount
    {
        get
        {
            return GameState.I != null
                ? GameState.I.GetUnreadExperimentalRecipeCount()
                : 0;
        }
    }

    private void Awake()
    {
        if (fragmentSlotAButton != null)
            fragmentSlotAButton.onClick.AddListener(OnClickFragmentSlotA);

        if (fragmentSlotBButton != null)
            fragmentSlotBButton.onClick.AddListener(OnClickFragmentSlotB);

        if (catalystSlotButton != null)
            catalystSlotButton.onClick.AddListener(OnClickCatalystSlot);

        if (fragmentSlotAPreviousButton != null)
            fragmentSlotAPreviousButton.onClick.AddListener(() => CycleFragmentA(-1));
        if (fragmentSlotANextButton != null)
            fragmentSlotANextButton.onClick.AddListener(() => CycleFragmentA(1));
        if (fragmentSlotBPreviousButton != null)
            fragmentSlotBPreviousButton.onClick.AddListener(() => CycleFragmentB(-1));
        if (fragmentSlotBNextButton != null)
            fragmentSlotBNextButton.onClick.AddListener(() => CycleFragmentB(1));
        if (catalystPreviousButton != null)
            catalystPreviousButton.onClick.AddListener(() => CycleCatalyst(-1));
        if (catalystNextButton != null)
            catalystNextButton.onClick.AddListener(() => CycleCatalyst(1));

        if (logButton != null)
            logButton.onClick.AddListener(OnClickLogButton);

        if (logCloseButton != null)
            logCloseButton.onClick.AddListener(OnClickLogCloseButton);
        
        if (coolButton != null)
            coolButton.onClick.AddListener(OnClickCoolButton);

        if (LocalizationManager.I != null)
            lastLocalizationRevision = LocalizationManager.I.Revision;
           
        if (mixButton != null)
            mixButton.onClick.AddListener(OnClickMixButton);
        
        if (logPanel != null)
            logPanel.SetActive(false);

        if (modeButton != null)
            modeButton.onClick.AddListener(OnClickModeButton);

        if (guidedIntentButton != null)
            guidedIntentButton.onClick.AddListener(OnClickGuidedIntentButton);

        LoadGuidedIntentFromGameState();
        PullFusionRuntimeStateFromGameState();
        TryFinalizePendingFusion();

        RefreshInstabilityUI();
        RefreshTrialModeUI();
        RefreshGuidedSynthesisUI();
        RefreshCoolButtonUI();
        RefreshStaticRoom2Texts();

    }

    private void OnEnable()
    {
        nextUiRefreshTime = 0f;
        LoadGuidedIntentFromGameState();
        PullFusionRuntimeStateFromGameState();
        TryFinalizePendingFusion();
        RefreshGuidedSynthesisUI();
        RefreshUI();
    }

    private void PullFusionRuntimeStateFromGameState()
    {
        if (GameState.I == null)
            return;

        currentInstability = System.Math.Clamp(
            GameState.I.fusionInstability, 0, GameState.FusionInstabilityMax);
        double savedCooldown = GameState.I.fusionCooldownRemainingSeconds;
        if (GameState.I.pendingFusion == null || !GameState.I.pendingFusion.active)
            savedCooldown = 0.0;
        currentFusionCooldownSeconds =
            double.IsNaN(savedCooldown) || double.IsInfinity(savedCooldown)
                ? 0.0
                : System.Math.Max(0.0, savedCooldown);
    }

    private void PushFusionRuntimeStateToGameState()
    {
        if (GameState.I == null)
            return;

        GameState.I.fusionInstability = System.Math.Clamp(
            currentInstability, 0, GameState.FusionInstabilityMax);
        GameState.I.fusionCooldownRemainingSeconds = System.Math.Max(
            0.0, currentFusionCooldownSeconds);
    }

    private int GetUnlockedFusionSlotCount()
    {
        if (MachineManager.I == null)
            return 0;

        return MachineManager.I.GetUnlockedFusionSlotCount();
    }

    private double GetFusionCooldownDuration()
    {
        double cooldown = baseFusionCooldownSeconds;

        if (MachineManager.I != null)
        {
            double reduction = MachineManager.I.GetTotalEffectValue(
                MachineNodeEffectType.FusionTimeReduction
            );

            reduction = Mathf.Clamp((float)reduction, 0f, 0.75f);
            cooldown *= (1.0 - reduction);
        }

        return System.Math.Max(1.0, cooldown);
    }

    private bool IsFusionCoolingDown()
    {
        return FusionInProgress && currentFusionCooldownSeconds > 0.0;
    }

    private void RefreshFusionSlotsUI()
    {
        if (fusionSlotsText == null)
            return;

        int slots = GetUnlockedFusionSlotCount();
        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
        string readiness = slots <= 0
            ? (english ? "FUSION TABLE LOCKED" : "MESA DE FUSIÓN BLOQUEADA")
            : FusionInProgress
                ? (english ? "FUSING " : "FUSIONANDO ") +
                    currentFusionCooldownSeconds.ToString("0.0") + "s"
                : (english ? "FUSION READY" : "FUSIÓN LISTA");
        fusionSlotsText.text = (english ? "SLOTS " : "RANURAS ") + slots +
            "     •     " + readiness;

        if (HasSynthesisCore() && GameState.I != null)
        {
            int counter = GameState.I.synthesisCoreFusionCounter;
            string coreState = counter >= 10
                ? (english ? "CORE CHARGED" : "NÚCLEO CARGADO")
                : (english ? "CORE " : "NÚCLEO ") + counter + "/10";
            fusionSlotsText.text += "     •     " + coreState;
        }
    }

    private bool HasCompositionReading()
    {
        if (MachineManager.I == null)
            return false;

        return MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.RevealFusionProbabilities) > 0.0;
    }

    private bool HasCurrentRecipeBeenDiscovered()
    {
        if (GameState.I == null)
            return false;

        if (selectedFragmentA == ExperimentalFragmentType.None ||
            selectedFragmentB == ExperimentalFragmentType.None ||
            selectedCatalyst == ExperimentalCatalystType.None)
            return false;

        string mixKey = GameState.I.BuildExperimentalMixKey(
            selectedFragmentA,
            selectedFragmentB,
            selectedCatalyst);

        if (GameState.I.experimentalMixLog == null)
            return false;

        foreach (var entry in GameState.I.experimentalMixLog)
        {
            if (entry == null)
                continue;

            if (entry.mixKey == mixKey && entry.discovered)
                return true;
        }

        return false;
    }

    private void RefreshCompositionReadingUI()
    {
        if (compositionReadingText == null)
            return;

        bool discovered = HasCurrentRecipeBeenDiscovered();
        string probableResultText = discovered
            ? GetResultDisplayName(D3FusionService.ResolveRecipeResult(
                selectedFragmentA, selectedFragmentB, selectedCatalyst))
            : "???";

        if (!HasCompositionReading())
        {
            compositionReadingText.text =
                "Lectura de composición: bloqueada\n" +
                "Resultado conocido: " + probableResultText + "\n" +
                "Riesgo estimado: ???";
            return;
        }

        int riskPercent = Mathf.RoundToInt((float)(GetCurrentFusionFailureChance() * 100.0));

        int modeRiskPercent = Mathf.RoundToInt(
            (float)(GetTrialModeFailureBonus() * 100.0));
        int instabilityRiskPercent = Mathf.RoundToInt(
            (float)(GetInstabilityFailureBonus() * 100.0));
        compositionReadingText.text =
            "Lectura de composición: activa\n" +
            "Resultado conocido: " + probableResultText + "\n" +
            "Riesgo " + riskPercent + "% · modo +" + modeRiskPercent +
            "% · inest. +" + instabilityRiskPercent + "%";
    }

    private void RefreshStaticRoom2Texts()
    {
        if (titleText != null)
        {
            titleText.text = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.title")
                : "Cámara Experimental";
        }

        if (introText != null)
        {
            introText.text =
                "Combina dos fragmentos y un catalizador. Las Mezclas crean " +
                "materiales necesarios para reparar símbolos del Monolito.";
        }

        if (logButtonText != null)
        {
            logButtonText.text = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.log.button")
                : "Log";
        }
    }

    private void LoadGuidedIntentFromGameState()
    {
        if (GameState.I == null)
        {
            currentGuidedIntent = GuidedSynthesisIntent.None;
            return;
        }

        int savedIntent = Mathf.Clamp(GameState.I.guidedSynthesisIntent, 0, 4);
        currentGuidedIntent = (GuidedSynthesisIntent)savedIntent;
    }

    private void SaveGuidedIntentToGameState()
    {
        if (GameState.I == null)
            return;

        GameState.I.guidedSynthesisIntent = (int)currentGuidedIntent;
    }

    private void OnClickGuidedIntentButton()
    {
        if (!HasGuidedSynthesis())
            return;

        switch (currentGuidedIntent)
        {
            case GuidedSynthesisIntent.None:
                currentGuidedIntent = GuidedSynthesisIntent.Hallazgo;
                break;

            case GuidedSynthesisIntent.Hallazgo:
                currentGuidedIntent = GuidedSynthesisIntent.Muestra;
                break;

            case GuidedSynthesisIntent.Muestra:
                currentGuidedIntent = GuidedSynthesisIntent.LecturaIncompleta;
                break;

            case GuidedSynthesisIntent.LecturaIncompleta:
                currentGuidedIntent = GuidedSynthesisIntent.CompuestoUtil;
                break;

            default:
                currentGuidedIntent = GuidedSynthesisIntent.None;
                break;
        }

        SaveGuidedIntentToGameState();
        RefreshGuidedSynthesisUI();
    }

    private void OnClickModeButton()
    {
        switch (currentTrialMode)
        {
            case TrialMode.Safe:
                currentTrialMode = TrialMode.Balanced;
                break;

            case TrialMode.Balanced:
                currentTrialMode = TrialMode.Forced;
                break;

            default:
                currentTrialMode = TrialMode.Safe;
                break;
        }

        RefreshTrialModeUI();
    }

    private void OnClickCoolButton()
    {
        if (GameState.I == null)
            return;

        PullFusionRuntimeStateFromGameState();

        if (currentInstability <= 0)
            return;

        if (!GameState.I.TrySpendTriangleEnergy(coolEnergyCost))
            return;

        currentInstability -= coolInstabilityReduction;

        if (currentInstability < 0)
            currentInstability = 0;

        PushFusionRuntimeStateToGameState();

        RefreshInstabilityUI();
        RefreshCoolButtonUI();
        RefreshCompositionReadingUI();

        if (SaveService.I != null)
        {
            SaveService.I.Save();
        }
    }

    private string GetTrialModeKey()
    {
        switch (currentTrialMode)
        {
            case TrialMode.Safe:
                return "room2.mode.safe";

            case TrialMode.Forced:
                return "room2.mode.forced";

            default:
                return "room2.mode.balanced";
        }
    }

    private int GetTrialModeRewardAmount()
    {
        switch (currentTrialMode)
        {
            case TrialMode.Forced:
                return 2;

            case TrialMode.Safe:
            case TrialMode.Balanced:
            default:
                return 1;
        }
    }
    private int GetTrialModeInstabilityGain()
    {
        switch (currentTrialMode)
        {
            case TrialMode.Safe:
                return 1;

            case TrialMode.Forced:
                return 4;

            case TrialMode.Balanced:
            default:
                return 2;
        }
    }

    private string GetTrialModeDescription()
    {
        bool english = IsEnglishLanguage();
        switch (currentTrialMode)
        {
            case TrialMode.Safe:
                return english
                    ? "1 result · lower risk · +1 instability"
                    : "1 resultado · menor riesgo · +1 inestabilidad";
            case TrialMode.Forced:
                return english
                    ? "2 results · higher risk · +4 instability"
                    : "2 resultados · mayor riesgo · +4 inestabilidad";
            default:
                return english
                    ? "1 result · normal risk · +2 instability"
                    : "1 resultado · riesgo normal · +2 inestabilidad";
        }
    }

    private double GetCurrentFusionFailureChance()
    {
        double failureChance = GetRecipeBaseFailureChance() +
            GetTrialModeFailureBonus() + GetInstabilityFailureBonus();

        if (MachineManager.I != null)
        {
            failureChance -= MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.FusionFailureReduction);
        }

        if (HasCatalystTuning() && selectedCatalyst == ExperimentalCatalystType.Beta)
        {
            failureChance -= 0.05;
        }

        failureChance = Mathf.Clamp01((float)failureChance);
        failureChance = D3FusionService.ApplyStableReactionChamberReduction(
            failureChance, HasStableReactionChamber());

        bool hasCompleteFusionSelection =
            selectedFragmentA != ExperimentalFragmentType.None &&
            selectedFragmentB != ExperimentalFragmentType.None &&
            selectedCatalyst != ExperimentalCatalystType.None;

        if (IsSynthesisCoreCharged())
        {
            failureChance *= 0.75;
        }

        if (hasCompleteFusionSelection)
        {
            failureChance = Mathf.Max((float)failureChance, 0.03f);
        }

        return Mathf.Clamp01((float)failureChance);
    }

    private double GetFusionFailureChanceBeforeStableChamber()
    {
        double failureChance = GetRecipeBaseFailureChance() +
            GetTrialModeFailureBonus() + GetInstabilityFailureBonus();

        if (MachineManager.I != null)
        {
            failureChance -= MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.FusionFailureReduction);
        }

        if (HasCatalystTuning() && selectedCatalyst == ExperimentalCatalystType.Beta)
        {
            failureChance -= 0.05;
        }

        return Mathf.Clamp01((float)failureChance);
    }

    private double GetTrialModeFailureBonus()
    {
        return currentTrialMode switch
        {
            TrialMode.Forced => 0.10,
            TrialMode.Balanced => 0.05,
            _ => 0.0
        };
    }

    private double GetInstabilityFailureBonus()
    {
        if (currentInstability > instabilityMediumMax) return 0.10;
        if (currentInstability > instabilityLowMax) return 0.05;
        return 0.0;
    }

    private double GetRecipeBaseFailureChance()
    {
        if (selectedFragmentA == ExperimentalFragmentType.None ||
            selectedFragmentB == ExperimentalFragmentType.None ||
            selectedCatalyst == ExperimentalCatalystType.None)
        {
            return 0.0;
        }

        int a = (int)selectedFragmentA;
        int b = (int)selectedFragmentB;

        if (a > b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        ExperimentalFragmentType left = (ExperimentalFragmentType)a;
        ExperimentalFragmentType right = (ExperimentalFragmentType)b;

        if (left == ExperimentalFragmentType.ResidualInterference &&
            right == ExperimentalFragmentType.ResidualInterference)
        {
            return 0.30;
        }

        if (left == right)
        {
            return 0.05;
        }

        return 0.15;
    }

    private bool RollFusionFailure()
    {
        double failureChance = GetCurrentFusionFailureChance();
        return Random.value < failureChance;
    }

    private int GetUsefulResultRewardBonus()
    {
        if (MachineManager.I == null)
            return 0;

        double bonusChance = MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.FusionUsefulResultBonus);

        // Núcleo de Sincronización: progreso de Zona 2 mejora resultados útiles.
        bonusChance += MachineManager.I.GetZoneProgressSyncBonus(MachineZoneType.FusionSector);

        if (HasCatalystTuning() && selectedCatalyst == ExperimentalCatalystType.Alpha)
        {
            bonusChance += 0.05;
        }

        if (IsSynthesisCoreCharged())
        {
            bonusChance += 0.15;
        }

        if (bonusChance <= 0.0)
            return 0;

        return Random.value < bonusChance ? 1 : 0;
    }
    

    private bool HasCatalystTuning()
    {
        if (MachineManager.I == null)
            return false;

        return MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.CatalystTuning) > 0.0;
    }

    private bool HasGuidedSynthesis()
    {
        if (MachineManager.I == null)
            return false;

        return MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.GuidedSynthesis) > 0.0;
    }

    private bool HasStableReactionChamber()
    {
        if (MachineManager.I == null)
            return false;

        return MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.StableReactionChamber) > 0.0;
    }

    private bool HasSynthesisCore()
    {
        if (MachineManager.I == null)
            return false;

        return MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.SynthesisCore) > 0.0;
    }

    private bool IsSynthesisCoreCharged()
    {
        if (!HasSynthesisCore())
            return false;

        if (GameState.I == null)
            return false;

        return GameState.I.synthesisCoreFusionCounter >= 10;
    }

    private bool TryConvertFailureIntoMinorResult(double failureChanceBeforeStableChamber)
    {
        double conversionChance =
            D3FusionService.GetStableMinorConversionChance(
                failureChanceBeforeStableChamber,
                GetCurrentFusionFailureChance(),
                HasStableReactionChamber());
        return Random.value < conversionChance;
    }


    private string GetInstabilityStateName()
    {
        if (LocalizationManager.I == null)
        {
            if (currentInstability <= instabilityLowMax)
                return "Baja";

            if (currentInstability <= instabilityMediumMax)
                return "Media";

            return "Alta";
        }

        if (currentInstability <= instabilityLowMax)
            return LocalizationManager.I.T("room2.instability.low");

        if (currentInstability <= instabilityMediumMax)
            return LocalizationManager.I.T("room2.instability.medium");

        return LocalizationManager.I.T("room2.instability.high");
    }

    private void RefreshInstabilityUI()
    {
        if (instabilityText == null)
            return;

        string instabilityLabel = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.instability.label")
            : "Inestabilidad:";

        string stateLabel = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.instability.state")
            : "Estado:";

        instabilityText.text =
            instabilityLabel.ToUpperInvariant() + " " + currentInstability + " / 20" +
            "\n" + stateLabel.ToUpperInvariant() + " " +
            GetInstabilityStateName().ToUpperInvariant();
    }

    private void RefreshCoolButtonUI()
    {
        if (coolButtonText != null)
        {
            string coolLabel = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.cool.button")
                : "Enfriar";

            bool english = LocalizationManager.I != null &&
                LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
            coolButtonText.text = coolLabel.ToUpperInvariant() + " · " +
                coolEnergyCost + (english ? " ENERGY" : " ENERGÍA");
        }

        if (coolButton != null)
        {
            bool canCool =
                GameState.I != null &&
                currentInstability > 0 &&
                GameState.I.triangleEnergy >= coolEnergyCost;

            coolButton.interactable = canCool;
        }
    }

    private string GetGuidedIntentDisplayName()
    {
        switch (currentGuidedIntent)
        {
            case GuidedSynthesisIntent.Hallazgo:
                return "Anomalía";

            case GuidedSynthesisIntent.Muestra:
                return "Condensado";

            case GuidedSynthesisIntent.LecturaIncompleta:
                return "Vestigio";

            case GuidedSynthesisIntent.CompuestoUtil:
                return "Compuesto";

            case GuidedSynthesisIntent.None:
            default:
                return "Ninguna";
        }
    }

    private float GetGuidedIntentChance()
    {
        switch (currentGuidedIntent)
        {
            case GuidedSynthesisIntent.Hallazgo:
                return 0.12f;

            case GuidedSynthesisIntent.Muestra:
                return 0.09f;

            case GuidedSynthesisIntent.LecturaIncompleta:
                return 0.07f;

            case GuidedSynthesisIntent.CompuestoUtil:
                return 0.05f;

            case GuidedSynthesisIntent.None:
            default:
                return 0f;
        }
    }

    private string GetGuidedIntentBonusText()
    {
        float chance = GetGuidedIntentChance();

        if (chance <= 0f)
            return "sin intención activa";

        return "+" + (chance * 100f).ToString("0") + "% hacia " + GetGuidedIntentDisplayName();
    }

    private void RefreshGuidedSynthesisUI()
    {
        bool unlocked = HasGuidedSynthesis();

        if (guidedIntentText != null)
        {
            guidedIntentText.text = unlocked
            ? "Síntesis Guiada:\nIntención: " + GetGuidedIntentDisplayName() + "\nBonus: " + GetGuidedIntentBonusText()
            : "Síntesis Guiada:\nBloqueada";
        }

        if (guidedIntentButtonText != null)
        {
            guidedIntentButtonText.text = unlocked
                ? "INTENCIÓN: " + GetGuidedIntentDisplayName().ToUpperInvariant()
                : "INTENCIÓN: BLOQUEADA";
        }

        if (guidedIntentButton != null)
        {
            guidedIntentButton.interactable = unlocked;
        }
    }

    private void RefreshTrialModeUI()
    {
        string modeLabel = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.mode.label")
            : "Modo de ensayo:";

        string modeName = LocalizationManager.I != null
            ? LocalizationManager.I.T(GetTrialModeKey())
            : "Balanceado";

        if (modeText != null)
            modeText.text = modeLabel + "\n" + modeName;

        if (modeButtonText != null)
            modeButtonText.text = modeName.ToUpperInvariant() + "\n" +
                GetTrialModeDescription().ToUpperInvariant();
    }

    private void RefreshLocalizationIfNeeded()
    {
        if (LocalizationManager.I == null)
            return;

        if (lastLocalizationRevision != LocalizationManager.I.Revision)
        {
            lastLocalizationRevision = LocalizationManager.I.Revision;
            RefreshTrialModeUI();
            RefreshInstabilityUI();
            RefreshCoolButtonUI();
            RefreshGuidedSynthesisUI();
            RefreshFusionSlotsUI();
            RefreshCompositionReadingUI();
            RefreshStaticRoom2Texts();
        }
    }
    private void OnClickLogCloseButton()
    {
        if (logPanel != null)
            logPanel.SetActive(false);

        isShowingLogPreview = false;
    }

    private void Update()
    {
        PullFusionRuntimeStateFromGameState();

        if (currentFusionCooldownSeconds > 0.0)
        {
            double elapsed = QaRuntimeService.ScaleOnlineSeconds(
                Time.unscaledDeltaTime);
            AdvanceQaFusionCooldown(elapsed);
        }

        RefreshLocalizationIfNeeded();
        if (Time.unscaledTime < nextUiRefreshTime)
            return;

        nextUiRefreshTime = Time.unscaledTime + UiRefreshInterval;
        RefreshUI();
    }

    public void AdvanceQaFusionCooldown(double simulatedSeconds)
    {
        if (simulatedSeconds <= 0.0 || double.IsNaN(simulatedSeconds) ||
            double.IsInfinity(simulatedSeconds))
            return;

        // El estado persistente es la fuente de verdad. Esto también cubre el
        // avance QA cuando el panel de Mezclas está cargado pero inactivo.
        PullFusionRuntimeStateFromGameState();
        currentFusionCooldownSeconds = System.Math.Max(
            0.0, currentFusionCooldownSeconds - simulatedSeconds);
        PushFusionRuntimeStateToGameState();
        TryFinalizePendingFusion();
    }

    private bool TryFinalizePendingFusion()
    {
        if (GameState.I == null || GameState.I.pendingFusion == null ||
            !GameState.I.pendingFusion.active ||
            GameState.I.fusionCooldownRemainingSeconds > 0.000001)
        {
            return false;
        }

        ExperimentalPendingFusionState pending = GameState.I.pendingFusion;
        ExperimentalResultType result =
            (ExperimentalResultType)pending.result;
        ExperimentalFragmentType fragmentA =
            (ExperimentalFragmentType)pending.fragmentA;
        ExperimentalFragmentType fragmentB =
            (ExperimentalFragmentType)pending.fragmentB;
        ExperimentalCatalystType catalyst =
            (ExperimentalCatalystType)pending.catalyst;

        if (result != ExperimentalResultType.None && pending.rewardAmount > 0)
            GameState.I.AddExperimentalResult(result, pending.rewardAmount);

        D3FusionService.RegisterMixResult(
            GameState.I, fragmentA, fragmentB, catalyst, result);
        D3DiagnosticSystem.RegisterManualFusionRecipe(
            GameState.I, fragmentA, fragmentB, catalyst);

        if (pending.hadSynthesisCore)
        {
            GameState.I.synthesisCoreFusionCounter =
                pending.synthesisCoreWasCharged
                    ? 0
                    : GameState.I.synthesisCoreFusionCounter + 1;
        }

        currentInstability = System.Math.Clamp(
            currentInstability + pending.instabilityGain,
            0, GameState.FusionInstabilityMax);
        lastFusionResult = result;
        lastRewardAmount = pending.rewardAmount;
        lastInstabilityGain = pending.instabilityGain;
        hasCompletedFusion = true;
        completedFusionSerial++;

        bool failureConverted = pending.failureConverted;
        bool synthesisCoreWasCharged = pending.synthesisCoreWasCharged;
        GameState.I.pendingFusion = new ExperimentalPendingFusionState();
        currentFusionCooldownSeconds = 0.0;
        PushFusionRuntimeStateToGameState();

        RefreshInstabilityUI();
        RefreshCoolButtonUI();
        RefreshFusionSlotsUI();

        if (statusText != null)
        {
            statusText.text = BuildResultMessage(result);
            if (failureConverted)
            {
                statusText.text += IsEnglishLanguage()
                    ? "\nSevere failure stabilized by the Reaction Chamber."
                    : "\nFallo fuerte estabilizado por la Cámara de Reacción.";
            }
            if (synthesisCoreWasCharged)
            {
                statusText.text += IsEnglishLanguage()
                    ? "\nSynthesis Core discharged: fusion reinforced."
                    : "\nNúcleo de Síntesis descargado: fusión reforzada.";
            }
        }

        SaveService.I?.Save();
        return true;
    }

    private void RefreshUI()
    {
        if (GameState.I == null)
            return;

        bool unlocked = GameState.I.experimentalChamberUnlocked;

        if (closedBlock != null) closedBlock.SetActive(!unlocked);
        if (openedBlock != null) openedBlock.SetActive(unlocked);
        if (machinePanelRoot != null) machinePanelRoot.SetActive(unlocked);

        RefreshFusionSlotsUI();
        RefreshCompositionReadingUI();
        RefreshGuidedSynthesisUI();

        if (titleText != null)
        {
            titleText.text = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.title")
                : "Cámara Experimental";
        }

        if (!unlocked)
        {
            if (introText != null)
            {
                introText.text = LocalizationManager.I != null
                    ? LocalizationManager.I.T("room2.intro_locked")
                    : "La cámara permanece sellada. Se requiere una keycard para habilitar el acceso al Cuarto 2.";
            }

            if (fragmentSlotAText != null) fragmentSlotAText.text = "-";
            if (fragmentSlotBText != null) fragmentSlotBText.text = "-";
            if (catalystSlotText != null) catalystSlotText.text = "-";
            if (statusText != null) statusText.text = FusionStatusTitle;
            return;
        }

        if (introText != null)
        {
            introText.text = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.intro")
                : "Selecciona dos fragmentos, elige un catalizador y ejecuta un ensayo.";
        }

        if (statusText != null && !isShowingLogPreview && !hasCompletedFusion)
            statusText.text = FusionStatusTitle;

        if (fragmentSlotAText != null)
            fragmentSlotAText.text = BuildFragmentSlotText(selectedFragmentA);
        if (fragmentSlotBText != null)
            fragmentSlotBText.text = BuildFragmentSlotText(selectedFragmentB);
        if (catalystSlotText != null)
            catalystSlotText.text = BuildCatalystSlotText(selectedCatalyst);

        RefreshFusionSlotIcons();
    }

    private string BuildFusionStatusTitle()
    {
        bool english = IsEnglishLanguage();
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return english ? "EXPERIMENTAL CHAMBER LOCKED" : "CÁMARA EXPERIMENTAL BLOQUEADA";
        if (!HasUnlockedFusionSlot)
            return english ? "FUSION TABLE NOT REPAIRED" : "MESA DE FUSIÓN SIN REPARAR";
        if (FusionInProgress)
            return (english ? "FUSION IN PROGRESS: " : "FUSIÓN EN PROCESO: ") +
                FusionCooldownRemaining.ToString("0.0") + " s";
        if (selectedFragmentA == ExperimentalFragmentType.None &&
            selectedFragmentB == ExperimentalFragmentType.None &&
            selectedCatalyst == ExperimentalCatalystType.None)
            return english ? "SELECT THE THREE COMPONENTS" : "SELECCIONA LOS TRES COMPONENTES";
        if (selectedFragmentA == ExperimentalFragmentType.None)
            return english ? "FRAGMENT A IS MISSING" : "FALTA EL FRAGMENTO A";
        if (selectedFragmentB == ExperimentalFragmentType.None)
            return english ? "FRAGMENT B IS MISSING" : "FALTA EL FRAGMENTO B";
        if (selectedCatalyst == ExperimentalCatalystType.None)
            return english ? "CATALYST IS MISSING" : "FALTA EL CATALIZADOR";
        if (!HasRequiredFragmentsForSelection)
            return english ? "INSUFFICIENT FRAGMENTS" : "FRAGMENTOS INSUFICIENTES";
        return english ? "FUSION CONFIGURATION READY" : "CONFIGURACIÓN DE FUSIÓN LISTA";
    }

    private string BuildFusionGuidanceMessage()
    {
        bool english = IsEnglishLanguage();
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return english
                ? "Obtain the Room 2 keycard to access the experimental chamber."
                : "Obtén la keycard del Cuarto 2 para acceder a la cámara experimental.";
        if (!HasUnlockedFusionSlot)
            return english
                ? "Uncover the Machine in the experimental chamber to activate the first Mixes slot."
                : "Destapa la Máquina en la cámara experimental para activar la primera ranura de Mezclas.";
        if (FusionInProgress)
            return (english ? "The reward will be delivered automatically in " :
                "La recompensa se entregará automáticamente en ") +
                FusionCooldownRemaining.ToString("0.0") + " s.";
        if (!FusionSelectionComplete)
            return english
                ? "Choose Fragment A, Fragment B and an Alpha or Beta catalyst."
                : "Elige Fragmento A, Fragmento B y un catalizador Alpha o Beta.";
        if (!HasRequiredFragmentsForSelection)
            return BuildMissingFragmentMessage(english);
        string knownResult = HasCurrentRecipeBeenDiscovered()
            ? GetResultDisplayName(D3FusionService.ResolveRecipeResult(
                selectedFragmentA, selectedFragmentB, selectedCatalyst))
            : "???";
        return english
            ? "Known output: " + knownResult +
              ". Press FUSE; the material is delivered when the timer ends."
            : "Resultado conocido: " + knownResult +
              ". Pulsa FUSIONAR; el material se entrega al terminar el contador.";
    }

    private string BuildMissingFragmentMessage(bool english)
    {
        if (GameState.I == null)
            return english ? "Fragment inventory is unavailable." : "El inventario de fragmentos no está disponible.";

        if (selectedFragmentA == selectedFragmentB)
        {
            int available = GameState.I.GetFragmentCount(selectedFragmentA);
            string name = GetFragmentDisplayName(selectedFragmentA);
            int missingCount = System.Math.Max(0, 2 - available);
            return english
                ? "Missing " + missingCount + " fragment" + (missingCount == 1 ? ": " : "s: ") + name + "."
                : "Falta" + (missingCount == 1 ? " " : "n ") + missingCount +
                  " fragmento" + (missingCount == 1 ? ": " : "s: ") + name + ".";
        }

        bool missingA = GameState.I.GetFragmentCount(selectedFragmentA) <= 0;
        bool missingB = GameState.I.GetFragmentCount(selectedFragmentB) <= 0;
        string nameA = GetFragmentDisplayName(selectedFragmentA);
        string nameB = GetFragmentDisplayName(selectedFragmentB);

        if (missingA && missingB)
            return english
                ? "Missing: 1 " + nameA + " + 1 " + nameB + "."
                : "Faltan: 1 " + nameA + " + 1 " + nameB + ".";

        string missingName = missingA ? nameA : nameB;
        return english
            ? "Missing 1 fragment: " + missingName + "."
            : "Falta 1 fragmento: " + missingName + ".";
    }

    private static bool IsEnglishLanguage()
    {
        return LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
    }

    private void RefreshFusionSlotIcons()
    {
        SetSlotIcon(fragmentSlotAIcon, GetFragmentIcon(selectedFragmentA));
        SetSlotIcon(fragmentSlotBIcon, GetFragmentIcon(selectedFragmentB));
        SetSlotIcon(catalystSlotIcon, GetCatalystIcon(selectedCatalyst));
    }

    private Sprite GetFragmentIcon(ExperimentalFragmentType fragmentType)
    {
        switch (fragmentType)
        {
            case ExperimentalFragmentType.Condensation:
                return fragmentCondensationIcon;
            case ExperimentalFragmentType.Confinement:
                return fragmentConfinementIcon;
            case ExperimentalFragmentType.ResidualInterference:
                return fragmentResidualIcon;
            default:
                return null;
        }
    }

    private Sprite GetCatalystIcon(ExperimentalCatalystType catalystType)
    {
        switch (catalystType)
        {
            case ExperimentalCatalystType.Alpha:
                return catalystAlphaIcon;
            case ExperimentalCatalystType.Beta:
                return catalystBetaIcon;
            default:
                return null;
        }
    }

    private static void SetSlotIcon(Image image, Sprite sprite)
    {
        if (image == null)
            return;

        image.sprite = sprite;
        image.color = Color.white;
        image.enabled = sprite != null;
        Transform emptyInterior = image.transform.parent != null
            ? image.transform.parent.Find("ApprovedSlotInterior")
            : null;
        if (emptyInterior != null)
            emptyInterior.gameObject.SetActive(sprite == null);
    }
    private void OnClickFragmentSlotA()
    {
        CycleFragmentA(1);
    }

    public void CycleFragmentA(int direction)
    {
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        selectedFragmentA = GetAvailableFragment(selectedFragmentA, direction);
        ClearMixPreview();
        RefreshCompositionReadingUI();
    }

    private void OnClickFragmentSlotB()
    {
        CycleFragmentB(1);
    }

    public void CycleFragmentB(int direction)
    {
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        selectedFragmentB = GetAvailableFragment(selectedFragmentB, direction);
        ClearMixPreview();
        RefreshCompositionReadingUI();
    }

    private void OnClickCatalystSlot()
    {
        CycleCatalyst(1);
    }

    public void CycleCatalyst(int direction)
    {
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        selectedCatalyst = GetCatalyst(selectedCatalyst, direction);
        ClearMixPreview();
        RefreshCompositionReadingUI();
    }

    private ExperimentalFragmentType GetAvailableFragment(
        ExperimentalFragmentType current, int direction)
    {
        ExperimentalFragmentType[] order = new ExperimentalFragmentType[]
        {
            ExperimentalFragmentType.None,
            ExperimentalFragmentType.Condensation,
            ExperimentalFragmentType.Confinement,
            ExperimentalFragmentType.ResidualInterference
        };

        int currentIndex = 0;

        for (int i = 0; i < order.Length; i++)
        {
            if (order[i] == current)
            {
                currentIndex = i;
                break;
            }
        }

        int sign = direction < 0 ? -1 : 1;
        for (int step = 1; step <= order.Length; step++)
        {
            int nextIndex = (currentIndex + sign * step) % order.Length;
            if (nextIndex < 0)
                nextIndex += order.Length;
            ExperimentalFragmentType candidate = order[nextIndex];

            if (candidate == ExperimentalFragmentType.None)
                return candidate;

            if (GameState.I.GetFragmentCount(candidate) > 0)
                return candidate;
        }

        return ExperimentalFragmentType.None;
    }

    private ExperimentalCatalystType GetCatalyst(
        ExperimentalCatalystType current, int direction)
    {
        ExperimentalCatalystType[] order =
        {
            ExperimentalCatalystType.None,
            ExperimentalCatalystType.Alpha,
            ExperimentalCatalystType.Beta
        };
        int index = System.Array.IndexOf(order, current);
        if (index < 0) index = 0;
        int sign = direction < 0 ? -1 : 1;
        int next = (index + sign) % order.Length;
        if (next < 0) next += order.Length;
        return order[next];
    }

    private string BuildFragmentSlotText(ExperimentalFragmentType fragmentType)
    {
        string emptyText = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.slot.empty")
            : "Vacío";

        switch (fragmentType)
        {
            case ExperimentalFragmentType.Condensation:
                return BuildFragmentInventoryLabel(
                    fragmentType, GameState.I.fragmentCondensation, "HIGGS");

            case ExperimentalFragmentType.Confinement:
                return BuildFragmentInventoryLabel(
                    fragmentType, GameState.I.fragmentConfinement, "TETRAQUARK");

            case ExperimentalFragmentType.ResidualInterference:
                return BuildFragmentInventoryLabel(
                    fragmentType, GameState.I.fragmentResidualInterference,
                    IsEnglishLanguage() ? "MODULATOR" : "MODULADOR");

            default:
                return emptyText;
        }
    }

    private string BuildFragmentInventoryLabel(
        ExperimentalFragmentType fragmentType, int amount, string source)
    {
        bool active = GameState.I != null &&
            GameState.I.IsExperimentalFragmentProducerActive(fragmentType);
        string cadence = active
            ? GameState.I.GetExperimentalFragmentSecondsPerUnit().ToString("0.#") +
              (IsEnglishLanguage() ? " s/unit" : " s/unidad")
            : (IsEnglishLanguage() ? "inactive" : "inactivo");
        return GetFragmentDisplayName(fragmentType) + " (" + amount + ")\n" +
            source + " · " + cadence;
    }

    private string BuildCatalystSlotText(ExperimentalCatalystType catalystType)
    {
        string noCatalystText = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.slot.no_catalyst")
            : "Sin catalizador";

        switch (catalystType)
        {
            case ExperimentalCatalystType.Alpha:
                return GetCatalystDisplayName(catalystType);

            case ExperimentalCatalystType.Beta:
                return GetCatalystDisplayName(catalystType);

            default:
                return noCatalystText;
        }
    }

    private void ClearMixPreview()
    {
        isShowingLogPreview = false;
        hasCompletedFusion = false;
        lastFusionResult = ExperimentalResultType.None;
        lastRewardAmount = 0;
        lastInstabilityGain = 0;

        if (statusText != null)
        {
            statusText.text = "Resultado del ensayo: —";
        }
    }

    private void OnClickMixButton()
    {
        isShowingLogPreview = false;
        PullFusionRuntimeStateFromGameState();

        if (!CanExecuteFusion)
        {
            hasCompletedFusion = false;
            if (statusText != null)
                statusText.text = FusionStatusTitle;
            return;
        }

        ExperimentalResultType result = D3FusionService.ResolveRecipeResult(
            selectedFragmentA,
            selectedFragmentB,
            selectedCatalyst);

        result = ApplyGuidedSynthesisIntent(result);

        bool synthesisCoreWasCharged = IsSynthesisCoreCharged();

        double failureChanceBeforeStableChamber = GetFusionFailureChanceBeforeStableChamber();

        bool failed = RollFusionFailure();
        bool failureConverted = false;

        if (failed)
        {
            if (TryConvertFailureIntoMinorResult(failureChanceBeforeStableChamber))
            {
                result = ExperimentalResultType.LecturaIncompleta;
                failed = false;
                failureConverted = true;
            }
            else
            {
                result = ExperimentalResultType.None;
            }
        }

        GameState.I.ConsumeFragment(selectedFragmentA, 1);
        GameState.I.ConsumeFragment(selectedFragmentB, 1);

        int rewardAmount = 0;
        if (!failed && result != ExperimentalResultType.None)
        {
            rewardAmount = GetTrialModeRewardAmount();
            rewardAmount += GetUsefulResultRewardBonus();
        }

        GameState.I.pendingFusion = new ExperimentalPendingFusionState
        {
            active = true,
            fragmentA = (int)selectedFragmentA,
            fragmentB = (int)selectedFragmentB,
            catalyst = (int)selectedCatalyst,
            result = (int)result,
            rewardAmount = rewardAmount,
            instabilityGain = GetTrialModeInstabilityGain(),
            failureConverted = failureConverted,
            hadSynthesisCore = HasSynthesisCore(),
            synthesisCoreWasCharged = synthesisCoreWasCharged
        };

        hasCompletedFusion = false;
        lastFusionResult = ExperimentalResultType.None;
        lastRewardAmount = 0;
        lastInstabilityGain = 0;
        currentFusionCooldownSeconds = GetFusionCooldownDuration();
        PushFusionRuntimeStateToGameState();
        RefreshFusionSlotsUI();

        if (statusText != null)
        {
            statusText.text = IsEnglishLanguage()
                ? "FUSION IN PROGRESS · RESULT IN " +
                  currentFusionCooldownSeconds.ToString("0.0") + " s"
                : "FUSIÓN EN PROCESO · RESULTADO EN " +
                  currentFusionCooldownSeconds.ToString("0.0") + " s";
        }

        SaveService.I?.Save();
    }

    private void OnClickLogButton()
    {
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        isShowingLogPreview = true;

        if (logPanel != null)
            logPanel.SetActive(true);

        if (logContentText != null)
            logContentText.text = BuildRecipeLogText();

        GameState.I.MarkExperimentalRecipesRead();
        SaveService.I?.Save();
    }

    private ExperimentalResultType GetResultFromGuidedIntent(GuidedSynthesisIntent intent)
    {
        switch (intent)
        {
            case GuidedSynthesisIntent.Hallazgo:
                return ExperimentalResultType.Hallazgo;

            case GuidedSynthesisIntent.Muestra:
                return ExperimentalResultType.Muestra;

            case GuidedSynthesisIntent.LecturaIncompleta:
                return ExperimentalResultType.LecturaIncompleta;

            case GuidedSynthesisIntent.CompuestoUtil:
                return ExperimentalResultType.CompuestoUtil;

            case GuidedSynthesisIntent.None:
            default:
                return ExperimentalResultType.None;
        }
    }

    private ExperimentalResultType ApplyGuidedSynthesisIntent(ExperimentalResultType baseResult)
    {
        if (!HasGuidedSynthesis())
            return baseResult;

        if (currentGuidedIntent == GuidedSynthesisIntent.None)
            return baseResult;

        if (baseResult == ExperimentalResultType.None)
            return baseResult;

        ExperimentalResultType intendedResult = GetResultFromGuidedIntent(currentGuidedIntent);

        if (intendedResult == ExperimentalResultType.None)
            return baseResult;

        if (Random.value < GetGuidedIntentChance())
            return intendedResult;

        return baseResult;
    }

    private ExperimentalResultType ResolveRecipeResult

    (
    ExperimentalFragmentType fragmentA,
    ExperimentalFragmentType fragmentB,
    ExperimentalCatalystType catalyst)
    {
        if (fragmentA == ExperimentalFragmentType.None ||
            fragmentB == ExperimentalFragmentType.None ||
            catalyst == ExperimentalCatalystType.None)
        {
            return ExperimentalResultType.None;
        }

        int a = (int)fragmentA;
        int b = (int)fragmentB;

        if (a > b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        ExperimentalFragmentType left = (ExperimentalFragmentType)a;
        ExperimentalFragmentType right = (ExperimentalFragmentType)b;

        // Condensación + Condensación
        if (left == ExperimentalFragmentType.Condensation &&
            right == ExperimentalFragmentType.Condensation)
        {
            if (catalyst == ExperimentalCatalystType.Alpha)
                return ExperimentalResultType.CompuestoUtil;

            if (catalyst == ExperimentalCatalystType.Beta)
                return ExperimentalResultType.Muestra;
        }

        // Condensación + Confinamiento
        if (left == ExperimentalFragmentType.Condensation &&
            right == ExperimentalFragmentType.Confinement)
        {
            if (catalyst == ExperimentalCatalystType.Alpha)
                return ExperimentalResultType.Hallazgo;

            if (catalyst == ExperimentalCatalystType.Beta)
                return ExperimentalResultType.LecturaIncompleta;
        }

        // Condensación + Residual
        if (left == ExperimentalFragmentType.Condensation &&
            right == ExperimentalFragmentType.ResidualInterference)
        {
            if (catalyst == ExperimentalCatalystType.Alpha)
                return ExperimentalResultType.Muestra;

            if (catalyst == ExperimentalCatalystType.Beta)
                return ExperimentalResultType.Hallazgo;
        }

        // Confinamiento + Confinamiento
        if (left == ExperimentalFragmentType.Confinement &&
            right == ExperimentalFragmentType.Confinement)
        {
            if (catalyst == ExperimentalCatalystType.Alpha)
                return ExperimentalResultType.CompuestoUtil;

            if (catalyst == ExperimentalCatalystType.Beta)
                return ExperimentalResultType.Muestra;
        }

        // Confinamiento + Residual
        if (left == ExperimentalFragmentType.Confinement &&
            right == ExperimentalFragmentType.ResidualInterference)
        {
            if (catalyst == ExperimentalCatalystType.Alpha)
                return ExperimentalResultType.Hallazgo;

            if (catalyst == ExperimentalCatalystType.Beta)
                return ExperimentalResultType.LecturaIncompleta;
        }

        // Residual + Residual
        if (left == ExperimentalFragmentType.ResidualInterference &&
            right == ExperimentalFragmentType.ResidualInterference)
        {
            if (catalyst == ExperimentalCatalystType.Alpha)
                return ExperimentalResultType.Muestra;

            if (catalyst == ExperimentalCatalystType.Beta)
                return ExperimentalResultType.LecturaIncompleta;
        }

        return ExperimentalResultType.None;
    }

    private string BuildResultMessage(ExperimentalResultType result)
    {
        string resultLabel = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.result.label")
            : "Resultado del ensayo:";

        string rewardLabel = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.result.reward")
            : "Recompensa:";

        if (result == ExperimentalResultType.None)
        {
            string failedText = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.result.failed")
                : "Fallo inestable";

            return resultLabel + " " + failedText;
        }

            int rewardAmount = lastRewardAmount > 0 ? lastRewardAmount : GetTrialModeRewardAmount();
            string resultName = GetResultDisplayName(result);
            string rewardName = GetResultDisplayName(result, rewardAmount != 1);

            return resultLabel + " " + resultName + " | " + rewardLabel + " +" + rewardAmount + " " + rewardName;
    }

    

    private bool HasRequiredFragmentsForCurrentSelection()
    {
        return D3FusionService.HasRequiredFragments(
            GameState.I, selectedFragmentA, selectedFragmentB);
    }

    private void RegisterCurrentMixResult(ExperimentalResultType result)
    {
        if (GameState.I == null)
            return;

        D3FusionService.RegisterMixResult(
            GameState.I, selectedFragmentA, selectedFragmentB,
            selectedCatalyst, result);
    }

        private List<(ExperimentalFragmentType fragmentA, ExperimentalFragmentType fragmentB, ExperimentalCatalystType catalyst)> GetAllBaseRecipes()
    {
        return new List<(ExperimentalFragmentType, ExperimentalFragmentType, ExperimentalCatalystType)>
        {
            (ExperimentalFragmentType.Condensation, ExperimentalFragmentType.Condensation, ExperimentalCatalystType.Alpha),
            (ExperimentalFragmentType.Condensation, ExperimentalFragmentType.Condensation, ExperimentalCatalystType.Beta),

            (ExperimentalFragmentType.Condensation, ExperimentalFragmentType.Confinement, ExperimentalCatalystType.Alpha),
            (ExperimentalFragmentType.Condensation, ExperimentalFragmentType.Confinement, ExperimentalCatalystType.Beta),

            (ExperimentalFragmentType.Condensation, ExperimentalFragmentType.ResidualInterference, ExperimentalCatalystType.Alpha),
            (ExperimentalFragmentType.Condensation, ExperimentalFragmentType.ResidualInterference, ExperimentalCatalystType.Beta),

            (ExperimentalFragmentType.Confinement, ExperimentalFragmentType.Confinement, ExperimentalCatalystType.Alpha),
            (ExperimentalFragmentType.Confinement, ExperimentalFragmentType.Confinement, ExperimentalCatalystType.Beta),

            (ExperimentalFragmentType.Confinement, ExperimentalFragmentType.ResidualInterference, ExperimentalCatalystType.Alpha),
            (ExperimentalFragmentType.Confinement, ExperimentalFragmentType.ResidualInterference, ExperimentalCatalystType.Beta),

            (ExperimentalFragmentType.ResidualInterference, ExperimentalFragmentType.ResidualInterference, ExperimentalCatalystType.Alpha),
            (ExperimentalFragmentType.ResidualInterference, ExperimentalFragmentType.ResidualInterference, ExperimentalCatalystType.Beta),
        };
    }

    private string GetFragmentDisplayName(ExperimentalFragmentType fragment)
    {
        bool english = IsEnglishLanguage();
        switch (fragment)
        {
            case ExperimentalFragmentType.Condensation:
                return english ? "Condensation" : "Condensación";

            case ExperimentalFragmentType.Confinement:
                return english ? "Confinement" : "Confinamiento";

            case ExperimentalFragmentType.ResidualInterference:
                return english ? "Residual" : "Residual";

            default:
                return english ? "None" : "Ninguno";
        }
    }

    private string GetCatalystDisplayName(ExperimentalCatalystType catalyst)
    {
        switch (catalyst)
        {
            case ExperimentalCatalystType.Alpha:
                return "Alpha";

            case ExperimentalCatalystType.Beta:
                return "Beta";

            default:
                return "Ninguno";
        }
    }

    private string GetResultDisplayName(ExperimentalResultType result, bool plural = false)
    {
        bool english = IsEnglishLanguage();
        switch (result)
        {
            case ExperimentalResultType.Hallazgo:
                return english
                    ? plural ? "Anomalies" : "Anomaly"
                    : plural ? "Anomalías" : "Anomalía";

            case ExperimentalResultType.Muestra:
                return english
                    ? plural ? "Condensates" : "Condensate"
                    : plural ? "Condensados" : "Condensado";

            case ExperimentalResultType.LecturaIncompleta:
                return english
                    ? plural ? "Vestiges" : "Vestige"
                    : plural ? "Vestigios" : "Vestigio";

            case ExperimentalResultType.CompuestoUtil:
                return english
                    ? plural ? "Compounds" : "Compound"
                    : plural ? "Compuestos" : "Compuesto";

            default:
                return "???";
        }
    }

    private string BuildRewardDisplayText(ExperimentalResultType result, int amount)
    {
        if (result == ExperimentalResultType.None || amount <= 0)
            return IsEnglishLanguage() ? "No recoverable result" : "Sin resultado recuperable";

        return "+" + amount + " " + GetResultDisplayName(result, amount != 1);
    }

    private string BuildRecipeLogLine(
        ExperimentalFragmentType fragmentA,
        ExperimentalFragmentType fragmentB,
        ExperimentalCatalystType catalyst,
        bool discovered)
    {
        string left = GetFragmentDisplayName(fragmentA);
        string right = GetFragmentDisplayName(fragmentB);
        string catalystName = GetCatalystDisplayName(catalyst);

        string resultText = "???";
        string rewardText = "???";

        if (discovered)
        {
            ExperimentalResultType result = ResolveRecipeResult(fragmentA, fragmentB, catalyst);
            resultText = GetResultDisplayName(result);
            rewardText = "+1 " + resultText;
        }

        return left + " + " + right + " + " + catalystName +
            " = " + resultText +
            (IsEnglishLanguage() ? " | Reward: " : " | Recompensa: ") + rewardText;
    }

    private string BuildRecipeLogText()
    {
        if (GameState.I == null)
            return "Log no disponible";

        var recipes = GetAllBaseRecipes();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < recipes.Count; i++)
        {
            var recipe = recipes[i];

            string mixKey = GameState.I.BuildExperimentalMixKey(
                recipe.fragmentA,
                recipe.fragmentB,
                recipe.catalyst);

            bool discovered = false;

            if (GameState.I.experimentalMixLog != null)
            {
                foreach (var entry in GameState.I.experimentalMixLog)
                {
                    if (entry != null && entry.mixKey == mixKey)
                    {
                        discovered = entry.discovered;
                        break;
                    }
                }
            }

            sb.Append(BuildRecipeLogLine(
                recipe.fragmentA,
                recipe.fragmentB,
                recipe.catalyst,
                discovered));

            if (i < recipes.Count - 1)
                sb.AppendLine();
        }

        return sb.ToString();
    }
}
