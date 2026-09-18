# Matriz de correcciones — Dimensión 3

Estado: contrato estático para las pantallas 01–10. La composición de Planta de
Producción V2 es la base visual aprobada.

## Contrato compartido

- Encabezado: título sin número documental; `3.25M LE`, `4.2K TRAZAS` y
  `56 AUTÓMATAS`; iconos idénticos a Planta aprobada.
- Inventario QA: 40 MK1 Normal reservados por Investigación, 10 MK2 Rápidos
  (2 asignados y estabilizados a Capacidad de Consola), 5 MK3 Normal y 1 MK4 Normal.
- Piezas: `CHASIS`, `SISTEMA MOTRIZ`, `HERRAMIENTA`, `MÓDULO DE CONTROL`,
  `REGULADOR`; nunca MOTOR, CONTROL ni HERRAM. como nombres visibles.
- Colas: iconos únicos para Piezas, Ensamble, Planos y Obras; conteos comunes
  `3/10 · 00:06`, `1/10 · 00:24`, `1/10 · 20:00`, `LIBRE · 0/1`.
- Instalaciones: símbolos de navegación únicos — terminal para Consola, cruz técnica
  para Diagnóstico, ancla para Puerto y módulo de encendido para Núcleo. Las máquinas
  físicas pueden conservar ilustraciones propias.
- Navegación inferior estable: PLANTA / TALLER / CONTROL; sólo una selección ámbar.
- Acciones válidas usan ámbar; acciones bloqueadas explican el requisito; cancelación
  y eliminación usan rojo óxido.
- Animación futura: maquinaria, luces e indicadores en capas separadas. Encabezado,
  textos, marcos, botones, pestañas, colas y navegación permanecen inmóviles.

## 01 — Taller de Ingeniería / Calibración

Función: registrar lecturas de calibración de las cinco piezas y guardar el perfil.
Código fuente: `D3CalibrationPanelUI` y `D3CalibrationSystem`.
Correcciones: nombres completos de piezas; pestañas CALIBRACIÓN / INVESTIGACIÓN /
INSTALACIONES; Chasis V1 seleccionado; Superior 35, Central 40, Inferior 25,
total 100/100; `REGISTRADAS 1/5`; `REGISTRAR CHASIS`; perfil bloqueado hasta 5/5.
Selección inferior: TALLER.

## 02 — Sala de Control / Núcleo

Función: consultar instalaciones y ampliar el Núcleo de Automatización.
Código fuente: `D3FacilitiesPanelUI`, `D3FacilitySystem`, `Dimension3Catalog`.
Estado: Consola N2, Diagnóstico N4, Puerto N1 y Núcleo N2.
Siguiente Núcleo N3: 2.000.000 LE + 1.500 Trazas, 45:00, 1 ensamblaje MK4;
acción exacta `AMPLIAR A NIVEL 3`.
Selección inferior: CONTROL.

## 03 — Autómatas

Función: consultar grupos y asignarlos a canales de instalaciones.
Código fuente: `Dimension3PanelUI`, `D3InventorySystem`, `D3FacilitySystem`.
Grupos visibles: MK1 Normal ×40, MK2 Rápido ×10, MK3 Normal ×5, MK4 Normal ×1.
Selección: MK2 Rápido; total del grupo 10, disponibles 8, asignados al canal 2.
Instalación: CONSOLA DE PRODUCCIÓN; canal: CAPACIDAD; asignación estabilizada.
Acciones: `ASIGNAR +1` y `RETIRAR -1`.
Selección inferior: PLANTA.

## 04 — Investigación

Función: reservar equipo y desbloquear la producción V4 de una pieza.
Código fuente: `D3ResearchPanelUI`, `D3ResearchSystem`, `Dimension3Catalog`.
Estado: Módulo de Control V4 en curso; 100.000 LE + 100 Trazas; 20:00;
potencia mínima 50; equipo 40 × MK1 Normal; versiones V5/V6 bloqueadas por cadena.
Acciones: estado `YA ESTÁ EN COLA` y `CANCELAR INVESTIGACIÓN`.
Selección inferior: TALLER.

