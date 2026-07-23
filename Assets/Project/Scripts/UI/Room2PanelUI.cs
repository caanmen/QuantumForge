using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Room2PanelUI : MonoBehaviour
{
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

    [SerializeField] private TextMeshProUGUI fragmentSlotAText;
    [SerializeField] private TextMeshProUGUI fragmentSlotBText;
    [SerializeField] private TextMeshProUGUI catalystSlotText;
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
    private const int instabilityLowMax = 5;
    private const int instabilityMediumMax = 12;
    private const int coolTraceCost = 30;
    private const int coolInstabilityReduction = 5;
    private int lastLocalizationRevision = -1;
    private bool isShowingLogPreview = false;

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

    private void Awake()
    {
        if (fragmentSlotAButton != null)
            fragmentSlotAButton.onClick.AddListener(OnClickFragmentSlotA);

        if (fragmentSlotBButton != null)
            fragmentSlotBButton.onClick.AddListener(OnClickFragmentSlotB);

        if (catalystSlotButton != null)
            catalystSlotButton.onClick.AddListener(OnClickCatalystSlot);

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

        RefreshInstabilityUI();
        RefreshTrialModeUI();
        RefreshGuidedSynthesisUI();
        RefreshCoolButtonUI();
        RefreshStaticRoom2Texts();

    }

    private void OnEnable()
    {
        LoadGuidedIntentFromGameState();
        RefreshGuidedSynthesisUI();
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
        return currentFusionCooldownSeconds > 0.0;
    }

    private void RefreshFusionSlotsUI()
    {
        if (fusionSlotsText == null)
            return;

        int slots = GetUnlockedFusionSlotCount();

        fusionSlotsText.text = "Ranuras de fusión: " + slots;

        if (IsFusionCoolingDown())
        {
            fusionSlotsText.text += "\nEnfriamiento de fusión: " +
                currentFusionCooldownSeconds.ToString("0.0") + "s";
        }
        else
        {
            fusionSlotsText.text += "\nFusión lista";
        }

        if (HasSynthesisCore() && GameState.I != null)
        {
            int counter = GameState.I.synthesisCoreFusionCounter;

            if (counter >= 10)
            {
                fusionSlotsText.text += "\nNúcleo de Síntesis: cargado";
            }
            else
            {
                fusionSlotsText.text += "\nNúcleo de Síntesis: " + counter + "/10";
            }
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

        if (!HasCompositionReading())
        {
            compositionReadingText.text =
                "Lectura de composición: bloqueada\n" +
                "Resultado probable: ???\n" +
                "Riesgo estimado: ???";
            return;
        }

        string probableResultText = "???";


        if (selectedFragmentA != ExperimentalFragmentType.None &&
            selectedFragmentB != ExperimentalFragmentType.None &&
            selectedCatalyst != ExperimentalCatalystType.None &&
                HasCurrentRecipeBeenDiscovered())
        {
            ExperimentalResultType probableResult = ResolveRecipeResult(
                selectedFragmentA,
                selectedFragmentB,
                selectedCatalyst);

            probableResultText = GetResultDisplayName(probableResult);
        }

        int riskPercent = Mathf.RoundToInt((float)(GetCurrentFusionFailureChance() * 100.0));

        compositionReadingText.text =
            "Lectura de composición: activa\n" +
            "Resultado probable: " + probableResultText + "\n" +
            "Riesgo estimado: " + riskPercent + "%";
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
            introText.text = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.intro")
                : "Selecciona dos fragmentos, elige un catalizador y ejecuta un ensayo.";
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

        if (currentInstability <= 0)
            return;

        if (GameState.I.Traces < coolTraceCost)
            return;

        GameState.I.Traces -= coolTraceCost;

        currentInstability -= coolInstabilityReduction;

        if (currentInstability < 0)
            currentInstability = 0;

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

    private double GetCurrentFusionFailureChance()
    {
        double failureChance = GetRecipeBaseFailureChance();

        switch (currentTrialMode)
        {
            case TrialMode.Safe:
                failureChance += 0.00;
                break;

            case TrialMode.Forced:
                failureChance += 0.10;
                break;

            case TrialMode.Balanced:
            default:
                failureChance += 0.05;
                break;
        }

        if (currentInstability > instabilityMediumMax)
        {
            failureChance += 0.10;
        }
        else if (currentInstability > instabilityLowMax)
        {
            failureChance += 0.05;
        }

        if (MachineManager.I != null)
        {
            failureChance -= MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.FusionFailureReduction);
        }

        if (HasCatalystTuning() && selectedCatalyst != ExperimentalCatalystType.None)
        {
            failureChance -= 0.03;
        }

        failureChance = Mathf.Clamp01((float)failureChance);
        failureChance = ApplyStableReactionChamberReduction(failureChance);

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
        double failureChance = GetRecipeBaseFailureChance();

        switch (currentTrialMode)
        {
            case TrialMode.Safe:
                failureChance += 0.00;
                break;

            case TrialMode.Forced:
                failureChance += 0.10;
                break;

            case TrialMode.Balanced:
            default:
                failureChance += 0.05;
                break;
        }

        if (currentInstability > instabilityMediumMax)
        {
            failureChance += 0.10;
        }
        else if (currentInstability > instabilityLowMax)
        {
            failureChance += 0.05;
        }

        if (MachineManager.I != null)
        {
            failureChance -= MachineManager.I.GetTotalEffectValue(MachineNodeEffectType.FusionFailureReduction);
        }


        return Mathf.Clamp01((float)failureChance);
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

        if (HasCatalystTuning() && selectedCatalyst != ExperimentalCatalystType.None)
        {
            bonusChance += 0.03;
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

    private bool IsStrongFusionRisk(double failureChance)
    {
        return failureChance >= 0.30;
    }

    private double ApplyStableReactionChamberReduction(double failureChance)
    {
        if (!HasStableReactionChamber())
            return failureChance;

        if (!IsStrongFusionRisk(failureChance))
            return failureChance;

        return failureChance * 0.80;
    }

    private bool TryConvertFailureIntoMinorResult(double failureChanceBeforeStableChamber)
    {
        if (!HasStableReactionChamber())
            return false;

        if (!IsStrongFusionRisk(failureChanceBeforeStableChamber))
            return false;

        return Random.value < 0.25f;
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
            instabilityLabel + " " + currentInstability +
            "\n" +
            stateLabel + " " + GetInstabilityStateName();
    }

    private void RefreshCoolButtonUI()
    {
        if (coolButtonText != null)
        {
            string coolLabel = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.cool.button")
                : "Enfriar";

            coolButtonText.text = coolLabel + " (" + coolTraceCost + " Trazas)";
        }

        if (coolButton != null)
        {
            bool canCool =
                GameState.I != null &&
                currentInstability > 0 &&
                GameState.I.Traces >= coolTraceCost;

            coolButton.interactable = canCool;
        }
    }

    private string GetGuidedIntentDisplayName()
    {
        switch (currentGuidedIntent)
        {
            case GuidedSynthesisIntent.Hallazgo:
                return "Hallazgo";

            case GuidedSynthesisIntent.Muestra:
                return "Muestra";

            case GuidedSynthesisIntent.LecturaIncompleta:
                return "Lectura incompleta";

            case GuidedSynthesisIntent.CompuestoUtil:
                return "Compuesto útil";

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
                ? "Cambiar intención"
                : "Bloqueado";
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

        string buttonLabel = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.mode.button")
            : "Cambiar modo";

        if (modeText != null)
            modeText.text = modeLabel + "\n" + modeName;

        if (modeButtonText != null)
            modeButtonText.text = buttonLabel;
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
        if (currentFusionCooldownSeconds > 0.0)
        {
            currentFusionCooldownSeconds -= Time.deltaTime;

            if (currentFusionCooldownSeconds < 0.0)
                currentFusionCooldownSeconds = 0.0;
        }

        RefreshLocalizationIfNeeded();
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (GameState.I == null) return;

        bool unlocked = GameState.I.experimentalChamberUnlocked;

        if (closedBlock != null) closedBlock.SetActive(!unlocked);
        if (openedBlock != null) openedBlock.SetActive(unlocked);

        if (machinePanelRoot != null)
        {
            machinePanelRoot.SetActive(unlocked);
        }

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
            if (statusText != null)
            statusText.text = "Cámara bloqueada";

            return;
        }

        if (introText != null)
        {
            introText.text = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.intro")
                : "Selecciona dos fragmentos, elige un catalizador y ejecuta un ensayo.";
        }

        if (statusText != null)
        {
        if (statusText != null)
        {
            if (statusText != null && !isShowingLogPreview)
            {
                if (selectedFragmentA == ExperimentalFragmentType.None &&
                    selectedFragmentB == ExperimentalFragmentType.None &&
                    selectedCatalyst == ExperimentalCatalystType.None)
                {
                statusText.text = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.status.select_all")
                : "Selecciona fragmentos y catalizador";                }
                else if (selectedFragmentA == ExperimentalFragmentType.None)
                {
                statusText.text = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.status.missing_a")
                : "Falta seleccionar Fragmento A";                
                }
                else if (selectedFragmentB == ExperimentalFragmentType.None)
                {
                    statusText.text = "Falta seleccionar Fragmento B";
                }
                else if (selectedCatalyst == ExperimentalCatalystType.None)
                {
                    statusText.text = "Falta seleccionar catalizador";
                }
            }
        }

        if (fragmentSlotAText != null)
            fragmentSlotAText.text = BuildFragmentSlotText(selectedFragmentA);

        if (fragmentSlotBText != null)
            fragmentSlotBText.text = BuildFragmentSlotText(selectedFragmentB);

        if (catalystSlotText != null)
            catalystSlotText.text = BuildCatalystSlotText(selectedCatalyst);
    }
    }
    private void OnClickFragmentSlotA()
    {
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        selectedFragmentA = GetNextAvailableFragment(selectedFragmentA);
        ClearMixPreview();
        RefreshCompositionReadingUI();
    }

    private void OnClickFragmentSlotB()
    {
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        selectedFragmentB = GetNextAvailableFragment(selectedFragmentB);
        ClearMixPreview();
        RefreshCompositionReadingUI();
    }

    private void OnClickCatalystSlot()
    {
        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        selectedCatalyst = GetNextCatalyst(selectedCatalyst);
        ClearMixPreview();
        RefreshCompositionReadingUI();
    }

    private ExperimentalFragmentType GetNextAvailableFragment(ExperimentalFragmentType current)
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

        for (int step = 1; step <= order.Length; step++)
        {
            int nextIndex = (currentIndex + step) % order.Length;
            ExperimentalFragmentType candidate = order[nextIndex];

            if (candidate == ExperimentalFragmentType.None)
                return candidate;

            if (GameState.I.GetFragmentCount(candidate) > 0)
                return candidate;
        }

        return ExperimentalFragmentType.None;
    }

    private ExperimentalCatalystType GetNextCatalyst(ExperimentalCatalystType current)
    {
        switch (current)
        {
            case ExperimentalCatalystType.None:
                return ExperimentalCatalystType.Alpha;

            case ExperimentalCatalystType.Alpha:
                return ExperimentalCatalystType.Beta;

            default:
                return ExperimentalCatalystType.None;
        }
    }

    private string BuildFragmentSlotText(ExperimentalFragmentType fragmentType)
    {
        string emptyText = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.slot.empty")
            : "Vacío";

        switch (fragmentType)
        {
            case ExperimentalFragmentType.Condensation:
                return GetFragmentDisplayName(fragmentType) + " (" + GameState.I.fragmentCondensation + ")";

            case ExperimentalFragmentType.Confinement:
                return GetFragmentDisplayName(fragmentType) + " (" + GameState.I.fragmentConfinement + ")";

            case ExperimentalFragmentType.ResidualInterference:
                return GetFragmentDisplayName(fragmentType) + " (" + GameState.I.fragmentResidualInterference + ")";

            default:
                return emptyText;
        }
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

        if (statusText != null)
        {
            statusText.text = "Resultado del ensayo: —";
        }
    }

    private void OnClickMixButton()
    {
        isShowingLogPreview = false;

        if (GameState.I == null || !GameState.I.experimentalChamberUnlocked)
            return;

        if (statusText == null)
            return;

        if (IsFusionCoolingDown())
        {
            statusText.text =
                "La cámara de fusión se está enfriando. Tiempo restante: " +
                currentFusionCooldownSeconds.ToString("0.0") + "s";

            return;
        }

        if (GetUnlockedFusionSlotCount() <= 0)
        {
            statusText.text = "Repara la Mesa de Fusión para habilitar los ensayos.";
            return;
        }

        if (selectedFragmentA == ExperimentalFragmentType.None)
        {
        statusText.text = LocalizationManager.I != null
        ? LocalizationManager.I.T("room2.status.missing_a")
        : "Falta seleccionar Fragmento A";    
            return;
        }

        if (selectedFragmentB == ExperimentalFragmentType.None)
        {
        statusText.text = LocalizationManager.I != null
        ? LocalizationManager.I.T("room2.status.missing_b")
        : "Falta seleccionar Fragmento B";            return;
        }

        if (selectedCatalyst == ExperimentalCatalystType.None)
        {
        statusText.text = LocalizationManager.I != null
        ? LocalizationManager.I.T("room2.status.missing_catalyst")
        : "Falta seleccionar catalizador"; 
            return;
        }

        if (!HasRequiredFragmentsForCurrentSelection())
        {
        statusText.text = LocalizationManager.I != null
        ? LocalizationManager.I.T("room2.status.not_enough_fragments")
        : "No hay fragmentos suficientes para este ensayo";            return;
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

        lastRewardAmount = 0;

        if (!failed && result != ExperimentalResultType.None)
        {
            int rewardAmount = GetTrialModeRewardAmount();
            rewardAmount += GetUsefulResultRewardBonus();

            lastRewardAmount = rewardAmount;
            GameState.I.AddExperimentalResult(result, rewardAmount);
        }

        RegisterCurrentMixResult(result);
        D3DiagnosticSystem.RegisterManualFusionRecipe(
            GameState.I, selectedFragmentA, selectedFragmentB, selectedCatalyst);

        if (HasSynthesisCore())
        {
            if (synthesisCoreWasCharged)
            {
                GameState.I.synthesisCoreFusionCounter = 0;
            }
            else
            {
                GameState.I.synthesisCoreFusionCounter++;
            }
        }

        currentInstability += GetTrialModeInstabilityGain();
        currentFusionCooldownSeconds = GetFusionCooldownDuration();
        RefreshInstabilityUI();
        RefreshCoolButtonUI();
        RefreshFusionSlotsUI();

        if (statusText != null)
        {
            statusText.text = BuildResultMessage(result);

            if (failureConverted)
            {
                statusText.text += "\nFallo fuerte estabilizado por la Cámara de Reacción.";
            }

            if (synthesisCoreWasCharged)
            {
                statusText.text += "\nNúcleo de Síntesis descargado: fusión reforzada.";
            }
        }
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

        string noneText = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.result.none")
            : "Sin resultado";

        string rewardLabel = LocalizationManager.I != null
            ? LocalizationManager.I.T("room2.result.reward")
            : "Reward:";

        if (result == ExperimentalResultType.None)
        {
            string failedText = LocalizationManager.I != null
                ? LocalizationManager.I.T("room2.result.failed")
                : "Fallo inestable";

            return resultLabel + " " + failedText;
        }

            int rewardAmount = lastRewardAmount > 0 ? lastRewardAmount : GetTrialModeRewardAmount();
            string resultName = GetResultDisplayName(result);

            return resultLabel + " " + resultName + " | " + rewardLabel + " +" + rewardAmount + " " + resultName;
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
        switch (fragment)
        {
            case ExperimentalFragmentType.Condensation:
                return "Condensación";

            case ExperimentalFragmentType.Confinement:
                return "Confinamiento";

            case ExperimentalFragmentType.ResidualInterference:
                return "Residual";

            default:
                return "Ninguno";
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

    private string GetResultDisplayName(ExperimentalResultType result)
    {
        switch (result)
        {
            case ExperimentalResultType.Hallazgo:
                return "Hallazgo";

            case ExperimentalResultType.Muestra:
                return "Muestra";

            case ExperimentalResultType.LecturaIncompleta:
                return "Lectura incompleta";

            case ExperimentalResultType.CompuestoUtil:
                return "Compuesto útil";

            default:
                return "???";
        }
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
            " | Reward: " + rewardText;
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
