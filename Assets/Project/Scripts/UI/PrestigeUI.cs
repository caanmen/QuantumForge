using UnityEngine;
using TMPro;

public class PrestigeUI : MonoBehaviour
{
    [Header("Textos de prestigio")]
    public TextMeshProUGUI entActualText;      // Muestra ENT actual
    public TextMeshProUGUI entGananciaText;    // Muestra ENT que ganarías al prestigiar

    [Header("Upgrades de prestigio - textos")]
    public TextMeshProUGUI leMult1Text;        // Texto del upgrade de LE/s
    public TextMeshProUGUI autoBuy1Text;       // Texto del upgrade de auto-compra

    [Header("Costes de upgrades (en ENT)")]
    public int costoLeMult1 = 3;
    public int costoAutoBuy1 = 5;

    private void Update()
    {
        var gs = GameState.I;
        if (gs == null) return;

        // ENT actual
        if (entActualText != null)
        {
            entActualText.text = $"ENT: {gs.ENT:0}";
        }

        // ENT que ganarías si prestigias ahora
        if (entGananciaText != null)
        {
            double entGanar = gs.GetENTGanariasAlPrestigiar();
            entGananciaText.text = $"Si prestigias ahora: +{entGanar:0} ENT";
        }

        // Texto upgrade LE/s
        if (leMult1Text != null)
        {
            if (gs.prestigeLeMult1Unlocked)
            {
                leMult1Text.text = $"Upgrade LE/s I (+{gs.prestigeLeMult1Bonus * 100:0}% LE/s) - COMPRADO";
            }
            else
            {
                leMult1Text.text = $"Upgrade LE/s I (+{gs.prestigeLeMult1Bonus * 100:0}% LE/s) - Coste: {costoLeMult1} ENT";
            }
        }

        // Texto upgrade auto-compra
        if (autoBuy1Text != null)
        {
            if (gs.prestigeAutoBuyFirstUnlocked)
            {
                autoBuy1Text.text = "Auto-compra Edificio 1 - COMPRADO";
            }
            else
            {
                autoBuy1Text.text = $"Auto-compra Edificio 1 (cada 0.5 s) - Coste: {costoAutoBuy1} ENT";
            }
        }
    }

    // Botón de prestigio
    public void OnClickPrestige()
    {
        var gs = GameState.I;
        if (gs == null) return;

        if (!gs.CanPrestige())
        {
            Debug.Log("[PrestigeUI] No puedes prestigiar todavía (ENT ganada < 1).");
            return;
        }

        double entGanada = gs.DoPrestigeReset();
        Debug.Log($"[PrestigeUI] Prestigio hecho. ENT ganada: {entGanada}");
    }

    // Botón: comprar upgrade de LE/s
    public void OnClickBuyLeMult1()
    {
        var gs = GameState.I;
        if (gs == null) return;

        if (gs.prestigeLeMult1Unlocked)
        {
            Debug.Log("[PrestigeUI] Upgrade LE/s I ya está comprado.");
            return;
        }

        if (gs.ENT < costoLeMult1)
        {
            Debug.Log("[PrestigeUI] No tienes suficiente ENT para Upgrade LE/s I.");
            return;
        }

        gs.ENT -= costoLeMult1;
        gs.prestigeLeMult1Unlocked = true;

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log("[PrestigeUI] Upgrade LE/s I comprado.");
    }

    // Botón: comprar upgrade de auto-compra
    public void OnClickBuyAutoBuy1()
    {
        var gs = GameState.I;
        if (gs == null) return;

        if (gs.prestigeAutoBuyFirstUnlocked)
        {
            Debug.Log("[PrestigeUI] Upgrade auto-compra ya está comprado.");
            return;
        }

        if (gs.ENT < costoAutoBuy1)
        {
            Debug.Log("[PrestigeUI] No tienes suficiente ENT para auto-compra.");
            return;
        }

        gs.ENT -= costoAutoBuy1;
        gs.prestigeAutoBuyFirstUnlocked = true;

        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log("[PrestigeUI] Upgrade auto-compra comprado.");
    }
}
