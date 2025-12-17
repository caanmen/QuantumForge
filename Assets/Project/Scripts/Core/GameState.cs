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
    public double prestigeK = 5.0;

     // F6.4: Upgrades de prestigio
    [Header("Prestigio - upgrades")]
    [Tooltip("Upgrade de prestigio: multiplicador global de LE/s.")]
    public bool prestigeLeMult1Unlocked = false;

    [Tooltip("Upgrade de prestigio: auto-compra del primer edificio.")]
    public bool prestigeAutoBuyFirstUnlocked = false;

    [Tooltip("Si está en true, la auto-compra del primer edificio está activa.")]
    public bool prestigeAutoBuyFirstEnabled = true;

    // Bonus del upgrade de multiplicador (por ejemplo +25% LE/s)
    public double prestigeLeMult1Bonus = 0.25;

    // Temporizador interno para la auto-compra
    private double prestigeAutoBuyTimer = 0.0;

    [Header("Recursos avanzados (placeholder)")]
    [Tooltip("Recurso para el futuro sistema de BEC (aún sin implementar).")]
    public double BEC = 0.0;  // condensado de Bose-Einstein (futuro)

    // F7: Recursos late-game (por run)
    [Header("Recursos late-game (F7)")]
    [Tooltip("Recurso late-game generado en runs avanzadas. Se resetea con cualquier prestigio.")]
    public double ADP = 0.0;

    [Tooltip("Fragmentos de agujero de gusano (WHF). Recurso muy raro por run, se resetea con cualquier prestigio.")]
    public double WHF = 0.0;

    // F7: Prestigio 2 (Lambda)
    [Header("Prestigio 2 (Lambda)")]
    [Tooltip("Moneda de meta-prestigio (Prestigio 2). No se pierde al resetear runs ni en Prestigio 1.")]
    public double Lambda = 0.0;

        // F7.5: Meta-upgrades comprados con Λ
    [Header("Meta-upgrades (Prestigio 2)")]
    [Tooltip("Upgrade de Lambda: +20% ENT ganada permanentemente.")]
    public bool metaEntBoost1Bought = false;

    [Tooltip("Upgrade de Lambda: +15% EM base permanente.")]
    public bool metaEmBoost1Bought = false;

    // F7: Estadísticas acumuladas para meta-prestigio
    [Tooltip("Total de ENT acumulada a lo largo de todas las runs (sirve para fórmulas futuras de Lambda).")]
    public double totalENTAcumulada = 0.0;

    [Tooltip("Total de ADP generada a lo largo de todas las runs (placeholder F7).")]
    public double totalADPGenerada = 0.0;

    [Tooltip("Total de fragmentos de Wormhole generados a lo largo de todas las runs (placeholder F7).")]
    public double totalWHFGenerada = 0.0;

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
    public double baseLEps = 0.0;   // producción base sin edificios


    // Lista de edificios que producen LE (se llena desde la UI / BuildingList)
    private List<BuildingState> buildingStates = new List<BuildingState>();

    // Para evitar aplicar los niveles cargados más de una vez
    private bool buildingLevelsApplied = false;


    // Exporta los niveles de edificios para el sistema de guardado
    public List<SavedBuildingLevel> GetBuildingLevelsForSave()
    {
        var list = new List<SavedBuildingLevel>();

        if (buildingStates == null) return list;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue; // solo guardamos los que tengan nivel

            list.Add(new SavedBuildingLevel
            {
                id = b.def.id,
                level = b.level
            });
        }

        return list;
    }

    // Aplica niveles cargados desde el save
    public void ApplyBuildingLevelsFromSave(List<SavedBuildingLevel> saved)
    {
        if (saved == null || buildingStates == null) return;

        foreach (var sb in saved)
        {
            if (string.IsNullOrEmpty(sb.id)) continue;

            foreach (var b in buildingStates)
            {
                if (b == null || b.def == null) continue;
                if (b.def.id == sb.id)
                {
                    b.level = sb.level;
                    break;
                }
            }
        }

            // Si más adelante tienes un recalculo específico, lo puedes llamar aquí.
            // De momento, CalculateTotalLEps() ya usa buildingStates.
    }


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

        // 1b) Generar IP (pasivo a partir de EM)
        double ipPs = emPs * 0.5;
        IP += ipPs * dt;
    }

    // 2) Actualizar el multiplicador EM
    emMult = CalculateEMMultiplier();

    // 3) Producir LE
    //    - CalculateTotalLEps() se sigue usando para HUD y lógica de desbloqueos.
    //    - PERO ya NO sumamos LE usando esa fórmula directamente.
    double totalLEps = CalculateTotalLEps(); // <-- solo informativo / HUD
    GenerateLEFromBaseAndBuildings(dt);      // <-- NUEVO: producción real

    // 🔹 F7.3: Producir ADP
    double adpPs = CalculateADPps();
    if (adpPs > 0.0)
    {
        ADP += adpPs * dt;
        totalADPGenerada += adpPs * dt;
    }

    // 🔹 F7.4: WHF (Wormhole Fragments)
    double whfPs = CalculateWHFps();
    if (whfPs > 0.0)
    {
        WHF += whfPs * dt;
        totalWHFGenerada += whfPs * dt;
    }

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

    public void DebugResetRunState()
    {
    // 🔹 Recursos básicos
    LE = 10.0;
    VP = 0.0;
    
    // 🔹 EM e IP también deben resetearse
    EM = 0.0;         // 👈 NUEVO
    emMult = 0.0;     // 👈 NUEVO
    IP = 0.0;         // 👈 NUEVO
    
    // 🔹 Producción base sin edificios
    baseLEps = 0.0;

    // 🔹 Reset del máximo de LE alcanzado en el run
    maxLEAlcanzado = 0.0;

    // 🔹 Reset de edificios (nivel 0)
    if (buildingStates != null)
    {
        foreach (var b in buildingStates)
        {
            if (b == null) continue;
            b.ResetForPrestige();   // este método ya existe en BuildingState
        }
    }

    // Si más adelante quieres, aquí también puedes resetear BEC, EM, ADP, WHF, etc.
    }


    // F6.2: ENT total teórica según el máximo LE alcanzado en este run.
    public double GetENTTeorica()
    {
        if (maxLEAlcanzado <= 0.0)
            return 0.0;

        // log10 del máximo LE
        double log = System.Math.Log10(maxLEAlcanzado);

        // ENT base (antes de meta-upgrades)
        double baseEnt = log - prestigeK;
        if (baseEnt <= 0.0)
            return 0.0;

        // F7.5: aplicar multiplicador de meta-upgrades (Λ)
        double withMeta = baseEnt * GetMetaENTMultiplier();

        double raw = System.Math.Floor(withMeta);
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

    // F7.5: multiplicadores provenientes de meta-upgrades (Prestigio 2)

    /// <summary>
    /// Multiplicador para ENT ganada al prestigiar (ej: +20% si el upgrade está comprado).
    /// </summary>
    public double GetMetaENTMultiplier()
    {
        double mult = 1.0;

        if (metaEntBoost1Bought)
        {
            mult *= 1.20; // +20% ENT
        }

        return mult;
    }

    /// <summary>
    /// Multiplicador para la generación de EM (ej: +15% si el upgrade está comprado).
    /// </summary>
    public double GetMetaEMGenerationMultiplier()
    {
        double mult = 1.0;

        if (metaEmBoost1Bought)
        {
            mult *= 1.15; // +15% EM
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

    // F7.5: meta-upgrades de Λ que afectan la generación de EM
    emPs *= GetMetaEMGenerationMultiplier();

    return emPs;
    }

    /// <summary>
    /// F7.4: Calcula cuántos fragmentos de Wormhole (WHF) se generan por segundo.
    /// Depende de la cantidad de 'wormhole_generator' y de la ADP actual.
    /// </summary>
    private double CalculateWHFps()
    {
        double whfPs = 0.0;

        int wormholeGenerators = 0;

        foreach (var b in buildingStates)   // 👉 usa la misma colección que en CalculateTotalLEps
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue;

            switch (b.def.id)
            {
                case "wormhole_generator":
                    wormholeGenerators += b.level;  // o b.count si tu clase usa otro nombre
                    break;
            }
        }

        // Sin generadores o sin ADP práctica, no generamos nada
        if (wormholeGenerators <= 0 || ADP <= 0.0)
            return 0.0;

        // WHF base por generador: extremadamente bajo
        double basePerGenerator = 0.00001; // 1e-5 WHF/s por generador

        // Factor por ADP (a más ADP, más WHF)
        // Log10 suaviza para que no se dispare demasiado.
        double adpFactor = System.Math.Log10(1.0 + ADP);
        if (adpFactor <= 0.0) return 0.0;

        whfPs = wormholeGenerators * basePerGenerator * adpFactor;

        return whfPs;
    }


    /// <summary>
    /// F7.3: Calcula cuánta ADP/s se genera a partir de los edificios
    /// 'adp_reactor' y 'sc_matrix', usando EM como factor suave.
    /// </summary>
    private double CalculateADPps()
    {
        double adpPs = 0.0;

        int adpReactors = 0;
        int scMatrices = 0;

        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue;

            switch (b.def.id)
            {
                case "adp_reactor":
                    adpReactors += b.level;
                    break;

                case "sc_matrix":
                    scMatrices += b.level;
                    break;
            }
        }

        if (adpReactors <= 0)
            return 0.0;

        // ADP base por reactor (lo ajustamos luego en balance)
        double basePerReactor = 0.001; // ADP/s por reactor

        // Bonus por Matriz SC (ej: +10% ADP por cada una)
        double scBonusMult = 1.0 + 0.10 * scMatrices;

        // EM como factor suave
        double emFactor = 1.0 + System.Math.Log10(1.0 + EM);

        adpPs = adpReactors * basePerReactor * scBonusMult * emFactor;

        return adpPs;
    }


