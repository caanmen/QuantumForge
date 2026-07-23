using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// Capa de localización para la interfaz D3 generada por editor. Traduce tanto
/// etiquetas estáticas como textos de estado que los paneles reconstruyen.
/// </summary>
[DefaultExecutionOrder(10000)]
public class D3RuntimeLocalizer : MonoBehaviour
{
    private sealed class CachedText
    {
        public string spanishSource = "";
        public string lastOutput = "";
    }

    private readonly Dictionary<TMP_Text, CachedText> _cache =
        new Dictionary<TMP_Text, CachedText>();

    private struct Pair
    {
        public string es;
        public string en;
        public Pair(string es, string en) { this.es = es; this.en = en; }
    }

    private static readonly Pair[] Pairs =
    {
        new Pair("FÁBRICA — BANCO DE PROCESOS", "FACTORY — PROCESS BANK"),
        new Pair("BANCO DE DIAGNÓSTICO", "DIAGNOSTIC BANK"),
        new Pair("CONSOLA DE PRODUCCIÓN", "PRODUCTION CONSOLE"),
        new Pair("PUERTO DE EXPEDICIONES", "EXPEDITION PORT"),
        new Pair("NÚCLEO DE AUTOMATIZACIÓN", "AUTOMATION CORE"),
        new Pair("CONTROL DE DIAGNÓSTICO", "DIAGNOSTIC CONTROL"),
        new Pair("CONTROL DE CONSOLA", "CONSOLE CONTROL"),
        new Pair("RUTINAS Y PERFILES ONLINE", "ONLINE ROUTINES AND PROFILES"),
        new Pair("INSTALACIONES CONECTADAS", "CONNECTED FACILITIES"),
        new Pair("GESTIÓN DE COLAS", "QUEUE MANAGEMENT"),
        new Pair("MESA DE CALIBRACIÓN", "CALIBRATION TABLE"),
        new Pair("INVESTIGACIONES V4–V6", "V4–V6 RESEARCH"),
        new Pair("VOLVER A INSTALACIONES", "BACK TO FACILITIES"),
        new Pair("VOLVER AL BANCO", "BACK TO BANK"),
        new Pair("GUARDAR PRIORIDAD Y RESERVAS", "SAVE PRIORITY AND RESERVES"),
        new Pair("Solo se repiten recetas ejecutadas y marcadas manualmente.",
            "Only manually executed and marked recipes are repeated."),
        new Pair("Solo se automatiza lo que ya fue ejecutado manualmente.",
            "Only previously manual actions can be automated."),
        new Pair("Construye una instalación para asignar autómatas.",
            "Build a facility to assign automatons."),
        new Pair("GUARDAR POLÍTICA Y RESERVAS", "SAVE POLICY AND RESERVES"),
        new Pair("REGISTRAR TRIÁNGULO ACTUAL", "RECORD CURRENT TRIANGLE"),
        new Pair("REGISTRAR FASE ACTUAL", "RECORD CURRENT PHASE"),
        new Pair("RUTINAS DE COMPRA", "PURCHASE ROUTINES"),
        new Pair("GUARDAR RUTINA N5", "SAVE N5 ROUTINE"),
        new Pair("CARGAR RUTINA N5", "LOAD N5 ROUTINE"),
        new Pair("MARCAR RECETA", "MARK RECIPE"),
        new Pair("DESMARCAR RECETA", "UNMARK RECIPE"),
        new Pair("RECETAS DE FUSIÓN", "FUSION RECIPES"),
        new Pair("AUTOANÁLISIS", "AUTO-ANALYSIS"),
        new Pair("AUTORREPARACIÓN", "AUTO-REPAIR"),
        new Pair("AUTOFUSIÓN", "AUTO-FUSION"),
        new Pair("Prioridad por zona", "Zone priority"),
        new Pair("Orden ascendente", "Ascending order"),
        new Pair("Enlace Cuarto 1", "Room 1 Link"),
        new Pair("Sector de Fusión", "Fusion Sector"),
        new Pair("Soporte Interno", "Internal Support"),
        new Pair("Cámara Instantánea", "Instant Chamber"),
        new Pair("Sin zona", "No zone"),
        new Pair("Reserva LE", "LE reserve"),
        new Pair("Reserva T", "Trace reserve"),
        new Pair("Última ruta manual iniciada.", "Last manual route started."),
        new Pair("MOTOR ONLINE", "ONLINE ENGINE"),
        new Pair("CREAR RUTINA PAUSADA", "CREATE PAUSED ROUTINE"),
        new Pair("ACTIVAR RUTINA", "ENABLE ROUTINE"),
        new Pair("PAUSAR RUTINA", "PAUSE ROUTINE"),
        new Pair("ELIMINAR RUTINA", "DELETE ROUTINE"),
        new Pair("GUARDAR PERFIL", "SAVE PROFILE"),
        new Pair("CARGAR PERFIL", "LOAD PROFILE"),
        new Pair("RUTINA SELECCIONADA", "SELECTED ROUTINE"),
        new Pair("Sin rutinas", "No routines"),
        new Pair("Sin límite", "No limit"),
        new Pair("Prioridad", "Priority"),
        new Pair("Perfil", "Profile"),
        new Pair("Perfiles", "Profiles"),
        new Pair("Activas", "Active"),
        new Pair("Offline externo", "External offline"),
        new Pair("habilitado", "enabled"),
        new Pair("requiere Diagnóstico N5 + Núcleo N5",
            "requires Diagnostic N5 + Core N5"),
        new Pair("Capacidad efectiva", "Effective capacity"),
        new Pair("Capacidad", "Capacity"),
        new Pair("Respuesta", "Response"),
        new Pair("Disciplina", "Discipline"),
        new Pair("Coordinación", "Coordination"),
        new Pair("Canal seleccionado", "Selected channel"),
        new Pair("Asignados", "Assigned"),
        new Pair("Estables", "Stable"),
        new Pair("CONSTRUIR NIVEL", "BUILD LEVEL"),
        new Pair("AMPLIAR A NIVEL", "UPGRADE TO LEVEL"),
        new Pair("NIVEL MÁXIMO", "MAX LEVEL"),
        new Pair("FUNCIONES", "FUNCTIONS"),
        new Pair("AUTORIZACIONES MANUALES", "MANUAL AUTHORIZATIONS"),
        new Pair("Política", "Policy"),
        new Pair("Equilibrio", "Balance"),
        new Pair("Prioridad LE", "LE priority"),
        new Pair("Prioridad Trazas", "Trace priority"),
        new Pair("Próxima compra automática", "Next automatic purchase"),
        new Pair("Fases registradas", "Recorded phases"),
        new Pair("Triángulos básicos", "Basic triangles"),
        new Pair("BLOQUEADO", "LOCKED"),
        new Pair("Sin trabajos.", "No jobs."),
        new Pair("PRODUCCIÓN", "PRODUCTION"),
        new Pair("ENSAMBLAJE", "ASSEMBLY"),
        new Pair("MEJORAS", "UPGRADES"),
        new Pair("CANCELAR", "CANCEL"),
        new Pair("INVESTIGACIÓN", "RESEARCH"),
        new Pair("ENSAMBLAR", "ASSEMBLE"),
        new Pair("PRODUCIR", "PRODUCE"),
        new Pair("INVENTARIO", "INVENTORY"),
        new Pair("BONIFICACIONES DEL BANCO", "BANK BONUSES"),
        new Pair("Progreso", "Progress"),
        new Pair("Tiempo bruto", "Raw time"),
        new Pair("Costo bruto", "Raw cost"),
        new Pair("RECETAS", "RECIPES"),
        new Pair("ejecutadas manualmente", "manually executed"),
        new Pair("marcadas para repetir", "marked for repetition"),
        new Pair("Sin recetas manuales", "No manual recipes"),
        new Pair("GUARDAR", "SAVE"),
        new Pair("CARGAR", "LOAD"),
        new Pair("CERRAR", "CLOSE"),
        new Pair("VOLVER", "BACK"),
        new Pair("NIVEL", "LEVEL"),
        new Pair("Sí", "Yes"),
        new Pair("No", "No")
    };

