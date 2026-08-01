using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MachineCubeNodeVisualUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image haloImage;
    [SerializeField] private Image pictogramImage;
    [SerializeField] private TextMeshProUGUI stateGlyphText;
    [SerializeField] private TextMeshProUGUI tierText;
    [SerializeField] private Sprite iconLe;
    [SerializeField] private Sprite iconTraces;
    [SerializeField] private Sprite iconTriangle;
    [SerializeField] private Sprite iconArtifact;
    [SerializeField] private Sprite iconFusion;
    [SerializeField] private Sprite iconDiagnostic;
    [SerializeField] private Sprite iconStructure;
    [SerializeField] private Sprite iconConvergence;
    [SerializeField] private Sprite iconAnchor;
    [SerializeField] private Sprite iconSynthesis;

    private MachineCubeVisualUI _controller;
    private MachineNodeDef _node;

    public string BoundNodeId => _node?.id ?? "";
    public RectTransform Rect => transform as RectTransform;

    public void Initialize(MachineCubeVisualUI controller)
    {
        _controller = controller;
        if (button == null)
            return;

        button.onClick.RemoveListener(HandleClick);
        button.onClick.AddListener(HandleClick);
    }

    public void Bind(MachineNodeDef node, bool selected, Color accent)
    {
        _node = node;
        gameObject.SetActive(node != null);
        if (node == null || MachineManager.I == null)
            return;

        bool repaired = MachineManager.I.IsNodeRepaired(node.id);
        bool analyzed = MachineManager.I.IsNodeAnalyzed(node.id);
        bool damaged = node.damaged && !repaired && !analyzed;
        bool analyzing = IsCurrentAnalysis(node);
        bool canRepair = MachineManager.I.CanRepairNode(node.id, out string reason);
        bool requirementBlock = !canRepair &&
            reason.StartsWith("Falta reparar nodo requerido", System.StringComparison.Ordinal);
        bool resourceBlock = !canRepair &&
            (reason.StartsWith("Falta LE", System.StringComparison.Ordinal) ||
             reason.StartsWith("Faltan", System.StringComparison.Ordinal));

        string stateCode;
        Color color;
        if (repaired)
        {
            stateCode = "R";
            color = new Color(0.25f, 1f, 0.78f, 1f);
        }
        else if (analyzing)
        {
            stateCode = "...";
            color = new Color(0.35f, 0.78f, 1f, 1f);
        }
        else if (damaged)
        {
            stateCode = "!";
            color = new Color(1f, 0.42f, 0.2f, 1f);
        }
        else if (analyzed)
        {
            stateCode = "A";
            color = new Color(0.72f, 0.48f, 1f, 1f);
        }
        else if (canRepair)
        {
            stateCode = "+";
            color = accent;
        }
        else if (requirementBlock)
        {
            stateCode = "X";
            color = new Color(1f, 0.67f, 0.18f, 1f);
        }
        else if (resourceBlock)
        {
            stateCode = "$";
            color = new Color(1f, 0.32f, 0.28f, 1f);
        }
        else if (node.hidden)
        {
            stateCode = "S";
            color = new Color(0.88f, 0.4f, 1f, 1f);
        }
        else
        {
            stateCode = "-";
            color = new Color(0.42f, 0.48f, 0.52f, 1f);
        }

        if (frameImage != null)
            frameImage.color = selected
                ? new Color(0.65f, 0.84f, 0.92f, 0.10f)
                : new Color(0f, 0f, 0f, 0.001f);
        if (haloImage != null)
        {
            haloImage.gameObject.SetActive(selected || analyzing);
            Color reticle = selected
                ? new Color(0.82f, 0.94f, 1f, 0.30f)
                : new Color(accent.r, accent.g, accent.b, 0.36f);
            haloImage.color = reticle;
        }
        if (stateGlyphText != null)
        {
            stateGlyphText.text = MachineCubeVisualUI.GetEffectGlyph(node);
            stateGlyphText.color = color;
        }
        if (pictogramImage != null)
        {
            pictogramImage.sprite = ResolveIcon(node);
            pictogramImage.color = repaired
                ? Color.white
                : new Color(0.78f, 0.84f, 0.88f, 0.88f);
            pictogramImage.enabled = pictogramImage.sprite != null;
        }
        if (tierText != null)
        {
            string tier = node.tierIndex > 0 ? ToRoman(node.tierIndex) : "";
            tierText.text = string.IsNullOrEmpty(tier) ? stateCode : tier;
            tierText.color = selected ? Color.white : color;
        }
        if (button != null)
            button.interactable = true;
    }

    public void Hide()
    {
        _node = null;
        gameObject.SetActive(false);
    }

    private void HandleClick()
    {
        if (_node != null)
            _controller?.SelectNode(_node.id);
    }

    private static bool IsCurrentAnalysis(MachineNodeDef node)
    {
        if (node == null || MachineManager.I == null || !MachineManager.I.IsAnalyzingNode)
            return false;

        string active = MachineManager.I.AnalysisNodeId;
        return active == node.id ||
            (!string.IsNullOrWhiteSpace(node.tierGroup) &&
             active == "tierGroup:" + node.tierGroup);
    }

    private static string ToRoman(int value)
    {
        return value switch
        {
            1 => "I",
            2 => "II",
            3 => "III",
            4 => "IV",
            _ => value.ToString()
        };
    }

    private Sprite ResolveIcon(MachineNodeDef node)
    {
        return MachineCubeVisualUI.GetEffectGlyph(node) switch
        {
            "LE" => iconLe,
            "TR" => iconTraces,
            "TI" => iconTriangle,
            "AR" => iconArtifact,
            "FU" => iconFusion,
            "DX" => iconDiagnostic,
            "ST" => iconStructure,
            "CV" => iconConvergence,
            "AN" => iconAnchor,
            _ => iconSynthesis
        };
    }
}
