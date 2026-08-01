using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MachineCubeFaceViewUI : MonoBehaviour
{
    [SerializeField] private MachineZoneType zone;
    [SerializeField] private RectTransform faceRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private RawImage baseArtwork;
    [SerializeField] private RectTransform circuitLayer;
    [SerializeField] private MachineCubeNodeVisualUI[] nodeSlots;

    public MachineZoneType Zone => zone;
    public RectTransform FaceRect => faceRect;
    public CanvasGroup Group => canvasGroup;
    public Texture BaseArtworkTexture => baseArtwork != null ? baseArtwork.texture : null;

    public RectTransform FindSlotForNode(string nodeId)
    {
        MachineNodeDef selected = MachineManager.I != null
            ? MachineManager.I.GetDef(nodeId)
            : null;
        if (selected == null || nodeSlots == null)
            return null;

        foreach (MachineCubeNodeVisualUI slot in nodeSlots)
        {
            MachineNodeDef shown = MachineManager.I.GetDef(slot?.BoundNodeId);
            if (IsSameVisualBranch(shown, selected))
                return slot.Rect;
        }
        return null;
    }

    public void Initialize(MachineCubeVisualUI controller)
    {
        if (faceRect == null)
            faceRect = transform as RectTransform;
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (nodeSlots == null)
            return;
        foreach (MachineCubeNodeVisualUI slot in nodeSlots)
            slot?.Initialize(controller);
    }

    public void RefreshFace(string selectedNodeId, Color accent)
    {
        if (MachineManager.I == null)
            return;

        List<MachineNodeDef> nodes = MachineManager.I.GetDisplayNodesByZone(zone);
        MachineNodeDef selected = MachineManager.I.GetDef(selectedNodeId);

        for (int i = 0; i < nodeSlots.Length; i++)
        {
            MachineCubeNodeVisualUI slot = nodeSlots[i];
            if (slot == null)
                continue;

            if (nodes == null || i >= nodes.Count)
            {
                slot.Hide();
                continue;
            }

            MachineNodeDef node = nodes[i];
            slot.Bind(node, IsSameVisualBranch(node, selected), accent);
        }

        double progress = MachineManager.I.GetZoneRepairProgress01(zone);
        if (baseArtwork != null)
        {
            float energy = Mathf.Lerp(0.72f, 1.00f, (float)progress);
            baseArtwork.color = new Color(energy, energy, energy, 1f);
        }

        if (progressText != null)
        {
            progressText.text = $"REPARACIÓN DE CARA  {progress * 100.0:0}%";
            progressText.color = accent;
        }
    }

    public void ShowImmediate(bool visible)
    {
        gameObject.SetActive(visible);
        if (!visible)
            return;

        faceRect.anchoredPosition = Vector2.zero;
        faceRect.localScale = Vector3.one;
        faceRect.localRotation = Quaternion.identity;
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    private static bool IsSameVisualBranch(MachineNodeDef shown, MachineNodeDef selected)
    {
        if (shown == null || selected == null)
            return false;
        if (shown.id == selected.id)
            return true;
        return !string.IsNullOrWhiteSpace(shown.tierGroup) &&
            shown.tierGroup == selected.tierGroup;
    }
}