    private void LateUpdate()
    {
        if (LocalizationManager.I == null) return;
        bool english = LocalizationManager.I.CurrentLanguage ==
            LocalizationManager.Language.EN;
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null || string.IsNullOrEmpty(texts[i].text)) continue;
            TMP_Text text = texts[i];
            string current = RepairAccumulatedText(text.text);
            if (!_cache.TryGetValue(text, out CachedText cached))
            {
                // Los paneles D3 generan su contenido original en español.
                cached = new CachedText { spanishSource = current };
                _cache[text] = cached;
            }
            else if (current != cached.lastOutput)
            {
                // Un panel reconstruyó un estado dinámico: esa nueva salida es
                // la fuente española limpia para ambos idiomas.
                cached.spanishSource = current;
            }

            string value = english
                ? TranslateText(cached.spanishSource, true)
                : cached.spanishSource;
            cached.lastOutput = value;
            if (text.text != value) text.text = value;
        }
    }

    private static string RepairAccumulatedText(string value)
    {
        if (string.IsNullOrEmpty(value)) return value ?? "";
        // Corrige sesiones que recibieron el bug anterior CANCELARARAR...
        while (value.Contains("CANCELARAR"))
            value = value.Replace("CANCELARAR", "CANCELAR");
        return value;
    }

    public static string TranslateText(string value, bool english)
    {
        if (string.IsNullOrEmpty(value)) return value ?? "";
        for (int j = 0; j < Pairs.Length; j++)
            value = english
                ? value.Replace(Pairs[j].es, Pairs[j].en)
                : value.Replace(Pairs[j].en, Pairs[j].es);
        return value;
    }
}
