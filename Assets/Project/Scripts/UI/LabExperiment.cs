using UnityEngine;
using TMPro;

public class LabExperiment : MonoBehaviour
{
    [Header("Costos del experimento")]
    [Tooltip("LE que se gasta cada vez que se ejecuta el experimento.")]
    public double leCost = 100000.0;

    [Tooltip("IP que se gana por cada experimento.")]
    public double ipReward = 10.0;

    [Header("UI")]
    [Tooltip("Texto que muestra la descripción del experimento.")]
    public TextMeshProUGUI infoText;

    private void Start()
    {
        ActualizarTexto();
    }

    public void EjecutarExperimento()
    {
        var gs = GameState.I;
        if (gs == null) return;

        if (gs.LE < leCost)
        {
#if UNITY_EDITOR
            Debug.Log("[LabExperiment] No hay LE suficiente para el experimento.");
#endif
            return;
        }

        gs.LE -= leCost;
        gs.IP += ipReward;

        ActualizarTexto();
    }

    private void ActualizarTexto()
    {
        if (infoText == null) return;

        infoText.text = $"Experimento:\nGasta {leCost:0} LE → Gana {ipReward:0} IP";
    }
}
