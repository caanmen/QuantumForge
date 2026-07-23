using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class D3CalibrationPanelUI : MonoBehaviour
{
    public GameObject factoryRoot;
    public TMP_Dropdown partDropdown;
    public TMP_Dropdown mkDropdown;
    public TMP_Dropdown quantityDropdown;
    public TMP_Text instructionsText;
    public TMP_Text valueALabel;
    public TMP_Text valueBLabel;
    public TMP_Text valueCLabel;
    public Slider valueASlider;
    public Slider valueBSlider;
    public Slider valueCSlider;
    public TMP_Dropdown optionADropdown;
    public TMP_Dropdown optionBDropdown;
    public TMP_Dropdown optionCDropdown;
    public Button recordPartButton;
    public Button saveProfileButton;
    public Button loadProfileButton;
    public Button autoRepeatPartButton;
    public Button autoRepeatAllButton;
    public Button queueTraitAssemblyButton;
    public Button backButton;
    public TMP_Text readingsText;
    public TMP_Text previewText;
    public TMP_Text noticeText;

    private readonly List<D3CalibrationControlState> _controls =
        new List<D3CalibrationControlState>();
    private readonly HashSet<string> _recordedParts = new HashSet<string>();
    private float _refreshRemaining;

    private void Awake()
    {
        ConfigureDropdown(partDropdown,
            new[] { "Chasis", "Sistema Motriz", "Herramienta", "Módulo de Control", "Regulador" });
        ConfigureDropdown(mkDropdown, new[] { "MK1", "MK2", "MK3", "MK4", "MK5", "MK6" });
        ConfigureDropdown(quantityDropdown, new[] { "Cantidad 1", "Cantidad 2", "Cantidad 3", "Cantidad 4", "Cantidad 5" });
        if (partDropdown != null) partDropdown.onValueChanged.AddListener(OnPartChanged);
        AddListener(recordPartButton, RecordSelectedPart);
        AddListener(saveProfileButton, SaveProfile);
        AddListener(loadProfileButton, LoadProfile);
        AddListener(autoRepeatPartButton, AutoRepeatSelectedPart);
        AddListener(autoRepeatAllButton, AutoRepeatAllParts);
        AddListener(queueTraitAssemblyButton, QueueTraitAssembly);
        AddListener(backButton, Close);
        ConfigureSelectedMinigame();
    }

    private void OnEnable()
    {
        ConfigureSelectedMinigame();
        Refresh();
    }

    private void Update()
    {
        _refreshRemaining -= Time.unscaledDeltaTime;
        if (_refreshRemaining > 0f) return;
        _refreshRemaining = 0.2f;
        Refresh();
    }

    public void Open()
    {
        if (factoryRoot != null) factoryRoot.SetActive(false);
        gameObject.SetActive(true);
        ConfigureSelectedMinigame();
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (factoryRoot != null) factoryRoot.SetActive(true);
    }

    private void OnPartChanged(int ignored)
    {
        ConfigureSelectedMinigame();
        Refresh();
    }

    private void ConfigureSelectedMinigame()
    {
        string partId = GetSelectedPartId();
        SetSlider(valueASlider, 0, 100);
        SetSlider(valueBSlider, 0, 100);
        SetSlider(valueCSlider, 0, 100);
        SetActive(valueASlider, true);
        SetActive(valueBSlider, true);
        SetActive(valueCSlider, true);
        SetActive(optionADropdown, false);
        SetActive(optionBDropdown, false);
        SetActive(optionCDropdown, false);

        if (partId == Dimension3Catalog.PartChassis)
        {
            SetInstructions("Distribuye exactamente 100 puntos entre las tres zonas.");
            SetValueLabels("Superior", "Central", "Inferior");
        }
        else if (partId == Dimension3Catalog.PartMotor)
        {
            SetInstructions("Ajusta el patrón de movimiento; ningún valor es universalmente mejor.");
            SetValueLabels("Frecuencia", "Empuje", "Recuperación");
        }
        else if (partId == Dimension3Catalog.PartTool)
        {
            SetInstructions("Elige una zona de trabajo y ajusta su intensidad.");
            SetValueLabels("Intensidad", "", "");
            SetActive(valueBSlider, false);
            SetActive(valueCSlider, false);
            ConfigureDropdown(optionADropdown,
                new[] { "Precisión", "Presión", "Ritmo", "Ajuste" });
            SetActive(optionADropdown, true);
        }
        else if (partId == Dimension3Catalog.PartControl)
        {
            SetInstructions("Asigna una vez cada nodo 1–3 y cada salida X–Z a las entradas A–C.");
            SetValueLabels("Nodo para A", "Nodo para B", "Nodo para C");
            SetSlider(valueASlider, 0, 2);
            SetSlider(valueBSlider, 0, 2);
            SetSlider(valueCSlider, 0, 2);
            ConfigureDropdown(optionADropdown, new[] { "Salida X", "Salida Y", "Salida Z" });
            ConfigureDropdown(optionBDropdown, new[] { "Salida X", "Salida Y", "Salida Z" });
            ConfigureDropdown(optionCDropdown, new[] { "Salida X", "Salida Y", "Salida Z" });
            SetActive(optionADropdown, true);
            SetActive(optionBDropdown, true);
            SetActive(optionCDropdown, true);
        }
        else
        {
            SetInstructions("Ajusta Voltaje y Resistencia para definir el flujo energético.");
            SetValueLabels("Voltaje", "Resistencia", "");
            SetActive(valueCSlider, false);
        }

        D3CalibrationControlState stored = FindControls(partId);
        ApplyControlsToWidgets(stored ?? CreateDefaultControls(partId));
    }

    private void RecordSelectedPart()
    {
        D3CalibrationControlState controls = ReadWidgets();
        int reading;
        string reason;
        if (!D3CalibrationSystem.TryCalculateReading(controls, out reading, out reason))
        {
            SetNotice(reason);
            return;
        }
        ReplaceControls(controls);
        _recordedParts.Add(controls.partId);
        SetNotice(GetPartDisplayName(controls.partId) + " registrada con lectura " + reading + ".");
        Refresh();
    }

    private void SaveProfile()
    {
        string reason;
        if (D3CalibrationSystem.TrySaveProfile(
                GameState.I, "Perfil de Fábrica", _controls, out reason))
            SetNotice("Configuración completa guardada.");
        else SetNotice(reason);
        Refresh();
    }

    private void LoadProfile()
    {
        List<D3CalibrationControlState> loaded;
        string reason;
        if (!D3CalibrationSystem.TryLoadProfile(GameState.I, out loaded, out reason))
        {
            SetNotice(reason);
            return;
        }
        _controls.Clear();
        _recordedParts.Clear();
        for (int i = 0; i < loaded.Count; i++)
            _controls.Add(D3CalibrationSystem.CloneControls(loaded[i]));
        ApplyControlsToWidgets(FindControls(GetSelectedPartId()));
        SetNotice("Perfil cargado. Puedes revisar y repetir cada pieza.");
        Refresh();
    }

    private void AutoRepeatSelectedPart()
    {
        if (!D3CalibrationSystem.CanAutoRepeatOnePiece(GameState.I))
        {
            SetNotice("Repetir una pieza automáticamente requiere Banco nivel 3.");
            return;
        }
        List<D3CalibrationControlState> loaded;
        string reason;
        if (!D3CalibrationSystem.TryLoadProfile(GameState.I, out loaded, out reason))
        {
            SetNotice(reason);
            return;
        }
        string partId = GetSelectedPartId();
        D3CalibrationControlState selected = FindControls(loaded, partId);
        if (selected == null)
        {
            SetNotice("El perfil no contiene esa pieza.");
            return;
        }
        ReplaceControls(selected);
        _recordedParts.Add(partId);
        ApplyControlsToWidgets(selected);
        SetNotice(GetPartDisplayName(partId) + " repetida automáticamente.");
        Refresh();
    }

    private void AutoRepeatAllParts()
    {
        if (D3FacilitySystem.GetProcessBankLevel(GameState.I.dimension3) < 4)
        { SetNotice("Repetir las cinco piezas requiere Banco nivel 4."); return; }
        List<D3CalibrationControlState> loaded; string reason;
        if (!D3CalibrationSystem.TryLoadProfile(GameState.I, out loaded, out reason))
        { SetNotice(reason); return; }
        _controls.Clear(); _recordedParts.Clear();
        for (int i = 0; i < loaded.Count; i++)
        { ReplaceControls(loaded[i]); _recordedParts.Add(loaded[i].partId); }
        ApplyControlsToWidgets(FindControls(GetSelectedPartId()));
        SetNotice("Las cinco piezas se repitieron automáticamente."); Refresh();
    }

    private void QueueTraitAssembly()
    {
        List<D3CalibrationReadingState> readings = BuildReadings();
        string reason;
        int mk = mkDropdown == null ? 1 : mkDropdown.value + 1;
        long quantity = quantityDropdown == null ? 1L : quantityDropdown.value + 1L;
        if (Dimension3System.TryQueueTraitAssembly(GameState.I, mk, quantity, readings, out reason))
        {
            D3CalibrationEvaluation evaluation = D3CalibrationSystem.Evaluate(readings);
            SetNotice("Intento MK" + mk + " confirmado: resultado previsto " +
                GetTraitDisplayName(evaluation.resultTraitId) + ".");
        }
        else SetNotice(reason);
        Refresh();
    }

    private void Refresh()
    {
        if (GameState.I == null || GameState.I.dimension3 == null) return;
        List<D3CalibrationReadingState> readings = BuildReadings();
        if (readingsText != null)
        {
            var builder = new StringBuilder("LECTURAS\n");
            for (int i = 0; i < Dimension3Catalog.PartIds.Length; i++)
            {
                string partId = Dimension3Catalog.PartIds[i];
                D3CalibrationReadingState reading = FindReading(readings, partId);
                builder.Append(GetPartDisplayName(partId));
                builder.Append(": ");
                builder.AppendLine(reading == null ? "Pendiente" : reading.reading.ToString());
            }
            readingsText.text = builder.ToString();
        }
        D3CalibrationEvaluation evaluation = D3CalibrationSystem.Evaluate(readings);
        if (previewText != null)
        {
            previewText.text = evaluation.calibratedCount == 0
                ? "PREVIEW\nCalibra al menos una pieza."
                : "PREVIEW\nCandidato: " + GetTraitDisplayName(evaluation.candidateTraitId) +
                  "\nAfinidad: " + evaluation.affinity.ToString("0.##") + "% (" +
                  evaluation.GetAffinityBand() + ")\nResultado actual: " +
                  GetTraitDisplayName(evaluation.resultTraitId);
        }
        int level = D3FacilitySystem.GetProcessBankLevel(GameState.I.dimension3);
        SetInteractable(saveProfileButton, readings.Count == 5 && level >= 1);
        SetInteractable(loadProfileButton,
            level >= 2 && GameState.I.dimension3.calibrationProfiles.Count > 0);
        SetInteractable(autoRepeatPartButton,
            level >= 3 && GameState.I.dimension3.calibrationProfiles.Count > 0);
        SetInteractable(autoRepeatAllButton,
            level >= 4 && GameState.I.dimension3.calibrationProfiles.Count > 0);
        if (quantityDropdown != null) quantityDropdown.interactable = level >= 5;
        int mk = mkDropdown == null ? 1 : mkDropdown.value + 1;
        SetInteractable(queueTraitAssemblyButton,
            readings.Count == 5 && D3AssemblySystem.IsNormalMkUnlocked(GameState.I, mk));
        UpdateLiveLabels();
    }

    private void UpdateLiveLabels()
    {
        string partId = GetSelectedPartId();
        int displayOffset = partId == Dimension3Catalog.PartControl ? 1 : 0;
        if (valueALabel != null)
            valueALabel.text = StripValue(valueALabel.text) + ": " +
                ((int)valueASlider.value + displayOffset);
        if (valueBLabel != null && valueBSlider.gameObject.activeSelf)
            valueBLabel.text = StripValue(valueBLabel.text) + ": " +
                ((int)valueBSlider.value + displayOffset);
        if (valueCLabel != null && valueCSlider.gameObject.activeSelf)
            valueCLabel.text = StripValue(valueCLabel.text) + ": " +
                ((int)valueCSlider.value + displayOffset);
        if (partId == Dimension3Catalog.PartChassis && instructionsText != null)
        {
            int sum = (int)valueASlider.value + (int)valueBSlider.value +
                (int)valueCSlider.value;
            instructionsText.text = "Distribuye exactamente 100 puntos. Total actual: " + sum + ".";
        }
    }

    private List<D3CalibrationReadingState> BuildReadings()
    {
        var readings = new List<D3CalibrationReadingState>();
        for (int i = 0; i < _controls.Count; i++)
        {
            if (!_recordedParts.Contains(_controls[i].partId)) continue;
            int reading;
            string reason;
            if (D3CalibrationSystem.TryCalculateReading(
                    _controls[i], out reading, out reason))
                readings.Add(new D3CalibrationReadingState
                    { partId = _controls[i].partId, reading = reading });
        }
        return readings;
    }

    private D3CalibrationControlState ReadWidgets()
    {
        return new D3CalibrationControlState
        {
            partId = GetSelectedPartId(),
            valueA = valueASlider == null ? 0 : (int)Math.Round(valueASlider.value),
            valueB = valueBSlider == null ? 0 : (int)Math.Round(valueBSlider.value),
            valueC = valueCSlider == null ? 0 : (int)Math.Round(valueCSlider.value),
            optionA = optionADropdown == null ? 0 : optionADropdown.value,
            optionB = optionBDropdown == null ? 0 : optionBDropdown.value,
            optionC = optionCDropdown == null ? 0 : optionCDropdown.value
        };
    }

    private void ApplyControlsToWidgets(D3CalibrationControlState controls)
    {
        if (controls == null) return;
        if (valueASlider != null) valueASlider.value = controls.valueA;
        if (valueBSlider != null) valueBSlider.value = controls.valueB;
        if (valueCSlider != null) valueCSlider.value = controls.valueC;
        if (optionADropdown != null) optionADropdown.value = controls.optionA;
        if (optionBDropdown != null) optionBDropdown.value = controls.optionB;
        if (optionCDropdown != null) optionCDropdown.value = controls.optionC;
    }

    private static D3CalibrationControlState CreateDefaultControls(string partId)
    {
        var controls = new D3CalibrationControlState { partId = partId };
        if (partId == Dimension3Catalog.PartChassis)
        {
            controls.valueA = 33; controls.valueB = 34; controls.valueC = 33;
        }
        else if (partId == Dimension3Catalog.PartMotor)
        {
            controls.valueA = controls.valueB = controls.valueC = 50;
        }
        else if (partId == Dimension3Catalog.PartTool)
        {
            controls.valueA = 50;
        }
        else if (partId == Dimension3Catalog.PartControl)
        {
            controls.valueA = 0; controls.valueB = 1; controls.valueC = 2;
            controls.optionA = 0; controls.optionB = 1; controls.optionC = 2;
        }
        else
        {
            controls.valueA = controls.valueB = 50;
        }
        return controls;
    }

    private void ReplaceControls(D3CalibrationControlState controls)
    {
        for (int i = _controls.Count - 1; i >= 0; i--)
            if (_controls[i].partId == controls.partId) _controls.RemoveAt(i);
        _controls.Add(D3CalibrationSystem.CloneControls(controls));
    }

    private D3CalibrationControlState FindControls(string partId)
    {
        return FindControls(_controls, partId);
    }

    private static D3CalibrationControlState FindControls(
        IList<D3CalibrationControlState> controls, string partId)
    {
        if (controls == null) return null;
        for (int i = 0; i < controls.Count; i++)
            if (controls[i] != null && controls[i].partId == partId) return controls[i];
        return null;
    }

    private static D3CalibrationReadingState FindReading(
        IList<D3CalibrationReadingState> readings, string partId)
    {
        for (int i = 0; i < readings.Count; i++)
            if (readings[i] != null && readings[i].partId == partId) return readings[i];
        return null;
    }

    private string GetSelectedPartId()
    {
        int index = partDropdown == null ? 0 : partDropdown.value;
        return Dimension3Catalog.PartIds[Math.Max(0,
            Math.Min(index, Dimension3Catalog.PartIds.Length - 1))];
    }

    private void SetInstructions(string text)
    {
        if (instructionsText != null) instructionsText.text = text;
    }

    private void SetValueLabels(string a, string b, string c)
    {
        if (valueALabel != null) valueALabel.text = a;
        if (valueBLabel != null) valueBLabel.text = b;
        if (valueCLabel != null) valueCLabel.text = c;
    }

    private static void ConfigureDropdown(TMP_Dropdown dropdown, string[] labels)
    {
        if (dropdown == null) return;
        int selected = dropdown.value;
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(labels));
        dropdown.value = Math.Min(selected, labels.Length - 1);
    }

    private static void SetSlider(Slider slider, int min, int max)
    {
        if (slider == null) return;
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = true;
    }

    private static void SetActive(Component component, bool active)
    {
        if (component != null) component.gameObject.SetActive(active);
    }

    private static void SetInteractable(Selectable selectable, bool value)
    {
        if (selectable != null) selectable.interactable = value;
    }

    private static void AddListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null) button.onClick.AddListener(action);
    }

    private void SetNotice(string message)
    {
        if (noticeText != null) noticeText.text = message ?? "";
    }

    private static string StripValue(string label)
    {
        if (string.IsNullOrEmpty(label)) return "Valor";
        int separator = label.IndexOf(':');
        return separator < 0 ? label : label.Substring(0, separator);
    }

    private static string GetPartDisplayName(string partId)
    {
        if (partId == Dimension3Catalog.PartChassis) return "Chasis";
        if (partId == Dimension3Catalog.PartMotor) return "Sistema Motriz";
        if (partId == Dimension3Catalog.PartTool) return "Herramienta";
        if (partId == Dimension3Catalog.PartControl) return "Módulo de Control";
        return "Regulador";
    }

    private static string GetTraitDisplayName(string traitId)
    {
        if (traitId == Dimension3Catalog.TraitFast) return "Rápido";
        if (traitId == Dimension3Catalog.TraitEfficient) return "Eficiente";
        if (traitId == Dimension3Catalog.TraitCoordinator) return "Coordinador";
        return "Normal";
    }
}
