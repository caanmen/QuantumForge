using System.Collections.Generic;
using System.Globalization;


public static class PresentationTextCatalog
{
    private sealed class Entry
    {
        public readonly string es;
        public readonly string en;
        public Entry(string es, string en) { this.es = es; this.en = en; }
    }

    private static readonly Dictionary<string, Entry> Entries =
        new Dictionary<string, Entry>
        {
            { "return.title", new Entry("REGISTRO DE AUSENCIA", "AWAY LOG") },
            { "load.failed.title", new Entry("NO SE PUDO CARGAR", "COULD NOT LOAD SAVE") },
            { "load.failed.paused", new Entry("PARTIDA EN PAUSA", "GAME PAUSED") },
            { "load.failed.protected", new Entry("GUARDADO PROTEGIDO", "SAVE PROTECTED") },
            { "load.failed.help", new Entry("El progreso está detenido.\nPuedes reintentar la carga o cerrar el juego.", "Progress is paused.\nYou can retry loading or close the game.") },
            { "load.failed.retry", new Entry("REINTENTAR CARGA", "RETRY LOADING") },
            { "return.away_time", new Entry("ESTUVISTE FUERA  <size=48>{0}</size>", "YOU WERE AWAY  <size=48>{0}</size>") },
            { "return.applied_time", new Entry("<size=27>PROGRESO APLICADO  {0}  ·  MÁXIMO 12 H</size>", "<size=27>PROGRESS APPLIED  {0}  ·  12 H MAXIMUM</size>") },
            { "return.complete", new Entry("Simulación completada", "Simulation complete") },
            { "return.time", new Entry("TIEMPO PROCESADO", "PROCESSED TIME") },
            { "return.applied", new Entry("{0} / límite 12 h donde corresponde", "{0} / 12 h limit where applicable") },
            { "return.d2.title", new Entry("DIMENSIÓN 2", "DIMENSION 2") },
            { "return.d2", new Entry("Los sistemas avanzaron durante tu ausencia", "Systems progressed while you were away") },
            { "return.d3.title", new Entry("DIMENSIÓN 3", "DIMENSION 3") },
            { "return.d3", new Entry("Los procesos autorizados continuaron", "Authorized processes continued") },
            { "return.balance", new Entry("RECURSOS OBTENIDOS", "RESOURCES RECEIVED") },
            { "return.balance.none", new Entry("No se obtuvieron LE, Trazas ni Energía", "No LE, Traces, or Energy received") },
            { "return.new", new Entry("Nuevo sistema disponible: {0}.", "New system available: {0}.") },
            { "return.resume", new Entry("Siguiente acción: retoma el objetivo desde su estado real.", "Next action: resume the objective from its current state.") },
            { "return.continue", new Entry("CONTINUAR", "CONTINUE") },
            { "return.open", new Entry("IR AL OBJETIVO", "GO TO OBJECTIVE") },
            { "help.review", new Entry("REPASAR", "REVIEW") },
            { "help.close", new Entry("CERRAR", "CLOSE") },
            { "help.d2.title", new Entry("GUÍA DE DIMENSIÓN 2", "DIMENSION 2 GUIDE") },
            { "help.d2.body", new Entry("AHORA: sigue la acción principal de la pantalla actual.\nDESPUÉS: el siguiente sistema aparecerá cuando el progreso real lo vuelva relevante.\nCerrar esta ayuda nunca bloquea acciones ni reinicia una introducción.", "NOW: follow the main action on the current screen.\nNEXT: the following system appears when actual progress makes it relevant.\nClosing this help never blocks actions or restarts an introduction.") },
            { "help.d3.title", new Entry("GUÍA DE DIMENSIÓN 3", "DIMENSION 3 GUIDE") },
            { "help.d3.body", new Entry("El ciclo aprendido es asignar, producir, completar el set, ensamblar y volver a asignar. Las actividades continúan al cambiar de pantalla.", "The learned cycle is assign, produce, complete the set, assemble, and assign again. Activities continue when changing screens.") },
            { "feature.unknown", new Entry("progreso relevante", "relevant progress") },

            { "d3.onboarding.shared.next", new Entry("DESPUÉS", "NEXT") },
            { "d3.onboarding.progress.cost_time", new Entry("{0} LE + {1} T · {2} s", "{0} LE + {1} T · {2} s") },
            { "d3.onboarding.progress.active_time", new Entry("{0} s restantes", "{0} s remaining") },
            { "d3.onboarding.action.waiting", new Entry("ESPERANDO PRODUCCIÓN", "WAITING FOR PRODUCTION") },
            { "d3.onboarding.action.produce_part", new Entry("PRODUCIR {0}", "PRODUCE {0}") },
            { "d3.onboarding.part.chassis", new Entry("Chasis", "Chassis") },
            { "d3.onboarding.part.motor", new Entry("Sistema Motriz", "Drive System") },
            { "d3.onboarding.part.tool", new Entry("Herramienta", "Tool") },
            { "d3.onboarding.part.control", new Entry("Módulo de Control", "Control Module") },
            { "d3.onboarding.part.regulator", new Entry("Regulador", "Regulator") },

            { "d3.onboarding.assign_initial.title", new Entry("AHORA · PON EN MARCHA EL BANCO", "NOW · START THE BANK") },
            { "d3.onboarding.assign_initial.body", new Entry("Asigna el MK1 inicial al canal Potencia.", "Assign the initial MK1 to the Power channel.") },
            { "d3.onboarding.assign_initial.progress", new Entry("MK1 libres: {0} · Potencia", "Free MK1: {0} · Power") },
            { "d3.onboarding.assign_initial.action", new Entry("ASIGNAR MK1", "ASSIGN MK1") },
            { "d3.onboarding.assign_initial.next", new Entry("Fabricarás un Chasis V1.", "You will manufacture a V1 Chassis.") },
            { "d3.onboarding.first_part.title", new Entry("AHORA · FABRICA UNA PIEZA", "NOW · MANUFACTURE A PART") },
            { "d3.onboarding.first_part.body", new Entry("Añade un Chasis V1 ×1 a Producción.", "Add one V1 Chassis to Production.") },
            { "d3.onboarding.first_part.action", new Entry("PRODUCIR CHASIS V1", "PRODUCE V1 CHASSIS") },
            { "d3.onboarding.first_part.action_active", new Entry("CHASIS EN PRODUCCIÓN", "CHASSIS IN PRODUCTION") },
            { "d3.onboarding.first_part.next", new Entry("Completarás el set de cinco piezas V1.", "You will complete the five-part V1 set.") },
            { "d3.onboarding.complete_set.title", new Entry("AHORA · COMPLETA UN SET V1", "NOW · COMPLETE A V1 SET") },
            { "d3.onboarding.complete_set.body", new Entry("Produce una pieza de cada tipo.", "Produce one part of each type.") },
            { "d3.onboarding.complete_set.progress", new Entry("SET V1 · {0}/5 recibidas · {1}/5 preparadas", "V1 SET · {0}/5 received · {1}/5 prepared") },
            { "d3.onboarding.complete_set.next", new Entry("Un set completo permite ensamblar un MK1.", "A complete set allows you to assemble an MK1.") },
            { "d3.onboarding.first_assembly.title", new Entry("AHORA · ENSAMBLA TU PRIMER AUTÓMATA", "NOW · ASSEMBLE YOUR FIRST AUTOMATON") },
            { "d3.onboarding.first_assembly.title_restock", new Entry("AHORA · REPÓN EL SET V1", "NOW · REBUILD THE V1 SET") },
            { "d3.onboarding.first_assembly.body", new Entry("Consume cinco piezas V1 y entrega un MK1 Normal.", "Consume five V1 parts and receive a Normal MK1.") },
            { "d3.onboarding.first_assembly.body_restock", new Entry("La cola se canceló: prepara de nuevo las piezas faltantes.", "The queue was canceled: prepare the missing parts again.") },
            { "d3.onboarding.first_assembly.progress_set", new Entry("SET V1 · {0}/5", "V1 SET · {0}/5") },
            { "d3.onboarding.first_assembly.action", new Entry("ENSAMBLAR MK1", "ASSEMBLE MK1") },
            { "d3.onboarding.first_assembly.action_active", new Entry("ENSAMBLAJE EN CURSO", "ASSEMBLY IN PROGRESS") },
            { "d3.onboarding.first_assembly.next", new Entry("Asignarás el nuevo MK1 al Banco.", "You will assign the new MK1 to the Bank.") },
            { "d3.onboarding.assign_new.title", new Entry("AHORA · ASIGNA EL NUEVO MK1", "NOW · ASSIGN THE NEW MK1") },
            { "d3.onboarding.assign_new.body", new Entry("Refuerza el canal Potencia con el Autómata fabricado.", "Reinforce the Power channel with the manufactured Automaton.") },
            { "d3.onboarding.assign_new.progress", new Entry("Fabricados: {0} · libres: {1} · asignados: {2}", "Manufactured: {0} · free: {1} · assigned: {2}") },
            { "d3.onboarding.assign_new.action", new Entry("ASIGNAR NUEVO MK1", "ASSIGN NEW MK1") },
            { "d3.onboarding.assign_new.next", new Entry("Cerrarás el primer ciclo de la Fábrica.", "You will complete the Factory's first cycle.") },
            { "d3.onboarding.completed.title", new Entry("AHORA · PREPARA BANCO N2", "NOW · PREPARE BANK N2") },
            { "d3.onboarding.completed.body", new Entry("Construye y asigna los MK1 que exija el Banco.", "Build and assign the MK1 units required by the Bank.") },
            { "d3.onboarding.completed.progress", new Entry("Capacidad efectiva: {0} / {1}", "Effective capacity: {0} / {1}") },
            { "d3.onboarding.completed.action", new Entry("MEJORAR BANCO", "UPGRADE BANK") },
            { "d3.onboarding.completed.next", new Entry("El sistema real validará recursos y capacidad.", "The actual system will validate resources and capacity.") },

            { "d3.onboarding.help.title", new Entry("AYUDA · DIMENSIÓN 3", "HELP · DIMENSION 3") },
            { "d3.onboarding.help.review_title", new Entry("REPASAR · PRIMER CICLO", "REVIEW · FIRST CYCLE") },
            { "d3.onboarding.help.review_body", new Entry("1. Asigna un Autómata al Banco.\n2. Produce un Chasis y completa las cinco piezas V1.\n3. Ensambla un MK1 Normal.\n4. Asigna el nuevo MK1 para reforzar el Banco.\n\nAsignar no consume unidades. Producción y Ensamblaje usan colas independientes y continúan hasta 12 h sin conexión.", "1. Assign an Automaton to the Bank.\n2. Produce a Chassis and complete all five V1 parts.\n3. Assemble a Normal MK1.\n4. Assign the new MK1 to reinforce the Bank.\n\nAssignments do not consume units. Production and Assembly use separate queues and continue for up to 12 h while offline.") },
            { "d3.onboarding.help.assign_initial", new Entry("Selecciona Potencia y asigna el MK1. La unidad no se consume, puede retirarse y tarda 30 s en estabilizarse.", "Select Power and assign the MK1. The unit is not consumed, can be removed, and takes 30 s to stabilize.") },
            { "d3.onboarding.help.first_part", new Entry("Produce un Chasis V1 ×1. El trabajo continúa al cambiar de pantalla y durante tu ausencia, hasta 12 h.", "Produce one V1 Chassis. The job continues when changing screens and while you are away, for up to 12 h.") },
            { "d3.onboarding.help.complete_set", new Entry("Completa un set con Chasis, Sistema Motriz, Herramienta, Control y Regulador. EN COLA ya cuenta como preparado.", "Complete a set with a Chassis, Drive System, Tool, Control Module, and Regulator. QUEUED already counts as prepared.") },
            { "d3.onboarding.help.first_assembly", new Entry("Ensamblar consume las cinco piezas. Producción y Ensamblaje son colas independientes; puedes preparar otro set en paralelo.", "Assembly consumes all five parts. Production and Assembly are separate queues; you can prepare another set in parallel.") },
            { "d3.onboarding.help.assign_new", new Entry("El nuevo MK1 está libre. Asígnalo a Potencia para reforzar el Banco.", "The new MK1 is free. Assign it to Power to reinforce the Bank.") },
            { "d3.onboarding.help.completed", new Entry("REPASAR muestra el ciclo aprendido sin reiniciar el tutorial ni cambiar los requisitos económicos.", "REVIEW shows the cycle you learned without restarting the tutorial or changing its economic requirements.") },

            { "d3.onboarding.coachmark", new Entry("MK1 NORMAL → POTENCIA\nAsignar no consume la unidad y puedes retirarla.", "NORMAL MK1 → POWER\nAssignment does not consume the unit, and you can remove it.") },
            { "d3.onboarding.nav.calibration", new Entry("CALIBRACIÓN · OPCIONAL", "CALIBRATION · OPTIONAL") },
            { "d3.onboarding.nav.research_available", new Entry("INVESTIGACIÓN · DISPONIBLE", "RESEARCH · AVAILABLE") },
            { "d3.onboarding.nav.research_soon", new Entry("INVESTIGACIÓN · PRÓXIMAMENTE", "RESEARCH · COMING SOON") },
            { "d3.onboarding.nav.facilities_available", new Entry("INSTALACIONES · DISPONIBLES", "FACILITIES · AVAILABLE") },
            { "d3.onboarding.nav.facilities_soon", new Entry("INSTALACIONES · PRÓXIMAMENTE", "FACILITIES · COMING SOON") },
            { "d3.onboarding.celebration", new Entry("PRIMER CICLO COMPLETADO\nYa sabes producir piezas, ensamblar Autómatas y reforzar el Banco.\n\nMK1 inicial: {0} · fabricados: {1} · libres: {2} · asignados: {3}\n\nSiguiente meta · capacidad efectiva {4} / {5} para Banco N2.", "FIRST CYCLE COMPLETE\nYou now know how to produce parts, assemble Automatons, and reinforce the Bank.\n\nInitial MK1: {0} · manufactured: {1} · free: {2} · assigned: {3}\n\nNext goal · effective capacity {4} / {5} for Bank N2.") },
            { "d3.onboarding.status", new Entry("BANCO N{0} · LE {1} · TRAZAS {2}\nMK1 disponibles: {3}", "BANK N{0} · LE {1} · TRACES {2}\nAvailable MK1: {3}") },
            { "d3.onboarding.inventory.set", new Entry("SET V1 · {0}/5", "V1 SET · {0}/5") },
            { "d3.onboarding.inventory.summary", new Entry("AUTÓMATAS MK1 NORMAL\nInicial otorgado: {0}\nFabricados: {1}\nTotales: {2} · libres: {3} · asignados: {4}", "NORMAL MK1 AUTOMATONS\nInitial grant: {0}\nManufactured: {1}\nTotal: {2} · free: {3} · assigned: {4}") },
            { "d3.onboarding.inventory.owned", new Entry(" [LISTO]", " [READY]") },
            { "d3.onboarding.inventory.queued", new Entry(" · EN COLA", " · QUEUED") },
            { "d3.onboarding.inventory.missing", new Entry(" —", " —") },
            { "d3.onboarding.part_button", new Entry("{0} · ×1 · {1} LE + {2} T · {3} s{4}", "{0} · ×1 · {1} LE + {2} T · {3} s{4}") },
            { "d3.onboarding.queue.production", new Entry("PRODUCCIÓN", "PRODUCTION") },
            { "d3.onboarding.queue.assembly", new Entry("ENSAMBLAJE", "ASSEMBLY") },
            { "d3.onboarding.queue.empty", new Entry("Sin trabajos.", "No jobs.") },
            { "d3.onboarding.queue.active", new Entry("ACTIVO — ", "ACTIVE — ") },
            { "d3.onboarding.queue.pending", new Entry("PENDIENTE — ", "PENDING — ") },
            { "d3.onboarding.queue.part", new Entry("{0} V{1}", "{0} V{1}") },
            { "d3.onboarding.queue.facility", new Entry("Banco de Procesos nivel {0}", "Process Bank level {0}") },
            { "d3.onboarding.queue.automaton", new Entry("MK{0} Normal", "Normal MK{0}") },
            { "d3.onboarding.queue.job_suffix", new Entry(" ×{0} — {1} s", " ×{0} — {1} s") },
            { "d3.onboarding.queue.more", new Entry("… y {0} más.", "… and {0} more.") },

            { "d3.onboarding.notice.factory_ready", new Entry("Banco de Procesos disponible. Producción V1 preparada.", "Process Bank available. V1 Production is ready.") },
            { "d3.onboarding.notice.cycle_continue", new Entry("Primer ciclo completado · prepara el Banco de Procesos N2.", "First cycle complete · prepare Process Bank N2.") },
            { "d3.onboarding.notice.part_queued", new Entry("{0} añadido a Producción.", "{0} added to Production.") },
            { "d3.onboarding.notice.assembly_started", new Entry("Ensamblaje iniciado · Producción y Ensamblaje avanzan por separado.", "Assembly started · Production and Assembly advance separately.") },
            { "d3.onboarding.notice.assigned", new Entry("MK1 asignado · Estabilización 0:30. Puedes comenzar a producir mientras se estabiliza.", "MK1 assigned · Stabilization 0:30. You can begin production while it stabilizes.") },
            { "d3.onboarding.notice.removed", new Entry("Autómata retirado y disponible inmediatamente.", "Automaton removed and available immediately.") },
            { "d3.onboarding.notice.upgrade_queued", new Entry("Mejora del Banco de Procesos añadida a la cola.", "Process Bank upgrade added to the queue.") },
            { "d3.onboarding.notice.chassis_received", new Entry("Chasis V1 recibido · SET V1 · 1/5.", "V1 Chassis received · V1 SET · 1/5.") },
            { "d3.onboarding.notice.set_complete", new Entry("SET V1 completo · Ensamblaje disponible.", "V1 SET complete · Assembly available.") },
            { "d3.onboarding.notice.mk1_built", new Entry("MK1 construido · {0} operarios totales · {1} libre.", "MK1 built · {0} total operators · {1} free.") },
            { "d3.onboarding.notice.cycle_complete", new Entry("Nuevo MK1 asignado · primer ciclo completado.", "New MK1 assigned · first cycle complete.") },
            { "d3.onboarding.error.part_le", new Entry("Faltan {0} LE para {1}.", "You need {0} more LE for {1}.") },
            { "d3.onboarding.error.part_traces", new Entry("Faltan {0} Trazas para {1}.", "You need {0} more Traces for {1}.") },
            { "d3.onboarding.error.part", new Entry("No se pudo añadir {0} a Producción.", "Could not add {0} to Production.") },
            { "d3.onboarding.error.assembly", new Entry("No se pudo iniciar el ensamblaje. Comprueba el set V1 y los recursos.", "Could not start assembly. Check the V1 set and your resources.") },
            { "d3.onboarding.error.assignment", new Entry("No se pudo cambiar la asignación del Banco.", "Could not change the Bank assignment.") },
            { "d3.onboarding.error.upgrade", new Entry("No se pudo añadir la mejora del Banco a la cola.", "Could not add the Bank upgrade to the queue.") }
        };