## 05 — Construcción y mejoras

Función: consultar, construir/ampliar instalaciones y asignar capacidad.
Código fuente: `D3FacilitiesPanelUI`, `D3FacilitySystem`, `Dimension3Catalog`.
Estado seleccionado: Consola de Producción N2; siguiente N3 cuesta 250.000 LE +
250 Trazas, tarda 15:00 y requiere 5 ensamblajes MK2.
Canal Capacidad: 2 MK2 Rápidos estabilizados.
Acciones: `AMPLIAR A NIVEL 3` y `ASIGNAR +1`.
Selección inferior: CONTROL.

## 06 — Consola

Función: configurar política de compra, reservas y autorizaciones.
Código fuente: `D3ConsolePanelUI` y `D3ConsoleSystem`.
Estado: Consola N2; política Equilibrio; reserva 10.000 LE / 100 Trazas;
autorizaciones Higgs Sí y Tetraquarks Sí.
Opciones reales: Prioridad LE / Prioridad Trazas / Equilibrio; reservas LE
0/1K/10K/100K/1M y Trazas 0/100/1K/10K.
Acciones: `GUARDAR POLÍTICA`, registro de circuito bloqueado hasta N5 y `ABRIR RUTINAS`.
Selección inferior: CONTROL.

## 07 — Diagnóstico

Función: configurar autoanálisis, autorreparación, autofusión, prioridad y reservas.
Código fuente: `D3DiagnosticPanelUI` y `D3DiagnosticSystem`.
Estado: Diagnóstico N4; autoanálisis ON, autorreparación ON, autofusión OFF;
Prioridad por zona / Sector de Fusión; reserva 10.000 LE / 100 Trazas.
Acciones: `MARCAR RECETA`, `GUARDAR AJUSTES`; guardar/cargar rutina requiere N5.
Selección inferior: CONTROL.

## 08 — Puerto de Expediciones

Función: mostrar destinos D1 aprendidos y crear rutinas permitidas.
Código fuente: `D3AutomationCatalog`, `D3FacilitySystem` y sistemas D1 autorizados.
Estado: Puerto N1, enlace D1 activo y patrón manual aprendido.
Destinos: Cinturón Mineral, Cementerio de Naves, Sondas a la Deriva y Nave Abandonada.
Acciones: crear rutina de Barrido simple, crear rutina de Repetir ruta y abrir Rutinas.
Selección inferior: CONTROL.

## 09 — Rutinas

Función: crear, activar, pausar, eliminar y guardar perfiles de automatización.
Código fuente: `D3AutomationPanelUI`, `D3AutomationSystem`, `D3AutomationCatalog`.
Ejemplo válido: acción `BARRIDO SIMPLE`, Prioridad 3, Reserva 100 y límite
5 ejecuciones; una rutina activa de un máximo de tres y Perfil 1.
Acciones: `CREAR NUEVA RUTINA`, `PAUSAR RUTINA`, `ELIMINAR`, `GUARDAR PERFIL`,
`CARGAR PERFIL`.
Selección inferior: CONTROL.

## 10 — Panel global de colas

Función: seleccionar una cola y un trabajo, consultar detalle y cancelar el elegido.
Código fuente: `D3QueuesPanelUI` y `D3JobQueueSystem`.
Correcciones: un único cierre superior; ningún botón de cancelación por cada fila;
un único `CANCELAR SELECCIONADO`; trabajo activo requiere segunda pulsación en cinco
segundos y no devuelve recursos, trabajo pendiente devuelve todo.
Datos: Chasis V1 activo; Chasis V2 y Sistema Motriz V1 pendientes; MK1 Normal activo;
Módulo de Control V4 activo; Instalaciones sin trabajos.

## Estado de aprobación

Estas decisiones autorizan producir candidatas estáticas. Cada imagen queda pendiente
de revisión del usuario. No son capturas reales ni assets finales.
