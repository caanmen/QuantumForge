# Instrucciones maestras del sistema UI

Última actualización: 7 de septiembre de 2026

## Propósito

Crear pantallas fieles, estables y reproducibles sin depender de la memoria de un chat. Este documento es el punto de entrada; las guías contienen el detalle.

## Alcance de las reglas y los ejemplos

Todo contenido del sistema debe poder distinguirse mediante una de estas categorías:

- **Regla general:** método funcional, técnico o de calidad transferible a cualquier
  dimensión cuando corresponda.
- **Ejemplo histórico:** caso real que demuestra una regla. Conserva su dimensión y
  pantalla de origen, pero no transmite automáticamente su estética o contenido.
- **Decisión de dimensión:** identidad visual, lenguaje y componentes autorizados sólo
  para el perfil de la dimensión indicada.
- **Decisión de pantalla:** excepción, composición o comportamiento exclusivo de una
  pantalla concreta.

Los ejemplos de Dimensión 1 son evidencia histórica del proceso, no una plantilla
artística universal. Nunca se heredan automáticamente colores, marcos, iconos,
navegación, efectos, vehículos, fondos ni metáforas visuales entre dimensiones.

## Aprendizaje continuo obligatorio

Este sistema debe mejorar con cada pantalla. Cuando aparezca un error nuevo o se
descubra una técnica útil nueva, no debe quedar solamente en el chat:

1. Registrar el hallazgo en `07_REGISTROS/BITACORA_APRENDIZAJES_UI.md`.
2. Si es un error o riesgo repetible, incorporarlo a
   `01_GUIAS/GUIA_PREVENCION_ERRORES_PANTALLAS.txt` con causa, prevención y
   prueba obligatoria.
3. Si es una forma positiva de trabajar mejor, incorporarla a
   `01_GUIAS/GUIA_CREACION_CORRECTA_PANTALLAS.txt` como paso o TIP accionable.
4. Añadir o ajustar la casilla correspondiente en la lista de control de la guía.
5. Registrar la pantalla donde se aprendió y los archivos o pruebas que lo
   demostraron.
6. Indicar el alcance del aprendizaje y qué decisiones del caso de origen no deben
   generalizarse.

No duplicar reglas con palabras distintas. Primero buscar si el aprendizaje amplía
una sección existente; crear una sección nueva solamente cuando sea un problema o
método realmente diferente. Ninguna pantalla se cierra hasta documentar los
aprendizajes nuevos encontrados durante su construcción.

## Autoridad

1. Petición actual y explícita del usuario.
2. Diseño canónico aprobado y referencias identificadas.
3. `REGLAS_DE_TRABAJO_Y_CONTINUIDAD.md`.
4. Las dos guías centrales de UI.
5. Perfiles de estilo, fichas y registros de decisiones.
6. Estado técnico real del proyecto.

Si dos fuentes se contradicen, detener únicamente la parte afectada y explicarlo antes de fijar una decisión creativa.

## Apertura de un trabajo UI

1. Identificar dimensión, pantalla y función.
2. Leer ambas guías completas.
3. Leer el perfil de estilo de la dimensión.
4. Separar las reglas generales de los ejemplos históricos y de las decisiones
   exclusivas de otras dimensiones.
5. Localizar la referencia permanente y confirmar su clasificación y estado.
6. Completar una copia de `02_PLANTILLAS/FICHA_NUEVA_PANTALLA.txt` dentro de la carpeta de la pantalla.
7. Ejecutar la Puerta 0 de auditoría y reutilización. Buscar en el catálogo, el mapa de
   reutilización, las escenas, prefabs, scripts, assets y capturas anteriores antes de
   autorizar cualquier recurso nuevo.
8. Registrar por separado assets reutilizados, candidatos, faltantes y sustituciones
   autorizadas. Una ausencia en el catálogo no demuestra que el asset no exista.
9. Ejecutar el preflight descrito en `08_PRUEBAS/PLAN_MAESTRO_PRUEBAS_UI.txt`.

