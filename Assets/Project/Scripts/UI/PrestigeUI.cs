using System;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Auto-compra: toggle")]
    public Button autoBuyToggleButton;
    public TextMeshProUGUI autoBuyToggleLabel;

       private string L(string key, string fallback)
    {
        var lm = LocalizationManager.I;
        if (lm == null) return fallback;
        var s = lm.T(key);
        return string.IsNullOrEmpty(s) ? fallback : s;
    }

    private string LF(string key, string fallback, params object[] args)
    {
        string fmt = L(key, fallback);
        try { return string.Format(fmt, args); }
        catch { return fmt; }
    }

        private void Update()
    {
        var gs = GameState.I;
        if (gs == null) return;

        // ENT actual
        if (entActualText != null)
        {
            entActualText.text = LF(
                "prestige.ent_current",
                "ENT: {0:0}",
                gs.ENT
            );
        }

        // ENT que ganarías si prestigias ahora
        if (entGananciaText != null)
        {
            double entGanar = gs.GetENTGanariasAlPrestigiar();
            entGananciaText.text = LF(
                "prestige.ent_gain",
                "Si prestigias ahora: +{0:0} ENT",
                entGanar
            );
        }

        // Texto upgrade LE/s
        if (leMult1Text != null)
        {
            double pct = gs.prestigeLeMult1Bonus * 100.0;

            if (gs.prestigeLeMult1Unlocked)
            {
                leMult1Text.text = LF(
                    "prestige.upg_le_mult1_bought",
                    "Upgrade LE/s I (+{0:0}% LE/s) - COMPRADO",
                    pct
                );
            }
            else
            {
                leMult1Text.text = LF(
                    "prestige.upg_le_mult1_cost",
                    "Upgrade LE/s I (+{0:0}% LE/s) - Coste: {1} ENT",
                    pct, costoLeMult1
                );
            }
        }

        // Texto upgrade auto-compra
        if (autoBuy1Text != null)
        {
            if (gs.prestigeAutoBuyFirstUnlocked)
            {
                autoBuy1Text.text = L(
                    "prestige.upg_autobuy1_bought",
                    "Auto-compra Edificio 1 - COMPRADO"
                );
            }
            else
            {
                autoBuy1Text.text = LF(
                    "prestige.upg_autobuy1_cost",
                    "Auto-compra Edificio 1 (cada 0.5 s) - Coste: {0} ENT",
                    costoAutoBuy1
                );
            }
        }

        // Toggle de auto-compra ON/OFF
        if (autoBuyToggleButton != null && autoBuyToggleLabel != null)
        {
            bool unlocked = gs.prestigeAutoBuyFirstUnlocked;

            // Solo mostramos el botón cuando el upgrade está comprado
            autoBuyToggleButton.gameObject.SetActive(unlocked);

            if (unlocked)
            {
                autoBuyToggleButton.interactable = true;

                autoBuyToggleLabel.text = gs.prestigeAutoBuyFirstEnabled
                    ? L("prestige.auto_on", "Auto: ON")
                    : L("prestige.auto_off", "Auto: OFF");
            }
        }
    }


    // Botón: encender / apagar auto-compra del edificio 1
    public void OnClickToggleAutoBuy()
    {
        var gs = GameState.I;
        if (gs == null) return;
        if (!gs.prestigeAutoBuyFirstUnlocked) return;

        gs.prestigeAutoBuyFirstEnabled = !gs.prestigeAutoBuyFirstEnabled;

        if (SaveService.I != null)
            SaveService.I.Save();
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
        gs.prestigeAutoBuyFirstEnabled  = true;   // se enciende al comprar


        if (SaveService.I != null)
            SaveService.I.Save();

        Debug.Log("[PrestigeUI] Upgrade auto-compra comprado.");
    }
}
