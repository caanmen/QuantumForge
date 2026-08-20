# Dimensión 3 — resumen de continuidad visual

Fecha de revisión: 2026-08-19

## Objetivo

Diseñar la identidad visual de la Dimensión 3 como un complejo fabril industrial, diferente del juego base y de las Dimensiones 1 y 2, pero realizable en Unity. Por ahora se trabaja sobre composiciones estáticas; la interacción y la animación se incorporarán después.

Las imágenes incluidas en este paquete son conceptos visuales. No son capturas reales de Unity, assets finales ni pantallas aprobadas al 95 %.

## Arquitectura acordada

La dimensión se organiza en tres espacios principales, siete subpantallas funcionales y un panel global:

### Pantallas principales

1. `00_Principal_Planta_Produccion.png`
   - Fabricación de las cinco piezas.
   - Selección V1–V6 y cantidad.
   - Ensamblaje MK1–MK6.
   - Acceso a Autómatas.
   - Resumen de las cuatro colas.

2. `01_Principal_Taller_Ingenieria_Calibracion.png`
   - Calibración de Chasis, Motor, Herramienta, Control y Regulador.
   - Perfiles de rasgos y repeticiones automáticas según el nivel del Banco de Procesos.
   - Acceso a Investigación y Construcción/Mejoras.

3. `02_Principal_Sala_Control_Nucleo.png`
   - Operación de Consola, Diagnóstico, Puerto y Núcleo.
   - Acceso a Instalaciones y Rutinas.
   - Progreso hacia la culminación de la dimensión.

### Subpantallas

4. `03_Subpantalla_Automatas.png`
5. `04_Subpantalla_Investigacion.png`
6. `05_Subpantalla_Construccion_Mejoras.png`
7. `06_Subpantalla_Consola.png`
8. `07_Subpantalla_Diagnostico.png`
9. `08_Subpantalla_Puerto_Expedicion.png`
10. `09_Subpantalla_Rutinas.png`

### Panel global

11. `10_Panel_Global_Colas.png`
   - Producción, Ensamblaje, Investigación e Instalaciones.
   - Debe poder abrirse desde cualquiera de las tres salas.

## Decisión de navegación

- En Planta se fabrican piezas, se ensamblan autómatas y se administran sus asignaciones.
- En Taller se calibra, se investiga y se construyen o mejoran instalaciones.
- En Control se utilizan las instalaciones y se administran las rutinas.
- Colas no es una cuarta pantalla principal; es un panel global desplegable.
- La numeración 00–10 es documental y no debe aparecer necesariamente dentro de la interfaz final.

## Correcciones globales pendientes

1. Recomponer todas las subpantallas a 1080×1920. Las tres principales están cerca de 9:16, pero las subpantallas originales son más anchas y no pueden estirarse sin deformación.
2. Crear un único estado de demostración reproducible y obtener todos los números de ese estado. Actualmente recursos, inventario, reservas, asignaciones y niveles de instalaciones no son compatibles entre todas las imágenes.
3. Unificar el encabezado y utilizar siempre los mismos iconos para LE, Trazas y Autómatas.
4. Unificar los iconos de Consola, Diagnóstico, Puerto y Núcleo en todas las vistas.
5. Unificar los iconos de las cuatro colas: Piezas, Ensamble, Planos y Obras.
6. Estandarizar pestañas y navegación:
   - Taller: Calibración, Investigación, Instalaciones.
   - Control: Instalaciones, Rutinas.
7. Definir un comportamiento real para menú, volver, cerrar y ayuda. Las subpantallas actuales del código usan Volver y la pantalla principal tiene Cerrar/Ayuda.
8. Mantener una sola acción de cierre en el panel de Colas; no usar simultáneamente X y Cerrar.
9. Diseñar y validar estados neutral, seleccionado, bloqueado, vacío, activo y completado.
10. Validar posteriormente en Unity a 1080×1920 y 720×1280.

## Correcciones por imagen

### 00 — Planta de producción

- Añadir un selector visible V1–V6.
- Añadir cantidad para Ensamble; las cantidades válidas son 1, 5, 10 y 25.
- Conservar acceso a mejora del Banco de Procesos y asignaciones, aunque se presente desde Autómatas.
- La cancelación puede vivir en el panel global de Colas, siempre que el resumen sea claramente pulsable.

### 01 — Taller / Calibración

- Con `Registradas 1/5`, el código solo revela la primera pieza y la siguiente; no deben aparecer activas las cinco.
- El código calcula un rasgo provisional con su afinidad, no tres medidores independientes. Sustituir las tres barras por el resultado real o ampliar expresamente el sistema.
- El estado compartido muestra Investigación V4, lo cual implica Banco de Procesos N4. Por ello deben existir Cargar perfil, Repetir pieza y Repetir todas.
- Unificar las pestañas con Investigación y Construcción/Mejoras.

### 02 — Sala de control / Núcleo

- `Coordinación ×1.05` es incorrecto: ×1.05 corresponde a eficiencia global.
- Mostrar Capacidad efectiva con un valor numérico. Núcleo N2 con tres rutinas y un perfil necesita capacidad activa de al menos 5.
- Mostrar progreso del requisito MK4 como `0/1` o `1/1` y activar o bloquear el botón en consecuencia.

### 03 — Autómatas

- El panel seleccionado MK2 Rápido debe mostrar total 8, no total global 48.
- La disponibilidad debe descontar autómatas reservados y asignados en cualquier instalación.
- Incorporar paginación, desplazamiento o filtros para todos los MK y rasgos posibles.
- Añadir estado de estabilización de 30 segundos después de asignar.
- Mantener Asignar y Retirar.