    public static readonly string[] RequiredKeys =
    {
        "return.title", "return.complete", "return.time", "return.applied",
        "return.d2.title", "return.d2", "return.d3.title", "return.d3",
        "return.balance", "return.balance.none", "return.new", "return.continue",
        "help.review", "help.close", "help.d2.title", "help.d2.body",
        "help.d3.title", "help.d3.body", "feature.unknown"
    };

    public static readonly string[] D3OnboardingRequiredKeys =
    {
        "d3.onboarding.shared.next", "d3.onboarding.progress.cost_time",
        "d3.onboarding.progress.active_time", "d3.onboarding.action.waiting",
        "d3.onboarding.action.produce_part", "d3.onboarding.part.chassis",
        "d3.onboarding.part.motor", "d3.onboarding.part.tool",
        "d3.onboarding.part.control", "d3.onboarding.part.regulator",
        "d3.onboarding.assign_initial.title", "d3.onboarding.assign_initial.body",
        "d3.onboarding.assign_initial.progress", "d3.onboarding.assign_initial.action",
        "d3.onboarding.assign_initial.next", "d3.onboarding.first_part.title",
        "d3.onboarding.first_part.body", "d3.onboarding.first_part.action",
        "d3.onboarding.first_part.action_active", "d3.onboarding.first_part.next",
        "d3.onboarding.complete_set.title", "d3.onboarding.complete_set.body",
        "d3.onboarding.complete_set.progress", "d3.onboarding.complete_set.next",
        "d3.onboarding.first_assembly.title", "d3.onboarding.first_assembly.title_restock",
        "d3.onboarding.first_assembly.body", "d3.onboarding.first_assembly.body_restock",
        "d3.onboarding.first_assembly.progress_set", "d3.onboarding.first_assembly.action",
        "d3.onboarding.first_assembly.action_active", "d3.onboarding.first_assembly.next",
        "d3.onboarding.assign_new.title", "d3.onboarding.assign_new.body",
        "d3.onboarding.assign_new.progress", "d3.onboarding.assign_new.action",
        "d3.onboarding.assign_new.next", "d3.onboarding.completed.title",
        "d3.onboarding.completed.body", "d3.onboarding.completed.progress",
        "d3.onboarding.completed.action", "d3.onboarding.completed.next",
        "d3.onboarding.help.title", "d3.onboarding.help.review_title",
        "d3.onboarding.help.review_body", "d3.onboarding.help.assign_initial",
        "d3.onboarding.help.first_part", "d3.onboarding.help.complete_set",
        "d3.onboarding.help.first_assembly", "d3.onboarding.help.assign_new",
        "d3.onboarding.help.completed", "d3.onboarding.coachmark",
        "d3.onboarding.nav.calibration", "d3.onboarding.nav.research_available",
        "d3.onboarding.nav.research_soon", "d3.onboarding.nav.facilities_available",
        "d3.onboarding.nav.facilities_soon",
        "d3.onboarding.celebration", "d3.onboarding.status",
        "d3.onboarding.inventory.set", "d3.onboarding.inventory.summary",
        "d3.onboarding.inventory.owned", "d3.onboarding.inventory.queued",
        "d3.onboarding.inventory.missing", "d3.onboarding.part_button",
        "d3.onboarding.queue.production", "d3.onboarding.queue.assembly",
        "d3.onboarding.queue.empty", "d3.onboarding.queue.active",
        "d3.onboarding.queue.pending", "d3.onboarding.queue.part",
        "d3.onboarding.queue.facility", "d3.onboarding.queue.automaton",
        "d3.onboarding.queue.job_suffix", "d3.onboarding.queue.more",
        "d3.onboarding.notice.factory_ready", "d3.onboarding.notice.cycle_continue",
        "d3.onboarding.notice.part_queued", "d3.onboarding.notice.assembly_started",
        "d3.onboarding.notice.assigned", "d3.onboarding.notice.removed",
        "d3.onboarding.notice.upgrade_queued", "d3.onboarding.notice.chassis_received",
        "d3.onboarding.notice.set_complete", "d3.onboarding.notice.mk1_built",
        "d3.onboarding.notice.cycle_complete", "d3.onboarding.error.part_le",
        "d3.onboarding.error.part_traces", "d3.onboarding.error.part",
        "d3.onboarding.error.assembly", "d3.onboarding.error.assignment",
        "d3.onboarding.error.upgrade"
    };

    public static string Get(string key, bool english)
    {
        if (string.IsNullOrEmpty(key)) return "";
        return Entries.TryGetValue(key, out Entry entry)
            ? english ? entry.en : entry.es
            : key;
    }

    public static bool HasBothLanguages(string key)
    {
        return Entries.TryGetValue(key, out Entry entry) &&
            !string.IsNullOrEmpty(entry.es) && !string.IsNullOrEmpty(entry.en);
    }

    public static string Current(string key)
    {
        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
        return Get(key, english);
    }

    public static string Format(string key, bool english, params object[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, Get(key, english), args);
    }

    public static string CurrentFormat(string key, params object[] args)
    {
        bool english = LocalizationManager.I != null &&
            LocalizationManager.I.CurrentLanguage == LocalizationManager.Language.EN;
        return Format(key, english, args);
    }
}
