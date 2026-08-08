using UnityEngine;

public enum MachineCube3DModuleRole
{
    Core,
    Surface,
    Frame,
    Armor,
    Conduit,
    NodeSocket,
    Damage,
    WarningLight,
    RepairLight,
    Detail
}

[DisallowMultipleComponent]
public sealed class MachineCube3DModule : MonoBehaviour
{
    [SerializeField] private string moduleId;
    [SerializeField] private int faceIndex = -1;
    [SerializeField] private MachineCube3DModuleRole role;
    [SerializeField] private MachineCube3DQualityLevel minimumQuality =
        MachineCube3DQualityLevel.Low;

    public string ModuleId => moduleId;
    public int FaceIndex => faceIndex;
    public MachineCube3DModuleRole Role => role;
    public MachineCube3DQualityLevel MinimumQuality => minimumQuality;

    public void Configure(string stableId, int ownerFaceIndex,
        MachineCube3DModuleRole moduleRole,
        MachineCube3DQualityLevel requiredQuality = MachineCube3DQualityLevel.Low)
    {
        moduleId = stableId ?? "";
        faceIndex = ownerFaceIndex;
        role = moduleRole;
        minimumQuality = requiredQuality;
    }

    public bool ApplyQuality(MachineCube3DQualityLevel quality)
    {
        if (minimumQuality == MachineCube3DQualityLevel.Low)
            return false;
        bool visible = quality >= minimumQuality;
        if (gameObject.activeSelf == visible)
            return false;
        gameObject.SetActive(visible);
        return true;
    }
}
