# Auditoría integral de Dimensión 1 — 20 de agosto de 2026

## Estado de este documento

- Auditoría de solo lectura realizada sobre código, escena, constructores, fichas, contratos,
  capturas y registros reales del proyecto.
- Los errores de esta lista todavía no se consideran corregidos salvo que una entrega posterior
  añada evidencia nueva y actualice expresamente este estado.
- No autoriza inventar planetas, destinos, naves, efectos, valores ni assets.

## Catálogo canónico confirmado

- 7 planetas: Borde 1–2, Anillo 3, Órbitas 4–5 y Frontera 6–7.
- 4 sectores explorables más Centro Galáctico.
- 10 tipos de destino activos distribuidos en 16 slots sectoriales.
- 4 naves activas en Parte 1. Rescate y Convergencia permanecen reservadas para Parte 2.
- 10 metales, 20 reliquias y 10 nodos del Árbol.
- No crear planetas 8+, destinos provisionales ni las dos naves de Parte 2 para llenar huecos.

## Problemas confirmados de prioridad crítica

### 1. Carta Galáctica: parpadeo «Entrar» en los cuatro sectores

`SelectedData` completo funciona como Button y su primer TMP es el contador de expediciones.
`SetButtonText` escribe temporalmente «Entrar» o «Volver al sector» en ese contador; el
controlador visual restaura el número en el siguiente refresco de 0,25 s. El error pertenece al
componente compartido, por lo que afecta Borde, Anillo, Órbitas y Frontera.

### 2. Explorar: bloqueo al conservar destinos de otro sector

El conteo de destinos disponibles no filtra por sector. Al cambiar de sector con resultados
pendientes, el CTA intenta iniciar con un destino del contexto anterior, el sistema lo rechaza y
la UI ya no ofrece escanear. Se debe filtrar/invalidar por sector y conservar una ruta de reescaneo.

### 3. Explorar: ciclo con el preview legacy

Elegir destino y nave activa automáticamente `ExplorationRewardsPanel`. Ese panel está marcado
como bloqueador del skin moderno; el skin desaparece y cerrar el preview borra las selecciones.
El recorrido seleccionar -> preview legacy -> ocultar skin -> cerrar -> perder selección impide
llegar de forma normal al CTA moderno.

### 4. ARK: subpantalla funcional sin ruta jugable moderna demostrada

Las pruebas abren ARK mediante llamadas directas. Entrar al Centro Galáctico desde Carta no abre
ARK y el único botón legacy queda debajo de la composición moderna del Centro de Mando. Se debe
crear o restaurar la ruta aprobada y probarla Carta -> Centro -> ARK con clics físicos.

## Problemas funcionales y visuales importantes

- Los selectores de destino, nave, apoyo y expedición activa existen, pero usan hitboxes
  transparentes de 42×42; en 720×1280 quedan cerca de 28 px y son poco descubribles.
- Cambiar nave modifica ID, texto y mecánica, pero no la ilustración fija del skin.
- El skin moderno no muestra toda la información legacy: duración, recompensas previstas,
  sinergia completa, punto especial y datos revelados por Lectura de Destinos.
- Las tarjetas sectoriales muestran cuatro destinos, pero si no fueron escaneados sólo quedan
  deshabilitadas y no explican el estado.
- Mejorar el escáner no guarda inmediatamente; depende del autosave posterior.
- Si varias expediciones terminan antes del mismo refresco, el Registro conserva todas, pero el
  modal parece mostrar únicamente la última. Falta reproducción runtime antes de clasificar la
  cola de resultados como error confirmado.

## Planetas y composición sectorial

- Frontera contiene P6 y P7 activos, pero ambos usan `d1_body_planet_silent_v3.png` con tintes
  diferentes. Pueden parecer duplicados o dar la impresión de que falta uno. No cambiar el asset
  sin una decisión o candidato aprobado.
- Anillo posee un único planeta real. Debe adaptar y centrar la composición sin inventar un
  segundo planeta ni conservar grandes vacíos por reutilizar mecánicamente el layout doble.
- El shell principal usa una geometría; Órbitas otra; Borde/Anillo/Frontera fuerzan una raíz
  vertical distinta y ARK usa otro tamaño de marco. Los márgenes todavía no son uniformes.

## Duplicidad, constructores y evidencia

- La escena contiene una sola raíz visual actual por pantalla de producción; no hay duplicados
  activos exactos por nombre.
- Sí permanecen capas legacy como autoridad funcional oculta. Deben tener propiedad explícita de
  visibilidad y no interceptar raycasts ni competir con el skin.
- Carta Galáctica conserva tres reconstructores ejecutables V2, V10 y V11 sobre la misma raíz.
  V11 es el canónico actual; los anteriores pueden reemplazar composición o conexiones.
- Sólo Hangar conserva una aprobación visual actual confiable. Carta tiene evidencia aprobada
  anterior a cambios posteriores del constructor/escena y necesita revalidación. Las demás
  pantallas siguen como candidatas o pendientes del 95 %.
- Las capturas antiguas con navegaciones inferiores distintas no representan necesariamente la
  escena actual; el sistema premium compartido ya existe y debe verificarse con capturas nuevas.

## Orden de corrección recomendado

1. Romper el ciclo preview legacy / skin moderno de Explorar.
2. Corregir el parpadeo del contador en los cuatro sectores.
3. Filtrar o invalidar destinos al cambiar sector y recuperar el reescaneo.
4. Restaurar y validar la ruta real hacia ARK.
5. Hacer visibles los selectores y sincronizar arte/texto/mecánica de cada nave.
6. Reintegrar previews, punto especial, sinergias y motivos de bloqueo reales.
7. Unificar márgenes y marcos de sectores y ARK con la autoridad visual vigente.
8. Adaptar Anillo y resolver la diferenciación de P6/P7 sólo con assets aprobados.
9. Retirar reconstructores antiguos y actualizar el registro maestro.
10. Ejecutar una matriz completa de rutas mediante botones físicos, guardado y carga, estados
    inmediatos y posteriores al refresh, 1080×1920 y 720×1280.

## Criterio de cierre

No cerrar Dimensión 1 hasta que las rutas del jugador, no sólo los métodos y capturas aisladas,
tengan PASS; cada pantalla conserve un único propietario visual/funcional; la evidencia sea
posterior al código actual; y el usuario apruebe la similitud perceptual mínima del 95 %.
