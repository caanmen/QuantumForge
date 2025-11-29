using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Recursos básicos")]
    public double LE = 0.0;   // Luz de Energía (recurso principal)
    public double VP = 0.0;   // Vacuum Points (recurso raro, aún sin lógica)

    // F6.1: Moneda de prestigio (Entrelazamiento Cuántico)
    public double ENT = 0.0;

    // F6.1: Máximo de LE alcanzado en el run actual
    public double maxLEAlcanzado = 0.0;

    // F6.2: constante para la fórmula de prestigio (log10(maxLE) - K)
    [Tooltip("Constante K para el cálculo de ENT (log10(maxLE) - K). Empieza en 6.0.")]
    public double prestigeK = 6.0;

     // F6.4: Upgrades de prestigio
    [Header("Prestigio - upgrades")]
    [Tooltip("Upgrade de prestigio: multiplicador global de LE/s.")]
    public bool prestigeLeMult1Unlocked = false;

    [Tooltip("Upgrade de prestigio: auto-compra del primer edificio.")]
    public bool prestigeAutoBuyFirstUnlocked = false;

    // Bonus del upgrade de multiplicador (por ejemplo +25% LE/s)
    public double prestigeLeMult1Bonus = 0.25;

    // Temporizador interno para la auto-compra
    private double prestigeAutoBuyTimer = 0.0;

    [Header("Recursos avanzados (placeholder)")]
    [Tooltip("Recurso para el futuro sistema de BEC (aún sin implementar).")]
    public double BEC = 0.0;  // condensado de Bose-Einstein (futuro)

    [Header("Recurso EM (mid-game)")]
    [Tooltip("Campo electromagnético acumulado. Se usará como multiplicador global de LE/s.")]
    public double EM = 0.0;

    [Tooltip("Multiplicador adicional global de LE/s generado por el sistema EM.")]
    public double emMult = 0.0;

    [Header("Investigación (Research)")]
    [Tooltip("Puntos de investigación (IP) usados para comprar mejoras de laboratorio.")]
    public double IP = 0.0;

    [Tooltip("Multiplicador global de LE/s proveniente de investigaciones.")]
    public double researchGlobalLEMult = 1.0;   // se recalcula desde ResearchManager


    [Header("Producción base (sin edificios)")]
    public double baseLEps = 0.5;   // producción base sin edificios


    // Lista de edificios que producen LE (se llena desde la UI / BuildingList)
    private List<BuildingState> buildingStates = new List<BuildingState>();

    [Header("Decoherencia (soft cap) - DESACTIVADA POR AHORA")]
    [Tooltip("Por ahora no afecta la producción. Más adelante se reutilizará.")]
    public bool useDecoherence = false;   // << clave: queda en false

    [Tooltip("A partir de esta cantidad de LE almacenada empezaría la decoherencia (futuro).")]
    public double decoStartLE = 3000.0;

    [Tooltip("Qué tan rápido caería la producción cuando te pases del umbral (futuro).")]
    public double decoStrength = 0.00004;

    [Tooltip("Factor mínimo de producción (0.6 = nunca baja de 60%) (futuro).")]
    public double decoMinFactor = 0.6;

    // Debug: acumulador de tiempo para logs
    private float _dbg = 0f;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1f;
    }

    private void Start()
    {
        // Cuando el GameState ya está creado, pedimos cargar el save
        if (SaveService.I != null)
        {
            SaveService.I.Load();
        }
    }

    private void Update()
    {
        double dt = Time.unscaledDeltaTime;
        Tick(dt);

        _dbg += Time.unscaledDeltaTime;
        if (_dbg >= 1f)
        {
        double totalLEps = GetTotalLEps();
        double entPreview = GetENTGanariasAlPrestigiar();
        Debug.Log($"[GameState] LE = {LE:0.000} | LE/s = {totalLEps:0.00} | ENT si prestigias: {entPreview}");
        _dbg = 0f;
        }
    }

    /// <summary>
    /// Avanza el juego dt segundos (lógica principal de producción).
    /// </summary>
    public void Tick(double dt)
    {
    // 1) Producir EM...
    double emPs = CalculateEMps();
    if (emPs > 0.0)
    {
        EM += emPs * dt;

        // 1b) Generar IP (una sola vez)
        double ipPs = emPs * 0.1;
        IP += ipPs * dt;
    }

    // 2) Actualizar el multiplicador EM
    emMult = CalculateEMMultiplier();

    // 3) Producir LE usando multiplicadores de EM + Research
    double totalLEps = CalculateTotalLEps();
    LE += totalLEps * dt;

    // 4) Automatizaciones de prestigio
    RunPrestigeAutomations(dt);

    // F6.1: registrar el máximo LE alcanzado
    ActualizarMaxLE();
    }



    // F6.1: Actualiza el máximo LE alcanzado en este run
    public void ActualizarMaxLE()
    {
        if (LE > maxLEAlcanzado)
        {
            maxLEAlcanzado = LE;
        }
    }

    // F6.2: ENT total teórica según el máximo LE alcanzado en este run.
    public double GetENTTeorica()
    {
        if (maxLEAlcanzado <= 0.0)
            return 0.0;

        // log10 del máximo LE
        double log = System.Math.Log10(maxLEAlcanzado);

        // ENT = floor(log10(maxLE) - K)
        double raw = System.Math.Floor(log - prestigeK);

        if (raw < 0.0)
            raw = 0.0;

        return raw;
    }

    // F6.2: ENT que ganarías si haces prestigio AHORA.
    // Por ahora es igual a la ENT teórica. Más adelante, si quieres evitar farmeo
    // repetido, podemos restar aquí las ENT ya ganadas en otros runs.
    public double GetENTGanariasAlPrestigiar()
    {
        return GetENTTeorica();
    }

        // F6.4: multiplicador global de LE/s proveniente de upgrades de prestigio
    public double GetPrestigeLEMultiplier()
    {
        double mult = 1.0;

        if (prestigeLeMult1Unlocked)
        {
            mult *= (1.0 + prestigeLeMult1Bonus); // +25% LE/s si está desbloqueado
        }

        return mult;
    }

    /// <summary>
    /// Calcula la producción total de LE/s:
    /// - producción base
    /// - producción de edificios
    /// - bonus globales
    /// (Por ahora SIN decoherencia).
    /// </summary>
    private double CalculateTotalLEps()
{
    double baseProd = baseLEps;
    double fromBuildings = 0.0;
    double multiplier = 1.0;
    double flatBonus = 0.0;

    foreach (var b in buildingStates)
    {
        if (b == null || b.def == null) continue;

        double buildingProd = b.GetLEps();
        fromBuildings += buildingProd;

        if (b.level <= 0) continue;

        switch (b.def.bonusType)
        {
            case BuildingBonusType.None:
                break;

            case BuildingBonusType.MultiplierLE:
                multiplier += b.def.bonusPerLevel * b.level;
                break;

            case BuildingBonusType.FlatLE:
                flatBonus += b.def.bonusPerLevel * b.level;
                break;
        }
    }

    // EM
    double emFactor = 1.0 + emMult;

    // Research (lo que ya tienes)
    double researchFactor = researchGlobalLEMult;

    // 🔥 Achievements
    double achFactor = 1.0;
    if (AchievementManager.I != null)
    {
        achFactor = AchievementManager.I.GetGlobalLEFactor();
    }
    
    // 🔥 F6.4: factor de prestigio
    double prestigeFactor = GetPrestigeLEMultiplier();

    double rawTotal = (baseProd + fromBuildings)
                      * multiplier
                      * emFactor
                      * researchFactor
                      * achFactor
                      + flatBonus;

    return rawTotal;
}


    /// <summary>
    /// Calcula cuánta EM/s generan los edificios relacionados con EM.
    /// </summary>
    
    private double CalculateEMps()
{
    double emPs = 0.0;

    foreach (var b in buildingStates)
    {
        if (b == null || b.def == null) continue;
        if (b.level <= 0) continue;

        switch (b.def.id)
        {
            case "em_field_emitter":
                emPs += 0.5 * b.level;
                break;

            case "em_field_array":
                emPs += 1.0 * b.level;
                break;

            case "micro_collider":
                emPs += 2.0 * b.level;
                break;
        }
    }

    // Aplicar multiplicador de investigaciones (Cosecha EM I/II)
    if (ResearchManager.I != null)
    {
        emPs *= ResearchManager.I.GetEMGenerationFactor();
    }

    return emPs;
}


