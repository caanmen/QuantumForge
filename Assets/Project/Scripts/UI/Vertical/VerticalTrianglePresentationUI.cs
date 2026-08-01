using UnityEngine;
using UnityEngine.UI;

public sealed class VerticalTrianglePresentationUI : MonoBehaviour
{
    public Image energyLine;
    public Image experimentalLine;
    public Image phaseLine;
    public Image centerGlow;
    public Color inactiveColor = new(0.08f, 0.18f, 0.28f, 0.78f);
    public Color lockedColor = new(0.05f, 0.08f, 0.12f, 0.55f);
    public Color energyColor = new(0f, 0.84f, 1f, 1f);
    public Color experimentalColor = new(0.73f, 0.31f, 0.93f, 1f);
    public Color phaseColor = new(1f, 0.60f, 0.13f, 1f);

    private void Update()
    {
        GameState state = GameState.I;
        TriangleCircuitType active = state != null
            ? state.triangleActiveCircuit
            : TriangleCircuitType.None;
        SetLine(energyLine, active == TriangleCircuitType.Energy,
            false, energyColor);
        SetLine(experimentalLine, active == TriangleCircuitType.Experimental,
            false, experimentalColor);
        SetLine(phaseLine, active == TriangleCircuitType.Phase,
            state == null || !state.IsTrianglePhaseUnlocked(), phaseColor);
        if (centerGlow != null)
        {
            Color glow = active == TriangleCircuitType.Energy ? energyColor :
                active == TriangleCircuitType.Experimental ? experimentalColor :
                active == TriangleCircuitType.Phase ? phaseColor : inactiveColor;
            glow.a = 0.20f + 0.08f * Mathf.Sin(Time.unscaledTime * 2.4f);
            centerGlow.color = glow;
        }
    }

    private void SetLine(Image line, bool active, bool locked, Color accent)
    {
        if (line == null)
            return;
        line.color = locked ? lockedColor : active ? accent : inactiveColor;
    }
}