/// <summary>
/// Convierte el EM acumulado en un multiplicador suave de producción de LE.
/// </summary>
private double CalculateEMMultiplier()
{
    if (EM <= 0.0) return 0.0;

    // Cada 100 EM aporta ~5% extra, con rendimientos decrecientes (sqrt)
    double k = 0.15; // 5% base
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

        // 🆕 Si hay niveles cargados desde el save, aplicarlos
        if (SaveService.LastLoadedBuildingLevels != null &&
            SaveService.LastLoadedBuildingLevels.Count > 0)
        {
            ApplyBuildingLevelsFromSave(SaveService.LastLoadedBuildingLevels);
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

    /// <summary>
    /// F7.5: Calcula cuánta Lambda (Λ) ganarías si hicieras Prestigio 2 ahora.
    /// Usa maxLEAlcanzado, totalENTAcumulada, totalADPGenerada y totalWHFGenerada.
    /// La fórmula es intencionalmente sub-exponencial para que no se dispare.
    /// </summary>
    private double CalculateLambdaGain()
    {
        // Si no has avanzado casi nada, no hay Lambda.
        if (maxLEAlcanzado <= 0.0 || totalENTAcumulada <= 0.0)
            return 0.0;

        // Base por LE máximo (ej: 1e6 -> 6, 1e9 -> 9...)
        double baseFromLE = System.Math.Log10(1.0 + maxLEAlcanzado);

        // Factor por ENT acumulada (ej: 100 ENT -> ~6.3 con exp 0.4)
        double entFactor = System.Math.Pow(totalENTAcumulada, 0.4);

        // Factor por ADP total generada (muy suave)
        double adpFactor = System.Math.Pow(1.0 + totalADPGenerada, 0.2);

        // Factor por WHF total generada (algo más fuerte pero controlado)
        double whfFactor = 1.0 + 0.5 * System.Math.Sqrt(totalWHFGenerada);

        // Escalado global para que los valores sean razonables
        double raw = baseFromLE * entFactor * adpFactor * whfFactor * 0.01;

        if (raw < 1.0)
            return 0.0;

        return System.Math.Floor(raw);
    }

    /// <summary>
    /// Valor visible para UI / preview de Lambda a ganar.
    /// </summary>
    public double GetLambdaPreview()
    {
        return CalculateLambdaGain();
    }

    /// <summary>
    /// Indica si hay suficiente progreso para hacer Prestigio 2 (Λ > 0).
    /// </summary>
    public bool CanMetaPrestige()
    {
        return GetLambdaPreview() >= 1.0;
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
        totalENTAcumulada += entGanar;
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

    /// <summary>
    /// F7.5: Ejecuta el Prestigio 2 (Meta-Prestigio).
    /// - Suma Lambda (Λ) usando CalculateLambdaGain().
    /// - Resetea ENT y sus upgrades.
    /// - Hace un reset profundo del run (usando ResetRunForPrestige).
    /// Devuelve la cantidad de Lambda ganada.
    /// </summary>
    public double DoMetaPrestigeReset()
    {
        double lambdaGanar = GetLambdaPreview();
        if (lambdaGanar <= 0.0)
        {
            Debug.Log("[GameState] No hay suficiente progreso para Meta-Prestigio (Λ ganada = 0).");
            return 0.0;
        }

        // 1) Añadir Lambda
        Lambda += lambdaGanar;
        Debug.Log($"[GameState] Meta-Prestigio realizado. Λ ganada: {lambdaGanar}, Λ total: {Lambda}");

        // 2) Resetear moneda de prestigio 1 y sus upgrades
        ENT = 0.0;
        prestigeLeMult1Unlocked = false;
        prestigeAutoBuyFirstUnlocked = false;

        // 3) Resetear todo el run (esto ya borra LE, EM, IP, ADP, WHF, edificios, etc.)
        ResetRunForPrestige();

        // 4) Opcional: reiniciar el máximo LE alcanzado (nuevo ciclo completamente)
        maxLEAlcanzado = 0.0;

        // 5) Guardar estado después del meta-prestigio
        if (SaveService.I != null)
        {
            SaveService.I.Save();
        }

        return lambdaGanar;
    }

    /// <summary>
    /// F7.5: Indica si el sistema de Meta-Prestigio debería estar visible en la UI.
    /// Lo consideramos desbloqueado cuando:
    /// - Has acumulado al menos 50 ENT en total, o
    /// - Ya tienes algo de Lambda.
    /// </summary>
    public bool IsMetaPrestigeUnlocked
    {
        get
        {
            return totalENTAcumulada >= 50.0 || Lambda > 0.0;
        }
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
        ADP = 0.0;
        WHF = 0.0;

        // F7: prestigio 2 (Lambda) y estadísticas
        Lambda = 0.0;
        totalENTAcumulada = 0.0;
        totalADPGenerada = 0.0;
        totalWHFGenerada = 0.0;

        // Meta-upgrades de Lambda
        metaEntBoost1Bought = false;
        metaEmBoost1Bought = false;

        // Upgrades de prestigio 1 (si quieres que el reset debug sea TOTAL)
        prestigeLeMult1Unlocked = false;
        prestigeAutoBuyFirstUnlocked = false;
        prestigeAutoBuyFirstEnabled = true;

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
    // F6.5: corre las automatizaciones asociadas a upgrades de prestigio
    private void RunPrestigeAutomations(double dt)
    {
        // Solo si tienes el upgrade Y está encendido
        if (!prestigeAutoBuyFirstUnlocked) return;
        if (!prestigeAutoBuyFirstEnabled)  return;

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

    // Pon esto DENTRO de la clase GameState
    /// <summary>
    /// F8: Genera LE usando:
    /// - baseLEps (producción base continua)
    /// - edificios:
    ///     * si NO tienen tickInterval -> se comportan como antes (LE/s continuo)
    ///     * si tienen tickInterval y lePerTickBase -> generan LE por tick
    /// 
    /// Los multiplicadores globales (EM, research, achievements) se aplican igual
    /// que en CalculateTotalLEps(), para que el HUD y la producción real estén alineados.
    /// </summary>
    private void GenerateLEFromBaseAndBuildings(double dt)
{
    // Seguridad
    if (dt <= 0.0) return;

    // 1) Multiplicadores globales y bonus planos (para LE)
    double multiplier = 1.0;
    double flatBonus = 0.0;

    if (buildingStates != null)
    {
        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue;

            switch (b.def.bonusType)
            {
                case BuildingBonusType.MultiplierLE:
                    multiplier += b.def.bonusPerLevel * b.level;
                    break;

                case BuildingBonusType.FlatLE:
                    flatBonus += b.def.bonusPerLevel * b.level;
                    break;
            }
        }
    }

    // EM → multiplicador de LE
    double emFactor = 1.0 + emMult;

    // Research global de LE
    double researchFactor = researchGlobalLEMult;

    // Achievements
    double achFactor = 1.0;
    if (AchievementManager.I != null)
    {
        achFactor = AchievementManager.I.GetGlobalLEFactor();
    }

    double worldMult = multiplier * emFactor * researchFactor * achFactor;

    // 2) Producción base continua (sin edificios)
    if (baseLEps > 0.0)
    {
        LE += baseLEps * worldMult * dt;
    }

    // 3) Producción de edificios (ticks + continuo)
    if (buildingStates != null)
    {
        foreach (var b in buildingStates)
        {
            if (b == null || b.def == null) continue;
            if (b.level <= 0) continue;

            var def = b.def;

            // 🔹 3A) Edificios con ticks (B1, B2, B3...)
            if (def.tickInterval > 0.0 && def.lePerTickBase > 0.0)
            {
                b.tickTimer += dt;

                if (b.tickTimer < def.tickInterval)
                    continue;

                int ticks = (int)(b.tickTimer / def.tickInterval);
                if (ticks <= 0) continue;

                b.tickTimer -= ticks * def.tickInterval;

                // --- LE por tick ---
                double lePerTick = def.lePerTickBase * b.level;

                // Buff de Placas de Casimir (B2) sobre Generador de Vacío (B1)
                if (def.id == "vacuum_observer")
                {
                    int casimirLevel = GetBuildingLevel("casimir_panel");
                    if (casimirLevel > 0)
                    {
                        double buffFromCasimir = 1.0 + 0.02 * casimirLevel;
                        buffFromCasimir = System.Math.Min(buffFromCasimir, 2.1);
                        lePerTick *= buffFromCasimir;
                    }
                }

                double leGain = lePerTick * ticks * worldMult;
                LE += leGain;

                // --- EM por tick (para edificios híbridos como B3) ---
                if (def.emPerTickBase > 0.0)
                {
                    // Factor de generación de EM (research + meta-upgrades Λ)
                    double emGenFactor = 1.0;

                    if (ResearchManager.I != null)
                    {
                        emGenFactor *= ResearchManager.I.GetEMGenerationFactor();
                    }

                    emGenFactor *= GetMetaEMGenerationMultiplier();

                    double emPerTick = def.emPerTickBase * b.level * emGenFactor;
                    double emGain = emPerTick * ticks;

                    EM += emGain;

                    // IP pasivo a partir de esta EM (igual que en Tick con EMps)
                    double ipGain = emGain * 0.5;
                    IP += ipGain;

                    // DEBUG (puedes quitarlo luego)
                    Debug.Log($"[TICK-EM] {def.id} lvl {b.level} → {ticks} ticks, +{emGain:0.00} EM, +{ipGain:0.00} IP");
                }

                
            }
            else
            {
                // 🔹 3B) Edificios "clásicos": LE/s continuo
                if (def.baseLEps > 0.0)
                {
                    double leps = def.baseLEps * b.level;
                    LE += leps * worldMult * dt;
                }
            }
        }
    }

    // 4) Bonus plano (LE/s constantes)
    if (flatBonus > 0.0)
    {
        LE += flatBonus * dt;
    }
}



}