/// <summary>
/// Convierte el EM acumulado en un multiplicador suave de producción de LE.
/// </summary>
private double CalculateEMMultiplier()
{
    if (EM <= 0.0) return 0.0;

    // Cada 100 EM aporta ~5% extra, con rendimientos decrecientes (sqrt)
    double k = 0.05; // 5% base
    double normalized = EM / 100.0;

    return k * System.Math.Sqrt(normalized);
}


    /// <summary>
    /// Placeholder: por ahora no se usa.
    /// </summary>
    private double ApplyDecoherence(double rawLEps)
    {
        // Devuelve tal cual, sin cambios.
        return rawLEps;

        // Cuando queramos reusar esta mecánica, aquí se reactivará la lógica.
    }

    public void RegisterBuildingState(BuildingState state)
    {
        if (state == null) return;
        if (!buildingStates.Contains(state))
        {
            buildingStates.Add(state);
        }
    }

    /// <summary>
    /// Devuelve la producción total de LE por segundo.
    /// </summary>
    public double GetTotalLEps()
    {
        return CalculateTotalLEps();
    }

    /// <summary>
    /// Devuelve el nivel actual de un edificio por id.
    /// </summary>
    public int GetBuildingLevel(string id)
    {
        if (string.IsNullOrEmpty(id)) return 0;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.def.id == id)
            {
                return b.level;
            }
        }

        return 0;
    }

        // F6.3: ¿puedo prestigiar ahora?
    public bool CanPrestige()
    {
        // Por ahora, pedimos al menos 1 ENT para que valga la pena
        double ent = GetENTGanariasAlPrestigiar();
        return ent >= 1.0;
    }

    // F6.3: aplica el prestigio (si es posible).
    // Devuelve cuánta ENT se ganó.
    public double DoPrestigeReset()
    {
        double entGanar = GetENTGanariasAlPrestigiar();
        if (entGanar <= 0.0)
        {
            Debug.Log("[GameState] No hay suficiente progreso para prestigiar (ENT ganada = 0).");
            return 0.0;
        }

        // 1) Añadir ENT
        ENT += entGanar;
        Debug.Log($"[GameState] Prestigio realizado. ENT ganada: {entGanar}, ENT total: {ENT}");

        // 2) Resetear el run (recursos y edificios)
        ResetRunForPrestige();

        // 3) Guardar estado después del prestigio
        if (SaveService.I != null)
        {
            SaveService.I.Save();
        }

        return entGanar;
    }

    // F6.3: lógica de reset del run (sin tocar ENT ni upgrades de prestigio)
    private void ResetRunForPrestige()
    {
        // Reset recursos básicos
        LE = 0.0;
        VP = 0.0;

        // Reset recursos avanzados
        BEC = 0.0;
        EM = 0.0;
        emMult = 0.0;
        IP = 0.0;

        // Multiplicadores de investigación (LOS DEJAMOS como están por ahora
        // porque más adelante podríamos decidir si el prestigio los borra o no).
        // researchGlobalLEMult se recalcula desde ResearchManager, así que no lo tocamos.

        // Reset decoherencia (por si la activamos en el futuro)
        useDecoherence = false;
        maxLEAlcanzado = 0.0;

        // Reset de edificios: por ahora dejamos los niveles en 0.
        foreach (var b in buildingStates)
        {
            if (b == null) continue;
            b.ResetForPrestige();
        }
    }

        // F6.5: corre las automatizaciones asociadas a upgrades de prestigio
    private void RunPrestigeAutomations(double dt)
    {
        if (!prestigeAutoBuyFirstUnlocked) return;

        prestigeAutoBuyTimer += dt;
        if (prestigeAutoBuyTimer < 0.5) return;   // cada 0.5 s aprox.
        prestigeAutoBuyTimer = 0.0;

        TryAutoBuyFirstBuilding();
    }

    // F6.5: intenta comprar automáticamente el primer edificio
    private void TryAutoBuyFirstBuilding()
    {
        if (buildingStates == null || buildingStates.Count == 0) return;

        var first = buildingStates[0];
        if (first == null || first.def == null) return;

        // Solo si está desbloqueado
        if (!BuildingUnlock.IsUnlocked(first.def))
            return;

        // Solo si podemos pagar
        if (!first.CanAfford(LE))
            return;

        // Pagar y comprar
        LE -= first.currentCost;
        first.OnPurchased();
    }


}
