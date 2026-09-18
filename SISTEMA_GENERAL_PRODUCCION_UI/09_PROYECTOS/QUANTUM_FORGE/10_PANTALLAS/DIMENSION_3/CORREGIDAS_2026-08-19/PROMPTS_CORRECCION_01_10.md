# Prompt final consolidado — correcciones D3 01–10

Modo: herramienta integrada de edición de imágenes. Caso: `precise-object-edit` y,
para erratas aisladas, `text-localization`.

## Invariantes usados en todas las ediciones

> Preservar la composición, maquinaria, perspectiva, fondo, iluminación, materiales,
> jerarquía, marcos y navegación de la imagen objetivo. Igualar el encabezado, la
> familia iconográfica y el lenguaje de acero ennegrecido, latón envejecido, ámbar y
> cian de la Planta de Producción aprobada. Corregir sólo nombres, cifras, controles,
> estados e iconos exigidos por el código real. No inventar mecánicas, monedas,
> botones, lore ni assets. Composición estática, sin movimiento ni marca de agua.

## Encabezado y sistema compartido

Texto literal: `3.25M LE`, `4.2K TRAZAS`, `56 AUTÓMATAS`.
Iconos: engranaje de LE, chip cian de Trazas y cabeza simple de autómata.
Colas: cubos/caja para PIEZAS, cabeza para ENSAMBLE, documento técnico para PLANOS y
grúa para OBRAS. Navegación: PLANTA / TALLER / CONTROL.

## Especificaciones por pantalla

1. Taller/Calibración: nombres completos de cinco piezas; tres secciones reales;
   35/40/25 = 100; 1/5 registradas; Registrar Chasis; perfil requiere 5/5.
2. Sala de Control/Núcleo: niveles N2/N4/N1/N2; símbolos terminal/cruz/ancla/módulo;
   Núcleo N3 cuesta 2.000.000 + 1.500, 45:00, requiere un MK4.
3. Autómatas: grupos 40/10/5/1; MK2 Rápido seleccionado; 10 total, 8 disponibles,
   2 asignados y estables a Capacidad de Consola; Asignar +1 / Retirar -1.
4. Investigación: Módulo de Control V4; coste pagado 100.000 + 100; 20:00;
   potencia 50; equipo 40 MK1 Normal; cancelar investigación.
5. Mejoras: Consola N2 a N3; 250.000 + 250; 15:00; 5 MK2; Capacidad con 2 MK2
   Rápidos estables; Ampliar a nivel 3.
6. Consola: Equilibrio; reservas 10.000/100; Higgs y Tetraquarks autorizados;
   Guardar política; circuito requiere N5; Abrir Rutinas.
7. Diagnóstico: N4; Autoanálisis ON; Autorreparación ON; Autofusión OFF; Prioridad
   por zona / Sector de Fusión; reservas 10.000/100; guardar/cargar requiere N5.
8. Puerto: N1; enlace D1 activo; patrón aprendido; cuatro destinos; Barrido simple,
   Repetir ruta y Abrir Rutinas; N2/N3 bloqueados.
9. Rutinas: Barrido simple; Prioridad 3; Reserva 100; límite 5; rutina activa 1/3;
   crear, pausar, eliminar, guardar y cargar perfil.
10. Colas: un único cierre y una única cancelación; Chasis V1 activo, Chasis V2 y
    Sistema Motriz V1 pendientes, MK1 Normal y Módulo de Control V4 activos;
    advertencia de cancelación activa sin devolución y doble confirmación en 5 s.

## Correcciones localizadas finales

- Autómatas: restaurar engranaje de LE y mantener iconos canónicos de colas.
- Investigación: `100.000 LE`, sin coma.
- Puerto: `NAVE ABANDONADA`.
- Colas: documento técnico para Planos y grúa para Obras.

Las salidas originales de la herramienta se conservaron como `*_GENERADA.png`. Las
entregas se normalizaron sin deformación a 1080×1920 y 720×1280.