## Convivencia segura de varios proyectos de Unity

Pueden permanecer abiertas varias instancias de Unity si cada una trabaja sobre una ruta
de proyecto diferente. La unidad de seguridad es la carpeta absoluta del proyecto, no el
nombre del proceso, la versión del Editor ni la cantidad de proyectos mostrados por Unity
Hub.

- Resolver y registrar la ruta absoluta del proyecto objetivo antes de automatizar.
- No abrir nunca dos instancias normales o batch sobre la misma carpeta.
- Para reconstruir, validar o capturar `Quantum Forge` por batch, comprobar que esa misma
  carpeta no esté abierta en el Editor normal.
- No pedir cerrar editores de otros proyectos independientes: no comparten `Assets`,
  `Library`, `ProjectSettings`, `Packages` ni `Temp` con el objetivo.
- Antes de escribir externamente escenas, prefabs, assets serializados o archivos `.meta`,
  pedir cerrar únicamente la instancia que usa el proyecto objetivo.
- No interpretar la lista de Unity Hub como prueba de que los proyectos están abiertos;
  comprobar procesos asociados a la ruta y señales del propio proyecto, como su bloqueo
  activo, y descartar bloqueos obsoletos antes de concluir.

## Puerta 0 — Auditoría y reutilización obligatoria

Esta puerta aplica a todo Quantum Forge, sin importar dimensión, pantalla o sistema.
Debe completarse antes de diseñar o reconstruir la composición.

- Inventariar cada concepto visible: marco, fondo, icono, ilustración, botón, navegación,
  efecto, tipografía, componente y patrón de interacción.
- Buscar primero su aparición anterior en el proyecto real, el catálogo, el mapa de
  reutilización y las capturas aprobadas.
- Registrar una matriz `concepto -> origen -> ruta exacta -> estado -> decisión`.
- Si el mismo concepto ya tiene un asset o componente aprobado dentro del alcance
  autorizado, reutilizarlo. No dibujar una aproximación ni crear una copia desde cero.
- Si existe sólo un candidato, conservar su clasificación y validar su uso en la nueva
  pantalla; reutilizarlo no lo vuelve canónico automáticamente.
- Si no existe recurso adecuado, registrar el faltante antes de producirlo y crearlo
  con la referencia y el perfil de la dimensión como autoridad.
- No reutilizar automáticamente estética entre dimensiones. La reutilización técnica
  sólo es válida si el alcance COMUN o el perfil de la dimensión la autoriza.

La Puerta 0 falla si la matriz está vacía, si se afirma que “no existe” sin buscar en el
proyecto real o si se propone recrear un concepto ya resuelto sin justificarlo.

## Regla global contra la simplificación silenciosa

Un diseño, composición o asset ya aprobado define también su ambición artística, medio
gráfico, densidad, proporciones, volumen y nivel de detalle. Implementarlo no autoriza a
reducirlo a wireframe, pictograma, placeholder, esquema o versión genérica.

- La fidelidad se conserva desde la primera composición estática, no se promete para
  una corrección futura.
- Una limitación técnica puede justificar optimización, pero no una pérdida visual
  silenciosa. La diferencia debe medirse, documentarse y aprobarse explícitamente.
- Si el recurso original no funciona a tamaño móvil, se prepara una variante del mismo
  concepto que conserve identidad, masas principales y acabado; no se sustituye por un
  símbolo diferente.
- No declarar ni sugerir 95 % mientras falten assets, detalle o composición esenciales.

## Puertas de aprobación

### Puerta 0 — Auditoría y reutilización

- Matriz de reutilización completa y rutas verificadas en el proyecto real.
- Assets aprobados reutilizados antes de listar faltantes.
- Candidatos y recursos nuevos claramente clasificados.
- Simplificaciones prohibidas y diferencias autorizadas registradas.

### Puerta 1 — Contrato

- Función, datos, estados, navegación, assets y referencia definidos.
- Intención de animación definida antes de producir assets: capas, pivotes, estados de
  inicio y fin, elementos inmóviles y objetivo informativo o ambiental.