### 04 — Investigación

- Control V4, coste 100.000 LE + 100 Trazas, duración 20:00 y potencia mínima 50 son correctos.
- 40 MK1 Normal producen potencia 50 correctamente.
- La interfaz nueva debe leer el equipo reservado desde el trabajo encolado; la interfaz actual borra la selección local después de encolar.
- Mostrar el requisito previo cumplido cuando sea relevante.

### 05 — Construcción y mejoras

- Debe usar el encabezado, pestañas y navegación del Taller, no de Sala de Control.
- Añadir Retirar asignación.
- Mostrar progreso del requisito, por ejemplo `5/5 MK2`.
- El botón de mejora debe reflejar recursos, requisito de ensamblajes y disponibilidad de la cola de Obras, no solamente que la cola esté libre.

### 06 — Consola

- Política, reservas, autorizaciones, Guardar política, Registrar circuito N5 y Abrir rutinas coinciden con el sistema.
- Quitar los denominadores inventados `320/600` y `140/300`. Mostrar únicamente el coste dinámico de la próxima compra.
- Mantener visible que las mejoras automáticas básicas de N3 siguen bloqueadas hasta cerrar su diseño funcional.

### 07 — Diagnóstico

- Los controles mostrados corresponden a N4.
- Localizar los nombres dinámicos de recetas. El código actual compone esos nombres a partir de enumeraciones en inglés.
- El diagrama de nodos necesita un controlador visual vinculado a datos reales; hoy no existe en la interfaz funcional.
- Guardar/Cargar rutina permanece bloqueado hasta N5.

### 08 — Puerto de expedición

- La lógica de Barrido simple, Repetir última, Rutas prioritarias y Ruta segura existe, pero no hay una subpantalla de Puerto funcional propia.
- Enlazar sus botones con el sistema de Automatización y exigir que el patrón manual correspondiente esté aprendido.
- Los destinos con nombres relacionados con naves proceden realmente de Dimensión 1. Si se quieren eliminar también de los textos, debe cambiarse el contenido/código y no solo el arte.

### 09 — Rutinas

Esta pantalla requiere una corrección importante:

- `Fabricar Chasis V1` no está expuesto como rutina aprendible; las acciones de fábrica están marcadas como internas.
- Usar una acción autorizada y aprendida, por ejemplo Barrido simple, Repetir ruta, Comprar Higgs o Comprar Tetraquark.
- `Reserva 200 LE` no es válida. Las opciones actuales son Reserva 0, 50, 100 y 1000 de recurso.
- El límite ×4 no existe. Las opciones reales son sin límite, 1, 5 y 10 ejecuciones.
- Diferenciar visualmente la rutina seleccionada del formulario para crear una rutina nueva.

### 10 — Colas

- Las cuatro colas y sus capacidades son correctas como concepto.
- La interfaz actual solo permite seleccionar una cola y un trabajo; conservar la vista simultánea requiere un controlador nuevo.
- Al cancelar un trabajo activo, la primera pulsación debe advertir que no se devolverán recursos y pedir una segunda confirmación dentro de cinco segundos.
- Los trabajos pendientes sí devuelven todos los recursos.
- Utilizar una sola acción para cerrar el panel.

## Estado coherente de demostración

No volver a introducir los valores visuales manualmente. Debe prepararse un estado QA y enlazar todas las pantallas a él.

El estado anterior de 48 autómatas es incompatible con las instalaciones mostradas:

- Núcleo N1 exige al menos 10 ensamblajes MK2.
- Núcleo N2 exige 5 ensamblajes MK3.
- Diagnóstico N4 exige 1 ensamblaje MK4.
- Además existen autómatas reservados para Investigación y asignados a instalaciones.

Los contadores del encabezado, Autómatas, Investigación, Instalaciones y Rutinas deben provenir del mismo estado.

## Situación actual del proyecto

- La interfaz funcional de D3 sigue construyéndose en `Main.unity` mediante `Dimension3Block1UISetup.cs`.
- Actualmente existen paneles independientes para Fábrica, Calibración, Investigación, Instalaciones, Automatización, Consola, Diagnóstico y Colas.
- La navegación de tres salas de los prototipos todavía no está enlazada con el estado real.
- El prototipo visual declara expresamente que está desconectado de los sistemas de producción.
- Solo `Assets/Project/Scenes/Main.unity` está habilitada en Build Settings.
- El perfil visual de Dimensión 3 sigue pendiente de consolidación.
- Todavía no existen fichas, referencias permanentes ni capturas aprobadas archivadas para estas once composiciones.

## Orden recomendado para continuar

1. Corregir las once referencias sin cambiar todavía el proyecto funcional.
2. Usar un mismo estado QA real para todas.
3. Aprobar una principal, preferiblemente Planta, y documentar con ella el perfil visual D3.
4. Rehacer las demás referencias a 1080×1920 usando los componentes compartidos aprobados.
5. Implementar primero la composición estática en Unity.
6. Conectar navegación y datos reales.
7. Validar estados, 1080×1920 y 720×1280.
8. Solo después incorporar animaciones.

## Frase de continuidad para otro chat

> Continuar la Dimensión 3 desde el paquete `Dimension3_Continuidad_2026-08-19.zip`. Mantener tres salas principales, siete subpantallas y Colas como panel global. Antes de implementar en Unity, corregir las inconsistencias descritas en `RESUMEN_CONTINUIDAD_DIMENSION_3.md`, recomponer todo a 1080×1920 y usar un único estado QA real. No regenerar ni inventar mecánicas, cifras, botones o iconos fuera de lo definido en el código.
