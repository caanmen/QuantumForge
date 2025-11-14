using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI leText;

    void Update()
    {
        var gs = GameState.I;
        if (gs == null || leText == null) return;

        // LE/s total = base + edificios
        double totalLEps = gs.GetTotalLEps();

        leText.text = $"LE: {gs.LE:0.000}  (LE/s: {totalLEps:0.##})";
    }
}