- Sin decisiones creativas importantes pendientes.

### Puerta 2 — Composición estática

- Estructura completa a 1080×1920.
- Proporciones, zonas seguras, tipografía, iconos y capas revisados.
- Comparación visual antes de añadir movimiento.
- Todo defecto visual marcado por el usuario se reabre como caso individual y debe cerrarse
  con un recorte de una captura nueva de Unity que incluya el marco propietario completo.
- En ese recorte se comprueban por separado eje X, eje Y y los cuatro márgenes visibles. La
  pantalla completa reducida, las coordenadas del Inspector y un PASS automático no sustituyen
  esta comprobación.
- No se puede informar «corregido» si la evidencia mostrada no corresponde a la última
  reconstrucción o si sólo demuestra que el RectTransform cabe técnicamente.
- La composición estática se aprueba sin implementar todavía la animación; las capas
  necesarias ya deben estar preparadas para evitar rehacer el arte.

### Puerta 3 — Datos e interacción

- Datos reales y propietarios únicos.
- Estados e interacciones inmediatas validados.
- Navegación de entrada, salida y botón Atrás comprobados.

### Puerta 4 — Animación

- Sólo comienza después de aprobar la composición estática y conectar los estados que
  deban controlarla.
- Pivotes centrados y posiciones base registradas.
- Animación aplicada únicamente a capas preparadas.
- Evidencia renderizada; no basta inspeccionar fórmulas.

## Regla de planificación de animación

La animación se **diseña antes de Unity** y se **implementa después de aprobar la
composición estática**. Planearla temprano no autoriza a animar temprano: sirve para
separar assets, reservar márgenes, elegir pivotes y evitar hornear en una sola imagen
partes que necesitarán comportamientos diferentes.

### Puerta 5 — Cierre

- Capturas nuevas neutral/seleccionada/bloqueada cuando correspondan.
- Recortes de cierre para todos los defectos visuales señalados durante la revisión, tomados
  de la misma ejecución que la captura final.
- Pruebas sin errores ni excepciones.
- Resoluciones objetivo comprobadas.
- Cuando exista build representativa, tamaño y dependencias comparados contra la entrega
  anterior; conceptos, referencias y candidatos no utilizados excluidos de producción.
- Diferencias restantes registradas y similitud visual mínima aprobada del 95 %.
- El PASS técnico y la conformidad visual del usuario se registran como decisiones distintas;
  ninguno implica automáticamente el otro.
- Referencia, captura y decisión archivadas.

## Estilos por dimensión

Las reglas funcionales y de calidad son comunes. El aspecto no lo es.

- `03_ESTILOS/COMUN`: contratos compartidos, sin imponer una estética.
- `03_ESTILOS/DIMENSION_1`: perfil derivado de pantallas aprobadas de Dimensión 1.
- Cada dimensión adicional construye su propio perfil a partir de su diseño canónico y
  de capturas reales aprobadas. Un perfil pendiente no hereda el de otra dimensión.

No copiar automáticamente colores, marcos o animaciones entre dimensiones.

## Fuentes permanentes

- Referencias: `05_REFERENCIAS/<DIMENSION>/<PANTALLA>`.
- Capturas reales aprobadas: `06_CAPTURAS_APROBADAS/<DIMENSION>/<PANTALLA>`.
- Decisiones: `07_REGISTROS/DECISIONES_UI.md`.
- Estado de pantallas: `07_REGISTROS/PANTALLAS_UI.csv`.
- Assets: `04_CATALOGO_ASSETS/CATALOGO_ASSETS_UI.csv`.
- Método y mapa de origen: `04_CATALOGO_ASSETS/MAPA_REUTILIZACION_Y_ORIGEN_UI.md`.

## Regla de reconstrucción

Si una pantalla se genera mediante un script de Editor, toda corrección aprobada debe guardarse en ese generador y después reconstruirse. Una edición aislada de la escena no es la fuente definitiva.
