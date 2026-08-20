using UnityEngine;

[CreateAssetMenu(
    fileName = "UIProductionScreenManifest",
    menuName = "Quantum Forge/UI/Production Screen Manifest")]
public sealed class UIProductionScreenManifest : ScriptableObject
{
    public enum DimensionId
    {
        Common = 0,
        Dimension1 = 1,
        Dimension2 = 2,
        Dimension3 = 3,
        Dimension4 = 4
    }

    public enum ApprovalStage
    {
        Draft,
        StaticComposition,
        DataAndInteraction,
        Animation,
        Approved
    }

    [Header("Identidad")]
    public string screenId;
    public string displayName;
    public DimensionId dimension;
    public ApprovalStage approvalStage;

    [Header("Contrato visual")]
    public Vector2Int baseResolution = new Vector2Int(1080, 1920);
    [Range(0f, 1f)] public float perceivedSimilarityTarget = 0.95f;
    [Tooltip("Ruta relativa a la raíz del proyecto.")]
    public string referenceRelativePath;
    [Tooltip("Rutas relativas a la raíz del proyecto.")]
    public string[] approvedCaptureRelativePaths;
    public VerticalUiTheme sharedTheme;

    [Header("Automatización")]
    [Tooltip("Clase y método que reconstruyen la pantalla, si existen.")]
    public string setupMethod;
    [Tooltip("Clase y método que ejecutan su validación o captura, si existen.")]
    public string validationMethod;
    public string[] requiredCaptureStates;

    [Header("Documentación")]
    [Tooltip("Ruta relativa a la raíz del proyecto.")]
    public string documentationFolder;
    [TextArea(3, 8)] public string notes;
}
